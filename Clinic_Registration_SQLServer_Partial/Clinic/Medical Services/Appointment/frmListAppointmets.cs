using Clinic.Doctor;
using Clinic.global_classes;
using Clinic.Medical_Services.Visit;
using Clinic.Patient;
using Clinic.Person;
using Clinic_Business;
using Clinic_Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Appointment
{
    public partial class frmListAppointmets : Form
    {
        private DataTable _dtAppointmets = clsAppointment.GetAllAppointments();
        private readonly Dictionary<Control, string> _originalControlTexts =
    new Dictionary<Control, string>();

        private readonly Dictionary<ToolStripItem, string> _originalMenuTexts =
            new Dictionary<ToolStripItem, string>();
        public frmListAppointmets()
        {
            InitializeComponent();

            // Add Check In to the Reception/Admin right-click menu.
            ToolStripMenuItem checkInMenu =
                new ToolStripMenuItem("Check In");

            checkInMenu.Name = "checkInReceptionMenu";

            checkInMenu.Click += checkINToolStripMenuItem_Click;

            cmsAppointmentRecption.Items.Add(checkInMenu);

            dgvAppointments.CellFormatting += dgvAppointments_CellFormatting;

            // Apply localization and mirroring before the form is displayed.
            ApplyLocalizationAndMirroring();
            panelFooterBar.Resize += panelFooterBar_Resize;
        }

        private string Tr(string text)
        {
            return TranslationClass.Translate(text, TranslationClass.IsArabic);
        }

        private string GetEnglishText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            string[] englishTexts =
            {
        "None",
        "Status",
        "Appointment ID",
        "Created By UserID",
        "Patient Name",
        "Doctor Name",
        "AppointmentType",
        "All",
        "Scheduled",
        "Cancelled",
        "Completed",
        "Postponed",
        "In Queue",
        "Check In",
        "View Visit",
        "Consultation",
        "Follow-up"
    };

            foreach (string english in englishTexts)
            {
                if (string.Equals(text, english, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(text, Tr(english), StringComparison.Ordinal))
                {
                    return english;
                }
            }

            return text;
        }
        private void ApplyAppointmentGridLanguage()
        {
            bool isArabic = TranslationClass.IsArabic;

            dgvAppointments.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            dgvAppointments.ColumnHeadersDefaultCellStyle.Alignment =
                isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvAppointments.DefaultCellStyle.Alignment =
                isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            SetAppointmentColumn("AppointmentID", "Appointment ID", 150);
            SetAppointmentColumn("PatientName", "Patient Name", 150);
            SetAppointmentColumn("DoctorName", "Doctor Name", 150);
            SetAppointmentColumn("TypeName", "AppointmentType", 150);
            SetAppointmentColumn("AppointmentFees", "Fees", 100);
            SetAppointmentColumn("AppointmentDate", "Date", 150);
            SetAppointmentColumn("AppointmentStatusText", "Status", 100);
            SetAppointmentColumn("CreatedByUserID", "C.UserID", 90);
        }

        private void SetAppointmentColumn(
            string columnName,
            string englishHeader,
            int width)
        {
            if (!dgvAppointments.Columns.Contains(columnName))
                return;

            DataGridViewColumn column = dgvAppointments.Columns[columnName];

            column.HeaderText = Tr(englishHeader);
            column.Width = width;
        }
        private void TranslateComboItems(ComboBox comboBox)
        {
            if (comboBox == null || comboBox.DataSource != null)
                return;

            int selectedIndex = comboBox.SelectedIndex;
            List<string> translatedItems = new List<string>();

            foreach (object item in comboBox.Items)
            {
                string itemText = item == null ? string.Empty : item.ToString();
                translatedItems.Add(string.IsNullOrEmpty(itemText)
                    ? itemText
                    : Tr(GetEnglishText(itemText)));
            }

            comboBox.Items.Clear();
            foreach (string itemText in translatedItems)
                comboBox.Items.Add(itemText);

            if (selectedIndex >= 0 && selectedIndex < comboBox.Items.Count)
                comboBox.SelectedIndex = selectedIndex;
        }

        private void TranslateToolStripItems(ToolStripItemCollection items)
        {
            bool isArabic = TranslationClass.IsArabic;

            foreach (ToolStripItem item in items)
            {
                if (!_originalMenuTexts.ContainsKey(item))
                    _originalMenuTexts[item] = item.Text;

                string englishText = _originalMenuTexts[item];

                if (!string.IsNullOrWhiteSpace(englishText))
                    item.Text = Tr(englishText.Trim());

                if (item is ToolStripDropDownItem dropDownItem)
                {
                    TranslateToolStripItems(dropDownItem.DropDownItems);
                }
            }
        }

        private void ApplyLocalizationAndMirroring()
        {
            bool isArabic = TranslationClass.IsArabic;

            this.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            this.RightToLeftLayout = isArabic;

            ApplyLocalizationToControls(this.Controls, isArabic);

            // Explicitly localize and mirror the footer Close button.
            btnClose.Text = isArabic ? Tr("Close") : "   Close";
            btnClose.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            btnClose.ImageAlign = isArabic
                ? System.Drawing.ContentAlignment.MiddleRight
                : System.Drawing.ContentAlignment.MiddleLeft;
            panelFooterBar.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            ApplyFooterMirroring();

            TranslateComboItems(cbFilterBy);
            TranslateComboItems(cbStatus);

            TranslateToolStripItems(cmsAppointmentRecption.Items);
            TranslateToolStripItems(cmsAppointmentCasher.Items);

            dgvAppointments.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            dgvAppointments.ColumnHeadersDefaultCellStyle.Alignment =
                isArabic ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft;
            dgvAppointments.DefaultCellStyle.Alignment =
                isArabic ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft;
        }

        private void panelFooterBar_Resize(object sender, EventArgs e)
        {
            ApplyFooterMirroring();
        }

        private void ApplyFooterMirroring()
        {
            if (panelFooterBar == null || btnClose == null ||
                label2 == null || lblRecordsCount == null)
                return;

            bool isArabic = TranslationClass.IsArabic;
            int margin = 25;
            int gap = 5;

            if (isArabic)
            {
                btnClose.Left = margin;
                label2.Left = panelFooterBar.ClientSize.Width - label2.Width - margin;
                lblRecordsCount.Left = label2.Left - lblRecordsCount.Width - gap;
            }
            else
            {
                btnClose.Left = panelFooterBar.ClientSize.Width - btnClose.Width - margin;
                label2.Left = margin;
                lblRecordsCount.Left = label2.Right + gap;
            }
        }

        private void ApplyLocalizationToControls(
            Control.ControlCollection controls,
            bool isArabic)
        {
            foreach (Control control in controls)
            {
                // Save original English text once.
                if (!_originalControlTexts.ContainsKey(control))
                    _originalControlTexts[control] = control.Text;

                string englishText = _originalControlTexts[control];

                if (!string.IsNullOrWhiteSpace(englishText))
                {
                    string lookupText = englishText
                        .Replace("\r", "")
                        .Replace("\n", " ")
                        .Trim();

                    control.Text = Tr(lookupText);
                }

                control.RightToLeft = isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control is Label label)
                {
                    // Preserve centered titles and record counts.
                    if (label.Name == "lblRecordsCount")
                        label.TextAlign = ContentAlignment.MiddleCenter;
                    else
                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }

                if (control.HasChildren)
                    ApplyLocalizationToControls(control.Controls, isArabic);
            }
        }
        private void dgvAppointments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!TranslationClass.IsArabic || e.Value == null || e.Value == DBNull.Value)
                return;

            string columnName = dgvAppointments.Columns[e.ColumnIndex].Name;
            if (columnName == "AppointmentStatusText" || columnName == "TypeName")
                e.Value = Tr(e.Value.ToString());
        }

        private void btnAddAppointment_Click(object sender, EventArgs e)
        {
            frmAddUpdateAppointment frm = new frmAddUpdateAppointment();
            frm.DatatBack += _DatatBackToAdd;
            frm.ShowDialog();
        }

        private void _RefreshAppointmentsList()
        {
            _dtAppointmets = clsAppointment.GetAllAppointments();
            MessageBox.Show(
    string.Join(", ",
        _dtAppointmets.Columns
            .Cast<DataColumn>()
            .Select(c => c.ColumnName)),
    "Appointment Columns");
            _dtAppointmets.PrimaryKey = new DataColumn[] { _dtAppointmets.Columns["AppointmentID"] };
            _dtAppointmets.Columns["AppointmentStatusText"].ReadOnly = false;
            lblRecordsCount.Text = _dtAppointmets.Rows.Count.ToString();
            dgvAppointments.DataSource = _dtAppointmets;
            _RefreshRecords();

        }
        private void frmListAppointmets_Load(object sender, EventArgs e)
        {
            if (clsGlobal.CurrentUser.RoleID == (int)UserRole.Cashier)
                dgvAppointments.ContextMenuStrip = cmsAppointmentCasher;
            else
                dgvAppointments.ContextMenuStrip = cmsAppointmentRecption;

            dgvAppointments.DataSource = _dtAppointmets;

            ApplyLocalizationAndMirroring();
            ApplyAppointmentGridLanguage();
            cbFilterBy.SelectedIndex = 0;
            if (_dtAppointmets.Rows.Count == 0)
                return;
            _dtAppointmets.PrimaryKey = new DataColumn[] { _dtAppointmets.Columns["AppointmentID"] };
            _dtAppointmets.Columns["AppointmentStatusText"].ReadOnly = false;
            lblRecordsCount.Text = _dtAppointmets.Rows.Count.ToString();

            dgvAppointments.Columns["AppointmentID"].HeaderText = Tr("Appointment ID");
            dgvAppointments.Columns["AppointmentID"].Width = 150;

            dgvAppointments.Columns["AppointmentTypeID"].HeaderText = Tr("AppointmentTypeID");
            dgvAppointments.Columns["AppointmentTypeID"].Width = 150;

            dgvAppointments.Columns["DoctorID"].HeaderText = Tr("DoctorID");
            dgvAppointments.Columns["DoctorID"].Width = 150;

            dgvAppointments.Columns["PatientID"].HeaderText = Tr("PatientID");
            dgvAppointments.Columns["PatientID"].Width = 150;

            dgvAppointments.Columns["AppointmentStatus"].HeaderText = Tr("AppointmentStatus");
            dgvAppointments.Columns["AppointmentStatus"].Width = 150;

            dgvAppointments.Columns["PatientName"].HeaderText = Tr("Patient Name");
            dgvAppointments.Columns["PatientName"].Width = 150;

            dgvAppointments.Columns["DoctorName"].HeaderText = Tr("Doctor Name");
            dgvAppointments.Columns["DoctorName"].Width = 150;

            dgvAppointments.Columns["TypeName"].HeaderText = Tr("AppointmentType");
            dgvAppointments.Columns["TypeName"].Width = 150;

            dgvAppointments.Columns["AppointmentFees"].HeaderText = Tr("Fees");
            dgvAppointments.Columns["AppointmentFees"].Width = 100;

            dgvAppointments.Columns["AppointmentDate"].HeaderText = Tr("Date");
            dgvAppointments.Columns["AppointmentDate"].Width = 150;

            dgvAppointments.Columns["AppointmentStatusText"].HeaderText = Tr("Status");
            dgvAppointments.Columns["AppointmentStatusText"].Width = 100;

            dgvAppointments.Columns["CreatedByUserID"].HeaderText = Tr("C.UserID");
            dgvAppointments.Columns["CreatedByUserID"].Width = 90;

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool IsNone = GetEnglishText(cbFilterBy.Text) == "None";
            bool IsStatus = GetEnglishText(cbFilterBy.Text) == "Status";

            txtFilterValue.Visible = !IsNone && !IsStatus;
            cbStatus.Visible = IsStatus;

            _dtAppointmets.DefaultView.RowFilter = string.Empty;
            lblRecordsCount.Text = _dtAppointmets.Rows.Count.ToString();
            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = string.Empty;
                txtFilterValue.Focus();
            }
            else if (cbStatus.Visible)
            {
                cbStatus.SelectedIndex = 0;
                cbStatus.Focus();
            }
            else
            {
                cbFilterBy.Focus();
            }
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (GetEnglishText(cbFilterBy.Text) == "Appointment ID" || GetEnglishText(cbFilterBy.Text) == "Created By UserID")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

            }
        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus.SelectedIndex == 0)
            {
                _dtAppointmets.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                _dtAppointmets.DefaultView.RowFilter = $"AppointmentStatusText='{GetEnglishText(cbStatus.Text)}'";
            }
            lblRecordsCount.Text = _dtAppointmets.Rows.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (_dtAppointmets == null)
                return;

            string filterColumn = _GetFilterColumnName(GetEnglishText(cbFilterBy.Text));
            string searchText = txtFilterValue.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText) ||
                filterColumn == "None")
            {
                _dtAppointmets.DefaultView.RowFilter = string.Empty;
                lblRecordsCount.Text =
                    _dtAppointmets.DefaultView.Count.ToString();
                return;
            }

            // Check whether the requested column exists.
            if (!_dtAppointmets.Columns.Contains(filterColumn))
            {
                _dtAppointmets.DefaultView.RowFilter = string.Empty;

                System.Diagnostics.Debug.WriteLine(
                    "Missing appointment filter column: " + filterColumn);

                return;
            }

            if (filterColumn == "AppointmentID" ||
                filterColumn == "CreatedByUserID")
            {
                int number;

                if (!int.TryParse(searchText, out number))
                {
                    _dtAppointmets.DefaultView.RowFilter = "1 = 0";
                }
                else
                {
                    _dtAppointmets.DefaultView.RowFilter =
                        $"[{filterColumn}] = {number}";
                }
            }
            else
            {
                string safeText = searchText
                    .Replace("'", "''")
                    .Replace("[", "[[]")
                    .Replace("%", "[%]")
                    .Replace("*", "[*]");

                _dtAppointmets.DefaultView.RowFilter =
                    $"Convert([{filterColumn}], 'System.String') LIKE '{safeText}%'";
            }

            lblRecordsCount.Text =
                _dtAppointmets.DefaultView.Count.ToString();
        }

        private string _GetFilterColumnName(string FilterBy)
        {
            switch (FilterBy)
            {
                case "Appointment ID": return "AppointmentID";
                case "Patient Name": return "PatientName";
                case "Doctor Name": return "DoctorName";
                case "AppointmentType": return "TypeName";
                case "Created By UserID": return "CreatedByUserID";
                default: return "None";
            }
        }
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int AppointmentID = (int)dgvAppointments.CurrentRow.Cells["AppointmentID"].Value;

            frmAddUpdateAppointment frm = new frmAddUpdateAppointment(AppointmentID);
            frm.DatatBack += _DataBackToUpdate;
            frm.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvAppointments.CurrentRow == null) return;

            int AppointmentID = (int)dgvAppointments.CurrentRow.Cells["AppointmentID"].Value;

            if (MessageBox.Show(Tr("Are you sure you want to delete Appoinment [") + AppointmentID + "]?", Tr("Confirm Delete"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (clsAppointment.DeleteAppointment(AppointmentID))
                {
                    MessageBox.Show(Tr("Appoinment Deleted Successfully."), Tr("Successful"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _DataBackToDelete(AppointmentID);
                }
                else
                {
                    MessageBox.Show(Tr("Appoinment was not deleted because it has data linked to it in the system."), Tr("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _RefreshRecords()
        {
            lblRecordsCount.Text = _dtAppointmets.Rows.Count.ToString();
        }
        private void _DatatBackToAdd(object sender, int AppointmentID)
        {
            DataRow NewAppointmentRow = clsAppointment.GetAppointmentInfoByID(AppointmentID);

            if (NewAppointmentRow != null)
            {
                _dtAppointmets.UpsertRow(NewAppointmentRow, AppointmentID);
                _RefreshRecords();
            }


        }

        private void _DataBackToUpdate(object sender, int AppointmentID)
        {
            DataRow UpdateAppointmentRow = clsAppointment.GetAppointmentInfoByID(AppointmentID);

            if (UpdateAppointmentRow != null)
            {
                _dtAppointmets.UpsertRow(UpdateAppointmentRow, AppointmentID);
                _RefreshRecords();
            }
        }

        private void _DataBackToDelete(int AppointmentID)
        {
            DataRow row = _dtAppointmets.Rows.Find(AppointmentID);
            if (row != null)
            {
                _dtAppointmets.Rows.Remove(row);
                _dtAppointmets.AcceptChanges();
                _RefreshRecords();
            }
        }

        public string GetAppointmentStatusText(enAppointmentStatus AppointmentStatus)
        {
                switch ((enAppointmentStatus)AppointmentStatus)
                {
                    case enAppointmentStatus.Scheduled:
                        return "Scheduled";

                    case enAppointmentStatus.InQueue:
                        return "In Queue";

                    case enAppointmentStatus.Progress:
                        return "In Progress";

                    case enAppointmentStatus.Postponed:
                        return "Postponed";

                    case enAppointmentStatus.Completed:
                        return "Completed";

                    case enAppointmentStatus.Cancelled:
                        return "Cancelled";

                    case enAppointmentStatus.Waiting_For_Vitals:
                        return "Waiting for Vitals";

                    case enAppointmentStatus.Ready_For_Doctor:
                        return "Ready for Doctor";

                    case enAppointmentStatus.In_Billing:
                        return "In Billing";

                    default:
                        return "Unknown";
                }
            
        }
        private void _ProcessAppointmentStatus(enAppointmentStatus newStatus, string actionName)
        {
            if (dgvAppointments.CurrentRow == null) return;

            int currentAppointmentID = (int)dgvAppointments.CurrentRow.Cells[0].Value;

            string message = Tr("Are you sure you want to ") + Tr(actionName) + "?";
            string title = Tr("Confirm Action");



            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsAppointment.UpdateAppointmentStatus(currentAppointmentID, GetAppointmentStatusText(newStatus), clsGlobal.CurrentUser.UserID))
                {
                    if (newStatus == enAppointmentStatus.InQueue)
                    {
                        if (dgvAppointments.CurrentRow == null) return;
                        int appointmentID = (int)dgvAppointments.CurrentRow.Cells["AppointmentID"].Value;
                        int doctorID = (int)clsAppointment.Find(appointmentID).DoctorID;
                        clsAppointment.RefreshQueueForDoctor(doctorID);

                    }
                    MessageBox.Show($"✅ The operation was completed successfully.", Tr("Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _DataBackToUpdate(this, currentAppointmentID);
                }

                else
                {
                    MessageBox.Show($"❌ An error occurred while updating the status.", Tr("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
            {
                checkINToolStripMenuItem.Enabled = false;
                return;
            }

            string currentStatus = Convert.ToString(dgvAppointments.CurrentRow.Cells["AppointmentStatusText"].Value);

            checkINToolStripMenuItem.Enabled = string.Equals(currentStatus, "Scheduled", StringComparison.OrdinalIgnoreCase);
            //if (dgvAppointments.CurrentRow == null) return;
            //string currentStatus = dgvAppointments.CurrentRow.Cells["AppointmentStatusText"].Value.ToString();
            //checkINToolStripMenuItem.Enabled = (currentStatus == "Scheduled");
            rescheduleToolStripMenuItem.Enabled = (currentStatus == "Scheduled" || currentStatus == "Postponed");

            deleteToolStripMenuItem.Enabled = (currentStatus != "Completed");
            editToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled && (currentStatus != "Cancelled");
        }

        private void showPatientDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int AppointmentID = (int)dgvAppointments.CurrentRow.Cells["AppointmentID"].Value;

            int PatientID = clsAppointment.Find(AppointmentID).PatientID;
            frmPatientInfo frm = new frmPatientInfo(PatientID);
            frm.ShowDialog();
        }

        private void showDoctorDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int AppointmentID = (int)dgvAppointments.CurrentRow.Cells["AppointmentID"].Value;

            int DoctorID = clsAppointment.Find(AppointmentID).DoctorID;
            frmDoctorInfo frm = new frmDoctorInfo(DoctorID);
            frm.ShowDialog();
        }

        private void rescheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int AppointmentID = (int)dgvAppointments.CurrentRow.Cells["AppointmentID"].Value;

            clsAppointment appointment = clsAppointment.Find(AppointmentID);

            frmReschedule frm = new frmReschedule(AppointmentID, appointment.DoctorID, appointment.PatientID);
            frm.ShowDialog();
            _RefreshAppointmentsList();
        }

        private void checkINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
                return;

            int appointmentID = Convert.ToInt32(
                dgvAppointments.CurrentRow.Cells["AppointmentID"].Value);

            decimal appointmentFees = Convert.ToDecimal(
                dgvAppointments.CurrentRow.Cells["AppointmentFees"].Value);

            string patientName = Convert.ToString(
                dgvAppointments.CurrentRow.Cells["PatientName"].Value);

            // 1. Find appointment
            clsAppointment appointment = clsAppointment.Find(appointmentID);

            if (appointment == null)
            {
                MessageBox.Show(
                    Tr("Appointment not found."),
                    Tr("Check-in Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // 2. Prevent duplicate check-in
            if (appointment.AppointmentStatus != (byte)enAppointmentStatus.Scheduled)
            {
                MessageBox.Show(
                    Tr("This appointment has already been checked in or is no longer scheduled."),
                    Tr("Check-in Not Allowed"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // 3. Open payment dialog
            using (frmCheckInDialog frm =
                new frmCheckInDialog(
                    appointmentID,
                    appointmentFees,
                    patientName))
            {
                if (frm.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    string paymentMethod = frm.SelectedPaymentMethod;
                    int userID = clsGlobal.CurrentUser.UserID;
                    // "Cash" = Default Account For Cash,"Card (POS)" = Default Account For Bank,"Bank Transfer" =Default Account For Bank

                    int debitAccountId = BindCombos.SelectDefaultLevelAccount(paymentMethod == "Cash" ? "Default Account For Cash" : "Default Account For Bank");  // Cash or card clearing account
                    int creditAccountId = BindCombos.SelectDefaultLevelAccount("Sales"); // here category name sales is accountid of Consultation Fee Revenue

                    // 4. Perform check-in
                    bool success = clsBillingService.CheckInPatient(
                                    appointment,
                                    userID,
                                    paymentMethod,
                                    debitAccountId,
                                    creditAccountId);

                    if (!success)
                    {
                        MessageBox.Show(
                            Tr("The patient could not be checked in. The appointment may already have been checked in."),
                            Tr("Check-in Failed"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    // 5. Refresh doctor's queue
                    clsAppointment.RefreshQueueForDoctor(
                        appointment.DoctorID);

                    // 6. Notify user
                    MessageBox.Show(
                        Tr("Patient checked in successfully."),
                        Tr("Success"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    _DataBackToUpdate(this, appointmentID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        Tr("Check-in Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        private void AddViewVisitMenu()
        {
            ToolStripMenuItem viewVisitMenu =
                new ToolStripMenuItem("View Visit");

            viewVisitMenu.Name = "viewVisitToolStripMenuItem";

            viewVisitMenu.Click += ViewVisitToolStripMenuItem_Click;

            // Add to both existing right-click menus.
            cmsAppointmentRecption.Items.Add(viewVisitMenu);

            ToolStripMenuItem cashierViewVisitMenu =
                new ToolStripMenuItem("View Visit");

            cashierViewVisitMenu.Click += ViewVisitToolStripMenuItem_Click;

            cmsAppointmentCasher.Items.Add(cashierViewVisitMenu);
        }

        private void ViewVisitToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
                return;

            int appointmentID = Convert.ToInt32(
                dgvAppointments.CurrentRow.Cells["AppointmentID"].Value);

            DataTable visits = clsVisit.GetAllVisits();

            DataRow[] matchingVisits = visits.Select(
                "AppointmentID = " + appointmentID);

            if (matchingVisits.Length == 0)
            {
                MessageBox.Show(
                    Tr("No visit found for Appointment ID: ") + appointmentID +
                    Tr("\n\nPlease check in the patient first."),
                    Tr("Visit Not Found"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            int visitID = Convert.ToInt32(
                matchingVisits[0]["VisitID"]);

            // Open the existing Visits page.
            // The selected visit is identified above.
            frmListVisits frm = new frmListVisits();
            frm.ShowDialog();
        }
        private void CancelToolStripMenuItem_Click(object sender, EventArgs e)
                    => _ProcessAppointmentStatus(enAppointmentStatus.Cancelled, "cancel this appointment");

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VisitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListPatient frm = new frmListPatient();
            frm.ShowDialog();
        }
    }
}
