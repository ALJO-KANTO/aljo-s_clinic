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

namespace Clinic.Controls
{
    public partial class ctrlPatienCardtInfo : UserControl
    {
        private int _PatientID = -1;
        private clsPatient _Patient;

        public ctrlPatienCardtInfo()
        {
            InitializeComponent();
        }

        public int PatientID
        {
            get { return _PatientID; }
        }

        public clsPatient SelectedPatient
        {
            get { return _Patient; }
        }

        public void ResetPetientInfo()
        {
            // 1. تصفير كارت الشخص أولاً
            ctrlPersonCard1.ResetPersonInfo();

            // 2. تصفير النصوص والـ Labels
            lblPatientID.Text = "[???]";
            lblBloodType.Text = "[???]";
            lblEmergencyContact.Text = "[???]";
            lblCreatedDate.Text = "[???]";
            lblCreatedBy.Text = "[???]";
            txtMedicalHistory.Text = string.Empty;

            // 3. إعادة تعيين المتغيرات البرمجية
            _PatientID = -1;
            _Patient = null;
        }

        private void _FillPetientInfo()
        {
            // تثبيت الـ ID المكتشف في حقل الفئة
            _PatientID = _Patient.PatientID;

            // 💡 تمرير الـ PersonID الصحيح لكارت الشخص ليعرض الاسم والصورة بنجاح
            ctrlPersonCard1.LoadPersonInfo(_Patient.PersonID);

            // ملء الحقول الخاصة بالمريض
            lblPatientID.Text = _PatientID.ToString();
            lblEmergencyContact.Text = !string.IsNullOrEmpty(_Patient.EmergencyContact)?_Patient.EmergencyContact: "No EmergencyContact recorded ";
            lblBloodType.Text = _Patient.BloodTypeInfo.BloodTypeName;
            lblCreatedDate.Text = clsFormat.DateToShort(_Patient.CreatedDate);

            // استدعاء دالة اسم المستخدم التي قمنا بتأمينها مسبقاً من الـ Null
            lblCreatedBy.Text = clsUser.GetFullName(_Patient.CreatedByUserID);

            // معالجة التاريخ الطبي في الـ TextBox
            txtMedicalHistory.Text = !string.IsNullOrEmpty(_Patient.MedicalHistory) ?
                                    _Patient.MedicalHistory : "No medical history recorded.";
        }

        public void LoadPatientInfo(int PatientID)
        {
        
            _Patient = clsPatient.FindPatientByID(PatientID);

            txtMedicalHistory.Enabled = false;

            if (_Patient == null)
            {
                ResetPetientInfo();
                MessageBox.Show("No Patient with ID = " + PatientID, "Patient Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _FillPetientInfo();
        }
    }
}