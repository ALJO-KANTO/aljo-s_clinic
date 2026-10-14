using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsPrescriptionData
    {
        public static int SavePrescription(int? VisitID, string Notes, DateTime Date, DataTable dtMedicines, byte PrescriptionStatus, byte Prescriptiontype)
        {
            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. حفظ رأس الوصفة واسترجاع الـ ID
                        string queryMaster = @"INSERT INTO Prescriptions (VisitID, PrescriptionDate, PrescriptionNotes,PrescriptionStatus,Prescriptiontype) 
                                      VALUES (@VisitID, @Date, @Notes,@PrescriptionStatus,@Prescriptiontype);
                                      SELECT LAST_INSERT_ID();";

                        int prescriptionID = -1;
                        using (MySqlCommand cmdMaster = new MySqlCommand(queryMaster, connection, transaction))
                        {
                            cmdMaster.Parameters.AddWithValue("@VisitID", (object)VisitID ?? DBNull.Value);
                            cmdMaster.Parameters.AddWithValue("@Date", Date);
                            cmdMaster.Parameters.AddWithValue("@Notes", Notes.ToDBValue());
                            cmdMaster.Parameters.AddWithValue("@PrescriptionStatus", PrescriptionStatus);
                            cmdMaster.Parameters.AddWithValue("@Prescriptiontype", Prescriptiontype);

                            prescriptionID = Convert.ToInt32(cmdMaster.ExecuteScalar());
                        }

                        dtMedicines.Columns.Add("PrescriptionID", typeof(int), prescriptionID.ToString());

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "PrescriptionDetails";

                            // ربط الأعمدة (يجب أن تتطابق مع أسماء الأعمدة في DataTable و SQL)
                            bulkCopy.ColumnMappings.Add("PrescriptionID", "PrescriptionID");
                            bulkCopy.ColumnMappings.Add("MedicineID", "MedicineID");
                            bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                            bulkCopy.ColumnMappings.Add("Dosage", "Dosage");
                            bulkCopy.ColumnMappings.Add("Instructions", "Instructions");
                            bulkCopy.ColumnMappings.Add("SavedMedicineName", "SavedMedicineName");
                            bulkCopy.ColumnMappings.Add("SavedMedicinePrice", "SavedMedicinePrice");
                            bulkCopy.ColumnMappings.Add("Frequency", "Frequency");
                            bulkCopy.ColumnMappings.Add("DiscountAmount", "DiscountAmount");
                            bulkCopy.WriteToServer(dtMedicines);
                        }
                        transaction.Commit();
                        return prescriptionID;
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();

                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                        return -1;

                    }
                }
            }
        }

        public static bool UpdatePrescription(int PrescriptionID, int? VisitID, string Notes, DateTime Date, byte PrescriptionStatus, DataTable dtMedicines)
        {
            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string queryMaster = @"UPDATE Prescriptions 
                                       SET PrescriptionDate = @Date, 
                                           PrescriptionNotes = @Notes ,
                                            PrescriptionStatus=@PrescriptionStatus
                                       WHERE PrescriptionID = @PrescriptionID";

                        using (MySqlCommand cmdMaster = new MySqlCommand(queryMaster, connection, transaction))
                        {
                            cmdMaster.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                            cmdMaster.Parameters.AddWithValue("@Date", Date);
                            cmdMaster.Parameters.AddWithValue("@Notes", Notes.ToDBValue());
                            cmdMaster.Parameters.AddWithValue("@PrescriptionStatus", PrescriptionStatus);

                            cmdMaster.ExecuteNonQuery();
                        }

                        // 2. حذف التفاصيل القديمة (عن طريق الـ PrescriptionID)
                        string queryDeleteDetails = "DELETE FROM PrescriptionDetails WHERE PrescriptionID = @PrescriptionID";
                        using (MySqlCommand cmdDelete = new MySqlCommand(queryDeleteDetails, connection, transaction))
                        {
                            cmdDelete.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                            cmdDelete.ExecuteNonQuery();
                        }


                        if (!dtMedicines.Columns.Contains("PrescriptionID"))
                            dtMedicines.Columns.Add("PrescriptionID", typeof(int), PrescriptionID.ToString());

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "PrescriptionDetails";

                            bulkCopy.ColumnMappings.Add("PrescriptionID", "PrescriptionID");
                            bulkCopy.ColumnMappings.Add("MedicineID", "MedicineID");
                            bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                            bulkCopy.ColumnMappings.Add("Dosage", "Dosage");
                            bulkCopy.ColumnMappings.Add("Instructions", "Instructions");
                            bulkCopy.ColumnMappings.Add("SavedMedicineName", "SavedMedicineName");
                            bulkCopy.ColumnMappings.Add("SavedMedicinePrice", "SavedMedicinePrice");
                            bulkCopy.ColumnMappings.Add("Frequency", "Frequency");
                            bulkCopy.ColumnMappings.Add("DiscountAmount", "DiscountAmount");
                            bulkCopy.WriteToServer(dtMedicines);
                        }


                        transaction.Commit();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();

                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                        return false;

                    }
                }
            }
        }

        public static int SavePrescription(int VisitID, string Notes, DateTime Date, DataTable dtMedicines, byte Prescriptiontype, MySqlTransaction transaction)
        {
            string queryMaster = @"INSERT INTO Prescriptions (VisitID, PrescriptionDate, PrescriptionNotes, PrescriptionStatus, Prescriptiontype) 
                           VALUES (@VisitID, @Date, @Notes, 1, @Prescriptiontype);
                           SELECT LAST_INSERT_ID();";

            int prescriptionID = -1;
            using (MySqlCommand cmdMaster = new MySqlCommand(queryMaster, transaction.Connection, transaction))
            {
                cmdMaster.Parameters.AddWithValue("@VisitID", VisitID);
                cmdMaster.Parameters.AddWithValue("@Date", Date);
                cmdMaster.Parameters.AddWithValue("@Notes", Notes.ToDBValue());
                cmdMaster.Parameters.AddWithValue("@Prescriptiontype", Prescriptiontype);

                prescriptionID = Convert.ToInt32(cmdMaster.ExecuteScalar());
            }

            dtMedicines.Columns.Add("PrescriptionID", typeof(int), prescriptionID.ToString());

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.DestinationTableName = "PrescriptionDetails";
                bulkCopy.ColumnMappings.Add("PrescriptionID", "PrescriptionID");
                bulkCopy.ColumnMappings.Add("MedicineID", "MedicineID");
                bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                bulkCopy.ColumnMappings.Add("Dosage", "Dosage");
                bulkCopy.ColumnMappings.Add("Instructions", "Instructions");
                bulkCopy.ColumnMappings.Add("SavedMedicineName", "SavedMedicineName");
                bulkCopy.ColumnMappings.Add("SavedMedicinePrice", "SavedMedicinePrice");
                bulkCopy.ColumnMappings.Add("Frequency", "Frequency");
                bulkCopy.ColumnMappings.Add("DiscountAmount", "DiscountAmount");
                bulkCopy.WriteToServer(dtMedicines);
            }

            return prescriptionID;
        }

        public static bool UpdatePrescription(int PrescriptionID, int VisitID, string Notes, DateTime Date, DataTable dtMedicines, MySqlTransaction transaction)
        {
            if (!IsPrescriptionPending(PrescriptionID))
                return false;
            string queryMaster = @"UPDATE Prescriptions
                            SET PrescriptionDate = @Date,
                            PrescriptionNotes = @Notes
                           WHERE PrescriptionID = @PrescriptionID";


            using (MySqlCommand cmdMaster = new MySqlCommand(queryMaster, transaction.Connection, transaction))
            {
                cmdMaster.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                cmdMaster.Parameters.AddWithValue("@Date", Date);
                cmdMaster.Parameters.AddWithValue("@Notes", Notes.ToDBValue());
                cmdMaster.ExecuteNonQuery();
            }

            // حذف التفاصيل القديمة
            using (MySqlCommand cmdDelete = new MySqlCommand("DELETE FROM PrescriptionDetails WHERE PrescriptionID = @PrescriptionID", transaction.Connection, transaction))
            {
                cmdDelete.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                cmdDelete.ExecuteNonQuery();
            }

            if (!dtMedicines.Columns.Contains("PrescriptionID"))
                dtMedicines.Columns.Add("PrescriptionID", typeof(int), PrescriptionID.ToString());

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.DestinationTableName = "PrescriptionDetails";

                bulkCopy.ColumnMappings.Add("PrescriptionID", "PrescriptionID");
                bulkCopy.ColumnMappings.Add("MedicineID", "MedicineID");
                bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                bulkCopy.ColumnMappings.Add("Dosage", "Dosage");
                bulkCopy.ColumnMappings.Add("Instructions", "Instructions");
                bulkCopy.ColumnMappings.Add("SavedMedicineName", "SavedMedicineName");
                bulkCopy.ColumnMappings.Add("SavedMedicinePrice", "SavedMedicinePrice");
                bulkCopy.ColumnMappings.Add("Frequency", "Frequency");
                bulkCopy.ColumnMappings.Add("DiscountAmount", "DiscountAmount");
                bulkCopy.WriteToServer(dtMedicines);
            }

            return true;
        }

        public static bool DeletePrescription(int PrescriptionID, MySqlTransaction transaction)
        {
            string queryDeleteDetails = "DELETE FROM PrescriptionDetails WHERE PrescriptionID = @PrescriptionID";
            using (MySqlCommand cmdDelete = new MySqlCommand(queryDeleteDetails, transaction.Connection, transaction))
            {
                cmdDelete.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                cmdDelete.ExecuteNonQuery();
            }

            string queryDelete = "DELETE FROM Prescriptions WHERE PrescriptionID = @PrescriptionID";
            using (MySqlCommand cmdMaster = new MySqlCommand(queryDelete, transaction.Connection, transaction))
            {
                cmdMaster.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                cmdMaster.ExecuteNonQuery();
            }
            return true;
        }
        public static DataTable GetVisitMedicines(int? VisitID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT
                                    PPD.MedicineID, 
                                    PPD.SavedMedicineName, 
                                    PPD.Dosage, 
                                    PPD.Instructions,
                                    PPD.SavedMedicinePrice, 
                                    PPD.Quantity,
                                    PPD.Frequency,
                                    PPD.DiscountAmount,
                                    M.TaxRate
                                FROM PrescriptionS PP
                                INNER JOIN PrescriptionDetails PPD ON 
                                 PP.PrescriptionID = PPD.PrescriptionID
                                  INNER JOIN Medicines M ON PPD.SavedMedicineName=M.MedicineName
                                WHERE PP.VisitID = @VisitID;";


                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VisitID", VisitID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return dt;
        }

        public static DataTable GetAllActivePrescriptions()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT 
                                        P.PrescriptionID,
                                        ISNULL(P.VisitID, -1) AS VisitID,
                                        ISNULL(V.AppointmentID, -1) AS AppointmentID, 
                                        ISNULL(PP.FullName, 'Not Found') AS PatientName,
                                        ISNULL(PD.FullName, 'Pharmacy') AS DoctorName,
                                       FORMAT(P.PrescriptionDate, 'HH:mm') AS PrescriptionTime,
                                        CASE P.PrescriptionStatus
                                            WHEN 1 THEN 'Pending'
                                            WHEN 2 THEN 'Waiting For Payment'
                                            WHEN 3 THEN 'Ready For Dispensing'
                                            WHEN 5 THEN 'Partially Dispensed'
                                            ELSE 'Unknown'
                                        END AS Status,
                                        CASE P.PrescriptionType
                                            WHEN 1 THEN 'Doctor Prescription'
                                            WHEN 2 THEN 'Pharmacy Direct'
                                            ELSE 'Unknown'
                                        END AS PrescriptionType
                                    FROM Prescriptions AS P
                                    LEFT JOIN Visits AS V ON P.VisitID = V.VisitID
                                    LEFT JOIN Patients AS Pat ON V.PatientID = Pat.PatientID
                                    LEFT JOIN Persons AS PP ON Pat.PersonID = PP.PersonID
                                    LEFT JOIN Doctors AS D ON V.DoctorID = D.DoctorID
                                    LEFT JOIN Persons AS PD ON D.PersonID = PD.PersonID
                                    WHERE P.PrescriptionStatus NOT IN (4, 6);";


                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return dt;
        }

        public static DataTable GetAllPrescriptionDetails()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT 
                                            P.PrescriptionStatus,
                                            PD.PrescriptionID,
                                            PD.PrescriptionDetailsID,
                                            PD.SavedMedicineName AS MedicineName,
                                            PD.Dosage,
                                            M.TaxRate,
                                            PD.SavedMedicinePrice,
                                            PD.Frequency,
                                            PD.Quantity AS RequiredQuantity,
                                            PD.Instructions,
                                            PD.DiscountAmount,

                                            -- منطق ذكي للكمية المصروفة
                                            CASE 
                                                WHEN P.PrescriptionStatus = 1 THEN 
                                                    (CASE 
                                                        WHEN M.CurrentStock IS NULL OR M.CurrentStock <= 0 THEN 0
                                                        WHEN M.CurrentStock >= PD.Quantity THEN PD.Quantity
                                                        ELSE M.CurrentStock 
                                                    END)
                                                ELSE PD.DispensedQuantity 
                                            END AS DispensedQuantity,

                                            -- منطق ذكي للـ CheckBox
                                            CASE 
                                                WHEN P.PrescriptionStatus = 1 THEN 
                                                    (CASE 
                                                        WHEN M.CurrentStock IS NULL OR M.CurrentStock <= 0 THEN 0
                                                        ELSE 1 
                                                    END)
                                                ELSE PD.IsDispensed 
                                            END AS IsDispensed,

                                            -- منطق ذكي لحالة التوفر
                                            CASE 
                                                WHEN P.PrescriptionStatus = 1 THEN
                                                    (CASE 
                                                        WHEN M.CurrentStock IS NULL OR M.CurrentStock <= 0 THEN 'Out of Stock'
                                                        WHEN M.CurrentStock >= PD.Quantity THEN 'Fully Available'
                                                        ELSE 'Partially Available (' + CAST(M.CurrentStock AS VARCHAR) + ')'
                                                    END)
                                                ELSE 'Finalized'
                                            END AS AvailableStatus

                                        FROM PrescriptionDetails PD
                                        LEFT JOIN Prescriptions P ON PD.PrescriptionID = P.PrescriptionID
                                        LEFT JOIN Medicines M ON PD.SavedMedicineName = M.MedicineName;";





























                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return dt;
        }

        public static DataTable GetAllPrescriptionRecords()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM View_PrescriptionDetails;";


                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return dt;
        }

        public static DataTable GetPrescriptionItemsRaw(int PrescriptionID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT 
                                      --  PrescriptionID,
                                        SavedMedicineName AS MedicineName,
                                        Dosage,
                                        Frequency,
                                        Quantity,
                                        Instructions,
                                        SavedMedicinePrice As Price,
                                        DiscountAmount
                                    FROM PrescriptionDetails
                                        Where PrescriptionID=@PrescriptionID";


                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return dt;
        }


        public static bool Find(int VisitID, ref int PrescriptionID, ref string PrescriptionNotes, ref DateTime PrescriptionDate, ref byte PrescriptionStatus)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT PrescriptionID,
                                            PrescriptionDate,
                                          PrescriptionNotes,
                                            PrescriptionStatus
                                      FROM  Prescriptions
                                      WHERE VisitID=@VisitID;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VisitID", VisitID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PrescriptionID = Convert.ToInt32(reader["PrescriptionID"]);
                                PrescriptionDate = Convert.ToDateTime(reader["PrescriptionDate"]);
                                PrescriptionNotes = reader["PrescriptionNotes"].ToStringOrEmpty();
                                string status = Convert.ToString(reader["PrescriptionStatus"]);

                                byte numericStatus;

                                if (byte.TryParse(status, out numericStatus))
                                {
                                    PrescriptionStatus = numericStatus;
                                }
                                else
                                {
                                    switch (status?.Trim().ToLowerInvariant())
                                    {
                                        case "pending":
                                            PrescriptionStatus = 1;
                                            break;

                                        case "waiting for payment":
                                            PrescriptionStatus = 2;
                                            break;

                                        case "ready for dispensing":
                                            PrescriptionStatus = 3;
                                            break;

                                        case "dispensed":
                                            PrescriptionStatus = 4;
                                            break;

                                        case "partially dispensed":
                                            PrescriptionStatus = 5;
                                            break;

                                        default:
                                            PrescriptionStatus = 0;
                                            break;
                                    }
                                }

                                return true;

                            }
                            else
                                return false;
                        }
                    }

                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }
        }


        public static bool UpdatePrescriptionStatus(int prescriptionId, int newStatus, MySqlTransaction transaction)
        {
            string query = @"UPDATE Prescriptions 
                     SET PrescriptionStatus = @Status 
                     WHERE PrescriptionID = @PrescriptionID";

            using (MySqlCommand command = new MySqlCommand(query, transaction.Connection, transaction))
            {
                command.Parameters.AddWithValue("@PrescriptionID", prescriptionId);
                command.Parameters.AddWithValue("@Status", newStatus);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdatePrescriptionStatusByBill(
    int billId,
    int newStatus,
    MySqlTransaction transaction)
        {
            if (transaction == null || transaction.Connection == null)
                throw new ArgumentNullException(nameof(transaction));

            string statusText;

            switch (newStatus)
            {
                case 1:
                    statusText = "Pending";
                    break;

                case 2:
                    statusText = "Waiting For Payment";
                    break;

                case 3:
                    statusText = "Ready For Dispensing";
                    break;

                case 4:
                    statusText = "Dispensed";
                    break;

                case 5:
                    statusText = "Partially Dispensed";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(newStatus),
                        "Invalid prescription status.");
            }

            string query = @"
        UPDATE prescriptions P
        INNER JOIN bills B
            ON P.VisitID = B.VisitID
        SET P.PrescriptionStatus = @Status
        WHERE B.BillID = @BillID
          AND P.PrescriptionStatus IN
          (
              'Pending',
              'Waiting For Payment',
              '1',
              '2'
          );";

            using (MySqlCommand command = new MySqlCommand(
                query,
                transaction.Connection,
                transaction))
            {
                command.Parameters.AddWithValue("@BillID", billId);
                command.Parameters.AddWithValue("@Status", statusText);

                command.ExecuteNonQuery();
            }

            return true;
        }
        public static bool UpdatePrescriptionStatus(int prescriptionId, byte newStatus)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Prescriptions 
                         SET PrescriptionStatus = @Status 
                         WHERE PrescriptionID = @PrescriptionID";

                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@PrescriptionID", prescriptionId);
                        command.Parameters.AddWithValue("@Status", newStatus);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }
        }

        public static decimal GetPrescriptionTotalForCashier(int prescriptionId)
        {
            decimal total = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT SUM((Quantity * SavedMedicinePrice) - DiscountAmount) 
                         FROM PrescriptionDetails 
                         WHERE PrescriptionID = @PrescriptionID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PrescriptionID", prescriptionId);


                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            total = Convert.ToDecimal(result);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return -1;
            }

            return total;
        }

        public static bool SendToCashier(int prescriptionId, DataTable dtDispensedItems, int? VisitID, int userId)
        {
            using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. إنشاء الجدول المؤقت
                    conn.ExecuteNonQuery("CREATE TABLE #TempDispensed (DetailsID INT, DispensedQty INT, IsDispensed BIT)", transaction);

                    // 2. نقل البيانات عبر SqlBulkCopy
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = "#TempDispensed";
                        bulkCopy.ColumnMappings.Add("PrescriptionDetailsID", "DetailsID");
                        bulkCopy.ColumnMappings.Add("DispensedQuantity", "DispensedQty");
                        bulkCopy.ColumnMappings.Add("IsDispensed", "IsDispensed");
                        bulkCopy.WriteToServer(dtDispensedItems);
                    }

                    // 3. تحديث الجدول الأصلي
                    string updateQuery = @"UPDATE PD 
                                   SET PD.DispensedQuantity = T.DispensedQty, 
                                       PD.IsDispensed = T.IsDispensed
                                   FROM PrescriptionDetails PD
                                   INNER JOIN #TempDispensed T ON PD.PrescriptionDetailsID = T.DetailsID";
                    conn.ExecuteNonQuery(updateQuery, transaction);

                    if (!UpdatePrescriptionStatus(prescriptionId, 2, transaction))
                    {
                        throw new Exception("Failed to update prescription status.");
                    }

                    int BillID = 0;
                    if (VisitID.HasValue && VisitID.Value != -1)
                    {
                        // ابحث هل توجد فاتورة غير ملغاة لهذه الزيارة
                        string selectBill = "SELECT TOP 1 BillID FROM Bills WHERE VisitID = @VisitID AND PaymentStatus != 4";
                        using (MySqlCommand cmdSelect = new MySqlCommand(selectBill, conn, transaction))
                        {
                            cmdSelect.Parameters.AddWithValue("@VisitID", VisitID);
                            object result = cmdSelect.ExecuteScalar();
                            if (result != null)
                            {
                                BillID = Convert.ToInt32(result);
                                clsBillingServiceData.UpdatePaymentStatus(VisitID.Value, 1, transaction);

                            }


                        }
                    }

                    // 3. إذا لم نجد فاتورة (أو كانت حالة صيدلية)، ننشئ واحدة جديدة
                    if (BillID == 0)
                    {
                        string insertBill = @"INSERT INTO Bills (VisitID, PaymentStatus, BillDate, CreatedByUserID) 
                                      VALUES (@VisitID, 0, GETDATE(), @UID); 
                                      SELECT LAST_INSERT_ID();";

                        using (MySqlCommand cmdBill = new MySqlCommand(insertBill, conn, transaction))
                        {
                            cmdBill.Parameters.AddWithValue("@UID", userId);
                            cmdBill.Parameters.AddWithValue("@VisitID", (VisitID == -1 || VisitID == null) ? (object)DBNull.Value : VisitID);
                            BillID = Convert.ToInt32(cmdBill.ExecuteScalar());
                            clsBillingServiceData.UpdateBillNumber(BillID, transaction);
                        }
                    }

                    string linkPresc = "UPDATE Prescriptions SET BillID = @BillID WHERE PrescriptionID = @PID";
                    using (MySqlCommand cmdLink = new MySqlCommand(linkPresc, conn, transaction))
                    {
                        cmdLink.Parameters.AddWithValue("@BillID", BillID);
                        cmdLink.Parameters.AddWithValue("@PID", prescriptionId);
                        cmdLink.ExecuteNonQuery();
                    }

                    string updateStockQuery = @"
                                UPDATE M
                                SET M.CurrentStock = M.CurrentStock - T.DispensedQty
                                FROM Medicines M
                                INNER JOIN PrescriptionDetails PD ON M.MedicineID = PD.MedicineID
                                INNER JOIN #TempDispensed T ON PD.PrescriptionDetailsID = T.DetailsID
                                WHERE T.IsDispensed = 1 AND T.DispensedQty > 0;";

                    using (MySqlCommand cmdUpdateStock = new MySqlCommand(updateStockQuery, conn, transaction))
                    {
                        cmdUpdateStock.ExecuteNonQuery();
                    }

                    // ب. تسجيل حركة الصرف بالسالب في جدول InventoryTransactions للرقابة والتدقيق
                    string insertTransactionQuery = @"
                                    INSERT INTO InventoryTransactions (MedicineID, QuantityChange, ReferenceID, TransactionDate, UserID)
                                    SELECT 
                                        PD.MedicineID,
                                        -T.DispensedQty,  -- الإشارة بالسالب لأنها حركة صرف (نقص في المخزن)
                                        @BillID,          -- المرجع هنا هو رقم الفاتورة التي تم ربطها
                                        GETDATE(),
                                        @UserID
                                    FROM #TempDispensed T
                                    INNER JOIN PrescriptionDetails PD ON T.DetailsID = PD.PrescriptionDetailsID
                                    WHERE T.IsDispensed = 1 AND T.DispensedQty > 0;";

                    using (MySqlCommand cmdInsertTrans = new MySqlCommand(insertTransactionQuery, conn, transaction))
                    {
                        cmdInsertTrans.Parameters.AddWithValue("@BillID", BillID);
                        cmdInsertTrans.Parameters.AddWithValue("@UserID", userId);
                        cmdInsertTrans.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (MySqlException ex)
                {
                    clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                    transaction.Rollback();
                    return false;
                }
                finally
                {
                    try { conn.ExecuteNonQuery("IF OBJECT_ID('tempdb..#TempDispensed') IS NOT NULL DROP TABLE #TempDispensed", transaction); }
                    catch { }
                }
            }
        }

        public static bool IsPrescriptionPending(int prescriptionID)
        {
            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                // تحقق ما إذا كانت الوصفة موجودة وحالتها "Pending" (1)
                string query = @"
    SELECT 1
    FROM Prescriptions
    WHERE PrescriptionID = @PrescriptionID
      AND PrescriptionStatus = '1'
    LIMIT 1;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PrescriptionID", prescriptionID);

                    try
                    {
                        connection.Open();
                        object result = cmd.ExecuteScalar();
                        return (result != null); // ستعيد true إذا كانت الحالة 1
                    }
                    catch (MySqlException ex)
                    {
                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                        return false;
                    }
                }
            }
        }
    }
}
