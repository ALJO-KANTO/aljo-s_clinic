using Clinic_DataAccess;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewJournalVoucher : Form
    {
        int id;

        public frmViewJournalVoucher(int id)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            //AppTheme.DefaultFont(dgvJV);
            this.id = id;
            headerUC1.FormText = id == 0 ? "New Journal Voucher" : "Edit Journal Voucher";
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmViewJournalVoucher_Load(object sender, EventArgs e)
        {
            dtJV.Value = DateTime.Now;
            LoadGVAccounts();
            //BindCombos.PopulateAllLevel4Account(cmbCostCenter);
            //BindCombos.PopulateEmployees(cmbEmployee);
            //cmbCostCenter.SelectedIndex = -1;
            txt_jv_code.Text = GenerateNextJournalCode();
            txtJvId.Text = GenerateNextJournalId();
            LoadComboAccount();

            if (id != 0)
                BindJournal();

        }
        private string GenerateNextJournalId()
        {
            string newCode = "1";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(id) AS lastCode FROM tbl_journal_voucher"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = code.ToString();
                }
            }

            return newCode;
        }
        private string GenerateNextJournalCode()
        {
            string newCode = "JV-0001";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(SUBSTRING(code, 4) AS UNSIGNED)) AS lastCode FROM tbl_journal_voucher"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = "JV-" + code.ToString("D4");
                }
            }

            return newCode;
        }

        private void LoadComboAccount()
        {
            //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CAST(code as CHAR) code,name FROM tbl_coa_level_4;");
            DataGridViewComboBoxColumn account = (DataGridViewComboBoxColumn)dgvJV.Columns["account_name"];
            BindCombos.PopulateLevel4Account(account,false);
            //account.DataSource = dt;
            //account.DisplayMember = "name";
            //account.ValueMember = "code";
        }
        private void LoadGVAccounts()
        {
            dgvJV.Rows.Clear();
           

            if (dgvJV.CurrentRow != null)
            {
                DataGridViewCell buttonColumn = dgvJV.CurrentRow.Cells["partner_name"];
                if (buttonColumn.Value == null || buttonColumn.Value.ToString() == "")
                {
                    buttonColumn.Value = "▼";
                    //dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                loadNameList(buttonColumn);
            }
        }
        private void loadNameList(DataGridViewCell buttonColumn)
        {
            string param = "";
            string query = @"SELECT id,CONCAT(CODE ,' - ' , Name) as NAME FROM tbl_employee
                             UNION ALL
                             SELECT id,CONCAT(CODE ,' - ' , NAME) as NAME FROM tbl_vendor
                             UNION ALL
                             SELECT id,CONCAT(CODE,' - ', NAME) as NAME FROM tbl_customer";

            if(dgvJV.CurrentRow != null && dgvJV.CurrentRow.Cells["account_name"].Value != null)
            {
                string accountName = dgvJV.CurrentRow.Cells["account_name"].Value.ToString();
                DataGridViewComboBoxCell comboCell = dgvJV.CurrentRow.Cells["account_name"] as DataGridViewComboBoxCell;

                if (comboCell != null && comboCell.Value != null)
                {
                    string accountId = comboCell.Value.ToString();
                    accountName = comboCell.FormattedValue.ToString();
                }
                if (accountName != "▼")
                {
                    param = string.IsNullOrEmpty(accountName) ? "" : accountName;
                    query = @"SELECT id,CONCAT(CODE ,' - ' , NAME) as NAME FROM tbl_vendor WHERE account_id = (SELECT id FROM tbl_coa_level_4 WHERE NAME = @accountName)
                            UNION ALL
                            SELECT id,CONCAT(CODE,' - ', NAME) as NAME FROM tbl_customer WHERE account_id = (SELECT id FROM tbl_coa_level_4 WHERE NAME = @accountName)
                            UNION ALL
                            SELECT id,CONCAT(CODE ,' - ' , Name) as NAME FROM tbl_employee WHERE account_id = (SELECT id FROM tbl_coa_level_4 WHERE NAME = @accountName)";
                }
            }

            if (buttonColumn.Value == null || buttonColumn.Value.ToString() == "")
            {
                buttonColumn.Value = "▼";
                //dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else if (buttonColumn.Value.ToString() == "▼")
            {
                //dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            loadDataToListView(query, param);
        }
        private void loadDataToListView(string query,string arguments)
        {
            nameslistView.Visible = true;
            nameslistView.BringToFront();

            // Optional: Position it near the clicked cell
            if (dgvJV.CurrentCell != null)
            {
                Rectangle cellRect = dgvJV.GetCellDisplayRectangle(dgvJV.CurrentCell.ColumnIndex, dgvJV.CurrentCell.RowIndex, true);
                Point cellLocation = dgvJV.PointToScreen(cellRect.Location);
                Point listLocation = this.PointToClient(cellLocation);

                nameslistView.SetBounds(listLocation.X, listLocation.Y + dgvJV.CurrentCell.Size.Height, 250, 120);
            }
            try
            {
                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@accountName", arguments));

                nameslistView.Clear();
                nameslistView.View = System.Windows.Forms.View.Details;
                nameslistView.FullRowSelect = true;
                nameslistView.Columns.Add(" Name", 250);

                if (dt.Rows.Count == 0)
                {
                    nameslistView.Visible = false;
                    nameslistView.SendToBack();
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    ListViewItem item = new ListViewItem(row["name"].ToString());
                    item.Font = new System.Drawing.Font("Segoe UI", 9F);
                    item.SubItems.Add(row["id"].ToString());
                    nameslistView.Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        private void BindJournal()
        {
            dgvJV.Rows.Clear();
            using (MySqlDataReader dr = ClinicDatabase.ExecuteReader(@"select * from tbl_journal_voucher WHERE id=@id", ClinicDatabase.CreateParameterNew("id", id)))
                if (dr.Read())
                {
                    txtJvId.Text = dr["id"].ToString();
                    dtJV.Value = DateTime.Parse(dr["date"].ToString());
                    txt_jv_code.Text = dr["code"].ToString();
                    txtDebitAmount.Text = dr["debit"].ToString();
                    txtCreditAmount.Text = dr["credit"].ToString();

                    int count = 1;
                    using (MySqlDataReader dr_details = ClinicDatabase.ExecuteReader(@"select j.date as Date,j.account_id ,ac.code as 'Account Code',ac.name as 'Account Name', j.debit as Debit, 
                                                        j.credit as Credit ,j.Description as Description,partner from tbl_journal_voucher_details j inner join tbl_coa_level_4 ac on
                                                        j.account_id = ac.id WHERE j.inv_id=@id", ClinicDatabase.CreateParameterNew("id", id)))
                        while (dr_details.Read())
                        {
                            dgvJV.Rows.Add(dr_details["account_id"].ToString(), count.ToString(), dr_details["Account Code"].ToString(), dr_details["Account Code"].ToString(), Utilities.FormatDecimal(dr_details["Debit"]), Utilities.FormatDecimal(dr_details["Credit"]), dr_details["Description"].ToString(), dr_details["partner"].ToString());
                            count++;
                        }
                    //dgvJV.Columns["Date"].Width = 150;
                    //dgvJV.Columns["Account Code"].Width = 150;
                    //dgvJV.Columns["Account Name"].Width = 400;
                    //dgvJV.Columns["Debit"].Width = 200;
                    //dgvJV.Columns["Credit"].Width = 200;
                    //dgvJV.Columns["partner_name"].Width = 550;
                }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (validateDebitAndCreditAmount())
            {
                if (id != 0)
                {
                    if (chkRequiredDate())
                    {
                        updateJV();
                        //.RefreshJournal();
                        loadPrint();
                    }
                }
                else
                {
                    if (chkRequiredDate())
                    {
                        insertJV();
                        //.RefreshJournal();
                        loadPrint();
                    }
                }
            }
            else
            {
                MessageBox.Show("Debit Total and Credit Total are not equal.");
            }
        }
        private void loadPrint()
        {
            DialogResult result = MessageBox.Show("Do You want To Show This Voucher ",
                                            "Confirmation",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question);
            // Check if the user clicked Yes or No
            if (result == DialogResult.Yes)
            {
                // Code for when the user clicks "Yes"

                //frmMain.openChildForm(new frmReport());
                //ShowReport();

            }
            else if (result == DialogResult.No)
            {
                // Code for when the user clicks "No"
                this.Close();
            }
        }
        private bool chkRequiredDate()
        {
            for (int i = 0; i < dgvJV.Rows.Count - 1; i++)
            {
                if (dgvJV.Rows[i].Cells["account_name"].Value == null
                    || dgvJV.Rows[i].Cells["account_name"].Value.ToString() == ""
                    || decimal.Parse(dgvJV.Rows[i].Cells["account_name"].Value.ToString()) == 0)
                {
                    MessageBox.Show("Total Item In Row " + (dgvJV.Rows[i].Index + 1) + " Can't Be 0 or Null");
                    return false;
                }
            }
            return true;
        }
        private bool updateJV()
        {
            invCode = GenerateNextJournalCode();
            string GlobalId = ClinicDatabase.ExecuteScalar("select GlobalId from tbl_journal_voucher where id=@id", ClinicDatabase.CreateParameterNew("id", id)).ToString();

            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_journal_voucher SET date=@date, code=@code, debit=@debit, credit=@credit, modified_by=@modified_by, modified_date=@modified_date,LastModified=@LastModified,SyncStatus = 0 WHERE id=@id",
                                                      ClinicDatabase.CreateParameterNew("id", id),
                                                      ClinicDatabase.CreateParameterNew("date", dtJV.Value.Date),
                                                      ClinicDatabase.CreateParameterNew("code", invCode),
                                                      ClinicDatabase.CreateParameterNew("debit", txtDebitAmount.Text.ToString()),
                                                      ClinicDatabase.CreateParameterNew("credit", txtCreditAmount.Text.ToString()),
                                                      ClinicDatabase.CreateParameterNew("modified_by", -1),
                                                      ClinicDatabase.CreateParameterNew("LastModified", DateTime.Now.Date),
                                                      ClinicDatabase.CreateParameterNew("modified_date", DateTime.Now.Date));
            ClinicDatabase.ExecuteNonQuery(@"DELETE from tbl_journal_voucher_details where inv_id=@id;", ClinicDatabase.CreateParameterNew("id", id.ToString()));
            CommonInsert.DeleteTransactionEntry(id, "JOURNAL");
            insertJvDetails(id.ToString(), GlobalId);
            ////Utilities.LogAudit(-1, "Update Journal Voucher", "Journal Voucher", id, "Updated Journal Voucher: " + invCode);
            if (!string.IsNullOrEmpty(filePath))
            {
                AddFileData(id);
            }
            return true;
        }
        private bool insertJV()
        {
            invCode = GenerateNextReceiptCode();
            string globalId = Guid.NewGuid().ToString();

            id = (int)decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_journal_voucher(
                                                          date, code, debit,credit, created_by, created_date, state, GlobalId, DeviceId) 
                                                          VALUES (@date, @code,@debit,@credit, @created_by, @created_date, 0, @GlobalId, @DeviceId);SELECT LAST_INSERT_ID();",
                                                          ClinicDatabase.CreateParameterNew("date", dtJV.Value.Date),
                                                          ClinicDatabase.CreateParameterNew("code", invCode),
                                                          ClinicDatabase.CreateParameterNew("debit", txtDebitAmount.Text.ToString()),
                                                          ClinicDatabase.CreateParameterNew("credit", txtCreditAmount.Text.ToString()),
                                                          ClinicDatabase.CreateParameterNew("created_by", -1),
                                                          ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date),
                ClinicDatabase.CreateParameterNew("GlobalId", globalId),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName),
                                                          ClinicDatabase.CreateParameterNew("state", 0)).ToString());

            insertJvDetails(id.ToString(), globalId);
            ////Utilities.LogAudit(-1, "Create Journal Voucher", "Journal Voucher", id, "Created Journal Voucher: " + invCode);
            if (!string.IsNullOrEmpty(filePath))
            {
                AddFileData(id);
            }
            return true;
        }
        private void insertJvDetails(string inv_id, string globalId)
        {
            for (int i = 0; i < dgvJV.Rows.Count; i++)
            {
                if (dgvJV.Rows[i].Cells["account_id"].Value == null || dgvJV.Rows[i].Cells["account_id"].Value.ToString() == "")
                {
                    //
                }
                else
                {
                    if (dgvJV.Rows[i].Cells["Debit"].Value == null || dgvJV.Rows[i].Cells["Debit"].Value.ToString() == "" || dgvJV.Rows[i].Cells["Credit"].Value == null || dgvJV.Rows[i].Cells["Credit"].Value.ToString() == "")
                        continue;
                    decimal DebitAmount = dgvJV.Rows[i].Cells["Debit"].Value == DBNull.Value ? 0 : Convert.ToDecimal(dgvJV.Rows[i].Cells["Debit"].Value);
                    decimal CreditAmount = dgvJV.Rows[i].Cells["Credit"].Value == DBNull.Value ? 0 : Convert.ToDecimal(dgvJV.Rows[i].Cells["Credit"].Value);
                    string accountId = dgvJV.Rows[i].Cells["account_id"].Value.ToString();
                    string description = dgvJV.Rows[i].Cells["description"].Value?.ToString() ?? "";
                    string partner = dgvJV.Rows[i].Cells["partner_name"].Value?.ToString() ?? "";
                    DateTime date = DateTime.Parse(dtJV.Value.Date.ToString());
                    string hum_id = "0";

                    ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_journal_voucher_details
                                        (date,debit,credit, inv_id, description, account_id,partner, GlobalId, DeviceId) 
                                        VALUES (@date,@debit,@credit, @inv_id, @description, @account_id, @partner, @GlobalId, @DeviceId);",
                        ClinicDatabase.CreateParameterNew("@date", date),
                        ClinicDatabase.CreateParameterNew("@debit", DebitAmount.ToString()),
                        ClinicDatabase.CreateParameterNew("@credit", CreditAmount.ToString()),
                        ClinicDatabase.CreateParameterNew("@inv_id", inv_id),
                        ClinicDatabase.CreateParameterNew("@description", description),
                        ClinicDatabase.CreateParameterNew("@partner", partner),
                        ClinicDatabase.CreateParameterNew("@account_id", accountId),
                        ClinicDatabase.CreateParameterNew("GlobalId", globalId),
                        ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName));

                    CommonInsert.addTransactionEntry(date,
                           accountId,
                           DebitAmount.ToString(), CreditAmount.ToString(), inv_id, hum_id, "JOURNAL VOUCHER", "JOURNAL", "Journal Voucher NO. " + invCode,
                            -1, DateTime.Now.Date,txt_jv_code.Text);
                }
            }
        }
        string invCode;
        private string GenerateNextReceiptCode()
        {
            string newCode = "JV-0001";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(SUBSTRING(code, 4) AS UNSIGNED)) AS lastCode FROM tbl_journal_voucher"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = "JV-" + code.ToString("D4");
                }
            }

            txt_jv_code.Text = newCode.ToString();

            return newCode;
        }

        private void btnSaveAndNew_Click(object sender, EventArgs e)
        {
            if (validateDebitAndCreditAmount())
            {
                if (id != 0)
                {
                    if (chkRequiredDate())
                        updateJV();
                }
                else
                {
                    if (chkRequiredDate())
                        insertJV();
                }
            }
            else
            {
                MessageBox.Show("Debit Total and Credit Total are not equal.");
            }

        }

        private void calculateTotal()
        {
            decimal totalDebit = 0, totalCredit = 0;
            for (int i = 0; i < dgvJV.Rows.Count; i++)
            {
                if (dgvJV.Columns.Contains("Debit") && dgvJV.Columns.Contains("Credit"))
                {
                    var debitColumnIndex = dgvJV.Columns["Debit"].Index;
                    var creditColumnIndex = dgvJV.Columns["Credit"].Index;
                    var debitCellValue = dgvJV.Rows[i].Cells[debitColumnIndex].Value;
                    var creditCellValue = dgvJV.Rows[i].Cells[creditColumnIndex].Value;
                    if (debitCellValue == DBNull.Value || debitCellValue == null || string.IsNullOrEmpty(debitCellValue.ToString().Trim()) || creditCellValue == DBNull.Value || creditCellValue == null || string.IsNullOrEmpty(creditCellValue.ToString().Trim()))
                    {
                        //
                    }
                    else
                    {

                        if (dgvJV.Rows[i].Cells["Debit"].Value.ToString() != "" || dgvJV.CurrentRow.Cells["Credit"].Value.ToString() != "")
                        {
                            totalDebit += decimal.Parse(dgvJV.Rows[i].Cells["Debit"].Value.ToString());
                            totalCredit += decimal.Parse(dgvJV.Rows[i].Cells["Credit"].Value.ToString());
                        }
                    }
                }
            }
            txtDebitAmount.Text = totalDebit.ToString();
            txtCreditAmount.Text = totalCredit.ToString();
            validateDebitAndCreditAmount();
        }
        private bool validateDebitAndCreditAmount()
        {
            if (!string.IsNullOrEmpty(txtDebitAmount.Text) && !string.IsNullOrEmpty(txtCreditAmount.Text))
            {
                decimal dr = decimal.Parse(txtDebitAmount.Text);
                decimal cr = decimal.Parse(txtCreditAmount.Text);
                if (dr == 0 && cr == 0)
                {
                    return false;
                }
                if (dr == cr)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private void dgvJV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvJV.Rows.Count > 1)
            {
                var row = dgvJV.Rows[e.RowIndex];
                if (e.ColumnIndex == dgvJV.Columns["account_code"].Index)
                {
                    string codeValue = row.Cells["account_code"].Value?.ToString();
                    DataGridViewComboBoxCell comboCell = row.Cells["account_name"] as DataGridViewComboBoxCell;
                    if (comboCell != null)
                        insertAccountThroughCodeOrCombo("account_code", comboCell, null);
                }
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == dgvJV.Columns["Debit"].Index || e.ColumnIndex == dgvJV.Columns["Credit"].Index)
                    {
                        var debitColumnIndex = dgvJV.Columns["Debit"].Index;
                        var creditColumnIndex = dgvJV.Columns["Credit"].Index;
                        var debitCellValue = dgvJV.Rows[e.RowIndex].Cells[debitColumnIndex].Value;
                        var creditCellValue = dgvJV.Rows[e.RowIndex].Cells[creditColumnIndex].Value;


                        if (debitCellValue == DBNull.Value || debitCellValue == null || string.IsNullOrEmpty(debitCellValue.ToString().Trim()))
                        {
                            dgvJV.Rows[e.RowIndex].Cells[debitColumnIndex].Value = "0";
                        }
                        if (creditCellValue == DBNull.Value || creditCellValue == null || string.IsNullOrEmpty(creditCellValue.ToString().Trim()))
                        {
                            dgvJV.Rows[e.RowIndex].Cells[creditColumnIndex].Value = "0";
                        }
                        var debitValue = Convert.ToDecimal(dgvJV.Rows[e.RowIndex].Cells[debitColumnIndex].Value ?? 0);
                        var creditValue = Convert.ToDecimal(dgvJV.Rows[e.RowIndex].Cells[creditColumnIndex].Value ?? 0);

                        if (debitValue > 0 && creditValue > 0)
                        {
                            MessageBox.Show("You cannot enter a value in both Debit and Credit columns.");
                            dgvJV.Rows[e.RowIndex].Cells[creditColumnIndex].Value = DBNull.Value;
                        }
                        else if (creditValue > 0 && debitValue > 0)
                        {
                            MessageBox.Show("You cannot enter a value in both Debit and Credit columns.");
                            dgvJV.Rows[e.RowIndex].Cells[debitColumnIndex].Value = DBNull.Value;
                        }
                        calculateTotal();
                    }
                }
            }
        }
        private void dgvJV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox comboBox = e.Control as ComboBox;
            if (dgvJV.CurrentCell.ColumnIndex == dgvJV.Columns["account_name"].Index)
            {
                if (e.Control is ComboBox combo)
                {
                    combo.DropDownStyle = ComboBoxStyle.DropDown;
                    combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    combo.AutoCompleteSource = AutoCompleteSource.ListItems;

                    combo.Text = "";
                }

                if (comboBox != null)
                {
                    comboBox.SelectedIndexChanged -= new EventHandler(ComboBoxName_SelectedIndexChanged);
                    comboBox.SelectedIndexChanged += new EventHandler(ComboBoxName_SelectedIndexChanged);
                }
            }
            else if (dgvJV.CurrentCell.ColumnIndex == dgvJV.Columns["Debit"].Index)
            {
                var debitColumnIndex = dgvJV.Columns["Debit"].Index;

                if (dgvJV.CurrentCell.ColumnIndex == debitColumnIndex)
                {
                    e.Control.KeyPress -= new KeyPressEventHandler(ValidateDebitInput);
                    e.Control.KeyPress += new KeyPressEventHandler(ValidateDebitInput);
                }
            }
            else if (dgvJV.CurrentCell.ColumnIndex == dgvJV.Columns["Credit"].Index)
            {
                var creditColumnIndex = dgvJV.Columns["Credit"].Index;
                if (dgvJV.CurrentCell.ColumnIndex == creditColumnIndex)
                {
                    e.Control.KeyPress -= new KeyPressEventHandler(ValidateCreditInput);
                    e.Control.KeyPress += new KeyPressEventHandler(ValidateCreditInput);
                }
            }
            if (dgvJV.CurrentCell.ColumnIndex == dgvJV.Columns["account_code"].Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.TextChanged -= AccountCodeTextBox_TextChanged;
                    tb.TextChanged += AccountCodeTextBox_TextChanged;
                }
            }
            else
            {
                // Ensure we remove suggestion logic from non-account_code columns
                if (e.Control is TextBox tb)
                {
                    tb.TextChanged -= AccountCodeTextBox_TextChanged;
                }

                // Hide suggestion box just in case
                lstAccountSuggestions.Visible = false;

                e.Control.KeyPress -= new KeyPressEventHandler(ValidateText);
                e.Control.KeyPress += new KeyPressEventHandler(ValidateText);
            }
        }
        private void AccountCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;

            string input = tb.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                lstAccountSuggestions.Visible = false;
                return;
            }

            string query = @"SELECT code, name FROM tbl_coa_level_4 
                     WHERE code LIKE @search OR name LIKE @search LIMIT 20";
            DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

            lstAccountSuggestions.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                lstAccountSuggestions.Items.Add(new ItemSuggestion
                {
                    Code = row["code"].ToString(),
                    Name = row["name"].ToString()
                });
            }

            if (lstAccountSuggestions.Items.Count > 0)
            {
                lstAccountSuggestions.Visible = true;
                Rectangle cellRect = dgvJV.GetCellDisplayRectangle(
                    dgvJV.CurrentCell.ColumnIndex, dgvJV.CurrentCell.RowIndex, true);

                lstAccountSuggestions.SetBounds(
                    dgvJV.Left + cellRect.Left,
                    dgvJV.Top + cellRect.Bottom,
                    cellRect.Width + 80,
                    120);
                lstAccountSuggestions.BringToFront();
            }
            else
            {
                lstAccountSuggestions.Visible = false;
            }
        }

        private void lstAccountSuggestions_Click(object sender, EventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem != null)
            {
                //string selected = lstAccountSuggestions.SelectedItem.ToString();
                //string code = selected.Split('-')[0].Trim();
                if (lstAccountSuggestions.SelectedItem is ItemSuggestion item)
                {
                    string code = item.Code;
                    dgvJV.CurrentCell.Value = code;
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void dgvJV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem != null)
            {
                string selected = lstAccountSuggestions.SelectedItem.ToString();
                string code = selected.Split('-')[0].Trim();

                //dgvJV.CurrentCell.Value = code;
                //lstAccountSuggestions.Visible = false;
            }
        }
        private void lstAccountSuggestions_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dgvJV_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvJV.CurrentCell == null) return;

            // Hide suggestions when not editing account_code column
            if (dgvJV.CurrentCell.OwningColumn.Name != "account_code")
            {
                lstAccountSuggestions.Visible = false;
            }
        }

        private void lstAccountSuggestions_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem == null) return;

            string selected = lstAccountSuggestions.SelectedItem.ToString();
            string code = selected.Split('-')[0].Trim();

            // Set value to the current cell
            dgvJV.CurrentCell.Value = code;

            // Hide the suggestion list
            lstAccountSuggestions.Visible = false;

            // Re-focus DataGridView if needed
            dgvJV.Focus();
        }

        int comboCount = 0;
        private void ComboBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedValue != null)
            {
                if (comboBox.Text == "<< Add >>")
                {
                    if (comboCount == 0)
                    {
                        comboCount++;
                        new frmAddAccount().ShowDialog();
                        DataGridViewComboBoxColumn account = (DataGridViewComboBoxColumn)dgvJV.Columns["account_name"];
                        BindCombos.PopulateLevel4Account(account, true, true);
                        account.AutoComplete = true;
                        comboBox.SelectedIndex = comboBox.Items.Count - 1;
                    }
                    else
                        comboCount = 0;
                }
                else
                    insertAccountThroughCodeOrCombo("combo", null, comboBox);
            }
        }
        private void ValidateText(object sender, KeyPressEventArgs e)
        {
            e.Handled = false;
        }

        private void ValidateDebitInput(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void ValidateCreditInput(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }
        private void insertAccountThroughCodeOrCombo(string type, DataGridViewComboBoxCell comboCell, ComboBox comboBox)
        {
            MySqlDataReader reader = null;
            try
            {
                if (type == "account_code")
                {
                    if (dgvJV.CurrentRow.Cells["account_code"].Value != null)
                    {
                        reader = ClinicDatabase.ExecuteReader(@"SELECT id,CAST(code as CHAR) code
                  FROM tbl_coa_level_4 
                  WHERE code = @code ",
                            ClinicDatabase.CreateParameterNew("code", dgvJV.CurrentRow.Cells["account_code"].Value.ToString()));
                    }
                }
                else if (type == "combo" && comboBox.SelectedValue != null)
                {
                    string selectedAccountCode = comboBox.SelectedValue.ToString();
                    reader = ClinicDatabase.ExecuteReader(@"SELECT id,CAST(code as CHAR) code FROM tbl_coa_level_4 WHERE code = @code",
                        ClinicDatabase.CreateParameterNew("code", selectedAccountCode));
                }

                if (reader != null && reader.Read())
                {
                    dgvJV.CurrentRow.Cells["account_code"].Value = reader["code"].ToString();
                    dgvJV.CurrentRow.Cells["account_id"].Value = reader["id"].ToString();
                    if (type == "account_code" && comboCell != null)
                        comboCell.Value = dgvJV.CurrentRow.Cells["account_code"].Value.ToString();
                }
            }
            finally
            {
                if (reader != null)
                {
                    //reader.Close();
                    //reader.Dispose();
                }
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtJvId.Text); // Utilities.GetVoucherIdFromCode(txtJvId.Text);
            if (currentId == null || currentId <= 1)
                return;

            currentId = currentId - 1;
            if (currentId <= 0)
            {
                clearAll();
                MessageBox.Show("No previous records.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string query = ("select id from tbl_journal_voucher where state = 0 and id =@id");
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("@id", currentId)))
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindJournal();
                }
                else
                {
                    clearAll();
                    MessageBox.Show("No previous record found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtJvId.Text); // Utilities.GetVoucherIdFromCode(txtJvId.Text);
            if (currentId is null) return;

            currentId = currentId + 1;
            string query = "SELECT id FROM tbl_journal_voucher WHERE state = 0 AND id =@id";

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("@id", currentId)))
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindJournal();
                }
                else
                {
                    clearAll();
                    MessageBox.Show("No next record found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        private void dgvJV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvJV.Rows.Count > 1 && dgvJV.CurrentRow.Cells["delete"].ColumnIndex == e.ColumnIndex)
            {
                try
                {
                    dgvJV.Rows.Remove(dgvJV.CurrentRow);
                    calculateTotal();
                }catch(Exception ex)
                {
                    ex.ToString();
                }
            }
            if (dgvJV.Rows.Count > 1 && dgvJV.CurrentRow.Cells["partner_name"].ColumnIndex == e.ColumnIndex)
            {
                nameslistView.Visible = true;
                nameslistView.BringToFront();
                DataGridViewCell buttonColumn = dgvJV.CurrentRow.Cells["partner_name"];
                if (buttonColumn.Value == null || buttonColumn.Value.ToString() == "")
                {
                    buttonColumn.Value = "▼";
                    //dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                loadNameList(buttonColumn);

                Rectangle cellRect = dgvJV.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                Point screenPoint = dgvJV.PointToScreen(new Point(cellRect.X, cellRect.Y + cellRect.Height - 160));
                Point formPoint = this.PointToClient(screenPoint);

                nameslistView.Location = formPoint;
                nameslistView.Width = 400;
                nameslistView.Height = 180;
                nameslistView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

           else if (dgvJV.CurrentCell.OwningColumn.Name == "delete")
            {
                RemoveSelectedRows();
            }

        }


        private void RemoveSelectedRows()
        {
            // Loop through the selected rows in reverse order (to avoid index issues)
            foreach (DataGridViewRow row in dgvJV.SelectedRows)
            {
                // Remove the selected row
                dgvJV.Rows.Remove(row);
            }
        }

        private void dgvJV_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvJV.Rows[e.RowIndex].Cells[1].Value = (e.RowIndex + 1).ToString();
        }

        private void cmbNameType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridViewCell buttonColumn = dgvJV.CurrentRow.Cells["partner_name"];
            if (buttonColumn.Value == null)
            {
                buttonColumn.Value = "▼";
                //dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void nameslistView_Click(object sender, EventArgs e)
        {
            if (nameslistView.SelectedItems.Count > 0 && dgvJV.CurrentCell != null)
            {
                string accountName = nameslistView.SelectedItems[0].SubItems[0].Text;
                //if (accountName == "<< Add New >>")
                //{
                //    //
                //}

                nameslistView.Visible = false;

                DataGridViewCell comboCell = dgvJV.CurrentRow.Cells["partner_name"];
                if (comboCell.Value != null)
                {
                    comboCell.Value = accountName;
                }
                if (comboCell.Value != null && comboCell.Value.ToString() == "▼")
                {
                    //dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else
                {
                    dgvJV.Columns["partner_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }
        public DataTable COMPANYINFO(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT * FROM tbl_company ;", ClinicDatabase.CreateParameterNew("@id", a1));
        }
        public DataTable JournalDetails(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT ROW_NUMBER() OVER (ORDER BY id) AS sn,(SELECT CODE FROM tbl_coa_level_4 WHERE id=account_id) code,
                                             (SELECT name FROM tbl_coa_level_4 WHERE id=account_id) name,date,description,debit,credit,partner,'' costCenter 
                                            FROM tbl_journal_voucher_details WHERE inv_id=@journalid;", ClinicDatabase.CreateParameterNew("@journalid", a1));
        }

        private void guna2TileButton23_Click(object sender, EventArgs e)
        {
            clearAll();
        }

        private void clearAll()
        {
            id = 0;
            dtJV.Value = DateTime.Now;
            dgvJV.Rows.Clear();
            txtCreditAmount.Text = "";
            txtDebitAmount.Text = "";
            txt_jv_code.Text = GenerateNextJournalCode();
        }

        private void guna2TileButton22_Click(object sender, EventArgs e)
        {

        }

        private void guna2TileButton21_Click(object sender, EventArgs e)
        {
            //if (dgvJV.Rows.Count == 0) return; 
            //try { 
            //    ClinicDatabase.ExecuteNonQuery("UPDATE tbl_journal_voucher SET state = -1 WHERE id = @id; UPDATE tbl_transaction SET state= -1 WHERE transaction_id=@id AND t_type = 'JOURNAL';", ClinicDatabase.CreateParameterNew("id", id)); 
            //    ////Utilities.LogAudit(-1, "Delete Journal Voucher", "Journal Voucher", id, "Deleted Journal Voucher: " + txt_jv_code.Text); 
            //} catch (Exception ex) { ex.ToString(); }
            if (dgvJV.Rows.Count == 0)
                return;

            try
            {
                // 1. Fetch data before deletion
                DataTable dtJournal = ClinicDatabase.ExecuteDataTable(
                    "SELECT * FROM tbl_journal_voucher WHERE id = @id",
                    ClinicDatabase.CreateParameterNew("id", id)
                );

                DataTable dtTransaction = ClinicDatabase.ExecuteDataTable(
                    "SELECT * FROM tbl_transaction WHERE transaction_id = @id AND t_type = 'JOURNAL'",
                    ClinicDatabase.CreateParameterNew("id", id)
                );

                // 2. Backup function
                void BackupData(DataTable dt, string tableName)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ClinicDatabase.ExecuteNonQuery(
                            "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                            ClinicDatabase.CreateParameterNew("table", tableName),
                            ClinicDatabase.CreateParameterNew("data", Newtonsoft.Json.JsonConvert.SerializeObject(row)),
                            ClinicDatabase.CreateParameterNew("user", "-1")
                        );
                    }
                }

                // Backup Journal Voucher and Transactions
                BackupData(dtJournal, "tbl_journal_voucher");
                BackupData(dtTransaction, "tbl_transaction");

                // 3. Mark as deleted (or permanently delete)
                ClinicDatabase.ExecuteNonQuery(
                    "UPDATE tbl_journal_voucher SET state = -1 WHERE id = @id; " +
                    "UPDATE tbl_transaction SET state = -1 WHERE transaction_id = @id AND t_type = 'JOURNAL';",
                    ClinicDatabase.CreateParameterNew("id", id)
                );

                // 4. Log audit
                ////Utilities.LogAudit(-1, "Journal Voucher Deleted", "Journal Voucher", id, "Deleted Journal Voucher: " + txt_jv_code.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting Journal Voucher: " + ex.Message);
            }
        }

        private void guna2TileButton20_Click(object sender, EventArgs e)
        {
            id = 0;
        }

        private void guna2TileButton18_Click(object sender, EventArgs e)
        {
            if (id > 0)
            {
                //ShowReport();
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

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (validateDebitAndCreditAmount())
            {
                if (id != 0)
                {
                    if (chkRequiredDate())
                    {
                        updateJV();
                        //.RefreshJournal();
                        loadPrint();
                        this.Close();
                    }
                }
                else
                {
                    if (chkRequiredDate())
                    {
                        insertJV();
                        //.RefreshJournal();
                        loadPrint();
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Debit Total and Credit Total are not equal.");
            }
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            new frmAddAccount().ShowDialog();
            LoadComboAccount();
        }

        private void nameslistView_Leave(object sender, EventArgs e)
        {
            nameslistView.Visible = false;
            nameslistView.SendToBack();
        }

        static string filePath = "";
        private void BtnAttach_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
            }
        }
        private void AddFileData(int fId)
        {
            AttachmentManager.SaveAttachment(
                    "JOURNAL VOUCHER",
                    fId,
                    openFileDialog1.FileName
                );
        }
    }
}
