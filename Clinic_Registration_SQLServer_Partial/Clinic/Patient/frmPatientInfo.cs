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
    public partial class frmPatientInfo : Form
    {
        
        public frmPatientInfo(int PatientID)
        {
            InitializeComponent();
            ctrlPatienCardtInfo1.LoadPatientInfo(PatientID);
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
