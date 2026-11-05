using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Clinic
{
    public static class ClinicLanguageManager
    {
        public static bool IsArabic
        {
            get
            {
                return string.Equals(
                    Properties.Settings.Default.ApplicationLanguage,
                    "ar", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static void Initialize()
        {
            TranslationClass.IsArabic = IsArabic;
        }

        public static void SetLanguage(bool arabic)
        {
            Properties.Settings.Default.ApplicationLanguage = arabic ? "ar" : "en";
            Properties.Settings.Default.Save();
            TranslationClass.IsArabic = arabic;

            // Copy first: form collections can change while UI is being refreshed.
            var openForms = new List<Form>();
            foreach (Form form in Application.OpenForms)
                openForms.Add(form);

            foreach (Form form in openForms)
            {
                if (form == null || form.IsDisposed)
                    continue;

                // These two forms already implement their own language/layout logic.
                if (form is frmMain || form is Clinic.Login.frmLogin)
                    continue;

                TranslationHelper.TranslateAnyForm(form, arabic);
            }
        }

        public static void InitializeForm(Form form)
        {
            ApplyToForm(form);
        }

        public static void ApplyToForm(Form form)
        {
            if (form == null || form.IsDisposed)
                return;

            TranslationClass.IsArabic = IsArabic;
            if (form is frmMain || form is Clinic.Login.frmLogin)
                return;

            TranslationHelper.TranslateAnyForm(form, IsArabic);
        }
    }
}
