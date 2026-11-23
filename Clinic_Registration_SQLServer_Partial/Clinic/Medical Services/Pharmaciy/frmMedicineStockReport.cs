using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Pharmaciy
{
    public partial class frmMedicineStockReport : Form
    {
        private DataTable _reportData = new DataTable();

        private readonly PrintDocument _printDocument = new PrintDocument();
        private int _printRowIndex = 0;

        public frmMedicineStockReport()
        {
            InitializeComponent();

            InitializeReport();
            RegisterEvents();
            ApplyArabicProperties();

        }
        public void ApplyArabicProperties()
        {
            bool arabic = TranslationClass.IsArabic;

            this.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            this.RightToLeftLayout = arabic;

            ApplyTranslationToControls(this);

            ApplyLanguageSpecificLayout(arabic);

            TranslateMedicineGridHeaders();
        }
        private string T(string text)
        {
            return TranslationClass.Translate(
                text,
                TranslationClass.IsArabic);
        }
        private void ApplyTranslationToControls(Control parent)
        {
            bool arabic = TranslationClass.IsArabic;

            foreach (Control control in parent.Controls)
            {
                // Avoid translating dynamically generated grid headers here.
                if (!(control is DataGridView) &&
                    !string.IsNullOrWhiteSpace(control.Text))
                {
                    control.Text = T(control.Text);
                }

                control.RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control is Label label)
                {
                    label.TextAlign = arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }

                if (control.HasChildren)
                    ApplyTranslationToControls(control);
            }

            dgvMedicines.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            dgvMedicines.ColumnHeadersDefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;

            dgvMedicines.DefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;

            dgvMedicines.RowHeadersDefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;

            txtMedicine.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;
        }
        private void ApplyLanguageSpecificLayout(bool arabic)
        {
            if (arabic)
            {
                // Header
                lblTitle.AutoSize = false;
                lblTitle.Location = new Point(0, 10);
                lblTitle.Size = new Size(
                    pnlHeader.ClientSize.Width, 45);

                lblTitle.TextAlign = ContentAlignment.MiddleCenter;

                lblTitle.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right;

                // Right-to-left filters
                txtMedicine.TextAlign = HorizontalAlignment.Right;

                // Filter labels
                lblFromDate.TextAlign = ContentAlignment.MiddleRight;
                lblToDate.TextAlign = ContentAlignment.MiddleRight;
                lblMedicine.TextAlign = ContentAlignment.MiddleRight;
                lblWarehouse.TextAlign = ContentAlignment.MiddleRight;
                lblStockStatus.TextAlign = ContentAlignment.MiddleRight;
                lblExpiryStatus.TextAlign = ContentAlignment.MiddleRight;

                // Summary cards
                lblTotalMedicinesTitle.TextAlign = ContentAlignment.MiddleRight;
                lblStockValueTitle.TextAlign = ContentAlignment.MiddleRight;
                lblLowStockTitle.TextAlign = ContentAlignment.MiddleRight;
                lblExpiredTitle.TextAlign = ContentAlignment.MiddleRight;

                // Data grid
                dgvMedicines.RightToLeft = RightToLeft.Yes;
            }
            else
            {
                // Restore original English layout
                lblTitle.AutoSize = true;
                lblTitle.Location = new Point(25, 15);
                lblTitle.Size = new Size(290, 31);
                lblTitle.TextAlign = ContentAlignment.MiddleLeft;

                lblTitle.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left;

                txtMedicine.TextAlign = HorizontalAlignment.Left;

                lblFromDate.TextAlign = ContentAlignment.MiddleLeft;
                lblToDate.TextAlign = ContentAlignment.MiddleLeft;
                lblMedicine.TextAlign = ContentAlignment.MiddleLeft;
                lblWarehouse.TextAlign = ContentAlignment.MiddleLeft;
                lblStockStatus.TextAlign = ContentAlignment.MiddleLeft;
                lblExpiryStatus.TextAlign = ContentAlignment.MiddleLeft;

                lblTotalMedicinesTitle.TextAlign = ContentAlignment.MiddleLeft;
                lblStockValueTitle.TextAlign = ContentAlignment.MiddleLeft;
                lblLowStockTitle.TextAlign = ContentAlignment.MiddleLeft;
                lblExpiredTitle.TextAlign = ContentAlignment.MiddleLeft;

                dgvMedicines.RightToLeft = RightToLeft.No;
            }
        }
        private void TranslateMedicineGridHeaders()
        {
            foreach (DataGridViewColumn column in dgvMedicines.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                {
                    column.HeaderText = T(column.HeaderText);
                }
            }
        }
        // =========================================================
        // INITIALIZE
        // =========================================================

        private void InitializeReport()
        {
            dtpFromDate.Value = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            dtpToDate.Value = DateTime.Today;

            cmbStockStatus.SelectedIndex = 0;
            cmbExpiryStatus.SelectedIndex = 0;

            LoadWarehouses();

            ConfigureGrid();

            _printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void RegisterEvents()
        {
            btnGenerate.Click += btnGenerate_Click;
            btnReset.Click += btnReset_Click;
            btnExportExcel.Click += btnExportExcel_Click;
            btnPrint.Click += btnPrint_Click;

            this.Load += frmMedicineStockReport_Load;
        }

        private void frmMedicineStockReport_Load(object sender, EventArgs e)
        {
            GenerateReport();
        }

        // =========================================================
        // WAREHOUSE FILTER
        // =========================================================

        private void LoadWarehouses()
        {
            cmbWarehouse.Items.Clear();

            cmbWarehouse.DisplayMember = "Value";
            cmbWarehouse.ValueMember = "Key";

            cmbWarehouse.Items.Add(
                new System.Collections.Generic.KeyValuePair<int, string>(
                    0, "All Warehouses"));

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                        SELECT DISTINCT warehouse_id
                        FROM tbl_item_transaction
                        WHERE warehouse_id IS NOT NULL
                          AND warehouse_id > 0
                        ORDER BY warehouse_id;";

                    using (MySqlCommand cmd =
                        new MySqlCommand(query, connection))
                    {
                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["warehouse_id"]);

                                cmbWarehouse.Items.Add(
                                    new System.Collections.Generic.KeyValuePair<int, string>(
                                        id,
                                        "Warehouse " + id));
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "Unable to load warehouses.\n" + ex.Message,
                    "Warehouse",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            cmbWarehouse.SelectedIndex = 0;
        }

        private int GetSelectedWarehouseId()
        {
            if (cmbWarehouse.SelectedItem == null)
                return 0;

            var selected =
                (System.Collections.Generic.KeyValuePair<int, string>)
                cmbWarehouse.SelectedItem;

            return selected.Key;
        }

        // =========================================================
        // DATAGRIDVIEW
        // =========================================================

        private void ConfigureGrid()
        {
            dgvMedicines.AutoGenerateColumns = true;

            dgvMedicines.AllowUserToAddRows = false;
            dgvMedicines.AllowUserToDeleteRows = false;

            dgvMedicines.ReadOnly = true;

            dgvMedicines.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvMedicines.MultiSelect = false;

            dgvMedicines.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvMedicines.RowTemplate.Height = 35;

            dgvMedicines.EnableHeadersVisualStyles = false;

            dgvMedicines.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(17, 35, 68);

            dgvMedicines.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvMedicines.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvMedicines.DefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F);

            dgvMedicines.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(215, 230, 248);

            dgvMedicines.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dgvMedicines.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 248, 252);
        }

        private void FormatGrid()
        {
            if (dgvMedicines.Columns.Count == 0)
                return;

            bool isArabic = TranslationClass.IsArabic;
            SetColumnHeader("MedicineID", 
                isArabic ? "معرّف" : "ID");
            SetColumnHeader("MedicineName", 
                isArabic ? "اسم الدواء"
                : "Medicine Name");
            SetColumnHeader("MedicinePrice", 
                isArabic ? "سعر البيع"
                : "Selling Price");
            SetColumnHeader("MedicineCost", 
                isArabic ? "سعر التكلفة"
                : "Cost Price");
            SetColumnHeader("CurrentStock", 
                isArabic ? "المخزون الحالي"
                : "Current Stock");
            SetColumnHeader("OpeningStock", 
                isArabic ? "المخزون الافتتاحي"
                : "Opening Stock");
            SetColumnHeader("StockIn", 
                isArabic ? "المخزون الداخل"
                : "Stock In");
            SetColumnHeader("StockOut", 
                isArabic ? "المخزون الخارج"
                : "Stock Out");
            SetColumnHeader("ClosingStock", 
                isArabic ? "المخزون الختامي"
                : "Closing Stock");
            SetColumnHeader("StockValue", 
                isArabic ? "قيمة المخزون"
                : "Stock Value");
            SetColumnHeader("ReorderLevel", 
                isArabic ? "مستوى إعادة الطلب"
                : "Reorder Level");
            SetColumnHeader("ExpiredBatches", 
                isArabic ? "الدفعات منتهية الصلاحية"
                : "Expired Batches");
            SetColumnHeader("ExpiringSoonBatches", 
                isArabic ? "الدفعات على وشك الانتهاء"
                : "Expiring Soon");
            SetColumnHeader("TaxRate", 
                isArabic ? "نسبة الضريبة"
                : "Tax %");

            SetNumberFormat("MedicinePrice", "N2");
            SetNumberFormat("MedicineCost", "N2");
            SetNumberFormat("CurrentStock", "N3");
            SetNumberFormat("OpeningStock", "N3");
            SetNumberFormat("StockIn", "N3");
            SetNumberFormat("StockOut", "N3");
            SetNumberFormat("ClosingStock", "N3");
            SetNumberFormat("StockValue", "N2");
            SetNumberFormat("ReorderLevel", "N3");
            SetNumberFormat("TaxRate", "N2");
        }

        private void SetColumnHeader(string column, string header)
        {
            if (dgvMedicines.Columns.Contains(column))
                dgvMedicines.Columns[column].HeaderText = header;
        }

        private void SetNumberFormat(string column, string format)
        {
            if (dgvMedicines.Columns.Contains(column))
            {
                dgvMedicines.Columns[column].DefaultCellStyle.Format =
                    format;

                dgvMedicines.Columns[column].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
        }

        // =========================================================
        // GENERATE REPORT
        // =========================================================

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            if (dtpFromDate.Value.Date > dtpToDate.Value.Date)
            {
                MessageBox.Show(
                    "From Date cannot be greater than To Date.",
                    "Invalid Date",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                int warehouseId = GetSelectedWarehouseId();

                string medicineSearch = txtMedicine.Text.Trim();

                string stockStatus =
                    cmbStockStatus.SelectedItem?.ToString() ?? "All";

                string expiryStatus =
                    cmbExpiryStatus.SelectedItem?.ToString() ?? "All";

                string query = @"

                    SELECT

                        m.MedicineID,
                        m.MedicineName,
                        m.MedicinePrice,
                        m.MedicineCost,
                        m.CurrentStock,

                        COALESCE(t.OpeningStock, 0) AS OpeningStock,

                        COALESCE(t.StockIn, 0) AS StockIn,

                        COALESCE(t.StockOut, 0) AS StockOut,

                        (
                            COALESCE(t.OpeningStock, 0)
                            + COALESCE(t.StockIn, 0)
                            - COALESCE(t.StockOut, 0)
                        ) AS ClosingStock,

                        (
                            COALESCE(m.CurrentStock, 0)
                            * COALESCE(m.MedicineCost, 0)
                        ) AS StockValue,

                        m.ReorderLevel,
                        m.TaxRate,

                        COALESCE(b.ExpiredBatches, 0) AS ExpiredBatches,

                        COALESCE(b.ExpiringSoonBatches, 0)
                            AS ExpiringSoonBatches

                    FROM medicines m

                    LEFT JOIN
                    (
                        SELECT

                            item_id,

                            SUM(
                                CASE
                                    WHEN `date` < @DateFrom
                                    THEN COALESCE(qty_in, 0)
                                       - COALESCE(qty_out, 0)
                                    ELSE 0
                                END
                            ) AS OpeningStock,

                            SUM(
                                CASE
                                    WHEN `date` >= @DateFrom
                                     AND `date` < @DateToExclusive
                                    THEN COALESCE(qty_in, 0)
                                    ELSE 0
                                END
                            ) AS StockIn,

                            SUM(
                                CASE
                                    WHEN `date` >= @DateFrom
                                     AND `date` < @DateToExclusive
                                    THEN COALESCE(qty_out, 0)
                                    ELSE 0
                                END
                            ) AS StockOut

                        FROM tbl_item_transaction

                        WHERE
                            (@WarehouseId = 0
                             OR warehouse_id = @WarehouseId)

                        GROUP BY item_id

                    ) t ON t.item_id = m.MedicineID

                    LEFT JOIN
                    (
                        SELECT

                            item_id,

                            COUNT(
                                CASE
                                    WHEN expiry_date < CURDATE()
                                     AND qty_in > qty_out
                                    THEN 1
                                END
                            ) AS ExpiredBatches,

                            COUNT(
                                CASE
                                    WHEN expiry_date >= CURDATE()
                                     AND expiry_date <= @ExpiryLimit
                                     AND qty_in > qty_out
                                    THEN 1
                                END
                            ) AS ExpiringSoonBatches

                        FROM tbl_item_batches

                        WHERE
                            (@WarehouseId = 0
                             OR warehouse_id = @WarehouseId)

                        GROUP BY item_id

                    ) b ON b.item_id = m.MedicineID

                    WHERE
                        1 = 1

                        AND (
                            @MedicineSearch = ''
                            OR m.MedicineName LIKE @SearchPattern
                            OR CAST(m.MedicineID AS CHAR) = @MedicineSearch
                        )

                        AND
                        (
                            @StockStatus = 'All'

                            OR (
                                @StockStatus = 'In Stock'
                                AND m.CurrentStock > 0
                            )

                            OR (
                                @StockStatus = 'Low Stock'
                                AND m.ReorderLevel > 0
                                AND m.CurrentStock > 0
                                AND m.CurrentStock <= m.ReorderLevel
                            )

                            OR (
                                @StockStatus = 'Out of Stock'
                                AND m.CurrentStock <= 0
                            )
                        )

                        AND
                        (
                            @ExpiryStatus = 'All'

                            OR (
                                @ExpiryStatus = 'Expired'
                                AND COALESCE(b.ExpiredBatches, 0) > 0
                            )

                            OR (
                                @ExpiryStatus = 'Expiring Soon'
                                AND COALESCE(b.ExpiringSoonBatches, 0) > 0
                            )

                            OR (
                                @ExpiryStatus = 'Valid'
                                AND COALESCE(b.ExpiredBatches, 0) = 0
                                AND COALESCE(b.ExpiringSoonBatches, 0) = 0
                            )
                        )

                    ORDER BY m.MedicineName;";

                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (MySqlCommand cmd =
                        new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DateFrom",
                            dtpFromDate.Value.Date);

                        cmd.Parameters.AddWithValue(
                            "@DateToExclusive",
                            dtpToDate.Value.Date.AddDays(1));

                        cmd.Parameters.AddWithValue(
                            "@WarehouseId",
                            warehouseId);

                        cmd.Parameters.AddWithValue(
                            "@MedicineSearch",
                            medicineSearch);

                        cmd.Parameters.AddWithValue(
                            "@SearchPattern",
                            "%" + medicineSearch + "%");

                        cmd.Parameters.AddWithValue(
                            "@StockStatus",
                            stockStatus);

                        cmd.Parameters.AddWithValue(
                            "@ExpiryStatus",
                            expiryStatus);

                        cmd.Parameters.AddWithValue(
                            "@ExpiryLimit",
                            DateTime.Today.AddDays(90));

                        using (MySqlDataAdapter adapter =
                            new MySqlDataAdapter(cmd))
                        {
                            _reportData = new DataTable();

                            adapter.Fill(_reportData);
                        }
                    }
                }

                dgvMedicines.DataSource = _reportData;

                FormatGrid();

                UpdateSummary();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "Unable to generate medicine stock report.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // SUMMARY CARDS
        // =========================================================

        private void UpdateSummary()
        {
            int totalMedicines = _reportData.Rows.Count;

            decimal stockValue = 0;

            int lowStock = 0;

            int expired = 0;

            foreach (DataRow row in _reportData.Rows)
            {
                decimal value = Convert.ToDecimal(row["StockValue"]);

                stockValue += value;

                decimal currentStock =
                    Convert.ToDecimal(row["CurrentStock"]);

                decimal reorderLevel =
                    Convert.ToDecimal(row["ReorderLevel"]);

                if (reorderLevel > 0 &&
                    currentStock > 0 &&
                    currentStock <= reorderLevel)
                {
                    lowStock++;
                }

                int expiredBatches =
                    Convert.ToInt32(row["ExpiredBatches"]);

                if (expiredBatches > 0)
                    expired++;
            }

            lblTotalMedicines.Text =
                totalMedicines.ToString("N0");

            lblStockValue.Text =
                stockValue.ToString("N2");

            lblLowStock.Text =
                lowStock.ToString("N0");

            lblExpired.Text =
                expired.ToString("N0");
        }

        // =========================================================
        // RESET FILTERS
        // =========================================================

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtMedicine.Clear();

            dtpFromDate.Value = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            dtpToDate.Value = DateTime.Today;

            cmbStockStatus.SelectedIndex = 0;

            cmbExpiryStatus.SelectedIndex = 0;

            if (cmbWarehouse.Items.Count > 0)
                cmbWarehouse.SelectedIndex = 0;

            GenerateReport();
        }

        // =========================================================
        // EXPORT TO EXCEL COMPATIBLE CSV
        // =========================================================

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (_reportData.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No data available to export.",
                    "Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV Files (*.csv)|*.csv";

                dialog.FileName =
                    "MedicineStockReport_" +
                    DateTime.Now.ToString("yyyyMMdd_HHmm") +
                    ".csv";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    using (StreamWriter writer =
                        new StreamWriter(
                            dialog.FileName,
                            false,
                            new UTF8Encoding(true)))
                    {
                        writer.WriteLine(
                            "Medicine Stock Report");

                        writer.WriteLine(
                            "From Date," +
                            dtpFromDate.Value.ToString("dd/MM/yyyy"));

                        writer.WriteLine(
                            "To Date," +
                            dtpToDate.Value.ToString("dd/MM/yyyy"));

                        writer.WriteLine();

                        for (int i = 0; i < _reportData.Columns.Count; i++)
                        {
                            writer.Write(
                                EscapeCsv(_reportData.Columns[i].ColumnName));

                            if (i < _reportData.Columns.Count - 1)
                                writer.Write(",");
                        }

                        writer.WriteLine();

                        foreach (DataRow row in _reportData.Rows)
                        {
                            for (int i = 0; i < _reportData.Columns.Count; i++)
                            {
                                writer.Write(
                                    EscapeCsv(row[i]?.ToString() ?? ""));

                                if (i < _reportData.Columns.Count - 1)
                                    writer.Write(",");
                            }

                            writer.WriteLine();
                        }
                    }

                    MessageBox.Show(
                        "Report exported successfully.",
                        "Export",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Export Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private string EscapeCsv(string value)
        {
            return "\"" +
                value.Replace("\"", "\"\"") +
                "\"";
        }

        // =========================================================
        // PRINT PREVIEW
        // =========================================================

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_reportData.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No data available to print.",
                    "Print",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            try
            {
                _printRowIndex = 0;

                using (PrintPreviewDialog preview =
                    new PrintPreviewDialog())
                {
                    preview.Document = _printDocument;

                    preview.Width = 1100;
                    preview.Height = 750;

                    preview.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Print Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(
            object sender,
            PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            float left = e.MarginBounds.Left;

            float top = e.MarginBounds.Top;

            float width = e.MarginBounds.Width;

            using (Font titleFont =
                new Font("Arial", 16, FontStyle.Bold))
            using (Font headerFont =
                new Font("Arial", 8, FontStyle.Bold))
            using (Font rowFont =
                new Font("Arial", 8))
            using (Brush headerBrush =
                new SolidBrush(Color.FromArgb(17, 35, 68)))
            using (Brush whiteBrush =
                new SolidBrush(Color.White))
            using (Pen linePen =
                new Pen(Color.LightGray))
            {
                g.DrawString(
                    "MEDICINE STOCK REPORT",
                    titleFont,
                    Brushes.Black,
                    left,
                    top);

                top += 35;

                g.DrawString(
                    "From: " + dtpFromDate.Value.ToString("dd/MM/yyyy") +
                    "     To: " + dtpToDate.Value.ToString("dd/MM/yyyy"),
                    rowFont,
                    Brushes.Black,
                    left,
                    top);

                top += 30;

                string[] columns =
                {
                    "ID",
                    "Medicine Name",
                    "Opening",
                    "Stock In",
                    "Stock Out",
                    "Closing",
                    "Current",
                    "Value"
                };

                float[] widths =
                {
                    width * 0.07f,
                    width * 0.25f,
                    width * 0.11f,
                    width * 0.11f,
                    width * 0.11f,
                    width * 0.11f,
                    width * 0.11f,
                    width * 0.13f
                };

                float x = left;

                float headerHeight = 27;

                g.FillRectangle(
                    headerBrush,
                    left,
                    top,
                    width,
                    headerHeight);

                for (int i = 0; i < columns.Length; i++)
                {
                    RectangleF rect = new RectangleF(
                        x + 3,
                        top + 5,
                        widths[i] - 6,
                        headerHeight);

                    g.DrawString(
                        columns[i],
                        headerFont,
                        whiteBrush,
                        rect);

                    x += widths[i];
                }

                top += headerHeight;

                while (_printRowIndex < _reportData.Rows.Count)
                {
                    if (top + 25 > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    DataRow row =
                        _reportData.Rows[_printRowIndex];

                    string[] values =
                    {
                        row["MedicineID"].ToString(),
                        row["MedicineName"].ToString(),
                        Convert.ToDecimal(row["OpeningStock"]).ToString("N2"),
                        Convert.ToDecimal(row["StockIn"]).ToString("N2"),
                        Convert.ToDecimal(row["StockOut"]).ToString("N2"),
                        Convert.ToDecimal(row["ClosingStock"]).ToString("N2"),
                        Convert.ToDecimal(row["CurrentStock"]).ToString("N2"),
                        Convert.ToDecimal(row["StockValue"]).ToString("N2")
                    };

                    x = left;

                    for (int i = 0; i < values.Length; i++)
                    {
                        RectangleF rect = new RectangleF(
                            x + 3,
                            top + 4,
                            widths[i] - 6,
                            22);

                        g.DrawString(
                            values[i],
                            rowFont,
                            Brushes.Black,
                            rect);

                        x += widths[i];
                    }

                    top += 25;

                    g.DrawLine(
                        linePen,
                        left,
                        top,
                        left + width,
                        top);

                    _printRowIndex++;
                }

                e.HasMorePages = false;
            }
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
