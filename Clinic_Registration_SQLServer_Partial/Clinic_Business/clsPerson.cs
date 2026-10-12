using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Clinic_Business
{
    public class clsPerson
    {
        public enum enMode { AddNew = 1, Update = 2 }

        public enMode Mode = enMode.AddNew;
        public int PersonID { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public short Gender { set; get; }
        public string Address { set; get; }
        public string PhoneNumber { set; get; }
        public string Email { set; get; }

        public int NationalityCountryID { set; get; }

        public clsCountry CountryInfo { set; get; }
        public string ImagePath { set; get; }

        private clsPerson(int PersonID, string Name, DateTime DateOfBirth,
                short Gender, string Address, string PhoneNumber, string Email,
                    int NationalityCountryID, string ImagePath)
        {
            this.PersonID = PersonID;
            this.Name = Name;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this.ImagePath = ImagePath;
            CountryInfo = clsCountry.FindByID(NationalityCountryID);
            Mode = enMode.Update;
           
        }

        public clsPerson()
        {
            this.PersonID = -1;
            this.NationalityCountryID = -1;
            this.Name = "";
            this.DateOfBirth = DateTime.Now;
            this.Gender = 0;
            this.Address = "";
            this.PhoneNumber = "";
            this.Email = "";
            this.ImagePath = "";
            Mode = enMode.AddNew;

        }

        private bool _AddNewPerson()
        {
            this.PersonID = clsPersonData.AddNewPerson(Name, DateOfBirth, Gender, PhoneNumber, Email, Address, ImagePath, NationalityCountryID);
            return this.PersonID > 0;
        }

        private bool _UpdatePerson()
        {
            return clsPersonData.UpdatePerson(PersonID, Name, DateOfBirth, Gender, PhoneNumber, Email, Address, ImagePath, NationalityCountryID);
        }

        public bool Delete()
        {
            return clsPersonData.DeletePerson(this.PersonID);
        }

        public static bool Delete(int PersonID)
        {
            return clsPersonData.DeletePerson(PersonID);
        }
        public static bool DeleteAllForTrash()
        {
            return clsPersonData.DeleteAllForTrash();
        }

        public static clsPerson Find(int PersonID)
        {
            string name = "";
            DateTime dateOfBirth = DateTime.Now;
            short Gender = 0;
            string address = "";
            string phoneNumber = "";
            string email = "";
            int nationalcontry = -1;
            string imagePath = "";

          if(clsPersonData.GetPersonByPersonID(PersonID, ref name, ref dateOfBirth, ref Gender, ref phoneNumber, ref email, ref address, ref nationalcontry, ref imagePath))
            {
                return new clsPerson(PersonID, name, dateOfBirth, Gender, address, phoneNumber, email, nationalcontry, imagePath);
            }
            else
            {
                return null;
            }



        }

        public static DataTable GetAllPersons()
        {
            return clsPersonData.GetAllPersons();
        }

        public static DataTable GetAllPersonsToTrash()
        {
            return clsPersonData.GetAllPersonsTrash();
        }

        public static DataRow GetPersonByID(int PersonID)
        {
            return clsPersonData.GetPersonByID(PersonID);
        }

        public static bool SoftDelete(int PersonID)
        {
            return clsPersonData.SoftDeletePerson(PersonID);
        }

        public static bool Restore(int PersonID)
        {
            return clsPersonData.RestorePerson(PersonID);
        }

        public bool Save()
        {

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdatePerson();
            }
            return false;
        }
    }
}
