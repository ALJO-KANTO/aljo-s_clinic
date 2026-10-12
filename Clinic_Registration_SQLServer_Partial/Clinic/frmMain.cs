
using Clinic.Appointment.AppointmentsType;
using Clinic.Doctor;
using Clinic.Login;
using Clinic.Medical_Services.Appointment;
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

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                BuildModernDashboardDesign();

                AddTranslateButton();
            }
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            lblUser.Text=$"User : {clsGlobal.CurrentUser.RoleInfo.RoleName}";
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

            this.Resize += (s, e) => PositionTranslateButton();

            btnTranslate.Click += (s, e) =>
            {
                isArabic = !isArabic;

                btnTranslate.Text = isArabic
                    ? "English"
                    : "🌐 Translate";
            };
        }
        private void PositionTranslateButton()
        {
            if (btnTranslate == null)
                return;

            // Position at the far-right of the top header
            btnTranslate.Location = new Point(
                this.ClientSize.Width - btnTranslate.Width - 12,
                43
            );

            btnTranslate.BringToFront();
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

        private void peopleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListPeople frm = new frmListPeople();
            frm.ShowDialog();
        }

        private void UsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmlistUser frm=new frmlistUser();
            frm.ShowDialog();
        }

        private void PatientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListPatient frm=new frmListPatient();
            frm.ShowDialog();
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
            frmFindDoctor frm=new frmFindDoctor();
            frm.ShowDialog();
        }

        private void btnFindDoctor_Click(object sender, EventArgs e)
        {
            frmFindDoctor frm=new frmFindDoctor();
            frm.ShowDialog();
        }

        private void btnFindPatient_Click(object sender, EventArgs e)
        {
            frmFindPatient frm = new frmFindPatient();
            frm.ShowDialog();
        }

        private void DoctorsStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmListDoctors frm = new frmListDoctors();
            frm.ShowDialog();
        }

        private void mangeAppointmentTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListAppointmentType frm = new frmListAppointmentType();
            frm.ShowDialog();
        }

        private void manageServicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListServicesType frm = new frmListServicesType();
            frm.ShowDialog();
        }

        private void appointmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListAppointmets frm = new frmListAppointmets();
            frm.ShowDialog();
        }

        private void btnAddApp_Click(object sender, EventArgs e)
        {
            frmAddUpdateAppointment frm = new frmAddUpdateAppointment();
            frm.ShowDialog();
        }

        private void doctorToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmDoctor frm = new frmDoctor();
            frm.ShowDialog();
        }

        private void nurseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmNurse frm = new frmNurse();
            frm.ShowDialog();
        }

        private void listVisitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListVisits frm = new frmListVisits();
            frm.ShowDialog();
        }

        private void prescriptionDispensingToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }
    }
}
