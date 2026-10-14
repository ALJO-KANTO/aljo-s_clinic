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


        public static bool SaveVisitServices(
            int VisitID,
            DataTable dtServices,
            MySqlTransaction transaction)
        {
            if (transaction == null || transaction.Connection == null)
                throw new ArgumentNullException(nameof(transaction));

            if (dtServices == null || dtServices.Rows.Count == 0)
                return true;

            string query = @"
        INSERT INTO VisitServices
        (
            VisitID,
            ServiceID,
            SavedServicePrice,
            Quantity,
            Discount
        )
        VALUES
        (
            @VisitID,
            @ServiceID,
            @SavedServicePrice,
            @Quantity,
            @Discount
        );";

            using (MySqlCommand cmd = new MySqlCommand(
                query,
                transaction.Connection,
                transaction))
            {
                cmd.Parameters.Add("@VisitID", MySqlDbType.Int32);
                cmd.Parameters.Add("@ServiceID", MySqlDbType.Int32);
                cmd.Parameters.Add("@SavedServicePrice", MySqlDbType.Decimal);
                cmd.Parameters.Add("@Quantity", MySqlDbType.Decimal);
                cmd.Parameters.Add("@Discount", MySqlDbType.Decimal);

                foreach (DataRow row in dtServices.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                        continue;

                    cmd.Parameters["@VisitID"].Value = VisitID;

                    cmd.Parameters["@ServiceID"].Value =
                        Convert.ToInt32(row["ServiceID"]);

                    cmd.Parameters["@SavedServicePrice"].Value =
                        Convert.ToDecimal(row["SavedServicePrice"]);

                    cmd.Parameters["@Quantity"].Value =
                        Convert.ToDecimal(row["Quantity"]);

                    cmd.Parameters["@Discount"].Value =
                        row["Discount"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(row["Discount"]);

                    cmd.ExecuteNonQuery();
                }
            }

            clsBillingServiceData.UpdatePaymentStatus(
                VisitID,
                1,
                transaction);

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
