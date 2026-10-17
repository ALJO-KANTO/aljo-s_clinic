using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Clinic.Patient
{
    public partial class frmAddUpdatePatient : Form
    {
        public enum enMode { AddNew = 1, Update = 2 };
        private enMode _Mode = enMode.AddNew;
        private int _PatientID = -1;
        private clsPatient _Patient;

        public delegate void DataBackEventHandler(object sender, int PatientID);

        // Declare an event using the delegate
        public event DataBackEventHandler DataBack;

        public frmAddUpdatePatient()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdatePatient(int patientID)
        {
            InitializeComponent();
            _PatientID = patientID;
            _Mode = enMode.Update;
        }

        private string Tr(string text)
        {
            return TranslationClass.Translate(text, TranslationClass.IsArabic);
        }

        private void ApplyArabicTranslation()
        {
            bool isArabic = TranslationClass.IsArabic;

            this.Text = Tr(this.Text);
            lblTitle.Text = Tr(lblTitle.Text);
            tpPersonalInfo.Text = Tr("Personal Info");
            tpPatientInfo.Text = Tr("Patient Info");

            label6.Text = Tr("Blood Type:");
            label15.Text = Tr("Emergency Contact:");
            label7.Text = Tr("Medical History:");
            label8.Text = Tr("Patient ID:");

            btnPersonInfoNext.Text = isArabic ? Tr("Next") : "Next   ";
            btnSave.Text = isArabic ? Tr("Save") : "   Save";
            btnClose.Text = isArabic ? Tr("Close") : "   Close";

        }

        private void _FillBloodTypesInComboBox()
        {
            DataTable dt = clsBloodType.GetAllBloodTypes(); // تأكد من اسم الكلاس والدالة في طبقة البزنس لديك
            if (dt != null && dt.Rows.Count > 0)
            {
                cbBloodType.DataSource = dt;
                cbBloodType.DisplayMember = "BloodTypeName";
                cbBloodType.ValueMember = "BloodTypeID";
                cbBloodType.SelectedIndex = cbBloodType.FindString("Unknown"); // البدء من أول عنصر دائماً كوضع افتراضي تلافياً للأخطاء
            }
        }

        private void _LoadData()
        {
            _Patient = clsPatient.FindPatientByID(_PatientID);
            if (_Patient == null)
            {
                MessageBox.Show(Tr("No Patient with ID = ") + _PatientID, Tr("Patient Not Found"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            ctrlPersonCardWithFilter1.LoadPersonInfo(_Patient.PersonID);
            ctrlPersonCardWithFilter1.FilterEnabled = false; // قفل الفلتر في التعديل لحماية تماسك البيانات

            lblPatientID.Text = _PatientID.ToString();
            txtEmergencyContact.Text = _Patient.EmergencyContact;
            txtMedicalHistory.Text = _Patient.MedicalHistory;

            // الحماية من خطأ عدم إيجاد فصيلة الدم في الـ ComboBox
            if (_Patient.BloodTypeInfo != null)
            {
                int bloodTypeIndex = cbBloodType.FindString(_Patient.BloodTypeInfo.BloodTypeName);
                if (bloodTypeIndex != -1)
                    cbBloodType.SelectedIndex = bloodTypeIndex;
            }
        }

        private void _ResetDefaultValues()
        {
            _FillBloodTypesInComboBox();

            tpPatientInfo.Enabled = false;
            btnSave.Enabled = false;

            if (_Mode == enMode.AddNew)
            {
                _Patient = new clsPatient();
                ctrlPersonCardWithFilter1.FilterEnabled = true;
                ctrlPersonCardWithFilter1.FocuseTextBox(); // تأكد أن الدالة موجودة بكارت الفلتر لديك
                lblTitle.Text = "Add New Patient";
            }
            else
            {
                lblTitle.Text = "Update Patient Info";
            }

            this.Text = lblTitle.Text;
        }

        private void frmAddUpdatePatient_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();
            if (_Mode == enMode.Update)
                _LoadData();

            ApplyArabicTranslation();
        }

        private void btnPersonInfoNext_Click(object sender, EventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                btnSave.Enabled = true;
                tpPatientInfo.Enabled = true;
                tcPatient.SelectedTab = tcPatient.TabPages["tpPatientInfo"];
                return;
            }

            if (ctrlPersonCardWithFilter1.PersonID == -1)
            {
                MessageBox.Show(Tr("Please Select a Person"), Tr("Select a Person"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FocuseTextBox();
                return;
            }

            // فحص تكرار المريض (Anti-Duplication Check)
            if (clsPatient.IsPatientExistForPersonID(ctrlPersonCardWithFilter1.PersonID))
            {
                MessageBox.Show(Tr("Selected Person already has a patient record, choose another one."), Tr("Select another Person"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FocuseTextBox();
            }
            else
            {
                btnSave.Enabled = true;
                tpPatientInfo.Enabled = true;
                tcPatient.SelectedTab = tcPatient.TabPages["tpPatientInfo"];
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtEmergencyContact_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmergencyContact.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmergencyContact, Tr("Emergency contact number cannot be blank!"));
            }
            else
            {
                errorProvider1.SetError(txtEmergencyContact, null);
            }
        }

        private void txtEmergencyContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            // حماية إدخال الأرقام فقط في رقم الهاتف الطارئ
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show(Tr("Some fields are not valid!, put the mouse over the red icon(s) to see the error"),
                    Tr("Validation Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // تحسين أمني: الحماية من قراءة قيمة فارغة null من الـ ComboBox
            if (cbBloodType.SelectedValue == null)
            {
                MessageBox.Show(Tr("Please select a valid blood type."), Tr("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _Patient.PersonID = ctrlPersonCardWithFilter1.PersonID;
            _Patient.EmergencyContact = txtEmergencyContact.Text.Trim();
            _Patient.MedicalHistory = string.IsNullOrEmpty(txtMedicalHistory.Text.Trim()) ? null : txtMedicalHistory.Text.Trim();
            _Patient.BloodTypeID = (int)cbBloodType.SelectedValue;

            _Patient.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            if (_Patient.SavePatient())
            {
                lblPatientID.Text = _Patient.PatientID.ToString();

                // التبديل لوضع التحديث الفوري وقفل الفلتر حمايةً للبيانات في الذاكرة
                _Mode = enMode.Update;
                ctrlPersonCardWithFilter1.FilterEnabled = false;

                lblTitle.Text = Tr("Update Patient Info");
                this.Text = Tr("Update Patient Info");

                DataBack?.Invoke(this, _Patient.PatientID);
                MessageBox.Show(Tr("Data Saved Successfully."), Tr("Saved"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Tr("Error: Data Is not Saved Successfully."), Tr("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // تحسين أمني حرج: منع تجاوز تبويب اختيار الشخص بالضغط اليدوي بالماوس على التبويبات العلوبة
        private void tcPatient_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage.Name == "tpPatientInfo" && _Mode == enMode.AddNew)
            {
                if (ctrlPersonCardWithFilter1.PersonID == -1 || clsPatient.IsPatientExistForPersonID(ctrlPersonCardWithFilter1.PersonID))
                {
                    e.Cancel = true; // إلغاء الانتقال القسري
                    MessageBox.Show(Tr("Please select a valid person first before moving to patient medical info."), Tr("Selection Required"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void frmAddUpdatePatient_Activated(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.FocuseTextBox();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
