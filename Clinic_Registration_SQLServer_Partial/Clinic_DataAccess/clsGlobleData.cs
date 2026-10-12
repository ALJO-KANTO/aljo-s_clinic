using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_DataAccess
{
    public static class clsGlobleData
    {
        public static object ToDBValue(this string value)
        {
            return string.IsNullOrEmpty(value) ? (object)DBNull.Value : value;
        }

        public static string ToStringOrEmpty(this object value)
        {
            return (value == null || value == DBNull.Value) ? string.Empty : value.ToString();
        }


    }
}
