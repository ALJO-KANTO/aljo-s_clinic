using Clinic_DataAccess.SaveExeption;
using System;
using MySql.Data.MySqlClient;

namespace Clinic_DataAccess.SaveMySqlException
{
    public static class clsGlobalLogger
    {
        private static clsLogger _logger =
            new clsLogger(clsLogin.LogToDatabase);

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Critical
        }

        public static void LogMySqlException(
       MySql.Data.MySqlClient.MySqlException ex,
       LogLevel level,
       int userID = -1)
        {
            _logger.Log(ex, level.ToString(), userID);
        }
    }
}