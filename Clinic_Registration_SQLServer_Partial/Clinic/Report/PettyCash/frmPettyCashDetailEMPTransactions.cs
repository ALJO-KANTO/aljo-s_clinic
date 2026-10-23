using Clinic;
using Clinic.global_classes;
using Clinic_DataAccess;
using Guna.UI2.WinForms;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmPettyCashDetailEMPTransactions : Form
    {
        int id;
        public frmPettyCashDetailEMPTransactions(int _id = 0)
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            headerUC1.FormText = this.Text;
            this.id = _id;
        }

        bool EnableApproval = false;

        private void frmPettyCashDetailEMPTransactions_Load(object sender, EventArgs e)
        {
            BindCombos.PopulateEmployees(cmbEmployee);
            DateTime dated = DateTime.Now;
            lblDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            lblTime.Text = DateTime.Now.ToString("hh:mm:tt");
            var barcodeS = Utilities.GeneralSettingsState("ENABLE PETTYCASH APPROVAL");
            if (!string.IsNullOrEmpty(barcodeS) && int.Parse(barcodeS) > 0)
            {
                EnableApproval = true;
            }
            else
            {
                EnableApproval = false;
            }
            loadCompany();
            LoadData();
            ApplyPettyCashArabicUI();
        }
        // UI translation only: never change grid cell values, employee selections,
        // transaction types, SQL, financial calculations, or export logic.
        private bool IsPettyCashArabic()
        {
            return (Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en")
                .StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        }

        private string PettyCashCaption(string english)
        {
            if (!IsPettyCashArabic()) return english;
            switch ((english ?? "").Trim())
            {
                case "Petty Cash Transaction": return "معاملات العهدة النقدية";
                case "Petty Cash Transactions": return "معاملات العهدة النقدية";
                case "Petty Cash Detail EMP Transaction":
                case "Petty Cash Detail EMP Transactions":
                    return "تفاصيل معاملات المصروفات النثرية للموظف";
                case "Petty Cash Balance Detail EMP":
                    return "تفاصيل رصيد العهدة النقدية للموظف";
                case "Petty Cash Summary": return "ملخص المصروفات النثرية";
                case "Petty Cash Balance Detail": return "تفاصيل رصيد المصروفات النثرية";
                case "All Transaction":
                case "All Transactions": return "جميع المعاملات";
                case "Company Name": return "اسم الشركة";
                case "Employee": return "الموظف";
                case "Employee Code": return "رمز الموظف";
                case "Employee Name": return "اسم الموظف";
                case "Employee Name:": return "اسم الموظف:";
                case "Voucher No": return "رقم السند";
                case "Type": return "النوع";
                case "Description": return "الوصف";
                case "Debit": return "مدين";
                case "Credit": return "دائن";
                case "Balance": return "الرصيد";
                case "SN": return "م";
                case "SN#": return "م";
                case "Inv Id": return "رقم الفاتورة";
                case "Id": return "الرقم";
                case "Excel": return "إكسل";
                case "Print": return "طباعة";
                default: return Clinic.TranslationClass.Translate(english, true);
            }
        }

        private void ApplyPettyCashArabicUI()
        {
            if (!IsPettyCashArabic()) return;

            // Form and reusable HeaderUC title are separate controls.
            string originalTitle = this.Text;
            this.Text = PettyCashCaption(originalTitle);
            headerUC1.FormText = PettyCashCaption(originalTitle);
            // The report has a separate heading inside the form.
            // Translate it through the recursive UI control pass below.
            headerUC1.ArabicLayout = true;

            TranslatePettyCashControls(this);
            ApplyPettyCashGridLanguage();
        }

        private void TranslatePettyCashControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control == headerUC1) continue;
                if (control == dgvSales) continue;
                if (control == guna2HtmlLabel8) continue; // company name from database
                if (control == lblDate || control == lblTime) continue;
                if (control is TextBoxBase || control is ComboBox ||
                    control is DateTimePicker || control is ListBox) continue;

                if (!string.IsNullOrWhiteSpace(control.Text))
                    control.Text = PettyCashCaption(control.Text);

                // Preserve centered report headings and existing control positions.
                TranslatePettyCashControls(control);
            }
        }

        private void ApplyPettyCashGridLanguage()
        {
            if (!IsPettyCashArabic()) return;
            dgvSales.RightToLeft = RightToLeft.Yes;
            foreach (DataGridViewColumn column in dgvSales.Columns)
                column.HeaderText = PettyCashCaption(column.HeaderText);
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
        private void LoadData(int empId = 0)
        {
            dgvSales.DataSource = null;
            dgvSales.ColumnHeadersVisible = true;
            dgvSales.EnableHeadersVisualStyles = false;

            dgvSales.Rows.Clear();
            dgvSales.Columns.Clear();

            // Define columns
            dgvSales.Columns.Add("id", "Id");
            dgvSales.Columns["id"].Visible = false;

            dgvSales.Columns.Add("SN", "SN#");
            dgvSales.Columns.Add("Date", "Date");
            dgvSales.Columns.Add("No", "Voucher No");
            dgvSales.Columns.Add("Type", "Type");
            dgvSales.Columns.Add("InvoiceId", "Inv Id");
            dgvSales.Columns.Add("Description", "Description");
            dgvSales.Columns.Add("Debit", "Debit");
            dgvSales.Columns.Add("Credit", "Credit");
            dgvSales.Columns.Add("Balance", "Balance");

            // Format numeric columns
            dgvSales.Columns["Debit"].DefaultCellStyle.Format = "N2";
            dgvSales.Columns["Debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSales.Columns["Credit"].DefaultCellStyle.Format = "N2";
            dgvSales.Columns["Credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSales.Columns["Balance"].DefaultCellStyle.Format = "N2";
            dgvSales.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Query
            string query = @"
                                SELECT 
                                    t.id,
                                    t.`Date`,
                                    CAST(t.voucher_no AS CHAR CHARACTER SET utf8mb4) AS `No`,
                                    CAST(t.type AS CHAR CHARACTER SET utf8mb4) AS `Type`,
                                    t.transaction_id AS `Invoice Id`,
                                    CAST(coa.name AS CHAR CHARACTER SET utf8mb4) AS `Description`,
                                    t.debit AS `Debit`,
                                    t.credit AS `Credit`
                                FROM 
                                    tbl_transaction t
                                INNER JOIN 
                                    tbl_coa_level_4 coa ON t.account_id = coa.id
                                WHERE 
                                    t.transaction_id IN (
                                        SELECT pt.id 
                                        FROM tbl_employee e
                                        INNER JOIN tbl_petty_cash pt ON pt.employee_id = e.id
                                        WHERE e.id = @empId
                                    )
                                    AND t.state = 0
                                    AND t.type = 'Petty Cash'
                                    AND t.date >= @sDate AND t.date <= @eDate
                                ORDER BY t.`Date`, t.id;
                                ";

            // Fetch data
            DataTable dt = ClinicDatabase.ExecuteDataTable(query,
                ClinicDatabase.CreateParameterNew("@empId", empId),
                ClinicDatabase.CreateParameterNew("@sDate", dateTimePicker1.Value.Date),
                ClinicDatabase.CreateParameterNew("@eDate", dateTimePicker2.Value.Date)
                );

            // Fill rows with running balance
            decimal balance = 0;
            int sn = 1;

            foreach (DataRow row in dt.Rows)
            {
                decimal debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0;
                decimal credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0;
                balance += debit - credit;

                dgvSales.Rows.Add(
                    row["id"],
                    sn++,
                    DateTime.Parse(row["Date"].ToString()).ToShortDateString(),
                    string.IsNullOrEmpty(row["No"].ToString()) ? ("PC-00" + row["id"]) : row["No"],
                    row["Type"],
                    row["Invoice Id"] != DBNull.Value ? row["Invoice Id"].ToString() : null,
                    row["Description"],
                    debit,
                    credit,
                    balance
                );
            }

            // Styling
            dgvSales.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            dgvSales.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            //dgvInvoice.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
            //dgvInvoice.DefaultCellStyle.Font = new Font("Times New Roman", 9);

            dgvSales.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSales.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            dgvSales.EnableHeadersVisualStyles = false;
            dgvSales.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvSales.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvSales.GridColor = System.Drawing.Color.LightGray;
            dgvSales.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvSales.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvSales.RowHeadersVisible = false;
            LocalizationManager.LocalizeDataGridViewHeaders(dgvSales);
            ApplyPettyCashGridLanguage();
        }

        private void dgvSales_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSales_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvSales.Rows[e.RowIndex].Cells["id"].Value != null)
                {
                    int _id = int.Parse(dgvSales.Rows[e.RowIndex].Cells["id"].Value.ToString());
                    if (EnableApproval)
                    {
                        //if(_type == "Petty Cash") { }
                        frmMain.openChildForm(new frmViewPettyCashVoucher(_id));
                    }
                    else
                    {
                        var _type = dgvSales.Rows[e.RowIndex].Cells["Type"].Value.ToString();
                        if (_type == "Petty Cash Request")
                        {
                            frmMain.openChildForm(new frmPettyCashRequest(_id));
                        }
                        else if (_type == "Petty Cash Submission")
                        {
                            frmMain.openChildForm(new frmPettyCashSubmission(_id));
                        }
                        else if (_type == "Employee Petty Cash Payment")
                        {
                            frmMain.openChildForm(new frmViewPaymentVoucher(_id));

                        }
                    }
                }
            }
        }
        private void btn_Excel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = "PettyCashDetails.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application excelApp = null;
                    Microsoft.Office.Interop.Excel.Workbook workbook = null;
                    Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
                    try
                    {
                        excelApp = new Microsoft.Office.Interop.Excel.Application();
                        excelApp.Visible = false;

                        workbook = excelApp.Workbooks.Add(Type.Missing);
                        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                        worksheet.Name = "Petty Cash Details";

                        // =========================
                        // Get visible columns only
                        // =========================
                        List<DataGridViewColumn> visibleColumns = new List<DataGridViewColumn>();
                        foreach (DataGridViewColumn col in dgvSales.Columns)
                        {
                            if (col.Visible)
                                visibleColumns.Add(col);
                        }

                        int totalColumns = visibleColumns.Count;

                        // =========================
                        // Header Date Row
                        // =========================
                        Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range[
                            worksheet.Cells[1, 1],
                            worksheet.Cells[1, totalColumns]
                        ];

                        headerRange.Merge();
                        headerRange.Value = DateTime.Now.ToString("MMM dd, yy");
                        headerRange.Font.Bold = true;
                        headerRange.Font.Name = "Times New Roman";
                        headerRange.Font.Size = 10;
                        headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                        // =========================
                        // Column Headers (same as grid)
                        // =========================
                        for (int i = 0; i < visibleColumns.Count; i++)
                        {
                            worksheet.Cells[2, i + 1] = visibleColumns[i].HeaderText;

                            var headerCell = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, i + 1];
                            headerCell.Font.Bold = true;
                            headerCell.Font.Name = "Times New Roman";
                            headerCell.Font.Size = 10;
                        }

                        // =========================
                        // Data Rows (same as grid)
                        // =========================
                        int rowIndex = 3;

                        foreach (DataGridViewRow row in dgvSales.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int colIndex = 0; colIndex < visibleColumns.Count; colIndex++)
                            {
                                var gridCol = visibleColumns[colIndex];
                                var cellValue = row.Cells[gridCol.Name].FormattedValue?.ToString() ?? "";

                                worksheet.Cells[rowIndex, colIndex + 1] = cellValue;

                                var excelCell = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, colIndex + 1];
                                excelCell.Font.Name = "Times New Roman";
                                excelCell.Font.Size = 10;

                                // Right align numeric columns
                                if (gridCol.ValueType == typeof(decimal) ||
                                    gridCol.ValueType == typeof(double) ||
                                    gridCol.ValueType == typeof(float) ||
                                    gridCol.ValueType == typeof(int) ||
                                    gridCol.ValueType == typeof(long))
                                {
                                    excelCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                                }
                            }

                            // Optional: apply TOTAL formatting if "Account" column exists
                            if (dgvSales.Columns.Contains("Account"))
                            {
                                string account = row.Cells["Account"].FormattedValue?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(account) && account.ToUpper().Contains("TOTAL"))
                                {
                                    for (int colIndex = 0; colIndex < visibleColumns.Count; colIndex++)
                                    {
                                        var excelCell = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, colIndex + 1];
                                        excelCell.Font.Bold = true;

                                        // Optional: underline amount-like columns
                                        string headerText = visibleColumns[colIndex].HeaderText.ToUpper();
                                        if (headerText.Contains("AMOUNT") || headerText.Contains("TOTAL") || headerText.Contains("BALANCE"))
                                        {
                                            excelCell.Font.Underline = Microsoft.Office.Interop.Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                                        }
                                    }
                                }
                            }

                            rowIndex++;
                        }

                        // =========================
                        // AutoFit all visible columns
                        // =========================
                        for (int col = 1; col <= totalColumns; col++)
                        {
                            ((Microsoft.Office.Interop.Excel.Range)worksheet.Columns[col]).AutoFit();
                        }

                        // Save
                        workbook.SaveAs(saveDialog.FileName);
                        MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Cleanup COM objects properly
                        if (workbook != null)
                        {
                            workbook.Close(false);
                            Marshal.ReleaseComObject(workbook);
                        }

                        if (worksheet != null)
                            Marshal.ReleaseComObject(worksheet);

                        if (excelApp != null)
                        {
                            excelApp.Quit();
                            Marshal.ReleaseComObject(excelApp);
                        }

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }
        }
        private void btn_Excel_ClickOld(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = "PettyCashDetails.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Petty Cash Details";

                    // Merge and format header row (Date)
                    Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range["A1", "G1"];
                    headerRange.Merge();
                    headerRange.Value = DateTime.Now.ToString("MMM dd, yy");
                    headerRange.Font.Bold = true;
                    headerRange.Font.Name = "Times New Roman";
                    headerRange.Font.Size = 10;
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    if (EnableApproval)
                    {
                        // Adding Column Headers
                        worksheet.Cells[2, 1] = "ID";
                        worksheet.Cells[2, 2] = "Ref";
                        worksheet.Cells[2, 3] = "Date";
                        worksheet.Cells[2, 4] = "Description";
                        worksheet.Cells[2, 5] = "Account";
                        worksheet.Cells[2, 6] = "Amount";
                        worksheet.Cells[2, 7] = "Status";

                        int rowIndex = 3;
                        foreach (DataGridViewRow row in dgvSales.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string id = row.Cells["id"].Value?.ToString();
                            string reference = row.Cells["Reference"].Value?.ToString();
                            string date = row.Cells["Date"].Value?.ToString();
                            string description = row.Cells["Description"].Value?.ToString();
                            string account = row.Cells["Account"].Value?.ToString();
                            string amount = row.Cells["Total"].Value?.ToString();
                            string status = row.Cells["Status"].Value?.ToString() ?? "";
                            //string status = string.IsNullOrEmpty(_status) ? "No" : int.Parse(_status) == 1 ? "Ok" : "No";

                            worksheet.Cells[rowIndex, 1] = id;
                            worksheet.Cells[rowIndex, 2] = reference;
                            worksheet.Cells[rowIndex, 3] = date;
                            worksheet.Cells[rowIndex, 4] = description;
                            worksheet.Cells[rowIndex, 5] = account;
                            worksheet.Cells[rowIndex, 6] = amount;
                            worksheet.Cells[rowIndex, 7] = status;

                            // Formatting
                            var accountCell = worksheet.Cells[rowIndex, 5];
                            var amountCell = worksheet.Cells[rowIndex, 6];

                            accountCell.Font.Name = "Times New Roman";
                            amountCell.Font.Name = "Times New Roman";

                            if (!string.IsNullOrEmpty(account) && account.ToUpper().Contains("TOTAL"))
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

                        // Auto-fit columns
                        for (int col = 1; col <= 7; col++)
                        {
                            worksheet.Columns[col].AutoFit();
                        }
                    }
                    else
                    {
                        // Adding Column Headers
                        worksheet.Cells[2, 1] = "ID";
                        worksheet.Cells[2, 2] = "Type";
                        worksheet.Cells[2, 3] = "Date";
                        worksheet.Cells[2, 4] = "Num";
                        worksheet.Cells[2, 5] = "Account";
                        worksheet.Cells[2, 6] = "Amount";
                        worksheet.Cells[2, 7] = "Balance";

                        int rowIndex = 3;
                        foreach (DataGridViewRow row in dgvSales.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string id = row.Cells["id"].Value?.ToString();
                            string type = row.Cells["Type"].Value?.ToString();
                            string date = row.Cells["Date"].Value?.ToString();
                            string num = row.Cells["Num"].Value?.ToString();
                            string account = row.Cells["Account"].Value?.ToString();
                            string amount = row.Cells["Total"].Value?.ToString();
                            string balance = row.Cells["Balance"].Value?.ToString();

                            worksheet.Cells[rowIndex, 1] = id;
                            worksheet.Cells[rowIndex, 2] = type;
                            worksheet.Cells[rowIndex, 3] = date;
                            worksheet.Cells[rowIndex, 4] = num;
                            worksheet.Cells[rowIndex, 5] = account;
                            worksheet.Cells[rowIndex, 6] = amount;
                            worksheet.Cells[rowIndex, 7] = balance;

                            // Formatting the cells
                            var accountCell = worksheet.Cells[rowIndex, 5];
                            var amountCell = worksheet.Cells[rowIndex, 6];

                            accountCell.Font.Name = "Times New Roman";
                            amountCell.Font.Name = "Times New Roman";

                            // If account contains 'TOTAL', apply special formatting
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

                        worksheet.Columns[1].AutoFit();
                        worksheet.Columns[2].AutoFit();
                        worksheet.Columns[3].AutoFit();
                        worksheet.Columns[4].AutoFit();
                        worksheet.Columns[5].AutoFit();
                        worksheet.Columns[6].AutoFit();
                        worksheet.Columns[7].AutoFit();
                    }

                    // Save the workbook and close
                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            string companyName = guna2HtmlLabel8.Text;

            // Create PDF document
            Document doc = new Document();
            Section section = doc.AddSection();

            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);

            // =========================
            // Header Table
            // =========================
            Table headerTable = section.AddTable();
            headerTable.Borders.Width = 0;
            headerTable.AddColumn("5cm");
            headerTable.AddColumn("8cm");
            headerTable.AddColumn("5cm");

            Row headerRow = headerTable.AddRow();

            // Left
            Paragraph left = headerRow.Cells[0].AddParagraph();
            left.Format.Font.Name = "Times New Roman";
            left.Format.Font.Size = 10;
            left.Format.Alignment = ParagraphAlignment.Left;
            headerRow.Cells[0].VerticalAlignment = VerticalAlignment.Top;
            left.AddText("Time: " + DateTime.Now.ToString("hh:mm tt") + "\n");
            left.AddText("Date: " + DateTime.Now.ToString("dd/MM/yyyy"));

            // Center
            Paragraph center = headerRow.Cells[1].AddParagraph();
            center.Format.Font.Name = "Times New Roman";
            center.Format.Alignment = ParagraphAlignment.Center;
            center.AddFormattedText(companyName.ToUpper() + "\n", TextFormat.Bold).Font.Size = 12;
            center.AddFormattedText("Petty Cash Summary\n", TextFormat.Bold).Font.Size = 10;
            center.AddFormattedText("All Transactions", TextFormat.NotBold).Font.Size = 9;

            headerRow.Cells[1].VerticalAlignment = VerticalAlignment.Top;

            // Line
            Paragraph line = section.AddParagraph();
            line.Format.Borders.Bottom.Width = 2;
            line.Format.SpaceAfter = "0.5cm";

            // =========================
            // Data Table
            // =========================
            Table dataTable = section.AddTable();
            dataTable.Borders.Width = 0.5;
            dataTable.Format.Font.Name = "Times New Roman";

            // Get visible columns in same display order
            List<DataGridViewColumn> exportColumns = dgvSales.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible) // remove this if you want hidden columns too
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            // Optional: skip ID column if you don't want it in PDF
            exportColumns = exportColumns
                .Where(c => c.Name.ToLower() != "id")
                .ToList();

            if (exportColumns.Count == 0)
            {
                MessageBox.Show("No columns available to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Page usable width تقريباً A4 landscape/portrait based on margins
            // Since your current design is portrait, keep widths compact.
            // We'll assign width dynamically.
            double totalWidthCm = 19.0; // approx usable width with 1cm margins on A4
            double defaultColWidth = totalWidthCm / exportColumns.Count;

            // Create columns
            foreach (var col in exportColumns)
            {
                double width = GetColumnWidthCm(col, defaultColWidth);
                dataTable.AddColumn(Unit.FromCentimeter(width));
            }

            // =========================
            // Header row (same as grid)
            // =========================
            Row header = dataTable.AddRow();
            header.HeadingFormat = true;
            header.Format.Font.Bold = true;
            header.Format.Font.Size = 9;

            for (int i = 0; i < exportColumns.Count; i++)
            {
                var gridCol = exportColumns[i];
                var cell = header.Cells[i];
                cell.AddParagraph(gridCol.HeaderText);
                cell.Format.Font.Bold = true;
                cell.Format.Font.Size = 9;
                cell.Borders.Bottom.Width = 0.5;
                cell.VerticalAlignment = VerticalAlignment.Center;
                cell.Format.Alignment = IsNumericColumn(gridCol)
                    ? ParagraphAlignment.Right
                    : ParagraphAlignment.Left;
            }

            // =========================
            // Data rows (same as grid)
            // =========================
            decimal totalAmount = 0m;
            int totalColumnIndex = -1;

            // Find amount/total column for total row
            for (int i = 0; i < exportColumns.Count; i++)
            {
                string colName = exportColumns[i].Name.ToLower();
                string headerText = exportColumns[i].HeaderText.ToLower();

                if (colName == "total" || colName == "amount" ||
                    headerText == "total" || headerText == "amount")
                {
                    totalColumnIndex = i;
                    break;
                }
            }

            foreach (DataGridViewRow row in dgvSales.Rows)
            {
                if (row.IsNewRow) continue;

                // Optional: skip TOTAL row from grid if already exists
                if (dgvSales.Columns.Contains("Account"))
                {
                    string accountText = row.Cells["Account"]?.FormattedValue?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(accountText) &&
                        accountText.ToUpper().Contains("TOTAL"))
                    {
                        continue;
                    }
                }

                Row dataRow = dataTable.AddRow();

                for (int i = 0; i < exportColumns.Count; i++)
                {
                    var gridCol = exportColumns[i];
                    string value = row.Cells[gridCol.Name]?.FormattedValue?.ToString() ?? "";
                    value = value.Replace("◀", "").Trim();

                    Paragraph p = dataRow.Cells[i].AddParagraph(value);
                    p.Format.Font.Size = 9;
                    dataRow.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                    dataRow.Cells[i].Format.Alignment = IsNumericColumn(gridCol)
                        ? ParagraphAlignment.Right
                        : ParagraphAlignment.Left;
                }

                // Calculate total from Amount/Total column
                if (totalColumnIndex >= 0)
                {
                    string amtStr = row.Cells[exportColumns[totalColumnIndex].Name]?.FormattedValue?.ToString() ?? "0";
                    amtStr = amtStr.Replace("◀", "").Trim();

                    decimal amt;
                    if (TryParseDecimal(amtStr, out amt))
                        totalAmount += amt;
                }
            }

            // =========================
            // Total row
            // =========================
            if (totalColumnIndex >= 0)
            {
                Row totalRow = dataTable.AddRow();

                // Put TOTAL label in previous text column if possible
                int labelIndex = Math.Max(0, totalColumnIndex - 1);

                totalRow.Cells[labelIndex].AddParagraph("TOTAL").Format.Font.Bold = true;

                Paragraph totalAmountCell = totalRow.Cells[totalColumnIndex].AddParagraph(totalAmount.ToString("N2"));
                totalAmountCell.Format.Font.Bold = true;
                totalAmountCell.Format.Font.Underline = Underline.Single;
                totalRow.Cells[totalColumnIndex].Format.Alignment = ParagraphAlignment.Right;
            }

            // =========================
            // Export
            // =========================
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Save PDF";
                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveDialog.FileName = "PettyCashDetails.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    renderer.PdfDocument.Save(saveDialog.FileName);
                    Process.Start("explorer.exe", saveDialog.FileName);
                }
            }
        }

        // =========================
        // Helper: detect numeric columns
        // =========================
        private bool IsNumericColumn(DataGridViewColumn col)
        {
            if (col.ValueType == typeof(decimal) ||
                col.ValueType == typeof(double) ||
                col.ValueType == typeof(float) ||
                col.ValueType == typeof(int) ||
                col.ValueType == typeof(long) ||
                col.ValueType == typeof(short))
            {
                return true;
            }

            string name = col.Name.ToLower();
            string header = col.HeaderText.ToLower();

            return name.Contains("amount") ||
                   name.Contains("total") ||
                   name.Contains("balance") ||
                   name.Contains("debit") ||
                   name.Contains("credit") ||
                   header.Contains("amount") ||
                   header.Contains("total") ||
                   header.Contains("balance") ||
                   header.Contains("debit") ||
                   header.Contains("credit");
        }

        // =========================
        // Helper: width by column type
        // =========================
        private double GetColumnWidthCm(DataGridViewColumn col, double defaultWidth)
        {
            string name = col.Name.ToLower();
            string header = col.HeaderText.ToLower();

            if (name.Contains("description") || header.Contains("description"))
                return 5.0;

            if (name.Contains("account") || header.Contains("account"))
                return 4.5;

            if (name.Contains("date") || header.Contains("date"))
                return 2.5;

            if (name.Contains("status") || header.Contains("status"))
                return 2.5;

            if (name.Contains("amount") || name.Contains("total") || name.Contains("balance") ||
                header.Contains("amount") || header.Contains("total") || header.Contains("balance"))
                return 3.0;

            if (name.Contains("reference") || header.Contains("ref"))
                return 2.5;

            if (name.Contains("num") || header.Contains("num"))
                return 2.5;

            if (name == "type" || header == "type")
                return 3.0;

            return defaultWidth;
        }

        // =========================
        // Helper: parse decimal safely
        // =========================
        private bool TryParseDecimal(string input, out decimal value)
        {
            input = (input ?? "").Replace(",", "").Trim();

            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                || decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
        }
        //private void btnPrint_Click(object sender, EventArgs e)
        //{
        //    // Get company name from DB
        //    string companyName = guna2HtmlLabel8.Text;

        //    // Create PDF document
        //    Document doc = new Document();
        //    Section section = doc.AddSection();
        //    section.PageSetup.TopMargin = Unit.FromCentimeter(1);
        //    section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
        //    section.PageSetup.RightMargin = Unit.FromCentimeter(1);
        //    section.PageSetup.BottomMargin = Unit.FromCentimeter(1);

        //    // Header Table (Time/Date + Company Info)
        //    Table headerTable = section.AddTable();
        //    headerTable.Borders.Width = 0;
        //    headerTable.AddColumn("5cm");
        //    headerTable.AddColumn("8cm");
        //    headerTable.AddColumn("5cm");

        //    Row headerRow = headerTable.AddRow();

        //    // Left cell - Time and Date
        //    Paragraph left = headerRow.Cells[0].AddParagraph();
        //    left.Format.Font.Name = "Times New Roman";
        //    left.Format.Font.Size = 10;
        //    left.Format.Alignment = ParagraphAlignment.Left;
        //    headerRow.Cells[0].VerticalAlignment = VerticalAlignment.Top;
        //    left.AddText("Time: " + DateTime.Now.ToString("hh:mm tt") + "\n");
        //    left.AddText("Date: " + DateTime.Now.ToString("dd/MM/yyyy"));

        //    // Center cell - Company Info
        //    Paragraph center = headerRow.Cells[1].AddParagraph();
        //    center.Format.Font.Name = "Times New Roman";
        //    center.Format.Alignment = ParagraphAlignment.Center;
        //    center.AddFormattedText(companyName.ToUpper() + "\n", TextFormat.Bold).Font.Size = 12;
        //    center.AddFormattedText("Petty Cash Summary\n", TextFormat.Bold).Font.Size = 10;
        //    center.AddFormattedText("All Transactions", TextFormat.NotBold).Font.Size = 9;

        //    headerRow.Cells[1].VerticalAlignment = VerticalAlignment.Top;

        //    // Bold black line
        //    Paragraph line = section.AddParagraph();
        //    line.Format.Borders.Bottom.Width = 2;
        //    line.Format.SpaceAfter = "0.5cm";

        //    // Create table for report data
        //    Table dataTable = section.AddTable();
        //    dataTable.Borders.Width = 0.5;
        //    dataTable.Format.Font.Name = "Times New Roman";

        //    if (EnableApproval)
        //    {
        //        // Define table columns with refined widths
        //        dataTable.AddColumn("2.0cm"); // Ref (smaller)
        //        dataTable.AddColumn("2.5cm"); // Date
        //        dataTable.AddColumn("5.0cm"); // Description
        //        dataTable.AddColumn("4.0cm"); // Account
        //        dataTable.AddColumn("3.0cm"); // Amount
        //        dataTable.AddColumn("2.5cm"); // Status

        //        // Header row
        //        Row header = dataTable.AddRow();
        //        string[] headers = { "Ref", "Date", "Description", "Account", "Amount", "Status" };
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            var cell = header.Cells[i];
        //            cell.AddParagraph(headers[i]);
        //            cell.Format.Font.Bold = true;
        //            cell.Format.Font.Size = 9;
        //            cell.Borders.Bottom.Width = 0.5;
        //            cell.Format.Alignment = (i == 4) ? ParagraphAlignment.Right : ParagraphAlignment.Left;
        //        }

        //        // Data rows
        //        decimal totalAmount = 0;
        //        foreach (DataGridViewRow row in dgvSales.Rows)
        //        {
        //            if (row.IsNewRow) continue;

        //            string reference = row.Cells["Reference"]?.Value?.ToString() ?? "";
        //            string date = row.Cells["Date"]?.Value?.ToString() ?? "";
        //            string description = row.Cells["Description"]?.Value?.ToString() ?? "";
        //            string account = row.Cells["Account"]?.Value?.ToString() ?? "";
        //            string amountStr = row.Cells["Amount"]?.Value?.ToString()?.Replace("◀", "").Trim() ?? "0";
        //            string status = row.Cells["Status"]?.Value?.ToString() ?? "";
        //            //string status = string.IsNullOrEmpty(_status) ? "No" : int.Parse(_status) == 1 ? "Ok" : "No";

        //            if (account.ToUpper().Contains("TOTAL")) continue;

        //            Row dataRow = dataTable.AddRow();
        //            dataRow.Cells[0].AddParagraph(reference).Format.Font.Size = 9;
        //            dataRow.Cells[1].AddParagraph(date).Format.Font.Size = 9;
        //            dataRow.Cells[2].AddParagraph(description).Format.Font.Size = 9;
        //            dataRow.Cells[3].AddParagraph(account).Format.Font.Size = 9;
        //            dataRow.Cells[4].AddParagraph(amountStr).Format.Alignment = ParagraphAlignment.Right;
        //            dataRow.Cells[5].AddParagraph(status).Format.Font.Size = 9;

        //            decimal amt;
        //            decimal.TryParse(amountStr, out amt);
        //            totalAmount += amt;
        //        }

        //        // Total row
        //        Row totalRow = dataTable.AddRow();
        //        totalRow.Cells[3].AddParagraph("TOTAL").Format.Font.Bold = true;
        //        Paragraph totalAmountCell = totalRow.Cells[4].AddParagraph(totalAmount.ToString("N2"));
        //        totalAmountCell.Format.Font.Bold = true;
        //        totalAmountCell.Format.Font.Underline = Underline.Single;
        //        totalRow.Cells[4].Format.Alignment = ParagraphAlignment.Right;
        //        totalRow.Cells[5].AddParagraph(""); // Leave Status cell blank

        //    }
        //    else
        //    {
        //        dataTable.AddColumn("3.5cm"); // Type
        //        dataTable.AddColumn("2.5cm"); // Date
        //        dataTable.AddColumn("2.5cm"); // Num
        //        dataTable.AddColumn("4.5cm"); // Account
        //        dataTable.AddColumn("3cm");   // Amount
        //        dataTable.AddColumn("3cm");   // Balance

        //        // Header row
        //        Row header = dataTable.AddRow();
        //        string[] headers = { "Type", "Date", "Num", "Account", "Amount", "Balance" };
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            var cell = header.Cells[i];
        //            cell.AddParagraph(headers[i]);
        //            cell.Format.Font.Bold = true;
        //            cell.Format.Font.Size = 9;
        //            cell.Borders.Bottom.Width = 0.5;
        //            cell.Format.Alignment = (i >= 4) ? ParagraphAlignment.Right : ParagraphAlignment.Left;
        //        }

        //        // Data rows
        //        decimal totalAmount = 0;
        //        foreach (DataGridViewRow row in dgvSales.Rows)
        //        {
        //            if (row.IsNewRow) continue;

        //            string type = row.Cells["Type"]?.Value?.ToString() ?? "";
        //            string date = row.Cells["Date"]?.Value?.ToString() ?? "";
        //            string num = row.Cells["Num"]?.Value?.ToString() ?? "";
        //            string account = row.Cells["Account"]?.Value?.ToString() ?? "";
        //            string amountStr = row.Cells["Total"]?.Value?.ToString()?.Replace("◀", "").Trim() ?? "0";
        //            string balance = row.Cells["Balance"]?.Value?.ToString()?.Replace("◀", "").Trim() ?? "0";

        //            if (account.ToUpper().Contains("TOTAL")) continue;

        //            Row dataRow = dataTable.AddRow();
        //            dataRow.Cells[0].AddParagraph(type).Format.Font.Size = 9;
        //            dataRow.Cells[1].AddParagraph(date).Format.Font.Size = 9;
        //            dataRow.Cells[2].AddParagraph(num).Format.Font.Size = 9;
        //            dataRow.Cells[3].AddParagraph(account).Format.Font.Size = 9;
        //            dataRow.Cells[4].AddParagraph(amountStr).Format.Alignment = ParagraphAlignment.Right;
        //            dataRow.Cells[5].AddParagraph(balance).Format.Alignment = ParagraphAlignment.Right;
        //            decimal amt;
        //            decimal.TryParse(amountStr, out amt);
        //            totalAmount += amt;
        //        }

        //        // Total row
        //        Row totalRow = dataTable.AddRow();
        //        totalRow.Cells[3].AddParagraph("TOTAL").Format.Font.Bold = true;
        //        Paragraph totalAmountCell = totalRow.Cells[4].AddParagraph(totalAmount.ToString("N2"));
        //        totalAmountCell.Format.Font.Bold = true;
        //        totalAmountCell.Format.Font.Underline = Underline.Single;
        //        totalRow.Cells[4].Format.Alignment = ParagraphAlignment.Right;
        //        totalRow.Cells[5].AddParagraph(""); // Leave balance cell blank
        //    }

        //    // Export
        //    PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
        //    renderer.Document = doc;
        //    renderer.RenderDocument();

        //    using (SaveFileDialog saveDialog = new SaveFileDialog())
        //    {
        //        saveDialog.Title = "Save PDF";
        //        saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
        //        saveDialog.FileName = "PettyCashDetails.pdf";

        //        if (saveDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            renderer.PdfDocument.Save(saveDialog.FileName);
        //            Process.Start("explorer.exe", saveDialog.FileName);
        //        }
        //    }
        //}

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            int eId = cmbEmployee.SelectedValue != null ? int.Parse(cmbEmployee.SelectedValue.ToString()) : 0;
            LoadData(eId);
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lstAccountSuggestions_Click(object sender, EventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem != null && lstAccountSuggestions.Tag is Guna2TextBox targetTextBox)
            {
                string selected = lstAccountSuggestions.SelectedItem.ToString();
                string selectedCode = "";// selected.Split('-')[0].Trim();

                if (lstAccountSuggestions.SelectedItem is ItemSuggestion item)
                {
                    selectedCode = item.Code;
                }

                targetTextBox.Text = selectedCode;
                lstAccountSuggestions.Visible = false;

                // Trigger TextChanged manually if needed
                targetTextBox.Focus();
                targetTextBox.SelectionStart = targetTextBox.Text.Length;

                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id from tbl_employee where code =@code",
                    ClinicDatabase.CreateParameter("code", selectedCode)))
                    if (reader.Read())
                        cmbEmployee.SelectedValue = int.Parse(reader["id"].ToString());
            }
        }

        private void txtEmployeeCode_TextChanged(object sender, EventArgs e)
        {
            if (txtEmployeeCode.Focused)
            {
                string input = txtEmployeeCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT code, name FROM tbl_employee 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameter("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion { Code = row["code"].ToString(), Name = row["name"].ToString() });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtEmployeeCode.Parent.PointToScreen(txtEmployeeCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtEmployeeCode.Height,
                        txtEmployeeCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtEmployeeCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void cmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployee.SelectedValue != null && cmbEmployee.SelectedValue is int empId)
            {
                if (cmbEmployee.Focused || txtEmployeeCode.Text.Trim() != "")
                {
                    int eId = cmbEmployee.SelectedValue is int id ? id : 0;
                    if (eId > 0)
                    {
                        LoadData(eId);
                    }
                }
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            int eId = cmbEmployee.SelectedValue != null ? int.Parse(cmbEmployee.SelectedValue.ToString()) : 0;
            LoadData(eId);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dateTimePicker1, dateTimePicker2);
                LoadData();
            }
        }
    }
}
