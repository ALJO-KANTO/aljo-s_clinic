using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterPettyCash : Form
    {
        private DataView _dataView;

        // Localization-only helpers. Existing controls and business logic are preserved.
        private bool IsArabicLanguage()
        {
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            return language.Equals("ar", StringComparison.OrdinalIgnoreCase) ||
                   language.Equals("Arabic", StringComparison.OrdinalIgnoreCase) ||
                   language.StartsWith("ar-", StringComparison.OrdinalIgnoreCase);
        }

        private string TranslatePettyCashText(string value, bool arabic)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            switch (value.Trim())
            {
                case "PettyCash Center": case "Petty Cash Center": case "مركز العهدة النثرية": return arabic ? "مركز العهدة النثرية" : "PettyCash Center";
                case "Settings": case "إعدادات": return arabic ? "إعدادات" : "Settings";
                case "Petty Cash Ledger": case "سجل النقدية الصغيرة": return arabic ? "سجل النقدية الصغيرة" : "Petty Cash Ledger";
                case "Petty Cash": case "العهدة النثرية": return arabic ? "العهدة النثرية" : "Petty Cash";
                case "Petty Cash Vouchers": case "سندات العهدة النثرية": return arabic ? "سندات العهدة النثرية" : "Petty Cash Vouchers";
                case "Request": case "طلب": return arabic ? "طلب" : "Request";
                case "Transactions": case "المعاملات": return arabic ? "المعاملات" : "Transactions";
                case "Employee Name": case "اسم الموظف": return arabic ? "اسم الموظف" : "Employee Name";
                case "Amount": case "المبلغ": return arabic ? "المبلغ" : "Amount";
                case "Date": case "التاريخ": return arabic ? "التاريخ" : "Date";
                case "Submission Date": case "تاريخ التقديم": return arabic ? "تاريخ التقديم" : "Submission Date";
                case "Account": case "الحساب": return arabic ? "الحساب" : "Account";
                case "Total": case "الإجمالي": return arabic ? "الإجمالي" : "Total";
                case "Total With VAT": case "الإجمالي شامل ضريبة القيمة المضافة": return arabic ? "الإجمالي شامل ضريبة القيمة المضافة" : "Total With VAT";
                case "VAT": case "Vat": case "ضريبة القيمة المضافة": return arabic ? "ضريبة القيمة المضافة" : "VAT";
                case "Notes": case "ملاحظات": return arabic ? "ملاحظات" : "Notes";
                case "Status": case "State": case "الحالة": return arabic ? "الحالة" : (value == "State" ? "State" : "Status");
                case "Pending": case "قيد الانتظار": return arabic ? "قيد الانتظار" : "Pending";
                case "Confirmed": case "مؤكد": return arabic ? "مؤكد" : "Confirmed";
                case "No": case "رقم": return arabic ? "رقم" : "No";
                case "Debit Account": case "حساب المدين": return arabic ? "حساب المدين" : "Debit Account";
                case "Credit Account": case "حساب الدائن": return arabic ? "حساب الدائن" : "Credit Account";
                case "Pay": case "المدفوع": return arabic ? "المدفوع" : "Pay";
                case "Change": case "الباقي": return arabic ? "الباقي" : "Change";
                case "Description": case "الوصف": return arabic ? "الوصف" : "Description";
                case "SN#": case "م": return arabic ? "م" : "SN#";
                case "Voucher No": case "رقم السند": return arabic ? "رقم السند" : "Voucher No";
                case "Type": case "النوع": return arabic ? "النوع" : "Type";
                case "Inv Id": case "Invoice Id": case "رقم الفاتورة": return arabic ? "رقم الفاتورة" : "Inv Id";
                case "Debit": case "مدين": return arabic ? "مدين" : "Debit";
                case "Credit": case "دائن": return arabic ? "دائن" : "Credit";
                case "Balance": case "الرصيد": return arabic ? "الرصيد" : "Balance";
                case "View Doc": case "عرض المستند": return arabic ? "عرض المستند" : "View Doc";
                case "New Petty Cash": case "عهدة نثرية جديدة": return arabic ? "عهدة نثرية جديدة" : "New Petty Cash";
                case "Edit Petty Cash": case "تعديل العهدة النثرية": return arabic ? "تعديل العهدة النثرية" : "Edit Petty Cash";
                case "Delete Petty Cash": case "حذف العهدة النثرية": return arabic ? "حذف العهدة النثرية" : "Delete Petty Cash";
                case "Approval": case "الموافقة": return arabic ? "الموافقة" : "Approval";
                case "Add Request": case "إضافة طلب": return arabic ? "إضافة طلب" : "Add Request";
                case "Add Account": case "إضافة حساب": return arabic ? "إضافة حساب" : "Add Account";
                case "Add Category": case "إضافة فئة": return arabic ? "إضافة فئة" : "Add Category";
                case "New Transaction": case "معاملة جديدة": return arabic ? "معاملة جديدة" : "New Transaction";
                case "Petty Cash Card": case "بطاقة العهدة النثرية": return arabic ? "بطاقة العهدة النثرية" : "Petty Cash Card";
                case "Category": case "الفئة": return arabic ? "الفئة" : "Category";
                case "All": case "الكل": return arabic ? "الكل" : "All";
                case "Invalid or missing ID.": return arabic ? "المعرّف غير صالح أو غير موجود." : value;
                case "Please select a row first.": return arabic ? "يرجى تحديد صف أولاً." : value;
                case "Request already closed": return arabic ? "تم إغلاق الطلب بالفعل." : value;
                case "Are you sure you want to delete this Petty Cash Card?": return arabic ? "هل أنت متأكد أنك تريد حذف بطاقة العهدة النثرية هذه؟" : value;
                case "Confirm Deletion": return arabic ? "تأكيد الحذف" : value;
                case "Cannot delete this Petty Cash Card because it is referenced in Petty Cash Vouchers.": return arabic ? "لا يمكن حذف بطاقة العهدة النثرية لأنها مرتبطة بسندات العهدة النثرية." : value;
                case "Deletion Error": return arabic ? "خطأ في الحذف" : value;

                case "Configuration": return arabic ? "الإعدادات" : "Configuration";
                case "Search By Name, Code": return arabic ? "البحث بالاسم أو الرمز" : "Search By Name, Code";
                case "Petty Cash Log": return arabic ? "سجل العهدة النثرية" : "Petty Cash Log";
                case "Petty Cash Voucher": return arabic ? "سند العهدة النثرية" : "Petty Cash Voucher";


                default: return value;
            }
        }

        private void ApplyPettyCashLanguage()
        {
            bool arabic = IsArabicLanguage();
            TranslatePettyCashControls(this, arabic);
            TranslatePettyCashControls(this, arabic);
            TranslatePettyCashSpecificControls(this, arabic);
            if (headerUC1 != null)
                headerUC1.FormText = TranslatePettyCashText("PettyCash Center", arabic);
            TranslateGridHeaders(dgvEmployees, arabic);
            TranslateGridHeaders(dgvInvoiceCash, arabic);
            TranslateGridHeaders(dgvRequest, arabic);
            TranslateGridHeaders(dgvInvoice, arabic);
        }

        private void TranslateGridHeaders(DataGridView grid, bool arabic)
        {
            if (grid == null) return;
            grid.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            foreach (DataGridViewColumn column in grid.Columns)
                column.HeaderText = TranslatePettyCashText(column.HeaderText, arabic);
        }

        private void TranslatePettyCashControls(Control parent, bool arabic)
        {
            if (parent == null) return;
            if (!(parent is DataGridView) && !(parent is ComboBox))
            {
                string translated = TranslatePettyCashText(parent.Text, arabic);
                if (!string.IsNullOrEmpty(translated)) parent.Text = translated;
            }
            if (parent is Label || parent is Button || parent is CheckBox || parent is RadioButton)
                parent.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            foreach (Control child in parent.Controls)
                TranslatePettyCashControls(child, arabic);
            if (parent.ContextMenuStrip != null)
                TranslatePettyCashToolStripItems(parent.ContextMenuStrip.Items, arabic);
        }

        private void TranslatePettyCashToolStripItems(ToolStripItemCollection items, bool arabic)
        {
            foreach (ToolStripItem item in items)
            {
                string translated = TranslatePettyCashText(item.Text, arabic);
                if (!string.IsNullOrEmpty(translated)) item.Text = translated;
                if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
                    TranslatePettyCashToolStripItems(menuItem.DropDownItems, arabic);
            }
        }
        private EventHandler employeeUpdateHandler;
        public MasterPettyCash()
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            headerUC1.FormText = this.Text;
            ApplyPettyCashLanguage();
            employeeUpdateHandler = (sender, args) => BindEmployees();
            //.Employee += employeeUpdateHandler;
        }
        public void BindEmployees()
        {
            string query = @"SELECT 
                                e.id, 
                                e.code, 
                                e.name AS 'Employee Name',
                                COALESCE((SUM(pc.total)), 0) AS Amount
                            FROM tbl_employee e
                            INNER JOIN tbl_petty_cash pc ON pc.employee_id = e.id
                            GROUP BY e.id, e.code, e.name";

            if (EnableApproval)
                query = @"SELECT 
                                e.id, 
                                e.code, 
                                e.name AS 'Employee Name',
                                COALESCE(
										  (select SUM(pcr.pay) FROM tbl_petty_cash_request pcr WHERE pcr.state != 'New' AND pcr.Petty_cash_name = e.id)
										  - (SUM(pc.total)), 0) AS Amount
                            FROM tbl_employee e
                            INNER JOIN tbl_petty_cash pc ON pc.employee_id = e.id
                            GROUP BY e.id, e.code, e.name";

            DataTable dt = ClinicDatabase.ExecuteDataTable(query);
            _dataView = dt.DefaultView;
            dgvEmployees.DataSource = _dataView;

            dgvEmployees.Columns["id"].Visible = false;
            dgvEmployees.Columns["code"].Visible = false;
            dgvEmployees.Columns["code"].MinimumWidth = 100;
            dgvEmployees.Columns["Employee Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvEmployees.Columns["Amount"].DefaultCellStyle.Format = "N2";

            LocalizationManager.LocalizeDataGridViewHeaders(dgvEmployees);
            ApplyPettyCashLanguage();
            if (dt != null)
            {
                AddViewDocImageColumn();
            }
        }

        bool EnableApproval = false;

        private void MasterPettyCash_Load(object sender, EventArgs e)
        {
            var opt = Utilities.GeneralSettingsState("ENABLE PETTYCASH APPROVAL");
            if (!string.IsNullOrEmpty(opt) && int.Parse(opt) > 0)
            {
                EnableApproval = true;
            }
            else
            {
                EnableApproval = false;
            }
            if (!EnableApproval)
            {
                toolStripMenuItem30.Visible = toolStripMenuItem12.Visible = false;
            }
            else
            {
                newPettyCashToolStripMenuItem.Visible = editPettyCashToolStripMenuItem.Visible = deletePettyCashToolStripMenuItem.Visible = true;
                toolStripMenuItem30.Visible = toolStripMenuItem12.Visible = true;
                //if (frmLogin.userRole != "Admin" && frmLogin.userRole != "Manager")
                //{
                //    approvalToolStripMenuItem.Visible = false;
                //    approvalSubmitionToolStripMenuItem.Visible = false;
                //}
            }

            BindEmployees();
            dgvInvoice.ColumnHeadersVisible = false;
            //AppTheme.DefaultFont(dgvInvoice);
            //AppTheme.DefaultFont(dgvInvoiceCash);
            //AppTheme.DefaultFont(dgvRequest);
            ApplyPettyCashLanguage();

            // Reapply after the form finishes loading, so nested/custom controls
            // that set their text during loading also receive the selected language.
            BeginInvoke((MethodInvoker)(() => ApplyPettyCashLanguage()));
        }

        private void dgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string empCode = dgvEmployees.Rows[e.RowIndex].Cells["code"].Value.ToString();
                string empId = dgvEmployees.Rows[e.RowIndex].Cells["id"].Value.ToString();
                BindEmployeePettyCash(empId);
                BindEmployeePettyCashRequest(empId);
                BindEmployeePettyCashTransaction(empCode);
            }
        }

        private void BindEmployeePettyCash(string empId)
        {
            dgvInvoiceCash.DataSource = null;
            dgvInvoiceCash.ColumnHeadersVisible = true;

            dgvInvoiceCash.EnableHeadersVisualStyles = false;
            if (!EnableApproval)
            {
                dgvInvoiceCash.DataSource = null;
                dgvInvoiceCash.ColumnHeadersVisible = true;

                dgvInvoiceCash.Rows.Clear();
                //string query = @"
                //                SELECT 
                //                d.id,
                //                d.date AS `Date`,
                //                ps.code,
                //                coa.name AS `Account`,
                //                d.total AS `Total With VAT`,
                //                cat.name AS `Category`,
                //                d.note
                //                FROM 
                //                tbl_petty_cash_submition_details d
                //                INNER JOIN 
                //                tbl_petty_cash_submition ps ON d.petty_id = ps.id
                //                INNER JOIN 
                //                tbl_employee e ON ps.name = e.id
                //                INNER JOIN tbl_petty_cash_category cat ON d.category = cat.id
                //                INNER JOIN tbl_coa_level_4 coa ON d.account_id = coa.id
                //                WHERE e.code = @empCode;";

                string query = @"
                                SELECT 
                                pc.Id,
                                pc.voucher_date AS `Date`,
                                pc.Code,
                                ac.name AS `Account`,
                                pc.Amount as `Amount`,
                                pc.vat as `Vat`,
                                pc.Total as `Total`,
                                pc.Notes,
                                CASE 
                                    WHEN pc.`status` = 0 THEN 'Pending' 
                                    ELSE 'Confirmed' 
                                END AS Status
                                FROM tbl_petty_cash pc
                                LEFT JOIN tbl_coa_level_4 ac ON pc.cash_account_id = ac.id
                                WHERE pc.employee_id = @empId;
                                ";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@empId", empId));
                dgvInvoiceCash.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    dgvInvoiceCash.ColumnHeadersVisible = false;
                    return;
                }

                dgvInvoiceCash.Columns["Date"].HeaderText = "Submission Date";
                dgvInvoiceCash.Columns["Date"].DefaultCellStyle.Format = "dd-MM-yyyy";

                dgvInvoiceCash.Columns["Account"].HeaderText = "Account";
                //dgvInvoiceCash.Columns["Category"].HeaderText = "Category";
                //dgvInvoiceCash.Columns["Cost Center"].HeaderText = "Cost Center";
                dgvInvoiceCash.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvInvoiceCash.Columns["VAT"].DefaultCellStyle.Format = "N2";
                dgvInvoiceCash.Columns["Vat"].DefaultCellStyle.Format = "N2";
                dgvInvoiceCash.Columns["Vat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInvoiceCash.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInvoiceCash.Columns["VAT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvInvoiceCash.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                dgvInvoiceCash.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
                //dgvInvoiceCash.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
                //dgvInvoiceCash.DefaultCellStyle.Font = new Font("Times New Roman", 9);
                dgvInvoiceCash.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                LocalizationManager.LocalizeDataGridViewHeaders(dgvInvoiceCash);
                ApplyPettyCashLanguage();
            }
            else
            {
                dgvInvoiceCash.Rows.Clear();
                string query = @"
                            SELECT 
                            p.id,p.code as Code,p.voucher_date as Date,p.amount as Amount,p.vat as Vat,p.total as Total,
                            CASE 
                                WHEN p.`status` = 0 THEN 'Pending' 
                                ELSE 'Confirmed' 
                            END AS Status
                            From tbl_petty_cash p
                            INNER JOIN 
                            tbl_petty_cash_card c ON p.employee_id = c.name
                            INNER JOIN 
                            tbl_employee e ON e.id = p.employee_id
                            WHERE e.id = @empId ORDER BY p.id;";
                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@empId", empId));
                dgvInvoiceCash.DataSource = dt;
                if (dt.Rows.Count == 0)
                {
                    dgvInvoiceCash.ColumnHeadersVisible = false;
                    return;
                }

                dgvInvoiceCash.Columns["Date"].DefaultCellStyle.Format = "dd-MM-yyyy";
                dgvInvoiceCash.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvInvoiceCash.Columns["Vat"].DefaultCellStyle.Format = "N2";
                dgvInvoiceCash.Columns["Total"].DefaultCellStyle.Format = "N2";
                dgvInvoiceCash.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInvoiceCash.Columns["Vat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInvoiceCash.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInvoiceCash.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                dgvInvoiceCash.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
                //dgvInvoiceCash.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
                //dgvInvoiceCash.DefaultCellStyle.Font = new Font("Times New Roman", 9);
                dgvInvoiceCash.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                LocalizationManager.LocalizeDataGridViewHeaders(dgvInvoiceCash);
                ApplyPettyCashLanguage();
            }
        }
        private void BindEmployeePettyCashRequest(string empId)
        {
            dgvRequest.DataSource = null;
            dgvRequest.ColumnHeadersVisible = true;

            dgvRequest.EnableHeadersVisualStyles = false;
            if (EnableApproval)
            {
                dgvRequest.DataSource = null;
                dgvRequest.ColumnHeadersVisible = true;

                dgvRequest.Rows.Clear();
                string query = @"
                                SELECT a.id,a.request_date `Date`,a.request_ref `No`,a.Amount,ifnull(cr.name,'') AS `Debit Account`,ifnull(dr.name,'') AS `Credit Account`,a.State,a.Pay,a.Change,a.Description FROM tbl_petty_cash_request a
                                LEFT JOIN tbl_coa_level_4 dr ON dr.id = a.debit_account_id
                                LEFT JOIN tbl_coa_level_4 cr ON cr.id = a.credit_account_id WHERE a.Petty_cash_name = @empId;
                                ";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@empId", empId));
                dgvRequest.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    dgvRequest.ColumnHeadersVisible = false;
                    return;
                }

                dgvRequest.Columns["Date"].DefaultCellStyle.Format = "dd-MM-yyyy";

                //dgvRequest.Columns["Account"].HeaderText = "Account";
                //dgvRequest.Columns["Category"].HeaderText = "Category";
                //dgvRequest.Columns["Cost Center"].HeaderText = "Cost Center";
                dgvRequest.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvRequest.Columns["Change"].DefaultCellStyle.Format = "N2";
                dgvRequest.Columns["Pay"].DefaultCellStyle.Format = "N2";
                dgvRequest.Columns["Pay"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRequest.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRequest.Columns["Change"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvRequest.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                dgvRequest.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
                //dgvRequest.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
                //dgvRequest.DefaultCellStyle.Font = new Font("Times New Roman", 9);
                dgvRequest.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                LocalizationManager.LocalizeDataGridViewHeaders(dgvRequest);
                ApplyPettyCashLanguage();
            }
        }

        private void BindEmployeePettyCashTransaction(string empCode)
        {
            dgvInvoice.DataSource = null;
            dgvInvoice.ColumnHeadersVisible = true;
            dgvInvoice.EnableHeadersVisualStyles = false;
            //if (!EnableApproval)
            //{
            //    dgvInvoice.Rows.Clear();
            //    string query = @"
            //                SELECT 
            //                d.id,
            //                d.date AS `Date`,
            //                ps.code,
            //                coa.name AS `Account`,
            //                d.total AS `Total With VAT`,
            //                cat.name AS `Category`,
            //                d.note
            //                FROM 
            //                tbl_petty_cash_submition_details d
            //                INNER JOIN 
            //                tbl_petty_cash_submition ps ON d.petty_id = ps.id
            //                INNER JOIN 
            //                tbl_employee e ON ps.name = e.id
            //                INNER JOIN tbl_petty_cash_category cat ON d.category = cat.id
            //                INNER JOIN tbl_coa_level_4 coa ON d.account_id = coa.id
            //                WHERE e.code = @empCode;";

            //    DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@empCode", empCode));
            //    dgvInvoice.DataSource = dt;

            //    if (dt.Rows.Count == 0)
            //    {
            //        dgvInvoice.ColumnHeadersVisible = false;
            //        return;
            //    }

            //    dgvInvoice.Columns["Date"].HeaderText = "Submission Date";
            //    dgvInvoice.Columns["Date"].DefaultCellStyle.Format = "yyyy-MM-dd";

            //    dgvInvoice.Columns["Account"].HeaderText = "Account";
            //    dgvInvoice.Columns["Category"].HeaderText = "Category";
            //    //dgvInvoice.Columns["Cost Center"].HeaderText = "Cost Center";

            //    //dgvInvoice.Columns["Amount"].DefaultCellStyle.Format = "N2";
            //    //dgvInvoice.Columns["VAT"].DefaultCellStyle.Format = "N2";
            //    dgvInvoice.Columns["Total With VAT"].DefaultCellStyle.Format = "N2";
            //    dgvInvoiceCash.Columns["Total With VAT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //    dgvInvoice.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            //    dgvInvoice.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            //    dgvInvoice.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
            //    dgvInvoice.DefaultCellStyle.Font = new Font("Times New Roman", 9);
            //    dgvInvoice.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //   // LocalizationManager.LocalizeDataGridViewHeaders(dgvInvoice);
            //}
            //else
            //{
            dgvInvoice.Rows.Clear();
            dgvInvoice.Columns.Clear();

            // Define columns
            dgvInvoice.Columns.Add("id", "Id");
            dgvInvoice.Columns["id"].Visible = false;

            dgvInvoice.Columns.Add("SN", "SN#");
            dgvInvoice.Columns.Add("Date", "Date");
            dgvInvoice.Columns.Add("No", "Voucher No");
            dgvInvoice.Columns.Add("Type", "Type");
            dgvInvoice.Columns.Add("InvoiceId", "Inv Id");
            dgvInvoice.Columns.Add("Description", "Description");
            dgvInvoice.Columns.Add("Debit", "Debit");
            dgvInvoice.Columns.Add("Credit", "Credit");
            dgvInvoice.Columns.Add("Balance", "Balance");

            // Format numeric columns
            dgvInvoice.Columns["Debit"].DefaultCellStyle.Format = "N2";
            dgvInvoice.Columns["Debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoice.Columns["Credit"].DefaultCellStyle.Format = "N2";
            dgvInvoice.Columns["Credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoice.Columns["Balance"].DefaultCellStyle.Format = "N2";
            dgvInvoice.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Query
            string query = @"
                                SELECT 
                                    t.id,
                                    t.`Date`,
                                    CAST(t.voucher_no AS CHAR CHARACTER SET utf8mb4) AS `No`,
                                    CAST(t.type AS CHAR CHARACTER SET utf8mb4) AS `Type`,
                                    t.transaction_id AS `Invoice Id`,
                                    CAST(coa.name AS CHAR CHARACTER SET utf8mb4) AS `Description`,
                                    t.debit AS `Debit`,
                                    t.credit AS `Credit`
                                FROM 
                                    tbl_transaction t
                                INNER JOIN 
                                    tbl_coa_level_4 coa ON t.account_id = coa.id
                                WHERE 
                                    t.transaction_id IN (
                                        SELECT pt.id 
                                        FROM tbl_employee e
                                        INNER JOIN tbl_petty_cash pt ON pt.employee_id = e.id
                                        WHERE e.code = @empCode
                                    )
                                    AND t.state = 0
                                    AND t.type = 'Petty Cash'
                                ORDER BY t.`Date`, t.id;
                                ";
            if (EnableApproval)
                query = @"
                                SELECT 
                                    t.id,
                                    t.`Date`,
                                    CAST(t.voucher_no AS CHAR CHARACTER SET utf8mb4) AS `No`,
                                    CAST(t.type AS CHAR CHARACTER SET utf8mb4) AS `Type`,
                                    t.transaction_id AS `Invoice Id`,
                                    CAST(coa.name AS CHAR CHARACTER SET utf8mb4) AS `Description`,
                                    t.debit AS `Debit`,
                                    t.credit AS `Credit`
                                FROM 
                                    tbl_transaction t
                                INNER JOIN 
                                    tbl_coa_level_4 coa ON t.account_id = coa.id
                                WHERE 
                                    t.transaction_id IN (
                                        SELECT pt.id 
                                        FROM tbl_employee e
                                        INNER JOIN tbl_petty_cash pt ON pt.employee_id = e.id
                                        WHERE e.code = @empCode
                                    )
                                    AND t.state = 0
                                    AND t.type IN ('Petty Cash','Employee Petty Cash Payment')
                                ORDER BY t.`Date`, t.id;
                                ";

            // Fetch data
            DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@empCode", empCode));

            // Fill rows with running balance
            decimal balance = 0;
            int sn = 1;

            foreach (DataRow row in dt.Rows)
            {
                decimal debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0;
                decimal credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0;
                balance += debit - credit;

                dgvInvoice.Rows.Add(
                    row["id"],
                    sn++,
                    DateTime.Parse(row["Date"].ToString()).ToShortDateString(),
                    string.IsNullOrEmpty(row["No"].ToString()) ? ("PC-00" + row["id"]) : row["No"],
                    row["Type"],
                    row["Invoice Id"] != DBNull.Value ? row["Invoice Id"].ToString() : null,
                    row["Description"],
                    debit,
                    credit,
                    balance
                );
            }

            // Styling
            dgvInvoice.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            dgvInvoice.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            //dgvInvoice.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
            //dgvInvoice.DefaultCellStyle.Font = new Font("Times New Roman", 9);

            dgvInvoice.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ApplyPettyCashLanguage();

            LocalizationManager.LocalizeDataGridViewHeaders(dgvInvoice);
            //}
        }

        private void newTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MasterPettyCashCategory().ShowDialog();
        }
        private void addAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmPettyCashCard().ShowDialog();
        }

        private void addCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MasterPettyCashCategory().ShowDialog();
        }

        private void addRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmPettyCashRequest(0).ShowDialog();
        }

        private void approvalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new MasterPettyCashApproval(0));
        }

        private void submitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmPettyCashSubmission(0));
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            new frmPettyCashCard().ShowDialog();

        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            new frmPettyCashCategory().ShowDialog();
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmPettyCashRequest(0));
        }
        private void dgvInvoice_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cell = dgvInvoice.Rows[e.RowIndex].Cells["InvoiceId"];
                if (cell != null && cell.Value != null)
                {
                    int submissionId = Convert.ToInt32(cell.Value);
                    if (!EnableApproval)
                    {
                        frmMain.openChildForm(new frmPettyCashSubmission(submissionId));
                    }
                    else
                    {
                        frmMain.openChildForm(new frmViewPettyCashVoucher(submissionId));
                    }
                }
                else
                {
                    MessageBox.Show("Invalid or missing ID.");
                }
            }
        }

        private void approvalSubmitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new MasterPettyCashApprovalSubmission(0));
        }

        private void newPettyCashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmViewPettyCashVoucher(0));
        }

        private void editPettyCashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvInvoiceCash.SelectedRows.Count > 0)
            {
                var cell = dgvInvoiceCash.SelectedRows[0].Cells["id"];
                if (cell != null && cell.Value != null)
                {
                    int _invId = Convert.ToInt32(cell.Value);
                    frmMain.openChildForm(new frmViewPettyCashVoucher(_invId));
                }
                else
                {
                    MessageBox.Show("Invalid or missing ID.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row first.");
            }
        }

        private void deletePettyCashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvInvoice.SelectedRows.Count > 0)
            {
                var cell = dgvInvoice.SelectedRows[0].Cells["id"];
                if (cell != null && cell.Value != null)
                {
                    int id = Convert.ToInt32(cell.Value);
                    if (!EnableApproval)
                    {
                        //ClinicDatabase.ExecuteNonQuery("UPDATE tbl_PettyCash_voucher SET state = -1 WHERE id = @id ",
                        //                              ClinicDatabase.CreateParameterNew("id", id.ToString()));
                    }
                    else
                    {
                        // 1. Read all records before deletion
                        DataTable dtVoucher = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_petty_cash WHERE id = @id",
                        ClinicDatabase.CreateParameterNew("id", id.ToString()));

                        DataTable dtDetails = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_petty_cash_details WHERE petty_cash_id = @id",
                            ClinicDatabase.CreateParameterNew("id", id.ToString()));

                        DataTable dtTransaction = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_transaction WHERE transaction_id = @id AND t_type='PettyCash'",
                            ClinicDatabase.CreateParameterNew("id", id.ToString()));

                        // 2. Insert backups into tbl_deleted_records
                        foreach (DataRow row in dtVoucher.Rows)
                        {
                            ClinicDatabase.ExecuteNonQuery(
                                "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                                ClinicDatabase.CreateParameterNew("table", "tbl_petty_cash"),
                                ClinicDatabase.CreateParameterNew("data", Newtonsoft.Json.JsonConvert.SerializeObject(row)),
                                ClinicDatabase.CreateParameterNew("user", "-1")
                            );
                        }

                        foreach (DataRow row in dtDetails.Rows)
                        {
                            ClinicDatabase.ExecuteNonQuery(
                                "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                                ClinicDatabase.CreateParameterNew("table", "tbl_petty_cash_details"),
                                ClinicDatabase.CreateParameterNew("data", Newtonsoft.Json.JsonConvert.SerializeObject(row)),
                                ClinicDatabase.CreateParameterNew("user", "-1")
                            );
                        }

                        foreach (DataRow row in dtTransaction.Rows)
                        {
                            ClinicDatabase.ExecuteNonQuery(
                                "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                                ClinicDatabase.CreateParameterNew("table", "tbl_transaction"),
                                ClinicDatabase.CreateParameterNew("data", Newtonsoft.Json.JsonConvert.SerializeObject(row)),
                                ClinicDatabase.CreateParameterNew("user", "-1")
                            );
                        }

                        // 3. Now delete records permanently
                        CommonInsert.DeleteTransactionEntry(id, "PettyCash");
                        CommonInsert.DeleteCostCenterTransactionEntry(id.ToString(), "PettyCash");
                        ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_petty_cash_details WHERE petty_cash_id = @id",
                            ClinicDatabase.CreateParameterNew("id", id.ToString()));
                        ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_petty_cash WHERE id = @id",
                            ClinicDatabase.CreateParameterNew("id", id.ToString()));

                        ////Utilities.LogAudit(-1, "PettyCash Voucher Permanently Deleted", "PettyCash Voucher", id, "Deleted PettyCash Voucher with ID: " + id);
                    }
                    BindEmployees();
                }
            }
        }

        private void dgvEmployees_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //var cell = dgvEmployees.Rows[e.RowIndex].Cells["id"];
                //if (cell != null && cell.Value != null)
                //{
                //    int eId = Convert.ToInt32(cell.Value);

                //}
                //else
                //{
                //    MessageBox.Show("Invalid or missing ID.");
                //}
            }
        }

        private void deletePCACToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                var cell = dgvEmployees.SelectedRows[0].Cells["id"];
                if (cell != null && cell.Value != null)
                {
                    int eId = Convert.ToInt32(cell.Value);
                    var EmployeeName = dgvEmployees.SelectedRows[0].Cells["Employee Name"].Value.ToString();

                    // 1. Read all records before deletion
                    object getId = ClinicDatabase.ExecuteScalar(
                        "SELECT id FROM tbl_petty_cash_card WHERE name = @id",
                        ClinicDatabase.CreateParameterNew("@id", eId));
                    int pId = (getId == DBNull.Value || getId == null) ? 0 : Convert.ToInt32(getId);
                    if (!EnableApproval)
                    {
                        //ClinicDatabase.ExecuteNonQuery("UPDATE tbl_PettyCash_voucher SET state = -1 WHERE id = @id ",
                        //                              ClinicDatabase.CreateParameterNew("id", pId.ToString()));
                    }
                    else
                    {
                        var result = MessageBox.Show("Are you sure you want to delete this Petty Cash Card?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id FROM tbl_petty_cash WHERE employee_id In (SELECT c.name from tbl_petty_cash_card c WHERE c.id = @id)", ClinicDatabase.CreateParameterNew("id", pId)))
                                if (reader.Read())
                                {
                                    MessageBox.Show("Cannot delete this Petty Cash Card because it is referenced in Petty Cash Vouchers.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_petty_cash_card WHERE id = @id", ClinicDatabase.CreateParameterNew("id", pId));
                            ////Utilities.LogAudit(-1, "Delete Petty Cash Card", "Petty Cash Card", pId, "Deleted Petty Cash Card: " + EmployeeName);
                            //.RefreshEmployee();
                            this.Close();
                        }
                    }
                    BindEmployees();
                }
            }
        }

        private void editPCACToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                var cell = dgvEmployees.SelectedRows[0].Cells["id"];
                if (cell != null && cell.Value != null)
                {
                    int eId = Convert.ToInt32(cell.Value);
                    var EmployeeName = dgvEmployees.SelectedRows[0].Cells["Employee Name"].Value.ToString();
                    // 1. Read all records before deletion
                    object getId = ClinicDatabase.ExecuteScalar(
                        "SELECT id FROM tbl_petty_cash_card WHERE name = @id",
                        ClinicDatabase.CreateParameterNew("@id", eId));
                    int pId = (getId == DBNull.Value || getId == null) ? 0 : Convert.ToInt32(getId);
                    if (!EnableApproval)
                    {
                        //ClinicDatabase.ExecuteNonQuery("UPDATE tbl_PettyCash_voucher SET state = -1 WHERE id = @id ",
                        //                              ClinicDatabase.CreateParameterNew("id", pId.ToString()));
                    }
                    else
                    {
                        new frmPettyCashCard(pId).ShowDialog();
                    }
                }
            }
        }

        private void dgvInvoiceCash_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cell = dgvInvoiceCash.Rows[e.RowIndex].Cells["id"];
                if (cell != null && cell.Value != null)
                {
                    int submissionId = Convert.ToInt32(cell.Value);
                    //if (!EnableApproval)
                    //{
                    //    frmMain.openChildForm(new frmPettyCashSubmission(submissionId));
                    //}
                    //else
                    //{
                    frmMain.openChildForm(new frmViewPettyCashVoucher(submissionId));
                    //}
                }
                else
                {
                    MessageBox.Show("Invalid or missing ID.");
                }
            }
        }

        private void dgvInvoiceCash_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvInvoiceCash.Columns.Contains("ViewDoc"))
            {
                SetViewDocIcons();
            }
        }
        System.Drawing.Image enabledIcon = Clinic.Properties.Resources.doc_icon;   // regular icon
        System.Drawing.Image disabledIcon = null; // or Properties.Resources.doc_icon_disabled

        private void SetViewDocIcons()
        {
            foreach (DataGridViewRow row in dgvInvoiceCash.Rows)
            {
                int purchaseId = Convert.ToInt32(row.Cells["id"].Value);

                bool hasAttachment = AttachmentManager.HasAttachment(purchaseId, "PETTY CASH");
                // You need a function to check if an invoice has attachments
                row.Cells["ViewDoc"].Value = hasAttachment ? enabledIcon : disabledIcon;
            }
        }

        private void AddViewDocImageColumn()
        {
            if (!dgvInvoiceCash.Columns.Contains("ViewDoc"))
            {
                DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
                imgColumn.Name = "ViewDoc";
                imgColumn.HeaderText = "View Doc";
                //imgColumn.Image = Properties.Resources.doc_icon; // <-- your image here
                imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom; // fit nicely
                dgvInvoiceCash.Columns.Add(imgColumn);
            }
        }

        private void dgvInvoiceCash_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvInvoiceCash.Columns[e.ColumnIndex].Name == "ViewDoc")
            {
                var cell = dgvInvoiceCash.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value == null) return; // no attachment, do nothing

                int purchaseId = Convert.ToInt32(dgvInvoiceCash.Rows[e.RowIndex].Cells["id"].Value);
                AttachmentManager.OpenAttachmentByType(purchaseId, "PETTY CASH");
            }
        }

        private void dgvRequest_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cell = dgvRequest.Rows[e.RowIndex].Cells["id"];
                if (cell != null && cell.Value != null)
                {
                    if (dgvRequest.Rows[e.RowIndex].Cells["state"].Value.ToString().Contains("Closed"))
                    {
                        MessageBox.Show("Request already closed");
                    }
                    else
                    {
                        int id = Convert.ToInt32(cell.Value);
                        frmPettyCashRequest frmPettyCash = new frmPettyCashRequest(id);
                        frmPettyCash.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid or missing ID.");
                }
            }
        }
        private void TranslatePettyCashSpecificControls(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                string text = (control.Text ?? "").Trim();

                if (text.Equals("Configuration", StringComparison.OrdinalIgnoreCase))
                    control.Text = arabic ? "الإعدادات" : "Configuration";
                else if (text.Equals("Search By Name, Code", StringComparison.OrdinalIgnoreCase))
                    control.Text = arabic ? "البحث بالاسم أو الرمز" : "Search By Name, Code";

                if (control.HasChildren)
                    TranslatePettyCashSpecificControls(control, arabic);
            }
        }
    }
}