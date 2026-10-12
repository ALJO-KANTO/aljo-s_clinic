
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Clinic_DataAccess
{
    public class clsCountryData
    {
        public static bool GetCountryInfoByID(
            int ID,
            ref string CountryName)
        {
            using (MySqlConnection connection =
                ClinicDatabase.GetConnection())
            {
                string query = @"
                    SELECT CountryName
                    FROM countries
                    WHERE CountryID = @CountryID
                    LIMIT 1;";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@CountryID", ID);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        CountryName = result.ToString();
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool GetCountryInfoByName(
            string CountryName,
            ref int ID)
        {
            using (MySqlConnection connection =
                ClinicDatabase.GetConnection())
            {
                string query = @"
                    SELECT CountryID
                    FROM countries
                    WHERE CountryName = @CountryName
                    LIMIT 1;";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@CountryName", CountryName);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        ID = Convert.ToInt32(result);
                        return true;
                    }
                }
            }

            return false;
        }

        public static DataTable GetAllCountries()
        {
            DataTable dt = new DataTable();

            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT *
            FROM countries
            ORDER BY CountryName;";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }
    }
}
