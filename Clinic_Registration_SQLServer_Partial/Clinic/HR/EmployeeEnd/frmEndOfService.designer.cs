namespace YamyProject
{
    partial class frmEndOfService
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
            System.Windows.Forms.DataGridViewCellStyle altStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle rowStyle = new System.Windows.Forms.DataGridViewCellStyle();

            this.headerUC1 = new YamyProject.HeaderUC();
            this.pnlPageHeader = new System.Windows.Forms.Panel();
            this.pnlIcon = new Guna.UI2.WinForms.Guna2Panel();
            this.lblIcon = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlContentOuter = new System.Windows.Forms.Panel();
            this.panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvLeaveSalary = new Guna.UI2.WinForms.Guna2DataGridView();

            this.pnlPageHeader.SuspendLayout();
            this.pnlIcon.SuspendLayout();
            this.pnlContentOuter.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeaveSalary)).BeginInit();
            this.SuspendLayout();

            // headerUC1
            this.headerUC1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerUC1.FormText = "";
            this.headerUC1.HeaderText = "End Of Service";
            this.headerUC1.Location = new System.Drawing.Point(0, 0);
            this.headerUC1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.headerUC1.Name = "headerUC1";
            this.headerUC1.Size = new System.Drawing.Size(1603, 38);
            this.headerUC1.TabIndex = 0;

            // pnlPageHeader
            this.pnlPageHeader.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlPageHeader.Controls.Add(this.pnlIcon);
            this.pnlPageHeader.Controls.Add(this.lblTitle);
            this.pnlPageHeader.Controls.Add(this.lblSubtitle);
            this.pnlPageHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPageHeader.Location = new System.Drawing.Point(0, 38);
            this.pnlPageHeader.Name = "pnlPageHeader";
            this.pnlPageHeader.Size = new System.Drawing.Size(1603, 132);
            this.pnlPageHeader.TabIndex = 1;

            // pnlIcon
            this.pnlIcon.BackColor = System.Drawing.Color.FromArgb(239, 233, 255);
            this.pnlIcon.BorderRadius = 13;
            this.pnlIcon.Controls.Add(this.lblIcon);
            this.pnlIcon.Location = new System.Drawing.Point(43, 24);
            this.pnlIcon.Name = "pnlIcon";
            this.pnlIcon.Size = new System.Drawing.Size(76, 76);
            this.pnlIcon.TabIndex = 0;

            // lblIcon
            this.lblIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 28F);
            this.lblIcon.ForeColor = System.Drawing.Color.FromArgb(92, 45, 210);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(76, 76);
            this.lblIcon.TabIndex = 0;
            this.lblIcon.Text = "";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 23F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(18, 32, 91);
            this.lblTitle.Location = new System.Drawing.Point(146, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "End Of Service";

            // lblSubtitle
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(91, 103, 151);
            this.lblSubtitle.Location = new System.Drawing.Point(149, 72);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "View and manage employee end of service details.";

            // pnlContentOuter
            this.pnlContentOuter.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.pnlContentOuter.Controls.Add(this.panel3);
            this.pnlContentOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContentOuter.Location = new System.Drawing.Point(0, 170);
            this.pnlContentOuter.Name = "pnlContentOuter";
            this.pnlContentOuter.Padding = new System.Windows.Forms.Padding(28, 8, 28, 28);
            this.pnlContentOuter.Size = new System.Drawing.Size(1603, 544);
            this.pnlContentOuter.TabIndex = 2;

            // panel3
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderColor = System.Drawing.Color.FromArgb(230, 232, 242);
            this.panel3.BorderRadius = 13;
            this.panel3.BorderThickness = 1;
            this.panel3.Controls.Add(this.dgvLeaveSalary);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(28, 8);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(18, 20, 18, 18);
            this.panel3.Size = new System.Drawing.Size(1547, 508);
            this.panel3.TabIndex = 8;

            // dgvLeaveSalary
            this.dgvLeaveSalary.AllowUserToAddRows = false;
            this.dgvLeaveSalary.AllowUserToDeleteRows = false;
            this.dgvLeaveSalary.AllowUserToResizeRows = false;
            this.dgvLeaveSalary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            altStyle.BackColor = System.Drawing.Color.FromArgb(252, 252, 254);
            altStyle.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            altStyle.SelectionBackColor = System.Drawing.Color.FromArgb(241, 238, 255);
            altStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvLeaveSalary.AlternatingRowsDefaultCellStyle = altStyle;

            this.dgvLeaveSalary.BackgroundColor = System.Drawing.Color.White;
            this.dgvLeaveSalary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLeaveSalary.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLeaveSalary.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = System.Drawing.Color.FromArgb(245, 242, 255);
            headerStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            headerStyle.ForeColor = System.Drawing.Color.FromArgb(76, 42, 190);
            headerStyle.SelectionBackColor = System.Drawing.Color.FromArgb(245, 242, 255);
            headerStyle.SelectionForeColor = System.Drawing.Color.FromArgb(76, 42, 190);
            headerStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLeaveSalary.ColumnHeadersDefaultCellStyle = headerStyle;
            this.dgvLeaveSalary.ColumnHeadersHeight = 48;
            this.dgvLeaveSalary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            rowStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            rowStyle.BackColor = System.Drawing.Color.White;
            rowStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            rowStyle.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            rowStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 236, 255);
            rowStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            rowStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLeaveSalary.DefaultCellStyle = rowStyle;

            this.dgvLeaveSalary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLeaveSalary.GridColor = System.Drawing.Color.FromArgb(230, 233, 244);
            this.dgvLeaveSalary.Location = new System.Drawing.Point(18, 20);
            this.dgvLeaveSalary.MultiSelect = false;
            this.dgvLeaveSalary.Name = "dgvLeaveSalary";
            this.dgvLeaveSalary.ReadOnly = true;
            this.dgvLeaveSalary.RowHeadersVisible = false;
            this.dgvLeaveSalary.RowTemplate.Height = 38;
            this.dgvLeaveSalary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dgvLeaveSalary.Size = new System.Drawing.Size(1511, 470);
            this.dgvLeaveSalary.TabIndex = 7;

            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(252, 252, 254);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(241, 238, 255);
            this.dgvLeaveSalary.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvLeaveSalary.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvLeaveSalary.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(230, 233, 244);
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(245, 242, 255);
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.FromArgb(76, 42, 190);
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLeaveSalary.ThemeStyle.HeaderStyle.Height = 48;
            this.dgvLeaveSalary.ThemeStyle.ReadOnly = true;
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(35, 48, 100);
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.Height = 38;
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(239, 236, 255);
            this.dgvLeaveSalary.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(35, 48, 100);

            this.dgvLeaveSalary.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLeaveSalary_CellDoubleClick);

            // frmEndOfService
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 253);
            this.ClientSize = new System.Drawing.Size(1603, 714);
            this.Controls.Add(this.pnlContentOuter);
            this.Controls.Add(this.pnlPageHeader);
            this.Controls.Add(this.headerUC1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MinimumSize = new System.Drawing.Size(1050, 560);
            this.Name = "frmEndOfService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "main";
            this.Text = "End Of Service";
            this.Load += new System.EventHandler(this.frmEndOfService_Load);

            this.pnlPageHeader.ResumeLayout(false);
            this.pnlPageHeader.PerformLayout();
            this.pnlIcon.ResumeLayout(false);
            this.pnlContentOuter.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
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
        private Guna.UI2.WinForms.Guna2DataGridView dgvLeaveSalary;
    }
}
