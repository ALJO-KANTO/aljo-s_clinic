using Clinic_DataAccess;
using MySql.Data.MySqlClient;
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
        public decimal MedicineCost { get; set; }

        // Changed from int to decimal
        public decimal CurrentStock { get; set; }
        public decimal ReorderLevel { get; set; }

        public decimal TaxRate { get; set; }

        public string BatchNo { get; set; }

        public DateTime ExpiryDate { get; set; }
        public int WarehouseId { get; set; }
        public int IncomeAccount { get; set; }
        public int AssetAccount { get; set; }
        public int COGSAccount { get; set; }

        public clsMedicine()
        {
            MedicineID = -1;
            MedicineName = "";
            MedicinePrice = 0;
            MedicineCost = 0;
            CurrentStock = 0;
            ReorderLevel = 0;
            TaxRate = 15.00M;
            BatchNo = "";
            ExpiryDate = DateTime.Now.AddDays(30);
            WarehouseId = 1;
            IncomeAccount = 0;
            AssetAccount = 0;
            COGSAccount = 0;

            Mode = enMode.AddNew;
        }

        private clsMedicine(
            int MedicineID,
            string MedicineName,
            decimal MedicinePrice,
            decimal MedicineCost,
            decimal CurrentStock,
            decimal ReorderLevel,
            decimal TaxRate,
            string BatchNo,
            DateTime ExpiryDate,
            int WarehouseId,
            int IncomeAccount,
            int AssetAccount,
            int COGSAccount)
        {
            this.MedicineID = MedicineID;
            this.MedicineName = MedicineName;
            this.MedicinePrice = MedicinePrice;
            this.MedicineCost = MedicineCost;
            this.CurrentStock = CurrentStock;
            this.ReorderLevel = ReorderLevel;
            this.TaxRate = TaxRate;
            this.BatchNo = BatchNo;
            this.ExpiryDate = ExpiryDate;
            this.WarehouseId = WarehouseId;
            this.IncomeAccount = IncomeAccount;
            this.AssetAccount = AssetAccount;
            this.COGSAccount = COGSAccount;

            Mode = enMode.Update;
        }
        public static bool IsUsedInTransactions(int medicineID)
        {
            return clsMedicineData.IsUsedInTransactions(medicineID);
        }
        private bool _AddNewMedicine()
        {
            MedicineID = clsMedicineData.AddNewMedicine(
                MedicineName,
                MedicinePrice,
                MedicineCost,
                CurrentStock,
                ReorderLevel,
                TaxRate,
                BatchNo,
                ExpiryDate,
                WarehouseId,
                IncomeAccount,
                AssetAccount,
                COGSAccount);

            return MedicineID != -1;
        }

        private bool _UpdateMedicine()
        {
            return clsMedicineData.UpdateMedicine(
                MedicineID,
                MedicineName,
                MedicinePrice,
                MedicineCost,
                CurrentStock,
                ReorderLevel,
                TaxRate,
                BatchNo,
                ExpiryDate,
                WarehouseId,
                IncomeAccount,
                AssetAccount,
                COGSAccount);
        }

        public static bool DeleteMedicine(int MedicineID)
        {
            return clsMedicineData.DeleteMedicine(MedicineID);
        }

        public static clsMedicine Find(int MedicineID)
        {
            string MedicineName = "";
            decimal MedicinePrice = 0;
            decimal MedicineCost = 0;

            // Changed from int to decimal
            decimal CurrentStock = 0;
            decimal ReorderLevel = 0;

            decimal TaxRate = 0;
            string BatchNo = "";
            DateTime ExpiryDate = DateTime.Now.AddDays(30);
            int WarehouseId = 1;
            int IncomeAccount = 1;
            int AssetAccount = 1;
            int COGSAccount = 1;

            if (clsMedicineData.GetMedicineInfoByID(
                MedicineID,
                ref MedicineName,
                ref MedicinePrice,
                ref MedicineCost,
                ref CurrentStock,
                ref ReorderLevel,
                ref TaxRate,
                ref BatchNo,
                ref ExpiryDate,
                ref WarehouseId,
                ref IncomeAccount,
                ref AssetAccount,
                ref COGSAccount))
            {
                return new clsMedicine(
                    MedicineID,
                    MedicineName,
                    MedicinePrice,
                    MedicineCost,
                    CurrentStock,
                    ReorderLevel,
                    TaxRate,
                    BatchNo,
                    ExpiryDate,
                    WarehouseId,
                    IncomeAccount,
                    AssetAccount,
                    COGSAccount);
            }

            return null;
        }

        public static bool IsMedicineAvailable(int medicineId, int requestedQuantity)
        {
            return clsMedicineData.IsMedicineAvailable(medicineId, requestedQuantity);
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