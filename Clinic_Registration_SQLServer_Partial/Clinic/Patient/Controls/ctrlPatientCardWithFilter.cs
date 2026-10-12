using Clinic.global_classes;
using Clinic.Patient;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Controls
{
    public partial class ctrlPatientCardWithFilter : UserControl
    {
      

        public event EventHandler<clsEventArgs> PatientCreated;

        public void OnPatientCreated(int PatientID, int PersonID)
        {
            if(PatientCreated != null)
                PatientCreated(this,new clsEventArgs(PatientID, PersonID));
        }

        public ctrlPatientCardWithFilter()
        {
            InitializeComponent();
        }
        private bool _FilterEnabled=false;

        public bool FilterEnabled
        {
            set { 
                _FilterEnabled = value;
                gbFilters.Enabled = _FilterEnabled;
            }
            get { return _FilterEnabled; }
        }

        private bool _btnAddNewEnabled = false;

        public bool btnAddNewEnabled
        {
            set
            {
                _btnAddNewEnabled = value;
                btnAddNewPatient.Enabled = _btnAddNewEnabled;
            }
            get { return _btnAddNewEnabled; }
        }

        public int PatientID { get { return ctrlPatientCard1.PatientID; } }
        public clsPatient SelectedPatient { get { return ctrlPatientCard1.SelectedPatient; }  }

        private void _LoadPatientInfo()
        {
            // قيد: التأكد أن القيمة قابلة للتحويل
            if (!int.TryParse(txtFilterValue.Text, out int patientID)) return;

            // استدعاء البيانات
            ctrlPatientCard1.LoadPatientInfo(patientID);

            // قيد: هل المريض موجود فعلاً؟
            if (ctrlPatientCard1.SelectedPatient == null) return; // توقف هنا ولا تطلق حدث Created
 

            // هنا المريض موجود، يمكن إطلاق الحدث
            if (PatientCreated != null && FilterEnabled)
                OnPatientCreated(patientID, ctrlPatientCard1.SelectedPatient.PersonID);
        }

        public void FocuseFilter()
        {
            txtFilterValue.Focus();
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            _LoadPatientInfo();
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel=string.IsNullOrEmpty(txtFilterValue.Text);
            if (e.Cancel)
                errorProvider1.SetError(txtFilterValue, "This field is required!");
            else 
                errorProvider1.SetError(txtFilterValue, null);
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled=!char.IsDigit(e.KeyChar)&& !char.IsControl(e.KeyChar);
        }

        private void btnAddNewPatient_Click(object sender, EventArgs e)
        {
            frmAddUpdatePatient frm =new frmAddUpdatePatient();
            frm.DataBack += _DataBack;
            frm.ShowDialog();
        }

        public void LoadPatientInfo(int PatientID)
        {
            txtFilterValue.Text = PatientID.ToString();
            _LoadPatientInfo();
        }
        private void _DataBack(object sender,int PatientID)
        {
            txtFilterValue.Text=string.Empty;
            txtFilterValue.Text=PatientID.ToString();
            _LoadPatientInfo();
        }
    }
}
