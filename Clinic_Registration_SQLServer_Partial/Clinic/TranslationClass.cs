using Clinic.Patient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    public static class TranslationClass
    {
        public static readonly Dictionary<string, string> sidebarTranslations = new Dictionary<string, string>
        {
            { "Dashboard", "لوحة التحكم" },
            { "Main", "الرئيسية" },
            { "Patients", "المرضى" },
            { "Appointments", "المواعيد" },
            { "Visits", "الزيارات" },
            { "Medical Records", "السجلات الطبية" },
            { "Clinic Services", "خدمات العيادة" },
            { "Billing & Payments", "الفواتير والمدفوعات" },
            { "Invoices", "الفواتير" },
            { "Prescriptions", "الوصفات الطبية" },
            { "Inventory / Medicines", "المخزون / الأدوية" },
            { "Laboratory", "المختبر" },
            { "Reports", "التقارير" },
            { "Administration", "الإدارة" },
            { "Users", "المستخدمون" },
            { "Doctors", "الأطباء" },
            { "Settings", "الإعدادات" },
            { "Sign Out", "تسجيل الخروج" },
            { "Visits Dashboard", "لوحة تحكم الزيارات" },
            { "Diagnosis Notes", "ملاحظات التشخيص" },
            { "Schedule Appointment", "جدولة موعد" },
            { "Appointment Queue", "قائمة الانتظار للمواعيد" },
            { "Patient Check-in", "تسجيل دخول المريض" },
            { "All Prescriptions", "جميع الوصفات الطبية" },
            { "In-Clinic Doctor Prescription", "وصفة الطبيب داخل العيادة" },
            { "Add New Drug", "إضافة دواء جديد" },
            { "Direct Medicine Dispensing", "صرف الدواء مباشرة" },
            { "Manage Bills", "إدارة الفواتير" },
            { "Process Payments", "معالجة المدفوعات" },
            { "Issue Invoice", "إصدار فاتورة" },
            { "Check in Appointment", "تسجيل دخول الموعد" },

            { "Welcome Back", "مرحباً بعودتك" },
            { "Admin", "مدير النظام" },
            { "Doctor", "طبيب" },
            { "Nurse", "ممرض" },
            { "Receptionist", "موظف الاستقبال" },
            { "Cashier", "أمين الصندوق" },
            { "Pharmacist", "صيدلي" },

            { "Doctor Role Dashboard", "لوحة تحكم الطبيب" },
            {"✚   Doctor Role Dashboard", "✚   لوحة تحكم الطبيب" },
            { "Cashier Role Dashboard", "لوحة تحكم أمين الصندوق" },
            {"$   Cashier Role Dashboard", "$   لوحة تحكم أمين الصندوق" },
            { "Pharmacist Role Dashboard", "لوحة تحكم الصيدلي" },
            {"✚   Pharmacist Role Dashboard", "✚   لوحة تحكم الصيدلي" },
            { "Receptionist Role Dashboard", "لوحة تحكم موظف الاستقبال" },
            {"♙   Receptionist Role Dashboard", "♙   لوحة تحكم موظف الاستقبال" },
            { "Nurse Role Dashboard", "لوحة تحكم الممرض" },
            {"✚   Nurse Role Dashboard", "✚   لوحة تحكم الممرض" },

            { "Nurse Station", "محطة التمريض" },
            { "Doctor Station", "محطة الطبيب" },
            { "Cashier Station", "محطة أمين الصندوق" },
            { "Pharmacist Station", "محطة الصيدلي" },
            { "Receptionist Station", "محطة الاستقبال" },

            { "Quick Actions", "إجراءات سريعة" },
            { "Register New Patient", "تسجيل مريض جديد" },
            { "New Appointment", "موعد جديد" },
            { "Find Patient", "البحث عن مريض" },
            { "Manage Users", "إدارة المستخدمين" },

            { "Total Patients", "إجمالي المرضى" },
            { "Appointments Today", "مواعيد اليوم" },
            { "Today's Appointments", "مواعيد اليوم" },
            { "Total Appointments", "إجمالي المواعيد" },
            { "Active Visits", "الزيارات النشطة" },
            { "Pending Bills", "الفواتير المعلقة" },
            { "Total Revenue", "إجمالي الإيرادات" },
            { "Dashboard Overview", "نظرة عامة على لوحة التحكم" },
            { "Role Dashboards", "لوحات التحكم حسب الدور" },

            {"Here's what's happening in your clinic today.","إليك ما يحدث في عيادتك اليوم." },

            { "Search patients...", "البحث عن المرضى..." },
            { "My Account", "حسابي" },
            { "Account", "الحساب" },
            {"⚙  Account", "⚙  الحساب" },
            { "Profile", "الملف الشخصي" },
            { "Change Password", "تغيير كلمة المرور" },
            { "Logout", "تسجيل الخروج" },
            { "Clinic Management", "إدارة العيادة" },
            { "Clinic Management System", "نظام إدارة العيادة" },//patient list
            { "Manage Patients","إدارة المرضى" },
            { "View, search, add, update and manage patients in the clinic","عرض المرضى والبحث عنهم وإضافتهم وتحديث بياناتهم وإدارتهم في العيادة"},
          

{ "Filter By", "تصفية حسب" },

{ "Patient ID", "رقم المريض" },
{ "Person ID", "رقم الشخص" },
{ "Full Name", "الاسم الكامل" },
{ "Emergency Contact", "جهة اتصال الطوارئ" },
{ "Medical History", "التاريخ الطبي" },
{ "Created Date", "تاريخ الإنشاء" },
{ "Keep track of patient information, medical history and visits.", "تابع معلومات المرضى وتاريخهم الطبي وزياراتهم" },
{ "Patient Actions", "إجراءات المريض" },
{ "Add New Patient", "إضافة مريض جديد" },
{ "Update Patient", "تحديث بيانات المريض" },
{ "View Details", "عرض التفاصيل" },
{ "Delete Patient", "حذف المريض" },
{ "Selected Patient", "المريض المحدد" },
{ "Select a row to view patient information", "حدد صفًا لعرض معلومات المريض" },
{ "Right-click a row for more options", "انقر بزر الفأرة الأيمن على صف لعرض المزيد من الخيارات" },
{ "Close", "إغلاق" },
{ "Patient records", "سجلات المرضى" },
{ "Use the list to view all patients", "استخدم القائمة لعرض جميع المرضى" },
{ "Patient Management", "إدارة المرضى" },
{ "Confirm Delete", "تأكيد الحذف" },
{ "Successful", "تم بنجاح" },
{ "Error", "خطأ" },
{ "Patient Deleted Successfully.", "تم حذف المريض بنجاح." },
{ "＋  Add New Patient", "إضافة مريض جديد" },
{ "✎  Update Patient", "تحديث بيانات المريض" },
{ "▣  View Details", "عرض التفاصيل" },
{ "✕  Delete Patient", "حذف المريض" },
{ "⌕   Search & Filter", "البحث والتصفية" },
{ "☷   Patients List", "قائمة المرضى" },




            {"✚  CLINIC\n     MANAGEMENT SYSTEM", "✚  نظام إدارة\n     العيادة" },
        };
        public static bool IsArabic { get; set; } = false;


        public static string Translate(string english, bool isArabic)
        {
            if (!isArabic)
                return english;

            string arabic;

            if (sidebarTranslations.TryGetValue(english, out arabic))
                return arabic;

            return english;
        }
    }

    public static class TranslationHelper
    {
        public static string Translate(string english, bool isArabic)
        {
            return TranslationClass.Translate(english, isArabic);
        }

        public static void TranslateForm(frmFindPatient frmFindPatient, bool isArabic)
        {
            TranslateControls(frmFindPatient, isArabic);
        }

        private static void TranslateControls(Control parent, bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                string english = control.Tag as string;

                if (!string.IsNullOrWhiteSpace(english) &&
                    (control is Label || control is Button))
                {
                    string lookupText = english
                        .Replace("\r", "")
                        .Replace("\n", " ")
                        .Trim();

                    string translated =
                        TranslationClass.Translate(lookupText, isArabic);

                    control.Text = isArabic
                        ? translated
                        : english;

                    if (control is Label)
                    {
                        Label label = (Label)control;

                        label.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                    else if (control is Button)
                    {
                        Button button = (Button)control;

                        button.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                    }
                }

                // Translate all nested panels and layouts.
                if (control.HasChildren)
                {
                    TranslateControls(control,isArabic);
                }
            }
        }
    }
}
