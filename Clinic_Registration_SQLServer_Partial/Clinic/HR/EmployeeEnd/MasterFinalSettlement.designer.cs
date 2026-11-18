namespace YamyProject
{
    partial class MasterFinalSettlement
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.headerUC1 = new YamyProject.HeaderUC();
            this.pnlPageHeader = new System.Windows.Forms.Panel();
            this.pnlTitleIcon = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitleIcon = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlFilterCard = new Guna.UI2.WinForms.Guna2Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbState = new Guna.UI2.WinForms.Guna2ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnlGridCard = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvCustomer = new Guna.UI2.WinForms.Guna2DataGridView();
            this.pnlGridHeader = new System.Windows.Forms.Panel();
            this.lblGridIcon = new System.Windows.Forms.Label();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.lblGridHint = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFooterHint = new System.Windows.Forms.Label();
            this.btnDelete = new Guna.UI2.WinForms.Guna2Button();
            this.btnNew = new Guna.UI2.WinForms.Guna2Button();
            this.pnlPageHeader.SuspendLayout();
            this.pnlTitleIcon.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlFilterCard.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnlGridCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();
            this.pnlGridHeader.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerUC1
            // 
            this.headerUC1.ArabicLayout = false;
            this.headerUC1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(42)))), ((int)(((byte)(190)))));
            this.headerUC1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerUC1.FormText = "Title";
            this.headerUC1.HeaderText = "Title";
            this.headerUC1.Location = new System.Drawing.Point(0, 0);
            this.headerUC1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.headerUC1.Name = "headerUC1";
            this.headerUC1.Size = new System.Drawing.Size(1600, 37);
            this.headerUC1.TabIndex = 0;
            // 
            // pnlPageHeader
            // 
            this.pnlPageHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.pnlPageHeader.Controls.Add(this.pnlTitleIcon);
            this.pnlPageHeader.Controls.Add(this.lblTitle);
            this.pnlPageHeader.Controls.Add(this.lblSubtitle);
            this.pnlPageHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPageHeader.Location = new System.Drawing.Point(0, 37);
            this.pnlPageHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlPageHeader.Name = "pnlPageHeader";
            this.pnlPageHeader.Padding = new System.Windows.Forms.Padding(37, 25, 37, 12);
            this.pnlPageHeader.Size = new System.Drawing.Size(1600, 129);
            this.pnlPageHeader.TabIndex = 1;
            // 
            // pnlTitleIcon
            // 
            this.pnlTitleIcon.BackColor = System.Drawing.Color.Transparent;
            this.pnlTitleIcon.BorderRadius = 12;
            this.pnlTitleIcon.Controls.Add(this.lblTitleIcon);
            this.pnlTitleIcon.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            this.pnlTitleIcon.Location = new System.Drawing.Point(40, 22);
            this.pnlTitleIcon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlTitleIcon.Name = "pnlTitleIcon";
            this.pnlTitleIcon.Size = new System.Drawing.Size(77, 71);
            this.pnlTitleIcon.TabIndex = 0;
            // 
            // lblTitleIcon
            // 
            this.lblTitleIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblTitleIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitleIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(69)))), ((int)(((byte)(205)))));
            this.lblTitleIcon.Location = new System.Drawing.Point(0, 0);
            this.lblTitleIcon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitleIcon.Name = "lblTitleIcon";
            this.lblTitleIcon.Size = new System.Drawing.Size(77, 71);
            this.lblTitleIcon.TabIndex = 0;
            this.lblTitleIcon.Text = "✓";
            this.lblTitleIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(45)))), ((int)(((byte)(91)))));
            this.lblTitle.Location = new System.Drawing.Point(137, 21);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(305, 50);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Final Settlement";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(112)))), ((int)(((byte)(139)))));
            this.lblSubtitle.Location = new System.Drawing.Point(141, 75);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(430, 23);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "Review and manage employee final settlement records.";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.panel2.Controls.Add(this.pnlFilterCard);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 166);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(37, 7, 37, 17);
            this.panel2.Size = new System.Drawing.Size(1600, 94);
            this.panel2.TabIndex = 2;
            // 
            // pnlFilterCard
            // 
            this.pnlFilterCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlFilterCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(229)))), ((int)(((byte)(239)))));
            this.pnlFilterCard.BorderRadius = 10;
            this.pnlFilterCard.BorderThickness = 1;
            this.pnlFilterCard.Controls.Add(this.label2);
            this.pnlFilterCard.Controls.Add(this.cmbState);
            this.pnlFilterCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilterCard.FillColor = System.Drawing.Color.White;
            this.pnlFilterCard.Location = new System.Drawing.Point(37, 7);
            this.pnlFilterCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlFilterCard.Name = "pnlFilterCard";
            this.pnlFilterCard.Size = new System.Drawing.Size(1526, 70);
            this.pnlFilterCard.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(46)))), ((int)(((byte)(83)))));
            this.label2.Location = new System.Drawing.Point(27, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "State";
            // 
            // cmbState
            // 
            this.cmbState.BackColor = System.Drawing.Color.Transparent;
            this.cmbState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(216)))), ((int)(((byte)(230)))));
            this.cmbState.BorderRadius = 7;
            this.cmbState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbState.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(69)))), ((int)(((byte)(205)))));
            this.cmbState.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(69)))), ((int)(((byte)(205)))));
            this.cmbState.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(68)))), ((int)(((byte)(92)))));
            this.cmbState.ItemHeight = 24;
            this.cmbState.Items.AddRange(new object[] {
            "All",
            "Active",
            "Inactive"});
            this.cmbState.Location = new System.Drawing.Point(109, 15);
            this.cmbState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbState.Name = "cmbState";
            this.cmbState.Size = new System.Drawing.Size(272, 30);
            this.cmbState.StartIndex = 0;
            this.cmbState.TabIndex = 1;
            this.cmbState.SelectedIndexChanged += new System.EventHandler(this.cmbState_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.panel3.Controls.Add(this.pnlGridCard);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 260);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(37, 5, 37, 17);
            this.panel3.Size = new System.Drawing.Size(1600, 583);
            this.panel3.TabIndex = 3;
            // 
            // pnlGridCard
            // 
            this.pnlGridCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlGridCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(229)))), ((int)(((byte)(239)))));
            this.pnlGridCard.BorderRadius = 10;
            this.pnlGridCard.BorderThickness = 1;
            this.pnlGridCard.Controls.Add(this.dgvCustomer);
            this.pnlGridCard.Controls.Add(this.pnlGridHeader);
            this.pnlGridCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridCard.FillColor = System.Drawing.Color.White;
            this.pnlGridCard.Location = new System.Drawing.Point(37, 5);
            this.pnlGridCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlGridCard.Name = "pnlGridCard";
            this.pnlGridCard.Padding = new System.Windows.Forms.Padding(16, 15, 16, 15);
            this.pnlGridCard.Size = new System.Drawing.Size(1526, 561);
            this.pnlGridCard.TabIndex = 0;
            // 
            // dgvCustomer
            // 
            this.dgvCustomer.AllowUserToAddRows = false;
            this.dgvCustomer.AllowUserToDeleteRows = false;
            this.dgvCustomer.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            this.dgvCustomer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomer.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(241)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(45)))), ((int)(((byte)(151)))));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(241)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(45)))), ((int)(((byte)(151)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomer.ColumnHeadersHeight = 44;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCustomer.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomer.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(241)))));
            this.dgvCustomer.Location = new System.Drawing.Point(16, 83);
            this.dgvCustomer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvCustomer.MultiSelect = false;
            this.dgvCustomer.Name = "dgvCustomer";
            this.dgvCustomer.ReadOnly = true;
            this.dgvCustomer.RowHeadersVisible = false;
            this.dgvCustomer.RowHeadersWidth = 51;
            this.dgvCustomer.RowTemplate.Height = 38;
            this.dgvCustomer.Size = new System.Drawing.Size(1494, 463);
            this.dgvCustomer.TabIndex = 1;
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(255)))));
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            this.dgvCustomer.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvCustomer.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(241)))));
            this.dgvCustomer.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(241)))), ((int)(((byte)(253)))));
            this.dgvCustomer.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvCustomer.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvCustomer.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(45)))), ((int)(((byte)(151)))));
            this.dgvCustomer.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCustomer.ThemeStyle.HeaderStyle.Height = 44;
            this.dgvCustomer.ThemeStyle.ReadOnly = true;
            this.dgvCustomer.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvCustomer.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvCustomer.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvCustomer.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            this.dgvCustomer.ThemeStyle.RowsStyle.Height = 38;
            this.dgvCustomer.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(255)))));
            this.dgvCustomer.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(76)))));
            this.dgvCustomer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomer_CellDoubleClick);
            this.dgvCustomer.MouseLeave += new System.EventHandler(this.dgvCustomer_Leave);
            this.dgvCustomer.MouseHover += new System.EventHandler(this.dgvCustomer_MouseHover);
            // 
            // pnlGridHeader
            // 
            this.pnlGridHeader.BackColor = System.Drawing.Color.White;
            this.pnlGridHeader.Controls.Add(this.lblGridIcon);
            this.pnlGridHeader.Controls.Add(this.lblGridTitle);
            this.pnlGridHeader.Controls.Add(this.lblGridHint);
            this.pnlGridHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGridHeader.Location = new System.Drawing.Point(16, 15);
            this.pnlGridHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlGridHeader.Name = "pnlGridHeader";
            this.pnlGridHeader.Size = new System.Drawing.Size(1494, 68);
            this.pnlGridHeader.TabIndex = 0;
            // 
            // lblGridIcon
            // 
            this.lblGridIcon.AutoSize = true;
            this.lblGridIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 15F);
            this.lblGridIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(69)))), ((int)(((byte)(205)))));
            this.lblGridIcon.Location = new System.Drawing.Point(9, 16);
            this.lblGridIcon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGridIcon.Name = "lblGridIcon";
            this.lblGridIcon.Size = new System.Drawing.Size(37, 35);
            this.lblGridIcon.TabIndex = 0;
            this.lblGridIcon.Text = "▦";
            // 
            // lblGridTitle
            // 
            this.lblGridTitle.AutoSize = true;
            this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(48)))), ((int)(((byte)(88)))));
            this.lblGridTitle.Location = new System.Drawing.Point(56, 10);
            this.lblGridTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(184, 25);
            this.lblGridTitle.TabIndex = 1;
            this.lblGridTitle.Text = "Settlement Records";
            // 
            // lblGridHint
            // 
            this.lblGridHint.AutoSize = true;
            this.lblGridHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblGridHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(126)))), ((int)(((byte)(148)))));
            this.lblGridHint.Location = new System.Drawing.Point(57, 37);
            this.lblGridHint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGridHint.Name = "lblGridHint";
            this.lblGridHint.Size = new System.Drawing.Size(285, 20);
            this.lblGridHint.TabIndex = 2;
            this.lblGridHint.Text = "Double-click a record to open settlement.";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblFooterHint);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnNew);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 843);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1600, 89);
            this.panel1.TabIndex = 4;
            // 
            // lblFooterHint
            // 
            this.lblFooterHint.AutoSize = true;
            this.lblFooterHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFooterHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(116)))), ((int)(((byte)(145)))));
            this.lblFooterHint.Location = new System.Drawing.Point(40, 36);
            this.lblFooterHint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFooterHint.Name = "lblFooterHint";
            this.lblFooterHint.Size = new System.Drawing.Size(0, 20);
            this.lblFooterHint.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(78)))), ((int)(((byte)(78)))));
            this.btnDelete.BorderRadius = 7;
            this.btnDelete.BorderThickness = 1;
            this.btnDelete.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnDelete.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.btnDelete.FillColor = System.Drawing.Color.White;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnDelete.Location = new System.Drawing.Point(1144, 21);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(180, 47);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.BorderRadius = 7;
            this.btnNew.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnNew.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnNew.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnNew.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnNew.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(69)))), ((int)(((byte)(205)))));
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(1340, 21);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(223, 47);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "+  New Settlement";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // MasterFinalSettlement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1600, 932);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlPageHeader);
            this.Controls.Add(this.headerUC1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MinimumSize = new System.Drawing.Size(1200, 738);
            this.Name = "MasterFinalSettlement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "main";
            this.Text = "Final Settlement Center";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MasterFinalSettlement_FormClosing);
            this.Load += new System.EventHandler(this.MasterFinalSettlement_Load);
            this.pnlPageHeader.ResumeLayout(false);
            this.pnlPageHeader.PerformLayout();
            this.pnlTitleIcon.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlFilterCard.ResumeLayout(false);
            this.pnlFilterCard.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.pnlGridCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).EndInit();
            this.pnlGridHeader.ResumeLayout(false);
            this.pnlGridHeader.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private HeaderUC headerUC1;

        private System.Windows.Forms.Panel pnlPageHeader;
        private Guna.UI2.WinForms.Guna2Panel pnlTitleIcon;
        private System.Windows.Forms.Label lblTitleIcon;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;

        private System.Windows.Forms.Panel panel2;
        private Guna.UI2.WinForms.Guna2Panel pnlFilterCard;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2ComboBox cmbState;

        private System.Windows.Forms.Panel panel3;
        private Guna.UI2.WinForms.Guna2Panel pnlGridCard;
        private System.Windows.Forms.Panel pnlGridHeader;
        private System.Windows.Forms.Label lblGridIcon;
        private System.Windows.Forms.Label lblGridTitle;
        private System.Windows.Forms.Label lblGridHint;
        private Guna.UI2.WinForms.Guna2DataGridView dgvCustomer;

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblFooterHint;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private Guna.UI2.WinForms.Guna2Button btnNew;
    }
}
