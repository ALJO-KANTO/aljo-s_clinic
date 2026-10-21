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
using Excel = Microsoft.Office.Interop.Excel;

namespace YamyProject
{
    public partial class ProfitandLossDetailReport : Form
    {
        string reportType = "";
        string startDate = "";
        string endDate = "";
        public ProfitandLossDetailReport(string _type, string _startDate, string _endDate)
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            this.reportType = _type;
            headerUC1.FormText = this.Text;
            this.startDate = _startDate;
            this.endDate = _endDate;
        }

        DateTime prevFrom;
        DateTime prevTo;
        void Dates(DateTime d)
        {
            DateTime sDateOfY = new DateTime(d.Year, 1, 1);
            DateTime eDateOfY = new DateTime(d.Year, 12, 31);

            dtFrom.Value = sDateOfY;
            dtTo.Value = eDateOfY;
        }
        private void dateChangeTo() {
            prevFrom = new DateTime(dtFrom.Value.Year, 1, 1);
            prevTo = new DateTime(dtTo.Value.Year, dtTo.Value.Month, DateTime.DaysInMonth(dtTo.Value.Year, dtTo.Value.Month));
        }

        private void ProfitandLossDetailReport_Load(object sender, EventArgs e)
        {
            Dates(DateTime.Now);
            using (var reader = ClinicDatabase.ExecuteReader("SELECT name FROM tbl_company"))
            {
                if (reader.Read() && reader["name"] != DBNull.Value)
                {
                    lblCompany.Text = reader["name"].ToString();
                }
            }

            dgvCustomer.Font = new System.Drawing.Font("Segoe UI", 7, FontStyle.Bold);
            dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;

            btnPrint.Text = "Print ▼";

            if(startDate != "")
            {
                DateTime sDate;
                if(DateTime.TryParse(startDate, out sDate))
                {
                    dtFrom.Value = sDate;
                }
            }
            if (endDate != "")
            {
                DateTime eDate;
                if (DateTime.TryParse(endDate, out eDate))
                {
                    dtTo.Value = eDate;
                }
            }
            //if (reportType != "")
            //{
            //    headerUC1.FormText = this.Text + reportType;
            //    guna2HtmlLabel7.Text = this.Text + reportType;
            //}
            AddDGVColumns();
            BindDGV();
        }

