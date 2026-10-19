using System.Windows.Forms;

namespace YamyProject.UI.Inventory.StockReconciliation
{
    partial class frmStockReconciliation
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

        private Panel pnlTop;
        private Label lblTitle;

        private GroupBox grpHeader;
        private GroupBox grpItems;
        private GroupBox grpSummary;

        private TextBox txtNo;
        private DateTimePicker dtDate;

        private ComboBox cmbWarehouse;
        private ComboBox cmbProject;

        private ComboBox cmbType;

        private ComboBox cmbCheckedBy;

        private ComboBox cmbStatus;

        private TextBox txtRemarks;

        private ComboBox cmbItem;

        private TextBox txtSystemQty;
        private TextBox txtPhysicalQty;
        private TextBox txtVariance;
        private TextBox txtUom;
        private TextBox txtCost;
        private TextBox txtVarianceValue;

        private Button btnAddItem;
        private Button btnLoadStock;

        private DataGridView dgvItems;

        private TextBox txtPositiveVariance;
        private TextBox txtNegativeVariance;

        private Button btnNew;
        private Button btnSave;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnApprove;
        private Button btnClear;

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.grpHeader = new System.Windows.Forms.GroupBox();
            this.h1 = new System.Windows.Forms.Label();
            this.txtNo = new System.Windows.Forms.TextBox();
            this.h2 = new System.Windows.Forms.Label();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.h3 = new System.Windows.Forms.Label();
            this.cmbWarehouse = new System.Windows.Forms.ComboBox();
            this.h4 = new System.Windows.Forms.Label();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.h5 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.h6 = new System.Windows.Forms.Label();
            this.cmbCheckedBy = new System.Windows.Forms.ComboBox();
            this.h7 = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.h8 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.grpItems = new System.Windows.Forms.GroupBox();
            this.i1 = new System.Windows.Forms.Label();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.i2 = new System.Windows.Forms.Label();
            this.txtSystemQty = new System.Windows.Forms.TextBox();
            this.i3 = new System.Windows.Forms.Label();
            this.txtPhysicalQty = new System.Windows.Forms.TextBox();
            this.i4 = new System.Windows.Forms.Label();
            this.txtVariance = new System.Windows.Forms.TextBox();
            this.i5 = new System.Windows.Forms.Label();
            this.txtUom = new System.Windows.Forms.TextBox();
            this.i6 = new System.Windows.Forms.Label();
            this.txtCost = new System.Windows.Forms.TextBox();
            this.i7 = new System.Windows.Forms.Label();
            this.txtVarianceValue = new System.Windows.Forms.TextBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnLoadStock = new System.Windows.Forms.Button();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.grpSummary = new System.Windows.Forms.GroupBox();
            this.s1 = new System.Windows.Forms.Label();
            this.txtPositiveVariance = new System.Windows.Forms.TextBox();
            this.s2 = new System.Windows.Forms.Label();
            this.txtNegativeVariance = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.grpHeader.SuspendLayout();
            this.grpItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.grpSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1184, 55);
            this.pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(231, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "STOCK RECONCILIATION";
            // 
            // grpHeader
            // 
            this.grpHeader.Controls.Add(this.h1);
            this.grpHeader.Controls.Add(this.txtNo);
            this.grpHeader.Controls.Add(this.h2);
            this.grpHeader.Controls.Add(this.dtDate);
            this.grpHeader.Controls.Add(this.h3);
            this.grpHeader.Controls.Add(this.cmbWarehouse);
            this.grpHeader.Controls.Add(this.h4);
            this.grpHeader.Controls.Add(this.cmbProject);
            this.grpHeader.Controls.Add(this.h5);
            this.grpHeader.Controls.Add(this.cmbType);
            this.grpHeader.Controls.Add(this.h6);
            this.grpHeader.Controls.Add(this.cmbCheckedBy);
            this.grpHeader.Controls.Add(this.h7);
            this.grpHeader.Controls.Add(this.cmbStatus);
            this.grpHeader.Controls.Add(this.h8);
            this.grpHeader.Controls.Add(this.txtRemarks);
            this.grpHeader.Location = new System.Drawing.Point(10, 65);
            this.grpHeader.Name = "grpHeader";
            this.grpHeader.Size = new System.Drawing.Size(1160, 170);
            this.grpHeader.TabIndex = 1;
            this.grpHeader.TabStop = false;
            this.grpHeader.Text = "Reconciliation Header";
            // 
            // h1
            // 
            this.h1.Location = new System.Drawing.Point(15, 30);
            this.h1.Name = "h1";
            this.h1.Size = new System.Drawing.Size(65, 25);
            this.h1.TabIndex = 0;
            this.h1.Text = "Recon No";
            // 
            // txtNo
            // 
            this.txtNo.Location = new System.Drawing.Point(85, 30);
            this.txtNo.Name = "txtNo";
            this.txtNo.Size = new System.Drawing.Size(150, 20);
            this.txtNo.TabIndex = 1;
            // 
            // h2
            // 
            this.h2.Location = new System.Drawing.Point(255, 30);
            this.h2.Name = "h2";
            this.h2.Size = new System.Drawing.Size(40, 25);
            this.h2.TabIndex = 2;
            this.h2.Text = "Date";
            // 
            // dtDate
            // 
            this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtDate.Location = new System.Drawing.Point(300, 30);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(150, 20);
            this.dtDate.TabIndex = 3;
            // 
            // h3
            // 
            this.h3.Location = new System.Drawing.Point(470, 30);
            this.h3.Name = "h3";
            this.h3.Size = new System.Drawing.Size(75, 25);
            this.h3.TabIndex = 4;
            this.h3.Text = "Warehouse";
            // 
            // cmbWarehouse
            // 
            this.cmbWarehouse.Location = new System.Drawing.Point(550, 30);
            this.cmbWarehouse.Name = "cmbWarehouse";
            this.cmbWarehouse.Size = new System.Drawing.Size(220, 21);
            this.cmbWarehouse.TabIndex = 5;
            // 
            // h4
            // 
            this.h4.Location = new System.Drawing.Point(790, 30);
            this.h4.Name = "h4";
            this.h4.Size = new System.Drawing.Size(50, 25);
            this.h4.TabIndex = 6;
            this.h4.Text = "Project";
            // 
            // cmbProject
            // 
            this.cmbProject.Location = new System.Drawing.Point(845, 30);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(250, 21);
            this.cmbProject.TabIndex = 7;
            // 
            // h5
            // 
            this.h5.Location = new System.Drawing.Point(15, 70);
            this.h5.Name = "h5";
            this.h5.Size = new System.Drawing.Size(40, 25);
            this.h5.TabIndex = 8;
            this.h5.Text = "Type";
            // 
            // cmbType
            // 
            this.cmbType.Location = new System.Drawing.Point(60, 70);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(180, 21);
            this.cmbType.TabIndex = 9;
            // 
            // h6
            // 
            this.h6.Location = new System.Drawing.Point(260, 70);
            this.h6.Name = "h6";
            this.h6.Size = new System.Drawing.Size(60, 25);
            this.h6.TabIndex = 10;
            this.h6.Text = "Checked";
            // 
            // cmbCheckedBy
            // 
            this.cmbCheckedBy.Location = new System.Drawing.Point(325, 70);
            this.cmbCheckedBy.Name = "cmbCheckedBy";
            this.cmbCheckedBy.Size = new System.Drawing.Size(220, 21);
            this.cmbCheckedBy.TabIndex = 11;
            // 
            // h7
            // 
            this.h7.Location = new System.Drawing.Point(565, 70);
            this.h7.Name = "h7";
            this.h7.Size = new System.Drawing.Size(50, 25);
            this.h7.TabIndex = 12;
            this.h7.Text = "Status";
            // 
            // cmbStatus
            // 
            this.cmbStatus.Location = new System.Drawing.Point(620, 70);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(150, 21);
            this.cmbStatus.TabIndex = 13;
            // 
            // h8
            // 
            this.h8.Location = new System.Drawing.Point(790, 70);
            this.h8.Name = "h8";
            this.h8.Size = new System.Drawing.Size(60, 25);
            this.h8.TabIndex = 14;
            this.h8.Text = "Remarks";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Location = new System.Drawing.Point(855, 70);
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(240, 20);
            this.txtRemarks.TabIndex = 15;
            // 
            // grpItems
            // 
            this.grpItems.Controls.Add(this.i1);
            this.grpItems.Controls.Add(this.cmbItem);
            this.grpItems.Controls.Add(this.i2);
            this.grpItems.Controls.Add(this.txtSystemQty);
            this.grpItems.Controls.Add(this.i3);
            this.grpItems.Controls.Add(this.txtPhysicalQty);
            this.grpItems.Controls.Add(this.i4);
            this.grpItems.Controls.Add(this.txtVariance);
            this.grpItems.Controls.Add(this.i5);
            this.grpItems.Controls.Add(this.txtUom);
            this.grpItems.Controls.Add(this.i6);
            this.grpItems.Controls.Add(this.txtCost);
            this.grpItems.Controls.Add(this.i7);
            this.grpItems.Controls.Add(this.txtVarianceValue);
            this.grpItems.Controls.Add(this.btnAddItem);
            this.grpItems.Controls.Add(this.btnLoadStock);
            this.grpItems.Controls.Add(this.dgvItems);
            this.grpItems.Location = new System.Drawing.Point(10, 240);
            this.grpItems.Name = "grpItems";
            this.grpItems.Size = new System.Drawing.Size(1160, 310);
            this.grpItems.TabIndex = 2;
            this.grpItems.TabStop = false;
            this.grpItems.Text = "Stock Items";
            // 
            // i1
            // 
            this.i1.Location = new System.Drawing.Point(15, 30);
            this.i1.Name = "i1";
            this.i1.Size = new System.Drawing.Size(40, 25);
            this.i1.TabIndex = 0;
            this.i1.Text = "Item";
            // 
            // cmbItem
            // 
            this.cmbItem.Location = new System.Drawing.Point(60, 25);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(240, 21);
            this.cmbItem.TabIndex = 1;
            // 
            // i2
            // 
            this.i2.Location = new System.Drawing.Point(315, 30);
            this.i2.Name = "i2";
            this.i2.Size = new System.Drawing.Size(50, 25);
            this.i2.TabIndex = 2;
            this.i2.Text = "System";
            // 
            // txtSystemQty
            // 
            this.txtSystemQty.Location = new System.Drawing.Point(370, 25);
            this.txtSystemQty.Name = "txtSystemQty";
            this.txtSystemQty.Size = new System.Drawing.Size(70, 20);
            this.txtSystemQty.TabIndex = 3;
            // 
            // i3
            // 
            this.i3.Location = new System.Drawing.Point(455, 30);
            this.i3.Name = "i3";
            this.i3.Size = new System.Drawing.Size(55, 25);
            this.i3.TabIndex = 4;
            this.i3.Text = "Physical";
            // 
            // txtPhysicalQty
            // 
            this.txtPhysicalQty.Location = new System.Drawing.Point(515, 25);
            this.txtPhysicalQty.Name = "txtPhysicalQty";
            this.txtPhysicalQty.Size = new System.Drawing.Size(70, 20);
            this.txtPhysicalQty.TabIndex = 5;
            // 
            // i4
            // 
            this.i4.Location = new System.Drawing.Point(600, 30);
            this.i4.Name = "i4";
            this.i4.Size = new System.Drawing.Size(55, 25);
            this.i4.TabIndex = 6;
            this.i4.Text = "Variance";
            // 
            // txtVariance
            // 
            this.txtVariance.Location = new System.Drawing.Point(660, 25);
            this.txtVariance.Name = "txtVariance";
            this.txtVariance.Size = new System.Drawing.Size(70, 20);
            this.txtVariance.TabIndex = 7;
            // 
            // i5
            // 
            this.i5.Location = new System.Drawing.Point(745, 30);
            this.i5.Name = "i5";
            this.i5.Size = new System.Drawing.Size(40, 25);
            this.i5.TabIndex = 8;
            this.i5.Text = "UOM";
            // 
            // txtUom
            // 
            this.txtUom.Location = new System.Drawing.Point(790, 25);
            this.txtUom.Name = "txtUom";
            this.txtUom.Size = new System.Drawing.Size(60, 20);
            this.txtUom.TabIndex = 9;
            // 
            // i6
            // 
            this.i6.Location = new System.Drawing.Point(865, 30);
            this.i6.Name = "i6";
            this.i6.Size = new System.Drawing.Size(35, 25);
            this.i6.TabIndex = 10;
            this.i6.Text = "Cost";
            // 
            // txtCost
            // 
            this.txtCost.Location = new System.Drawing.Point(905, 25);
            this.txtCost.Name = "txtCost";
            this.txtCost.Size = new System.Drawing.Size(70, 20);
            this.txtCost.TabIndex = 11;
            // 
            // i7
            // 
            this.i7.Location = new System.Drawing.Point(990, 30);
            this.i7.Name = "i7";
            this.i7.Size = new System.Drawing.Size(40, 25);
            this.i7.TabIndex = 12;
            this.i7.Text = "Value";
            // 
            // txtVarianceValue
            // 
            this.txtVarianceValue.Location = new System.Drawing.Point(1035, 25);
            this.txtVarianceValue.Name = "txtVarianceValue";
            this.txtVarianceValue.Size = new System.Drawing.Size(90, 20);
            this.txtVarianceValue.TabIndex = 13;
            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnAddItem.Location = new System.Drawing.Point(1035, 55);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(90, 28);
            this.btnAddItem.TabIndex = 14;
            this.btnAddItem.Text = "Add";
            this.btnAddItem.UseVisualStyleBackColor = false;
            // 
            // btnLoadStock
            // 
            this.btnLoadStock.Location = new System.Drawing.Point(15, 55);
            this.btnLoadStock.Name = "btnLoadStock";
            this.btnLoadStock.Size = new System.Drawing.Size(180, 28);
            this.btnLoadStock.TabIndex = 15;
            this.btnLoadStock.Text = "Load Current Stock";
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.Location = new System.Drawing.Point(15, 90);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.Size = new System.Drawing.Size(1125, 200);
            this.dgvItems.TabIndex = 16;
            // 
            // grpSummary
            // 
            this.grpSummary.Controls.Add(this.s1);
            this.grpSummary.Controls.Add(this.txtPositiveVariance);
            this.grpSummary.Controls.Add(this.s2);
            this.grpSummary.Controls.Add(this.txtNegativeVariance);
            this.grpSummary.Location = new System.Drawing.Point(870, 555);
            this.grpSummary.Name = "grpSummary";
            this.grpSummary.Size = new System.Drawing.Size(300, 80);
            this.grpSummary.TabIndex = 3;
            this.grpSummary.TabStop = false;
            this.grpSummary.Text = "Summary";
            // 
            // s1
            // 
            this.s1.Location = new System.Drawing.Point(15, 25);
            this.s1.Name = "s1";
            this.s1.Size = new System.Drawing.Size(110, 25);
            this.s1.TabIndex = 0;
            this.s1.Text = "Positive Variance";
            // 
            // txtPositiveVariance
            // 
            this.txtPositiveVariance.Location = new System.Drawing.Point(135, 20);
            this.txtPositiveVariance.Name = "txtPositiveVariance";
            this.txtPositiveVariance.Size = new System.Drawing.Size(140, 20);
            this.txtPositiveVariance.TabIndex = 1;
            // 
            // s2
            // 
            this.s2.Location = new System.Drawing.Point(15, 50);
            this.s2.Name = "s2";
            this.s2.Size = new System.Drawing.Size(110, 25);
            this.s2.TabIndex = 2;
            this.s2.Text = "Negative Variance";
            // 
            // txtNegativeVariance
            // 
            this.txtNegativeVariance.Location = new System.Drawing.Point(135, 50);
            this.txtNegativeVariance.Name = "txtNegativeVariance";
            this.txtNegativeVariance.Size = new System.Drawing.Size(140, 20);
            this.txtNegativeVariance.TabIndex = 3;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnNew.Location = new System.Drawing.Point(120, 575);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(110, 35);
            this.btnNew.TabIndex = 4;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSave.Location = new System.Drawing.Point(240, 575);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 35);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnUpdate.Location = new System.Drawing.Point(360, 575);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(110, 35);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnDelete.Location = new System.Drawing.Point(480, 575);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(110, 35);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnApprove.Location = new System.Drawing.Point(600, 575);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(110, 35);
            this.btnApprove.TabIndex = 8;
            this.btnApprove.Text = "Approve";
            this.btnApprove.UseVisualStyleBackColor = false;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnClear.Location = new System.Drawing.Point(720, 575);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(110, 35);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // frmStockReconciliation
            // 
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.grpHeader);
            this.Controls.Add(this.grpItems);
            this.Controls.Add(this.grpSummary);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnApprove);
            this.Controls.Add(this.btnClear);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmStockReconciliation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock Reconciliation";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.grpHeader.ResumeLayout(false);
            this.grpHeader.PerformLayout();
            this.grpItems.ResumeLayout(false);
            this.grpItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.grpSummary.ResumeLayout(false);
            this.grpSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        private Label h1;
        private Label h2;
        private Label h3;
        private Label h4;
        private Label h5;
        private Label h6;
        private Label h7;
        private Label h8;
        private Label i1;
        private Label i2;
        private Label i3;
        private Label i4;
        private Label i5;
        private Label i6;
        private Label i7;
        private Label s1;
        private Label s2;
    }
}