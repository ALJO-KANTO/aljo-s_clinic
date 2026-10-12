using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsVisitServices
    {

        public static DataTable GetVisitServices(int VisitID)
        {
            return clsVisitServicesData.GetVisitServices(VisitID);
        }

       
    }
}
