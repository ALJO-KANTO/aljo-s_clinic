
using Clinic.ControlsMain;
using Clinic_Business;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Laboratory
{
    internal static class LabUi
    {
        // Laboratory-specific captions not necessarily included in TranslationClass.
        // Database values and identifiers are intentionally not translated.
        private static readonly System.Collections.Generic.Dictionary<string, string> LabArabic =
            new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Enter Lab Result", "إدخال نتيجة المختبر" },
                { "Lab Request", "طلب المختبر" },
                { "Laboratory Test", "الفحص المخبري" },
                { "Results entered", "النتائج المدخلة" },
                { "Validate Test Result", "اعتماد نتيجة الفحص" },
                { "Parameter", "المعلمة" },
                { "Result", "النتيجة" },
                { "Unit", "وحدة القياس" },
                { "Reference Range", "المدى المرجعي" },
                { "Flag", "التصنيف" },
                { "Remarks", "ملاحظات" },
                { "Cancel", "إلغاء" },
                { "Normal", "طبيعي" },
                { "Low", "منخفض" },
                { "High", "مرتفع" },
                { "Abnormal", "غير طبيعي" },
                { "Critical", "حرج" },
                { "Load Lab Result", "تحميل نتيجة المختبر" },
                { "New Lab Request", "طلب مختبر جديد" },
                { "Configure Parameters", "إعداد المعلمات" },
                { "Manage Lab Tests", "إدارة الفحوصات المخبرية" },
                { "Test Name", "اسم الفحص" },
                { "Test Price", "سعر الفحص" },
                { "Active", "نشط" },
                { "Save Test", "حفظ الفحص" },
                { "New", "جديد" },
                { "Deactivate", "إلغاء التفعيل" }
            };

        public static string T(string text)
        {
            if (!TranslationClass.IsArabic || string.IsNullOrEmpty(text))
                return text;

            string translated;
            if (LabArabic.TryGetValue(text.Trim(), out translated))
                return translated;

            return TranslationClass.Translate(text, true);
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
        private Button configureParameters;
        private int id;
        private bool loadingRows;

        public frmLabTests()
        {
            bool arabic = TranslationClass.IsArabic;

            Text = LabUi.T("Manage Lab Tests");
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1050, 620);
            MinimumSize = new Size(850, 480);
            Font = new Font("Segoe UI", 10);
            RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            RightToLeftLayout = arabic;

            // Two rows: existing test controls, then parameter configuration.
            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 135,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10, 8, 10, 4),
                BackColor = Color.FromArgb(248, 249, 251)
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            top.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            top.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

            var firstRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No
            };
            var secondRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No
            };

            name = new TextBox { Width = 210 };
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
                AutoSize = true
            };

            var save = LabUi.Button("Save Test");
            var clear = LabUi.Button("New");
            var disable = LabUi.Button("Deactivate");
            configureParameters = LabUi.Button(TranslationClass.IsArabic
                ? "إعداد معلمات الفحص" : "Configure Parameters");
            configureParameters.Width = 210;
            configureParameters.Enabled = false;

            firstRow.Controls.Add(LabUi.Label("Test Name"));
            firstRow.Controls.Add(name);
            firstRow.Controls.Add(LabUi.Label("Price"));
            firstRow.Controls.Add(price);
            firstRow.Controls.Add(active);
            firstRow.Controls.Add(save);
            firstRow.Controls.Add(clear);
            firstRow.Controls.Add(disable);

            // The new button is below the existing Add/New controls.
            secondRow.Controls.Add(configureParameters);
            top.Controls.Add(firstRow, 0, 0);
            top.Controls.Add(secondRow, 0, 1);

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No
            };

            Controls.Add(grid);
            Controls.Add(top);

            save.Click += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(name.Text))
                    {
                        MessageBox.Show(this, LabUi.T("Please enter a test name."),
                            LabUi.T("Validation"), MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        name.Focus();
                        return;
                    }

                    ClinicDatabase.SaveLabTest(id, name.Text.Trim(), price.Value, active.Checked);
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
                if (id < 1) return;
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

            configureParameters.Click += (s, e) => OpenConfiguration();
            grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) OpenConfiguration();
            };

            grid.SelectionChanged += (s, e) =>
            {
                if (loadingRows || grid.CurrentRow == null ||
                    grid.CurrentRow.IsNewRow || !grid.Columns.Contains("TestID"))
                    return;

                DataGridViewRow row = grid.CurrentRow;
                if (row.Cells["TestID"].Value == null ||
                    row.Cells["TestID"].Value == DBNull.Value)
                    return;

                id = Convert.ToInt32(row.Cells["TestID"].Value);
                name.Text = Convert.ToString(row.Cells["TestName"].Value);
                decimal selectedPrice = Convert.ToDecimal(row.Cells["TestPrice"].Value);
                price.Value = Math.Min(price.Maximum, Math.Max(price.Minimum, selectedPrice));
                active.Checked = Convert.ToBoolean(row.Cells["IsActive"].Value);
                configureParameters.Enabled = id > 0;
            };

            Load += (s, e) => LoadRows();
        }

        private void OpenConfiguration()
        {
            if (id <= 0)
            {
                MessageBox.Show(this,
                    LabUi.T("Select a saved laboratory test first."),
                    LabUi.T("Configure Parameters"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var form = new frmLabTestConfiguration(id))
                    form.ShowDialog(this);

                // Reload in case the configuration form or another operation changed data.
                LoadRows();
            }
            catch (Exception ex)
            {
                LabUi.ShowError(ex, "Configure Parameters");
            }
        }

        private void Reset()
        {
            id = 0;
            name.Clear();
            price.Value = 0;
            active.Checked = true;
            configureParameters.Enabled = false;
            loadingRows = true;
            try
            {
                grid.ClearSelection();
                grid.CurrentCell = null;
            }
            finally { loadingRows = false; }
        }

        private void LoadRows()
        {
            loadingRows = true;
            try
            {
                using (var connection = new MySqlConnection(
                    clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    using (var adapter = new MySqlDataAdapter(@"
                        SELECT TestID, TestName, TestPrice, IsActive
                        FROM laboratorytests
                        ORDER BY TestName;", connection))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        grid.DataSource = dt;
                    }
                }

                // Only these four columns are displayed. No Low/Medium/High columns.
                grid.Columns["TestID"].FillWeight = 55;
                grid.Columns["TestName"].FillWeight = 250;
                grid.Columns["TestPrice"].FillWeight = 85;
                grid.Columns["IsActive"].FillWeight = 70;
                grid.Columns["TestPrice"].DefaultCellStyle.Format = "N2";
                LabUi.TranslateGridHeaders(grid);
                grid.ClearSelection();
                grid.CurrentCell = null;
                id = 0;
                configureParameters.Enabled = false;
            }
            finally { loadingRows = false; }
        }
    }

    // ---------------------------------------------------------
    // ---------------------------------------------------------
    // NEW LAB REQUEST
    // ---------------------------------------------------------

    public enum LabRequestMode
    {
        DoctorVisit,
        Direct
    }

    public class frmLabRequest : Form
    {
        private readonly LabRequestMode _mode;
        private readonly int? _patientID;
        private readonly int? _visitID;
        private readonly int? _doctorID;
        private Label lblVisit;
        private TableLayoutPanel requestLayout;

        private ComboBox cmbPatient;
        private ComboBox cmbVisit;
        private TextBox txtSearchTest;
        private CheckedListBox lstTests;
        private Label lblSelected;
        private Button btnSave;
        private DataTable allTests;

        private readonly System.Collections.Generic.HashSet<int> selectedTestIDs =
            new System.Collections.Generic.HashSet<int>();
        private bool loadingTests;

        public frmLabRequest() : this(LabRequestMode.Direct) { }

        public frmLabRequest(
            LabRequestMode mode,
            int? patientID = null,
            int? visitID = null,
            int? doctorID = null)
        {
            _mode = mode;
            _patientID = patientID;
            _visitID = visitID;
            _doctorID = doctorID;

            bool arabic = TranslationClass.IsArabic;
            Text = LabUi.T("New Lab Request");
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(760, 590);
            MinimumSize = new Size(650, 500);
            Font = new Font("Segoe UI", 10);
            RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            RightToLeftLayout = arabic;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(18),
                RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            Controls.Add(table);
            requestLayout = table;

            cmbPatient = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbVisit = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            txtSearchTest = new TextBox
            {
                Dock = DockStyle.Fill
            };
            lstTests = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                IntegralHeight = false,
                BorderStyle = BorderStyle.FixedSingle
            };
            lblSelected = new Label
            {
                Dock = DockStyle.Fill,
                Text = LabUi.T("Selected Tests") + ": 0",
                TextAlign = arabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(0, 121, 107),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave = LabUi.Button("Create Requests");
            btnSave.Width = 190;

            table.Controls.Add(LabUi.Label("Patient"), 0, 0);
            table.Controls.Add(cmbPatient, 1, 0);
            lblVisit = LabUi.Label("Patient Visit");
            table.Controls.Add(lblVisit, 0, 1);
            table.Controls.Add(cmbVisit, 1, 1);
            table.Controls.Add(LabUi.Label("Search Lab Tests"), 0, 2);
            table.Controls.Add(txtSearchTest, 1, 2);
            table.Controls.Add(LabUi.Label("Lab Tests"), 0, 3);
            table.Controls.Add(lstTests, 1, 3);
            table.Controls.Add(lblSelected, 1, 4);
            table.Controls.Add(btnSave, 1, 5);

            Load += frmLabRequest_Load;
            cmbPatient.SelectedIndexChanged += cmbPatient_SelectedIndexChanged;
            txtSearchTest.TextChanged += (s, e) => RefreshTestList();
            lstTests.ItemCheck += lstTests_ItemCheck;
            btnSave.Click += btnSave_Click;

            if (_mode == LabRequestMode.Direct)
            {
                lblVisit.Visible = false;
                cmbVisit.Visible = false;
                requestLayout.RowStyles[1].SizeType = SizeType.Absolute;
                requestLayout.RowStyles[1].Height = 0;
            }
        }

        private void frmLabRequest_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable patients = ClinicDatabase.GetLabPatients();
                cmbPatient.DisplayMember = "PatientName";
                cmbPatient.ValueMember = "PatientID";
                cmbPatient.DataSource = patients;
                if (_patientID.HasValue)
                {
                    cmbPatient.SelectedValue = _patientID.Value;
                    if (!(cmbPatient.SelectedValue is int) ||
                        (int)cmbPatient.SelectedValue != _patientID.Value)
                        throw new InvalidOperationException("Selected patient not found.");
                    cmbPatient.Enabled = false;
                }

                allTests = ClinicDatabase.GetLabTests();
                RefreshTestList();
            }
            catch (Exception ex)
            {
                LabUi.ShowError(ex, "Load Lab Request");
            }
        }

        private void RefreshTestList()
        {
            if (allTests == null) return;
            loadingTests = true;
            lstTests.BeginUpdate();
            try
            {
                lstTests.Items.Clear();
                string search = txtSearchTest.Text.Trim();
                foreach (DataRow row in allTests.Rows)
                {
                    if (row["IsActive"] == DBNull.Value ||
                        !Convert.ToBoolean(row["IsActive"])) continue;

                    int id = Convert.ToInt32(row["TestID"]);
                    string name = Convert.ToString(row["TestName"]);
                    if (name.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    var item = new LabTestItem(id, name);
                    int index = lstTests.Items.Add(item);
                    lstTests.SetItemChecked(index, selectedTestIDs.Contains(id));
                }
            }
            finally
            {
                lstTests.EndUpdate();
                loadingTests = false;
                UpdateSelectedCount();
            }
        }

        private sealed class LabTestItem
        {
            public int ID { get; private set; }
            public string Name { get; private set; }
            public LabTestItem(int id, string name) { ID = id; Name = name; }
            public override string ToString() { return Name; }
        }
        private void lstTests_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (loadingTests) return;
            var item = lstTests.Items[e.Index] as LabTestItem;
            if (item == null) return;
            if (e.NewValue == CheckState.Checked)
                selectedTestIDs.Add(item.ID);
            else
                selectedTestIDs.Remove(item.ID);
            UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            lblSelected.Text = LabUi.T("Selected Tests") + ": " + selectedTestIDs.Count;
        }

        private void cmbPatient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mode == LabRequestMode.Direct) return;
            if (!(cmbPatient.SelectedValue is int))
            {
                cmbVisit.DataSource = null;
                return;
            }
            try
            {
                int patientID = Convert.ToInt32(cmbPatient.SelectedValue);
                DataTable visits = ClinicDatabase.GetLabPatientVisits(patientID);
                cmbVisit.DisplayMember = "VisitDisplay";
                cmbVisit.ValueMember = "VisitID";
                cmbVisit.DataSource = visits;
                if (_visitID.HasValue)
                {
                    cmbVisit.SelectedValue = _visitID.Value;
                    cmbVisit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LabUi.ShowError(ex, "Load Patient Visits");
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbPatient.SelectedValue == null ||
                cmbPatient.SelectedValue == DBNull.Value)
            {
                MessageBox.Show(LabUi.T("Please select a patient."));
                return;
            }

            if (selectedTestIDs.Count == 0)
            {
                MessageBox.Show(
                    LabUi.T("Please select at least one lab test."));
                return;
            }

            int patientID = Convert.ToInt32(cmbPatient.SelectedValue);

            int? visitID = null;
            int? doctorID = null;

            if (_mode == LabRequestMode.DoctorVisit)
            {
                visitID = _visitID;
                doctorID = _doctorID;

                if (!visitID.HasValue || visitID.Value <= 0)
                {
                    MessageBox.Show(
                        LabUi.T("Please select a valid patient visit."));
                    return;
                }
            }

            btnSave.Enabled = false;

            try
            {
                int orderID = ClinicDatabase.CreateLabOrder(
                    patientID: patientID,
                    visitID: visitID,
                    doctorID: doctorID,
                    requestSource:
                        _mode == LabRequestMode.Direct
                            ? "WalkIn"
                            : "Doctor",
                    testIDs: new System.Collections.Generic.List<int>(selectedTestIDs)
                );

                MessageBox.Show(
                    LabUi.T("Laboratory order created successfully.") +
                    "\nOrder ID: " + orderID,
                    LabUi.T("Success"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                LabUi.ShowError(ex, "Cannot Create Lab Request");
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }
    }

    // ---------------------------------------------------------
    // ENTER LAB RESULT
    // ---------------------------------------------------------



    public class frmLabResult : Form
    {
        private readonly int _requestID;

        private readonly DataGridView grid = new DataGridView();
        private readonly Label info = new Label();
        private readonly Label summary = new Label();
        private readonly Button save =
            LabUi.Button("Validate Test Result");

        private DataTable parameters;
        private int testID;
        private string patientGender = "";
        private string testName = "";
        private bool loading;

        public frmLabResult(int requestID)
        {
            _requestID = requestID;

            bool arabic = TranslationClass.IsArabic;

            Text = LabUi.T("Enter Lab Result") + " #" + requestID;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1180, 720);
            MinimumSize = new Size(850, 480);
            Font = new Font("Segoe UI", 10);
            RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;
            RightToLeftLayout = arabic;
            BackColor = Color.White;

            // ================= HEADER =================

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 112,
                Padding = new Padding(18, 12, 18, 6),
                BackColor = Color.FromArgb(245, 249, 250)
            };

            info.Dock = DockStyle.Top;
            info.Height = 48;
            info.Font = new Font(
                "Segoe UI", 12, FontStyle.Bold);
            info.ForeColor = Color.FromArgb(0, 110, 105);

            summary.Dock = DockStyle.Top;
            summary.Height = 32;
            summary.ForeColor = Color.DimGray;

            header.Controls.Add(summary);
            header.Controls.Add(info);

            // ================= GRID =================

            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoGenerateColumns = false;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 38;
            grid.ColumnHeadersHeight = 44;
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(0, 121, 107);

            grid.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            grid.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            grid.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(240, 248, 247);

            grid.SelectionMode =
                DataGridViewSelectionMode.CellSelect;

            grid.MultiSelect = false;

            grid.EditMode =
                DataGridViewEditMode.EditOnEnter;

            grid.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            AddTextColumn(
                "ParameterName", "Parameter", 220, true);

            AddTextColumn(
                "ObservedValue", "Result", 120, false);

            AddTextColumn(
                "Unit", "Unit", 80, true);

            AddTextColumn(
                "SelectedReferenceRange",
                "Reference Range", 240, true);

            var flag = new DataGridViewComboBoxColumn
            {
                Name = "ResultFlag",
                HeaderText = LabUi.T("Flag"),
                DataPropertyName = "ResultFlag",
                Width = 110,
                FlatStyle = FlatStyle.Flat
            };

            flag.Items.AddRange(
                "", "Low", "Normal", "High",
                "Abnormal", "Critical");

            grid.Columns.Add(flag);

            AddTextColumn(
                "ResultNotes", "Remarks", 190, false);

            grid.Columns["ParameterName"]
                .DefaultCellStyle.WrapMode =
                    DataGridViewTriState.True;

            grid.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            // ================= FOOTER =================

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 74,
                Padding = new Padding(15),
                FlowDirection = arabic
                    ? FlowDirection.LeftToRight
                    : FlowDirection.RightToLeft,
                BackColor = Color.FromArgb(245, 249, 250)
            };

            var cancel = LabUi.Button("Cancel");
            cancel.BackColor = Color.DimGray;
            cancel.Click += (s, e) => Close();

            save.Width = 205;
            save.Click += (s, e) => SaveAll();

            footer.Controls.Add(cancel);
            footer.Controls.Add(save);

            Controls.Add(grid);
            Controls.Add(footer);
            Controls.Add(header);

            // ================= EVENTS =================

            grid.CellEndEdit += (s, e) =>
            {
                if (!loading && e.RowIndex >= 0)
                {
                    Recalculate(e.RowIndex);
                    UpdateSummary();
                }
            };

            grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (grid.IsCurrentCellDirty)
                    grid.CommitEdit(
                        DataGridViewDataErrorContexts.Commit);
            };

            grid.DataError += (s, e) =>
            {
                e.ThrowException = false;
            };

            Load += (s, e) =>
            {
                try
                {
                    LoadResults();
                }
                catch (Exception ex)
                {
                    LabUi.ShowError(ex, "Load Lab Result");
                    save.Enabled = false;
                }
            };
        }

        // =====================================================
        // ADD GRID COLUMN
        // =====================================================

        private void AddTextColumn(
            string name,
            string title,
            int width,
            bool readOnly)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = name,
                HeaderText = LabUi.T(title),
                Width = width,
                MinimumWidth = 70,
                ReadOnly = readOnly,
                SortMode =
                    DataGridViewColumnSortMode.NotSortable
            });
        }

        private static string S(object value)
        {
            return value == null || value == DBNull.Value
                ? ""
                : Convert.ToString(value);
        }

        // =====================================================
        // LOAD REQUEST AND CONFIGURED PARAMETERS
        // =====================================================

        private void LoadResults()
        {
            loading = true;

            try
            {
                using (var con = new MySqlConnection(
                    clsDataAccessSettings.ConnectionString))
                {
                    con.Open();

                    // Get selected test and patient gender.

                    using (var cmd = new MySqlCommand(@"
    SELECT
        lr.TestID,
        lt.TestName,
        p.Gender
    FROM labrequests lr

    INNER JOIN laboratorytests lt
        ON lt.TestID = lr.TestID

    
LEFT JOIN visits v
    ON v.VisitID = lr.VisitID

LEFT JOIN patients pa
    ON pa.PatientID = COALESCE(
        lr.PatientID,
        v.PatientID
    )

LEFT JOIN persons pe
    ON pe.PersonID = pa.PersonID


    WHERE lr.LabRequestID = @id
    LIMIT 1;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", _requestID);

                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read())
                            {
                                throw new Exception(
                                    "Laboratory request not found.");
                            }

                            testID = Convert.ToInt32(r["TestID"]);
                            testName = S(r["TestName"]);
                            patientGender = S(r["Gender"]);
                        }
                    }

                    //using (var cmd = new MySqlCommand(@"
                    //SELECT
                    //    lr.TestID,
                    //    lt.TestName,
                    //    p.Gender
                    //FROM labrequests lr
                    //JOIN laboratorytests lt
                    //    ON lt.TestID = lr.TestID
                    //JOIN visits v
                    //    ON v.VisitID = lr.VisitID
                    //JOIN patients pa
                    //    ON pa.PatientID = v.PatientID
                    //JOIN persons p
                    //    ON p.PersonID = pa.PersonID
                    //WHERE lr.LabRequestID = @id
                    //LIMIT 1;", con))
                    //{
                    //    cmd.Parameters.AddWithValue(
                    //        "@id", _requestID);

                    //    using (var r = cmd.ExecuteReader())
                    //    {
                    //        if (!r.Read())
                    //            throw new Exception(
                    //                "Laboratory request not found.");

                    //        testID = Convert.ToInt32(r["TestID"]);
                    //        testName = S(r["TestName"]);
                    //        patientGender = S(r["Gender"]);
                    //    }
                    //}

                    info.Text =
                        LabUi.T("Lab Request") + " #" +
                        _requestID + "     " +
                        LabUi.T("Laboratory Test") +
                        ": " + testName;

                    // Load configured parameters and saved results.
                    // Gender: 1 = Male, 2 = Female.

                    using (var adapter = new MySqlDataAdapter(@"
                    SELECT
                        p.ParameterID,
                        p.ParameterName,
                        p.Unit,
                        p.ResultType,
                        p.LowLimit,
                        p.HighLimit,
                        p.ReferenceRange,
                        p.MaleReferenceRange,
                        p.FemaleReferenceRange,

                        CASE
                            WHEN @gender = '1'
                             AND NULLIF(
                                 TRIM(p.MaleReferenceRange), ''
                             ) IS NOT NULL
                            THEN p.MaleReferenceRange

                            WHEN @gender = '2'
                             AND NULLIF(
                                 TRIM(p.FemaleReferenceRange), ''
                             ) IS NOT NULL
                            THEN p.FemaleReferenceRange

                            ELSE COALESCE(
                                NULLIF(
                                    TRIM(p.ReferenceRange), ''
                                ), ''
                            )
                        END AS SelectedReferenceRange,

                        COALESCE(
                            e.ObservedValue, ''
                        ) AS ObservedValue,

                        COALESCE(
                            e.ResultFlag, ''
                        ) AS ResultFlag,

                        COALESCE(
                            e.ResultNotes, ''
                        ) AS ResultNotes

                    FROM laboratorytestparameters p

                    LEFT JOIN labparameterresultentries e
                        ON e.ParameterID = p.ParameterID
                       AND e.LabRequestID = @requestID

                    WHERE p.TestID = @testID
                      AND p.IsActive = 1

                    ORDER BY
                        p.DisplayOrder,
                        p.ParameterID;", con))
                    {
                        adapter.SelectCommand.Parameters
                            .AddWithValue(
                                "@gender", patientGender);

                        adapter.SelectCommand.Parameters
                            .AddWithValue(
                                "@requestID", _requestID);

                        adapter.SelectCommand.Parameters
                            .AddWithValue(
                                "@testID", testID);

                        parameters = new DataTable();
                        adapter.Fill(parameters);
                    }
                }

                if (parameters.Rows.Count == 0)
                {
                    throw new Exception(
                        "No active parameters configured for this test. " +
                        "Configure parameters first.");
                }

                grid.DataSource = parameters;

                for (int i = 0; i < parameters.Rows.Count; i++)
                {
                    Recalculate(i, false);
                }

                UpdateSummary();
            }
            finally
            {
                loading = false;
            }
        }

        // =====================================================
        // CHECK NUMERIC RESULT TYPE
        // =====================================================

        private static bool IsNumeric(string type)
        {
            type = (type ?? "").Trim().ToLowerInvariant();

            return type == "numeric"
                || type == "quantitative"
                || type == "number";
        }

        // =====================================================
        // PARSE REFERENCE RANGE (EXAMPLE: 25-110)
        // =====================================================

        private static bool TryInterval(
            string range,
            out decimal low,
            out decimal high)
        {
            low = high = 0;

            if (string.IsNullOrWhiteSpace(range))
                return false;

            var match =
                System.Text.RegularExpressions.Regex.Match(
                    range.Trim(),
                    @"^\s*(-?\d+(?:[.,]\d+)?)\s*[-–]\s*(-?\d+(?:[.,]\d+)?)\s*$");

            if (!match.Success)
                return false;

            return decimal.TryParse(
                    match.Groups[1].Value.Replace(',', '.'),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out low)

                && decimal.TryParse(
                    match.Groups[2].Value.Replace(',', '.'),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out high)

                && low <= high;
        }

        // =====================================================
        // AUTOMATIC LOW / NORMAL / HIGH CLASSIFICATION
        // =====================================================

        private void Recalculate(
            int index,
            bool overwrite = true)
        {
            DataRow row = parameters.Rows[index];

            string value =
                S(row["ObservedValue"]).Trim();

            if (value.Length == 0)
            {
                row["ResultFlag"] = "";
                return;
            }

            if (!IsNumeric(S(row["ResultType"])))
                return;

            decimal number;

            if (!decimal.TryParse(
                value.Replace(',', '.'),
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out number))
            {
                return;
            }

            decimal low;
            decimal high;

            if (!TryInterval(
                S(row["SelectedReferenceRange"]),
                out low,
                out high))
            {
                // Use configured numeric limits only when
                // the selected reference range is empty.

                if (!string.IsNullOrWhiteSpace(
                    S(row["SelectedReferenceRange"])))
                {
                    return;
                }

                if (row["LowLimit"] == DBNull.Value ||
                    row["HighLimit"] == DBNull.Value)
                {
                    return;
                }

                low = Convert.ToDecimal(row["LowLimit"]);
                high = Convert.ToDecimal(row["HighLimit"]);

                if (low > high)
                    return;
            }

            if (overwrite ||
                string.IsNullOrWhiteSpace(
                    S(row["ResultFlag"])))
            {
                row["ResultFlag"] =
                    number < low ? "Low" :
                    number > high ? "High" :
                    "Normal";
            }
        }

        // =====================================================
        // UPDATE ENTERED RESULT COUNT
        // =====================================================

        private void UpdateSummary()
        {
            if (parameters == null)
                return;

            int entered = 0;

            foreach (DataRow row in parameters.Rows)
            {
                if (!string.IsNullOrWhiteSpace(
                    S(row["ObservedValue"])))
                {
                    entered++;
                }
            }

            summary.Text =
                LabUi.T("Results entered") +
                ": " + entered +
                " / " + parameters.Rows.Count;
        }

        // =====================================================
        // SAVE ALL PARAMETER RESULTS
        // =====================================================

        private void SaveAll()
        {
            try
            {
                grid.EndEdit();

                if (parameters == null ||
                    parameters.Rows.Count == 0)
                {
                    return;
                }

                // Validate every parameter result.

                foreach (DataRow row in parameters.Rows)
                {
                    string value =
                        S(row["ObservedValue"]).Trim();

                    if (value.Length == 0)
                    {
                        MessageBox.Show(
                            LabUi.T("Enter a result for") +
                            " " + S(row["ParameterName"]));

                        return;
                    }

                    decimal numeric;

                    if (IsNumeric(S(row["ResultType"])) &&
                        !decimal.TryParse(
                            value.Replace(',', '.'),
                            System.Globalization.NumberStyles.Number,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out numeric))
                    {
                        MessageBox.Show(
                            LabUi.T("Invalid numeric result for") +
                            " " + S(row["ParameterName"]));

                        return;
                    }
                }

                save.Enabled = false;

                using (var con = new MySqlConnection(
                    clsDataAccessSettings.ConnectionString))
                {
                    con.Open();

                    using (var tx = con.BeginTransaction())
                    {
                        try
                        {
                            // Lock and verify the request.

                            using (var check = new MySqlCommand(
                                @"SELECT Status
                              FROM labrequests
                              WHERE LabRequestID = @id
                              FOR UPDATE;",
                                con, tx))
                            {
                                check.Parameters.AddWithValue(
                                    "@id", _requestID);

                                string status =
                                    Convert.ToString(
                                        check.ExecuteScalar());

                                if (string.IsNullOrEmpty(status) ||
                                    status.Equals(
                                        "Cancelled",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new Exception(
                                        "Request was not found or has been cancelled.");
                                }
                            }

                            // Save each parameter result.

                            foreach (DataRow row in parameters.Rows)
                            {
                                string value =
                                    S(row["ObservedValue"]).Trim();

                                decimal numeric = 0m;

                                bool numericOk =
                                    IsNumeric(S(row["ResultType"])) &&
                                    decimal.TryParse(
                                        value.Replace(',', '.'),
                                        System.Globalization.NumberStyles.Number,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        out numeric);

                                using (var cmd = new MySqlCommand(@"
                                INSERT INTO labparameterresultentries
                                (
                                    LabRequestID,
                                    ParameterID,
                                    ObservedValue,
                                    NumericValue,
                                    SavedParameterName,
                                    SavedUnit,
                                    SavedReferenceRange,
                                    ResultFlag,
                                    ResultNotes
                                )
                                VALUES
                                (
                                    @rid,
                                    @pid,
                                    @value,
                                    @num,
                                    @name,
                                    @unit,
                                    @range,
                                    @flag,
                                    @notes
                                )
                                ON DUPLICATE KEY UPDATE
                                    ObservedValue =
                                        VALUES(ObservedValue),
                                    NumericValue =
                                        VALUES(NumericValue),
                                    SavedParameterName =
                                        VALUES(SavedParameterName),
                                    SavedUnit =
                                        VALUES(SavedUnit),
                                    SavedReferenceRange =
                                        VALUES(SavedReferenceRange),
                                    ResultFlag =
                                        VALUES(ResultFlag),
                                    ResultNotes =
                                        VALUES(ResultNotes),
                                    EnteredAt =
                                        CURRENT_TIMESTAMP;",
                                    con, tx))
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@rid", _requestID);

                                    cmd.Parameters.AddWithValue(
                                        "@pid", row["ParameterID"]);

                                    cmd.Parameters.AddWithValue(
                                        "@value", value);

                                    cmd.Parameters.AddWithValue(
                                        "@num",
                                        numericOk
                                            ? (object)numeric
                                            : DBNull.Value);

                                    cmd.Parameters.AddWithValue(
                                        "@name",
                                        row["ParameterName"]);

                                    cmd.Parameters.AddWithValue(
                                        "@unit",
                                        S(row["Unit"]));

                                    cmd.Parameters.AddWithValue(
                                        "@range",
                                        S(row["SelectedReferenceRange"]));

                                    cmd.Parameters.AddWithValue(
                                        "@flag",
                                        S(row["ResultFlag"]));

                                    cmd.Parameters.AddWithValue(
                                        "@notes",
                                        S(row["ResultNotes"]));

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Update overall request status.

                            using (var cmd = new MySqlCommand(@"
                            UPDATE labrequests
                            SET
                                Status = 'Completed',
                                ResultedAt = NOW(),
                                ResultValue = @summary,
                                ResultNotes = @notes
                            WHERE LabRequestID = @id
                              AND Status <> 'Cancelled';",
                                con, tx))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@id", _requestID);

                                cmd.Parameters.AddWithValue(
                                    "@summary",
                                    "Parameter results: " +
                                    parameters.Rows.Count);

                                cmd.Parameters.AddWithValue(
                                    "@notes",
                                    "Validated parameter-wise results");

                                if (cmd.ExecuteNonQuery() != 1)
                                {
                                    throw new Exception(
                                        "Unable to complete lab request.");
                                }
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                LabUi.ShowError(ex, "Save Lab Results");
            }
            finally
            {
                save.Enabled = true;
            }
        }
    }

}
