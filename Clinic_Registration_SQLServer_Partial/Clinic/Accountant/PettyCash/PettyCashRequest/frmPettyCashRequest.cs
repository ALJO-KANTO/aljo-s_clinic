using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmPettyCashRequest : Form
    {
        int id;
        public frmPettyCashRequest(int _id)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            this.id = _id;
            headerUC1.FormText = this.Text;
        }

        private void frmPettyCashRequest_Load(object sender, EventArgs e)
        {
            dtpRequest.Value = DateTime.Now.Date;
            BindCombos.PopulatePettyCash(cmbPettyCash);
            BindPetty(id);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
                if (id == 0)
            {
                if (insertPettyCash())
                    this.Close();
            }
            else
                   if (updatePetty())
                this.Close();
        }
        private bool updatePetty()
        {
            string query = @"
                UPDATE tbl_petty_cash_request SET
                    request_date = @request_date,
                    request_ref = @request_ref,
                    Petty_cash_name = @Petty_cash_name,
                    amount = @amount,
                    description = @description
                WHERE id = @id";

                ClinicDatabase.ExecuteNonQuery(query,
                    ClinicDatabase.CreateParameterNew("@request_date", dtpRequest.Value.Date),
                    ClinicDatabase.CreateParameterNew("@request_ref", txtRequestREF.Text),
                    ClinicDatabase.CreateParameterNew("@Petty_cash_name", cmbPettyCash.SelectedValue?.ToString() ?? ""),
                    ClinicDatabase.CreateParameterNew("@amount", string.IsNullOrWhiteSpace(txtAmount.Text) ? 0 : Convert.ToDecimal(txtAmount.Text)),
                    ClinicDatabase.CreateParameterNew("@description", txtDescription.Text),
                    ClinicDatabase.CreateParameterNew("@id", id)
                );
                return true;
        }
        private string GenerateNextREFCode()
        {
            string newCode = "PR-0001";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(SUBSTRING(request_ref, 4) AS UNSIGNED)) AS lastCode FROM tbl_petty_cash_request"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = "PR-" + code.ToString("D4");
                }
            }

            return newCode;
        }
        private bool insertPettyCash()
        {
            if (!chkRequireData())
                return false;
            int debitAccountId = 0;
            string pettyCashName = cmbPettyCash.SelectedValue?.ToString() ?? "";
            using (var reader = ClinicDatabase.ExecuteReader("select account_id from tbl_petty_cash_card where name=@name",
                ClinicDatabase.CreateParameterNew("name", pettyCashName)))
            {
                if (reader.Read())
                {
                    debitAccountId = Convert.ToInt32(reader["account_id"]);
                }
            }
                string query = @"
                                INSERT INTO `tbl_petty_cash_request`
                                (`request_date`, `request_ref`, `Petty_cash_name`,`amount`, `description`, `debit_account_id`, state, `pay`,`change`, `created_by`, `created_date`) 
                                VALUES 
                                (@request_date, @request_ref, @Petty_cash_name, @amount, @description,@debit_account_id, @state,@pay,@change, @created_by, @created_date);";

            try
            {
                ClinicDatabase.ExecuteNonQuery(query,
                    ClinicDatabase.CreateParameterNew("request_date", dtpRequest.Value.Date),
                    ClinicDatabase.CreateParameterNew("request_ref", GenerateNextREFCode()),
                    ClinicDatabase.CreateParameterNew("Petty_cash_name", cmbPettyCash.SelectedValue?.ToString() ?? ""),
                    ClinicDatabase.CreateParameterNew("amount", string.IsNullOrWhiteSpace(txtAmount.Text) ? 0 : Convert.ToDecimal(txtAmount.Text)),
                    ClinicDatabase.CreateParameterNew("description", txtDescription.Text),
                    ClinicDatabase.CreateParameterNew("debit_account_id", debitAccountId),
                    ClinicDatabase.CreateParameterNew("state", "New"),
                    ClinicDatabase.CreateParameterNew("pay", 0),
                    ClinicDatabase.CreateParameterNew("change", string.IsNullOrWhiteSpace(txtAmount.Text) ? 0 : Convert.ToDecimal(txtAmount.Text)),
                    ClinicDatabase.CreateParameterNew("created_by", -1),
                    ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date)
                );

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during insert: " + ex.Message);
                return false;
            }
        }
        private bool chkRequireData()
        {
            if (txtAmount.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Amount");
                return false;
            }
            return true;
        }
        public void BindPetty(int _id)
        {
            id = Convert.ToInt32(_id);
            string query = "SELECT * FROM tbl_petty_cash_request WHERE id = @id";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("@id", _id)))
            {
                if (reader.Read())
                {
                    txtRequestREF.Text = reader["request_ref"].ToString();
                    cmbPettyCash.SelectedValue = reader["Petty_cash_name"].ToString();
                    txtAmount.Text = Convert.ToDecimal(reader["amount"]).ToString("N2");
                    if(reader["state"].ToString() == "New")
                    {
                        txtAmount.Enabled = true;
                    }
                    else
                        txtAmount.Enabled = false;
                    txtDescription.Text = reader["description"].ToString();
                    dtpRequest.Value = Convert.ToDateTime(reader["request_date"]);
                }
            }
        }

        private void guna2GroupBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
