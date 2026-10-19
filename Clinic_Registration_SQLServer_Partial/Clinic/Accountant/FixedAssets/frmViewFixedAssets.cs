using Clinic.Login;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
   
    public partial class frmViewFixedAssets : Form
    {
        public int manID;
        int id;
        private EventHandler FixedAssetCategoryUpdatedHandler;
        private readonly Dictionary<Control, Rectangle>
    _designerControlBounds =
        new Dictionary<Control, Rectangle>();
        public frmViewFixedAssets(int id = 0)
        {
            InitializeComponent();

            // =====================================================
            // IMPORTANT
            // Capture the exact Designer positions BEFORE
            //// LocalizationManager gets a chance to move anything.
            // =====================================================
            CaptureDesignerLayout(this);

            ApplyModernDesign();

            // Translate visible text.
           // LocalizationManager.LocalizeForm(this);

            // =====================================================
            // Restore the exact Designer positions.
            //
            // This prevents:
            // English -> accidentally mirrored
            // Arabic  -> whole form physically mirrored
            //
            // We only want translated text, not a mirrored form.
            // =====================================================
            RestoreDesignerLayout();

            this.RightToLeft = RightToLeft.No;
            this.RightToLeftLayout = false;

            ApplyFixedAssetLanguageLayout();

            FixedAssetCategoryUpdatedHandler =
                (sender, args) => setComboBoxCategory();

            this.id = id;

            bool arabic = IsArabicLanguage();

            headerUC1.FormText =
                id == 0
                    ? (arabic
                        ? "أصل ثابت - جديد"
                        : "Fixed Asset - New")
                    : (arabic
                        ? "أصل ثابت - تعديل"
                        : "Fixed Asset - Edit");

            //.FixedAssetsCategory +=
                //FixedAssetCategoryUpdatedHandler;
        }
        private void ApplyFixedAssetLanguageLayout()
        {
            bool arabic = IsArabicLanguage();

            // =====================================================
            // FORM
            // NEVER mirror the complete Fixed Asset form.
            // =====================================================
            this.RightToLeft = RightToLeft.No;
            this.RightToLeftLayout = false;

            // =====================================================
            // LABELS
            // =====================================================
            Label[] labels =
            {
        label1,
        label2,
        label3,
        label4,
        label5,
        label6,
        label7,
        label8,
        label9,
        label10,
        label11,
        label12,
        label13,
        label14,
        label15,
        label16,
        label17,
        label18,
        label19,
        label20,
        label21,
        label24,
        label25
    };

            foreach (Label lbl in labels)
            {
                if (lbl == null)
                    continue;

                // DO NOT MOVE THE LABEL.
                // Designer position is preserved.

                if (arabic)
                {
                    lbl.RightToLeft =
                        RightToLeft.Yes;

                    lbl.TextAlign =
                        ContentAlignment.MiddleRight;
                }
                else
                {
                    lbl.RightToLeft =
                        RightToLeft.No;

                    lbl.TextAlign =
                        ContentAlignment.MiddleLeft;
                }
            }

            // =====================================================
            // NORMAL TEXTBOXES
            // =====================================================
            txtAssetName.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            txtAssetBrand.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            txtModel.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            txtSupplierName.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            txtStatus.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            // =====================================================
            // CODE / NUMBER FIELDS
            // Keep these LTR even in Arabic.
            // =====================================================
            txtCode.RightToLeft =
                RightToLeft.No;

            txtAccountCode.RightToLeft =
                RightToLeft.No;

            txtCreditAccountCode.RightToLeft =
                RightToLeft.No;

            txtExpenceAccount.RightToLeft =
                RightToLeft.No;

            txtOpeningBalanceCode.RightToLeft =
                RightToLeft.No;

            txtInvoiceNumber.RightToLeft =
                RightToLeft.No;

            txtPurchasePrice.RightToLeft =
                RightToLeft.No;

            txtDepreciationLife.RightToLeft =
                RightToLeft.No;

            txtDepreciationValue.RightToLeft =
                RightToLeft.No;

            txtTotalDays.RightToLeft =
                RightToLeft.No;

            // =====================================================
            // COMBOBOXES
            // Only text direction changes.
            // Their physical position NEVER changes.
            // =====================================================
            // Category can follow the selected language.
            cmbCategory.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            // =====================================================
            // ACCOUNT COMBOS
            // =====================================================
            // DO NOT set these Guna2ComboBoxes to RTL.
            //
            // Arabic account names are already translated by
            // BindCombos.PopulateAllLevel4Account().
            //
            // Keeping the combo LTR prevents the Guna dropdown from
            // becoming blank / huge / incorrectly rendered.
            cmbAccountName.RightToLeft = RightToLeft.No;
            cmbCreditAccount.RightToLeft = RightToLeft.No;
            cmbExpenceAccount.RightToLeft = RightToLeft.No;
            cmbOpeningBalance.RightToLeft = RightToLeft.No;
        }
        private bool IsArabicLanguage()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.StartsWith(
                "ar",
                StringComparison.OrdinalIgnoreCase);
        }


        private void CaptureDesignerLayout(Control parent)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (!_designerControlBounds.ContainsKey(control))
                {
                    _designerControlBounds.Add(
                        control,
                        control.Bounds);
                }

                if (control.HasChildren)
                {
                    CaptureDesignerLayout(control);
                }
            }
        }


        private void RestoreDesignerLayout()
        {
            foreach (var item in _designerControlBounds)
            {
                Control control = item.Key;

                if (control == null ||
                    control.IsDisposed)
                {
                    continue;
                }

                control.Bounds = item.Value;
            }
        }
        private void ApplyModernDesign()
        {
            // Main background
            this.BackColor = Color.FromArgb(248, 249, 252);

            // Style all existing controls WITHOUT moving them
            foreach (Control control in this.Controls)
            {
                ApplyControlStyleRecursive(control);
            }

            // ===============================
            // DELETE
            // ===============================
            if (btnDelete is Guna.UI2.WinForms.Guna2Button deleteButton)
            {
                deleteButton.FillColor = Color.FromArgb(255, 245, 247);
                deleteButton.ForeColor = Color.FromArgb(190, 55, 75);
                deleteButton.BorderColor = Color.FromArgb(235, 190, 198);
                deleteButton.BorderThickness = 1;
                deleteButton.BorderRadius = 8;
                deleteButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            // ===============================
            // SAVE - PRIMARY BUTTON
            // ===============================
            if (btnSave is Guna.UI2.WinForms.Guna2Button saveButton)
            {
                saveButton.FillColor = Color.FromArgb(103, 58, 183);
                saveButton.ForeColor = Color.White;
                saveButton.BorderThickness = 0;
                saveButton.BorderRadius = 8;
                saveButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                saveButton.HoverState.FillColor =
                    Color.FromArgb(86, 45, 165);
            }

            // ===============================
            // CLOSE
            // ===============================
            if (btnClose is Guna.UI2.WinForms.Guna2Button closeButton)
            {
                closeButton.FillColor = Color.White;
                closeButton.ForeColor = Color.FromArgb(55, 48, 75);
                closeButton.BorderColor = Color.FromArgb(205, 202, 215);
                closeButton.BorderThickness = 1;
                closeButton.BorderRadius = 8;
                closeButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            // ===============================
            // JOURNAL
            // ===============================
            chkJournal.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            chkJournal.ForeColor =
                Color.FromArgb(55, 48, 75);

            // Category link
            lnkCategory.LinkColor =
                Color.FromArgb(103, 58, 183);

            lnkCategory.ActiveLinkColor =
                Color.FromArgb(81, 45, 145);

            lnkCategory.Font =
                new Font("Segoe UI", 8.5F, FontStyle.Bold);
        }
        private void ApplyControlStyleRecursive(Control control)
        {
            if (control == null)
                return;

            // =========================
            // TEXTBOX
            // =========================
            if (control is Guna.UI2.WinForms.Guna2TextBox textBox)
            {
                textBox.BorderRadius = 6;

                textBox.BorderColor =
                    Color.FromArgb(210, 213, 222);

                textBox.FocusedState.BorderColor =
                    Color.FromArgb(103, 58, 183);

                textBox.HoverState.BorderColor =
                    Color.FromArgb(170, 160, 195);

                textBox.FillColor =
                    textBox.Enabled
                        ? Color.White
                        : Color.FromArgb(242, 243, 246);

                textBox.ForeColor =
                    Color.FromArgb(48, 45, 60);

                textBox.Font =
                    new Font("Segoe UI", 9F);
            }

            // =========================
            // COMBOBOX
            // =========================
            else if (control is Guna.UI2.WinForms.Guna2ComboBox comboBox)
            {
                comboBox.BorderRadius = 6;

                comboBox.BorderColor =
                    Color.FromArgb(210, 213, 222);

                comboBox.FocusedColor =
                    Color.FromArgb(103, 58, 183);

                comboBox.FillColor = Color.White;

                comboBox.ForeColor =
                    Color.FromArgb(48, 45, 60);

                comboBox.Font =
                    new Font("Segoe UI", 9F);

                // IMPORTANT:
                // Prevent the Guna dropdown rows from becoming
                // extremely tall / appearing blank.
                comboBox.ItemHeight = 30;

                // Give enough room for Arabic + account code.
                comboBox.DropDownWidth =
                    Math.Max(comboBox.Width, 390);

                comboBox.RightToLeft =
                    RightToLeft.No;
            }

            // =========================
            // DATE PICKER
            // =========================
            else if (control is Guna.UI2.WinForms.Guna2DateTimePicker picker)
            {
                picker.BorderRadius = 6;

                picker.BorderColor =
                    Color.FromArgb(210, 213, 222);

                picker.BorderThickness = 1;

                picker.FillColor =
                    picker.Enabled
                        ? Color.White
                        : Color.FromArgb(242, 243, 246);

                picker.ForeColor =
                    Color.FromArgb(48, 45, 60);

                picker.Font =
                    new Font("Segoe UI", 9F);
            }

            // =========================
            // LABEL
            // =========================
            else if (control is Label label)
            {
                label.ForeColor =
                    Color.FromArgb(58, 53, 75);

                label.Font =
                    new Font(
                        "Segoe UI",
                        8.5F,
                        FontStyle.Regular
                    );
            }

            // =========================
            // PANELS
            // =========================
            else if (control is Panel panel)
            {
                // Don't overwrite special panel colors unnecessarily
                if (panel.BackColor == SystemColors.Control)
                {
                    panel.BackColor =
                        Color.FromArgb(248, 249, 252);
                }
            }

            foreach (Control child in control.Controls)
            {
                ApplyControlStyleRecursive(child);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            //.FixedAssetsCategory -= FixedAssetCategoryUpdatedHandler;
            this.Close();
        }
        private void frmViewFixedAssets_Load(
        object sender,
        EventArgs e)
        {
            txtCode.Text =
                GenerateNextFixedAssetCode();

            // =====================================================
            // CATEGORY
            // =====================================================
            BindCombos.PopulateFixedAssetsCategories(
                cmbCategory);

            // =====================================================
            // LEVEL 4 ACCOUNTS
            //
            // English:
            // CODE - English Account Name
            //
            // Arabic:
            // CODE - Arabic Account Name
            //
            // SelectedValue ALWAYS remains original account ID.
            // =====================================================
            BindCombos.PopulateAllLevel4Account(
                cmbAccountName);

            BindCombos.PopulateAllLevel4Account(
                cmbCreditAccount);

            BindCombos.PopulateAllLevel4Account(
                cmbExpenceAccount);

            BindCombos.PopulateAllLevel4Account(
                cmbOpeningBalance);

            // =====================================================
            // DEFAULT ACCOUNTS
            // =====================================================
            SelectAccountComboById(
      cmbCreditAccount,
      frmLogin.defaultAccounts.ContainsKey("Fixed Asset Credit Account")
          ? frmLogin.defaultAccounts["Fixed Asset Credit Account"]
          : 0);

            SelectAccountComboById(
                cmbAccountName,
                frmLogin.defaultAccounts.ContainsKey("Fixed Asset Debit Account")
                    ? frmLogin.defaultAccounts["Fixed Asset Debit Account"]
                    : 0);

            SelectAccountComboById(
                cmbOpeningBalance,
                BindCombos.SelectDefaultLevelAccount(
                    "Opening Balance Equity"));
            // =====================================================
            // EDIT EXISTING ASSET
            // =====================================================
            if (id != 0)
            {
                BindAsset();
            }

            // Apply text direction AFTER data binding.
            // Do not move any controls.
            FixAccountComboDisplay(cmbAccountName);
            FixAccountComboDisplay(cmbCreditAccount);
            FixAccountComboDisplay(cmbExpenceAccount);
            FixAccountComboDisplay(cmbOpeningBalance);

            ApplyFixedAssetLanguageLayout();
            ApplyFixedAssetLanguageLayout();
        }
        private void FixAccountComboDisplay(
    Guna.UI2.WinForms.Guna2ComboBox combo)
        {
            if (combo == null)
                return;

            combo.RightToLeft = RightToLeft.No;

            combo.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            combo.ForeColor =
                Color.FromArgb(48, 45, 60);

            combo.FillColor =
                Color.White;

            combo.ItemHeight = 30;

            combo.DropDownWidth =
                Math.Max(combo.Width, 390);

            combo.Invalidate();
            combo.Refresh();
        }
        private int GetSelectedAccountId(
    Guna.UI2.WinForms.Guna2ComboBox combo)
        {
            if (combo?.SelectedItem is
                BindCombos.Level4AccountComboItem item)
            {
                return item.Id;
            }

            return 0;
        }
        private void setComboBoxCategory()
        {
            BindCombos.PopulateFixedAssetsCategories(cmbCategory);
            SetSelectedIndexToLastItem(cmbCategory);
        }

        private void SetSelectedIndexToLastItem(Guna.UI2.WinForms.Guna2ComboBox comboBox)
        {
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = comboBox.Items.Count - 1;
            }
        }

        private void txtDepreciationLife_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDepreciationLife.Text))
            {
                txtDepreciationValue.Text = string.Empty;
                txtTotalDays.Text = string.Empty;
                return;
            }
            int lifeYears;
            if (int.TryParse(txtDepreciationLife.Text, out lifeYears) && lifeYears > 0)
            {
                txtDepreciationValue.Text = (100 / lifeYears).ToString();

                DateTime purchaseDate = dtpPurchaseDate.Value.Date;
                DateTime endDate = purchaseDate.AddYears(lifeYears).AddDays(-1);
                dtpEndDate.Value = endDate;

                txtTotalDays.Text = ((endDate - purchaseDate).Days + 1).ToString();
            }
            else
            {
                txtDepreciationValue.Text = string.Empty;
                txtTotalDays.Text = string.Empty;
            }
        }

        private void dtpPurchaseDate_ValueChanged(object sender, EventArgs e)
        {
            if (txtDepreciationLife.Text.Trim() == "")
                return;
            dtpEndDate.Value = dtpPurchaseDate.Value.Date.AddYears(int.Parse(txtDepreciationLife.Text)).AddDays(-1); ;
            txtTotalDays.Text = ((dtpEndDate.Value.Date - dtpPurchaseDate.Value.Date).Days + 1).ToString();
        }
        private void BindAsset()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_fixed_assets where id = " + id))
                if (reader.Read())
                {
                    txtAssetBrand.Text = reader["brand"].ToString();
                    txtAssetName.Text = reader["name"].ToString();
                    txtCode.Text = reader["code"].ToString();
                    txtDepreciationLife.Text = reader["depreciation_life"].ToString();
                    txtInvoiceNumber.Text = reader["invoice_number"].ToString();
                    txtModel.Text = reader["model"].ToString();
                    txtPurchasePrice.Text = reader["purchase_price"].ToString();
                    txtStatus.Text = reader["status"].ToString();
                    txtSupplierName.Text = reader["supplier"].ToString();
                    cmbAccountName.SelectedValue = reader["debit_account_id"].ToString();
                    cmbCreditAccount.SelectedValue = reader["credit_account_id"].ToString();
                    cmbExpenceAccount.SelectedValue = reader["expence_account_id"].ToString();
                    cmbCategory.SelectedValue = reader["category_id"].ToString();
                    dtpAssets.Value = DateTime.Parse(reader["date"].ToString());
                    dtpPurchaseDate.Value = DateTime.Parse(reader["purchase_date"].ToString());
                    if (int.Parse(reader["manufacture"].ToString()) == 1)
                    {
                        guna2ToggleSwitch1.Checked = pnlmanifacuter.Visible == true;
                        var mfrStatus = reader["manufactureStatus"].ToString();
                    }

                    using (MySqlDataReader reader0 = ClinicDatabase.ExecuteReader("select * from tbl_purchase where invoice_id = @id and fixed_asset_category_id=@catId and purchase_type='Fixed Assets'",
                        ClinicDatabase.CreateParameterNew("id", txtInvoiceNumber.Text), ClinicDatabase.CreateParameterNew("catId", cmbCategory.SelectedValue.ToString())))
                        if (reader0.Read())
                        {
                            SelectAccountComboById(
                                cmbAccountName,
                                int.Parse(reader0["account_cash_id"].ToString())); dtpPurchaseDate.Enabled = false;
                            txtSupplierName.Enabled = false;
                            txtInvoiceNumber.Enabled = false;
                            txtPurchasePrice.Enabled = false;
                            dtpEndDate.Enabled = true;
                        }
                }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (guna2ToggleSwitch1.Checked &&  pnlmanifacuter.Visible == true)
            {
                manID = 1;
            }
            else
            {
                manID = 0;
            }


            if (id == 0)
            {
                if (insertAsset())
                {
                    //.RefreshFixedAsset();
                    this.Close();
                }
            }
            else
            {
                if (updateAsset())
                {
                    //.RefreshFixedAsset();
                    this.Close();
                }
            }
        }
        private bool updateAsset()
        {
            if (!chkRequireData())
                return false;

            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_fixed_assets 
                            SET name = @name,date=@date,
                                brand = @brand,
                                category_id = @category_id,
                                model=@model,
                                supplier=@supplier,
                                status=@status,
                                invoice_number=@invoice_number,
                                purchase_date=@purchase_date,
                                end_date=@end_date,
                                depreciation_life=@depreciation_life,
                                purchase_price=@purchase_price,
                                debit_account_id=@debit_account_id,
                                credit_account_id=@credit_account_id,
                                expence_account_id=@expence_account_id,
                                modified_by = @modified_by,
                                modified_date = @modified_date,
                                manufacture = @manufacture
                                WHERE id = @id;",
                              ClinicDatabase.CreateParameterNew("id", id),
                              ClinicDatabase.CreateParameterNew("date", dtpAssets.Value.Date),
                              ClinicDatabase.CreateParameterNew("name", txtAssetName.Text),
                              ClinicDatabase.CreateParameterNew("brand", txtAssetBrand.Text),
                              ClinicDatabase.CreateParameterNew("category_id", cmbCategory.SelectedValue.ToString()),
                              ClinicDatabase.CreateParameterNew("model", txtModel.Text),
                              ClinicDatabase.CreateParameterNew("supplier", txtSupplierName.Text),
                              ClinicDatabase.CreateParameterNew("status", txtStatus.Text),
                              ClinicDatabase.CreateParameterNew("invoice_number", txtInvoiceNumber.Text),
                              ClinicDatabase.CreateParameterNew("purchase_date", dtpPurchaseDate.Value.Date),
                              ClinicDatabase.CreateParameterNew("end_date", dtpEndDate.Value.Date),
                              ClinicDatabase.CreateParameterNew("depreciation_life", txtDepreciationLife.Text),
                              ClinicDatabase.CreateParameterNew("purchase_price", txtPurchasePrice.Text),
                             ClinicDatabase.CreateParameterNew(
    "debit_account_id",
    GetSelectedAccountId(cmbAccountName)),

ClinicDatabase.CreateParameterNew(
    "credit_account_id",
    GetSelectedAccountId(cmbCreditAccount)),

ClinicDatabase.CreateParameterNew(
    "expence_account_id",
    GetSelectedAccountId(cmbExpenceAccount)),
                              ClinicDatabase.CreateParameterNew("modified_by", -1),
                              ClinicDatabase.CreateParameterNew("modified_date", DateTime.Now.Date),
                              ClinicDatabase.CreateParameterNew("manufacture", manID)).ToString();
            ClinicDatabase.ExecuteNonQuery("delete from tbl_transaction where transaction_id = @id and type = 'Fixed Assets'",
                                ClinicDatabase.CreateParameterNew("id", id));
            if (chkJournal.Checked)
            {
                InsertJournal(id);
            }
            ////Utilities.LogAudit(-1, "Update Fixed Assets", "Fixed Assets", id, "Updated Fixed Assets: " + txtAssetName.Text);
            return true;
        }
        decimal AssetId;
        private string GenerateNextFixedAssetCode()
        {
            int code;
            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(code AS UNSIGNED)) AS lastCode FROM tbl_fixed_assets"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                    code = int.Parse(reader["lastCode"].ToString()) + 1;
                else
                    code = 0001;
            }
            return code.ToString("D5");
        }
        private bool insertAsset()
        {
            if (!chkRequireData())
                return false;
            AssetId = decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO `tbl_fixed_assets`(date,
                                                            `code`, name, brand, `category_id`, `model`, `supplier`, `status`, 
                                                            `invoice_number`, `purchase_date`, `end_date`, `depreciation_life`,  
                                                            `purchase_price`, debit_account_id,credit_account_id, expence_account_id, `created_by`, `created_date`, `state`,manufacture,
                                                             manufactureStatus) 
                                                        VALUES (@date,
                                                            @code, @name, @brand, @category_id, @model, @supplier, @status, 
                                                            @invoice_number, @purchase_date, @end_date, @depreciation_life, 
                                                            @purchase_price, @debit_account_id, @credit_account_id, @expence_account_id, @created_by, @created_date, 0,
                                                        @manufacture,'Draft');
                                                        SELECT LAST_INSERT_ID();",
                                                        ClinicDatabase.CreateParameterNew("date", dtpAssets.Value.Date),
                              ClinicDatabase.CreateParameterNew("code", GenerateNextFixedAssetCode()),
                              ClinicDatabase.CreateParameterNew("name", txtAssetName.Text),
                              ClinicDatabase.CreateParameterNew("brand", txtAssetBrand.Text),
                              ClinicDatabase.CreateParameterNew("category_id", cmbCategory.SelectedValue.ToString()),
                              ClinicDatabase.CreateParameterNew("model", txtModel.Text),
                              ClinicDatabase.CreateParameterNew("supplier", txtSupplierName.Text),
                              ClinicDatabase.CreateParameterNew("status", txtStatus.Text),
                              ClinicDatabase.CreateParameterNew("invoice_number", txtInvoiceNumber.Text),
                              ClinicDatabase.CreateParameterNew("purchase_date", dtpPurchaseDate.Value.Date),
                              ClinicDatabase.CreateParameterNew("end_date", dtpEndDate.Value.Date),
                              ClinicDatabase.CreateParameterNew("depreciation_life", txtDepreciationLife.Text),
                              ClinicDatabase.CreateParameterNew("purchase_price", txtPurchasePrice.Text),
                              ClinicDatabase.CreateParameterNew("debit_account_id", cmbAccountName.SelectedValue.ToString()),
                              ClinicDatabase.CreateParameterNew("credit_account_id", cmbCreditAccount.SelectedValue.ToString()),
                              ClinicDatabase.CreateParameterNew("expence_account_id", cmbExpenceAccount.SelectedValue.ToString()),
                              ClinicDatabase.CreateParameterNew("created_by", -1),
                              ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date),
                              ClinicDatabase.CreateParameterNew("manufacture", manID)).ToString());

            if (chkJournal.Checked)
            {
                InsertJournal((int)AssetId);
            }
            ////Utilities.LogAudit(-1, "Insert Fixed Assets", "Fixed Assets", (int)AssetId, "Inserted Fixed Assets: " + txtAssetName.Text);
            return true;
        }
        public static void UpdateOrAddFixedAssets(int pId,int cId)
        {
            //load from purchase 
            using (MySqlDataReader reader1 = ClinicDatabase.ExecuteReader("select total,(tbl_vendor.name) AS 'Name',tbl_purchase.invoice_id as VoucherNo,tbl_purchase.date as Date from tbl_purchase INNER JOIN tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id where tbl_purchase.id =@id",
             ClinicDatabase.CreateParameterNew("id", pId)))
                if (reader1.Read())
                {
                    string PurchasePrice = reader1["total"].ToString();
                    string SupplierName = reader1["Name"].ToString();
                    string Status = "Draft";
                    string voucherNo = reader1["VoucherNo"].ToString();
                    DateTime date = DateTime.Parse(reader1["Date"].ToString());
                    int oldVId = 0;
                    using (MySqlDataReader reader0 = ClinicDatabase.ExecuteReader("select * from tbl_fixed_assets WHERE purchase_date = @date AND supplier = @SupplierName AND invoice_number = @voucherNo",
                        ClinicDatabase.CreateParameterNew("date", date),
                        ClinicDatabase.CreateParameterNew("SupplierName", SupplierName),
                        ClinicDatabase.CreateParameterNew("voucherNo", voucherNo)))
                        if (reader0.Read())
                        {
                            oldVId = int.Parse(reader0["id"].ToString());
                        }
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_fixed_assets_category where id = @id", ClinicDatabase.CreateParameterNew("id", cId.ToString())))
                    {
                        if (reader.Read())
                        {
                            int assets_account_id = 0, depreciation_account_id = 0, expense_account_id = 0;

                            if (reader["assets_account_id"] != DBNull.Value && int.Parse(reader["assets_account_id"].ToString()) > 0)
                                assets_account_id = int.Parse(reader["assets_account_id"].ToString());

                            if (reader["depreciation_account_id"] != DBNull.Value && int.Parse(reader["depreciation_account_id"].ToString()) > 0)
                                depreciation_account_id = int.Parse(reader["depreciation_account_id"].ToString());

                            if (reader["expence_account_id"] != DBNull.Value && int.Parse(reader["expence_account_id"].ToString()) > 0)
                                expense_account_id = int.Parse(reader["expence_account_id"].ToString());
                            
                            string AssetName = reader["category_name"].ToString();
                            object obItem = ClinicDatabase.ExecuteScalar("SELECT i.name FROM tbl_purchase_details pd INNER JOIN tbl_items i ON i.id = pd.item_id WHERE pd.purchase_id = @id", ClinicDatabase.CreateParameterNew("id", pId.ToString()));

                            if (PurchasePrice.Trim() != "")
                            {
                                if (oldVId > 0)
                                {
                                    ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_fixed_assets 
                                                            SET date=@date,
                                                                category_id = @category_id,
                                                                supplier=@supplier,
                                                                status=@status,
                                                                invoice_number=@invoice_number,
                                                                purchase_date=@purchase_date,
                                                                end_date=@end_date,
                                                                depreciation_life=@depreciation_life,
                                                                purchase_price=@purchase_price,
                                                                debit_account_id=@debit_account_id,
                                                                credit_account_id=@credit_account_id,
                                                                expence_account_id=@expence_account_id,
                                                                modified_by = @modified_by,
                                                                modified_date = @modified_date
                                                                WHERE id = @id;",
                                                      ClinicDatabase.CreateParameterNew("id", oldVId),
                                                      ClinicDatabase.CreateParameterNew("date", date),
                                                      ClinicDatabase.CreateParameterNew("category_id", cId.ToString()),
                                                      ClinicDatabase.CreateParameterNew("supplier", SupplierName),
                                                      ClinicDatabase.CreateParameterNew("status", Status),
                                                      ClinicDatabase.CreateParameterNew("invoice_number", voucherNo),
                                                      ClinicDatabase.CreateParameterNew("purchase_date", date),
                                                      ClinicDatabase.CreateParameterNew("end_date", date),
                                                      ClinicDatabase.CreateParameterNew("depreciation_life", "0"),
                                                      ClinicDatabase.CreateParameterNew("purchase_price", PurchasePrice),
                                                      ClinicDatabase.CreateParameterNew("debit_account_id", assets_account_id.ToString()),
                                                      ClinicDatabase.CreateParameterNew("credit_account_id", depreciation_account_id.ToString()),
                                                      ClinicDatabase.CreateParameterNew("expence_account_id", expense_account_id.ToString()),
                                                      ClinicDatabase.CreateParameterNew("modified_by", -1),
                                                      ClinicDatabase.CreateParameterNew("modified_date", DateTime.Now.Date));

                                    ////Utilities.LogAudit(-1, "Update Fixed Assets", "Fixed Assets", oldVId, "Updated Fixed Assets: " + AssetName);
                                }
                                else
                                {
                                    int code;
                                    using (var reader2 = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(code AS UNSIGNED)) AS lastCode FROM tbl_fixed_assets"))
                                    {
                                        if (reader2.Read() && reader2["lastCode"] != DBNull.Value)
                                            code = int.Parse(reader2["lastCode"].ToString()) + 1;
                                        else
                                            code = 0001;
                                    }
                                    string coded = code.ToString("D5");

                                    var AssetId = ClinicDatabase.ExecuteScalar(@"INSERT INTO `tbl_fixed_assets`(date,
                                                            `code`, name, brand, `category_id`, `model`, `supplier`, `status`, 
                                                            `invoice_number`, `purchase_date`, `end_date`, `depreciation_life`,  
                                                            `purchase_price`, debit_account_id,credit_account_id, expence_account_id, `created_by`, `created_date`, `state`) 
                                                        VALUES (@date,
                                                            @code, @name, @brand, @category_id, @model, @supplier, @status, 
                                                            @invoice_number, @purchase_date, @end_date, @depreciation_life, 
                                                            @purchase_price, @debit_account_id, @credit_account_id, @expence_account_id, @created_by, @created_date, 0
                                                        );
                                                        SELECT LAST_INSERT_ID();",
                                                                                ClinicDatabase.CreateParameterNew("date", date),
                                                      ClinicDatabase.CreateParameterNew("code", coded),
                                                      ClinicDatabase.CreateParameterNew("name", AssetName),
                                                      ClinicDatabase.CreateParameterNew("brand", ""),
                                                      ClinicDatabase.CreateParameterNew("category_id", cId.ToString()),
                                                      ClinicDatabase.CreateParameterNew("model", ""),
                                                      ClinicDatabase.CreateParameterNew("supplier", SupplierName),
                                                      ClinicDatabase.CreateParameterNew("status", Status),
                                                      ClinicDatabase.CreateParameterNew("invoice_number", voucherNo),
                                                      ClinicDatabase.CreateParameterNew("purchase_date", date),
                                                      ClinicDatabase.CreateParameterNew("end_date", date),
                                                      ClinicDatabase.CreateParameterNew("depreciation_life", "0"),
                                                      ClinicDatabase.CreateParameterNew("purchase_price", PurchasePrice),
                                                      ClinicDatabase.CreateParameterNew("debit_account_id", assets_account_id.ToString()),
                                                      ClinicDatabase.CreateParameterNew("credit_account_id", depreciation_account_id.ToString()),
                                                      ClinicDatabase.CreateParameterNew("expence_account_id", expense_account_id.ToString()),
                                                      ClinicDatabase.CreateParameterNew("created_by", -1),
                                                      ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date)).ToString();
                                    ////Utilities.LogAudit(-1, "Insert Fixed Assets", "Fixed Assets", int.Parse(AssetId), "Inserted Fixed Assets: " + AssetName);
                                }
                            }
                        }
                    }
                }
        }

        private void InsertJournal(int pId)
        {
            DateTime startDate = dtpPurchaseDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date;
            decimal totalAmount = decimal.Parse(txtPurchasePrice.Text);
            int totalDays = (endDate - startDate).Days + 1;
            string depexpencac =
                GetSelectedAccountId(cmbExpenceAccount).ToString(); decimal actualTotal = 0;

            //if (txtSupplierName.Enabled)
            //{
                CommonInsert.InsertTransactionEntry(dtpAssets.Value,
                   GetSelectedAccountId(cmbAccountName).ToString(), totalAmount.ToString("F2"), "0", pId.ToString(), "0", "Fixed Assets",
                    txtAssetName.Text + " - Fixed Assets No. " + pId, -1, DateTime.Now.Date);

                CommonInsert.InsertTransactionEntry(dtpAssets.Value,
                   GetSelectedAccountId(cmbOpeningBalance).ToString(), "0", totalAmount.ToString("F2"), pId.ToString(), "0", "Fixed Assets",
                    txtAssetName.Text + " - Fixed Assets No. " + pId, -1, DateTime.Now.Date);
            //}

            DateTime currentMonthStart = startDate;

            while (currentMonthStart <= endDate)
            {
                DateTime currentMonthEnd = new DateTime(currentMonthStart.Year, currentMonthStart.Month, DateTime.DaysInMonth(currentMonthStart.Year, currentMonthStart.Month));
                if (currentMonthEnd > endDate)
                    currentMonthEnd = endDate;

                DateTime periodStart = currentMonthStart < startDate ? startDate : currentMonthStart;
                DateTime periodEnd = currentMonthEnd > endDate ? endDate : currentMonthEnd;

                int daysInPeriod = (periodEnd - periodStart).Days + 1;

                decimal amount = Math.Round((totalAmount / totalDays) * daysInPeriod, 2);

                // Final adjustment to correct rounding
                if (periodEnd == endDate)
                {
                    amount = totalAmount - actualTotal;
                }

                actualTotal += amount;

                CommonInsert.InsertTransactionEntry(periodEnd,
                    depexpencac, amount.ToString("F2"), "0", pId.ToString(), "0", "Fixed Assets",
                    txtAssetName.Text + " - Fixed Assets No. " + pId, -1, DateTime.Now.Date);

                CommonInsert.InsertTransactionEntry(periodEnd,
                   GetSelectedAccountId(cmbCreditAccount).ToString(), "0", amount.ToString("F2"), pId.ToString(), "0", "Fixed Assets",
                    txtAssetName.Text + " - Fixed Assets No. " + pId, -1, DateTime.Now.Date);

                currentMonthStart = currentMonthStart.AddMonths(1);
                currentMonthStart = new DateTime(currentMonthStart.Year, currentMonthStart.Month, 1);
            }
        }

        private bool chkRequireData()
        {
            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Select Category First");
                cmbCategory.Focus();
                return false;
            }
            if (txtPurchasePrice.Text.Trim() == "")
            {
                MessageBox.Show("Enter Purchase Price First.");
                txtPurchasePrice.Focus();
                return false;
            }
            if (txtDepreciationValue.Text == "")
            {
                MessageBox.Show("Enter Depreciation In Life");
                return false;
            }
            if (txtAssetName.Text == "")
            {
                MessageBox.Show("Enter Asset Name First");
                txtAssetName.Focus();
                return false;
            }
            return true;
        }


        private void lnkCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //new frmViewFixedAssetsCategory().Show();
            new frmViewFixedAssetsCategory().Show();
        }
        private void cmbAccountName_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            int accountId =
                GetSelectedAccountId(cmbAccountName);

            if (accountId <= 0)
            {
                txtAccountCode.Text = "";
                return;
            }

            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    "SELECT code FROM tbl_coa_level_4 WHERE id = @id",
                    ClinicDatabase.CreateParameterNew("id", accountId)))
            {
                if (reader.Read())
                    txtAccountCode.Text =
                        reader["code"].ToString();
                else
                    txtAccountCode.Text = "";
            }
        }
        private void txtAccountCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtAccountCode.Text)))
                if (reader.Read())
                    cmbAccountName.SelectedValue = int.Parse(reader["id"].ToString());
            
        }

        private void txtAccountCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                   ClinicDatabase.CreateParameterNew("code", txtAccountCode.Text)))
                if (!reader.Read())
                    cmbAccountName.SelectedIndex = -1;

        }

        private void cmbCreditAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCreditAccount.SelectedValue == null)
            {
                txtCreditAccountCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbCreditAccount.SelectedValue.ToString()))
                if (reader.Read())
                    txtCreditAccountCode.Text = reader["code"].ToString();
                else
                    txtCreditAccountCode.Text = "";

        }
        private void cmbOpeningBalance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbOpeningBalance.SelectedValue == null)
            {
                txtOpeningBalanceCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbOpeningBalance.SelectedValue.ToString()))
                if (reader.Read())
                    txtOpeningBalanceCode.Text = reader["code"].ToString();
                else
                    txtOpeningBalanceCode.Text = "";
        }
        private void txtCreditAccountCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
              ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
                if (reader.Read())
                    cmbCreditAccount.SelectedValue = int.Parse(reader["id"].ToString());
            
        }

        private void txtCreditAccountCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
                if (!reader.Read())
                    cmbCreditAccount.SelectedIndex = -1;
            
        }

        private void txtPurchasePrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && txtPurchasePrice.Text.Contains("."))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '-' && txtPurchasePrice.SelectionStart != 0)
            {
                e.Handled = true;
            }
        }

        private void txtDepreciationLife_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && txtDepreciationLife.Text.Contains("."))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '-' && txtDepreciationLife.SelectionStart != 0)
            {
                e.Handled = true;
            }
        }

        private void guna2GroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void cmbexpenceAccount_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
                if (!reader.Read())
                    cmbExpenceAccount.SelectedIndex = -1;
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
             ClinicDatabase.CreateParameterNew("code", txtExpenceAccount.Text)))
                if (reader.Read())
                    cmbExpenceAccount.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void txtInvoiceNumber_TextChanged(object sender, EventArgs e)
        {
            //
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedValue!=null && int.Parse(cmbCategory.SelectedValue.ToString()) != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_fixed_assets_category where id =@id", ClinicDatabase.CreateParameterNew("id", cmbCategory.SelectedValue.ToString())))
                {
                    if (reader.Read())
                    {
                        if (reader["assets_account_id"] != DBNull.Value && int.Parse(reader["assets_account_id"].ToString()) > 0)
                            cmbAccountName.SelectedValue = int.Parse(reader["assets_account_id"].ToString());

                        if (reader["depreciation_account_id"] != DBNull.Value && int.Parse(reader["depreciation_account_id"].ToString()) > 0)
                            cmbCreditAccount.SelectedValue = int.Parse(reader["depreciation_account_id"].ToString());

                        if (reader["expence_account_id"] != DBNull.Value && int.Parse(reader["expence_account_id"].ToString()) > 0)
                            cmbExpenceAccount.SelectedValue = int.Parse(reader["expence_account_id"].ToString());
                    }
                }
            }
        }

        private void cmbExpenceAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbExpenceAccount.SelectedValue == null)
            {
                txtExpenceAccount.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbExpenceAccount.SelectedValue.ToString()))
                if (reader.Read())
                    txtExpenceAccount.Text = reader["code"].ToString();
                else
                    txtExpenceAccount.Text = "";
        }
        private void ApplyFixedAssetArabicLayout()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool arabic = language.StartsWith(
                "ar",
                StringComparison.OrdinalIgnoreCase);

            if (!arabic)
                return;

            // All normal field labels
            Label[] labels =
            {
        label1,
        label2,
        label3,
        label4,
        label5,
        label6,
        label7,
        label8,
        label9,
        label10,
        label11,
        label12,
        label13,
        label14,
        label15,
        label16,
        label17,
        label18,
        label19,
        label20,
        label21,
        label24,
        label25
    };

            foreach (Label lbl in labels)
            {
                if (lbl == null)
                    continue;

                lbl.RightToLeft = RightToLeft.Yes;
                lbl.TextAlign = ContentAlignment.MiddleRight;

                // Give Arabic text more vertical room
                lbl.Height = 24;

                // Move label upward so it doesn't sit on textbox border
                lbl.Top -= 6;
            }

            // Arabic input direction
            txtAssetName.RightToLeft = RightToLeft.Yes;
            txtAssetBrand.RightToLeft = RightToLeft.Yes;
            txtModel.RightToLeft = RightToLeft.Yes;
            txtSupplierName.RightToLeft = RightToLeft.Yes;
            txtStatus.RightToLeft = RightToLeft.Yes;

            cmbCategory.RightToLeft = RightToLeft.Yes;
            cmbAccountName.RightToLeft = RightToLeft.Yes;
            cmbCreditAccount.RightToLeft = RightToLeft.Yes;
            cmbExpenceAccount.RightToLeft = RightToLeft.Yes;
            cmbOpeningBalance.RightToLeft = RightToLeft.Yes;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (id > 0)
            {
                //
            }
        }

        private void SelectAccountComboById(
     Guna.UI2.WinForms.Guna2ComboBox combo,
     int accountId)
        {
            if (combo == null)
                return;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is BindCombos.Level4AccountComboItem item &&
                    item.Id == accountId)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = -1;
        }
        private void Lbheader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
          
        }

    }
}
