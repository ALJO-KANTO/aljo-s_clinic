using Clinic.global_classes;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Clinic_Business.clsDoctor;

namespace Clinic.Doctor
{
    public partial class frmAddUpdateDoctor : Form
    {
        private int _DoctorID = -1;
        private clsDoctor _Doctor;
        public enum enMode { AddNew = 1, Update = 2 };
        private enMode _Mode = enMode.AddNew;

        // Weekly schedule UI. Existing database structure remains dtWorkDays
        // with one row per shift (maximum two shifts per day).
        private TabPage tpWorkingSchedule;
        private DataGridView dgvWeeklySchedule;
        private ComboBox cmbCopyFromDay;
        private CheckedListBox clbCopyToDays;
        private Button btnCopyHours;
        private Button btnDoctorInfoNext;
        private DataTable _dtWeeklySchedule;

        private sealed class ScheduleDayItem
        {
            public int DayID { get; private set; }
            public string DayName { get; private set; }

            public ScheduleDayItem(int dayID, string dayName)
            {
                DayID = dayID;
                DayName = dayName;
            }

            public override string ToString() { return DayName; }
        }

        //private BindingList<clsDoctorShift> _ShiftsList = new BindingList<clsDoctor.clsDoctorShift>();
        public delegate void frmAddUpdateDoctorEventHandler(object sender, int DoctorID);

        public event frmAddUpdateDoctorEventHandler DataBack;

        public frmAddUpdateDoctor()
        {
            InitializeComponent();
            InitializeWeeklySchedulePage();
            InitializeDoctorInfoNextButton();
            tcDoctor.SelectedIndexChanged += tcDoctor_SelectedIndexChanged;
            _Mode = enMode.AddNew;
        }

        private class SpecializationItem
        {
            public int ID { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public frmAddUpdateDoctor(int DoctorID)
        {
            InitializeComponent();
            InitializeWeeklySchedulePage();
            InitializeDoctorInfoNextButton();
            tcDoctor.SelectedIndexChanged += tcDoctor_SelectedIndexChanged;
            _DoctorID = DoctorID;
            _Mode = enMode.Update;
        }

        private void LoadSpecializations()
        {
            DataTable dt = clsSpecialization.GetAllSpecializations();

            clbSpesalizations.DataSource = null;

            clbSpesalizations.DisplayMember = "SpecializationName";
            clbSpesalizations.ValueMember = "SpecializationID";

            clbSpesalizations.DataSource = dt;
        }


        private void _FillSpecializationsInCheckBox()
        {
            DataTable dt = clsSpecialization.GetAllSpecializations();

            clbSpesalizations.DataSource = null;
            clbSpesalizations.Items.Clear();

            if (dt == null)
                return;

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["SpecializationID"]);
                string name = Convert.ToString(row["SpecializationName"]);

                var item = new SpecializationItem
                {
                    ID = id,
                    Name = name
                };

                bool isDefault =
                    _Mode == enMode.AddNew &&
                    name == "General Medicine";

                clbSpesalizations.Items.Add(item, isDefault);
            }
        }




        private void _FiilDaysInComboBox()
        {
            DataTable dt = clsDoctor.GetAllDays();
            cmbDays.DataSource = dt;
            cmbDays.DisplayMember = "DayName";
            cmbDays.ValueMember = "DayID";
        }

        private void _ResetDefaultValues()
        {
            _FillSpecializationsInCheckBox();

            _FiilDaysInComboBox();

            tpDoctorInfo.Enabled = false;
            tpWorkingSchedule.Enabled = false;
            btnSave.Enabled = false;
            btnSave.Visible = false;

            if (_Mode == enMode.AddNew)
            {
                _Doctor = new clsDoctor();
                ctrlPersonCardWithFilter1.FilterEnabled = true;
                ctrlPersonCardWithFilter1.FocuseTextBox(); // تأكد أن الدالة موجودة بكارت الفلتر لديك
                lblTitle.Text = "Add New Doctor";
                _SetupShiftsGrid();
                _LoadWeeklyScheduleRows();

            }
            else
            {
                lblTitle.Text = "Update Doctor Info";
            }

            this.Text = lblTitle.Text;
        }


