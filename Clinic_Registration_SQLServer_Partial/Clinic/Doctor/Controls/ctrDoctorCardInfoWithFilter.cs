using Clinic.Doctor;
using Clinic.global_classes;
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

namespace Clinic.Controls
{
    public partial class ctrDoctorCardInfoWithFilter : UserControl
    {
        public event EventHandler<clsEventArgs> DoctorCreated;

        public void OnDoctorCreated(int DoctorID, int PersonID)
        {
            if (DoctorCreated != null)
                DoctorCreated(this, new clsEventArgs(DoctorID, PersonID));
        }

        private int _DoctorID = -1;
        public ctrDoctorCardInfoWithFilter()
        {
            InitializeComponent();
        }

        private bool _FilterEnabled = false;
        public bool FilterEnabled
        {
            get
            {
                return _FilterEnabled;
            }
            set
            {
                _FilterEnabled = value;
                gbFilters.Enabled = _FilterEnabled;
            }
        }

        private bool _btnAddNewEnabled = false;
        public bool btnAddNewEnabled
        {
            get
            {
                return _btnAddNewEnabled;
            }
            set
            {
                _btnAddNewEnabled = value;
                btnAddNewDoctor.Enabled = _btnAddNewEnabled;
            }
        }

        public int DoctorID
        {
            get { return ctrDoctorCardInfo1.DoctorID; }
        }

        public clsDoctor SelectedDoctor
        {
            get { return ctrDoctorCardInfo1.SelectedDoctor; }
        }
        public void FilterFocus()
        {
            txtFilterValue.Focus();
        }

        private void _LoadDoctorInfo()
        {
            ctrDoctorCardInfo1.LoadDoctorInfo(_DoctorID);
            if(ctrDoctorCardInfo1.SelectedDoctor == null)
            {
                _DoctorID = -1;
                return;
            }
               
            if (DoctorCreated != null && FilterEnabled)
                OnDoctorCreated(ctrDoctorCardInfo1.DoctorID, ctrDoctorCardInfo1.SelectedDoctor.PersonID);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (!int.TryParse(txtFilterValue.Text, out int doctorID)) return;
            else _DoctorID = doctorID;
            _LoadDoctorInfo();
        }

        private void btnAddNewDoctor_Click(object sender, EventArgs e)
        {
            frmAddUpdateDoctor frm = new frmAddUpdateDoctor();
            frm.DataBack += (s, DoctorID) =>
            {
                _DoctorID = DoctorID;
                txtFilterValue.Text = _DoctorID.ToString();
                _LoadDoctorInfo();
            };
            frm.ShowDialog();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel=string.IsNullOrEmpty(txtFilterValue.Text);
            if(e.Cancel)
                errorProvider1.SetError(txtFilterValue, "Please enter Doctor ID");
            else
                errorProvider1.SetError(txtFilterValue, string.Empty);
        }

        public void LoadDoctorInfo(int DoctorID)
        {
            txtFilterValue.Text = DoctorID.ToString();
            _DoctorID= DoctorID;
            _LoadDoctorInfo();
        }

    }
}
