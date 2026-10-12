using Clinic.Medical_Services.Appointment;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Clinic_Business.clsDoctor;

namespace Clinic.ControlsMain
{
    public partial class ucReceptionistStation : UserControl
    {
        public ucReceptionistStation()
        {
            InitializeComponent();
        }

        private void btnVisitsDashboard_Click(object sender, EventArgs e)
        {
            frmAddUpdateAppointment frm=new frmAddUpdateAppointment();
            frm.Show();
        }

        private void btnMRecords_Click(object sender, EventArgs e)
        {

        }

        private void btnFindPatient_Click(object sender, EventArgs e)
        {
            frmListAppointmets frm=new frmListAppointmets();
            frm.Show();
        }

        private void btnDiagonsisNotes_Click(object sender, EventArgs e)
        {
            List<DoctorInfo> allDoctors = clsDoctor.GetAllDoctorsForQueue();

            if (allDoctors.Count > 0)
            {
                frmQueueDisplay frm = new frmQueueDisplay(allDoctors);
                frm.Show();
            }
        }
    }
}
