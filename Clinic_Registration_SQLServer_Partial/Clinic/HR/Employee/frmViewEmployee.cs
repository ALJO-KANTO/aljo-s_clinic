using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamyProject.Localization;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Clinic_DataAccess;
using Clinic.Login;
using Clinic;

namespace YamyProject
{
    public partial class frmViewEmployee : Form
    {
        int id;
        EventHandler empDept, empPosition, employee;

        // Sudan HR additions
        private Guna.UI2.WinForms.Guna2ComboBox cmbEmployeeType;
        private Guna.UI2.WinForms.Guna2Button btnAddPosition;
        private Guna.UI2.WinForms.Guna2Button btnAddDepartment;
        private string isTeacher = "0"; // Default to not a teacher
        private Form _pageLoadingForm;
        private Thread _pageLoadingThread;
        private volatile bool _pageLoaderReady;

        private bool _pageFinishedLoading;

        // NEW
        private volatile int _pageLoadingPercent = 5;
        private System.Windows.Forms.Timer _pageLoaderTimer;
        public frmViewEmployee(int id = 0)
        {
            StartPageLoading();

            InitializeComponent();

            LocalizationManager.LocalizeForm(this);

            // When HR selects or creates a person in the embedded Person
            // Information section, copy that person's name to Employee Name.
            // This only updates the existing textbox; it does not change saving
            // logic or database structure.
            ctrlPersonCardWithFilter1.OnPersonSelected +=
                CtrlPersonCardWithFilter1_OnPersonSelected;

            empDept = (sender, args) =>
                BindCombos.PopulateDepartments(cmbDepartments);

            empPosition = (sender, args) =>
                BindCombos.PopulatePositions(
                    cmbPosition,
                    cmbDepartments.SelectedValue.ToString() == ""
                        ? 0
                        : (int)cmbDepartments.SelectedValue);

            //.EmployeeDept += empDept;
            //.EmployeePosition += empPosition;

            this.id = id;

            headerUC1.FormText =
                id == 0
                ? "Employee - New Employee"
                : "Employee - Edit Employee";
            ApplyEmployeeLanguageLayout();

            // Some controls are populated or reset after the constructor.
            // Apply the final translated captions once the form is visible.
            this.Shown += (sender, args) =>
                BeginInvoke(new Action(() => ApplyEmployeeLanguageLayout()));
        }
        private void CtrlPersonCardWithFilter1_OnPersonSelected(int personID)
        {
            if (personID <= 0 || ctrlPersonCardWithFilter1 == null)
                return;

            var selectedPerson = ctrlPersonCardWithFilter1.SelectedPerson;
            if (selectedPerson == null)
                return;

            // Person master uses Name for the person's displayed full name.
            txtEmpName.Text = selectedPerson.Name ?? string.Empty;
        }

        private void StartPageLoading()
        {
            _pageLoaderReady = false;
            _pageLoadingPercent = 5;

            bool arabic =
                (Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en")
                .StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            _pageLoadingThread = new Thread(() =>
            {
                Form loader = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.CenterScreen,
                    ShowInTaskbar = false,
                    TopMost = true,

                    Width = 260,
                    Height = 260,

                    BackColor = Color.Black,
                    Opacity = 0.94
                };

                // =====================================================
                // CIRCULAR LOADER
                // =====================================================
                Panel circle = new Panel
                {
                    Width = 150,
                    Height = 150,
                    BackColor = Color.Transparent
                };

                circle.Location = new Point(
                    (loader.ClientSize.Width - circle.Width) / 2,
                    30);

                Label lblPercentage = new Label
                {
                    AutoSize = false,
                    Width = 100,
                    Height = 45,

                    Location = new Point(25, 52),

                    TextAlign = ContentAlignment.MiddleCenter,

                    Font = new Font(
                        "Segoe UI",
                        18F,
                        FontStyle.Bold),

                    ForeColor = Color.FromArgb(150, 220, 255),

                    BackColor = Color.Transparent,

                    Text = "5%"
                };

                circle.Controls.Add(lblPercentage);

                // =====================================================
                // DRAW CIRCLE
                // =====================================================
                circle.Paint += (sender, e) =>
                {
                    e.Graphics.SmoothingMode =
                        System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    int padding = 8;

                    Rectangle rect = new Rectangle(
                        padding,
                        padding,
                        circle.Width - (padding * 2),
                        circle.Height - (padding * 2));

                    // Dark blue background circle
                    using (Pen backgroundPen = new Pen(
                        Color.FromArgb(40, 100, 180), 9))
                    {
                        e.Graphics.DrawArc(
                            backgroundPen,
                            rect,
                            -90,
                            360);
                    }

                    // Blue progress circle
                    using (Pen progressPen = new Pen(
                        Color.FromArgb(90, 190, 255), 11))
                    {
                        progressPen.StartCap =
                            System.Drawing.Drawing2D.LineCap.Round;

                        progressPen.EndCap =
                            System.Drawing.Drawing2D.LineCap.Round;

                        float sweep =
                            360f * _pageLoadingPercent / 100f;

                        e.Graphics.DrawArc(
                            progressPen,
                            rect,
                            -90,
                            sweep);
                    }

                    // Inner glow ring
                    Rectangle innerRect = new Rectangle(
                        17,
                        17,
                        circle.Width - 34,
                        circle.Height - 34);

                    using (Pen glowPen = new Pen(
                        Color.FromArgb(70, 140, 255), 2))
                    {
                        e.Graphics.DrawEllipse(
                            glowPen,
                            innerRect);
                    }
                };

                // =====================================================
                // LOADING TEXT
                // =====================================================
                Label lblLoading = new Label
                {
                    AutoSize = false,

                    Width = loader.ClientSize.Width,
                    Height = 35,

                    Location = new Point(0, 185),

                    TextAlign = ContentAlignment.MiddleCenter,

                    Font = new Font(
                        "Segoe UI",
                        10F,
                        FontStyle.Regular),

                    ForeColor = Color.White,

                    BackColor = Color.Transparent,

                    Text = arabic
                        ? "جاري تحميل بيانات الموظف..."
                        : "Loading employee data..."
                };

                loader.Controls.Add(circle);
                loader.Controls.Add(lblLoading);

                // =====================================================
                // ANIMATION TIMER
                // =====================================================
                _pageLoaderTimer = new System.Windows.Forms.Timer();
                _pageLoaderTimer.Interval = 80;

                _pageLoaderTimer.Tick += (sender, e) =>
                {
                    // Slowly increase while real loading continues.
                    // Never show 100% until loading actually finishes.
                    if (_pageLoadingPercent < 95)
                    {
                        if (_pageLoadingPercent < 50)
                            _pageLoadingPercent += 2;
                        else
                            _pageLoadingPercent += 1;
                    }

                    if (_pageLoadingPercent > 95)
                        _pageLoadingPercent = 95;

                    lblPercentage.Text =
                        _pageLoadingPercent + "%";

                    circle.Invalidate();
                };

                _pageLoadingForm = loader;

                loader.Shown += (sender, e) =>
                {
                    _pageLoaderReady = true;
                    _pageLoaderTimer.Start();
                };

                loader.FormClosed += (sender, e) =>
                {
                    if (_pageLoaderTimer != null)
                    {
                        _pageLoaderTimer.Stop();
                        _pageLoaderTimer.Dispose();
                        _pageLoaderTimer = null;
                    }
                };

                Application.Run(loader);
            });

            _pageLoadingThread.IsBackground = true;
            _pageLoadingThread.SetApartmentState(
                ApartmentState.STA);

            _pageLoadingThread.Start();

            int wait = 0;

            while (!_pageLoaderReady && wait < 100)
            {
                Thread.Sleep(10);
                wait++;
            }
        }

