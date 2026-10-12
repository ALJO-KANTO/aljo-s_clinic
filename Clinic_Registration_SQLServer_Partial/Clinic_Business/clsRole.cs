using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsRole
    {
        public enum enMode { AddNew = 1, Update = 2 };
        private enMode _Mode= enMode.AddNew;
        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public clsRole()
        {
            this.RoleID = -1;
            this.RoleName = "";
            _Mode = enMode.AddNew;
        }

        public clsRole(int RoleID, string RoleName)
        {
            this.RoleID = RoleID;
            this.RoleName = RoleName;
            _Mode= enMode.Update;
        }

        public static clsRole FindByName(string RoleName)
        {
            int RoleID = -1;
            if(clsRoleData.FindByName(RoleName,ref RoleID))
                return new clsRole(RoleID,RoleName);
            return null;
        }

        public static clsRole FindByID(int RoleID)
        {
            string RoleName = "";
            if (clsRoleData.FindByID(RoleID, ref RoleName))
                return new clsRole(RoleID, RoleName);
            return null;
        }

        public static DataTable GetAllRole()
        {
            return clsRoleData.GetAllRoles();
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:

                    return true;
                case enMode.Update:
                    return true;
                default: return false;
            }
        }
        }
}
