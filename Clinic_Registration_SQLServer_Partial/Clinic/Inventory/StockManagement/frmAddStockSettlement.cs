using Clinic_DataAccess;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using YamyProject.Localization;


namespace YamyProject
{
    public partial class frmAddStockSettlement : Form
    {
        int inventoryAccount, stockSettleAccount;
        decimal invId;

        string invCode = "SIS-0001";
        int id;

        // This form uses explicit translations so it does not depend on missing resource keys.
        private bool IsArabicLanguage()
        {
            // The application setting may store "Arabic" instead of "ar".
            // Also check the active UI culture because the language switch updates it.
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? string.Empty;
            return language.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
                || language.Equals("Arabic", StringComparison.OrdinalIgnoreCase)
                || Thread.CurrentThread.CurrentUICulture.Name.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
                || Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        }

        private string TranslateSettlementText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            string key = text.Trim();
            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Stock Settlement - Edit"] = "تعديل تسوية المخزون",
                ["Stock Settlement - Edit "] = "تعديل تسوية المخزون",
                ["Stock Settlement - New"] = "تسوية مخزون جديدة",
                ["Stock Settlement - New "] = "تسوية مخزون جديدة",
                ["Stock Settlement"] = "تسوية المخزون",
                ["Warehouse"] = "المستودع",
                ["Date"] = "التاريخ",
                ["Invoice No"] = "رقم الفاتورة",
                ["Invoice ID"] = "رقم الفاتورة",
                ["Item"] = "الصنف",
                ["Item Name"] = "اسم الصنف",
                ["Name"] = "الاسم",
                ["Code"] = "الرمز",
                ["Qty"] = "الكمية",
                ["Quantity"] = "الكمية",
                ["On Hand"] = "الكمية المتوفرة",
                ["Cost Price"] = "سعر التكلفة",
                ["Total"] = "الإجمالي",
                ["New Qty"] = "الكمية الجديدة",
                ["New Quantity"] = "الكمية الجديدة",
                ["Qty Difference"] = "فرق الكمية",
                ["Quantity Difference"] = "فرق الكمية",
                ["Minus Amount"] = "مبلغ النقص",
                ["Plus Amount"] = "مبلغ الزيادة",
                ["Amount Minus"] = "مبلغ النقص",
                ["Amount Plus"] = "مبلغ الزيادة",
                ["Amount"] = "المبلغ",
                ["Save"] = "حفظ",
                ["Save & New"] = "حفظ وجديد",
                ["Save and New"] = "حفظ وجديد",
                ["Close"] = "إغلاق",
                ["Cancel"] = "إلغاء",
                ["Previous"] = "السابق",
                ["Next"] = "التالي",
                ["Search"] = "بحث",
                ["Add"] = "إضافة",
                ["Delete"] = "حذف",
                ["Remove"] = "إزالة",
                ["Info"] = "معلومات",
                ["Inventory Account Must Be Set First"] = "يجب إعداد حساب المخزون أولاً",
                ["Stock Settlement Account Must Be Set First"] = "يجب إعداد حساب تسوية المخزون أولاً",
                ["Invalid Item ID."] = "معرّف الصنف غير صالح.",
                ["Invalid Quantity."] = "الكمية غير صالحة.",
                ["Insert Items First."] = "يرجى إضافة الأصناف أولاً.",
                ["Amount Must Be Bigger Than Zero"] = "يجب أن يكون المبلغ أكبر من صفر.",
                ["No previous records."] = "لا توجد سجلات سابقة.",
                ["No next records."] = "لا توجد سجلات تالية."
            };

