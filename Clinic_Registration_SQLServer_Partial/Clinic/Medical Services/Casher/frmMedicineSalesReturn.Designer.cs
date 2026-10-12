namespace Clinic.Medical_Services.Casher
{
    partial class frmMedicineSalesReturn
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTopHeader = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSearchTitle = new System.Windows.Forms.Label();
            this.pnlSearchGlow = new System.Windows.Forms.Panel();
            this.txtBillIDSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.pnlPatientInfo = new System.Windows.Forms.Panel();
            this.lblBillNumber = new System.Windows.Forms.Label();
            this.lblBillIDTitle = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblPatientNameTitle = new System.Windows.Forms.Label();
            this.lblBillDate = new System.Windows.Forms.Label();
            this.lblDateTitle = new System.Windows.Forms.Label();
            this.dgvReturnItems = new System.Windows.Forms.DataGridView();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnConfirmReturn = new System.Windows.Forms.Button();
            this.lblTotalRefundAmount = new System.Windows.Forms.Label();
            this.lblTotalRefundTitle = new System.Windows.Forms.Label();
            this.pnlTopHeader.SuspendLayout();
            this.pnlSearchGlow.SuspendLayout();
            this.pnlPatientInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturnItems)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTopHeader
            // 
            this.pnlTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.pnlTopHeader.Controls.Add(this.btnExit);
            this.pnlTopHeader.Controls.Add(this.lblTitle);
            this.pnlTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlTopHeader.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTopHeader.Name = "pnlTopHeader";
            this.pnlTopHeader.Size = new System.Drawing.Size(1210, 74);
            this.pnlTopHeader.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnExit.Image = global::Clinic.Properties.Resources.sign_out_32__2;
            this.btnExit.Location = new System.Drawing.Point(1059, 15);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 42);
            this.btnExit.TabIndex = 255;
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(373, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(495, 46);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Medicine Sales Return System";
            // 
            // lblSearchTitle
            // 
            this.lblSearchTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearchTitle.AutoSize = true;
            this.lblSearchTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSearchTitle.ForeColor = System.Drawing.Color.White;
            this.lblSearchTitle.Location = new System.Drawing.Point(938, 94);
            this.lblSearchTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchTitle.Name = "lblSearchTitle";
            this.lblSearchTitle.Size = new System.Drawing.Size(221, 28);
            this.lblSearchTitle.TabIndex = 1;
            this.lblSearchTitle.Text = "Enter Bill ID to Search";
            // 
            // pnlSearchGlow
            // 
            this.pnlSearchGlow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSearchGlow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.pnlSearchGlow.Controls.Add(this.txtBillIDSearch);
            this.pnlSearchGlow.Location = new System.Drawing.Point(643, 82);
            this.pnlSearchGlow.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSearchGlow.Name = "pnlSearchGlow";
            this.pnlSearchGlow.Padding = new System.Windows.Forms.Padding(2);
            this.pnlSearchGlow.Size = new System.Drawing.Size(287, 40);
            this.pnlSearchGlow.TabIndex = 2;
            // 
            // txtBillIDSearch
            // 
            this.txtBillIDSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(53)))), ((int)(((byte)(54)))));
            this.txtBillIDSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBillIDSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBillIDSearch.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtBillIDSearch.ForeColor = System.Drawing.Color.White;
            this.txtBillIDSearch.Location = new System.Drawing.Point(2, 2);
            this.txtBillIDSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtBillIDSearch.Name = "txtBillIDSearch";
            this.txtBillIDSearch.Size = new System.Drawing.Size(283, 32);
            this.txtBillIDSearch.TabIndex = 0;
            this.txtBillIDSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(95)))), ((int)(((byte)(97)))));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(493, 87);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 44);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "🔍 Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // pnlPatientInfo
            // 
            this.pnlPatientInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPatientInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(35)))), ((int)(((byte)(36)))));
            this.pnlPatientInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPatientInfo.Controls.Add(this.lblBillNumber);
            this.pnlPatientInfo.Controls.Add(this.lblBillIDTitle);
            this.pnlPatientInfo.Controls.Add(this.lblPatientName);
            this.pnlPatientInfo.Controls.Add(this.lblPatientNameTitle);
            this.pnlPatientInfo.Controls.Add(this.lblBillDate);
            this.pnlPatientInfo.Controls.Add(this.lblDateTitle);
            this.pnlPatientInfo.Location = new System.Drawing.Point(709, 148);
            this.pnlPatientInfo.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPatientInfo.Name = "pnlPatientInfo";
            this.pnlPatientInfo.Size = new System.Drawing.Size(486, 91);
            this.pnlPatientInfo.TabIndex = 4;
            // 
            // lblBillNumber
            // 
            this.lblBillNumber.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblBillNumber.ForeColor = System.Drawing.Color.White;
            this.lblBillNumber.Location = new System.Drawing.Point(15, 48);
            this.lblBillNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillNumber.Name = "lblBillNumber";
            this.lblBillNumber.Size = new System.Drawing.Size(100, 25);
            this.lblBillNumber.TabIndex = 5;
            this.lblBillNumber.Text = "B-1001";
            this.lblBillNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBillIDTitle
            // 
            this.lblBillIDTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblBillIDTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblBillIDTitle.Location = new System.Drawing.Point(15, 14);
            this.lblBillIDTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillIDTitle.Name = "lblBillIDTitle";
            this.lblBillIDTitle.Size = new System.Drawing.Size(119, 25);
            this.lblBillIDTitle.TabIndex = 4;
            this.lblBillIDTitle.Text = "Bill Number";
            this.lblBillIDTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPatientName.ForeColor = System.Drawing.Color.White;
            this.lblPatientName.Location = new System.Drawing.Point(142, 48);
            this.lblPatientName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(195, 25);
            this.lblPatientName.TabIndex = 3;
            this.lblPatientName.Text = "أحمد مصطفى";
            this.lblPatientName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPatientNameTitle
            // 
            this.lblPatientNameTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPatientNameTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblPatientNameTitle.Location = new System.Drawing.Point(142, 14);
            this.lblPatientNameTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPatientNameTitle.Name = "lblPatientNameTitle";
            this.lblPatientNameTitle.Size = new System.Drawing.Size(195, 25);
            this.lblPatientNameTitle.TabIndex = 2;
            this.lblPatientNameTitle.Text = "Patient Name";
            this.lblPatientNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBillDate
            // 
            this.lblBillDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblBillDate.ForeColor = System.Drawing.Color.White;
            this.lblBillDate.Location = new System.Drawing.Point(344, 48);
            this.lblBillDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillDate.Name = "lblBillDate";
            this.lblBillDate.Size = new System.Drawing.Size(127, 25);
            this.lblBillDate.TabIndex = 1;
            this.lblBillDate.Text = "2024/05/15";
            this.lblBillDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDateTitle
            // 
            this.lblDateTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDateTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblDateTitle.Location = new System.Drawing.Point(344, 14);
            this.lblDateTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDateTitle.Name = "lblDateTitle";
            this.lblDateTitle.Size = new System.Drawing.Size(127, 25);
            this.lblDateTitle.TabIndex = 0;
            this.lblDateTitle.Text = "Date";
            this.lblDateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvReturnItems
            // 
            this.dgvReturnItems.AllowUserToAddRows = false;
            this.dgvReturnItems.AllowUserToDeleteRows = false;
            this.dgvReturnItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReturnItems.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(50)))), ((int)(((byte)(51)))));
            this.dgvReturnItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReturnItems.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(68)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(68)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReturnItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReturnItems.ColumnHeadersHeight = 40;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(76)))), ((int)(((byte)(78)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(110)))), ((int)(((byte)(112)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReturnItems.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvReturnItems.EnableHeadersVisualStyles = false;
            this.dgvReturnItems.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.dgvReturnItems.Location = new System.Drawing.Point(14, 261);
            this.dgvReturnItems.Margin = new System.Windows.Forms.Padding(4);
            this.dgvReturnItems.Name = "dgvReturnItems";
            this.dgvReturnItems.RowHeadersVisible = false;
            this.dgvReturnItems.RowHeadersWidth = 51;
            this.dgvReturnItems.RowTemplate.Height = 35;
            this.dgvReturnItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReturnItems.Size = new System.Drawing.Size(1182, 423);
            this.dgvReturnItems.TabIndex = 5;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.pnlFooter.Controls.Add(this.btnConfirmReturn);
            this.pnlFooter.Controls.Add(this.lblTotalRefundAmount);
            this.pnlFooter.Controls.Add(this.lblTotalRefundTitle);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 705);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(4);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1210, 98);
            this.pnlFooter.TabIndex = 6;
            // 
            // btnConfirmReturn
            // 
            this.btnConfirmReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmReturn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(52)))), ((int)(((byte)(54)))));
            this.btnConfirmReturn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmReturn.FlatAppearance.BorderSize = 0;
            this.btnConfirmReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmReturn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnConfirmReturn.ForeColor = System.Drawing.Color.White;
            this.btnConfirmReturn.Location = new System.Drawing.Point(883, 18);
            this.btnConfirmReturn.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirmReturn.Name = "btnConfirmReturn";
            this.btnConfirmReturn.Size = new System.Drawing.Size(313, 62);
            this.btnConfirmReturn.TabIndex = 2;
            this.btnConfirmReturn.Text = "Confirm and Return Cash   ✔";
            this.btnConfirmReturn.UseVisualStyleBackColor = false;
            this.btnConfirmReturn.Click += new System.EventHandler(this.btnConfirmReturn_Click);
            // 
            // lblTotalRefundAmount
            // 
            this.lblTotalRefundAmount.AutoSize = true;
            this.lblTotalRefundAmount.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTotalRefundAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(237)))), ((int)(((byte)(240)))));
            this.lblTotalRefundAmount.Location = new System.Drawing.Point(14, 21);
            this.lblTotalRefundAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalRefundAmount.Name = "lblTotalRefundAmount";
            this.lblTotalRefundAmount.Size = new System.Drawing.Size(212, 54);
            this.lblTotalRefundAmount.TabIndex = 1;
            this.lblTotalRefundAmount.Text = "15.00 ر.س";
            // 
            // lblTotalRefundTitle
            // 
            this.lblTotalRefundTitle.AutoSize = true;
            this.lblTotalRefundTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalRefundTitle.ForeColor = System.Drawing.Color.White;
            this.lblTotalRefundTitle.Location = new System.Drawing.Point(219, 37);
            this.lblTotalRefundTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalRefundTitle.Name = "lblTotalRefundTitle";
            this.lblTotalRefundTitle.Size = new System.Drawing.Size(394, 32);
            this.lblTotalRefundTitle.TabIndex = 0;
            this.lblTotalRefundTitle.Text = "Total Refund Amount for Patient:";
            // 
            // frmMedicineSalesReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(1210, 803);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.dgvReturnItems);
            this.Controls.Add(this.pnlPatientInfo);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.pnlSearchGlow);
            this.Controls.Add(this.lblSearchTitle);
            this.Controls.Add(this.pnlTopHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1210, 803);
            this.Name = "frmMedicineSalesReturn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmMedicineSalesReturn_Load);
            this.pnlTopHeader.ResumeLayout(false);
            this.pnlTopHeader.PerformLayout();
            this.pnlSearchGlow.ResumeLayout(false);
            this.pnlSearchGlow.PerformLayout();
            this.pnlPatientInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturnItems)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSearchTitle;
        private System.Windows.Forms.Panel pnlSearchGlow;
        private System.Windows.Forms.TextBox txtBillIDSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel pnlPatientInfo;
        private System.Windows.Forms.Label lblBillDate;
        private System.Windows.Forms.Label lblDateTitle;
        private System.Windows.Forms.Label lblBillNumber;
        private System.Windows.Forms.Label lblBillIDTitle;
        private System.Windows.Forms.Label lblPatientName;
        private System.Windows.Forms.Label lblPatientNameTitle;
        private System.Windows.Forms.DataGridView dgvReturnItems;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblTotalRefundAmount;
        private System.Windows.Forms.Label lblTotalRefundTitle;
        private System.Windows.Forms.Button btnConfirmReturn;
        private System.Windows.Forms.Button btnExit;
    }
}