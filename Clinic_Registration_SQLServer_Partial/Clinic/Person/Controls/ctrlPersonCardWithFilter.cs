using Clinic.Person;
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
    public partial class ctrlPersonCardWithFilter : UserControl
    {

        public event Action<int> OnPersonSelected;
        // Create a protected method to raise the event with a parameter
        protected virtual void PersonSelected(int PersonID)
        {
            Action<int> handler = OnPersonSelected;
            if (handler != null)
            {
                handler(PersonID); // Raise the event with the parameter
            }
        }

        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();
        }

        public int PersonID { get { return ctrlPersonCard1.PersonID; } }

        public clsPerson SelectedPerson { get { return ctrlPersonCard1.SelectedPersonInfo; } }

        private bool _FilterEnabled = false;
        public bool FilterEnabled
        {
            set
            {
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
                btnAddNewPerson.Enabled = _btnAddNewEnabled;
            }
            get { return _btnAddNewEnabled; }
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void _LoadPersonInfo()
        {
            ctrlPersonCard1.LoadPersonInfo(int.Parse(txtFilterValue.Text));
            if (OnPersonSelected != null && FilterEnabled)
                OnPersonSelected(this.PersonID);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }


            _LoadPersonInfo();


        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFilterValue, "This field is required!");
            }
            else
            {
                //e.Cancel = false;
                errorProvider1.SetError(txtFilterValue, null);
            }
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frmAdd = new frmAddUpdatePerson();
            frmAdd.DataBack += _DataBack;
            frmAdd.ShowDialog();
        }

        private void _DataBack(object sender, int PersonID)
        {
            txtFilterValue.Text = PersonID.ToString();
            _LoadPersonInfo();

        }

        public void LoadPersonInfo(int PersonID)
        {
            txtFilterValue.Text = PersonID.ToString();
            _LoadPersonInfo();
        }

        public void FocuseTextBox()
        {
            txtFilterValue.Focus();
        }

    }
}
