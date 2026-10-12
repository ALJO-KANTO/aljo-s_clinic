
using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;


namespace Clinic_Business
{
    public class clsCountry
    {

        public int CountryID { set; get; }
        public string CountryName { set; get; }

        private static List<clsCountry> _CountriesCache = new List<clsCountry>();
        public clsCountry()

        {
            this.CountryID = -1;
            this.CountryName = "";

        }

        private clsCountry(int ID, string CountryName)

        {
            this.CountryID = ID;
            this.CountryName = CountryName;
        }

        public static clsCountry FindByID(int ID)
        {
            string CountryName = "";

            if (clsCountryData.GetCountryInfoByID(ID, ref CountryName))

                return new clsCountry(ID, CountryName);
            else
                return null;

        }

        public static clsCountry Find(string CountryName)
        {

            int ID = -1;

            if (clsCountryData.GetCountryInfoByName(CountryName, ref ID))

                return new clsCountry(ID, CountryName);
            else
                return null;

        }

        public static DataTable GetAllCountries()
        {
            return clsCountryData.GetAllCountries();

        }

        public static List<clsCountry> GetAllCountriesList()
        {
            // إذا كانت القائمة فارغة (أول مرة يطلبها البرنامج)، اذهب واجلبها من قاعدة البيانات
            if (_CountriesCache.Count == 0)
            {
                // نطلب الـ DataTable من طبقة الـ DataAccess
                DataTable dt = clsCountryData.GetAllCountries();

                // تحويل الأسطر (DataRows) إلى كائنات (Objects) وإضافتها للـ Cache في الذاكرة
                foreach ( DataRow row in dt.Rows)
                {
                    _CountriesCache.Add(new clsCountry(
                        Convert.ToInt32(row["CountryID"]),
                        row["CountryName"].ToString()
                    ));
                }
            }

            // في المرات القادمة، سيرجع القائمة مباشرة من الـ RAM دون لمس السيرفر
            return _CountriesCache;
        }

    }
}
