using Clinic_DataAccess;
using System;
using System.Data;

namespace Clinic_Business
{
    public class clsVital
    {
        public enum enMode { AddNew = 1, Update = 2 }
        public enMode Mode = enMode.AddNew;

        public int VitalID { get; set; }
        public int AppointmentID { get; set; }
        public int VisitID { get; set; }
        public string BloodPressure { get; set; }
        public decimal Temperature { get; set; }
        public decimal Weight { get; set; }
        public short Pulse { get; set; }
        public DateTime RecordedDate { get; set; }
        public int CreatedByUserID { get; set; }

        // Constructor للـ AddNew
        public clsVital()
        {
            this.VitalID = -1;
            this.AppointmentID = -1;
            this.VisitID = -1;
            this.BloodPressure = "";
            this.Temperature = 0;
            this.Weight = 0;
            this.Pulse = 0;
            this.CreatedByUserID = -1;
            Mode = enMode.AddNew;
        }

        // Constructor للـ Update (Private)
        private clsVital(int VitalID, int AppointmentID, int VisitID, string BloodPressure,
                         decimal Temperature, decimal Weight, short Pulse, DateTime RecordedDate,int CreatedByUserID)
        {
            this.VitalID = VitalID;
            this.AppointmentID = AppointmentID;
            this.VisitID = VisitID;
            this.BloodPressure = BloodPressure;
            this.Temperature = Temperature;
            this.Weight = Weight;
            this.Pulse = Pulse;
            this.RecordedDate = RecordedDate;
            this.CreatedByUserID = CreatedByUserID;
            Mode = enMode.Update;
        }

        private bool _AddNewVitals()
        {
            this.VitalID = clsVitalsData.AddNewVitals(VisitID,AppointmentID, BloodPressure, Temperature, Weight, Pulse,this.CreatedByUserID);
            return (this.VitalID != -1);
        }

        private bool _UpdateVitals()
        {
            return clsVitalsData.UpdateVitals(VitalID, BloodPressure, Temperature, Weight, Pulse, CreatedByUserID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewVitals())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateVitals();
            }
            return false;
        }

        public static clsVital FindByAppointmentID(int AppointmentID)
        {
            int VitalID = -1;
            int VisitID = -1;
            string BloodPressure = "";
            decimal Temperature = 0;
            decimal Weight = 0;
            short Pulse = 0;
            int CreatedByUserID = -1;
            DateTime RecordedDate = DateTime.Now;

            if (clsVitalsData.GetVitalsByAppointmentID(AppointmentID, ref VitalID, ref VisitID, ref BloodPressure,
                                                       ref Temperature, ref Weight, ref Pulse, ref RecordedDate,ref CreatedByUserID))
            {
                return new clsVital(VitalID, AppointmentID, VisitID, BloodPressure, Temperature, Weight, Pulse, RecordedDate, CreatedByUserID);
            }
            return null;
        }

        public static clsVital FindByVisitID(int VisitID)
        {
            int VitalID = -1;
            int AppointmentID = -1;
            string BloodPressure = "";
            decimal Temperature = 0;
            decimal Weight = 0;
            short Pulse = 0;
            int CreatedByUserID = -1;
            DateTime RecordedDate = DateTime.Now;

            if (clsVitalsData.GetVitalsByVisitID(VisitID, ref VitalID, ref AppointmentID, ref BloodPressure,
                                                       ref Temperature, ref Weight, ref Pulse, ref RecordedDate, ref CreatedByUserID))
            {
                return new clsVital(VitalID, AppointmentID, VisitID, BloodPressure, Temperature, Weight, Pulse, RecordedDate, CreatedByUserID);
            }
            return null;
        }

      

        public static DataTable GetPatientsWaitingForVitals()
        {
            return clsVitalsData.GetPatientsWaitingForVitals();
        }
    }
}