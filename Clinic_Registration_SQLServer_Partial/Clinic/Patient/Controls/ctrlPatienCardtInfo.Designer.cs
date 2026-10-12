namespace Clinic.Controls
{
    partial class ctrlPatienCardtInfo
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

        private void InitializeComponent()
        {
            this.gbPatientAdditionalInfo = new System.Windows.Forms.GroupBox();
            this.lblEmergencyContact = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCreatedBy = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCreatedDate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMedicalHistory = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.lblBloodType = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblPatientID = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ctrlPersonCard1 = new Clinic.Controls.ctrlPersonCard();
            this.gbPatientAdditionalInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPatientAdditionalInfo
            // 
            this.gbPatientAdditionalInfo.BackColor = System.Drawing.Color.White;
            this.gbPatientAdditionalInfo.Controls.Add(this.lblEmergencyContact);
            this.gbPatientAdditionalInfo.Controls.Add(this.label3);
            this.gbPatientAdditionalInfo.Controls.Add(this.lblCreatedBy);
            this.gbPatientAdditionalInfo.Controls.Add(this.label4);
            this.gbPatientAdditionalInfo.Controls.Add(this.lblCreatedDate);
            this.gbPatientAdditionalInfo.Controls.Add(this.label2);
            this.gbPatientAdditionalInfo.Controls.Add(this.txtMedicalHistory);
            this.gbPatientAdditionalInfo.Controls.Add(this.label13);
            this.gbPatientAdditionalInfo.Controls.Add(this.lblBloodType);
            this.gbPatientAdditionalInfo.Controls.Add(this.label7);
            this.gbPatientAdditionalInfo.Controls.Add(this.lblPatientID);
            this.gbPatientAdditionalInfo.Controls.Add(this.label1);
            this.gbPatientAdditionalInfo.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.gbPatientAdditionalInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.gbPatientAdditionalInfo.Location = new System.Drawing.Point(4, 293);
            this.gbPatientAdditionalInfo.Margin = new System.Windows.Forms.Padding(4);
            this.gbPatientAdditionalInfo.Name = "gbPatientAdditionalInfo";
            this.gbPatientAdditionalInfo.Padding = new System.Windows.Forms.Padding(4);
            this.gbPatientAdditionalInfo.Size = new System.Drawing.Size(899, 208);
            this.gbPatientAdditionalInfo.TabIndex = 1;
            this.gbPatientAdditionalInfo.TabStop = false;
            this.gbPatientAdditionalInfo.Text = "Patient Medical Information";
            // 
            // lblEmergencyContact
            // 
            this.lblEmergencyContact.AutoSize = true;
            this.lblEmergencyContact.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblEmergencyContact.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.lblEmergencyContact.Location = new System.Drawing.Point(179, 151);
            this.lblEmergencyContact.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmergencyContact.Name = "lblEmergencyContact";
            this.lblEmergencyContact.Size = new System.Drawing.Size(46, 23);
            this.lblEmergencyContact.TabIndex = 12;
            this.lblEmergencyContact.Text = "[???]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label3.Location = new System.Drawing.Point(26, 151);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 23);
            this.label3.TabIndex = 11;
            this.label3.Text = "Emer. Cont:";
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.AutoSize = true;
            this.lblCreatedBy.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.lblCreatedBy.Location = new System.Drawing.Point(773, 171);
            this.lblCreatedBy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(46, 23);
            this.lblCreatedBy.TabIndex = 10;
            this.lblCreatedBy.Text = "[???]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label4.Location = new System.Drawing.Point(637, 171);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 23);
            this.label4.TabIndex = 9;
            this.label4.Text = "Created By:";
            // 
            // lblCreatedDate
            // 
            this.lblCreatedDate.AutoSize = true;
            this.lblCreatedDate.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCreatedDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.lblCreatedDate.Location = new System.Drawing.Point(491, 162);
            this.lblCreatedDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCreatedDate.Name = "lblCreatedDate";
            this.lblCreatedDate.Size = new System.Drawing.Size(46, 23);
            this.lblCreatedDate.TabIndex = 8;
            this.lblCreatedDate.Text = "[???]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label2.Location = new System.Drawing.Point(378, 162);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "Reg. Date:";
            // 
            // txtMedicalHistory
            // 
            this.txtMedicalHistory.BackColor = System.Drawing.Color.White;
            this.txtMedicalHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMedicalHistory.Enabled = false;
            this.txtMedicalHistory.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtMedicalHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.txtMedicalHistory.Location = new System.Drawing.Point(518, 41);
            this.txtMedicalHistory.Margin = new System.Windows.Forms.Padding(4);
            this.txtMedicalHistory.Multiline = true;
            this.txtMedicalHistory.Name = "txtMedicalHistory";
            this.txtMedicalHistory.ReadOnly = true;
            this.txtMedicalHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMedicalHistory.Size = new System.Drawing.Size(381, 90);
            this.txtMedicalHistory.TabIndex = 6;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label13.Location = new System.Drawing.Point(350, 42);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(142, 23);
            this.label13.TabIndex = 5;
            this.label13.Text = "Medical History:";
            // 
            // lblBloodType
            // 
            this.lblBloodType.AutoSize = true;
            this.lblBloodType.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblBloodType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblBloodType.Location = new System.Drawing.Point(179, 96);
            this.lblBloodType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBloodType.Name = "lblBloodType";
            this.lblBloodType.Size = new System.Drawing.Size(46, 23);
            this.lblBloodType.TabIndex = 4;
            this.lblBloodType.Text = "[???]";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label7.Location = new System.Drawing.Point(26, 96);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 23);
            this.label7.TabIndex = 3;
            this.label7.Text = "Blood Type:";
            // 
            // lblPatientID
            // 
            this.lblPatientID.AutoSize = true;
            this.lblPatientID.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblPatientID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.lblPatientID.Location = new System.Drawing.Point(179, 42);
            this.lblPatientID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPatientID.Name = "lblPatientID";
            this.lblPatientID.Size = new System.Drawing.Size(46, 23);
            this.lblPatientID.TabIndex = 2;
            this.lblPatientID.Text = "[???]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.label1.Location = new System.Drawing.Point(26, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Patient ID:";
            // 
            // ctrlPersonCard1
            // 
            this.ctrlPersonCard1.BackColor = System.Drawing.Color.White;
            this.ctrlPersonCard1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ctrlPersonCard1.Location = new System.Drawing.Point(4, 4);
            this.ctrlPersonCard1.Margin = new System.Windows.Forms.Padding(4);
            this.ctrlPersonCard1.Name = "ctrlPersonCard1";
            this.ctrlPersonCard1.Size = new System.Drawing.Size(899, 281);
            this.ctrlPersonCard1.TabIndex = 0;
            // 
            // ctrlPatienCardtInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gbPatientAdditionalInfo);
            this.Controls.Add(this.ctrlPersonCard1);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ctrlPatienCardtInfo";
            this.Size = new System.Drawing.Size(911, 508);
            this.gbPatientAdditionalInfo.ResumeLayout(false);
            this.gbPatientAdditionalInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        private Clinic.Controls.ctrlPersonCard ctrlPersonCard1;
        private System.Windows.Forms.GroupBox gbPatientAdditionalInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPatientID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblBloodType;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtMedicalHistory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCreatedDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCreatedBy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblEmergencyContact;
    }
}
