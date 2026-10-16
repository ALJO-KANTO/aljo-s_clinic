
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Controls
{
    public partial class ctrlPatienCardtInfo : UserControl
    {
        private int _PatientID = -1;
        private clsPatient _Patient;

        // Save the original positions of the medical-information controls.
        private readonly Dictionary<Control, Point>
            _medicalInfoOriginalPositions =
                new Dictionary<Control, Point>();

        private bool _medicalInfoPositionsSaved = false;

        public ctrlPatienCardtInfo()
        {
            InitializeComponent();
            ApplyPatientCardLanguage();
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
            // Reset person information.
            ctrlPersonCard1.ResetPersonInfo();

            // Reset patient information.
            lblPatientID.Text = "[???]";
            lblBloodType.Text = "[???]";
            lblEmergencyContact.Text = "[???]";
            lblCreatedDate.Text = "[???]";
            lblCreatedBy.Text = "[???]";
            txtMedicalHistory.Text = string.Empty;

            // Reset stored patient data.
            _PatientID = -1;
            _Patient = null;

            ApplyPatientCardLanguage();
        }

        private void _FillPetientInfo()
        {
            if (_Patient == null)
                return;

            _PatientID = _Patient.PatientID;

            // Load existing person information.
            ctrlPersonCard1.LoadPersonInfo(_Patient.PersonID);

            lblPatientID.Text = _PatientID.ToString();

            lblEmergencyContact.Text =
                !string.IsNullOrEmpty(_Patient.EmergencyContact)
                    ? _Patient.EmergencyContact
                    : TranslationClass.Translate(
                        "No EmergencyContact recorded ",
                        TranslationClass.IsArabic);

            lblBloodType.Text = _Patient.BloodTypeInfo != null
                ? _Patient.BloodTypeInfo.BloodTypeName
                : string.Empty;

            lblCreatedDate.Text =
                clsFormat.DateToShort(_Patient.CreatedDate);

            lblCreatedBy.Text =
                clsUser.GetFullName(_Patient.CreatedByUserID);

            txtMedicalHistory.Text =
                !string.IsNullOrEmpty(_Patient.MedicalHistory)
                    ? _Patient.MedicalHistory
                    : TranslationClass.Translate(
                        "No medical history recorded.",
                        TranslationClass.IsArabic);
        }

        public void LoadPatientInfo(int PatientID)
        {
            _Patient = clsPatient.FindPatientByID(PatientID);

            txtMedicalHistory.Enabled = false;
            txtMedicalHistory.ReadOnly = true;

            if (_Patient == null)
            {
                ResetPetientInfo();

                MessageBox.Show(
                    TranslationClass.Translate(
                        "No Patient with ID = ",
                        TranslationClass.IsArabic) + PatientID,
                    TranslationClass.Translate(
                        "Patient Not Found",
                        TranslationClass.IsArabic),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return;
            }

            // Load the original database information first.
            _FillPetientInfo();

            // Apply the selected language and layout.
            ApplyPatientCardLanguage();
        }

        private void ApplyPatientCardLanguage()
        {
            bool isArabic = TranslationClass.IsArabic;

            // Preserve existing person-card localization.
            ctrlPersonCard1.ApplyLanguage();

            gbPatientAdditionalInfo.Text =
                TranslationClass.Translate(
                    "Patient Medical Information", isArabic);

            label1.Text =
                TranslationClass.Translate("Patient ID:", isArabic);

            label7.Text =
                TranslationClass.Translate("Blood Type:", isArabic);

            label13.Text =
                TranslationClass.Translate("Medical History:", isArabic);

            label3.Text =
                TranslationClass.Translate("Emer. Cont:", isArabic);

            label2.Text =
                TranslationClass.Translate("Reg. Date:", isArabic);

            label4.Text =
                TranslationClass.Translate("Created By:", isArabic);

            // Preserve the existing RTL behavior for the full control.
            ApplyPatientCardDirection(this, isArabic);

            // Mirror only the circled medical-information section.
            ApplyMedicalInfoDirection(isArabic);
        }

        private void ApplyMedicalInfoDirection(bool isArabic)
        {
            if (gbPatientAdditionalInfo == null)
                return;

            gbPatientAdditionalInfo.SuspendLayout();

            try
            {
                // Save positions once, after the control has been laid out.
                if (!_medicalInfoPositionsSaved)
                {
                    foreach (Control control
                        in gbPatientAdditionalInfo.Controls)
                    {
                        _medicalInfoOriginalPositions[control] =
                            control.Location;
                    }

                    _medicalInfoPositionsSaved = true;
                }

                gbPatientAdditionalInfo.RightToLeft =
                    isArabic ? RightToLeft.Yes : RightToLeft.No;

                foreach (Control control
                    in gbPatientAdditionalInfo.Controls)
                {
                    control.RightToLeft =
                        isArabic ? RightToLeft.Yes : RightToLeft.No;

                    Point originalPosition =
                        _medicalInfoOriginalPositions[control];

                    if (isArabic)
                    {
                        // Mirror the control horizontally inside the group box.
                        control.Left =
                            gbPatientAdditionalInfo.ClientSize.Width
                            - originalPosition.X
                            - control.Width;
                    }
                    else
                    {
                        // Restore its original English position.
                        control.Location = originalPosition;
                    }

                    if (control is Label label)
                    {
                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                    else if (control is TextBox textBox)
                    {
                        textBox.TextAlign = isArabic
                            ? HorizontalAlignment.Right
                            : HorizontalAlignment.Left;
                    }
                }
            }
            finally
            {
                gbPatientAdditionalInfo.ResumeLayout(true);
            }
        }

        private void ApplyPatientCardDirection(
            Control parent,
            bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                control.RightToLeft =
                    isArabic ? RightToLeft.Yes : RightToLeft.No;

                if (control is Label label)
                {
                    label.TextAlign = isArabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }
                else if (control is TextBox textBox)
                {
                    textBox.RightToLeft =
                        isArabic ? RightToLeft.Yes : RightToLeft.No;

                    textBox.TextAlign = isArabic
                        ? HorizontalAlignment.Right
                        : HorizontalAlignment.Left;
                }

                if (control.HasChildren)
                {
                    ApplyPatientCardDirection(control, isArabic);
                }
            }
        }
    }
}
