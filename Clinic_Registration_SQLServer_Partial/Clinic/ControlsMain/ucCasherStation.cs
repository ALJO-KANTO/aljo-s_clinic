using Clinic.Doctor;
using Clinic.Medical_Services.Appointment;
using Clinic.Medical_Services.Casher;
using Clinic.Medical_Services.Visit;
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
    public partial class ucCasherStation : UserControl
    {
        public ucCasherStation()
        {
            InitializeComponent();
        }
        private void btnListAppoinment_Click(object sender, EventArgs e)
        {
            clsFormHelper.ShowForm<frmListAppointmets>();
        }

        private void btnMangeBills_Click(object sender, EventArgs e)
        {
            clsFormHelper.ShowForm<frmManageBills>();
        }

        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            clsFormHelper.ShowForm<frmProcessPayments>();
        }

        private void btnIssueInvoice_Click(object sender, EventArgs e)
        {
            clsFormHelper.ShowForm<frmIssueInvoice>();
        }
    }
}
