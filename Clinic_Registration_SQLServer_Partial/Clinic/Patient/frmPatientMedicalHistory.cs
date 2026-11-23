
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

        private DataGridView dgvHistory;
        private TextBox txtVisitNotes;
        private Label lblPatient;
        private Label lblVisitCount;

        private readonly Color navy =
            Color.FromArgb(11, 61, 107);

        private readonly Color teal =
            Color.FromArgb(0, 121, 107);

        public frmPatientMedicalHistory(int patientID)
        {
            InitializeComponent();

            _patientID = patientID;

            BuildDesign();

            Load += (s, e) => LoadHistory();
        }
        private void BuildDesign()
        {
            Text = "Patient Medical History";
            Size = new Size(1050, 720);
            MinimumSize = new Size(800, 550);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(242, 247, 250);
            Font = new Font("Segoe UI", 10F);

            bool arabic = Properties.Settings.Default
                .ApplicationLanguage == "ar";

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = navy
            };

            var title = new Label
            {
                Text = arabic
                    ? "السجل الطبي للمريض"
                    : "Patient Medical History",
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI", 20F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 20)
            };

            header.Controls.Add(title);
            Controls.Add(header);

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 1,
                RowCount = 4
            };

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
                Text = $"Patient ID: {_patientID}",
                Font = new Font(
                    "Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblVisitCount = new Label
            {
                Text = "Previous Visits",
                Font = new Font(
                    "Segoe UI", 12F, FontStyle.Bold),
                ForeColor = navy,
                Dock = DockStyle.Fill
            };

            dgvHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                RowTemplate = { Height = 40 }
            };

            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.ColumnHeadersHeight = 42;

            dgvHistory.ColumnHeadersDefaultCellStyle
                .BackColor = navy;

            dgvHistory.ColumnHeadersDefaultCellStyle
                .ForeColor = Color.White;

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
                Text = arabic
                    ? "ملاحظات الزيارة"
                    : "Visit / Consultation Notes",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font(
                    "Segoe UI", 12F, FontStyle.Bold),
                ForeColor = navy
            };

            txtVisitNotes = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 11F)
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

        private void LoadHistory()
        {
            try
            {
                DataTable dt =
                    clsPatient.GetPatientVisitHistory(_patientID);

                dgvHistory.DataSource = dt;

                lblVisitCount.Text =
                    $"Previous Visits ({dt.Rows.Count})";

                if (dgvHistory.Columns.Contains("VisitNotes"))
                    dgvHistory.Columns["VisitNotes"].Visible = false;

                if (dgvHistory.Columns.Contains("VisitID"))
                    dgvHistory.Columns["VisitID"].HeaderText =
                        "Visit ID";

                if (dgvHistory.Columns.Contains("VisitDate"))
                {
                    dgvHistory.Columns["VisitDate"].HeaderText =
                        "Visit Date";

                    dgvHistory.Columns["VisitDate"]
                        .DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
                }

                if (dgvHistory.Columns.Contains("DoctorName"))
                    dgvHistory.Columns["DoctorName"].HeaderText =
                        "Doctor";

                if (dt.Rows.Count > 0)
                {
                    dgvHistory.ClearSelection();
                    dgvHistory.Rows[0].Selected = true;
                    dgvHistory.CurrentCell =
                        dgvHistory.Rows[0].Cells[0];
                    ShowSelectedNotes();
                }
                else
                {
                    txtVisitNotes.Text =
                        "No previous visits found.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load medical history:\n" +
                    ex.Message,
                    "Medical History",
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
                dgvHistory.CurrentRow.DataBoundItem
                as DataRowView;

            if (row == null)
                return;

            string notes =
                Convert.ToString(row["VisitNotes"]);

            txtVisitNotes.Text =
                string.IsNullOrWhiteSpace(notes)
                    ? "No consultation notes recorded."
                    : notes;
        }
    }
}
