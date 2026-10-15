using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsBillingServiceData
    {
        public static bool CheckInPatient(int AppointmentID, int DoctorID, int PatientID, decimal Fees, int UserID, string PaymentMethod)
        {
            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. إنشاء سجل الزيارة (Visit)
                        string queryVisit = @"
    INSERT INTO visits
    (
        PatientID,
        AppointmentID,
        DoctorID,
        VisitDate,
        VisitStatus,
        CreatedByUserID
    )
    VALUES
    (
        @PatientID,
        @AppointmentID,
        @DoctorID,
        NOW(),
        1,
        @UserID
    );
    SELECT LAST_INSERT_ID();";

                        MySqlCommand cmdVisit = new MySqlCommand(queryVisit, connection, transaction);
                        cmdVisit.Parameters.AddWithValue("@PatientID", PatientID);
                        cmdVisit.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                        cmdVisit.Parameters.AddWithValue("@UserID", UserID);
                        cmdVisit.Parameters.AddWithValue("@DoctorID", DoctorID);

                        int VisitID = Convert.ToInt32(cmdVisit.ExecuteScalar());

                        string queryBill = @"
    INSERT INTO Bills
    (
        VisitID,
        PaymentStatus,
        CreatedByUserID,
        BillDate
    )
    VALUES
    (
        @VisitID,
        2,
        @UserID,
        NOW()
    );

    SELECT LAST_INSERT_ID();";
                        int BillID = -1;
                        using (MySqlCommand cmdBill = new MySqlCommand(queryBill, connection, transaction))
                        {
                            cmdBill.Parameters.AddWithValue("@VisitID", VisitID);
                            cmdBill.Parameters.AddWithValue("@UserID", UserID);
                            BillID = Convert.ToInt32(cmdBill.ExecuteScalar());

                            UpdateBillNumber(BillID, transaction);
                        }

                        // 4. تسجيل الدفعة (هنا يتم استخدام PaymentMethod)
                        string queryPay = @"
    INSERT INTO Payments
    (
        BillID,
        PaymentAmount,
        PaymentDate,
        PaymentMethod,
        CreatedByUserID
    )
    VALUES
    (
        @BillID,
        @Amount,
        NOW(),
        @Method,
        @UserID
    );";

                        using (MySqlCommand cmdPay = new MySqlCommand(queryPay, connection, transaction))
                        {
                            cmdPay.Parameters.AddWithValue("@BillID", BillID);
                            cmdPay.Parameters.AddWithValue("@Amount", Fees);
                            cmdPay.Parameters.AddWithValue("@Method", PaymentMethod);
                            cmdPay.Parameters.AddWithValue("@UserID", UserID);
                            cmdPay.ExecuteNonQuery();
                        }

                        // 5. تحديث حالة الموعد
                        if (!clsAppointmentData.UpdateAppointmentStatus(
                            AppointmentID, "InQueue", UserID, transaction))
                        {
                            throw new InvalidOperationException(
                                "Unable to update appointment status.");
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

        public static bool UpdateBillNumber(
      int BillID,
      MySqlTransaction transaction)
        {
            string query = @"
        UPDATE Bills
        SET BillNumber = CONCAT(
            'INV-',
            YEAR(NOW()),
            '-',
            LPAD(@BillID, 4, '0')
        )
        WHERE BillID = @BillID;";

            using (MySqlCommand cmd = new MySqlCommand(
                query,
                transaction.Connection,
                transaction))
            {
                cmd.Parameters.AddWithValue("@BillID", BillID);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdatePaymentStatus(int VisitID, byte PaymentStatus, MySqlTransaction transaction)
        {
            string queryUpdateBillNumber = @"UPDATE Bills SET PaymentStatus=@PaymentStatus 
                                                 WHERE VisitID = @VisitID";
            MySqlCommand cmdUpdate = new MySqlCommand(queryUpdateBillNumber, transaction.Connection, transaction);
            cmdUpdate.Parameters.AddWithValue("@VisitID", VisitID);
            cmdUpdate.Parameters.AddWithValue("@PaymentStatus", PaymentStatus);

            return cmdUpdate.ExecuteNonQuery() > 0;
        }

        public static bool UpdatePaymentStatus(int BillID, byte PaymentStatus)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))

                {
                    string queryUpdateBillNumber = @"UPDATE Bills SET PaymentStatus=@PaymentStatus 
                                                 WHERE BillID = @BillID";
                    using (MySqlCommand cmdUpdate = new MySqlCommand(queryUpdateBillNumber, connection ))
                    {
                        cmdUpdate.Parameters.AddWithValue("@BillID", BillID);
                        cmdUpdate.Parameters.AddWithValue("@PaymentStatus", PaymentStatus);
                        connection.Open();
                        return cmdUpdate.ExecuteNonQuery() > 0;
                    }

                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }

        }

        public static DataTable GetAllBills()
        {
            DataTable dt = new DataTable();

            using (MySqlConnection connection =
                new MySqlConnection(
                    clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT
                VB.*,

                DATE_FORMAT(
                    VB.BillDate,
                    '%Y-%m-%d'
                ) AS BillDateString,

                CASE
                    WHEN VB.PaymentStatus IN ('0', 'Pending', 'Unpaid')
                        THEN 'Pending'

                    WHEN VB.PaymentStatus IN ('1', 'Partial')
                        THEN 'Partial'

                    WHEN VB.PaymentStatus IN ('2', 'Paid')
                        THEN 'Paid'

                    WHEN VB.PaymentStatus IN ('3', 'Refunded')
                        THEN 'Refunded'

                    WHEN VB.PaymentStatus IN ('4', 'Cancelled')
                        THEN 'Cancelled'

                    ELSE VB.PaymentStatus
                END AS Status

            FROM View_Bills VB

            ORDER BY VB.BillDate DESC, VB.BillID DESC;";

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
        public static DataRow GetBillByID(int BillID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM View_Bills WHERE BillID = @BillID;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BillID", BillID);

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

            return (dt.Rows.Count > 0) ? dt.Rows[0] : null;
        }

        public static bool CancelBillAndRestoreStock(int billID, int userID)
        {
            bool isCancelled = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // 1. إرجاع المخزون
                        string query1 = @"
                    UPDATE M
                    SET M.CurrentStock = M.CurrentStock + PD.DispensedQuantity
                    FROM Medicines M
                    INNER JOIN PrescriptionDetails PD ON M.MedicineID = PD.MedicineID
                    INNER JOIN Prescriptions P ON P.PrescriptionID = PD.PrescriptionID
                    WHERE P.BillID = @BillID
                    AND PD.DispensedQuantity > 0;
                ";

                        using (MySqlCommand cmd1 = new MySqlCommand(query1, connection, transaction))
                        {
                            cmd1.Parameters.AddWithValue("@BillID", billID);
                            cmd1.ExecuteNonQuery();
                        }

                        // 2. تسجيل حركة الإرجاع في InventoryTransactions
                        string query2 = @"
                    INSERT INTO InventoryTransactions
                    (
                        MedicineID,
                        QuantityChange,
                        ReferenceID,
                        TransactionDate,
                        UserID
                    )
                    SELECT
                        PD.MedicineID,
                        PD.DispensedQuantity,
                        @BillID,
                        GETDATE(),
                        @UserID
                    FROM PrescriptionDetails PD
                    INNER JOIN Prescriptions P ON P.PrescriptionID = PD.PrescriptionID
                    WHERE P.BillID = @BillID
                    AND PD.DispensedQuantity > 0;
                ";

                        using (MySqlCommand cmd2 = new MySqlCommand(query2, connection, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@BillID", billID);
                            cmd2.Parameters.AddWithValue("@UserID", userID);
                            cmd2.ExecuteNonQuery();
                        }

                        // 3. تصفير الصرف
                        string query3 = @"
                    UPDATE PrescriptionDetails
                    SET DispensedQuantity = 0,
                        IsDispensed = 0
                    WHERE PrescriptionID IN
                    (SELECT PrescriptionID FROM Prescriptions WHERE BillID = @BillID);
                ";

                        using (MySqlCommand cmd3 = new MySqlCommand(query3, connection, transaction))
                        {
                            cmd3.Parameters.AddWithValue("@BillID", billID);
                            cmd3.ExecuteNonQuery();
                        }

                        // 4. إلغاء الوصفات
                        string query4 = @"
                    UPDATE Prescriptions
                    SET PrescriptionStatus = 6
                    WHERE BillID = @BillID;
                ";

                        using (MySqlCommand cmd4 = new MySqlCommand(query4, connection, transaction))
                        {
                            cmd4.Parameters.AddWithValue("@BillID", billID);
                            cmd4.ExecuteNonQuery();
                        }

                        // 5. إلغاء الفاتورة (آخر خطوة للتحقق)
                        string query5 = @"
                    UPDATE Bills
                    SET PaymentStatus = 4
                    WHERE BillID = @BillID
                    AND PaymentStatus = 0;
                ";

                        using (MySqlCommand cmd5 = new MySqlCommand(query5, connection, transaction))
                        {
                            cmd5.Parameters.AddWithValue("@BillID", billID);

                            int affected = cmd5.ExecuteNonQuery();

                            if (affected == 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        transaction.Commit();
                        isCancelled = true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, billID);
                isCancelled = false;
            }

            return isCancelled;
        }

        public static bool GetBillSummaryData(
            int BillID,
            ref string billNumber,
            ref string patientName,
            ref decimal appointmentFees,
            ref decimal totalMedicines,
            ref decimal totalServices,
            ref decimal totalDiscount,
            ref decimal totalTax,
            ref decimal paymentAmount,
            ref decimal balanceDue,
            ref decimal totalBeforeDiscountAndTax,
            ref decimal finalTotalIncludingAll,
            DataTable dtBillDetails)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                SELECT
                    BillID,
                    BillNumber,
                    PatientName,
                    TotalAmount,
                    PaidAmount,
                    BalanceDue
                FROM View_Bills
                WHERE BillID = @BillID
                LIMIT 1;";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BillID", BillID);
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return false;

                            billNumber = Convert.ToString(reader["BillNumber"]);
                            patientName = Convert.ToString(reader["PatientName"]);

                            decimal totalAmount = reader["TotalAmount"] == DBNull.Value
                                ? 0m : Convert.ToDecimal(reader["TotalAmount"]);

                            paymentAmount = reader["PaidAmount"] == DBNull.Value
                                ? 0m : Convert.ToDecimal(reader["PaidAmount"]);

                            balanceDue = reader["BalanceDue"] == DBNull.Value
                                ? 0m : Convert.ToDecimal(reader["BalanceDue"]);

                            // View_Bills currently provides aggregate totals only.
                            // Do not invent medicine, service, tax or discount amounts.
                            appointmentFees = 0m;
                            totalMedicines = 0m;
                            totalServices = 0m;
                            totalDiscount = 0m;
                            totalTax = 0m;

                            totalBeforeDiscountAndTax = totalAmount;
                            finalTotalIncludingAll = totalAmount;

                            dtBillDetails.Clear();
                            dtBillDetails.Columns.Clear();

                            dtBillDetails.Columns.Add("BillNumber", typeof(string));
                            dtBillDetails.Columns.Add("PatientName", typeof(string));
                            dtBillDetails.Columns.Add("TotalAmount", typeof(decimal));
                            dtBillDetails.Columns.Add("PaidAmount", typeof(decimal));
                            dtBillDetails.Columns.Add("BalanceDue", typeof(decimal));

                            DataRow row = dtBillDetails.NewRow();

                            row["BillNumber"] = billNumber;
                            row["PatientName"] = patientName;
                            row["TotalAmount"] = totalAmount;
                            row["PaidAmount"] = paymentAmount;
                            row["BalanceDue"] = balanceDue;

                            dtBillDetails.Rows.Add(row);

                            return true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error);

                throw new InvalidOperationException(
                    "Unable to load bill summary for BillID " + BillID,
                    ex);
            }
        }



        #region EDitPending Prescription ITems
        public static DataTable GetPrescriptionsDetails(int BillID, ref int PrescriptionID, ref DateTime PrescriptionDate, ref bool isFound)
        {
            DataTable dtDetails = new DataTable();
            isFound = false;

            string query = @"
        -- الاستعلام الأول: جلب معلومات الفاتورة والمريض العامة
          Select PrescriptionID,Format(PrescriptionDate,'yyyy/MM/dd')As PrescriptionDate
                 From Prescriptions
                Where BillID=@BillID

        -- الاستعلام الثاني: جلب تفاصيل الأدوية والضرائب والكميات المرتجعة سابقاً
         Select PrescriptionDetailsID,SavedMedicineName,SavedMedicinePrice,DispensedQuantity
                  From PrescriptionDetails PD
                  INNER JOIN Prescriptions P ON PD.PrescriptionID=P.PrescriptionID
                  Where P.BillID=@BillID";

            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BillID", BillID);

                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // 1. قراءة النتيجة الأولى (المعلومات العامة للفاتورة)
                            if (reader.Read())
                            {
                                PrescriptionID = (int)reader["PrescriptionID"];
                                PrescriptionDate = Convert.ToDateTime(reader["PrescriptionDate"]); // تم تصحيح الكاستنج هنا
                                isFound = true;
                            }

                            // 2. الانتقال إلى النتيجة الثانية (جدول تفاصيل الأدوية)
                            if (reader.NextResult())
                            {
                                if (reader.HasRows)
                                {
                                    dtDetails.Load(reader);
                                }
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                        isFound = false;
                    }
                }
            }
            return dtDetails;
        }

        public static bool UpdateAfterEditPendingPrescription(DataTable dtDetails,int BillID,int CreatedByUserID)
        {
            using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. إنشاء الجدول المؤقت
                    conn.ExecuteNonQuery("CREATE TABLE #TempDispensed (DetailsID INT, DispensedQty INT)", transaction);

                    // 2. نقل البيانات عبر SqlBulkCopy
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = "#TempDispensed";
                        bulkCopy.ColumnMappings.Add("PrescriptionDetailsID", "DetailsID");
                        bulkCopy.ColumnMappings.Add("QtyReturned", "DispensedQty");
                        bulkCopy.WriteToServer(dtDetails);
                    }

                    // 3. تحديث الجدول الأصلي
                    string updateQuery = @"UPDATE PD 
                                   SET PD.DispensedQuantity =PD.DispensedQuantity - T.DispensedQty, 
                                       PD.IsDispensed = CASE WHEN (PD.DispensedQuantity - T.DispensedQty) > 0 THEN 1 ELSE 0 END
                                   FROM PrescriptionDetails PD
                                   INNER JOIN #TempDispensed T ON PD.PrescriptionDetailsID = T.DetailsID";
                    conn.ExecuteNonQuery(updateQuery, transaction);

                  
                    

                    string updateStockQuery = @"
                                UPDATE M
                                SET M.CurrentStock = M.CurrentStock + T.DispensedQty
                                FROM Medicines M
                                INNER JOIN PrescriptionDetails PD ON M.MedicineID = PD.MedicineID
                                INNER JOIN #TempDispensed T ON PD.PrescriptionDetailsID = T.DetailsID
                                WHERE T.DispensedQty > 0;";

                    using (MySqlCommand cmdUpdateStock = new MySqlCommand(updateStockQuery, conn, transaction))
                    {
                        cmdUpdateStock.ExecuteNonQuery();
                    }

                    // ب. تسجيل حركة الصرف بالسالب في جدول InventoryTransactions للرقابة والتدقيق
                    string insertTransactionQuery = @"
                                    INSERT INTO InventoryTransactions (MedicineID, QuantityChange, ReferenceID, TransactionDate, UserID)
                                    SELECT 
                                        PD.MedicineID,
                                        +T.DispensedQty,  
                                        @BillID,          -- المرجع هنا هو رقم الفاتورة التي تم ربطها
                                        GETDATE(),
                                        @UserID
                                    FROM #TempDispensed T
                                    INNER JOIN PrescriptionDetails PD ON T.DetailsID = PD.PrescriptionDetailsID
                                    WHERE T.DispensedQty > 0;";

                    using (MySqlCommand cmdInsertTrans = new MySqlCommand(insertTransactionQuery, conn, transaction))
                    {
                        cmdInsertTrans.Parameters.AddWithValue("@BillID", BillID);
                        cmdInsertTrans.Parameters.AddWithValue("@UserID", CreatedByUserID);
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

        #endregion


    }
}
