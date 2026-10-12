using Clinic.Doctor;
using Clinic.Medical_Services.Pharmaciy;
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
    public partial class ucPharmacistStation : UserControl
    {
        public ucPharmacistStation()
        {
            InitializeComponent();
        }

        private void btnVisitsDashboard_Click(object sender, EventArgs e)
        {
            frmListVisits frm = new frmListVisits();
            frm.Show();
        }

        private void btnMRecords_Click(object sender, EventArgs e)
        {
           
        }

        private void btnInClinicDoctor_Click(object sender, EventArgs e)
        {
            frmPrescriptionDispnsing frm = new frmPrescriptionDispnsing();
            frm.ShowDialog();
        }

        private void btnAllPrescription_Click(object sender, EventArgs e)
        {
            frmAllPrescriptions frm = new frmAllPrescriptions();
            frm.ShowDialog();
        }

        private void btnAddNewDrug_Click(object sender, EventArgs e)
        {
            frmManageDrugs frm = new frmManageDrugs();
            frm.ShowDialog();
        }

        private void btnDirectMedicine_Click(object sender, EventArgs e)
        {
            frmDirictSales frm = new frmDirictSales();
            frm.ShowDialog();
        }
    }
}
