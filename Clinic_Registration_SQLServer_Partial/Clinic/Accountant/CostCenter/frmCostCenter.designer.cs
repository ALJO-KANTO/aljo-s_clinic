namespace YamyProject
{
    partial class frmCostCenter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();

            // =========================================================
            // MAIN CONTAINERS
            // =========================================================

            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();

            // =========================================================
            // PAGE HEADER
            // =========================================================

            this.lblPageTitle = new System.Windows.Forms.Label();
            this.lblPageDescription = new System.Windows.Forms.Label();

            // =========================================================
            // TYPE CARD
            // =========================================================

            this.cardType = new System.Windows.Forms.Panel();
            this.lblTypeTitle = new System.Windows.Forms.Label();
            this.lblTypeDescription = new System.Windows.Forms.Label();
            this.radioMain = new System.Windows.Forms.RadioButton();
            this.radioSub = new System.Windows.Forms.RadioButton();

            // =========================================================
            // PARENT CARD
            // =========================================================

            this.cardParent = new System.Windows.Forms.Panel();
            this.lblParentTitle = new System.Windows.Forms.Label();
            this.lblParentDescription = new System.Windows.Forms.Label();
            this.lblMain = new System.Windows.Forms.Label();
            this.cmbCostCenter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lnkEditCategory = new System.Windows.Forms.LinkLabel();

            // =========================================================
            // CODE CARD
            // =========================================================

            this.cardCode = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCodeHint = new System.Windows.Forms.Label();
            this.txtCode = new Guna.UI2.WinForms.Guna2TextBox();

            // =========================================================
            // NAME CARD
            // =========================================================

            this.cardName = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNameHint = new System.Windows.Forms.Label();
            this.txtName = new Guna.UI2.WinForms.Guna2TextBox();

            // =========================================================
            // FOOTER
            // =========================================================

            this.lblFooterHint = new System.Windows.Forms.Label();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();

            // =========================================================
            // HEADER
            // =========================================================

            this.headerUC1 = new YamyProject.HeaderUC();

            // =========================================================
            // DRAG CONTROL
            // =========================================================

            this.guna2DragControl1 =
                new Guna.UI2.WinForms.Guna2DragControl(this.components);

            // =========================================================
            // SUSPEND LAYOUT
            // =========================================================

            this.pnlMain.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.cardType.SuspendLayout();
            this.cardParent.SuspendLayout();
            this.cardCode.SuspendLayout();
            this.cardName.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();

            // =========================================================
            // pnlMain
            // =========================================================

            this.pnlMain.BackColor =
                System.Drawing.Color.FromArgb(246, 245, 250);

            this.pnlMain.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.pnlMain.Padding =
                new System.Windows.Forms.Padding(24, 20, 24, 0);

            this.pnlMain.Controls.Add(this.pnlContent);
            this.pnlMain.Controls.Add(this.pnlFooter);

            this.pnlMain.Location =
                new System.Drawing.Point(0, 45);

            this.pnlMain.Name =
                "pnlMain";

            this.pnlMain.Size =
                new System.Drawing.Size(1400, 755);

            this.pnlMain.TabIndex = 1;

            // =========================================================
            // pnlContent
            // =========================================================

            this.pnlContent.BackColor =
                System.Drawing.Color.Transparent;

            this.pnlContent.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.pnlContent.Controls.Add(this.lblPageTitle);
            this.pnlContent.Controls.Add(this.lblPageDescription);
            this.pnlContent.Controls.Add(this.cardType);
            this.pnlContent.Controls.Add(this.cardParent);
            this.pnlContent.Controls.Add(this.cardCode);
            this.pnlContent.Controls.Add(this.cardName);

            this.pnlContent.Location =
                new System.Drawing.Point(24, 20);

            this.pnlContent.Name =
                "pnlContent";

            this.pnlContent.Size =
                new System.Drawing.Size(1352, 595);

            this.pnlContent.TabIndex = 0;

            // =========================================================
            // PAGE TITLE
            // =========================================================

            this.lblPageTitle.AutoSize = true;

            this.lblPageTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    20F,
                    System.Drawing.FontStyle.Bold);

            this.lblPageTitle.ForeColor =
                System.Drawing.Color.FromArgb(43, 35, 75);

            this.lblPageTitle.Location =
                new System.Drawing.Point(4, 0);

            this.lblPageTitle.Name =
                "lblPageTitle";

            this.lblPageTitle.Text =
                "Cost Center Setup";

            this.lblPageTitle.TabIndex = 0;

            // =========================================================
            // PAGE DESCRIPTION
            // =========================================================

            this.lblPageDescription.AutoSize = true;

            this.lblPageDescription.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            this.lblPageDescription.ForeColor =
                System.Drawing.Color.FromArgb(110, 105, 125);

            this.lblPageDescription.Location =
                new System.Drawing.Point(7, 43);

            this.lblPageDescription.Name =
                "lblPageDescription";

            this.lblPageDescription.Text =
                "Create and organize your school's accounting cost centers.";

            this.lblPageDescription.TabIndex = 1;

            // =========================================================
            // CARD TYPE
            // =========================================================

            this.cardType.BackColor =
                System.Drawing.Color.White;

            this.cardType.Location =
                new System.Drawing.Point(0, 82);

            this.cardType.Name =
                "cardType";

            this.cardType.Size =
                new System.Drawing.Size(710, 66);

            this.cardType.TabIndex = 2;

            this.cardType.Controls.Add(this.lblTypeTitle);
            this.cardType.Controls.Add(this.lblTypeDescription);
            this.cardType.Controls.Add(this.radioMain);
            this.cardType.Controls.Add(this.radioSub);

            // =========================================================
            // TYPE TITLE
            // =========================================================

            this.lblTypeTitle.AutoSize = true;

            this.lblTypeTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold);

            this.lblTypeTitle.ForeColor =
                System.Drawing.Color.FromArgb(48, 43, 69);

            this.lblTypeTitle.Location =
                new System.Drawing.Point(21, 10);

            this.lblTypeTitle.Name =
                "lblTypeTitle";

            this.lblTypeTitle.Text =
                "Cost Center Type";

            // =========================================================
            // TYPE DESCRIPTION
            // =========================================================

            this.lblTypeDescription.AutoSize = true;

            this.lblTypeDescription.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8.5F);

            this.lblTypeDescription.ForeColor =
                System.Drawing.Color.FromArgb(125, 120, 140);

            this.lblTypeDescription.Location =
                new System.Drawing.Point(21, 36);

            this.lblTypeDescription.Name =
                "lblTypeDescription";

            this.lblTypeDescription.Text =
                "Choose the accounting level";

            // =========================================================
            // MAIN RADIO
            // =========================================================

            this.radioMain.AutoSize = true;

            this.radioMain.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F,
                    System.Drawing.FontStyle.Bold);

            this.radioMain.ForeColor =
                System.Drawing.Color.FromArgb(65, 55, 100);

            this.radioMain.Location =
                new System.Drawing.Point(300, 20);

            this.radioMain.Name =
                "radioMain";

            this.radioMain.Size =
                new System.Drawing.Size(70, 25);

            this.radioMain.TabIndex = 2;

            this.radioMain.Text =
                "Main";

            this.radioMain.UseVisualStyleBackColor = true;

            this.radioMain.CheckedChanged +=
                new System.EventHandler(
                    this.radioMain_CheckedChanged);

            // =========================================================
            // SUB RADIO
            // =========================================================

            this.radioSub.AutoSize = true;

            this.radioSub.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F,
                    System.Drawing.FontStyle.Bold);

            this.radioSub.ForeColor =
                System.Drawing.Color.FromArgb(65, 55, 100);

            this.radioSub.Location =
                new System.Drawing.Point(400, 20);

            this.radioSub.Name =
                "radioSub";

            this.radioSub.Size =
                new System.Drawing.Size(65, 25);

            this.radioSub.TabIndex = 3;

            this.radioSub.Text =
                "Sub";

            this.radioSub.UseVisualStyleBackColor = true;

            this.radioSub.CheckedChanged +=
                new System.EventHandler(
                    this.radioSub_CheckedChanged);

            // =========================================================
            // CARD PARENT
            // =========================================================

            this.cardParent.BackColor =
                System.Drawing.Color.White;

            this.cardParent.Location =
                new System.Drawing.Point(0, 160);

            this.cardParent.Name =
                "cardParent";

            this.cardParent.Size =
                new System.Drawing.Size(710, 82);

            this.cardParent.TabIndex = 3;

            // IMPORTANT:
            // MAIN IS THE DEFAULT.
            // Therefore Parent Cost Center is hidden initially.
            this.cardParent.Visible = false;

            this.cardParent.Controls.Add(this.lblParentTitle);
            this.cardParent.Controls.Add(this.lblParentDescription);
            this.cardParent.Controls.Add(this.lblMain);
            this.cardParent.Controls.Add(this.cmbCostCenter);
            this.cardParent.Controls.Add(this.lnkEditCategory);

            // =========================================================
            // PARENT TITLE
            // =========================================================

            this.lblParentTitle.AutoSize = true;

            this.lblParentTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold);

            this.lblParentTitle.ForeColor =
                System.Drawing.Color.FromArgb(48, 43, 69);

            this.lblParentTitle.Location =
                new System.Drawing.Point(21, 10);

            this.lblParentTitle.Name =
                "lblParentTitle";

            this.lblParentTitle.Text =
                "Parent Cost Center";

            // =========================================================
            // PARENT DESCRIPTION
            // =========================================================

            this.lblParentDescription.AutoSize = true;

            this.lblParentDescription.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8.5F);

            this.lblParentDescription.ForeColor =
                System.Drawing.Color.FromArgb(125, 120, 140);

            this.lblParentDescription.Location =
                new System.Drawing.Point(21, 36);

            this.lblParentDescription.Name =
                "lblParentDescription";

            this.lblParentDescription.Text =
                "Select the main cost center";

            // =========================================================
            // PARENT LABEL
            // =========================================================

            this.lblMain.AutoSize = true;

            this.lblMain.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblMain.ForeColor =
                System.Drawing.Color.FromArgb(75, 70, 95);

            this.lblMain.Location =
                new System.Drawing.Point(320, 11);

            this.lblMain.Name =
                "lblMain";

            this.lblMain.Text =
                "Parent Cost Center";

            // =========================================================
            // PARENT COMBOBOX
            // =========================================================

            this.cmbCostCenter.BackColor =
                System.Drawing.Color.Transparent;

            this.cmbCostCenter.BorderColor =
                System.Drawing.Color.FromArgb(220, 216, 230);

            this.cmbCostCenter.BorderRadius = 8;

            this.cmbCostCenter.DrawMode =
                System.Windows.Forms.DrawMode.OwnerDrawFixed;

            this.cmbCostCenter.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cmbCostCenter.FocusedColor =
                System.Drawing.Color.FromArgb(104, 55, 215);

            this.cmbCostCenter.FocusedState.BorderColor =
                System.Drawing.Color.FromArgb(104, 55, 215);

            this.cmbCostCenter.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F);

            this.cmbCostCenter.ForeColor =
                System.Drawing.Color.FromArgb(55, 50, 70);

            this.cmbCostCenter.ItemHeight = 30;

            this.cmbCostCenter.Location =
                new System.Drawing.Point(320, 35);

            this.cmbCostCenter.Name =
                "cmbCostCenter";

            this.cmbCostCenter.Size =
                new System.Drawing.Size(300, 36);

            this.cmbCostCenter.TabIndex = 3;

            // =========================================================
            // EDIT LINK
            // =========================================================

            this.lnkEditCategory.AutoSize = true;

            this.lnkEditCategory.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lnkEditCategory.LinkColor =
                System.Drawing.Color.FromArgb(104, 55, 215);

            this.lnkEditCategory.Location =
                new System.Drawing.Point(635, 46);

            this.lnkEditCategory.Name =
                "lnkEditCategory";

            this.lnkEditCategory.Text =
                "Edit";

            this.lnkEditCategory.TabStop = true;

            this.lnkEditCategory.LinkClicked +=
                new System.Windows.Forms.LinkLabelLinkClickedEventHandler(
                    this.lnkEditCategory_LinkClicked);

            // =========================================================
            // CARD CODE
            // =========================================================

            this.cardCode.BackColor =
                System.Drawing.Color.White;

            this.cardCode.Location =
                new System.Drawing.Point(0, 160);

            this.cardCode.Name =
                "cardCode";

            this.cardCode.Size =
                new System.Drawing.Size(220, 100);

            this.cardCode.TabIndex = 4;

            this.cardCode.Controls.Add(this.label3);
            this.cardCode.Controls.Add(this.lblCodeHint);
            this.cardCode.Controls.Add(this.txtCode);

            // =========================================================
            // CODE LABEL
            // =========================================================

            this.label3.AutoSize = true;

            this.label3.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.label3.ForeColor =
                System.Drawing.Color.FromArgb(55, 50, 75);

            this.label3.Location =
                new System.Drawing.Point(16, 13);

            this.label3.Name =
                "label3";

            this.label3.Text =
                "Code";

            // =========================================================
            // CODE HINT
            // =========================================================

            this.lblCodeHint.AutoSize = true;

            this.lblCodeHint.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8F);

            this.lblCodeHint.ForeColor =
                System.Drawing.Color.FromArgb(145, 140, 155);

            this.lblCodeHint.Location =
                new System.Drawing.Point(55, 14);

            this.lblCodeHint.Name =
                "lblCodeHint";

            this.lblCodeHint.Text =
                "Auto generated";

            // =========================================================
            // CODE TEXTBOX
            // =========================================================

            this.txtCode.BackColor =
                System.Drawing.Color.Transparent;

            this.txtCode.BorderColor =
                System.Drawing.Color.FromArgb(225, 221, 233);

            this.txtCode.BorderRadius = 8;

            this.txtCode.Cursor =
                System.Windows.Forms.Cursors.IBeam;

            this.txtCode.DefaultText = "";

            this.txtCode.DisabledState.BorderColor =
                System.Drawing.Color.FromArgb(225, 221, 233);

            this.txtCode.DisabledState.FillColor =
                System.Drawing.Color.FromArgb(243, 242, 247);

            this.txtCode.DisabledState.ForeColor =
                System.Drawing.Color.FromArgb(120, 115, 130);

            this.txtCode.Enabled = false;

            this.txtCode.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F);

            this.txtCode.ForeColor =
                System.Drawing.Color.FromArgb(55, 50, 70);

            this.txtCode.Location =
                new System.Drawing.Point(16, 40);

            this.txtCode.Name =
                "txtCode";

            this.txtCode.PlaceholderText =
                "Auto";

            this.txtCode.SelectedText = "";

            this.txtCode.Size =
                new System.Drawing.Size(188, 40);

            this.txtCode.TabIndex = 2;

            // =========================================================
            // CARD NAME
            // =========================================================

            this.cardName.BackColor =
                System.Drawing.Color.White;

            this.cardName.Location =
                new System.Drawing.Point(235, 160);

            this.cardName.Name =
                "cardName";

            this.cardName.Size =
                new System.Drawing.Size(747, 100);

            this.cardName.TabIndex = 5;

            this.cardName.Controls.Add(this.label2);
            this.cardName.Controls.Add(this.lblNameHint);
            this.cardName.Controls.Add(this.txtName);

            // =========================================================
            // NAME LABEL
            // =========================================================

            this.label2.AutoSize = true;

            this.label2.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.label2.ForeColor =
                System.Drawing.Color.FromArgb(55, 50, 75);

            this.label2.Location =
                new System.Drawing.Point(16, 13);

            this.label2.Name =
                "label2";

            this.label2.Text =
                "Name";

            // =========================================================
            // NAME HINT
            // =========================================================

            this.lblNameHint.AutoSize = true;

            this.lblNameHint.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8F);

            this.lblNameHint.ForeColor =
                System.Drawing.Color.FromArgb(145, 140, 155);

            this.lblNameHint.Location =
                new System.Drawing.Point(60, 14);

            this.lblNameHint.Name =
                "lblNameHint";

            this.lblNameHint.Text =
                "Enter a clear accounting name";

            // =========================================================
            // NAME TEXTBOX
            // =========================================================

            this.txtName.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                ((System.Windows.Forms.AnchorStyles.Top |
                  System.Windows.Forms.AnchorStyles.Left |
                  System.Windows.Forms.AnchorStyles.Right)));

            this.txtName.BackColor =
                System.Drawing.Color.Transparent;

            this.txtName.BorderColor =
                System.Drawing.Color.FromArgb(220, 216, 230);

            this.txtName.BorderRadius = 8;

            this.txtName.Cursor =
                System.Windows.Forms.Cursors.IBeam;

            this.txtName.DefaultText = "";

            this.txtName.FocusedState.BorderColor =
                System.Drawing.Color.FromArgb(104, 55, 215);

            this.txtName.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F);

            this.txtName.ForeColor =
                System.Drawing.Color.FromArgb(45, 43, 58);

            this.txtName.Location =
                new System.Drawing.Point(16, 40);

            this.txtName.Name =
                "txtName";

            this.txtName.PlaceholderForeColor =
                System.Drawing.Color.FromArgb(165, 160, 175);

            this.txtName.PlaceholderText =
                "e.g. Administration, Transport, Library...";

            this.txtName.SelectedText = "";

            this.txtName.Size =
                new System.Drawing.Size(715, 40);

            this.txtName.TabIndex = 2;

            // =========================================================
            // FOOTER
            // =========================================================

            this.pnlFooter.BackColor =
                System.Drawing.Color.White;

            this.pnlFooter.Dock =
                System.Windows.Forms.DockStyle.Bottom;

            this.pnlFooter.Height = 82;

            this.pnlFooter.Controls.Add(this.lblFooterHint);
            this.pnlFooter.Controls.Add(this.btnSave);
            this.pnlFooter.Controls.Add(this.btnClose);

            this.pnlFooter.Location =
                new System.Drawing.Point(24, 615);

            this.pnlFooter.Name =
                "pnlFooter";

            this.pnlFooter.Size =
                new System.Drawing.Size(1352, 82);

            this.pnlFooter.TabIndex = 1;

            // =========================================================
            // FOOTER HINT
            // =========================================================

            this.lblFooterHint.AutoSize = true;

            this.lblFooterHint.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    8.5F);

            this.lblFooterHint.ForeColor =
                System.Drawing.Color.FromArgb(135, 130, 145);

            this.lblFooterHint.Location =
                new System.Drawing.Point(20, 31);

            this.lblFooterHint.Name =
                "lblFooterHint";

            this.lblFooterHint.Text =
                "Make sure the cost center name is unique.";

            // =========================================================
            // SAVE BUTTON
            // =========================================================

            this.btnSave.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                ((System.Windows.Forms.AnchorStyles.Top |
                  System.Windows.Forms.AnchorStyles.Right)));

            this.btnSave.BorderRadius = 9;

            this.btnSave.FillColor =
                System.Drawing.Color.FromArgb(104, 55, 215);

            this.btnSave.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F,
                    System.Drawing.FontStyle.Bold);

            this.btnSave.ForeColor =
                System.Drawing.Color.White;

            this.btnSave.HoverState.FillColor =
                System.Drawing.Color.FromArgb(118, 67, 229);

            // FIXED POSITION
            this.btnSave.Location =
                new System.Drawing.Point(1007, 18);

            this.btnSave.Name =
                "btnSave";

            this.btnSave.Size =
                new System.Drawing.Size(170, 46);

            this.btnSave.TabIndex = 1;

            this.btnSave.Text =
                "Save Cost Center";

            this.btnSave.Click +=
                new System.EventHandler(
                    this.btnSave_Click);

            // =========================================================
            // CLOSE BUTTON
            // =========================================================

            this.btnClose.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                ((System.Windows.Forms.AnchorStyles.Top |
                  System.Windows.Forms.AnchorStyles.Right)));

            this.btnClose.BorderColor =
                System.Drawing.Color.FromArgb(215, 210, 225);

            this.btnClose.BorderRadius = 9;

            this.btnClose.BorderThickness = 1;

            this.btnClose.FillColor =
                System.Drawing.Color.White;

            this.btnClose.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9.5F,
                    System.Drawing.FontStyle.Bold);

            this.btnClose.ForeColor =
                System.Drawing.Color.FromArgb(75, 68, 92);

            this.btnClose.HoverState.FillColor =
                System.Drawing.Color.FromArgb(245, 243, 249);

            // FIXED POSITION
            this.btnClose.Location =
                new System.Drawing.Point(1192, 18);

            this.btnClose.Name =
                "btnClose";

            this.btnClose.Size =
                new System.Drawing.Size(140, 46);

            this.btnClose.TabIndex = 2;

            this.btnClose.Text =
                "Close";

            this.btnClose.Click +=
                new System.EventHandler(
                    this.btnClose_Click);

            // =========================================================
            // HEADER
            // =========================================================

            this.headerUC1.BackColor =
                System.Drawing.Color.FromArgb(43, 27, 103);

            this.headerUC1.Dock =
                System.Windows.Forms.DockStyle.Top;

            this.headerUC1.FormText =
                "";

            this.headerUC1.HeaderText =
                "";

            this.headerUC1.Location =
                new System.Drawing.Point(0, 0);

            this.headerUC1.Margin =
                new System.Windows.Forms.Padding(0);

            this.headerUC1.Name =
                "headerUC1";

            this.headerUC1.Size =
                new System.Drawing.Size(1400, 45);

            this.headerUC1.TabIndex = 0;

            // =========================================================
            // DRAG CONTROL
            // =========================================================

            this.guna2DragControl1.DockIndicatorTransparencyValue =
                0.6D;

            this.guna2DragControl1.UseTransparentDrag =
                true;

            // =========================================================
            // FORM
            // =========================================================

            this.AutoScaleDimensions =
                new System.Drawing.SizeF(8F, 16F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.BackColor =
                System.Drawing.Color.FromArgb(246, 245, 250);

            this.ClientSize =
                new System.Drawing.Size(1400, 800);

            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.headerUC1);

            this.FormBorderStyle =
                System.Windows.Forms.FormBorderStyle.None;

            this.Name =
                "frmCostCenter";

            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Text =
                "Cost Center";

            this.Load +=
                new System.EventHandler(
                    this.frmCostCenter_Load);

            // =========================================================
            // RESUME LAYOUT
            // =========================================================

            this.pnlMain.ResumeLayout(false);

            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();

            this.cardType.ResumeLayout(false);
            this.cardType.PerformLayout();

            this.cardParent.ResumeLayout(false);
            this.cardParent.PerformLayout();

            this.cardCode.ResumeLayout(false);
            this.cardCode.PerformLayout();

            this.cardName.ResumeLayout(false);
            this.cardName.PerformLayout();

            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();

            this.ResumeLayout(false);
        }

        #endregion

        // =============================================================
        // CONTROLS
        // =============================================================

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlFooter;

        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Label lblPageDescription;

        private System.Windows.Forms.Panel cardType;
        private System.Windows.Forms.Label lblTypeTitle;
        private System.Windows.Forms.Label lblTypeDescription;

        private System.Windows.Forms.RadioButton radioMain;
        private System.Windows.Forms.RadioButton radioSub;

        private System.Windows.Forms.Panel cardParent;
        private System.Windows.Forms.Label lblParentTitle;
        private System.Windows.Forms.Label lblParentDescription;
        private System.Windows.Forms.Label lblMain;

        public Guna.UI2.WinForms.Guna2ComboBox cmbCostCenter;

        private System.Windows.Forms.LinkLabel lnkEditCategory;

        private System.Windows.Forms.Panel cardCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCodeHint;

        private Guna.UI2.WinForms.Guna2TextBox txtCode;

        private System.Windows.Forms.Panel cardName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblNameHint;

        private Guna.UI2.WinForms.Guna2TextBox txtName;

        private System.Windows.Forms.Label lblFooterHint;

        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2Button btnClose;

        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;

        private HeaderUC headerUC1;
    }
}