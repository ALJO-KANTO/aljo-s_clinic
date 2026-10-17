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
// Patient information card
{ "Patient Medical Information", "المعلومات الطبية للمريض" },
{ "Blood Type:", "فصيلة الدم:" },
{ "Medical History:", "التاريخ الطبي:" },
{ "Emer. Cont:", "جهة اتصال الطوارئ:" },
{ "Reg. Date:", "تاريخ التسجيل:" },
{ "Created By:", "تم الإنشاء بواسطة:" },

// Person information card
{ "Person Information", "معلومات الشخص" },
{ "Name:", "الاسم:" },
{ "Gender:", "الجنس:" },
{ "Date Of Birth:", "تاريخ الميلاد:" },
{ "Phone:", "الهاتف:" },
{ "Email:", "البريد الإلكتروني:" },
{ "Country:", "الدولة:" },
{ "Address:", "العنوان:" },
{ "Male", "ذكر" },
{ "Female", "أنثى" },
{ "Edit Person Info", "تعديل معلومات الشخص" },
{ "Patient Details", "بيانات المريض" },
{ "No Person with National No. = ", "لا يوجد شخص بالرقم الوطني: " },
{ "Could not find this image: = ", "تعذر العثور على هذه الصورة: " },
// Person information
{ "Person ID", "رقم الشخص" },
{ "Person ID:", "رقم الشخص:" },

// Patient information
{ "Patient ID", "رقم المريض" },
{ "Patient ID:", "رقم المريض:" },

{ "Add New Person", "إضافة شخص جديد" },
{ "Update Person", "تحديث بيانات الشخص" },
{ "Full Name:", "الاسم الكامل:" },
{ "Set Image", "تعيين الصورة" },
{ "Remove", "إزالة" },
{ "Save", "حفظ" },
{ "Person Not Found", "لم يتم العثور على الشخص" },
{ "No Person with ID = ", "لا يوجد شخص بالرقم: " },
{ "This field is required.", "هذا الحقل مطلوب." },
{ "Invalid Email Address Format!", "تنسيق البريد الإلكتروني غير صحيح!" },
{ "Image Files", "ملفات الصور" },
{ "Error Copying Image File", "حدث خطأ أثناء نسخ ملف الصورة" },
{ "Validation Error", "خطأ في التحقق من صحة البيانات" },
{ "Some fields are not valid. Move the mouse over the red icon(s) to see the error.", "بعض الحقول غير صحيحة. مرر مؤشر الفأرة فوق العلامات الحمراء لعرض الخطأ." },
{ "Data Saved Successfully.", "تم حفظ البيانات بنجاح." },
{ "Saved", "تم الحفظ" },
{ "Error: Data Is not Saved Successfully.", "خطأ: لم يتم حفظ البيانات بنجاح." },
{ "Personal Info", "المعلومات الشخصية" },
{ "Patient Info", "بيانات المريض" },
{ "Next", "التالي" },
{ "Selection Required", "الاختيار مطلوب" },
{ "Please select a valid person first before moving to patient medical info.", "يرجى اختيار شخص صالح قبل الانتقال إلى المعلومات الطبية للمريض." },
{ "Filter", "تصفية" },
{ "Find By Person ID:", "البحث برقم الشخص:" },
{ "Person ID : ", "رقم الشخص:" },
{ "Find", "بحث" },
{ "Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "بعض الحقول غير صحيحة! مرر مؤشر الفأرة فوق العلامات الحمراء لعرض الخطأ." },
{ "Selected Person already has a patient record, choose another one.",
  "الشخص المحدد لديه سجل مريض بالفعل، يرجى اختيار شخص آخر." },

{ "Select another Person", "اختر شخصًا آخر" },

{ "Please Select a Person", "يرجى اختيار شخص" },//appointment list
{ "Manage Appointments", "إدارة المواعيد" },
{ "Appointment Management Station", "مركز إدارة المواعيد" },
{ "Filter By:", "تصفية حسب:" },
{ "None", "الكل" },
{ "Appointment ID", "رقم الموعد" },
{ "Patient Name", "اسم المريض" },
{ "Doctor ID", "رقم الطبيب" },
{ "Doctor Name", "اسم الطبيب" },
{ "AppointmentType", "نوع الموعد" },
{ "Status", "الحالة" },
{ "Created By UserID", "رقم المستخدم المنشئ" },
{ "All", "الكل" },
{ "Scheduled", "مجدول" },
{ "Cancelled", "ملغي" },
{ "Completed", "مكتمل" },
{ "Postponed", "مؤجل" },
{ "In Queue", "في قائمة الانتظار" },
{ "Appointments Registry", "سجل المواعيد" },
{ "# Records:", "عدد السجلات:" },
{ "Fees", "الرسوم" },
{ "Date", "التاريخ" },
{ "C.UserID", "رقم المستخدم المنشئ" },
{ "Show Details", "عرض التفاصيل" },
{ "Show Patient Details", "عرض بيانات المريض" },
{ "Show Doctor Details", "عرض بيانات الطبيب" },
{ "&List Patient", "قائمة المرضى" },
{ "Reschedule", "إعادة جدولة الموعد" },
{ "Add &New Appointment", "إضافة موعد جديد" },
{ "&Edit", "تعديل" },
{ "&Delete", "حذف" },
{ "Check_In", "تسجيل الدخول" },
{ "Check In", "تسجيل الدخول" },
{ "View Visit", "عرض الزيارة" },
{ "Consultation", "استشارة" },
{ "Follow-up", "متابعة" },



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
