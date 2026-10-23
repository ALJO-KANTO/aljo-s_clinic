using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Clinic
{
    public partial class frmMain
    {
        private Label modernBrand;
        private Button modernAccount;
        private TextBox modernSearch;
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
                Tag = text,
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
                Tag = caption,
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

            // Small toolbar for the Translate button only.
            // No clinic title, search box, Account button or user label.
            modernHeader = new Panel
            {
                Name = "modernHeader",
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.White
            };

            modernRoot.Controls.Add(modernHeader);

            modernHeader.Resize += (s, e) =>
            {
                PositionTranslateButton();
                PositionAccountHeader();
            };
            BuildAccountHeader();
            // Create the sidebar before accessing it
            modernSidebar = new Panel
            {
                Name = "modernSidebar",
                Dock = DockStyle.Left,
                Width = 266,
                BackColor = navy,
                AutoScroll = true
            };

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
            if (CanAccessMenu(clsGlobal.UserRole.Admin, clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse, clsGlobal.UserRole.Receptionist, clsGlobal.UserRole.Cashier))
                AddMenuHeading(menu, "Main");
            if (CanAccessMenu(clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse, clsGlobal.UserRole.Receptionist))
            {
                menu.Controls.Add(MakeNav("♙", "Patients", () => PatientsToolStripMenuItem_Click(this, EventArgs.Empty)));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse, clsGlobal.UserRole.Receptionist, clsGlobal.UserRole.Cashier))
            {
                menu.Controls.Add(MakeNav("▦", "Appointments", () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty)));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse))
            {
                menu.Controls.Add(MakeNav("✚", "Visits", () => listVisitToolStripMenuItem_Click(this, EventArgs.Empty)));
                menu.Controls.Add(MakeNav("▤", "Medical Records", () => doctorToolStripMenuItem2_Click(this, EventArgs.Empty)));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Admin, clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Pharmacist, clsGlobal.UserRole.Cashier))
                AddMenuHeading(menu, "Clinic Services");
            if (CanAccessMenu(clsGlobal.UserRole.Cashier))
            {
                menu.Controls.Add(MakeNav("▣", "Billing & Payments", () => OpenManageBills()));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Cashier))
            {
                menu.Controls.Add(MakeNav("🧾", "Invoices", () => IssueInvoiceToolStripMenuItem_Click(this, EventArgs.Empty)));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Pharmacist))
            {
                menu.Controls.Add(MakeNav("✚", "Prescriptions", () => listPrescriptionToolStripMenuItem_Click(this, EventArgs.Empty)));
            }
          
            if (CanAccessMenu(clsGlobal.UserRole.Doctor))
            {
                menu.Controls.Add(
                    MakeNav("⚗", "Laboratory", () =>
                    {
                        OpenLaboratory();
                    })
                );
            }
            // ERP-style expandable groups: visible only to Administrator.
            if (clsGlobal.CurrentUser != null &&
                clsGlobal.CurrentUser.RoleID == (int)clsGlobal.UserRole.Admin)
            {
                AddAdminNavigationGroups(menu);
            }

            // Administrator gets expandable accounting reports; retain the existing
            // Reports entry for Cashier without granting access to accounting reports.
            if (clsGlobal.CurrentUser != null &&
                clsGlobal.CurrentUser.RoleID == (int)clsGlobal.UserRole.Admin)
            {
                AddAdminGroup(menu, "▥", "Reports", new string[]
                {
                    "Accountant|Account Ledger|frmAccountLedger",
                    "Accountant|Balance Sheet|frmBalanceSheetReport",
                    "Accountant|Cost Center Summary|frmCostCenterSummary",
                    "Accountant|Trial Balance|MasterTrialBalance",
                    "Accountant|Profit and Loss Detail|ProfitandLossDetailReport",
                    "Accountant|Profit and Loss|ProfitandLossReport",
                    "Employee|Employee Balance Details|frmEmployeeBalanceDetails",
                    "Employee|Employee Balance Summary|frmEmployeeBalanceSummary",
                    "Employee|Employee Salary Report|frmEmployeeSalaryReport",
                    "Inventory|Physical Inventory Worksheet|frmPhysicalInventoryWorksheet",
                    "PettyCash|Petty Cash Balance Detail EMP|frmPettyCashBalanceDetailEMP",
                    "PettyCash|Petty Cash Balance Details|frmPettyCashBalanceDetails",
                    "PettyCash|Petty Cash Detail EMP Transaction|frmPettyCashDetailEMPTransactions",
                    "Purchase|Purchase by Vendor Details|frmPurchaseByVendorDetails",
                    "Purchase|Purchase by Vendor Summary|frmPurchaseByVendorSummary",
                    "Vendor|Vendor Balance Details|frmVendorBalanceDetails",
                    "Vendor|Vendor Summary|frmVendorSummary"
                });
            }
            else if (CanAccessMenu(clsGlobal.UserRole.Cashier))
            {
                menu.Controls.Add(MakeNav("▥", "Reports", null));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Admin))
                AddMenuHeading(menu, "Administration");
            if (CanAccessMenu(clsGlobal.UserRole.Admin))
            {
                menu.Controls.Add(MakeNav("♙", "Users", () => UsersToolStripMenuItem_Click(this, EventArgs.Empty)));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Admin))
                menu.Controls.Add(MakeNav("⚑", "Doctors", () => DoctorsStripMenuItem1_Click(this, EventArgs.Empty)));
            if (CanAccessMenu(clsGlobal.UserRole.Admin))
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
        private ContextMenuStrip modernAccountMenu;
        private Label modernAccountRole;

        private string HeaderTranslate(string text)
        {
            return TranslationClass.Translate(
                text,
                TranslationClass.IsArabic);
        }

        private void BuildAccountHeader()
        {
            modernAccount = new Button
            {
                Name = "modernAccount",
                Text = "Account  ▼",
                Tag = "Account",
                Width = 145,
                Height = 38,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(235, 247, 254),
                ForeColor = ink,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            modernAccount.FlatAppearance.BorderSize = 0;

            modernHeader.Controls.Add(modernAccount);

            modernUser = new Label
            {
                Name = "modernUser",
                AutoSize = false,
                Width = 200,
                Height = 22,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ink,
                TextAlign = ContentAlignment.MiddleRight
            };

            modernAccountRole = new Label
            {
                Name = "modernAccountRole",
                AutoSize = false,
                Width = 200,
                Height = 18,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleRight
            };

            modernHeader.Controls.Add(modernUser);
            modernHeader.Controls.Add(modernAccountRole);

            modernAccountMenu = new ContextMenuStrip
            {
                Font = new Font("Segoe UI", 10F),
                ShowImageMargin = false
            };

            modernAccount.Click += (s, e) =>
            {
                RefreshAccountHeader();

                modernAccountMenu.Show(
                    modernAccount,
                    new Point(0, modernAccount.Height));
            };

            RefreshAccountHeader();
            PositionAccountHeader();
        }

        private void RefreshAccountHeader()
        {
            string userName = clsGlobal.CurrentUser == null
                ? "Unknown"
                : clsGlobal.CurrentUser.UserName;

            string role = clsGlobal.CurrentUser == null
                ? ""
                : Enum.GetName(
                    typeof(clsGlobal.UserRole),
                    clsGlobal.CurrentUser.RoleID) ?? "";

            modernUser.Text = userName;
            modernAccountRole.Text = HeaderTranslate(role);

            modernAccount.Text =
                HeaderTranslate("Account") + "  ▼";

            modernAccountMenu.Items.Clear();

            ToolStripMenuItem profileItem =
                new ToolStripMenuItem(HeaderTranslate("My Account"));

            profileItem.Click += (s, e) =>
            {
                MessageBox.Show(
                    "Username: " + userName +
                    "\nRole: " + role,
                    HeaderTranslate("My Account"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            ToolStripMenuItem passwordItem =
                new ToolStripMenuItem(
                    HeaderTranslate("Change Password"));

            // Connect this to your existing password-change form.
            passwordItem.Enabled = false;

            ToolStripMenuItem logoutItem =
                new ToolStripMenuItem(HeaderTranslate("Logout"));

            logoutItem.Click += (s, e) =>
            {
                signOutToolStripMenuItem_Click(
                    this,
                    EventArgs.Empty);
            };

            modernAccountMenu.Items.Add(profileItem);
            modernAccountMenu.Items.Add(passwordItem);
            modernAccountMenu.Items.Add(
                new ToolStripSeparator());
            modernAccountMenu.Items.Add(logoutItem);
        }

        private void PositionAccountHeader()
        {
            if (modernHeader == null ||
                modernAccount == null ||
                modernUser == null ||
                modernAccountRole == null)
                return;

            bool arabic = TranslationClass.IsArabic;

            modernAccount.Size = new Size(145, 38);
            modernAccount.Top = 13;

            modernUser.AutoSize = false;
            modernUser.Size = new Size(170, 22);
            modernUser.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            modernAccountRole.AutoSize = false;
            modernAccountRole.Size = new Size(170, 18);
            modernAccountRole.Font = new Font(
                "Segoe UI", 8.5F, FontStyle.Regular);

            modernUser.AutoEllipsis = true;
            modernAccountRole.AutoEllipsis = true;

            modernUser.Top = 12;
            modernAccountRole.Top = 35;

            int spacing = 14;
            int translateWidth = btnTranslate != null
                ? btnTranslate.Width
                : 115;

            if (arabic)
            {
                modernAccount.Left = translateWidth + 35;

                modernUser.Left =
                    modernAccount.Right + spacing;

                modernAccountRole.Left = modernUser.Left;

                modernUser.TextAlign =
                    ContentAlignment.MiddleLeft;

                modernAccountRole.TextAlign =
                    ContentAlignment.MiddleLeft;
            }
            else
            {
                modernAccount.Left =
                    modernHeader.ClientSize.Width
                    - translateWidth
                    - modernAccount.Width
                    - 35;

                modernUser.Left =
                    modernAccount.Left
                    - modernUser.Width
                    - spacing;

                modernAccountRole.Left = modernUser.Left;

                modernUser.TextAlign =
                    ContentAlignment.MiddleRight;

                modernAccountRole.TextAlign =
                    ContentAlignment.MiddleRight;
            }

            modernUser.BringToFront();
            modernAccountRole.BringToFront();
            modernAccount.BringToFront();
        }
        private static void AddMenuHeading(FlowLayoutPanel menu, string heading)
        {
            Label label = TextLabel(heading, 9.5f, Color.FromArgb(159, 204, 220));
            label.Tag = heading;
            label.Width = 230; label.Height = 40;
            label.Padding = new Padding(15, 13, 0, 0);
            menu.Controls.Add(label);
        }

        // Admin-only accordion navigation. Child pages will be connected later.
        private Panel expandedAdminGroup;
        private Button expandedAdminHeader;
        private readonly Color adminNavSelected = Color.FromArgb(83, 79, 213);
        private readonly Color adminNavBackground = Color.FromArgb(4, 37, 65);

        private void AddAdminNavigationGroups(FlowLayoutPanel menu)
        {
            AddAdminGroup(menu, "▤", "Accounts", new string[]
            {
                "Account Center", "Fixed Assets", "Voucher",
                "Prepaid", "Petty Cash List"
            });

            AddAdminGroup(menu, "🛒", "Vendors", new string[]
            {
                "Vendor Center", "Purchases Center", "Payment Voucher"
            });

            AddAdminGroup(menu, "⬡", "Inventory", new string[]
            {
                "Inventory Items", "Add New Item", "Item Category", "Item Unit"
            });

            AddAdminGroup(menu, "♙", "HR", new string[]
            {
                "Human Resource", "Attendance", "Salary Sheet",
                "Leave Salary", "End Of Services", "Loans",
                "Final Settlement", "Payment Voucher"
            });
        }

        private void AddAdminGroup(FlowLayoutPanel menu, string symbol,
            string caption, string[] pages)
        {
            Panel group = new Panel
            {
                Name = "adminGroup" + caption.Replace(" ", ""),
                Width = 242,
                Height = 47,
                Margin = new Padding(3, 2, 3, 2),
                BackColor = adminNavBackground,
                Tag = caption
            };

            Button header = new Button
            {
                Name = "btnAdmin" + caption.Replace(" ", ""),
                Text = "   " + symbol + "    " + caption + "  ▾",
                Tag = caption,
                Dock = DockStyle.Top,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                BackColor = adminNavBackground,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            header.FlatAppearance.BorderSize = 0;
            header.FlatAppearance.MouseOverBackColor = adminNavSelected;

            FlowLayoutPanel children = new FlowLayoutPanel
            {
                Name = "subMenu" + caption.Replace(" ", ""),
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.FromArgb(10, 32, 60),
                Padding = new Padding(15, 5, 0, 5),
                Visible = false
            };

            string lastReportFolder = null;
            foreach (string entry in pages)
            {
                string page = entry;
                string targetForm = null;
                if (caption == "Reports")
                {
                    string[] parts = entry.Split(new[] { '|' }, 3);
                    if (parts.Length != 3) continue;
                    string folder = parts[0];
                    page = parts[1];
                    targetForm = parts[2];

                    if (!string.Equals(lastReportFolder, folder, StringComparison.Ordinal))
                    {
                        Label folderHeading = new Label
                        {
                            Name = "lblReport" + folder + "Heading",
                            Tag = folder,
                            Text = folder,
                            Width = 214,
                            Height = 28,
                            Margin = new Padding(0, 3, 0, 2),
                            ForeColor = Color.FromArgb(159, 204, 220),
                            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        children.Controls.Add(folderHeading);
                        lastReportFolder = folder;
                    }
                }

                Button item = new Button
                {
                    Name = "btn" + caption.Replace(" ", "") + page.Replace(" ", ""),
                    Text = "│  ├─  " + page,
                    Tag = page,
                    AccessibleDescription = targetForm, // Exact target form class for report navigation
                    Width = 214,
                    Height = 36,
                    Margin = new Padding(0, 1, 0, 1),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(10, 32, 60),
                    ForeColor = Color.FromArgb(223, 229, 245),
                    Font = new Font("Segoe UI", 9F),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };
                item.FlatAppearance.BorderSize = 0;
                item.FlatAppearance.MouseOverBackColor = Color.FromArgb(32, 56, 101);
                children.Controls.Add(item);
            }

            // Dock order: child fills the area below the fixed header.
            group.Controls.Add(children);
            group.Controls.Add(header);

            header.Click += (sender, args) =>
            {
                bool open = !children.Visible;

                if (expandedAdminGroup != null && expandedAdminGroup != group)
                {
                    FlowLayoutPanel previousChildren = null;
                    foreach (Control c in expandedAdminGroup.Controls)
                    {
                        previousChildren = c as FlowLayoutPanel;
                        if (previousChildren != null) break;
                    }
                    if (previousChildren != null) previousChildren.Visible = false;
                    expandedAdminGroup.Height = 47;
                    if (expandedAdminHeader != null)
                        expandedAdminHeader.BackColor = adminNavBackground;
                }

                children.Visible = open;
                group.Height = open
                    ? 47 + children.Padding.Vertical + children.Controls.Cast<Control>().Sum(c => c.Height + c.Margin.Vertical)
                    : 47;
                header.BackColor = open ? adminNavSelected : adminNavBackground;
                expandedAdminGroup = open ? group : null;
                expandedAdminHeader = open ? header : null;
                menu.PerformLayout();
            };

            menu.Controls.Add(group);
        }

        private void BuildDashboardRows()
        {
            Panel welcome = Rounded(Color.FromArgb(214, 237, 251));
            welcome.Height = 166;
            welcome.Margin = new Padding(0, 0, 0, 16);

            modernGreeting = TextLabel("", 16F, ink, true);
            modernGreeting.AutoSize = false;
            modernGreeting.AutoEllipsis = false;

            Place(welcome, modernGreeting, 28, 44, 550, 60);
            Label welcomeDescription = TextLabel(
     "Here's what's happening in your clinic today.",
     11,
     Color.FromArgb(67, 111, 155));

            Place(welcome, welcomeDescription, 30, 112, 600, 32);
            Label medical = TextLabel("✚  ♡  🩺", 40, Color.FromArgb(18, 145, 192));
            medical.TextAlign = ContentAlignment.MiddleCenter;
            medical.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            Place(welcome, medical, 720, 28, 250, 95);
            welcome.Resize += (s, e) =>
            {
                medical.Left = isArabic
                    ? 28
                    : Math.Max(520, welcome.Width - 285);

                modernGreeting.Left = isArabic
                    ? Math.Max(28, welcome.Width - modernGreeting.Width - 28)
                    : 28;
            };
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

            bool isAdmin = clsGlobal.CurrentUser.RoleID == (int)clsGlobal.UserRole.Admin;
            TableLayoutPanel roleRows = new TableLayoutPanel
            {
                ColumnCount = isAdmin ? 2 : 1,
                RowCount = isAdmin ? 3 : 1,
                Height = isAdmin ? 428 : 180,
                Margin = new Padding(0, 0, 0, 16),
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
            //roleRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            //roleRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            //roleRows.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            //roleRows.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            if (isAdmin)
            {
                roleRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                roleRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                for (int i = 0; i < 3; i++)
                {
                    roleRows.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 3));
                }
            }
            else
            {
                roleRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                roleRows.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            }
            if (CanShowDashboard(clsGlobal.UserRole.Doctor))
            {
                roleRows.Controls.Add(
                    RoleCard("✚", "Doctor Role Dashboard", Color.FromArgb(0, 121, 246),
                            new string[] { "Visits\nDashboard", "Medical\nRecords", "Find\nPatient", "Diagnosis\nNotes" },
                            new Action[] { () => listVisitToolStripMenuItem_Click(this, EventArgs.Empty),
                                () => doctorToolStripMenuItem2_Click(this, EventArgs.Empty),
                                () => btnFindPatient_Click(this, EventArgs.Empty),
                                () => doctorToolStripMenuItem2_Click(this, EventArgs.Empty) }
                    ), 0, 0);
            }
            if (CanShowDashboard(clsGlobal.UserRole.Nurse))
            {
                roleRows.Controls.Add(
                    RoleCard("✚", "Nurse Role Dashboard", Color.FromArgb(15, 118, 110),
                        new string[] { "Nurse\nStation" },
                        new Action[] { () => nurseToolStripMenuItem1_Click(this, EventArgs.Empty) }
                    ), 0, 2);
            }
            if (CanShowDashboard(clsGlobal.UserRole.Receptionist))
            {
                roleRows.Controls.Add(RoleCard("♙", "Receptionist Role Dashboard", Color.FromArgb(248, 105, 35),
                new string[] { "Register\nNew Patient", "Schedule\nAppointment", "Appointment\nQueue", "Patient\nCheck-in" },
                new Action[] { () => PatientsToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => btnAddApp_Click(this, EventArgs.Empty),
                    () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty) }), 1, 1);
            }
            if (CanShowDashboard(clsGlobal.UserRole.Pharmacist))
            {
                roleRows.Controls.Add(RoleCard("✚", "Pharmacist Role Dashboard", Color.FromArgb(159, 37, 225),
                new string[] { "All\nPrescriptions", "In-Clinic\nDoctor\nPrescription", "Add New\nDrug", "Direct Medicine\nDispensing" },
                new Action[]
                {
                    () => listPrescriptionToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => inClinicDoctorToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => addNewDrugToolStripMenuItem_Click(this, EventArgs.Empty),
                    () => directMedicinenDispensingToolStripMenuItem_Click(this, EventArgs.Empty)
                }), 0, 1);
            }
            if (CanShowDashboard(clsGlobal.UserRole.Cashier))
            {
                roleRows.Controls.Add(RoleCard(
                     "$",
                     "Cashier Role Dashboard",
                     Color.FromArgb(0, 169, 116),
                     new string[]
                     {
                        "Manage\nBills",
                        "Process\nPayments",
                        "Issue\nInvoice",
                        "Check in\nAppointment"
                     },
                     new Action[]
                     {
                        () => OpenManageBills(),
                        () => OpenManageBills(),
                        () => IssueInvoiceToolStripMenuItem_Click(this, EventArgs.Empty),
                        () => appointmentsToolStripMenuItem_Click(this, EventArgs.Empty)
                     }
                 ), 1, 0);
            }

            modernDashboard.Controls.Add(roleRows);

            Panel actions = Rounded(Color.White);
            actions.Height = 184; actions.Margin = new Padding(0, 0, 0, 12);
            Place(actions, TextLabel("Quick Actions", 15, ink, true), 20, 8, 500, 38);
            FlowLayoutPanel quick = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 126, Padding = new Padding(10, 0, 0, 0) };
            actions.Controls.Add(quick);
            if (CanAccessMenu(clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse, clsGlobal.UserRole.Receptionist))
            {
                QuickButton(quick, "Register New Patient", () => PatientsToolStripMenuItem_Click(this, EventArgs.Empty));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse, clsGlobal.UserRole.Receptionist, clsGlobal.UserRole.Cashier))
            {
                QuickButton(quick, "New Appointment", () => btnAddApp_Click(this, EventArgs.Empty));
            }

            if (CanAccessMenu(clsGlobal.UserRole.Doctor, clsGlobal.UserRole.Nurse, clsGlobal.UserRole.Receptionist))
            {
                QuickButton(quick, "Find Patient", () => btnFindPatient_Click(this, EventArgs.Empty));
            }
            if (CanAccessMenu(clsGlobal.UserRole.Admin))
            {
                QuickButton(quick, "Manage Users", () => UsersToolStripMenuItem_Click(this, EventArgs.Empty));
            }


            if (quick.Controls.Count > 0)
            {
                modernDashboard.Controls.Add(actions);
            }
            else
            {
                actions.Dispose();
            }

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
            card.Resize += (s, e) =>
            {
                bool arabic = TranslationClass.IsArabic;

                icon.Left = arabic
                    ? card.Width - icon.Width - 12
                    : 12;

                titleLabel.Left = arabic ? 12 : 85;
                valueLabel.Left = arabic ? 12 : 85;

                titleLabel.Width = Math.Max(70, card.Width - 100);
                valueLabel.Width = Math.Max(70, card.Width - 100);

                titleLabel.TextAlign = arabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft;

                valueLabel.TextAlign = arabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft;
            }; row.Controls.Add(card, index, 0);
        }

        private Panel RoleCard(string symbol, string title, Color accent, string[] labels, Action[] actions)
        {
            Panel card = Rounded(Color.White);
            card.Dock = DockStyle.Fill; card.Margin = new Padding(0, 0, 14, 14);
            Label heading = TextLabel(symbol + "   " + title, 14, ink, true);
            heading.Dock = DockStyle.Top; heading.Height = 61;
            heading.Padding = new Padding(17, 5, 0, 0);
            heading.Tag = heading.Text;
            card.Controls.Add(heading);

            int buttonCount = labels.Length;
            TableLayoutPanel buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = buttonCount,
                RowCount = 1,
                Padding = new Padding(12, 0, 12, 12)
            };
            //for (int i = 0; i < 4; i++) 
            //    buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            for (int i = 0; i < buttonCount; i++)
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / buttonCount));

            for (int i = 0; i < labels.Length; i++)
            {
                Action action = actions[i];
                Button button = new Button
                {
                    Text = labels[i],
                    Tag = labels[i],
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
                Tag = title,
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
