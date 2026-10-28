using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Controls
{
    public partial class ctrDoctorCardInfo : UserControl
    {
        private int _DoctorID;
        private clsDoctor _Doctor;
        public ctrDoctorCardInfo()
        {
            InitializeComponent();
        }

        public int DoctorID
        {
            get { return _DoctorID; }
        }

        public clsDoctor SelectedDoctor
        {
            get { return _Doctor; }
        }

        public void ResetDoctorInfo()
        {
            _Doctor = null;
            _DoctorID = -1;

            ctrlPersonCard1.ResetPersonInfo();

            lblDoctorID.Text = "[???]";
            lblSpecialization.Text = "[???]";
            lblActiveStatus.Text = "[???]";
            lblConsultationFees.Text = "[???]";
            lblLicenseNo.Text = "[???]";

            dgvShifts.DataSource = null;
        }

        private void _LoadDoctorInfo()
        {
            if (_Doctor == null)
                return;

            _DoctorID = _Doctor.DoctorID;

            // Load person information from database.
            ctrlPersonCard1.LoadPersonInfo(_Doctor.PersonID);

            // Load doctor information.
            lblDoctorID.Text = _Doctor.DoctorID.ToString();

            lblSpecialization.Text = _Doctor.GetSpecializations();

            lblActiveStatus.Text = TranslationClass.Translate(
                _Doctor.IsActive ? "Active" : "Inactive",
                TranslationClass.IsArabic);

            lblActiveStatus.ForeColor = _Doctor.IsActive
                ? Color.Green
                : Color.Red;

            lblConsultationFees.Text =
                _Doctor.ConsultationFees.ToString("N2");

            lblLicenseNo.Text = _Doctor.LicenseNumber;

            // Load working days.
            _LoadDoctorShiftInfo();

            // IMPORTANT:
            // Do not translate the entire UserControl after
            // loading dynamic database values.
        }
        private void _LoadDoctorShiftInfo()
        {
           
            if (_Doctor.dtWorkDays == null)
                return;
            dgvShifts.DataSource = _Doctor.dtWorkDays;
            if (_Doctor.dtWorkDays.Rows.Count == 0)
                return;
            dgvShifts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvShifts.Columns["DayID"].Visible = false;
            dgvShifts.Columns["DayName"].HeaderText = "Day";
            dgvShifts.Columns["DayName"].Width = 100;

            dgvShifts.Columns["StartTime"].HeaderText = "Start Time";
            dgvShifts.Columns["StartTime"].Width = 120;

            dgvShifts.Columns["EndTime"].HeaderText = "End Time";
            dgvShifts.Columns["EndTime"].Width = 120;
        }
        public void LoadDoctorInfo(int DoctorID)
        {
            _Doctor=clsDoctor.FindDoctorByID(DoctorID);
            if(_Doctor==null)
            {
                MessageBox.Show("Doctor not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetDoctorInfo();
                return;
            }

            _LoadDoctorInfo();
        }

      
        
    }
}
