namespace YamyProject
{
    partial class frmInventoryLocationHistory
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnClose;

        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReference;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvHistory = new System.Windows.Forms.DataGridView();

            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReference = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();

            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.SuspendLayout();

            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 60;
            this.pnlTop.BackColor = System.Drawing.Color.WhiteSmoke;

            this.lblTitle.AutoSize = true;
            this.lblTitle.Text = "Locations";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTitle.Location = new System.Drawing.Point(12, 8);

            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Text = "Moves History";
            this.lblSubtitle.Location = new System.Drawing.Point(12, 34);

            this.lblCount.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(950, 22);
            this.lblCount.Text = "0 records";

            this.btnClose.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Text = "Close";
            this.btnClose.Location = new System.Drawing.Point(1070, 14);
            this.btnClose.Size = new System.Drawing.Size(80, 32);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.lblSubtitle);
            this.pnlTop.Controls.Add(this.lblCount);
            this.pnlTop.Controls.Add(this.btnClose);

            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersVisible = false;
            this.dgvHistory.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.ColumnHeadersHeight = 40;
            this.dgvHistory.RowTemplate.Height = 40;

            this.colDate.Name = "colDate";
            this.colDate.HeaderText = "Date";

            this.colReference.Name = "colReference";
            this.colReference.HeaderText = "Reference";

            this.colProduct.Name = "colProduct";
            this.colProduct.HeaderText = "Product";

            this.colFrom.Name = "colFrom";
            this.colFrom.HeaderText = "From";

            this.colTo.Name = "colTo";
            this.colTo.HeaderText = "To";

            this.colQuantity.Name = "colQuantity";
            this.colQuantity.HeaderText = "Quantity";

            this.colUnit.Name = "colUnit";
            this.colUnit.HeaderText = "Unit";

            this.colStatus.Name = "colStatus";
            this.colStatus.HeaderText = "Status";

            this.dgvHistory.Columns.AddRange(
                new System.Windows.Forms.DataGridViewColumn[]
                {
                    this.colDate,
                    this.colReference,
                    this.colProduct,
                    this.colFrom,
                    this.colTo,
                    this.colQuantity,
                    this.colUnit,
                    this.colStatus
                });

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1170, 620);
            this.Controls.Add(this.dgvHistory);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "frmInventoryLocationHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Moves History";
            this.Load += new System.EventHandler(this.frmInventoryLocationHistory_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
