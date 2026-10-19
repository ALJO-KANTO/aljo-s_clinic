using Clinic.Login;
using Clinic_DataAccess;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using YamyProject;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewPettyCashVoucherJournal : Form
    {
        int id;
        decimal totalAmount = 0;
        string code;
        bool isOldBill = false;
        int pettyCashId = 0, pettyCashAccountId;

        public frmViewPettyCashVoucherJournal(int id = 0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            this.id = id;

            headerUC1.FormText = this.Text;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmViewPettyCashVoucherJournal_Load(object sender, EventArgs e)
        {
            dtOpen.Value = DateTime.Now.Date;
            //AppTheme.DefaultFont(dgvInv);
            // Load Accounts (Name)
            DataTable dtAccounts = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE , ' - ' , NAME) AS name, id FROM tbl_coa_level_4");
            if (dgvInv.Columns.Contains("AC_Name") && dgvInv.Columns["AC_Name"] is DataGridViewComboBoxColumn cmbAccount)
            {
                cmbAccount.DataSource = dtAccounts;
                cmbAccount.DisplayMember = "name";
                cmbAccount.ValueMember = "id";
            }

            // Load Accounts (Code)
            DataTable dtAccountCodes = ClinicDatabase.ExecuteDataTable("SELECT code AS name, id FROM tbl_coa_level_4");
            if (dgvInv.Columns.Contains("AC_Code") && dgvInv.Columns["AC_Code"] is DataGridViewComboBoxColumn cmbAccountCode)
            {
                cmbAccountCode.DataSource = dtAccountCodes;
                cmbAccountCode.DisplayMember = "name";
                cmbAccountCode.ValueMember = "id";
            }

            // Load Cost Centers
            DataTable dtCC = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE , ' - ' , NAME) AS name, id FROM tbl_sub_cost_center");
            if (dgvInv.Columns.Contains("CostCenter") && dgvInv.Columns["CostCenter"] is DataGridViewComboBoxColumn cmbCostcenter)
            {
                cmbCostcenter.DataSource = dtCC;
                cmbCostcenter.DisplayMember = "name";
                cmbCostcenter.ValueMember = "id";
            }

            // Populate petty cash combo
            BindCombos.PopulatePettyCash(cmbPettyCash);

            // Load voucher or set default
            if (id > 0)
                BindVoucher();
            else
                SetDefault();

            // Localize headers
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvInv);
        }


        private void BindVoucher()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT * FROM tbl_petty_cash WHERE id = " + id))
                if (reader.Read())
                {
                    txtNo.Text = txtCode.Text = reader["code"].ToString();
                    dtOpen.Value = DateTime.Parse(reader["voucher_date"].ToString());
                    code = reader["code"].ToString();
                    string cash_account_id = reader["cash_account_id"].ToString();
                    cmbPettyCash.SelectedValue = int.Parse(cash_account_id);
                    txtAmount.Text = reader["total"].ToString();
                    isOldBill = reader["status"].ToString().ToLower() == "1" ? true : false;
                    pettyCashId = int.Parse(reader["cash_account_id"].ToString());

                    var retResult = ClinicDatabase.ExecuteScalar(
                        @"SELECT pcc.account_id
                        FROM tbl_petty_cash_card pcc 
                        JOIN tbl_employee emp ON CAST(pcc.name AS UNSIGNED) = emp.id WHERE emp.id = @id",
                        ClinicDatabase.CreateParameterNew("id", cmbPettyCash.SelectedValue.ToString())
                    );
                    string ret = retResult != DBNull.Value ? retResult.ToString() : "0";

                    pettyCashAccountId = int.Parse(ret.ToString());

                    loadPettyCashDataDetails();
                }
        }

        private void loadPettyCashDataDetails()
        {
            dgvInv.Rows.Clear();

            int serialNumber = 1;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                SELECT ROW_NUMBER() OVER (ORDER BY entry_date) AS SN, 
                       dt.id, dt.petty_cash_id, dt.entry_date, dt.ref_id, dt.hum_id, dt.category,
                       dt.cost_center_id, dt.description, dt.amount,dt.vatP,dt.vat,dt.total, dt.project_id, dt.note
                FROM tbl_petty_cash_details dt 
                WHERE dt.petty_cash_id = @id",
                ClinicDatabase.CreateParameterNew("id", id)))
            {
                while (reader.Read())
                {
                    string inv_id = "0";
                    string inv_code = reader["ref_id"].ToString();
                    string inv_date = DateTime.Parse(reader["entry_date"].ToString()).ToShortDateString();
                    string description = reader["description"].ToString();
                    string PettyCashAmount = Utilities.FormatDecimal(reader["amount"].ToString());
                    string PettyCashVatAmount = Utilities.FormatDecimal(reader["vatP"].ToString());
                    string PettyCashVat = int.Parse(reader["vat"].ToString()).ToString();
                    string PettyCashTotal = Utilities.FormatDecimal(reader["total"].ToString());
                    string _humId = reader["hum_id"].ToString();
                    string vCategory = reader["category"].ToString();
                    string colNote = reader["note"].ToString() ?? "";

                    string cost_center_id = reader["cost_center_id"].ToString();
                    if (string.IsNullOrEmpty(cost_center_id))
                        cost_center_id = null; // optional: null if empty

                    // Add row with null placeholder for CostCenter
                    dgvInv.Rows.Add(serialNumber.ToString(), null, null, null, description, PettyCashAmount, PettyCashVatAmount, PettyCashTotal, PettyCashTotal, _humId, "View");

                    // Get the last added row (always last)
                    DataGridViewComboBoxCell comboCellCostCenter = dgvInv.Rows[dgvInv.Rows.Count - 2].Cells["CostCenter"] as DataGridViewComboBoxCell;
                    if (comboCellCostCenter != null && !string.IsNullOrEmpty(cost_center_id) && cost_center_id != "0")
                    {
                        comboCellCostCenter.Value = int.Parse(cost_center_id);
                    }

                    var retResult = ClinicDatabase.ExecuteScalar(
                        @"SELECT account_id FROM tbl_transaction WHERE transaction_id = @id AND t_type='PettyCash' and account_id !=@cashId AND DESCRIPTION=@description",
                        ClinicDatabase.CreateParameterNew("id", id.ToString()),
                        ClinicDatabase.CreateParameterNew("cashId", pettyCashAccountId),
                        ClinicDatabase.CreateParameterNew("description", description)
                    );
                    if (retResult != null)
                    {
                        string ret = retResult.ToString();
                        DataGridViewComboBoxCell comboCellAcName = dgvInv.Rows[dgvInv.Rows.Count - 2].Cells["AC_Name"] as DataGridViewComboBoxCell;
                        if (comboCellAcName != null && !string.IsNullOrEmpty(ret) && ret != "0")
                        {
                            comboCellAcName.Value = int.Parse(ret);
                        }
                    }

                    serialNumber++;
                }

                calculateTotal();
            }
        }

        private bool insertPV()
        {
            if (!chkRequireData())
                return false;
            insertINV(id);
            ////Utilities.LogAudit(-1, "Insert PettyCash JV Voucher", "PettyCash JV Voucher", id, "Inserted PettyCash JV Voucher: " + code);

            return true;
        }

        private bool updatePV()
        {
            if (!chkRequireData())
                return false;

            CommonInsert.DeleteTransactionEntry(id, "PettyCash");

            insertINV(id);
            CommonInsert.DeleteCostCenterTransactionEntry(id.ToString(), "PettyCash");
            ////Utilities.LogAudit(-1, "Update PettyCash JV Voucher", "PettyCash JV Voucher", id, "Updated PettyCash JV Voucher: " + code);
            return true;
        }

        private bool chkRequireData()
        {
            if (txtAmount.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter PettyCash Amount.");
                return false;
            }
            if ((txtAmount.Text == "" || decimal.Parse(txtAmount.Text) == 0) && dgvInv.Rows.Count == 0)
            {
                MessageBox.Show("Enter Amount");
                return false;
            }
            for (int i = 0; i < dgvInv.Rows.Count; i++)
            {
                if (dgvInv.Rows[i].Cells["Debit"].Value == null ||
                    dgvInv.Rows[i].Cells["Debit"].Value.ToString() == "" ||
                    decimal.Parse(dgvInv.Rows[i].Cells["Debit"].Value.ToString()) == 0)
                    continue;
                
                if(dgvInv.Rows[i].Cells["AC_Name"].Value == null)
                {
                    MessageBox.Show("Choose an account");
                    return false;
                }
            }

            return true;
        }
        private void insertINV(int pvId)
        {
            decimal totalCredit = 0; // gross total = amount + vat

            for (int i = 0; i < dgvInv.Rows.Count; i++)
            {
                if (dgvInv.Rows[i].IsNewRow) continue;

                decimal amount = 0;
                decimal vat = 0;

                decimal.TryParse(dgvInv.Rows[i].Cells["amount"].Value?.ToString(), out amount);
                decimal.TryParse(dgvInv.Rows[i].Cells["vat"].Value?.ToString(), out vat);

                if (amount <= 0)
                    continue;

                string costCenterId = dgvInv.Rows[i].Cells["CostCenter"].Value?.ToString() ?? "0";
                string acNameId = dgvInv.Rows[i].Cells["AC_Name"].Value?.ToString() ?? "0";
                string description = dgvInv.Rows[i].Cells["Description"].Value?.ToString() ?? "";
                string humId = dgvInv.Rows[i].Cells["humId"].Value?.ToString() ?? "0";

                // Cost center transaction (usually net expense only)
                CommonInsert.InsertCostCenterTransaction(
                    dtOpen.Value,
                    amount.ToString("0.00"),
                    "0",
                    id.ToString(),
                    "PettyCash",
                    "PettyCash Entry",
                    costCenterId
                );

                // Journal entries per line
                InsertJournals(vat, amount, description, humId, acNameId);

                // Gross total for petty cash credit
                totalCredit += (amount + vat);
            }

            // Credit petty cash with TOTAL PAID (net + VAT)
            InsertJournalsTotal(
                totalCredit.ToString("0.00"),
                "PETTY CASH NO. " + code,
                "0",
                pettyCashAccountId.ToString()
            );

            // update balance
            //ClinicDatabase.ExecuteNonQuery(
            //    "UPDATE tbl_petty_cash_request SET pay = pay + @pay, `change` = amount - @pay WHERE id (select employee_id from tbl_petty_cash perry_cash_name = @id)",
            //    ClinicDatabase.CreateParameterNew("pay", totalCredit),
            //    ClinicDatabase.CreateParameterNew("id", pettyCashId));
            UpdateRequest(totalCredit);
            
        }

        private void UpdateRequest(decimal voucherTotal)
        {
            decimal settle = voucherTotal;

            object empId = ClinicDatabase.ExecuteScalar("select employee_id from tbl_petty_cash where id = @id", ClinicDatabase.CreateParameterNew("id", id));
            if (empId == null) return;

            ClinicDatabase.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS `tbl_petty_cash_settlement` (
                `id` INT NOT NULL AUTO_INCREMENT,
                `request_id` INT NOT NULL,
                `petty_cash_id` INT NOT NULL,
                `settled_amount` DECIMAL(20,2) NOT NULL DEFAULT 0,
                `created_by` INT DEFAULT NULL,
                `created_date` DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (`id`)
            ) ENGINE=InnoDB
            DEFAULT CHARSET=utf8mb4
            COLLATE=utf8mb4_general_ci;
            ");

            using (var rd = ClinicDatabase.ExecuteReader(@"
                    SELECT id, `change`
                    FROM tbl_petty_cash_request
                    WHERE Petty_cash_name=@emp
                    AND `change` > 0
                    ORDER BY request_date,id",
                ClinicDatabase.CreateParameterNew("@emp", empId)))
            {
                while (rd.Read() && settle > 0)
                {
                    int reqId = Convert.ToInt32(rd["id"]);
                    decimal bal = Convert.ToDecimal(rd["change"]);

                    decimal useAmt = Math.Min(settle, bal);

                    // insert mapping
                    ClinicDatabase.ExecuteNonQuery(@"
                        INSERT INTO tbl_petty_cash_settlement
                        (request_id, petty_cash_id, settled_amount)
                        VALUES(@r,@p,@a)",
                    ClinicDatabase.CreateParameterNew("@r", reqId),
                    ClinicDatabase.CreateParameterNew("@p", pettyCashId),
                    ClinicDatabase.CreateParameterNew("@a", useAmt));

                    // update request
                    ClinicDatabase.ExecuteNonQuery(@"
                        UPDATE tbl_petty_cash_request
                        SET pay = pay + @amt,
                            `change` = `change` - @amt,
                            state = CASE
                                      WHEN (`change` - @amt)<=0 THEN 'Closed'
                                      ELSE 'Partial'
                                    END
                        WHERE id=@id",
                    ClinicDatabase.CreateParameterNew("@amt", useAmt),
                    ClinicDatabase.CreateParameterNew("@id", reqId));

                    settle -= useAmt;
                }
            }
        }

        void InsertJournals(decimal vat, decimal amount, string description, string hum_id, string accountId)
        {
            string tType = "Petty Cash";

            // 1) Debit Expense (Net)
            CommonInsert.addTransactionEntry(
                dtOpen.Value.Date,
                accountId,
                amount.ToString("0.00"), "0",
                id.ToString(), hum_id,
                tType, "PettyCash", description,
                -1, DateTime.Now.Date, txtNo.Text
            );

            // 2) Debit VAT Input
            if (vat > 0)
            {
                int vatInputId = frmLogin.defaultAccounts.ContainsKey("Vat Input")
                    ? frmLogin.defaultAccounts["Vat Input"]
                    : 0;

                if (vatInputId <= 0)
                {
                    MessageBox.Show("VAT Input account is not configured in default accounts.");
                    return;
                }

                CommonInsert.addTransactionEntry(
                    dtOpen.Value.Date,
                    vatInputId.ToString(),
                    vat.ToString("0.00"), "0",
                    id.ToString(), hum_id,
                    tType, "PettyCash VAT", description,
                    -1, DateTime.Now.Date, txtNo.Text
                );
            }
        }
        void InsertJournalsTotal(string amount, string description, string hum_id, string accountId)
        {
            string tType = "Petty Cash";

            // Credit Petty Cash with total paid
            CommonInsert.addTransactionEntry(
                dtOpen.Value.Date,
                accountId,
                "0", amount,
                id.ToString(), hum_id,
                tType, "PettyCash", description,
                -1, DateTime.Now.Date, txtNo.Text
            );
        }
        //private void insertINV(int pvId)
        //{
        //    // no, AC_Code, AC_Name, CostCenter, Description, Debit, Credit

        //    for (int i = 0; i < dgvInv.Rows.Count; i++)
        //    {
        //        if (dgvInv.Rows[i].Cells["Debit"].Value == null ||
        //            dgvInv.Rows[i].Cells["Debit"].Value.ToString() == "" ||
        //            decimal.Parse(dgvInv.Rows[i].Cells["Debit"].Value.ToString()) == 0)
        //            continue;

        //        string CostCenterId = dgvInv.Rows[i].Cells["CostCenter"].Value == null ? "0" : dgvInv.Rows[i].Cells["CostCenter"].Value.ToString() ?? "0";
        //        //string acCodeId = dgvInv.Rows[i].Cells["AC_Code"].Value.ToString() ?? "0";
        //        string acNameId = dgvInv.Rows[i].Cells["AC_Name"].Value.ToString() ?? "0";
        //        var description = dgvInv.Rows[i].Cells["Description"].Value?.ToString() ?? "";
        //        var vat = dgvInv.Rows[i].Cells["vat"].Value?.ToString() ?? "0";
        //        var amount = dgvInv.Rows[i].Cells["Debit"].Value?.ToString() ?? "0";
        //        var _humId = dgvInv.Rows[i].Cells["humId"].Value?.ToString() ?? "0";

        //        CommonInsert.InsertCostCenterTransaction(dtOpen.Value, amount.ToString(), "0", id.ToString(), "PettyCash", "PettyCash Entry", CostCenterId.ToString());
        //        //CommonInsert.InsertCostCenterTransaction(dtOpen.Value, "0", amount.ToString(), id.ToString(), "PettyCash", "PettyCash Credit Entry", CostCenterId.ToString());

        //        InsertJournals(vat, amount, description, _humId, acNameId);
        //        //}
        //    }
        //    InsertJournalsTotal(txtAmount.Text, "PETTY CASH NO." + code, "0", pettyCashAccountId.ToString());
        //}

        //void InsertJournals(string vat, string amount, string description, string hum_id, string AccountId)
        //{
        //    string tType = "Petty Cash";

        //    CommonInsert.addTransactionEntry(dtOpen.Value.Date,
        //        AccountId,
        //        amount, "0", id.ToString(), hum_id, tType, "PettyCash", description,
        //         -1, DateTime.Now.Date, txtNo.Text);
        //    if (decimal.Parse(vat) > 0)
        //    {
        //        int level4VatId = frmLogin.defaultAccounts.ContainsKey("Vat Output") ? frmLogin.defaultAccounts["Vat Output"] : 0;
        //        CommonInsert.addTransactionEntry(dtOpen.Value.Date,
        //            level4VatId.ToString(),
        //            vat, "0", id.ToString(), hum_id, tType, "PettyCash VAT", description,
        //             -1, DateTime.Now.Date, txtNo.Text);
        //    }
        //}

        //void InsertJournalsTotal(string amount, string description, string hum_id, string AccountId)
        //{
        //    string tType = "Petty Cash";
        //    CommonInsert.addTransactionEntry(dtOpen.Value.Date,
        //                          AccountId,
        //                       "0", amount, id.ToString(), hum_id, tType, "PettyCash", description,
        //                           -1, DateTime.Now.Date, txtNo.Text);
        //}

        private void SetDefault()
        {
            //using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "Default Account For Cash")))
            //    if (reader.Read())
            //    {
            //        txtAccountCode.Text = reader["code"].ToString();
            //        string accountId = reader["id"].ToString();
            //        if (!string.IsNullOrEmpty(accountId))
            //        {
            //            cmbAccountName.SelectedValue = int.Parse(accountId);
            //        }
            //    }
        }

        private void dgvInv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInv.Rows.Count <= 0)
                return;
            if (dgvInv.CurrentRow == null)
                return;

            calculateTotal();
        }
        private void calculateTotal()
        {
            for (int i = 0; i < dgvInv.Rows.Count; i++)
            {
                if (dgvInv.Rows[i].Cells["Debit"].Value != null)
                {
                    if (dgvInv.Rows[i].Cells["Debit"].Value.ToString() == "")
                        continue;
                    totalAmount += decimal.Parse(dgvInv.Rows[i].Cells["Debit"].Value.ToString());
                }
            }
            txtAmount.Text = totalAmount.ToString();
            totalAmount = 0;
        }
        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if (txtAmount.Text == "")
            {
                txtAmountInWord.Text = "";
                return;
            }
            txtAmountInWord.Text = AmountToWordsHelper.ConvertToSelectedLanguage(decimal.Parse(txtAmount.Text));
        }

        private void dgvInv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvInv.Columns[e.ColumnIndex].Name == "Navigate")
            {
                string accountCode = id.ToString();
                string description = dgvInv.Rows[e.RowIndex].Cells["Description"].Value?.ToString();

                using (TransactionsDialog dlg = new TransactionsDialog(accountCode, description))
                {
                    dlg.ShowDialog();
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (isOldBill)
            {
                if (updatePV())
                {
                    ClinicDatabase.ExecuteNonQuery("Update tbl_petty_cash set status=1 where id=@id", ClinicDatabase.CreateParameterNew("@id", id));
                    //.RefreshPettyCashVoucher();
                    MessageBox.Show("The PettyCash Journal Updated !");

                    dgvInv.Rows.Clear();
                    this.Close();
                }
            }
            else
            {
                if (insertPV())
                {
                    ClinicDatabase.ExecuteNonQuery("Update tbl_petty_cash set status=1 where id=@id", ClinicDatabase.CreateParameterNew("@id", id));
                    //.RefreshPettyCashVoucher();
                    MessageBox.Show("The PettyCash Journal Created");

                    dgvInv.Rows.Clear();
                    this.Close();
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal; // Restore down
            }
            else
            {
                this.WindowState = FormWindowState.Maximized; // Maximize
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

    }
}

public class TransactionsDialog : Form
{
    private Guna2DataGridView dgvInv;
    private string _accountCode;
    private string _description;

    public TransactionsDialog(string accountCode = "0", string description = "")
    {
        _accountCode = accountCode;
        _description = description;
        InitializeComponents();
        LoadTransactionData();
    }

    private void InitializeComponents()
    {
        this.Text = "Transaction Details";
        this.Size = new System.Drawing.Size(800, 450);
        this.StartPosition = FormStartPosition.CenterParent;

        dgvInv = new Guna2DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true
        };

        this.Controls.Add(dgvInv);
    }

    private void LoadTransactionData()
    {

        string query = @"
                SELECT
                    t.transaction_id AS id,
                    a.name AS name,
                    t.debit AS debit,
                    0.00 AS credit
                FROM tbl_transaction t
                INNER JOIN tbl_petty_cash_details pd 
                    ON t.transaction_id = pd.petty_cash_id
                    AND t.description COLLATE utf8mb4_general_ci = pd.description COLLATE utf8mb4_general_ci
                INNER JOIN tbl_coa_level_4 a ON t.account_id = a.id
                WHERE 
                    t.type = 'Petty Cash'
                    AND pd.petty_cash_id = @PettyCashId
                    AND pd.description = @Description COLLATE utf8mb4_general_ci

                UNION ALL

                SELECT
                    t.transaction_id AS id,
                    a.name AS name,
                    0.00 AS debit,
                    pd.amount AS credit
                FROM tbl_transaction t
                INNER JOIN tbl_petty_cash_details pd 
                    ON t.transaction_id = pd.petty_cash_id
                    AND t.credit > 0
                INNER JOIN tbl_coa_level_4 a ON t.account_id = a.id
                WHERE 
                    t.type = 'Petty Cash'
                    AND pd.petty_cash_id = @PettyCashId
                    AND pd.description = @Description COLLATE utf8mb4_general_ci

                ORDER BY id, name;";

        DataTable dt = ClinicDatabase.ExecuteDataTable(query,
            ClinicDatabase.CreateParameterNew("@PettyCashId", _accountCode),
            ClinicDatabase.CreateParameterNew("@Description", _description));

        dgvInv.DataSource = dt;

        // Optional formatting
        dgvInv.Columns["id"].HeaderText = "ID";
        dgvInv.Columns["name"].HeaderText = "Account Name";
        dgvInv.Columns["debit"].DefaultCellStyle.Format = "N2";
        dgvInv.Columns["credit"].DefaultCellStyle.Format = "N2";
    }
}
