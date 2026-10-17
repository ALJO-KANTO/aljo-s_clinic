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

            // Reduce repaint flicker without changing the existing form controls or behavior.
            this.DoubleBuffered = true;

            // Apply the selected layout direction before the form is first displayed.
            bool isArabic = TranslationClass.IsArabic;
            this.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            this.RightToLeftLayout = isArabic;

            // Keep only the filter row and section heading physically mirrored.
            this.Resize += (sender, e) => ApplyArabicFilterAndHeadingLayout();
        }

        private string Tr(string text)
        {
            return TranslationClass.Translate(text, TranslationClass.IsArabic);
        }

        private void ApplyVisitFormTranslation()
        {
            bool isArabic = TranslationClass.IsArabic;

            // RTL direction is set in the constructor before the form is displayed.
            // Translate controls here without repeatedly changing the form layout.
            ApplyTranslationToControls(this.Controls, isArabic);

            TranslateComboItems(cbFilterBy);

            if (dgvVisit.Columns.Count > 0)
            {
                TranslateColumnHeader("VisitID", "Visit ID", isArabic);
                TranslateColumnHeader("AppointmentID", "App.ID", isArabic);
                TranslateColumnHeader("PatientName", "Patient Name", isArabic);
                TranslateColumnHeader("DoctorName", "Doctor Name", isArabic);
                TranslateColumnHeader("VisitDate", "Visit Date", isArabic);
                TranslateColumnHeader("Diagnosis", "Diagnosis", isArabic);
                TranslateColumnHeader("StatusText", "Status", isArabic);
                TranslateColumnHeader("CreatedByUserID", "C.UserID", isArabic);

                dgvVisit.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                dgvVisit.ColumnHeadersDefaultCellStyle.Alignment = isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
                dgvVisit.DefaultCellStyle.Alignment = isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
            }

            // Explicitly translate and mirror the footer Close button.
            if (btnClose != null)
            {
                btnClose.Text = isArabic ? Tr("Close") : "   Close";
                btnClose.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                btnClose.ImageAlign = isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft;
            }

            Control footer = btnClose == null ? null : btnClose.Parent;
            if (footer != null)
                footer.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;

            ApplyFooterMirroring();
            ApplyArabicFilterAndHeadingLayout();
        }

        private void ApplyArabicFilterAndHeadingLayout()
        {
            bool isArabic = TranslationClass.IsArabic;

            // Filter row: keep its existing layout in English; mirror it to the
            // right side when Arabic is selected.
            if (panelFilterCard != null && label1 != null && cbFilterBy != null)
            {
                if (isArabic)
                {
                    int rightMargin = 20;
                    label1.Left = panelFilterCard.ClientSize.Width - label1.Width - rightMargin;
                    cbFilterBy.Left = label1.Left - cbFilterBy.Width - 8;

                    if (txtFilterValue != null)
                        txtFilterValue.Left = cbFilterBy.Left - txtFilterValue.Width - 8;

                    label1.TextAlign = ContentAlignment.MiddleRight;
                }
                else
                {
                    label1.Left = 15;
                    cbFilterBy.Left = 87;
                    if (txtFilterValue != null)
                        txtFilterValue.Left = 287;
                    label1.TextAlign = ContentAlignment.MiddleLeft;
                }
            }

            // Move only the section heading to the opposite side in Arabic.
            if (panelGridHeader != null && lblGridTitle != null)
            {
                if (isArabic)
                {
                    lblGridTitle.Left = panelGridHeader.ClientSize.Width - lblGridTitle.Width - 15;
                    lblGridTitle.TextAlign = ContentAlignment.MiddleRight;
                }
                else
                {
                    lblGridTitle.Left = 15;
                    lblGridTitle.TextAlign = ContentAlignment.MiddleLeft;
                }
            }
        }

        private void TranslateComboItems(ComboBox comboBox)
        {
            if (comboBox == null)
                return;

            int selectedIndex = comboBox.SelectedIndex;
            var translatedItems = new List<string>();

            foreach (object item in comboBox.Items)
            {
                string itemText = item == null ? string.Empty : item.ToString();
                translatedItems.Add(string.IsNullOrEmpty(itemText) ? itemText : Tr(itemText));
            }

            comboBox.Items.Clear();
            foreach (string itemText in translatedItems)
                comboBox.Items.Add(itemText);

            if (selectedIndex >= 0 && selectedIndex < comboBox.Items.Count)
                comboBox.SelectedIndex = selectedIndex;
        }

        private void ApplyFooterMirroring()
        {
            if (btnClose == null || lblRecordsCount == null || btnClose.Parent == null)
                return;

            Control footer = btnClose.Parent;
            bool isArabic = TranslationClass.IsArabic;
            int margin = 25;
            int gap = 5;

            // Find the static record-count label without depending on a designer name.
            Label recordsLabel = footer.Controls
                .OfType<Label>()
                .FirstOrDefault(label => label != lblRecordsCount &&
                    (label.Text.Contains("Records") || label.Text.Contains("السجلات")));

            if (isArabic)
            {
                btnClose.Left = margin;
                if (recordsLabel != null)
                    recordsLabel.Left = footer.ClientSize.Width - recordsLabel.Width - margin;
                lblRecordsCount.Left = recordsLabel != null
                    ? recordsLabel.Left - lblRecordsCount.Width - gap
                    : footer.ClientSize.Width - lblRecordsCount.Width - margin;
            }
            else
            {
                btnClose.Left = footer.ClientSize.Width - btnClose.Width - margin;
                if (recordsLabel != null)
                    recordsLabel.Left = margin;
                lblRecordsCount.Left = recordsLabel != null
                    ? recordsLabel.Right + gap
                    : margin;
            }
        }

        private void ApplyTranslationToControls(Control.ControlCollection controls, bool isArabic)
        {
            foreach (Control control in controls)
            {
                // ComboBox items are handled separately so their visible text can be
                // translated while the existing filtering code remains functional.
                if (!(control is ComboBox) && !string.IsNullOrWhiteSpace(control.Text))
                    control.Text = TranslationClass.Translate(control.Text, isArabic);

                control.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;

                if (control.HasChildren)
                    ApplyTranslationToControls(control.Controls, isArabic);
            }
        }

        private void TranslateColumnHeader(string columnName, string englishText, bool isArabic)
        {
            if (dgvVisit.Columns.Contains(columnName))
                dgvVisit.Columns[columnName].HeaderText =
                    TranslationClass.Translate(englishText, isArabic);
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
            if (clsGlobal.CurrentUser.RoleID == 3)
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

            ApplyVisitFormTranslation();
            dgvVisit.CellFormatting -= dgvVisit_CellFormatting;
            dgvVisit.CellFormatting += dgvVisit_CellFormatting;
        }

        private void dgvVisit_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!TranslationClass.IsArabic || e.Value == null || e.Value == DBNull.Value)
                return;

            string columnName = dgvVisit.Columns[e.ColumnIndex].Name;
            string value = e.Value.ToString();

            if (columnName == "StatusText")
            {
                if (value == "Open") e.Value = Tr("Open");
                else if (value == "Completed") e.Value = Tr("Completed");
                else if (value == "Closed") e.Value = Tr("Closed");
                else if (value == "Cancelled") e.Value = Tr("Cancelled");
                e.FormattingApplied = true;
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFilter = cbFilterBy.Text;
            if (TranslationClass.IsArabic && selectedFilter == Tr("None"))
                selectedFilter = "None";

            txtFilterValue.Visible = (selectedFilter != "None");

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

            string selectedFilter = cbFilterBy.Text;
            if (TranslationClass.IsArabic)
            {
                if (selectedFilter == Tr("None")) selectedFilter = "None";
                else if (selectedFilter == Tr("Visit ID")) selectedFilter = "Visit ID";
                else if (selectedFilter == Tr("Appointment ID")) selectedFilter = "Appointment ID";
                else if (selectedFilter == Tr("Patient Name")) selectedFilter = "Patient Name";
                else if (selectedFilter == Tr("Doctor Name")) selectedFilter = "Doctor Name";
                else if (selectedFilter == Tr("Diagnosis")) selectedFilter = "Diagnosis";
                else if (selectedFilter == Tr("CreatedByUserID")) selectedFilter = "CreatedByUserID";
            }

            switch (selectedFilter)
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
                selectedFilter == "None")
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
            string selectedFilter = cbFilterBy.Text;
            if (TranslationClass.IsArabic)
            {
                if (selectedFilter == Tr("Appointment ID")) selectedFilter = "Appointment ID";
                else if (selectedFilter == Tr("Visit ID")) selectedFilter = "Visit ID";
                else if (selectedFilter == Tr("CreatedByUserID")) selectedFilter = "CreatedByUserID";
            }

            if (selectedFilter == "Appointment ID" || selectedFilter == "Visit ID" || selectedFilter == "CreatedByUserID")
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
            if (clsVisit.IsVisitClosed(VisitID))
            {
                MessageBox.Show("This visit is already closed and cannot be modified.",
                     "Operation Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmDoctor frm = new frmDoctor(VisitID, frmDoctor.enMode.Update);
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
            frmDoctor frm = new frmDoctor((int)dgvVisit.CurrentRow.Cells[0].Value, frmDoctor.enMode.ShowVisit);
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
