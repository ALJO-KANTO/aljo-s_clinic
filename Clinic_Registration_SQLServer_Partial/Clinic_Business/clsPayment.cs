using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Clinic_Business
{
   
    public class clsPayment
    {
        public int PaymentID { get; set; }
        public int BillID { get; set; }
        public decimal PaymentAmount { get; set; }

        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public int CreatedByUserID { get; set; }

       public clsBillingService.enPaymetnStatus Status { get; set; }
        public clsPayment()
        {
            PaymentID = -1;
            BillID = -1;
            CreatedByUserID = -1;
            PaymentAmount = 0.0m;
            PaymentDate=DateTime.Now;
            PaymentMethod = "Cash";
        }

        private bool _AddPayment()
        {
            return clsPaymentData.AddPaymentAndSetStatus(BillID, PaymentAmount,PaymentMethod,CreatedByUserID,(byte)Status);
        }

        public bool Save()
        {
            return _AddPayment();
        }

    }
}
