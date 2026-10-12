using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Clinic.Medical_Services.Pharmaciy
{
    partial class frmDirictSales
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panTitle = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.lbCurrentUser = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnAddToPrescription = new System.Windows.Forms.Button();
            this.lbPrescriptionID = new System.Windows.Forms.Label();
            this.kfjaf = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.dgPrescriptionDetails = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSavePrescription = new System.Windows.Forms.Button();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.panel16 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPrescriptionDetails)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel16.SuspendLayout();
            this.SuspendLayout();
            // 
            // panTitle
            // 
            this.panTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(107)))));
            this.panTitle.Controls.Add(this.label5);
            this.panTitle.Controls.Add(this.pictureBox4);
            this.panTitle.Controls.Add(this.btnExit);
            this.panTitle.Controls.Add(this.lbCurrentUser);
            this.panTitle.Controls.Add(this.lblTitle);
            this.panTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTitle.Location = new System.Drawing.Point(0, 0);
            this.panTitle.Margin = new System.Windows.Forms.Padding(10, 5, 5, 5);
            this.panTitle.Name = "panTitle";
            this.panTitle.Size = new System.Drawing.Size(1545, 69);
            this.panTitle.TabIndex = 239;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(1120, 20);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(137, 28);
            this.label5.TabIndex = 240;
            this.label5.Text = "Current User:";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.Image = global::Clinic.Properties.Resources.Person_32;
            this.pictureBox4.Location = new System.Drawing.Point(1073, 18);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(41, 31);
            this.pictureBox4.TabIndex = 255;
            this.pictureBox4.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Image = global::Clinic.Properties.Resources.sign_out_32__2;
            this.btnExit.Location = new System.Drawing.Point(1430, 13);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 42);
            this.btnExit.TabIndex = 254;
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lbCurrentUser
            // 
            this.lbCurrentUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCurrentUser.AutoSize = true;
            this.lbCurrentUser.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lbCurrentUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(223)))), ((int)(((byte)(219)))));
            this.lbCurrentUser.Location = new System.Drawing.Point(1265, 20);
            this.lbCurrentUser.Name = "lbCurrentUser";
            this.lbCurrentUser.Size = new System.Drawing.Size(54, 28);
            this.lbCurrentUser.TabIndex = 241;
            this.lbCurrentUser.Text = "[???]";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 14);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(332, 41);
            this.lblTitle.TabIndex = 236;
            this.lblTitle.Text = "Direct Pharmacy Sales";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.btnAddToPrescription);
            this.panel5.Controls.Add(this.lbPrescriptionID);
            this.panel5.Controls.Add(this.kfjaf);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(1193, 69);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(20);
            this.panel5.Size = new System.Drawing.Size(352, 573);
            this.panel5.TabIndex = 240;
            // 
            // btnAddToPrescription
            // 
            this.btnAddToPrescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnAddToPrescription.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddToPrescription.FlatAppearance.BorderSize = 0;
            this.btnAddToPrescription.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddToPrescription.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnAddToPrescription.ForeColor = System.Drawing.Color.White;
            this.btnAddToPrescription.Image = global::Clinic.Properties.Resources.show_medicine_32;
            this.btnAddToPrescription.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddToPrescription.Location = new System.Drawing.Point(20, 40);
            this.btnAddToPrescription.Name = "btnAddToPrescription";
            this.btnAddToPrescription.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnAddToPrescription.Size = new System.Drawing.Size(312, 75);
            this.btnAddToPrescription.TabIndex = 270;
            this.btnAddToPrescription.Text = "   Add Medicine";
            this.btnAddToPrescription.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddToPrescription.UseVisualStyleBackColor = false;
            this.btnAddToPrescription.Click += new System.EventHandler(this.btnAddToPrescription_Click);
            // 
            // lbPrescriptionID
            // 
            this.lbPrescriptionID.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lbPrescriptionID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(107)))));
            this.lbPrescriptionID.Location = new System.Drawing.Point(20, 310);
            this.lbPrescriptionID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbPrescriptionID.Name = "lbPrescriptionID";
            this.lbPrescriptionID.Size = new System.Drawing.Size(312, 50);
            this.lbPrescriptionID.TabIndex = 255;
            this.lbPrescriptionID.Text = "0000";
            this.lbPrescriptionID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // kfjaf
            // 
            this.kfjaf.AutoSize = true;
            this.kfjaf.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.kfjaf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(144)))), ((int)(((byte)(156)))));
            this.kfjaf.Location = new System.Drawing.Point(105, 270);
            this.kfjaf.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.kfjaf.Name = "kfjaf";
            this.kfjaf.Size = new System.Drawing.Size(143, 28);
            this.kfjaf.TabIndex = 254;
            this.kfjaf.Text = "Invoice / Bill ID";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.panel7.Controls.Add(this.dgPrescriptionDetails);
            this.panel7.Controls.Add(this.panel2);
            this.panel7.Controls.Add(this.panel16);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 69);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(20);
            this.panel7.Size = new System.Drawing.Size(1193, 573);
            this.panel7.TabIndex = 241;
            // 
            // dgPrescriptionDetails
            // 
            this.dgPrescriptionDetails.AllowUserToAddRows = false;
            this.dgPrescriptionDetails.AllowUserToDeleteRows = false;
            this.dgPrescriptionDetails.AllowUserToResizeRows = false;
            this.dgPrescriptionDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgPrescriptionDetails.BackgroundColor = System.Drawing.Color.White;
            this.dgPrescriptionDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgPrescriptionDetails.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgPrescriptionDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgPrescriptionDetails.ColumnHeadersHeight = 45;
            this.dgPrescriptionDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgPrescriptionDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgPrescriptionDetails.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgPrescriptionDetails.EnableHeadersVisualStyles = false;
            this.dgPrescriptionDetails.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(238)))), ((int)(((byte)(237)))));
            this.dgPrescriptionDetails.Location = new System.Drawing.Point(20, 75);
            this.dgPrescriptionDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgPrescriptionDetails.MultiSelect = false;
            this.dgPrescriptionDetails.Name = "dgPrescriptionDetails";
            this.dgPrescriptionDetails.ReadOnly = true;
            this.dgPrescriptionDetails.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(71)))), ((int)(((byte)(79)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(242)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(77)))), ((int)(((byte)(64)))));
            this.dgPrescriptionDetails.RowsDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.dgPrescriptionDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgPrescriptionDetails.RowTemplate.Height = 40;
            this.dgPrescriptionDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgPrescriptionDetails.Size = new System.Drawing.Size(1153, 314);
            this.dgPrescriptionDetails.TabIndex = 251;
            this.dgPrescriptionDetails.TabStop = false;
            this.dgPrescriptionDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgPrescriptionDetails_CellContentClick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnSavePrescription);
            this.panel2.Controls.Add(this.lblTotalAmount);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(20, 389);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1153, 164);
            this.panel2.TabIndex = 250;
            // 
            // btnSavePrescription
            // 
            this.btnSavePrescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePrescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(107)))));
            this.btnSavePrescription.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSavePrescription.FlatAppearance.BorderSize = 0;
            this.btnSavePrescription.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePrescription.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnSavePrescription.ForeColor = System.Drawing.Color.White;
            this.btnSavePrescription.Image = global::Clinic.Properties.Resources.Medicine_32;
            this.btnSavePrescription.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSavePrescription.Location = new System.Drawing.Point(823, 32);
            this.btnSavePrescription.Name = "btnSavePrescription";
            this.btnSavePrescription.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnSavePrescription.Size = new System.Drawing.Size(310, 100);
            this.btnSavePrescription.TabIndex = 278;
            this.btnSavePrescription.Text = "Save Invoice  ";
            this.btnSavePrescription.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnSavePrescription.UseVisualStyleBackColor = false;
            this.btnSavePrescription.Click += new System.EventHandler(this.btnSavePrescription_Click);
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblTotalAmount.Location = new System.Drawing.Point(30, 75);
            this.lblTotalAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(100, 62);
            this.lblTotalAmount.TabIndex = 270;
            this.lblTotalAmount.Text = "$ 0.00";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(144)))), ((int)(((byte)(156)))));
            this.label20.Location = new System.Drawing.Point(35, 35);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(134, 28);
            this.label20.TabIndex = 254;
            this.label20.Text = "Total Amount";
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.panel16.Controls.Add(this.label2);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel16.Location = new System.Drawing.Point(20, 20);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(1153, 55);
            this.panel16.TabIndex = 248;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(15, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 32);
            this.label2.TabIndex = 239;
            this.label2.Text = "Selected Items";
            // 
            // frmDirictSales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(1545, 642);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // جعلها بدون إطار لتطابق الشاشات السابقة الأنيقة
            this.Name = "frmDirictSales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Direct Pharmacy Sales";
            this.Load += new System.EventHandler(this.frmDirictSales_Load);
            this.panTitle.ResumeLayout(false);
            this.panTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPrescriptionDetails)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel16.ResumeLayout(false);
            this.panel16.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lbCurrentUser;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DataGridView dgPrescriptionDetails;
        private System.Windows.Forms.Button btnSavePrescription;
        private System.Windows.Forms.Label lbPrescriptionID;
        private System.Windows.Forms.Label kfjaf;
        private System.Windows.Forms.Button btnAddToPrescription;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label label20;
    }
}