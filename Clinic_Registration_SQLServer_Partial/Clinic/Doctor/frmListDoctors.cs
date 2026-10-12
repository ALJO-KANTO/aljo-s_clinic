using Clinic.global_classes;
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

namespace Clinic.Doctor
{
    public partial class frmListDoctors : Form
    {
        private DataTable _dtAllDoctors = clsDoctor.GetAllDoctors();
        public frmListDoctors()
        {
            InitializeComponent();
        }

        private void btnAddDoctor_Click(object sender, EventArgs e)
        {
            frmAddUpdateDoctor frm = new frmAddUpdateDoctor();
            frm.DataBack += _DatatBackToAdd;
            frm.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DoctorID = (int)dgvDoctors.CurrentRow.Cells["DoctorID"].Value;
            frmDoctorInfo frm = new frmDoctorInfo(DoctorID);
            frm.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DoctorID = (int)dgvDoctors.CurrentRow.Cells["DoctorID"].Value;
            frmAddUpdateDoctor frm = new frmAddUpdateDoctor(DoctorID);
            frm.DataBack += _DataBackToUpdate;
            frm.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvDoctors.CurrentRow == null) return;

            int DoctorID = (int)dgvDoctors.CurrentRow.Cells["DoctorID"].Value;

            if (MessageBox.Show("Are you sure you want to delete Doctor [" + DoctorID + "]?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (clsDoctor.DeleteDoctor(DoctorID))
                {
                    MessageBox.Show("Doctor Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _DataBackToDelete(DoctorID);
                }
                else
                {
                    MessageBox.Show("Doctor was not deleted because it has data linked to it in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void frmListDoctors_Load(object sender, EventArgs e)
        {
            _RefreshDoctorsList();
        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            string FilterColumn = "IsActive";
            string FilterValue = cbIsActive.Text;

            switch (FilterValue)
            {
                case "Yes":
                    FilterValue = "1";
                    break;
                case "No":
                    FilterValue = "0";
                    break;
                default:
                    FilterValue = "All";
                    break;
            }

            if (FilterValue == "All")
                _dtAllDoctors.DefaultView.RowFilter = "";
            else
                _dtAllDoctors.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, FilterValue);

            lblRecordsCount.Text = _dtAllDoctors.DefaultView.Count.ToString();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool IsNone = cbFilterBy.Text == "None";
            bool IsActive = cbFilterBy.Text == "IsActive";

            txtFilterValue.Visible = !IsNone && !IsActive;
            cbIsActive.Visible = IsActive && !IsNone;
            _dtAllDoctors.DefaultView.RowFilter = string.Empty;
            _RefreshUIState();

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            else if (cbIsActive.Visible)
            {
                cbIsActive.Focus();
                cbIsActive.SelectedIndex = 0;
            }
            else { cbFilterBy.Focus(); }

        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (_dtAllDoctors == null) return;

            string FilterColumn = _GetFilterColumnName(cbFilterBy.Text);

            if (string.IsNullOrEmpty(txtFilterValue.Text) || FilterColumn == "None")
            {
                _dtAllDoctors.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                // استخدام LIKE للنصوص و = للأرقام
                if (FilterColumn == "DoctorID" || FilterColumn == "PersonID" )
                    _dtAllDoctors.DefaultView.RowFilter = $"{FilterColumn} = {txtFilterValue.Text}";
               
                else
                    _dtAllDoctors.DefaultView.RowFilter = $"{FilterColumn} LIKE '{txtFilterValue.Text}%'";
            }
            _RefreshUIState();
        }
        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.Text == "Doctor ID" || cbFilterBy.Text == "Person ID" || cbFilterBy.Text == "ConsultationFees")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void _DatatBackToAdd(object sender, int DoctorID)
        {
            DataRow NewDoctorRow = clsDoctor.GetDoctorByID(DoctorID);

            if (NewDoctorRow != null)
            {
               _dtAllDoctors.UpsertRow(NewDoctorRow,DoctorID);
                _RefreshUIState();
            }

           
        }

        private void _DataBackToUpdate(object sender, int DoctorID)
        {

            DataRow UpdateDoctorRow = clsDoctor.GetDoctorByID(DoctorID);

            if (UpdateDoctorRow != null)
            {
                _dtAllDoctors.UpsertRow(UpdateDoctorRow, DoctorID);
                _RefreshUIState();
            }
        }

        private void _DataBackToDelete(int DoctorID)
        {
            DataRow row = _dtAllDoctors.Rows.Find(DoctorID);
            if (row != null)
            {
                _dtAllDoctors.Rows.Remove(row);
                _dtAllDoctors.AcceptChanges();
                _RefreshUIState();
            }
        }

        private string _GetFilterColumnName(string FilterBy)
        {
            switch (FilterBy)
            {
                case "Doctor ID": return "DoctorID";
                case "Person ID": return "PersonID";
                case "FullName": return "FullName";
                case "LicenseNumber": return "LicenseNumber";
                case "ConsultationFees": return "FeesText";
                case "Specializations": return "Specializations";
                case "WorkingDays": return "WorkingDays";
                case "CountryName": return "CountryName";
                default: return "None";
            }
        }

        private void _SetupDataGridViewColumns()
        {
            SetDoctorColumn("DoctorID", "Doctor ID", 80);
            SetDoctorColumn("PersonID", "Person ID", 80);
            SetDoctorColumn("FullName", "Full Name", 180);
            SetDoctorColumn("LicenseNumber", "License #", 120);
            SetDoctorColumn("ConsultationFees", "Fees", 100);
            SetDoctorColumn("Specializations", "Specializations", 160);
            SetDoctorColumn("WorkingDays", "Working Days", 150);
            SetDoctorColumn("CountryName", "Country", 120);
            SetDoctorColumn("IsActive", "Is Active", 80);
        }

        private void SetDoctorColumn(
            string columnName,
            string header,
            int width)
        {
            if (!dgvDoctors.Columns.Contains(columnName))
                return;

            dgvDoctors.Columns[columnName].HeaderText = header;
            dgvDoctors.Columns[columnName].MinimumWidth = 60;
            dgvDoctors.Columns[columnName].FillWeight = width;
        }
        private void _RefreshDoctorsList()
        {
            _dtAllDoctors = clsDoctor.GetAllDoctors();

            if (_dtAllDoctors == null ||
    !_dtAllDoctors.Columns.Contains("DoctorID"))
            {
                string columns = _dtAllDoctors == null
                    ? "DataTable is null"
                    : string.Join(", ",
                        _dtAllDoctors.Columns
                            .Cast<DataColumn>()
                            .Select(c => c.ColumnName));

                MessageBox.Show(
                    "Doctor data could not be loaded.\n\n" +
                    "Returned columns: " + columns,
                    "Doctor List Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            _dtAllDoctors.PrimaryKey = new DataColumn[]
            {
        _dtAllDoctors.Columns["DoctorID"]
            };

            // Add FeesText safely
            if (!_dtAllDoctors.Columns.Contains("FeesText"))
            {
                DataColumn feesColumn =
                    _dtAllDoctors.Columns.Add("FeesText", typeof(string));

                if (_dtAllDoctors.Columns.Contains("ConsultationFees"))
                {
                    foreach (DataRow row in _dtAllDoctors.Rows)
                    {
                        row["FeesText"] =
                            row["ConsultationFees"] == DBNull.Value
                                ? ""
                                : Convert.ToString(row["ConsultationFees"]);
                    }
                }
            }

            dgvDoctors.AutoGenerateColumns = true;
            dgvDoctors.DataSource = _dtAllDoctors;

            dgvDoctors.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvDoctors.Columns.Contains("FeesText"))
            {
                dgvDoctors.Columns["FeesText"].Visible = false;
            }

            cbFilterBy.Visible = true;
            cbFilterBy.SelectedIndex = 0;

            _SetupDataGridViewColumns();

            _RefreshUIState();
        }
        private void _RefreshUIState()
        {
            lblRecordsCount.Text =
                _dtAllDoctors?.DefaultView.Count.ToString() ?? "0";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
