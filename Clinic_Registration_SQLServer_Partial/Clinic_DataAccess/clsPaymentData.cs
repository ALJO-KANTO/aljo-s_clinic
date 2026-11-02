
using Clinic_DataAccess.SaveMySqlException;
using System;
using MySql.Data.MySqlClient;

namespace Clinic_DataAccess
{
    public class clsPaymentData
    {

        public static bool AddPaymentAndSetStatus(
            int billID,
            decimal paymentAmount,
            string paymentMethod,
            int userID,
            byte newStatus,
            int debitAccountId,
            int creditAccountId)
        {
            if (billID <= 0)
                throw new ArgumentException("Invalid Bill ID.");

            if (paymentAmount <= 0)
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");

            if (debitAccountId <= 0 || creditAccountId <= 0)
                throw new ArgumentException(
                    "Invalid accounting account IDs.");

            if (debitAccountId == creditAccountId)
                throw new ArgumentException(
                    "Debit and credit accounts cannot be the same.");

            // STEP 1: Determine payment method and journal type.
            string normalizedMethod =
                (paymentMethod ?? "").Trim().ToLowerInvariant();

            string journalType;
            string methodName;

            switch (normalizedMethod)
            {
                case "cash":
                    journalType = "By Cash";
                    methodName = "Cash";
                    break;

                case "credit card":
                    journalType = "By Credit Card";
                    methodName = "Credit Card";
                    break;

                case "insurance":
                    journalType = "By Insurance";
                    methodName = "Insurance";
                    break;

                default:
                    throw new InvalidOperationException(
                        "Unsupported payment method: " + paymentMethod);
            }

            using (MySqlConnection connection =
                new MySqlConnection(
                    clsDataAccessSettings.ConnectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        decimal totalAmount = 0m;
                        decimal paidBefore = 0m;
                        int visitID = 0;

                        // STEP 2: Lock the bill.
                        string lockBillQuery = @"
                    SELECT VisitID
                    FROM bills
                    WHERE BillID = @BillID
                    FOR UPDATE;";

                        using (MySqlCommand cmd = new MySqlCommand(
                            lockBillQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            object result = cmd.ExecuteScalar();

                            if (result == null ||
                                result == DBNull.Value)
                            {
                                throw new InvalidOperationException(
                                    "Bill not found: " + billID);
                            }

                            visitID = Convert.ToInt32(result);
                        }

                        // STEP 3: Read bill total and existing payments.
                        string billQuery = @"
                    SELECT
                        COALESCE(Subtotal, 0) AS TotalAmount,
                        COALESCE(PaymentAmount, 0) AS PaidAmount
                    FROM view_billsummaries
                    WHERE BillID = @BillID;";

                        using (MySqlCommand cmd = new MySqlCommand(
                            billQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            using (MySqlDataReader reader =
                                cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    throw new InvalidOperationException(
                                        "Bill summary not found: " + billID);
                                }

                                totalAmount = Convert.ToDecimal(
                                    reader["TotalAmount"]);

                                paidBefore = Convert.ToDecimal(
                                    reader["PaidAmount"]);
                            }
                        }

                        // STEP 4: Validate balance due.
                        decimal balanceDue = Math.Max(
                            totalAmount - paidBefore, 0m);
                        System.Diagnostics.Debug.WriteLine(
    $"BillID: {billID}, " +
    $"TotalAmount: {totalAmount}, " +
    $"PaidBefore: {paidBefore}, " +
    $"BalanceDue: {balanceDue}, " +
    $"PaymentAmount: {paymentAmount}");
                        if (paymentAmount > balanceDue)
                        {
                            throw new InvalidOperationException(
                                "Payment exceeds outstanding balance. " +
                                "Remaining balance: " +
                                balanceDue.ToString("N2"));
                        }

                        // STEP 5: Insert payment.
                        string insertPayment = @"
                    INSERT INTO payments
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
                        @PaymentAmount,
                        NOW(),
                        @PaymentMethod,
                        @UserID
                    );";

                        using (MySqlCommand cmd = new MySqlCommand(
                            insertPayment, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            cmd.Parameters.AddWithValue(
                                "@PaymentAmount", paymentAmount);

                            cmd.Parameters.AddWithValue(
                                "@PaymentMethod", methodName);

                            cmd.Parameters.AddWithValue(
                                "@UserID", userID);

                            cmd.ExecuteNonQuery();
                        }

                        // STEP 6: Calculate new payment status.
                        decimal totalPaid = paidBefore + paymentAmount;

                        string statusText;

                        if (totalPaid >= totalAmount)
                            statusText = "Paid";
                        else if (totalPaid > 0)
                            statusText = "Partial";
                        else
                            statusText = "Pending";

                        // STEP 7: Update bill.
                        string updateBill = @"
                    UPDATE bills
                    SET
                        TotalAmount = @TotalAmount,
                        PaymentStatus = @Status
                    WHERE BillID = @BillID;";

                        using (MySqlCommand cmd = new MySqlCommand(
                            updateBill, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@TotalAmount", totalAmount);

                            cmd.Parameters.AddWithValue(
                                "@Status", statusText);

                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            cmd.ExecuteNonQuery();
                        }

                        // STEP 8: Complete visit and update prescription
                        // only when bill is fully paid.
                        if (statusText == "Paid")
                        {
                            string updateVisit = @"
                        UPDATE visits
                        SET VisitStatus = 'Completed'
                        WHERE VisitID = @VisitID
                          AND VisitStatus = 'Pending_Billing';";

                            using (MySqlCommand cmd = new MySqlCommand(
                                updateVisit, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@VisitID", visitID);

                                cmd.ExecuteNonQuery();
                            }

                            if (!clsPrescriptionData
                                .UpdatePrescriptionStatusByBill(
                                    billID, 3, transaction))
                            {
                                throw new InvalidOperationException(
                                    "Failed to update prescription status.");
                            }
                        }

                        // STEP 9: Prepare journal details.
                        string transactionId = billID.ToString();

                        string journalDescription =
                            $"Payment by {methodName} for Bill #{billID}";

                        DateTime journalDate = DateTime.Now;

                        string amount = paymentAmount.ToString(
                            System.Globalization.CultureInfo.InvariantCulture);

                        // STEP 10: DEBIT selected payment account.
                        CommonInsert.InsertTransactionEntryWithTransaction(
                            connection,
                            transaction,
                            journalDate,
                            debitAccountId.ToString(),
                            amount,
                            "0",
                            transactionId,
                            "0",
                            journalType,
                            journalDescription,
                            userID,
                            journalDate);

                        // STEP 11: CREDIT Accounts Receivable.
                        CommonInsert.InsertTransactionEntryWithTransaction(
                            connection,
                            transaction,
                            journalDate,
                            creditAccountId.ToString(),
                            "0",
                            amount,
                            transactionId,
                            "0",
                            journalType,
                            journalDescription,
                            userID,
                            journalDate);

                        // STEP 12: Commit everything together.
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
                                "Payment rollback failed: " +
                                rollbackEx);
                        }

                        if (ex is MySqlException mysqlEx)
                        {
                            clsGlobalLogger.LogMySqlException(
                                mysqlEx,
                                clsGlobalLogger.LogLevel.Error,
                                userID);
                        }

                        throw new InvalidOperationException(
                            "Failed to save payment for BillID: " +
                            billID, ex);
                    }
                }
            }
        }

    }
}
