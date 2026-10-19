namespace YamyProject
{
    partial class frmWarehouseLocation
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDiscard;
        private System.Windows.Forms.Button btnPutawayRules;

        private System.Windows.Forms.Label lblTitle;

        private System.Windows.Forms.Label lblLocationName;
        private System.Windows.Forms.TextBox txtLocationName;

        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;

        private System.Windows.Forms.Label lblWarehouse;
        private System.Windows.Forms.ComboBox cmbWarehouse;

        private System.Windows.Forms.Label lblParentLocation;
        private System.Windows.Forms.ComboBox cmbParentLocation;

        private System.Windows.Forms.Label lblAdditional;

        private System.Windows.Forms.Label lblLocationType;
        private System.Windows.Forms.ComboBox cmbLocationType;

        private System.Windows.Forms.CheckBox chkScrap;
        private System.Windows.Forms.CheckBox chkReturn;
        private System.Windows.Forms.CheckBox chkReplenish;
        private System.Windows.Forms.CheckBox chkActive;

        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.TextBox txtExternalNote;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.btnPutawayRules = new System.Windows.Forms.Button();

            this.lblTitle = new System.Windows.Forms.Label();

            this.lblLocationName = new System.Windows.Forms.Label();
            this.txtLocationName = new System.Windows.Forms.TextBox();

            this.lblCode = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();

            this.lblWarehouse = new System.Windows.Forms.Label();
            this.cmbWarehouse = new System.Windows.Forms.ComboBox();

            this.lblParentLocation = new System.Windows.Forms.Label();
            this.cmbParentLocation = new System.Windows.Forms.ComboBox();

            this.lblAdditional = new System.Windows.Forms.Label();

            this.lblLocationType = new System.Windows.Forms.Label();
            this.cmbLocationType = new System.Windows.Forms.ComboBox();

            this.chkScrap = new System.Windows.Forms.CheckBox();
            this.chkReturn = new System.Windows.Forms.CheckBox();
            this.chkReplenish = new System.Windows.Forms.CheckBox();
            this.chkActive = new System.Windows.Forms.CheckBox();

            this.lblNote = new System.Windows.Forms.Label();
            this.txtExternalNote = new System.Windows.Forms.TextBox();

            this.pnlTop.SuspendLayout();
            this.SuspendLayout();

            // =========================================================
            // TOP PANEL
            // =========================================================

            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 66;
            this.pnlTop.BackColor = System.Drawing.Color.WhiteSmoke;

            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(14, 15);
            this.btnSave.Size = new System.Drawing.Size(60, 36);
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(112, 88, 166);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnDiscard.Text = "Discard";
            this.btnDiscard.Location = new System.Drawing.Point(84, 15);
            this.btnDiscard.Size = new System.Drawing.Size(80, 36);
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);

            // Putaway button is intentionally placed at X=760.
            // It remains visible even if the form is around 1000 px wide.
            this.btnPutawayRules.Text = "Putaway Rules";
            this.btnPutawayRules.Location = new System.Drawing.Point(760, 15);
            this.btnPutawayRules.Size = new System.Drawing.Size(170, 36);
            this.btnPutawayRules.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;
            this.btnPutawayRules.BackColor = System.Drawing.Color.White;
            this.btnPutawayRules.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPutawayRules.Click +=
                new System.EventHandler(this.btnPutawayRules_Click);

            this.pnlTop.Controls.Add(this.btnSave);
            this.pnlTop.Controls.Add(this.btnDiscard);
            this.pnlTop.Controls.Add(this.btnPutawayRules);

            // =========================================================
            // TITLE
            // =========================================================

            this.lblTitle.Text = "Locations / New";
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTitle.Location = new System.Drawing.Point(42, 96);

            // =========================================================
            // LOCATION NAME
            // =========================================================

            this.lblLocationName.Text = "Location Name";
            this.lblLocationName.AutoSize = true;
            this.lblLocationName.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);
            this.lblLocationName.Location =
                new System.Drawing.Point(42, 145);

            this.txtLocationName.Location =
                new System.Drawing.Point(42, 170);
            this.txtLocationName.Size =
                new System.Drawing.Size(330, 32);
            this.txtLocationName.Font =
                new System.Drawing.Font("Segoe UI", 14F);

            // =========================================================
            // CODE
            // =========================================================

            this.lblCode.Text = "Code";
            this.lblCode.AutoSize = true;
            this.lblCode.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);
            this.lblCode.Location =
                new System.Drawing.Point(410, 145);

            this.txtCode.Location =
                new System.Drawing.Point(410, 172);
            this.txtCode.Size =
                new System.Drawing.Size(190, 25);

            // =========================================================
            // WAREHOUSE
            // =========================================================

            this.lblWarehouse.Text = "Warehouse";
            this.lblWarehouse.AutoSize = true;
            this.lblWarehouse.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);
            this.lblWarehouse.Location =
                new System.Drawing.Point(42, 230);

            this.cmbWarehouse.Location =
                new System.Drawing.Point(205, 225);
            this.cmbWarehouse.Size =
                new System.Drawing.Size(300, 25);
            this.cmbWarehouse.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWarehouse.SelectedIndexChanged +=
                new System.EventHandler(
                    this.cmbWarehouse_SelectedIndexChanged);

            // =========================================================
            // PARENT LOCATION
            // =========================================================

            this.lblParentLocation.Text = "Parent Location";
            this.lblParentLocation.AutoSize = true;
            this.lblParentLocation.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);
            this.lblParentLocation.Location =
                new System.Drawing.Point(42, 270);

            this.cmbParentLocation.Location =
                new System.Drawing.Point(205, 265);
            this.cmbParentLocation.Size =
                new System.Drawing.Size(300, 25);
            this.cmbParentLocation.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;

            // =========================================================
            // ADDITIONAL INFORMATION
            // =========================================================

            this.lblAdditional.Text =
                "ADDITIONAL INFORMATION";
            this.lblAdditional.AutoSize = true;
            this.lblAdditional.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold);
            this.lblAdditional.Location =
                new System.Drawing.Point(42, 335);

            this.lblLocationType.Text =
                "Location Type";
            this.lblLocationType.AutoSize = true;
            this.lblLocationType.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);
            this.lblLocationType.Location =
                new System.Drawing.Point(42, 385);

            this.cmbLocationType.Location =
                new System.Drawing.Point(205, 380);
            this.cmbLocationType.Size =
                new System.Drawing.Size(300, 25);
            this.cmbLocationType.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;

            // =========================================================
            // CHECKBOXES
            // =========================================================

            this.chkScrap.Text =
                "Is a Scrap Location?";
            this.chkScrap.AutoSize = true;
            this.chkScrap.Location =
                new System.Drawing.Point(42, 435);

            this.chkReturn.Text =
                "Is a Return Location?";
            this.chkReturn.AutoSize = true;
            this.chkReturn.Location =
                new System.Drawing.Point(42, 470);

            this.chkReplenish.Text =
                "Replenish Location";
            this.chkReplenish.AutoSize = true;
            this.chkReplenish.Location =
                new System.Drawing.Point(42, 505);

            this.chkActive.Text =
                "Active";
            this.chkActive.AutoSize = true;
            this.chkActive.Location =
                new System.Drawing.Point(42, 540);

            // =========================================================
            // NOTE
            // =========================================================

            this.lblNote.Text = "External Note";
            this.lblNote.AutoSize = true;
            this.lblNote.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);
            this.lblNote.Location =
                new System.Drawing.Point(42, 585);

            this.txtExternalNote.Location =
                new System.Drawing.Point(42, 610);
            this.txtExternalNote.Size =
                new System.Drawing.Size(640, 85);
            this.txtExternalNote.Multiline = true;
            this.txtExternalNote.ScrollBars =
                System.Windows.Forms.ScrollBars.Vertical;

            // =========================================================
            // FORM
            // =========================================================

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize =
                new System.Drawing.Size(1000, 720);
            this.MinimumSize =
                new System.Drawing.Size(900, 650);
            this.BackColor =
                System.Drawing.Color.White;

            this.Controls.Add(this.txtExternalNote);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.chkReplenish);
            this.Controls.Add(this.chkReturn);
            this.Controls.Add(this.chkScrap);
            this.Controls.Add(this.cmbLocationType);
            this.Controls.Add(this.lblLocationType);
            this.Controls.Add(this.lblAdditional);
            this.Controls.Add(this.cmbParentLocation);
            this.Controls.Add(this.lblParentLocation);
            this.Controls.Add(this.cmbWarehouse);
            this.Controls.Add(this.lblWarehouse);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.txtLocationName);
            this.Controls.Add(this.lblLocationName);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlTop);

            this.Font =
                new System.Drawing.Font("Segoe UI", 9F);
            this.Name =
                "frmWarehouseLocation";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text =
                "Location";
            this.Load +=
                new System.EventHandler(
                    this.frmWarehouseLocation_Load);

            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
