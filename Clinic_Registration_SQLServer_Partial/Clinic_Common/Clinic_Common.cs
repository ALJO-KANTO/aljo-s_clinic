namespace Clinic_Common
{
    public enum UserRole { Admin = 1, Doctor = 2, Nurse = 3, Receptionist = 4, Staff = 5, Pharmacist = 6, Cashier = 7 }
    public enum enAppointmentStatus
    {
        Scheduled = 1,
        InQueue = 2,
        Progress = 3,
        Postponed = 4,
        Completed = 5,
        Cancelled = 6,
        Waiting_For_Vitals = 7,
        Ready_For_Doctor = 8,
        In_Billing = 9
    }
    public enum enPaymetnStatus : byte
    {
        Pending = 0,
        Partial = 1,
        Paid = 2,
        Refunded = 3,
        Cancelled = 4
    }
}
