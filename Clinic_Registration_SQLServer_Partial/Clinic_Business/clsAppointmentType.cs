using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Clinic_Business
{
    public class clsAppointmentType
    {
        public enum enMode { AddNew=1, Update = 2 }
        private enMode _Mode= enMode.AddNew;

        public enum enAppointmentType
        {
            StandardConsultation = 1,
            Emergency=2,
            FollowUp = 3,
            SpecialistConsultation = 4
             
        }
        public int AppointmentTypeID { get; set; }
        public string TypeName { get; set; }

        public float DefaultFees { get; set; }

        public int DefaultDuration { get; set; }

        public bool IsActive {  get; set; }
        public clsAppointmentType()
        { 
            this.AppointmentTypeID = -1;
            this.TypeName = string.Empty;
            this.DefaultFees = 0.0f;
            this.DefaultDuration = 0;
            this.IsActive = true;

            _Mode = enMode.AddNew;
        }

        public clsAppointmentType(int appointmentTypeID, string typeName, float defaultFees,int DefaultDuration,bool IsActive)
        {
            this.AppointmentTypeID = appointmentTypeID;
            this.TypeName = typeName;
            this.DefaultFees = defaultFees;
            this.DefaultDuration = DefaultDuration;
            this.IsActive = IsActive;
            _Mode = enMode.Update;
        }

        private bool _AddAppointment()
        {
            this.AppointmentTypeID=clsAppointmentTypeData.AddAppointmentType(this.TypeName, this.DefaultFees,this.DefaultDuration,this.IsActive);
            return this.AppointmentTypeID >0;
        }

        private bool _UpdateAppointment()
        {
            return clsAppointmentTypeData.UpdateAppointmentType(this.AppointmentTypeID, this.TypeName, this.DefaultFees, this.DefaultDuration, this.IsActive);
        }

        public static bool Delete(int AppointmentTypeID)
        {
            return clsAppointmentTypeData.DeleteAppointmentType(AppointmentTypeID);
        }

        public static clsAppointmentType Find(int AppointmentTypeID)
        {
            string TypeName = string.Empty;
            float DefaultFees = 0.0f;
            int DefaultDuration = 0;
            bool IsActive = false;

            if (clsAppointmentTypeData.GetAppointmentTypeByID(AppointmentTypeID, ref TypeName, ref DefaultFees,ref DefaultDuration,ref IsActive))
            {
                return new clsAppointmentType(AppointmentTypeID, TypeName, DefaultFees,DefaultDuration,IsActive);
            }

            return null;
        }

        public static DataTable GetAllAppointmentType()
        {
            return clsAppointmentTypeData.GetAllAppintmentType();
        }

        public bool Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                    if(_AddAppointment())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else 
                        return false;
                case enMode.Update:
                    return _UpdateAppointment();
            }
            return false;
        }
    }
}
