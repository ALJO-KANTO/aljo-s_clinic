using Clinic.ControlsMain;
using Clinic.Medical_Services.Casher;
using Clinic_DataAccess;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;


namespace Clinic.Medical_Services.Laboratory
{
    public partial class frmLaboratory : Form
    {
        private DataTable _individualRequests;

        public frmLaboratory()
        {
            InitializeComponent();

            dgvRequests.CellContentClick += dgvRequests_CellContentClick;
            dgvRequests.CellFormatting += dgvRequests_CellFormatting;
            ApplyArabicProperties();

            // Keep Arabic heading centered when resized.
            header.Resize += Header_Resize;
        }
        private void dgvRequests_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                dgvRequests.Columns[e.ColumnIndex].Name != "PrintLabReport") return;
            DataGridViewRow row = dgvRequests.Rows[e.RowIndex];
            bool complete = Convert.ToString(row.Cells["Status"].Value) == "Completed";
            e.Value = complete ? T("Print") : "—";
            e.FormattingApplied = true;
        }

        private void dgvRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                dgvRequests.Columns[e.ColumnIndex].Name != "PrintLabReport") return;
            try
            {
                DataRow[] requests = GetGroupRequests(dgvRequests.Rows[e.RowIndex]);
                DataRow selected = ChooseRequest(requests, true);
                if (selected == null) return;
                int id = Convert.ToInt32(selected["LabRequestID"]);
                if (!string.Equals(Convert.ToString(selected["Status"]), "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(T("The laboratory result must be completed before printing."));
                    return;
                }
                if (!ClinicDatabase.IsLabRequestPaid(id))
                {
                    MessageBox.Show(T("The laboratory bill must be fully paid before printing."));
                    return;
                }
                using (var form = new frmLabReportPrint(id)) form.ShowDialog(this);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, T("Print Lab Report")); }
        }

        private string T(string text)
        {
            return TranslationClass.Translate(
                text,
                TranslationClass.IsArabic
            );
        }

        public void ApplyArabicProperties()
        {
            bool arabic = TranslationClass.IsArabic;

            this.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            this.RightToLeftLayout = arabic;

            ApplyTranslationToControls(this);

            if (arabic)
            {
                title.Dock = DockStyle.None;
                title.AutoSize = false;
                title.Left = 0;
                title.Width = header.ClientSize.Width;
                title.Height = 44;
                title.TextAlign = ContentAlignment.MiddleCenter;

                title.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right;

                subtitle.Dock = DockStyle.None;
                subtitle.AutoSize = false;
                subtitle.Left = 0;
                subtitle.Width = header.ClientSize.Width;
                subtitle.Height = 24;
                subtitle.TextAlign = ContentAlignment.MiddleCenter;

                subtitle.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right;

                title.Top = 14;
                subtitle.Top = title.Bottom;
            }
            else
            {
                title.Dock = DockStyle.Top;
                title.AutoSize = false;
                title.Height = 44;
                title.TextAlign = ContentAlignment.MiddleLeft;

                subtitle.Dock = DockStyle.Top;
                subtitle.AutoSize = false;
                subtitle.Height = 24;
                subtitle.TextAlign = ContentAlignment.MiddleLeft;
            }

            if (dgvRequests.Columns.Count > 0)
                TranslateGridHeaders();
        }

        private void Header_Resize(object sender, EventArgs e)
        {
            if (!TranslationClass.IsArabic)
                return;

            title.Left = 0;
            title.Width = header.ClientSize.Width;
            title.TextAlign = ContentAlignment.MiddleCenter;

            subtitle.Left = 0;
            subtitle.Width = header.ClientSize.Width;
            subtitle.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void ApplyTranslationToControls(Control parent)
        {
            bool arabic = TranslationClass.IsArabic;

            foreach (Control control in parent.Controls)
            {
                if (!string.IsNullOrWhiteSpace(control.Text))
                    control.Text = T(control.Text);

                control.RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control is Label label)
                {
                    label.TextAlign = arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;

                    string name = label.Name.ToLowerInvariant();

                    if (arabic &&
                        (name.Contains("title") ||
                         name.Contains("heading")))
                    {
                        label.TextAlign = ContentAlignment.MiddleCenter;
                    }
                }

                if (control is Button button)
                {
                    button.TextAlign = ContentAlignment.MiddleCenter;
                }

                if (control.HasChildren)
                    ApplyTranslationToControls(control);
            }

            dgvRequests.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            dgvRequests.ColumnHeadersDefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleCenter
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvRequests.DefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
        }

        private void TranslateGridHeaders()
        {
            foreach (DataGridViewColumn column in dgvRequests.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                    column.HeaderText = T(column.HeaderText);
            }
        }

        private void frmLaboratory_Load(object sender, EventArgs e)
        {
            try
            {
                ClinicDatabase.EnsureLaboratoryTables();

                ClinicDatabase.BackfillLabRequestPrices();
                ReloadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(ex.Message),
                    T("Laboratory setup"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void FiltersChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
                ReloadData();
        }

        private void ReloadData()
        {
            try
            {
                DataTable stats =
                    ClinicDatabase.GetLabDashboardStats();

                if (stats.Rows.Count > 0)
                {
                    DataRow r = stats.Rows[0];

                    string[] fields =
                    {
                        "TotalTests",
                        "TotalRequests",
                        "Pending",
                        "InProgress",
                        "Completed"
                    };

                    for (int i = 0; i < fields.Length; i++)
                    {
                        statValues[i].Text =
                            r[fields[i]].ToString();
                    }
                }

                _individualRequests = ClinicDatabase.GetLabRequests(txtSearch.Text, cmbStatus.Text);
                dgvRequests.DataSource = BuildGroupedRequests(_individualRequests);
                if (dgvRequests.Columns.Contains("GroupKey"))
                    dgvRequests.Columns["GroupKey"].Visible = false;

                // Add Print button column after loading the requests.
                if (!dgvRequests.Columns.Contains("PrintLabReport"))
                {
                    DataGridViewButtonColumn printColumn =
                        new DataGridViewButtonColumn();

                    printColumn.Name = "PrintLabReport";
                    printColumn.HeaderText = T("Print Report");
                    printColumn.Text = T("Print");
                    printColumn.UseColumnTextForButtonValue = true;
                    printColumn.Width = 115;
                    printColumn.MinimumWidth = 105;
                    printColumn.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.None;

                    printColumn.FlatStyle = FlatStyle.Flat;
                    printColumn.DefaultCellStyle.BackColor =
                        Color.FromArgb(0, 121, 107);
                    printColumn.DefaultCellStyle.ForeColor =
                        Color.White;

                    dgvRequests.Columns.Add(printColumn);
                }

                dgvRequests.Columns["PrintLabReport"].DisplayIndex =
                    dgvRequests.Columns.Count - 1;

                if (dgvRequests.Columns.Contains("ResultNotes"))
                    dgvRequests.Columns["ResultNotes"].Visible = false;

                if (dgvRequests.Columns.Contains("ResultValue"))
                    dgvRequests.Columns["ResultValue"].Visible = false;

                if (dgvRequests.Columns.Contains("ResultedAt"))
                    dgvRequests.Columns["ResultedAt"].Visible = false;

                if (dgvRequests.Columns.Contains("RequestedAt"))
                {
                    dgvRequests.Columns["RequestedAt"]
                        .DefaultCellStyle.Format =
                            "dd MMM yyyy HH:mm";
                }

                TranslateGridHeaders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(ex.Message),
                    T("Load laboratory"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Group new requests by LabOrderID, legacy requests by their own request ID.
        // Never merge separate orders belonging to the same visit.
        private static string RequestGroupKey(DataRow r)
        {
            if (r.Table.Columns.Contains("LabOrderID") && r["LabOrderID"] != DBNull.Value)
                return "O:" + Convert.ToString(r["LabOrderID"]);
            return "R:" + Convert.ToString(r["LabRequestID"]);
        }

        private DataTable BuildGroupedRequests(DataTable source)
        {
            DataTable grouped = new DataTable();
            grouped.Columns.Add("GroupKey", typeof(string));
            grouped.Columns.Add("LabOrderID", typeof(int));
            grouped.Columns.Add("LabRequestID", typeof(int));
            grouped.Columns.Add("VisitID", typeof(int));
            grouped.Columns.Add("BillID", typeof(string));
            grouped.Columns.Add("PatientName", typeof(string));
            grouped.Columns.Add("TestName", typeof(string));
            grouped.Columns.Add("SavedTestPrice", typeof(decimal));
            grouped.Columns.Add("PaidAmount", typeof(decimal));
            grouped.Columns.Add("BalanceDue", typeof(decimal));
            grouped.Columns.Add("RequestedAt", typeof(DateTime));
            grouped.Columns.Add("Status", typeof(string));
            foreach (var group in source.AsEnumerable().GroupBy(RequestGroupKey))
            {
                var rows = group.ToList();
                DataRow first = rows[0];
                var billGroups = rows.Where(r => r["BillID"] != DBNull.Value)
                    .GroupBy(r => Convert.ToInt32(r["BillID"])).ToList();
                decimal total = rows.Sum(r => SafeDecimal(r, "SavedTestPrice"));
                // PaidAmount is repeated per bill; count each bill only once.
                decimal paid = billGroups.Sum(g => g.Max(r => SafeDecimal(r, "PaidAmount")));
                bool allCompleted = rows.All(r => Convert.ToString(r["Status"]) == "Completed");
                string status = allCompleted ? "Completed" :
                    rows.Any(r => Convert.ToString(r["Status"]) == "In Progress") ? "In Progress" :
                    rows.Any(r => Convert.ToString(r["Status"]) == "Partial Payment") ? "Partial Payment" :
                    rows.All(r => Convert.ToString(r["Status"]) == "Paid") ? "Paid" : "Ordered";
                DataRow display = grouped.NewRow();
                display["GroupKey"] = group.Key;
                display["LabOrderID"] = first["LabOrderID"];
                display["LabRequestID"] = first["LabRequestID"];
                display["VisitID"] = first["VisitID"];
                display["BillID"] = string.Join(", ", billGroups.Select(g => g.Key.ToString()));
                display["PatientName"] = Convert.ToString(first["PatientName"]);
                display["TestName"] = rows.Count == 1 ? Convert.ToString(first["TestName"]) : rows.Count + " Tests";
                display["SavedTestPrice"] = total;
                display["PaidAmount"] = paid;
                display["BalanceDue"] = Math.Max(0m, total - paid);
                display["RequestedAt"] = rows.Max(r => Convert.ToDateTime(r["RequestedAt"]));
                display["Status"] = status;
                grouped.Rows.Add(display);
            }
            return grouped;
        }

        private decimal SafeDecimal(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return 0m;
            return Convert.ToDecimal(row[column]);
        }

        private DataRow[] GetGroupRequests(DataGridViewRow gridRow)
        {
            if (_individualRequests == null || gridRow == null) return new DataRow[0];
            string key = Convert.ToString(gridRow.Cells["GroupKey"].Value);
            return _individualRequests.AsEnumerable()
                .Where(r => RequestGroupKey(r) == key).ToArray();
        }

        private DataRow ChooseRequest(DataRow[] rows, bool completedOnly = false)
        {
            if (rows.Length == 0) return null;
            var eligible = completedOnly
                ? rows.Where(r => string.Equals(Convert.ToString(r["Status"]), "Completed", StringComparison.OrdinalIgnoreCase)).ToArray()
                : rows;
            if (eligible.Length == 0)
            {
                MessageBox.Show(T("No completed laboratory tests in this group."));
                return null;
            }
            if (eligible.Length == 1) return eligible[0];
            using (var dialog = new Form())
            {
                dialog.Text = T("Select Laboratory Test");
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.Size = new Size(680, 400);
                dialog.RightToLeft = TranslationClass.IsArabic ? RightToLeft.Yes : RightToLeft.No;
                dialog.RightToLeftLayout = TranslationClass.IsArabic;
                var table = new DataTable();
                table.Columns.Add("LabRequestID", typeof(int));
                table.Columns.Add("TestName", typeof(string));
                table.Columns.Add("Status", typeof(string));
                foreach (DataRow r in eligible)
                    table.Rows.Add(Convert.ToInt32(r["LabRequestID"]), Convert.ToString(r["TestName"]), Convert.ToString(r["Status"]));
                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = table,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    RowHeadersVisible = false
                };
                var button = new Button { Text = T("Select"), Dock = DockStyle.Bottom, Height = 44 };
                Action choose = () => { if (grid.CurrentRow != null) dialog.DialogResult = DialogResult.OK; };
                button.Click += (sender, e) => choose();
                grid.CellDoubleClick += (sender, e) => { if (e.RowIndex >= 0) choose(); };
                dialog.Controls.Add(grid);
                dialog.Controls.Add(button);
                if (dialog.ShowDialog(this) != DialogResult.OK || grid.CurrentRow == null) return null;
                int id = Convert.ToInt32(grid.CurrentRow.Cells["LabRequestID"].Value);
                return eligible.First(r => Convert.ToInt32(r["LabRequestID"]) == id);
            }
        }

        private void btnNewRequest_Click(object sender, EventArgs e)
        {
            using (var form = new frmLabRequest())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    ReloadData();
            }
        }

        private void btnManageTests_Click(object sender, EventArgs e)
        {
            using (var form = new frmLabTests())
            {
                form.ShowDialog(this);
                ReloadData();
            }
        }

        // =====================================================
        // NEW: SEND LAB REQUEST TO BILLING
        // =====================================================
        private void btnSendToBilling_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null)
            {
                MessageBox.Show(T("Select a lab request first."));
                return;
            }

            try
            {
                DataRow[] rows = GetGroupRequests(dgvRequests.CurrentRow);
                if (rows.Length == 0) return;
                if (rows[0]["LabOrderID"] != DBNull.Value)
                {
                    int orderID = Convert.ToInt32(rows[0]["LabOrderID"]);
                    int orderBillID = ClinicDatabase.GetOrCreateLabOrderBill(orderID);
                    using (var form = new frmManageBills(orderBillID))
                        form.ShowDialog(this);
                    ReloadData();
                    return;
                }


                // A grouped visit may contain several EXISTING invoices.
                // Let the user choose the actual invoice; never combine
                // previously billed/paid invoices simply for display.
                var billGroups = rows
                    .Where(r => r["BillID"] != DBNull.Value)
                    .GroupBy(r => Convert.ToInt32(r["BillID"]))
                    .OrderBy(g => g.Key)
                    .ToList();

                int billID = -1;
                if (billGroups.Count == 1 &&
                    rows.All(r => r["BillID"] != DBNull.Value))
                {
                    billID = billGroups[0].Key;
                }
                else
                {
                    using (var dialog = new Form())
                    {
                        dialog.Text = T("Select Laboratory Bill");
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        dialog.Size = new Size(700, 420);
                        dialog.MinimumSize = new Size(560, 320);
                        dialog.RightToLeft = TranslationClass.IsArabic
                            ? RightToLeft.Yes : RightToLeft.No;
                        dialog.RightToLeftLayout = TranslationClass.IsArabic;

                        DataTable choices = new DataTable();
                        choices.Columns.Add("BillID", typeof(int));
                        choices.Columns.Add("Tests", typeof(string));
                        choices.Columns.Add("Amount", typeof(decimal));
                        choices.Columns.Add("RequestID", typeof(int));

                        foreach (var g in billGroups)
                        {
                            choices.Rows.Add(g.Key,
                                g.Count() + " Tests (existing bill)",
                                g.Sum(r => SafeDecimal(r, "SavedTestPrice")),
                                Convert.ToInt32(g.First()["LabRequestID"]));
                        }

                        // Unbilled requests have no invoice yet. The existing
                        // database method bills ONE request; do not imply it
                        // creates a combined invoice for the entire visit.
                        foreach (DataRow r in rows.Where(r => r["BillID"] == DBNull.Value))
                        {
                            choices.Rows.Add(-1,
                                Convert.ToString(r["TestName"]) + " (unbilled)",
                                SafeDecimal(r, "SavedTestPrice"),
                                Convert.ToInt32(r["LabRequestID"]));
                        }

                        // Create columns explicitly before binding. When a grid is
                        // not parented yet, auto-generated columns may not exist.
                        var grid = new DataGridView
                        {
                            Dock = DockStyle.Fill,
                            ReadOnly = true,
                            AllowUserToAddRows = false,
                            AllowUserToDeleteRows = false,
                            RowHeadersVisible = false,
                            MultiSelect = false,
                            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                            AutoGenerateColumns = false
                        };

                        grid.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            Name = "BillID",
                            DataPropertyName = "BillID",
                            HeaderText = T("Bill ID (-1 = unbilled)")
                        });
                        grid.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            Name = "Tests",
                            DataPropertyName = "Tests",
                            HeaderText = T("Tests")
                        });
                        grid.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            Name = "Amount",
                            DataPropertyName = "Amount",
                            HeaderText = T("Amount"),
                            DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
                        });
                        grid.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            Name = "RequestID",
                            DataPropertyName = "RequestID",
                            Visible = false
                        });
                        grid.DataSource = choices;

                        var select = new Button
                        {
                            Text = T("Open / Create Selected Bill"),
                            Dock = DockStyle.Bottom,
                            Height = 46
                        };
                        select.Click += (s, args) =>
                        {
                            if (grid.CurrentRow != null)
                                dialog.DialogResult = DialogResult.OK;
                        };
                        grid.CellDoubleClick += (s, args) =>
                        {
                            if (args.RowIndex >= 0)
                                dialog.DialogResult = DialogResult.OK;
                        };
                        dialog.Controls.Add(grid);
                        dialog.Controls.Add(select);

                        if (dialog.ShowDialog(this) != DialogResult.OK ||
                            grid.CurrentRow == null)
                            return;

                        int chosenBillID = Convert.ToInt32(
                            grid.CurrentRow.Cells["BillID"].Value);
                        if (chosenBillID > 0)
                            billID = chosenBillID;
                        else
                            billID = ClinicDatabase.GetOrCreateLabRequestBill(
                                Convert.ToInt32(grid.CurrentRow.Cells["RequestID"].Value));
                    }
                }

                using (var form = new frmManageBills(billID))
                    form.ShowDialog(this);

                ReloadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, T("Laboratory Billing"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // =====================================================
        // UPDATED: ENTER RESULT ONLY AFTER PAYMENT
        // =====================================================
        private void btnEnterResult_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null) { MessageBox.Show(T("Select a lab request first.")); return; }
            try
            {
                DataRow selected = ChooseRequest(GetGroupRequests(dgvRequests.CurrentRow));
                if (selected == null) return;
                int id = Convert.ToInt32(selected["LabRequestID"]);
                string status = Convert.ToString(selected["Status"]);
                if (status == "Completed" || status == "Cancelled")
                {
                    MessageBox.Show(T("Completed or cancelled requests cannot be overwritten."));
                    return;
                }
                if (!ClinicDatabase.IsLabRequestPaid(id))
                {
                    MessageBox.Show(T("Please complete laboratory payment before entering results."),
                        T("Payment Required"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (var form = new frmLabResult(id))
                    if (form.ShowDialog(this) == DialogResult.OK) ReloadData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, T("Laboratory")); }
        }
    }
}
