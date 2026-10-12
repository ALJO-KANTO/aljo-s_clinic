using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Login
{
    public partial class frmRegister
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelBrand;
        private Panel panelForm;
        private Panel panelFields;
        private Label lblBrandHeading;
        private Label lblBrandDescription;
        private Label lblBrandFeatures;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblApproval;
        private Label lblField0;
        private Label lblField1;
        private Label lblField2;
        private Label lblField3;
        private Label lblField4;
        private Label lblField5;
        private Label lblField6;
        private Label lblField7;
        private Label lblField8;
        private Label lblField9;
        private TextBox fullName;
        private TextBox phone;
        private TextBox email;
        private TextBox address;
        private TextBox username;
        private TextBox password;
        private TextBox confirm;
        private DateTimePicker dob;
        private ComboBox gender;
        private ComboBox country;
        private Button btnClose;
        private Button btnRegister;
        private Button btnShowPassword;
        private Button btnShowConfirm;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelBrand = new System.Windows.Forms.Panel();
            this.lblBrandHeading = new System.Windows.Forms.Label();
            this.lblBrandDescription = new System.Windows.Forms.Label();
            this.lblBrandFeatures = new System.Windows.Forms.Label();
            this.panelForm = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.panelFields = new System.Windows.Forms.Panel();
            this.lblField0 = new System.Windows.Forms.Label();
            this.lblField1 = new System.Windows.Forms.Label();
            this.lblField2 = new System.Windows.Forms.Label();
            this.lblField3 = new System.Windows.Forms.Label();
            this.lblField4 = new System.Windows.Forms.Label();
            this.lblField5 = new System.Windows.Forms.Label();
            this.lblField6 = new System.Windows.Forms.Label();
            this.lblField7 = new System.Windows.Forms.Label();
            this.lblField8 = new System.Windows.Forms.Label();
            this.lblField9 = new System.Windows.Forms.Label();
            this.fullName = new System.Windows.Forms.TextBox();
            this.dob = new System.Windows.Forms.DateTimePicker();
            this.gender = new System.Windows.Forms.ComboBox();
            this.phone = new System.Windows.Forms.TextBox();
            this.email = new System.Windows.Forms.TextBox();
            this.address = new System.Windows.Forms.TextBox();
            this.country = new System.Windows.Forms.ComboBox();
            this.username = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.confirm = new System.Windows.Forms.TextBox();
            this.btnShowPassword = new System.Windows.Forms.Button();
            this.btnShowConfirm = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.lblApproval = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelBrand.SuspendLayout();
            this.panelForm.SuspendLayout();
            this.panelFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBrand
            // 
            this.panelBrand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(58)))), ((int)(((byte)(86)))));
            this.panelBrand.Controls.Add(this.pictureBox4);
            this.panelBrand.Controls.Add(this.pictureBox3);
            this.panelBrand.Controls.Add(this.pictureBox2);
            this.panelBrand.Controls.Add(this.pictureBox1);
            this.panelBrand.Controls.Add(this.lblBrandHeading);
            this.panelBrand.Controls.Add(this.lblBrandDescription);
            this.panelBrand.Controls.Add(this.lblBrandFeatures);
            this.panelBrand.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelBrand.Location = new System.Drawing.Point(0, 0);
            this.panelBrand.Name = "panelBrand";
            this.panelBrand.Size = new System.Drawing.Size(400, 780);
            this.panelBrand.TabIndex = 0;
            // 
            // lblBrandHeading
            // 
            this.lblBrandHeading.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandHeading.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblBrandHeading.ForeColor = System.Drawing.Color.White;
            this.lblBrandHeading.Location = new System.Drawing.Point(35, 445);
            this.lblBrandHeading.Name = "lblBrandHeading";
            this.lblBrandHeading.Size = new System.Drawing.Size(350, 55);
            this.lblBrandHeading.TabIndex = 3;
            this.lblBrandHeading.Text = "Create Your Account";
            // 
            // lblBrandDescription
            // 
            this.lblBrandDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandDescription.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblBrandDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(236)))), ((int)(((byte)(244)))));
            this.lblBrandDescription.Location = new System.Drawing.Point(35, 500);
            this.lblBrandDescription.Name = "lblBrandDescription";
            this.lblBrandDescription.Size = new System.Drawing.Size(340, 100);
            this.lblBrandDescription.TabIndex = 4;
            this.lblBrandDescription.Text = "Join our clinic management system\nand be part of better healthcare\ntomorrow.";
            // 
            // lblBrandFeatures
            // 
            this.lblBrandFeatures.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandFeatures.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblBrandFeatures.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.lblBrandFeatures.Location = new System.Drawing.Point(30, 678);
            this.lblBrandFeatures.Name = "lblBrandFeatures";
            this.lblBrandFeatures.Size = new System.Drawing.Size(345, 70);
            this.lblBrandFeatures.TabIndex = 5;
            this.lblBrandFeatures.Text = "✓ Secure Registration    ✓ Easy Access\n\n       Better Healthcare";
            // 
            // panelForm
            // 
            this.panelForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(250)))), ((int)(((byte)(254)))));
            this.panelForm.Controls.Add(this.btnClose);
            this.panelForm.Controls.Add(this.lblTitle);
            this.panelForm.Controls.Add(this.lblSubtitle);
            this.panelForm.Controls.Add(this.panelFields);
            this.panelForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForm.Location = new System.Drawing.Point(400, 0);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new System.Drawing.Size(830, 780);
            this.panelForm.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(34)))), ((int)(((byte)(67)))));
            this.btnClose.Location = new System.Drawing.Point(769, 18);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(42, 40);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "×";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(34)))), ((int)(((byte)(67)))));
            this.lblTitle.Location = new System.Drawing.Point(72, 55);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(690, 54);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Register Clinic Account";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(118)))), ((int)(((byte)(146)))));
            this.lblSubtitle.Location = new System.Drawing.Point(74, 112);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(660, 32);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "Fill in your details to create a new account";
            // 
            // panelFields
            // 
            this.panelFields.BackColor = System.Drawing.Color.White;
            this.panelFields.Controls.Add(this.lblField0);
            this.panelFields.Controls.Add(this.lblField1);
            this.panelFields.Controls.Add(this.lblField2);
            this.panelFields.Controls.Add(this.lblField3);
            this.panelFields.Controls.Add(this.lblField4);
            this.panelFields.Controls.Add(this.lblField5);
            this.panelFields.Controls.Add(this.lblField6);
            this.panelFields.Controls.Add(this.lblField7);
            this.panelFields.Controls.Add(this.lblField8);
            this.panelFields.Controls.Add(this.lblField9);
            this.panelFields.Controls.Add(this.fullName);
            this.panelFields.Controls.Add(this.dob);
            this.panelFields.Controls.Add(this.gender);
            this.panelFields.Controls.Add(this.phone);
            this.panelFields.Controls.Add(this.email);
            this.panelFields.Controls.Add(this.address);
            this.panelFields.Controls.Add(this.country);
            this.panelFields.Controls.Add(this.username);
            this.panelFields.Controls.Add(this.password);
            this.panelFields.Controls.Add(this.confirm);
            this.panelFields.Controls.Add(this.btnShowPassword);
            this.panelFields.Controls.Add(this.btnShowConfirm);
            this.panelFields.Controls.Add(this.btnRegister);
            this.panelFields.Controls.Add(this.lblApproval);
            this.panelFields.Location = new System.Drawing.Point(42, 165);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(744, 583);
            this.panelFields.TabIndex = 3;
            // 
            // lblField0
            // 
            this.lblField0.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField0.Location = new System.Drawing.Point(28, 19);
            this.lblField0.Name = "lblField0";
            this.lblField0.Size = new System.Drawing.Size(315, 27);
            this.lblField0.TabIndex = 0;
            this.lblField0.Text = "Full Name";
            // 
            // lblField1
            // 
            this.lblField1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField1.Location = new System.Drawing.Point(390, 19);
            this.lblField1.Name = "lblField1";
            this.lblField1.Size = new System.Drawing.Size(315, 27);
            this.lblField1.TabIndex = 1;
            this.lblField1.Text = "Date of Birth";
            // 
            // lblField2
            // 
            this.lblField2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField2.Location = new System.Drawing.Point(28, 110);
            this.lblField2.Name = "lblField2";
            this.lblField2.Size = new System.Drawing.Size(315, 27);
            this.lblField2.TabIndex = 2;
            this.lblField2.Text = "Gender";
            // 
            // lblField3
            // 
            this.lblField3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField3.Location = new System.Drawing.Point(390, 110);
            this.lblField3.Name = "lblField3";
            this.lblField3.Size = new System.Drawing.Size(315, 27);
            this.lblField3.TabIndex = 3;
            this.lblField3.Text = "Phone";
            // 
            // lblField4
            // 
            this.lblField4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField4.Location = new System.Drawing.Point(28, 201);
            this.lblField4.Name = "lblField4";
            this.lblField4.Size = new System.Drawing.Size(315, 27);
            this.lblField4.TabIndex = 4;
            this.lblField4.Text = "Email";
            // 
            // lblField5
            // 
            this.lblField5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField5.Location = new System.Drawing.Point(390, 201);
            this.lblField5.Name = "lblField5";
            this.lblField5.Size = new System.Drawing.Size(315, 27);
            this.lblField5.TabIndex = 5;
            this.lblField5.Text = "Address";
            // 
            // lblField6
            // 
            this.lblField6.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField6.Location = new System.Drawing.Point(28, 292);
            this.lblField6.Name = "lblField6";
            this.lblField6.Size = new System.Drawing.Size(315, 27);
            this.lblField6.TabIndex = 6;
            this.lblField6.Text = "Nationality";
            // 
            // lblField7
            // 
            this.lblField7.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField7.Location = new System.Drawing.Point(390, 292);
            this.lblField7.Name = "lblField7";
            this.lblField7.Size = new System.Drawing.Size(315, 27);
            this.lblField7.TabIndex = 7;
            this.lblField7.Text = "Username";
            // 
            // lblField8
            // 
            this.lblField8.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField8.Location = new System.Drawing.Point(28, 383);
            this.lblField8.Name = "lblField8";
            this.lblField8.Size = new System.Drawing.Size(315, 27);
            this.lblField8.TabIndex = 8;
            this.lblField8.Text = "Password";
            // 
            // lblField9
            // 
            this.lblField9.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblField9.Location = new System.Drawing.Point(390, 383);
            this.lblField9.Name = "lblField9";
            this.lblField9.Size = new System.Drawing.Size(315, 27);
            this.lblField9.TabIndex = 9;
            this.lblField9.Text = "Confirm Password";
            // 
            // fullName
            // 
            this.fullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fullName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.fullName.Location = new System.Drawing.Point(28, 52);
            this.fullName.Name = "fullName";
            this.fullName.Size = new System.Drawing.Size(314, 32);
            this.fullName.TabIndex = 10;
            // 
            // dob
            // 
            this.dob.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dob.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dob.Location = new System.Drawing.Point(390, 52);
            this.dob.Name = "dob";
            this.dob.Size = new System.Drawing.Size(314, 32);
            this.dob.TabIndex = 11;
            // 
            // gender
            // 
            this.gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gender.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gender.Location = new System.Drawing.Point(28, 143);
            this.gender.Name = "gender";
            this.gender.Size = new System.Drawing.Size(314, 33);
            this.gender.TabIndex = 12;
            // 
            // phone
            // 
            this.phone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.phone.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.phone.Location = new System.Drawing.Point(390, 143);
            this.phone.Name = "phone";
            this.phone.Size = new System.Drawing.Size(314, 32);
            this.phone.TabIndex = 13;
            // 
            // email
            // 
            this.email.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.email.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.email.Location = new System.Drawing.Point(28, 234);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(314, 32);
            this.email.TabIndex = 14;
            // 
            // address
            // 
            this.address.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.address.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.address.Location = new System.Drawing.Point(390, 234);
            this.address.Name = "address";
            this.address.Size = new System.Drawing.Size(314, 32);
            this.address.TabIndex = 15;
            // 
            // country
            // 
            this.country.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.country.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.country.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.country.Location = new System.Drawing.Point(28, 325);
            this.country.Name = "country";
            this.country.Size = new System.Drawing.Size(314, 33);
            this.country.TabIndex = 16;
            // 
            // username
            // 
            this.username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.username.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.username.Location = new System.Drawing.Point(390, 325);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(314, 32);
            this.username.TabIndex = 17;
            // 
            // password
            // 
            this.password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.password.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.password.Location = new System.Drawing.Point(28, 416);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(252, 32);
            this.password.TabIndex = 18;
            this.password.UseSystemPasswordChar = true;
            // 
            // confirm
            // 
            this.confirm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.confirm.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.confirm.Location = new System.Drawing.Point(390, 416);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(252, 32);
            this.confirm.TabIndex = 19;
            this.confirm.UseSystemPasswordChar = true;
            // 
            // btnShowPassword
            // 
            this.btnShowPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(248)))), ((int)(((byte)(253)))));
            this.btnShowPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowPassword.FlatAppearance.BorderSize = 0;
            this.btnShowPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowPassword.Location = new System.Drawing.Point(280, 415);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.Size = new System.Drawing.Size(62, 33);
            this.btnShowPassword.TabIndex = 20;
            this.btnShowPassword.Text = "Show";
            this.btnShowPassword.UseVisualStyleBackColor = false;
            // 
            // btnShowConfirm
            // 
            this.btnShowConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(248)))), ((int)(((byte)(253)))));
            this.btnShowConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowConfirm.FlatAppearance.BorderSize = 0;
            this.btnShowConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowConfirm.Location = new System.Drawing.Point(642, 415);
            this.btnShowConfirm.Name = "btnShowConfirm";
            this.btnShowConfirm.Size = new System.Drawing.Size(62, 33);
            this.btnShowConfirm.TabIndex = 21;
            this.btnShowConfirm.Text = "Show";
            this.btnShowConfirm.UseVisualStyleBackColor = false;
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(159)))), ((int)(((byte)(191)))));
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(28, 480);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(676, 53);
            this.btnRegister.TabIndex = 12;
            this.btnRegister.Text = "Register Account";
            this.btnRegister.UseVisualStyleBackColor = false;
            // 
            // lblApproval
            // 
            this.lblApproval.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblApproval.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(118)))), ((int)(((byte)(146)))));
            this.lblApproval.Location = new System.Drawing.Point(28, 538);
            this.lblApproval.Name = "lblApproval";
            this.lblApproval.Size = new System.Drawing.Size(680, 30);
            this.lblApproval.TabIndex = 22;
            this.lblApproval.Text = "Registration requires administrator approval before login.";
            this.lblApproval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImage = global::Clinic.Properties.Resources.Neon_Blue_Group_Users_Icon;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(275, 641);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(52, 34);
            this.pictureBox4.TabIndex = 9;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Neon_Shield_Checkmark_Icon;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(102, 641);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(52, 34);
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Blue_Add_User_Profile_Icon;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(77, 308);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(203, 134);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::Clinic.Properties.Resources.Glossy_Neon_Clinic_Management_Logo2;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(54, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(260, 187);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // frmRegister
            // 
            this.AcceptButton = this.btnRegister;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(247)))), ((int)(((byte)(253)))));
            this.ClientSize = new System.Drawing.Size(1230, 780);
            this.Controls.Add(this.panelForm);
            this.Controls.Add(this.panelBrand);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1230, 780);
            this.MinimumSize = new System.Drawing.Size(1230, 780);
            this.Name = "frmRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Register Clinic Account";
            this.panelBrand.ResumeLayout(false);
            this.panelForm.ResumeLayout(false);
            this.panelFields.ResumeLayout(false);
            this.panelFields.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
    }
}
