using Clinic_DataAccess.SaveMySqlException;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_DataAccess
{
    public class clsDashboardData
    {
        public static DataRow GetDashboardStats()
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT
                    (SELECT COUNT(*)
                     FROM Patients) AS TotalPatients,

                    (SELECT COUNT(*)
                     FROM appointments
                     WHERE DATE(AppointmentDate) = CURDATE()
                       AND AppointmentStatus <> 'Cancelled'
                    ) AS TodayAppointments,

                    (SELECT COALESCE(SUM(PaymentAmount), 0)
                     FROM Payments
                     WHERE DATE(PaymentDate) = CURDATE()
                    ) AS TodayRevenue,

                    (SELECT COUNT(*)
                     FROM visits
                     WHERE DATE(VisitDate) = CURDATE()
                       AND VisitStatus NOT IN ('Completed', 'Cancelled')
                    ) AS ActiveVisits;
            ";

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(
                        clsDataAccessSettings.ConnectionString))
                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error);

                System.Diagnostics.Debug.WriteLine(
                    "GetDashboardStats ERROR: " + ex);

                throw;
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
    }
}
