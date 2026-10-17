using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Clinic_DataAccess;

namespace Clinic.Medical_Services.Radiology
{
    public partial class frmXRay : Form
    {
        private static readonly Color Teal = Color.FromArgb(0, 121, 107);
        private static readonly Color Light = Color.FromArgb(242, 247, 250);
        private Label[] statValues = new Label[5];
        private DataGridView dgvRequests;
        private TextBox txtSearch;
        private ComboBox cmbStatus;
        private DateTimePicker dtFrom, dtTo;
        private CheckBox chkDates;

        public frmXRay()
        {
            InitializeComponent();
            BuildUi();
            Load += frmXRay_Load;
        }

        private Button MakeButton(string text, Color color, int width = 155)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 43,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 9, 4)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void BuildUi()
        {
            BackColor = Light;
            Font = new Font("Segoe UI", 10);
            Padding = new Padding(15);
            var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(body);

            var header = new Panel { Dock = DockStyle.Fill, BackColor = Teal, Padding = new Padding(24, 10, 15, 8) };
            var close = MakeButton("✕ Close", Color.FromArgb(198, 45, 58), 110);
            close.Dock = DockStyle.Right;
            close.Click += (s, e) => Close();
            var title = new Label
            {
                Text = "X-Ray / Radiology Management",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 48
            };
            var subtitle = new Label
            {
                Text = "X-ray requests, imaging studies and patient results",
                ForeColor = Color.FromArgb(207, 239, 235),
                Dock = DockStyle.Top,
                Height = 25
            };
            header.Controls.Add(subtitle);
            header.Controls.Add(title);
            header.Controls.Add(close);
            body.Controls.Add(header, 0, 0);

            var cards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                BackColor = Light,
                Padding = new Padding(0, 12, 0, 10)
            };
            for (int i = 0; i < 5; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            string[] captions = { "Active X-Ray Tests", "Total Requests", "Pending", "In Progress", "Completed" };
            for (int i = 0; i < 5; i++)
            {
                var panel = new Panel
                {
                    BackColor = Color.White,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(4),
                    Padding = new Padding(12)
                };
                var label = new Label
                {
                    Text = captions[i],
                    Dock = DockStyle.Top,
                    Height = 28,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                var value = new Label
                {
                    Text = "0",
                    Dock = DockStyle.Fill,
                    ForeColor = Teal,
                    Font = new Font("Segoe UI", 22, FontStyle.Bold)
                };
                panel.Controls.Add(value);
                panel.Controls.Add(label);
                statValues[i] = value;
                cards.Controls.Add(panel, i, 0);
            }
            body.Controls.Add(cards, 0, 1);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 12, 12, 5),
                AutoScroll = true,
                WrapContents = false
            };
            var newButton = MakeButton("+ New X-Ray Request", Color.FromArgb(0, 150, 136), 200);
            var testsButton = MakeButton("Manage X-Ray Tests", Color.FromArgb(23, 112, 155), 190);
            var resultButton = MakeButton("Enter Result", Color.FromArgb(102, 81, 169), 150);
            var viewButton = MakeButton("View Image", Color.FromArgb(74, 96, 109), 145);
            var editButton = MakeButton("Edit Request", Color.FromArgb(65, 110, 170), 145);
            var deleteButton = MakeButton("Cancel Request", Color.FromArgb(180, 55, 55), 150);
            var refreshButton = MakeButton("Refresh", Color.FromArgb(74, 96, 109), 105);
            actions.Controls.AddRange(new Control[] { newButton, testsButton, resultButton, viewButton,
                editButton, deleteButton, refreshButton });
            body.Controls.Add(actions, 0, 2);

