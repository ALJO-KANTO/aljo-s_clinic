using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Vital
{
    partial class frmNurse
    {
        private bool _nurseModernApplied;
        private readonly System.Collections.Generic.List<Control> _nurseModernDecorations = new System.Collections.Generic.List<Control>();

        private static Color NurseColor(int r, int g, int b) { return Color.FromArgb(r, g, b); }

        private void ApplyNurseModernDesign()
        {
            if (_nurseModernApplied) return;
            _nurseModernApplied = true;
            SuspendLayout();
            BackColor = NurseColor(238, 248, 252);
            panTitle.BackColor = NurseColor(0, 127, 137);
            panel1.BackColor = NurseColor(238, 248, 252);
            foreach (Panel p in new[] { pnlAction, panel2, panel3 })
            {
                p.BackColor = Color.White;
                p.BorderStyle = BorderStyle.None;
                NurseRound(p, 17);
            }
            foreach (Panel p in new[] { panel4, panel5 })
            {
                p.BackColor = NurseColor(0, 132, 141);
                NurseRound(p, 12);
            }
            NurseRound(panTitle, 15);
            label11.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lbDate.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            label1.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            label2.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lbPatientName.ForeColor = NurseColor(0, 149, 149);
            NurseActionButton(btnWaiting_For_Vitals, "Waiting Vitals", NurseColor(255, 117, 13), NurseIcon.Clock);
            NurseActionButton(btnNextPatient, "Next Patient", NurseColor(0, 143, 243), NurseIcon.PersonPlus);
            NurseActionButton(btnClose, "Close", NurseColor(245, 58, 76), NurseIcon.Close);
            NurseActionButton(btnSave, "Save and Send Next Patient", NurseColor(0, 149, 151), NurseIcon.Save);
            btnSave.TextAlign = ContentAlignment.MiddleCenter;
            btnSave.ImageAlign = ContentAlignment.MiddleLeft;
            btnRefresh.Image = NurseDrawIcon(NurseIcon.Refresh, Color.White, 40);
            btnRefresh.ImageAlign = ContentAlignment.MiddleCenter;
            btnExit.Image = NurseDrawIcon(NurseIcon.PersonPlus, Color.White, 34);
            btnExit.ImageAlign = ContentAlignment.MiddleCenter;
            // Replace the four repeated ID images with colored medical icon tiles.
            NurseVitalsTile(pictureBox14, NurseIcon.Stethoscope, NurseColor(244, 63, 94), NurseColor(255, 231, 235));
            NurseVitalsTile(pictureBox1, NurseIcon.Thermometer, NurseColor(239, 68, 68), NurseColor(255, 238, 215));
            NurseVitalsTile(pictureBox2, NurseIcon.Weight, NurseColor(104, 58, 235), NurseColor(239, 232, 255));
            NurseVitalsTile(pictureBox3, NurseIcon.Pulse, NurseColor(5, 150, 105), NurseColor(207, 250, 238));
            dgvNurseQueue.BackgroundColor = Color.White;
            dgvNurseQueue.BorderStyle = BorderStyle.None;
            dgvNurseQueue.GridColor = NurseColor(225, 235, 245);
            dgvNurseQueue.EnableHeadersVisualStyles = false;
            dgvNurseQueue.ColumnHeadersDefaultCellStyle.BackColor = NurseColor(235, 245, 253);
            dgvNurseQueue.ColumnHeadersDefaultCellStyle.ForeColor = NurseColor(23, 42, 72);
            dgvNurseQueue.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvNurseQueue.ColumnHeadersHeight = 45;
            dgvNurseQueue.DefaultCellStyle.SelectionBackColor = NurseColor(208, 245, 242);
            dgvNurseQueue.DefaultCellStyle.SelectionForeColor = NurseColor(12, 74, 83);
            dgvNurseQueue.AlternatingRowsDefaultCellStyle.BackColor = NurseColor(248, 251, 255);
            foreach (Control c in new Control[] { txtBloodPressure, nudTemperature, nudWeight, nudPulse })
            {
                c.BackColor = NurseColor(250, 252, 255);
                c.ForeColor = NurseColor(19, 42, 72);
                c.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            }
            // Keep original data, validation and selection behavior intact.
            ResumeLayout(true);
        }

        private void NurseActionButton(Button button, string caption, Color color, NurseIcon icon)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            button.Text = "      " + caption;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Image = NurseDrawIcon(icon, Color.White, 30);
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.Padding = new Padding(9, 0, 5, 0);
            NurseRound(button, 12);
        }

        private void NurseVitalsTile(PictureBox picture, NurseIcon icon, Color ink, Color background)
        {
            picture.BackColor = background;
            picture.Image = NurseDrawIcon(icon, ink, 32);
            picture.Size = new Size(48, 48);
            picture.SizeMode = PictureBoxSizeMode.CenterImage;
            // Center the enlarged icon on the original position.
            picture.Left -= 8;
            picture.Top -= 10;
            NurseRound(picture, 11);
        }

        private void NurseRound(Control control, int radius)
        {
            Action update = () =>
            {
                if (control.Width < 2 || control.Height < 2) return;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int d = Math.Min(radius * 2, Math.Min(control.Width, control.Height));
                    Rectangle r = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
                    path.AddArc(r.Left, r.Top, d, d, 180, 90);
                    path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
                    path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                    path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
                    path.CloseFigure();
                    Region old = control.Region;
                    control.Region = new Region(path);
                    if (old != null) old.Dispose();
                }
            };
            control.SizeChanged += (s, e) => update();
            update();
        }

        private enum NurseIcon { Clock, PersonPlus, Close, Save, Refresh, Stethoscope, Thermometer, Weight, Pulse }

        private static Bitmap NurseDrawIcon(NurseIcon icon, Color color, int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen pen = new Pen(color, 2.7f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.ScaleTransform(size / 32f, size / 32f);
                switch (icon)
                {
                    case NurseIcon.Clock:
                        g.DrawEllipse(pen, 4, 4, 24, 24); g.DrawLines(pen, new[] { new Point(16, 8), new Point(16, 17), new Point(22, 20) }); break;
                    case NurseIcon.PersonPlus:
                        g.FillEllipse(brush, 4, 3, 12, 12); g.FillPie(brush, 1, 16, 20, 14, 180, 180);
                        g.DrawLine(pen, 25, 15, 25, 29); g.DrawLine(pen, 18, 22, 32, 22); break;
                    case NurseIcon.Close:
                        g.DrawEllipse(pen, 3, 3, 26, 26); g.DrawLine(pen, 11, 11, 21, 21); g.DrawLine(pen, 21, 11, 11, 21); break;
                    case NurseIcon.Save:
                        g.DrawRectangle(pen, 5, 4, 22, 24); g.DrawRectangle(pen, 10, 4, 12, 9); g.DrawRectangle(pen, 10, 19, 12, 9); break;
                    case NurseIcon.Refresh:
                        g.DrawArc(pen, 5, 5, 22, 22, 35, 285); g.DrawLines(pen, new[] { new Point(21, 4), new Point(28, 5), new Point(27, 12) }); break;
                    case NurseIcon.Thermometer:
                        g.DrawArc(pen, 11, 2, 10, 15, 180, 180); g.DrawLine(pen, 11, 9, 11, 20); g.DrawLine(pen, 21, 9, 21, 20);
                        g.DrawEllipse(pen, 9, 18, 14, 12); g.DrawLine(pen, 16, 10, 16, 24); break;
                    case NurseIcon.Weight:
                        g.DrawRectangle(pen, 5, 4, 22, 25); g.DrawArc(pen, 11, 8, 10, 9, 180, 180); g.DrawLine(pen, 16, 11, 16, 15); break;
                    case NurseIcon.Pulse:
                        g.DrawLines(pen, new[] { new Point(2, 17), new Point(9, 17), new Point(13, 8), new Point(17, 26), new Point(21, 13), new Point(24, 17), new Point(30, 17) }); break;
                    case NurseIcon.Stethoscope:
                        g.DrawArc(pen, 5, 4, 14, 18, 0, 180); g.DrawLine(pen, 5, 12, 5, 17); g.DrawLine(pen, 19, 12, 19, 17);
                        g.DrawArc(pen, 12, 16, 15, 12, 0, 180); g.DrawEllipse(pen, 22, 17, 6, 6); break;
                }
            }
            return bmp;
        }
    }
}
