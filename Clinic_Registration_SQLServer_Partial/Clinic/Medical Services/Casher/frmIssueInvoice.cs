using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Casher
{
    public partial class frmIssueInvoice : Form
    {
        private int _BillID = -1;
        private clsInvoice _Invoice;
        private decimal _RemainingAmount = 0;
        public frmIssueInvoice(int BillID)
        {
            InitializeComponent();
            lbCurrentUser.Text = clsGlobal.PersonName;
            _BillID = BillID;
            _FillForm();
        }
        public frmIssueInvoice()
        {
            InitializeComponent();
            lbCurrentUser.Text = clsGlobal.PersonName;
           
        }

        private void _ResetForm()
        {
            dgBill.DataSource = null;
            lbSubtotal.Text = "$ 0";
            lbCoPay.Text = "$ 0";
            lbDiscoutns.Text = "$ 0";
            lbFinalTotal.Text = "$ 0";
            lbRemainingAmount.Text = "$ 0";

            txtInvoiceNumber.Text = "";
            txtInvoiceDate.Text = "";
            txtPatientName.Text = "";
            txtPatientID.Text = "";
            _RemainingAmount = 0;
        }
        private void _FillForm()
        {
            _Invoice = clsInvoice.GetInvoice(_BillID);
            if (_Invoice == null)
            {
                MessageBox.Show("الفاتورة غير موجودة أو لا توجد بيانات مرتبطة بها.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ResetForm();
                return;
            }
            dgBill.DataSource = _Invoice.ItemsTable;
            lbSubtotal.Text = "$ " + _Invoice.Subtotal.ToString("N2");
            lbCoPay.Text = "$ " + _Invoice.PaymentAmount.ToString("N2");
            lbDiscoutns.Text = "$ " + _Invoice.TotalDiscount.ToString("N2");
            lbFinalTotal.Text = "$ " + _Invoice.FinalTotal.ToString("N2");
            _RemainingAmount = (_Invoice.FinalTotal - _Invoice.PaymentAmount);
            lbRemainingAmount.Text = "$ " + _RemainingAmount.ToString("N2");

            txtInvoiceNumber.Text = _Invoice.InvoiceNumber.ToString();
            txtInvoiceDate.Text = _Invoice.InvoiceDate.ToString("dd/MM/yyyy");
            txtPatientName.Text = _Invoice.PatientName.ToString();
            txtPatientID.Text = _Invoice.PatientID.ToString();
        }

        private void _PrintInvoiceDirectly()
        {
            try
            {
                PrintDocument pd = new PrintDocument();

                // التوجيه لطابعة الـ PDF الافتراضية في الويندوز
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                if (!pd.PrinterSettings.IsValid)
                {
                    pd.PrinterSettings.PrinterName = new PrinterSettings().PrinterName;
                }

                pd.PrintPage += new PrintPageEventHandler(_PrintReceiptPage);
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Printing Error: {ex.Message}", "Printer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void _PrintReceiptPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            Font fontTitle = new Font("Segoe UI", 14, FontStyle.Bold);
            Font fontHeader = new Font("Segoe UI", 10, FontStyle.Bold);
            Font fontBody = new Font("Segoe UI", 9, FontStyle.Regular);
            Font fontTotal = new Font("Segoe UI", 11, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);

            int startX = 10;
            int startY = 10;
            int offset = 30;

            // ترويسة الفاتورة
            g.DrawString("CLINIC MEDICAL CENTER", fontTitle, brush, startX, startY);
            offset += 25;
            g.DrawString($"Invoice No: {_Invoice.InvoiceNumber}", fontBody, brush, startX, startY + offset);
            offset += 20;
            g.DrawString($"Patient: {_Invoice.PatientName}", fontBody, brush, startX, startY + offset);
            offset += 20;
            g.DrawString($"Date: {_Invoice.InvoiceDate.ToString("yyyy-MM-dd")}", fontBody, brush, startX, startY + offset);
            offset += 20;
            g.DrawString(new string('-', 45), fontBody, brush, startX, startY + offset);

            // هيدر جدول الأدوية والخدمات
            offset += 20;
            g.DrawString("Item Description", fontHeader, brush, startX, startY + offset);
            g.DrawString("Qty", fontHeader, brush, startX + 160, startY + offset);
            g.DrawString("Price", fontHeader, brush, startX + 210, startY + offset);

            offset += 20;
            g.DrawString(new string('-', 45), fontBody, brush, startX, startY + offset);

            // طباعة الحقول ديناميكياً من الـ ItemsTable الخاص بك
            if (_Invoice.ItemsTable != null && _Invoice.ItemsTable.Rows.Count > 0)
            {
                // تحويل الجدول إلى DataView لسهولة عمل الفلترة
                DataView dvItems = _Invoice.ItemsTable.DefaultView;

                // ==========================================
                // 1️⃣ طباعة الموعد (Appointment) إن وُجد
                // ==========================================
                dvItems.RowFilter = "ItemType = 'Appointment'";
                if (dvItems.Count > 0)
                {
                    foreach (DataRowView rowView in dvItems)
                    {
                        DataRow row = rowView.Row;
                        offset += 20;
                        string itemName = row["ItemName"].ToString();
                        string qty = row["Quantity"].ToString();
                        decimal total = Convert.ToDecimal(row["Total"]);

                        if (itemName.Length > 20) itemName = itemName.Substring(0, 18) + "..";

                        g.DrawString(itemName, fontBody, brush, startX, startY + offset);
                        g.DrawString(qty, fontBody, brush, startX + 160, startY + offset);
                        g.DrawString($"$ {total.ToString("N2")}", fontBody, brush, startX + 210, startY + offset);
                    }

                    // طباعة خط فاصل بعد الموعد
                    offset += 20;
                    g.DrawString(new string('-', 45), fontBody, brush, startX, startY + offset);
                }

                // ==========================================
                // 2️⃣ طباعة الخدمات (Service) إن وُجدت
                // ==========================================
                dvItems.RowFilter = "ItemType = 'Service'";
                if (dvItems.Count > 0)
                {
                    foreach (DataRowView rowView in dvItems)
                    {
                        DataRow row = rowView.Row;
                        offset += 20;
                        string itemName = row["ItemName"].ToString();
                        string qty = row["Quantity"].ToString();
                        decimal total = Convert.ToDecimal(row["Total"]);

                        if (itemName.Length > 20) itemName = itemName.Substring(0, 18) + "..";

                        g.DrawString(itemName, fontBody, brush, startX, startY + offset);
                        g.DrawString(qty, fontBody, brush, startX + 160, startY + offset);
                        g.DrawString($"$ {total.ToString("N2")}", fontBody, brush, startX + 210, startY + offset);
                    }

                    // طباعة خط فاصل بعد الخدمات
                    offset += 20;
                    g.DrawString(new string('-', 45), fontBody, brush, startX, startY + offset);
                }

                // ==========================================
                // 3️⃣ طباعة الأدوية (Medicine) إن وُجدت
                // ==========================================
                dvItems.RowFilter = "ItemType = 'Medicine'";
                if (dvItems.Count > 0)
                {
                    foreach (DataRowView rowView in dvItems)
                    {
                        DataRow row = rowView.Row;
                        offset += 20;
                        string itemName = row["ItemName"].ToString();
                        string qty = row["Quantity"].ToString();
                        decimal total = Convert.ToDecimal(row["Total"]);

                        if (itemName.Length > 20) itemName = itemName.Substring(0, 18) + "..";

                        g.DrawString(itemName, fontBody, brush, startX, startY + offset);
                        g.DrawString(qty, fontBody, brush, startX + 160, startY + offset);
                        g.DrawString($"$ {total.ToString("N2")}", fontBody, brush, startX + 210, startY + offset);
                    }

                    // طباعة خط فاصل نهائي بعد الأدوية قبل المربع المالي
                    offset += 20;
                    g.DrawString(new string('-', 45), fontBody, brush, startX, startY + offset);
                }

                // 🎯 تصفير الفلتر لإعادة الـ DataView لحالته الطبيعية
                dvItems.RowFilter = "";
            }

            offset += 20;
            g.DrawString(new string('-', 45), fontBody, brush, startX, startY + offset);

            // المربع المالي النهائي المأخوذ من الـ Properties الخاصة بك
            offset += 25;
            g.DrawString("Subtotal:", fontBody, brush, startX, startY + offset);
            g.DrawString($"$ {_Invoice.Subtotal.ToString("N2")}", fontBody, brush, startX + 180, startY + offset);

            offset += 20;
            g.DrawString("Total Discount:", fontBody, brush, startX, startY + offset);
            g.DrawString($"$ {_Invoice.TotalDiscount.ToString("N2")}", fontBody, brush, startX + 180, startY + offset);

            offset += 20;
            g.DrawString("Final Total:", fontHeader, brush, startX, startY + offset);
            g.DrawString($"$ {_Invoice.FinalTotal.ToString("N2")}", fontHeader, brush, startX + 180, startY + offset);

            offset += 20;
            g.DrawString("Amount Paid:", fontBody, brush, startX, startY + offset);
            g.DrawString($"$ {_Invoice.PaymentAmount.ToString("N2")}", fontBody, brush, startX + 180, startY + offset);

            // طباعة المتبقي إذا كان دفعاً جزئياً
            if (_RemainingAmount > 0)
            {
                offset += 20;
                g.DrawString("Remaining Due:", fontTotal, brush, startX, startY + offset);
                g.DrawString($"$ {_RemainingAmount.ToString("N2")}", fontTotal, brush, startX + 180, startY + offset);
            }

            // ذيل الفاتورة
            offset += 35;
            g.DrawString("Thank you for choosing our clinic!", fontHeader, brush, startX + 20, startY + offset);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void lbPlaceHolederBillID_Click(object sender, EventArgs e)
        {
            txtSearchByPatientName.Focus();
        }
        private void txtSearchByPatientName_TextChanged(object sender, EventArgs e)
        {
            lbPlaceHoleder.Visible = string.IsNullOrEmpty(txtSearchByPatientName.Text);
        }
        private void txtSearchByPatientName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && char.IsControl(e.KeyChar);

        }
        private void txtSearchByPatientName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtSearchByPatientName.Text))
                {
                    _BillID = int.Parse(txtSearchByPatientName.Text);
                    _FillForm();
                }
            }
        }
        private void btnCenerateAndFinalizeInVoice_Click(object sender, EventArgs e)
        {
            if (_Invoice == null) return;

                _PrintInvoiceDirectly();
            this.DialogResult = DialogResult.OK;

            if (MessageBox.Show("Do you want to go the Mange Bills Screen?", "Print Receipt",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                clsFormHelper.ShowForm<frmManageBills>();
                this.Close(); 
            }
           
        }
        private void btnPreviewInvoice_Click(object sender, EventArgs e)
        {
            if (_Invoice == null)
            {
                MessageBox.Show("No active invoice loaded to preview.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(_PrintReceiptPage);
            previewDialog.Document = pd;
            previewDialog.ShowDialog();
        }
        private void btnSaveDraft_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Invoice draft remains pending. You can re-open and process it from Bill Management anytime.",
                            "Draft Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
