
using Clinic.ControlsMain;
using Clinic.Medical_Services.Visit;
using Clinic_Business;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Pharmaciy
{
    public partial class frmAllPrescriptions : Form
    {
        private ucPrescriptionItems _detailsPanel = new ucPrescriptionItems();
        private int _expandedRowIndex = -1;
        DataTable dtMaster = null;
        public frmAllPrescriptions()
        {
            InitializeComponent();
            //ThemeManager.ApplyTheme(this);
            dgPrescriptionDetails.Paint += (s, e) =>
            {
                if (_detailsPanel.Visible) UpdatePanelPosition();
            };
            dgPrescriptionDetails.CellFormatting += dgPrescriptionDetails_CellFormatting;
            dgPrescriptionDetails.CellClick +=
                dgPrescriptionDetails_CellContentClick; _detailsPanel.Visible = false;
            _detailsPanel.BorderStyle = BorderStyle.FixedSingle; // لتراه بوضوح أثناء الاختبار
            this.Controls.Add(_detailsPanel);

            // 3. ضبط خصائص الجدول
            dgPrescriptionDetails.AutoGenerateColumns = false;
            SetupGridDesign();
            LoadMasterData();
            cmbStatus.SelectedIndex = 0;
            lbCurrentUser.Text = clsGlobal.CurrentUser.PersonInfo.Name;
        }

        private void SetupGridDesign()
        {
            dgPrescriptionDetails.AutoGenerateColumns = false;
            dgPrescriptionDetails.Columns.Clear();

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "ExpandButton",
                    HeaderText = "",
                    Width = 45,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = Color.Teal
                    }
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "PrescriptionID",
                    HeaderText = "Prescription ID",
                    DataPropertyName = "PrescriptionID",
                    ReadOnly = true,
                    Width = 150
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "PrescriptionDate",
                    HeaderText = "Date",
                    DataPropertyName = "PrescriptionDate",
                    ReadOnly = true,
                    Width = 150
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "PatientName",
                    HeaderText = "Patient Name",
                    DataPropertyName = "PatientName",
                    ReadOnly = true,
                    Width = 180
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "DoctorName",
                    HeaderText = "Doctor Name",
                    DataPropertyName = "DoctorName",
                    ReadOnly = true,
                    Width = 180
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "VisitID",
                    HeaderText = "Visit ID",
                    DataPropertyName = "VisitID",
                    ReadOnly = true,
                    Width = 100
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Status",
                    DataPropertyName = "Status",
                    ReadOnly = true,
                    Width = 160
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Notes",
                    HeaderText = "Notes",
                    DataPropertyName = "Notes",
                    ReadOnly = true,
                    Width = 180
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewLinkColumn
                {
                    Name = "Edit",
                    HeaderText = "Edit",
                    Text = "Edit",
                    UseColumnTextForLinkValue = true,
                    TrackVisitedState = false,
                    Width = 70
                });

            dgPrescriptionDetails.Columns.Add(
                new DataGridViewLinkColumn
                {
                    Name = "Delete",
                    HeaderText = "Delete",
                    Text = "Delete",
                    UseColumnTextForLinkValue = true,
                    TrackVisitedState = false,
                    Width = 80
                });

            dgPrescriptionDetails.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
        }

        private void LoadMasterData()
        {
            try
            {
                dtMaster = clsPrescription.GetAllPrescriptionRecords();

                if (dtMaster == null)
                {
                    MessageBox.Show(
                        "GetAllPrescriptionRecords returned null.",
                        "Prescription Data"
                    );
                    return;
                }

              

                dgPrescriptionDetails.AutoGenerateColumns = false;
                dgPrescriptionDetails.DataSource = dtMaster;

                foreach (DataGridViewRow row in dgPrescriptionDetails.Rows)
                {
                    if (row.Cells["ExpandButton"] != null)
                        row.Cells["ExpandButton"].Value = "➕";
                }

                dgPrescriptionDetails.ColumnHeadersDefaultCellStyle.Font =
                    new Font("Segoe UI", 13.8F, FontStyle.Bold);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Error Loading Prescriptions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void dgPrescriptionDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // تحقق من أننا في عمود الزر وأن الخلية فارغة
            if (dgPrescriptionDetails.Columns[e.ColumnIndex].Name == "ExpandButton" && e.Value == null)
            {
                e.Value = "➕";
                e.FormattingApplied = true;
            }
        }

        private void AddContainerRow(int rowIndex)
        {
            DataTable dt = (DataTable)dgPrescriptionDetails.DataSource;
            DataRow newRow = dt.NewRow();

            // هنا نملأ الأعمدة التي لديها قيد (AllowDBNull = false) بقيم افتراضية
            newRow["PrescriptionID"] = -1; // القيمة التي نعتمدها كصف حاوية
            newRow["PatientName"] = " ";   // مسافة (تجاوز للـ Null)
            newRow["DoctorName"] = " ";    // مسافة
            newRow["PrescriptionDate"] = DateTime.Now; // تاريخ افتراضي
            newRow["Status"] = " ";        // قيمة افتراضية
            newRow["Notes"] = " ";         // قيمة افتراضية
            newRow["VisitID"] = 0;         // قيمة افتراضية

            // الآن لن يشتكي الـ DataTable من القيود
            dt.Rows.InsertAt(newRow, rowIndex + 1);
            dgPrescriptionDetails.Rows[rowIndex + 1].Height = 315;
        }
        private void dgPrescriptionDetails_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string columnName =
                dgPrescriptionDetails.Columns[e.ColumnIndex].Name;

            DataGridViewRow row =
                dgPrescriptionDetails.Rows[e.RowIndex];

            if (!int.TryParse(
                Convert.ToString(row.Cells["PrescriptionID"].Value),
                out int prescriptionID))
                return;

            if (prescriptionID <= 0)
                return;

            if (columnName == "ExpandButton")
            {
                if (_expandedRowIndex == e.RowIndex + 1)
                {
                    CloseAllExpandedRows();
                    return;
                }

                CloseAllExpandedRows();

                // Find the row again because closing an expanded row
                // may change DataGridView row positions.
                foreach (DataGridViewRow currentRow
                    in dgPrescriptionDetails.Rows)
                {
                    if (Convert.ToString(
                        currentRow.Cells["PrescriptionID"].Value)
                        == prescriptionID.ToString())
                    {
                        AddContainerRow(currentRow.Index);

                        currentRow.Cells["ExpandButton"].Value = "➖";

                        _expandedRowIndex = currentRow.Index + 1;

                        _detailsPanel.LoadDetails(prescriptionID);

                        UpdatePanelPosition();

                        _detailsPanel.Visible = true;

                        break;
                    }
                }

                return;
            }

            if (columnName == "Edit")
            {
                int visitID;

                if (!int.TryParse(
                    Convert.ToString(row.Cells["VisitID"].Value),
                    out visitID) || visitID <= 0)
                {
                    MessageBox.Show(
                        "Invalid Visit ID.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                CloseAllExpandedRows();

                using (frmDoctor frm = new frmDoctor(
                    visitID,
                    frmDoctor.enMode.Update))
                {
                    frm.ShowDialog(this);
                }

                LoadMasterData();
                return;
            }

            if (columnName == "Delete")
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to permanently delete prescription #" +
                    prescriptionID + "?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;

                try
                {
                    bool deleted =
                        clsPrescription.DeletePrescription(prescriptionID);

                    if (deleted)
                    {
                        _detailsPanel.Visible = false;
                        _expandedRowIndex = -1;

                        LoadMasterData();

                        MessageBox.Show(
                            "Prescription deleted successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Prescription not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Delete Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                return;
            }
        }
        private void UpdatePanelPosition()
        {
            if (_expandedRowIndex == -1 || _expandedRowIndex >= dgPrescriptionDetails.Rows.Count) return;

            // الحصول على إحداثيات الصف الحاوي بدقة
            Rectangle rect = dgPrescriptionDetails.GetCellDisplayRectangle(0, _expandedRowIndex, true);

            // التحقق من "خارج حدود الجدول" (هذا هو سر الاختفاء التلقائي عند التمرير)
            if (rect.Y < 0 || rect.Y > dgPrescriptionDetails.Height - 30) // الـ 30 هي مساحة الصف في الأسفل
            {
                _detailsPanel.Visible = false;
                return;
            }

            // التثبيت: تحديث الموقع فقط إذا كان هناك تغيير فعلي (لمنع الوميض)
            Point newLocation = new Point(rect.X, rect.Y);
            if (_detailsPanel.Location != newLocation)
            {
                _detailsPanel.Location = newLocation;
            }
            _detailsPanel.Parent = dgPrescriptionDetails;
            //_detailsPanel.Size = new Size(dgPrescriptionDetails.Width - 20, 314); // ارتفاع الصف الوهمي
            _detailsPanel.Visible = true;
            _detailsPanel.BringToFront();
        }
        public void CloseAllExpandedRows()
        {
            DataTable dt = (DataTable)dgPrescriptionDetails.DataSource;

            // البحث عن الصف الوهمي وحذفه من الـ DataTable
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["PrescriptionID"].ToString() == "-1")
                {
                    dt.Rows.RemoveAt(i);
                }
            }

            _detailsPanel.Visible = false;
            _expandedRowIndex = -1;

            // إعادة تعيين الأيقونات
            foreach (DataGridViewRow row in dgPrescriptionDetails.Rows)
                row.Cells["ExpandButton"].Value = "➕";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dtMaster == null || dtMaster.Columns.Count == 0)
                return;

            string selectedStatus = cmbStatus.Text.Trim();

            if (selectedStatus == "All" ||
                string.IsNullOrWhiteSpace(selectedStatus))
            {
                dtMaster.DefaultView.RowFilter = "";
                return;
            }

            string statusColumn = "";

            if (dtMaster.Columns.Contains("Status"))
                statusColumn = "Status";
            else if (dtMaster.Columns.Contains("PrescriptionStatus"))
                statusColumn = "PrescriptionStatus";
            else
                return;

            string safeStatus = selectedStatus.Replace("'", "''");

            dtMaster.DefaultView.RowFilter =
                $"[{statusColumn}] = '{safeStatus}'";
        }
        private void lbBlaceholder_Click(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lbBlaceholder.Visible =
                string.IsNullOrEmpty(txtSearch.Text);

            if (dtMaster == null ||
                !dtMaster.Columns.Contains("PatientName"))
                return;

            string search = txtSearch.Text.Trim()
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("*", "[*]");

            dtMaster.DefaultView.RowFilter =
                string.IsNullOrWhiteSpace(search)
                ? ""
                : $"[PatientName] LIKE '%{search}%'";
        }
    }
}
