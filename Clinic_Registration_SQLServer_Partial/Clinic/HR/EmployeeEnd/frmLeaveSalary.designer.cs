namespace YamyProject
{
    partial class frmLeaveSalary
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
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle rowStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle alternateStyle = new System.Windows.Forms.DataGridViewCellStyle();

            this.headerUC1 = new YamyProject.HeaderUC();
            this.pnlPageHeader = new System.Windows.Forms.Panel();
            this.pnlIcon = new Guna.UI2.WinForms.Guna2Panel();
            this.lblIcon = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();

            this.pnlContentOuter = new System.Windows.Forms.Panel();
            this.panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlGridHeader = new System.Windows.Forms.Panel();
            this.lblGridIcon = new System.Windows.Forms.Label();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.lblGridSubtitle = new System.Windows.Forms.Label();
            this.dgvLeaveSalary = new Guna.UI2.WinForms.Guna2DataGridView();

            this.pnlPageHeader.SuspendLayout();
            this.pnlIcon.SuspendLayout();
            this.pnlContentOuter.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnlGridHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeaveSalary)).BeginInit();
            this.SuspendLayout();

            // 
            // headerUC1
            // 
            this.headerUC1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerUC1.FormText = "";
            this.headerUC1.HeaderText = "Leave Salary";
            this.headerUC1.Location = new System.Drawing.Point(0, 0);
            this.headerUC1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.headerUC1.Name = "headerUC1";
            this.headerUC1.Size = new System.Drawing.Size(1202, 34);
            this.headerUC1.TabIndex = 0;

            // 
            // pnlPageHeader
            // 
            this.pnlPageHeader.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlPageHeader.Controls.Add(this.pnlIcon);
            this.pnlPageHeader.Controls.Add(this.lblTitle);
            this.pnlPageHeader.Controls.Add(this.lblSubtitle);
            this.pnlPageHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPageHeader.Location = new System.Drawing.Point(0, 34);
            this.pnlPageHeader.Name = "pnlPageHeader";
            this.pnlPageHeader.Padding = new System.Windows.Forms.Padding(32, 20, 32, 15);
            this.pnlPageHeader.Size = new System.Drawing.Size(1202, 112);
            this.pnlPageHeader.TabIndex = 1;

            // 
            // pnlIcon
            // 
            this.pnlIcon.BackColor = System.Drawing.Color.FromArgb(239, 233, 255);
            this.pnlIcon.BorderRadius = 12;
            this.pnlIcon.Controls.Add(this.lblIcon);
            this.pnlIcon.Location = new System.Drawing.Point(34, 22);
            this.pnlIcon.Name = "pnlIcon";
            this.pnlIcon.Size = new System.Drawing.Size(62, 62);
            this.pnlIcon.TabIndex = 0;

            // 
            // lblIcon
            // 
            this.lblIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 24F);
            this.lblIcon.ForeColor = System.Drawing.Color.FromArgb(103, 65, 198);
            this.lblIcon.Location = new System.Drawing.Point(0, 0);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(62, 62);
            this.lblIcon.TabIndex = 0;
            this.lblIcon.Text = "";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82);
            this.lblTitle.Location = new System.Drawing.Point(116, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(197, 41);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Leave Salary";

            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(105, 115, 145);
            this.lblSubtitle.Location = new System.Drawing.Point(119, 65);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(343, 19);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "Review employee leave salary balances and payments.";

            // 
            // pnlContentOuter
            // 
            this.pnlContentOuter.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlContentOuter.Controls.Add(this.panel3);
            this.pnlContentOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContentOuter.Location = new System.Drawing.Point(0, 146);
            this.pnlContentOuter.Name = "pnlContentOuter";
            this.pnlContentOuter.Padding = new System.Windows.Forms.Padding(28, 8, 28, 28);
            this.pnlContentOuter.Size = new System.Drawing.Size(1202, 434);
            this.pnlContentOuter.TabIndex = 2;

            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderColor = System.Drawing.Color.FromArgb(228, 232, 241);
            this.panel3.BorderRadius = 12;
            this.panel3.BorderThickness = 1;
            this.panel3.Controls.Add(this.dgvLeaveSalary);
            this.panel3.Controls.Add(this.pnlGridHeader);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(28, 8);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(10, 0, 10, 12);
            this.panel3.Size = new System.Drawing.Size(1146, 398);
            this.panel3.TabIndex = 8;

            // 
            // pnlGridHeader
            // 
            this.pnlGridHeader.BackColor = System.Drawing.Color.White;
            this.pnlGridHeader.Controls.Add(this.lblGridIcon);
            this.pnlGridHeader.Controls.Add(this.lblGridTitle);
            this.pnlGridHeader.Controls.Add(this.lblGridSubtitle);
            this.pnlGridHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGridHeader.Location = new System.Drawing.Point(10, 0);
            this.pnlGridHeader.Name = "pnlGridHeader";
            this.pnlGridHeader.Size = new System.Drawing.Size(1126, 62);
            this.pnlGridHeader.TabIndex = 8;

            // 
            // lblGridIcon
            // 
            this.lblGridIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 16F);
            this.lblGridIcon.ForeColor = System.Drawing.Color.FromArgb(103, 65, 198);
            this.lblGridIcon.Location = new System.Drawing.Point(12, 15);
            this.lblGridIcon.Name = "lblGridIcon";
            this.lblGridIcon.Size = new System.Drawing.Size(32, 32);
            this.lblGridIcon.TabIndex = 0;
            this.lblGridIcon.Text = "";
            this.lblGridIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblGridTitle
            // 
            this.lblGridTitle.AutoSize = true;
            this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(17, 42, 82);
            this.lblGridTitle.Location = new System.Drawing.Point(51, 12);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(145, 19);
            this.lblGridTitle.TabIndex = 1;
            this.lblGridTitle.Text = "Leave Salary Details";

            // 
            // lblGridSubtitle
            // 
            this.lblGridSubtitle.AutoSize = true;
            this.lblGridSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblGridSubtitle.ForeColor = System.Drawing.Color.FromArgb(112, 121, 145);
            this.lblGridSubtitle.Location = new System.Drawing.Point(52, 34);
            this.lblGridSubtitle.Name = "lblGridSubtitle";
            this.lblGridSubtitle.Size = new System.Drawing.Size(264, 15);
            this.lblGridSubtitle.TabIndex = 2;
            this.lblGridSubtitle.Text = "Employee leave salary entitlement and remaining balance";

            // 
            // dgvLeaveSalary
            // 
            this.dgvLeaveSalary.AllowUserToAddRows = false;
            this.dgvLeaveSalary.AllowUserToDeleteRows = false;
            this.dgvLeaveSalary.AllowUserToResizeRows = false;
            this.dgvLeaveSalary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            alternateStyle.BackColor = System.Drawing.Color.FromArgb(252, 252, 254);
            alternateStyle.ForeColor = System.Drawing.Color.FromArgb(40, 53, 78);
            alternateStyle.SelectionBackColor = System.Drawing.Color.FromArgb(240, 236, 255);
            alternateStyle.SelectionForeColor = System.Drawing.Color.FromArgb(40, 53, 78);
            this.dgvLeaveSalary.AlternatingRowsDefaultCellStyle = alternateStyle;

            this.dgvLeaveSalary.BackgroundColor = System.Drawing.Color.White;
            this.dgvLeaveSalary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLeaveSalary.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLeaveSalary.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = System.Drawing.Color.FromArgb(244, 241, 252);
            headerStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            headerStyle.ForeColor = System.Drawing.Color.FromArgb(73, 46, 145);
            headerStyle.SelectionBackColor = System.Drawing.Color.FromArgb(244, 241, 252);
            headerStyle.SelectionForeColor = System.Drawing.Color.FromArgb(73, 46, 145);
            headerStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLeaveSalary.ColumnHeadersDefaultCellStyle = headerStyle;
            this.dgvLeaveSalary.ColumnHeadersHeight = 46;
            this.dgvLeaveSalary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            rowStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            rowStyle.BackColor = System.Drawing.Color.White;
            rowStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            rowStyle.ForeColor = System.Drawing.Color.FromArgb(40, 53, 78);
            rowStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            rowStyle.SelectionForeColor = System.Drawing.Color.FromArgb(40, 53, 78);
            rowStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLeaveSalary.DefaultCellStyle = rowStyle;

            this.dgvLeaveSalary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLeaveSalary.GridColor = System.Drawing.Color.FromArgb(232, 235, 242);
            this.dgvLeaveSalary.Location = new System.Drawing.Point(10, 62);
            this.dgvLeaveSalary.MultiSelect = false;
            this.dgvLeaveSalary.Name = "dgvLeaveSalary";
            this.dgvLeaveSalary.ReadOnly = true;
            this.dgvLeaveSalary.RowHeadersVisible = false;
            this.dgvLeaveSalary.RowTemplate.Height = 36;
            this.dgvLeaveSalary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dgvLeaveSalary.Size = new System.Drawing.Size(1126, 324);
            this.dgvLeaveSalary.TabIndex = 7;

            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(252, 252, 254);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.FromArgb(40, 53, 78);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(240, 236, 255);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(40, 53, 78);

            this.dgvLeaveSalary.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvLeaveSalary.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(232, 235, 242);

            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(244, 241, 252);
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.FromArgb(73, 46, 145);
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.Height = 46;

            this.dgvLeaveSalary.ThemeStyle.ReadOnly = true;

            this.dgvLeaveSalary.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(40, 53, 78);
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.Height = 36;
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 235, 255);
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(40, 53, 78);

            this.dgvLeaveSalary.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLeaveSalary_CellDoubleClick);

            // 
            // frmLeaveSalary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.ClientSize = new System.Drawing.Size(1202, 580);
            this.Controls.Add(this.pnlContentOuter);
            this.Controls.Add(this.pnlPageHeader);
            this.Controls.Add(this.headerUC1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1000, 520);
            this.Name = "frmLeaveSalary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "main";
            this.Text = "Leave Salary";
            this.Load += new System.EventHandler(this.frmLeaveSalary_Load);

            this.pnlPageHeader.ResumeLayout(false);
            this.pnlPageHeader.PerformLayout();
            this.pnlIcon.ResumeLayout(false);
            this.pnlContentOuter.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.pnlGridHeader.ResumeLayout(false);
            this.pnlGridHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeaveSalary)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private HeaderUC headerUC1;

        private System.Windows.Forms.Panel pnlPageHeader;
        private Guna.UI2.WinForms.Guna2Panel pnlIcon;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;

        private System.Windows.Forms.Panel pnlContentOuter;
        private Guna.UI2.WinForms.Guna2Panel panel3;
        private System.Windows.Forms.Panel pnlGridHeader;
        private System.Windows.Forms.Label lblGridIcon;
        private System.Windows.Forms.Label lblGridTitle;
        private System.Windows.Forms.Label lblGridSubtitle;

        private Guna.UI2.WinForms.Guna2DataGridView dgvLeaveSalary;
    }
}
