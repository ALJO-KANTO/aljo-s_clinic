
using System;
using System.Drawing;
using System.Windows.Forms;
using Clinic;

namespace Clinic.Patient
{
    public partial class frmPatientInfo : Form
    {
        public frmPatientInfo(int PatientID)
        {
            InitializeComponent();

            ApplyPatientInfoLanguage();

            // Keep the existing patient-loading logic.
            ctrlPatienCardtInfo1.LoadPatientInfo(PatientID);

            // Apply language to the embedded patient information control.
            ctrlPatienCardtInfo1.LoadPatientInfo(PatientID);
        }

        private void ApplyPatientInfoLanguage()
        {
            bool isArabic = TranslationClass.IsArabic;

            // Apply right-to-left layout to the form.
            this.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            this.RightToLeftLayout = isArabic;

            // Translate form title.
            this.Text = TranslationClass.Translate(
                "Patient Details", isArabic);

            // Translate heading labels.
            lblTitle.Text = TranslationClass.Translate(
                "Patient Details", isArabic);

            label1.Text = TranslationClass.Translate(
                "Patient Details", isArabic);

            // Translate Exit button.
            btnExit.Text = TranslationClass.Translate(
                "Close", isArabic);

            // Apply direction and alignment.
            lblTitle.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            label1.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            label1.TextAlign = ContentAlignment.MiddleCenter;

            btnExit.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            btnExit.TextAlign = ContentAlignment.MiddleCenter;

            // Apply RTL direction to nested controls.
            ApplyArabicLayout(this, isArabic);
        }

        private void ApplyArabicLayout(Control parent, bool isArabic)
        {
            parent.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            foreach (Control control in parent.Controls)
            {
                control.RightToLeft = isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control.HasChildren)
                {
                    ApplyArabicLayout(control, isArabic);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
