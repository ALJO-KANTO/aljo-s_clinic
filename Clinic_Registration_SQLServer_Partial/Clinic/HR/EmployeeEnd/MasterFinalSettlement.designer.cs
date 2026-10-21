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
            this.pnlGridHeader = new System.Windows.Forms.Panel();
            this.lblGridIcon = new System.Windows.Forms.Label();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.lblGridHint = new System.Windows.Forms.Label();
            this.dgvCustomer = new Guna.UI2.WinForms.Guna2DataGridView();

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
            this.pnlGridHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();

            // 
            // headerUC1
            // 
            this.headerUC1.BackColor = System.Drawing.Color.FromArgb(83, 42, 190);
            this.headerUC1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerUC1.FormText = "";
            this.headerUC1.HeaderText = "";
            this.headerUC1.Location = new System.Drawing.Point(0, 0);
            this.headerUC1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.headerUC1.Name = "headerUC1";
            this.headerUC1.Size = new System.Drawing.Size(1200, 30);
            this.headerUC1.TabIndex = 0;

            // 
            // pnlPageHeader
            // 
            this.pnlPageHeader.BackColor = System.Drawing.Color.FromArgb(247, 248, 252);
            this.pnlPageHeader.Controls.Add(this.pnlTitleIcon);
            this.pnlPageHeader.Controls.Add(this.lblTitle);
            this.pnlPageHeader.Controls.Add(this.lblSubtitle);
            this.pnlPageHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPageHeader.Location = new System.Drawing.Point(0, 30);
            this.pnlPageHeader.Name = "pnlPageHeader";
            this.pnlPageHeader.Padding = new System.Windows.Forms.Padding(28, 20, 28, 10);
            this.pnlPageHeader.Size = new System.Drawing.Size(1200, 105);
            this.pnlPageHeader.TabIndex = 1;

            // 
            // pnlTitleIcon
            // 
            this.pnlTitleIcon.BackColor = System.Drawing.Color.Transparent;
            this.pnlTitleIcon.BorderRadius = 12;
            this.pnlTitleIcon.Controls.Add(this.lblTitleIcon);
            this.pnlTitleIcon.FillColor = System.Drawing.Color.FromArgb(239, 232, 255);
            this.pnlTitleIcon.Location = new System.Drawing.Point(30, 18);
            this.pnlTitleIcon.Name = "pnlTitleIcon";
            this.pnlTitleIcon.Size = new System.Drawing.Size(58, 58);
            this.pnlTitleIcon.TabIndex = 0;

            // 
            // lblTitleIcon
            // 
            this.lblTitleIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblTitleIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitleIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleIcon.ForeColor = System.Drawing.Color.FromArgb(104, 69, 205);
            this.lblTitleIcon.Location = new System.Drawing.Point(0, 0);
            this.lblTitleIcon.Name = "lblTitleIcon";
            this.lblTitleIcon.Size = new System.Drawing.Size(58, 58);
            this.lblTitleIcon.TabIndex = 0;
            this.lblTitleIcon.Text = "✓";
            this.lblTitleIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(16, 45, 91);
            this.lblTitle.Location = new System.Drawing.Point(103, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(252, 41);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Final Settlement";

            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(101, 112, 139);
            this.lblSubtitle.Location = new System.Drawing.Point(106, 61);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(359, 19);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "Review and manage employee final settlement records.";

            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(247, 248, 252);
            this.panel2.Controls.Add(this.pnlFilterCard);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 135);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(28, 6, 28, 14);
            this.panel2.Size = new System.Drawing.Size(1200, 76);
            this.panel2.TabIndex = 2;

            // 
            // pnlFilterCard
            // 
            this.pnlFilterCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlFilterCard.BorderColor = System.Drawing.Color.FromArgb(226, 229, 239);
            this.pnlFilterCard.BorderRadius = 10;
            this.pnlFilterCard.BorderThickness = 1;
            this.pnlFilterCard.Controls.Add(this.label2);
            this.pnlFilterCard.Controls.Add(this.cmbState);
            this.pnlFilterCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilterCard.FillColor = System.Drawing.Color.White;
            this.pnlFilterCard.Location = new System.Drawing.Point(28, 6);
            this.pnlFilterCard.Name = "pnlFilterCard";
            this.pnlFilterCard.Size = new System.Drawing.Size(1144, 56);
            this.pnlFilterCard.TabIndex = 0;

            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(24, 46, 83);
            this.label2.Location = new System.Drawing.Point(20, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "State";

            // 
            // cmbState
            // 
            this.cmbState.BackColor = System.Drawing.Color.Transparent;
            this.cmbState.BorderColor = System.Drawing.Color.FromArgb(210, 216, 230);
            this.cmbState.BorderRadius = 7;
            this.cmbState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbState.FillColor = System.Drawing.Color.White;
            this.cmbState.FocusedColor = System.Drawing.Color.FromArgb(104, 69, 205);
            this.cmbState.FocusedState.BorderColor = System.Drawing.Color.FromArgb(104, 69, 205);
            this.cmbState.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbState.ForeColor = System.Drawing.Color.FromArgb(55, 68, 92);
            this.cmbState.ItemHeight = 24;
            this.cmbState.Items.AddRange(new object[] {
            "All",
            "Active",
            "Inactive"});
            this.cmbState.Location = new System.Drawing.Point(82, 12);
            this.cmbState.Name = "cmbState";
            this.cmbState.Size = new System.Drawing.Size(205, 30);
            this.cmbState.StartIndex = 0;
            this.cmbState.TabIndex = 1;
            this.cmbState.SelectedIndexChanged += new System.EventHandler(this.cmbState_SelectedIndexChanged);

            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(247, 248, 252);
            this.panel3.Controls.Add(this.pnlGridCard);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 211);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(28, 4, 28, 14);
            this.panel3.Size = new System.Drawing.Size(1200, 474);
            this.panel3.TabIndex = 3;

            // 
            // pnlGridCard
            // 
            this.pnlGridCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlGridCard.BorderColor = System.Drawing.Color.FromArgb(226, 229, 239);
            this.pnlGridCard.BorderRadius = 10;
            this.pnlGridCard.BorderThickness = 1;
            this.pnlGridCard.Controls.Add(this.dgvCustomer);
            this.pnlGridCard.Controls.Add(this.pnlGridHeader);
            this.pnlGridCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridCard.FillColor = System.Drawing.Color.White;
            this.pnlGridCard.Location = new System.Drawing.Point(28, 4);
            this.pnlGridCard.Name = "pnlGridCard";
            this.pnlGridCard.Padding = new System.Windows.Forms.Padding(12);
            this.pnlGridCard.Size = new System.Drawing.Size(1144, 456);
            this.pnlGridCard.TabIndex = 0;

            // 
            // pnlGridHeader
            // 
            this.pnlGridHeader.BackColor = System.Drawing.Color.White;
            this.pnlGridHeader.Controls.Add(this.lblGridIcon);
            this.pnlGridHeader.Controls.Add(this.lblGridTitle);
            this.pnlGridHeader.Controls.Add(this.lblGridHint);
            this.pnlGridHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGridHeader.Location = new System.Drawing.Point(12, 12);
            this.pnlGridHeader.Name = "pnlGridHeader";
            this.pnlGridHeader.Size = new System.Drawing.Size(1120, 55);
            this.pnlGridHeader.TabIndex = 0;

            // 
            // lblGridIcon
            // 
            this.lblGridIcon.AutoSize = true;
            this.lblGridIcon.Font = new System.Drawing.Font("Segoe UI Symbol", 15F);
            this.lblGridIcon.ForeColor = System.Drawing.Color.FromArgb(104, 69, 205);
            this.lblGridIcon.Location = new System.Drawing.Point(7, 13);
            this.lblGridIcon.Name = "lblGridIcon";
            this.lblGridIcon.Size = new System.Drawing.Size(27, 28);
            this.lblGridIcon.TabIndex = 0;
            this.lblGridIcon.Text = "▦";

            // 
            // lblGridTitle
            // 
            this.lblGridTitle.AutoSize = true;
            this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(22, 48, 88);
            this.lblGridTitle.Location = new System.Drawing.Point(42, 8);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(143, 20);
            this.lblGridTitle.TabIndex = 1;
            this.lblGridTitle.Text = "Settlement Records";

            // 
            // lblGridHint
            // 
            this.lblGridHint.AutoSize = true;
            this.lblGridHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblGridHint.ForeColor = System.Drawing.Color.FromArgb(116, 126, 148);
            this.lblGridHint.Location = new System.Drawing.Point(43, 30);
            this.lblGridHint.Name = "lblGridHint";
            this.lblGridHint.Size = new System.Drawing.Size(230, 15);
            this.lblGridHint.TabIndex = 2;
            this.lblGridHint.Text = "Double-click a record to open settlement.";

            // 
            // dgvCustomer
            // 
            this.dgvCustomer.AllowUserToAddRows = false;
            this.dgvCustomer.AllowUserToDeleteRows = false;
            this.dgvCustomer.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(251, 251, 253);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(45, 55, 76);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(239, 234, 255);
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(45, 55, 76);
            this.dgvCustomer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomer.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvCustomer.BackgroundColor = System.Drawing.Color.White;
            this.dgvCustomer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCustomer.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvCustomer.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(244, 241, 253);
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(72, 45, 151);
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(244, 241, 253);
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(72, 45, 151);
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomer.ColumnHeadersHeight = 44;
            this.dgvCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(45, 55, 76);
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(239, 234, 255);
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(45, 55, 76);
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCustomer.DefaultCellStyle = dataGridViewCellStyle3;

            this.dgvCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomer.GridColor = System.Drawing.Color.FromArgb(232, 234, 241);
            this.dgvCustomer.Location = new System.Drawing.Point(12, 67);
            this.dgvCustomer.MultiSelect = false;
            this.dgvCustomer.Name = "dgvCustomer";
            this.dgvCustomer.ReadOnly = true;
            this.dgvCustomer.RowHeadersVisible = false;
            this.dgvCustomer.RowTemplate.Height = 38;
            this.dgvCustomer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomer.Size = new System.Drawing.Size(1120, 377);
            this.dgvCustomer.TabIndex = 1;

            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(251, 251, 253);
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.FromArgb(45, 55, 76);
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 234, 255);
            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(45, 55, 76);
            this.dgvCustomer.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvCustomer.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(232, 234, 241);
            this.dgvCustomer.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(244, 241, 253);
            this.dgvCustomer.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvCustomer.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvCustomer.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.FromArgb(72, 45, 151);
            this.dgvCustomer.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCustomer.ThemeStyle.HeaderStyle.Height = 44;
            this.dgvCustomer.ThemeStyle.ReadOnly = true;
            this.dgvCustomer.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvCustomer.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvCustomer.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvCustomer.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(45, 55, 76);
            this.dgvCustomer.ThemeStyle.RowsStyle.Height = 38;
            this.dgvCustomer.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 234, 255);
            this.dgvCustomer.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(45, 55, 76);

            this.dgvCustomer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomer_CellDoubleClick);
            this.dgvCustomer.MouseLeave += new System.EventHandler(this.dgvCustomer_Leave);
            this.dgvCustomer.MouseHover += new System.EventHandler(this.dgvCustomer_MouseHover);

            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblFooterHint);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnNew);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 685);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1200, 72);
            this.panel1.TabIndex = 4;

            // 
            // lblFooterHint
            // 
            this.lblFooterHint.AutoSize = true;
            this.lblFooterHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFooterHint.ForeColor = System.Drawing.Color.FromArgb(103, 116, 145);
            this.lblFooterHint.Location = new System.Drawing.Point(30, 29);
            this.lblFooterHint.Name = "lblFooterHint";
            this.lblFooterHint.Size = new System.Drawing.Size(287, 15);
            this.lblFooterHint.TabIndex = 0;
           // this.lblFooterHint.Text = "ⓘ  Manage employee end-of-service settlement records.";

            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BorderColor = System.Drawing.Color.FromArgb(225, 78, 78);
            this.btnDelete.BorderRadius = 7;
            this.btnDelete.BorderThickness = 1;
            this.btnDelete.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.FillColor = System.Drawing.Color.FromArgb(230, 230, 230);
            this.btnDelete.DisabledState.ForeColor = System.Drawing.Color.FromArgb(150, 150, 150);
            this.btnDelete.FillColor = System.Drawing.Color.White;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(196, 55, 55);
            this.btnDelete.Location = new System.Drawing.Point(858, 17);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(135, 38);
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
            this.btnNew.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnNew.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnNew.FillColor = System.Drawing.Color.FromArgb(104, 69, 205);
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(1005, 17);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(167, 38);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "+  New Settlement";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            // 
            // MasterFinalSettlement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(247, 248, 252);
            this.ClientSize = new System.Drawing.Size(1200, 757);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlPageHeader);
            this.Controls.Add(this.headerUC1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MasterFinalSettlement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "main";
            this.Text = "Final Settlement Center";
            this.Load += new System.EventHandler(this.MasterFinalSettlement_Load);

            this.pnlPageHeader.ResumeLayout(false);
            this.pnlPageHeader.PerformLayout();
            this.pnlTitleIcon.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlFilterCard.ResumeLayout(false);
            this.pnlFilterCard.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.pnlGridCard.ResumeLayout(false);
            this.pnlGridHeader.ResumeLayout(false);
            this.pnlGridHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).EndInit();
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
