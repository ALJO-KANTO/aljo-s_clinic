using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Doctor
{
    public partial class frmFindDoctor : Form
    {
        public frmFindDoctor()
        {
            InitializeComponent();
        }

       
        private void frmFindDoctor_Activated(object sender, EventArgs e)
        {
            ctrDoctorCardInfoWithFilter1.FilterFocus();
        }

        private void ctrDoctorCardInfoWithFilter1_DoctorCreated(object sender, global_classes.clsEventArgs e)
        {
            MessageBox.Show($"Doctor ID {e.ID} PersonID {e.PersonID}");

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
