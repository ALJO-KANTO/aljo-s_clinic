using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;
using YamyProject.UI.Inventory.InventoryManagement;
using Excel = Microsoft.Office.Interop.Excel;

namespace YamyProject
{
    public partial class MasterInventory : Form
    {
        private DataView _dataView;
        private EventHandler itemUpdatedHandler;
        private EventHandler invoiceUpdatedHandler;
        private EventHandler purchaseUpdatedHandler;
        public MasterInventory()
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            ApplyInventoryLocalization();
            itemUpdatedHandler = (sender, args) => BindItems();
            invoiceUpdatedHandler = purchaseUpdatedHandler = (sender, args) => BindItems();
            //.Item += itemUpdatedHandler;
            //.SalesInv += invoiceUpdatedHandler;
            //.PurchaseInv += purchaseUpdatedHandler;
            headerUC1.FormText = this.Text;
        }
        private void MasterInventory_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.Item -= itemUpdatedHandler;
            //.SalesInv -= invoiceUpdatedHandler;
            //.PurchaseInv -= purchaseUpdatedHandler;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {

        }
        private void MasterInventory_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvDetails);
            BindCombos.PopulateItemsCategory(cmbCategory);
            ApplyInventoryLocalization();
            BindItems();
            ApplyInventoryLocalization();
        }
        private bool IsArabicLanguage()
        {
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            return language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        }

        // Applies translations to visible UI text only. Database values and query logic remain unchanged.
        private void ApplyInventoryLocalization()
        {
            bool arabic = IsArabicLanguage();
            this.Text = arabic ? "مركز المخزون" : "Inventory Center";
            this.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            this.RightToLeftLayout = arabic;

            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Inventory Center", "مركز المخزون" },
                { "Inventory", "المخزون" },
                { "Search By Item Name", "البحث حسب الصنف" },
                { "Item Code", "رمز الصنف" },
                { "Item Name", "اسم الصنف" },
                { "Barcode", "الباركود" },
                { "Cost Price", "سعر التكلفة" },
                { "Sale Price", "سعر البيع" },
                { "Sales Price", "سعر البيع" },
                { "Balance Qty", "الرصيد الكمي" },
                { "Go To Item Card", "الانتقال إلى بطاقة الصنف" },
                { "Item Operations", "عمليات الأصناف" },
                { "Excel", "إكسل" },
                { "Print", "طباعة" },
                { "INV NO", "رقم الفاتورة" },
                { "Date", "التاريخ" },
                { "Type", "النوع" },
                { "State", "الحالة" },
                { "Category", "الفئة" },
                { "All", "الكل" },
                { "Active", "نشط" },
                { "Inactive", "غير نشط" },
                { "Code", "الرمز" },
                { "Name", "الاسم" },
                { "Unit", "الوحدة" },
                { "Qty", "الكمية" },
                { "Qty In", "كمية واردة" },
                { "Qty Out", "كمية صادرة" },
                { "Qty Balance", "رصيد الكمية" },
                { "Reference", "المرجع" },
                { "New", "جديد" },
                { "Edit", "تعديل" },
                { "Delete", "حذف" },
                { "Recycle", "استعادة" },
                { "Add Item", "إضافة صنف" },
                { "Add New Item", "إضافة صنف جديد" },
                { "Item List", "قائمة الأصناف" },
                { "Customer List", "قائمة العملاء" },
                { "Export", "تصدير" },
                { "Import", "استيراد" },
                { "PDF", "PDF" },
                { "Product Name:", "اسم المنتج:" },
                { "Balance:", "الرصيد:" },
                { "Opening Qty", "الكمية الافتتاحية" },
                { "Purchase Invoice", "فاتورة شراء" },
                { "Service", "خدمة" },
                { "Inventory Assembly", "تجميع المخزون" }
            };

            ApplyTranslationsToControls(this.Controls, translations, arabic);
            // Explicitly localize this label as it may be hosted inside a custom panel/control.
            ForceSearchByItemTranslation(this.Controls, arabic);
            ApplyStateComboLanguage(arabic);
            if (headerUC1 != null)
                headerUC1.FormText = this.Text;
            ApplyInventoryGridHeaders(dgvCustomer);
            ApplyInventoryGridHeaders(dgvDetails);
        }


        private void ForceSearchByItemTranslation(Control.ControlCollection controls, bool arabic)
        {
            foreach (Control control in controls)
            {
                string text = (control.Text ?? string.Empty).Trim();
                if (string.Equals(text, "Search By Item Name", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(text, "البحث حسب الصنف", StringComparison.Ordinal))
                {
                    control.Text = arabic ? "البحث حسب الصنف" : "Search By Item Name";
                    control.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
                    if (control is Label label)
                        label.TextAlign = arabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                }

                if (control is ToolStrip toolStrip)
                    ForceSearchByItemToolStripTranslation(toolStrip.Items, arabic);
                if (control is ContextMenuStrip contextMenu)
                    ForceSearchByItemToolStripTranslation(contextMenu.Items, arabic);

                if (control.HasChildren)
                    ForceSearchByItemTranslation(control.Controls, arabic);
            }
        }

        private void ForceSearchByItemToolStripTranslation(ToolStripItemCollection items, bool arabic)
        {
            foreach (ToolStripItem item in items)
            {
                string text = (item.Text ?? string.Empty).Trim();
                if (string.Equals(text, "Search By Item Name", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(text, "البحث حسب الصنف", StringComparison.Ordinal))
                    item.Text = arabic ? "البحث حسب الصنف" : "Search By Item Name";

                if (item is ToolStripDropDownItem dropDown)
                    ForceSearchByItemToolStripTranslation(dropDown.DropDownItems, arabic);
            }
        }

        private void ApplyStateComboLanguage(bool arabic)
        {
            if (cmbState == null || cmbState.DataSource != null || cmbState.Items.Count == 0)
                return;

            int selectedIndex = cmbState.SelectedIndex;
            for (int i = 0; i < cmbState.Items.Count; i++)
            {
                string itemText = Convert.ToString(cmbState.Items[i]);
                if (string.Equals(itemText, "All", StringComparison.OrdinalIgnoreCase) || itemText == "الكل")
                    cmbState.Items[i] = arabic ? "الكل" : "All";
                else if (string.Equals(itemText, "Active", StringComparison.OrdinalIgnoreCase) || itemText == "نشط")
                    cmbState.Items[i] = arabic ? "نشط" : "Active";
                else if (string.Equals(itemText, "Inactive", StringComparison.OrdinalIgnoreCase) || itemText == "غير نشط")
                    cmbState.Items[i] = arabic ? "غير نشط" : "Inactive";
            }
            if (selectedIndex >= 0 && selectedIndex < cmbState.Items.Count)
                cmbState.SelectedIndex = selectedIndex;
        }

        private void ApplyTranslationsToControls(Control.ControlCollection controls, Dictionary<string, string> translations, bool arabic)
        {
            foreach (Control control in controls)
            {
                string original = (control.Text ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(original))
                {
                    string english = GetInventoryEnglishText(original);
                    if (!translations.ContainsKey(english))
                        english = MatchInventoryText(original, translations);

                    if (!string.IsNullOrEmpty(english) && translations.ContainsKey(english))
                        control.Text = arabic ? translations[english] : english;
                    else if (!arabic && translations.ContainsValue(original))
                        control.Text = GetInventoryEnglishText(original);
                }

                if (arabic)
                {
                    if (control is Label || control is CheckBox || control is RadioButton || control is Button)
                        control.RightToLeft = RightToLeft.Yes;
                }
                else
                {
                    if (control is Label || control is CheckBox || control is RadioButton || control is Button)
                        control.RightToLeft = RightToLeft.No;
                }

                if (control is DataGridView grid)
                    ApplyInventoryGridHeaders(grid);

                if (control is MenuStrip menuStrip)
                    ApplyToolStripTranslations(menuStrip.Items, translations, arabic);
                else if (control is ToolStrip toolStrip)
                    ApplyToolStripTranslations(toolStrip.Items, translations, arabic);
                else if (control is ContextMenuStrip contextMenu)
                    ApplyToolStripTranslations(contextMenu.Items, translations, arabic);

                if (control.HasChildren)
                    ApplyTranslationsToControls(control.Controls, translations, arabic);
            }
        }

        private string GetInventoryEnglishText(string text)
        {
            switch (text)
            {
                case "مركز المخزون": return "Inventory Center";
                case "البحث حسب الصنف": return "Search By Item Name";
                case "رمز الصنف": return "Item Code";
                case "اسم الصنف": return "Item Name";
                case "الباركود": return "Barcode";
                case "Barcode": return "Barcode";
                case "سعر التكلفة": return "Cost Price";
                case "سعر البيع": return "Sales Price";
                case "الرصيد الكمي": return "Balance Qty";
                case "الانتقال إلى بطاقة الصنف": return "Go To Item Card";
                case "عمليات الأصناف": return "Item Operations";
                case "إكسل": return "Excel";
                case "طباعة": return "Print";
                case "رقم الفاتورة": return "INV NO";
                case "التاريخ": return "Date";
                case "النوع": return "Type";
                case "الحالة": return "State";
                case "الفئة": return "Category";
                case "الكل": return "All";
                case "نشط": return "Active";
                case "غير نشط": return "Inactive";
                case "الرمز": return "Code";
                case "الاسم": return "Name";
                case "الوحدة": return "Unit";
                case "الكمية": return "Qty";
                case "كمية واردة": return "Qty In";
                case "كمية صادرة": return "Qty Out";
                case "رصيد الكمية": return "Qty Balance";
                case "المرجع": return "Reference";
                case "جديد": return "New";
                case "تعديل": return "Edit";
                case "حذف": return "Delete";
                case "استعادة": return "Recycle";
                case "إضافة صنف": return "Add Item";
                case "إضافة صنف جديد": return "Add New Item";
                case "قائمة الأصناف": return "Item List";
                case "قائمة العملاء": return "Customer List";
                case "تصدير": return "Export";
                case "استيراد": return "Import";
                case "اسم المنتج:": return "Product Name:";
                case "الرصيد:": return "Balance:";
                case "الكمية الافتتاحية": return "Opening Qty";
                case "فاتورة شراء": return "Purchase Invoice";
                case "خدمة": return "Service";
                case "تجميع المخزون": return "Inventory Assembly";
                default: return text;
            }
        }

        private string MatchInventoryText(string text, Dictionary<string, string> translations)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            string normalized = text.Replace("&", "").Trim();
            string exact = GetInventoryEnglishText(normalized);
            if (translations.ContainsKey(exact)) return exact;

            // Handles toolbar captions such as "Print ▼" or labels with trailing ellipses.
            string withoutEllipsis = normalized.TrimEnd('.', '…').Trim();
            exact = GetInventoryEnglishText(withoutEllipsis);
            if (translations.ContainsKey(exact)) return exact;

            foreach (string key in translations.Keys)
            {
                if (normalized.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                {
                    string suffix = normalized.Substring(key.Length).Trim();
                    if (suffix.Length == 0 || suffix.All(ch => !char.IsLetterOrDigit(ch)))
                        return key;
                }
            }
            return string.Empty;
        }

        private void ApplyToolStripTranslations(ToolStripItemCollection items, Dictionary<string, string> translations, bool arabic)
        {
            foreach (ToolStripItem item in items)
            {
                string text = (item.Text ?? string.Empty).Trim();
                if (text.Length > 0)
                {
                    string english = GetInventoryEnglishText(text);
                    if (!translations.ContainsKey(english))
                        english = MatchInventoryText(text, translations);

                    if (!string.IsNullOrEmpty(english) && translations.ContainsKey(english))
                        item.Text = arabic ? translations[english] : english;
                }

                if (item is ToolStripDropDownItem dropDown)
                    ApplyToolStripTranslations(dropDown.DropDownItems, translations, arabic);
            }
        }

        private void ApplyInventoryGridHeaders(DataGridView grid)
        {
            if (grid == null) return;
            bool arabic = IsArabicLanguage();
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Item Name", "اسم الصنف" }, { "Barcode", "الباركود" }, { "Type", "النوع" },
                { "Date", "التاريخ" }, { "Inv_no", "رقم الفاتورة" }, { "INV NO", "رقم الفاتورة" },
                { "Reference", "المرجع" }, { "Qty In", "كمية واردة" }, { "Qty Out", "كمية صادرة" },
                { "Cost Price", "سعر التكلفة" }, { "Sales Price", "سعر البيع" }, { "Qty", "الكمية" },
                { "Unit", "الوحدة" }, { "Qty Balance", "رصيد الكمية" }
            };
            foreach (DataGridViewColumn column in grid.Columns)
            {
                string key = (column.HeaderText ?? column.Name ?? string.Empty).Trim();
                if (!headers.ContainsKey(key)) key = (column.Name ?? string.Empty).Trim();
                if (headers.ContainsKey(key))
                {
                    string arabicText = headers[key];
                    column.HeaderText = arabic ? arabicText : GetInventoryEnglishText(arabicText);
                }
            }
            grid.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            grid.ColumnHeadersDefaultCellStyle.Alignment = arabic ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft;
        }

        private void ShowInventoryMessage(string message)
        {
            if (IsArabicLanguage())
            {
                var translations = new Dictionary<string, string>
                {
                    { "Item already used cannot delete! \n you can inactive the Item", "لا يمكن حذف الصنف لأنه مستخدم بالفعل. يمكنك إلغاء تنشيط الصنف." },
                    { "Item name format is incorrect. Expected format: 'code - name'", "تنسيق اسم الصنف غير صحيح. التنسيق المتوقع: 'الرمز - الاسم'." },
                    { "Please select a row first.", "يرجى تحديد صف أولاً." },
                    { "No data to print.", "لا توجد بيانات للطباعة." },
                    { "Please select a item first.", "يرجى تحديد صنف أولاً." },
                    { "Excel is not properly installed!", "برنامج Excel غير مثبت بشكل صحيح!" },
                    { "Exported Successfully!", "تم التصدير بنجاح!" },
                    { "Please add item first.", "يرجى إضافة صنف أولاً." }
                };
                if (translations.ContainsKey(message)) message = translations[message];
                else if (message.StartsWith("Export failed: ", StringComparison.OrdinalIgnoreCase)) message = "فشل التصدير: " + message.Substring("Export failed: ".Length);
            }
            MessageBox.Show(message);
        }

        public void BindItems()
        {
            DataTable dt;
            string query = @"Select id,Concat( code , ' - ',name) as 'Item Name',barcode as Barcode,
              Type from tbl_items where state = 0";
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (cmbCategory.Text != "" && !chkAllCategory.Checked)
            {
                query += " and category_id = @category_id";
                parameters.Add(ClinicDatabase.CreateParameterNew("category_id", cmbCategory.SelectedValue.ToString()));
            }
            if (cmbType.Text != "" && !chkAllType.Checked)
            {
                query += " and type = @type";
                parameters.Add(ClinicDatabase.CreateParameterNew("type", cmbType.Text));
            }
            if (cmbState.Text == "Active" || cmbState.Text == "نشط")
                query += " and active = 0";
            else if (cmbState.Text != "All" && cmbState.Text != "الكل")
                query += " and active != 0";
            dt = ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
            if (dt == null)
            {
                dgvCustomer.DataSource = null;
                return;
            }
            _dataView = dt.DefaultView;
            dgvCustomer.DataSource = dt;
            dgvCustomer.Columns["Item name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvCustomer.Columns["type"].Width = 140;
            dgvCustomer.Columns["id"].Visible = false;
            LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
            ApplyInventoryGridHeaders(dgvCustomer);

            if (dgvCustomer.Rows.Count > 0)
            {
                deleteCustomerToolStripMenuItem.Visible = recycleItemToolStripMenuItem.Visible = UserPermissions.canDelete("Inventory Items");
                editCustomerToolStripMenuItem1.Visible = UserPermissions.canEdit("Inventory Items");
            }
        }
        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            cmbCategory.Enabled = !chkAllCategory.Checked;
            cmbType.Enabled = !chkAllType.Checked;
            BindItems();
        }
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindItems();
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {

        }
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            frmMain.openChildForm(new frmViewItem(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void dgvCustomer_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
        }

        private void btnRecycle_Click(object sender, EventArgs e)
        {


        }
        private void RestoreForm_DataRestored(object sender, EventArgs e)
        {
            BindItems();
        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            BindItemDetails();
            BindItemTransaction();
        }
        private void BindItemDetails()
        {
            int itemId = int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString());
            string query = "SELECT * FROM tbl_items WHERE id=@itemId";
            DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("itemId", itemId));
            foreach (DataRow row in dt.Rows)
            {
                lblCode.Text = row["code"].ToString();
                lblName.Text = row["name"].ToString();
                lblPrice.Text = Convert.ToDecimal(row["cost_price"]).ToString("N2");
                lblSalePrice.Text = Convert.ToDecimal(row["sales_price"]).ToString("N2");
                lblOnHand.Text = Convert.ToDecimal(row["on_hand"]).ToString("N2");
            }
        }
        private void BindItemTransaction()
        {
            dgvDetails.Rows.Clear();
            int itemId = int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString());
            string query = "SELECT * FROM tbl_item_transaction WHERE item_id=@itemId";
            string type = dgvCustomer.SelectedRows[0].Cells["type"].Value.ToString();
            if (type == "12 - Service")
            {
                query = "SELECT * FROM tbl_item_transaction WHERE item_id=@itemId";
            }
            else if (type == "13 - Inventory Assembly")
            {
                query = "SELECT * FROM tbl_item_transaction WHERE item_id=@itemId";
            }
            decimal QtyBalance = 0;
            DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("itemId", itemId));
            foreach (DataRow row in dt.Rows)
            {
                string _id = row["id"].ToString();
                string _date = Convert.ToDateTime(row["date"]).ToString("dd-MM-yyyy");
                string _reference = row["reference"].ToString();
                string _type = row["type"].ToString();
                decimal _qtyIn = Convert.ToDecimal(row["qty_in"]);
                decimal _qtyOut = Convert.ToDecimal(row["qty_out"]);
                decimal _costPrice = Convert.ToDecimal(row["cost_price"]);
                decimal _salesPrice = Convert.ToDecimal(row["sales_price"]);
                decimal _qtyBalance = _qtyIn - _qtyOut;

                QtyBalance += _qtyBalance;
                dgvDetails.Rows.Add(
                    _id,
                    _date,
                    _reference,
                    _type,
                    _qtyIn.ToString("N2"),
                    _qtyOut.ToString("N2"),
                    _costPrice.ToString("N2"),
                    _salesPrice.ToString("N2"),
                    QtyBalance.ToString("N2")
                );
            }
            dgvDetails.Columns["t_qty_in"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns["t_qty_out"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns["t_cost_price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns["t_sales_price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns["t_qty_balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void nToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewItem());
        }

        private void editCustomerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewItem(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }

        private void deleteCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            int id = int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString());
            object result = ClinicDatabase.ExecuteScalar(@"SELECT COUNT(1) from tbl_item_transaction where type != 'Opening Qty' and item_id = @id", ClinicDatabase.CreateParameterNew("id", id));
            int recordCount = 0;
            if (result != null && result != DBNull.Value)
                recordCount = Convert.ToInt32(result);
            if (recordCount > 0)
            {
                ShowInventoryMessage("Item already used cannot delete! \n you can inactive the Item");
                return;
            }

            ClinicDatabase.ExecuteNonQuery("delete from tbl_items where id = @id; delete from tbl_item_transaction WHERE item_id = @id", ClinicDatabase.CreateParameterNew("id", id));
            BindItems();
        }

        private void recycleItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MasterRecycle restoreForm = new MasterRecycle("Item");
            restoreForm.DataRestored += RestoreForm_DataRestored;
            restoreForm.ShowDialog();
        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCard_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.SelectedRows.Count > 0)
            {
                var selectedRow = dgvCustomer.SelectedRows[0];

                string[] value = selectedRow.Cells["item name"].Value.ToString().Split(new string[] { " - " }, StringSplitOptions.None);
                if (value.Length >= 2)
                {
                    string code = value[0].Trim();
                    string name = value[1].Trim();
                    int itemId = int.Parse(selectedRow.Cells["id"].Value.ToString());

                    frmMain.openChildForm(new frmItemCard(code, name, itemId));
                }
                else
                {
                    ShowInventoryMessage("Item name format is incorrect. Expected format: 'code - name'");
                }
            }
            else
            {
                ShowInventoryMessage("Please select a row first.");
            }
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindItemDetails();
            BindItemTransaction();
        }

        private void pDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDetails.Rows.Count == 0)
            {
                ShowInventoryMessage("No data to print.");
                return;
            }

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 1000,
                Height = 800
            };

            previewDialog.ShowDialog();
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int leftMargin = 40;
            int topMargin = 40;
            int rowHeight = 25;
            int colSpacing = 5;

            Font font = new Font("Segoe UI", 9);
            Font headerFont = new Font("Segoe UI", 9, FontStyle.Bold);
            Brush brush = Brushes.Black;

            int y = topMargin;
            int x = leftMargin;

            // Print headers
            for (int i = 0; i < dgvDetails.Columns.Count; i++)
            {
                string header = dgvDetails.Columns[i].HeaderText;
                e.Graphics.DrawString(header, headerFont, brush, x, y);
                x += 100; // fixed width; you can customize per column
            }

            y += rowHeight;

            // Print rows
            int currentRow = 0;
            while (currentRow < dgvDetails.Rows.Count)
            {
                DataGridViewRow row = dgvDetails.Rows[currentRow];

                x = leftMargin;
                for (int i = 0; i < dgvDetails.Columns.Count; i++)
                {
                    string cellText = row.Cells[i].FormattedValue?.ToString() ?? "";
                    StringFormat format = new StringFormat();
                    decimal _;
                    // Right-align decimals
                    if (decimal.TryParse(cellText, out _))
                        format.Alignment = StringAlignment.Far;

                    RectangleF rect = new RectangleF(x, y, 100 - colSpacing, rowHeight);
                    e.Graphics.DrawString(cellText, font, brush, rect, format);
                    x += 100;
                }

                y += rowHeight;
                currentRow++;

                // New page if beyond margin
                if (y + rowHeight > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            currentRow = 0; // Reset after done
            e.HasMorePages = false;
        }
        string GetCompanyName()
        {
            return ClinicDatabase.ExecuteScalar("select name from tbl_company LIMIT 1")?.ToString() ?? "";
        }

        private void exportImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmOpeningQty());
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.CurrentRow == null)
            {
                ShowInventoryMessage("Please select a item first.");
                return;
            }

            try
            {
                string customerName = dgvCustomer.CurrentRow.Cells["Item Name"].Value.ToString();

                decimal balance = 0;
                if (!string.IsNullOrEmpty(lblOnHand.Text.ToString()))
                {
                    balance = decimal.Parse(lblOnHand.Text.ToString());
                }

                ExportToExcel(dgvDetails, customerName, balance);
            }
            catch (Exception ex)
            {
                ShowInventoryMessage("Export failed: " + ex.Message);
            }
        }
        private void ExportToExcel(DataGridView dgvDetails, string customerName, decimal customerBalance)
        {
            Excel.Application xlApp = new Excel.Application();
            if (xlApp == null)
            {
                ShowInventoryMessage("Excel is not properly installed!");
                return;
            }

            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            int row = 1;

            // Customer Info
            xlWorkSheet.Cells[row, 1] = IsArabicLanguage() ? "اسم المنتج:" : "Product Name:";
            xlWorkSheet.Cells[row, 2] = customerName;
            row++;
            xlWorkSheet.Cells[row, 1] = IsArabicLanguage() ? "الرصيد:" : "Balance:";
            xlWorkSheet.Cells[row, 2] = customerBalance.ToString("N2");
            row += 2;

            // Column Headers
            for (int i = 2; i < dgvDetails.Columns.Count; i++)
            {
                xlWorkSheet.Cells[row, i - 1] = dgvDetails.Columns[i].HeaderText;
            }
            row++;

            // Data Rows
            for (int i = 0; i < dgvDetails.Rows.Count; i++)
            {
                for (int j = 2; j < dgvDetails.Columns.Count; j++)
                {
                    var cellValue = dgvDetails.Rows[i].Cells[j].Value;
                    xlWorkSheet.Cells[row + i, j - 1] = cellValue?.ToString();
                }
            }

            xlWorkSheet.Columns.AutoFit();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = IsArabicLanguage() ? "ملفات Excel (*.xlsx)|*.xlsx" : "Excel files (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                xlWorkBook.SaveAs(saveFileDialog.FileName);
                ShowInventoryMessage("Exported Successfully!");
            }

            xlWorkBook.Close(false);
            xlApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }
        private void itemListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count <= 0)
            {
                ShowInventoryMessage("Please add item first.");
                return;
            }

            string query = @"SELECT i.code Code, i.name Name, i.barcode Barcode, 
                (SELECT price FROM tbl_item_card_details WHERE tbl_item_card_details.itemId = i.id ORDER BY id DESC LIMIT 1) AS `Cost Price`,
                (SELECT sales_price FROM tbl_item_transaction WHERE tbl_item_transaction.item_id = i.id ORDER BY id DESC LIMIT 1) AS `Sales Price`,
                i.on_hand Qty, i.type Type, IFNULL(u.name, '') AS Unit 
                FROM tbl_items i 
                LEFT JOIN tbl_unit u ON u.id = i.unit_id 
                WHERE i.id > 0";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (cmbCategory.Text != "" && !chkAllCategory.Checked)
            {
                query += " AND i.category_id = @category_id";
                parameters.Add(ClinicDatabase.CreateParameterNew("category_id", cmbCategory.SelectedValue.ToString()));
            }
            if (cmbType.Text != "" && !chkAllType.Checked)
            {
                query += " AND i.type = @type";
                parameters.Add(ClinicDatabase.CreateParameterNew("type", cmbType.Text));
            }
            if (cmbState.Text == "Active" || cmbState.Text == "نشط")
                query += " AND i.active = 0";
            else if (cmbState.Text != "All" && cmbState.Text != "الكل")
                query += " AND i.active != 0";

            DataTable dt = ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());

            // Export to Excel
            Excel.Application xlApp = new Excel.Application();
            if (xlApp == null)
            {
                ShowInventoryMessage("Excel is not properly installed!");
                return;
            }

            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            // Write column headers
            for (int col = 0; col < dt.Columns.Count; col++)
            {
                xlWorkSheet.Cells[1, col + 1] = dt.Columns[col].ColumnName;
            }

            // Write data rows
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    xlWorkSheet.Cells[row + 2, col + 1] = dt.Rows[row][col].ToString();
                }
            }

            // Optional: Autofit columns
            xlWorkSheet.Columns.AutoFit();

            // Show Excel
            xlApp.Visible = true;
        }

        private void customerListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //
        }

        private void dgvDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvDetails.Rows.Count)
            {
                if (dgvDetails.Columns.Contains("t_type"))
                {
                    if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Purchase Invoice")
                        frmMain.openChildForm(new frmPurchase(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Purchase Return Invoice")
                    //    frmMain.openChildForm(new frmPurchaseReturn(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Opening Qty")
                        frmMain.openChildForm(new frmViewItem(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Debit Note")
                    //    frmMain.openChildForm(new frmCreditNote(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Sales Invoice")
                    //    frmMain.openChildForm(new frmSales(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Sales Return Invoice")
                    //    frmMain.openChildForm(new frmSalesReturn(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Credit Note")
                    //    frmMain.openChildForm(new frmCreditNote(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Stock Settlement")
                    //    frmMain.openChildForm(new frmCreditNote(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString() == "Warehouse Transfer")
                    //    frmMain.openChildForm(new frmCreditNote(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString()) == "Project Tender")
                    //    frmMain.openChildForm(new frmCreditNote(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                    //else if (dgvDetails.Rows[e.RowIndex].Cells["t_type"].Value.ToString()) == "Manufacturing")
                    //    frmMain.openChildForm(new frmCreditNote(int.Parse(dgvDetails.Rows[e.RowIndex].Cells["Inv_no"].Value.ToString())));
                }
            }
        }
    }
}
