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

        public MasterItemUnit()
        {
            InitializeComponent();
            headerUC1.FormText = this.Text;
            LocalizationManager.LocalizeForm(this);
            itemUnitUpdatedHandler = (sender, args) => BindItems();
        }

        private void MasterItemUnit_FormClosing(object sender, FormClosingEventArgs e)
        {
            EventHub.ItemUnit -= itemUnitUpdatedHandler;
            //EventHub.SalesInv -= invoiceUpdatedHandler;
            //EventHub.PurchaseInv -= purchaseUpdatedHandler;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (frmViewUomCategory frm = new frmViewUomCategory())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    BindItems();
                }
            }
        }
        private void MasterItemUnit_Load(object sender, EventArgs e)
        {
            EventHub.ItemUnit += itemUnitUpdatedHandler;
            BindItems();
        }
        public void BindItems()
        {
            dgvCustomer.DataSource = ClinicDatabase.ExecuteDataTable(@"
                SELECT 
                    id,
                    name
                FROM tbl_uom_category
                WHERE state = 0
                ORDER BY name;
            ");

            dgvCustomer.Columns["name"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvCustomer.Columns["name"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;

            dgvCustomer.Columns["id"].Visible = false;

            LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
        }
        public void BindItemsOld()
        {
            dgvCustomer.DataSource = ClinicDatabase.ExecuteDataTable(@"select id,name from tbl_unit");
            dgvCustomer.Columns["name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomer.Columns["name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvCustomer.Columns["id"].Visible = false;
            LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
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
            if (dgvCustomer.SelectedRows.Count == 0)
                return;

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
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgvCustomer.Rows.Count == 0)
            //    return;

            //    new frmViewUomCategory(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())).ShowDialog();
            //if (dgvCustomer.Rows.Count == 0)
            //    return;
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
            object result = ClinicDatabase.ExecuteScalar(@"SELECT COUNT(1) FROM tbl_items 
                  WHERE unit_id = @id", ClinicDatabase.CreateParameter("id", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
            int recordCount = 0;
            if (result != null && result != DBNull.Value)
                recordCount = Convert.ToInt32(result);
            if (recordCount > 0)
            {
                MessageBox.Show("Already used");
                return;
            }
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_uom_category WHERE id=@id", ClinicDatabase.CreateParameter("id", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
            BindItems();
            MessageBox.Show("Deleted");
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
