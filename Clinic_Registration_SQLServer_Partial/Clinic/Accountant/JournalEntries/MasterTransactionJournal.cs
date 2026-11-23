using Clinic;
using Clinic_DataAccess;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterTransactionJournal : Form
    {
        private EventHandler JournalVoucherUpdatedHandler;
        string transactionId;
        string type;
        public MasterTransactionJournal() : this("", "")
        {
        }

        public MasterTransactionJournal(string transactionId, string type)
        {
            InitializeComponent();
            // LocalizationManager.LocalizeForm(this);
            JournalVoucherUpdatedHandler = (sender, args) => BindJournal();
            //.Journal += JournalVoucherUpdatedHandler;
            this.type = type ?? "";
            this.transactionId = transactionId ?? "";
            headerUC1.FormText = this.Text;
            ApplyJournalArabicLocalization();
        }

        private void MasterTransactionJournal_Load(object sender, EventArgs e)
        {
            lblJVNO.Text = !string.IsNullOrEmpty(transactionId)
                ? transactionId + " " + (IsArabicJournal() ? TranslateJournalCaption(type) : type)
                : "";
            dtFrom.Enabled = dtTo.Enabled = !chkDate.Checked;
            BindJournal();
            // Reapply after the form's load event in case a custom control
            // restores its Designer caption during initialization.
            ApplyJournalArabicLocalization();
        }

        public void BindJournal()
        {
            DataTable dt;
            string condition = "";

            if (GetJournalSelectionMethod() == "General Ledger")
            {
                condition = " and tbl_transaction.state = 0 and tbl_transaction.hum_id !=0 ";
            }
            else if (GetJournalSelectionMethod() == "Default")
            {
                condition = " and tbl_transaction.state = 0 ";
                //condition = " and tbl_transaction.hum_id =1 ";
            }
            else if (GetJournalSelectionMethod() == "Inventory Opening Stock")
            {
                condition = " and tbl_transaction.state = 0 and tbl_transaction.type ='Opening Balance' ";
            }
            else
            {
                condition = " and tbl_transaction.state = 0 and tbl_transaction.type ='" + GetJournalSelectionMethod().ToString() + "'";
            }
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                ClinicDatabase.CreateParameterNew("id", transactionId)
            };
            if (!chkDate.Checked)
            {
                condition += " and tbl_transaction.date >= @dateFrom and tbl_transaction.date <=@dateTo";
                parameters.Add(ClinicDatabase.CreateParameterNew("dateFrom", dtFrom.Value.Date));
                parameters.Add(ClinicDatabase.CreateParameterNew("dateTo", dtTo.Value.Date));
            }
            if (GetJournalSelectionMethod() == "Sales Invoice")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date AS `Date`, tbl_transaction.transaction_id,
                                tbl_transaction.type `Type`,
                                tbl_coa_level_4.code AS 'A/C Code',
                                tbl_coa_level_4.name AS `A/C NAME`, 
                                tbl_transaction.description AS `Description`,
                                tbl_transaction.debit AS `DEBIT`, 
                                tbl_transaction.credit AS `CREDIT`
                            FROM tbl_transaction 
                            INNER JOIN tbl_coa_level_4 
                            ON tbl_transaction.account_id = tbl_coa_level_4.id where (tbl_transaction.type = 'Sales Invoice Cash' or tbl_transaction.type = 'Sales Invoice') and tbl_transaction.state = 0",
                            parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["A/C name"].Width = 230;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else
                if (GetJournalSelectionMethod() == "Purchase Invoice")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date AS `Date`, tbl_transaction.transaction_id,
                                tbl_transaction.type `Type`,
                                tbl_coa_level_4.code AS 'A/C Code',
                                tbl_coa_level_4.name AS `A/C NAME`, 
                                tbl_transaction.description AS `Description`,
                                tbl_transaction.debit AS `DEBIT`, 
                                tbl_transaction.credit AS `CREDIT`
                            FROM tbl_transaction 
                            INNER JOIN tbl_coa_level_4 
                            ON tbl_transaction.account_id = tbl_coa_level_4.id where (tbl_transaction.type = 'Purchase Invoice Cash' or tbl_transaction.type = 'Purchase Invoice') and tbl_transaction.state = 0",
                            parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["A/C name"].Width = 230;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else
                    if (type.ToLower() == "sales")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date, tbl_transaction.transaction_id,tbl_transaction.type `Type`,  tbl_coa_level_4.code AS 'A/C CODE', 
                                tbl_coa_level_4.name AS 'A/C NAME', tbl_transaction.description AS Description, 
                                tbl_transaction.debit AS DEBIT, tbl_transaction.credit AS CREDIT, tbl_customer.name AS Partner
                                FROM tbl_transaction  INNER JOIN tbl_coa_level_4 ON tbl_transaction.account_id = tbl_coa_level_4.id 
                                INNER JOIN tbl_sales ON tbl_transaction.transaction_id = tbl_sales.id INNER JOIN 
                                tbl_customer ON tbl_sales.customer_id = tbl_customer.id where transaction_id=@id and tbl_transaction.t_type = 'SALES'",
                                parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "sales return")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date, tbl_transaction.transaction_id,tbl_transaction.type `Type`,  tbl_coa_level_4.code AS 'A/C CODE', 
                                tbl_coa_level_4.name AS 'A/C NAME', tbl_transaction.description AS Description, 
                                tbl_transaction.debit AS DEBIT, tbl_transaction.credit AS CREDIT, tbl_customer.name AS Partner
                                FROM tbl_transaction  INNER JOIN tbl_coa_level_4 ON tbl_transaction.account_id = tbl_coa_level_4.id 
                                INNER JOIN tbl_sales_return ON tbl_transaction.transaction_id = tbl_sales_return.id INNER JOIN 
                                tbl_customer ON tbl_sales_return.customer_id = tbl_customer.id where transaction_id=@id and tbl_transaction.t_type = 'SALES RETURN'",
                                parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "purchase")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (
                                ORDER BY tbl_transaction.date) AS `SN`,
                                 LPAD(transaction_id, 4, '0') as `Ref Id`,
                                 tbl_transaction.date, tbl_transaction.transaction_id,tbl_transaction.type `Type`,  tbl_coa_level_4.code AS 'A/C CODE', 
                                 tbl_coa_level_4.name AS 'A/C NAME', tbl_transaction.description AS Description, 
                                 tbl_transaction.debit AS DEBIT, tbl_transaction.credit AS CREDIT, tbl_vendor.name AS Partner
                                FROM tbl_transaction
                                INNER JOIN tbl_coa_level_4 ON tbl_transaction.account_id = tbl_coa_level_4.id
                                INNER JOIN tbl_purchase ON tbl_transaction.transaction_id = tbl_purchase.id
                                INNER JOIN 
                                 tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id
                                WHERE transaction_id= @id AND t_type = 'PURCHASE'",
                                parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "purchase return")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (
                                ORDER BY tbl_transaction.date) AS `SN`,
                                 LPAD(transaction_id, 4, '0') as `Ref Id`,
                                 tbl_transaction.date, tbl_transaction.transaction_id,tbl_transaction.type `Type`,  tbl_coa_level_4.code AS 'A/C CODE', 
                                 tbl_coa_level_4.name AS 'A/C NAME', tbl_transaction.description AS Description, 
                                 tbl_transaction.debit AS DEBIT, tbl_transaction.credit AS CREDIT, tbl_vendor.name AS Partner
                                FROM tbl_transaction
                                INNER JOIN tbl_coa_level_4 ON tbl_transaction.account_id = tbl_coa_level_4.id
                                INNER JOIN tbl_purchase_return ON tbl_transaction.transaction_id = tbl_purchase_return.id
                                INNER JOIN 
                                 tbl_vendor ON tbl_purchase_return.vendor_id = tbl_vendor.id
                                WHERE transaction_id= @id AND t_type = 'PURCHASE RETURN'",
                                parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "payment")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                             ROW_NUMBER() OVER (
                            ORDER BY tbl_transaction.date) AS `SN`,
                                                            LPAD(transaction_id, 4, '0') as `Ref Id`,
                             tbl_transaction.date, tbl_transaction.transaction_id,tbl_transaction.type `Type`,  tbl_coa_level_4.code AS 'A/C CODE', 
                             tbl_coa_level_4.name AS 'A/C NAME', tbl_transaction.description AS Description, 
                             tbl_transaction.debit AS DEBIT, tbl_transaction.credit AS CREDIT, CASE WHEN tbl_payment_voucher.type = 'Vendor' THEN (
                            SELECT name
                            FROM tbl_vendor
                            WHERE id=(
                            SELECT hum_id
                            FROM tbl_payment_voucher_details
                            WHERE payment_id = tbl_payment_voucher.id)) WHEN tbl_payment_voucher.type = 'Employee' THEN (
                            SELECT name
                            FROM tbl_employee
                            WHERE id=(
                            SELECT hum_id
                            FROM tbl_payment_voucher_details
                            WHERE payment_id = tbl_payment_voucher.id)) WHEN tbl_payment_voucher.type = 'General' THEN (
                            SELECT name
                            FROM tbl_coa_level_4
                            WHERE id=(
                            SELECT hum_id
                            FROM tbl_payment_voucher_details
                            WHERE payment_id = tbl_payment_voucher.id)) ELSE '' END Partner
                            FROM tbl_transaction
                            INNER JOIN tbl_coa_level_4 ON tbl_transaction.account_id = tbl_coa_level_4.id
                            INNER JOIN tbl_payment_voucher ON tbl_transaction.transaction_id = tbl_payment_voucher.id
                            WHERE transaction_id= @id AND t_type = 'PAYMENT'",
                                parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "receipt")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (
                                ORDER BY tbl_transaction.date) AS `SN`,
                                                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                 tbl_transaction.date, tbl_transaction.transaction_id,tbl_transaction.type `Type`,  tbl_coa_level_4.code AS 'A/C CODE', 
                                 tbl_coa_level_4.name AS 'A/C NAME', tbl_transaction.description AS Description, 
                                 tbl_transaction.debit AS DEBIT, tbl_transaction.credit AS CREDIT, CASE WHEN tbl_receipt_voucher.type = 'Customer' THEN (
                                SELECT name
                                FROM tbl_customer
                                WHERE id=(
                                SELECT hum_id
                                FROM tbl_receipt_voucher_details
                                WHERE payment_id = tbl_receipt_voucher.id limit 1)) WHEN tbl_receipt_voucher.type = 'General' THEN (
                                SELECT name
                                FROM tbl_coa_level_4
                                WHERE id=(
                                SELECT hum_id
                                FROM tbl_receipt_voucher_details
                                WHERE payment_id = tbl_receipt_voucher.id)) ELSE '' END Partner
                                FROM tbl_transaction
                                INNER JOIN tbl_coa_level_4 ON tbl_transaction.account_id = tbl_coa_level_4.id
                                INNER JOIN tbl_receipt_voucher ON tbl_transaction.transaction_id = tbl_receipt_voucher.id
                                WHERE transaction_id= @id AND t_type = 'RECEIPT'",
                                parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "journal")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (
                                ORDER BY tbl_transaction.date) AS `SN`,
                                                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                 tbl_transaction.date, tbl_transaction.transaction_id,
                                 tbl_transaction.type `Type`, tbl_coa_level_4.code AS 'A/C CODE', 
                                 tbl_coa_level_4.name AS 'A/C NAME', tbl_transaction.description AS Description, 
                                 tbl_transaction.debit AS DEBIT, tbl_transaction.credit AS CREDIT, (SELECT partner FROM tbl_journal_voucher_details
                                WHERE inv_id = tbl_journal_voucher.id LIMIT 1
                                 ) as parent
                                FROM tbl_transaction
                                INNER JOIN tbl_coa_level_4 ON tbl_transaction.account_id = tbl_coa_level_4.id
                                INNER JOIN tbl_journal_voucher ON tbl_transaction.transaction_id = tbl_journal_voucher.id
                                WHERE transaction_id= @id AND t_type = 'JOURNAL'",
                                parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit, null);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "inventory")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date AS `Date`, tbl_transaction.transaction_id,
                                tbl_transaction.type `Type`,
                                tbl_coa_level_4.code AS 'A/C Code',
                                tbl_coa_level_4.name AS `A/C NAME`, 
                                tbl_transaction.description AS `Description`,
                                tbl_transaction.debit AS `DEBIT`, 
                                tbl_transaction.credit AS `CREDIT`
                            FROM tbl_transaction 
                            INNER JOIN tbl_coa_level_4 
                            ON tbl_transaction.account_id = tbl_coa_level_4.id where description 
                                like '%- Item Code - " + transactionId + "'",
                             parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["A/C name"].Width = 230;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else if (type.ToLower() == "vat input" || type.ToLower() == "vat output")
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date AS `Date`, tbl_transaction.transaction_id,
                                tbl_transaction.type `Type`,
                                tbl_coa_level_4.code AS 'A/C Code',
                                tbl_coa_level_4.name AS `A/C NAME`, 
                                tbl_transaction.description AS `Description`,
                                tbl_transaction.debit AS `DEBIT`, 
                                tbl_transaction.credit AS `CREDIT`
                            FROM tbl_transaction 
                            INNER JOIN tbl_coa_level_4 
                            ON tbl_transaction.account_id = tbl_coa_level_4.id " + condition + @" and tbl_transaction.account_id = (SELECT account_id FROM tbl_coa_config WHERE category = '" + type + "')",
                            parameters.ToArray());
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
                try
                {
                    dgvJournal.Columns["A/C name"].Width = 230;
                    dgvJournal.Columns["sn"].Width = 50;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            else if (new[] { "Sales All", "Purchase All", "Purchase Return All", "Sales Return All", "Debit Note All", "Credit Note All", "Petty Cash All" }.Contains(type, StringComparer.OrdinalIgnoreCase))
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date AS `Date`, tbl_transaction.transaction_id,
                                tbl_transaction.type `Type`,
                                tbl_coa_level_4.code AS 'A/C Code',
                                tbl_coa_level_4.name AS `A/C NAME`, 
                                tbl_transaction.description AS `Description`,
                                tbl_transaction.debit AS `DEBIT`, 
                                tbl_transaction.credit AS `CREDIT`
                            FROM tbl_transaction 
                            INNER JOIN tbl_coa_level_4 
                            ON tbl_transaction.account_id = tbl_coa_level_4.id " + condition + @" and tbl_transaction.type Like '" + type.Replace(" All", "") + "%'",
                            parameters.ToArray());


                if (dt == null)
                {
                    dgvJournal.DataSource = null;
                    return;
                }
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
                try
                {
                    dgvJournal.Columns["A/C name"].Width = 230;
                    dgvJournal.Columns["sn"].Width = 50;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            else
            {
                dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                                 ROW_NUMBER() OVER (ORDER BY tbl_transaction.date) AS `SN`,
                                LPAD(transaction_id, 4, '0') as `Ref Id`,
                                tbl_transaction.date AS `Date`, tbl_transaction.transaction_id,
                                tbl_transaction.type `Type`,
                                tbl_coa_level_4.code AS 'A/C Code',
                                tbl_coa_level_4.name AS `A/C NAME`, 
                                tbl_transaction.description AS `Description`,
                                tbl_transaction.debit AS `DEBIT`, 
                                tbl_transaction.credit AS `CREDIT`
                            FROM tbl_transaction 
                            INNER JOIN tbl_coa_level_4 
                            ON tbl_transaction.account_id = tbl_coa_level_4.id " + condition,
                            parameters.ToArray());
                if (dt == null)
                {
                    dgvJournal.DataSource = null;
                    return;
                }
                decimal totalDebit = dt.AsEnumerable().Sum(row => row.Field<decimal>("debit"));
                decimal totalCredit = dt.AsEnumerable().Sum(row => row.Field<decimal>("credit"));

                dt.Rows.Add(null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
                try
                {
                    dgvJournal.Columns["A/C name"].Width = 230;
                    dgvJournal.Columns["sn"].Width = 50;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            dgvJournal.CellPainting += new DataGridViewCellPaintingEventHandler(dgvJournal_CellPainting);

            // LocalizationManager.LocalizeDataGridViewHeaders(dgvJournal);
            ApplyJournalGridArabicHeaders();
        }

        private bool IsArabicJournal()
        {
            return TranslationClass.IsArabic
                || CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                || CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar"
                || this.RightToLeft == RightToLeft.Yes;
        }

        private string TranslateJournalCaption(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            switch (value.Trim())
            {
                case "Master Transaction Journal": return "دفتر يومية المعاملات";
                case "Transaction Journal": return "دفتر يومية المعاملات";
                case "Journal Transactions": return "معاملات اليومية";
                case "All Transactions": return "جميع المعاملات";
                case "Date:": return "التاريخ:";
                case "Export": return "تصدير";
                case "Excel Files (*.xlsx)|*.xlsx": return "ملفات Excel (*.xlsx)|*.xlsx";
                case "Save Journal Transactions": return "حفظ معاملات اليومية";
                case "Save Excel File": return "حفظ ملف Excel";
                case "Excel exported successfully!": return "تم تصدير ملف Excel بنجاح!";
                case "General Ledger": return "دفتر الأستاذ العام";
                case "Default": return "افتراضي";
                case "Inventory Opening Stock": return "رصيد افتتاحي للمخزون";
                case "Sales Invoice": return "فاتورة مبيعات";
                case "Purchase Invoice": return "فاتورة مشتريات";
                case "Sales": return "مبيعات";
                case "Sales Return": return "مرتجع مبيعات";
                case "Purchase": return "مشتريات";
                case "Purchase Return": return "مرتجع مشتريات";
                case "Payment": return "سند دفع";
                case "Receipt": return "سند قبض";
                case "Journal": return "قيد يومية";
                case "Inventory": return "المخزون";
                case "VAT Input": return "ضريبة القيمة المضافة - مدخلات";
                case "VAT Output": return "ضريبة القيمة المضافة - مخرجات";
                case "Sales All": return "جميع المبيعات";
                case "Purchase All": return "جميع المشتريات";
                case "Purchase Return All": return "جميع مرتجعات المشتريات";
                case "Sales Return All": return "جميع مرتجعات المبيعات";
                case "Debit Note All": return "جميع إشعارات الخصم";
                case "Credit Note All": return "جميع إشعارات الدائن";
                case "Petty Cash All": return "جميع المصروفات النثرية";
                case "From": return "من";
                case "To": return "إلى";
                case "All": return "الكل";
                case "Selection Method": return "طريقة الاختيار";
                case "PDF": return "بي دي إف";
                case "Excel": return "إكسل";
                case "Date": return "التاريخ";
                case "Type": return "النوع";
                case "Description": return "الوصف";
                case "Debit": return "مدين";
                case "DEBIT": return "مدين";
                case "Credit": return "دائن";
                case "CREDIT": return "دائن";
                case "A/C Code": return "رمز الحساب";
                case "A/C CODE": return "رمز الحساب";
                case "A/C NAME": return "اسم الحساب";
                case "A/C Name": return "اسم الحساب";
                case "Ref Id": return "رقم المرجع";
                case "SN": return "م";
                case "Partner": return "الطرف";
                case "parent": return "الطرف";
                case "TOTAL": return "الإجمالي";
                case "Print": return "طباعة";
                case "PDF Files (*.pdf)|*.pdf": return "ملفات PDF (*.pdf)|*.pdf";
                default: return value;
            }
        }

        private string GetJournalSelectionMethod()
        {
            string selected = cmbSelectionMethod.Text ?? "";
            if (!IsArabicJournal())
                return selected;

            switch (selected.Trim())
            {
                case "دفتر الأستاذ العام": return "General Ledger";
                case "افتراضي": return "Default";
                case "رصيد افتتاحي للمخزون": return "Inventory Opening Stock";
                case "فاتورة مبيعات": return "Sales Invoice";
                case "فاتورة مشتريات": return "Purchase Invoice";
                case "مبيعات": return "Sales";
                case "مرتجع مبيعات": return "Sales Return";
                case "مشتريات": return "Purchase";
                case "مرتجع مشتريات": return "Purchase Return";
                case "سند دفع": return "Payment";
                case "سند قبض": return "Receipt";
                case "قيد يومية": return "Journal";
                case "المخزون": return "Inventory";
                case "ضريبة القيمة المضافة - مدخلات": return "VAT Input";
                case "ضريبة القيمة المضافة - مخرجات": return "VAT Output";
                case "جميع المبيعات": return "Sales All";
                case "جميع المشتريات": return "Purchase All";
                case "جميع مرتجعات المشتريات": return "Purchase Return All";
                case "جميع مرتجعات المبيعات": return "Sales Return All";
                case "جميع إشعارات الخصم": return "Debit Note All";
                case "جميع إشعارات الدائن": return "Credit Note All";
                case "جميع المصروفات النثرية": return "Petty Cash All";
                default: return selected;
            }
        }

        private void ApplyJournalArabicLocalization()
        {
            if (!IsArabicJournal())
                return;

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = TranslateJournalCaption(this.Text);
            headerUC1.FormText = "دفتر يومية المعاملات";

            TranslateJournalControls(this);
            ApplyJournalDirectCaptions();

            // Translate the selection list for display. SQL logic converts the
            // selected Arabic caption back to its original English key.
            if (cmbSelectionMethod.DataSource == null)
            {
                for (int i = 0; i < cmbSelectionMethod.Items.Count; i++)
                {
                    string itemText = Convert.ToString(cmbSelectionMethod.Items[i]);
                    cmbSelectionMethod.Items[i] = TranslateJournalCaption(itemText);
                }
            }

            ApplyJournalGridArabicHeaders();
        }

        private void TranslateJournalControls(Control parent)
        {
            if (parent == null)
                return;

            // Guna controls do not always inherit from ButtonBase/Label,
            // so include their caption-control types explicitly. Do not
            // translate TextBox, ComboBox, DateTimePicker or grid data.
            string controlType = parent.GetType().Name;
            bool isCaptionControl =
                parent is Label ||
                parent is LinkLabel ||
                parent is ButtonBase ||
                parent is CheckBox ||
                parent is RadioButton ||
                parent is GroupBox ||
                parent is TabPage ||
                controlType.IndexOf("Button", StringComparison.OrdinalIgnoreCase) >= 0 ||
                controlType.IndexOf("Label", StringComparison.OrdinalIgnoreCase) >= 0 ||
                controlType.IndexOf("GroupBox", StringComparison.OrdinalIgnoreCase) >= 0 ||
                controlType.IndexOf("CheckBox", StringComparison.OrdinalIgnoreCase) >= 0 ||
                controlType.IndexOf("RadioButton", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isCaptionControl)
            {
                string translated = TranslateJournalCaption(parent.Text);
                if (!string.IsNullOrEmpty(translated))
                    parent.Text = translated;
                parent.RightToLeft = RightToLeft.Yes;
            }

            foreach (Control child in parent.Controls)
                TranslateJournalControls(child);

            if (parent is ToolStrip toolStrip)
                TranslateJournalMenuItems(toolStrip.Items);

            if (parent is MenuStrip menuStrip)
                TranslateJournalMenuItems(menuStrip.Items);

            if (parent.ContextMenuStrip != null)
                TranslateJournalMenuItems(parent.ContextMenuStrip.Items);
        }

        private void TranslateJournalMenuItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.Text = TranslateJournalCaption(item.Text);
                if (item is ToolStripDropDownItem dropDown && dropDown.DropDownItems.Count > 0)
                    TranslateJournalMenuItems(dropDown.DropDownItems);
            }
        }

        // Apply the three captions shown in the toolbar/filter row directly.
        // These controls can be custom Guna controls or hosted designer fields,
        // so do not rely only on Label/Button inheritance or the parent tree.
        private void ApplyJournalDirectCaptions()
        {
            // First walk the complete control tree and translate only these exact
            // captions. This is safe for date, numeric and user-entered controls.
            ApplyJournalDirectCaptionsRecursive(this);

            // Also inspect designer fields in case a custom/hosted control is not
            // attached to the expected parent Controls collection.
            System.Reflection.FieldInfo[] fields = GetType().GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            foreach (System.Reflection.FieldInfo field in fields)
            {
                object value = null;
                try { value = field.GetValue(this); }
                catch { continue; }

                if (value is Control control)
                    TranslateJournalDirectCaption(control);
                else if (value is ToolStripItem toolStripItem)
                    TranslateJournalDirectToolStripItem(toolStripItem);
                else if (value is ToolStrip toolStrip)
                    TranslateJournalDirectToolStripItems(toolStrip.Items);
            }
        }

        private void ApplyJournalDirectCaptionsRecursive(Control parent)
        {
            if (parent == null)
                return;

            TranslateJournalDirectCaption(parent);

            foreach (Control child in parent.Controls)
                ApplyJournalDirectCaptionsRecursive(child);

            if (parent is ToolStrip toolStrip)
                TranslateJournalDirectToolStripItems(toolStrip.Items);

            if (parent.ContextMenuStrip != null)
                TranslateJournalDirectToolStripItems(parent.ContextMenuStrip.Items);
        }

        private void TranslateJournalDirectCaption(Control control)
        {
            if (control == null || string.IsNullOrWhiteSpace(control.Text))
                return;

            string caption = control.Text.Trim();
            if (caption.Equals("PDF", StringComparison.OrdinalIgnoreCase) ||
                caption == "بي دي إف")
            {
                control.Text = "بي دي إف";
                control.RightToLeft = RightToLeft.Yes;
            }
            else if (caption.Equals("Excel", StringComparison.OrdinalIgnoreCase) ||
                     caption == "إكسل")
            {
                control.Text = "إكسل";
                control.RightToLeft = RightToLeft.Yes;
            }
            else if (caption.Equals("Selection Method", StringComparison.OrdinalIgnoreCase) ||
                     caption == "طريقة الاختيار")
            {
                control.Text = "طريقة الاختيار";
                control.RightToLeft = RightToLeft.Yes;
            }
        }

        private void TranslateJournalDirectToolStripItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
                TranslateJournalDirectToolStripItem(item);
        }

        private void TranslateJournalDirectToolStripItem(ToolStripItem item)
        {
            if (item == null)
                return;

            string caption = (item.Text ?? string.Empty).Trim();
            if (caption.Equals("PDF", StringComparison.OrdinalIgnoreCase))
                item.Text = "بي دي إف";
            else if (caption.Equals("Excel", StringComparison.OrdinalIgnoreCase))
                item.Text = "إكسل";
            else if (caption.Equals("Selection Method", StringComparison.OrdinalIgnoreCase))
                item.Text = "طريقة الاختيار";

            if (item is ToolStripDropDownItem dropDown && dropDown.DropDownItems.Count > 0)
                TranslateJournalDirectToolStripItems(dropDown.DropDownItems);
        }

        private void ApplyJournalGridArabicHeaders()
        {
            if (!IsArabicJournal() || dgvJournal == null)
                return;

            dgvJournal.RightToLeft = RightToLeft.Yes;
            foreach (DataGridViewColumn column in dgvJournal.Columns)
                column.HeaderText = TranslateJournalCaption(column.HeaderText);
        }

        private void dgvJournal_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }
        private void dgvJournal_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvJournal.Rows.Count)
                return;

            DataGridViewRow row = dgvJournal.Rows[e.RowIndex];
            if (row.IsNewRow || !dgvJournal.Columns.Contains("transaction_id") ||
                !dgvJournal.Columns.Contains("Type") ||
                !dgvJournal.Columns.Contains("Ref Id") ||
                !dgvJournal.Columns.Contains("Date"))
                return;

            object idValue = row.Cells["transaction_id"].Value;
            if (idValue == null || idValue == DBNull.Value ||
                !int.TryParse(idValue.ToString(), out int selectedTransactionId))
                return; // Ignore the TOTAL row and incomplete rows.

            frmMain.openChildForm(new frmTransactionJournal(
                selectedTransactionId,
                Convert.ToString(row.Cells["Type"].Value) ?? "",
                Convert.ToString(row.Cells["Ref Id"].Value) ?? "",
                Convert.ToString(row.Cells["Date"].Value) ?? ""));
        }

        private void cmbSelectionMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindJournal();
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void dgvJournal_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvJournal_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            dtFrom.Enabled = dtTo.Enabled = !chkDate.Checked;
            BindJournal();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            BindJournal();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            BindJournal();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            bool arabic = IsArabicJournal();
            string companyName = arabic ? "معاملات اليومية" : "Journal Transactions";

            // Create PDF document
            Document doc = new Document();
            Section section = doc.AddSection();

            // Set margins
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);

            // Header table
            Table headerTable = section.AddTable();
            headerTable.Borders.Width = 0;
            headerTable.AddColumn("5cm");
            headerTable.AddColumn("8cm");
            headerTable.AddColumn("5cm");

            Row headerRow = headerTable.AddRow();
            Paragraph left = headerRow.Cells[0].AddParagraph();
            left.Format.Font.Name = "Times New Roman";
            left.Format.Font.Size = 10;
            left.Format.Alignment = ParagraphAlignment.Left;
            left.AddText(DateTime.Now.ToString("hh:mm tt") + "\n");
            left.AddText(DateTime.Now.ToString("dd/MM/yyyy"));

            Paragraph center = headerRow.Cells[1].AddParagraph();
            center.Format.Font.Name = "Times New Roman";
            center.Format.Alignment = ParagraphAlignment.Center;
            center.AddFormattedText(companyName.ToUpper() + "\n", TextFormat.Bold).Font.Size = 12;
            center.AddFormattedText((arabic ? "معاملات اليومية" : "Journal Transactions") + "\n", TextFormat.Bold).Font.Size = 12;
            center.AddFormattedText(arabic ? "جميع المعاملات" : "All Transactions", TextFormat.NotBold).Font.Size = 9;

            //section.Add(headerTable);

            // Separator line
            Paragraph line = section.AddParagraph();
            line.Format.Borders.Bottom.Width = 2;
            line.Format.SpaceAfter = "0.5cm";

            // Main data table
            Table dataTable = section.AddTable();
            dataTable.Borders.Width = 0.75;
            dataTable.AddColumn("1cm"); // SN
            dataTable.AddColumn("2cm"); // Ref Id
            dataTable.AddColumn("2cm"); // Date
            dataTable.AddColumn("2cm"); // Type
            dataTable.AddColumn("2cm"); // A/C Code
            dataTable.AddColumn("3cm"); // A/C Name
            dataTable.AddColumn("3cm"); // Description
            dataTable.AddColumn("2cm"); // Debit
            dataTable.AddColumn("2cm"); // Credit

            dataTable.AddColumn(
                Unit.FromCentimeter(2)); // Partner
            dataTable.AddColumn(
                Unit.FromCentimeter(2)); // Total
            Row header = dataTable.AddRow();
            header.Shading.Color = Colors.LightGray;
            string[] pdfHeaders = arabic
                ? new[] { "م", "رقم المرجع", "التاريخ", "النوع", "رمز الحساب", "اسم الحساب", "الوصف", "مدين", "دائن" }
                : new[] { "SN", "Ref Id", "Date", "Type", "A/C Code", "A/C Name", "Description", "Debit", "Credit" };
            for (int i = 0; i < pdfHeaders.Length; i++)
                header.Cells[i].AddParagraph(pdfHeaders[i]).Format.Font.Bold = true;

            foreach (DataGridViewRow row in dgvJournal.Rows)
            {
                if (row.IsNewRow) continue;
                Row dataRow = dataTable.AddRow();
                dataRow.Cells[0].AddParagraph(row.Cells["SN"].Value?.ToString() ?? "");
                dataRow.Cells[1].AddParagraph(row.Cells["Ref Id"].Value?.ToString() ?? "");
                dataRow.Cells[2].AddParagraph(row.Cells["Date"].Value?.ToString() ?? "");
                dataRow.Cells[3].AddParagraph(row.Cells["Type"].Value?.ToString() ?? "");
                dataRow.Cells[4].AddParagraph(row.Cells["A/C Code"].Value?.ToString() ?? "");
                dataRow.Cells[5].AddParagraph(row.Cells["A/C Name"].Value?.ToString() ?? "");
                dataRow.Cells[6].AddParagraph(row.Cells["Description"].Value?.ToString() ?? "");
                dataRow.Cells[7].AddParagraph(row.Cells["Debit"].Value?.ToString() ?? "").Format.Alignment = ParagraphAlignment.Right;
                dataRow.Cells[8].AddParagraph(row.Cells["Credit"].Value?.ToString() ?? "").Format.Alignment = ParagraphAlignment.Right;
            }

            // Ask user to save file
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = arabic
                    ? "ملفات PDF (*.pdf)|*.pdf"
                    : "PDF Files (*.pdf)|*.pdf";
                saveDialog.Title = arabic
                    ? "حفظ معاملات اليومية"
                    : "Save Journal Transactions";
                saveDialog.FileName = "JournalTransactions.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                    renderer.Document = doc;
                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(saveDialog.FileName);
                    Process.Start("explorer.exe", saveDialog.FileName);
                }
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            bool arabic = IsArabicJournal();
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = arabic
                    ? "ملفات Excel (*.xlsx)|*.xlsx"
                    : "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = arabic
                    ? "حفظ ملف Excel"
                    : "Save Excel File";
                saveDialog.FileName = "JournalTransactions.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = arabic ? "معاملات اليومية" : "Journal Transactions";

                    // Header row (Date)
                    Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range["A1", "C1"];
                    headerRange.Merge();
                    headerRange.Value = (arabic ? "التاريخ: " : "Date: ") + DateTime.Now.ToString("MMM dd, yyyy");
                    headerRange.Font.Bold = true;
                    headerRange.Font.Name = "Times New Roman";
                    headerRange.Font.Size = 10;
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    // Column headers
                    string[] columnHeaders = arabic
                        ? new[] { "م", "رقم المرجع", "التاريخ", "النوع", "رمز الحساب", "اسم الحساب", "الوصف", "مدين", "دائن" }
                        : new[] { "SN", "Ref Id", "Date", "Type", "A/C Code", "A/C Name", "Description", "Debit", "Credit" };
                    for (int i = 0; i < columnHeaders.Length; i++)
                    {
                        worksheet.Cells[2, i + 1] = columnHeaders[i];
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, i + 1]).Font.Bold = true;
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, i + 1]).Font.Name = "Times New Roman";
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, i + 1]).Font.Size = 10;
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, i + 1]).Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    }

                    int rowIndex = 3;

                    foreach (DataGridViewRow row in dgvJournal.Rows)
                    {
                        if (row.IsNewRow) continue;

                        worksheet.Cells[rowIndex, 1] = row.Cells["SN"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 2] = row.Cells["Ref Id"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 3] = row.Cells["Date"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 4] = row.Cells["Type"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 5] = row.Cells["A/C Code"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 6] = row.Cells["A/C Name"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 7] = row.Cells["Description"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 8] = row.Cells["Debit"].Value?.ToString() ?? "";
                        worksheet.Cells[rowIndex, 9] = row.Cells["Credit"].Value?.ToString() ?? "";
                        rowIndex++;
                    }

                    worksheet.Columns.AutoFit();

                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close(false);
                    excelApp.Quit();

                    MessageBox.Show(
                        arabic ? "تم تصدير ملف Excel بنجاح!" : "Excel exported successfully!",
                        arabic ? "تصدير" : "Export",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
    }
}
