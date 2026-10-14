
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
                        // STEP 1: Get bill amount and existing payments.
                        decimal totalAmount = 0;
                        decimal paidBefore = 0;
                        int visitID = 0;

                        string billQuery = @"
                            SELECT
                                B.VisitID,
                                B.TotalAmount,
                                COALESCE(
                                    (
                                        SELECT SUM(P.PaymentAmount)
                                        FROM payments P
                                        WHERE P.BillID = B.BillID
                                    ), 0
                                ) AS PaidAmount
                            FROM bills B
                            WHERE B.BillID = @BillID
                            FOR UPDATE;";

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                billQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            using (MySqlDataReader reader =
                                cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception(
                                        "Bill not found: " + billID);

                                visitID = Convert.ToInt32(
                                    reader["VisitID"]);

                                totalAmount = Convert.ToDecimal(
                                    reader["TotalAmount"]);

                                paidBefore = Convert.ToDecimal(
                                    reader["PaidAmount"]);
                            }
                        }

                        decimal balanceDue =
                            totalAmount - paidBefore;

                        if (paymentAmount > balanceDue)
                        {
                            throw new Exception(
                                "Payment exceeds outstanding balance.");
                        }

                        // STEP 2: Insert payment.
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

                        // STEP 3: Calculate final payment status.
                        decimal totalPaid =
                            paidBefore + paymentAmount;

                        string statusText;

                        if (totalPaid >= totalAmount)
                            statusText = "Paid";
                        else if (totalPaid > 0)
                            statusText = "Partial";
                        else
                            statusText = "Pending";

                        // STEP 4: Update bill.
                        string updateBill = @"
                            UPDATE bills
                            SET PaymentStatus = @Status
                            WHERE BillID = @BillID;";

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                updateBill,
                                connection,
                                transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Status", statusText);

                            cmd.Parameters.AddWithValue(
                                "@BillID", billID);

                            cmd.ExecuteNonQuery();
                        }

                        // STEP 5: Update visit only if fully paid.
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

                            // STEP 6: Update prescription status.
                            // Keep the existing status value 3.
                            if (!clsPrescriptionData
                                .UpdatePrescriptionStatusByBill(
                                    billID, 3, transaction))
                            {
                                throw new Exception(
                                    "Failed to update prescription status.");
                            }
                        }

                        // STEP 7: Save all changes.
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
