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
using static Clinic_Business.clsAppointmentType;

namespace Clinic.Medical_Services.Appointment
{
    public partial class frmReschedule : Form
    {
        private int _DoctorID = -1;
        private int _PatientID = -1;
        private int _AppointmentID = -1;
        public frmReschedule(int AppointmentID, int DoctorID,int PatientID)
        {
            InitializeComponent();
            _DoctorID=DoctorID;
            _PatientID = PatientID;
            _AppointmentID = AppointmentID;
        }

        private void frmReschedule_Load(object sender, EventArgs e)
        {
            if (clsAppointment.IsPatinentBlakListed(_PatientID))
            {
                MessageBox.Show("This patient is blacklisted and cannot modify appointments.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }
            dtpNewAppointment.MinDate = DateTime.Now;
        }

        private void dtpNewAppointment_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDateTime = new DateTime(
      dtpNewAppointment.Value.Year,
      dtpNewAppointment.Value.Month,
      dtpNewAppointment.Value.Day,
      dtpNewAppointment.Value.Hour,
      dtpNewAppointment.Value.Minute,
      0 // الثواني = 0
                  );

            if (selectedDateTime < DateTime.Now)
            {
                errorProvider1.SetError(dtpNewAppointment, "Date cannot be in the past.");
                btnSave.Enabled = false;
                return;
            }

            bool isAvailable = clsAppointment.IsDoctorAvailable(_DoctorID, selectedDateTime);
            if (!isAvailable)
            {
                errorProvider1.SetError(dtpNewAppointment, "Doctor is not available at this time!");
                btnSave.Enabled = false;
            }
            else
            {
                errorProvider1.Clear();
                btnSave.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please fill all required fields correctly.", "Validation Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (!clsAppointment.IsDoctorAvailable(_DoctorID, dtpNewAppointment.Value))
            {
                MessageBox.Show("Sorry, the doctor is no longer available at this time.", "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpNewAppointment.Value < DateTime.Now)
            {
                MessageBox.Show("Cannot select a date in the past.");
                return;
            }

            // استدعاء دالة إعادة الجدولة (التي تعتمد على إنشاء سجل جديد)
            if (clsAppointment.RescheduleAppointment(_AppointmentID, dtpNewAppointment.Value, clsGlobal.CurrentUser.UserID))
            {
                MessageBox.Show("Rescheduled successfully!");
                this.Close();
            }

        }
    }
}
