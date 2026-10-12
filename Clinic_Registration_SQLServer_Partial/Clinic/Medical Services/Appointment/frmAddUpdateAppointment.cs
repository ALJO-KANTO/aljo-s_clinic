using Clinic_Business;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Appointment
{
    public partial class frmAddUpdateAppointment : Form
    {
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;
        private int _AppointmentID = -1;
        private clsAppointment _Appointment;

        private int _PatientID = -1;
        private int _DoctorID = -1;
        private int _PersonIDP = -1;
        private int _PersonIDD = -1;

        public event Action<object,int> DatatBack;

        public frmAddUpdateAppointment()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdateAppointment(int AppointmentID)
        {
            InitializeComponent();
            _AppointmentID = AppointmentID;
            _Mode = enMode.Update;
        }

        private void frmAddUpdateAppointment_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();
            if (_Mode == enMode.Update) _LoadData();
        }

        private void _ResetDefaultValues()
        {
            _FillAppointmentInCompbox();
            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Appointment";
                _Appointment = new clsAppointment();
                ctrDoctorCardInfoWithFilter1.Enabled = false;
                btnSave.Enabled = false;
                paAppointmentInfo.Enabled = false;
                cbStatus.SelectedIndex = 0;
                cbAppointmentType.SelectedIndex = 0;
                dtpAppointmentDate.Value = DateTime.Now;
                dtpAppointmentDate.MinDate = DateTime.Now;
                dtpAppointmentDate.MaxDate = DateTime.Now.AddMonths(1);
            }
            else
            {
                lblTitle.Text = "Update Appointment";
            }
            this.Text = lblTitle.Text;
            cbStatus.Enabled = false;
           
        }

        private void _FillAppointmentInCompbox()
        {
            DataTable dataTable = clsAppointmentType.GetAllAppointmentType();
            cbAppointmentType.ValueMember = "AppointmentTypeID";
            cbAppointmentType.DisplayMember = "TypeName";
            cbAppointmentType.DataSource = dataTable;
            cbAppointmentType.SelectedIndex = 0;
        }

        private void _LoadData()
        {
            _Appointment=clsAppointment.Find(_AppointmentID);
            if (_Appointment == null)
            {
                MessageBox.Show("Appoinment Not Found");
                this.Close();
                return;
            }
            if (clsAppointment.IsPatinentBlakListed(_Appointment.PatientID))
            {
                ShowWarning("This patient is blacklisted and cannot modify appointments.", "Access Denied");
                _Mode = enMode.AddNew;
                _ResetDefaultValues();
                return;
            }

            
            // 2. تحميل عناصر التحكم (Controls)
            ctrlPatientCardWithFilter1.LoadPatientInfo(_Appointment.PatientID);
            ctrlPatientCardWithFilter1.FilterEnabled = false;

            ctrDoctorCardInfoWithFilter1.LoadDoctorInfo(_Appointment.DoctorID);
            ctrDoctorCardInfoWithFilter1.FilterEnabled = false;

            // 3. تعبئة البيانات (Mapping)
            lblAppointmentID.Text = _Appointment.AppointmentID.ToString();
            cbStatus.SelectedIndex = _Appointment.AppointmentStatus - 1;
            cbAppointmentType.SelectedValue = _Appointment.AppointmentTypeID;

            lblPatientID.Text = _Appointment.PatientID.ToString();
            lblDoctorID.Text = _Appointment.DoctorID.ToString();
            lblCreatedByUserID.Text = _Appointment.CreatedByUserID.ToString();

            // 4. معالجة الرسوم (Fees)
            // ملاحظة: قمت بدمجها لأنك كنت تعيد تعيين lblAppointmentTypeFees مرتين في كودك الأصلي
            lblAppointmentTypeFees.Text = _Appointment.AppointmentTypeInfo.DefaultFees.ToString("0.00");
            lblDoctorConsaltantFees.Text = _Appointment.DoctorInfo.ConsultationFees.ToString("0.00");

            _TotalAppointment();

            // 5. الوقت والجدول
            lblWorkingDays.Text = _Appointment.DoctorInfo.GetWorkingDays();
            dtpAppointmentDate.Value = _Appointment.AppointmentDate;

            _DoctorID=_Appointment.DoctorID;
            _PatientID = _Appointment.PatientID;

            bool isExpired = (_Appointment.AppointmentDate < DateTime.Now) ||
                     (_Appointment.CreatedDate < DateTime.Now.AddMinutes(-10));

            if (isExpired)
            {
                btnSave.Enabled = false;
                dtpAppointmentDate.Enabled = false;
                lblTitle.Text = "This appointment is locked (Past time or 10-minute window exceeded).";
                lblTitle.ForeColor = Color.Red;

            }
        }
        private void _MapFormToAppointmentObject()
        {
            _Appointment.PatientID = _PatientID;
            _Appointment.DoctorID = _DoctorID;
            _Appointment.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            _Appointment.AppointmentTypeID = (int)cbAppointmentType.SelectedValue;
            _Appointment.AppointmentFees = Convert.ToDecimal(lblTotalAppointmentFees.Text);
            _Appointment.AppointmentDate = dtpAppointmentDate.Value;
            if (_Mode == enMode.AddNew)
                _Appointment.AppointmentStatus = (byte)clsAppointment.enAppointmentStatus.Scheduled;
        }

        private void _TotalAppointment()
        {
            decimal docFees = 0;
            decimal.TryParse(lblDoctorConsaltantFees.Text, out docFees);

            decimal typeFees = 0;
            decimal.TryParse(lblAppointmentTypeFees.Text, out typeFees);

            // المنطق الاحترافي:
            // إذا كان نوع الموعد "طوارئ" (مثلاً ID = 2)، نعتبر الـ typeFees هي السعر الأساسي.
            // إذا كان موعداً عادياً، نعتمد سعر الطبيب.

            decimal total = 0;

           

            if ((int)cbAppointmentType.SelectedValue == 2) // إذا كان طوارئ
            {
                // الطوارئ: سعر الموعد (ثابت) + أي رسوم إضافية للطبيب إن وجدت
                total = typeFees + (docFees * 0.5m); // مثال: الطبيب يأخذ نسبة 50% إضافية في الطوارئ
            }
            else
            {
                // الكشف العادي: سعر الطبيب هو الأساس
                total = docFees > 0 ? docFees : typeFees;
            }

            lblTotalAppointmentFees.Text = total.ToString("N2");
        }

        private void ShowError(string msg, string title) => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowWarning(string msg, string title) => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                ShowError("Please fill all required fields correctly.", "Validation Error");
                return;
            }

            string error = clsAppointment.BookingValidator(_DoctorID, _PatientID, dtpAppointmentDate.Value, (clsAppointmentType.enAppointmentType)cbAppointmentType.SelectedValue);
            if (!string.IsNullOrEmpty(error))
            {
                ShowWarning(error, "Booking Conflict");
                return;
            }

            _MapFormToAppointmentObject();

            if (_Appointment.Save())
            {
                MessageBox.Show("Appointment Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DatatBack?.Invoke(this,_Appointment.AppointmentID);
               _Mode = enMode.Update;
                lblTitle.Text = "Update Appointment";
                this.Text = lblTitle.Text;
                lblAppointmentID.Text = _Appointment.AppointmentID.ToString();
                _AppointmentID = _Appointment.AppointmentID;
            }
            else
            {
                ShowError("Database error occurred while saving.", "Save Failed");
            }
        }

        private void dtpAppointmentDate_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDateTime = new DateTime(
        dtpAppointmentDate.Value.Year,
        dtpAppointmentDate.Value.Month,
        dtpAppointmentDate.Value.Day,
        dtpAppointmentDate.Value.Hour,
        dtpAppointmentDate.Value.Minute,
        0 // الثواني = 0
                    );

           if (selectedDateTime < DateTime.Now)
    {
        errorProvider1.SetError(dtpAppointmentDate, "Date cannot be in the past.");
        btnSave.Enabled = false;
        return;
    }

            bool isAvailable = clsAppointment.IsDoctorAvailable(_DoctorID, selectedDateTime);
            if (!isAvailable)
            {
                errorProvider1.SetError(dtpAppointmentDate, "Doctor is not available at this time!");
                btnSave.Enabled = false;
            }
            else
            {
                errorProvider1.Clear();
                btnSave.Enabled = true;
            }
        }

        private void btnPersonInfoNext_Click(object sender, EventArgs e)
        {
            if (ctrlPatientCardWithFilter1.PatientID == -1)
            {
                ShowWarning("Please select a patient.", "Missing Information");
                return;
            }
            if (clsAppointment.IsPatinentBlakListed(_PatientID))
            {
                ShowWarning("Patient is blacklisted.", "Access Denied");
                return;
            }
            ctrDoctorCardInfoWithFilter1.Enabled = true;
            tcAppointmentInfo.SelectTab(1);
            ctrDoctorCardInfoWithFilter1.FilterFocus();
        }

        private void btnDoctorInfoNext_Click(object sender, EventArgs e)
        {
            if (_DoctorID == -1)
            {
                ShowWarning("Please select a doctor.", "Missing Information");
                return;
            }
            if (_PersonIDD == _PersonIDP)
            {
                ShowError("Patient cannot be their own doctor.", "Conflict");
                return;
            }
            if (_Mode == enMode.AddNew)
            {
                paAppointmentInfo.Enabled = true;
                lblDoctorID.Text = _DoctorID.ToString();
                lblDoctorConsaltantFees.Text = clsDoctor.GetConsultationFees(_DoctorID).ToString("0.00");

                _TotalAppointment();
                lblPatientID.Text = _PatientID.ToString();
                lblCreatedByUserID.Text = clsGlobal.CurrentUser.UserID.ToString();
                lblWorkingDays.Text = clsDoctor.WorkingDays(_DoctorID);
            }
            cbAppointmentType.Focus();
            paAppointmentInfo.Enabled = true;
            tcAppointmentInfo.SelectTab(2);
        }

        private void cbAppointmentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. التحقق من أن القيمة ليست فارغة (حماية من الـ Runtime Errors)
            if (cbAppointmentType.SelectedValue == null || cbAppointmentType.SelectedValue == DBNull.Value)
                return;

            // 2. جلب البيانات (استخدام الكائن مباشرة)
            int appointmentTypeID = (int)cbAppointmentType.SelectedValue;
            clsAppointmentType appointment = clsAppointmentType.Find(appointmentTypeID);

            // 3. التحقق من وجود النوع قبل الوصول لخصائصه
            if (appointment != null)
            {
                lblAppointmentTypeFees.Text = appointment.DefaultFees.ToString("0.00");

                // ملاحظة: تنسيق المدة بـ "MM" قد يعطي نتائج غير متوقعة إذا لم تكن Duration قيمة وقتية (Time/DateTime).
                // إذا كانت Duration تمثل عدداً من الدقائق، يفضل عرضها كما هي (مثلاً 30).
                lblAppoinmentDuration.Text = appointment.DefaultDuration.ToString();
            }

            _TotalAppointment();
        }

        private void ctrlPatientCardWithFilter1_PatientCreated(object sender, global_classes.clsEventArgs e)
        {
            if (e.ID != -1)
            {
                _PatientID = e.ID;
                _PersonIDP = e.PersonID; 
                btnPersonInfoNext.Focus();
            }

        }

        private void ctrDoctorCardInfoWithFilter1_DoctorCreated(object sender, global_classes.clsEventArgs e)
        {
            if (e.ID != -1)
            {
                _DoctorID = e.ID;
                _PersonIDD = e.PersonID;
                btnDoctorInfoNext.Focus();
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void frmAddUpdateAppointment_Activated(object sender, EventArgs e) => ctrlPatientCardWithFilter1.FocuseFilter();

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}