using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterLoan : Form
    {
        private DataView _dataView;
        private EventHandler employeeUpdateHandler;

        public MasterLoan()
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            ApplyMasterLoanLocalization(this);
            employeeUpdateHandler = (sender, args) => BindEmployee();
            //.Employee += employeeUpdateHandler;

            headerUC1.FormText = Text;
        }

        private void MasterLoan_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvEmployee);
            ConfigureDataGridViews();
            BindEmployee();
            pnlInfo.Height = 80;
            btnExpand.Text = IsArabicLanguage() ? "المزيد" : "More";
            ApplyMasterLoanLocalization(this);
            LocalizeLoanGridHeaders();
            headerUC1.FormText = Text;
        }
        private void MasterLoan_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.Employee -= employeeUpdateHandler;
        }

        private void ConfigureDataGridViews()
        {
            dgvEmployee.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvEmployee.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvEmployee.EnableHeadersVisualStyles = false;
            dgvEmployee.RowsDefaultCellStyle.BackColor = Color.White;
            dgvEmployee.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#eaf1fa");
            dgvEmployee.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#D1EAD0");
            dgvEmployee.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvEmployee.BorderStyle = BorderStyle.None;
            dgvEmployee.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvEmployee.RowHeadersVisible = false;
            dgvEmployee.AllowUserToAddRows = dgvEmployee.AllowUserToResizeRows = false;
        }

        public void BindEmployee()
        {
            string query = @"
                    SELECT 
                        e.id,
                        e.code AS `Employee Code`,
                        e.name AS `Name`
                    FROM tbl_employee e";

            DataTable dt = ClinicDatabase.ExecuteDataTable(query);
            _dataView = dt.DefaultView;
            dgvEmployee.DataSource = _dataView;
            dgvEmployee.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Hide all other columns except "Employee Code" and "Name"
            foreach (DataGridViewColumn column in dgvEmployee.Columns)
            {
                if (column.Name != "Employee Code" && column.Name != "Name")
                    column.Visible = false;
            }

            if (dgvEmployee.Rows.Count > 0)
                BindEmployeeInfo();
            else
                ClearEmployeeDetails();

            LocalizationManager.LocalizeDataGridViewHeaders(dgvEmployee);
            LocalizationManager.LocalizeDataGridViewHeaders(dgvInvoice);
            LocalizeLoanGridHeaders();
        }

        private void BindEmployeeInfo()
        {
            if (dgvEmployee.SelectedRows.Count == 0)
                return;

            string empCode = dgvEmployee.SelectedRows[0].Cells["Employee Code"].Value.ToString();
            string empId = dgvEmployee.SelectedRows[0].Cells["id"].Value.ToString();

            // Fetch Employee Personal Info
            string employeeQuery = @"
                                    SELECT 
                                        e.code,
                                        e.name AS Name,
                                        p.name AS Position, 
                                        e.address AS Address,
                                        e.email AS Email,
                                        e.phone
                                    FROM tbl_employee e
                                    LEFT JOIN tbl_position p ON e.Position_id = p.id  
                                    WHERE e.id = @id;
                                    ";

            var employeeParams = new List<MySqlParameter>
                {
                    new MySqlParameter("@id", empId)
                };

            DataTable empDt = ClinicDatabase.ExecuteDataTable(employeeQuery, employeeParams.ToArray());

            if (empDt.Rows.Count > 0)
            {
                lblCode.Text = empDt.Rows[0]["code"].ToString();
                lblName.Text = empDt.Rows[0]["Name"].ToString();
                lblCat.Text = empDt.Rows[0]["Position"].ToString();
                lblAddress.Text = empDt.Rows[0]["Address"].ToString();
                lblE.Text = empDt.Rows[0]["Email"].ToString();
                lblPh.Text = empDt.Rows[0]["phone"].ToString();
            }
            else
            {
                lblName.Text = lblAddress.Text = lblE.Text = "-";
            }

            // Fetch Salary Records and Display in dgvInvoice
            // BindEmployeeSalary(empCode);
            BindLoan(empCode);
        }

        private void BindLoan(string empCode)
        {
            string loanQuery = @"
                SELECT id, code, LoanDate, EmployeeID, EmployeeName, RequestAmount,
                       Installments, StartDate, EndDate, Description, pay, `change`
                FROM (
                    SELECT *,
                           ROW_NUMBER() OVER (PARTITION BY code ORDER BY id) AS rn
                    FROM tbl_loan
                ) x
                WHERE rn = 1 AND EmployeeID = @empCode;";

            DataTable loanDt = ClinicDatabase.ExecuteDataTable(loanQuery,
                new MySqlParameter("@empCode", empCode));

            // Optional total row for line total
            if (loanDt.Rows.Count > 0)
            {
                decimal finalBalance = Convert.ToDecimal(loanDt.Rows[loanDt.Rows.Count - 1]["RequestAmount"]);
                decimal finalTotalTimes = Convert.ToDecimal(loanDt.Rows[loanDt.Rows.Count - 1]["Installments"]);
                DataRow totalRow = loanDt.NewRow();
                totalRow["EmployeeName"] = "Total";
                totalRow["Description"] = DBNull.Value;
                totalRow["code"] = DBNull.Value;
                totalRow["pay"] = DBNull.Value;
                totalRow["change"] = DBNull.Value;
                totalRow["RequestAmount"] = finalBalance;
                totalRow["Installments"] = finalTotalTimes;

                loanDt.Rows.Add(totalRow);
            }

            dgvInvoice.DataSource = loanDt;
            dgvInvoice.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvInvoice.Columns["RequestAmount"].DefaultCellStyle.Format = "N2";
            dgvInvoice.Columns["pay"].DefaultCellStyle.Format = "N2";
            dgvInvoice.Columns["change"].DefaultCellStyle.Format = "N2";
            dgvInvoice.Columns["Installments"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoice.Columns["RequestAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoice.Columns["pay"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoice.Columns["change"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoice.Columns["LoanDate"].DefaultCellStyle.Format = "dd-MM-yyyy";
            dgvInvoice.Columns["id"].Visible = false;
            LocalizeLoanGridHeaders();
        }

        private void BindEmployeeSalary(string empCode)
        {
            string salaryQuery = @"
                WITH combined AS (
                    -- Loans Received: TYPE = 'P.V', Order = 1
                    SELECT 
                        pv.DATE,
                        pv.id,
                        'P.V' AS `Type`,
                        'LOANS RECEIVED' AS DESCRIPTION,
                        pvd.total AS `RECEIVED_AMOUNT`,
                        0 AS `PAY_AMOUNT`,
                        1 AS order_type
                    FROM tbl_payment_voucher_details pvd
                    INNER JOIN tbl_payment_voucher pv ON pv.id = pvd.payment_id
                    WHERE 
                        pv.`type` = 'Employee' 
                        AND pvd.voucher_type = 'Employee Loan Payment' 
                        AND pvd.hum_id = (select id from tbl_employee where code=@empCode)

                    UNION ALL

                    -- Loan Payments: TYPE = 'S.S', Order = 2
                    SELECT 
                        s.date,
                        s.id,
                        'S.S' AS `Type`,
                        'SALARY - LOAN PAYMENT INSTALLMENT' AS DESCRIPTION,
                        0 AS `RECEIVED_AMOUNT`,
                        s.total_loan AS `PAY_AMOUNT`,
                        2 AS order_type
                    FROM tbl_attendance_salary s
                    WHERE s.emp_code = @empCode AND s.pay <> 0
                )

                SELECT 
                    DATE,
                    id,
                    `Type`,
                    DESCRIPTION,
                    RECEIVED_AMOUNT,
                    PAY_AMOUNT,
                    SUM(RECEIVED_AMOUNT - PAY_AMOUNT) OVER (ORDER BY order_type, DATE, id ROWS UNBOUNDED PRECEDING) AS BALANCE
                FROM combined
                ORDER BY order_type, DATE, id;";

            var salaryParams = new List<MySqlParameter>
            {
                new MySqlParameter("@empCode", empCode)
            };

            DataTable salaryDt = ClinicDatabase.ExecuteDataTable(salaryQuery, salaryParams.ToArray());

            // Optional total row for final balance
            if (salaryDt.Rows.Count > 0)
            {
                decimal finalBalance = Convert.ToDecimal(salaryDt.Rows[salaryDt.Rows.Count - 1]["BALANCE"]);
                DataRow totalRow = salaryDt.NewRow();
                totalRow["Type"] = "Total";
                totalRow["DESCRIPTION"] = DBNull.Value;
                totalRow["RECEIVED_AMOUNT"] = DBNull.Value;
                totalRow["PAY_AMOUNT"] = DBNull.Value;
                totalRow["BALANCE"] = finalBalance;

                salaryDt.Rows.Add(totalRow);
            }

            dgvInvoice.DataSource = salaryDt;
            dgvInvoice.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvInvoice.Rows.Count == 0)
            {
                MessageBox.Show(
                    TranslateMasterLoanText("No salary/loan records found for this employee.", IsArabicLanguage()),
                    TranslateMasterLoanText("Info", IsArabicLanguage()),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (dgvInvoice.Rows.Count > 0)
            {
                //editCustomerToolStripMenuItem1.v
            }
        }

        private void dgvInvoice_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvInvoice.Rows[e.RowIndex].Cells["EmployeeName"].Value?.ToString() == "Total")
            {
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                e.CellStyle.BackColor = Color.LightGray;
            }
        }
        private void ClearEmployeeDetails()
        {
            dgvEmployee.DataSource = null;
            lblName.Text = lblAddress.Text = lblE.Text = "-";
        }

        private void dgvEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                BindEmployeeInfo();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (pnlInfo.Height == 80)
            {
                pnlInfo.Height = 240;
                btnExpand.Text = IsArabicLanguage() ? "أقل" : "Less";
            }
            else
            {
                pnlInfo.Height = 80;
                btnExpand.Text = IsArabicLanguage() ? "المزيد" : "More";
            }
        }

        private void loanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewLoan(0));
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewLoan());
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportPrinter reportPrinter = new ReportPrinter(dgvInvoice, "Employee Loan Report ", true);
            reportPrinter.Print();
        }

        private void excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = IsArabicLanguage() ? "ملفات إكسل|*.xlsx" : "Excel Files|*.xlsx",
                Title = IsArabicLanguage() ? "حفظ ملف إكسل" : "Save Excel File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelExporter exporter = new ExcelExporter(dgvInvoice);
                exporter.ExportToExcel(saveFileDialog.FileName);
            }
        }

        private void dgvInvoice_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // open loan details form
            if (e.RowIndex >= 0 && dgvInvoice.Rows[e.RowIndex].Cells["code"].Value != DBNull.Value)
            {
                int loanId = Convert.ToInt32(dgvInvoice.Rows[e.RowIndex].Cells["id"].Value);
                frmMain.openChildForm(new frmViewLoan(loanId));
            }
        }

        private bool IsArabicLanguage()
        {
            try
            {
                string language = Clinic.Properties.Settings.Default.ApplicationLanguage;
                return !string.IsNullOrWhiteSpace(language) &&
                       (language.Equals("Arabic", StringComparison.OrdinalIgnoreCase) ||
                        language.Equals("AR", StringComparison.OrdinalIgnoreCase) ||
                        language.StartsWith("ar-", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private string TranslateMasterLoanText(string text, bool arabic)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Master Loan", "إدارة القروض" },
                { "Employee Loan Center", "مركز قروض الموظفين" },
                { "Employee Information", "معلومات الموظف" },
                { "Employees", "الموظفون" },
                { "View employee details and manage loan records.", "عرض تفاصيل الموظف وإدارة سجلات القروض." },
                { "View employee details and manage loan records", "عرض تفاصيل الموظف وإدارة سجلات القروض" },
                { "Loan Info", "معلومات القرض" },
                { "Add New Loan", "إضافة قرض جديد" },
                { "Add New", "إضافة جديد" },
                { "Employee Details", "تفاصيل الموظف" },
                { "Loan Details", "تفاصيل القرض" },
                { "Employee Code", "رمز الموظف" },
                { "Employee Name", "اسم الموظف" },
                { "Employee Address", "عنوان الموظف" },
                { "Employee Email", "البريد الإلكتروني للموظف" },
                { "Employee Phone", "هاتف الموظف" },
                { "Print ▼", "طباعة ▼" },
                { "▼ Print", "▼ طباعة" },
                { "Print  ▼", "طباعة  ▼" },
                { "Print...", "طباعة..." },
                { "Export", "تصدير" },
                { "Search", "بحث" },
                { "Employee Loan", "قرض الموظف" },
                { "Loan", "القرض" },
                { "Employee", "الموظف" },
                { "Code", "الرمز" },
                { "Name", "الاسم" },
                { "Position", "الوظيفة" },
                { "Category", "الفئة" },
                { "Address", "العنوان" },
                { "Email", "البريد الإلكتروني" },
                { "Phone", "الهاتف" },
                { "Loan Date", "تاريخ القرض" },
                { "Date", "التاريخ" },
                { "Request Amount", "المبلغ المطلوب" },
                { "RequestAmount", "المبلغ المطلوب" },
                { "Installments", "عدد الأقساط" },
                { "Start Date", "تاريخ البدء" },
                { "StartDate", "تاريخ البدء" },
                { "End Date", "تاريخ الانتهاء" },
                { "EndDate", "تاريخ الانتهاء" },
                { "Description", "الوصف" },
                { "Pay", "المدفوع" },
                { "Change", "المتبقي" },
                { "Total", "الإجمالي" },
                { "More", "المزيد" },
                { "Less", "أقل" },
                { "Print", "طباعة" },
                { "Excel", "إكسل" },
                { "Export to Excel", "تصدير إلى إكسل" },
                { "Save Excel File", "حفظ ملف إكسل" },
                { "Excel Files|*.xlsx", "ملفات إكسل|*.xlsx" },
                { "No salary/loan records found for this employee.", "لا توجد سجلات رواتب أو قروض لهذا الموظف." },
                { "Info", "معلومات" },
                { "OK", "موافق" },
                { "Cancel", "إلغاء" },
                { "Close", "إغلاق" }
            };

            string trimmed = text.Trim();
            if (arabic && translations.TryGetValue(trimmed, out string translated))
                return translated;

            // Print controls may contain a dropdown arrow or extra spacing.
            if (arabic && trimmed.IndexOf("Print", StringComparison.OrdinalIgnoreCase) >= 0)
                return trimmed.Replace("Print", "طباعة").Replace("print", "طباعة");
            if (arabic && string.Equals(trimmed, "Excel", StringComparison.OrdinalIgnoreCase))
                return "إكسل";

            if (!arabic)
            {
                foreach (var item in translations)
                {
                    if (string.Equals(item.Value, trimmed, StringComparison.Ordinal))
                        return item.Key;
                }
                if (trimmed.Contains("طباعة")) return trimmed.Replace("طباعة", "Print");
            }

            return text;
        }

        private void ApplyMasterLoanLocalization(Control root)
        {
            bool arabic = IsArabicLanguage();

            foreach (Control control in root.Controls)
            {
                string translated = TranslateMasterLoanText(control.Text, arabic);
                if (!string.Equals(control.Text, translated, StringComparison.Ordinal))
                    control.Text = translated;

                if (arabic)
                {
                    if (control is Label)
                        control.RightToLeft = RightToLeft.Yes;
                }
                else
                {
                    if (control is Label)
                        control.RightToLeft = RightToLeft.No;
                }

                if (control.HasChildren)
                    ApplyMasterLoanLocalization(control);
            }

            if (arabic)
                RightToLeft = RightToLeft.Yes;
            else
                RightToLeft = RightToLeft.No;

            RightToLeftLayout = arabic;
            if (btnExpand != null)
                btnExpand.Text = TranslateMasterLoanText(btnExpand.Text, arabic);

            // ToolStrip items are not part of Control.Controls, so localize them separately.
            ApplyMasterLoanToolStripLocalization(this, arabic);

            if (headerUC1 != null)
                headerUC1.FormText = TranslateMasterLoanText(Text, arabic);
        }

        private void ApplyMasterLoanToolStripLocalization(Control root, bool arabic)
        {
            foreach (Control control in root.Controls)
            {
                if (control is ToolStrip toolStrip)
                {
                    foreach (ToolStripItem item in toolStrip.Items)
                    {
                        item.Text = TranslateMasterLoanText(item.Text, arabic);
                        if (item is ToolStripDropDownItem dropDown)
                            ApplyMasterLoanToolStripItems(dropDown.DropDownItems, arabic);
                    }
                }

                if (control.HasChildren)
                    ApplyMasterLoanToolStripLocalization(control, arabic);
            }
        }

        private void ApplyMasterLoanToolStripItems(ToolStripItemCollection items, bool arabic)
        {
            foreach (ToolStripItem item in items)
            {
                item.Text = TranslateMasterLoanText(item.Text, arabic);
                if (item is ToolStripDropDownItem dropDown)
                    ApplyMasterLoanToolStripItems(dropDown.DropDownItems, arabic);
            }
        }

        private void LocalizeLoanGridHeaders()
        {
            bool arabic = IsArabicLanguage();

            if (dgvEmployee.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in dgvEmployee.Columns)
                {
                    string key = column.HeaderText;
                    if (column.Name == "Employee Code" || column.DataPropertyName == "Employee Code")
                        key = "Employee Code";
                    else if (column.Name == "Name" || column.DataPropertyName == "Name")
                        key = "Name";

                    column.HeaderText = TranslateMasterLoanText(key, arabic);
                }
            }

            if (dgvInvoice.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in dgvInvoice.Columns)
                {
                    string key = column.Name;
                    if (string.IsNullOrWhiteSpace(key))
                        key = column.HeaderText;

                    switch (key.ToLowerInvariant())
                    {
                        case "id": column.HeaderText = "ID"; break;
                        case "code": column.HeaderText = arabic ? "الرمز" : "code"; break;
                        case "loandate": column.HeaderText = arabic ? "تاريخ القرض" : "LoanDate"; break;
                        case "employeeid": column.HeaderText = arabic ? "رمز الموظف" : "EmployeeID"; break;
                        case "employeename": column.HeaderText = arabic ? "اسم الموظف" : "EmployeeName"; break;
                        case "requestamount": column.HeaderText = arabic ? "المبلغ المطلوب" : "RequestAmount"; break;
                        case "installments": column.HeaderText = arabic ? "عدد الأقساط" : "Installments"; break;
                        case "startdate": column.HeaderText = arabic ? "تاريخ البدء" : "StartDate"; break;
                        case "enddate": column.HeaderText = arabic ? "تاريخ الانتهاء" : "EndDate"; break;
                        case "description": column.HeaderText = arabic ? "الوصف" : "Description"; break;
                        case "pay": column.HeaderText = arabic ? "المدفوع" : "pay"; break;
                        case "change": column.HeaderText = arabic ? "المتبقي" : "change"; break;
                        case "date": column.HeaderText = arabic ? "التاريخ" : "DATE"; break;
                        case "type": column.HeaderText = arabic ? "النوع" : "Type"; break;
                        case "received_amount": column.HeaderText = arabic ? "المبلغ المستلم" : "RECEIVED_AMOUNT"; break;
                        case "pay_amount": column.HeaderText = arabic ? "مبلغ السداد" : "PAY_AMOUNT"; break;
                        case "balance": column.HeaderText = arabic ? "الرصيد" : "BALANCE"; break;
                        default:
                            column.HeaderText = TranslateMasterLoanText(column.HeaderText, arabic);
                            break;
                    }
                }
            }
        }

    }
}
