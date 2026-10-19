namespace YamyProject
{
    partial class frmSalarySheet
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle rowStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle altStyle = new System.Windows.Forms.DataGridViewCellStyle();

            this.headerUC1 = new YamyProject.HeaderUC();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);

            this.pnlHero = new System.Windows.Forms.Panel();
            this.pnlTitleIcon = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitleIcon = new System.Windows.Forms.Label();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.lblPageSubTitle = new System.Windows.Forms.Label();
            this.pnlMetaCard = new Guna.UI2.WinForms.Guna2Panel();

            this.lblDateIcon = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblRefIcon = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblREF = new System.Windows.Forms.Label();
            this.lblUserIcon = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblSSNo = new System.Windows.Forms.Label();

            this.pnlFilterOuter = new System.Windows.Forms.Panel();
            this.pnlFilterCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lblMonthIcon = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMonth = new System.Windows.Forms.ComboBox();
            this.lblYearIcon = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbYear = new System.Windows.Forms.ComboBox();

            this.pnlSummary = new System.Windows.Forms.Panel();
            this.cardEmployees = new Guna.UI2.WinForms.Guna2Panel();
            this.cardBasic = new Guna.UI2.WinForms.Guna2Panel();
            this.cardDeduction = new Guna.UI2.WinForms.Guna2Panel();
            this.cardNet = new Guna.UI2.WinForms.Guna2Panel();
            this.lblEmployeesIcon = new System.Windows.Forms.Label();
            this.lblEmployeesTitle = new System.Windows.Forms.Label();
            this.lblEmployeesValue = new System.Windows.Forms.Label();
            this.lblBasicIcon = new System.Windows.Forms.Label();
            this.lblBasicTitle = new System.Windows.Forms.Label();
            this.lblBasicValue = new System.Windows.Forms.Label();
            this.lblDeductionIcon = new System.Windows.Forms.Label();
            this.lblDeductionTitle = new System.Windows.Forms.Label();
            this.lblDeductionValue = new System.Windows.Forms.Label();
            this.lblNetIcon = new System.Windows.Forms.Label();
            this.lblNetTitle = new System.Windows.Forms.Label();
            this.lblNetValue = new System.Windows.Forms.Label();

            this.pnlGridOuter = new System.Windows.Forms.Panel();
            this.pnlGridCard = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlGridTitle = new System.Windows.Forms.Panel();
            this.lblGridIcon = new System.Windows.Forms.Label();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.txtSearchVisual = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel4 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel5 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel6 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel7 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel8 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel1 = new System.Windows.Forms.Label();
            this.guna2HtmlLabel2 = new System.Windows.Forms.Label();
            this.guna2HtmlLabel3 = new System.Windows.Forms.Label();
            this.guna2HtmlLabel4 = new System.Windows.Forms.Label();

            this.dgvSalarySheet = new Guna.UI2.WinForms.Guna2DataGridView();
            this.Sn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EmployeeID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EmployeeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbsentDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BasicSalary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Allowance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Other = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Additions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAdditions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EmployeeAdvance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkAbsence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Salik = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.othersDeductions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalDeductions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NetSalary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cash = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WPS = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblFooterInfo = new System.Windows.Forms.Label();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();

            // legacy designer fields retained for compatibility
            this.panel2 = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column23 = new System.Windows.Forms.DataGridViewTextBoxColumn();

            ((System.ComponentModel.ISupportInitialize)(this.dgvSalarySheet)).BeginInit();
            this.SuspendLayout();

            // headerUC1
            this.headerUC1.BackColor = System.Drawing.Color.FromArgb(87, 39, 190);
            this.headerUC1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerUC1.FormText = "";
            this.headerUC1.HeaderText = "Salary Sheet";
            this.headerUC1.Location = new System.Drawing.Point(0, 0);
            this.headerUC1.Name = "headerUC1";
            this.headerUC1.Size = new System.Drawing.Size(1386, 34);
            this.headerUC1.TabIndex = 0;

            this.guna2DragControl1.TargetControl = this.headerUC1;
            this.guna2DragControl1.UseTransparentDrag = true;

            // pnlHero
            this.pnlHero.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlHero.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHero.Height = 105;
            this.pnlHero.Padding = new System.Windows.Forms.Padding(28, 18, 28, 12);

            // title icon
            this.pnlTitleIcon.BackColor = System.Drawing.Color.FromArgb(239, 233, 255);
            this.pnlTitleIcon.BorderRadius = 12;
            this.pnlTitleIcon.Location = new System.Drawing.Point(34, 18);
            this.pnlTitleIcon.Size = new System.Drawing.Size(58, 58);
            this.lblTitleIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitleIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 23F);
            this.lblTitleIcon.ForeColor = System.Drawing.Color.FromArgb(105, 65, 198);
            this.lblTitleIcon.Text = "";
            this.lblTitleIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.pnlTitleIcon.Controls.Add(this.lblTitleIcon);

            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82);
            this.lblPageTitle.Location = new System.Drawing.Point(108, 18);
            this.lblPageTitle.Text = "Salary Sheet";

            this.lblPageSubTitle.AutoSize = true;
            this.lblPageSubTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPageSubTitle.ForeColor = System.Drawing.Color.FromArgb(105, 115, 145);
            this.lblPageSubTitle.Location = new System.Drawing.Point(111, 62);
            this.lblPageSubTitle.Text = "View and manage employee salary details.";

            // meta card
            this.pnlMetaCard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.pnlMetaCard.BackColor = System.Drawing.Color.White;
            this.pnlMetaCard.BorderColor = System.Drawing.Color.FromArgb(230, 233, 242);
            this.pnlMetaCard.BorderRadius = 12;
            this.pnlMetaCard.BorderThickness = 1;
            this.pnlMetaCard.Location = new System.Drawing.Point(825, 15);
            this.pnlMetaCard.Size = new System.Drawing.Size(525, 70);

            this.lblDateIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 16F);
            this.lblDateIcon.ForeColor = System.Drawing.Color.FromArgb(91, 46, 205);
            this.lblDateIcon.Location = new System.Drawing.Point(16, 21);
            this.lblDateIcon.Size = new System.Drawing.Size(28, 28);
            this.lblDateIcon.Text = "";
            this.lblDateIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.AutoSize = true; this.label1.Font = new System.Drawing.Font("Segoe UI", 8F); this.label1.ForeColor = System.Drawing.Color.FromArgb(90, 100, 125); this.label1.Location = new System.Drawing.Point(48, 12); this.label1.Text = "Date";
            this.lblDate.AutoSize = true; this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblDate.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82); this.lblDate.Location = new System.Drawing.Point(48, 34); this.lblDate.Text = "-";

            this.lblRefIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 16F);
            this.lblRefIcon.ForeColor = System.Drawing.Color.FromArgb(30, 64, 120);
            this.lblRefIcon.Location = new System.Drawing.Point(180, 21);
            this.lblRefIcon.Size = new System.Drawing.Size(28, 28);
            this.lblRefIcon.Text = "";
            this.lblRefIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.AutoSize = true; this.label2.Font = new System.Drawing.Font("Segoe UI", 8F); this.label2.ForeColor = System.Drawing.Color.FromArgb(90, 100, 125); this.label2.Location = new System.Drawing.Point(212, 12); this.label2.Text = "REF";
            this.lblREF.AutoSize = true; this.lblREF.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblREF.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82); this.lblREF.Location = new System.Drawing.Point(212, 34); this.lblREF.Text = "-";

            this.lblUserIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 17F);
            this.lblUserIcon.ForeColor = System.Drawing.Color.FromArgb(105, 65, 198);
            this.lblUserIcon.Location = new System.Drawing.Point(355, 20);
            this.lblUserIcon.Size = new System.Drawing.Size(30, 30);
            this.lblUserIcon.Text = "";
            this.lblUserIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.AutoSize = true; this.label5.Font = new System.Drawing.Font("Segoe UI", 8F); this.label5.ForeColor = System.Drawing.Color.FromArgb(90, 100, 125); this.label5.Location = new System.Drawing.Point(390, 9); this.label5.Text = "User";
            this.lblUser.AutoSize = true; this.lblUser.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblUser.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82); this.lblUser.Location = new System.Drawing.Point(430, 9); this.lblUser.Text = "-";
            this.label7.AutoSize = true; this.label7.Font = new System.Drawing.Font("Segoe UI", 8F); this.label7.ForeColor = System.Drawing.Color.FromArgb(90, 100, 125); this.label7.Location = new System.Drawing.Point(390, 37); this.label7.Text = "S.S No";
            this.lblSSNo.AutoSize = true; this.lblSSNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblSSNo.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82); this.lblSSNo.Location = new System.Drawing.Point(440, 37); this.lblSSNo.Text = "-";

            this.pnlMetaCard.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblDateIcon, this.label1, this.lblDate, this.lblRefIcon, this.label2, this.lblREF, this.lblUserIcon, this.label5, this.lblUser, this.label7, this.lblSSNo });
            this.pnlHero.Controls.AddRange(new System.Windows.Forms.Control[] { this.pnlTitleIcon, this.lblPageTitle, this.lblPageSubTitle, this.pnlMetaCard });

            // Filter
            this.pnlFilterOuter.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlFilterOuter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilterOuter.Height = 78;
            this.pnlFilterOuter.Padding = new System.Windows.Forms.Padding(28, 4, 28, 10);

            this.pnlFilterCard.BackColor = System.Drawing.Color.White;
            this.pnlFilterCard.BorderColor = System.Drawing.Color.FromArgb(230, 233, 242);
            this.pnlFilterCard.BorderRadius = 12;
            this.pnlFilterCard.BorderThickness = 1;
            this.pnlFilterCard.Dock = System.Windows.Forms.DockStyle.Fill;

            this.lblMonthIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 15F);
            this.lblMonthIcon.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82);
            this.lblMonthIcon.Location = new System.Drawing.Point(28, 19);
            this.lblMonthIcon.Size = new System.Drawing.Size(28, 28);
            this.lblMonthIcon.Text = "";
            this.lblMonthIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.AutoSize = true; this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.label3.ForeColor = System.Drawing.Color.FromArgb(35, 48, 75); this.label3.Location = new System.Drawing.Point(62, 23); this.label3.Text = "Month";

            this.cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonth.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbMonth.Location = new System.Drawing.Point(120, 18);
            this.cmbMonth.Size = new System.Drawing.Size(200, 25);
            this.cmbMonth.SelectedIndexChanged += new System.EventHandler(this.cmbMonth_SelectedIndexChanged);

            this.lblYearIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 15F);
            this.lblYearIcon.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82);
            this.lblYearIcon.Location = new System.Drawing.Point(365, 19);
            this.lblYearIcon.Size = new System.Drawing.Size(28, 28);
            this.lblYearIcon.Text = "";
            this.lblYearIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.AutoSize = true; this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.label4.ForeColor = System.Drawing.Color.FromArgb(35, 48, 75); this.label4.Location = new System.Drawing.Point(399, 23); this.label4.Text = "Year";

            this.cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYear.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbYear.Location = new System.Drawing.Point(445, 18);
            this.cmbYear.Size = new System.Drawing.Size(145, 25);
            this.cmbYear.SelectedIndexChanged += new System.EventHandler(this.cmbYear_SelectedIndexChanged);

            this.pnlFilterCard.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblMonthIcon, this.label3, this.cmbMonth, this.lblYearIcon, this.label4, this.cmbYear });
            this.pnlFilterOuter.Controls.Add(this.pnlFilterCard);

            // Summary row (visual only; existing salary logic remains untouched)
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSummary.Height = 105;
            this.pnlSummary.Padding = new System.Windows.Forms.Padding(28, 6, 28, 10);

            System.Windows.Forms.Control[] cards = { this.cardEmployees, this.cardBasic, this.cardDeduction, this.cardNet };
            foreach (System.Windows.Forms.Control c in cards) c.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cardEmployees.BackColor = System.Drawing.Color.FromArgb(240, 248, 255); this.cardEmployees.BorderRadius = 12; this.cardEmployees.Location = new System.Drawing.Point(28, 6); this.cardEmployees.Size = new System.Drawing.Size(315, 86);
            this.cardBasic.BackColor = System.Drawing.Color.FromArgb(240, 252, 246); this.cardBasic.BorderRadius = 12; this.cardBasic.Location = new System.Drawing.Point(357, 6); this.cardBasic.Size = new System.Drawing.Size(315, 86);
            this.cardDeduction.BackColor = System.Drawing.Color.FromArgb(255, 249, 237); this.cardDeduction.BorderRadius = 12; this.cardDeduction.Location = new System.Drawing.Point(686, 6); this.cardDeduction.Size = new System.Drawing.Size(315, 86);
            this.cardNet.BackColor = System.Drawing.Color.FromArgb(247, 243, 255); this.cardNet.BorderRadius = 12; this.cardNet.Location = new System.Drawing.Point(1015, 6); this.cardNet.Size = new System.Drawing.Size(315, 86);

            this.lblEmployeesIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 21F); this.lblEmployeesIcon.ForeColor = System.Drawing.Color.FromArgb(30, 136, 229); this.lblEmployeesIcon.Location = new System.Drawing.Point(20, 23); this.lblEmployeesIcon.Size = new System.Drawing.Size(45, 40); this.lblEmployeesIcon.Text = ""; this.lblEmployeesIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmployeesTitle.AutoSize = true; this.lblEmployeesTitle.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblEmployeesTitle.ForeColor = System.Drawing.Color.FromArgb(30, 136, 229); this.lblEmployeesTitle.Location = new System.Drawing.Point(24, 17); this.lblEmployeesTitle.Text = "Total Employees";
            this.lblEmployeesValue.AutoSize = true; this.lblEmployeesValue.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold); this.lblEmployeesValue.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82); this.lblEmployeesValue.Location = new System.Drawing.Point(22, 39); this.lblEmployeesValue.Text = "—";
            this.cardEmployees.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblEmployeesTitle, this.lblEmployeesValue });

            this.lblBasicIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 21F); this.lblBasicIcon.ForeColor = System.Drawing.Color.FromArgb(20, 150, 100); this.lblBasicIcon.Location = new System.Drawing.Point(20, 23); this.lblBasicIcon.Size = new System.Drawing.Size(45, 40); this.lblBasicIcon.Text = ""; this.lblBasicIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBasicTitle.AutoSize = true; this.lblBasicTitle.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblBasicTitle.ForeColor = System.Drawing.Color.FromArgb(20, 130, 90); this.lblBasicTitle.Location = new System.Drawing.Point(24, 17); this.lblBasicTitle.Text = "Total Basic Salary";
            this.lblBasicValue.AutoSize = true; this.lblBasicValue.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold); this.lblBasicValue.ForeColor = System.Drawing.Color.FromArgb(20, 110, 80); this.lblBasicValue.Location = new System.Drawing.Point(22, 39); this.lblBasicValue.Text = "—";
            this.cardBasic.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblBasicTitle, this.lblBasicValue });

            this.lblDeductionIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 21F); this.lblDeductionIcon.ForeColor = System.Drawing.Color.FromArgb(235, 145, 25); this.lblDeductionIcon.Location = new System.Drawing.Point(20, 23); this.lblDeductionIcon.Size = new System.Drawing.Size(45, 40); this.lblDeductionIcon.Text = ""; this.lblDeductionIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDeductionTitle.AutoSize = true; this.lblDeductionTitle.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblDeductionTitle.ForeColor = System.Drawing.Color.FromArgb(220, 125, 15); this.lblDeductionTitle.Location = new System.Drawing.Point(24, 17); this.lblDeductionTitle.Text = "Total Deductions";
            this.lblDeductionValue.AutoSize = true; this.lblDeductionValue.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold); this.lblDeductionValue.ForeColor = System.Drawing.Color.FromArgb(185, 90, 10); this.lblDeductionValue.Location = new System.Drawing.Point(22, 39); this.lblDeductionValue.Text = "—";
            this.cardDeduction.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblDeductionTitle, this.lblDeductionValue });

            this.lblNetIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 21F); this.lblNetIcon.ForeColor = System.Drawing.Color.FromArgb(105, 65, 198); this.lblNetIcon.Location = new System.Drawing.Point(20, 23); this.lblNetIcon.Size = new System.Drawing.Size(45, 40); this.lblNetIcon.Text = ""; this.lblNetIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNetTitle.AutoSize = true; this.lblNetTitle.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblNetTitle.ForeColor = System.Drawing.Color.FromArgb(105, 65, 198); this.lblNetTitle.Location = new System.Drawing.Point(24, 17); this.lblNetTitle.Text = "Total Net Salary";
            this.lblNetValue.AutoSize = true; this.lblNetValue.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold); this.lblNetValue.ForeColor = System.Drawing.Color.FromArgb(72, 35, 160); this.lblNetValue.Location = new System.Drawing.Point(22, 39); this.lblNetValue.Text = "—";
            this.cardNet.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblNetTitle, this.lblNetValue });

            // Summary area intentionally left blank.

            // Grid outer/card
            this.pnlGridOuter.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlGridOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridOuter.Padding = new System.Windows.Forms.Padding(28, 4, 28, 8);

            this.pnlGridCard.BackColor = System.Drawing.Color.White;
            this.pnlGridCard.BorderColor = System.Drawing.Color.FromArgb(230, 233, 242);
            this.pnlGridCard.BorderRadius = 12;
            this.pnlGridCard.BorderThickness = 1;
            this.pnlGridCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridCard.Padding = new System.Windows.Forms.Padding(10, 0, 10, 10);

            this.pnlGridTitle.BackColor = System.Drawing.Color.White;
            this.pnlGridTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGridTitle.Height = 52;
            this.lblGridIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 15F);
            this.lblGridIcon.ForeColor = System.Drawing.Color.FromArgb(91, 46, 205);
            this.lblGridIcon.Location = new System.Drawing.Point(12, 13);
            this.lblGridIcon.Size = new System.Drawing.Size(28, 28);
            this.lblGridIcon.Text = "";
            this.lblGridIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblGridTitle.AutoSize = true; this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold); this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82); this.lblGridTitle.Location = new System.Drawing.Point(44, 16); this.lblGridTitle.Text = "Salary Information";
            this.txtSearchVisual.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.txtSearchVisual.BorderColor = System.Drawing.Color.FromArgb(220, 225, 236);
            this.txtSearchVisual.BorderRadius = 7;
            this.txtSearchVisual.PlaceholderText = "Search employee...";
            this.txtSearchVisual.Location = new System.Drawing.Point(1085, 10);
            this.txtSearchVisual.Size = new System.Drawing.Size(240, 32);
            this.pnlGridTitle.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblGridIcon, this.lblGridTitle, this.txtSearchVisual });

            // grouped header
            this.guna2Panel1.BackColor = System.Drawing.Color.White;
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.Height = 36;
            this.guna2Panel1.Controls.AddRange(new System.Windows.Forms.Control[] { this.guna2Panel2, this.guna2Panel4, this.guna2Panel6, this.guna2Panel8 });
            this.guna2Panel2.BackColor = System.Drawing.Color.FromArgb(232, 242, 255); this.guna2Panel2.Dock = System.Windows.Forms.DockStyle.Left; this.guna2Panel2.Width = 380;
            this.guna2Panel4.BackColor = System.Drawing.Color.FromArgb(232, 249, 240); this.guna2Panel4.Dock = System.Windows.Forms.DockStyle.Left; this.guna2Panel4.Width = 450;
            this.guna2Panel6.BackColor = System.Drawing.Color.FromArgb(255, 238, 238); this.guna2Panel6.Dock = System.Windows.Forms.DockStyle.Left; this.guna2Panel6.Width = 310;
            this.guna2Panel8.BackColor = System.Drawing.Color.FromArgb(241, 235, 255); this.guna2Panel8.Dock = System.Windows.Forms.DockStyle.Fill;

            this.guna2HtmlLabel1.Dock = System.Windows.Forms.DockStyle.Fill; this.guna2HtmlLabel1.Text = "Employee Information"; this.guna2HtmlLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.FromArgb(20, 70, 150); this.guna2HtmlLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter; this.guna2Panel2.Controls.Add(this.guna2HtmlLabel1);
            this.guna2HtmlLabel2.Dock = System.Windows.Forms.DockStyle.Fill; this.guna2HtmlLabel2.Text = "Salary Information"; this.guna2HtmlLabel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.FromArgb(20, 115, 75); this.guna2HtmlLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter; this.guna2Panel4.Controls.Add(this.guna2HtmlLabel2);
            this.guna2HtmlLabel3.Dock = System.Windows.Forms.DockStyle.Fill; this.guna2HtmlLabel3.Text = "Deduction"; this.guna2HtmlLabel3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.guna2HtmlLabel3.ForeColor = System.Drawing.Color.FromArgb(190, 55, 55); this.guna2HtmlLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter; this.guna2Panel6.Controls.Add(this.guna2HtmlLabel3);
            this.guna2HtmlLabel4.Dock = System.Windows.Forms.DockStyle.Fill; this.guna2HtmlLabel4.Text = "Salary Summary"; this.guna2HtmlLabel4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.guna2HtmlLabel4.ForeColor = System.Drawing.Color.FromArgb(80, 45, 165); this.guna2HtmlLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter; this.guna2Panel8.Controls.Add(this.guna2HtmlLabel4);

            // dgvSalarySheet
            this.dgvSalarySheet.AllowUserToAddRows = false;
            this.dgvSalarySheet.AllowUserToDeleteRows = false;
            this.dgvSalarySheet.AllowUserToResizeRows = false;
            this.dgvSalarySheet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvSalarySheet.BackgroundColor = System.Drawing.Color.White;
            this.dgvSalarySheet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSalarySheet.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvSalarySheet.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            headerStyle.BackColor = System.Drawing.Color.FromArgb(248, 249, 252);
            headerStyle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            headerStyle.ForeColor = System.Drawing.Color.FromArgb(24, 48, 86);
            headerStyle.SelectionBackColor = System.Drawing.Color.FromArgb(248, 249, 252);
            headerStyle.SelectionForeColor = System.Drawing.Color.FromArgb(24, 48, 86);
            headerStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSalarySheet.ColumnHeadersDefaultCellStyle = headerStyle;
            this.dgvSalarySheet.ColumnHeadersHeight = 48;
            this.dgvSalarySheet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSalarySheet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.Sn,
                this.EmployeeID,
                this.EmployeeName,
                this.Position,
                this.WorkDays,
                this.AbsentDays,
                this.BasicSalary,
                this.Allowance,
                this.Other,
                this.Additions,
                this.TotalAdditions,
                this.EmployeeAdvance,
                this.Delay,
                this.WorkAbsence,
                this.Salik,
                this.othersDeductions,
                this.TotalDeductions,
                this.NetSalary,
                this.Cash,
                this.Bank,
                this.WPS
            });
            rowStyle.BackColor = System.Drawing.Color.White;
            rowStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            rowStyle.ForeColor = System.Drawing.Color.FromArgb(35, 55, 85);
            rowStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            rowStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 55, 85);
            this.dgvSalarySheet.DefaultCellStyle = rowStyle;
            altStyle.BackColor = System.Drawing.Color.FromArgb(252, 252, 254);
            this.dgvSalarySheet.AlternatingRowsDefaultCellStyle = altStyle;
            this.dgvSalarySheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSalarySheet.GridColor = System.Drawing.Color.FromArgb(232, 235, 242);
            this.dgvSalarySheet.MultiSelect = false;
            this.dgvSalarySheet.Name = "dgvSalarySheet";
            this.dgvSalarySheet.RowHeadersVisible = false;
            this.dgvSalarySheet.RowTemplate.Height = 34;
            this.dgvSalarySheet.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dgvSalarySheet.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalarySheet_CellDoubleClick);
            this.dgvSalarySheet.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalarySheet_CellValueChanged);

            // Sn
            this.Sn.HeaderText = "Sn";
            this.Sn.Name = "Sn";
            this.Sn.ReadOnly = true;
            this.Sn.MinimumWidth = 55;
            this.Sn.Width = 45;

            // EmployeeID
            this.EmployeeID.HeaderText = "EMP ID";
            this.EmployeeID.Name = "EmployeeID";
            this.EmployeeID.ReadOnly = true;
            this.EmployeeID.MinimumWidth = 55;
            this.EmployeeID.Width = 80;

            // EmployeeName
            this.EmployeeName.HeaderText = "Employee Name";
            this.EmployeeName.Name = "EmployeeName";
            this.EmployeeName.ReadOnly = true;
            this.EmployeeName.MinimumWidth = 55;
            this.EmployeeName.Width = 130;

            // Position
            this.Position.HeaderText = "Position";
            this.Position.Name = "Position";
            this.Position.ReadOnly = true;
            this.Position.MinimumWidth = 55;
            this.Position.Width = 90;

            // WorkDays
            this.WorkDays.HeaderText = "Work Days";
            this.WorkDays.Name = "WorkDays";
            this.WorkDays.ReadOnly = true;
            this.WorkDays.MinimumWidth = 55;
            this.WorkDays.Width = 75;

            // AbsentDays
            this.AbsentDays.HeaderText = "Absent Days";
            this.AbsentDays.Name = "AbsentDays";
            this.AbsentDays.ReadOnly = true;
            this.AbsentDays.MinimumWidth = 55;
            this.AbsentDays.Width = 80;
            this.AbsentDays.Visible = false;

            // BasicSalary
            this.BasicSalary.HeaderText = "Basic Salary";
            this.BasicSalary.Name = "BasicSalary";
            this.BasicSalary.ReadOnly = true;
            this.BasicSalary.MinimumWidth = 55;
            this.BasicSalary.Width = 95;

            // Allowance
            this.Allowance.HeaderText = "Allowance";
            this.Allowance.Name = "Allowance";
            this.Allowance.ReadOnly = true;
            this.Allowance.MinimumWidth = 55;
            this.Allowance.Width = 85;

            // Other
            this.Other.HeaderText = "Other";
            this.Other.Name = "Other";
            this.Other.ReadOnly = true;
            this.Other.MinimumWidth = 55;
            this.Other.Width = 75;

            // Additions
            this.Additions.HeaderText = "Additions";
            this.Additions.Name = "Additions";
            this.Additions.ReadOnly = true;
            this.Additions.MinimumWidth = 55;
            this.Additions.Width = 85;

            // TotalAdditions
            this.TotalAdditions.HeaderText = "Total Addition";
            this.TotalAdditions.Name = "TotalAdditions";
            this.TotalAdditions.ReadOnly = true;
            this.TotalAdditions.MinimumWidth = 55;
            this.TotalAdditions.Width = 100;

            // EmployeeAdvance
            this.EmployeeAdvance.HeaderText = "Loan";
            this.EmployeeAdvance.Name = "EmployeeAdvance";
            this.EmployeeAdvance.ReadOnly = true;
            this.EmployeeAdvance.MinimumWidth = 55;
            this.EmployeeAdvance.Width = 80;

            // Delay
            this.Delay.HeaderText = "Delay";
            this.Delay.Name = "Delay";
            this.Delay.ReadOnly = true;
            this.Delay.MinimumWidth = 55;
            this.Delay.Width = 70;
            this.Delay.Visible = false;

            // WorkAbsence
            this.WorkAbsence.HeaderText = "Work Absence";
            this.WorkAbsence.Name = "WorkAbsence";
            this.WorkAbsence.ReadOnly = true;
            this.WorkAbsence.MinimumWidth = 55;
            this.WorkAbsence.Width = 95;

            // Salik
            this.Salik.HeaderText = "Fines & Salik";
            this.Salik.Name = "Salik";
            this.Salik.ReadOnly = false;
            this.Salik.MinimumWidth = 55;
            this.Salik.Width = 95;

            // othersDeductions
            this.othersDeductions.HeaderText = "Others";
            this.othersDeductions.Name = "othersDeductions";
            this.othersDeductions.ReadOnly = false;
            this.othersDeductions.MinimumWidth = 55;
            this.othersDeductions.Width = 80;

            // TotalDeductions
            this.TotalDeductions.HeaderText = "Total Deductions";
            this.TotalDeductions.Name = "TotalDeductions";
            this.TotalDeductions.ReadOnly = true;
            this.TotalDeductions.MinimumWidth = 55;
            this.TotalDeductions.Width = 110;

            // NetSalary
            this.NetSalary.HeaderText = "Net Salary";
            this.NetSalary.Name = "NetSalary";
            this.NetSalary.ReadOnly = true;
            this.NetSalary.MinimumWidth = 55;
            this.NetSalary.Width = 100;

            // Cash
            this.Cash.HeaderText = "Cash";
            this.Cash.Name = "Cash";
            this.Cash.ReadOnly = true;
            this.Cash.MinimumWidth = 55;
            this.Cash.Width = 80;

            // Bank
            this.Bank.HeaderText = "Bank";
            this.Bank.Name = "Bank";
            this.Bank.ReadOnly = true;
            this.Bank.MinimumWidth = 55;
            this.Bank.Width = 80;

            // WPS
            this.WPS.HeaderText = "WPS";
            this.WPS.Name = "WPS";
            this.WPS.ReadOnly = true;
            this.WPS.MinimumWidth = 55;
            this.WPS.Width = 80;


            this.pnlGridCard.Controls.Add(this.dgvSalarySheet);
            this.pnlGridCard.Controls.Add(this.guna2Panel1);
            this.pnlGridCard.Controls.Add(this.pnlGridTitle);
            this.pnlGridOuter.Controls.Add(this.pnlGridCard);

            // Footer
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height = 74;
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(28, 8, 28, 12);

            this.lblFooterInfo.AutoSize = true;
            this.lblFooterInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFooterInfo.ForeColor = System.Drawing.Color.FromArgb(70, 100, 160);
            this.lblFooterInfo.Location = new System.Drawing.Point(42, 28);
            this.lblFooterInfo.Text = "ⓘ   Salary sheet helps track employee payroll, allowances, deductions and net salary.";

            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnSave.BorderRadius = 8;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(103, 65, 198);
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(1180, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(170, 44);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.pnlFooter.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblFooterInfo, this.btnSave });

            // hidden legacy controls retained only so older partial references remain safe
            this.panel2.Visible = false;
            this.linkLabel1.Visible = false;
            this.guna2Panel3.Visible = false;
            this.guna2Panel5.Visible = false;
            this.guna2Panel7.Visible = false;

            // frmSalarySheet
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.ClientSize = new System.Drawing.Size(1386, 760);
            this.Controls.Add(this.pnlGridOuter);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlFilterOuter);
            this.Controls.Add(this.pnlHero);
            this.Controls.Add(this.headerUC1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(1180, 680);
            this.Name = "frmSalarySheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "main";
            this.Text = "Salary Sheet";
            this.Load += new System.EventHandler(this.frmSalarySheet_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvSalarySheet)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private HeaderUC headerUC1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private System.Windows.Forms.Panel pnlHero;
        private Guna.UI2.WinForms.Guna2Panel pnlTitleIcon;
        private System.Windows.Forms.Label lblTitleIcon;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Label lblPageSubTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlMetaCard;
        private System.Windows.Forms.Label lblDateIcon;
        private System.Windows.Forms.Label lblRefIcon;
        private System.Windows.Forms.Label lblUserIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblREF;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblSSNo;

        private System.Windows.Forms.Panel pnlFilterOuter;
        private Guna.UI2.WinForms.Guna2Panel pnlFilterCard;
        private System.Windows.Forms.Label lblMonthIcon;
        private System.Windows.Forms.Label lblYearIcon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbMonth;
        private System.Windows.Forms.ComboBox cmbYear;

        private System.Windows.Forms.Panel pnlSummary;
        private Guna.UI2.WinForms.Guna2Panel cardEmployees;
        private Guna.UI2.WinForms.Guna2Panel cardBasic;
        private Guna.UI2.WinForms.Guna2Panel cardDeduction;
        private Guna.UI2.WinForms.Guna2Panel cardNet;
        private System.Windows.Forms.Label lblEmployeesIcon;
        private System.Windows.Forms.Label lblEmployeesTitle;
        private System.Windows.Forms.Label lblEmployeesValue;
        private System.Windows.Forms.Label lblBasicIcon;
        private System.Windows.Forms.Label lblBasicTitle;
        private System.Windows.Forms.Label lblBasicValue;
        private System.Windows.Forms.Label lblDeductionIcon;
        private System.Windows.Forms.Label lblDeductionTitle;
        private System.Windows.Forms.Label lblDeductionValue;
        private System.Windows.Forms.Label lblNetIcon;
        private System.Windows.Forms.Label lblNetTitle;
        private System.Windows.Forms.Label lblNetValue;

        private System.Windows.Forms.Panel pnlGridOuter;
        private Guna.UI2.WinForms.Guna2Panel pnlGridCard;
        private System.Windows.Forms.Panel pnlGridTitle;
        private System.Windows.Forms.Label lblGridIcon;
        private System.Windows.Forms.Label lblGridTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtSearchVisual;

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel4;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel5;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel6;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel7;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel8;
        private System.Windows.Forms.Label guna2HtmlLabel1;
        private System.Windows.Forms.Label guna2HtmlLabel2;
        private System.Windows.Forms.Label guna2HtmlLabel3;
        private System.Windows.Forms.Label guna2HtmlLabel4;

        private Guna.UI2.WinForms.Guna2DataGridView dgvSalarySheet;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sn;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmployeeID;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmployeeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkDays;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbsentDays;
        private System.Windows.Forms.DataGridViewTextBoxColumn BasicSalary;
        private System.Windows.Forms.DataGridViewTextBoxColumn Allowance;
        private System.Windows.Forms.DataGridViewTextBoxColumn Other;
        private System.Windows.Forms.DataGridViewTextBoxColumn Additions;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAdditions;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmployeeAdvance;
        private System.Windows.Forms.DataGridViewTextBoxColumn Delay;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkAbsence;
        private System.Windows.Forms.DataGridViewTextBoxColumn Salik;
        private System.Windows.Forms.DataGridViewTextBoxColumn othersDeductions;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalDeductions;
        private System.Windows.Forms.DataGridViewTextBoxColumn NetSalary;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cash;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bank;
        private System.Windows.Forms.DataGridViewTextBoxColumn WPS;

        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblFooterInfo;
        private Guna.UI2.WinForms.Guna2Button btnSave;

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column19;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column20;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column21;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column22;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column23;
    }
}
