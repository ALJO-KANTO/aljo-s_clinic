using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Clinic.Medical_Services.Vital
{
    public partial class frmNurse : Form
    {

        private DataTable _dtAllVitals;

        private clsVital _Vital = new clsVital();
        public frmNurse()
        {
            InitializeComponent();
        }


        private void _RefreashData()
        {
            _dtAllVitals = clsVital.GetPatientsWaitingForVitals();

            if (_dtAllVitals == null)
            {
                MessageBox.Show("The nurse query returned NULL.");
                return;
            }

            dgvNurseQueue.DataSource = _dtAllVitals;

            lblRecordsCount.Text =
                _dtAllVitals.Rows.Count.ToString();

            System.Diagnostics.Debug.WriteLine(
                "Nurse queue rows: " + _dtAllVitals.Rows.Count);

            foreach (DataColumn col in _dtAllVitals.Columns)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Column: " + col.ColumnName);
            }
        }


        private void _SetupNurseGrid()
        {

            dgvNurseQueue.AutoGenerateColumns = false;

            if (dgvNurseQueue.Columns.Count > 0)
                return;

            // الأعمدة المرئية (نستخدم الأسماء الجديدة لتطابق الجدول)
            dgvNurseQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "VisitID", HeaderText = "VisitID", DataPropertyName = "VisitID", ReadOnly = true, Width = 100 });
            dgvNurseQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "AppointmentID", HeaderText = "App.ID", DataPropertyName = "AppointmentID", ReadOnly = true, Width = 100 });
            dgvNurseQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "PatientName", HeaderText = "Patient Name", DataPropertyName = "PatientName", ReadOnly = true, Width = 200 });
            dgvNurseQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "CheckInTime", HeaderText = "CheckInTime", DataPropertyName = "CheckInTime", ReadOnly = true, Width = 150 });
            dgvNurseQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "StatusText", HeaderText = "StatusText", DataPropertyName = "StatusText", ReadOnly = true, Width = 150 });

            dgvNurseQueue.Columns.Add(
    new DataGridViewCheckBoxColumn
    {
        Name = "IsCalled",
        DataPropertyName = "IsCalled",
        Visible = false
    });

        }

        private void frmNurse_Load(object sender, EventArgs e)
        {
            ApplyNurseModernDesign();

            _SetupNurseGrid();
            _RefreashData();

            timer1.Start();
            timer2.Start();

            if (dgvNurseQueue.Rows.Count > 0)
                _CallNextPatientInQueue();

        }

        private void _ResetForm()
        {
            _RefreashData();

            txtBloodPressure.Text = string.Empty;
            lblPlaceholderBP.Visible = true;
            nudTemperature.Value = 30;
            nudWeight.Value = 20;
            nudPulse.Value = 30;
            errorProvider1.Clear();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _ResetForm();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _RefreashData();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            lbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Dispose();

            timer2.Stop();
            timer2.Dispose();

            this.Close();
        }

        private void txtBloodPressure_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '/';

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvNurseQueue.CurrentRow == null)
            {
                MessageBox.Show(
                    "Please select a patient from the queue first.",
                    "Selection Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateChildren())
            {
                MessageBox.Show(
                    "Please correct the input errors.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int appointmentID = Convert.ToInt32(
                dgvNurseQueue.CurrentRow.Cells["AppointmentID"].Value);

            int visitID = Convert.ToInt32(
                dgvNurseQueue.CurrentRow.Cells["VisitID"].Value);

            _Vital.AppointmentID = appointmentID;
            _Vital.VisitID = visitID;
            _Vital.BloodPressure = txtBloodPressure.Text.Trim();
            _Vital.Pulse = (short)nudPulse.Value;
            _Vital.Temperature = nudTemperature.Value;
            _Vital.Weight = nudWeight.Value;
            _Vital.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            if (!_Vital.Save())
            {
                MessageBox.Show(
                    "Unable to save vital signs.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            bool statusUpdated = clsAppointment.UpdateAppointmentStatus(
                appointmentID,
                clsAppointment.enAppointmentStatus.Ready_For_Doctor,
                clsGlobal.CurrentUser.UserID);

            if (!statusUpdated)
            {
                MessageBox.Show(
                    "Vitals were saved, but the appointment status was not updated. Please retry the status transition without saving duplicate vitals.",
                    "Status Update Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _Vital = new clsVital();

            _ResetForm();

            MessageBox.Show(
                "Vital signs saved successfully. Patient is ready for the doctor.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void _CallNextPatientInQueue()
        {
            DataTable dtQueue = clsVital.GetPatientsWaitingForVitals();

            if (dtQueue == null || dtQueue.Rows.Count == 0)
            {
                lbPatientName.Text = "No patients waiting";
                _RefreashData();
                return;
            }

            DataRow nextRow = null;

            if (dtQueue.Columns.Contains("IsCalled"))
            {
                nextRow = dtQueue.AsEnumerable()
                    .FirstOrDefault(r =>
                        r["IsCalled"] != DBNull.Value &&
                        Convert.ToBoolean(r["IsCalled"]));
            }

            if (nextRow == null)
            {
                nextRow = dtQueue.AsEnumerable()
                    .FirstOrDefault(r =>
                        !dtQueue.Columns.Contains("IsCalled") ||
                        r["IsCalled"] == DBNull.Value ||
                        !Convert.ToBoolean(r["IsCalled"]));
            }

            if (nextRow == null)
            {
                lbPatientName.Text = "No available patient";
                _RefreashData();
                return;
            }

            int appointmentID =
                Convert.ToInt32(nextRow["AppointmentID"]);

            string patientName =
                Convert.ToString(nextRow["PatientName"]);

            string status =
                Convert.ToString(nextRow["StatusText"]);

            bool isCalled =
                dtQueue.Columns.Contains("IsCalled") &&
                nextRow["IsCalled"] != DBNull.Value &&
                Convert.ToBoolean(nextRow["IsCalled"]);

            if (status != "Waiting_For_Vitals")
            {
                bool updated = clsAppointment.UpdateAppointmentStatus(
                    appointmentID,
                    clsAppointment.enAppointmentStatus.Waiting_For_Vitals,
                    clsGlobal.CurrentUser.UserID);

                if (!updated)
                {
                    MessageBox.Show(
                        "Unable to update patient status.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            if (!isCalled)
            {
                clsAppointment.UpdatePatientCallStatus(
                    appointmentID,
                    true,
                    clsGlobal.CurrentUser.UserID);
            }

            lbPatientName.Text = patientName;
            _RefreashData();
        }
        private void txtBloodPressure_Validating(object sender, CancelEventArgs e)
        {

            string input = txtBloodPressure.Text.Trim();
            string pattern = @"^\d{1,3}/\d{1,3}$";

            if (string.IsNullOrEmpty(input))
            {
                errorProvider1.SetError(txtBloodPressure, "This field is required.");
                e.Cancel = true;
            }
            else if (!Regex.IsMatch(input, pattern))
            {
                errorProvider1.SetError(txtBloodPressure, "Invalid format! Please use (e.g., 120/80).");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(txtBloodPressure, null);
                e.Cancel = false;
            }
        }


        private void dgvNurseQueue_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn column in dgvNurseQueue.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }


        }

        private void dgvNurseQueue_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNurseQueue.Rows.Count == 0) return;

            if (dgvNurseQueue.CurrentRow != null && dgvNurseQueue.CurrentRow.Index == 0) return;

            dgvNurseQueue.SelectionChanged -= dgvNurseQueue_SelectionChanged;

            dgvNurseQueue.ClearSelection();
            dgvNurseQueue.Rows[0].Selected = true;
            dgvNurseQueue.CurrentCell = dgvNurseQueue.Rows[0].Cells[0];
            dgvNurseQueue.SelectionChanged += dgvNurseQueue_SelectionChanged;
        }

        private void txtBloodPressure_TextChanged(object sender, EventArgs e)
        {
            lblPlaceholderBP.Visible = string.IsNullOrEmpty(txtBloodPressure.Text);
        }

        private void lblPlaceholderBP_Click(object sender, EventArgs e)
        {
            txtBloodPressure.Focus();
        }

        private void btnWaiting_For_Vitals_Click(
            object sender, EventArgs e)
        {
            if (dgvNurseQueue.CurrentRow == null)
                return;

            string status = Convert.ToString(
                dgvNurseQueue.CurrentRow.Cells["StatusText"].Value);

            if (status == "Waiting_For_Vitals")
                return;

            int appointmentID = Convert.ToInt32(
                dgvNurseQueue.CurrentRow.Cells["AppointmentID"].Value);

            bool updated = clsAppointment.UpdateAppointmentStatus(
                appointmentID,
                clsAppointment.enAppointmentStatus.Waiting_For_Vitals,
                clsGlobal.CurrentUser.UserID);

            if (updated)
            {
                _RefreashData();
            }
            else
            {
                MessageBox.Show(
                    "Unable to update appointment status.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void btnNextPatient_Click(object sender, EventArgs e)
        {

            if (dgvNurseQueue.SelectedRows.Count > 0)
            {
                int AppoinmetnID = (int)dgvNurseQueue.CurrentRow.Cells["AppointmentID"].Value;
                bool IsCalled = Convert.ToBoolean(dgvNurseQueue.CurrentRow.Cells["IsCalled"].Value);
                if (IsCalled)
                    return;
                else
                    _CallNextPatientInQueue();

            }
        }

        private void frmNurse_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Dispose();

            timer2.Stop();
            timer2.Dispose();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}
