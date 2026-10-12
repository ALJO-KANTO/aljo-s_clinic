using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Clinic.Login
{
    partial class frmLogin
    {
        private IContainer components = null;
        private SplitContainer splitContainer1;
        private ClinicGradientPanel leftPanel, rightPanel;
        private ClinicRoundedPanel loginCard, passwordField;
        private Label lblWelcome, lblSystem, lblSlogan, lblVersion, lblLoginTitle, lblSubtitle;
        private Label lblAppointmentsTitle, lblAppointmentsDesc;
        private Label lblPatientsTitle, lblPatientsDesc;
        private Label lblReportsTitle, lblReportsDesc;
        private Label lblNewUser, lblForgot, lblEye;
        private TextBox txtUserName, txtPassword;
        private CheckBox chkRememberMe;
        private ClinicGradientButton btnLogin;
        private ClinicOutlineButton btnRegister;
        private Button btnClose;
        private ErrorProvider errorProvider1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.leftPanel = new Clinic.Login.ClinicGradientPanel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblReportsDesc = new System.Windows.Forms.Label();
            this.lblReportsTitle = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblSystem = new System.Windows.Forms.Label();
            this.lblSlogan = new System.Windows.Forms.Label();
            this.lblAppointmentsTitle = new System.Windows.Forms.Label();
            this.lblAppointmentsDesc = new System.Windows.Forms.Label();
            this.lblPatientsTitle = new System.Windows.Forms.Label();
            this.lblPatientsDesc = new System.Windows.Forms.Label();
            this.rightPanel = new Clinic.Login.ClinicGradientPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.loginCard = new Clinic.Login.ClinicRoundedPanel();
            this.btnRegister = new Clinic.Login.ClinicOutlineButton();
            this.lblNewUser = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.lblLoginTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.userField = new Clinic.Login.ClinicRoundedPanel();
            this.picUser = new System.Windows.Forms.PictureBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.passwordField = new Clinic.Login.ClinicRoundedPanel();
            this.picLock = new System.Windows.Forms.PictureBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblEye = new System.Windows.Forms.Label();
            this.chkRememberMe = new System.Windows.Forms.CheckBox();
            this.lblForgot = new System.Windows.Forms.Label();
            this.btnLogin = new Clinic.Login.ClinicGradientButton();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.rightPanel.SuspendLayout();
            this.loginCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.userField.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            this.passwordField.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.leftPanel);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rightPanel);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(1440, 650);
            this.splitContainer1.SplitterDistance = 710;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // leftPanel
            // 
            this.leftPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(47)))), ((int)(((byte)(67)))));
            this.leftPanel.Controls.Add(this.lblVersion);
            this.leftPanel.Controls.Add(this.lblReportsDesc);
            this.leftPanel.Controls.Add(this.lblReportsTitle);
            this.leftPanel.Controls.Add(this.pictureBox6);
            this.leftPanel.Controls.Add(this.pictureBox5);
            this.leftPanel.Controls.Add(this.pictureBox3);
            this.leftPanel.Controls.Add(this.pictureBox2);
            this.leftPanel.Controls.Add(this.pictureBox1);
            this.leftPanel.Controls.Add(this.lblWelcome);
            this.leftPanel.Controls.Add(this.lblSystem);
            this.leftPanel.Controls.Add(this.lblSlogan);
            this.leftPanel.Controls.Add(this.lblAppointmentsTitle);
            this.leftPanel.Controls.Add(this.lblAppointmentsDesc);
            this.leftPanel.Controls.Add(this.lblPatientsTitle);
            this.leftPanel.Controls.Add(this.lblPatientsDesc);
            this.leftPanel.Decorate = true;
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(29)))), ((int)(((byte)(53)))));
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(710, 650);
            this.leftPanel.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(84)))), ((int)(((byte)(106)))));
            this.leftPanel.TabIndex = 0;
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(229)))), ((int)(((byte)(239)))));
            this.lblVersion.Location = new System.Drawing.Point(12, 614);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(230, 25);
            this.lblVersion.TabIndex = 14;
            this.lblVersion.Text = "Version 1.0";
            // 
            // lblReportsDesc
            // 
            this.lblReportsDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblReportsDesc.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblReportsDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(228)))), ((int)(((byte)(240)))));
            this.lblReportsDesc.Location = new System.Drawing.Point(138, 571);
            this.lblReportsDesc.Name = "lblReportsDesc";
            this.lblReportsDesc.Size = new System.Drawing.Size(510, 32);
            this.lblReportsDesc.TabIndex = 13;
            this.lblReportsDesc.Text = "Insights for improved care";
            // 
            // lblReportsTitle
            // 
            this.lblReportsTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblReportsTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblReportsTitle.ForeColor = System.Drawing.Color.White;
            this.lblReportsTitle.Location = new System.Drawing.Point(143, 543);
            this.lblReportsTitle.Name = "lblReportsTitle";
            this.lblReportsTitle.Size = new System.Drawing.Size(510, 31);
            this.lblReportsTitle.TabIndex = 12;
            this.lblReportsTitle.Text = "Reports & Analytics";
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox6.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Neon_Glass_Bar_Chart_Icon;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox6.Location = new System.Drawing.Point(85, 550);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(56, 43);
            this.pictureBox6.TabIndex = 19;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox5.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Neon_User_Profile_Icon;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox5.Location = new System.Drawing.Point(85, 496);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(56, 43);
            this.pictureBox5.TabIndex = 18;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Neon_Clinic_Management_Logo1;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(371, 35);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(213, 152);
            this.pictureBox3.TabIndex = 17;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::Clinic.Properties.Resources.Glowing_Cyan_AI_Profile_Network1;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(365, 218);
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Neon_Glass_Calendar_Icon;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(85, 447);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(56, 43);
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // lblWelcome
            // 
            this.lblWelcome.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 28F);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(233)))), ((int)(((byte)(224)))));
            this.lblWelcome.Location = new System.Drawing.Point(48, 211);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(600, 54);
            this.lblWelcome.TabIndex = 2;
            this.lblWelcome.Text = "Welcome To";
            // 
            // lblSystem
            // 
            this.lblSystem.BackColor = System.Drawing.Color.Transparent;
            this.lblSystem.Font = new System.Drawing.Font("Segoe UI", 34F, System.Drawing.FontStyle.Bold);
            this.lblSystem.ForeColor = System.Drawing.Color.White;
            this.lblSystem.Location = new System.Drawing.Point(52, 255);
            this.lblSystem.Name = "lblSystem";
            this.lblSystem.Size = new System.Drawing.Size(625, 158);
            this.lblSystem.TabIndex = 3;
            this.lblSystem.Text = "Clinic Management\nSystem";
            // 
            // lblSlogan
            // 
            this.lblSlogan.BackColor = System.Drawing.Color.Transparent;
            this.lblSlogan.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSlogan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(234)))), ((int)(((byte)(243)))));
            this.lblSlogan.Location = new System.Drawing.Point(60, 411);
            this.lblSlogan.Name = "lblSlogan";
            this.lblSlogan.Size = new System.Drawing.Size(635, 33);
            this.lblSlogan.TabIndex = 4;
            this.lblSlogan.Text = "PROFESSIONAL CARE   |   BETTER HEALTH   |   BRIGHTER TOMORROW";
            // 
            // lblAppointmentsTitle
            // 
            this.lblAppointmentsTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblAppointmentsTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblAppointmentsTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppointmentsTitle.Location = new System.Drawing.Point(147, 438);
            this.lblAppointmentsTitle.Name = "lblAppointmentsTitle";
            this.lblAppointmentsTitle.Size = new System.Drawing.Size(510, 31);
            this.lblAppointmentsTitle.TabIndex = 6;
            this.lblAppointmentsTitle.Text = "Manage Appointments";
            // 
            // lblAppointmentsDesc
            // 
            this.lblAppointmentsDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblAppointmentsDesc.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblAppointmentsDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(228)))), ((int)(((byte)(240)))));
            this.lblAppointmentsDesc.Location = new System.Drawing.Point(147, 465);
            this.lblAppointmentsDesc.Name = "lblAppointmentsDesc";
            this.lblAppointmentsDesc.Size = new System.Drawing.Size(510, 27);
            this.lblAppointmentsDesc.TabIndex = 7;
            this.lblAppointmentsDesc.Text = "Efficient scheduling for better service";
            // 
            // lblPatientsTitle
            // 
            this.lblPatientsTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblPatientsTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblPatientsTitle.ForeColor = System.Drawing.Color.White;
            this.lblPatientsTitle.Location = new System.Drawing.Point(147, 488);
            this.lblPatientsTitle.Name = "lblPatientsTitle";
            this.lblPatientsTitle.Size = new System.Drawing.Size(510, 31);
            this.lblPatientsTitle.TabIndex = 9;
            this.lblPatientsTitle.Text = "Patient Management";
            // 
            // lblPatientsDesc
            // 
            this.lblPatientsDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblPatientsDesc.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblPatientsDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(228)))), ((int)(((byte)(240)))));
            this.lblPatientsDesc.Location = new System.Drawing.Point(147, 515);
            this.lblPatientsDesc.Name = "lblPatientsDesc";
            this.lblPatientsDesc.Size = new System.Drawing.Size(510, 27);
            this.lblPatientsDesc.TabIndex = 10;
            this.lblPatientsDesc.Text = "Comprehensive patient records";
            // 
            // rightPanel
            // 
            this.rightPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(247)))), ((int)(((byte)(254)))));
            this.rightPanel.Controls.Add(this.btnClose);
            this.rightPanel.Controls.Add(this.loginCard);
            this.rightPanel.Decorate = true;
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.rightPanel.Location = new System.Drawing.Point(0, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(729, 650);
            this.rightPanel.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(246)))), ((int)(((byte)(254)))));
            this.rightPanel.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(248)))), ((int)(((byte)(253)))));
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(77)))), ((int)(((byte)(105)))));
            this.btnClose.Location = new System.Drawing.Point(688, -3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(36, 72);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "×";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // loginCard
            // 
            this.loginCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.loginCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(230)))), ((int)(((byte)(241)))));
            this.loginCard.Controls.Add(this.btnRegister);
            this.loginCard.Controls.Add(this.lblNewUser);
            this.loginCard.Controls.Add(this.pictureBox4);
            this.loginCard.Controls.Add(this.lblLoginTitle);
            this.loginCard.Controls.Add(this.lblSubtitle);
            this.loginCard.Controls.Add(this.userField);
            this.loginCard.Controls.Add(this.passwordField);
            this.loginCard.Controls.Add(this.chkRememberMe);
            this.loginCard.Controls.Add(this.lblForgot);
            this.loginCard.Controls.Add(this.btnLogin);
            this.loginCard.CornerRadius = 29;
            this.loginCard.Location = new System.Drawing.Point(35, 28);
            this.loginCard.Name = "loginCard";
            this.loginCard.Size = new System.Drawing.Size(650, 600);
            this.loginCard.TabIndex = 1;
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.btnRegister.CornerRadius = 14;
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(124)))), ((int)(((byte)(173)))));
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(111)))), ((int)(((byte)(139)))));
            this.btnRegister.Location = new System.Drawing.Point(81, 538);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(526, 55);
            this.btnRegister.TabIndex = 4;
            this.btnRegister.Text = "✚  Register Account";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lblNewUser
            // 
            this.lblNewUser.BackColor = System.Drawing.Color.Transparent;
            this.lblNewUser.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblNewUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(138)))), ((int)(((byte)(160)))));
            this.lblNewUser.Location = new System.Drawing.Point(76, 494);
            this.lblNewUser.Name = "lblNewUser";
            this.lblNewUser.Size = new System.Drawing.Size(526, 46);
            this.lblNewUser.TabIndex = 6;
            this.lblNewUser.Text = "──────────   New to the system?   ──────────";
            this.lblNewUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Blue_Caduceus_Medical_Badge1;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(246, 16);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(153, 101);
            this.pictureBox4.TabIndex = 18;
            this.pictureBox4.TabStop = false;
            // 
            // lblLoginTitle
            // 
            this.lblLoginTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblLoginTitle.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblLoginTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(27)))), ((int)(((byte)(52)))));
            this.lblLoginTitle.Location = new System.Drawing.Point(34, 120);
            this.lblLoginTitle.Name = "lblLoginTitle";
            this.lblLoginTitle.Size = new System.Drawing.Size(600, 65);
            this.lblLoginTitle.TabIndex = 1;
            this.lblLoginTitle.Text = "Login to your account";
            this.lblLoginTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(125)))), ((int)(((byte)(149)))));
            this.lblSubtitle.Location = new System.Drawing.Point(44, 185);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(590, 36);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "Access your clinic management system";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // userField
            // 
            this.userField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.userField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(220)))), ((int)(((byte)(237)))));
            this.userField.Controls.Add(this.picUser);
            this.userField.Controls.Add(this.txtUserName);
            this.userField.CornerRadius = 30;
            this.userField.Location = new System.Drawing.Point(79, 239);
            this.userField.Name = "userField";
            this.userField.Size = new System.Drawing.Size(523, 67);
            this.userField.TabIndex = 3;
            // 
            // picUser
            // 
            this.picUser.BackColor = System.Drawing.Color.Transparent;
            this.picUser.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Teal_Glass_User_Avatar_Icon;
            this.picUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUser.Location = new System.Drawing.Point(20, 20);
            this.picUser.Name = "picUser";
            this.picUser.Size = new System.Drawing.Size(54, 33);
            this.picUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picUser.TabIndex = 0;
            this.picUser.TabStop = false;
            // 
            // txtUserName
            // 
            this.txtUserName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserName.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(55)))), ((int)(((byte)(79)))));
            this.txtUserName.Location = new System.Drawing.Point(91, 20);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(409, 32);
            this.txtUserName.TabIndex = 0;
            this.txtUserName.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValiditin);
            // 
            // passwordField
            // 
            this.passwordField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.passwordField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(220)))), ((int)(((byte)(237)))));
            this.passwordField.Controls.Add(this.picLock);
            this.passwordField.Controls.Add(this.txtPassword);
            this.passwordField.Controls.Add(this.lblEye);
            this.passwordField.CornerRadius = 30;
            this.passwordField.Location = new System.Drawing.Point(79, 322);
            this.passwordField.Name = "passwordField";
            this.passwordField.Size = new System.Drawing.Size(526, 67);
            this.passwordField.TabIndex = 4;
            // 
            // picLock
            // 
            this.picLock.BackColor = System.Drawing.Color.Transparent;
            this.picLock.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Blue_3D_Padlock_Icon2;
            this.picLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picLock.Location = new System.Drawing.Point(20, 19);
            this.picLock.Name = "picLock";
            this.picLock.Size = new System.Drawing.Size(54, 33);
            this.picLock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picLock.TabIndex = 0;
            this.picLock.TabStop = false;
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(55)))), ((int)(((byte)(79)))));
            this.txtPassword.Location = new System.Drawing.Point(91, 20);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(355, 32);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValiditin);
            // 
            // lblEye
            // 
            this.lblEye.BackColor = System.Drawing.Color.Transparent;
            this.lblEye.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblEye.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblEye.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(112)))), ((int)(((byte)(135)))));
            this.lblEye.Location = new System.Drawing.Point(468, 10);
            this.lblEye.Name = "lblEye";
            this.lblEye.Size = new System.Drawing.Size(48, 47);
            this.lblEye.TabIndex = 2;
            this.lblEye.Text = "◉";
            this.lblEye.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEye.Click += new System.EventHandler(this.lblEye_Click);
            // 
            // chkRememberMe
            // 
            this.chkRememberMe.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.chkRememberMe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(48)))), ((int)(((byte)(70)))));
            this.chkRememberMe.Location = new System.Drawing.Point(79, 399);
            this.chkRememberMe.Name = "chkRememberMe";
            this.chkRememberMe.Size = new System.Drawing.Size(210, 34);
            this.chkRememberMe.TabIndex = 2;
            this.chkRememberMe.Text = "Remember Me";
            // 
            // lblForgot
            // 
            this.lblForgot.BackColor = System.Drawing.Color.Transparent;
            this.lblForgot.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblForgot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(82)))), ((int)(((byte)(218)))));
            this.lblForgot.Location = new System.Drawing.Point(388, 394);
            this.lblForgot.Name = "lblForgot";
            this.lblForgot.Size = new System.Drawing.Size(216, 34);
            this.lblForgot.TabIndex = 5;
            this.lblForgot.Text = "Forgot Password?";
            this.lblForgot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(175)))), ((int)(((byte)(171)))));
            this.btnLogin.CornerRadius = 0;
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(67)))), ((int)(((byte)(190)))));
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(205, 439);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(279, 58);
            this.btnLogin.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(199)))), ((int)(((byte)(168)))));
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "➜  Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // frmLogin
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1440, 650);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(1440, 650);
            this.MinimumSize = new System.Drawing.Size(1440, 650);
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clinic Management System - Login";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.rightPanel.ResumeLayout(false);
            this.loginCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.userField.ResumeLayout(false);
            this.userField.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
            this.passwordField.ResumeLayout(false);
            this.passwordField.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox6;
        private PictureBox pictureBox5;
        private ClinicRoundedPanel userField;
        private PictureBox picUser;
        private PictureBox picLock;
    }

    // Designer-safe custom drawing. No changes to frmLogin.cs are required.
    public class ClinicGradientPanel : Panel
    {
        public Color StartColor { get; set; } = Color.FromArgb(0, 72, 93);
        public Color EndColor { get; set; } = Color.FromArgb(0, 32, 55);
        public bool Decorate { get; set; }
        public ClinicGradientPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new LinearGradientBrush(ClientRectangle, StartColor, EndColor,
                                                        LinearGradientMode.ForwardDiagonal))
                g.FillRectangle(brush, ClientRectangle);
            if (Decorate)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(Width / 2 - 170, -Height / 3, Width, Height + 360);
                    using (var b = new SolidBrush(Color.FromArgb(19, Color.White)))
                        g.FillPath(b, path);
                }
                using (var pen = new Pen(Color.FromArgb(50, 33, 217, 232), 2f))
                {
                    g.DrawArc(pen, Width / 2, -Height / 2, Width, Height * 2, 75, 165);
                    g.DrawArc(pen, Width / 2 + 28, -Height / 2, Width, Height * 2, 75, 165);
                }
            }
        }
    }

    public class ClinicRoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 18;
        public Color BorderColor { get; set; } = Color.FromArgb(194, 226, 239);
        public ClinicRoundedPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = ClinicShapes.Rounded(new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius))
            using (var pen = new Pen(BorderColor, 1.3f))
                e.Graphics.DrawPath(pen, path);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width < 2 || Height < 2) return;
            using (var path = ClinicShapes.Rounded(new Rectangle(0, 0, Width, Height), CornerRadius))
                Region = new Region(path);
        }
    }

    internal static class ClinicShapes
    {
        internal static GraphicsPath Rounded(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = Math.Max(1, Math.Min(radius * 2, Math.Min(r.Width, r.Height)));
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }

    public class ClinicGradientButton : Button
    {
        public Color StartColor { get; set; } = Color.FromArgb(0, 197, 165);
        public Color EndColor { get; set; } = Color.FromArgb(0, 68, 192);
        public int CornerRadius { get; set; } = 16;
        public ClinicGradientButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var p = ClinicShapes.Rounded(new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius))
            using (var b = new LinearGradientBrush(ClientRectangle, StartColor, EndColor,
                                                    LinearGradientMode.Horizontal))
            {
                e.Graphics.FillPath(b, p);
                if (!Enabled) using (var dim = new SolidBrush(Color.FromArgb(110, Color.White)))
                        e.Graphics.FillPath(dim, p);
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }

    public class ClinicOutlineButton : Button
    {
        public int CornerRadius { get; set; } = 13;
        public ClinicOutlineButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var p = ClinicShapes.Rounded(new Rectangle(1, 1, Width - 3, Height - 3), CornerRadius))
            using (var b = new SolidBrush(BackColor))
            using (var pen = new Pen(Color.FromArgb(0, 116, 177), 1.5f))
            {
                e.Graphics.FillPath(b, p);
                e.Graphics.DrawPath(pen, p);
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }

    public class ClinicQualityPictureBox : PictureBox
    {
        public ClinicQualityPictureBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null) { base.OnPaint(e); return; }
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            float scale = Math.Min((float)Width / Image.Width, (float)Height / Image.Height);
            int w = (int)(Image.Width * scale), h = (int)(Image.Height * scale);
            e.Graphics.DrawImage(Image, new Rectangle((Width - w) / 2, (Height - h) / 2, w, h),
                                 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
        }
    }

}
