using Clinic_DataAccess;
using System;
using System.Data;

namespace Clinic_Business
{
    public class clsMedicine
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int MedicineID { get; set; }
        public string MedicineName { get; set; }
        public decimal MedicinePrice { get; set; }

        // Changed from int to decimal
        public decimal CurrentStock { get; set; }
        public decimal ReorderLevel { get; set; }

        public decimal TaxRate { get; set; }

        public clsMedicine()
        {
            MedicineID = -1;
            MedicineName = "";
            MedicinePrice = 0;
            CurrentStock = 0;
            ReorderLevel = 0;
            TaxRate = 15.00M;

            Mode = enMode.AddNew;
        }

        private clsMedicine(
            int MedicineID,
            string MedicineName,
            decimal MedicinePrice,
            decimal CurrentStock,
            decimal ReorderLevel,
            decimal TaxRate)
        {
            this.MedicineID = MedicineID;
            this.MedicineName = MedicineName;
            this.MedicinePrice = MedicinePrice;
            this.CurrentStock = CurrentStock;
            this.ReorderLevel = ReorderLevel;
            this.TaxRate = TaxRate;

            Mode = enMode.Update;
        }

        private bool _AddNewMedicine()
        {
            MedicineID = clsMedicineData.AddNewMedicine(
                MedicineName,
                MedicinePrice,
                CurrentStock,
                ReorderLevel,
                TaxRate);

            return MedicineID != -1;
        }

        private bool _UpdateMedicine()
        {
            return clsMedicineData.UpdateMedicine(
                MedicineID,
                MedicineName,
                MedicinePrice,
                CurrentStock,
                ReorderLevel,
                TaxRate);
        }

        public static bool DeleteMedicine(int MedicineID)
        {
            return clsMedicineData.DeleteMedicine(MedicineID);
        }

        public static clsMedicine Find(int MedicineID)
        {
            string MedicineName = "";
            decimal MedicinePrice = 0;

            // Changed from int to decimal
            decimal CurrentStock = 0;
            decimal ReorderLevel = 0;

            decimal TaxRate = 0;

            if (clsMedicineData.GetMedicineInfoByID(
                MedicineID,
                ref MedicineName,
                ref MedicinePrice,
                ref CurrentStock,
                ref ReorderLevel,
                ref TaxRate))
            {
                return new clsMedicine(
                    MedicineID,
                    MedicineName,
                    MedicinePrice,
                    CurrentStock,
                    ReorderLevel,
                    TaxRate);
            }

            return null;
        }

        public static bool IsMedicineAvailable(
            int medicineId,
            int requestedQuantity)
        {
            return clsMedicineData.IsMedicineAvailable(
                medicineId,
                requestedQuantity);
        }

        public static DataTable GetAllMedicines()
        {
            return clsMedicineData.GetAllMedicines();
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:

                    if (_AddNewMedicine())
                    {
                        Mode = enMode.Update;
                        return true;
                    }

                    return false;

                case enMode.Update:

                    return _UpdateMedicine();
            }

            return false;
        }
    }
}