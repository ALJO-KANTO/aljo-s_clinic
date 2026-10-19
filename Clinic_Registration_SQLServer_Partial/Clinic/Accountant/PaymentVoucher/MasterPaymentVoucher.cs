using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterPaymentVoucher : Form
    {
        private EventHandler PaymentVoucherUpdatedHandler;
        private static readonly Dictionary<int, string> PaymentTypeEnglishNames =
     new Dictionary<int, string>
     {
        { 1, "Vendor" },
        { 2, "Employee" },
        { 3, "Petty Cash" }
     };

        private static readonly Dictionary<int, string> PaymentTypeArabicNames =
            new Dictionary<int, string>
            {
        { 1, "المورد" },
        { 2, "الموظف" },
        { 3, "النقدية النثرية" }
            };
        private void LoadPaymentTypeCombo()
        {
            cmbType.SelectedIndexChanged -=
                cmbType_SelectedIndexChanged;

            try
            {
                bool arabic = IsArabicCulture();

                // Completely remove anything placed in the ComboBox
                // by the Designer or previous language.
                cmbType.DataSource = null;
                cmbType.Items.Clear();
                cmbType.DisplayMember = "";
                cmbType.ValueMember = "";

                DataTable dt = new DataTable();

                dt.Columns.Add("id", typeof(int));
                dt.Columns.Add("name", typeof(string));

                if (arabic)
                {
                    dt.Rows.Add(1, "المورد");
                    dt.Rows.Add(2, "الموظف");
                    dt.Rows.Add(3, "النقدية النثرية");
                }
                else
                {
                    dt.Rows.Add(1, "Vendor");
                    dt.Rows.Add(2, "Employee");
                    dt.Rows.Add(3, "Petty Cash");
                }

                cmbType.ValueMember = "id";
                cmbType.DisplayMember = "name";
                cmbType.DataSource = dt;

                cmbType.RightToLeft =
                    arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                if (cmbType.Items.Count > 0)
                    cmbType.SelectedIndex = 0;

                cmbType.Invalidate();
                cmbType.Refresh();
            }
            finally
            {
                cmbType.SelectedIndexChanged +=
                    cmbType_SelectedIndexChanged;
            }
        }

        private int GetSelectedPaymentTypeId()
        {
            if (cmbType.SelectedValue == null)
                return 0;

            if (cmbType.SelectedValue is DataRowView row)
            {
                return Convert.ToInt32(row["id"]);
            }

            int id;

            return int.TryParse(
                cmbType.SelectedValue.ToString(),
                out id)
                    ? id
                    : 0;
        }

        private string GetSelectedPaymentType()
        {
            switch (GetSelectedPaymentTypeId())
            {
                case 1:
                    return "Vendor";

                case 2:
                    return "Employee";

                case 3:
                    return "Petty Cash";

                default:
                    return "";
            }
        }
        public MasterPaymentVoucher()
        {
            InitializeComponent();

           // LocalizationManager.LocalizeForm(this);

            PaymentVoucherUpdatedHandler =
                (sender, args) =>
                {
                    string type = GetSelectedPaymentType();

                    if (!string.IsNullOrWhiteSpace(type))
                        BindPayment(type);
                };

            //.PaymentVoucher +=
                //PaymentVoucherUpdatedHandler;

            headerUC1.FormText = this.Text;
        }
        private void MasterPaymentVoucher_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.PaymentVoucher -= PaymentVoucherUpdatedHandler;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewPaymentVoucher());
        }
        private bool IsArabicCulture()
        {
            string language =
               Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.StartsWith(
                "ar",
                StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyLanguageLayout()
        {
            bool arabic = IsArabicCulture();

            if (arabic)
            {
                guna2HtmlLabel2.Text =
                    "قسم سندات الدفع";

                guna2HtmlLabel2.RightToLeft =
                    RightToLeft.Yes;

                guna2HtmlLabel2.TextAlign =
                    ContentAlignment.MiddleRight;

                guna2HtmlLabel2.AutoSize = false;

                guna2HtmlLabel2.Location =
                    new Point(20, guna2HtmlLabel2.Top);

                guna2HtmlLabel2.Width =
                    Math.Max(
                        100,
                        this.ClientSize.Width - 40);

                guna2HtmlLabel2.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right;

                dgvCustomer.RightToLeft =
                    RightToLeft.Yes;

                dgvDetails.RightToLeft =
                    RightToLeft.Yes;

                dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                dgvDetails.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                guna2HtmlLabel2.Text =
                    "Master Payment Voucher";

                guna2HtmlLabel2.RightToLeft =
                    RightToLeft.No;

                guna2HtmlLabel2.TextAlign =
                    ContentAlignment.MiddleLeft;

                guna2HtmlLabel2.AutoSize = false;

                guna2HtmlLabel2.Location =
                    new Point(20, guna2HtmlLabel2.Top);

                guna2HtmlLabel2.Width =
                    Math.Max(
                        100,
                        this.ClientSize.Width - 40);

                guna2HtmlLabel2.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right;

                dgvCustomer.RightToLeft =
                    RightToLeft.No;

                dgvDetails.RightToLeft =
                    RightToLeft.No;

                dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;

                dgvDetails.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private void MasterPaymentVoucher_Load(
       object sender,
       EventArgs e)
        {
            //AppTheme.DefaultFont(dgvCustomer);
            //AppTheme.DefaultFont(dgvDetails);

            StyleGrid(dgvCustomer);
            StyleGrid(dgvDetails);

            ApplyLanguageLayout();

            // Must run after// LocalizationManager has already
            // applied the application's language.
            LoadPaymentTypeCombo();

            string type = GetSelectedPaymentType();

            if (!string.IsNullOrWhiteSpace(type))
                BindPayment(type);

            SetViewDocHeaderLanguage();
        }

        private void StyleGrid(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgv.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(204, 229, 255);
            dgv.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            // Header styles
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Silver; // Set header background to Silver
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;  // Keep text black for contrast
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9);

            // General Grid Appearance
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 28;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public void BindPayment(string type)
        {
            DataTable dt;
            if (chkDate.Checked)
                dt = ClinicDatabase.ExecuteDataTable(@"
                    SELECT 
                        tp.id, 
                        tp.date AS Date,
                        tp.code AS 'Payment Code',
                        LPAD(tp.id, 4, '0') AS 'JV NO', 
                        tp.amount AS Amount,
                        CONCAT(tc.code, ' - ', tc.name) AS 'Debit Account', 
                        CONCAT(tc1.code, ' - ', tc1.name) AS 'Credit Account' 
                    FROM tbl_payment_voucher tp
                    INNER JOIN tbl_coa_level_4 tc ON tp.debit_account_id = tc.id
                    INNER JOIN tbl_coa_level_4 tc1 ON tp.credit_account_id = tc1.id
                    WHERE tp.type = @type",
                    ClinicDatabase.CreateParameterNew("type", type));
            else
                dt = ClinicDatabase.ExecuteDataTable(@"
                    SELECT 
                        tp.id, 
                        tp.date AS Date,
                        tp.code AS 'Payment Code',
                        LPAD(tp.id, 4, '0') AS 'JV NO', 
                        tp.amount AS Amount,
                        CONCAT(tc.code, ' - ', tc.name) AS 'Debit Account', 
                        CONCAT(tc1.code, ' - ', tc1.name) AS 'Credit Account' 
                    FROM tbl_payment_voucher tp
                    INNER JOIN tbl_coa_level_4 tc ON tp.debit_account_id = tc.id
                    INNER JOIN tbl_coa_level_4 tc1 ON tp.credit_account_id = tc1.id
                    WHERE tp.type = @type 
                        AND tp.date >= @dateFrom 
                        AND tp.date <= @dateTo",
                    ClinicDatabase.CreateParameterNew("dateFrom", dtFrom.Value.Date),
                    ClinicDatabase.CreateParameterNew("dateTo", dtTo.Value.Date),
                    ClinicDatabase.CreateParameterNew("type", type));

            dgvCustomer.DataSource = dt;
            dgvCustomer.Columns["id"].Visible = false;
            dgvCustomer.Columns["Amount"].DefaultCellStyle.Format = "N2";
            //dgvCustomer.Columns["Amount"].Width = 100;
            dgvCustomer.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            if (dgvCustomer.Rows.Count > 0)
            {
                BindPVDetails();
                btnDelete.Enabled = UserPermissions.canDelete("Payment Voucher");
            }
            else
                dgvDetails.DataSource = null;

           // LocalizationManager.LocalizeDataGridViewHeaders(
//
            if (dt != null)
                AddViewDocImageColumn();

            // ViewDoc is dynamically created, so force its language LAST.
            SetViewDocHeaderLanguage();

            dgvCustomer.Invalidate();
            dgvCustomer.Refresh();
        }
        private void dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvCustomer.Columns.Contains("ViewDoc"))
            {
                SetViewDocIcons();
            }
        }
        System.Drawing.Image enabledIcon = Clinic.Properties.Resources.doc_icon;   // regular icon
        System.Drawing.Image disabledIcon = null; // or Properties.Resources.doc_icon_disabled

        private void SetViewDocIcons()
        {
            foreach (DataGridViewRow row in dgvCustomer.Rows)
            {
                int purchaseId = Convert.ToInt32(row.Cells["id"].Value);

                bool hasAttachment = AttachmentManager.HasAttachment(purchaseId, "PAYMENT VOUCHER");
                // You need a function to check if an invoice has attachments
                row.Cells["ViewDoc"].Value = hasAttachment ? enabledIcon : disabledIcon;
            }
        }

        private void AddViewDocImageColumn()
        {
            if (!dgvCustomer.Columns.Contains("ViewDoc"))
            {
                DataGridViewImageColumn imgColumn =
                    new DataGridViewImageColumn();

                imgColumn.Name = "ViewDoc";

                imgColumn.ImageLayout =
                    DataGridViewImageCellLayout.Zoom;

                dgvCustomer.Columns.Add(imgColumn);
            }

            // Always force correct language AFTER creating the column.
            SetViewDocHeaderLanguage();
        }
        private void SetViewDocHeaderLanguage()
        {
            if (!dgvCustomer.Columns.Contains("ViewDoc"))
                return;

            bool arabic = IsArabicCulture();

            DataGridViewColumn column =
                dgvCustomer.Columns["ViewDoc"];

            if (arabic)
            {
                column.HeaderText = "عرض المستند";
                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                column.HeaderText = "View Doc";
                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;
            }
        }
        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCustomer.Columns[e.ColumnIndex].Name == "ViewDoc")
            {
                var cell = dgvCustomer.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value == null) return; // no attachment, do nothing

                int purchaseId = Convert.ToInt32(dgvCustomer.Rows[e.RowIndex].Cells["id"].Value);
                AttachmentManager.OpenAttachmentByType(purchaseId, "PAYMENT VOUCHER");
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewPaymentVoucher(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            // Check if the double-clicked column is "JV NO"
            if (dgvCustomer.Columns["JV NO"].Index == e.ColumnIndex)
            {
                var jvNoCell = dgvCustomer.CurrentRow.Cells["JV NO"];

                // ✅ Check if JV NO cell is not null or empty
                if (jvNoCell != null && jvNoCell.Value != null && !string.IsNullOrWhiteSpace(jvNoCell.Value.ToString()))
                {
                    frmMain.openChildForm(new MasterTransactionJournal(jvNoCell.Value.ToString(), "PAYMENT"));
                }
                else
                {
                    //MessageBox.Show("Journal Voucher No. is missing or invalid.", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                frmMain.openChildForm(new frmViewPaymentVoucher(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
            }
        }
        private void dgvCustomer_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void dgvCustomer_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;

        }
        private void chkDate_CheckedChanged(
      object sender,
      EventArgs e)
        {
            if (chkDate.Checked)
                dtFrom.Enabled = dtTo.Enabled = false;
            else
                dtFrom.Enabled = dtTo.Enabled = true;

            string type = GetSelectedPaymentType();

            if (!string.IsNullOrWhiteSpace(type))
                BindPayment(type);
        }

        private void cmbType_SelectedIndexChanged(
      object sender,
      EventArgs e)
        {
            string type = GetSelectedPaymentType();

            if (!string.IsNullOrWhiteSpace(type))
                BindPayment(type);
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindPVDetails();
        }

        private void BindPVDetails()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"
                               SELECT tp.Id,tp.Date,tp.inv_id `Inv No`,tp.inv_code `Invoice`,tp.Total,tp.Payment,tp.Description
                                FROM tbl_payment_voucher_details tp WHERE tp.payment_id = @id;",
                      ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));

            dgvDetails.DataSource = dt;

            dgvDetails.Columns["Total"].DefaultCellStyle.Format = dgvDetails.Columns["Payment"].DefaultCellStyle.Format = "N2";
            //dgvDetails.Columns["Total"].Width = 100;
            dgvDetails.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns["Payment"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvDetails);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_payment_voucher SET state = -1 WHERE id = @id ",
                                          ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            ////Utilities.LogAudit(-1, "Delete Payment Voucher", "Payment Voucher", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()), "Deleted Payment Voucher: " + dgvCustomer.SelectedRows[0].Cells["Payment Code"].Value.ToString());
            BindPayment(GetSelectedPaymentType());
        }

        private void btnRecycle_Click(object sender, EventArgs e)
        {
            MasterRecycle restoreForm = new MasterRecycle("Payment");
            restoreForm.DataRestored += RestoreForm_DataRestored;
            restoreForm.ShowDialog();
        }
        private void RestoreForm_DataRestored(
     object sender,
     EventArgs e)
        {
            BindPayment(GetSelectedPaymentType());
        }

        private void dtFrom_ValueChanged(
       object sender,
       EventArgs e)
        {
            string type = GetSelectedPaymentType();

            if (!string.IsNullOrWhiteSpace(type))
                BindPayment(type);
        }

        private void dtTo_ValueChanged(
            object sender,
            EventArgs e)
        {
            string type = GetSelectedPaymentType();

            if (!string.IsNullOrWhiteSpace(type))
                BindPayment(type);
        }
    }
}
