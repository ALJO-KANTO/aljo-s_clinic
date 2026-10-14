using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Clinic_DataAccess
{
   public static partial class ClinicDatabase
    {
        public const string DatabaseName = "clinic_management";

        public static string GetConnectionString()
        {
            string cc = Environment.GetEnvironmentVariable("clinic_connection", EnvironmentVariableTarget.User);

            string connectionString = string.IsNullOrEmpty(cc) ? "" : CryptoHelper.Decrypt(cc);
            var builder = new MySqlConnectionStringBuilder(connectionString);
            return builder.ToString();
        }

        private static readonly string ServerConnection = GetConnectionString();

        private static string ConnectionString => GetConnectionString() + ";Database=" + DatabaseName + ";";

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
public static class CryptoHelper
{
    // Simple fixed key and IV. Ensure these are 16 bytes for AES-128.
    private static readonly string key = "1234567890123456";  // 16 bytes key for AES-128
    private static readonly string iv = "1234567890123456";   // 16 bytes IV

    // Encrypt method
    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
                swEncrypt.Flush();
                csEncrypt.FlushFinalBlock();
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    // Decrypt method
    public static string Decrypt(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);  // Convert Base64 to byte array

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);  // Ensure key is 16 bytes
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);    // Ensure IV is 16 bytes

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();  // Return the decrypted string
            }
        }
    }
}