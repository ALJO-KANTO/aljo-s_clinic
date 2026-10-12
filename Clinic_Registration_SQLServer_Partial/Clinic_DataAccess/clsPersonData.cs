using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsPersonData
    {

        public static bool GetPersonByPersonID(
            int PersonID,
            ref string FullName,
            ref DateTime DateOfBirth,
            ref short Gender,
            ref string PhoneNumber,
            ref string Email,
            ref string Address,
            ref int NationalityCountryID,
            ref string ImagePath)
        {
            using (var connection = ClinicDatabase.GetConnection())
            using (var command = new MySql.Data.MySqlClient.MySqlCommand(@"
        SELECT FullName, DateOfBirth, Gender,
               PhoneNumber, Email, Address,
               NationalityCountryID, ImagePath
        FROM persons
        WHERE PersonID = @PersonID
        LIMIT 1;", connection))
            {
                command.Parameters.AddWithValue("@PersonID", PersonID);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return false;

                    FullName = Convert.ToString(reader["FullName"]);
                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                    Gender = Convert.ToInt16(reader["Gender"]);
                    PhoneNumber = Convert.ToString(reader["PhoneNumber"]);
                    Email = Convert.ToString(reader["Email"]);
                    Address = Convert.ToString(reader["Address"]);
                    ImagePath = Convert.ToString(reader["ImagePath"]);

                    NationalityCountryID =
                        reader["NationalityCountryID"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["NationalityCountryID"]);

                    return true;
                }
            }
        }


        public static int AddNewPerson(string FullName, DateTime DateOfBirth, short Gender, string PhoneNumber, string Email,
                                               string Address, string ImagePath, int NationalityCountryID)
        {
            int PersonID = -1;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO Persons (FullName, DateOfBirth, Gender, PhoneNumber, Email, Address, ImagePath, NationalityCountryID)
                             VALUES (@FullName, @DateOfBirth, @Gender, @PhoneNumber, @Email, @Address, @ImagePath, @NationalityCountryID);
                             SELECT LAST_INSERT_ID();";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@FullName", FullName);
                        command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        command.Parameters.AddWithValue("@Gender", Gender);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@Email", Email.ToDBValue());
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@ImagePath", ImagePath.ToDBValue());
                        command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int newPersonID))
                        {
                            PersonID = newPersonID;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);

                PersonID = -1;
            }

            return PersonID;
        }

        public static bool UpdatePerson(int PersonID, string FullName, DateTime DateOfBirth, short Gender, string PhoneNumber, string Email,
                                               string Address, string ImagePath, int NationalityCountryID)
        {
            bool IsUpdated = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Persons SET FullName = @FullName, DateOfBirth = @DateOfBirth, Gender = @Gender,
                             PhoneNumber = @PhoneNumber, Email = @Email, Address = @Address, ImagePath = @ImagePath,
                             NationalityCountryID = @NationalityCountryID WHERE PersonID = @PersonID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@FullName", FullName);
                        command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        command.Parameters.AddWithValue("@Gender", Gender);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@Email", Email.ToDBValue());
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@ImagePath", ImagePath.ToDBValue());
                        command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);

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
        public static bool DeletePerson(int PersonID)
        {
            bool IsDeleted = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"DELETE FROM Persons WHERE PersonID = @PersonID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
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
        public static bool DeleteAllForTrash()
        {
            bool IsDeleted = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"DELETE FROM Persons WHERE IsDeleted = 1";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
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

        public static DataTable GetAllPersons()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM View_PersonDetails;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
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

        public static DataTable GetAllPersonsTrash()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM View_PersonTrashDetails;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
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

        public static DataRow GetPersonByID(int PersonID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM View_PersonDetails
                        Where PersonID=@PersonID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                        }
                    }
                }
            }

            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);

            }

            return (dt.Rows.Count > 0) ? dt.Rows[0] : null;

        }

        public static bool SoftDeletePerson(int PersonID)
        {
            bool IsDeleted = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Persons
                                SET IsDeleted = 1 
                                WHERE PersonID = @PersonID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
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

        public static bool RestorePerson(int PersonID)
        {
            bool IsRestored = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Persons
                     SET IsDeleted = 0 
                     WHERE PersonID = @PersonID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        IsRestored = rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);

                IsRestored = false;
            }
            return IsRestored;
        }
    }
}
