using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using YamyProject.Localization;
using Clinic;
namespace YamyProject
{
    public partial class MasterItemCategory : Form
    {
        private EventHandler itemCategoryUpdatedHandler;
        private bool isArabic => TranslationClass.IsArabic;
        public MasterItemCategory()
        {
            InitializeComponent();

            LocalizationManager.LocalizeForm(this);

            ApplyCategoryTranslations();

            itemCategoryUpdatedHandler = (sender, args) => BindItems();
            EventHub.ItemCategory += itemCategoryUpdatedHandler;
        }
        private void ApplyCategoryTranslations()
        {
            this.Text = isArabic
                ? "فئات الأصناف"
                : "Item Categories";

            // Translate custom heading
            headerUC1.FormText = this.Text;

            btnNew.Text = isArabic ? "إضافة جديد" : "New";
            btnEdit.Text = isArabic ? "تعديل" : "Edit";
            btnDelete.Text = isArabic ? "حذف" : "Delete";

            this.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            this.RightToLeftLayout = false;

            dgvCustomer.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;
        }
        private void MasterItemCategory_FormClosing(object sender, FormClosingEventArgs e)
        {
            EventHub.ItemCategory -= itemCategoryUpdatedHandler;
            //EventHub.SalesInv -= invoiceUpdatedHandler;
            //EventHub.PurchaseInv -= purchaseUpdatedHandler;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            new frmViewCategory().ShowDialog();
        }
        private void MasterItemCategory_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvCustomer);
            BindItems();
        }
        public void BindItems()
        {
            dgvCustomer.DataSource = ClinicDatabase.ExecuteDataTable(
                @"SELECT
              id,
              CONCAT(code, ' - ', name) AS `Category Name`
          FROM tbl_item_category");

            if (dgvCustomer.Columns.Contains("Category Name"))
            {
                var column = dgvCustomer.Columns["Category Name"];

                column.HeaderText = isArabic
                    ? "اسم الفئة"
                    : "Category Name";

                column.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                column.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }

            if (dgvCustomer.Columns.Contains("id"))
                dgvCustomer.Columns["id"].Visible = false;
        }
        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            //cmbCategory.Enabled = !chkAllCategory.Checked;
            //cmbType.Enabled = !chkAllType.Checked;
            //BindItems();
        }
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindItems();
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            new frmViewCategory( int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())).ShowDialog();
        }
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
          
                new frmViewCategory(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())).ShowDialog();
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
            if (dgvCustomer.SelectedRows.Count == 0)
            {
                MessageBox.Show(isArabic
                    ? "يرجى تحديد فئة أولاً"
                    : "Please select a category first.");
                return;
            }

            int categoryId = Convert.ToInt32(
                dgvCustomer.SelectedRows[0].Cells["id"].Value);

            object result = ClinicDatabase.ExecuteScalar(
                @"SELECT COUNT(1)
          FROM tbl_items
          WHERE category_id = @id",
                ClinicDatabase.CreateParameterNew("id", categoryId));

            int recordCount = result == null || result == DBNull.Value
                ? 0
                : Convert.ToInt32(result);

            if (recordCount > 0)
            {
                MessageBox.Show(isArabic
                    ? "الفئة مستخدمة بالفعل"
                    : "Already used");
                return;
            }

            ClinicDatabase.ExecuteNonQuery(
                "DELETE FROM tbl_item_category WHERE id = @id",
                ClinicDatabase.CreateParameterNew("id", categoryId));

            BindItems();

            MessageBox.Show(isArabic
                ? "تم الحذف بنجاح"
                : "Deleted");
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

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
    }
}
