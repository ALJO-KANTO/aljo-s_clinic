using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Clinic.Medical_Services.Casher
{
    public partial class frmProcessPayments : Form
    {
        private int _BillID;
        public frmProcessPayments(int BillID)
        {
            InitializeComponent();
            lbCurrentUser.Text = clsGlobal.PersonName;
            _BillID = BillID;
            LoadPaymentScreenDetails(BillID);
        }
        public frmProcessPayments()
        {
            InitializeComponent();
            lbCurrentUser.Text = clsGlobal.PersonName;
        }

        private void LoadPaymentScreenDetails(int billID)
        {
            // 1. إنشاء كائن مالي للفاتورة وتمرير الـ BillID له فقط!
            clsBillingService paymentSummary = new clsBillingService(billID);

            // 2. التحقق مما إذا تم العثور على الفاتورة بنجاح وملء بياناتها
            if (paymentSummary.IsFound)
            {
                // عرض البيانات المالية مباشرة من خصائص الكائن الممتلئ
                txtSerachByBillID.Text = paymentSummary.BillNumber;
                lblSubtotal.Text = paymentSummary.TotalBeforeDiscountAndTax.ToString("C");
                lblMedicines.Text= paymentSummary.MedicinesTotal.ToString("C");
                lblServices.Text = paymentSummary.ServicesTotal.ToString("C");
                lblAppoinmentFees.Text= paymentSummary.AppointmentFees.ToString("C");
                lblTax.Text = paymentSummary.TotalTax.ToString("C");
                lblDiscount.Text = paymentSummary.TotalDiscount.ToString("C");
                txtDiscount.Text = paymentSummary.TotalDiscount.ToString("C");
                lblFinalTotal.Text = paymentSummary.FinalTotalIncludingAll.ToString("C");
                lbTotalBillAmount.Text = paymentSummary.FinalTotalIncludingAll.ToString("C");
                lblPaymentAmmount.Text = paymentSummary.PaymentAmount.ToString("C");
                lblFinalPaymentDue.Text = paymentSummary.BalanceDue.ToString("C");
                txtAmountToPay.Text = paymentSummary.BalanceDue.ToString("C2");
                lbAmmountToPay.Text= paymentSummary.BalanceDue.ToString("C2");
                // ربط الـ DataTable الجاهز والمملوء تلقائياً بالـ DataGridView
                dgvBillDetails.DataSource = paymentSummary.BillDetailsTable;
            }
            else
            {
                MessageBox.Show("عذراً، لم يتم العثور على تفاصيل الفاتورة المطلوبة!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lbPlaceHolederBillID_Click(object sender, EventArgs e)
        {
            txtSerachByBillID.Focus();
        }


        private void radioButton_Paint(object sender, PaintEventArgs e)
        {
            // تحويل الـ sender تلقائياً ليعبر عن أي RadioButton يتم رسمه حالياً
         RadioButton radioButton = (RadioButton)sender;

            // 1. تنظيف الخلفية بلون الشاشة البترولية لإخفاء الرسم الافتراضي للويندوز
            e.Graphics.Clear(Color.FromArgb(20, 43, 50));

            // تفعيل تنعيم الحواف (Anti-aliasing) لتظهر الدوائر بشكل ناعم واحترافي بدون تكسر
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 2. رسم نص الـ RadioButton المخصص باللون الأبيض
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                // تم تغييرها إلى radioButton.Text و radioButton.Font لتصبح ديناميكية
                e.Graphics.DrawString(radioButton.Text, radioButton.Font, textBrush, 20, 0);
            }

            // 3. تحديد أبعاد الدائرة الخارجية (القطر 14 بكسل)
            Rectangle outerCircle = new Rectangle(2, 4, 14, 14);

            // 4. رسم خلفية الدائرة باللون البترولي الداكن وحوافها المضيئة
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(28, 58, 68)))
            using (Pen borderPen = new Pen(Color.FromArgb(0, 210, 190), 1.5f)) // حافة بترولية مضيئة
            {
                e.Graphics.FillEllipse(bgBrush, outerCircle);  // تعبئة الدائرة
                e.Graphics.DrawEllipse(borderPen, outerCircle); // رسم الإطار الخارجي
            }

            // 5. إذا كان الخيار محدداً (Checked).. ارسم نقطة التحديد الداخلية (Dot)
            if (radioButton.Checked) // تم تعديلها هنا أيضاً إلى radioButton
            {
                // تحديد أبعاد الدائرة الداخلية لتكون في المنتصف تماماً
                Rectangle innerDot = new Rectangle(6, 8, 6, 6);

                // رسم النقطة باللون الأبيض الناصع
                using (Brush dotBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillEllipse(dotBrush, innerDot);
                }
            }
        }

        private void txtSerachByBillID_TextChanged(object sender, EventArgs e)
        {
            lbPlaceHolederBillID.Visible = string.IsNullOrEmpty(txtSerachByBillID.Text);
        }

        private void frmProcessPayments_Load(object sender, EventArgs e)
        {

        }

        private void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            // 1. التحقق من صحة مدخلات الشاشة (Validation)
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please fill all required fields correctly.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string cleanAmountToPay = txtAmountToPay.Text.Replace("$", "").Trim();
            string cleanAmountReceived = txtAmountReceived.Text.Replace("$", "").Trim();
            // 2. التحقق من تحويل المبالغ بشكل آمن (Safe Parsing)
            if (!decimal.TryParse(cleanAmountToPay, out decimal amountToPay) ||
                !decimal.TryParse(cleanAmountReceived, out decimal amountReceived))
            {
                MessageBox.Show("Invalid financial numbers. Please check the amounts.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. تطبيق شرط حماية العيادة (يجب دفع 70% على الأقل من الفاتورة)
            decimal minimumRequired = amountToPay * 0.70m;
            if (amountReceived < minimumRequired)
            {
                MessageBox.Show($"Payment rejected! The minimum required amount to process this payment is 70% ({minimumRequired:N2}).",
                                "Policy Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. رسالة تأكيد للمستخدم قبل الحفظ نهائياً
            if (MessageBox.Show("Are you sure you want to confirm this payment?", "Confirm Payment",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            // 5. إنشاء كائن الدفع وتعبئة البيانات
            clsPayment Payment = new clsPayment();
            Payment.BillID = _BillID;

            // تحديد طريقة الدفع بشكل نظيف
            if (rdbCreditCard.Checked)
                Payment.PaymentMethod = rdbCreditCard.Text;
            else if (rdbCash.Checked)
                Payment.PaymentMethod = rdbCash.Text;
            else
                Payment.PaymentMethod = rdbInSurance.Text;

            // 🎯 إصلاح منطق الحالة: نقارن ما دفعه بما هو مطلوب منه فعلياً
            if (amountReceived >= amountToPay)
                Payment.Status = clsBillingService.enPaymetnStatus.Paid; // دفع كامل المبلغ أو زيادة
            else
                Payment.Status = clsBillingService.enPaymetnStatus.Partial; // دفع جزءاً مقبولاً (بين 70% و 99%)

            Payment.PaymentAmount = amountReceived;
            Payment.PaymentDate = DateTime.Now;
            Payment.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            // 6. حفظ العملية والتعامل مع النتائج
            if (Payment.Save())
            {
                MessageBox.Show("Payment has been processed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // تعطيل عناصر التحكم لمنع تكرار الضغط والدفع بالخطأ
                btnConfirmPayment.Enabled = false;
                txtAmountReceived.Enabled = false;
                pnlPaymentMethods.Enabled = false; // افترضنا أن الـ RadioButtons داخل Panel لتعطيلها معاً

                // 7. [إجراء اختياري احترافي] طباعة الإيصال فوراً لو رغب المستخدم
                if (MessageBox.Show("Do you want to print the payment receipt?", "Print Receipt",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    clsFormHelper.ShowForm(() => new frmIssueInvoice(_BillID));

                    this.Close();
                }
                else
                {
                    clsFormHelper.ShowForm<frmManageBills>();
                    this.Close();
                }


               
            }
            else
            {
                MessageBox.Show("An error occurred while saving the payment. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtAmountReceived_TextChanged(object sender, EventArgs e)
        {
            string cleanAmountToPay = txtAmountToPay.Text.Replace("$", "").Trim();
            string cleanAmountReceived = txtAmountReceived.Text.Replace("$", "").Trim();

            // 1. التأكد من قراءة المبلغ المطلوب بشكل صحيح
            if (!decimal.TryParse(cleanAmountToPay, out decimal amountToPay)) return;

            // 2. التحقق من الرقم المدخل في المبلغ المستلم
            if (decimal.TryParse(cleanAmountReceived, out decimal amountReceived))
            {
                // 🛑 الشرط الجديد: منع إدخال قيمة أكبر من المطلوب (إذا كانت طريقة الدفع ليست كاش)
                // ملاحظة: إذا كنت تريد منع الزيادة في كل الحالات (حتى الكاش)، احذف شرط الـ RadioButton واكتفِ بـ (amountReceived > amountToPay)
                if (amountReceived > amountToPay && (rdbCreditCard.Checked || rdbInSurance.Checked))
                {
                    lblChangeDue.Text = "Amount cannot exceed the required total!";
                    lblChangeDue.ForeColor = Color.DarkRed;
                    btnConfirmPayment.Enabled = false; // تعطيل الزر
                    return;
                }

                // 3. حساب الحد الأدنى المسموح به للدفع (70% من إجمالي المبلغ)
                decimal minimumRequiredAmount = amountToPay * 0.70m;

                // 4. التحقق من الحد الأدنى ماليًا
                if (amountReceived < minimumRequiredAmount)
                {
                    lblChangeDue.Text = "Minimum 70% payment required!";
                    lblChangeDue.ForeColor = Color.DarkRed;
                    btnConfirmPayment.Enabled = false;
                    return;
                }

                // 5. حساب الباقي (Change Due) أو المتبقي عليه (Balance Due)
                decimal changeDue = amountReceived - amountToPay;

                if (changeDue >= 0)
                {
                    // المريض دفع المبلغ كاملاً وزيادة (مسموح فقط في الكاش بناءً على الشرط أعلاه)
                    lblChangeDue.Text = "Change to return: " + changeDue.ToString("N2");
                    lblChangeDue.ForeColor = Color.Green;
                    btnConfirmPayment.Enabled = true;
                }
                else
                {
                    // المريض دفع جزء مقبول (بين 70% و 99%) وسيتبقى عليه جزء (آجل)
                    decimal remainingBalance = amountToPay - amountReceived;
                    lblChangeDue.Text = "Remaining Debt: " + remainingBalance.ToString("N2");
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

        private void txtAmountToPay_TextChanged(object sender, EventArgs e)
        {

        }

        private void RadioB_CheckedChanged(object sender, EventArgs e)
        {
            txtAmountReceived_TextChanged(null, null);
        }
    }
}
