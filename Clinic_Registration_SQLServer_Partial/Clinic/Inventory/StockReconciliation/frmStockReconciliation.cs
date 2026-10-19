using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace YamyProject.UI.Inventory.StockReconciliation
{
    public partial class frmStockReconciliation : Form
    {
        int reconciliationId = 0;

        public frmStockReconciliation(int _id = 0)
        {
            InitializeComponent();

            this.reconciliationId = _id;
            Load += Frm_Load;

            btnNew.Click += BtnNew_Click;
            btnClear.Click += BtnNew_Click;

            btnAddItem.Click += BtnAddItem_Click;
            btnSave.Click += BtnSave_Click;
            btnApprove.Click += BtnApprove_Click;

            btnLoadStock.Click += BtnLoadStock_Click;

            txtPhysicalQty.TextChanged += CalculateVariance;
            txtCost.TextChanged += CalculateVariance;

            cmbItem.SelectedIndexChanged +=
                CmbItem_SelectedIndexChanged;
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            LoadCombos();
            InitGrid();

            if (reconciliationId > 0)
                LoadData();
            else
                NewRecord();
        }
        private void LoadData()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_stock_reconciliation where reconciliation_id = @id", ClinicDatabase.CreateParameterNew("@id", reconciliationId)))
            {
                if (reader.Read())
                {
                    txtNo.Text = reader["reconciliation_no"].ToString();
                    dtDate.Value = Convert.ToDateTime(reader["reconciliation_date"]);
                    cmbWarehouse.SelectedValue = reader["warehouse_id"];
                    cmbProject.SelectedValue = reader["project_id"];
                    cmbType.Text = reader["reconciliation_type"].ToString();
                    cmbCheckedBy.SelectedValue = reader["checked_by"];
                    cmbStatus.Text = reader["status"].ToString();
                    txtRemarks.Text = reader["remarks"].ToString();

                    //items
                    using (MySqlDataReader reader1 = ClinicDatabase.ExecuteReader("select i.name,sri.* from tbl_stock_reconciliation_item sri INNER JOIN tbl_items i ON i.id = sri.item_id where reconciliation_id = @id", ClinicDatabase.CreateParameterNew("@id", reconciliationId)))
                    {
                        while (reader1.Read())
                        {
                            dgvItems.Rows.Add(reader1["item_id"], reader1["name"], reader1["system_qty"], reader1["physical_qty"], reader1["variance_qty"], reader1["adjustment_type"], reader1["uom"], reader1["unit_cost"], reader1["variance_value"]);
                        }
                    }

                    CalculateSummary();
                }
            }
        }
        void LoadCombos()
        {
            cmbWarehouse.DataSource =
                ClinicDatabase.ExecuteDataTable(
                "SELECT id, name FROM tbl_warehouse");

            cmbWarehouse.DisplayMember = "name";
            cmbWarehouse.ValueMember = "id";

            cmbProject.DataSource =
                ClinicDatabase.ExecuteDataTable(
                "SELECT id, name FROM tbl_projects");

            cmbProject.DisplayMember = "name";
            cmbProject.ValueMember = "id";

            cmbCheckedBy.DataSource =
                ClinicDatabase.ExecuteDataTable(
                "SELECT id, name FROM tbl_employee");

            cmbCheckedBy.DisplayMember = "name";
            cmbCheckedBy.ValueMember = "id";

            cmbItem.DataSource =
                ClinicDatabase.ExecuteDataTable(
                "SELECT id, name FROM tbl_items");

            cmbItem.DisplayMember = "name";
            cmbItem.ValueMember = "id";

            cmbType.Items.AddRange(new object[]
            {
                "Periodic",
                "Cycle Count",
                "Annual Audit",
                "Damage Adjustment"
            });

            cmbStatus.Items.AddRange(new object[]
            {
                "Draft",
                "Pending",
                "Approved",
                "Cancelled"
            });
        }

        void InitGrid()
        {
            dgvItems.Columns.Add("itemId", "Item Id");
            dgvItems.Columns.Add("itemName", "Item");
            dgvItems.Columns.Add("systemQty", "System Qty");
            dgvItems.Columns.Add("physicalQty", "Physical Qty");
            dgvItems.Columns.Add("varianceQty", "Variance");
            dgvItems.Columns.Add("adjustment", "Adjustment");
            dgvItems.Columns.Add("uom", "UOM");
            dgvItems.Columns.Add("cost", "Cost");
            dgvItems.Columns.Add("value", "Value");

            dgvItems.Columns["itemId"].Visible = false;
        }

        void NewRecord()
        {
            reconciliationId = 0;

            txtNo.Text =
                "REC-" +
                DateTime.Now.ToString("yyyyMMddHHmmss");

            dtDate.Value = DateTime.Today;

            cmbType.Text = "Periodic";
            cmbStatus.Text = "Draft";

            txtRemarks.Clear();

            txtPositiveVariance.Text = "0";
            txtNegativeVariance.Text = "0";

            dgvItems.Rows.Clear();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            NewRecord();
        }

        private void CmbItem_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (cmbItem.SelectedValue == null ||
                cmbWarehouse.SelectedValue == null)
                return;

            string sql = @"
            SELECT IFNULL(SUM(qty_in - qty_out),0)
            FROM tbl_item_transaction
            WHERE item_id=@item
            AND warehouse_id=@warehouse";

            txtSystemQty.Text =
                ClinicDatabase.ExecuteScalar(sql,

                ClinicDatabase.CreateParameterNew("@item",
                    cmbItem.SelectedValue),

                ClinicDatabase.CreateParameterNew("@warehouse",
                    cmbWarehouse.SelectedValue)
                ).ToString();
        }

        void CalculateVariance(object sender, EventArgs e)
        {
            try
            {
                decimal systemQty =
                    Convert.ToDecimal(txtSystemQty.Text);

                decimal physicalQty =
                    Convert.ToDecimal(txtPhysicalQty.Text);

                decimal variance =
                    physicalQty - systemQty;

                txtVariance.Text =
                    variance.ToString("0.000");

                decimal cost =
                    Convert.ToDecimal(txtCost.Text);

                txtVarianceValue.Text =
                    (variance * cost).ToString("0.00");
            }
            catch
            {
                txtVariance.Text = "0";
                txtVarianceValue.Text = "0";
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            decimal variance =
                Convert.ToDecimal(txtVariance.Text);

            string adjustment =
                variance >= 0 ? "INCREASE" : "DECREASE";

            dgvItems.Rows.Add(
                cmbItem.SelectedValue,
                cmbItem.Text,
                txtSystemQty.Text,
                txtPhysicalQty.Text,
                txtVariance.Text,
                adjustment,
                txtUom.Text,
                txtCost.Text,
                txtVarianceValue.Text
            );

            CalculateSummary();
        }

        void CalculateSummary()
        {
            decimal positive = 0;
            decimal negative = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                decimal variance =
                    Convert.ToDecimal(
                        row.Cells["varianceQty"].Value);

                if (variance > 0)
                    positive += variance;

                if (variance < 0)
                    negative += Math.Abs(variance);
            }

            txtPositiveVariance.Text =
                positive.ToString("0.000");

            txtNegativeVariance.Text =
                negative.ToString("0.000");
        }

        private void BtnLoadStock_Click(
            object sender, EventArgs e)
        {
            dgvItems.Rows.Clear();

            string sql = @"
            SELECT
                i.item_id,
                i.item_name,

                IFNULL(SUM(
                    t.qty_in - t.qty_out
                ),0) stock_qty

            FROM tbl_items i

            LEFT JOIN tbl_item_transaction t
                ON i.item_id=t.item_id
                AND t.warehouse_id=@warehouse

            GROUP BY i.item_id,i.item_name

            HAVING stock_qty <> 0

            ORDER BY i.item_name";

            DataTable dt =
                ClinicDatabase.ExecuteDataTable(sql,

                ClinicDatabase.CreateParameterNew("@warehouse",
                    cmbWarehouse.SelectedValue));

            foreach (DataRow row in dt.Rows)
            {
                dgvItems.Rows.Add(
                    row["item_id"],
                    row["item_name"],
                    row["stock_qty"],
                    row["stock_qty"],
                    0,
                    "",
                    "",
                    0,
                    0
                );
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (dgvItems.Rows.Count <= 0) return;

            string sql = @"
            INSERT INTO tbl_stock_reconciliation
            (
                reconciliation_no,
                reconciliation_date,
                warehouse_id,
                project_id,
                reconciliation_type,
                checked_by,
                status,
                remarks,
                created_by
            )
            VALUES
            (
                @no,@date,@warehouse,@project,
                @type,@checkedby,@status,
                @remarks,@user
            );

            SELECT LAST_INSERT_ID();";

            reconciliationId =
                Convert.ToInt32(ClinicDatabase.ExecuteScalar(sql,

                ClinicDatabase.CreateParameterNew("@no", txtNo.Text),

                ClinicDatabase.CreateParameterNew("@date",
                    dtDate.Value.Date),

                ClinicDatabase.CreateParameterNew("@warehouse",
                    cmbWarehouse.SelectedValue),

                ClinicDatabase.CreateParameterNew("@project",
                    cmbProject.SelectedValue),

                ClinicDatabase.CreateParameterNew("@type",
                    cmbType.Text),

                ClinicDatabase.CreateParameterNew("@checkedby",
                    cmbCheckedBy.SelectedValue),

                ClinicDatabase.CreateParameterNew("@status",
                    cmbStatus.Text),

                ClinicDatabase.CreateParameterNew("@remarks",
                    txtRemarks.Text),

                ClinicDatabase.CreateParameterNew("@user",
                    -1)
            ));

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(@"
                INSERT INTO tbl_stock_reconciliation_item
                (
                    reconciliation_id,
                    item_id,
                    system_qty,
                    physical_qty,
                    variance_qty,
                    adjustment_type,
                    uom,
                    unit_cost,
                    variance_value
                )
                VALUES
                (
                    @reconciliation,
                    @item,
                    @system,
                    @physical,
                    @variance,
                    @adjustment,
                    @uom,
                    @cost,
                    @value
                )",

                ClinicDatabase.CreateParameterNew("@reconciliation",
                    reconciliationId),

                ClinicDatabase.CreateParameterNew("@item",
                    row.Cells["itemId"].Value),

                ClinicDatabase.CreateParameterNew("@system",
                    row.Cells["systemQty"].Value),

                ClinicDatabase.CreateParameterNew("@physical",
                    row.Cells["physicalQty"].Value),

                ClinicDatabase.CreateParameterNew("@variance",
                    row.Cells["varianceQty"].Value),

                ClinicDatabase.CreateParameterNew("@adjustment",
                    row.Cells["adjustment"].Value),

                ClinicDatabase.CreateParameterNew("@uom",
                    row.Cells["uom"].Value),

                ClinicDatabase.CreateParameterNew("@cost",
                    row.Cells["cost"].Value),

                ClinicDatabase.CreateParameterNew("@value",
                    row.Cells["value"].Value));
            }

            //.RefreshStockReconciliation();
            MessageBox.Show(
                "Stock Reconciliation Saved Successfully");
        }

        private void BtnApprove_Click(
            object sender, EventArgs e)
        {
            if (reconciliationId == 0)
                return;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                decimal variance =
                    Convert.ToDecimal(
                        row.Cells["varianceQty"].Value);

                if (variance == 0)
                    continue;

                if (variance > 0)
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                    INSERT INTO tbl_item_transaction
                    (
                        date,type,reference,item_id,
                        cost_price,qty_in,
                        description,
                        warehouse_id,
                        project_id, GlobalId, DeviceId
                    )
                    VALUES
                    (
                        @date,
                        'STOCK_ADJUSTMENT_IN',
                        @ref,
                        @item,
                        @cost,
                        @qty,
                        'Stock Reconciliation Increase',
                        @warehouse,
                        @project, @GlobalId, @DeviceId
                    )",

                    ClinicDatabase.CreateParameterNew("@date",
                        dtDate.Value.Date),

                    ClinicDatabase.CreateParameterNew("@ref",
                        txtNo.Text),

                    ClinicDatabase.CreateParameterNew("@item",
                        row.Cells["itemId"].Value),

                    ClinicDatabase.CreateParameterNew("@cost",
                        row.Cells["cost"].Value),

                    ClinicDatabase.CreateParameterNew("@qty",
                        variance),

                    ClinicDatabase.CreateParameterNew("@warehouse",
                        cmbWarehouse.SelectedValue),
                ClinicDatabase.CreateParameterNew("GlobalId", Guid.NewGuid().ToString()),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName),

                    ClinicDatabase.CreateParameterNew("@project",
                        cmbProject.SelectedValue));
                }
                else
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                    INSERT INTO tbl_item_transaction
                    (
                        date,type,reference,item_id,
                        cost_price,qty_out,
                        description,
                        warehouse_id,
                        project_id, GlobalId, DeviceId
                    )
                    VALUES
                    (
                        @date,
                        'STOCK_ADJUSTMENT_OUT',
                        @ref,
                        @item,
                        @cost,
                        @qty,
                        'Stock Reconciliation Decrease',
                        @warehouse,
                        @project, @GlobalId, @DeviceId
                    )",

                    ClinicDatabase.CreateParameterNew("@date",
                        dtDate.Value.Date),

                    ClinicDatabase.CreateParameterNew("@ref",
                        txtNo.Text),

                    ClinicDatabase.CreateParameterNew("@item",
                        row.Cells["itemId"].Value),

                    ClinicDatabase.CreateParameterNew("@cost",
                        row.Cells["cost"].Value),

                    ClinicDatabase.CreateParameterNew("@qty",
                        Math.Abs(variance)),

                    ClinicDatabase.CreateParameterNew("@warehouse",
                        cmbWarehouse.SelectedValue),
                ClinicDatabase.CreateParameterNew("GlobalId", Guid.NewGuid().ToString()),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName),

                    ClinicDatabase.CreateParameterNew("@project",
                        cmbProject.SelectedValue));
                }
            }

            ClinicDatabase.ExecuteNonQuery(@"
            UPDATE tbl_stock_reconciliation
            SET status='Approved',
                approved_by=@user
            WHERE reconciliation_id=@id",

            ClinicDatabase.CreateParameterNew("@user",
                -1),

            ClinicDatabase.CreateParameterNew("@id",
                reconciliationId));

            //.RefreshStockReconciliation();
            MessageBox.Show(
                "Stock Reconciliation Approved Successfully");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (reconciliationId == 0) return;

            if (dgvItems.Rows.Count <= 0) return;

            // Update header
            string updateQuery = @"
            UPDATE tbl_stock_reconciliation
            SET
                reconciliation_no = @no,
                reconciliation_date = @date,
                warehouse_id = @warehouse,
                project_id = @project,
                reconciliation_type = @type,
                checked_by = @checkedby,
                status = @status,
                remarks = @remarks
            WHERE reconciliation_id = @id";

            ClinicDatabase.ExecuteNonQuery(updateQuery,

                ClinicDatabase.CreateParameterNew("@no", txtNo.Text),

                ClinicDatabase.CreateParameterNew("@date", dtDate.Value.Date),

                ClinicDatabase.CreateParameterNew("@warehouse",
                    cmbWarehouse.SelectedValue),

                ClinicDatabase.CreateParameterNew("@project",
                    cmbProject.SelectedValue),

                ClinicDatabase.CreateParameterNew("@type",
                    cmbType.Text),

                ClinicDatabase.CreateParameterNew("@checkedby",
                    cmbCheckedBy.SelectedValue),

                ClinicDatabase.CreateParameterNew("@status",
                    cmbStatus.Text),

                ClinicDatabase.CreateParameterNew("@remarks",
                    txtRemarks.Text),

                ClinicDatabase.CreateParameterNew("@id",
                    reconciliationId)
            );

            // Remove existing items for this reconciliation
            ClinicDatabase.ExecuteNonQuery(
                "DELETE FROM tbl_stock_reconciliation_item WHERE reconciliation_id=@id",
                ClinicDatabase.CreateParameterNew("@id", reconciliationId)
            );

            // Re-insert current items
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(@"
                INSERT INTO tbl_stock_reconciliation_item
                (
                    reconciliation_id,
                    item_id,
                    system_qty,
                    physical_qty,
                    variance_qty,
                    adjustment_type,
                    uom,
                    unit_cost,
                    variance_value
                )
                VALUES
                (
                    @reconciliation,
                    @item,
                    @system,
                    @physical,
                    @variance,
                    @adjustment,
                    @uom,
                    @cost,
                    @value
                )",

                ClinicDatabase.CreateParameterNew("@reconciliation", reconciliationId),

                ClinicDatabase.CreateParameterNew("@item",
                    row.Cells["itemId"].Value),

                ClinicDatabase.CreateParameterNew("@system",
                    row.Cells["systemQty"].Value),

                ClinicDatabase.CreateParameterNew("@physical",
                    row.Cells["physicalQty"].Value),

                ClinicDatabase.CreateParameterNew("@variance",
                    row.Cells["varianceQty"].Value),

                ClinicDatabase.CreateParameterNew("@adjustment",
                    row.Cells["adjustment"].Value),

                ClinicDatabase.CreateParameterNew("@uom",
                    row.Cells["uom"].Value),

                ClinicDatabase.CreateParameterNew("@cost",
                    row.Cells["cost"].Value),

                ClinicDatabase.CreateParameterNew("@value",
                    row.Cells["value"].Value));
            }

            //.RefreshStockReconciliation();
            MessageBox.Show("Stock Reconciliation Updated Successfully");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (reconciliationId == 0) return;

            var confirm = MessageBox.Show(
                "Are you sure you want to delete this Stock Reconciliation?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            // Delete child items first
            ClinicDatabase.ExecuteNonQuery(
                "DELETE FROM tbl_stock_reconciliation_item WHERE reconciliation_id=@id",
                ClinicDatabase.CreateParameterNew("@id", reconciliationId)
            );

            // Delete header
            ClinicDatabase.ExecuteNonQuery(
                "DELETE FROM tbl_stock_reconciliation WHERE reconciliation_id=@id",
                ClinicDatabase.CreateParameterNew("@id", reconciliationId)
            );

            reconciliationId = 0;
            NewRecord();

            //.RefreshStockReconciliation();
            MessageBox.Show("Stock Reconciliation Deleted Successfully");
        }
    }
}