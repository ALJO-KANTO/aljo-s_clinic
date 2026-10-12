namespace Clinic.ControlsMain
{
    partial class ucAdminStation
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Users", 2, 2);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Doctors", 4, 4);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Patients", 3, 4);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Peaple", 5, 5);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("System Management", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucAdminStation));
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Mange Services", 6, 6);
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Mange Appoinment", 7, 7);
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("gi", 8, 8, new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("List Patient Blok");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Blok Patinet ");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("UnBlock Patient");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Patient Blok", 3, 3, new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Account Settings", 1, 1, new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode12});
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeSystemManagement = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRoleMangement = new System.Windows.Forms.Button();
            this.treeAccountSettings = new System.Windows.Forms.TreeView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1477, 31);
            this.flowLayoutPanel1.TabIndex = 33;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.treeSystemManagement);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnRoleMangement);
            this.panel1.Controls.Add(this.treeAccountSettings);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1477, 764);
            this.panel1.TabIndex = 34;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // treeSystemManagement
            // 
            this.treeSystemManagement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.treeSystemManagement.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeSystemManagement.Cursor = System.Windows.Forms.Cursors.Hand;
            this.treeSystemManagement.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeSystemManagement.ForeColor = System.Drawing.Color.White;
            this.treeSystemManagement.FullRowSelect = true;
            this.treeSystemManagement.ImageIndex = 0;
            this.treeSystemManagement.ImageList = this.imageList1;
            this.treeSystemManagement.Location = new System.Drawing.Point(14, 80);
            this.treeSystemManagement.Name = "treeSystemManagement";
            treeNode1.ImageIndex = 2;
            treeNode1.Name = "";
            treeNode1.SelectedImageIndex = 2;
            treeNode1.Text = "Users";
            treeNode2.ImageIndex = 4;
            treeNode2.Name = "";
            treeNode2.SelectedImageIndex = 4;
            treeNode2.Text = "Doctors";
            treeNode3.ImageIndex = 3;
            treeNode3.Name = "";
            treeNode3.SelectedImageIndex = 4;
            treeNode3.Text = "Patients";
            treeNode4.ImageIndex = 5;
            treeNode4.Name = "";
            treeNode4.SelectedImageIndex = 5;
            treeNode4.Text = "Peaple";
            treeNode5.Checked = true;
            treeNode5.ImageIndex = 0;
            treeNode5.Name = "";
            treeNode5.Text = "System Management";
            this.treeSystemManagement.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode5});
            this.treeSystemManagement.SelectedImageIndex = 0;
            this.treeSystemManagement.Size = new System.Drawing.Size(284, 176);
            this.treeSystemManagement.TabIndex = 37;
            this.treeSystemManagement.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeSystemManagement_NodeMouseClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "role 32.png");
            this.imageList1.Images.SetKeyName(1, "Account Setttings 64.png");
            this.imageList1.Images.SetKeyName(2, "User Options 32.png");
            this.imageList1.Images.SetKeyName(3, "Add Patient 32.png");
            this.imageList1.Images.SetKeyName(4, "doctor1 32.png");
            this.imageList1.Images.SetKeyName(5, "Person 32.png");
            this.imageList1.Images.SetKeyName(6, "Mange Services32.png");
            this.imageList1.Images.SetKeyName(7, "Mange Services 64.png");
            this.imageList1.Images.SetKeyName(8, "mange 32.png");
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(14, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(284, 68);
            this.panel2.TabIndex = 32;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.label3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(19, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(234, 45);
            this.label3.TabIndex = 6;
            this.label3.Text = "Admin Station";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::Clinic.Properties.Resources.role2_32;
            this.pictureBox1.Location = new System.Drawing.Point(26, 274);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(67, 44);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 36;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(94, 287);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 28);
            this.label2.TabIndex = 35;
            this.label2.Text = "Role Management";
            // 
            // btnRoleMangement
            // 
            this.btnRoleMangement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnRoleMangement.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRoleMangement.Location = new System.Drawing.Point(9, 264);
            this.btnRoleMangement.Margin = new System.Windows.Forms.Padding(5);
            this.btnRoleMangement.Name = "btnRoleMangement";
            this.btnRoleMangement.Size = new System.Drawing.Size(289, 87);
            this.btnRoleMangement.TabIndex = 34;
            this.btnRoleMangement.UseVisualStyleBackColor = false;
            // 
            // treeAccountSettings
            // 
            this.treeAccountSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.treeAccountSettings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeAccountSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.treeAccountSettings.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeAccountSettings.ForeColor = System.Drawing.Color.White;
            this.treeAccountSettings.FullRowSelect = true;
            this.treeAccountSettings.ImageIndex = 0;
            this.treeAccountSettings.ImageList = this.imageList1;
            this.treeAccountSettings.Location = new System.Drawing.Point(14, 371);
            this.treeAccountSettings.Name = "treeAccountSettings";
            treeNode6.ImageIndex = 6;
            treeNode6.Name = "Node4";
            treeNode6.SelectedImageIndex = 6;
            treeNode6.Text = "Mange Services";
            treeNode7.ImageIndex = 7;
            treeNode7.Name = "Node5";
            treeNode7.SelectedImageIndex = 7;
            treeNode7.Text = "Mange Appoinment";
            treeNode8.ImageIndex = 8;
            treeNode8.Name = "";
            treeNode8.SelectedImageIndex = 8;
            treeNode8.Text = "gi";
            treeNode9.ImageIndex = 3;
            treeNode9.Name = "Node6";
            treeNode9.Text = "List Patient Blok";
            treeNode10.ImageIndex = 3;
            treeNode10.Name = "Node7";
            treeNode10.Text = "Blok Patinet ";
            treeNode11.ImageIndex = 3;
            treeNode11.Name = "Node8";
            treeNode11.Text = "UnBlock Patient";
            treeNode12.ImageIndex = 3;
            treeNode12.Name = "";
            treeNode12.SelectedImageIndex = 3;
            treeNode12.Text = "Patient Blok";
            treeNode13.ImageIndex = 1;
            treeNode13.Name = "";
            treeNode13.SelectedImageIndex = 1;
            treeNode13.Text = "Account Settings";
            this.treeAccountSettings.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode13});
            this.treeAccountSettings.SelectedImageIndex = 0;
            this.treeAccountSettings.Size = new System.Drawing.Size(284, 133);
            this.treeAccountSettings.TabIndex = 33;
            this.treeAccountSettings.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeAccountSettings_NodeMouseClick);
            // 
            // ucAdminStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ucAdminStation";
            this.Size = new System.Drawing.Size(1477, 795);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView treeSystemManagement;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRoleMangement;
        private System.Windows.Forms.TreeView treeAccountSettings;
    }
}
