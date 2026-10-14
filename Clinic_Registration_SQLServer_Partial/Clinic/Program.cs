
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

            int navigate = 0;

            // Splash screen
            using (frmSplashScreen splash = new frmSplashScreen())
            {
                splash.ShowDialog();
                navigate = splash.NavigationCode;
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
