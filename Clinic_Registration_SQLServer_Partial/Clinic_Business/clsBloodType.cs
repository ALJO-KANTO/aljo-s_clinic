using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;

namespace Clinic_Business
{
    public class clsBloodType
    {
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;
        public int BloodTypeID { get; set; }
        public string BloodTypeName { get; set; }

        public clsBloodType()
        {
            this.BloodTypeID = -1;
            this.BloodTypeName = "";
            _Mode = enMode.AddNew;
        }

        public clsBloodType(int bloodTypeID, string bloodTypeName)
        {
            this.BloodTypeID = bloodTypeID;
            this.BloodTypeName = bloodTypeName;
            _Mode = enMode.Update;
        }

        public static clsBloodType Find(int BloodTypeID)
        {
            string BloodTypeName = "";

            if (clsBloodTypeData.FindBloodTypeByID(BloodTypeID, ref BloodTypeName))
            {
                return new clsBloodType(BloodTypeID, BloodTypeName);
            }
            return null;
        }

        public static clsBloodType Find(string BloodTypeName)
        {
            int BloodTypeID = -1;

            if (clsBloodTypeData.FindBloodTypeByName(BloodTypeName, ref BloodTypeID))
            {
                return new clsBloodType(BloodTypeID, BloodTypeName);
            }
            return null;
        }

        public bool Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                   return true;
                    case enMode.Update:
                    return true;
                    default: return false;
            }
        }
        public static DataTable GetAllBloodTypes()
        {
            return clsBloodTypeData.GetAllBloodTypes();
        }
    }
}