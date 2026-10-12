using Clinic_DataAccess;
using System;
using System.Data;

namespace Clinic_Business
{
    public class clsSalesReturn
    {
        // الخصائص العامة لعملية المرتجع (تطابق أعمدة الجدول الرئيسي)
        public int ReturnID { get; private set; }
        public int BillID { get; set; }
        public DateTime ReturnDate { get; private set; }
        public decimal TotalRefund { get; set; }
        public int CashierID { get; set; }

    
        public class clsBillMasterInfo
        {
            public string BillNumber { get; set; }
            public DateTime BillDate { get; set; }
            public string PatientName { get; set; }
            public bool IsFound { get; set; }
        }

        public clsSalesReturn()
        {
            this.ReturnID = -1;
            this.BillID = -1;
            this.ReturnDate = DateTime.Now;
            this.TotalRefund = 0;
            this.CashierID = -1;
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

    
        public bool Save(DataTable dtReturnItems)
        {
            // [Business Rule]: التحقق من أن الجدول يحتوي على أسطر صالحة للإرجاع
            if (dtReturnItems == null || dtReturnItems.Rows.Count == 0)
                return false;

            // [Business Rule]: فحص إضافي للتأكد من عدم وجود تلاعب في الكميات قبل الإرسال لقاعدة البيانات
            foreach (DataRow row in dtReturnItems.Rows)
            {
                int qtyBought = Convert.ToInt32(row["QtyBought"]);
                int qtyReturnedBefore = Convert.ToInt32(row["QtyReturnedBefore"]);
                int qtyReturnedNow = Convert.ToInt32(row["QtyReturned"]);

                // إذا حاول الكاشير إدخال كمية تجعل الإجمالي أكبر من المشتراة
                if (qtyReturnedNow > (qtyBought - qtyReturnedBefore))
                {
                    // رمي استثناء مخصص لطبقة الواجهة لتوضيح الخطأ الحسابي
                    throw new Exception($"الكمية المطلوبة للدواء '{row["SavedMedicineName"]}' تتجاوز الحد المسموح به للإرجاع.");
                }
            }

            // استدعاء دالة الحفظ الاحترافية والـ Bulk المجهزة بالـ Transaction من الـ DAL
            int resultID = clsSalesReturnDataAccess.SaveSalesReturn(
                this.BillID, this.TotalRefund, this.CashierID, dtReturnItems
            );

            if (resultID != -1)
            {
                this.ReturnID = resultID;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsBillPaidOrPartiallyPaid(int billID)
        {
            return clsSalesReturnDataAccess.IsBillPaidOrPartiallyPaid(billID);
        }
    }
}