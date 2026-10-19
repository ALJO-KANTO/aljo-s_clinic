namespace YamyProject
{
    partial class MasterCostCenter
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 =
                new System.Windows.Forms.DataGridViewCellStyle();

            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 =
                new System.Windows.Forms.DataGridViewCellStyle();

            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 =
                new System.Windows.Forms.DataGridViewCellStyle();

            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 =
                new System.Windows.Forms.DataGridViewCellStyle();

            this.headerUC1 = new YamyProject.HeaderUC();

            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();

            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.lblPageDescription = new System.Windows.Forms.Label();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();

            this.pnlGridCard = new System.Windows.Forms.Panel();
            this.pnlGridHeader = new System.Windows.Forms.Panel();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.lblGridDescription = new System.Windows.Forms.Label();

            this.dgvCustomer = new Guna.UI2.WinForms.Guna2DataGridView();

            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblFooterText = new System.Windows.Forms.Label();

            this.guna2DragControl1 =
                new Guna.UI2.WinForms.Guna2DragControl();

            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();

            this.pnlMain.SuspendLayout();
            this.pnlContent.SuspendLayout();

            this.pnlTop.SuspendLayout();
            this.pnlGridCard.SuspendLayout();
            this.pnlGridHeader.SuspendLayout();

            this.pnlFooter.SuspendLayout();

            this.SuspendLayout();

            // =========================================================
            // HEADER
            // =========================================================

            this.headerUC1.BackColor =
                System.Drawing.Color.FromArgb(43, 27, 103);

            this.headerUC1.Dock =
                System.Windows.Forms.DockStyle.Top;

            this.headerUC1.FormText =
                "Master Cost Center";

            this.headerUC1.HeaderText =
                "";

            this.headerUC1.Location =
                new System.Drawing.Point(0, 0);

            this.headerUC1.Margin =
                new System.Windows.Forms.Padding(0);

            this.headerUC1.Name =
                "headerUC1";

            this.headerUC1.Size =
                new System.Drawing.Size(1400, 45);

            this.headerUC1.TabIndex =
                0;

            // =========================================================
            // MAIN BACKGROUND
            // =========================================================

            this.pnlMain.BackColor =
                System.Drawing.Color.FromArgb(246, 245, 250);

            this.pnlMain.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.pnlMain.Location =
                new System.Drawing.Point(0, 45);

            this.pnlMain.Name =
                "pnlMain";

            this.pnlMain.Padding =
                new System.Windows.Forms.Padding(24, 22, 24, 18);

            this.pnlMain.Size =
                new System.Drawing.Size(1400, 755);

            this.pnlMain.TabIndex =
                1;

            this.pnlMain.Controls.Add(
                this.pnlContent);

            // =========================================================
            // CONTENT
            // =========================================================

            this.pnlContent.BackColor =
                System.Drawing.Color.Transparent;

            this.pnlContent.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.pnlContent.Location =
                new System.Drawing.Point(24, 22);

            this.pnlContent.Name =
                "pnlContent";

            this.pnlContent.Size =
                new System.Drawing.Size(1352, 715);

            this.pnlContent.TabIndex =
                0;

            this.pnlContent.Controls.Add(
                this.pnlGridCard);

            this.pnlContent.Controls.Add(
                this.pnlFooter);

            this.pnlContent.Controls.Add(
                this.pnlTop);

            // =========================================================
            // TOP SECTION
            // =========================================================

            this.pnlTop.BackColor =
                System.Drawing.Color.Transparent;

            this.pnlTop.Dock =
                System.Windows.Forms.DockStyle.Top;

            this.pnlTop.Location =
                new System.Drawing.Point(0, 0);

            this.pnlTop.Name =
                "pnlTop";

            this.pnlTop.Size =
                new System.Drawing.Size(1352, 100);

            this.pnlTop.TabIndex =
                0;

            this.pnlTop.Controls.Add(
                this.lblPageTitle);

            this.pnlTop.Controls.Add(
                this.lblPageDescription);

            this.pnlTop.Controls.Add(
                this.btnSave);

            // =========================================================
            // PAGE TITLE
            // =========================================================

            this.lblPageTitle.AutoSize =
                true;

            this.lblPageTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    20F,
                    System.Drawing.FontStyle.Bold,
                    System.Drawing.GraphicsUnit.Point,
                    ((byte)(0)));

            this.lblPageTitle.ForeColor =
                System.Drawing.Color.FromArgb(43, 35, 75);

            this.lblPageTitle.Location =
                new System.Drawing.Point(4, 4);

            this.lblPageTitle.Name =
                "lblPageTitle";

            this.lblPageTitle.Size =
                new System.Drawing.Size(310, 37);

            this.lblPageTitle.TabIndex =
                0;

            this.lblPageTitle.Text =
                "Cost Center Management";

            // =========================================================
            // PAGE DESCRIPTION
            // =========================================================

            this.lblPageDescription.AutoSize =
                true;

            this.lblPageDescription.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F,
                    System.Drawing.FontStyle.Regular,
                    System.Drawing.GraphicsUnit.Point,
                    ((byte)(0)));

            this.lblPageDescription.ForeColor =
                System.Drawing.Color.FromArgb(112, 106, 130);

            this.lblPageDescription.Location =
                new System.Drawing.Point(7, 49);

            this.lblPageDescription.Name =
                "lblPageDescription";

            this.lblPageDescription.Size =
                new System.Drawing.Size(430, 17);

            this.lblPageDescription.TabIndex =
                1;

            this.lblPageDescription.Text =
                "Create, organize and manage your accounting cost centers.";

            // =========================================================
            // NEW COST CENTER BUTTON
            // =========================================================

            this.btnSave.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                ((System.Windows.Forms.AnchorStyles.Top |
                  System.Windows.Forms.AnchorStyles.Right)));

            this.btnSave.Animated =
                true;

            this.btnSave.BorderRadius =
                9;

            this.btnSave.FillColor =
                System.Drawing.Color.FromArgb(104, 55, 215);

            this.btnSave.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F,
                    System.Drawing.FontStyle.Bold,
                    System.Drawing.GraphicsUnit.Point,
                    ((byte)(0)));

            this.btnSave.ForeColor =
                System.Drawing.Color.White;

            this.btnSave.HoverState.FillColor =
                System.Drawing.Color.FromArgb(118, 67, 229);

            this.btnSave.HoverState.ForeColor =
                System.Drawing.Color.White;

            this.btnSave.Location =
                new System.Drawing.Point(1157, 23);

            this.btnSave.Name =
                "btnSave";

            this.btnSave.ShadowDecoration.BorderRadius =
                9;

            this.btnSave.ShadowDecoration.Enabled =
                true;

            this.btnSave.ShadowDecoration.Depth =
                3;

            this.btnSave.ShadowDecoration.Shadow =
                new System.Windows.Forms.Padding(0, 2, 0, 4);

            this.btnSave.Size =
                new System.Drawing.Size(195, 48);

            this.btnSave.TabIndex =
                2;

            this.btnSave.Text =
                "+  New Cost Center";

            this.btnSave.Click +=
                new System.EventHandler(
                    this.btnSave_Click);

            // =========================================================
            // GRID CARD
            // =========================================================

            this.pnlGridCard.BackColor =
                System.Drawing.Color.White;

            this.pnlGridCard.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.pnlGridCard.Location =
                new System.Drawing.Point(0, 100);

            this.pnlGridCard.Name =
                "pnlGridCard";

            this.pnlGridCard.Padding =
                new System.Windows.Forms.Padding(0);

            this.pnlGridCard.Size =
                new System.Drawing.Size(1352, 565);

            this.pnlGridCard.TabIndex =
                1;

            this.pnlGridCard.Controls.Add(
                this.dgvCustomer);

            this.pnlGridCard.Controls.Add(
                this.pnlGridHeader);

            // =========================================================
            // GRID HEADER
            // =========================================================

            this.pnlGridHeader.BackColor =
                System.Drawing.Color.White;

            this.pnlGridHeader.Dock =
                System.Windows.Forms.DockStyle.Top;

            this.pnlGridHeader.Location =
                new System.Drawing.Point(0, 0);

            this.pnlGridHeader.Name =
                "pnlGridHeader";

            this.pnlGridHeader.Size =
                new System.Drawing.Size(1352, 70);

            this.pnlGridHeader.TabIndex =
                0;

            this.pnlGridHeader.Controls.Add(
                this.lblGridTitle);

            this.pnlGridHeader.Controls.Add(
                this.lblGridDescription);

            // =========================================================
            // GRID TITLE
            // =========================================================

            this.lblGridTitle.AutoSize =
                true;

            this.lblGridTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    11F,
                    System.Drawing.FontStyle.Bold,
                    System.Drawing.GraphicsUnit.Point,
                    ((byte)(0)));

            this.lblGridTitle.ForeColor =
                System.Drawing.Color.FromArgb(48, 43, 69);

            this.lblGridTitle.Location =
                new System.Drawing.Point(20, 12);

            this.lblGridTitle.Name =
                "lblGridTitle";

            this.lblGridTitle.Size =
                new System.Drawing.Size(151, 20);

            this.lblGridTitle.TabIndex =
                0;

            this.lblGridTitle.Text =
                "Cost Centers";

            // =========================================================
            // GRID DESCRIPTION
            // =========================================================

            this.lblGridDescription.AutoSize =
                true;

            this.lblGridDescription.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8.5F,
                    System.Drawing.FontStyle.Regular,
                    System.Drawing.GraphicsUnit.Point,
                    ((byte)(0)));

            this.lblGridDescription.ForeColor =
                System.Drawing.Color.FromArgb(130, 124, 148);

            this.lblGridDescription.Location =
                new System.Drawing.Point(21, 38);

            this.lblGridDescription.Name =
                "lblGridDescription";

            this.lblGridDescription.Size =
                new System.Drawing.Size(395, 15);

            this.lblGridDescription.TabIndex =
                1;

            this.lblGridDescription.Text =
                "Double-click a cost center to view or edit details.";

            // =========================================================
            // DATA GRID
            // =========================================================

            this.dgvCustomer.AllowUserToAddRows =
                false;

            this.dgvCustomer.AllowUserToDeleteRows =
                false;

            this.dgvCustomer.AllowUserToResizeColumns =
                false;

            this.dgvCustomer.AllowUserToResizeRows =
                false;

            this.dgvCustomer.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                (((System.Windows.Forms.AnchorStyles.Top |
                   System.Windows.Forms.AnchorStyles.Bottom) |
                  System.Windows.Forms.AnchorStyles.Left) |
                  System.Windows.Forms.AnchorStyles.Right));

            // =========================================================
            // ALTERNATING ROW
            // =========================================================

            dataGridViewCellStyle1.BackColor =
                System.Drawing.Color.FromArgb(250, 249, 253);

            dataGridViewCellStyle1.ForeColor =
                System.Drawing.Color.FromArgb(65, 59, 82);

            this.dgvCustomer.AlternatingRowsDefaultCellStyle =
                dataGridViewCellStyle1;

            // =========================================================
            // GRID GENERAL
            // =========================================================

            this.dgvCustomer.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;

            this.dgvCustomer.BackgroundColor =
                System.Drawing.Color.White;

            this.dgvCustomer.BorderStyle =
                System.Windows.Forms.BorderStyle.None;

            this.dgvCustomer.CellBorderStyle =
                System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;

            // =========================================================
            // HEADER STYLE
            // =========================================================

            dataGridViewCellStyle2.Alignment =
                System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            dataGridViewCellStyle2.BackColor =
                System.Drawing.Color.FromArgb(247, 245, 251);

            dataGridViewCellStyle2.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8.5F,
                    System.Drawing.FontStyle.Bold);

            dataGridViewCellStyle2.ForeColor =
                System.Drawing.Color.FromArgb(72, 65, 95);

            dataGridViewCellStyle2.SelectionBackColor =
                System.Drawing.Color.FromArgb(247, 245, 251);

            dataGridViewCellStyle2.SelectionForeColor =
                System.Drawing.Color.FromArgb(72, 65, 95);

            dataGridViewCellStyle2.WrapMode =
                System.Windows.Forms.DataGridViewTriState.False;

            this.dgvCustomer.ColumnHeadersDefaultCellStyle =
                dataGridViewCellStyle2;

            this.dgvCustomer.ColumnHeadersHeight =
                42;

            this.dgvCustomer.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // =========================================================
            // DEFAULT CELL STYLE
            // =========================================================

            dataGridViewCellStyle3.Alignment =
                System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            dataGridViewCellStyle3.BackColor =
                System.Drawing.Color.White;

            dataGridViewCellStyle3.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Regular);

            dataGridViewCellStyle3.ForeColor =
                System.Drawing.Color.FromArgb(65, 59, 82);

            dataGridViewCellStyle3.SelectionBackColor =
                System.Drawing.Color.FromArgb(239, 234, 255);

            dataGridViewCellStyle3.SelectionForeColor =
                System.Drawing.Color.FromArgb(52, 42, 82);

            dataGridViewCellStyle3.WrapMode =
                System.Windows.Forms.DataGridViewTriState.False;

            this.dgvCustomer.DefaultCellStyle =
                dataGridViewCellStyle3;

            // =========================================================
            // GRID
            // =========================================================

            this.dgvCustomer.GridColor =
                System.Drawing.Color.FromArgb(238, 235, 245);

            this.dgvCustomer.Location =
                new System.Drawing.Point(0, 70);

            this.dgvCustomer.MultiSelect =
                false;

            this.dgvCustomer.Name =
                "dgvCustomer";

            this.dgvCustomer.ReadOnly =
                true;

            this.dgvCustomer.RowHeadersBorderStyle =
                System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            this.dgvCustomer.RowHeadersDefaultCellStyle =
                dataGridViewCellStyle4;

            this.dgvCustomer.RowHeadersVisible =
                false;

            this.dgvCustomer.RowHeadersWidth =
                51;

            this.dgvCustomer.RowTemplate.Height =
                38;

            this.dgvCustomer.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            this.dgvCustomer.Size =
                new System.Drawing.Size(1352, 495);

            this.dgvCustomer.TabIndex =
                1;

            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.BackColor =
                System.Drawing.Color.FromArgb(250, 249, 253);

            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.Font =
                null;

            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.ForeColor =
                System.Drawing.Color.Empty;

            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.SelectionBackColor =
                System.Drawing.Color.Empty;

            this.dgvCustomer.ThemeStyle.AlternatingRowsStyle.SelectionForeColor =
                System.Drawing.Color.Empty;

            this.dgvCustomer.ThemeStyle.BackColor =
                System.Drawing.Color.White;

            this.dgvCustomer.ThemeStyle.GridColor =
                System.Drawing.Color.FromArgb(238, 235, 245);

            this.dgvCustomer.ThemeStyle.HeaderStyle.BackColor =
                System.Drawing.Color.FromArgb(247, 245, 251);

            this.dgvCustomer.ThemeStyle.HeaderStyle.BorderStyle =
                System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            this.dgvCustomer.ThemeStyle.HeaderStyle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8.5F,
                    System.Drawing.FontStyle.Bold);

            this.dgvCustomer.ThemeStyle.HeaderStyle.ForeColor =
                System.Drawing.Color.FromArgb(72, 65, 95);

            this.dgvCustomer.ThemeStyle.HeaderStyle.HeaightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            this.dgvCustomer.ThemeStyle.HeaderStyle.Height =
                42;

            this.dgvCustomer.ThemeStyle.ReadOnly =
                true;

            this.dgvCustomer.ThemeStyle.RowsStyle.BackColor =
                System.Drawing.Color.White;

            this.dgvCustomer.ThemeStyle.RowsStyle.BorderStyle =
                System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;

            this.dgvCustomer.ThemeStyle.RowsStyle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F);

            this.dgvCustomer.ThemeStyle.RowsStyle.ForeColor =
                System.Drawing.Color.FromArgb(65, 59, 82);

            this.dgvCustomer.ThemeStyle.RowsStyle.Height =
                38;

            this.dgvCustomer.ThemeStyle.RowsStyle.SelectionBackColor =
                System.Drawing.Color.FromArgb(239, 234, 255);

            this.dgvCustomer.ThemeStyle.RowsStyle.SelectionForeColor =
                System.Drawing.Color.FromArgb(52, 42, 82);

            // =========================================================
            // EXISTING EVENTS
            // =========================================================

            this.dgvCustomer.CellDoubleClick +=
                new System.Windows.Forms.DataGridViewCellEventHandler(
                    this.dgvCustomer_CellDoubleClick);

            this.dgvCustomer.CellFormatting +=
                new System.Windows.Forms.DataGridViewCellFormattingEventHandler(
                    this.dgvCustomer_CellFormatting);

            this.dgvCustomer.MouseMove +=
                new System.Windows.Forms.MouseEventHandler(
                    this.dgvCustomer_MouseMove);

            // =========================================================
            // FOOTER
            // =========================================================

            this.pnlFooter.BackColor =
                System.Drawing.Color.White;

            this.pnlFooter.Dock =
                System.Windows.Forms.DockStyle.Bottom;

            this.pnlFooter.Location =
                new System.Drawing.Point(0, 665);

            this.pnlFooter.Name =
                "pnlFooter";

            this.pnlFooter.Padding =
                new System.Windows.Forms.Padding(20, 0, 20, 0);

            this.pnlFooter.Size =
                new System.Drawing.Size(1352, 50);

            this.pnlFooter.TabIndex =
                2;

            this.pnlFooter.Controls.Add(
                this.lblFooterText);

            // =========================================================
            // FOOTER TEXT
            // =========================================================

            this.lblFooterText.AutoSize =
                true;

            this.lblFooterText.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8.5F);

            this.lblFooterText.ForeColor =
                System.Drawing.Color.FromArgb(135, 128, 150);

            this.lblFooterText.Location =
                new System.Drawing.Point(20, 17);

            this.lblFooterText.Name =
                "lblFooterText";

            this.lblFooterText.Size =
                new System.Drawing.Size(430, 15);

            this.lblFooterText.TabIndex =
                0;

            this.lblFooterText.Text =
                "Tip: Double-click a cost center to view or edit it.";

            // =========================================================
            // DRAG CONTROL
            // =========================================================

            this.guna2DragControl1.DockIndicatorTransparencyValue =
                0.6D;

            this.guna2DragControl1.UseTransparentDrag =
                true;

            // =========================================================
            // FORM
            // =========================================================

            this.AutoScaleDimensions =
                new System.Drawing.SizeF(8F, 16F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.BackColor =
                System.Drawing.Color.FromArgb(246, 245, 250);

            this.ClientSize =
                new System.Drawing.Size(1400, 800);

            this.Controls.Add(
                this.pnlMain);

            this.Controls.Add(
                this.headerUC1);

            this.FormBorderStyle =
                System.Windows.Forms.FormBorderStyle.None;

            this.Margin =
                new System.Windows.Forms.Padding(0);

            this.Name =
                "MasterCostCenter";

            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Tag =
                "main";

            this.Text =
                "Master Cost Center";

            this.Load +=
                new System.EventHandler(
                    this.MasterCostCenter_Load);

            this.DoubleClick +=
                new System.EventHandler(
                    this.MasterCostCenter_DoubleClick);

            // =========================================================
            // RESUME
            // =========================================================

            ((System.ComponentModel.ISupportInitialize)
                (this.dgvCustomer)).EndInit();

            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();

            this.pnlGridHeader.ResumeLayout(false);
            this.pnlGridHeader.PerformLayout();

            this.pnlGridCard.ResumeLayout(false);

            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();

            this.pnlContent.ResumeLayout(false);

            this.pnlMain.ResumeLayout(false);

            this.ResumeLayout(false);
        }

        #endregion

        // =============================================================
        // CONTROLS
        // =============================================================

        private Guna.UI2.WinForms.Guna2DataGridView dgvCustomer;

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlContent;

        private System.Windows.Forms.Panel pnlTop;

        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Label lblPageDescription;

        private Guna.UI2.WinForms.Guna2Button btnSave;

        private System.Windows.Forms.Panel pnlGridCard;
        private System.Windows.Forms.Panel pnlGridHeader;

        private System.Windows.Forms.Label lblGridTitle;
        private System.Windows.Forms.Label lblGridDescription;

        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblFooterText;

        private HeaderUC headerUC1;

        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
    }
}