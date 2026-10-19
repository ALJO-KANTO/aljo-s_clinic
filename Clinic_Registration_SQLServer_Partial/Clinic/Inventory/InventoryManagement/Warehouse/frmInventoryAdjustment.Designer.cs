namespace YamyProject
{
    partial class frmInventoryAdjustment
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlTop;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;

        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClose;

        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblLocationValue;

        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblProductValue;

        private System.Windows.Forms.Label lblOnHand;
        private System.Windows.Forms.Label lblOnHandValue;

        private System.Windows.Forms.Label lblCounted;

        private System.Windows.Forms.NumericUpDown numCounted;

        private System.Windows.Forms.Label lblDifference;
        private System.Windows.Forms.Label lblDifferenceValue;

        private System.Windows.Forms.Label lblUnit;

        protected override void Dispose(
            bool disposing)
        {
            if (disposing &&
                components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop =
                new System.Windows.Forms.Panel();

            this.lblTitle =
                new System.Windows.Forms.Label();

            this.lblSubTitle =
                new System.Windows.Forms.Label();

            this.btnHistory =
                new System.Windows.Forms.Button();

            this.btnApply =
                new System.Windows.Forms.Button();

            this.btnClear =
                new System.Windows.Forms.Button();

            this.btnClose =
                new System.Windows.Forms.Button();

            this.lblLocation =
                new System.Windows.Forms.Label();

            this.lblLocationValue =
                new System.Windows.Forms.Label();

            this.lblProduct =
                new System.Windows.Forms.Label();

            this.lblProductValue =
                new System.Windows.Forms.Label();

            this.lblOnHand =
                new System.Windows.Forms.Label();

            this.lblOnHandValue =
                new System.Windows.Forms.Label();

            this.lblCounted =
                new System.Windows.Forms.Label();

            this.numCounted =
                new System.Windows.Forms.NumericUpDown();

            this.lblDifference =
                new System.Windows.Forms.Label();

            this.lblDifferenceValue =
                new System.Windows.Forms.Label();

            this.lblUnit =
                new System.Windows.Forms.Label();


            ((System.ComponentModel.ISupportInitialize)
                (this.numCounted))
                .BeginInit();

            this.pnlTop.SuspendLayout();

            this.SuspendLayout();


            // ==============================
            // TOP PANEL
            // ==============================

            this.pnlTop.BackColor =
                System.Drawing.Color.WhiteSmoke;

            this.pnlTop.Dock =
                System.Windows.Forms.DockStyle.Top;

            this.pnlTop.Location =
                new System.Drawing.Point(
                    0,
                    0);

            this.pnlTop.Name =
                "pnlTop";

            this.pnlTop.Size =
                new System.Drawing.Size(
                    1080,
                    66);


            // ==============================
            // TITLE
            // ==============================

            this.lblTitle.AutoSize =
                true;

            this.lblTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            this.lblTitle.Location =
                new System.Drawing.Point(
                    15,
                    8);

            this.lblTitle.Text =
                "Locations / Inventory Adjustments";


            this.lblSubTitle.AutoSize =
                true;

            this.lblSubTitle.Location =
                new System.Drawing.Point(
                    15,
                    36);

            this.lblSubTitle.Text =
                "Inventory Adjustments";


            // ==============================
            // HISTORY
            // ==============================

            this.btnHistory.Anchor =
                System.Windows.Forms
                .AnchorStyles.Top |
                System.Windows.Forms
                .AnchorStyles.Right;

            this.btnHistory.Location =
                new System.Drawing.Point(
                    715,
                    16);

            this.btnHistory.Size =
                new System.Drawing.Size(
                    80,
                    32);

            this.btnHistory.Text =
                "History";

            this.btnHistory.Click +=
                new System.EventHandler(
                    this.btnHistory_Click);


            // ==============================
            // APPLY
            // ==============================

            this.btnApply.Anchor =
                System.Windows.Forms
                .AnchorStyles.Top |
                System.Windows.Forms
                .AnchorStyles.Right;

            this.btnApply.Location =
                new System.Drawing.Point(
                    803,
                    16);

            this.btnApply.Size =
                new System.Drawing.Size(
                    80,
                    32);

            this.btnApply.Text =
                "Apply";

            this.btnApply.BackColor =
                System.Drawing.Color.FromArgb(
                    112,
                    88,
                    166);

            this.btnApply.ForeColor =
                System.Drawing.Color.White;

            this.btnApply.FlatStyle =
                System.Windows.Forms
                .FlatStyle.Flat;

            this.btnApply.Click +=
                new System.EventHandler(
                    this.btnApply_Click);


            // ==============================
            // CLEAR
            // ==============================

            this.btnClear.Anchor =
                System.Windows.Forms
                .AnchorStyles.Top |
                System.Windows.Forms
                .AnchorStyles.Right;

            this.btnClear.Location =
                new System.Drawing.Point(
                    891,
                    16);

            this.btnClear.Size =
                new System.Drawing.Size(
                    80,
                    32);

            this.btnClear.Text =
                "Clear";

            this.btnClear.Click +=
                new System.EventHandler(
                    this.btnClear_Click);


            // ==============================
            // CLOSE
            // ==============================

            this.btnClose.Anchor =
                System.Windows.Forms
                .AnchorStyles.Top |
                System.Windows.Forms
                .AnchorStyles.Right;

            this.btnClose.Location =
                new System.Drawing.Point(
                    979,
                    16);

            this.btnClose.Size =
                new System.Drawing.Size(
                    80,
                    32);

            this.btnClose.Text =
                "Close";

            this.btnClose.Click +=
                new System.EventHandler(
                    this.btnClose_Click);


            this.pnlTop.Controls.Add(
                this.lblTitle);

            this.pnlTop.Controls.Add(
                this.lblSubTitle);

            this.pnlTop.Controls.Add(
                this.btnHistory);

            this.pnlTop.Controls.Add(
                this.btnApply);

            this.pnlTop.Controls.Add(
                this.btnClear);

            this.pnlTop.Controls.Add(
                this.btnClose);


            // ==============================
            // LOCATION
            // ==============================

            this.lblLocation.AutoSize =
                true;

            this.lblLocation.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblLocation.Location =
                new System.Drawing.Point(
                    35,
                    110);

            this.lblLocation.Text =
                "Location";


            this.lblLocationValue.AutoSize =
                true;

            this.lblLocationValue.Location =
                new System.Drawing.Point(
                    190,
                    110);

            this.lblLocationValue.Text =
                "-";


            // ==============================
            // PRODUCT
            // ==============================

            this.lblProduct.AutoSize =
                true;

            this.lblProduct.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblProduct.Location =
                new System.Drawing.Point(
                    35,
                    155);

            this.lblProduct.Text =
                "Product";


            this.lblProductValue.AutoSize =
                true;

            this.lblProductValue.Location =
                new System.Drawing.Point(
                    190,
                    155);

            this.lblProductValue.Text =
                "-";


            // ==============================
            // ON HAND
            // ==============================

            this.lblOnHand.AutoSize =
                true;

            this.lblOnHand.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblOnHand.Location =
                new System.Drawing.Point(
                    35,
                    210);

            this.lblOnHand.Text =
                "On Hand Quantity";


            this.lblOnHandValue.AutoSize =
                true;

            this.lblOnHandValue.Location =
                new System.Drawing.Point(
                    190,
                    210);

            this.lblOnHandValue.Text =
                "0";


            // ==============================
            // COUNTED QUANTITY
            // ==============================

            this.lblCounted.AutoSize =
                true;

            this.lblCounted.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblCounted.Location =
                new System.Drawing.Point(
                    35,
                    255);

            this.lblCounted.Text =
                "Counted Quantity";


            this.numCounted.Location =
                new System.Drawing.Point(
                    190,
                    248);

            this.numCounted.Size =
                new System.Drawing.Size(
                    160,
                    23);

            this.numCounted.DecimalPlaces =
                4;

            this.numCounted.Maximum =
                new decimal(
                    new int[]
                    {
                        999999999,
                        0,
                        0,
                        0
                    });

            this.numCounted.ThousandsSeparator =
                true;

            this.numCounted.ValueChanged +=
                new System.EventHandler(
                    this.numCounted_ValueChanged);


            this.lblUnit.AutoSize =
                true;

            this.lblUnit.Location =
                new System.Drawing.Point(
                    365,
                    255);

            this.lblUnit.Text =
                "Units";


            // ==============================
            // DIFFERENCE
            // ==============================

            this.lblDifference.AutoSize =
                true;

            this.lblDifference.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblDifference.Location =
                new System.Drawing.Point(
                    35,
                    305);

            this.lblDifference.Text =
                "Difference";


            this.lblDifferenceValue.AutoSize =
                true;

            this.lblDifferenceValue.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold);

            this.lblDifferenceValue.Location =
                new System.Drawing.Point(
                    190,
                    302);

            this.lblDifferenceValue.Text =
                "0";


            // ==============================
            // FORM
            // ==============================

            this.AutoScaleMode =
                System.Windows.Forms
                .AutoScaleMode.Font;

            this.BackColor =
                System.Drawing.Color.White;

            this.ClientSize =
                new System.Drawing.Size(
                    1080,
                    430);

            this.Controls.Add(
                this.lblLocation);

            this.Controls.Add(
                this.lblLocationValue);

            this.Controls.Add(
                this.lblProduct);

            this.Controls.Add(
                this.lblProductValue);

            this.Controls.Add(
                this.lblOnHand);

            this.Controls.Add(
                this.lblOnHandValue);

            this.Controls.Add(
                this.lblCounted);

            this.Controls.Add(
                this.numCounted);

            this.Controls.Add(
                this.lblUnit);

            this.Controls.Add(
                this.lblDifference);

            this.Controls.Add(
                this.lblDifferenceValue);

            this.Controls.Add(
                this.pnlTop);

            this.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F);

            this.Name =
                "frmInventoryAdjustment";

            this.StartPosition =
                System.Windows.Forms
                .FormStartPosition
                .CenterParent;

            this.Text =
                "Inventory Adjustments";

            this.Load +=
                new System.EventHandler(
                    this.frmInventoryAdjustment_Load);


            ((System.ComponentModel.ISupportInitialize)
                (this.numCounted))
                .EndInit();

            this.pnlTop.ResumeLayout(
                false);

            this.pnlTop.PerformLayout();

            this.ResumeLayout(
                false);

            this.PerformLayout();
        }
    }
}