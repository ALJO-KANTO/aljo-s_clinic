using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using Clinic_DataAccess;
using Clinic;

namespace Clinic.Medical_Services.Radiology
{
    internal static class XRayUi
    {
        public static string T(string text)
        {
            return TranslationClass.Translate(text, TranslationClass.IsArabic);
        }

        // Apply direction as one layout operation to reduce repeated redraws.
        public static void ApplyDirection(Control parent)
        {
            if (parent == null || parent.IsDisposed) return;

            SuspendLayoutTree(parent);
            try
            {
                ApplyDirectionCore(parent, TranslationClass.IsArabic);
                EnableDoubleBufferingTree(parent);
            }
            finally
            {
                ResumeLayoutTree(parent);
            }
        }

        private static void ApplyDirectionCore(Control parent, bool arabic)
        {
            parent.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;

            if (parent is Form form)
                form.RightToLeftLayout = arabic;

            if (parent is FlowLayoutPanel flow)
                flow.FlowDirection = arabic
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;

            if (parent is Label label)
                label.TextAlign = arabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;

            if (parent is DataGridView grid)
            {
                grid.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.DefaultCellStyle.Alignment = arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
            }

            foreach (Control child in parent.Controls)
                ApplyDirectionCore(child, arabic);
        }

        private static void SuspendLayoutTree(Control parent)
        {
            parent.SuspendLayout();
            foreach (Control child in parent.Controls)
                SuspendLayoutTree(child);
        }

        private static void ResumeLayoutTree(Control parent)
        {
            foreach (Control child in parent.Controls)
                ResumeLayoutTree(child);
            parent.ResumeLayout(false);
        }

        // WinForms DataGridView and nested panels can flicker while redrawing.
        // Use the protected DoubleBuffered property without changing control types.
        public static void EnableDoubleBufferingTree(Control parent)
        {
            if (parent == null || parent.IsDisposed) return;

            try
            {
                PropertyInfo property = typeof(Control).GetProperty(
                    "DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                if (property != null)
                    property.SetValue(parent, true, null);
            }
            catch
            {
                // Double buffering is an optional visual optimization.
            }

            foreach (Control child in parent.Controls)
                EnableDoubleBufferingTree(child);
        }

        private static string TranslateTitle(string title)
        {
            int marker = title.LastIndexOf(" #", StringComparison.Ordinal);
            if (marker >= 0)
                return T(title.Substring(0, marker)) + title.Substring(marker);

            return T(title);
        }

        public static TableLayoutPanel Layout(Form form, string title, int rows, int width = 610)
        {
            form.Text = TranslateTitle(title);
            form.StartPosition = FormStartPosition.CenterParent;
            form.Size = new Size(width, Math.Max(300, 110 + rows * 57));
            form.MinimumSize = form.Size;
            form.Font = new Font("Segoe UI", 10);
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                ColumnCount = 2,
                RowCount = rows,
                RightToLeft = TranslationClass.IsArabic ? RightToLeft.Yes : RightToLeft.No
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            for (int i = 0; i < rows; i++) table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
            form.Controls.Add(table);
            return table;
        }

        public static Label Label(string text) => new Label
        {
            Text = T(text),
            AutoSize = true,
            Margin = new Padding(4, 12, 4, 4),
            TextAlign = TranslationClass.IsArabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft
        };

        public static Button Button(string text) => new Button
        {
            Text = T(text),
            Height = 37,
            Width = 160,
            BackColor = Color.FromArgb(0, 121, 107),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            RightToLeft = TranslationClass.IsArabic ? RightToLeft.Yes : RightToLeft.No
        };

        public static void TranslateGridHeaders(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;
            string[] keys = { "XRayTestID", "TestName", "TestPrice", "IsActive" };
            string[] captions = { "Test ID", "Test Name", "Price", "Active" };
            for (int i = 0; i < keys.Length; i++)
                if (grid.Columns.Contains(keys[i])) grid.Columns[keys[i]].HeaderText = T(captions[i]);
        }
    }

