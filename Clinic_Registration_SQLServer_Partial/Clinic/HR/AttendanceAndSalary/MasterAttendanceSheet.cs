using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterAttendanceSheet : Form
    {
        string codest = "";
        int code = 0;
        int id = 0;
        int workDays, totalAbsence = 0;
        decimal dailyDeduction, basicSalary, housingAllowance, transportationAllowance, otherAllowance, deductionAmount = 0, totalDeduction = 0, deductionRate = 0, totalDelayMinutes = 0;
        bool isAllDataSucceed = true;
        DateTime ContractIssuse;
        TimeSpan defaultTimeIn = TimeSpan.Zero;
        bool isManuaEntry = false;

        private Dictionary<string, DataTable> loadedExcelTables = new Dictionary<string, DataTable>();

        // REQUIRED for Activator.CreateInstance
        public MasterAttendanceSheet() : this(0)
        {
        }
        public MasterAttendanceSheet(int id = 0)
        {
            InitializeComponent();
            this.Resize += MasterAttendanceSheet_Resize;
            // Reassert the custom title bar after all controls have been laid out.
            this.Shown += MasterAttendanceSheet_Shown;
            LocalizationManager.LocalizeForm(this);

            this.id = id;

            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
            ConfigureAttendanceClosingHeader(isArabic);
        }

        // Keep the existing HeaderUC as the dark-blue title bar for this page.
        // The control itself owns its minimize/maximize/close button behavior.

        private void ConfigureAttendanceClosingHeader(bool isArabic)
        {
            if (headerUC1 == null || headerUC1.IsDisposed)
                return;

            // Change appearance only. Keep the existing designer layout.
            headerUC1.BackColor = Color.FromArgb(29, 53, 84);
            headerUC1.ForeColor = Color.White;

            headerUC1.FormText = isArabic
                ? "سجل الحضور الرئيسي"
                : "Master Attendance Sheet";

            headerUC1.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            headerUC1.Invalidate();
            headerUC1.Refresh();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Form parentForm = this.FindForm();

            if (parentForm != null)
                parentForm.Close();
        }

        // Find the four toolbar buttons by their displayed text so this code
        // does not depend on designer field names for the upload button.
        private void SetAttendanceToolbarButtonsVisible()
        {
            // Intentionally do not reposition, resize, reparent, or restyle controls.
            // The original WinForms Designer layout must remain unchanged.
        }

        // Restore the custom title bar and toolbar whenever the page is shown.
        private void RestoreAttendanceHeader()
        {
            bool isArabic = IsArabicLanguage();
            ConfigureAttendanceClosingHeader(isArabic);
            ApplyModernHeaderLanguage(isArabic);

            // Refresh only; do not change Parent, Dock, Location, Size, Anchor or Z-order.
            if (headerUC1 != null && !headerUC1.IsDisposed)
                headerUC1.Refresh();
        }

        private bool IsArabicLanguage()
        {
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            return language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        }

        private string L(string english, string arabic)
        {
            return IsArabicLanguage() ? arabic : english;
        }

        // Translate visible captions only. No control bounds, sizes, anchors,
        // colors, fonts, docking, parent containers, or positions are changed.
        private void ApplyArabicAttendanceText(Control parent, bool isArabic)
        {
            if (parent == null) return;

            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Master Attendance Sheet", "سجل الحضور الرئيسي" },
                { "Import, review and manage employee attendance records.", "استيراد ومراجعة وإدارة سجلات حضور الموظفين" },
                { "Attendance Saved Before", "الحضور المحفوظ سابقًا" },
                { "Add Manual Attendance", "إضافة حضور يدوي" },
                { "Download Sample Excel Sheet", "تنزيل نموذج ملف Excel" },
                { "Save Attendance", "حفظ الحضور" },
                { "Upload Current Employee", "رفع بيانات الموظف الحالي" },
                { "Sheet", "الورقة" },
                { "Employee Code", "رمز الموظف" },
                { "Employee Name", "اسم الموظف" },
                { "Emp ID", "رقم الموظف" },
                { "Emp Name", "اسم الموظف" },
                { "Work Date", "تاريخ العمل" },
                { "Time In", "وقت الحضور" },
                { "Time Out", "وقت الانصراف" },
                { "Day Of Week", "يوم الأسبوع" },
                { "Status", "الحالة" },
                { "Employee", "الموظف" },
                { "Month", "الشهر" },
                { "Year", "السنة" },
                { "Select Employee", "اختر الموظف" },
                { "Select Month", "اختر الشهر" },
                { "Records #", "عدد السجلات:" },
                { "Manual Attendance", "الحضور اليدوي" },
                { "Attendance", "الحضور" },
                { "Payroll", "مسير الرواتب" },
                { "Save", "حفظ" },
                { "Cancel", "إلغاء" },
                { "Close", "إغلاق" },
                { "Browse", "استعراض" },
                { "Upload", "رفع" },
                { "Download", "تنزيل" },
                { "Total", "الإجمالي" },
                { "Present", "حاضر" },
                { "Absent", "غائب" },
                { "Late", "متأخر" }
            };

            foreach (Control control in parent.Controls)
            {
                string currentText = (control.Text ?? string.Empty).Trim();
                if (isArabic)
                {
                    if (translations.TryGetValue(currentText, out string translated))
                        control.Text = translated;
                }
                else
                {
                    foreach (var item in translations)
                    {
                        if (string.Equals(item.Value, currentText, StringComparison.OrdinalIgnoreCase))
                        {
                            control.Text = item.Key;
                            break;
                        }
                    }
                }

                if (control is Label || control is Button || control is TextBox || control is ComboBox)
                    control.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;

                if (control.HasChildren)
                    ApplyArabicAttendanceText(control, isArabic);
            }
        }

        private void TranslateAttendanceGridHeaders(bool isArabic)
        {
            TranslateGrid(dgvAttendance, isArabic);
            TranslateGrid(guna2DataGridView1, isArabic);
        }

        private void TranslateGrid(DataGridView grid, bool isArabic)
        {
            if (grid == null) return;
            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Emp ID", "رقم الموظف" },
                { "Emp Name", "اسم الموظف" },
                { "Work Date", "تاريخ العمل" },
                { "Time In", "وقت الحضور" },
                { "Time Out", "وقت الانصراف" },
                { "Day Of Week", "يوم الأسبوع" },
                { "Status", "الحالة" },
                { "Employee Code", "رمز الموظف" },
                { "Employee Name", "اسم الموظف" },
                { "Records #", "عدد السجلات" }
            };
            foreach (DataGridViewColumn column in grid.Columns)
            {
                string original = column.Name;
                if (isArabic && translations.TryGetValue(original, out string arabic))
                    column.HeaderText = arabic;
                else if (!isArabic)
                    column.HeaderText = original;
            }
        }

        public void ApplyLanguageLayout(bool isArabic)
        {
            // Localization only. Do not modify the English designer layout.
            RightToLeft = RightToLeft.No;
            RightToLeftLayout = false;

            Text = isArabic ? "سجل الحضور الرئيسي" : "Master Attendance Sheet";
            btnAttendanceReport.Text = isArabic ? "الحضور المحفوظ سابقًا" : "Attendance Saved Before";
            btnAddManualAttendance.Text = isArabic ? "إضافة حضور يدوي" : "Add Manual Attendance";
            DownloadExcelSheet.Text = isArabic ? "تنزيل نموذج ملف Excel" : "Download Sample Excel Sheet";
            btnSave.Text = isArabic ? "حفظ الحضور" : "Save Attendance";

            ConfigureAttendanceClosingHeader(isArabic);
            ApplyModernHeaderLanguage(isArabic);
            ApplyArabicAttendanceText(this, isArabic);
            TranslateAttendanceGridHeaders(isArabic);

            // Change grid reading direction only; keep all columns and dimensions intact.
            foreach (DataGridView grid in new[] { dgvAttendance, guna2DataGridView1 })
            {
                if (grid == null) continue;
                grid.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                grid.ColumnHeadersDefaultCellStyle.Alignment = isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
                grid.DefaultCellStyle.Alignment = isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
            }
        }
        private void ApplyModernHeaderLanguage(bool isArabic)
        {
            // Text and reading direction only. Keep the designer's exact positions,
            // sizes, anchors, icon placement, fonts and colors.
            if (lblModernTitle != null)
            {
                lblModernTitle.Text = isArabic ? "سجل حضور الموظفين" : "Master Attendance Sheet";
                lblModernTitle.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                lblModernTitle.TextAlign = isArabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            }

            if (lblModernSubtitle != null)
            {
                lblModernSubtitle.Text = isArabic
                    ? "استيراد ومراجعة وإدارة سجلات حضور الموظفين"
                    : "Import, review and manage employee attendance records.";
                lblModernSubtitle.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                lblModernSubtitle.TextAlign = isArabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            }
        }

        private void MasterAttendanceSheet_Shown(object sender, EventArgs e)
        {
            // The Shown event runs after the designer and child panels have completed
            // their initial layout, so the custom header is visible from first display.
            RestoreAttendanceHeader();

            // Run once more at the end of the current UI message to override any
            // initial z-order changes made while the form was being displayed.
            if (!IsDisposed && IsHandleCreated)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    if (!IsDisposed)
                        RestoreAttendanceHeader();
                }));
            }
        }

        private void MasterAttendanceSheet_Resize(
     object sender,
     EventArgs e)
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            ApplyModernHeaderLanguage(isArabic);
            ConfigureAttendanceClosingHeader(isArabic);
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            pnlData.Visible = true;
            pnlAddData.Visible = false;

            // Showing the upload/data panel can cover the custom title bar.
            // Reassert the header after changing panel visibility.
            RestoreAttendanceHeader();

            dgvAttendance.DataSource = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = L("Excel Files (*.xlsx)|*.xlsx", "ملفات Excel (*.xlsx)|*.xlsx");
                openFileDialog.Title = L("Select Excel File", "اختر ملف Excel");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    LoadExcelSheets(filePath);
                    RestoreAttendanceHeader();
                    if (dgvAttendance.Rows.Count == 0)
                        isAllDataSucceed = false;
                    else if (!chkCorrectDataForEmpNameEmpCode())
                        isAllDataSucceed = false;
                    else if (!chkExistingEmployee())
                        isAllDataSucceed = false;
                    else if (!CheckAllDaysIncluded(dgvAttendance, DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value.ToString()).Year, DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value.ToString()).Month))
                        isAllDataSucceed = false;
                }
            }
        }
        private void LoadExcelSheets(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show(L("The selected file does not exist.", "الملف المحدد غير موجود."), L("Error", "خطأ"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                cmbSheet.Items.Clear();
                dgvAttendance.DataSource = null;
                dgvAttendance.Rows.Clear();
                dgvAttendance.Columns.Clear();
                loadedExcelTables.Clear();
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
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

                        RemoveEmptyColumns(dataTable);
                        loadedExcelTables[sheet.Name] = dataTable;

                        dgvAttendance.DataSource = dataTable;

                        dgvAttendance.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        cmbSheet.Items.Add(sheet.Name);
                        if (dgvAttendance.Rows.Count == 0)
                            return;
                        pnlData.Visible = true;
                        RestoreAttendanceHeader();
                        lblCode.Text = dgvAttendance.Rows[0].Cells[0].Value.ToString();
                        lblName.Text = dgvAttendance.Rows[0].Cells[1].Value.ToString();
                        LocalizationManager.LocalizeDataGridViewHeaders(dgvAttendance);
                    }
                }

                if (cmbSheet.Items.Count > 0)
                    cmbSheet.SelectedIndex = 0;
                else
                    MessageBox.Show(L("No sheets found in the Excel file.", "لم يتم العثور على أوراق في ملف Excel."), L("Warning", "تنبيه"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        private bool CheckAllDaysIncluded(DataGridView dgv, int year, int month)
        {
            DateTime _d;
            int totalDaysInMonth = DateTime.DaysInMonth(year, month);
            var expectedDays = Enumerable.Range(1, totalDaysInMonth).ToList();

            var daysInDgv = dgv.Rows
                .Cast<DataGridViewRow>()
                .Select(row => row.Cells[2].Value)
                .Where(value => DateTime.TryParse(value?.ToString(), out _d))
                .Select(value => DateTime.Parse(value.ToString()))
                .Where(date => date.Year == year && date.Month == month)
                .Select(date => date.Day)
                .Distinct()
                .ToList();

            bool allDaysIncluded = !expectedDays.Except(daysInDgv).Any();

            if (!allDaysIncluded)
            {
                var missingDays = expectedDays.Except(daysInDgv);
                MessageBox.Show(L($"Missing days for {month}/{year}: {string.Join(", ", missingDays)}", $"الأيام غير الموجودة للشهر {month}/{year}: {string.Join(", ", missingDays)}"));
                return false;
            }
            return true;
        }
        // ===============================================
        // PROFESSIONAL PAYROLL SAVE SYSTEM (ERP VERSION)
        // ===============================================

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isAllDataSucceed)
            {
                MessageBox.Show(L("Check all required data first.", "يرجى التحقق من جميع البيانات المطلوبة أولاً."));
                return;
            }

            if (dgvAttendance.Rows.Count == 0)
                return;

            try
            {
                SavePayrollMonth();
                MessageBox.Show(L("Payroll saved successfully.", "تم حفظ مسير الرواتب بنجاح."));
            }
            catch (Exception ex)
            {
                MessageBox.Show(IsArabicLanguage() ? "حدث خطأ أثناء تنفيذ العملية." : ex.Message);
            }
        }

        // ===============================================
        // MAIN SAVE
        // ===============================================
        private void SavePayrollMonth()
        {
            using (MySqlConnection con = new MySqlConnection(ClinicDatabase.GetConnectionString()))
            {
                con.Open();

                using (MySqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        string empCode = dgvAttendance.Rows[0].Cells["Emp ID"].Value.ToString();
                        DateTime firstDate = Convert.ToDateTime(dgvAttendance.Rows[0].Cells["Work Date"].Value);

                        int month = firstDate.Month;
                        int year = firstDate.Year;

                        // check duplicate
                        object exists = ClinicDatabase.ExecuteScalarNew(con, tran,
                            @"SELECT id FROM tbl_attendance_salary
                              WHERE emp_code=@code
                              AND YEAR(date)=@year
                              AND MONTH(date)=@month LIMIT 1",
                            ClinicDatabase.CreateParameterNew("@code", empCode),
                            ClinicDatabase.CreateParameterNew("@year", year),
                            ClinicDatabase.CreateParameterNew("@month", month));

                        if (exists != null)
                            throw new Exception("Employee month already saved.");

                        // load employee
                        EmployeePayroll emp = LoadEmployeePayroll(con, tran, empCode);

                        // calculate
                        PayrollResult result = CalculatePayroll(emp);

                        // save header
                        int attendanceSalaryId = SavePayrollHeader(con, tran, empCode, firstDate, result);

                        // save detail rows
                        SaveAttendanceRows(con, tran, attendanceSalaryId);

                        // save LS
                        SaveLeaveSalary(con, tran, emp, result, firstDate);

                        // save EOS
                        SaveEOS(con, tran, emp, result, firstDate);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        // ===============================================
        // LOAD EMPLOYEE
        // ===============================================
        private EmployeePayroll LoadEmployeePayroll(MySqlConnection con, MySqlTransaction tran, string code)
        {
            EmployeePayroll emp = new EmployeePayroll();

            using (MySqlDataReader rd = ClinicDatabase.ExecuteReaderNew(con, tran, @"SELECT * FROM tbl_employee WHERE code=@code", ClinicDatabase.CreateParameterNew("@code", code)))
            {
                if (rd.Read())
                {
                    emp.Code = code;
                    emp.Basic = Convert.ToDecimal(rd["BasicSalary"]);
                    emp.Housing = Convert.ToDecimal(rd["HousingAllowance"]);
                    emp.Transport = Convert.ToDecimal(rd["TransportationAllowance"]);
                    emp.Other = Convert.ToDecimal(rd["Other"]);
                    emp.ContractDate = Convert.ToDateTime(rd["contractIssueDate"]);
                }
            }

            emp.GrossSalary = emp.Basic + emp.Housing + emp.Transport + emp.Other;

            return emp;
        }

        // ===============================================
        // CALCULATE PAYROLL
        // ===============================================
        private PayrollResult CalculatePayroll(EmployeePayroll emp)
        {
            PayrollResult r = new PayrollResult();

            int totalDays = 0;
            int absentDays = 0;
            int lateMinutes = 0;

            decimal lateRate = GetLateRate();

            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                if (row.IsNewRow) continue;

                string status = row.Cells["Status"].Value.ToString().ToUpper();

                totalDays++;

                if (status == "A")
                {
                    absentDays++;
                }

                if (status == "P")
                {
                    TimeSpan defaultIn = TimeSpan.Parse("08:00:00");
                    TimeSpan actualIn = TimeSpan.Parse(row.Cells["Time In"].Value.ToString() + ":00");

                    if (actualIn > defaultIn)
                        lateMinutes += (int)(actualIn - defaultIn).TotalMinutes;
                }
            }

            decimal dailyRate = emp.GrossSalary / totalDays;
            decimal absenceDeduction = dailyRate * absentDays;
            decimal lateDeduction = lateMinutes * lateRate;

            DateTime dt = Convert.ToDateTime(dgvAttendance.Rows[0].Cells["Work Date"].Value);

            decimal loan = GetEmployeeLoan(emp.Code, dt.Month, dt.Year);

            r.TotalDays = totalDays;
            r.AbsentDays = absentDays;
            r.LateMinutes = lateMinutes;
            r.AbsenceDeduction = Math.Round(absenceDeduction, 2);
            r.LateDeduction = Math.Round(lateDeduction, 2);
            r.Loan = loan;

            r.TotalDeduction = r.AbsenceDeduction + r.LateDeduction + r.Loan;
            r.NetSalary = emp.GrossSalary - r.TotalDeduction;

            return r;
        }

        // ===============================================
        // SAVE HEADER
        // ===============================================
        private int SavePayrollHeader(MySqlConnection con, MySqlTransaction tran,
            string empCode, DateTime dt, PayrollResult r)
        {
            object id = ClinicDatabase.ExecuteScalarNew(con, tran,
                            @"INSERT INTO tbl_attendance_salary
                            (
                                date,
                                emp_code,
                                absence_days,
                                total_absence,
                                delay_minutes,
                                total_delay,
                                total_loan,
                                net_salary,
                                pay,
                                `change`,
                                created_by,
                                created_date
                            )
                            VALUES
                            (
                                @date,@code,@abs,@totabs,@late,@totlate,@loan,@net,0,@net,@by,@cd
                            );
                            SELECT LAST_INSERT_ID();",
                            ClinicDatabase.CreateParameterNew("@date", dt),
                            ClinicDatabase.CreateParameterNew("@code", empCode),
                            ClinicDatabase.CreateParameterNew("@abs", r.AbsentDays),
                            ClinicDatabase.CreateParameterNew("@totabs", r.AbsenceDeduction),
                            ClinicDatabase.CreateParameterNew("@late", r.LateMinutes),
                            ClinicDatabase.CreateParameterNew("@totlate", r.LateDeduction),
                            ClinicDatabase.CreateParameterNew("@loan", r.Loan),
                            ClinicDatabase.CreateParameterNew("@net", r.NetSalary),
                            ClinicDatabase.CreateParameterNew("@by", -1),
                            ClinicDatabase.CreateParameterNew("@cd", DateTime.Now)
                        );

            return Convert.ToInt32(id);
        }

        // ===============================================
        // SAVE DETAIL
        // ===============================================
        private void SaveAttendanceRows(MySqlConnection con, MySqlTransaction tran, int salaryId)
        {
            DateTime parseDateR = DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString());
            int _month = parseDateR.Month;
            int _year = parseDateR.Year;

            string refr = (parseDateR.Year % 100) + $"{parseDateR.Month:D2}";
            object ref_no_result = ClinicDatabase.ExecuteScalar(@"SELECT Ref_Code FROM tbl_attendancesheet 
                  WHERE YEAR(WorkDate) = @year AND MONTH(WorkDate) = @month 
                  LIMIT 1",
            ClinicDatabase.CreateParameterNew("@year", _year),
            ClinicDatabase.CreateParameterNew("@month", _month));

            int ref_no;
            if (ref_no_result != null && ref_no_result != DBNull.Value && Convert.ToInt32(ref_no_result) > 0)
            {
                ref_no = Convert.ToInt32(ref_no_result);
            }
            else
            {
                object maxResult = ClinicDatabase.ExecuteScalar(@"SELECT MAX(Ref_Code) FROM tbl_attendancesheet");
                int max = (maxResult != null && maxResult != DBNull.Value) ? Convert.ToInt32(maxResult) : 600000;
                ref_no = max + 1;
            }

            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                if (row.IsNewRow) continue;

                ClinicDatabase.ExecuteNonQueryNew(con, tran,
                    @"INSERT INTO tbl_attendancesheet
                    (
                        attendance_salary_id,
                        code,
                        WorkDate,
                        TimeIn,
                        TimeOut,
                        DayOfWeek,
                        Status, 
                        Reference,
                        Ref_Code
                    )
                    VALUES
                    (
                        @id,@code,@dt,@in,@out,@day,@st,@Reference,@Ref_Code
                    )",
                    ClinicDatabase.CreateParameterNew("@id", salaryId),
                    ClinicDatabase.CreateParameterNew("@code", row.Cells["Emp ID"].Value),
                    ClinicDatabase.CreateParameterNew("@dt", row.Cells["Work Date"].Value),
                    ClinicDatabase.CreateParameterNew("@in", row.Cells["Time In"].Value),
                    ClinicDatabase.CreateParameterNew("@out", row.Cells["Time Out"].Value),
                    ClinicDatabase.CreateParameterNew("@day", row.Cells["Day Of Week"].Value),
                    ClinicDatabase.CreateParameterNew("@st", row.Cells["Status"].Value),
                    ClinicDatabase.CreateParameterNew("@Ref_Code", ref_no.ToString()),
                    ClinicDatabase.CreateParameterNew("@Reference", refr.ToString())
                );
            }
        }

        // ===============================================
        // SAVE LEAVE SALARY
        // ===============================================
        private void SaveLeaveSalary(MySqlConnection con, MySqlTransaction tran,
            EmployeePayroll emp, PayrollResult r, DateTime dt)
        {
            decimal leaveDays = (decimal)(r.TotalDays - r.AbsentDays) / 365m * 30m;
            decimal amount = emp.Basic * 12m / 365m * leaveDays;

            ClinicDatabase.ExecuteNonQueryNew(con, tran,
                @"INSERT INTO tbl_leave_salary
                (date,code,leave_days,credit,created_by,created_date)
                VALUES(@dt,@code,@days,@amt,@by,@cd)",
            ClinicDatabase.CreateParameterNew("@dt", dt),
            ClinicDatabase.CreateParameterNew("@code", emp.Code),
            ClinicDatabase.CreateParameterNew("@days", Math.Round(leaveDays, 2)),
            ClinicDatabase.CreateParameterNew("@amt", Math.Round(amount, 2)),
            ClinicDatabase.CreateParameterNew("@by", -1),
            ClinicDatabase.CreateParameterNew("@cd", DateTime.Now));
        }

        // ===============================================
        // SAVE EOS
        // ===============================================
        private void SaveEOS(MySqlConnection con, MySqlTransaction tran,
            EmployeePayroll emp, PayrollResult r, DateTime dt)
        {
            int years = (int)((DateTime.Now - emp.ContractDate).TotalDays / 365);

            decimal eosDays = years < 5
                ? ((decimal)(r.TotalDays - r.AbsentDays) / 365m * 21m)
                : ((decimal)(r.TotalDays - r.AbsentDays) / 365m * 30m);

            decimal amount = emp.Basic * 12m / 365m * eosDays;

            ClinicDatabase.ExecuteNonQueryNew(con, tran,
                @"INSERT INTO tbl_end_of_service
                (date,code,leave_days,credit,created_by,created_date)
                VALUES(@dt,@code,@days,@amt,@by,@cd)",

            ClinicDatabase.CreateParameterNew("@dt", dt),
            ClinicDatabase.CreateParameterNew("@code", emp.Code),
            ClinicDatabase.CreateParameterNew("@days", Math.Round(eosDays, 2)),
            ClinicDatabase.CreateParameterNew("@amt", Math.Round(amount, 2)),
            ClinicDatabase.CreateParameterNew("@by", -1),
            ClinicDatabase.CreateParameterNew("@cd", DateTime.Now));
        }

        // ===============================================
        // HELPERS
        // ===============================================
        private decimal GetLateRate()
        {
            object x = ClinicDatabase.ExecuteScalar("SELECT latearrivaldeduction FROM tbl_setting_deduction_config LIMIT 1");
            return x == null ? 0 : Convert.ToDecimal(x);
        }

        // ===============================================
        private class EmployeePayroll
        {
            public string Code;
            public decimal Basic;
            public decimal Housing;
            public decimal Transport;
            public decimal Other;
            public decimal GrossSalary;
            public DateTime ContractDate;
        }

        private class PayrollResult
        {
            public int TotalDays;
            public int AbsentDays;
            public int LateMinutes;

            public decimal AbsenceDeduction;
            public decimal LateDeduction;
            public decimal Loan;
            public decimal TotalDeduction;
            public decimal NetSalary;
        }
        private void btnSave_ClickOld(object sender, EventArgs e)
        {
            if (!isAllDataSucceed)
            {
                MessageBox.Show(L("Check All Data Required First...", "يرجى التحقق من جميع البيانات المطلوبة أولاً..."));
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_attendancesheet where code = @code and WorkDate=@date",
                                             ClinicDatabase.CreateParameterNew("code", dgvAttendance.Rows[0].Cells[0].Value.ToString()),
                                             ClinicDatabase.CreateParameterNew("date", DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value.ToString()).ToString("yyyy-MM-dd"))))
                if (reader.Read())
                {
                    MessageBox.Show(L("Employee Month already saved before", "تم حفظ بيانات الموظف لهذا الشهر مسبقاً."));
                    return;
                }
            for (int i = 0; i < dgvAttendance.Rows.Count; i++)
            {

                if ((dgvAttendance.Rows[i].Cells["time in"].Value.ToString() == "" || dgvAttendance.Rows[i].Cells["time out"].Value.ToString() == "") && (dgvAttendance.Rows[i].Cells["status"].Value.ToString() == "p"))
                {
                    MessageBox.Show(L("Date ", "التاريخ ") + dgvAttendance.Rows[i].Cells["work date"].Value.ToString() + L(" Does Not Contain Time, Please Check It.", " لا يحتوي على وقت. يرجى التحقق منه."));
                    return;
                }
            }
            SaveMultipleAttendanceRows();
            SaveLSOld();
            SaveEOSOld();
            MessageBox.Show(L("Employee Month Saved Successfully", "تم حفظ بيانات الموظف لهذا الشهر بنجاح."));
        }
        private void SaveMultipleAttendanceRows()
        {
            DateTime workDate = DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString());
            DateTime endOfMonthDate = new DateTime(workDate.Year, workDate.Month, DateTime.DaysInMonth(workDate.Year, workDate.Month));
            int totalDelayMinutesNew = 0;
            decimal attendanceSalaryId = decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO `tbl_attendance_salary`(date,emp_code,created_by,created_date) values(@date,@emp_code,@created_by,@created_date);SELECT LAST_INSERT_ID(); ",
                     ClinicDatabase.CreateParameterNew("@emp_code", dgvAttendance.Rows[0].Cells["Emp ID"].Value.ToString()),
                     ClinicDatabase.CreateParameterNew("@date", endOfMonthDate),
                     ClinicDatabase.CreateParameterNew("@created_by", -1),
                     ClinicDatabase.CreateParameterNew("@created_date", DateTime.Now.Date)).ToString());
            DateTime parseDateR = DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString());
            string refr = (parseDateR.Year % 100) + $"{parseDateR.Month:D2}";
            ////Utilities.LogAudit(-1, "Save Attendance", "Attendance", (int)attendanceSalaryId, "Saved Attendance for Employee: " + dgvAttendance.Rows[0].Cells["Emp ID"].Value.ToString() + " for Month: " + parseDateR.ToString("MMMM yyyy"));
            int _month = parseDateR.Month;
            int _year = parseDateR.Year;
            object ref_no_result = ClinicDatabase.ExecuteScalar(@"SELECT Ref_Code FROM tbl_attendancesheet 
                  WHERE YEAR(WorkDate) = @year AND MONTH(WorkDate) = @month 
                  LIMIT 1",
            ClinicDatabase.CreateParameterNew("@year", _year),
            ClinicDatabase.CreateParameterNew("@month", _month));

            int ref_no;
            if (ref_no_result != null && ref_no_result != DBNull.Value && Convert.ToInt32(ref_no_result) > 0)
            {
                ref_no = Convert.ToInt32(ref_no_result);
            }
            else
            {
                object maxResult = ClinicDatabase.ExecuteScalar(@"SELECT MAX(Ref_Code) FROM tbl_attendancesheet");
                int max = (maxResult != null && maxResult != DBNull.Value) ? Convert.ToInt32(maxResult) : 600000;
                ref_no = max + 1;
            }

            using (MySqlDataReader Creader = ClinicDatabase.ExecuteReader("select delaytime,latearrivaldeduction from tbl_setting_deduction_config LIMIT 1"))
                if (Creader.Read())
                {
                    defaultTimeIn = TimeSpan.Parse(Creader["delaytime"].ToString());
                    deductionRate = Convert.ToDecimal(Creader["latearrivaldeduction"]);
                }

            decimal totalSalary = 0;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT BasicSalary, HousingAllowance, TransportationAllowance, Other, WorkDays,contractIssueDate FROM tbl_employee WHERE code = @code",
                      ClinicDatabase.CreateParameterNew("@code", dgvAttendance.Rows[0].Cells["Emp ID"].Value ?? 0)))
                if (reader.Read())
                {
                    workDays = int.Parse(reader["workDays"].ToString());
                    basicSalary = Convert.ToDecimal(reader["BasicSalary"]);
                    housingAllowance = Convert.ToDecimal(reader["HousingAllowance"]);
                    transportationAllowance = Convert.ToDecimal(reader["TransportationAllowance"]);
                    otherAllowance = Convert.ToDecimal(reader["Other"]);
                    ContractIssuse = DateTime.Parse(reader["contractIssueDate"].ToString());
                    totalSalary = basicSalary + housingAllowance + transportationAllowance + otherAllowance;
                    //dailyDeduction = Math.Round(totalSalary / Convert.ToInt32(reader["WorkDays"]), 3);
                }
            int totalPrecentDays = 0;
            int workingDays = dgvAttendance.Rows
                                        .Cast<DataGridViewRow>()
                                        .Count(r => r.Cells["Status"].Value != null &&
                                                    (r.Cells["Status"].Value.ToString().ToUpper() == "P" || r.Cells["Status"].Value.ToString().ToUpper() == "V"));

            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                if (row.Cells["Status"].Value.ToString().ToUpper() == "A")
                {
                    dailyDeduction = Math.Round(totalSalary / workingDays, 3);
                    totalDeduction += dailyDeduction;
                    totalAbsence++;
                }
                else if (row.Cells["Status"].Value.ToString().ToUpper() == "P")
                {
                    // check with settings

                    string timeIn = TimeSpan.Parse(row.Cells["Time In"].Value.ToString() + ":00").ToString();
                    string query = @"SELECT (SELECT latearrivaldeduction FROM tbl_setting_deduction_config ) deduction
                                        FROM tbl_setting_attendance 
                                        WHERE DATE >= @date AND DATE <= @date AND state=1;";
                    using (MySqlDataReader settingsReader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("date", workDate)))
                        if (settingsReader.Read())
                        {
                            var deductionSet = settingsReader["deduction"];

                            TimeSpan actualTimeIn0 = TimeSpan.Parse(row.Cells["Time In"].Value.ToString() + ":00");
                            TimeSpan actualTimeOut0 = TimeSpan.Parse(row.Cells["Time Out"].Value.ToString() + ":00");

                            int delayMinutes0 = (actualTimeIn0 > defaultTimeIn) ? (int)(actualTimeIn0 - defaultTimeIn).TotalMinutes : 0;
                            totalDelayMinutesNew += (actualTimeIn0 > defaultTimeIn) ? (int)(actualTimeIn0 - defaultTimeIn).TotalMinutes : 0;
                            deductionRate = decimal.Parse(deductionSet.ToString());

                            int delayMinutesx = delayMinutes0;
                            deductionAmount = delayMinutes0 * deductionRate;
                            totalDeduction += deductionAmount;
                            totalDelayMinutes += delayMinutes0;
                        }

                    //TimeSpan actualTimeIn = TimeSpan.Parse(row.Cells["Time In"].Value.ToString() + ":00");
                    //TimeSpan actualTimeOut = TimeSpan.Parse(row.Cells["Time Out"].Value.ToString() + ":00");
                    //int delayMinutes = (actualTimeIn > defaultTimeIn) ? (int)(actualTimeIn - defaultTimeIn).TotalMinutes : 0;
                    //delayMinutes += (actualTimeOut > defaultTimeIn) ? (int)(actualTimeIn - defaultTimeIn).TotalMinutes : 0;
                    //deductionAmount = delayMinutes * deductionRate;
                    //totalDeduction += deductionAmount;
                    //totalDelayMinutes += delayMinutes;
                    totalPrecentDays++;
                }
                id = (int)Convert.ToDecimal(ClinicDatabase.ExecuteScalar(@"
                INSERT INTO tbl_attendancesheet 
                (attendance_salary_id, code, WorkDate, TimeIn, TimeOut, DayOfWeek, Status, Reference,Ref_Code) 
                VALUES(@attendance_salary_id, @code, @WorkDate, @TimeIn, @TimeOut, @DayOfWeek, @Status, @Reference,@Ref_Code);
                SELECT LAST_INSERT_ID();",
                ClinicDatabase.CreateParameterNew("@attendance_salary_id", attendanceSalaryId),
                ClinicDatabase.CreateParameterNew("@code", row.Cells["Emp ID"].Value ?? 0),
                ClinicDatabase.CreateParameterNew("@WorkDate", DateTime.Parse(row.Cells["Work Date"].Value.ToString()).ToString("yyyy-MM-dd")),
                ClinicDatabase.CreateParameterNew("@TimeIn", row.Cells["Time In"].Value.ToString() + ":00"),
                ClinicDatabase.CreateParameterNew("@TimeOut", row.Cells["Time Out"].Value.ToString() + ":00"),
                ClinicDatabase.CreateParameterNew("@DayOfWeek", row.Cells["Day Of Week"].Value.ToString()),
                ClinicDatabase.CreateParameterNew("@Status", row.Cells["Status"].Value ?? ""),
                ClinicDatabase.CreateParameterNew("@Reference", refr),
                ClinicDatabase.CreateParameterNew("@Ref_Code", ref_no.ToString())
            ));
            }
            string dateValue = dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString();
            DateTime parseDate = DateTime.Parse(dateValue);


            var empId = dgvAttendance.Rows[0].Cells["Emp ID"].Value ?? DBNull.Value;
            int month = parseDate.Month;
            int year = parseDate.Year;

            object result = ClinicDatabase.ExecuteScalar(@"SELECT ss_no FROM tbl_attendance_salary 
                  WHERE YEAR(date) = @year AND MONTH(date) = @month 
                  LIMIT 1",
            ClinicDatabase.CreateParameterNew("@year", year),
            ClinicDatabase.CreateParameterNew("@month", month));

            int ss_no;
            if (result != null && result != DBNull.Value && Convert.ToInt32(result) > 0)
            {
                ss_no = Convert.ToInt32(result);
            }
            else
            {
                object maxResult = ClinicDatabase.ExecuteScalar(@"SELECT MAX(ss_no) FROM tbl_attendance_salary");
                int max = (maxResult != null && maxResult != DBNull.Value) ? Convert.ToInt32(maxResult) : 0;
                ss_no = max + 1;
            }
            decimal loanAmount = GetEmployeeLoan(empId, month, year);
            decimal totalDeductions = totalDeduction + loanAmount;
            decimal totalAdditions = basicSalary + housingAllowance + transportationAllowance + otherAllowance;
            decimal netSalary = totalAdditions - totalDeductions;
            ClinicDatabase.ExecuteNonQuery(@"update tbl_attendance_salary set absence_days=@absence_days,total_absence=@total_absence,delay_minutes=@delay_minutes,
                                    total_delay=@total_delay,total_loan=@total_loan,net_salary = @net_salary , pay = 0 , `change` = @net_salary,
                            ss_no = @ss_no where id = @id",
                    ClinicDatabase.CreateParameterNew("@id", attendanceSalaryId),
                    ClinicDatabase.CreateParameterNew("@code", empId),
                    ClinicDatabase.CreateParameterNew("@absence_days", totalAbsence),
                    ClinicDatabase.CreateParameterNew("@total_absence", totalAbsence * dailyDeduction),
                    ClinicDatabase.CreateParameterNew("@delay_minutes", totalDelayMinutes),
                    ClinicDatabase.CreateParameterNew("@total_delay", totalDelayMinutes * deductionRate),
                    ClinicDatabase.CreateParameterNew("@total_loan", loanAmount),
                    ClinicDatabase.CreateParameterNew("@net_salary", netSalary),
                    ClinicDatabase.CreateParameterNew("@ss_no", ss_no),
                    ClinicDatabase.CreateParameterNew("@created_by", -1),
                    ClinicDatabase.CreateParameterNew("@created_date", DateTime.Now.Date)
                    );

            insertJournals(netSalary, id);
            ////Utilities.LogAudit(-1, "Save Attendance Salary", "Attendance Salary", (int)attendanceSalaryId, "Saved Attendance Salary for Employee: " + dgvAttendance.Rows[0].Cells["Emp ID"].Value.ToString() + " for Month: " + parseDate.ToString("MMMM yyyy"));
        }
        private void MasterAttendanceSheet_Load(object sender, EventArgs e)
        {
            BindCombos.PopulateEmployeesForLoan(cmbEmployeeName);
            BindCombos.PopulateEmployeesForLoan(cmbEmployees);
            cmbEmployees.SelectedIndex = -1;

            var EnglishMonths = new CultureInfo("en-US")
                .DateTimeFormat.MonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .ToArray();

            cmbMonths.Items.AddRange(EnglishMonths);

            cmbMonths.SelectedItem = DateTime.Now.ToString("MMMM", new CultureInfo("en-US"));
            cmbMonths.SelectedIndex = -1;

            string language =
      Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
            ConfigureAttendanceClosingHeader(isArabic);
            RestoreAttendanceHeader();

            // Ensure the top toolbar buttons remain visible after initial layout.
            foreach (Control button in new Control[]
            {
                btnAttendanceReport,
                btnAddManualAttendance,
                DownloadExcelSheet
            })
            {
                if (button != null && !button.IsDisposed)
                {
                    button.Visible = true;
                    button.BringToFront();
                }
            }

            // Reassert after the remaining Load/layout handlers have completed.
            if (IsHandleCreated && !IsDisposed)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    if (!IsDisposed)
                        RestoreAttendanceHeader();
                }));
            }
        }

        private void guna2NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployeeName.SelectedValue == null)
            {
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where id = " + cmbEmployeeName.SelectedValue.ToString()))
                if (reader.Read())
                {
                    cmbEmployeeName.Text = reader["code"].ToString();
                    codest = reader["code"].ToString();
                    code = Convert.ToInt32(codest);
                }
            BindCombos.monthcheckcmb(cmbmonth, code);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            //guna2DataGridView1.Rows.Clear();
            GetData();
        }

        private decimal GetEmployeeLoan(object empId, int month, int year)
        {
            if (empId == DBNull.Value) return 0;

            object result = ClinicDatabase.ExecuteScalar(@"SELECT SUM(amount) AS totalLoan 
                                           FROM tbl_loan 
                                           WHERE YEAR(loandates) = @year 
                                           AND MONTH(loandates) = @month 
                                           AND employeeId = @code",
                            ClinicDatabase.CreateParameterNew("@year", year),
                            ClinicDatabase.CreateParameterNew("@month", month),
                            ClinicDatabase.CreateParameterNew("@code", empId));
            return result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
        }

        private void cmbmonth_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddManualAttendance_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            pnlData.Visible = false;
            pnlAddData.Visible = true;
            dgvAttendance.DataSource = null;
            cmbMonths.SelectedIndex = cmbEmployees.SelectedIndex = -1;
        }

        private void btnAttendanceReport_Click(object sender, EventArgs e)
        {
            panel5.Visible = true;
            pnlData.Visible = false;
            pnlAddData.Visible = false;
            panel5.BringToFront();
            dgvAttendance.DataSource = null;
        }

        private void cmbEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedValue == null)
            {
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where id = " + cmbEmployees.SelectedValue.ToString()))
                if (reader.Read())
                {
                    lblEmployeeCode.Text = reader["code"].ToString();
                    codest = reader["code"].ToString();
                    code = Convert.ToInt32(codest);
                }

        }

        private void cmbMonths_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedItem == null || cmbMonths.SelectedItem == null)
                return;
            dgvAttendance.DataSource = null;
            dgvAttendance.Rows.Clear();
            dgvAttendance.Columns.Clear();

            // Setup columns
            dgvAttendance.Columns.Add("Emp ID", "Emp ID");
            dgvAttendance.Columns.Add("Emp Name", "Emp Name");
            dgvAttendance.Columns.Add("Work Date", "Work Date");
            dgvAttendance.Columns.Add("Time In", "Time In");
            dgvAttendance.Columns.Add("Time Out", "Time Out");
            dgvAttendance.Columns.Add("Day Of Week", "Day of Week");
            dgvAttendance.Columns.Add("Status", "Status");

            LocalizationManager.LocalizeDataGridViewHeaders(dgvAttendance);
            string selectedMonth = cmbMonths.SelectedItem.ToString();
            int monthNumber = DateTime.ParseExact(selectedMonth.Trim(), "MMMM", new CultureInfo("en-US")).Month;
            int year = DateTime.Now.Year;

            DateTime startDate = new DateTime(year, monthNumber, 1);
            int daysInMonth = DateTime.DaysInMonth(year, monthNumber);
            string query = @"SELECT id,date,day,timein,timeout FROM tbl_setting_attendance 
                                        WHERE DATE >= @date AND DATE <= @date AND state=1 order by id DESC limit 1;";
            string actualTimeIn = "08:00", actualTimeOut = "18:00";
            using (MySqlDataReader settingsReader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("date", startDate)))
                if (settingsReader.Read())
                {
                    actualTimeIn = settingsReader["timein"].ToString() + ":00";
                    actualTimeOut = settingsReader["timeout"].ToString() + ":00";
                }

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime workDate = new DateTime(year, monthNumber, day);
                string dayOfWeek = workDate.DayOfWeek.ToString();
                string status = (workDate.DayOfWeek == DayOfWeek.Sunday) ? "V" : "P";

                dgvAttendance.Rows.Add(lblEmployeeCode.Text, cmbEmployees.Text, workDate.ToString("yyyy-MM-dd"), actualTimeIn, actualTimeOut, dayOfWeek, status);
            }
            // 1. Enable editing for the whole grid
            dgvAttendance.ReadOnly = false;

            // 2. Make all columns read-only first
            foreach (DataGridViewColumn col in dgvAttendance.Columns)
            {
                col.ReadOnly = true;
            }

            // 3. Enable editing only for specific columns
            dgvAttendance.Columns["Time In"].ReadOnly = false;
            dgvAttendance.Columns["Time Out"].ReadOnly = false;
            dgvAttendance.Columns["Status"].ReadOnly = false;

            // Optional: Control how editing starts
            dgvAttendance.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            isAllDataSucceed = true;
            if (dgvAttendance.Rows.Count == 0)
                isAllDataSucceed = false;
            else if (!chkCorrectDataForEmpNameEmpCode())
                isAllDataSucceed = false;
            else if (!chkExistingEmployee())
                isAllDataSucceed = false;
            else if (!CheckAllDaysIncluded(dgvAttendance, DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value.ToString()).Year, DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value.ToString()).Month))
                isAllDataSucceed = false;
        }

        public void GetData()
        {
            string qry = @"SELECT WorkDate,TimeIn,TimeOut,DayOfWeek,Status FROM tbl_attendancesheet WHERE  MONTHNAME(WorkDate)  = '" + cmbmonth.Text + "' and YEAR(WorkDate) = '" + guna2NumericUpDown1.Value.ToString() + "' and code = " + code + "  ";

            ListBox lb = new ListBox();
            lb.Items.Add(dgvdate);
            lb.Items.Add(dgvtimeIn);
            lb.Items.Add(dgvTimeOut);
            lb.Items.Add(dgvday);
            lb.Items.Add(dgvStatus);

            DataTable dt = ClinicDatabase.ExecuteDataTable(qry);
            if (dt != null)
                LoadDataGridView(dt, dgvAttendance, lb);

        }
        public static void LoadDataGridView(
        DataTable dt,
        DataGridView gv,
        ListBox lb)
        {
            if (dt == null)
                return;

            gv.AutoGenerateColumns = false;

            for (int i = 0; i < lb.Items.Count && i < dt.Columns.Count; i++)
            {
                DataGridViewColumn column = lb.Items[i] as DataGridViewColumn;

                if (column == null)
                    continue;

                if (gv.Columns.Contains(column.Name))
                {
                    gv.Columns[column.Name].DataPropertyName =
                        dt.Columns[i].ColumnName;
                }
            }

            gv.DataSource = dt;
        }
        private void SaveLSOld()
        {
            int debitAccountId = 0;
            int creditAccountId = 0;

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
            "SELECT account_id FROM tbl_coa_config WHERE category = @cat",
            ClinicDatabase.CreateParameterNew("@cat", "Leave Salary Debit")))
            {
                if (reader.Read())
                    debitAccountId = Convert.ToInt32(reader["account_id"]);
            }

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
            "SELECT account_id FROM tbl_coa_config WHERE category = @cat",
            ClinicDatabase.CreateParameterNew("@cat", "Leave Salary Credit")))
            {
                if (reader.Read())
                    creditAccountId = Convert.ToInt32(reader["account_id"]);
            }
            var empId = dgvAttendance.Rows[0].Cells["Emp ID"].Value ?? DBNull.Value;
            DateTime parseDate = DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString());
            string description = "Leave Salary";
            var empName = dgvAttendance.Rows[0].Cells["Emp Name"].Value ?? DBNull.Value;
            string refr = (parseDate.Year % 100) + $"{parseDate.Month:D2}";
            decimal leaveDays = ((decimal)workDays - (decimal)totalAbsence) / (decimal)365 * 30;
            decimal leaveAmount = (decimal)((decimal)basicSalary * 12) / 365 * (decimal)leaveDays;
            string voucherNote = "Leave Salary " + refr;
            string empCode = dgvAttendance.Rows[0].Cells["Emp ID"].Value?.ToString();
            object empIdResult = ClinicDatabase.ExecuteScalar("SELECT id FROM tbl_employee WHERE code = @code", ClinicDatabase.CreateParameterNew("@code", empCode));
            string employeeId = empIdResult.ToString();
            ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_leave_salary(date,code,name ,Reference, 
                                    description, leave_days, credit,created_by, created_date)
                                VALUES (@date, @code,@name, @Reference, @description, @leave_days, @credit, @created_by, @created_date);",
                        ClinicDatabase.CreateParameterNew("@date", DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString()).ToString("MMM-yy")),
                        ClinicDatabase.CreateParameterNew("@code", dgvAttendance.Rows[0].Cells["Emp ID"].Value ?? DBNull.Value),
                        ClinicDatabase.CreateParameterNew("@Reference", refr),
                        ClinicDatabase.CreateParameterNew("@name", empName),
                        ClinicDatabase.CreateParameterNew("@description", description),
                        ClinicDatabase.CreateParameterNew("@leave_days", Math.Round(leaveDays, 3)),
                        ClinicDatabase.CreateParameterNew("@credit", Math.Round(leaveAmount, 3)),
                        ClinicDatabase.CreateParameterNew("@created_by", -1),
                        ClinicDatabase.CreateParameterNew("@created_date", DateTime.Now.Date));
            ////Utilities.LogAudit(-1, "Save Leave Salary", "Leave Salary", int.Parse(refr), "Saved Leave Salary for Employee: " + empCode + " for Month: " + parseDate.ToString("MMMM yyyy"));
            CommonInsert.addTransactionEntry(
          DateTime.Now.Date, debitAccountId.ToString(), leaveAmount.ToString(), "0", id.ToString(), employeeId, description, "Leave Salary", voucherNote, -1, DateTime.Now.Date, "");

            CommonInsert.addTransactionEntry(
            DateTime.Now.Date, creditAccountId.ToString(), "0", leaveAmount.ToString(), id.ToString(), "0", description, "Leave Salary", voucherNote, -1, DateTime.Now.Date, "");
        }

        private void SaveEOSOld()
        {
            int debitAccountId = 0;
            int creditAccountId = 0;

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
            "SELECT account_id FROM tbl_coa_config WHERE category = @cat",
            ClinicDatabase.CreateParameterNew("@cat", "End of Service Debit")))
            {
                if (reader.Read())
                    debitAccountId = Convert.ToInt32(reader["account_id"]);
            }

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
            "SELECT account_id FROM tbl_coa_config WHERE category = @cat",
            ClinicDatabase.CreateParameterNew("@cat", "End of Service Credit")))
            {
                if (reader.Read())
                    creditAccountId = Convert.ToInt32(reader["account_id"]);
            }
            string empCode = dgvAttendance.Rows[0].Cells["Emp ID"].Value?.ToString();
            object empIdResult = ClinicDatabase.ExecuteScalar("SELECT id FROM tbl_employee WHERE code = @code",
                ClinicDatabase.CreateParameterNew("@code", empCode));
            string employeeId = empIdResult.ToString();
            string dateValue = dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString();
            DateTime parseDate = DateTime.Parse(dateValue);
            int month = parseDate.Month;
            string Date = DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString()).ToString("MMM-yy");
            var empId = dgvAttendance.Rows[0].Cells["Emp ID"].Value ?? DBNull.Value;
            var empName = dgvAttendance.Rows[0].Cells["Emp Name"].Value ?? DBNull.Value;
            string description = "End Of Service";
            string refr = (parseDate.Year % 100) + $"0{month:D2}";
            int yearsOfService = (int)((DateTime.Now - ContractIssuse).TotalDays / 365);
            decimal endOfServiceDays = (yearsOfService < 5) ? ((decimal)workDays - (decimal)totalAbsence) / 365 * 21 : ((decimal)workDays - (decimal)totalAbsence) / 365 * 30;
            decimal endOfServiceAmount = (decimal)((decimal)basicSalary * 12) / 365 * (decimal)endOfServiceDays;
            string voucherNote = "End Of Services NO. " + refr;

            ClinicDatabase.ExecuteNonQuery(@"INSERT INTO `tbl_end_of_service`(`date`, `code`, `name`, `Reference`, 
                                    `description`, `debit`, `leave_days`, `credit`, `created_by`, `created_date`)
                                VALUES(@date, @code, @name, @Reference, @description, @debit, @leave_days, @credit, @created_by, @created_date);",
                        ClinicDatabase.CreateParameterNew("@date", Date),
                        ClinicDatabase.CreateParameterNew("@code", empId),
                        ClinicDatabase.CreateParameterNew("@name", empName),
                        ClinicDatabase.CreateParameterNew("@Reference", refr),
                        ClinicDatabase.CreateParameterNew("@description", description),
                        ClinicDatabase.CreateParameterNew("@debit", 0),
                        ClinicDatabase.CreateParameterNew("@leave_days", Math.Round(endOfServiceDays, 3)),
                        ClinicDatabase.CreateParameterNew("@credit", Math.Round(endOfServiceAmount, 3)),
                        ClinicDatabase.CreateParameterNew("@created_by", -1),
                        ClinicDatabase.CreateParameterNew("@created_date", DateTime.Now.Date));
            ////Utilities.LogAudit(-1, "Save End of Service", "End of Service", int.Parse(refr), "Saved End of Service for Employee: " + empCode + " for Month: " + parseDate.ToString("MMMM yyyy"));
            CommonInsert.addTransactionEntry(
           DateTime.Now.Date, debitAccountId.ToString(), endOfServiceAmount.ToString(), "0", id.ToString(), employeeId, description, "End Of Services", voucherNote, -1, DateTime.Now.Date, "");

            CommonInsert.addTransactionEntry(
            DateTime.Now.Date, creditAccountId.ToString(), "0", endOfServiceAmount.ToString(), id.ToString(), "0", description, "End Of Services", voucherNote, -1, DateTime.Now.Date, "");
        }
        private int GetAccountIdByCategory(string category)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT account_id FROM tbl_coa_config WHERE category = @category",
                ClinicDatabase.CreateParameterNew("category", category)))
                if (reader.Read())
                    return Convert.ToInt32(reader["account_id"]);
                else
                    return 0;
        }

        void insertJournals(decimal netSalary, int id)
        {
            int accruedSalaryAccountId = GetAccountIdByCategory("Accrued Salaries");
            if (accruedSalaryAccountId == 0)
            {
                MessageBox.Show(L("No Default Account Set for 'Accrued Salaries' Please Add It", "لم يتم تعيين الحساب الافتراضي للرواتب المستحقة. يرجى إضافته."), L("Error", "خطأ"), MessageBoxButtons.OK);
                return;
            }

            string empCode = dgvAttendance.Rows[0].Cells["Emp ID"].Value?.ToString();
            object empIdResult = ClinicDatabase.ExecuteScalar("SELECT id FROM tbl_employee WHERE code = @code",
                ClinicDatabase.CreateParameterNew("@code", empCode));
            string employeeId = empIdResult.ToString();

            string empAccountID = dgvAttendance.Rows[0].Cells["Emp ID"].Value?.ToString();
            object empAccountIdResult = ClinicDatabase.ExecuteScalar("SELECT account_id FROM tbl_coa_config WHERE category = @cat",
                ClinicDatabase.CreateParameterNew("@cat", "Salaries"));
            string empaccId = empAccountIdResult.ToString();

            string description = "Employee Salary";
            string refr = (DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString()).Year % 100).ToString() +
                          $"{DateTime.Parse(dgvAttendance.Rows[0].Cells["Work Date"].Value.ToString()).Month:D2}";
            string voucherNote = "Salary Sheet NO. " + refr;
            string debitAccountId = accruedSalaryAccountId.ToString();

            CommonInsert.addTransactionEntry(
            DateTime.Now.Date, empaccId, netSalary.ToString(), "0", id.ToString(), employeeId, description, "Salary", voucherNote, -1, DateTime.Now.Date, "");

            CommonInsert.addTransactionEntry(
            DateTime.Now.Date, debitAccountId, "0", netSalary.ToString(), id.ToString(), "0", description, "Salary", voucherNote, -1, DateTime.Now.Date, "");
        }
        private bool chkCorrectDataForEmpNameEmpCode()
        {
            if (dgvAttendance.Rows.Count > 1)
            {
                var empCode = dgvAttendance.Rows[0].Cells[0].Value?.ToString();
                var empName = dgvAttendance.Rows[0].Cells[1].Value?.ToString();
                int month = DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value?.ToString()).Month;

                foreach (DataGridViewRow row in dgvAttendance.Rows)
                {
                    if (row.Cells[0].Value?.ToString() != empCode)
                    {
                        MessageBox.Show(L("Employee Code has Different Values in Column 0 Please Correct it and Re Upload", "رمز الموظف يحتوي على قيم مختلفة في العمود 0. يرجى تصحيحها وإعادة الرفع."));
                        return false;
                    }
                    if (row.Cells[1].Value?.ToString().ToLower() != empName.ToLower())
                    {
                        MessageBox.Show(L("Employee Name has Different Values in Column 1 Please Correct it and Re Upload", "اسم الموظف يحتوي على قيم مختلفة في العمود 1. يرجى تصحيحها وإعادة الرفع."));
                        return false;
                    }
                    if (DateTime.Parse(row.Cells[2].Value?.ToString()).Month != month)
                    {
                        MessageBox.Show(L("Please Upload Only One Month.", "يرجى رفع بيانات شهر واحد فقط."));
                        return false;
                    }
                }
                return true;
            }
            else
                return false;
        }
        private bool chkExistingEmployee()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where code = @code",
                          ClinicDatabase.CreateParameterNew("code", dgvAttendance.Rows[0].Cells[0].Value.ToString())))
                if (reader.Read())
                {
                    string dbName = reader["name"].ToString().Trim().ToLower();
                    string excelName = dgvAttendance.Rows[0].Cells[1].Value.ToString().Trim().ToLower();

                    if (dbName != excelName)
                    {
                        MessageBox.Show(L($"Emp Name Does Not Belong To This Code. (DB: '{dbName}', Excel: '{excelName}')", $"اسم الموظف لا يتطابق مع هذا الرمز. (قاعدة البيانات: '{dbName}'، ملف Excel: '{excelName}')"));
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show(L("No Data For this Code", "لا توجد بيانات لهذا الرمز."));
                    return false;
                }
            return true;
        }

        private void DownloadExcelSheet_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            pnlData.Visible = true;
            pnlAddData.Visible = false;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
            saveFileDialog.Title = "Save Excel File";
            saveFileDialog.FileName = "Attendance Sheet.xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells[1, 1].Value = "Emp ID";
                    worksheet.Cells[1, 2].Value = "Emp Name";
                    worksheet.Cells[1, 3].Value = "Work Date";
                    worksheet.Cells[1, 4].Value = "Time In";
                    worksheet.Cells[1, 5].Value = "Time Out";
                    worksheet.Cells[1, 6].Value = "Day of Week";
                    worksheet.Cells[1, 7].Value = "Status";
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    MessageBox.Show(L("Excel file created successfully at: ", "تم إنشاء ملف Excel بنجاح في: ") + filePath, L("Success", "نجاح"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void cmbSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            isAllDataSucceed = true;
            string selectedSheet = cmbSheet.SelectedItem.ToString();

            dgvAttendance.DataSource = loadedExcelTables[selectedSheet];
            if (dgvAttendance.Rows.Count == 0)
                isAllDataSucceed = false;
            else if (!chkCorrectDataForEmpNameEmpCode())
                isAllDataSucceed = false;
            else if (!chkExistingEmployee())
                isAllDataSucceed = false;
            else if (!CheckAllDaysIncluded(dgvAttendance, DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value.ToString()).Year, DateTime.Parse(dgvAttendance.Rows[0].Cells[2].Value.ToString()).Month))
                isAllDataSucceed = false;
            if (dgvAttendance.Rows.Count > 0)
            {
                pnlData.Visible = true;
                RestoreAttendanceHeader();
                lblCode.Text = dgvAttendance.Rows[0].Cells[0].Value?.ToString() ?? "";
                lblName.Text = dgvAttendance.Rows[0].Cells[1].Value?.ToString() ?? "";
            }
            LocalizationManager.LocalizeDataGridViewHeaders(dgvAttendance);
        }
    }
}