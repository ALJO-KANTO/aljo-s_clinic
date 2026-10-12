namespace Clinic.Medical_Services.Casher
{
    partial class frmIssueInvoice
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCenerateAndFinalizeInVoice = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lbSubtotal = new System.Windows.Forms.Label();
            this.lbDiscoutns = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.lbFinalTotal = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.dgBill = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSearchByPatientName = new System.Windows.Forms.TextBox();
            this.lbPlaceHoleder = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnPreviewInvoice = new System.Windows.Forms.Button();
            this.btnSaveDraft = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceDate = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPatientID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPatientName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbCoPay = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lbRemainingAmount = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lbCurrentUser = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.panTitle = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgBill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.panel2.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(783, 420);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2, 249);
            this.panel2.TabIndex = 275;
            // 
            // btnCenerateAndFinalizeInVoice
            // 
            this.btnCenerateAndFinalizeInVoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            this.btnCenerateAndFinalizeInVoice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCenerateAndFinalizeInVoice.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCenerateAndFinalizeInVoice.ForeColor = System.Drawing.Color.White;
            this.btnCenerateAndFinalizeInVoice.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCenerateAndFinalizeInVoice.Location = new System.Drawing.Point(678, 697);
            this.btnCenerateAndFinalizeInVoice.Name = "btnCenerateAndFinalizeInVoice";
            this.btnCenerateAndFinalizeInVoice.Size = new System.Drawing.Size(601, 48);
            this.btnCenerateAndFinalizeInVoice.TabIndex = 280;
            this.btnCenerateAndFinalizeInVoice.Text = "Cenerate and Finalize InVoice";
            this.btnCenerateAndFinalizeInVoice.UseVisualStyleBackColor = false;
            this.btnCenerateAndFinalizeInVoice.Click += new System.EventHandler(this.btnCenerateAndFinalizeInVoice_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(794, 462);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 31);
            this.label9.TabIndex = 282;
            this.label9.Text = "Subtotal";
            // 
            // lbSubtotal
            // 
            this.lbSubtotal.AutoSize = true;
            this.lbSubtotal.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSubtotal.ForeColor = System.Drawing.Color.White;
            this.lbSubtotal.Location = new System.Drawing.Point(1158, 462);
            this.lbSubtotal.Name = "lbSubtotal";
            this.lbSubtotal.Size = new System.Drawing.Size(46, 31);
            this.lbSubtotal.TabIndex = 283;
            this.lbSubtotal.Text = "$ 0";
            // 
            // lbDiscoutns
            // 
            this.lbDiscoutns.AutoSize = true;
            this.lbDiscoutns.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDiscoutns.ForeColor = System.Drawing.Color.White;
            this.lbDiscoutns.Location = new System.Drawing.Point(1158, 521);
            this.lbDiscoutns.Name = "lbDiscoutns";
            this.lbDiscoutns.Size = new System.Drawing.Size(46, 31);
            this.lbDiscoutns.TabIndex = 287;
            this.lbDiscoutns.Text = "$ 0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(794, 523);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(120, 31);
            this.label13.TabIndex = 286;
            this.label13.Text = "Discoutns";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.panel7.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel7.Location = new System.Drawing.Point(783, 596);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(473, 3);
            this.panel7.TabIndex = 276;
            // 
            // lbFinalTotal
            // 
            this.lbFinalTotal.AutoSize = true;
            this.lbFinalTotal.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFinalTotal.ForeColor = System.Drawing.Color.White;
            this.lbFinalTotal.Location = new System.Drawing.Point(1158, 606);
            this.lbFinalTotal.Name = "lbFinalTotal";
            this.lbFinalTotal.Size = new System.Drawing.Size(46, 31);
            this.lbFinalTotal.TabIndex = 289;
            this.lbFinalTotal.Text = "$ 0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Location = new System.Drawing.Point(794, 606);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(125, 31);
            this.label15.TabIndex = 288;
            this.label15.Text = "Final Total";
            // 
            // dgBill
            // 
            this.dgBill.AllowUserToAddRows = false;
            this.dgBill.AllowUserToDeleteRows = false;
            this.dgBill.AllowUserToResizeColumns = false;
            this.dgBill.AllowUserToResizeRows = false;
            this.dgBill.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgBill.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgBill.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.dgBill.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgBill.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgBill.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgBill.ColumnHeadersHeight = 40;
            this.dgBill.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgBill.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgBill.EnableHeadersVisualStyles = false;
            this.dgBill.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.dgBill.Location = new System.Drawing.Point(536, 204);
            this.dgBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgBill.MultiSelect = false;
            this.dgBill.Name = "dgBill";
            this.dgBill.ReadOnly = true;
            this.dgBill.RowHeadersVisible = false;
            this.dgBill.RowHeadersWidth = 51;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgBill.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgBill.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgBill.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(68)))));
            this.dgBill.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dgBill.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgBill.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(90)))));
            this.dgBill.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgBill.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgBill.RowTemplate.Height = 38;
            this.dgBill.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgBill.Size = new System.Drawing.Size(723, 185);
            this.dgBill.TabIndex = 290;
            this.dgBill.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(535, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 31);
            this.label1.TabIndex = 291;
            this.label1.Text = "ITEM LISTING";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(536, 397);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(725, 4);
            this.panel1.TabIndex = 292;
            // 
            // txtSearchByPatientName
            // 
            this.txtSearchByPatientName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.txtSearchByPatientName.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchByPatientName.ForeColor = System.Drawing.Color.White;
            this.txtSearchByPatientName.Location = new System.Drawing.Point(20, 120);
            this.txtSearchByPatientName.Name = "txtSearchByPatientName";
            this.txtSearchByPatientName.Size = new System.Drawing.Size(1233, 38);
            this.txtSearchByPatientName.TabIndex = 293;
            this.txtSearchByPatientName.TextChanged += new System.EventHandler(this.txtSearchByPatientName_TextChanged);
            this.txtSearchByPatientName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchByPatientName_KeyDown);
            this.txtSearchByPatientName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearchByPatientName_KeyPress);
            // 
            // lbPlaceHoleder
            // 
            this.lbPlaceHoleder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.lbPlaceHoleder.ForeColor = System.Drawing.Color.Gray;
            this.lbPlaceHoleder.Location = new System.Drawing.Point(35, 125);
            this.lbPlaceHoleder.Name = "lbPlaceHoleder";
            this.lbPlaceHoleder.Size = new System.Drawing.Size(330, 23);
            this.lbPlaceHoleder.TabIndex = 295;
            this.lbPlaceHoleder.Text = "Search by Bill ID";
            this.lbPlaceHoleder.Click += new System.EventHandler(this.lbPlaceHolederBillID_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.panel4.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel4.Location = new System.Drawing.Point(512, 168);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(2, 482);
            this.panel4.TabIndex = 296;
            // 
            // btnPreviewInvoice
            // 
            this.btnPreviewInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.btnPreviewInvoice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreviewInvoice.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreviewInvoice.ForeColor = System.Drawing.Color.White;
            this.btnPreviewInvoice.Image = global::Clinic.Properties.Resources.bill_32;
            this.btnPreviewInvoice.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreviewInvoice.Location = new System.Drawing.Point(349, 697);
            this.btnPreviewInvoice.Name = "btnPreviewInvoice";
            this.btnPreviewInvoice.Size = new System.Drawing.Size(292, 48);
            this.btnPreviewInvoice.TabIndex = 297;
            this.btnPreviewInvoice.TabStop = false;
            this.btnPreviewInvoice.Text = "Preview Invoice";
            this.btnPreviewInvoice.UseVisualStyleBackColor = false;
            this.btnPreviewInvoice.Click += new System.EventHandler(this.btnPreviewInvoice_Click);
            // 
            // btnSaveDraft
            // 
            this.btnSaveDraft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.btnSaveDraft.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveDraft.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveDraft.ForeColor = System.Drawing.Color.White;
            this.btnSaveDraft.Image = global::Clinic.Properties.Resources.Save_32;
            this.btnSaveDraft.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSaveDraft.Location = new System.Drawing.Point(23, 697);
            this.btnSaveDraft.Name = "btnSaveDraft";
            this.btnSaveDraft.Size = new System.Drawing.Size(292, 48);
            this.btnSaveDraft.TabIndex = 298;
            this.btnSaveDraft.TabStop = false;
            this.btnSaveDraft.Text = "Save Draft";
            this.btnSaveDraft.UseVisualStyleBackColor = false;
            this.btnSaveDraft.Click += new System.EventHandler(this.btnSaveDraft_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(23, 213);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(181, 31);
            this.label4.TabIndex = 300;
            this.label4.Text = "Invoice Number";
            // 
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.txtInvoiceNumber.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInvoiceNumber.ForeColor = System.Drawing.Color.White;
            this.txtInvoiceNumber.Location = new System.Drawing.Point(20, 247);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.ReadOnly = true;
            this.txtInvoiceNumber.Size = new System.Drawing.Size(448, 38);
            this.txtInvoiceNumber.TabIndex = 299;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(18, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 31);
            this.label2.TabIndex = 301;
            this.label2.Text = "INVOICE DETAILS";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(17, 306);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 31);
            this.label3.TabIndex = 303;
            this.label3.Text = "Invoice Date";
            // 
            // txtInvoiceDate
            // 
            this.txtInvoiceDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.txtInvoiceDate.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInvoiceDate.ForeColor = System.Drawing.Color.White;
            this.txtInvoiceDate.Location = new System.Drawing.Point(20, 340);
            this.txtInvoiceDate.Name = "txtInvoiceDate";
            this.txtInvoiceDate.ReadOnly = true;
            this.txtInvoiceDate.Size = new System.Drawing.Size(448, 38);
            this.txtInvoiceDate.TabIndex = 302;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.panel5.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel5.Location = new System.Drawing.Point(20, 397);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(483, 2);
            this.panel5.TabIndex = 293;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(18, 574);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 31);
            this.label6.TabIndex = 308;
            this.label6.Text = "Pateint ID";
            // 
            // txtPatientID
            // 
            this.txtPatientID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.txtPatientID.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPatientID.ForeColor = System.Drawing.Color.White;
            this.txtPatientID.Location = new System.Drawing.Point(16, 608);
            this.txtPatientID.Name = "txtPatientID";
            this.txtPatientID.ReadOnly = true;
            this.txtPatientID.Size = new System.Drawing.Size(448, 38);
            this.txtPatientID.TabIndex = 307;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(10, 420);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(270, 31);
            this.label7.TabIndex = 306;
            this.label7.Text = "PATIENT INFORMATION";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI Semibold", 13.8F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(18, 476);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(153, 31);
            this.label10.TabIndex = 305;
            this.label10.Text = "Patient Name";
            // 
            // txtPatientName
            // 
            this.txtPatientName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(72)))), ((int)(((byte)(75)))));
            this.txtPatientName.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPatientName.ForeColor = System.Drawing.Color.White;
            this.txtPatientName.Location = new System.Drawing.Point(16, 510);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.ReadOnly = true;
            this.txtPatientName.Size = new System.Drawing.Size(448, 38);
            this.txtPatientName.TabIndex = 304;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(794, 418);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(185, 31);
            this.label8.TabIndex = 309;
            this.label8.Text = "BILL SUMMARY";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(794, 494);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 31);
            this.label12.TabIndex = 310;
            this.label12.Text = "Co-Pay";
            // 
            // lbCoPay
            // 
            this.lbCoPay.AutoSize = true;
            this.lbCoPay.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCoPay.ForeColor = System.Drawing.Color.White;
            this.lbCoPay.Location = new System.Drawing.Point(1158, 493);
            this.lbCoPay.Name = "lbCoPay";
            this.lbCoPay.Size = new System.Drawing.Size(46, 31);
            this.lbCoPay.TabIndex = 311;
            this.lbCoPay.Text = "$ 0";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(190)))));
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = global::Clinic.Properties.Resources.Financials;
            this.pictureBox2.Location = new System.Drawing.Point(1190, 705);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(63, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 312;
            this.pictureBox2.TabStop = false;
            // 
            // lbRemainingAmount
            // 
            this.lbRemainingAmount.AutoSize = true;
            this.lbRemainingAmount.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemainingAmount.ForeColor = System.Drawing.Color.White;
            this.lbRemainingAmount.Location = new System.Drawing.Point(1158, 554);
            this.lbRemainingAmount.Name = "lbRemainingAmount";
            this.lbRemainingAmount.Size = new System.Drawing.Size(46, 31);
            this.lbRemainingAmount.TabIndex = 316;
            this.lbRemainingAmount.Text = "$ 0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(794, 554);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(239, 31);
            this.label11.TabIndex = 315;
            this.label11.Text = "⚠️Remaining (Due): ";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.lblTitle.Location = new System.Drawing.Point(373, 17);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(478, 45);
            this.lblTitle.TabIndex = 236;
            this.lblTitle.Text = "ISSUE INVOICE - System View";
            // 
            // lbCurrentUser
            // 
            this.lbCurrentUser.AutoSize = true;
            this.lbCurrentUser.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.lbCurrentUser.Location = new System.Drawing.Point(927, 45);
            this.lbCurrentUser.Name = "lbCurrentUser";
            this.lbCurrentUser.Size = new System.Drawing.Size(60, 31);
            this.lbCurrentUser.TabIndex = 241;
            this.lbCurrentUser.Text = "[???]";
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnExit.Image = global::Clinic.Properties.Resources.sign_out_32__2;
            this.btnExit.Location = new System.Drawing.Point(1106, 22);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 42);
            this.btnExit.TabIndex = 254;
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.label5.Location = new System.Drawing.Point(902, 12);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(156, 31);
            this.label5.TabIndex = 240;
            this.label5.Text = "Current User:";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.Image = global::Clinic.Properties.Resources.bill_32;
            this.pictureBox3.Location = new System.Drawing.Point(287, 17);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(70, 59);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 257;
            this.pictureBox3.TabStop = false;
            // 
            // panTitle
            // 
            this.panTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.panTitle.Controls.Add(this.pictureBox3);
            this.panTitle.Controls.Add(this.label5);
            this.panTitle.Controls.Add(this.btnExit);
            this.panTitle.Controls.Add(this.lbCurrentUser);
            this.panTitle.Controls.Add(this.lblTitle);
            this.panTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTitle.Location = new System.Drawing.Point(20, 20);
            this.panTitle.Margin = new System.Windows.Forms.Padding(10, 5, 5, 5);
            this.panTitle.Name = "panTitle";
            this.panTitle.Size = new System.Drawing.Size(1245, 89);
            this.panTitle.TabIndex = 253;
            // 
            // frmIssueInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1285, 751);
            this.Controls.Add(this.lbRemainingAmount);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lbCoPay);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtInvoiceDate);
            this.Controls.Add(this.txtInvoiceNumber);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPatientID);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtPatientName);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSaveDraft);
            this.Controls.Add(this.btnPreviewInvoice);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.lbPlaceHoleder);
            this.Controls.Add(this.txtSearchByPatientName);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgBill);
            this.Controls.Add(this.lbFinalTotal);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.lbDiscoutns);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lbSubtotal);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnCenerateAndFinalizeInVoice);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Cornsilk;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmIssueInvoice";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAllPrescriptions";
            ((System.ComponentModel.ISupportInitialize)(this.dgBill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panTitle.ResumeLayout(false);
            this.panTitle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCenerateAndFinalizeInVoice;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbSubtotal;
        private System.Windows.Forms.Label lbDiscoutns;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lbFinalTotal;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridView dgBill;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSearchByPatientName;
        private System.Windows.Forms.Label lbPlaceHoleder;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnPreviewInvoice;
        private System.Windows.Forms.Button btnSaveDraft;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInvoiceDate;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPatientID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtPatientName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbCoPay;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lbRemainingAmount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lbCurrentUser;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Panel panTitle;
    }
}