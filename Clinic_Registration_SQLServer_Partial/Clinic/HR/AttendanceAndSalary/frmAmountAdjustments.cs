using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmAmountAdjustments : Form
    {
        public decimal TotalAmount { get; private set; } = 0;
        public string AdjustmentType { get; set; }
        private string empName, empCode,refId="0";
        private DataTable dtAdjustments;

        public frmAmountAdjustments(string name, string code, string type,string _refId="0")
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            string language =
     Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
            lblDate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");
            lblName.Text = name;
            lblCode.Text = code;
            empName = name;
            empCode = code;
            refId = _refId;
            AdjustmentType = type;
        }
        public void ApplyLanguageLayout(bool isArabic)
        {
            SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel6.SuspendLayout();

            try
            {
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                int margin = 16;

                if (isArabic)
                {
                    // =========================
                    // TEXT
                    // =========================
                    Text = "تعديل المبلغ";
                    headerUC1.FormText = "تعديل المبلغ";

                    label1.Text = "التاريخ :";
                    label3.Text = "الاسم :";
                    label4.Text = "الرمز :";
                    label2.Text = "الإجمالي";

                    btnSave.Text = "حفظ";
                    btnClose.Text = "إغلاق";


                    // =========================
                    // INFORMATION -> RIGHT
                    // =========================
                    panel2.RightToLeft = RightToLeft.No;

                    label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                    lblDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    lblName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    lblCode.Anchor = AnchorStyles.Top | AnchorStyles.Right;


                    // DATE
                    label1.Location = new Point(
                        panel2.ClientSize.Width
                        - margin
                        - label1.Width,
                        14);

                    lblDate.Location = new Point(
                        label1.Left
                        - lblDate.Width
                        - 8,
                        14);


                    // NAME
                    label3.Location = new Point(
                        panel2.ClientSize.Width
                        - margin
                        - label3.Width,
                        39);

                    lblName.Location = new Point(
                        label3.Left
                        - lblName.Width
                        - 8,
                        39);


                    // CODE
                    label4.Location = new Point(
                        panel2.ClientSize.Width
                        - margin
                        - label4.Width,
                        65);

                    lblCode.Location = new Point(
                        label4.Left
                        - lblCode.Width
                        - 8,
                        65);


                    label1.TextAlign = ContentAlignment.MiddleRight;
                    label3.TextAlign = ContentAlignment.MiddleRight;
                    label4.TextAlign = ContentAlignment.MiddleRight;


                    // =========================
                    // GRID RTL
                    // =========================
                    dgvAdjustments.RightToLeft = RightToLeft.Yes;

                    dgvAdjustments.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    dgvAdjustments.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;


                    if (dgvAdjustments.Columns.Contains("Description"))
                        dgvAdjustments.Columns["Description"].HeaderText =
                            "الوصف";

                    if (dgvAdjustments.Columns.Contains("Amount"))
                        dgvAdjustments.Columns["Amount"].HeaderText =
                            "المبلغ";


                    // =========================
                    // TOTAL -> RIGHT
                    // =========================
                    label2.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    lblTotal.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    label2.Location = new Point(
                        panel6.ClientSize.Width
                        - margin
                        - label2.Width,
                        10);

                    lblTotal.Location = new Point(
                        label2.Left
                        - lblTotal.Width
                        - 15,
                        10);


                    // =========================
                    // BUTTONS -> LEFT
                    // =========================
                    btnClose.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    btnSave.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    btnClose.Location =
                        new Point(margin, 4);

                    btnSave.Location =
                        new Point(
                            btnClose.Right + 6,
                            4);
                }
                else
                {
                    // =========================
                    // ENGLISH ORIGINAL
                    // =========================
                    Text = "Edit Amount";
                    headerUC1.FormText = "Edit Amount";

                    label1.Text = "Date :";
                    label3.Text = "Name :";
                    label4.Text = "Code :";
                    label2.Text = "Total";

                    btnSave.Text = "Save";
                    btnClose.Text = "Close";


                    // ORIGINAL INFORMATION
                    label1.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;
                    lblDate.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    label3.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;
                    lblName.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    label4.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;
                    lblCode.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    label1.Location = new Point(16, 14);
                    lblDate.Location = new Point(62, 14);

                    label3.Location = new Point(11, 39);
                    lblName.Location = new Point(62, 39);

                    label4.Location = new Point(14, 65);
                    lblCode.Location = new Point(61, 65);


                    // GRID LTR
                    dgvAdjustments.RightToLeft =
                        RightToLeft.No;

                    dgvAdjustments.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;

                    dgvAdjustments.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    if (dgvAdjustments.Columns.Contains("Description"))
                        dgvAdjustments.Columns["Description"].HeaderText =
                            "Description";

                    if (dgvAdjustments.Columns.Contains("Amount"))
                        dgvAdjustments.Columns["Amount"].HeaderText =
                            "Amount";


                    // TOTAL ORIGINAL
                    label2.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    lblTotal.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    label2.Location =
                        new Point(84, 10);

                    lblTotal.Location =
                        new Point(281, 10);


                    // ORIGINAL BUTTONS
                    btnSave.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    btnClose.Anchor =
                        AnchorStyles.Top | AnchorStyles.Right;

                    btnSave.Location =
                        new Point(
                            panel1.ClientSize.Width - 211,
                            4);

                    btnClose.Location =
                        new Point(
                            panel1.ClientSize.Width - 105,
                            4);
                }

                btnSave.BringToFront();
                btnClose.BringToFront();
            }
            finally
            {
                panel6.ResumeLayout(false);
                panel2.ResumeLayout(false);
                panel1.ResumeLayout(false);
                ResumeLayout(false);

                panel2.PerformLayout();
                panel6.PerformLayout();
                panel1.PerformLayout();

                Invalidate(true);
            }
        }
        private void frmAmountAdjustments_Load(object sender, EventArgs e)
        {
            dtAdjustments = new DataTable();
            //AppTheme.DefaultFont(dgvAdjustments);
            // 💥 Ensure these exact names match everywhere
            dtAdjustments.Columns.Add("Description", typeof(string));
            dtAdjustments.Columns.Add("Amount", typeof(decimal));

            dgvAdjustments.DataSource = dtAdjustments;

            dgvAdjustments.AllowUserToAddRows = true;
            dgvAdjustments.AllowUserToDeleteRows = true;
            dgvAdjustments.Columns["Amount"].DefaultCellStyle.Format = "N3";
            dgvAdjustments.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvAdjustments.CellEndEdit += dgvAdjustments_CellEndEdit;

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvAdjustments);
            UpdateTotalLabel();
            string language =
    Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
        }

        private void UpdateTotalLabel()
        {
            if (dtAdjustments == null) return;

            decimal total = 0;

            foreach (DataRow row in dtAdjustments.Rows)
            {
                string desc = row["Description"]?.ToString();
                if (string.IsNullOrWhiteSpace(desc))
                    continue;

                decimal value = 0;
                decimal.TryParse(row["Amount"]?.ToString(), out value);
                total += value;
            }

            lblTotal.Text = $"Total: {total.ToString()}";
        }

        private void dgvAdjustments_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateTotalLabel();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            decimal total = 0;

            if (dtAdjustments == null || !dtAdjustments.Columns.Contains("Description") || !dtAdjustments.Columns.Contains("Amount"))
            {
                MessageBox.Show("Columns not found in DataTable.");
                return;
            }

            foreach (DataRow row in dtAdjustments.Rows)
            {
                if (!dtAdjustments.Columns.Contains("Description") || !dtAdjustments.Columns.Contains("Amount"))
                {
                    MessageBox.Show("Missing expected columns in the row.");
                    continue;
                }

                string desc = row["Description"]?.ToString();
                if (string.IsNullOrWhiteSpace(desc))
                    continue;

                decimal amount = 0;
                decimal.TryParse(row["Amount"]?.ToString(), out amount);
                total += amount;

                if (amount > 0)
                {
                    ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_salary_adjustments 
                        (code, name, adjustment_type, description, amount,date,ref_id)
                        VALUES (@code, @name, @adjustment_type, @description, @amount,@date,@refId)",
                        ClinicDatabase.CreateParameterNew("@code", lblCode.Text),
                        ClinicDatabase.CreateParameterNew("@name", lblName.Text),
                        ClinicDatabase.CreateParameterNew("@adjustment_type", AdjustmentType),
                        ClinicDatabase.CreateParameterNew("@description", desc),
                        ClinicDatabase.CreateParameterNew("@amount", amount),
                        ClinicDatabase.CreateParameterNew("@refId", refId),
                        ClinicDatabase.CreateParameterNew("@date", lblDate.Text)
                    );
                    ////Utilities.LogAudit(-1, "Add Salary Adjustment", "Salary Adjustment", int.Parse(refId), 
                        //$"Added Salary Adjustment for {lblName.Text} ({lblCode.Text}): {desc} - Amount: {amount}");
                }
            }

            TotalAmount = total;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dgvAdjustments_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateTotalLabel();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
