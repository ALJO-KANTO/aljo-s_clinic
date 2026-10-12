using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsSpecialization
    {
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;
        public int SpecializationID { get; set; }

        public string SpecializationName { get; set; }

        public clsSpecialization()
        {
            this.SpecializationID = -1;
            this.SpecializationName = "";
            _Mode = enMode.AddNew;
        }

        public clsSpecialization(int specializationID, string specializationName)
        {
            this.SpecializationID = specializationID;
            this.SpecializationName = specializationName;
            _Mode = enMode.Update;
        }

        public static clsSpecialization Find(int SpecializationID)
        {
            clsSpecialization specialization = null;
            string specializationName = "";
            if (clsSpecializationData.GetSpecializationByID(SpecializationID, ref specializationName))
            {
                specialization = new clsSpecialization(SpecializationID, specializationName);
            }
            return specialization;
        }

        public static clsSpecialization Find(string SpecializationName)
        {
            clsSpecialization specialization = null;
            int specializationID = -1;
            if (clsSpecializationData.GetSpecializationByName(SpecializationName, ref specializationID))
            {
                specialization = new clsSpecialization(specializationID, SpecializationName);
            }
            return specialization;
        }

        private bool _AddNewSpecialization()
        {
            this.SpecializationID = clsSpecializationData.AddNewSpecialization(this.SpecializationName);
            return this.SpecializationID != -1;
        }

        private bool _UpdateSpecialization()
        {
            return clsSpecializationData.UpdateSpecialization(this.SpecializationID, this.SpecializationName);
        }

        public static bool DeleteSpecialization(int SpecializationID)
        {
            return clsSpecializationData.DeleteSpecialization(SpecializationID);
        }

        public bool Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewSpecialization())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateSpecialization();
                default:
                    return false;
            }
        }

        public static DataTable GetAllSpecializations()
        {
            return clsSpecializationData.GetAllSpecializations();
        }
    }
}

