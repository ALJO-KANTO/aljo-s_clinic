using System.Drawing;

namespace Clinic.Medical_Services.Casher
{
    partial class frmManageBills
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panTitle = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.lbCurrentUser = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panCardPending = new System.Windows.Forms.Panel();
            this.lblCountPending = new System.Windows.Forms.Label();
            this.lblTitlePending = new System.Windows.Forms.Label();
            this.panCardPartial = new System.Windows.Forms.Panel();
            this.lblCountPartial = new System.Windows.Forms.Label();
            this.lblTitlePartial = new System.Windows.Forms.Label();
            this.panCardPaid = new System.Windows.Forms.Panel();
            this.lblCountPaid = new System.Windows.Forms.Label();
            this.lblTitlePaid = new System.Windows.Forms.Label();
            this.dgMasterBillList = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chePaid = new System.Windows.Forms.CheckBox();
            this.chePartial = new System.Windows.Forms.CheckBox();
            this.chePending = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkAllDates = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSearchByPatientName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panCardPending.SuspendLayout();
            this.panCardPartial.SuspendLayout();
            this.panCardPaid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMasterBillList)).BeginInit();
            this.panel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panTitle
            // 
            this.panTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.panTitle.Controls.Add(this.label5);
            this.panTitle.Controls.Add(this.pictureBox4);
            this.panTitle.Controls.Add(this.btnExit);
            this.panTitle.Controls.Add(this.lbCurrentUser);
            this.panTitle.Controls.Add(this.lblTitle);
            this.panTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTitle.Location = new System.Drawing.Point(20, 20);
            this.panTitle.Name = "panTitle";
            this.panTitle.Size = new System.Drawing.Size(1504, 89);
            this.panTitle.TabIndex = 253;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.label5.Location = new System.Drawing.Point(1029, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(137, 28);
            this.label5.TabIndex = 240;
            this.label5.Text = "Current User:";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.Image = global::Clinic.Properties.Resources.Bill_64;
            this.pictureBox4.Location = new System.Drawing.Point(401, 9);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(70, 59);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 255;
            this.pictureBox4.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnExit.Image = global::Clinic.Properties.Resources.sign_out_32__2;
            this.btnExit.Location = new System.Drawing.Point(1315, 28);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 42);
            this.btnExit.TabIndex = 254;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lbCurrentUser
            // 
            this.lbCurrentUser.AutoSize = true;
            this.lbCurrentUser.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lbCurrentUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            this.lbCurrentUser.Location = new System.Drawing.Point(1029, 50);
            this.lbCurrentUser.Name = "lbCurrentUser";
            this.lbCurrentUser.Size = new System.Drawing.Size(53, 28);
            this.lbCurrentUser.TabIndex = 241;
            this.lbCurrentUser.Text = "[???]";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(503, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(344, 46);
            this.lblTitle.TabIndex = 236;
            this.lblTitle.Text = "BILL MANAGEMENT";
            // 
            // panCardPending
            // 
            this.panCardPending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.panCardPending.Controls.Add(this.lblCountPending);
            this.panCardPending.Controls.Add(this.lblTitlePending);
            this.panCardPending.Location = new System.Drawing.Point(421, 125);
            this.panCardPending.Name = "panCardPending";
            this.panCardPending.Size = new System.Drawing.Size(230, 80);
            this.panCardPending.TabIndex = 400;
            // 
            // lblCountPending
            // 
            this.lblCountPending.AutoSize = true;
            this.lblCountPending.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCountPending.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            this.lblCountPending.Location = new System.Drawing.Point(15, 5);
            this.lblCountPending.Name = "lblCountPending";
            this.lblCountPending.Size = new System.Drawing.Size(43, 50);
            this.lblCountPending.TabIndex = 0;
            this.lblCountPending.Text = "0";
            // 
            // lblTitlePending
            // 
            this.lblTitlePending.AutoSize = true;
            this.lblTitlePending.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTitlePending.ForeColor = System.Drawing.Color.DarkGray;
            this.lblTitlePending.Location = new System.Drawing.Point(18, 50);
            this.lblTitlePending.Name = "lblTitlePending";
            this.lblTitlePending.Size = new System.Drawing.Size(106, 23);
            this.lblTitlePending.TabIndex = 1;
            this.lblTitlePending.Text = "Pending Bills";
            // 
            // panCardPartial
            // 
            this.panCardPartial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.panCardPartial.Controls.Add(this.lblCountPartial);
            this.panCardPartial.Controls.Add(this.lblTitlePartial);
            this.panCardPartial.Location = new System.Drawing.Point(665, 125);
            this.panCardPartial.Name = "panCardPartial";
            this.panCardPartial.Size = new System.Drawing.Size(230, 80);
            this.panCardPartial.TabIndex = 401;
            // 
            // lblCountPartial
            // 
            this.lblCountPartial.AutoSize = true;
            this.lblCountPartial.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCountPartial.ForeColor = System.Drawing.Color.Orange;
            this.lblCountPartial.Location = new System.Drawing.Point(15, 5);
            this.lblCountPartial.Name = "lblCountPartial";
            this.lblCountPartial.Size = new System.Drawing.Size(43, 50);
            this.lblCountPartial.TabIndex = 0;
            this.lblCountPartial.Text = "0";
            // 
            // lblTitlePartial
            // 
            this.lblTitlePartial.AutoSize = true;
            this.lblTitlePartial.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTitlePartial.ForeColor = System.Drawing.Color.DarkGray;
            this.lblTitlePartial.Location = new System.Drawing.Point(18, 50);
            this.lblTitlePartial.Name = "lblTitlePartial";
            this.lblTitlePartial.Size = new System.Drawing.Size(135, 23);
            this.lblTitlePartial.TabIndex = 1;
            this.lblTitlePartial.Text = "Partial Payments";
            // 
            // panCardPaid
            // 
            this.panCardPaid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.panCardPaid.Controls.Add(this.lblCountPaid);
            this.panCardPaid.Controls.Add(this.lblTitlePaid);
            this.panCardPaid.Location = new System.Drawing.Point(910, 125);
            this.panCardPaid.Name = "panCardPaid";
            this.panCardPaid.Size = new System.Drawing.Size(230, 80);
            this.panCardPaid.TabIndex = 402;
            // 
            // lblCountPaid
            // 
            this.lblCountPaid.AutoSize = true;
            this.lblCountPaid.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCountPaid.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblCountPaid.Location = new System.Drawing.Point(15, 5);
            this.lblCountPaid.Name = "lblCountPaid";
            this.lblCountPaid.Size = new System.Drawing.Size(43, 50);
            this.lblCountPaid.TabIndex = 0;
            this.lblCountPaid.Text = "0";
            // 
            // lblTitlePaid
            // 
            this.lblTitlePaid.AutoSize = true;
            this.lblTitlePaid.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTitlePaid.ForeColor = System.Drawing.Color.DarkGray;
            this.lblTitlePaid.Location = new System.Drawing.Point(18, 50);
            this.lblTitlePaid.Name = "lblTitlePaid";
            this.lblTitlePaid.Size = new System.Drawing.Size(140, 23);
            this.lblTitlePaid.TabIndex = 1;
            this.lblTitlePaid.Text = "Fully Paid (Today)";
            // 
            // dgMasterBillList
            // 
            this.dgMasterBillList.AllowUserToAddRows = false;
            this.dgMasterBillList.AllowUserToDeleteRows = false;
            this.dgMasterBillList.AllowUserToResizeRows = false;
            this.dgMasterBillList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgMasterBillList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.dgMasterBillList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgMasterBillList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgMasterBillList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgMasterBillList.ColumnHeadersHeight = 45;
            this.dgMasterBillList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgMasterBillList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgMasterBillList.EnableHeadersVisualStyles = false;
            this.dgMasterBillList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.dgMasterBillList.Location = new System.Drawing.Point(421, 220);
            this.dgMasterBillList.MultiSelect = false;
            this.dgMasterBillList.Name = "dgMasterBillList";
            this.dgMasterBillList.ReadOnly = true;
            this.dgMasterBillList.RowHeadersVisible = false;
            this.dgMasterBillList.RowHeadersWidth = 51;
            this.dgMasterBillList.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgMasterBillList.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.dgMasterBillList.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dgMasterBillList.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgMasterBillList.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(90)))));
            this.dgMasterBillList.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgMasterBillList.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgMasterBillList.RowTemplate.Height = 40;
            this.dgMasterBillList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgMasterBillList.Size = new System.Drawing.Size(1100, 580);
            this.dgMasterBillList.TabIndex = 255;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.panel2.Location = new System.Drawing.Point(405, 129);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(3, 680);
            this.panel2.TabIndex = 408;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Segoe UI", 12F);
            this.dateTimePicker1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(30, 235);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(328, 34);
            this.dateTimePicker1.TabIndex = 407;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.panel3.Controls.Add(this.chePaid);
            this.panel3.Controls.Add(this.chePartial);
            this.panel3.Controls.Add(this.chePending);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(30, 295);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(328, 230);
            this.panel3.TabIndex = 406;
            // 
            // chePaid
            // 
            this.chePaid.Appearance = System.Windows.Forms.Appearance.Button;
            this.chePaid.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(90)))));
            this.chePaid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chePaid.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.chePaid.ForeColor = System.Drawing.Color.White;
            this.chePaid.Location = new System.Drawing.Point(20, 165);
            this.chePaid.Name = "chePaid";
            this.chePaid.Size = new System.Drawing.Size(288, 40);
            this.chePaid.TabIndex = 0;
            this.chePaid.Text = "✅  Fully Paid";
            this.chePaid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chePaid.Click += new System.EventHandler(this.checkedBox_CheckedChanged);
            // 
            // chePartial
            // 
            this.chePartial.Appearance = System.Windows.Forms.Appearance.Button;
            this.chePartial.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(90)))));
            this.chePartial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chePartial.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.chePartial.ForeColor = System.Drawing.Color.White;
            this.chePartial.Location = new System.Drawing.Point(20, 115);
            this.chePartial.Name = "chePartial";
            this.chePartial.Size = new System.Drawing.Size(288, 40);
            this.chePartial.TabIndex = 1;
            this.chePartial.Text = "🔸  Partial Payments";
            this.chePartial.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chePartial.CheckedChanged += new System.EventHandler(this.checkedBox_CheckedChanged);
            // 
            // chePending
            // 
            this.chePending.Appearance = System.Windows.Forms.Appearance.Button;
            this.chePending.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(90)))));
            this.chePending.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chePending.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.chePending.ForeColor = System.Drawing.Color.White;
            this.chePending.Location = new System.Drawing.Point(20, 65);
            this.chePending.Name = "chePending";
            this.chePending.Size = new System.Drawing.Size(288, 40);
            this.chePending.TabIndex = 2;
            this.chePending.Text = "⏳  Pending Bills";
            this.chePending.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chePending.CheckedChanged += new System.EventHandler(this.checkedBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 30);
            this.label1.TabIndex = 3;
            this.label1.Text = "Bill Status";
            // 
            // chkAllDates
            // 
            this.chkAllDates.AutoSize = true;
            this.chkAllDates.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.chkAllDates.ForeColor = System.Drawing.Color.DarkGray;
            this.chkAllDates.Location = new System.Drawing.Point(250, 200);
            this.chkAllDates.Name = "chkAllDates";
            this.chkAllDates.Size = new System.Drawing.Size(104, 27);
            this.chkAllDates.TabIndex = 0;
            this.chkAllDates.Text = "All Dates";
            this.chkAllDates.CheckedChanged += new System.EventHandler(this.chkAllDates_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.DarkGray;
            this.label4.Location = new System.Drawing.Point(27, 550);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(201, 28);
            this.label4.TabIndex = 405;
            this.label4.Text = "Search Patient Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.DarkGray;
            this.label6.Location = new System.Drawing.Point(27, 200);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 28);
            this.label6.TabIndex = 404;
            this.label6.Text = "Filter by Date";
            // 
            // txtSearchByPatientName
            // 
            this.txtSearchByPatientName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.txtSearchByPatientName.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.txtSearchByPatientName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            this.txtSearchByPatientName.Location = new System.Drawing.Point(30, 585);
            this.txtSearchByPatientName.Name = "txtSearchByPatientName";
            this.txtSearchByPatientName.Size = new System.Drawing.Size(328, 36);
            this.txtSearchByPatientName.TabIndex = 403;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "🎯 Search & Filters";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(20, 132);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(368, 50);
            this.flowLayoutPanel1.TabIndex = 257;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(20, 109);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1504, 17);
            this.panel1.TabIndex = 409;
            // 
            // frmManageBills
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(1544, 829);
            this.Controls.Add(this.chkAllDates);
            this.Controls.Add(this.panCardPaid);
            this.Controls.Add(this.panCardPartial);
            this.Controls.Add(this.panCardPending);
            this.Controls.Add(this.txtSearchByPatientName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dgMasterBillList);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmManageBills";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmManageBills_Load);
            this.panTitle.ResumeLayout(false);
            this.panTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panCardPending.ResumeLayout(false);
            this.panCardPending.PerformLayout();
            this.panCardPartial.ResumeLayout(false);
            this.panCardPartial.PerformLayout();
            this.panCardPaid.ResumeLayout(false);
            this.panCardPaid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgMasterBillList)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lbCurrentUser;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgMasterBillList; // تم تعديل الاسم ليعبر عن الفواتير الشاملة
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox chePending;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chePaid;
        private System.Windows.Forms.CheckBox chePartial;
        private System.Windows.Forms.CheckBox chkAllDates;

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSearchByPatientName;
        private System.Windows.Forms.PictureBox pictureBox4;

        // كائنات العدادات العلوية المضافة
        private System.Windows.Forms.Panel panCardPending;
        private System.Windows.Forms.Label lblCountPending;
        private System.Windows.Forms.Label lblTitlePending;
        private System.Windows.Forms.Panel panCardPartial;
        private System.Windows.Forms.Label lblCountPartial;
        private System.Windows.Forms.Label lblTitlePartial;
        private System.Windows.Forms.Panel panCardPaid;
        private System.Windows.Forms.Label lblCountPaid;
        private System.Windows.Forms.Label lblTitlePaid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}