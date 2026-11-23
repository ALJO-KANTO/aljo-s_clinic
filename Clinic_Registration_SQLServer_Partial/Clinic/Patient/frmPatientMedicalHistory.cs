
using Clinic_Business;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Patient
{
    public partial class frmPatientMedicalHistory : Form
    {
        private readonly int _patientID;
        private readonly bool _isArabic;

        private DataGridView dgvHistory;
        private TextBox txtVisitNotes;
        private Label lblPatient;
        private Label lblVisitCount;

        private readonly Color navy =
            Color.FromArgb(11, 61, 107);

        public frmPatientMedicalHistory(int patientID)
        {
            InitializeComponent();

            _patientID = patientID;

            string language =
                Properties.Settings.Default.ApplicationLanguage ?? "en";

            _isArabic = language.Equals(
                "ar",
                StringComparison.OrdinalIgnoreCase);

            BuildDesign();

            Load += (s, e) => LoadHistory();
        }

        private string T(string english, string arabic)
        {
            return _isArabic ? arabic : english;
        }

        private void BuildDesign()
        {
            Text = T("Patient Medical History", "السجل الطبي للمريض");

            Size = new Size(1050, 720);
            MinimumSize = new Size(800, 550);
            StartPosition = FormStartPosition.CenterParent;

            BackColor = Color.FromArgb(242, 247, 250);
            Font = new Font("Segoe UI", 10F);

            RightToLeft = _isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            RightToLeftLayout = _isArabic;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = navy
            };

            var title = new Label
            {
                Text = T(
                    "Patient Medical History",
                    "السجل الطبي للمريض"),

                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI", 20F, FontStyle.Bold),

                Dock = DockStyle.Fill,

                TextAlign = _isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft,

                Padding = new Padding(25, 0, 25, 0)
            };

            header.Controls.Add(title);
            Controls.Add(header);

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 1,
                RowCount = 4,
                RightToLeft = RightToLeft.No
            };

            body.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            body.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 65));

            body.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45));

            body.RowStyles.Add(
                new RowStyle(SizeType.Percent, 55));

            body.RowStyles.Add(
                new RowStyle(SizeType.Percent, 45));

            lblPatient = new Label
            {
                Text = T(
                    $"Patient ID: {_patientID}",
                    $"رقم المريض: {_patientID}"),

                Font = new Font(
                    "Segoe UI", 14F, FontStyle.Bold),

                Dock = DockStyle.Fill,

                TextAlign = _isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft
            };

            lblVisitCount = new Label
            {
                Text = T("Previous Visits", "الزيارات السابقة"),

                Font = new Font(
                    "Segoe UI", 12F, FontStyle.Bold),

                ForeColor = navy,
                Dock = DockStyle.Fill,

                TextAlign = _isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft
            };

            dgvHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                RightToLeft = _isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No,
                RowTemplate = { Height = 40 }
            };

            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.ColumnHeadersHeight = 42;

            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = navy;
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvHistory.ColumnHeadersDefaultCellStyle.Alignment =
                _isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvHistory.DefaultCellStyle.Alignment =
                _isArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvHistory.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 240, 245);

            dgvHistory.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dgvHistory.SelectionChanged +=
                DgvHistory_SelectionChanged;

            var notesPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 15, 0, 0)
            };

            var lblNotes = new Label
            {
                Text = T(
                    "Visit / Consultation Notes",
                    "ملاحظات الزيارة / الاستشارة"),

                Dock = DockStyle.Top,
                Height = 35,

                Font = new Font(
                    "Segoe UI", 12F, FontStyle.Bold),

                ForeColor = navy,

                TextAlign = _isArabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft
            };

            txtVisitNotes = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 11F),

                RightToLeft = _isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No,

                TextAlign = _isArabic
                    ? HorizontalAlignment.Right
                    : HorizontalAlignment.Left
            };

            notesPanel.Controls.Add(txtVisitNotes);
            notesPanel.Controls.Add(lblNotes);

            body.Controls.Add(lblPatient, 0, 0);
            body.Controls.Add(lblVisitCount, 0, 1);
            body.Controls.Add(dgvHistory, 0, 2);
            body.Controls.Add(notesPanel, 0, 3);

            Controls.Add(body);
            body.BringToFront();
        }

        private void TranslateGridColumns()
        {
            SetColumnHeader(
                "VisitID", "Visit ID", "رقم الزيارة");

            SetColumnHeader(
                "AppointmentID", "Appointment ID", "رقم الموعد");

            SetColumnHeader(
                "VisitDate", "Visit Date", "تاريخ الزيارة");

            SetColumnHeader(
                "DoctorName", "Doctor", "الطبيب");

            if (dgvHistory.Columns.Contains("VisitNotes"))
            {
                dgvHistory.Columns["VisitNotes"].Visible = false;
            }

            if (dgvHistory.Columns.Contains("VisitDate"))
            {
                dgvHistory.Columns["VisitDate"]
                    .DefaultCellStyle.Format =
                    "dd/MM/yyyy hh:mm tt";
            }
        }

        private void SetColumnHeader(
            string columnName,
            string english,
            string arabic)
        {
            if (dgvHistory.Columns.Contains(columnName))
            {
                dgvHistory.Columns[columnName].HeaderText =
                    T(english, arabic);
            }
        }

        private void LoadHistory()
        {
            try
            {
                DataTable dt =
                    clsPatient.GetPatientVisitHistory(_patientID);

                dgvHistory.DataSource = dt;

                lblVisitCount.Text = T(
                    $"Previous Visits ({dt.Rows.Count})",
                    $"الزيارات السابقة ({dt.Rows.Count})");

                TranslateGridColumns();

                if (dt.Rows.Count > 0)
                {
                    dgvHistory.ClearSelection();

                    dgvHistory.CurrentCell =
                        dgvHistory.Rows[0].Cells[0];

                    dgvHistory.Rows[0].Selected = true;

                    ShowSelectedNotes();
                }
                else
                {
                    txtVisitNotes.Text = T(
                        "No previous visits found.",
                        "لا توجد زيارات سابقة للمريض.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(
                        "Unable to load medical history:",
                        "تعذر تحميل السجل الطبي:")
                    + Environment.NewLine + ex.Message,

                    T("Medical History", "السجل الطبي"),

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DgvHistory_SelectionChanged(
            object sender, EventArgs e)
        {
            ShowSelectedNotes();
        }

        private void ShowSelectedNotes()
        {
            if (dgvHistory.CurrentRow == null)
                return;

            DataRowView row =
                dgvHistory.CurrentRow.DataBoundItem as DataRowView;

            if (row == null)
                return;

            string notes =
                Convert.ToString(row["VisitNotes"]);

            txtVisitNotes.Text =
                string.IsNullOrWhiteSpace(notes)
                    ? T(
                        "No consultation notes recorded.",
                        "لم يتم تسجيل ملاحظات للاستشارة.")
                    : notes;
        }
    }
}
