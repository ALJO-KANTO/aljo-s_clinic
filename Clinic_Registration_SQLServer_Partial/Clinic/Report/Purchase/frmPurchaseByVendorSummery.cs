using Clinic;
using Clinic.global_classes;
using Clinic_DataAccess;
using Guna.UI2.WinForms;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YamyProject.Localization;
using Excel = Microsoft.Office.Interop.Excel;

namespace YamyProject
{
    public partial class frmPurchaseByVendorSummary : Form
    {
        bool isSubcontractors;
        public frmPurchaseByVendorSummary(bool isSubcontractors = false)
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            this.isSubcontractors = isSubcontractors;
            headerUC1.FormText = isSubcontractors ? "Purchase By Subcontractor Summary" : this.Text;
        }

        private void frmPurchaseByVendorSummary_Load(object sender, EventArgs e)
        {
            btnPrint.Text = "Print ▼";
            if (isSubcontractors)
            {
                guna2HtmlLabel7.Text = "Purchase By Subcontractor Summary";
            }
            LoadPurchaseData();
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT name FROM tbl_company LIMIT 1"))
                if (reader.Read())
                {
                    lblCompany.Text = reader["name"].ToString();
                }
            lblDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            lblTime.Text = DateTime.Now.ToString("hh:mm:tt");
            ApplyArabicReportUI();
        }
        // UI localization only. No database, filtering, totals, navigation or export logic changes.
        private void ApplyArabicReportUI()
        {
            bool arabic = (Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en")
                .StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            headerUC1.ArabicLayout = arabic;
            if (!arabic) return;

            string title = isSubcontractors
                ? "ملخص المشتريات حسب المقاول الفرعي"
                : "ملخص المشتريات حسب المورد";
            this.Text = title;
            headerUC1.FormText = title;
            guna2HtmlLabel7.Text = title;
            btnPrint.Text = "طباعة ▼";

            TranslateReportControls(this);
            TranslateExportMenu(contextMenuExport.Items);

            dgvPurchase.RightToLeft = RightToLeft.Yes;
            dgvPurchase.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            foreach (DataGridViewColumn column in dgvPurchase.Columns)
            {
                if (column.Name == "sn") column.HeaderText = "الرقم";
                else if (column.Name == "name") column.HeaderText = "الاسم";
                else if (column.Name == "debit") column.HeaderText = "صافي المشتريات";
            }
            // Keep the debit numeric values right-aligned, as before.
            if (dgvPurchase.Columns.Contains("debit"))
                dgvPurchase.Columns["debit"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

            // Do NOT translate row values such as TOTAL: PDF and Excel exports check them.
        }

        private void TranslateReportControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // Do not modify HeaderUC's own buttons, title, or its positioning.
                if (control == headerUC1) continue;
                if (control is Label || control is Button || control is CheckBox ||
                    control is GroupBox || control is Guna2HtmlLabel)
                {
                    string original = control.Text;
                    if (!string.IsNullOrWhiteSpace(original))
                    {
                        string translated = Clinic.TranslationClass.Translate(original.Trim(), true);
                        control.Text = translated;
                    }
                    if (control is Label)
                        ((Label)control).TextAlign = ContentAlignment.MiddleRight;
                    if (control is Guna2HtmlLabel)
                        ((Guna2HtmlLabel)control).TextAlignment = ContentAlignment.MiddleRight;
                }
                if (control.HasChildren) TranslateReportControls(control);
            }
        }

