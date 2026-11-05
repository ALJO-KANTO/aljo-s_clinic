
using Clinic.Doctor;
using Clinic.global_classes;
using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Controls
{
    public partial class ctrDoctorCardInfoWithFilter : UserControl
    {
        public event EventHandler<clsEventArgs> DoctorCreated;
        private int _DoctorID = -1;
        private bool filterEnabled;
        private bool addEnabled;
        private bool loading;
        private TextBox txtNameSearch;
        private DataGridView dgvSuggestions;
        private DataTable searchData;

        public ctrDoctorCardInfoWithFilter()
        {
            InitializeComponent();
            SetupNameSearch();
        }

        public void OnDoctorCreated(int doctorID, int personID) =>
            DoctorCreated?.Invoke(this, new clsEventArgs(doctorID, personID));

        public bool FilterEnabled
        {
            get => filterEnabled;
            set
            {
                filterEnabled = value;
                gbFilters.Enabled = value;
                if (dgvSuggestions != null) dgvSuggestions.Visible = false;
            }
        }

        public bool btnAddNewEnabled
        {
            get => addEnabled;
            set { addEnabled = value; btnAddNewDoctor.Enabled = value; }
        }

        public int DoctorID => ctrDoctorCardInfo1.DoctorID;
        public clsDoctor SelectedDoctor => ctrDoctorCardInfo1.SelectedDoctor;
        public void FilterFocus() => txtFilterValue.Focus();

        private void SetupNameSearch()
        {
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
                if (control is Label && control.Text.IndexOf("Doctor ID", StringComparison.OrdinalIgnoreCase) >= 0)
                    control.Text = TranslationClass.IsArabic ? "البحث برقم الطبيب:" : "Find By Doctor ID:";

            txtNameSearch = new TextBox
            {
                Name = "txtDoctorNameSearch",
                Font = txtFilterValue.Font,
                Size = new Size(200, txtFilterValue.Height)
            };
            gbFilters.Controls.Add(txtNameSearch);
            txtNameSearch.BringToFront();
            txtNameSearch.TextChanged += (s, e) => FilterNames();
            txtNameSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Down && dgvSuggestions.Visible && dgvSuggestions.Rows.Count > 0)
                {
                    dgvSuggestions.Focus();
                    dgvSuggestions.CurrentCell = dgvSuggestions.Rows[0].Cells[0];
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvSuggestions.Visible)
                {
                    e.SuppressKeyPress = true;
                    SelectSuggestion();
                }
                else if (e.KeyCode == Keys.Escape) dgvSuggestions.Visible = false;
            };

            dgvSuggestions = new DataGridView
            {
                Name = "dgvDoctorSuggestions",
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 30,
                RowTemplate = { Height = 30 },
                Visible = false,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(220, 225, 230),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    BackColor = Color.White,
                    SelectionForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(0, 120, 215)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    BackColor = Color.FromArgb(242, 247, 252),
                    SelectionForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(0, 120, 215)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    BackColor = Color.FromArgb(225, 237, 247),
                    SelectionForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(225, 237, 247)
                }
            };
            Controls.Add(dgvSuggestions);
            dgvSuggestions.BringToFront();
            // Set explicit colors for each row, including rows created after rebinding.
            dgvSuggestions.DataBindingComplete += (s, e) => ApplySuggestionColors();
            dgvSuggestions.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0) SelectSuggestion();
            };
            dgvSuggestions.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) SelectSuggestion(); };
            dgvSuggestions.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SelectSuggestion(); }
                else if (e.KeyCode == Keys.Escape) { dgvSuggestions.Visible = false; txtNameSearch.Focus(); }
            };
            gbFilters.Resize += (s, e) => PositionSearch();
            Resize += (s, e) => PositionSearch();
            PositionSearch();
        }

        private void PositionSearch()
        {
            if (txtNameSearch == null || dgvSuggestions == null) return;
            int left = txtFilterValue.Right + 12;
            int rightLimit = btnFind.Left - 12;
            if (rightLimit - left < 130)
            {
                txtNameSearch.Location = new Point(txtFilterValue.Left, txtFilterValue.Bottom + 5);
                txtNameSearch.Width = Math.Max(150, Math.Min(txtFilterValue.Width, gbFilters.ClientSize.Width - txtNameSearch.Left - 12));
            }
            else
            {
                txtNameSearch.Location = new Point(left, txtFilterValue.Top);
                txtNameSearch.Width = rightLimit - left;
            }
            Point p = PointToClient(gbFilters.PointToScreen(txtNameSearch.Location));
            dgvSuggestions.SetBounds(p.X, p.Y + txtNameSearch.Height + 3,
                Math.Max(250, txtNameSearch.Width), 190);
            dgvSuggestions.BringToFront();
        }

        private static string Read(DataRow row, params string[] names)
        {
            foreach (string name in names)
                if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value)
                    return Convert.ToString(row[name]);
            return "";
        }

        private void LoadSearchData()
        {
            // Refresh on each search so the dropdown uses the latest doctor records.
            DataTable source = clsDoctor.GetAllDoctors();
            searchData = new DataTable();
            searchData.Columns.Add("DoctorID", typeof(int));
            searchData.Columns.Add("FullName", typeof(string));
            searchData.Columns.Add("Specialization", typeof(string));

            if (source == null) return;

            foreach (DataRow row in source.Rows)
            {
                int doctorID;
                if (!int.TryParse(Read(row, "DoctorID", "Doctor ID"), out doctorID) || doctorID <= 0)
                    continue;

                string name = Read(row, "FullName", "Full Name", "DoctorName", "Name");
                if (string.IsNullOrWhiteSpace(name))
                {
                    int personID;
                    if (int.TryParse(Read(row, "PersonID", "Person ID"), out personID))
                    {
                        clsPerson person = clsPerson.Find(personID);
                        if (person != null) name = person.Name;
                    }
                }

                if (string.IsNullOrWhiteSpace(name)) continue;
                searchData.Rows.Add(doctorID, name,
                    Read(row, "Specialization", "Specializations", "SpecializationName"));
            }
        }

        private void FilterNames()
        {
            if (loading || !FilterEnabled || dgvSuggestions == null) return;
            string term = txtNameSearch.Text.Trim();
            if (term.Length == 0) { dgvSuggestions.Visible = false; return; }
            try
            {
                LoadSearchData();
                DataTable matches = searchData.Clone();
                foreach (DataRow row in searchData.Rows)
                {
                    string name = Convert.ToString(row["FullName"]) ?? "";
                    string id = Convert.ToString(row["DoctorID"]) ?? "";
                    if (name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        id.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                        matches.ImportRow(row);
                }
                dgvSuggestions.DataSource = matches;
                ApplySuggestionColors();
                dgvSuggestions.Visible = matches.Rows.Count > 0;
                if (dgvSuggestions.Visible) { PositionSearch(); dgvSuggestions.BringToFront(); }
            }
            catch (Exception ex)
            {
                dgvSuggestions.Visible = false;
                MessageBox.Show(ex.Message, "Doctor Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplySuggestionColors()
        {
            if (dgvSuggestions == null) return;
            dgvSuggestions.DefaultCellStyle.ForeColor = Color.Black;
            dgvSuggestions.DefaultCellStyle.BackColor = Color.White;
            dgvSuggestions.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvSuggestions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 252);
            dgvSuggestions.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvSuggestions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);

            foreach (DataGridViewRow row in dgvSuggestions.Rows)
            {
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.BackColor = row.Index % 2 == 0
                    ? Color.White : Color.FromArgb(242, 247, 252);
                row.DefaultCellStyle.SelectionForeColor = Color.White;
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            }
        }

        private void SelectSuggestion()
        {
            DataRowView row = dgvSuggestions.CurrentRow?.DataBoundItem as DataRowView;
            if (row == null) return;
            int doctorID = Convert.ToInt32(row["DoctorID"]);
            dgvSuggestions.Visible = false;
            LoadDoctorInfo(doctorID);
        }

        private void _LoadDoctorInfo()
        {
            if (_DoctorID <= 0) return;
            ctrDoctorCardInfo1.LoadDoctorInfo(_DoctorID);
            if (ctrDoctorCardInfo1.SelectedDoctor == null) { _DoctorID = -1; return; }
            loading = true;
            try
            {
                txtNameSearch.Text = clsPerson.Find(ctrDoctorCardInfo1.SelectedDoctor.PersonID)?.Name ?? "";
                dgvSuggestions.Visible = false;
            }
            finally { loading = false; }
            if (FilterEnabled)
                OnDoctorCreated(ctrDoctorCardInfo1.DoctorID, ctrDoctorCardInfo1.SelectedDoctor.PersonID);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtFilterValue.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Enter a valid Doctor ID.", "Doctor Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _DoctorID = id;
            _LoadDoctorInfo();
        }

        private void btnAddNewDoctor_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAddUpdateDoctor())
            {
                frm.DataBack += (s, id) => LoadDoctorInfo(id);
                frm.ShowDialog(this.FindForm());
            }
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e) =>
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);

        private void txtFilterValue_Validating(object sender, CancelEventArgs e) =>
            errorProvider1.SetError(txtFilterValue, "");

        public void LoadDoctorInfo(int doctorID)
        {
            _DoctorID = doctorID;
            txtFilterValue.Text = doctorID.ToString();
            _LoadDoctorInfo();
        }
    }
}

