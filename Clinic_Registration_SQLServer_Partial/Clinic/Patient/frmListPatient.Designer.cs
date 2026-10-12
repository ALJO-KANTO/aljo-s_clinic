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


        // Visual-only dashboard styling. All existing data and event handlers are retained.
        private void ApplyPatientDashboardDesign()
        {
            System.Drawing.Color navy = System.Drawing.Color.FromArgb(13, 48, 80);
            System.Drawing.Color blue = System.Drawing.Color.FromArgb(0, 119, 231);
            System.Drawing.Color muted = System.Drawing.Color.FromArgb(93, 114, 142);
            System.Drawing.Color pale = System.Drawing.Color.FromArgb(241, 248, 253);
            System.Drawing.Color white = System.Drawing.Color.White;
            System.Drawing.Font body = new System.Drawing.Font("Segoe UI", 10F);

            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1260, 700);
            this.MinimumSize = new System.Drawing.Size(1200, 700); this.BackColor = pale;
            this.Font = body;
            this.Padding = new System.Windows.Forms.Padding(0);

            // Reuse the original controls, not replacements.
            this.Controls.Clear();
            this.panel2.Controls.Clear();
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Height = 78;
            this.panel2.BackColor = navy;
            this.lblTitle.Text = "Manage Patients";
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 21F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = white;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Location = new System.Drawing.Point(94, 8);
            this.panel2.Controls.Add(this.lblTitle);
            System.Windows.Forms.Label headerSub = DashLabel("View, search, add, update and manage patients in the clinic", 10F, System.Drawing.Color.FromArgb(192, 220, 240));
            headerSub.Location = new System.Drawing.Point(97, 47);
            headerSub.AutoSize = true;
            this.panel2.Controls.Add(headerSub);
            this.pbPersonImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPersonImage.BackColor = System.Drawing.Color.Transparent;
            this.pbPersonImage.Size = new System.Drawing.Size(64, 64);
            this.pbPersonImage.Location = new System.Drawing.Point(20, 7);
            this.panel2.Controls.Add(this.pbPersonImage);
            this.btnExit.Size = new System.Drawing.Size(52, 46);
            this.btnExit.Location = new System.Drawing.Point(this.ClientSize.Width - 70, 15);
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(27, 77, 112);
            this.panel2.Controls.Add(this.btnExit);
            this.Controls.Add(this.panel2);

            System.Windows.Forms.Panel footer = DashPanel(white);
            footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            footer.Height = 52;
            footer.Padding = new System.Windows.Forms.Padding(16, 7, 20, 7);
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.Width = 112;
            this.btnClose.Height = 36;
            this.btnClose.BackColor = white;
            this.btnClose.Image = null;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(205, 225, 242);
            this.btnClose.ForeColor = navy;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            footer.Controls.Add(this.btnClose);
            this.Controls.Add(footer);

            System.Windows.Forms.TableLayoutPanel columns = new System.Windows.Forms.TableLayoutPanel();
            columns.Dock = System.Windows.Forms.DockStyle.Fill;
            columns.ColumnCount = 2;
            columns.RowCount = 1;
            columns.Padding = new System.Windows.Forms.Padding(16, 12, 16, 10);
            columns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73F));
            columns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.Controls.Add(columns);

            // Correct WinForms docking order:
            // Header at top, footer at bottom, content fills remaining space.
            this.Controls.SetChildIndex(columns, 2);
            this.Controls.SetChildIndex(footer, 1);
            this.Controls.SetChildIndex(this.panel2, 0);

            System.Windows.Forms.TableLayoutPanel left = new System.Windows.Forms.TableLayoutPanel();
            left.Dock = System.Windows.Forms.DockStyle.Fill;
            left.ColumnCount = 1;
            left.RowCount = 3;
            left.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            left.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            left.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            columns.Controls.Add(left, 0, 0);

            System.Windows.Forms.Panel banner = DashPanel(System.Drawing.Color.FromArgb(214, 238, 255));
            banner.Margin = new System.Windows.Forms.Padding(0, 0, 12, 14);
            banner.Dock = System.Windows.Forms.DockStyle.Fill;
            System.Windows.Forms.Label bannerTitle = DashLabel("Patient Management", 23F, navy);
            bannerTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            bannerTitle.Location = new System.Drawing.Point(24, 9);
            bannerTitle.AutoSize = true;
            banner.Controls.Add(bannerTitle);
            System.Windows.Forms.Label bannerDesc = DashLabel("Keep track of patient information, medical history and visits.", 10F, muted);
            bannerDesc.Location = new System.Drawing.Point(26, 51);
            bannerDesc.AutoSize = true;
            banner.Controls.Add(bannerDesc);
            left.Controls.Add(banner, 0, 0);

            System.Windows.Forms.Panel search = DashPanel(white);
            search.Margin = new System.Windows.Forms.Padding(0, 0, 12, 14);
            search.Dock = System.Windows.Forms.DockStyle.Fill;
            System.Windows.Forms.Label searchTitle = DashLabel("⌕   Search & Filter", 14F, navy);
            searchTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            searchTitle.Location = new System.Drawing.Point(22, 14);
            searchTitle.AutoSize = true;
            search.Controls.Add(searchTitle);
            this.label1.Text = "Filter By";
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.label1.ForeColor = muted;
            this.label1.Location = new System.Drawing.Point(22, 59);
            this.label1.AutoSize = true;
            search.Controls.Add(this.label1);
            this.cbFilterBy.Location = new System.Drawing.Point(102, 54);
            this.cbFilterBy.Size = new System.Drawing.Size(205, 32);
            this.cbFilterBy.Font = body;
            search.Controls.Add(this.cbFilterBy);
            this.txtFilterValue.Location = new System.Drawing.Point(325, 54);
            this.txtFilterValue.Size = new System.Drawing.Size(330, 32);
            this.txtFilterValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtFilterValue.Font = body;
            search.Controls.Add(this.txtFilterValue);
            this.cbBloodType.Location = this.txtFilterValue.Location;
            this.cbBloodType.Size = new System.Drawing.Size(210, 32);
            search.Controls.Add(this.cbBloodType);
            search.Resize += (s, e) =>
            {
                int availableWidth = Math.Max(
                    100,
                    search.ClientSize.Width - 345);

                txtFilterValue.Width = availableWidth;

                cbBloodType.Width = Math.Min(
                    210,
                    availableWidth);
            };
            left.Controls.Add(search, 0, 1);

            System.Windows.Forms.Panel listCard = DashPanel(white);
            listCard.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            listCard.Dock = System.Windows.Forms.DockStyle.Fill;
            System.Windows.Forms.Label listTitle = DashLabel("☷   Patients List", 14F, navy);
            listTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            listTitle.Location = new System.Drawing.Point(22, 16);
            listTitle.AutoSize = true;
            listCard.Controls.Add(listTitle);
            System.Windows.Forms.Panel listFooter = DashPanel(white);
            listFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            listFooter.Height = 46;
            this.label2.Text = "# Records:";
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 12);
            this.label2.ForeColor = muted;
            this.lblRecordsCount.AutoSize = true;
            this.lblRecordsCount.Location = new System.Drawing.Point(123, 12);
            this.lblRecordsCount.ForeColor = blue;
            listFooter.Controls.Add(this.label2);
            listFooter.Controls.Add(this.lblRecordsCount);
            listCard.Controls.Add(listFooter);
            this.dgvPatient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPatient.Margin = new System.Windows.Forms.Padding(0);
            this.dgvPatient.BackgroundColor = white;
            this.dgvPatient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPatient.EnableHeadersVisualStyles = false;
            this.dgvPatient.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(16, 84, 132);
            this.dgvPatient.ColumnHeadersDefaultCellStyle.ForeColor = white;
            this.dgvPatient.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.dgvPatient.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(16, 84, 132);
            this.dgvPatient.ColumnHeadersDefaultCellStyle.SelectionForeColor = white;
            this.dgvPatient.ColumnHeadersHeight = 48;
            this.dgvPatient.RowTemplate.Height = 42;
            this.dgvPatient.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 250, 255);
            this.dgvPatient.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(217, 238, 255);
            this.dgvPatient.DefaultCellStyle.SelectionForeColor = navy;
            this.dgvPatient.GridColor = System.Drawing.Color.FromArgb(232, 240, 248);
            this.dgvPatient.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            this.dgvPatient.AutoSizeRowsMode =
                System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;

            this.dgvPatient.ScrollBars =
                System.Windows.Forms.ScrollBars.Both; listCard.Controls.Add(this.dgvPatient);
            System.Windows.Forms.Panel listHeading = DashPanel(white);
            listHeading.Dock = System.Windows.Forms.DockStyle.Top;
            listHeading.Height = 58;
            listHeading.Controls.Add(listTitle);
            listCard.Controls.Add(listHeading);
            left.Controls.Add(listCard, 0, 2);

            System.Windows.Forms.TableLayoutPanel right = new System.Windows.Forms.TableLayoutPanel();
            right.Dock = System.Windows.Forms.DockStyle.Fill;
            right.ColumnCount = 1;
            right.RowCount = 3;
            right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            columns.Controls.Add(right, 1, 0);

            System.Windows.Forms.Panel stats = DashPanel(white);
            stats.Dock = System.Windows.Forms.DockStyle.Fill;
            stats.Margin = new System.Windows.Forms.Padding(0, 0, 0, 14);
            stats.Controls.Add(DashLabel("PATIENT OVERVIEW", 10F, muted, 18, 10));
            stats.Controls.Add(DashLabel("Patient records", 12F, navy, 18, 35));
            stats.Controls.Add(DashLabel("Use the list to view all patients", 9F, muted, 18, 62));
            right.Controls.Add(stats, 0, 0);

            System.Windows.Forms.Panel actions = DashPanel(white);
            actions.Dock = System.Windows.Forms.DockStyle.Fill;
            actions.Margin = new System.Windows.Forms.Padding(0, 0, 0, 14);
            actions.Controls.Add(DashLabel("Patient Actions", 14F, navy, 18, 15));
            System.Windows.Forms.TableLayoutPanel actionGrid = new System.Windows.Forms.TableLayoutPanel();
            actionGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            actionGrid.Height = 157;
            actionGrid.Padding = new System.Windows.Forms.Padding(12, 0, 12, 12);
            actionGrid.ColumnCount = 2;
            actionGrid.RowCount = 2;
            actionGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            actionGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            actionGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            actionGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.btnAddPatient.Text = "＋  Add New Patient";
            this.btnAddPatient.Image = null;
            StyleActionButton(this.btnAddPatient, System.Drawing.Color.FromArgb(218, 250, 238), System.Drawing.Color.FromArgb(0, 125, 83));
            actionGrid.Controls.Add(this.btnAddPatient, 0, 0);
            System.Windows.Forms.Button btnEditVisual = DashAction("✎  Update Patient", System.Drawing.Color.FromArgb(227, 242, 255), blue);
            btnEditVisual.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            actionGrid.Controls.Add(btnEditVisual, 1, 0);
            System.Windows.Forms.Button btnViewVisual = DashAction("▣  View Details", System.Drawing.Color.FromArgb(255, 246, 222), System.Drawing.Color.FromArgb(158, 104, 0));
            btnViewVisual.Click += new System.EventHandler(this.showDetailsToolStripMenuItem_Click);
            actionGrid.Controls.Add(btnViewVisual, 0, 1);
            System.Windows.Forms.Button btnDeleteVisual = DashAction("✕  Delete Patient", System.Drawing.Color.FromArgb(255, 233, 239), System.Drawing.Color.FromArgb(192, 44, 74));
            btnDeleteVisual.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            actionGrid.Controls.Add(btnDeleteVisual, 1, 1);
            actions.Controls.Add(actionGrid);
            right.Controls.Add(actions, 0, 1);

            System.Windows.Forms.Panel details = DashPanel(white);
            details.Dock = System.Windows.Forms.DockStyle.Fill;
            details.Margin = new System.Windows.Forms.Padding(0);
            details.Controls.Add(DashLabel("Selected Patient", 14F, navy, 18, 18));
            details.Controls.Add(DashLabel("Select a row to view patient information", 10F, muted, 18, 66));
            details.Controls.Add(DashLabel("Right-click a row for more options", 9F, muted, 18, 106));
            right.Controls.Add(details, 0, 2);

            this.ResumeLayout(true);
        }

        private static System.Windows.Forms.Panel DashPanel(System.Drawing.Color background)
        {
            return new System.Windows.Forms.Panel { BackColor = background };
        }

        private static System.Windows.Forms.Label DashLabel(string text, float size, System.Drawing.Color color, int x = 0, int y = 0)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                AutoSize = true,
                Location = new System.Drawing.Point(x, y),
                Font = new System.Drawing.Font("Segoe UI", size),
                ForeColor = color,
                BackColor = System.Drawing.Color.Transparent
            };
        }

        private static void StyleActionButton(System.Windows.Forms.Button button, System.Drawing.Color back, System.Drawing.Color fore)
        {
            button.Dock = System.Windows.Forms.DockStyle.Fill;
            button.Margin = new System.Windows.Forms.Padding(5);
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = back;
            button.ForeColor = fore;
            button.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            button.Cursor = System.Windows.Forms.Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private static System.Windows.Forms.Button DashAction(string text, System.Drawing.Color back, System.Drawing.Color fore)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button { Text = text };
            StyleActionButton(button, back, fore);
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