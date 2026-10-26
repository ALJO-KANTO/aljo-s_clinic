
using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterItemUnit : Form
    {
        private EventHandler itemUnitUpdatedHandler;

        // Shared language property
        private bool isArabic => TranslationClass.IsArabic;

        public MasterItemUnit()
        {
            InitializeComponent();

            LocalizationManager.LocalizeForm(this);

            ApplyUnitTranslations();

            itemUnitUpdatedHandler =
                (sender, args) => BindItems();
        }

        // ============================================
        // ARABIC / ENGLISH TRANSLATION
        // ============================================
        private void ApplyUnitTranslations()
        {
            this.Text = isArabic
                ? "فئات وحدات القياس"
                : "Unit Categories";

            // Custom heading
            headerUC1.FormText = this.Text;

            // Buttons
            btnNew.Text = isArabic
                ? "إضافة جديد"
                : "New";

            btnEdit.Text = isArabic
                ? "تعديل"
                : "Edit";

            btnDelete.Text = isArabic
                ? "حذف"
                : "Delete";

            // Keep existing layout and design
            this.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            this.RightToLeftLayout = false;

            dgvCustomer.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;
        }

        // ============================================
        // FORM EVENTS
        // ============================================
        private void MasterItemUnit_Load(
            object sender, EventArgs e)
        {
            EventHub.ItemUnit += itemUnitUpdatedHandler;

            ApplyUnitTranslations();

            BindItems();
        }

        private void MasterItemUnit_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            EventHub.ItemUnit -= itemUnitUpdatedHandler;
        }

        // ============================================
        // LOAD UNIT CATEGORIES
        // ============================================
        public void BindItems()
        {
            dgvCustomer.DataSource =
                ClinicDatabase.ExecuteDataTable(@"
                    SELECT
                        id,
                        name
                    FROM tbl_uom_category
                    WHERE state = 0
                    ORDER BY name;
                ");

            if (dgvCustomer.Columns.Contains("name"))
            {
                DataGridViewColumn column =
                    dgvCustomer.Columns["name"];

                column.HeaderText = isArabic
                    ? "اسم فئة الوحدة"
                    : "Unit Category Name";

                column.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                column.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }

            if (dgvCustomer.Columns.Contains("id"))
            {
                dgvCustomer.Columns["id"].Visible = false;
            }

            dgvCustomer.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;
        }

        // ============================================
        // OLD UNIT BINDING
        // ============================================
        public void BindItemsOld()
        {
            dgvCustomer.DataSource =
                ClinicDatabase.ExecuteDataTable(
                    @"SELECT id, name FROM tbl_unit");

            if (dgvCustomer.Columns.Contains("name"))
            {
                DataGridViewColumn column =
                    dgvCustomer.Columns["name"];

                column.HeaderText = isArabic
                    ? "اسم الوحدة"
                    : "Unit Name";

                column.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                column.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }

            if (dgvCustomer.Columns.Contains("id"))
                dgvCustomer.Columns["id"].Visible = false;
        }

        // ============================================
        // NEW CATEGORY
        // ============================================
        private void btnNew_Click(
            object sender, EventArgs e)
        {
            using (frmViewUomCategory frm =
                new frmViewUomCategory())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    BindItems();
                }
            }
        }

        // ============================================
        // EDIT CATEGORY
        // ============================================
        private void btnEdit_Click(
            object sender, EventArgs e)
        {
            if (dgvCustomer.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    isArabic
                        ? "يرجى تحديد فئة وحدة أولاً"
                        : "Please select a unit category first.");
                return;
            }

            int categoryId = Convert.ToInt32(
                dgvCustomer.SelectedRows[0]
                    .Cells["id"].Value);

            using (frmViewUomCategory frm =
                new frmViewUomCategory(categoryId))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    BindItems();
                }
            }
        }

        // ============================================
        // DOUBLE CLICK TO EDIT
        // ============================================
        private void dgvCustomer_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int categoryId = Convert.ToInt32(
                dgvCustomer.Rows[e.RowIndex]
                    .Cells["id"].Value);

            using (frmViewUomCategory frm =
                new frmViewUomCategory(categoryId))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    BindItems();
                }
            }
        }

        // ============================================
        // DELETE CATEGORY
        // ============================================
        private void btnDelete_Click(
            object sender, EventArgs e)
        {
            if (dgvCustomer.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    isArabic
                        ? "يرجى تحديد فئة وحدة أولاً"
                        : "Please select a unit category first.");
                return;
            }

            int categoryId = Convert.ToInt32(
                dgvCustomer.SelectedRows[0]
                    .Cells["id"].Value);

            // Check whether category has units
            object result = ClinicDatabase.ExecuteScalar(
                @"SELECT COUNT(1)
                  FROM tbl_uom
                  WHERE category_id = @id",
                ClinicDatabase.CreateParameter(
                    "id", categoryId));

            int recordCount =
                result == null || result == DBNull.Value
                    ? 0
                    : Convert.ToInt32(result);

            if (recordCount > 0)
            {
                MessageBox.Show(
                    isArabic
                        ? "لا يمكن حذف فئة الوحدة لأنها تحتوي على وحدات مستخدمة أو مسجلة"
                        : "Cannot delete this category because it contains units.",
                    isArabic ? "تنبيه" : "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult confirmation = MessageBox.Show(
                isArabic
                    ? "هل أنت متأكد من حذف فئة الوحدة المحددة؟"
                    : "Are you sure you want to delete the selected unit category?",
                isArabic ? "تأكيد الحذف" : "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmation != DialogResult.Yes)
                return;

            ClinicDatabase.ExecuteNonQuery(
                @"DELETE FROM tbl_uom_category
                  WHERE id = @id",
                ClinicDatabase.CreateParameter(
                    "id", categoryId));

            BindItems();

            MessageBox.Show(
                isArabic
                    ? "تم الحذف بنجاح"
                    : "Deleted successfully.",
                isArabic ? "نجاح" : "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ============================================
        // FILTER EVENTS
        // ============================================
        private void chkAll_CheckedChanged(
            object sender, EventArgs e)
        {
        }

        private void cmbCategory_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            BindItems();
        }

        // ============================================
        // CURSOR EVENTS
        // ============================================
        private void dgvCustomer_MouseHover(
            object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void dgvCustomer_Leave(
            object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        // ============================================
        // WINDOW BUTTONS
        // ============================================
        private void guna2Button4_Click(
            object sender, EventArgs e)
        {
            if (this.WindowState ==
                FormWindowState.Maximized)
            {
                this.WindowState =
                    FormWindowState.Normal;
            }
            else
            {
                this.WindowState =
                    FormWindowState.Maximized;
            }
        }

        private void guna2Button1_Click(
            object sender, EventArgs e)
        {
            this.WindowState =
                FormWindowState.Minimized;
        }

        private void panel1_Paint(
            object sender, PaintEventArgs e)
        {
        }

        private void LbHeader_Click(
            object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
