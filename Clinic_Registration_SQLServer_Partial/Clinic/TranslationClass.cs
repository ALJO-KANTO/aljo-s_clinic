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
            { "Today", "اليوم" },
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
{ "Doctor ID", "رقم الطبيب" },
{ "Doctor Name", "اسم الطبيب" },
{ "AppointmentType", "نوع الموعد" },
{ "Created By UserID", "رقم المستخدم المنشئ" },
{ "All", "الكل" },
{ "Scheduled", "مجدول" },
{ "Cancelled", "ملغي" },
{ "Completed", "مكتمل" },
{ "Postponed", "مؤجل" },
{ "In Queue", "في قائمة الانتظار" },
{ "Appointments Registry", "سجل المواعيد" },
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
{ "Follow-up", "متابعة" },//manage visits
{ "Manage Visits", "إدارة الزيارات" },
{ "Visit ID", "رقم الزيارة" },
{ "App.ID", "رقم الموعد" },
{ "Visit Date", "تاريخ الزيارة" },
{ "Diagnosis", "التشخيص" },
{ "Open", "مفتوح" },//
 { "Add New Visit", "إضافة زيارة جديدة" },
                { "Update Visit Info", "تحديث معلومات الزيارة" },
                { "Visit Details", "تفاصيل الزيارة" },
                { "Patient Information", "معلومات المريض" },
                { "Doctor Info", "معلومات الطبيب" },
                { "Doctor ID:", "رقم الطبيب:" },
                { "Apponitment ID:", "رقم الموعد:" },
                { "Appointment ID:", "رقم الموعد:" },
                { "Visit ID:", "رقم الزيارة:" },
                { "Visit ID :", "رقم الزيارة:" },
                { "VisitInfo", "معلومات الزيارة" },
                { "Vitals Info", "المعلومات الحيوية" },
                { "Vitals Information", "المعلومات الحيوية" },
                { "Visit Information", "معلومات الزيارة" },
                { "Patient Waiting List", "قائمة انتظار المرضى" },
                { "View Patient Details", "عرض بيانات المريض" },
                { "View Doctor Details", "عرض بيانات الطبيب" },
                { "Visit Notes", "ملاحظات الزيارة" },
                { "Blood Pressure", "ضغط الدم" },
                { "Temperature", "درجة الحرارة" },
                { "Pulse", "النبض" },
                { "BP:", "ضغط الدم:" },
                { "Weight:", "الوزن:" },
                { "Temp:", "درجة الحرارة:" },
                { "Pluse:", "النبض:" },
                { "Pulse:", "النبض:" },
                { "Notes:", "ملاحظات:" },
                { "Notes", "ملاحظات" },
                { "DiscountAmount:", "مبلغ الخصم:" },
                { "Quality :", "الكمية:" },
                { "Services :", "الخدمات:" },
                { "Add New Services", "إضافة خدمات جديدة" },
                { "Services & Procedures", "الخدمات والإجراءات" },
                { "Prescription Info", "معلومات الوصفة الطبية" },
                { "Add New Medicen", "إضافة دواء جديد" },
                { "Add New Medicine", "إضافة دواء جديد" },
                { "Prescription Notes:", "ملاحظات الوصفة الطبية:" },
                { "Prescription Date:", "تاريخ الوصفة الطبية:" },
                { "Prescription ID :", "رقم الوصفة الطبية:" },
                { "Save Visit", "حفظ الزيارة" },
                { "In-Progress", "قيد التنفيذ" },
                { "Cancel Visit", "إلغاء الزيارة" },
                { "Postpone", "تأجيل" },
                { "Next Patient", "المريض التالي" },
                { "Waitting List", "قائمة الانتظار" },
                { "Waiting List", "قائمة الانتظار" },
                { "Date Time", "التاريخ والوقت" },
                { "Modern Clinic - Doctor Station", "العيادة الحديثة - محطة الطبيب" },
                { "Modern Clinic  •  Doctor Station", "العيادة الحديثة  •  محطة الطبيب" },
                { "Service Name", "اسم الخدمة" },
                { "Unit Price", "سعر الوحدة" },
                { "Qty", "الكمية" },
                { "Discount", "الخصم" },
                { "Total", "الإجمالي" },
                { "Action", "الإجراء" },
                { "M.Name", "اسم الدواء" },
                { "Dosage", "الجرعة" },
                { "Instructions", "التعليمات" },
                { "FRY", "التكرار" },
                { "TaxRate", "نسبة الضريبة" },
                { "Check-In Time", "وقت تسجيل الوصول" },
                { "Call", "استدعاء" },
                { "V.ID", "رقم الزيارة" },
                { "Ap.ID", "رقم الموعد" },
                { "Select", "اختيار" },
                { "Add", "إضافة" },
                { "Edit", "تعديل" },
                { "Delete", "حذف" },
                // Add New Visit - additional translations
{ "Modern Clinic", "العيادة الحديثة" },
{ "Records #:", "عدد السجلات:" },
{ "Check-In", "تسجيل الوصول" },
{ "Prescription", "الوصفة الطبية" },
// Add New Visit - remaining Arabic translations
{ "Patient Name", "اسم المريض" },
{ "Weight", "الوزن" },
{ "Next", "التالي" },
{ "→ Next", "التالي ←" },
{ "Records #", "عدد السجلات" },
{ "→ View Doctor Details", "عرض بيانات الطبيب ←" },
{ "→ View Patient Details", "عرض بيانات المريض ←" },
{ "← View Doctor Details", "عرض بيانات الطبيب →" },
{ "← View Patient Details", "عرض بيانات المريض →" },
{ "← Next", "التالي →" },
{ "Please select a visit first.", ".يرجى اختيار زيارة أولاً" },
// Doctor List
{ "Manage Doctors", "إدارة الأطباء" },
{ "Doctor List", "قائمة الأطباء" },
{ "Add New Doctor", "إضافة طبيب جديد" },
{ "LicenseNumber", "رقم الترخيص" },
{ "License #", "رقم الترخيص" },

