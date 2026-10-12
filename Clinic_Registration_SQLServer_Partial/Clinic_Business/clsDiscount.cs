using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Business
{
    public class clsDiscount
    {
        public enum enTargetType : byte
        {
            Visit= 1,
            MedicalService = 2,
            Laboratory = 3,
            Medicine = 4
        }
        public static bool ValidateDiscount(int roleID, enTargetType serviceCategoryID, decimal requestedDiscount)
        {
            // 1. جلب الحد المسموح للدور
            decimal userLimit = clsDiscountData.GetMaxDiscountLimitByRole(roleID);

            // 2. جلب الحد المسموح للسياسة (النوع)
            decimal policyLimit = clsDiscountData.GetMaxDiscountPolicy((byte)serviceCategoryID);

            // 3. تحديد الصلاحية النهائية (الأقل هو الأكثر أماناً)
            decimal finalAllowed = Math.Min(userLimit, policyLimit);

            // 4. التحقق: إذا كان الخصم المطلوب أقل من أو يساوي المسموح به فهو مقبول
            return requestedDiscount <= finalAllowed;
        }
    }
}
