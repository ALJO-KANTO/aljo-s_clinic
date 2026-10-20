using Clinic;
using Clinic_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
//using Clinic.Localization;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterFixedAssets : Form
    {
        EventHandler fixedAsset;
        public MasterFixedAssets()
        {
            InitializeComponent();

            LocalizationManager.LocalizeForm(this);
            headerUC1.FormText = this.Text;
            fixedAsset = (sender, args) => BindFixedAseet();

        }
       
        private void MasterFixedAssets_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.FixedAsset -= fixedAsset;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewFixedAssets());
        }
        private void MasterFixedAssets_Load(object sender, EventArgs e)
        {
            dgvMain.Columns.Add("SN", "SN");
            dgvMain.Columns.Add("DATE", "DATE");
            dgvMain.Columns.Add("DAYS", "DAYS");
            dgvMain.Columns.Add("DESCRIPTION", "DESCRIPTION");
            dgvMain.Columns.Add("AMOUNT", "AMOUNT");
            dgvMain.Columns.Add("NOTE", "NOTE");
            dgvMain.Columns["note"].Width = 160;
            LocalizationManager.LocalizeDataGridViewHeaders(dgvMain);

            BindFixedAseet();

            dgvCustomer.Columns["code"].Width = dgvCustomer.Columns["sn"].Width = 40;
            dgvCustomer.Columns["from"].Width = dgvCustomer.Columns["to"].Width = 60;
            dgvMain.Columns["description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
            LocalizationManager.LocalizeDataGridViewHeaders(dgvMain);
            string language =
    Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic = language.StartsWith(
                "ar",
                StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
        }
        private void ApplyLanguageLayout(bool isArabic)
        {

            SuspendLayout();

            try
            {
                if (isArabic)
                {
                    Text = "مركز الأصول الثابتة";
                    headerUC1.FormText = "مركز الأصول الثابتة";
                    headerUC1.ArabicLayout = true;

                    // Main grid headers
                    dgvMain.Columns["SN"].HeaderText = "الرقم التسلسلي";
                    dgvMain.Columns["DATE"].HeaderText = "التاريخ";
                    dgvMain.Columns["DAYS"].HeaderText = "الأيام";
                    dgvMain.Columns["DESCRIPTION"].HeaderText = "الوصف";
                    dgvMain.Columns["AMOUNT"].HeaderText = "المبلغ";
                    dgvMain.Columns["NOTE"].HeaderText = "ملاحظات";

                    // Fixed assets grid headers
                    if (dgvCustomer.Columns.Contains("sn"))
                        dgvCustomer.Columns["sn"].HeaderText = "الرقم التسلسلي";
                    if (dgvCustomer.Columns.Contains("code"))
                        dgvCustomer.Columns["code"].HeaderText = "الرمز";
                    if (dgvCustomer.Columns.Contains("name"))
                        dgvCustomer.Columns["name"].HeaderText = "الاسم";
                    if (dgvCustomer.Columns.Contains("from"))
                        dgvCustomer.Columns["from"].HeaderText = "من";
                    if (dgvCustomer.Columns.Contains("to"))
                        dgvCustomer.Columns["to"].HeaderText = "إلى";
                    if (dgvCustomer.Columns.Contains("total"))
                        dgvCustomer.Columns["total"].HeaderText = "الإجمالي";

                    dgvMain.RightToLeft = RightToLeft.Yes;
                    dgvCustomer.RightToLeft = RightToLeft.Yes;

                    dgvMain.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    // Translate the ToolStrip menu and its dropdown items.
                    TranslateToolStripItems(this, true);
                }
                else
                {
                    Text = "Fixed Assets Center";
                    headerUC1.FormText = "Fixed Assets Center";
                    headerUC1.ArabicLayout = false;

                    dgvMain.Columns["SN"].HeaderText = "SN";
                    dgvMain.Columns["DATE"].HeaderText = "DATE";
                    dgvMain.Columns["DAYS"].HeaderText = "DAYS";
                    dgvMain.Columns["DESCRIPTION"].HeaderText = "DESCRIPTION";
                    dgvMain.Columns["AMOUNT"].HeaderText = "AMOUNT";
                    dgvMain.Columns["NOTE"].HeaderText = "NOTE";

                    if (dgvCustomer.Columns.Contains("sn"))
                        dgvCustomer.Columns["sn"].HeaderText = "SN";
                    if (dgvCustomer.Columns.Contains("code"))
                        dgvCustomer.Columns["code"].HeaderText = "Code";
                    if (dgvCustomer.Columns.Contains("name"))
                        dgvCustomer.Columns["name"].HeaderText = "Name";
                    if (dgvCustomer.Columns.Contains("from"))
                        dgvCustomer.Columns["from"].HeaderText = "From";
                    if (dgvCustomer.Columns.Contains("to"))
                        dgvCustomer.Columns["to"].HeaderText = "To";
                    if (dgvCustomer.Columns.Contains("total"))
                        dgvCustomer.Columns["total"].HeaderText = "Total";

                    dgvMain.RightToLeft = RightToLeft.No;
                    dgvCustomer.RightToLeft = RightToLeft.No;

                    dgvMain.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    // Restore the English ToolStrip menu and its dropdown items.
                    TranslateToolStripItems(this, false);
                }
            }
            finally
            {
                ResumeLayout(true);
            }

        }
        private void TranslateToolStripItems(Control parent, bool isArabic)
        {
            HashSet<ToolStripItem> processedItems = new HashSet<ToolStripItem>();

            // Translate ToolStrip controls and context menus attached to controls.
            TranslateToolStripControls(parent, isArabic, processedItems);

            // Some menu components are owned by the form but are not in Controls.
            FieldInfo[] fields = GetType().GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(this);

                if (value is ToolStrip toolStrip)
                {
                    TranslateToolStripItemCollection(
                        toolStrip.Items, isArabic, processedItems);
                }
                else if (value is ContextMenuStrip contextMenu)
                {
                    TranslateToolStripItemCollection(
                        contextMenu.Items, isArabic, processedItems);
                }
                else if (value is ToolStripItem toolStripItem)
                {
                    TranslateOneToolStripItem(
                        toolStripItem, isArabic, processedItems);
                }
            }

            // Explicitly translate the known menu items used by this form.
            if (nToolStripMenuItem != null)
                nToolStripMenuItem.Text = isArabic ? "أصول ثابتة جديدة" : "New Fixed Assets";

            if (newCategoryToolStripMenuItem != null)
                newCategoryToolStripMenuItem.Text = isArabic
                    ? "فئة أصول ثابتة جديدة"
                    : "New Fixed Assets Category";

            if (editCustomerToolStripMenuItem1 != null)
                editCustomerToolStripMenuItem1.Text = isArabic
                    ? "تعديل الأصول الثابتة"
                    : "Edit Fixed Assets";

            if (deleteCustomerToolStripMenuItem != null)
                deleteCustomerToolStripMenuItem.Text = isArabic
                    ? "حذف الأصول الثابتة"
                    : "Delete Fixed Assets";
        }

        private void TranslateToolStripControls(
            Control parent,
            bool isArabic,
            HashSet<ToolStripItem> processedItems)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (control is ToolStrip toolStrip)
                {
                    TranslateToolStripItemCollection(
                        toolStrip.Items, isArabic, processedItems);
                }

                if (control.ContextMenuStrip != null)
                {
                    TranslateToolStripItemCollection(
                        control.ContextMenuStrip.Items, isArabic, processedItems);
                }

                TranslateToolStripControls(control, isArabic, processedItems);
            }

            if (parent.ContextMenuStrip != null)
            {
                TranslateToolStripItemCollection(
                    parent.ContextMenuStrip.Items, isArabic, processedItems);
            }
        }

        private void TranslateToolStripItemCollection(
            ToolStripItemCollection items,
            bool isArabic,
            HashSet<ToolStripItem> processedItems)
        {
            foreach (ToolStripItem item in items)
                TranslateOneToolStripItem(item, isArabic, processedItems);
        }

        private void TranslateOneToolStripItem(
            ToolStripItem item,
            bool isArabic,
            HashSet<ToolStripItem> processedItems)
        {
            if (item == null || !processedItems.Add(item))
                return;

            string text = (item.Text ?? string.Empty).Trim();
            string normalizedText = text.Replace("&", "").Replace("...", "").Trim();

            if (normalizedText.Equals("Fixed Assets Operations   ▼", StringComparison.OrdinalIgnoreCase) ||
                normalizedText.Equals("Fixed Assets Operations   ▼", StringComparison.OrdinalIgnoreCase) ||
                normalizedText == "▼ عمليات الأصول الثابتة  ")
            {
                item.Text = isArabic ? "عمليات الأصول الثابتة" : "Fixed Assets Operations";
            }
            else if (normalizedText.Equals("New Fixed Assets", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "أصول ثابتة جديدة")
            {
                item.Text = isArabic ? "أصول ثابتة جديدة" : "New Fixed Assets";
            }
            else if (normalizedText.Equals("New Fixed Assets Category", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "فئة أصول ثابتة جديدة")
            {
                item.Text = isArabic ? "فئة أصول ثابتة جديدة" : "New Fixed Assets Category";
            }
            else if (normalizedText.Equals("Edit Fixed Assets", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "تعديل الأصول الثابتة")
            {
                item.Text = isArabic ? "تعديل الأصول الثابتة" : "Edit Fixed Assets";
            }
            else if (normalizedText.Equals("Delete Fixed Assets", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "حذف الأصول الثابتة")
            {
                item.Text = isArabic ? "حذف الأصول الثابتة" : "Delete Fixed Assets";
            }
            else if (normalizedText.Equals("New", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "جديد")
            {
                item.Text = isArabic ? "جديد" : "New";
            }
            else if (normalizedText.Equals("Edit", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "تعديل")
            {
                item.Text = isArabic ? "تعديل" : "Edit";
            }
            else if (normalizedText.Equals("Delete", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "حذف")
            {
                item.Text = isArabic ? "حذف" : "Delete";
            }
            else if (normalizedText.Equals("New Category", StringComparison.OrdinalIgnoreCase) ||
                     normalizedText == "فئة جديدة")
            {
                item.Text = isArabic ? "فئة جديدة" : "New Category";
            }

            if (item is ToolStripDropDownItem dropDownItem &&
                dropDownItem.DropDownItems.Count > 0)
            {
                TranslateToolStripItemCollection(
                    dropDownItem.DropDownItems, isArabic, processedItems);
            }
        }

        public void BindFixedAseet()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
            ROW_NUMBER() OVER (ORDER BY tbl_fixed_assets.id) AS `SN`, tbl_fixed_assets.id,
            tbl_fixed_assets.code AS 'Code', tbl_fixed_assets.name as 'Name', purchase_date as 'From',end_date as 'To',
            tbl_fixed_assets.purchase_price as Total from tbl_fixed_assets where tbl_fixed_assets.state = 0");
            dgvCustomer.DataSource = dt;
            LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
            dgvCustomer.Columns["id"].Visible = false;

            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            bool isArabic = language.StartsWith(
                "ar", StringComparison.OrdinalIgnoreCase);
            ApplyLanguageLayout(isArabic);
            if (dgvCustomer.Rows.Count > 0)
            {
                editCustomerToolStripMenuItem1.Visible = UserPermissions.canEdit("Fixed Assets");
                deleteCustomerToolStripMenuItem.Visible = UserPermissions.canDelete("Fixed Assets");
                bindFixedAseetData();
            }
        }

        //private void bindFixedAseetData()
        //{
        //    DateTime startDate = DateTime.Parse(dgvCustomer.SelectedRows[0].Cells["from"].Value.ToString());
        //    DateTime endDate = DateTime.Parse(dgvCustomer.SelectedRows[0].Cells["to"].Value.ToString());
        //    decimal totalAmount = decimal.Parse(dgvCustomer.SelectedRows[0].Cells["total"].Value.ToString());
        //    decimal actualTotal = 0;
        //    int totalDays = (endDate - startDate).Days + 1;

        //    dgvMain.Rows.Clear();

        //    DateTime currentDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month)); // Last day of first month
        //    int serialNumber = 1;

        //    int daysInFirstMonth = (currentDate - startDate).Days + 1;
        //    decimal firstMonthAmount = Math.Round((totalAmount / totalDays) * daysInFirstMonth, 4);
        //    actualTotal += firstMonthAmount;
        //    dgvMain.Rows.Add(serialNumber,
        //                     currentDate.ToString("MM-dd-yyyy"),
        //                     daysInFirstMonth,
        //                     dgvCustomer.SelectedRows[0].Cells["name"].Value.ToString(),
        //                     firstMonthAmount.ToString(),
        //                     "Fixed Asset REGISTER");
        //    serialNumber++;

        //    // Move to the last day of the next month
        //    currentDate = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1);
        //    currentDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));

        //    while (currentDate <= endDate)
        //    {
        //        int daysInMonth = (currentDate - new DateTime(currentDate.Year, currentDate.Month, 1)).Days + 1;

        //        if (currentDate > endDate)
        //            daysInMonth = (endDate - new DateTime(endDate.Year, endDate.Month, 1)).Days + 1;

        //        decimal monthlyAmount = Math.Round((totalAmount / totalDays) * daysInMonth, 4);
        //        actualTotal += monthlyAmount;

        //        dgvMain.Rows.Add(serialNumber,
        //                         currentDate.ToString("MM-dd-yyyy"),
        //                         daysInMonth,
        //                         dgvCustomer.SelectedRows[0].Cells["name"].Value.ToString(),
        //                         monthlyAmount.ToString(),
        //                         "Fixed Asset REGISTER");

        //        serialNumber++;

        //        // Move to the last day of the next month
        //        currentDate = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1);
        //        currentDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
        //    }

        //    dgvMain.Rows.Add(serialNumber, "", totalDays, "", actualTotal, "");
        //}
        private void bindFixedAseetData()
        {
            DateTime startDate = DateTime.Parse(dgvCustomer.SelectedRows[0].Cells["from"].Value.ToString());
            DateTime endDate = DateTime.Parse(dgvCustomer.SelectedRows[0].Cells["to"].Value.ToString());
            decimal totalAmount = decimal.Parse(dgvCustomer.SelectedRows[0].Cells["total"].Value.ToString());
            decimal actualTotal = 0;
            int totalDays = (endDate - startDate).Days + 1;

            dgvMain.Rows.Clear();

            DateTime currentMonthStart = startDate;
            int serialNumber = 1;

            while (currentMonthStart <= endDate)
            {
                DateTime currentMonthEnd = new DateTime(currentMonthStart.Year, currentMonthStart.Month, DateTime.DaysInMonth(currentMonthStart.Year, currentMonthStart.Month));

                if (currentMonthEnd > endDate)
                    currentMonthEnd = endDate;

                DateTime periodStart = currentMonthStart < startDate ? startDate : currentMonthStart;
                DateTime periodEnd = currentMonthEnd > endDate ? endDate : currentMonthEnd;

                int daysInPeriod = (periodEnd - periodStart).Days + 1;
                decimal amount = Math.Round((totalAmount / totalDays) * daysInPeriod, 2);

                if (periodEnd == endDate)
                {
                    amount = totalAmount - actualTotal;
                }

                actualTotal += amount;

                dgvMain.Rows.Add(serialNumber,
                                 periodEnd.ToString("MM-dd-yyyy"),
                                 daysInPeriod,
                                 dgvCustomer.SelectedRows[0].Cells["name"].Value.ToString(),
                                 amount.ToString("N2"),
                                 (Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en")
                                     .StartsWith("ar", StringComparison.OrdinalIgnoreCase)
                                     ? "سجل الأصول الثابتة"
                                     : "Fixed Asset REGISTER");

                serialNumber++;
                currentMonthStart = currentMonthStart.AddMonths(1);
                currentMonthStart = new DateTime(currentMonthStart.Year, currentMonthStart.Month, 1);
            }

            dgvMain.Rows.Add(serialNumber, "", totalDays, "", actualTotal.ToString("N2"), "");
        }


        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFixedAseet();
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewFixedAssets(int.Parse(dgvCustomer.CurrentRow.Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewFixedAssets(int.Parse(dgvCustomer.CurrentRow.Cells["id"].Value.ToString())));
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
            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_fixed_assets SET state = -1 WHERE id = @id ",
                                          ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            ////Utilities.LogAudit(-1, "Delete Fixed Asset", "Fixed Asset", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()), "Deleted Fixed Asset: " + dgvCustomer.SelectedRows[0].Cells["name"].Value.ToString());
            BindFixedAseet();
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            //_mainForm.openChildForm(new MasterInventoryRecycle(_mainForm, this));

        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bindFixedAseetData();

        }

        private void lnkCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmViewFixedAssetsCategory().Show();
        }

        private void nToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewFixedAssets());
        }

        private void editCustomerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewFixedAssets(int.Parse(dgvCustomer.CurrentRow.Cells["id"].Value.ToString())));
        }

        private void newCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmViewFixedAssetsCategory().Show();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
