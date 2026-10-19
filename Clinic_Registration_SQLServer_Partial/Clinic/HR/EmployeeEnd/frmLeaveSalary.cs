using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmLeaveSalary : Form
    {

        public frmLeaveSalary(int id = 0)
        {
            InitializeComponent();

           // LocalizationManager.LocalizeForm(this);

            ApplyLanguageLayout();
        }

        private void frmLeaveSalary_Load(object sender, EventArgs e)
        {
            bindLeaveSalary();
            //AppTheme.DefaultFont(dgvLeaveSalary);

            // Apply AFTER grid/data loading also.
            ApplyLanguageLayout();
        }
        public void ApplyLanguageLayout()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            SuspendLayout();
            pnlPageHeader.SuspendLayout();
            pnlGridHeader.SuspendLayout();

            try
            {
                // Do not mirror complete form.
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                if (isArabic)
                {
                    // =====================================================
                    // ARABIC
                    // =====================================================

                    Text = "راتب الإجازة";

                    headerUC1.FormText = "راتب الإجازة";
                    headerUC1.RightToLeft = RightToLeft.Yes;

                    // =====================================================
                    // MAIN PAGE HEADER
                    // Icon and heading -> RIGHT
                    // =====================================================

                    lblTitle.Text = "راتب الإجازة";
                    lblSubtitle.Text =
                        "مراجعة أرصدة ومدفوعات راتب إجازة الموظفين";

                    pnlIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblSubtitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblTitle.RightToLeft = RightToLeft.Yes;
                    lblSubtitle.RightToLeft = RightToLeft.Yes;

                    lblTitle.TextAlign =
                        ContentAlignment.MiddleRight;

                    lblSubtitle.TextAlign =
                        ContentAlignment.MiddleRight;

                    // Icon at far RIGHT
                    pnlIcon.Location = new Point(
                        pnlPageHeader.ClientSize.Width
                            - pnlPageHeader.Padding.Right
                            - pnlIcon.Width,
                        22);

                    // Title to LEFT of icon
                    lblTitle.Location = new Point(
                        pnlIcon.Left
                            - 20
                            - lblTitle.PreferredWidth,
                        20);

                    // Subtitle to LEFT of icon
                    lblSubtitle.Location = new Point(
                        pnlIcon.Left
                            - 20
                            - lblSubtitle.PreferredWidth,
                        65);

                    // =====================================================
                    // GRID SECTION HEADER
                    // Move icon/title/subtitle to RIGHT
                    // =====================================================

                    lblGridTitle.Text = "تفاصيل راتب الإجازة";

                    lblGridSubtitle.Text =
                        "استحقاق راتب إجازة الموظف والرصيد المتبقي";

                    lblGridIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblGridTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblGridSubtitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblGridTitle.RightToLeft = RightToLeft.Yes;
                    lblGridSubtitle.RightToLeft = RightToLeft.Yes;

                    lblGridTitle.TextAlign =
                        ContentAlignment.MiddleRight;

                    lblGridSubtitle.TextAlign =
                        ContentAlignment.MiddleRight;

                    // Grid icon -> right
                    lblGridIcon.Location = new Point(
                        pnlGridHeader.ClientSize.Width
                            - 12
                            - lblGridIcon.Width,
                        15);

                    // Grid title -> left of icon
                    lblGridTitle.Location = new Point(
                        lblGridIcon.Left
                            - 8
                            - lblGridTitle.PreferredWidth,
                        12);

                    // Grid subtitle -> left of icon
                    lblGridSubtitle.Location = new Point(
                        lblGridIcon.Left
                            - 8
                            - lblGridSubtitle.PreferredWidth,
                        34);

                    // =====================================================
                    // DATAGRIDVIEW
                    // =====================================================

                    dgvLeaveSalary.RightToLeft = RightToLeft.Yes;

                    dgvLeaveSalary.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    dgvLeaveSalary.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Code",
                        "رمز الموظف");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Name",
                        "اسم الموظف");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Leave Salary Days",
                        "أيام راتب الإجازة");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Leave Salary Amount",
                        "مبلغ راتب الإجازة");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Used Days",
                        "أيام الإجازة المستخدمة");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Remaining Days",
                        "أيام الإجازة المتبقية");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Received Amount",
                        "المبلغ المستلم");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Remaining Amount",
                        "المبلغ المتبقي");
                }
                else
                {
                    // =====================================================
                    // ENGLISH - RESTORE ORIGINAL DESIGN
                    // =====================================================

                    Text = "Leave Salary";

                    headerUC1.FormText = "Leave Salary";
                    headerUC1.RightToLeft = RightToLeft.No;

                    // =====================================================
                    // MAIN PAGE HEADER
                    // =====================================================

                    lblTitle.Text = "Leave Salary";

                    lblSubtitle.Text =
                        "Review employee leave salary balances and payments.";

                    pnlIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblSubtitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblTitle.RightToLeft = RightToLeft.No;
                    lblSubtitle.RightToLeft = RightToLeft.No;

                    lblTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblSubtitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    // Exact Designer positions
                    pnlIcon.Location =
                        new Point(34, 22);

                    lblTitle.Location =
                        new Point(116, 20);

                    lblSubtitle.Location =
                        new Point(119, 65);

                    // =====================================================
                    // GRID HEADER - RESTORE ORIGINAL
                    // =====================================================

                    lblGridTitle.Text =
                        "Leave Salary Details";

                    lblGridSubtitle.Text =
                        "Employee leave salary entitlement and remaining balance";

                    lblGridIcon.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblGridTitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblGridSubtitle.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblGridTitle.RightToLeft =
                        RightToLeft.No;

                    lblGridSubtitle.RightToLeft =
                        RightToLeft.No;

                    lblGridTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblGridSubtitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    // Exact Designer positions
                    lblGridIcon.Location =
                        new Point(12, 15);

                    lblGridTitle.Location =
                        new Point(51, 12);

                    lblGridSubtitle.Location =
                        new Point(52, 34);

                    // =====================================================
                    // DATAGRIDVIEW
                    // =====================================================

                    dgvLeaveSalary.RightToLeft =
                        RightToLeft.No;

                    dgvLeaveSalary.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    dgvLeaveSalary.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Code",
                        "Employee Code");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Name",
                        "Employee Name");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Leave Salary Days",
                        "Leave Salary Days");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Leave Salary Amount",
                        "Leave Salary Amount");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Used Days",
                        "L.S Used Days");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Remaining Days",
                        "L.S Remaining Days");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Received Amount",
                        "L.S Received Amount");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "L.S Remaining Amount",
                        "L.S Remaining Amount");
                }

                headerUC1.BringToFront();
            }
            finally
            {
                pnlGridHeader.ResumeLayout(true);
                pnlPageHeader.ResumeLayout(true);

                ResumeLayout(true);
                PerformLayout();
            }
        }
      
      
        private void SetColumnHeader(
            DataGridView dgv,
            string columnName,
            string text)
        {
            if (dgv.Columns.Contains(columnName))
                dgv.Columns[columnName].HeaderText = text;
        }
        private void bindLeaveSalary()
        {
          
                DataTable dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                    CODE AS 'Employee Code',
                                    NAME AS 'Employee Name',
                                    SUM(leave_days) AS 'Leave Salary Days',
                                    SUM(credit) AS 'Leave Salary Amount',
                                    SUM(debit) / (SUM(credit) / SUM(leave_days)) AS 'L.S Used Days',
                                    SUM(leave_days) - (SUM(debit) / (SUM(credit) / SUM(leave_days))) AS 'L.S Remaining Days',
                                    SUM(debit) AS 'L.S Received Amount',
                                    SUM(credit) - SUM(debit) AS 'L.S Remaining Amount'
                                FROM tbl_leave_salary
                                GROUP BY CODE,name;
                                ");
            dgvLeaveSalary.DataSource = dt;

            dgvLeaveSalary.Columns["Employee Code"].Width = 100;
            dgvLeaveSalary.Columns["Employee Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvLeaveSalary.Columns["Leave Salary Days"].Width = 120;
            dgvLeaveSalary.Columns["Leave Salary Days"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["Leave Salary Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvLeaveSalary.Columns["Leave Salary Amount"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["L.S Used Days"].Width = 100;
            dgvLeaveSalary.Columns["L.S Used Days"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvLeaveSalary.Columns["L.S REMAINING DAYS"].Width = 100;
            dgvLeaveSalary.Columns["L.S REMAINING DAYS"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["L.S RECEIVED AMOUNT"].Width = 150;
            dgvLeaveSalary.Columns["L.S RECEIVED AMOUNT"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["L.S REMAINING AMOUNT"].Width = 160;
            dgvLeaveSalary.Columns["L.S REMAINING AMOUNT"].DefaultCellStyle.Format = "N2";
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvLeaveSalary);
        }

        private void EmpLeaveSalary()
        {
            DataTable dt;
            string query = @"
        WITH LastMonthAttendance AS (
            -- Get the most recent month worked for each employee
            SELECT 
                EmpID, YEAR(WorkDate) AS YearWorked, MONTH(WorkDate) AS MonthWorked
            FROM tbl_attendancesheet
            WHERE WorkDate = (
                SELECT MAX(WorkDate)
                FROM tbl_attendancesheet sub
                WHERE sub.EmpID = tbl_attendancesheet.EmpID
            )
            GROUP BY EmpID, YearWorked, MonthWorked
        )
        SELECT 
            e.code AS `EmpID`, 
            e.EmployeeName AS `EmpName`, 
            e.WorkDays AS `WorkDays`, 
            COALESCE(latest_attendance.`Absent Days`, 0) AS `Absent Days`, 
            e.BasicSalary AS `Basic Salary`, 
            (e.HousingAllowance + e.TransportationAllowance) AS `Allowance`, 
            e.Other AS `Others`, 
            (e.BasicSalary + e.HousingAllowance + e.TransportationAllowance + e.Other) AS `Total Additions`, 
            ((e.WorkDays - COALESCE(latest_attendance.`Absent Days`, 0)) / 365 * 30) AS `Leave Salary Days`, 
            (e.BasicSalary * 12 / 365 * ((e.WorkDays - COALESCE(latest_attendance.`Absent Days`, 0)) / 365 * 30)) AS `Leave Salary Amount`,
            
            -- Vacation count for the last month worked
            COUNT(CASE WHEN a.Status = 'v' AND YEAR(a.WorkDate) = lma.YearWorked AND MONTH(a.WorkDate) = lma.MonthWorked THEN 1 END) AS vacation_count,

            -- Total vacation days from settings
            (SELECT COUNT(*) FROM tbl_setting_attendance WHERE state = 0) AS total_vacation_days,

            -- Extra vacation days (L.S USED DAYS)
            GREATEST(
                COUNT(CASE WHEN a.Status = 'v' AND YEAR(a.WorkDate) = lma.YearWorked AND MONTH(a.WorkDate) = lma.MonthWorked THEN 1 END) - 
                (SELECT COUNT(*) FROM tbl_setting_attendance WHERE state = 0), 0
            ) AS `L.S Used Days`,

            -- L.S REMAINING DAYS
            ((e.WorkDays - COALESCE(latest_attendance.`Absent Days`, 0)) / 365 * 30) - 
            GREATEST(
                COUNT(CASE WHEN a.Status = 'v' AND YEAR(a.WorkDate) = lma.YearWorked AND MONTH(a.WorkDate) = lma.MonthWorked THEN 1 END) - 
                (SELECT COUNT(*) FROM tbl_setting_attendance WHERE state = 0), 0
            ) AS `L.S Remaining Days`,

            -- L.S RECEIVED AMOUNT (Kept as 0 for now)
            0 AS `L.S Received Amount`,

            -- L.S REMAINING AMOUNT = Leave Salary Amount - L.S RECEIVED AMOUNT
            (e.BasicSalary * 12 / 365 * ((e.WorkDays - COALESCE(latest_attendance.`Absent Days`, 0)) / 365 * 30)) - 0 AS `L.S Remaining Amount`
            
        FROM tbl_employee e
        INNER JOIN tbl_attendancesheet a 
            ON e.code = a.EmpID
        INNER JOIN LastMonthAttendance lma 
            ON a.EmpID = lma.EmpID
            AND YEAR(a.WorkDate) = lma.YearWorked
            AND MONTH(a.WorkDate) = lma.MonthWorked
        LEFT JOIN (
            -- Get the most recent attendance data per employee (most recent attendance date)
            SELECT 
                a.EmpID, 
                SUM(CASE WHEN a.Status = 'A' THEN 1 ELSE 0 END) AS `Absent Days`
            FROM tbl_attendancesheet a
            WHERE a.WorkDate = (
                SELECT MAX(sub.WorkDate)
                FROM tbl_attendancesheet sub
                WHERE sub.EmpID = a.EmpID
            )
            GROUP BY a.EmpID
        ) AS latest_attendance 
            ON e.code = latest_attendance.EmpID

        GROUP BY e.code, e.EmployeeName, e.WorkDays, e.BasicSalary, 
                 e.HousingAllowance, e.TransportationAllowance, 
                 e.Other, latest_attendance.`Absent Days`, 
                 lma.YearWorked, lma.MonthWorked;";

            dt = ClinicDatabase.ExecuteDataTable(query, new MySqlParameter[] { });
            dgvLeaveSalary.DataSource = dt;

            // Adjust column widths dynamically for a better UI experience
            dgvLeaveSalary.Columns["EmpID"].Width = 100;
            dgvLeaveSalary.Columns["EmpName"].MinimumWidth = 180;
            dgvLeaveSalary.Columns["EmpName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvLeaveSalary.Columns["WorkDays"].Visible = false;
            dgvLeaveSalary.Columns["Absent Days"].Visible = false;
            dgvLeaveSalary.Columns["Basic Salary"].Visible = false;
            dgvLeaveSalary.Columns["Allowance"].Visible = false;
            dgvLeaveSalary.Columns["Others"].Visible = false;
            dgvLeaveSalary.Columns["Total Additions"].Visible = false;
            dgvLeaveSalary.Columns["Leave Salary Days"].Width = 140;
            dgvLeaveSalary.Columns["Leave Salary Days"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["Leave Salary Amount"].Width = 160;
            dgvLeaveSalary.Columns["Leave Salary Amount"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["L.S USED DAYS"].Width = 140;
            dgvLeaveSalary.Columns["L.S REMAINING DAYS"].Width = 140;
            dgvLeaveSalary.Columns["L.S REMAINING DAYS"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["L.S RECEIVED AMOUNT"].Width = 140;
            dgvLeaveSalary.Columns["L.S RECEIVED AMOUNT"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["L.S REMAINING AMOUNT"].Width = 160;
            dgvLeaveSalary.Columns["L.S REMAINING AMOUNT"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["vacation_count"].Visible = false;
            dgvLeaveSalary.Columns["total_vacation_days"].Visible = false;

            // Apply professional styling
            dgvLeaveSalary.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
            dgvLeaveSalary.DefaultCellStyle.Font = new Font("Times New Roman", 9);
            dgvLeaveSalary.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSlateGray; // Header background color
            dgvLeaveSalary.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Header text color
            dgvLeaveSalary.EnableHeadersVisualStyles = false;

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvLeaveSalary);
        }

        private void dgvLeaveSalary_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvLeaveSalary.Rows.Count != 0)
            {
                frmMain.openChildForm(new frmLeaveSalaryStatement(int.Parse(dgvLeaveSalary.CurrentRow.Cells["Employee Code"].Value.ToString())));
            }
        }
    }
}
