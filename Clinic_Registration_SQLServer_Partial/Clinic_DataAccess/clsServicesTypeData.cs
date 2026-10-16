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
                    string query = @"INSERT INTO Services 
                    (ServiceName, ServicePrice)
                     VALUES (@ServiceName, @ServicePrice);
                        SELECT LAST_INSERT_ID();";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@ServicePrice", ServicePrice);

                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            ServiceID = id;
                        }

                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);

                return -1;
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
                    string query = @"UPDATE Services
                        SET ServiceName = @ServiceName,
                            ServicePrice = @ServicePrice
                             WHERE ServiceID = @ServiceID";
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

                return false;
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
                    string query = @"DELETE FROM Services
                            WHERE ServiceID = @ServiceID";
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

                return false;
            }
            return rowsAffected > 0;
        }
        public static bool GetServiceByID(int ServiceID, ref string ServiceName, ref string Description, ref float ServicePrice)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT ServiceName,'' Description,
                            ServicePrice FROM Services
                                WHERE ServiceID = @ServiceID";
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

                return false;
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
                    string query = @"
                                    SELECT
                                        ServiceID,
                                        ServiceName,
                                        ServicePrice AS ServiceFees
                                    FROM services
                                    WHERE IsActive = 1
                                    ORDER BY ServiceName;";
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

            }
            return serviceList;
        }
    }
}
