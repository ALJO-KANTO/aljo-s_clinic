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
            ApplyDoctorListTranslation();
        }
        private void ApplyDoctorListTranslation()
        {
            bool isArabic = TranslationClass.IsArabic;

            // Form title
            this.Text = TranslationClass.Translate(
                "Manage Doctors", isArabic);

            this.RightToLeft =
                isArabic ? RightToLeft.Yes : RightToLeft.No;

            this.RightToLeftLayout = isArabic;

            // Buttons and labels
            TranslateDoctorControls(this);

            // DataGridView headings
            if (dgvDoctors.Columns.Contains("DoctorID"))
                dgvDoctors.Columns["DoctorID"].HeaderText =
                    TranslationClass.Translate("Doctor ID", isArabic);

            if (dgvDoctors.Columns.Contains("PersonID"))
                dgvDoctors.Columns["PersonID"].HeaderText =
                    TranslationClass.Translate("Person ID", isArabic);

            if (dgvDoctors.Columns.Contains("FullName"))
                dgvDoctors.Columns["FullName"].HeaderText =
                    TranslationClass.Translate("Full Name", isArabic);

            if (dgvDoctors.Columns.Contains("LicenseNumber"))
                dgvDoctors.Columns["LicenseNumber"].HeaderText =
                    TranslationClass.Translate("License #", isArabic);

            if (dgvDoctors.Columns.Contains("ConsultationFees"))
                dgvDoctors.Columns["ConsultationFees"].HeaderText =
                    TranslationClass.Translate("Fees", isArabic);

            if (dgvDoctors.Columns.Contains("Specializations"))
                dgvDoctors.Columns["Specializations"].HeaderText =
                    TranslationClass.Translate("Specializations", isArabic);

            if (dgvDoctors.Columns.Contains("WorkingDays"))
                dgvDoctors.Columns["WorkingDays"].HeaderText =
                    TranslationClass.Translate("Working Days", isArabic);

            if (dgvDoctors.Columns.Contains("CountryName"))
                dgvDoctors.Columns["CountryName"].HeaderText =
                    TranslationClass.Translate("Country", isArabic);

            if (dgvDoctors.Columns.Contains("IsActive"))
                dgvDoctors.Columns["IsActive"].HeaderText =
                    TranslationClass.Translate("Is Active", isArabic);

            // Records count
            if (lblRecordsCount != null)
            {
                lblRecordsCount.RightToLeft =
                    isArabic ? RightToLeft.Yes : RightToLeft.No;
            }

            // Grid direction
            dgvDoctors.RightToLeft =
                isArabic ? RightToLeft.Yes : RightToLeft.No;

            // Keep the internal filter values in English because
            // your existing filtering code depends on them.
            cbFilterBy.RightToLeft =
                isArabic ? RightToLeft.Yes : RightToLeft.No;

            cbIsActive.RightToLeft =
                isArabic ? RightToLeft.Yes : RightToLeft.No;
        }



        private void TranslateDoctorControls(Control parent)
        {
            bool isArabic = TranslationClass.IsArabic;

            foreach (Control control in parent.Controls)
            {
                // Do not translate the numeric record-count label.
                // Its value is set by _RefreshUIState().
                if (control != lblRecordsCount)
                {
                    string originalText = control.Tag as string;

                    // If Tag is missing or is not a dictionary key,
                    // try the control's current text.
                    if (string.IsNullOrWhiteSpace(originalText) ||
                        !TranslationClass.sidebarTranslations.ContainsKey(originalText))
                    {
                        originalText = control.Text;
                    }

                    if (!string.IsNullOrWhiteSpace(originalText) &&
                        TranslationClass.sidebarTranslations.ContainsKey(originalText))
                    {
                        control.Tag = originalText;

                        control.Text = TranslationClass.Translate(
                            originalText, isArabic);
                    }
                }

                if (control is Label label)
                {
                    label.RightToLeft = isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                    label.TextAlign = isArabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }
                else if (control is Button button)
                {
                    button.RightToLeft = isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;
                }

                if (control.HasChildren)
                {
                    TranslateDoctorControls(control);
                }
            }

            // Translate the Close button explicitly.
            btnClose.Text = TranslationClass.Translate("Close", isArabic);
            btnClose.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;
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
            ApplyDoctorListTranslation();
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
