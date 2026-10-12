using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    // Single data-access entry point for immediate, self-service registration.
    // Uses the SAME SQL Server database as clsUserData and clsPersonData.
    public static class clsRegistrationData
    {
        public static int Register(
     string fullName,
     DateTime birthDate,
     byte gender,
     string phone,
     string email,
     string address,
     int countryId,
     string username,
     string passwordHash)
        {
            using (var connection = ClinicDatabase.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var check = new MySql.Data.MySqlClient.MySqlCommand(@"
                    SELECT COUNT(*)
                    FROM tbl_registration_account
                    WHERE Username = @Username;",
                            connection, transaction))
                        {
                            check.Parameters.AddWithValue("@Username", username);

                            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                                throw new InvalidOperationException(
                                    "This username is already registered.");
                        }

                        using (var command = new MySql.Data.MySqlClient.MySqlCommand(@"
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
                        ApprovalStatus
                    )
                    VALUES
                    (
                        @FullName,
                        @DateOfBirth,
                        @Gender,
                        @Phone,
                        @Email,
                        @Address,
                        @CountryID,
                        @Username,
                        @PasswordHash,
                        1,
                        'Approved'
                    );

                    SELECT LAST_INSERT_ID();",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@FullName", fullName);
                            command.Parameters.AddWithValue("@DateOfBirth", birthDate);
                            command.Parameters.AddWithValue("@Gender", gender);
                            command.Parameters.AddWithValue("@Phone", phone);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Address", address);
                            command.Parameters.AddWithValue("@CountryID", countryId);
                            command.Parameters.AddWithValue("@Username", username);
                            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                            int registrationId =
                                Convert.ToInt32(command.ExecuteScalar());

                            transaction.Commit();

                            return registrationId;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private static int RequiredId(MySqlConnection connection, MySqlTransaction tx,
            string sql, string value, string missingMessage)
        {
            using (var cmd = new MySqlCommand(sql, connection, tx))
            {
                cmd.Parameters.AddWithValue("@value", value);
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException(missingMessage);
                return Convert.ToInt32(result);
            }
        }

        // Called after a successful commit. Never email plaintext passwords.
        public static void SendWelcomeEmail(string email, string username)
        {
            string host = ConfigurationManager.AppSettings["RegistrationSmtpHost"];
            string sender = ConfigurationManager.AppSettings["RegistrationSmtpFrom"];
            string portText = ConfigurationManager.AppSettings["RegistrationSmtpPort"];
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(sender))
                throw new InvalidOperationException("SMTP is not configured. Set RegistrationSmtpHost and RegistrationSmtpFrom in Clinic/App.config.");
            int port;
            if (!int.TryParse(portText, out port)) port = 587;
            using (var message = new MailMessage(sender, email))
            using (var smtp = new SmtpClient(host, port))
            {
                message.Subject = "Clinic account created";
                message.Body = "Your clinic account is ready.\r\nUsername: " + username +
                    "\r\nSign in with the password you chose during registration.\r\n" +
                    "For your security, passwords are not sent by email.";
                smtp.EnableSsl = true;
                string smtpUser = ConfigurationManager.AppSettings["RegistrationSmtpUser"];
                string smtpPassword = ConfigurationManager.AppSettings["RegistrationSmtpPassword"];
                if (!string.IsNullOrWhiteSpace(smtpUser))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                }
                smtp.Send(message);
            }
        }
    }
}
