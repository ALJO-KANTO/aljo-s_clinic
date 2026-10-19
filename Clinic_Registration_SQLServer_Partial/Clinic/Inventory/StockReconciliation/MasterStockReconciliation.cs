using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;
using YamyProject.UI.Inventory.StockReconciliation;

namespace YamyProject
{
    public partial class MasterStockReconciliation : Form
    {
        private EventHandler vendorUpdatedHandler;
        private EventHandler invoiceUpdatedHandler;

        int _currentPage = 1;
        int _pageSize = 50; // Or any default rows per page
        int _totalRecords = 0;
        int _totalPages = 0;

        public MasterStockReconciliation()
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            vendorUpdatedHandler = (sender, args) => BindCombos.PopulateEmployees(cmbCustomer);
            invoiceUpdatedHandler = (sender, args) => BindInvoices();
            //.Vendor += vendorUpdatedHandler;
            //.StockReconciliation += invoiceUpdatedHandler;
            headerUC1.FormText = this.Text;
        }
        private void MasterStockReconciliation_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.Vendor -= vendorUpdatedHandler;
            //.StockReconciliation -= invoiceUpdatedHandler;

        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmStockReconciliation());
        }
        private void MasterStockReconciliation_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvCustomer);
            //AppTheme.DefaultFont(dgvItems);
            BindCombos.PopulateVendors(cmbCustomer);
            BindInvoices();
        }
        public void BindInvoices()
        {
            string query = "";
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (cmbSelectionMethod.Text == "Default")
            {
                query = @"SELECT ROW_NUMBER() OVER (ORDER BY sr.reconciliation_date) AS `SN`,sr.reconciliation_id id,sr.reconciliation_no `No`,sr.reconciliation_type `Type`,p.name Project,w.name `Warehouse`,sr.Status
                        FROM tbl_stock_reconciliation sr INNER JOIN tbl_projects p ON p.id = project_id INNER JOIN tbl_warehouse w ON w.id = sr.warehouse_id";
            }
            else
            {
                query = @"Select ROW_NUMBER() OVER (ORDER BY sr.reconciliation_date) AS `SN`,sr.reconciliation_id id,sr.reconciliation_no `No`,p.name Project,i.name `Item Name`,sri.system_qty `System Qty`,sri.physical_qty `Physical Qty`,sri.variance_qty `Variance Qty`,uom `UOM`,unit_cost `Cost`,variance_value `Value`,sri.remarks
                        FROM tbl_stock_reconciliation sr INNER JOIN tbl_projects p ON p.id = project_id
                        INNER JOIN tbl_stock_reconciliation_item sri ON sri.item_id = sr.reconciliation_id 
                        INNER JOIN tbl_items i ON i.id = sri.item_id";
            }


            if (!chkDate.Checked)
            {
                query += " AND sr.reconciliation_date >= @dateFrom AND sr.reconciliation_date <= @dateTo";

                parameters.Add(ClinicDatabase.CreateParameterNew("@dateFrom", dtFrom.Value.Date));
                parameters.Add(ClinicDatabase.CreateParameterNew("@dateTo", dtTo.Value.Date));
            }
            if (cmbCustomer.SelectedValue != null && !chkCustomer.Checked)
            {
                query += " AND sr.checked_by = @checked_by";
                parameters.Add(ClinicDatabase.CreateParameterNew("@checked_by", cmbCustomer.SelectedValue));
            }

            int offset = (_currentPage - 1) * _pageSize;

            query += " LIMIT @_limit OFFSET @_offset";

            parameters.Add(ClinicDatabase.CreateParameterNew("@_limit", _pageSize));
            parameters.Add(ClinicDatabase.CreateParameterNew("@_offset", offset));


            DataTable dt = ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());


            string countQuery = @"
                                SELECT COUNT(*) 
                                FROM tbl_stock_reconciliation sr
                                WHERE sr.reconciliation_id > 0
                            ";

            List<MySqlParameter> countParams = new List<MySqlParameter>();

            if (!chkDate.Checked)
            {
                countQuery += " AND sr.reconciliation_date >= @dateFrom AND sr.reconciliation_date <= @dateTo";

                countParams.Add(ClinicDatabase.CreateParameterNew("@dateFrom", dtFrom.Value.Date));
                countParams.Add(ClinicDatabase.CreateParameterNew("@dateTo", dtTo.Value.Date));
            }

            _totalRecords = Convert.ToInt32(ClinicDatabase.ExecuteScalar(countQuery, countParams.ToArray()));
            _totalPages = (_totalRecords + _pageSize - 1) / _pageSize;


            if (dt != null)
            {
                dgvCustomer.DataSource = dt;

                if (dgvCustomer.Columns.Contains("id"))
                    dgvCustomer.Columns["id"].Visible = false;

                if (dgvCustomer.Columns.Contains("No"))
                    dgvCustomer.Columns["No"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // Load items if header mode
                if (dgvCustomer.Rows.Count > 0 && cmbSelectionMethod.Text == "Default")
                    bindInvoiceItems();
                else
                    dgvItems.DataSource = null;

                if (dgvCustomer.Rows.Count > 0)
                {
                    btnEdit.Enabled = UserPermissions.canEdit("Stock Reconciliation");
                    btnDelete.Enabled = btnRecycle.Enabled = UserPermissions.canDelete("Stock Reconciliation");
                }

               // LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
                AddViewDocImageColumn();
            }

            lblPageInfo.Text = $"Page {_currentPage} of {_totalPages}";
            btnPrev.Enabled = btnFirst.Enabled = _currentPage > 1;
            btnNext.Enabled = btnLast.Enabled = _currentPage < _totalPages;
        }

        private void dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvCustomer.Columns.Contains("ViewDoc"))
            {
                SetViewDocIcons();
            }
        }
        System.Drawing.Image enabledIcon = Clinic.Properties.Resources.doc_icon;   // regular icon
        System.Drawing.Image disabledIcon = null; // or Properties.Resources.doc_icon_disabled

        private void SetViewDocIcons()
        {
            foreach (DataGridViewRow row in dgvCustomer.Rows)
            {
                int purchaseId = Convert.ToInt32(row.Cells["id"].Value);

                bool hasAttachment = AttachmentManager.HasAttachment(purchaseId, "STOCK RECONCILIATION");
                // You need a function to check if an invoice has attachments
                row.Cells["ViewDoc"].Value = hasAttachment ? enabledIcon : disabledIcon;
            }
        }

        private void AddViewDocImageColumn()
        {
            if (!dgvCustomer.Columns.Contains("ViewDoc"))
            {
                DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
                imgColumn.Name = "ViewDoc";
                imgColumn.HeaderText = "View Doc";
                //imgColumn.Image = Properties.Resources.doc_icon; // <-- your image here
                imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom; // fit nicely
                dgvCustomer.Columns.Add(imgColumn);
            }
        }
        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCustomer.Columns[e.ColumnIndex].Name == "ViewDoc")
            {
                var cell = dgvCustomer.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value == null) return; // no attachment, do nothing

                int purchaseId = Convert.ToInt32(dgvCustomer.Rows[e.RowIndex].Cells["id"].Value);
                AttachmentManager.OpenAttachmentByType(purchaseId, "STOCK RECONCILIATION");
            }
        }

        private void bindInvoiceItems()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                concat(i.code,' - ' ,i.name) as 'Item Name',
								sri.system_qty `System Qty`,sri.physical_qty `Physical Qty`,sri.variance_qty `Variance Qty`,sri.uom `UOM`,sri.unit_cost `Cost`,sri.variance_value `Value`,
                                sri.Remarks
									 FROM tbl_stock_reconciliation_item sri
                            INNER JOIN tbl_items i ON sri.item_id = i.id
                            WHERE sri.reconciliation_id = @id",
                            ClinicDatabase.CreateParameterNew("@id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));

            dgvItems.DataSource = dt;
            dgvItems.Columns["item name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvItems);
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentPage = 1;
            BindInvoices();
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            frmMain.openChildForm(new frmStockReconciliation( int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            frmMain.openChildForm(new frmStockReconciliation(int.Parse(dgvCustomer.CurrentRow.Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void dgvCustomer_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void chkCustomer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustomer.Checked)
                cmbCustomer.Enabled = false;
            else
                cmbCustomer.Enabled = true;

            _currentPage = 1;
            BindInvoices();
        }
        private void chkPayment_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPayment.Checked)
                cmbPaymentMethod.Enabled = false;
            else
                cmbPaymentMethod.Enabled = true;

            _currentPage = 1;
            BindInvoices();
        }
        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
                dtFrom.Enabled = dtTo.Enabled = false;
            else
                dtFrom.Enabled = dtTo.Enabled = true;

            _currentPage = 1;
            BindInvoices();
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count > 0 && cmbSelectionMethod.Text == "Default")
                bindInvoiceItems();
            else
                dgvItems.DataSource = null;
        }

        private void btnCreateBill_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            //if (int.Parse(dgvCustomer.SelectedRows[0].Cells["tranfer_status"].Value.ToString()) <= 0)
            //{
            //    frmMain.openChildForm(new frmPurchaseOrder(int.Parse(dgvCustomer.CurrentRow.Cells["id"].Value.ToString()), "PR"));
            //}
            //else
            //{
            //    MessageBox.Show("Materila Consumption already created.", "Cannot Create", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //if (dgvCustomer.Rows.Count == 0)
            //    return;
            //ClinicDatabase.ExecuteNonQuery("UPDATE tbl_stock_reconciliation SET state = -1 WHERE id = @id ",
            //                              ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            //////Utilities.LogAudit(-1, "Delete Stock Reconciliation", "Stock Reconciliation", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()), "Deleted Stock Reconciliation: " + dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString());

            //_currentPage = 1;
            //BindInvoices();
        }
        private void btnRecycle_Click(object sender, EventArgs e)
        {
            //MasterRecycle restoreForm = new MasterRecycle("StockReconciliation");
            //restoreForm.DataRestored += RestoreForm_DataRestored;
            //restoreForm.ShowDialog();
        }
        private void RestoreForm_DataRestored(object sender, EventArgs e)
        {
            _currentPage = 1;
            BindInvoices();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            BindInvoices();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            BindInvoices();
        }
    }
}
