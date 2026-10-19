namespace YamyProject
{
    partial class frmPutawayRules
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAddRule;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.DataGridView dgvRules;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRuleId;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSourceLocation;
        private System.Windows.Forms.DataGridViewComboBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDestinationLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPriority;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colActive;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAddRule = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.dgvRules = new System.Windows.Forms.DataGridView();
            this.colRuleId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSourceLocation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colProduct = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCategory = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDestinationLocation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).BeginInit();
            this.SuspendLayout();

            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 60;
            this.pnlTop.BackColor = System.Drawing.Color.WhiteSmoke;

            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(14, 11);
            this.btnSave.Size = new System.Drawing.Size(55, 36);
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(112, 88, 166);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnAddRule.Text = "New";
            this.btnAddRule.Location = new System.Drawing.Point(76, 11);
            this.btnAddRule.Size = new System.Drawing.Size(60, 36);
            this.btnAddRule.Click += new System.EventHandler(this.btnAddRule_Click);

            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(143, 11);
            this.btnClose.Size = new System.Drawing.Size(70, 36);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            this.lblTitle.Text = "Locations / Putaway Rules";
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTitle.Location = new System.Drawing.Point(240, 20);

            this.lblCount.Text = "0 rules";
            this.lblCount.AutoSize = true;
            this.lblCount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblCount.Location = new System.Drawing.Point(1110, 22);

            this.pnlTop.Controls.Add(this.btnSave);
            this.pnlTop.Controls.Add(this.btnAddRule);
            this.pnlTop.Controls.Add(this.btnClose);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.lblCount);

            this.dgvRules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRules.BackgroundColor = System.Drawing.Color.White;
            this.dgvRules.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRules.AllowUserToAddRows = false;
            this.dgvRules.AllowUserToDeleteRows = false;
            this.dgvRules.RowHeadersVisible = false;
            this.dgvRules.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRules.ColumnHeadersHeight = 42;
            this.dgvRules.RowTemplate.Height = 42;
            this.dgvRules.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRules_CellContentClick);

            this.colRuleId.Name = "colRuleId";
            this.colRuleId.Visible = false;

            this.colSourceLocation.Name = "colSourceLocation";
            this.colSourceLocation.HeaderText = "When product arrives in";
            this.colSourceLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colSourceLocation.FillWeight = 130F;

            this.colProduct.Name = "colProduct";
            this.colProduct.HeaderText = "Product";
            this.colProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colProduct.FillWeight = 150F;

            this.colCategory.Name = "colCategory";
            this.colCategory.HeaderText = "Product Category";
            this.colCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colCategory.FillWeight = 120F;

            this.colDestinationLocation.Name = "colDestinationLocation";
            this.colDestinationLocation.HeaderText = "Store to sublocation";
            this.colDestinationLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colDestinationLocation.FillWeight = 140F;

            this.colPriority.Name = "colPriority";
            this.colPriority.HeaderText = "Priority";
            this.colPriority.FillWeight = 55F;

            this.colActive.Name = "colActive";
            this.colActive.HeaderText = "Active";
            this.colActive.FillWeight = 45F;

            this.colDelete.Name = "colDelete";
            this.colDelete.HeaderText = "";
            this.colDelete.Text = "Delete";
            this.colDelete.UseColumnTextForButtonValue = true;
            this.colDelete.FillWeight = 55F;

            this.dgvRules.Columns.AddRange(
                new System.Windows.Forms.DataGridViewColumn[]
                {
                    this.colRuleId,
                    this.colSourceLocation,
                    this.colProduct,
                    this.colCategory,
                    this.colDestinationLocation,
                    this.colPriority,
                    this.colActive,
                    this.colDelete
                });

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 620);
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.dgvRules);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "frmPutawayRules";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Putaway Rules";
            this.Load += new System.EventHandler(this.frmPutawayRules_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
