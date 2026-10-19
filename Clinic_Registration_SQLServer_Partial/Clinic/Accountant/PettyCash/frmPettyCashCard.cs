using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmPettyCashCard : Form
    {
        int id;
        public frmPettyCashCard(int _id=0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            BindCombos.PopulateAllLevel4Account(cmbDebitAccountName);
            //_mainForm = mainForm;
            //this._petty = _petty;
            this.id = _id;
            if (id != 0)
                this.Text = "Petty Cash Card - Edit";
            else
                this.Text = "Petty Cash Card - New";
            headerUC1.FormText = this.Text;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmPettyCashCard_Load(object sender, EventArgs e)
        {
            if (id != 0)
                BindCombos.PopulateEmployeesToPettyCardAllEmp(cmbEmployee);
            else
                BindCombos.PopulateEmployeesToPettyCard(cmbEmployee);

            if (cmbEmployee.SelectedValue != null)
            {
                int selectedEmployeeId = Convert.ToInt32(cmbEmployee.SelectedValue);
            }

            BindData();
        }
        private void BindData()
        {
            if (id > 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code, mobile, whatsapp_no, email,account_id,name FROM tbl_petty_cash_card WHERE id = @id",
                            ClinicDatabase.CreateParameterNew("@id", id)))
                {
                    if (reader.Read())
                    {
                        id = int.Parse(reader["id"].ToString());
                        int empId = Convert.ToInt32(reader["name"].ToString());
                        cmbEmployee.SelectedValue = empId;
                        cmbDebitAccountName.SelectedValue = reader["account_id"].ToString();
                        txtCode.Text = reader["code"].ToString();
                        txtEmail.Text = reader["email"].ToString();
                        txtMobile.Text = reader["mobile"].ToString();
                        txtWhatsappNo.Text = reader["whatsapp_no"].ToString();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                if (insertAsset())
                {
                    //.RefreshEmployee();
                    this.Close();
                }
                   
            }
            else
                if (updateAsset())
            {
                //.RefreshEmployee();
                this.Close();
            }
        }
        private bool updateAsset()
        {
            if (!chkRequireData())
                return false;

            ClinicDatabase.ExecuteNonQuery(@"Update tbl_petty_cash_card SET code=@code, name=@name, mobile=@mobile, whatsapp_no=@whatsapp_no, email=@email,account_id=@accountId WHERE id=@id",
                ClinicDatabase.CreateParameterNew("id", id),
                ClinicDatabase.CreateParameterNew("code", txtCode.Text),
                        ClinicDatabase.CreateParameterNew("name", cmbEmployee.SelectedValue.ToString()),
                        ClinicDatabase.CreateParameterNew("mobile", txtMobile.Text),
                        ClinicDatabase.CreateParameterNew("whatsapp_no", txtWhatsappNo.Text),
                        ClinicDatabase.CreateParameterNew("email", txtEmail.Text),
                        ClinicDatabase.CreateParameterNew("accountId", cmbDebitAccountName.SelectedValue.ToString()));
            ////Utilities.LogAudit(-1, "Update Petty Cash Card", "Petty Cash Card", id, "Updated Petty Cash Card: " + cmbEmployee.Text);
            return true;
        }
        decimal AssetId;
        private bool insertAsset()
        {
            string code = "PC-0001";
            if (!chkRequireData())
                return false;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT code FROM tbl_petty_cash_card 
                                                           ORDER BY CAST(SUBSTRING_INDEX(code, '-', -1) AS UNSIGNED) DESC LIMIT 1;"))
                if (reader.Read() && reader["code"].ToString() != "")
                    code = "PC-000" + (int.Parse(reader["code"].ToString().Replace("PC-", "")) + 1);

            int refId = ClinicDatabase.ExecuteNonQuery(@"INSERT INTO `tbl_petty_cash_card`(`code`, `name`, `mobile`, `whatsapp_no`, `email`,account_id) 
                          VALUES (@code, @name, @mobile, @whatsapp_no, @email,@accountId);",
                        ClinicDatabase.CreateParameterNew("code", code),
                        ClinicDatabase.CreateParameterNew("name", cmbEmployee.SelectedValue.ToString()),
                        ClinicDatabase.CreateParameterNew("mobile", txtMobile.Text),
                        ClinicDatabase.CreateParameterNew("whatsapp_no", txtWhatsappNo.Text),
                        ClinicDatabase.CreateParameterNew("email", txtEmail.Text),
                        ClinicDatabase.CreateParameterNew("accountId", cmbDebitAccountName.SelectedValue.ToString()));
            if (refId > 0)
            {
                ////Utilities.LogAudit(-1, "Create Petty Cash Card", "Petty Cash Card", refId, "Created Petty Cash Card: " + cmbEmployee.Text);
                return true;
            }
            else
            {
                string message = Utilities.LastLogError();
                MessageBox.Show("Error Check your data" + message);
                return false;
            }
        }

        private bool chkRequireData()
        {
            if (txtDebitCode.Text.Trim() == "")
            {
                MessageBox.Show("Enter Account Code First.");
                return false;
            }
            if (cmbEmployee.SelectedValue ==null || int.Parse(cmbEmployee.SelectedValue.ToString()) <= 0)
            {
                MessageBox.Show("Select Employee First.");
                return false;
            }
            if (cmbDebitAccountName.SelectedValue == null || int.Parse(cmbDebitAccountName.SelectedValue.ToString()) <= 0)
            {
                MessageBox.Show("Select Account First.");
                return false;
            }
            return true;
        }
        private void cmbDebitAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDebitAccountName.SelectedValue == null)
            {
                txtDebitCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbDebitAccountName.SelectedValue.ToString()))
                if (reader.Read())
                    txtDebitCode.Text = reader["code"].ToString();
                else
                    txtDebitCode.Text = "";
        }

        private void txtDebitCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                 ClinicDatabase.CreateParameterNew("code", txtDebitCode.Text)))
                if (reader.Read())
                    cmbDebitAccountName.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtDebitCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code", ClinicDatabase.CreateParameterNew("code", txtDebitCode.Text)))
                if (!reader.Read())
                    cmbDebitAccountName.SelectedIndex = -1;
        }

        private void cmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployee.SelectedValue == null)
            {
                cmbDebitAccountName.SelectedIndex = -1;
                txtDebitCode.Text = "";
                return;
            }

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT petty_cash_id,phone,email FROM tbl_employee WHERE id = @id", ClinicDatabase.CreateParameterNew("@id", cmbEmployee.SelectedValue)))
                if (reader.Read())
                {
                    string pettyCashId = reader["petty_cash_id"].ToString();
                    txtMobile.Text = reader["phone"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                    if (!string.IsNullOrEmpty(pettyCashId))
                    {
                        cmbDebitAccountName.SelectedValue = int.Parse(pettyCashId);

                        // Now fetch the code from tbl_coa_level_4
                        MySqlDataReader coaReader = ClinicDatabase.ExecuteReader(
                            "SELECT code FROM tbl_coa_level_4 WHERE id = @id",
                            ClinicDatabase.CreateParameterNew("@id", pettyCashId));

                        if (coaReader.Read())
                        {
                            txtDebitCode.Text = coaReader["code"].ToString();
                        }
                        else
                        {
                            txtDebitCode.Text = "";
                        }

                        coaReader.Close();
                        coaReader = ClinicDatabase.ExecuteReader(
                            "SELECT id,code, mobile, whatsapp_no, email,account_id FROM tbl_petty_cash_card WHERE name = @id",
                            ClinicDatabase.CreateParameterNew("@id", cmbEmployee.SelectedValue.ToString()));

                        if (coaReader.Read())
                        {
                            //if (id == 0)
                            //{
                                id = int.Parse(coaReader["id"].ToString());
                            //}
                            cmbDebitAccountName.SelectedValue = coaReader["account_id"].ToString();
                            txtCode.Text = coaReader["code"].ToString();
                            txtEmail.Text = coaReader["email"].ToString();
                            txtMobile.Text = coaReader["mobile"].ToString();
                            txtWhatsappNo.Text = coaReader["whatsapp_no"].ToString();
                        }
                    }
                    else
                    {
                        cmbDebitAccountName.SelectedIndex = -1;
                        txtDebitCode.Text = "";
                    }
                }
                else
                {
                    cmbDebitAccountName.SelectedIndex = -1;
                    txtDebitCode.Text = "";
                }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this Petty Cash Card?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id FROM tbl_petty_cash WHERE employee_id In (SELECT c.name from tbl_petty_cash_card c WHERE c.id = @id)", ClinicDatabase.CreateParameterNew("id", id)))
                    if (reader.Read())
                    {
                        MessageBox.Show("Cannot delete this Petty Cash Card because it is referenced in Petty Cash Vouchers.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_petty_cash_card WHERE id = @id", ClinicDatabase.CreateParameterNew("id", id));
                ////Utilities.LogAudit(-1, "Delete Petty Cash Card", "Petty Cash Card", id, "Deleted Petty Cash Card: " + cmbEmployee.Text);
                //.RefreshEmployee();
                this.Close();
            }
        }
    }
}