        private void TranslateExportMenu(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.Text))
                    item.Text = Clinic.TranslationClass.Translate(item.Text.Trim(), true);
                ToolStripDropDownItem dropDown = item as ToolStripDropDownItem;
                if (dropDown != null) TranslateExportMenu(dropDown.DropDownItems);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            contextMenuExport.Show(btnPrint, new Point(0, btnPrint.Height));
        }

        private void Report_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exporting Report...");
        }
        private void LoadPurchaseData()
        {
            dgvPurchase.Rows.Clear();

            if (dgvPurchase.Columns.Count == 0)
            {
                dgvPurchase.Columns.Add("id", "id");
                dgvPurchase.Columns["id"].Visible = false;
                dgvPurchase.Columns.Add("sn", "SN");
                dgvPurchase.Columns.Add("name", "Name");
                dgvPurchase.Columns.Add("debit", "Net Purchase");
                LocalizationManager.LocalizeDataGridViewHeaders(dgvPurchase);

                dgvPurchase.Columns["debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                foreach (DataGridViewColumn col in dgvPurchase.Columns)
                {
                    col.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular);
                }

                dgvPurchase.Columns["debit"].DefaultCellStyle.Format = "N2";
                dgvPurchase.CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical;
                dgvPurchase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvPurchase.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                dgvPurchase.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                dgvPurchase.EnableHeadersVisualStyles = false;
                dgvPurchase.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvPurchase.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                dgvPurchase.GridColor = System.Drawing.Color.LightGray;
                dgvPurchase.BorderStyle = System.Windows.Forms.BorderStyle.None;
                dgvPurchase.RowHeadersVisible = false;
            }

            decimal totalAmount = 0;

            string query = @"
                            SELECT 
                            ROW_NUMBER() OVER(ORDER BY c.id) AS SN,
                            c.id AS vendor_id,
                            c.name AS vendor_name,
                            SUM(s.net) AS `Total Purchase`
                            FROM 
                            tbl_purchase s
                            JOIN 
                            tbl_vendor c ON s.vendor_id = c.id
                            and s.state = 0
                            AND s.date >= @startDate AND s.date<= @endDate
                            WHERE c.type = 'Vendor'
                            GROUP BY 
                            c.id, c.name
                            ORDER BY 
                            c.id;";
            if (isSubcontractors)
            {
                // Modify query for subcontractors
                query = @"
                            SELECT 
                            ROW_NUMBER() OVER(ORDER BY c.id) AS SN,
                            c.id AS vendor_id,
                            c.name AS vendor_name,
                            SUM(s.net) AS `Total Purchase`
                            FROM 
                            tbl_purchase s
                            JOIN 
                            tbl_vendor c ON s.vendor_id = c.id
                            and s.state = 0
                            AND s.date >= @startDate AND s.date<= @endDate
                            WHERE c.type = 'Subcontractor'
                            GROUP BY 
                            c.id, c.name
                            ORDER BY 
                            c.id;";
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query,
                ClinicDatabase.CreateParameterNew("startDate", dtpFrom.Value.Date),
                ClinicDatabase.CreateParameterNew("endDate", dtpTo.Value.Date)))
            {
                while (reader.Read())
                {
                    int sn = Convert.ToInt32(reader["SN"]);
                    string name = reader["vendor_name"].ToString();
                    decimal netPurchase = Convert.ToDecimal(reader["Total Purchase"]);
                    int rowIndex = dgvPurchase.Rows.Add(reader["vendor_id"].ToString(), sn, name, netPurchase.ToString("N2"), "0.00", "0.00");

                    dgvPurchase.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    dgvPurchase.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular);

                    totalAmount += netPurchase;
                }
            }

            // Add total row
            int totalRow = dgvPurchase.Rows.Add("", "", "TOTAL", totalAmount.ToString("N2"), "", "");
            dgvPurchase.Rows[totalRow].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold | FontStyle.Underline);
            dgvPurchase.Rows[totalRow].DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
            dgvPurchase.Rows[totalRow].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvPurchase.Rows[totalRow].Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
        private void SavePDF_Click(object sender, EventArgs e)
        {
            string companyName = "Company Name";

            // Get company name from DB
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT name FROM tbl_company LIMIT 1"))
            {
                if (reader.Read())
                    companyName = reader["name"].ToString();
            }

            // Create PDF document
            Document doc = new Document();
            Section section = doc.AddSection();

            // Adjust margins
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);

            // Header table
            Table headerTable = section.AddTable();
            headerTable.Borders.Width = 0;
            headerTable.AddColumn("5cm");
            headerTable.AddColumn("8cm");
            headerTable.AddColumn("5cm");

            Row headerRow = headerTable.AddRow();
            Paragraph left = headerRow.Cells[0].AddParagraph();
            left.Format.Font.Name = "Times New Roman";
            left.Format.Font.Size = 10;
            left.Format.Alignment = ParagraphAlignment.Left;
            left.AddText(DateTime.Now.ToString("hh:mm tt") + "\n");
            left.AddText(DateTime.Now.ToString("dd/MM/yyyy"));

            Paragraph center = headerRow.Cells[1].AddParagraph();
            center.Format.Font.Name = "Times New Roman";
            center.Format.Alignment = ParagraphAlignment.Center;
            center.AddFormattedText(companyName.ToUpper() + "\n", TextFormat.Bold).Font.Size = 12;
            center.AddFormattedText("Purchase By Vendor Summary\n", TextFormat.Bold).Font.Size = 12;
            center.AddFormattedText("All Transactions", TextFormat.NotBold).Font.Size = 9;

            //section.Add(headerTable); // ✅ Add header

            // Separator
            Paragraph line = section.AddParagraph();
            line.Format.Borders.Bottom.Width = 2;
            line.Format.SpaceAfter = "0.5cm";

            // Main table
            Table dataTable = section.AddTable();
            dataTable.Borders.Width = 0.75;
            dataTable.Format.Font.Name = "Times New Roman";
            dataTable.Format.Font.Size = 9;

            dataTable.AddColumn("2cm"); // SN
            dataTable.AddColumn("11cm"); // Name
            dataTable.AddColumn("6cm");   // Net Purchase

            Row header = dataTable.AddRow();
            header.Shading.Color = Colors.LightGray;
            header.Format.Font.Bold = true;
            header.Cells[0].AddParagraph("SN");
            header.Cells[1].AddParagraph("Name");
            header.Cells[2].AddParagraph("Net Purchase");

            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dgvPurchase.Rows)
            {
                if (row.IsNewRow) continue;

                string sn = row.Cells["sn"].Value?.ToString() ?? "";
                string name = row.Cells["name"].Value?.ToString() ?? "";
                string purchaseStr = row.Cells["debit"].Value?.ToString() ?? "0";

                if (name.ToUpper().Contains("TOTAL")) continue;

                decimal amount;
                decimal.TryParse(purchaseStr, out amount);
                totalAmount += amount;

                Row tRow = dataTable.AddRow();
                tRow.Cells[0].AddParagraph(sn);
                tRow.Cells[1].AddParagraph(name);
                tRow.Cells[2].AddParagraph(amount.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
            }

            Row totalRow = dataTable.AddRow();
            totalRow.Cells[0].MergeRight = 1;
            totalRow.Cells[0].AddParagraph("TOTAL").Format.Font.Bold = true;

            Paragraph totalText = totalRow.Cells[2].AddParagraph();
            totalText.Format.Alignment = ParagraphAlignment.Right;
            totalText.AddFormattedText(totalAmount.ToString("N2"), TextFormat.Bold).Font.Underline = Underline.Single;

            //section.Add(dataTable); // ✅ Add main table only once

            // Let user choose path
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveDialog.Title = "Save Vendor Summary PDF";
                saveDialog.FileName = "PurchaseByVendorSummary.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                    renderer.Document = doc;
                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(saveDialog.FileName);
                    Process.Start("explorer.exe", saveDialog.FileName);
                }
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = "PurchaseByVendorBalanceSummary.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Purchase By Vendor Balance";

                    // Merge and format header row (Date)
                    Excel.Range headerRange0 = worksheet.Range["A1", "D1"];
                    headerRange0.Merge();
                    headerRange0.Value = lblCompany.Text;
                    headerRange0.Font.Bold = true;
                    headerRange0.Font.Name = "Times New Roman";
                    headerRange0.Font.Size = 12;
                    headerRange0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    Excel.Range headerRange1 = worksheet.Range["A2", "D2"];
                    headerRange1.Merge();
                    headerRange1.Value = guna2HtmlLabel7.Text;
                    headerRange1.Font.Bold = true;
                    headerRange1.Font.Name = "Times New Roman";
                    headerRange1.Font.Size = 11;
                    headerRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    Excel.Range headerRange2 = worksheet.Range["A3", "D3"];
                    headerRange2.Merge();
                    headerRange2.Value = "From " + dtpFrom.Text + " - To " + dtpTo.Text;
                    headerRange2.Font.Bold = false;
                    headerRange2.Font.Name = "Times New Roman";
                    headerRange2.Font.Size = 9;
                    headerRange2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    // Merge and format header row (Date)
                    Excel.Range headerRange3 = worksheet.Range["A4", "B4"];
                    headerRange3.Merge();
                    headerRange3.Value = DateTime.Now.ToString("MMM dd, yy");
                    headerRange3.Font.Bold = false;
                    headerRange3.Font.Name = "Times New Roman";
                    headerRange3.Font.Size = 9;
                    headerRange3.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    int rowIndex = 5;

                    // Add column headers (row 2)
                    worksheet.Cells[rowIndex, 1] = "SN";
                    worksheet.Cells[rowIndex, 2] = "Name";
                    worksheet.Cells[rowIndex, 3] = "Net Purchase";

                    for (int i = 1; i <= 3; i++)
                    {
                        var headerCell = worksheet.Cells[2, i];
                        headerCell.Font.Bold = true;
                        headerCell.Font.Name = "Times New Roman";
                        headerCell.Font.Size = 10;
                        headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    }

                    decimal totalAmount = 0;

                    foreach (DataGridViewRow row in dgvPurchase.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string sn = row.Cells["sn"].Value?.ToString() ?? "";
                        string name = row.Cells["name"].Value?.ToString() ?? "";
                        string netPurchaseStr = row.Cells["debit"].Value?.ToString() ?? "0";

                        if (name.ToUpper().Contains("TOTAL"))
                            continue;

                        decimal netPurchase = 0;
                        decimal.TryParse(netPurchaseStr, out netPurchase);
                        totalAmount += netPurchase;

                        worksheet.Cells[rowIndex, 1] = sn;
                        worksheet.Cells[rowIndex, 2] = name;
                        worksheet.Cells[rowIndex, 3] = netPurchase.ToString("N2");

                        for (int col = 1; col <= 3; col++)
                        {
                            var cell = worksheet.Cells[rowIndex, col];
                            cell.Font.Name = "Times New Roman";
                            cell.Font.Size = 9;
                            if (col == 3)
                                cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        }

                        rowIndex++;
                    }

                    // Add TOTAL row
                    worksheet.Cells[rowIndex, 1] = "";
                    worksheet.Cells[rowIndex, 2] = "TOTAL";
                    worksheet.Cells[rowIndex, 3] = totalAmount.ToString("N2");

                    worksheet.Cells[rowIndex, 2].Font.Bold = true;
                    worksheet.Cells[rowIndex, 3].Font.Bold = true;
                    worksheet.Cells[rowIndex, 3].Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                    worksheet.Cells[rowIndex, 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    worksheet.Columns.AutoFit();
                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close(false);
                    excelApp.Quit();

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void dgvPurchase_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (Utilities.UserPermissionCheck("CustomerBalanceSummary"))
            //{
            int id = int.Parse(dgvPurchase.CurrentRow.Cells["id"].Value.ToString());
            frmMain.openChildForm(new frmPurchaseByVendorDetails(id));
            //}
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadPurchaseData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadPurchaseData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dtpFrom, dtpTo);
                LoadPurchaseData();
            }
        }
    }
}