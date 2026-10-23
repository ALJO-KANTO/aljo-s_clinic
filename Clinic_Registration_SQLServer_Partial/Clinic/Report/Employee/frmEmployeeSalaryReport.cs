using Clinic.global_classes;
using Clinic_DataAccess;
using Guna.UI2.WinForms;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmEmployeeSalaryReport : Form
    {
        int id;
        public frmEmployeeSalaryReport(int _id = 0)
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            headerUC1.FormText = this.Text;
            this.id = _id;
        }

        private void frmEmployeeSalaryReport_Load(object sender, EventArgs e)
        {
            btnPrint.Text = "Print ▼";
            LoadData();
            DateTime dated = DateTime.Now;
            lblDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            lblTime.Text = DateTime.Now.ToString("hh:mm:tt");
            loadCompany();
            ApplyEmployeeSalaryLanguage();
        }
        private void loadCompany()
        {
            using (var reader = ClinicDatabase.ExecuteReader("SELECT name FROM tbl_company"))
            {
                if (reader.Read() && reader["name"] != DBNull.Value)
                {
                    guna2HtmlLabel8.Text = reader["name"].ToString();
                }
            }
        }

        // Presentation only: preserve salary calculations, filtering, and exports.
        private void ApplyEmployeeSalaryLanguage()
        {
            bool arabic = (Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en")
                .StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            headerUC1.ArabicLayout = arabic;
            if (!arabic) return;

            this.Text = "تقرير رواتب الموظفين";
            headerUC1.FormText = this.Text;
            btnPrint.Text = "طباعة ▼";

            TranslateSalaryControls(this);
            TranslateSalaryMenus(this);
            PositionSalaryToolbarArabic();

            dgvSales.RightToLeft = RightToLeft.Yes;
            dgvSales.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            SetSalaryColumn("SN", "الرقم");
            SetSalaryColumn("EmployeeName", "اسم الموظف");
            SetSalaryColumn("BasicSalary", "الراتب الأساسي");
            SetSalaryColumn("WorkingHours", "ساعات العمل");
            SetSalaryColumn("WorkingDays", "أيام العمل");
            SetSalaryColumn("DailyWage", "الأجر اليومي");
            SetSalaryColumn("LoanAmount", "مبلغ السلفة");
            SetSalaryColumn("Balance", "الرصيد");
            // Keep TOTAL row value intact for exports and existing logic.
        }

        private void SetSalaryColumn(string name, string caption)
        {
            if (dgvSales.Columns.Contains(name))
                dgvSales.Columns[name].HeaderText = caption;
        }

        private string TranslateSalaryText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            string key = text.Trim();
            string translated = Clinic.TranslationClass.Translate(key, true);
            if (translated != key) return translated;
            switch (key)
            {
                case "Employee Salary Report": return "تقرير رواتب الموظفين";
                case "Employee Name": return "اسم الموظف";
                case "Basic Salary": return "الراتب الأساسي";
                case "Working Hours": return "ساعات العمل";
                case "Work Days": return "أيام العمل";
                case "Daily Wage": return "الأجر اليومي";
                case "Loan Amount": return "مبلغ السلفة";
                case "Print ▼": return "طباعة ▼";
                case "Print": return "طباعة";
                case "From": return "من";
                case "To": return "إلى";
                case "All Dates": return "جميع التواريخ";
                case "Export to PDF": return "تصدير إلى PDF";
                case "Export to Excel": return "تصدير إلى Excel";
                default: return text;
            }
        }

        private void TranslateSalaryControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                // Do not alter dynamic company names, date/time, or grid values.
                if (c != guna2HtmlLabel8 && c != lblDate && c != lblTime &&
                    (c is Label || c is Button || c is CheckBox ||
                     c is GroupBox || c is RadioButton ||
                     c.GetType().Name == "Guna2HtmlLabel"))
                {
                    c.Text = TranslateSalaryText(c.Text);
                    if (c is Label)
                        ((Label)c).TextAlign = ContentAlignment.MiddleRight;
                    if (c is CheckBox)
                        ((CheckBox)c).CheckAlign = ContentAlignment.MiddleRight;
                    if (c.GetType().Name == "Guna2HtmlLabel")
                        c.RightToLeft = RightToLeft.Yes;
                }
                if (c.HasChildren) TranslateSalaryControls(c);
            }
        }

        private void TranslateSalaryMenus(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is ToolStrip)
                    TranslateSalaryMenuItems(((ToolStrip)c).Items);
                if (c.ContextMenuStrip != null)
                    TranslateSalaryMenuItems(c.ContextMenuStrip.Items);
                if (c.HasChildren) TranslateSalaryMenus(c);
            }
            // This report opens its export menu directly from btnPrint.
            if (parent == this && contextMenuExport != null)
                TranslateSalaryMenuItems(contextMenuExport.Items);
        }

        private void TranslateSalaryMenuItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.Text = TranslateSalaryText(item.Text);
                if (item is ToolStripDropDownItem)
                    TranslateSalaryMenuItems(((ToolStripDropDownItem)item).DropDownItems);
            }
        }

        private void PositionSalaryToolbarArabic()
        {
            Control parent = btnPrint.Parent;
            if (parent == null || parent.ClientSize.Width <= 0) return;

            var buttons = new List<Control>();
            foreach (Control c in parent.Controls)
            {
                if (c is Button && c.Visible && c.Dock == DockStyle.None &&
                    c.Top >= btnPrint.Top - 12 && c.Top <= btnPrint.Bottom + 12)
                    buttons.Add(c);
            }
            if (buttons.Count == 0) return;
            buttons.Sort((a, b) => a.Left.CompareTo(b.Left));

            int required = 20 + (buttons.Count - 1) * 7;
            foreach (Control b in buttons) required += b.Width;
            if (required > parent.ClientSize.Width) return;

            int right = parent.ClientSize.Width - 10;
            foreach (Control b in buttons)
            {
                b.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                b.Left = right - b.Width;
                right = b.Left - 7;
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
        private void LoadData()
        {
            try
            {
                dgvSales.Rows.Clear();
                dgvSales.Columns.Clear();

                dgvSales.Columns.Add("SN", "SN");
                dgvSales.Columns.Add("EmployeeName", "Employee Name");
                dgvSales.Columns.Add("BasicSalary", "Basic Salary");
                dgvSales.Columns.Add("WorkingHours", "Working Hours");
                dgvSales.Columns.Add("WorkingDays", "Work Days");
                dgvSales.Columns.Add("DailyWage", "Daily Wage");
                dgvSales.Columns.Add("LoanAmount", "Loan Amount");
                dgvSales.Columns.Add("Balance", "Balance");

                dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvSales.RowHeadersVisible = false;

                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    ClinicDatabase.CreateParameterNew("@id", id)
                };

                string dateFilter = "";
                if (!chkDate.Checked)
                {
                    dateFilter = " AND t.date >= @from AND t.date  <= @to";
                    parameters.Add(ClinicDatabase.CreateParameterNew("@from", dtpFrom.Value.Date));
                    parameters.Add(ClinicDatabase.CreateParameterNew("@to", dtpTo.Value.Date));
                }

                string query = $@"
                               SELECT
                                ROW_NUMBER() OVER (ORDER BY e.name) AS SN,
                                e.id,
                                e.name,
                                IFNULL(e.BasicSalary,0) AS BasicSalary,
                                IFNULL(e.workinghours,0) AS WorkingHours,
                                IFNULL(e.WorkDays,0) AS WorkDays,

                                IFNULL(SUM(
                                    CASE
                                        WHEN t.type = 'Loan Request'
                                        THEN t.debit
                                        ELSE 0
                                    END
                                ),0) AS LoanAmount,

                                (
                                    IFNULL(e.BasicSalary,0)
                                    +
                                    IFNULL(SUM(
                                        CASE
                                            WHEN t.type = 'Loan Request'
                                            THEN t.debit
                                            ELSE 0
                                        END
                                    ),0)
                                    -
                                    IFNULL(SUM(
                                        CASE
                                            WHEN t.type IN ('Employee Salary Payment','Employee Loan Payment')
                                            THEN t.debit
                                            ELSE 0
                                        END
                                    ),0)
                                ) AS Balance

                            FROM tbl_employee e
                            LEFT JOIN tbl_transaction t
                                ON t.hum_id = e.id
                               AND t.state = 0

                            WHERE e.state = 0

                            {dateFilter}
                    
                            GROUP BY
                                e.id,
                                e.name,
                                e.BasicSalary,
                                e.workinghours,
                                e.WorkDays

                            ORDER BY e.name";

                decimal totalSalary = 0;
                decimal totalLoan = 0;
                decimal totalBalance = 0;

                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, parameters.ToArray()))
                {
                    while (reader.Read())
                    {
                        decimal basicSalary = Convert.ToDecimal(reader["BasicSalary"]);
                        int workDays = Convert.ToInt32(reader["WorkDays"]);

                        decimal dailyWage = 0;

                        if (workDays > 0)
                            dailyWage = basicSalary / workDays;

                        decimal loanAmount = Convert.ToDecimal(reader["LoanAmount"]);
                        decimal balance = Convert.ToDecimal(reader["Balance"]);

                        int rowIndex = dgvSales.Rows.Add(
                            reader["SN"].ToString(),
                            reader["name"].ToString(),
                            basicSalary.ToString("N2"),
                            reader["WorkingHours"].ToString(),
                            workDays.ToString(),
                            dailyWage.ToString("N2"),
                            loanAmount.ToString("N2"),
                            balance.ToString("N2") + " ◀"
                        );

                        dgvSales.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                        dgvSales.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.White;

                        totalSalary += basicSalary;
                        totalLoan += loanAmount;
                        totalBalance += balance;
                    }
                }

                // TOTAL ROW
                int totalRow = dgvSales.Rows.Add(
                    "",
                    "TOTAL",
                    totalSalary.ToString("N2"),
                    "",
                    "",
                    "",
                    totalLoan.ToString("N2"),
                    totalBalance.ToString("N2") + " ◀"
                );

                dgvSales.Rows[totalRow].DefaultCellStyle.Font =
                    new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);

                dgvSales.Rows[totalRow].DefaultCellStyle.BackColor =
                    System.Drawing.Color.Gainsboro;

                dgvSales.Columns["BasicSalary"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                dgvSales.Columns["WorkingDays"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                dgvSales.Columns["DailyWage"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                dgvSales.Columns["LoanAmount"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                dgvSales.Columns["Balance"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                dgvSales.Refresh();
                dgvSales.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SavePDF_Click(object sender, EventArgs e)
        {
            // Get company name from DB
            string companyName = guna2HtmlLabel8.Text;

            // Create PDF document
            Document doc = new Document();
            Section section = doc.AddSection();
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);

            // Header Table (Time/Date + Company Info)
            Table headerTable = section.AddTable();
            headerTable.Borders.Width = 0;
            headerTable.AddColumn("5cm");
            headerTable.AddColumn("8cm");
            headerTable.AddColumn("5cm");

            Row headerRow = headerTable.AddRow();

            // Left cell - Time and Date
            Paragraph left = headerRow.Cells[0].AddParagraph();
            left.Format.Font.Name = "Times New Roman";
            left.Format.Font.Size = 10;
            left.Format.Alignment = ParagraphAlignment.Left;
            headerRow.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            left.AddText("Time: " + DateTime.Now.ToString("hh:mm tt") + "\n");
            left.AddText("Date: " + DateTime.Now.ToString("dd/MM/yyyy"));

            // Center cell - Company Info
            Paragraph center = headerRow.Cells[1].AddParagraph();
            center.Format.Font.Name = "Times New Roman";
            center.Format.Alignment = ParagraphAlignment.Center;
            center.AddFormattedText(companyName.ToUpper() + "\n", TextFormat.Bold).Font.Size = 12;
            center.AddFormattedText("Employee Salary Report\n", TextFormat.Bold).Font.Size = 10;
            center.AddFormattedText("All Transactions", TextFormat.NotBold).Font.Size = 9;

            headerRow.Cells[1].VerticalAlignment = VerticalAlignment.Top;

            // Bold black line
            Paragraph line = section.AddParagraph();
            line.Format.Borders.Bottom.Width = 2;
            line.Format.SpaceAfter = "0.5cm";

            // Table for data
            Table dataTable = section.AddTable();
            dataTable.Borders.Width = 0;

            List<string> headerText = new List<string>();

            foreach (DataGridViewColumn col in dgvSales.Columns)
            {
                dataTable.AddColumn(Unit.FromCentimeter(3));
                headerText.Add(col.HeaderText);
            }

            Row headerRow0 = dataTable.AddRow();

            for (int i = 0; i < headerText.Count; i++)
            {
                headerRow0.Cells[i].AddParagraph(headerText[i]);
            }
            foreach (DataGridViewRow dgvRow in dgvSales.Rows)
            {
                if (dgvRow.IsNewRow)
                    continue;

                Row pdfRow = dataTable.AddRow();

                for (int i = 0; i < dgvSales.Columns.Count; i++)
                {
                    object value = dgvRow.Cells[i].Value;
                    string text = "";

                    if (value != null && value != DBNull.Value)
                    {
                        if (value is DateTime dt)
                            text = dt.ToString("dd-MM-yyyy");
                        else if (value is decimal dec)
                            text = dec.ToString("N2");
                        else if (value is double dbl)
                            text = dbl.ToString("N2");
                        else
                            text = value.ToString();
                    }

                    pdfRow.Cells[i].AddParagraph(text);
                }
            }

            // Export
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Save PDF";
                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveDialog.FileName = "EmployeeSalaryReport.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    renderer.PdfDocument.Save(saveDialog.FileName);
                    Process.Start("explorer.exe", saveDialog.FileName);
                }
            }
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = "EmployeeSalaryReport.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Employee Salary Report";

                    // Merge and format header row (Date)
                    Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range["A1", "G1"];
                    headerRange.Merge();
                    headerRange.Value = DateTime.Now.ToString("MMM dd, yy");
                    headerRange.Font.Bold = true;
                    headerRange.Font.Name = "Times New Roman";
                    headerRange.Font.Size = 10;
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    // Column headers
                    for (int col = 0; col < dgvSales.Columns.Count; col++)
                    {
                        worksheet.Cells[2, col + 1] = dgvSales.Columns[col].HeaderText;

                        var cell = worksheet.Cells[2, col + 1];
                        cell.Font.Bold = true;
                        cell.Font.Name = "Times New Roman";
                        cell.Font.Size = 10;
                        cell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    }

                    int rowIndex = 3;

                    foreach (DataGridViewRow dgvRow in dgvSales.Rows)
                    {
                        if (dgvRow.IsNewRow)
                            continue;

                        for (int col = 0; col < dgvSales.Columns.Count; col++)
                        {
                            object value = dgvRow.Cells[col].Value;

                            worksheet.Cells[rowIndex, col + 1] = value;

                            var cell = worksheet.Cells[rowIndex, col + 1];
                            cell.Font.Name = "Times New Roman";
                            cell.Font.Size = 10;

                            // Right-align numeric values
                            if (value is int || value is long || value is float ||
                                value is double || value is decimal)
                            {
                                cell.HorizontalAlignment =
                                    Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                            }
                            // Center dates
                            else if (value is DateTime dt)
                            {
                                cell.NumberFormat = "dd-MM-yyyy";
                                cell.HorizontalAlignment =
                                    Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                            }
                        }

                        rowIndex++;
                    }

                    worksheet.Columns.AutoFit();

                    // Save the workbook and close
                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void frmEmployeeSalaryReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
            //dgvSales.CellMouseEnter -= dgvSales_CellMouseEnter;
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = dtpTo.Enabled = !chkDate.Checked;
            LoadData();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dtpFrom, dtpTo);
                LoadData();
            }
        }
    }
}
