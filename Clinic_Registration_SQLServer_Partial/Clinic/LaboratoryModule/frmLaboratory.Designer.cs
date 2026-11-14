using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Laboratory
{
    partial class frmLaboratory
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header, statsPanel, actionsPanel, filterPanel, gridPanel;
        private Label title, subtitle;
        private Label[] statValues;
        private Button btnNewRequest, btnManageTests, btnSendToBilling, btnEnterResult, btnRefresh, btnClose;
        private TextBox txtSearch;
        private ComboBox cmbStatus;
        private DataGridView dgvRequests;

        private static readonly Color Navy = Color.FromArgb(13, 64, 124);
        private static readonly Color Background = Color.FromArgb(232, 244, 254);
        private static readonly Color TextInk = Color.FromArgb(20, 43, 78);

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d < 2) { path.AddRectangle(r); return path; }
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Rounded panel with a real clipped background. Child labels use transparent backgrounds.
        private class Surface : Panel
        {
            public Color StartColor { get; set; } = Color.White;
            public Color EndColor { get; set; } = Color.White;
            public Color Outline { get; set; } = Color.FromArgb(219, 232, 247);
            public int CornerRadius { get; set; } = 16;
            public Surface()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                         ControlStyles.SupportsTransparentBackColor, true);
            }
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                e.Graphics.Clear(Parent == null ? Background : Parent.BackColor);
                if (Width < 2 || Height < 2) return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(1, 1, Width - 3, Height - 3);
                using (GraphicsPath path = RoundRect(rect, CornerRadius))
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, StartColor, EndColor, LinearGradientMode.Horizontal))
                using (Pen border = new Pen(Outline))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(border, path);
                }
            }
        }

        // Paint the complete header as one surface. No child labels can overlap the icon.
        private sealed class HeaderSurface : Surface
        {
            public HeaderSurface()
            {
                StartColor = Color.FromArgb(10, 78, 162);
                EndColor = Color.FromArgb(9, 45, 105);
                Outline = Color.FromArgb(31, 102, 185);
                CornerRadius = 20;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath clip = RoundRect(new Rectangle(1, 1, Width - 3, Height - 3), CornerRadius))
                {
                    GraphicsState saved = g.Save();
                    g.SetClip(clip);
                    using (Brush glow = new SolidBrush(Color.FromArgb(18, 45, 157, 240)))
                    {
                        g.FillEllipse(glow, Width - 440, -170, 510, 340);
                        g.FillEllipse(glow, Width - 190, -95, 290, 260);
                    }
                    g.Restore(saved);
                }
                Rectangle iconCircle = new Rectangle(28, (Height - 78) / 2, 78, 78);
                using (var b = new LinearGradientBrush(iconCircle, Color.FromArgb(35, 167, 255), Color.FromArgb(20, 92, 211), 70f))
                    g.FillEllipse(b, iconCircle);
                // Flask icon, drawn directly to avoid the opaque rectangular child-control background.
                float cx = iconCircle.Left + iconCircle.Width / 2f;
                float cy = iconCircle.Top + iconCircle.Height / 2f;
                using (var pen = new Pen(Color.White, 3.1f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, cx - 8, cy - 19, cx + 8, cy - 19);
                    g.DrawLine(pen, cx - 5, cy - 19, cx - 5, cy - 6);
                    g.DrawLine(pen, cx + 5, cy - 19, cx + 5, cy - 6);
                    g.DrawLine(pen, cx - 5, cy - 6, cx - 18, cy + 18);
                    g.DrawLine(pen, cx + 5, cy - 6, cx + 18, cy + 18);
                    g.DrawLine(pen, cx - 18, cy + 18, cx + 18, cy + 18);
                    g.DrawLine(pen, cx - 11, cy + 7, cx + 11, cy + 7);
                }
                int textWidth = Math.Max(40, Width - 310);
                using (Font heading = new Font("Segoe UI", 23f, FontStyle.Bold))
                using (Font subheading = new Font("Segoe UI", 11f))
                {
                    TextRenderer.DrawText(g, "Laboratory Management", heading,
                        new Rectangle(132, 25, textWidth, 48), Color.White,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    TextRenderer.DrawText(g, "Lab requests, tests and patient results", subheading,
                        new Rectangle(134, 75, textWidth, 27), Color.FromArgb(216, 235, 255),
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }

                // Close button is drawn ON the header, not inside a child control.
                // The surrounding pixels remain the original navy header gradient.
                Rectangle bounds = GetHeaderCloseBounds(this);
                Rectangle buttonRect = new Rectangle(bounds.X + 1, bounds.Y + 1,
                    bounds.Width - 3, bounds.Height - 3);
                using (GraphicsPath buttonPath = RoundRect(buttonRect, 18))
                using (LinearGradientBrush fill = new LinearGradientBrush(buttonRect,
                    Color.FromArgb(37, 100, 174), Color.FromArgb(12, 57, 123),
                    LinearGradientMode.Vertical))
                using (Pen outline = new Pen(Color.FromArgb(125, 174, 215)))
                {
                    g.FillPath(fill, buttonPath);
                    g.DrawPath(outline, buttonPath);
                }
                using (Font buttonFont = new Font("Segoe UI", 10f, FontStyle.Bold))
                    TextRenderer.DrawText(g, "Close  ×", buttonFont, bounds, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.NoPadding);

            }
        }

        private static Rectangle GetHeaderCloseBounds(Control surface)
        {
            const int width = 116, height = 42;
            return new Rectangle(Math.Max(0, surface.ClientSize.Width - width - 24),
                Math.Max(0, (surface.ClientSize.Height - height) / 2), width, height);
        }

        private sealed class StatCard : Surface
        {
            public Color Accent { get; set; }
            public Color Tint { get; set; }
            public string Symbol { get; set; }
            public StatCard(Color accent, Color tint, string symbol)
            {
                Accent = accent; Tint = tint; Symbol = symbol;
                StartColor = Color.White; EndColor = tint;
                CornerRadius = 16;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath clip = RoundRect(new Rectangle(1, 1, Width - 3, Height - 3), CornerRadius))
                {
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.SetClip(clip);
                    using (Brush wave = new SolidBrush(Color.FromArgb(24, Accent)))
                        e.Graphics.FillEllipse(wave, Width - 145, Height - 30, 195, 100);
                    e.Graphics.Restore(state);
                }
                Rectangle circle = new Rectangle(17, (Height - 65) / 2, 65, 65);
                using (Brush b = new SolidBrush(Color.FromArgb(28, Accent))) e.Graphics.FillEllipse(b, circle);
                using (Font f = new Font("Segoe UI Symbol", 26, FontStyle.Bold))
                    TextRenderer.DrawText(e.Graphics, Symbol, f, circle, Accent,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        private sealed class ModernButton : Button
        {
            public Color GradientEnd { get; set; } = Color.FromArgb(10, 65, 160);
            public int CornerRadius { get; set; } = 11;
            public bool IsHeaderButton { get; set; } = false;
            public ModernButton()
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                FlatAppearance.MouseOverBackColor = Color.Transparent;
                FlatAppearance.MouseDownBackColor = Color.Transparent;
                UseVisualStyleBackColor = false;
                Cursor = Cursors.Hand;
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor, true);
            }
            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                if (Width > 2 && Height > 2)
                {
                    using (GraphicsPath p = RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius))
                    {
                        Region old = Region;
                        Region = new Region(p);
                        if (old != null) old.Dispose();
                    }
                }
            }
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                // Ask WinForms to repaint the parent behind this transparent hit target.
                if (IsHeaderButton) { base.OnPaintBackground(e); return; }
                base.OnPaintBackground(e);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                // HeaderSurface draws the Close button, so this control is only clickable.
                if (IsHeaderButton) return;
                // The rounded Region clips the entire button, avoiding square corner patches.
                // Do not clear with an unrelated opaque color: that caused the visible halo.
                // The button is region-clipped; fill its actual rounded area only.
                if (!IsHeaderButton)
                    e.Graphics.Clear(Parent == null ? Color.White : Parent.BackColor);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(1, 1, Width - 3, Height - 3);
                if (r.Width < 2 || r.Height < 2) return;
                using (GraphicsPath path = RoundRect(r, CornerRadius))
                using (LinearGradientBrush brush = new LinearGradientBrush(r, BackColor, GradientEnd, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPath(brush, path);
                    using (Pen edge = new Pen(IsHeaderButton
                        ? Color.FromArgb(95, 174, 220, 255)
                        : Color.FromArgb(65, Color.White)))
                        e.Graphics.DrawPath(edge, path);
                }
                TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(7, 0, Width - 14, Height),
                    ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        private static Label MakeLabel(string text, float size, bool bold = false)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = TextInk,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }
        private static Button MakeAction(string name, string text, Color first, Color last, int width)
        {
            return new ModernButton
            {
                Name = name,
                Text = text,
                Width = width,
                Height = 52,
                BackColor = first,
                GradientEnd = last,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Margin = new Padding(0, 0, 14, 0)
            };
        }

        private void ApplyLaboratoryGridWidths()
        {
            if (dgvRequests == null || dgvRequests.IsDisposed) return;
            dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvRequests.ScrollBars = ScrollBars.Both;
            foreach (DataGridViewColumn column in dgvRequests.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                string key = (column.DataPropertyName + " " + column.Name + " " + column.HeaderText).ToLowerInvariant();
                int width = 112;
                if (key.Contains("patientname")) width = 165;
                else if (key.Contains("testname")) width = 165;
                else if (key.Contains("requestedat")) width = 165;
                else if (key.Contains("savedtestprice")) width = 140;
                else if (key.Contains("paidamount") || key.Contains("balancedue")) width = 125;
                else if (key.Contains("labrequestid")) width = 125;
                else if (key.Contains("laborderid")) width = 115;
                else if (key.Contains("status")) width = 145;
                else if (key.Contains("print")) width = 135;
                column.Width = width;
            }
        }

        // Draw only the Print Report cell contents; the existing CellClick logic
        // still receives the click because the DataGridView itself is unchanged.
        private void LaboratoryGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            DataGridViewColumn col = dgvRequests.Columns[e.ColumnIndex];
            string key = (col.Name + " " + col.HeaderText).ToLowerInvariant();
            if (!key.Contains("print")) return;

            // Paint the entire cell ourselves. A DataGridViewButtonCell may have
            // a green Style.BackColor even when its value is a dash; letting the
            // default background paint caused the solid green rectangles.
            bool selected = (e.State & DataGridViewElementStates.Selected) != 0;
            Color rowBack = selected ? Color.FromArgb(203, 230, 255)
                : (e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(240, 248, 255));
            using (Brush backgroundBrush = new SolidBrush(rowBack))
                e.Graphics.FillRectangle(backgroundBrush, e.CellBounds);
            using (Pen divider = new Pen(Color.FromArgb(222, 234, 245)))
                e.Graphics.DrawLine(divider, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                    e.CellBounds.Right, e.CellBounds.Bottom - 1);

            string value = Convert.ToString(e.FormattedValue) ?? "";
            bool canPrint = value.Trim().Equals("Print", StringComparison.OrdinalIgnoreCase);
            Rectangle r = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 9,
                Math.Max(12, e.CellBounds.Width - 24), Math.Max(12, e.CellBounds.Height - 18));
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color fillColor = canPrint ? Color.FromArgb(4, 132, 105) : Color.FromArgb(245, 248, 252);
            Color borderColor = canPrint ? Color.FromArgb(4, 132, 105) : Color.FromArgb(215, 227, 239);
            using (GraphicsPath path = RoundRect(r, 8))
            using (Brush fill = new SolidBrush(fillColor))
            using (Pen outline = new Pen(borderColor))
            {
                e.Graphics.FillPath(fill, path);
                e.Graphics.DrawPath(outline, path);
            }
            using (Font font = new Font("Segoe UI", 9.5f,
                canPrint ? FontStyle.Bold : FontStyle.Regular))
                TextRenderer.DrawText(e.Graphics, canPrint ? "Print" : "—", font, r,
                    canPrint ? Color.White : Color.FromArgb(133, 151, 170),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.NoPadding);
            e.Handled = true;
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 10);
            BackColor = Background;
            ClientSize = new Size(1440, 850);
            MinimumSize = new Size(820, 570);
            FormBorderStyle = FormBorderStyle.None;
            Text = "Laboratory";
            Padding = new Padding(18, 14, 18, 18);

            TableLayoutPanel body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Background
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 138));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 146));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 103));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(body);

            // The heading, subtitle and flask are rendered by HeaderSurface itself.
            // This avoids WinForms transparent-label painting/z-order problems.
            header = new HeaderSurface { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 13) };
            title = MakeLabel("Laboratory Management", 23, true);
            subtitle = MakeLabel("Lab requests, tests and patient results", 11);
            // Draw the Close button directly on HeaderSurface instead of using a
            // transparent child control (WinForms transparency is not true alpha).
            btnClose = MakeAction("btnClose", "Close  ×", Color.FromArgb(36, 91, 157),
                Color.FromArgb(10, 49, 109), 116);
            btnClose.Height = 42;
            btnClose.Click += (s, e) => Close();
            // Do NOT add btnClose to header.Controls: the header paints it itself.
            header.MouseMove += (s, e) =>
            {
                bool over = GetHeaderCloseBounds(header).Contains(e.Location);
                header.Cursor = over ? Cursors.Hand : Cursors.Default;
            };
            header.MouseLeave += (s, e) => header.Cursor = Cursors.Default;
            header.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && GetHeaderCloseBounds(header).Contains(e.Location))
                    btnClose.PerformClick();
            };
            body.Controls.Add(header, 0, 0);

            // Statistics: icons, color-tinted cards and wave details.
            statsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Background, Padding = new Padding(0, 3, 0, 15) };
            TableLayoutPanel cards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                BackColor = Background,
                Margin = Padding.Empty
            };
            for (int i = 0; i < 5; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            cards.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            string[] captions = { "Active Lab Tests", "Total Requests", "Pending", "In Progress", "Completed" };
            string[] symbols = { "♧", "▤", "⌛", "↻", "✓" };
            Color[] accents = {
                Color.FromArgb(25, 96, 199), Color.FromArgb(106, 68, 206),
                Color.FromArgb(230, 113, 17), Color.FromArgb(0, 141, 190),
                Color.FromArgb(20, 117, 72)
            };
            Color[] tints = {
                Color.FromArgb(230, 241, 255), Color.FromArgb(240, 234, 255),
                Color.FromArgb(255, 241, 226), Color.FromArgb(227, 247, 255),
                Color.FromArgb(230, 248, 239)
            };
            statValues = new Label[5];
            for (int i = 0; i < 5; i++)
            {
                StatCard card = new StatCard(accents[i], tints[i], symbols[i])
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(i == 0 ? 0 : 6, 1, i == 4 ? 0 : 6, 3)
                };
                Label caption = MakeLabel(captions[i], 10, true);
                caption.Name = "statCaption" + i;
                caption.SetBounds(96, 20, 155, 29);
                Label value = MakeLabel("—", 25, true);
                value.Name = "statValue" + i;
                value.ForeColor = accents[i];
                value.SetBounds(96, 48, 135, 53);
                card.Controls.Add(caption);
                card.Controls.Add(value);
                card.Resize += (s, e) =>
                {
                    int available = Math.Max(40, card.ClientSize.Width - 105);
                    caption.Width = available;
                    value.Width = available;
                };
                cards.Controls.Add(card, i, 0);
                statValues[i] = value;
            }
            statsPanel.Controls.Add(cards);
            body.Controls.Add(statsPanel, 0, 1);

            actionsPanel = new Surface
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 9),
                Padding = new Padding(17, 16, 14, 12),
                CornerRadius = 17
            };
            FlowLayoutPanel actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White,
                Margin = Padding.Empty
            };
            btnNewRequest = MakeAction("btnNewRequest", "+   New Lab Request", Color.FromArgb(32, 128, 249), Color.FromArgb(9, 67, 173), 235);
            btnManageTests = MakeAction("btnManageTests", "▤   Manage Lab Tests", Color.FromArgb(131, 103, 216), Color.FromArgb(66, 54, 154), 235);
            btnSendToBilling = MakeAction("btnSendToBilling", "▣   Send to Billing", Color.FromArgb(0, 166, 209), Color.FromArgb(0, 103, 154), 225);
            btnEnterResult = MakeAction("btnEnterResult", "⚗   Enter Result", Color.FromArgb(25, 143, 248), Color.FromArgb(7, 85, 185), 210);
            btnRefresh = MakeAction("btnRefresh", "↻   Refresh", Color.FromArgb(248, 251, 255), Color.FromArgb(221, 234, 255), 145);
            btnRefresh.ForeColor = Navy;
            foreach (Button b in new[] { btnNewRequest, btnManageTests, btnSendToBilling, btnEnterResult, btnRefresh })
                actions.Controls.Add(b);
            actionsPanel.Controls.Add(actions);
            body.Controls.Add(actionsPanel, 0, 2);

            filterPanel = new Surface
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(24, 15, 12, 9),
                CornerRadius = 16
            };
            FlowLayoutPanel filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White,
                Margin = Padding.Empty
            };
            Label searchLabel = MakeLabel("Search", 10, true);
            searchLabel.Size = new Size(72, 40);
            searchLabel.Margin = new Padding(0, 0, 12, 0);
            txtSearch = new TextBox
            {
                Name = "txtSearch",
                Width = 390,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255),
                Margin = new Padding(0, 6, 34, 0)
            };
            Label statusLabel = MakeLabel("Status", 10, true);
            statusLabel.Size = new Size(66, 40);
            statusLabel.Margin = new Padding(0, 0, 12, 0);
            cmbStatus = new ComboBox
            {
                Name = "cmbStatus",
                Width = 235,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 6, 0, 0)
            };
            cmbStatus.Items.AddRange(new object[] { "All", "Ordered", "In Progress", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            filters.Controls.Add(searchLabel);
            filters.Controls.Add(txtSearch);
            filters.Controls.Add(statusLabel);
            filters.Controls.Add(cmbStatus);
            filterPanel.Controls.Add(filters);
            body.Controls.Add(filterPanel, 0, 3);

            gridPanel = new Surface
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(13, 15, 13, 14),
                CornerRadius = 17
            };
            dgvRequests = new DataGridView
            {
                Name = "dgvRequests",
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ScrollBars = ScrollBars.Both,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(220, 233, 247),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                RowTemplate = { Height = 49 }
            };
            dgvRequests.ColumnHeadersHeight = 49;
            dgvRequests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvRequests.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(27, 79, 147);
            dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRequests.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(27, 79, 147);
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvRequests.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            dgvRequests.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgvRequests.DefaultCellStyle.ForeColor = TextInk;
            dgvRequests.DefaultCellStyle.BackColor = Color.White;
            dgvRequests.DefaultCellStyle.SelectionBackColor = Color.FromArgb(203, 230, 255);
            dgvRequests.DefaultCellStyle.SelectionForeColor = TextInk;
            dgvRequests.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            dgvRequests.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgvRequests.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(203, 230, 255);
            dgvRequests.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextInk;
            // Set practical minimum widths so the grid can scroll horizontally rather
            // than squeezing all columns and truncating important data.
            dgvRequests.DataBindingComplete += (s, e) => ApplyLaboratoryGridWidths();
            dgvRequests.ColumnAdded += (s, e) =>
            {
                e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                e.Column.MinimumWidth = 70;
            };
            dgvRequests.CellPainting += LaboratoryGrid_CellPainting;
            gridPanel.Controls.Add(dgvRequests);
            body.Controls.Add(gridPanel, 0, 4);

            Load += frmLaboratory_Load;
            txtSearch.TextChanged += FiltersChanged;
            cmbStatus.SelectedIndexChanged += FiltersChanged;
            btnRefresh.Click += (s, e) => ReloadData();
            btnNewRequest.Click += btnNewRequest_Click;
            btnManageTests.Click += btnManageTests_Click;
            btnEnterResult.Click += btnEnterResult_Click;
            btnSendToBilling.Click += btnSendToBilling_Click;
            ResumeLayout(false);
        }
    }
}
