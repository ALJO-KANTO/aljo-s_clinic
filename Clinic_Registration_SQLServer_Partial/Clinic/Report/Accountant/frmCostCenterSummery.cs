using Clinic;
using Clinic.global_classes;
using Clinic.Login;
using Clinic_DataAccess;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmCostCenterSummary : Form
    {
        public frmCostCenterSummary()
        {
            InitializeComponent();

            LocalizationManager.LocalizeForm(this);

            headerUC1.FormText = this.Text;
        }

        private void frmCostCenterSummary_Load(object sender, EventArgs e)
        {
            btnPrint.Text = "Print ▼";

            dtFrom.Value = dtTo.Value = DateTime.Now;

            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    "SELECT name FROM tbl_company LIMIT 1"))
            {
                if (reader.Read())
                {
                    lblCompany.Text = reader["name"].ToString();
                }
            }

            lblDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            lblTime.Text = DateTime.Now.ToString("hh:mm:tt");

            BindCombos.PopulateCostCenter(comboBox1);

            comboBox1.SelectedIndex = -1;

            LoadData();

            ApplyCostCenterArabic();
            FixCostCenterToolbarTranslation();

            headerUC1.ArabicLayout = TranslationClass.IsArabic;
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            contextMenuExport.Show(btnPrint, new Point(0, btnPrint.Height));
        }

        private void Report_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exporting Report...");
        }
        private void LoadData()
        {
            dgvSales.Rows.Clear();
            string firstDateStr = "";

            // Get the first transaction date
            object firstDateObj = ClinicDatabase.ExecuteScalar("SELECT MIN(`date`) FROM tbl_transaction WHERE `date` IS NOT NULL");
            if (firstDateObj != null && firstDateObj != DBNull.Value)
            {
                DateTime firstDate = Convert.ToDateTime(firstDateObj);
                firstDateStr = firstDate.ToString("MMM dd, yy");
            }

            if (dgvSales.Columns.Count == 0)
            {
                dgvSales.Columns.Add("id", "Id");
                dgvSales.Columns.Add("name", "Description");
                dgvSales.Columns.Add("VoucherNo", "Voucher No");
                dgvSales.Columns.Add("debit", "Debit");
                dgvSales.Columns.Add("credit", "Credit");
                dgvSales.Columns.Add("balance", "Balance");
                LocalizationManager.LocalizeDataGridViewHeaders(dgvSales);

                dgvSales.Columns["debit"].DefaultCellStyle.Alignment = dgvSales.Columns["credit"].DefaultCellStyle.Alignment = dgvSales.Columns["balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSales.Columns["debit"].DefaultCellStyle.Font = dgvSales.Columns["credit"].DefaultCellStyle.Font = dgvSales.Columns["balance"].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Regular);
                dgvSales.Columns["VoucherNo"].DefaultCellStyle.Font = dgvSales.Columns["name"].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Regular);

                dgvSales.Columns["id"].Visible = false;

                dgvSales.CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical;

                dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvSales.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
                dgvSales.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                dgvSales.EnableHeadersVisualStyles = false;
                dgvSales.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvSales.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                dgvSales.GridColor = System.Drawing.Color.LightGray;
                dgvSales.BorderStyle = System.Windows.Forms.BorderStyle.None;
                dgvSales.RowHeadersVisible = false;
            }

            decimal totalAmount = 0;
            string query1 = @"
                                SELECT c.id, c.name, COUNT(t.id) AS Num, SUM(t.debit) AS Total_Debit, SUM(t.credit) AS Total_Credit
                                FROM tbl_cost_center c JOIN tbl_cost_center_transaction t ON c.id = t.cost_center_id
                                WHERE c.id>0 ";
            if (!chkDate.Checked)
                query1 += " AND t.date >= @from AND t.date <= @to";

            if (!guna2CheckBox1.Checked && comboBox1.SelectedIndex != -1)
                query1 += " AND c.id = @cId ";

            query1 += " GROUP BY c.id,c.name ORDER BY c.id";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query1,
                ClinicDatabase.CreateParameterNew("from",dtFrom.Value), 
                ClinicDatabase.CreateParameterNew("to", dtTo.Value),
                ClinicDatabase.CreateParameterNew("cId", (comboBox1.SelectedValue??0))))
                while (reader.Read())
                {
                    string name = reader["name"].ToString();
                    string Inv_No = reader["Num"].ToString();
                    decimal debit = reader["Total_Debit"] as decimal? ?? 0;
                    decimal credit = reader["Total_Credit"] as decimal? ?? 0;
                    decimal balance = (debit - credit);

                    totalAmount += balance;
                    dgvSales.Rows.Add(reader["id"], name, Inv_No, debit.ToString("N2"), credit.ToString("N2"), balance.ToString("N2"));

                }

            // Add total row
            //int totalRow = dgvSales.Rows.Add(null,null,"TOTAL",debitTotal,creditTotal, totalAmount.ToString("N2"));
            //dgvSales.Rows[totalRow].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline);
            //dgvSales.Rows[totalRow].DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
            //dgvSales.Rows[totalRow].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            //dgvSales.Rows[totalRow].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void SavePDF_Click(object sender, EventArgs e)
        {
            string companyName = lblCompany.Text;
            // Create PDF document
            Document doc = new Document();
            Section section = doc.AddSection();
            // Adjust page margins to position content to top-left
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);

            // Create a header table with 3 columns
            Table headerTable = section.AddTable();
            headerTable.Borders.Width = 0;
            headerTable.AddColumn("5cm");  // Time/Date
            headerTable.AddColumn("8cm");  // Title Center
            headerTable.AddColumn("5cm");  // Empty for spacing

            Row headerRow = headerTable.AddRow();

            // Left cell - Time & Date (Top-left aligned, Times New Roman)
            Paragraph left = headerRow.Cells[0].AddParagraph();
            left.Format.Font.Name = "Times New Roman";
            left.Format.Font.Size = 10;
            left.Format.Alignment = ParagraphAlignment.Left;
            left.Format.SpaceAfter = 0;
            headerRow.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            //left.AddFormattedText("Time: ", TextFormat.Bold);
            left.AddText(DateTime.Now.ToString("hh:mm tt") + "\n");
            //left.AddFormattedText("Date: ", TextFormat.Bold);
            left.AddText(DateTime.Now.ToString("dd/MM/yyyy"));

            // Center cell - Company Name & Report Titles
            Paragraph center = headerRow.Cells[1].AddParagraph();
            center.Format.Font.Name = "Times New Roman";
            center.Format.Alignment = ParagraphAlignment.Center;
            center.Format.SpaceAfter = 0;

            // Company name - Bold, size 10
            FormattedText companyText = center.AddFormattedText(companyName.ToUpper() + "\n", TextFormat.Bold);
            companyText.Font.Size = 12;

            // "Cost Center Summary" - Bold, size 10
            FormattedText summaryText = center.AddFormattedText("Cost Center Summary\n", TextFormat.Bold);
            summaryText.Font.Size = 12;

            // "All Transactions" - Regular, size 9
            FormattedText allTransText = center.AddFormattedText("All Transactions", TextFormat.NotBold);
            allTransText.Font.Size = 9;

            headerRow.Cells[1].VerticalAlignment = VerticalAlignment.Top;


            //section.Add(headerTable);

            // Bold line
            Paragraph line = section.AddParagraph();
            line.Format.Borders.Bottom.Width = 2;
            line.Format.SpaceAfter = "0.5cm";

            // Table for data
            Table dataTable = section.AddTable();
            dataTable.Borders.Width = 0;
            dataTable.AddColumn("6cm");
            dataTable.AddColumn("1cm");
            dataTable.AddColumn("1.5cm");
            dataTable.AddColumn("1.5cm");
            dataTable.AddColumn("2cm");

            decimal totalAmount = 0;

            // Load from DataGridView
            foreach (DataGridViewRow row in dgvSales.Rows)
            {
                if (row.IsNewRow) continue;
                
                string id = row.Cells["id"].Value?.ToString() ?? "0";
                string name = row.Cells["name"].Value?.ToString() ?? "";
                string vno = row.Cells["VoucherNo"].Value?.ToString() ?? "";
                decimal dr = Convert.ToDecimal(row.Cells["debit"].Value?.ToString() ?? "0");
                decimal cr = Convert.ToDecimal(row.Cells["credit"].Value?.ToString() ?? "0");
                decimal balance = Convert.ToDecimal(row.Cells["balance"].Value?.ToString() ?? "0");

                Row tRow = dataTable.AddRow();
                tRow.Cells[0].AddParagraph(name);
                tRow.Cells[1].AddParagraph(vno);
                tRow.Cells[2].AddParagraph(dr.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
                tRow.Cells[3].AddParagraph(cr.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
                tRow.Cells[4].AddParagraph(balance.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
            }
            
            // Render and save
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CostCenterSummary.pdf");
            renderer.PdfDocument.Save(filePath);
            Process.Start("explorer.exe", filePath);
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = "CustomerBalance.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Cost Center";

                    // Merge and format header row (Date)
                    Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range["B1", "C1"];
                    headerRange.Merge();
                    headerRange.Value = DateTime.Now.ToString("MMM dd, yy");
                    headerRange.Font.Bold = true;
                    headerRange.Font.Name = "Times New Roman";
                    headerRange.Font.Size = 10;
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    int rowIndex = 2;
                    foreach (DataGridViewRow row in dgvSales.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string account = row.Cells["Account"].Value?.ToString();
                        string amount = row.Cells["Amount"].Value?.ToString();

                        var accountCell = worksheet.Cells[rowIndex, 2];
                        var amountCell = worksheet.Cells[rowIndex, 3];

                        accountCell.Value = account.Replace("▶", "").Trim();
                        //amountCell.Value = amount.Replace("◀", "").Trim();

                        accountCell.Font.Name = "Times New Roman";
                        amountCell.Font.Name = "Times New Roman";

                        if (account.ToUpper().Contains("TOTAL"))
                        {
                            accountCell.Font.Size = 10;
                            accountCell.Font.Bold = true;

                            amountCell.Font.Size = 10;
                            amountCell.Font.Bold = true;
                            amountCell.Font.Underline = Microsoft.Office.Interop.Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                        }
                        else
                        {
                            accountCell.Font.Size = 10;
                            accountCell.Font.Bold = true;

                            amountCell.Font.Size = 9;
                        }

                        amountCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                        rowIndex++;
                    }

                    worksheet.Columns[2].AutoFit();
                    worksheet.Columns[3].AutoFit();

                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

     

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        private bool IsArabicReport()
        {
            return TranslationClass.IsArabic;
        }

        private string ReportTranslate(string english)
        {
            return TranslationClass.Translate(english, IsArabicReport());
        }

        private void ApplyCostCenterArabic()
        {
            bool isArabic = IsArabicReport();

            // Report title
            string title = ReportTranslate("Cost Center Summary");
            this.Text = title;
            headerUC1.FormText = title;

            // Translate every eligible control.
            TranslateCostCenterControls(this);

            // Explicit translations for important report text.
            btnPrint.Text = ReportTranslate("Print") + " ▼";

            // Translate export menu.
            TranslateCostCenterMenu(contextMenuExport.Items);

            // Translate grid headers.
            foreach (DataGridViewColumn column in dgvSales.Columns)
            {
                string englishHeader;

                switch (column.Name)
                {
                    case "name":
                        englishHeader = "Description";
                        break;
                    case "VoucherNo":
                        englishHeader = "Voucher No";
                        break;
                    case "debit":
                        englishHeader = "Debit";
                        break;
                    case "credit":
                        englishHeader = "Credit";
                        break;
                    case "balance":
                        englishHeader = "Balance";
                        break;
                    default:
                        englishHeader = column.HeaderText;
                        break;
                }

                column.HeaderText = ReportTranslate(englishHeader);
                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            // Mirror grid column display without changing underlying data.
            dgvSales.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            dgvSales.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            if (dgvSales.Columns.Contains("name"))
            {
                dgvSales.Columns["name"].DefaultCellStyle.Alignment =
                    isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;
            }

            // Mirror toolbar/filter control positions.
            ApplyCostCenterLayout(isArabic);
            FixCostCenterCheckboxLayout(isArabic);

            // Mirror minimize, maximize and close buttons.
            // Apply title-bar direction through HeaderUC itself.
            headerUC1.ArabicLayout = isArabic;
        }
        private void FixCostCenterCheckboxLayout(bool isArabic)
        {
            guna2CheckBox1.RightToLeft =
                isArabic ? RightToLeft.Yes : RightToLeft.No;

            chkDate.RightToLeft =
                isArabic ? RightToLeft.Yes : RightToLeft.No;

            guna2CheckBox1.TextAlign =
                isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft;

            chkDate.TextAlign =
                isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft;

            // Ensure the Arabic label has enough space.
            guna2CheckBox1.AutoSize = true;
            chkDate.AutoSize = true;
        }
        private readonly Dictionary<Control, string> _costCenterEnglishText =
    new Dictionary<Control, string>();
        private void TranslateCostCenterControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (!(control is TextBox) &&
                    !(control is ComboBox) &&
                    !(control is DateTimePicker) &&
                    !(control is DataGridView) &&
                    control != lblCompany &&
                    control != lblDate &&
                    control != lblTime)
                {
                    if (!_costCenterEnglishText.ContainsKey(control))
                    {
                        _costCenterEnglishText[control] = control.Text ?? "";
                    }

                    string english = _costCenterEnglishText[control].Trim();

                    if (!string.IsNullOrWhiteSpace(english))
                    {
                        control.Text = ReportTranslate(english);
                    }

                    if (control is Label label)
                    {
                        label.TextAlign = IsArabicReport()
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                }

                if (control.HasChildren)
                {
                    TranslateCostCenterControls(control);
                }
            }
        }
        private readonly Dictionary<Control, Rectangle> _originalBounds =
       new Dictionary<Control, Rectangle>();

        private void ApplyCostCenterLayout(bool isArabic)
        {
            // Keep the report body and title bar unchanged.
            // Only reposition toolbar and filter controls.

            int toolbarTop = headerUC1.Bottom;
            int filterBottom = dgvSales.Top;

            foreach (Control parent in GetAllContainers(this))
            {
                if (parent == dgvSales || parent == headerUC1)
                    continue;

                foreach (Control control in parent.Controls)
                {
                    if (control == dgvSales ||
                        control == headerUC1 ||
                        control.Dock != DockStyle.None)
                        continue;

                    if (!_originalBounds.ContainsKey(control))
                        _originalBounds[control] = control.Bounds;

                    Rectangle original = _originalBounds[control];

                    // Convert control position to form coordinates.
                    Point formPoint = this.PointToClient(
                        parent.PointToScreen(original.Location));

                    // Only mirror toolbar and filter rows.
                    if (formPoint.Y < toolbarTop ||
                        formPoint.Y >= filterBottom)
                        continue;

                    control.Left = isArabic
                        ? parent.ClientSize.Width - original.Right
                        : original.Left;

                    if (control is Label label)
                    {
                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                }
            }
        }

        private IEnumerable<Control> GetAllContainers(Control root)
        {
            yield return root;

            foreach (Control child in root.Controls)
            {
                if (child.HasChildren)
                {
                    foreach (Control nested in GetAllContainers(child))
                        yield return nested;
                }
            }
        }
        private void TranslateCostCenterMenu(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.Text))
                {
                    item.Text = ReportTranslate(item.Text.Trim());
                }

                if (item is ToolStripDropDownItem dropDown)
                {
                    TranslateCostCenterMenu(dropDown.DropDownItems);
                }
            }
        }
        private void ApplyArabicHeaderLayout(bool isArabic)
        {
            // Only mirror controls inside the custom header.
            // Do not change the entire form's RightToLeftLayout.

            foreach (Control control in headerUC1.Controls)
            {
                if (control is Button ||
                    control.GetType().Name.Contains("Button"))
                {
                    // Store the original position once.
                    if (!_originalBounds.ContainsKey(control))
                        _originalBounds[control] = control.Bounds;

                    Rectangle original = _originalBounds[control];

                    control.Left = isArabic
                        ? headerUC1.ClientSize.Width - original.Right
                        : original.Left;

                    control.Anchor = isArabic
                        ? AnchorStyles.Top | AnchorStyles.Left
                        : AnchorStyles.Top | AnchorStyles.Right;
                }
            }
        }
        private void SetReportText(
    string english,
    string arabic,
    bool isArabic)
        {
            foreach (Control control in GetAllReportControls(this))
            {
                string current = (control.Text ?? "").Trim();

                if (current.Equals(english,
                        StringComparison.OrdinalIgnoreCase) ||
                    current.Equals(arabic,
                        StringComparison.OrdinalIgnoreCase))
                {
                    control.Text = isArabic ? arabic : english;
                }
            }
        }

        private IEnumerable<Control> GetAllReportControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;

                foreach (Control child in GetAllReportControls(control))
                    yield return child;
            }
        }

        private void TranslateRemainingReportControls(
            Control parent,
            bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label ||
                    control is ButtonBase ||
                    control.GetType().Name.Contains("Button"))
                {
                    if (control != lblDate &&
                        control != lblTime &&
                        control != lblCompany)
                    {
                        string current = (control.Text ?? "").Trim();

                        // Handle texts that are still English
                        if (!string.IsNullOrWhiteSpace(current))
                        {
                            control.Text = TranslationClass.Translate(
                                current, isArabic);
                        }
                    }
                }

                if (control.HasChildren)
                {
                    TranslateRemainingReportControls(
                        control, isArabic);
                }
            }
        }
        private void FixCostCenterToolbarTranslation()
        {
            bool ar = TranslationClass.IsArabic;

            // Toolbar buttons
            SetReportText("Customize Report", "تخصيص التقرير", ar);
            SetReportText("Comment On", "إضافة تعليق", ar);
            SetReportText("Memorize", "حفظ التقرير", ar);
            SetReportText("Excel", "إكسل", ar);

            btnPrint.Text = ar ? "طباعة ▼" : "Print ▼";

            // Report title
            string title = ar
                ? "ملخص مراكز التكلفة"
                : "Cost Center Summary";

            this.Text = title;
            headerUC1.FormText = title;

            // Translate all static labels and buttons
            TranslateRemainingReportControls(this, ar);

            // Translate grid headers and total cells

            // Translate grid headers - compatible with C# 7.3
            foreach (DataGridViewColumn col in dgvSales.Columns)
            {
                string english;

                switch (col.Name)
                {
                    case "name":
                        english = "Description";
                        break;

                    case "VoucherNo":
                        english = "Voucher No";
                        break;

                    case "debit":
                        english = "Debit";
                        break;

                    case "credit":
                        english = "Credit";
                        break;

                    case "balance":
                        english = "Balance";
                        break;

                    default:
                        english = col.HeaderText;
                        break;
                }

                col.HeaderText = TranslationClass.Translate(english, ar);
            }


            foreach (DataGridViewRow row in dgvSales.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value is string value &&
                        value.Trim().Equals("TOTAL",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        cell.Value = ar ? "الإجمالي" : "TOTAL";
                    }
                }
            }

            dgvSales.Refresh();
        }
        private void TranslateToolbarButtons(
            Control parent,
            Dictionary<string, string> captions,
            bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is ButtonBase ||
                    control.GetType().Name.Contains("Button"))
                {
                    string current = (control.Text ?? "").Trim();

                    foreach (var item in captions)
                    {
                        if (current.Equals(item.Key,
                            StringComparison.OrdinalIgnoreCase) ||
                            current.Equals(item.Value,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            control.Text = isArabic
                                ? item.Value
                                : item.Key;

                            break;
                        }
                    }
                }

                if (control.HasChildren)
                {
                    TranslateToolbarButtons(
                        control, captions, isArabic);
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dtFrom, dtTo);
                LoadData();
            }
        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !guna2CheckBox1.Checked;
            LoadData();
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            dtFrom.Enabled = dtTo.Enabled = !chkDate.Checked;
            LoadData();
        }
    }
}