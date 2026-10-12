using Clinic.Controls;
using Clinic.ControlsMain;
using Clinic.Doctor;
using Clinic.Patient;
using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
        private int _DoctorPersonID = clsGlobal.CurrentUser.PersonID;
        #endregion

        public enum enMode { AddNew = 1, Update = 2, ShowVisit = 3 }

        #region Constructor
        public frmDoctor(int visitID = -1, enMode mode = enMode.AddNew)
        {
            InitializeComponent();
            _VisitID = visitID;
            _Mode = mode;
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
            if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (ConfirmAction(msg) == DialogResult.Yes)
                {
                    dt.Rows.RemoveAt(e.RowIndex);
                    RefreshGrid(dgv, dt);
                }
            }
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
            _FillServicesInCheckBox();
            timer2.Start();

            if (_Mode == enMode.AddNew)
                _LoadAddNewMode();
            else if (_Mode == enMode.Update)
                _LoadUpdateMode();
            else
                _LoadShowMode();
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
            _LoadVisitData(_VisitID);
            tcVisitInfo.Dock = DockStyle.Fill;

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
            if (_Visit == null)
            {
                ShowError($"Visit with ID {visitID} could not be found.");
                this.Close();
                return;
            }

            // Fill basic info
            _VisitID = visitID;
            _AppointmentID = _Visit.AppointmentID;
            lbVisitID.Text = _VisitID.ToString();
            txtDiagnosis.Text = _Visit.Diagnosis;
            txtNotes.Text = _Visit.VisitNotes;
            pnlAction.Visible = false;
            _FillPatientInfo();
            _FillVitalsInfo();

            // Load detail tables
            dgvServices.DataSource = _Visit.dtServices;

            if (!_Visit.PrescriptionInfo.IsPrescriptionPending())
            {
                pnlPrescriptionInfo.Enabled = false;
            }

            lbPrescriptionID.Text = _Visit.PrescriptionInfo.PrescriptionID.ToString();
            dtpPrescriptionDate.Value = _Visit.PrescriptionInfo.PrescriptionDate;
            txtPrescriptionNotes.Text = _Visit.PrescriptionInfo.PrescriptionNotes;
            dgvMedicines.DataSource = _Visit.PrescriptionInfo.dtMedicines;

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
                clsAppointment.UpdateAppointmentStatus(_AppointmentID, clsAppointment.enAppointmentStatus.Waiting_For_Vitals, clsGlobal.CurrentUser.UserID);
                _RefreshQueueData();
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
            DataTable dtServices = clsServicesType.GetServiceList();
            if (dtServices != null)
            {
                cmbServices.DataSource = dtServices;
                cmbServices.DisplayMember = "ServiceName";
                cmbServices.ValueMember = "ServiceID";
            }
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
            if (_Visit.PrescriptionInfo == null)
                _Visit.PrescriptionInfo = new clsPrescription();
            if (_Visit.PrescriptionInfo.dtMedicines != null && _Visit.PrescriptionInfo.dtMedicines.Columns.Count == 0)
                _Visit.PrescriptionInfo.InitializePrescriptionsTable();
            DataTable _dtAllMedicines = _Visit.PrescriptionInfo.dtMedicines;
            if (_dtAllMedicines != null && _dtAllMedicines.Columns.Count == 0)
                return;
            using (frmAddUpdateMedicineToPrescription frm = new frmAddUpdateMedicineToPrescription(ref _dtAllMedicines))
            {
                // 3. تصحيح صياغة الـ Delegate (DataBack)
                frm.DataBack += (senderForm) =>
                {
                    _Visit.PrescriptionInfo.dtMedicines = _dtAllMedicines;

                    // تحديث الـ DataSource
                    RefreshGrid(dgvMedicines, _Visit.PrescriptionInfo.dtMedicines);
                };

                frm.ShowDialog();
            }
        }


        private void btnFullSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                ShowWarning("Please correct the input errors before saving.");
                return;
            }
            _Visit.PatientID = int.Parse(lblPatientID.Text);
            _Visit.DoctorID = int.Parse(lblDoctorID.Text);
            _Visit.Diagnosis = txtDiagnosis.Text;
            _Visit.VisitNotes = txtNotes.Text;
            _Visit.VisitDate = DateTime.Now;
            _Visit.AppointmentID = int.Parse(lblAppointmentID.Text);
            _Visit.DoctorID = int.Parse(lblDoctorID.Text);
            _Visit.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            _Visit.PrescriptionInfo.PrescriptionDate = dtpPrescriptionDate.Value;
            _Visit.PrescriptionInfo.VisitID = _VisitID;
            _Visit.PrescriptionInfo.PrescriptionNotes = txtPrescriptionNotes.Text;
            _Visit.PrescriptionInfo.Prescriptiontype = (byte)clsPrescription.enPrescriptionType.DoctorPrescription;
            if (_Visit.Save())
            {
                lbPrescriptionID.Text = _Visit.PrescriptionInfo.PrescriptionID.ToString();
                ShowSuccess("Visit details saved successfully.");
            }
            else
            {
                ShowError("An error occurred while saving the visit data.");
            }
            if (_Mode == enMode.Update)
                return;
            _ResetForm();
            _CallNextPatientInQueue();


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
            if (dgvServices.Rows.Count > 0)
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
            DataTable dtQueue = clsVisit.GetPatientsWaitingForDoctors(_DoctorPersonID);
            dgvDoctorQueue.DataSource = dtQueue;
            lblRecordsCount.Text = dtQueue.Rows.Count.ToString();
        }

        private void _ResetForm()
        {
            // 1. إعادة ضبط المتغيرات الأساسية
            _Mode = enMode.AddNew;
            _VisitID = -1;

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
            timer1.Dispose();
            timer2.Stop();
            timer2.Dispose();

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
            DataTable dtQueue = clsVisit.GetPatientsWaitingForDoctors(_DoctorPersonID);

            if (dtQueue.Rows.Count > 0)
            {

                _AppointmentID = (int)dtQueue.Rows[0]["AppointmentID"];
                _VisitID = (int)dtQueue.Rows[0]["VisitID"];
                if ((int)dtQueue.Rows[0]["IsCalled"] != 1)
                {
                    clsAppointment.UpdatePatientCallStatus(_AppointmentID, true, 2);
                }
                _RefreshQueueData();
                _Visit = clsVisit.Find(_VisitID);
                _FillVitalsInfo();
                _FillPatientInfo();
                pnlVisitInfo.Enabled = false;
                btnFullSave.Enabled = false;
            }
            else
                _ResetForm();

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
            if (e.RowIndex >= 0 && dgvMedicines.Columns[e.ColumnIndex].Name == "Edit")
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
        private void _ProcessAppointmentStatus(clsAppointment.enAppointmentStatus newStatus, string actionName)
        {
            if (dgvDoctorQueue.CurrentRow == null) return;

            int currentAppointmentID = (int)dgvDoctorQueue.CurrentRow.Cells["AppointmentID"].Value;

            string message = $"Are you sure you want to {actionName}?";
            string title = "Confirm Action";

            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsAppointment.UpdateAppointmentStatus(currentAppointmentID, newStatus, clsGlobal.CurrentUser.UserID))
                {
                    if (newStatus == clsAppointment.enAppointmentStatus.InQueue)
                    {
                        if (dgvDoctorQueue.CurrentRow == null) return;
                        int appointmentID = (int)dgvDoctorQueue.CurrentRow.Cells["AppointmentID"].Value;
                    }
                    MessageBox.Show($"✅ The operation was completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    _RefreshQueueData();
                }

                else
                {
                    MessageBox.Show($"❌ An error occurred while updating the status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnInProgress_Click(object sender, EventArgs e)
        {
            if (dgvDoctorQueue.CurrentRow == null) return;

            if (dgvDoctorQueue.CurrentRow.Cells["StatusText"].Value.ToString() != "In-Progress")
            {
                _ProcessAppointmentStatus(clsAppointment.enAppointmentStatus.Progress, "Progress this appointment");
               

            }

            SetFormControlsState(true);
            lbVisitID.Text = _VisitID.ToString();

        }

        private void btnPostpone_Click(object sender, EventArgs e)
        {
            _ProcessAppointmentStatus(clsAppointment.enAppointmentStatus.Postponed, "postpone this appointment");
            _CallNextPatientInQueue();

        }

        private void btnCalnceVisit_Click(object sender, EventArgs e)
        {
            _ProcessAppointmentStatus(clsAppointment.enAppointmentStatus.Cancelled, "cancel this appointment");
            _CallNextPatientInQueue();
        }

        private void dgvDoctorQueue_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn column in dgvDoctorQueue.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void dgvDoctorQueue_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDoctorQueue.Rows.Count == 0) return;

            if (dgvDoctorQueue.CurrentRow != null && dgvDoctorQueue.CurrentRow.Index == 0) return;

            dgvDoctorQueue.SelectionChanged -= dgvDoctorQueue_SelectionChanged;

            dgvDoctorQueue.ClearSelection();
            dgvDoctorQueue.Rows[0].Selected = true;
            dgvDoctorQueue.CurrentCell = dgvDoctorQueue.Rows[0].Cells[0];

            dgvDoctorQueue.SelectionChanged += dgvDoctorQueue_SelectionChanged;
        }

        private void frmDoctor_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer2.Stop();
            timer2.Dispose();

            timer1.Stop();
            timer1.Dispose();
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

       
    }
}