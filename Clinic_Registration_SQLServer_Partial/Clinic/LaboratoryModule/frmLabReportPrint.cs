using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Laboratory
{
    public class frmLabReportPrint : Form
    {
        private readonly int _requestID;
        private readonly PrintDocument _printDocument;
        private DataRow _report;
        private DataTable _results;
        private int _nextRow;
        private int _pageNumber;
        private readonly Color _primary = Color.FromArgb(18, 67, 91);
        private readonly Color _accent = Color.FromArgb(0, 135, 125);

        public frmLabReportPrint(int requestID)
        {
            _requestID = requestID;
            Text = "Laboratory Report";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(620, 250);
            MinimumSize = new Size(500, 220);
            Font = new Font("Segoe UI", 10);
            BackColor = Color.White;

            _printDocument = new PrintDocument();
            _printDocument.DocumentName = "Laboratory Report - " + requestID;
            _printDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            _printDocument.DefaultPageSettings.Landscape = false;
            _printDocument.DefaultPageSettings.Margins = new Margins(45, 45, 45, 45);
            _printDocument.BeginPrint += (s, e) => { _nextRow = 0; _pageNumber = 0; };
            _printDocument.PrintPage += PrintDocument_PrintPage;

            Label lblTitle = new Label
            {
                Text = "Laboratory Report",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _primary,
                AutoSize = true,
                Location = new Point(25, 20)
            };
            Label lblRequest = new Label
            {
                Text = "Request #" + requestID,
                AutoSize = true,
                ForeColor = Color.DimGray,
                Location = new Point(28, 65)
            };
            Button btnPreview = new Button
            {
                Text = "Print Preview",
                Size = new Size(155, 43),
                Location = new Point(25, 110),
                BackColor = _primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            Button btnPrint = new Button
            {
                Text = "Print Report",
                Size = new Size(155, 43),
                Location = new Point(195, 110),
                BackColor = _accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPreview.Click += (s, e) =>
            {
                if (!LoadReport()) return;
                using (var preview = new PrintPreviewDialog())
                {
                    preview.Document = _printDocument;
                    preview.Width = 1050;
                    preview.Height = 800;
                    preview.ShowDialog(this);
                }
            };
            btnPrint.Click += (s, e) =>
            {
                if (!LoadReport()) return;
                foreach (DataRow r in _results.Rows)
                    if (Convert.ToString(r["ObservedValue"]) == "Not entered")
                    {
                        MessageBox.Show("Cannot print a final laboratory report while parameter results are missing.",
                            "Incomplete Results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                using (var dialog = new PrintDialog())
                {
                    dialog.Document = _printDocument;
                    dialog.UseEXDialog = true;
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        _printDocument.Print();
                }
            };
            Controls.Add(lblTitle);
            Controls.Add(lblRequest);
            Controls.Add(btnPreview);
            Controls.Add(btnPrint);
        }

        private bool LoadReport()
        {
            try
            {
                DataTable details = ClinicDatabase.GetLabReportDetails(_requestID);
                if (details == null || details.Rows.Count == 0)
                    throw new Exception("Laboratory request not found.");

                DataRow row = details.Rows[0];
                string status = Convert.ToString(row["RequestStatus"]);
                if (!string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("The laboratory result must be completed before printing.");

                decimal price = Convert.ToDecimal(row["SavedTestPrice"]);
                decimal paid = Convert.ToDecimal(row["PaidAmount"]);
                if (price <= 0 || paid < price)
                    throw new Exception("Full payment is required before printing the laboratory report.");

                // Load the parameters configured for the actual test in this request.
                // Patient observations may exist in either result table, depending on
                // which result-entry workflow saved them. Never manufacture observations.
                DataTable entries = new DataTable();
                using (var con = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                using (var cmd = new MySqlCommand(@"
                    SELECT
                        p.ParameterID,
                        p.ParameterName,
                        COALESCE(NULLIF(e.ObservedValue, ''),
                                 NULLIF(r.ObservedValue, ''), '') AS ObservedValue,
                        COALESCE(NULLIF(e.SavedUnit, ''),
                                 NULLIF(r.SavedUnit, ''), p.Unit, '') AS Unit,
                        COALESCE(NULLIF(e.SavedReferenceRange, ''),
                                 NULLIF(r.SavedReferenceRange, ''),
                                 NULLIF(p.ReferenceRange, ''),
                                 CONCAT('Male: ', COALESCE(p.MaleReferenceRange, '-'),
                                        ' / Female: ', COALESCE(p.FemaleReferenceRange, '-')))
                                 AS ReferenceRange,
                        COALESCE(NULLIF(e.ResultFlag, ''),
                                 NULLIF(r.ResultFlag, ''), '') AS ResultFlag,
                        COALESCE(NULLIF(e.ResultNotes, ''),
                                 NULLIF(r.ResultNotes, ''), '') AS ResultNotes
                    FROM labrequests lr
                    INNER JOIN laboratorytestparameters p
                        ON p.TestID = lr.TestID AND p.IsActive = 1
                    LEFT JOIN labparameterresultentries e
                        ON e.LabRequestID = lr.LabRequestID
                       AND e.ParameterID = p.ParameterID
                    LEFT JOIN laboratoryparameterresults r
                        ON r.LabRequestID = lr.LabRequestID
                       AND r.ParameterID = p.ParameterID
                    WHERE lr.LabRequestID = @requestID
                    ORDER BY p.DisplayOrder, p.ParameterID;", con))
                {
                    cmd.Parameters.AddWithValue("@requestID", _requestID);
                    using (var adapter = new MySqlDataAdapter(cmd))
                        adapter.Fill(entries);
                }

                if (entries.Rows.Count == 0)
                    throw new Exception("No configured parameters found for the test in this request.");

                int missing = 0;
                foreach (DataRow result in entries.Rows)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(result["ObservedValue"])))
                    {
                        result["ObservedValue"] = "Not entered";
                        result["ResultFlag"] = "Pending";
                        missing++;
                    }
                }

                _report = row;
                _results = entries;

               

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to Load Laboratory Report",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private string Field(string name)
        {
            if (_report == null || !_report.Table.Columns.Contains(name) ||
                _report[name] == DBNull.Value) return "-";
            string value = Convert.ToString(_report[name]);
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }
        private string DateField(string name)
        {
            if (_report == null || !_report.Table.Columns.Contains(name) ||
                _report[name] == DBNull.Value) return "-";
            return Convert.ToDateTime(_report[name]).ToString("dd MMM yyyy, hh:mm tt");
        }
        private static string Cell(DataRow row, string column)
        {
            if (row[column] == DBNull.Value) return "";
            return Convert.ToString(row[column]) ?? "";
        }
        private static void DrawText(Graphics g, string text, Font font, Brush brush,
            float x, float y, float width, float height)
        {
            using (var format = new StringFormat())
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.LineLimit;
                g.DrawString(text ?? "", font, brush,
                    new RectangleF(x, y, Math.Max(1, width), Math.Max(1, height)), format);
            }
        }
        private static float TextHeight(Graphics g, string text, Font font, float width)
        {
            if (string.IsNullOrWhiteSpace(text)) return font.Height + 2;
            return g.MeasureString(text, font, (int)Math.Max(10, width)).Height + 2;
        }
        private void Pair(Graphics g, string label, string value, float x, float y,
            float width, Font small, Font bold)
        {
            DrawText(g, label, small, Brushes.DimGray, x, y, width, 17);
            DrawText(g, value, bold, Brushes.Black, x, y + 16, width, 34);
        }
        private float Section(Graphics g, string title, float x, float y,
            float width, Font font, Pen line)
        {
            using (var brush = new SolidBrush(_primary))
                DrawText(g, title, font, brush, x, y, width, 24);
            y += 25;
            g.DrawLine(line, x, y, x + width, y);
            return y + 12;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_report == null || _results == null) { e.HasMorePages = false; return; }
            _pageNumber++;
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
            float w = e.MarginBounds.Width;
            float bottom = e.MarginBounds.Bottom;
            float footerTop = bottom - 73;

            using (Font clinic = new Font("Segoe UI", 16, FontStyle.Bold))
            using (Font heading = new Font("Segoe UI", 11, FontStyle.Bold))
            using (Font section = new Font("Segoe UI", 9, FontStyle.Bold))
            using (Font body = new Font("Segoe UI", 8))
            using (Font bold = new Font("Segoe UI", 8, FontStyle.Bold))
            using (Font small = new Font("Segoe UI", 7))
            using (Pen line = new Pen(Color.FromArgb(185, 200, 205)))
            using (Brush primary = new SolidBrush(_primary))
            using (Brush accent = new SolidBrush(_accent))
            {
                DrawText(g, "CLINIC MEDICAL CENTER", clinic, primary, x, y, w, 32);
                y += 31;
                DrawText(g, "DIAGNOSTIC LABORATORY", bold, accent, x, y, w, 19);
                y += 23;
                g.DrawLine(line, x, y, x + w, y);
                y += 12;
                DrawText(g, "LABORATORY TEST REPORT", heading, primary, x, y, w, 25);
                y += 34;

                Pair(g, "REQUEST NO.", Field("LabRequestID"), x, y, w * .47f, small, bold);
                Pair(g, "REQUEST DATE", DateField("RequestedAt"), x + w * .51f, y, w * .48f, small, bold);
                y += 47;
                y = Section(g, "PATIENT INFORMATION", x, y, w, section, line);
                Pair(g, "PATIENT NAME", Field("PatientName"), x, y, w * .47f, small, bold);
                Pair(g, "PATIENT ID", Field("PatientID"), x + w * .51f, y, w * .48f, small, bold);
                y += 45;
                Pair(g, "REFERRING DOCTOR", Field("DoctorName"), x, y, w * .65f, small, bold);
                y += 45;
                y = Section(g, "TEST DETAILS", x, y, w, section, line);
                Pair(g, "TEST NAME", Field("TestName"), x, y, w * .68f, small, bold);
                Pair(g, "TEST PRICE", "AED " + Convert.ToDecimal(_report["SavedTestPrice"]).ToString("N2"),
                    x + w * .72f, y, w * .28f, small, bold);
                y += 49;
                y = Section(g, "PARAMETER RESULTS", x, y, w, section, line);

                // Width proportions: parameter 29%, result 12%, unit 12%, range 25%, flag 12%, remarks 10%.
                float[] widths = { .29f, .12f, .12f, .25f, .12f, .10f };
                float[] offsets = new float[6];
                for (int i = 1; i < 6; i++) offsets[i] = offsets[i - 1] + widths[i - 1];
                string[] headers = { "Parameter", "Result", "Unit", "Reference interval", "Flag", "Remarks" };
                g.FillRectangle(Brushes.Gainsboro, x, y, w, 32);
                for (int i = 0; i < 6; i++)
                    DrawText(g, headers[i], bold, Brushes.Black,
                        x + offsets[i] * w + 3, y + 6, widths[i] * w - 6, 26);
                y += 32;
                g.DrawLine(line, x, y, x + w, y);

                // Print only as many parameter rows as fit; continue on next page.
                while (_nextRow < _results.Rows.Count)
                {
                    DataRow r = _results.Rows[_nextRow];
                    string[] cells = {
                        Cell(r, "ParameterName"), Cell(r, "ObservedValue"),
                        Cell(r, "Unit"), Cell(r, "ReferenceRange"),
                        Cell(r, "ResultFlag"), Cell(r, "ResultNotes")
                    };
                    float rowHeight = 28;
                    for (int i = 0; i < cells.Length; i++)
                        rowHeight = Math.Max(rowHeight,
                            TextHeight(g, cells[i], i == 1 ? bold : body, widths[i] * w - 9) + 12);
                    rowHeight = Math.Min(rowHeight, 100);
                    if (y + rowHeight > footerTop - 25) break;

                    if (_nextRow % 2 == 1)
                        using (var stripe = new SolidBrush(Color.FromArgb(245, 249, 249)))
                            g.FillRectangle(stripe, x, y, w, rowHeight);
                    for (int i = 0; i < 6; i++)
                    {
                        Brush brush = Brushes.Black;
                        if (i == 4 && (cells[i].Equals("High", StringComparison.OrdinalIgnoreCase) ||
                                       cells[i].Equals("Low", StringComparison.OrdinalIgnoreCase) ||
                                       cells[i].Equals("Critical", StringComparison.OrdinalIgnoreCase) ||
                                       cells[i].Equals("Abnormal", StringComparison.OrdinalIgnoreCase)))
                            brush = Brushes.DarkRed;
                        else if (i == 4 && cells[i].Equals("Normal", StringComparison.OrdinalIgnoreCase))
                            brush = Brushes.DarkGreen;
                        DrawText(g, cells[i], i == 1 ? bold : body, brush,
                            x + offsets[i] * w + 3, y + 5, widths[i] * w - 6, rowHeight - 9);
                    }
                    y += rowHeight;
                    g.DrawLine(line, x, y, x + w, y);
                    _nextRow++;
                }

                if (_nextRow == _results.Rows.Count)
                {
                    y += 14;
                    string notes = Field("ResultNotes");
                    if (notes != "-" && y + 48 < footerTop)
                    {
                        y = Section(g, "RESULT NOTES", x, y, w, section, line);
                        DrawText(g, notes, body, Brushes.Black, x, y, w, Math.Min(45, footerTop - y));
                        y += Math.Min(45, TextHeight(g, notes, body, w) + 6);
                    }
                    if (y + 58 < footerTop)
                    {
                        y = Section(g, "REPORT COMPLETION", x, y + 7, w, section, line);
                        Pair(g, "COMPLETED DATE", DateField("ResultedAt"), x, y, w * .48f, small, bold);
                        Pair(g, "STATUS", Field("RequestStatus"), x + w * .52f, y, w * .47f, small, bold);
                    }
                }

                g.DrawLine(line, x, footerTop, x + w, footerTop);
                DrawText(g, "Authorized Laboratory Signature", body, primary,
                    x, footerTop + 9, w * .52f, 19);
                DrawText(g, "________________________", body, Brushes.Black,
                    x, footerTop + 29, w * .5f, 20);
                DrawText(g, "Printed: " + DateTime.Now.ToString("dd MMM yyyy, hh:mm tt"),
                    small, Brushes.DimGray, x + w * .61f, footerTop + 11, w * .39f, 19);
                DrawText(g, "Page " + _pageNumber + "  |  End of Laboratory Report",
                    small, Brushes.DimGray, x + w * .61f, footerTop + 30, w * .39f, 19);
            }
            e.HasMorePages = _nextRow < _results.Rows.Count;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _printDocument.PrintPage -= PrintDocument_PrintPage;
                _printDocument.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
