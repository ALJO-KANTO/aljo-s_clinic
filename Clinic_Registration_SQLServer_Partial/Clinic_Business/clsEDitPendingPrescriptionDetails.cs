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
    public class clsEDitPendingPrescriptionDetails
    {
        public int PrescriptionID { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public int BillID { get; set; }
        public bool IsFound { get; set; }
        public DataTable dtDispensedItems { get; set; }

        public clsEDitPendingPrescriptionDetails()
        {
            this.BillID = -1;
            this.IsFound = false;
            this.dtDispensedItems = new DataTable();
            this.PrescriptionID = -1;
            this.PrescriptionDate = new DateTime();
            
        }
        public clsEDitPendingPrescriptionDetails(int BilliID)
        {
            this.BillID = BilliID;
            _LoadData();
        }

        private void _LoadData()
        {
            int PrescriptionID = -1;
            DateTime PrescriptionDate= DateTime.Now;
            bool IsFound = false;
            dtDispensedItems = clsBillingServiceData.GetPrescriptionsDetails(this.BillID, ref PrescriptionID, ref PrescriptionDate, ref IsFound);
            if(IsFound)
            {
                this.PrescriptionID = PrescriptionID;
                this.PrescriptionDate = PrescriptionDate;
                this.IsFound = true;
            }
            else
            {
                this.IsFound=false;
            }
        }

        public bool UpdateAfterEditPendingPrescription(int CreatedByUserID)=> clsBillingServiceData.UpdateAfterEditPendingPrescription(this.dtDispensedItems,this.BillID,CreatedByUserID);


    }
}
