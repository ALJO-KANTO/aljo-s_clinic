using Clinic.Controls;
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

namespace Clinic.Appointment.AppointmentsType
{
    public partial class frmAdddUpdateAppointmentType : Form
    {
        private int _AppointmentTypeID = -1;
        private clsAppointmentType _AppointmentType;
        public enum enMode { AddNew=1, Update=2 }
        private enMode _Mode= enMode.AddNew;
        public frmAdddUpdateAppointmentType(int AppointmentTypeID)
        {
            InitializeComponent();
            _AppointmentTypeID = AppointmentTypeID;
            _Mode = enMode.Update;
        }

        public frmAdddUpdateAppointmentType()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        private void _ResetToDefult()
        {
            if (_Mode == enMode.Update)
            {
                lblTitle.Text = "Update Appointment Type";

            }
            else
            {
                lblTitle.Text = "Add New Appointment Type";
                _AppointmentType=new clsAppointmentType();
                _AppointmentTypeID = -1;
            }

            this.Text = lblTitle.Text;
            lblApplicationTypeID.Text = "[???]";
            txtAppointmentTypeFees.Text = "";
            txtAppointmentTypeName.Text = "";
            txtDuration.Text = "";
            chkIsActive.Checked = true;
        }

        private void _Load()
        {
            _AppointmentType=clsAppointmentType.Find(_AppointmentTypeID);
            if(_AppointmentType==null)
            {
                MessageBox.Show("Appointment Type not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            lblApplicationTypeID.Text=_AppointmentType.AppointmentTypeID.ToString();
            txtAppointmentTypeFees.Text = _AppointmentType.DefaultFees.ToString();
            txtAppointmentTypeName.Text= _AppointmentType.TypeName.ToString();
            txtDuration.Text=_AppointmentType.DefaultDuration.ToString();
            chkIsActive.Checked = _AppointmentType.IsActive;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAppointmentTypeFees_Validating(object sender, CancelEventArgs e)
        {
            if(string.IsNullOrEmpty(txtAppointmentTypeFees.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtAppointmentTypeFees, "Please enter the fees");
            }
            else
                errorProvider1.SetError(txtAppointmentTypeFees, null);
        }

        private void txtAppointmentTypeName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAppointmentTypeFees.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtAppointmentTypeName, "Please enter the Name");
            }
            else
                errorProvider1.SetError(txtAppointmentTypeName, null);
        }

        private void txtAppointmentTypeFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled =!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.';
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!this.ValidateChildren())
            {
                MessageBox.Show("Please enter the required data");
                return;
            }

            _AppointmentType.TypeName= txtAppointmentTypeName.Text.Trim();
            _AppointmentType.DefaultFees = Convert.ToSingle(txtAppointmentTypeFees.Text.Trim());

            if(_AppointmentType.Save())
            {
                lblApplicationTypeID.Text = _AppointmentType.AppointmentTypeID.ToString();

                _Mode = enMode.Update;

                lblTitle.Text = "Update Appointment Type";
                this.Text = lblTitle.Text;
                MessageBox.Show("Appointment Type saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Failed to save Appointment Type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmAdddUpdateAppointmentType_Load(object sender, EventArgs e)
        {
            _ResetToDefult();
            if (_Mode == enMode.Update)
                _Load();
        }

        private void txtDuration_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled=!char.IsDigit(e.KeyChar) &&!char.IsControl(e.KeyChar);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
