using Clinic.Doctor;
using Clinic.Medical_Services.Visit;
using Clinic.Patient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.ControlsMain
{
    public partial class ucDoctorStation : UserControl
    {
        public ucDoctorStation()
        {
            InitializeComponent();
        }

        private void btnVisitsDashboard_Click(object sender, EventArgs e)
        {
            frmListVisits frm=new frmListVisits();
            frm.Show();
        }

        private void btnMRecords_Click(object sender, EventArgs e)
        {
            frmDoctor frm=new frmDoctor();
            frm.Show();
        }

        private void btnFindPatient_Click(object sender, EventArgs e)
        {
            frmFindPatient frm=new frmFindPatient();
            frm.Show();
        }

        private void btnDiagonsisNotes_Click(object sender, EventArgs e)
        {

        }
    }
}
