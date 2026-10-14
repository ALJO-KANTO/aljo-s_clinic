using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace Clinic.Medical_Services.Casher
{
    public partial class frmManageBills : Form
    {
        private DataView _dvBills = new DataView();
        private DataTable _dtBills = new DataTable();

        public frmManageBills()
        {
            InitializeComponent();

            txtSearchByPatientName.TextChanged +=
                txtSearchByPatientName_TextChanged;

            dateTimePicker1.ValueChanged +=
                dateTimePicker1_ValueChanged;

            chePending.CheckedChanged +=
                FilterStatus_CheckedChanged;

            chePartial.CheckedChanged +=
                FilterStatus_CheckedChanged;

            chePaid.CheckedChanged +=
                FilterStatus_CheckedChanged;

            CheckBox chkAllDates = this.Controls
                .Find("chkAllDates", true)
                .OfType<CheckBox>()
                .FirstOrDefault();

            if (chkAllDates != null)
            {
                chkAllDates.CheckedChanged -= chkAllDates_CheckedChanged;
                chkAllDates.CheckedChanged += chkAllDates_CheckedChanged;
            }

            dgMasterBillList.CellFormatting +=
                dgMasterBillList_CellFormatting;

            dgMasterBillList.CellContentClick +=
                dgMasterBillList_CellContentClick;
        }
        private void frmManageBills_Load(object sender, EventArgs e)
        {
            lbCurrentUser.Text = clsGlobal.PersonName;

            chePending.Checked = true;
            chePartial.Checked = true;
            chePaid.Checked = true;

            // Show all dates initially
            CheckBox chkAllDates = this.Controls
                .Find("chkAllDates", true)
                .OfType<CheckBox>()
                .FirstOrDefault();

            if (chkAllDates != null)
                chkAllDates.Checked = true;

            _RefreshBillsList();
        }

        private void _RefreshBillsList()
        {
            try
            {
                // 1. Load bills from MySQL
                _dtBills = clsBillingService.GetAllBills();

                if (_dtBills == null)
                    _dtBills = new DataTable();

                // 2. Check only the columns actually needed
                string[] requiredColumns =
                {
            "BillID",
            "BillDate",
            "Status",
            "PatientName"
        };

                foreach (string column in requiredColumns)
                {
                    if (!_dtBills.Columns.Contains(column))
                    {
                        MessageBox.Show(
                            "Missing billing column: " + column,
                            "Billing Column Missing",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        return;
                    }
                }

                // 3. Create DataView
                _dvBills = new DataView(_dtBills);

                // 4. Bind to the actual billing grid
                dgMasterBillList.DataSource = _dvBills;

                string[] hiddenColumns =
                {
                    "BillID", "VisitID", "PatientID", "PersonID",
                    "DoctorID", "AppointmentID", "CreatedByUserID", "PaymentStatus"
                };
                foreach (string column in hiddenColumns)
                {
                    if (dgMasterBillList.Columns.Contains(column))
                        dgMasterBillList.Columns[column].Visible = false;
                }

                dgMasterBillList.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
                dgMasterBillList.ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgMasterBillList.ColumnHeadersHeight = 42;

                if (dgMasterBillList.Columns.Contains("BillNumber"))
                    dgMasterBillList.Columns["BillNumber"].HeaderText = "Bill Number";
                if (dgMasterBillList.Columns.Contains("PatientName"))
                    dgMasterBillList.Columns["PatientName"].HeaderText = "Patient Name";
                if (dgMasterBillList.Columns.Contains("TotalAmount"))
                    dgMasterBillList.Columns["TotalAmount"].HeaderText = "Total Amount";
                if (dgMasterBillList.Columns.Contains("PaidAmount"))
                    dgMasterBillList.Columns["PaidAmount"].HeaderText = "Paid Amount";
                if (dgMasterBillList.Columns.Contains("BalanceDue"))
                    dgMasterBillList.Columns["BalanceDue"].HeaderText = "Balance Due";

                // 5. Apply date, status and patient filters
                _ApplyAdvancedFilter();

                // 6. Add action buttons
                _AddActionButtonsOnce();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Unable to Load Bills",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private int GetPrescriptionCount(string itemsTotal)
        {
            if (string.IsNullOrWhiteSpace(itemsTotal))
                return 0;

            Match match = Regex.Match(itemsTotal, @"(\d+)\s+Presc");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        private int GetPharmacySaleCount(string itemsTotal)
        {
            if (string.IsNullOrWhiteSpace(itemsTotal))
                return 0;

            Match match = Regex.Match(itemsTotal, @"(\d+)\s+Pharmacy Sale");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        private int GetServiceCount(string itemsTotal)
        {
            if (string.IsNullOrWhiteSpace(itemsTotal))
                return 0;

            Match match = Regex.Match(itemsTotal, @"(\d+)\s+Serv");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        private bool IsEmptyBill(string itemsTotal)
        {
            int serv = GetServiceCount(itemsTotal);
            int presc = GetPrescriptionCount(itemsTotal);
            int pharm = GetPharmacySaleCount(itemsTotal);

            return serv == 0 && presc == 0 && pharm == 0;
        }

        private void _UpdateKPICards(DataTable dt)
        {
            lblCountPending.Text = "0";
            lblCountPartial.Text = "0";
            lblCountPaid.Text = "0";

            if (dt == null || dt.Rows.Count == 0)
                return;

            CheckBox chkAllDates = this.Controls
                .Find("chkAllDates", true)
                .OfType<CheckBox>()
                .FirstOrDefault();

            bool showAllDates =
                chkAllDates != null && chkAllDates.Checked;

            DateTime selectedDate = dateTimePicker1.Value.Date;

            int pendingCount = 0;
            int partialCount = 0;
            int paidCount = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["BillDate"] == DBNull.Value)
                    continue;

                DateTime billDate =
                    Convert.ToDateTime(row["BillDate"]).Date;

                if (!showAllDates && billDate != selectedDate)
                    continue;

                string status = Convert.ToString(row["Status"])
                    .Trim();

                switch (status)
                {
                    case "Pending":
                        pendingCount++;
                        break;

                    case "Partial":
                        partialCount++;
                        break;

                    case "Paid":
                        paidCount++;
                        break;
                }
            }

            lblCountPending.Text = pendingCount.ToString();
            lblCountPartial.Text = partialCount.ToString();
            lblCountPaid.Text = paidCount.ToString();
        }

        private void _ApplyAdvancedFilter()
        {
            if (_dtBills == null || _dvBills == null)
                return;

            if (!_dtBills.Columns.Contains("Status") ||
                !_dtBills.Columns.Contains("BillDate") ||
                !_dtBills.Columns.Contains("PatientName"))
                return;

            List<string> filters = new List<string>();

            // 1. DATE FILTER
            CheckBox chkAllDates = this.Controls
                .Find("chkAllDates", true)
                .OfType<CheckBox>()
                .FirstOrDefault();

            bool allDates = chkAllDates != null && chkAllDates.Checked;

            if (!allDates)
            {
                DateTime selectedDate = dateTimePicker1.Value.Date;

                string fromDate = selectedDate.ToString(
                    "MM/dd/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);

                string toDate = selectedDate.AddDays(1).ToString(
                    "MM/dd/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);

                filters.Add(
                    $"BillDate >= #{fromDate}# AND BillDate < #{toDate}#");
            }

            // 2. STATUS FILTER
            List<string> selectedStatuses = new List<string>();

            if (chePending.Checked)
                selectedStatuses.Add("'Pending'");

            if (chePartial.Checked)
                selectedStatuses.Add("'Partial'");

            if (chePaid.Checked)
                selectedStatuses.Add("'Paid'");

            if (selectedStatuses.Count == 0)
            {
                filters.Add("1 = 0");
            }
            else
            {
                filters.Add(
                    "Status IN (" + string.Join(",", selectedStatuses) + ")");
            }

            // 3. PATIENT NAME FILTER
            string patientName = txtSearchByPatientName.Text.Trim();

            if (!string.IsNullOrWhiteSpace(patientName))
            {
                patientName = patientName
                    .Replace("'", "''")
                    .Replace("[", "[[]")
                    .Replace("%", "[%]")
                    .Replace("*", "[*]");

                filters.Add(
                    $"PatientName LIKE '%{patientName}%'");
            }

            // 4. APPLY FILTER
            _dvBills.RowFilter = string.Join(" AND ", filters);

            // 5. REFRESH GRID
            dgMasterBillList.Refresh();

            // 6. UPDATE COUNTS
            _UpdateKPICards(_dtBills);
        }
        private void txtSearchByPatientName_TextChanged(object sender, EventArgs e) => _ApplyAdvancedFilter();
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) => _ApplyAdvancedFilter();
        private void FilterStatus_CheckedChanged(object sender, EventArgs e) => _ApplyAdvancedFilter();

        private void _AddActionButtonsOnce()
        {
            if (dgMasterBillList.Columns.Contains("BillID")) dgMasterBillList.Columns["BillID"].Visible = false;

            if (dgMasterBillList.Columns.Contains("btnPayAction")) return;

            // الزر الأول: الدفع الكامل والإنهاء
            DataGridViewButtonColumn payColumn = new DataGridViewButtonColumn();
            payColumn.Name = "btnPayAction";
            payColumn.HeaderText = "Full Pay";
            payColumn.Text = "💳 Pay All";
            payColumn.UseColumnTextForButtonValue = true;
            payColumn.FlatStyle = FlatStyle.Flat;
            dgMasterBillList.Columns.Add(payColumn);

            // الزر الثاني: زر ديناميكي (تعديل ودفع جزئي في حالة المعلق / أو ارتجاع في حالة المدفوع)
            DataGridViewButtonColumn editColumn = new DataGridViewButtonColumn();
            editColumn.Name = "btnEditAction";
            editColumn.HeaderText = "Adjustment";
            editColumn.Text = "📝 Edit";
            editColumn.UseColumnTextForButtonValue = true;
            editColumn.FlatStyle = FlatStyle.Flat;
            dgMasterBillList.Columns.Add(editColumn);

            // الزر الثالث: زر الإلغاء النهائي (يظهر فقط للفواتير المعلقة)
            DataGridViewButtonColumn cancelColumn = new DataGridViewButtonColumn();
            cancelColumn.Name = "btnCancelAction";
            cancelColumn.HeaderText = "Cancel Bill";
            cancelColumn.Text = "❌ Cancel";
            cancelColumn.UseColumnTextForButtonValue = true;
            cancelColumn.FlatStyle = FlatStyle.Flat;
            dgMasterBillList.Columns.Add(cancelColumn);

            // ترتيب الأعمدة لتظهر أقصى اليمين بشكل منظم
            payColumn.DisplayIndex = dgMasterBillList.Columns.Count - 3;
            editColumn.DisplayIndex = dgMasterBillList.Columns.Count - 2;
            cancelColumn.DisplayIndex = dgMasterBillList.Columns.Count - 1;
        }
        private string GetBillItemsTotal(DataGridViewRow row)
        {
            if (row == null)
                return "";

            if (!dgMasterBillList.Columns.Contains("ItemsTotal"))
                return "";

            return Convert.ToString(
                row.Cells["ItemsTotal"].Value
            ) ?? "";
        }
        private void dgMasterBillList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                e.RowIndex >= dgMasterBillList.Rows.Count)
                return;

            DataGridViewRow row = dgMasterBillList.Rows[e.RowIndex];
            string status = Convert.ToString(row.Cells["Status"].Value) ?? "";
            string columnName = dgMasterBillList.Columns[e.ColumnIndex].Name;
            bool hasItemDetails = dgMasterBillList.Columns.Contains("ItemsTotal");
            string itemsTotal = GetBillItemsTotal(row);
            int prescriptionCount = GetPrescriptionCount(itemsTotal);
            int pharmacySaleCount = GetPharmacySaleCount(itemsTotal);
            bool hasServices = GetServiceCount(itemsTotal) > 0;

            if (columnName == "Status")
            {
                switch (status)
                {
                    case "Paid": e.CellStyle.ForeColor = Color.SpringGreen; break;
                    case "Partial": e.CellStyle.ForeColor = Color.Orange; break;
                    case "Pending": e.CellStyle.ForeColor = Color.Tomato; break;
                    case "Cancelled": e.CellStyle.ForeColor = Color.Crimson; break;
                    case "Refunded": e.CellStyle.ForeColor = Color.DodgerBlue; break;
                }
                e.CellStyle.Font = new Font(dgMasterBillList.Font, FontStyle.Bold);
            }
            else if (columnName == "btnPayAction")
            {
                // Do not infer an empty bill if ItemsTotal was not loaded.
                if (status == "Pending" && hasItemDetails && IsEmptyBill(itemsTotal))
                {
                    e.Value = "Delete";
                    e.CellStyle.BackColor = Color.FromArgb(120, 40, 40);
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (status == "Pending" || status == "Partial")
                {
                    e.Value = status == "Partial" ? "Complete" : "Pay";
                    e.CellStyle.BackColor = Color.FromArgb(14, 114, 95);
                    e.CellStyle.ForeColor = Color.White;
                }
                else
                {
                    e.Value = status == "Paid" || status == "Refunded" ? "Print" : "Locked";
                    e.CellStyle.BackColor = Color.FromArgb(28, 58, 68);
                    e.CellStyle.ForeColor = Color.FromArgb(0, 210, 190);
                }
            }
            else if (columnName == "btnEditAction")
            {
                if (status == "Pending")
                {
                    bool canEdit = hasItemDetails &&
                        (prescriptionCount > 0 || pharmacySaleCount > 0);
                    e.Value = canEdit ? "Edit" : "Unavailable";
                    e.CellStyle.BackColor = canEdit
                        ? Color.FromArgb(0, 80, 90) : Color.FromArgb(20, 43, 50);
                    e.CellStyle.ForeColor = canEdit ? Color.White : Color.DarkGray;
                }
                else if (status == "Paid" || status == "Partial")
                {
                    e.Value = "Refund";
                    e.CellStyle.BackColor = Color.FromArgb(28, 58, 68);
                    e.CellStyle.ForeColor = Color.FromArgb(0, 210, 190);
                }
                else
                {
                    e.Value = "Locked";
                    e.CellStyle.BackColor = Color.FromArgb(30, 30, 30);
                    e.CellStyle.ForeColor = Color.DarkGray;
                }
            }
            else if (columnName == "btnCancelAction")
            {
                bool canCancel = status == "Pending" && hasItemDetails && !hasServices;
                e.Value = canCancel ? "Cancel" : "Locked";
                e.CellStyle.BackColor = canCancel
                    ? Color.FromArgb(120, 40, 40) : Color.FromArgb(20, 43, 50);
                e.CellStyle.ForeColor = canCancel ? Color.White : Color.DarkGray;
            }
        }

        private void dgMasterBillList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                e.RowIndex >= dgMasterBillList.Rows.Count)
                return;

            string columnName = dgMasterBillList.Columns[e.ColumnIndex].Name;
            if (columnName != "btnPayAction" &&
                columnName != "btnEditAction" &&
                columnName != "btnCancelAction")
                return;

            DataGridViewRow row = dgMasterBillList.Rows[e.RowIndex];
            int billID = Convert.ToInt32(row.Cells["BillID"].Value);
            string currentStatus = Convert.ToString(row.Cells["Status"].Value) ?? "";
            bool hasItemDetails = dgMasterBillList.Columns.Contains("ItemsTotal");
            string itemsTotal = GetBillItemsTotal(row);
            int prescriptionCount = GetPrescriptionCount(itemsTotal);
            int pharmacySaleCount = GetPharmacySaleCount(itemsTotal);
            bool hasServices = GetServiceCount(itemsTotal) > 0;

            // PAY / COMPLETE / PRINT
            if (columnName == "btnPayAction")
            {
                if (currentStatus == "Cancelled")
                {
                    MessageBox.Show("This bill is cancelled.");
                    return;
                }

                if (currentStatus == "Paid" || currentStatus == "Refunded")
                {
                    clsFormHelper.ShowForm(() => new frmIssueInvoice(billID));
                    return;
                }

                if (currentStatus != "Pending" && currentStatus != "Partial")
                    return;

                if (hasItemDetails && IsEmptyBill(itemsTotal) && currentStatus == "Pending")
                {
                    // No delete method is currently implemented. Never show a false success.
                    MessageBox.Show("Delete bill is not implemented yet.",
                        "Action Unavailable", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                clsFormHelper.ShowForm(() => new frmProcessPayments(billID), _RefreshBillsList);
                return;
            }

            // EDIT / REFUND
            if (columnName == "btnEditAction")
            {
                if (currentStatus == "Cancelled" || currentStatus == "Refunded")
                {
                    MessageBox.Show("This bill is locked.");
                    return;
                }

                if (currentStatus == "Pending")
                {
                    if (!hasItemDetails)
                    {
                        MessageBox.Show("Bill item details are not available. Editing is disabled.",
                            "Action Unavailable", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    if (prescriptionCount == 0 && pharmacySaleCount == 0)
                    {
                        MessageBox.Show("No medicine items to edit.");
                        return;
                    }

                    using (frmEditPendingPrescription frm = new frmEditPendingPrescription(billID))
                        frm.ShowDialog();

                    _RefreshBillsList();
                    return;
                }

                if (currentStatus == "Paid" || currentStatus == "Partial")
                {
                    using (frmMedicineSalesReturn frm = new frmMedicineSalesReturn(billID))
                        frm.ShowDialog();

                    _RefreshBillsList();
                }
                return;
            }

            // CANCEL (requires actual item details)
            if (columnName == "btnCancelAction")
            {
                if (currentStatus != "Pending")
                {
                    MessageBox.Show("Only pending bills can be cancelled.");
                    return;
                }

                if (!hasItemDetails)
                {
                    MessageBox.Show(
                        "Bill item information is not available. Cancellation is disabled until item details are loaded.",
                        "Action Unavailable", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (hasServices)
                {
                    MessageBox.Show("Bills containing services cannot be cancelled.");
                    return;
                }

                if (MessageBox.Show($"Cancel bill {billID}?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                bool isCancelled = clsBillingService.CancelBill(
                    billID, clsGlobal.CurrentUser.UserID);

                MessageBox.Show(isCancelled
                    ? "The bill has been cancelled successfully."
                    : "Failed to cancel the bill.",
                    isCancelled ? "Success" : "Error", MessageBoxButtons.OK,
                    isCancelled ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                if (isCancelled)
                    _RefreshBillsList();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkedBox_CheckedChanged(object sender, EventArgs e) => _ApplyAdvancedFilter();

        private void chkAllDates_CheckedChanged(object sender, EventArgs e) => _ApplyAdvancedFilter();



    }
}