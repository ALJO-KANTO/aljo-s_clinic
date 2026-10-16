using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Clinic_DataAccess;

namespace Clinic.Medical_Services.Laboratory
{
    internal static class LabUi
    {
        public static Label Label(string text) { return new Label { Text = text, AutoSize = true, Margin = new Padding(5, 10, 5, 5) }; }
        public static Button Button(string text) { return new Button { Text = text, Width = 145, Height = 36, BackColor = Color.FromArgb(0, 121, 107), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; }
        public static TableLayoutPanel Layout(Form f, string title, int rows)
        {
            f.Text = title; f.StartPosition = FormStartPosition.CenterParent; f.Size = new Size(520, 150 + rows * 54);
            f.MinimumSize = f.Size; f.Font = new Font("Segoe UI", 10);
            var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = rows, Padding = new Padding(16) };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            for (int i = 0; i < rows; i++) t.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            f.Controls.Add(t); return t;
        }
    }

    public class frmLabTests : Form
    {
        private DataGridView grid;
        private TextBox name;
        private NumericUpDown price;
        private CheckBox active;
        private int id;
        public frmLabTests()
        {
            Text = "Manage Lab Tests"; StartPosition = FormStartPosition.CenterParent;
            Size = new Size(780, 560); Font = new Font("Segoe UI", 10);
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 110, Padding = new Padding(10), AutoScroll = true };
            name = new TextBox { Width = 210 }; price = new NumericUpDown { Width = 100, Maximum = 1000000, DecimalPlaces = 2 };
            active = new CheckBox { Text = "Active", Checked = true, AutoSize = true };
            var save = LabUi.Button("Save Test"); var clear = LabUi.Button("New"); var disable = LabUi.Button("Deactivate");
            top.Controls.Add(LabUi.Label("Test Name")); top.Controls.Add(name);
            top.Controls.Add(LabUi.Label("Price")); top.Controls.Add(price); top.Controls.Add(active);
            top.Controls.Add(save); top.Controls.Add(clear); top.Controls.Add(disable);
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            Controls.Add(grid); Controls.Add(top);
            save.Click += (s, e) => { try { ClinicDatabase.SaveLabTest(id, name.Text, price.Value, active.Checked); Reset(); LoadRows(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            clear.Click += (s, e) => Reset();
            disable.Click += (s, e) => { if (id < 1) return; ClinicDatabase.DeactivateLabTest(id); Reset(); LoadRows(); };
            grid.SelectionChanged += (s, e) => {
                if (grid.CurrentRow == null || grid.CurrentRow.IsNewRow) return;
                id = Convert.ToInt32(grid.CurrentRow.Cells["TestID"].Value);
                name.Text = Convert.ToString(grid.CurrentRow.Cells["TestName"].Value);
                price.Value = Convert.ToDecimal(grid.CurrentRow.Cells["TestPrice"].Value);
                active.Checked = Convert.ToBoolean(grid.CurrentRow.Cells["IsActive"].Value);
            };
            Load += (s, e) => LoadRows();
        }
        private void Reset() { id = 0; name.Clear(); price.Value = 0; active.Checked = true; grid.ClearSelection(); }
        private void LoadRows() { grid.DataSource = ClinicDatabase.GetLabTests(); grid.ClearSelection(); }
    }

    public class frmLabRequest : Form
    {
        private NumericUpDown visit;
        private ComboBox test;
        public frmLabRequest()
        {
            var t = LabUi.Layout(this, "New Lab Request", 3);
            visit = new NumericUpDown { Minimum = 1, Maximum = int.MaxValue, Dock = DockStyle.Fill };
            test = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            var save = LabUi.Button("Create Request");
            t.Controls.Add(LabUi.Label("Visit ID"), 0, 0); t.Controls.Add(visit, 1, 0);
            t.Controls.Add(LabUi.Label("Lab Test"), 0, 1); t.Controls.Add(test, 1, 1);
            t.Controls.Add(save, 1, 2);
            Load += (s, e) => {
                DataTable dt = ClinicDatabase.GetLabTests();
                dt.DefaultView.RowFilter = "IsActive = 1";
                test.DataSource = dt.DefaultView; test.DisplayMember = "TestName"; test.ValueMember = "TestID";
            };
            save.Click += (s, e) => {
                try {
                    if (test.SelectedValue == null) throw new Exception("Choose an active test.");
                    ClinicDatabase.CreateLabRequest((int)visit.Value, Convert.ToInt32(test.SelectedValue));
                    DialogResult = DialogResult.OK;
                } catch (Exception ex) { MessageBox.Show(ex.Message, "Cannot create request"); }
            };
        }
    }

    public class frmLabResult : Form
    {
        public frmLabResult(int requestID)
        {
            var t = LabUi.Layout(this, "Enter Lab Result #" + requestID, 3);
            var result = new TextBox { Dock = DockStyle.Fill };
            var notes = new TextBox { Dock = DockStyle.Fill, Multiline = true };
            var save = LabUi.Button("Save Result");
            t.Controls.Add(LabUi.Label("Result"), 0, 0); t.Controls.Add(result, 1, 0);
            t.Controls.Add(LabUi.Label("Notes"), 0, 1); t.Controls.Add(notes, 1, 1);
            t.Controls.Add(save, 1, 2);
            save.Click += (s, e) => {
                try {
                    if (!ClinicDatabase.SaveLabResult(requestID, result.Text, notes.Text))
                        throw new Exception("Request was already completed or cancelled.");
                    DialogResult = DialogResult.OK;
                } catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
        }
    }
}