            var filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 12, 12, 4),
                WrapContents = false,
                AutoScroll = true
            };
            filters.Controls.Add(new Label { Text = "Search", AutoSize = true, Margin = new Padding(0, 9, 10, 0) });
            txtSearch = new TextBox { Width = 240, Margin = new Padding(0, 3, 18, 0) };
            filters.Controls.Add(txtSearch);
            filters.Controls.Add(new Label { Text = "Status", AutoSize = true, Margin = new Padding(0, 9, 10, 0) });
            cmbStatus = new ComboBox
            {
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 3, 18, 0)
            };
            cmbStatus.Items.AddRange(new object[] { "All", "Ordered", "In Progress", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            filters.Controls.Add(cmbStatus);
            chkDates = new CheckBox { Text = "Dates", AutoSize = true, Margin = new Padding(0, 8, 7, 0) };
            filters.Controls.Add(chkDates);
            dtFrom = new DateTimePicker { Width = 115, Format = DateTimePickerFormat.Short, Margin = new Padding(0, 3, 6, 0) };
            dtTo = new DateTimePicker { Width = 115, Format = DateTimePickerFormat.Short, Margin = new Padding(0, 3, 0, 0) };
            filters.Controls.Add(dtFrom);
            filters.Controls.Add(dtTo);
            body.Controls.Add(filters, 0, 3);

            var gridPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(8) };
            dgvRequests = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };
            dgvRequests.RowTemplate.Height = 38;
            dgvRequests.ColumnHeadersHeight = 43;
            dgvRequests.ColumnHeadersDefaultCellStyle.BackColor = Teal;
            dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRequests.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 150, 136);
            dgvRequests.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(239, 248, 248);
            gridPanel.Controls.Add(dgvRequests);
            body.Controls.Add(gridPanel, 0, 4);

            newButton.Click += (s, e) => { using (var f = new frmXRayRequest()) if (f.ShowDialog(this) == DialogResult.OK) ReloadData(); };
            testsButton.Click += (s, e) => { using (var f = new frmXRayTests()) f.ShowDialog(this); ReloadData(); };
            resultButton.Click += (s, e) => OpenResult();
            viewButton.Click += (s, e) => ViewImage();
            editButton.Click += (s, e) => EditRequest();
            deleteButton.Click += (s, e) => CancelRequest();
            refreshButton.Click += (s, e) => ReloadData();
            txtSearch.TextChanged += (s, e) => ReloadData();
            cmbStatus.SelectedIndexChanged += (s, e) => ReloadData();
            chkDates.CheckedChanged += (s, e) => ReloadData();
            dtFrom.ValueChanged += (s, e) => { if (chkDates.Checked) ReloadData(); };
            dtTo.ValueChanged += (s, e) => { if (chkDates.Checked) ReloadData(); };
            dgvRequests.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) ViewImage(); };
        }

        private void frmXRay_Load(object sender, EventArgs e)
        {
            try { ClinicDatabase.EnsureXRayTables(); ReloadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "X-Ray setup", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ReloadData()
        {
            if (!IsHandleCreated) return;
            try
            {
                var stats = ClinicDatabase.GetXRayDashboardStats();
                if (stats.Rows.Count > 0)
                {
                    string[] keys = { "TotalTests", "TotalRequests", "Pending", "InProgress", "Completed" };
                    for (int i = 0; i < keys.Length; i++) statValues[i].Text = Convert.ToString(stats.Rows[0][keys[i]]);
                }
                dgvRequests.DataSource = ClinicDatabase.GetXRayRequests(txtSearch.Text, cmbStatus.Text,
                    chkDates.Checked ? (DateTime?)dtFrom.Value.Date : null,
                    chkDates.Checked ? (DateTime?)dtTo.Value.Date : null);
                foreach (string hidden in new[] { "XRayTestID", "ClinicalNotes", "Findings", "Impression", "ResultedAt", "ImagePath" })
                    if (dgvRequests.Columns.Contains(hidden)) dgvRequests.Columns[hidden].Visible = false;
                if (dgvRequests.Columns.Contains("RequestedAt"))
                    dgvRequests.Columns["RequestedAt"].DefaultCellStyle.Format = "dd MMM yyyy HH:mm";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Load X-Ray", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private int SelectedId()
        {
            if (dgvRequests.CurrentRow == null || dgvRequests.CurrentRow.IsNewRow) return -1;
            return Convert.ToInt32(dgvRequests.CurrentRow.Cells["XRayRequestID"].Value);
        }
        private string SelectedStatus() => dgvRequests.CurrentRow == null ? "" :
            Convert.ToString(dgvRequests.CurrentRow.Cells["Status"].Value);

        private void EditRequest()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show("Select a request."); return; }
            if (SelectedStatus() != "Ordered") { MessageBox.Show("Only ordered requests can be edited."); return; }
            using (var f = new frmXRayRequest(id)) if (f.ShowDialog(this) == DialogResult.OK) ReloadData();
        }
        private void OpenResult()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show("Select a request."); return; }
            if (SelectedStatus() == "Completed" || SelectedStatus() == "Cancelled")
            { MessageBox.Show("Completed/cancelled reports cannot be overwritten."); return; }
            using (var f = new frmXRayResult(id)) if (f.ShowDialog(this) == DialogResult.OK) ReloadData();
        }
        private void CancelRequest()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show("Select a request."); return; }
            if (SelectedStatus() != "Ordered") { MessageBox.Show("Only ordered requests can be cancelled."); return; }
            if (MessageBox.Show("Cancel this X-Ray request?", "Confirm", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { if (!ClinicDatabase.DeleteXRayRequest(id)) MessageBox.Show("Request could not be cancelled."); ReloadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void ViewImage()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show("Select a request."); return; }
            DataRow row = ClinicDatabase.GetXRayRequest(id);
            if (row == null) return;
            string path = Convert.ToString(row["ImagePath"]);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            { MessageBox.Show("No available image is attached to this request."); return; }
            using (var f = new Form
            {
                Text = "X-Ray Image #" + id,
                Size = new Size(900, 680),
                StartPosition = FormStartPosition.CenterParent
            })
            using (var pb = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = LoadUnlockedImage(path)
            })
            { f.Controls.Add(pb); f.ShowDialog(this); pb.Image.Dispose(); }
        }
        internal static Image LoadUnlockedImage(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var image = Image.FromStream(stream)) return new Bitmap(image);
        }
    }
}
