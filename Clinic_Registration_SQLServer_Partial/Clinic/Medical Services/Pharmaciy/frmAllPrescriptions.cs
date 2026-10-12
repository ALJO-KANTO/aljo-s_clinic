
using Clinic.ControlsMain;
using Clinic_Business;
using System;
using System.Data;
using System.Drawing;
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
            dgPrescriptionDetails.CellContentClick += dgPrescriptionDetails_CellContentClick;
            _detailsPanel.Visible = false;
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

            // 1. زر التوسيع الملون بالأكوا
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ExpandButton",
                HeaderText = "",
                Width = 45,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 210, 190),
                    SelectionForeColor = Color.FromArgb(0, 210, 190)
                }
            });
            // 2. ربط الأعمدة بأسماء الحقول (DataPropertyName) لكي تظهر النصوص داخل الخلايا
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrescriptionID", HeaderText = "Prescription ID", DataPropertyName = "PrescriptionID", ReadOnly = true, Width = 180 });
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrescriptionDate", HeaderText = "Date", DataPropertyName = "PrescriptionDate", ReadOnly = true, Width = 150 });
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "PatientName", HeaderText = "Patient Name", DataPropertyName = "PatientName", ReadOnly = true, Width = 200 });
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "DoctorName", HeaderText = "Doctor Name", DataPropertyName = "DoctorName", ReadOnly = true, Width = 200 });

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "VisitID", HeaderText = "Visit ID", DataPropertyName = "VisitID", ReadOnly = true, Width = 120 });

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", DataPropertyName = "Status", ReadOnly = true, Width = 150 });
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "Notes", HeaderText = "Notes", DataPropertyName = "Notes", ReadOnly = true, Width = 200 });
            DataGridViewLinkColumn actionColumn = new DataGridViewLinkColumn
            {
                Name = "Actions",
                HeaderText = "Actions",
                Text = "Edit  |  Delete",
                UseColumnTextForLinkValue = true, // لعرض النص الثابت على كل الأسطر
                ActiveLinkColor = Color.Blue,
                LinkColor = Color.Brown, // لون مميز ومناسب للهوية البصرية للعيادة
                TrackVisitedState = false,
                Width = 148 // مساحة ثابتة وممتازة للكلمتين معاً
            };
            // جعل النص يتوسط الخلية تماماً بشكل أنيق
            dgPrescriptionDetails.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgPrescriptionDetails.Columns.Add(actionColumn);
            //dgPrescriptionDetails.DataSource = dtMaster;

        }

        private void LoadMasterData()
        {
            dtMaster = clsPrescription.GetAllPrescriptionRecords();

            if (dtMaster == null || dtMaster.Rows.Count == 0) return;

            // هام جداً: إبقاء AutoGenerateColumns = false
            dgPrescriptionDetails.AutoGenerateColumns = false;
            dgPrescriptionDetails.DataSource = dtMaster;

            // بما أننا قمنا بإنشاء الأعمدة يدوياً، سيتم ربط الـ DataPropertyName تلقائياً
            // يبقى فقط تعبئة الـ ExpandButton يدوياً بعد التحميل
            foreach (DataGridViewRow row in dgPrescriptionDetails.Rows)
            {
                row.Cells["ExpandButton"].Value = "➕";
            }

            dgPrescriptionDetails.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold);
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
        private void dgPrescriptionDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgPrescriptionDetails.Columns[e.ColumnIndex].Name != "ExpandButton") return;

            var row = dgPrescriptionDetails.Rows[e.RowIndex];
            int prescriptionID = Convert.ToInt32(row.Cells["PrescriptionID"].Value);

            if (row.Cells["ExpandButton"].Value?.ToString() == "➕")
            {
                CloseAllExpandedRows();

                // إدراج الصف عبر الدالة التي تملأ البيانات الافتراضية
                AddContainerRow(e.RowIndex);

                row.Cells["ExpandButton"].Value = "➖";
                _expandedRowIndex = e.RowIndex + 1;

                _detailsPanel.LoadDetails(prescriptionID);
                UpdatePanelPosition();
                _detailsPanel.Visible = true;
            }
            else
            {
                CloseAllExpandedRows();
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
            string selectedStatus = cmbStatus.Text;

            // 2. إذا كانت "All" نعرض كل شيء (نفرغ الفلتر)
            if (selectedStatus == "All" || string.IsNullOrEmpty(selectedStatus))
            {
                dtMaster.DefaultView.RowFilter = "";
            }
            else
            {
                // 3. فلترة الجدول حسب الحالة
                dtMaster.DefaultView.RowFilter = $"Status = '{selectedStatus}'";
            }
        }

        private void lbBlaceholder_Click(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            lbBlaceholder.Visible = string.IsNullOrEmpty(txtSearch.Text);
            string filter = string.IsNullOrEmpty(txtSearch.Text) ? "" : $"PatientName LIKE '%{txtSearch.Text}%'";
            dtMaster.DefaultView.RowFilter = filter;



        }
    }
}
