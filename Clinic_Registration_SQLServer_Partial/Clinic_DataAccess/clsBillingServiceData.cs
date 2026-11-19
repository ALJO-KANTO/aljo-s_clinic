using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using Clinic_Common;

namespace Clinic_DataAccess
{
    public class clsBillingServiceData
    {
        public static bool CheckInPatient(
            int AppointmentID,
            int DoctorID,
            int PatientID,
            decimal Fees,
            int UserID,
            string PaymentMethod,
            string status,
            int debitAccountId,
            int creditAccountId)
        {
            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 0. Verify appointment is still Scheduled
                        string queryCheckAppointment = @"
                                SELECT AppointmentStatus
                                FROM appointments
                                WHERE AppointmentID = @AppointmentID
                                FOR UPDATE;";

                        using (MySqlCommand cmdCheck = new MySqlCommand(
                            queryCheckAppointment, connection, transaction))
                        {
                            cmdCheck.Parameters.AddWithValue(
                                "@AppointmentID", AppointmentID);

                            object result = cmdCheck.ExecuteScalar();

                            if (result == null || result == DBNull.Value)
                            {
                                throw new InvalidOperationException(
                                    "Appointment not found.");
                            }

                            string currentStatus = result.ToString();

                            if (currentStatus != "Scheduled")
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        // 1. Create Visit
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

                        int VisitID;

                        using (MySqlCommand cmdVisit =
                            new MySqlCommand(queryVisit, connection, transaction))
                        {
                            cmdVisit.Parameters.AddWithValue("@PatientID", PatientID);
                            cmdVisit.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                            cmdVisit.Parameters.AddWithValue("@DoctorID", DoctorID);
                            cmdVisit.Parameters.AddWithValue("@UserID", UserID);

                            VisitID = Convert.ToInt32(cmdVisit.ExecuteScalar());
                        }

                        // 2. Create Bill
                        string queryBill = @"
                                INSERT INTO Bills
                                (
                                    VisitID,
                                    PaymentStatus,
                                    CreatedByUserID,
                                    BillDate,
                                    TotalAmount
                                )
                                VALUES
                                (
                                    @VisitID,
                                    2,
                                    @UserID,
                                    NOW(),
                                    @Amount
                                );

                                SELECT LAST_INSERT_ID();";

                        int BillID;

                        using (MySqlCommand cmdBill =
                            new MySqlCommand(queryBill, connection, transaction))
                        {
                            cmdBill.Parameters.AddWithValue("@VisitID", VisitID);
                            cmdBill.Parameters.AddWithValue("@UserID", UserID);
                            cmdBill.Parameters.AddWithValue("@Amount", Fees);

                            BillID = Convert.ToInt32(cmdBill.ExecuteScalar());
                        }

                        // 3. Generate Bill Number
                        if (!UpdateBillNumber(BillID, transaction))
                        {
                            throw new InvalidOperationException(
                                "Unable to generate the bill number.");
                        }

                        // 4. Record Payment
                        string queryPayment = @"
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

                        using (MySqlCommand cmdPayment =
                            new MySqlCommand(queryPayment, connection, transaction))
                        {
                            cmdPayment.Parameters.AddWithValue("@BillID", BillID);
                            cmdPayment.Parameters.AddWithValue("@Amount", Fees);
                            cmdPayment.Parameters.AddWithValue("@Method", PaymentMethod);
                            cmdPayment.Parameters.AddWithValue("@UserID", UserID);

                            cmdPayment.ExecuteNonQuery();
                        }
                        // 5. Record accounting journal entries
                        // Shared references for both sides of the journal
                        string transactionId = BillID.ToString();
                        string humId = VisitID.ToString();

                        string description = $"Consultation fee for Bill #{BillID}";

                        // Debit: Cash / Bank / Card Clearing
                        CommonInsert.InsertTransactionEntryWithTransaction(
                            connection,
                            transaction,
                            DateTime.Now,
                            debitAccountId.ToString(),
                            Fees.ToString(),
                            "0",
                            transactionId,
                            humId,
                            "Consultation Fee",
                            description,
                            UserID,
                            DateTime.Now);

                        // Credit: Consultation Fee Revenue
                        CommonInsert.InsertTransactionEntryWithTransaction(
                            connection,
                            transaction,
                            DateTime.Now,
                            creditAccountId.ToString(),
                            "0",
                            Fees.ToString(),
                            transactionId,
                           "0",
                            "Consultation Fee",
                            description,
                            UserID,
                            DateTime.Now);

                        // 5. Update Appointment Status
                        if (!clsAppointmentData.UpdateAppointmentStatus(
                            AppointmentID,
                            status,
                            UserID,
                            transaction))
                        {
                            throw new InvalidOperationException(
                                "Unable to update appointment status.");
                        }

                        // 6. Commit all operations together
                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Check-in rollback failed: " + rollbackEx);
                        }

                        if (ex is MySqlException)
                        {
                            clsGlobalLogger.LogMySqlException(
                                (MySqlException)ex,
                                clsGlobalLogger.LogLevel.Error);
                        }

                        System.Diagnostics.Debug.WriteLine(
                            "Check-in failed for Appointment ID " +
                            AppointmentID + ": " + ex);

                        throw new InvalidOperationException(
                            "Patient check-in failed for Appointment ID " +
                            AppointmentID + ". " + ex.Message,
                            ex);
                    }
                }
            }
        }

        public static bool UpdateBillNumber(int BillID, MySqlTransaction transaction)
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

                bool updated = cmd.ExecuteNonQuery() > 0;

                if (!updated)
                {
                    throw new InvalidOperationException(
                        "Unable to generate the bill number for Bill ID: " + BillID);
                }

                return true;
            }
        }
        //public static bool CheckInPatientOld(
        //    int AppointmentID,
        //    int DoctorID,
        //    int PatientID,
        //    decimal Fees,
        //    int UserID,
        //    string PaymentMethod,
        //    string status)
        //{
        //    using (MySqlConnection connection =
        //        new MySqlConnection(clsDataAccessSettings.ConnectionString))
        //    {
        //        connection.Open();

        //        using (MySqlTransaction transaction =
        //            connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                // 1. Create visit
        //                string queryVisit = @"
        //            INSERT INTO visits
        //            (
        //                PatientID,
        //                AppointmentID,
        //                DoctorID,
        //                VisitDate,
        //                VisitStatus,
        //                CreatedByUserID
        //            )
        //            VALUES
        //            (
        //                @PatientID,
        //                @AppointmentID,
        //                @DoctorID,
        //                NOW(),
        //                1,
        //                @UserID
        //            );

        //            SELECT LAST_INSERT_ID();";

        //                int VisitID;

        //                using (MySqlCommand cmdVisit =
        //                    new MySqlCommand(queryVisit, connection, transaction))
        //                {
        //                    cmdVisit.Parameters.AddWithValue("@PatientID", PatientID);
        //                    cmdVisit.Parameters.AddWithValue("@AppointmentID", AppointmentID);
        //                    cmdVisit.Parameters.AddWithValue("@DoctorID", DoctorID);
        //                    cmdVisit.Parameters.AddWithValue("@UserID", UserID);

        //                    VisitID = Convert.ToInt32(cmdVisit.ExecuteScalar());
        //                }


        //                // 4. Update appointment status
        //                if (!clsAppointmentData.UpdateAppointmentStatus(
        //                    AppointmentID,
        //                    status,
        //                    UserID,
        //                    transaction))
        //                {
        //                    throw new InvalidOperationException(
        //                        "Unable to update appointment status.");
        //                }

        //                transaction.Commit();

        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                try
        //                {
        //                    transaction.Rollback();
        //                }
        //                catch (Exception rollbackEx)
        //                {
        //                    System.Diagnostics.Debug.WriteLine(
        //                        "Check-in rollback failed: " +
        //                        rollbackEx.ToString());
        //                }

        //                if (ex is MySqlException)
        //                {
        //                    clsGlobalLogger.LogMySqlException(
        //                        (MySqlException)ex,
        //                        clsGlobalLogger.LogLevel.Error);
        //                }

        //                System.Diagnostics.Debug.WriteLine(
        //                    "Check-in failed for Appointment ID " +
        //                    AppointmentID + ": " + ex.ToString());

        //                throw new InvalidOperationException(
        //                    "Patient check-in failed for Appointment ID " +
        //                    AppointmentID + ". " + ex.Message,
        //                    ex);
        //            }
        //        }
        //    }
        //}

        //public static bool UpdateBillNumber(
        //  int BillID,
        //  MySqlTransaction transaction)
        //{
        //    string query = @"
        //UPDATE Bills
        //SET BillNumber = CONCAT(
        //    'INV-',
        //    YEAR(NOW()),
        //    '-',
        //    LPAD(@BillID, 4, '0')
        //)
        //WHERE BillID = @BillID;";

        //    using (MySqlCommand cmd = new MySqlCommand(
        //        query,
        //        transaction.Connection,
        //        transaction))
        //    {
        //        cmd.Parameters.AddWithValue("@BillID", BillID);

        //        return cmd.ExecuteNonQuery() > 0;
        //    }
        //}

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
                    using (MySqlCommand cmdUpdate = new MySqlCommand(queryUpdateBillNumber, connection))
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
        //public static bool InsertBillOld(int VisitID,int UserID, decimal Fees,string Type,string PaymentMethod,
        //    int AppointmentID
        //    )
        //{
        //    using (MySqlConnection connection =
        //        new MySqlConnection(clsDataAccessSettings.ConnectionString))
        //    {
        //        connection.Open();

        //        using (MySqlTransaction transaction =
        //            connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                // 2. Create bill
        //                string queryBill = @"
        //                    INSERT INTO Bills
        //                       (
        //                           VisitID,
        //                           PaymentStatus,
        //                           CreatedByUserID,
        //                           BillDate,
        //                           TotalAmount,
        //                           BillNumber
        //                       )
        //                       SELECT 
        //                           @VisitID,
        //                           2,                                      -- PaymentStatus
        //                           @UserID,
        //                           NOW(),
        //                           @Amount,
        //                           CONCAT('INV-', YEAR(NOW()), '-', LPAD(
        //                               (SELECT IFNULL(MAX(CAST(SUBSTRING(BillNumber, 10) AS UNSIGNED)), 0) + 1 
        //                                FROM Bills 
        //                                WHERE BillNumber LIKE CONCAT('INV-', YEAR(NOW()), '-%')), 
        //                               4, '0')
        //                           ) AS BillNumber
        //                       FROM visitservices vs
        //                       WHERE vs.VisitID = @VisitID;

        //                       SELECT LAST_INSERT_ID();";

        //                int BillID;

        //                using (MySqlCommand cmdBill =
        //                    new MySqlCommand(queryBill, connection, transaction))
        //                {
        //                    cmdBill.Parameters.AddWithValue("@VisitID", VisitID);
        //                    cmdBill.Parameters.AddWithValue("@UserID", UserID);
        //                    cmdBill.Parameters.AddWithValue("@Amount", Fees);

        //                    BillID = Convert.ToInt32(cmdBill.ExecuteScalar());
        //                }

        //                UpdateBillNumber(BillID, transaction);

        //                transaction.Commit();

        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                try
        //                {
        //                    transaction.Rollback();
        //                }
        //                catch (Exception rollbackEx)
        //                {
        //                    System.Diagnostics.Debug.WriteLine(
        //                        "Bill rollback failed: " +
        //                        rollbackEx.ToString());
        //                }

        //                if (ex is MySqlException)
        //                {
        //                    clsGlobalLogger.LogMySqlException(
        //                        (MySqlException)ex,
        //                        clsGlobalLogger.LogLevel.Error);
        //                }

        //                System.Diagnostics.Debug.WriteLine(
        //                    "Bill failed for Appointment ID " +
        //                    AppointmentID + ": " + ex.ToString());

        //                throw new InvalidOperationException(
        //                    "Patient Bill failed for Appointment ID " +
        //                    AppointmentID + ". " + ex.Message,
        //                    ex);
        //            }
        //        }
        //    }
        //}
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
                        NOW(),
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
            if (dtBillDetails == null)
                throw new ArgumentNullException(nameof(dtBillDetails));

            const string query = @"
        SELECT
            BillNumber,
            PatientName,
            AppointmentFees,
            MedicinesTotal,
            ServicesTotal,
            TotalDiscount,
            TotalTax,
            Subtotal,
            PaymentAmount,
            BalanceDue
        FROM view_billsummaries
        WHERE BillID = @BillID;

        SELECT
            ItemType,
            ItemName,
            Quantity,
            Price,
            Total
        FROM view_billitems
        WHERE BillID = @BillID
        ORDER BY
            CASE ItemType
                WHEN 'Appointment' THEN 1
                WHEN 'Service' THEN 2
                WHEN 'Medicine' THEN 3
                ELSE 4
            END,
            ItemName;";

            using (MySqlConnection connection =
                new MySqlConnection(
                    clsDataAccessSettings.ConnectionString))
            using (MySqlCommand command =
                new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillID", BillID);

                connection.Open();

                using (MySqlDataReader reader =
                    command.ExecuteReader())
                {
                    if (!reader.Read())
                        return false;

                    decimal ReadAmount(string column)
                    {
                        object value = reader[column];

                        return value == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(value);
                    }

                    billNumber =
                        Convert.ToString(reader["BillNumber"]) ?? "";

                    patientName =
                        Convert.ToString(reader["PatientName"]) ?? "";

                    appointmentFees = ReadAmount("AppointmentFees");
                    totalMedicines = ReadAmount("MedicinesTotal");
                    totalServices = ReadAmount("ServicesTotal");
                    totalDiscount = ReadAmount("TotalDiscount");
                    totalTax = ReadAmount("TotalTax");
                    paymentAmount = ReadAmount("PaymentAmount");

                    finalTotalIncludingAll = ReadAmount("Subtotal");

                    // Gross total before discount and tax.
                    // Use the invoice subtotal, which includes laboratory tests.
                    totalBeforeDiscountAndTax =
                        finalTotalIncludingAll +
                        totalDiscount -
                        totalTax;

                    balanceDue = Math.Max(
                        finalTotalIncludingAll - paymentAmount,
                        0m);

                    dtBillDetails.Clear();
                    dtBillDetails.Columns.Clear();

                    if (reader.NextResult())
                        dtBillDetails.Load(reader);

                    return true;
                }
            }
        }

        public static DataTable GetLabOrderForEditing(int billID)
        {
            const string query = @"
        SELECT
            B.BillID,
            B.BillNumber,
            B.LabOrderID,
            O.PatientID,
            O.VisitID,
            O.DoctorID,
            O.RequestSource,
            R.TestID,
            COALESCE(PAY.PaidAmount, 0) AS PaidAmount
        FROM bills B
        INNER JOIN laborders O
            ON O.LabOrderID = B.LabOrderID
        LEFT JOIN labrequests R
            ON R.LabOrderID = O.LabOrderID
           AND R.BillID = B.BillID
           AND R.Status <> 'Cancelled'
        LEFT JOIN
        (
            SELECT BillID, SUM(PaymentAmount) AS PaidAmount
            FROM payments
            GROUP BY BillID
        ) PAY ON PAY.BillID = B.BillID
        WHERE B.BillID = @BillID
        ORDER BY R.TestID;";

            using (var connection = new MySqlConnection(
                clsDataAccessSettings.ConnectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillID", billID);

                var dt = new DataTable();
                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }

                return dt;
            }
        }

        public static bool UpdateUnpaidLabOrder(
            int billID,
            IEnumerable<int> selectedTestIDs)
        {
            if (billID <= 0)
                throw new ArgumentException("Invalid Bill ID.");

            if (selectedTestIDs == null)
                throw new ArgumentNullException(nameof(selectedTestIDs));

            int[] tests = selectedTestIDs.Distinct().ToArray();

            if (tests.Length == 0 || tests.Any(x => x <= 0))
                throw new ArgumentException("Select at least one valid lab test.");

            using (var connection = new MySqlConnection(
                clsDataAccessSettings.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int labOrderID;
                        int patientID;
                        int? visitID;

                        // Lock the existing unpaid invoice and get its linked lab order.
                        using (var cmd = new MySqlCommand(@"
                            SELECT LabOrderID, PatientID, VisitID
                            FROM bills
                            WHERE BillID = @BillID
                              AND PaymentStatus IN ('0', 'Pending', 'Unpaid')
                            FOR UPDATE;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new InvalidOperationException(
                                        "Invoice not found or is not pending.");

                                if (reader["LabOrderID"] == DBNull.Value)
                                    throw new InvalidOperationException(
                                        "Invoice is not linked to a lab order.");

                                labOrderID = Convert.ToInt32(reader["LabOrderID"]);
                                patientID = Convert.ToInt32(reader["PatientID"]);

                                visitID = reader["VisitID"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["VisitID"]);
                            }
                        }

                        // Editing is allowed only if no payment has been received.
                        using (var cmd = new MySqlCommand(@"
                            SELECT COALESCE(SUM(PaymentAmount), 0)
                            FROM payments
                            WHERE BillID = @BillID;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);

                            decimal paidAmount = Convert.ToDecimal(cmd.ExecuteScalar());

                            if (paidAmount > 0)
                                throw new InvalidOperationException(
                                    "Editing is not allowed because payment has been received.");
                        }

                        string testParameters = string.Join(",",
                            tests.Select((x, i) => "@T" + i));

                        // Confirm that all selected tests are active.
                        using (var cmd = new MySqlCommand(@"
                            SELECT COUNT(*)
                            FROM laboratorytests
                            WHERE IsActive = 1
                              AND TestID IN (" + testParameters + ");",
                            connection, transaction))
                        {
                            for (int i = 0; i < tests.Length; i++)
                                cmd.Parameters.AddWithValue("@T" + i, tests[i]);

                            if (Convert.ToInt32(cmd.ExecuteScalar()) != tests.Length)
                                throw new InvalidOperationException(
                                    "One or more selected tests are inactive or unavailable.");
                        }

                        // Detach and cancel tests that the user unchecked.
                        using (var cmd = new MySqlCommand(@"
                            UPDATE labrequests
                            SET Status = 'Cancelled',
                                BillID = NULL
                            WHERE LabOrderID = @OrderID
                              AND BillID = @BillID
                              AND Status <> 'Cancelled'
                              AND TestID NOT IN (" + testParameters + ");",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", labOrderID);
                            cmd.Parameters.AddWithValue("@BillID", billID);

                            for (int i = 0; i < tests.Length; i++)
                                cmd.Parameters.AddWithValue("@T" + i, tests[i]);

                            cmd.ExecuteNonQuery();
                        }

                        // Add newly selected tests to the same lab order and invoice.
                        foreach (int testID in tests)
                        {
                            using (var cmd = new MySqlCommand(@"
                                INSERT INTO labrequests
                                (
                                    LabOrderID,
                                    PatientID,
                                    VisitID,
                                    TestID,
                                    BillID,
                                    Status,
                                    SavedTestPrice
                                )
                                SELECT
                                    @OrderID,
                                    @PatientID,
                                    @VisitID,
                                    T.TestID,
                                    @BillID,
                                    'Ordered',
                                    T.TestPrice
                                FROM laboratorytests T
                                WHERE T.TestID = @TestID
                                  AND T.IsActive = 1
                                  AND NOT EXISTS
                                  (
                                      SELECT 1
                                      FROM labrequests R
                                      WHERE R.LabOrderID = @OrderID
                                        AND R.TestID = @TestID
                                        AND R.BillID = @BillID
                                        AND R.Status <> 'Cancelled'
                                  );",
                                connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@OrderID", labOrderID);
                                cmd.Parameters.AddWithValue("@PatientID", patientID);
                                cmd.Parameters.AddWithValue(
                                    "@VisitID", (object)visitID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@BillID", billID);
                                cmd.Parameters.AddWithValue("@TestID", testID);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Recalculate the invoice amount from its current bill items.
                        decimal newTotal;

                        using (var cmd = new MySqlCommand(@"
                            SELECT COALESCE(SUM(Total), 0)
                            FROM view_billitems
                            WHERE BillID = @BillID;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);
                            newTotal = Convert.ToDecimal(cmd.ExecuteScalar());
                        }

                        // Update only the existing bill; BillID and BillNumber remain unchanged.
                        using (var cmd = new MySqlCommand(@"
                            UPDATE bills
                            SET TotalAmount = @Total
                            WHERE BillID = @BillID
                              AND LabOrderID = @OrderID;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Total", newTotal);
                            cmd.Parameters.AddWithValue("@BillID", billID);
                            cmd.Parameters.AddWithValue("@OrderID", labOrderID);

                            if (cmd.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException(
                                    "Unable to update the existing invoice.");
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

        #region EDitPending Prescription ITems

        public static DataTable GetPrescriptionsDetails(
            int BillID,
            ref int PrescriptionID,
            ref DateTime PrescriptionDate,
            ref bool isFound)
        {
            DataTable dtDetails = new DataTable();
            isFound = false;

            string query = @"
        SELECT
            PrescriptionID,
            PrescriptionDate
        FROM Prescriptions
        WHERE BillID = @BillID;

        SELECT
          PD.PrescriptionDetailID AS PrescriptionDetailsID,
            PD.SavedMedicineName,
            PD.SavedMedicinePrice,
            PD.DispensedQuantity
        FROM PrescriptionDetails PD
        INNER JOIN Prescriptions P
            ON PD.PrescriptionID = P.PrescriptionID
        WHERE P.BillID = @BillID;
    ";

            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BillID", BillID);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // First result: Prescription information
                            if (reader.Read())
                            {
                                PrescriptionID =
                                    Convert.ToInt32(reader["PrescriptionID"]);

                                if (reader["PrescriptionDate"] != DBNull.Value)
                                {
                                    PrescriptionDate =
                                        Convert.ToDateTime(reader["PrescriptionDate"]);
                                }

                                isFound = true;
                            }

                            // Second result: Prescription medicine details
                            if (reader.NextResult())
                            {
                                dtDetails.Load(reader);
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        clsGlobalLogger.LogMySqlException(
                            ex,
                            clsGlobalLogger.LogLevel.Error);

                        isFound = false;
                    }
                }
            }

            return dtDetails;
        }

        public static bool UpdateAfterEditPendingPrescription(DataTable dtDetails, int BillID, int CreatedByUserID)
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
                                   INNER JOIN #TempDispensed T ON PD.PrescriptionDetailID = T.DetailsID";
                    conn.ExecuteNonQuery(updateQuery, transaction);




                    string updateStockQuery = @"
                                UPDATE M
                                SET M.CurrentStock = M.CurrentStock + T.DispensedQty
                                FROM Medicines M
                                INNER JOIN PrescriptionDetails PD ON M.MedicineID = PD.MedicineID
                                INNER JOIN #TempDispensed T ON PD.PrescriptionDetailID = T.DetailsID
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
                                        NOW(),
                                        @UserID
                                    FROM #TempDispensed T
                                    INNER JOIN PrescriptionDetails PD ON T.DetailsID = PD.PrescriptionDetailID
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
