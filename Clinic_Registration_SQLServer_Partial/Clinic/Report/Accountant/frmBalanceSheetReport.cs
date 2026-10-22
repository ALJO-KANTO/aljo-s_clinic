using Clinic;
using Clinic.global_classes;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using Excel = Microsoft.Office.Interop.Excel;

namespace YamyProject
{
    public partial class frmBalanceSheetReport : Form
    {
        string type = "";
        public frmBalanceSheetReport(string type)
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            this.type = type;
            if (type == "detail")
                headerUC1.FormText = "Balance Sheet Detail";
            else if (type == "standard")
                headerUC1.FormText = "Balance Sheet";
            else
                headerUC1.FormText = this.Text;
        }
        //int RetainedEarningsId = 17; // Assuming this is the ID for Retained Earnings in your database
        //decimal retainedEarningsBalance = 0;
        private void frmBalanceSheetReport_Load(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            lblTime.Text = DateTime.Now.ToString("hh:mm:tt");
            using (var reader = ClinicDatabase.ExecuteReader("SELECT name FROM tbl_company"))
            {
                if (reader.Read() && reader["name"] != DBNull.Value)
                {
                    lblCompany.Text = reader["name"].ToString();
                }
            }
            btnPrint.Text = "Print ▼";
            if(type == "detail")
            {
                label1.Text = label1.Text + " Detail";
                dateTimePicker2.Visible = true;
            }
            else if (type == "Prev Year Comparison")
            {
                label1.Text = label1.Text + " " + type;
                dateTimePicker2.Visible = true;
            }
            else if (type == "by Class")
            {
                label1.Text = label1.Text + " By Class";
                dateTimePicker2.Visible = false;
            }
            //RetainedEarningsId = 17;
            LoadData();
            ApplyBalanceSheetArabic();
        }
        private void LoadData()
        {
            dateChangeTo();
            if (type == "detail")
            {
                dgvReport.Dock = panel2.Dock = DockStyle.Fill;
            }

            AddDGVColumns();
            BindDGV();
        }
        DateTime prevFrom;
        DateTime prevTo;
        private void dateChangeTo()
        {
            if (type == "Prev Year Comparison")
            {
                dateTimePicker1.Value = new DateTime(DateTime.Now.Year, 1, 1);
            }
            prevFrom = new DateTime(dateTimePicker1.Value.Year, 1, 1);
            prevTo = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, DateTime.DaysInMonth(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month));
        }
        private void AddDGVColumns()
        {
            if (type == "Prev Year Comparison")
            {
                dgvReport.Columns.Add("state", "");
                dgvReport.Columns.Add("loadState", "");
                dgvReport.Columns.Add("currLvl", "");
                dgvReport.Columns.Add("lvl1", " ");
                dgvReport.Columns.Add("id", "");
                dgvReport.Columns.Add("currname", "");
                string currentPeriod = $"{dateTimePicker1.Value:MMM dd} – {dateTimePicker2.Value:MMM dd yy}";
                string previousPeriod = $"{prevFrom:MMM dd} – {prevTo:MMM dd yy}";
                dgvReport.Columns.Add("balance1", currentPeriod);
                dgvReport.Columns.Add("balance2", previousPeriod);
                dgvReport.Columns.Add("change", "Change");
                dgvReport.Columns.Add("change_percentage", "% Change");

                dgvReport.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvReport.Columns["state"].Visible = dgvReport.Columns["currname"].Visible = dgvReport.Columns["id"].Visible = dgvReport.Columns["loadState"].Visible = dgvReport.Columns["currLvl"].Visible = false;

                dgvReport.Columns["balance1"].DefaultCellStyle.Alignment = dgvReport.Columns["balance2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvReport.Columns["change"].DefaultCellStyle.Alignment = dgvReport.Columns["change_percentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                dgvReport.Columns.Add("state", "");
                dgvReport.Columns.Add("loadState", "");
                dgvReport.Columns.Add("currLvl", "");
                dgvReport.Columns.Add("lvl1", " ");
                dgvReport.Columns.Add("id", "");
                dgvReport.Columns.Add("currname", "");
                if (type == "detail")
                {
                    dgvReport.Columns.Add("date", "Date");
                    dgvReport.Columns.Add("num", "Num");
                    dgvReport.Columns.Add("adj", "Adj");
                    dgvReport.Columns.Add("name", "Name");
                    dgvReport.Columns.Add("memo", "Memo");
                    dgvReport.Columns.Add("clr", "Clr");
                    dgvReport.Columns.Add("split", "Split");
                    dgvReport.Columns.Add("debit", "Debit");
                    dgvReport.Columns.Add("credit", "Credit");
                }
                if (type == "by Class")
                    dgvReport.Columns.Add("balance0", "Unclassified");

                dgvReport.Columns.Add("balance", "Balance");
                dgvReport.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvReport.Columns["state"].Visible = dgvReport.Columns["currname"].Visible = dgvReport.Columns["id"].Visible = dgvReport.Columns["loadState"].Visible = dgvReport.Columns["currLvl"].Visible = false;
                if (type == "detail")
                {
                    dgvReport.Columns["debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvReport.Columns["credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (type == "by Class")
                    dgvReport.Columns["balance0"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvReport.Columns["balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            dgvReport.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvReport.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvReport.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
            dgvReport.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            dgvReport.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvReport.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;

            LocalizationManager.LocalizeDataGridViewHeaders(dgvReport);
        }

        public void BindDGV()
        {
            //retainedEarningsBalance = GetNetProfit(dateTimePicker1.Value, dateTimePicker2.Value);
            dgvReport.Rows.Clear();
            string filterDate = (type == "detail" || type == "Prev Year Comparison")
                                ? " AND t.date BETWEEN @fromDate AND @toDate"
                                : " AND t.date <= @fromDate";
            bool isStandard = true;
            decimal totalLandE = 0;
            decimal totalBal1 = 0;
            decimal totalBal2 = 0;
            decimal totalChange = 0;
            decimal totalChangePerc = 0;

            if (type == "Prev Year Comparison")
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                            WITH AccountBalances AS (
                                -- Calculate balances for Income, Cost, and Expense categories (for P&L integration)
                                SELECT 
                                    l1.category_code AS Category, 
                                    COALESCE(SUM(CASE WHEN t.date BETWEEN @currentFrom AND @currentTo THEN t.debit - t.credit END), 0) AS Balance1,
                                    COALESCE(SUM(CASE WHEN t.date BETWEEN @prevFrom AND @prevTo THEN t.debit - t.credit END), 0) AS Balance2
                                FROM tbl_coa_level_1 l1
                                LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                                LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                                LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                                LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                                WHERE l1.category_code IN ('INCOME', 'COST', 'EXPENSE')
                                  AND t.state = 0
                                GROUP BY l1.category_code
                            ),

                            FinalBalance AS (
                                -- Compute retained earnings = Income - Cost - Expenses
                                SELECT 
                                    'Final Balance (Income - Cost - Expenses)' AS Category,
                                    COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='INCOME'),0)
                                    - COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='COST'),0)
                                    - COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='EXPENSE'),0) AS Balance1,

                                    COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='INCOME'),0)
                                    - COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='COST'),0)
                                    - COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='EXPENSE'),0) AS Balance2
                            ),

                            EquityBalances AS (
                                -- Compute balances for Asset, Liability, and Equity categories
                                SELECT 
                                    l1.name,
                                    l1.id,
                                    l1.category_code,
                                    COALESCE(SUM(CASE WHEN t.date BETWEEN @currentFrom AND @currentTo THEN t.debit - t.credit END), 0) AS Balance1,
                                    COALESCE(SUM(CASE WHEN t.date BETWEEN @prevFrom AND @prevTo THEN t.debit - t.credit END), 0) AS Balance2
                                FROM tbl_coa_level_1 l1
                                LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                                LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                                LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                                LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                                WHERE l1.category_code IN ('ASSET', 'LIABILITY', 'EQUITY')
                                  AND t.state = 0
                                GROUP BY l1.id, l1.name, l1.category_code
                            )

                            SELECT 
                                eb.name,
                                eb.id,
                                CASE 
                                    WHEN eb.category_code = 'EQUITY' THEN eb.Balance1 + (SELECT Balance1 FROM FinalBalance)
                                    ELSE eb.Balance1
                                END AS Balance1,
                                CASE 
                                    WHEN eb.category_code = 'EQUITY' THEN eb.Balance2 + (SELECT Balance2 FROM FinalBalance)
                                    ELSE eb.Balance2
                                END AS Balance2
                            FROM EquityBalances eb
                            ORDER BY eb.id;
                            ",
                        ClinicDatabase.CreateParameterNew("@currentFrom", dateTimePicker1.Value.Date),
                        ClinicDatabase.CreateParameterNew("@currentTo", dateTimePicker2.Value.Date),
                        ClinicDatabase.CreateParameterNew("@prevFrom", prevFrom),
                        ClinicDatabase.CreateParameterNew("@prevTo", prevTo)))
                {
                    while (reader.Read())
                    {
                        if (type == "detail")
                        {
                            isStandard = true;
                            dgvReport.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                                reader["id"].ToString(), reader["name"].ToString(),
                                " ", " ", " ", " ", " ", " ", " ", " ", " ",
                                Utilities.FormatDecimal(reader["balance"].ToString()));

                            if (!CheckNameIsAssets(reader["name"].ToString()))
                            {
                                totalLandE += Convert.ToDecimal(reader["balance"]);
                            }
                        }
                        else if(type == "Prev Year Comparison")
                        {
                            isStandard = false;
                            decimal bal1 = Convert.ToDecimal(reader["Balance1"]);
                            decimal bal2 = Convert.ToDecimal(reader["Balance2"]);
                            decimal change = bal1 - bal2;
                            decimal changePerc = (bal2 != 0) ? (change / Math.Abs(bal2)) * 100 : 0;
                            dgvReport.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                            reader["id"].ToString(), reader["name"].ToString(),
                            Utilities.FormatDecimal(bal1.ToString()),
                            Utilities.FormatDecimal(bal2.ToString()),
                            Utilities.FormatDecimal(change.ToString()),
                            Utilities.FormatDecimal(changePerc.ToString()) + " %");

                            totalBal1 += bal1;
                            totalBal2 += bal2;
                            totalChange += change;

                        }
                        else
                        {
                            isStandard = true;
                            dgvReport.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                            reader["id"].ToString(), reader["name"].ToString(),
                            Utilities.FormatDecimal(reader["balance"].ToString()));


                            if (!CheckNameIsAssets(reader["name"].ToString()))
                            {
                                totalLandE += Convert.ToDecimal(reader["balance"]);
                            }
                        }
                    }

                    // Auto-expand everything
                    for (int i = 0; i < dgvReport.Rows.Count; i++)
                    {
                        ExpandRowRecursively(i);
                    }

                    var totalRow = new DataGridViewRow();
                    totalRow.CreateCells(dgvReport);
                    if (isStandard)
                    {
                        // Set the label column (adjust index if needed)
                        int nameColumnIndex = 3;
                        totalRow.Cells[nameColumnIndex].Value =
      "          " + ReportTranslate("Total Liabilities & Equity");

                        // Set value in the last column dynamically
                        int lastColumnIndex = dgvReport.Columns.Count - 1;
                        totalRow.Cells[lastColumnIndex].Value =
                            Utilities.FormatDecimal(totalLandE.ToString());
                    }
                    else
                    {
                        if (type == "Prev Year Comparison")
                        {
                            if (totalBal2 != 0)
                                totalChangePerc = (totalChange / Math.Abs(totalBal2)) * 100;
                            else
                                totalChangePerc = 0;

                            int nameColumnIndex = 3;
                            totalRow.Cells[nameColumnIndex].Value =
                                "          Total Liabilities & Equity";

                            totalRow.Cells[6].Value = Utilities.FormatDecimal(totalBal1.ToString());
                            totalRow.Cells[7].Value = Utilities.FormatDecimal(totalBal2.ToString());
                            totalRow.Cells[8].Value = Utilities.FormatDecimal(totalChange.ToString());
                            totalRow.Cells[9].Value =
                                Utilities.FormatDecimal(totalChangePerc.ToString()) + " %";
                        }
                        else if (type == "by Class")
                        {
                            int nameColumnIndex = 3;
                            totalRow.Cells[nameColumnIndex].Value =
                                "          Total Liabilities & Equity";
                            totalRow.Cells[6].Value = Utilities.FormatDecimal(totalBal1.ToString());
                            totalRow.Cells[7].Value = Utilities.FormatDecimal(totalBal2.ToString());
                        }
                    }
                    // Style
                    totalRow.DefaultCellStyle.Font =
                            new System.Drawing.Font(dgvReport.Font, FontStyle.Bold);

                    totalRow.DefaultCellStyle.BackColor =
                        System.Drawing.Color.Beige;

                    // Add row
                    dgvReport.Rows.Add(totalRow);
                }
            }
            else
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                    WITH AccountBalances AS (
                        SELECT 
                            l1.category_code AS Category, 
                            CASE 
                               WHEN l1.category_code = 'ASSET'
                                   THEN COALESCE(SUM(t.debit) - SUM(t.credit), 0)

                               WHEN l1.category_code IN ('LIABILITY','EQUITY','INCOME')
                                   THEN COALESCE(SUM(t.credit) - SUM(t.debit), 0)

                               WHEN l1.category_code IN ('COST','EXPENSE')
                                   THEN COALESCE(SUM(t.debit) - SUM(t.credit), 0)
                            END AS Balance
                        FROM tbl_coa_level_1 l1
                        LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                        LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                        LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                        LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                        WHERE l1.category_code IN ('INCOME', 'COST', 'EXPENSE') 
                          AND t.state = 0
                            " + filterDate + @"
                        GROUP BY l1.category_code
                    ),

                    FinalBalance AS (
                        SELECT 
                            'Final Balance (Income - Cost - Expenses)' AS Category,
                            COALESCE((SELECT Balance FROM AccountBalances WHERE Category = 'INCOME'), 0)
                            - COALESCE((SELECT Balance FROM AccountBalances WHERE Category = 'COST'), 0)
                            - COALESCE((SELECT Balance FROM AccountBalances WHERE Category = 'EXPENSE'), 0)
                            AS Balance
                    ),

                    EquityBalances AS (
                        SELECT 
                            l1.name, 
                            l1.id, 
                            l1.category_code,
                            CASE 
                               WHEN l1.category_code = 'ASSET'
                                   THEN COALESCE(SUM(t.debit) - SUM(t.credit), 0)

                               WHEN l1.category_code IN ('LIABILITY','EQUITY','INCOME')
                                   THEN COALESCE(SUM(t.credit) - SUM(t.debit), 0)

                               WHEN l1.category_code IN ('COST','EXPENSE')
                                   THEN COALESCE(SUM(t.debit) - SUM(t.credit), 0)
                            END AS balance
                        FROM tbl_coa_level_1 l1
                        LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                        LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                        LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                        LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                        WHERE t.state = 0 
                          AND l1.category_code IN ('ASSET', 'LIABILITY', 'EQUITY')
                          " + filterDate + @"
                        GROUP BY l1.id, l1.name, l1.category_code
                    )

                    SELECT 
                        eb.name, 
                        eb.id, 
                        CASE 
                            WHEN eb.category_code = 'EQUITY' THEN 
                                eb.balance 
                                + COALESCE((SELECT Balance FROM FinalBalance), 0)
                            ELSE 
                                eb.balance
                        END AS balance
                    FROM EquityBalances eb
                    ORDER BY eb.id;
                    ",
                        ClinicDatabase.CreateParameterNew("@fromDate", dateTimePicker1.Value.Date),
                        ClinicDatabase.CreateParameterNew("@toDate", dateTimePicker2.Value.Date)))
                {
                    while (reader.Read())
                    {
                        if (type == "detail")
                        {
                            dgvReport.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                                reader["id"].ToString(), reader["name"].ToString(),
                                " ", " ", " ", " ", " ", " ", " ", " ", " ",
                                Utilities.FormatDecimal(reader["balance"].ToString()));

                            if (!CheckNameIsAssets(reader["name"].ToString()))
                            {
                                totalLandE += Convert.ToDecimal(reader["balance"]);
                            }
                            isStandard = true;
                        }
                        else
                        {
                            if (type == "by Class")
                            {
                                isStandard = false;
                                dgvReport.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                                reader["id"].ToString(), reader["name"].ToString(),
                                Utilities.FormatDecimal(reader["balance"].ToString()),
                                Utilities.FormatDecimal(reader["balance"].ToString()));
                                if (!CheckNameIsAssets(reader["name"].ToString()))
                                {
                                    totalBal1 += Convert.ToDecimal(reader["balance"]);
                                    totalBal2 += Convert.ToDecimal(reader["balance"]);
                                }
                            }
                            else
                            {
                                isStandard = true;
                                dgvReport.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                                reader["id"].ToString(), reader["name"].ToString(),
                                Utilities.FormatDecimal(reader["balance"].ToString()));

                                if (!CheckNameIsAssets(reader["name"].ToString()))
                                {
                                    totalLandE += Convert.ToDecimal(reader["balance"]);
                                }
                            }
                        }
                    }

                    // Auto-expand everything
                    for (int i = 0; i < dgvReport.Rows.Count; i++)
                    {
                        ExpandRowRecursively(i);
                    }

                    var totalRow = new DataGridViewRow();
                    totalRow.CreateCells(dgvReport);


                    if (isStandard)
                    {
                        // Set the label column (adjust index if needed)
                        int nameColumnIndex = 3;
                        totalRow.Cells[nameColumnIndex].Value = "          Total Liabilities & Equity";

                        // Set value in the last column dynamically
                        int lastColumnIndex = dgvReport.Columns.Count - 1;
                        totalRow.Cells[lastColumnIndex].Value =
                            Utilities.FormatDecimal((totalLandE).ToString());
                    }
                    else
                    {
                        if (type == "Prev Year Comparison")
                        {
                            if (totalBal2 != 0)
                                totalChangePerc = (totalChange / Math.Abs(totalBal2)) * 100;
                            else
                                totalChangePerc = 0;

                            int nameColumnIndex = 3;
                            totalRow.Cells[nameColumnIndex].Value =
                                "          Total Liabilities & Equity";

                            totalRow.Cells[6].Value = Utilities.FormatDecimal(totalBal1.ToString());
                            totalRow.Cells[7].Value = Utilities.FormatDecimal(totalBal2.ToString());
                            totalRow.Cells[8].Value = Utilities.FormatDecimal(totalChange.ToString());
                            totalRow.Cells[9].Value =
                                Utilities.FormatDecimal(totalChangePerc.ToString()) + " %";
                        }
                        else if(type == "by Class")
                        {
                            int nameColumnIndex = 3;
                            totalRow.Cells[nameColumnIndex].Value =
                                "          Total Liabilities & Equity";
                            totalRow.Cells[6].Value = Utilities.FormatDecimal(totalBal1.ToString());
                            totalRow.Cells[7].Value = Utilities.FormatDecimal(totalBal2.ToString());
                        }
                    }
                    // Style
                    totalRow.DefaultCellStyle.Font =
                        new System.Drawing.Font(dgvReport.Font, FontStyle.Bold);

                    totalRow.DefaultCellStyle.BackColor =
                        System.Drawing.Color.Beige;

                    // Add row
                    dgvReport.Rows.Add(totalRow);

                }
            }


            dateWords();
        }

        private bool CheckNameIsAssets(string name)
        {
            object result = ClinicDatabase.ExecuteScalar("SELECT category_code FROM tbl_coa_level_1 where name = @name", ClinicDatabase.CreateParameterNew("@name", name));
            if (result != null)
            {
                string categoryName = (string)result;
                if (categoryName != "ASSET")
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        private void ExpandRowRecursively(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvReport.Rows.Count)
                return;

            var row = dgvReport.Rows[rowIndex];

            string state = row.Cells["state"].Value.ToString();
            string loadState = row.Cells["loadState"].Value.ToString();
            int currLvl = int.Parse(row.Cells["currLvl"].Value.ToString());

            if (currLvl >= 4 || state != "e") return;

            string space = new string(' ', currLvl * 6 + 3);
            string cellValue = row.Cells["lvl1"].Value.ToString();

            if (loadState == "u")
            {
                int parentId = int.Parse(row.Cells["id"].Value.ToString());
                List<DataGridViewRow> newRows = new List<DataGridViewRow>();

                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader($@"SELECT name as currname, concat(code,' - ',name) as name , id FROM tbl_coa_level_" + (currLvl + 1) + " WHERE main_id = @parentId ORDER BY code",
                                                                      ClinicDatabase.CreateParameterNew("parentId", parentId)))
                {
                    int insertIndex = rowIndex + 1;

                    //row.Cells["balance"].Value = "";

                    while (reader.Read())
                    {
                        string nextState = (currLvl + 1 == 4) ? "n" : "e";
                        string nextIcon = (currLvl + 1 == 4) ? "  " : "►";

                        var newRow = new DataGridViewRow();
                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = nextState });
                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "u" });
                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = currLvl + 1 });
                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = space + nextIcon + "     " + reader["name"].ToString() });
                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = reader["id"].ToString() });
                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = reader["currname"].ToString() });
                        if (type == "detail")
                        {
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                            var balanceData = GetBalanceData(currLvl + 1, int.Parse(reader["id"].ToString()), dateTimePicker1.Value, dateTimePicker2.Value.Date);
                            if (balanceData != null)
                            {

                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });

                                newRow.Height = row.Height;

                                ApplyRowStyle(newRow, currLvl + 1);
                                newRows.Add(newRow);
                            }
                            if (currLvl + 1 == 4)
                            {
                                using (MySqlDataReader tReader = ClinicDatabase.ExecuteReader(@"
                                        SELECT 
                                                t.id,
                                                t.type,
                                                t.date,
                                                t.voucher_no AS num,
                                                '' AS adjustment,

                                                COALESCE(
                                                    CASE 
                                                        WHEN t.type IN ('Customer','Sales Invoice Cash','Sales Invoice','Credit Note','SalesReturn Invoice','Customer Receipt','Customer Opening Balance','Customer Advance Payment','Check Cancel (Customer)','PDC Receivable') THEN
                                                            (SELECT concat(c.code,' ', c.name) 
                                                             FROM tbl_customer c
                                                             JOIN tbl_transaction x ON x.hum_id = c.id 
                                                             WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type 
                                                             LIMIT 1)
                                                        WHEN t.type IN ('Vendor','Purchase Invoice','Vendor Payment','Subcontractor Payment','Purchase Invoice Cash','Purchase Return Invoice','Debit Note','Vendor Opening Balance','Vendor Advance Payment','Check Cancel (Vendor)','PDC Payable') THEN
                                                            (SELECT concat(v.code,' ', v.name) 
                                                             FROM tbl_vendor v
                                                             JOIN tbl_transaction x ON x.hum_id = v.id
                                                             WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type 
                                                             LIMIT 1)
                                                        WHEN t.type IN ('Employee','Petty Cash','Employee Salary Payment','Employee Loan Payment','Check Cancel (Employee)','Employee Salary') THEN
                                                            (SELECT concat(e.code,' ', e.name) 
                                                             FROM tbl_employee e
                                                             JOIN tbl_transaction x ON x.hum_id = e.id
                                                             WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type 
                                                             LIMIT 1)
                                                        ELSE ''
                                                    END, ''
                                                ) AS name,

                                                COALESCE((
                                                    SELECT concat(lev4.code,' ',lev4.name)
                                                    FROM tbl_transaction x 
                                                    JOIN tbl_coa_level_4 lev4 ON lev4.id = x.account_id
                                                    WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type AND x.hum_id>0
                                                    LIMIT 1
                                                ), '') AS AccountNamed,

                                                COALESCE(t.description, '') AS memo,
                                                '' AS clr,
                                                COALESCE((SELECT name FROM tbl_coa_level_4 WHERE id=t.account_id LIMIT 1), '') AS split,

                                                COALESCE((
                                                    SELECT x.debit 
                                                    FROM tbl_transaction x 
                                                    WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type AND x.hum_id>0 
                                                    LIMIT 1
                                                ), 0) AS debit,

                                                COALESCE((
                                                    SELECT x.credit 
                                                    FROM tbl_transaction x 
                                                    WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type AND x.hum_id>0 
                                                    LIMIT 1
                                                ), 0) AS credit,

                                                (SELECT COALESCE(SUM(tt.debit - tt.credit),0) 
                                                 FROM tbl_transaction tt 
                                                 WHERE tt.transaction_id = t.transaction_id AND tt.type=t.type 
                                                   AND tt.id <= t.id AND tt.hum_id>0) AS running_balance

                                            FROM tbl_transaction t
                                            WHERE t.account_id = @acctId AND t.date BETWEEN @fromDate AND @toDate
                                            ORDER BY t.date, t.id;
                                    ",
                                    ClinicDatabase.CreateParameterNew("acctId", int.Parse(reader["id"].ToString())),
                                    ClinicDatabase.CreateParameterNew("fromDate", dateTimePicker1.Value),
                                    ClinicDatabase.CreateParameterNew("toDate", dateTimePicker2.Value)))
                                {
                                    while (tReader.Read())
                                    {
                                        string trans_type = tReader["type"]?.ToString() ?? "";
                                        string _dateStr = "";
                                        if (tReader["date"] != DBNull.Value)
                                        {
                                            DateTime dt;
                                            if (DateTime.TryParse(tReader["date"].ToString(), out dt))
                                                _dateStr = dt.ToString("MM/dd/yyyy");
                                        }
                                        string _num = tReader["num"]?.ToString() ?? "";
                                        string _adj = tReader["adjustment"]?.ToString() ?? "";
                                        string _trName = tReader["name"]?.ToString() ?? "";
                                        string _AcName = tReader["AccountNamed"]?.ToString() ?? "";
                                        string _memo = tReader["memo"]?.ToString() ?? "";
                                        string _clr = tReader["clr"]?.ToString() ?? "";
                                        string _split = tReader["split"]?.ToString() ?? "";
                                        string debit = Utilities.FormatDecimal(tReader["debit"].ToString());
                                        string credit = Utilities.FormatDecimal(tReader["credit"].ToString());
                                        string running = Utilities.FormatDecimal(tReader["running_balance"].ToString());

                                        newRow = new DataGridViewRow();
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = nextState });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "u" });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = currLvl + 1 });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = space + "     " + trans_type });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["id"].ToString() });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = reader["name"].ToString() });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _dateStr });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _num });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _adj });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _trName });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _memo });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _clr });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _split });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = debit });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = credit });
                                        decimal _bal = decimal.Parse(debit) - decimal.Parse(credit);
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = _bal.ToString("N2") });
                                        newRow.Height = row.Height;

                                        ApplyRowStyle(newRow, currLvl + 1);
                                        newRows.Add(newRow);
                                    }
                                }
                            }
                        }
                        else if (type == "Prev Year Comparison")
                        {
                            var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString(), dateTimePicker1.Value, dateTimePicker2.Value, prevFrom, prevTo);
                            decimal balance1 = balanceData.balance1;
                            decimal balance2 = balanceData.balance2;
                            decimal change = balance1 - balance2;
                            decimal percentChange = (balance2 != 0) ? (change / balance2) * 100 : 0;


                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData.balance1) });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData.balance2) });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(change) });
                            newRow.Cells.Add(new DataGridViewTextBoxCell { Value = $"{percentChange:N1}%" });

                            newRow.Height = row.Height;

                            ApplyRowStyle(newRow, currLvl + 1);
                            newRows.Add(newRow);
                        }
                        else
                        {
                            var balanceData = GetBalanceData(currLvl + 1, int.Parse(reader["id"].ToString()), dateTimePicker1.Value, dateTimePicker2.Value.Date);
                            if (balanceData != null)
                            {
                                if (type == "by Class")
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });

                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });

                                newRow.Height = row.Height;

                                ApplyRowStyle(newRow, currLvl + 1);
                                newRows.Add(newRow);
                            }
                        }
                    }

                    foreach (var newRow in newRows)
                    {
                        dgvReport.Rows.Insert(insertIndex, newRow);
                        insertIndex++;
                    }
                }

                row.Cells["loadState"].Value = "l";

                bool isStandard = (type == "detail" ? false : type == "Prev Year Comparison" ? false : type == "by Class" ? false : true);
                if (isStandard)
                {
                    decimal subgroupTotal = 0;

                    foreach (var newRow in newRows)
                    {
                        if (decimal.TryParse(newRow.Cells["balance"].Value?.ToString(), out decimal val))
                            subgroupTotal += val;
                    }

                    // After all subgroup rows inserted
                    var totalRow = new DataGridViewRow();
                    totalRow.DefaultCellStyle.Font = new System.Drawing.Font(dgvReport.Font, FontStyle.Bold);
                    totalRow.DefaultCellStyle.BackColor = System.Drawing.Color.Beige;

                    totalRow.Cells.Add(new DataGridViewTextBoxCell { Value = "n" }); // state
                    totalRow.Cells.Add(new DataGridViewTextBoxCell { Value = "l" }); // loadState
                    totalRow.Cells.Add(new DataGridViewTextBoxCell { Value = currLvl + 1 }); // currLvl
                    int.TryParse(row.Cells["id"].Value.ToString(), out int idL3);
                    int.TryParse(row.Cells["currLvl"].Value.ToString(), out int L3subId);

                    totalRow.Cells.Add(new DataGridViewTextBoxCell { Value = space + "Total " + row.Cells["lvl1"].Value.ToString().Trim() });
                    totalRow.Cells.Add(new DataGridViewTextBoxCell { Value = row.Cells["id"].Value.ToString() });
                    totalRow.Cells.Add(new DataGridViewTextBoxCell { Value = "TotalRow" });
                    totalRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(subgroupTotal) });

                    // Insert total after children
                    dgvReport.Rows.Insert(rowIndex + newRows.Count + 1, totalRow);
                }
            }

            row.Cells["lvl1"].Value = cellValue.Replace("►", "▼");
            row.Cells["state"].Value = "c";

            // Recursively expand all newly inserted child rows
            int nextRowIndex = rowIndex + 1;
            while (nextRowIndex < dgvReport.Rows.Count &&
                   int.Parse(dgvReport.Rows[nextRowIndex].Cells["currLvl"].Value.ToString()) > currLvl)
            {
                if (int.Parse(dgvReport.Rows[nextRowIndex].Cells["currLvl"].Value.ToString()) == currLvl + 1)
                {
                    ExpandRowRecursively(nextRowIndex);
                }
                nextRowIndex++;
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

            // "Balance Sheet Summary" - Bold, size 10
            FormattedText summaryText = type == "detail" ? center.AddFormattedText("Balance Sheet Detail\n", TextFormat.Bold) : center.AddFormattedText("Balance Sheet Summary\n", TextFormat.Bold);
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

            if (type != "detail")
            {
                // Table for data
                Table dataTable = section.AddTable();
                dataTable.Borders.Width = 0;
                dataTable.AddColumn("10cm");
                dataTable.AddColumn("5cm");

                decimal totalAmount = 0;

                // Load from DataGridView
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.IsNewRow) continue;

                    string account = row.Cells["lvl1"].Value?.ToString() ?? "";
                    if (account.ToUpper().Contains("TOTAL")) continue;

                    string amountStr = row.Cells["balance"].Value?.ToString() ?? "0";
                    account = account.Replace("▶", "").Trim();
                    amountStr = amountStr.Replace("◀", "").Trim();

                    decimal amount = 0;
                    decimal.TryParse(amountStr, out amount);

                    Row tRow = dataTable.AddRow();
                    tRow.Cells[0].AddParagraph(account);
                    tRow.Cells[1].AddParagraph(amount.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;

                    totalAmount += amount;
                }

                // TOTAL row
                Row totalRow = dataTable.AddRow();
                totalRow.Cells[0].AddParagraph("TOTAL").Format.Font.Bold = true;
                Paragraph totalText = totalRow.Cells[1].AddParagraph(totalAmount.ToString("N2"));
                totalText.Format.Font.Bold = true;
                totalText.Format.Font.Underline = Underline.Single;
                totalRow.Cells[1].Format.Alignment = ParagraphAlignment.Right;

                //section.Add(dataTable);
            } else if(type == "detail")
            {
                Table dataTable = section.AddTable();
                dataTable.Borders.Width = 0;
                dataTable.AddColumn("6cm");
                dataTable.AddColumn("2cm");
                dataTable.AddColumn("1cm");
                dataTable.AddColumn("0.5cm");
                dataTable.AddColumn("2cm");
                dataTable.AddColumn("2cm");
                dataTable.AddColumn("0.5cm");
                dataTable.AddColumn("1cm");
                dataTable.AddColumn("2cm");
                dataTable.AddColumn("2cm");
                dataTable.AddColumn("3cm");

                decimal totalBalance = 0;
                decimal totalDebit = 0;
                decimal totalCredit = 0;

                foreach(DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.IsNewRow) continue;

                    string _account = row.Cells["lvl1"].Value?.ToString() ?? "";
                    if (_account.ToUpper().Contains("TOTAL")) continue;

                    string _date = row.Cells["date"].Value?.ToString() ?? "";
                    string _num = row.Cells["num"].Value?.ToString()??"";
                    string _adj = row.Cells["adj"].Value?.ToString() ?? "";
                    string _name = row.Cells["name"].Value?.ToString() ?? "";
                    string _memo = row.Cells["memo"].Value?.ToString() ?? "";
                    string _clr = row.Cells["clr"].Value?.ToString() ?? "";
                    string _split = row.Cells["split"].Value?.ToString() ?? "";
                    string _debit = row.Cells["debit"].Value?.ToString() ?? "0";
                    string _credit = row.Cells["credit"].Value?.ToString() ?? "0";
                    string _balance = row.Cells["balance"].Value?.ToString() ?? "0";

                    decimal balance = 0;
                    decimal.TryParse(_balance, out balance);
                    decimal debit = 0;
                    decimal.TryParse(_debit, out  debit);
                    decimal credit = 0;
                    decimal.TryParse(_credit, out  credit);

                    Row tRow = dataTable.AddRow();
                    tRow.Cells[0].AddParagraph(_account);
                    tRow.Cells[1].AddParagraph(_date);
                    tRow.Cells[2].AddParagraph(_num);
                    tRow.Cells[3].AddParagraph(_adj);
                    tRow.Cells[4].AddParagraph(_num);
                    tRow.Cells[5].AddParagraph(_name);
                    tRow.Cells[6].AddParagraph(_memo);
                    tRow.Cells[7].AddParagraph(_clr);
                    tRow.Cells[8].AddParagraph(_split);
                    tRow.Cells[9].AddParagraph(debit.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
                    tRow.Cells[10].AddParagraph(credit.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
                    tRow.Cells[11].AddParagraph(balance.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
                    
                    totalDebit += debit;
                    totalCredit += credit;
                    totalBalance += balance;
                }

                // TOTAL row
                Row totalRow = dataTable.AddRow();
                totalRow.Cells[1].AddParagraph("TOTAL").Format.Font.Bold = true;
                totalRow.Cells[9].AddParagraph(totalDebit.ToString("N2"));
                totalRow.Cells[10].AddParagraph(totalCredit.ToString("N2"));
                Paragraph totalText = totalRow.Cells[11].AddParagraph(totalBalance.ToString("N2"));
                totalText.Format.Font.Bold = true;
                totalText.Format.Font.Underline = Underline.Single;
                totalRow.Cells[1].Format.Alignment = ParagraphAlignment.Right;

            }
            // Render and save
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), type == "detail" ? "BalanceSheetDetail.pdf" : "BalanceSheet.pdf");
            renderer.PdfDocument.Save(filePath);
            Process.Start("explorer.exe", filePath);
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = type == "detail" ? "BalanceSheetDetail.xlsx" : type == "Prev Year Comparison" ? "BalanceSheetPrevYearComparison.xlsx" : type == "by Class" ? "BalanceSheetByClass" : "BalanceSheet.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = (type == "detail" ? "BalanceSheetDetail" : type == "Prev Year Comparison" ? "BalanceSheetPrevYearComparison" : type == "by Class" ? "BalanceSheetByClass" : "BalanceSheet");
                    
                    Excel.Range headerRange0 = type == "detail" ? worksheet.Range["D1", "J1"] : type == "Prev Year Comparison" ? worksheet.Range["C1", "F1"] : worksheet.Range["A1", "B1"];
                    headerRange0.Merge();
                    headerRange0.Value = lblCompany.Text;
                    headerRange0.Font.Bold = true;
                    headerRange0.Font.Name = "Times New Roman";
                    headerRange0.Font.Size = 12;
                    headerRange0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    Excel.Range headerRange1 = type == "detail" ? worksheet.Range["D2", "J2"] : type == "Prev Year Comparison" ? worksheet.Range["C2", "F2"] : worksheet.Range["A2", "B2"];
                    headerRange1.Merge();
                    headerRange1.Value = label1.Text;
                    headerRange1.Font.Bold = true;
                    headerRange1.Font.Name = "Times New Roman";
                    headerRange1.Font.Size = 11;
                    headerRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    Excel.Range headerRange2 = type == "detail" ? worksheet.Range["D3", "J3"] : type == "Prev Year Comparison" ? worksheet.Range["C3", "F3"] : worksheet.Range["A3", "B3"];
                    headerRange2.Merge();
                    headerRange2.Value = "From " + dateTimePicker1.Text + " - To " + dateTimePicker2.Text;
                    headerRange2.Font.Bold = false;
                    headerRange2.Font.Name = "Times New Roman";
                    headerRange2.Font.Size = 9;
                    headerRange2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    // Merge and format header row (Date)
                    Excel.Range headerRange3 = type == "detail" ? worksheet.Range["B4", "C4"] : type == "Prev Year Comparison" ? worksheet.Range["C4", "F4"] : worksheet.Range["A4", "B4"];
                    headerRange3.Merge();
                    headerRange3.Value = DateTime.Now.ToString("MMM dd, yy");
                    headerRange3.Font.Bold = false;
                    headerRange3.Font.Name = "Times New Roman";
                    headerRange3.Font.Size = 9;
                    headerRange3.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    int rowIndex = 5;

                    // 1️⃣ Write header row first
                    for (int col = 0; col < dgvReport.Columns.Count; col++)
                    {
                        var column = dgvReport.Columns[col];

                        // Skip hidden columns if needed
                        if (!column.Visible) continue;
                        if (column.HeaderText.Length > 0)
                        {
                            var headerCell = worksheet.Cells[rowIndex, col];
                            headerCell.Value = column.HeaderText;  // show column header
                            headerCell.Font.Bold = true;
                            headerCell.Font.Name = "Times New Roman";
                            headerCell.Font.Size = 11;
                            headerCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        }
                    }

                    // 2️⃣ Write data rows
                    rowIndex++; // move to next row for data
                    foreach (DataGridViewRow row in dgvReport.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string account = row.Cells["lvl1"].Value?.ToString();

                        var accountCell = worksheet.Cells[rowIndex, 2];
                        if (type == "detail")
                        {
                            string _date = row.Cells["date"].Value?.ToString();
                            string _num = row.Cells["num"].Value?.ToString();
                            string _adj = row.Cells["adj"].Value?.ToString();
                            string _name = row.Cells["name"].Value?.ToString();
                            string _memo = row.Cells["memo"].Value?.ToString();
                            string _clr = row.Cells["clr"].Value?.ToString();
                            string _split = row.Cells["split"].Value?.ToString();
                            string _debit = row.Cells["debit"].Value?.ToString();
                            string _credit = row.Cells["credit"].Value?.ToString();

                            string _balance = row.Cells["balance"].Value?.ToString();
                            worksheet.Cells[rowIndex, 2].Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            worksheet.Cells[rowIndex, 3].Value = _date;
                            worksheet.Cells[rowIndex, 4].Value = _num;
                            worksheet.Cells[rowIndex, 5].Value = _adj;
                            worksheet.Cells[rowIndex, 6].Value = _name;
                            worksheet.Cells[rowIndex, 7].Value = _memo;
                            worksheet.Cells[rowIndex, 8].Value = _clr;
                            worksheet.Cells[rowIndex, 9].Value = _split;
                            worksheet.Cells[rowIndex, 10].Value = _debit;
                            worksheet.Cells[rowIndex, 11].Value = _credit;
                            worksheet.Cells[rowIndex, 12].Value = _balance;
                            for (int col = 2; col <= 12; col++)
                            {
                                var cell = worksheet.Cells[rowIndex, col];
                                cell.Font.Name = "Times New Roman";
                                if (account.ToUpper().Contains("TOTAL"))
                                {
                                    cell.Font.Size = 10;
                                    cell.Font.Bold = true;
                                    if (col == 12) // Balance column
                                    {
                                        cell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                                    }
                                }
                                else
                                {
                                    cell.Font.Size = 9;
                                }
                                if (col == 10 || col == 11 || col == 12) // Debit, Credit, Balance columns
                                {
                                    cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                }
                            }
                        }
                        else if (type == "Prev Year Comparison")
                        {
                            string amount1Value = row.Cells["balance1"].Value?.ToString();
                            string amount2Value = row.Cells["balance2"].Value?.ToString();
                            //amount1HeaderCell.Value = dgvCustomer.Columns["balance1"].HeaderCell.Value;
                            //amount2HeaderCell.Value = dgvCustomer.Columns["balance2"].HeaderCell.Value;
                            //changeHeaderCell.Value = dgvCustomer.Columns["change"].HeaderCell.Value;
                            //changePercentageHeaderCell.Value = dgvCustomer.Columns["change_percentage"].HeaderCell.Value;

                            var accountHeaderCell = worksheet.Cells[rowIndex, 2];
                            var amount1HeaderCell = worksheet.Cells[rowIndex, 3];
                            var amount2HeaderCell = worksheet.Cells[rowIndex, 4];
                            var changeHeaderCell = worksheet.Cells[rowIndex, 5];
                            var changePercentageHeaderCell = worksheet.Cells[rowIndex, 6];


                            accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            amount1HeaderCell.Value = amount1Value.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();
                            amount2HeaderCell.Value = amount2Value.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();

                            accountCell.Font.Name = "Times New Roman";
                            amount1HeaderCell.Font.Name = "Times New Roman";

                            if (account.ToUpper().Contains("TOTAL"))
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amount1HeaderCell.Font.Size = 10;
                                amount1HeaderCell.Font.Bold = true;
                                amount1HeaderCell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;

                                amount2HeaderCell.Font.Size = 10;
                                amount2HeaderCell.Font.Bold = true;
                                amount2HeaderCell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                            }
                            else
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amount1HeaderCell.Font.Size = 9;
                                amount2HeaderCell.Font.Size = 9;
                            }

                            amount1HeaderCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            amount2HeaderCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        }
                        else
                        {
                            string amount = row.Cells["balance"].Value?.ToString();
                            var amount0Cell = worksheet.Cells[rowIndex, 3];
                            var amountCell = worksheet.Cells[rowIndex, 4];

                            accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            amount0Cell.Value = amount.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();
                            amountCell.Value = amount.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();

                            accountCell.Font.Name = "Times New Roman";
                            amount0Cell.Font.Name = "Times New Roman";
                            amountCell.Font.Name = "Times New Roman";

                            if (account.ToUpper().Contains("TOTAL"))
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amount0Cell.Font.Size = 10;
                                amountCell.Font.Size = 10;
                                amount0Cell.Font.Bold = true;
                                amountCell.Font.Bold = true;
                                amount0Cell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                                amountCell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                            }
                            else
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amount0Cell.Font.Size = 9;
                                amountCell.Font.Size = 9;
                            }

                            amount0Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            amountCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        }

                        rowIndex++;
                    }

                    worksheet.Columns[2].AutoFit();
                    worksheet.Columns[3].AutoFit();
                    worksheet.Columns[4].AutoFit();

                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //open same excel file
                    Process.Start("explorer.exe", saveDialog.FileName);
                }
            }
        }

        private void dgvSales_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (Utilities.UserPermissionCheck("CustomerBalanceSummery"))
            //{
            //int id = int.Parse(dgvReport.CurrentRow.Cells["id"].Value.ToString());
            //frmLogin.frmMain.openChildForm(new frmCustomerBalanceDetails(id));
            //}
            if (e.RowIndex < 0) return;

            if (dgvReport.Rows[e.RowIndex].Cells["currLvl"].Value == null)
                return;

            int currLvl = int.Parse(dgvReport.Rows[e.RowIndex].Cells["currLvl"].Value.ToString());
            if (currLvl > 4) return;
            if (currLvl == 4)
            {
               //frmMain.openChildForm(new MasterTransactionByAccount("Transactions By Account", int.Parse(dgvReport.Rows[e.RowIndex].Cells["id"].Value.ToString()), dateTimePicker1.Value.ToString(), dateTimePicker1.Value.ToString()));
            }

            string state = dgvReport.Rows[e.RowIndex].Cells["state"].Value.ToString();
            string loadState = dgvReport.Rows[e.RowIndex].Cells["loadState"].Value.ToString();
            string cellValue = dgvReport.Rows[e.RowIndex].Cells["lvl1"].Value.ToString();
            string space = new string(' ', currLvl * 6 + 3);

            try
            {
                if (state == "e")
                {
                    if (loadState == "u")
                    {
                        int parentId = int.Parse(dgvReport.Rows[e.RowIndex].Cells["id"].Value.ToString());
                        List<DataGridViewRow> newRows = new List<DataGridViewRow>();

                        using (MySqlDataReader reader = ClinicDatabase.ExecuteReader($@"SELECT name as currname, concat(code,' - ',name) as name , id FROM tbl_coa_level_" + (currLvl + 1) + " WHERE main_id = @parentId ORDER BY code",
                                                                          ClinicDatabase.CreateParameterNew("parentId", parentId)))
                        {
                            int insertIndex = e.RowIndex + 1;

                            //dgvReport.Rows[e.RowIndex].Cells["balance"].Value = "";

                            while (reader.Read())
                            {
                                string nextState = (currLvl + 1 == 4) ? "n" : "e";
                                string nextIcon = (currLvl + 1 == 4) ? "  " : "►";
                                var balanceData = GetBalanceData(currLvl + 1, int.Parse(reader["id"].ToString()), dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                                if (balanceData != null)
                                {
                                    var newRow = new DataGridViewRow();
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = nextState });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "u" });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = currLvl + 1 });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = space + nextIcon + "     " + reader["name"].ToString() });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = reader["id"].ToString() });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = reader["currname"].ToString() });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });
                                    newRow.Height = dgvReport.Rows[e.RowIndex].Height;

                                    ApplyRowStyle(newRow, currLvl + 1);
                                    newRows.Add(newRow);
                                }
                            }

                            // Add all new rows in one go for better performance
                            foreach (var row in newRows)
                            {
                                dgvReport.Rows.Insert(insertIndex, row);
                                insertIndex++;
                            }
                        }

                        dgvReport.Rows[e.RowIndex].Cells["loadState"].Value = "l";
                    }
                    else
                    {
                        //dgvReport.Rows[e.RowIndex].Cells["balance"].Value = "";

                        GetAllChildren(e.RowIndex);
                    }

                    dgvReport.Rows[e.RowIndex].Cells["lvl1"].Value = cellValue.Replace("►", "▼");
                    dgvReport.Rows[e.RowIndex].Cells["state"].Value = "c";
                }
                else if (state == "c")
                {
                    var parentBalanceData = GetBalanceData(currLvl, int.Parse(dgvReport.Rows[e.RowIndex].Cells["id"].Value.ToString().Replace("▼", "")), dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);

                    if (!string.IsNullOrEmpty(parentBalanceData))
                    {
                        //dgvReport.Rows[e.RowIndex].Cells["balance"].Value = parentBalanceData;
                    }
                    ToggleChildRowsVisibility(e.RowIndex, currLvl, false);
                    dgvReport.Rows[e.RowIndex].Cells["lvl1"].Value = cellValue.Replace("▼", "►");
                    dgvReport.Rows[e.RowIndex].Cells["state"].Value = "e";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyRowStyle(DataGridViewRow row, int level)
        {
            //if (level == 2)
            //    row.DefaultCellStyle.BackColor = row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            //else if (level == 3)
            //    row.DefaultCellStyle.BackColor = row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Gainsboro;
            //else
            //    row.DefaultCellStyle.BackColor = row.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Silver;
        }
        private string GetBalanceData(int level, int id, DateTime fromDate, DateTime toDate)
        {
            string filterDate = (type == "detail")
                ? " AND tt.date BETWEEN @fromDate AND @toDate"
                : " AND tt.date <= @fromDate";

            string query = $@"
                            SELECT 
                                COALESCE(SUM(
                                    CASE 
                                        WHEN l1.category_code = 'ASSET'
                                            THEN tt.debit - tt.credit

                                        WHEN l1.category_code IN ('LIABILITY','EQUITY','INCOME')
                                            THEN tt.credit - tt.debit

                                        WHEN l1.category_code IN ('COST','EXPENSE')
                                            THEN tt.debit - tt.credit
                                    END
                                ),0) AS balance

                            FROM tbl_transaction tt
                            JOIN tbl_coa_level_4 l4 ON l4.id = tt.account_id
                            JOIN tbl_coa_level_3 l3 ON l3.id = l4.main_id
                            JOIN tbl_coa_level_2 l2 ON l2.id = l3.main_id
                            JOIN tbl_coa_level_1 l1 ON l1.id = l2.main_id

                            WHERE tt.state = 0
                            {filterDate}
                            AND (
                                CASE {level}
                                    WHEN 1 THEN l1.id = @id
                                    WHEN 2 THEN l2.id = @id
                                    WHEN 3 THEN l3.id = @id
                                    WHEN 4 THEN l4.id = @id
                                END
                            )";

            object result = ClinicDatabase.ExecuteScalar(
                query,
                ClinicDatabase.CreateParameterNew("@id", id),
                ClinicDatabase.CreateParameterNew("@fromDate", fromDate),
                ClinicDatabase.CreateParameterNew("@toDate", toDate)
            );

            string bal = result?.ToString() ?? "0";
            decimal balance = Convert.ToDecimal(bal);

            //if (level == 3 && id == RetainedEarningsId)
            //{
            //    balance = GetNetProfit(fromDate, toDate);
            //}

            //object result0 = ClinicDatabase.ExecuteScalar("SELECT l1.id FROM tbl_coa_level_1 l1 Inner join tbl_coa_level_2 l2 ON l1.id = l2.main_id where category_code = 'EQUITY' and l2.id = @id", ClinicDatabase.CreateParameterNew("@id", id));
            
            //if(result0 != null)
            //{
            //    if (retainedEarningsBalance != 0)
            //    {
            //        balance = balance + retainedEarningsBalance;
            //    }
            //}


            return balance.ToString();
        }
        private string GetBalanceDataOrginal(int level, int id, DateTime cutoffDate, DateTime toDated)
        {
            // Determine date filter dynamically
            string filterDate = (type == "detail")
                ? " AND tt.date BETWEEN @fromDate AND @toDate"
                : " AND tt.date <= @fromDate";

            string query = $@"
                            SELECT 
                                t1.id AS level1_id, 
                                t2.id AS level2_id, 
                                t3.id AS level3_id, 
                                t4.id AS level4_id,

                                -- Balance for Level 1
                                (
                                    SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                    FROM tbl_transaction tt
                                    WHERE tt.state = 0 {filterDate}
                                    AND tt.account_id IN (
                                        SELECT id FROM tbl_coa_level_4 
                                        WHERE main_id IN (
                                            SELECT id FROM tbl_coa_level_3 
                                            WHERE main_id IN (
                                                SELECT id FROM tbl_coa_level_2 
                                                WHERE main_id = t1.id
                                            )
                                        )
                                    )
                                ) AS level1_balance,

                                -- Balance for Level 2
                                (
                                    SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                    FROM tbl_transaction tt
                                    WHERE tt.state = 0 {filterDate}
                                    AND tt.account_id IN (
                                        SELECT id FROM tbl_coa_level_4 
                                        WHERE main_id IN (
                                            SELECT id FROM tbl_coa_level_3 
                                            WHERE main_id = t2.id
                                        )
                                    )
                                ) AS level2_balance,

                                -- Balance for Level 3
                                (
                                    SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                    FROM tbl_transaction tt
                                    WHERE tt.state = 0 {filterDate}
                                    AND tt.account_id IN (
                                        SELECT id FROM tbl_coa_level_4 
                                        WHERE main_id = t3.id
                                    )
                                ) AS level3_balance,

                                -- Balance for Level 4
                                (
                                    SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                    FROM tbl_transaction tt
                                    WHERE tt.state = 0 {filterDate}
                                    AND tt.account_id = t4.id
                                ) AS level4_balance,
                                0 t1main_id,t2.main_id AS t2main_id,t3.main_id AS t3main_id,t4.main_id AS t4main_id

                            FROM tbl_coa_level_1 t1
                            LEFT JOIN tbl_coa_level_2 t2 ON t2.main_id = t1.id
                            LEFT JOIN tbl_coa_level_3 t3 ON t3.main_id = t2.id
                            LEFT JOIN tbl_coa_level_4 t4 ON t4.main_id = t3.id

                            WHERE t1.category_code IN ('ASSET', 'LIABILITY', 'EQUITY') 
                              AND t{level}.id = @id

                            GROUP BY t1.id, t2.id, t3.id, t4.id;
                        ";

            using (MySqlDataReader read = ClinicDatabase.ExecuteReader(
                query,
                ClinicDatabase.CreateParameterNew("@id", id),
                ClinicDatabase.CreateParameterNew("@fromDate", cutoffDate),
                ClinicDatabase.CreateParameterNew("@toDate", toDated)))
            {
                if (read.Read())
                {
                    decimal balance = Convert.ToDecimal(read[$"level{level}_balance"]);
                    //if(level == 3 && id == RetainedEarningsId)
                    //{
                    //    decimal netProfit = GetNetProfit(cutoffDate, toDated);
                    //    balance += netProfit;
                    //}

                    return balance.ToString();
                }
            }

            return "0";
        }
        //private decimal GetNetProfit(DateTime fromDate, DateTime toDate)
        //{
        //    string query = @"
        //                    SELECT 
        //                        COALESCE(SUM(CASE WHEN l1.category_code = 'INCOME' 
        //                                          THEN t.credit - t.debit ELSE 0 END),0)
        //                      - COALESCE(SUM(CASE WHEN l1.category_code IN ('COST','EXPENSE') 
        //                                          THEN t.debit - t.credit ELSE 0 END),0)
        //                    FROM tbl_transaction t
        //                    JOIN tbl_coa_level_4 l4 ON l4.id = t.account_id
        //                    JOIN tbl_coa_level_3 l3 ON l3.id = l4.main_id
        //                    JOIN tbl_coa_level_2 l2 ON l2.id = l3.main_id
        //                    JOIN tbl_coa_level_1 l1 ON l1.id = l2.main_id
        //                    WHERE t.state = 0
        //                      AND t.date <= @toDate";

        //    object result = ClinicDatabase.ExecuteScalar(
        //        query,
        //        ClinicDatabase.CreateParameterNew("@toDate", fromDate)
        //    );
        //    if (result == null)
        //        return 0;
        //    else
        //        return Convert.ToDecimal(result);
        //}
        private (decimal balance1, decimal balance2) GetBalanceData(int level, string name, DateTime currentFrom, DateTime currentTo, DateTime previousFrom, DateTime previousTo)
        {
            string query = $@"
                                SELECT 
                                    t1.id AS level1_id, 
                                    t2.id AS level2_id, 
                                    t3.id AS level3_id, 
                                    t4.id AS level4_id,

                                    -- Current Period Balance for Level 1
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @currFrom AND @currTo
                                          AND tt.account_id IN (
                                              SELECT id FROM tbl_coa_level_4 
                                              WHERE main_id IN (
                                                  SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id IN (
                                                      SELECT id FROM tbl_coa_level_2 
                                                      WHERE main_id = t1.id
                                                  )
                                              )
                                          )
                                    ) AS level1_balance_curr,

                                    -- Previous Period Balance for Level 1
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @prevFrom AND @prevTo
                                          AND tt.account_id IN (
                                              SELECT id FROM tbl_coa_level_4 
                                              WHERE main_id IN (
                                                  SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id IN (
                                                      SELECT id FROM tbl_coa_level_2 
                                                      WHERE main_id = t1.id
                                                  )
                                              )
                                          )
                                    ) AS level1_balance_prev,

                                    -- Current Period Balance for Level 2
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @currFrom AND @currTo
                                          AND tt.account_id IN (
                                              SELECT id FROM tbl_coa_level_4 
                                              WHERE main_id IN (
                                                  SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id = t2.id
                                              )
                                          )
                                    ) AS level2_balance_curr,

                                    -- Previous Period Balance for Level 2
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @prevFrom AND @prevTo
                                          AND tt.account_id IN (
                                              SELECT id FROM tbl_coa_level_4 
                                              WHERE main_id IN (
                                                  SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id = t2.id
                                              )
                                          )
                                    ) AS level2_balance_prev,

                                    -- Current Period Balance for Level 3
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @currFrom AND @currTo
                                          AND tt.account_id IN (
                                              SELECT id FROM tbl_coa_level_4 
                                              WHERE main_id = t3.id
                                          )
                                    ) AS level3_balance_curr,

                                    -- Previous Period Balance for Level 3
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @prevFrom AND @prevTo
                                          AND tt.account_id IN (
                                              SELECT id FROM tbl_coa_level_4 
                                              WHERE main_id = t3.id
                                          )
                                    ) AS level3_balance_prev,

                                    -- Current Period Balance for Level 4
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @currFrom AND @currTo
                                          AND tt.account_id = t4.id
                                    ) AS level4_balance_curr,

                                    -- Previous Period Balance for Level 4
                                    (
                                        SELECT COALESCE(SUM(
                                        CASE 
                                            WHEN t1.category_code = 'ASSET' 
                                                THEN tt.debit - tt.credit

                                            WHEN t1.category_code IN ('LIABILITY','EQUITY','INCOME') 
                                                THEN tt.credit - tt.debit

                                            WHEN t1.category_code IN ('COST','EXPENSE') 
                                                THEN tt.debit - tt.credit
                                        END
                                    ), 0)
                                        FROM tbl_transaction tt
                                        WHERE tt.state = 0
                                          AND tt.date BETWEEN @prevFrom AND @prevTo
                                          AND tt.account_id = t4.id
                                    ) AS level4_balance_prev

                                FROM tbl_coa_level_1 t1
                                LEFT JOIN tbl_coa_level_2 t2 ON t2.main_id = t1.id
                                LEFT JOIN tbl_coa_level_3 t3 ON t3.main_id = t2.id
                                LEFT JOIN tbl_coa_level_4 t4 ON t4.main_id = t3.id

                                WHERE t1.category_code IN ('ASSET', 'LIABILITY', 'EQUITY') 
                                  AND t{level}.name = @name

                                GROUP BY t1.id, t2.id, t3.id, t4.id;
                                ";

            using (MySqlDataReader read = ClinicDatabase.ExecuteReader(
                query,
                ClinicDatabase.CreateParameterNew("@name", name),
                ClinicDatabase.CreateParameterNew("@currFrom", currentFrom),
                ClinicDatabase.CreateParameterNew("@currTo", currentTo),
                ClinicDatabase.CreateParameterNew("@prevFrom", previousFrom),
                ClinicDatabase.CreateParameterNew("@prevTo", previousTo)
            ))
            {
                if (read.Read())
                {
                    decimal curr = Convert.ToDecimal(read[$"level{level}_balance_curr"]);
                    decimal prev = Convert.ToDecimal(read[$"level{level}_balance_prev"]);
                    return (curr, prev);
                }
            }
            return (0, 0);

        }

        private string GetBalanceDataXOld(int level, string name)
        {
            using (MySqlDataReader read = ClinicDatabase.ExecuteReader($@"SELECT 
                        t1.name AS level1_name, 
                        t2.name AS level2_name, 
                        t3.name AS level3_name, 
                        t4.name AS level4_name, 
                        t1.id AS level1_id, 
                        t2.id AS level2_id, 
                        t3.id AS level3_id, 
                        t4.id AS level4_id,

                        -- Balance for Level 1 (Summing all child transactions)
                        (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                         FROM tbl_transaction tt
                         WHERE  tt.state=0 and tt.account_id IN (
                             SELECT id FROM tbl_coa_level_4 
                             WHERE main_id IN (SELECT id FROM tbl_coa_level_3 
                                              WHERE main_id IN (SELECT id FROM tbl_coa_level_2 
                                                               WHERE main_id = t1.id))
                         )
                        ) AS level1_balance,

                        -- Balance for Level 2 (Summing its child transactions)
                        (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                         FROM tbl_transaction tt
                         WHERE  tt.state=0 and tt.account_id IN (
                             SELECT id FROM tbl_coa_level_4 
                             WHERE main_id IN (SELECT id FROM tbl_coa_level_3 
                                              WHERE main_id = t2.id)
                         )
                        ) AS level2_balance,

                        -- Balance for Level 3 (Summing only Level 4 transactions)
                        (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                         FROM tbl_transaction tt
                         WHERE  tt.state=0 and tt.account_id IN (
                             SELECT id FROM tbl_coa_level_4 
                             WHERE main_id = t3.id
                         )
                        ) AS level3_balance,

                        -- Balance for Level 4 (Direct transactions only)
                        COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) AS level4_balance

                    FROM tbl_coa_level_1 t1
                    LEFT JOIN tbl_coa_level_2 t2 ON t2.main_id = t1.id
                    LEFT JOIN tbl_coa_level_3 t3 ON t3.main_id = t2.id
                    LEFT JOIN tbl_coa_level_4 t4 ON t4.main_id = t3.id
                    LEFT JOIN tbl_transaction tt ON tt.account_id = t4.id  

                    WHERE  tt.state=0 and t1.category_code IN ('ASSET', 'LIABILITY', 'EQUITY') AND t{level}.name = @name

                    GROUP BY t1.id, t2.id, t3.id, t4.id;
                    ", ClinicDatabase.CreateParameterNew("name", name)))
            {
                if (read.Read())
                {
                    return read[$"level{level}_balance"].ToString();
                }
            }

            return null;
        }

        private void ToggleChildRowsVisibility(int parentRowIndex, int parentLevel, bool visible)
        {
            for (int i = parentRowIndex + 1; i < dgvReport.Rows.Count; i++)
            {
                int rowLevel = dgvReport.Rows[i].Cells["currLvl"].Value ==null ? 0 : int.Parse(dgvReport.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel > parentLevel)
                {
                    if (visible)
                    {
                        dgvReport.Rows[i].Visible = true;

                        if (dgvReport.Rows[i].Cells["state"].Value.ToString() == "c")
                            GetAllChildren(dgvReport.Rows[i].Index);
                    }
                    else
                        dgvReport.Rows[i].Visible = false;
                }

                else
                    break;
            }
        }

        private void GetAllChildren(int parentRowIndex)
        {
            int parentLevel = int.Parse(dgvReport.Rows[parentRowIndex].Cells["currLvl"].Value.ToString());

            for (int i = parentRowIndex + 1; i < dgvReport.Rows.Count; i++)
            {
                if (dgvReport.Rows[i].Cells["currLvl"].Value == null)
                    continue;
                int rowLevel = int.Parse(dgvReport.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel > parentLevel)
                {
                    if (rowLevel == parentLevel + 1)
                    {
                        dgvReport.Rows[i].Visible = true;
                        if (dgvReport.Rows[i].Cells["state"].Value.ToString() == "c")
                            GetAllChildren(i);
                    }
                    else
                        continue;
                }
                else
                    break;
            }

        }

        private DataGridViewRow GetParentRow(int currentRowIndex)
        {
            if (currentRowIndex <= 0) return null;

            int currentLevel = int.Parse(dgvReport.Rows[currentRowIndex].Cells["currLvl"].Value.ToString());

            for (int i = currentRowIndex - 1; i >= 0; i--)
            {
                int rowLevel = int.Parse(dgvReport.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel == currentLevel - 1)
                    return dgvReport.Rows[i];
            }

            return null;
        }

        private void ExpandAll()
        {
            for (int i = 0; i < dgvReport.Rows.Count; i++)
            {
                string state = dgvReport.Rows[i].Cells["state"].Value.ToString();
                string loadState = dgvReport.Rows[i].Cells["loadState"].Value.ToString();
                int currLvl = int.Parse(dgvReport.Rows[i].Cells["currLvl"].Value.ToString()??"0");

                if (state == "e")
                {
                    dgvSales_CellDoubleClick(dgvReport, new DataGridViewCellEventArgs(0, i));
                }

                // Re-check i after possible row insertions to ensure all nodes are processed
                if (currLvl < 4)
                {
                    i = Math.Max(i - 1, 0); // Step back and reprocess new rows
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //BindDGV();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            BindDGV();
        }
        private bool IsArabicReport()
        {
            return Clinic.Properties.Settings.Default.ApplicationLanguage
                ?.StartsWith("ar", StringComparison.OrdinalIgnoreCase) == true;
        }

        private string ReportTranslate(string english)
        {
            return TranslationClass.Translate(english, IsArabicReport());
        }

        private void ApplyBalanceSheetArabic()
        {
            bool isArabic = IsArabicReport();

            string title = type == "detail"
                ? "Balance Sheet Detail"
                : "Balance Sheet";

            headerUC1.FormText = ReportTranslate(title);

            label1.Text = ReportTranslate("Balance Sheet");

            if (type == "detail")
                label1.Text += isArabic ? " - تفصيلي" : " Detail";
            else if (type == "Prev Year Comparison")
                label1.Text += isArabic
                    ? " - مقارنة بالسنة السابقة"
                    : " Prev Year Comparison";
            else if (type == "by Class")
                label1.Text += isArabic ? " - حسب الفئة" : " By Class";

            btnPrint.Text = ReportTranslate("Print") + " ▼";

            // Translate toolbar and other controls.
            TranslateBalanceSheetControls(this);

            // Translate the export context menu.
            TranslateBalanceSheetMenu(contextMenuExport.Items);

            // Translate dynamically created grid headers.
            foreach (DataGridViewColumn column in dgvReport.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                {
                    column.HeaderText =
                        ReportTranslate(column.HeaderText);
                }

                column.HeaderCell.Style.Alignment = isArabic
                    ? DataGridViewContentAlignment.MiddleCenter
                    : DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void TranslateBalanceSheetControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button || control is Label ||
                    control is CheckBox || control is GroupBox)
                {
                    if (control.Tag == null)
                        control.Tag = control.Text;

                    string english = control.Tag as string;

                    if (!string.IsNullOrWhiteSpace(english))
                        control.Text = ReportTranslate(english);
                }

                if (control.HasChildren)
                    TranslateBalanceSheetControls(control);
            }
        }

        private void TranslateBalanceSheetMenu(
            ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (item.Tag == null)
                    item.Tag = item.Text;

                string english = item.Tag as string;

                if (!string.IsNullOrWhiteSpace(english))
                    item.Text = ReportTranslate(english);

                if (item is ToolStripDropDownItem dropDown)
                    TranslateBalanceSheetMenu(dropDown.DropDownItems);
            }
        }
        private void dateWords()
        {
            bool isArabic = IsArabicReport();

            string fromDate = dateTimePicker1.Value.ToString(
                "MMMM dd, yyyy",
                isArabic
                    ? new System.Globalization.CultureInfo("ar-AE")
                    : new System.Globalization.CultureInfo("en-US"));

            guna2HtmlLabel10.Text = isArabic
                ? $"اعتباراً من {fromDate}"
                : $"As of {fromDate}";
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dateTimePicker1, dateTimePicker1);
                BindDGV();
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            //
        }
    }
}