        private void _LoadData()
        {
            _Doctor = clsDoctor.FindDoctorByID(_DoctorID);

            if (_Doctor == null)
            {
                MessageBox.Show(
                    "No Doctor with ID = " + _DoctorID,
                    "Doctor Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                this.Close();
                return;
            }

            ctrlPersonCardWithFilter1.LoadPersonInfo(_Doctor.PersonID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;

            lbDoctorID.Text = _Doctor.DoctorID.ToString();
            txtConsultationFees.Text = _Doctor.ConsultationFees.ToString();
            txtLicenseNo.Text = _Doctor.LicenseNumber;
            chkIsActive.Checked = _Doctor.IsActive;

            _Doctor.SelectedSpecialtyIDs =
                _Doctor.GetDoctorSpecializations() ?? new List<int>();

            for (int i = 0; i < clbSpesalizations.Items.Count; i++)
            {
                SpecializationItem item =
                    clbSpesalizations.Items[i] as SpecializationItem;

                if (item == null)
                    continue;

                clbSpesalizations.SetItemChecked(
                    i,
                    _Doctor.SelectedSpecialtyIDs.Contains(item.ID));
            }

            _SetupShiftsGrid();
            _LoadWeeklyScheduleRows();

            tpDoctorInfo.Enabled = false;
            tpWorkingSchedule.Enabled = false;
            btnSave.Enabled = false;
            btnSave.Visible = false;
        }

        private void _SetupShiftsGrid()
        {
            dgvShifts.Columns.Clear();
            dgvShifts.AutoGenerateColumns = false;
            dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "DayID", DataPropertyName = "DayID", Visible = false });
            dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "DayName", HeaderText = "Day", DataPropertyName = "DayName", ReadOnly = true });
            dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "StartTime", HeaderText = "Start", DataPropertyName = "StartTime", ReadOnly = true });
            dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "EndTime", HeaderText = "End", DataPropertyName = "EndTime", ReadOnly = true });

            // 2. إضافة عمود الحذف
            DataGridViewImageColumn imgDelete = new DataGridViewImageColumn
            {
                Name = "Delete",
                HeaderText = "Action",
                Image = Properties.Resources.Delete_32,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 30
            };
            dgvShifts.Columns.Add(imgDelete);

            dgvShifts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (_Mode == enMode.AddNew)
                _Doctor.InsializeDoctorShifts();
            // ربط القائمة بالجدول
            dgvShifts.DataSource = _Doctor.dtWorkDays;
        }

        private void InitializeDoctorInfoNextButton()
        {
            btnDoctorInfoNext = new Button
            {
                Name = "btnDoctorInfoNext",
                Text = "Next",
                Size = new Size(135, 42),
                Location = new Point(
                    tpDoctorInfo.ClientSize.Width - 155,
                    tpDoctorInfo.ClientSize.Height - 60),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(11, 61, 107),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnDoctorInfoNext.FlatAppearance.BorderColor =
                Color.FromArgb(220, 225, 230);

            btnDoctorInfoNext.Click += btnDoctorInfoNext_Click;

            tpDoctorInfo.Controls.Add(btnDoctorInfoNext);
            btnDoctorInfoNext.BringToFront();
        }

        private void tcDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isWorkingScheduleTab =
                tcDoctor.SelectedTab == tpWorkingSchedule;

            btnSave.Visible = isWorkingScheduleTab;
            btnSave.Enabled = isWorkingScheduleTab;
        }

        private void InitializeWeeklySchedulePage()
        {
            // Keep the old shift controls and event handlers in the form for
            // compatibility, but move schedule entry to this simpler weekly tab.
            Control[] oldScheduleControls =
            {
                cmbDays, dtpStartTime, dtpEndTime, dgvShifts, btnAddTodataGrid,
                label2, label3, label4, label6, pictureBox3
            };

            foreach (Control control in oldScheduleControls)
            {
                if (control != null)
                    control.Visible = false;
            }

            tpWorkingSchedule = new TabPage
            {
                Name = "tpWorkingSchedule",
                Text = "Working Schedule",
                BackColor = Color.White,
                Padding = new Padding(8),
                UseVisualStyleBackColor = false
            };

            dgvWeeklySchedule = new DataGridView
            {
                Name = "dgvWeeklySchedule",
                Location = new Point(15, 18),
                Size = new Size(680, 405),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                EditMode = DataGridViewEditMode.EditOnEnter,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 43 },
                Font = new Font("Segoe UI", 9.5F),
                GridColor = Color.FromArgb(229, 231, 235)
            };
            dgvWeeklySchedule.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(11, 61, 107);
            dgvWeeklySchedule.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvWeeklySchedule.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvWeeklySchedule.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvWeeklySchedule.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvWeeklySchedule.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DisplayDayName",
                DataPropertyName = "DisplayDayName",
                HeaderText = "Day",
                ReadOnly = true,
                FillWeight = 92
            });

            List<string> timeOptions = new List<string> { "" };
            for (int minutes = 0; minutes < 24 * 60; minutes += 15)
                timeOptions.Add(DateTime.Today.AddMinutes(minutes).ToString("hh:mm tt", CultureInfo.InvariantCulture));

            AddScheduleTimeColumn("Start1", "From", timeOptions);
            AddScheduleTimeColumn("End1", "To", timeOptions);
            dgvWeeklySchedule.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "IsWorking",
                DataPropertyName = "IsWorking",
                HeaderText = "Active",
                FillWeight = 55
            });

            Panel copyPanel = new Panel
            {
                Location = new Point(710, 18),
                Size = new Size(310, 405),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            Label copyTitle = CreateScheduleLabel("Copy Working Hours", 14, 12, true, 12F);
            Label fromLabel = CreateScheduleLabel("Copy from Day", 16, 62, true, 9.5F);
            cmbCopyFromDay = new ComboBox
            {
                Name = "cmbCopyFromDay",
                Location = new Point(16, 88),
                Size = new Size(276, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                FlatStyle = FlatStyle.Flat
            };
            Label toLabel = CreateScheduleLabel("Copy to Days", 16, 137, true, 9.5F);
            clbCopyToDays = new CheckedListBox
            {
                Name = "clbCopyToDays",
                Location = new Point(16, 164),
                Size = new Size(276, 155),
                CheckOnClick = true,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White
            };
            btnCopyHours = new Button
            {
                Name = "btnCopyHours",
                Text = "Copy to Selected Days",
                Location = new Point(16, 337),
                Size = new Size(276, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(11, 61, 107),
                ForeColor = Color.White,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnCopyHours.FlatAppearance.BorderSize = 0;
            btnCopyHours.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 75, 125);
            btnCopyHours.FlatAppearance.MouseDownBackColor = Color.FromArgb(8, 45, 80);

            btnCopyHours.UseCompatibleTextRendering = true;
            btnCopyHours.ForeColor = Color.White;

            btnCopyHours.Click += btnCopyHours_Click;
            copyPanel.Controls.Add(copyTitle);
            copyPanel.Controls.Add(fromLabel);
            copyPanel.Controls.Add(cmbCopyFromDay);
            copyPanel.Controls.Add(toLabel);
            copyPanel.Controls.Add(clbCopyToDays);
            copyPanel.Controls.Add(btnCopyHours);

            Label scheduleNote = CreateScheduleLabel(
                "Select one working time range for each active day.",
                18, 432, false, 8.5F);
            scheduleNote.ForeColor = Color.FromArgb(107, 114, 128);
            scheduleNote.AutoSize = true;

            tpWorkingSchedule.Controls.Add(dgvWeeklySchedule);
            tpWorkingSchedule.Controls.Add(copyPanel);
            tpWorkingSchedule.Controls.Add(scheduleNote);
            tcDoctor.TabPages.Add(tpWorkingSchedule);

            ApplyWeeklyScheduleTranslation();
        }

        private void AddScheduleTimeColumn(string name, string header, List<string> options)
        {
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn
            {
                Name = name,
                DataPropertyName = name,
                HeaderText = header,
                DataSource = new List<string>(options),
                FlatStyle = FlatStyle.Flat,
                FillWeight = 82,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            };
            dgvWeeklySchedule.Columns.Add(column);
        }

        private Label CreateScheduleLabel(string text, int x, int y, bool bold, float size)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
        }

        private void ApplyWeeklyScheduleTranslation()
        {
            tpWorkingSchedule.RightToLeft = RightToLeft.No;
            dgvWeeklySchedule.RightToLeft = RightToLeft.No;
            dgvWeeklySchedule.Columns[0].HeaderText = "Day";
            dgvWeeklySchedule.Columns[1].HeaderText = "From";
            dgvWeeklySchedule.Columns[2].HeaderText = "To";
            dgvWeeklySchedule.Columns[3].HeaderText = "Active";
            if (cmbCopyFromDay != null) cmbCopyFromDay.RightToLeft = RightToLeft.No;
            if (clbCopyToDays != null) clbCopyToDays.RightToLeft = RightToLeft.No;
        }

        private void _LoadWeeklyScheduleRows()
        {
            if (dgvWeeklySchedule == null || cmbDays.DataSource == null)
                return;

            DataTable days = cmbDays.DataSource as DataTable;
            if (days == null || !days.Columns.Contains("DayID") || !days.Columns.Contains("DayName"))
                return;

            _dtWeeklySchedule = new DataTable();
            _dtWeeklySchedule.Columns.Add("DayID", typeof(int));
            _dtWeeklySchedule.Columns.Add("DayName", typeof(string));
            _dtWeeklySchedule.Columns.Add("DisplayDayName", typeof(string));
            _dtWeeklySchedule.Columns.Add("IsWorking", typeof(bool));
            _dtWeeklySchedule.Columns.Add("Start1", typeof(string));
            _dtWeeklySchedule.Columns.Add("End1", typeof(string));

            foreach (DataRow day in days.Rows)
            {
                int dayID = Convert.ToInt32(day["DayID"]);
                string dayName = Convert.ToString(day["DayName"]);
                DataRow weeklyRow = _dtWeeklySchedule.NewRow();
                weeklyRow["DayID"] = dayID;
                weeklyRow["DayName"] = dayName;
                weeklyRow["DisplayDayName"] = dayName;
                weeklyRow["IsWorking"] = false;
                weeklyRow["Start1"] = "";
                weeklyRow["End1"] = "";

                if (_Doctor != null && _Doctor.dtWorkDays != null)
                {
                    DataRow[] shifts = _Doctor.dtWorkDays.Select("DayID = " + dayID, "StartTime ASC");
                    if (shifts.Length > 0)
                    {
                        weeklyRow["Start1"] = FormatScheduleTime(shifts[0]["StartTime"]);
                        weeklyRow["End1"] = FormatScheduleTime(shifts[0]["EndTime"]);
                        weeklyRow["IsWorking"] = true;
                    }
                }
                _dtWeeklySchedule.Rows.Add(weeklyRow);
            }

            // Automatically mark a day as Active when at least one complete,
            // valid From/To time range is selected.
            _dtWeeklySchedule.ColumnChanged += dtWeeklySchedule_ColumnChanged;

            dgvWeeklySchedule.DataSource = _dtWeeklySchedule;
            cmbCopyFromDay.DataSource = _dtWeeklySchedule.Copy();
            cmbCopyFromDay.DisplayMember = "DisplayDayName";
            cmbCopyFromDay.ValueMember = "DayID";
            cmbCopyFromDay.SelectedIndex = _dtWeeklySchedule.Rows.Count > 0 ? 0 : -1;

            clbCopyToDays.Items.Clear();
            foreach (DataRow row in _dtWeeklySchedule.Rows)
            {
                clbCopyToDays.Items.Add(
                    new ScheduleDayItem(Convert.ToInt32(row["DayID"]), Convert.ToString(row["DisplayDayName"])), false);
            }
            ApplyWeeklyScheduleTranslation();
        }


        private void dtWeeklySchedule_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName != "Start1" && e.Column.ColumnName != "End1")
                return;

            TimeSpan start, end;
            bool hasValidShift =
                TryParseScheduleTime(Convert.ToString(e.Row["Start1"]), out start) &&
                TryParseScheduleTime(Convert.ToString(e.Row["End1"]), out end) && end > start;

            e.Row["IsWorking"] = hasValidShift;
        }

        private string FormatScheduleTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return "";

            TimeSpan time;
            if (value is TimeSpan)
                time = (TimeSpan)value;
            else if (value is DateTime)
                time = ((DateTime)value).TimeOfDay;
            else if (!TimeSpan.TryParse(Convert.ToString(value), out time))
                return "";

            return DateTime.Today.Add(time).ToString("hh:mm tt", CultureInfo.InvariantCulture);
        }

        private bool TryParseScheduleTime(string value, out TimeSpan time)
        {
            time = TimeSpan.Zero;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            DateTime parsed;
            if (!DateTime.TryParseExact(value, "hh:mm tt", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out parsed))
                return false;

            time = parsed.TimeOfDay;
            return true;
        }

        private void btnCopyHours_Click(object sender, EventArgs e)
        {
            if (_dtWeeklySchedule == null || cmbCopyFromDay.SelectedValue == null)
                return;

            dgvWeeklySchedule.EndEdit();
            int sourceDayID = Convert.ToInt32(cmbCopyFromDay.SelectedValue);
            DataRow[] sourceRows = _dtWeeklySchedule.Select("DayID = " + sourceDayID);
            if (sourceRows.Length == 0 || !Convert.ToBoolean(sourceRows[0]["IsWorking"]))
            {
                MessageBox.Show("The selected source day has no active working hours.", "Copy Working Hours",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int copied = 0;
            foreach (object checkedItem in clbCopyToDays.CheckedItems)
            {
                ScheduleDayItem target = checkedItem as ScheduleDayItem;
                if (target == null || target.DayID == sourceDayID)
                    continue;

                DataRow[] targetRows = _dtWeeklySchedule.Select("DayID = " + target.DayID);
                if (targetRows.Length == 0) continue;

                targetRows[0]["IsWorking"] = sourceRows[0]["IsWorking"];
                targetRows[0]["Start1"] = sourceRows[0]["Start1"];
                targetRows[0]["End1"] = sourceRows[0]["End1"];
                copied++;
            }

            dgvWeeklySchedule.Refresh();
            MessageBox.Show(copied == 0 ? "Select at least one different target day." :
                "Working hours copied successfully.", "Copy Working Hours",
                MessageBoxButtons.OK, copied == 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        private bool _SyncWeeklyScheduleToDoctorShifts()
        {
            if (_dtWeeklySchedule == null || _Doctor == null || _Doctor.dtWorkDays == null)
                return true;

            dgvWeeklySchedule.EndEdit();
            DataTable newShifts = _Doctor.dtWorkDays.Clone();

            foreach (DataRow day in _dtWeeklySchedule.Rows)
            {
                if (!Convert.ToBoolean(day["IsWorking"]))
                    continue;

                TimeSpan start, end;
                bool hasStart = TryParseScheduleTime(Convert.ToString(day["Start1"]), out start);
                bool hasEnd = TryParseScheduleTime(Convert.ToString(day["End1"]), out end);

                if (!hasStart || !hasEnd)
                {
                    MessageBox.Show("Please select both From and To times for " + day["DayName"] + ".",
                        "Invalid Schedule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!ValidateAndAddScheduleShift(newShifts, day, start, end)) return false;
            }

            _Doctor.dtWorkDays.Clear();
            foreach (DataRow row in newShifts.Rows)
                _Doctor.dtWorkDays.ImportRow(row);

            return true;
        }

        private bool ValidateAndAddScheduleShift(DataTable target, DataRow day, TimeSpan start, TimeSpan end)
        {
            if (end <= start)
            {
                MessageBox.Show("End time must be later than start time for " + day["DayName"] + ".",
                    "Invalid Time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (end - start < TimeSpan.FromHours(4))
            {
                MessageBox.Show("Each shift must be at least 4 hours for " + day["DayName"] + ".",
                    "Invalid Shift Duration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataRow newRow = target.NewRow();
            newRow["DayID"] = day["DayID"];
            newRow["DayName"] = day["DayName"];
            newRow["StartTime"] = start;
            newRow["EndTime"] = end;
            target.Rows.Add(newRow);
            return true;
        }

        private void frmAddUpdateDoctor_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (_Mode == enMode.Update)
            {
                _LoadData();
            }

        }

        private void tcDoctor_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if ((e.TabPage.Name == "tpDoctorInfo" || e.TabPage.Name == "tpWorkingSchedule") && _Mode == enMode.AddNew)
            {
                if (ctrlPersonCardWithFilter1.PersonID == -1 || clsDoctor.IsDoctorExistForPersonID(ctrlPersonCardWithFilter1.PersonID))
                {
                    e.Cancel = true; // إلغاء الانتقال القسري
                    MessageBox.Show("Please select a valid person first before moving to doctor info.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tcDoctor.SelectedTab != tpWorkingSchedule)
                return;

            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid! Please correct the errors and try again.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _Doctor.PersonID = ctrlPersonCardWithFilter1.PersonID;

            _Doctor.ConsultationFees = Convert.ToSingle(txtConsultationFees.Text.Trim());

            _Doctor.LicenseNumber = txtLicenseNo.Text.Trim();

            _Doctor.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            _Doctor.IsActive = chkIsActive.Checked;

            _Doctor.SelectedSpecialtyIDs =
                clbSpesalizations.CheckedItems
                    .Cast<SpecializationItem>()
                    .Select(x => x.ID)
                    .ToList();
            // Copy the weekly grid into the existing dtWorkDays table.
            // SaveDoctor continues using the original database/save logic.
            if (!_SyncWeeklyScheduleToDoctorShifts())
                return;

            if (_Doctor.SaveDoctor())
            {
                lbDoctorID.Text = _Doctor.DoctorID.ToString();

                _Mode = enMode.Update;
                ctrlPersonCardWithFilter1.FilterEnabled = false;

                lblTitle.Text = "Update Doctor Info";
                this.Text = lblTitle.Text;
                DataBack?.Invoke(this, _Doctor.DoctorID);
                MessageBox.Show("Doctor information saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);



            }
            else
            {

                MessageBox.Show("Failed to save doctor information. Please try again.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);



            }
        }

        private void btnPersonInfoNext_Click(object sender, EventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                tpDoctorInfo.Enabled = true;
                tpWorkingSchedule.Enabled = false;

                tcDoctor.SelectedTab = tpDoctorInfo;

                btnSave.Enabled = false;
                btnSave.Visible = false;

                txtLicenseNo.Focus();
                return;
            }

            if (ctrlPersonCardWithFilter1.PersonID == -1)
            {
                MessageBox.Show("Please select a valid person first before moving to doctor info.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clsDoctor.IsDoctorExistForPersonID(ctrlPersonCardWithFilter1.PersonID) &&
                _Mode == enMode.AddNew)
            {
                MessageBox.Show("The selected person is already assigned as a doctor. Please select a different person.",
                    "Duplicate Doctor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tpDoctorInfo.Enabled = true;
            tpWorkingSchedule.Enabled = false;

            tcDoctor.SelectedTab = tpDoctorInfo;

            btnSave.Enabled = false;
            btnSave.Visible = false;

            txtLicenseNo.Focus();
        }

        private void btnDoctorInfoNext_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                MessageBox.Show(
                    "Please complete all required doctor information.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (clbSpesalizations.CheckedItems.Count == 0)
            {
                MessageBox.Show(
                    "Please select at least one specialization.",
                    "Specialization Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            tpWorkingSchedule.Enabled = true;
            tcDoctor.SelectedTab = tpWorkingSchedule;

            btnSave.Visible = true;
            btnSave.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtConsultationFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        private void TextBoxValidating(object sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (string.IsNullOrEmpty(txt.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txt, "This field is required.");
            }
            else
            {
                errorProvider1.SetError(txt, null);
            }
        }

        private void CheckedListBoxValidating(object sender, CancelEventArgs e)
        {
            CheckedListBox checkedListBox = (CheckedListBox)sender;

            if (checkedListBox.CheckedItems.Count == 0)
            {
                e.Cancel = true;
                errorProvider1.SetError(checkedListBox, "Please select at least one specialization.");
            }
            else
            {
                errorProvider1.SetError(checkedListBox, null);
            }
        }

        private void frmAddUpdateDoctor_Activated(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.FocuseTextBox();
        }

        private void btnAddTodataGrid_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren()) return;

            int newDayID = (int)cmbDays.SelectedValue;
            TimeSpan newStart = dtpStartTime.Value.TimeOfDay;
            TimeSpan newEnd = dtpEndTime.Value.TimeOfDay;

            TimeSpan minShiftDuration = TimeSpan.FromHours(4);
            if (newEnd <= newStart)
            {
                MessageBox.Show("End time cannot be earlier than start time.", "Invalid Time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newEnd - newStart < minShiftDuration)
            {
                MessageBox.Show("Shift duration must be at least 4 hours.", "Invalid Shift Duration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. التحقق من عدد النوبات (بحد أقصى 2 في اليوم)
            int countSameDay = _Doctor.dtWorkDays.Rows.Cast<DataRow>().Count(r => Convert.ToInt32(r["DayID"]) == newDayID);
            if (countSameDay >= 2)
            {
                MessageBox.Show("You cannot add more than two shifts for the same day.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. التحقق من التداخل الزمني باستخدام LINQ
            bool isOverlapping = _Doctor.dtWorkDays.Rows.Cast<DataRow>().Any(r => Convert.ToInt32(r["DayID"]) == newDayID &&
                                 !(newEnd <= (TimeSpan)r["StartTime"] || newStart >= (TimeSpan)r["EndTime"]));

            if (isOverlapping)
            {
                MessageBox.Show("This shift conflicts with another shift already added for this day.", "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. الإضافة مباشرة للقائمة (الجدول سيحدث نفسه تلقائياً)
            //_ShiftsList.Add(new clsDoctorShift
            //{
            //    DayID = newDayID,
            //    DayName=cmbDays.Text,
            //    StartTime = newStart,
            //    EndTime = newEnd
            //});
            DataRow newRow = _Doctor.dtWorkDays.NewRow();
            newRow["DayID"] = newDayID;
            newRow["DayName"] = cmbDays.Text;
            newRow["StartTime"] = newStart;
            newRow["EndTime"] = newEnd;
            _Doctor.dtWorkDays.Rows.Add(newRow);

        }

        private void dgvShifts_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvShifts.Columns[e.ColumnIndex].Name == "Delete")
            {
                dgvShifts.Cursor = Cursors.Hand; // يتحول الماوس ليد
            }
            else
            {
                dgvShifts.Cursor = Cursors.Default; // يعود لشكل السهم
            }
        }

        private void dgvShifts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvShifts.Columns[e.ColumnIndex].Name == "Delete")
            {
                // تأكيد الحذف
                DialogResult result = MessageBox.Show("Are you sure you want to delete this shift?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dgvShifts.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void dtpStartTime_ValueChanged(object sender, EventArgs e)
        {
            DateTime startTime = dtpStartTime.Value;

            // 2. حساب الوقت الأدنى المسموح به للنهاية (البداية + 4 ساعات)
            DateTime minAllowedEndTime = startTime.AddHours(4);

            // 3. تحديث وقت النهاية إذا كان الحالي أقل من الحد الأدنى الجديد
            // لضمان عدم حدوث خطأ عند تغيير البداية
            if (dtpEndTime.Value < minAllowedEndTime)
            {
                dtpEndTime.Value = minAllowedEndTime;
            }
        }

        private void dtpEndTime_ValueChanged(object sender, EventArgs e)
        {
            DateTime minAllowedEndTime = dtpStartTime.Value.AddHours(4);

            if (dtpEndTime.Value < minAllowedEndTime)
            {
                // منع المستخدم من اختيار وقت أقل من 4 ساعات
                dtpEndTime.Value = minAllowedEndTime;

                // إظهار تنبيه بصري خفيف
                errorProvider1.SetError(dtpEndTime, "Minimum shift duration is 4 hours.");
            }
            else
            {
                errorProvider1.SetError(dtpEndTime, ""); // مسح التنبيه
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
