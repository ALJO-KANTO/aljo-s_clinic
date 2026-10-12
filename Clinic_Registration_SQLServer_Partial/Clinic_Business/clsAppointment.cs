using Clinic_DataAccess;
using System;
using System.Data;

namespace Clinic_Business
{
    public class clsAppointment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;
        public enum enAppointmentStatus
        {
            Scheduled = 1, 
            InQueue = 2,
            Progress = 3, 
            Postponed = 4,
            Completed = 5, 
            Cancelled = 6,
            Waiting_For_Vitals= 7,
            Ready_For_Doctor=8,
            In_Billing = 9
        }

        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public clsDoctor DoctorInfo { get; set; }
        public int CreatedByUserID { get; set; }
        public byte AppointmentStatus { get; set; }
        public int AppointmentTypeID { get; set; }
        public clsAppointmentType AppointmentTypeInfo { get; set; }
        public decimal AppointmentFees { get; set; }
        public DateTime AppointmentDate { get; set; }

        public DateTime CreatedDate { get; }
        public string GetAppointmentStatusText
        {
            get
            {
                switch (AppointmentStatus)
                {
                    case 1:
                        return "Scheduled";
                    case 2:
                        return "Completed";
                    case 3:
                        return "Cancelled";
                    default:
                        return "Unknown";
                }
            }
        }

        public clsAppointment()
        {
            this.AppointmentID = -1;
            this.PatientID = -1;
            this.DoctorID = -1;
            this.CreatedByUserID = -1;
            this.AppointmentStatus = 1; // Default: Scheduled
            this.AppointmentTypeID = -1;
            this.AppointmentFees = 0;
            this.AppointmentDate = DateTime.Now;
            this.CreatedDate = DateTime.Now;
            _Mode = enMode.AddNew;
        }

        private clsAppointment(int AppointmentID, int PatientID, int DoctorID, int CreatedByUserID,
            byte AppointmentStatus, int AppointmentTypeID, decimal AppointmentFees, DateTime AppointmentDate,DateTime CreatedDate)
        {
            this.AppointmentID = AppointmentID;
            this.PatientID = PatientID;
            this.DoctorID = DoctorID;
            this.DoctorInfo = clsDoctor.FindDoctorByID(DoctorID);
            this.CreatedByUserID = CreatedByUserID;
            this.AppointmentStatus = AppointmentStatus;
            this.AppointmentTypeID = AppointmentTypeID;
            this.AppointmentTypeInfo = clsAppointmentType.Find(AppointmentTypeID);
            this.AppointmentFees = AppointmentFees;
            this.AppointmentDate = AppointmentDate;
            _Mode = enMode.Update;
        }

        public static clsAppointment Find(int AppointmentID)
        {
            int PatientID = -1, DoctorID = -1, CreatedByUserID = -1, AppointmentTypeID = -1;
            byte AppointmentStatus = 1;
            decimal AppointmentFees = 0;
            DateTime AppointmentDate = DateTime.Now;
            DateTime CreatedDate = DateTime.Now;
            if (clsAppointmentData.GetAppointmentByID(AppointmentID, ref PatientID, ref DoctorID, ref CreatedByUserID,
                ref AppointmentStatus, ref AppointmentTypeID, ref AppointmentFees, ref AppointmentDate,ref CreatedDate))
            {
                return new clsAppointment(AppointmentID, PatientID, DoctorID, CreatedByUserID,
                    AppointmentStatus, AppointmentTypeID, AppointmentFees, AppointmentDate,CreatedDate);
            }
            return null;
        }

        private bool _AddNewAppointment()
        {
            this.AppointmentID = clsAppointmentData.AddNewAppointment(this.PatientID, this.DoctorID, this.CreatedByUserID,
                this.AppointmentStatus, this.AppointmentTypeID, this.AppointmentFees, this.AppointmentDate);
            return (this.AppointmentID != -1);
        }

