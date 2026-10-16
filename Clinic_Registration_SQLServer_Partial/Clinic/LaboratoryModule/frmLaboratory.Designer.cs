using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Laboratory
{
    partial class frmLaboratory
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header, statsPanel, actionsPanel, filterPanel, gridPanel;
        private Label title, subtitle;
        private Label[] statValues;
        private Button btnNewRequest, btnManageTests, btnEnterResult, btnRefresh;
        private TextBox txtSearch;
        private ComboBox cmbStatus;
        private DataGridView dgvRequests;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private static readonly Color Teal = Color.FromArgb(0, 121, 107);
        private static readonly Color Light = Color.FromArgb(242, 247, 250);
        private static readonly Color Ink = Color.FromArgb(38, 55, 70);

        private Label MakeLabel(string text, int size, bool bold = false)
        {
            return new Label { Text = text, AutoSize = true,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Ink };
        }

        private Button MakeButton(string text, Color color)
        {
            var b = new Button { Text = text, BackColor = color, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Height = 43, Width = 170,
                Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10);
            BackColor = Light;
            ClientSize = new Size(1180, 760);
            MinimumSize = new Size(760, 520);
            FormBorderStyle = FormBorderStyle.None;
            Text = "Laboratory";
            Padding = new Padding(15);

            var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5,
                BackColor = Light };
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(body);

            header = new Panel { Dock = DockStyle.Fill, BackColor = Teal, Padding = new Padding(24, 14, 12, 10) };
            title = new Label { Text = "Laboratory Management", ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold), Dock = DockStyle.Top, Height = 44 };
            subtitle = new Label { Text = "Lab requests, tests and patient results", ForeColor = Color.FromArgb(207, 239, 235),
                Font = new Font("Segoe UI", 10), Dock = DockStyle.Top, Height = 24 };
            header.Controls.Add(subtitle);
            header.Controls.Add(title);
            body.Controls.Add(header, 0, 0);

            statsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Light, Padding = new Padding(0, 12, 0, 10) };
            var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1 };
            for (int i = 0; i < 5; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            string[] captions = { "Active Lab Tests", "Total Requests", "Pending", "In Progress", "Completed" };
            statValues = new Label[5];
            for (int i = 0; i < 5; i++)
            {
                var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(4), Padding = new Padding(12) };
                var label = MakeLabel(captions[i], 10, true); label.Dock = DockStyle.Top; label.Height = 30;
                var value = MakeLabel("—", 22, true); value.ForeColor = Teal; value.Dock = DockStyle.Fill;
                card.Controls.Add(value); card.Controls.Add(label);
                cards.Controls.Add(card, i, 0); statValues[i] = value;
            }
            statsPanel.Controls.Add(cards); body.Controls.Add(statsPanel, 0, 1);

            actionsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12, 14, 12, 10) };
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true, AutoScroll = true };
            btnNewRequest = MakeButton("+ New Lab Request", Color.FromArgb(0, 150, 136));
            btnManageTests = MakeButton("Manage Lab Tests", Color.FromArgb(23, 112, 155));
            btnEnterResult = MakeButton("Enter Result", Color.FromArgb(102, 81, 169));
            btnRefresh = MakeButton("Refresh", Color.FromArgb(74, 96, 109)); btnRefresh.Width = 110;
            foreach (Button b in new[] { btnNewRequest, btnManageTests, btnEnterResult, btnRefresh })
            { b.Margin = new Padding(0, 0, 10, 0); actions.Controls.Add(b); }
            actionsPanel.Controls.Add(actions); body.Controls.Add(actionsPanel, 0, 2);

            filterPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12, 12, 12, 8) };
            var filters = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
            var searchLabel = MakeLabel("Search", 10, true); searchLabel.Margin = new Padding(0, 8, 10, 0);
            txtSearch = new TextBox { Width = 310, Font = new Font("Segoe UI", 11), Margin = new Padding(0, 3, 22, 0) };
            var statusLabel = MakeLabel("Status", 10, true); statusLabel.Margin = new Padding(0, 8, 10, 0);
            cmbStatus = new ComboBox { Width = 175, DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10), Margin = new Padding(0, 3, 0, 0) };
            cmbStatus.Items.AddRange(new object[] { "All", "Ordered", "In Progress", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            filters.Controls.Add(searchLabel); filters.Controls.Add(txtSearch);
            filters.Controls.Add(statusLabel); filters.Controls.Add(cmbStatus);
            filterPanel.Controls.Add(filters); body.Controls.Add(filterPanel, 0, 3);

            gridPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(8) };
            dgvRequests = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, MultiSelect = false, RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowTemplate = { Height = 38 },
                EnableHeadersVisualStyles = false };
            dgvRequests.ColumnHeadersHeight = 43;
            dgvRequests.ColumnHeadersDefaultCellStyle.BackColor = Teal;
            dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRequests.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvRequests.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 150, 136);
            dgvRequests.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(239, 248, 248);
            gridPanel.Controls.Add(dgvRequests); body.Controls.Add(gridPanel, 0, 4);

            Load += frmLaboratory_Load;
            txtSearch.TextChanged += FiltersChanged;
            cmbStatus.SelectedIndexChanged += FiltersChanged;
            btnRefresh.Click += (s, e) => ReloadData();
            btnNewRequest.Click += btnNewRequest_Click;
            btnManageTests.Click += btnManageTests_Click;
            btnEnterResult.Click += btnEnterResult_Click;
            ResumeLayout(false);
        }
    }
}