        private void BindDGV()
        {
            dateChangeTo();
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
                        LEFT JOIN tbl_transaction t ON t.account_id = l4.id
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

                        "))
            {
                dgvCustomer.Rows.Add("e", "u", "1", "Ordinary Income/Expense", "0", "", "", "", "", "", "", "", "", "", "", "", "");
                while (reader.Read())
                {
                    dgvCustomer.Rows.Add(
                        "e", 
                        "u", 
                        "1", 
                        "   ►     " + reader["name"].ToString(),
                        reader["id"].ToString(), 
                        reader["name"].ToString(),
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        Utilities.FormatDecimal(reader["balance"].ToString()));

                }
                // Auto-expand everything
                for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                {
                    ExpandRowRecursively(i);
                }

                // Calculate totals from already-expanded rows
                decimal totalIncome = 0;
                decimal totalCOGS = 0;
                decimal totalExpense = 0;

                foreach (DataGridViewRow row in dgvCustomer.Rows)
                {
                    string name = row.Cells["currname"].Value?.ToString()?.ToUpper() ?? "";
                    decimal amount = 0;
                    decimal.TryParse(row.Cells["tranBalance"].Value?.ToString(), out amount);
                    int currLvl = int.Parse(row.Cells["currLvl"].Value.ToString());

                }
                using (MySqlDataReader mySqlDataReader = ClinicDatabase.ExecuteReader(@"
                            SELECT
                                t1.category_code,
                                IFNULL(COALESCE(SUM(tt.debit) - SUM(tt.credit), 0),0) AS total_balance
                            FROM tbl_coa_level_1 t1
                            JOIN tbl_coa_level_2 t2 ON t2.main_id = t1.id
                            JOIN tbl_coa_level_3 t3 ON t3.main_id = t2.id
                            JOIN tbl_coa_level_4 t4 ON t4.main_id = t3.id
                            LEFT JOIN tbl_transaction tt ON tt.account_id = t4.id
                            WHERE t1.category_code IN ('Income', 'Cost', 'EXPENSE')
                            GROUP BY t1.category_code;
                            ")
                    )
                {
                    while (mySqlDataReader.Read())
                    {
                        string categoryCode = mySqlDataReader["category_code"].ToString();
                        decimal totalBalance = Convert.ToDecimal(mySqlDataReader["total_balance"].ToString());
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
                    "0", "Net Profit", "", "", "", "", "", "", "", "", "", "",
                    Utilities.FormatDecimal(netProfit)
                );
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

        private void AddDGVColumns()
        {
            
                // Hidden helper columns (keep compatibility with other code paths)
                dgvCustomer.Columns.Add("state", "");
                dgvCustomer.Columns.Add("loadState", "");
                dgvCustomer.Columns.Add("currLvl", "");
                dgvCustomer.Columns.Add("lvl1", "");
                dgvCustomer.Columns.Add("id", "");
                dgvCustomer.Columns.Add("currname", "");

                // Transaction specific columns (visible)
                dgvCustomer.Columns.Add("tranType", "Type");
                dgvCustomer.Columns.Add("tranDate", "Date");
                dgvCustomer.Columns.Add("tranNum", "Num");
                dgvCustomer.Columns.Add("tranAdj", "Adj");
                dgvCustomer.Columns.Add("tranName", "Name");
                dgvCustomer.Columns.Add("tranMemo", "Memo");
                dgvCustomer.Columns.Add("tranClr", "Clr");
                dgvCustomer.Columns.Add("tranSplit", "Split");
                dgvCustomer.Columns.Add("tranDebit", "Debit");
                dgvCustomer.Columns.Add("tranCredit", "Credit");
                dgvCustomer.Columns.Add("tranBalance", "Balance");

                LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);

                // Visuals
                dgvCustomer.Columns["lvl1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCustomer.Columns["state"].Visible = dgvCustomer.Columns["currname"].Visible = dgvCustomer.Columns["id"].Visible = dgvCustomer.Columns["loadState"].Visible = dgvCustomer.Columns["currLvl"].Visible = false;

                dgvCustomer.Columns["tranDebit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["tranDebit"].DefaultCellStyle.Format = "N2";
                dgvCustomer.Columns["tranCredit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["tranCredit"].DefaultCellStyle.Format = "N2";
                dgvCustomer.Columns["tranBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["tranBalance"].DefaultCellStyle.Format = "N2";

                dgvCustomer.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //dgvCustomer.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
                //dgvCustomer.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                dgvCustomer.RowsDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                dgvCustomer.RowsDefaultCellStyle.BackColor = System.Drawing.Color.White;
            
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
            int parentId = int.Parse(row.Cells["id"].Value.ToString());

            try
            {
                if (state == "e")
                {
                    if (loadState == "u")
                    {
                        

                        List<DataGridViewRow> newRows = new List<DataGridViewRow>();

                        using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                            $@"SELECT name as currname, concat(code,' - ',name) as name , id 
                                    FROM tbl_coa_level_" + (currLvl + 1) + " WHERE main_id = @parentId ORDER BY code",
                            ClinicDatabase.CreateParameterNew("parentId", parentId)))
                        {
                            int insertIndex = rowIndex + 1;

                            if (int.Parse(row.Cells["id"].Value.ToString()) > 0)
                                dgvCustomer.Rows[rowIndex].Cells["tranBalance"].Value = "";

                            
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

                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });

                                var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString());
                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = Utilities.FormatDecimal(balanceData) });
                                
                                newRow.Height = dgvCustomer.Rows[rowIndex].Height;

                                ApplyRowStyle(newRow, currLvl + 1);
                                newRows.Add(newRow);

                                // If this is level 4 → load transactions instead of next COA level
                                if ((currLvl + 1) == 4)
                                {
                                    //object objId = ClinicDatabase.ExecuteScalar($@"SELECT id FROM tbl_coa_level_" + (currLvl + 1) + " WHERE main_id = @parentId ORDER BY code", ClinicDatabase.CreateParameterNew("parentId", parentId));
                                    //int refLevel4Id = objId == null ? 0 : (int)objId;
                                    if (int.Parse(reader["id"].ToString()) > 0)
                                    {
                                        //LoadTransactionsUnderLevel4(newRows, rowIndex + 1, int.Parse(row.Cells["id"].Value.ToString()));
                                        //return;

                                        insertIndex = rowIndex + 1;

                                        using (MySqlDataReader tReader = ClinicDatabase.ExecuteReader(@"
                                                                    SELECT 
                                                                        t.id,
                                                                        t.type,
                                                                        t.date,
                                                                        t.voucher_no,
                                                                        COALESCE(t.debit,0) AS debit,
                                                                        COALESCE(t.credit,0) AS credit,
                                                                    CASE WHEN t.type IN (
	                                                                    'Customer Receipt',
	                                                                    'Sales Invoice',
	                                                                    'Customer Opening Balance',
	                                                                    'Customer Advance Payment',
	                                                                    'Check Cancel (Customer)',
	                                                                    'SalesReturn Invoice',
	                                                                    'Credit Note',
	                                                                    'PDC Receivable'
	                                                                ) Then 
                                                                    IFNULL((SELECT NAME FROM tbl_customer WHERE t.hum_id),'')
                                                                    WHEN t.type IN (
                                                                          'Vendor Payment',
                                                                          'Petty Cash',
                                                                          'Purchase Invoice',
                                                                          'Vendor Opening Balance',
                                                                          'Vendor Advance Payment',
                                                                          'Check Cancel (Vendor)',
                                                                          'Purchase Return Invoice',
                                                                          'Debit Note',
                                                                          'PDC Payable'
                                                                    ) Then 
                                                                    IFNULL((SELECT NAME FROM tbl_vendor WHERE t.hum_id),'')
                                                                    WHEN t.type IN (
		                                                                'Employee Salary Payment',
		                                                                'Petty Cash',
		                                                                'Check Cancel (Employee)',
		                                                                'Employee Salary',
		                                                                'Employee Loan Payment',
		                                                                'Employee Petty Cash Payment'
	                                                                 ) Then 
                                                                    IFNULL((SELECT NAME FROM tbl_employee WHERE t.hum_id),'')
	                                                                 ELSE '' END AS name,
                                                                    (SELECT IFNULL(NAME,'') FROM tbl_coa_level_4 WHERE id = t.account_id) Account,
                                                                        COALESCE(t.description,'') AS memo
                                                                    FROM tbl_transaction t
                                                                    WHERE t.account_id = @acctId
                                                                      -- AND t.date BETWEEN @fromDate AND @toDate
                                                                    ORDER BY t.date, t.id
                                                                ",
                                        ClinicDatabase.CreateParameterNew("acctId", reader["id"].ToString()),
                                        ClinicDatabase.CreateParameterNew("fromDate", dtFrom.Value),
                                        ClinicDatabase.CreateParameterNew("toDate", dtTo.Value)))
                                        {
                                            while (tReader.Read())
                                            {
                                                string indent = new string(' ', 4 * 6 + 3);
                                                newRow = new DataGridViewRow();
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "n" });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = 5 });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell
                                                {
                                                    Value = tReader["id"].ToString()
                                                });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell
                                                {
                                                    Value = indent + "• " + tReader["type"].ToString()
                                                });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                                string dateStr0 = "";
                                                if (tReader["date"] != DBNull.Value)
                                                {
                                                    DateTime dt;
                                                    if (DateTime.TryParse(tReader["date"].ToString(), out dt))
                                                        dateStr0 = dt.ToString("MM/dd/yyyy");
                                                }
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = dateStr0 });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["voucher_no"].ToString() });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["Name"].ToString() });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["memo"].ToString() });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                                                newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["Account"].ToString() });

                                                newRow.Cells.Add(new DataGridViewTextBoxCell
                                                {
                                                    Value = Utilities.FormatDecimal(tReader["debit"].ToString())
                                                });

                                                newRow.Cells.Add(new DataGridViewTextBoxCell
                                                {
                                                    Value = Utilities.FormatDecimal(tReader["credit"].ToString())
                                                });

                                                //dgvCustomer.Rows.Insert(insertIndex, newRow);
                                                newRows.Add(newRow);
                                                insertIndex++;
                                            }
                                        }
                                    }
                                }
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
                        dgvCustomer.Rows[rowIndex].Cells["tranBalance"].Value = "";
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
        private void LoadTransactionsUnderLevel4Old(List<DataGridViewRow> listRows, int rowIndex, int level4Id)
        {
            DataGridViewRow row = dgvCustomer.Rows[rowIndex];

            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date;

            int insertIndex = rowIndex + 1;

            using (MySqlDataReader tReader = ClinicDatabase.ExecuteReader(@"
                                                                    SELECT 
                                                                        t.id,
                                                                        t.type,
                                                                        t.date,
                                                                        t.voucher_no,
                                                                        COALESCE(t.debit,0) AS debit,
                                                                        COALESCE(t.credit,0) AS credit,
                                                                    CASE WHEN t.type IN (
	                                                                    'Customer Receipt',
	                                                                    'Sales Invoice',
	                                                                    'Customer Opening Balance',
	                                                                    'Customer Advance Payment',
	                                                                    'Check Cancel (Customer)',
	                                                                    'SalesReturn Invoice',
	                                                                    'Credit Note',
	                                                                    'PDC Receivable'
	                                                                ) Then 
                                                                    IFNULL((SELECT NAME FROM tbl_customer WHERE t.hum_id),'')
                                                                    WHEN t.type IN (
                                                                          'Vendor Payment',
                                                                          'Petty Cash',
                                                                          'Purchase Invoice',
                                                                          'Vendor Opening Balance',
                                                                          'Vendor Advance Payment',
                                                                          'Check Cancel (Vendor)',
                                                                          'Purchase Return Invoice',
                                                                          'Debit Note',
                                                                          'PDC Payable'
                                                                    ) Then 
                                                                    IFNULL((SELECT NAME FROM tbl_vendor WHERE t.hum_id),'')
                                                                    WHEN t.type IN (
		                                                                'Employee Salary Payment',
		                                                                'Petty Cash',
		                                                                'Check Cancel (Employee)',
		                                                                'Employee Salary',
		                                                                'Employee Loan Payment',
		                                                                'Employee Petty Cash Payment'
	                                                                 ) Then 
                                                                    IFNULL((SELECT NAME FROM tbl_employee WHERE t.hum_id),'')
	                                                                 ELSE '' END AS name,
                                                                    (SELECT IFNULL(NAME,'') FROM tbl_coa_level_4 WHERE id = t.account_id) Account,
                                                                        COALESCE(t.description,'') AS memo
                                                                    FROM tbl_transaction t
                                                                    WHERE t.account_id = @acctId
                                                                      -- AND t.date BETWEEN @fromDate AND @toDate
                                                                    ORDER BY t.date, t.id
                                                                ",
            ClinicDatabase.CreateParameterNew("acctId", level4Id),
            ClinicDatabase.CreateParameterNew("fromDate", from),
            ClinicDatabase.CreateParameterNew("toDate", to)))
            {
                while (tReader.Read())
                {
                    string indent = new string(' ', 4 * 6 + 3);
                    var newRow = new DataGridViewRow();
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "n" });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = 5 });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value = tReader["id"].ToString()
                    });
                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value = indent + "• " + tReader["type"].ToString()
                    });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                    string dateStr0 = "";
                    if (tReader["date"] != DBNull.Value)
                    {
                        DateTime dt;
                        if (DateTime.TryParse(tReader["date"].ToString(), out dt))
                            dateStr0 = dt.ToString("MM/dd/yyyy");
                    }
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = dateStr0 });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["voucher_no"].ToString() });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["Name"].ToString() });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["memo"].ToString() });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = "" });
                    newRow.Cells.Add(new DataGridViewTextBoxCell { Value = tReader["Account"].ToString() });

                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value = Utilities.FormatDecimal(tReader["debit"].ToString())
                    });

                    newRow.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value = Utilities.FormatDecimal(tReader["credit"].ToString())
                    });

                    //dgvCustomer.Rows.Insert(insertIndex, newRow);
                    listRows.Add(newRow);
                    insertIndex++;
                }
            }

            //dgvCustomer.Rows[rowIndex].Cells["loadState"].Value = "l";
            //dgvCustomer.Rows[rowIndex].Cells["state"].Value = "c";
        }
        public void BindDGVOld()
        {
            dgvCustomer.Rows.Clear();

            // Detailed: list each account that has transactions in the date range,
            // add account header row then the transactions below it.
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date;
            dgvCustomer.Rows.Add("e", "u", "1", "Ordinary Income/Expense", "0", "", "", "", "", "", "", "", "", "", "", "", "");
            // First obtain accounts (level4) that have transactions in the date range, along with account balances
            using (MySqlDataReader acctReader = ClinicDatabase.ExecuteReader(@"
                    SELECT 
                        l1.name AS level1_name,
                        l2.name AS level2_name,
                        l3.name AS level3_name,
                        l4.name AS level4_name,
                        l4.id AS level4_id,
                        l4.code AS level4_code,
                        COALESCE(SUM(t.debit) - SUM(t.credit), 0) AS account_balance
                    FROM tbl_coa_level_1 l1
                    LEFT JOIN tbl_coa_level_2 l2 ON l2.main_id = l1.id
                    LEFT JOIN tbl_coa_level_3 l3 ON l3.main_id = l2.id
                    LEFT JOIN tbl_coa_level_4 l4 ON l4.main_id = l3.id
                    LEFT JOIN tbl_transaction t ON t.account_id = l4.id AND t.date BETWEEN @fromDate AND @toDate
                    WHERE l1.category_code IN ('Income', 'Cost', 'EXPENSE')
                    GROUP BY l1.id , l1.name , l2.name, l3.name, l4.id
                    ORDER BY l1.id, l4.code;
                ", ClinicDatabase.CreateParameterNew("fromDate", from), ClinicDatabase.CreateParameterNew("toDate", to)))
            {
                while (acctReader.Read())
                {
                    string accountFull = (acctReader["level4_code"] != DBNull.Value ? acctReader["level4_code"].ToString() + " - " : "") + acctReader["level4_name"].ToString();
                    string accountName = acctReader["level1_name"].ToString();
                    string acctId = string.IsNullOrEmpty(acctReader["level4_id"].ToString()) ? "0" : acctReader["level4_id"].ToString();
                    string acctBalance = Utilities.FormatDecimal(acctReader["account_balance"].ToString());

                    // Account header row
                    dgvCustomer.Rows.Add("n", "", "2", "  " + "   " + " " + accountName, "0", "", "", "", "", "", "", "", "", "", "", "", "");
                    dgvCustomer.Rows.Add("e", "u", "3", "      " + "   " + " " + accountFull, acctId, accountFull, "", "", "", "", "", "", "", "", "", "", "");
                    decimal subTotalDebit = 0, subTotalCredit = 0, subTotalBalance = 0;
                    // Load transactions for this account
                    using (MySqlDataReader tReader = ClinicDatabase.ExecuteReader(@"
                                SELECT 
                                    t.id,
                                    t.type,
                                    t.date,
                                    t.voucher_no AS num,
                                    '' AS adjustment,
        
                                    -- Get name based on transaction type
                                    COALESCE(
                                        CASE 
                                            WHEN t.type IN ('Customer','Sales Invoice Cash','Sales Invoice','Credit Note','SalesReturn Invoice','Customer Receipt','Customer Opening Balance','Customer Advance Payment','Check Cancel (Customer)','PDC Receivable') THEN
                                                (SELECT concat(c.code,' ', c.name) 
                                                 FROM tbl_customer c
                                                 JOIN tbl_transaction x ON x.hum_id = c.id 
                                                 WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type LIMIT 1)
                                            WHEN t.type IN ('Vendor','Purchase Invoice','Vendor Payment','Subcontractor Payment','Purchase Invoice Cash','Purchase Return Invoice','Debit Note','Vendor Opening Balance','Vendor Advance Payment','Check Cancel (Vendor)','PDC Payable') THEN
                                                (SELECT concat(v.code,' ', v.name) 
                                                 FROM tbl_vendor v
                                                 JOIN tbl_transaction x ON x.hum_id = v.id
                                                 WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type LIMIT 1)
                                            WHEN t.type IN ('Employee','Petty Cash','Employee Salary Payment','Employee Loan Payment','Check Cancel (Employee)','Employee Salary') THEN
                                                (SELECT concat(e.code,' ', e.name) 
                                                 FROM tbl_employee e
                                                 JOIN tbl_transaction x ON x.hum_id = e.id
                                                 WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type LIMIT 1)
                                            ELSE ''
                                        END, ''
                                    ) AS name,
                                    COALESCE(
                                     (
                                      SELECT concat(lev4.code,' ',lev4.name) FROM tbl_transaction x JOIN tbl_coa_level_4 lev4 ON lev4.id = x.account_id WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type AND x.hum_id>0
                                     )
                                     , '') AS AccountNamed,
                                    COALESCE(t.description, '') AS memo,
                                    '' AS clr,
                                    COALESCE((select name from tbl_coa_level_4 where id=t.account_id), '') AS split,
                                    COALESCE(
                                    (SELECT x.debit FROM tbl_transaction x WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type AND x.hum_id>0)
	                                 , 0) AS debit,
                                    COALESCE((SELECT x.credit FROM tbl_transaction x WHERE x.transaction_id = t.transaction_id AND x.TYPE = t.type AND x.hum_id>0), 0) AS credit,
        
                                    -- running balance up to and including this transaction
                                    (SELECT COALESCE(SUM(tt.debit - tt.credit),0) 
                                     FROM tbl_transaction tt 
                                     WHERE tt.transaction_id = t.transaction_id and tt.type=t.type AND tt.id <= t.id AND tt.hum_id>0) AS running_balance

                                FROM tbl_transaction t
                                WHERE t.account_id = @acctId AND t.date BETWEEN @fromDate AND @toDate
                                ORDER BY t.date, t.id;
                            ",
                        ClinicDatabase.CreateParameterNew("acctId", int.Parse(acctId)),
                        ClinicDatabase.CreateParameterNew("fromDate", from),
                        ClinicDatabase.CreateParameterNew("toDate", to)))
                    {
                        int insertIndex = dgvCustomer.Rows.Count; // append after header
                        while (tReader.Read())
                        {
                            string space = new string(' ', 6 * 5 + 3); // keep transaction indent smaller than account
                            string type = tReader["type"]?.ToString() ?? "";
                            string dateStr = "";
                            if (tReader["date"] != DBNull.Value)
                            {
                                DateTime dt;
                                if (DateTime.TryParse(tReader["date"].ToString(), out dt))
                                    dateStr = dt.ToString("MM/dd/yyyy");
                            }

                            string num = tReader["num"]?.ToString() ?? "";
                            string adj = tReader["adjustment"]?.ToString() ?? "";
                            string trName = tReader["name"]?.ToString() ?? "";
                            string AcName = tReader["AccountNamed"]?.ToString() ?? "";
                            string memo = tReader["memo"]?.ToString() ?? "";
                            string clr = tReader["clr"]?.ToString() ?? "";
                            string split = tReader["split"]?.ToString() ?? "";
                            string debit = Utilities.FormatDecimal(tReader["debit"].ToString());
                            string credit = Utilities.FormatDecimal(tReader["credit"].ToString());
                            string running = Utilities.FormatDecimal(tReader["running_balance"].ToString());

                            // transaction row values must match columns added in AddDGVColumns
                            dgvCustomer.Rows.Add(
                                "n",               // state
                                "",                // loadState
                                "5",               // currLvl (transaction level)
                                "", // lvl1 (indented display)
                                tReader["id"].ToString(), // id (transaction id)
                                "",       // currname
                                type,              // tranType
                                dateStr,           // tranDate
                                num,               // tranNum
                                adj,               // tranAdj
                                trName,            // tranName
                                memo,              // tranMemo
                                clr,               // tranClr
                                AcName,             // tranSplit
                                debit,             // tranDebit
                                credit,            // tranCredit
                                running            // tranBalance
                            );
                            subTotalDebit += decimal.Parse(debit);
                            subTotalCredit += decimal.Parse(credit);
                            subTotalBalance += decimal.Parse(running);
                            // optionally set formatting per-row (thin)
                            dgvCustomer.Rows[dgvCustomer.Rows.Count - 1].Height = dgvCustomer.RowTemplate.Height;
                        }
                    }
                    dgvCustomer.Rows.Add("n", "", "2", "  " + "   " + "Total " + accountFull, "0", "", "", "", "", "", "", "", "", "", subTotalDebit, subTotalCredit, subTotalBalance);

                    // Optionally auto-expand account (we already appended transactions so mark as loaded)
                    int headerRowIndex = dgvCustomer.Rows.Count - 1;
                    // find the header row (last header inserted is before its transactions)
                    for (int r = 0; r < dgvCustomer.Rows.Count; r++)
                    {
                        if (dgvCustomer.Rows[r].Cells["id"].Value?.ToString() == acctId &&
                            dgvCustomer.Rows[r].Cells["currLvl"].Value?.ToString() == "4")
                        {
                            dgvCustomer.Rows[r].Cells["loadState"].Value = "l";
                            break;
                        }
                    }
                }
            }
        }
        private void ExpandRowRecursivelyOld(int rowIndex)
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
                            dgvCustomer.Rows[rowIndex].Cells["balance"].Value = "";

                            while (reader.Read())
                            {
                                string nextState = (currLvl + 1 == 4) ? "n" : "e";
                                string nextIcon = (currLvl + 1 == 4) ? "  " : "►";
                                var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString());

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
                                    newRow.Height = dgvCustomer.Rows[rowIndex].Height;

                                    ApplyRowStyle(newRow, currLvl + 1);
                                    newRows.Add(newRow);
                                }
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
                //frmMain.openChildForm(new MasterTransactionByAccount("Transactions Detail By Account", int.Parse(dgvCustomer.Rows[e.RowIndex].Cells["id"].Value.ToString()), dtFrom.Value.ToString(), dtTo.Value.ToString()));
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
                                var balanceData = GetBalanceData(currLvl + 1, reader["currname"].ToString());
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
                                    newRow.Height = dgvCustomer.Rows[e.RowIndex].Height;

                                    ApplyRowStyle(newRow, currLvl + 1);
                                    newRows.Add(newRow);
                                }
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
                    var parentBalanceData = GetBalanceData(currLvl, dgvCustomer.Rows[e.RowIndex].Cells["currname"].Value.ToString().Replace("▼", ""));

                    if (!string.IsNullOrEmpty(parentBalanceData))
                    {
                        dgvCustomer.Rows[e.RowIndex].Cells["balance"].Value = Utilities.FormatDecimal(parentBalanceData);
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
                             )
                            ) AS level1_balance,

                            -- Balance for Level 2 (Summing its child transactions)
                            (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                             FROM tbl_transaction tt
                             WHERE tt.account_id IN (
                                 SELECT id FROM tbl_coa_level_4 
                                 WHERE main_id IN (SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id = t2.id)
                             )
                            ) AS level2_balance,

                            -- Balance for Level 3 (Summing only Level 4 transactions)
                            (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                             FROM tbl_transaction tt
                             WHERE tt.account_id IN (
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

                        WHERE t1.category_code IN ('Income', 'Cost', 'EXPENSE') AND t{level}.name = @name

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

        private string GetBalanceDataOld(int level, string name)
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
                             )
                            ) AS level1_balance,

                            -- Balance for Level 2 (Summing its child transactions)
                            (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                             FROM tbl_transaction tt
                             WHERE tt.account_id IN (
                                 SELECT id FROM tbl_coa_level_4 
                                 WHERE main_id IN (SELECT id FROM tbl_coa_level_3 
                                                  WHERE main_id = t2.id)
                             )
                            ) AS level2_balance,

                            -- Balance for Level 3 (Summing only Level 4 transactions)
                            (SELECT COALESCE(SUM(tt.debit) - SUM(tt.credit), 0) 
                             FROM tbl_transaction tt
                             WHERE tt.account_id IN (
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

                        WHERE t1.category_code IN ('Income', 'Cost', 'EXPENSE') AND t{level}.name = @name

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
                    
                        Excel.Range headerRange0 = worksheet.Range["E1", "J1"];
                        headerRange0.Merge();
                        headerRange0.Value = lblCompany.Text;
                        headerRange0.Font.Bold = true;
                        headerRange0.Font.Name = "Times New Roman";
                        headerRange0.Font.Size = 12;
                        headerRange0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange1 = worksheet.Range["E2", "J2"];
                        headerRange1.Merge();
                        headerRange1.Value = guna2HtmlLabel7.Text;
                        headerRange1.Font.Bold = true;
                        headerRange1.Font.Name = "Times New Roman";
                        headerRange1.Font.Size = 11;
                        headerRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        Excel.Range headerRange2 = worksheet.Range["E3", "J3"];
                        headerRange2.Merge();
                        headerRange2.Value = "From "+dtFrom.Text+" - To "+dtTo.Text;
                        headerRange2.Font.Bold = false;
                        headerRange2.Font.Name = "Times New Roman";
                        headerRange2.Font.Size = 9;
                        headerRange2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        // Merge and format header row (Date)
                        Excel.Range headerRange3 = worksheet.Range["B4", "C4"];
                        headerRange3.Merge();
                        headerRange3.Value = DateTime.Now.ToString("MMM dd, yy");
                        headerRange3.Font.Bold = false;
                        headerRange3.Font.Name = "Times New Roman";
                        headerRange3.Font.Size = 9;
                        headerRange3.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        int rowIndex = 5;

                        // 1️⃣ Write header row first
                        for (int col = 0; col < dgvCustomer.Columns.Count; col++)
                        {
                            var column = dgvCustomer.Columns[col];

                            // Skip hidden columns if needed
                            if (!column.Visible) continue;

                            var headerCell = worksheet.Cells[rowIndex, col];
                            headerCell.Value = column.HeaderText;  // show column header
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
                            if (row.IsNewRow) continue; // skip the new row

                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                var column = dgvCustomer.Columns[cell.ColumnIndex];
                                if (!column.Visible) continue;

                                var excelCell = worksheet.Cells[rowIndex, cell.ColumnIndex + 1];

                                // Set cell value
                                excelCell.Value = cell.Value?.ToString() ?? string.Empty;

                                // Default font
                                excelCell.Font.Name = "Times New Roman";
                                excelCell.Font.Size = 10;

                                // Handle dynamic columns by header text
                                string columnName = column.HeaderText.ToLower();

                                // Formatting rules
                                if (columnName.Contains("lvl1"))
                                {
                                    excelCell.Font.Bold = true;
                                }
                                else if (columnName.Contains("debit") ||
                                         columnName.Contains("credit") ||
                                         columnName.Contains("balance"))
                                {
                                    excelCell.Font.Size = 9;
                                    excelCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                }
                                else
                                {
                                    excelCell.Font.Size = 10;
                                    excelCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                }
                            }

                            rowIndex++; // move to next row
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
            FormattedText summaryText = center.AddFormattedText(title + "\n", TextFormat.Bold);
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
                    dataTable.AddColumn(Unit.FromCentimeter(3)); // Adjust width as needed
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
                    if (colName.Contains("debit") || colName.Contains("credit") || colName.Contains("balance"))
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

            //section.Add(dataTable);

            // Render and save
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), title + ".pdf");
            renderer.PdfDocument.Save(filePath);
            Process.Start("explorer.exe", filePath);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DateRangeHelper.SetDateRange(comboBox1.SelectedItem.ToString(), dtFrom, dtTo);
                //AddDGVColumns();
                BindDGV();
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            BindDGV();
        }
    }
}