{ "ConsultationFees", "رسوم الاستشارة" },
{ "Specializations", "التخصصات" },
{ "WorkingDays", "أيام العمل" },
{ "Working Days", "أيام العمل" },
{ "Country", "الدولة" },
{ "CountryName", "اسم الدولة" },
{ "Is Active", "نشط" },
{ "IsActive", "حالة النشاط" },
{ "Yes", "نعم" },
{ "No", "لا" },
{ "Doctor Deleted Successfully.", "تم حذف الطبيب بنجاح." },
{ "Doctor List Error", "خطأ في قائمة الأطباء" },
{ "Doctor data could not be loaded.", "تعذر تحميل بيانات الأطباء." },
{ "Are you sure you want to delete Doctor", "هل أنت متأكد أنك تريد حذف الطبيب" },
{ "Doctor was not deleted because it has data linked to it in the system.", "لم يتم حذف الطبيب لوجود بيانات مرتبطة به في النظام." },

{ "User ID", "رقم المستخدم" },
{ "UserName", "اسم المستخدم" },
{ "Role Name", "اسم الدور" },
{ "# Records:", "عدد السجلات:" },

{ "Translate", "ترجمة" },
{ "Billing_Payments", "الفواتير والمدفوعات" },

{ "X-Ray / Radiology", "الأشعة السينية / الأشعة" },
// Bill Management
{ "Bill Management", "إدارة الفواتير" },
{ "Search Filters", "فلاتر البحث" },
{ "Filter by Date", "تصفية حسب التاريخ" },
{ "All Dates", "جميع التواريخ" },
{ "Bill Status", "حالة الفاتورة" },
{ "Partial Payments", "المدفوعات الجزئية" },
{ "Fully Paid (Today)", "المدفوعة بالكامل (اليوم)" },
{ "Search Patient Name", "البحث باسم المريض" },
{ "Current User:", "المستخدم الحالي:" },

