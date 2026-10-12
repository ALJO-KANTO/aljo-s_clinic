using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsSpecializationData
    {
        public static bool GetSpecializationByID(int SpecializationID, ref string SpecializationName)
        {
            bool Isfound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT SpecializationName
                        FROM Specializations WHERE 
                            SpecializationID=@SpecializationID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationID", SpecializationID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            SpecializationName = result.ToString();
                            Isfound = true;
                        }
                    }

                }
            }

            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                Isfound = false;
            }

            return Isfound;
        }

        public static bool GetSpecializationByName(string SpecializationName, ref int SpecializationID)
        {
            bool Isfound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT SpecializationID
                        FROM Specializations WHERE SpecializationName=@SpecializationName";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationName", SpecializationName);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            SpecializationID = id;
                            Isfound = true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                Isfound = false;
            }

            return Isfound;
        }

        public static DataTable GetAllSpecializations()
        {
            DataTable table = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT *
                        FROM Specializations";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                table.Load(reader);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return table;
        }

        public static int AddNewSpecialization(string SpecializationName)
        {
            int SpecializationID = -1;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO Specializations (SpecializationName) 
                        VALUES (@SpecializationName);
                            SELECT LAST_INSERT_ID();";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationName", SpecializationName);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            SpecializationID = id;
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                SpecializationID = -1;
            }
            return SpecializationID;
        }

        public static bool UpdateSpecialization(int SpecializationID, string SpecializationName)
        {
            bool IsUpdated = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Specializations 
                        SET SpecializationName=@SpecializationName
                        WHERE SpecializationID=@SpecializationID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationID", SpecializationID);
                        command.Parameters.AddWithValue("@SpecializationName", SpecializationName);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        IsUpdated = rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                IsUpdated = false;
            }

            return IsUpdated;
        }

        public static bool DeleteSpecialization(int SpecializationID)
        {
            bool IsDeleted = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"DELETE FROM Specializations 
                        WHERE SpecializationID=@SpecializationID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SpecializationID", SpecializationID);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        IsDeleted = rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                IsDeleted = false;
            }
            return IsDeleted;
        }
    }
}

