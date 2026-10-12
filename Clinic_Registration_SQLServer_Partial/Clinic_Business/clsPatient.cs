

using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsPatient : clsPerson
    {

        private enMode _Mode= enMode.AddNew;
        public int PatientID { get; set; }
        public string EmergencyContact { get; set; }
        public int BloodTypeID { get; set; }
        public clsBloodType BloodTypeInfo { get; set; }
        public string MedicalHistory { get; set; }

        public DateTime CreatedDate { get;private set;  }
        public int CreatedByUserID { get; set; }

        public clsPatient()
        {
            this.PatientID = -1;
            this.EmergencyContact = "";
            this.BloodTypeID = -1;
            this.MedicalHistory = "";
            this.CreatedByUserID = -1;
            _Mode = enMode.AddNew;
        }

        public clsPatient(int PatientID,string EmergencyContact,int BloodTypeID,string MedicalHistory,DateTime CreatedDate, int CreatedByUserID,int PersonID, string Name, DateTime DateOfBirth,
                short Gender, string Address, string PhoneNumber, string Email,int NationalityCountryID,string ImagePath) 
                   
        {
            this.PatientID = PatientID;
            this.EmergencyContact = EmergencyContact;
            this.BloodTypeID = BloodTypeID;
            this.BloodTypeInfo=clsBloodType.Find(BloodTypeID);
            this.MedicalHistory = MedicalHistory;
            this.CreatedDate = CreatedDate;
            this.CreatedByUserID = CreatedByUserID;

            this.PersonID = PersonID;
            this.Name = Name;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this.ImagePath = ImagePath;

            _Mode = enMode.Update;
        }

        private bool _AddNewPatient()
        {
            // تم تحديث اسم كلاس الداتا إلى clsPatientData طبقاً للتصحيح السابق
            this.PatientID = clsPatientData.AddNewPatient(this.PersonID, this.EmergencyContact, this.BloodTypeID, this.MedicalHistory,DateTime.Now,this.CreatedByUserID);
            return this.PatientID > 0;
        }

        private bool _UpdatePatient()
        {
            return clsPatientData.UpdatePatient(this.PatientID, this.PersonID, this.EmergencyContact, this.BloodTypeID, this.MedicalHistory,this.CreatedByUserID);
        }

        public  bool DeletePatient()
        {
            return clsPatientData.DeletePatient(this.PatientID);
        }

        public static clsPatient FindPatientByID(int PatientID)
        {
            string EmergencyContact = "";
            int BloodTypeID = -1;
            string MedicalHistory = "";
            DateTime CreatedDate = DateTime.Now;
            int CreatedByUserID = -1;

            int PersonID = -1;
            string Name = "";
            DateTime DateOfBirth = DateTime.Now;
            short Gender = 0;
            string Address = "";
            string PhoneNumber = "";
            string Email = "";
            int NationalityCountryID = -1;
            string ImagePath = "";

            if (clsPatientData.GetPatientByPatientID(PatientID, ref PersonID, ref EmergencyContact, ref BloodTypeID, ref MedicalHistory,ref CreatedDate, ref CreatedByUserID))
            {
                if (clsPersonData.GetPersonByPersonID(PersonID, ref Name, ref DateOfBirth, ref Gender, ref PhoneNumber, ref Email, ref Address, ref NationalityCountryID, ref ImagePath))
                {
                    return new clsPatient(PatientID, EmergencyContact, BloodTypeID, MedicalHistory,CreatedDate, CreatedByUserID, PersonID, Name, DateOfBirth, Gender, Address, PhoneNumber, Email, NationalityCountryID, ImagePath);
                }
            }
            return null;
        }

        public static DataTable GetAllPatients()
        {
            // تم تصحيح استدعاء الدالة لتطابق اسم GetAllPatients المصحح
            return clsPatientData.GetAllPatients();
        }


        public static DataRow GetPatientByID(int PatientID)
        {
            return clsPatientData.GetPatientByID(PatientID);
        }

        public static bool IsPatientExistForPersonID(int PersonID)
        {
            // استدعاء الدالة مباشرة من طبقة البيانات
            return clsPatientData.IsPatientExistForPersonID(PersonID);
        }

        public bool SavePatient()
        {
 

            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPatient())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdatePatient();
            }

            return false;
        }


    }
}
