using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmSalarySheet : Form
    {
        // REQUIRED for Activator.CreateInstances
        public frmSalarySheet() : this(0)
        {
        }
        public frmSalarySheet(int id = 0)
        {
            InitializeComponent();

           // LocalizationManager.LocalizeForm(this);

            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
        }
        public void ApplyLanguageLayout(bool isArabic)
        {
            SuspendLayout();
            pnlHero.SuspendLayout();
            pnlFilterCard.SuspendLayout();
            pnlGridTitle.SuspendLayout();
            pnlFooter.SuspendLayout();

            try
            {
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                if (isArabic)
                {
                    // =====================================================
                    // ARABIC TEXT
                    // =====================================================
                    Text = "كشف الرواتب";
                    headerUC1.FormText = "كشف الرواتب";

                    lblPageTitle.Text = "كشف الرواتب";
                    lblPageSubTitle.Text = "عرض وإدارة تفاصيل رواتب الموظفين";

                    label1.Text = "التاريخ";
                    label2.Text = "المرجع";
                    label5.Text = "المستخدم";
                    label7.Text = "رقم الكشف";

                    label3.Text = "الشهر";
                    label4.Text = "السنة";

                    lblEmployeesTitle.Text = "الموظفون";
                    lblBasicTitle.Text = "إجمالي الراتب الأساسي";
                    lblDeductionTitle.Text = "إجمالي الخصومات";
                    lblNetTitle.Text = "إجمالي صافي الراتب";

                    lblGridTitle.Text = "معلومات الرواتب";
                    txtSearchVisual.PlaceholderText = "بحث عن موظف...";

                    guna2HtmlLabel1.Text = "معلومات الموظف";
                    guna2HtmlLabel2.Text = "معلومات الراتب";
                    guna2HtmlLabel3.Text = "الخصومات";
                    guna2HtmlLabel4.Text = "ملخص الراتب";

                    btnSave.Text = "حفظ";


                    // =====================================================
                    // HERO TITLE -> RIGHT
                    // =====================================================
                    int margin = 34;
                    int gap = 16;

                    pnlTitleIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    pnlTitleIcon.Location = new Point(
                        pnlHero.ClientSize.Width
                        - margin
                        - pnlTitleIcon.Width,
                        18);


                    lblPageTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblPageTitle.RightToLeft = RightToLeft.Yes;
                    lblPageTitle.TextAlign = ContentAlignment.MiddleRight;

                    lblPageTitle.Location = new Point(
                        pnlTitleIcon.Left
                        - gap
                        - lblPageTitle.Width,
                        18);


                    lblPageSubTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblPageSubTitle.RightToLeft = RightToLeft.Yes;
                    lblPageSubTitle.TextAlign = ContentAlignment.MiddleRight;

                    lblPageSubTitle.Location = new Point(
                        pnlTitleIcon.Left
                        - gap
                        - lblPageSubTitle.Width,
                        62);


                    // =====================================================
                    // META CARD -> LEFT
                    // =====================================================
                    pnlMetaCard.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    pnlMetaCard.Location =
                        new Point(34, pnlMetaCard.Top);

                    pnlMetaCard.RightToLeft = RightToLeft.Yes;


                    // =====================================================
                    // FILTERS
                    // =====================================================
                    pnlFilterCard.RightToLeft = RightToLeft.Yes;

                    label3.RightToLeft = RightToLeft.Yes;
                    label4.RightToLeft = RightToLeft.Yes;


                    // =====================================================
                    // GRID TITLE -> RIGHT
                    // SEARCH -> LEFT
                    // =====================================================
                    lblGridIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblGridIcon.Location = new Point(
                        pnlGridTitle.ClientSize.Width
                        - lblGridIcon.Width
                        - 12,
                        13);


                    lblGridTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblGridTitle.RightToLeft = RightToLeft.Yes;
                    lblGridTitle.TextAlign = ContentAlignment.MiddleRight;

                    lblGridTitle.Location = new Point(
                        lblGridIcon.Left
                        - lblGridTitle.Width
                        - 8,
                        16);


                    txtSearchVisual.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    txtSearchVisual.Location =
                        new Point(12, 10);

                    txtSearchVisual.RightToLeft =
                        RightToLeft.Yes;


                    // =====================================================
                    // GRID
                    // =====================================================
                    dgvSalarySheet.RightToLeft =
                        RightToLeft.Yes;

                    dgvSalarySheet.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;

                    dgvSalarySheet.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;


                    // =====================================================
                    // COLUMN TRANSLATION
                    // =====================================================
                    Sn.HeaderText = "م";
                    EmployeeID.HeaderText = "رقم الموظف";
                    EmployeeName.HeaderText = "اسم الموظف";
                    Position.HeaderText = "المنصب";
                    WorkDays.HeaderText = "أيام العمل";
                    AbsentDays.HeaderText = "أيام الغياب";
                    BasicSalary.HeaderText = "الراتب الأساسي";
                    Allowance.HeaderText = "البدلات";
                    Other.HeaderText = "أخرى";
                    Additions.HeaderText = "الإضافات";
                    TotalAdditions.HeaderText = "إجمالي المستحقات";
                    EmployeeAdvance.HeaderText = "السلفة";
                    Delay.HeaderText = "التأخير";
                    WorkAbsence.HeaderText = "غياب العمل";
                    Salik.HeaderText = "المخالفات وسالك";
                    othersDeductions.HeaderText = "خصومات أخرى";
                    TotalDeductions.HeaderText = "إجمالي الخصومات";
                    NetSalary.HeaderText = "صافي الراتب";
                    Cash.HeaderText = "نقداً";
                    Bank.HeaderText = "البنك";
                    WPS.HeaderText = "WPS";


                    // Footer/save on LEFT for Arabic
                    btnSave.Anchor =
                        AnchorStyles.Bottom | AnchorStyles.Left;
                }
                else
                {
                    // =====================================================
                    // ENGLISH
                    // =====================================================
                    Text = "Salary Sheet";
                    headerUC1.FormText = "Salary Sheet";

                    lblPageTitle.Text = "Salary Sheet";
                    lblPageSubTitle.Text =
                        "View and manage employee salary details.";

                    label1.Text = "Date";
                    label2.Text = "REF";
                    label5.Text = "User";
                    label7.Text = "S.S No";

                    label3.Text = "Month";
                    label4.Text = "Year";

                    lblEmployeesTitle.Text = "Employees";
                    lblBasicTitle.Text = "Total Basic Salary";
                    lblDeductionTitle.Text = "Total Deductions";
                    lblNetTitle.Text = "Total Net Salary";

                    lblGridTitle.Text = "Salary Information";
                    txtSearchVisual.PlaceholderText =
                        "Search employee...";

                    guna2HtmlLabel1.Text = "Employee Information";
                    guna2HtmlLabel2.Text = "Salary Information";
                    guna2HtmlLabel3.Text = "Deduction";
                    guna2HtmlLabel4.Text = "Salary Summary";

                    btnSave.Text = "Save";


                    // ORIGINAL HERO
                    pnlTitleIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    pnlTitleIcon.Location =
                        new Point(34, 18);

                    lblPageTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblPageTitle.RightToLeft = RightToLeft.No;
                    lblPageTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblPageTitle.Location =
                        new Point(108, 18);

                    lblPageSubTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblPageSubTitle.RightToLeft = RightToLeft.No;
                    lblPageSubTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblPageSubTitle.Location =
                        new Point(111, 62);


                    // ORIGINAL META CARD -> RIGHT
                    pnlMetaCard.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    pnlMetaCard.Location = new Point(
                        pnlHero.ClientSize.Width
                        - pnlMetaCard.Width
                        - 36,
                        15);

                    pnlMetaCard.RightToLeft =
                        RightToLeft.No;


                    // GRID TITLE -> LEFT
                    lblGridIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblGridIcon.Location =
                        new Point(12, 13);

                    lblGridTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblGridTitle.RightToLeft =
                        RightToLeft.No;

                    lblGridTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblGridTitle.Location =
                        new Point(44, 16);


                    // SEARCH -> RIGHT
                    txtSearchVisual.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    txtSearchVisual.Location = new Point(
                        pnlGridTitle.ClientSize.Width
                        - txtSearchVisual.Width
                        - 12,
                        10);

                    txtSearchVisual.RightToLeft =
                        RightToLeft.No;


                    dgvSalarySheet.RightToLeft =
                        RightToLeft.No;

                    dgvSalarySheet.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;


                    // ORIGINAL HEADERS
                    Sn.HeaderText = "Sn";
                    EmployeeID.HeaderText = "EMP ID";
                    EmployeeName.HeaderText = "Employee Name";
                    Position.HeaderText = "Position";
                    WorkDays.HeaderText = "Work Days";
                    AbsentDays.HeaderText = "Absent Days";
                    BasicSalary.HeaderText = "Basic Salary";
                    Allowance.HeaderText = "Allowance";
                    Other.HeaderText = "Other";
                    Additions.HeaderText = "Additions";
                    TotalAdditions.HeaderText = "Total Addition";
                    EmployeeAdvance.HeaderText = "Loan";
                    Delay.HeaderText = "Delay";
                    WorkAbsence.HeaderText = "Work Absence";
                    Salik.HeaderText = "Fines & Salik";
                    othersDeductions.HeaderText = "Other Deductions";
                    TotalDeductions.HeaderText = "Total Deductions";
                    NetSalary.HeaderText = "Net Salary";
                    Cash.HeaderText = "Cash";
                    Bank.HeaderText = "Bank";
                    WPS.HeaderText = "WPS";


                    btnSave.Anchor =
                        AnchorStyles.Bottom | AnchorStyles.Right;
                }

                pnlTitleIcon.BringToFront();
                lblPageTitle.BringToFront();
                lblPageSubTitle.BringToFront();
                pnlMetaCard.BringToFront();
                btnSave.BringToFront();
            }
            finally
            {
                pnlFooter.ResumeLayout(false);
                pnlGridTitle.ResumeLayout(false);
                pnlFilterCard.ResumeLayout(false);
                pnlHero.ResumeLayout(false);
                ResumeLayout(false);

                pnlHero.PerformLayout();
                pnlGridTitle.PerformLayout();
                PerformLayout();

                Invalidate(true);
            }
        }
        private void frmSalarySheet_Load(object sender, EventArgs e)
        {
          
            Years();
            BindMonths();
            cmbMonth.SelectedItem = DateTime.Now.Month.ToString("D2");
            cmbYear.SelectedItem = DateTime.Now.Year.ToString();
            BindGridView();
            string language =
    Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
        }
        private void BindMonths()
        {
            for (int month = 1; month <= 12; month++)
            {
                cmbMonth.Items.Add(month.ToString("D2")); // "01", "02", ..., "12"
            }
        }
        private void Years()
        {
            for(int year = 2000; year <= 2050; year++)
            {
                cmbYear.Items.Add(year.ToString());
            }
        }
        private void BindGridView()
        {

            dgvSalarySheet.Rows.Clear();
            if(cmbMonth.SelectedItem == null || cmbYear.SelectedItem == null)
            {
                MessageBox.Show("Please select both Month and Year.");
            }
            string selectedMonthName = cmbMonth.SelectedItem.ToString();
            int selectedYear = int.Parse(cmbYear.SelectedItem.ToString());
            int selectedMonth = int.Parse(selectedMonthName);
            string refQuery = @"SELECT CONCAT(Ref_Code, ' - ', YEAR(WorkDate)) AS Ref_Code FROM tbl_attendancesheet 
            WHERE YEAR(WorkDate) = @year AND MONTH(WorkDate) = @month 
            LIMIT 1";
            object refResult = ClinicDatabase.ExecuteScalar(refQuery,
            ClinicDatabase.CreateParameterNew("@year", selectedYear),
            ClinicDatabase.CreateParameterNew("@month", selectedMonth));
            lblREF.Text = (refResult != null && refResult != DBNull.Value) ? refResult.ToString() : "";
            //SSNO
            refQuery = @"SELECT ss_no FROM tbl_attendance_salary 
            WHERE YEAR(date) = @year AND MONTH(date) = @month 
            LIMIT 1";
            refResult = ClinicDatabase.ExecuteScalar(refQuery,
            ClinicDatabase.CreateParameterNew("@year", selectedYear),
            ClinicDatabase.CreateParameterNew("@month", selectedMonth));
            lblSSNo.Text = (refResult != null && refResult != DBNull.Value) ? refResult.ToString() : "";
            //user
            lblUser.Text = clsGlobal.CurrentUser.UserName;

            string employeeQuery = @"SELECT 
             ROW_NUMBER() OVER(
            ORDER BY e.code) AS SN, 
             e.*, 
             p.name AS PositionName
            FROM tbl_employee e
            LEFT JOIN tbl_position p ON e.Position_id = p.id
            WHERE e.state = 0";
            
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(employeeQuery))
            {
                while (reader.Read())
                {
                    string empCode = reader["code"].ToString();
                    using (MySqlDataReader areader = ClinicDatabase.ExecuteReader(
                    "SELECT * FROM tbl_attendancesheet WHERE YEAR(WorkDate) = @year AND MONTH(WorkDate) = @month AND code = @id",
                    ClinicDatabase.CreateParameterNew("year", selectedYear),
                    ClinicDatabase.CreateParameterNew("month", selectedMonth),
                    ClinicDatabase.CreateParameterNew("id", empCode)))
                    {
                        if (!areader.Read())
                            continue;
                        decimal allowance = decimal.Parse(reader["HousingAllowance"].ToString()) + decimal.Parse(reader["TransportationAllowance"].ToString());
                        decimal other = decimal.Parse(reader["Other"].ToString());
                        decimal totalBasicSalary = decimal.Parse(reader["basicSalary"].ToString());

                        int absDays = GetAbsenceDays(empCode, selectedYear, selectedMonth);
                        decimal totalLoan = GetTotalLoan(empCode, selectedYear, selectedMonth);
                        decimal totalDelay = GetTotalDelay(empCode, selectedYear, selectedMonth);
                        decimal totalWorkAbsence = GetTotalAbsence(empCode, selectedYear, selectedMonth);
                        int ref_Id = string.IsNullOrEmpty(areader["Ref_Code"].ToString()) ? 0 : int.Parse(areader["Ref_Code"].ToString());
                        refCode = ref_Id.ToString();
                        decimal additions = GetSalaryAdjustment(empCode, ref_Id, "Additions");
                        decimal salik = GetSalaryAdjustment(empCode, ref_Id, "Salik");
                        decimal otherDedactions = GetSalaryAdjustment(empCode, ref_Id, "othersDeductions");

                        decimal totalEarnings = totalBasicSalary + allowance + other + additions;
                        decimal totalDeductions = totalLoan + totalDelay + totalWorkAbsence + salik + otherDedactions;
                        decimal netSalary = totalEarnings - totalDeductions;
                        //decimal salik = 0;
                        //decimal additions = 0;
                        //decimal otherDedactions = 0;
                        dgvSalarySheet.Rows.Add(reader["SN"].ToString(), empCode, reader["name"].ToString(),
                       reader["PositionName"].ToString(), reader["WorkDays"].ToString(), absDays, totalBasicSalary,
                       allowance, other, additions.ToString("N2"), totalEarnings, totalLoan, totalDelay, totalWorkAbsence, salik.ToString("N2"), otherDedactions.ToString("N2"),
                       totalDeductions, netSalary, "", "", "");
                        string[] numericColumns = {
                                                "Basic Salary", "Allowance", "Other", "Total Additions",
                                                "Emp Advance", "Delay", "Absence", "FinesSalik",
                                                "Total Deductions", "Net Salary","WorkAbsence","TotalDeductions","NetSalary",
                                            };

                        foreach (string colName in numericColumns)
                        {
                            if (dgvSalarySheet.Columns.Contains(colName))
                            {
                                dgvSalarySheet.Columns[colName].DefaultCellStyle.Format = "N2";
                            }
                        }
                    }
                }
            }
            refQuery = @"SELECT max(WorkDate) AS dated FROM tbl_attendancesheet 
            WHERE YEAR(WorkDate) = @year AND MONTH(WorkDate) = @month 
            LIMIT 1";
            refResult = ClinicDatabase.ExecuteScalar(refQuery,
            ClinicDatabase.CreateParameterNew("@year", selectedYear),
            ClinicDatabase.CreateParameterNew("@month", selectedMonth));
            string dated = (refResult != null && refResult != DBNull.Value) ? refResult.ToString() : "";
            if (dated!="")
            {
                DateTime workDate = Convert.ToDateTime(dated);
                dated = workDate.ToString("dd-MM-yyyy");
            }
            lblDate.Text = dated;
        }
        private int GetAbsenceDays(string empCode, int year, int month)
        {
            string query = @"SELECT COUNT(attendance_salary_id) AS count FROM tbl_attendancesheet WHERE YEAR(WorkDate) = @year AND MONTH(WorkDate) = @month AND attendance_salary_id = @code AND status = 'A'";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query,
                ClinicDatabase.CreateParameterNew("year", year),
                ClinicDatabase.CreateParameterNew("month", month),
                ClinicDatabase.CreateParameterNew("code", empCode)))
            {
                return reader.Read() && reader["count"] != DBNull.Value ? int.Parse(reader["count"].ToString()) : 0;
            }
        }
        private decimal GetTotalLoan(string empCode, int year, int month)
        {
            string query = @"SELECT SUM(amount) AS totalLoan FROM tbl_loan WHERE YEAR(loandates) = @year AND MONTH(loandates) = @month AND employeeId = @code";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query,
                ClinicDatabase.CreateParameterNew("year", year),
                ClinicDatabase.CreateParameterNew("month", month),
                ClinicDatabase.CreateParameterNew("code", empCode)))
            {
                return reader.Read() && reader["totalLoan"] != DBNull.Value ? decimal.Parse(reader["totalLoan"].ToString()) : 0;
            }
        }
        private decimal GetTotalDeduction(string empCode, int year, int month, string status)
        {
            string query = @"SELECT total_absence, total_delay, total_loan FROM tbl_attendance_salary 
                     WHERE YEAR(date) = @year AND MONTH(date) = @month AND emp_code = @code";

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query,
                ClinicDatabase.CreateParameterNew("year", year),
                ClinicDatabase.CreateParameterNew("month", month),
                ClinicDatabase.CreateParameterNew("code", empCode)))
            {
                if (reader.Read())
                {
                    decimal totalAbsence = reader["total_absence"] != DBNull.Value ? decimal.Parse(reader["total_absence"].ToString()) : 0;
                    decimal totalDelay = reader["total_delay"] != DBNull.Value ? decimal.Parse(reader["total_delay"].ToString()) : 0;
                    decimal totalLoan = reader["total_loan"] != DBNull.Value ? decimal.Parse(reader["total_loan"].ToString()) : 0;

                    return totalAbsence + totalDelay + totalLoan;
                }
                return 0;
            }
        }
        private decimal GetTotalDelay(string empCode, int year, int month)
        {
            string query = @"SELECT total_delay FROM tbl_attendance_salary 
                     WHERE YEAR(date) = @year AND MONTH(date) = @month AND emp_code = @code";

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query,
                ClinicDatabase.CreateParameterNew("year", year),
                ClinicDatabase.CreateParameterNew("month", month),
                ClinicDatabase.CreateParameterNew("code", empCode)))
            {
                if (reader.Read())
                {
                    return reader["total_delay"] != DBNull.Value ? decimal.Parse(reader["total_delay"].ToString()) : 0;
                }
                return 0;
            }
        }
        private decimal GetTotalAbsence(string empCode, int year, int month)
        {
            string query = @"SELECT total_absence FROM tbl_attendance_salary 
                     WHERE YEAR(date) = @year AND MONTH(date) = @month AND emp_code = @code";

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query,
                ClinicDatabase.CreateParameterNew("year", year),
                ClinicDatabase.CreateParameterNew("month", month),
                ClinicDatabase.CreateParameterNew("code", empCode)))
            {
                if (reader.Read())
                {
                    return reader["total_absence"] != DBNull.Value ? decimal.Parse(reader["total_absence"].ToString()) : 0;
                }
                return 0;
            }
        }
        private decimal GetSalaryAdjustment(string empCode, int refId, string adjustmentType)
        {
            string query = @"SELECT SUM(amount) FROM tbl_salary_adjustments 
                     WHERE code = @code 
                     AND ref_id = @id and adjustment_type=@adjustment_type";

            object result = ClinicDatabase.ExecuteScalar(query,
                ClinicDatabase.CreateParameterNew("@code", empCode),
                ClinicDatabase.CreateParameterNew("@adjustment_type", adjustmentType),
                ClinicDatabase.CreateParameterNew("@id", refId));

            return (result != null && result != DBNull.Value) ? Convert.ToDecimal(result) : 0m;
        }
        private void dgvSalarySheet_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dgvSalarySheet.Rows[e.RowIndex];
            RecalculateRowTotals(row);
        }

        private void cmbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMonth.SelectedItem != null && cmbYear.SelectedItem != null)
                BindGridView();
        }
        string refCode = "0";
        private void dgvSalarySheet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string column = dgvSalarySheet.Columns[e.ColumnIndex].Name;
            if (column == "Additions" || column == "Salik" || column == "othersDeductions")
            {
                var row = dgvSalarySheet.Rows[e.RowIndex];
                string empName = row.Cells["EmployeeName"].Value.ToString();
                string empCode = row.Cells["EmployeeID"].Value.ToString();

                using (frmAmountAdjustments form = new frmAmountAdjustments(empName, empCode, column,refCode))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Update the grid cell with the returned total amount
                        row.Cells[column].Value = form.TotalAmount;

                        // Optionally: trigger Net Salary update here again
                        dgvSalarySheet_CellValueChanged(null, new DataGridViewCellEventArgs(e.ColumnIndex, e.RowIndex));
                    }
                }
            }
        }
        private void RecalculateRowTotals(DataGridViewRow row)
        {
            try
            {
                decimal basicSalary = ParseDecimal(row.Cells["BasicSalary"].Value);
                decimal allowance = ParseDecimal(row.Cells["Allowance"].Value);
                decimal other = ParseDecimal(row.Cells["Other"].Value);
                decimal additions = ParseDecimal(row.Cells["Additions"].Value);

                decimal totalAdditions = basicSalary + allowance + other + additions;
                row.Cells["TotalAdditions"].Value = totalAdditions;

                decimal loan = ParseDecimal(row.Cells["EmployeeAdvance"].Value);
                decimal delay = ParseDecimal(row.Cells["Delay"].Value);
                decimal absence = ParseDecimal(row.Cells["WorkAbsence"].Value);
                decimal salik = ParseDecimal(row.Cells["Salik"].Value);
                decimal others = ParseDecimal(row.Cells["othersDeductions"].Value);

                decimal totalDeductions = loan + delay + absence + salik + others;
                row.Cells["TotalDeductions"].Value = totalDeductions;

                decimal net = totalAdditions - totalDeductions;
                row.Cells["NetSalary"].Value = net;

                row.Cells["NetSalary"].Style.ForeColor = net < 0 ? Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(42)))), ((int)(((byte)(83))))) : Color.Black;
            }
            catch
            {
                //
            }
        }
        private decimal ParseDecimal(object val)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()))
                return 0m;

            decimal result;
            if (decimal.TryParse(val.ToString(), out result))
                return result;

            return 0m; // fallback
        }

        private void cmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMonth.SelectedItem != null && cmbYear.SelectedItem != null)
                BindGridView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int selectedYear = int.Parse(cmbYear.SelectedItem.ToString());
            int selectedMonth = int.Parse(cmbMonth.SelectedItem.ToString());

            foreach (DataGridViewRow row in dgvSalarySheet.Rows)
            {
                if (row.IsNewRow) continue;

                string empCode = row.Cells["EmployeeID"].Value?.ToString();
                decimal netSalary = ParseDecimal(row.Cells["NetSalary"].Value);

                string query = @"UPDATE tbl_attendance_salary 
                         SET net_salary = @net, `change` = @change 
                         WHERE emp_code = @code 
                         AND YEAR(date) = @year 
                         AND MONTH(date) = @month";

                ClinicDatabase.ExecuteNonQuery(query,
                    ClinicDatabase.CreateParameterNew("@net", netSalary),
                    ClinicDatabase.CreateParameterNew("@change", netSalary),
                    ClinicDatabase.CreateParameterNew("@code", empCode),
                    ClinicDatabase.CreateParameterNew("@year", selectedYear),
                    ClinicDatabase.CreateParameterNew("@month", selectedMonth)
                );
                ////Utilities.LogAudit(-1, "Update Salary", "Salary", 0, $"Updated Salary for {empCode} for {selectedMonth}-{selectedYear} to {netSalary}");
            }
            ////Utilities.LogAudit(-1, "Update Salary Sheet", "Salary Sheet", 0, $"Updated Salary Sheet for {selectedMonth}-{selectedYear}");
            MessageBox.Show("Salary sheet updated successfully.");
        }
    }
}