            if (translations.TryGetValue(key, out string translated)) return translated;
            if (key.StartsWith("Total Item In Row ", StringComparison.OrdinalIgnoreCase) && key.EndsWith(" Can't Be 0 or Null", StringComparison.OrdinalIgnoreCase))
            {
                string rowNumber = key.Substring("Total Item In Row ".Length);
                rowNumber = rowNumber.Substring(0, rowNumber.Length - " Can't Be 0 or Null".Length);
                return "إجمالي الصنف في الصف " + rowNumber + " لا يمكن أن يكون صفراً أو فارغاً";
            }
            return text;
        }

        private void ApplySettlementLocalization(Control root)
        {
            if (root == null) return;

            bool arabic = IsArabicLanguage();
            if (root is DataGridView grid)
            {
                ApplySettlementGridLocalization(grid, arabic);
                return;
            }

            // The form title is assigned after InitializeComponent, so localize it here too.
            if (root == this)
                root.Text = arabic
                    ? (id != 0 ? "تعديل تسوية المخزون" : "تسوية مخزون جديدة")
                    : (id != 0 ? "Stock Settlement - Edit " : "Stock Settlement - New ");
            else if (arabic)
                root.Text = TranslateSettlementText(root.Text);

            // PlaceholderText is separate from Text and must be translated explicitly.
            if (arabic && root is Guna2TextBox gunaTextBox && !string.IsNullOrWhiteSpace(gunaTextBox.PlaceholderText))
                gunaTextBox.PlaceholderText = TranslateSettlementText(gunaTextBox.PlaceholderText);

            foreach (Control child in root.Controls)
                ApplySettlementLocalization(child);

            // Explicitly cover the action buttons and amount captions if their designer text
            // differs slightly from the resource key.
            if (arabic)
            {
                if (btnSaveNew != null) btnSaveNew.Text = "حفظ وجديد";
                if (btnSave != null) btnSave.Text = "حفظ";
                if (btnClose != null) btnClose.Text = "إغلاق";
            }
        }

        private void ApplySettlementGridLocalization(DataGridView grid, bool arabic)
        {
            if (grid == null) return;
            foreach (DataGridViewColumn column in grid.Columns)
            {
                string name = (column.Name ?? string.Empty).Replace("_", "").ToLowerInvariant();
                string english = column.HeaderText;
                string arabicText = null;
                switch (name)
                {
                    case "code": arabicText = "الرمز"; break;
                    case "name": arabicText = "اسم الصنف"; break;
                    case "qty": arabicText = "الكمية المتوفرة"; break;
                    case "costprice": arabicText = "سعر التكلفة"; break;
                    case "total": arabicText = "الإجمالي"; break;
                    case "newqty": arabicText = "الكمية الجديدة"; break;
                    case "qtydif": arabicText = "فرق الكمية"; break;
                    case "minusamount": arabicText = "مبلغ النقص"; break;
                    case "plusamount": arabicText = "مبلغ الزيادة"; break;
                    case "delete": arabicText = "حذف"; break;
                    case "method": arabicText = "طريقة التكلفة"; break;
                    case "type": arabicText = "النوع"; break;
                }
                if (arabic && !string.IsNullOrEmpty(arabicText))
                    column.HeaderText = arabicText;
            }
        }

        private void ShowSettlementMessage(string message, string title = "")
        {
            string displayMessage = IsArabicLanguage() ? TranslateSettlementText(message) : message;
            string displayTitle = IsArabicLanguage() ? TranslateSettlementText(title) : title;
            if (string.IsNullOrEmpty(displayTitle))
                MessageBox.Show(displayMessage);
            else
                MessageBox.Show(displayMessage, displayTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public frmAddStockSettlement(int id = 0)
        {
            InitializeComponent();
            dgvItems.EnableHeadersVisualStyles = false; // important
            //AppTheme.DefaultFont(dgvItems);
            this.id = id;
            LocalizationManager.LocalizeForm(this);
            ApplySettlementLocalization(this);
            headerUC1.FormText = this.Text;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmAddStockSettlement_Load(object sender, EventArgs e)
        {
            ApplySettlementLocalization(this);
            headerUC1.FormText = this.Text;
            //AppTheme.DefaultFont(dgvItems);
            dgvItems.Columns["newQty"].DefaultCellStyle.BackColor = Color.LightGray;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_config WHERE category='Inventory'"))
                if (reader.Read())
                    inventoryAccount = int.Parse(reader["account_id"].ToString());
                else
                {
                    ShowSettlementMessage("Inventory Account Must Be Set First");
                    return;
                }
            using (var reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_config WHERE category='Stock Settlement'"))
                if (reader.Read())
                    stockSettleAccount = int.Parse(reader["account_id"].ToString());
                else
                {
                    ShowSettlementMessage("Stock Settlement Account Must Be Set First");
                    return;
                }
            BindCombos.PopulateWarehouse(cmbWarehouse);
            if (id != 0)
            {
                BindInvoice();
                btnSave.Enabled = btnSaveNew.Enabled = UserPermissions.canEdit("Stock Management");
            }
        }
        private void BindInvoice()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(" select * from tbl_item_stock_settlement where id = @id",
                ClinicDatabase.CreateParameterNew("id", id)))
            {
                reader.Read();
                cmbWarehouse.SelectedValue = reader["warehouse_id"].ToString();
                invCode = txtInvoiceId.Text = reader["code"].ToString();
            }
            invId = id;
            BindInvoiceItems();
            ApplySettlementGridLocalization(dgvItems, IsArabicLanguage());
            CalculateTotal();
        }
        private void BindInvoiceItems()
        {
            dgvItems.Rows.Clear();
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT tbl_item_stock_settlement_details.*,tbl_items.type,method, tbl_items.code as code FROM tbl_item_stock_settlement_details INNER JOIN 
                                                                    tbl_items ON tbl_item_stock_settlement_details.item_id = tbl_items.id WHERE 
                                                                    tbl_item_stock_settlement_details.settle_id = @id;",
                                                            ClinicDatabase.CreateParameterNew("id", id)))
                while (reader.Read())
                {
                    dgvItems.Rows.Add(reader["item_id"].ToString(), "", reader["code"].ToString(), reader["code"].ToString(), reader["on_hand"].ToString(),
                        reader["price"].ToString(), (decimal.Parse(reader["price"].ToString()) * decimal.Parse(reader["on_hand"].ToString())), reader["new_on_hand"].ToString(),
                        (decimal.Parse(reader["new_on_hand"].ToString()) - decimal.Parse(reader["on_hand"].ToString())));
                    if (decimal.Parse(reader["new_on_hand"].ToString()) > decimal.Parse(reader["on_hand"].ToString()))
                    {
                        dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["minusamount"].Value = "0";
                        dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["plusamount"].Value = (decimal.Parse(reader["price"].ToString()) * (decimal.Parse(reader["new_on_hand"].ToString()) - decimal.Parse(reader["on_hand"].ToString()))).ToString();
                    }
                    else
                    {
                        dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["plusamount"].Value = "0";
                        dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["minusamount"].Value = (decimal.Parse(reader["price"].ToString()) * (decimal.Parse(reader["on_hand"].ToString()) - decimal.Parse(reader["new_on_hand"].ToString()))).ToString();
                    }
                    dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["method"].Value = reader["method"].ToString();
                    dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["method"].Value = reader["method"].ToString();
                    dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["type"].Value = reader["type"].ToString();
                }
            ApplySettlementGridLocalization(dgvItems, IsArabicLanguage());
        }
        private DataTable GetFilteredProductsByName(string name = "")
        {
            int warehouseId = cmbWarehouse.SelectedValue != null
                ? int.Parse(cmbWarehouse.SelectedValue.ToString())
                : 0;

            string query = @"SELECT DISTINCT i.code, i.name FROM tbl_items i LEFT JOIN tbl_item_transaction t ON t.item_id = i.id 
                WHERE t.warehouse_id = @warehouseId AND i.state = 0 AND i.active = 0 AND i.type != 'Service'";
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@warehouseId", warehouseId)
            };

            if (!string.IsNullOrEmpty(name))
            {
                query += " AND i.name LIKE @name";
                parameters.Add(new MySqlParameter("@name", $"%{name}%"));
            }

            query += " ORDER BY i.name LIMIT 20";

            return ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
        }

        private DataTable GetFilteredProductsByCode(string code = "")
        {
            int warehouseId = cmbWarehouse.SelectedValue != null ? int.Parse(cmbWarehouse.SelectedValue.ToString()) : 0;

            string query = @"SELECT DISTINCT i.code, i.name FROM tbl_items i LEFT JOIN tbl_item_transaction t ON t.item_id = i.id 
                    WHERE t.warehouse_id = @warehouseId AND i.state = 0 AND i.active = 0 AND i.type != 'Service'";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@warehouseId", warehouseId)
            };

            if (!string.IsNullOrEmpty(code))
            {
                query += " AND i.code LIKE @code";
                parameters.Add(new MySqlParameter("@code", $"%{code}%"));
            }

            query += " ORDER BY i.code LIMIT 20";

            return ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_config WHERE category='Inventory'"))
                if (reader.Read())
                    inventoryAccount = int.Parse(reader["account_id"].ToString());
                else
                {
                    ShowSettlementMessage("Inventory Account Must Be Set First");
                    return;
                }
            using (var reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_config WHERE category='Stock Settlement'"))
                if (reader.Read())
                    stockSettleAccount = int.Parse(reader["account_id"].ToString());
                else
                {
                    ShowSettlementMessage("Stock Settlement Account Must Be Set First");
                    return;
                }
            checkItemValidty();
            if (id == 0)
            {
                if (insertInvoice())
                    this.Close();
            }
            else
            {
                if (updateInvoice())
                    this.Close();
            }
        }
        private bool updateInvoice()
        {
            if (!chkRequiredDate())
                return false;
            ClinicDatabase.ExecuteNonQuery(@"
                UPDATE tbl_item_stock_settlement 
                SET 
                    code = @code,
                    date = @date,
                    warehouse_id = @warehouse_id,
                    total_plus = @total_plus,
                    total_minus = @total_minus,
                    created_by = @created_by,
                    created_date = @created_date
                WHERE id = @id;",
                  ClinicDatabase.CreateParameterNew("date", dtInv.Value.Date),
                  ClinicDatabase.CreateParameterNew("code", invCode),
                  ClinicDatabase.CreateParameterNew("warehouse_id", cmbWarehouse.SelectedValue),
                  ClinicDatabase.CreateParameterNew("total_plus", txtAmountPlus.Text),
                  ClinicDatabase.CreateParameterNew("total_minus", txtAmountMinus.Text),
                  ClinicDatabase.CreateParameterNew("created_by", -1),
                  ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date),
                  ClinicDatabase.CreateParameterNew("id", id)
              );
            ClinicDatabase.ExecuteReader(@"delete from tbl_transaction where transaction_id =@id and (description like 'Stock Inventory Settlement%')",
                                                       ClinicDatabase.CreateParameterNew("id", id));
            insertJournalEntries();
            ReturnItemsToInventory();
            insertInvItems();

            return true;
        }

        private void ReturnItemsToInventory()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"select tbl_item_stock_settlement_details.* ,tbl_items.method,tbl_items.type from tbl_item_stock_settlement_details
                                                           inner join tbl_items on tbl_item_stock_settlement_details.item_id = tbl_items.id 
                                                           where settle_id =" + id))
                while (reader.Read())
                {
                    if (decimal.Parse(reader["qty"].ToString()) > 0)
                    {
                        ClinicDatabase.ExecuteNonQuery("update tbl_items set on_hand =on_hand- @qty where id =@id", ClinicDatabase.CreateParameterNew("id", reader["item_id"].ToString()), ClinicDatabase.CreateParameterNew("qty", reader["qty"].ToString()));
                        ClinicDatabase.ExecuteNonQuery("delete from tbl_item_transaction  where reference =@invId and type= 'Stock Inventory Settlement'", ClinicDatabase.CreateParameterNew("invId", id));
                    }
                    else
                    {
                        ClinicDatabase.ExecuteNonQuery("update tbl_items set on_hand =on_hand+ @qty where id =@id", ClinicDatabase.CreateParameterNew("id", reader["item_id"].ToString()), ClinicDatabase.CreateParameterNew("qty", -decimal.Parse(reader["qty"].ToString())));
                        ClinicDatabase.ExecuteNonQuery("delete from tbl_item_transaction  where reference =@invId and type= 'Stock Inventory Settlement'",
                            ClinicDatabase.CreateParameterNew("invId", id));
                        string tq = "";
                        if (reader["method"].ToString() == "agv")
                            continue;
                        if (reader["method"].ToString() == "fifo")
                            tq = (@"SELECT * FROM tbl_item_transaction WHERE item_id = @id and qty_in != qty_inc
                                                                     and qty_out = 0 ORDER BY id asc");
                        else
                            tq = (@"SELECT * FROM tbl_item_transaction WHERE item_id = @id and qty_in != qty_inc
                                                                     and qty_out = 0 ORDER BY id asc");
                        decimal attempQty = -decimal.Parse(reader["qty"].ToString());
                        using (var tReader = ClinicDatabase.ExecuteReader(tq, ClinicDatabase.CreateParameterNew("id", reader["item_id"].ToString())))
                            while (tReader.Read())
                            {
                                if (decimal.Parse(tReader["qty_inc"].ToString()) + attempQty <= decimal.Parse(tReader["qty_in"].ToString()))
                                {
                                    ClinicDatabase.ExecuteNonQuery("update tbl_item_transaction set qty_inc = qty_inc +@qty where id = @id",
                                        ClinicDatabase.CreateParameterNew("qty", attempQty),
                                        ClinicDatabase.CreateParameterNew("id", tReader["id"].ToString()));
                                    break;
                                }
                                else
                                {
                                    attempQty = attempQty - decimal.Parse(tReader["qty_in"].ToString()) - decimal.Parse(tReader["qty_inc"].ToString());
                                    ClinicDatabase.ExecuteNonQuery("update tbl_item_transaction set qty_inc = qty_in where id = @id",
                                      ClinicDatabase.CreateParameterNew("id", tReader["id"].ToString()));
                                }
                            }
                    }
                    ClinicDatabase.ExecuteNonQuery("delete from tbl_item_stock_settlement_details where id =@id", ClinicDatabase.CreateParameterNew("id", reader["id"].ToString()));
                }
        }

        private bool insertInvoice()
        {
            if (!chkRequiredDate())
                return false;

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT code FROM tbl_item_stock_settlement ORDER BY 
                CAST(SUBSTRING_INDEX(code, '-', -1) AS UNSIGNED) DESC LIMIT 1; "))
                if (reader.Read() && reader["code"].ToString() != "")
                    invCode = "SIS-000" + (int.Parse(reader["code"].ToString().Replace("SIS-", "")) + 1);

            invId = decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_item_stock_settlement (code,date, warehouse_id, total_plus, total_minus,
                created_by, created_date, state) VALUES (@code,@DATE,@warehouse_id,@total_plus, @total_minus,@created_by, @created_date, 0);
                SELECT LAST_INSERT_ID();",
               ClinicDatabase.CreateParameterNew("date", dtInv.Value.Date),
               ClinicDatabase.CreateParameterNew("code", invCode),
               ClinicDatabase.CreateParameterNew("warehouse_id", cmbWarehouse.SelectedValue),
               ClinicDatabase.CreateParameterNew("total_plus", txtAmountPlus.Text),
               ClinicDatabase.CreateParameterNew("total_minus", txtAmountMinus.Text),
               ClinicDatabase.CreateParameterNew("created_by", -1),
               ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date)).ToString());
            insertJournalEntries();
            insertInvItems();
            txtInvoiceId.Text = invCode.ToString();

            ////Utilities.LogAudit(-1, "Insert Stock Settlement", "Stock Settlement", (int)invId, "Inserted Stock Settlement: " + invCode);

            return true;
        }

        private void insertJournalEntries()
        {
            if (decimal.Parse(txtAmountMinus.Text) > 0)
            {
                CommonInsert.InsertTransactionEntry(dtInv.Value.Date,
                     stockSettleAccount.ToString(),
                       txtAmountMinus.Text, "0", invId.ToString(), invId.ToString(), "Stock Inventory Settlement", "Stock Inventory Settlement NO. " + invCode,
                     -1, DateTime.Now.Date);
                CommonInsert.InsertTransactionEntry(dtInv.Value.Date,
                     inventoryAccount.ToString(),
                     "0", txtAmountMinus.Text, invId.ToString(), invId.ToString(), "Inventory Settlement", "Stock Inventory Settlement NO. " + invCode,
                    -1, DateTime.Now.Date);
            }
            if (decimal.Parse(txtAmountPlus.Text) > 0)
            {
                CommonInsert.InsertTransactionEntry(dtInv.Value.Date,
                     stockSettleAccount.ToString(),
                        "0", txtAmountPlus.Text, invId.ToString(), invId.ToString(), "Stock Inventory Settlement", "Stock Inventory Settlement NO. " + invCode,
                     -1, DateTime.Now.Date);
                CommonInsert.InsertTransactionEntry(dtInv.Value.Date,
                     inventoryAccount.ToString(),
                    txtAmountPlus.Text, "0", invId.ToString(), invId.ToString(), "Inventory Settlement", "Stock Inventory Settlement NO. " + invCode,
                    -1, DateTime.Now.Date);
            }
        }

        private void insertInvItems()
        {
            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_item_stock_settlement_details (settle_id, item_id,on_hand, price, new_on_hand, qty, minusamount,plusamount)
                                         VALUES (@settle_id, @item_id, @on_hand,@price, @new_on_hand, @qty, @minusamount,@plusamount);",
                  ClinicDatabase.CreateParameterNew("@settle_id", invId),
                  ClinicDatabase.CreateParameterNew("@item_id", dgvItems.Rows[i].Cells["itemId"].Value.ToString()),
                  ClinicDatabase.CreateParameterNew("@on_hand", dgvItems.Rows[i].Cells["qty"].Value == DBNull.Value ? 0 : Convert.ToDecimal(dgvItems.Rows[i].Cells["qty"].Value)),
                  ClinicDatabase.CreateParameterNew("@price", dgvItems.Rows[i].Cells["cost_price"].Value),
                  ClinicDatabase.CreateParameterNew("@new_on_hand", dgvItems.Rows[i].Cells["newQty"].Value),
                  ClinicDatabase.CreateParameterNew("@qty", dgvItems.Rows[i].Cells["qtydif"].Value),
                  ClinicDatabase.CreateParameterNew("@minusamount", decimal.Parse(dgvItems.Rows[i].Cells["minusamount"].Value.ToString())),
                ClinicDatabase.CreateParameterNew("@plusamount", (decimal.Parse(dgvItems.Rows[i].Cells["plusamount"].Value.ToString()))));

                insertItemTransaction(dgvItems.Rows[i]);

                ////Utilities.LogAudit(-1, "Insert Stock Settlement Item", "Stock Settlement", (int)invId, 
            }
        }
        private void insertItemTransaction(DataGridViewRow row)
        {
            decimal qty = 0;

            if (row.Cells["itemId"].Value == null || string.IsNullOrWhiteSpace(row.Cells["itemId"].Value.ToString()))
            {
                ShowSettlementMessage("Invalid Item ID.");
                return;
            }
            if (!decimal.TryParse(row.Cells["newqty"].Value.ToString(), out qty) || qty <= 0)
            {
                ShowSettlementMessage("Invalid Quantity.");
                return;
            }

            if (decimal.Parse(row.Cells["newQty"].Value.ToString()) < decimal.Parse(row.Cells["qty"].Value.ToString()))
            {
                qty = decimal.Parse(row.Cells["qty"].Value.ToString()) - decimal.Parse(row.Cells["newQty"].Value.ToString());
                if (row.Cells["method"].Value.ToString() == "fifo")
                {
                    decimal remainingQty = qty;
                    using (var reader = ClinicDatabase.ExecuteReader(@"SELECT * FROM tbl_item_transaction WHERE qty_inc > 0 AND item_id = @id ORDER BY id ASC",
                        ClinicDatabase.CreateParameterNew("id", row.Cells["itemId"].Value.ToString())))
                        while (reader.Read() && remainingQty > 0)
                        {
                            decimal availableQty = Convert.ToDecimal(reader["qty_inc"]);
                            decimal qtyToUse = Math.Min(remainingQty, availableQty);
                            remainingQty -= qtyToUse;

                            CommonInsert.InsertItemTransaction(dtInv.Value.Date, "Stock Settlement", invId.ToString(), row.Cells["itemId"].Value.ToString(),
                                row.Cells["cost_price"].Value.ToString(), "0", "0", qtyToUse.ToString(), "0", "Stock Settlement No. " + invCode, "0", "0", "0", "0", "0");

                            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_item_transaction SET qty_inc = qty_inc - @qty WHERE id = @id",
                                ClinicDatabase.CreateParameterNew("qty", qtyToUse),
                                ClinicDatabase.CreateParameterNew("id", reader["id"].ToString()));
                        }
                }
                else if (row.Cells["method"].Value.ToString() == "lifo")
                {
                    decimal remainingQty = qty;
                    using (var reader = ClinicDatabase.ExecuteReader(@"SELECT * FROM tbl_item_transaction WHERE qty_inc > 0 AND item_id = @id ORDER BY id DESC",
                        ClinicDatabase.CreateParameterNew("id", row.Cells["itemId"].Value.ToString())))
                        while (reader.Read() && remainingQty > 0)
                        {
                            decimal availableQty = Convert.ToDecimal(reader["qty_inc"]);

                            decimal qtyToUse = Math.Min(remainingQty, availableQty);
                            remainingQty -= qtyToUse;

                            CommonInsert.InsertItemTransaction(dtInv.Value.Date, "Stock Settlement", invId.ToString(), row.Cells["itemId"].Value.ToString(),
                                row.Cells["cost_price"].Value.ToString(), "0", "0", qtyToUse.ToString(), "0", "Stock Settlement No. " + invCode, "0", "0", "0", "0", "0");

                            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_item_transaction SET qty_inc = qty_inc - @qty WHERE id = @id",
                                ClinicDatabase.CreateParameterNew("qty", qtyToUse),
                                ClinicDatabase.CreateParameterNew("id", reader["id"].ToString()));
                        }
                }
                else
                {

                    using (var reader = ClinicDatabase.ExecuteReader(@"SELECT SUM(qty_in * cost_price) / SUM(qty_in) AS cost_price 
                                         FROM tbl_item_transaction WHERE qty_in > 0 AND item_id = @id",
                        ClinicDatabase.CreateParameterNew("id", row.Cells["itemId"].Value.ToString())))

                        CommonInsert.InsertItemTransaction(dtInv.Value.Date, "Stock Settlement", invId.ToString(), row.Cells["itemId"].Value.ToString(),
                           row.Cells["cost_price"].Value.ToString(), "0", row.Cells["cost_price"].Value.ToString(), qty.ToString(), "0", "Stock Settlement No. " + invCode, "0", "0", "0", "0", "0");
                }

            }
            else
            {
                qty = decimal.Parse(row.Cells["newQty"].Value.ToString()) - decimal.Parse(row.Cells["qty"].Value.ToString());

                CommonInsert.InsertItemTransaction(dtInv.Value.Date, "Stock Settlement", invId.ToString(), row.Cells["itemId"].Value.ToString(), row.Cells["cost_price"].Value.ToString(),
                      qty.ToString(), "0", "0", qty.ToString(), "Stock Settlement No. " + invCode, "0", "0", "0", "0", "0");

            }

        }
        private bool chkRequiredDate()
        {
            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                if ((dgvItems.Rows[i].Cells["plusamount"].Value == null && dgvItems.Rows[i].Cells["minusamount"].Value == null)
                    || (dgvItems.Rows[i].Cells["plusamount"].Value.ToString() == "" && dgvItems.Rows[i].Cells["minusamount"].Value.ToString() == "")
                    || (decimal.Parse(dgvItems.Rows[i].Cells["plusamount"].Value.ToString()) == 0 && decimal.Parse(dgvItems.Rows[i].Cells["minusamount"].Value.ToString()) == 0))
                {
                    ShowSettlementMessage("Total Item In Row " + (dgvItems.Rows[i].Index + 1) + " Can't Be 0 or Null");
                    return false;
                }
            }

            if (dgvItems.Rows.Count == 1)
            {
                ShowSettlementMessage("Insert Items First.");
                return false;
            }
            if ((txtAmountMinus.Text == "" || decimal.Parse(txtAmountMinus.Text) == 0) && ((txtAmountPlus.Text == "" || decimal.Parse(txtAmountPlus.Text) == 0)))
            {
                ShowSettlementMessage("Amount Must Be Bigger Than Zero");
                return false;
            }
            return true;
        }
        private void resetTextBox()
        {
            txtInvoiceId.Text = txtAmountMinus.Text = txtAmountPlus.Text = "";
            dtInv.Value = DateTime.Now;
            dgvItems.Rows.Clear();
        }
        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                if (insertInvoice())
                    resetTextBox();
            }
            else
            {
                if (updateInvoice())
                    resetTextBox();
            }
        }
        private void txtSalesPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }


        private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox0)
            {
                textBox0.TextChanged -= ItemCode_TextChanged;
                textBox0.TextChanged -= ItemName_TextChanged;
                if (lstAccountSuggestions.Visible)
                {
                    lstAccountSuggestions.Visible = false;
                }
                lstAccountSuggestions.SendToBack();
            }
            if (dgvItems.CurrentCell.ColumnIndex == dgvItems.Columns["name"].Index)
            {
                if (e.Control is TextBox textBox)
                {
                    textBox.TextChanged -= ItemName_TextChanged;
                    textBox.TextChanged += ItemName_TextChanged;

                    lstAccountSuggestions.Tag = textBox;
                }
            }
            else if (dgvItems.CurrentCell.ColumnIndex == dgvItems.Columns["code"].Index)
            {
                if (e.Control is TextBox textBox)
                {
                    textBox.TextChanged -= ItemCode_TextChanged;
                    textBox.TextChanged += ItemCode_TextChanged;

                    lstAccountSuggestions.Tag = textBox;
                }
            }
            else
            {
                lstAccountSuggestions.Visible = false;
                lstAccountSuggestions.Tag = null;
            }

        }

        private void ItemCode_TextChanged(object sender, EventArgs e)
        {
            TextBox editingTextBox = sender as TextBox;
            if (editingTextBox == null) return;

            string input = editingTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                lstAccountSuggestions.Visible = false;
                return;
            }
            DataTable filtered = GetFilteredProductsByCode(input);

            lstAccountSuggestions.Items.Clear();

            foreach (DataRow row in filtered.Rows)
            {
                lstAccountSuggestions.Items.Add(new ItemSuggestion { Code = row["code"].ToString(), Name = row["name"].ToString() });
            }

            if (lstAccountSuggestions.Items.Count > 0)
            {
                // Get cell location
                var cellRect = dgvItems.GetCellDisplayRectangle(dgvItems.CurrentCell.ColumnIndex, dgvItems.CurrentCell.RowIndex, true);
                Point locationOnForm = dgvItems.PointToScreen(cellRect.Location);
                Point locationRelativeToForm = this.PointToClient(locationOnForm);

                lstAccountSuggestions.SetBounds(
                    locationRelativeToForm.X,
                    locationRelativeToForm.Y + cellRect.Height,
                    cellRect.Width + 100,
                    120
                );

                lstAccountSuggestions.Tag = editingTextBox;
                lstAccountSuggestions.Visible = true;
                lstAccountSuggestions.BringToFront();
            }
            else
            {
                lstAccountSuggestions.Visible = false;
            }
        }

        private void ItemName_TextChanged(object sender, EventArgs e)
        {
            TextBox editingTextBox = sender as TextBox;
            if (editingTextBox == null) return;

            string input = editingTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                lstAccountSuggestions.Visible = false;
                return;
            }
            DataTable filtered = GetFilteredProductsByName(input);

            lstAccountSuggestions.Items.Clear();

            foreach (DataRow row in filtered.Rows)
            {
                lstAccountSuggestions.Items.Add(new ItemSuggestion { Code = row["code"].ToString(), Name = row["name"].ToString() });
            }

            if (lstAccountSuggestions.Items.Count > 0)
            {
                // Get cell location
                var cellRect = dgvItems.GetCellDisplayRectangle(dgvItems.CurrentCell.ColumnIndex, dgvItems.CurrentCell.RowIndex, true);
                Point locationOnForm = dgvItems.PointToScreen(cellRect.Location);
                Point locationRelativeToForm = this.PointToClient(locationOnForm);

                lstAccountSuggestions.SetBounds(
                    locationRelativeToForm.X,
                    locationRelativeToForm.Y + cellRect.Height,
                    cellRect.Width + 100,
                    120
                );

                lstAccountSuggestions.Tag = editingTextBox;
                lstAccountSuggestions.Visible = true;
                lstAccountSuggestions.BringToFront();
            }
            else
            {
                lstAccountSuggestions.Visible = false;
            }
        }

        void CalculateTotal()
        {
            decimal minus = 0, plus = 0;
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.Cells["minusamount"].Value != null)
                    minus += Convert.ToDecimal(row.Cells["minusamount"].Value);
                if (row.Cells["plusamount"].Value != null)
                    plus += Convert.ToDecimal(row.Cells["plusamount"].Value);

            }
            txtAmountPlus.Text = plus.ToString("0.000");
            txtAmountMinus.Text = minus.ToString("0.000");
        }
        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItems.Rows.Count > 1)
            {
                var row = dgvItems.Rows[e.RowIndex];
                decimal qty = GetDecimalValue(row, "qty");
                decimal costPrice = GetDecimalValue(row, "cost_price");
                if (e.ColumnIndex == dgvItems.Columns["Code"].Index)
                {
                    string codeValue = row.Cells["Code"].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(codeValue))
                        insertItemThroughCodeOrText("code", codeValue);
                }
                else if (e.ColumnIndex == dgvItems.Columns["Name"].Index)
                {
                    string nameValue = row.Cells["Name"].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(nameValue))
                        insertItemThroughCodeOrText("name", nameValue);
                }
                else if (e.ColumnIndex == dgvItems.Columns["newqty"].Index)
                {
                    if (row.Cells["qty"].Value == null)
                        return;

                    if (dgvItems.CurrentRow.Cells["itemId"].Value == null || row.Cells["newqty"].Value.ToString() == "" || row.Cells["newqty"].Value.ToString() == row.Cells["qty"].Value.ToString())
                        row.Cells["minusamount"].Value = row.Cells["qtydif"].Value = row.Cells["plusamount"].Value = 0;
                    else
                    {
                        row.Cells["qtydif"].Value = (decimal.Parse(row.Cells["newqty"].Value.ToString()) - decimal.Parse(row.Cells["qty"].Value.ToString())).ToString();
                        if (decimal.Parse(row.Cells["qtydif"].Value.ToString()) > 0)
                        {
                            row.Cells["plusamount"].Value = (decimal.Parse(row.Cells["qtydif"].Value.ToString()) * decimal.Parse(row.Cells["cost_price"].Value.ToString())).ToString();
                            row.Cells["minusamount"].Value = "0";
                        }
                        else
                        {
                            row.Cells["minusamount"].Value = (-decimal.Parse(row.Cells["qtydif"].Value.ToString()) * decimal.Parse(row.Cells["cost_price"].Value.ToString())).ToString();
                            row.Cells["plusamount"].Value = "0";
                        }
                    }
                }
                CalculateTotal();
            }
        }
        void checkItemValidty()
        {
            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                if (dgvItems.Rows[i].Cells["itemId"].Value == null)
                {
                    dgvItems.Rows.Remove(dgvItems.Rows[i]);
                    i--;
                }
            }
        }
        private decimal GetDecimalValue(DataGridViewRow row, string columnName)
        {
            decimal result;
            var cellValue = row.Cells[columnName].Value;
            if (cellValue != null && decimal.TryParse(cellValue.ToString(), out result))
                return result;
            else
                return 0;
        }

        private void insertItemThroughCodeOrText(string type, string inputText = null)
        {
            if (string.IsNullOrWhiteSpace(inputText))
                return;

            MySqlDataReader reader = null;
            try
            {
                if (type == "code")
                {
                    reader = ClinicDatabase.ExecuteReader(@"SELECT i.id,method,type, code, 
                            (SELECT IFNULL(SUM(qty_in - qty_out), 0) FROM tbl_item_transaction WHERE item_id = i.id AND warehouse_id = @wId) AS qty, cost_price, name
                          FROM tbl_items i LEFT JOIN tbl_items_warehouse w ON w.item_id = i.id
                          WHERE code = @code",
                        ClinicDatabase.CreateParameterNew("code", inputText.Trim()),
                        ClinicDatabase.CreateParameterNew("wId", cmbWarehouse.SelectedValue.ToString()));
                }
                else if (type == "name")
                {
                    reader = ClinicDatabase.ExecuteReader(@"SELECT i.id,method,type, code, 
                            (SELECT IFNULL(SUM(qty_in - qty_out), 0) FROM tbl_item_transaction WHERE item_id = i.id AND warehouse_id = @wId) AS qty, cost_price, name 
                          FROM tbl_items i LEFT JOIN tbl_items_warehouse w ON w.item_id = i.id
                          WHERE name = @name",
                        ClinicDatabase.CreateParameterNew("wId", cmbWarehouse.SelectedValue.ToString()),
                        ClinicDatabase.CreateParameterNew("name", inputText.Trim()));
                }

                if (reader != null && reader.Read())
                {
                    dgvItems.CurrentRow.Cells["qty"].Value = reader["qty"].ToString();
                    dgvItems.CurrentRow.Cells["code"].Value = reader["code"].ToString();
                    dgvItems.CurrentRow.Cells["name"].Value = reader["name"].ToString();
                    dgvItems.CurrentRow.Cells["itemid"].Value = reader["id"].ToString();
                    dgvItems.CurrentRow.Cells["cost_price"].Value = reader["cost_price"].ToString();
                    dgvItems.CurrentRow.Cells["total"].Value = Convert.ToDecimal(reader["cost_price"]) * Convert.ToDecimal(reader["qty"]);
                    dgvItems.CurrentRow.Cells["newqty"].Value = Convert.ToDecimal(reader["qty"]);
                    dgvItems.CurrentRow.Cells["qtydif"].Value = "0";
                    dgvItems.CurrentRow.Cells["minusamount"].Value = "0";
                    dgvItems.CurrentRow.Cells["plusamount"].Value = "0";
                    dgvItems.CurrentRow.Cells["method"].Value = reader["method"];
                    dgvItems.CurrentRow.Cells["type"].Value = reader["type"];
                }
                else
                {
                    DataGridViewRow row = dgvItems.CurrentRow;
                    if (row != null)
                    {
                        row.Cells["itemid"].Value = null;
                        row.Cells["cost_price"].Value = null;
                        row.Cells["total"].Value = null;
                        row.Cells["method"].Value = null;
                        row.Cells["type"].Value = null;
                        row.Cells["qty"].Value = 0;
                        row.Cells["newqty"].Value = null;
                        row.Cells["qtydif"].Value = null;
                        row.Cells["minusamount"].Value = null;
                        row.Cells["plusamount"].Value = null;
                        if (type == "name")
                            row.Cells["code"].Value = null;
                        if (type == "code")
                            row.Cells["name"].Value = null;
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceId.Text))
                return;
            int? currentId = Utilities.GetVoucherIdFromCode(txtInvoiceId.Text);
            if (currentId == null || currentId <= 1)
                return;

            currentId = currentId - 1;
            if (currentId <= 0)
            {
                clear();
                ShowSettlementMessage("No previous records.", "Info");
                return;
            }

            string query = "select id from tbl_item_stock_settlement where state = 0 and id =@id";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("id", currentId)))
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindInvoice();
                }
                else
                {
                    clear();
                    ShowSettlementMessage("No previous records.", "Info");
                }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceId.Text))
                return;
            int? currentId = Utilities.GetVoucherIdFromCode(txtInvoiceId.Text);
            if (currentId is null) return;

            currentId = currentId + 1;
            string query = "SELECT id FROM tbl_item_stock_settlement WHERE state = 0 AND id =@id";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("id", currentId)))
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindInvoice();
                }
                else
                {
                    clear();
                    ShowSettlementMessage("No next records.", "Info");
                }
        }

        private void clear()
        {
            id = 0;
            invId = 0;
            invCode = "";
            resetTextBox();
            dgvItems.Rows.Clear();
            cmbWarehouse.SelectedIndex = -1;
        }

        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItems.Rows.Count > 1 && dgvItems.CurrentRow.Cells["delete"].ColumnIndex == e.ColumnIndex)
            {
                dgvItems.Rows.Remove(dgvItems.CurrentRow);
                CalculateTotal();
            }
        }
        private void dgvItems_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvItems.Rows[e.RowIndex].Cells[1].Value = (e.RowIndex + 1).ToString();
        }

        private void lstAccountSuggestions_Click(object sender, EventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem == null)
                return;

            string selected = lstAccountSuggestions.SelectedItem.ToString();

            //int separatorIndex = selected.IndexOf('-');
            //if (separatorIndex == -1) return;

            string selectedCode = "";
            string selectedName = "";
            if (lstAccountSuggestions.SelectedItem is ItemSuggestion item)
            {
                selectedCode = item.Code;
                selectedName = item.Name;
            }

            if (lstAccountSuggestions.Tag is TextBox textBox)
            {
                if (dgvItems.CurrentCell != null && dgvItems.CurrentCell.ColumnIndex == dgvItems.Columns["code"].Index)
                {
                    dgvItems.CurrentCell.Value = selectedCode;
                }
                else if (dgvItems.CurrentCell != null && dgvItems.CurrentCell.ColumnIndex == dgvItems.Columns["name"].Index)
                {
                    dgvItems.CurrentCell.Value = selectedName;
                }
            }

            lstAccountSuggestions.Visible = false;
        }
    }
}
