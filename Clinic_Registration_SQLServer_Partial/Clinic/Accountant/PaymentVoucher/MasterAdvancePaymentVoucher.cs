using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterAdvancePaymentVoucher : Form
    {
        private EventHandler PaymentVoucherUpdatedHandler;
        private static readonly Dictionary<int, string>
            PaymentTypeEnglishNames =
                new Dictionary<int, string>
                {
            { 1, "Vendor" },
            { 2, "Employee" },
            { 3, "Petty Cash" }
                };

        private static readonly Dictionary<int, string>
            PaymentTypeArabicNames =
                new Dictionary<int, string>
                {
            { 1, "المورد" },
            { 2, "الموظف" },
            { 3, "النقدية النثرية" }
                }; public MasterAdvancePaymentVoucher()
        {
            InitializeComponent();
            LocalizationManager.ApplySavedLanguage();
            LocalizationManager.LocalizeForm(this);
            ApplyLanguageLayout();
            ApplyRuntimeArabicText();
            PaymentVoucherUpdatedHandler =
                (sender, args) =>
                {
                    BindPayment(
                        GetSelectedPaymentType());

                    ApplyRuntimeArabicText();
                }; //.PaymentVoucher += PaymentVoucherUpdatedHandler;
            headerUC1.FormText = this.Text;
            //AppTheme.DefaultFont(dgvCustomer);
            //AppTheme.DefaultFont(dgvDetails);
        }
        private void MasterAdvancePaymentVoucher_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.PaymentVoucher -= PaymentVoucherUpdatedHandler;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewPaymentVoucherAdvance());
        }
        private void MasterAdvancePaymentVoucher_Load(
      object sender,
      EventArgs e)
        {
            StyleGrid(dgvCustomer);
            StyleGrid(dgvDetails);

            ApplyLanguageLayout();

            LoadPaymentTypeCombo();

            string type = GetSelectedPaymentType();

            if (!string.IsNullOrWhiteSpace(type))
                BindPayment(type);

            ApplyLanguageLayout();
            ApplyRuntimeArabicText();
            SetViewDocHeaderLanguage();
        }
        private void ApplyRuntimeArabicText()
        {
            bool arabic = IsArabicCulture();

            if (arabic)
            {
                btnNew.Text = GetLocalizedOrArabic("New AP", "سند جديد");
                btnEdit.Text = GetLocalizedOrArabic("Edit AP", "تعديل السند");
                btnDelete.Text = GetLocalizedOrArabic("Delete", "حذف");
                btnRecycle.Text = GetLocalizedOrArabic("Recycle", "استعادة");
                label4.Text = GetLocalizedOrArabic("Type", "النوع");
                label11.Text = GetLocalizedOrArabic("From", "من تاريخ");
                label2.Text = GetLocalizedOrArabic("To", "إلى تاريخ");
                chkDate.Text = GetLocalizedOrArabic("All", "الكل");
                this.Text = GetLocalizedOrArabic(
                    "Advance Payment Voucher Center", "مركز سندات الدفع المقدم");
            }
            else
            {
                btnNew.Text = GetLocalizedOrEnglish("New AP");
                btnEdit.Text = GetLocalizedOrEnglish("Edit AP");
                btnDelete.Text = GetLocalizedOrEnglish("Delete");
                btnRecycle.Text = GetLocalizedOrEnglish("Recycle");
                label4.Text = GetLocalizedOrEnglish("Type");
                label11.Text = GetLocalizedOrEnglish("From");
                label2.Text = GetLocalizedOrEnglish("To");
                chkDate.Text = GetLocalizedOrEnglish("All");
                this.Text = GetLocalizedOrEnglish("Advance Payment Voucher Center");
            }

            headerUC1.FormText = this.Text;
            ApplyLanguageLayout();
            SetViewDocHeaderLanguage();
            ApplyGridHeaderFallbacks(dgvCustomer);
            ApplyGridHeaderFallbacks(dgvDetails);

            cmbType.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            cmbType.Invalidate();
            cmbType.Refresh();
            dgvCustomer.Invalidate();
            dgvCustomer.Refresh();
            dgvDetails.Invalidate();
            dgvDetails.Refresh();
        }

        private string GetLocalizedOrEnglish(string english)
        {
            string translated = LocalizationManager.GetString(english);
            return string.IsNullOrWhiteSpace(translated) ? english : translated;
        }

        private string GetLocalizedOrArabic(string english, string arabicFallback)
        {
            string translated = LocalizationManager.GetString(english);
            if (string.IsNullOrWhiteSpace(translated) ||
                string.Equals(translated, english, StringComparison.OrdinalIgnoreCase))
                return arabicFallback;

            return translated;
        }

        private void ApplyGridHeaderFallbacks(DataGridView grid)
        {
            if (grid == null || !IsArabicCulture())
                return;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                string key = (column.Name + "|" + column.DataPropertyName + "|" +
                              column.HeaderText).ToLowerInvariant();

                if (key.Contains("date"))
                    column.HeaderText = "التاريخ";
                else if (key.Contains("payment code"))
                    column.HeaderText = "رقم سند الدفع";
                else if (key.Contains("jv no"))
                    column.HeaderText = "رقم القيد";
                else if (key.Contains("amount"))
                    column.HeaderText = "المبلغ";
                else if (key.Contains("debit account"))
                    column.HeaderText = "الحساب المدين";
                else if (key.Contains("credit account"))
                    column.HeaderText = "الحساب الدائن";
                else if (key.Contains("description"))
                    column.HeaderText = "الوصف";
                else if (key.Contains("name"))
                    column.HeaderText = "الاسم";

                column.HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
        }

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

        //private string GetSelectedPaymentType()
        //{
        //    if (cmbType.SelectedItem is PaymentTypeItem item)
        //        return item.Value;

        //    return "";
        //}
        private string GetSelectedPaymentType()
        {
            int typeId = GetSelectedPaymentTypeId();

            switch (typeId)
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
            {
                dt = ClinicDatabase.ExecuteDataTable(@"
            SELECT 
                tp.id, 
                tp.date AS Date,
                tp.pv_code AS 'Payment Code',
                LPAD(tp.id, 4, '0') AS 'JV NO', 
                tp.amount AS Amount,
                CONCAT(tc.code, ' - ', tc.name) AS 'Debit Account', 
                CONCAT(tc1.code, ' - ', tc1.name) AS 'Credit Account' 
            FROM tbl_advance_payment_voucher tp
            INNER JOIN tbl_coa_level_4 tc 
                ON tp.debit_account_id = tc.id
            INNER JOIN tbl_coa_level_4 tc1 
                ON tp.credit_account_id = tc1.id
            WHERE tp.type = @type",
                    ClinicDatabase.CreateParameterNew(
                        "type",
                        type));
            }
            else
            {
                dt = ClinicDatabase.ExecuteDataTable(@"
            SELECT 
                tp.id, 
                tp.date AS Date,
                tp.pv_code AS 'Payment Code',
                LPAD(tp.id, 4, '0') AS 'JV NO', 
                tp.amount AS Amount,
                CONCAT(tc.code, ' - ', tc.name) AS 'Debit Account', 
                CONCAT(tc1.code, ' - ', tc1.name) AS 'Credit Account' 
            FROM tbl_advance_payment_voucher tp
            INNER JOIN tbl_coa_level_4 tc 
                ON tp.debit_account_id = tc.id
            INNER JOIN tbl_coa_level_4 tc1 
                ON tp.credit_account_id = tc1.id
            WHERE tp.type = @type 
              AND tp.date >= @dateFrom 
              AND tp.date <= @dateTo",
                    ClinicDatabase.CreateParameterNew(
                        "dateFrom",
                        dtFrom.Value.Date),
                    ClinicDatabase.CreateParameterNew(
                        "dateTo",
                        dtTo.Value.Date),
                    ClinicDatabase.CreateParameterNew(
                        "type",
                        type));
            }

            dgvCustomer.DataSource = dt;

            if (dgvCustomer.Columns.Contains("id"))
                dgvCustomer.Columns["id"].Visible = false;

            if (dgvCustomer.Rows.Count > 0)
                BindPVDetails();
            else
                dgvDetails.DataSource = null;

            // -----------------------------------------
            // FIRST create dynamic column
            // -----------------------------------------
            AddViewDocImageColumn();

            // -----------------------------------------
            // THEN localize normal DB columns
            // -----------------------------------------
            LocalizationManager.LocalizeDataGridViewHeaders(
                dgvCustomer);

            // -----------------------------------------
            // LAST:
            // ViewDoc is OUR dynamic column.
            // Do not depend on// LocalizationManager.
            // -----------------------------------------
            SetViewDocHeaderLanguage();
            ApplyGridHeaderFallbacks(dgvCustomer);
            ApplyRuntimeArabicText();

            dgvCustomer.Invalidate();
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

                bool hasAttachment = AttachmentManager.HasAttachment(purchaseId, "ADVANCE PAYMENT VOUCHER");
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
                AttachmentManager.OpenAttachmentByType(purchaseId, "ADVANCE PAYMENT VOUCHER");
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewPaymentVoucherAdvance(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            if (dgvCustomer.Columns["JV NO"].Index == e.ColumnIndex)
                frmMain.openChildForm(new MasterTransactionJournal(dgvCustomer.CurrentRow.Cells["JV NO"].Value.ToString(), "PAYMENT"));
            else
                frmMain.openChildForm(new frmViewPaymentVoucherAdvance(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
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

            if (string.IsNullOrWhiteSpace(type))
                return;

            BindPayment(type);
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindPVDetails();
        }

        private void BindPVDetails()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"
                                   SELECT 
                                   e.name,
                                   pd.amount,
                                   pd.description
                                FROM tbl_advance_payment_voucher_details pd
                                INNER JOIN tbl_employee e ON pd.name = e.id
                                WHERE pd.payment_id =@id;",
                      ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));

            dgvDetails.DataSource = dt;

            LocalizationManager.LocalizeDataGridViewHeaders(dgvDetails);
            ApplyGridHeaderFallbacks(dgvDetails);
            ApplyRuntimeArabicText();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            if (UserPermissions.canDelete("Vouchers"))
            {
                ClinicDatabase.ExecuteNonQuery("UPDATE tbl_advance_payment_voucher SET state = -1 WHERE id = @id ",
                                              ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
                BindPayment(GetSelectedPaymentType());
                ////Utilities.LogAudit(-1, "Delete Advance Payment Voucher", "Advance Payment Voucher", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()), "Deleted Advance Payment Voucher: " + dgvCustomer.SelectedRows[0].Cells["Payment Code"].Value.ToString());
            }
        }
        //private bool IsArabicLanguage()
        //{
        //// LocalizationManager has already translated the form
        // before Form_Load runs.
        //
        // label4 is:
        // English = "Type"
        // Arabic  = "نوع"

        //
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
                    "قسم سندات الدفع المقدم";

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
                    "Advance Payment Voucher Center";

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

            // Explicitly restore combo direction for both languages.
            cmbType.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;
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
    }
}
