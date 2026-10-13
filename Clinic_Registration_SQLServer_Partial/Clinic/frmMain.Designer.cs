using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.msMainMenue = new System.Windows.Forms.MenuStrip();
            this.servicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appointmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doctorToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.nurseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.listVisitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prescriptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prescriptionDispensingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listPrescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FinancialstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paymentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageBillsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.peopleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PatientsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DoctorsStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.UsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MangementStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageServicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mangeAppointmentTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UserOptiontoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentUserInfoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.Password32ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.signOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.spring = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDateTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsQuick = new System.Windows.Forms.ToolStrip();
            this.btnAddApp = new System.Windows.Forms.ToolStripButton();
            this.btnFindPatient = new System.Windows.Forms.ToolStripButton();
            this.btnFindDoctor = new System.Windows.Forms.ToolStripButton();
            this.sep = new System.Windows.Forms.ToolStripSeparator();
            this.btnPayment = new System.Windows.Forms.ToolStripButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlCard1 = new System.Windows.Forms.Panel();
            this.lbValue = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblValue2 = new System.Windows.Forms.Label();
            this.lblTitle2 = new System.Windows.Forms.Label();
            this.pnlCard3 = new System.Windows.Forms.Panel();
            this.lblValue3 = new System.Windows.Forms.Label();
            this.lblTitle3 = new System.Windows.Forms.Label();
            this.pnlCard2 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.msMainMenue.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tsQuick.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlCard1.SuspendLayout();
            this.pnlCard3.SuspendLayout();
            this.pnlCard2.SuspendLayout();
            this.SuspendLayout();
            // 
            // msMainMenue
            // 
            this.msMainMenue.BackColor = System.Drawing.Color.White;
            this.msMainMenue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msMainMenue.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.msMainMenue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.servicesToolStripMenuItem,
            this.FinancialstoolStripMenuItem,
            this.peopleToolStripMenuItem,
            this.PatientsToolStripMenuItem,
            this.DoctorsStripMenuItem1,
            this.UsersToolStripMenuItem,
            this.accountSettingsToolStripMenuItem});
            this.msMainMenue.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.msMainMenue.Location = new System.Drawing.Point(0, 0);
            this.msMainMenue.Name = "msMainMenue";
            this.msMainMenue.Size = new System.Drawing.Size(1924, 72);
            this.msMainMenue.TabIndex = 0;
            this.msMainMenue.Text = "menuStrip1";
            // 
            // servicesToolStripMenuItem
            // 
            this.servicesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appointmentsToolStripMenuItem,
            this.visitsToolStripMenuItem,
            this.prescriptionsToolStripMenuItem});
            this.servicesToolStripMenuItem.Image = global::Clinic.Properties.Resources.MedicalServices;
            this.servicesToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.servicesToolStripMenuItem.Name = "servicesToolStripMenuItem";
            this.servicesToolStripMenuItem.Size = new System.Drawing.Size(255, 68);
            this.servicesToolStripMenuItem.Text = "&Medical Services";
            // 
            // appointmentsToolStripMenuItem
            // 
            this.appointmentsToolStripMenuItem.Image = global::Clinic.Properties.Resources.appointment_64;
            this.appointmentsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.appointmentsToolStripMenuItem.Name = "appointmentsToolStripMenuItem";
            this.appointmentsToolStripMenuItem.Size = new System.Drawing.Size(358, 70);
            this.appointmentsToolStripMenuItem.Text = "Manage Appointments";
            this.appointmentsToolStripMenuItem.Click += new System.EventHandler(this.appointmentsToolStripMenuItem_Click);
            // 
            // visitsToolStripMenuItem
            // 
            this.visitsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doctorToolStripMenuItem2,
            this.nurseToolStripMenuItem1,
            this.listVisitToolStripMenuItem});
            this.visitsToolStripMenuItem.Image = global::Clinic.Properties.Resources.Visits_64;
            this.visitsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.visitsToolStripMenuItem.Name = "visitsToolStripMenuItem";
            this.visitsToolStripMenuItem.Size = new System.Drawing.Size(358, 70);
            this.visitsToolStripMenuItem.Text = "Manage Visits";
            // 
            // doctorToolStripMenuItem2
            // 
            this.doctorToolStripMenuItem2.Image = global::Clinic.Properties.Resources.doctor1_32;
            this.doctorToolStripMenuItem2.Name = "doctorToolStripMenuItem2";
            this.doctorToolStripMenuItem2.Size = new System.Drawing.Size(180, 30);
            this.doctorToolStripMenuItem2.Text = "Doctor";
            this.doctorToolStripMenuItem2.Click += new System.EventHandler(this.doctorToolStripMenuItem2_Click);
            // 
            // nurseToolStripMenuItem1
            // 
            this.nurseToolStripMenuItem1.Name = "nurseToolStripMenuItem1";
            this.nurseToolStripMenuItem1.Size = new System.Drawing.Size(180, 30);
            this.nurseToolStripMenuItem1.Text = "Nurse";
            this.nurseToolStripMenuItem1.Click += new System.EventHandler(this.nurseToolStripMenuItem1_Click);
            // 
            // listVisitToolStripMenuItem
            // 
            this.listVisitToolStripMenuItem.Name = "listVisitToolStripMenuItem";
            this.listVisitToolStripMenuItem.Size = new System.Drawing.Size(180, 30);
            this.listVisitToolStripMenuItem.Text = "List Visit";
            this.listVisitToolStripMenuItem.Click += new System.EventHandler(this.listVisitToolStripMenuItem_Click);
            // 
            // prescriptionsToolStripMenuItem
            // 
            this.prescriptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prescriptionDispensingToolStripMenuItem,
            this.listPrescriptionToolStripMenuItem});
            this.prescriptionsToolStripMenuItem.Image = global::Clinic.Properties.Resources.Prescription_64;
            this.prescriptionsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.prescriptionsToolStripMenuItem.Name = "prescriptionsToolStripMenuItem";
            this.prescriptionsToolStripMenuItem.Size = new System.Drawing.Size(358, 70);
            this.prescriptionsToolStripMenuItem.Text = "Manage Prescriptions";
            // 
            // prescriptionDispensingToolStripMenuItem
            // 
            this.prescriptionDispensingToolStripMenuItem.Name = "prescriptionDispensingToolStripMenuItem";
            this.prescriptionDispensingToolStripMenuItem.Size = new System.Drawing.Size(325, 30);
            this.prescriptionDispensingToolStripMenuItem.Text = "Prescription Dispensing";
            this.prescriptionDispensingToolStripMenuItem.Click += new System.EventHandler(this.prescriptionDispensingToolStripMenuItem_Click);
            // 
            // listPrescriptionToolStripMenuItem
            // 
            this.listPrescriptionToolStripMenuItem.Name = "listPrescriptionToolStripMenuItem";
            this.listPrescriptionToolStripMenuItem.Size = new System.Drawing.Size(325, 30);
            this.listPrescriptionToolStripMenuItem.Text = "List Prescription";

            this.listPrescriptionToolStripMenuItem.Click +=
                new System.EventHandler(this.listPrescriptionToolStripMenuItem_Click);



            // 
            // FinancialstoolStripMenuItem
            // 
            this.FinancialstoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.paymentsToolStripMenuItem,
            this.manageBillsToolStripMenuItem});
            this.FinancialstoolStripMenuItem.Image = global::Clinic.Properties.Resources.Financials;
            this.FinancialstoolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.FinancialstoolStripMenuItem.Name = "FinancialstoolStripMenuItem";
            this.FinancialstoolStripMenuItem.Size = new System.Drawing.Size(188, 68);
            this.FinancialstoolStripMenuItem.Text = "Financials";
            // 
            // paymentsToolStripMenuItem
            // 
            this.paymentsToolStripMenuItem.Image = global::Clinic.Properties.Resources.Payment_64;
            this.paymentsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.paymentsToolStripMenuItem.Name = "paymentsToolStripMenuItem";
            this.paymentsToolStripMenuItem.Size = new System.Drawing.Size(266, 70);
            this.paymentsToolStripMenuItem.Text = "Payments";
            // 
            // manageBillsToolStripMenuItem
            // 
            this.manageBillsToolStripMenuItem.Image = global::Clinic.Properties.Resources.Bill_64;
            this.manageBillsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.manageBillsToolStripMenuItem.Name = "manageBillsToolStripMenuItem";
            this.manageBillsToolStripMenuItem.Size = new System.Drawing.Size(266, 70);
            this.manageBillsToolStripMenuItem.Text = "Manage Bills";
            // 
            // peopleToolStripMenuItem
            // 
            this.peopleToolStripMenuItem.Image = global::Clinic.Properties.Resources.People_64;
            this.peopleToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.peopleToolStripMenuItem.Name = "peopleToolStripMenuItem";
            this.peopleToolStripMenuItem.Size = new System.Drawing.Size(157, 68);
            this.peopleToolStripMenuItem.Text = "People";
            this.peopleToolStripMenuItem.Click += new System.EventHandler(this.peopleToolStripMenuItem_Click);
            // 
            // PatientsToolStripMenuItem
            // 
            this.PatientsToolStripMenuItem.Image = global::Clinic.Properties.Resources.patient;
            this.PatientsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.PatientsToolStripMenuItem.Name = "PatientsToolStripMenuItem";
            this.PatientsToolStripMenuItem.Size = new System.Drawing.Size(168, 68);
            this.PatientsToolStripMenuItem.Text = "Patients";
            this.PatientsToolStripMenuItem.Click += new System.EventHandler(this.PatientsToolStripMenuItem_Click);
            // 
            // DoctorsStripMenuItem1
            // 
            this.DoctorsStripMenuItem1.Image = global::Clinic.Properties.Resources.doctor;
            this.DoctorsStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.DoctorsStripMenuItem1.Name = "DoctorsStripMenuItem1";
            this.DoctorsStripMenuItem1.Size = new System.Drawing.Size(164, 68);
            this.DoctorsStripMenuItem1.Text = "Doctors";
            this.DoctorsStripMenuItem1.Click += new System.EventHandler(this.DoctorsStripMenuItem1_Click);
            // 
            // UsersToolStripMenuItem
            // 
            this.UsersToolStripMenuItem.Image = global::Clinic.Properties.Resources.User_Options_64;
            this.UsersToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.UsersToolStripMenuItem.Name = "UsersToolStripMenuItem";
            this.UsersToolStripMenuItem.Size = new System.Drawing.Size(146, 68);
            this.UsersToolStripMenuItem.Text = "Users";
            this.UsersToolStripMenuItem.Click += new System.EventHandler(this.UsersToolStripMenuItem_Click);
            // 
            // accountSettingsToolStripMenuItem
            // 
            this.accountSettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MangementStripMenuItem,
            this.UserOptiontoolStripMenuItem,
            this.toolStripSeparator4,
            this.signOutToolStripMenuItem});
            this.accountSettingsToolStripMenuItem.Image = global::Clinic.Properties.Resources.Account_Setttings_64;
            this.accountSettingsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.accountSettingsToolStripMenuItem.Name = "accountSettingsToolStripMenuItem";
            this.accountSettingsToolStripMenuItem.Size = new System.Drawing.Size(254, 68);
            this.accountSettingsToolStripMenuItem.Text = "Account Settings";
            // 
            // MangementStripMenuItem
            // 
            this.MangementStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageServicesToolStripMenuItem,
            this.mangeAppointmentTypeToolStripMenuItem});
            this.MangementStripMenuItem.Image = global::Clinic.Properties.Resources.mange_32;
            this.MangementStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MangementStripMenuItem.Name = "MangementStripMenuItem";
            this.MangementStripMenuItem.Size = new System.Drawing.Size(236, 38);
            this.MangementStripMenuItem.Text = "gi";
            // 
            // manageServicesToolStripMenuItem
            // 
            this.manageServicesToolStripMenuItem.Image = global::Clinic.Properties.Resources.Mange_Services32;
            this.manageServicesToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.manageServicesToolStripMenuItem.Name = "manageServicesToolStripMenuItem";
            this.manageServicesToolStripMenuItem.Size = new System.Drawing.Size(358, 38);
            this.manageServicesToolStripMenuItem.Text = "Manage Services";
            this.manageServicesToolStripMenuItem.Click += new System.EventHandler(this.manageServicesToolStripMenuItem_Click);
            // 
            // mangeAppointmentTypeToolStripMenuItem
            // 
            this.mangeAppointmentTypeToolStripMenuItem.Image = global::Clinic.Properties.Resources.appointment_type_32;
            this.mangeAppointmentTypeToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mangeAppointmentTypeToolStripMenuItem.Name = "mangeAppointmentTypeToolStripMenuItem";
            this.mangeAppointmentTypeToolStripMenuItem.Size = new System.Drawing.Size(358, 38);
            this.mangeAppointmentTypeToolStripMenuItem.Text = "Mange Appointment Type";
            this.mangeAppointmentTypeToolStripMenuItem.Click += new System.EventHandler(this.mangeAppointmentTypeToolStripMenuItem_Click);
            // 
            // UserOptiontoolStripMenuItem
            // 
            this.UserOptiontoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentUserInfoToolStripMenuItem1,
            this.Password32ToolStripMenuItem});
            this.UserOptiontoolStripMenuItem.Image = global::Clinic.Properties.Resources.User_Options_32;
            this.UserOptiontoolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.UserOptiontoolStripMenuItem.Name = "UserOptiontoolStripMenuItem";
            this.UserOptiontoolStripMenuItem.Size = new System.Drawing.Size(236, 38);
            this.UserOptiontoolStripMenuItem.Text = "UserOption";
            // 
            // currentUserInfoToolStripMenuItem1
            // 
            this.currentUserInfoToolStripMenuItem1.Image = global::Clinic.Properties.Resources.PersonDetails_32;
            this.currentUserInfoToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.currentUserInfoToolStripMenuItem1.Name = "currentUserInfoToolStripMenuItem1";
            this.currentUserInfoToolStripMenuItem1.Size = new System.Drawing.Size(286, 38);
            this.currentUserInfoToolStripMenuItem1.Text = "&Current User Info";
            this.currentUserInfoToolStripMenuItem1.Click += new System.EventHandler(this.currentUserInfoToolStripMenuItem1_Click);
            // 
            // Password32ToolStripMenuItem
            // 
            this.Password32ToolStripMenuItem.Image = global::Clinic.Properties.Resources.Password_32;
            this.Password32ToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.Password32ToolStripMenuItem.Name = "Password32ToolStripMenuItem";
            this.Password32ToolStripMenuItem.Size = new System.Drawing.Size(286, 38);
            this.Password32ToolStripMenuItem.Text = "Change Password";
            this.Password32ToolStripMenuItem.Click += new System.EventHandler(this.Password32ToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(233, 6);
            // 
            // signOutToolStripMenuItem
            // 
            this.signOutToolStripMenuItem.Image = global::Clinic.Properties.Resources.sign_out_32__2;
            this.signOutToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.signOutToolStripMenuItem.Name = "signOutToolStripMenuItem";
            this.signOutToolStripMenuItem.Size = new System.Drawing.Size(236, 38);
            this.signOutToolStripMenuItem.Text = "Sign &Out";
            this.signOutToolStripMenuItem.Click += new System.EventHandler(this.signOutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUser,
            this.spring,
            this.lblDateTime,
            this.lblDbStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1005);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1924, 50);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblUser
            // 
            this.lblUser.Image = global::Clinic.Properties.Resources.User_32__2;
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(117, 44);
            this.lblUser.Text = "  User: Admin";
            // 
            // spring
            // 
            this.spring.AutoSize = false;
            this.spring.Name = "spring";
            this.spring.Size = new System.Drawing.Size(1540, 44);
            this.spring.Spring = true;
            // 
            // lblDateTime
            // 
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(148, 44);
            this.lblDateTime.Text = "14/05/2026 00:32 ص";
            // 
            // lblDbStatus
            // 
            this.lblDbStatus.ForeColor = System.Drawing.Color.Green;
            this.lblDbStatus.Name = "lblDbStatus";
            this.lblDbStatus.Size = new System.Drawing.Size(104, 44);
            this.lblDbStatus.Text = "DB Connected";
            // 
            // tsQuick
            // 
            this.tsQuick.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tsQuick.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsQuick.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsQuick.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tsQuick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddApp,
            this.btnFindPatient,
            this.btnFindDoctor,
            this.sep,
            this.btnPayment});
            this.tsQuick.Location = new System.Drawing.Point(0, 72);
            this.tsQuick.Name = "tsQuick";
            this.tsQuick.Size = new System.Drawing.Size(1924, 39);
            this.tsQuick.TabIndex = 1;
            // 
            // btnAddApp
            // 
            this.btnAddApp.Image = global::Clinic.Properties.Resources.Add_Appointment_32;
            this.btnAddApp.Name = "btnAddApp";
            this.btnAddApp.Size = new System.Drawing.Size(160, 36);
            this.btnAddApp.Text = "New Appointment";
            this.btnAddApp.Click += new System.EventHandler(this.btnAddApp_Click);
            // 
            // btnFindPatient
            // 
            this.btnFindPatient.Image = global::Clinic.Properties.Resources.patient;
            this.btnFindPatient.Name = "btnFindPatient";
            this.btnFindPatient.Size = new System.Drawing.Size(121, 36);
            this.btnFindPatient.Text = "Find Patient";
            this.btnFindPatient.Click += new System.EventHandler(this.btnFindPatient_Click);
            // 
            // btnFindDoctor
            // 
            this.btnFindDoctor.Image = global::Clinic.Properties.Resources.doctor;
            this.btnFindDoctor.Name = "btnFindDoctor";
            this.btnFindDoctor.Size = new System.Drawing.Size(122, 36);
            this.btnFindDoctor.Text = "Find Doctor";
            this.btnFindDoctor.Click += new System.EventHandler(this.btnFindDoctor_Click);
            // 
            // sep
            // 
            this.sep.Name = "sep";
            this.sep.Size = new System.Drawing.Size(6, 39);
            // 
            // btnPayment
            // 
            this.btnPayment.Image = global::Clinic.Properties.Resources.Payment_64;
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(136, 36);
            this.btnPayment.Text = "New Payment";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Clinic.Properties.Resources.ClinicWallpaper;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1924, 1055);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // pnlCard1
            // 
            this.pnlCard1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(222)))));
            this.pnlCard1.Controls.Add(this.lbValue);
            this.pnlCard1.Controls.Add(this.lblTitle);
            this.pnlCard1.Location = new System.Drawing.Point(50, 100);
            this.pnlCard1.Name = "pnlCard1";
            this.pnlCard1.Size = new System.Drawing.Size(250, 100);
            this.pnlCard1.TabIndex = 11;
            // 
            // lbValue
            // 
            this.lbValue.AutoSize = true;
            this.lbValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.lbValue.ForeColor = System.Drawing.Color.White;
            this.lbValue.Location = new System.Drawing.Point(23, 40);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(37, 39);
            this.lbValue.TabIndex = 2;
            this.lbValue.Text = "0";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(10, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(227, 25);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Today\'s Appointments";
            // 
            // lblValue2
            // 
            this.lblValue2.AutoSize = true;
            this.lblValue2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.lblValue2.ForeColor = System.Drawing.Color.White;
            this.lblValue2.Location = new System.Drawing.Point(10, 40);
            this.lblValue2.Name = "lblValue2";
            this.lblValue2.Size = new System.Drawing.Size(37, 39);
            this.lblValue2.TabIndex = 0;
            this.lblValue2.Text = "0";
            // 
            // lblTitle2
            // 
            this.lblTitle2.AutoSize = true;
            this.lblTitle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle2.ForeColor = System.Drawing.Color.White;
            this.lblTitle2.Location = new System.Drawing.Point(10, 10);
            this.lblTitle2.Name = "lblTitle2";
            this.lblTitle2.Size = new System.Drawing.Size(146, 25);
            this.lblTitle2.TabIndex = 1;
            this.lblTitle2.Text = "Waiting Room";
            // 
            // pnlCard3
            // 
            this.pnlCard3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.pnlCard3.Controls.Add(this.lblValue3);
            this.pnlCard3.Controls.Add(this.lblTitle3);
            this.pnlCard3.Location = new System.Drawing.Point(590, 100);
            this.pnlCard3.Name = "pnlCard3";
            this.pnlCard3.Size = new System.Drawing.Size(250, 100);
            this.pnlCard3.TabIndex = 12;
            // 
            // lblValue3
            // 
            this.lblValue3.AutoSize = true;
            this.lblValue3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.lblValue3.ForeColor = System.Drawing.Color.White;
            this.lblValue3.Location = new System.Drawing.Point(10, 40);
            this.lblValue3.Name = "lblValue3";
            this.lblValue3.Size = new System.Drawing.Size(107, 39);
            this.lblValue3.TabIndex = 0;
            this.lblValue3.Text = "$0.00";
            // 
            // lblTitle3
            // 
            this.lblTitle3.AutoSize = true;
            this.lblTitle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle3.ForeColor = System.Drawing.Color.White;
            this.lblTitle3.Location = new System.Drawing.Point(10, 10);
            this.lblTitle3.Name = "lblTitle3";
            this.lblTitle3.Size = new System.Drawing.Size(165, 25);
            this.lblTitle3.TabIndex = 1;
            this.lblTitle3.Text = "Today\'s Income";
            // 
            // pnlCard2
            // 
            this.pnlCard2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.pnlCard2.Controls.Add(this.lblValue2);
            this.pnlCard2.Controls.Add(this.lblTitle2);
            this.pnlCard2.Location = new System.Drawing.Point(320, 100);
            this.pnlCard2.Name = "pnlCard2";
            this.pnlCard2.Size = new System.Drawing.Size(250, 100);
            this.pnlCard2.TabIndex = 12;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 1055);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tsQuick);
            this.Controls.Add(this.msMainMenue);
            this.Controls.Add(this.pnlCard1);
            this.Controls.Add(this.pnlCard2);
            this.Controls.Add(this.pnlCard3);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.msMainMenue.ResumeLayout(false);
            this.msMainMenue.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tsQuick.ResumeLayout(false);
            this.tsQuick.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlCard1.ResumeLayout(false);
            this.pnlCard1.PerformLayout();
            this.pnlCard3.ResumeLayout(false);
            this.pnlCard3.PerformLayout();
            this.pnlCard2.ResumeLayout(false);
            this.pnlCard2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }




        private void _CreateStatsCards()
        {
            // مصفوفة بأسماء الكروت وقيم تجريبية
            string[] cardNames = { "Today's Appointments", "Waiting Room", "Today's Income" };
            Color[] cardColors = { Color.FromArgb(150, 0, 122, 204), Color.FromArgb(150, 255, 140, 0), Color.FromArgb(150, 46, 139, 87) };

            int xPos = 50; // مكان البداية من اليسار
            for (int i = 0; i < 3; i++)
            {
                Panel pnlCard = new Panel();
                pnlCard.Size = new Size(250, 100);
                pnlCard.Location = new Point(xPos, 100); // تحت المنيو بمسافة
                pnlCard.BackColor = cardColors[i]; // لون نصف شفاف
                pnlCard.BorderStyle = BorderStyle.None;

                Label lblTitle = new Label();
                lblTitle.Text = cardNames[i];
                lblTitle.ForeColor = Color.White;
                lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblTitle.Location = new Point(10, 10);
                lblTitle.AutoSize = true;

                Label lblValue = new Label();
                lblValue.Text = (i == 2) ? "$0.00" : "0"; // قيم افتراضية
                lblValue.ForeColor = Color.White;
                lblValue.Font = new Font("Segoe UI", 20, FontStyle.Bold);
                lblValue.Location = new Point(10, 40);
                lblValue.AutoSize = true;

                pnlCard.Controls.Add(lblTitle);
                pnlCard.Controls.Add(lblValue);

                // ربط الـ Panel بالـ PictureBox لضمان الشفافية الصحيحة
                pictureBox1.Controls.Add(pnlCard);
                xPos += 270; // إزاحة الكارت القادم
            }
        }
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem accountSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PatientsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem peopleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servicesToolStripMenuItem;
        private System.Windows.Forms.MenuStrip msMainMenue;
        private System.Windows.Forms.ToolStripMenuItem DoctorsStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem appointmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prescriptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FinancialstoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageBillsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paymentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MangementStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UserOptiontoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentUserInfoToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem Password32ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageServicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mangeAppointmentTypeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private ToolStrip tsQuick;
        private ToolStripButton btnAddApp;
        private ToolStripButton btnFindDoctor;
        private ToolStripSeparator sep;
        private ToolStripButton btnPayment;
        private PictureBox pictureBox1;
        private ToolStripStatusLabel lblUser;
        private ToolStripStatusLabel spring;
        private ToolStripStatusLabel lblDateTime;
        private ToolStripStatusLabel lblDbStatus;
        private Panel pnlCard1;
        private Panel pnlCard3;
        private Panel pnlCard2;
        private Label lblValue3;
        private Label lblTitle3;
        private Label lblValue2;
        private Label lblTitle2;
        private Label lbValue;
        private Label lblTitle;
        private Timer timer1;
        private ToolStripButton btnFindPatient;
        private ToolStripMenuItem prescriptionDispensingToolStripMenuItem;
        private ToolStripMenuItem doctorToolStripMenuItem2;
        private ToolStripMenuItem nurseToolStripMenuItem1;
        private ToolStripMenuItem listVisitToolStripMenuItem;
        private ToolStripMenuItem listPrescriptionToolStripMenuItem;
    }
}

