using Clinic.Controls;
using Clinic.ControlsMain;
using Clinic.Doctor;
using Clinic.Patient;
using Clinic.Serveces.ServecesType;
using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Visit
{
    public partial class frmDoctor : Form
    {
        #region Private Members

        private enMode _Mode;
        private int _VisitID = -1;
        private int _AppointmentID = -1;

        private clsVisit _Visit = new clsVisit();

        private int _DoctorPersonID =
            clsGlobal.CurrentUser.RoleID == 1
                ? 0
                : clsGlobal.CurrentUser.PersonID;

        #endregion

        public enum enMode { AddNew = 1, Update = 2, ShowVisit = 3 }

        #region Constructor
        public frmDoctor(int visitID = -1, enMode mode = enMode.AddNew)
        {
            InitializeComponent();

            _VisitID = visitID;
            _Mode = mode;

            ApplyModernDoctorDesign();
            ApplyDoctorArabicMirroring();

        }

        private void ApplyDoctorArabicMirroring()
        {
            bool isArabic = TranslationClass.IsArabic;

            // IMPORTANT:
            // Do not mirror the complete form.
            // Keep the original English layout coordinates.
            this.RightToLeft = RightToLeft.No;
            this.RightToLeftLayout = false;

            // Apply Arabic text direction to text-based controls only.
            ApplyArabicTextDirection(this, isArabic);
        }

        private void ApplyArabicTextDirection(Control parent, bool isArabic)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (control is Label ||
                    control is LinkLabel ||
                    control is GroupBox ||
                    control is TabPage ||
                    control is Button)
                {
                    control.RightToLeft = isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;
                }

                if (control is TextBoxBase textBox)
                {
                    textBox.RightToLeft = isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;
                }

                if (control is DataGridView grid)
                {
                    grid.RightToLeft = isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                    // Keep grid columns inside the available width.
                    grid.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;

                    grid.ColumnHeadersDefaultCellStyle.WrapMode =
                        DataGridViewTriState.True;

                    grid.AutoSizeRowsMode =
                        DataGridViewAutoSizeRowsMode.None;
                }

                // Do not change Dock, Size, Location, Anchor or WindowState.
                if (control.HasChildren)
                    ApplyArabicTextDirection(control, isArabic);
            }
        }
        #endregion

        #region UI Feedback (Professional Messaging)
        private void ShowSuccess(string message) => MessageBox.Show(message, "✅ Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowError(string message) => MessageBox.Show(message, "❌ Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowWarning(string message) => MessageBox.Show(message, "⚠️ Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private DialogResult ConfirmAction(string message) => MessageBox.Show(message, "❓ Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        #endregion

        #region Refactored Helper Functions

        private void RefreshGrid(DataGridView dgv, object dataSource)
        {
            dgv.DataSource = null;
            dgv.DataSource = dataSource;
        }

        private void DeleteRowFromGrid(DataGridView dgv, DataTable dt, DataGridViewCellEventArgs e, string msg)
        {
            if (dt == null || e.RowIndex < 0 || e.ColumnIndex < 0 ||
                e.RowIndex >= dgv.Rows.Count || e.ColumnIndex >= dgv.Columns.Count ||
                dgv.Columns[e.ColumnIndex].Name != "Delete") return;

            DataRowView view = dgv.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (view == null) return;
            if (ConfirmAction(msg) != DialogResult.Yes) return;
            view.Row.Delete();
            RefreshGrid(dgv, dt);
        }

        private void SetFormControlsState(bool isEnabled)
        {
            pnlVisitInfo.Enabled = isEnabled;
            pnlServicesInfo.Enabled = isEnabled;
            pnlPrescriptionInfo.Enabled = isEnabled;
            btnFullSave.Enabled = isEnabled;
            tpServices.Enabled = isEnabled;
            tpPrescriptionInfo.Enabled = isEnabled;
        }

        #endregion
        #region Initialization
        private void frmDoctor_Load(object sender, EventArgs e)
        {

            if (!CheckAuth()) return;

            _SetupUI();
            pnlWaittingList.BackColor = Color.LightYellow;
            dgvDoctorQueue.BackColor = Color.White;
            dgvDoctorQueue.Visible = true;
            dgvDoctorQueue.BringToFront();
            _FillServicesInCheckBox();
            timer2.Start();

            if (_Mode == enMode.AddNew)
                _LoadAddNewMode();
            else if (_Mode == enMode.Update)
                _LoadUpdateMode();
            else
                _LoadShowMode();

            // Apply translations after mode-specific labels and grid columns are initialized.
            ApplyDoctorTranslations();


        }




        private void TranslateDoctorControls(Control parent, System.Collections.Generic.Dictionary<string, string> translations)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (!string.IsNullOrWhiteSpace(control.Text))
                {
                    string translated;
                    string originalText = control.Text.Trim();

                    // The queue's record label may include a leading colon/spacing.
                    if (originalText.IndexOf("Records #", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        control.Text = "عدد السجلات:";
                    }
                    // First try the complete caption, including any arrow.
                    else if (translations.TryGetValue(originalText, out translated))
                    {
                        control.Text = translated;
                    }
                    else
                    {
                        // Also support captions whose arrow differs from the dictionary key.
                        string cleanedText = originalText
                            .Replace("→", "")
                            .Replace("➜", "")
                            .Replace("➔", "")
                            .Replace("←", "")
                            .Trim();

                        if (translations.TryGetValue(cleanedText, out translated))
                        {
                            // Keep the arrow direction appropriate for Arabic layout.
                            bool hasLeadingArrow = originalText.StartsWith("→") ||
                                                    originalText.StartsWith("➜") ||
                                                    originalText.StartsWith("➔");
                            control.Text = hasLeadingArrow
                                ? translated + " ←"
                                : translated;
                        }
                        else if (string.Equals(cleanedText, "View Doctor Details", StringComparison.OrdinalIgnoreCase))
                        {
                            control.Text = "عرض بيانات الطبيب";
                        }
                        else if (string.Equals(cleanedText, "View Patient Details", StringComparison.OrdinalIgnoreCase))
                        {
                            control.Text = "عرض بيانات المريض";
                        }
                        else if (string.Equals(cleanedText, "Next", StringComparison.OrdinalIgnoreCase))
                        {
                            control.Text = "التالي";
                        }
                    }
                }

                if (control.HasChildren)
                    TranslateDoctorControls(control, translations);
            }
        }

        private void TranslateDoctorGrid(DataGridView grid, System.Collections.Generic.Dictionary<string, string> translations)
        {
            if (grid == null)
                return;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                string translated;
                if (translations.TryGetValue(column.HeaderText ?? string.Empty, out translated))
                    column.HeaderText = translated;
            }

            bool isArabic = TranslationClass.IsArabic;
            grid.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.DefaultCellStyle.Alignment = isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private void ApplyDoctorTranslations()
        {
            bool isArabic = TranslationClass.IsArabic;
            ApplyDoctorArabicMirroring();

            if (!isArabic)
                return;

            // Use a form-local copy so translations here do not change other forms.
            var translations = new System.Collections.Generic.Dictionary<string, string>(
                TranslationClass.sidebarTranslations);

            translations["Add Medicine +"] = "إضافة دواء +";
            translations["Add Medicen +"] = "إضافة دواء +";
            translations["Add Service +"] = "إضافة خدمة +";
            translations["Add Service"] = "إضافة خدمة";
            translations["New +"] = "جديد +";
            translations["New"] = "جديد";
            translations["Service"] = "الخدمة";
            translations["Blood Test"] = "فحص الدم";
            translations["Prescription Date"] = "تاريخ الوصفة الطبية";
            translations["Prescription Notes"] = "ملاحظات الوصفة الطبية";
            translations["Records #"] = "عدد السجلات";
            translations[":Records #"] = "عدد السجلات:";
            translations[":Records # "] = "عدد السجلات:";
            translations["Records #:"] = "عدد السجلات:";

            TranslateDoctorControls(this, translations);
            TranslateDoctorGrid(dgvDoctorQueue, translations);
            TranslateDoctorGrid(dgvServices, translations);
            TranslateDoctorGrid(dgvMedicines, translations);

            // Explicitly translate only the controls marked in the screenshot.
            btnAddService.Text = "إضافة خدمة +";
            btnNewService.Text = "جديد +";
            lbServeces.Text = "الخدمة";
            btnAddMedicen.Text = "إضافة دواء +";

            // Translate the dropdown display only; keep ServiceName unchanged for saving.
            DataTable serviceTable = cmbServices.DataSource as DataTable;
            if (serviceTable != null)
            {
                const string arabicDisplayColumn = "ServiceNameArabic";
                if (!serviceTable.Columns.Contains(arabicDisplayColumn))
                    serviceTable.Columns.Add(arabicDisplayColumn, typeof(string));

                foreach (DataRow row in serviceTable.Rows)
                {
                    string serviceName = Convert.ToString(row["ServiceName"]);
                    string translatedName;
                    row[arabicDisplayColumn] =
                        translations.TryGetValue(serviceName == null ? string.Empty : serviceName.Trim(), out translatedName)
                            ? translatedName
                            : serviceName;
                }

                cmbServices.DisplayMember = arabicDisplayColumn;
            }

            // DateTimePicker uses the Windows culture for month/day names.
            // Use a culture-neutral numeric date in Arabic mode to avoid English words.
            dtpPrescriptionDate.Format = DateTimePickerFormat.Custom;
            dtpPrescriptionDate.CustomFormat = "dd/MM/yyyy";

            foreach (DataGridView grid in new[] { dgvDoctorQueue, dgvServices, dgvMedicines })
            {
                if (grid == null) continue;
                grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }

        private bool CheckAuth()
        {
            if (clsGlobal.CurrentUser.RoleID == 1 || clsGlobal.CurrentUser.RoleID == 2) return true;
            ShowError("Access Denied! You do not have permission to access this area.");
            this.Close();
            return false;
        }

        private void _SetupUI()
        {
            // Initialize Grid structures once
            _SetupServicesGrid();
            _SetupMedicinesGrid();
        }

        private void _LoadAddNewMode()
        {
            lblTitle.Text = "Add New Visit";
            pnlWaittingList.Visible = true;
            timer1.Start();
            tpServices.Enabled = false;
            tpPrescriptionInfo.Enabled = false;
            dtpPrescriptionDate.Value = DateTime.Now;
            nudQuality.Value = 1;
            btnFullSave.Enabled = false;
            _SetupWaitingListGrid();
            _RefreshQueueData();

            if (dgvDoctorQueue.Rows.Count > 0)
            {
                _CallNextPatientInQueue();
            }
            else
            {
                llPatientInfo.Enabled = false;
                llDoctorInfo.Enabled = false;
            }

        }

        private void _LoadUpdateMode()
        {
            lblTitle.Text = "Update Visit Info";

            if (_VisitID <= 0)
            {
                ShowError("Invalid Visit ID.");
                Close();
                return;
            }

            _LoadVisitData(_VisitID);

            if (_Visit == null)
                return;

            tcVisitInfo.Dock = DockStyle.Fill;
            pnlWaittingList.Visible = false;

            pnlVisitInfo.Enabled = true;
            pnlServicesInfo.Enabled = true;
            tpServices.Enabled = true;
            tpPrescriptionInfo.Enabled = true;

            bool canEditPrescription =
                _Visit.PrescriptionInfo != null &&
                _Visit.PrescriptionInfo.IsPrescriptionPending();

            pnlPrescriptionInfo.Enabled = canEditPrescription;
            btnAddMedicen.Enabled = canEditPrescription;

            btnFullSave.Enabled = true;
            btnFullSave.Visible = true;

            if (!canEditPrescription)
            {
                MessageBox.Show(
                    "This prescription has already been sent to the pharmacy " +
                    "or completed.\n\n" +
                    "You can view it, but medicine changes require an authorized amendment.",
                    "Prescription Read-Only",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        private void _LoadShowMode()
        {
            lblTitle.Text = "Visit Details";
            _LoadVisitData(_VisitID);
            tcVisitInfo.Dock = DockStyle.Fill;

            pnlVisitInfo.Enabled = false;
            btnSaveVisit.Visible = false;

            pnlServicesInfo.Enabled = false;
            btnSaveVisitServices.Visible = false;
            btnAddService.Visible = false;
            lbServeces.Visible = false;
            cmbServices.Visible = false;
            lbQuality.Visible = false;
            nudQuality.Visible = false;

            pnlPrescriptionInfo.Enabled = false;
            btnAddMedicen.Visible = false;
            lbPrescriptionDate.Visible = false;
            dtpPrescriptionDate.Visible = false;
            pbPrecriptionDate.Visible = false;
            btnFullSave.Visible = false;
        }
        #endregion

        #region Data Loading
        private void _LoadVisitData(int visitID)
        {
            _Visit = clsVisit.Find(visitID);
            Debug.WriteLine("========== PRESCRIPTION LOAD ==========");
            Debug.WriteLine("Visit ID: " + visitID);

            if (_Visit.PrescriptionInfo == null)
            {
                Debug.WriteLine("PrescriptionInfo is NULL");
            }
            else
            {
                Debug.WriteLine(
                    "Prescription ID: " +
                    _Visit.PrescriptionInfo.PrescriptionID);

                Debug.WriteLine(
                    "Prescription Date: " +
                    _Visit.PrescriptionInfo.PrescriptionDate);

                Debug.WriteLine(
                    "Medicine Count: " +
                    (_Visit.PrescriptionInfo.dtMedicines?.Rows.Count ?? 0));

                if (_Visit.PrescriptionInfo.dtMedicines != null)
                {
                    foreach (DataRow row in _Visit.PrescriptionInfo.dtMedicines.Rows)
                    {
                        Debug.WriteLine(
                            "Medicine: " + row["SavedMedicineName"] +
                            " | Quantity: " + row["Quantity"]);
                    }
                }
            }

            Debug.WriteLine("=======================================");
            if (_Visit == null)
            {
                ShowError($"Visit with ID {visitID} could not be found.");
                Close();
                return;
            }

            _VisitID = visitID;
            _AppointmentID = _Visit.AppointmentID;

            lbVisitID.Text = _VisitID.ToString();

            txtDiagnosis.Text = _Visit.Diagnosis ?? "";
            txtNotes.Text = _Visit.VisitNotes ?? "";

            pnlAction.Visible = false;

            _FillPatientInfo();
            _FillVitalsInfo();

            // Load services
            dgvServices.DataSource = null;
            dgvServices.DataSource = _Visit.dtServices;

            // Load prescription associated with this visit.
            if (_Visit.PrescriptionInfo == null)
            {
                _Visit.PrescriptionInfo = clsPrescription.Find(visitID);
            }

            if (_Visit.PrescriptionInfo == null)
            {
                _Visit.PrescriptionInfo = new clsPrescription();
                _Visit.PrescriptionInfo.VisitID = visitID;
                _Visit.PrescriptionInfo.InitializePrescriptionsTable();
            }

            if (_Visit.PrescriptionInfo.dtMedicines == null ||
                _Visit.PrescriptionInfo.dtMedicines.Columns.Count == 0)
            {
                _Visit.PrescriptionInfo.InitializePrescriptionsTable();
            }

            bool canEditPrescription =
                _Visit.PrescriptionInfo.PrescriptionID <= 0 ||
                _Visit.PrescriptionInfo.IsPrescriptionPending();

            pnlPrescriptionInfo.Enabled = canEditPrescription;
            btnAddMedicen.Enabled = canEditPrescription;

            lbPrescriptionID.Text =
                _Visit.PrescriptionInfo.PrescriptionID > 0
                ? _Visit.PrescriptionInfo.PrescriptionID.ToString()
                : "New";

            dtpPrescriptionDate.Value =
                _Visit.PrescriptionInfo.PrescriptionDate;

            txtPrescriptionNotes.Text =
                _Visit.PrescriptionInfo.PrescriptionNotes ?? "";

            dgvMedicines.DataSource = null;
            dgvMedicines.AutoGenerateColumns = true;
            dgvMedicines.DataSource = _Visit.PrescriptionInfo.dtMedicines;

            Debug.WriteLine(
                "Prescription ID: " +
                _Visit.PrescriptionInfo.PrescriptionID);

            Debug.WriteLine(
                "Medicine rows loaded: " +
                _Visit.PrescriptionInfo.dtMedicines.Rows.Count);

            tpPrescriptionInfo.Enabled = true;
            tpServices.Enabled = true;
            pnlActions.Enabled = true;
            pnlWaittingList.Visible = false;
        }
        private void _FillVitalsInfo()
        {
            clsVital vital = clsVital.FindByVisitID(_VisitID);
            if (vital == null)
            {
                if (_Mode == enMode.AddNew)
                {
                    clsAppointment.UpdateAppointmentStatus(
                        _AppointmentID,
                        clsAppointment.enAppointmentStatus.Waiting_For_Vitals,
                        clsGlobal.CurrentUser.UserID);

                    _RefreshQueueData();
                }

                return;
            }
            lbPluse.Text = vital.Pulse.ToString() + "  bpm";
            lblTemp.Text = vital.Temperature.ToString() + " °C";
            lbBP.Text = vital.BloodPressure.ToString() + " mmHg";
            lbWeight.Text = vital.Weight.ToString() + " kg";
        }

        private void _FillPatientInfo()
        {
            lblVisitID.Text = _Visit.VisitID.ToString();
            lblAppointmentID.Text = _Visit.AppointmentID.ToString();
            lblDoctorID.Text = _Visit.DoctorID.ToString();
            lblPatientID.Text = _Visit.PatientID.ToString();

            if (_Visit.VisitID > 0)
            {
                llDoctorInfo.Enabled = true;
                llPatientInfo.Enabled = true;
            }
        }

        private void _FillServicesInCheckBox()
        {
            DataTable dtServices = new DataTable();

            string query = @"
        SELECT
            id AS ServiceID,
            name AS ServiceName,
            COALESCE(sales_price, 0) AS ServiceFees
        FROM tbl_items
        WHERE TRIM(type) IN ('12', '12 - Service')
          AND state = 0
AND active = 0        ORDER BY name;";

            using (MySql.Data.MySqlClient.MySqlConnection connection =
                Clinic_DataAccess.ClinicDatabase.GetConnection())
            {
                using (MySql.Data.MySqlClient.MySqlDataAdapter adapter =
                    new MySql.Data.MySqlClient.MySqlDataAdapter(query, connection))
                {
                    adapter.Fill(dtServices);
                }
            }

            cmbServices.DataSource = null;
            cmbServices.DisplayMember = "ServiceName";
            cmbServices.ValueMember = "ServiceID";
            cmbServices.DataSource = dtServices;
        }
        private string FindServiceColumn(DataTable table, params string[] names)
        {
            foreach (string name in names)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (string.Equals(
                        column.ColumnName,
                        name,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return column.ColumnName;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Saving Logic

        private void btnSaveVisit_Click(object sender, EventArgs e)
        {
            tcVisitInfo.SelectedIndex = 1;
        }

        private void btnSaveVisitServices_Click(object sender, EventArgs e)
        {
            tcVisitInfo.SelectedIndex = 2;

        }

        private void btnAddService_Click(object sender, EventArgs e)
        {
            if (cmbServices.SelectedValue == null) return;

            int serviceID = (int)cmbServices.SelectedValue;

            decimal discount = 0;
            decimal.TryParse(txtDiscount.Text, out discount);

            // التحقق من الصلاحية: (استخدم ! لعكس النتيجة)
            if (!clsDiscount.ValidateDiscount(clsGlobal.CurrentUser.RoleID, clsDiscount.enTargetType.MedicalService, discount))
            {
                MessageBox.Show("Discount exceeds your allowed limit or is invalid for this role.", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // يجب الخروج وعدم إكمال الحفظ
            }

            // 1. منع التكرار
            DataRow[] existingRows = _Visit.dtServices.Select($"ServiceID = {serviceID}");
            if (existingRows.Length > 0)
            {
                ShowWarning("This service has already been added to the visit.");
                return;
            }
            // 2. الحصول على سعر الخدمة من الـ DataSource الخاص بالـ ComboBox (لا حاجة لقاعدة البيانات)
            DataTable dtSource = (DataTable)cmbServices.DataSource;
            DataRow[] foundRows = dtSource.Select($"ServiceID = {serviceID}");

            if (foundRows.Length > 0)
            {
                DataRow serviceRow = foundRows[0];

                // 3. إضافة الصف الجديد
                DataRow newRow = _Visit.dtServices.NewRow();
                newRow["ServiceID"] = serviceID;
                newRow["ServiceName"] = serviceRow["ServiceName"];
                newRow["SavedServicePrice"] = serviceRow["ServiceFees"];
                newRow["Quantity"] = nudQuality.Value;
                newRow["Discount"] = discount;

                _Visit.dtServices.Rows.Add(newRow);
                RefreshGrid(dgvServices, _Visit.dtServices);

            }
        }

        private void btnAddMedicen_Click(object sender, EventArgs e)
        {
            if (_Visit == null)
            {
                ShowWarning("Please select a visit first.");
                return;
            }

            if (_Visit.PrescriptionInfo == null)
            {
                _Visit.PrescriptionInfo = new clsPrescription();
                _Visit.PrescriptionInfo.VisitID = _VisitID;
            }

            if (_Visit.PrescriptionInfo.PrescriptionID > 0 &&
                !_Visit.PrescriptionInfo.IsPrescriptionPending())
            {
                ShowWarning(
                    "This prescription has already been processed. " +
                    "You cannot add medicines to it.");
                return;
            }

            if (_Visit.PrescriptionInfo.dtMedicines == null ||
                _Visit.PrescriptionInfo.dtMedicines.Columns.Count == 0)
            {
                _Visit.PrescriptionInfo.InitializePrescriptionsTable();
            }

            DataTable dtMedicines = _Visit.PrescriptionInfo.dtMedicines;

            using (frmAddUpdateMedicineToPrescription frm =
                new frmAddUpdateMedicineToPrescription(ref dtMedicines))
            {
                frm.DataBack += (senderForm) =>
                {
                    _Visit.PrescriptionInfo.dtMedicines = dtMedicines;

                    RefreshGrid(
                        dgvMedicines,
                        _Visit.PrescriptionInfo.dtMedicines);
                };

                frm.ShowDialog(this);
            }
        }

        private void btnFullSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                ShowWarning("Please correct the input errors before saving.");
                return;
            }

            int patientId, doctorId, appointmentId;

            if (_Visit == null ||
                !int.TryParse(lblPatientID.Text, out patientId) ||
                !int.TryParse(lblDoctorID.Text, out doctorId) ||
                !int.TryParse(lblAppointmentID.Text, out appointmentId))
            {
                ShowWarning("Please select a valid patient visit before saving.");
                return;
            }

            bool isUpdate = _Mode == enMode.Update;

            bool canEditPrescription = !isUpdate ||
                (_Visit.PrescriptionInfo != null &&
                 _Visit.PrescriptionInfo.IsPrescriptionPending());

            if (!isUpdate && !canEditPrescription)
            {
                ShowWarning(
                    "This prescription is already processed or sent to pharmacy. " +
                    "Prescription changes are not permitted. " +
                    "The visit can only be saved if its save method preserves " +
                    "the existing prescription.");
            }

            _Visit.PatientID = patientId;
            _Visit.DoctorID = doctorId;
            _Visit.Diagnosis = txtDiagnosis.Text.Trim();
            _Visit.VisitNotes = txtNotes.Text.Trim();
            _Visit.AppointmentID = appointmentId;

            // Preserve original visit date and creator on update.
            if (!isUpdate)
            {
                _Visit.VisitDate = DateTime.Now;
                _Visit.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            }

            if (canEditPrescription)
            {
                if (_Visit.PrescriptionInfo == null)
                    _Visit.PrescriptionInfo = new clsPrescription();

                _Visit.PrescriptionInfo.PrescriptionDate =
                    dtpPrescriptionDate.Value;

                _Visit.PrescriptionInfo.VisitID = _Visit.VisitID;

                _Visit.PrescriptionInfo.PrescriptionNotes =
                    txtPrescriptionNotes.Text;

                _Visit.PrescriptionInfo.Prescriptiontype =
                    (byte)clsPrescription.enPrescriptionType.DoctorPrescription;
            }

            try
            {
                Debug.WriteLine("========== VISIT SAVE DEBUG ==========");
                Debug.WriteLine("Visit ID: " + _Visit.VisitID);
                Debug.WriteLine("Update Mode: " + isUpdate);
                Debug.WriteLine("Can Edit Prescription: " + canEditPrescription);

                Debug.WriteLine("Services Count: " +
                    (_Visit.dtServices?.Rows.Count ?? 0));

                Debug.WriteLine("Medicines Count: " +
                    (_Visit.PrescriptionInfo?.dtMedicines?.Rows.Count ?? 0));

                Debug.WriteLine("======================================");

                // IMPORTANT:
                // clsVisit.Save() must not delete, recreate, or overwrite
                // a non-pending prescription during a visit update.
                if (isUpdate && !canEditPrescription)
                {
                    ShowWarning(
                        "This visit contains a pharmacy-processed prescription. " +
                        "Saving is temporarily blocked until clsVisit.Save() " +
                        "is verified to preserve the existing prescription.");
                    return;
                }

                if (!_Visit.Save())
                {
                    ShowError(
                        "An error occurred while saving the visit data. " +
                        "Your entries have been kept.");
                    return;
                }

                if (_Visit.PrescriptionInfo != null)
                {
                    lbPrescriptionID.Text =
                        _Visit.PrescriptionInfo.PrescriptionID.ToString();
                }

                ShowSuccess("Visit details saved successfully.");

                if (isUpdate)
                    return;

                _ResetForm();
                _CallNextPatientInQueue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                ShowError("Unable to save visit: " + ex.Message);
            }
        }

        #endregion


        #region Grid Definitions
        private void _SetupWaitingListGrid()
        {
            dgvDoctorQueue.AutoGenerateColumns = false;

            if (dgvDoctorQueue.Columns.Count == 0)
            {
                // المجموع التقريبي للعرض: 80+70+160+140+100+40 = 590 (مناسب جداً)
                dgvDoctorQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "VisitID", DataPropertyName = "VisitID", HeaderText = "V.ID", Width = 80 });
                dgvDoctorQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "AppointmentID", DataPropertyName = "AppointmentID", HeaderText = "Ap.ID", Width = 70 });
                dgvDoctorQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "PatientName", DataPropertyName = "PatientName", HeaderText = "Patient Name", Width = 160 });
                dgvDoctorQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "CheckInTime", DataPropertyName = "CheckInTime", HeaderText = "Check-In Time", Width = 140 });
                dgvDoctorQueue.Columns.Add(new DataGridViewTextBoxColumn { Name = "StatusText", DataPropertyName = "StatusText", HeaderText = "Status", Width = 100 });

                // عمود الـ CheckBox
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
                {
                    Name = "IsCalledCheck",
                    HeaderText = "Call",
                    DataPropertyName = "IsCalled",
                    TrueValue = 1,
                    FalseValue = 0,
                    Width = 45 // تصغيره قليلاً ليوفر مساحة
                };
                dgvDoctorQueue.Columns.Add(chk);
            }
        }
        private void _SetupServicesGrid()
        {

            dgvServices.AutoGenerateColumns = false;
            if (dgvServices.Columns.Count > 0)
                return;
            dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "ServiceID", DataPropertyName = "ServiceID", Visible = false });
            dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "ServiceName", HeaderText = "Service Name", DataPropertyName = "ServiceName", ReadOnly = true });
            dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "SavedServicePrice", HeaderText = "Unit Price", DataPropertyName = "SavedServicePrice", ReadOnly = true });
            dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Qty", DataPropertyName = "Quantity", ReadOnly = true });
            dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Discount", HeaderText = "Discount", DataPropertyName = "Discount", ReadOnly = false });

            dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", DataPropertyName = "Total", ReadOnly = true });

            DataGridViewImageColumn imgDelete = new DataGridViewImageColumn
            {
                Name = "Delete",
                HeaderText = "Action",
                Image = Properties.Resources.Delete_32,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 50
            };
            dgvServices.Columns.Add(imgDelete);
            dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _Visit.InitializeServicesTable();

            dgvServices.DataSource = _Visit.dtServices;
        }

        private void _SetupMedicinesGrid()
        {

            dgvMedicines.AutoGenerateColumns = false;

            if (dgvMedicines.Columns.Count > 0)
                return;
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "MedicineID", DataPropertyName = "MedicineID", Visible = false });

            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "TaxRate", HeaderText = "TaxRate", DataPropertyName = "TaxRate", Visible = false });

            // الأعمدة المرئية (نستخدم الأسماء الجديدة لتطابق الجدول)
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "SavedMedicineName", HeaderText = "M.Name", DataPropertyName = "SavedMedicineName", ReadOnly = true });
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dosage", HeaderText = "Dosage", DataPropertyName = "Dosage", ReadOnly = true });
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "Instructions", HeaderText = "Instructions", DataPropertyName = "Instructions", ReadOnly = true });

            // استخدام السعر المحفوظ للعرض
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "SavedMedicinePrice", HeaderText = "Unit Price", DataPropertyName = "SavedMedicinePrice", ReadOnly = true });
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Qty", DataPropertyName = "Quantity", ReadOnly = true });
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "Frequency", HeaderText = "FRY", DataPropertyName = "Frequency", ReadOnly = true });
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiscountAmount", HeaderText = "Discount", DataPropertyName = "DiscountAmount", ReadOnly = true });

            // الإجمالي (يستخدم عمود Total المحسوب في الـ DataTable)
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", DataPropertyName = "Total", ReadOnly = true });

            // عمود الحذف
            DataGridViewImageColumn imgDelete = new DataGridViewImageColumn
            {
                Name = "Delete",
                HeaderText = "Action",
                Image = Properties.Resources.Delete_32,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 50
            };
            dgvMedicines.Columns.Add(imgDelete);

            DataGridViewImageColumn imgUpdate = new DataGridViewImageColumn
            {
                Name = "Edit",
                HeaderText = "Action",
                Image = Properties.Resources.edit_32,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 50
            };
            dgvMedicines.Columns.Add(imgUpdate);
            dgvMedicines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvMedicines.DataSource = _Visit.PrescriptionInfo.dtMedicines;
        }
        #endregion

        #region Utilities
        private void _RefreshQueueData()
        {
            DateTime? selectedDate = mdChkToday.Checked ? (DateTime?)null : mdQueueDate.Value.Date;

            DataTable dtQueue = clsVisit.GetPatientsWaitingForDoctors(_DoctorPersonID, selectedDate);
            if (dtQueue == null)
                dtQueue = new DataTable();

            DataView queueView = dtQueue.DefaultView;

            //if (dtQueue.Columns.Contains("StatusText"))
            //{
            //    queueView.RowFilter = "TRIM(StatusText) = 'InQueue'";
            //}

            dgvDoctorQueue.AutoGenerateColumns = false;
            dgvDoctorQueue.DataSource = queueView;

            // Apply existing Today / Date / Search filters.
            
            MdFilterQueue();

            dgvDoctorQueue.BringToFront();
            dgvDoctorQueue.Refresh();

            lblRecordsCount.Text =
                dgvDoctorQueue.Rows
                    .Cast<DataGridViewRow>()
                    .Count(r => !r.IsNewRow)
                    .ToString();
        }
        private void _ResetForm()
        {
            // 1. إعادة ضبط المتغيرات الأساسية
            _Mode = enMode.AddNew;
            _VisitID = -1;
            _AppointmentID = -1;

            // 2. تحديث الحالة البصرية أولاً
            SetFormControlsState(false);
            tcVisitInfo.SelectedIndex = 0;

            // 3. تنظيف الجداول بأمان (Safe Clearing)
            if (_Visit != null)
            {
                // استخدام عامل الـ null-conditional (?.) لمنع الانهيار إذا كان أحد الكائنات null
                _Visit.dtServices?.Clear();

                if (_Visit.PrescriptionInfo != null)
                {
                    _Visit.PrescriptionInfo.dtMedicines?.Clear();
                }
            }

            // 4. إعادة إنشاء كائن الزيارة
            _Visit = new clsVisit();

            // 5. مسح نصوص وعناصر الواجهة
            lbPluse.Text = "[???]  bpm";
            lblTemp.Text = "[???]  °C";
            lbBP.Text = "[???]  mmHg";
            lbWeight.Text = "[???]  kg";

            txtDiagnosis.Clear();
            txtNotes.Clear();
            txtPrescriptionNotes.Clear();

            lbVisitID.Text = "[???]";
            lblPatientID.Text = "[???]";
            lblAppointmentID.Text = "[???]";
            lblDoctorID.Text = "[???]";
            lblVisitID.Text = "[???]";
            lbPrescriptionID.Text = "[???]";

            llDoctorInfo.Enabled = false;
            llPatientInfo.Enabled = false;

            // 6. إعادة ضبط خيارات افتراضية
            nudQuality.Value = 1;
            dtpPrescriptionDate.Value = DateTime.Now;

            // 7. تحديث الحالة البصرية النهائية
            errorProvider1.Clear();
            _RefreshQueueData();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();

            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNextPatient_Click(object sender, EventArgs e)
        {
            if (dgvDoctorQueue.CurrentRow == null) return;

            if (dgvDoctorQueue.SelectedRows.Count > 0)
            {
                // استخدم Convert.ToBoolean فهو يتعامل بذكاء مع الأرقام 0 و 1
                bool IsCalled = Convert.ToBoolean(dgvDoctorQueue.CurrentRow.Cells["IsCalledCheck"].Value);
                if (IsCalled)
                {
                    int AppointmentID = (int)dgvDoctorQueue.CurrentRow.Cells["AppointmentID"].Value;

                    if (ConfirmAction("The Patient IS Called Are youe Update Status To Postpone and Call Next Patient") == DialogResult.Yes)
                    {
                        clsAppointment.UpdateAppointmentStatus(AppointmentID, clsAppointment.enAppointmentStatus.Postponed, clsGlobal.CurrentUser.UserID);
                        _CallNextPatientInQueue();

                    }

                }
                else
                    _CallNextPatientInQueue();



            }
        }

        private void _CallNextPatientInQueue()
        {
            DateTime? selectedDate = mdChkToday.Checked ? (DateTime?)null : mdQueueDate.Value.Date;
            DataTable dtQueue = clsVisit.GetPatientsWaitingForDoctors(_DoctorPersonID, selectedDate);

            if (dtQueue.Rows.Count > 0)
            {
                DataRow row = dtQueue.Rows[0];

                _AppointmentID =
                    Convert.ToInt32(row["AppointmentID"]);

                _VisitID =
                    Convert.ToInt32(row["VisitID"]);

                bool isCalled =
                    row["IsCalled"] != DBNull.Value &&
                    Convert.ToBoolean(row["IsCalled"]);

                if (!isCalled)
                {
                    clsAppointment.UpdatePatientCallStatus(
                        _AppointmentID,
                        true,
                        2);
                }

                _RefreshQueueData();

                _Visit = clsVisit.Find(_VisitID);

                if (_Visit == null)
                {
                    ShowError("The selected visit could not be loaded.");
                    return;
                }

                _FillVitalsInfo();
                _FillPatientInfo();

                // InQueue patients must first be moved to
                // InProgress before consultation editing is enabled.
                SetFormControlsState(false);
            }
            else
            {
                _ResetForm();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _RefreshQueueData();
        }

        private void txtDiagnosis_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDiagnosis.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDiagnosis, "This Falid Is Requaired");
            }
            else
                errorProvider1.SetError(txtDiagnosis, null);
        }
        private void llPatientInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lblPatientID.Text == "[???]")
                return;
            frmPatientInfo frm = new frmPatientInfo(int.Parse(lblPatientID.Text));
            frm.ShowDialog();
        }

        private void llDoctorInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lblDoctorID.Text == "[???]")
                return;
            frmDoctorInfo frm = new frmDoctorInfo(int.Parse(lblDoctorID.Text));
            frm.ShowDialog();
        }

        private void dataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dataGrid = sender as DataGridView;
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && (dataGrid.Columns[e.ColumnIndex].Name == "Delete" || dataGrid.Columns[e.ColumnIndex].Name == "Edit"))
            {
                dataGrid.Cursor = Cursors.Hand; // يتحول الماوس ليد
            }
            else
            {
                dataGrid.Cursor = Cursors.Default; // يعود لشكل السهم
            }
        }

        private void dgvMedicines_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // حذف الدواء
            DeleteRowFromGrid(dgvMedicines, _Visit.PrescriptionInfo.dtMedicines, e, "Are you sure you want to delete this Medicine?");

            // تعديل الدواء (يبقى كما هو لأنه منطق خاص)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.ColumnIndex < dgvMedicines.Columns.Count && dgvMedicines.Columns[e.ColumnIndex].Name == "Edit")
            {
                DataRow selectedRow = ((DataRowView)dgvMedicines.Rows[e.RowIndex].DataBoundItem).Row;
                DataTable _dtAllMedicines = _Visit.PrescriptionInfo.dtMedicines;
                using (frmAddUpdateMedicineToPrescription frm = new frmAddUpdateMedicineToPrescription(ref _dtAllMedicines, selectedRow))
                {
                    frm.DataBack += (s) => RefreshGrid(dgvMedicines, _Visit.PrescriptionInfo.dtMedicines);
                    frm.ShowDialog();
                }
            }
        }

        private void dgvServices_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DeleteRowFromGrid(dgvServices, _Visit.dtServices, e, "Are you sure you want to delete this Service?");
        }
        private bool _ProcessAppointmentStatus(clsAppointment.enAppointmentStatus newStatus, string actionName)
        {
            if (dgvDoctorQueue.CurrentRow == null) return false;
            int appointmentId;
            if (!int.TryParse(Convert.ToString(
                dgvDoctorQueue.CurrentRow.Cells["AppointmentID"].Value), out appointmentId))
                return false;

            if (ConfirmAction("Are you sure you want to " + actionName + "?") != DialogResult.Yes)
                return false;

            try
            {
                if (!clsAppointment.UpdateAppointmentStatus(
                    appointmentId, newStatus, clsGlobal.CurrentUser.UserID))
                {
                    ShowError("Could not update the appointment status.");
                    return false;
                }
                _RefreshQueueData();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ShowError("Status update failed: " + ex.Message);
                return false;
            }
        }

        private void btnInProgress_Click(object sender, EventArgs e)
        {
            if (_Visit == null || _VisitID <= 0 || _AppointmentID <= 0)
            {
                ShowWarning("Please select a patient visit first.");
                return;
            }

            if (ConfirmAction("Start consultation for this patient?")
                != DialogResult.Yes)
            {
                return;
            }

            bool updated = clsAppointment.UpdateAppointmentStatus(
                _AppointmentID,
                clsAppointment.enAppointmentStatus.Progress,
                clsGlobal.CurrentUser.UserID);

            if (!updated)
            {
                ShowError("Unable to start the appointment.");
                return;
            }
            else
            {
                int rowIndex = dgvDoctorQueue.SelectedRows[0].Index;
                int _visitID = Convert.ToInt32(dgvDoctorQueue.Rows[rowIndex].Cells["VisitID"].Value);
                if (_visitID == 0) return;
                int _appointmentID = Convert.ToInt32(dgvDoctorQueue.Rows[rowIndex].Cells["AppointmentID"].Value);
                _Visit = clsVisit.Find(_visitID);
                if (_Visit == null) return;
                int _doctorID = Convert.ToInt32(_Visit.DoctorID);
                int _patientID = Convert.ToInt32(_Visit.PatientID);
                lblAppointmentID.Text = _appointmentID.ToString();
                lblDoctorID.Text = _doctorID.ToString();
                lblPatientID.Text = _patientID.ToString();
            }

                // Enable visit details, services, prescription and saving
                SetFormControlsState(true);

            // Show the visit ID
            lbVisitID.Text = _VisitID.ToString();

            // Refresh waiting list
            _RefreshQueueData();

            // Open Visit Information tab
            tcVisitInfo.SelectedIndex = 0;

            // Focus diagnosis field
            txtDiagnosis.Focus();
        }

        private void btnPostpone_Click(object sender, EventArgs e)
        {
            if (_ProcessAppointmentStatus(clsAppointment.enAppointmentStatus.Postponed,
                "postpone this appointment"))
                _CallNextPatientInQueue();
        }

        private void btnCalnceVisit_Click(object sender, EventArgs e)
        {
            if (_ProcessAppointmentStatus(clsAppointment.enAppointmentStatus.Cancelled,
                "cancel this appointment"))
                _CallNextPatientInQueue();
        }

        private void dgvDoctorQueue_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn column in dgvDoctorQueue.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        
        private void dgvDoctorQueue_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvDoctorQueue.Columns[e.ColumnIndex].Name == "IsCalledCheck")
            {
                bool isCalled = Convert.ToBoolean(dgvDoctorQueue.Rows[e.RowIndex].Cells["IsCalledCheck"].Value);
                int appointmentId = Convert.ToInt32(dgvDoctorQueue.Rows[e.RowIndex].Cells["AppointmentID"].Value);
                if (ConfirmAction("Are you sure you want to " + (isCalled ? "uncall" : "call") + " this patient?") == DialogResult.Yes)
                {
                    clsAppointment.UpdatePatientCallStatus(appointmentId, !isCalled, clsGlobal.CurrentUser.UserID);
                    _RefreshQueueData();
                }
            }
            //int _visitID = Convert.ToInt32(dgvDoctorQueue.Rows[e.RowIndex].Cells["VisitID"].Value);
            //if (_visitID == 0) return;
            //int _appointmentID = Convert.ToInt32(dgvDoctorQueue.Rows[e.RowIndex].Cells["AppointmentID"].Value);
            //_Visit = clsVisit.Find(_visitID);
            //if (_Visit == null) return;
            //int _doctorID = Convert.ToInt32(_Visit.DoctorID);
            //int _patientID = Convert.ToInt32(_Visit.PatientID);
            //lblAppointmentID.Text = _appointmentID.ToString();
            //lblDoctorID.Text = _doctorID.ToString();
            //lblPatientID.Text = _patientID.ToString();

        }

        //private void dgvDoctorQueue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        //    if (dgvDoctorQueue.Columns[e.ColumnIndex].Name == "IsCalledCheck")
        //    {
        //        bool isCalled = Convert.ToBoolean(dgvDoctorQueue.Rows[e.RowIndex].Cells["IsCalledCheck"].Value);
        //        int appointmentId = Convert.ToInt32(dgvDoctorQueue.Rows[e.RowIndex].Cells["AppointmentID"].Value);
        //        if (ConfirmAction("Are you sure you want to " + (isCalled ? "uncall" : "call") + " this patient?") == DialogResult.Yes)
        //        {
        //            clsAppointment.UpdatePatientCallStatus(appointmentId, !isCalled, clsGlobal.CurrentUser.UserID);
        //            _RefreshQueueData();
        //        }
        //    }
        //}

        private void dgvDoctorQueue_SelectionChanged(object sender, EventArgs e)
        {
            //if (dgvDoctorQueue.Rows.Count == 0) return;

            //if (dgvDoctorQueue.CurrentRow != null && dgvDoctorQueue.CurrentRow.Index == 0) return;

            //dgvDoctorQueue.SelectionChanged -= dgvDoctorQueue_SelectionChanged;

            //dgvDoctorQueue.ClearSelection();
            //dgvDoctorQueue.Rows[0].Selected = true;
            //dgvDoctorQueue.CurrentCell = dgvDoctorQueue.Rows[0].Cells[0];

            //dgvDoctorQueue.SelectionChanged += dgvDoctorQueue_SelectionChanged;
        }

        private void frmDoctor_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer2.Stop();

            timer1.Stop();
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح فقط بالأرقام، أزرار التحكم (BackSpace)، والفاصلة
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // منع تكرار الفاصلة العشرية
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDiscount_Leave(object sender, EventArgs e)
        {
            decimal value;
            if (decimal.TryParse(txtDiscount.Text, out value))
            {
                txtDiscount.Text = value.ToString("F2");
            }
            else
            {
                // إذا كان الحقل فارغاً أو خطأ، اجعله صفر
                txtDiscount.Text = "0.00";
            }
        }
        #endregion

        #region Code Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            _RefreshQueueData();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            lbDate.Text = DateTime.Now.ToString();
        }



        #endregion

        private void btnNewService_Click(object sender, EventArgs e)
        {
            using (YamyProject.frmViewItem frm =
                new YamyProject.frmViewItem(0))
            {
                frm.ShowDialog(this);
            }

            _FillServicesInCheckBox();
        }

    }
}