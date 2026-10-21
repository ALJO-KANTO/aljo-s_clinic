using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmFinalSettlement : Form
    {

        int id;

        // Keep the designer's original positions so Arabic mirroring can be
        // applied without repeatedly moving controls when the form is refreshed.
        private readonly Dictionary<Control, Rectangle> finalSettlementOriginalBounds =
            new Dictionary<Control, Rectangle>();

        public frmFinalSettlement(int id = 0)
        {
            InitializeComponent();
            CaptureFinalSettlementOriginalBounds(this);
            LocalizationManager.LocalizeForm(this);
            this.id = id;
            this.Text = "Final Settlement";

            headerUC2.FormText = this.Text;
            ApplyFinalSettlementLanguage();
            Resize += (sender, args) =>
            {
                if (IsFinalSettlementArabic())
                    PositionFinalSettlementButtons(true);
            };
        }

        // Applies the same ApplicationLanguage-based English/Arabic switching
        // used by MasterFinalSettlement. Existing settlement logic is unchanged.
        private bool IsFinalSettlementArabic()
        {
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            return language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyFinalSettlementLanguage()
        {
            bool isArabic = IsFinalSettlementArabic();

            Text = isArabic ? "التسوية النهائية" : "Final Settlement";
            if (headerUC2 != null)
                headerUC2.FormText = Text;

            // Translate captions first, then mirror control positions manually.
            // RightToLeftLayout alone does not move labels beside text boxes inside
            // nested panels/group boxes.
            RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            RightToLeftLayout = false;

            ApplyFinalSettlementControlText(this, isArabic);

            // Explicitly set these buttons by control name because their designer
            // text may be reapplied after recursive translation.
            if (btnSave != null)
                btnSave.Text = isArabic ? "حفظ" : "Save";
            if (btnSaveClose != null)
                btnSaveClose.Text = isArabic ? "حفظ وإضافة جديد" : "Save & New";
            if (btnClose != null)
                btnClose.Text = isArabic ? "إغلاق" : "Close";

            ApplyFinalSettlementMirroring(this, isArabic);
            if (isArabic)
                AdjustFinalSettlementArabicLabelSpacing(this);
            PositionFinalSettlementButtons(isArabic);
            PerformLayout();
            Invalidate(true);
        }

        // Arabic labels can be longer than their English captions. Widen field
        // labels and keep a clear gap between each label and its nearby input.
        // Original control bounds are restored before every language refresh.
        private void AdjustFinalSettlementArabicLabelSpacing(Control parent)
        {
            if (parent == null) return;

            foreach (Control child in parent.Controls)
            {
                if (child is Label label && !string.IsNullOrWhiteSpace(label.Text))
                {
                    // Group headings are not Label controls; these are field captions.
                    const int desiredLabelWidth = 145;
                    int oldRight = label.Right;
                    if (label.Width < desiredLabelWidth)
                    {
                        label.SetBounds(oldRight - desiredLabelWidth, label.Top,
                            desiredLabelWidth, label.Height);
                    }
                    label.TextAlign = ContentAlignment.MiddleRight;
                    label.RightToLeft = RightToLeft.Yes;

                    Control nearestInput = null;
                    int nearestDistance = int.MaxValue;
                    foreach (Control candidate in parent.Controls)
                    {
                        if (candidate == label ||
                            !(candidate is TextBoxBase || candidate is ComboBox ||
                              candidate is DateTimePicker || candidate is NumericUpDown))
                            continue;

                        int verticalDistance = Math.Abs((candidate.Top + candidate.Height / 2) -
                                                        (label.Top + label.Height / 2));
                        if (verticalDistance > Math.Max(12, label.Height / 2 + 5))
                            continue;

                        int distance = Math.Abs(label.Left - candidate.Right);
                        if (candidate.Right <= label.Left + 8 && distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestInput = candidate;
                        }
                    }

                    if (nearestInput != null)
                    {
                        const int gap = 12;
                        int overlap = nearestInput.Right + gap - label.Left;
                        if (overlap > 0)
                            nearestInput.Left = Math.Max(0, nearestInput.Left - overlap);
                    }
                }

                if (child.HasChildren)
                    AdjustFinalSettlementArabicLabelSpacing(child);
            }
        }

        // Keep the action buttons grouped at the bottom-right in Arabic mode.
        // The main mirroring routine reverses every control, which can otherwise
        // move this button group to the left side of the form.
        private void PositionFinalSettlementButtons(bool isArabic)
        {
            if (!isArabic || btnSave == null || btnSaveClose == null || btnClose == null)
                return;

            Control parent = btnSave.Parent;
            if (parent == null || btnSaveClose.Parent != parent || btnClose.Parent != parent)
                return;

            const int rightPadding = 12;
            const int gap = 12;
            int y = btnSave.Top;

            // Arabic reading order: Save at the far right, then Save & New,
            // then Close to its left.
            int xSave = parent.ClientSize.Width - rightPadding - btnSave.Width;
            int xSaveClose = xSave - gap - btnSaveClose.Width;
            int xClose = xSaveClose - gap - btnClose.Width;

            btnSave.Location = new Point(Math.Max(0, xSave), y);
            btnSaveClose.Location = new Point(Math.Max(0, xSaveClose), y);
            btnClose.Location = new Point(Math.Max(0, xClose), y);
        }

        private void CaptureFinalSettlementOriginalBounds(Control parent)
        {
            if (parent == null) return;

            foreach (Control control in parent.Controls)
            {
                if (!finalSettlementOriginalBounds.ContainsKey(control))
                    finalSettlementOriginalBounds.Add(control, control.Bounds);

                CaptureFinalSettlementOriginalBounds(control);
            }
        }

        private void ApplyFinalSettlementMirroring(Control parent, bool isArabic)
        {
            if (parent == null) return;

            foreach (Control control in parent.Controls)
            {
                // Keep the custom title-bar control's internal minimize, maximize,
                // and close buttons in their designed positions.
                if (control.Name == "headerUC2")
                    continue;

                if (finalSettlementOriginalBounds.TryGetValue(control, out Rectangle original))
                {
                    int left = isArabic
                        ? parent.ClientSize.Width - original.Right
                        : original.Left;

                    control.SetBounds(left, original.Top, original.Width, original.Height);
                }

                control.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;

                if (control is Label label)
                {
                    label.TextAlign = isArabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }

                ApplyFinalSettlementMirroring(control, isArabic);
            }
        }

        private void ApplyFinalSettlementControlText(Control parent, bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                string current = (control.Text ?? "").Trim();
                string translated = TranslateFinalSettlementText(current, isArabic);

                if (!string.Equals(current, translated, StringComparison.Ordinal))
                    control.Text = translated;

                // Textboxes show entered values, not labels. Do not translate
                // employee names, codes, dates, or numeric values.
                if (control.HasChildren)
                    ApplyFinalSettlementControlText(control, isArabic);
            }
        }

        private string TranslateFinalSettlementText(string text, bool isArabic)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            string key = text.Trim();

            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Final Settlement", "التسوية النهائية" },
                { "Basic Data", "البيانات الأساسية" },
                { "Salary Information", "معلومات الراتب" },
                { "Settlement No", "رقم التسوية" },
                { "Date of Last Working Day", "تاريخ آخر يوم عمل" },
                { "Additions", "الإضافات" },
                { "Deductions", "الاستقطاعات" },
                { "End Of Services Allowance", "مكافأة نهاية الخدمة" },
                { "Leave Salary Allowance", "بدل راتب الإجازة" },
                { "Save and New", "حفظ وإضافة جديد" },
                { "Employee", "الموظف" },
                { "Employee Name", "اسم الموظف" },
                { "Employee Code", "رمز الموظف" },
                { "Code", "الرمز" },
                { "Date", "التاريخ" },
                { "Start Date", "تاريخ البدء" },
                { "Commencement Date", "تاريخ بدء العمل" },
                { "Date of Commencement", "تاريخ بدء العمل" },
                { "Last Working Day", "آخر يوم عمل" },
                { "Last Work Day", "آخر يوم عمل" },
                { "Last Working Date", "تاريخ آخر يوم عمل" },
                { "Basic Salary", "الراتب الأساسي" },
                { "Housing Allowance", "بدل السكن" },
                { "Transportation Allowance", "بدل المواصلات" },
                { "Other", "أخرى" },
                { "Others", "أخرى" },
                { "Total Salary", "إجمالي الراتب" },
                { "Salary", "الراتب" },
                { "Salaries", "الرواتب" },
                { "Leave Salary", "راتب الإجازة" },
                { "End Of Service", "مكافأة نهاية الخدمة" },
                { "Employee Advances", "سلف الموظف" },
                { "Advances", "السلف" },
                { "Payments", "المدفوعات" },
                { "Other Additions", "إضافات أخرى" },
                { "Total Additions", "إجمالي الإضافات" },
                { "Other Deductions", "استقطاعات أخرى" },
                { "Total Deductions", "إجمالي الاستقطاعات" },
                { "Net Accruals", "صافي المستحقات" },
                { "Save", "حفظ" },
                { "Save & Close", "حفظ وإغلاق" },
                { "Save and Close", "حفظ وإغلاق" },
                { "Save & New", "حفظ وإضافة جديد" },
                { "Close", "إغلاق" },
                { "Cancel", "إلغاء" },
                { "Add New", "إضافة جديد" },
                { "Please Chose Employee First.", "يرجى اختيار الموظف أولاً." },
                { "Please Choose Employee First.", "يرجى اختيار الموظف أولاً." },
                { "Error Check your data", "خطأ، يرجى التحقق من البيانات" }
            };

            foreach (var pair in translations)
            {
                if (string.Equals(key, pair.Key, StringComparison.OrdinalIgnoreCase))
                    return isArabic ? pair.Value : pair.Key;
            }

            // Restore the English form title if the language is switched back.
            if (!isArabic && key == "التسوية النهائية")
                return "Final Settlement";

            return text;
        }

        private void frmFinalSettlement_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void frmFinalSettlement_Load(object sender, EventArgs e)
        {
            BindCombos.PopulateEmployees(cmbEmployeeName);
            ApplyFinalSettlementLanguage();
            if (id != 0)
            {
                bindFinalsettle();
                btnSave.Enabled = btnSaveClose.Enabled = UserPermissions.canDelete("Final Settlement");
            }
        }
        private void cmbEmployeeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployeeName.SelectedValue == null || cmbEmployeeName.SelectedValue.ToString().ToLower().Contains("rowview"))
            {
                txtEmpCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where id = " + cmbEmployeeName.SelectedValue.ToString()))
                if (reader.Read())
                    txtEmpCode.Text = reader["code"].ToString();
                else
                    txtEmpCode.Text = "";
            if (cmbEmployeeName.SelectedValue == null || cmbEmployeeName.SelectedIndex == -1)
            {
                ClearTextFields();
                return;
            }
            string query = "SELECT BasicSalary, HousingAllowance, TransportationAllowance, Other FROM tbl_employee WHERE id = @id";

            MySqlParameter param = ClinicDatabase.CreateParameterNew("@id", cmbEmployeeName.SelectedValue);

            using (var reader = ClinicDatabase.ExecuteReader(query, param))
            {
                if (reader.Read())
                {
                    txtBasicSalary.Text = decimal.Parse(reader["BasicSalary"]?.ToString()).ToString("N2") ?? "";
                    txtHousingAllowance.Text = decimal.Parse(reader["HousingAllowance"]?.ToString()).ToString("N2") ?? "";
                    txtTransportationAllowance.Text = decimal.Parse(reader["TransportationAllowance"]?.ToString()).ToString("N2") ?? "";
                    txtOthers.Text = decimal.Parse(reader["Other"]?.ToString()).ToString("N2") ?? "";
                }
                else
                {
                    ClearTextFields();
                }
            }

            string Lreader = "SELECT ifnull(SUM(credit) - SUM(debit) ,0)AS 'Sum' FROM tbl_leave_salary where code = @code";
            MySqlParameter Lparam = ClinicDatabase.CreateParameterNew("@code", txtEmpCode.Text);

            using (var reader = ClinicDatabase.ExecuteReader(Lreader, Lparam))
            {
                if (reader.Read())
                {
                    txtLeaveSalary.Text = decimal.Parse(reader["Sum"]?.ToString()).ToString("N2") ?? "";
                }
                else
                {
                    ClearTextFields();
                }
            }

            string Ereader = "SELECT ifnull( SUM(credit) - SUM(debit) ,0)AS 'EOS' FROM tbl_end_of_service where code=@code";
            MySqlParameter Eparam = ClinicDatabase.CreateParameterNew("@code", txtEmpCode.Text);

            using (var reader = ClinicDatabase.ExecuteReader(Ereader, Eparam))
            {
                if (reader.Read())
                {
                    txtEndOfService.Text = decimal.Parse(reader["EOS"]?.ToString()).ToString("N2") ?? "";
                }
                else
                {
                    ClearTextFields();
                }
            }

            using (MySqlDataReader read = ClinicDatabase.ExecuteReader("SELECT ifnull ( SUM(Amount),0) AS amount FROM tbl_loan WHERE EmployeeID = @code AND loanDates >= CURDATE();",
                ClinicDatabase.CreateParameterNew("code", txtEmpCode.Text)))
                if (read.Read())
                    txtEmployeeAdvances.Text = decimal.Parse(read["amount"].ToString()).ToString("N2");
            Salarytotal();
            Salaries();
        }

        private void ClearTextFields()
        {
            txtBasicSalary.Text = "";
            txtHousingAllowance.Text = "";
            txtTransportationAllowance.Text = "";
            txtOthers.Text = "";
        }

        private void Salarytotal()
        {
            decimal basicSalary, houseAllowance, transportationAllowance, other, totalSalary;
            decimal.TryParse(txtBasicSalary.Text, out basicSalary);
            decimal.TryParse(txtHousingAllowance.Text, out houseAllowance);
            decimal.TryParse(txtTransportationAllowance.Text, out transportationAllowance);
            decimal.TryParse(txtOthers.Text, out other);
            totalSalary = basicSalary + transportationAllowance + houseAllowance + other;
            txtTotalSalary.Text = totalSalary.ToString();
        }

        private void Salaries()
        {
            decimal totalSalary, calculatedValue1, otherAdditions, totalAdditions, LeveSalary, EndOfServices;
            decimal.TryParse(txtTotalSalary.Text, out totalSalary);
            decimal.TryParse(txtOtherAdditions.Text, out otherAdditions);
            calculatedValue1 = (totalSalary / 31) * 29;
            //calculatedValue2 = (totalSalary / 30) * 20;
            txtSalaries.Text = calculatedValue1.ToString("N2");
            decimal.TryParse(txtEndOfService.Text, out EndOfServices);
            decimal.TryParse(txtLeaveSalary.Text, out LeveSalary);
            totalAdditions = calculatedValue1 + LeveSalary + EndOfServices + otherAdditions;
            txtTotalAdditions.Text = totalAdditions.ToString("N2");
            CalculateNetAccruals();
        }

        //private void CalculateTotalDeductions()
        //{
        //    decimal employeeAdvances, otherDeductions, payments, totalDeductions;
        //    decimal.TryParse(txtEmployeeAdvances.Text, out employeeAdvances);
        //    decimal.TryParse(txtOtherDeductions.Text, out otherDeductions);
        //    decimal.TryParse(txtPayments.Text, out payments);
        //    totalDeductions = employeeAdvances + otherDeductions + payments;
        //    txtTotalDeductions.Text = totalDeductions.ToString("");
        //    CalculateNetAccruals();
        //}

        private void CalculateNetAccruals()
        {
            decimal totalAdditions, totalDeductions, netAccruals;
            decimal.TryParse(txtTotalAdditions.Text, out totalAdditions);
            decimal.TryParse(txtTotalDeductions.Text, out totalDeductions);
            netAccruals = totalAdditions - totalDeductions;
            txtNetAccruals.Text = Math.Abs(netAccruals).ToString("");

        }


        private void txtTotalDeductions_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmpCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where code =@code",
       ClinicDatabase.CreateParameterNew("code", txtEmpCode.Text)))
                if (reader.Read())
                    cmbEmployeeName.SelectedValue = int.Parse(reader["id"].ToString());

        }

        private void txtEmpCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where code =@code",
        ClinicDatabase.CreateParameterNew("code", txtEmpCode.Text)))
                if (!reader.Read())
                    cmbEmployeeName.SelectedIndex = -1;
        }

        private void txtSalaries_TextChanged(object sender, EventArgs e)
        {
            if (txtSalaries.Text == "")
                txtSalaries.Text = "0";
            if (txtOtherAdditions.Text == "")
                txtOtherAdditions.Text = "0";
            if (txtEndOfService.Text == "")
                txtEndOfService.Text = "0";
            if (txtLeaveSalary.Text == "")
                txtLeaveSalary.Text = "0";
            txtTotalAdditions.Text = (decimal.Parse(txtSalaries.Text) + decimal.Parse(txtLeaveSalary.Text) + decimal.Parse(txtEndOfService.Text) + decimal.Parse(txtOtherAdditions.Text)).ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                if (insertEmployeeFinallSet())
                    this.Close();
            }
            else
               if (bindFinalsettle())
                this.Close();
        }

        bool bindFinalsettle()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT 
                                    tf.code,tf.Date,e.code AS 'Employee ID',e.Name as EmployeeName,tf.emp_id,
                                     tf.DateCommencement, 
                                     tf.DateLastWork,
                                      tf.TotalSalary, 
                                    tf.OtherAdditions,
                                      tf.TotalAdditions, 
                                    tf.Payments,
                                    tf.OtherDeductions,
                                      tf.TotalDeductions,
                                       tf.NetAccruals
                                       from tbl_final_settlement tf
                                       INNER JOIN tbl_employee e ON tf.emp_id = e.id where tf.emp_id=@id
                                    ",
                                    ClinicDatabase.CreateParameterNew("id", id)))
                if (reader.Read())
                {
                    dtpDate.Value = Convert.ToDateTime(reader["Date"]);
                    txtCode.Text = reader["code"].ToString();
                    cmbEmployeeName.SelectedValue = Convert.ToInt32(reader["emp_id"]);
                    dtpCommencement.Value = Convert.ToDateTime(reader["DateCommencement"]);
                    dtpLastWorkDay.Value = Convert.ToDateTime(reader["DateLastWork"]);
                    txtTotalSalary.Text = reader["TotalSalary"].ToString();
                    txtOtherAdditions.Text = reader["OtherAdditions"].ToString();
                    txtTotalAdditions.Text = reader["TotalAdditions"].ToString();
                    txtPayments.Text = reader["Payments"].ToString();
                    txtOtherDeductions.Text = reader["OtherDeductions"].ToString();
                    txtTotalDeductions.Text = reader["TotalDeductions"].ToString();
                    txtNetAccruals.Text = reader["NetAccruals"].ToString();
                }
            return true;
        }
        private bool insertEmployeeFinallSet()
        {
            string code = "FS-0001";
            if (cmbEmployeeName.Text.Trim() == "")
            {
                MessageBox.Show(IsFinalSettlementArabic() ? "يرجى اختيار الموظف أولاً." : "Please Chose Employee First.");
                return false;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT code FROM tbl_final_settlement 
                                                           ORDER BY CAST(SUBSTRING_INDEX(code, '-', -1) AS UNSIGNED) DESC LIMIT 1;"))
                if (reader.Read() && reader["code"].ToString() != "")
                    code = "FS-000" + (int.Parse(reader["code"].ToString().Replace("FS-", "")) + 1);

            int refId = ClinicDatabase.ExecuteNonQuery(@"INSERT INTO `tbl_final_settlement`(`code`, `Date`, `emp_id`, `DateCommencement`, `DateLastWork`,
                                `TotalSalary`, `TotalAdditions`, `TotalDeductions`, `NetAccruals`) 
                                VALUES(@code, @Date,@emp_id, @DateCommencement, @DateLastWork, @TotalSalary, @TotalAdditions, @TotalDeductions,
                                    @NetAccruals);",
                    ClinicDatabase.CreateParameterNew("code", code),
                    ClinicDatabase.CreateParameterNew("Date", dtpDate.Value.Date),
                    ClinicDatabase.CreateParameterNew("emp_id", cmbEmployeeName.SelectedValue),
                    ClinicDatabase.CreateParameterNew("DateCommencement", dtpCommencement.Value.Date),
                    ClinicDatabase.CreateParameterNew("DateLastWork", dtpLastWorkDay.Value.Date),
                    ClinicDatabase.CreateParameterNew("TotalSalary", txtTotalSalary.Text),
                    ClinicDatabase.CreateParameterNew("TotalAdditions", txtTotalAdditions.Text),
                    ClinicDatabase.CreateParameterNew("TotalDeductions", txtTotalDeductions.Text),
                    ClinicDatabase.CreateParameterNew("NetAccruals", txtNetAccruals.Text));
            if (refId != 0)
            {
                ////Utilities.LogAudit(-1, "Insert Final Settlement", "Final Settlement", refId, "Inserted Final Settlement for Employee: " + cmbEmployeeName.Text + " with Code: " + code);
                return true;
            }
            else
            {
                string message = Utilities.LastLogError();
                MessageBox.Show(IsFinalSettlementArabic() ? "خطأ، يرجى التحقق من البيانات: " + message : "Error Check your data" + message);
                return false;
            }
        }

        private void txtOtherAdditions_TextChanged(object sender, EventArgs e)
        {

            if (txtSalaries.Text == "")
                txtSalaries.Text = "0";
            if (txtEndOfService.Text == "")
                txtEndOfService.Text = "0";
            if (txtLeaveSalary.Text == "")
                txtLeaveSalary.Text = "0";
            if (txtOtherAdditions.Text == "")
                txtOtherAdditions.Text = "0";
            txtTotalAdditions.Text = (decimal.Parse(txtSalaries.Text) + decimal.Parse(txtEndOfService.Text) + decimal.Parse(txtLeaveSalary.Text) + decimal.Parse(txtOtherAdditions.Text)).ToString();
            txtNetAccruals.Text = (decimal.Parse(txtTotalAdditions.Text)).ToString();
        }

        private void txtOtherDeductions_TextChanged(object sender, EventArgs e)
        {
            if (txtEmployeeAdvances.Text == "")
                txtEmployeeAdvances.Text = "0";
            if (txtPayments.Text == "")
                txtPayments.Text = "0";
            if (txtOtherDeductions.Text == "")
                txtOtherDeductions.Text = "0";
            txtTotalDeductions.Text = (decimal.Parse(txtEmployeeAdvances.Text) + decimal.Parse(txtPayments.Text) + decimal.Parse(txtOtherDeductions.Text)).ToString();
            txtNetAccruals.Text = (decimal.Parse(txtTotalAdditions.Text) - decimal.Parse(txtTotalDeductions.Text)).ToString();
        }
        private void FormatNumberWithCommas(Guna.UI2.WinForms.Guna2TextBox txtBox)
        {
            if (string.IsNullOrWhiteSpace(txtBox.Text))
                return;

            string rawText = txtBox.Text.Replace(",", "");

            decimal number;
            if (decimal.TryParse(rawText, out number))
            {
                int cursorPosition = txtBox.SelectionStart;
                txtBox.Text = number.ToString("N0");
                txtBox.SelectionStart = txtBox.Text.Length;
            }
        }

        private void txtTotalDeductions_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void txtBasicSalary_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtTransportationAllowance_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtHousingAllowance_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtOthers_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtTotalSalary_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtSalaries_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void txtEndOfService_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtLeaveSalary_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtTotalAdditions_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtEmployeeAdvances_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtNetAccruals_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtTotalSalary_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtSalaries_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtEndOfService_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtLeaveSalary_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtOtherAdditions_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtTotalAdditions_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtEmployeeAdvances_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtPayments_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtOtherDeductions_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtTotalDeductions_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtNetAccruals_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}