using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Clinic
{
    public partial class frmMain
    {
        private Panel modernRoot;
        private Panel modernSidebar;
        private Panel modernHeader;
        private Panel modernContent;
        private FlowLayoutPanel modernDashboard;
        private Label modernClock;
        private Label modernGreeting;
        private Label modernUser;
        private readonly Color navy = Color.FromArgb(4, 37, 65);
        private readonly Color ink = Color.FromArgb(13, 42, 77);
        private readonly Color cyan = Color.FromArgb(0, 189, 213);
        private readonly Color canvasColor = Color.FromArgb(240, 247, 252);

        private static Label TextLabel(string text, float size, Color color, bool bold = false)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                ForeColor = color,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
        }

        private static Panel Rounded(Color color, int radius = 16)
        {
            Panel panel = new Panel { BackColor = color, Margin = new Padding(0) };
            panel.Resize += (s, e) =>
            {
                if (panel.Width <= 0 || panel.Height <= 0) return;
                using (GraphicsPath p = new GraphicsPath())
                {
                    int r = Math.Min(radius * 2, Math.Min(panel.Width, panel.Height));
                    p.AddArc(0, 0, r, r, 180, 90);
                    p.AddArc(panel.Width - r - 1, 0, r, r, 270, 90);
                    p.AddArc(panel.Width - r - 1, panel.Height - r - 1, r, r, 0, 90);
                    p.AddArc(0, panel.Height - r - 1, r, r, 90, 90);
                    p.CloseFigure();
                    var old = panel.Region;
                    panel.Region = new Region(p);
                    if (old != null) old.Dispose();
                }
            };
            return panel;
        }

        private static void Place(Panel parent, Control child, int x, int y, int w, int h)
        {
            child.SetBounds(x, y, w, h);
            parent.Controls.Add(child);
        }

        private Button MakeNav(string symbol, string caption, Action action, bool selected = false)
        {
            Button b = new Button
            {
                Text = "   " + symbol + "    " + caption,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = selected ? Color.FromArgb(0, 149, 188) : navy,
                Font = new Font("Segoe UI", 10F, selected ? FontStyle.Bold : FontStyle.Regular),
                Height = 42,
                Width = 230,
                Margin = new Padding(8, 1, 8, 1),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(9, 104, 143);
            if (action != null) b.Click += (s, e) => action();
            return b;
        }

        private void BuildModernDashboardDesign()
        {
            SuspendLayout();
            // Keep designer controls and their event handlers intact, but do not show the old UI.
            msMainMenue.Visible = false;
            tsQuick.Visible = false;
            pictureBox1.Visible = false;
            pnlCard1.Visible = false;
            pnlCard2.Visible = false;
            pnlCard3.Visible = false;
            statusStrip1.Visible = false;

            Text = "Clinic Management System";
            WindowState = FormWindowState.Maximized;
            BackColor = canvasColor;
            MinimumSize = new Size(1100, 720);
            modernRoot = new Panel { Dock = DockStyle.Fill, BackColor = canvasColor };
            Controls.Add(modernRoot);
            modernRoot.BringToFront();

            modernHeader = new Panel { Dock = DockStyle.Top, Height = 88, BackColor = Color.FromArgb(5, 94, 116) };
            modernRoot.Controls.Add(modernHeader);
            Label brand = TextLabel("✚  CLINIC\n     MANAGEMENT SYSTEM", 15, Color.White, true);
            Place(modernHeader, brand, 30, 12, 290, 62);
            modernUser = TextLabel("●  Admin User\n     Administrator", 11, Color.White, true);
            modernUser.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            Place(modernHeader, modernUser, 1130, 15, 235, 60);
            Button account = new Button
            {
                Text = "⚙  Account",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(6, 108, 128),
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            account.FlatAppearance.BorderSize = 0;
            Place(modernHeader, account, 1010, 24, 115, 38);
            account.Click += (s, e) => {
                ContextMenuStrip menuOptions = new ContextMenuStrip();
                menuOptions.Items.Add("Current User Info", null, (a, b) => currentUserInfoToolStripMenuItem1_Click(this, EventArgs.Empty));
                menuOptions.Items.Add("Change Password", null, (a, b) => Password32ToolStripMenuItem_Click(this, EventArgs.Empty));
                menuOptions.Items.Add("Sign Out", null, (a, b) => signOutToolStripMenuItem_Click(this, EventArgs.Empty));
                menuOptions.Show(account, new Point(0, account.Height));
            };
            TextBox search = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Text = "",
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };
            Place(modernHeader, search, 340, 27, 620, 34);
            search.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { btnFindPatient_Click(s, e); e.SuppressKeyPress = true; } };
            modernHeader.Resize += (s, e) => {
                modernUser.Left = modernHeader.Width - 245;
                account.Left = modernHeader.Width - 365;
                search.Width = Math.Max(150, account.Left - search.Left - 25);
            };

            modernSidebar = new Panel { Dock = DockStyle.Left, Width = 266, BackColor = navy, AutoScroll = true };
            modernRoot.Controls.Add(modernSidebar);
            modernSidebar.BringToFront();
            // The header spans the whole window, including the sidebar.
            modernHeader.BringToFront();
            FlowLayoutPanel menu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = navy,
                Padding = new Padding(8, 16, 0, 12)
            };
            modernSidebar.Controls.Add(menu);
            menu.Controls.Add(MakeNav("⌂", "Dashboard", () => modernDashboard.AutoScrollPosition = new Point(0, 0), true));
            AddMenuHeading(menu, "Main");
            menu.Controls.Add(MakeNav("♙", "Patients", () => PatientsToolStripMenuItem_Click(this, EventArgs.Empty)));
            menu.Controls.Add(MakeNav("▦", "Appointments", () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty)));
            menu.Controls.Add(MakeNav("✚", "Visits", () => listVisitToolStripMenuItem_Click(this, EventArgs.Empty)));
            menu.Controls.Add(MakeNav("▤", "Medical Records", () => doctorToolStripMenuItem2_Click(this, EventArgs.Empty)));
            AddMenuHeading(menu, "Clinic Services");
            menu.Controls.Add(MakeNav("▣", "Billing & Payments", null));
            menu.Controls.Add(MakeNav("▤", "Invoices", null));
            menu.Controls.Add(MakeNav("✚", "Prescriptions", null));
            menu.Controls.Add(MakeNav("◈", "Inventory / Medicines", null));
            menu.Controls.Add(MakeNav("⚗", "Laboratory", null));
            menu.Controls.Add(MakeNav("▥", "Reports", null));
            AddMenuHeading(menu, "Administration");
            menu.Controls.Add(MakeNav("♙", "Users", () => UsersToolStripMenuItem_Click(this, EventArgs.Empty)));
            menu.Controls.Add(MakeNav("⚑", "Doctors", () => DoctorsStripMenuItem1_Click(this, EventArgs.Empty)));
            menu.Controls.Add(MakeNav("⚙", "Settings", () => manageServicesToolStripMenuItem_Click(this, EventArgs.Empty)));
            menu.Controls.Add(MakeNav("↪", "Sign Out", () => signOutToolStripMenuItem_Click(this, EventArgs.Empty)));

            modernContent = new Panel { Dock = DockStyle.Fill, BackColor = canvasColor };
            modernRoot.Controls.Add(modernContent);
            modernContent.BringToFront();
            modernHeader.BringToFront();
            modernDashboard = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(20, 18, 20, 20),
                BackColor = canvasColor
            };
            modernContent.Controls.Add(modernDashboard);
            BuildDashboardRows();
            modernDashboard.Resize += (s, e) => ResizeDashboardRows();
            ResumeLayout(true);
        }

        private static void AddMenuHeading(FlowLayoutPanel menu, string heading)
        {
            Label label = TextLabel(heading, 9.5f, Color.FromArgb(159, 204, 220));
            label.Width = 230; label.Height = 40;
            label.Padding = new Padding(15, 13, 0, 0);
            menu.Controls.Add(label);
        }

        private void BuildDashboardRows()
        {
            Panel welcome = Rounded(Color.FromArgb(214, 237, 251));
            welcome.Height = 166; welcome.Margin = new Padding(0, 0, 0, 16);
            modernGreeting = TextLabel("Welcome Back,\nAdmin User 👋", 25, ink, true);
            Place(welcome, modernGreeting, 28, 18, 550, 91);
            Place(welcome, TextLabel("Here's what's happening in your clinic today.", 11,
                Color.FromArgb(67, 111, 155)), 30, 114, 600, 35);
            Label medical = TextLabel("✚  ♡  🩺", 40, Color.FromArgb(18, 145, 192));
            medical.TextAlign = ContentAlignment.MiddleCenter;
            medical.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            Place(welcome, medical, 720, 28, 250, 95);
            welcome.Resize += (s, e) => medical.Left = Math.Max(520, welcome.Width - 285);
            modernDashboard.Controls.Add(welcome);

            TableLayoutPanel stats = new TableLayoutPanel
            {
                Height = 126,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 16)
            };
            for (int i = 0; i < 4; i++) stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            AddStat(stats, 0, "♙", "Total Patients", "—", Color.FromArgb(17, 123, 234));
            AddStat(stats, 1, "▦", "Today's Appointments", "—", Color.FromArgb(3, 177, 110));
            AddStat(stats, 2, "$", "Today's Revenue", "—", Color.FromArgb(190, 38, 202));
            AddStat(stats, 3, "♡", "Active Visits", "—", Color.FromArgb(246, 137, 22));
            modernDashboard.Controls.Add(stats);

            TableLayoutPanel roleRows = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Height = 428,
                Margin = new Padding(0, 0, 0, 16)
            };
            roleRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            roleRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            roleRows.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            roleRows.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            roleRows.Controls.Add(RoleCard("✚", "Doctor Role Dashboard", Color.FromArgb(0, 121, 246),
                new string[] { "Visits\nDashboard", "Medical\nRecords", "Find\nPatient", "Diagnosis\nNotes" },
                new Action[] { () => listVisitToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => doctorToolStripMenuItem2_Click(this, EventArgs.Empty),
                    () => btnFindPatient_Click(this, EventArgs.Empty),
                    () => doctorToolStripMenuItem2_Click(this, EventArgs.Empty) }), 0, 0);
            roleRows.Controls.Add(RoleCard("$", "Cashier Role Dashboard", Color.FromArgb(0, 169, 116),
                new string[] { "Manage\nBills", "Process\nPayments", "Issue\nInvoice", "Check in\nAppointment" },
                new Action[] { null, null, null, () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty) }), 1, 0);
            roleRows.Controls.Add(RoleCard("✚", "Pharmacist Role Dashboard", Color.FromArgb(159, 37, 225),
                new string[] { "All\nPrescriptions", "In-Clinic\nPrescriptions", "Add New\nDrug", "Direct Medicine\nDispensing" },
                new Action[] { null, null, null, () => prescriptionDispensingToolStripMenuItem_Click(this, EventArgs.Empty) }), 0, 1);
            roleRows.Controls.Add(RoleCard("♙", "Receptionist Role Dashboard", Color.FromArgb(248, 105, 35),
                new string[] { "Register\nNew Patient", "Schedule\nAppointment", "Appointment\nQueue", "Patient\nCheck-in" },
                new Action[] { () => PatientsToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => btnAddApp_Click(this, EventArgs.Empty),
                    () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty) }), 1, 1);
            modernDashboard.Controls.Add(roleRows);

            Panel actions = Rounded(Color.White);
            actions.Height = 184; actions.Margin = new Padding(0, 0, 0, 12);
            Place(actions, TextLabel("Quick Actions", 15, ink, true), 20, 8, 500, 38);
            FlowLayoutPanel quick = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 126, Padding = new Padding(10, 0, 0, 0) };
            actions.Controls.Add(quick);
            QuickButton(quick, "Register New Patient", () => PatientsToolStripMenuItem_Click(this, EventArgs.Empty));
            QuickButton(quick, "New Appointment", () => btnAddApp_Click(this, EventArgs.Empty));
            QuickButton(quick, "Find Patient", () => btnFindPatient_Click(this, EventArgs.Empty));
            QuickButton(quick, "Manage Users", () => UsersToolStripMenuItem_Click(this, EventArgs.Empty));
            modernDashboard.Controls.Add(actions);
            Panel footer = Rounded(Color.White); footer.Height = 64;
            modernClock = TextLabel(DateTime.Now.ToString("dddd, dd MMMM yyyy  hh:mm tt"), 12, ink, true);
            Place(footer, modernClock, 20, 12, 600, 38);
            modernDashboard.Controls.Add(footer);
            ResizeDashboardRows();
        }

        private void ResizeDashboardRows()
        {
            if (modernDashboard == null) return;
            int w = Math.Max(720, modernDashboard.ClientSize.Width - modernDashboard.Padding.Horizontal - 24);
            foreach (Control c in modernDashboard.Controls) c.Width = w;
        }

        private static void AddStat(TableLayoutPanel row, int index, string symbol,
            string title, string value, Color accent)
        {
            Panel card = Rounded(Color.White);
            card.Dock = DockStyle.Fill; card.Margin = new Padding(0, 0, 12, 0);
            Label icon = TextLabel(symbol, 26, accent, true);
            icon.TextAlign = ContentAlignment.MiddleCenter;
            Place(card, icon, 12, 20, 65, 65);
            Label titleLabel = TextLabel(title, 10, Color.FromArgb(25, 64, 107), true);
            titleLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Place(card, titleLabel, 85, 18, 135, 35);
            Label valueLabel = TextLabel(value, 23, Color.FromArgb(9, 38, 77), true);
            valueLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Place(card, valueLabel, 85, 55, 135, 45);
            card.Resize += (s, e) => {
                titleLabel.Width = Math.Max(70, card.Width - 95);
                valueLabel.Width = Math.Max(70, card.Width - 95);
            };
            row.Controls.Add(card, index, 0);
        }

        private Panel RoleCard(string symbol, string title, Color accent, string[] labels, Action[] actions)
        {
            Panel card = Rounded(Color.White);
            card.Dock = DockStyle.Fill; card.Margin = new Padding(0, 0, 14, 14);
            Label heading = TextLabel(symbol + "   " + title, 14, ink, true);
            heading.Dock = DockStyle.Top; heading.Height = 61;
            heading.Padding = new Padding(17, 5, 0, 0);
            card.Controls.Add(heading);
            TableLayoutPanel buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                Padding = new Padding(12, 0, 12, 12)
            };
            for (int i = 0; i < 4; i++) buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            for (int i = 0; i < labels.Length; i++)
            {
                Action action = actions[i];
                Button button = new Button
                {
                    Text = labels[i],
                    Dock = DockStyle.Fill,
                    Margin = new Padding(3),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(240, 247, 254),
                    ForeColor = ink,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(219, 237, 251);
                if (action != null) button.Click += (s, e) => action();
                else { button.Enabled = false; button.ForeColor = Color.Gray; }
                buttons.Controls.Add(button, i, 0);
            }
            card.Controls.Add(buttons);
            buttons.BringToFront();
            return card;
        }

        private static void QuickButton(FlowLayoutPanel container, string title, Action action)
        {
            Button b = new Button
            {
                Text = "＋  " + title,
                Width = 195,
                Height = 43,
                Margin = new Padding(6),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(235, 247, 254),
                ForeColor = Color.FromArgb(10, 53, 91),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += (s, e) => action();
            container.Controls.Add(b);
        }
    }
}
