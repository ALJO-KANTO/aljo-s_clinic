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
            dgvNurseQueue.DataSource = _dtAllVitals;
            lblRecordsCount.Text = _dtAllVitals.DefaultView.Count.ToString();
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

        }

        private void frmNurse_Load(object sender, EventArgs e)
        {
            _RefreashData();
            _SetupNurseGrid();
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
                MessageBox.Show("Please select a patient from the queue first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please correct the errors in the input fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int currentAppID = (int)dgvNurseQueue.CurrentRow.Cells["AppointmentID"].Value;
            _Vital.AppointmentID = currentAppID;
            _Vital.BloodPressure = txtBloodPressure.Text.Trim();
            _Vital.Pulse = (short)nudPulse.Value;
            _Vital.Temperature = nudTemperature.Value;
            _Vital.Weight = nudWeight.Value;
            _Vital.CreatedByUserID = clsGlobal.CurrentUser.UserID;
           _Vital.VisitID= (int)dgvNurseQueue.CurrentRow.Cells["VisitID"].Value;
            if (_Vital.Save())
            {
                clsAppointment.UpdateAppointmentStatus(currentAppID, clsAppointment.enAppointmentStatus.Ready_For_Doctor, clsGlobal.CurrentUser.UserID);

                _CallNextPatientInQueue();

                MessageBox.Show("Vital signs saved successfully, and the next patient has been called.", "Process Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _ResetForm();

            }
            else
            {
                MessageBox.Show("An error occurred while saving data. Please try again.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _CallNextPatientInQueue()
        {
            DataTable dtQueue = clsVital.GetPatientsWaitingForVitals();

            if (dtQueue.Rows.Count > 0)
            {
                int nextAppointmentID = (int)dtQueue.Rows[0]["AppointmentID"];
                bool IsWaiting_For_Vitals = dtQueue.Rows[0]["StatusText"].ToString() == "Waiting_For_Vitals" ? true : false;
                if(!IsWaiting_For_Vitals)
                    clsAppointment.UpdateAppointmentStatus(nextAppointmentID, clsAppointment.enAppointmentStatus.Waiting_For_Vitals, clsGlobal.CurrentUser.UserID);


                clsAppointment.UpdatePatientCallStatus(nextAppointmentID, true, 1);
                _RefreashData();
                lbPatientName.Text = dgvNurseQueue.CurrentRow.Cells["PatientName"].Value.ToString();

            }

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

        private void btnWaiting_For_Vitals_Click(object sender, EventArgs e)
        {
            if(dgvNurseQueue.Rows.Count==0) return;
            bool IsWaiting_For_Vitals = dgvNurseQueue.CurrentRow.Cells["StatusText"].ToString() == "Waiting_For_Vitals" ? true : false;
            if (IsWaiting_For_Vitals)
                return;
            int AppoinmetnID = (int)dgvNurseQueue.CurrentRow.Cells["AppointmentID"].Value;

            if (clsAppointment.UpdateAppointmentStatus(AppoinmetnID, clsAppointment.enAppointmentStatus.Waiting_For_Vitals, clsGlobal.CurrentUser.UserID))
                _ResetForm();


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
