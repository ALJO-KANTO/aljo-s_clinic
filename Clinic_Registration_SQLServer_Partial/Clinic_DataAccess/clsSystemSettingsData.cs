using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsSystemSettingsData
    {
        public static decimal GetTaxRateFromSettings()
        {
            decimal taxRate = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // افترضنا أن الجدول يحتوي على صف واحد فقط للإعدادات
                    string query = "SELECT TOP 1 TaxPercentage FROM SystemSettings";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            // تحويل النسبة المئوية (مثلاً 15) إلى كسر عشري (0.15)
                            taxRate = Convert.ToDecimal(result) / 100;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex,clsGlobalLogger.LogLevel.Error);
                taxRate = 0;
            }
            return taxRate;
            
        }
    }
}
