
using System;
using System.Data;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmInventoryLocations : Form
    {
        private bool IsArabic
        {
            get
            {
                string language =
                    Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

                return language.StartsWith(
                           "ar", StringComparison.OrdinalIgnoreCase)
                       || language.IndexOf(
                           "arabic", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public frmInventoryLocations()
        {
            InitializeComponent();

            btnNew.Click += btnNew_Click;
            dgvLocations.CellDoubleClick += dgvLocations_CellDoubleClick;

            ApplyInventoryLocationsLanguage();
        }

        private string T(string english, string arabic)
        {
            return IsArabic ? arabic : english;
        }

        private void ApplyInventoryLocationsLanguage()
        {
            bool isArabic = IsArabic;

            // Localize controls using the existing localization manager first.
           // LocalizationManager.LocalizeForm(this);

            // Apply these translations last to preserve the requested captions.
            btnNew.Text = T("New", "جديد");
            btnInventoryAtDate.Text = T("Inventory at", "المخزون في تاريخ");
            btnRefresh.Text = T("Refresh", "تحديث");

            dgvLocations.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            SetColumnHeader("colLocation", "Location", "الموقع");
            SetColumnHeader("colProduct", "Product", "المنتج");
            SetColumnHeader("colOnHand", "On Hand Quantity", "الكمية المتوفرة");
            SetColumnHeader("colReserved", "Reserved Quantity", "الكمية المحجوزة");
            SetColumnHeader("colUnit", "Unit", "الوحدة");

            SetColumnHeader("colHistory", "History", "السجل");
            SetColumnHeader("colAdjust", "Adjust", "تعديل");

            UpdateRecordCount();
            TranslateLocationsTitle(this);
        }

        private void SetColumnHeader(
            string columnName,
            string english,
            string arabic)
        {
            if (dgvLocations.Columns.Contains(columnName))
            {
                dgvLocations.Columns[columnName].HeaderText =
                    T(english, arabic);
            }
        }

        private void UpdateRecordCount()
        {
            lblCount.Text = IsArabic
                ? dgvLocations.Rows.Count.ToString() + " سجل"
                : dgvLocations.Rows.Count.ToString() + " records";
        }

        private void frmInventoryLocations_Load(object sender, EventArgs e)
        {
            ApplyInventoryLocationsLanguage();

            try
            {
                InventoryLocationService.EnsureSchema();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    T("Could not load Locations.\r\n\r\n",
                      "تعذر تحميل المواقع.\r\n\r\n") + ex.Message,
                    T("Locations", "المواقع"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    IsArabic
                        ? MessageBoxOptions.RightAlign |
                          MessageBoxOptions.RtlReading
                        : 0);
            }
        }

        public void LoadData()
        {
            try
            {
                DataTable dt =
                    InventoryLocationService.GetLocationStock(
                        txtSearch.Text.Trim());

                dgvLocations.Rows.Clear();

                if (dt == null)
                {
                    UpdateRecordCount();
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    int index = dgvLocations.Rows.Add();
                    DataGridViewRow gridRow = dgvLocations.Rows[index];

                    gridRow.Cells["colStockId"].Value =
                        GetInt(row["id"]);

                    gridRow.Cells["colWarehouseId"].Value =
                        GetInt(row["warehouse_id"]);

                    gridRow.Cells["colLocationId"].Value =
                        GetInt(row["location_id"]);

                    gridRow.Cells["colItemId"].Value =
                        GetInt(row["item_id"]);

                    gridRow.Cells["colLocation"].Value =
                        row["location_name"] == DBNull.Value
                            ? ""
                            : row["location_name"].ToString();

                    gridRow.Cells["colProduct"].Value =
                        row["product_name"] == DBNull.Value
                            ? ""
                            : row["product_name"].ToString();

                    gridRow.Cells["colOnHand"].Value =
                        row["on_hand"] == DBNull.Value
                            ? "0"
                            : Convert.ToDecimal(row["on_hand"])
                                .ToString("0.####");

                    gridRow.Cells["colReserved"].Value =
                        row["reserved_qty"] == DBNull.Value
                            ? "0"
                            : Convert.ToDecimal(row["reserved_qty"])
                                .ToString("0.####");

                    gridRow.Cells["colUnit"].Value =
                        row["unit_name"] == DBNull.Value
                            ? T("Units", "وحدات")
                            : row["unit_name"].ToString();
                }

                UpdateRecordCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    T("Could not refresh Locations.\r\n\r\n",
                      "تعذر تحديث المواقع.\r\n\r\n") + ex.Message,
                    T("Locations", "المواقع"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    IsArabic
                        ? MessageBoxOptions.RightAlign |
                          MessageBoxOptions.RtlReading
                        : 0);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (frmWarehouseLocation frm =
                new frmWarehouseLocation())
            {
                frm.ShowDialog(this);
            }

            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dgvLocations_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = dgvLocations.Rows[e.RowIndex];

            int warehouseId =
                GetInt(row.Cells["colWarehouseId"].Value);

            int locationId =
                GetInt(row.Cells["colLocationId"].Value);

            int itemId =
                GetInt(row.Cells["colItemId"].Value);

            if (itemId <= 0)
                return;

            string columnName =
                dgvLocations.Columns[e.ColumnIndex].Name;

            if (columnName == "colHistory")
            {
                using (frmInventoryLocationHistory frm =
                    new frmInventoryLocationHistory(
                        itemId, warehouseId, locationId))
                {
                    frm.ShowDialog(this);
                }
            }
            else if (columnName == "colOnHand" ||
                     columnName == "colAdjust")
            {
                using (frmInventoryAdjustment frm =
                    new frmInventoryAdjustment(
                        itemId, warehouseId, locationId))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                        LoadData();
                }
            }
        }

        private void dgvLocations_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int locationId =
                GetInt(
                    dgvLocations.Rows[e.RowIndex]
                        .Cells["colLocationId"].Value);

            if (locationId <= 0)
                return;

            using (frmWarehouseLocation frm =
                new frmWarehouseLocation(locationId))
            {
                frm.ShowDialog(this);
            }

            LoadData();
        }

        private void btnInventoryAtDate_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                T(
                    "This page shows current stock by warehouse location.",
                    "تعرض هذه الصفحة المخزون الحالي حسب موقع المستودع."),
                T("Inventory at Date", "المخزون في تاريخ"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                IsArabic
                    ? MessageBoxOptions.RightAlign |
                      MessageBoxOptions.RtlReading
                    : 0);
        }

        private int GetInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            int result;

            return int.TryParse(value.ToString(), out result)
                ? result
                : 0;
        }
        private void TranslateLocationsTitle(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (string.Equals(
                    control.Text?.Trim(),
                    "Locations",
                    StringComparison.OrdinalIgnoreCase))
                {
                    control.Text = IsArabic ? "المواقع" : "Locations";
                }

                if (control.HasChildren)
                {
                    TranslateLocationsTitle(control);
                }
            }
        }
    }
}
