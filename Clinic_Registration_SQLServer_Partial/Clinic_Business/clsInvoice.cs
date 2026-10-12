using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    // داخل clsInvoice.cs
    public class clsInvoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public decimal FinalTotal { get; set; }
        public decimal Subtotal { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public DataTable ItemsTable { get; set; }

        public static clsInvoice GetInvoice(int billID)
        {
            DataSet ds = clsInvoiceData.GetInvoiceFullData(billID);
            if (ds.Tables.Count < 3 || ds.Tables[0].Rows.Count == 0) return null;

            clsInvoice invoice = new clsInvoice();

            // تعبئة الملخص المالي (Table 0)
            var rowSum = ds.Tables[0].Rows[0];
            invoice.FinalTotal = (decimal)rowSum["FinalTotal"];
            invoice.Subtotal = (decimal)rowSum["Subtotal"];
            invoice.PaymentAmount = (decimal)rowSum["PaymentAmount"];
            invoice.TotalDiscount = (decimal)rowSum["TotalDiscount"];

            // تعبئة تفاصيل المريض (Table 1)
            if (ds.Tables[1].Rows.Count > 0)
            {
                var rowHeader = ds.Tables[1].Rows[0];
                invoice.InvoiceNumber = rowHeader["InvoiceNumber"].ToString();
                invoice.InvoiceDate = (DateTime)rowHeader["InvoiceDate"];
                invoice.PatientName = rowHeader["PatienName"].ToString();
                invoice.PatientID = (int)rowHeader["PatientID"];
            }

            // تعبئة الأصناف (Table 2)
            invoice.ItemsTable = ds.Tables[2];

            return invoice;
        }
    }
}
