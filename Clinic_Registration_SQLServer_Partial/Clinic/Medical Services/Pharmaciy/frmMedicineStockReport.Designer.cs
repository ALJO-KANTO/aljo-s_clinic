namespace Clinic.Medical_Services.Pharmaciy
{
    partial class frmMedicineStockReport
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.guna2ImageButton1 = new Guna.UI2.WinForms.Guna2ImageButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblMedicine = new System.Windows.Forms.Label();
            this.txtMedicine = new System.Windows.Forms.TextBox();
            this.lblWarehouse = new System.Windows.Forms.Label();
            this.cmbWarehouse = new System.Windows.Forms.ComboBox();
            this.lblStockStatus = new System.Windows.Forms.Label();
            this.cmbStockStatus = new System.Windows.Forms.ComboBox();
            this.lblExpiryStatus = new System.Windows.Forms.Label();
            this.cmbExpiryStatus = new System.Windows.Forms.ComboBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.pnlTotalMedicines = new System.Windows.Forms.Panel();
            this.lblTotalMedicinesTitle = new System.Windows.Forms.Label();
            this.lblTotalMedicines = new System.Windows.Forms.Label();
            this.pnlStockValue = new System.Windows.Forms.Panel();
            this.lblStockValueTitle = new System.Windows.Forms.Label();
            this.lblStockValue = new System.Windows.Forms.Label();
            this.pnlLowStock = new System.Windows.Forms.Panel();
            this.lblLowStockTitle = new System.Windows.Forms.Label();
            this.lblLowStock = new System.Windows.Forms.Label();
            this.pnlExpired = new System.Windows.Forms.Panel();
            this.lblExpiredTitle = new System.Windows.Forms.Label();
            this.lblExpired = new System.Windows.Forms.Label();
            this.lblReportHeading = new System.Windows.Forms.Label();
            this.dgvMedicines = new System.Windows.Forms.DataGridView();
            this.pnlHeader.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.pnlTotalMedicines.SuspendLayout();
            this.pnlStockValue.SuspendLayout();
            this.pnlLowStock.SuspendLayout();
            this.pnlExpired.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicines)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(35)))), ((int)(((byte)(68)))));
            this.pnlHeader.Controls.Add(this.guna2ImageButton1);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1250, 65);
            this.pnlHeader.TabIndex = 0;
            // 
            // guna2ImageButton1
            // 
            this.guna2ImageButton1.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.guna2ImageButton1.HoverState.ImageSize = new System.Drawing.Size(64, 64);
            this.guna2ImageButton1.Image = global::Clinic.Properties.Resources.Close_32;
            this.guna2ImageButton1.ImageOffset = new System.Drawing.Point(0, 0);
            this.guna2ImageButton1.ImageRotate = 0F;
            this.guna2ImageButton1.Location = new System.Drawing.Point(1200, 8);
            this.guna2ImageButton1.Name = "guna2ImageButton1";
            this.guna2ImageButton1.PressedState.ImageSize = new System.Drawing.Size(64, 64);
            this.guna2ImageButton1.Size = new System.Drawing.Size(41, 39);
            this.guna2ImageButton1.TabIndex = 1;
            this.guna2ImageButton1.Click += new System.EventHandler(this.guna2ImageButton1_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(25, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(257, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Medicine Stock Report";
            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = System.Drawing.Color.White;
            this.pnlFilters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFilters.Controls.Add(this.lblFromDate);
            this.pnlFilters.Controls.Add(this.dtpFromDate);
            this.pnlFilters.Controls.Add(this.lblToDate);
            this.pnlFilters.Controls.Add(this.dtpToDate);
            this.pnlFilters.Controls.Add(this.lblMedicine);
            this.pnlFilters.Controls.Add(this.txtMedicine);
            this.pnlFilters.Controls.Add(this.lblWarehouse);
            this.pnlFilters.Controls.Add(this.cmbWarehouse);
            this.pnlFilters.Controls.Add(this.lblStockStatus);
            this.pnlFilters.Controls.Add(this.cmbStockStatus);
            this.pnlFilters.Controls.Add(this.lblExpiryStatus);
            this.pnlFilters.Controls.Add(this.cmbExpiryStatus);
            this.pnlFilters.Controls.Add(this.btnGenerate);
            this.pnlFilters.Controls.Add(this.btnReset);
            this.pnlFilters.Controls.Add(this.btnExportExcel);
            this.pnlFilters.Controls.Add(this.btnPrint);
            this.pnlFilters.Location = new System.Drawing.Point(20, 85);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Size = new System.Drawing.Size(1210, 180);
            this.pnlFilters.TabIndex = 1;
            // 
            // lblFromDate
            // 
            this.lblFromDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblFromDate.Location = new System.Drawing.Point(20, 17);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(208, 19);
            this.lblFromDate.TabIndex = 0;
            this.lblFromDate.Text = "From Date";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(23, 42);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(205, 25);
            this.dtpFromDate.TabIndex = 1;
            // 
            // lblToDate
            // 
            this.lblToDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblToDate.Location = new System.Drawing.Point(255, 17);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(208, 19);
            this.lblToDate.TabIndex = 2;
            this.lblToDate.Text = "To Date";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(258, 42);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(205, 25);
            this.dtpToDate.TabIndex = 3;
            // 
            // lblMedicine
            // 
            this.lblMedicine.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMedicine.Location = new System.Drawing.Point(490, 17);
            this.lblMedicine.Name = "lblMedicine";
            this.lblMedicine.Size = new System.Drawing.Size(323, 19);
            this.lblMedicine.TabIndex = 4;
            this.lblMedicine.Text = "Medicine";
            // 
            // txtMedicine
            // 
            this.txtMedicine.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMedicine.Location = new System.Drawing.Point(493, 42);
            this.txtMedicine.Name = "txtMedicine";
            this.txtMedicine.Size = new System.Drawing.Size(320, 25);
            this.txtMedicine.TabIndex = 5;
            // 
            // lblWarehouse
            // 
            this.lblWarehouse.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWarehouse.Location = new System.Drawing.Point(840, 17);
            this.lblWarehouse.Name = "lblWarehouse";
            this.lblWarehouse.Size = new System.Drawing.Size(333, 19);
            this.lblWarehouse.TabIndex = 6;
            this.lblWarehouse.Text = "Warehouse";
            // 
            // cmbWarehouse
            // 
            this.cmbWarehouse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWarehouse.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbWarehouse.FormattingEnabled = true;
            this.cmbWarehouse.Location = new System.Drawing.Point(843, 42);
            this.cmbWarehouse.Name = "cmbWarehouse";
            this.cmbWarehouse.Size = new System.Drawing.Size(330, 25);
            this.cmbWarehouse.TabIndex = 7;
            // 
            // lblStockStatus
            // 
            this.lblStockStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStockStatus.Location = new System.Drawing.Point(20, 87);
            this.lblStockStatus.Name = "lblStockStatus";
            this.lblStockStatus.Size = new System.Drawing.Size(208, 19);
            this.lblStockStatus.TabIndex = 8;
            this.lblStockStatus.Text = "Stock Status";
            // 
            // cmbStockStatus
            // 
            this.cmbStockStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStockStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbStockStatus.FormattingEnabled = true;
            this.cmbStockStatus.Items.AddRange(new object[] {
            "All",
            "In Stock",
            "Low Stock",
            "Out of Stock"});
            this.cmbStockStatus.Location = new System.Drawing.Point(23, 112);
            this.cmbStockStatus.Name = "cmbStockStatus";
            this.cmbStockStatus.Size = new System.Drawing.Size(205, 25);
            this.cmbStockStatus.TabIndex = 9;
            // 
            // lblExpiryStatus
            // 
            this.lblExpiryStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblExpiryStatus.Location = new System.Drawing.Point(255, 87);
            this.lblExpiryStatus.Name = "lblExpiryStatus";
            this.lblExpiryStatus.Size = new System.Drawing.Size(208, 19);
            this.lblExpiryStatus.TabIndex = 10;
            this.lblExpiryStatus.Text = "Expiry Status";
            // 
            // cmbExpiryStatus
            // 
            this.cmbExpiryStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExpiryStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbExpiryStatus.FormattingEnabled = true;
            this.cmbExpiryStatus.Items.AddRange(new object[] {
            "All",
            "Valid",
            "Expiring Soon",
            "Expired"});
            this.cmbExpiryStatus.Location = new System.Drawing.Point(258, 112);
            this.cmbExpiryStatus.Name = "cmbExpiryStatus";
            this.cmbExpiryStatus.Size = new System.Drawing.Size(205, 25);
            this.cmbExpiryStatus.TabIndex = 11;
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(493, 105);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(155, 38);
            this.btnGenerate.TabIndex = 12;
            this.btnGenerate.Text = "Generate Report";
            this.btnGenerate.UseVisualStyleBackColor = false;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(658, 105);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(105, 38);
            this.btnReset.TabIndex = 13;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(130)))), ((int)(((byte)(85)))));
            this.btnExportExcel.FlatAppearance.BorderSize = 0;
            this.btnExportExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportExcel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExportExcel.ForeColor = System.Drawing.Color.White;
            this.btnExportExcel.Location = new System.Drawing.Point(785, 105);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(155, 38);
            this.btnExportExcel.TabIndex = 14;
            this.btnExportExcel.Text = "Export Excel";
            this.btnExportExcel.UseVisualStyleBackColor = false;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(952, 105);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 38);
            this.btnPrint.TabIndex = 15;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            // 
            // pnlSummary
            // 
            this.pnlSummary.Controls.Add(this.pnlTotalMedicines);
            this.pnlSummary.Controls.Add(this.pnlStockValue);
            this.pnlSummary.Controls.Add(this.pnlLowStock);
            this.pnlSummary.Controls.Add(this.pnlExpired);
            this.pnlSummary.Location = new System.Drawing.Point(20, 280);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(1210, 105);
            this.pnlSummary.TabIndex = 2;
            // 
            // pnlTotalMedicines
            // 
            this.pnlTotalMedicines.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            this.pnlTotalMedicines.Controls.Add(this.lblTotalMedicinesTitle);
            this.pnlTotalMedicines.Controls.Add(this.lblTotalMedicines);
            this.pnlTotalMedicines.Location = new System.Drawing.Point(0, 0);
            this.pnlTotalMedicines.Name = "pnlTotalMedicines";
            this.pnlTotalMedicines.Size = new System.Drawing.Size(285, 95);
            this.pnlTotalMedicines.TabIndex = 0;
            // 
            // lblTotalMedicinesTitle
            // 
            this.lblTotalMedicinesTitle.AutoSize = true;
            this.lblTotalMedicinesTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTotalMedicinesTitle.ForeColor = System.Drawing.Color.White;
            this.lblTotalMedicinesTitle.Location = new System.Drawing.Point(17, 15);
            this.lblTotalMedicinesTitle.Name = "lblTotalMedicinesTitle";
            this.lblTotalMedicinesTitle.Size = new System.Drawing.Size(103, 19);
            this.lblTotalMedicinesTitle.TabIndex = 0;
            this.lblTotalMedicinesTitle.Text = "Total Medicines";
            // 
            // lblTotalMedicines
            // 
            this.lblTotalMedicines.AutoSize = true;
            this.lblTotalMedicines.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTotalMedicines.ForeColor = System.Drawing.Color.White;
            this.lblTotalMedicines.Location = new System.Drawing.Point(17, 40);
            this.lblTotalMedicines.Name = "lblTotalMedicines";
            this.lblTotalMedicines.Size = new System.Drawing.Size(35, 41);
            this.lblTotalMedicines.TabIndex = 1;
            this.lblTotalMedicines.Text = "0";
            // 
            // pnlStockValue
            // 
            this.pnlStockValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(115)))), ((int)(((byte)(100)))));
            this.pnlStockValue.Controls.Add(this.lblStockValueTitle);
            this.pnlStockValue.Controls.Add(this.lblStockValue);
            this.pnlStockValue.Location = new System.Drawing.Point(305, 0);
            this.pnlStockValue.Name = "pnlStockValue";
            this.pnlStockValue.Size = new System.Drawing.Size(285, 95);
            this.pnlStockValue.TabIndex = 1;
            // 
            // lblStockValueTitle
            // 
            this.lblStockValueTitle.AutoSize = true;
            this.lblStockValueTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStockValueTitle.ForeColor = System.Drawing.Color.White;
            this.lblStockValueTitle.Location = new System.Drawing.Point(17, 15);
            this.lblStockValueTitle.Name = "lblStockValueTitle";
            this.lblStockValueTitle.Size = new System.Drawing.Size(112, 19);
            this.lblStockValueTitle.TabIndex = 0;
            this.lblStockValueTitle.Text = "Total Stock Value";
            // 
            // lblStockValue
            // 
            this.lblStockValue.AutoSize = true;
            this.lblStockValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblStockValue.ForeColor = System.Drawing.Color.White;
            this.lblStockValue.Location = new System.Drawing.Point(17, 40);
            this.lblStockValue.Name = "lblStockValue";
            this.lblStockValue.Size = new System.Drawing.Size(77, 41);
            this.lblStockValue.TabIndex = 1;
            this.lblStockValue.Text = "0.00";
            // 
            // pnlLowStock
            // 
            this.pnlLowStock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(139)))), ((int)(((byte)(40)))));
            this.pnlLowStock.Controls.Add(this.lblLowStockTitle);
            this.pnlLowStock.Controls.Add(this.lblLowStock);
            this.pnlLowStock.Location = new System.Drawing.Point(610, 0);
            this.pnlLowStock.Name = "pnlLowStock";
            this.pnlLowStock.Size = new System.Drawing.Size(285, 95);
            this.pnlLowStock.TabIndex = 2;
            // 
            // lblLowStockTitle
            // 
            this.lblLowStockTitle.AutoSize = true;
            this.lblLowStockTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblLowStockTitle.ForeColor = System.Drawing.Color.White;
            this.lblLowStockTitle.Location = new System.Drawing.Point(17, 15);
            this.lblLowStockTitle.Name = "lblLowStockTitle";
            this.lblLowStockTitle.Size = new System.Drawing.Size(136, 19);
            this.lblLowStockTitle.TabIndex = 0;
            this.lblLowStockTitle.Text = "Low Stock Medicines";
            // 
            // lblLowStock
            // 
            this.lblLowStock.AutoSize = true;
            this.lblLowStock.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblLowStock.ForeColor = System.Drawing.Color.White;
            this.lblLowStock.Location = new System.Drawing.Point(17, 40);
            this.lblLowStock.Name = "lblLowStock";
            this.lblLowStock.Size = new System.Drawing.Size(35, 41);
            this.lblLowStock.TabIndex = 1;
            this.lblLowStock.Text = "0";
            // 
            // pnlExpired
            // 
            this.pnlExpired.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.pnlExpired.Controls.Add(this.lblExpiredTitle);
            this.pnlExpired.Controls.Add(this.lblExpired);
            this.pnlExpired.Location = new System.Drawing.Point(915, 0);
            this.pnlExpired.Name = "pnlExpired";
            this.pnlExpired.Size = new System.Drawing.Size(285, 95);
            this.pnlExpired.TabIndex = 3;
            // 
            // lblExpiredTitle
            // 
            this.lblExpiredTitle.AutoSize = true;
            this.lblExpiredTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblExpiredTitle.ForeColor = System.Drawing.Color.White;
            this.lblExpiredTitle.Location = new System.Drawing.Point(17, 15);
            this.lblExpiredTitle.Name = "lblExpiredTitle";
            this.lblExpiredTitle.Size = new System.Drawing.Size(104, 19);
            this.lblExpiredTitle.TabIndex = 0;
            this.lblExpiredTitle.Text = "Expired Batches";
            // 
            // lblExpired
            // 
            this.lblExpired.AutoSize = true;
            this.lblExpired.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblExpired.ForeColor = System.Drawing.Color.White;
            this.lblExpired.Location = new System.Drawing.Point(17, 40);
            this.lblExpired.Name = "lblExpired";
            this.lblExpired.Size = new System.Drawing.Size(35, 41);
            this.lblExpired.TabIndex = 1;
            this.lblExpired.Text = "0";
            // 
            // lblReportHeading
            // 
            this.lblReportHeading.AutoSize = true;
            this.lblReportHeading.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblReportHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            this.lblReportHeading.Location = new System.Drawing.Point(20, 400);
            this.lblReportHeading.Name = "lblReportHeading";
            this.lblReportHeading.Size = new System.Drawing.Size(184, 21);
            this.lblReportHeading.TabIndex = 3;
            this.lblReportHeading.Text = "Medicine Stock Details";
            // 
            // dgvMedicines
            // 
            this.dgvMedicines.AllowUserToAddRows = false;
            this.dgvMedicines.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dgvMedicines.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMedicines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMedicines.BackgroundColor = System.Drawing.Color.White;
            this.dgvMedicines.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMedicines.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(35)))), ((int)(((byte)(68)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMedicines.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMedicines.ColumnHeadersHeight = 42;
            this.dgvMedicines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(230)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMedicines.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvMedicines.EnableHeadersVisualStyles = false;
            this.dgvMedicines.GridColor = System.Drawing.Color.LightGray;
            this.dgvMedicines.Location = new System.Drawing.Point(20, 435);
            this.dgvMedicines.MultiSelect = false;
            this.dgvMedicines.Name = "dgvMedicines";
            this.dgvMedicines.ReadOnly = true;
            this.dgvMedicines.RowHeadersVisible = false;
            this.dgvMedicines.RowTemplate.Height = 35;
            this.dgvMedicines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMedicines.Size = new System.Drawing.Size(1210, 300);
            this.dgvMedicines.TabIndex = 4;
            // 
            // frmMedicineStockReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1250, 749);
            this.Controls.Add(this.dgvMedicines);
            this.Controls.Add(this.lblReportHeading);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlFilters);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(1100, 650);
            this.Name = "frmMedicineStockReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Medicine Stock Report";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlSummary.ResumeLayout(false);
            this.pnlTotalMedicines.ResumeLayout(false);
            this.pnlTotalMedicines.PerformLayout();
            this.pnlStockValue.ResumeLayout(false);
            this.pnlStockValue.PerformLayout();
            this.pnlLowStock.ResumeLayout(false);
            this.pnlLowStock.PerformLayout();
            this.pnlExpired.ResumeLayout(false);
            this.pnlExpired.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicines)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label lblMedicine;
        private System.Windows.Forms.TextBox txtMedicine;
        private System.Windows.Forms.Label lblWarehouse;
        private System.Windows.Forms.ComboBox cmbWarehouse;
        private System.Windows.Forms.Label lblStockStatus;
        private System.Windows.Forms.ComboBox cmbStockStatus;
        private System.Windows.Forms.Label lblExpiryStatus;
        private System.Windows.Forms.ComboBox cmbExpiryStatus;

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnPrint;

        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Panel pnlTotalMedicines;
        private System.Windows.Forms.Label lblTotalMedicinesTitle;
        private System.Windows.Forms.Label lblTotalMedicines;

        private System.Windows.Forms.Panel pnlStockValue;
        private System.Windows.Forms.Label lblStockValueTitle;
        private System.Windows.Forms.Label lblStockValue;

        private System.Windows.Forms.Panel pnlLowStock;
        private System.Windows.Forms.Label lblLowStockTitle;
        private System.Windows.Forms.Label lblLowStock;

        private System.Windows.Forms.Panel pnlExpired;
        private System.Windows.Forms.Label lblExpiredTitle;
        private System.Windows.Forms.Label lblExpired;

        private System.Windows.Forms.Label lblReportHeading;
        private System.Windows.Forms.DataGridView dgvMedicines;
        private Guna.UI2.WinForms.Guna2ImageButton guna2ImageButton1;
    }
}