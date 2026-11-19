
using Clinic_DataAccess.SaveMySqlException;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_DataAccess
{
    public class clsServicesTypeData
    {
        public static int AddNewService(string ServiceName, string Description, float ServicePrice)
        {
            int ServiceID = -1;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // CHANGED: Insert into actual service table instead of view.
                    string query = @"
    INSERT INTO tbl_items
        (name, type, sales_price, active, state, `date`, created_date)
    VALUES
        (@ServiceName, '12 - Service', @ServicePrice, 0, 0, NOW(), NOW());";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@ServicePrice", ServicePrice);

                        conn.Open();

                        // CHANGED: Get inserted ID directly.
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ServiceID = Convert.ToInt32(cmd.LastInsertedId);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                throw;
            }

            return ServiceID;
        }

        public static bool UpdateService(int ServiceID, string ServiceName, string Description, float ServicePrice)
        {
            int rowsAffected = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // CHANGED: Update tbl_items using its real column names.
                    string query = @"
                        UPDATE tbl_items
                        SET name = @ServiceName,
                            sales_price = @ServicePrice
                        WHERE id = @ServiceID
                          AND type = '12 - Service';";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ServiceID", ServiceID);
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@ServicePrice", ServicePrice);

                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                throw;
            }

            return rowsAffected > 0;
        }

        public static bool DeleteService(int ServiceID)
        {
            int rowsAffected = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // CHANGED: Soft delete to preserve invoice history.
                    string query = @"
                        UPDATE tbl_items
                        SET active = 1
                        WHERE id = @ServiceID
                          AND type = '12 - Service';";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ServiceID", ServiceID);
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                throw;
            }

            return rowsAffected > 0;
        }

        public static bool GetServiceByID(int ServiceID, ref string ServiceName, ref string Description, ref float ServicePrice)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // CHANGED: Read service from tbl_items.
                    string query = @"
                        SELECT
                            name AS ServiceName,
                            '' AS Description,
                            sales_price AS ServicePrice
                        FROM tbl_items
                        WHERE id = @ServiceID
                          AND type = '12 - Service';";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ServiceID", ServiceID);
                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ServiceName = reader["ServiceName"].ToString();
                                Description = reader["Description"].ToStringOrEmpty();
                                ServicePrice = Convert.ToSingle(reader["ServicePrice"]);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                throw;
            }

            return false;
        }

        public static DataTable GetServiceList()
        {
            DataTable serviceList = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // CHANGED: List active services from tbl_items.
                    string query = @"
                        SELECT
                            id AS ServiceID,
                            name AS ServiceName,
                            sales_price AS ServiceFees
                        FROM tbl_items
                        WHERE type = '12 - Service'
                         AND active = 0
                        ORDER BY name;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                serviceList.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                throw;
            }

            return serviceList;
        }
    }
}
