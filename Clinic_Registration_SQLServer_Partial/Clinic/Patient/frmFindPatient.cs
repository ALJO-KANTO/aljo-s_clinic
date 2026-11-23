using System;
using System.Windows.Forms;

namespace Clinic.Patient
{
    public partial class frmFindPatient : Form
    {
        private bool isArabic = false;

        public frmFindPatient()
        {
            InitializeComponent();

            string language =
                Properties.Settings.Default.ApplicationLanguage ?? "en";

            isArabic = language.Equals(
                "ar",
                StringComparison.OrdinalIgnoreCase);

            TranslationHelper.TranslateForm(this, isArabic);
        }

        private void frmFindPatient_Activated(
            object sender, EventArgs e)
        {
            ctrlPatientCardWithFilter1.FocuseFilter();
        }

        private void ctrlPatientCardWithFilter1_PatientCreated(
            object sender,
            global_classes.clsEventArgs e)
        {
            // Patient information is already loaded
            // in ctrlPatientCardWithFilter.

            // No MessageBox needed.
        }

        private void btnExit_Click(
            object sender, EventArgs e)
        {
            this.Close();
        }
    }
}