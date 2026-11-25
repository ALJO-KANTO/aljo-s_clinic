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
    public class clsInvoiceData
    {

        public static DataSet GetInvoiceFullData(int billID)
        {
            DataSet ds = new DataSet();

            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();

                // TABLE 0: Financial Summary
                string summaryQuery = @"
            SELECT
                (Subtotal + TotalDiscount) AS Subtotal,
                PaymentAmount,
                TotalDiscount,
                Subtotal AS FinalTotal
            FROM View_BillSummaries
            WHERE BillID = @BillID;";

                using (MySqlCommand command =
                    new MySqlCommand(summaryQuery, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        adapter.Fill(ds, "Summary");
                    }
                }

                // TABLE 1: Invoice Header and Patient Details
                string headerQuery = @"
            SELECT
                B.BillNumber AS InvoiceNumber,
                B.BillDate AS InvoiceDate,
                COALESCE(Per.FullName, 'Pharmacy Sales') AS PatientName,
                COALESCE(Pat.PatientID, -1) AS PatientID

            FROM Bills B

            LEFT JOIN Visits V
                ON B.VisitID = V.VisitID

            LEFT JOIN Patients Pat
                ON Pat.PatientID = COALESCE(B.PatientID, V.PatientID)

            LEFT JOIN Persons Per
                ON Pat.PersonID = Per.PersonID

            WHERE B.BillID = @BillID
            LIMIT 1;";

                using (MySqlCommand command =
                    new MySqlCommand(headerQuery, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        adapter.Fill(ds, "Header");
                    }
                }

                // TABLE 2: Invoice Items
                string itemsQuery = @"
            SELECT *
            FROM View_BillItems
            WHERE BillID = @BillID;";

                using (MySqlCommand command =
                    new MySqlCommand(itemsQuery, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        adapter.Fill(ds, "Items");
                    }
                }
                // TABLE 3: Payment Methods and Amounts
                string paymentQuery = @"
    SELECT
        PaymentMethod,
        SUM(PaymentAmount) AS PaymentAmount
    FROM payments
    WHERE BillID = @BillID
    GROUP BY PaymentMethod
    ORDER BY PaymentMethod;";

                using (MySqlCommand command =
                    new MySqlCommand(paymentQuery, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        adapter.Fill(ds, "Payments");
                    }
                }
            }

            return ds;
        }


        #region IssueInvoice
        public static bool GetBillSummariesToIssueInvoice(int BillID, ref decimal FinalTotal, ref decimal Subtotal,
                                                                        ref decimal PaymentAmount, ref decimal TotalDiscount)
        {
            try
            {

                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // استدعاء الـ View بالكامل
                    string query = @"  SELECT (Subtotal+TotalDiscount) AS Subtotal,PaymentAmount ,TotalDiscount,Subtotal AS FinalTotal
FROM View_BillSummaries                                                 Where BillID=@BillID;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BillID", BillID);
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 1. تعبئة المتغيرات المالية (للعمليات الحسابية في الـ C#)
                                TotalDiscount = Convert.ToDecimal(reader["TotalDiscount"]);
                                FinalTotal = Convert.ToDecimal(reader["FinalTotal"]);
                                Subtotal = Convert.ToDecimal(reader["Subtotal"]);
                                PaymentAmount = Convert.ToDecimal(reader["PaymentAmount"]);


                            }
                            return true;
                        }
                    }
                }

            }

            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }
            return false;
        }
        public static DataTable GetBillItemsToIssueInvoice(int BillID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM View_BillItems
                                         WHERE BillID=@BillID";

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
            return dt;
        }


        public static bool GetINVoiceAndPatientDetalis(
            int BillID,
            ref int InVoiceNumber,
            ref DateTime InVoiceDate,
            ref int PateintID,
            ref string PatientName)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(
                        clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                SELECT
                    B.BillNumber AS InvoiceNumber,
                    B.BillDate AS InvoiceDate,
                    IFNULL(
                        Per.FullName,
                        'Pharmacy Sales'
                    ) AS PatienName,
                    IFNULL(
                        Pat.PatientID,
                        -1
                    ) AS PatientID
                FROM Bills B
                LEFT JOIN Visits V
                    ON B.VisitID = V.VisitID
                LEFT JOIN Patients Pat
                    ON V.PatientID = Pat.PatientID
                LEFT JOIN Persons Per
                    ON Pat.PersonID = Per.PersonID
                WHERE B.BillID = @BillID
                LIMIT 1;";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@BillID", BillID);

                        connection.Open();

                        using (MySqlDataReader reader =
                            command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return false;

                            InVoiceNumber =
                                reader["InvoiceNumber"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        reader["InvoiceNumber"]);

                            InVoiceDate =
                                Convert.ToDateTime(
                                    reader["InvoiceDate"]);

                            PateintID =
                                Convert.ToInt32(
                                    reader["PatientID"]);

                            PatientName =
                                reader["PatienName"].ToString();

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
            }

            return false;
        }
        #endregion
    }
}
