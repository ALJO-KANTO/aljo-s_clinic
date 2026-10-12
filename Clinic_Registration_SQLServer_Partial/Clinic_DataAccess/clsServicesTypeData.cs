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
        public static int AddNewService(string ServiceName, string Description, float ServiceFees)
        {
            int ServiceID = -1;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO Services 
                    (ServiceName, Description, ServiceFees)
                     VALUES (@ServiceName, @Description, @ServiceFees);
                        SELECT LAST_INSERT_ID();";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@Description", Description.ToDBValue());
                        cmd.Parameters.AddWithValue("@ServiceFees", ServiceFees);

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
        public static bool UpdateService(int ServiceID, string ServiceName, string Description, float ServiceFees)
        {
            int rowsAffected = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Services
                        SET ServiceName = @ServiceName,
                            Description = @Description,
                            ServiceFees = @ServiceFees
                             WHERE ServiceID = @ServiceID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ServiceID", ServiceID);
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@Description", Description.ToDBValue());
                        cmd.Parameters.AddWithValue("@ServiceFees", ServiceFees);
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
        public static bool GetServiceByID(int ServiceID, ref string ServiceName, ref string Description, ref float ServiceFees)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT ServiceName, Description,
                            ServiceFees FROM Services
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
                                ServiceFees = Convert.ToSingle(reader["ServiceFees"]);
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
                    string query = @"SELECT ServiceID, ServiceName, 
                            Description, ServiceFees
                                FROM Services";
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