    public class frmXRayTests : Form
    {
        private DataGridView grid;
        private TextBox txtName;
        private NumericUpDown numPrice;
        private CheckBox chkActive;
        private int selectedId;
        public frmXRayTests()
        {
            Text = XRayUi.T("Manage X-Ray Tests"); Size = new Size(820, 550);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);
            RightToLeft = TranslationClass.IsArabic ? RightToLeft.Yes : RightToLeft.No;
            RightToLeftLayout = TranslationClass.IsArabic;
            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 115,
                Padding = new Padding(10),
                AutoScroll = true,
                RightToLeft = TranslationClass.IsArabic ? RightToLeft.Yes : RightToLeft.No,
                FlowDirection = TranslationClass.IsArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight
            };
            txtName = new TextBox { Width = 210 };
            numPrice = new NumericUpDown { Width = 110, DecimalPlaces = 2, Maximum = 1000000 };
            chkActive = new CheckBox { Text = XRayUi.T("Active"), Checked = true, AutoSize = true };
            var save = XRayUi.Button("Save Test");
            var clear = XRayUi.Button("New");
            var disable = XRayUi.Button("Deactivate");
            top.Controls.AddRange(new Control[] { XRayUi.Label("Test Name"), txtName,
                XRayUi.Label("Price"), numPrice, chkActive, save, clear, disable });
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                MultiSelect = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RightToLeft = TranslationClass.IsArabic ? RightToLeft.Yes : RightToLeft.No
            };
            Controls.Add(grid); Controls.Add(top);
            save.Click += (s, e) => {
                try { ClinicDatabase.SaveXRayTest(selectedId, txtName.Text, numPrice.Value, chkActive.Checked); LoadRows(); Reset(); }
                catch (Exception ex) { MessageBox.Show(XRayUi.T(ex.Message)); }
            };
            clear.Click += (s, e) => Reset();
            disable.Click += (s, e) => {
                if (selectedId < 1) return;
                if (MessageBox.Show(XRayUi.T("Deactivate selected test?"), XRayUi.T("Confirm"), MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                try { ClinicDatabase.DeleteXRayTest(selectedId); LoadRows(); Reset(); }
                catch (Exception ex) { MessageBox.Show(XRayUi.T(ex.Message)); }
            };
            grid.SelectionChanged += (s, e) => {
                if (grid.CurrentRow == null || grid.CurrentRow.IsNewRow || grid.SelectedRows.Count == 0) return;
                selectedId = Convert.ToInt32(grid.CurrentRow.Cells["XRayTestID"].Value);
                txtName.Text = Convert.ToString(grid.CurrentRow.Cells["TestName"].Value);
                numPrice.Value = Convert.ToDecimal(grid.CurrentRow.Cells["TestPrice"].Value);
                chkActive.Checked = Convert.ToBoolean(grid.CurrentRow.Cells["IsActive"].Value);
            };
            XRayUi.EnableDoubleBufferingTree(this);
            Load += (s, e) =>
            {
                try
                {
                    LoadRows();
                    Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        XRayUi.T("Unable to load X-Ray tests") + Environment.NewLine + ex.Message,
                        XRayUi.T("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }
        private void LoadRows() { grid.DataSource = ClinicDatabase.GetXRayTests(); XRayUi.TranslateGridHeaders(grid); grid.ClearSelection(); grid.CurrentCell = null; }
        private void Reset() { selectedId = 0; txtName.Clear(); numPrice.Value = 0; chkActive.Checked = true; grid.ClearSelection(); grid.CurrentCell = null; }
    }

    public class frmXRayRequest : Form
    {
        private readonly int requestId;
        private ComboBox cmbVisit, cmbTest;
        private TextBox txtNotes;
        public frmXRayRequest(int id = 0)
        {
            XRayUi.Label(
    TranslationClass.Translate(
        "Patient / Visit",
        TranslationClass.IsArabic
    )
);
            XRayUi.Label(
   TranslationClass.Translate(
       "X-Ray Test",
       TranslationClass.IsArabic
   )
);
            XRayUi.Label(
         TranslationClass.Translate(
             "Clinical Notes",
             TranslationClass.IsArabic
         )
      );
            XRayUi.Label(
                TranslationClass.Translate(
                    "Create Request",
                    TranslationClass.IsArabic
                )
            );
            requestId = id;
            var t = XRayUi.Layout(this, id == 0 ? "New X-Ray Request" : "Edit X-Ray Request" + " #" + id, 5);
            cmbVisit = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTest = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            txtNotes = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };
            var save = XRayUi.Button(id == 0 ? "Create Request" : "Save Changes");
            var close = XRayUi.Button("Close");
            t.Controls.Add(XRayUi.Label("Patient / Visit"), 0, 0); t.Controls.Add(cmbVisit, 1, 0);
            t.Controls.Add(XRayUi.Label("X-Ray Test"), 0, 1); t.Controls.Add(cmbTest, 1, 1);
            t.Controls.Add(XRayUi.Label("Clinical Notes"), 0, 2); t.Controls.Add(txtNotes, 1, 2);
            t.Controls.Add(save, 1, 3); t.Controls.Add(close, 1, 4);
            Load += (s, e) => {
                try
                {
                    var visits = ClinicDatabase.GetXRayVisits();
                    cmbVisit.DataSource = visits; cmbVisit.DisplayMember = "VisitDisplay"; cmbVisit.ValueMember = "VisitID";
                    var tests = ClinicDatabase.GetXRayTests(true);
                    if (requestId > 0)
                    {
                        DataRow old = ClinicDatabase.GetXRayRequest(requestId);
                        if (old == null) throw new Exception(XRayUi.T("Request not found."));
                        int oldTest = Convert.ToInt32(old["XRayTestID"]);
                        if (tests.Select("XRayTestID=" + oldTest).Length == 0)
                        {
                            DataRow[] oldRows = ClinicDatabase.GetXRayTests().Select("XRayTestID=" + oldTest);
                            foreach (DataRow oldRow in oldRows) tests.ImportRow(oldRow);
                        }
                    }
                    cmbTest.DataSource = tests; cmbTest.DisplayMember = "TestName"; cmbTest.ValueMember = "XRayTestID";
                    if (requestId > 0)
                    {
                        var r = ClinicDatabase.GetXRayRequest(requestId);
                        cmbVisit.SelectedValue = Convert.ToInt32(r["VisitID"]);
                        cmbTest.SelectedValue = Convert.ToInt32(r["XRayTestID"]);
                        txtNotes.Text = Convert.ToString(r["ClinicalNotes"]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        XRayUi.T("Unable to load X-Ray request") + Environment.NewLine + ex.Message,
                        XRayUi.T("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Keep the form open so the loading error can be diagnosed.
                }
            };
            save.Click += (s, e) => {
                try
                {
                    if (cmbVisit.SelectedValue == null || cmbTest.SelectedValue == null) throw new Exception(XRayUi.T("Select a patient visit and test."));
                    int visit = Convert.ToInt32(cmbVisit.SelectedValue);
                    int test = Convert.ToInt32(cmbTest.SelectedValue);
                    if (requestId == 0) ClinicDatabase.CreateXRayRequest(visit, test, txtNotes.Text);
                    else if (!ClinicDatabase.UpdateXRayRequest(requestId, visit, test, txtNotes.Text))
                        throw new Exception(XRayUi.T("Only ordered requests can be updated."));
                    DialogResult = DialogResult.OK;
                }
                catch (Exception ex) { MessageBox.Show(XRayUi.T(ex.Message), XRayUi.T("Cannot save X-Ray request")); }
            };
            close.Click += (s, e) => Close();
            XRayUi.ApplyDirection(this);
            XRayUi.EnableDoubleBufferingTree(this);
        }
    }

    public class frmXRayResult : Form
    {
        private string imagePath;
        private readonly int requestId;
        private PictureBox preview;
        public frmXRayResult(int id)
        {
            requestId = id;
            var t = XRayUi.Layout(this, "Enter X-Ray Result #" + id, 6, 730);
            var findings = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };
            var impression = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };
            var imageButton = XRayUi.Button("Attach Image...");
            var start = XRayUi.Button("Mark In Progress");
            var save = XRayUi.Button("Complete Report");
            preview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            t.Controls.Add(XRayUi.Label("Findings"), 0, 0); t.Controls.Add(findings, 1, 0);
            t.Controls.Add(XRayUi.Label("Impression"), 0, 1); t.Controls.Add(impression, 1, 1);
            t.Controls.Add(imageButton, 1, 2);
            t.Controls.Add(preview, 1, 3);
            t.Controls.Add(start, 1, 4);
            t.Controls.Add(save, 1, 5);
            imageButton.Click += (s, e) => {
                using (var ofd = new OpenFileDialog { Filter = XRayUi.T("Images") + "|*.png;*.jpg;*.jpeg;*.bmp" })
                {
                    if (ofd.ShowDialog(this) != DialogResult.OK) return;
                    try
                    {
                        imagePath = ofd.FileName;
                        if (preview.Image != null) { preview.Image.Dispose(); preview.Image = null; }
                        preview.Image = frmXRay.LoadUnlockedImage(imagePath);
                    }
                    catch (Exception ex) { MessageBox.Show(XRayUi.T(ex.Message)); }
                }
            };
            start.Click += (s, e) => {
                try
                {
                    if (!ClinicDatabase.SetXRayInProgress(requestId)) MessageBox.Show(XRayUi.T("Request is not in Ordered status."));
                    else { MessageBox.Show(XRayUi.T("Marked In Progress.")); DialogResult = DialogResult.OK; }
                }
                catch (Exception ex) { MessageBox.Show(XRayUi.T(ex.Message)); }
            };
            save.Click += (s, e) => {
                try
                {
                    string stored = null;
                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Clinic", "XRayImages");
                        Directory.CreateDirectory(dir);
                        stored = Path.Combine(dir, Guid.NewGuid().ToString("N") + Path.GetExtension(imagePath).ToLowerInvariant());
                        File.Copy(imagePath, stored, false);
                    }
                    if (!ClinicDatabase.SaveXRayResult(requestId, findings.Text, impression.Text, stored))
                    {
                        if (stored != null && File.Exists(stored)) File.Delete(stored);
                        throw new Exception(XRayUi.T("Request is already completed or cancelled."));
                    }
                    DialogResult = DialogResult.OK;
                }
                catch (Exception ex) { MessageBox.Show(XRayUi.T(ex.Message), XRayUi.T("Cannot save result")); }
            };
            XRayUi.ApplyDirection(this);
            XRayUi.EnableDoubleBufferingTree(this);
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (preview != null && preview.Image != null) preview.Image.Dispose();
            base.OnFormClosed(e);
        }
    }
}
