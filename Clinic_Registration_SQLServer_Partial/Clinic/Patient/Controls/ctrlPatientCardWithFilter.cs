using Clinic.global_classes;
using Clinic.Patient;
using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Clinic.Controls
{
    public partial class ctrlPatientCardWithFilter : UserControl
    {
        public event EventHandler<clsEventArgs> PatientCreated;
        private TextBox txtNameSearch;
        private ListBox dgvSuggestions;
        private DataTable searchData;
        private bool loading;
        private bool filterEnabled;
        private bool addEnabled;
        private ToolStripDropDown suggestionPopup;
        private ToolStripControlHost suggestionHost;

        public ctrlPatientCardWithFilter()
        {
            InitializeComponent();
            SetupNameSearch();
            Load += (s, e) => PositionSearch();
            Disposed += (s, e) => { if (suggestionPopup != null) suggestionPopup.Dispose(); };
        }

        public void OnPatientCreated(int patientID, int personID)
        {
            PatientCreated?.Invoke(this, new clsEventArgs(patientID, personID));
        }

        public bool FilterEnabled
        {
            get => filterEnabled;
            set
            {
                filterEnabled = value;
                gbFilters.Enabled = value;
                if (dgvSuggestions != null) HideSuggestions();
            }
        }

        public bool btnAddNewEnabled
        {
            get => addEnabled;
            set { addEnabled = value; btnAddNewPatient.Enabled = value; }
        }

        public int PatientID => ctrlPatientCard1.PatientID;
        public clsPatient SelectedPatient => ctrlPatientCard1.SelectedPatient;

        private void SetupNameSearch()
        {
            // The existing Designer textbox remains the ID textbox.
            txtFilterValue.KeyPress -= txtFilterValue_KeyPress;
            txtFilterValue.KeyPress += txtFilterValue_KeyPress;
            txtFilterValue.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnFind_Click(btnFind, EventArgs.Empty);
                }
            };

            foreach (Control control in gbFilters.Controls)
                if (control is Label && control.Text.IndexOf("Patient ID", StringComparison.OrdinalIgnoreCase) >= 0)
                    control.Text = TranslationClass.IsArabic ? "البحث برقم المريض:" : "Find By Patient ID:";

            int left = txtFilterValue.Right + 12;
            int available = Math.Max(150, gbFilters.ClientSize.Width - left - 18);
            txtNameSearch = new TextBox
            {
                Name = "txtPatientNameSearch",
                Location = new Point(left, txtFilterValue.Top),
                Size = new Size(Math.Min(260, available), txtFilterValue.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Font = txtFilterValue.Font
            };
            gbFilters.Controls.Add(txtNameSearch);
            txtNameSearch.BringToFront();
            txtNameSearch.TextChanged += (s, e) =>
            {
                if (!loading && !string.IsNullOrWhiteSpace(txtNameSearch.Text))
                    txtFilterValue.Clear();
                FilterNames();
            };
            txtNameSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Down && suggestionPopup != null && suggestionPopup.Visible && dgvSuggestions.Items.Count > 0)
                {
                    dgvSuggestions.Focus();
                    dgvSuggestions.SelectedIndex = 0;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    if (suggestionPopup != null && suggestionPopup.Visible) SelectSuggestion();
                    else FilterNames(showAll: true);
                }
                else if (e.KeyCode == Keys.Escape) HideSuggestions();
            };

            dgvSuggestions = new ListBox
            {
                Name = "lstPatientSuggestions",
                DisplayMember = "FullName",
                ValueMember = "PatientID",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false,
                Visible = true
            };
            suggestionHost = new ToolStripControlHost(dgvSuggestions)
            {
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                Size = new Size(380, 180)
            };
            suggestionPopup = new ToolStripDropDown
            {
                AutoClose = true,
                AutoSize = false,
                Size = new Size(380, 180),
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            suggestionPopup.Items.Add(suggestionHost);
            dgvSuggestions.Click += (s, e) => SelectSuggestion();
            dgvSuggestions.DoubleClick += (s, e) => SelectSuggestion();
            dgvSuggestions.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SelectSuggestion();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    HideSuggestions();
                    txtNameSearch.Focus();
                }
            };
            gbFilters.Resize += (s, e) => PositionSearch();
            Resize += (s, e) => PositionSearch();
            VisibleChanged += (s, e) => { if (!Visible && dgvSuggestions != null) HideSuggestions(); };
            PositionSearch();
        }

        private void HideSuggestions()
        {
            if (suggestionPopup != null && suggestionPopup.Visible)
                suggestionPopup.Close();
        }

        private void PositionSearch()
        {
            if (txtNameSearch == null || txtFilterValue == null || btnFind == null)
                return;

            int available = btnFind.Left - 12 - txtFilterValue.Left;
            if (available < 160) return;
            int idWidth = Math.Min(115, Math.Max(65, available / 3));
            txtFilterValue.Width = idWidth;
            txtNameSearch.SetBounds(txtFilterValue.Right + 8, txtFilterValue.Top,
                Math.Max(70, btnFind.Left - 12 - (txtFilterValue.Right + 8)),
                txtFilterValue.Height);
        }
        //private void SelectSuggestion()
        //{
        //    PatientSearchItem selected =
        //        dgvSuggestions.SelectedItem as PatientSearchItem;

        //    if (selected == null)
        //        return;

        //    int patientID = selected.PatientID;

        //    HideSuggestions();

        //    LoadPatientInfo(patientID);
        //}
        //private void ShowSuggestionsOld()
        //{
        //    if (suggestionPopup == null ||
        //        txtNameSearch == null ||
        //        dgvSuggestions == null ||
        //        !txtNameSearch.IsHandleCreated)
        //        return;

        //    if (dgvSuggestions.Items.Count == 0)
        //    {
        //        HideSuggestions();
        //        return;
        //    }

        //    int width = Math.Max(300, txtNameSearch.Width);

        //    int height = Math.Min(
        //        200,
        //        Math.Max(40, dgvSuggestions.Items.Count * 29 + 6));

        //    suggestionHost.Size = new Size(width, height);
        //    suggestionPopup.Size = new Size(width, height);
        //    dgvSuggestions.Size = new Size(width, height);

        //    if (!suggestionPopup.Visible)
        //    {
        //        suggestionPopup.Show(
        //            txtNameSearch,
        //            new Point(0, txtNameSearch.Height + 2));
        //    }
        //}

        private void ShowSuggestions()
        {
            if (suggestionPopup == null || txtNameSearch == null ||
                !txtNameSearch.IsHandleCreated) return;

            int width = Math.Max(300, txtNameSearch.Width);
            int height = Math.Min(200, Math.Max(40, dgvSuggestions.Items.Count * 29 + 6));
            suggestionHost.Size = new Size(width, height);
            suggestionPopup.Size = new Size(width, height);
            dgvSuggestions.Size = new Size(width, height);
            if (!suggestionPopup.Visible)
                suggestionPopup.Show(txtNameSearch, new Point(0, txtNameSearch.Height + 2));
        }

        private void LoadSearchData()
        {
            // GetAllPatients now returns PatientID, PersonID, FullName and PhoneNumber.
            // Do not join the results to GetAllPersons() again.
            DataTable source = clsPerson.GetAllPatients();
            if (source == null)
                throw new InvalidOperationException("GetAllPatients returned null.");

            foreach (string required in new[] { "PatientID", "FullName" })
                if (!source.Columns.Contains(required))
                    throw new InvalidOperationException(
                        "GetAllPatients is missing " + required + ". Returned: " +
                        string.Join(", ", source.Columns.Cast<DataColumn>()
                            .Select(c => c.ColumnName)));

            searchData = new DataTable();
            searchData.Columns.Add("PatientID", typeof(int));
            searchData.Columns.Add("FullName", typeof(string));
            searchData.Columns.Add("Phone", typeof(string));

            foreach (DataRow row in source.Rows)
            {
                int id;
                if (!int.TryParse(Convert.ToString(row["PatientID"]), out id) || id <= 0)
                    continue;

                string name = Convert.ToString(row["FullName"]) ?? "";
                string phone = source.Columns.Contains("PhoneNumber")
                    ? Convert.ToString(row["PhoneNumber"]) ?? ""
                    : "";
                if (!string.IsNullOrWhiteSpace(name))
                    searchData.Rows.Add(id, name, phone);
            }
        }

        private void FilterNames(bool showAll = false)
        {
            if (loading || !FilterEnabled || dgvSuggestions == null) return;
            string term = txtNameSearch.Text.Trim();
            if (term.Length == 0 && !showAll)
            {
                HideSuggestions();
                return;
            }
            try
            {
                LoadSearchData();
                DataTable matches = searchData.Clone();
                foreach (DataRow row in searchData.Rows)
                {
                    string name = Convert.ToString(row["FullName"]);
                    if (term.Length == 0 || name.IndexOf(term,
                        StringComparison.OrdinalIgnoreCase) >= 0)
                        matches.ImportRow(row);
                }

                dgvSuggestions.BeginUpdate();

                try
                {
                    dgvSuggestions.Items.Clear();

                    foreach (DataRow row in matches.Rows)
                    {
                        int patientID = Convert.ToInt32(row["PatientID"]);
                        string fullName = Convert.ToString(row["FullName"]);

                        dgvSuggestions.Items.Add(
                            new PatientSearchItem
                            {
                                PatientID = patientID,
                                FullName = fullName
                            });
                    }
                }
                finally
                {
                    dgvSuggestions.EndUpdate();
                }

                if (matches.Rows.Count > 0)
                    ShowSuggestions();
                else
                {
                    HideSuggestions();
                    if (showAll)
                        MessageBox.Show(
                            "No matches for: " + term + "\n" +
                            "Rows returned by GetAllPatients: " +
                            (searchData == null ? 0 : searchData.Rows.Count) + "\n" +
                            "Check that the latest Clinic_DataAccess DLL is built and loaded.",
                            "Patient Search", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                HideSuggestions();
                MessageBox.Show(ex.Message, "Patient Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectSuggestion()
        {
            PatientSearchItem selected = dgvSuggestions.SelectedItem as PatientSearchItem;
            if (selected == null) return;
            int id = selected.PatientID;
            HideSuggestions();
            LoadPatientInfo(id);
        }

        private void _LoadPatientInfo()
        {
            if (!int.TryParse(txtFilterValue.Text, out int patientID) || patientID <= 0) return;
            ctrlPatientCard1.LoadPatientInfo(patientID);
            if (ctrlPatientCard1.SelectedPatient == null) return;
            loading = true;
            try
            {
                if (txtNameSearch != null)
                    txtNameSearch.Text = clsPerson.Find(ctrlPatientCard1.SelectedPatient.PersonID)?.Name ?? "";
                if (dgvSuggestions != null) HideSuggestions();
            }
            finally { loading = false; }
            if (FilterEnabled)
                OnPatientCreated(patientID, ctrlPatientCard1.SelectedPatient.PersonID);
        }

        public void FocuseFilter() => txtFilterValue.Focus();

        private void btnFind_Click(object sender, EventArgs e)
        {
            // Search by ID when supplied, otherwise search by name.
            string idText = txtFilterValue.Text.Trim();
            if (idText.Length > 0)
            {
                if (!int.TryParse(idText, out int id) || id <= 0)
                {
                    MessageBox.Show("Enter a valid Patient ID.", "Patient Search",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                LoadPatientInfo(id);
                return;
            }
            FilterNames(showAll: true);
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {
            // Do not block selecting a name from the dropdown.
            errorProvider1.SetError(txtFilterValue, "");
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnAddNewPatient_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAddUpdatePatient())
            {
                frm.DataBack += _DataBack;
                frm.ShowDialog(this.FindForm());
            }
        }

        public void LoadPatientInfo(int patientID)
        {
            txtFilterValue.Text = patientID.ToString();
            _LoadPatientInfo();
        }

        private void _DataBack(object sender, int patientID)
        {
            searchData = null;
            LoadPatientInfo(patientID);
        }
    }

    public class PatientSearchItem
    {
        public int PatientID { get; set; }
        public string FullName { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
