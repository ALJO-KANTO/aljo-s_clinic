using Clinic_DataAccess;
using DocumentFormat.OpenXml.Office2010.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewLoan : Form
    {
        int id;
        public frmViewLoan(int id = 0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            this.id = id;
            headerUC1.FormText = this.Text;
        }

        private void frmViewLoan_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            //AppTheme.DefaultFont(dgvLoan);
            BindCombos.PopulateEmployeesForLoan(cmbEmployeeName);
            BindCombos.PopulateAllLevel4Account(cmbAccountNameCredit);
            BindCombos.PopulateAllLevel4Account(cmbAccountNameDebit);
            if (id > 0)
            {
                BindData();
            }
            else
            {
                txtCode.Text = GenerateNextCode();
            }
        }

        private void BindData()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT l.*,e.id AS empId FROM tbl_loan l LEFT JOIN tbl_employee e ON e.code=l.EmployeeID WHERE l.id = @id", ClinicDatabase.CreateParameterNew("id", id)))
            {
                if (reader.Read())
                {
                    cmbAccountNameCredit.SelectedValue = reader["credit_account_id"];
                    cmbAccountNameDebit.SelectedValue = reader["debit_account_id"];
                    cmbEmployeeName.SelectedValue = reader["empId"];
                    txtCode.Text = reader["code"].ToString();
                    txtDescription.Text = reader["description"].ToString();
                    txtEmployeeCode.Text = reader["employeeId"].ToString();
                    txtInstallments.Text = reader["installments"].ToString();
                    txtRequestAmount.Text = decimal.Parse(reader["RequestAmount"].ToString()).ToString("N2");
                    dtpRequestDate.Value = DateTime.Parse(reader["LoanDate"].ToString());
                    dtpStartDate.Value = DateTime.Parse(reader["StartDate"].ToString());
                    dtpEndDate.Value = DateTime.Parse(reader["EndDate"].ToString());

                    using (MySqlDataReader readerDetail = ClinicDatabase.ExecuteReader("select * from tbl_loan where code = @code", ClinicDatabase.CreateParameterNew("code", reader["code"].ToString())))
                    {
                        int slNo = 1;
                        while (readerDetail.Read())
                        {
                            string install = readerDetail["Installments"].ToString();
                            string dated = readerDetail["LoanDates"].ToString();
                            DateTime dateTime = DateTime.Parse(dated);
                            dgvLoan.Rows.Add(slNo, dateTime.ToShortDateString(), dateTime.ToString("MMMM"), txtDescription.Text, decimal.Parse(readerDetail["Amount"].ToString()).ToString("N2"));
                            slNo++;
                        }
                    }

                    BtnDelete.Visible = true;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                if (insertLoan())
                    this.Close();
            }
            else
                  if (updateLoan())
                this.Close();
        }

        private void SetupDataGridView()
        {
            dgvLoan.Columns.Clear();
            dgvLoan.Columns.Add("SN", "SN#");
            dgvLoan.Columns.Add("Date", "Date");
            dgvLoan.Columns.Add("Month", "Month");
            dgvLoan.Columns.Add("Description", "Description");
            dgvLoan.Columns.Add("Amount", "Amount");
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvLoan);
        }

        private void txtInstallments_TextChanged(object sender, EventArgs e)
        {
            if (txtInstallments.Focused)
                BindGrid();
        }
        decimal attempInstall = 1;
        private void BindGrid()
        {
            if (txtInstallments.Text.Trim() == "" || txtRequestAmount.Text.Trim() == "")
            {
                return;
            }
            dtpEndDate.Value = dtpStartDate.Value.AddMonths(int.Parse(txtInstallments.Text) - 1);
            dgvLoan.Rows.Clear();
            attempInstall = 1;
            for (DateTime dt = dtpStartDate.Value.Date; dt <= dtpEndDate.Value.Date; dt = dt.AddMonths(1))
            {
                decimal requestedAmount = decimal.Parse(txtRequestAmount.Text) / decimal.Parse(txtInstallments.Text);
                dgvLoan.Rows.Add(attempInstall, dt.ToShortDateString(), dt.ToString("MMMM"), txtDescription.Text, requestedAmount.ToString("N2"));
                attempInstall++;
            }
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            //CalculateAndUpdateGrid();
        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            //CalculateAndUpdateGrid();
        }

        private bool insertLoan()
        {
            if (!chkRequiredDate())
                return false;

            txtCode.Text = GenerateNextCode();
            if (dgvLoan.Rows.Count == 0)
            {
                MessageBox.Show("No data available to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DateTime loanDate = dtpRequestDate.Value;
            string employeeName = cmbEmployeeName.Text;
            int employeeCode = Convert.ToInt32(txtEmployeeCode.Text);
            decimal requestAmount = Convert.ToDecimal(txtRequestAmount.Text);
            int installments = Convert.ToInt32(txtInstallments.Text);
            DateTime startDate = dtpStartDate.Value;
            DateTime endDate = dtpEndDate.Value;

            foreach (DataGridViewRow row in dgvLoan.Rows)
            {
                if (!row.IsNewRow)
                {
                    DateTime loanDates = Convert.ToDateTime(row.Cells["Date"].Value);
                    string months = row.Cells["Month"].Value.ToString();
                    string description = row.Cells["Description"].Value.ToString();
                    decimal amount = Convert.ToDecimal(row.Cells["Amount"].Value);

                    id = (int)decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_loan (code, LoanDate, EmployeeID, EmployeeName, RequestAmount, Installments,
                                            StartDate, EndDate, loanDates, Months, Description, Amount,debit_account_id,credit_account_id,`change`) 
                                            VALUES (@code, @LoanDate, @EmployeeID, @EmployeeName, @RequestAmount, @Installments, @StartDate, @EndDate,
                                            @loanDates, @Months, @Description, @Amount,@debit_account_id,@credit_account_id,@change); SELECT LAST_INSERT_ID();",
                        ClinicDatabase.CreateParameterNew("code", txtCode.Text),
                        ClinicDatabase.CreateParameterNew("LoanDate", loanDate.ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("EmployeeID", employeeCode),
                        ClinicDatabase.CreateParameterNew("EmployeeName", employeeName),
                        ClinicDatabase.CreateParameterNew("RequestAmount", requestAmount),
                        ClinicDatabase.CreateParameterNew("Installments", installments),
                        ClinicDatabase.CreateParameterNew("StartDate", startDate.ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("EndDate", endDate.ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("loanDates", DateTime.Parse(row.Cells["Date"].Value.ToString()).ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("Months", row.Cells["Month"].Value.ToString()),
                        ClinicDatabase.CreateParameterNew("Description", row.Cells["Description"].Value.ToString()),
                        ClinicDatabase.CreateParameterNew("Amount", Convert.ToDecimal(row.Cells["Amount"].Value)),
                        ClinicDatabase.CreateParameterNew("debit_account_id", cmbAccountNameDebit.SelectedValue),
                        ClinicDatabase.CreateParameterNew("credit_account_id", cmbAccountNameCredit.SelectedValue),
                        ClinicDatabase.CreateParameterNew("change", requestAmount)
                        ).ToString());
                    ClinicDatabase.ExecuteNonQuery(@"
                UPDATE tbl_attendance_salary 
                SET total_loan = IFNULL(total_loan, 0) + @Amount
                WHERE emp_code = @EmployeeID
                  AND MONTH(`date`) = @Month
                  AND YEAR(`date`) = @Year;",
               ClinicDatabase.CreateParameterNew("EmployeeID", employeeCode),
               ClinicDatabase.CreateParameterNew("Amount", amount),
               ClinicDatabase.CreateParameterNew("Month", loanDates.Month),
               ClinicDatabase.CreateParameterNew("Year", loanDates.Year)
           );
                }
            }
            insertJournals();
            ////Utilities.LogAudit(-1, "Insert Loan", "Loan", id, "Inserted Loan: " + txtCode.Text + " for Employee: " + employeeName);
            MessageBox.Show("Data saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private bool updateLoan()
        {
            if (!chkRequiredDate())
                return false;

            if (dgvLoan.Rows.Count == 0)
            {
                MessageBox.Show("No data available to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            ClinicDatabase.ExecuteNonQuery(@"DELETE FROM tbl_loan WHERE code = @code", ClinicDatabase.CreateParameterNew("code", txtCode.Text));
            CommonInsert.DeleteTransactionEntry(id, "Loan Request");

            ClinicDatabase.ExecuteNonQuery(@"
                        UPDATE tbl_attendance_salary 
                        SET total_loan = (select IFNULL(SUM(RequestAmount), 0) from tbl_loan where emp_code=@EmployeeID)
                        WHERE emp_code = @EmployeeID");

            DateTime loanDate = dtpRequestDate.Value;
            string employeeName = cmbEmployeeName.Text;
            int employeeCode = Convert.ToInt32(txtEmployeeCode.Text);
            decimal requestAmount = Convert.ToDecimal(txtRequestAmount.Text);
            int installments = Convert.ToInt32(txtInstallments.Text);
            DateTime startDate = dtpStartDate.Value;
            DateTime endDate = dtpEndDate.Value;

            foreach (DataGridViewRow row in dgvLoan.Rows)
            {
                if (!row.IsNewRow)
                {
                    DateTime loanDates = Convert.ToDateTime(row.Cells["Date"].Value);
                    string months = row.Cells["Month"].Value.ToString();
                    string description = row.Cells["Description"].Value.ToString();
                    decimal amount = Convert.ToDecimal(row.Cells["Amount"].Value);

                    id = (int)decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_loan (code, LoanDate, EmployeeID, EmployeeName, RequestAmount, Installments,
                                            StartDate, EndDate, loanDates, Months, Description, Amount,debit_account_id,credit_account_id,`change`) 
                                            VALUES (@code, @LoanDate, @EmployeeID, @EmployeeName, @RequestAmount, @Installments, @StartDate, @EndDate,
                                            @loanDates, @Months, @Description, @Amount,@debit_account_id,@credit_account_id,@change); SELECT LAST_INSERT_ID();",
                        ClinicDatabase.CreateParameterNew("code", txtCode.Text),
                        ClinicDatabase.CreateParameterNew("LoanDate", loanDate.ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("EmployeeID", employeeCode),
                        ClinicDatabase.CreateParameterNew("EmployeeName", employeeName),
                        ClinicDatabase.CreateParameterNew("RequestAmount", requestAmount),
                        ClinicDatabase.CreateParameterNew("Installments", installments),
                        ClinicDatabase.CreateParameterNew("StartDate", startDate.ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("EndDate", endDate.ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("loanDates", DateTime.Parse(row.Cells["Date"].Value.ToString()).ToString("yyyy-MM-dd")),
                        ClinicDatabase.CreateParameterNew("Months", row.Cells["Month"].Value.ToString()),
                        ClinicDatabase.CreateParameterNew("Description", row.Cells["Description"].Value.ToString()),
                        ClinicDatabase.CreateParameterNew("Amount", Convert.ToDecimal(row.Cells["Amount"].Value)),
                        ClinicDatabase.CreateParameterNew("debit_account_id", cmbAccountNameDebit.SelectedValue),
                        ClinicDatabase.CreateParameterNew("credit_account_id", cmbAccountNameCredit.SelectedValue),
                        ClinicDatabase.CreateParameterNew("change", requestAmount)
                        ).ToString());
                    ClinicDatabase.ExecuteNonQuery(@"
                        UPDATE tbl_attendance_salary 
                        SET total_loan = IFNULL(total_loan, 0) + @Amount
                        WHERE emp_code = @EmployeeID
                          AND MONTH(`date`) = @Month
                          AND YEAR(`date`) = @Year;",
                       ClinicDatabase.CreateParameterNew("EmployeeID", employeeCode),
                       ClinicDatabase.CreateParameterNew("Amount", amount),
                       ClinicDatabase.CreateParameterNew("Month", loanDates.Month),
                       ClinicDatabase.CreateParameterNew("Year", loanDates.Year)
                   );
                }
            }
            insertJournals();
            ////Utilities.LogAudit(-1, "Update Loan", "Loan", id, "Updated Loan: " + txtCode.Text + " for Employee: " + employeeName);
            MessageBox.Show("Data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private string GenerateNextCode()
        {
            int code = 10001;
            string result = "LO" + code;

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT code FROM tbl_loan ORDER BY CAST(SUBSTRING(code, 3) AS UNSIGNED) DESC LIMIT 1;"))
                if (reader.Read() && reader["code"].ToString() != "")
                    code = int.Parse(reader["code"].ToString().Replace("LO", "")) + 1;
            result = "LO" + code;

            return result;
        }
        private void insertJournals()
        {

            CommonInsert.addTransactionEntry(dtpRequestDate.Value.Date,
                  cmbAccountNameDebit.SelectedValue.ToString(),
                  txtRequestAmount.Text, "0", id.ToString(), cmbEmployeeName.SelectedValue.ToString(), "Loan Request", "Employee LOAN", "Loan Request " + txtCode.Text,
                   -1, DateTime.Now.Date, txtCode.Text);
            CommonInsert.addTransactionEntry(dtpRequestDate.Value.Date,
                                  cmbAccountNameCredit.SelectedValue.ToString(),
                                 "0", txtRequestAmount.Text, id.ToString(), cmbEmployeeName.SelectedValue.ToString(), "Loan Request", "Employee LOAN", "Loan Request " + txtCode.Text,
                   -1, DateTime.Now.Date, txtCode.Text);

        }
        private bool chkRequiredDate()
        {
            if (txtRequestAmount.Text.Trim() == "")
            {
                MessageBox.Show("Enter Request Amount Please.");
                return false;
            }

            return true;
        }
        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (insertLoan())
                resetTextBox();
        }
        private void resetTextBox()
        {

        }

        private void cmbEmployeeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployeeName.SelectedValue == null)
            {
                txtEmployeeCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where id = " + cmbEmployeeName.SelectedValue.ToString()))
                if (reader.Read())
                    txtEmployeeCode.Text = reader["code"].ToString();
                else
                    txtEmployeeCode.Text = "";
        }

        private void cmbAccountNameDebit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountNameDebit.SelectedValue == null)
            {
                txtAccountCodeDebit.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbAccountNameDebit.SelectedValue.ToString()))
                if (reader.Read())
                    txtAccountCodeDebit.Text = reader["code"].ToString();
                else
                    txtAccountCodeDebit.Text = "";
        }

        private void cmbAccountNameCredit_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cmbAccountNameCredit.SelectedValue == null)
            {
                txtAccountCodeCredit.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbAccountNameCredit.SelectedValue.ToString()))
                if (reader.Read())
                    txtAccountCodeCredit.Text = reader["code"].ToString();
                else
                    txtAccountCodeCredit.Text = "";
        }

        private void txtAccountCodeDebit_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
            ClinicDatabase.CreateParameterNew("code", txtAccountCodeDebit.Text)))
                if (reader.Read())
                    cmbAccountNameDebit.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtAccountCodeCredit_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
            ClinicDatabase.CreateParameterNew("code", txtAccountCodeCredit.Text)))
                if (reader.Read())
                    cmbAccountNameCredit.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtAccountCodeDebit_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
        ClinicDatabase.CreateParameterNew("code", txtAccountCodeDebit.Text)))
                if (!reader.Read())
                    cmbAccountNameDebit.SelectedIndex = -1;
        }

        private void txtAccountCodeCredit_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                     ClinicDatabase.CreateParameterNew("code", txtAccountCodeCredit.Text)))
                if (!reader.Read())
                    cmbAccountNameCredit.SelectedIndex = -1;
        }

        private void txtRequestAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void txtInstallments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private bool DeleteData()
        {
            if (!chkRequiredDate())
                return false;

            if (dgvLoan.Rows.Count == 0)
            {
                MessageBox.Show("No data available to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string query = @"SELECT * FROM tbl_payment_voucher_details pvd
                    INNER JOIN tbl_payment_voucher pv ON pv.id = pvd.payment_id
                    WHERE
                        pv.`type` = 'Employee'
                        AND pvd.voucher_type = 'Employee Loan Payment'
                        AND pvd.hum_id = (select id from tbl_employee where code = @empCode)";
            using(MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("empCode", txtEmployeeCode.Text)))
            {
                if (reader.Read())
                {
                    MessageBox.Show("Cannot delete this loan as there are associated payment vouchers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            ClinicDatabase.ExecuteNonQuery(@"DELETE FROM tbl_loan WHERE code = @code", ClinicDatabase.CreateParameterNew("code", txtCode.Text));
            CommonInsert.DeleteTransactionEntry(id, "Loan Request");

            ClinicDatabase.ExecuteNonQuery(@"
                        UPDATE tbl_attendance_salary 
                        SET total_loan = (select IFNULL(SUM(RequestAmount), 0) from tbl_loan where emp_code=@EmployeeID)
                        WHERE emp_code = @EmployeeID");

            dgvLoan.Rows.Clear();
            txtCode.Text = GenerateNextCode();
            txtAccountCodeDebit.Text = "";
            txtAccountCodeCredit.Text = "";
            cmbAccountNameDebit.SelectedIndex = -1;
            cmbAccountNameCredit.SelectedIndex = -1;
            cmbEmployeeName.SelectedIndex = -1;
            txtDescription.Text = "";
            txtEmployeeCode.Text = "";
            txtInstallments.Text = "";
            txtRequestAmount.Text = "";
            dtpRequestDate.Value = DateTime.Now;
            dtpStartDate.Value = DateTime.Now;
            dtpEndDate.Value = DateTime.Now;

            MessageBox.Show("Loan deleted successfully.", "Success");
            return true;
        }
    }
}
