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
        public MasterTransactionJournal(string transactionId, string type)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            JournalVoucherUpdatedHandler = (sender, args) => BindJournal();
            //.Journal += JournalVoucherUpdatedHandler;
            this.type = type;
            this.transactionId = transactionId;
            headerUC1.FormText = this.Text;
        }

        private void MasterTransactionJournal_Load(object sender, EventArgs e)
        {
            lblJVNO.Text = !string.IsNullOrEmpty(transactionId) ? transactionId + " " + type : "";
            dtFrom.Enabled = dtTo.Enabled = !chkDate.Checked;
            BindJournal();
        }

        public void BindJournal()
        {
            DataTable dt;
            string condition = "";

            if (cmbSelectionMethod.Text == "General Ledger")
            {
                condition = " and tbl_transaction.state = 0 and tbl_transaction.hum_id !=0 ";
            }
            else if (cmbSelectionMethod.Text == "Default")
            {
                condition = " and tbl_transaction.state = 0 ";
                //condition = " and tbl_transaction.hum_id =1 ";
            }
            else if (cmbSelectionMethod.Text == "Inventory Opening Stock")
            {
                condition = " and tbl_transaction.state = 0 and tbl_transaction.type ='Opening Balance' ";
            }
            else
            {
                condition = " and tbl_transaction.state = 0 and tbl_transaction.type ='" + cmbSelectionMethod.Text.ToString() + "'";
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
            if (cmbSelectionMethod.Text == "Sales Invoice")
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

                dt.Rows.Add( null, null, null, null, null, null, null, "TOTAL", totalDebit, totalCredit);
                dgvJournal.DataSource = dt;
                dgvJournal.Columns["A/C name"].Width = 230;
                dgvJournal.Columns["sn"].Width = 50;
                dgvJournal.Columns["A/C NAME"].AutoSizeMode = dgvJournal.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvJournal.Columns["transaction_id"].Visible = dgvJournal.Columns["Description"].Visible = false;
            }
            else
            if (cmbSelectionMethod.Text == "Purchase Invoice")
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
        }

        private void dgvJournal_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }
        private void dgvJournal_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvJournal.Rows.Count == 0)
                return;

            frmMain.openChildForm(new frmTransactionJournal(int.Parse(dgvJournal.SelectedRows[0].Cells["transaction_id"].Value.ToString()), dgvJournal.SelectedRows[0].Cells["type"].Value.ToString(), dgvJournal.SelectedRows[0].Cells["Ref Id"].Value.ToString(), dgvJournal.SelectedRows[0].Cells["Date"].Value.ToString()));
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
            string companyName = "Journal Transactions";

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
            center.AddFormattedText("Journal Transactions\n", TextFormat.Bold).Font.Size = 12;
            center.AddFormattedText("All Transactions", TextFormat.NotBold).Font.Size = 9;

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
            header.Cells[0].AddParagraph("SN").Format.Font.Bold = true;
            header.Cells[1].AddParagraph("Ref Id").Format.Font.Bold = true;
            header.Cells[2].AddParagraph("Date").Format.Font.Bold = true;
            header.Cells[3].AddParagraph("Type").Format.Font.Bold = true;
            header.Cells[4].AddParagraph("A/C Code").Format.Font.Bold = true;
            header.Cells[5].AddParagraph("A/C Name").Format.Font.Bold = true;
            header.Cells[6].AddParagraph("Description").Format.Font.Bold = true;
            header.Cells[7].AddParagraph("Debit").Format.Font.Bold = true;
            header.Cells[8].AddParagraph("Credit").Format.Font.Bold = true;

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
                saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveDialog.Title = "Save Journal Transactions";
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
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.Title = "Save Excel File";
                saveDialog.FileName = "JournalTransactions.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = false;
                    var workbook = excelApp.Workbooks.Add(Type.Missing);
                    var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                    worksheet.Name = "Journal Transactions";

                    // Header row (Date)
                    Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range["A1", "C1"];
                    headerRange.Merge();
                    headerRange.Value = "Date: " + DateTime.Now.ToString("MMM dd, yyyy");
                    headerRange.Font.Bold = true;
                    headerRange.Font.Name = "Times New Roman";
                    headerRange.Font.Size = 10;
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    // Column headers
                    string[] columnHeaders = { "SN", "Ref Id", "Date", "Type", "A/C Code", "A/C Name", "Description", "Debit", "Credit" };
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

                    MessageBox.Show("Excel exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
