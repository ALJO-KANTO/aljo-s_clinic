using Clinic_Business;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Casher
{
    public partial class frmMedicineSalesReturn : Form
    {
        private int _BillID = -1;
        private DataTable _dtReturnItems;
        private clsSalesReturn _SalesReturn;

        private NumericUpDown _qtyNumericUpDown = new NumericUpDown();

        public frmMedicineSalesReturn(int BillID)
        {
            InitializeComponent();
            this._BillID = BillID;

            txtBillIDSearch.Enter += txtBillIDSearch_Enter;
            txtBillIDSearch.Leave += txtBillIDSearch_Leave;

            // 🎨 إعدادات المظهر المدمج
            _qtyNumericUpDown.Minimum = 0;
            _qtyNumericUpDown.Visible = false;
            _qtyNumericUpDown.BorderStyle = BorderStyle.None;

            // ربط أحداث العداد للمزامنة الفورية
            _qtyNumericUpDown.ValueChanged += QtyNumericUpDown_ValueChanged;
            _qtyNumericUpDown.Leave += QtyNumericUpDown_Leave;

            // إضافة العداد لعناصر الجدول
            dgvReturnItems.Controls.Add(_qtyNumericUpDown);

            // 🔔 ربط أحداث الجدول لضمان الظهور التلقائي والثابت للعداد
            dgvReturnItems.SelectionChanged += dgvReturnItems_SelectionChanged;
            dgvReturnItems.Paint += dgvReturnItems_Paint;
            dgvReturnItems.Scroll += (s, ev) => { _ShowUpDownOnCurrentCell(); };
            dgvReturnItems.DataError += (s, ev) => { ev.ThrowException = false; };
        }

        private void frmMedicineSalesReturn_Load(object sender, EventArgs e)
        {
            if (_BillID != -1)
            {
                txtBillIDSearch.Text = _BillID.ToString();
                _LoadBillData(_BillID);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBillIDSearch.Text.Trim()) || !int.TryParse(txtBillIDSearch.Text.Trim(), out int billID))
            {
                MessageBox.Show("Please enter a valid Bill ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _LoadBillData(billID);
        }

        private void _LoadBillData(int billID)
        {
            if (!clsSalesReturn.IsBillPaidOrPartiallyPaid(billID))
            {
                MessageBox.Show("Cannot process return. This bill is either unpaid, cancelled, or does not exist.",
                                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                _ResetForm();
                return;
            }
            _qtyNumericUpDown.Visible = false;
            clsSalesReturn.clsBillMasterInfo billMaster;

            _dtReturnItems = clsSalesReturn.GetBillCompleteDetails(billID, out billMaster);
            if (!billMaster.IsFound|| _dtReturnItems.Rows.Count == 0 || _dtReturnItems == null)
            {
                MessageBox.Show("This bill was not found in the system, or its medicines have not been dispensed yet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ResetForm();
                this.Close();
                return;
            }

            this._BillID = billID;
            lblBillNumber.Text = billMaster.BillNumber;
            lblBillDate.Text = billMaster.BillDate.ToShortDateString();
            lblPatientName.Text = billMaster.PatientName;

            if (!_dtReturnItems.Columns.Contains("QtyReturned"))
            {
                DataColumn dc = new DataColumn("QtyReturned", typeof(int));
                dc.DefaultValue = 0;
                _dtReturnItems.Columns.Add(dc);
            }

            dgvReturnItems.DataSource = _dtReturnItems;
            _SetupDataGridViewColumns();
            _CalculateTotalRefund();

            // 🎯 إظهار العداد فوراً على أول سطر بمجرد تحميل البيانات
            _ShowUpDownOnCurrentCell();
        }

        private void _SetupDataGridViewColumns()
        {
            if (dgvReturnItems.Columns.Contains("PrescriptionDetailsID")) dgvReturnItems.Columns["PrescriptionDetailsID"].Visible = false;
            if (dgvReturnItems.Columns.Contains("MedicineID")) dgvReturnItems.Columns["MedicineID"].Visible = false;
            if (dgvReturnItems.Columns.Contains("TaxRate")) dgvReturnItems.Columns["TaxRate"].Visible = false;

            dgvReturnItems.Columns["SavedMedicineName"].HeaderText = "Medicine Name";
            dgvReturnItems.Columns["SavedMedicineName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvReturnItems.Columns["SavedMedicineName"].MinimumWidth = 350;
            dgvReturnItems.Columns["SavedMedicineName"].ReadOnly = true;

            dgvReturnItems.Columns["QtyBought"].HeaderText = "Qty Bought";
            dgvReturnItems.Columns["QtyBought"].Width = 140;
            dgvReturnItems.Columns["QtyBought"].ReadOnly = true;

            dgvReturnItems.Columns["QtyReturnedBefore"].HeaderText = "Qty Returned Before";
            dgvReturnItems.Columns["QtyReturnedBefore"].Width = 180;
            dgvReturnItems.Columns["QtyReturnedBefore"].ReadOnly = true;

            dgvReturnItems.Columns["UnitPrice"].HeaderText = "Unit Price";
            dgvReturnItems.Columns["UnitPrice"].Width = 130;
            dgvReturnItems.Columns["UnitPrice"].ReadOnly = true;

            dgvReturnItems.Columns["QtyReturned"].HeaderText = "Qty Returned";
            dgvReturnItems.Columns["QtyReturned"].Width = 140;
            dgvReturnItems.Columns["QtyReturned"].ReadOnly = true;

            dgvReturnItems.Columns["QtyReturned"].DefaultCellStyle.BackColor = Color.FromArgb(224, 255, 255);
            dgvReturnItems.Columns["QtyReturned"].DefaultCellStyle.ForeColor = Color.FromArgb(0, 0, 0);

            dgvReturnItems.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvReturnItems.AllowUserToAddRows = false;
            dgvReturnItems.RowHeadersVisible = false;
        }

        private void _ShowUpDownOnCurrentCell()
        {
            if (dgvReturnItems.CurrentCell == null)
            {
                _qtyNumericUpDown.Visible = false;
                return;
            }

            // التحقق أن الخلية المحددة حالياً تنتمي لعمود الكمية المرتجعة
            if (dgvReturnItems.CurrentCell.OwningColumn.Name == "QtyReturned")
            {
                int rowIndex = dgvReturnItems.CurrentCell.RowIndex;
                int columnIndex = dgvReturnItems.CurrentCell.ColumnIndex;
                DataGridViewRow row = dgvReturnItems.Rows[rowIndex];

                int qtyBought = Convert.ToInt32(row.Cells["QtyBought"].Value);
                int qtyReturnedBefore = Convert.ToInt32(row.Cells["QtyReturnedBefore"].Value);
                int maxAllowedForReturn = qtyBought - qtyReturnedBefore;

                if (maxAllowedForReturn <= 0)
                {
                    _qtyNumericUpDown.Visible = false;
                    return;
                }

                _qtyNumericUpDown.Maximum = maxAllowedForReturn;

                // حساب وتحديد موقع الخلية الحالية بدقة هندسية
                Rectangle rect = dgvReturnItems.GetCellDisplayRectangle(columnIndex, rowIndex, true);

                // إذا كانت الخلية خارج نطاق الرؤية الحالي (بسبب السكرول مثلاً) إخفها
                if (rect.Width == 0 || rect.Height == 0)
                {
                    _qtyNumericUpDown.Visible = false;
                    return;
                }

                _qtyNumericUpDown.Location = rect.Location;
                _qtyNumericUpDown.Size = rect.Size;

                _qtyNumericUpDown.Font = new Font("Segoe UI", 17F, FontStyle.Bold); // ارفع الـ Size إلى 12 أو 14 حسب رغبتك
                _qtyNumericUpDown.BorderStyle = BorderStyle.None;
                _qtyNumericUpDown.TextAlign = HorizontalAlignment.Center;
                _qtyNumericUpDown.BackColor = Color.FromArgb(10, 50, 51);
                _qtyNumericUpDown.ForeColor = Color.White;

                // تحميل القيمة برمجياً بدون إطلاق حدث التغيير بشكل دائري مكرر
                if (row.Cells[columnIndex].Value != DBNull.Value && row.Cells[columnIndex].Value != null)
                {
                    _qtyNumericUpDown.Value = Convert.ToDecimal(row.Cells[columnIndex].Value);
                }
                else
                {
                    _qtyNumericUpDown.Value = 0;
                }

                _qtyNumericUpDown.Visible = true;
                _qtyNumericUpDown.Focus();
                _qtyNumericUpDown.Select(0, _qtyNumericUpDown.Text.Length);
            }
            else
            {
                // إذا تحرك المستخدم لعمود آخر، قم بإخفاء العداد
                _qtyNumericUpDown.Visible = false;
            }


        }

        private void dgvReturnItems_SelectionChanged(object sender, EventArgs e)
        {
            _ShowUpDownOnCurrentCell();
        }

        private void dgvReturnItems_Paint(object sender, PaintEventArgs e)
        {
            if (_qtyNumericUpDown.Visible)
            {
                _ShowUpDownOnCurrentCell();
            }
        }

        private void QtyNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (dgvReturnItems.CurrentCell != null && dgvReturnItems.CurrentCell.OwningColumn.Name == "QtyReturned")
            {
                dgvReturnItems.CurrentCell.Value = (int)_qtyNumericUpDown.Value;
                _CalculateTotalRefund();
            }
        }

        private void QtyNumericUpDown_Leave(object sender, EventArgs e)
        {
            // نترك العداد مرئياً طالما الخلية محددة لتجربة مستخدم ثابتة ومستقرة
        }

        private void _CalculateTotalRefund()
        {
            decimal totalRefund = 0;

            foreach (DataGridViewRow row in dgvReturnItems.Rows)
            {
                if (row.IsNewRow) continue;

                int qtyReturnedNow = row.Cells["QtyReturned"].Value == DBNull.Value
       ? 0
       : Convert.ToInt32(row.Cells["QtyReturned"].Value);

                decimal unitPrice = row.Cells["UnitPrice"].Value == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(row.Cells["UnitPrice"].Value);

                decimal taxRate = row.Cells["TaxRate"].Value == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(row.Cells["TaxRate"].Value);

                if (qtyReturnedNow > 0)
                {
                    decimal itemTotal = unitPrice * qtyReturnedNow;
                    decimal itemTax = itemTotal * (taxRate / 100);
                    totalRefund += (itemTotal + itemTax);
                }
            }

            lblTotalRefundAmount.Text = totalRefund.ToString("F2") + " $";
        }

        private void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            _qtyNumericUpDown.Visible = false;
            if (_BillID == -1 || _dtReturnItems == null) return;

            _CalculateTotalRefund();
            decimal totalRefund = decimal.Parse(lblTotalRefundAmount.Text.Replace("$", "").Trim());

            if (totalRefund <= 0)
            {
                MessageBox.Show("Please select valid items and quantities to return first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to confirm this sales return and refund the patient?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            _SalesReturn = new clsSalesReturn();
            _SalesReturn.BillID = this._BillID;
            _SalesReturn.TotalRefund = totalRefund;
            _SalesReturn.CashierID = clsGlobal.CurrentUser.UserID;

            try
            {
                if (_SalesReturn.Save(_dtReturnItems))
                {
                    MessageBox.Show($"Sales return saved successfully. Inventories updated.\nReturn Receipt ID: {_SalesReturn.ReturnID}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadBillData(this._BillID);
                }
                else
                {
                    MessageBox.Show("Failed to save the sales return. Please contact the system administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Business Rule Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void _ResetForm()
        {
            _BillID = -1;
            _dtReturnItems = null;
            dgvReturnItems.DataSource = null;
            lblBillNumber.Text = "---";
            lblBillDate.Text = "---";
            lblPatientName.Text = "---";
            lblTotalRefundAmount.Text = "0.00 $";
            _qtyNumericUpDown.Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e) => this.Close();
        private void txtBillIDSearch_Enter(object sender, EventArgs e) => pnlSearchGlow.BackColor = Color.Cyan;
        private void txtBillIDSearch_Leave(object sender, EventArgs e) => pnlSearchGlow.BackColor = Color.FromArgb(23, 73, 74);
    }
}