        private bool _UpdateAppointment()
        {
            return clsAppointmentData.UpdateAppointment(this.AppointmentID, this.PatientID, this.DoctorID, this.CreatedByUserID,
                this.AppointmentStatus, this.AppointmentTypeID, this.AppointmentFees, this.AppointmentDate);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAppointment())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateAppointment();
            }
            return false;
        }

        public static bool DeleteAppointment(int AppointmentID)
        {
            return clsAppointmentData.DeleteAppointment(AppointmentID);
        }

        public static DataTable GetAllAppointments()
        {
            return clsAppointmentData.GetAllAppointments();
        }

        public static DataRow GetAppointmentInfoByID(int AppointmentID)
        {
            return clsAppointmentData.GetAppointmentInfoByID(AppointmentID);
        }
        public static bool IsExists(int AppointmentID)
        {
            return clsAppointmentData.DoesAppointmentExist(AppointmentID);
        }

        public static bool IsPatinentBlakListed(int PatientID)
        {
            return clsAppointmentData.IsPatinentBlakListed(PatientID);
        }

        public static bool IsDoctorBusy(int DoctorID, DateTime AppoinmentTime)
        {
            return clsAppointmentData.IsDoctorBusy(DoctorID, AppoinmentTime);
        }

        public static bool IsDoctorAvailable(int DoctorID, DateTime AppointmentDateTime)
        {
            return clsDoctorData.IsDoctorAvailable(DoctorID, AppointmentDateTime);
        }

        public static DataTable GetWaitingPatients(int DoctorID)
        {
            return clsAppointmentData.GetWaitingPatients(DoctorID);
        }
        public static string BookingValidator(int DoctorID, int PatientID, DateTime AppointmentDateTime, clsAppointmentType.enAppointmentType AppointmentType)
        {
            // 1. أمنياً: التأكد من حالة المريض
            if (clsAppointmentData.IsPatinentBlakListed(PatientID))
                return "Patient is blacklisted and cannot book appointments.";

            // 2. منطقياً: منع حجز المواعيد في الماضي
            if (AppointmentDateTime < DateTime.Now)
                return "Appointment date and time cannot be in the past.";

            // 3. منطقياً: هل المريض مشغول؟
            if (clsAppointmentData.IsPatientHasConflict(PatientID, AppointmentDateTime))
                return "Patient has a conflicting appointment within the 24-hour buffer period.";

            // 4. مهنياً: التأكد من دوام الطبيب (هل يعمل في هذا اليوم وفي هذه الساعة؟)
            // ملاحظة: قمنا بدمج الفحوصات في دالة واحدة IsDoctorAvailable لتقليل الاتصال بقاعدة البيانات
            if (!clsDoctorData.IsDoctorAvailable(DoctorID, AppointmentDateTime))
                return "Doctor is not working or not available at this time.";

            // 5. إدارياً: فحص السعة (إلا في حالة الطوارئ)
            if (AppointmentType != clsAppointmentType.enAppointmentType.Emergency)
            {
                if (!clsAppointmentData.IsTimeCapacityAvailable(DoctorID, AppointmentDateTime, (int)AppointmentType))
                    return "Doctor has no capacity for this appointment.";
            }

            return string.Empty; // كل الشروط مستوفاة
        }

        public static bool UpdateAppointmentStatus(int AppointmentID,enAppointmentStatus Status, int UserID)
        {
            return clsAppointmentData.UpdateAppointmentStatus(AppointmentID, (int)Status, UserID);
        }

        public static bool RescheduleAppointment(int OldAppointmentID, DateTime NewDate, int UserID)
        {
            clsAppointment OldAppointment = clsAppointment.Find(OldAppointmentID);
            if (OldAppointment == null) return false;

            return clsAppointmentData.RescheduleAppointmentTransaction(
                OldAppointmentID,
                OldAppointment.PatientID,
                OldAppointment.DoctorID,
                OldAppointment.CreatedByUserID, // أو يمكنك الاحتفاظ بالـ CreatedByUserID الأصلي
                OldAppointment.AppointmentTypeID,
                OldAppointment.AppointmentFees,
                NewDate,
                UserID
            );
        }

        public static void RefreshQueueForDoctor(int DoctorID)
        {
            // جلب عدد المرضى الموجودين فعلياً في الحالة 7 لهذا الطبيب
            int countInVitals = clsAppointmentData.GetCountByStatusForDoctor(7, DoctorID);

            // إذا كان العدد أقل من 3، قم بترقية مرضى من الحالة 2 إلى الحالة 7
            if (countInVitals < 3)
            {
                int needed = 3 - countInVitals;
                clsAppointmentData.PromotePatientsToVitalsForDoctor(needed, DoctorID);
            }
        }

        public static bool UpdatePatientCallStatus(int AppoinmentID, bool IsCalled,int CallType)
        {
            return clsAppointmentData.UpdatePatientCallStatus(AppoinmentID, IsCalled, CallType);

        }

        public static decimal GetAppointmentFees(int AppoinmentID)
        {
            return clsAppointmentData.GetAppointmentFees(AppoinmentID);
        }
    }
}