using Clinic_DataAccess;
using System;
using System.Data;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmPutawayRules : Form
    {
        private readonly int fixedSourceLocationId;

        public frmPutawayRules(int sourceLocationId = 0)
        {
            InitializeComponent();
            fixedSourceLocationId = sourceLocationId;
        }

        private void frmPutawayRules_Load(object sender, EventArgs e)
        {
            try
            {
                LoadComboData();
                LoadRules();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load Putaway Rules.\r\n\r\n" + ex.Message,
                    "Putaway Rules",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadComboData()
        {
            DataTable locations = ClinicDatabase.ExecuteDataTable(@"
                SELECT
                    l.id,
                    l.warehouse_id,
                    CONCAT(
                        IFNULL(w.name,''),
                        '/',
                        IFNULL(NULLIF(l.code,''),l.name)
                    ) AS display_name
                FROM tbl_warehouse_location l
                INNER JOIN tbl_warehouse w
                    ON w.id = l.warehouse_id
                WHERE l.state = 0
                  AND l.active = 1
                ORDER BY w.name, l.name;");

            colSourceLocation.DataSource = locations.Copy();
            colSourceLocation.DisplayMember = "display_name";
            colSourceLocation.ValueMember = "id";

            colDestinationLocation.DataSource = locations.Copy();
            colDestinationLocation.DisplayMember = "display_name";
            colDestinationLocation.ValueMember = "id";

            DataTable products = ClinicDatabase.ExecuteDataTable(@"
                SELECT
                    id,
                    CONCAT(
                        '[',
                        IFNULL(code,''),
                        '] ',
                        IFNULL(name,'')
                    ) AS display_name
                FROM tbl_items
                WHERE state = 0
                ORDER BY name;");

            DataRow emptyProduct = products.NewRow();
            emptyProduct["id"] = 0;
            emptyProduct["display_name"] = "(Any Product in Category)";
            products.Rows.InsertAt(emptyProduct, 0);

            colProduct.DataSource = products;
            colProduct.DisplayMember = "display_name";
            colProduct.ValueMember = "id";

            DataTable categories = ClinicDatabase.ExecuteDataTable(@"
                SELECT id, name
                FROM tbl_item_category
                ORDER BY name;");

            DataRow emptyCategory = categories.NewRow();
            emptyCategory["id"] = 0;
            emptyCategory["name"] = "(No Category)";
            categories.Rows.InsertAt(emptyCategory, 0);

            colCategory.DataSource = categories;
            colCategory.DisplayMember = "name";
            colCategory.ValueMember = "id";
        }

        private void LoadRules()
        {
            dgvRules.Rows.Clear();

            DataTable dt =
                InventoryPutawayService.GetRules(
                    fixedSourceLocationId);

            if (dt == null)
            {
                lblCount.Text = "0 rules";
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                int index = dgvRules.Rows.Add();
                DataGridViewRow gridRow = dgvRules.Rows[index];

                gridRow.Cells["colRuleId"].Value = row["id"];
                gridRow.Cells["colSourceLocation"].Value = row["source_location_id"];
                gridRow.Cells["colProduct"].Value = row["item_id"];
                gridRow.Cells["colCategory"].Value = row["item_category_id"];
                gridRow.Cells["colDestinationLocation"].Value = row["destination_location_id"];
                gridRow.Cells["colPriority"].Value = row["priority"];
                gridRow.Cells["colActive"].Value =
                    Convert.ToInt32(row["active"]) == 1;
            }

            lblCount.Text = dgvRules.Rows.Count + " rules";
        }

        private void btnAddRule_Click(object sender, EventArgs e)
        {
            int index = dgvRules.Rows.Add();
            DataGridViewRow row = dgvRules.Rows[index];

            row.Cells["colRuleId"].Value = 0;
            row.Cells["colProduct"].Value = 0;
            row.Cells["colCategory"].Value = 0;
            row.Cells["colPriority"].Value = 10;
            row.Cells["colActive"].Value = true;

            if (fixedSourceLocationId > 0)
            {
                row.Cells["colSourceLocation"].Value =
                    fixedSourceLocationId;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dgvRules.EndEdit();

                foreach (DataGridViewRow row in dgvRules.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    int ruleId = SafeInt(row.Cells["colRuleId"].Value);

                    int sourceLocationId =
                        fixedSourceLocationId > 0
                            ? fixedSourceLocationId
                            : SafeInt(row.Cells["colSourceLocation"].Value);

                    int itemId = SafeInt(row.Cells["colProduct"].Value);
                    int categoryId = SafeInt(row.Cells["colCategory"].Value);
                    int destinationLocationId =
                        SafeInt(row.Cells["colDestinationLocation"].Value);
                    int priority = SafeInt(row.Cells["colPriority"].Value);
                    bool active = SafeBool(row.Cells["colActive"].Value);

                    if (sourceLocationId <= 0 &&
                        itemId <= 0 &&
                        categoryId <= 0 &&
                        destinationLocationId <= 0)
                    {
                        continue;
                    }

                    if (sourceLocationId <= 0)
                    {
                        MessageBox.Show(
                            "Select 'When product arrives in' location.");
                        return;
                    }

                    if (destinationLocationId <= 0)
                    {
                        MessageBox.Show(
                            "Select 'Store to sublocation'.");
                        return;
                    }

                    if (sourceLocationId == destinationLocationId)
                    {
                        MessageBox.Show(
                            "Source and destination location cannot be the same.");
                        return;
                    }

                    if (itemId <= 0 && categoryId <= 0)
                    {
                        MessageBox.Show(
                            "Select either Product or Product Category.");
                        return;
                    }

                    int sourceWarehouseId = GetWarehouseId(sourceLocationId);
                    int destinationWarehouseId = GetWarehouseId(destinationLocationId);

                    if (sourceWarehouseId <= 0 ||
                        destinationWarehouseId <= 0 ||
                        sourceWarehouseId != destinationWarehouseId)
                    {
                        MessageBox.Show(
                            "Putaway source and destination must be in the same warehouse.");
                        return;
                    }

                    if (ruleId <= 0)
                    {
                        object newId = ClinicDatabase.ExecuteScalar(@"
                            INSERT INTO tbl_putaway_rule
                            (
                                warehouse_id,
                                source_location_id,
                                item_id,
                                item_category_id,
                                destination_location_id,
                                priority,
                                active,
                                state,
                                created_by,
                                created_date
                            )
                            VALUES
                            (
                                @warehouseId,
                                @sourceLocationId,
                                @itemId,
                                @categoryId,
                                @destinationLocationId,
                                @priority,
                                @active,
                                0,
                                @createdBy,
                                NOW()
                            );

                            SELECT LAST_INSERT_ID();",
                            ClinicDatabase.CreateParameterNew("@warehouseId", sourceWarehouseId),
                            ClinicDatabase.CreateParameterNew("@sourceLocationId", sourceLocationId),
                            ClinicDatabase.CreateParameterNew("@itemId", itemId),
                            ClinicDatabase.CreateParameterNew("@categoryId", categoryId),
                            ClinicDatabase.CreateParameterNew("@destinationLocationId", destinationLocationId),
                            ClinicDatabase.CreateParameterNew("@priority", priority),
                            ClinicDatabase.CreateParameterNew("@active", active ? 1 : 0),
                            ClinicDatabase.CreateParameterNew("@createdBy", -1));

                        row.Cells["colRuleId"].Value =
                            Convert.ToInt32(newId);
                    }
                    else
                    {
                        ClinicDatabase.ExecuteNonQuery(@"
                            UPDATE tbl_putaway_rule
                            SET warehouse_id = @warehouseId,
                                source_location_id = @sourceLocationId,
                                item_id = @itemId,
                                item_category_id = @categoryId,
                                destination_location_id = @destinationLocationId,
                                priority = @priority,
                                active = @active,
                                modified_by = @modifiedBy,
                                modified_date = NOW()
                            WHERE id = @id;",
                            ClinicDatabase.CreateParameterNew("@warehouseId", sourceWarehouseId),
                            ClinicDatabase.CreateParameterNew("@sourceLocationId", sourceLocationId),
                            ClinicDatabase.CreateParameterNew("@itemId", itemId),
                            ClinicDatabase.CreateParameterNew("@categoryId", categoryId),
                            ClinicDatabase.CreateParameterNew("@destinationLocationId", destinationLocationId),
                            ClinicDatabase.CreateParameterNew("@priority", priority),
                            ClinicDatabase.CreateParameterNew("@active", active ? 1 : 0),
                            ClinicDatabase.CreateParameterNew("@modifiedBy", -1),
                            ClinicDatabase.CreateParameterNew("@id", ruleId));
                    }
                }

                MessageBox.Show(
                    "Putaway Rules saved.",
                    "Putaway Rules",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadRules();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not save Putaway Rules.\r\n\r\n" + ex.Message,
                    "Putaway Rules",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvRules_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0)
                return;

            if (dgvRules.Columns[e.ColumnIndex].Name != "colDelete")
                return;

            DataGridViewRow row = dgvRules.Rows[e.RowIndex];
            int ruleId = SafeInt(row.Cells["colRuleId"].Value);

            if (ruleId > 0)
            {
                DialogResult answer = MessageBox.Show(
                    "Delete this Putaway Rule?",
                    "Putaway Rules",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (answer != DialogResult.Yes)
                    return;

                ClinicDatabase.ExecuteNonQuery(@"
                    UPDATE tbl_putaway_rule
                    SET state = -1,
                        active = 0
                    WHERE id = @id;",
                    ClinicDatabase.CreateParameterNew("@id", ruleId));
            }

            dgvRules.Rows.RemoveAt(e.RowIndex);
            lblCount.Text = dgvRules.Rows.Count + " rules";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int GetWarehouseId(int locationId)
        {
            object result = ClinicDatabase.ExecuteScalar(@"
                SELECT warehouse_id
                FROM tbl_warehouse_location
                WHERE id = @locationId
                LIMIT 1;",
                ClinicDatabase.CreateParameterNew("@locationId", locationId));

            if (result == null || result == DBNull.Value)
                return 0;

            return Convert.ToInt32(result);
        }

        private int SafeInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            int result;

            return int.TryParse(value.ToString(), out result)
                ? result
                : 0;
        }

        private bool SafeBool(object value)
        {
            if (value == null || value == DBNull.Value)
                return false;

            bool result;

            if (bool.TryParse(value.ToString(), out result))
                return result;

            return value.ToString() == "1";
        }
    }
}
