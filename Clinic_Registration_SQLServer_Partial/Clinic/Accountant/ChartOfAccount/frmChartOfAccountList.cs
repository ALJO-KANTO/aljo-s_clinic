using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmChartOfAccountList : Form
    {
        DataTable datatable;
        public frmChartOfAccountList(string _type)
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            this.Text = "Chart Of Account Level";
            headerUC1.FormText = this.Text;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmChartOfAccountList_Load(object sender, EventArgs e)
        {
            BindData();
            BindCombos.PopulateAllLevel4Account(cmbAccount);
        }
        private void BindData()
        {
            datatable = ClinicDatabase.ExecuteDataTable(@"SELECT l1.code AS `Level1 Code`,l1.name AS `Level1 Name`,
                                                    l2.code AS `Level2 Code`,l2.name AS `Level2 Name`,
                                                    l3.code AS `Level3 Code`,l3.name AS `Level3 Name`,
                                                    l4.code AS `Level4 Code`,l4.name AS `Level4 Name`,
                                                    l4.debit `Level4 Debit`,l4.credit `Level4 Credit`
                                                    FROM tbl_coa_level_4 l4,tbl_coa_level_3 l3,tbl_coa_level_2 l2,tbl_coa_level_1 l1 
                                                    WHERE l4.main_id=l3.id AND l3.main_id=l2.id AND l2.main_id=l1.id;");
            if (datatable != null && datatable.Rows.Count > 0)
            {
                dgvCustomer.DataSource = datatable;
                dgvCustomer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                dgvCustomer.Columns["Level4 Debit"].DefaultCellStyle.Alignment = dgvCustomer.Columns["Level4 Credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCustomer.Columns["Level4 Debit"].DefaultCellStyle.Format = dgvCustomer.Columns["Level4 Credit"].DefaultCellStyle.Format = "N2";

                LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);

                openingBalanceEquity = BindCombos.SelectDefaultLevelAccount("Opening Balance Equity").ToString();
                if (int.Parse(openingBalanceEquity) == 0)
                {
                    object result = ClinicDatabase.ExecuteScalar(@"SELECT id FROM tbl_coa_level_4 WHERE name = 'Opening Balance Equity'");
                    if (result != null && result != DBNull.Value)
                    {
                        openingBalanceEquity = result.ToString();
                    }
                }
            }
        }

        string openingBalanceEquity = "0";

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (chkRequiredDate())
            {
                insertData();
            }
        }

        private void insertData()
        {
            DateTime obDate = new DateTime(DateTime.Now.Year, 1, 1);
            try
            {
                //string accountId = cmbAccount.SelectedValue?.ToString() ?? "0";

                for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                {
                    int level1Id = 0, level2Id = 0, level3Id = 0, level4Id = 0;
                    //level 1
                    if (dgvCustomer.Rows[i].Cells["Level1 Name"].Value != null && dgvCustomer.Rows[i].Cells["Level1 Name"].Value.ToString() != "")
                    {
                        string code = dgvCustomer.Rows[i].Cells["Level1 Code"].Value.ToString();
                        string name = dgvCustomer.Rows[i].Cells["Level1 Name"].Value.ToString();
                        string categoryCode = "";
                        if (chkCustomizedFile.Checked)
                        {
                            object objCode = ClinicDatabase.ExecuteScalar("select max(code) as code from tbl_coa_level_1");
                            code = objCode != null ? (int.Parse(objCode.ToString()) + 1).ToString() : string.IsNullOrEmpty(code) ? "1" : code;
                            categoryCode = GetMainAccountCategoryCode(name);
                        }

                        using (var reader = ClinicDatabase.ExecuteReader("SELECT id FROM tbl_coa_level_1 WHERE name = @name or code=@code",
                            ClinicDatabase.CreateParameterNew("name", name),
                            ClinicDatabase.CreateParameterNew("code", code)))
                        {
                            if (reader.Read())
                            {
                                level1Id = Convert.ToInt32(reader["id"]);
                            }
                            else
                            {
                                level1Id = Convert.ToInt32(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_coa_level_1 (code, name, category_code) 
                                                VALUES(@code, @name, @category_code);
                                                SELECT LAST_INSERT_ID();",
                                                    ClinicDatabase.CreateParameterNew("code", code),
                                                    ClinicDatabase.CreateParameterNew("category_code", categoryCode),
                                                    ClinicDatabase.CreateParameterNew("name", name)).ToString());
                            }
                        }
                    }
                    //level 2
                    if (dgvCustomer.Rows[i].Cells["Level2 Name"].Value != null && dgvCustomer.Rows[i].Cells["Level2 Name"].Value.ToString() != "")
                    {
                        string code = dgvCustomer.Rows[i].Cells["Level2 Code"].Value.ToString();
                        string name = dgvCustomer.Rows[i].Cells["Level2 Name"].Value.ToString();
                        if (chkCustomizedFile.Checked)
                        {
                            object objL1Code = ClinicDatabase.ExecuteScalar("select code from tbl_coa_level_1 where id = @id", ClinicDatabase.CreateParameterNew("id", level1Id));
                            string l1Code = objL1Code != null ? objL1Code.ToString() : "000";
                            object objCode = ClinicDatabase.ExecuteScalar("select max(code) as code from tbl_coa_level_2 where code like '" + l1Code + "%'");
                            code = objCode != null ? (int.Parse(objCode.ToString()) + 1).ToString() : string.IsNullOrEmpty(code) ? level1Id + "1" : code;
                        }

                        using (var reader = ClinicDatabase.ExecuteReader("SELECT id FROM tbl_coa_level_2 WHERE name = @name or code=@code",
                            ClinicDatabase.CreateParameterNew("name", name),
                            ClinicDatabase.CreateParameterNew("code", code)))
                        {
                            if (reader.Read())
                            {
                                level2Id = Convert.ToInt32(reader["id"]);
                            }
                            else
                            {
                                level2Id = Convert.ToInt32(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_coa_level_2 (code, name,main_id) 
                                                VALUES(@code, @name,@mainId);
                                                SELECT LAST_INSERT_ID();",
                                                    ClinicDatabase.CreateParameterNew("code", code),
                                                    ClinicDatabase.CreateParameterNew("name", name),
                                                    ClinicDatabase.CreateParameterNew("mainId", level1Id.ToString())).ToString());
                            }
                        }
                    }
                    //level 3
                    if (dgvCustomer.Rows[i].Cells["Level3 Name"].Value != null && dgvCustomer.Rows[i].Cells["Level3 Name"].Value.ToString() != "")
                    {
                        string code = dgvCustomer.Rows[i].Cells["Level3 Code"].Value.ToString();
                        string name = dgvCustomer.Rows[i].Cells["Level3 Name"].Value.ToString();
                        if (chkCustomizedFile.Checked)
                        {
                            object objL2Code = ClinicDatabase.ExecuteScalar("select code from tbl_coa_level_2 where id = @id", ClinicDatabase.CreateParameterNew("id", level2Id));
                            string l2Code = objL2Code != null ? objL2Code.ToString() : "001";
                            object objCode2 = ClinicDatabase.ExecuteScalar("select max(code) as code from tbl_coa_level_3 where code like '" + l2Code + "%'");
                            code = objCode2 != null ? (int.Parse(objCode2.ToString()) + 1).ToString() : string.IsNullOrEmpty(code) ? l2Code + "01" : code;
                        }
                        using (var reader = ClinicDatabase.ExecuteReader("SELECT id FROM tbl_coa_level_3 WHERE name = @name or code=@code",
                            ClinicDatabase.CreateParameterNew("name", name),
                            ClinicDatabase.CreateParameterNew("code", code)))
                        {
                            if (reader.Read())
                            {
                                level3Id = Convert.ToInt32(reader["id"]);
                            }
                            else
                            {
                                level3Id = Convert.ToInt32(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_coa_level_3 (code, name,main_id) 
                                                VALUES(@code, @name,@mainId);
                                                SELECT LAST_INSERT_ID();",
                                                    ClinicDatabase.CreateParameterNew("code", code),
                                                    ClinicDatabase.CreateParameterNew("name", name),
                                                    ClinicDatabase.CreateParameterNew("mainId", level2Id.ToString())).ToString());
                            }
                        }
                    }
                    //level 4
                    if (dgvCustomer.Rows[i].Cells["Level4 Name"].Value != null && dgvCustomer.Rows[i].Cells["Level4 Name"].Value.ToString() != "")
                    {
                        string code = dgvCustomer.Rows[i].Cells["Level4 Code"].Value.ToString();
                        string name = dgvCustomer.Rows[i].Cells["Level4 Name"].Value.ToString();
                        string debit = "0", credit = "0";
                        if (chkCustomizedFile.Checked)
                        {
                            object objL3Code = ClinicDatabase.ExecuteScalar("select code from tbl_coa_level_3 where id = @id", ClinicDatabase.CreateParameterNew("id", level3Id));
                            string l3Code = objL3Code != null ? objL3Code.ToString() : "001";
                            object objCode3 = ClinicDatabase.ExecuteScalar("select max(code) as code from tbl_coa_level_4 where code like '" + l3Code + "%'");
                            code = objCode3 != null ? (int.Parse(objCode3.ToString()) + 1).ToString() : string.IsNullOrEmpty(code) ? l3Code + "01" : code;
                        }
                        if (dgvCustomer.Rows[i].Cells["Level4 Debit"].Value != null && dgvCustomer.Rows[i].Cells["Level4 Debit"].Value.ToString() != "") 
                            debit = Convert.ToDecimal(dgvCustomer.Rows[i].Cells["Level4 Debit"].Value.ToString()).ToString("0.00");
                        
                        if(dgvCustomer.Rows[i].Cells["Level4 Credit"].Value != null && dgvCustomer.Rows[i].Cells["Level4 Credit"].Value.ToString() != "")
                            credit = Convert.ToDecimal(dgvCustomer.Rows[i].Cells["Level4 Credit"].Value.ToString()).ToString("0.00");

                        using (var reader = ClinicDatabase.ExecuteReader("SELECT id,main_id FROM tbl_coa_level_4 WHERE name = @name or code=@code", 
                            ClinicDatabase.CreateParameterNew("name", name),
                            ClinicDatabase.CreateParameterNew("code", code)))
                        {
                            if (reader.Read())
                            {
                                level4Id = Convert.ToInt32(reader["id"]);
                            }
                            else
                            {
                                level4Id = Convert.ToInt32(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_coa_level_4 (code, name,main_id,debit,credit,date) 
                                                VALUES(@code, @name,@mainId,@debit,@credit,@date);
                                                SELECT LAST_INSERT_ID();",
                                                    ClinicDatabase.CreateParameterNew("code", code),
                                                    ClinicDatabase.CreateParameterNew("name", name),
                                                    ClinicDatabase.CreateParameterNew("mainId", level3Id.ToString()),
                                                    ClinicDatabase.CreateParameterNew("debit", debit.ToString()),
                                                    ClinicDatabase.CreateParameterNew("credit", credit.ToString()),
                                                    ClinicDatabase.CreateParameterNew("date", obDate.Date)).ToString());

                                decimal creditAmount = decimal.Parse(credit);
                                decimal debitAmount = decimal.Parse(debit);

                                if (int.Parse(openingBalanceEquity) > 0)
                                {
                                    if (creditAmount != 0)
                                    {
                                        CommonInsert.addTransactionEntry(obDate.Date, openingBalanceEquity, creditAmount.ToString(), "0",
                                          level4Id.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Opening Balance Equity - Ledger", clsGlobal.CurrentUser.UserID, DateTime.Now.Date, "");

                                        CommonInsert.addTransactionEntry(obDate.Date, level4Id.ToString(), "0", creditAmount.ToString(),
                                           level4Id.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Account Payable - Ledger Code ", clsGlobal.CurrentUser.UserID, DateTime.Now.Date, "");
                                    }

                                    if (debitAmount != 0)
                                    {
                                        CommonInsert.addTransactionEntry(obDate.Date, openingBalanceEquity, "0", debitAmount.ToString(),
                                                level4Id.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Opening Balance Equity - Ledger Code ", clsGlobal.CurrentUser.UserID, DateTime.Now.Date, "");

                                        CommonInsert.addTransactionEntry(obDate.Date, level4Id.ToString(), debitAmount.ToString(), "0",
                                           level4Id.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Account Payable - Ledger Code - ", clsGlobal.CurrentUser.UserID, DateTime.Now.Date, "");
                                    }
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Successfully saved! ChartOfAccount List", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetMainAccountCategoryCode(string name)
        {
            string input = name.Trim().ToUpper();
            string categoryCode =
            input.StartsWith("ASSET") ? AccountCategory.ASSET :
            input.StartsWith("LIABILITY") ? AccountCategory.LIABILITY :
            input.StartsWith("EQUITY") ? AccountCategory.EQUITY :
            input.StartsWith("INCOME") ? AccountCategory.INCOME :
            input.StartsWith("COST") ? AccountCategory.COST :
            input.StartsWith("COST") ? AccountCategory.EXPENSE :
            "UNKNOWN" ;

            return categoryCode;
        }

        private bool chkRequiredDate()
        {
            if(dgvCustomer.Rows.Count == 1)
            {
                MessageBox.Show("Please Enter Data First");
                return false;
            }
            else {
                if (chkCustomizedFile.Created)
                {
                    for (int i = 0; i < dgvCustomer.Rows.Count; i++)
                    {
                        if (dgvCustomer.Rows[i].IsNewRow) continue;

                        if (dgvCustomer.Rows[i].Cells["Level4 Name"].Value == null || dgvCustomer.Rows[i].Cells["Level4 Name"].Value.ToString() == "")
                        {
                            MessageBox.Show("Please Enter Level4 Name");
                            return false;
                        }
                    }
                }
                else
                {
                    bool isValid = CheckForDuplicates(dgvCustomer);
                    if (!isValid)
                    {
                        MessageBox.Show("Please Check Duplicate Code and Name");
                        return false;
                    }
                }

            }

            return true;
        }
        private bool CheckForDuplicates(DataGridView dgvCustomer)
        {
            var levelNames = new[] { "Level1", "Level2", "Level3", "Level4" };
            var nameSets = new Dictionary<string, HashSet<string>>();
            var codeSets = new Dictionary<string, HashSet<string>>();

            // Initialize HashSets for each level
            foreach (var level in levelNames)
            {
                nameSets[level] = new HashSet<string>();
                codeSets[level] = new HashSet<string>();
            }

            foreach (DataGridViewRow row in dgvCustomer.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (var level in levelNames)
                {
                    string nameCol = $"{level} Name";
                    string codeCol = $"{level} Code";

                    string name = row.Cells[nameCol].Value?.ToString().Trim() ?? "";
                    string code = row.Cells[codeCol].Value?.ToString().Trim() ?? "";

                    // === Check Empty Name ===
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        MessageBox.Show($"Empty {level} Name found in the data.", $"{level} Name Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // === Check Empty Code ===
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        MessageBox.Show($"Empty {level} Code found for Name: {name}", $"{level} Code Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // === Check Duplicate Name ===
                    if (!nameSets[level].Add(name))
                    {
                        MessageBox.Show($"Duplicate {level} Name found: {name}", $"{level} Name Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // === Check Duplicate Code ===
                    if (!codeSets[level].Add(code))
                    {
                        MessageBox.Show($"Duplicate {level} Code found: {code}", $"{level} Code Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            return true; // No duplicates or missing values found
        }

        private decimal GetDecimalValue(DataGridViewRow row, string columnName)
        {
            decimal result;
            var cellValue = row.Cells[columnName].Value;
            if (cellValue != null && decimal.TryParse(cellValue.ToString(), out result))
                return result;
            else
                return 0;
        }
        
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ChartOfAccount");
                    worksheet.Cells["A1"].LoadFromDataTable(datatable, true);
                    
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo fi = new FileInfo(saveFileDialog.FileName);
                        package.SaveAs(fi);
                        MessageBox.Show("Export successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (chkCustomizedFile.Checked)
            {
                frmImportAccountData importForm = new frmImportAccountData();
                if (importForm.ShowDialog() == DialogResult.OK)
                {
                    DataTable importedData = importForm.ImportedData;
                    dgvCustomer.DataSource = importedData;
                    LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
                }
            }
            else
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    openFileDialog.Title = "Select Excel File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        LoadExcelSheets(filePath);
                    }
                }
            }
        }
        private void LoadExcelSheets(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("The selected file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                dgvCustomer.Columns.Clear();
                dgvCustomer.DataSource = null;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    if (sheet.Dimension != null)
                    {
                        DataTable dataTable = new DataTable(sheet.Name);

                        for (int col = 1; col <= sheet.Dimension.End.Column; col++)
                        {
                            string columnName = sheet.Cells[1, col].Text.Trim();
                            if (string.IsNullOrWhiteSpace(columnName)) columnName = "Column " + col;
                            dataTable.Columns.Add(columnName);
                        }

                        for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                        {
                            bool isRowEmpty = true;
                            DataRow dataRow = dataTable.NewRow();

                            for (int col = 1; col <= sheet.Dimension.End.Column; col++)
                            {
                                string cellValue = sheet.Cells[row, col].Text.Trim();

                                if (!string.IsNullOrWhiteSpace(cellValue))
                                    isRowEmpty = false;

                                dataRow[col - 1] = cellValue;
                            }

                            if (!isRowEmpty)
                                dataTable.Rows.Add(dataRow);
                        }

                        //RemoveEmptyColumns(dataTable);

                        dgvCustomer.DataSource = dataTable;
                        foreach (DataGridViewColumn column in dgvCustomer.Columns)
                        {
                            if (column.HeaderText == "name" || column.HeaderText == "country" || column.HeaderText == "building_name" || column.HeaderText == "TRN")
                            {
                                column.Width = 550;
                            }
                            else
                            {
                                column.Width = 200;
                            }
                        }

                        LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
                    }
                }
            }
        }
        private void RemoveEmptyColumns(DataTable dt)
        {
            List<DataColumn> emptyColumns = new List<DataColumn>();

            foreach (DataColumn col in dt.Columns)
            {
                bool isEmpty = true;

                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(row[col].ToString()))
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty)
                    emptyColumns.Add(col);
            }

            foreach (DataColumn col in emptyColumns)
            {
                dt.Columns.Remove(col);
            }
        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCode.Text) && txtCode.Text.Length > 0)
            {
                //
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}

public class frmImportAccountData : Form
{
    private TextBox txtStartCell, txtEndCell;
    TextBox txtL1Code, txtL1Name, txtL2Code, txtL2Name, txtL3Code, txtL3Name, txtL4Code, txtL4Name, txtDebit, txtCredit;
    private Button btnLoadExcel, btnImport;
    private DataGridView dataGridView;
    private string excelPath = string.Empty;
    DataTable dtFiltered = new DataTable();
    public DataTable ImportedData { get; private set; }

    public frmImportAccountData()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        this.Text = "Excel Import Form";
        this.Size = new Size(1624, 900);
        this.StartPosition = FormStartPosition.CenterScreen;

        // ==== Top Panel ====
        Panel panelTop = new Panel()
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(10),
        };

        // Starting X position for controls
        int x = 10;
        int spacing = 8;
        int labelWidth = 80;
        int textboxWidth = 15;
        int controlHeight = 28;

        // Excel buttons
        btnLoadExcel = new Button() { Text = "Load Excel", Size = new Size(100, controlHeight), Location = new Point(x, 10) };
        x += 100 + spacing;

        btnImport = new Button() { Text = "Set Data", Size = new Size(80, controlHeight), Location = new Point(x, 10) };
        x += 80 + spacing;

        // Start Cell
        panelTop.Controls.Add(new Label() { Text = "Start Cell:", AutoSize = true, Location = new Point(x, 15) });
        x += labelWidth;
        txtStartCell = new TextBox() { Width = textboxWidth, Location = new Point(x - 13, 12) };
        x += textboxWidth + spacing;

        // End Cell
        panelTop.Controls.Add(new Label() { Text = "End Cell:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtEndCell = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL1Code
        panelTop.Controls.Add(new Label() { Text = "L1 Code:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL1Code = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL1Name
        panelTop.Controls.Add(new Label() { Text = "L1 Name:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL1Name = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL2Code
        panelTop.Controls.Add(new Label() { Text = "L2 Code:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL2Code = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL2Name
        panelTop.Controls.Add(new Label() { Text = "L2 Name:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL2Name = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL3Code
        panelTop.Controls.Add(new Label() { Text = "L3 Code:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL3Code = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL3Name
        panelTop.Controls.Add(new Label() { Text = "L3 Name:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL3Name = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL4Code
        panelTop.Controls.Add(new Label() { Text = "L4 Code:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL4Code = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtL4Name
        panelTop.Controls.Add(new Label() { Text = "L4 Name:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtL4Name = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtDebit
        panelTop.Controls.Add(new Label() { Text = "Debit:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtDebit = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // txtCredit
        panelTop.Controls.Add(new Label() { Text = "Credit:", AutoSize = true, Location = new Point(x - 10, 15) });
        x += labelWidth;
        txtCredit = new TextBox() { Width = textboxWidth, Location = new Point(x - 27, 12) };
        x += textboxWidth + spacing;

        // Add controls to top panel
        panelTop.Controls.Add(txtStartCell);
        panelTop.Controls.Add(txtEndCell);
        panelTop.Controls.Add(txtL1Code);
        panelTop.Controls.Add(txtL1Name);
        panelTop.Controls.Add(txtL2Code);
        panelTop.Controls.Add(txtL2Name);
        panelTop.Controls.Add(txtL3Code);
        panelTop.Controls.Add(txtL3Name);
        panelTop.Controls.Add(txtL4Code);
        panelTop.Controls.Add(txtL4Name);
        panelTop.Controls.Add(txtDebit);
        panelTop.Controls.Add(txtCredit);

        // add buttons to top panel
        panelTop.Controls.Add(btnLoadExcel);
        panelTop.Controls.Add(btnImport);

        this.Controls.Add(panelTop);

        btnLoadExcel.Click += BtnLoadExcel_Click;
        btnImport.Click += BtnImport_Click;

        // ==== Main Panel ====
        Panel panelMain = new Panel()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
        };

        dataGridView = new DataGridView()
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        panelMain.Controls.Add(dataGridView);

        // Add both panels to form
        this.Controls.Add(panelMain);
        this.Controls.Add(panelTop);

        txtL1Code.KeyPress += TxtLettersOnly_KeyPress;
        txtL1Name.KeyPress += TxtLettersOnly_KeyPress;
        txtL2Code.KeyPress += TxtLettersOnly_KeyPress;
        txtL2Name.KeyPress += TxtLettersOnly_KeyPress;
        txtL3Code.KeyPress += TxtLettersOnly_KeyPress;
        txtL3Name.KeyPress += TxtLettersOnly_KeyPress;
        txtL4Code.KeyPress += TxtLettersOnly_KeyPress;
        txtL4Name.KeyPress += TxtLettersOnly_KeyPress;
        txtDebit.KeyPress += TxtLettersOnly_KeyPress;
        txtCredit.KeyPress += TxtLettersOnly_KeyPress;
    }

    private void BtnImport_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(excelPath))
        {
            MessageBox.Show("Please load an Excel file first.");
            return;
        }
        else
        {
            //dataGridView.Rows.Clear();
            //dataGridView.Columns.Clear();
            if (dataGridView.Rows.Count > 0)
            {
                int startCol = ToNumber(txtStartCell.Text);
                int endCol = ToNumber(txtEndCell.Text);

                // Check for invalid inputs
                if (startCol == -1 || endCol == -1 || endCol < startCol)
                {
                    MessageBox.Show("Invalid start or end column.");
                    return;
                }

                int cL1Code = ExcelColumnNameToNumber(txtL1Code.Text);
                int cL1Name = ExcelColumnNameToNumber(txtL1Name.Text);
                int cL2Code = ExcelColumnNameToNumber(txtL2Code.Text);
                int cL2Name = ExcelColumnNameToNumber(txtL2Name.Text);
                int cL3Code = ExcelColumnNameToNumber(txtL3Code.Text);
                int cL3Name = ExcelColumnNameToNumber(txtL3Name.Text);
                int cL4Code = ExcelColumnNameToNumber(txtL4Code.Text);
                int cL4Name = ExcelColumnNameToNumber(txtL4Name.Text);
                int cDebit = ExcelColumnNameToNumber(txtDebit.Text);
                int cCredit = ExcelColumnNameToNumber(txtCredit.Text);

                using (var package = new ExcelPackage(new FileInfo(excelPath)))
                {
                    // Create DataTable with new columns

                    dtFiltered.Columns.Add("Level1 Code");
                    dtFiltered.Columns.Add("Level1 Name");
                    dtFiltered.Columns.Add("Level2 Code");
                    dtFiltered.Columns.Add("Level2 Name");
                    dtFiltered.Columns.Add("Level3 Code");
                    dtFiltered.Columns.Add("Level3 Name");
                    dtFiltered.Columns.Add("Level4 Code");
                    dtFiltered.Columns.Add("Level4 Name");
                    dtFiltered.Columns.Add("Level4 Debit");
                    dtFiltered.Columns.Add("Level4 Credit");

                    var ws = package.Workbook.Worksheets[0];
                    for (int row = startCol; row <= endCol; row++)
                    {
                        if (IsRowEmpty(ws, row, endCol)) continue;
                        
                        DataRow dr = dtFiltered.NewRow();

                        //dr["Code"] = (colCode != -1) ? ws.Cells[row, colCode].Text : "";
                        dr["Level1 Code"] = (cL1Code != -1) ? ws.Cells[row, cL1Code].Text : "";
                        dr["Level1 Name"] = (cL1Name != -1) ? ws.Cells[row, cL1Name].Text : "";
                        dr["Level2 Code"] = (cL2Code != -1) ? ws.Cells[row, cL2Code].Text : "";
                        dr["Level2 Name"] = (cL2Name != -1) ? ws.Cells[row, cL2Name].Text : "";
                        dr["Level3 Code"] = (cL3Code != -1) ? ws.Cells[row, cL3Code].Text : "";
                        dr["Level3 Name"] = (cL3Name != -1) ? ws.Cells[row, cL3Name].Text : "";
                        dr["Level4 Code"] = (cL4Code != -1) ? ws.Cells[row, cL4Code].Text : "";
                        dr["Level4 Name"] = (cL4Name != -1) ? ws.Cells[row, cL4Name].Text : "";
                        dr["Level4 Debit"] = (cDebit != -1) ? ws.Cells[row, cDebit].Text : "0";
                        dr["Level4 Credit"] = (cCredit != -1) ? ws.Cells[row, cCredit].Text : "0";

                        dtFiltered.Rows.Add(dr);
                    }

                    // Now you can bind the filtered table somewhere or return it
                    dataGridView.DataSource = dtFiltered;
                }

                this.ImportedData = dtFiltered;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please load an Excel file first.");
                return;
            }
        }
    }

    private int ToNumber(string text)
    {
        if (string.IsNullOrEmpty(text)) return -1;
        else return int.Parse(text);
    }

    int ExcelColumnNameToNumber(string columnName)
    {
        if (string.IsNullOrEmpty(columnName)) return -1;

        columnName = columnName.ToUpperInvariant();
        int sum = 0;
        foreach (char c in columnName)
        {
            if (c < 'A' || c > 'Z') return -1; // invalid char
            sum *= 26;
            sum += (c - 'A' + 1);
        }
        return sum;
    }
    private void TxtIntOnly_KeyPress(object sender, KeyPressEventArgs e)
    {
        // Allow control keys like Backspace
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        {
            e.Handled = true; // Reject non-digit keys
        }
    }
    private void TxtLettersOnly_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar))
        {
            e.Handled = true; // block non-letter keys
        }
    }

    private void BtnLoadExcel_Click(object sender, EventArgs e)
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            excelPath = ofd.FileName;
            ImportToGrid();
        }
    }
    private void ImportToGrid()
    {
        if (string.IsNullOrEmpty(excelPath))
        {
            MessageBox.Show("Please load an Excel file first.");
            return;
        }

        try
        {
            using (var package = new ExcelPackage(new FileInfo(excelPath)))
            {
                var ws = package.Workbook.Worksheets[0];
                int startRow = ws.Dimension.Start.Row;
                int endRow = ws.Dimension.End.Row;
                int startCol = ws.Dimension.Start.Column;
                int endCol = ws.Dimension.End.Column;

                DataTable dt = new DataTable();

                // First column: "Index" or "Row"
                dt.Columns.Add("Index");

                // Set headers manually as A, B, C, etc.
                for (int col = startCol; col <= endCol; col++)
                {
                    string colLetter = GetExcelColumnLetter(col);
                    dt.Columns.Add(colLetter);
                }

                // Fill data from all rows
                for (int row = startRow; row <= endRow; row++)
                {
                    DataRow dr = dt.NewRow();

                    dr["Index"] = row.ToString();  // Add Excel-like row number

                    for (int col = startCol; col <= endCol; col++)
                    {
                        dr[col - startCol + 1] = ws.Cells[row, col].Text;
                    }

                    dt.Rows.Add(dr);
                }

                dataGridView.DataSource = dt;

                dataGridView.ReadOnly = true;
                dataGridView.RowHeadersVisible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading Excel: {ex.Message}");
        }
    }
    private string GetExcelColumnLetter(int col)
    {
        string columnString = "";
        int columnNumber = col;
        while (columnNumber > 0)
        {
            int currentLetterNumber = (columnNumber - 1) % 26;
            char currentLetter = (char)(currentLetterNumber + 65);
            columnString = currentLetter + columnString;
            columnNumber = (columnNumber - 1) / 26;
        }
        return columnString;
    }

    private bool IsRowEmpty(ExcelWorksheet ws, int row, int maxCol)
    {
        for (int col = 1; col <= maxCol; col++)
        {
            if (!string.IsNullOrWhiteSpace(ws.Cells[row, col].Text))
                return false;
        }
        return true;
    }
}