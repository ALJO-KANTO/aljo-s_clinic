using Clinic_DataAccess.SaveMySqlException;
using MySql.Data.MySqlClient;
using System;
using System.Data;
namespace Clinic_DataAccess
{
    public class clsMedicineData
    {
        // 1. جلب بيانات دواء واحد عبر الـ ID
        public static bool GetMedicineInfoByID(int MedicineID, ref string MedicineName, ref decimal MedicinePrice, ref decimal MedicineCost, ref decimal CurrentStock, ref decimal ReorderLevel, 
            ref decimal TaxRate, ref string BatchNo, ref DateTime ExpiryDate, ref int WarehouseId, ref int incomeAccount, ref int assetAccount, ref int COGSAccount)
        {
            bool isFound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                                SELECT
                                    m.MedicineID,
                                    m.MedicineName,
                                    m.MedicinePrice,
                                    m.MedicineCost,
                                    m.CurrentStock,
                                    m.ReorderLevel,
                                    m.TaxRate,
                                    b.batch_no AS BatchNo,
                                    b.expiry_date AS ExpiryDate,
                                    b.warehouse_id AS WarehouseId
                                FROM Medicines m
                                LEFT JOIN tbl_item_batches b
                                    ON b.item_id = m.MedicineID
                                WHERE m.MedicineID = @MedicineID
                                ORDER BY b.id DESC
                                LIMIT 1;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MedicineID", MedicineID);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                MedicineName = Convert.ToString(reader["MedicineName"]);
                                MedicinePrice = Convert.ToDecimal(reader["MedicinePrice"]);
                                MedicineCost = Convert.ToDecimal(reader["MedicineCost"]);
                                CurrentStock = Convert.ToDecimal(reader["CurrentStock"]);
                                ReorderLevel = Convert.ToDecimal(reader["ReorderLevel"]);
                                TaxRate = Convert.ToDecimal(reader["TaxRate"]);

                                bool hasBatch =
                                    reader["BatchNo"] != DBNull.Value &&
                                    reader["ExpiryDate"] != DBNull.Value;

                                if (hasBatch)
                                {
                                    BatchNo = Convert.ToString(reader["BatchNo"]);
                                    ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]);
                                }
                                else
                                {
                                    BatchNo = "";
                                    ExpiryDate = DateTime.Now.AddDays(30);
                                    WarehouseId = 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
                isFound = false;
            }
            return isFound;
        }

        // 3. (إضافي) للتحقق من وجود الدواء بالاسم قبل إضافته
        public static bool IsMedicineExists(string MedicineName)
        {
            bool isFound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT 1 FROM Medicines WHERE MedicineName = @MedicineName LIMIT 1;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MedicineName", MedicineName);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null) isFound = true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);
            }
            return isFound;
        }

        public static DataTable GetAllMedicines()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT *
                                FROM Medicines;";



                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return dt;
        }

        public static bool IsMedicineAvailable(int medicineId, int requestedQuantity)
        {
            string query = "SELECT CurrentStock FROM Medicines WHERE MedicineID = @MedicineID";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MedicineID", medicineId);

                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int c))
                        {
                            int currentStock = c;

                            // التأكد من أن الكمية في المخزن تغطي الكمية المطلوبة
                            if (currentStock >= requestedQuantity)
                            {
                                return true; // الدواء متوفر والكمية كافية
                            }

                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error,
                    -1);
            }
            return false;

        }

        public static int AddNewMedicine(
            string medicineName,
            decimal medicinePrice,
            decimal MedicineCost,
            decimal currentStock,
            decimal reorderLevel,
            decimal taxRate,
            string batchNo,
            DateTime expiryDate,
            int warehouseId,
            int incomeAccount,
            int assetAccount,
            int COGSAccount)
        {
            int medicineID;

            try
            {
                

                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();

                    using (MySqlTransaction transaction =
                        connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Insert medicine into tbl_items
                            string itemSql = @"
                                        INSERT INTO tbl_items
                                        (
                                            code,
                                            warehouse_id,
                                            type,
                                            name,
                                            sales_price,
                                            cost_price,
                                            on_hand,
                                            cogs_account_id,
                                            tax_code_id,
	                                        income_account_id,
	                                        asset_account_id,
	                                        date,
	                                        method,
	                                        created_By,
	                                        created_date,
	                                        category_id
                                        )
                                        VALUES
                                        (
                                            @code,
                                            @warehouseId,
                                            '11 - Inventory Part',
                                            @name,
                                            @salesPrice,
                                            @costPrice,
                                            @openingQty,
                                            @cogs_account_id,
                                            @tax_code_id,
	                                        @income_account_id,
	                                        @asset_account_id,
	                                        NOW(),
	                                        'AVG',
	                                        1,
	                                        NOW(),
	                                        0
                                        );

                                        SELECT LAST_INSERT_ID();";


                            using (MySqlCommand cmd =
                                new MySqlCommand(itemSql, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@code", GenerateMedicineCode());
                                cmd.Parameters.AddWithValue("@warehouseId", warehouseId);
                                cmd.Parameters.AddWithValue("@name", medicineName);
                                cmd.Parameters.AddWithValue("@salesPrice", medicinePrice);
                                cmd.Parameters.AddWithValue("@costPrice", MedicineCost);
                                cmd.Parameters.AddWithValue("@openingQty", currentStock);
                                cmd.Parameters.AddWithValue("@cogs_account_id", COGSAccount);
                                cmd.Parameters.AddWithValue("@tax_code_id", Convert.ToInt32(taxRate));
                                cmd.Parameters.AddWithValue("@income_account_id", incomeAccount);
                                cmd.Parameters.AddWithValue("@asset_account_id", assetAccount);

                                medicineID = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            int batchId = 0;
                            // 2. Insert opening stock batch if quantity is greater than zero
                            if (currentStock > 0)
                            {
                                string batchSql = @"
                                        INSERT INTO tbl_item_batches
                                        (
                                            item_id,
                                            batch_no,
                                            expiry_date,
                                            cost_price,
                                            sale_price,
                                            warehouse_id,
                                            qty_in,
                                            qty_out,
                                            base_cost,
                                            factor,
                                            created_date
                                        )
                                        VALUES
                                        (
                                            @itemId,
                                            @batchNo,
                                            @expiryDate,
                                            @costPrice,
                                            @salePrice,
                                            @warehouseId,
                                            @qty,
                                            0,
                                            @costPrice,
                                            1,
                                            NOW()
                                        );

                                        SELECT LAST_INSERT_ID();";

                                using (MySqlCommand cmd =
                                    new MySqlCommand(batchSql, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@itemId", medicineID);
                                    cmd.Parameters.AddWithValue("@batchNo", batchNo);
                                    cmd.Parameters.AddWithValue("@expiryDate", expiryDate.Date);
                                    cmd.Parameters.AddWithValue("@costPrice", MedicineCost);
                                    cmd.Parameters.AddWithValue("@salePrice", medicinePrice);
                                    cmd.Parameters.AddWithValue("@warehouseId", warehouseId);
                                    cmd.Parameters.AddWithValue("@qty", currentStock);

                                    batchId = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                if (!IsUsedInTransactions(medicineID))
                                {
                                    decimal txtTotalValue = currentStock * MedicineCost;
                                    DateTime dtStart = DateTime.Now.Date;
                                    int inventoryId = SelectDefaultLevelAccount("Inventory");
                                    int openingBalanceEquityId = SelectDefaultLevelAccount("Opening Balance Equity");
                                    
                                    CommonInsert.InsertItemTransactionWithTransaction(
                                        connection,
                                        transaction,
                                        dtStart,
                                        "Opening Qty",
                                        "0",
                                        medicineID.ToString(),
                                        MedicineCost.ToString(),
                                        currentStock.ToString(),
                                        "0",
                                        "0",
                                        currentStock.ToString(),
                                        "Opening Balance",
                                        warehouseId.ToString(),
                                        batchId.ToString(),
                                        "0",
                                        "1",
                                        currentStock.ToString()
                                    );
                                    if (txtTotalValue > 0)
                                    {
                                        // Debit Inventory
                                        CommonInsert.InsertTransactionEntryWithTransaction(
                                            connection,
                                            transaction,
                                            dtStart,
                                            inventoryId.ToString(),
                                            txtTotalValue.ToString(),
                                            "0",
                                            medicineID.ToString(),
                                            medicineID.ToString(),
                                            "Opening Qty",
                                            "Opening Balance - Medicine ID - " + medicineID,
                                            -1,
                                            dtStart
                                        );

                                        // Credit Opening Balance Equity
                                        CommonInsert.InsertTransactionEntryWithTransaction(
                                            connection,
                                            transaction,
                                            dtStart,
                                            openingBalanceEquityId.ToString(),
                                            "0",
                                            txtTotalValue.ToString(),
                                            medicineID.ToString(),
                                            "0",
                                            "Opening Qty",
                                            "Opening Balance Equity - Medicine ID - " + medicineID,
                                            -1,
                                            dtStart
                                        );
                                    }
                                }
                            
                            }

                            transaction.Commit();
                            return medicineID;
                        }
                        catch
                        {
                            transaction.Rollback();
                            medicineID = 0;
                            throw;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, -1);

                medicineID = -1;
                return medicineID;
            }
        }

        public static int SelectDefaultLevelAccount(string type)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT category, account_id FROM tbl_coa_config WHERE category =@type",
                    ClinicDatabase.CreateParameterNew("type", type)))
                if (reader.Read())
                    return int.Parse(reader["account_id"].ToString());
                else return 0;
        }

        private static string GenerateMedicineCode()
        {
            string typeCategory = "";
            string query = "SELECT MAX(RIGHT(code, 4)) FROM tbl_items WHERE LEFT(code, 5) = @typeCategory";
            object result = ClinicDatabase.ExecuteScalar(query, ClinicDatabase.CreateParameterNew("@typeCategory", typeCategory));
            int nextSerial = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
            string formattedSerial = nextSerial.ToString().PadLeft(4, '0');
            string newItemCode = typeCategory + formattedSerial;
            return "MED-" + newItemCode;
        }

        public static bool UpdateMedicine(
            int MedicineID,
            string MedicineName,
            decimal MedicinePrice,
            decimal MedicineCost,
            decimal CurrentStock,
            decimal ReorderLevel,
            decimal TaxRate,
            string batchNo,
            DateTime expiryDate,
            int warehouseId,
            int incomeAccount,
            int assetAccount,
            int COGSAccount)
        {
            int rowsAffected = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE tbl_items 
                                SET Name = @MedicineName,
                                    sales_price = @MedicinePrice,
                                    cost_price = @MedicineCost,
                                    tax_code_id = @TaxRate,
                                    income_account_id = @incomeAccount,
                                    asset_account_id = @assetAccount,
                                    cogs_account_id = @COGSAccount,
                                    warehouse_id = @warehouseId

                             WHERE id = @MedicineID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MedicineID", MedicineID);
                        command.Parameters.AddWithValue("@MedicineName", MedicineName);
                        command.Parameters.AddWithValue("@MedicinePrice", MedicinePrice);
                        command.Parameters.AddWithValue("@MedicineCost", MedicineCost);
                        command.Parameters.AddWithValue("@TaxRate", TaxRate);
                        command.Parameters.AddWithValue("@incomeAccount", incomeAccount);
                        command.Parameters.AddWithValue("@assetAccount", assetAccount);
                        command.Parameters.AddWithValue("@COGSAccount", COGSAccount);
                        command.Parameters.AddWithValue("@warehouseId", warehouseId);

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, MedicineID);
                return false;
            }
            return (rowsAffected > 0);
        }

        public static bool DeleteMedicine(int MedicineID)
        {
            int rowsAffected = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"Delete From tbl_items 
                             WHERE id = @MedicineID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MedicineID", MedicineID);
                      

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, MedicineID);
                return false;
            }
            return (rowsAffected > 0);
        }

        public static bool IsUsedInTransactions(int medicineID)
        {
            string query = @"
                    SELECT EXISTS (
                        SELECT 1
                        FROM tbl_item_transaction
                        WHERE item_id = @MedicineID
                    );";

            using (MySqlConnection connection =
                   new MySqlConnection(clsDataAccessSettings.ConnectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MedicineID", medicineID);

                connection.Open();

                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }
    }
}