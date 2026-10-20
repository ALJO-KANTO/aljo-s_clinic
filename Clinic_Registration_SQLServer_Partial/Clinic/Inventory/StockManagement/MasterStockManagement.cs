using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterStockManagement : Form
    {
        private bool? _languageOverride;

        public MasterStockManagement()
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            ApplyStockManagementLocalization(this);
            if (btnNew != null)
                btnNew.Text = IsArabicCulture() ? "جديد" : "New";
            headerUC1.FormText = this.Text;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmAddStockSettlement());
        }

        private void MasterStockManagement_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvCustomer);
            //AppTheme.DefaultFont(dgvItems);
            LocalizationManager.LocalizeForm(this);
            ApplyStockManagementLocalization(this);
            if (btnNew != null)
                btnNew.Text = IsArabicCulture() ? "جديد" : "New";
            headerUC1.FormText = this.Text;
            BindInvoices();
        }

        private bool IsArabicCulture()
        {
            if (_languageOverride.HasValue)
                return _languageOverride.Value;

            // In this application the language button shows the language to
            // switch TO, not the language currently being displayed:
            // "English" means the application is currently in Arabic;
            // "العربية" means the application is currently in English.
            try
            {
                foreach (Form openForm in Application.OpenForms)
                {
                    string switchText = FindLanguageSwitchText(openForm);

                    if (string.Equals(switchText, "English", StringComparison.OrdinalIgnoreCase))
                        return true;

                    if (string.Equals(switchText, "العربية", StringComparison.Ordinal))
                        return false;
                }
            }
            catch
            {
                // Fall back to the saved language setting below.
            }

            string language = Convert.ToString(
                Clinic.Properties.Settings.Default.ApplicationLanguage);

            return !string.IsNullOrWhiteSpace(language) &&
                   (language.Trim().Equals("Arabic", StringComparison.OrdinalIgnoreCase) ||
                    language.Trim().Equals("AR", StringComparison.OrdinalIgnoreCase) ||
                    language.Trim().StartsWith("ar-", StringComparison.OrdinalIgnoreCase) ||
                    language.Trim().StartsWith("ar_", StringComparison.OrdinalIgnoreCase));
        }

        private string FindLanguageSwitchText(Control parent)
        {
            if (parent == null)
                return string.Empty;

            foreach (Control control in parent.Controls)
            {
                string text = (control.Text ?? string.Empty).Trim();

                if (string.Equals(text, "English", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(text, "العربية", StringComparison.Ordinal))
                {
                    return text;
                }

                if (control.HasChildren)
                {
                    string nestedText = FindLanguageSwitchText(control);
                    if (!string.IsNullOrEmpty(nestedText))
                        return nestedText;
                }
            }

            return string.Empty;
        }

        // The main form may call this after changing the application language.
        public void ApplyLanguage(bool arabic)
        {
            _languageOverride = arabic;
            LocalizationManager.LocalizeForm(this);
            ApplyStockManagementLocalization(this);
            if (btnNew != null)
                btnNew.Text = IsArabicCulture() ? "جديد" : "New";
            headerUC1.FormText = this.Text;

            if (dgvCustomer != null && dgvCustomer.Columns.Count > 0)
                ApplyStockGridHeaderLocalization(dgvCustomer);

            if (dgvItems != null && dgvItems.Columns.Count > 0)
                ApplyStockGridHeaderLocalization(dgvItems);
        }

        private bool IsDefaultSelectionMethod()
        {
            return string.Equals(cmbSelectionMethod.Text, "Default", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(cmbSelectionMethod.Text, "افتراضي", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyStockManagementLocalization(Control parent)
        {
            if (parent == null)
                return;

            bool arabic = IsArabicCulture();
            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Inventory - Stock Management", "المخزون - إدارة المخزون" },
                { "Stock Management", "إدارة المخزون" },
                { "From", "من" },
                { "To", "إلى" },
                { "Selection Method", "طريقة الاختيار" },
                { "All", "الكل" },
                { "Default", "افتراضي" },
                { "New", "جديد" },
                { "Edit Current", "تعديل الحالي" },
                { "Delete", "حذف" },
                { "Recycle", "استعادة" }
            };

            var englishTexts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "المخزون - إدارة المخزون", "Inventory - Stock Management" },
                { "إدارة المخزون", "Stock Management" },
                { "من", "From" },
                { "إلى", "To" },
                { "طريقة الاختيار", "Selection Method" },
                { "الكل", "All" },
                { "افتراضي", "Default" },
                { "جديد", "New" },
                { "تعديل الحالي", "Edit Current" },
                { "حذف", "Delete" },
                { "استعادة", "Recycle" }
            };

            // Translate the form's own title as well as its child controls.
            if (!string.IsNullOrWhiteSpace(parent.Text))
            {
                string parentText = parent.Text.Trim();
                string translatedParentText;
                if (arabic && translations.TryGetValue(parentText, out translatedParentText))
                    parent.Text = translatedParentText;
                else if (!arabic && englishTexts.TryGetValue(parentText, out translatedParentText))
                    parent.Text = translatedParentText;
            }

            foreach (Control control in parent.Controls)
            {
                if (!string.IsNullOrWhiteSpace(control.Text))
                {
                    string translated;
                    if (arabic && translations.TryGetValue(control.Text.Trim(), out translated))
                        control.Text = translated;
                    else if (!arabic && englishTexts.TryGetValue(control.Text.Trim(), out translated))
                        control.Text = translated;
                }

                if (control.HasChildren)
                    ApplyStockManagementLocalization(control);
            }

            // Explicitly translate the New button by control name. This avoids
            // relying on its current Text value, which may be reset by the designer
            // or another localization call.
            if (parent == this && btnNew != null)
            {
                btnNew.Text = arabic ? "جديد" : "New";
            }

            // ComboBox items are translated too; the query logic recognizes both values.
            if (parent == this && cmbSelectionMethod != null)
            {
                for (int i = 0; i < cmbSelectionMethod.Items.Count; i++)
                {
                    string itemText = Convert.ToString(cmbSelectionMethod.Items[i]);
                    if (arabic && string.Equals(itemText, "Default", StringComparison.OrdinalIgnoreCase))
                        cmbSelectionMethod.Items[i] = "افتراضي";
                    else if (!arabic && string.Equals(itemText, "افتراضي", StringComparison.OrdinalIgnoreCase))
                        cmbSelectionMethod.Items[i] = "Default";
                }
            }
        }

        private void ApplyStockGridHeaderLocalization(DataGridView grid)
        {
            if (grid == null || grid.Columns.Count == 0)
                return;

            var arabicHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "SN", "م" },
                { "Date", "التاريخ" },
                { "JV NO", "رقم القيد" },
                { "INV NO", "رقم الفاتورة" },
                { "Item Name", "اسم الصنف" },
                { "Qty", "الكمية" },
                { "Cost Price", "سعر التكلفة" },
                { "On_Hand", "الرصيد المتوفر" },
                { "new_on_hand", "الرصيد الجديد" },
                { "minusamount", "كمية صادرة" },
                { "plusamount", "كمية واردة" }
            };

            var englishHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "SN", "SN" },
                { "Date", "Date" },
                { "JV NO", "JV NO" },
                { "INV NO", "INV NO" },
                { "Item Name", "Item Name" },
                { "Qty", "Qty" },
                { "Cost Price", "Cost Price" },
                { "On_Hand", "On_Hand" },
                { "new_on_hand", "new_on_hand" },
                { "minusamount", "minusamount" },
                { "plusamount", "plusamount" }
            };

            bool arabic = IsArabicCulture();
            foreach (DataGridViewColumn column in grid.Columns)
            {
                string key = !string.IsNullOrWhiteSpace(column.DataPropertyName)
                    ? column.DataPropertyName.Trim()
                    : column.Name.Trim();
                string headerKey = column.HeaderText == null ? string.Empty : column.HeaderText.Trim();
                string translated;
                if (arabic)
                {
                    if (arabicHeaders.TryGetValue(key, out translated) || arabicHeaders.TryGetValue(headerKey, out translated))
                        column.HeaderText = translated;
                }
                else
                {
                    if (englishHeaders.TryGetValue(key, out translated) || englishHeaders.TryGetValue(headerKey, out translated))
                        column.HeaderText = translated;
                }
            }
        }

        public void BindInvoices()
        {
            DataTable dt;
            string query = "";
            if (IsDefaultSelectionMethod())
                query = @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_item_stock_settlement.date) AS `SN`, 
                        tbl_item_stock_settlement.date AS Date, tbl_item_stock_settlement.id,
                        LPAD(tbl_item_stock_settlement.id, 4, '0') AS 'JV NO', 
                        tbl_item_stock_settlement.code AS 'INV NO'
                        FROM tbl_item_stock_settlement
                        WHERE tbl_item_stock_settlement.state = 0 ";
            else
                query = @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_item_stock_settlement.date) AS `SN`, 
                        tbl_item_stock_settlement.date AS Date, 
                        tbl_item_stock_settlement.id,
                        tbl_item_stock_settlement.code AS 'INV NO', 
                        concat(ti.code,' - ',ti.name) as 'Item Name' ,ts.On_Hand As Qty,ts.price 
                        As 'Cost Price' , ts.new_on_hand  ,ts.minusamount ,ts.plusamount
                        FROM tbl_item_stock_settlement
                        INNER JOIN tbl_item_stock_settlement_details ts ON tbl_item_stock_settlement.id = ts.settle_id
                        inner join tbl_items ti on ts.item_id =ti.id
                        WHERE tbl_item_stock_settlement.state = 0";
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if (!chkDate.Checked)
                query += " and tbl_item_stock_settlement.date >= @dateFrom and tbl_item_stock_settlement.date <= @dateTo";
            if (IsDefaultSelectionMethod())
                query += " GROUP BY tbl_item_stock_settlement.id, tbl_item_stock_settlement.date, tbl_item_stock_settlement.code; ";
            else
                query += " GROUP BY tbl_item_stock_settlement.id, tbl_item_stock_settlement.date, tbl_item_stock_settlement.code,ti.code,ti.name,ts.on_hand,ts.price,ts.new_on_hand  ,ts.minusamount ,ts.plusamount; ";
            parameters.Add(ClinicDatabase.CreateParameterNew("dateFrom", dtFrom.Value.Date));
            parameters.Add(ClinicDatabase.CreateParameterNew("dateTo", dtTo.Value.Date));
            dt = ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
            dgvCustomer.DataSource = dt;
            if (dgvCustomer.Columns.Contains("id"))
                dgvCustomer.Columns["id"].Visible = false;

            if (dgvCustomer.Rows.Count > 0 && IsDefaultSelectionMethod())
                bindInvoiceItems();
            else
                dgvItems.DataSource = null;

            if (dgvCustomer.Rows.Count > 0)
            {
                btnEdit.Enabled = UserPermissions.canEdit("Stock Management");
            }
            LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
            ApplyStockGridHeaderLocalization(dgvCustomer);
            ApplyStockGridHeaderLocalization(dgvItems);
        }

        private void bindInvoiceItems()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"select concat(ti.code,' - ' ,ti.name) as 'Item Name' ,ts.on_hand as Qty ,ts.price as 'Cost Price' , ts.new_on_hand  from 
                                                     tbl_item_stock_settlement_details ts inner join tbl_items ti on ts.item_id = ti.id where settle_id = @id",
                                                     ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            dgvItems.DataSource = dt;
            dgvItems.Columns["item name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            LocalizationManager.LocalizeDataGridViewHeaders(dgvItems);
            ApplyStockGridHeaderLocalization(dgvItems);
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindInvoices();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmAddStockSettlement(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }

        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmAddStockSettlement(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
            if (dgvCustomer.Rows.Count == 0)
                return;
        }

        private void dgvCustomer_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void dgvCustomer_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            //ClinicDatabase.ExecuteNonQuery("UPDATE tbl_items SET state = -1 WHERE id = @id ",
            //                              ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            //BindInvoices();
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count > 0 && IsDefaultSelectionMethod())
                bindInvoiceItems();
            else
                dgvItems.DataSource = null;
        }
    }
}
