using Clinic_Business;
using System;
using System.Data;
using System.Drawing;
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

            txtSearchByPatientName.TextChanged += txtSearchByPatientName_TextChanged;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

            chePending.CheckedChanged += FilterStatus_CheckedChanged;
            chePartial.CheckedChanged += FilterStatus_CheckedChanged;
            chePaid.CheckedChanged += FilterStatus_CheckedChanged;

            dgMasterBillList.CellFormatting += dgMasterBillList_CellFormatting;
            dgMasterBillList.CellContentClick += dgMasterBillList_CellContentClick;
        }

        private void frmManageBills_Load(object sender, EventArgs e)
        {
            lbCurrentUser.Text = clsGlobal.PersonName;

            chePending.Checked = true;
            chePartial.Checked = true;
            chePaid.Checked = true;

            _RefreshBillsList();
        }

        private void _RefreshBillsList()
        {
            _dtBills = clsBillingService.GetAllBills();
            _dvBills = _dtBills.DefaultView;
            dgMasterBillList.DataSource = _dvBills;

            // تطبيق الفلترة الافتراضية وتحديث العدادات
            _ApplyAdvancedFilter();
            _UpdateKPICards(_dtBills);
            _AddActionButtonsOnce();
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
            // 1. التحقق من أن جدول البيانات يحتوي على أسطر فعلياً لتجنب الأخطاء
            if (dt == null || dt.Rows.Count == 0)
            {
                lblCountPending.Text = "0";
                lblCountPartial.Text = "0";
                lblCountPaid.Text = "0";
                return;
            }

            // 2. جلب التاريخ المختار من الـ DateTimePicker
            string targetDateStr = dateTimePicker1.Value.ToString("dd/MM/yyyy");

            // 3. فحص هل الكاشير اختار "عرض كل التواريخ" chkAllDates
            CheckBox chkAllDates = (CheckBox)this.Controls["chkAllDates"];

            int pendingCount, partialCount, paidCount;

            if (chkAllDates != null && chkAllDates.Checked)
            {
                // 🔓 إذا كان يعرض كل التواريخ: نحسب الإجمالي الشامل لجميع الفواتير في النظام
                pendingCount = dt.Select("Status = 'Pending'").Length;
                partialCount = dt.Select("Status = 'Partial'").Length;
                paidCount = dt.Select("Status = 'Paid'").Length;
            }
            else
            {
                // 🔒 إذا كان يفلتر بتاريخ معين: نحسب الإحصائيات لهذا التاريخ المختار فقط (تعديلك الممتاز)
                pendingCount = dt.Select($"Status = 'Pending' AND BillDateString = '{targetDateStr}'").Length;
                partialCount = dt.Select($"Status = 'Partial' AND BillDateString = '{targetDateStr}'").Length;
                paidCount = dt.Select($"Status = 'Paid' AND BillDateString = '{targetDateStr}'").Length;
            }

            // 4. تحديث الواجهة الرقمية المضيئة
            lblCountPending.Text = pendingCount.ToString();
            lblCountPartial.Text = partialCount.ToString();
            lblCountPaid.Text = paidCount.ToString();
        }

        private void _ApplyAdvancedFilter()
        {
            string nameTarget = txtSearchByPatientName.Text.Replace("'", "''").Trim();

            // 1. بناء فلتر الحالات
            string statusFilter = "";
            if (chePending.Checked) statusFilter += "'Pending',";
            if (chePartial.Checked) statusFilter += "'Partial',";
            if (chePaid.Checked) statusFilter += "'Paid',";

            if (statusFilter.EndsWith(","))
                statusFilter = statusFilter.TrimEnd(',');

            // 2. فحص خيار منع فلترة التاريخ (عرض كل التواريخ)
            string finalFilter = "";

            // ابحث عن الشيك بوكس الذي أضفناه
            CheckBox chkAllDates = (CheckBox)this.Controls["chkAllDates"];

            if (chkAllDates != null && chkAllDates.Checked)
            {
                // 🔓 إذا كان مؤشراً عليه: نضع شرطاً وهمياً دائماً صحيح ليتم تجاهل التاريخ وعرض الكل
                finalFilter = "1=1";
            }
            else
            {
                // 🔒 إذا لم يكن مؤشراً عليه: نفلتر بالتاريخ المختار كالمعتاد
                string dateTarget = dateTimePicker1.Value.ToString("dd/MM/yyyy");
                finalFilter = $"BillDateString = '{dateTarget}'";
            }

            // 3. دمج فلتر الحالات
            if (!string.IsNullOrEmpty(statusFilter))
            {
                finalFilter += $" AND Status IN ({statusFilter})";
            }
            else
            {
                finalFilter += " AND 1=1"; ;
            }

            // 4. دمج فلتر اسم المريض
            if (!string.IsNullOrEmpty(nameTarget))
            {
                finalFilter += $" AND PatientName LIKE '%{nameTarget}%'";
            }

            // 5. تطبيق التصفية النهائية
            _dvBills.RowFilter = finalFilter;
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

        private void dgMasterBillList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgMasterBillList.Rows.Count)
                return;

            string status = dgMasterBillList.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
            string itemsTotal = dgMasterBillList.Rows[e.RowIndex].Cells["ItemsTotal"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(status))
                return;

            int prescriptionCount = GetPrescriptionCount(itemsTotal);
            int pharmacySaleCount = GetPharmacySaleCount(itemsTotal);
            bool hasServices = GetServiceCount(itemsTotal) > 0;

            // ===== STATUS =====
            if (dgMasterBillList.Columns[e.ColumnIndex].Name == "Status")
            {
                switch (status)
                {
                    case "Paid":
                        e.CellStyle.ForeColor = Color.SpringGreen;
                        break;

                    case "Partial":
                        e.CellStyle.ForeColor = Color.Orange;
                        break;

                    case "Pending":
                        e.CellStyle.ForeColor = Color.Tomato;
                        break;

                    case "Cancelled":
                        e.CellStyle.ForeColor = Color.Crimson;
                        break;

                    case "Refunded":
                        e.CellStyle.ForeColor = Color.DodgerBlue;
                        break;
                }

                e.CellStyle.Font = new Font(dgMasterBillList.Font, FontStyle.Bold);
            }

            // ===== PAY BUTTON =====
            if (dgMasterBillList.Columns[e.ColumnIndex].Name == "btnPayAction")
            {
                if (IsEmptyBill(itemsTotal) && status == "Pending")
                {
                    e.Value = "🗑 Delete";
                    e.CellStyle.BackColor = Color.FromArgb(120, 40, 40);
                    e.CellStyle.ForeColor = Color.White;
                    return;
                }

                if (status == "Pending")
                {
                    e.Value = "💳 Pay";
                    e.CellStyle.BackColor = Color.FromArgb(14, 114, 95);
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (status == "Partial")
                {
                    e.Value = "🔸 Complete";
                    e.CellStyle.BackColor = Color.FromArgb(14, 114, 95);
                    e.CellStyle.ForeColor = Color.White;
                }
                else
                {
                    e.Value = "🖨 Print";
                    e.CellStyle.BackColor = Color.FromArgb(28, 58, 68);
                    e.CellStyle.ForeColor = Color.FromArgb(0, 210, 190);
                }
            }

            // ===== EDIT BUTTON =====
            if (dgMasterBillList.Columns[e.ColumnIndex].Name == "btnEditAction")
            {
                if (status == "Pending")
                {
                    if (prescriptionCount > 0 || pharmacySaleCount > 0)
                    {
                        e.Value = "📝 Edit";
                        e.CellStyle.BackColor = Color.FromArgb(0, 80, 90);
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        e.Value = "🚫 No Medicines";
                        e.CellStyle.BackColor = Color.FromArgb(20, 43, 50);
                        e.CellStyle.ForeColor = Color.DarkGray;
                    }
                }
                else if (status == "Paid" || status == "Partial")
                {
                    e.Value = "🔄 Refund";
                    e.CellStyle.BackColor = Color.FromArgb(28, 58, 68);
                    e.CellStyle.ForeColor = Color.FromArgb(0, 210, 190);
                }
                else
                {
                    e.Value = "👁 View";
                    e.CellStyle.BackColor = Color.FromArgb(30, 30, 30);
                    e.CellStyle.ForeColor = Color.DarkGray;
                }
            }

            // ===== CANCEL BUTTON =====
            if (dgMasterBillList.Columns[e.ColumnIndex].Name == "btnCancelAction")
            {
                if (status == "Pending" && !hasServices)
                {
                    e.Value = "❌ Cancel";
                    e.CellStyle.BackColor = Color.FromArgb(120, 40, 40);
                    e.CellStyle.ForeColor = Color.White;
                }
                else
                {
                    e.Value = "🔒 Locked";
                    e.CellStyle.BackColor = Color.FromArgb(20, 43, 50);
                    e.CellStyle.ForeColor = Color.DarkGray;
                }
            }
        }

        private void dgMasterBillList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int billID = Convert.ToInt32(dgMasterBillList.Rows[e.RowIndex].Cells["BillID"].Value);

            string currentStatus = dgMasterBillList.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
            string itemsTotal = dgMasterBillList.Rows[e.RowIndex].Cells["ItemsTotal"].Value?.ToString();

            int prescriptionCount = GetPrescriptionCount(itemsTotal);
            int pharmacySaleCount = GetPharmacySaleCount(itemsTotal);
            bool hasServices = GetServiceCount(itemsTotal) > 0;

            // ===== PAY / DELETE =====
            if (dgMasterBillList.Columns[e.ColumnIndex].Name == "btnPayAction")
            {
                if (IsEmptyBill(itemsTotal) && currentStatus == "Pending")
                {
                    if (MessageBox.Show(
                        "This bill is empty. Do you want to delete it?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //_DeleteBill(billID);
                        _RefreshBillsList();
                    }

                    return;
                }

                if (currentStatus == "Paid" || currentStatus == "Refunded")
                {
                    clsFormHelper.ShowForm(() => new frmIssueInvoice(billID));
                    return;
                }

                if (currentStatus == "Cancelled")
                {
                    MessageBox.Show("This bill is cancelled.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                clsFormHelper.ShowForm(() => new frmProcessPayments(billID), _RefreshBillsList);
                return;
            }

            // ===== EDIT =====
            if (dgMasterBillList.Columns[e.ColumnIndex].Name == "btnEditAction")
            {
                if (currentStatus == "Cancelled" || currentStatus == "Refunded")
                {
                    MessageBox.Show("This bill is locked.",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                if (currentStatus == "Pending")
                {
                    if (prescriptionCount == 0 && pharmacySaleCount == 0)
                    {
                        MessageBox.Show("No items to edit.",
                            "Info",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    using (frmEditPendingPrescription frm = new frmEditPendingPrescription(billID))
                    {
                        frm.ShowDialog();
                    }

                    _RefreshBillsList();
                    return;
                }

                if (currentStatus == "Paid" || currentStatus == "Partial")
                {
                    using (frmMedicineSalesReturn frm = new frmMedicineSalesReturn(billID))
                    {
                        frm.ShowDialog();
                    }

                    _RefreshBillsList();
                }

                return;
            }

            // ===== CANCEL =====
            if (dgMasterBillList.Columns[e.ColumnIndex].Name == "btnCancelAction")
            {
                if (currentStatus != "Pending")
                {
                    MessageBox.Show("Only pending bills can be cancelled.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (hasServices)
                {
                    MessageBox.Show("Bills containing services cannot be cancelled.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"Cancel bill {billID}?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    bool isCancelled = clsBillingService.CancelBill(billID,clsGlobal.CurrentUser.UserID);

                    if (isCancelled)
                    {
                        MessageBox.Show(
                            "The bill has been cancelled successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        _RefreshBillsList();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to cancel the bill. Please try again or contact support.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }

                return;
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