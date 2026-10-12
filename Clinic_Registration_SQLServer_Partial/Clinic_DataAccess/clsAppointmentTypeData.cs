using Clinic_DataAccess.SaveMySqlException;
using Clinic_DataAccess.SaveExeption;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsAppointmentTypeData
    {
        public static DataTable GetAllAppintmentType()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT AppointmentTypeID, TypeName, DefaultFees FROM AppointmentTypes";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {

                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
            }




            return dt;
        }
        public static bool GetAppointmentTypeByID(int AppointmentTypeID, ref string TypeName, ref float DefaultFees,ref int DefaultDuration,ref bool IsActive)
        {
            bool isFound = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT TypeName, DefaultFees,DefaultDuration,IsActive FROM AppointmentTypes WHERE AppointmentTypeID = @AppointmentTypeID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);

                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TypeName = reader["TypeName"].ToString();
                                DefaultFees = Convert.ToSingle(reader["DefaultFees"]);
                                DefaultDuration = reader["DefaultDuration"]==DBNull.Value?0: (int)reader["DefaultDuration"];
                                IsActive =(Boolean) reader["IsActive"];
                                return true;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
                isFound = false;
            }
            return isFound;
        }
        public static int AddAppointmentType(string TypeName, float DefaultFees,int ?DefaultDuration,bool IsActive)
        {

            int AppointmentTypeID = -1;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO AppointmentTypes 
                            (TypeName, DefaultFees,DefaultDuration,IsActive) 
                                VALUES (@TypeName, @DefaultFees,@DefaultDuration,@IsActive);
                                    SELECT LAST_INSERT_ID();";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TypeName", TypeName);
                        cmd.Parameters.AddWithValue("@DefaultFees", DefaultFees);
                        cmd.Parameters.AddWithValue("@IsActive", IsActive);
                        cmd.Parameters.AddWithValue("DefaultDuration", DefaultDuration??(Object)DBNull.Value);

                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int newID))
                        {
                            AppointmentTypeID = newID;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
            }


            return AppointmentTypeID;
        }
        public static bool UpdateAppointmentType(int AppointmentTypeID, string TypeName, float DefaultFees, int ?DefaultDuration, bool IsActive)
        {
            bool isUpdated = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE AppointmentTypes
                            SET TypeName = @TypeName,
                            DefaultFees = @DefaultFees,
                            DefaultDuration=@DefaultDuration,
                            IsActive=@IsActive
                            WHERE AppointmentTypeID = @AppointmentTypeID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
                        cmd.Parameters.AddWithValue("@TypeName", TypeName);
                        cmd.Parameters.AddWithValue("@DefaultFees", DefaultFees);
                        cmd.Parameters.AddWithValue("@IsActive", IsActive);
                        cmd.Parameters.AddWithValue("DefaultDuration", DefaultDuration ?? (Object)DBNull.Value);
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        isUpdated = rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
                isUpdated = false;
            }


            return isUpdated;
        }
        public static bool DeleteAppointmentType(int AppointmentTypeID)
        {
            bool isDeleted = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"DELETE FROM AppointmentTypes
                    WHERE AppointmentTypeID = @AppointmentTypeID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        isDeleted = rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
                isDeleted = false;
            }


            return isDeleted;

        }

    }
}
