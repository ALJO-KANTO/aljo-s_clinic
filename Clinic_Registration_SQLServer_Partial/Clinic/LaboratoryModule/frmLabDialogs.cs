
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Clinic_DataAccess;
using Clinic.ControlsMain;

namespace Clinic.Medical_Services.Laboratory
{
    internal static class LabUi
    {
        public static string T(string text)
        {
            return TranslationClass.Translate(
                text,
                TranslationClass.IsArabic
            );
        }

        public static Label Label(string text)
        {
            bool arabic = TranslationClass.IsArabic;

            return new Label
            {
                Text = T(text),
                AutoSize = true,
                Margin = new Padding(5, 10, 5, 5),
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No,
                TextAlign = arabic
                    ? ContentAlignment.MiddleRight
                    : ContentAlignment.MiddleLeft
            };
        }

        public static Button Button(string text)
        {
            bool arabic = TranslationClass.IsArabic;

            return new Button
            {
                Text = T(text),
                Width = 145,
                Height = 36,
                BackColor = Color.FromArgb(0, 121, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        public static void ApplyFormLanguage(Form form)
        {
            bool arabic = TranslationClass.IsArabic;

            form.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            form.RightToLeftLayout = arabic;

            ApplyControlLanguage(form);
        }

        private static void ApplyControlLanguage(Control parent)
        {
            bool arabic = TranslationClass.IsArabic;

            foreach (Control control in parent.Controls)
            {
                if (!string.IsNullOrWhiteSpace(control.Text))
                    control.Text = T(control.Text);

                control.RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control is Label label)
                {
                    label.TextAlign = arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }

                if (control is Button button)
                {
                    button.TextAlign = ContentAlignment.MiddleCenter;
                }

                if (control.HasChildren)
                    ApplyControlLanguage(control);
            }
        }

        public static TableLayoutPanel Layout(
            Form form, string title, int rows)
        {
            bool arabic = TranslationClass.IsArabic;

            form.Text = T(title);
            form.StartPosition = FormStartPosition.CenterParent;
            form.Size = new Size(520, 150 + rows * 54);
            form.MinimumSize = form.Size;
            form.Font = new Font("Segoe UI", 10);

            form.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            form.RightToLeftLayout = arabic;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = rows,
                Padding = new Padding(16),
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 35));

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 65));

