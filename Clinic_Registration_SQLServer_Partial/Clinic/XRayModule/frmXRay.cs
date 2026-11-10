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
        private readonly System.Collections.Generic.Dictionary<string, Image> icons =
            new System.Collections.Generic.Dictionary<string, Image>();
        private readonly System.Collections.Generic.Dictionary<int, Image> thumbnails =
            new System.Collections.Generic.Dictionary<int, Image>();
        private static readonly Color Navy = Color.FromArgb(29, 53, 84);

        // Uses the existing TranslationClass dictionary. Add translations there only.
        private string T(string english)
        {
            return TranslationClass.Translate(english, TranslationClass.IsArabic);
        }

        public frmXRay()
        {
            InitializeComponent();

            LoadIcons();
            BuildUi();
            ApplyArabicLayout();
            Load += frmXRay_Load;
        }

        // Apply the same layout mirroring method for Arabic and restore
        // the normal layout when English is selected.
        private void ApplyArabicLayout()
        {
            bool isArabic = TranslationClass.IsArabic;

            RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            // Keep manual header positions stable; Arabic text direction is applied to controls below.
            RightToLeftLayout = false;

            ApplyArabicLayoutToControls(this, isArabic);
            PerformLayout();
            Invalidate(true);
        }

        private void ApplyArabicLayoutToControls(Control parent, bool isArabic)
        {
            parent.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;

            foreach (Control control in parent.Controls)
            {
                control.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;

                if (control is FlowLayoutPanel flow)
                {
                    // Let FlowDirection control the order. Keeping the panel's
                    // own RightToLeft as No avoids a second RTL reversal, which
                    // otherwise makes the Arabic buttons/filters appear LTR.
                    flow.RightToLeft = RightToLeft.No;
                    flow.FlowDirection = isArabic
                        ? FlowDirection.RightToLeft
                        : FlowDirection.LeftToRight;
                    flow.WrapContents = false;
                    flow.PerformLayout();
                }

                if (control is Label label)
                {
                    label.TextAlign = isArabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }

                if (control is DataGridView grid)
                {
                    grid.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                    grid.ColumnHeadersDefaultCellStyle.Alignment = isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleCenter;
                    grid.DefaultCellStyle.Alignment = isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;
                }

                // The five dashboard cards are manually placed in reverse
                // order for Arabic, so do not let the table reverse them again.
                if (control is TableLayoutPanel table &&
                    table.RowCount == 1 && table.ColumnCount == 5)
                {
                    table.RightToLeft = RightToLeft.No;
                }

                ApplyArabicLayoutToControls(control, isArabic);
            }

            if (parent is FlowLayoutPanel flowParent)
                flowParent.PerformLayout();
        }

        private void LoadIcons()
        {
            string folder = Path.Combine(Application.StartupPath, "RadiologyAssets");
            foreach (string key in new[] { "lungs", "requests", "pending", "progress", "completed" })
            {
                string path = Path.Combine(folder, key + ".png");
                if (File.Exists(path))
                {
                    try { icons[key] = LoadUnlockedImage(path); } catch { }
                }
            }
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
            Text = T("X-Ray / Radiology Management");
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

            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(0, 121, 132),
                Padding = new Padding(15, 10, 15, 8)
            };

            // Position header controls explicitly; do not use Dock for these labels.
            // In Arabic, the close button stays on the far left and the title is
            // anchored to the far right of the teal header.
            bool arabicLayout = TranslationClass.IsArabic;
            var close = MakeButton("✕ " + T("Close"), Color.FromArgb(198, 45, 58), 110);
            close.Dock = DockStyle.None;
            close.Margin = new Padding(0);
            close.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            close.Click += (s, e) => Close();

            var title = new Label
            {
                Text = T("X-Ray / Radiology Management"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = false,
                Height = 48,
                TextAlign = arabicLayout
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            var subtitle = new Label
            {
                Text = T("X-ray requests, imaging studies and patient results"),
                ForeColor = Color.FromArgb(207, 239, 235),
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Height = 25,
                TextAlign = arabicLayout
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            header.Controls.Add(close);
            header.Controls.Add(title);
            header.Controls.Add(subtitle);

            Action arrangeHeader = () =>
            {
                int sideMargin = 15;
                int buttonWidth = 110;
                int buttonHeight = Math.Max(40, header.ClientSize.Height - 20);
                int titleLeft;
                int titleWidth;

                close.SetBounds(
                    arabicLayout ? sideMargin : header.ClientSize.Width - sideMargin - buttonWidth,
                    10, buttonWidth, buttonHeight);

                if (arabicLayout)
                {
                    // Reserve the left edge for the red close button and place
                    // the title block in the RIGHT portion of the teal header.
                    // Right-align both labels so their text ends at the far right.
                    titleLeft = Math.Max(buttonWidth + (sideMargin * 2),
                                         (int)(header.ClientSize.Width * 0.40));
                    titleWidth = Math.Max(0,
                        header.ClientSize.Width - titleLeft - sideMargin);
                    title.RightToLeft = RightToLeft.Yes;
                    subtitle.RightToLeft = RightToLeft.Yes;
                    title.TextAlign = ContentAlignment.MiddleRight;
                    subtitle.TextAlign = ContentAlignment.MiddleRight;
                    title.BringToFront();
                    subtitle.BringToFront();
                    close.BringToFront();
                }
                else
                {
                    titleLeft = sideMargin;
                    titleWidth = Math.Max(0, close.Left - 12 - titleLeft);
                    title.RightToLeft = RightToLeft.No;
                    subtitle.RightToLeft = RightToLeft.No;
                    title.TextAlign = ContentAlignment.MiddleLeft;
                    subtitle.TextAlign = ContentAlignment.MiddleLeft;
                }

                title.SetBounds(titleLeft, 7, titleWidth, 48);
                subtitle.SetBounds(titleLeft, 56, titleWidth, 25);
            };

            header.Resize += (s, e) => arrangeHeader();
            arrangeHeader();
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
            // Card positions are reversed explicitly for Arabic.
            cards.RightToLeft = RightToLeft.No;
            string[] captions = { T("Active X-Ray Tests"), T("Total Requests"), T("Pending"), T("In Progress"), T("Completed") };
            string[] iconKeys = { "lungs", "requests", "pending", "progress", "completed" };
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
                if (icons.ContainsKey(iconKeys[i]))
                {
                    var iconBox = new PictureBox
                    {
                        Image = icons[iconKeys[i]],
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Width = 40,
                        Height = 42,
                        Dock = TranslationClass.IsArabic ? DockStyle.Left : DockStyle.Right,
                        Margin = new Padding(0, 12, 4, 0)
                    };
                    panel.Controls.Add(iconBox);
                    iconBox.BringToFront();
                }
                panel.Controls.Add(value);
                panel.Controls.Add(label);
                statValues[i] = value;
                // Place cards explicitly so the first Arabic card starts on the right.
                // English remains in its original left-to-right order.
                int cardColumn = arabicLayout ? 4 - i : i;
                cards.Controls.Add(panel, cardColumn, 0);
            }
            body.Controls.Add(cards, 0, 1);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 12, 12, 5),
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = TranslationClass.IsArabic
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight
            };
            var newButton = MakeButton("+ " + T("New X-Ray Request"), Color.FromArgb(0, 150, 136), 200);
            var testsButton = MakeButton(T("Manage X-Ray Tests"), Color.FromArgb(23, 112, 155), 190);
            var resultButton = MakeButton(T("Enter Result"), Color.FromArgb(102, 81, 169), 150);
            var viewButton = MakeButton(T("View Image"), Color.FromArgb(74, 96, 109), 145);
            var refreshButton = MakeButton(T("Refresh"), Color.FromArgb(74, 96, 109), 105);
            actions.Controls.AddRange(new Control[] { newButton, testsButton, resultButton, viewButton, refreshButton });
            actions.FlowDirection = TranslationClass.IsArabic
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;
            body.Controls.Add(actions, 0, 2);

            var filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 12, 12, 4),
                WrapContents = false,
                AutoScroll = true,
                FlowDirection = TranslationClass.IsArabic
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight
            };
            filters.Controls.Add(new Label { Text = T("Search"), AutoSize = true, Margin = new Padding(0, 9, 10, 0) });
            txtSearch = new TextBox { Width = 240, Margin = new Padding(0, 3, 18, 0) };
            filters.Controls.Add(txtSearch);
            filters.Controls.Add(new Label { Text = T("Status"), AutoSize = true, Margin = new Padding(0, 9, 10, 0) });
            cmbStatus = new ComboBox
            {
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 3, 18, 0)
            };
            cmbStatus.Items.AddRange(new object[] { T("All"), T("Ordered"), T("In Progress"), T("Completed"), T("Cancelled") });
            cmbStatus.SelectedIndex = 0;
            filters.Controls.Add(cmbStatus);
            chkDates = new CheckBox { Text = T("Dates"), AutoSize = true, Margin = new Padding(0, 8, 7, 0) };
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
            dgvRequests.RowTemplate.Height = 52;
            dgvRequests.ColumnHeadersHeight = 43;
            dgvRequests.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 128, 139);
            dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRequests.DefaultCellStyle.SelectionBackColor = Color.FromArgb(213, 246, 246);
            dgvRequests.DefaultCellStyle.SelectionForeColor = Navy;
            dgvRequests.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(239, 248, 248);
            gridPanel.Controls.Add(dgvRequests);
            body.Controls.Add(gridPanel, 0, 4);

            newButton.Click += (s, e) => { using (var f = new frmXRayRequest()) if (f.ShowDialog(this) == DialogResult.OK) ReloadData(); };
            testsButton.Click += (s, e) => { using (var f = new frmXRayTests()) f.ShowDialog(this); ReloadData(); };
            resultButton.Click += (s, e) => OpenResult();
            viewButton.Click += (s, e) => ViewImage();
            refreshButton.Click += (s, e) => ReloadData();
            txtSearch.TextChanged += (s, e) => ReloadData();
            cmbStatus.SelectedIndexChanged += (s, e) => ReloadData();
            chkDates.CheckedChanged += (s, e) => ReloadData();
            dtFrom.ValueChanged += (s, e) => { if (chkDates.Checked) ReloadData(); };
            dtTo.ValueChanged += (s, e) => { if (chkDates.Checked) ReloadData(); };
            dgvRequests.CellContentClick += DgvRequests_CellContentClick;
            dgvRequests.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvRequests.Columns[e.ColumnIndex].Name == "Image") ViewImage(); };
        }

        private void frmXRay_Load(object sender, EventArgs e)
        {
            try { ClinicDatabase.EnsureXRayTables(); ReloadData(); }
            catch (Exception ex) { MessageBox.Show(T(ex.Message), T("X-Ray setup"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
                foreach (var image in thumbnails.Values) image.Dispose();
                thumbnails.Clear();
                dgvRequests.Columns.Clear();
                dgvRequests.AutoGenerateColumns = true;
                DataTable data = ClinicDatabase.GetXRayRequests(txtSearch.Text, GetSelectedStatusEnglish(),
                    chkDates.Checked ? (DateTime?)dtFrom.Value.Date : null,
                    chkDates.Checked ? (DateTime?)dtTo.Value.Date : null);
                dgvRequests.DataSource = data;
                TranslateGridHeaders();
                TranslateGridStatuses();
                foreach (string hidden in new[] { "XRayTestID", "ClinicalNotes", "Findings", "Impression", "ResultedAt", "ImagePath" })
                    if (dgvRequests.Columns.Contains(hidden)) dgvRequests.Columns[hidden].Visible = false;
                if (dgvRequests.Columns.Contains("RequestedAt"))
                    dgvRequests.Columns["RequestedAt"].DefaultCellStyle.Format = "dd MMM yyyy HH:mm";

                var imageColumn = new DataGridViewImageColumn
                {
                    Name = "Image",
                    HeaderText = T("Image"),
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 95,
                    MinimumWidth = 85,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    DefaultCellStyle = { NullValue = null }
                };
                dgvRequests.Columns.Add(imageColumn);
                AddActionColumn("ViewAction", T("View"), "👁");
                AddActionColumn("EditAction", T("Edit"), "✎");
                AddActionColumn("DeleteAction", T("Delete"), "✕");

                // DataGridView mirrors its visible column order in RTL mode.
                dgvRequests.RightToLeft = TranslationClass.IsArabic ? RightToLeft.Yes : RightToLeft.No;
                dgvRequests.ColumnHeadersDefaultCellStyle.Alignment = TranslationClass.IsArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleCenter;
                dgvRequests.DefaultCellStyle.Alignment = TranslationClass.IsArabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

                if (dgvRequests.Columns.Contains("XRayRequestID"))
                    dgvRequests.Columns["XRayRequestID"].DisplayIndex = 0;
                foreach (DataGridViewRow gridRow in dgvRequests.Rows)
                {
                    if (gridRow.IsNewRow) continue;
                    int id = Convert.ToInt32(gridRow.Cells["XRayRequestID"].Value);
                    string path = data.Columns.Contains("ImagePath") ?
                        Convert.ToString(gridRow.Cells["ImagePath"].Value) : "";
                    if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    {
                        try
                        {
                            thumbnails[id] = LoadUnlockedImage(path);
                            gridRow.Cells["Image"].Value = thumbnails[id];
                        }
                        catch { gridRow.Cells["Image"].Value = null; }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(T(ex.Message), T("Load X-Ray"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string GetSelectedStatusEnglish()
        {
            string[] statuses = { "All", "Ordered", "In Progress", "Completed", "Cancelled" };
            int index = cmbStatus.SelectedIndex;
            return index >= 0 && index < statuses.Length ? statuses[index] : "All";
        }

        private void TranslateGridHeaders()
        {
            if (dgvRequests.Columns.Contains("XRayRequestID")) dgvRequests.Columns["XRayRequestID"].HeaderText = T("XRayRequestID");
            if (dgvRequests.Columns.Contains("VisitID")) dgvRequests.Columns["VisitID"].HeaderText = T("VisitID");
            if (dgvRequests.Columns.Contains("PatientName")) dgvRequests.Columns["PatientName"].HeaderText = T("PatientName");
            if (dgvRequests.Columns.Contains("TestName")) dgvRequests.Columns["TestName"].HeaderText = T("TestName");
            if (dgvRequests.Columns.Contains("RequestedAt")) dgvRequests.Columns["RequestedAt"].HeaderText = T("RequestedAt");
            if (dgvRequests.Columns.Contains("Status")) dgvRequests.Columns["Status"].HeaderText = T("Status");
        }

        private void TranslateGridStatuses()
        {
            if (!dgvRequests.Columns.Contains("Status")) return;
            foreach (DataGridViewRow row in dgvRequests.Rows)
            {
                if (row.IsNewRow) continue;
                string status = Convert.ToString(row.Cells["Status"].Value);
                if (!string.IsNullOrEmpty(status)) row.Cells["Status"].Value = T(status);
            }
        }

        private void AddActionColumn(string name, string title, string label)
        {
            var col = new DataGridViewButtonColumn
            {
                Name = name,
                HeaderText = title,
                Text = label,
                UseColumnTextForButtonValue = true,
                Width = 56,
                MinimumWidth = 52,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat
            };
            dgvRequests.Columns.Add(col);
        }

        private void DgvRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string column = dgvRequests.Columns[e.ColumnIndex].Name;
            if (column == "ViewAction") ViewImage();
            else if (column == "EditAction") EditRequest();
            else if (column == "DeleteAction") CancelRequest();
        }

        private int SelectedId()
        {
            if (dgvRequests.CurrentRow == null || dgvRequests.CurrentRow.IsNewRow) return -1;
            return Convert.ToInt32(dgvRequests.CurrentRow.Cells["XRayRequestID"].Value);
        }
        private string SelectedStatus()
        {
            if (dgvRequests.CurrentRow == null) return "";
            string status = Convert.ToString(dgvRequests.CurrentRow.Cells["Status"].Value);
            string[] statuses = { "Ordered", "In Progress", "Completed", "Cancelled" };
            foreach (string english in statuses)
                if (string.Equals(status, T(english), StringComparison.OrdinalIgnoreCase)) return english;
            return status;
        }

        private void EditRequest()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show(T("Select a request.")); return; }
            if (SelectedStatus() != "Ordered") { MessageBox.Show(T("Only ordered requests can be edited.")); return; }
            using (var f = new frmXRayRequest(id)) if (f.ShowDialog(this) == DialogResult.OK) ReloadData();
        }
        private void OpenResult()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show(T("Select a request.")); return; }
            if (SelectedStatus() == "Completed" || SelectedStatus() == "Cancelled")
            { MessageBox.Show(T("Completed/cancelled reports cannot be overwritten.")); return; }
            using (var f = new frmXRayResult(id)) if (f.ShowDialog(this) == DialogResult.OK) ReloadData();
        }
        private void CancelRequest()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show(T("Select a request.")); return; }
            if (SelectedStatus() != "Ordered") { MessageBox.Show(T("Only ordered requests can be cancelled.")); return; }
            if (MessageBox.Show(T("Cancel this X-Ray request?"), T("Confirm"), MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { if (!ClinicDatabase.DeleteXRayRequest(id)) MessageBox.Show(T("Request could not be cancelled.")); ReloadData(); }
            catch (Exception ex) { MessageBox.Show(T(ex.Message)); }
        }
        private void ViewImage()
        {
            int id = SelectedId();
            if (id < 1) { MessageBox.Show(T("Select a request.")); return; }
            DataRow row = ClinicDatabase.GetXRayRequest(id);
            if (row == null) return;
            string path = Convert.ToString(row["ImagePath"]);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            { MessageBox.Show(T("No available image is attached to this request.")); return; }
            using (var f = new Form
            {
                Text = T("X-Ray Image") + " #" + id,
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
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            foreach (var image in thumbnails.Values) image.Dispose();
            foreach (var image in icons.Values) image.Dispose();
            base.OnFormClosed(e);
        }

        internal static Image LoadUnlockedImage(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var image = Image.FromStream(stream)) return new Bitmap(image);
        }

    }
}
