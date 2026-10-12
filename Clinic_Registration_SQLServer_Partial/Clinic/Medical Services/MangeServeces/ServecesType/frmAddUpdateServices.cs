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

        public frmAddUpdateServices(int ServiceID)
        {
            InitializeComponent();
            _ServiceID = ServiceID;
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
            this.Text = lblTitle.Text;
            lblApplicationTypeID.Text = "[???]";
            txtServiceName.Text = string.Empty;
            txtServiceDescription.Text = string.Empty;
            txtServicePrice.Text = string.Empty;
        }

        private void _LoadData()
        {
            _ServicesType = clsServicesType.GetServiceByID(_ServiceID);
            if (_ServicesType != null)
            {
                lblApplicationTypeID.Text = _ServicesType.ServiceID.ToString();
                txtServiceName.Text = _ServicesType.ServiceName;
                txtServiceDescription.Text = _ServicesType.Description;
                txtServicePrice.Text = _ServicesType.ServiceFees.ToString("0.00");
            }
            else
            {
                MessageBox.Show("Service not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

        }
        private void frmAddUpdateServices_Load(object sender, EventArgs e)
        {
            _RefreshData();
            if (_Mode == enMode.Update)
                _LoadData();
        }

        private void TextValiditn(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            e.Cancel = string.IsNullOrEmpty(textBox.Text.Trim());
            if (e.Cancel)
                errorProvider1.SetError(textBox, "This field is required.");
            else
                errorProvider1.SetError(textBox, string.Empty);
        }

        private void txtServicePrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.';
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                MessageBox.Show("Please correct the errors and try again.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _ServicesType.ServiceName = txtServiceName.Text.Trim();
            _ServicesType.Description = txtServiceDescription.Text.Trim();
            _ServicesType.ServiceFees = Convert.ToSingle(txtServicePrice.Text.Trim());

            if (_ServicesType.Save())
            {
                MessageBox.Show("Service saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _Mode = enMode.Update;
                lblApplicationTypeID.Text = _ServicesType.ServiceID.ToString();
                lblTitle.Text = "Update Service";
                this.Text = lblTitle.Text;

            }
            else
            {
                MessageBox.Show("An error occurred while saving the service. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
