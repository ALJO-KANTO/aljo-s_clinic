using static Clinic.Medical_Services.Pharmaciy.frmAllPrescriptions;

namespace Clinic.Medical_Services.Pharmaciy
{
    partial class frmManageDrugs
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panTitle = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.lbCurrentUser = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.nudReorderLevel = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudCurrentStock = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMedicinePrice = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMedicneName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbBlaceholder = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgMedicines = new System.Windows.Forms.DataGridView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.txtBatchNo = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.dtpExpiryDate = new System.Windows.Forms.DateTimePicker();
            this.cmbWarehouse = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.nudTaxRate = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtMedicineCost = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReorderLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentStock)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMedicines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panTitle
            // 
            this.panTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(107)))));
            this.panTitle.Controls.Add(this.label5);
            this.panTitle.Controls.Add(this.pictureBox4);
            this.panTitle.Controls.Add(this.btnExit);
            this.panTitle.Controls.Add(this.lbCurrentUser);
            this.panTitle.Controls.Add(this.lblTitle);
            this.panTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTitle.Location = new System.Drawing.Point(20, 20);
            this.panTitle.Name = "panTitle";
            this.panTitle.Size = new System.Drawing.Size(1318, 77);
            this.panTitle.TabIndex = 253;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(1000, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 21);
            this.label5.TabIndex = 240;
            this.label5.Text = "Current User:";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.Image = global::Clinic.Properties.Resources.Person_32;
            this.pictureBox4.Location = new System.Drawing.Point(25, 22);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(41, 31);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 255;
            this.pictureBox4.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Image = global::Clinic.Properties.Resources.sign_out_32__2;
            this.btnExit.Location = new System.Drawing.Point(1230, 15);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 45);
            this.btnExit.TabIndex = 254;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lbCurrentUser
            // 
            this.lbCurrentUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCurrentUser.AutoSize = true;
            this.lbCurrentUser.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lbCurrentUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(223)))), ((int)(((byte)(219)))));
            this.lbCurrentUser.Location = new System.Drawing.Point(1143, 24);
            this.lbCurrentUser.Name = "lbCurrentUser";
            this.lbCurrentUser.Size = new System.Drawing.Size(43, 21);
            this.lbCurrentUser.TabIndex = 241;
            this.lbCurrentUser.Text = "[???]";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 19.8F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(75, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(327, 37);
            this.lblTitle.TabIndex = 236;
            this.lblTitle.Text = "Pharmacist Manage Drug";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.txtMedicineCost);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.nudTaxRate);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.cmbWarehouse);
            this.panel1.Controls.Add(this.dtpExpiryDate);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.txtBatchNo);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.nudReorderLevel);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.nudCurrentStock);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtMedicinePrice);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtMedicneName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(20, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1318, 353);
            this.panel1.TabIndex = 254;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label7.Location = new System.Drawing.Point(186, 230);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 21);
            this.label7.TabIndex = 250;
            this.label7.Text = "Tax Rate:";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = global::Clinic.Properties.Resources.show_medicine_32;
            this.pictureBox2.Location = new System.Drawing.Point(387, 290);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 36);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 249;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(367, 280);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(540, 55);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Add New Drug";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // nudReorderLevel
            // 
            this.nudReorderLevel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudReorderLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.nudReorderLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudReorderLevel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.nudReorderLevel.Location = new System.Drawing.Point(367, 175);
            this.nudReorderLevel.Name = "nudReorderLevel";
            this.nudReorderLevel.Size = new System.Drawing.Size(127, 29);
            this.nudReorderLevel.TabIndex = 7;
            this.nudReorderLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudReorderLevel.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label4.Location = new System.Drawing.Point(186, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 21);
            this.label4.TabIndex = 6;
            this.label4.Text = "Reorder Level:";
            // 
            // nudCurrentStock
            // 
            this.nudCurrentStock.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nudCurrentStock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.nudCurrentStock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudCurrentStock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.nudCurrentStock.Location = new System.Drawing.Point(367, 125);
            this.nudCurrentStock.Name = "nudCurrentStock";
            this.nudCurrentStock.Size = new System.Drawing.Size(185, 29);
            this.nudCurrentStock.TabIndex = 5;
            this.nudCurrentStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCurrentStock.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label3.Location = new System.Drawing.Point(186, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 21);
            this.label3.TabIndex = 4;
            this.label3.Text = "Current Stock:";
            // 
            // txtMedicinePrice
            // 
            this.txtMedicinePrice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtMedicinePrice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.txtMedicinePrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMedicinePrice.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtMedicinePrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtMedicinePrice.Location = new System.Drawing.Point(367, 78);
            this.txtMedicinePrice.Name = "txtMedicinePrice";
            this.txtMedicinePrice.Size = new System.Drawing.Size(185, 29);
            this.txtMedicinePrice.TabIndex = 3;
            this.txtMedicinePrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMedicinePrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMedicinePrice_KeyPress);
            this.txtMedicinePrice.Validating += new System.ComponentModel.CancelEventHandler(this.txtMedicinePrice_Validating);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label2.Location = new System.Drawing.Point(186, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Medicine Price:";
            // 
            // txtMedicneName
            // 
            this.txtMedicneName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtMedicneName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.txtMedicneName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMedicneName.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtMedicneName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtMedicneName.Location = new System.Drawing.Point(367, 28);
            this.txtMedicneName.Name = "txtMedicneName";
            this.txtMedicneName.Size = new System.Drawing.Size(540, 29);
            this.txtMedicneName.TabIndex = 1;
            this.txtMedicneName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMedicneName.Validating += new System.ComponentModel.CancelEventHandler(this.txtMedicneName_Validating);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label1.Location = new System.Drawing.Point(186, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Medicine Name:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.lbBlaceholder);
            this.panel2.Controls.Add(this.txtSearch);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(20, 450);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1318, 59);
            this.panel2.TabIndex = 256;
            // 
            // lbBlaceholder
            // 
            this.lbBlaceholder.AutoSize = true;
            this.lbBlaceholder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.lbBlaceholder.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Italic);
            this.lbBlaceholder.ForeColor = System.Drawing.Color.DarkGray;
            this.lbBlaceholder.Location = new System.Drawing.Point(135, 20);
            this.lbBlaceholder.Name = "lbBlaceholder";
            this.lbBlaceholder.Size = new System.Drawing.Size(194, 20);
            this.lbBlaceholder.TabIndex = 252;
            this.lbBlaceholder.Text = "Search By Medicine Name .....";
            this.lbBlaceholder.Click += new System.EventHandler(this.lbBlaceholder_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtSearch.Location = new System.Drawing.Point(118, 14);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(540, 29);
            this.txtSearch.TabIndex = 251;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label6.Location = new System.Drawing.Point(19, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 21);
            this.label6.TabIndex = 250;
            this.label6.Text = "Search";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.dgMedicines);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(20, 509);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1318, 220);
            this.panel3.TabIndex = 257;
            // 
            // dgMedicines
            // 
            this.dgMedicines.AllowUserToAddRows = false;
            this.dgMedicines.AllowUserToDeleteRows = false;
            this.dgMedicines.AllowUserToResizeRows = false;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White;
            this.dgMedicines.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgMedicines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgMedicines.BackgroundColor = System.Drawing.Color.White;
            this.dgMedicines.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgMedicines.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(107)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(107)))));
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgMedicines.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgMedicines.ColumnHeadersHeight = 50;
            this.dgMedicines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgMedicines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgMedicines.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgMedicines.EnableHeadersVisualStyles = false;
            this.dgMedicines.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(218)))));
            this.dgMedicines.Location = new System.Drawing.Point(0, 0);
            this.dgMedicines.MultiSelect = false;
            this.dgMedicines.Name = "dgMedicines";
            this.dgMedicines.ReadOnly = true;
            this.dgMedicines.RowHeadersVisible = false;
            this.dgMedicines.RowHeadersWidth = 51;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.White;
            this.dgMedicines.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.dgMedicines.RowTemplate.Height = 45;
            this.dgMedicines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgMedicines.Size = new System.Drawing.Size(1318, 220);
            this.dgMedicines.TabIndex = 256;
            this.dgMedicines.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgMedicines_CellContentClick);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label9.Location = new System.Drawing.Point(760, 204);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(147, 21);
            this.label9.TabIndex = 253;
            this.label9.Text = "Expiry Date";
            // 
            // txtBatchNo
            // 
            this.txtBatchNo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBatchNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.txtBatchNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBatchNo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtBatchNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtBatchNo.Location = new System.Drawing.Point(588, 230);
            this.txtBatchNo.Name = "txtBatchNo";
            this.txtBatchNo.Size = new System.Drawing.Size(136, 29);
            this.txtBatchNo.TabIndex = 254;
            this.txtBatchNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label10.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label10.Location = new System.Drawing.Point(588, 204);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(136, 21);
            this.label10.TabIndex = 255;
            this.label10.Text = "Batch No";
            // 
            // dtpExpiryDate
            // 
            this.dtpExpiryDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpiryDate.Location = new System.Drawing.Point(764, 230);
            this.dtpExpiryDate.Name = "dtpExpiryDate";
            this.dtpExpiryDate.Size = new System.Drawing.Size(143, 29);
            this.dtpExpiryDate.TabIndex = 256;
            // 
            // cmbWarehouse
            // 
            this.cmbWarehouse.BackColor = System.Drawing.Color.Transparent;
            this.cmbWarehouse.BorderRadius = 6;
            this.cmbWarehouse.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbWarehouse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWarehouse.FocusedColor = System.Drawing.Color.Empty;
            this.cmbWarehouse.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbWarehouse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbWarehouse.ItemHeight = 26;
            this.cmbWarehouse.Location = new System.Drawing.Point(592, 167);
            this.cmbWarehouse.Name = "cmbWarehouse";
            this.cmbWarehouse.Size = new System.Drawing.Size(315, 32);
            this.cmbWarehouse.TabIndex = 257;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label11.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label11.Location = new System.Drawing.Point(588, 143);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(319, 21);
            this.label11.TabIndex = 258;
            this.label11.Text = "Warehouse";
            // 
            // nudTaxRate
            // 
            this.nudTaxRate.BackColor = System.Drawing.Color.Transparent;
            this.nudTaxRate.BorderRadius = 6;
            this.nudTaxRate.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.nudTaxRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nudTaxRate.FocusedColor = System.Drawing.Color.Empty;
            this.nudTaxRate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudTaxRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.nudTaxRate.ItemHeight = 26;
            this.nudTaxRate.Location = new System.Drawing.Point(367, 219);
            this.nudTaxRate.Name = "nudTaxRate";
            this.nudTaxRate.Size = new System.Drawing.Size(169, 32);
            this.nudTaxRate.TabIndex = 259;
            // 
            // txtMedicineCost
            // 
            this.txtMedicineCost.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtMedicineCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.txtMedicineCost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMedicineCost.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtMedicineCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtMedicineCost.Location = new System.Drawing.Point(722, 78);
            this.txtMedicineCost.Name = "txtMedicineCost";
            this.txtMedicineCost.Size = new System.Drawing.Size(185, 29);
            this.txtMedicineCost.TabIndex = 261;
            this.txtMedicineCost.Text = "0";
            this.txtMedicineCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMedicineCost.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMedicineCost_KeyPress);
            this.txtMedicineCost.Validating += new System.ComponentModel.CancelEventHandler(this.txtMedicineCost_Validating);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            this.label8.Location = new System.Drawing.Point(588, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 21);
            this.label8.TabIndex = 260;
            this.label8.Text = "Medicine Cost:";
            // 
            // frmManageDrugs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(1358, 749);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmManageDrugs";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Drugs";
            this.panTitle.ResumeLayout(false);
            this.panTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReorderLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentStock)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgMedicines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lbCurrentUser;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtMedicinePrice;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMedicneName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.NumericUpDown nudReorderLevel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudCurrentStock;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgMedicines;
        private System.Windows.Forms.Label lbBlaceholder;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBatchNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dtpExpiryDate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private Guna.UI2.WinForms.Guna2ComboBox cmbWarehouse;
        private Guna.UI2.WinForms.Guna2ComboBox nudTaxRate;
        private System.Windows.Forms.TextBox txtMedicineCost;
        private System.Windows.Forms.Label label8;
    }
}