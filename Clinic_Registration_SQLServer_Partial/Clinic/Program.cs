
using Clinic.Login;
using Clinic_Business;
using Clinic_DataAccess;
using System;
using System.Windows.Forms;

namespace Clinic
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point for the Clinic application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // STEP 1: Connect to MySQL
                using (var connection = ClinicDatabase.GetConnection())
                {
                    connection.Open();

                    System.Diagnostics.Debug.WriteLine(
                        "Connected to MySQL: " +
                        connection.DataSource + " / " +
                        connection.Database);
                }

                // STEP 2: Apply pending database migrations
                // This creates schema_migrations automatically.
                ClinicDatabase.ApplyMigrations();

                System.Diagnostics.Debug.WriteLine(
                    "Database migrations completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database initialization failed:\n\n" +
                    ex.ToString(),
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // STEP 3: Existing application startup
            clsCountry.GetAllCountriesList();

            // STEP 4: Open login form
            Application.Run(new frmLogin());
        }
    }
}