            for (int i = 0; i < rows; i++)
            {
                table.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 50));
            }

            form.Controls.Add(table);

            return table;
        }

        public static void TranslateGridHeaders(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                {
                    column.HeaderText = T(column.HeaderText);
                }
            }

            bool arabic = TranslationClass.IsArabic;

            grid.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            grid.ColumnHeadersDefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleCenter
                : DataGridViewContentAlignment.MiddleLeft;

            grid.DefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;
        }

        public static void ShowError(Exception ex, string caption)
        {
            MessageBox.Show(
                T(ex.Message),
                T(caption),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    // ---------------------------------------------------------
    // MANAGE LAB TESTS
    // ---------------------------------------------------------

    public class frmLabTests : Form
    {
        private DataGridView grid;
        private TextBox name;
        private NumericUpDown price;
        private CheckBox active;
        private int id;

        public frmLabTests()
        {
            bool arabic = TranslationClass.IsArabic;

            Text = LabUi.T("Manage Lab Tests");
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(780, 560);
            Font = new Font("Segoe UI", 10);

            RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            RightToLeftLayout = arabic;

            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 110,
                Padding = new Padding(10),
                AutoScroll = true,
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            name = new TextBox
            {
                Width = 210
            };

            price = new NumericUpDown
            {
                Width = 100,
                Maximum = 1000000,
                DecimalPlaces = 2
            };

            active = new CheckBox
            {
                Text = LabUi.T("Active"),
                Checked = true,
                AutoSize = true,
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            var save = LabUi.Button("Save Test");
            var clear = LabUi.Button("New");
            var disable = LabUi.Button("Deactivate");

            top.Controls.Add(LabUi.Label("Test Name"));
            top.Controls.Add(name);
            top.Controls.Add(LabUi.Label("Price"));
            top.Controls.Add(price);
            top.Controls.Add(active);
            top.Controls.Add(save);
            top.Controls.Add(clear);
            top.Controls.Add(disable);

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            Controls.Add(grid);
            Controls.Add(top);

            save.Click += (s, e) =>
            {
                try
                {
                    ClinicDatabase.SaveLabTest(
                        id,
                        name.Text,
                        price.Value,
                        active.Checked
                    );

                    Reset();
                    LoadRows();
                }
                catch (Exception ex)
                {
                    LabUi.ShowError(ex, "Save Test");
                }
            };

            clear.Click += (s, e) => Reset();

            disable.Click += (s, e) =>
            {
                if (id < 1)
                    return;

                try
                {
                    ClinicDatabase.DeactivateLabTest(id);
                    Reset();
                    LoadRows();
                }
                catch (Exception ex)
                {
                    LabUi.ShowError(ex, "Deactivate");
                }
            };

            grid.SelectionChanged += (s, e) =>
            {
                if (grid.CurrentRow == null ||
                    grid.CurrentRow.IsNewRow)
                    return;

                if (!grid.Columns.Contains("TestID") ||
                    !grid.Columns.Contains("TestName") ||
                    !grid.Columns.Contains("TestPrice") ||
                    !grid.Columns.Contains("IsActive"))
                    return;

                id = Convert.ToInt32(
                    grid.CurrentRow.Cells["TestID"].Value);

                name.Text = Convert.ToString(
                    grid.CurrentRow.Cells["TestName"].Value);

                price.Value = Convert.ToDecimal(
                    grid.CurrentRow.Cells["TestPrice"].Value);

                active.Checked = Convert.ToBoolean(
                    grid.CurrentRow.Cells["IsActive"].Value);
            };

            Load += (s, e) => LoadRows();
        }

        private void Reset()
        {
            id = 0;
            name.Clear();
            price.Value = 0;
            active.Checked = true;
            grid.ClearSelection();
        }

        private void LoadRows()
        {
            grid.DataSource = ClinicDatabase.GetLabTests();

            LabUi.TranslateGridHeaders(grid);

            grid.ClearSelection();
        }
    }

    // ---------------------------------------------------------
    // NEW LAB REQUEST
    // ---------------------------------------------------------

    public class frmLabRequest : Form
    {
        private ComboBox cmbPatient;
        private ComboBox cmbVisit;
        private ComboBox cmbTest;

        public frmLabRequest()
        {
            var table = LabUi.Layout(
                this,
                "New Lab Request",
                4
            );

            Size = new Size(620, 390);
            MinimumSize = Size;

            bool arabic = TranslationClass.IsArabic;

            cmbPatient = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            cmbVisit = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            cmbTest = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            var btnSave = LabUi.Button("Create Request");

            table.Controls.Add(
                LabUi.Label("Patient"), 0, 0);

            table.Controls.Add(cmbPatient, 1, 0);

            table.Controls.Add(
                LabUi.Label("Patient Visit"), 0, 1);

            table.Controls.Add(cmbVisit, 1, 1);

            table.Controls.Add(
                LabUi.Label("Lab Test"), 0, 2);

            table.Controls.Add(cmbTest, 1, 2);

            table.Controls.Add(btnSave, 1, 3);

            Load += frmLabRequest_Load;

            cmbPatient.SelectedIndexChanged +=
                cmbPatient_SelectedIndexChanged;

            btnSave.Click += btnSave_Click;
        }

        private void frmLabRequest_Load(
            object sender, EventArgs e)
        {
            try
            {
                DataTable patients =
                    ClinicDatabase.GetLabPatients();

                cmbPatient.DisplayMember = "PatientName";
                cmbPatient.ValueMember = "PatientID";
                cmbPatient.DataSource = patients;

                DataTable tests =
                    ClinicDatabase.GetLabTests();

                tests.DefaultView.RowFilter = "IsActive = 1";

                cmbTest.DisplayMember = "TestName";
                cmbTest.ValueMember = "TestID";
                cmbTest.DataSource = tests.DefaultView;
            }
            catch (Exception ex)
            {
                LabUi.ShowError(ex, "Load Lab Request");
            }
        }

        private void cmbPatient_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (!(cmbPatient.SelectedValue is int))
            {
                cmbVisit.DataSource = null;
                return;
            }

            try
            {
                int patientID = Convert.ToInt32(
                    cmbPatient.SelectedValue);

                DataTable visits =
                    ClinicDatabase.GetLabPatientVisits(patientID);

                cmbVisit.DisplayMember = "VisitDisplay";
                cmbVisit.ValueMember = "VisitID";
                cmbVisit.DataSource = visits;
            }
            catch (Exception ex)
            {
                LabUi.ShowError(ex, "Load Patient Visits");
            }
        }

        private void btnSave_Click(
            object sender, EventArgs e)
        {
            try
            {
                if (!(cmbPatient.SelectedValue is int))
                {
                    MessageBox.Show(
                        LabUi.T("Please select a patient."),
                        LabUi.T("Validation"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!(cmbVisit.SelectedValue is int))
                {
                    MessageBox.Show(
                        LabUi.T(
                            "This patient has no visit. Create a visit first."),
                        LabUi.T("Validation"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!(cmbTest.SelectedValue is int))
                {
                    MessageBox.Show(
                        LabUi.T("Please select a lab test."),
                        LabUi.T("Validation"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                int visitID = Convert.ToInt32(
                    cmbVisit.SelectedValue);

                int testID = Convert.ToInt32(
                    cmbTest.SelectedValue);

                int requestID =
                    ClinicDatabase.CreateLabRequest(
                        visitID,
                        testID
                    );

                MessageBox.Show(
                    LabUi.T("Lab Request") + " #" +
                    requestID + " " +
                    LabUi.T("created successfully."),
                    LabUi.T("Success"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                LabUi.ShowError(
                    ex,
                    "Cannot Create Lab Request"
                );
            }
        }
    }

    // ---------------------------------------------------------
    // ENTER LAB RESULT
    // ---------------------------------------------------------

    public class frmLabResult : Form
    {
        public frmLabResult(int requestID)
        {
            var table = LabUi.Layout(
                this,
                "Enter Lab Result",
                3
            );

            Text = LabUi.T("Enter Lab Result") +
                   " #" + requestID;

            var result = new TextBox
            {
                Dock = DockStyle.Fill,
                RightToLeft = TranslationClass.IsArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            var notes = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                RightToLeft = TranslationClass.IsArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No
            };

            var save = LabUi.Button("Save Result");

            table.Controls.Add(
                LabUi.Label("Result"), 0, 0);

            table.Controls.Add(result, 1, 0);

            table.Controls.Add(
                LabUi.Label("Notes"), 0, 1);

            table.Controls.Add(notes, 1, 1);

            table.Controls.Add(save, 1, 2);

            save.Click += (s, e) =>
            {
                try
                {
                    if (!ClinicDatabase.SaveLabResult(
                        requestID,
                        result.Text,
                        notes.Text))
                    {
                        throw new Exception(
                            "Request was already completed or cancelled.");
                    }

                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    LabUi.ShowError(ex, "Save Result");
                }
            };
        }
    }
}
