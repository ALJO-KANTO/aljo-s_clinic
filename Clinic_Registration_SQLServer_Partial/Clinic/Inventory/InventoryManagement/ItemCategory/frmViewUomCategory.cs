using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmViewUomCategory : Form
    {
        private int categoryId;
        // IDs that existed when edit page was loaded.
        private readonly List<int> originalUnitIds =
            new List<int>();

        public frmViewUomCategory(int id = 0)
        {
            InitializeComponent();
            headerUC1.FormText = this.Text;
            categoryId = id;

            // Make runtime grid compatible with new UOM logic.
            EnsureUomGridColumns();
            Localization.LocalizationManager.LocalizeForm(this);
        }
        private void frmViewUomCategory_Load(
             object sender,
             EventArgs e)
        {
            if (categoryId > 0)
            {
                LoadCategory();
            }
            else
            {
                AddDefaultReferenceRow();
            }
            Localization.LocalizationManager.LocalizeDataGridViewHeaders(dgvUnits);
        }

        private void AddDefaultReferenceRow()
        {
            EnsureUomGridColumns();

            int rowIndex = dgvUnits.Rows.Add();

            DataGridViewRow row =
                dgvUnits.Rows[rowIndex];

            row.Cells["colUnitId"].Value = 0;
            row.Cells["colUnit"].Value = "";
            row.Cells["colType"].Value = "Reference";
            row.Cells["colRatio"].Value = 1m;
            row.Cells["colUnitActive"].Value = true;
            row.Cells["colPrecision"].Value = 0.01m;
        }

        private void LoadCategory()
        {
            // ============================================================
            // LOAD CATEGORY HEADER
            // ============================================================
            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(@"
            SELECT
                id,
                name,
                active
            FROM tbl_uom_category
            WHERE id = @id
              AND state = 0
            LIMIT 1;",
                    ClinicDatabase.CreateParameterNew(
                        "@id",
                        categoryId)))
            {
                if (!reader.Read())
                {
                    MessageBox.Show(
                        "Unit of Measure Category not found.",
                        "UOM",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Close();
                    return;
                }

                txtCategory.Text =
                    reader["name"] == DBNull.Value
                        ? ""
                        : reader["name"].ToString();

                chkActive.Checked =
                    reader["active"] != DBNull.Value &&
                    Convert.ToInt32(reader["active"]) == 1;
            }

            // ============================================================
            // CLEAR OLD GRID DATA
            // ============================================================
            dgvUnits.Rows.Clear();
            originalUnitIds.Clear();

            // ============================================================
            // LOAD UOM ROWS
            // ============================================================
            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(@"
            SELECT
                id,
                name,
                uom_type,
                ratio,
                rounding_precision,
                active
            FROM tbl_uom
            WHERE category_id = @category
              AND state = 0
            ORDER BY
                CASE
                    WHEN uom_type = 'reference' THEN 0
                    ELSE 1
                END,
                id;",
                    ClinicDatabase.CreateParameterNew(
                        "@category",
                        categoryId)))
            {
                while (reader.Read())
                {
                    int unitId =
                        reader["id"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["id"]);

                    originalUnitIds.Add(unitId);

                    // ====================================================
                    // UOM TYPE
                    // ====================================================
                    string dbType =
                        reader["uom_type"] == DBNull.Value
                            ? ""
                            : reader["uom_type"]
                                .ToString()
                                .Trim()
                                .ToLower();

                    string displayType;

                    switch (dbType)
                    {
                        case "reference":
                            displayType = "Reference";
                            break;

                        case "bigger":
                            displayType = "Bigger";
                            break;

                        case "smaller":
                            displayType = "Smaller";
                            break;

                        default:
                            displayType = "Reference";
                            break;
                    }

                    // ====================================================
                    // RATIO
                    // ====================================================
                    decimal ratio = 1m;

                    if (reader["ratio"] != DBNull.Value)
                        decimal.TryParse(
                            reader["ratio"].ToString(),
                            out ratio);

                    if (ratio <= 0m)
                        ratio = 1m;

                    // ====================================================
                    // ROUNDING PRECISION
                    // ====================================================
                    decimal precision = 0.01m;

                    if (reader["rounding_precision"] != DBNull.Value)
                        decimal.TryParse(
                            reader["rounding_precision"].ToString(),
                            out precision);

                    if (precision <= 0m)
                        precision = 0.01m;

                    // ====================================================
                    // ACTIVE - MUST BE BOOLEAN FOR CHECKBOX COLUMN
                    // ====================================================
                    bool active =
                        reader["active"] != DBNull.Value &&
                        Convert.ToInt32(reader["active"]) == 1;

                    // ====================================================
                    // ADD EMPTY ROW FIRST
                    // IMPORTANT:
                    // DO NOT USE Rows.Add(value1, value2, ...)
                    // because that depends on Designer column order.
                    // ====================================================
                    int rowIndex =
                        dgvUnits.Rows.Add();

                    DataGridViewRow row =
                        dgvUnits.Rows[rowIndex];

                    // ====================================================
                    // ASSIGN BY COLUMN NAME
                    // ====================================================
                    row.Cells["colUnitId"].Value =
                        unitId;

                    row.Cells["colUnit"].Value =
                        reader["name"] == DBNull.Value
                            ? ""
                            : reader["name"].ToString();

                    row.Cells["colType"].Value =
                        displayType;

                    row.Cells["colRatio"].Value =
                        ratio.ToString("0.######");

                    row.Cells["colUnitActive"].Value =
                        active;

                    row.Cells["colPrecision"].Value =
                        precision.ToString("0.######");
                }
            }
        }

        private void btnAdd_Click(
    object sender,
    EventArgs e)
        {
            EnsureUomGridColumns();

            int rowIndex =
                dgvUnits.Rows.Add();

            DataGridViewRow row =
                dgvUnits.Rows[rowIndex];

            row.Cells["colUnitId"].Value = 0;
            row.Cells["colUnit"].Value = "";
            row.Cells["colType"].Value = "Bigger";
            row.Cells["colRatio"].Value = 1m;
            row.Cells["colUnitActive"].Value = true;
            row.Cells["colPrecision"].Value = 0.01m;

            dgvUnits.CurrentCell =
                row.Cells["colUnit"];

            dgvUnits.BeginEdit(true);
        }

        private bool ValidateData()
        {
            string categoryName =
                txtCategory.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                categoryName))
            {
                MessageBox.Show(
                    "Enter Unit of Measure Category.");

                txtCategory.Focus();
                return false;
            }

            object duplicateCategory =
                ClinicDatabase.ExecuteScalar(@"
                    SELECT COUNT(*)
                    FROM tbl_uom_category
                    WHERE name = @name
                      AND state = 0
                      AND id <> @id;",
                    ClinicDatabase.CreateParameterNew(
                        "@name",
                        categoryName),
                    ClinicDatabase.CreateParameterNew(
                        "@id",
                        categoryId));

            if (Convert.ToInt32(
                duplicateCategory ?? 0) > 0)
            {
                MessageBox.Show(
                    "This Unit of Measure Category already exists.");

                txtCategory.Focus();
                return false;
            }

            if (dgvUnits.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Add at least one Unit of Measure.");

                return false;
            }

            int referenceCount = 0;

            HashSet<string> names =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (DataGridViewRow row
                in dgvUnits.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string name =
                    row.Cells["colUnit"]
                    .Value?.ToString()
                    .Trim() ?? "";

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show(
                        "Enter Unit of Measure.");

                    dgvUnits.CurrentCell =
                        row.Cells["colUnit"];

                    return false;
                }

                if (!names.Add(name))
                {
                    MessageBox.Show(
                        "Duplicate Unit of Measure: " +
                        name);

                    return false;
                }

                string type =
                    row.Cells["colType"]
                    .Value?.ToString()
                    .Trim() ?? "";

                if (type != "Reference" &&
                    type != "Bigger" &&
                    type != "Smaller")
                {
                    MessageBox.Show(
                        "Select a valid UOM type for " +
                        name + ".");

                    return false;
                }

                decimal ratio;

                if (!decimal.TryParse(
                    row.Cells["colRatio"]
                    .Value?.ToString(),
                    out ratio) ||
                    ratio <= 0)
                {
                    MessageBox.Show(
                        "Ratio for " + name +
                        " must be greater than zero.");

                    dgvUnits.CurrentCell =
                        row.Cells["colRatio"];

                    return false;
                }

                if (type == "Reference")
                {
                    referenceCount++;

                    if (ratio != 1m)
                    {
                        MessageBox.Show(
                            "Reference Unit ratio must be 1.");

                        return false;
                    }
                }

                decimal precision;

                if (!decimal.TryParse(
                    row.Cells["colPrecision"]
                    .Value?.ToString(),
                    out precision) ||
                    precision <= 0)
                {
                    MessageBox.Show(
                        "Rounding Precision for " +
                        name +
                        " must be greater than zero.");

                    return false;
                }
            }

            if (referenceCount != 1)
            {
                MessageBox.Show(
                    "Each category must have exactly one Reference Unit.");

                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvUnits.EndEdit();

            if (!ValidateData())
                return;

            try
            {
                if (categoryId == 0)
                    InsertCategory();
                else
                    UpdateCategory();

                MessageBox.Show(
                    "Unit of Measure saved successfully.",
                    "UOM",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                EventHub.RefreshItemUnit();

                DialogResult =
                    DialogResult.OK;

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save Unit of Measure.\n\n" +
                    ex.Message,
                    "UOM",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InsertCategory()
        {
            object result =
                ClinicDatabase.ExecuteScalar(@"
                    INSERT INTO tbl_uom_category
                    (
                        name,
                        active,
                        state,
                        created_by,
                        created_date
                    )
                    VALUES
                    (
                        @name,
                        @active,
                        0,
                        @created_by,
                        NOW()
                    );

                    SELECT LAST_INSERT_ID();",
                    ClinicDatabase.CreateParameterNew(
                        "@name",
                        txtCategory.Text.Trim()),
                    ClinicDatabase.CreateParameterNew(
                        "@active",
                        chkActive.Checked ? 1 : 0),
                    ClinicDatabase.CreateParameterNew(
                        "@created_by",
                        clsGlobal.CurrentUser.UserID));

            categoryId =
                Convert.ToInt32(result);

            SaveUnits();
        }

        private void UpdateCategory()
        {
            ClinicDatabase.ExecuteNonQuery(@"
                UPDATE tbl_uom_category
                SET
                    name = @name,
                    active = @active
                WHERE id = @id;",
                ClinicDatabase.CreateParameterNew(
                    "@name",
                    txtCategory.Text.Trim()),
                ClinicDatabase.CreateParameterNew(
                    "@active",
                    chkActive.Checked ? 1 : 0),
                ClinicDatabase.CreateParameterNew(
                    "@id",
                    categoryId));

            SaveUnits();
            DeactivateRemovedUnits();
        }

        private void SaveUnits()
        {
            foreach (DataGridViewRow row
                in dgvUnits.Rows)
            {
                if (row.IsNewRow)
                    continue;

                int unitId = 0;

                if (row.Cells["colUnitId"].Value != null)
                {
                    int.TryParse(
                        row.Cells["colUnitId"]
                        .Value.ToString(),
                        out unitId);
                }

                string name =
                    row.Cells["colUnit"]
                    .Value?.ToString()
                    .Trim() ?? "";

                string displayType =
                    row.Cells["colType"]
                    .Value?.ToString()
                    .Trim() ?? "";

                string dbType;

                if (displayType == "Reference")
                    dbType = "reference";
                else if (displayType == "Bigger")
                    dbType = "bigger";
                else
                    dbType = "smaller";

                decimal ratio =
                    Convert.ToDecimal(
                        row.Cells["colRatio"]
                        .Value);

                decimal precision =
                    Convert.ToDecimal(
                        row.Cells["colPrecision"]
                        .Value);

                bool active =
                    Convert.ToBoolean(
                        row.Cells["colUnitActive"]
                        .Value ?? true);

                if (unitId > 0)
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                        UPDATE tbl_uom
                        SET
                            name = @name,
                            uom_type = @type,
                            ratio = @ratio,
                            rounding_precision = @precision,
                            active = @active,
                            state = 0,
                            modified_by = @modified_by,
                            modified_date = NOW()
                        WHERE id = @id
                          AND category_id = @category;",
                        ClinicDatabase.CreateParameterNew(
                            "@name", name),
                        ClinicDatabase.CreateParameterNew(
                            "@type", dbType),
                        ClinicDatabase.CreateParameterNew(
                            "@ratio", ratio),
                        ClinicDatabase.CreateParameterNew(
                            "@precision", precision),
                        ClinicDatabase.CreateParameterNew(
                            "@active",
                            active ? 1 : 0),
                        ClinicDatabase.CreateParameterNew(
                            "@modified_by",
                            clsGlobal.CurrentUser.UserID),
                        ClinicDatabase.CreateParameterNew(
                            "@id", unitId),
                        ClinicDatabase.CreateParameterNew(
                            "@category",
                            categoryId));
                }
                else
                {
                    object result =
                        ClinicDatabase.ExecuteScalar(@"
                            INSERT INTO tbl_uom
                            (
                                category_id,
                                name,
                                uom_type,
                                ratio,
                                rounding_precision,
                                active,
                                state,
                                created_by,
                                created_date,
                                modified_by,
                                modified_date
                            )
                            VALUES
                            (
                                @category,
                                @name,
                                @type,
                                @ratio,
                                @precision,
                                @active,
                                0,
                                @created_by,
                                NOW(),
                                @modified_by,
                                NOW()
                            );

                            SELECT LAST_INSERT_ID();",
                            ClinicDatabase.CreateParameterNew(
                                "@category",
                                categoryId),
                            ClinicDatabase.CreateParameterNew(
                                "@name", name),
                            ClinicDatabase.CreateParameterNew(
                                "@type", dbType),
                            ClinicDatabase.CreateParameterNew(
                                "@ratio", ratio),
                            ClinicDatabase.CreateParameterNew(
                                "@precision",
                                precision),
                            ClinicDatabase.CreateParameterNew(
                                "@active",
                                active ? 1 : 0),
                            ClinicDatabase.CreateParameterNew(
                                "@created_by",
                                clsGlobal.CurrentUser.UserID),
                            ClinicDatabase.CreateParameterNew(
                                "@modified_by",
                                clsGlobal.CurrentUser.UserID));

                    int newId =
                        Convert.ToInt32(result);

                    row.Cells["colUnitId"].Value =
                        newId;
                }
            }
        }

        private void DeactivateRemovedUnits()
        {
            foreach (int oldUnitId
                in originalUnitIds)
            {
                bool stillExists = false;

                foreach (DataGridViewRow row
                    in dgvUnits.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    int currentId = 0;

                    if (row.Cells["colUnitId"].Value != null)
                    {
                        int.TryParse(
                            row.Cells["colUnitId"]
                            .Value.ToString(),
                            out currentId);
                    }

                    if (currentId == oldUnitId)
                    {
                        stillExists = true;
                        break;
                    }
                }

                if (stillExists)
                    continue;

                object used =
                    ClinicDatabase.ExecuteScalar(@"
                        SELECT COUNT(*)
                        FROM tbl_items
                        WHERE unit_id = @unitId
                          AND state = 0;",
                        ClinicDatabase.CreateParameterNew(
                            "@unitId",
                            oldUnitId));

                if (Convert.ToInt32(used ?? 0) > 0)
                {
                    // Keep database ID intact.
                    // Disable it instead of deleting it.
                    ClinicDatabase.ExecuteNonQuery(@"
                        UPDATE tbl_uom
                        SET
                            active = 0,
                            modified_by = @user,
                            modified_date = NOW()
                        WHERE id = @id;",
                        ClinicDatabase.CreateParameterNew(
                            "@user",
                            clsGlobal.CurrentUser.UserID),
                        ClinicDatabase.CreateParameterNew(
                            "@id",
                            oldUnitId));

                    continue;
                }

                ClinicDatabase.ExecuteNonQuery(@"
                    UPDATE tbl_uom
                    SET
                        state = 1,
                        active = 0,
                        modified_by = @user,
                        modified_date = NOW()
                    WHERE id = @id;",
                    ClinicDatabase.CreateParameterNew(
                        "@user",
                        clsGlobal.CurrentUser.UserID),
                    ClinicDatabase.CreateParameterNew(
                        "@id",
                        oldUnitId));
            }
        }
        private void EnsureUomGridColumns()
        {
            // ============================================================
            // HIDDEN DATABASE ID
            // ============================================================
            if (!dgvUnits.Columns.Contains("colUnitId"))
            {
                DataGridViewTextBoxColumn colId =
                    new DataGridViewTextBoxColumn();

                colId.Name = "colUnitId";
                colId.HeaderText = "ID";
                colId.Visible = false;

                dgvUnits.Columns.Insert(0, colId);
            }

            // ============================================================
            // UNIT NAME
            // ============================================================
            DataGridViewColumn unitColumn = null;

            foreach (DataGridViewColumn col in dgvUnits.Columns)
            {
                if (col.Name == "colUnit")
                {
                    unitColumn = col;
                    break;
                }

                if (col.HeaderText.Trim()
                    .Equals(
                        "Unit of Measure",
                        StringComparison.OrdinalIgnoreCase))
                {
                    unitColumn = col;
                    break;
                }
            }

            if (unitColumn != null)
                unitColumn.Name = "colUnit";

            // ============================================================
            // TYPE
            // ============================================================
            if (!dgvUnits.Columns.Contains("colType"))
            {
                DataGridViewComboBoxColumn colType =
                    new DataGridViewComboBoxColumn();

                colType.Name = "colType";
                colType.HeaderText = "Type";

                colType.Items.Add("Reference");
                colType.Items.Add("Bigger");
                colType.Items.Add("Smaller");

                int insertIndex =
                    dgvUnits.Columns.Contains("colUnit")
                        ? dgvUnits.Columns["colUnit"].Index + 1
                        : 1;

                dgvUnits.Columns.Insert(
                    insertIndex,
                    colType);
            }

            // ============================================================
            // RATIO
            // ============================================================
            DataGridViewColumn ratioColumn = null;

            foreach (DataGridViewColumn col in dgvUnits.Columns)
            {
                if (col.Name == "colRatio")
                {
                    ratioColumn = col;
                    break;
                }

                if (col.HeaderText.Trim()
                    .Equals(
                        "Ratio",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ratioColumn = col;
                    break;
                }
            }

            if (ratioColumn != null)
                ratioColumn.Name = "colRatio";

            // ============================================================
            // ACTIVE
            // ============================================================
            DataGridViewColumn activeColumn = null;

            foreach (DataGridViewColumn col in dgvUnits.Columns)
            {
                if (col.Name == "colUnitActive")
                {
                    activeColumn = col;
                    break;
                }

                if (col.HeaderText.Trim()
                    .Equals(
                        "Active",
                        StringComparison.OrdinalIgnoreCase))
                {
                    activeColumn = col;
                    break;
                }
            }

            if (activeColumn != null)
                activeColumn.Name = "colUnitActive";

            // ============================================================
            // ROUNDING PRECISION
            // ============================================================
            if (!dgvUnits.Columns.Contains("colPrecision"))
            {
                DataGridViewTextBoxColumn colPrecision =
                    new DataGridViewTextBoxColumn();

                colPrecision.Name = "colPrecision";
                colPrecision.HeaderText = "Rounding Precision";
                colPrecision.Visible = false;

                dgvUnits.Columns.Add(colPrecision);
            }

            // ============================================================
            // DELETE
            // ============================================================
            DataGridViewColumn deleteColumn = null;

            foreach (DataGridViewColumn col in dgvUnits.Columns)
            {
                if (col.Name == "colDelete")
                {
                    deleteColumn = col;
                    break;
                }

                if (col.HeaderText.Trim()
                    .Equals(
                        "Delete",
                        StringComparison.OrdinalIgnoreCase))
                {
                    deleteColumn = col;
                    break;
                }
            }

            if (deleteColumn != null)
                deleteColumn.Name = "colDelete";
        }
        private void dgvUnits_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex !=
                dgvUnits.Columns["colDelete"].Index)
                return;

            DataGridViewRow row =
                dgvUnits.Rows[e.RowIndex];

            string type =
                row.Cells["colType"]
                .Value?.ToString() ?? "";

            if (type == "Reference")
            {
                MessageBox.Show(
                    "Reference Unit cannot be deleted.");

                return;
            }

            int unitId = 0;

            if (row.Cells["colUnitId"].Value != null)
            {
                int.TryParse(
                    row.Cells["colUnitId"]
                    .Value.ToString(),
                    out unitId);
            }

            if (unitId > 0 &&
                IsUomUsedInTransactions(unitId))
            {
                MessageBox.Show(
                    "This Unit of Measure has already been used in a Purchase or Sales transaction.\n\n" +
                    "You cannot remove it. You can make it inactive instead.",
                    "UOM Already Used",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (unitId > 0)
            {
                object used =
                    ClinicDatabase.ExecuteScalar(@"
                SELECT COUNT(*)
                FROM tbl_items
                WHERE unit_id = @unitId
                  AND state = 0;",
                        ClinicDatabase.CreateParameterNew(
                            "@unitId",
                            unitId));

                if (Convert.ToInt32(used ?? 0) > 0)
                {
                    MessageBox.Show(
                        "This Unit of Measure is already used by an item.\n" +
                        "You cannot remove it. You can make it inactive instead.",
                        "UOM",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }
            }

            dgvUnits.Rows.RemoveAt(e.RowIndex);
        }
        private void dgvUnits_CellContentClickOld(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex !=
                dgvUnits.Columns["colDelete"].Index)
                return;

            DataGridViewRow row =
                dgvUnits.Rows[e.RowIndex];

            string type =
                row.Cells["colType"]
                .Value?.ToString() ?? "";

            if (type == "Reference")
            {
                MessageBox.Show(
                    "Reference Unit cannot be deleted.");

                return;
            }

            int unitId = 0;

            if (row.Cells["colUnitId"].Value != null)
            {
                int.TryParse(
                    row.Cells["colUnitId"]
                    .Value.ToString(),
                    out unitId);
            }

            if (unitId > 0)
            {
                object used =
                    ClinicDatabase.ExecuteScalar(@"
                        SELECT COUNT(*)
                        FROM tbl_items
                        WHERE unit_id = @unitId
                          AND state = 0;",
                        ClinicDatabase.CreateParameterNew(
                            "@unitId",
                            unitId));

                if (Convert.ToInt32(used ?? 0) > 0)
                {
                    MessageBox.Show(
                        "This Unit of Measure is already used by an item.\n" +
                        "You cannot remove it. You can make it inactive instead.",
                        "UOM",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }
            }

            dgvUnits.Rows.RemoveAt(
                e.RowIndex);
        }

        private void btnClose_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }

        private void dgvUnits_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0)
                return;

            DataGridViewRow row =
                dgvUnits.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            int unitId = 0;

            if (row.Cells["colUnitId"].Value != null)
            {
                int.TryParse(
                    row.Cells["colUnitId"].Value.ToString(),
                    out unitId);
            }

            // New UOM - allow editing
            if (unitId <= 0)
                return;

            string columnName =
                dgvUnits.Columns[e.ColumnIndex].Name;

            // These values affect historical transactions
            bool protectedColumn =
                columnName == "colUnit" ||
                columnName == "colType" ||
                columnName == "colRatio";

            if (!protectedColumn)
                return;

            if (IsUomUsedInTransactions(unitId))
            {
                e.Cancel = true;

                MessageBox.Show(
                    "This Unit of Measure has already been used in a Purchase or Sales transaction.\n\n" +
                    "Unit Name, Type and Ratio cannot be changed because it would affect historical inventory quantities.",
                    "UOM Already Used",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        private bool IsUomUsedInTransactions(int unitId)
        {
            if (unitId <= 0)
                return false;

            object result = ClinicDatabase.ExecuteScalar(@"
                    SELECT
                    (
                        (SELECT COUNT(*)
                         FROM tbl_purchase_details
                         WHERE unit_id = @unitId)

                        +

                        (SELECT COUNT(*)
                         FROM tbl_sales_details
                         WHERE unit_id = @unitId)
                    );",
                ClinicDatabase.CreateParameterNew(
                    "@unitId",
                    unitId));

            return Convert.ToInt32(result ?? 0) > 0;
        }
    }

}