// Bill grid headings
{ "Bill Number", "رقم الفاتورة" },
{ "Bill Date", "تاريخ الفاتورة" },
{ "Total Amount", "المبلغ الإجمالي" },
{ "Paid Amount", "المبلغ المدفوع" },
{ "Balance Due", "المبلغ المتبقي" },
{ "Status", "الحالة" },
{ "Full Pay", "الدفع الكامل" },
{ "Adjustment", "التعديل" },
{ "Cancel Bill", "إلغاء الفاتورة" },

// Dynamic action buttons
{ "Pay", "دفع" },
{ "Pay All", "دفع الكل" },
{ "Complete", "إكمال" },
{ "Print", "طباعة" },
{ "Refund", "استرداد" },
{ "Cancel", "إلغاء" },
{ "Locked", "مقفل" },
{ "Unavailable", "غير متاح" },
{ "BILL MANAGEMENT", "إدارة الفواتير" },
// Bill statuses
{ "Pending", "معلقة" },
{ "Partial", "مدفوعة جزئياً" },
{ "Paid", "مدفوعة" },
{ "Refunded", "مستردة" },
{ "Fully Paid", "مدفوعة بالكامل" },
 { "PROCESS PAYMENTS - System View", "معالجة المدفوعات - عرض النظام" },
        
        { "BillNumber", "رقم الفاتورة" },
        { "PatientName", "اسم المريض" },
        { "TotalAmount", "المبلغ الإجمالي" },
        { "PaidAmount", "المبلغ المدفوع" },
        { "BalanceDue", "الرصيد المستحق" },
        { "Total Bill Amount", "إجمالي مبلغ الفاتورة" },
        { "PaymentAmount", "مبلغ الدفع" },
        { "Select Payment Method", "اختر طريقة الدفع" },
        { "Cash", "نقداً" },
        { "Credit Card", "بطاقة ائتمان" },
        { "InSurance", "التأمين" },
        { "Payment App", "تطبيق الدفع" },
        { "Summary", "الملخص" },
        { "Subtotal", "المجموع الفرعي" },
        { "Tax", "الضريبة" },
        { "Discounts", "الخصومات" },
        { "Medicines", "الأدوية" },
        { "Services", "الخدمات" },
        { "App.Fees", "رسوم الموعد" },
        { "Final Total", "الإجمالي النهائي" },
        { "Payment", "المدفوع" },
        { "Final Payment Due", "المبلغ النهائي المستحق" },
        { "Amount to Pay", "المبلغ المطلوب دفعه" },
        { "Ammount to Pay", "المبلغ المطلوب دفعه" },
        { "Amount Received", "المبلغ المستلم" },
        { "New X-Ray Request", "طلب أشعة سينية جديد" },
        { "Change", "الباقي" },
        { "Confirm Payment", "تأكيد الدفع" },
        { "Bill Not Found", "الفاتورة غير موجودة" },
        { "Unable to Load Payment Details", "تعذر تحميل تفاصيل الدفع" },
        { "Please fill all required fields correctly.", "يرجى تعبئة جميع الحقول المطلوبة بشكل صحيح." },
        { "Input Error", "خطأ في الإدخال" },
        { "Invalid financial numbers. Please check the amounts.", "المبالغ المالية غير صحيحة. يرجى التحقق من المبالغ." },
        { "Policy Restriction", "قيود السياسة" },
        { "Success", "نجاح" },
        { "Payment has been processed successfully!", "تمت معالجة الدفع بنجاح!" },
        { "Payment Error", "خطأ في الدفع" },
        { "Payment Database Error", "خطأ في قاعدة بيانات الدفع" },
        { "Amount cannot exceed the required total!", "لا يمكن أن يتجاوز المبلغ الإجمالي المطلوب!" },
        { "Minimum 70% payment required!", "يجب دفع 70٪ على الأقل!" },
        { "Change to return: ", "المبلغ المتبقي للإرجاع: " },
        { "Remaining Debt: ", "المبلغ المتبقي المستحق: " },
        { "Are you sure you want to confirm this payment?", "هل أنت متأكد من تأكيد هذا الدفع؟" },
        { "X-Ray / Radiology Management", "إدارة الأشعة" },
{ "X-ray requests, imaging studies and patient results", "طلبات الأشعة والفحوصات ونتائج المرضى" },
{ "Active X-Ray Tests", "فحوصات الأشعة النشطة" },
{ "Total Requests", "إجمالي الطلبات" },
{ "In Progress", "قيد التنفيذ" },
{ "Manage X-Ray Tests", "إدارة فحوصات الأشعة" },
{ "Enter Result", "إدخال النتيجة" },
{ "View Image", "عرض الصورة" },
{ "Refresh", "تحديث" },
{ "VisitID", "رقم الزيارة" },
{ "Dates", "التواريخ" },
{ "XRayRequestID", "رقم طلب الأشعة" },
{ "Image", "الصورة" },
{ "View", "عرض" },
{ "TestName", "اسم الفحص" },
{ "RequestedAt", "تاريخ الطلب" },
{ "Search", "بحث" },
{ "Select a request.", "يرجى تحديد طلب." },
{ "Only ordered requests can be edited.", "يمكن تعديل الطلبات التي تم طلبها فقط." },
{ "Completed/cancelled reports cannot be overwritten.", "لا يمكن استبدال التقارير المكتملة أو الملغاة." },
{ "Cancel this X-Ray request?", "هل تريد إلغاء طلب الأشعة هذا؟" },
{ "Confirm", "تأكيد" },
{ "Request could not be cancelled.", "تعذر إلغاء الطلب." },
{ "No available image is attached to this request.", "لا توجد صورة مرفقة بهذا الطلب." },
{ "X-Ray setup", "إعداد الأشعة" },
{ "Load X-Ray", "تحميل بيانات الأشعة" },
{ "Patient / Visit", "المريض / الزيارة" },
{ "X-Ray Test", "فحص الأشعة" },
{ "Clinical Notes", "ملاحظات سريرية" },
{ "Create Request", "إنشاء الطلب" },
{ "New", "جديد" },
{ "Test Name", "اسم الفحص" },
{ "Price", "السعر" },
{ "Active", "نشط" },
{ "Save Test", "حفظ الفحص" },
{ "Deactivate", "إلغاء التفعيل" },
{ "Test ID", "رقم الفحص" },
{ "Deactivate selected test?", "هل تريد إلغاء تفعيل الفحص المحدد؟" },
{ "Unable to load X-Ray tests", "تعذر تحميل فحوصات الأشعة" },//prescription
{ "Prescription ID", "رقم الوصفة الطبية" },
{ "Master Prescriptions Registry", "سجل الوصفات الطبية الرئيسي" },

