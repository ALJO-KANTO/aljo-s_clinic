using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsServicesType
    {
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode= enMode.AddNew;

        public int ServiceID { get; set; }

        public string ServiceName { get; set; }
        public string Description { get; set; }
        public float ServiceFees { get; set; }

        public clsServicesType()
        {
            ServiceID = -1;
            ServiceName = string.Empty;
            Description = string.Empty;
            ServiceFees = 0;
            _Mode = enMode.AddNew;
        }

        public clsServicesType(int serviceID, string serviceName, string description, float serviceFees)
        {
            ServiceID = serviceID;
            ServiceName = serviceName;
            Description = description;
            ServiceFees = serviceFees;
            _Mode = enMode.Update;
        }

        private bool _AddNewService()
        {
            this.ServiceID=clsServicesTypeData.AddNewService(this.ServiceName, this.Description, this.ServiceFees);
            return this.ServiceID > 0;
        }

        private bool _UpdateService()
        {
            return clsServicesTypeData.UpdateService(this.ServiceID, this.ServiceName, this.Description, this.ServiceFees);
        }

        public static bool Delete(int ServiceID)
        {
            return clsServicesTypeData.DeleteService(ServiceID);
        }

        public static clsServicesType GetServiceByID(int ServiceID)
        {
            string ServiceName="", Description="";
            float ServiceFees = 0;
            if(clsServicesTypeData.GetServiceByID(ServiceID, ref ServiceName, ref Description, ref ServiceFees))
            {
                return new clsServicesType(ServiceID, ServiceName, Description, ServiceFees);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetServiceList()
        {
             return clsServicesTypeData.GetServiceList();
        }

        public bool Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                    if(_AddNewService())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateService();
                default:
                    return false;
            }
        }
    }
}
