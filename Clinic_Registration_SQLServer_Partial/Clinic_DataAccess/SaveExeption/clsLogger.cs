
using MySql.Data.MySqlClient;

namespace Clinic_DataAccess.SaveExeption
{
    public class clsLogger
    {
        public delegate void LogEventHandler(
            MySqlException ex,
            string level,
            int userID);

        private readonly LogEventHandler _logEvent;

        public clsLogger(LogEventHandler logEvent)
        {
            _logEvent = logEvent;
        }

        public void Log(
            MySqlException ex,
            string level,
            int userID)
        {
            _logEvent(ex, level, userID);
        }
    }
}
