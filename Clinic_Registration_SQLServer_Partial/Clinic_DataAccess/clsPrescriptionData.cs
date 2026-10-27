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

        public static bool UpdatePrescription(
            int PrescriptionID,
            int VisitID,
            string Notes,
            DateTime Date,
            DataTable dtMedicines,
            MySqlTransaction transaction)
        {
            if (transaction == null || transaction.Connection == null)
                throw new ArgumentNullException(nameof(transaction));

            MySqlConnection connection = transaction.Connection;

            string status;

            // Check the current prescription status.
            using (MySqlCommand cmd = new MySqlCommand(@"
        SELECT PrescriptionStatus
        FROM prescriptions
        WHERE PrescriptionID = @PrescriptionID
          AND VisitID = @VisitID
        FOR UPDATE;",
                connection, transaction))
            {
                cmd.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);
                cmd.Parameters.AddWithValue("@VisitID", VisitID);

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException(
                        "Prescription not found for this visit.");

                status = Convert.ToString(result).Trim();
            }

            if (!string.Equals(status, "Pending",
                StringComparison.OrdinalIgnoreCase) && status != "1")
            {
                throw new InvalidOperationException(
                    "Only pending prescriptions can be edited. " +
                    "Current status: " + status);
            }

            // Prevent modification of prescription details
            // referenced by existing pharmacy returns.
            using (MySqlCommand cmd = new MySqlCommand(@"
        SELECT COUNT(*)
        FROM prescriptiondetails pd
        INNER JOIN salesreturndetails srd
            ON srd.PrescriptionDetailsID = pd.PrescriptionDetailID
        WHERE pd.PrescriptionID = @PrescriptionID;",
                connection, transaction))
            {
                cmd.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    throw new InvalidOperationException(
                        "This prescription has pharmacy return transactions. " +
                        "Its medicine details cannot be modified.");
            }

            if (dtMedicines == null || dtMedicines.Rows.Count == 0)
                throw new InvalidOperationException(
                    "A prescription must contain at least one medicine.");

            // Medicine synchronization is intentionally blocked
            // until stable PrescriptionDetailID-based updates
            // and pharmacy reference checks are implemented.
            throw new InvalidOperationException(
                "Medicine editing is not yet enabled. " +
                "The transaction has been rolled back to protect " +
                "existing prescription and pharmacy records.");
        }

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
        public static bool HasPharmacyTransactions(int prescriptionID)
        {
            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();

                string query = @"
            SELECT EXISTS (
                SELECT 1
                FROM prescriptiondetails pd
                INNER JOIN salesreturndetails srd
                    ON srd.PrescriptionDetailsID = pd.PrescriptionDetailID
                WHERE pd.PrescriptionID = @PrescriptionID
            );";

                using (MySqlCommand cmd =
                    new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@PrescriptionID", prescriptionID);

                    return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                }
            }
        }
        public static bool DeletePrescription(int prescriptionID)
        {
            using (MySqlConnection connection = ClinicDatabase.GetConnection())
            {
                connection.Open();

                using (MySqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand(
                            @"DELETE FROM prescriptiondetails
                      WHERE PrescriptionID = @PrescriptionID;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@PrescriptionID", prescriptionID);

                            cmd.ExecuteNonQuery();
                        }

                        int affectedRows;

                        using (MySqlCommand cmd = new MySqlCommand(
                            @"DELETE FROM prescriptions
                      WHERE PrescriptionID = @PrescriptionID;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@PrescriptionID", prescriptionID);

                            affectedRows = cmd.ExecuteNonQuery();
                        }

                        if (affectedRows == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static bool UpdatePrescription(
      int PrescriptionID,
      int? VisitID,
      string Notes,
      DateTime Date,
      byte PrescriptionStatus,
      DataTable dtMedicines)
        {
            if (!VisitID.HasValue || VisitID.Value <= 0)
                return false;

            using (MySqlConnection connection =
                ClinicDatabase.GetConnection())
            {
                connection.Open();

                using (MySqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        bool result = UpdatePrescription(
                            PrescriptionID,
                            VisitID.Value,
                            Notes,
                            Date,
                            dtMedicines,
                            transaction);

                        if (!result)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
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
                    string query = @"
    SELECT
        PPD.PrescriptionDetailID,
        PPD.PrescriptionID,
        PPD.MedicineID,
        PPD.SavedMedicineName,
        PPD.Dosage,
        PPD.Instructions,
        PPD.SavedMedicinePrice,
        PPD.Quantity,
        PPD.Frequency,
        PPD.DiscountAmount,
        M.TaxRate
    FROM prescriptions PP
    INNER JOIN prescriptiondetails PPD
        ON PP.PrescriptionID = PPD.PrescriptionID
    LEFT JOIN medicines M
        ON PPD.MedicineID = M.MedicineID
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
                                        IFNULL(P.VisitID, -1) AS VisitID,
                                        IFNULL(V.AppointmentID, -1) AS AppointmentID, 
                                        IFNULL(PP.FullName, 'Not Found') AS PatientName,
                                        IFNULL(PD.FullName, 'Pharmacy') AS DoctorName,
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
                    string query = @"
                                    SELECT 
                                        P.PrescriptionStatus,
                                        PD.PrescriptionID,
                                        PD.PrescriptionDetailID,
                                        PD.SavedMedicineName AS MedicineName,
                                        PD.Dosage,
                                        M.TaxRate,
                                        PD.SavedMedicinePrice,
                                        PD.Frequency,
                                        PD.Quantity AS RequiredQuantity,
                                        PD.Instructions,
                                        PD.DiscountAmount,

                                        -- Smart dispensed quantity
                                        CASE 
                                            WHEN P.PrescriptionStatus = 1 THEN 
                                                CASE 
                                                    WHEN M.CurrentStock IS NULL OR M.CurrentStock <= 0 THEN 0
                                                    WHEN M.CurrentStock >= PD.Quantity THEN PD.Quantity
                                                    ELSE M.CurrentStock 
                                                END
                                            ELSE PD.DispensedQuantity 
                                        END AS DispensedQuantity,

                                        -- Smart checkbox state
                                        CASE 
                                            WHEN P.PrescriptionStatus = 1 THEN 
                                                CASE 
                                                    WHEN M.CurrentStock IS NULL OR M.CurrentStock <= 0 THEN 0
                                                    ELSE 1 
                                                END
                                            ELSE PD.IsDispensed 
                                        END AS IsDispensed,

                                        -- Availability status
                                        CASE 
                                            WHEN P.PrescriptionStatus = 1 THEN
                                                CASE 
                                                    WHEN M.CurrentStock IS NULL OR M.CurrentStock <= 0
                                                        THEN 'Out of Stock'
                                                    WHEN M.CurrentStock >= PD.Quantity
                                                        THEN 'Fully Available'
                                                    ELSE CONCAT(
                                                        'Partially Available (',
                                                        CAST(M.CurrentStock AS CHAR),
                                                        ')'
                                                    )
                                                END
                                            ELSE 'Finalized'
                                        END AS AvailableStatus

                                    FROM PrescriptionDetails PD
                                    LEFT JOIN Prescriptions P
                                        ON PD.PrescriptionID = P.PrescriptionID
                                    LEFT JOIN
                                    (
                                        SELECT
                                            MedicineName,
                                            MAX(TaxRate) AS TaxRate,
                                            MAX(CurrentStock) AS CurrentStock
                                        FROM Medicines
                                        GROUP BY MedicineName
                                    ) M
                                        ON PD.SavedMedicineName COLLATE utf8mb4_0900_ai_ci
                                         = M.MedicineName COLLATE utf8mb4_0900_ai_ci";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
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

            string query = @"
        SELECT
            P.PrescriptionID,
            P.PrescriptionDate,

            COALESCE(PP.FullName, 'Unknown Patient')
                AS PatientName,

            COALESCE(DP.FullName, 'Unknown Doctor')
                AS DoctorName,

            P.VisitID,

            CASE TRIM(P.PrescriptionStatus)
                WHEN '1' THEN 'Pending'
                WHEN '2' THEN 'Waiting For Payment'
                WHEN '3' THEN 'Ready For Dispensing'
                WHEN '4' THEN 'Dispensed'
                WHEN '5' THEN 'Partially Dispensed'
                WHEN '6' THEN 'Cancelled'
                ELSE COALESCE(P.PrescriptionStatus, 'Unknown')
            END AS Status,

            COALESCE(P.PrescriptionNotes, '') AS Notes

        FROM prescriptions P

        LEFT JOIN visits V
            ON P.VisitID = V.VisitID

        LEFT JOIN patients Pat
            ON V.PatientID = Pat.PatientID

        LEFT JOIN persons PP
            ON Pat.PersonID = PP.PersonID

        LEFT JOIN doctors D
            ON V.DoctorID = D.DoctorID

        LEFT JOIN persons DP
            ON D.PersonID = DP.PersonID

        ORDER BY P.PrescriptionDate DESC,
                 P.PrescriptionID DESC;";

            using (MySqlConnection connection =
                new MySqlConnection(
                    clsDataAccessSettings.ConnectionString))
            {
                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
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
                        // Check whether a non-cancelled bill already exists for this visit.
                        string selectBill = @"
        SELECT BillID
        FROM Bills
        WHERE VisitID = @VisitID
          AND PaymentStatus != 4
        LIMIT 1
        FOR UPDATE;";

                        using (MySqlCommand cmdSelect =
                            new MySqlCommand(selectBill, conn, transaction))
                        {
                            cmdSelect.Parameters.AddWithValue("@VisitID", VisitID.Value);

                            object result = cmdSelect.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                BillID = Convert.ToInt32(result);

                                // Existing bill: update its payment status.
                                clsBillingServiceData.UpdatePaymentStatus(
                                    VisitID.Value, 1, transaction);
                            }
                        }
                    }

                    // If no existing bill is found, create a new bill.
                    if (BillID == 0)
                    {
                        string insertBill = @"
        INSERT INTO Bills
        (
            VisitID,
            PaymentStatus,
            BillDate,
            CreatedByUserID
        )
        VALUES
        (
            @VisitID,
            0,
            NOW(),
            @UID
        );

        SELECT LAST_INSERT_ID();";

                        using (MySqlCommand cmdBill =
                            new MySqlCommand(insertBill, conn, transaction))
                        {
                            cmdBill.Parameters.AddWithValue("@UID", userId);

                            cmdBill.Parameters.AddWithValue(
                                "@VisitID",
                                (VisitID == -1 || VisitID == null)
                                    ? (object)DBNull.Value
                                    : VisitID.Value);

                            BillID = Convert.ToInt32(cmdBill.ExecuteScalar());

                            clsBillingServiceData.UpdateBillNumber(
                                BillID, transaction);
                        }
                    }

                    string linkPresc = "UPDATE Prescriptions SET BillID = @BillID WHERE PrescriptionID = @PID";
                    using (MySqlCommand cmdLink = new MySqlCommand(linkPresc, conn, transaction))
                    {
                        cmdLink.Parameters.AddWithValue("@BillID", BillID);
                        cmdLink.Parameters.AddWithValue("@PID", prescriptionId);
                        cmdLink.ExecuteNonQuery();
                    }

                    //string updateStockQuery = @"
                    //            UPDATE M
                    //            SET M.CurrentStock = M.CurrentStock - T.DispensedQty
                    //            FROM Medicines M
                    //            INNER JOIN PrescriptionDetails PD ON M.MedicineID = PD.MedicineID
                    //            INNER JOIN #TempDispensed T ON PD.PrescriptionDetailsID = T.DetailsID
                    //            WHERE T.IsDispensed = 1 AND T.DispensedQty > 0;";

                    ////using (MySqlCommand cmdUpdateStock = new MySqlCommand(updateStockQuery, conn, transaction))
                    ////{
                    ////    cmdUpdateStock.ExecuteNonQuery();
                    ////}

                    // ب. تسجيل حركة الصرف بالسالب في جدول InventoryTransactions للرقابة والتدقيق
                    string insertTransactionQuery = @"
                                    INSERT INTO InventoryTransactions (MedicineID, QuantityChange, ReferenceID, TransactionDate, UserID)
                                    SELECT 
                                        PD.MedicineID,
                                        -T.DispensedQty,  -- الإشارة بالسالب لأنها حركة صرف (نقص في المخزن)
                                        @BillID,          -- المرجع هنا هو رقم الفاتورة التي تم ربطها
                                        NOW(),
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
                    string transactionId = BillID.ToString();

                    foreach (DataRow row in dtDispensedItems.Rows)
                    {
                        int detailsId = Convert.ToInt32(
                            row["PrescriptionDetailsID"]);

                        decimal dispensedQty = Convert.ToDecimal(
                            row["DispensedQuantity"]);

                        bool isDispensed = Convert.ToBoolean(
                            row["IsDispensed"]);

                        if (!isDispensed || dispensedQty <= 0)
                            continue;

                        string itemQuery = @"
        SELECT
            PD.MedicineID,
            IFNULL(I.cost_price, 0) AS CostPrice,
            IFNULL(I.sales_price, 0) AS SalesPrice
        FROM PrescriptionDetails PD
        INNER JOIN tbl_items I
            ON I.id = PD.MedicineID
        WHERE PD.PrescriptionDetailsID = @DetailsID
        LIMIT 1;";

                        using (MySqlCommand cmd = new MySqlCommand(
                            itemQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@DetailsID", detailsId);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception(
                                        $"Medicine item not found for detail {detailsId}");

                                int itemId = Convert.ToInt32(
                                    reader["MedicineID"]);

                                decimal costPrice = Convert.ToDecimal(
                                    reader["CostPrice"]);

                                decimal salesPrice = Convert.ToDecimal(
                                    reader["SalesPrice"]);

                                // Close the reader before executing another command.
                                reader.Close();

                                CommonInsert.InsertItemTransactionWithTransaction(
                                    conn,
                                    transaction,
                                    DateTime.Now,
                                    "Prescription Dispense",
                                    transactionId,
                                    itemId.ToString(),
                                    costPrice.ToString(
                                        System.Globalization.CultureInfo.InvariantCulture),
                                    "0", // qtyIn
                                    salesPrice.ToString(
                                        System.Globalization.CultureInfo.InvariantCulture),
                                    dispensedQty.ToString(
                                        System.Globalization.CultureInfo.InvariantCulture), // qtyOut
                                    "0", // qtyInc
                                    $"Medicine dispensed against Bill #{BillID}",
                                    null, // warehouseId
                                    null, // batchId
                                    null, // unitId
                                    "1", // factor
                                    dispensedQty.ToString(
                                        System.Globalization.CultureInfo.InvariantCulture) // qtyDisplay
                                );
                            }
                        }
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
            using (MySqlConnection connection =
                ClinicDatabase.GetConnection())
            {
                const string query = @"
            SELECT COUNT(*)
            FROM prescriptions
            WHERE PrescriptionID = @PrescriptionID
              AND PrescriptionStatus IN ('1', 'Pending');";

                using (MySqlCommand cmd =
                    new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue(
                        "@PrescriptionID", prescriptionID);

                    connection.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
