namespace Clinic.Medical_Services.Casher
{
    partial class frmEditPendingPrescription
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTopHeader = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlPrescriptionInfo = new System.Windows.Forms.Panel();
            this.lblPrescriptionIDVal = new System.Windows.Forms.Label();
            this.lblPrescriptionIDTitle = new System.Windows.Forms.Label();
            this.lblDateVal = new System.Windows.Forms.Label();
            this.lblDateTitle = new System.Windows.Forms.Label();
            this.pnlOrderDetailsHeader = new System.Windows.Forms.Panel();
            this.lblOrderDetailsTitle = new System.Windows.Forms.Label();
            this.dgvPrescriptionItems = new System.Windows.Forms.DataGridView();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlTopHeader.SuspendLayout();
            this.pnlPrescriptionInfo.SuspendLayout();
            this.pnlOrderDetailsHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrescriptionItems)).BeginInit();
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
            this.btnExit.Location = new System.Drawing.Point(1038, 16);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 42);
            this.btnExit.TabIndex = 256;
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
            this.lblTitle.Size = new System.Drawing.Size(523, 46);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Edit Pending Prescription Items";
            // 
            // pnlPrescriptionInfo
            // 
            this.pnlPrescriptionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPrescriptionInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(35)))), ((int)(((byte)(36)))));
            this.pnlPrescriptionInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPrescriptionInfo.Controls.Add(this.lblPrescriptionIDVal);
            this.pnlPrescriptionInfo.Controls.Add(this.lblPrescriptionIDTitle);
            this.pnlPrescriptionInfo.Controls.Add(this.lblDateVal);
            this.pnlPrescriptionInfo.Controls.Add(this.lblDateTitle);
            this.pnlPrescriptionInfo.Location = new System.Drawing.Point(612, 92);
            this.pnlPrescriptionInfo.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPrescriptionInfo.Name = "pnlPrescriptionInfo";
            this.pnlPrescriptionInfo.Size = new System.Drawing.Size(583, 91);
            this.pnlPrescriptionInfo.TabIndex = 1;
            // 
            // lblPrescriptionIDVal
            // 
            this.lblPrescriptionIDVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(56)))));
            this.lblPrescriptionIDVal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPrescriptionIDVal.ForeColor = System.Drawing.Color.White;
            this.lblPrescriptionIDVal.Location = new System.Drawing.Point(18, 43);
            this.lblPrescriptionIDVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrescriptionIDVal.Name = "lblPrescriptionIDVal";
            this.lblPrescriptionIDVal.Size = new System.Drawing.Size(257, 34);
            this.lblPrescriptionIDVal.TabIndex = 3;
            this.lblPrescriptionIDVal.Text = "B-2005";
            this.lblPrescriptionIDVal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrescriptionIDTitle
            // 
            this.lblPrescriptionIDTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPrescriptionIDTitle.ForeColor = System.Drawing.Color.LightGray;
            this.lblPrescriptionIDTitle.Location = new System.Drawing.Point(18, 10);
            this.lblPrescriptionIDTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrescriptionIDTitle.Name = "lblPrescriptionIDTitle";
            this.lblPrescriptionIDTitle.Size = new System.Drawing.Size(257, 28);
            this.lblPrescriptionIDTitle.TabIndex = 2;
            this.lblPrescriptionIDTitle.Text = "Prescription ID";
            this.lblPrescriptionIDTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDateVal
            // 
            this.lblDateVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(56)))));
            this.lblDateVal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblDateVal.ForeColor = System.Drawing.Color.White;
            this.lblDateVal.Location = new System.Drawing.Point(303, 43);
            this.lblDateVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDateVal.Name = "lblDateVal";
            this.lblDateVal.Size = new System.Drawing.Size(257, 34);
            this.lblDateVal.TabIndex = 1;
            this.lblDateVal.Text = "2024/06/25";
            this.lblDateVal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDateTitle
            // 
            this.lblDateTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDateTitle.ForeColor = System.Drawing.Color.LightGray;
            this.lblDateTitle.Location = new System.Drawing.Point(303, 10);
            this.lblDateTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDateTitle.Name = "lblDateTitle";
            this.lblDateTitle.Size = new System.Drawing.Size(257, 28);
            this.lblDateTitle.TabIndex = 0;
            this.lblDateTitle.Text = "Date";
            this.lblDateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlOrderDetailsHeader
            // 
            this.pnlOrderDetailsHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOrderDetailsHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(53)))), ((int)(((byte)(54)))));
            this.pnlOrderDetailsHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOrderDetailsHeader.Controls.Add(this.lblOrderDetailsTitle);
            this.pnlOrderDetailsHeader.Location = new System.Drawing.Point(14, 92);
            this.pnlOrderDetailsHeader.Margin = new System.Windows.Forms.Padding(4);
            this.pnlOrderDetailsHeader.Name = "pnlOrderDetailsHeader";
            this.pnlOrderDetailsHeader.Size = new System.Drawing.Size(581, 91);
            this.pnlOrderDetailsHeader.TabIndex = 2;
            // 
            // lblOrderDetailsTitle
            // 
            this.lblOrderDetailsTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOrderDetailsTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblOrderDetailsTitle.ForeColor = System.Drawing.Color.White;
            this.lblOrderDetailsTitle.Location = new System.Drawing.Point(0, 0);
            this.lblOrderDetailsTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOrderDetailsTitle.Name = "lblOrderDetailsTitle";
            this.lblOrderDetailsTitle.Size = new System.Drawing.Size(579, 89);
            this.lblOrderDetailsTitle.TabIndex = 0;
            this.lblOrderDetailsTitle.Text = "Order Details";
            this.lblOrderDetailsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvPrescriptionItems
            // 
            this.dgvPrescriptionItems.AllowUserToAddRows = false;
            this.dgvPrescriptionItems.AllowUserToDeleteRows = false;
            this.dgvPrescriptionItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPrescriptionItems.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(50)))), ((int)(((byte)(51)))));
            this.dgvPrescriptionItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPrescriptionItems.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(68)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(68)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPrescriptionItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPrescriptionItems.ColumnHeadersHeight = 45;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(76)))), ((int)(((byte)(78)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(110)))), ((int)(((byte)(112)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPrescriptionItems.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPrescriptionItems.EnableHeadersVisualStyles = false;
            this.dgvPrescriptionItems.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.dgvPrescriptionItems.Location = new System.Drawing.Point(14, 203);
            this.dgvPrescriptionItems.Margin = new System.Windows.Forms.Padding(4);
            this.dgvPrescriptionItems.Name = "dgvPrescriptionItems";
            this.dgvPrescriptionItems.RowHeadersVisible = false;
            this.dgvPrescriptionItems.RowHeadersWidth = 51;
            this.dgvPrescriptionItems.RowTemplate.Height = 40;
            this.dgvPrescriptionItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPrescriptionItems.Size = new System.Drawing.Size(1182, 468);
            this.dgvPrescriptionItems.TabIndex = 3;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.pnlFooter.Controls.Add(this.btnSaveChanges);
            this.pnlFooter.Controls.Add(this.btnCancel);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 692);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(4);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1210, 111);
            this.pnlFooter.TabIndex = 4;
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveChanges.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(135)))), ((int)(((byte)(132)))));
            this.btnSaveChanges.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveChanges.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(237)))), ((int)(((byte)(240)))));
            this.btnSaveChanges.FlatAppearance.BorderSize = 2;
            this.btnSaveChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveChanges.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnSaveChanges.ForeColor = System.Drawing.Color.White;
            this.btnSaveChanges.Location = new System.Drawing.Point(883, 25);
            this.btnSaveChanges.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(313, 62);
            this.btnSaveChanges.TabIndex = 0;
            this.btnSaveChanges.Text = "Save Changes  ✔";
            this.btnSaveChanges.UseVisualStyleBackColor = false;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(14, 25);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(233, 62);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmEditPendingPrescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(1210, 803);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.dgvPrescriptionItems);
            this.Controls.Add(this.pnlOrderDetailsHeader);
            this.Controls.Add(this.pnlPrescriptionInfo);
            this.Controls.Add(this.pnlTopHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1210, 803);
            this.Name = "frmEditPendingPrescription";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Pending Prescription Items";
            this.pnlTopHeader.ResumeLayout(false);
            this.pnlTopHeader.PerformLayout();
            this.pnlPrescriptionInfo.ResumeLayout(false);
            this.pnlOrderDetailsHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrescriptionItems)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlPrescriptionInfo;
        private System.Windows.Forms.Label lblDateVal;
        private System.Windows.Forms.Label lblDateTitle;
        private System.Windows.Forms.Label lblPrescriptionIDVal;
        private System.Windows.Forms.Label lblPrescriptionIDTitle;
        private System.Windows.Forms.Panel pnlOrderDetailsHeader;
        private System.Windows.Forms.Label lblOrderDetailsTitle;
        private System.Windows.Forms.DataGridView dgvPrescriptionItems;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
    }
}