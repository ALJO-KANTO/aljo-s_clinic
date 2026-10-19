namespace YamyProject
{
    partial class MasterLoan
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle empAlt = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle empHeader = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle empRow = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle loanAlt = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle loanHeader = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle loanRow = new System.Windows.Forms.DataGridViewCellStyle();

            this.headerUC1 = new YamyProject.HeaderUC();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.panel3 = new System.Windows.Forms.Panel();
            this.pnlEmployeeTitle = new System.Windows.Forms.Panel();
            this.lblEmployeesTitle = new System.Windows.Forms.Label();
            this.dgvEmployee = new Guna.UI2.WinForms.Guna2DataGridView();

            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblInfoSubtitle = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCode = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCat = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblE = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblPh = new System.Windows.Forms.Label();
            this.btnExpand = new Guna.UI2.WinForms.Guna2Button();

            this.panel16 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dgvInvoice = new Guna.UI2.WinForms.Guna2DataGridView();

            // legacy fields retained for source compatibility
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.panel18 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();

            this.panel2.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnlEmployeeTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployee)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.pnlInfo.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel16.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoice)).BeginInit();
            this.SuspendLayout();

            // headerUC1
            this.headerUC1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerUC1.FormText = "";
            this.headerUC1.HeaderText = "Employee Loan Center";
            this.headerUC1.Location = new System.Drawing.Point(0, 0);
            this.headerUC1.Margin = new System.Windows.Forms.Padding(2);
            this.headerUC1.Name = "headerUC1";
            this.headerUC1.Size = new System.Drawing.Size(1400, 36);
            this.headerUC1.TabIndex = 0;

            // panel2 - action toolbar
            this.panel2.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.panel2.Controls.Add(this.menuStrip2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 36);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(26, 13, 0, 10);
            this.panel2.Size = new System.Drawing.Size(1400, 72);
            this.panel2.TabIndex = 1;

            // menuStrip2
            this.menuStrip2.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.toolStripMenuItem8,
                this.printToolStripMenuItem,
                this.excelToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(26, 13);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip2.Size = new System.Drawing.Size(370, 42);
            this.menuStrip2.TabIndex = 0;

            // Add New Loan
            this.toolStripMenuItem8.BackColor = System.Drawing.Color.FromArgb(102, 54, 218);
            this.toolStripMenuItem8.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem8.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Padding = new System.Windows.Forms.Padding(18, 0, 18, 0);
            this.toolStripMenuItem8.Size = new System.Drawing.Size(132, 42);
            this.toolStripMenuItem8.Text = "+  Add New Loan";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);

            // Print
            this.printToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.printToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.printToolStripMenuItem.Image = global::Clinic.Properties.Resources.printN;
            this.printToolStripMenuItem.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Padding = new System.Windows.Forms.Padding(14, 0, 14, 0);
            this.printToolStripMenuItem.Size = new System.Drawing.Size(94, 42);
            this.printToolStripMenuItem.Text = " Print  ▼";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);

            // Excel
            this.excelToolStripMenuItem.BackColor = System.Drawing.Color.White;
            this.excelToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.excelToolStripMenuItem.Image = global::Clinic.Properties.Resources.Microsoft_Excel;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Padding = new System.Windows.Forms.Padding(14, 0, 14, 0);
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(94, 42);
            this.excelToolStripMenuItem.Text = " Excel  ▼";
            this.excelToolStripMenuItem.Click += new System.EventHandler(this.excelToolStripMenuItem_Click);

            // panel3 - employee sidebar
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.dgvEmployee);
            this.panel3.Controls.Add(this.pnlEmployeeTitle);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 108);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(26, 0, 10, 26);
            this.panel3.Size = new System.Drawing.Size(390, 692);
            this.panel3.TabIndex = 2;

            // pnlEmployeeTitle
            this.pnlEmployeeTitle.BackColor = System.Drawing.Color.White;
            this.pnlEmployeeTitle.Controls.Add(this.lblEmployeesTitle);
            this.pnlEmployeeTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEmployeeTitle.Location = new System.Drawing.Point(26, 0);
            this.pnlEmployeeTitle.Name = "pnlEmployeeTitle";
            this.pnlEmployeeTitle.Size = new System.Drawing.Size(354, 62);
            this.pnlEmployeeTitle.TabIndex = 0;

            // lblEmployeesTitle
            this.lblEmployeesTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmployeesTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblEmployeesTitle.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblEmployeesTitle.Location = new System.Drawing.Point(0, 0);
            this.lblEmployeesTitle.Name = "lblEmployeesTitle";
            this.lblEmployeesTitle.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblEmployeesTitle.Size = new System.Drawing.Size(354, 62);
            this.lblEmployeesTitle.TabIndex = 0;
            this.lblEmployeesTitle.Text = "Employees";
            this.lblEmployeesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // dgvEmployee
            this.dgvEmployee.AllowUserToAddRows = false;
            this.dgvEmployee.AllowUserToDeleteRows = false;
            this.dgvEmployee.AllowUserToResizeRows = false;
            empAlt.BackColor = System.Drawing.Color.FromArgb(249, 250, 253);
            empAlt.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            empAlt.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            empAlt.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvEmployee.AlternatingRowsDefaultCellStyle = empAlt;
            this.dgvEmployee.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmployee.BackgroundColor = System.Drawing.Color.White;
            this.dgvEmployee.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEmployee.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvEmployee.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            empHeader.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            empHeader.BackColor = System.Drawing.Color.FromArgb(246, 243, 255);
            empHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            empHeader.ForeColor = System.Drawing.Color.FromArgb(82, 49, 184);
            empHeader.SelectionBackColor = System.Drawing.Color.FromArgb(246, 243, 255);
            empHeader.SelectionForeColor = System.Drawing.Color.FromArgb(82, 49, 184);
            empHeader.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployee.ColumnHeadersDefaultCellStyle = empHeader;
            this.dgvEmployee.ColumnHeadersHeight = 40;
            this.dgvEmployee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            empRow.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            empRow.BackColor = System.Drawing.Color.White;
            empRow.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            empRow.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            empRow.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            empRow.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            empRow.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvEmployee.DefaultCellStyle = empRow;
            this.dgvEmployee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployee.GridColor = System.Drawing.Color.FromArgb(232, 234, 244);
            this.dgvEmployee.Location = new System.Drawing.Point(26, 62);
            this.dgvEmployee.MultiSelect = false;
            this.dgvEmployee.Name = "dgvEmployee";
            this.dgvEmployee.ReadOnly = true;
            this.dgvEmployee.RowHeadersVisible = false;
            this.dgvEmployee.RowTemplate.Height = 40;
            this.dgvEmployee.Size = new System.Drawing.Size(354, 604);
            this.dgvEmployee.TabIndex = 1;
            this.dgvEmployee.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(249, 250, 253);
            this.dgvEmployee.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvEmployee.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(232, 234, 244);
            this.dgvEmployee.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(246, 243, 255);
            this.dgvEmployee.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvEmployee.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvEmployee.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.FromArgb(82, 49, 184);
            this.dgvEmployee.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmployee.ThemeStyle.HeaderStyle.Height = 40;
            this.dgvEmployee.ThemeStyle.ReadOnly = true;
            this.dgvEmployee.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvEmployee.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvEmployee.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dgvEmployee.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvEmployee.ThemeStyle.RowsStyle.Height = 40;
            this.dgvEmployee.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            this.dgvEmployee.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvEmployee.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmployee_CellClick);

            // pnlMain
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlMain.Controls.Add(this.panel4);
            this.pnlMain.Controls.Add(this.panel16);
            this.pnlMain.Controls.Add(this.pnlInfo);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(390, 108);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(14, 0, 26, 26);
            this.pnlMain.Size = new System.Drawing.Size(1010, 692);
            this.pnlMain.TabIndex = 3;

            // pnlInfo
            this.pnlInfo.BackColor = System.Drawing.Color.White;
            this.pnlInfo.Controls.Add(this.btnExpand);
            this.pnlInfo.Controls.Add(this.lblPh);
            this.pnlInfo.Controls.Add(this.label4);
            this.pnlInfo.Controls.Add(this.lblE);
            this.pnlInfo.Controls.Add(this.label12);
            this.pnlInfo.Controls.Add(this.lblAddress);
            this.pnlInfo.Controls.Add(this.label5);
            this.pnlInfo.Controls.Add(this.lblCat);
            this.pnlInfo.Controls.Add(this.label1);
            this.pnlInfo.Controls.Add(this.lblName);
            this.pnlInfo.Controls.Add(this.label8);
            this.pnlInfo.Controls.Add(this.lblCode);
            this.pnlInfo.Controls.Add(this.label2);
            this.pnlInfo.Controls.Add(this.lblInfoSubtitle);
            this.pnlInfo.Controls.Add(this.panel11);
            this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlInfo.Location = new System.Drawing.Point(14, 0);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(970, 80);
            this.pnlInfo.TabIndex = 0;

            // panel11
            this.panel11.BackColor = System.Drawing.Color.White;
            this.panel11.Controls.Add(this.label3);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(970, 48);
            this.panel11.TabIndex = 0;

            // label3
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(22, 5, 0, 0);
            this.label3.Size = new System.Drawing.Size(970, 48);
            this.label3.TabIndex = 0;
            this.label3.Text = "Employee Information";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // subtitle
            this.lblInfoSubtitle.AutoSize = true;
            this.lblInfoSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblInfoSubtitle.ForeColor = System.Drawing.Color.FromArgb(102, 111, 148);
            this.lblInfoSubtitle.Location = new System.Drawing.Point(25, 50);
            this.lblInfoSubtitle.Name = "lblInfoSubtitle";
            this.lblInfoSubtitle.Size = new System.Drawing.Size(305, 17);
            this.lblInfoSubtitle.Text = "View employee details and manage loan records.";

            // detail labels - visible when pnlInfo expands to 240 from existing logic
            this.label2.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(92, 101, 137);
            this.label2.Location = new System.Drawing.Point(25, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 44);
            this.label2.Text = "Code";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblCode.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.lblCode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCode.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblCode.Location = new System.Drawing.Point(115, 88);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(335, 44);
            this.lblCode.Text = "-";
            this.lblCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.label8.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(92, 101, 137);
            this.label8.Location = new System.Drawing.Point(475, 88);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 44);
            this.label8.Text = "Name";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblName.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblName.Location = new System.Drawing.Point(565, 88);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(380, 44);
            this.lblName.Text = "-";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.label1.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(92, 101, 137);
            this.label1.Location = new System.Drawing.Point(25, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 44);
            this.label1.Text = "Position";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblCat.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.lblCat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCat.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblCat.Location = new System.Drawing.Point(115, 140);
            this.lblCat.Name = "lblCat";
            this.lblCat.Size = new System.Drawing.Size(335, 44);
            this.lblCat.Text = "-";
            this.lblCat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.label5.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(92, 101, 137);
            this.label5.Location = new System.Drawing.Point(475, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 44);
            this.label5.Text = "Address";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblAddress.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAddress.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblAddress.Location = new System.Drawing.Point(565, 140);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(380, 44);
            this.lblAddress.Text = "-";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.label12.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label12.ForeColor = System.Drawing.Color.FromArgb(92, 101, 137);
            this.label12.Location = new System.Drawing.Point(25, 192);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(90, 38);
            this.label12.Text = "Email";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblE.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.lblE.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblE.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblE.Location = new System.Drawing.Point(115, 192);
            this.lblE.Name = "lblE";
            this.lblE.Size = new System.Drawing.Size(335, 38);
            this.lblE.Text = "-";
            this.lblE.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.label4.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(92, 101, 137);
            this.label4.Location = new System.Drawing.Point(475, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 38);
            this.label4.Text = "Phone";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblPh.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.lblPh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPh.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblPh.Location = new System.Drawing.Point(565, 192);
            this.lblPh.Name = "lblPh";
            this.lblPh.Size = new System.Drawing.Size(250, 38);
            this.lblPh.Text = "-";
            this.lblPh.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // btnExpand
            this.btnExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExpand.BorderColor = System.Drawing.Color.FromArgb(105, 63, 218);
            this.btnExpand.BorderRadius = 8;
            this.btnExpand.BorderThickness = 1;
            this.btnExpand.FillColor = System.Drawing.Color.White;
            this.btnExpand.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExpand.ForeColor = System.Drawing.Color.FromArgb(94, 54, 201);
            this.btnExpand.Location = new System.Drawing.Point(840, 42);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(105, 30);
            this.btnExpand.TabIndex = 16;
            this.btnExpand.Text = "More";
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);

            // panel16 - Loan Info title
            this.panel16.BackColor = System.Drawing.Color.FromArgb(80, 43, 177);
            this.panel16.Controls.Add(this.label10);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel16.Location = new System.Drawing.Point(14, 80);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(970, 48);
            this.panel16.TabIndex = 1;

            // label10
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label10.Size = new System.Drawing.Size(970, 48);
            this.label10.Text = "Loan Info";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // panel4
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.dgvInvoice);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(14, 128);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.panel4.Size = new System.Drawing.Size(970, 538);
            this.panel4.TabIndex = 2;

            // dgvInvoice
            this.dgvInvoice.AllowUserToAddRows = false;
            this.dgvInvoice.AllowUserToDeleteRows = false;
            this.dgvInvoice.AllowUserToResizeRows = false;
            this.dgvInvoice.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            loanAlt.BackColor = System.Drawing.Color.FromArgb(252, 252, 254);
            loanAlt.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            loanAlt.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            loanAlt.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvInvoice.AlternatingRowsDefaultCellStyle = loanAlt;
            this.dgvInvoice.BackgroundColor = System.Drawing.Color.White;
            this.dgvInvoice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvInvoice.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvInvoice.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            loanHeader.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            loanHeader.BackColor = System.Drawing.Color.FromArgb(246, 243, 255);
            loanHeader.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            loanHeader.ForeColor = System.Drawing.Color.FromArgb(76, 42, 190);
            loanHeader.SelectionBackColor = System.Drawing.Color.FromArgb(246, 243, 255);
            loanHeader.SelectionForeColor = System.Drawing.Color.FromArgb(76, 42, 190);
            loanHeader.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInvoice.ColumnHeadersDefaultCellStyle = loanHeader;
            this.dgvInvoice.ColumnHeadersHeight = 46;
            this.dgvInvoice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            loanRow.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            loanRow.BackColor = System.Drawing.Color.White;
            loanRow.Font = new System.Drawing.Font("Segoe UI", 9F);
            loanRow.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            loanRow.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            loanRow.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            loanRow.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvInvoice.DefaultCellStyle = loanRow;
            this.dgvInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoice.GridColor = System.Drawing.Color.FromArgb(232, 234, 244);
            this.dgvInvoice.Location = new System.Drawing.Point(0, 0);
            this.dgvInvoice.MultiSelect = false;
            this.dgvInvoice.Name = "dgvInvoice";
            this.dgvInvoice.ReadOnly = true;
            this.dgvInvoice.RowHeadersVisible = false;
            this.dgvInvoice.RowTemplate.Height = 38;
            this.dgvInvoice.Size = new System.Drawing.Size(970, 538);
            this.dgvInvoice.TabIndex = 7;
            this.dgvInvoice.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(252, 252, 254);
            this.dgvInvoice.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvInvoice.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(232, 234, 244);
            this.dgvInvoice.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(246, 243, 255);
            this.dgvInvoice.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvInvoice.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.dgvInvoice.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.FromArgb(76, 42, 190);
            this.dgvInvoice.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvInvoice.ThemeStyle.HeaderStyle.Height = 46;
            this.dgvInvoice.ThemeStyle.ReadOnly = true;
            this.dgvInvoice.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvInvoice.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvInvoice.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvInvoice.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvInvoice.ThemeStyle.RowsStyle.Height = 38;
            this.dgvInvoice.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            this.dgvInvoice.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvInvoice.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvInvoice_CellDoubleClick);
            this.dgvInvoice.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvInvoice_CellFormatting);

            // legacy panels not used visually
            this.panel6.Size = new System.Drawing.Size(0, 0);
            this.panel7.Size = new System.Drawing.Size(0, 0);
            this.panel5.Size = new System.Drawing.Size(0, 0);
            this.panel1.Size = new System.Drawing.Size(0, 0);
            this.panel12.Size = new System.Drawing.Size(0, 0);
            this.panel15.Size = new System.Drawing.Size(0, 0);
            this.panel13.Size = new System.Drawing.Size(0, 0);
            this.panel14.Size = new System.Drawing.Size(0, 0);
            this.panel17.Size = new System.Drawing.Size(0, 0);
            this.panel18.Size = new System.Drawing.Size(0, 0);
            this.panel9.Size = new System.Drawing.Size(0, 0);
            this.panel10.Size = new System.Drawing.Size(0, 0);

            // MasterLoan
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.headerUC1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(1100, 650);
            this.Name = "MasterLoan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "main";
            this.Text = "Employee Loan Center";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MasterLoan_FormClosing);
            this.Load += new System.EventHandler(this.MasterLoan_Load);

            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.pnlEmployeeTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployee)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel16.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoice)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private HeaderUC headerUC1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlEmployeeTitle;
        private System.Windows.Forms.Label lblEmployeesTitle;
        private Guna.UI2.WinForms.Guna2DataGridView dgvEmployee;

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblInfoSubtitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblE;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblPh;
        private Guna.UI2.WinForms.Guna2Button btnExpand;

        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel4;
        private Guna.UI2.WinForms.Guna2DataGridView dgvInvoice;

        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Panel panel17;
        private System.Windows.Forms.Panel panel18;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
    }
}
