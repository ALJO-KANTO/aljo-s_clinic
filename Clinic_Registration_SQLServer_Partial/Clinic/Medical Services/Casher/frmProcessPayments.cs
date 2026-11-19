
using Clinic_Business;
using Clinic_Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using MySql.Data.MySqlClient;
using Clinic_DataAccess;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Casher
{
    public partial class frmProcessPayments : Form
    {
        private int _BillID;



        public frmProcessPayments(int BillID)
        {
            InitializeComponent();

            InitializeAccountPaymentOptions();
            ApplyPaymentTranslations();

            lbCurrentUser.Text = clsGlobal.PersonName;

            _BillID = BillID;

            LoadPaymentScreenDetails(BillID);
        }

        public frmProcessPayments()
        {
            InitializeComponent();

            InitializeAccountPaymentOptions();
            ApplyPaymentTranslations();

            lbCurrentUser.Text = clsGlobal.PersonName;
        }


        // Account selector is added in code so the existing Designer remains intact.
        private ComboBox cmbPaymentAccount;
        private Label lblPaymentAccount;
        private DataTable _paymentAccounts;

        private void InitializeAccountPaymentOptions()
        {
            // All radio buttons MUST share one parent to be mutually exclusive.
            pnlPaymentMethods.Location = new Point(109, 317);
            pnlPaymentMethods.Size = new Size(812, 49);
            pnlPaymentMethods.BackColor = Color.Transparent;

            pnlPaymentMethods.Controls.Add(rdbCash);
            pnlPaymentMethods.Controls.Add(rdbCreditCard);
            pnlPaymentMethods.Controls.Add(radioButton1);
            pnlPaymentMethods.Controls.Add(rdbBank);
            rdbCash.Location = new Point(16, 7);
            rdbCreditCard.Location = new Point(168, 7);
            radioButton1.Location = new Point(360, 7);
            rdbBank.Location = new Point(620, 7);
            radioButton1.Paint += radioButton_Paint;
            radioButton1.CheckedChanged += RadioB_CheckedChanged;
            pictureBox1.Visible = false; // remove old Payment App image
            rdbBank.Text = "Bank";
            rdbBank.CheckedChanged += PaymentAccountMethodChanged;
            rdbCash.CheckedChanged += PaymentAccountMethodChanged;
            rdbCreditCard.CheckedChanged += PaymentAccountMethodChanged;
            radioButton1.CheckedChanged += PaymentAccountMethodChanged;

            lblPaymentAccount = new Label
            {
                Name = "lblPaymentAccount",
                Text = "Account Name",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(125, 375)
            };
            cmbPaymentAccount = new ComboBox
            {
                Name = "cmbPaymentAccount",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F),
                Location = new Point(310, 373),
                Size = new Size(340, 34),
                BackColor = Color.FromArgb(67, 72, 75),
                ForeColor = Color.White
            };
            Controls.Add(lblPaymentAccount);
            Controls.Add(cmbPaymentAccount);
            lblPaymentAccount.BringToFront();
            cmbPaymentAccount.BringToFront();
            LoadAllPaymentAccounts();
            PaymentAccountMethodChanged(this, EventArgs.Empty);
        }

        private void LoadAllPaymentAccounts()
        {
            _paymentAccounts = new DataTable();
            using (var connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            using (var command = new MySqlCommand(
                "SELECT id, name FROM tbl_coa_level_4 ORDER BY name, id", connection))
            using (var adapter = new MySqlDataAdapter(command))
            {
                adapter.Fill(_paymentAccounts);
            }
            cmbPaymentAccount.DisplayMember = "name";
            cmbPaymentAccount.ValueMember = "id";
            cmbPaymentAccount.DataSource = _paymentAccounts;
            cmbPaymentAccount.SelectedIndex = -1;
        }

        private void PaymentAccountMethodChanged(object sender, EventArgs e)
        {
            if (cmbPaymentAccount == null || _paymentAccounts == null) return;
            // The full COA stays visible, regardless of payment method.
            string accountName = rdbCash.Checked ? "Cash On Hand"
                : (rdbBank.Checked || rdbCreditCard.Checked) ? "Bank Account"
                : radioButton1.Checked ? "Accounts Receivable" : null;

            // Insurance receivables should use a configured insurance account,
            // not an invented ledger ID. If absent, require manual selection.
            int selectedId = 0;
            if (!string.IsNullOrEmpty(accountName))
            {
                foreach (DataRow row in _paymentAccounts.Rows)
                {
                    if (string.Equals(Convert.ToString(row["name"]), accountName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        selectedId = Convert.ToInt32(row["id"]);
                        break;
                    }
                }
            }
            cmbPaymentAccount.SelectedValue = selectedId > 0 ? (object)selectedId : -1;
            txtAmountReceived_TextChanged(null, null);
        }

        private readonly Dictionary<Control, string> _originalTexts =
            new Dictionary<Control, string>();

        private readonly Dictionary<DataGridViewColumn, string> _originalHeaders =
            new Dictionary<DataGridViewColumn, string>();

        private string T(string text)
        {
            return TranslationClass.Translate(
                text,
                TranslationClass.IsArabic);
        }

        private void ApplyPaymentTranslations()
        {
            bool arabic = TranslationClass.IsArabic;

            SuspendLayout();

            try
            {
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                RightToLeftLayout = arabic;

                TranslateControls(this, arabic);
                TranslateBillGridHeaders();
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        private void TranslateControls(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                // Do not translate user-entered values.
                bool translatable =
                    control is Label ||
                    control is Button ||
                    control is CheckBox ||
                    control is RadioButton ||
                    control is GroupBox;

                if (translatable)
                {
                    if (!_originalTexts.ContainsKey(control))
                        _originalTexts[control] = control.Text;

                    string english = _originalTexts[control];

                    if (!string.IsNullOrWhiteSpace(english))
                    {
                        control.Text = arabic
                            ? T(english)
                            : english;
                    }

                    control.RightToLeft = arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                    if (control is Label label)
                    {
                        // Preserve centered labels.
                        if (label.TextAlign != ContentAlignment.MiddleCenter)
                        {
                            label.TextAlign = arabic
                                ? ContentAlignment.MiddleRight
                                : ContentAlignment.MiddleLeft;
                        }
                    }
                }

                if (control.HasChildren)
                    TranslateControls(control, arabic);
            }
        }

        private void TranslateBillGridHeaders()
        {
            if (dgvBillDetails == null)
                return;

            bool arabic = TranslationClass.IsArabic;

            foreach (DataGridViewColumn column in dgvBillDetails.Columns)
            {
                if (!_originalHeaders.ContainsKey(column))
                    _originalHeaders[column] = column.HeaderText;

                string english = _originalHeaders[column];

                column.HeaderText = arabic
                    ? T(english)
                    : english;
            }

            dgvBillDetails.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            dgvBillDetails.ColumnHeadersDefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
        }


        private void ShowTranslatedMessage(
            string message,
            string title,
            MessageBoxIcon icon)
        {
            MessageBox.Show(
                T(message),
                T(title),
                MessageBoxButtons.OK,
                icon);
        }

        private void LoadPaymentScreenDetails(int billID)
        {
            try
            {
                clsBillingService paymentSummary =
                    new clsBillingService(billID);

                if (!paymentSummary.IsFound)
                {
                    ShowTranslatedMessage(
                        "Bill not found. Bill ID: " + billID,
                        "Bill Not Found",
                        MessageBoxIcon.Warning);

                    btnConfirmPayment.Enabled = false;
                    return;
                }

                txtSerachByBillID.Text =
                    paymentSummary.BillNumber;

                lblSubtotal.Text =
                    paymentSummary.TotalBeforeDiscountAndTax
                        .ToString("N2");

                lblMedicines.Text =
                    paymentSummary.MedicinesTotal.ToString("N2");

                lblServices.Text =
                    paymentSummary.ServicesTotal.ToString("N2");

                lblAppoinmentFees.Text =
                    paymentSummary.AppointmentFees.ToString("N2");

                lblTax.Text =
                    paymentSummary.TotalTax.ToString("N2");

                lblDiscount.Text =
                    paymentSummary.TotalDiscount.ToString("N2");

                txtDiscount.Text =
                    paymentSummary.TotalDiscount.ToString("N2");

                lblFinalTotal.Text =
                    paymentSummary.FinalTotalIncludingAll
                        .ToString("N2");

                lbTotalBillAmount.Text =
                    paymentSummary.FinalTotalIncludingAll
                        .ToString("N2");

                lblPaymentAmmount.Text =
                    paymentSummary.PaymentAmount.ToString("N2");

                lblFinalPaymentDue.Text =
                    paymentSummary.BalanceDue.ToString("N2");

                txtAmountToPay.Text =
                    paymentSummary.BalanceDue.ToString("N2");

                lbAmmountToPay.Text =
                    paymentSummary.BalanceDue.ToString("N2");

                dgvBillDetails.DataSource =
                    paymentSummary.BillDetailsTable;

                TranslateBillGridHeaders();

                btnConfirmPayment.Enabled =
                    paymentSummary.BalanceDue > 0;
            }
            catch (Exception ex)
            {
                btnConfirmPayment.Enabled = false;

                MessageBox.Show(
                    ex.ToString(),
                    T("Unable to Load Payment Details"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lbPlaceHolederBillID_Click(
            object sender, EventArgs e)
        {
            txtSerachByBillID.Focus();
        }

        private void radioButton_Paint(
            object sender, PaintEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            e.Graphics.Clear(Color.FromArgb(20, 43, 50));

            e.Graphics.SmoothingMode =
                System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (Brush textBrush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString(
                    radioButton.Text,
                    radioButton.Font,
                    textBrush,
                    20,
                    0);
            }

            Rectangle outerCircle =
                new Rectangle(2, 4, 14, 14);

            using (Brush bgBrush =
                   new SolidBrush(Color.FromArgb(28, 58, 68)))
            using (Pen borderPen =
                   new Pen(Color.FromArgb(0, 210, 190), 1.5f))
            {
                e.Graphics.FillEllipse(bgBrush, outerCircle);
                e.Graphics.DrawEllipse(borderPen, outerCircle);
            }

            if (radioButton.Checked)
            {
                Rectangle innerDot =
                    new Rectangle(6, 8, 6, 6);

                using (Brush dotBrush =
                       new SolidBrush(Color.White))
                {
                    e.Graphics.FillEllipse(dotBrush, innerDot);
                }
            }
        }

        private void txtSerachByBillID_TextChanged(
            object sender, EventArgs e)
        {
            lbPlaceHolederBillID.Visible =
                string.IsNullOrEmpty(txtSerachByBillID.Text);
        }

        private void frmProcessPayments_Load(
            object sender, EventArgs e)
        {
            ApplyPaymentTranslations();
        }

        private void btnConfirmPayment_Click(
            object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                ShowTranslatedMessage(
                    "Please fill all required fields correctly.",
                    "Validation Error",
                    MessageBoxIcon.Error);

                return;
            }

            string cleanAmountToPay =
                txtAmountToPay.Text.Replace("$", "").Trim();

            string cleanAmountReceived =
                txtAmountReceived.Text.Replace("$", "").Trim();

            if (!decimal.TryParse(
                    cleanAmountToPay, out decimal amountToPay) ||
                !decimal.TryParse(
                    cleanAmountReceived, out decimal amountReceived))
            {
                ShowTranslatedMessage(
                    "Invalid financial numbers. Please check the amounts.",
                    "Input Error",
                    MessageBoxIcon.Error);

                return;
            }

            decimal minimumRequired = amountToPay * 0.70m;

            if ((rdbBank.Checked || rdbCreditCard.Checked || radioButton1.Checked) && amountReceived > amountToPay)
            {
                ShowTranslatedMessage("Non-cash payment cannot exceed the amount due.", "Input Error", MessageBoxIcon.Warning);
                return;
            }

            if (amountReceived < minimumRequired)
            {
                string message =
                    TranslationClass.IsArabic
                    ? "تم رفض الدفع! الحد الأدنى المطلوب لإتمام الدفع هو 70٪ (" +
                      minimumRequired.ToString("N2") + ")."
                    : "Payment rejected! The minimum required amount to process this payment is 70% (" +
                      minimumRequired.ToString("N2") + ").";

                MessageBox.Show(
                    message,
                    T("Policy Restriction"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string confirmationMessage =
                TranslationClass.IsArabic
                ? "هل أنت متأكد من تأكيد هذا الدفع؟"
                : "Are you sure you want to confirm this payment?";

            if (MessageBox.Show(
                    confirmationMessage,
                    T("Confirm Payment"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            string paymentMethod = rdbCash.Checked ? "Cash"
                : rdbBank.Checked ? "Bank"
                : rdbCreditCard.Checked ? "Credit Card"
                : "Insurance";

            if (cmbPaymentAccount.SelectedValue == null ||
                !int.TryParse(Convert.ToString(cmbPaymentAccount.SelectedValue), out int debitAccountId)
                || debitAccountId <= 0)
            {
                ShowTranslatedMessage("Select a valid account name.", "Accounting Configuration", MessageBoxIcon.Warning);
                return;
            }

            int creditAccountId =
                BindCombos.SelectDefaultLevelAccount(
                    "Patient");

            if (debitAccountId <= 0 || creditAccountId <= 0)
            {
                MessageBox.Show(
                    "Cash/Bank or Accounts Receivable account is not configured.",
                    "Accounting Configuration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            clsPayment Payment = new clsPayment();

            Payment.BillID = _BillID;
            Payment.PaymentMethod = paymentMethod;
            Payment.DebitAccountID = debitAccountId;
            Payment.CreditAccountID = creditAccountId;

            if (amountReceived >= amountToPay)
                Payment.Status =
                    enPaymetnStatus.Paid;
            else
                Payment.Status =
                    enPaymetnStatus.Partial;

            Payment.PaymentAmount = amountReceived;
            Payment.PaymentDate = DateTime.Now;
            Payment.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            try
            {

                if (Payment.Save())
                {
                    ShowTranslatedMessage(
                        "Payment has been processed successfully!",
                        "Success",
                        MessageBoxIcon.Information);

                    // Clear the previous payment amount.
                    txtAmountReceived.Text = "";

                    // Refresh the invoice and outstanding balance.
                    LoadPaymentScreenDetails(_BillID);

                    // Do not allow another submission until a new amount is entered.
                    btnConfirmPayment.Enabled = true;
                }
                else
                {
                    ShowTranslatedMessage(
                        "Payment.Save() returned false. Please check clsPayment.Save().",
                        "Payment Error",
                        MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    T("Payment Database Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void txtAmountReceived_TextChanged(
            object sender, EventArgs e)
        {
            string cleanAmountToPay =
                txtAmountToPay.Text.Replace("$", "").Trim();

            string cleanAmountReceived =
                txtAmountReceived.Text.Replace("$", "").Trim();

            if (!decimal.TryParse(
                    cleanAmountToPay, out decimal amountToPay))
                return;

            if (decimal.TryParse(
                    cleanAmountReceived, out decimal amountReceived))
            {
                if (amountReceived > amountToPay &&
                    (rdbCreditCard.Checked || rdbBank.Checked || radioButton1.Checked))
                {
                    lblChangeDue.Text = T(
                        "Amount cannot exceed the required total!");

                    lblChangeDue.ForeColor = Color.DarkRed;
                    btnConfirmPayment.Enabled = false;
                    return;
                }

                decimal minimumRequiredAmount =
                    amountToPay * 0.70m;

                if (amountReceived < minimumRequiredAmount)
                {
                    lblChangeDue.Text =
                        T("Minimum 70% payment required!");

                    lblChangeDue.ForeColor = Color.DarkRed;
                    btnConfirmPayment.Enabled = false;
                    return;
                }

                decimal changeDue =
                    amountReceived - amountToPay;

                if (changeDue >= 0)
                {
                    string prefix =
                        TranslationClass.IsArabic
                        ? "المبلغ المتبقي للإرجاع: "
                        : "Change to return: ";

                    lblChangeDue.Text =
                        prefix + changeDue.ToString("N2");

                    lblChangeDue.ForeColor = Color.Green;
                    btnConfirmPayment.Enabled = true;
                }
                else
                {
                    decimal remainingBalance =
                        amountToPay - amountReceived;

                    string prefix =
                        TranslationClass.IsArabic
                        ? "المبلغ المتبقي المستحق: "
                        : "Remaining Debt: ";

                    lblChangeDue.Text =
                        prefix + remainingBalance.ToString("N2");

                    lblChangeDue.ForeColor = Color.OrangeRed;
                    btnConfirmPayment.Enabled = true;
                }
            }
            else
            {
                lblChangeDue.Text = "0.00";
                btnConfirmPayment.Enabled = false;
            }
        }

        private void txtAmountToPay_TextChanged(
            object sender, EventArgs e)
        {
        }

        private void RadioB_CheckedChanged(
            object sender, EventArgs e)
        {
            txtAmountReceived_TextChanged(null, null);
        }
    }
}
