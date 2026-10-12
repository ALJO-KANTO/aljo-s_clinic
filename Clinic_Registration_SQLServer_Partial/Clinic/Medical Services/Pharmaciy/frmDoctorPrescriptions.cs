using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Pharmaciy
{
    public partial class frmPrescriptionDispnsing : Form
    {
        private DataTable dtAllDetails = clsPrescription.GetAllPrescriptionDetails();
        private DataTable dtAllActivePrescriptions = clsPrescription.GetAllActivePrescriptions();
        private bool _isUpdating = false; // Guard flag to prevent re-entry loops
        private int _currentPrescriptionId = -1;
        private int _AppointmentID = -1;
        private int _VisitID = -1;
        private decimal TotalMedicinesAmount = 0.0m, TaxRate = 0.0m;
        private NumericUpDown _qtyNumericUpDown = new NumericUpDown();

        public frmPrescriptionDispnsing()
        {
            InitializeComponent();

            // Wire up event handlers
            dgPrescriptionDetails.DataError += dgPrescriptionDetails_DataError;
            dgPrescriptionDetails.CellValueChanged += dgPrescriptionDetails_CellValueChanged;
            dgPrescriptionDetails.CellClick += dgPrescriptionDetails_CellClick;
            dgPrescriptionDetails.CellBeginEdit += dgPrescriptionDetails_CellBeginEdit;
            // Configure custom embedded numeric up-down
            _qtyNumericUpDown.Minimum = 0;
            _qtyNumericUpDown.Visible = false;
            _qtyNumericUpDown.ValueChanged += QtyNumericUpDown_ValueChanged;
            _qtyNumericUpDown.Leave += QtyNumericUpDown_Leave;

            dgPrescriptionDetails.Controls.Add(_qtyNumericUpDown);

            // Hide numeric up-down control during scrolling to prevent visual glitches
            dgPrescriptionDetails.Scroll += (s, ev) => { _qtyNumericUpDown.Visible = false; };
        }

        private void PrintShortageReport_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // إعدادات الخطوط والألوان للتقرير
            Font titleFont = new Font("Segoe UI", 18, FontStyle.Bold);
            Font headerFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Font bodyFont = new Font("Segoe UI", 10, FontStyle.Regular);
            Font totalFont = new Font("Segoe UI", 11, FontStyle.Bold);

            Brush tealBrush = new SolidBrush(Color.FromArgb(0, 105, 92)); // لون التيل المعتمد للهيدر
            Brush textBrush = Brushes.Black;
            Pen linePen = new Pen(Color.DarkGray, 1);

            int startX = 50;
            int startY = 40;
            int offsetY = 0;

            // --- 1. ترويسة التتقرير (Header) ---
            e.Graphics.DrawString("Clinic Management System", headerFont, tealBrush, startX, startY);
            offsetY += 25;
            e.Graphics.DrawString("Medicine Shortage Report", titleFont, textBrush, startX, startY + offsetY);
            offsetY += 35;
            e.Graphics.DrawString($"Date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", bodyFont, textBrush, startX, startY + offsetY);
            offsetY += 25;

            // رسم خط فاصل
            e.Graphics.DrawLine(linePen, startX, startY + offsetY, e.PageBounds.Width - 50, startY + offsetY);
            offsetY += 15;

            // --- 2. ترويسة الجدول (Table Headers) ---
            // تحديد أماكن الأعمدة (X Positions)
            int colMedX = startX;       // اسم العلاج
            int colPriceX = startX + 250; // السعر
            int colReqX = startX + 370;   // المطلوبة
            int colDispX = startX + 470;  // المصروفة
            int colShortX = startX + 580; // النواقص

            e.Graphics.DrawString("Medicine Name", headerFont, tealBrush, colMedX, startY + offsetY);
            e.Graphics.DrawString("Price", headerFont, tealBrush, colPriceX, startY + offsetY);
            e.Graphics.DrawString("Req.Qty", headerFont, tealBrush, colReqX, startY + offsetY);
            e.Graphics.DrawString("Disp.Qty", headerFont, tealBrush, colDispX, startY + offsetY);
            e.Graphics.DrawString("Shortage", headerFont, tealBrush, colShortX, startY + offsetY);

            offsetY += 25;
            e.Graphics.DrawLine(linePen, startX, startY + offsetY, e.PageBounds.Width - 50, startY + offsetY);
            offsetY += 10;

            bool hasShortage = false;
            decimal totalShortageValue = 0;

            // --- 3. قراءة البيانات وفحص النواقص ---
            foreach (DataGridViewRow row in dgPrescriptionDetails.Rows)
            {
                // تجنب السطور الفارغة إن وجدت
                if (row.Cells["MedicineName"].Value == null) continue;

                // جلب القيم بأمان مع التحويل الرقمي
                string medicineName = row.Cells["MedicineName"].Value.ToString();
                decimal price = Convert.ToDecimal(row.Cells["SavedMedicinePrice"].Value ?? 0);
                int reqQty = Convert.ToInt32(row.Cells["RequiredQuantity"].Value ?? 0);
                int dispQty = Convert.ToInt32(row.Cells["DispensedQuantity"].Value ?? 0);

                // الشرط: إذا كانت الكمية المصروفة أصغر من الكمية المطلوبة
                if (dispQty < reqQty)
                {
                    hasShortage = true;
                    int shortageQty = reqQty - dispQty; // حساب كمية النواقص
                    decimal shortagePrice = price * shortageQty; // تكلفة النواقص لهذا العلاج
                    totalShortageValue += shortagePrice;

                    // رسم السطر الخاص بالعلاج الناقص
                    e.Graphics.DrawString(medicineName, bodyFont, textBrush, colMedX, startY + offsetY);
                    e.Graphics.DrawString(price.ToString("C2"), bodyFont, textBrush, colPriceX, startY + offsetY);
                    e.Graphics.DrawString(reqQty.ToString(), bodyFont, textBrush, colReqX, startY + offsetY);
                    e.Graphics.DrawString(dispQty.ToString(), bodyFont, textBrush, colDispX, startY + offsetY);

                    // تمييز كمية النواقص بلون أحمر داكن متناسق للفت الانتباه
                    e.Graphics.DrawString(shortageQty.ToString(), headerFont, new SolidBrush(Color.FromArgb(211, 47, 47)), colShortX, startY + offsetY);

                    offsetY += 25;

                    // تحقق من عدم تجاوز حدود الصفحة (أمان إضافي للطباعة)
                    if (startY + offsetY > e.PageBounds.Height - 100)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                }
            }

            // --- 4. التذييل والمجموع (Footer) ---
            if (!hasShortage)
            {
                e.Graphics.DrawString("No shortages found. All medicines are fully dispensed!", bodyFont, Brushes.Green, startX, startY + offsetY + 20);
            }
            else
            {
                offsetY += 15;
                e.Graphics.DrawLine(linePen, startX, startY + offsetY, e.PageBounds.Width - 50, startY + offsetY);
                offsetY += 10;
                e.Graphics.DrawString($"Estimated Shortage Value: {totalShortageValue.ToString("C2")}", totalFont, tealBrush, colPriceX, startY + offsetY);
            }

            e.HasMorePages = false; // لا توجد صفحات أخرى
        }

        private void _AddColumn(string header, string dataProp, bool visible, int fillWeight = 10)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.HeaderText = header;
            col.DataPropertyName = dataProp;
            col.Name = dataProp;
            col.Visible = visible;
            col.FillWeight = fillWeight;
            col.ReadOnly = true;
            dgPrescriptionDetails.Columns.Add(col);
        }

        private void _ResetFormat(string Status)
        {
            lbNotReady.Text = "(Not Ready for Dispensing)";
            lbX.Text = "✕";
            lbStatus.Text = Status;
            panbtnDispense.Enabled = false;
            panbtnSendToAccounting.Enabled = false;
            panbtnCancelOrder.Enabled = false;
            dgPrescriptionDetails.EditMode = DataGridViewEditMode.EditProgrammatically;

            // Handle UI elements and workflow based on Prescription Status
            if (Status == "Pending")
            {
                lbStatus.ForeColor = Color.Teal;
                lbNotReady.ForeColor = Color.Teal;
                lbX.ForeColor = Color.Teal;
                lbContainer.BackColor = Color.Teal;

                panbtnSendToAccounting.Enabled = true;
                panbtnCancelOrder.Enabled = true;

                dgPrescriptionDetails.ReadOnly = false;
                dgPrescriptionDetails.EditMode = DataGridViewEditMode.EditOnEnter;
            }
            else if (Status == "Waiting For Payment")
            {
                lbStatus.ForeColor = Color.Brown;
                lbNotReady.ForeColor = Color.Brown;
                lbX.ForeColor = Color.Brown;
                lbContainer.BackColor = Color.Brown;
                panbtnSendToAccounting.Enabled = false;
            }
            else if (Status == "Ready For Dispensing")
            {
                lbStatus.ForeColor = Color.Green;
                lbNotReady.ForeColor = Color.Green;
                lbX.ForeColor = Color.Green;
                lbContainer.BackColor = Color.Green;
                lbNotReady.Text = "(Ready for Dispensing)";
                lbX.Text = "✓";
                panbtnDispense.Enabled = true;
            }
            else if (Status == "PartiallyDispensed")
            {
                lbStatus.ForeColor = Color.GreenYellow;
                lbNotReady.ForeColor = Color.GreenYellow;
                lbX.ForeColor = Color.GreenYellow;
                lbContainer.ForeColor = Color.GreenYellow;
                btnDispense.Enabled = true;
                panbtnSendToAccounting.Enabled = true;
            }
        }

        private void _SetupPrescriptionGrid()
        {
            dgvPrescription.AutoGenerateColumns = false;
            dgvPrescription.Columns.Clear();
            dgvPrescription.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPrescription.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPrescription.MultiSelect = false;

            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "VisitID", DataPropertyName = "VisitID", Visible = false });
            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "AppointmentID", DataPropertyName = "AppointmentID", Visible = false });
            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrescriptionID", DataPropertyName = "PrescriptionID", FillWeight = 7 });
            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "PatientName", HeaderText = "Patient", DataPropertyName = "PatientName", ReadOnly = true, FillWeight = 15 });
            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "DoctorName", HeaderText = "Doctor", DataPropertyName = "DoctorName", ReadOnly = true, FillWeight = 15 });
            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrescriptionTime", HeaderText = "Time", DataPropertyName = "PrescriptionTime", ReadOnly = true, FillWeight = 7 });
            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", DataPropertyName = "Status", ReadOnly = true, FillWeight = 15 });
            dgvPrescription.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrescriptionType", HeaderText = "Type", DataPropertyName = "PrescriptionType", ReadOnly = true, FillWeight = 10 });

            dgvPrescription.DataSource = dtAllActivePrescriptions;
        }

        private void _SetupPrescriptionDetailsGrid()
        {
            dgPrescriptionDetails.AutoGenerateColumns = false;
            dgPrescriptionDetails.Columns.Clear();
            dgPrescriptionDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgPrescriptionDetails.ReadOnly = false;

            _AddColumn("Status", "AvailableStatus", true, 30);
            _AddColumn("ID", "PrescriptionDetailsID", false);
            _AddColumn("PID", "PrescriptionID", false);
            _AddColumn("Medicine", "MedicineName", true, 20);
            _AddColumn("Price", "SavedMedicinePrice", true, 20);
            _AddColumn("Discount", "DiscountAmount", true, 10);
            _AddColumn("Dosage", "Dosage", true, 15);
            _AddColumn("Freq", "Frequency", true, 10);
            _AddColumn("Req Qty", "RequiredQuantity", true, 10);

            // Add editable quantity column
            DataGridViewTextBoxColumn colQty = new DataGridViewTextBoxColumn();
            colQty.HeaderText = "Disp Qty";
            colQty.DataPropertyName = "DispensedQuantity";
            colQty.Name = "DispensedQuantity";
            colQty.ReadOnly = false;
            colQty.FillWeight = 10;
            dgPrescriptionDetails.Columns.Add(colQty);

            // Add editable checkbox column
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
            {
                Name = "IsDispensed",
                HeaderText = "Dispensed",
                DataPropertyName = "IsDispensed",
                ReadOnly = false,
                FillWeight = 10
            };
            dgPrescriptionDetails.Columns.Add(chk);

            _AddColumn("Instr.", "Instructions", false, 15);
        }

        private int _GetMaxAvailable(DataGridViewRow row)
        {
            if (row.Cells["AvailableStatus"].Value == null) return 0;

            string status = row.Cells["AvailableStatus"].Value.ToString();
            if (status == "Fully Available")
                return Convert.ToInt32(row.Cells["RequiredQuantity"].Value);

            if (status.StartsWith("Partially Available"))
            {
                try
                {
                    string numStr = status.Substring(status.IndexOf("(") + 1, status.IndexOf(")") - status.IndexOf("(") - 1);
                    return int.Parse(numStr);
                }
                catch
                {
                    return 0;
                }
            }
            return 0;
        }

        private void _CalculatePrescriptionTotals(DataView dv, out decimal totalNet, out decimal totalTax)
        {
            totalNet = 0;
            totalTax = 0;

            foreach (DataRowView rowView in dv)
            {
                DataRow row = rowView.Row;
                if (row.RowState == DataRowState.Deleted) continue;

                if (Convert.ToBoolean(row["IsDispensed"] ?? false))
                {
                    decimal price = Convert.ToDecimal(row["SavedMedicinePrice"] ?? 0);
                    int qty = Convert.ToInt32(row["DispensedQuantity"] ?? 0);
                    decimal discount = Convert.ToDecimal(row["DiscountAmount"] ?? 0);
                    decimal taxRate = Convert.ToDecimal(row["TaxRate"] ?? 0) / 100;

                    decimal itemNet = (price * qty) - discount;
                    decimal itemTax = itemNet * taxRate;

                    totalNet += itemNet;
                    totalTax += itemTax;
                }
            }
        }

        private void _RefrashData()
        {
            dtAllDetails = clsPrescription.GetAllPrescriptionDetails();
            dgPrescriptionDetails.DataSource = dtAllDetails;

            dtAllActivePrescriptions = clsPrescription.GetAllActivePrescriptions();
            dgvPrescription.DataSource = dtAllActivePrescriptions;
        }

        private void frmPrescriptionDispnsing_Load(object sender, EventArgs e)
        {
            dtAllDetails = clsPrescription.GetAllPrescriptionDetails();
            dtAllDetails.DefaultView.AllowEdit = true;
            dtAllDetails.Columns["DispensedQuantity"].ReadOnly = false;
            dtAllDetails.Columns["IsDispensed"].ReadOnly = false;

            if (clsGlobal.CurrentUser?.PersonInfo != null)
                lbCurrentUser.Text = clsGlobal.CurrentUser.PersonInfo.Name;

            _SetupPrescriptionGrid();
            _SetupPrescriptionDetailsGrid();
        }

        private void dgPrescriptionDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            string columnName = dgPrescriptionDetails.Columns[e.ColumnIndex].Name;

            if (columnName == "DispensedQuantity" || columnName == "IsDispensed")
            {
                if (dgPrescriptionDetails.Rows[e.RowIndex].Cells["AvailableStatus"].Value == null) return;

                string status = dgPrescriptionDetails.Rows[e.RowIndex].Cells["AvailableStatus"].Value.ToString();

                if (status == "Out of Stock")
                {
                    e.Cancel = true;
                    MessageBox.Show("This item is completely out of stock and cannot be modified.", "Stock Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        private void btnSendToAccounting_Click(object sender, EventArgs e)
        {
            dgPrescriptionDetails.EndEdit();
            this.BindingContext[dgPrescriptionDetails.DataSource].EndCurrentEdit();

            DataView dv = (DataView)dgPrescriptionDetails.DataSource;
            if (dv == null) return;

            bool hasDispensedItems = false;
            foreach (DataRowView row in dv)
            {
                bool isDispensed = (row["IsDispensed"] != DBNull.Value) && Convert.ToBoolean(row["IsDispensed"]);
                if (isDispensed)
                {
                    hasDispensedItems = true;
                    break;
                }
            }

            if (!hasDispensedItems)
            {
                MessageBox.Show("Cannot proceed: Please select at least one item to dispense by checking the 'Dispensed' box.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Optional: recalculate before sending if necessary
            _CalculatePrescriptionTotals(dv, out TotalMedicinesAmount, out TaxRate);

            if (TotalMedicinesAmount <= 0)
            {
                MessageBox.Show("Total amount must be greater than zero. Please check dispensed quantities.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clsPrescription.SendToCashier(_currentPrescriptionId, dv.ToTable(), _VisitID, clsGlobal.CurrentUser.UserID))
            {
                MessageBox.Show("Prescription billing details have been successfully forwarded to accounting.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _RefrashData();
            }
            else
            {
                MessageBox.Show("Failed to forward the prescription invoice to accounting. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPrescription_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPrescription.SelectedRows.Count > 0)
            {
                var selectedRow = dgvPrescription.SelectedRows[0];

                lbPatientName.Text = selectedRow.Cells["PatientName"].Value?.ToString() + ")";
                lbPatientNameDetalis.Text = $"[{lbPatientName.Text}]";
                _currentPrescriptionId = Convert.ToInt32(selectedRow.Cells["PrescriptionID"].Value);
                lbOrderID.Text = $"(Order [{_currentPrescriptionId.ToString()}] )";
                _AppointmentID = Convert.ToInt32(selectedRow.Cells["AppointmentID"].Value);
                _VisitID = Convert.ToInt32(selectedRow.Cells["VisitID"].Value);

                string status = selectedRow.Cells["Status"].Value?.ToString() ?? "";
                _ResetFormat(status);

                dtAllDetails.DefaultView.RowFilter = $"PrescriptionID = {_currentPrescriptionId}";
                dtAllDetails.Columns["IsDispensed"].ReadOnly = false;
                dgPrescriptionDetails.DataSource = dtAllDetails.DefaultView;

                DataView dv = (DataView)dgPrescriptionDetails.DataSource;
                _CalculatePrescriptionTotals(dv, out TotalMedicinesAmount, out TaxRate);

                lbTotalMedicen.Text = "$ " + (TotalMedicinesAmount + TaxRate).ToString("N2");
            }
        }

        private void dgPrescriptionDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdating || e.RowIndex < 0) return;

            _isUpdating = true;

            try
            {
                string colName = dgPrescriptionDetails.Columns[e.ColumnIndex].Name;
                DataGridViewRow row = dgPrescriptionDetails.Rows[e.RowIndex];
                DataRowView rowView = (DataRowView)row.DataBoundItem;

                string status = row.Cells["AvailableStatus"].Value?.ToString() ?? "";
                int maxAvailable = _GetMaxAvailable(row);

                if (colName == "IsDispensed")
                {
                    object val = row.Cells["IsDispensed"].Value;
                    bool isChecked = (val != null && val != DBNull.Value) && Convert.ToBoolean(val);

                    if (isChecked && status == "Out of Stock")
                    {
                        rowView["IsDispensed"] = false;
                    }
                    else if (isChecked)
                    {
                        rowView["DispensedQuantity"] = maxAvailable;
                    }
                    else
                    {
                        rowView["DispensedQuantity"] = 0;
                    }
                }
                else if (colName == "DispensedQuantity")
                {
                    int newQty = Convert.ToInt32(row.Cells["DispensedQuantity"].Value ?? 0);

                    if (newQty > maxAvailable)
                    {
                        MessageBox.Show($"Requested quantity exceeds stock limits. Maximum available for this item is {maxAvailable}.", "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rowView["DispensedQuantity"] = maxAvailable;
                    }
                    else
                    {
                        rowView["IsDispensed"] = (newQty > 0);
                    }
                }

                rowView.EndEdit();

                DataView dv = (DataView)dgPrescriptionDetails.DataSource;
                _CalculatePrescriptionTotals(dv, out TotalMedicinesAmount, out TaxRate);
                lbTotalMedicen.Text = "$ " + (TotalMedicinesAmount + TaxRate).ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating row information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void dgPrescriptionDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgPrescriptionDetails.Columns[e.ColumnIndex].Name == "DispensedQuantity")
            {
                DataGridViewRow row = dgPrescriptionDetails.Rows[e.RowIndex];
                int maxAvailable = _GetMaxAvailable(row);

                if (maxAvailable <= 0)
                {
                    _qtyNumericUpDown.Visible = false;
                    return;
                }

                _qtyNumericUpDown.Maximum = maxAvailable;

                Rectangle rect = dgPrescriptionDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                _qtyNumericUpDown.Location = rect.Location;
                _qtyNumericUpDown.Size = rect.Size;

                if (row.Cells[e.ColumnIndex].Value != DBNull.Value && row.Cells[e.ColumnIndex].Value != null)
                {
                    _qtyNumericUpDown.Value = Convert.ToDecimal(row.Cells[e.ColumnIndex].Value);
                }
                else
                {
                    _qtyNumericUpDown.Value = 0;
                }

                _qtyNumericUpDown.Visible = true;
                _qtyNumericUpDown.Focus();
            }
            else
            {
                _qtyNumericUpDown.Visible = false;
            }
        }

        private void dgPrescriptionDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void QtyNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (dgPrescriptionDetails.CurrentCell != null && dgPrescriptionDetails.CurrentCell.OwningColumn.Name == "DispensedQuantity")
            {
                dgPrescriptionDetails.CurrentCell.Value = (int)_qtyNumericUpDown.Value;
            }
        }

        private void QtyNumericUpDown_Leave(object sender, EventArgs e)
        {
            _qtyNumericUpDown.Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e) => this.Close();

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lblPlaceholderSer.Visible = string.IsNullOrEmpty(txtSearch.Text);
            string filter = string.IsNullOrEmpty(txtSearch.Text) ? "" : $"PatientName LIKE '%{txtSearch.Text}%'";
            dtAllActivePrescriptions.DefaultView.RowFilter = filter;
        }

        private void btnPrintShotageSlip_Click(object sender, EventArgs e)
        {
            if (dgPrescriptionDetails.Rows.Count == 0)
            {
                MessageBox.Show("No prescription detail items available to scan for shortage.", "Report Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            System.Drawing.Printing.PrintDocument printDoc = new System.Drawing.Printing.PrintDocument();
            printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(PrintShortageReport_PrintPage);

            PrintPreviewDialog previewDlg = new PrintPreviewDialog();
            previewDlg.Document = printDoc;
            previewDlg.WindowState = FormWindowState.Maximized;
            previewDlg.ShowDialog();
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (_currentPrescriptionId == -1) return;

            if (MessageBox.Show("Are you sure you want to cancel this prescription order?", "Confirm Cancellation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsPrescription.UpdatePrescriptionStatus(_currentPrescriptionId, clsPrescription.enPrescriptionStatus.Cancelled))
                {
                    MessageBox.Show("The prescription order has been successfully cancelled.", "Order Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefrashData();
                }
                else
                {
                    MessageBox.Show("Failed to cancel the prescription order. Please check system logs.", "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDispense_Click(object sender, EventArgs e)
        {
            if (_currentPrescriptionId == -1) return;

            if (MessageBox.Show("Confirm final medicine fulfillment and closure of this order?", "Confirm Dispense",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsPrescription.UpdatePrescriptionStatus(_currentPrescriptionId, clsPrescription.enPrescriptionStatus.Dispensed))
                {
                    MessageBox.Show("The items have been marked as fully dispensed, and inventory levels updated.", "Order Fulfilled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefrashData();
                }
                else
                {
                    MessageBox.Show("Fulfillment request failed. Please verify medication stock balance values.", "Fulfillment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblPlaceholderSer_Click(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }
    }
}