{ "Invalid Visit ID.", "رقم الزيارة غير صالح." },
{ "Prescription deleted successfully.", "تم حذف الوصفة الطبية بنجاح." },
{ "Prescription not found.", "لم يتم العثور على الوصفة الطبية." },
{ "Delete Failed", "فشل الحذف" },
{ "Search By Patient Name .....", ".....البحث باسم المريض" },
{ "From:", ":من" },
{ "To:", ":إلى" },
{ "Filter by Status", "تصفية حسب الحالة" },//manage drugs
{ "Pharmacist Manage Drug", "إدارة أدوية الصيدلية" },
{ "Medicine Name:", "اسم الدواء:" },
{ "Medicine Price:", "سعر الدواء:" },
{ "Current Stock:", "المخزون الحالي:" },
{ "Reorder Level:", "حد إعادة الطلب:" },
{ "Tax Rate:", "نسبة الضريبة:" },
{ "Search By Medicine Name .....", ".....البحث حسب اسم الدواء" },
{ "ID", "الرقم" },
{ "Medicine Name", "اسم الدواء" },

{ "Stock", "المخزون" },
{ "Reorder", "إعادة الطلب" },
{ "Tax %", "نسبة الضريبة %" },
{ "Actions", "الإجراءات" },
{ "Warning", "تحذير" },
{ "Confirm Action", "تأكيد الإجراء" },
{ "Please correct the input errors before saving.", "يرجى تصحيح أخطاء الإدخال قبل الحفظ." },
{ "Medicine saved successfully.", "تم حفظ الدواء بنجاح." },
{ "Failed to save medicine.", "فشل حفظ الدواء." },
{ "This Medicine Is Not Found ...", "لم يتم العثور على هذا الدواء..." },
{ "Choose action: Yes for Edit, No for Delete", "اختر الإجراء: نعم للتعديل، لا للحذف" },
{ "Medicine Action", "إجراء الدواء" },
{ "Are you sure you want to delete this medicine?", "هل أنت متأكد أنك تريد حذف هذا الدواء؟" },
{ "Deleted Successfully", "تم الحذف بنجاح" },
{ "Medicine name cannot be blank!", "لا يمكن ترك اسم الدواء فارغًا!" },
{ "Please enter a valid price greater than 0!", "يرجى إدخال سعر صحيح أكبر من صفر!" },
{ "Edit  |  Delete", "تعديل  |  حذف" },//laboratory
{ "Laboratory setup", "إعداد المختبر" },
{ "Load laboratory", "تحميل بيانات المختبر" },
{ "Select a lab request first.", "يرجى اختيار طلب مختبر أولاً." },
{ "Completed or cancelled requests cannot be overwritten.", "لا يمكن تعديل الطلبات المكتملة أو الملغاة." },
{ "TotalTests", "إجمالي الفحوصات" },
{ "Laboratory Management" ,"إدارة المختبر"},
{ "ResultNotes", "ملاحظات النتيجة" },
{ "ResultValue", "قيمة النتيجة" },
{ "ResultedAt", "تاريخ النتيجة" },
{"LabRequestID","معرّف طلب المختبر" },
{"Lab requests, tests and patient results","طلبات المختبر والفحوصات ونتائج المرضى" },
{"Active Lab Tests","الفحوصات المخبرية النشطة" },
{"New Lab Request","طلب مختبر جديد" },
{"Manage Lab Tests","إدارة الفحوصات المخبرية" },
{"Enter Lab Result","إدخال نتيجة المختبر" },
            {"Patient","المريض" },
            {"Patient Visit","زيارة المريض" },
            {"Lab Test","الفحص المخبري" },
            {"TestID","معرّف الفحص" },
            { "TestPrice", "سعر الفحص" },



            {"✚  CLINIC\n     MANAGEMENT SYSTEM", "✚  نظام إدارة\n     العيادة" },
{ "Account Ledger", "دفتر الأستاذ" },
{ "Opening Balance", "الرصيد الافتتاحي" },
{ "Closing Balance", "الرصيد الختامي" },
{ "Period Total", "إجمالي الفترة" },
{ "Reference", "المرجع" },
{ "Transaction", "المعاملة" },
{ "Description", "الوصف" },
{ "Debit", "مدين" },
{ "Credit", "دائن" },
{ "Balance", "الرصيد" },
{ "From Date", "من تاريخ" },
{ "To Date", "إلى تاريخ" },
{ "View Ledger", "عرض دفتر الأستاذ" },        };
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

        public static void TranslateAnyForm(Form form, bool isArabic)
        {
            TranslateControls(form, isArabic);
        }

        private static void TranslateControls(Control parent, bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label || control is Button ||
                    control is CheckBox || control is GroupBox)
                {
                    // Preserve the original English text for language switching.
                    if (!(control.Tag is string) ||
                        string.IsNullOrWhiteSpace(control.Tag as string))
                    {
                        control.Tag = control.Text;
                    }

                    string english = control.Tag as string;

                    if (!string.IsNullOrWhiteSpace(english))
                    {
                        string lookupText = english
                            .Replace("\r", "")
                            .Replace("\n", " ")
                            .Trim();

                        string translated =
                            TranslationClass.Translate(lookupText, isArabic);

                        control.Text = isArabic ? translated : english;

                        control.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                        if (control is Label)
                        {
                            Label label = (Label)control;

                            label.TextAlign = isArabic
                                ? ContentAlignment.MiddleRight
                                : ContentAlignment.MiddleLeft;
                        }
                    }
                }

                if (control.HasChildren)
                {
                    TranslateControls(control, isArabic);
                }
            }
        }
    }

}
