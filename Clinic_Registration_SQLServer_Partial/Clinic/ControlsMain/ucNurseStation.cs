using Clinic.Medical_Services.Vital;
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
    public partial class ucNurseStation : UserControl
    {
        public ucNurseStation()
        {
            InitializeComponent();
        }

        private void btnNurse_Click(object sender, EventArgs e)
        {
            frmNurse frm=new frmNurse();
            frm.Show();
        }
    }
}
