using Clinic_DataAccess;
using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsSalesReturnDataAccess
    {



        public static DataTable GetBillCompleteInfo(
                    int billID,
                    ref string billNumber,
                    ref DateTime billDate,
                    ref string patientName,
                    ref bool isFound)
        {
            DataTable dtDetails = new DataTable();
            isFound = false;

            string query = @"
        -- الاستعلام الأول: جلب معلومات الفاتورة والمريض العامة
        SELECT 
            B.BillNumber,
            B.BillDate,
            ISNULL(P.FullName, 'Pharmacy Sales') AS PatientName
        FROM dbo.Bills B
        LEFT JOIN dbo.Visits V ON B.VisitID = V.VisitID
        LEFT JOIN dbo.Patients Pa ON V.PatientID = Pa.PatientID
        LEFT JOIN dbo.Persons P ON Pa.PersonID = P.PersonID
        WHERE B.BillID = @BillID;

        -- الاستعلام الثاني: جلب تفاصيل الأدوية والضرائب والكميات المرتجعة سابقاً
        SELECT 
            PD.PrescriptionDetailsID,
            PD.MedicineID,
            PD.SavedMedicineName,
            PD.DispensedQuantity AS QtyBought,
            ISNULL((SELECT SUM(SRD.QtyReturned) 
                    FROM SalesReturnDetails SRD
                    INNER JOIN SalesReturns SR ON SRD.ReturnID = SR.ReturnID
                    WHERE SRD.PrescriptionDetailsID = PD.PrescriptionDetailsID AND SR.BillID = @BillID), 0) AS QtyReturnedBefore,
            PD.SavedMedicinePrice AS UnitPrice,
            M.TaxRate
        FROM PrescriptionDetails PD
        INNER JOIN Medicines M ON PD.MedicineID = M.MedicineID
        INNER JOIN Prescriptions P ON PD.PrescriptionID = P.PrescriptionID
        INNER JOIN Bills B ON P.BillID = B.BillID
        WHERE B.BillID = @BillID AND PD.IsDispensed = 1;";

            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // 1. قراءة النتيجة الأولى (المعلومات العامة للفاتورة)
                            if (reader.Read())
                            {
                                billNumber = reader["BillNumber"].ToString();
                                patientName = reader["PatientName"].ToString();
                                billDate = Convert.ToDateTime(reader["BillDate"]); // تم تصحيح الكاستنج هنا
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
        public static int SaveSalesReturn(int billID, decimal totalRefund, int cashierID, DataTable dtReturnItems)
        {
            int newReturnID = -1;

            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                // بدء المعاملة المالية لضمان تكامل البيانات بالكامل
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // --- الخطوة 1: إدخال السجل الرئيسي واشتقاق الـ ID الجديد ---
                    string insertMasterQuery = @"
                        INSERT INTO SalesReturns (BillID, ReturnDate, TotalRefund, CashierID)
                        VALUES (@BillID, GETDATE(), @TotalRefund, @CashierID);
                        SELECT LAST_INSERT_ID();";

                    MySqlCommand masterCommand = new MySqlCommand(insertMasterQuery, connection, transaction);
                    masterCommand.Parameters.AddWithValue("@BillID", billID);
                    masterCommand.Parameters.AddWithValue("@TotalRefund", totalRefund);
                    masterCommand.Parameters.AddWithValue("@CashierID", cashierID);

                    newReturnID = Convert.ToInt32(masterCommand.ExecuteScalar());

                    // --- الخطوة 2: فلترة وتجهيز الـ DataTable للـ Bulk Copy بدون For Loops ---
                    DataRow[] returnedRows = dtReturnItems.Select("QtyReturned > 0");

                    if (returnedRows.Length > 0)
                    {
                        // تحويل الأسطر المفلترة فقط إلى جدول مستقل بالذاكرة
                        DataTable dtBulkDetails = returnedRows.CopyToDataTable();

                        // فكرتك الذكية: تعبئة العمود الجديد بالـ ReturnID لكل الأسطر دفعة واحدة
                        dtBulkDetails.Columns.Add("ReturnID", typeof(int), newReturnID.ToString());

                        // --- الخطوة 3: تنفيذ الـ SqlBulkCopy لإدخال تفاصيل المرتجع دفعة واحدة ---
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "dbo.SalesReturnDetails";

                            // تعيين خريطة الأعمدة الأربعة المستهدفة فقط (سيتجاهل بقية الأعمدة الزائدة)
                            bulkCopy.ColumnMappings.Add("ReturnID", "ReturnID");
                            bulkCopy.ColumnMappings.Add("PrescriptionDetailsID", "PrescriptionDetailsID");
                            bulkCopy.ColumnMappings.Add("QtyReturned", "QtyReturned");
                            bulkCopy.ColumnMappings.Add("UnitPrice", "UnitPrice");

                            bulkCopy.WriteToServer(dtBulkDetails);
                        }

                        // --- الخطوة 4: تحديث المخزون (CurrentStock) بضربة واحدة عبر الـ Set-Based JOIN ---
                        string updateStockQuery = @"
                            UPDATE M
                            SET M.CurrentStock = M.CurrentStock + SRD.QtyReturned
                            FROM Medicines M
                            INNER JOIN PrescriptionDetails PD ON M.MedicineID = PD.MedicineID
                            INNER JOIN SalesReturnDetails SRD ON PD.PrescriptionDetailsID = SRD.PrescriptionDetailsID
                            WHERE SRD.ReturnID = @ReturnID;";

                        MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, connection, transaction);
                        updateStockCmd.Parameters.AddWithValue("@ReturnID", newReturnID);
                        updateStockCmd.ExecuteNonQuery();

                        // --- الخطوة 5: تسجيل حركات المخزن (InventoryTransactions) مجمعة بضربة واحدة ---
                        string insertInventoryTransactionsQuery = @"
                            INSERT INTO InventoryTransactions (MedicineID, QuantityChange, ReferenceID, TransactionDate, UserID)
                            SELECT 
                                PD.MedicineID,
                                SRD.QtyReturned,
                                SRD.ReturnID,
                                GETDATE(),
                                @UserID
                            FROM SalesReturnDetails SRD
                            INNER JOIN PrescriptionDetails PD ON SRD.PrescriptionDetailsID = PD.PrescriptionDetailsID
                            WHERE SRD.ReturnID = @ReturnID;";

                        MySqlCommand insertInvCmd = new MySqlCommand(insertInventoryTransactionsQuery, connection, transaction);
                        insertInvCmd.Parameters.AddWithValue("@ReturnID", newReturnID);
                        insertInvCmd.Parameters.AddWithValue("@UserID", cashierID);
                        insertInvCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (MySqlException ex)
                {
                    clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                    transaction.Rollback();
                    newReturnID = -1;
                }

            }

            return newReturnID;
        }

        public static bool IsBillPaidOrPartiallyPaid(int billID)
        {
            bool isEligible = false;

            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 1 1 FROM Bills 
                                 WHERE BillID = @BillID 
                                 AND PaymentStatus IN (1, 2);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            isEligible = true;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                        isEligible = false;
                    }
                }
            }

            return isEligible;
        }
    }
}