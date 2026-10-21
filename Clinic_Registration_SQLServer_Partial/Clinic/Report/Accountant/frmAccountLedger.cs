using Clinic_DataAccess;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamyProject.Localization;
using Excel = Microsoft.Office.Interop.Excel;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;

namespace YamyProject.UI.Reports.Accountant
{
    public partial class frmAccountLedger : Form
    {
        public frmAccountLedger()
        {
            InitializeComponent();

            //LocalizationManager.LocalizeForm(this);
        }


        // ============================================================
        // FORM LOAD
        // ============================================================
        private void frmAccountLedger_Load(
            object sender,
            EventArgs e)
        {
            SetupGrid();

            LoadAccounts();

            // Current month
            dtpFrom.Value =
                new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1);

            dtpTo.Value =
                DateTime.Today;

            ResetSummary();
        }


        // ============================================================
        // GRID
        // ============================================================
        private void SetupGrid()
        {
            dgvLedger.Columns.Clear();

            dgvLedger.Columns.Add(
                "TransactionId",
                "");

            dgvLedger.Columns.Add(
                "Date",
                "Date");

            dgvLedger.Columns.Add(
                "Reference",
                "Reference");

            dgvLedger.Columns.Add(
                "Transaction",
                "Transaction");

            dgvLedger.Columns.Add(
                "Description",
                "Description");

            dgvLedger.Columns.Add(
                "Debit",
                "Debit");

            dgvLedger.Columns.Add(
                "Credit",
                "Credit");

            dgvLedger.Columns.Add(
                "Balance",
                "Balance");


            dgvLedger.Columns["TransactionId"].Visible =
                false;


            // Widths
            dgvLedger.Columns["Date"].FillWeight = 80;
            dgvLedger.Columns["Reference"].FillWeight = 80;
            dgvLedger.Columns["Transaction"].FillWeight = 110;
            dgvLedger.Columns["Description"].FillWeight = 220;
            dgvLedger.Columns["Debit"].FillWeight = 80;
            dgvLedger.Columns["Credit"].FillWeight = 80;
            dgvLedger.Columns["Balance"].FillWeight = 100;


            // Right align amounts
            dgvLedger.Columns["Debit"]
                .DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            dgvLedger.Columns["Credit"]
                .DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            dgvLedger.Columns["Balance"]
                .DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;


            dgvLedger.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvLedger.AllowUserToAddRows = false;
            dgvLedger.AllowUserToDeleteRows = false;

            dgvLedger.ReadOnly = true;

            dgvLedger.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvLedger.MultiSelect = false;


            LocalizationManager
                .LocalizeDataGridViewHeaders(dgvLedger);
        }


