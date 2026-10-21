namespace YamyProject.UI.Inventory.InventoryManagement.ItemUnit
{
    partial class frmItemUOMList
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
            System.Windows.Forms.DataGridViewCellStyle headerStyle =
                new System.Windows.Forms.DataGridViewCellStyle();

            System.Windows.Forms.DataGridViewCellStyle rowStyle =
                new System.Windows.Forms.DataGridViewCellStyle();

            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();

            this.pnlSearch = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();

            this.dgvItems = new System.Windows.Forms.DataGridView();

            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDefaultUnitId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDefaultUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlternativeUnits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUOM = new System.Windows.Forms.DataGridViewButtonColumn();

            this.pnlHeader.SuspendLayout();
            this.pnlSearch.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)(this.dgvItems))
                .BeginInit();

            this.SuspendLayout();

            // =====================================================
            // pnlHeader
            // =====================================================

            this.pnlHeader.BackColor =
                System.Drawing.Color.FromArgb(27, 52, 93);

            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1100, 60);
            this.pnlHeader.TabIndex = 0;

            // =====================================================
            // lblTitle
            // =====================================================

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    14F,
                    System.Drawing.FontStyle.Bold);

            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(199, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Item Unit of Measure";

            // =====================================================
            // pnlSearch
            // =====================================================

            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Controls.Add(this.btnRefresh);

            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 60);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding =
                new System.Windows.Forms.Padding(15);

            this.pnlSearch.Size = new System.Drawing.Size(1100, 70);
            this.pnlSearch.TabIndex = 1;

            // =====================================================
            // lblSearch
            // =====================================================

            this.lblSearch.AutoSize = true;
            this.lblSearch.Font =
                new System.Drawing.Font("Segoe UI", 9F);

            this.lblSearch.Location =
                new System.Drawing.Point(18, 26);

            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(45, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search";

            // =====================================================
            // txtSearch
            // =====================================================

            this.txtSearch.Font =
                new System.Drawing.Font("Segoe UI", 10F);

            this.txtSearch.Location =
                new System.Drawing.Point(75, 20);

            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size =
                new System.Drawing.Size(350, 25);

            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // =====================================================
            // btnRefresh
            // =====================================================

            this.btnRefresh.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                ((System.Windows.Forms.AnchorStyles.Top |
                  System.Windows.Forms.AnchorStyles.Right)));

            this.btnRefresh.BackColor =
                System.Drawing.Color.FromArgb(27, 52, 93);

            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnRefresh.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.btnRefresh.ForeColor =
                System.Drawing.Color.White;

            this.btnRefresh.Location =
                new System.Drawing.Point(965, 18);

            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size =
                new System.Drawing.Size(115, 32);

            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;

            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // =====================================================
            // dgvItems
            // =====================================================

            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AllowUserToResizeRows = false;

            this.dgvItems.BackgroundColor =
                System.Drawing.Color.White;

            this.dgvItems.BorderStyle =
                System.Windows.Forms.BorderStyle.None;

            this.dgvItems.CellBorderStyle =
                System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;

            this.dgvItems.ColumnHeadersBorderStyle =
                System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            headerStyle.Alignment =
                System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            headerStyle.BackColor =
                System.Drawing.Color.FromArgb(235, 240, 248);

            headerStyle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            headerStyle.ForeColor =
                System.Drawing.Color.FromArgb(30, 30, 30);

            headerStyle.SelectionBackColor =
                System.Drawing.Color.FromArgb(235, 240, 248);

            headerStyle.SelectionForeColor =
                System.Drawing.Color.FromArgb(30, 30, 30);

            headerStyle.WrapMode =
                System.Windows.Forms.DataGridViewTriState.True;

            this.dgvItems.ColumnHeadersDefaultCellStyle =
                headerStyle;

            this.dgvItems.ColumnHeadersHeight = 42;

            this.dgvItems.Columns.AddRange(
                new System.Windows.Forms.DataGridViewColumn[]
                {
                    this.colId,
                    this.colNo,
                    this.colCode,
                    this.colName,
                    this.colDefaultUnitId,
                    this.colDefaultUnit,
                    this.colAlternativeUnits,
                    this.colUOM
                });

            rowStyle.BackColor = System.Drawing.Color.White;
            rowStyle.Font =
                new System.Drawing.Font("Segoe UI", 9F);

            rowStyle.ForeColor =
                System.Drawing.Color.FromArgb(40, 40, 40);

            rowStyle.SelectionBackColor =
                System.Drawing.Color.FromArgb(220, 230, 245);

            rowStyle.SelectionForeColor =
                System.Drawing.Color.Black;

            this.dgvItems.DefaultCellStyle = rowStyle;

            this.dgvItems.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.dgvItems.EnableHeadersVisualStyles = false;
            this.dgvItems.Location =
                new System.Drawing.Point(0, 130);

            this.dgvItems.MultiSelect = false;
            this.dgvItems.Name = "dgvItems";

            this.dgvItems.ReadOnly = true;

            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.RowTemplate.Height = 38;

            this.dgvItems.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            this.dgvItems.Size =
                new System.Drawing.Size(1100, 520);

            this.dgvItems.TabIndex = 2;

            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentClick);

            // =====================================================
            // colId
            // =====================================================

            this.colId.HeaderText = "ID";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colId.Visible = false;

            // =====================================================
            // colNo
            // =====================================================

            this.colNo.HeaderText = "#";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 55;

            // =====================================================
            // colCode
            // =====================================================

            this.colCode.HeaderText = "Item Code";
            this.colCode.Name = "colCode";
            this.colCode.ReadOnly = true;
            this.colCode.Width = 140;

            // =====================================================
            // colName
            // =====================================================

            this.colName.AutoSizeMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            this.colName.HeaderText = "Item Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;

            // =====================================================
            // colDefaultUnitId
            // =====================================================

            this.colDefaultUnitId.HeaderText = "Default Unit ID";
            this.colDefaultUnitId.Name = "colDefaultUnitId";
            this.colDefaultUnitId.ReadOnly = true;
            this.colDefaultUnitId.Visible = false;

            // =====================================================
            // colDefaultUnit
            // =====================================================

            this.colDefaultUnit.HeaderText = "Default Unit";
            this.colDefaultUnit.Name = "colDefaultUnit";
            this.colDefaultUnit.ReadOnly = true;
            this.colDefaultUnit.Width = 150;

            // =====================================================
            // colAlternativeUnits
            // =====================================================

            this.colAlternativeUnits.HeaderText =
                "Alternative UOMs";

            this.colAlternativeUnits.Name =
                "colAlternativeUnits";

            this.colAlternativeUnits.ReadOnly = true;
            this.colAlternativeUnits.Width = 280;

            // =====================================================
            // colUOM
            // =====================================================

            this.colUOM.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.colUOM.HeaderText = "UOM";
            this.colUOM.Name = "colUOM";
            this.colUOM.ReadOnly = true;
            this.colUOM.Text = "Add / Edit UOM";
            this.colUOM.UseColumnTextForButtonValue = true;
            this.colUOM.Width = 130;

            // =====================================================
            // frmItemUOMList
            // =====================================================

            this.AutoScaleDimensions =
                new System.Drawing.SizeF(7F, 15F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.BackColor = System.Drawing.Color.White;

            this.ClientSize =
                new System.Drawing.Size(1100, 650);

            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlHeader);

            this.Font =
                new System.Drawing.Font("Segoe UI", 9F);

            this.FormBorderStyle =
                System.Windows.Forms.FormBorderStyle.None;

            this.Name = "frmItemUOMList";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Text = "Item Unit of Measure";

            this.Load += new System.EventHandler(this.frmItemUOMList_Load);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();

            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();

            ((System.ComponentModel.ISupportInitialize)(this.dgvItems))
                .EndInit();

            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;

        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.DataGridView dgvItems;

        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefaultUnitId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefaultUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlternativeUnits;
        private System.Windows.Forms.DataGridViewButtonColumn colUOM;
    }
}