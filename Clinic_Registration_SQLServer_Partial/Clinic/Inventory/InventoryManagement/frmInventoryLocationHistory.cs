using System;
using System.Data;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmInventoryLocationHistory : Form
    {
        private readonly int itemId;
        private readonly int warehouseId;
        private readonly int locationId;

        public frmInventoryLocationHistory(
            int itemId = 0,
            int warehouseId = 0,
            int locationId = 0)
        {
            InitializeComponent();

            this.itemId = itemId;
            this.warehouseId = warehouseId;
            this.locationId = locationId;
        }

        private void frmInventoryLocationHistory_Load(object sender, EventArgs e)
        {
            try
            {
                InventoryLocationService.EnsureSchema();
                LoadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load history.\r\n\r\n" + ex.Message,
                    "Moves History",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadHistory()
        {
            DataTable dt = InventoryLocationService.GetHistory(
                itemId,
                warehouseId,
                locationId);

            dgvHistory.Rows.Clear();

            if (dt == null)
            {
                lblCount.Text = "0 records";
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                int index = dgvHistory.Rows.Add();
                DataGridViewRow gridRow = dgvHistory.Rows[index];

                gridRow.Cells["colDate"].Value =
                    row["movement_date"] == DBNull.Value
                        ? ""
                        : Convert.ToDateTime(row["movement_date"])
                            .ToString("MM/dd/yyyy HH:mm:ss");

                gridRow.Cells["colReference"].Value =
                    row["reference_no"] == DBNull.Value
                        ? ""
                        : row["reference_no"].ToString();

                gridRow.Cells["colProduct"].Value =
                    row["product_name"] == DBNull.Value
                        ? ""
                        : row["product_name"].ToString();

                gridRow.Cells["colFrom"].Value =
                    row["from_location"] == DBNull.Value
                        ? ""
                        : row["from_location"].ToString();

                gridRow.Cells["colTo"].Value =
                    row["to_location"] == DBNull.Value
                        ? ""
                        : row["to_location"].ToString();

                gridRow.Cells["colQuantity"].Value =
                    row["quantity"] == DBNull.Value
                        ? "0"
                        : Convert.ToDecimal(row["quantity"]).ToString("0.####");

                gridRow.Cells["colUnit"].Value =
                    row["unit_name"] == DBNull.Value
                        ? "Units"
                        : row["unit_name"].ToString();

                gridRow.Cells["colStatus"].Value =
                    row["status"] == DBNull.Value
                        ? ""
                        : row["status"].ToString();
            }

            lblCount.Text = dgvHistory.Rows.Count + " records";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
