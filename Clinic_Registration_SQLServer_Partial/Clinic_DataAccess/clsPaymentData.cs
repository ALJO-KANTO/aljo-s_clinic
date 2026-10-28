
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
            byte newStatus)
        {
            if (billID <= 0)
                throw new ArgumentException("Invalid Bill ID.");

            if (paymentAmount <= 0)
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");

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

                        // STEP 1: Lock the bill row.
                        // This helps serialize payments for this bill.
                        string lockBillQuery = @"
                            SELECT VisitID
                            FROM bills
                            WHERE BillID = @BillID
                            FOR UPDATE;";

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                lockBillQuery,
                                connection,
                                transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            object result = cmd.ExecuteScalar();

                            if (result == null ||
                                result == DBNull.Value)
                            {
                                throw new Exception(
                                    "Bill not found: " + billID);
                            }

                            visitID = Convert.ToInt32(result);
                        }

                        // STEP 2: Get the complete bill total.
                        // Includes appointment, medicines and services.
                        string billQuery = @"
                            SELECT
                                COALESCE(Subtotal, 0) AS TotalAmount,
                                COALESCE(PaymentAmount, 0) AS PaidAmount
                            FROM view_billsummaries
                            WHERE BillID = @BillID;";

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                billQuery,
                                connection,
                                transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            using (MySqlDataReader reader =
                                cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    throw new Exception(
                                        "Bill summary not found: " +
                                        billID);
                                }

                                totalAmount =
                                    Convert.ToDecimal(
                                        reader["TotalAmount"]);

                                paidBefore =
                                    Convert.ToDecimal(
                                        reader["PaidAmount"]);
                            }
                        }

                        // STEP 3: Validate outstanding balance.
                        decimal balanceDue =
                            Math.Max(
                                totalAmount - paidBefore,
                                0m);

                        if (paymentAmount > balanceDue)
                        {
                            throw new Exception(
                                "Payment exceeds outstanding balance. " +
                                "Remaining balance: " +
                                balanceDue.ToString("N2"));
                        }

                        // STEP 4: Insert payment.
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

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                insertPayment,
                                connection,
                                transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            cmd.Parameters.AddWithValue(
                                "@PaymentAmount", paymentAmount);

                            cmd.Parameters.AddWithValue(
                                "@PaymentMethod", paymentMethod);

                            cmd.Parameters.AddWithValue(
                                "@UserID", userID);

                            cmd.ExecuteNonQuery();
                        }

                        // STEP 5: Calculate payment status.
                        decimal totalPaid =
                            paidBefore + paymentAmount;

                        string statusText;

                        if (totalPaid >= totalAmount)
                            statusText = "Paid";
                        else if (totalPaid > 0)
                            statusText = "Partial";
                        else
                            statusText = "Pending";

                        // STEP 6: Update stored bill total
                        // and payment status together.
                        string updateBill = @"
                            UPDATE bills
                            SET
                                TotalAmount = @TotalAmount,
                                PaymentStatus = @Status
                            WHERE BillID = @BillID;";

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                updateBill,
                                connection,
                                transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@TotalAmount", totalAmount);

                            cmd.Parameters.AddWithValue(
                                "@Status", statusText);

                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            cmd.ExecuteNonQuery();
                        }

                        // STEP 7: Update visit only if fully paid.
                        if (statusText == "Paid")
                        {
                            string updateVisit = @"
                                UPDATE visits
                                SET VisitStatus = 'Completed'
                                WHERE VisitID = @VisitID
                                  AND VisitStatus = 'Pending_Billing';";

                            using (MySqlCommand cmd =
                                new MySqlCommand(
                                    updateVisit,
                                    connection,
                                    transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@VisitID", visitID);

                                cmd.ExecuteNonQuery();
                            }

                            // STEP 8: Update prescription status.
                            // Preserve your existing status value 3.
                            if (!clsPrescriptionData
                                .UpdatePrescriptionStatusByBill(
                                    billID, 3, transaction))
                            {
                                throw new Exception(
                                    "Failed to update prescription status.");
                            }
                        }

                        // STEP 9: Commit all changes.
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
                                rollbackEx.ToString());
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
