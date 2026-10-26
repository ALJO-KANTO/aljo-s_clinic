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

            BuildPharmacistDesign();
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
            UpdatePharmacistDisplay();
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
            ApplyPrescriptionGridStyle();
            UpdatePharmacistDisplay();
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
                UpdatePharmacistDisplay();
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
                UpdatePharmacistDisplay();
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

        // ==================== PHARMACIST STATION DESIGN ====================
        // No NuGet packages, images, or designer edits are required.
        private static readonly Color RxDark = Color.FromArgb(0, 63, 68);
        private static readonly Color RxTeal = Color.FromArgb(0, 112, 113);
        private static readonly Color RxAqua = Color.FromArgb(19, 196, 187);
        private static readonly Color RxInk = Color.FromArgb(18, 53, 67);
        private static readonly Color RxCanvas = Color.FromArgb(244, 250, 251);

        private Label rxPatientHeader, rxDetailsPatient, rxUser, rxOrder, rxTotal, rxStatus, rxStatusNote;
        private Button rxDispense, rxAccounting, rxCancel;
        private Panel rxRoot;

        private sealed class RxRoundedPanel : Panel
        {
            public int CornerRadius { get; set; } = 14;
            public Color Stroke { get; set; } = Color.FromArgb(220, 236, 239);
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius))
                using (var pen = new Pen(Stroke)) e.Graphics.DrawPath(pen, path);
            }
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundRect(Rectangle r, int radius)
        {
            int d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            var p = new System.Drawing.Drawing2D.GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static Label RxLabel(string text, int size, bool bold, Color color)
        {
            return new Label { Text = text, ForeColor = color, BackColor = Color.Transparent,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
        }

        private static Panel RxSection(string title, out Label rightText)
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 63, BackColor = RxDark, Padding = new Padding(22, 8, 16, 8) };
            var heading = RxLabel("▤   " + title, 16, true, Color.White);
            heading.Dock = DockStyle.Fill;
            rightText = RxLabel("", 10, true, Color.FromArgb(202, 255, 251));
            rightText.Dock = DockStyle.Right;
            rightText.Width = 265;
            rightText.TextAlign = ContentAlignment.MiddleRight;
            bar.Controls.Add(heading);
            bar.Controls.Add(rightText);
            return bar;
        }

        private static Button RxAction(string icon, string title, string subtitle, Color bg, Color fg, int height)
        {
            var button = new Button { Dock = DockStyle.Top, Height = height, Margin = Padding.Empty,
                FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = fg,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 6, 0), Cursor = Cursors.Hand,
                Text = icon + "    " + title + "    ›" + (string.IsNullOrEmpty(subtitle) ? "" : "\n         " + subtitle) };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }


        // Reference artwork is loaded from Assets next to the executable.
        // Set each PNG Build Action=Content, Copy to Output Directory=Copy if newer.
        private static Image RxAsset(string name)
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", name + ".png");
            if (!File.Exists(file)) return null;
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var image = Image.FromStream(stream)) return new Bitmap(image);
        }
        private static void RxArt(Button button, string name)
        {
            Image image = RxAsset(name);
            if (image == null) return;
            button.BackgroundImage = image;
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.Text = "";
            button.FlatAppearance.MouseOverBackColor = button.BackColor;
            button.FlatAppearance.MouseDownBackColor = button.BackColor;
        }
        private static PictureBox RxIcon(string name, int width)
        {
            return new PictureBox { Dock = DockStyle.Left, Width = width,
                Image = RxAsset(name), SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent, Margin = Padding.Empty };
        }

        private void BuildPharmacistDesign()
        {
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10);
            BackColor = RxCanvas;
            MinimumSize = new Size(990, 640);
            if (Width < 1200) Width = 1280;
            if (Height < 730) Height = 790;
            Text = "Clinic Management System — Pharmacist Station";

            // Reparent existing working grids/search controls. All existing events remain attached.
            Controls.Clear();
            rxRoot = new Panel { Dock = DockStyle.Fill, BackColor = RxCanvas };
            Controls.Add(rxRoot);

            var header = new Panel { Dock = DockStyle.Top, Height = 102, BackColor = RxDark, Padding = new Padding(24, 12, 20, 12) };
            rxRoot.Controls.Add(header);
            var logo = RxLabel("💊", 32, true, Color.White);
            logo.Dock = DockStyle.Left; logo.Width = 84;
            header.Controls.Add(logo);
            logo.Text = ""; logo.Controls.Add(RxIcon("pharmacy_logo", 76));
            var brand = RxLabel("Pharmacist Station", 23, true, Color.White);
            brand.Dock = DockStyle.Left; brand.Width = 340;
            header.Controls.Add(brand);
            var divider = new Panel { Dock = DockStyle.Left, Width = 2, BackColor = RxAqua, Margin = new Padding(10) };
            header.Controls.Add(divider);
            var subtitle = RxLabel("    Prescription Dispensing", 20, false, Color.FromArgb(223, 247, 247));
            subtitle.Dock = DockStyle.Fill;
            header.Controls.Add(subtitle);
            var userPanel = new Panel { Dock = DockStyle.Right, Width = 245, BackColor = RxDark };
            header.Controls.Add(userPanel);
            rxOrder = RxLabel("", 9, false, RxInk);
            rxUser = RxLabel("●   Current User\n      —", 11, true, Color.FromArgb(149, 250, 239));
            userPanel.Controls.Add(rxUser);
            var close = new Button { Text = "×", Dock = DockStyle.Right, Width = 35,
                Font = new Font("Segoe UI", 21), ForeColor = RxAqua, BackColor = RxDark, FlatStyle = FlatStyle.Flat };
            close.FlatAppearance.BorderSize = 0;
            close.Click += btnExit_Click;
            userPanel.Controls.Add(close);

            var body = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = RxCanvas,
                ColumnCount = 2, RowCount = 1, Padding = new Padding(16, 14, 16, 12) };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            rxRoot.Controls.Add(body);
            body.BringToFront();
            header.BringToFront();

            var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                Padding = new Padding(0, 0, 12, 0) };
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
            body.Controls.Add(left, 0, 0);

            var topCard = new RxRoundedPanel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 8) };
            var topInner = new Panel { Dock = DockStyle.Fill };
            topCard.Controls.Add(topInner);
            var topBar = RxSection("Tab 1: Pending Prescriptions", out rxPatientHeader);
            topBar.Controls.Add(RxIcon("section_icon", 53));
            topInner.Controls.Add(topBar);
            var searchWrap = new Panel { Dock = DockStyle.Bottom, Height = 58, Padding = new Padding(6, 8, 6, 4) };
            topInner.Controls.Add(searchWrap);
            txtSearch.Dock = DockStyle.Fill;
            txtSearch.Font = new Font("Segoe UI", 11);
            searchWrap.Controls.Add(txtSearch);
            // Existing placeholder is intentionally hidden: TextBox gets its own visible placeholder.
            lblPlaceholderSer.Visible = false;
            var topGridHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 4) };
            topInner.Controls.Add(topGridHost);
            topGridHost.Controls.Add(dgvPrescription);
            dgvPrescription.Dock = DockStyle.Fill;
            topGridHost.BringToFront();
            topBar.BringToFront();
            searchWrap.BringToFront();
            left.Controls.Add(topCard, 0, 0);

            var bottomCard = new RxRoundedPanel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10), Margin = new Padding(0, 8, 0, 0) };
            var bottomInner = new Panel { Dock = DockStyle.Fill };
            bottomCard.Controls.Add(bottomInner);
            var bottomBar = RxSection("Tab 2: Prescription Details", out rxDetailsPatient);
            bottomBar.Controls.Add(RxIcon("details_icon", 53));
            bottomInner.Controls.Add(bottomBar);
            var detailsHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
            bottomInner.Controls.Add(detailsHost);
            detailsHost.Controls.Add(dgPrescriptionDetails);
            dgPrescriptionDetails.Dock = DockStyle.Fill;
            detailsHost.BringToFront();
            bottomBar.BringToFront();
            left.Controls.Add(bottomCard, 0, 1);

            var right = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(4, 0, 0, 0) };
            body.Controls.Add(right, 1, 0);
            var stack = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1, RowCount = 7, BackColor = RxCanvas };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            right.Controls.Add(stack);

            rxDispense = RxAction("💊", "Dispense Medication", "", Color.FromArgb(207, 249, 240), RxInk, 92);
            rxAccounting = RxAction("▤", "Send To Accounting", "Prepare Invoice", Color.FromArgb(220, 237, 255), Color.FromArgb(16, 75, 142), 92);
            rxCancel = RxAction("✖", "Cancel Order", "", Color.FromArgb(255, 218, 224), Color.FromArgb(218, 24, 55), 74);
            RxArt(rxDispense, "dispense_card");
            RxArt(rxAccounting, "accounting_card");
            RxArt(rxCancel, "cancel_card");
            rxDispense.Click += btnDispense_Click;
            rxAccounting.Click += btnSendToAccounting_Click;
            rxCancel.Click += btnCancelOrder_Click;
            AddSide(stack, rxDispense, 0);
            AddSide(stack, rxAccounting, 1);
            AddSide(stack, rxCancel, 2);

            var shortage = RxAction("▥", "Report Shortage", "", RxDark, Color.White, 61);
            shortage.Click += btnPrintShotageSlip_Click;
            AddSide(stack, shortage, 3);

            var nextCard = new RxRoundedPanel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Padding = new Padding(12, 8, 12, 8) };
            nextCard.Controls.Add(RxLabel("◷   Next Expected Patient\n\n      —", 10, true, RxTeal));
            AddSide(stack, nextCard, 4);

            var totalCard = new RxRoundedPanel { Dock = DockStyle.Top, Height = 86, BackColor = Color.White, Padding = new Padding(12, 8, 12, 8) };
            rxTotal = RxLabel("$   Total Medicine\n     $ 0.00", 15, true, Color.FromArgb(0, 154, 112));
            totalCard.Controls.Add(rxTotal);
            AddSide(stack, totalCard, 5);

            var statusCard = new RxRoundedPanel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(255, 235, 239), Padding = new Padding(12, 5, 12, 5) };
            rxStatus = RxLabel("⚠   Unknown", 13, true, Color.FromArgb(228, 34, 66));
            rxStatus.Dock = DockStyle.Top; rxStatus.Height = 40;
            rxStatusNote = RxLabel("(Not Ready for Dispensing)", 9, false, Color.FromArgb(228, 34, 66));
            statusCard.Controls.Add(rxStatusNote);
            statusCard.Controls.Add(rxStatus);
            AddSide(stack, statusCard, 6);
            ResumeLayout(true);
        }

        private static void AddSide(TableLayoutPanel table, Control control, int row)
        {
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var wrapper = new Panel { Height = control.Height + 9, Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, 9), Margin = Padding.Empty };
            control.Dock = DockStyle.Fill;
            wrapper.Controls.Add(control);
            table.Controls.Add(wrapper, 0, row);
        }

        private void ApplyPrescriptionGridStyle()
        {
            StyleRxGrid(dgvPrescription);
            StyleRxGrid(dgPrescriptionDetails);
        }

        private static void StyleRxGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(225, 239, 241);
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = RxDark;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 44;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.ForeColor = RxInk;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(213, 245, 244);
            grid.DefaultCellStyle.SelectionForeColor = RxInk;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 250, 251);
            grid.RowTemplate.Height = 41;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void UpdatePharmacistDisplay()
        {
            if (rxPatientHeader == null) return;
            string patient = "";
            if (dgvPrescription.SelectedRows.Count > 0)
                patient = Convert.ToString(dgvPrescription.SelectedRows[0].Cells["PatientName"].Value) ?? "";
            rxPatientHeader.Text = string.IsNullOrWhiteSpace(patient) ? "Patient: —" : "●   Patient: " + patient;
            rxDetailsPatient.Text = string.IsNullOrWhiteSpace(patient) ? "" : "[" + patient + "]";
            rxUser.Text = "●   Current User\n      " + (clsGlobal.CurrentUser?.PersonInfo?.Name ?? "—");
            rxOrder.Text = _currentPrescriptionId < 0 ? "" : "Order " + _currentPrescriptionId;
            rxTotal.Text = "$   Total Medicine\n     " + lbTotalMedicen.Text;
            string status = lbStatus.Text ?? "Unknown";
            rxStatus.Text = "⚠   " + status;
            rxStatusNote.Text = lbNotReady.Text;
            bool pending = status.Equals("Pending", StringComparison.OrdinalIgnoreCase);
            bool ready = status.Equals("Ready For Dispensing", StringComparison.OrdinalIgnoreCase);
            bool partial = status.Equals("PartiallyDispensed", StringComparison.OrdinalIgnoreCase);
            rxDispense.Enabled = ready || partial;
            rxAccounting.Enabled = pending || partial;
            rxCancel.Enabled = pending;
            if (ready || partial)
            {
                rxStatus.ForeColor = Color.FromArgb(0, 133, 88);
                rxStatusNote.ForeColor = rxStatus.ForeColor;
            }
            else
            {
                rxStatus.ForeColor = Color.FromArgb(228, 34, 66);
                rxStatusNote.ForeColor = rxStatus.ForeColor;
            }
        }
    }
}