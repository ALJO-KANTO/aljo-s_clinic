using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess.SaveExeption
{
    public class clsLogin
    {
        private static string GetCleanStackTrace(MySqlException ex)
        {
            if (ex.StackTrace == null) return "No Stack Trace";

            // تقسيم الـ StackTrace إلى أسطر
            string[] lines = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // استخراج فقط الأسطر التي تحتوي على اسم مشروعك (Clinic_)
            var cleanLines = lines.Where(line => line.Contains("Clinic_"));

            // إعادة تجميعها في نص واحد
            return string.Join(Environment.NewLine, cleanLines);
        }
        public static void LogToScreen(MySqlException ex, string Level, int UserID)
        {
            Console.WriteLine($"MySqlException: {ex.Message}, Level: {Level}, User ID: {UserID}");
        }

        public static void LogToFile(
            MySqlException ex,
            string level,
            int userId)
        {
            string fileName = "SystemLogTable.csv";
            string backupFileName = "SystemLogTable_Backup.csv";

            try
            {
                _WriteToFile(fileName, ex, level, userId);
            }
            catch (IOException)
            {
                try
                {
                    _WriteToFile(backupFileName, ex, level, userId);
                }
                catch (Exception logError)
                {
                    System.Diagnostics.Trace.WriteLine(
                        "Critical Failure: Could not write to main or backup log. "
                        + logError.Message);
                }
            }
        }

        private static void _WriteToFile(string path, MySqlException ex, string Level, int UserID)
        {

            bool fileExists = File.Exists(path);
            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Timestamp,LogLevel,UserID,Message,StackTrace,Source,MachineName");
                }

                string line = string.Format("{0},{1},{2},\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Level,
                    UserID == -1 ? "N/A" : UserID.ToString(),
                    ex.Message.Replace(",", " "),
                    GetCleanStackTrace(ex).Replace(",", " "),
                    ex.Source ?? "Unknown",
                    Environment.MachineName
                );
                writer.WriteLine(line);
            }
        }
        public static void LogToDatabase(MySqlException ex, string level, int UserID)
        {
            //write the code to log the message in the database .


            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO SystemLogs (LogLevel, Message, StackTrace, Source, UserID, MachineName, Timestamp)
                             VALUES (@Level, @Message, @StackTrace, @Source, @UserID, @MachineName, @Timestamp)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Level", level);
                        cmd.Parameters.AddWithValue("@Message", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", GetCleanStackTrace(ex));
                        cmd.Parameters.AddWithValue("@Source", (object)ex.Source ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserID", UserID != -1 ? (object)UserID : DBNull.Value);
                        cmd.Parameters.AddWithValue("@MachineName", Environment.MachineName);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException exLog)
            {
                System.Diagnostics.Trace.WriteLine("Database Log Failed, switching to File Log: " + exLog.Message);
                LogToFile(ex, level, UserID);
            }

            
               
                  
        }


        
    }
}