        private void StopPageLoading()
        {
            try
            {
                Form loader = _pageLoadingForm;

                if (loader == null || loader.IsDisposed)
                    return;

                if (loader.InvokeRequired)
                {
                    loader.BeginInvoke(new Action(async () =>
                    {
                        if (loader.IsDisposed)
                            return;

                        // Actual page initialization is complete.
                        _pageLoadingPercent = 100;

                        loader.Refresh();

                        // Small time only to make 100% visible.
                        await Task.Delay(120);

                        if (!loader.IsDisposed)
                            loader.Close();
                    }));
                }
                else
                {
                    _pageLoadingPercent = 100;

                    loader.Refresh();

                    Task.Delay(120).ContinueWith(t =>
                    {
                        try
                        {
                            if (!loader.IsDisposed)
                            {
                                loader.BeginInvoke(new Action(() =>
                                {
                                    if (!loader.IsDisposed)
                                        loader.Close();
                                }));
                            }
                        }
                        catch
                        {
                        }
                    });
                }
            }
            catch
            {
                // Loader must never break employee page.
            }
            finally
            {
                _pageLoaderReady = false;
            }
        }
        private void frmViewEmployee_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.EmployeeDept -= empDept;
            //.EmployeePosition -= empPosition;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void frmViewEmployee_Load(object sender, EventArgs e)
        {
            try
            {
                // =====================================================
                // KEEP ALL YOUR EXISTING LOAD CODE HERE
                // IN THE SAME ORDER.
                // =====================================================

                BindCombos.PopulateCountries(cmbCountry);
                BindCombos.PopulateCountries(cmbCountryOfIssue);

                BindCombos.PopulateAllLevel4Account(cmbAccountName);
                BindCombos.PopulateAllLevel4Account(cmbAccruedSalaries);
                BindCombos.PopulateAllLevel4Account(cmbEmployeeRecivable);
                BindCombos.PopulateAllLevel4Account(cmbAcroalLeaveSalary);
                BindCombos.PopulateAllLevel4Account(cmbPettyCash);
                BindCombos.PopulateAllLevel4Account(cmbGratuit);

                BindCombos.PopulateDepartments(cmbDepartments);
                BindCombos.PopulateRegisterBanks(cmbBankName);

                // Populate employee positions
                int departmentId = cmbDepartments.SelectedValue == null
                    ? 0
                    : Convert.ToInt32(cmbDepartments.SelectedValue);

                BindCombos.PopulatePositions(cmbPosition, departmentId);

                // Load saved employee details when opening an existing employee
                if (id > 0)
                {
                    BindEmployee();
                }

                // =====================================================
                // KEEP THE REST OF YOUR CURRENT LOAD CODE HERE.
                //
                // This includes:
                // positions
                // employee binding
                // edit-mode logic
                // account selections
                // teacher logic
                // images
                // localization
                // permissions
                // etc.
                // =====================================================


                // Only after ALL your current initialization has finished:
                PerformLayout();
                Refresh();

                await Task.Yield();

                Application.DoEvents();

                await Task.Delay(30);
                // LocalizationManager.LocalizeForm(this);

                ApplyEmployeeLanguageLayout();

                PerformLayout();
                Refresh();

                await Task.Yield();
                Application.DoEvents();

                _pageFinishedLoading = true;

                // Reapply after pending control layout/data-binding updates.
                BeginInvoke(new Action(() => ApplyEmployeeLanguageLayout()));
            }
            finally
            {
                StopPageLoading();
            }
        }

        private void BindEmployee()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT tbl_employee.* , tbl_city.country_id FROM tbl_employee left join tbl_city on tbl_employee.city_id = tbl_city.id  WHERE tbl_employee.id = " + id))
                if (reader.Read())
                {
                    txtEmployeeCode.Text = reader["code"].ToString();
                    txtEmpName.Text = reader["name"].ToString();
                    cmbCountry.SelectedValue = string.IsNullOrEmpty(reader["country_id"].ToString()) ? 0 : Convert.ToInt32(reader["country_id"]);
                    cmbCity.SelectedValue = Convert.ToInt32(reader["city_id"]);
                    txtAddress.Text = reader["address"].ToString();

                    txtPhoneNum.Text = reader["phone"].ToString();
                    if (!string.IsNullOrEmpty(reader["Birth_day"].ToString()))
                    {
                        dtOfBirth.Value = Convert.ToDateTime(reader["Birth_day"]);
                    }
                    cmbSocialState.Text = reader["Social_Status"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                    txtSocialInsurance.Text = reader["Social_Insurance_Number"].ToString();

                    txtEmergency.Text = reader["EmergencyName"].ToString();
                    txtAddressEmerg.Text = reader["EmergencyAddress"].ToString();
                    txtPhoneNumEmerg.Text = reader["EmergencyPhone"].ToString();
                    cmbRelation.Text = reader["relation"].ToString();

                    txtBasicSalary.Text = reader["BasicSalary"].ToString();
                    txtHouseingAllowance.Text = reader["HousingAllowance"].ToString();
                    txtTransportaionAllow.Text = reader["TransportationAllowance"].ToString();
                    txtSalaryOther.Text = reader["Other"].ToString();

                    cmbBankName.SelectedValue = Convert.ToInt32(reader["bank_id"]);
                    txtIBanNum.Text = reader["Iban_Number"].ToString();
                    txtBankAccountNumber.Text = reader["Bank_Account_Number"].ToString();

                    txtEmiratesIdFileNum.Text = reader["EmiratesIDFileNumber"].ToString();
                    txtEmiratesIdIssue.Text = reader["EmiratesIDIssuingAuthority"].ToString();
                    dtEmiratesIdIssue.Value = Convert.ToDateTime(reader["EmiratesIDIssueDate"]);
                    dtEmiratesIdExpire.Value = Convert.ToDateTime(reader["EmiratesIDExpiryDate"]);

                    txtPassportNum.Text = reader["PassportNumber"].ToString();
                    cmbCountryOfIssue.Text = reader["CountryOfIssue"].ToString();
                    dtPassportIssue.Value = Convert.ToDateTime(reader["PassportIssueDate"]);
                    dtPassportEnd.Value = Convert.ToDateTime(reader["PassportExpiryDate"]);

                    txtWorkContractNum.Text = reader["WorkContractNumber"].ToString();
                    txtContractType.Text = reader["WorkContractType"].ToString();
                    cmbPosition.SelectedValue = Convert.ToInt32(reader["Position_id"]);
                    cmbDepartments.SelectedValue = Convert.ToInt32(reader["Department_id"]);
                    txtWorkDays.Text = reader["WorkDays"].ToString();
                    txtWorkingHours.Text = reader["workinghours"].ToString();
                    dtContractIssue.Value = Convert.ToDateTime(reader["ContractIssueDate"]);
                    dtContractExpire.Value = Convert.ToDateTime(reader["ContractExpiryDate"]);

                    txtResidencyFileNum.Text = reader["ResidencyFileNumber"].ToString();
                    txtResidencyIssue.Text = reader["ResidencyIssuingAuthority"].ToString();
                    dtResidencyIsseue.Value = Convert.ToDateTime(reader["ResidencyIssueDate"]);
                    dtResidencyExpire.Value = Convert.ToDateTime(reader["ResidencyExpiryDate"]);

                    cmbAccountName.SelectedValue = Convert.ToInt32(reader["Account_id"]);
                    cmbEmployeeRecivable.SelectedValue = Convert.ToInt32(reader["Employee_Recivable_id"]);
                    cmbAccruedSalaries.SelectedValue = Convert.ToInt32(reader["Accrued_Salaries_id"]);
                    cmbAcroalLeaveSalary.SelectedValue = Convert.ToInt32(reader["Acroal_Leave_Salary_id"]);
                    cmbGratuit.SelectedValue = Convert.ToInt32(reader["Gratuit_id"]);
                    cmbPettyCash.SelectedValue = Convert.ToInt32(reader["Petty_Cash_id"]);
                    chkActive.Checked = !Convert.ToBoolean(reader["active"]);

                    object teacherCount = ClinicDatabase.ExecuteScalar(
                        @"SELECT COUNT(*)
                          FROM tbl_employee
                          WHERE id = @employee_id
                            AND state = 0;",
                        ClinicDatabase.CreateParameterNew("@employee_id", id));

                    ChkTeacher.Checked = Convert.ToInt32(teacherCount ?? 0) > 0;
                }
        }
        private void cmbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCountry.SelectedValue != null)
                BindCombos.PopulateCities(cmbCity, (int)cmbCountry.SelectedValue);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                if (insertEmployee())
                {
                    MessageBox.Show(
                        "Employee created successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    frmMain.openChildForm(new MasterEmployee());
                }
            }
            else
            {
                if (updateEmployee())
                {
                    MessageBox.Show(
                        "Employee updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    frmMain.openChildForm(new MasterEmployee());
                }
            }
        }

