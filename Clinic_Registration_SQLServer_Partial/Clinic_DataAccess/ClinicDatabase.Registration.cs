using MySql.Data.MySqlClient;
using System;
using System.Data;
namespace Clinic_DataAccess
{
    // Change the existing ClinicDatabase declaration to:
    // public static partial class ClinicDatabase
    // Then add this file to the Clinic_DataAccess project.
    public static partial class ClinicDatabase
    {
        public static void InitializeRegistrationTables()
        {
            string[] statements = {
                @"CREATE TABLE IF NOT EXISTS tbl_country (
                    CountryID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    CountryName VARCHAR(120) NOT NULL UNIQUE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                @"CREATE TABLE IF NOT EXISTS tbl_registration_account (
                    RegistrationID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    FullName VARCHAR(200) NOT NULL,
                    DateOfBirth DATE NOT NULL,
                    Gender TINYINT UNSIGNED NOT NULL,
                    Phone VARCHAR(20) NOT NULL,
                    Email VARCHAR(150) NULL,
                    Address VARCHAR(500) NOT NULL,
                    CountryID INT NOT NULL,
                    Username VARCHAR(20) NOT NULL UNIQUE,
                    PasswordHash VARCHAR(255) NOT NULL,
                    IsActive TINYINT(1) NOT NULL DEFAULT 0,
                    ApprovalStatus VARCHAR(20) NOT NULL DEFAULT 'Pending',
                    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ApprovedAt DATETIME NULL,
                    ApprovedBy VARCHAR(100) NULL,
                    CONSTRAINT fk_registration_country FOREIGN KEY (CountryID)
                        REFERENCES countries(CountryID)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"
            };
            using (var connection = GetConnection())
            {
                connection.Open();
                foreach (string sql in statements)
                    using (var command = new MySqlCommand(sql, connection))
                        command.ExecuteNonQuery();
            }
        }

        public static DataTable GetRegistrationCountries()
        {
            return ExecuteDataTable(@"
        SELECT
            CountryID,
            CountryName
        FROM countries
        ORDER BY CountryName ASC;");
        }


        public static int RegisterClinicAccount(
            string fullName,
            DateTime dob,
            byte gender,
            string phone,
            string email,
            string address,
            int countryId,
            string username,
            string passwordHash)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                using (MySqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        int personId;
                        int userId;
                        int registrationId;

                        // 1. Check username
                        using (MySqlCommand cmd = new MySqlCommand(@"
                    SELECT COUNT(*)
                    FROM users
                    WHERE UserName = @Username;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Username", username);

                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                throw new InvalidOperationException(
                                    "This username already exists.");
                        }

                        // 2. Insert person
                        using (MySqlCommand cmd = new MySqlCommand(@"
                    INSERT INTO persons
                    (
                        FullName,
                        DateOfBirth,
                        Gender,
                        PhoneNumber,
                        Email,
                        Address,
                        NationalityCountryID
                    )
                    VALUES
                    (
                        @FullName,
                        @DOB,
                        @Gender,
                        @Phone,
                        @Email,
                        @Address,
                        @CountryID
                    );", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@FullName", fullName);
                            cmd.Parameters.AddWithValue(
                                "@DOB", dob.Date);
                            cmd.Parameters.AddWithValue(
                                "@Gender", gender);
                            cmd.Parameters.AddWithValue(
                                "@Phone", phone);
                            cmd.Parameters.AddWithValue(
                                "@Email", email);
                            cmd.Parameters.AddWithValue(
                                "@Address", address);
                            cmd.Parameters.AddWithValue(
                                "@CountryID", countryId);

                            cmd.ExecuteNonQuery();

                            personId = Convert.ToInt32(
                                cmd.LastInsertedId);
                        }

                        // 3. Insert application user
                        // Staff role only. Administrator must be
                        // assigned through an authorized workflow.
                        using (MySqlCommand cmd = new MySqlCommand(@"
                    INSERT INTO users
                    (
                        PersonID,
                        UserName,
                        Password,
                        IsActive,
                        RoleID
                    )
                    VALUES
                    (
                        @PersonID,
                        @Username,
                        @Password,
                        1,
                        5
                    );", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@PersonID", personId);
                            cmd.Parameters.AddWithValue(
                                "@Username", username);
                            cmd.Parameters.AddWithValue(
                                "@Password", passwordHash);

                            cmd.ExecuteNonQuery();

                            userId = Convert.ToInt32(
                                cmd.LastInsertedId);
                        }

                        // 4. Save registration history
                        using (MySqlCommand cmd = new MySqlCommand(@"
                    INSERT INTO tbl_registration_account
                    (
                        FullName,
                        DateOfBirth,
                        Gender,
                        Phone,
                        Email,
                        Address,
                        CountryID,
                        Username,
                        PasswordHash,
                        IsActive,
                        ApprovalStatus,
                        ApprovedAt
                    )
                    VALUES
                    (
                        @FullName,
                        @DOB,
                        @Gender,
                        @Phone,
                        @Email,
                        @Address,
                        @CountryID,
                        @Username,
                        @PasswordHash,
                        1,
                        'Approved',
                        NOW()
                    );", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@FullName", fullName);
                            cmd.Parameters.AddWithValue(
                                "@DOB", dob.Date);
                            cmd.Parameters.AddWithValue(
                                "@Gender", gender);
                            cmd.Parameters.AddWithValue(
                                "@Phone", phone);
                            cmd.Parameters.AddWithValue(
                                "@Email", email);
                            cmd.Parameters.AddWithValue(
                                "@Address", address);
                            cmd.Parameters.AddWithValue(
                                "@CountryID", countryId);
                            cmd.Parameters.AddWithValue(
                                "@Username", username);
                            cmd.Parameters.AddWithValue(
                                "@PasswordHash", passwordHash);

                            cmd.ExecuteNonQuery();

                            registrationId = Convert.ToInt32(
                                cmd.LastInsertedId);
                        }

                        transaction.Commit();

                        return registrationId;
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();

                        if (ex.Number == 1062)
                            throw new InvalidOperationException(
                                "This username is already registered.", ex);

                        throw;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Call only from an authenticated, authorized administrator workflow.
        // This helper does not implement authorization by itself.
        public static int ApproveRegistration(int registrationId, string adminUsername)
        {
            return ExecuteNonQuery(@"
                UPDATE tbl_registration_account
                SET IsActive = 1, ApprovalStatus = 'Approved',
                    ApprovedAt = NOW(), ApprovedBy = @Admin
                WHERE RegistrationID = @ID AND ApprovalStatus = 'Pending';",
                new MySqlParameter("@Admin", adminUsername),
                new MySqlParameter("@ID", registrationId));
        }
    }
}
