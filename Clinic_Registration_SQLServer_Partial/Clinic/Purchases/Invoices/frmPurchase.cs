using Clinic;
using Clinic_DataAccess;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YamyProject;
using YamyProject.Localization;

namespace YamyProject
{

    public partial class frmPurchase : Form
    {
        private EventHandler vendorUpdatedHandler;
        private EventHandler itemUpdatedHandler;
        private EventHandler warehouseUpdatedHandler;

        decimal invId;
        int level4PaymentCreditMethodId, level4VatId, level4PurchaseInvoice, level4Inventory;
        int id, purchaseOrderId = 0, defaultVendorId = 0;
        string PO;
        bool EnableBarcode = false;
        string typed = "";

        public frmPurchase(int _id = 0, string _PO = "", int _vendorId = 0, bool _isSubcontractor = false)
        {
            InitializeComponent();
            dgvItems.EnableHeadersVisualStyles = false; // important
            //AppTheme.DefaultFont(dgvItems);
            headerUC1.FormText = this.Text;
            LocalizationManager.LocalizeForm(this);
            vendorUpdatedHandler = (sender, args) => BindCombos.PopulateVendors(cmbVendor);
            itemUpdatedHandler = (sender, args) => RefreshItems();
            warehouseUpdatedHandler = (sender, args) => BindCombos.PopulateWarehouse(cmbWarehouse);
            EventHub.Item += itemUpdatedHandler;
            EventHub.wareHouse += warehouseUpdatedHandler;
            EventHub.Vendor += vendorUpdatedHandler;

            this.PO = _PO;
            if (PO == "PO")
            {
                txtPONO.Text = _id.ToString();
            }
            else
            {
                this.id = _id;
                txtPONO.Text = "";
            }
            this.defaultVendorId = _vendorId;
            if (_isSubcontractor)
            {
                CbSubcontractors.Checked = true;
                typed = "Subcontractor";
            }
        }

        private void frmPurchase_FormClosing(object sender, FormClosingEventArgs e)
        {
            EventHub.Vendor -= vendorUpdatedHandler;
            EventHub.Item -= itemUpdatedHandler;
            EventHub.wareHouse -= warehouseUpdatedHandler;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPurchase_Load(object sender, EventArgs e)
        {
            guna2Panel1.Height = 0;
            dtPaymentTerms.Value = dtInv.Value = dtShip.Value = DateTime.Now.Date;
            BindCombos.PopulateWarehouse(cmbWarehouse);
            if (cmbWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Enter At Least One Warehouse");
                new frmViewWarehouse().ShowDialog();
                BindCombos.PopulateWarehouse(cmbWarehouse, false, true);
            }
            loadVat();
            //loadItemUnits();
            cmbPaymentMethod.SelectedIndex = 0;
            bindCombo();
            txtNextCode.Text = GenerateNextSalesCode();
            txtInvoiceId.Text = GetNextPurchaseId();
            if (id != 0)
            {
                btnSave.Enabled = btnSaveClose.Enabled = UserPermissions.canEdit("Purchases Center");
                BindInvoice();
            }
            else
            {
                if (defaultVendorId > 0)
                {
                    cmbVendor.SelectedValue = defaultVendorId;
                }
            }
            purchaseOrderId = PO == "" ? purchaseOrderId = 0 : int.Parse(txtPONO.Text.ToString());
            if (purchaseOrderId != 0)
            {
                loadPurchaseOrderData();
            }
            var barcodeS = Utilities.GeneralSettingsState("ENABLE BARCODE");
            if (!string.IsNullOrEmpty(barcodeS) && int.Parse(barcodeS) > 0)
            {
                EnableBarcode = true;
            }
            else
            {
                EnableBarcode = false;
            }

            if (EnableBarcode)
            {
                txtBarcodeScan.Visible = true;
                this.ActiveControl = txtBarcodeScan;
            }
        }

        private bool suppressCellValueChanged = false;

        private void TxtBarcodeScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string scannedCode = txtBarcodeScan.Text.Trim();
                if (!string.IsNullOrEmpty(scannedCode))
                {
                    InsertItemByBarcode(scannedCode);
                    txtBarcodeScan.Clear();
                }
            }
        }

        private void InsertItemByBarcode(string barcode)
        {
            suppressCellValueChanged = true;
            // 1. Fetch the item from DB
            DataTable dt = ClinicDatabase.ExecuteDataTable(
                "SELECT id, method, type, code, IFNULL(sales_price,0) sales_price, IFNULL(cost_price,0) cost_price, name FROM tbl_items WHERE barcode = @barcode",
                ClinicDatabase.CreateParameterNew("@barcode", barcode)
            );

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Item not found for barcode: " + barcode);
                return;
            }

            // Always reference this row freshly
            DataRow item = dt.Rows[0];

            string itemCode = item["code"].ToString();
            string itemName = item["name"].ToString();
            string itemId = item["id"].ToString();
            string itemMethod = item["method"].ToString();
            string itemType = item["type"].ToString();
            decimal salesPrice = Convert.ToDecimal(item["sales_price"]);
            decimal costPrice = Convert.ToDecimal(item["cost"]);

