using MySql.Data.MySqlClient;

namespace Clinic_DataAccess
{
    public class clsDataAccessSettings
    {
        public static string ConnectionString =
            "Server=127.0.0.1;" +
            "Port=3306;" +
            "Database=clinic_management;" +
            "Uid=root;" +
            "Pwd=;" +
            "SslMode=None;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}