using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            { "Clinic Management System", "نظام إدارة العيادة" },

            {"✚  CLINIC\n     MANAGEMENT SYSTEM", "✚  نظام إدارة\n     العيادة" },
        };

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
}
