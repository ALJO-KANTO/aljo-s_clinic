using Clinic.Appointment;
using Clinic.Doctor;
using Clinic.Medical_Services.Appointment;
using Clinic.Patient;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Visit
{
    public partial class frmListVisits : Form
    {
       

        static DataTable _dtAllVisits;
        DataTable _dtVists;

        public frmListVisits()
        {
            InitializeComponent();
        }

        private void _RefreshVisitList()
        {
            _dtAllVisits = clsVisit.GetAllVisits();

            if (_dtAllVisits == null)
            {
                MessageBox.Show(
                    "GetAllVisits returned NULL. Check the DataAccess method.",
                    "Visit Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            string[] requiredColumns =
            {
        "VisitID",
        "PatientID",
        "DoctorID",
        "AppointmentID",
        "PatientName",
        "DoctorName",
        "VisitDate",
        "Diagnosis",
        "StatusText",
        "CreatedByUserID"
    };

            string[] missingColumns = requiredColumns
                .Where(c => !_dtAllVisits.Columns.Contains(c))
                .ToArray();

            if (missingColumns.Length > 0)
            {
                MessageBox.Show(
                    "GetAllVisits returned an incorrect table structure.\n\n" +
                    "Records: " + _dtAllVisits.Rows.Count + "\n" +
                    "Missing columns: " + string.Join(", ", missingColumns) + "\n\n" +
                    "Returned columns: " +
                    string.Join(", ",
                        _dtAllVisits.Columns
                            .Cast<DataColumn>()
                            .Select(c => c.ColumnName)),
                    "Visit Data Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                _dtVists = new DataTable();

                foreach (string column in requiredColumns)
                    _dtVists.Columns.Add(column);

                dgvVisit.DataSource = _dtVists.DefaultView;
                lblRecordsCount.Text = "0";

                return;
            }

            _dtVists = _dtAllVisits.DefaultView.ToTable(
                false,
                requiredColumns);

            dgvVisit.DataSource = _dtVists.DefaultView;

            lblRecordsCount.Text =
                _dtVists.DefaultView.Count.ToString();
        }

        private void frmListVisits_Load(object sender, EventArgs e)
        {
            if (clsGlobal.CurrentUser.RoleID== 3 )
            {
                editToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
            }
            _RefreshVisitList();
            cbFilterBy.SelectedIndex = 0;
            lblRecordsCount.Text = dgvVisit.Rows.Count.ToString();
            if (dgvVisit.Columns.Count > 0)
            {
                dgvVisit.Columns["VisitID"].HeaderText = "Visit ID";
                dgvVisit.Columns["VisitID"].Width = 110;

                dgvVisit.Columns["PatientID"].Visible = false; //PatientID
                dgvVisit.Columns["DoctorID"].Visible = false; //DoctorID

                dgvVisit.Columns["AppointmentID"].HeaderText = "App.ID";
                dgvVisit.Columns["AppointmentID"].Width = 90;

                dgvVisit.Columns["PatientName"].HeaderText = "Patient Name";
                dgvVisit.Columns["PatientName"].Width = 200;

                dgvVisit.Columns["DoctorName"].HeaderText = "Doctor Name";
                dgvVisit.Columns["DoctorName"].Width = 200;

                dgvVisit.Columns["VisitDate"].HeaderText = "Visit Date";
                dgvVisit.Columns["VisitDate"].Width = 200;

                dgvVisit.Columns["Diagnosis"].HeaderText = "Diagnosis";
                dgvVisit.Columns["Diagnosis"].Width = 250;

                dgvVisit.Columns["StatusText"].HeaderText = "Status";
                dgvVisit.Columns["StatusText"].Width = 100;

                dgvVisit.Columns["CreatedByUserID"].HeaderText = "C.UserID";
                dgvVisit.Columns["CreatedByUserID"].Width = 90;
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
        }

        private void txtFilterValue_TextChanged(
     object sender,
     EventArgs e)
        {
            if (_dtVists == null)
                return;

            string filterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "Visit ID":
                    filterColumn = "VisitID";
                    break;

                case "Appointment ID":
                    filterColumn = "AppointmentID";
                    break;

                case "Patient Name":
                    filterColumn = "PatientName";
                    break;

                case "Doctor Name":
                    filterColumn = "DoctorName";
                    break;

                case "Diagnosis":
                    filterColumn = "Diagnosis";
                    break;

                case "CreatedByUserID":
                    filterColumn = "CreatedByUserID";
                    break;
            }

            string searchText = txtFilterValue.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText) ||
                string.IsNullOrWhiteSpace(filterColumn) ||
                cbFilterBy.Text == "None")
            {
                _dtVists.DefaultView.RowFilter = "";
                lblRecordsCount.Text =
                    _dtVists.DefaultView.Count.ToString();

                return;
            }

            bool isNumeric =
                filterColumn == "AppointmentID" ||
                filterColumn == "VisitID" ||
                filterColumn == "CreatedByUserID";

            if (isNumeric)
            {
                int number;

                if (int.TryParse(searchText, out number))
                {
                    _dtVists.DefaultView.RowFilter =
                        $"[{filterColumn}] = {number}";
                }
                else
                {
                    _dtVists.DefaultView.RowFilter = "1 = 0";
                }
            }
            else
            {
                string safeText = searchText
                    .Replace("'", "''")
                    .Replace("[", "[[]")
                    .Replace("]", "[]]")
                    .Replace("%", "[%]")
                    .Replace("*", "[*]");

                _dtVists.DefaultView.RowFilter =
                    $"[{filterColumn}] LIKE '%{safeText}%'";
            }

            lblRecordsCount.Text =
                _dtVists.DefaultView.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.Text == "Appointment ID" || cbFilterBy.Text == "Visit ID" || cbFilterBy.Text == "CreatedByUserID")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void ShowAppointmentListtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListAppointmets frm = new frmListAppointmets();
            frm.ShowDialog();
            frmListVisits_Load(null, null);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int VisitID = (int)dgvVisit.CurrentRow.Cells["VisitID"].Value;
            if(clsVisit.IsVisitClosed(VisitID))
            {
                MessageBox.Show("This visit is already closed and cannot be modified.",
                     "Operation Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmDoctor frm = new frmDoctor(VisitID,frmDoctor.enMode.Update);
            frm.ShowDialog();
            frmListVisits_Load(null, null);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int VisitID = (int)dgvVisit.CurrentRow.Cells["VisitID"].Value;
            int AppointmentID = (int)dgvVisit.CurrentRow.Cells["AppointmentID"].Value;
            if (clsVisit.IsVisitClosed(VisitID))
            {
                MessageBox.Show("This visit is already closed and cannot be deleted.",
                     "Operation Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want delete this Visit!", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsVisit.DeleteVisit(VisitID, AppointmentID))
                {
                    MessageBox.Show("Visit has been deleted successfully", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmListVisits_Load(null, null);
                }
                else
                    MessageBox.Show("Visit is not deleted due to data connected to it.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showPatientDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int patientID = (int)dgvVisit.CurrentRow.Cells["PatientID"].Value;
            frmPatientInfo frm = new frmPatientInfo(patientID);
            frm.ShowDialog();
        }

        private void showDoctorDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DoctorID = (int)dgvVisit.CurrentRow.Cells["DoctorID"].Value;
            frmDoctorInfo frm = new frmDoctorInfo(DoctorID);
            frm.ShowDialog();
        }

        private void ShowVisitDetailstoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmDoctor frm = new frmDoctor((int)dgvVisit.CurrentRow.Cells[0].Value,frmDoctor.enMode.ShowVisit);
            frm.ShowDialog();
        }

        private void rescheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
