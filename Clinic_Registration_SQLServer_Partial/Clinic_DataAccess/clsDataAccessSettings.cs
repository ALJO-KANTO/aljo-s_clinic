using MySql.Data.MySqlClient;

namespace Clinic_DataAccess
{
    public class clsDataAccessSettings
    {
        // abc
        public static string ConnectionString => ClinicDatabase.GetConnectionString() + ";Database=" + ClinicDatabase.DatabaseName + ";";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}