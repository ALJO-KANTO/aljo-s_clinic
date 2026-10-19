using Clinic_DataAccess;
using System;
using System.Data;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmInventoryAdjustment : Form
    {
        private readonly int itemId;
        private readonly int warehouseId;
        private readonly int locationId;

        private decimal onHand = 0;

        public frmInventoryAdjustment(
            int itemId,
            int warehouseId,
            int locationId)
        {
            InitializeComponent();

            this.itemId = itemId;
            this.warehouseId = warehouseId;
            this.locationId = locationId;
        }

        private void frmInventoryAdjustment_Load(
            object sender,
            EventArgs e)
        {
            try
            {
                InventoryLocationService.EnsureSchema();

                LoadRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load Inventory Adjustment."
                    + "\r\n\r\n"
                    + ex.Message,
                    "Inventory Adjustment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadRecord()
        {
            DataTable dt =
                ClinicDatabase.ExecuteDataTable(
                    @"
                    SELECT
                        CONCAT(
                            IFNULL(
                                NULLIF(w.code,''),
                                w.name
                            ),
                            '/',
                            IFNULL(
                                NULLIF(l.code,''),
                                l.name
                            )
                        ) AS location_name,

                        CONCAT(
                            '[',
                            IFNULL(i.code,''),
                            '] ',
                            IFNULL(i.name,'')
                        ) AS product_name,

                        IFNULL(
                            s.on_hand,
                            0
                        ) AS on_hand

                    FROM tbl_items i

                    LEFT JOIN
                        tbl_inventory_location_stock s
                    ON
                        s.item_id = i.id
                    AND
                        s.warehouse_id =
                        @warehouseId
                    AND
                        s.location_id =
                        @locationId

                    LEFT JOIN tbl_warehouse w
                    ON
                        w.id =
                        @warehouseId

                    LEFT JOIN
                        tbl_warehouse_location l
                    ON
                        l.id =
                        @locationId

                    WHERE
                        i.id = @itemId

                    LIMIT 1;
                    ",

                   ClinicDatabase.CreateParameterNew(
                        "@warehouseId",
                        warehouseId),

                   ClinicDatabase.CreateParameterNew(
                        "@locationId",
                        locationId),

                   ClinicDatabase.CreateParameterNew(
                        "@itemId",
                        itemId)
                );

            if (dt == null ||
                dt.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Product was not found.",
                    "Inventory Adjustment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                Close();
                return;
            }

            DataRow row =
                dt.Rows[0];

            lblLocationValue.Text =
                row["location_name"] ==
                DBNull.Value
                    ? ""
                    : row[
                        "location_name"
                    ].ToString();

            lblProductValue.Text =
                row["product_name"] ==
                DBNull.Value
                    ? ""
                    : row[
                        "product_name"
                    ].ToString();

            if (row["on_hand"] ==
                DBNull.Value)
            {
                onHand = 0;
            }
            else
            {
                onHand =
                    Convert.ToDecimal(
                        row["on_hand"]);
            }

            lblOnHandValue.Text =
                onHand.ToString(
                    "0.####");

            // You requested Counted Qty
            // initially = 0.
            numCounted.Value = 0;

            UpdateDifference();
        }

        private void numCounted_ValueChanged(
            object sender,
            EventArgs e)
        {
            UpdateDifference();
        }

        private void UpdateDifference()
        {
            decimal counted =
                numCounted.Value;

            decimal difference =
                counted - onHand;

            lblDifferenceValue.Text =
                difference.ToString(
                    "0.####");

            if (difference > 0)
            {
                lblDifferenceValue.ForeColor =
                    System.Drawing.Color.Green;
            }
            else if (difference < 0)
            {
                lblDifferenceValue.ForeColor =
                    System.Drawing.Color.Firebrick;
            }
            else
            {
                lblDifferenceValue.ForeColor =
                    System.Drawing.Color.DimGray;
            }
        }

        private void btnApply_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                decimal countedQty =
                    numCounted.Value;

                decimal difference =
                    countedQty -
                    onHand;

                DialogResult answer =
                    MessageBox.Show(
                        "Apply this inventory adjustment?"
                        +
                        "\r\n\r\nOn Hand: "
                        +
                        onHand.ToString(
                            "0.####")
                        +
                        "\r\nCounted: "
                        +
                        countedQty.ToString(
                            "0.####")
                        +
                        "\r\nDifference: "
                        +
                        difference.ToString(
                            "0.####"),

                        "Inventory Adjustment",

                        MessageBoxButtons.YesNo,

                        MessageBoxIcon.Question
                    );

                if (answer !=
                    DialogResult.Yes)
                {
                    return;
                }

                InventoryLocationService
                    .ApplyInventoryAdjustment(
                        warehouseId,
                        locationId,
                        itemId,
                        countedQty,
                        -1
                    );

                MessageBox.Show(
                    "Inventory quantity updated successfully.",
                    "Inventory Adjustment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult =
                    DialogResult.OK;

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not apply inventory adjustment."
                    +
                    "\r\n\r\n"
                    +
                    ex.Message,

                    "Inventory Adjustment",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error
                );
            }
        }

        private void btnHistory_Click(
            object sender,
            EventArgs e)
        {
            using (
                frmInventoryLocationHistory frm =
                new frmInventoryLocationHistory(
                    itemId,
                    warehouseId,
                    locationId))
            {
                frm.ShowDialog(this);
            }
        }

        private void btnClear_Click(
            object sender,
            EventArgs e)
        {
            numCounted.Value = 0;
        }

        private void btnClose_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }
    }
}