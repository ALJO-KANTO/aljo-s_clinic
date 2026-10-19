using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;   // ADD THIS
using System.Linq;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmEditLevel4 : Form
    {
        private int id;
        // Original English Level 4 name from database
        private string originalLevel4Name = "";
        private bool level4NameWasEdited = false;
        private bool loadingLevel4 = false;
        public frmEditLevel4(int id)
        {
            InitializeComponent(); 
           // LocalizationManager.LocalizeForm(this);
            this.id = id;
            headerUC1.FormText = this.Text;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtAccount.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Level 4 Name First");
                return;
            }
            if(cmbLevel3.SelectedValue == null || cmbLevel3.SelectedValue.ToString() == "0")
            {
                MessageBox.Show("Please Select Level 3 Account");
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where name = @name",
                ClinicDatabase.CreateParameterNew("name", txtAccount.Text)))
                if (reader.Read())
                {
                    if ((id != int.Parse(reader["id"].ToString())))
                    {
                        MessageBox.Show("Name Already Exists. Enter Another Name.");
                        return;
                    }
                }
            ClinicDatabase.ExecuteNonQuery("update tbl_coa_level_4 set name=@name,debit=@debit,credit=@credit,date=@date,main_id=@main_id where id = @id",
                ClinicDatabase.CreateParameterNew("id", id),
            ClinicDatabase.CreateParameterNew("name", txtAccount.Text),
                                               ClinicDatabase.CreateParameterNew("debit", txtDebit.Text.ToString()),
                                               ClinicDatabase.CreateParameterNew("credit", txtCredit.Text),
                                               ClinicDatabase.CreateParameterNew("date", dtOpen.Value),
                                               ClinicDatabase.CreateParameterNew("main_id", cmbLevel3.SelectedValue));

            ////Utilities.LogAudit(-1, "Update Level 4 Account", "Level 4 Account", id, "Updated Level 4 Account: " + txtAccount.Text);
            if (!string.IsNullOrEmpty(txtCredit.Text.ToString().Trim()) || !string.IsNullOrEmpty(txtDebit.Text.ToString().Trim()))
            {
                if (txtCredit.Text == "")
                {
                    txtCredit.Text = "0";
                }
                if (txtDebit.Text == "")
                {
                    txtDebit.Text = "0";
                }
                insertTransactions(id);
            }

            //.Refreshlvl4Account();
            this.Close();
        }
        private void insertTransactions(int refId)
        {
            string accountId = refId.ToString();
            string openingBalanceEquity = BindCombos.SelectDefaultLevelAccount("Opening Balance Equity").ToString();


            decimal creditAmount = decimal.Parse(txtCredit.Text);
            decimal debitAmount = decimal.Parse(txtDebit.Text);
            if (int.Parse(openingBalanceEquity) == 0)
            {
                object result = ClinicDatabase.ExecuteScalar(@"SELECT id FROM tbl_coa_level_4 WHERE name = 'Opening Balance Equity'");
                if (result != null && result != DBNull.Value)
                {
                    openingBalanceEquity = result.ToString();
                }
            }
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_transaction where transaction_id=@refId and t_type='GENERAL LEDGER OPENING BALANCE';", ClinicDatabase.CreateParameterNew("refId", refId));
            if (int.Parse(openingBalanceEquity) > 0)
            {
                if (creditAmount != 0)
                {
                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, openingBalanceEquity, creditAmount.ToString(), "0",
                      refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Opening Balance Equity - Ledger", -1, DateTime.Now.Date, "");

                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, accountId, "0", creditAmount.ToString(),
                       refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Account Payable - Ledger Code ", -1, DateTime.Now.Date, "");
                }

                if (debitAmount != 0)
                {
                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, openingBalanceEquity, "0", debitAmount.ToString(),
                            refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Opening Balance Equity - Ledger Code ", -1, DateTime.Now.Date, "");

                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, accountId, debitAmount.ToString(), "0",
                       refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Account Payable - Ledger Code - ", -1, DateTime.Now.Date, "");
                }
            }
            else
            {
                MessageBox.Show("Cannot make opening balance without opening balance equity account");
            }
        }
        private void frmEditLevel4_Load(object sender, EventArgs e)
        {
            dtOpen.Value = DateTime.Now;

            if (id > 0)
            {
                btnDelete.Enabled = true;
                btnSave.Enabled = UserPermissions.canEdit("Chart Of Account");
            }
            else
            {
                btnDelete.Enabled = false;
            }

            btnDelete.Enabled =
                UserPermissions.canDelete("Chart Of Account");

            // =========================================================
            // LOAD LEVEL 3 ACCOUNTS
            // =========================================================
            LoadLevel3Accounts();

            // =========================================================
            // LOAD CURRENT LEVEL 4 ACCOUNT
            // =========================================================
            loadingLevel4 = true;

            try
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                    @"SELECT
                  l4.name,
                  l4.debit,
                  l4.credit,
                  l4.date,
                  l4.main_id AS lvl3_id
              FROM tbl_coa_level_4 l4
              WHERE l4.id = @id",
                    ClinicDatabase.CreateParameterNew("@id", id)))
                {
                    if (reader.Read())
                    {
                        // Keep original database name.
                        originalLevel4Name =
                            reader["name"].ToString();

                        // Arabic -> translated display
                        // English -> original database name
                        txtAccount.Text =
      IsArabicLanguage()
          ? TranslateLevel4AccountName(originalLevel4Name)
          : originalLevel4Name;

                        txtDebit.Text =
                            reader["debit"].ToString();

                        txtCredit.Text =
                            reader["credit"].ToString();

                        if (reader["date"] != DBNull.Value)
                        {
                            dtOpen.Value =
                                DateTime.Parse(reader["date"].ToString());
                        }

                        cmbLevel3.SelectedValue =
                            string.IsNullOrWhiteSpace(
                                reader["lvl3_id"].ToString())
                            ? 0
                            : Convert.ToInt32(reader["lvl3_id"]);
                    }
                }
               
                level4NameWasEdited = false;
            }
            finally
            {
                loadingLevel4 = false;
            }
            ApplyLevel4LanguageLayout();
        }
        private string TranslateLevel4AccountName(string englishName)
        {
            if (string.IsNullOrWhiteSpace(englishName))
                return "";

            string name = englishName.Trim();

            // =========================================================
            // EXACT LEVEL 4 ACCOUNT TRANSLATIONS
            // Always check these FIRST.
            // =========================================================
            var exactTranslations =
                new System.Collections.Generic.Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase)
                {
            // =====================================================
            // CASH / BANK
            // =====================================================
            { "Bank Account", "الحساب البنكي" },
            { "Bank Accounts", "الحسابات البنكية" },
            { "Cash", "النقدية" },
            { "Cash On Hand", "النقدية في الصندوق" },
            { "Petty Cash", "النقدية النثرية" },
            { "Cash & Cash Equivalents", "النقد وما في حكمه" },
            { "Cash and Cash Equivalents", "النقد وما في حكمه" },

            // =====================================================
            // RECEIVABLE
            // =====================================================
            { "Accounts Receivable", "الذمم المدينة" },
            { "Trade Receivable", "الذمم التجارية المدينة" },
            { "Customer Receivable", "ذمم العملاء" },
            { "Employee Receivable", "ذمم الموظفين" },
            { "Employee Receivables", "ذمم الموظفين" },
            { "Other Receivable", "ذمم مدينة أخرى" },
            { "Other Receivables", "ذمم مدينة أخرى" },
            { "Related Parties Receivable", "ذمم الأطراف ذات العلاقة" },

            // =====================================================
            // INVENTORY / MATERIAL
            // =====================================================
            { "Inventory", "المخزون" },
            { "Materials", "المواد" },
            { "Material", "المواد" },
            { "Raw Materials", "المواد الخام" },
            { "Finished Goods", "البضاعة التامة" },
            { "Finished Goods Inventory", "مخزون البضاعة التامة" },
            { "Spare Parts", "قطع الغيار" },
            { "Parts Inventory", "مخزون قطع الغيار" },
            { "Vehicle Inventory", "مخزون المركبات" },
            { "Prepayment", "المدفوعات المقدمة" },
            { "Prepayments", "المدفوعات المقدمة" },

            // =====================================================
            // FIXED ASSETS
            // =====================================================
            { "Fixed Assets", "الأصول الثابتة" },
            { "Fixed Assets Net", "صافي الأصول الثابتة" },
            { "Net Fixed Assets", "صافي الأصول الثابتة" },

            { "Furniture", "الأثاث" },
            { "Furniture & Fixtures", "الأثاث والتجهيزات" },
            { "Furniture and Fixtures", "الأثاث والتجهيزات" },

            { "Vehicles", "المركبات" },
            { "Motor Vehicles", "المركبات" },

            { "Equipment", "المعدات" },
            { "Office Equipment", "معدات المكتب" },
            { "Computer Equipment", "معدات الحاسب الآلي" },

            { "Building", "المباني" },
            { "Buildings", "المباني" },
            { "Land", "الأراضي" },

            // =====================================================
            // ACCUMULATED DEPRECIATION
            // =====================================================
            { "Accumulated Depreciation", "مجمع الإهلاك" },

            { "Accumulated Dep Fur", "مجمع إهلاك الأثاث" },
            { "Accumulated Dep Furniture", "مجمع إهلاك الأثاث" },
            { "Accumulated Depreciation Furniture", "مجمع إهلاك الأثاث" },

            { "Accumulated Dep Vehicle", "مجمع إهلاك المركبات" },
            { "Accumulated Dep Vehicles", "مجمع إهلاك المركبات" },
            { "Accumulated Depreciation Vehicles", "مجمع إهلاك المركبات" },

            { "Accumulated Dep Equipment", "مجمع إهلاك المعدات" },
            { "Accumulated Depreciation Equipment", "مجمع إهلاك المعدات" },

            { "Accumulated Dep Building", "مجمع إهلاك المباني" },
            { "Accumulated Depreciation Building", "مجمع إهلاك المباني" },

            // =====================================================
            // PAYABLE / SUPPLIERS
            // =====================================================
            { "Accounts Payable", "الذمم الدائنة" },
            { "Trade Payable", "الذمم التجارية الدائنة" },
            { "Trade Payables", "الذمم التجارية الدائنة" },

            { "Suppliers", "الموردون" },
            { "Supplier", "المورد" },

            { "Suppliers (Creditors)", "الموردون (الدائنون)" },
            { "Suppliers (Creditors) Net", "صافي حساب الموردين (الدائنين)" },
            { "Supplier (Creditors) Net", "صافي حساب الموردين (الدائنين)" },

            { "Other Payable", "ذمم دائنة أخرى" },
            { "Other Payables", "ذمم دائنة أخرى" },

            // =====================================================
            // EMPLOYEE / GRATUITY / PROVISIONS
            // =====================================================
            { "Gratuity Provision", "مخصص مكافأة نهاية الخدمة" },
            { "Provision for Gratuity", "مخصص مكافأة نهاية الخدمة" },
            { "Employee Gratuity Provision", "مخصص مكافأة نهاية خدمة الموظفين" },
            { "End of Service Provision", "مخصص مكافأة نهاية الخدمة" },
            { "End Of Service Benefits", "مكافآت نهاية الخدمة" },

            { "Accrued Employee Benefits Expense", "مصروفات منافع الموظفين المستحقة" },
            { "Payroll Liabilities", "التزامات الرواتب" },
            { "Accrued Expenses", "المصروفات المستحقة" },

            // =====================================================
            // TAX
            // =====================================================
            { "Tax Payable", "الضرائب المستحقة" },
            { "Sales Tax Payable", "ضريبة المبيعات المستحقة" },
            { "VAT Payable", "ضريبة القيمة المضافة المستحقة" },
            { "VAT Input", "ضريبة القيمة المضافة - مدخلات" },
            { "VAT Output", "ضريبة القيمة المضافة - مخرجات" },
            { "Input VAT", "ضريبة القيمة المضافة - مدخلات" },
            { "Output VAT", "ضريبة القيمة المضافة - مخرجات" },

            // =====================================================
            // EQUITY
            // =====================================================
            { "Capital", "رأس المال" },
            { "Share Capital", "رأس المال" },
            { "Members Draw", "مسحوبات الأعضاء" },
            { "Member Draw", "مسحوبات الأعضاء" },
            { "Drawings", "المسحوبات" },

            { "Opening Balance Equity", "حقوق ملكية الرصيد الافتتاحي" },
            { "Reserve", "الاحتياطي" },
            { "Retained Earnings", "الأرباح المحتجزة" },
            { "Current Year Earnings", "أرباح السنة الحالية" },

            // =====================================================
            // SALES / INCOME
            // =====================================================
            { "Sales", "المبيعات" },
            { "Sales - Local", "المبيعات المحلية" },
            { "Local Sales", "المبيعات المحلية" },
            { "Sales - Export", "مبيعات التصدير" },
            { "Export Sales", "مبيعات التصدير" },

            { "Parts Sales", "مبيعات قطع الغيار" },
            { "Vehicle Sales", "مبيعات المركبات" },
            { "Service Sales", "مبيعات الخدمات" },

            { "Sales Return", "مردودات المبيعات" },
            { "Sales Returns", "مردودات المبيعات" },

            { "Maintenance Income", "إيرادات الصيانة" },
            { "Service Income", "إيرادات الخدمات" },
            { "Other Income", "إيرادات أخرى" },
            { "Discount Received", "خصم مكتسب" },

            // =====================================================
            // PURCHASE
            // =====================================================
            { "Purchase", "المشتريات" },
            { "Purchases", "المشتريات" },
            { "Purchase Return", "مردودات المشتريات" },
            { "Purchase Returns", "مردودات المشتريات" },

            // =====================================================
            // COST
            // =====================================================
          { "Cost Of Goods Sold", "تكلفة البضاعة المباعة" },
{ "COGS", "تكلفة البضاعة المباعة" },

{ "Cost Of Sales", "تكلفة المبيعات" },

            { "Parts COGS", "تكلفة مبيعات قطع الغيار" },
            { "Vehicle COGS", "تكلفة مبيعات المركبات" },

            { "Direct Cost", "التكلفة المباشرة" },
            { "Direct Costs", "التكاليف المباشرة" },

            { "Freight Shipping Cost", "تكلفة الشحن" },
            { "Customs & Clearance", "الجمارك والتخليص" },
            { "Customs and Clearance", "الجمارك والتخليص" },
            { "Other Cost", "تكاليف أخرى" },
            { "Other Costs", "تكاليف أخرى" },

            // =====================================================
            // EXPENSES
            // =====================================================
            { "General Expenses", "المصروفات العامة" },
            { "General Expense", "المصروفات العامة" },

            { "Administrative Expenses", "المصروفات الإدارية" },
            { "Administrative Expense", "المصروفات الإدارية" },

            { "Salary", "الرواتب" },
            { "Salaries", "الرواتب" },
            { "Salary Expense", "مصروف الرواتب" },

            { "Rent", "الإيجار" },
            { "Rent Expense", "مصروف الإيجار" },

            { "Electricity", "الكهرباء" },
            { "Telephone", "الهاتف" },
            { "Internet", "الإنترنت" },
            { "Transportation", "النقل" },

            { "Depreciation", "الإهلاك" },
            { "Depreciation Expense", "مصروف الإهلاك" },

            { "Bank Charges", "رسوم بنكية" },
            { "Office Expenses", "مصروفات المكتب" },
            { "Office Expense", "مصروفات المكتب" },

            { "Maintenance", "الصيانة" },
            { "Maintenance Expense", "مصروفات الصيانة" },

            { "Insurance", "التأمين" },
            { "Insurance Expense", "مصروف التأمين" },

            // =====================================================
            // STOCK / LOSSES
            // =====================================================
            { "Loss", "الخسائر" },
            { "Losses", "الخسائر" },
            { "Stock Write-offs", "شطب المخزون" },
            { "Stock Write-off", "شطب المخزون" },
            { "Stock Adjustments", "تسويات المخزون" },
            { "Stock Adjustment", "تسويات المخزون" }
                };

            // =========================================================
            // 1. BEST OPTION - EXACT ACCOUNT TRANSLATION
            // =========================================================
            if (exactTranslations.TryGetValue(name, out string exactArabic))
                return exactArabic;

            // =========================================================
            // 2. FALLBACK FOR CUSTOM / COMBINED ACCOUNT NAMES
            // =========================================================
            var replacements =
                new System.Collections.Generic.Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase)
                {
            // Long phrases first
            { "Accumulated Depreciation", "مجمع الإهلاك" },
            { "Accumulated Dep", "مجمع إهلاك" },
            { "Opening Balance", "الرصيد الافتتاحي" },
            { "Cost Of Goods Sold", "تكلفة البضاعة المباعة" },
            { "Accounts Receivable", "الذمم المدينة" },
            { "Accounts Payable", "الذمم الدائنة" },
            { "Fixed Assets", "الأصول الثابتة" },
            { "Bank Account", "الحساب البنكي" },
            { "Bank Charges", "رسوم بنكية" },
            { "Sales Return", "مردودات المبيعات" },
            { "Purchase Return", "مردودات المشتريات" },
            { "General Expenses", "المصروفات العامة" },

            // Specific words
            { "Creditors", "الدائنون" },
            { "Debtors", "المدينون" },

            { "Gratuity", "مكافأة نهاية الخدمة" },
            { "Provision", "مخصص" },

            { "Furniture", "الأثاث" },
            { "Fur", "الأثاث" },
            { "Materials", "المواد" },
            { "Material", "المواد" },

            { "Suppliers", "الموردون" },
            { "Supplier", "المورد" },

            { "Customers", "العملاء" },
            { "Customer", "العميل" },

            { "Employees", "الموظفون" },
            { "Employee", "الموظف" },

            { "Vehicles", "المركبات" },
            { "Vehicle", "المركبة" },

            { "Equipment", "المعدات" },
            { "Building", "المباني" },
            { "Land", "الأراضي" },

            { "Sales", "المبيعات" },
            { "Purchases", "المشتريات" },
            { "Purchase", "المشتريات" },

            { "Inventory", "المخزون" },
            { "Stock", "المخزون" },

            { "Income", "الإيرادات" },
            { "Revenue", "الإيرادات" },

            { "Expenses", "المصروفات" },
            { "Expense", "المصروف" },

            { "Assets", "الأصول" },
            { "Asset", "الأصل" },

            { "Liabilities", "الخصوم" },
            { "Liability", "الالتزام" },

            { "Receivable", "مدينة" },
            { "Payable", "دائنة" },

            { "Capital", "رأس المال" },
            { "Equity", "حقوق الملكية" },

            { "Reserve", "الاحتياطي" },

            { "Maintenance", "الصيانة" },
            { "Service", "الخدمات" },

            { "Parts", "قطع الغيار" },

            { "Depreciation", "الإهلاك" },
            { "Dep", "إهلاك" },

            { "Bank", "البنك" },
            { "Cash", "النقدية" },

            { "Local", "محلية" },
            { "Export", "تصدير" },
            { "Import", "استيراد" },

            { "General", "عامة" },
            { "Direct", "مباشرة" },
            { "Other", "أخرى" },

            { "Current", "متداولة" },
            { "Net", "صافي" },

            { "Cost", "التكلفة" },
            { "Discount", "الخصم" },

            { "Account", "الحساب" }
                };

            foreach (var item in replacements
                .OrderByDescending(x => x.Key.Length))
            {
                name = ReplaceIgnoreCase(
                    name,
                    item.Key,
                    item.Value);
            }

            return name.Trim();
        }
        private string ReplaceIgnoreCase(
    string source,
    string oldValue,
    string newValue)
        {
            if (string.IsNullOrEmpty(source) ||
                string.IsNullOrEmpty(oldValue))
                return source;

            int index = source.IndexOf(
                oldValue,
                StringComparison.OrdinalIgnoreCase);

            while (index >= 0)
            {
                source =
                    source.Substring(0, index) +
                    newValue +
                    source.Substring(index + oldValue.Length);

                index = source.IndexOf(
                    oldValue,
                    index + newValue.Length,
                    StringComparison.OrdinalIgnoreCase);
            }

            return source;
        }
        private void LoadLevel3Accounts()
        {
            bool isArabic = IsArabicLanguage();

            DataTable table = new DataTable();

            table.Columns.Add("id", typeof(int));
            table.Columns.Add("display_name", typeof(string));

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                @"SELECT id, code, name
          FROM tbl_coa_level_3
          ORDER BY code"))
            {
                while (reader.Read())
                {
                    int accountId =
                        Convert.ToInt32(reader["id"]);

                    string code =
                        reader["code"].ToString();

                    string accountName =
                        reader["name"].ToString();

                    // Translate ONLY the displayed account name.
                    if (isArabic)
                    {
                        accountName =
                            GetArabicAccountName(accountName);
                    }

                    table.Rows.Add(
                        accountId,
                        code + " - " + accountName);
                }
            }

            cmbLevel3.DataSource = table;

            cmbLevel3.DisplayMember = "display_name";
            cmbLevel3.ValueMember = "id";

            // Arabic -> text/list aligned right
            // English -> text/list aligned left
            cmbLevel3.RightToLeft =
                isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtAccount.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Level 4 Name First");
                return;
            }
            object result = ClinicDatabase.ExecuteScalar(@"SELECT COUNT(1) FROM tbl_transaction 
                  WHERE type !='General Ledger Opening Balance' and account_id = @id", ClinicDatabase.CreateParameterNew("id", id));
            int recordCount = 0;
            if (result != null && result != DBNull.Value)
                recordCount = Convert.ToInt32(result);
            if (recordCount > 0)
            {
                MessageBox.Show("Already used");
                return;
            }
            ClinicDatabase.ExecuteNonQuery("delete from tbl_coa_level_4 where id = @id; delete from tbl_transaction WHERE type ='General Ledger Opening Balance' and transaction_id = @id",
            ClinicDatabase.CreateParameterNew("id", id));
            //.Refreshlvl4Account();
            this.Close();
        }
        private void txtDebit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }
        private bool IsArabicLanguage()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.StartsWith(
                "ar",
                StringComparison.OrdinalIgnoreCase);
        }

        private string GetArabicAccountName(string englishName)
        {
            if (string.IsNullOrWhiteSpace(englishName))
                return "";

            string key = englishName.Trim().ToLowerInvariant();

            switch (key)
            {
                // =====================================================
                // ASSETS
                // =====================================================
                case "assets":
                    return "الأصول";

                case "current":
                case "current assets":
                case "current asset":
                    return "الأصول المتداولة";

                case "non-current assets":
                case "non current assets":
                    return "الأصول غير المتداولة";

                case "fixed assets":
                    return "الأصول الثابتة";

                case "long-term assets":
                case "long term assets":
                    return "الأصول طويلة الأجل";

                case "banks":
                case "bank":
                    return "البنوك";

                case "petty cash":
                    return "النقدية النثرية";

                case "cash":
                    return "النقدية";

                case "cash on hand":
                    return "النقدية في الصندوق";

                case "cash & cash equivalents":
                case "cash and cash equivalents":
                    return "النقد وما في حكمه";

                case "accounts receivable":
                    return "الذمم المدينة";

                case "inventory":
                    return "المخزون";

                case "prepayment":
                case "prepayments":
                    return "المدفوعات المقدمة";

                case "other's receivable":
                case "other receivable":
                case "other receivables":
                    return "ذمم مدينة أخرى";

                case "related parties receivable":
                case "related party receivable":
                    return "ذمم الأطراف ذات العلاقة";

                case "employee receivable":
                case "employee receivables":
                    return "ذمم الموظفين";

                case "short term investment":
                case "short-term investment":
                case "short term investments":
                    return "استثمارات قصيرة الأجل";

                case "investment":
                case "investments":
                    return "الاستثمارات";

                case "property, plant and equipment":
                case "property plant and equipment":
                    return "الممتلكات والآلات والمعدات";

                case "accumulated depreciation":
                    return "مجمع الإهلاك";

                case "intangible assets":
                    return "الأصول غير الملموسة";


                // =====================================================
                // LIABILITIES
                // =====================================================
                case "liabilities":
                case "liability":
                    return "الخصوم";

                case "current liabilities":
                    return "الخصوم المتداولة";

                case "non-current liabilities":
                case "non current liabilities":
                    return "الخصوم غير المتداولة";

                case "long-term liabilities":
                case "long term liabilities":
                    return "الخصوم طويلة الأجل";

                case "accounts payable":
                    return "الذمم الدائنة";

                case "trade payable":
                case "trade payables":
                    return "الذمم التجارية الدائنة";

                case "accrued employee benefits expense":
                    return "مصروفات منافع الموظفين المستحقة";

                case "accrued expenses":
                    return "المصروفات المستحقة";

                case "other current liabilities":
                    return "خصوم متداولة أخرى";

                case "payroll liabilities":
                    return "التزامات الرواتب";

                case "other payable":
                case "other payables":
                    return "ذمم دائنة أخرى";

                case "long-term employee benefits":
                case "long term employee benefits":
                    return "منافع الموظفين طويلة الأجل";

                case "tax payable":
                    return "الضرائب المستحقة";

                case "sales tax payable":
                    return "ضريبة المبيعات المستحقة";

                case "vat payable":
                    return "ضريبة القيمة المضافة المستحقة";

                case "loan":
                case "loans":
                    return "القروض";

                case "suppliers":
                    return "الموردون";

                case "supplier":
                    return "المورد";
                // =====================================================
                // EQUITY
                // =====================================================
                case "equity":
                    return "حقوق الملكية";

                case "capital":
                case "share capital":
                    return "رأس المال";

                case "members draw":
                case "member draw":
                case "drawings":
                    return "مسحوبات الأعضاء";

                case "opening balance equity":
                    return "حقوق ملكية الرصيد الافتتاحي";

                case "reserve":
                    return "الاحتياطي";

                case "retained earnings":
                    return "الأرباح المحتجزة";

                case "current year earnings":
                    return "أرباح السنة الحالية";


                // =====================================================
                // INCOME / REVENUE
                // =====================================================
                case "income":
                case "revenue":
                    return "الإيرادات";

                case "sales":
                case "sales income":
                case "sales revenue":
                    return "المبيعات";

                case "sales return":
                case "sales returns":
                    return "مردودات المبيعات";

                case "maintenance income":
                    return "إيرادات الصيانة";

                case "service income":
                case "service revenue":
                    return "إيرادات الخدمات";

                case "other income":
                    return "إيرادات أخرى";

                case "discount received":
                    return "خصم مكتسب";


                // =====================================================
                // COST
                // =====================================================
                case "cost":
                    return "التكلفة";

                case "cost of sales":
                    return "تكلفة المبيعات";

                case "cost of goods sold":
                case "cogs":
                    return "تكلفة البضاعة المباعة";

                case "direct cost":
                case "direct costs":
                    return "التكاليف المباشرة";

                case "freight shipping cost":
                    return "تكلفة الشحن";

                case "customs & clearance":
                case "customs and clearance":
                    return "الجمارك والتخليص";

                case "other cost":
                case "other costs":
                    return "تكاليف أخرى";


                // =====================================================
                // EXPENSES
                // =====================================================
                case "expenses":
                case "expense":
                    return "المصروفات";

                case "general & direct expenses":
                case "general and direct expenses":
                    return "المصروفات العامة والمباشرة";

                case "general expenses":
                case "general expense":
                    return "المصروفات العامة";

                case "administrative expenses":
                case "administrative expense":
                    return "المصروفات الإدارية";

                case "salary":
                case "salaries":
                case "salary expense":
                    return "الرواتب";

                case "rent":
                case "rent expense":
                    return "مصروف الإيجار";

                case "electricity":
                    return "الكهرباء";

                case "telephone":
                    return "الهاتف";

                case "internet":
                    return "الإنترنت";

                case "transportation":
                    return "النقل";

                case "depreciation":
                case "depreciation expense":
                    return "مصروف الإهلاك";

                case "bank charges":
                    return "رسوم بنكية";

                case "office expenses":
                case "office expense":
                    return "مصروفات المكتب";

                case "maintenance":
                case "maintenance expense":
                    return "مصروفات الصيانة";

                case "insurance":
                case "insurance expense":
                    return "مصروف التأمين";


                // =====================================================
                // LOSSES / STOCK
                // =====================================================
                case "loss":
                case "losses":
                    return "الخسائر";

                case "stock write-off":
                case "stock write-offs":
                case "stock writeoff":
                case "stock writeoffs":
                    return "شطب المخزون";

                case "stock adjustment":
                case "stock adjustments":
                    return "تسويات المخزون";


                // =====================================================
                // UNKNOWN ACCOUNT
                // =====================================================
                default:
                    return englishName;
            }
        }
        private void ApplyLevel4LanguageLayout()
        {
            bool arabic = IsArabicLanguage();

            // =========================================================
            // DO NOT MOVE INPUT CONTROLS
            // Keep Designer positions exactly as they are
            // =========================================================

            if (arabic)
            {
                // =====================================================
                // LEVEL 3
                // =====================================================
                label5.Text = "المستوى 3";
                label5.AutoSize = true;
                label5.RightToLeft = RightToLeft.Yes;

                // =====================================================
                // LEVEL 4 NAME
                // =====================================================
                label2.Text = "أدخل اسم المستوى 4";
                label2.AutoSize = true;
                label2.RightToLeft = RightToLeft.Yes;

                txtAccount.RightToLeft = RightToLeft.Yes;
                txtAccount.TextAlign = HorizontalAlignment.Right;

                // =====================================================
                // BOTTOM ROW
                //
                // Arabic:
                // [ DATE ]     [ CREDIT ]     [ DEBIT ]
                // =====================================================

                label3.Text = "تاريخ الرصيد";
                label4.Text = "دائن";
                label1.Text = "مدين";

                label3.AutoSize = true;
                label4.AutoSize = true;
                label1.AutoSize = true;

                label3.RightToLeft = RightToLeft.Yes;
                label4.RightToLeft = RightToLeft.Yes;
                label1.RightToLeft = RightToLeft.Yes;

                // -----------------------------------------------------
                // Put each label directly ABOVE its existing control
                // -----------------------------------------------------

                label3.Location = new Point(
                    dtOpen.Left + dtOpen.Width - label3.PreferredWidth,
                    dtOpen.Top - label3.PreferredHeight - 4);

                label4.Location = new Point(
                    txtCredit.Left + txtCredit.Width - label4.PreferredWidth,
                    txtCredit.Top - label4.PreferredHeight - 4);

                label1.Location = new Point(
                    txtDebit.Left + txtDebit.Width - label1.PreferredWidth,
                    txtDebit.Top - label1.PreferredHeight - 4);

                // Textbox direction
                txtCredit.RightToLeft = RightToLeft.No;
                txtDebit.RightToLeft = RightToLeft.No;

                txtCredit.TextAlign = HorizontalAlignment.Left;
                txtDebit.TextAlign = HorizontalAlignment.Left;

                // =====================================================
                // BUTTONS
                // =====================================================
                btnSave.Text = "حفظ";
                btnDelete.Text = "حذف";
                btnClose.Text = "إغلاق";
            }
            else
            {
                // =====================================================
                // ENGLISH
                // =====================================================

                label5.Text = "LEVEL 3";
                label5.AutoSize = true;
                label5.RightToLeft = RightToLeft.No;

                label2.Text = "Enter Level 4 Name";
                label2.AutoSize = true;
                label2.RightToLeft = RightToLeft.No;

                txtAccount.RightToLeft = RightToLeft.No;
                txtAccount.TextAlign = HorizontalAlignment.Left;

                // =====================================================
                // BOTTOM ROW
                // =====================================================

                label1.Text = "Debit";
                label4.Text = "Credit";
                label3.Text = "Date";

                label1.AutoSize = true;
                label4.AutoSize = true;
                label3.AutoSize = true;

                label1.RightToLeft = RightToLeft.No;
                label4.RightToLeft = RightToLeft.No;
                label3.RightToLeft = RightToLeft.No;

                // Put labels directly above existing controls
                label1.Location = new Point(
                    txtDebit.Left,
                    txtDebit.Top - label1.PreferredHeight - 4);

                label4.Location = new Point(
                    txtCredit.Left,
                    txtCredit.Top - label4.PreferredHeight - 4);

                label3.Location = new Point(
                    dtOpen.Left,
                    dtOpen.Top - label3.PreferredHeight - 4);

                txtDebit.RightToLeft = RightToLeft.No;
                txtCredit.RightToLeft = RightToLeft.No;

                txtDebit.TextAlign = HorizontalAlignment.Left;
                txtCredit.TextAlign = HorizontalAlignment.Left;

                // =====================================================
                // BUTTONS
                // =====================================================
                btnSave.Text = "Save";
                btnDelete.Text = "Delete";
                btnClose.Text = "Close";
            }
        }
        private void txtCredit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void txtDebit_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCredit_TextChanged(object sender, EventArgs e)
        {

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

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }
    }
}
