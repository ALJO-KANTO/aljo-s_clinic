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
    public class clsVisitServicesData
    {

        public static DataTable GetVisitServices(int VisitID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT VS.ServiceID, S.ServiceName, VS.SavedServicePrice, VS.Quantity, VS.Discount 
                     FROM VisitServices VS
                     INNER JOIN Services S ON VS.ServiceID = S.ServiceID
                     WHERE VS.VisitID = @VisitID";

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


        public static bool SaveVisitServices(int VisitID, DataTable dtServices, MySqlTransaction transaction)
        {

            //DeleteVisitServices(VisitID, transaction);
            if(!dtServices.Columns.Contains("VisitID"))
            dtServices.Columns.Add("VisitID", typeof(int), VisitID.ToString());

            // الإدراج الجماعي
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.DestinationTableName = "VisitServices";
                bulkCopy.ColumnMappings.Add("ServiceID", "ServiceID");
                bulkCopy.ColumnMappings.Add("VisitID", "VisitID");
                bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                bulkCopy.ColumnMappings.Add("SavedServicePrice", "SavedServicePrice");
                bulkCopy.ColumnMappings.Add("Discount", "Discount");

                bulkCopy.WriteToServer(dtServices);
            }
            clsBillingServiceData.UpdatePaymentStatus(VisitID,1, transaction);
            return true;


        }

        public static bool DeleteVisitServices(int VisitID, MySqlTransaction transaction)
        {
            using (MySqlCommand cmdDelete = new MySqlCommand("DELETE FROM VisitServices WHERE VisitID = @VisitID", transaction.Connection, transaction))
            {
                cmdDelete.Parameters.AddWithValue("@VisitID", VisitID);
                cmdDelete.ExecuteNonQuery();
            }


            return true;




        }

    }
}
