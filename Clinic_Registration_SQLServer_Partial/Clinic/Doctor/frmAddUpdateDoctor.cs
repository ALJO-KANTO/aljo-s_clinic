using Clinic.global_classes;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        //private BindingList<clsDoctorShift> _ShiftsList = new BindingList<clsDoctor.clsDoctorShift>();
        public delegate void frmAddUpdateDoctorEventHandler(object sender, int DoctorID);

        public event frmAddUpdateDoctorEventHandler DataBack;

        public frmAddUpdateDoctor()
        {
            InitializeComponent();
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
            btnSave.Enabled = false;

            if (_Mode == enMode.AddNew)
            {
                _Doctor = new clsDoctor();
                ctrlPersonCardWithFilter1.FilterEnabled = true;
                ctrlPersonCardWithFilter1.FocuseTextBox(); // تأكد أن الدالة موجودة بكارت الفلتر لديك
                lblTitle.Text = "Add New Doctor";
                _SetupShiftsGrid();

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

            tpDoctorInfo.Enabled = true;
            btnSave.Enabled = true;
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
            if(_Mode==enMode.AddNew)
            _Doctor.InsializeDoctorShifts();
            // ربط القائمة بالجدول
            dgvShifts.DataSource = _Doctor.dtWorkDays;
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
            if (e.TabPage.Name == "tpDoctorInfo" && _Mode == enMode.AddNew)
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
                tcDoctor.SelectedTab = tpDoctorInfo;
                tpDoctorInfo.Enabled = true;
                btnSave.Enabled = true;
                return;
            }

            if (ctrlPersonCardWithFilter1.PersonID == -1)
            {
                MessageBox.Show("Please select a valid person first before moving to doctor info.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            if (clsDoctor.IsDoctorExistForPersonID(ctrlPersonCardWithFilter1.PersonID) && _Mode==enMode.AddNew)
            {
                MessageBox.Show("The selected person is already assigned as a doctor. Please select a different person.", "Duplicate Doctor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tcDoctor.SelectedTab = tpDoctorInfo;
            tpDoctorInfo.Enabled = true;
            btnSave.Enabled = true;
            txtLicenseNo.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtConsultationFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled=!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) ;
        }

        private void TextBoxValidating(object sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox) sender ;
            if(string.IsNullOrEmpty(txt.Text)) {
                e.Cancel = true;
                errorProvider1.SetError(txt, "This field is required.");
            } 
            else {
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
            if(newEnd <= newStart)
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
            if(e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvShifts.Columns[e.ColumnIndex].Name == "Delete")
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
