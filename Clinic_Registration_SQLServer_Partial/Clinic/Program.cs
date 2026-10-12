using Clinic.Login;
using Clinic.Medical_Services.Pharmaciy;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            clsCountry.GetAllCountriesList();
            Application.Run(new frmLogin());
            //Application.Run(new frmPrescriptionDispnsing());
            //Application.Run(new frm());

        }
    }
}