        // ============================================================
        // LOAD ACCOUNTS
        // ============================================================
        private void LoadAccounts()
        {
            try
            {
                string sql = @"
                    SELECT
                        id,
                        CONCAT(code, ' - ', name) AS account_name
                    FROM tbl_coa_level_4
                    WHERE id IN
                    (
                        SELECT DISTINCT account_id
                        FROM tbl_transaction
                    )
                    ORDER BY code;
                ";


                DataTable dt =
                    ClinicDatabase.ExecuteDataTable(sql);


                DataRow firstRow =
                    dt.NewRow();

                firstRow["id"] = 0;
                firstRow["account_name"] =
                    "-- Select Account --";

                dt.Rows.InsertAt(firstRow, 0);


                cmbAccount.DataSource = dt;

                cmbAccount.DisplayMember =
                    "account_name";

                cmbAccount.ValueMember =
                    "id";

                cmbAccount.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load accounts.\n\n" +
                    ex.Message,
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // ============================================================
        // VIEW
        // ============================================================
        private void btnView_Click(
            object sender,
            EventArgs e)
        {
            if (cmbAccount.SelectedValue == null)
            {
                MessageBox.Show(
                    "Please select an account.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }


            int accountId;

            if (!int.TryParse(
                cmbAccount.SelectedValue.ToString(),
                out accountId))
            {
                return;
            }


            if (accountId <= 0)
            {
                MessageBox.Show(
                    "Please select an account.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }


            if (dtpFrom.Value.Date >
                dtpTo.Value.Date)
            {
                MessageBox.Show(
                    "From Date cannot be greater than To Date.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            LoadLedger();
        }


        // ============================================================
        // LOAD LEDGER
        // ============================================================
        private void LoadLedger()
        {
            try
            {
                dgvLedger.Rows.Clear();


                int accountId =
                    Convert.ToInt32(
                        cmbAccount.SelectedValue);


                DateTime fromDate =
                    dtpFrom.Value.Date;

                DateTime toDate =
                    dtpTo.Value.Date;


                // ====================================================
                // HEADER
                // ====================================================
                lblSelectedAccount.Text =
                    cmbAccount.Text;

                lblPeriod.Text =
                    "Period: " +
                    fromDate.ToString("dd MMM yyyy") +
                    " - " +
                    toDate.ToString("dd MMM yyyy");


                // ====================================================
                // OPENING BALANCE
                // ====================================================
                decimal openingBalance =
                    GetOpeningBalance(
                        accountId,
                        fromDate);


                decimal runningBalance =
                    openingBalance;


                // ====================================================
                // OPENING ROW
                // ====================================================
                int openingRowIndex =
                    dgvLedger.Rows.Add(
                        null,
                        "",
                        "",
                        "",
                        "Opening Balance",
                        "",
                        "",
                        FormatBalance(openingBalance)
                    );


                DataGridViewRow openingRow =
                    dgvLedger.Rows[openingRowIndex];


                openingRow.DefaultCellStyle.Font =
                    new System.Drawing.Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Bold);

                openingRow.DefaultCellStyle.BackColor =
                    System.Drawing.Color.FromArgb(
                        248,
                        250,
                        252);

                openingRow.DefaultCellStyle.ForeColor =
                    System.Drawing.Color.FromArgb(
                        71,
                        85,
                        105);


                // ====================================================
                // TRANSACTIONS
                // ====================================================
                string sql = @"
                    SELECT
                        t.id,

                        t.transaction_id,

                        t.date,

                        t.type,

                        t.t_type,

                        t.description,

                        IFNULL(t.debit, 0) AS debit,

                        IFNULL(t.credit, 0) AS credit

                    FROM tbl_transaction t

                    WHERE t.account_id = @account_id

                      AND t.date >= @date_from

                      AND t.date <= @date_to

                    ORDER BY
                        t.date,
                        t.id;
                ";


                decimal totalDebit = 0m;
                decimal totalCredit = 0m;

                int transactionCount = 0;


                using (MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(
                        sql,

                        ClinicDatabase.CreateParameterNew(
                            "account_id",
                            accountId),

                        ClinicDatabase.CreateParameterNew(
                            "date_from",
                            fromDate),

                        ClinicDatabase.CreateParameterNew(
                            "date_to",
                            toDate)))
                {
                    while (reader.Read())
                    {
                        decimal debit =
                            reader["debit"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(
                                reader["debit"]);


                        decimal credit =
                            reader["credit"] == DBNull.Value
                            ? 0m
                            : Convert.ToDecimal(
                                reader["credit"]);


                        totalDebit += debit;
                        totalCredit += credit;


                        runningBalance +=
                            debit - credit;


                        string type =
                            reader["type"].ToString();


                        string transactionId =
                            reader["transaction_id"]
                            .ToString();


                        string reference =
                            GetReference(
                                type,
                                transactionId,
                                reader["description"]
                                .ToString());


                        int rowIndex =
                            dgvLedger.Rows.Add(
                                reader["id"].ToString(),

                                Convert
                                    .ToDateTime(
                                        reader["date"])
                                    .ToString("dd/MM/yyyy"),

                                reference,

                                type,

                                reader["description"]
                                    .ToString(),

                                debit == 0
                                    ? "-"
                                    : debit.ToString("N2"),

                                credit == 0
                                    ? "-"
                                    : credit.ToString("N2"),

                                FormatBalance(
                                    runningBalance)
                            );


                        DataGridViewRow row =
                            dgvLedger.Rows[rowIndex];


                        row.DefaultCellStyle.Font =
                            new System.Drawing.Font(
                                "Segoe UI",
                                9F,
                                FontStyle.Regular);


                        transactionCount++;
                    }
                }


                // ====================================================
                // CLOSING BALANCE
                // ====================================================
                decimal closingBalance =
                    openingBalance
                    + totalDebit
                    - totalCredit;


                // ====================================================
                // TOTAL ROW
                // ====================================================
                int totalRowIndex =
                    dgvLedger.Rows.Add(
                        null,
                        "",
                        "",
                        "",
                        "Period Total",

                        totalDebit.ToString("N2"),

                        totalCredit.ToString("N2"),

                        FormatBalance(
                            closingBalance)
                    );


                DataGridViewRow totalRow =
                    dgvLedger.Rows[totalRowIndex];


                totalRow.DefaultCellStyle.Font =
                    new System.Drawing.Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Bold);


                totalRow.DefaultCellStyle.BackColor =
                    System.Drawing.Color.FromArgb(
                        241,
                        245,
                        249);


                // ====================================================
                // SUMMARY CARDS
                // ====================================================
                lblOpeningValue.Text =
                    FormatCurrencyBalance(
                        openingBalance);


                lblDebitValue.Text =
                    "AED " +
                    totalDebit.ToString("N2");


                lblCreditValue.Text =
                    "AED " +
                    totalCredit.ToString("N2");


                lblClosingValue.Text =
                    FormatCurrencyBalance(
                        closingBalance);


                lblTransactionCount.Text =
                    transactionCount == 1
                    ? "1 transaction"
                    : transactionCount +
                      " transactions";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load account ledger.\n\n" +
                    ex.Message,
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // ============================================================
        // OPENING BALANCE
        // ============================================================
        private decimal GetOpeningBalance(
            int accountId,
            DateTime fromDate)
        {
            string sql = @"
                SELECT
                    IFNULL(SUM(debit), 0)
                    -
                    IFNULL(SUM(credit), 0)
                    AS opening_balance

                FROM tbl_transaction

                WHERE account_id = @account_id

                  AND date < @from_date;
            ";


            object result =
                ClinicDatabase.ExecuteScalar(
                    sql,

                    ClinicDatabase.CreateParameterNew(
                        "account_id",
                        accountId),

                    ClinicDatabase.CreateParameterNew(
                        "from_date",
                        fromDate));


            if (result == null ||
                result == DBNull.Value)
            {
                return 0m;
            }


            return Convert.ToDecimal(result);
        }


        // ============================================================
        // FORMAT BALANCE
        // ============================================================
        private string FormatBalance(
            decimal balance)
        {
            if (balance > 0)
            {
                return balance.ToString("N2") +
                       " Dr";
            }


            if (balance < 0)
            {
                return Math.Abs(balance)
                           .ToString("N2") +
                       " Cr";
            }


            return "0.00";
        }


        // ============================================================
        // FORMAT CURRENCY BALANCE
        // ============================================================
        private string FormatCurrencyBalance(
            decimal balance)
        {
            if (balance > 0)
            {
                return "AED " +
                       balance.ToString("N2") +
                       " Dr";
            }


            if (balance < 0)
            {
                return "AED " +
                       Math.Abs(balance)
                           .ToString("N2") +
                       " Cr";
            }


            return "AED 0.00";
        }


        // ============================================================
        // REFERENCE
        // ============================================================
        private string GetReference(
            string type,
            string transactionId,
            string description)
        {
            /*
             * Your tbl_transaction currently stores
             * transaction_id but your user wants to see:
             *
             * SI-0010
             * PI-0002
             *
             * rather than:
             *
             * 10
             * 2
             *
             * First try to extract the document number
             * already present in your description.
             */


            if (!string.IsNullOrWhiteSpace(description))
            {
                string upper =
                    description.ToUpper();


                string[] prefixes =
                {
                    "SI-",
                    "PI-",
                    "RV-",
                    "PV-",
                    "CN-",
                    "DN-"
                };


                foreach (string prefix in prefixes)
                {
                    int index =
                        upper.IndexOf(prefix);


                    if (index >= 0)
                    {
                        int end = index;


                        while (
                            end < description.Length &&
                            !char.IsWhiteSpace(
                                description[end]) &&
                            description[end] != ',' &&
                            description[end] != ')' &&
                            description[end] != ';')
                        {
                            end++;
                        }


                        return description
                            .Substring(
                                index,
                                end - index)
                            .Trim();
                    }
                }
            }


            // Fallback
            return transactionId;
        }


        // ============================================================
        // RESET
        // ============================================================
        private void ResetSummary()
        {
            lblOpeningValue.Text =
                "AED 0.00";

            lblDebitValue.Text =
                "AED 0.00";

            lblCreditValue.Text =
                "AED 0.00";

            lblClosingValue.Text =
                "AED 0.00";

            lblSelectedAccount.Text =
                "Select an account";

            lblPeriod.Text =
                "Period";

            lblTransactionCount.Text =
                "0 transactions";
        }


        // ============================================================
        // PRINT
        // ============================================================
        private void btnPrint_Click(
            object sender,
            EventArgs e)
        {
            if (cmbAccount.SelectedValue == null ||
                Convert.ToInt32(
                    cmbAccount.SelectedValue) <= 0)
            {
                MessageBox.Show(
                    "Please select an account first.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }


            ContextMenuStrip menu =
                new ContextMenuStrip();


            ToolStripMenuItem pdfItem =
                new ToolStripMenuItem(
                    "Export to PDF");


            ToolStripMenuItem excelItem =
                new ToolStripMenuItem(
                    "Export to Excel");


            pdfItem.Click +=
                (s, args) =>
                {
                    MessageBox.Show(
                        "PDF export can be connected here.");
                };


            excelItem.Click +=
                (s, args) =>
                {
                    MessageBox.Show(
                        "Excel export can be connected here.");
                };


            menu.Items.Add(pdfItem);
            menu.Items.Add(excelItem);


            menu.Show(
                btnPrint,
                new Point(
                    0,
                    btnPrint.Height));
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (cmbAccount.SelectedValue == null ||
                Convert.ToInt32(cmbAccount.SelectedValue) <= 0)
            {
                MessageBox.Show(
                    "Please select an account first.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (dgvLedger.Rows.Count == 0)
            {
                MessageBox.Show(
                    "There is no ledger data to export.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            ExportLedgerToExcel();
        }
        private void ExportLedgerToExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter =
                "Excel Workbook (*.xlsx)|*.xlsx";

            saveDialog.Title =
                "Export Account Ledger";

            saveDialog.FileName =
                "Account_Ledger_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") +
                ".xlsx";


            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;


            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                Cursor = Cursors.WaitCursor;


                // ============================================================
                // CREATE EXCEL
                // ============================================================
                excelApp =
                    new Excel.Application();

                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;


                workbook =
                    excelApp.Workbooks.Add();


                worksheet =
                    (Excel.Worksheet)workbook.Worksheets[1];


                worksheet.Name =
                    "Account Ledger";


                // ============================================================
                // PAGE TITLE
                // ============================================================
                Excel.Range titleRange =
                    worksheet.Range["A1", "G1"];

                titleRange.Merge();

                titleRange.Value =
                    "ACCOUNT LEDGER";

                titleRange.Font.Bold = true;
                titleRange.Font.Size = 18;

                titleRange.HorizontalAlignment =
                    Excel.XlHAlign.xlHAlignCenter;

                titleRange.VerticalAlignment =
                    Excel.XlVAlign.xlVAlignCenter;

                worksheet.Rows[1].RowHeight = 30;


                // ============================================================
                // ACCOUNT
                // ============================================================
                worksheet.Cells[3, 1] =
                    "Account:";

                worksheet.Cells[3, 2] =
                    cmbAccount.Text;


                Excel.Range accountLabel =
                    worksheet.Cells[3, 1];

                accountLabel.Font.Bold = true;


                Excel.Range accountValue =
                    worksheet.Range["B3", "G3"];

                accountValue.Merge();

                accountValue.Font.Bold = true;


                // ============================================================
                // PERIOD
                // ============================================================
                worksheet.Cells[4, 1] =
                    "Period:";

                worksheet.Cells[4, 2] =
                    dtpFrom.Value.ToString("dd MMM yyyy")
                    + " - "
                    + dtpTo.Value.ToString("dd MMM yyyy");


                Excel.Range periodLabel =
                    worksheet.Cells[4, 1];

                periodLabel.Font.Bold = true;


                Excel.Range periodValue =
                    worksheet.Range["B4", "G4"];

                periodValue.Merge();


                // ============================================================
                // SUMMARY
                // ============================================================
                worksheet.Cells[6, 1] =
                    "Opening Balance";

                worksheet.Cells[6, 3] =
                    "Total Debit";

                worksheet.Cells[6, 5] =
                    "Total Credit";

                worksheet.Cells[6, 7] =
                    "Closing Balance";


                worksheet.Cells[7, 1] =
                    lblOpeningValue.Text;

                worksheet.Cells[7, 3] =
                    lblDebitValue.Text;

                worksheet.Cells[7, 5] =
                    lblCreditValue.Text;

                worksheet.Cells[7, 7] =
                    lblClosingValue.Text;


                Excel.Range summaryHeader =
                    worksheet.Range["A6", "G6"];

                summaryHeader.Font.Bold = true;

                summaryHeader.HorizontalAlignment =
                    Excel.XlHAlign.xlHAlignCenter;


                Excel.Range summaryValues =
                    worksheet.Range["A7", "G7"];

                summaryValues.Font.Bold = true;

                summaryValues.Font.Size = 11;

                summaryValues.HorizontalAlignment =
                    Excel.XlHAlign.xlHAlignCenter;


                // ============================================================
                // TABLE HEADER
                // ============================================================
                int headerRow = 10;


                string[] headers =
                {
            "Date",
            "Reference",
            "Transaction",
            "Description",
            "Debit",
            "Credit",
            "Balance"
        };


                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[
                        headerRow,
                        i + 1] = headers[i];
                }


                Excel.Range headerRange =
                    worksheet.Range[
                        worksheet.Cells[headerRow, 1],
                        worksheet.Cells[headerRow, 7]
                    ];


                headerRange.Font.Bold = true;

                headerRange.HorizontalAlignment =
                    Excel.XlHAlign.xlHAlignCenter;

                headerRange.VerticalAlignment =
                    Excel.XlVAlign.xlVAlignCenter;

                headerRange.WrapText = true;


                headerRange.Borders.LineStyle =
                    Excel.XlLineStyle.xlContinuous;

                headerRange.Borders.Weight =
                    Excel.XlBorderWeight.xlThin;


                worksheet.Rows[headerRow].RowHeight = 25;


                // ============================================================
                // EXPORT DATAGRIDVIEW
                // ============================================================
                int excelRow =
                    headerRow + 1;


                foreach (DataGridViewRow gridRow
                    in dgvLedger.Rows)
                {
                    if (gridRow.IsNewRow)
                        continue;


                    // Date
                    worksheet.Cells[
                        excelRow,
                        1] =
                        GetGridCellText(
                            gridRow,
                            "Date");


                    // Reference
                    worksheet.Cells[
                        excelRow,
                        2] =
                        GetGridCellText(
                            gridRow,
                            "Reference");


                    // Transaction
                    worksheet.Cells[
                        excelRow,
                        3] =
                        GetGridCellText(
                            gridRow,
                            "Transaction");


                    // Description
                    worksheet.Cells[
                        excelRow,
                        4] =
                        GetGridCellText(
                            gridRow,
                            "Description");


                    // Debit
                    worksheet.Cells[
                        excelRow,
                        5] =
                        GetGridCellText(
                            gridRow,
                            "Debit");


                    // Credit
                    worksheet.Cells[
                        excelRow,
                        6] =
                        GetGridCellText(
                            gridRow,
                            "Credit");


                    // Balance
                    worksheet.Cells[
                        excelRow,
                        7] =
                        GetGridCellText(
                            gridRow,
                            "Balance");


                    // ========================================================
                    // SPECIAL ROW FORMATTING
                    // ========================================================
                    string description =
                        GetGridCellText(
                            gridRow,
                            "Description");


                    Excel.Range currentRow =
                        worksheet.Range[
                            worksheet.Cells[
                                excelRow,
                                1],

                            worksheet.Cells[
                                excelRow,
                                7]
                        ];


                    if (description ==
                        "Opening Balance")
                    {
                        currentRow.Font.Bold = true;

                        currentRow.Font.Italic = true;
                    }


                    if (description ==
                        "Period Total")
                    {
                        currentRow.Font.Bold = true;

                        currentRow.Borders[
                            Excel.XlBordersIndex.xlEdgeTop]
                            .LineStyle =
                            Excel.XlLineStyle.xlContinuous;
                    }


                    excelRow++;
                }


                // ============================================================
                // TABLE BORDER
                // ============================================================
                int lastDataRow =
                    excelRow - 1;


                if (lastDataRow >= headerRow)
                {
                    Excel.Range tableRange =
                        worksheet.Range[
                            worksheet.Cells[
                                headerRow,
                                1],

                            worksheet.Cells[
                                lastDataRow,
                                7]
                        ];


                    tableRange.Borders.LineStyle =
                        Excel.XlLineStyle.xlContinuous;

                    tableRange.Borders.Weight =
                        Excel.XlBorderWeight.xlThin;


                    tableRange.VerticalAlignment =
                        Excel.XlVAlign.xlVAlignCenter;
                }


                // ============================================================
                // ALIGNMENT
                // ============================================================
                if (lastDataRow > headerRow)
                {
                    Excel.Range debitRange =
                        worksheet.Range[
                            worksheet.Cells[
                                headerRow + 1,
                                5],

                            worksheet.Cells[
                                lastDataRow,
                                5]
                        ];


                    Excel.Range creditRange =
                        worksheet.Range[
                            worksheet.Cells[
                                headerRow + 1,
                                6],

                            worksheet.Cells[
                                lastDataRow,
                                6]
                        ];


                    Excel.Range balanceRange =
                        worksheet.Range[
                            worksheet.Cells[
                                headerRow + 1,
                                7],

                            worksheet.Cells[
                                lastDataRow,
                                7]
                        ];


                    debitRange.HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignRight;

                    creditRange.HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignRight;

                    balanceRange.HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignRight;
                }


                // ============================================================
                // COLUMN WIDTH
                // ============================================================
                worksheet.Columns["A"].ColumnWidth = 14;
                worksheet.Columns["B"].ColumnWidth = 16;
                worksheet.Columns["C"].ColumnWidth = 20;
                worksheet.Columns["D"].ColumnWidth = 45;
                worksheet.Columns["E"].ColumnWidth = 16;
                worksheet.Columns["F"].ColumnWidth = 16;
                worksheet.Columns["G"].ColumnWidth = 18;


                worksheet.Columns["D"].WrapText = true;


                // ============================================================
                // FREEZE HEADER
                // ============================================================
                worksheet.Activate();

                Excel.Range freezeCell =
                    worksheet.Cells[
                        headerRow + 1,
                        1];

                freezeCell.Select();

                excelApp.ActiveWindow.FreezePanes =
                    true;


                // ============================================================
                // PAGE SETUP
                // ============================================================
                worksheet.PageSetup.Orientation =
                    Excel.XlPageOrientation.xlLandscape;

                worksheet.PageSetup.PaperSize =
                    Excel.XlPaperSize.xlPaperA4;

                worksheet.PageSetup.Zoom =
                    false;

                worksheet.PageSetup.FitToPagesWide =
                    1;

                worksheet.PageSetup.FitToPagesTall =
                    false;

                worksheet.PageSetup.LeftMargin =
                    excelApp.InchesToPoints(0.3);

                worksheet.PageSetup.RightMargin =
                    excelApp.InchesToPoints(0.3);

                worksheet.PageSetup.TopMargin =
                    excelApp.InchesToPoints(0.5);

                worksheet.PageSetup.BottomMargin =
                    excelApp.InchesToPoints(0.5);


                // Repeat table header when printing
                worksheet.PageSetup.PrintTitleRows =
                    "$10:$10";


                // ============================================================
                // FOOTER
                // ============================================================
                worksheet.PageSetup.CenterFooter =
                    "Account Ledger";

                worksheet.PageSetup.RightFooter =
                    "Page &P of &N";


                // ============================================================
                // SAVE
                // ============================================================
                workbook.SaveAs(
                    saveDialog.FileName,
                    Excel.XlFileFormat.xlOpenXMLWorkbook);


                workbook.Close(false);

                excelApp.Quit();


                MessageBox.Show(
                    "Account Ledger exported successfully.",
                    "Export Excel",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


                // Optional:
                // Open the generated Excel file automatically.
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName =
                            saveDialog.FileName,

                        UseShellExecute =
                            true
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to export Account Ledger.\n\n" +
                    ex.Message,
                    "Export Excel",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor =
                    Cursors.Default;


                // ============================================================
                // RELEASE COM OBJECTS
                // ============================================================
                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(
                        worksheet);

                    worksheet = null;
                }


                if (workbook != null)
                {
                    try
                    {
                        workbook.Close(false);
                    }
                    catch
                    {
                    }

                    Marshal.ReleaseComObject(
                        workbook);

                    workbook = null;
                }


                if (excelApp != null)
                {
                    try
                    {
                        excelApp.Quit();
                    }
                    catch
                    {
                    }

                    Marshal.ReleaseComObject(
                        excelApp);

                    excelApp = null;
                }


                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private string GetGridCellText(DataGridViewRow row, string columnName)
        {
            if (row == null)
                return "";

            if (!dgvLedger.Columns.Contains(columnName))
                return "";

            object value =
                row.Cells[columnName].Value;

            if (value == null ||
                value == DBNull.Value)
            {
                return "";
            }

            return value.ToString();
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            if (cmbAccount.SelectedValue == null ||
                Convert.ToInt32(cmbAccount.SelectedValue) <= 0)
            {
                MessageBox.Show(
                    "Please select an account first.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (dgvLedger.Rows.Count == 0)
            {
                MessageBox.Show(
                    "There is no ledger data to export.",
                    "Account Ledger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            ExportLedgerToPdf();
        }

        private void ExportLedgerToPdf()
        {
            SaveFileDialog saveDialog =
                new SaveFileDialog();

            saveDialog.Filter =
                "PDF File (*.pdf)|*.pdf";

            saveDialog.Title =
                "Export Account Ledger";

            saveDialog.FileName =
                "Account_Ledger_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") +
                ".pdf";


            if (saveDialog.ShowDialog() !=
                DialogResult.OK)
            {
                return;
            }


            try
            {
                Cursor = Cursors.WaitCursor;


                // ============================================================
                // CREATE DOCUMENT
                // ============================================================
                Document document =
                    new Document();


                document.Info.Title =
                    "Account Ledger";

                document.Info.Subject =
                    cmbAccount.Text;


                // ============================================================
                // DEFAULT STYLE
                // ============================================================
                Style normalStyle =
                    document.Styles["Normal"];

                normalStyle.Font.Name =
                    "Arial";

                normalStyle.Font.Size =
                    8.5;


                // ============================================================
                // SECTION
                // ============================================================
                Section section =
                    document.AddSection();


                section.PageSetup.PageFormat =
                    PageFormat.A4;

                section.PageSetup.Orientation =
                    Orientation.Landscape;


                section.PageSetup.TopMargin =
                    Unit.FromCentimeter(1.2);

                section.PageSetup.BottomMargin =
                    Unit.FromCentimeter(1.2);

                section.PageSetup.LeftMargin =
                    Unit.FromCentimeter(1.2);

                section.PageSetup.RightMargin =
                    Unit.FromCentimeter(1.2);


                // ============================================================
                // TITLE
                // ============================================================
                Paragraph title =
                    section.AddParagraph();

                title.AddText(
                    "ACCOUNT LEDGER");

                title.Format.Font.Size = 18;
                title.Format.Font.Bold = true;

                title.Format.Alignment =
                    ParagraphAlignment.Center;

                title.Format.SpaceAfter =
                    Unit.FromCentimeter(0.15);


                // Subtitle
                Paragraph subtitle =
                    section.AddParagraph();

                subtitle.AddText(
                    "Account Activity Statement");

                subtitle.Format.Font.Size = 9;

                subtitle.Format.Font.Color =
                    Colors.Gray;

                subtitle.Format.Alignment =
                    ParagraphAlignment.Center;

                subtitle.Format.SpaceAfter =
                    Unit.FromCentimeter(0.6);


                // ============================================================
                // ACCOUNT INFORMATION
                // ============================================================
                Table infoTable =
                    section.AddTable();

                infoTable.Borders.Visible = false;

                infoTable.AddColumn(
                    Unit.FromCentimeter(2.2));

                infoTable.AddColumn(
                    Unit.FromCentimeter(11));

                infoTable.AddColumn(
                    Unit.FromCentimeter(2));

                infoTable.AddColumn(
                    Unit.FromCentimeter(9));


                // Account row
                Row infoRow =
                    infoTable.AddRow();


                Paragraph p =
                    infoRow.Cells[0]
                        .AddParagraph();

                p.AddText("Account:");

                p.Format.Font.Bold = true;


                p =
                    infoRow.Cells[1]
                        .AddParagraph();

                p.AddText(
                    cmbAccount.Text);

                p.Format.Font.Bold = true;


                // Period
                p =
                    infoRow.Cells[2]
                        .AddParagraph();

                p.AddText("Period:");

                p.Format.Font.Bold = true;


                p =
                    infoRow.Cells[3]
                        .AddParagraph();

                p.AddText(
                    dtpFrom.Value.ToString(
                        "dd MMM yyyy")
                    +
                    " - "
                    +
                    dtpTo.Value.ToString(
                        "dd MMM yyyy"));


                section.AddParagraph()
                    .Format.SpaceAfter =
                    Unit.FromCentimeter(0.2);


                // ============================================================
                // SUMMARY CARDS / TABLE
                // ============================================================
                Table summaryTable =
                    section.AddTable();


                summaryTable.Borders.Width = 0.5;

                summaryTable.Borders.Color =
                    Colors.LightGray;


                for (int i = 0; i < 4; i++)
                {
                    summaryTable.AddColumn(
                        Unit.FromCentimeter(6.4));
                }


                // Header
                Row summaryHeader =
                    summaryTable.AddRow();

                summaryHeader.Shading.Color =
                    Colors.LightGray;

                summaryHeader.Format.Font.Bold =
                    true;


                AddCellText(
                    summaryHeader.Cells[0],
                    "Opening Balance",
                    ParagraphAlignment.Center);

                AddCellText(
                    summaryHeader.Cells[1],
                    "Total Debit",
                    ParagraphAlignment.Center);

                AddCellText(
                    summaryHeader.Cells[2],
                    "Total Credit",
                    ParagraphAlignment.Center);

                AddCellText(
                    summaryHeader.Cells[3],
                    "Closing Balance",
                    ParagraphAlignment.Center);


                // Values
                Row summaryValues =
                    summaryTable.AddRow();

                summaryValues.Height =
                    Unit.FromCentimeter(0.8);

                summaryValues.VerticalAlignment =
                    VerticalAlignment.Center;

                summaryValues.Format.Font.Bold =
                    true;

                summaryValues.Format.Font.Size =
                    10;


                AddCellText(
                    summaryValues.Cells[0],
                    lblOpeningValue.Text,
                    ParagraphAlignment.Center);

                AddCellText(
                    summaryValues.Cells[1],
                    lblDebitValue.Text,
                    ParagraphAlignment.Center);

                AddCellText(
                    summaryValues.Cells[2],
                    lblCreditValue.Text,
                    ParagraphAlignment.Center);

                AddCellText(
                    summaryValues.Cells[3],
                    lblClosingValue.Text,
                    ParagraphAlignment.Center);


                // Space
                Paragraph space =
                    section.AddParagraph();

                space.Format.SpaceAfter =
                    Unit.FromCentimeter(0.3);


                // ============================================================
                // LEDGER TABLE
                // ============================================================
                Table table =
                    section.AddTable();


                table.Borders.Width =
                    0.35;

                table.Borders.Color =
                    Colors.LightGray;


                // Date
                table.AddColumn(
                    Unit.FromCentimeter(2.4));

                // Reference
                table.AddColumn(
                    Unit.FromCentimeter(2.7));

                // Transaction
                table.AddColumn(
                    Unit.FromCentimeter(3.5));

                // Description
                table.AddColumn(
                    Unit.FromCentimeter(7.7));

                // Debit
                table.AddColumn(
                    Unit.FromCentimeter(3));

                // Credit
                table.AddColumn(
                    Unit.FromCentimeter(3));

                // Balance
                table.AddColumn(
                    Unit.FromCentimeter(3.2));


                // ============================================================
                // HEADER ROW
                // ============================================================
                Row header =
                    table.AddRow();


                header.HeadingFormat = true;

                header.Format.Font.Bold = true;

                header.Format.Font.Size = 8;

                header.Shading.Color =
                    Colors.LightGray;

                header.VerticalAlignment =
                    VerticalAlignment.Center;


                AddCellText(
                    header.Cells[0],
                    "Date",
                    ParagraphAlignment.Left);

                AddCellText(
                    header.Cells[1],
                    "Reference",
                    ParagraphAlignment.Left);

                AddCellText(
                    header.Cells[2],
                    "Transaction",
                    ParagraphAlignment.Left);

                AddCellText(
                    header.Cells[3],
                    "Description",
                    ParagraphAlignment.Left);

                AddCellText(
                    header.Cells[4],
                    "Debit",
                    ParagraphAlignment.Right);

                AddCellText(
                    header.Cells[5],
                    "Credit",
                    ParagraphAlignment.Right);

                AddCellText(
                    header.Cells[6],
                    "Balance",
                    ParagraphAlignment.Right);


                // ============================================================
                // DATA
                // ============================================================
                foreach (DataGridViewRow gridRow
                    in dgvLedger.Rows)
                {
                    if (gridRow.IsNewRow)
                        continue;


                    string description =
                        GetGridCellText(
                            gridRow,
                            "Description");


                    Row row =
                        table.AddRow();


                    row.VerticalAlignment =
                        VerticalAlignment.Center;


                    AddCellText(
                        row.Cells[0],
                        GetGridCellText(
                            gridRow,
                            "Date"),
                        ParagraphAlignment.Left);


                    AddCellText(
                        row.Cells[1],
                        GetGridCellText(
                            gridRow,
                            "Reference"),
                        ParagraphAlignment.Left);


                    AddCellText(
                        row.Cells[2],
                        GetGridCellText(
                            gridRow,
                            "Transaction"),
                        ParagraphAlignment.Left);


                    AddCellText(
                        row.Cells[3],
                        description,
                        ParagraphAlignment.Left);


                    AddCellText(
                        row.Cells[4],
                        GetGridCellText(
                            gridRow,
                            "Debit"),
                        ParagraphAlignment.Right);


                    AddCellText(
                        row.Cells[5],
                        GetGridCellText(
                            gridRow,
                            "Credit"),
                        ParagraphAlignment.Right);


                    AddCellText(
                        row.Cells[6],
                        GetGridCellText(
                            gridRow,
                            "Balance"),
                        ParagraphAlignment.Right);


                    // ========================================================
                    // OPENING BALANCE
                    // ========================================================
                    if (description ==
                        "Opening Balance")
                    {
                        row.Format.Font.Bold =
                            true;

                        row.Format.Font.Italic =
                            true;

                        row.Shading.Color =
                            Colors.WhiteSmoke;
                    }


                    // ========================================================
                    // PERIOD TOTAL
                    // ========================================================
                    if (description ==
                        "Period Total")
                    {
                        row.Format.Font.Bold =
                            true;

                        row.Shading.Color =
                            Colors.LightGray;
                    }
                }


                // ============================================================
                // FOOTER
                // ============================================================
                Paragraph footer =
                    section.Footers.Primary
                        .AddParagraph();


                footer.Format.Font.Size = 7;

                footer.Format.Font.Color =
                    Colors.Gray;


                footer.AddText(
                    "Account Ledger   |   Generated "
                    +
                    DateTime.Now.ToString(
                        "dd MMM yyyy hh:mm tt")
                    +
                    "   |   Page ");


                footer.AddPageField();

                footer.AddText(" of ");

                footer.AddNumPagesField();


                footer.Format.Alignment =
                    ParagraphAlignment.Center;


                // ============================================================
                // RENDER PDF
                // ============================================================
                PdfDocumentRenderer renderer =
                    new PdfDocumentRenderer(true);


                renderer.Document =
                    document;


                renderer.RenderDocument();


                renderer.PdfDocument.Save(
                    saveDialog.FileName);


                // ============================================================
                // SUCCESS
                // ============================================================
                MessageBox.Show(
                    "Account Ledger PDF exported successfully.",
                    "Export PDF",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


                // ============================================================
                // OPEN PDF
                // ============================================================
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName =
                            saveDialog.FileName,

                        UseShellExecute =
                            true
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to export Account Ledger PDF."
                    +
                    Environment.NewLine
                    +
                    Environment.NewLine
                    +
                    ex.Message,

                    "Export PDF",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor =
                    Cursors.Default;
            }
        }

        private void AddCellText(Cell cell, string text, ParagraphAlignment alignment)
        {
            Paragraph paragraph =
                cell.AddParagraph();

            paragraph.AddText(
                text ?? "");

            paragraph.Format.Alignment =
                alignment;

            paragraph.Format.SpaceBefore = 2;
            paragraph.Format.SpaceAfter = 2;
        }

    }
}
