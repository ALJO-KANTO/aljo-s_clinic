using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsUser
    {
        public enum enMode { AddNew=1, Update=2}
        private enMode _Mode=enMode.AddNew;

        public int UserID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public int RoleID { get; set; }

       public clsRole RoleInfo { get; set; }

        public clsUser() 
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this.Password = "";
            this.IsActive = false;
            this.RoleID = -1 ;
             _Mode = enMode.AddNew;
        }

        public clsUser (int UserID,int PersonID,string UserName,
            string Password,bool IsActive,int RoleID)
        {

            
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.PersonInfo = clsPerson.Find(PersonID);
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = IsActive;
            this.RoleID = RoleID;
            this.RoleInfo = clsRole.FindByID(RoleID);
             _Mode = enMode.Update;
        }
          

        public static clsUser Find(int UserID)
        {
            int PersonID = -1;

            string UserName = "";  string Password = ""; bool IsActive = false;
            int RoleID = -1;

            if(clsUserData.Find(UserID,ref PersonID,ref UserName ,ref Password,ref IsActive,ref RoleID))
            {
                return new clsUser(UserID, PersonID, UserName, Password, IsActive, RoleID);
            }

            return null;
        }


        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string UserName = "";
            string Password = "";
            bool IsActive = false;
            int RoleID = -1;

            if (clsUserData.FindByPersonID(
                PersonID,
                ref UserID,
                ref UserName,
                ref Password,
                ref IsActive,
                ref RoleID))
            {
                return new clsUser(
                    UserID, PersonID, UserName,
                    Password, IsActive, RoleID);
            }

            return null;
        }


        public static clsUser FindByUsernameAndPassword(string UserName, string Password)
        {
            int UserID = -1;
            int PersonID = -1;
           bool IsActive = false;
            int RoleID = -1;

            if (clsUserData.FindByUsernameAndPassword(UserName, Password, ref UserID, ref PersonID, ref IsActive, ref RoleID))
            {
                return new clsUser(UserID, PersonID, UserName, Password, IsActive, RoleID);
            }

            return null;
        }

        private bool _AddNewUser()
        {
            this.UserID = clsUserData.AddNewUser(this.PersonID, this.UserName, this.Password, this.IsActive, this.RoleID);
            return this.UserID > 0;
        }

        private bool _UpdateUser()
        {
            return clsUserData.UpdateUser(this.UserID,this.PersonID, this.UserName, this.Password, this.IsActive, this.RoleID);
        }

        public  bool DeleteUser()
        {
            return clsUserData.DeleteUser(this.UserID);

        }

        public static DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            return clsUserData.IsUserExistForPersonID(PersonID);
        }

        public static bool IsUserExist(string UserName)
        {
            return clsUserData.IsUserExist(UserName);
        }

        public static DataRow GetUserByID(int UserID)
        {
            return clsUserData.GetUserByID(UserID);
        }

        public static string GetFullName(int UserID)
        {
            return clsUserData.GetFullName(UserID);
        }
        public bool Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                    if(_AddNewUser())
                    {
                        _Mode = enMode.Update;
                        return true;

                    }
                    else
                        return false;   
                case enMode.Update:
                    return _UpdateUser();
                       
            }

            return false;
        }

        

    }

}
