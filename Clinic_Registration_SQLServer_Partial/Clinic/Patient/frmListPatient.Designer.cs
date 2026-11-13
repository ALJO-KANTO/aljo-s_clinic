using System;

namespace Clinic.Patient
{
    partial class frmListPatient
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cbBloodType = new System.Windows.Forms.ComboBox();
            this.dgvPatient = new System.Windows.Forms.DataGridView();
            this.cmsPeople = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblRecordsCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbFilterBy = new System.Windows.Forms.ComboBox();
            this.txtFilterValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAddPatient = new System.Windows.Forms.Button();
            this.pbPersonImage = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatient)).BeginInit();
            this.cmsPeople.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPersonImage)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbBloodType
            // 
            this.cbBloodType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBloodType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbBloodType.FormattingEnabled = true;
            this.cbBloodType.Items.AddRange(new object[] {
            "All",
            "Unknown",
            "A+",
            "A-",
            "B+",
            "B-",
            "AB+",
            "AB-",
            "O+",
            "O-"});
            this.cbBloodType.Location = new System.Drawing.Point(317, 267);
            this.cbBloodType.Name = "cbBloodType";
            this.cbBloodType.Size = new System.Drawing.Size(152, 31);
            this.cbBloodType.TabIndex = 114;
            this.cbBloodType.SelectedIndexChanged += new System.EventHandler(this.cbBloodType_SelectedIndexChanged);
            // 
            // dgvPatient
            // 
            this.dgvPatient.AllowUserToAddRows = false;
            this.dgvPatient.AllowUserToDeleteRows = false;
            this.dgvPatient.AllowUserToResizeRows = false;
            this.dgvPatient.BackgroundColor = System.Drawing.Color.White;
            this.dgvPatient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPatient.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvPatient.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPatient.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPatient.ColumnHeadersHeight = 40;
            this.dgvPatient.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPatient.ContextMenuStrip = this.cmsPeople;
            this.dgvPatient.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPatient.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.dgvPatient.Location = new System.Drawing.Point(20, 315);
            this.dgvPatient.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvPatient.MultiSelect = false;
            this.dgvPatient.Name = "dgvPatient";
            this.dgvPatient.ReadOnly = true;
            this.dgvPatient.RowHeadersVisible = false;
            this.dgvPatient.RowHeadersWidth = 51;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPatient.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPatient.RowTemplate.Height = 38;
            this.dgvPatient.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPatient.Size = new System.Drawing.Size(1126, 355);
            this.dgvPatient.TabIndex = 104;
            this.dgvPatient.TabStop = false;
            // 
            // cmsPeople
            // 
            this.cmsPeople.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmsPeople.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmsPeople.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDetailsToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripMenuItem1,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.cmsPeople.Name = "contextMenuStrip1";
            this.cmsPeople.Size = new System.Drawing.Size(186, 130);
            // 
            // showDetailsToolStripMenuItem
            // 
            this.showDetailsToolStripMenuItem.Image = global::Clinic.Properties.Resources.PersonDetails_32;
            this.showDetailsToolStripMenuItem.Name = "showDetailsToolStripMenuItem";
            this.showDetailsToolStripMenuItem.Size = new System.Drawing.Size(185, 30);
            this.showDetailsToolStripMenuItem.Text = "&Show Details";
            this.showDetailsToolStripMenuItem.Click += new System.EventHandler(this.showDetailsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewToolStripMenuItem,
            this.findToolStripMenuItem});
            this.toolStripMenuItem1.Image = global::Clinic.Properties.Resources.Add_Patient_32;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(185, 30);
            this.toolStripMenuItem1.Text = "Patient";
            // 
            // addNewToolStripMenuItem
            // 
            this.addNewToolStripMenuItem.Image = global::Clinic.Properties.Resources.patient;
            this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
            this.addNewToolStripMenuItem.Size = new System.Drawing.Size(228, 30);
            this.addNewToolStripMenuItem.Text = "Add &New ";
            this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Image = global::Clinic.Properties.Resources.patient;
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(228, 30);
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Image = global::Clinic.Properties.Resources.edit_32;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(185, 30);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::Clinic.Properties.Resources.Delete_32;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(185, 30);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // lblRecordsCount
            // 
            this.lblRecordsCount.AutoSize = true;
            this.lblRecordsCount.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblRecordsCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.lblRecordsCount.Location = new System.Drawing.Point(124, 687);
            this.lblRecordsCount.Name = "lblRecordsCount";
            this.lblRecordsCount.Size = new System.Drawing.Size(28, 25);
            this.lblRecordsCount.TabIndex = 109;
            this.lblRecordsCount.Text = "??";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label2.Location = new System.Drawing.Point(22, 687);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 25);
            this.label2.TabIndex = 108;
            this.label2.Text = "# Records:";
            // 
            // cbFilterBy
            // 
            this.cbFilterBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterBy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbFilterBy.FormattingEnabled = true;
            this.cbFilterBy.Items.AddRange(new object[] {
            "None",
            "Patient ID",
            "Person ID",
            "Full Name",
            "Blood Type",
            "Medical History",
            "Emergency Contact",
            "Country"});
            this.cbFilterBy.Location = new System.Drawing.Point(105, 267);
            this.cbFilterBy.Name = "cbFilterBy";
            this.cbFilterBy.Size = new System.Drawing.Size(205, 31);
            this.cbFilterBy.TabIndex = 107;
            this.cbFilterBy.SelectedIndexChanged += new System.EventHandler(this.cbFilterBy_SelectedIndexChanged);
            // 
            // txtFilterValue
            // 
            this.txtFilterValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilterValue.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtFilterValue.Location = new System.Drawing.Point(317, 267);
            this.txtFilterValue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtFilterValue.Name = "txtFilterValue";
            this.txtFilterValue.Size = new System.Drawing.Size(256, 31);
            this.txtFilterValue.TabIndex = 106;
            this.txtFilterValue.TextChanged += new System.EventHandler(this.txtFilterValue_TextChanged);
            this.txtFilterValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFilterValue_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label1.Location = new System.Drawing.Point(22, 270);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 25);
            this.label1.TabIndex = 105;
            this.label1.Text = "Filter By:";
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnClose.Image = global::Clinic.Properties.Resources.Close_32;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(1011, 682);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(135, 38);
            this.btnClose.TabIndex = 113;
            this.btnClose.Text = "   Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAddPatient
            // 
            this.btnAddPatient.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddPatient.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnAddPatient.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnAddPatient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPatient.Image = global::Clinic.Properties.Resources.patient;
            this.btnAddPatient.Location = new System.Drawing.Point(1068, 218);
            this.btnAddPatient.Name = "btnAddPatient";
            this.btnAddPatient.Size = new System.Drawing.Size(78, 75);
            this.btnAddPatient.TabIndex = 112;
            this.btnAddPatient.UseVisualStyleBackColor = true;
            this.btnAddPatient.Click += new System.EventHandler(this.btnAddPatient_Click);
            // 
            // pbPersonImage
            // 
            this.pbPersonImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbPersonImage.Image = global::Clinic.Properties.Resources.Patients_512;
            this.pbPersonImage.InitialImage = null;
            this.pbPersonImage.Location = new System.Drawing.Point(478, 122);
            this.pbPersonImage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbPersonImage.Name = "pbPersonImage";
            this.pbPersonImage.Size = new System.Drawing.Size(158, 93);
            this.pbPersonImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPersonImage.TabIndex = 110;
            this.pbPersonImage.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.panel2.Controls.Add(this.btnExit);
            this.panel2.Controls.Add(this.lblTitle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1166, 83);
            this.panel2.TabIndex = 119;
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
            this.btnExit.Location = new System.Drawing.Point(1049, 19);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 42);
            this.btnExit.TabIndex = 256;
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(460, 19);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(270, 45);
            this.lblTitle.TabIndex = 6;
            this.lblTitle.Text = "Manage Patients";
            // 
            // frmListPatient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1166, 740);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.cbBloodType);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAddPatient);
            this.Controls.Add(this.dgvPatient);
            this.Controls.Add(this.pbPersonImage);
            this.Controls.Add(this.lblRecordsCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbFilterBy);
            this.Controls.Add(this.txtFilterValue);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmListPatient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Patients";
            this.Load += new System.EventHandler(this.frmListPatient_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatient)).EndInit();
            this.cmsPeople.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPersonImage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.ApplyPatientDashboardDesign();

        }


        // Reference-inspired presentation. Existing patient logic and event handlers are retained.
        private void ApplyPatientDashboardDesign()
        {
            var navy = System.Drawing.Color.FromArgb(5, 47, 85);
            var blue = System.Drawing.Color.FromArgb(13, 109, 215);
            var pale = System.Drawing.Color.FromArgb(240, 247, 255);
            var muted = System.Drawing.Color.FromArgb(86, 115, 145);
            var white = System.Drawing.Color.White;
            this.SuspendLayout();
            this.Controls.Clear();
            this.panel2.Controls.Clear();
            this.ClientSize = new System.Drawing.Size(1340, 760);
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.BackColor = pale;
            this.Padding = new System.Windows.Forms.Padding(0);

            // Header matching the supplied reference: glossy medical icon, navy gradient.
            panel2.Dock = System.Windows.Forms.DockStyle.None;
            panel2.Height = 108;
            panel2.BackColor = navy;
            panel2.Padding = new System.Windows.Forms.Padding(0);
            panel2.Paint += (sender, e) =>
            {
                if (panel2.ClientRectangle.Width <= 0 || panel2.ClientRectangle.Height <= 0) return;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    panel2.ClientRectangle,
                    System.Drawing.Color.FromArgb(3, 40, 75),
                    System.Drawing.Color.FromArgb(3, 62, 106), 0F))
                    e.Graphics.FillRectangle(brush, panel2.ClientRectangle);

                // Subtle lower-right blue sweep, as in the reference header.
                using (var sweep = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int w = panel2.ClientSize.Width;
                    int h = panel2.ClientSize.Height;
                    sweep.AddPolygon(new System.Drawing.Point[] {
                        new System.Drawing.Point(System.Math.Max(0, w - 280), h),
                        new System.Drawing.Point(w, System.Math.Max(0, h - 45)),
                        new System.Drawing.Point(w, h)
                    });
                    using (var sb = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(25, 0, 128, 216)))
                        e.Graphics.FillPath(sb, sweep);
                }
            };
            // Add ManagePatientsHeaderIcon.png to Project Properties > Resources
            // with the exact resource name ManagePatientsHeaderIcon.
            var medicalIcon = new System.Windows.Forms.PictureBox
            {
                Location = new System.Drawing.Point(24, 10),
                Size = new System.Drawing.Size(90, 90),
                BackColor = System.Drawing.Color.Transparent,
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom,
                Image = global::Clinic.Properties.Resources.ManagePatientsHeaderIcon
            };
            panel2.Controls.Add(medicalIcon);
            lblTitle.Text = "Manage Patients";
            lblTitle.AutoSize = true;
            lblTitle.Location = new System.Drawing.Point(140, 13);
            lblTitle.BackColor = System.Drawing.Color.Transparent;
            lblTitle.ForeColor = white;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 23F, System.Drawing.FontStyle.Bold);
            panel2.Controls.Add(lblTitle);
            var subtitle = UiLabel("View, search, add, update and manage patients in the clinic", 10F,
                System.Drawing.Color.FromArgb(220, 232, 247), false);
            subtitle.Location = new System.Drawing.Point(142, 69);
            panel2.Controls.Add(subtitle);
            btnExit.Image = null;
            btnExit.Text = "×";
            btnExit.Font = new System.Drawing.Font("Segoe UI", 23F);
            btnExit.ForeColor = white;
            btnExit.BackColor = System.Drawing.Color.Transparent;
            btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Size = new System.Drawing.Size(50, 50);
            btnExit.Location = new System.Drawing.Point(ClientSize.Width - 69, 25);
            btnExit.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            panel2.Controls.Add(btnExit);

            // Footer is outside the main content area and always visible.
            var footer = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.None,
                Height = 67,
                BackColor = pale,
                Padding = new System.Windows.Forms.Padding(15, 10, 23, 10)
            };
            btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            btnClose.Width = 172;
            btnClose.Image = null;
            btnClose.Text = "Close";
            btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnClose.BackColor = white;
            btnClose.ForeColor = navy;
            btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = white;
            btnClose.FlatAppearance.MouseDownBackColor = white;
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Margin = new System.Windows.Forms.Padding(0);
            // Draw the entire rounded outline ourselves; no Region clipping of its edges.
            btnClose.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = new System.Drawing.Rectangle(2, 2, btnClose.Width - 5, btnClose.Height - 5);
                using (var path = RoundedRectanglePath(rect, 13))
                using (var fill = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                using (var outline = new System.Drawing.Pen(System.Drawing.Color.FromArgb(159, 192, 226), 1.5F))
                {
                    g.FillPath(fill, path);
                    g.DrawPath(outline, path);
                }
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(12, 102, 189), 1.8F))
                {
                    var cx = 38;
                    var cy = btnClose.Height / 2;
                    g.DrawEllipse(pen, cx - 11, cy - 11, 22, 22);
                    g.DrawLine(pen, cx - 4, cy - 4, cx + 4, cy + 4);
                    g.DrawLine(pen, cx + 4, cy - 4, cx - 4, cy + 4);
                }
                System.Windows.Forms.TextRenderer.DrawText(g, "Close", btnClose.Font,
                    new System.Drawing.Rectangle(60, 0, btnClose.Width - 68, btnClose.Height),
                    navy, System.Windows.Forms.TextFormatFlags.VerticalCenter |
                    System.Windows.Forms.TextFormatFlags.Left |
                    System.Windows.Forms.TextFormatFlags.SingleLine);
            };
            footer.Controls.Add(btnClose);

            var main = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.None,
                BackColor = pale,
                Padding = new System.Windows.Forms.Padding(14, 15, 14, 8),
                ColumnCount = 2,
                RowCount = 1
            };
            main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            Controls.Add(main);
            Controls.Add(footer);
            Controls.Add(panel2);
            // Explicit root layout avoids WinForms docking/z-order overlap.
            // Header, main content, and footer never occupy the same pixels.
            System.Action arrangeRoot = () =>
            {
                int w = this.ClientSize.Width;
                int h = this.ClientSize.Height;
                panel2.SetBounds(0, 0, w, 108);
                footer.SetBounds(0, h - 67, w, 67);
                main.SetBounds(0, 108, w, System.Math.Max(0, h - 175));
                btnExit.Location = new System.Drawing.Point(panel2.ClientSize.Width - 69, 25);
            };
            this.Resize += (sender, e) => arrangeRoot();
            arrangeRoot();

            var left = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = pale,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new System.Windows.Forms.Padding(0, 0, 12, 0)
            };
            left.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            left.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            main.Controls.Add(left, 0, 0);

            var search = UiCard();
            search.Margin = new System.Windows.Forms.Padding(0, 0, 0, 14);
            var searchHeading = UiHeading("⌕", "Search Filter", blue, navy);
            searchHeading.Dock = System.Windows.Forms.DockStyle.Top;
            search.Controls.Add(searchHeading);
            label1.Text = "Filter By:";
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            label1.ForeColor = navy;
            label1.Location = new System.Drawing.Point(24, 88);
            search.Controls.Add(label1);
            cbFilterBy.Location = new System.Drawing.Point(136, 81);
            cbFilterBy.Size = new System.Drawing.Size(235, 34);
            cbFilterBy.Font = new System.Drawing.Font("Segoe UI", 11F);
            cbFilterBy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            search.Controls.Add(cbFilterBy);
            txtFilterValue.Location = new System.Drawing.Point(386, 81);
            txtFilterValue.Size = new System.Drawing.Size(235, 34);
            txtFilterValue.Font = new System.Drawing.Font("Segoe UI", 11F);
            search.Controls.Add(txtFilterValue);
            cbBloodType.Location = txtFilterValue.Location;
            cbBloodType.Size = txtFilterValue.Size;
            search.Controls.Add(cbBloodType);
            search.Resize += (sender, e) =>
            {
                int width = System.Math.Max(120, search.ClientSize.Width - 413);
                txtFilterValue.Width = width;
                cbBloodType.Width = width;
            };
            left.Controls.Add(search, 0, 0);

            var list = UiCard();
            list.Margin = new System.Windows.Forms.Padding(0);
            var listHeading = UiHeading("☷", "Patients List", blue, navy);
            listHeading.Dock = System.Windows.Forms.DockStyle.Top;
            list.Controls.Add(listHeading);
            var recordFooter = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Height = 40,
                BackColor = white
            };
            label2.Text = "# Records:";
            label2.Location = new System.Drawing.Point(20, 9);
            label2.AutoSize = true;
            label2.ForeColor = muted;
            lblRecordsCount.Location = new System.Drawing.Point(119, 9);
            lblRecordsCount.AutoSize = true;
            lblRecordsCount.ForeColor = blue;
            recordFooter.Controls.Add(label2);
            recordFooter.Controls.Add(lblRecordsCount);
            list.Controls.Add(recordFooter);
            dgvPatient.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvPatient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvPatient.BackgroundColor = white;
            dgvPatient.EnableHeadersVisualStyles = false;
            dgvPatient.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(8, 95, 161);
            dgvPatient.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgvPatient.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(8, 95, 161);
            dgvPatient.ColumnHeadersDefaultCellStyle.SelectionForeColor = white;
            dgvPatient.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            dgvPatient.ColumnHeadersHeight = 50;
            dgvPatient.RowTemplate.Height = 43;
            dgvPatient.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgvPatient.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(215, 235, 255);
            dgvPatient.DefaultCellStyle.SelectionForeColor = navy;
            dgvPatient.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(237, 246, 255);
            dgvPatient.GridColor = System.Drawing.Color.FromArgb(229, 239, 251);
            dgvPatient.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            list.Controls.Add(dgvPatient);
            // Header above grid, record count below grid, no overlap.
            list.Controls.SetChildIndex(listHeading, 0);
            list.Controls.SetChildIndex(dgvPatient, 1);
            list.Controls.SetChildIndex(recordFooter, 2);
            left.Controls.Add(list, 0, 1);

            var right = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = pale,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new System.Windows.Forms.Padding(0)
            };
            right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            main.Controls.Add(right, 1, 0);

            var actions = UiCard();
            actions.Margin = new System.Windows.Forms.Padding(0, 0, 0, 14);
            var actionsHeading = UiHeading("⚙", "Patient Actions", blue, navy);
            actionsHeading.Dock = System.Windows.Forms.DockStyle.Top;
            actions.Controls.Add(actionsHeading);
            var grid = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Height = 196,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = white,
                Padding = new System.Windows.Forms.Padding(12, 0, 12, 12)
            };
            grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));
            grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));
            grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
            grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
            btnAddPatient.Image = null;
            btnAddPatient.Text = "✚  Add New\nPatient";
            StyleTile(btnAddPatient, System.Drawing.Color.FromArgb(218, 250, 237), System.Drawing.Color.FromArgb(0, 133, 76));
            grid.Controls.Add(btnAddPatient, 0, 0);
            var edit = UiAction("✎  Update\nPatient", System.Drawing.Color.FromArgb(225, 239, 255), blue);
            edit.Click += editToolStripMenuItem_Click;
            grid.Controls.Add(edit, 1, 0);
            var view = UiAction("⌕  View\nDetails", System.Drawing.Color.FromArgb(255, 244, 220), System.Drawing.Color.FromArgb(199, 113, 0));
            view.Click += showDetailsToolStripMenuItem_Click;
            grid.Controls.Add(view, 0, 1);
            var delete = UiAction("⊗  Delete\nPatient", System.Drawing.Color.FromArgb(255, 230, 237), System.Drawing.Color.FromArgb(192, 30, 59));
            delete.Click += deleteToolStripMenuItem_Click;
            grid.Controls.Add(delete, 1, 1);
            actions.Controls.Add(grid);
            actions.Controls.SetChildIndex(actionsHeading, 0);
            actions.Controls.SetChildIndex(grid, 1);
            right.Controls.Add(actions, 0, 0);

            var selected = UiCard();
            selected.Margin = new System.Windows.Forms.Padding(0);
            selected.Controls.Add(UiHeading("♙", "Selected Patient", blue, navy));
            var selectedBody = new System.Windows.Forms.Label
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Text = "Select a row to view patient information",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = muted,
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Padding = new System.Windows.Forms.Padding(15),
                BackColor = white
            };
            selected.Controls.Add(selectedBody);
            // The heading must be docked and placed before the filling body.
            var selectedHeading = selected.Controls[0];
            selectedHeading.Dock = System.Windows.Forms.DockStyle.Top;
            selected.Controls.SetChildIndex(selectedHeading, 0);
            selected.Controls.SetChildIndex(selectedBody, 1);
            dgvPatient.SelectionChanged += (sender, e) =>
            {
                if (dgvPatient.CurrentRow == null || dgvPatient.CurrentRow.IsNewRow) return;
                var row = dgvPatient.CurrentRow;
                string id = dgvPatient.Columns.Contains("PatientID")
                    ? System.Convert.ToString(row.Cells["PatientID"].Value) : "";
                string name = dgvPatient.Columns.Contains("FullName")
                    ? System.Convert.ToString(row.Cells["FullName"].Value) : "";
                selectedBody.Text = "Patient ID: " + id + "\n" + name + "\n\nRight-click for more options";
            };
            right.Controls.Add(selected, 0, 1);
            this.ResumeLayout(true);
        }

        private static System.Windows.Forms.Panel UiCard()
        {
            var panel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = System.Drawing.Color.White
            };
            RoundControl(panel, 16);
            return panel;
        }

        private static System.Windows.Forms.Label UiLabel(string text, float size,
            System.Drawing.Color color, bool bold)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                AutoSize = true,
                BackColor = System.Drawing.Color.Transparent,
                ForeColor = color,
                Font = new System.Drawing.Font("Segoe UI", size,
                    bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular)
            };
        }

        private static System.Windows.Forms.Panel UiHeading(string icon, string title,
            System.Drawing.Color iconColor, System.Drawing.Color textColor)
        {
            var panel = new System.Windows.Forms.Panel
            {
                Height = 66,
                BackColor = System.Drawing.Color.FromArgb(233, 243, 255)
            };
            // Docked row avoids the first letter being covered by the icon at DPI scaling.
            var layout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = System.Drawing.Color.Transparent,
                Padding = new System.Windows.Forms.Padding(18, 0, 10, 0),
                Margin = new System.Windows.Forms.Padding(0)
            };
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(
                System.Windows.Forms.SizeType.Absolute, 52F));
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(
                System.Windows.Forms.SizeType.Percent, 100F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(
                System.Windows.Forms.SizeType.Percent, 100F));
            var symbol = new System.Windows.Forms.Label
            {
                Text = icon,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI Symbol", 23F, System.Drawing.FontStyle.Regular),
                ForeColor = iconColor,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Margin = new System.Windows.Forms.Padding(0),
                AutoSize = false
            };
            var heading = new System.Windows.Forms.Label
            {
                Text = title,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI Semibold", 15F, System.Drawing.FontStyle.Bold),
                ForeColor = textColor,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Margin = new System.Windows.Forms.Padding(8, 0, 0, 0),
                AutoSize = false,
                AutoEllipsis = true
            };
            layout.Controls.Add(symbol, 0, 0);
            layout.Controls.Add(heading, 1, 0);
            panel.Controls.Add(layout);
            return panel;
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRectanglePath(
            System.Drawing.Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void RoundControl(System.Windows.Forms.Control control, int radius)
        {
            System.Action update = () => {
                if (control.Width < 2 || control.Height < 2) return;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int d = System.Math.Min(radius * 2, System.Math.Min(control.Width, control.Height));
                    path.AddArc(0, 0, d, d, 180, 90);
                    path.AddArc(control.Width - d, 0, d, d, 270, 90);
                    path.AddArc(control.Width - d, control.Height - d, d, d, 0, 90);
                    path.AddArc(0, control.Height - d, d, d, 90, 90);
                    path.CloseFigure();
                    var old = control.Region;
                    control.Region = new System.Drawing.Region(path);
                    if (old != null) old.Dispose();
                }
            };
            control.SizeChanged += (sender, e) => update();
            update();
        }

        private static void StyleTile(System.Windows.Forms.Button button,
            System.Drawing.Color background, System.Drawing.Color foreground)
        {
            button.Dock = System.Windows.Forms.DockStyle.Fill;
            button.Margin = new System.Windows.Forms.Padding(6);
            button.Image = null;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            button.BackColor = background;
            button.ForeColor = foreground;
            button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            button.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            button.Cursor = System.Windows.Forms.Cursors.Hand;
            RoundControl(button, 12);
        }

        private static System.Windows.Forms.Button UiAction(string text,
            System.Drawing.Color background, System.Drawing.Color foreground)
        {
            var button = new System.Windows.Forms.Button { Text = text };
            StyleTile(button, background, foreground);
            return button;
        }

        #endregion

        private System.Windows.Forms.ComboBox cbBloodType;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAddPatient;
        private System.Windows.Forms.DataGridView dgvPatient;
        private System.Windows.Forms.ContextMenuStrip cmsPeople;
        private System.Windows.Forms.ToolStripMenuItem showDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbPersonImage;
        private System.Windows.Forms.Label lblRecordsCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbFilterBy;
        private System.Windows.Forms.TextBox txtFilterValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ToolStripMenuItem addNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
    }
}