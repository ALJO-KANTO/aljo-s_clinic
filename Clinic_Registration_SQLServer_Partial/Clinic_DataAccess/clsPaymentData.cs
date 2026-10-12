using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsPaymentData
    {
        public static bool AddPaymentAndSetStatus(int billID, decimal paymentAmount, string paymentMethod, int userID, byte newStatus)
        {
            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // 1. إضافة الدفعة
                    string insertPayment = @"
                                        INSERT INTO Payments (BillID, PaymentAmount, PaymentDate, PaymentMethod, CreatedByUserID)
                                        VALUES (@BillID, @PaymentAmount, GETDATE(), @PaymentMethod, @UserID)";

                    MySqlCommand cmdInsert = new MySqlCommand(insertPayment, connection, transaction);
                    cmdInsert.Parameters.AddWithValue("@BillID", billID);
                    cmdInsert.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
                    cmdInsert.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmdInsert.Parameters.AddWithValue("@UserID", userID);
                    cmdInsert.ExecuteNonQuery();

                    // 2. تحديث الحالة التي أرسلتها أنت من الشاشة
                    string updateBill = "UPDATE Bills SET PaymentStatus = @Status WHERE BillID = @BillID";
                    MySqlCommand cmdUpdate = new MySqlCommand(updateBill, connection, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Status", newStatus);
                    cmdUpdate.Parameters.AddWithValue("@BillID", billID);
                    cmdUpdate.ExecuteNonQuery();

                    if (!clsPrescriptionData.UpdatePrescriptionStatusByBill(billID, 3, transaction))
                    {
                        throw new Exception("Failed to update prescription status.");
                    }

                    transaction.Commit();
                    return true;
                }
                catch (MySqlException ex)
                {
                    transaction.Rollback();
                   clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, userID);
                    return false;
                }
            }
        }

    }
}
