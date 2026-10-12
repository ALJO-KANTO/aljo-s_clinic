using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Patient
{
    public partial class frmFindPatient : Form
    {
        public frmFindPatient()
        {
            InitializeComponent();
        }

      
       

        private void frmFindPatient_Activated(object sender, EventArgs e)
        {
            ctrlPatientCardWithFilter1.FocuseFilter();
        }

        private void ctrlPatientCardWithFilter1_PatientCreated(object sender, global_classes.clsEventArgs e)
        {
            MessageBox.Show($"Patient ID {e.ID} PersonID {e.PersonID}");

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
