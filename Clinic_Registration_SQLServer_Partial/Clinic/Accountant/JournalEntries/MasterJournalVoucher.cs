using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterJournalVoucher : Form
    {
        private EventHandler JournalVoucherUpdatedHandler;

        // Language-only changes. Existing controls, layout, events and business logic are preserved.
        private bool IsArabicLanguage()
        {
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            return language.Equals("ar", StringComparison.OrdinalIgnoreCase) ||
                   language.Equals("Arabic", StringComparison.OrdinalIgnoreCase) ||
                   language.StartsWith("ar-", StringComparison.OrdinalIgnoreCase);
        }

        private string TranslateJournalText(string value, bool arabic)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            switch (value.Trim())
            {
                case "Master Journal Voucher":
                case "Journal Voucher Center":
                case "مركز سندات القيد":
                    return arabic ? "مركز سندات القيد" : "Journal Voucher Center";
                case "Date": case "التاريخ": return arabic ? "التاريخ" : "Date";
                case "Journal Code": case "كود القيد": return arabic ? "كود القيد" : "Journal Code";
                case "JV NO": case "رقم سند القيد": return arabic ? "رقم سند القيد" : "JV NO";
                case "Debit Amount": case "مبلغ المدين": return arabic ? "مبلغ المدين" : "Debit Amount";
                case "Credit Amount": case "مبلغ الدائن": return arabic ? "مبلغ الدائن" : "Credit Amount";
                case "View Doc": case "عرض المستند": return arabic ? "عرض المستند" : "View Doc";
                case "Partner": case "الشريك": return arabic ? "الشريك" : "Partner";
                case "Description": case "الوصف": return arabic ? "الوصف" : "Description";
                case "Name": case "الاسم": return arabic ? "الاسم" : "Name";
                case "All": case "الكل": return arabic ? "الكل" : "All";
                case "From": case "من": return arabic ? "من" : "From";
                case "To": case "إلى": return arabic ? "إلى" : "To";
                case "New": case "جديد": return arabic ? "جديد" : "New";
                case "Edit": case "تعديل": return arabic ? "تعديل" : "Edit";
                case "Delete": case "حذف": return arabic ? "حذف" : "Delete";
                case "Restore": case "استعادة": return arabic ? "استعادة" : "Restore";
                case "Journal": case "القيد": return arabic ? "القيد" : "Journal";
                default: return value;
            }
        }

        private void ApplyJournalLanguage()
        {
            bool arabic = IsArabicLanguage();

            // Translate existing controls only; do not add, remove, move or resize controls.
            TranslateJournalControls(this, arabic);

            if (headerUC1 != null)
                headerUC1.FormText = TranslateJournalText("Journal Voucher Center", arabic);

            if (Lbheader != null)
                Lbheader.Text = TranslateJournalText("Master Journal Voucher", arabic);

            if (dgvCustomer != null)
            {
                dgvCustomer.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
                foreach (DataGridViewColumn column in dgvCustomer.Columns)
                    column.HeaderText = TranslateJournalText(column.HeaderText, arabic);
            }

            if (dgvDetails != null)
            {
                dgvDetails.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
                foreach (DataGridViewColumn column in dgvDetails.Columns)
                    column.HeaderText = TranslateJournalText(column.HeaderText, arabic);
            }
        }

        private void TranslateJournalControls(Control parent, bool arabic)
        {
            if (parent == null) return;

            // Preserve text of controls whose content is not a UI caption.
            if (!(parent is DataGridView) && !(parent is ComboBox))
            {
                string translated = TranslateJournalText(parent.Text, arabic);
                if (!string.IsNullOrEmpty(translated))
                    parent.Text = translated;
            }

            if (parent is Label || parent is Button || parent is CheckBox || parent is RadioButton)
                parent.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;

            foreach (Control child in parent.Controls)
                TranslateJournalControls(child, arabic);
        }

        public MasterJournalVoucher()
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            JournalVoucherUpdatedHandler = (sender, args) => BindVoucherData();
            //.Journal += JournalVoucherUpdatedHandler;
            headerUC1.FormText = this.Text;
            ApplyJournalLanguage();
            //AppTheme.DefaultFont(dgvCustomer);
            //AppTheme.DefaultFont(dgvDetails);
        }
        private void MasterPaymentVoucher_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.Journal -= JournalVoucherUpdatedHandler;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewJournalVoucher(0));
        }
        private void MasterJournalVoucher_Load(object sender, EventArgs e)
        {
            StyleGrid(dgvCustomer);
            StyleGrid(dgvDetails);
            Lbheader.Text = "Master Journal Voucher";
            ApplyJournalLanguage();
            BindVoucherData();
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

        public void BindVoucherData()
        {
            DataTable dt;
            if (chkDate.Checked)
                dt = ClinicDatabase.ExecuteDataTable(@"
                        SELECT 
                            jv.id, 
                            jv.date AS Date,
                            jv.code AS 'Journal Code',
                            LPAD(jv.id, 4, '0') AS 'JV NO',
                            jv.debit AS 'Debit Amount', 
                            jv.credit AS 'Credit Amount' 
                        FROM tbl_journal_voucher jv Where state=0");
            else
                dt = ClinicDatabase.ExecuteDataTable(@"
                        SELECT 
                            jv.id, 
                            jv.date AS Date,
                            jv.code AS 'Journal Code',
                            LPAD(jv.id, 4, '0') AS 'JV NO',
                            jv.debit AS 'Debit Amount', 
                            jv.credit AS 'Credit Amount' 
                        FROM tbl_journal_voucher jv Where state=0
                            AND jv.date >= @dateFrom 
                            AND jv.date <= @dateTo",
                    ClinicDatabase.CreateParameterNew("dateFrom", dtFrom.Value.Date),
                    ClinicDatabase.CreateParameterNew("dateTo", dtTo.Value));

            dgvCustomer.DataSource = dt;
            dgvCustomer.Columns["id"].Visible = false;
            if (dgvCustomer.Rows.Count > 0)
                BindJVDetails();
            else
                dgvDetails.DataSource = null;

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
            ApplyJournalLanguage();
            if (dt != null)
            {
                AddViewDocImageColumn();
            }
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

                bool hasAttachment = AttachmentManager.HasAttachment(purchaseId, "JOURNAL VOUCHER");
                // You need a function to check if an invoice has attachments
                row.Cells["ViewDoc"].Value = hasAttachment ? enabledIcon : disabledIcon;
            }
        }

        private void AddViewDocImageColumn()
        {
            if (!dgvCustomer.Columns.Contains("ViewDoc"))
            {
                DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
                imgColumn.Name = "ViewDoc";
                imgColumn.HeaderText = "View Doc";
                //imgColumn.Image = Properties.Resources.doc_icon; // <-- your image here
                imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom; // fit nicely
                dgvCustomer.Columns.Add(imgColumn);
            }
            dgvCustomer.Columns["ViewDoc"].HeaderText = TranslateJournalText("View Doc", IsArabicLanguage());
        }
        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCustomer.Columns[e.ColumnIndex].Name == "ViewDoc")
            {
                var cell = dgvCustomer.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value == null) return; // no attachment, do nothing

                int purchaseId = Convert.ToInt32(dgvCustomer.Rows[e.RowIndex].Cells["id"].Value);
                AttachmentManager.OpenAttachmentByType(purchaseId, "JOURNAL VOUCHER");
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewJournalVoucher(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            if (dgvCustomer.Columns["JV NO"].Index == e.ColumnIndex)
                frmMain.openChildForm(new MasterTransactionJournal(dgvCustomer.CurrentRow.Cells["JV NO"].Value.ToString(), "JOURNAL"));
            else
                frmMain.openChildForm(new frmViewJournalVoucher(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
        private void dgvCustomer_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void dgvCustomer_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;

        }
        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
                dtFrom.Enabled = dtTo.Enabled = false;
            else
                dtFrom.Enabled = dtTo.Enabled = true;
            BindVoucherData();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindVoucherData();
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindJVDetails();
        }

        private void BindJVDetails()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"
                           SELECT 
                           jd.Date,
                           a.Name,
                           jd.debit AS 'Debit Amount',
                           jd.credit AS 'Credit Amount',
                           jd.partner AS Partner,
                           jd.description AS Description
                        FROM tbl_journal_voucher_details jd
                        INNER JOIN tbl_coa_level_4 a ON jd.account_id = a.id
                        WHERE jd.inv_id = @id;",
                      ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));

            dgvDetails.DataSource = dt;

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvDetails);
            ApplyJournalLanguage();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_journal_voucher SET state = -1 WHERE id = @id ",
                                          ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            ////Utilities.LogAudit(-1, "Delete Journal Voucher", "Journal Voucher", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()), "Deleted Journal Voucher: " + dgvCustomer.SelectedRows[0].Cells["Journal Code"].Value.ToString());
            BindVoucherData();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            MasterRecycle restoreForm = new MasterRecycle("Journal");
            restoreForm.DataRestored += RestoreForm_DataRestored;
            restoreForm.ShowDialog();
        }
        private void RestoreForm_DataRestored(object sender, EventArgs e)
        {
            BindVoucherData();
        }

        private void nToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewJournalVoucher(0));
        }

        private void editCustomerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewJournalVoucher(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }

        private void deleteCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_journal_voucher SET state = -1 WHERE id = @id ",
                                          ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            ////Utilities.LogAudit(-1, "Delete Journal Voucher", "Journal Voucher", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()), "Deleted Journal Voucher: " + dgvCustomer.SelectedRows[0].Cells["Journal Code"].Value.ToString());
            BindVoucherData();
        }

        private void restoreJournalVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MasterRecycle restoreForm = new MasterRecycle("Journal");
            restoreForm.DataRestored += RestoreForm_DataRestored;
            restoreForm.ShowDialog();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {

        }

        private void Lbheader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
