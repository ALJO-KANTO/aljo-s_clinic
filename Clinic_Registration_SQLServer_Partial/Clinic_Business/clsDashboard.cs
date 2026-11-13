using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsDashboard
    {
        public int TotalPatients { get; private set; }
        public int TodayAppointments { get; private set; }
        public decimal TodayRevenue { get; private set; }
        public int ActiveVisits { get; private set; }

        private clsDashboard(DataRow row)
        {
            TotalPatients = Convert.ToInt32(row["TotalPatients"]);
            TodayAppointments = Convert.ToInt32(row["TodayAppointments"]);
            TodayRevenue = Convert.ToDecimal(row["TodayRevenue"]);
            ActiveVisits = Convert.ToInt32(row["ActiveVisits"]);
        }

        public static clsDashboard GetDashboardStats()
        {
            DataRow row = clsDashboardData.GetDashboardStats();

            if (row == null)
                return null;

            return new clsDashboard(row);
        }
    }
}
