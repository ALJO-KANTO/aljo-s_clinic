namespace YamyProject
{
    partial class frmInventoryLocations
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnInventoryAtDate;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.DataGridView dgvLocations;

        private System.Windows.Forms.DataGridViewTextBoxColumn colStockId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWarehouseId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewButtonColumn colOnHand;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReserved;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewButtonColumn colHistory;
        private System.Windows.Forms.DataGridViewButtonColumn colAdjust;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnInventoryAtDate = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.dgvLocations = new System.Windows.Forms.DataGridView();

            this.colStockId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWarehouseId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocationId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOnHand = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colReserved = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistory = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colAdjust = new System.Windows.Forms.DataGridViewButtonColumn();

            ((System.ComponentModel.ISupportInitialize)(this.dgvLocations)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();

            // pnlTop
            this.pnlTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlTop.Controls.Add(this.btnNew);
            this.pnlTop.Controls.Add(this.btnInventoryAtDate);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.lblCount);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1200, 64);
            this.pnlTop.TabIndex = 0;

            // btnNew
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(112, 88, 166);
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(15, 15);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(55, 34);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;

            // btnInventoryAtDate
            this.btnInventoryAtDate.BackColor = System.Drawing.Color.FromArgb(112, 88, 166);
            this.btnInventoryAtDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInventoryAtDate.ForeColor = System.Drawing.Color.White;
            this.btnInventoryAtDate.Location = new System.Drawing.Point(78, 15);
            this.btnInventoryAtDate.Name = "btnInventoryAtDate";
            this.btnInventoryAtDate.Size = new System.Drawing.Size(130, 34);
            this.btnInventoryAtDate.TabIndex = 1;
            this.btnInventoryAtDate.Text = "Inventory at Date";
            this.btnInventoryAtDate.UseVisualStyleBackColor = false;
            this.btnInventoryAtDate.Click += new System.EventHandler(this.btnInventoryAtDate_Click);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTitle.Location = new System.Drawing.Point(218, 23);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(72, 19);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Locations";

            // txtSearch
            this.txtSearch.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearch.Location = new System.Drawing.Point(465, 17);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(440, 25);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // btnRefresh
            this.btnRefresh.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;
            this.btnRefresh.Location = new System.Drawing.Point(915, 16);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 30);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // lblCount
            this.lblCount.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(1010, 24);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(54, 15);
            this.lblCount.TabIndex = 5;
            this.lblCount.Text = "0 records";

            // dgvLocations
            this.dgvLocations.AllowUserToAddRows = false;
            this.dgvLocations.AllowUserToDeleteRows = false;
            this.dgvLocations.AllowUserToResizeRows = false;
            this.dgvLocations.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLocations.BackgroundColor = System.Drawing.Color.White;
            this.dgvLocations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLocations.ColumnHeadersHeight = 42;
            this.dgvLocations.Columns.AddRange(
                new System.Windows.Forms.DataGridViewColumn[]
                {
                    this.colStockId,
                    this.colWarehouseId,
                    this.colLocationId,
                    this.colItemId,
                    this.colLocation,
                    this.colProduct,
                    this.colOnHand,
                    this.colReserved,
                    this.colUnit,
                    this.colHistory,
                    this.colAdjust
                });
            this.dgvLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLocations.Location = new System.Drawing.Point(0, 64);
            this.dgvLocations.MultiSelect = false;
            this.dgvLocations.Name = "dgvLocations";
            this.dgvLocations.ReadOnly = true;
            this.dgvLocations.RowHeadersVisible = false;
            this.dgvLocations.RowTemplate.Height = 40;
            this.dgvLocations.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLocations.Size = new System.Drawing.Size(1200, 586);
            this.dgvLocations.TabIndex = 1;
            this.dgvLocations.CellContentClick +=
                new System.Windows.Forms.DataGridViewCellEventHandler(
                    this.dgvLocations_CellContentClick);

            // hidden ids
            this.colStockId.HeaderText = "StockId";
            this.colStockId.Name = "colStockId";
            this.colStockId.ReadOnly = true;
            this.colStockId.Visible = false;

            this.colWarehouseId.HeaderText = "WarehouseId";
            this.colWarehouseId.Name = "colWarehouseId";
            this.colWarehouseId.ReadOnly = true;
            this.colWarehouseId.Visible = false;

            this.colLocationId.HeaderText = "LocationId";
            this.colLocationId.Name = "colLocationId";
            this.colLocationId.ReadOnly = true;
            this.colLocationId.Visible = false;

            this.colItemId.HeaderText = "ItemId";
            this.colItemId.Name = "colItemId";
            this.colItemId.ReadOnly = true;
            this.colItemId.Visible = false;

            // visible columns
            this.colLocation.FillWeight = 95F;
            this.colLocation.HeaderText = "Location";
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;

            this.colProduct.FillWeight = 230F;
            this.colProduct.HeaderText = "Product";
            this.colProduct.Name = "colProduct";
            this.colProduct.ReadOnly = true;

            this.colOnHand.FillWeight = 90F;
            this.colOnHand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colOnHand.HeaderText = "On Hand Quantity";
            this.colOnHand.Name = "colOnHand";
            this.colOnHand.ReadOnly = true;
            this.colOnHand.UseColumnTextForButtonValue = false;

            this.colReserved.FillWeight = 90F;
            this.colReserved.HeaderText = "Reserved Quantity";
            this.colReserved.Name = "colReserved";
            this.colReserved.ReadOnly = true;

            this.colUnit.FillWeight = 55F;
            this.colUnit.HeaderText = "Unit";
            this.colUnit.Name = "colUnit";
            this.colUnit.ReadOnly = true;

            this.colHistory.FillWeight = 65F;
            this.colHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colHistory.HeaderText = "";
            this.colHistory.Name = "colHistory";
            this.colHistory.ReadOnly = true;
            this.colHistory.Text = "History";
            this.colHistory.UseColumnTextForButtonValue = true;

            this.colAdjust.FillWeight = 65F;
            this.colAdjust.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colAdjust.HeaderText = "";
            this.colAdjust.Name = "colAdjust";
            this.colAdjust.ReadOnly = true;
            this.colAdjust.Text = "Adjust";
            this.colAdjust.UseColumnTextForButtonValue = true;

            // frmInventoryLocations
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 650);
            this.Controls.Add(this.dgvLocations);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "frmInventoryLocations";
            this.Text = "Locations";
            this.Load += new System.EventHandler(this.frmInventoryLocations_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvLocations)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
