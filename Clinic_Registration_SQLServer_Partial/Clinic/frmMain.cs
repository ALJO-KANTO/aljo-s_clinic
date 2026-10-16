
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
namespace Clinic
{
    public partial class frmMain : Form
    {

        private frmLogin _frmLogin;

        public frmMain(frmLogin frmLogin)
        {
            InitializeComponent();

            _frmLogin = frmLogin;
            string language = Properties.Settings.Default.ApplicationLanguage ?? "en";
            isArabic = language.Equals("ar", StringComparison.OrdinalIgnoreCase);
            // Force the List Prescription click event to be connected
            listPrescriptionToolStripMenuItem.Click -=
                listPrescriptionToolStripMenuItem_Click;

            listPrescriptionToolStripMenuItem.Click +=
                listPrescriptionToolStripMenuItem_Click;

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                BuildModernDashboardDesign();
                AddTranslateButton();
                ChangeDashboardLanguage(false);
            }
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

    }
}
