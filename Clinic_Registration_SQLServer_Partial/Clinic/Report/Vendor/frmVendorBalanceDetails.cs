using Clinic;
using Clinic.global_classes;
using Clinic_DataAccess;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmVendorBalanceDetails : Form
    {
        int id;
        public frmVendorBalanceDetails(int _id = 0)
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            headerUC1.FormText = this.Text;
            this.id = _id;
        }

        private void frmVendorBalanceDetails_Load(object sender, EventArgs e)
        {
            btnPrint.Text = "Print ▼";
            LoadSalesData();
            DateTime dated = DateTime.Now;
            guna2HtmlLabel11.Text = dated.TimeOfDay.ToString();
            guna2HtmlLabel11.Text = dated.Date.ToShortDateString();
            loadCompany();
            LoadSalesData();
            ApplyVendorArabicUI();
        }
        // UI localization only. Column names and transaction values remain in English
        // because navigation and export routines use them as internal identifiers.
        private bool VendorArabicEnabled()
        {
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            return language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyVendorArabicUI()
        {
            bool arabic = VendorArabicEnabled();
            headerUC1.ArabicLayout = arabic;
            if (!arabic) return;

            this.Text = "تفاصيل أرصدة الموردين";
            headerUC1.FormText = this.Text;
            TranslateVendorControls(this);

            // The company name comes from the database. Translate only the
            // literal placeholder, never an actual registered company name.
            if (string.Equals(guna2HtmlLabel8.Text.Trim(), "Company Name",
                StringComparison.OrdinalIgnoreCase))
                guna2HtmlLabel8.Text = "اسم الشركة";

            btnPrint.Text = Clinic.TranslationClass.Translate("Print ▼", true);
            ApplyVendorGridHeaders();
        }

        private void TranslateVendorControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control == headerUC1 || control == guna2HtmlLabel11) continue;

                if (control is Label || control is Button || control is CheckBox ||
                    control is RadioButton || control is GroupBox ||
                    control is Guna.UI2.WinForms.Guna2HtmlLabel ||
                    control is Guna.UI2.WinForms.Guna2Button)
                {
                    string english = control.Text;
                    if (!string.IsNullOrWhiteSpace(english))
                    {
                        string key = english.Trim();
                        string translated;
                        switch (key)
                        {
                            case "Vendor Balance Detail":
                            case "Vendor Balance Details":
                                translated = "تفاصيل أرصدة الموردين";
                                break;
                            case "Company Name":
                                translated = "اسم الشركة";
                                break;
                            default:
                                translated = Clinic.TranslationClass.Translate(key, true);
                                break;
                        }
                        if (translated != key) control.Text = translated;
                    }
                    // Keep the Designer's centered heading alignment unchanged.
                    control.RightToLeft = RightToLeft.Yes;
                }
                if (control.HasChildren) TranslateVendorControls(control);
            }
        }

        private void ApplyVendorGridHeaders()
        {
            bool arabic = VendorArabicEnabled();
            dgvSales.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            if (!arabic) return;

            foreach (DataGridViewColumn column in dgvSales.Columns)
            {
                string key = column.Name;
                string caption = key == "Account" ? "الحساب" :
                    Clinic.TranslationClass.Translate(key, true);
                column.HeaderText = caption;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string companyName = guna2HtmlLabel8.Text.Trim();

            Document doc = new Document();
            Section section = doc.AddSection();

            // Margins
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);

            // Header
            Paragraph header = section.AddParagraph();
            header.AddFormattedText(companyName.ToUpper() + "\n", TextFormat.Bold);
            header.AddFormattedText("Vendor Balance Detail\n", TextFormat.Bold);
            header.AddFormattedText("From " + dtpFrom.Value.ToShortDateString() +
                                    " To " + dtpTo.Value.ToShortDateString(), TextFormat.NotBold);
            header.Format.Alignment = ParagraphAlignment.Center;
            header.Format.SpaceAfter = "0.5cm";

            // Table
            Table table = section.AddTable();
            table.Borders.Width = 0.75;
            table.Format.Font.Name = "Times New Roman";
            table.Format.Font.Size = 9;

            //dgvSales.Columns.Add("id", "id");
            //dgvSales.Columns.Add("Type", "Type");
            //dgvSales.Columns.Add("Date", "Date");
            //dgvSales.Columns.Add("Num", "Num");
            //dgvSales.Columns.Add("Account", "Account");
            //dgvSales.Columns.Add("Amount", "Amount");
            //dgvSales.Columns.Add("Balance", "Balance");
            table.AddColumn("3cm"); // Type
            table.AddColumn("2.5cm"); // Date
            table.AddColumn("3cm"); // Num
            table.AddColumn("7cm"); // Account
            table.AddColumn("3cm"); // Amount
            table.AddColumn("3cm"); // Balance

            Row th = table.AddRow();
            th.Shading.Color = Colors.LightGray;
            th.Format.Font.Bold = true;
            th.Cells[0].AddParagraph("Type");
            th.Cells[1].AddParagraph("Date");
            th.Cells[2].AddParagraph("Num");
            th.Cells[3].AddParagraph("Account");
            th.Cells[4].AddParagraph("Amount").Format.Alignment = ParagraphAlignment.Right;
            th.Cells[5].AddParagraph("Balance").Format.Alignment = ParagraphAlignment.Right;

            decimal totalAmount = 0, totalBalance = 0;
            int sn = 1;

            foreach (DataGridViewRow row in dgvSales.Rows)
            {
                if (row.IsNewRow) continue;

                string type = row.Cells["Type"].Value?.ToString() ?? "";
                string date = row.Cells["Date"].Value?.ToString() ?? "";
                string num = row.Cells["Num"].Value?.ToString() ?? "";
                string account = row.Cells["Account"].Value?.ToString() ?? "";
                string amountStr = row.Cells["Amount"].Value?.ToString() ?? "";
                string balanceStr = row.Cells["Balance"].Value?.ToString() ?? "";

                if (account.ToUpper().Contains("TOTAL")) continue;

                decimal amount = 0;
                decimal.TryParse(amountStr.Trim(), out amount);
                totalAmount += amount;
                decimal balance = 0;
                decimal.TryParse(balanceStr.Trim(), out balance);
                totalBalance += balance;

                Row r = table.AddRow();
                r.Cells[0].AddParagraph(type);
                r.Cells[1].AddParagraph(date);
                r.Cells[2].AddParagraph(num);
                r.Cells[3].AddParagraph(account);
                r.Cells[4].AddParagraph(amount.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
                r.Cells[5].AddParagraph(balance.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;

                sn++;
            }

            // Total row
            Row totalRow = table.AddRow();
            totalRow.Cells[0].MergeRight = 1;
            totalRow.Cells[0].AddParagraph("TOTAL").Format.Font.Bold = true;
            totalRow.Cells[4].AddParagraph(totalAmount.ToString("N2")).Format.Font.Bold = true;
            totalRow.Cells[4].Format.Alignment = ParagraphAlignment.Right;
            totalRow.Cells[5].AddParagraph(totalBalance.ToString("N2")).Format.Font.Bold = true;
            totalRow.Cells[5].Format.Alignment = ParagraphAlignment.Right;

            // Render PDF
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "VendorBalanceDetail.pdf");
            renderer.PdfDocument.Save(filePath);
            Process.Start("explorer.exe", filePath);
        }

        private void LoadSalesData()
        {
            dgvSales.Rows.Clear();

            if (dgvSales.Columns.Count == 0)
            {
                dgvSales.Columns.Add("id", "id");
                dgvSales.Columns.Add("Type", "Type");
                dgvSales.Columns.Add("Date", "Date");
                dgvSales.Columns.Add("Num", "Num");
                dgvSales.Columns.Add("Account", "Account");
                dgvSales.Columns.Add("Amount", "Amount");
                dgvSales.Columns.Add("Balance", "Balance");
                LocalizationManager.LocalizeDataGridViewHeaders(dgvSales);

                dgvSales.Columns["Account"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvSales.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSales.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvSales.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSales.Columns["Balance"].DefaultCellStyle.Format = "N2";
                dgvSales.Columns["Amount"].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Regular);
                dgvSales.Columns["Account"].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Regular);

                dgvSales.Columns["id"].Visible = false;
                dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvSales.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                dgvSales.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                dgvSales.EnableHeadersVisualStyles = false;
                dgvSales.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvSales.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                dgvSales.GridColor = System.Drawing.Color.LightGray;
                dgvSales.BorderStyle = System.Windows.Forms.BorderStyle.None;
                dgvSales.CellBorderStyle = DataGridViewCellBorderStyle.None;
                dgvSales.RowHeadersVisible = false;
            }

            decimal totalAmount = 0, totalBalance = 0;

            List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    ClinicDatabase.CreateParameterNew("id", id)
                };

            string dateFilter = "";
            if (!chkDate.Checked)
            {
                dateFilter = " AND tbl_transaction.date >= @dateFrom AND tbl_transaction.date <= @dateTo";
                parameters.Add(ClinicDatabase.CreateParameterNew("dateFrom", dtpFrom.Value.Date));
                parameters.Add(ClinicDatabase.CreateParameterNew("dateTo", dtpTo.Value.Date.AddDays(1).AddSeconds(-1)));
            }

            string query = $@"
                            SELECT 
                                DATE_FORMAT(tbl_transaction.date, '%M %d %Y') AS date,
                                tbl_transaction.transaction_id,
                                CASE 
                                    WHEN tbl_transaction.type = 'Vendor Payment' THEN 
                                        (SELECT tbl_payment_voucher.code 
                                         FROM tbl_payment_voucher 
                                         WHERE tbl_payment_voucher.state = 0 AND tbl_payment_voucher.id = tbl_transaction.transaction_id)
                                    WHEN tbl_transaction.type = 'Purchase Invoice' OR tbl_transaction.type = 'Purchase Invoice Cash' THEN 
                                        (SELECT tbl_purchase.invoice_id 
                                         FROM tbl_purchase 
                                         WHERE tbl_purchase.state = 0 AND tbl_purchase.id = tbl_transaction.transaction_id)
                                    WHEN tbl_transaction.type = 'Purchase Return Invoice' THEN 
                                        (SELECT tbl_purchase_return.invoice_id 
                                         FROM tbl_purchase_return 
                                         WHERE tbl_purchase_return.state = 0 AND tbl_purchase_return.id = tbl_transaction.transaction_id)
                                    ELSE ''
                                END AS `Num`,
                                tbl_transaction.type AS `Type`,  
                                tbl_coa_level_4.name AS 'A/C NAME',
                                (tbl_transaction.credit - tbl_transaction.debit) AS `AMOUNT`,
                                SUM(tbl_transaction.debit - tbl_transaction.credit) 
                                    OVER (PARTITION BY tbl_transaction.hum_id ORDER BY tbl_transaction.date) AS `BALANCE`
                            FROM 
                                tbl_transaction  
                            INNER JOIN tbl_coa_level_4 
                                ON tbl_transaction.account_id = tbl_coa_level_4.id 
                            WHERE 
                                tbl_transaction.hum_id = @id
                                AND tbl_transaction.type IN (
                                    'Vendor Payment', 
                                    'Purchase Invoice',
                                    'Vendor Opening Balance',
                                    'Check Cancel (Vendor)',
                                    'Purchase Return Invoice', 
                                    'Debit Note',
                                    'PDC Payable'
                                )
                                {dateFilter}
                            ORDER BY tbl_transaction.date;";

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, parameters.ToArray()))
            {
                while (reader.Read())
                {
                    if (reader["Date"] == DBNull.Value)
                        continue;

                    decimal amount = Convert.ToDecimal(reader["AMOUNT"]);
                    decimal balance = Convert.ToDecimal(reader["BALANCE"]);

                    int rowIndex = dgvSales.Rows.Add(
                        reader["transaction_id"].ToString(),
                        reader["Type"].ToString(),
                        reader["date"].ToString(),
                        reader["Num"].ToString(),
                        reader["A/C NAME"].ToString(),
                        amount.ToString("N2"),
                        balance.ToString("N2") + " ◀"
                    );

                    dgvSales.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    dgvSales.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Regular);

                    totalAmount += amount;
                    totalBalance += balance;
                }
            }

            // Add total row
            int totalRow = dgvSales.Rows.Add(null, null, null, null, "TOTAL", totalAmount.ToString("N2"), "");
            dgvSales.Rows[totalRow].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold | FontStyle.Underline);
            dgvSales.Rows[totalRow].DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
            dgvSales.Rows[totalRow].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvSales.Rows[totalRow].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvSales.Rows[totalRow].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            ApplyVendorGridHeaders();
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
                    var _type = dgvSales.Rows[e.RowIndex].Cells["Type"].Value.ToString();
                    if (_type.Contains("Purchase"))
                    {
                        frmMain.openChildForm(new frmPurchase(_id, "", id));
                    }
                    else if (_type.Contains("Vendor Payment"))
                    {
                        frmMain.openChildForm(new frmViewPaymentVoucher(id));
                    }
                    else if (_type == "Vendor Opening Balance")
                    {
                        frmMain.openChildForm(new frmTransactionJournal(_id, _type, _id.ToString()));
                    }
                }
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = dtpTo.Enabled = !chkDate.Checked;
            LoadSalesData();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadSalesData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadSalesData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dtpFrom, dtpTo);
                LoadSalesData();
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = "VendorBalanceDetail.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Vendor Balance Detail";

                    // Header row (Date)
                    Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range["A1", "C1"];
                    headerRange.Merge();
                    headerRange.Value = "Date: " + DateTime.Now.ToString("MMM dd, yyyy");
                    headerRange.Font.Bold = true;
                    headerRange.Font.Name = "Times New Roman";
                    headerRange.Font.Size = 10;
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    //dgvSales.Columns.Add("id", "id");
                    //dgvSales.Columns.Add("Type", "Type");
                    //dgvSales.Columns.Add("Date", "Date");
                    //dgvSales.Columns.Add("Num", "Num");
                    //dgvSales.Columns.Add("Account", "Account");
                    //dgvSales.Columns.Add("Amount", "Amount");
                    //dgvSales.Columns.Add("Balance", "Balance");
                    // Column headers
                    worksheet.Cells[2, 1] = "Type";
                    worksheet.Cells[2, 2] = "Date";
                    worksheet.Cells[2, 3] = "Num";
                    worksheet.Cells[2, 4] = "Account";
                    worksheet.Cells[2, 5] = "Amount";
                    worksheet.Cells[2, 6] = "Balance";

                    for (int i = 1; i <= 6; i++)
                    {
                        var headerCell = worksheet.Cells[2, i];
                        headerCell.Font.Bold = true;
                        headerCell.Font.Name = "Times New Roman";
                        headerCell.Font.Size = 10;
                        headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        headerCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    }

                    int rowIndex = 6;
                    decimal totalBalance = 0, totalAmount = 0;
                    foreach (DataGridViewRow row in dgvSales.Rows)
                    {
                        if (row.IsNewRow) continue;
                        worksheet.Cells[rowIndex, 1] = row.Cells["Type"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 2] = row.Cells["Date"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 3] = row.Cells["Num"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 4] = row.Cells["Account"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 5] = row.Cells["Amount"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 6] = row.Cells["Balance"].Value?.ToString() ?? "";

                        decimal amount;
                        decimal.TryParse(row.Cells["Debit"].Value?.ToString(), out amount);
                        decimal balanceAmount;
                        decimal.TryParse(row.Cells["Balance"].Value?.ToString(), out balanceAmount);
                        totalBalance += balanceAmount;
                        totalAmount += amount;
                        worksheet.Cells[rowIndex, 5] = amount;
                        worksheet.Cells[rowIndex, 6] = balanceAmount;

                        // Formatting for each row
                        for (int i = 1; i <= 6; i++)
                        {
                            var dataCell = worksheet.Cells[rowIndex, i];
                            dataCell.Font.Name = "Times New Roman";
                            dataCell.Font.Size = 9;
                            if (i >= 5) // Debit, Credit, Balance columns
                            {
                                dataCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                                dataCell.NumberFormat = "#,##0.00";
                            }
                            else
                            {
                                dataCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                            }
                        }

                        rowIndex++;
                    }
                    // Add TOTAL row
                    worksheet.Cells[rowIndex, 1] = "TOTAL";
                    worksheet.Cells[rowIndex, 1].Font.Bold = true;
                    worksheet.Cells[rowIndex, 5] = totalAmount;
                    worksheet.Cells[rowIndex, 5].Font.Bold = true;
                    worksheet.Cells[rowIndex, 5].Font.Underline = Microsoft.Office.Interop.Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                    worksheet.Cells[rowIndex, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    worksheet.Cells[rowIndex, 6] = totalBalance;
                    worksheet.Cells[rowIndex, 6].Font.Bold = true;
                    worksheet.Cells[rowIndex, 6].Font.Underline = Microsoft.Office.Interop.Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                    worksheet.Cells[rowIndex, 6].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

                    worksheet.Columns.AutoFit();

                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close(false);
                    excelApp.Quit();

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
