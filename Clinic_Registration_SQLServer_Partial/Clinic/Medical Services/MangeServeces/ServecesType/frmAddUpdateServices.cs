using Clinic_Business;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Clinic.Serveces.ServecesType
{
    public partial class frmAddUpdateServices : Form
    {
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;
        private int _ServiceID = -1;
        private clsServicesType _ServicesType;

        public frmAddUpdateServices()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdateServices(int serviceID)
        {
            InitializeComponent();
            _ServiceID = serviceID;
            _Mode = enMode.Update;
        }

        private void _RefreshData()
        {
            if (_Mode == enMode.AddNew)
            {
                _ServicesType = new clsServicesType();
                _ServiceID = -1;
                lblTitle.Text = "Add New Service";
            }
            else
            {
                lblTitle.Text = "Update Service";
            }
            Text = lblTitle.Text;
            lblApplicationTypeID.Text = "[???]";
            txtServiceName.Clear();
            txtServiceDescription.Clear();
            txtServicePrice.Clear();
        }

        private void _LoadData()
        {
            try
            {
                _ServicesType = clsServicesType.GetServiceByID(_ServiceID);
                if (_ServicesType == null)
                {
                    MessageBox.Show("Service not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                lblApplicationTypeID.Text = _ServicesType.ServiceID.ToString();
                txtServiceName.Text = _ServicesType.ServiceName;
                txtServiceDescription.Text = _ServicesType.Description;
                txtServicePrice.Text = _ServicesType.ServiceFees.ToString("0.00", CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Service loading failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }


        private void frmAddUpdateServices_Load(
            object sender, EventArgs e)
        {
            _RefreshData();

            if (_Mode == enMode.Update)
                _LoadData();

            _ApplyServiceLanguage();
        }


        private void TextValiditn(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;
            // Description is optional. Validate the required fields explicitly on Save.
            if (textBox == txtServiceDescription) { e.Cancel = false; errorProvider1.SetError(textBox, ""); return; }
            e.Cancel = string.IsNullOrWhiteSpace(textBox.Text);
            errorProvider1.SetError(textBox, e.Cancel ? "This field is required." : "");
        }

        private void txtServicePrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            char separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;
            if ((e.KeyChar == separator || e.KeyChar == '.') &&
                !txtServicePrice.Text.Contains(separator.ToString()) && !txtServicePrice.Text.Contains(".")) return;
            e.Handled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtServiceName.Text.Trim();
            string priceText = txtServicePrice.Text.Trim();
            errorProvider1.SetError(txtServiceName, "");
            errorProvider1.SetError(txtServicePrice, "");
            if (name.Length == 0)
            {
                errorProvider1.SetError(txtServiceName, "Service name is required.");
                MessageBox.Show("Enter a service name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            float price;
            if (!float.TryParse(priceText, NumberStyles.Number, CultureInfo.CurrentCulture, out price) &&
                !float.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out price))
            {
                errorProvider1.SetError(txtServicePrice, "Enter a valid price.");
                MessageBox.Show("Enter a valid service price.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (float.IsNaN(price) || float.IsInfinity(price) || price < 0)
            {
                MessageBox.Show("Service price must be zero or greater.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (_ServicesType == null)
                    throw new InvalidOperationException("Service model was not initialized.");
                _ServicesType.ServiceName = name;
                _ServicesType.Description = txtServiceDescription.Text.Trim();
                _ServicesType.ServiceFees = price;

                if (_ServicesType.Save())
                {
                    MessageBox.Show(
     _T("Service saved successfully."),
     _T("Success"),
     MessageBoxButtons.OK,
     MessageBoxIcon.Information);
                    _Mode = enMode.Update;
                    _ServiceID = _ServicesType.ServiceID;
                    lblApplicationTypeID.Text = _ServiceID.ToString();
                    lblTitle.Text = "Update Service";
                    Text = lblTitle.Text;
                    _ApplyServiceLanguage();
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("clsServicesType.Save() returned false. Check the Save(), AddNewService(), and data-access INSERT methods for the underlying SQL error. No database error details were provided to this form.",
                        "Service was not saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Service save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool _IsArabic()
        {
            string language =
                Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.Equals(
                "ar",
                StringComparison.OrdinalIgnoreCase);
        }

        private string _T(string english)
        {
            if (!_IsArabic())
                return english;

            string arabic;

            if (TranslationClass.sidebarTranslations.TryGetValue(
                english, out arabic))
            {
                return arabic;
            }

            return english;
        }

        private void _ApplyServiceLanguage()
        {
            bool arabic = _IsArabic();

            // Keep the existing form layout.
            // Translate labels and align their text.
            foreach (Control control in GetAllServiceControls(this))
            {
                if (control is Label label)
                {
                    string english = label.Text.Trim();

                    label.Text = _T(english);

                    if (arabic)
                        label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                }
                else if (control is Button button)
                {
                    button.Text = _T(button.Text.Trim());
                }
                else if (control is TextBox textBox)
                {
                    // Keep prices and numbers left-to-right.
                    if (textBox == txtServicePrice)
                    {
                        textBox.RightToLeft = RightToLeft.No;
                        textBox.TextAlign = HorizontalAlignment.Left;
                    }
                    else
                    {
                        textBox.RightToLeft =
                            arabic ? RightToLeft.Yes : RightToLeft.No;
                    }
                }
            }

            // Explicitly translate dynamic controls.
            lblTitle.Text = _T(
                _Mode == enMode.AddNew
                    ? "Add New Service"
                    : "Update Service");

            Text = lblTitle.Text;

            // Do not mirror the entire form.
            // This avoids disturbing Designer positions.
            RightToLeft = RightToLeft.No;
            RightToLeftLayout = false;
        }

        private System.Collections.Generic.IEnumerable<Control>
            GetAllServiceControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;

                foreach (Control child in GetAllServiceControls(control))
                    yield return child;
            }
        }

        private void btnExit_Click(object sender, EventArgs e) { Close(); }
    }
}