        private bool updateEmployee()
        {
            if (!chkRequiredDate())
                return false;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where  Name = @name",
               ClinicDatabase.CreateParameterNew("name", txtEmpName.Text)))
                if (reader.Read() && id != int.Parse(reader["id"].ToString()))
                {
                    MessageBox.Show("Employee Name Already Exists. Enter Another Name.");
                    return false;
                }
            ClinicDatabase.ExecuteNonQuery(@"UPDATE `tbl_employee` SET
                        `code` = @code, `name` = @name, `birth_day` = @birth_day, `Social_Status` = @Social_Status,
                        `city_id` = @city_id, `address` = @address, `phone` = @phone, `Email` = @Email,
                        `Social_Insurance_Number` = @Social_Insurance_Number, `EmergencyName` = @EmergencyName,
                        `EmergencyAddress` = @EmergencyAddress, `EmergencyPhone` = @EmergencyPhone, `Relation` = @Relation,
                        `PassportNumber` = @PassportNumber, `CountryOfIssue` = @CountryOfIssue, `PassportIssueDate` = @PassportIssueDate,
                        `PassportExpiryDate` = @PassportExpiryDate, `WorkContractNumber` = @WorkContractNumber, `Position_id` = @Position_id,
                        `Department_id` = @Department_id, `WorkContractType` = @WorkContractType, `WorkDays` = @WorkDays,
                        `workinghours` = @workinghours, `ContractIssueDate` = @ContractIssueDate, `ContractExpiryDate` = @ContractExpiryDate,
                        `ResidencyFileNumber` = @ResidencyFileNumber, `ResidencyIssuingAuthority` = @ResidencyIssuingAuthority,
                        `ResidencyIssueDate` = @ResidencyIssueDate, `ResidencyExpiryDate` = @ResidencyExpiryDate,
                        `EmiratesIDFileNumber` = @EmiratesIDFileNumber, `EmiratesIDIssuingAuthority` = @EmiratesIDIssuingAuthority,
                        `EmiratesIDIssueDate` = @EmiratesIDIssueDate, `EmiratesIDExpiryDate` = @EmiratesIDExpiryDate,
                        `BasicSalary` = @BasicSalary, `HousingAllowance` = @HousingAllowance, `TransportationAllowance` = @TransportationAllowance,
                        `Other` = @Other, `account_id` = @account_id, `bank_id` = @bank_id, `Iban_Number` = @Iban_Number,
                        `Bank_account_Number` = @Bank_account_Number, `Employee_Recivable_id` = @Employee_Recivable_id,
                        `Accrued_Salaries_id` = @Accrued_Salaries_id, `Acroal_Leave_Salary_id` = @Acroal_Leave_Salary_id,
                        `Gratuit_id` = @Gratuit_id,Petty_Cash_id=@Petty_Cash_id, `sRole` = @sRole, `active` = @active, `state` = 0 WHERE `id` = @id;",
                ClinicDatabase.CreateParameterNew("id", id),
                ClinicDatabase.CreateParameterNew("code", txtEmployeeCode.Text),
                ClinicDatabase.CreateParameterNew("name", txtEmpName.Text),
                ClinicDatabase.CreateParameterNew("birth_day", dtOfBirth.Value.Date),
                ClinicDatabase.CreateParameterNew("Social_Status", cmbSocialState.Text),
                ClinicDatabase.CreateParameterNew("city_id", cmbCity.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("address", txtAddress.Text),
                ClinicDatabase.CreateParameterNew("phone", txtPhoneNum.Text),
                ClinicDatabase.CreateParameterNew("Email", txtEmail.Text),
                ClinicDatabase.CreateParameterNew("Social_Insurance_Number", txtSocialInsurance.Text),
                ClinicDatabase.CreateParameterNew("EmergencyName", txtEmergency.Text),
                ClinicDatabase.CreateParameterNew("EmergencyAddress", txtAddressEmerg.Text),
                ClinicDatabase.CreateParameterNew("EmergencyPhone", txtPhoneNumEmerg.Text),
                ClinicDatabase.CreateParameterNew("Relation", cmbRelation.Text),
                ClinicDatabase.CreateParameterNew("PassportNumber", txtPassportNum.Text),
                ClinicDatabase.CreateParameterNew("CountryOfIssue", cmbCountryOfIssue.Text),
                ClinicDatabase.CreateParameterNew("PassportIssueDate", dtPassportIssue.Value.Date),
                ClinicDatabase.CreateParameterNew("PassportExpiryDate", dtPassportEnd.Value.Date),
                ClinicDatabase.CreateParameterNew("WorkContractNumber", txtWorkContractNum.Text),
                ClinicDatabase.CreateParameterNew("Position_id", cmbPosition.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("Department_id", cmbDepartments.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("WorkContractType", txtContractType.Text),
                ClinicDatabase.CreateParameterNew("WorkDays", txtWorkDays.Text),
                ClinicDatabase.CreateParameterNew("workinghours", txtWorkingHours.Text),
                ClinicDatabase.CreateParameterNew("ContractIssueDate", dtContractIssue.Value.Date),
                ClinicDatabase.CreateParameterNew("ContractExpiryDate", dtContractExpire.Value.Date),
                ClinicDatabase.CreateParameterNew("ResidencyFileNumber", txtResidencyFileNum.Text),
                ClinicDatabase.CreateParameterNew("ResidencyIssuingAuthority", txtResidencyIssue.Text),
                ClinicDatabase.CreateParameterNew("ResidencyIssueDate", dtResidencyIsseue.Value.Date),
                ClinicDatabase.CreateParameterNew("ResidencyExpiryDate", dtResidencyExpire.Value.Date),
                ClinicDatabase.CreateParameterNew("EmiratesIDFileNumber", txtEmiratesIdFileNum.Text),
                ClinicDatabase.CreateParameterNew("EmiratesIDIssuingAuthority", txtEmiratesIdIssue.Text),
                ClinicDatabase.CreateParameterNew("EmiratesIDIssueDate", dtEmiratesIdIssue.Value.Date),
                ClinicDatabase.CreateParameterNew("EmiratesIDExpiryDate", dtEmiratesIdExpire.Value.Date),
                ClinicDatabase.CreateParameterNew("BasicSalary", txtBasicSalary.Text),
                ClinicDatabase.CreateParameterNew("HousingAllowance", txtHouseingAllowance.Text),
                ClinicDatabase.CreateParameterNew("TransportationAllowance", txtTransportaionAllow.Text),
                ClinicDatabase.CreateParameterNew("Other", txtSalaryOther.Text),
                ClinicDatabase.CreateParameterNew("account_id", cmbAccountName.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("bank_id", cmbBankName.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("Iban_Number", txtIBanNum.Text),
                ClinicDatabase.CreateParameterNew("Bank_account_Number", txtBankAccountNumber.Text),
                ClinicDatabase.CreateParameterNew("Employee_Recivable_id", cmbEmployeeRecivable.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("Accrued_Salaries_id", cmbAccruedSalaries.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("Acroal_Leave_Salary_id", cmbAcroalLeaveSalary.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("Gratuit_id", cmbGratuit.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("Petty_Cash_id", cmbPettyCash.SelectedValue ?? 0),
                ClinicDatabase.CreateParameterNew("sRole", string.IsNullOrWhiteSpace(cmbEmployeeType.Text) ||
                    string.Equals(cmbEmployeeType.Text.Trim(), "Select Role", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(cmbEmployeeType.Text.Trim(), "اختر المسمى الوظيفي", StringComparison.Ordinal)
                    ? (object)DBNull.Value
                    : TranslateEmployeeTypeValue(cmbEmployeeType.Text, false)),
                ClinicDatabase.CreateParameterNew("active", chkActive.Checked ? 0 : 1));
            ////Utilities.LogAudit(-1, "Update Employee", "Employee", id, GenerateNextEmployeeCode() + ", Updated Employee: " + txtEmpName.Text + ", Code: " + txtEmployeeCode.Text);

            return true;
        }
        private string GenerateNextEmployeeCode()
        {
            int code;
            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(code AS UNSIGNED)) AS lastCode FROM tbl_employee"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                    code = int.Parse(reader["lastCode"].ToString()) + 1;
                else
                    code = 30001;
            }
            return code.ToString("D5");
        }
        private bool insertEmployee()
        {
            if (!chkRequiredDate())
                return false;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_employee where Name = @name",
               ClinicDatabase.CreateParameterNew("name", txtEmpName.Text)))
                if (reader.Read())
                {
                    MessageBox.Show("Employee Name Already Exists. Enter Another Name.");
                    return false;
                }


            int refId = ClinicDatabase.ExecuteNonQuery(@"INSERT INTO `tbl_employee`(`code`, `name`, `birth_day`, `Social_Status`, `city_id`, 
                                `address`, `phone`, `Email`, `Social_Insurance_Number`, `EmergencyName`, 
                                `EmergencyAddress`, `EmergencyPhone`, `Relation`, `PassportNumber`, `CountryOfIssue`, 
                                `PassportIssueDate`, `PassportExpiryDate`, `WorkContractNumber`, `Position_id`, 
                                `Department_id`, `WorkContractType`, `WorkDays`, `workinghours`, `ContractIssueDate`, 
                                `ContractExpiryDate`, `ResidencyFileNumber`, `ResidencyIssuingAuthority`, 
                                `ResidencyIssueDate`, `ResidencyExpiryDate`, `EmiratesIDFileNumber`, 
                                `EmiratesIDIssuingAuthority`, `EmiratesIDIssueDate`, `EmiratesIDExpiryDate`, 
                                `BasicSalary`, `HousingAllowance`, `TransportationAllowance`, `Other`, 
                                `account_id`, `bank_id`, `Iban_Number`, `Bank_account_Number`, 
                                `Employee_Recivable_id`, `Accrued_Salaries_id`, `Acroal_Leave_Salary_id`, `Gratuit_id`,Petty_Cash_id, `sRole`,
                                `active`, `state`) 
                        VALUES(@code, @name, @birth_day, @Social_Status, @city_id, @address, @phone, @Email, 
                               @Social_Insurance_Number, @EmergencyName, @EmergencyAddress, @EmergencyPhone, 
                               @Relation, @PassportNumber, @CountryOfIssue, @PassportIssueDate, @PassportExpiryDate, 
                               @WorkContractNumber, @Position_id, @Department_id, @WorkContractType, @WorkDays, 
                               @workinghours, @ContractIssueDate, @ContractExpiryDate, @ResidencyFileNumber, 
                               @ResidencyIssuingAuthority, @ResidencyIssueDate, @ResidencyExpiryDate, 
                               @EmiratesIDFileNumber, @EmiratesIDIssuingAuthority, @EmiratesIDIssueDate, 
                               @EmiratesIDExpiryDate, @BasicSalary, @HousingAllowance, @TransportationAllowance, 
                               @Other, @account_id, @bank_id, @Iban_Number, @Bank_account_Number, 
                               @Employee_Recivable_id, @Accrued_Salaries_id, @Acroal_Leave_Salary_id, @Gratuit_id,@Petty_Cash_id, @sRole,
                               @active, 0);",
                   ClinicDatabase.CreateParameterNew("code", GenerateNextEmployeeCode()),
                   ClinicDatabase.CreateParameterNew("name", txtEmpName.Text),
                   ClinicDatabase.CreateParameterNew("birth_day", dtOfBirth.Value.Date),
                   ClinicDatabase.CreateParameterNew("Social_Status", cmbSocialState.Text),
                   ClinicDatabase.CreateParameterNew("city_id", cmbCity.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("address", txtAddress.Text),
                   ClinicDatabase.CreateParameterNew("phone", txtPhoneNum.Text),
                   ClinicDatabase.CreateParameterNew("Email", txtEmail.Text),
                   ClinicDatabase.CreateParameterNew("Social_Insurance_Number", txtSocialInsurance.Text),
                   ClinicDatabase.CreateParameterNew("EmergencyName", txtEmergency.Text),
                   ClinicDatabase.CreateParameterNew("EmergencyAddress", txtAddressEmerg.Text),
                   ClinicDatabase.CreateParameterNew("EmergencyPhone", txtPhoneNumEmerg.Text),
                   ClinicDatabase.CreateParameterNew("Relation", cmbRelation.Text),
                   ClinicDatabase.CreateParameterNew("PassportNumber", txtPassportNum.Text),
                   ClinicDatabase.CreateParameterNew("CountryOfIssue", cmbCountryOfIssue.Text),
                   ClinicDatabase.CreateParameterNew("PassportIssueDate", dtPassportIssue.Value.Date),
                   ClinicDatabase.CreateParameterNew("PassportExpiryDate", dtPassportEnd.Value.Date),
                   ClinicDatabase.CreateParameterNew("WorkContractNumber", txtWorkContractNum.Text),
                   ClinicDatabase.CreateParameterNew("Position_id", cmbPosition.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("Department_id", cmbDepartments.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("WorkContractType", txtContractType.Text),
                   ClinicDatabase.CreateParameterNew("WorkDays", txtWorkDays.Text),
                   ClinicDatabase.CreateParameterNew("workinghours", txtWorkingHours.Text),
                   ClinicDatabase.CreateParameterNew("ContractIssueDate", dtContractIssue.Value.Date),
                   ClinicDatabase.CreateParameterNew("ContractExpiryDate", dtContractExpire.Value.Date),
                   ClinicDatabase.CreateParameterNew("ResidencyFileNumber", txtResidencyFileNum.Text),
                   ClinicDatabase.CreateParameterNew("ResidencyIssuingAuthority", txtResidencyIssue.Text),
                   ClinicDatabase.CreateParameterNew("ResidencyIssueDate", dtResidencyIsseue.Value.Date),
                   ClinicDatabase.CreateParameterNew("ResidencyExpiryDate", dtResidencyExpire.Value.Date),
                   ClinicDatabase.CreateParameterNew("EmiratesIDFileNumber", txtEmiratesIdFileNum.Text),
                   ClinicDatabase.CreateParameterNew("EmiratesIDIssuingAuthority", txtEmiratesIdIssue.Text),
                   ClinicDatabase.CreateParameterNew("EmiratesIDIssueDate", dtEmiratesIdIssue.Value.Date),
                   ClinicDatabase.CreateParameterNew("EmiratesIDExpiryDate", dtEmiratesIdExpire.Value.Date),
                   ClinicDatabase.CreateParameterNew("BasicSalary", txtBasicSalary.Text),
                   ClinicDatabase.CreateParameterNew("HousingAllowance", txtHouseingAllowance.Text),
                   ClinicDatabase.CreateParameterNew("TransportationAllowance", txtTransportaionAllow.Text),
                   ClinicDatabase.CreateParameterNew("Other", txtSalaryOther.Text),
                   ClinicDatabase.CreateParameterNew("account_id", cmbAccountName.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("bank_id", cmbBankName.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("Iban_Number", txtIBanNum.Text),
                   ClinicDatabase.CreateParameterNew("Bank_account_Number", txtBankAccountNumber.Text),
                   ClinicDatabase.CreateParameterNew("Employee_Recivable_id", cmbEmployeeRecivable.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("Accrued_Salaries_id", cmbAccruedSalaries.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("Acroal_Leave_Salary_id", cmbAcroalLeaveSalary.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("Gratuit_id", cmbGratuit.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("Petty_Cash_id", cmbPettyCash.SelectedValue ?? 0),
                   ClinicDatabase.CreateParameterNew("sRole", string.IsNullOrWhiteSpace(cmbEmployeeType.Text) ||
                    string.Equals(cmbEmployeeType.Text.Trim(), "Select Role", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(cmbEmployeeType.Text.Trim(), "اختر المسمى الوظيفي", StringComparison.Ordinal)
                    ? (object)DBNull.Value
                    : TranslateEmployeeTypeValue(cmbEmployeeType.Text, false)),
                   ClinicDatabase.CreateParameterNew("active", chkActive.Checked ? 0 : 1));

            if (refId != 0)
            {
                // Keep the inserted employee id so a checked Teacher can be
                // linked to this employee immediately after saving.
                id = refId;

                if (ChkTeacher.Checked)
                {
                    string salt;
                    ClinicDatabase.ExecuteNonQuery(@"insert into tbl_sec_users (user_name,passwordhash,salt,emp_id,first_name,last_name,role_id,active,state)values 
                                         (@user_name,@passwordhash,@salt,@emp_id,@first_name,@last_name,@role_id,@active,0)",
                 ClinicDatabase.CreateParameterNew("user_name", txtEmpName.Text.Trim()),
                 ClinicDatabase.CreateParameterNew("passwordhash", PasswordHelper.HashPassword(txtEmpName.Text.Trim(), out salt)),
                   ClinicDatabase.CreateParameterNew("emp_id", id),
                   ClinicDatabase.CreateParameterNew("first_name", txtEmpName.Text.Trim()),
                   ClinicDatabase.CreateParameterNew("salt", salt),
                   ClinicDatabase.CreateParameterNew("last_name", "."),
                   ClinicDatabase.CreateParameterNew("role_id", "10"),
                   ClinicDatabase.CreateParameterNew("active", "0"));
                }


                ////Utilities.LogAudit(-1, "Insert Employee", "Employee", refId,
                //"/Inserted Employee: " + txtEmpName.Text + ", Code: " + txtEmployeeCode.Text);

                return true;
            }
            else
            {
                string message = Utilities.LastLogError();
                MessageBox.Show("Error Check your data" + message);
                return false;
            }
        }
        private bool chkRequiredDate()
        {
            if (txtPhoneNum.Text.Trim() == "")
            {
                MessageBox.Show("Enter Main Phone First.");
                tbC.SelectedIndex = 0;
                txtPhoneNum.Focus();
                return false;
            }

            if (txtEmpName.Text.Trim() == "")
            {
                MessageBox.Show("Enter Employee Name First");
                tbC.SelectedIndex = 0;
                txtEmpName.Focus();
                return false;
            }
            if (cmbCity.SelectedValue == null)
            {
                MessageBox.Show("Choose City First");
                return false;
            }
            if (dtOfBirth.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Date Value Must Be Less Or Equal Today");
                tbC.SelectedIndex = 0;
                return false;
            }
            if (txtBasicSalary.Text.Trim() == "")
            {
                MessageBox.Show("Enter Basic Salary Please.");
                tbC.SelectedIndex = 0;
                txtBasicSalary.Focus();
                return false;
            }
            if (txtWorkingHours.Text.Trim() == "")
            {
                MessageBox.Show("Enter Working Hours Please.");
                tbC.SelectedIndex = 1;
                txtWorkingHours.Focus();
                return false;
            }
            if (txtWorkDays.Text.Trim() == "")
            {
                MessageBox.Show("Enter Work Days Please.");
                tbC.SelectedIndex = 1;
                txtWorkDays.Focus();
                return false;
            }

            //if (cmbAccountName.Text.Trim() == "")
            //{
            //    MessageBox.Show("Enter Account Name First");
            //    tbC.SelectedIndex = 1;
            //    cmbAccountName.Focus();
            //    return false;
            //}
            if (cmbPosition.Text.Trim() == "")
            {
                MessageBox.Show("Enter Position Name First");
                tbC.SelectedIndex = 1;
                cmbPosition.Focus();
                return false;
            }
            if (cmbDepartments.Text.Trim() == "")
            {
                MessageBox.Show("Enter Department Name First");
                tbC.SelectedIndex = 1;
                cmbDepartments.Focus();
                return false;
            }
            //if (cmbAccountName.Text.Trim() == "")
            //{
            //    MessageBox.Show("Enter Account Name First");
            //    tbC.SelectedIndex = 1;
            //    cmbAccountName.Focus();
            //    return false;
            //}
            if (txtAccruedSalaries.Text == "")
                txtAccruedSalaries.Text = "0";
            if (txtHouseingAllowance.Text == "")
                txtHouseingAllowance.Text = "0";
            if (txtTransportaionAllow.Text == "")
                txtTransportaionAllow.Text = "0";
            if (txtSalaryOther.Text == "")
                txtSalaryOther.Text = "0";
            if (txtEmployeeRecivable.Text == "")
                txtEmployeeRecivable.Text = "0";
            if (txtAcroalLeaveSalary.Text == "")
                txtAcroalLeaveSalary.Text = "0";
            if (txtGratuit.Text == "")
                txtGratuit.Text = "0";
            if (txtPettyCash.Text == "")
                txtPettyCash.Text = "0";
            return true;
        }

        private void cmbAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountName.SelectedValue == null)
            {
                txtAccountCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbAccountName.SelectedValue.ToString()))
                if (reader.Read())
                    txtAccountCode.Text = reader["code"].ToString();
                else
                    txtAccountCode.Text = "";
        }

        private void txtBasicSalary_TextChanged(object sender, EventArgs e)
        {
            decimal basicSalary = string.IsNullOrWhiteSpace(txtBasicSalary.Text) ? 0 : Convert.ToDecimal(txtBasicSalary.Text);
            decimal housingAllowance = string.IsNullOrWhiteSpace(txtHouseingAllowance.Text) ? 0 : Convert.ToDecimal(txtHouseingAllowance.Text);
            decimal transportationAllowance = string.IsNullOrWhiteSpace(txtTransportaionAllow.Text) ? 0 : Convert.ToDecimal(txtTransportaionAllow.Text);
            decimal otherSalary = string.IsNullOrWhiteSpace(txtSalaryOther.Text) ? 0 : Convert.ToDecimal(txtSalaryOther.Text);

            decimal totalSalary = basicSalary + housingAllowance + transportationAllowance + otherSalary;
            txtTotalSalary.Text = totalSalary.ToString();
        }

        private void txtBasicSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }

        private void txtAccountCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtAccountCode.Text)))
                if (reader.Read())
                    cmbAccountName.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtAccountCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
            ClinicDatabase.CreateParameterNew("code", txtAccountCode.Text)))
                if (!reader.Read())
                    cmbAccountName.SelectedIndex = -1;
        }

        private void cmbAccruedSalaries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccruedSalaries.SelectedValue == null)
            {
                txtAccruedSalaries.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbAccruedSalaries.SelectedValue.ToString()))
                if (reader.Read())
                    txtAccruedSalaries.Text = reader["code"].ToString();
                else
                    txtAccruedSalaries.Text = "";
        }

        private void cmbEmployeeRecivable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployeeRecivable.SelectedValue == null)
            {
                txtEmployeeRecivable.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbEmployeeRecivable.SelectedValue.ToString()))
                if (reader.Read())
                    txtEmployeeRecivable.Text = reader["code"].ToString();
                else
                    txtEmployeeRecivable.Text = "";
        }

        private void cmbAcroalLeaveSalary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAcroalLeaveSalary.SelectedValue == null)
            {
                txtAcroalLeaveSalary.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbAcroalLeaveSalary.SelectedValue.ToString()))
                if (reader.Read())
                    txtAcroalLeaveSalary.Text = reader["code"].ToString();
                else
                    txtAcroalLeaveSalary.Text = "";
        }

        private void cmbGratuit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGratuit.SelectedValue == null)
            {
                txtGratuit.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbGratuit.SelectedValue.ToString()))
                if (reader.Read())
                    txtGratuit.Text = reader["code"].ToString();
                else
                    txtGratuit.Text = "";
        }

        private void txtBasicSalary_KeyPress(object sender, EventArgs e)
        {

        }

        private void lnkTax_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenDepartmentPopup();
        }

        private void btnAddDepartment_Click(object sender, EventArgs e)
        {
            OpenDepartmentPopup();
        }

        private void OpenDepartmentPopup()
        {
            int oldDepartmentId = 0;
            if (cmbDepartments.SelectedValue != null)
                int.TryParse(cmbDepartments.SelectedValue.ToString(), out oldDepartmentId);

            using (frmViewDepartments frm = new frmViewDepartments())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }

            BindCombos.PopulateDepartments(cmbDepartments);

            if (oldDepartmentId > 0)
            {
                try { cmbDepartments.SelectedValue = oldDepartmentId; }
                catch { }
            }

            int departmentId = 0;
            if (cmbDepartments.SelectedValue != null)
                int.TryParse(cmbDepartments.SelectedValue.ToString(), out departmentId);

            BindCombos.PopulatePositions(cmbPosition, departmentId);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenJobPositionPopup();
        }

        private void btnAddPosition_Click(object sender, EventArgs e)
        {
            OpenJobPositionPopup();
        }

        private void OpenJobPositionPopup()
        {
            // Use the project's existing Job Position form as the popup.
            // After it closes, immediately refresh the Position dropdown.
            using (frmViewPosition frm = new frmViewPosition())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }

            int departmentId = 0;
            if (cmbDepartments.SelectedValue != null)
                int.TryParse(cmbDepartments.SelectedValue.ToString(), out departmentId);

            BindCombos.PopulatePositions(cmbPosition, departmentId);
        }

        private void cmbDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCombos.PopulatePositions(cmbPosition, cmbDepartments.SelectedValue == null ? 0 : (int)cmbDepartments.SelectedValue);
        }

        private void cmbBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBankName.SelectedValue == null)
            {
                txtBankCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_bank where id = " + cmbBankName.SelectedValue.ToString()))
                if (reader.Read())
                    txtBankCode.Text = reader["code"].ToString();
                else
                    txtBankCode.Text = "";
        }

        private void txtBankCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_bank where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtBankCode.Text)))
                if (reader.Read())
                    cmbBankName.SelectedValue = int.Parse(reader["id"].ToString());

        }

        private void txtBankCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_bank where code =@code",
            ClinicDatabase.CreateParameterNew("code", txtBankCode.Text)))
                if (!reader.Read())
                    cmbBankName.SelectedIndex = -1;
        }

        private void txtAccruedSalaries_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
              ClinicDatabase.CreateParameterNew("code", txtAccruedSalaries.Text)))
                if (reader.Read())
                    cmbAccruedSalaries.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtEmployeeRecivable_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
              ClinicDatabase.CreateParameterNew("code", txtEmployeeRecivable.Text)))
                if (reader.Read())
                    cmbEmployeeRecivable.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtAcroalLeaveSalary_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
              ClinicDatabase.CreateParameterNew("code", txtAcroalLeaveSalary.Text)))
                if (reader.Read())
                    cmbAcroalLeaveSalary.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtGratuit_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
              ClinicDatabase.CreateParameterNew("code", cmbGratuit.Text)))
                if (reader.Read())
                    cmbGratuit.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtGratuit_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
          ClinicDatabase.CreateParameterNew("code", cmbGratuit.Text)))
                if (!reader.Read())
                    cmbGratuit.SelectedIndex = -1;
        }

        private void txtAcroalLeaveSalary_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
          ClinicDatabase.CreateParameterNew("code", txtAcroalLeaveSalary.Text)))
                if (!reader.Read())
                    cmbAcroalLeaveSalary.SelectedIndex = -1;
        }

        private void txtEmployeeRecivable_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
          ClinicDatabase.CreateParameterNew("code", txtEmployeeRecivable.Text)))
                if (!reader.Read())
                    cmbEmployeeRecivable.SelectedIndex = -1;
        }

        private void cmbPettyCash_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPettyCash.SelectedValue == null)
            {
                txtPettyCash.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbPettyCash.SelectedValue.ToString()))
                if (reader.Read())
                    txtPettyCash.Text = reader["code"].ToString();
                else
                    txtPettyCash.Text = "";
        }

        private void txtPettyCash_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBasicSalary_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void txtTransportaionAllow_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtHouseingAllowance_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtSalaryOther_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtAccruedSalaries_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
          ClinicDatabase.CreateParameterNew("code", txtAccruedSalaries.Text)))
                if (!reader.Read())
                    cmbAccruedSalaries.SelectedIndex = -1;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmViewCity().ShowDialog();
        }
        private static bool ContainsArabicCharacters(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (char c in value)
            {
                if ((c >= '\u0600' && c <= '\u06FF') ||
                    (c >= '\u0750' && c <= '\u077F') ||
                    (c >= '\u08A0' && c <= '\u08FF'))
                    return true;
            }
            return false;
        }

        public void ApplyEmployeeLanguageLayout()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            // The application can localize section captions through RESX even when
            // its saved language or current thread culture is still English.
            // Detect the language already displayed by the employee page as well.
            bool hasArabicCaption =
                ContainsArabicCharacters(label13.Text) ||
                ContainsArabicCharacters(label11.Text) ||
                ContainsArabicCharacters(label14.Text) ||
                ContainsArabicCharacters(label2.Text) ||
                ContainsArabicCharacters(label8.Text);

            bool isArabic =
                language.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ||
                System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith(
                    "ar", StringComparison.OrdinalIgnoreCase) ||
                hasArabicCaption;

            SuspendLayout();
            modernHeader.SuspendLayout();
            TabPage1.SuspendLayout();
            TabPage2.SuspendLayout();

            try
            {
                // IMPORTANT:
                // Do NOT mirror the complete Form automatically.
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                if (isArabic)
                {
                    ApplyEmployeeArabicLayout();
                }
                else
                {
                    ApplyEmployeeEnglishLayout();
                }

                ApplyEmployeeArabicText(isArabic);
            }
            finally
            {
                TabPage2.ResumeLayout(true);
                TabPage1.ResumeLayout(true);
                modernHeader.ResumeLayout(true);
                ResumeLayout(true);

                PerformLayout();
            }
        }


        private void ApplyEmployeeArabicText(bool arabic)
        {
            // Translate by control name, independent of RESX keys and current text.
            btnClose.Text = arabic ? "إغلاق" : "Close";
            btnSave.Text = arabic ? "حفظ" : "Save";
            TabPage1.Text = arabic ? "معلومات الموظف" : "Employee Information";
            lblPersonInformationTitle.Text = arabic ? "معلومات الشخص" : "Person Information";
            ChkTeacher.Text = arabic ? "معلم" : "Teacher";
            label35.Text = arabic ? "رقم التأمين الاجتماعي" : "Social Insurance NO";
            label34.Text = arabic ? "البريد الإلكتروني" : "Email";
            label44.Text = arabic ? "رقم الهاتف" : "Phone Number";
            label32.Text = arabic ? "رقم الهاتف" : "Phone Number";
            label31.Text = arabic ? "العنوان" : "Address";
            chkActive.Text = arabic ? "الموظف نشط" : "Employee Active";
            label1.Text = arabic ? "صلة القرابة" : "Relation";
            label22.Text = arabic ? "الهوية الإماراتية" : "Emirates ID";
            label21.Text = arabic ? "تاريخ انتهاء الهوية" : "Emirates ID Expiry Date";
            label20.Text = arabic ? "تاريخ إصدار الهوية" : "Emirates ID Issue Date";
            label9.Text = arabic ? "المعلومات البنكية" : "Bank Info";
            label8.Text = arabic ? "معلومات الراتب" : "Salary Info";
            label2.Text = arabic ? "المعلومات الأساسية" : "Basic Info";
            label46.Text = arabic ? "معلومات إضافية" : "Additional Info";
            Label33.Text = arabic ? "معلومات الطوارئ" : "Emergency Info";
            Label25.Text = arabic ? "المدينة" : "City";
            label43.Text = arabic ? "العنوان" : "Address";
            label42.Text = arabic ? "الاسم" : "Name";
            label30.Text = arabic ? "اسم الموظف" : "Employee Name";
            label38.Text = arabic ? "بدل السكن" : "House Allowance";
            label47.Text = arabic ? "رقم الآيبان" : "IBAN Number";
            label49.Text = arabic ? "رمز البنك" : "Bank Code";
            label51.Text = arabic ? "جهة إصدار الهوية" : "Emirates ID Issuing Authority";
            label50.Text = arabic ? "رقم الهوية الوطنية" : "National ID";
            label48.Text = arabic ? "رقم الحساب البنكي" : "Bank Account Number";
            label45.Text = arabic ? "اسم البنك" : "Bank Name";
            label41.Text = arabic ? "إجمالي الراتب" : "Total Salary";
            label40.Text = arabic ? "بدلات أخرى" : "Others";
            label39.Text = arabic ? "بدل المواصلات" : "Transportation Allowance";
            label37.Text = arabic ? "الراتب الأساسي" : "Basic Salary";
            label36.Text = arabic ? "رمز الموظف" : "Employee Code";
            Label24.Text = arabic ? "الدولة" : "Country";
            Label6.Text = arabic ? "تاريخ الميلاد" : "Date Of Birth";
            Label7.Text = arabic ? "الحالة الاجتماعية" : "Social Status";
            linkLabel2.Text = arabic ? "جديد" : "New";
            TabPage2.Text = arabic ? "المستندات القانونية" : "Legal Document";
            label66.Text = arabic ? "رمز الحساب" : "AC / Code";
            label67.Text = arabic ? "العهدة النقدية" : "Petty Cash";
            label53.Text = arabic ? "رقم عقد العمل" : "Work Contract Number";
            label65.Text = arabic ? "الأقسام" : "Departments";
            label55.Text = arabic ? "نوع العقد" : "Contract Type";
            label54.Text = arabic ? "المنصب الوظيفي" : "Position";
            label56.Text = arabic ? "أيام العمل" : "Work Days";
            label57.Text = arabic ? "ساعات العمل" : "Working Hours";
            label18.Text = arabic ? "تاريخ إصدار العقد" : "Contract Issue Date";
            label19.Text = arabic ? "تاريخ انتهاء العقد" : "Contract Expiry Date";
            linkLabel1.Text = arabic ? "جديد" : "New";
            lnkTax.Text = arabic ? "جديد" : "New";
            label26.Text = arabic ? "الرواتب المستحقة" : "Accrued Salaries";
            label59.Text = arabic ? "رمز الحساب" : "AC / Code";
            label27.Text = arabic ? "ذمم الموظفين المدينة" : "Employee Receivable";
            label60.Text = arabic ? "رمز الحساب" : "AC / Code";
            label28.Text = arabic ? "مخصص رواتب الإجازات" : "Accrual Leave Salary";
            label61.Text = arabic ? "رمز الحساب" : "AC / Code";
            label29.Text = arabic ? "مكافأة نهاية الخدمة" : "Gratuit";
            label62.Text = arabic ? "رمز الحساب" : "AC / Code";
            label63.Text = arabic ? "رقم ملف الإقامة" : "Residency File Number";
            label64.Text = arabic ? "جهة إصدار الإقامة" : "Residency Issue Authority";
            label10.Text = arabic ? "تاريخ إصدار الإقامة" : "Residency Issue Date";
            label12.Text = arabic ? "تاريخ انتهاء الإقامة" : "Residency Expiry Date";
            label52.Text = arabic ? "رقم جواز السفر" : "Passport Number";
            label15.Text = arabic ? "بلد الإصدار" : "Country Of Issue";
            label16.Text = arabic ? "تاريخ إصدار جواز السفر" : "Passport Issue Date";
            label17.Text = arabic ? "تاريخ انتهاء جواز السفر" : "Passport End Date";
            label58.Text = arabic ? "رمز الحساب" : "AC / Code";
            label3.Text = arabic ? "اسم الحساب" : "Account Name";
            label23.Text = arabic ? "حساب المصروفات (دليل الحسابات)" : "Expense Account (Chart - Account Name)";
            label13.Text = arabic ? "الإقامة" : "Residency";
            label11.Text = arabic ? "عقد العمل" : "Work Contract";
            label14.Text = arabic ? "جواز السفر" : "Passport";
            TabPage1.Text = arabic ? "معلومات الموظف" : "Employee Information";
            TabPage2.Text = arabic ? "المستندات القانونية" : "Legal Document";
            // The Employee Role caption is created separately from the designer labels.
            // Find it recursively so its parent panel does not matter.
            TranslateEmployeeTypeControls(this, arabic);
            if (cmbEmployeeType != null)
            {
                cmbEmployeeType.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;

                // Keep actual item values unchanged when the combo is data-bound.
                if (cmbEmployeeType.DataSource != null)
                {
                    cmbEmployeeType.FormattingEnabled = true;
                    cmbEmployeeType.Format -= CmbEmployeeType_Format;
                    cmbEmployeeType.Format += CmbEmployeeType_Format;
                    cmbEmployeeType.Refresh();
                }
                else
                {
                    int selectedIndex = cmbEmployeeType.SelectedIndex;
                    for (int i = 0; i < cmbEmployeeType.Items.Count; i++)
                    {
                        if (!(cmbEmployeeType.Items[i] is string item)) continue;
                        string translated = TranslateEmployeeTypeValue(item, arabic);
                        if (translated != item) cmbEmployeeType.Items[i] = translated;
                    }
                    if (selectedIndex >= 0 && selectedIndex < cmbEmployeeType.Items.Count)
                        cmbEmployeeType.SelectedIndex = selectedIndex;
                }
            }
        }

        private static string TranslateEmployeeTypeValue(string value, bool arabic)
        {
            string[,] roles =
            {
                { "Select Role", "اختر المسمى الوظيفي" },
                { "Doctor", "طبيب" },
                { "Dentist", "طبيب أسنان" },
                { "Nurse", "ممرض" },
                { "Pharmacist", "صيدلي" },
                { "Laboratory Technician", "فني مختبر" },
                { "Radiology Technician", "فني أشعة" },
                { "Physiotherapist", "أخصائي علاج طبيعي" },
                { "Medical Coder", "أخصائي ترميز طبي" },
                { "Receptionist", "موظف استقبال" },
                { "Billing Officer", "مسؤول الفواتير" },
                { "Insurance Coordinator", "منسق التأمين" },
                { "Accountant", "محاسب" },
                { "HR Officer", "مسؤول الموارد البشرية" },
                { "Clinic Manager", "مدير العيادة" },
                { "Medical Records Officer", "مسؤول السجلات الطبية" },
                { "Admin Staff", "موظف إداري" },
                { "Support Staff", "موظف دعم" },
                { "Housekeeping Staff", "موظف خدمات" },
                { "Other", "أخرى" }
            };

            for (int i = 0; i < roles.GetLength(0); i++)
            {
                if (string.Equals(value, roles[i, 0], StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(value, roles[i, 1], StringComparison.Ordinal))
                {
                    return arabic ? roles[i, 1] : roles[i, 0];
                }
            }

            return value;
        }

        private void CmbEmployeeType_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem == null) return;
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            bool arabic = language.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ||
                          ContainsArabicCharacters(label13.Text);
            string displayed = e.Value == null ? "" : e.Value.ToString();
            e.Value = TranslateEmployeeTypeValue(displayed, arabic);
        }

        private static void TranslateEmployeeTypeControls(Control root, bool arabic)
        {
            foreach (Control child in root.Controls)
            {
                if ((child is Label || child is Guna.UI2.WinForms.Guna2HtmlLabel) &&
                    (string.Equals(child.Text.Trim(), "Employee Role", StringComparison.OrdinalIgnoreCase) ||
                     child.Text.Trim() == "المسمى الوظيفي" ||
                     string.Equals(child.Text.Trim(), "Employee Type", StringComparison.OrdinalIgnoreCase) ||
                     child.Text.Trim() == "نوع الموظف"))
                {
                    child.Text = arabic ? "المسمى الوظيفي" : "Employee Role";
                    child.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
                }
                if (child.HasChildren) TranslateEmployeeTypeControls(child, arabic);
            }
        }

        private void ApplyEmployeeArabicLayout()
        {
            // =========================================================
            // FORM / OLD HEADER
            // =========================================================

            Text = id == 0
                ? "الموظف - موظف جديد"
                : "الموظف - تعديل الموظف";

            headerUC1.FormText = id == 0
                ? "الموظف - موظف جديد"
                : "الموظف - تعديل الموظف";

            headerUC1.RightToLeft = RightToLeft.Yes;


            // =========================================================
            // MODERN HEADER
            //
            // English:
            // Title     = LEFT
            // Breadcrumb = RIGHT
            // Back       = FAR RIGHT
            //
            // Arabic:
            // Title      = RIGHT
            // Breadcrumb = LEFT
            // Back       = FAR LEFT
            // =========================================================

            lblModernTitle.Text = id == 0
                ? "الموظف - موظف جديد"
                : "الموظف - تعديل الموظف";

            lblModernSubtitle.Text = id == 0
                ? "إضافة موظف جديد إلى مؤسستك"
                : "عرض وتعديل بيانات الموظف";

            lblModernBreadcrumb.Text = id == 0
                ? "الموارد البشرية   ‹   الموظفون   ‹   موظف جديد"
                : "الموارد البشرية   ‹   الموظفون   ‹   تعديل الموظف";

            btnBackEmployees.Text = "العودة إلى الموظفين  →";


            // ---------- TITLE -> RIGHT ----------

            lblModernTitle.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            lblModernSubtitle.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            lblModernTitle.RightToLeft = RightToLeft.Yes;
            lblModernSubtitle.RightToLeft = RightToLeft.Yes;

            lblModernTitle.TextAlign =
                ContentAlignment.MiddleRight;

            lblModernSubtitle.TextAlign =
                ContentAlignment.MiddleRight;


            int rightMargin = 50;

            lblModernTitle.Location = new Point(
                modernHeader.ClientSize.Width
                    - rightMargin
                    - lblModernTitle.PreferredWidth,
                22);

            lblModernSubtitle.Location = new Point(
                modernHeader.ClientSize.Width
                    - rightMargin
                    - lblModernSubtitle.PreferredWidth,
                64);


            // ---------- BACK BUTTON -> LEFT ----------

            btnBackEmployees.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left;

            btnBackEmployees.Location =
                new Point(50, 22);

            btnBackEmployees.RightToLeft =
                RightToLeft.Yes;


            // ---------- BREADCRUMB -> LEFT ----------

            lblModernBreadcrumb.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left;

            lblModernBreadcrumb.RightToLeft =
                RightToLeft.Yes;

            lblModernBreadcrumb.TextAlign =
                ContentAlignment.MiddleLeft;

            lblModernBreadcrumb.Location =
                new Point(
                    btnBackEmployees.Right + 35,
                    33);


            // =========================================================
            // TAB CONTROL
            // =========================================================

            tbC.RightToLeft = RightToLeft.Yes;

            TabPage1.Text = "معلومات الموظف";
            TabPage2.Text = "المستندات القانونية";

            TabPage1.RightToLeft = RightToLeft.Yes;
            TabPage2.RightToLeft = RightToLeft.Yes;


            // =========================================================
            // LEGAL DOCUMENT PAGE
            //
            // IMPORTANT:
            // Your page width is fixed at 1560.
            //
            // English:
            // Passport  | Residency
            // Contract  | Expense
            //
            // Arabic:
            // Residency | Passport
            // Expense   | Contract
            //
            // Visually:
            //
            //             Passport       Residency
            //             RIGHT          LEFT
            //
            //             Contract       Expense
            //             RIGHT          LEFT
            //
            // =========================================================

            TabPage2.AutoScrollMinSize =
                new Size(1560, 724);

            int leftX = 20;
            int rightX = 772;

            // Passport moves RIGHT
            cardPassport.Location =
                new Point(rightX, 20);

            // Residency moves LEFT
            cardResidency.Location =
                new Point(leftX, 20);

            // Work Contract moves RIGHT
            cardContract.Location =
                new Point(rightX, 250);

            // Expense Account moves LEFT
            cardExpense.Location =
                new Point(leftX, 250);


            // =========================================================
            // CARD DIRECTION
            // =========================================================

            ApplyCardArabic(cardPassport);
            ApplyCardArabic(cardResidency);
            ApplyCardArabic(cardContract);
            ApplyCardArabic(cardExpense);


            // =========================================================
            // CARD TITLES
            // =========================================================

            cardPassportTitle.Text =
                "▣   جواز السفر";

            cardResidencyTitle.Text =
                "⌂   الإقامة";

            cardContractTitle.Text =
                "▣   عقد العمل";

            cardExpenseTitle.Text =
                "•   حسابات مصروفات الموظف";


            PositionCardTitleRight(
                cardPassport,
                cardPassportTitle);

            PositionCardTitleRight(
                cardResidency,
                cardResidencyTitle);

            PositionCardTitleRight(
                cardContract,
                cardContractTitle);

            PositionCardTitleRight(
                cardExpense,
                cardExpenseTitle);


            // =========================================================
            // FOOTER BUTTONS
            //
            // English buttons are RIGHT.
            // Arabic buttons go LEFT.
            // =========================================================

            btnSave.Text = "حفظ";
            btnClose.Text = "إغلاق";

            btnSave.Anchor =
                AnchorStyles.Left;

            btnClose.Anchor =
                AnchorStyles.Left;

            btnSave.Location =
                new Point(50, 17);

            btnClose.Location =
                new Point(
                    btnSave.Right + 10,
                    17);

            btnSave.RightToLeft =
                RightToLeft.Yes;

            btnClose.RightToLeft =
                RightToLeft.Yes;


            // =========================================================
            // FORCE LABEL ALIGNMENT
            // =========================================================

            SetLabelsDirection(
                TabPage1,
                true);

            SetLabelsDirection(
                TabPage2,
                true);


            // Reposition card titles again because SetLabelsDirection()
            // affects all labels.
            PositionCardTitleRight(
                cardPassport,
                cardPassportTitle);

            PositionCardTitleRight(
                cardResidency,
                cardResidencyTitle);

            PositionCardTitleRight(
                cardContract,
                cardContractTitle);

            PositionCardTitleRight(
                cardExpense,
                cardExpenseTitle);
        }


        private void ApplyEmployeeEnglishLayout()
        {
            // =========================================================
            // FORM
            // =========================================================

            Text = id == 0
                ? "Employee - New Employee"
                : "Employee - Edit Employee";

            headerUC1.FormText = id == 0
                ? "Employee - New Employee"
                : "Employee - Edit Employee";

            headerUC1.RightToLeft =
                RightToLeft.No;


            // =========================================================
            // MODERN HEADER
            // =========================================================

            lblModernTitle.Text = id == 0
                ? "Employee - New Employee"
                : "Employee - Edit Employee";

            lblModernSubtitle.Text = id == 0
                ? "Add a new employee to your organization"
                : "View and update employee information";

            lblModernBreadcrumb.Text = id == 0
                ? "HR   ›   Employees   ›   New Employee"
                : "HR   ›   Employees   ›   Edit Employee";

            btnBackEmployees.Text =
                "←  Back to Employees";


            // Exact original English positions

            lblModernTitle.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left;

            lblModernSubtitle.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left;

            lblModernTitle.RightToLeft =
                RightToLeft.No;

            lblModernSubtitle.RightToLeft =
                RightToLeft.No;

            lblModernTitle.TextAlign =
                ContentAlignment.MiddleLeft;

            lblModernSubtitle.TextAlign =
                ContentAlignment.MiddleLeft;

            lblModernTitle.Location =
                new Point(116, 22);

            lblModernSubtitle.Location =
                new Point(118, 64);


            // Breadcrumb original RIGHT

            lblModernBreadcrumb.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            lblModernBreadcrumb.RightToLeft =
                RightToLeft.No;

            lblModernBreadcrumb.TextAlign =
                ContentAlignment.MiddleLeft;

            lblModernBreadcrumb.Location =
                new Point(
                    modernHeader.ClientSize.Width
                        - 540,
                    33);


            // Back button original RIGHT

            btnBackEmployees.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            btnBackEmployees.Location =
                new Point(
                    modernHeader.ClientSize.Width
                        - 245,
                    22);

            btnBackEmployees.RightToLeft =
                RightToLeft.No;


            // =========================================================
            // TABS
            // =========================================================

            tbC.RightToLeft =
                RightToLeft.No;

            TabPage1.RightToLeft =
                RightToLeft.No;

            TabPage2.RightToLeft =
                RightToLeft.No;

            TabPage1.Text =
                "Employee Information";

            TabPage2.Text =
                "Legal Document";


            // =========================================================
            // LEGAL DOCUMENT CARDS
            // Restore original Designer positions
            // =========================================================

            TabPage2.AutoScrollMinSize =
                new Size(1560, 724);

            cardPassport.Location =
                new Point(20, 20);

            cardResidency.Location =
                new Point(772, 20);

            cardContract.Location =
                new Point(20, 250);

            cardExpense.Location =
                new Point(772, 250);


            ApplyCardEnglish(cardPassport);
            ApplyCardEnglish(cardResidency);
            ApplyCardEnglish(cardContract);
            ApplyCardEnglish(cardExpense);


            // =========================================================
            // TITLES
            // =========================================================

            cardPassportTitle.Text =
                "▣   Passport";

            cardResidencyTitle.Text =
                "⌂   Residency";

            cardContractTitle.Text =
                "▣   Work Contract";

            cardExpenseTitle.Text =
                "•   Expense Account (Chart - Account Name)";

            cardPassportTitle.Location =
                new Point(24, 19);

            cardResidencyTitle.Location =
                new Point(24, 19);

            cardContractTitle.Location =
                new Point(24, 19);

            cardExpenseTitle.Location =
                new Point(24, 19);


            // =========================================================
            // FOOTER
            // Restore original Designer values
            // =========================================================

            btnClose.Text = "Close";
            btnSave.Text = "Save";

            btnClose.Anchor =
                AnchorStyles.Right;

            btnSave.Anchor =
                AnchorStyles.Right;

            btnClose.Location =
                new Point(
                    panel1.ClientSize.Width - 330,
                    17);

            btnSave.Location =
                new Point(
                    panel1.ClientSize.Width - 195,
                    17);

            btnClose.RightToLeft =
                RightToLeft.No;

            btnSave.RightToLeft =
                RightToLeft.No;


            SetLabelsDirection(
                TabPage1,
                false);

            SetLabelsDirection(
                TabPage2,
                false);


            // Restore titles after general label processing.
            cardPassportTitle.Location =
                new Point(24, 19);

            cardResidencyTitle.Location =
                new Point(24, 19);

            cardContractTitle.Location =
                new Point(24, 19);

            cardExpenseTitle.Location =
                new Point(24, 19);
        }


        private void ApplyCardArabic(Control card)
        {
            if (card == null)
                return;

            card.RightToLeft =
                RightToLeft.Yes;

            foreach (Control control in card.Controls)
            {
                if (control is Label label)
                {
                    label.RightToLeft =
                        RightToLeft.Yes;

                    label.TextAlign =
                        ContentAlignment.MiddleRight;
                }
                else if (control is TextBox textBox)
                {
                    textBox.RightToLeft =
                        RightToLeft.Yes;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.RightToLeft =
                        RightToLeft.Yes;
                }
                else if (control is DateTimePicker datePicker)
                {
                    datePicker.RightToLeft =
                        RightToLeft.Yes;

                    datePicker.RightToLeftLayout =
                        true;
                }
                else if (
                    control is Guna.UI2.WinForms.Guna2ComboBox gunaCombo)
                {
                    gunaCombo.RightToLeft =
                        RightToLeft.Yes;
                }
            }
        }


        private void ApplyCardEnglish(Control card)
        {
            if (card == null)
                return;

            card.RightToLeft =
                RightToLeft.No;

            foreach (Control control in card.Controls)
            {
                if (control is Label label)
                {
                    label.RightToLeft =
                        RightToLeft.No;

                    label.TextAlign =
                        ContentAlignment.MiddleLeft;
                }
                else if (control is TextBox textBox)
                {
                    textBox.RightToLeft =
                        RightToLeft.No;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.RightToLeft =
                        RightToLeft.No;
                }
                else if (control is DateTimePicker datePicker)
                {
                    datePicker.RightToLeft =
                        RightToLeft.No;

                    datePicker.RightToLeftLayout =
                        false;
                }
                else if (
                    control is Guna.UI2.WinForms.Guna2ComboBox gunaCombo)
                {
                    gunaCombo.RightToLeft =
                        RightToLeft.No;
                }
            }
        }


        private void PositionCardTitleRight(
            Control card,
            Label title)
        {
            if (card == null || title == null)
                return;

            title.RightToLeft =
                RightToLeft.Yes;

            title.TextAlign =
                ContentAlignment.MiddleRight;

            title.Location =
                new Point(
                    card.ClientSize.Width
                        - title.PreferredWidth
                        - 24,
                    19);
        }


        private void SetLabelsDirection(
            Control parent,
            bool arabic)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (control is Label label)
                {
                    label.RightToLeft =
                        arabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    label.TextAlign =
                        arabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }

                if (control.HasChildren)
                {
                    SetLabelsDirection(
                        control,
                        arabic);
                }
            }
        }
        private void FormatNumberWithCommas(TextBox txtBox)
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
        private int GetDefaultAccountId(string category)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT account_id FROM tbl_coa_config WHERE category = @category",
                ClinicDatabase.CreateParameterNew("category", category)))
                if (reader.Read())
                    return Convert.ToInt32(reader["account_id"]);
                else
                    return 0;
        }

    }
}
