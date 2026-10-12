using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Clinic_DataAccess
{
   public static partial class ClinicDatabase
    {
        private const string DatabaseName = "clinic_management";

        // Change these values to your local MySQL credentials.
        private static readonly string ServerConnection =
            "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;SslMode=None;";

        private static string ConnectionString =>
            ServerConnection + "Database=" + DatabaseName + ";";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // Create database and all initial tables.
        public static void InitializeDatabase()
        {
            using (var connection = new MySqlConnection(ServerConnection))
            {
                connection.Open();

                using (var command = new MySqlCommand(
                    @"CREATE DATABASE IF NOT EXISTS `clinic_management`
                      CHARACTER SET utf8mb4
                      COLLATE utf8mb4_unicode_ci;", connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            string[] tables =
            {
                @"CREATE TABLE IF NOT EXISTS tbl_patient (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    patient_code VARCHAR(50) NOT NULL UNIQUE,
                    full_name VARCHAR(200) NOT NULL,
                    gender VARCHAR(20),
                    dob DATE,
                    mobile VARCHAR(30),
                    email VARCHAR(150),
                    address VARCHAR(500),
                    blood_group VARCHAR(10),
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                ) ENGINE=InnoDB;",
                // ... (rest unchanged)
            };

            using (var connection = GetConnection())
            {
                connection.Open();

                foreach (string sql in tables)
                {
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // INSERT / UPDATE / DELETE
        public static int ExecuteNonQuery(
            string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        // SELECT: Return DataTable
        public static DataTable ExecuteDataTable(
            string query, params MySqlParameter[] parameters)
        {
            var table = new DataTable();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                connection.Open();

                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        // SELECT: Return one value
        public static object ExecuteScalar(
            string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                connection.Open();
                return command.ExecuteScalar();
            }
        }
        public static DataRow FindApprovedRegistration(
      string username,
      string passwordHash)
        {
            DataTable dt = ExecuteDataTable(@"
        SELECT
            RegistrationID,
            FullName,
            Username,
            PasswordHash,
            IsActive,
            ApprovalStatus
        FROM tbl_registration_account
        WHERE Username = @Username
          AND PasswordHash = @PasswordHash
          AND IsActive = 1
          AND ApprovalStatus = 'Approved'
        LIMIT 1;",
                new MySqlParameter("@Username", username),
                new MySqlParameter("@PasswordHash", passwordHash));

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
        // Check connection
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}