using Clinic_DataAccess.SaveMySqlException;
using System.Data;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsBloodTypeData
    {
        public static bool FindBloodTypeByID(int BloodTypeID, ref string BloodTypeName)
        {
            bool Isfound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT BloodTypeName
                                FROM BloodTypes WHERE
                                BloodTypeID = @BloodTypeID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BloodTypeID", BloodTypeID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            BloodTypeName = result.ToString();
                            Isfound = true;
                        }
                    }

                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
                Isfound = false;
            }

            return Isfound;
        }

        public static bool FindBloodTypeByName(string BloodTypeName, ref int BloodTypeID)
        {
            bool Isfound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT BloodTypeID
                                FROM BloodTypes WHERE
                                BloodTypeName = @BloodTypeName";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BloodTypeName", BloodTypeName);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int parsedValue))
                        {
                            BloodTypeID = parsedValue;
                            Isfound = true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
                Isfound = false;
            }
            return Isfound;
        }

        public static DataTable GetAllBloodTypes()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM BloodTypes";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
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
    }
}