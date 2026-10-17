
using Clinic;
using Clinic.Person;
using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Clinic.Controls
{
    public partial class ctrlPersonCardWithFilter : UserControl
    {
        public event Action<int> OnPersonSelected;

        protected virtual void PersonSelected(int PersonID)
        {
            Action<int> handler = OnPersonSelected;
            if (handler != null)
            {
                handler(PersonID);
            }
        }

        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();
            ApplyArabicTranslation();
        }

        private string Tr(string text)
        {
            return TranslationClass.Translate(
                text,
                TranslationClass.IsArabic
            );
        }

        // Translate fixed UI captions only.
        // Keep the existing control positions and layout unchanged.
        private void ApplyArabicTranslation()
        {
            bool isArabic = TranslationClass.IsArabic;

            TranslateStaticControls(this);

            this.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            if (ctrlPersonCard1 != null)
            {
                ctrlPersonCard1.RightToLeft = isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                ApplyRightToLeftToChildren(ctrlPersonCard1, isArabic);
            }
        }

        private void ApplyRightToLeftToChildren(Control parent, bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                control.RightToLeft = isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control.HasChildren)
                {
                    ApplyRightToLeftToChildren(control, isArabic);
                }
            }
        }

        private void TranslateStaticControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label ||
                    control is Button ||
                    control is GroupBox ||
                    control is TabPage ||
                    control is CheckBox ||
                    control is RadioButton ||
                    control is LinkLabel)
                {
                    if (!string.IsNullOrWhiteSpace(control.Text))
                    {
                        control.Text = Tr(control.Text);
                    }
                }

                // Translate captions in nested controls.
                if (control.HasChildren)
                {
                    TranslateStaticControls(control);
                }
            }
        }

        public int PersonID
        {
            get { return ctrlPersonCard1.PersonID; }
        }

        public clsPerson SelectedPerson
        {
            get { return ctrlPersonCard1.SelectedPersonInfo; }
        }

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

        private void txtFilterValue_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            e.Handled =
                !char.IsDigit(e.KeyChar) &&
                !char.IsControl(e.KeyChar);
        }

        private void _LoadPersonInfo()
        {
            ctrlPersonCard1.LoadPersonInfo(
                int.Parse(txtFilterValue.Text)
            );

            if (OnPersonSelected != null && FilterEnabled)
            {
                OnPersonSelected(this.PersonID);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show(
                    Tr("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro"),
                    Tr("Validation Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            _LoadPersonInfo();
        }

        private void txtFilterValue_Validating(
            object sender,
            CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                e.Cancel = true;

                errorProvider1.SetError(
                    txtFilterValue,
                    Tr("This field is required!")
                );
            }
            else
            {
                errorProvider1.SetError(txtFilterValue, null);
            }
        }

        private void btnAddNewPerson_Click(
            object sender,
            EventArgs e)
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
