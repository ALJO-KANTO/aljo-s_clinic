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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YamyProject.Localization;
using Excel = Microsoft.Office.Interop.Excel;
using Clinic;
namespace YamyProject
{
    public partial class ProfitandLossReport : Form
    {
        string reportType = "";
        public ProfitandLossReport(string _type)
        {
            InitializeComponent();

            this.reportType = _type;

            LocalizationManager.LocalizeForm(this);

            headerUC1.ArabicLayout =
                TranslationClass.IsArabic;

            headerUC1.FormText =
                ReportTranslate("Profit and Loss");
        }

        void Dates(DateTime d)
        {
            DateTime sDateOfY = new DateTime(d.Year, 1, 1);
            DateTime eDateOfY = new DateTime(d.Year, 12, 31);

            dtFrom.Value = sDateOfY;
            dtTo.Value = eDateOfY;
        }
        private void FixProfitLossGridPosition()
        {
            Control container = dgvCustomer.Parent;

            if (container == null)
                return;

            container.SuspendLayout();

            try
            {
                dgvCustomer.Dock = DockStyle.Fill;
                dgvCustomer.Margin = Padding.Empty;

                dgvCustomer.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.None;

                if (dgvCustomer.Columns.Contains("lvl1"))
                {
                    dgvCustomer.Columns["lvl1"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;

                    dgvCustomer.Columns["lvl1"].MinimumWidth = 220;
                }

                foreach (DataGridViewColumn column
                         in dgvCustomer.Columns)
                {
                    if (!column.Visible ||
                        column.Name == "lvl1")
                        continue;

                    column.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.None;

                    column.Width = 125;
                }
            }
            finally
            {
                container.ResumeLayout(true);
            }
        }
        private void ProfitandLossReport_Load(
     object sender,
     EventArgs e)
        {
            Dates(DateTime.Now);

            using (var reader = ClinicDatabase.ExecuteReader(
                "SELECT name FROM tbl_company"))
            {
                if (reader.Read() &&
                    reader["name"] != DBNull.Value)
                {
                    lblCompany.Text =
                        reader["name"].ToString();
                }
            }

            dgvCustomer.Font = new System.Drawing.Font(
                "Segoe UI", 8, FontStyle.Bold);

            btnPrint.Text = "Print ▼";

            dateChangeTo();

            BindDGV_Data();

            DateWords();

            // Apply localization after dynamic report content is created.
            ApplyProfitLossLayout();

            // Fix report body positioning.
            FixProfitLossGridPosition();
        }
        DateTime prevFrom;
        DateTime prevTo;
        private void dateChangeTo()
        {
            if (reportType == "Prev Year Comparison" || reportType == "by Job" || reportType == "by class")
            {
                dtFrom.Value = new DateTime(DateTime.Now.Year, 1, 1);
            }
            prevFrom = new DateTime(dtFrom.Value.Year, 1, 1);
            prevTo = new DateTime(dtTo.Value.Year, dtTo.Value.Month, DateTime.DaysInMonth(dtTo.Value.Year, dtTo.Value.Month));
        }

        private void AddDGVColumnsAll()
        {
            dgvCustomer.Columns.Clear();
            if (reportType == "standard" || reportType == "Unclassified")
            {
                dgvCustomer.Columns.Add("state", "");
                dgvCustomer.Columns.Add("loadState", "");
                dgvCustomer.Columns.Add("currLvl", "");
                dgvCustomer.Columns.Add("lvl1", "   ");
                dgvCustomer.Columns.Add("id", "");
                dgvCustomer.Columns.Add("currname", "");
                dgvCustomer.Columns.Add("balance", "Amount");
                dgvCustomer.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCustomer.Columns["state"].Visible = dgvCustomer.Columns["currname"].Visible = dgvCustomer.Columns["id"].Visible = dgvCustomer.Columns["loadState"].Visible = dgvCustomer.Columns["currLvl"].Visible = false;
                dgvCustomer.Columns["balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //dgvCustomer.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
                //dgvCustomer.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                dgvCustomer.Columns["lvl1"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Format = "N2";
                dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            else if (reportType == "YTD Comparison")
            {
                string currentPeriod = $"{dtFrom.Value:MMM ''yy} – {dtTo.Value:MMM ''yy}";
                string previousPeriod = $"{prevFrom:MMM ''yy} – {prevTo:MMM ''yy}";
                var s = "";
                dgvCustomer.Columns.Add("state", "");
                dgvCustomer.Columns.Add("loadState", "");
                dgvCustomer.Columns.Add("currLvl", "");
                dgvCustomer.Columns.Add("lvl1", "Ordinary Income/Expense");
                dgvCustomer.Columns.Add("id", "");
                dgvCustomer.Columns.Add("currname", "");
                dgvCustomer.Columns.Add("balance1", currentPeriod);
                dgvCustomer.Columns.Add("balance2", previousPeriod);
                dgvCustomer.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCustomer.Columns["state"].Visible = dgvCustomer.Columns["currname"].Visible = dgvCustomer.Columns["id"].Visible = dgvCustomer.Columns["loadState"].Visible = dgvCustomer.Columns["currLvl"].Visible = false;
                dgvCustomer.Columns["balance1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["balance2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["lvl1"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance1"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance1"].HeaderCell.Style.Format = dgvCustomer.Columns["balance2"].HeaderCell.Style.Format = "N2";
                dgvCustomer.Columns["balance2"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //dgvCustomer.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
                //dgvCustomer.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            else if (reportType == "Prev Year Comparison")
            {
                dgvCustomer.Dock = DockStyle.Fill;
                string currentPeriod = $"{dtFrom.Value:MMM dd} – {dtTo.Value:MMM dd yy}";
                string previousPeriod = $"{prevFrom:MMM dd} – {prevTo:MMM dd yy}";

                dgvCustomer.Columns.Add("state", "");
                dgvCustomer.Columns.Add("loadState", "");
                dgvCustomer.Columns.Add("currLvl", "");
                dgvCustomer.Columns.Add("lvl1", "    ");
                dgvCustomer.Columns.Add("id", "");
                dgvCustomer.Columns.Add("currname", "");
                dgvCustomer.Columns.Add("balance1", currentPeriod);
                dgvCustomer.Columns.Add("balance2", previousPeriod);
                dgvCustomer.Columns.Add("change", "Change");
                dgvCustomer.Columns.Add("change_percentage", "% Change");
                dgvCustomer.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCustomer.Columns["state"].Visible = dgvCustomer.Columns["currname"].Visible = dgvCustomer.Columns["id"].Visible = dgvCustomer.Columns["loadState"].Visible = dgvCustomer.Columns["currLvl"].Visible = false;
                dgvCustomer.Columns["balance1"].DefaultCellStyle.Alignment =
                    dgvCustomer.Columns["balance2"].DefaultCellStyle.Alignment =
                    dgvCustomer.Columns["change"].DefaultCellStyle.Alignment =
                    dgvCustomer.Columns["change_percentage"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["lvl1"].HeaderCell.Style.Alignment =
                    dgvCustomer.Columns["balance1"].HeaderCell.Style.Alignment =
                    dgvCustomer.Columns["balance2"].HeaderCell.Style.Alignment =
                    dgvCustomer.Columns["change"].HeaderCell.Style.Alignment =
                    dgvCustomer.Columns["change_percentage"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance1"].HeaderCell.Style.Format = dgvCustomer.Columns["balance2"].HeaderCell.Style.Format = dgvCustomer.Columns["change"].HeaderCell.Style.Format = dgvCustomer.Columns["change_percentage"].HeaderCell.Style.Format = "N2";
                dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //dgvCustomer.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
                //dgvCustomer.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            else if(reportType == "by Job")
            {
                dgvCustomer.Columns.Add("state", "");
                dgvCustomer.Columns.Add("loadState", "");
                dgvCustomer.Columns.Add("currLvl", "");
                dgvCustomer.Columns.Add("lvl1", "    ");
                dgvCustomer.Columns.Add("id", "");
                dgvCustomer.Columns.Add("currname", "");
                // here i want column name from job name(customer name) from tbl_customer and total amount from tbl_transaction
                dgvCustomer.Columns.Add("balance", "Total");
                dgvCustomer.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCustomer.Columns["state"].Visible = dgvCustomer.Columns["currname"].Visible = dgvCustomer.Columns["id"].Visible = dgvCustomer.Columns["loadState"].Visible = dgvCustomer.Columns["currLvl"].Visible = false;
                dgvCustomer.Columns["balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //dgvCustomer.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
                //dgvCustomer.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                dgvCustomer.Columns["lvl1"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Format = "N2";
                dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            else if(reportType == "by Class")
            {
                dgvCustomer.Columns.Add("state", "");
                dgvCustomer.Columns.Add("loadState", "");
                dgvCustomer.Columns.Add("currLvl", "");
                dgvCustomer.Columns.Add("lvl1", "    ");
                dgvCustomer.Columns.Add("id", "");
                dgvCustomer.Columns.Add("currname", "");
                dgvCustomer.Columns.Add("balance", "Unclassified");
                dgvCustomer.Columns.Add("balance1", "Total");
                dgvCustomer.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCustomer.Columns["state"].Visible = dgvCustomer.Columns["currname"].Visible = dgvCustomer.Columns["id"].Visible = dgvCustomer.Columns["loadState"].Visible = dgvCustomer.Columns["currLvl"].Visible = false;
                dgvCustomer.Columns["balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["balance1"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //dgvCustomer.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
                //dgvCustomer.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                dgvCustomer.Columns["lvl1"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance1"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Format = "N2";
                dgvCustomer.Columns["balance1"].HeaderCell.Style.Format = "N2";
                dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            else if (reportType == "Unclassified")
            {
                dgvCustomer.Columns.Add("state", "");
                dgvCustomer.Columns.Add("loadState", "");
                dgvCustomer.Columns.Add("currLvl", "");
                dgvCustomer.Columns.Add("lvl1", "    ");
                dgvCustomer.Columns.Add("id", "");
                dgvCustomer.Columns.Add("currname", "");
                dgvCustomer.Columns.Add("balance", "Amount");
                dgvCustomer.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCustomer.Columns["state"].Visible = dgvCustomer.Columns["currname"].Visible = dgvCustomer.Columns["id"].Visible = dgvCustomer.Columns["loadState"].Visible = dgvCustomer.Columns["currLvl"].Visible = false;
                dgvCustomer.Columns["balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //dgvCustomer.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
                //dgvCustomer.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                dgvCustomer.Columns["lvl1"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns["balance"].HeaderCell.Style.Format = "N2";
                dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
        }

        public void BindDGV_Data()
        {
            AddDGVColumnsAll();

            if (reportType == "standard")
            {
                dgvCustomer.Rows.Clear();
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                        WITH AccountBalances AS (
                        SELECT 
                            l1.id,
                            l1.name,
                            l1.category_code,
                            CASE 
                                WHEN l1.category_code = 'Income'
                                    THEN COALESCE(SUM(t.credit) - SUM(t.debit), 0)
                                ELSE
                                    COALESCE(SUM(t.debit) - SUM(t.credit), 0)
                            END AS balance,
                            CASE
                                WHEN l1.category_code = 'Income'  THEN 1
                                WHEN l1.category_code = 'Cost'    THEN 2
                                WHEN l1.category_code = 'EXPENSE' THEN 4
                            END AS sort_order
                        FROM tbl_coa_level_1 l1
                        LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                        LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                        LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                        LEFT JOIN tbl_transaction t ON t.account_id = l4.id AND t.date BETWEEN @sDate AND @eDate
                        WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                        GROUP BY l1.id, l1.name, l1.category_code
                    ),

                    TotalIncome AS (
                        SELECT
                            -1 AS id,
                            'Total Income' AS name,
                            'TOTAL_INCOME' AS category_code,
                            SUM(balance) AS balance,
                            1.5 AS sort_order
                        FROM AccountBalances
                        WHERE category_code = 'Income'
                    ),

                    GrossProfit AS (
                        SELECT
                            0 AS id,
                            'Gross Profit' AS name,
                            'GROSS_PROFIT' AS category_code,
                            COALESCE(
                                (SELECT SUM(balance) FROM AccountBalances WHERE category_code = 'Income'), 0
                            ) -
                            COALESCE(
                                (SELECT SUM(balance) FROM AccountBalances WHERE category_code = 'Cost'), 0
                            ) AS balance,
                            3 AS sort_order
                    ),

                    TotalExpenses AS (
                        SELECT
                            -2 AS id,
                            'Total Expenses' AS name,
                            'TOTAL_EXPENSE' AS category_code,
                            SUM(balance) AS balance,
                            4.5 AS sort_order
                        FROM AccountBalances
                        WHERE category_code = 'EXPENSE'
                    )

                    SELECT id, name, balance, sort_order
                    FROM AccountBalances
                    WHERE category_code IN ('Income', 'Cost')

                    UNION ALL
                    SELECT id, name, balance, sort_order FROM TotalIncome
                    UNION ALL
                    SELECT id, name, balance, sort_order FROM GrossProfit
                    UNION ALL
                    SELECT id, name, balance, sort_order FROM AccountBalances WHERE category_code = 'EXPENSE'
                    UNION ALL
                    SELECT id, name, balance, sort_order FROM TotalExpenses

                    ORDER BY sort_order;

                        ", ClinicDatabase.CreateParameterNew("sDate", dtFrom.Value.Date), ClinicDatabase.CreateParameterNew("eDate", dtTo.Value.Date)))
                {
                    while (reader.Read())
                    {
                        dgvCustomer.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                            reader["id"].ToString(), reader["name"].ToString(),
                            Utilities.FormatDecimal(reader["balance"].ToString()));
                    }
                    // Auto-expand everything
                    for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                    {
                        ExpandRowRecursively(i);
                    }

                    // ===============================
                    // APPEND GROSS & NET PROFIT LAST
                    // ===============================

                    // Calculate totals from already-expanded rows
                    decimal totalIncome = 0;
                    decimal totalCOGS = 0;
                    decimal totalExpense = 0;

                    foreach (DataGridViewRow row in dgvCustomer.Rows)
                    {
                        string name = row.Cells["currname"].Value?.ToString()?.ToUpper() ?? "";
                        decimal amount = 0;
                        decimal.TryParse(row.Cells["balance"].Value?.ToString(), out amount);
                        int currLvl = int.Parse(row.Cells["currLvl"].Value.ToString());

                        //if (name == ("INCOME") && totalIncome == 0)
                        //    totalIncome += decimal.Parse(GetBalanceData(1, name));

                        //else if (name == ("COST") && totalCOGS ==0)
                        //    totalCOGS += decimal.Parse(GetBalanceData(1, name));

                        //else if (name == ("EXPENSE") && totalExpense ==0)
                        //    totalExpense += decimal.Parse(GetBalanceData(1, name));
                    }
                    using(MySqlDataReader mySqlDataReader = ClinicDatabase.ExecuteReader(@"
                            SELECT
                                t1.category_code,
                                IFNULL(COALESCE(SUM(tt.debit) - SUM(tt.credit), 0),0) AS total_balance
                            FROM tbl_coa_level_1 t1
                            JOIN tbl_coa_level_2 t2 ON t2.main_id = t1.id
                            JOIN tbl_coa_level_3 t3 ON t3.main_id = t2.id
                            JOIN tbl_coa_level_4 t4 ON t4.main_id = t3.id
                            LEFT JOIN tbl_transaction tt ON tt.account_id = t4.id AND tt.date BETWEEN @sDate AND @eDate
                            WHERE t1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY t1.category_code;
                            ", ClinicDatabase.CreateParameterNew("sDate", dtFrom.Value.Date), ClinicDatabase.CreateParameterNew("eDate", dtTo.Value.Date))
                        )
                    {
                        while (mySqlDataReader.Read())
                        {
                            string categoryCode = mySqlDataReader["category_code"].ToString();
                            decimal totalBalance = Convert.ToDecimal(mySqlDataReader["total_balance"].ToString().Replace("-",""));
                            if (categoryCode.ToLower() == "income")
                                totalIncome = totalBalance;
                            else if (categoryCode.ToLower() == "cost")
                                totalCOGS = totalBalance;
                            else if (categoryCode.ToLower() == "expense")
                                totalExpense = totalBalance;
                        }
                    }

                    // Calculate profits
                    decimal grossProfit = totalIncome - totalCOGS;
                    decimal netProfit = grossProfit - totalExpense;

                    // Net Profit row
                    dgvCustomer.Rows.Add(
                        "n", "l", "1",
                        "   Net Profit",
                        "0", "Net Profit",
                        Utilities.FormatDecimal(netProfit)
                    );

                    // Make them bold
                    foreach (DataGridViewRow row in dgvCustomer.Rows)
                    {
                        string text = row.Cells["lvl1"].Value?.ToString()?.ToUpper() ?? "";
                        if (text.Contains("GROSS PROFIT") || text.Contains("NET PROFIT") || text.Contains("TOTAL"))
                        {
                            row.DefaultCellStyle.Font =
                                new System.Drawing.Font("Segoe UI", 8, FontStyle.Bold);
                        }
                    }
                }
            }
            else if (reportType == "YTD Comparison")
            {
                dgvCustomer.Rows.Clear();
                headerUC1.FormText = this.Text + " " + reportType;
                guna2HtmlLabel7.Text = this.Text + " " + reportType;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                        WITH AccountBalances AS (
                            -- Calculate balances for Income, Cost, and Expenses
                            SELECT 
                                l1.name AS Category, 
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @currentFrom AND @currentTo THEN t.debit - t.credit END),0) AS Balance1,
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @prevFrom AND @prevTo THEN t.debit - t.credit END),0) AS Balance2
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.name
                        ),

                        FinalBalance AS (
                            -- Calculate final balance: Income - Cost - Expenses
                            SELECT 
                                'Final Balance (Income - Cost - Expenses)' AS Category,
                                COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='Income'),0) 
                                    - COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='Cost'),0)
                                    - COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='EXPENSE'),0) AS Balance1,

                                COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='Income'),0) 
                                    - COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='Cost'),0)
                                    - COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='EXPENSE'),0) AS Balance2
                        ),

                        EquityBalances AS (
                            -- Calculate balances for Assets, Liabilities, and Equity
                            SELECT 
                                l1.name, 
                                l1.id, 
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @currentFrom AND @currentTo THEN t.debit - t.credit END),0) AS Balance1,
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @prevFrom AND @prevTo THEN t.debit - t.credit END),0) AS Balance2
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.id, l1.name
                        )

                        -- Final result combining the balances
                        SELECT 
                            eb.name, 
                            eb.id, 
                            CASE 
                                WHEN eb.name='Equity' THEN eb.Balance1 + (SELECT Balance1 FROM FinalBalance)
                                ELSE eb.Balance1
                            END AS Balance1,
                            CASE 
                                WHEN eb.name='Equity' THEN eb.Balance2 + (SELECT Balance2 FROM FinalBalance)
                                ELSE eb.Balance2
                            END AS Balance2
                        FROM EquityBalances eb
                        ORDER BY eb.id;

                        ",
                        ClinicDatabase.CreateParameterNew("@currentFrom", dtFrom.Value),
                        ClinicDatabase.CreateParameterNew("@currentTo", dtTo.Value),
                        ClinicDatabase.CreateParameterNew("@prevFrom", prevFrom),
                        ClinicDatabase.CreateParameterNew("@prevTo", prevTo)
                        )
                    )
                {
                    while (reader.Read())
                        dgvCustomer.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                            reader["id"].ToString(), reader["name"].ToString(),
                            Utilities.FormatDecimal(reader["balance1"].ToString()), Utilities.FormatDecimal(reader["balance2"].ToString()));

                    // Auto-expand everything
                    for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                    {
                        ExpandRowRecursively(i);
                    }
                }
            }
            else if (reportType == "Prev Year Comparison")
            {
                dgvCustomer.Rows.Clear();
                headerUC1.FormText = this.Text + " " + reportType;
                guna2HtmlLabel7.Text = this.Text + " " + reportType;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                        WITH AccountBalances AS (
                            -- Calculate balances for Income, Cost, and Expenses
                            SELECT 
                                l1.name AS Category, 
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @currentFrom AND @currentTo THEN t.debit - t.credit END),0) AS Balance1,
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @prevFrom AND @prevTo THEN t.debit - t.credit END),0) AS Balance2
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.name
                        ),

                        FinalBalance AS (
                            -- Calculate final balance: Income - Cost - Expenses
                            SELECT 
                                'Final Balance (Income - Cost - Expenses)' AS Category,
                                COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='Income'),0) 
                                    - COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='Cost'),0)
                                    - COALESCE((SELECT Balance1 FROM AccountBalances WHERE Category='EXPENSE'),0) AS Balance1,

                                COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='Income'),0) 
                                    - COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='Cost'),0)
                                    - COALESCE((SELECT Balance2 FROM AccountBalances WHERE Category='EXPENSE'),0) AS Balance2
                        ),

                        EquityBalances AS (
                            -- Calculate balances for Assets, Liabilities, and Equity
                            SELECT 
                                l1.name, 
                                l1.id, 
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @currentFrom AND @currentTo THEN t.debit - t.credit END),0) AS Balance1,
                                COALESCE(SUM(CASE WHEN t.date BETWEEN @prevFrom AND @prevTo THEN t.debit - t.credit END),0) AS Balance2
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.id, l1.name
                        )

                        -- Final result combining the balances
                        SELECT 
                            eb.name, 
                            eb.id, 
                            CASE 
                                WHEN eb.name='Equity' THEN eb.Balance1 + (SELECT Balance1 FROM FinalBalance)
                                ELSE eb.Balance1
                            END AS Balance1,
                            CASE 
                                WHEN eb.name='Equity' THEN eb.Balance2 + (SELECT Balance2 FROM FinalBalance)
                                ELSE eb.Balance2
                            END AS Balance2
                        FROM EquityBalances eb
                        ORDER BY eb.id;

                        ",
                        ClinicDatabase.CreateParameterNew("@currentFrom", dtFrom.Value),
                        ClinicDatabase.CreateParameterNew("@currentTo", dtTo.Value),
                        ClinicDatabase.CreateParameterNew("@prevFrom", prevFrom),
                        ClinicDatabase.CreateParameterNew("@prevTo", prevTo)
                        )
                    )
                {
                    while (reader.Read())
                    {
                        decimal balance1 = Convert.ToDecimal(reader["balance1"] ?? 0);
                        decimal balance2 = Convert.ToDecimal(reader["balance2"] ?? 0);
                        // Calculate $ Change
                        decimal change = balance1 - balance2;

                        // Calculate % Change (avoid division by zero)
                        decimal percentChange = (balance2 != 0)
                            ? (change / balance2) * 100
                            : 0;

                        dgvCustomer.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                                                reader["id"].ToString(), reader["name"].ToString(), Utilities.FormatDecimal(balance1),
                                                Utilities.FormatDecimal(balance2), Utilities.FormatDecimal(change), $"{percentChange:N1}%"
                                             );
                    }
                    // Auto-expand everything
                    for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                    {
                        ExpandRowRecursively(i);
                    }
                }
            }
            else if (reportType == "by Job")
            {
                headerUC1.FormText = this.Text + " " + reportType;
                guna2HtmlLabel7.Text = this.Text + " " + reportType;
                
                //dgvCustomer.Columns.Clear();
                DataTable customersTable = ClinicDatabase.ExecuteDataTable(@"SELECT DISTINCT 
                                                                            c.name,c.id
                                                                        FROM tbl_customer AS c
                                                                        JOIN tbl_transaction AS t 
                                                                            ON c.id = t.hum_id
                                                                        WHERE t.hum_id <> 0
                                                                          AND EXISTS (
                                                                                SELECT 1
                                                                                FROM tbl_transaction AS tx
                                                                                LEFT JOIN tbl_coa_level_4 AS l4 ON tx.account_id = l4.id
                                                                                LEFT JOIN tbl_coa_level_3 AS l3 ON l4.main_id = l3.id
                                                                                LEFT JOIN tbl_coa_level_2 AS l2 ON l3.main_id = l2.id
                                                                                LEFT JOIN tbl_coa_level_1 AS l1 ON l2.main_id = l1.id
                                                                                WHERE 
                                                                                    (tx.id = t.id
                                                                                     OR (tx.type = t.type AND tx.transaction_id = t.transaction_id))
                                                                                  AND l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                                                                            )
                                                                          AND t.type IN (
                                                                                'Customer Receipt',
                                                                                'Sales Invoice',
                                                                                'Customer Opening Balance',
                                                                                'Customer Advance Payment',
                                                                                'Check Cancel (Customer)',
                                                                                'SalesReturn Invoice',
                                                                                'Credit Note',
                                                                                'PDC Receivable'
                                                                            )
                                                                          AND t.date BETWEEN @sDate AND @eDate
                                                                        ORDER BY c.name;
                                                                      ", 
                                                                      ClinicDatabase.CreateParameterNew("sDate", dtFrom.Value),
                                                                      ClinicDatabase.CreateParameterNew("eDate", dtTo.Value));
                // Build dynamic CASE SUM(...) for each customer
                string pivotColumns = string.Join(", ",
                                                        customersTable.Rows.Cast<DataRow>().Select(c =>
                                                        {
                                                            string customerName = c["name"].ToString();
                                                            string customerId = c["id"].ToString();

                                                            return $@"
                                                                SUM(
                                                                    CASE 
                                                                        WHEN t.hum_id = {customerId} THEN 
                                                                            CASE 
                                                                                WHEN t.type = 'Sales Invoice Cash' THEN -(COALESCE(t.debit - t.credit, 0))
                                                                                ELSE COALESCE(t.debit - t.credit, 0)
                                                                            END
                                                                        ELSE 0
                                                                    END
                                                                ) AS `{MySqlHelper.EscapeString(customerName)}`
                                                            ";
                                                        })
                                                    );


                // Build final SQL
                string sql = $@"
                                SELECT 
                                    l1.id,
                                    l1.name AS AccountName,
                                    {pivotColumns},
                                    SUM(
                                        CASE 
                                            WHEN t.type = 'Sales Invoice Cash' THEN -(COALESCE(t.debit - t.credit, 0))
                                            ELSE COALESCE(t.debit - t.credit, 0)
                                        END
                                    ) AS Total
                                FROM tbl_coa_level_1 AS l1
                                LEFT JOIN tbl_coa_level_2 AS l2 ON l2.main_id = l1.id
                                LEFT JOIN tbl_coa_level_3 AS l3 ON l3.main_id = l2.id
                                LEFT JOIN tbl_coa_level_4 AS l4 ON l4.main_id = l3.id
                                LEFT JOIN tbl_transaction AS t 
                                    ON t.account_id = l4.id
                                LEFT JOIN tbl_customer AS c 
                                    ON c.id = t.hum_id
                                WHERE t.hum_id <> 0
                                  AND EXISTS (
                                        SELECT 1
                                        FROM tbl_transaction AS tx
                                        LEFT JOIN tbl_coa_level_4 AS lx4 ON tx.account_id = lx4.id
                                        LEFT JOIN tbl_coa_level_3 AS lx3 ON lx4.main_id = lx3.id
                                        LEFT JOIN tbl_coa_level_2 AS lx2 ON lx3.main_id = lx2.id
                                        LEFT JOIN tbl_coa_level_1 AS lx1 ON lx2.main_id = lx1.id
                                        WHERE 
                                            (tx.id = t.id
                                             OR (tx.type = t.type AND tx.transaction_id = t.transaction_id))
                                          AND lx1.category_code IN ('Income', 'Cost', 'EXPENSE')
                                    )
                                  AND t.type IN (
                                        'Customer Receipt',
                                        'Sales Invoice',
                                        'Customer Opening Balance',
                                        'Customer Advance Payment',
                                        'Check Cancel (Customer)',
                                        'SalesReturn Invoice',
                                        'Credit Note',
                                        'PDC Receivable'
                                    )
                                  AND t.date BETWEEN @sDate AND @eDate
                                GROUP BY l1.id, l1.name
                                ORDER BY l1.id;
                                ";

                // Execute
                DataTable ResultData = ClinicDatabase.ExecuteDataTable(sql,
                    ClinicDatabase.CreateParameterNew("sDate", dtFrom.Value),
                    ClinicDatabase.CreateParameterNew("eDate", dtTo.Value));

                var op = "";
                dgvCustomer.Columns.Clear();
                dgvCustomer.DataSource = ResultData;
                dgvCustomer.Columns["id"].Visible = false;

            }
            else if(reportType == "by Class")
            {
                headerUC1.FormText = this.Text + " " + reportType;
                guna2HtmlLabel7.Text = this.Text + " " + reportType;

                dgvCustomer.Rows.Clear();
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                        WITH AccountBalances AS (
                            -- Calculate balances for Income, Cost, and Expenses
                            SELECT 
                                l1.name AS Category, 
                                COALESCE(SUM(t.debit) - SUM(t.credit), 0) AS Balance
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.name
                        ),

                        FinalBalance AS (
                            -- Calculate final balance: Income - Cost - Expenses
                            SELECT 
                                'Final Balance (Income - Cost - Expenses)' AS Category,
                                COALESCE(
                                    (SELECT Balance FROM AccountBalances WHERE Category = 'Income'), 0
                                ) -
                                COALESCE(
                                    (SELECT Balance FROM AccountBalances WHERE Category = 'Cost'), 0
                                ) -
                                COALESCE(
                                    (SELECT Balance FROM AccountBalances WHERE Category = 'EXPENSE'), 0
                                ) AS Balance
                        ),

                        EquityBalances AS (
                            -- Calculate balances for Assets, Liabilities, and Equity
                            SELECT 
                                l1.name, 
                                l1.id, 
                                COALESCE(SUM(t.debit) - SUM(t.credit), 0) AS balance
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.id, l1.name
                        )

                        -- Final result combining the balances
                        SELECT 
                            eb.name, 
                            eb.id, 
                            CASE 
                                -- For Equity, add the final balance from Income - Cost - Expenses to Retained Earnings
                                WHEN eb.name = 'Equity' THEN 
                                    eb.balance + 
                                    (SELECT Balance FROM FinalBalance)
                                ELSE 
                                    eb.balance
                            END AS balance
                        FROM EquityBalances eb
                        ORDER BY eb.id;

                        "))
                {
                    while (reader.Read())
                        dgvCustomer.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                            reader["id"].ToString(), reader["name"].ToString(),
                            Utilities.FormatDecimal(reader["balance"].ToString()), Utilities.FormatDecimal(reader["balance"].ToString()));

                    // Auto-expand everything
                    for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                    {
                        ExpandRowRecursively(i);
                    }
                }
            }
            else if(reportType == "Unclassified")
            {
                headerUC1.FormText = this.Text + " " + reportType;
                guna2HtmlLabel7.Text = this.Text + " " + reportType;

                dgvCustomer.Rows.Clear();
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                        WITH AccountBalances AS (
                            -- Calculate balances for Income, Cost, and Expenses
                            SELECT 
                                l1.name AS Category, 
                                COALESCE(SUM(t.debit) - SUM(t.credit), 0) AS Balance
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.name
                        ),

                        FinalBalance AS (
                            -- Calculate final balance: Income - Cost - Expenses
                            SELECT 
                                'Final Balance (Income - Cost - Expenses)' AS Category,
                                COALESCE(
                                    (SELECT Balance FROM AccountBalances WHERE Category = 'Income'), 0
                                ) -
                                COALESCE(
                                    (SELECT Balance FROM AccountBalances WHERE Category = 'Cost'), 0
                                ) -
                                COALESCE(
                                    (SELECT Balance FROM AccountBalances WHERE Category = 'EXPENSE'), 0
                                ) AS Balance
                        ),

                        EquityBalances AS (
                            -- Calculate balances for Assets, Liabilities, and Equity
                            SELECT 
                                l1.name, 
                                l1.id, 
                                COALESCE(SUM(t.debit) - SUM(t.credit), 0) AS balance
                            FROM tbl_coa_level_1 l1
                            LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                            LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                            LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                            LEFT JOIN tbl_transaction t ON t.account_id = l4.id
                            WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY l1.id, l1.name
                        )

                        -- Final result combining the balances
                        SELECT 
                            eb.name, 
                            eb.id, 
                            CASE 
                                -- For Equity, add the final balance from Income - Cost - Expenses to Retained Earnings
                                WHEN eb.name = 'Equity' THEN 
                                    eb.balance + 
                                    (SELECT Balance FROM FinalBalance)
                                ELSE 
                                    eb.balance
                            END AS balance
                        FROM EquityBalances eb
                        ORDER BY eb.id;

                        "))
                {
                    while (reader.Read())
                        dgvCustomer.Rows.Add("e", "u", "1", "   ►     " + reader["name"].ToString(),
                            reader["id"].ToString(), reader["name"].ToString(),
                            Utilities.FormatDecimal(reader["balance"].ToString()));

                    // Auto-expand everything
                    for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                    {
                        ExpandRowRecursively(i);
                    }
                }
            }
        }
        private void ExpandRowRecursively(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvCustomer.Rows.Count)
                return;

            DataGridViewRow row = dgvCustomer.Rows[rowIndex];
            int currLvl = int.Parse(row.Cells["currLvl"].Value.ToString());
            string state = row.Cells["state"].Value.ToString();
            string loadState = row.Cells["loadState"].Value.ToString();
            string cellValue = row.Cells["lvl1"].Value.ToString();
            string space = new string(' ', currLvl * 6 + 3);

            try
            {
                if (state == "e")
                {
                    if (loadState == "u")
                    {
                        int parentId = int.Parse(row.Cells["id"].Value.ToString());
                        List<DataGridViewRow> newRows = new List<DataGridViewRow>();

                        using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                            $@"SELECT name as currname, concat(code,' - ',name) as name , id 
                                FROM tbl_coa_level_" + (currLvl + 1) + " WHERE main_id = @parentId ORDER BY code",
                            ClinicDatabase.CreateParameterNew("parentId", parentId)))
                        {
                            int insertIndex = rowIndex + 1;
                            if (reportType == "standard" || reportType == "Unclassified")
                                if(int.Parse(row.Cells["id"].Value.ToString()) > 0 )
                                    dgvCustomer.Rows[rowIndex].Cells["balance"].Value = "";
                            else if (reportType == "YTD Comparison")
                                    if (int.Parse(row.Cells["id"].Value.ToString()) > 0) 
                                        dgvCustomer.Rows[rowIndex].Cells["balance1"].Value = dgvCustomer.Rows[rowIndex].Cells["balance2"].Value = "";
                            else if (reportType == "by Class")
                                        if (int.Parse(row.Cells["id"].Value.ToString()) > 0) 
                                            dgvCustomer.Rows[rowIndex].Cells["balance"].Value = dgvCustomer.Rows[rowIndex].Cells["balance1"].Value = "";

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
                                if (reportType == "YTD Comparison")
                                {
                                    var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString(), dtFrom.Value, dtTo.Value, prevFrom, prevTo);
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData.balance1) });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData.balance2) });
                                }
                                else if (reportType == "standard" || reportType == "Unclassified")
                                {
                                    var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString());
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });
                                } else if (reportType == "Prev Year Comparison")
                                {
                                    var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString(), dtFrom.Value, dtTo.Value, prevFrom, prevTo);
                                    decimal balance1 = balanceData.balance1;
                                    decimal balance2 = balanceData.balance2;
                                    decimal change = balance1 - balance2;
                                    decimal percentChange = (balance2 != 0) ? (change / balance2) * 100 : 0;
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balance1) });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balance2) });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(change) });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = $"{percentChange:N1}%" });
                                }
                                else if (reportType == "by Class")
                                {
                                    var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString());
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });
                                }
                                newRow.Height = dgvCustomer.Rows[rowIndex].Height;

                                ApplyRowStyle(newRow, currLvl + 1);
                                newRows.Add(newRow);

                            }

                            foreach (var r in newRows)
                            {
                                dgvCustomer.Rows.Insert(insertIndex, r);
                                insertIndex++;
                            }
                        }

                        dgvCustomer.Rows[rowIndex].Cells["loadState"].Value = "l";
                    }
                    else
                    {
                        dgvCustomer.Rows[rowIndex].Cells["balance"].Value = "";
                        GetAllChildren(rowIndex);
                    }

                    dgvCustomer.Rows[rowIndex].Cells["lvl1"].Value = cellValue.Replace("►", "▼");
                    dgvCustomer.Rows[rowIndex].Cells["state"].Value = "c";
                }

                // Expand child rows recursively
                int currentRowLevel = int.Parse(row.Cells["currLvl"].Value.ToString());
                int nextIndex = rowIndex + 1;
                while (nextIndex < dgvCustomer.Rows.Count)
                {
                    int childLvl = int.Parse(dgvCustomer.Rows[nextIndex].Cells["currLvl"].Value.ToString());
                    if (childLvl <= currentRowLevel)
                        break;

                    if (dgvCustomer.Rows[nextIndex].Cells["state"].Value.ToString() == "e")
                    {
                        ExpandRowRecursively(nextIndex);
                    }
                    nextIndex++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while expanding: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int currLvl = int.Parse(dgvCustomer.Rows[e.RowIndex].Cells["currLvl"].Value.ToString());
            if (currLvl > 4) return;
            if (currLvl == 4)
            {
                //frmLogin.frmMain.openChildForm(new MasterTransactionByAccount("Transactions Detail By Account", int.Parse(dgvCustomer.Rows[e.RowIndex].Cells["id"].Value.ToString()), dtFrom.Value.ToString(), dtTo.Value.ToString()));
            }

            string state = dgvCustomer.Rows[e.RowIndex].Cells["state"].Value.ToString();
            string loadState = dgvCustomer.Rows[e.RowIndex].Cells["loadState"].Value.ToString();
            string cellValue = dgvCustomer.Rows[e.RowIndex].Cells["lvl1"].Value.ToString();
            string space = new string(' ', currLvl * 6 + 3);

            try
            {
                if (state == "e")
                {
                    if (loadState == "u")
                    {
                        int parentId = int.Parse(dgvCustomer.Rows[e.RowIndex].Cells["id"].Value.ToString());
                        List<DataGridViewRow> newRows = new List<DataGridViewRow>();

                        using (MySqlDataReader reader = ClinicDatabase.ExecuteReader($@"SELECT name as currname, concat(code,' - ',name) as name , id FROM tbl_coa_level_" + (currLvl + 1) + " WHERE main_id = @parentId ORDER BY code",
                                                                          ClinicDatabase.CreateParameterNew("parentId", parentId)))
                        {
                            int insertIndex = e.RowIndex + 1;

                            //dgvCustomer.Rows[e.RowIndex].Cells["balance"].Value = "";

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


                                if (reportType == "standard" || reportType == "Unclassified")
                                {
                                    var balance = GetBalanceData(currLvl + 1, reader["currname"].ToString());

                                    if (balance != null)
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balance) });
                                }
                                else if (reportType == "YTD Comparison")
                                {
                                    var balances = GetBalanceData(currLvl + 1, reader["currname"].ToString(), dtFrom.Value, dtTo.Value, prevFrom, prevTo);

                                    if (balances.balance1 != null || balances.balance2 != null)
                                    {
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balances.balance1) });
                                        newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balances.balance2) });
                                    }
                                }
                                else if (reportType == "Prev Year Comparison")
                                {
                                    var balances = GetBalanceData(currLvl + 1, reader["currname"].ToString(), dtFrom.Value, dtTo.Value, prevFrom, prevTo);

                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balances.balance1) });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balances.balance2) });
                                    decimal change = balances.balance1 - balances.balance2;
                                    decimal percentChange = (balances.balance2 != 0) ? (change / balances.balance2) * 100 : 0;
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(change) });
                                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = $"{percentChange:N1}%" });
                                }

                                newRow.Height = dgvCustomer.Rows[e.RowIndex].Height;

                                ApplyRowStyle(newRow, currLvl + 1);
                                newRows.Add(newRow);

                            }

                            // Add all new rows in one go for better performance
                            foreach (var row in newRows)
                            {
                                dgvCustomer.Rows.Insert(insertIndex, row);
                                insertIndex++;
                            }
                        }

                        dgvCustomer.Rows[e.RowIndex].Cells["loadState"].Value = "l";
                    }
                    else
                    {
                        //dgvCustomer.Rows[e.RowIndex].Cells["balance"].Value = "";

                        GetAllChildren(e.RowIndex);
                    }

                    dgvCustomer.Rows[e.RowIndex].Cells["lvl1"].Value = cellValue.Replace("►", "▼");
                    dgvCustomer.Rows[e.RowIndex].Cells["state"].Value = "c";
                }
                else if (state == "c")
                {

                    if (reportType == "standard" || reportType == "Unclassified")
                    {
                        var balanceData = GetBalanceData(currLvl, dgvCustomer.Rows[e.RowIndex].Cells["currname"].Value.ToString().Replace("▼", ""));

                        if (balanceData != null)
                        {

                            dgvCustomer.Rows[e.RowIndex].Cells["balance"].Value = Utilities.FormatDecimal(balanceData);
                        }
                    }
                    if (reportType == "YTD Comparison")
                    {
                        var balances = GetBalanceData(currLvl, dgvCustomer.Rows[e.RowIndex].Cells["currname"].Value.ToString().Replace("▼", ""), dtFrom.Value, dtTo.Value, prevFrom, prevTo);
                        if (balances.balance1 != null && balances.balance2 != null)
                        {
                            dgvCustomer.Rows[e.RowIndex].Cells["balance1"].Value = Utilities.FormatDecimal(balances.balance1);
                            dgvCustomer.Rows[e.RowIndex].Cells["balance2"].Value = Utilities.FormatDecimal(balances.balance2);
                        }
                    }
                    else if (reportType == "Prev Year Comparison")
                    {
                        var balances = GetBalanceData(currLvl, dgvCustomer.Rows[e.RowIndex].Cells["currname"].Value.ToString().Replace("▼", ""), dtFrom.Value, dtTo.Value, prevFrom, prevTo);

                        dgvCustomer.Rows[e.RowIndex].Cells["balance1"].Value = Utilities.FormatDecimal(balances.balance1);
                        dgvCustomer.Rows[e.RowIndex].Cells["balance2"].Value = Utilities.FormatDecimal(balances.balance2);
                        decimal change = balances.balance1 - balances.balance2;
                        decimal percentChange = (balances.balance2 != 0) ? (change / balances.balance2) * 100 : 0;
                        dgvCustomer.Rows[e.RowIndex].Cells["change"].Value = Utilities.FormatDecimal(change);
                        dgvCustomer.Rows[e.RowIndex].Cells["change_percentage"].Value = $"{percentChange:N1}%";
                    }
            
                    ToggleChildRowsVisibility(e.RowIndex, currLvl, false);
                    dgvCustomer.Rows[e.RowIndex].Cells["lvl1"].Value = cellValue.Replace("▼", "►");
                    dgvCustomer.Rows[e.RowIndex].Cells["state"].Value = "e";
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

        private string GetBalanceData(int level, string name)
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
                             WHERE tt.account_id IN (
                                 SELECT id FROM tbl_coa_level_4 
                                 WHERE main_id IN (SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id IN (SELECT id FROM tbl_coa_level_2 
                                                                   WHERE main_id = t1.id))
                             ) AND tt.date BETWEEN @fromDate AND @toDate
                            ) AS level1_balance,

                            -- Balance for Level 2 (Summing its child transactions)
                            (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                             FROM tbl_transaction tt
                             WHERE tt.account_id IN (
                                 SELECT id FROM tbl_coa_level_4 
                                 WHERE main_id IN (SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id = t2.id)
                             ) AND tt.date BETWEEN @fromDate AND @toDate
                            ) AS level2_balance,

                            -- Balance for Level 3 (Summing only Level 4 transactions)
                            (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                             FROM tbl_transaction tt
                             WHERE tt.account_id IN (
                                 SELECT id FROM tbl_coa_level_4 
                                 WHERE main_id = t3.id
                             ) AND tt.date BETWEEN @fromDate AND @toDate
                            ) AS level3_balance,

                            -- Balance for Level 4 (Direct transactions only)
                            COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) AS level4_balance

                        FROM tbl_coa_level_1 t1
                        LEFT JOIN tbl_coa_level_2 t2 ON t2.main_id = t1.id
                        LEFT JOIN tbl_coa_level_3 t3 ON t3.main_id = t2.id
                        LEFT JOIN tbl_coa_level_4 t4 ON t4.main_id = t3.id
                        LEFT JOIN tbl_transaction tt ON tt.account_id = t4.id AND tt.date BETWEEN @fromDate AND @toDate 

                        WHERE t1.category_code IN ('Income', 'Cost', 'EXPENSE') AND t{level}.name = @name

                        GROUP BY t1.id, t2.id, t3.id, t4.id;
                        ", ClinicDatabase.CreateParameterNew("name", name), ClinicDatabase.CreateParameterNew("fromDate", dtFrom.Value.Date), ClinicDatabase.CreateParameterNew("toDate", dtTo.Value.Date)))
            {
                if (read.Read())
                {
                    return read[$"level{level}_balance"].ToString();
                }
            }

            return null;
        }
        private (decimal balance1, decimal balance2) GetBalanceData(int level, string name, DateTime currentFrom, DateTime currentTo, DateTime previousFrom, DateTime previousTo)
        {
            using (MySqlDataReader read = ClinicDatabase.ExecuteReader($@"
                    SELECT
                        -- Balance for current period
                        (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit),0)
                         FROM tbl_transaction tt
                         WHERE tt.account_id IN (
                             SELECT id FROM tbl_coa_level_4 
                             WHERE main_id IN (
                                 SELECT id FROM tbl_coa_level_3 
                                 WHERE main_id IN (
                                     SELECT id FROM tbl_coa_level_2 
                                     WHERE main_id = t1.id
                                 )
                             )
                         ) AND tt.date BETWEEN @currFrom AND @currTo
                        ) AS balance1,

                        -- Balance for previous period
                        (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit),0)
                         FROM tbl_transaction tt
                         WHERE tt.account_id IN (
                             SELECT id FROM tbl_coa_level_4 
                             WHERE main_id IN (
                                 SELECT id FROM tbl_coa_level_3 
                                 WHERE main_id IN (
                                     SELECT id FROM tbl_coa_level_2 
                                     WHERE main_id = t1.id
                                 )
                             )
                         ) AND tt.date BETWEEN @prevFrom AND @prevTo
                        ) AS balance2

                    FROM tbl_coa_level_1 t1
                    LEFT JOIN tbl_coa_level_2 t2 ON t2.main_id = t1.id
                    LEFT JOIN tbl_coa_level_3 t3 ON t3.main_id = t2.id
                    LEFT JOIN tbl_coa_level_4 t4 ON t4.main_id = t3.id
                    WHERE t{level}.name = @name
                    GROUP BY t1.id, t2.id, t3.id, t4.id;
                ",
                ClinicDatabase.CreateParameterNew("name", name),
                ClinicDatabase.CreateParameterNew("currFrom", currentFrom),
                ClinicDatabase.CreateParameterNew("currTo", currentTo),
                ClinicDatabase.CreateParameterNew("prevFrom", previousFrom),
                ClinicDatabase.CreateParameterNew("prevTo", previousTo)
            ))
            {
                if (read.Read())
                {
                    decimal b1 = Convert.ToDecimal(read["balance1"]);
                    decimal b2 = Convert.ToDecimal(read["balance2"]);
                    return (b1, b2);
                }
            }
            return (0, 0);
        }

        private void ToggleChildRowsVisibility(int parentRowIndex, int parentLevel, bool visible)
        {
            for (int i = parentRowIndex + 1; i < dgvCustomer.Rows.Count; i++)
            {
                int rowLevel = int.Parse(dgvCustomer.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel > parentLevel)
                {
                    if (visible)
                    {
                        dgvCustomer.Rows[i].Visible = true;

                        if (dgvCustomer.Rows[i].Cells["state"].Value.ToString() == "c")
                            GetAllChildren(dgvCustomer.Rows[i].Index);
                    }
                    else
                        dgvCustomer.Rows[i].Visible = false;
                }

                else
                    break;
            }
        }

        private void GetAllChildren(int parentRowIndex)
        {
            int parentLevel = int.Parse(dgvCustomer.Rows[parentRowIndex].Cells["currLvl"].Value.ToString());

            for (int i = parentRowIndex + 1; i < dgvCustomer.Rows.Count; i++)
            {
                int rowLevel = int.Parse(dgvCustomer.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel > parentLevel)
                {
                    if (rowLevel == parentLevel + 1)
                    {
                        dgvCustomer.Rows[i].Visible = true;
                        if (dgvCustomer.Rows[i].Cells["state"].Value.ToString() == "c")
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

            int currentLevel = int.Parse(dgvCustomer.Rows[currentRowIndex].Cells["currLvl"].Value.ToString());

            for (int i = currentRowIndex - 1; i >= 0; i--)
            {
                int rowLevel = int.Parse(dgvCustomer.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel == currentLevel - 1)
                    return dgvCustomer.Rows[i];
            }

            return null;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            contextMenuExport.Show(btnPrint, new Point(0, btnPrint.Height));
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = guna2HtmlLabel7.Text.Replace(" ", "") + ".xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = guna2HtmlLabel7.Text.Replace(" ", "");
                    if (reportType == "standard" || reportType == "Unclassified")
                    {
                        Excel.Range headerRange0 = worksheet.Range["A1", "C1"];
                        headerRange0.Merge();
                        headerRange0.Value = lblCompany.Text;
                        headerRange0.Font.Bold = true;
                        headerRange0.Font.Name = "Times New Roman";
                        headerRange0.Font.Size = 12;
                        headerRange0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange1 = worksheet.Range["A2", "C2"];
                        headerRange1.Merge();
                        headerRange1.Value = guna2HtmlLabel7.Text;
                        headerRange1.Font.Bold = true;
                        headerRange1.Font.Name = "Times New Roman";
                        headerRange1.Font.Size = 11;
                        headerRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange2 = worksheet.Range["A3", "C3"];
                        headerRange2.Merge();
                        headerRange2.Value = "From " + dtFrom.Text + " - To " + dtTo.Text;
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
                        headerRange3.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        int rowIndex = 5;
                        // 1️⃣ Write header row (aligned with data columns)
                        var accountHeaderCell = worksheet.Cells[rowIndex, 2];
                        var amountHeaderCell = worksheet.Cells[rowIndex, 3];

                        accountHeaderCell.Value = "Account";
                        amountHeaderCell.Value = "Balance";

                        foreach (var headerCell in new[] { accountHeaderCell, amountHeaderCell })
                        {
                            headerCell.Font.Bold = true;
                            headerCell.Font.Name = "Times New Roman";
                            headerCell.Font.Size = 11;
                            headerCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        }

                        // 2️⃣ Write data rows
                        rowIndex++; // move to next row for data
                        foreach (DataGridViewRow row in dgvCustomer.Rows)
                        {
                            if (row.IsNewRow) continue;


                            string account = row.Cells["lvl1"].Value?.ToString();
                            string amount = row.Cells["balance"].Value?.ToString();

                            var accountCell = worksheet.Cells[rowIndex, 2];
                            var amountCell = worksheet.Cells[rowIndex, 3];


                            //accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            string safeText = account.Replace("▶", "").Replace("▼", "").Trim();

                            // Excel max cell length
                            if (safeText.Length > 32767)
                                safeText = safeText.Substring(0, 32767);

                            accountCell.Value2 = safeText;

                            accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            amountCell.Value = amount.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();

                            accountCell.Font.Name = "Times New Roman";
                            amountCell.Font.Name = "Times New Roman";

                            if (account.ToUpper().Contains("TOTAL"))
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amountCell.Font.Size = 10;
                                amountCell.Font.Bold = true;
                                amountCell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                            }
                            else
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amountCell.Font.Size = 9;
                            }

                            amountCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;


                            rowIndex++;
                        }

                        worksheet.Columns[2].AutoFit();
                        worksheet.Columns[3].AutoFit();
                    }
                    else if (reportType == "YTD Comparison")
                    {
                        Excel.Range headerRange0 = worksheet.Range["A1", "C1"];
                        headerRange0.Merge();
                        headerRange0.Value = lblCompany.Text;
                        headerRange0.Font.Bold = true;
                        headerRange0.Font.Name = "Times New Roman";
                        headerRange0.Font.Size = 12;
                        headerRange0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange1 = worksheet.Range["A2", "C2"];
                        headerRange1.Merge();
                        headerRange1.Value = guna2HtmlLabel7.Text;
                        headerRange1.Font.Bold = true;
                        headerRange1.Font.Name = "Times New Roman";
                        headerRange1.Font.Size = 11;
                        headerRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange2 = worksheet.Range["A3", "C3"];
                        headerRange2.Merge();
                        headerRange2.Value = "From " + dtFrom.Text + " - To " + dtTo.Text;
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
                        // 1️⃣ Write header row (aligned with data columns)
                        var accountHeaderCell = worksheet.Cells[rowIndex, 2];
                        var amount1HeaderCell = worksheet.Cells[rowIndex, 3];
                        var amount2HeaderCell = worksheet.Cells[rowIndex, 4];

                        accountHeaderCell.Value = "Account";
                        amount1HeaderCell.Value = dgvCustomer.Columns["balance1"].HeaderCell.Value;
                        amount2HeaderCell.Value = dgvCustomer.Columns["balance2"].HeaderCell.Value;

                        foreach (var headerCell in new[] { accountHeaderCell, amount1HeaderCell, amount2HeaderCell })
                        {
                            headerCell.Font.Bold = true;
                            headerCell.Font.Name = "Times New Roman";
                            headerCell.Font.Size = 11;
                            headerCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        }

                        // 2️⃣ Write data rows
                        rowIndex++; // move to next row for data
                        foreach (DataGridViewRow row in dgvCustomer.Rows)
                        {
                            if (row.IsNewRow) continue;


                            string account = row.Cells["lvl1"].Value?.ToString();
                            string amount1 = row.Cells["balance1"].Value?.ToString();
                            string amount2 = row.Cells["balance2"].Value?.ToString();

                            var accountCell = worksheet.Cells[rowIndex, 2];
                            var amount1Cell = worksheet.Cells[rowIndex, 3];
                            var amount2Cell = worksheet.Cells[rowIndex, 4];

                            accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            amount1Cell.Value = amount1.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();
                            amount2Cell.Value = amount2.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();

                            accountCell.Font.Name = "Times New Roman";
                            amount1Cell.Font.Name = "Times New Roman";
                            amount2Cell.Font.Name = "Times New Roman";

                            if (account.ToUpper().Contains("TOTAL"))
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amount1Cell.Font.Size = 10;
                                amount2Cell.Font.Size = 10;
                                amount1Cell.Font.Bold = true;
                                amount2Cell.Font.Bold = true;
                                amount1Cell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                                amount2Cell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                            }
                            else
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amount1Cell.Font.Size = 9;
                                amount2Cell.Font.Size = 9;
                            }

                            amount1Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            amount2Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                            rowIndex++;
                        }

                        worksheet.Columns[2].AutoFit();
                        worksheet.Columns[3].AutoFit();
                        worksheet.Columns[4].AutoFit();
                    }
                    else if (reportType == "Prev Year Comparison")
                    {
                        Excel.Range headerRange0 = worksheet.Range["A1", "C1"];
                        headerRange0.Merge();
                        headerRange0.Value = lblCompany.Text;
                        headerRange0.Font.Bold = true;
                        headerRange0.Font.Name = "Times New Roman";
                        headerRange0.Font.Size = 12;
                        headerRange0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange1 = worksheet.Range["A2", "C2"];
                        headerRange1.Merge();
                        headerRange1.Value = guna2HtmlLabel7.Text;
                        headerRange1.Font.Bold = true;
                        headerRange1.Font.Name = "Times New Roman";
                        headerRange1.Font.Size = 11;
                        headerRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange2 = worksheet.Range["A3", "C3"];
                        headerRange2.Merge();
                        headerRange2.Value = "From " + dtFrom.Text + " - To " + dtTo.Text;
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
                        // 1️⃣ Write header row (aligned with data columns)
                        var accountHeaderCell = worksheet.Cells[rowIndex, 2];
                        var amount1HeaderCell = worksheet.Cells[rowIndex, 3];
                        var amount2HeaderCell = worksheet.Cells[rowIndex, 4];
                        var changeHeaderCell = worksheet.Cells[rowIndex, 5];
                        var changePercentageHeaderCell = worksheet.Cells[rowIndex, 6];

                        accountHeaderCell.Value = "Account";
                        amount1HeaderCell.Value = dgvCustomer.Columns["balance1"].HeaderCell.Value;
                        amount2HeaderCell.Value = dgvCustomer.Columns["balance2"].HeaderCell.Value;
                        changeHeaderCell.Value = dgvCustomer.Columns["change"].HeaderCell.Value;
                        changePercentageHeaderCell.Value = dgvCustomer.Columns["change_percentage"].HeaderCell.Value;

                        foreach (var headerCell in new[] { accountHeaderCell, amount1HeaderCell, amount2HeaderCell, changeHeaderCell, changePercentageHeaderCell })
                        {
                            headerCell.Font.Bold = true;
                            headerCell.Font.Name = "Times New Roman";
                            headerCell.Font.Size = 11;
                            headerCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        }

                        // 2️⃣ Write data rows
                        rowIndex++; // move to next row for data
                        foreach (DataGridViewRow row in dgvCustomer.Rows)
                        {
                            if (row.IsNewRow) continue;


                            string account = row.Cells["lvl1"].Value?.ToString();
                            string amount1 = row.Cells["balance1"].Value?.ToString();
                            string amount2 = row.Cells["balance2"].Value?.ToString();
                            string change = row.Cells["change"].Value?.ToString();
                            string change_percentage = row.Cells["change_percentage"].Value?.ToString();

                            var accountCell = worksheet.Cells[rowIndex, 2];
                            var amount1Cell = worksheet.Cells[rowIndex, 3];
                            var amount2Cell = worksheet.Cells[rowIndex, 4];
                            var changeCell = worksheet.Cells[rowIndex, 5];
                            var changePercentageCell = worksheet.Cells[rowIndex, 6];

                            accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            amount1Cell.Value = amount1.Trim();
                            amount2Cell.Value = amount2.Trim();
                            changeCell.Value = change.Trim();
                            changePercentageCell.Value = change_percentage.Trim();

                            accountCell.Font.Name = "Times New Roman";
                            amount1Cell.Font.Name = "Times New Roman";
                            amount2Cell.Font.Name = "Times New Roman";
                            changeCell.Font.Name = "Times New Roman";
                            changePercentageCell.Font.Name = "Times New Roman";

                            if (account.ToUpper().Contains("TOTAL"))
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amount1Cell.Font.Size = amount2Cell.Font.Size = changeCell.Font.Size = changePercentageCell.Font.Size = 10;
                                amount1Cell.Font.Bold = amount2Cell.Font.Bold = changeCell.Font.Bold = changePercentageCell.Font.Bold = true;
                                amount1Cell.Font.Underline = amount2Cell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                                changeCell.Font.Underline = changePercentageCell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                            }
                            else
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;
                                amount1Cell.Font.Size = amount2Cell.Font.Size = 9;
                                changeCell.Font.Size = changePercentageCell.Font.Size = 9;
                            }

                            amount1Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            amount2Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            changeCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            changePercentageCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                            rowIndex++;
                        }

                        worksheet.Columns[2].AutoFit();
                        worksheet.Columns[3].AutoFit();
                        worksheet.Columns[4].AutoFit();
                        worksheet.Columns[5].AutoFit();
                        worksheet.Columns[6].AutoFit();
                    }
                    else if(reportType == "by Job")
                    {
                        Excel.Range headerRange0 = worksheet.Range["A1", "C1"];
                        headerRange0.Merge();
                        headerRange0.Value = lblCompany.Text;
                        headerRange0.Font.Bold = true;
                        headerRange0.Font.Name = "Times New Roman";
                        headerRange0.Font.Size = 12;
                        headerRange0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange1 = worksheet.Range["A2", "C2"];
                        headerRange1.Merge();
                        headerRange1.Value = guna2HtmlLabel7.Text;
                        headerRange1.Font.Bold = true;
                        headerRange1.Font.Name = "Times New Roman";
                        headerRange1.Font.Size = 11;
                        headerRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange2 = worksheet.Range["A3", "C3"];
                        headerRange2.Merge();
                        headerRange2.Value = "From " + dtFrom.Text + " - To " + dtTo.Text;
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
                        // header names from dgvCustomer
                        foreach(DataGridViewColumn col in dgvCustomer.Columns)
                        {
                            if(col.Visible)
                            {
                                var headerCell = worksheet.Cells[rowIndex, col.Index + 1];
                                headerCell.Value = col.HeaderText;
                                headerCell.Font.Bold = true;
                                headerCell.Font.Name = "Times New Roman";
                                headerCell.Font.Size = 11;
                                headerCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            }
                        }
                        //// 1️⃣ Write header row (aligned with data columns)
                        //var accountHeaderCell = worksheet.Cells[rowIndex, 2];
                        //var amount1HeaderCell = worksheet.Cells[rowIndex, 3];
                        //var amount2HeaderCell = worksheet.Cells[rowIndex, 4];
                        //var changeHeaderCell = worksheet.Cells[rowIndex, 5];
                        //var changePercentageHeaderCell = worksheet.Cells[rowIndex, 6];

                        //accountHeaderCell.Value = "Account";
                        //amount1HeaderCell.Value = dgvCustomer.Columns["balance1"].HeaderCell.Value;
                        //amount2HeaderCell.Value = dgvCustomer.Columns["balance2"].HeaderCell.Value;
                        //changeHeaderCell.Value = dgvCustomer.Columns["change"].HeaderCell.Value;
                        //changePercentageHeaderCell.Value = dgvCustomer.Columns["change_percentage"].HeaderCell.Value;

                        //foreach (var headerCell in new[] { accountHeaderCell, amount1HeaderCell, amount2HeaderCell, changeHeaderCell, changePercentageHeaderCell })
                        //{
                        //    headerCell.Font.Bold = true;
                        //    headerCell.Font.Name = "Times New Roman";
                        //    headerCell.Font.Size = 11;
                        //    headerCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        //    headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        //}

                        // 2️⃣ Write data rows
                        rowIndex++; // move to next row for data
                        //foreach (DataGridViewRow row in dgvCustomer.Rows)
                        //{
                        //    if (row.IsNewRow) continue;


                        //    string account = row.Cells["lvl1"].Value?.ToString();
                        //    string amount1 = row.Cells["balance1"].Value?.ToString();
                        //    string amount2 = row.Cells["balance2"].Value?.ToString();
                        //    string change = row.Cells["change"].Value?.ToString();
                        //    string change_percentage = row.Cells["change_percentage"].Value?.ToString();

                        //    var accountCell = worksheet.Cells[rowIndex, 2];
                        //    var amount1Cell = worksheet.Cells[rowIndex, 3];
                        //    var amount2Cell = worksheet.Cells[rowIndex, 4];
                        //    var changeCell = worksheet.Cells[rowIndex, 5];
                        //    var changePercentageCell = worksheet.Cells[rowIndex, 6];

                        //    accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                        //    amount1Cell.Value = amount1.Trim();
                        //    amount2Cell.Value = amount2.Trim();
                        //    changeCell.Value = change.Trim();
                        //    changePercentageCell.Value = change_percentage.Trim();

                        //    accountCell.Font.Name = "Times New Roman";
                        //    amount1Cell.Font.Name = "Times New Roman";
                        //    amount2Cell.Font.Name = "Times New Roman";
                        //    changeCell.Font.Name = "Times New Roman";
                        //    changePercentageCell.Font.Name = "Times New Roman";

                        //    if (account.ToUpper().Contains("TOTAL"))
                        //    {
                        //        accountCell.Font.Size = 10;
                        //        accountCell.Font.Bold = true;

                        //        amount1Cell.Font.Size = amount2Cell.Font.Size = changeCell.Font.Size = changePercentageCell.Font.Size = 10;
                        //        amount1Cell.Font.Bold = amount2Cell.Font.Bold = changeCell.Font.Bold = changePercentageCell.Font.Bold = true;
                        //        amount1Cell.Font.Underline = amount2Cell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                        //        changeCell.Font.Underline = changePercentageCell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                        //    }
                        //    else
                        //    {
                        //        accountCell.Font.Size = 10;
                        //        accountCell.Font.Bold = true;
                        //        amount1Cell.Font.Size = amount2Cell.Font.Size = 9;
                        //        changeCell.Font.Size = changePercentageCell.Font.Size = 9;
                        //    }

                        //    amount1Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        //    amount2Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        //    changeCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        //    changePercentageCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        //    rowIndex++;
                        //}

                        //worksheet.Columns[2].AutoFit();
                        //worksheet.Columns[3].AutoFit();
                        //worksheet.Columns[4].AutoFit();
                        //worksheet.Columns[5].AutoFit();
                        //worksheet.Columns[6].AutoFit();
                        foreach (DataGridViewRow row in dgvCustomer.Rows)
                        {
                            if (row.IsNewRow) continue;
                            foreach (DataGridViewColumn col in dgvCustomer.Columns)
                            {
                                if (col.Visible)
                                {
                                    var dataCell = worksheet.Cells[rowIndex, col.Index + 1];
                                    DataGridViewCell dgvCell = row.Cells[col.Index];
                                    dataCell.Value = dgvCell.Value?.ToString() ?? "";
                                    dataCell.Font.Name = "Times New Roman";
                                    // Align numeric columns to right
                                    string colName = col.Name.ToLower();
                                    if (colName.Contains("balance") || colName.Contains("debit") || colName.Contains("credit") || colName.Contains("amount") || colName.Contains("total"))
                                    {
                                        dataCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        dataCell.Font.Size = 9;
                                    }
                                    else
                                    {
                                        dataCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                        dataCell.Font.Size = 10;
                                    }
                                }
                            }
                            rowIndex++;
                        }
                    }
                    else if (reportType == "by Class")
                    {
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
                        headerRange2.Value = "From " + dtFrom.Text + " - To " + dtTo.Text;
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
                        headerRange3.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        int rowIndex = 5;
                        // 1️⃣ Write header row (aligned with data columns)
                        var accountHeaderCell = worksheet.Cells[rowIndex, 2];
                        var amountHeaderCell = worksheet.Cells[rowIndex, 3];
                        var amount1HeaderCell = worksheet.Cells[rowIndex, 4];

                        accountHeaderCell.Value = "Account";
                        amountHeaderCell.Value = "Unclassified";
                        amount1HeaderCell.Value = "Total";

                        foreach (var headerCell in new[] { accountHeaderCell, amountHeaderCell, amount1HeaderCell })
                        {
                            headerCell.Font.Bold = true;
                            headerCell.Font.Name = "Times New Roman";
                            headerCell.Font.Size = 11;
                            headerCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        }

                        // 2️⃣ Write data rows
                        rowIndex++; // move to next row for data
                        foreach (DataGridViewRow row in dgvCustomer.Rows)
                        {
                            if (row.IsNewRow) continue;


                            string account = row.Cells["lvl1"].Value?.ToString();
                            string amount = row.Cells["balance"].Value?.ToString();
                            string amount1 = row.Cells["balance1"].Value?.ToString();

                            var accountCell = worksheet.Cells[rowIndex, 2];
                            var amountCell = worksheet.Cells[rowIndex, 3];
                            var amount1Cell = worksheet.Cells[rowIndex, 4];

                            accountCell.Value = account.Replace("▶", "").Trim().Replace("▼", "").Trim();
                            amountCell.Value = amount.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();
                            amount1Cell.Value = amount1.Replace("◀", "").Trim().Trim().Replace("▼", "").Trim();

                            accountCell.Font.Name = "Times New Roman";
                            amountCell.Font.Name = "Times New Roman";
                            amount1Cell.Font.Name = "Times New Roman";

                            if (account.ToUpper().Contains("TOTAL"))
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amountCell.Font.Size = 10;
                                amountCell.Font.Bold = true;
                                amountCell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                                amount1Cell.Font.Size = 10;
                                amount1Cell.Font.Bold = true;
                                amount1Cell.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleDouble;
                            }
                            else
                            {
                                accountCell.Font.Size = 10;
                                accountCell.Font.Bold = true;

                                amountCell.Font.Size = 9;
                                amount1Cell.Font.Size = 9;
                            }

                            amountCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            amount1Cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;


                            rowIndex++;
                        }

                        worksheet.Columns[2].AutoFit();
                        worksheet.Columns[3].AutoFit();
                    }
                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //open same excel file
                    Process.Start("explorer.exe", saveDialog.FileName);
                }
            }
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

            // "ProfitAndLoss Summary" - Bold, size 10
            string title = guna2HtmlLabel7.Text.Replace(" ", "");
            FormattedText summaryText = center.AddFormattedText(title+"\n", TextFormat.Bold);
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

            if (reportType == "standard" || reportType == "Unclassified")
            {
                // Table for data
                Table dataTable = section.AddTable();
                dataTable.Borders.Width = 0;
                dataTable.AddColumn("10cm");
                dataTable.AddColumn("5cm");

                decimal totalAmount = 0;

                // Load from DataGridView
                foreach (DataGridViewRow row in dgvCustomer.Rows)
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
            }
            else if(reportType == "YTD Comparison")
            {
                //dynamic columns so i want to create table based on dgv columns
                Table dataTable = section.AddTable();
                dataTable.Borders.Width = 0;
                // Add columns based on DataGridView
                // Add columns starting from index 3
                for (int i = 3; i < dgvCustomer.Columns.Count; i++)
                {
                    DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                    if (dgvCol.Visible) // only add visible columns
                    {
                        if(dgvCol.HeaderText.Contains("Account Name"))
                            dataTable.AddColumn("10cm"); // Adjust width as needed
                        else
                            dataTable.AddColumn("3cm"); // Adjust width as needed
                    }
                }

                // Add header row
                Row header = dataTable.AddRow();
                int colIndex = 0;
                for (int i = 3; i < dgvCustomer.Columns.Count; i++)
                {
                    DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                    if (!dgvCol.Visible) continue;

                    Paragraph p = header.Cells[colIndex].AddParagraph(dgvCol.HeaderText);
                    p.Format.Font.Bold = true;
                    p.Format.Font.Name = "Times New Roman";
                    p.Format.Alignment = ParagraphAlignment.Center;
                    colIndex++;
                }

                // Add data rows
                foreach (DataGridViewRow dgvRow in dgvCustomer.Rows)
                {
                    if (dgvRow.IsNewRow) continue;

                    Row row = dataTable.AddRow();
                    int dataColIndex = 0;

                    for (int i = 3; i < dgvCustomer.Columns.Count; i++)
                    {
                        DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                        if (!dgvCol.Visible) continue;

                        DataGridViewCell dgvCell = dgvRow.Cells[i];
                        Paragraph p = row.Cells[dataColIndex].AddParagraph(dgvCell.Value?.ToString() ?? "");
                        p.Format.Font.Name = "Times New Roman";

                        // Align numeric columns to right
                        string colName = dgvCol.Name.ToLower();
                        if (colName.Contains("balance1") || colName.Contains("balance2"))
                        {
                            p.Format.Alignment = ParagraphAlignment.Right;
                            p.Format.Font.Size = 9;
                        }
                        else
                        {
                            p.Format.Alignment = ParagraphAlignment.Left;
                            p.Format.Font.Size = 10;
                        }

                        dataColIndex++;
                    }
                }
            }
            else if (reportType == "Prev Year Comparison")
            {
                // dynamic columns so i want to create table based on dgv columns
                Table dataTable = section.AddTable();
                dataTable.Borders.Width = 0;
                // Add columns based on DataGridView
                // Add columns starting from index 3
                for (int i = 3; i < dgvCustomer.Columns.Count; i++)
                {
                    DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                    if (dgvCol.Visible) // only add visible columns
                    {
                        if (dgvCol.HeaderText.Contains("Account Name"))
                            dataTable.AddColumn("7cm"); // Adjust width as needed
                        else
                            dataTable.AddColumn("3cm"); // Adjust width as needed
                    }
                }

                // Add header row
                Row header = dataTable.AddRow();
                int colIndex = 0;
                for (int i = 3; i < dgvCustomer.Columns.Count; i++)
                {
                    DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                    if (!dgvCol.Visible) continue;

                    Paragraph p = header.Cells[colIndex].AddParagraph(dgvCol.HeaderText);
                    p.Format.Font.Bold = true;
                    p.Format.Font.Name = "Times New Roman";
                    p.Format.Alignment = ParagraphAlignment.Center;
                    colIndex++;
                }

                // Add data rows
                foreach (DataGridViewRow dgvRow in dgvCustomer.Rows)
                {
                    if (dgvRow.IsNewRow) continue;

                    Row row = dataTable.AddRow();
                    int dataColIndex = 0;

                    for (int i = 3; i < dgvCustomer.Columns.Count; i++)
                    {
                        DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                        if (!dgvCol.Visible) continue;

                        DataGridViewCell dgvCell = dgvRow.Cells[i];
                        Paragraph p = row.Cells[dataColIndex].AddParagraph(dgvCell.Value?.ToString() ?? "");
                        p.Format.Font.Name = "Times New Roman";

                        // Align numeric columns to right
                        string colName = dgvCol.Name.ToLower();
                        if (colName.Contains("balance1") || colName.Contains("balance2") || colName.Contains("change") || colName.Contains("change_percentage"))
                        {
                            p.Format.Alignment = ParagraphAlignment.Right;
                            p.Format.Font.Size = 9;
                        }
                        else
                        {
                            p.Format.Alignment = ParagraphAlignment.Left;
                            p.Format.Font.Size = 10;
                        }

                        dataColIndex++;
                    }
                }
            }
            else if(reportType == "by Job")
            {
                // dynamic columns so i want to create table based on dgv columns
                Table dataTable = section.AddTable();
                dataTable.Borders.Width = 0;
                // Add columns based on DataGridView
                // Add columns starting from index 0
                for (int i = 0; i < dgvCustomer.Columns.Count; i++)
                {
                    DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                    if (dgvCol.Visible) // only add visible columns
                    {
                        if (dgvCol.HeaderText.Contains("Account Name") || dgvCol.HeaderText.Contains("Job"))
                            dataTable.AddColumn("7cm"); // Adjust width as needed
                        else
                            dataTable.AddColumn("3cm"); // Adjust width as needed
                    }
                }
                // Add header row
                Row header = dataTable.AddRow();
                int colIndex = 0;
                for (int i = 0; i < dgvCustomer.Columns.Count; i++)
                {
                    DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                    if (!dgvCol.Visible) continue;
                    Paragraph p = header.Cells[colIndex].AddParagraph(dgvCol.HeaderText);
                    p.Format.Font.Bold = true;
                    p.Format.Font.Name = "Times New Roman";
                    p.Format.Alignment = ParagraphAlignment.Center;
                    colIndex++;
                }
                // Add data rows
                foreach (DataGridViewRow dgvRow in dgvCustomer.Rows)
                {
                    if (dgvRow.IsNewRow) continue;
                    Row row = dataTable.AddRow();
                    int dataColIndex = 0;
                    for (int i = 0; i < dgvCustomer.Columns.Count; i++)
                    {
                        DataGridViewColumn dgvCol = dgvCustomer.Columns[i];
                        if (!dgvCol.Visible) continue;
                        DataGridViewCell dgvCell = dgvRow.Cells[i];
                        Paragraph p = row.Cells[dataColIndex].AddParagraph(dgvCell.Value?.ToString() ?? "");
                        p.Format.Font.Name = "Times New Roman";
                        // Align numeric columns to right
                        string colName = dgvCol.Name.ToLower();
                        if (colName.Contains("balance") || colName.Contains("debit") || colName.Contains("credit") || colName.Contains("amount") || colName.Contains("total"))
                        {
                            p.Format.Alignment = ParagraphAlignment.Right;
                            p.Format.Font.Size = 9;
                        }
                        else
                        {
                            p.Format.Alignment = ParagraphAlignment.Left;
                            p.Format.Font.Size = 10;
                        }
                        dataColIndex++;
                    }
                }
            }
            else if (reportType == "by Class")
            {
                // Table for data
                Table dataTable = section.AddTable();
                dataTable.Borders.Width = 0;
                dataTable.AddColumn("10cm");
                dataTable.AddColumn("5cm");

                decimal totalAmount = 0;
                decimal totalAmount1 = 0;

                // Load from DataGridView
                foreach (DataGridViewRow row in dgvCustomer.Rows)
                {
                    if (row.IsNewRow) continue;

                    string account = row.Cells["lvl1"].Value?.ToString() ?? "";
                    if (account.ToUpper().Contains("TOTAL")) continue;

                    string amountStr = row.Cells["balance"].Value?.ToString() ?? "0";
                    string amount1Str = row.Cells["balance1"].Value?.ToString() ?? "0";
                    account = account.Replace("▶", "").Trim();
                    amountStr = amountStr.Replace("◀", "").Trim();
                    amount1Str = amount1Str.Replace("◀", "").Trim();

                    decimal amount = 0;
                    decimal amount1 = 0;
                    decimal.TryParse(amountStr, out amount);
                    decimal.TryParse(amount1Str, out amount1);

                    Row tRow = dataTable.AddRow();
                    tRow.Cells[0].AddParagraph(account);
                    tRow.Cells[1].AddParagraph(amount.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;
                    tRow.Cells[1].AddParagraph(amount1.ToString("N2")).Format.Alignment = ParagraphAlignment.Right;

                    totalAmount += amount;
                    totalAmount1 += amount1;
                }

                // TOTAL row
                Row totalRow = dataTable.AddRow();
                totalRow.Cells[0].AddParagraph("TOTAL").Format.Font.Bold = true;
                Paragraph totalText = totalRow.Cells[1].AddParagraph(totalAmount.ToString("N2"));
                totalText.Format.Font.Bold = true;
                totalText.Format.Font.Underline = Underline.Single;
                totalRow.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                Paragraph totalText1 = totalRow.Cells[2].AddParagraph(totalAmount1.ToString("N2"));
                totalText1.Format.Font.Bold = true;
                totalText1.Format.Font.Underline = Underline.Single;
                totalRow.Cells[2].Format.Alignment = ParagraphAlignment.Right;
            }

            //section.Add(dataTable);

            // Render and save
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), title+".pdf");
            renderer.PdfDocument.Save(filePath);
            Process.Start("explorer.exe", filePath);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dtFrom, dtTo);
                dateChangeTo();
                BindDGV_Data();
                DateWords();
            }
        }
        private bool IsArabicReport()
        {
            return TranslationClass.IsArabic;
        }

        private string ReportTranslate(string english)
        {
            return TranslationClass.Translate(
                english ?? "",
                IsArabicReport());
        }

        private readonly Dictionary<Control, string> _originalReportTexts =
            new Dictionary<Control, string>();

        private void TranslateReportControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is HeaderUC)
                    continue;

                bool translatable =
                    control is ButtonBase ||
                    control is Label ||
                    control is GroupBox ||
                    control.GetType().Name == "Guna2Button" ||
                    control.GetType().Name == "Guna2HtmlLabel" ||
                    control.GetType().Name == "Guna2CheckBox" ||
                    control.GetType().Name == "Guna2RadioButton";

                if (translatable)
                {
                    if (!_originalReportTexts.ContainsKey(control))
                        _originalReportTexts[control] = control.Text;

                    string english = _originalReportTexts[control];

                    control.Text = ReportTranslate(english.Trim());

                    control.RightToLeft = IsArabicReport()
                        ? RightToLeft.Yes
                        : RightToLeft.No;
                }

                if (control.HasChildren)
                    TranslateReportControls(control);
            }
        }
        private void TranslateExportMenu(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                string english = item.Text;

                if (IsArabicReport())
                {
                    item.Text = ReportTranslate(english);
                }

                if (item is ToolStripDropDownItem dropDown)
                    TranslateExportMenu(dropDown.DropDownItems);
            }
        }

        private string TranslateReportRow(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !IsArabicReport())
                return value;

            string text = value.Trim();

            string arrow = "";

            if (text.StartsWith("►") ||
                text.StartsWith("▼") ||
                text.StartsWith("▲"))
            {
                arrow = text.Substring(0, 1) + " ";
                text = text.Substring(1).Trim();
            }

            string translated = ReportTranslate(text);

            if (translated == text)
            {
                int separator = text.LastIndexOf(" - ");

                if (separator > 0)
                {
                    string first = text.Substring(0, separator).Trim();
                    string second = text.Substring(separator + 3).Trim();

                    translated =
                        ReportTranslate(first) + " - " +
                        ReportTranslate(second);
                }
            }

            return arrow + translated;
        }
        private void TranslateReportGrid()
        {
            foreach (DataGridViewColumn column in dgvCustomer.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                {
                    column.HeaderText =
                        ReportTranslate(column.HeaderText.Trim());
                }

                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (!IsArabicReport())
                return;

            foreach (DataGridViewRow row in dgvCustomer.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (dgvCustomer.Columns.Contains("lvl1"))
                {
                    DataGridViewCell cell = row.Cells["lvl1"];

                    if (cell.Value != null)
                    {
                        cell.Value = TranslateReportRow(
                            cell.Value.ToString());
                    }

                    cell.Style.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        private void ApplyProfitLossLayout()
        {
            bool arabic = IsArabicReport();

            SuspendLayout();

            try
            {
                // Header
                headerUC1.ArabicLayout = arabic;

                string title = ReportTranslate("Profit and Loss");

                headerUC1.FormText = title;

                // Report heading
                guna2HtmlLabel7.Text =
                    ReportTranslate("Profit & Loss");

                guna2HtmlLabel7.TextAlignment =
      ContentAlignment.MiddleCenter;

                lblCompany.TextAlignment =
                    ContentAlignment.MiddleCenter;

                guna2HtmlLabel10.TextAlignment =
                    ContentAlignment.MiddleCenter;

                // Print button
                btnPrint.Text =
                    ReportTranslate("Print") + " ▼";

                // Translate existing controls
                TranslateReportControls(this);

                // Export context menu
                TranslateExportMenu(contextMenuExport.Items);

                // Report grid
                TranslateReportGrid();

                dgvCustomer.RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                dgvCustomer.RowHeadersVisible = false;
            }
            finally
            {
                ResumeLayout(true);
            }
        }
        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            //dateChangeTo();
            //BindDGV_Data();
            //DateWords();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            //dateChangeTo();
            //BindDGV_Data();
            //DateWords();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            dateChangeTo();
            BindDGV_Data();
            DateWords();

            ApplyProfitLossLayout();
            FixProfitLossGridPosition();
        }
        private void DateWords()
        {
            if (IsArabicReport())
            {
                var culture = new System.Globalization.CultureInfo("ar-AE");

                string fromDate =
                    dtFrom.Value.ToString("dd MMMM yyyy", culture);

                string toDate =
                    dtTo.Value.ToString("dd MMMM yyyy", culture);

                guna2HtmlLabel10.Text =
                    $"من {fromDate} إلى {toDate}";
            }
            else
            {
                string fromDate =
                    dtFrom.Value.ToString("MMMM dd, yyyy");

                string toDate =
                    dtTo.Value.ToString("MMMM dd, yyyy");

                guna2HtmlLabel10.Text =
                    $"{fromDate} through {toDate}";
            }
        }
    }
}