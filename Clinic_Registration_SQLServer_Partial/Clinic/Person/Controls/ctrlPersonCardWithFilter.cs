
using Clinic;
using Clinic.Person;
using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Controls
{
    public partial class ctrlPersonCardWithFilter : UserControl
    {
        public event Action<int> OnPersonSelected;
        private DataGridView dgvPersonSuggestions;
        private DataTable _personsTable;
        private bool _selectingPerson = false;
        private bool _FilterEnabled = false;
        private bool _btnAddNewEnabled = false;
        private bool _loadingPerson = false;
        private bool _personSelectionValid = false;

        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();

            ConfigureNameSearch();
            ApplyArabicTranslation();
        }

        private string Tr(string text)
        {
            return TranslationClass.Translate(
                text, TranslationClass.IsArabic);
        }

        public int PersonID
        {
            get { return _personSelectionValid ? ctrlPersonCard1.PersonID : -1; }
        }

        public clsPerson SelectedPerson
        {
            get { return ctrlPersonCard1.SelectedPersonInfo; }
        }

        public bool FilterEnabled
        {
            get { return _FilterEnabled; }
            set
            {
                _FilterEnabled = value;
                gbFilters.Enabled = value;
                if (!value && dgvPersonSuggestions != null)
                    dgvPersonSuggestions.Visible = false;
            }
        }

        public bool btnAddNewEnabled
        {
            get { return _btnAddNewEnabled; }
            set
            {
                _btnAddNewEnabled = value;
                btnAddNewPerson.Enabled = value;
            }
        }

        private void ConfigureNameSearchold()
        {
            // Allow letters, spaces, and other name characters.
            txtFilterValue.KeyPress -= txtFilterValue_KeyPress;

            txtFilterValue.Validating -=
                txtFilterValue_Validating;

            txtFilterValue.Validating +=
                txtFilterValue_Validating;

            txtFilterValue.TextChanged += (s, e) =>
            {
                if (!_loadingPerson &&
                    FilterEnabled &&
                    ctrlPersonCard1.PersonID > 0)
                {
                    // The existing person remains selected until
                    // the user explicitly selects another person.
                }
            };

            txtFilterValue.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && FilterEnabled)
                {
                    e.SuppressKeyPress = true;
                    FilterPersonSuggestions();
                }
            };

            // Update the existing label in the Filter group.
            foreach (Control control in gbFilters.Controls)
            {
                if (control is Label &&
                    control.Text.IndexOf(
                        "Person ID",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    control.Text = "Find By ID / Name:";
                    control.AutoSize = true;
                }
            }
        }

        private void ApplyArabicTranslation()
        {
            bool isArabic = TranslationClass.IsArabic;

            TranslateStaticControls(this);

            RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            if (ctrlPersonCard1 != null)
            {
                ctrlPersonCard1.RightToLeft = RightToLeft;

                ApplyRightToLeftToChildren(
                    ctrlPersonCard1, isArabic);
            }
        }

        private void ApplyRightToLeftToChildren(
            Control parent, bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                control.RightToLeft = isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control.HasChildren)
                    ApplyRightToLeftToChildren(
                        control, isArabic);
            }
        }

        private void TranslateStaticControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label ||
                    control is Button ||
                    control is GroupBox ||
                    control is TabPage ||
                    control is CheckBox ||
                    control is RadioButton ||
                    control is LinkLabel)
                {
                    if (!string.IsNullOrWhiteSpace(control.Text))
                        control.Text = Tr(control.Text);
                }

                if (control.HasChildren)
                    TranslateStaticControls(control);
            }
        }

        private void txtFilterValue_KeyPress(
            object sender, KeyPressEventArgs e)
        {
            // Name search accepts all normal text characters.
            e.Handled = false;
        }

        private void txtFilterValue_Validating(
            object sender, CancelEventArgs e)
        {
            // Do not block the user when opening the list
            // or when using the Add New Person button.
            errorProvider1.SetError(txtFilterValue, null);
        }
        private void txtPId_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            e.Handled =
                !char.IsDigit(e.KeyChar) &&
                !char.IsControl(e.KeyChar);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            // Person ID takes priority when entered.
            if (!string.IsNullOrWhiteSpace(txtPId.Text))
            {
                SearchByPersonID();
                return;
            }

            FilterPersonSuggestions();
            txtFilterValue.Focus();
        }

        private void SearchByPersonID()
        {
            if (!FilterEnabled) return;

            int personID;
            if (!int.TryParse(txtPId.Text.Trim(), out personID) || personID <= 0)
            {
                MessageBox.Show(Tr("Enter a valid Person ID."), Tr("Search"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPId.Focus();
                return;
            }

            try
            {
                clsPerson person = clsPerson.Find(personID);
                if (person == null)
                {
                    MessageBox.Show(Tr("Person not found."), Tr("Search"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _personSelectionValid = false;
                    return;
                }

                LoadPersonInfo(personID);
                if (dgvPersonSuggestions != null)
                    dgvPersonSuggestions.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Tr("Search Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string ReadValue(
            DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) ||
                row[column] == DBNull.Value)
                return "";

            return Convert.ToString(row[column]);
        }

        //private void ShowPersonSearch()
        //{
        //    if (!FilterEnabled)
        //        return;

        //    try
        //    {
        //        // Existing Clinic_Business method.
        //        DataTable source = clsPerson.GetAllPersons();

        //        if (source == null ||
        //            !source.Columns.Contains("PersonID") ||
        //            !source.Columns.Contains("FullName"))
        //        {
        //            MessageBox.Show(
        //                "Person list must contain PersonID and FullName.",
        //                Tr("Search"),
        //                MessageBoxButtons.OK,
        //                MessageBoxIcon.Warning);
        //            return;
        //        }

        //        using (Form searchForm = new Form())
        //        {
        //            searchForm.Text = Tr("Select Person");
        //            searchForm.StartPosition =
        //                FormStartPosition.CenterParent;
        //            searchForm.Size = new Size(950, 600);
        //            searchForm.MinimumSize = new Size(650, 420);
        //            searchForm.BackColor = Color.White;
        //            searchForm.Font =
        //                new Font("Segoe UI", 10);
        //            searchForm.RightToLeft =
        //                TranslationClass.IsArabic
        //                    ? RightToLeft.Yes
        //                    : RightToLeft.No;
        //            searchForm.RightToLeftLayout =
        //                TranslationClass.IsArabic;

        //            var layout = new TableLayoutPanel
        //            {
        //                Dock = DockStyle.Fill,
        //                ColumnCount = 1,
        //                RowCount = 3,
        //                Padding = new Padding(12)
        //            };

        //            layout.RowStyles.Add(
        //                new RowStyle(SizeType.Absolute, 55));
        //            layout.RowStyles.Add(
        //                new RowStyle(SizeType.Percent, 100));
        //            layout.RowStyles.Add(
        //                new RowStyle(SizeType.Absolute, 55));

        //            searchForm.Controls.Add(layout);

        //            var searchPanel = new TableLayoutPanel
        //            {
        //                Dock = DockStyle.Fill,
        //                ColumnCount = 2
        //            };

        //            searchPanel.ColumnStyles.Add(
        //                new ColumnStyle(
        //                    SizeType.Absolute, 125));
        //            searchPanel.ColumnStyles.Add(
        //                new ColumnStyle(
        //                    SizeType.Percent, 100));

        //            var searchLabel = new Label
        //            {
        //                Text = Tr("Search Name:"),
        //                Dock = DockStyle.Fill,
        //                TextAlign =
        //                    ContentAlignment.MiddleLeft
        //            };

        //            var searchBox = new TextBox
        //            {
        //                Dock = DockStyle.Fill,
        //                Font = new Font("Segoe UI", 11),
        //                Text = txtFilterValue.Text.Trim()
        //            };

        //            searchPanel.Controls.Add(searchLabel, 0, 0);
        //            searchPanel.Controls.Add(searchBox, 1, 0);
        //            layout.Controls.Add(searchPanel, 0, 0);

        //            // Build the list without changing database data.
        //            DataTable display = new DataTable();
        //            display.Columns.Add(
        //                "PersonID", typeof(int));
        //            display.Columns.Add(
        //                "FullName", typeof(string));
        //            display.Columns.Add(
        //                "Phone", typeof(string));
        //            display.Columns.Add(
        //                "Gender", typeof(string));
        //            display.Columns.Add(
        //                "DateOfBirth", typeof(string));

        //            foreach (DataRow row in source.Rows)
        //            {
        //                int personID;

        //                if (!int.TryParse(
        //                    ReadValue(row, "PersonID"),
        //                    out personID))
        //                    continue;

        //                display.Rows.Add(
        //                    personID,
        //                    ReadValue(row, "FullName"),
        //                    ReadValue(row, "Phone"),
        //                    ReadValue(row, "Gender"),
        //                    ReadValue(row, "DateOfBirth"));
        //            }

        //            DataView view = new DataView(display);

        //            var grid = new DataGridView
        //            {
        //                Dock = DockStyle.Fill,
        //                DataSource = view,
        //                ReadOnly = true,
        //                AllowUserToAddRows = false,
        //                AllowUserToDeleteRows = false,
        //                MultiSelect = false,
        //                SelectionMode =
        //                    DataGridViewSelectionMode.FullRowSelect,
        //                AutoSizeColumnsMode =
        //                    DataGridViewAutoSizeColumnsMode.Fill,
        //                RowHeadersVisible = false,
        //                BackgroundColor = Color.White,
        //                BorderStyle = BorderStyle.None,
        //                RowTemplate = { Height = 36 },
        //                ColumnHeadersHeight = 42,
        //                EnableHeadersVisualStyles = false
        //            };

        //            grid.ColumnHeadersDefaultCellStyle.BackColor =
        //                Color.FromArgb(0, 120, 107);
        //            grid.ColumnHeadersDefaultCellStyle.ForeColor =
        //                Color.White;
        //            grid.ColumnHeadersDefaultCellStyle.Font =
        //                new Font(
        //                    "Segoe UI", 10, FontStyle.Bold);

        //            grid.AlternatingRowsDefaultCellStyle.BackColor =
        //                Color.FromArgb(240, 249, 248);

        //            layout.Controls.Add(grid, 0, 1);

        //            var footer = new FlowLayoutPanel
        //            {
        //                Dock = DockStyle.Fill,
        //                FlowDirection =
        //                    FlowDirection.RightToLeft
        //            };

        //            var btnSelect = new Button
        //            {
        //                Text = Tr("Select Person"),
        //                Width = 160,
        //                Height = 40,
        //                BackColor =
        //                    Color.FromArgb(0, 120, 107),
        //                ForeColor = Color.White,
        //                FlatStyle = FlatStyle.Flat
        //            };

        //            var btnCancel = new Button
        //            {
        //                Text = Tr("Cancel"),
        //                Width = 110,
        //                Height = 40,
        //                DialogResult = DialogResult.Cancel
        //            };

        //            footer.Controls.Add(btnCancel);
        //            footer.Controls.Add(btnSelect);
        //            layout.Controls.Add(footer, 0, 2);

        //            Action applyFilter = () =>
        //            {
        //                string term = searchBox.Text.Trim();

        //                if (term.Length == 0)
        //                {
        //                    view.RowFilter = "";
        //                }
        //                else
        //                {
        //                    string escaped = term
        //                        .Replace("'", "''")
        //                        .Replace("[", "[[]")
        //                        .Replace("%", "[%]")
        //                        .Replace("*", "[*]");

        //                    view.RowFilter =
        //                        "FullName LIKE '%" +
        //                        escaped + "%'";
        //                }
        //            };

        //            searchBox.TextChanged +=
        //                (s, e) => applyFilter();

        //            Action selectPerson = () =>
        //            {
        //                if (grid.CurrentRow == null)
        //                    return;

        //                DataRowView selected =
        //                    grid.CurrentRow.DataBoundItem
        //                        as DataRowView;

        //                if (selected == null)
        //                    return;

        //                int selectedID =
        //                    Convert.ToInt32(
        //                        selected["PersonID"]);

        //                // Existing patients cannot be
        //                // registered a second time.
        //                if (clsPatient.IsPatientExistForPersonID(
        //                    selectedID))
        //                {
        //                    MessageBox.Show(
        //                        Tr("Selected Person already has a patient record, choose another one."),
        //                        Tr("Select another Person"),
        //                        MessageBoxButtons.OK,
        //                        MessageBoxIcon.Warning);
        //                    return;
        //                }

        //                LoadPersonInfo(selectedID);

        //                searchForm.DialogResult =
        //                    DialogResult.OK;
        //                searchForm.Close();
        //            };

        //            btnSelect.Click +=
        //                (s, e) => selectPerson();

        //            grid.CellDoubleClick +=
        //                (s, e) =>
        //                {
        //                    if (e.RowIndex >= 0)
        //                        selectPerson();
        //                };

        //            searchBox.KeyDown += (s, e) =>
        //            {
        //                if (e.KeyCode == Keys.Enter)
        //                {
        //                    e.SuppressKeyPress = true;
        //                    selectPerson();
        //                }
        //            };

        //            searchForm.AcceptButton = btnSelect;
        //            searchForm.CancelButton = btnCancel;

        //            applyFilter();

        //            searchForm.Shown +=
        //                (s, e) => searchBox.Focus();

        //            searchForm.ShowDialog(this.FindForm());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(
        //            ex.Message,
        //            Tr("Search Error"),
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Error);
        //    }
        //}

        private void _LoadPersonInfo()
        {
            if (!string.IsNullOrWhiteSpace(txtPId.Text))
                SearchByPersonID();
            else
                FilterPersonSuggestions();
        }

        private void btnAddNewPerson_Click(
            object sender, EventArgs e)
        {
            frmAddUpdatePerson frmAdd =
                new frmAddUpdatePerson();

            frmAdd.DataBack += _DataBack;
            frmAdd.ShowDialog();
        }
        private void ConfigureNameSearch()
        {
            txtFilterValue.KeyPress -= txtFilterValue_KeyPress;
            txtFilterValue.Validating -= txtFilterValue_Validating;

            txtFilterValue.Validating += txtFilterValue_Validating;

            // Initialize unconditionally: this runs before the textbox receives focus.
            // Keep the existing Person ID textbox and name textbox side by side.
            txtPId.Visible = true;
            txtPId.Enabled = true;
            txtPId.KeyPress -= txtPId_KeyPress;
            txtPId.KeyPress += txtPId_KeyPress;
            txtPId.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SearchByPersonID();
                }
            };
            txtPId.TextChanged += (s, e) =>
            {
                if (!_loadingPerson && !_selectingPerson)
                {
                    _personSelectionValid = false;
                    if (dgvPersonSuggestions != null)
                        dgvPersonSuggestions.Visible = false;
                }
            };
            txtFilterValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // Change the label.
            foreach (Control control in gbFilters.Controls)
            {
                if (control is Label &&
                    control.Text.IndexOf(
                        "Person ID",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    control.Text = "Find By ID / Name:";
                    control.AutoSize = true;
                }
            }

            // Create suggestion grid.
            dgvPersonSuggestions = new DataGridView
            {
                Name = "dgvPersonSuggestions",
                Visible = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 34,
                RowTemplate = { Height = 30 },
                EnableHeadersVisualStyles = false
            };

            dgvPersonSuggestions.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(0, 120, 107);

            dgvPersonSuggestions.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvPersonSuggestions.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvPersonSuggestions.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SelectSuggestedPerson();
            };

            dgvPersonSuggestions.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SelectSuggestedPerson();
            };

            // Add to the user control, not the filter GroupBox,
            // so the dropdown can extend below the filter area.
            this.Controls.Add(dgvPersonSuggestions);
            dgvPersonSuggestions.BringToFront();

            txtFilterValue.TextChanged += (s, e) =>
            {
                if (!_loadingPerson && !_selectingPerson)
                {
                    // Typed text is not a confirmed person selection.
                    _personSelectionValid = false;
                    FilterPersonSuggestions();
                }
            };

            txtFilterValue.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Down &&
                    dgvPersonSuggestions.Visible)
                {
                    dgvPersonSuggestions.Focus();

                    if (dgvPersonSuggestions.Rows.Count > 0)
                        dgvPersonSuggestions.Rows[0].Selected = true;

                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;

                    if (dgvPersonSuggestions.Visible &&
                        dgvPersonSuggestions.Rows.Count > 0)
                    {
                        SelectSuggestedPerson();
                    }
                    else
                    {
                        FilterPersonSuggestions();
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    dgvPersonSuggestions.Visible = false;
                }
            };

            dgvPersonSuggestions.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SelectSuggestedPerson();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    dgvPersonSuggestions.Visible = false;
                    txtFilterValue.Focus();
                }
            };

            txtFilterValue.Enter += (s, e) =>
            {
                if (FilterEnabled && !string.IsNullOrWhiteSpace(txtFilterValue.Text))
                    FilterPersonSuggestions();
            };

            this.Resize += (s, e) => PositionPersonSuggestions();
        }
        private void LoadPersonsForSuggestions()
        {
            if (_personsTable != null)
                return;

            _personsTable = clsPerson.GetAllPersons();

            if (_personsTable == null ||
                !_personsTable.Columns.Contains("PersonID") ||
                !_personsTable.Columns.Contains("FullName"))
            {
                throw new InvalidOperationException(
                    "The person list must contain PersonID and FullName.");
            }
        }

        private void PositionPersonSuggestions()
        {
            if (dgvPersonSuggestions == null)
                return;

            // Find textbox position relative to the user control.
            Point location = this.PointToClient(
                txtFilterValue.Parent.PointToScreen(
                    txtFilterValue.Location));

            dgvPersonSuggestions.Left = location.X;
            dgvPersonSuggestions.Top =
                location.Y + txtFilterValue.Height + 3;

            dgvPersonSuggestions.Width =
                Math.Min(
                    Math.Max(txtFilterValue.Width, 480),
                    this.ClientSize.Width - dgvPersonSuggestions.Left - 10);

            dgvPersonSuggestions.Height = Math.Min(
                230,
                Math.Max(50,
                    this.ClientSize.Height - dgvPersonSuggestions.Top - 10));

            dgvPersonSuggestions.BringToFront();
        }

        private void FilterPersonSuggestions()
        {
            if (!FilterEnabled || _selectingPerson || _loadingPerson)
                return;

            // Prevent NullReferenceException.
            if (dgvPersonSuggestions == null)
                return;

            try
            {
                LoadPersonsForSuggestions();

                string searchText = txtFilterValue.Text.Trim();

                DataTable filtered = _personsTable.Clone();

                foreach (DataRow row in _personsTable.Rows)
                {
                    string fullName =
                        Convert.ToString(row["FullName"]) ?? "";

                    if (string.IsNullOrEmpty(searchText) ||
                        fullName.IndexOf(
                            searchText,
                            StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        filtered.ImportRow(row);
                    }
                }

                dgvPersonSuggestions.DataSource = filtered;

                foreach (DataGridViewColumn column
                    in dgvPersonSuggestions.Columns)
                {
                    column.Visible =
                        column.Name == "PersonID" ||
                        column.Name == "FullName" ||
                        column.Name == "Phone";
                }

                if (dgvPersonSuggestions.Columns.Contains("PersonID"))
                    dgvPersonSuggestions.Columns["PersonID"].HeaderText = "ID";

                if (dgvPersonSuggestions.Columns.Contains("FullName"))
                    dgvPersonSuggestions.Columns["FullName"].HeaderText =
                        Tr("Name");

                if (dgvPersonSuggestions.Columns.Contains("Phone"))
                    dgvPersonSuggestions.Columns["Phone"].HeaderText =
                        Tr("Phone");

                PositionPersonSuggestions();

                dgvPersonSuggestions.Visible = filtered.Rows.Count > 0;

                if (dgvPersonSuggestions.Visible)
                    dgvPersonSuggestions.BringToFront();
            }
            catch (Exception ex)
            {
                dgvPersonSuggestions.Visible = false;

                MessageBox.Show(
                    ex.Message,
                    Tr("Search Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void SelectSuggestedPerson()
        {
            if (dgvPersonSuggestions == null || dgvPersonSuggestions.CurrentRow == null)
                return;

            DataRowView selected =
                dgvPersonSuggestions.CurrentRow.DataBoundItem as DataRowView;

            if (selected == null)
                return;

            int personID = Convert.ToInt32(selected["PersonID"]);

            _selectingPerson = true;

            try
            {
                // Existing method loads the person card.
                LoadPersonInfo(personID);

                // Display the name instead of the numeric ID.
                txtFilterValue.Text =
                    Convert.ToString(selected["FullName"]);

                dgvPersonSuggestions.Visible = false;
            }
            finally
            {
                _selectingPerson = false;
            }
        }
        private void _DataBack(
            object sender, int personID)
        {
            LoadPersonInfo(personID);
        }

        public void LoadPersonInfo(int personID)
        {
            _loadingPerson = true;
            _personSelectionValid = false;

            try
            {
                ctrlPersonCard1.LoadPersonInfo(personID);
                _personSelectionValid = ctrlPersonCard1.PersonID == personID &&
                    ctrlPersonCard1.SelectedPersonInfo != null;
                txtPId.Text = _personSelectionValid ? personID.ToString() : "";

                clsPerson selected =
    ctrlPersonCard1.SelectedPersonInfo;

                if (selected != null)
                {
                    txtFilterValue.Text = selected.Name;
                }
                else
                {
                    txtFilterValue.Text = "";
                    txtPId.Text = "";
                }
                if (dgvPersonSuggestions != null)
                    dgvPersonSuggestions.Visible = false;
            }
            finally
            {
                _loadingPerson = false;
            }

            if (FilterEnabled &&
                _personSelectionValid &&
                ctrlPersonCard1.PersonID > 0)
            {
                OnPersonSelected?.Invoke(
                    ctrlPersonCard1.PersonID);
            }
        }
        private string GetPersonName(int personID)
        {

            DataRow row = clsPerson.GetPersonByID(personID);

            if (row == null)
                return "";

            if (Convert.ToInt32(row["PersonID"]) == personID)
            {
                return Convert.ToString(row["FullName"]);
            }

            return "";
        }
        public void FocuseTextBox()
        {
            txtFilterValue.Focus();
        }
    }
}
