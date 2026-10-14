using Clinic.Login;
using Clinic_Business;
using Clinic_DataAccess;
using System;
using System.Windows.Forms;

namespace Clinic
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                ClinicDatabase.ApplyMigrations();

                MessageBox.Show(
                    "Database migrations completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Migration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            int navigate = 0;

            // Show splash screen first
            using (frmSplashScreen splash = new frmSplashScreen())
            {
                splash.ShowDialog();
                navigate = splash.NavigationCode;
            }

            // Apply database migrations before loading application data
            try
            {
                ClinicDatabase.ApplyMigrations();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database migration failed.\n\n" +
                    ex.ToString(),
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (navigate == 1)
            {
                Application.Run(new frmConfig());
            }
            else if (navigate == 2)
            {
                clsCountry.GetAllCountriesList();
                Application.Run(new frmLogin());
            }
            else
            {
                Application.Run(new frmConfig());
            }
        }
    }
}