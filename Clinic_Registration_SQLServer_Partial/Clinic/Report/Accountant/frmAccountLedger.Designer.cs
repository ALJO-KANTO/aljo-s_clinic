namespace YamyProject.UI.Reports.Accountant
{
    partial class frmAccountLedger
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

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFilter = new Guna.UI2.WinForms.Guna2Panel();
            this.btnView = new Guna.UI2.WinForms.Guna2Button();
            this.dtpTo = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.dtpFrom = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.cmbAccount = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.pnlSummary = new System.Windows.Forms.TableLayoutPanel();
            this.pnlOpening = new Guna.UI2.WinForms.Guna2Panel();
            this.lblOpeningValue = new System.Windows.Forms.Label();
            this.lblOpeningCaption = new System.Windows.Forms.Label();
            this.pnlDebit = new Guna.UI2.WinForms.Guna2Panel();
            this.lblDebitValue = new System.Windows.Forms.Label();
            this.lblDebitCaption = new System.Windows.Forms.Label();
            this.pnlCredit = new Guna.UI2.WinForms.Guna2Panel();
            this.lblCreditValue = new System.Windows.Forms.Label();
            this.lblCreditCaption = new System.Windows.Forms.Label();
            this.pnlClosing = new Guna.UI2.WinForms.Guna2Panel();
            this.lblClosingValue = new System.Windows.Forms.Label();
            this.lblClosingCaption = new System.Windows.Forms.Label();
            this.pnlLedger = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvLedger = new Guna.UI2.WinForms.Guna2DataGridView();
            this.pnlFooter = new Guna.UI2.WinForms.Guna2Panel();
            this.btnPrint = new Guna.UI2.WinForms.Guna2Button();
            this.lblTransactionCount = new System.Windows.Forms.Label();
            this.pnlLedgerHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.lblSelectedAccount = new System.Windows.Forms.Label();
            this.guna2CircleButton1 = new Guna.UI2.WinForms.Guna2CircleButton();
            this.btnExcel = new Guna.UI2.WinForms.Guna2Button();
            this.btnPdf = new Guna.UI2.WinForms.Guna2Button();
            this.pnlHeader.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.pnlOpening.SuspendLayout();
            this.pnlDebit.SuspendLayout();
            this.pnlCredit.SuspendLayout();
            this.pnlClosing.SuspendLayout();
            this.pnlLedger.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLedger)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.pnlLedgerHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.guna2CircleButton1);
            this.pnlHeader.Controls.Add(this.lblSubTitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(28, 15, 28, 10);
            this.pnlHeader.Size = new System.Drawing.Size(1350, 82);
            this.pnlHeader.TabIndex = 3;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSubTitle.Location = new System.Drawing.Point(31, 50);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(321, 15);
            this.lblSubTitle.TabIndex = 0;
            this.lblSubTitle.Text = "View account activity, opening balance and closing balance";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTitle.Location = new System.Drawing.Point(28, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(182, 31);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Account Ledger";
            // 
            // pnlFilter
            // 
            this.pnlFilter.BackColor = System.Drawing.Color.White;
            this.pnlFilter.BorderRadius = 10;
            this.pnlFilter.Controls.Add(this.btnPdf);
            this.pnlFilter.Controls.Add(this.btnExcel);
            this.pnlFilter.Controls.Add(this.btnView);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.cmbAccount);
            this.pnlFilter.Controls.Add(this.lblTo);
            this.pnlFilter.Controls.Add(this.lblFrom);
            this.pnlFilter.Controls.Add(this.lblAccount);
            this.pnlFilter.Location = new System.Drawing.Point(28, 101);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(1294, 92);
            this.pnlFilter.TabIndex = 2;
            // 
            // btnView
            // 
            this.btnView.BorderRadius = 7;
            this.btnView.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnView.ForeColor = System.Drawing.Color.White;
            this.btnView.Location = new System.Drawing.Point(905, 36);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(130, 36);
            this.btnView.TabIndex = 0;
            this.btnView.Text = "View Report";
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // dtpTo
            // 
            this.dtpTo.BorderRadius = 6;
            this.dtpTo.Checked = true;
            this.dtpTo.CustomFormat = "dd/MM/yyyy";
            this.dtpTo.FillColor = System.Drawing.Color.White;
            this.dtpTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(690, 36);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(190, 36);
            this.dtpTo.TabIndex = 1;
            this.dtpTo.Value = new System.DateTime(2026, 9, 29, 16, 9, 49, 60);
            // 
            // dtpFrom
            // 
            this.dtpFrom.BorderRadius = 6;
            this.dtpFrom.Checked = true;
            this.dtpFrom.CustomFormat = "dd/MM/yyyy";
            this.dtpFrom.FillColor = System.Drawing.Color.White;
            this.dtpFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(475, 36);
            
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(190, 36);
            this.dtpFrom.TabIndex = 2;
            this.dtpFrom.Value = new System.DateTime(2026, 9, 29, 16, 9, 49, 171);
            // 
            // cmbAccount
            // 
            this.cmbAccount.BackColor = System.Drawing.Color.Transparent;
            this.cmbAccount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.cmbAccount.BorderRadius = 6;
            this.cmbAccount.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccount.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cmbAccount.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cmbAccount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.cmbAccount.ItemHeight = 30;
            this.cmbAccount.Location = new System.Drawing.Point(20, 36);
            this.cmbAccount.Name = "cmbAccount";
            this.cmbAccount.Size = new System.Drawing.Size(430, 36);
            this.cmbAccount.TabIndex = 3;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTo.Location = new System.Drawing.Point(690, 13);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(20, 15);
            this.lblTo.TabIndex = 4;
            this.lblTo.Text = "To";
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblFrom.Location = new System.Drawing.Point(475, 13);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(35, 15);
            this.lblFrom.TabIndex = 5;
            this.lblFrom.Text = "From";
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblAccount.Location = new System.Drawing.Point(20, 13);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(52, 15);
            this.lblAccount.TabIndex = 6;
            this.lblAccount.Text = "Account";
            // 
            // pnlSummary
            // 
            this.pnlSummary.ColumnCount = 4;
            this.pnlSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlSummary.Controls.Add(this.pnlOpening, 0, 0);
            this.pnlSummary.Controls.Add(this.pnlDebit, 1, 0);
            this.pnlSummary.Controls.Add(this.pnlCredit, 2, 0);
            this.pnlSummary.Controls.Add(this.pnlClosing, 3, 0);
            this.pnlSummary.Location = new System.Drawing.Point(28, 210);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.RowCount = 1;
            this.pnlSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlSummary.Size = new System.Drawing.Size(1294, 86);
            this.pnlSummary.TabIndex = 1;
            // 
            // pnlOpening
            // 
            this.pnlOpening.BackColor = System.Drawing.Color.White;
            this.pnlOpening.BorderRadius = 9;
            this.pnlOpening.Controls.Add(this.lblOpeningValue);
            this.pnlOpening.Controls.Add(this.lblOpeningCaption);
            this.pnlOpening.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOpening.Location = new System.Drawing.Point(0, 0);
            this.pnlOpening.Margin = new System.Windows.Forms.Padding(0, 0, 9, 0);
            this.pnlOpening.Name = "pnlOpening";
            this.pnlOpening.Size = new System.Drawing.Size(314, 86);
            this.pnlOpening.TabIndex = 0;
            // 
            // lblOpeningValue
            // 
            this.lblOpeningValue.AutoSize = true;
            this.lblOpeningValue.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblOpeningValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblOpeningValue.Location = new System.Drawing.Point(17, 40);
            this.lblOpeningValue.Name = "lblOpeningValue";
            this.lblOpeningValue.Size = new System.Drawing.Size(107, 30);
            this.lblOpeningValue.TabIndex = 0;
            this.lblOpeningValue.Text = "AED 0.00";
            // 
            // lblOpeningCaption
            // 
            this.lblOpeningCaption.AutoSize = true;
            this.lblOpeningCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblOpeningCaption.Location = new System.Drawing.Point(18, 15);
            this.lblOpeningCaption.Name = "lblOpeningCaption";
            this.lblOpeningCaption.Size = new System.Drawing.Size(97, 15);
            this.lblOpeningCaption.TabIndex = 1;
            this.lblOpeningCaption.Text = "Opening Balance";
            // 
            // pnlDebit
            // 
            this.pnlDebit.BackColor = System.Drawing.Color.White;
            this.pnlDebit.BorderRadius = 9;
            this.pnlDebit.Controls.Add(this.lblDebitValue);
            this.pnlDebit.Controls.Add(this.lblDebitCaption);
            this.pnlDebit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDebit.Location = new System.Drawing.Point(326, 0);
            this.pnlDebit.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
            this.pnlDebit.Name = "pnlDebit";
            this.pnlDebit.Size = new System.Drawing.Size(314, 86);
            this.pnlDebit.TabIndex = 1;
            // 
            // lblDebitValue
            // 
            this.lblDebitValue.AutoSize = true;
            this.lblDebitValue.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblDebitValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblDebitValue.Location = new System.Drawing.Point(17, 40);
            this.lblDebitValue.Name = "lblDebitValue";
            this.lblDebitValue.Size = new System.Drawing.Size(107, 30);
            this.lblDebitValue.TabIndex = 0;
            this.lblDebitValue.Text = "AED 0.00";
            // 
            // lblDebitCaption
            // 
            this.lblDebitCaption.AutoSize = true;
            this.lblDebitCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblDebitCaption.Location = new System.Drawing.Point(18, 15);
            this.lblDebitCaption.Name = "lblDebitCaption";
            this.lblDebitCaption.Size = new System.Drawing.Size(64, 15);
            this.lblDebitCaption.TabIndex = 1;
            this.lblDebitCaption.Text = "Total Debit";
            // 
            // pnlCredit
            // 
            this.pnlCredit.BackColor = System.Drawing.Color.White;
            this.pnlCredit.BorderRadius = 9;
            this.pnlCredit.Controls.Add(this.lblCreditValue);
            this.pnlCredit.Controls.Add(this.lblCreditCaption);
            this.pnlCredit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCredit.Location = new System.Drawing.Point(652, 0);
            this.pnlCredit.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.pnlCredit.Name = "pnlCredit";
            this.pnlCredit.Size = new System.Drawing.Size(314, 86);
            this.pnlCredit.TabIndex = 2;
            // 
            // lblCreditValue
            // 
            this.lblCreditValue.AutoSize = true;
            this.lblCreditValue.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblCreditValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(83)))), ((int)(((byte)(9)))));
            this.lblCreditValue.Location = new System.Drawing.Point(17, 40);
            this.lblCreditValue.Name = "lblCreditValue";
            this.lblCreditValue.Size = new System.Drawing.Size(107, 30);
            this.lblCreditValue.TabIndex = 0;
            this.lblCreditValue.Text = "AED 0.00";
            // 
            // lblCreditCaption
            // 
            this.lblCreditCaption.AutoSize = true;
            this.lblCreditCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCreditCaption.Location = new System.Drawing.Point(18, 15);
            this.lblCreditCaption.Name = "lblCreditCaption";
            this.lblCreditCaption.Size = new System.Drawing.Size(68, 15);
            this.lblCreditCaption.TabIndex = 1;
            this.lblCreditCaption.Text = "Total Credit";
            // 
            // pnlClosing
            // 
            this.pnlClosing.BackColor = System.Drawing.Color.White;
            this.pnlClosing.BorderRadius = 9;
            this.pnlClosing.Controls.Add(this.lblClosingValue);
            this.pnlClosing.Controls.Add(this.lblClosingCaption);
            this.pnlClosing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlClosing.Location = new System.Drawing.Point(978, 0);
            this.pnlClosing.Margin = new System.Windows.Forms.Padding(9, 0, 0, 0);
            this.pnlClosing.Name = "pnlClosing";
            this.pnlClosing.Size = new System.Drawing.Size(316, 86);
            this.pnlClosing.TabIndex = 3;
            // 
            // lblClosingValue
            // 
            this.lblClosingValue.AutoSize = true;
            this.lblClosingValue.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblClosingValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.lblClosingValue.Location = new System.Drawing.Point(17, 40);
            this.lblClosingValue.Name = "lblClosingValue";
            this.lblClosingValue.Size = new System.Drawing.Size(107, 30);
            this.lblClosingValue.TabIndex = 0;
            this.lblClosingValue.Text = "AED 0.00";
            // 
            // lblClosingCaption
            // 
            this.lblClosingCaption.AutoSize = true;
            this.lblClosingCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblClosingCaption.Location = new System.Drawing.Point(18, 15);
            this.lblClosingCaption.Name = "lblClosingCaption";
            this.lblClosingCaption.Size = new System.Drawing.Size(91, 15);
            this.lblClosingCaption.TabIndex = 1;
            this.lblClosingCaption.Text = "Closing Balance";
            // 
            // pnlLedger
            // 
            this.pnlLedger.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLedger.BackColor = System.Drawing.Color.White;
            this.pnlLedger.BorderRadius = 10;
            this.pnlLedger.Controls.Add(this.dgvLedger);
            this.pnlLedger.Controls.Add(this.pnlFooter);
            this.pnlLedger.Controls.Add(this.pnlLedgerHeader);
            this.pnlLedger.Location = new System.Drawing.Point(28, 314);
            this.pnlLedger.Name = "pnlLedger";
            this.pnlLedger.Size = new System.Drawing.Size(1294, 420);
            this.pnlLedger.TabIndex = 0;
            // 
            // dgvLedger
            // 
            this.dgvLedger.AllowUserToAddRows = false;
            this.dgvLedger.AllowUserToDeleteRows = false;
            this.dgvLedger.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvLedger.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.dgvLedger.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLedger.ColumnHeadersHeight = 42;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLedger.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLedger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLedger.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvLedger.Location = new System.Drawing.Point(0, 61);
            this.dgvLedger.MultiSelect = false;
            this.dgvLedger.Name = "dgvLedger";
            this.dgvLedger.ReadOnly = true;
            this.dgvLedger.RowHeadersVisible = false;
            this.dgvLedger.RowTemplate.Height = 38;
            this.dgvLedger.Size = new System.Drawing.Size(1294, 304);
            this.dgvLedger.TabIndex = 0;
            this.dgvLedger.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvLedger.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvLedger.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvLedger.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvLedger.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvLedger.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvLedger.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvLedger.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvLedger.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLedger.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvLedger.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvLedger.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLedger.ThemeStyle.HeaderStyle.Height = 42;
            this.dgvLedger.ThemeStyle.ReadOnly = true;
            this.dgvLedger.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvLedger.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLedger.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvLedger.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvLedger.ThemeStyle.RowsStyle.Height = 38;
            this.dgvLedger.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvLedger.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.White;
            this.pnlFooter.Controls.Add(this.btnPrint);
            this.pnlFooter.Controls.Add(this.lblTransactionCount);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 365);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1294, 55);
            this.pnlFooter.TabIndex = 1;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BorderRadius = 7;
            this.btnPrint.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(2234, 10);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(135, 36);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "Print ▼";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lblTransactionCount
            // 
            this.lblTransactionCount.AutoSize = true;
            this.lblTransactionCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblTransactionCount.Location = new System.Drawing.Point(18, 20);
            this.lblTransactionCount.Name = "lblTransactionCount";
            this.lblTransactionCount.Size = new System.Drawing.Size(80, 15);
            this.lblTransactionCount.TabIndex = 1;
            this.lblTransactionCount.Text = "0 transactions";
            // 
            // pnlLedgerHeader
            // 
            this.pnlLedgerHeader.BackColor = System.Drawing.Color.White;
            this.pnlLedgerHeader.Controls.Add(this.lblPeriod);
            this.pnlLedgerHeader.Controls.Add(this.lblSelectedAccount);
            this.pnlLedgerHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLedgerHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlLedgerHeader.Name = "pnlLedgerHeader";
            this.pnlLedgerHeader.Size = new System.Drawing.Size(1294, 61);
            this.pnlLedgerHeader.TabIndex = 2;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblPeriod.Location = new System.Drawing.Point(19, 36);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(41, 15);
            this.lblPeriod.TabIndex = 0;
            this.lblPeriod.Text = "Period";
            // 
            // lblSelectedAccount
            // 
            this.lblSelectedAccount.AutoSize = true;
            this.lblSelectedAccount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblSelectedAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblSelectedAccount.Location = new System.Drawing.Point(18, 10);
            this.lblSelectedAccount.Name = "lblSelectedAccount";
            this.lblSelectedAccount.Size = new System.Drawing.Size(130, 20);
            this.lblSelectedAccount.TabIndex = 1;
            this.lblSelectedAccount.Text = "Select an account";
            // 
            // guna2CircleButton1
            // 
            this.guna2CircleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2CircleButton1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2CircleButton1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2CircleButton1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2CircleButton1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2CircleButton1.FillColor = System.Drawing.Color.RoyalBlue;
            this.guna2CircleButton1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2CircleButton1.ForeColor = System.Drawing.Color.White;
            this.guna2CircleButton1.Image = global::Clinic.Properties.Resources.closeN;
            this.guna2CircleButton1.Location = new System.Drawing.Point(1312, 0);
            this.guna2CircleButton1.Name = "guna2CircleButton1";
            this.guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CircleButton1.Size = new System.Drawing.Size(38, 36);
            this.guna2CircleButton1.TabIndex = 2;
            this.guna2CircleButton1.Click += new System.EventHandler(this.guna2CircleButton1_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.BorderRadius = 7;
            this.btnExcel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnExcel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExcel.ForeColor = System.Drawing.Color.White;
            this.btnExcel.Location = new System.Drawing.Point(1063, 36);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(82, 36);
            this.btnExcel.TabIndex = 7;
            this.btnExcel.Text = "Excel";
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // btnPdf
            // 
            this.btnPdf.BorderRadius = 7;
            this.btnPdf.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnPdf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPdf.ForeColor = System.Drawing.Color.White;
            this.btnPdf.Location = new System.Drawing.Point(1176, 36);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(82, 36);
            this.btnPdf.TabIndex = 8;
            this.btnPdf.Text = "PDF";
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // frmAccountLedger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1350, 749);
            this.Controls.Add(this.pnlLedger);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmAccountLedger";
            this.Text = "Account Ledger";
            this.Load += new System.EventHandler(this.frmAccountLedger_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlSummary.ResumeLayout(false);
            this.pnlOpening.ResumeLayout(false);
            this.pnlOpening.PerformLayout();
            this.pnlDebit.ResumeLayout(false);
            this.pnlDebit.PerformLayout();
            this.pnlCredit.ResumeLayout(false);
            this.pnlCredit.PerformLayout();
            this.pnlClosing.ResumeLayout(false);
            this.pnlClosing.PerformLayout();
            this.pnlLedger.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLedger)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.pnlLedgerHeader.ResumeLayout(false);
            this.pnlLedgerHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;

        private Guna.UI2.WinForms.Guna2Panel pnlFilter;
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;

        private Guna.UI2.WinForms.Guna2ComboBox cmbAccount;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpFrom;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpTo;
        private Guna.UI2.WinForms.Guna2Button btnView;

        private System.Windows.Forms.TableLayoutPanel pnlSummary;

        private Guna.UI2.WinForms.Guna2Panel pnlOpening;
        private System.Windows.Forms.Label lblOpeningCaption;
        private System.Windows.Forms.Label lblOpeningValue;

        private Guna.UI2.WinForms.Guna2Panel pnlDebit;
        private System.Windows.Forms.Label lblDebitCaption;
        private System.Windows.Forms.Label lblDebitValue;

        private Guna.UI2.WinForms.Guna2Panel pnlCredit;
        private System.Windows.Forms.Label lblCreditCaption;
        private System.Windows.Forms.Label lblCreditValue;

        private Guna.UI2.WinForms.Guna2Panel pnlClosing;
        private System.Windows.Forms.Label lblClosingCaption;
        private System.Windows.Forms.Label lblClosingValue;

        private Guna.UI2.WinForms.Guna2Panel pnlLedger;
        private Guna.UI2.WinForms.Guna2Panel pnlLedgerHeader;

        private System.Windows.Forms.Label lblSelectedAccount;
        private System.Windows.Forms.Label lblPeriod;

        private Guna.UI2.WinForms.Guna2DataGridView dgvLedger;

        private Guna.UI2.WinForms.Guna2Panel pnlFooter;
        private System.Windows.Forms.Label lblTransactionCount;
        private Guna.UI2.WinForms.Guna2Button btnPrint;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton1;
        private Guna.UI2.WinForms.Guna2Button btnExcel;
        private Guna.UI2.WinForms.Guna2Button btnPdf;
    }
}