            // 2. Check if item already in grid, then increase qty
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["code"].Value?.ToString() == itemCode)
                {
                    decimal currentQty = GetDecimalValue(row, "qty");
                    row.Cells["qty"].Value = currentQty + 1;

                    ChkRowValidity(row);
                    CalculateTotal();
                    return;
                }
            }

            // 3. Add a new row for new item
            int rowIndex = dgvItems.Rows.Add();
            var newRow = dgvItems.Rows[rowIndex];

            newRow.Cells["itemId"].Value = itemId;
            newRow.Cells["code"].Value = itemCode;
            newRow.Cells["name"].Value = itemName;
            newRow.Cells["qty"].Value = 1;
            newRow.Cells["discount"].Value = 0;
            newRow.Cells["cost"].Value = costPrice.ToString("N2");
            newRow.Cells["price"].Value = salesPrice.ToString("N2");
            newRow.Cells["method"].Value = itemMethod;
            newRow.Cells["type"].Value = itemType;

            if (!string.IsNullOrEmpty(defaultTax))
                newRow.Cells["vat"].Value = int.Parse(defaultTax);

            decimal discount = 0;
            decimal qty = 1;
            decimal netPrice = (salesPrice - discount) * qty;
            decimal vatAmount = 0;
            if (!string.IsNullOrEmpty(defaultTax))
            {
                int vatRate = int.Parse(defaultTax);
                vatAmount = netPrice * vatRate / 100;
            }

            newRow.Cells["net_price"].Value = netPrice.ToString("0.00");
            newRow.Cells["VatP"].Value = vatAmount;
            newRow.Cells["total"].Value = (netPrice + vatAmount).ToString("0.00");

            ChkRowValidity(newRow);
            CalculateTotal();

            suppressCellValueChanged = false;
        }

        private void BindInvoice()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(" select * from tbl_purchase where id = @id",
                ClinicDatabase.CreateParameterNew("id", id)))
            {
                if (reader.Read())
                {
                    dtInv.Value = DateTime.Parse(reader["date"].ToString());
                    txtNextCode.Text = reader["invoice_id"].ToString();
                    txtInvoiceId.Text = reader["id"].ToString();
                    var Vendor_Id = reader["vendor_id"].ToString();
                    //CbSubcontractors.Checked = Utilities.CheckVendorType(Vendor_Id);
                    cmbVendor.SelectedValue = Vendor_Id;
                    using (var rd = ClinicDatabase.ExecuteReader("select type,name from tbl_vendor where id=@id", ClinicDatabase.CreateParameterNew("id", Vendor_Id)))
                    {
                        if (rd.Read())
                        {
                            var t = rd["type"];
                            if (!string.IsNullOrWhiteSpace(t.ToString()))
                                CbSubcontractors.Checked = t.ToString() == "Subcontractor";

                            cmbVendor.Text = cmbVendor.SelectedText = rd["name"].ToString();
                        }
                    }

                    cmbPaymentMethod.Text = reader["payment_method"].ToString();
                    if (reader["payment_method"].ToString() == "Cash")
                        cmbAccountCashName.SelectedValue = reader["account_cash_id"].ToString();
                    else
                    {
                        cmbPaymentTerms.Text = reader["payment_terms"].ToString();
                        dtPaymentTerms.Value = DateTime.Parse(reader["payment_date"].ToString());
                    }
                    cmbShipVia.Text = reader["ship_via"].ToString();
                    cmbWarehouse.SelectedValue = reader["warehouse_id"].ToString();
                    cmbPurchasetype.Text = reader["purchase_type"].ToString();
                    txtNextCode.Text = reader["invoice_id"].ToString();
                    txtInvoiceId.Text = reader["id"].ToString();
                    richTextDescription.Text = reader["description"].ToString();
                    invId = id;
                    txtBillTo.Text = reader["bill_to"].ToString();
                    txtShipTo.Text = reader["ship_to"].ToString();
                    txtSalesMan.Text = reader["sales_man"].ToString();
                    txtRefId.Text = reader["reference_no"].ToString();
                    if (cmbPurchasetype.Text.ToString().Trim() == "Fixed Assets")
                    {
                        labelFixedAssetCategory.Visible = true;
                        cmbFixedAssetCategory.Visible = true;
                        cmbFixedAssetCategory.SelectedValue = int.Parse(reader["fixed_asset_category_id"].ToString());
                    }
                    cmbCostCenter.SelectedValue = reader["cost_center_id"].ToString();
                }
                BindInvoiceItems();
                CalculateTotal();
            }
        }

        private void BindInvoiceItems()
        {
            dgvItems.Rows.Clear();
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT tbl_purchase_details.*,tbl_items.type,method, tbl_items.code, tbl_items.name FROM tbl_purchase_details INNER JOIN 
                                                                    tbl_items ON tbl_purchase_details.item_id = tbl_items.id WHERE 
                                                                    tbl_purchase_details.purchase_id = @id;",
                                                            ClinicDatabase.CreateParameterNew("id", id)))
                while (reader.Read())
                {
                    int rowIndex = dgvItems.Rows.Add(reader["item_id"].ToString(), "", reader["code"].ToString(), reader["name"].ToString(), Utilities.FormatDecimal(reader["qty"]), null, Utilities.FormatDecimal(reader["factor"]), Utilities.FormatDecimal(Convert.ToDecimal(reader["factor"]) * Convert.ToDecimal(reader["qty"])), reader["batch_no"], Convert.ToDateTime(reader["expiry_date"]),
                                                    Utilities.FormatDecimal(reader["cost_price"]), Utilities.FormatDecimal(reader["price"]), Utilities.FormatDecimal(reader["discount"]));
                    
                    DataGridViewRow row = dgvItems.Rows[rowIndex];
                    DataGridViewComboBoxCell comboCell = row.Cells["vat"] as DataGridViewComboBoxCell;
                    if (comboCell != null && reader["vat"].ToString() != "0")
                    {
                        comboCell.Value = int.Parse(reader["vat"].ToString());
                        row.Cells["vatp"].Value = decimal.Parse(reader["vatP"].ToString());
                    }
                    int itemId = Convert.ToInt32(reader["item_id"]);
                    int savedUnitId =
                            reader["item_unit_id"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["item_unit_id"]);

                    decimal savedUnitFactor =
                        reader["factor"] == DBNull.Value
                            ? 1m
                            : Convert.ToDecimal(reader["factor"]);

                    if (savedUnitFactor <= 0)
                        savedUnitFactor = 1m;
                    LoadUnitsForExistingRow(
                            row,
                            itemId,
                            savedUnitId,
                            savedUnitFactor
                        );
                    //DataGridViewComboBoxCell comboCellItemUnit = dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["unit_id"] as DataGridViewComboBoxCell;
                    //if (comboCellItemUnit != null && reader["item_unit_id"].ToString() != "0")
                    //{
                    //    comboCellItemUnit.Value = int.Parse(reader["item_unit_id"].ToString());
                    //}
                    //net_price
                    decimal netPrice = 
                        (decimal.Parse(row.Cells["qty"].Value.ToString()) 
                        * decimal.Parse(row.Cells["cost"].Value.ToString())
                        - decimal.Parse(row.Cells["discount"].Value.ToString()));
                    row.Cells["net_price"].Value = netPrice;
                    row.Cells["total"].Value = netPrice 
                        + decimal.Parse(row.Cells["vatp"].Value == null ? "0" : row.Cells["vatp"].Value.ToString());
                    row.Cells["method"].Value = reader["method"].ToString();
                    row.Cells["type"].Value = reader["type"].ToString();
                }
        }
        private void LoadUnitsForExistingRow(
              DataGridViewRow row,
              int itemId,
              int savedUnitId,
              decimal savedUnitFactor)
        {
            if (row == null)
                return;

            if (!(row.Cells["unit_id"]
                is DataGridViewComboBoxCell unitCell))
                return;

            DataTable units = GetItemUnits(itemId);

            // =========================================================
            // If an old invoice contains a UOM that is no longer active,
            // temporarily add it so the old invoice can still display.
            // =========================================================
            if (savedUnitId > 0)
            {
                bool exists = units.AsEnumerable().Any(r =>
                    Convert.ToInt32(r["id"]) == savedUnitId);

                if (!exists)
                {
                    DataTable oldUnit = ClinicDatabase.ExecuteDataTable(@"
                        SELECT
                            id AS unit_id,
                            name AS unit_name,
                            @factor AS factor,
                            0 AS is_default
                        FROM tbl_uom
                        WHERE id = @unitId
                        LIMIT 1;",
                        ClinicDatabase.CreateParameterNew(
                            "@unitId",
                            savedUnitId),
                        ClinicDatabase.CreateParameterNew(
                            "@factor",
                            savedUnitFactor > 0
                                ? savedUnitFactor
                                : 1m));

                    if (oldUnit.Rows.Count > 0)
                    {
                        DataRow newRow = units.NewRow();

                        newRow["id"] =
                            oldUnit.Rows[0]["unit_id"];

                        newRow["name"] =
                            oldUnit.Rows[0]["unit_name"];

                        newRow["factor"] =
                            oldUnit.Rows[0]["factor"];

                        newRow["is_default"] = 0;

                        units.Rows.Add(newRow);
                    }
                }
            }

            unitCell.DataSource = null;

            unitCell.DisplayMember = "name";
            unitCell.ValueMember = "id";
            unitCell.DataSource = units;

            if (units.Rows.Count == 0)
            {
                unitCell.Value = null;

                row.Cells["unit_id"].Value = 0;
                row.Cells["unit_factor"].Value = 1m;

                return;
            }

            DataRow selectedUnit = null;

            // =========================================================
            // First preference:
            // previously saved invoice UOM
            // =========================================================
            if (savedUnitId > 0)
            {
                selectedUnit = units.AsEnumerable()
                    .FirstOrDefault(r =>
                        Convert.ToInt32(r["id"])
                            == savedUnitId);
            }

            // =========================================================
            // Second preference:
            // item's reference/default UOM
            // =========================================================
            if (selectedUnit == null)
            {
                selectedUnit = units.AsEnumerable()
                    .FirstOrDefault(r =>
                        Convert.ToInt32(r["is_default"]) == 1);
            }

            // =========================================================
            // Final fallback
            // =========================================================
            if (selectedUnit == null)
                selectedUnit = units.Rows[0];

            int selectedUnitId =
                Convert.ToInt32(
                    selectedUnit["id"]);

            decimal factor;

            // Keep the historical factor from the invoice.
            if (savedUnitId == selectedUnitId &&
                savedUnitFactor > 0)
            {
                factor = savedUnitFactor;
            }
            else
            {
                factor =
                    selectedUnit["factor"] == DBNull.Value
                        ? 1m
                        : Convert.ToDecimal(
                            selectedUnit["factor"]);
            }

            if (factor <= 0)
                factor = 1m;

            unitCell.Value = selectedUnitId;

            row.Cells["unit_id"].Value =
                selectedUnitId;

            row.Cells["factor"].Value =
                factor;
        }
        private string GetUnitName(int unitId)
        {
            if (unitId <= 0)
                return "";

            object result = ClinicDatabase.ExecuteScalar(@"
        SELECT name
        FROM tbl_uom
        WHERE id = @id
        LIMIT 1;",
                ClinicDatabase.CreateParameter(
                    "@id",
                    unitId));

            if (result == null ||
                result == DBNull.Value)
            {
                return "";
            }

            return result.ToString();
        }
        private void UpdateSelectedUnitFactor(
        DataGridViewRow row,
        int unitId)
        {
            if (row == null ||
                row.IsNewRow ||
                unitId <= 0)
                return;

            object result = ClinicDatabase.ExecuteScalar(@"
        SELECT
            CASE
                WHEN uom_type = 'reference'
                    THEN 1
                ELSE IFNULL(ratio, 1)
            END
        FROM tbl_uom
        WHERE id = @id
          AND state = 0
        LIMIT 1;",
                ClinicDatabase.CreateParameter(
                    "@id",
                    unitId));

            decimal factor = 1m;

            if (result != null &&
                result != DBNull.Value)
            {
                factor =
                    Convert.ToDecimal(result);
            }

            if (factor <= 0)
                factor = 1m;

            row.Cells["unit_id"].Value =
                unitId;

            row.Cells["unit_factor"].Value =
                factor;

            decimal qty =
                GetDecimalValue(row, "qty");

            if (dgvItems.Columns.Contains("base_qty"))
            {
                row.Cells["base_qty"].Value =
                    qty * factor;
            }
        }
        private DataTable GetFilteredProductsByName(string name = "")
        {
            string option = Utilities.GeneralSettingsState("ALL ITEMS IN PURCHASE");

            string query = "select id,code,name from tbl_items where state = 0 AND active = 0";
            var parameters = new List<MySqlParameter>();

            if (string.IsNullOrEmpty(option) || int.Parse(option) <= 0)
            {
                query += " AND type = '11 - Inventory Part'";

                string purchaseType = "";
                if (cmbPurchasetype.InvokeRequired)
                {
                    cmbPurchasetype.Invoke(new Action(() =>
                    {
                        purchaseType = cmbPurchasetype.Text.Trim();
                    }));
                }
                else
                {
                    purchaseType = cmbPurchasetype.Text.Trim();
                }

                if (!string.IsNullOrEmpty(purchaseType))
                {
                    query += " AND item_type = @itemType";
                    parameters.Add(new MySqlParameter("@itemType", purchaseType));
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                query += " AND name LIKE @name";
                parameters.Add(new MySqlParameter("@name", $"%{name}%"));
            }

            query += " ORDER BY name LIMIT 20";

            return ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
        }

        private DataTable GetFilteredProductsByCode(string code = "")
        {
            string option = Utilities.GeneralSettingsState("ALL ITEMS IN PURCHASE");

            string query = "select id, code, name from tbl_items where state = 0 AND active = 0";
            var parameters = new List<MySqlParameter>();

            if (string.IsNullOrEmpty(option) || int.Parse(option) <= 0)
            {
                query += " AND type = '11 - Inventory Part'";

                string purchaseType = "";
                if (cmbPurchasetype.InvokeRequired)
                {
                    cmbPurchasetype.Invoke(new Action(() =>
                    {
                        purchaseType = cmbPurchasetype.Text.Trim();
                    }));
                }
                else
                {
                    purchaseType = cmbPurchasetype.Text.Trim();
                }

                if (!string.IsNullOrEmpty(purchaseType))
                {
                    query += " AND item_type = @itemType";
                    parameters.Add(new MySqlParameter("@itemType", purchaseType));
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                query += " AND code LIKE @code";
                parameters.Add(new MySqlParameter("@code", $"%{code}%"));
            }

            query += " ORDER BY code Limit 20";

            return ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
        }

        private void loadVat()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable("select id, concat(name,'-',value , '%') as name,value from tbl_tax");
            DataGridViewComboBoxColumn vat = (DataGridViewComboBoxColumn)dgvItems.Columns["vat"];
            vat.DataSource = dt;
            vat.DisplayMember = "name";
            vat.ValueMember = "id";
            vat.FlatStyle = FlatStyle.Popup;
            vat.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            vat.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            vat.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            var dTax = Utilities.GeneralSettings("DEFAULT TAX PERCENTAGE");
            if (!string.IsNullOrEmpty(dTax) & int.Parse(dTax) > 0)
            {
                defaultTax = dTax;
            }
        }
        string defaultTax = "";
        private void loadItemUnits(int itemId)
        {
            if (dgvItems.CurrentRow == null)
                return;

            DataGridViewRow row = dgvItems.CurrentRow;
            DataTable units = GetItemUnits(itemId);
            DataGridViewComboBoxCell unitCell = row.Cells["unit_id"] as DataGridViewComboBoxCell;

            if (unitCell == null)
                return;

            unitCell.Value = null;
            unitCell.DataSource = null;

            if (units == null || units.Rows.Count == 0)
                return;

            unitCell.DataSource = units;
            unitCell.DisplayMember = "name";
            unitCell.ValueMember = "id";
            unitCell.FlatStyle = FlatStyle.Popup;
            unitCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            unitCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataRow[] defaultUnits = units.Select("is_default = 1");

            if (defaultUnits.Length > 0)
            {
                unitCell.Value = defaultUnits[0]["id"];
            }
            else
            {
                unitCell.Value = units.Rows[0]["id"];
            }

            if (dgvItems.Columns.Contains("unit_factor"))
            {
                row.Cells["unit_factor"].Value =
                    units.Select("id = " + Convert.ToInt32(unitCell.Value))[0]["factor"];
            }
        }
        private DataTable GetItemUnits(int itemId)
        {
            return ClinicDatabase.ExecuteDataTable(@"
                    SELECT
                        u.id AS id,
                        u.name AS name,
                        CASE
                            WHEN u.uom_type = 'reference'
                                THEN 1
                            ELSE IFNULL(u.ratio, 1)
                        END AS factor,
                        CASE
                            WHEN u.id = i.unit_id
                                THEN 1
                            ELSE 0
                        END AS is_default
                    FROM tbl_items i
                    INNER JOIN tbl_uom reference_uom
                        ON reference_uom.id = i.unit_id
                       AND reference_uom.state = 0
                    INNER JOIN tbl_uom u
                        ON u.category_id = reference_uom.category_id
                       AND u.state = 0
                       AND u.active = 1
                    WHERE i.id = @itemId
                    ORDER BY
                        CASE
                            WHEN u.id = i.unit_id THEN 0
                            ELSE 1
                        END,

                        CASE
                            WHEN u.uom_type = 'reference' THEN 0
                            ELSE 1
                        END,

                        u.name;",
                ClinicDatabase.CreateParameter("@itemId", itemId));
        }
        private void bindCombo()
        {
            BindCombos.PopulateVendors(cmbVendor);
            BindCombos.PopulateAllLevel4Account(cmbAccountCashName);
            BindCombos.PopulateFixedAssetsCategories(cmbFixedAssetCategory);
            BindCombos.PopulateCostCenter(cmbCostCenter);
            cmbCostCenter.SelectedIndex = -1;
            var defaultAccounts = BindCombos.DefaultAccounts();

            cmbAccountCashName.SelectedValue = defaultAccounts.ContainsKey("Purchase Payment Cash Method")
                ? defaultAccounts["Purchase Payment Cash Method"] : 0;

            level4PaymentCreditMethodId = defaultAccounts.ContainsKey("Vendor") ? defaultAccounts["Vendor"] : 0;
            level4VatId = defaultAccounts.ContainsKey("Vat Output") ? defaultAccounts["Vat Input"] : 0;
            level4Inventory = defaultAccounts.ContainsKey("Inventory") ? defaultAccounts["Inventory"] : 0;
            level4PurchaseInvoice = defaultAccounts.ContainsKey("Purchase") ? defaultAccounts["Purchase"] : 0;
        }
        private string GenerateNextSalesCode()
        {
            string newCode = "PI-0001";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(SUBSTRING(invoice_id, 4) AS UNSIGNED)) AS lastCode FROM tbl_purchase"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = "PI-" + code.ToString("D4");
                }
            }

            return newCode;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvItems.Rows.Count == 0 || dgvItems.Rows.Count == 1 && dgvItems.Rows[0].IsNewRow)
            {
                MessageBox.Show("Can't Save Empty Purchase Invoice");
                // Optionally, you can stop the event if needed, but normally the event will end after the message.
                return;
            }

            if (!Utilities.AreDefaultAccountsSet(new List<string> { "Vendor", "Purchase", "Vat Input", "Inventory" }))
            {
                MessageBox.Show("Default accounts for invoice are not properly configured. Please check your settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (id == 0)
            {
                if (insertInvoice())
                {
                    int pId = int.Parse(invId.ToString());
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        AddFileData(pId);
                    }
                    int cId = cmbFixedAssetCategory.SelectedValue == null ? 0 : int.Parse(cmbFixedAssetCategory.SelectedValue.ToString());
                    if (pId > 0 && cId > 0)
                    {
                        frmViewFixedAssets.UpdateOrAddFixedAssets(pId, cId);
                    }
                    EventHub.RefreshPurchase();
                    if (PO == "PO")
                    {
                        EventHub.RefreshPurchaseOrder();
                    }
                    loadPrint();
                    MessageBox.Show("The Purchase Invoice  Saved");
                    dgvItems.Rows.Clear();

                }
            }
            else
            {
                if (updateInvoice())
                {
                    int pId = int.Parse(id.ToString());
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        AddFileData(pId);
                    }
                    int cId = cmbFixedAssetCategory.SelectedValue == null ? 0 : int.Parse(cmbFixedAssetCategory.SelectedValue.ToString());
                    if (pId > 0 && cId > 0)
                    {
                        frmViewFixedAssets.UpdateOrAddFixedAssets(pId, cId);
                    }
                    EventHub.RefreshPurchase();
                    loadPrint();
                    MessageBox.Show("The Purchase Invoice  Updated");
                    dgvItems.Rows.Clear();
                }
            }
        }
        private void loadPrint()
        {
            //DialogResult result = MessageBox.Show("Do You want To Show This Bill ",
            //                                   "Confirmation",
            //                                   MessageBoxButtons.YesNo,
            //                                   MessageBoxIcon.Question);
            //// Check if the user clicked Yes or No
            //if (result == DialogResult.Yes)
            //{
            //    // Code for when the user clicks "Yes"

            if (chkPrint.Checked)
            {
                ShowReport();
            }

            //    if (checkBoxReceiverNote.Checked)
            //    {
            //        frmLogin.frmMain.openChildForm(new frmReport());
            //        ShowReportReceiverNote();
            //    }

            //}
            //else if (result == DialogResult.No)
            //{
            //    // Code for when the user clicks "No"
            //    this.Close();
            //}
        }
        private bool updateInvoice()
        {
            if (!chkRequiredDate())
                return false;

            decimal paidAmount = string.IsNullOrEmpty(txtTotal.Text) ? 0 : Convert.ToDecimal(txtTotal.Text);
            decimal changeAmount = string.IsNullOrEmpty(txtTotal.Text) ? 0 : Convert.ToDecimal(txtTotal.Text);
            if (cmbPaymentMethod.Text != "Cash")
            {
                object objOldPayment = ClinicDatabase.ExecuteScalar(@"SELECT IFNULL(sum(payment),0) amount FROM tbl_payment_voucher_details WHERE inv_id =@id",
                            ClinicDatabase.CreateParameterNew("@id", id));
                decimal oldPaymentTotal = (objOldPayment != null && objOldPayment != DBNull.Value) ? decimal.Parse(objOldPayment.ToString()) : 0;
                string total = txtTotal.Text;
                if (oldPaymentTotal > 0)
                {
                    paidAmount = oldPaymentTotal;
                }
                changeAmount = decimal.Parse(total);
                paidAmount = 0;
            }
            else
            {
                changeAmount = 0;
            }

            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_purchase 
                                     SET  modified_by = @modifiedBy, modified_date = @modifiedDate ,date = @date,sales_man=@sales_man, vendor_id = @vendor_id, invoice_id = @invoice_id, warehouse_id = @warehouse_id,
                                     po_num = @po_num, bill_to = @bill_to, ship_date = @ship_date, 
                                     ship_via = @ship_via, ship_to = @ship_to, payment_method = @payment_method, account_cash_id = @account_cash_id, 
                                     payment_terms = @payment_terms, payment_date = @payment_date, total = @total, 
                                     vat = @vat, net = @net, pay = @pay, `change` = @change, city = @city,purchase_type =@purchase_type, fixed_asset_category_id=@fixed_asset_category_id, description = @description, reference_no = @reference_no, cost_center_id = @cost_center_id WHERE id = @id;",
                                     ClinicDatabase.CreateParameterNew("id", id),
                                     ClinicDatabase.CreateParameterNew("date", dtInv.Value.Date),
                                   ClinicDatabase.CreateParameterNew("vendor_id", cmbVendor.SelectedValue),
                                   ClinicDatabase.CreateParameterNew("invoice_id", txtNextCode.Text),
                                   ClinicDatabase.CreateParameterNew("city", cmbCity.Text),
                                   ClinicDatabase.CreateParameterNew("warehouse_id", cmbWarehouse.SelectedValue),
                                   ClinicDatabase.CreateParameterNew("po_num", txtPONO.Text),
                                   ClinicDatabase.CreateParameterNew("bill_to", txtBillTo.Text),
                                   ClinicDatabase.CreateParameterNew("ship_date", dtShip.Value.Date),
                                   ClinicDatabase.CreateParameterNew("sales_man", txtSalesMan.Text),
                                   ClinicDatabase.CreateParameterNew("ship_via", cmbShipVia.Text),
                                   ClinicDatabase.CreateParameterNew("ship_to", txtShipTo.Text),
                                   ClinicDatabase.CreateParameterNew("payment_method", cmbPaymentMethod.Text),
                                   ClinicDatabase.CreateParameterNew("account_cash_id", cmbAccountCashName.SelectedValue),
                                   ClinicDatabase.CreateParameterNew("payment_terms", cmbPaymentTerms.Text),
                                   ClinicDatabase.CreateParameterNew("payment_date", dtPaymentTerms.Value.Date),
                                   ClinicDatabase.CreateParameterNew("vat", TotalVatAmount),
                                   ClinicDatabase.CreateParameterNew("total", TotalBeforeAmount),
                                   ClinicDatabase.CreateParameterNew("net", TotalAmount),
                                    ClinicDatabase.CreateParameterNew("@modifiedBy", clsGlobal.CurrentUser.UserID),
                                    ClinicDatabase.CreateParameterNew("@modifiedDate", DateTime.Now.Date),
                                ClinicDatabase.CreateParameterNew("pay", paidAmount),
                                ClinicDatabase.CreateParameterNew("change", changeAmount),
                                ClinicDatabase.CreateParameterNew("purchase_type", cmbPurchasetype.Text),
                                ClinicDatabase.CreateParameterNew("description", richTextDescription.Text),
                                ClinicDatabase.CreateParameterNew("reference_no", txtRefId.Text),
                                ClinicDatabase.CreateParameterNew("cost_center_id", cmbCostCenter.SelectedValue == null ? 0 : cmbCostCenter.SelectedValue),
                                   ClinicDatabase.CreateParameterNew("fixed_asset_category_id", cmbFixedAssetCategory.SelectedValue != null ? cmbFixedAssetCategory.SelectedValue.ToString() : "0"));
            ReturnItemsToInventory();
            CommonInsert.DeleteCostCenterTransactionEntry(id.ToString(), "Purchase");
            CommonInsert.DeleteTransactionEntry(id, "PURCHASE");

            insertInvItems();

            transactions();
            //add cost center
            if (cmbCostCenter.SelectedValue != null)
                CommonInsert.InsertCostCenterTransaction(dtInv.Value, TotalAmount.ToString(), invId.ToString(), "0", "Purchase", "", cmbCostCenter.SelectedValue.ToString());

            Utilities.LogAudit(clsGlobal.CurrentUser.UserID, "Update Purchase Invoice", "Purchase", id, "Updated Purchase Invoice: " + txtNextCode.Text);

            return true;
        }

        private void ReturnItemsToInventory()
        {
            ClinicDatabase.ExecuteNonQuery(@"
                                    DELETE FROM tbl_item_batches Where id IN (
                                    SELECT batch_id from tbl_item_transaction 
                                    where type = 'Purchase Invoice' and reference = @id);

                                    DELETE FROM tbl_item_transaction  
                                    WHERE reference = @id AND type = 'Purchase Invoice';
                                    
                                    DELETE FROM tbl_item_card_details 
                                    WHERE trans_type = 'Purchase Invoice' AND trans_no = @id;
                                    
                                    DELETE FROM tbl_purchase_details 
                                    WHERE purchase_id = @id;",
                                    ClinicDatabase.CreateParameterNew("id", id));
        }

        private bool insertInvoice()
        {
            if (!chkRequiredDate())
                return false;

            invId = decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_purchase (date, vendor_id, invoice_id,warehouse_id, po_num, bill_to,city,sales_man,
               ship_date,ship_via, ship_to, payment_method,account_cash_id, payment_terms, payment_date,  
               total,vat,net, pay, `change`, created_by, created_date, state,purchase_type,fixed_asset_category_id,description,reference_no,cost_center_id) VALUES (@DATE, @vendor_id, @invoice_id,@warehouse_id,
               @po_num, @bill_to,@city,@sales_man, @ship_date, @ship_via,@ship_to, @payment_method,@account_cash_id ,@payment_terms, @payment_date, 
               @total,  @vat,@net, @pay, @change, @created_by, @created_date, @state,@purchase_type,@fixed_asset_category_id,@description,@reference_no,@cost_center_id);
               SELECT LAST_INSERT_ID();",
               ClinicDatabase.CreateParameterNew("date", dtInv.Value.Date),
               ClinicDatabase.CreateParameterNew("vendor_id", cmbVendor.SelectedValue),
               ClinicDatabase.CreateParameterNew("invoice_id", txtNextCode.Text),
               ClinicDatabase.CreateParameterNew("city", cmbCity.Text),
               ClinicDatabase.CreateParameterNew("warehouse_id", cmbWarehouse.SelectedValue),
               ClinicDatabase.CreateParameterNew("po_num", txtPONO.Text),
               ClinicDatabase.CreateParameterNew("bill_to", txtBillTo.Text),
               ClinicDatabase.CreateParameterNew("ship_date", dtShip.Value.Date),
               ClinicDatabase.CreateParameterNew("sales_man", txtSalesMan.Text),
               ClinicDatabase.CreateParameterNew("ship_via", cmbShipVia.Text),
               ClinicDatabase.CreateParameterNew("ship_to", txtShipTo.Text),
               ClinicDatabase.CreateParameterNew("payment_method", cmbPaymentMethod.Text),
               ClinicDatabase.CreateParameterNew("account_cash_id", cmbAccountCashName.SelectedValue),
               ClinicDatabase.CreateParameterNew("payment_terms", cmbPaymentTerms.Text),
               ClinicDatabase.CreateParameterNew("payment_date", dtPaymentTerms.Value.Date),
               ClinicDatabase.CreateParameterNew("vat", TotalVatAmount),
               ClinicDatabase.CreateParameterNew("total", TotalBeforeAmount),
               ClinicDatabase.CreateParameterNew("net", TotalAmount),
               ClinicDatabase.CreateParameterNew("pay", cmbPaymentMethod.Text == "Cash" ? string.IsNullOrEmpty(txtTotal.Text) ? 0 : Convert.ToDecimal(txtTotal.Text) : 0),
               ClinicDatabase.CreateParameterNew("change", cmbPaymentMethod.Text == "Cash" ? 0 : string.IsNullOrEmpty(txtTotal.Text) ? 0 : Convert.ToDecimal(txtTotal.Text)),
               ClinicDatabase.CreateParameterNew("created_by", clsGlobal.CurrentUser.UserID),
               ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date),
               ClinicDatabase.CreateParameterNew("state", 0),
               ClinicDatabase.CreateParameterNew("purchase_type", cmbPurchasetype.Text),
               ClinicDatabase.CreateParameterNew("description", richTextDescription.Text),
               ClinicDatabase.CreateParameterNew("reference_no", txtRefId.Text),
               ClinicDatabase.CreateParameterNew("cost_center_id", cmbCostCenter.SelectedValue == null ? 0 : cmbCostCenter.SelectedValue),
               ClinicDatabase.CreateParameterNew("fixed_asset_category_id", cmbFixedAssetCategory.SelectedValue != null ? cmbFixedAssetCategory.SelectedValue.ToString() : "0")).ToString());

            insertInvItems();
            txtNextCode.Text.ToString();
            txtInvoiceId.Text = invId.ToString();
            transactions();
            if (!string.IsNullOrEmpty(txtPONO.Text))
            {
                ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_purchase_order set tranfer_status=1, purchase_id=@id where id=@poId", ClinicDatabase.CreateParameterNew("poId", txtPONO.Text), ClinicDatabase.CreateParameterNew("id", invId));
            }
            //add cost center
            if (cmbCostCenter.SelectedValue != null)
                CommonInsert.InsertCostCenterTransaction(dtInv.Value, TotalAmount.ToString(), invId.ToString(), "0", "Purchase", "", cmbCostCenter.SelectedValue.ToString());

            Utilities.LogAudit(clsGlobal.CurrentUser.UserID, "Add Purchase Invoice", "Purchase", (int)invId, "Added Purchase Invoice: " + txtNextCode.Text);

            return true;
        }

        private void insertInvItems()
        {
            //var valueList = new List<string>();
            //var parameters = new List<MySqlParameter>();
            //int paramIndex = 0;

            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                var row = dgvItems.Rows[i];
                if (row.IsNewRow || string.IsNullOrWhiteSpace(row.Cells["itemId"].Value?.ToString()))
                    continue;

                var parameters = new List<MySqlParameter>();
                string itemId = row.Cells["itemId"].Value.ToString();
                decimal qty = row.Cells["qty"].Value == null ? 0 : Convert.ToDecimal(row.Cells["qty"].Value);
                string batch = row.Cells["batch_no"].Value == null ? "" : row.Cells["batch_no"].Value.ToString();
                DateTime expiry = Convert.ToDateTime(row.Cells["expiry_date"].Value);

                // Prepare parameterized values for each row
                //valueList.Add($"(@purchase_id, @item_id{paramIndex}, @qty{paramIndex}, @cost_price{paramIndex}, @price{paramIndex}, @discount{paramIndex}, @vatp{paramIndex}, @vat{paramIndex}, @total{paramIndex}, @unitName{paramIndex}, @factor{paramIndex}, @batch{paramIndex}, @expiry{paramIndex})");
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@item_id{paramIndex}", itemId));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@qty{paramIndex}", qty));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@cost_price{paramIndex}", row.Cells["cost"].Value?.ToString()));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@price{paramIndex}", row.Cells["price"].Value?.ToString()));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@discount{paramIndex}", row.Cells["discount"].Value?.ToString()));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@vat{paramIndex}", row.Cells["vat"].Value?.ToString()));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@vatp{paramIndex}", row.Cells["vatp"].Value?.ToString()));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@total{paramIndex}", row.Cells["total"].Value?.ToString()));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@unitName{paramIndex}", row.Cells["unit_id"].Value ?? 0));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@factor{paramIndex}", row.Cells["factor"].Value ?? 0));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@batch{paramIndex}", batch));
                //parameters.Add(ClinicDatabase.CreateParameterNew($"@expiry{paramIndex}", expiry.Date));
                //paramIndex++;
                parameters.Add(ClinicDatabase.CreateParameterNew($"@purchase_id", invId));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@item_id", itemId));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@qty", qty));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@cost_price", row.Cells["cost"].Value?.ToString()));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@price", row.Cells["price"].Value?.ToString()));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@discount", row.Cells["discount"].Value?.ToString()));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@vat", row.Cells["vat"].Value?.ToString()));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@vatp", row.Cells["vatp"].Value?.ToString()));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@total", row.Cells["total"].Value?.ToString()));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@unitName", row.Cells["unit_id"].Value ?? 0));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@factor", row.Cells["factor"].Value ?? 0));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@batch", batch));
                parameters.Add(ClinicDatabase.CreateParameterNew($"@expiry", expiry.Date));
                string sql = $@"
                                INSERT INTO tbl_purchase_details
                                (purchase_id, item_id, qty, cost_price, price, discount, vatp, vat, total, item_unit_id,factor,batch_no,expiry_date)
                                VALUES (@purchase_id, @item_id, @qty, @cost_price, @price, @discount, @vatp, @vat, @total, @unitName, @factor, @batch, @expiry);";
                ClinicDatabase.ExecuteNonQuery(sql, parameters.ToArray());
            }

            //if (valueList.Count == 0)
            //    return;

            //string sql = $@"
            //                INSERT INTO tbl_purchase_details
            //                (purchase_id, item_id, qty, cost_price, price, discount, vatp, vat, total, item_unit_id,factor,batch_no,expiry_date)
            //                VALUES {string.Join(", ", valueList)};";

            //using (var conn = ClinicDatabase.GetConnection())
            //{
            //    conn.Open();
            //    using (var trans = conn.BeginTransaction())
            //    using (var cmd = new MySqlCommand(sql, conn, trans))
            //    {
            //        cmd.Parameters.AddWithValue("@purchase_id", invId);
            //        foreach (var p in parameters)
            //            cmd.Parameters.Add(p);

            //        cmd.ExecuteNonQuery();
            //        trans.Commit();
            //    }
            //}

            // Inventory and transaction updates (still per item)
            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                var row = dgvItems.Rows[i];
                if (row.IsNewRow || string.IsNullOrWhiteSpace(row.Cells["itemId"].Value?.ToString()))
                    continue;

                string itemId = row.Cells["itemId"].Value.ToString();
                decimal qty = row.Cells["qty"].Value == DBNull.Value ? 0 : Convert.ToDecimal(row.Cells["qty"].Value);

                if (row.Cells["type"].Value.ToString() != "12 - Service")
                {
                    insertItemTransaction(row);
                }
            }
        }

        private void insertItemTransaction(DataGridViewRow row)
        {
            decimal qty = 0;

            if (row.Cells["itemId"].Value == null || string.IsNullOrWhiteSpace(row.Cells["itemId"].Value.ToString()))
            {
                MessageBox.Show("Invalid Item ID.");
                return;
            }

            if (!decimal.TryParse(row.Cells["qty"].Value.ToString(), out qty) || qty <= 0)
            {
                MessageBox.Show("Invalid Quantity.");
                return;
            }
            // batchId
            int itemId = Convert.ToInt32(row.Cells["itemid"].Value);
            string batch = row.Cells["batch_no"].Value.ToString();
            DateTime expiry = Convert.ToDateTime(row.Cells["expiry_date"].Value);

            int unitId = Convert.ToInt32(row.Cells["unit_id"].Value);
            decimal factor = Convert.ToDecimal(row.Cells["factor"].Value);
            decimal baseQty = qty * factor;
            decimal baseCost = row.Cells["cost"].Value == DBNull.Value ? 0 : Convert.ToDecimal(row.Cells["cost"].Value) / factor;

            int batchId = Convert.ToInt32(ClinicDatabase.ExecuteScalar(@"
                INSERT INTO tbl_item_batches
                (item_id, batch_no, expiry_date,cost_price,sale_price,warehouse_id, qty_in, qty_out, base_cost, factor,created_date)
                VALUES
                (@itemId, @batch, @expiry,@cost,@salePrice,@warwhouseId, @qty, 0,@baseCost,@factor,@date);
                SELECT LAST_INSERT_ID();",
                new MySqlParameter("@itemId", itemId),
                new MySqlParameter("@batch", batch),
                new MySqlParameter("@expiry", expiry),
                new MySqlParameter("@cost", row.Cells["cost"].Value),
                new MySqlParameter("@baseCost", baseCost),
                new MySqlParameter("@salePrice", row.Cells["price"].Value),
                new MySqlParameter("@warwhouseId", cmbWarehouse.SelectedValue),
                new MySqlParameter("@date", DateTime.Now),
                new MySqlParameter("@qty", baseQty),
                new MySqlParameter("@factor", factor)

            ));
            // Insert the item transaction for the purchase=
            decimal finalQty = baseQty > 0 ? baseQty : qty;
            CommonInsert.InsertItemTransaction(dtInv.Value.Date, "Purchase Invoice", invId.ToString(), row.Cells["itemId"].Value.ToString(),
                row.Cells["cost"].Value.ToString(), finalQty.ToString(), "0", "0", row.Cells["qty"].Value.ToString(),
                "Purchase Invoice No. " + txtNextCode.Text, cmbWarehouse.SelectedValue.ToString(), batchId.ToString(), unitId.ToString(), factor.ToString(), qty.ToString());
        }

        private void transactions()
        {
            string accountId = cmbPaymentMethod.Text == "Credit" ? level4PaymentCreditMethodId.ToString()
                 : cmbAccountCashName.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(txtTotal.Text))
            {
                CommonInsert.addTransactionEntry(dtInv.Value.Date,
                     accountId,
                      "0", TotalAmount.ToString(), invId.ToString(), cmbVendor.SelectedValue.ToString(), cmbPaymentMethod.Text == "Credit" ? "Purchase Invoice" : "Purchase Invoice Cash", "PURCHASE", "Purchase Invoice NO. " + txtNextCode.Text,
                     clsGlobal.CurrentUser.UserID, DateTime.Now.Date, txtNextCode.Text);
            }
            if (TotalVatAmount > 0)
            {
                CommonInsert.addTransactionEntry(dtInv.Value.Date,
              level4VatId.ToString(), TotalVatAmount.ToString(), "0", invId.ToString(), "0", "Purchase Invoice", "PURCHASE",
              "Vat Input For Invoice No. " + txtNextCode.Text, clsGlobal.CurrentUser.UserID, DateTime.Now.Date, txtNextCode.Text);
            }
            if (TotalBeforeAmount > 0)
            {
                CommonInsert.addTransactionEntry(dtInv.Value.Date,
                  level4Inventory.ToString(), TotalBeforeAmount.ToString(), "0", invId.ToString(), "0", "Purchase Invoice", "PURCHASE",
                  "Purchase For Invoice No. " + txtNextCode.Text, clsGlobal.CurrentUser.UserID, DateTime.Now.Date, txtNextCode.Text);
            }
        }
        private bool chkRequiredDate()
        {
            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                if (dgvItems.Rows[i].Cells["total"].Value == null
                    || dgvItems.Rows[i].Cells["total"].Value.ToString() == ""
                    || decimal.Parse(dgvItems.Rows[i].Cells["total"].Value.ToString()) == 0)
                {
                    MessageBox.Show("Total Item In Row " + (dgvItems.Rows[i].Index + 1) + " Can't Be 0 or Null");
                    return false;
                }
                int invAcId = GetLevel4AccountIdByItemId(dgvItems.Rows[i].Cells["itemId"].Value.ToString());
                level4Inventory = invAcId > 0 ? invAcId : level4Inventory;
            }
            if (cmbVendor.SelectedValue == null)
            {
                MessageBox.Show("Vendor Must be Selected.");
                txtVendorCode.Focus();
                return false;
            }
            if (cmbAccountCashName.SelectedValue == null)
            {
                MessageBox.Show("Account Cash Name Must be Selected.");
                cmbAccountCashName.Focus();
                return false;
            }
            if (cmbPaymentMethod.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Payment Method Must be Selected.");
                return false;
            }
            if (cmbWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Warehouse Must be Selected.");
                return false;
            }
            if (cmbAccountCashName.SelectedValue == null)
            {
                MessageBox.Show("Cash Account Must be Selected.");
                return false;
            }
            if (dgvItems.Rows.Count == 1)
            {
                MessageBox.Show("Insert Items First.");
                return false;
            }
            if (txtTotal.Text == "" || decimal.Parse(txtTotal.Text) == 0)
            {
                MessageBox.Show("Total Must Be Bigger Than Zero");
                return false;
            }
            if (level4PaymentCreditMethodId <= 0 && level4VatId <= 0 && level4PurchaseInvoice <= 0 && level4Inventory <= 0)
            {
                MessageBox.Show("Default accounts for invoice are not properly configured. Please check your settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private int GetLevel4AccountIdByItemId(string v)
        {
            object objInvAcId = ClinicDatabase.ExecuteScalar(@"SELECT asset_account_id FROM tbl_items WHERE id =@id",
                            ClinicDatabase.CreateParameterNew("@id", v));
            int invAcId = (objInvAcId != null && objInvAcId != DBNull.Value) ? int.Parse(objInvAcId.ToString()) : 0;
            return invAcId;
        }

        private void resetTextBox()
        {
            txtSalesMan.Text = txtBillTo.Text = txtShipTo.Text = txtPONO.Text =
                txtTotal.Text = txtTotalVat.Text = "";
            txtInvoiceId.Text = GetNextPurchaseId();
            cmbPaymentMethod.SelectedIndex = id = 0;
            dtInv.Value = DateTime.Now;
            dgvItems.Rows.Clear();
        }
        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            //if (id == 0)
            //{
            //    if (insertInvoice())
            //    {
            //        int pId = int.Parse(invId.ToString());
            //        int cId = cmbFixedAssetCategory.SelectedValue == null ? 0 : int.Parse(cmbFixedAssetCategory.SelectedValue.ToString());
            //        if (pId > 0 && cId > 0)
            //        {
            //            frmViewFixedAssets.UpdateOrAddFixedAssets(pId, cId);
            //        }
            //        resetTextBox();
            //    }
            //}

            if (!Utilities.AreDefaultAccountsSet(new List<string> { "Vendor", "Purchase", "Vat Input", "Inventory" }))
            {
                MessageBox.Show("Default accounts for invoice are not properly configured. Please check your settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (id == 0)
            {
                if (insertInvoice())
                {
                    int pId = int.Parse(invId.ToString());
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        AddFileData(pId);
                    }
                    int cId = cmbFixedAssetCategory.SelectedValue == null ? 0 : int.Parse(cmbFixedAssetCategory.SelectedValue.ToString());
                    if (pId > 0 && cId > 0)
                    {
                        frmViewFixedAssets.UpdateOrAddFixedAssets(pId, cId);
                    }
                    EventHub.RefreshPurchase();
                    if (PO == "PO")
                    {
                        EventHub.RefreshPurchaseOrder();
                    }
                    loadPrint();
                    this.Close();
                }
            }
            else
            {
                if (updateInvoice())
                {
                    int pId = int.Parse(invId.ToString());
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        AddFileData(pId);
                    }
                    int cId = cmbFixedAssetCategory.SelectedValue == null ? 0 : int.Parse(cmbFixedAssetCategory.SelectedValue.ToString());
                    if (pId > 0 && cId > 0)
                    {
                        frmViewFixedAssets.UpdateOrAddFixedAssets(pId, cId);
                    }
                    EventHub.RefreshPurchase();
                    loadPrint();
                    this.Close();
                }
            }

        }
        private void AddFileData(int fId)
        {
            AttachmentManager.SaveAttachment(
                    "PURCHASE INVOICE",
                    fId,
                    openFileDialog1.FileName
                );
        }
        private void txtSalesPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }
        private void cmbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVendor.SelectedValue == null)
            {
                txtBillTo.Text = txtVendorCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select code from tbl_vendor where id = @id", ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue.ToString())))
                if (reader.Read())
                {
                    txtVendorCode.Text = reader["code"].ToString();
                    txtBillTo.Text = cmbVendor.Text;
                }
                else
                    txtBillTo.Text = txtVendorCode.Text = "";
        }
        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMethod.Text == "Cash")
            {
                cmbPaymentTerms.SelectedIndex = -1;
                cmbAccountCashName.SelectedValue = BindCombos.SelectDefaultLevelAccount("Purchase Payment Cash Method");
                cmbAccountCashName.Enabled = true;
                cmbPaymentTerms.Enabled = false;
            }
            else if (cmbPaymentMethod.Text == "Credit")
            {
                cmbPaymentTerms.SelectedIndex = 0;
                cmbAccountCashName.Enabled = false;
                cmbAccountCashName.SelectedValue = level4PaymentCreditMethodId;
                cmbPaymentTerms.Enabled = true;
            }
        }

        private void txtVendorCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id from tbl_vendor where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtVendorCode.Text)))
                if (reader.Read())
                    cmbVendor.SelectedValue = int.Parse(reader["id"].ToString());

            if (txtVendorCode.Focused)
            {
                string input = txtVendorCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_vendor 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["code"].ToString(),
                        Name = row["name"].ToString()
                    });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtVendorCode.Parent.PointToScreen(txtVendorCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtVendorCode.Height,
                        txtVendorCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtVendorCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void txtVendorCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id from tbl_vendor where code =@code",
            ClinicDatabase.CreateParameterNew("code", txtVendorCode.Text)))
                if (!reader.Read())
                    cmbVendor.SelectedIndex = -1;

            BeginInvoke((Action)(() =>
            {
                if (!lstAccountSuggestions.Focused)
                    lstAccountSuggestions.Visible = false;
            }));
        }

        private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int colIndex = dgvItems.CurrentCell.ColumnIndex;

            if (e.Control is TextBox textBox0)
            {
                textBox0.TextChanged -= ItemCode_TextChanged;
                textBox0.TextChanged -= ItemName_TextChanged;
                textBox0.KeyPress -= NumericColumn_KeyPress;

                if (lstAccountSuggestions.Visible)
                    lstAccountSuggestions.Visible = false;

                lstAccountSuggestions.SendToBack();
            }

            if (colIndex == dgvItems.Columns["name"].Index)
            {
                if (e.Control is TextBox textBox)
                {
                    textBox.TextChanged -= ItemName_TextChanged;
                    textBox.TextChanged += ItemName_TextChanged;

                    lstAccountSuggestions.Tag = textBox;
                }
            }
            else if (colIndex == dgvItems.Columns["unit_id"].Index)
            {
                if (e.Control is ComboBox comboBox)
                {
                    comboBox.SelectedIndexChanged -= new EventHandler(ComboBoxUnitName_SelectedIndexChanged);
                    comboBox.SelectedIndexChanged += new EventHandler(ComboBoxUnitName_SelectedIndexChanged);
                }
            }
            else if (colIndex == dgvItems.Columns["vat"].Index)
            {
                if (e.Control is ComboBox comboBox)
                {
                    comboBox.SelectedIndexChanged -= new EventHandler(ComboBoxTax_SelectedIndexChanged);
                    comboBox.SelectedIndexChanged += new EventHandler(ComboBoxTax_SelectedIndexChanged);
                }
            }
            else if (colIndex == dgvItems.Columns["code"].Index)
            {
                if (e.Control is TextBox textBox)
                {
                    textBox.TextChanged -= ItemCode_TextChanged;
                    textBox.TextChanged += ItemCode_TextChanged;

                    lstAccountSuggestions.Tag = textBox;
                }
            }
            else if (dgvItems.CurrentCell.OwningColumn.Name == "qty" ||
                     dgvItems.CurrentCell.OwningColumn.Name == "cost" ||
                     dgvItems.CurrentCell.OwningColumn.Name == "discount")
            {
                if (e.Control is TextBox textBox)
                {
                    textBox.KeyPress -= NumericColumn_KeyPress;
                    textBox.KeyPress += NumericColumn_KeyPress;
                }
            }
            else
            {
                lstAccountSuggestions.Visible = false;
                lstAccountSuggestions.Tag = null;
            }
        }

        private void NumericColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // block anything that's not a digit or decimal point
            }

            // Allow only one decimal point
            TextBox txt = sender as TextBox;
            if (e.KeyChar == '.' && txt.Text.Contains("."))
            {
                e.Handled = true;
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
                lstAccountSuggestions.Items.Add(new ItemSuggestion
                {
                    Id = Convert.ToInt32(row["id"]),
                    Code = row["code"].ToString(),
                    Name = row["name"].ToString()
                });
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
                lstAccountSuggestions.Items.Add(new ItemSuggestion
                {
                    Id = Convert.ToInt32(row["id"]),
                    Code = row["code"].ToString(),
                    Name = row["name"].ToString()
                });
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

        private void ComboBoxUnitName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is DataRowView selectedRow)
            {
                if (dgvItems.CurrentCell == null || dgvItems.CurrentCell.ColumnIndex != dgvItems.Columns["unit_id"].Index)
                    return;

                var row = dgvItems.CurrentRow;
                if (row == null || row.IsNewRow)
                    return;

                int unitId = Convert.ToInt32(selectedRow["id"]);
                //string unitName = selectedRow["name"].ToString();

                decimal qty = GetDecimalValue(row, "qty");
                decimal factor = GetUnitFactor(Convert.ToInt32(row.Cells["itemId"].Value), unitId);
                row.Cells["factor"].Value = factor;
                decimal baseQty = qty * factor;
                row.Cells["base_qty"].Value = baseQty;
            }
        }
        private decimal GetUnitFactor(int itemId, int unitId)
        {
            if (unitId <= 0)
                return 1m;

            object result = ClinicDatabase.ExecuteScalar(@"
                    SELECT
                        CASE
                            WHEN LOWER(TRIM(uom_type)) = 'reference'
                                THEN 1
                            ELSE IFNULL(ratio, 1)
                        END
                    FROM tbl_uom
                    WHERE id = @id AND IFNULL(state, 0) = 0 AND IFNULL(active, 1) = 1
                    LIMIT 1;", ClinicDatabase.CreateParameter("@id", unitId)
            );

            if (result == null || result == DBNull.Value)
                return 1m;

            decimal factor;

            if (!decimal.TryParse(result.ToString(), out factor))
            {
                return 1m;
            }

            return factor > 0 ? factor : 1m;
        }
        private void ComboBoxTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is DataRowView selectedRow)
            {
                if (dgvItems.CurrentCell == null || dgvItems.CurrentCell.ColumnIndex != dgvItems.Columns["vat"].Index)
                    return;

                var row = dgvItems.CurrentRow;
                if (row == null || row.IsNewRow)
                    return;

                string taxId = selectedRow["id"].ToString();
                string taxP = selectedRow["value"].ToString();

                decimal price = GetDecimalValue(row, "cost");
                decimal qty = GetDecimalValue(row, "qty");
                decimal discount = GetDecimalValue(row, "discount");

                if (price == 0 || qty == 0)
                {
                    row.Cells["total"].Value = row.Cells["discount"].Value = row.Cells["vatp"].Value = "0";
                }
                else
                {
                    decimal netPrice = (qty * price) - discount;
                    row.Cells["net_price"].Value = netPrice;
                    row.Cells["vat"].Value = Convert.ToInt32(taxId);
                    row.Cells["vatp"].Value = (decimal.Parse(netPrice.ToString()) * decimal.Parse(taxP) / 100);
                    row.Cells["total"].Value = netPrice + decimal.Parse(row.Cells["vatp"].Value.ToString());
                }
            }
            //try
            //{
            //    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgvItems.CurrentRow.Cells["vat"];
            //    if (dgvItems.CurrentCell.ColumnIndex != dgvItems.Columns["vat"].Index)
            //        return;
            //    if (comboCell != null)
            //    {
            //        ComboBox comboBox = sender as ComboBox;
            //        DataRowView dRow = (DataRowView)comboBox.SelectedItem;

            //        if (dRow != null)
            //        {
            //            var taxDetails = dRow.Row.ItemArray;
            //            string taxId = dRow.Row.ItemArray[0].ToString();
            //            string taxName = dRow.Row.ItemArray[1].ToString();
            //            string taxP = taxDetails[2].ToString();
            //            decimal price = GetDecimalValue(dgvItems.CurrentRow, "cost");
            //            decimal qty = GetDecimalValue(dgvItems.CurrentRow, "qty");
            //            decimal discount = GetDecimalValue(dgvItems.CurrentRow, "discount");

            //            if (price == 0 || qty == 0)
            //                dgvItems.CurrentRow.Cells["total"].Value = dgvItems.CurrentRow.Cells["discount"].Value = dgvItems.CurrentRow.Cells["vatp"].Value = "0";
            //            else
            //            {
            //                decimal netPrice = (qty * price) - discount;
            //                dgvItems.CurrentRow.Cells["net_price"].Value = netPrice;

            //                dgvItems.CurrentRow.Cells["vatp"].Value = (decimal.Parse(netPrice.ToString()) * decimal.Parse(taxP) / 100);
            //            }
            //        }
            //    }
            //    else
            //        dgvItems.CurrentRow.Cells["vatp"].Value = "0";

            //    ChkRowValidity();
            //}
            //catch (Exception ex)
            //{
            //    ex.ToString();
            //}
        }

        private void ChkRowValidity(DataGridViewRow row)
        {
            if (row == null || row.IsNewRow)
                return;

            decimal price = GetDecimalValue(row, "cost");
            decimal qty = GetDecimalValue(row, "qty");
            decimal discount = GetDecimalValue(row, "discount");

            if (price == 0 || qty == 0)
            {
                row.Cells["total"].Value = row.Cells["discount"].Value = row.Cells["vatp"].Value = "0";
            }
            else
            {
                try
                {
                    decimal netPrice = (qty * price) - discount;
                    row.Cells["net_price"].Value = netPrice;

                    string vatValue = row.Cells["vat"].Value?.ToString() ?? "0";
                    string vatPercentage = "0";

                    DataTable vatTable = ((DataGridViewComboBoxColumn)dgvItems.Columns["vat"]).DataSource as DataTable;
                    if (vatTable != null)
                    {
                        DataRow[] vatRows = vatTable.Select($"id = '{vatValue}'");
                        if (vatRows.Length > 0)
                        {
                            vatPercentage = vatRows[0]["value"].ToString();
                        }
                    }

                    decimal vatAmount = (netPrice * Convert.ToDecimal(vatPercentage)) / 100;
                    row.Cells["vatp"].Value = vatAmount;
                    row.Cells["total"].Value = netPrice + vatAmount;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("VAT calc error: " + ex.Message);
                }
            }
            //decimal price = GetDecimalValue(dgvItems.CurrentRow, "cost");
            //decimal qty = GetDecimalValue(dgvItems.CurrentRow, "qty");
            //decimal discount = GetDecimalValue(dgvItems.CurrentRow, "discount");

            //if (price == 0 || qty == 0)
            //    dgvItems.CurrentRow.Cells["total"].Value = dgvItems.CurrentRow.Cells["discount"].Value = dgvItems.CurrentRow.Cells["vatp"].Value = "0";
            //else
            //{
            //    try
            //    {
            //        decimal netPrice = (qty * price) - discount;
            //        dgvItems.CurrentRow.Cells["net_price"].Value = netPrice;

            //        DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgvItems.CurrentRow.Cells["vat"];
            //        if (comboCell != null && comboCell.Value != null)
            //        {
            //            string vP = "0";
            //            foreach (var item in comboCell.Items)
            //            {
            //                DataRowView dr = (DataRowView)item;
            //                string id = dr["id"].ToString();
            //                var name = dr["name"];
            //                var value = dr["value"];
            //                if (dgvItems.CurrentRow.Cells["vat"].Value.ToString() == id)
            //                {
            //                    vP = value.ToString();
            //                }
            //            }

            //            dgvItems.CurrentRow.Cells["vatp"].Value = (decimal.Parse(netPrice.ToString()) * decimal.Parse(vP) / 100);
            //        }
            //        else
            //            dgvItems.CurrentRow.Cells["vatp"].Value = "0";

            //        dgvItems.CurrentRow.Cells["total"].Value = netPrice + decimal.Parse(dgvItems.CurrentRow.Cells["vatp"].Value.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.ToString();
            //    }
            //}
        }

        private void RefreshItems()
        {
            //DataTable dt = ClinicDatabase.ExecuteDataTable("select code,name from tbl_items where state = 0 and active = 0");
            //DataGridViewComboBoxColumn name = (DataGridViewComboBoxColumn)dgvItems.Columns["name"];
            //DataRow newRow = dt.NewRow();
            //newRow["code"] = 0;
            //newRow["name"] = "<< Add New Item >>";
            //dt.Rows.InsertAt(newRow, 0);
            //name.DataSource = dt;
            //name.DisplayMember = "name";
            //name.ValueMember = "code";
        }

        bool CheckItemValidity(int itemId)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select type from tbl_items where id = @id",
                ClinicDatabase.CreateParameterNew("id", itemId)))
            {
                reader.Read();
                if (reader["type"].ToString() == "Service")
                    return true;
            }
            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                if (dgvItems.Rows[i].Cells["itemId"].Value == null)
                {
                    dgvItems.Rows.Remove(dgvItems.Rows[i]);
                    continue;
                }
            }
            return true;
        }
        void CalculateTotal()
        {
            decimal total = 0, vat = 0;
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.Cells["total"].Value != null)
                    total += Convert.ToDecimal(row.Cells["total"].Value);
                if (row.Cells["vatp"].Value != null && row.Cells["vatp"].Value.ToString().Trim() != "")
                    vat += Convert.ToDecimal(row.Cells["vatp"].Value);

            }
            TotalBeforeAmount = (total - vat);
            txtTotalBefore.Text = TotalBeforeAmount.ToString("N2");
            TotalVatAmount = Convert.ToDecimal(vat.ToString("N2"));
            txtTotalVat.Text = TotalVatAmount.ToString("N2");
            TotalAmount = Convert.ToDecimal(total.ToString("N2"));
            txtTotal.Text = TotalAmount.ToString("N2");
            FormatNumberWithCommas(txtTotalBefore);
            FormatNumberWithCommas(txtTotalVat);
            FormatNumberWithCommas(txtTotal);
        }
        decimal TotalBeforeAmount = 0, TotalVatAmount = 0, TotalAmount = 0;
        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (suppressCellValueChanged) return;

            if (dgvItems.Rows.Count > 1)
            {
                var row = dgvItems.Rows[e.RowIndex];
                decimal price = GetDecimalValue(row, "price");
                decimal qty = GetDecimalValue(row, "qty");
                decimal costPrice = GetDecimalValue(row, "cost");
                decimal discount = GetDecimalValue(row, "discount");
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
                else if (e.ColumnIndex == dgvItems.Columns["qty"].Index)
                {
                    if (dgvItems.CurrentRow.Cells["itemId"].Value == null || !CheckItemValidity(int.Parse(dgvItems.CurrentRow.Cells["itemId"].Value.ToString())))
                        row.Cells["qty"].Value = 0;
                }
                ChkRowValidity(row);
                CalculateTotal();
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
                    reader = ClinicDatabase.ExecuteReader(@"
                            SELECT id, method, type, code, sales_price, cost_price, name 
                            FROM tbl_items 
                            WHERE code = @code",
                        ClinicDatabase.CreateParameterNew("code", inputText.Trim()));
                }
                else if (type == "name")
                {
                    // Search by name (case-insensitive)
                    reader = ClinicDatabase.ExecuteReader(@"
                                SELECT id, method, type, code, sales_price, cost_price, name 
                                FROM tbl_items 
                                WHERE name = @name COLLATE utf8mb4_general_ci LIMIT 1",
                        ClinicDatabase.CreateParameterNew("name", inputText.Trim()));
                }

                if (reader != null && reader.Read())
                {
                    dgvItems.CurrentRow.Cells["qty"].Value = 0;
                    dgvItems.CurrentRow.Cells["discount"].Value = 0;
                    dgvItems.CurrentRow.Cells["itemId"].Value = reader["id"].ToString();
                    dgvItems.CurrentRow.Cells["cost"].Value = Convert.ToDecimal(reader["cost_price"]).ToString("N2");
                    dgvItems.CurrentRow.Cells["price"].Value = Convert.ToDecimal(reader["sales_price"]).ToString("N2");
                    dgvItems.CurrentRow.Cells["method"].Value = reader["method"];
                    dgvItems.CurrentRow.Cells["type"].Value = reader["type"];
                    dgvItems.CurrentRow.Cells["code"].Value = reader["code"].ToString();
                    dgvItems.CurrentRow.Cells["name"].Value = reader["name"].ToString();

                    // Optional default VAT
                    if (!string.IsNullOrEmpty(defaultTax))
                    {
                        dgvItems.CurrentRow.Cells["vat"].Value = int.Parse(defaultTax);
                    }
                }
                else
                {
                    DataGridViewRow row = dgvItems.CurrentRow;
                    if (row != null)
                    {
                        row.Cells["itemid"].Value = null;
                        row.Cells["cost"].Value = null;
                        row.Cells["price"].Value = null;
                        row.Cells["method"].Value = null;
                        row.Cells["type"].Value = null;
                        row.Cells["vat"].Value = null;
                        if (type == "name")
                            row.Cells["code"].Value = null;
                        if (type == "code")
                            row.Cells["name"].Value = null;
                    }
                }
            }
            finally
            {
                reader?.Close();
                reader?.Dispose();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtInvoiceId.Text);//Utilities.GetVoucherIdFromCode(txtInvoiceId.Text);
            if (currentId == null || currentId <= 1)
                return;

            int previousId = currentId.Value - 1;

            // keep searching backwards until we either find a record or hit the first id
            while (previousId > 0)
            {
                string query = "SELECT id FROM tbl_purchase WHERE state = 0 AND id = @id";
                using (var reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("id", previousId)))
                {
                    if (reader.Read())
                    {
                        id = int.Parse(reader["id"].ToString());
                        BindInvoice();   // found valid invoice
                        return;
                    }
                }

                previousId--; // try the next one back
            }

            // if nothing was found at all
            clear();
            MessageBox.Show("No previous records.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtInvoiceId.Text);// Utilities.GetVoucherIdFromCode(txtInvoiceId.Text);
            if (currentId is null) return;

            currentId = currentId + 1;
            string query = "SELECT id FROM tbl_purchase WHERE state = 0 AND id = @id";

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("id", currentId)))
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindInvoice();
                }
                else
                {
                    clear();
                    MessageBox.Show("No next records.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        private void clear()
        {
            resetTextBox();
            dgvItems.Rows.Clear();
            txtInvoiceId.Text = GetNextPurchaseId();
            id = 0;
            invId = 0;
            cmbVendor.SelectedIndex = -1;
            cmbWarehouse.SelectedIndex = -1;
            cmbPaymentMethod.SelectedIndex = -1;
            cmbAccountCashName.SelectedIndex = -1;
            cmbShipVia.SelectedIndex = -1;
            dtInv.Value = DateTime.Now.Date;
            txtNextCode.Text = GenerateNextSalesCode();
        }

        private string GetNextPurchaseId()
        {
            string newCode = "1";
            using (var reader = ClinicDatabase.ExecuteReader("SELECT IFNULL(MAX(id),0) AS lastCode FROM tbl_purchase"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = code.ToString();
                }
            }

            return newCode;
        }

        private void lnkNewVendor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmViewVendor().ShowDialog();
        }

        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvItems.Rows.Count > 1 && dgvItems.CurrentRow.Cells["delete"].ColumnIndex == e.ColumnIndex)
                {
                    dgvItems.Rows.Remove(dgvItems.CurrentRow);
                    CalculateTotal();
                }
                if (e.ColumnIndex == dgvItems.Columns["vat"].Index)
                {
                    ChkRowValidity(dgvItems.CurrentRow);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void cmbAccountCashName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountCashName.Focused && int.Parse(cmbAccountCashName.SelectedValue.ToString()) == -1)
            {
                new frmAddAccount().ShowDialog();
            }
            else
            {
            }
        }

        private void dgvItems_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvItems.Rows[e.RowIndex].Cells[1].Value = (e.RowIndex + 1).ToString();
        }


        private void cmbPaymentTerms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMethod.Text == "Credit")
            {
                dtPaymentTerms.Value = dtPaymentTerms.Value.AddDays(int.Parse(cmbPaymentTerms.Text));
            }
            else
                dtPaymentTerms.Value = DateTime.Now.Date;
        }

        private void loadPurchaseOrderData()
        {
            //get purchase order
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(" select * from tbl_purchase_order where id = @id",
                ClinicDatabase.CreateParameterNew("id", purchaseOrderId)))
                if (reader.Read())
                {
                    txtPONO.Text = reader["id"].ToString();

                    cmbVendor.SelectedValue = reader["vendor_id"].ToString();
                    cmbPaymentMethod.Text = reader["payment_method"].ToString();
                    if (reader["payment_method"].ToString() == "Cash")
                        cmbAccountCashName.SelectedValue = reader["account_cash_id"].ToString();
                    else
                    {
                        cmbPaymentTerms.Text = reader["payment_terms"].ToString();
                        dtPaymentTerms.Value = DateTime.Parse(reader["payment_date"].ToString());
                    }
                    cmbShipVia.Text = reader["ship_via"].ToString();
                    richTextDescription.Text = reader["description"].ToString();
                    cmbWarehouse.SelectedValue = reader["warehouse_id"].ToString();
                    invId = id;
                    txtInvoiceId.Text = invId.ToString();
                    txtBillTo.Text = reader["bill_to"].ToString();
                    txtShipTo.Text = reader["ship_to"].ToString();

                    dgvItems.Rows.Clear();

                    using (MySqlDataReader readerDetails = ClinicDatabase.ExecuteReader(@"SELECT tbl_purchase_order_details.*,tbl_items.type,method, tbl_items.code, tbl_items.name FROM tbl_purchase_order_details INNER JOIN 
                                                                    tbl_items ON tbl_purchase_order_details.item_id = tbl_items.id WHERE 
                                                                    tbl_purchase_order_details.purchase_id = @id;",
                                                                    ClinicDatabase.CreateParameterNew("id", purchaseOrderId)))
                        while (readerDetails.Read())
                        {
                            decimal netPrice = Convert.ToDecimal(readerDetails["qty"].ToString()) * Convert.ToDecimal(readerDetails["cost_price"].ToString());
                            dgvItems.Rows.Add(readerDetails["item_id"].ToString(), "", readerDetails["code"].ToString(), readerDetails["name"].ToString(), Utilities.FormatDecimal(readerDetails["qty"]), null, 1, Utilities.FormatDecimal(readerDetails["qty"]), "", DateTime.Now.Date,
                                Utilities.FormatDecimal(readerDetails["cost_price"]), Utilities.FormatDecimal(readerDetails["price"]), Utilities.FormatDecimal(readerDetails["discount"]));

                            //    dgvItems.Rows.Add(reader["item_id"].ToString(), "", reader["code"].ToString(), reader["name"].ToString(), Utilities.FormatDecimal(reader["qty"]), null, Utilities.FormatDecimal(reader["factor"]), Utilities.FormatDecimal(Convert.ToDecimal(reader["factor"]) * Convert.ToDecimal(reader["qty"])), reader["batch_no"], Convert.ToDateTime(reader["expiry_date"]),
                            //Utilities.FormatDecimal(reader["cost_price"]), Utilities.FormatDecimal(reader["price"]), Utilities.FormatDecimal(reader["discount"]));
                            //    DataGridViewComboBoxCell comboCell = dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["vat"] as DataGridViewComboBoxCell;
                            //    if (comboCell != null && reader["vat"].ToString() != "0")
                            //    {
                            //        comboCell.Value = int.Parse(reader["vat"].ToString());
                            //        dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["vatp"].Value = decimal.Parse(reader["vatP"].ToString());
                            //    }
                            //    DataGridViewComboBoxCell comboCellItemUnit = dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["unit_id"] as DataGridViewComboBoxCell;
                            //    if (comboCellItemUnit != null && reader["item_unit_id"].ToString() != "0")
                            //    {
                            //        comboCellItemUnit.Value = int.Parse(reader["item_unit_id"].ToString());
                            //    }

                            DataGridViewComboBoxCell comboCell = dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["vat"] as DataGridViewComboBoxCell;
                            if (comboCell != null && readerDetails["vat"].ToString() != "0")
                            {
                                comboCell.Value = int.Parse(readerDetails["vat"].ToString());
                                dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["vatp"].Value = (decimal.Parse(dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["qty"].Value.ToString()) *
                                                   decimal.Parse(dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["cost"].Value.ToString()) * decimal.Parse(readerDetails["vat"].ToString()) / 100);
                            }
                            //DataGridViewComboBoxCell comboCellItemUnit = dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["unit_id"] as DataGridViewComboBoxCell;
                            //if (comboCellItemUnit != null && reader["item_unit_id"].ToString() != "0")
                            //{
                            //    comboCellItemUnit.Value = int.Parse(reader["item_unit_id"].ToString());
                            //}
                            dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["total"].Value = ((decimal.Parse(dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["qty"].Value.ToString()) *
                              decimal.Parse(dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["cost"].Value.ToString()))
                              + decimal.Parse(dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["vatp"].Value == null ? "0" : dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["vatp"].Value.ToString()));

                            dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["method"].Value = readerDetails["method"].ToString();
                            dgvItems.Rows[dgvItems.Rows.Count - 2].Cells["type"].Value = readerDetails["type"].ToString();
                        }

                    CalculateTotal();
                }
        }
        private void FormatNumberWithCommas(Guna2TextBox txtBox)
        {
            if (string.IsNullOrWhiteSpace(txtBox.Text))
                return;

            string rawText = txtBox.Text.Replace(",", "");

            decimal number;
            if (decimal.TryParse(rawText, out number))
            {
                int cursorPosition = txtBox.SelectionStart;
                txtBox.Text = number.ToString("N2");
                txtBox.SelectionStart = txtBox.Text.Length;
            }
        }

        public DataTable COMPANYINFO()
        {
            return ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_company Limit 1");
        }

        public DataTable VendorDetails(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT p.id,p.date,p.vendor_id,p.invoice_id,p.bill_to,p.city,p.sales_man,p.ship_date,p.ship_via,ship_to,po_num,payment_method,payment_terms,payment_date,(SELECT NAME FROM tbl_coa_level_4 WHERE id=account_cash_id) accountName,total,vat,net,pay,`change` 
                                  , v.name vendorName, v.main_phone,v.email,v.trn,v.mobile from tbl_purchase p, tbl_vendor v
                                  WHERE p.vendor_id = v.id AND p.id = @purchaseId; ", ClinicDatabase.CreateParameterNew("@purchaseId", a1));
        }
        public DataTable PurchaseDetails(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT d.id,d.item_id,i.name,d.qty,d.cost_price,d.price,(d.qty*d.cost_price) subCostTotal,(d.qty*d.price) subPriceTotal,d.discount,((d.qty*d.cost_price)-d.discount) subTotal,d.vatp vatAmount,d.vat vatPercentage,d.total,d.item_unit_id,(SELECT NAME FROM tbl_sub_cost_center WHERE id=item_unit_id) costCenterName,i.type,i.method,i.unit_id,(select name from tbl_unit WHERE id=i.unit_id) unitName, i.code as code FROM  tbl_purchase_details d INNER JOIN tbl_items i ON d.item_id = i.id WHERE d.purchase_id =@purchaseId; ", ClinicDatabase.CreateParameterNew("@purchaseId", a1));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (guna2Panel1.Height == 55)

            {
                timer1.Stop();

            }
            else
            {
                guna2Panel1.Height = guna2Panel1.Height + 11;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (guna2Panel1.Height == 0)
            {
                timer2.Stop();
            }
            else
            {
                guna2Panel1.Height = guna2Panel1.Height - 11;
            }
        }

        private void guna2TileButton18_Click(object sender, EventArgs e)
        {
            if (guna2Panel1.Height >= 55)
            {
                timer2.Start();
                timer1.Stop();
            }
            else
            {
                timer2.Stop();
                timer1.Start();
            }
        }

        private void guna2TileButton30_Click(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void guna2TileButton31_Click(object sender, EventArgs e)
        {
            ShowReportReceiverNote();
        }

        private void cmbPaymentTerms_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbPaymentMethod.Text == "Credit")
                dtPaymentTerms.Value = dtInv.Value.AddDays(int.Parse(cmbPaymentTerms.Text));
            else
                dtPaymentTerms.Value = dtInv.Value;
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal; // Restore down
            }
            else
            {
                this.WindowState = FormWindowState.Maximized; // Maximize
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2TileButton23_Click(object sender, EventArgs e)
        {
            clearData();
        }
        private void clearData()
        {
            clear();
            txtBillTo.Text = "";
            id = 0;
        }

        private void guna2TileButton22_Click(object sender, EventArgs e)
        {
            btnSave.PerformClick();
        }

        private void guna2TileButton21_Click(object sender, EventArgs e)
        {
            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_purchase SET state = -1 WHERE id = @id; UPDATE tbl_transaction SET state= -1 WHERE transaction_id=@id AND t_type = 'PURCHASE';",
                                          ClinicDatabase.CreateParameterNew("id", id.ToString()));
            CommonInsert.DeleteItemTransaction("Purchase", id.ToString());
            CommonInsert.DeleteCostCenterTransactionEntry(id.ToString(), "Purchase");
            CommonInsert.DeleteTransactionEntry(id, "PURCHASE");
            Utilities.LogAudit(clsGlobal.CurrentUser.UserID, "PURCHASE", "Delete Purchase Invoice", id, "Deleted Purchase Invoice with ID: " + id.ToString());
            clearData();
        }

        private void guna2TileButton20_Click(object sender, EventArgs e)
        {
            id = 0;
        }

        private void guna2TileButton24_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmPurchaseByVendorSummary());
        }

        private void guna2TileButton25_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new MasterTransactionJournal(id.ToString(), "PURCHASE"));
        }

        private void CbSubcontractors_CheckedChanged(object sender, EventArgs e)
        {
            typed = CbSubcontractors.Checked ? "Subcontractor" : "";
            BindCombos.PopulateVendors(cmbVendor, false, false, typed);
        }

        private void dtInv_ValueChanged(object sender, EventArgs e)
        {
            dtPaymentTerms.Value = dtShip.Value = dtInv.Value;
            if (!string.IsNullOrEmpty(cmbPaymentTerms.Text))
            {
                int numberOfDays = Convert.ToInt32(cmbPaymentTerms.Text);
                dtPaymentTerms.Value = dtPaymentTerms.Value.AddDays(numberOfDays);
            }
        }
        string filePath = "";
        private void guna2TileButton16_Click(object sender, EventArgs e)
        {
            //if (id > 0)
            //{
            //    //open in dialog box
            //    AttachmentManager.OpenAttachmentByType(id, "PURCHASE INVOICE");
            //}
            //else
            //{
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
            }
            //}
        }

        private void guna2TileButton26_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new MasterTransactionJournal(id.ToString(), "PURCHASE"));
        }

        private void cmbPurchasetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPurchasetype.Text.ToString().Trim() == "Fixed Assets")
            {
                labelFixedAssetCategory.Visible = true;
                cmbFixedAssetCategory.Visible = true;
            }
            //else if (cmbPurchasetype.Text.ToString() == "Expense")
            //{
            //    //
            //}
            else
            {
                labelFixedAssetCategory.Visible = false;
                cmbFixedAssetCategory.Visible = false;
            }
        }

        private void lstAccountSuggestions_Click(object sender, EventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem == null)
                return;

            if (!(lstAccountSuggestions.SelectedItem is ItemSuggestion item))
                return;

            int selectedItemId = item.Id;
            string selectedCode = item.Code;
            string selectedName = item.Name;


            if (lstAccountSuggestions.Tag is Guna2TextBox gunaTextBox)
            {
                gunaTextBox.Text = selectedCode;
                gunaTextBox.Focus();
                gunaTextBox.SelectionStart = gunaTextBox.Text.Length;

                if (gunaTextBox == txtVendorCode)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                        "SELECT id FROM tbl_vendor WHERE code = @code",
                        ClinicDatabase.CreateParameterNew("code", selectedCode)))
                    {
                        if (reader.Read())
                            cmbVendor.SelectedValue = int.Parse(reader["id"].ToString());
                    }
                }
            }
            else if (lstAccountSuggestions.Tag is TextBox textBox)
            {
                if (dgvItems.CurrentCell != null && dgvItems.CurrentCell.OwningColumn.Name == "code")
                {
                    DataGridViewRow row = dgvItems.CurrentRow;
                    row.Cells["code"].Value = selectedCode;
                    loadItemUnits(selectedItemId);
                }
                else if (dgvItems.CurrentCell != null && dgvItems.CurrentCell.OwningColumn.Name == "name")
                {
                    DataGridViewRow row = dgvItems.CurrentRow;
                    row.Cells["name"].Value = selectedName;
                    loadItemUnits(selectedItemId);
                }
            }

            lstAccountSuggestions.Visible = false;
        }

        private void dgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dgvItems.Rows[e.RowIndex];
            var columnName = dgvItems.Columns[e.ColumnIndex].Name;
            string inputText = row.Cells[e.ColumnIndex].Value?.ToString()?.Trim();
            var itemId = row.Cells["itemId"].Value?.ToString();

            if (!string.IsNullOrWhiteSpace(itemId) || string.IsNullOrWhiteSpace(inputText))
                return;

            if ((columnName == "code" || columnName == "name") && lstAccountSuggestions.Items.Count <= 0)
            {
                ItemNotFoundMessage(inputText);
            }
        }

        private void dgvItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dgvItems.Columns[e.ColumnIndex].Name == "unit_id")
            {
                e.ThrowException = false;
            }
        }

        private void ItemNotFoundMessage(string name)
        {
            var result = MessageBox.Show(
                                            $"'{name}' not found!\nDo you want to create it?",
                                            "Invalid Item",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Warning
                                        );

            if (result == DialogResult.Yes)
            {
                new frmViewItem().ShowDialog();
            }

            DataGridViewRow row = dgvItems.CurrentRow;
            if (row != null)
            {
                row.Cells["itemid"].Value = null;
                row.Cells["cost"].Value = null;
                row.Cells["price"].Value = null;
                row.Cells["method"].Value = null;
                row.Cells["type"].Value = null;
                row.Cells["vat"].Value = null;
                row.Cells["name"].Value = null;
                row.Cells["code"].Value = null;
            }
        }

        private void guna2TileButton28_Click(object sender, EventArgs e)
        {
            int vendorId = cmbVendor.SelectedValue != null ? int.Parse(cmbVendor.SelectedValue.ToString()) : 0;
            frmMain.openChildForm(new frmPurchaseByVendorDetails(vendorId));
        }

        public void ShowReport()
        {
            //try
            //{
            //    string reportPath = Path.Combine(Application.StartupPath, "CrystalReport", "PurchaseInvoice.rpt");

            //    // Load the report file from disk
            //    ReportDocument cr = new ReportDocument();
            //    cr.Load(reportPath);

            //    // Fetch your data
            //    DataTable companyData = COMPANYINFO();
            //    DataTable VendorData = VendorDetails(invId.ToString());
            //    DataTable PurchaseData = PurchaseDetails(invId.ToString());

            //    // Assign data to subreports by name
            //    cr.Subreports["Company"].SetDataSource(companyData);
            //    cr.Subreports["Vendor"].SetDataSource(VendorData);
            //    cr.Subreports["Details"].SetDataSource(PurchaseData);

            //    MasterReportView reportForm = new MasterReportView();
            //    reportForm.crReportViewer.ReportSource = cr;
            //    reportForm.crReportViewer.RefreshReport();
            //    reportForm.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error: " + ex.Message);
            //}

        }

        public void ShowReportReceiverNote()
        {

            //try
            //{
            //    // Create the report 
            //    //ReceiverNote cr = new ReceiverNote();
            //    string reportPath = Path.Combine(Application.StartupPath, "CrystalReport", "ReceiverNote.rpt");

            //    // Load the report file from disk
            //    ReportDocument cr = new ReportDocument();
            //    cr.Load(reportPath);
            //    // Load the main report data
            //    DataTable companyData = COMPANYINFO();  // Assuming you want to pass ID 1
            //    DataTable VendorData = VendorDetails(invId.ToString());
            //    DataTable PurchaseData = PurchaseDetails(invId.ToString());
            //    if (companyData != null)  // Ensure that data was successfully retrieved
            //    {
            //        //cr.SetDataSource(companyData);
            //        cr.Subreports["Company"].SetDataSource(companyData);
            //        cr.Subreports["Vendor"].SetDataSource(VendorData);
            //        cr.Subreports["Items"].SetDataSource(PurchaseData);
            //    }
            //    else
            //    {
            //        MessageBox.Show("No data available for the report.");
            //        return;  // Exit the method if no data is available
            //    }

            //    MasterReportView reportForm = new MasterReportView();
            //    reportForm.crReportViewer.ReportSource = cr;
            //    reportForm.crReportViewer.RefreshReport();
            //    reportForm.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error: " + ex.Message);
            //}

        }


        // Step 1: Create a custom Calendar column class
        public class DataGridViewCalendarColumn : DataGridViewColumn
        {
            public DataGridViewCalendarColumn() : base(new DataGridViewCalendarCell())
            {
            }

            public override DataGridViewCell CellTemplate
            {
                get { return base.CellTemplate; }
                set
                {
                    if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewCalendarCell)))
                    {
                        throw new InvalidCastException("Must be a DataGridViewCalendarCell");
                    }
                    base.CellTemplate = value;
                }
            }
        }

        public class DataGridViewCalendarCell : DataGridViewTextBoxCell
        {
            public DataGridViewCalendarCell() : base()
            {
                this.Style.Format = "d"; // short date format
            }

            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                CalendarEditingControl ctl = DataGridView.EditingControl as CalendarEditingControl;
                if (this.Value == null || this.Value == DBNull.Value)
                {
                    ctl.Value = DateTime.Today;
                }
                else
                {
                    ctl.Value = Convert.ToDateTime(this.Value);
                }
            }

            public override Type EditType => typeof(CalendarEditingControl);
            public override Type ValueType => typeof(DateTime);
            public override object DefaultNewRowValue => DateTime.Today;
        }

        class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
        {
            DataGridView dataGridView;
            private bool valueChanged = false;
            int rowIndex;

            public CalendarEditingControl()
            {
                this.Format = DateTimePickerFormat.Short;
            }

            public object EditingControlFormattedValue
            {
                get { return this.Value.ToShortDateString(); }
                set
                {
                    if (value is string)
                    {
                        this.Value = DateTime.Parse((string)value);
                    }
                }
            }

            public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            {
                return EditingControlFormattedValue;
            }

            public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
            {
                this.Font = dataGridViewCellStyle.Font;
                this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
                this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
            }

            public int EditingControlRowIndex
            {
                get { return rowIndex; }
                set { rowIndex = value; }
            }

            public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        return true;
                    default:
                        return !dataGridViewWantsInputKey;
                }
            }

            public void PrepareEditingControlForEdit(bool selectAll) { }

            public bool RepositionEditingControlOnValueChange => false;

            public DataGridView EditingControlDataGridView
            {
                get { return dataGridView; }
                set { dataGridView = value; }
            }

            public bool EditingControlValueChanged
            {
                get { return valueChanged; }
                set { valueChanged = value; }
            }

            public Cursor EditingPanelCursor => base.Cursor;

            protected override void OnValueChanged(EventArgs eventargs)
            {
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
                base.OnValueChanged(eventargs);
            }
        }
    }
}