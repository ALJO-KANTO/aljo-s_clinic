namespace YamyProject
{
    partial class frmViewUomCategory
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.CheckBox chkActive;

        private System.Windows.Forms.DataGridView dgvUnits;

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCategory = new System.Windows.Forms.Label();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.dgvUnits = new System.Windows.Forms.DataGridView();
            this.colUnitId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRatio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colPrecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.headerUC1 = new YamyProject.HeaderUC();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnits)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCategory
            // 
            this.lblCategory.Location = new System.Drawing.Point(190, 40);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(403, 15);
            this.lblCategory.TabIndex = 0;
            this.lblCategory.Text = "Unit of Measure Category";
            // 
            // txtCategory
            // 
            this.txtCategory.Location = new System.Drawing.Point(193, 58);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Size = new System.Drawing.Size(400, 23);
            this.txtCategory.TabIndex = 1;
            // 
            // chkActive
            // 
            this.chkActive.Checked = true;
            this.chkActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActive.Location = new System.Drawing.Point(632, 57);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(104, 24);
            this.chkActive.TabIndex = 2;
            this.chkActive.Text = "Active";
            // 
            // dgvUnits
            // 
            this.dgvUnits.AllowUserToAddRows = false;
            this.dgvUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUnits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUnits.BackgroundColor = System.Drawing.Color.White;
            this.dgvUnits.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colUnitId,
            this.colUnit,
            this.colRatio,
            this.colUnitActive,
            this.colPrecision,
            this.colDelete});
            this.dgvUnits.Location = new System.Drawing.Point(30, 90);
            this.dgvUnits.Name = "dgvUnits";
            this.dgvUnits.RowHeadersVisible = false;
            this.dgvUnits.RowTemplate.Height = 35;
            this.dgvUnits.Size = new System.Drawing.Size(900, 350);
            this.dgvUnits.TabIndex = 3;
            this.dgvUnits.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvUnits_CellBeginEdit);
            this.dgvUnits.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUnits_CellContentClick);
            // 
            // colUnitId
            // 
            this.colUnitId.HeaderText = "";
            this.colUnitId.Name = "colUnitId";
            this.colUnitId.Visible = false;
            // 
            // colUnit
            // 
            this.colUnit.HeaderText = "Unit of Measure";
            this.colUnit.Name = "colUnit";
            // 
            // colRatio
            // 
            this.colRatio.HeaderText = "Ratio";
            this.colRatio.Name = "colRatio";
            // 
            // colUnitActive
            // 
            this.colUnitActive.HeaderText = "Active";
            this.colUnitActive.Name = "colUnitActive";
            // 
            // colPrecision
            // 
            this.colPrecision.HeaderText = "Rounding Precision";
            this.colPrecision.Name = "colPrecision";
            this.colPrecision.Visible = false;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "";
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "Delete";
            this.colDelete.UseColumnTextForButtonValue = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(30, 455);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 35);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "+ Add Unit";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(720, 455);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 38);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(830, 455);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 38);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // headerUC1
            // 
            this.headerUC1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(83)))));
            this.headerUC1.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerUC1.FormText = "";
            this.headerUC1.HeaderText = "";
            this.headerUC1.Location = new System.Drawing.Point(0, 0);
            this.headerUC1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.headerUC1.Name = "headerUC1";
            this.headerUC1.Size = new System.Drawing.Size(960, 35);
            this.headerUC1.TabIndex = 7;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(42)))), ((int)(((byte)(83)))));
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(2, 512);
            this.panel5.Margin = new System.Windows.Forms.Padding(2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(956, 3);
            this.panel5.TabIndex = 17;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(42)))), ((int)(((byte)(83)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(958, 35);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(2, 480);
            this.panel4.TabIndex = 16;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(42)))), ((int)(((byte)(83)))));
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(0, 35);
            this.panel6.Margin = new System.Windows.Forms.Padding(2);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(2, 480);
            this.panel6.TabIndex = 15;
            // 
            // frmViewUomCategory
            // 
            this.ClientSize = new System.Drawing.Size(960, 515);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.headerUC1);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.txtCategory);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.dgvUnits);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmViewUomCategory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unit of Measure Category";
            this.Load += new System.EventHandler(this.frmViewUomCategory_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnits)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRatio;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colUnitActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrecision;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private HeaderUC headerUC1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
    }
}