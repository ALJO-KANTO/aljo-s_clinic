using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YamyProject.DAL;

namespace YamyProject.UI.Inventory.InventoryManagement.ItemUnit
{
    public partial class frmItemUOMList : Form
    {
        public frmItemUOMList()
        {
            InitializeComponent();
        }
        private void frmItemUOMList_Load(object sender, EventArgs e)
        {
            LoadItems();
        }

        // =========================================================
        // LOAD ITEMS
        // =========================================================

        private void LoadItems(string search = "")
        {
            dgvItems.Rows.Clear();

            string query = @"
                SELECT
                    i.id,
                    i.code,
                    i.name,

                    i.unit_id AS default_unit_id,

                    IFNULL(u.name, '') AS default_unit,

                    IFNULL(
                        GROUP_CONCAT(
                            DISTINCT CONCAT(
                                su.name,
                                ' (',
                                TRIM(TRAILING '.000000'
                                    FROM CAST(iu.factor AS CHAR)),
                                ')'
                            )
                            ORDER BY su.name
                            SEPARATOR ', '
                        ),
                        ''
                    ) AS alternative_units

                FROM tbl_items i

                INNER JOIN tbl_unit u
                    ON u.id = i.unit_id

                LEFT JOIN tbl_items_unit iu
                    ON iu.item_id = i.id

                LEFT JOIN tbl_unit su
                    ON su.id = iu.unit_id

                WHERE
                    i.unit_id > 0

                    AND
                    (
                        @search = ''

                        OR i.code LIKE CONCAT('%', @search, '%')

                        OR i.name LIKE CONCAT('%', @search, '%')

                        OR u.name LIKE CONCAT('%', @search, '%')
                    )

                GROUP BY
                    i.id,
                    i.code,
                    i.name,
                    i.unit_id,
                    u.name

                ORDER BY
                    i.name;";

            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    query,
                    ClinicDatabase.CreateParameter(
                        "@search",
                        search)))
            {
                int serialNo = 1;

                while (reader.Read())
                {
                    dgvItems.Rows.Add(
                        reader["id"],
                        serialNo,
                        reader["code"],
                        reader["name"],
                        reader["default_unit_id"],
                        reader["default_unit"],
                        reader["alternative_units"]
                    );

                    serialNo++;
                }
            }
        }

        // =========================================================
        // SEARCH
        // =========================================================

        private void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            LoadItems(txtSearch.Text.Trim());
        }

        // =========================================================
        // REFRESH
        // =========================================================

        private void btnRefresh_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Clear();

            LoadItems();
        }

        // =========================================================
        // UOM BUTTON
        // =========================================================

        private void dgvItems_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0)
                return;

            if (dgvItems.Columns[e.ColumnIndex].Name
                != "colUOM")
                return;

            DataGridViewRow row =
                dgvItems.Rows[e.RowIndex];

            int itemId =
                Convert.ToInt32(
                    row.Cells["colId"].Value);

            string itemCode =
                row.Cells["colCode"].Value?.ToString()
                ?? "";

            string itemName =
                row.Cells["colName"].Value?.ToString()
                ?? "";

            int defaultUnitId =
                Convert.ToInt32(
                    row.Cells["colDefaultUnitId"].Value);

            string defaultUnitName =
                row.Cells["colDefaultUnit"].Value?.ToString()
                ?? "";

            // Open UOM management
            using (frmAddUnitsToItem frm =
                new frmAddUnitsToItem(
                    itemId,
                    itemCode,
                    itemName,
                    defaultUnitId,
                    defaultUnitName))
            {
                frm.ShowDialog();
            }

            // Refresh because UOMs may have changed
            LoadItems(txtSearch.Text.Trim());
        }
    }
}