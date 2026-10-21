
using Clinic.Appointment.AppointmentsType;
using Clinic.Doctor;
using Clinic.Login;
using Clinic.Medical_Services.Appointment;
using Clinic.Medical_Services.Casher;
using Clinic.Medical_Services.Laboratory;
using Clinic.Medical_Services.Pharmaciy;
using Clinic.Medical_Services.Visit;
using Clinic.Medical_Services.Vital;
using Clinic.Patient;
using Clinic.Serveces.ServecesType;
using Clinic.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Clinic.Medical_Services.Radiology;
using YamyProject;
namespace Clinic
{
    public partial class frmMain : Form
    {

        private frmLogin _frmLogin;

        public frmMain(frmLogin frmLogin)
        {
            InitializeComponent();

            _frmLogin = frmLogin;

            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            isArabic = language.Equals(
                "ar",
                StringComparison.OrdinalIgnoreCase);

            listPrescriptionToolStripMenuItem.Click -=
                listPrescriptionToolStripMenuItem_Click;

            listPrescriptionToolStripMenuItem.Click +=
                listPrescriptionToolStripMenuItem_Click;

            AddXRayMenu();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                BuildModernDashboardDesign();
                BindMigratedAdminSubmenus();
                AddXRayDashboardButton();
                AddTranslateButton();
                ChangeDashboardLanguage(false);
            }
        }
        private void AddXRayDashboardButton()
        {
            if (modernSidebar == null)
                return;

            FlowLayoutPanel menu = null;

            foreach (Control container in modernSidebar.Controls)
            {
                FlowLayoutPanel flow = container as FlowLayoutPanel;

                if (flow != null)
                {
                    menu = flow;
                    break;
                }
            }

            if (menu == null)
            {
                MessageBox.Show(
                    "Modern sidebar menu container was not found.");
                return;
            }

            if (menu.Controls.ContainsKey("btnXRay"))
                return;

            Button btnXRay = new Button
            {
                Name = "btnXRay",
                Text = "   🩻    X-Ray / Radiology",
                Tag = "X-Ray / Radiology",
                Height = 45,
                Width = Math.Max(180, menu.ClientSize.Width - 10),
                Margin = new Padding(3, 2, 3, 2),
                BackColor = Color.FromArgb(24, 38, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };

            btnXRay.FlatAppearance.BorderSize = 0;

            btnXRay.Click += (s, e) => OpenXRay();

            menu.Controls.Add(btnXRay);
        }
        private ToolStripMenuItem xrayToolStripMenuItem;

        private void AddXRayMenu()
        {
            if (xrayToolStripMenuItem != null)
                return;

            xrayToolStripMenuItem = new ToolStripMenuItem
            {
                Name = "xrayToolStripMenuItem",
                Text = "X-Ray / Radiology",
                Tag = "X-Ray / Radiology"
            };

            xrayToolStripMenuItem.Click += (s, e) => OpenXRay();

            servicesToolStripMenuItem.DropDownItems.Add(
                xrayToolStripMenuItem);
        }
        private void OpenXRay()
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmXRay frm = new frmXRay())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Admin,
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse
            );
        }
        private void OpenLaboratory()
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmLaboratory frm = new frmLaboratory())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Admin,
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse
            );
        }
        private void OpenManageBills()
        {
            if (!CanAccessMenu(clsGlobal.UserRole.Cashier))
            {
                MessageBox.Show(
                    "You do not have permission to access Billing & Payments.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                using (frmManageBills frm = new frmManageBills())
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Unable to Open Billing Payments",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            if (clsGlobal.CurrentUser == null)
                return;

            string roleName = clsGlobal.CurrentUser.RoleInfo.RoleName;

            // Existing designer label
            lblUser.Text = $"User : {roleName}";

            BindDataTable.FetchData();
            BindCombos.ClearCache();
            // Modern dashboard header
            if (modernUser != null)
            {
                modernUser.Text =
                    "●  " + roleName + "\n     " + roleName;
            }

            // Modern dashboard welcome message
            //if (modernGreeting != null)
            //{
            //    modernGreeting.Text =
            //        "Welcome Back,\n" + roleName + " 👋";
            //}
            TranslateDashboardWelcome();
        }
        private Button btnTranslate;
        private bool isArabic = false;

        private void AddTranslateButton()
        {
            btnTranslate = new Button
            {
                Name = "btnTranslate",
                Text = "🌐 Translate",
                Size = new Size(95, 32),
                BackColor = Color.FromArgb(0, 145, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnTranslate.FlatAppearance.BorderSize = 0;

            this.Controls.Add(btnTranslate);
            btnTranslate.BringToFront();

            PositionTranslateButton();

            //this.Resize += (s, e) => PositionTranslateButton();
            modernHeader.Resize += (s, e) => PositionTranslateButton();

            btnTranslate.Click += (s, e) =>
            {
                ChangeDashboardLanguage();

                Properties.Settings.Default.ApplicationLanguage =
                    isArabic ? "ar" : "en";

                Properties.Settings.Default.Save();
            };
        }

        private void PositionTranslateButton()
        {
            if (btnTranslate == null ||
                btnTranslate.IsDisposed)
                return;

            if (modernHeader == null ||
                modernHeader.IsDisposed)
                return;

            // Keep Translate button inside the modern header.
            if (btnTranslate.Parent != modernHeader)
            {
                modernHeader.Controls.Add(btnTranslate);
            }

            // Position below the header brand area,
            // near the right side.
            btnTranslate.Location = new Point(
                Math.Max(0,
                    modernHeader.ClientSize.Width
                    - btnTranslate.Width - 12),
                48
            );

            btnTranslate.BringToFront();
        }


        private void peopleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListPeople frm = new frmListPeople();
            frm.ShowDialog();
        }

        private void UsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmlistUser frm = new frmlistUser())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Admin
            );
        }

        private void PatientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmListPatient frm = new frmListPatient())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse,
                clsGlobal.UserRole.Receptionist
            );
        }

        private void currentUserInfoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmUserInfo frm = new frmUserInfo(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();
        }

        private void Password32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();
        }

        private void btnSearchPat_Click(object sender, EventArgs e)
        {
            OpenFindDoctor();
        }

        private void btnFindDoctor_Click(object sender, EventArgs e)
        {
            OpenFindDoctor();
        }

        private void OpenFindDoctor()
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmFindDoctor frm = new frmFindDoctor())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse,
                clsGlobal.UserRole.Receptionist
            );
        }

        private void btnFindPatient_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmFindPatient frm = new frmFindPatient())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse,
                clsGlobal.UserRole.Receptionist
            );
        }

        private void DoctorsStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmListDoctors frm = new frmListDoctors())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Admin
            );
        }

        private void mangeAppointmentTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListAppointmentType frm = new frmListAppointmentType();
            frm.ShowDialog();
        }

        private void manageServicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmListServicesType frm = new frmListServicesType())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Admin
            );
        }

        private void appointmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmListAppointmets frm = new frmListAppointmets())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse,
                clsGlobal.UserRole.Receptionist,
                clsGlobal.UserRole.Cashier
            );
        }

        private void btnAddApp_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmAddUpdateAppointment frm =
                        new frmAddUpdateAppointment())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse,
                clsGlobal.UserRole.Receptionist,
                clsGlobal.UserRole.Cashier
            );
        }

        private void doctorToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmDoctor frm = new frmDoctor())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse
            );
        }

        private void nurseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmNurse frm = new frmNurse();
            frm.ShowDialog();
        }

        private void listVisitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmListVisits frm = new frmListVisits())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Nurse
            );
        }
        private void directMedicinenDispensingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmDirictSales frm =
                        new frmDirictSales())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Pharmacist
            );
        }

        private void prescriptionDispensingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmPrescriptionDispnsing frm =
                        new frmPrescriptionDispnsing())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Pharmacist
            );
        }

        private void listPrescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    try
                    {
                        using (frmAllPrescriptions frm = new frmAllPrescriptions())
                        {
                            frm.ShowDialog(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.ToString(),
                            "Unable to Open Prescriptions",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                },
                clsGlobal.UserRole.Doctor,
                clsGlobal.UserRole.Pharmacist
            );
        }
        private void inClinicDoctorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmPrescriptionDispnsing frm =
                        new frmPrescriptionDispnsing())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Pharmacist
            );
        }
        private void addNewDrugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenIfAuthorized(
                () =>
                {
                    using (frmManageDrugs frm = new frmManageDrugs())
                    {
                        frm.ShowDialog(this);
                    }
                },
                clsGlobal.UserRole.Pharmacist
            );
        }
        private bool CanShowDashboard(clsGlobal.UserRole role)
        {
            if (clsGlobal.CurrentUser == null)
                return false;

            int currentRoleId = clsGlobal.CurrentUser.RoleID;

            // Admin can see every role dashboard.
            if (currentRoleId == (int)clsGlobal.UserRole.Admin)
                return true;

            // Other users see only their assigned role.
            return currentRoleId == (int)role;
        }

        private bool CanAccessMenu(params clsGlobal.UserRole[] allowedRoles)
        {
            if (clsGlobal.CurrentUser == null)
                return false;

            int currentRoleId = clsGlobal.CurrentUser.RoleID;

            // Admin can access all menus.
            if (currentRoleId == (int)clsGlobal.UserRole.Admin)
                return true;

            foreach (clsGlobal.UserRole role in allowedRoles)
            {
                if (currentRoleId == (int)role)
                    return true;
            }

            return false;
        }

        private void OpenIfAuthorized(Action openForm, params clsGlobal.UserRole[] allowedRoles)
        {
            if (!CanAccessMenu(allowedRoles))
            {
                MessageBox.Show(
                    "You do not have permission to access this module.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            openForm?.Invoke();
        }

        private string TranslateText(string english, string arabic)
        {
            return isArabic ? arabic : english;
        }
        private void ChangeDashboardLanguage(bool toggle = true)
        {
            if (toggle)
                isArabic = !isArabic;

            TranslationClass.IsArabic = isArabic;


            btnTranslate.Text = isArabic
                ? "English"
                : "🌐 Translate";

            TranslateSidebar();
            TranslateDashboardWelcome();
            TranslateRoleDashboard();
            TranslateModernHeader();

            ApplyDashboardLanguageLayout();
        }


        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            timer1.Interval = 1000;
            lblDateTime.Text = DateTime.Now.ToString();

        }

        private void TranslateSidebar()
        {
            if (modernSidebar == null)
                return;

            foreach (Control container in modernSidebar.Controls)
            {
                FlowLayoutPanel menu =
                    container as FlowLayoutPanel;

                if (menu == null)
                    continue;

                foreach (Control control in menu.Controls)
                {
                    // Translate expandable administrator groups and their children.
                    Panel adminGroup = control as Panel;
                    if (adminGroup != null &&
                        adminGroup.Name.StartsWith("adminGroup", StringComparison.Ordinal))
                    {
                        TranslateAdminGroup(adminGroup);
                        continue;
                    }

                    string english = control.Tag as string;

                    if (string.IsNullOrEmpty(english))
                        continue;

                    string translated = english;

                    if (isArabic &&
                        TranslationClass.sidebarTranslations.ContainsKey(english))
                    {
                        translated = TranslationClass.sidebarTranslations[english];
                    }

                    if (control is Button)
                    {
                        Button button = (Button)control;

                        // Preserve the existing navigation symbol.
                        string symbol = "";

                        string original = button.Text.Trim();

                        int spaceIndex = original.IndexOf(' ');

                        if (spaceIndex > 0)
                            symbol = original.Substring(0, spaceIndex);

                        button.Text = "   " + symbol + "    " + translated;

                        button.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;

                        button.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                    }
                    else if (control is Label)
                    {
                        control.Text = translated;

                        Label label = (Label)control;

                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                }
            }
        }

        private void TranslateAdminGroup(Panel group)
        {
            foreach (Control control in group.Controls)
            {
                Button header = control as Button;
                if (header != null)
                {
                    string caption = header.Tag as string ?? "";
                    string translated = TranslateAdminNav(caption);
                    string symbol = caption == "Accounts" ? "▤" :
                        caption == "Vendors" ? "🛒" :
                        caption == "Inventory" ? "⬡" :
                        caption == "Reports" ? "▥" : "♙";
                    header.Text = "   " + symbol + "    " + translated + "  ▾";
                    header.TextAlign = isArabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                    header.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                }
                FlowLayoutPanel submenu = control as FlowLayoutPanel;
                if (submenu != null)
                {
                    foreach (Control item in submenu.Controls)
                    {
                        Label sectionHeading = item as Label;
                        if (sectionHeading != null)
                        {
                            string headingKey = sectionHeading.Tag as string ?? sectionHeading.Text;
                            sectionHeading.Text = TranslateAdminNav(headingKey);
                            sectionHeading.TextAlign = isArabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                            sectionHeading.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                            continue;
                        }
                        Button b = item as Button;
                        if (b == null) continue;
                        string english = b.Tag as string ?? "";
                        b.Text = "│  ├─  " + TranslateAdminNav(english);
                        b.TextAlign = isArabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                        b.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                    }
                }
            }
        }

        private string TranslateAdminNav(string english)
        {
            if (!isArabic) return english;
            switch (english)
            {
                case "Reports": return "التقارير";
                case "Account Ledger": return "دفتر الأستاذ";
                case "Cost Center Summary": return "ملخص مراكز التكلفة";
                case "Trial Balance": return "ميزان المراجعة";
                case "Profit and Loss": return "الأرباح والخسائر";
                case "Purchase": return "المشتريات";
                case "Purchase by Vendor Summary": return "ملخص المشتريات حسب المورد";
                case "Accounts": return "الحسابات";
                case "Account Center": return "مركز الحسابات";
                case "Fixed Assets": return "الأصول الثابتة";
                case "Voucher": return "السندات";
                case "Prepaid": return "المدفوعات المقدمة";
                case "Petty Cash List": return "قائمة المصروفات النثرية";
                case "Vendors": return "الموردون";
                case "Vendor Center": return "مركز الموردين";
                case "Purchases Center": return "مركز المشتريات";
                case "Locations": return "المواقع";
                case "Receipts": return "الإيصالات";
                case "Purchase Invoice": return "فاتورة الشراء";
                case "Payment Voucher": return "سند صرف";
                case "Inventory": return "المخزون";
                case "Inventory Items": return "أصناف المخزون";
                case "Add New Item": return "إضافة صنف جديد";
                case "Item Category": return "فئات الأصناف";
                case "Item Unit": return "وحدات الأصناف";
                case "Inventory Center": return "مركز المخزون";
                case "Stock": return "المخزون";
                case "HR": return "الموارد البشرية";
                case "Human Resource": return "إدارة الموارد البشرية";
                case "Attendance": return "الحضور";
                case "Salary Sheet": return "كشف الرواتب";
                case "Leave Salary": return "راتب الإجازة";
                case "End Of Services": return "نهاية الخدمة";
                case "Loans": return "القروض";
                case "Final Settlement": return "التسوية النهائية";
                default: return TranslationClass.Translate(english, true);
            }
        }

        private void TranslateDashboardWelcome()
        {
            if (modernGreeting == null ||
                clsGlobal.CurrentUser == null)
                return;

            string roleName =
                clsGlobal.CurrentUser.RoleInfo.RoleName;

            string translatedRole =
                TranslationClass.Translate(roleName, isArabic);

            string welcome =
                TranslationClass.Translate("Welcome Back", isArabic);

            modernGreeting.Text =
                welcome + ",\n" + translatedRole + " 👋";

            modernGreeting.RightToLeft =
                isArabic ? RightToLeft.Yes : RightToLeft.No;

            modernGreeting.TextAlign =
                isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft;
        }

        private void TranslateRoleDashboard()
        {
            if (modernDashboard == null)
                return;

            TranslateRoleControls(modernDashboard);
        }

        private void TranslateRoleControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                string english = control.Tag as string;

                if (!string.IsNullOrWhiteSpace(english) &&
                    (control is Label || control is Button))
                {
                    string lookupText = english
                        .Replace("\r", "")
                        .Replace("\n", " ")
                        .Trim();

                    string translated =
                        TranslationClass.Translate(lookupText, isArabic);

                    control.Text = isArabic
                        ? translated
                        : english;

                    if (control is Label)
                    {
                        Label label = (Label)control;

                        label.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                    else if (control is Button)
                    {
                        Button button = (Button)control;

                        button.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                    }
                }

                // Translate all nested panels and layouts.
                if (control.HasChildren)
                {
                    TranslateRoleControls(control);
                }
            }
        }

        private void TranslateModernHeader()
        {
            if (modernHeader == null || modernHeader.IsDisposed)
                return;

            foreach (Control control in modernHeader.Controls)
            {
                string english = control.Tag as string;

                if (string.IsNullOrWhiteSpace(english))
                    continue;

                string translated =
                    TranslationClass.Translate(english, isArabic);

                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;

                    // Only translate the placeholder when the box is empty.
                    if (string.IsNullOrEmpty(textBox.Text))
                        textBox.Text = translated;

                    textBox.RightToLeft = isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;
                }
                else if (control is Label || control is Button)
                {
                    control.Text = translated;

                    if (control is Label label)
                    {
                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                }
            }

            // Translate the logged-in role without changing its identity.
            if (modernUser != null &&
                clsGlobal.CurrentUser != null)
            {
                string roleName =
                    clsGlobal.CurrentUser.RoleInfo.RoleName;

                string translatedRole =
                    TranslationClass.Translate(roleName, isArabic);

                modernUser.Text =
                    "●  " + translatedRole +
                    "\n     " + translatedRole;
            }
        }

        private void ApplyDashboardLanguageLayout()
        {
            if (modernDashboard == null ||
                modernDashboard.IsDisposed)
                return;

            ApplyDashboardTextAlignment(modernDashboard);
        }

        private void ApplyDashboardTextAlignment(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // Only adjust controls explicitly marked for translation.
                string english = control.Tag as string;

                if (!string.IsNullOrWhiteSpace(english))
                {
                    if (control is Label label)
                    {
                        label.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                        label.TextAlign = isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                    else if (control is Button button)
                    {
                        button.RightToLeft = isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                    }
                }

                if (control.HasChildren)
                    ApplyDashboardTextAlignment(control);
            }
        }

        public static void openChildForm(Form frm)
        {
            // new dialog form
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        public static void LoadFormIntoPanel(Form frm)
        {
            openChildForm(frm);
        }

        // Added navigation only. Existing sidebar design and role rules remain unchanged.
        private void BindMigratedAdminSubmenus()
        {
            if (modernSidebar == null ||
                clsGlobal.CurrentUser == null ||
                clsGlobal.CurrentUser.RoleID != (int)clsGlobal.UserRole.Admin)
                return;

            var forms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Account Ledger", "frmAccountLedger" },
                { "Cost Center Summary", "frmCostCenterSummary" },
                { "Trial Balance", "MasterTrialBalance" },
                { "Profit and Loss", "ProfitandLossReport" },
                { "Purchase by Vendor Summary", "frmPurchaseByVendorSummery" },
                { "Account Center", "MasterChartOfAccount" },
                { "Fixed Assets", "MasterFixedAssets" },
                { "Voucher", "MasterJournalVoucher" },
                { "Prepaid", "MasterAdvancePaymentVoucher" },
                { "Petty Cash List", "MasterPettyCash" },
                { "Vendor Center", "MasterVendor" },
                { "Purchases Center", "MasterPurchases" },
                { "Payment Voucher", "MasterPaymentVoucher" },
                { "Inventory Items", "MasterInventory" },
                { "Add New Item", "frmViewItem" },
                { "Item Category", "MasterItemCategory" },
                { "Item Unit", "MasterItemUnit" },
                { "Human Resource", "MasterEmployee" },
                { "Attendance", "MasterAttendanceSheet" },
                { "Salary Sheet", "frmSalarySheet" },
                { "Leave Salary", "frmLeaveSalary" },
                { "End Of Services", "frmEndOfService" },
                { "Loans", "MasterLoan" },
                { "Final Settlement", "MasterFinalSettlement" }
            };

            foreach (Control container in modernSidebar.Controls)
            {
                FlowLayoutPanel menu = container as FlowLayoutPanel;
                if (menu == null) continue;

                foreach (Control control in menu.Controls)
                {
                    Panel group = control as Panel;
                    if (group == null || !group.Name.StartsWith("adminGroup", StringComparison.Ordinal))
                        continue;

                    foreach (Control child in group.Controls)
                    {
                        FlowLayoutPanel submenu = child as FlowLayoutPanel;
                        if (submenu == null) continue;

                        foreach (Control item in submenu.Controls)
                        {
                            Button button = item as Button;
                            if (button == null) continue;
                            string caption = button.Tag as string;
                            string formName;
                            if (caption == null || !forms.TryGetValue(caption, out formName) ||
                                string.IsNullOrWhiteSpace(formName)) continue;

                            string target = formName;
                            button.Click += (sender, args) => OpenMigratedAdminForm(target);
                        }
                    }
                }
            }
        }

        private void OpenMigratedAdminForm(string className)
        {
            OpenIfAuthorized(() =>
            {
                try
                {
                    var matches = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly =>
                        {
                            try
                            {
                                return assembly.GetTypes();
                            }
                            catch (System.Reflection.ReflectionTypeLoadException ex)
                            {
                                return ex.Types.Where(t => t != null);
                            }
                        })
                        .Where(t =>
                            t != null &&
                            t.Name.Equals(className, StringComparison.Ordinal) &&
                            typeof(Form).IsAssignableFrom(t) &&
                            !t.IsAbstract)
                        .ToList();

                    if (matches.Count != 1)
                    {
                        MessageBox.Show(
                            matches.Count == 0
                                ? "Form not found: " + className
                                : "Multiple forms named " + className +
                                  ". Specify its namespace.",
                            "Navigation",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    Type formType = matches[0];

                    // First try a public parameterless constructor.
                    var constructor = formType.GetConstructor(Type.EmptyTypes);

                    Form form = null;

                    if (constructor != null)
                    {
                        form = (Form)constructor.Invoke(null);
                    }
                    else
                    {
                        // Try a public constructor where every parameter is optional.
                        var optionalConstructor = formType
                            .GetConstructors()
                            .FirstOrDefault(c =>
                                c.GetParameters().All(p => p.IsOptional));

                        if (optionalConstructor != null)
                        {
                            object[] parameters = optionalConstructor
                                .GetParameters()
                                .Select(p => p.DefaultValue)
                                .ToArray();

                            form = (Form)optionalConstructor.Invoke(parameters);
                        }
                    }

                    if (form == null)
                    {
                        var constructors = formType.GetConstructors()
                            .Select(c =>
                                "(" + string.Join(", ",
                                    c.GetParameters().Select(p =>
                                        p.ParameterType.Name + " " + p.Name)) + ")");

                        MessageBox.Show(
                            "Cannot open: " + className +
                            Environment.NewLine +
                            Environment.NewLine +
                            "This form requires constructor parameters." +
                            Environment.NewLine +
                            Environment.NewLine +
                            "Available constructors:" +
                            Environment.NewLine +
                            string.Join(Environment.NewLine, constructors),
                            "Form Constructor Required",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    using (form)
                    {
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.ToString(),
                        "Unable to Open Form",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

            }, clsGlobal.UserRole.Admin);
        }
    }
}
