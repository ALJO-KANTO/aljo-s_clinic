using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Clinic_Business.clsSalesReturn;

namespace Clinic_Business
{
    public class clsBillingService
    {
        public static bool CheckInPatient(clsAppointment Appointment, int UserID, string PaymentMethod)
        {
            return clsBillingServiceData.CheckInPatient(Appointment.AppointmentID, Appointment.DoctorID, Appointment.PatientID, Appointment.AppointmentFees, UserID, PaymentMethod);
        }

        public enum enPaymetnStatus:byte
        {
            Pending = 0, 
            Partial = 1, 
            Paid = 2,
            Refunded = 3,
            Cancelled = 4
        }

        public int BillID { get; private set; }
        public string BillNumber { get; set; }
        public string PatientName { get; set; }
        public decimal AppointmentFees { get; set; }
        public decimal MedicinesTotal { get; set; }
        public decimal ServicesTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal TotalBeforeDiscountAndTax { get; set; }
        public decimal FinalTotalIncludingAll { get; set; }
        public DataTable BillDetailsTable { get; set; }
        public bool IsFound { get; private set; } = false;

        // 2. الـ Constructor الذكي الذي يطلب الـ BillID فقط ويملأ الكائن بالبيانات
        public clsBillingService(int billID)
        {
            this.BillID = billID;
            this.BillDetailsTable = new DataTable();

            // استدعاء الدالة الداخلية للملء
            _LoadBillSummaryData();
        }

        public static DataTable GetBillCompleteDetails(int billID, out clsBillMasterInfo billMaster)
        {
            billMaster = new clsBillMasterInfo();
            string billNumber = "";
            DateTime billDate = DateTime.Now;
            string patientName = "";
            bool isFound = false;

            // استدعاء الدالة المدمجة الذكية من طبقة الـ DAL
            DataTable dtDetails = clsSalesReturnDataAccess.GetBillCompleteInfo(
                billID, ref billNumber, ref billDate, ref patientName, ref isFound
            );

            if (isFound)
            {
                billMaster.BillNumber = billNumber;
                billMaster.BillDate = billDate;
                billMaster.PatientName = patientName;
                billMaster.IsFound = true;
            }
            else
            {
                billMaster.IsFound = false;
            }

            return dtDetails;
        }
        private void _LoadBillSummaryData()
        {
            string bNumber = ""; string pName = "";
            decimal appFees = 0; decimal medTotal = 0; decimal servTotal = 0;
            decimal discount = 0; decimal tax = 0; decimal paid = 0; decimal due = 0;
            decimal totalBefore = 0; decimal finalTotal = 0;

            // تمرير الـ BillID المتاح في الكائن إلى الـ DAL مع متغيرات مؤقتة
            bool isSuccess = clsBillingServiceData.GetBillSummaryData(this.BillID,
                ref bNumber, ref pName, ref appFees, ref medTotal, ref servTotal,
                ref discount, ref tax, ref paid, ref due, ref totalBefore, ref finalTotal, this.BillDetailsTable);

            if (isSuccess)
            {
                // إسناد البيانات المجلوبة لخصائص الكائن الحالي (Object Properties)
                this.BillNumber = bNumber;
                this.PatientName = pName;
                this.AppointmentFees = appFees;
                this.MedicinesTotal = medTotal;
                this.ServicesTotal = servTotal;
                this.TotalDiscount = discount;
                this.TotalTax = tax;
                this.PaymentAmount = paid;
                this.BalanceDue = due;
                this.TotalBeforeDiscountAndTax = totalBefore;
                this.FinalTotalIncludingAll = finalTotal;

                this.IsFound = true;
            }
            else
            {
                this.IsFound = false;
            }
        }
    

        public static bool UpdatePaymentStatus(int BillID,enPaymetnStatus Status)
        {
            return clsBillingServiceData.UpdatePaymentStatus(BillID, (byte)Status);
        }

        public static DataTable GetAllBills()
        {
            return clsBillingServiceData.GetAllBills();
        }

        public static bool CancelBill(int billID,int userID)
        {
            return clsBillingServiceData.CancelBillAndRestoreStock(billID,userID);
        }
    }
}
