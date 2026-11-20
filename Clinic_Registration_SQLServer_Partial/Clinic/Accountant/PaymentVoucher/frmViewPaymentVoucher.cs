using Clinic.Login;
using Clinic_DataAccess;
//using CrystalDecisions.CrystalReports.Engine;
using Guna.UI2.WinForms;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;
using static BindCombos;
//using static YamyProject.BindCombos;

namespace YamyProject
{
    public partial class frmViewPaymentVoucher : Form
    {
        int id, defaultCustomerId = 0;
        decimal totalAmount = 0;
        string code;
        bool isSubContractors;
        string VendorType = "Vendor";

        public frmViewPaymentVoucher(int id = 0, int _customerId = 0, bool _isSubContractors = false)
        {
            InitializeComponent();
            tabControl2.TabPages.Remove(tabPage4);
            LocalizationManager.LocalizeForm(this);
            this.id = id;
            headerUC1.FormText = this.Text;

            this.defaultCustomerId = _customerId;
            this.isSubContractors = _isSubContractors;
            CbSubcontractors.Checked = _isSubContractors;
            VendorType = isSubContractors ? "Subcontractor" : "Vendor";
      
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ApplyPaymentVoucherArabic()
        {
            if (!Clinic.TranslationClass.IsArabic)
                return;

            // Form header
            this.Text = "سند صرف";
            headerUC1.FormText = "سند صرف";


            // Toolbar buttons
            BtnClear.Text = "جديد";
            BtnSaveNew.Text = "حفظ";
            BtnDelete.Text = "حذف";
            BtnCopy.Text = "إنشاء نسخة";
            BtnPrint.Text = "طباعة";
            BtnEmail.Text = "بريد إلكتروني";
            BtnAttach.Text = "إرفاق ملف";

            guna2TileButton19.Text = "حفظ كقالب";
            guna2TileButton15.Text = "إضافة وقت / تكاليف";

            chkPrint.Text = "الطباعة بعد الحفظ";
            // Main tab
            tabPage3.Text = "الرئيسية";

            // Bottom buttons
            btnSave.Text = "حفظ";
            btnClose.Text = "إغلاق";
            guna2Button3.Text = "حفظ وإغلاق";

            // Main information
            label2.Text = "التاريخ";
            label1.Text = "نوع الدفع";
            lblBalance.Text = "طريقة الدفع";

            CbSubcontractors.Text = "مقاولو الباطن";
            cbpettycash.Text = "العهدة النقدية";

            // Amount placeholders
            txtAmount.PlaceholderText = "المبلغ";
            txtAmountInWord.PlaceholderText = "المبلغ كتابةً";

            // Debit section
            guna2GroupBox2.Text = "الدفع إلى - مدين";

            label9.Text = "رمز الحساب";
            label4.Text = "اسم الحساب";
            lblCostcenter.Text = "مركز التكلفة";
            label3.Text = "ملاحظات";

            // Vendor details
            lblCode.Text = "رمز المورد";
            lblCustomer.Text = "اسم المورد";
            txtVendor.PlaceholderText = "رمز المورد";

            // Credit section
            guna2GroupBox3.Text = "الدفع من - دائن";

            label10.Text = "رمز الحساب";
            label6.Text = "اسم الحساب";
            label7.Text = "مركز التكلفة";

            // Bank information
            lblBankName.Text = "اسم البنك";
            lblBankAccountName.Text = "اسم الحساب البنكي";
            lblBookNo.Text = "رقم دفتر الشيكات";

            label12.Text = "تاريخ الشيك";
            label5.Text = "اسم المستفيد";
            label8.Text = "رقم الشيك";

            txtBankCode.PlaceholderText = "رمز البنك";
            txtCheckName.PlaceholderText = "اسم المستفيد";

            // Transfer information
            lblTransDate.Text = "تاريخ التحويل";
            lblTransName.Text = "اسم المحول إليه";
            lblTransRef.Text = "مرجع التحويل";

            txtTransName.PlaceholderText = "اسم المحول إليه";
            txtTransRef.PlaceholderText = "مرجع التحويل";

            // Grid header translations
            no.HeaderText = "الرقم";
            invDate.HeaderText = "التاريخ";
            InvNo.HeaderText = "رقم الفاتورة";
            Total.HeaderText = "المبلغ";
            chkPay.HeaderText = "دفع";
            Pay.HeaderText = "المبلغ المدفوع";
            Description.HeaderText = "البيان";
            voucherType.HeaderText = "نوع السند";

            // Grid alignment
            dgvInv.RightToLeft = RightToLeft.Yes;

            dgvInv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
        }
        private void frmViewPaymentVoucher_Load(object sender, EventArgs e)
        {
            dtOpen.Value = dtpTransDate.Value =
                dt_check_date.Value = DateTime.Now.Date;

            txtPVCode.Text = GenerateNextPaymentCode();
            txtId.Text = GenerateNextPaymentId();

            this.Invoke((Action)(() =>
            {
                BindCombos.PopulateListEmployeesNew(lstOfEmp, false);
            }));

            // Keep original English ComboBox values
            BindCombos.PopulateCostCenter(cmbDebitCostCenter, true);
            BindCombos.PopulateCostCenter(cmbCreditCostCenter, true);

            BindCombos.PopulateAllLevel4Account(cmbDebitAccountName, true);
            BindCombos.PopulateAllLevel4Account(cmbCreditAccountName, true);

            BindCombos.PopulateRegisterBanks(cmbBankName, true);

            cmbMethod.SelectedIndex =
                cmbPaymentType.SelectedIndex = 0;

            if (id > 0)
            {
                btnSave.Enabled = BtnSaveNew.Enabled =
                    UserPermissions.canEdit("Vouchers");

                BtnDelete.Enabled =
                    UserPermissions.canDelete("Vouchers");

                BindVoucher();
            }
            else
            {
                if (defaultCustomerId > 0)
                {
                    cmbVendor.SelectedValue = defaultCustomerId;
                    LoadData();
                }
            }

            // Translate grid headers
            LocalizationManager.LocalizeDataGridViewHeaders(dgvInv);

            // Translate labels, buttons and headings only
            ApplyPaymentVoucherArabic();
        }
        private void BindVoucher()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT * FROM tbl_payment_voucher WHERE id = " + id))
                if (reader.Read())
                {
                    txtId.Text = reader["id"].ToString();
                    txtPVCode.Text = reader["code"].ToString();
                    dtOpen.Value = DateTime.Parse(reader["date"].ToString());
                    code = reader["code"].ToString(); 
                    string typeValue = reader["type"].ToString();
                    cmbPaymentType.SelectedItem = typeValue;    
                    string methodValue = reader["method"].ToString();
                    cmbMethod.SelectedItem = methodValue;
                    txtAmount.Text = reader["amount"].ToString();
                    cmbCreditAccountName.SelectedValue = reader["credit_account_id"].ToString();
                    cmbCreditCostCenter.SelectedValue = reader["credit_cost_center_id"].ToString();
                    txtDescription1.Text = reader["description"].ToString();
                    cmbDebitAccountName.SelectedValue = reader["debit_account_id"].ToString();
                    cmbDebitCostCenter.SelectedValue = reader["debit_cost_center_id"].ToString();

                    if (reader["bank_id"] != null)
                    {
                        if (cmbMethod.Text == "Cheque")
                        {
                            cmbBankName.SelectedValue = reader["bank_id"].ToString();
                        }
                    }
                    //txtBankCode.Text = reader["bank_code"].ToString();
                    if (cmbMethod.Text == "Cheque")
                    {
                        txtCheckName.Text = reader["check_name"].ToString();
                        cmbCheckNo.SelectedValue = reader["check_no"].ToString();
                    }
                    if (reader["trans_date"].ToString() != "")
                    {
                        dtpTransDate.Value = DateTime.Parse(reader["trans_date"].ToString());
                    }
                    txtTransName.Text = reader["trans_name"].ToString();
                    txtTransRef.Text = reader["trans_ref"].ToString();

                    if (cmbPaymentType.Text == "Vendor")
                    {
                        txtAmount.Enabled = false;
                        BindCombos.PopulateVendors(cmbVendor, false, false, VendorType);
                        dgvInv.Columns["InvNo"].HeaderText = "Inv No";
                        cmbCreditCostCenter.Visible =
                        lblCostcenter.Visible =
                        dgvInv.Visible = true;
                    }
                    else if (cmbPaymentType.Text == "Petty Cash")
                    {
                        //txtAmount.Enabled = true;
                        //cmbCreditCostCenter.Visible = false;
                        //lblCostcenter.Visible = dgvInv.Visible = true;
                    }

                    loadPaymentDataDetails();
                }
        }
        private void loadPaymentDataDetails()
        {
            dgvInv.Rows.Clear();

            int serialNumber = 1;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                     SELECT dt.id,dt.date,dt.payment_id,dt.hum_id,dt.inv_code,dt.inv_id,dt.payment,dt.total,dt.description,dt.cost_center_id,voucher_type
                     FROM tbl_payment_voucher_details dt
                     WHERE dt.payment_id = @id;",
                    ClinicDatabase.CreateParameterNew("id", id)))
                while (reader.Read())
                {
                    string query = "";
                    if (cmbPaymentType.Text == "Vendor")
                    {
                        query = @"
                            SELECT tbl_purchase.date,tbl_purchase.id,tbl_purchase.invoice_id,tbl_purchase.net,tbl_purchase.pay,tbl_purchase.change 
                            FROM tbl_purchase 
                            INNER JOIN tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id
                            WHERE tbl_purchase.id = @id
                            ";
                    }
                    else if (cmbPaymentType.Text == "Employee")
                    {
                        query = @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_attendance_salary.date) AS SN, tbl_attendance_salary.date,tbl_attendance_salary.id,tbl_attendance_salary.ss_no invoice_id,
                                tbl_attendance_salary.net_salary net,tbl_attendance_salary.pay,tbl_attendance_salary.change FROM tbl_attendance_salary 
                                WHERE emp_code = (SELECT CODE FROM tbl_employee where id=@id ) AND `change` <> 0";
                    }
                    else if(cmbPaymentType.Text == "Petty Cash")
                    {
                        query = @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_petty_cash_request.request_date) AS SN, 
                                tbl_petty_cash_request.request_date date,tbl_petty_cash_request.id,tbl_petty_cash_request.request_ref invoice_id,tbl_petty_cash_request.amount net,
                                tbl_petty_cash_request.pay,tbl_petty_cash_request.change FROM tbl_petty_cash_request 
                                WHERE tbl_petty_cash_request.Petty_cash_name = @id AND `change` <> 0";
                    }

                    string inv_id = "0", inv_code = reader["inv_code"].ToString(), bill_amount = "0", bill_date = "", payment = "0", description = "", cost_center_id = "0";
                    cost_center_id = (reader["cost_center_id"].ToString() == "" ? null : reader["cost_center_id"].ToString());
                    description = reader["description"].ToString();
                    payment = Utilities.FormatDecimal(reader["payment"].ToString());
                    string humId = reader["hum_id"].ToString();
                    cmbVendor.SelectedValue = int.Parse(humId);
                    string vType = reader["voucher_type"].ToString();
                    //if (vType.StartsWith("Petty Cash"))
                    //{
                    //    cbpettycash.Enabled = true;
                    //    cbpettycash.Checked = true;
                    //    cbpettycash.Visible = true;
                    //    lstOfEmp.Enabled = true;
                    //}
                    string _refId = (cmbPaymentType.Text == "Employee" ? reader["hum_id"].ToString() : cmbPaymentType.Text == "Petty Cash" ? reader["hum_id"].ToString() : reader["inv_id"].ToString());
                    using (MySqlDataReader drSale = ClinicDatabase.ExecuteReader(query,
                        ClinicDatabase.CreateParameterNew("id", _refId)))
                        if (drSale.Read())
                        {
                            inv_id = drSale["id"].ToString();
                            bill_amount = Utilities.FormatDecimal(drSale["net"].ToString());
                            bill_date = DateTime.Parse(drSale["date"].ToString()).ToShortDateString();
                        }
                        else
                        {
                            bill_amount = Utilities.FormatDecimal(reader["total"].ToString());
                            bill_date = DateTime.Parse(reader["date"].ToString()).ToShortDateString();
                        }
                    dgvInv.Rows.Add(serialNumber.ToString(), humId, inv_id, bill_date, inv_code, bill_amount, false, payment, description, cost_center_id, vType);

                    serialNumber++;
                }

            dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
            dgvInv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvInv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            dgvInv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvInv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvInv.ColumnHeadersHeight = 18;
            dgvInv.EnableHeadersVisualStyles = false;
            dgvInv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            calculateTotal();

            if (cmbPaymentType.Text == "Employee")
            {
                if (id > 0)
                    TickFillCheckNameEmployee();
            }
            else if(cmbPaymentType.Text == "Petty Cash")
            {
                if (id > 0)
                    TickFillCheckNameEmployee();
            }
        }
        private string GenerateNextPaymentCode()
        {
            string newCode = "PV-0001";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(SUBSTRING(code, 4) AS UNSIGNED)) AS lastCode FROM tbl_payment_voucher"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = "PV-" + code.ToString("D4");
                }
            }

            return newCode;
        }
        private string GenerateNextPaymentId()
        {
            string newCode = "1";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(id) AS lastCode FROM tbl_payment_voucher"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = code.ToString();
                }
            }

            return newCode;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                if (insertPV())
                {
                    EventHub.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Paid  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                }
            }
            else
            {
                if (updatePV())
                {
                    EventHub.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Updated !  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                }
            }
        }
        private void loadPrint(string mm)
        {
            //DialogResult result = MessageBox.Show("Do You want To Show This Bill ",
            //                                   "Confirmation",
            //                                   MessageBoxButtons.YesNo,
            //                                   MessageBoxIcon.Question);
            //// Check if the user clicked Yes or No
            //if (result == DialogResult.Yes)
            //{
                // Code for when the user clicks "Yes"
                if (mm == "Cash")
                {
                    //ShowReport1();
                }
                else if (mm == "Cheque")
                {               
                    ShowReport2();

                }
            //else if (mm == "Transfer")
            //{
            //    ShowReport3();
            //}

            //else if (result == DialogResult.No)
            //{
            //    this.Close();
            //}
        }
        private bool updatePV()
        {
            if (!chkRequireData())
                return false;

            string GlobalId = ClinicDatabase.ExecuteScalar("select GlobalId from tbl_payment_voucher where id=@id", ClinicDatabase.CreateParameterNew("id", id)).ToString();

            ClinicDatabase.ExecuteScalar(@"UPDATE tbl_payment_voucher SET date=@date, code=@code, type=@type, method=@method, amount=@amount,  
                                                            debit_account_id=@debit_account_id, debit_cost_center_id=@debit_cost_center_id, description=@description,credit_account_id=@credit_account_id,
                                                            credit_cost_center_id=@credit_cost_center_id,bank_id=@bank_id, bank_account_id=@bank_account_id, book_no=@book_no, check_name=@check_name, check_no=@check_no,
                                                            check_date=@check_date, trans_date=@trans_date, trans_name=@trans_name, trans_ref=@trans_ref, modified_by=@modified_by, modified_date=@modified_date,LastModified=@LastModified,SyncStatus = 0 where id=@id",
                                                            ClinicDatabase.CreateParameterNew("id", id),
                                                            ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                                                            ClinicDatabase.CreateParameterNew("code", code),
                                                            ClinicDatabase.CreateParameterNew("type", cmbPaymentType.Text),
                                                            ClinicDatabase.CreateParameterNew("method", cmbMethod.Text),
                                                            ClinicDatabase.CreateParameterNew("amount", txtAmount.Text),
                                                            ClinicDatabase.CreateParameterNew("debit_account_id", cmbDebitAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("debit_cost_center_id", cmbDebitCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("description", txtDescription1.Text),
                                                            ClinicDatabase.CreateParameterNew("credit_account_id", cmbCreditAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("credit_cost_center_id", cmbCreditCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("bank_id", cmbMethod.Text == "Cheque" ? cmbBankName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("bank_account_id", cmbMethod.Text == "Cheque" ? cmbBankAccountName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("book_no", cmbBookNo.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                                                            ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                                                            ClinicDatabase.CreateParameterNew("check_date", cmbMethod.Text == "Cheque" ? (object)dt_check_date.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_date", cmbMethod.Text == "Transfer" ? (object)dtpTransDate.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_name", txtTransName.Text),
                                                            ClinicDatabase.CreateParameterNew("trans_ref", txtTransRef.Text),
                                                            ClinicDatabase.CreateParameterNew("modified_by", -1),
                                                            ClinicDatabase.CreateParameterNew("modified_date", DateTime.Now.Date),
                                                            ClinicDatabase.CreateParameterNew("LastModified", DateTime.Now.Date));

            ClinicDatabase.ExecuteNonQuery("Delete from tbl_payment_voucher_details where payment_id=@id", ClinicDatabase.CreateParameterNew("id", id));
            CommonInsert.DeleteTransactionEntry(id, "PAYMENT");

            if (cmbMethod.Text == "Cheque")
            {
                object result = ClinicDatabase.ExecuteScalar(
                 @"SELECT COUNT(1) FROM tbl_check_details 
                  WHERE pvc_no = @pvc_no AND check_type = @check_type AND check_id = @check_id",
                 ClinicDatabase.CreateParameterNew("pvc_no", id),
                 ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                 ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue)
                 );

                int recordCount = 0;
                if (result != null && result != DBNull.Value)
                    recordCount = Convert.ToInt32(result); // safer than (int)

                if (recordCount > 0)
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                    UPDATE tbl_check_details
                    SET check_no = @check_no, 
                        check_date = @check_date, 
                        check_name = @check_name, 
                        amount = @amount
                    WHERE pvc_no = @pvc_no AND check_type = @check_type AND check_id = @check_id",
                        ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                        ClinicDatabase.CreateParameterNew("check_date", dt_check_date.Value.Date),
                        ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                        ClinicDatabase.CreateParameterNew("amount", decimal.Parse(txtAmount.Text)),
                        ClinicDatabase.CreateParameterNew("pvc_no", id),
                        ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                        ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue)
                    );
                }
                else
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                    INSERT INTO tbl_check_details
                    (date, check_id, check_no, check_date, check_type, pvc_no, check_name, amount, state)
                    VALUES
                    (@date, @check_id, @check_no, @check_date, @check_type, @pvc_no, @check_name, @amount, @state)",
                        ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                        ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue),
                        ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                        ClinicDatabase.CreateParameterNew("check_date", dt_check_date.Value.Date),
                        ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                        ClinicDatabase.CreateParameterNew("pvc_no", id),
                        ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                        ClinicDatabase.CreateParameterNew("amount", decimal.Parse(txtAmount.Text)),
                        ClinicDatabase.CreateParameterNew("state", "New")
                    );
                }
            }
            else if (cmbMethod.Text == "Transfer")
            {
            }
            //insertJournals();
            insertINV(id, GlobalId);
            CommonInsert.DeleteCostCenterTransactionEntry(id.ToString(), "Payment");
            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, txtAmount.Text.ToString(), "0", id.ToString(), "Payment", "Payment Debit Entry", cmbDebitCostCenter.SelectedValue.ToString());
            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, "0", txtAmount.Text.ToString(), id.ToString(), "Payment", "Payment Credit Entry", cmbCreditCostCenter.SelectedValue.ToString());

            Utilities.LogAudit(-1, "Update Payment Voucher", "Payment Voucher", id, "Updated Payment Voucher: " + code);
            if (!string.IsNullOrEmpty(filePath))
            {
                AddFileData(id);
            }
            return true;
        }
        private bool insertPV()
        {
            if (!chkRequireData())
                return false;
            code = GenerateNextPaymentCode();
            string GlobalId = Guid.NewGuid().ToString();
            id = (int)decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_payment_voucher(date, code, type, method, amount,  
                                                            debit_account_id, debit_cost_center_id, description,credit_account_id,
                                                            credit_cost_center_id,bank_id, bank_account_id, book_no, check_name, check_no,
                                                            check_date, trans_date, trans_name, trans_ref, created_by, created_date, state, GlobalId, DeviceId) 
                                                            VALUES (@date, @code, @type, @method, @amount, @debit_account_id, @debit_cost_center_id,
                                                            @description,@credit_account_id, @credit_cost_center_id, @bank_id, @bank_account_id, 
                                                            @book_no, @check_name, @check_no,@check_date, @trans_date, @trans_name, @trans_ref, 
                                                            @created_by, @created_date, 0, @GlobalId, @DeviceId); SELECT LAST_INSERT_ID();",
                                                            ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                                                            ClinicDatabase.CreateParameterNew("code", code),
                                                            ClinicDatabase.CreateParameterNew("type", cmbPaymentType.Text),
                                                            ClinicDatabase.CreateParameterNew("method", cmbMethod.Text),
                                                            ClinicDatabase.CreateParameterNew("amount", txtAmount.Text),
                                                            ClinicDatabase.CreateParameterNew("debit_account_id", cmbDebitAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("debit_cost_center_id", cmbDebitCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("description", txtDescription1.Text),
                                                            ClinicDatabase.CreateParameterNew("credit_account_id", cmbCreditAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("credit_cost_center_id", cmbCreditCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("bank_id", cmbMethod.Text == "Cheque" ? cmbBankName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("bank_account_id", cmbMethod.Text == "Cheque" ? cmbBankAccountName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("book_no", cmbBookNo.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                                                            ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                                                            ClinicDatabase.CreateParameterNew("check_date", cmbMethod.Text == "Cheque" ? (object)dt_check_date.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_date", cmbMethod.Text == "Transfer" ? (object)dtpTransDate.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_name", txtTransName.Text),
                                                            ClinicDatabase.CreateParameterNew("trans_ref", txtTransRef.Text),
                                                            ClinicDatabase.CreateParameterNew("created_by", -1),
                ClinicDatabase.CreateParameterNew("GlobalId", GlobalId),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName),
                                                            ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date)).ToString());
            if (cmbMethod.Text == "Cheque")
            {
                ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_check_details
                    (date, check_id, check_no, check_date, check_type, pvc_no, check_name, amount, state)
                    VALUES
                    (@date, @check_id, @check_no, @check_date, @check_type, @pvc_no, @check_name, @amount, @state)",
                    ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                    ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue),
                    ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                    ClinicDatabase.CreateParameterNew("check_date", dt_check_date.Value.Date),
                    ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                    ClinicDatabase.CreateParameterNew("pvc_no", id),
                    ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                    ClinicDatabase.CreateParameterNew("amount", decimal.Parse(txtAmount.Text)),
                    ClinicDatabase.CreateParameterNew("state", "New")
                );
            }
            else if (cmbMethod.Text == "Transfer")
            {
            }
            //insertJournals();
            insertINV(id, GlobalId);
            Utilities.LogAudit(-1, "Insert Payment Voucher", "Payment Voucher", id, "Inserted Payment Voucher: " + code);

            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, txtAmount.Text.ToString(), "0", id.ToString(), "Payment", "Payment Debit Entry", cmbDebitCostCenter.SelectedValue.ToString());
            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, "0", txtAmount.Text.ToString(), id.ToString(), "Payment", "Payment Credit Entry", cmbCreditCostCenter.SelectedValue.ToString());

            if (!string.IsNullOrEmpty(filePath))
            {
                AddFileData(id);
            }

            return true;
        }
        private bool chkRequireData()
        {
            if (cmbPaymentType.Text.Trim() == "")
            {
                MessageBox.Show("Please Choose Payment Type.");
                return false;
            }
            if (txtAmount.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Payment Amount.");
                return false;
            }
            if (cmbDebitAccountName.Text.Trim() == "")
            {
                MessageBox.Show("Please Choose Debit Account Name");
                return false;
            }
            if (cmbCreditAccountName.Text.Trim() == "")
            {
                MessageBox.Show("Please Choose Credit Account Name");
                return false;
            }
            if ((txtAmount.Text == "" || decimal.Parse(txtAmount.Text) == 0) && dgvInv.Rows.Count == 0)
            {
                MessageBox.Show("Enter Amount");
                return false;
            }
            if (cmbMethod.Text == "Cheque")
            {
                if (cmbBankName.SelectedValue == null || cmbBankAccountName.SelectedValue == null || cmbBookNo.SelectedValue == null)
                {
                    MessageBox.Show("Please Enter Bank Information First");
                    return false;
                }
            }
            else if (cmbMethod.Text == "Transfer")
            {
                if (txtTransName.Text == "" || txtTransRef.Text == "")
                {
                    MessageBox.Show("Please Enter Transfer Information First");
                    return false;
                }
            }
            return true;
        }
        private void insertINV(int pvId, string globalId)
        {
            for (int i = 0; i < dgvInv.Rows.Count; i++)
            {
                if (dgvInv.Rows[i].Cells["Pay"].Value == null ||
                    dgvInv.Rows[i].Cells["Pay"].Value.ToString() == "" ||
                    decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString()) == 0)
                    continue;
                var invId = dgvInv.Rows[i].Cells["invId"].Value?.ToString() != "" ? dgvInv.Rows[i].Cells["invId"].Value?.ToString() : "0";
                var hum = dgvInv.Rows[i].Cells["humId"].Value?.ToString() ?? string.Empty;
                var total = dgvInv.Rows[i].Cells["Total"].Value?.ToString() ?? string.Empty;
                string payment = decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString()).ToString();
                string change = (decimal.Parse(total) - decimal.Parse(payment)).ToString();
                var description = dgvInv.Rows[i].Cells["Description"].Value?.ToString() ?? "";
                var voucherType = dgvInv.Rows[i].Cells["voucherType"].Value?.ToString() ?? "";

                DateTime date = DateTime.Parse(dgvInv.Rows[i].Cells["invDate"].Value.ToString());

                ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_payment_voucher_details(date,payment_id,hum_id, inv_id,inv_code,total, payment, description,voucher_type, GlobalId, DeviceId) 
                                        VALUES (@date,@payment_id,@hum_id ,@inv_id,@inv_code,@total, @payment, @description,@voucher_type, @GlobalId, @DeviceId);",
                                        ClinicDatabase.CreateParameterNew("@payment_id", pvId),
                                        ClinicDatabase.CreateParameterNew("@date", date),
                                        ClinicDatabase.CreateParameterNew("@hum_id", hum),
                                        ClinicDatabase.CreateParameterNew("@total", total),
                                        ClinicDatabase.CreateParameterNew("@inv_code", dgvInv.Rows[i].Cells["InvNo"].Value.ToString()),
                                        ClinicDatabase.CreateParameterNew("@inv_id", int.Parse(invId)),
                                        ClinicDatabase.CreateParameterNew("@payment", payment),
                                        ClinicDatabase.CreateParameterNew("@description", description),
                                        ClinicDatabase.CreateParameterNew("@voucher_type", voucherType),
                ClinicDatabase.CreateParameterNew("GlobalId", globalId),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName)
                                        );

                if (cmbPaymentType.Text == "Employee")
                {
                    if (dgvInv.Rows[i].Cells["voucherType"].Value != null &&
                            dgvInv.Rows[i].Cells["voucherType"].Value.ToString() == "Salary")
                    {
                        ClinicDatabase.ExecuteNonQuery(" UPDATE tbl_attendance_salary SET tbl_attendance_salary.pay = pay+@pay , tbl_attendance_salary.change = @change where id = @id",
                        ClinicDatabase.CreateParameterNew("pay", payment),
                        ClinicDatabase.CreateParameterNew("change", change),
                        ClinicDatabase.CreateParameterNew("id", int.Parse(dgvInv.Rows[i].Cells["invId"].Value.ToString())));
                    }
                    //Employee Loan Payment
                    //else if (dgvInv.Rows[i].Cells["voucherType"].Value?.ToString() == "Employee Loan Payment")
                    //{
                    //    int employeeCode = Convert.ToInt32(dgvInv.Rows[i].Cells["invno"].Value.ToString().Split('-')[0]);
                    //    ClinicDatabase.ExecuteNonQuery(" UPDATE tbl_loan SET pay = pay+@pay , `change` = @change where EmployeeID = @code and loanDate=@date",
                    //    ClinicDatabase.CreateParameterNew("pay", decimal.Parse(payment)),
                    //    ClinicDatabase.CreateParameterNew("change", change),
                    //    ClinicDatabase.CreateParameterNew("code", employeeCode.ToString()),
                    //    ClinicDatabase.CreateParameterNew("date", DateTime.Parse(dgvInv.Rows[i].Cells["invDate"].Value.ToString())));
                    //}
                }
                else if (cmbPaymentType.Text == "Vendor")
                {
                    int currentInvId = dgvInv.Rows[i].Cells["invId"].Value?.ToString() != "" ? int.Parse(dgvInv.Rows[i].Cells["invId"].Value?.ToString()) : 0;
                    decimal newPayment = decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString());

                    // 🟡 Fetch net amount directly from tbl_purchase
                    object netResult = ClinicDatabase.ExecuteScalar(
                        "SELECT net FROM tbl_purchase WHERE id = @id",
                        ClinicDatabase.CreateParameterNew("id", currentInvId)
                    );

                    decimal net = netResult != DBNull.Value ? Convert.ToDecimal(netResult) : 0;

                    // 🟢 Fetch cumulative payment (total paid so far)
                    object result = ClinicDatabase.ExecuteScalar(
                        "SELECT SUM(payment) FROM tbl_payment_voucher_details WHERE inv_id = @id",
                        ClinicDatabase.CreateParameterNew("id", currentInvId)
                    );

                    decimal totalPaid = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                    // 🔵 Calculate remaining amount
                    decimal remaining = net - totalPaid;

                    // 🔴 Update tbl_purchase with correct pay and change
                    ClinicDatabase.ExecuteNonQuery(
                        "UPDATE tbl_purchase SET pay = @pay, `change` = @change WHERE id = @id",
                        ClinicDatabase.CreateParameterNew("pay", totalPaid),
                        ClinicDatabase.CreateParameterNew("change", remaining),
                        ClinicDatabase.CreateParameterNew("id", currentInvId)
                    );

                }
                else if (cmbPaymentType.Text == "Petty Cash")
                {
                    // Petty Cash Pay
                    //if (dgvInv.Rows[i].Cells["voucherType"].Value != null &&
                    //    dgvInv.Rows[i].Cells["voucherType"].Value.ToString() == "Petty Cash Request")
                    if(cbpettycash.Checked)
                    {
                        int requestId = int.Parse(dgvInv.Rows[i].Cells["invId"].Value.ToString());
                        if (cbpettycash.Checked)
                        {
                            ClinicDatabase.ExecuteNonQuery(
                            "UPDATE tbl_petty_cash_request SET pay = pay + @pay, `change` = @change WHERE id = @id",
                            ClinicDatabase.CreateParameterNew("pay", payment),
                            ClinicDatabase.CreateParameterNew("change", change),
                            ClinicDatabase.CreateParameterNew("id", requestId));
                        }
                    }
                }
                insertJournals(cmbPaymentType.Text.ToString(), voucherType, payment, description);
            }
        }
        void insertJournals(string type, string voucherType, string amount, string description)
        {
            string tType = "", humId = "0";
            if (cmbPaymentType.Text == "Vendor")
            {
                tType = CbSubcontractors.Checked ? "Subcontractor Payment" : "Vendor Payment";
                humId = cmbPaymentType.Text == "Vendor" ? cmbVendor.SelectedValue.ToString() : "0";
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                humId = (lstOfEmp.SelectedItem as ListItemModel).Id.ToString();
                if (voucherType == "Salary")
                {
                    tType = "Employee Salary Payment";
                }
                //else if (voucherType == "Employee Loan Payment")
                //{
                //    tType = "Employee Loan Payment";
                //}
            }
            else if (cmbPaymentType.Text == "Petty Cash")
            {
                //if (voucherType == "Petty Cash Request")
                //{
                    tType = "Employee Petty Cash Payment";
                //}
            }

            CommonInsert.addTransactionEntry(dtOpen.Value.Date,
                cmbDebitAccountName.SelectedValue.ToString(),
                amount, "0", id.ToString(), humId, tType, "PAYMENT", "Payment Voucher NO. " + code,
                 -1, DateTime.Now.Date, txtPVCode.Text);

            CommonInsert.addTransactionEntry(dtOpen.Value.Date,
                                  cmbCreditAccountName.SelectedValue.ToString(),
                               "0", amount, id.ToString(), "0", tType, "PAYMENT", "Payment Voucher NO. " + code,
                                   -1, DateTime.Now.Date, txtPVCode.Text);
        }

        private void cmbPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvInv.Rows.Clear();
            if (cmbPaymentType.Text == "Vendor")
            {
                dgvInv.Visible = pnlCustomer.Visible = true;
                lstOfEmp.Visible = txtAmount.Enabled = false;
                BindCombos.PopulateVendors(cmbVendor, false, false, VendorType);
                dgvInv.Columns["InvNo"].HeaderText = "Inv No";
                dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "Vendor")))
                    if (reader.Read())
                    {
                        txtDebitCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbDebitAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }

                cbpettycash.Visible = false;
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                pnlCustomer.Visible = txtAmount.Enabled = false;
                lstOfEmp.Visible = true;
                dgvInv.Columns["InvNo"].HeaderText = "Employee Name";
                dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "Accrued Salaries")))
                    if (reader.Read())
                    {
                        txtDebitCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbDebitAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }
                cbpettycash.Visible = false;

            }
            if (cmbPaymentType.Text == "Petty Cash")
            {
                pnlCustomer.Visible = false;
                txtAmount.Enabled = false;
                lstOfEmp.Visible = true;
                cbpettycash.Visible = true;
                cbpettycash.Checked = true;
                cbpettycash.Enabled = true;
                dgvInv.Columns["InvNo"].HeaderText = "Petty Cash Ref";
                // Debit = Cash / Bank if employee returns money
                // OR employee advance account if expense settlement

                using (MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(@"
                        SELECT id,code 
                        FROM tbl_coa_level_4
                        WHERE id = (
                            SELECT account_id
                            FROM tbl_coa_config
                            WHERE category='Employee Advance'
                        )"))
                {
                    if (reader.Read())
                    {
                        txtDebitCode.Text = reader["code"].ToString();
                        cmbDebitAccountName.SelectedValue =
                            Convert.ToInt32(reader["id"]);
                    }
                }
            }
        }
        private void LoadData()
        {
            if (cmbPaymentType.Text == "Vendor")
            {
                decimal customerTotalBalance = 0, customerTotalOB = 0; int counter = 0;
                dgvInv.Rows.Clear();
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT balance as amount FROM tbl_vendor where id = @id
                    ", ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                    if (reader.Read() && reader["amount"].ToString() != "")
                        customerTotalOB = decimal.Parse(reader["amount"].ToString());

                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT c.id, c.date,
                        (SELECT sum(t.payment) FROM tbl_payment_voucher_details t WHERE t.inv_code ='Vendor Opening Balance' AND t.hum_id = c.id) as amount 
                        FROM tbl_vendor c where c.id = @id;",
                          ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                {
                    if (reader.Read() && reader["amount"].ToString() != "")
                        customerTotalBalance = decimal.Parse(reader["amount"].ToString());

                    if (customerTotalOB != customerTotalBalance)
                        dgvInv.Rows.Add(++counter, cmbVendor.SelectedValue, "", DateTime.Parse(reader["date"].ToString()).ToShortDateString(), "Vendor Opening Balance", customerTotalOB - customerTotalBalance, 0, 0, "Vendor Opening Balance");
                }
                
                using (var reader = ClinicDatabase.ExecuteReader(@"SELECT ROW_NUMBER() OVER (ORDER BY tbl_purchase.date) AS SN, 
                                                                      tbl_purchase.date AS DATE, tbl_purchase.id, tbl_purchase.invoice_id AS 'INV NO', 
                                                                      tbl_purchase.change FROM tbl_purchase INNER JOIN tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id
                                                                      WHERE tbl_purchase.state = 0 AND tbl_purchase.change <> 0 AND tbl_vendor.id = @id
                                                                      GROUP BY tbl_purchase.id, tbl_purchase.date;",
                                                                      ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                {
                    while (reader.Read())
                    {
                        string date = DateTime.Parse(reader["DATE"].ToString()).ToShortDateString();
                        string invoiceId = reader["INV NO"].ToString();
                        string change = reader["change"].ToString();
                        dgvInv.Rows.Add((int.Parse(reader["SN"].ToString()) + counter).ToString(), cmbVendor.SelectedValue, reader["id"].ToString(), date, invoiceId, change, 0, 0, "");
                    }
                }
                dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
                dgvInv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
                dgvInv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Regular);
                dgvInv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dgvInv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvInv.ColumnHeadersHeight = 18;
                dgvInv.EnableHeadersVisualStyles = false;
                dgvInv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                //
            }
            else if (cmbPaymentType.Text == "Petty Cash")
            {
                dgvInv.Rows.Clear();
                int empId = 0; // GetSelectedEmployeeId();

                using (var reader = ClinicDatabase.ExecuteReader(@"
                    SELECT id,
                           request_date,
                           request_ref,
                           `change`
                    FROM tbl_petty_cash_request
                    WHERE Petty_cash_name=@empId
                    AND state='Approved'
                    AND `change` > 0",
                    ClinicDatabase.CreateParameterNew("@empId", empId)))
                {
                    int i = 1;

                    while (reader.Read())
                    {
                        dgvInv.Rows.Add(
                            i++,
                            empId,
                            reader["id"],
                            Convert.ToDateTime(reader["request_date"]).ToShortDateString(),
                            reader["request_ref"].ToString(),
                            reader["change"].ToString(),
                            0,
                            0,
                            "Outstanding Petty Cash"
                        );
                    }
                }
            }
        }
        private void cmbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void txtTypeCode_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtTypeCode_Leave(object sender, EventArgs e)
        {

        }

        bool isDebitAccountNameRefreshing = false;
        private void cmbDebitAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDebitAccountName.SelectedValue == null)
            {
                txtDebitCode.Text = "";
                return;
            }

            if (cmbDebitAccountName.SelectedItem != null)
            {
                if (cmbDebitAccountName.SelectedItem is DataRowView row)
                {
                    string code = row["code"].ToString();
                    txtDebitCode.Text = code == "<< Add >>" ? "" : code;
                }
            }
            else
            {
                txtDebitCode.Text = "";
            }


            if (cmbDebitAccountName.Focused) {
                if (isDebitAccountNameRefreshing) return;

                if (cmbDebitAccountName.SelectedValue != null &&
                (cmbDebitAccountName.SelectedValue.ToString() == "0" || cmbDebitAccountName.Text == "<< Add >>"))
                {
                    new frmAddAccount().ShowDialog();
                    isDebitAccountNameRefreshing = true;
                    BindCombos.PopulateAllLevel4Account(cmbDebitAccountName, true, true);
                    BindCombos.PopulateAllLevel4Account(cmbCreditAccountName, true);
                    cmbDebitAccountName.SelectedIndex = cmbDebitAccountName.Items.Count - 1;
                    isDebitAccountNameRefreshing = false;
                }
            }
        }
        bool isCreditAccountNameRefreshing = false;
        private void cmbCreditAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCreditAccountName.SelectedValue == null)
            {
                txtCreditAccountCode.Text = "";
                return;
            }

            if (cmbCreditAccountName.SelectedItem != null)
            {
                if (cmbCreditAccountName.SelectedItem is DataRowView row)
                {
                    string code = row["code"].ToString();
                    txtCreditAccountCode.Text = code == "<< Add >>" ? "" : code;
                }
            }
            else
            {
                txtCreditAccountCode.Text = "";
            }

            if (cmbCreditAccountName.Focused)
            {
                if (isCreditAccountNameRefreshing) return;

                if (cmbCreditAccountName.SelectedValue != null &&
                (cmbCreditAccountName.SelectedValue.ToString() == "0" || cmbCreditAccountName.Text == "<< Add >>"))
                {
                    new frmAddAccount().ShowDialog(); // This will wait until form is closed

                    isCreditAccountNameRefreshing = true;
                    BindCombos.PopulateAllLevel4Account(cmbCreditAccountName, true);
                    cmbCreditAccountName.SelectedIndex = cmbCreditAccountName.Items.Count - 1;
                    isCreditAccountNameRefreshing = false;
                }
            }
        }

        private void txtDebitCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
              ClinicDatabase.CreateParameterNew("code", txtDebitCode.Text)))
            {
                if (reader.Read())
                    cmbDebitAccountName.SelectedValue = int.Parse(reader["id"].ToString());
            }

            if (txtDebitCode.Focused)
            {
                string input = txtDebitCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_coa_level_4 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["code"].ToString(),
                        Name = row["name"].ToString()
                    });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtDebitCode.Parent.PointToScreen(txtDebitCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtDebitCode.Height,
                        txtDebitCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtDebitCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void txtCreditAccountCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                "SELECT * FROM tbl_coa_level_4 WHERE code = @code",
                ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
            {
                if (reader.Read())
                    cmbCreditAccountName.SelectedValue = int.Parse(reader["id"].ToString());
            }

            if (txtCreditAccountCode.Focused)
            {
                string input = txtCreditAccountCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_coa_level_4 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion { Id = Convert.ToInt32(row["id"]),Code = row["code"].ToString(), Name = row["name"].ToString() });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtCreditAccountCode.Parent.PointToScreen(txtCreditAccountCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtCreditAccountCode.Height,
                        txtCreditAccountCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtCreditAccountCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void txtDebitCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code", ClinicDatabase.CreateParameterNew("code", txtDebitCode.Text)))
                if (!reader.Read())
                    cmbDebitAccountName.SelectedIndex = -1;

            BeginInvoke((Action)(() =>
            {
                if (!lstAccountSuggestions.Focused)
                    lstAccountSuggestions.Visible = false;
            }));
        }

        private void txtCreditAccountCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code", ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
                if (!reader.Read())
                    cmbCreditAccountName.SelectedIndex = -1;

            BeginInvoke((Action)(() =>
            {
                if (!lstAccountSuggestions.Focused)
                    lstAccountSuggestions.Visible = false;
            }));
        }
        private void cmbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(cmbMethod.SelectedItem) == "Cash")
            {
                pnlBank.Visible = pnlTrans.Visible = false;
                cmbBankName.SelectedValue = cmbBankAccountName.SelectedValue = -1;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "Default Account For Cash")))
                    if (reader.Read())
                    {
                        txtCreditAccountCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }
            }
            else if (cmbMethod.Text == "Cheque")
            {
                pnlBank.Visible = true;
                pnlTrans.Visible = false;

                CheckDateCheck();
            }
            else if (cmbMethod.Text == "Transfer")
            {
                pnlBank.Visible = false;
                pnlTrans.Visible = true;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_level_4 WHERE main_id = (SELECT id FROM tbl_coa_level_3 WHERE NAME = 'Banks') LIMIT 1;"))
                    if (reader.Read())
                    {
                        txtCreditAccountCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }
            }
            else
            {
                pnlTrans.Visible = true;
                pnlBank.Visible = false;
            }
            if (cmbPaymentType.Text == "Vendor")
            {
                AutoFillCheckNameVendor();
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                AutoFillCheckNameEmployee();
            }
        }

        private void dt_check_date_ValueChanged(object sender, EventArgs e)
        {
            if(dt_check_date.Focused)
                CheckDateCheck();
        }

        private void CheckDateCheck()
        {
            if (!pnlBank.Visible)
                return;

            if (cmbMethod.Text == "Cheque")
            {
                //if (dt_check_date.Value.Date == DateTime.Now.Date)
                //{
                //    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_level_4 WHERE main_id = (SELECT id FROM tbl_coa_level_3 WHERE NAME = 'Banks') LIMIT 1;"))
                //        if (reader.Read())
                //        {
                //            txtCreditAccountCode.Text = reader["code"].ToString();
                //            string accountId = reader["id"].ToString();
                //            if (!string.IsNullOrEmpty(accountId))
                //            {
                //                cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                //            }
                //        }
                //}
                //else
                //{
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "PDC Payable")))
                        if (reader.Read())
                        {
                            txtCreditAccountCode.Text = reader["code"].ToString();
                            string accountId = reader["id"].ToString();
                            if (!string.IsNullOrEmpty(accountId))
                            {
                                cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                            }
                        }
                //}
            }
            else
            {
                dt_check_date.Enabled = false;
                cmbBookNo.Enabled = false;
                cmbBankName.Enabled = false;
                cmbBankAccountName.Enabled = false;
                txtCheckName.Enabled = false;
                cmbCheckNo.Enabled = false;
            }
        }
        bool isBankNameRefreshing = false;
        private void cmbBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBankName.SelectedValue == null)
            {
                txtBankCode.Text = "";
                return;
            }

            if (cmbBankName.Focused)
            {
                if (isBankNameRefreshing) return;
                if (cmbBankName.SelectedValue != null &&
            (cmbBankName.SelectedValue.ToString() == "0" || cmbBankName.Text == "<< Add >>"))
                {
                                            //var frm = new frmBankRegister(0);
                                            //frm.ShowDialog();
                                            isBankNameRefreshing = true;
                                            BindCombos.PopulateRegisterBanks(cmbBankName, true);
                                            cmbBankName.SelectedIndex = cmbBankName.Items.Count - 1;
                    isBankNameRefreshing = false;
                }
            }


            if (cmbBankName.SelectedItem != null)
            {
                string displayText = cmbBankName.Text;
                string code = displayText.Split('-')[0].Trim();
                txtBankCode.Text = code == "<< Add >>" ? "" : code;
            }
            else
            {
                txtBankCode.Text = "";
            }

            DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT id,account_name  from tbl_bank_card WHERE bank_id= " + cmbBankName.SelectedValue.ToString());
            cmbBankAccountName.DisplayMember = "account_name";
            cmbBankAccountName.ValueMember = "id";
            cmbBankAccountName.DataSource = dt;
        }

        private void txtBankCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_bank where code =@code",
           ClinicDatabase.CreateParameterNew("code", txtBankCode.Text)))
                if (reader.Read())
                    cmbBankName.SelectedValue = int.Parse(reader["id"].ToString());

            if (txtBankCode.Focused)
           {
                string input = txtBankCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_bank 
                         WHERE state = 0 and code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion { Id = Convert.ToInt32(row["id"]), Code = row["code"].ToString(), Name = row["name"].ToString() });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtBankCode.Parent.PointToScreen(txtBankCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtBankCode.Height,
                        txtBankCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtBankCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void txtBankCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_bank where code =@code",
                    ClinicDatabase.CreateParameterNew("code", txtBankCode.Text)))
                if (!reader.Read())
                    cmbBankName.SelectedIndex = -1;
        }

        private void cmbBankAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBankAccountName.SelectedValue == null)
                return;
            DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT id,chq_book_no  from tbl_cheque WHERE bank_card_id = " + cmbBankAccountName.SelectedValue.ToString());
            cmbBookNo.DisplayMember = "chq_book_no";
            cmbBookNo.ValueMember = "id";
            cmbBookNo.DataSource = dt;
        }

        private void dgvInv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInv.Rows.Count <= 0)
                return;
            if (dgvInv.CurrentRow == null)
                return;

            if (e.ColumnIndex == dgvInv.Columns["chkPay"].Index)
            {
                if (dgvInv.CurrentRow.Cells["chkPay"].Value.ToString() == "True")
                    dgvInv.CurrentRow.Cells["Pay"].Value = dgvInv.CurrentRow.Cells["Total"].Value.ToString();
                else
                    dgvInv.CurrentRow.Cells["Pay"].Value = "0";
            }
            if (e.ColumnIndex == dgvInv.Columns["Pay"].Index)
            {
                if (dgvInv.CurrentRow.Cells["Pay"].Value == null || dgvInv.CurrentRow.Cells["Pay"].Value.ToString() == "" || dgvInv.CurrentRow.Cells["Total"].Value == null)
                    return;

                decimal pay = decimal.Parse(dgvInv.CurrentRow.Cells["Pay"].Value.ToString());
                decimal total = decimal.Parse(dgvInv.CurrentRow.Cells["Total"].Value.ToString());

                if (pay > 0)
                {
                    if (decimal.Parse(dgvInv.CurrentRow.Cells["Pay"].Value.ToString()) > decimal.Parse(dgvInv.CurrentRow.Cells["Total"].Value.ToString()))
                    {
                        var dialog = MessageBox.Show("The amount you entered is grater than the total amount. Do you want to set the payment to the total amount?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialog == DialogResult.No)
                            if (decimal.Parse(dgvInv.CurrentRow.Cells["Pay"].Value.ToString()) > decimal.Parse(dgvInv.CurrentRow.Cells["Total"].Value.ToString()))
                                dgvInv.CurrentRow.Cells["Pay"].Value = dgvInv.CurrentRow.Cells["Total"].Value.ToString();
                    }
                }
                else if (pay < 0)
                {
                    if (pay < total)
                        dgvInv.CurrentRow.Cells["Pay"].Value = dgvInv.CurrentRow.Cells["Total"].Value.ToString();
                }
                    
            }
            calculateTotal();
        }
        private void calculateTotal()
        {
            for (int i = 0; i < dgvInv.Rows.Count; i++)
            {
                if (dgvInv.Rows[i].Cells["Pay"].Value.ToString() == "" || dgvInv.CurrentRow.Cells["Total"].Value.ToString() == "")
                    continue;
                totalAmount += decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString());
            }
            txtAmount.Text = totalAmount.ToString();
            totalAmount = 0;
        }
        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if (txtAmount.Text == "")
            {
                txtAmountInWord.Text = "";
                return;
            }
            txtAmountInWord.Text = AmountToWordsHelper.ConvertToSelectedLanguage(decimal.Parse(txtAmount.Text));
        }
        

        private string GenerateNextChequeCode()
        {
            int code;
            using (var reader = ClinicDatabase.ExecuteReader("select max(check_no) as lastCode from tbl_check_details   where check_type = 'Payment' and check_id =@id ",
                ClinicDatabase.CreateParameterNew("id", cmbBookNo.SelectedValue.ToString())))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                    code = int.Parse(reader["lastCode"].ToString()) + 1;
                else
                    code = 0001;
            }
            return code.ToString("D4");
        }

        private void cmbBookNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBookNo.SelectedValue == null)
                return;

            if (cmbBookNo.Focused)
            {
                cmbCheckNo.Items.Clear(); // Clear existing items

                // Get the selected cheque book's start and end leaves
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT leaves_start_from, leaves_end_in FROM tbl_cheque WHERE id = @id",
                    ClinicDatabase.CreateParameterNew("id", cmbBookNo.SelectedValue.ToString())))

                    if (reader.Read())
                    {
                        int start = int.Parse(reader["leaves_start_from"].ToString());
                        int end = int.Parse(reader["leaves_end_in"].ToString());
                        reader.Dispose();

                        // Create full list of all check numbers
                        List<int> availableChecks = new List<int>();
                        for (int i = start; i <= end; i++)
                        {
                            availableChecks.Add(i);
                        }

                        // Get used check numbers and remove them
                        using(MySqlDataReader usedReader = ClinicDatabase.ExecuteReader(@"
                            SELECT CAST(check_no AS UNSIGNED) AS check_no 
                            FROM tbl_check_details 
                            WHERE check_type = 'Payment' AND check_id = @check_id",
                            ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue.ToString())))
                        while (usedReader.Read())
                        {
                            if (usedReader["check_no"] != DBNull.Value)
                            {
                                int used = Convert.ToInt32(usedReader["check_no"]);
                                availableChecks.Remove(used); // Remove if exists
                            }
                        }

                        // Add remaining (available) checks to combo box
                        foreach (int checkNo in availableChecks)
                        {
                            cmbCheckNo.Items.Add(checkNo.ToString());
                        }
                    }
            }
        }

        bool isVendorRefreshing = false;

        private void cmbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (id > 0)
                return;

            if (cmbVendor.SelectedValue == null)
            {
                txtVendor.Text = "";
                txtDebitCode.Text = "";
                cmbDebitAccountName.SelectedIndex = -1;
                return;
            }

            if (cmbVendor.Focused)
            {
                if (isVendorRefreshing) return;

                if (cmbVendor.SelectedValue != null &&
                (cmbVendor.SelectedValue.ToString() == "0" || cmbVendor.Text == "<< Add >>"))
                {
                    var addVendorForm = new frmViewVendor();
                    addVendorForm.ShowDialog(); // This will wait until form is closed

                    isVendorRefreshing = true;
                    BindCombos.PopulateVendors(cmbVendor, true, true,VendorType);
                    cmbVendor.SelectedIndex = cmbVendor.Items.Count - 1;
                    isVendorRefreshing = false;
                }
            }

            if (cmbVendor.SelectedItem != null)
            {
                if (cmbVendor.SelectedItem is DataRowView row)
                {
                    string code = row["code"].ToString();
                    txtVendor.Text = code == "<< Add >>" ? "" : code;
                }
            }
            else
            {
                txtVendor.Text = "";
            }
            
            dgvInv.Rows.Clear();
            if (cmbVendor.SelectedValue == null)
                return;
            int counter = 0; decimal vendorTotalBalance = 0, vendorTotalOB = 0;
            string vendorOBDate = DateTime.Now.ToShortDateString(); // Default date
            using (MySqlDataReader obReader = ClinicDatabase.ExecuteReader(@"SELECT balance, date FROM tbl_vendor WHERE id = @id",
                ClinicDatabase.CreateParameterNew("@id", cmbVendor.SelectedValue)))

            if (obReader.Read())
            {
                if (!string.IsNullOrEmpty(obReader["balance"].ToString()))
                    vendorTotalOB = decimal.Parse(obReader["balance"].ToString());

                if (!string.IsNullOrEmpty(obReader["date"].ToString()))
                    vendorOBDate = DateTime.Parse(obReader["date"].ToString()).ToShortDateString();
            }
            // 2️⃣ Get how much has been paid from opening balance
            using (MySqlDataReader paidReader = ClinicDatabase.ExecuteReader(@"SELECT 
                            (SELECT SUM(payment) 
                             FROM tbl_payment_voucher_details 
                             WHERE inv_code = 'Vendor Opening Balance' AND hum_id = @id) AS amount",
                ClinicDatabase.CreateParameterNew("@id", cmbVendor.SelectedValue)))

                if (paidReader.Read() && paidReader["amount"].ToString() != "")
                    vendorTotalBalance = decimal.Parse(paidReader["amount"].ToString());

            // 3️⃣ Add opening balance row if there's an unpaid amount
            if (vendorTotalOB != vendorTotalBalance)
            {
                decimal pendingOB = vendorTotalOB - vendorTotalBalance;
                if (pendingOB != 0)
                {
                    dgvInv.Rows.Add(++counter, cmbVendor.SelectedValue, "", vendorOBDate, "Vendor Opening Balance", pendingOB, 0, 0);
                }
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT ROW_NUMBER() OVER (
                ORDER BY tbl_purchase.date) AS SN, 
                 tbl_purchase.date AS DATE, tbl_purchase.id, tbl_purchase.invoice_id AS 'INV NO', 
                 tbl_purchase.change
                FROM tbl_purchase
                INNER JOIN tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id
                WHERE tbl_purchase.state = 0 AND tbl_purchase.change <> 0 AND tbl_vendor.id = @id
                GROUP BY tbl_purchase.id, tbl_purchase.date;",
            ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                while (reader.Read())
                    dgvInv.Rows.Add((int.Parse(reader["SN"].ToString()) + counter).ToString(), cmbVendor.SelectedValue, reader["id"].ToString(),
                    DateTime.Parse(reader["DATE"].ToString()).ToShortDateString(), reader["INV NO"].ToString(), reader["change"].ToString(), 0, 0, "");

            AutoFillCheckNameVendor();
        }

        private void txtVendor_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id,CODE from tbl_vendor where code =@code",
                  ClinicDatabase.CreateParameterNew("code", txtVendor.Text)))
                if (reader.Read())
                    cmbVendor.SelectedValue = int.Parse(reader["id"].ToString());

            if (txtVendor.Focused)
            {
                string input = txtVendor.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_vendor 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion { Id = Convert.ToInt32(row["id"]), Code = row["code"].ToString(), Name = row["name"].ToString() });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtVendor.Parent.PointToScreen(txtVendor.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtVendor.Height,
                        txtVendor.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtVendor;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void dgvInv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dgvInv.Columns[e.ColumnIndex].Name == "chkPay" && e.RowIndex >= 0)
            {
                bool isChecked = Convert.ToBoolean(dgvInv.Rows[e.RowIndex].Cells["chkPay"].Value);

                if (isChecked)
                {
                    dgvInv.Rows[e.RowIndex].Cells["chkPay"].Value = false;
                }
                else
                {
                    dgvInv.Rows[e.RowIndex].Cells["chkPay"].Value = true;
                }
            }
        }

        private void txtVendor_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id,CODE from tbl_vendor where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtVendor.Text)))
                if (!reader.Read())
                    cmbVendor.SelectedIndex = -1;

            BeginInvoke((Action)(() =>
            {
                if (!lstAccountSuggestions.Focused)
                    lstAccountSuggestions.Visible = false;
            }));
        }
        // Called from ItemCheck (employee selection)
        private void AutoFillCheckNameEmployee()
        {
            foreach (var item in lstOfEmp.CheckedItems)
            {
                if (item is ListItemModel empItem && !string.IsNullOrEmpty(empItem.Name) && empItem.Name.Contains("-"))
                {
                    // Split into code and name
                    string[] parts = empItem.Name.Split('-');
                    if (parts.Length >= 2)
                    {
                        txtCheckName.Text = parts[1].Trim(); // Only the first checked name
                        return;
                    }
                }
            }
            //foreach (var item in lstOfEmp.CheckedItems)
            //{
            //    var rowView = item as DataRowView;
            //    if (rowView != null && rowView["name"].ToString().Contains("-"))
            //    {
            //        txtCheckName.Text = rowView["name"].ToString().Split('-')[1].Trim();
            //        return; // only the first checked name
            //    }
            //}

            txtCheckName.Text = "";
        }
        private void TickFillCheckNameEmployee()
        {
            // Clear previous checks if needed
            for (int i = 0; i < lstOfEmp.Items.Count; i++)
            {
                if (lstOfEmp.Items[i] is ListItemModel empItem)
                {
                    int empId = empItem.Id; // Access the Id directly

                    // Loop through DataGridView rows
                    foreach (DataGridViewRow row in dgvInv.Rows)
                    {
                        if (row.Cells["humId"].Value != null &&
                            int.TryParse(row.Cells["humId"].Value.ToString(), out int humId))
                        {
                            if (humId == empId)
                            {
                                // Check this employee in the list
                                lstOfEmp.SetItemChecked(i, true);
                                break; // No need to continue checking this employee
                            }
                        }
                    }
                }
            }
        }
        private void AutoFillCheckNameVendor()
        {
            if (cmbMethod.Text != "Cheque")
                return;

            if (cmbPaymentType.Text == "Vendor" && cmbVendor.SelectedValue != null)
            {
                using (var reader = ClinicDatabase.ExecuteReader(
                    "SELECT name FROM tbl_vendor WHERE id = @id",
                    ClinicDatabase.CreateParameterNew("@id", cmbVendor.SelectedValue)))
                {
                    if (reader.Read())
                    {
                        txtCheckName.Text = reader["name"].ToString();
                    }
                }
            }
            else
            {
                txtCheckName.Text = "";
            }
        }

        public DataTable COMPANYINFO(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT * FROM tbl_company ;", ClinicDatabase.CreateParameterNew("@id", a1));
        }
        public DataTable PaymentVoucherData(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT 1 sn,id,date,CODE AS RvNo,type,method,amount,
                                                  CASE 
                                                    WHEN tbl_payment_voucher.type = 'Customer' THEN (SELECT code FROM tbl_customer WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Vendor' THEN (SELECT code FROM tbl_vendor WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Employee' THEN (SELECT code FROM tbl_employee WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'General' THEN (SELECT code FROM tbl_coa_level_4 WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    ELSE ''
                                                  END AS reciverCode,
                                                    CASE 
                                                    WHEN tbl_payment_voucher.type = 'Customer' THEN (SELECT name FROM tbl_customer WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Vendor' THEN (SELECT name FROM tbl_vendor WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Employee' THEN (SELECT name FROM tbl_employee WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'General' THEN (SELECT name FROM tbl_coa_level_4 WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    ELSE ''
                                                   END AS reciverName,
                                             (SELECT code FROM tbl_coa_level_4 WHERE id=debit_account_id) DebitCode,
                                            (SELECT NAME FROM tbl_coa_level_4 WHERE id=debit_account_id) debitAccount,
                                            (SELECT NAME FROM tbl_sub_cost_center WHERE id=debit_cost_center_id) debitCostCenter,
                                            (SELECT code FROM tbl_coa_level_4 WHERE id=credit_account_id) CreditCode,
                                            (SELECT NAME FROM tbl_coa_level_4 WHERE id=credit_account_id) creditAccount,
                                            (select name FROM tbl_sub_cost_center WHERE id=credit_cost_center_id) creditCostCenter,
                                            description ,(select name FROM tbl_bank WHERE id = bank_id) bankName,
                                            (select code FROM tbl_bank_card,tbl_bank WHERE tbl_bank_card.id = bank_account_id AND tbl_bank.id = tbl_bank_card.bank_id) BankCode,
                                            (select account_name FROM tbl_bank_card WHERE id = bank_account_id) bankAccount,
                                            check_name,check_no,check_date,trans_date,trans_name,trans_ref 
                                            FROM tbl_payment_voucher where id=@paymentId;", ClinicDatabase.CreateParameterNew("@paymentId", a1));
        }

        public DataTable PaymentDetail(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT ROW_NUMBER() OVER (ORDER BY d.id) AS sn,d.date,d.inv_id,
                                CASE
                                    WHEN i.type = 'Customer' THEN(SELECT NAME FROM tbl_customer WHERE id = d.hum_id)
                                    WHEN i.type = 'Vendor' THEN (SELECT NAME FROM tbl_vendor WHERE id = d.hum_id)
                                    WHEN i.type = 'Employee' THEN (SELECT NAME FROM tbl_employee WHERE id = d.hum_id)
                                    WHEN i.type = 'General' THEN (SELECT NAME FROM tbl_coa_level_4 WHERE id = d.hum_id)
                                    ELSE 'Unregistered'
                                END AS NAME,d.hum_id,d.inv_code,d.total,d.payment,d.description,(SELECT NAME FROM tbl_sub_cost_center WHERE id = d.cost_center_id) AS costCenterName
                            FROM tbl_payment_voucher_details d,tbl_payment_voucher i WHERE d.payment_id = i.id AND i.id= @paymentId;", ClinicDatabase.CreateParameterNew("@paymentId", a1));
        }

        public DataTable VoucherTransaction(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT ROW_NUMBER() OVER (ORDER BY tr.id) AS sn,
                            (SELECT Code FROM tbl_coa_level_4 WHERE id=tr.account_id)as acCode,
                            (SELECT NAME FROM tbl_coa_level_4 WHERE id=tr.account_id)as acName,
                            tr.debit,tr.credit,
                            (SELECT NAME FROM tbl_sub_cost_center WHERE id = (select cost_center_id FROM tbl_payment_voucher_details WHERE payment_id=r.id LIMIT 1)) AS centerName 
                            FROM tbl_transaction tr,tbl_payment_voucher r WHERE tr.transaction_id=r.id and r.id=@paymentId AND tr.TYPE = 'Vendor payment';", ClinicDatabase.CreateParameterNew("@paymentId", a1));
        }
        //public void ShowReport1()
        //{

        //    try
        //    {

        //        // Create the report 
        //        //CPV_GENERAL cr = new CPV_GENERAL();
        //        string reportPath = Path.Combine(Application.StartupPath, "CrystalReport", "PaymentVoucherGeneral.rpt");

        //        // Load the report file from disk
        //        //ReportDocument cr = new ReportDocument();
        //        //cr.Load(reportPath);
        //        // Load the main report data
        //        DataTable companyData = COMPANYINFO("1");  // Assuming you want to pass ID 1
        //        DataTable PaymentDetails = PaymentDetail(id.ToString());
        //        DataTable PaymentVoucher = PaymentVoucherData(id.ToString());
        //        DataTable transaction = VoucherTransaction(id.ToString());
        //        if (companyData != null)  // Ensure that data was successfully retrieved
        //        {
        //            //cr.SetDataSource(companyData);
        //            cr.Subreports["Company"].SetDataSource(companyData);
        //            cr.Subreports["PaymentHeader"].SetDataSource(PaymentVoucher);
        //            cr.Subreports["PaymentDetails"].SetDataSource(PaymentDetails);
        //            cr.Subreports["TransactionHeader"].SetDataSource(transaction);
        //            cr.Subreports["PaymentAcHeader"].SetDataSource(PaymentVoucher);
        //            ((TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["Text6"]).Text = txtDescription1.Text;
        //        }
        //        else
        //        {
        //            MessageBox.Show("No data available for the report.");
        //            return;  // Exit the method if no data is available
        //        }

        //        // Assign the main report to the viewer
        //        string fileName = "PaymentVoucher_" + id + "_" + cmbVendor.Text.Trim();
        //        MasterReportView reportForm = new MasterReportView(cr, fileName);
        //        reportForm.crReportViewer.ReportSource = cr;
        //        reportForm.crReportViewer.RefreshReport();
        //        reportForm.ShowDialog();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message);
        //    }
        //}

        private void guna2TileButton18_Click(object sender, EventArgs e)
        {
            double amount = 0;

            bool hasValidAmount = double.TryParse(txtAmount.Text, out amount) && amount > 0;

            if (id > 0 || hasValidAmount)
            {
                loadPrint(cmbMethod.Text);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

            if (id == 0)
            {
                if (insertPV())
                {
                    EventHub.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Paid  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                    dgvInv.Rows.Clear();
                    loadPaymentDataDetails();
                    txtPVCode.Text = GenerateNextPaymentCode();
                    this.Close();
                }
            }
            else
            {
                if (updatePV())
                {
                    EventHub.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Updated !  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                    dgvInv.Rows.Clear();
                    loadPaymentDataDetails();
                    txtPVCode.Text = GenerateNextPaymentCode();
                    this.Close();
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal; // Restore down
            }
            else
            {
                this.WindowState = FormWindowState.Maximized; // Maximize
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void ShowReport2()
        {
            try
            {
                // Create the report 
                //cpvcheq cr = new cpvcheq();
                string reportPath = Path.Combine(Application.StartupPath, "CrystalReport", "PaymentVoucherCheque.rpt");

                // Load the report file from disk
                //ReportDocument cr = new ReportDocument();
                //cr.Load(reportPath);
                // Load the main report data
                DataTable companyData = COMPANYINFO("1");
                DataTable PaymentVoucher = PaymentVoucherData(id.ToString());
                DataTable transaction = VoucherTransaction(id.ToString());
                DataTable PaymentDetails = PaymentDetail(id.ToString());
                if (companyData != null)
                {
                    //cr.Subreports["Company"].SetDataSource(companyData);
                    //cr.Subreports["PaymentHeader"].SetDataSource(PaymentVoucher);
                    //cr.Subreports["TransactionHeader"].SetDataSource(transaction);
                    //cr.Subreports["AccountHeader"].SetDataSource(PaymentVoucher);
                    ////cr.Subreports["PaymentDetails"].SetDataSource(PaymentDetails);
                    //cr.Subreports["ChequeDetails"].SetDataSource(PaymentVoucher);
                    //((TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["Text6"]).Text = txtDescription1.Text;
                }
                else
                {
                    MessageBox.Show("No data available for the report.");
                    return;  // Exit the method if no data is available
                }

                // Assign the main report to the viewer
                string fileName = "PaymentVoucher_" + id + "_" + cmbVendor.Text.Trim();
                //MasterReportView reportForm = new MasterReportView(cr, fileName);
                //reportForm.crReportViewer.ReportSource = cr;
                //reportForm.crReportViewer.RefreshReport();
                //reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtId.Text); // Utilities.GetVoucherIdFromCode(txtPVCode.Text);
            if (currentId == null || currentId <= 1)
                return;

            currentId = currentId - 1;
            txtId.Text = currentId.ToString();
            if (currentId <= 0)
            {
                clear();
                MessageBox.Show("No previous records.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string query = "select id from tbl_payment_voucher where state = 0 and id =@id";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("@id", currentId)))
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindVoucher();
                }
                else
                {
                    clear();
                    MessageBox.Show("No previous record found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtId.Text); // Utilities.GetVoucherIdFromCode(txtPVCode.Text);
            if (currentId is null) return;

            currentId = currentId + 1;
            txtId.Text = currentId.ToString();
            string query = "SELECT id FROM tbl_payment_voucher WHERE state = 0 AND id =@id";
            using (var reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("@id", currentId)))
            {
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindVoucher();
                }
                else
                {
                    clear();
                    MessageBox.Show("No next record found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtPVCode.Text = GenerateNextPaymentCode();
            clear();
        }
        private void clear()
        {
            dgvInv.Rows.Clear();
            defaultCustomerId = 0;
            totalAmount = 0;
            id = 0;
        }

        private void BtnSaveNew_Click(object sender, EventArgs e)
        {
            btnSave.PerformClick();
            dgvInv.Rows.Clear();
            txtPVCode.Text = GenerateNextPaymentCode();
            loadPaymentDataDetails();
            clear();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            //ClinicDatabase.ExecuteNonQuery("UPDATE tbl_payment_voucher SET state = -1 WHERE id = @id ",
            //                              ClinicDatabase.CreateParameterNew("id", id.ToString()));

            // 1. Read all records before deletion
            DataTable dtVoucher = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_payment_voucher WHERE id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            DataTable dtDetails = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_payment_voucher_details WHERE payment_id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            DataTable dtTransaction = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_transaction WHERE transaction_id = @id AND t_type='PAYMENT'",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            // 2. Insert backups into tbl_deleted_records
            foreach (DataRow row in dtVoucher.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                    ClinicDatabase.CreateParameterNew("table", "tbl_payment_voucher"),
                    ClinicDatabase.CreateParameterNew("data", Newtonsoft.Json.JsonConvert.SerializeObject(row)),
                    ClinicDatabase.CreateParameterNew("user", -1)
                );
            }

            foreach (DataRow row in dtDetails.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                    ClinicDatabase.CreateParameterNew("table", "tbl_payment_voucher_details"),
                    ClinicDatabase.CreateParameterNew("data", Newtonsoft.Json.JsonConvert.SerializeObject(row)),
                    ClinicDatabase.CreateParameterNew("user", -1)
                );
            }

            foreach (DataRow row in dtTransaction.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                    ClinicDatabase.CreateParameterNew("table", "tbl_transaction"),
                    ClinicDatabase.CreateParameterNew("data", Newtonsoft.Json.JsonConvert.SerializeObject(row)),
                    ClinicDatabase.CreateParameterNew("user", -1)
                );
            }

            // 3. Now delete records permanently
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_transaction WHERE transaction_id = @id AND t_type='PAYMENT'",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_payment_voucher_details WHERE payment_id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_payment_voucher WHERE id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            Utilities.LogAudit(-1, "Payment Voucher Permanently Deleted", "Payment Voucher", id, "Deleted Payment Voucher with ID: " + id);
            clear();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            id = 0;
        }

        private void guna2TileButton25_Click(object sender, EventArgs e)
        {

        }

        private void guna2TileButton26_Click(object sender, EventArgs e)
        {

        }

        bool isDebitCostCenterRefreshing = false;

        private void cmbDebitCostCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cmbDebitCostCenter.Focused)
                return;

            if (isDebitCostCenterRefreshing) return;
            if (cmbDebitCostCenter.SelectedValue != null &&
        (cmbDebitCostCenter.SelectedValue.ToString() == "0" || cmbDebitCostCenter.Text == "<< Add >>"))
            {
                var frm = new frmCostCenter(0);
                frm.ShowDialog();

                isDebitCostCenterRefreshing = true;
                BindCombos.PopulateCostCenter(cmbDebitCostCenter, true);
                cmbDebitCostCenter.SelectedIndex = cmbDebitCostCenter.Items.Count - 1;
                isDebitCostCenterRefreshing = false;
            }
        }

        bool isCreditCostCenterRefreshing = false;

        private void cmbCreditCostCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cmbCreditCostCenter.Focused)
                return;
            if (isCreditCostCenterRefreshing) return;

            if (cmbCreditCostCenter.SelectedValue != null &&
        (cmbCreditCostCenter.SelectedValue.ToString() == "0" || cmbCreditCostCenter.Text == "<< Add >>"))
            {
                var frm = new frmCostCenter(0);
                frm.ShowDialog();
                isCreditCostCenterRefreshing = true;
                BindCombos.PopulateCostCenter(cmbCreditCostCenter, true);
                cmbCreditCostCenter.SelectedIndex = cmbCreditCostCenter.Items.Count - 1;
                isCreditCostCenterRefreshing = false;
            }
        }


        //private bool _suppressTextChanged = false;
        //private bool _isSelectingFromList = false;

        //private void ShowAccountSuggestions0(TextBox tb)
        //{
        //    string input = tb.Text.Trim();

        //    if (string.IsNullOrEmpty(input))
        //    {
        //        lstAccountSuggestions.Visible = false;
        //        return;
        //    }

        //    string query = @"SELECT code, name FROM tbl_coa_level_4 
        //             WHERE code LIKE @search OR name LIKE @search LIMIT 20";

        //    DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

        //    lstAccountSuggestions.Items.Clear();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        lstAccountSuggestions.Items.Add(new ItemSuggestion
        //{
        //    Id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
        //    Code = row["code"].ToString(),
        //    Name = row["name"].ToString()
        //});
        //    }

        //    if (lstAccountSuggestions.Items.Count > 0)
        //    {
        //        lstAccountSuggestions.Visible = true;

        //        // Position the listbox just below the textbox
        //        Point locationOnForm = tb.Parent.PointToScreen(tb.Location);
        //        Point locationRelativeToForm = this.PointToClient(locationOnForm);

        //        lstAccountSuggestions.SetBounds(
        //            locationRelativeToForm.X,
        //            locationRelativeToForm.Y + tb.Height,
        //            tb.Width + 100,
        //            120
        //        );

        //        lstAccountSuggestions.BringToFront();

        //        // Optional: store current TextBox for selection use
        //        lstAccountSuggestions.Tag = tb;
        //    }
        //    else
        //    {
        //        lstAccountSuggestions.Visible = false;
        //    }
        //}

        //private void ShowAccountSuggestions1(Guna2TextBox tb)
        //{
        //    string input = tb.Text.Trim();

        //    if (string.IsNullOrEmpty(input))
        //    {
        //        lstAccountSuggestions.Visible = false;
        //        return;
        //    }

        //    string query = @"SELECT code, name FROM tbl_coa_level_4 
        //             WHERE code LIKE @search OR name LIKE @search LIMIT 20";

        //    DataTable dt = ClinicDatabase.ExecuteDataTable(query,
        //        ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

        //    lstAccountSuggestions.Items.Clear();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        lstAccountSuggestions.Items.Add(new ItemSuggestion
        //{
        //    Id = row["id"] != DBNull.Value ? Convert.ToInt32(row["id"]) : 0,
        //    Code = row["code"].ToString(),
        //    Name = row["name"].ToString()
        //});
        //    }

        //    if (lstAccountSuggestions.Items.Count > 0)
        //    {
        //        Point locationOnForm = tb.Parent.PointToScreen(tb.Location);
        //        Point locationRelativeToForm = this.PointToClient(locationOnForm);

        //        lstAccountSuggestions.SetBounds(
        //            locationRelativeToForm.X,
        //            locationRelativeToForm.Y + tb.Height,
        //            tb.Width + 100,
        //            120
        //        );

        //        lstAccountSuggestions.Tag = tb;
        //        lstAccountSuggestions.Visible = true;
        //        lstAccountSuggestions.BringToFront();
        //        // ❌ Do NOT call Focus here
        //    }
        //    else
        //    {
        //        lstAccountSuggestions.Visible = false;
        //    }
        //}

        //private void lstAccountSuggestions_Click(object sender, EventArgs e)
        //{
        //    if (lstAccountSuggestions.SelectedItem != null && lstAccountSuggestions.Tag is TextBox tb)
        //    {
        //        tb.Text = lstAccountSuggestions.SelectedItem.ToString().Split('-')[0].Trim(); // Set only the code part
        //        lstAccountSuggestions.Visible = false;
        //        tb.Focus(); // bring focus back to textbox
        //    }
        //    else if(lstAccountSuggestions.SelectedItem != null && lstAccountSuggestions.Tag is Guna2TextBox tb1)
        //    {
        //        tb1.Text = lstAccountSuggestions.SelectedItem.ToString().Split('-')[0].Trim(); // Set only the code part
        //        lstAccountSuggestions.Visible = false;
        //        tb1.Focus(); // bring focus back to textbox
        //    }
        //}

        private void lstAccountSuggestions_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    lstAccountSuggestions_Click(sender, e);
            //    e.Handled = true;
            //}

            //int index = lstAccountSuggestions.IndexFromPoint(e.Location);
            //if (index != ListBox.NoMatches)
            //{
            //    lstAccountSuggestions.SelectedIndex = index;
            //    string selectedCode = lstAccountSuggestions.SelectedItem.ToString().Split('-')[0].Trim();

            //    if (lstAccountSuggestions.Tag is Guna2TextBox tb)
            //    {
            //        _isSelectingFromList = true;
            //        _suppressTextChanged = true;
            //        tb.Text = selectedCode;
            //        _suppressTextChanged = false;
            //        _isSelectingFromList = false;
            //    }

            //    lstAccountSuggestions.Visible = false;
            //}
        }

        private void lstAccountSuggestions_Click(object sender, EventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem != null && lstAccountSuggestions.Tag is Guna2TextBox targetTextBox)
            {
                string selected = lstAccountSuggestions.SelectedItem.ToString();
                string selectedCode = "";// selected.Split('-')[0].Trim();
                int selectedId = 0;
                if (lstAccountSuggestions.SelectedItem is ItemSuggestion item)
                {
                    selectedId = item.Id;
                    selectedCode = item.Code;
                }

                targetTextBox.Text = selectedCode;
                lstAccountSuggestions.Visible = false;

                // Trigger TextChanged manually if needed
                targetTextBox.Focus();
                targetTextBox.SelectionStart = targetTextBox.Text.Length;

                // Example of post-selection logic:
                if (targetTextBox == txtDebitCode)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                        ClinicDatabase.CreateParameterNew("code", selectedCode)))
                        if (reader.Read())
                            cmbDebitAccountName.SelectedValue = int.Parse(reader["id"].ToString());
                }
                else if (targetTextBox == txtCreditAccountCode)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                        ClinicDatabase.CreateParameterNew("code", selectedCode)))
                        if (reader.Read())
                            cmbCreditAccountName.SelectedValue = int.Parse(reader["id"].ToString());
                }
                else if (targetTextBox == txtVendor)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id from tbl_vendor where code =@code",
                        ClinicDatabase.CreateParameterNew("code", selectedCode)))
                        if (reader.Read())
                            cmbVendor.SelectedValue = int.Parse(reader["id"].ToString());
                }
            }

            //if (lstAccountSuggestions.SelectedItem != null)
            //{
            //    txtDebitCode.Text = lstAccountSuggestions.SelectedItem.ToString();
            //    lstAccountSuggestions.Visible = false;

            //    MessageBox.Show("You selected: " + txtDebitCode.Text);
            //}
        }

        private void cbpettycash_CheckedChanged(object sender, EventArgs e)
        {
            if (cbpettycash.Checked == true )
            {
                //lspettycashemployee.Visible = true;
                //lstOfEmp.Visible = true;
                //else if (cmbMethod.Text == "Petty Cash")
                //{
                    //pnlBank.Visible = false;
                    //pnlTrans.Visible = true;
                    //using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_level_4 WHERE main_id = (SELECT id FROM tbl_coa_level_3 WHERE NAME = 'Petty') LIMIT 1;"))
                    //    if (reader.Read())
                    //    {
                    //        txtCreditAccountCode.Text = reader["code"].ToString();
                    //        string accountId = reader["id"].ToString();
                    //        if (!string.IsNullOrEmpty(accountId))
                    //        {
                    //            cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                    //        }
                    //    }
                //}
            }
            else
            {
                //lspettycashemployee.Visible = false;
                lstOfEmp.Visible = false;
            }

        }

        private void CbSubcontractors_CheckedChanged(object sender, EventArgs e)
        {
            isSubContractors= CbSubcontractors.Checked;
            VendorType = isSubContractors ? "Subcontractor" : "Vendor";
            if (cmbPaymentType.Text == "Vendor" && cmbVendor.SelectedValue != null)
            {
                BindCombos.PopulateVendors(cmbVendor, false, false, VendorType);
            }
        }

        private void dgvInv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvInv.CurrentCell.ColumnIndex == dgvInv.Columns["Pay"].Index)
            {
                TextBox txt = e.Control as TextBox;
                if (txt != null)
                {
                    txt.KeyPress -= Txt_KeyPress;
                    txt.KeyPress += Txt_KeyPress;
                }
            }
        }

        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (backspace, delete, etc.)
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.')
            {
                e.Handled = true; // Block the input
            }

            // Allow only one decimal point
            TextBox txt = sender as TextBox;
            if (e.KeyChar == '.' && txt.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void lstOfEmp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lstOfEmp.Focused)
                return; // Ignore changes not made by user interaction

            if (lstOfEmp.SelectedIndex == -1)
            {
                txtCheckName.Text = "";
                return;
            }

            txtCheckName.Text = lstOfEmp.Text;

        }

        private void lstOfEmp_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!lstOfEmp.Focused)
                return;

            ListItemModel empItem = lstOfEmp.Items[e.Index] as ListItemModel;
            if (empItem == null)
                return;

            string fullName = empItem.Name ?? "";
            string[] nameParts = fullName.Split('-');
            if (nameParts.Length < 2) return;

            string empCode = nameParts[0].Trim();
            string empName = nameParts[1].Trim();
            int empId = empItem.Id;

            if (e.NewValue == CheckState.Checked)
            {

                var _idd = "0";


                /****Petty Cash****/
                if (cbpettycash.Visible && cbpettycash.Enabled)
                {
                    using (MySqlDataReader pReader1 = ClinicDatabase.ExecuteReader(@"SELECT pcr.id, pcr.`change`,request_date,pcr.amount AS total FROM tbl_petty_cash_request pcr 
                                                 INNER JOIN tbl_employee te ON pcr.Petty_cash_name = te.id
                                                 WHERE te.id = " + empId + " AND pcr.state = 'Approved'"
                                             ))

                        while (pReader1.Read())
                        {
                            decimal total = 0;
                            if (pReader1["total"] != DBNull.Value &&
                                decimal.TryParse(pReader1["total"].ToString(), out total) && total > 0)
                            {
                                dgvInv.Rows.Add(dgvInv.Rows.Count + 1, empId, pReader1["id"].ToString(), pReader1["request_date"].ToString(),
                                                fullName.ToString(), Utilities.FormatDecimal(total),
                                                0, 0, "", "Petty Cash Request");
                            }
                        }
                }
                else
                {

                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                        @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_attendance_salary.id) AS SN,  
                          tbl_attendance_salary.* 
                          FROM tbl_attendance_salary 
                          WHERE emp_code = @code AND `change` <> 0",
                        ClinicDatabase.CreateParameterNew("code", empCode)))
                    {
                        while (reader.Read())
                        {
                            decimal netSalary = Convert.ToDecimal(reader["net_salary"]);
                            decimal totalLoan = Convert.ToDecimal(reader["total_loan"]);
                            decimal paid = Convert.ToDecimal(reader["Pay"]);
                            decimal finalAmount = netSalary - totalLoan - paid;

                            dgvInv.Rows.Add(
                                reader["SN"].ToString(),     // Serial number
                                empId,                        // Employee ID
                                reader["id"].ToString(),      // Salary record ID
                                reader["date"].ToString(),    // Date
                                empName,                      // Employee Name
                                Utilities.FormatDecimal(finalAmount), // Remaining Salary
                                0,
                                0,
                                "",
                                "Salary"
                            );

                            _idd = reader["SN"].ToString();
                        }
                    }

                    // Leave Salary
                    //MySqlDataReader reade = ClinicDatabase.ExecuteReader(
                    //     "SELECT SUM(tl.credit) - SUM(tl.debit) AS total " +
                    //     "FROM tbl_leave_salary tl " +
                    //     "INNER JOIN tbl_employee te ON tl.code = te.code " +
                    //     "WHERE te.id = " + empId);


                    //if (reade.Read())
                    //{
                    //    decimal total = 0;
                    //    if (reade["total"] != DBNull.Value &&
                    //        decimal.TryParse(reade["total"].ToString(), out total) && total > 0)
                    //    {
                    //        dgvInv.Rows.Add(no, empId, _idd, DateTime.Now.Date, rowView["name"].ToString(),
                    //                        total.ToString("F2"), false, "", "Leave Salary");
                    //    }
                    //}
                    //            MySqlDataReader Ereade = ClinicDatabase.ExecuteReader(
                    //"SELECT SUM(tl.credit) - SUM(tl.debit) AS total " +
                    //"FROM tbl_end_of_service tl " +
                    //"INNER JOIN tbl_employee te ON tl.code = te.code " +
                    //"WHERE te.id = " + empId);

                    //            if (Ereade.Read())
                    //            {
                    //                decimal total = 0;
                    //                if (Ereade["total"] != DBNull.Value &&
                    //                    decimal.TryParse(Ereade["total"].ToString(), out total) && total > 0)
                    //                {
                    //                    dgvInv.Rows.Add(no, empId, _idd, DateTime.Now.Date, rowView["name"].ToString(),
                    //                                    total.ToString("F2"), false, "", "End Of Service");
                    //                }
                    //            }


                    // Employee Loan
            //        MySqlDataReader pReader = ClinicDatabase.ExecuteReader(@"
            //SELECT 
            //ROW_NUMBER() OVER (ORDER BY loanDate) AS SN, 
            //id, LoanDate, RequestAmount AS Amount, `Change` 
            //FROM tbl_loan 
            //WHERE EmployeeID = @code AND `change` > 0 
            //LIMIT 1;",
            //        ClinicDatabase.CreateParameterNew("code", empCode));

            //        while (pReader.Read())
            //        {
            //            dgvInv.Rows.Add(
            //                pReader["SN"].ToString(),
            //                empId,
            //                pReader["id"].ToString(),                       // invId (Loan ID)
            //                pReader["LoanDate"].ToString(),                 // invDate
            //                fullName,                     // InvNo (e.g. "30001 - John")
            //                Utilities.FormatDecimal(pReader["Change"]),     // Total (Amount to pay)
            //                false,                                          // humId
            //                "",                                             // Pay (you can set = Change if needed)
            //                "",
            //                "Employee Loan Payment"// Description
            //            );

            //            _idd = pReader["SN"].ToString();
            //        }
            //        pReader.Dispose();
                }
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                for (int i = dgvInv.Rows.Count - 1; i >= 0; i--)
                    if (dgvInv.Rows[i].Cells["humId"].Value != null && int.Parse(dgvInv.Rows[i].Cells["humId"].Value.ToString()) == empId)
                        dgvInv.Rows.RemoveAt(i);

                calculateTotal();
            }
            dgvInv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.BeginInvoke((MethodInvoker)delegate
            {
                AutoFillCheckNameEmployee();
            });
        }
        static string filePath = "";

        //private void LocalizePaymentVoucherComboBoxes()
        //{
        //    LocalizeComboBox(cmbPaymentType);
        //    LocalizeComboBox(cmbMethod);

        //    LocalizeComboBox(cmbDebitAccountName);
        //    LocalizeComboBox(cmbCreditAccountName);

        //    LocalizeComboBox(cmbDebitCostCenter);
        //    LocalizeComboBox(cmbCreditCostCenter);

        //    LocalizeComboBox(cmbBankAccountName);
        //}

        //private void LocalizeComboBox(ComboBox combo)
        //{
        //    combo.FormattingEnabled = true;

        //    combo.Format -= PaymentCombo_Format;
        //    combo.Format += PaymentCombo_Format;
        //}

        private void PaymentCombo_Format(
            object sender, ListControlConvertEventArgs e)
        {
            string text = Convert.ToString(e.Value);

            e.Value = Clinic.TranslationClass.Translate(
                text,
                Clinic.TranslationClass.IsArabic);
        }
        private void BtnAttach_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
            }
        }
        private void AddFileData(int fId)
        {
            AttachmentManager.SaveAttachment(
                    "PAYMENT VOUCHER",
                    fId,
                    openFileDialog1.FileName
                );
        }

      
    }
}