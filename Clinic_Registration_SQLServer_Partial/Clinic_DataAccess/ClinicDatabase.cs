using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Clinic_DataAccess
{
   public static partial class ClinicDatabase
    {
        public const string DatabaseName = "clinic_management";

        public static string GetConnectionString()
        {
            string cc = Environment.GetEnvironmentVariable("clinic_connection", EnvironmentVariableTarget.User);

            string connectionString = string.IsNullOrEmpty(cc) ? "" : CryptoHelper.Decrypt(cc);
            var builder = new MySqlConnectionStringBuilder(connectionString);
            return builder.ToString();
        }

        private static readonly string ServerConnection = GetConnectionString();

        private static string ConnectionString => GetConnectionString() + ";Database=" + DatabaseName + ";";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // Create database and all initial tables.
        public static void InitializeDatabase()
        {
            using (var connection = new MySqlConnection(ServerConnection))
            {
                connection.Open();

                using (var command = new MySqlCommand(
                    @"CREATE DATABASE IF NOT EXISTS `clinic_management`
                      CHARACTER SET utf8mb4
                      COLLATE utf8mb4_unicode_ci;", connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            string[] tables =
            {
                @"CREATE TABLE IF NOT EXISTS tbl_patient (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    patient_code VARCHAR(50) NOT NULL UNIQUE,
                    full_name VARCHAR(200) NOT NULL,
                    gender VARCHAR(20),
                    dob DATE,
                    mobile VARCHAR(30),
                    email VARCHAR(150),
                    address VARCHAR(500),
                    blood_group VARCHAR(10),
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                ) ENGINE=InnoDB;",
                // ... (rest unchanged)
            };

            using (var connection = GetConnection())
            {
                connection.Open();

                foreach (string sql in tables)
                {
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // INSERT / UPDATE / DELETE
        public static int ExecuteNonQuery(
            string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        // SELECT: Return DataTable
        public static DataTable ExecuteDataTable(
            string query, params MySqlParameter[] parameters)
        {
            var table = new DataTable();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                connection.Open();

                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        // SELECT: Return one value
        public static object ExecuteScalar(
            string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                connection.Open();
                return command.ExecuteScalar();
            }
        }
        public static DataRow FindApprovedRegistration(
      string username,
      string passwordHash)
        {
            DataTable dt = ExecuteDataTable(@"
        SELECT
            RegistrationID,
            FullName,
            Username,
            PasswordHash,
            IsActive,
            ApprovalStatus
        FROM tbl_registration_account
        WHERE Username = @Username
          AND PasswordHash = @PasswordHash
          AND IsActive = 1
          AND ApprovalStatus = 'Approved'
        LIMIT 1;",
                new MySqlParameter("@Username", username),
                new MySqlParameter("@PasswordHash", passwordHash));

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
        // Check connection
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
public static class CryptoHelper
{
    // Simple fixed key and IV. Ensure these are 16 bytes for AES-128.
    private static readonly string key = "1234567890123456";  // 16 bytes key for AES-128
    private static readonly string iv = "1234567890123456";   // 16 bytes IV

    // Encrypt method
    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
                swEncrypt.Flush();
                csEncrypt.FlushFinalBlock();
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    // Decrypt method
    public static string Decrypt(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);  // Convert Base64 to byte array

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);  // Ensure key is 16 bytes
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);    // Ensure IV is 16 bytes

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();  // Return the decrypted string
            }
        }
    }
}
public static class Utilities
    {
    public static string MachineName = "";

    public static string FormatDecimal(object value)
    {
        if (value != DBNull.Value && value != null)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return "";
            }
            else
            {
                return Convert.ToDecimal(value).ToString(DecimalFormatDigit());
            }
        }
        return "0.00";
    }
    public static string DecimalFormatDigit()
    {
        return DecimalPlace == 1 ? "F1" : DecimalPlace == 2 ? "F2" : DecimalPlace == 3 ? "F3" : DecimalPlace == 4 ? "F4" : "F2";
    }
    public static int? GetVoucherIdFromCode(string code)
    {
        var match = Regex.Match(code, @"(\d+)$");
        if (match.Success && int.TryParse(match.Value, out int id))
            return id;
        return null;
    }
    public static int DecimalPlace = 2;
    public static string LastLogError()
    {
        throw new NotImplementedException();
    }

    public static decimal ParseDecimalValue(object cellValue)
    {
        if (cellValue == null)
            return 0;

        string rawValue = cellValue.ToString().Trim();
        string cleanedValue = Regex.Replace(rawValue, @"[^\d.]", "");
        int dotIndex = cleanedValue.IndexOf('.');
        if (dotIndex >= 0)
        {
            cleanedValue = cleanedValue.Substring(0, dotIndex + 1) +
                           cleanedValue.Substring(dotIndex + 1).Replace(".", "");
        }
        decimal costPrice = 0;
        decimal.TryParse(cleanedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out costPrice);

        return Math.Round(costPrice, 2);
    }

    public static string GeneralSettingsState(string name)
    {
        DataRow[] rows = BindDataTable.tableGeneralSettings.Select($"name = '" + name + "'");
        if (rows.Length > 0)
        {
            return rows[0]["status"].ToString();
        }
        return "";
    }
    public static string GeneralSettings(string name)
    {
        DataRow[] rows = BindDataTable.tableGeneralSettings.Select($"name = '" + name + "'");
        if (rows.Length > 0)
        {
            return rows[0]["value"].ToString();
        }
        return "";
    }

    public static bool AreDefaultAccountsSet(List<string> accountCategories)
    {
        string categories = string.Join(",", accountCategories.ConvertAll(c => $"'{c}'"));

        string query = $"SELECT COUNT(*) FROM tbl_coa_config WHERE category IN ({categories})";
        using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query))
            if (reader.Read() && Convert.ToInt32(reader[0]) == accountCategories.Count)
                return true;

        return false;
    }

    public static void LogAudit(object userId, string v1, string v2, int v3, string v4)
    {
        string message = "insert system log";
    }

    //AreDefaultAccountsSet
}
public static class CommonInsert
{
    public static void InsertItemTransactionWithTransaction(
    MySqlConnection connection,
    MySqlTransaction transaction,
    DateTime date,
    string type,
    string reference,
    string itemId,
    string costPrice,
    string qtyIn,
    string salesPrice,
    string qtyOut,
    string qtyInc,
    string description,
    string warehouseId,
    string batchId,
    string unitId,
    string factor,
    string qtyDisplay)
    {
        using (MySqlCommand cmd = new MySqlCommand(@"
        INSERT INTO tbl_item_transaction
        (
            date, type, reference, item_id, cost_price,
            qty_in, sales_price, qty_out, qty_inc, description,
            warehouse_id, unit_id, factor, qty_display, batch_id
        )
        VALUES
        (
            @date, @type, @reference, @itemId, @costPrice,
            @qtyIn, @salesPrice, @qtyOut, @qtyInc, @description,
            @warehouseId, @unitId, @factor, @qtyDisplay, @batchId
        );", connection, transaction))
        {
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@reference", reference);
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.Parameters.AddWithValue("@costPrice", costPrice);
            cmd.Parameters.AddWithValue("@qtyIn", qtyIn);
            cmd.Parameters.AddWithValue("@salesPrice", salesPrice);
            cmd.Parameters.AddWithValue("@qtyOut", qtyOut);
            cmd.Parameters.AddWithValue("@qtyInc", qtyInc);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@warehouseId", warehouseId);
            cmd.Parameters.AddWithValue("@unitId", unitId);
            cmd.Parameters.AddWithValue("@factor", factor);
            cmd.Parameters.AddWithValue("@qtyDisplay", qtyDisplay);
            cmd.Parameters.AddWithValue("@batchId", batchId);

            cmd.ExecuteNonQuery();
        }

        // Update stock using the same connection and transaction.
        using (MySqlCommand cmd = new MySqlCommand(@"
        UPDATE tbl_items
        SET on_hand = (
            SELECT COALESCE(SUM(qty_in - qty_out), 0)
            FROM tbl_item_transaction
            WHERE item_id = @itemId
        )
        WHERE id = @itemId;", connection, transaction))
        {
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.ExecuteNonQuery();
        }

        // Insert inventory card details in the same transaction.
        InsertItemCardDetailsWithTransaction(
            connection,
            transaction,
            date, type, reference, itemId,
            costPrice, qtyIn, salesPrice, qtyOut,
            qtyInc, description, warehouseId);
    }
    private static void InsertItemCardDetailsWithTransaction(
    MySqlConnection connection,
    MySqlTransaction transaction,
    DateTime date,
    string type,
    string reference,
    string itemId,
    string costPrice,
    string qtyIn,
    string salesPrice,
    string qtyOut,
    string qtyInc,
    string description,
    string warehouseId)
    {
        string invoiceNo = "INV-" + reference;

        decimal price = Convert.ToDecimal(costPrice);
        decimal inQty = Convert.ToDecimal(qtyIn);
        decimal outQty = Convert.ToDecimal(qtyOut);

        decimal debit = inQty * price;
        decimal credit = outQty * price;

        decimal previousQtyBalance;
        decimal previousBalance;

        using (MySqlCommand cmd = new MySqlCommand(@"
        SELECT
            COALESCE(SUM(qty_in - qty_out), 0),
            COALESCE(SUM(debit - credit), 0)
        FROM tbl_item_card_details
        WHERE itemId = @itemId;", connection, transaction))
        {
            cmd.Parameters.AddWithValue("@itemId", itemId);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                previousQtyBalance = Convert.ToDecimal(reader[0]);
                previousBalance = Convert.ToDecimal(reader[1]);
            }
        }

        decimal qtyBalance = previousQtyBalance + inQty - outQty;
        decimal balance = previousBalance + debit - credit;

        using (MySqlCommand cmd = new MySqlCommand(@"
        INSERT INTO tbl_item_card_details
        (
            itemId, date, wharehouse_id, inv_no, trans_no,
            trans_type, description, price, qty_in, qty_out,
            qty_balance, debit, credit, balance, fifo_qty, fifo_cost
        )
        VALUES
        (
            @itemId, @date, @warehouseId, @invoiceNo, @reference,
            @type, @description, @price, @qtyIn, @qtyOut,
            @qtyBalance, @debit, @credit, @balance, 0, 0
        );", connection, transaction))
        {
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@warehouseId", warehouseId);
            cmd.Parameters.AddWithValue("@invoiceNo", invoiceNo);
            cmd.Parameters.AddWithValue("@reference", reference);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@qtyIn", inQty);
            cmd.Parameters.AddWithValue("@qtyOut", outQty);
            cmd.Parameters.AddWithValue("@qtyBalance", qtyBalance);
            cmd.Parameters.AddWithValue("@debit", debit);
            cmd.Parameters.AddWithValue("@credit", credit);
            cmd.Parameters.AddWithValue("@balance", balance);

            cmd.ExecuteNonQuery();
        }
    }
    public static void InsertItemTransaction(DateTime date, string type, string reference, string itemId,
                                  string costPrice, string qtyIn, string salesPrice, string qtyOut, string qtyInc,
                                  string description, string warehouseId, string batchId, string unitId, string factor, string qtyDisplay)
    {
        ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_item_transaction 
                    (date, type, reference, item_id, cost_price, qty_in,sales_price, qty_out,qty_inc, description, warehouse_id, unit_id, factor, qty_display, batch_id) 
                    VALUES (@date, @type, @reference, @itemId, @costPrice, @qtyIn,@sales_price, @qtyOut,@qtyInc, @description, @warehouseId, @unit_id, @factor, @qty_display, @batchId);",
                                ClinicDatabase.CreateParameterNew("@date", date),
                                ClinicDatabase.CreateParameterNew("@type", type),
                                ClinicDatabase.CreateParameterNew("@reference", reference),
                                ClinicDatabase.CreateParameterNew("@itemId", itemId),
                                ClinicDatabase.CreateParameterNew("@costPrice", costPrice),
                                ClinicDatabase.CreateParameterNew("@sales_price", salesPrice),
                                ClinicDatabase.CreateParameterNew("@qtyIn", qtyIn),
                                ClinicDatabase.CreateParameterNew("@qtyOut", qtyOut),
                                ClinicDatabase.CreateParameterNew("@qtyInc", qtyInc),
                                ClinicDatabase.CreateParameterNew("@description", description),
                                ClinicDatabase.CreateParameterNew("unit_id", unitId),
                                ClinicDatabase.CreateParameterNew("factor", factor),
                                ClinicDatabase.CreateParameterNew("qty_display", qtyDisplay),
                                ClinicDatabase.CreateParameterNew("batchId", batchId),
                                ClinicDatabase.CreateParameterNew("warehouseId", warehouseId));

        UpdateOnHandItem(itemId);
        AddItemCardDetails(date, type, reference, itemId, costPrice, qtyIn, salesPrice, qtyOut, qtyInc, description, warehouseId);
    }
    private static void AddItemCardDetails(DateTime date, string type, string reference, string itemId,
                              string costPrice, string qtyIn, string salesPrice, string qtyOut, string qtyInc,
                              string description, string warehouseId)
    {
        string invoiceNo = "INV-" + reference, transNo = reference, transType = type;
        decimal qtyBalance = 0, debit = 0, credit = 0, price = decimal.Parse(costPrice), balance = 0, fifoQty = 0, fifoCost = 0;
        decimal _qtyIn = 0, _qtyOut = 0;
        if (!string.IsNullOrEmpty(qtyIn) && decimal.Parse(qtyIn) > 0)
        {
            debit = decimal.Parse(qtyIn) * decimal.Parse(costPrice);
            _qtyIn = decimal.Parse(qtyIn);
        }
        if (!string.IsNullOrEmpty(qtyOut) && decimal.Parse(qtyOut) > 0)
        {
            credit = decimal.Parse(qtyOut) * decimal.Parse(costPrice);
            _qtyOut = decimal.Parse(qtyOut);
        }
        string query = "SELECT * FROM tbl_items WHERE id = @itemId";

        using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameter("itemId", itemId)))
        {
            object netResult = ClinicDatabase.ExecuteScalar(
                        "SELECT IFNULL(SUM(qty_in-qty_out),0) as QtyBalance FROM tbl_item_card_details WHERE itemId = @id",
                        ClinicDatabase.CreateParameter("id", itemId)
                    );
            decimal _QtyBalance = netResult != DBNull.Value ? Convert.ToDecimal(netResult) : 0;
            qtyBalance = _QtyBalance + (_qtyIn - _qtyOut);
            netResult = ClinicDatabase.ExecuteScalar(
                        "SELECT IFNULL(SUM(debit-credit),0) as Balance FROM tbl_item_card_details WHERE itemId = @id",
                        ClinicDatabase.CreateParameter("id", itemId)
                    );
            decimal _Balance = netResult != DBNull.Value ? Convert.ToDecimal(netResult) : 0;
            balance = _Balance + (debit - credit);

            ClinicDatabase.ExecuteNonQuery(@"
                            INSERT INTO tbl_item_card_details (
                                itemId, date, wharehouse_id, inv_no, trans_no, trans_type, description,
                                price, qty_in, qty_out, qty_balance, debit, credit, balance, fifo_qty, fifo_cost
                            ) VALUES (
                                @itemId, @date, @wharehouse_id, @inv_no, @trans_no, @trans_type, @description,
                                @price, @qty_in, @qty_out, @qty_balance, @debit, @credit, @balance, @fifo_qty, @fifo_cost
                            );",
                            ClinicDatabase.CreateParameterNew("itemId", itemId),
                            ClinicDatabase.CreateParameterNew("date", date),
                            ClinicDatabase.CreateParameterNew("wharehouse_id", warehouseId),
                            ClinicDatabase.CreateParameterNew("inv_no", invoiceNo),
                            ClinicDatabase.CreateParameterNew("trans_no", transNo),
                            ClinicDatabase.CreateParameterNew("trans_type", transType),
                            ClinicDatabase.CreateParameterNew("description", description),
                            ClinicDatabase.CreateParameterNew("price", price),
                            ClinicDatabase.CreateParameterNew("qty_in", qtyIn),
                            ClinicDatabase.CreateParameterNew("qty_out", qtyOut),
                            ClinicDatabase.CreateParameterNew("qty_balance", qtyBalance),
                            ClinicDatabase.CreateParameterNew("debit", debit),
                            ClinicDatabase.CreateParameterNew("credit", credit),
                            ClinicDatabase.CreateParameterNew("balance", balance),
                            ClinicDatabase.CreateParameterNew("fifo_qty", fifoQty),
                            ClinicDatabase.CreateParameterNew("fifo_cost", fifoCost)
            );
        }
    }
    public static void UpdateOnHandItem(string itemId)
    {
        ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_items SET on_hand = (SELECT SUM(qty_in - qty_out) FROM tbl_item_transaction WHERE item_id = @itemId) where id = @itemId",
                                ClinicDatabase.CreateParameter("@itemId", itemId));
    }
    public static void DeleteItemTransaction(string type, string reference)
    {
        if (type == "Sales")
        {
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_transaction WHERE `type` = 'Sales Invoice' AND `REFERENCE` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_card_details WHERE `trans_type` = 'Sales Invoice' AND `trans_no` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_items i
                                            INNER JOIN tbl_sales_details sd ON i.id = sd.item_id
                                            SET i.on_hand = i.on_hand + sd.qty
                                            WHERE sd.sales_id = @id;", ClinicDatabase.CreateParameter("id", reference));
        }
        else if (type == "Sales return")
        {
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_transaction WHERE `type` = 'Sales Return Invoice' AND `REFERENCE` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_card_details WHERE `trans_type` = 'Sales Return Invoice' AND `trans_no` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_items i
                                            INNER JOIN tbl_sales_details sd ON i.id = sd.item_id
                                            SET i.on_hand = i.on_hand - sd.qty
                                            WHERE sd.sales_id = @id;", ClinicDatabase.CreateParameter("id", reference));
        }
        else if (type == "Purchase")
        {
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_transaction WHERE `type` = 'Purchase Invoice' AND `REFERENCE` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_card_details WHERE `trans_type` = 'Purchase Invoice' AND `trans_no` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_items i
                                            INNER JOIN tbl_sales_details sd ON i.id = sd.item_id
                                            SET i.on_hand = i.on_hand - sd.qty
                                            WHERE sd.sales_id = @id;", ClinicDatabase.CreateParameter("id", reference));
        }
        else if (type == "Purchase return")
        {
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_transaction WHERE `type` = 'Purchase Return Invoice' AND `REFERENCE` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_card_details WHERE `trans_type` = 'Purchase Return Invoice' AND `trans_no` = @id", ClinicDatabase.CreateParameter("id", reference));
            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_items i
                                            INNER JOIN tbl_sales_details sd ON i.id = sd.item_id
                                            SET i.on_hand = i.on_hand + sd.qty
                                            WHERE sd.sales_id = @id;", ClinicDatabase.CreateParameter("id", reference));
        }
    }
    public static void InsertTransactionEntryWithTransaction(
    MySqlConnection connection,
    MySqlTransaction transaction,
    DateTime date,
    string accountId,
    string debit,
    string credit,
    string transactionId,
    string humId,
    string type,
    string description,
    int createdBy,
    DateTime createdDate)
    {
        using (MySqlCommand cmd = new MySqlCommand(@"
        INSERT INTO tbl_transaction
        (
            date,
            account_id,
            debit,
            credit,
            transaction_id,
            hum_id,
            t_type,
            type,
            description,
            created_by,
            created_date,
            state,
            GlobalId,
            DeviceId
        )
        VALUES
        (
            @date,
            @accountId,
            @debit,
            @credit,
            @transactionId,
            @hum_id,
            @tType,
            @type,
            @description,
            @createdBy,
            @createdDate,
            0,
            @GlobalId,
            @DeviceId
        );", connection, transaction))
        {
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@accountId", accountId);
            cmd.Parameters.AddWithValue("@debit", debit);
            cmd.Parameters.AddWithValue("@credit", credit);
            cmd.Parameters.AddWithValue("@transactionId", transactionId);
            cmd.Parameters.AddWithValue("@hum_id", humId);
            cmd.Parameters.AddWithValue("@tType", "");
            cmd.Parameters.AddWithValue("@type", type.Trim());
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@createdBy", createdBy);
            cmd.Parameters.AddWithValue("@createdDate", createdDate);
            cmd.Parameters.AddWithValue("@GlobalId", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@DeviceId", Utilities.MachineName);

            cmd.ExecuteNonQuery();
        }
    }
    public static void InsertTransactionEntry(DateTime date, string accountId, string debit, string credit,
                           string transactionId, string humId, string type, string description, int createdBy, DateTime createdDate)
    {
        ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_transaction 
                    (date, account_id, debit, credit, transaction_id,hum_id,t_type,type, description, created_by, created_date, state, GlobalId, DeviceId) 
                    VALUES (@date, @accountId, @debit, @credit, @transactionId,@hum_id,@tType,@type, @description, @createdBy, @createdDate, 0, @GlobalId, @DeviceId);",
        ClinicDatabase.CreateParameterNew("@date", date),
        ClinicDatabase.CreateParameterNew("@accountId", accountId),
        ClinicDatabase.CreateParameterNew("@debit", debit),
        ClinicDatabase.CreateParameterNew("@credit", credit),
        ClinicDatabase.CreateParameterNew("@transactionId", transactionId),
        ClinicDatabase.CreateParameterNew("@type", type.Trim()),
        ClinicDatabase.CreateParameterNew("@tType", ""),
        ClinicDatabase.CreateParameterNew("@hum_id", humId),
        ClinicDatabase.CreateParameterNew("@description", description),
        ClinicDatabase.CreateParameterNew("@createdBy", createdBy),
        ClinicDatabase.CreateParameterNew("@createdDate", createdDate),
            ClinicDatabase.CreateParameterNew("GlobalId", Guid.NewGuid().ToString()),
            ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName));
    }
    public static void addTransactionEntry(DateTime date, string accountId, string debit, string credit,
                           string transactionId, string humId, string type, string voucher_name, string description, int createdBy, DateTime createdDate, string VoucherNo, int projectId = 0)
    {
        ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_transaction 
                    (date, account_id, debit, credit, transaction_id,hum_id,t_type,type, description, created_by, created_date, state,voucher_no,project_id, GlobalId, DeviceId) 
                    VALUES (@date, @accountId, @debit, @credit, @transactionId,@hum_id,@tType,@type, @description, @createdBy, @createdDate, 0,@voucher_no,@projectId, @GlobalId, @DeviceId);",
        ClinicDatabase.CreateParameterNew("@date", date),
        ClinicDatabase.CreateParameterNew("@accountId", accountId),
        ClinicDatabase.CreateParameterNew("@debit", debit),
        ClinicDatabase.CreateParameterNew("@credit", credit),
        ClinicDatabase.CreateParameterNew("@transactionId", transactionId),
        ClinicDatabase.CreateParameterNew("@type", type),
        ClinicDatabase.CreateParameterNew("@tType", voucher_name),
        ClinicDatabase.CreateParameterNew("@hum_id", humId),
        ClinicDatabase.CreateParameterNew("@description", description),
        ClinicDatabase.CreateParameterNew("@createdBy", createdBy),
        ClinicDatabase.CreateParameterNew("@createdDate", createdDate),
        ClinicDatabase.CreateParameterNew("@projectId", projectId),
        ClinicDatabase.CreateParameterNew("@voucher_no", VoucherNo),
            ClinicDatabase.CreateParameterNew("GlobalId", Guid.NewGuid().ToString()),
            ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName));
    }
    public static void UpdateTransactionEntry(int journalId, DateTime date, string accountId, string debit, string credit,
                                  string transactionId, string humId, string type, string tType, string description, int modifiedBy, DateTime modifiedDate)
    {
        ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_transaction 
                            SET date = @date, 
                                account_id = @accountId, 
                                debit = @debit, 
                                credit = @credit, 
                                transaction_id = @transactionId, 
                                hum_id =@hum_id,
                                type = @type,
                                t_type = @tType,
                                description = @description, 
                                modified_by = @modifiedBy, 
                                modified_date = @modifiedDate
                            WHERE id = @journalId;",
            ClinicDatabase.CreateParameterNew("@date", date),
            ClinicDatabase.CreateParameterNew("@accountId", accountId),
            ClinicDatabase.CreateParameterNew("@debit", debit),
            ClinicDatabase.CreateParameterNew("@credit", credit),
            ClinicDatabase.CreateParameterNew("@transactionId", transactionId),
            ClinicDatabase.CreateParameterNew("@hum_id", humId),
            ClinicDatabase.CreateParameterNew("@type", type),
            ClinicDatabase.CreateParameterNew("@tType", tType),
            ClinicDatabase.CreateParameterNew("@description", description),
            ClinicDatabase.CreateParameterNew("@modifiedBy", modifiedBy),
            ClinicDatabase.CreateParameterNew("@modifiedDate", modifiedDate),
            ClinicDatabase.CreateParameterNew("@journalId", journalId)
        );
    }
    public static void DeleteTransactionEntry(int id, string type)
    {
        ClinicDatabase.ExecuteNonQuery(@"DELETE FROM tbl_transaction Where t_type= @tType and transaction_id=@id;",
        ClinicDatabase.CreateParameterNew("@id", id),
        ClinicDatabase.CreateParameterNew("@tType", type));
    }
    public static void InsertCostCenterTransaction(DateTime date, string debit, string credit, string refId, string type, string description, string cost_center_id)
    {
        ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_cost_center_transaction (type,date,ref_id,debit,credit,description,cost_center_id) 
                                VALUES (@type,@date,@ref,@debit,@credit,@description,@cost_center_id);",
            ClinicDatabase.CreateParameterNew("@date", date),
            ClinicDatabase.CreateParameterNew("@type", type),
            ClinicDatabase.CreateParameterNew("@debit", debit),
            ClinicDatabase.CreateParameterNew("@credit", credit),
            ClinicDatabase.CreateParameterNew("@ref", refId),
            ClinicDatabase.CreateParameterNew("@description", description),
            ClinicDatabase.CreateParameterNew("@cost_center_id", cost_center_id));
    }
    public static void DeleteCostCenterTransactionEntry(string refId, string type)
    {
        ClinicDatabase.ExecuteNonQuery(@"DELETE FROM tbl_cost_center_transaction Where type= @type and ref_id=@id;",
        ClinicDatabase.CreateParameterNew("@id", refId),
        ClinicDatabase.CreateParameterNew("@type", type));
    }
    public static string GetFormName(string transactionType)
    {
        string type = transactionType.ToLower();

        if (type.Equals("sales invoice") || type.Equals("sales invoice cash"))
        {
            return "Sales";
        }
        else if (type.Equals("sales return invoice") || type.Equals("sales return invoice cash"))
        {
            return "Sales Return";
        }
        else if (type.Equals("purchase invoice") || type.Equals("purchase invoice cash"))
        {
            return "Purchase";
        }
        else if (type.Equals("purchase return invoice") || type.Equals("purchase return invoice cash"))
        {
            return "Purchase Return";
        }
        else if (type.Equals("vendor payment"))
        {
            return "Payment";
        }
        else if (type.Equals("customer receipt"))
        {
            return "Receipt";
        }
        else if (type.Equals("employee salary"))
        {
            return "Employee Salary";
        }
        else if (type.Equals("petty cash request"))
        {
            return "Petty Cash Request";
        }
        else if (type.Equals("petty cash submission"))
        {
            return "Petty Cash Submission";
        }
        else if (type.Equals("inventory opening stock"))
        {
            return "Inventory";
        }
        else if (type.Equals("prepaid expense"))
        {
            return "Prepaid Expense";
        }
        else if (type.Equals("fixed assets"))
        {
            return "Fixed Assets";
        }
        else if (type.Equals("pdc payable"))
        {
            return "PDC Payable";
        }
        else if (type.Equals("pdc receivable"))
        {
            return "PDC Receivable";
        }
        else if (type.Equals("loan request"))
        {
            return "Loan Request";
        }
        else if (type.Equals("customer advance payment"))
        {
            return "Customer Advance Payment";
        }
        else if (type.Equals("rms bill"))
        {
            return "RMS Bill";
        }
        else
        {
            return transactionType; // If no match
        }

        /*
         * Customer Opening Balance",
                "Vendor Opening Balance",
                "Sales Invoice",
                "Purchase Invoice",
                "Customer Receipt",
                "Vendor Payment",
                "Employee Salary",
                "Petty Cash Request",
                "Petty Cash Submission",
                "Purchase Return Invoice",
                "Employee Petty Cash Payment",
                "Inventory Opening Stock",
                "Prepaid Expense",
                "Fixed Assets",
                "PDC Payable",
                "PDC Receivable",
                "Employee Salary Payment",
                "Loan Request",
                "Vendor Advance Payment",
                "Customer Advance Payment",
                "RMS Bill
         */
    }
}
