using Clinic.global_classes;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Patient
{
    public partial class frmListPatient : Form
    {
        // سحب البيانات من طبقة الـ Business مرة واحدة عند الإقلاع
        private DataTable _dtAllPatients = clsPatient.GetAllPatients();

        public frmListPatient()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _ResetDefault()
        {
            _dtAllPatients = clsPatient.GetAllPatients();

            if (_dtAllPatients == null || _dtAllPatients.Columns.Count == 0)
            {
                return;
            }

            if (_dtAllPatients.Columns.Contains("PatientID"))
            {
                _dtAllPatients.PrimaryKey =
                    new DataColumn[]
                    {
                _dtAllPatients.Columns["PatientID"]
                    };
            }

            dgvPatient.DataSource = _dtAllPatients;
            lblRecordsCount.Text =
                _dtAllPatients.DefaultView.Count.ToString();
        }
        private void frmListPatient_Load(object sender, EventArgs e)
        {
            _ResetDefault();

            if (_dtAllPatients == null ||
                _dtAllPatients.Columns.Count == 0)
                return;

            cbFilterBy.SelectedIndex = 0;
            dgvPatient.AutoSizeColumnsMode =
    DataGridViewAutoSizeColumnsMode.Fill;

            dgvPatient.Columns["PatientID"].FillWeight = 12;
            dgvPatient.Columns["PersonID"].FillWeight = 12;
            dgvPatient.Columns["FullName"].FillWeight = 22;
            dgvPatient.Columns["MedicalHistory"].FillWeight = 25;
            dgvPatient.Columns["EmergencyContact"].FillWeight = 18;
            dgvPatient.Columns["CreatedDate"].FillWeight = 18;
            SetColumn("PatientID", "Patient ID", 100);
            SetColumn("PersonID", "Person ID", 100);
            SetColumn("FullName", "Full Name", 220);
            SetColumn("MedicalHistory", "Medical History", 180);
            SetColumn("CountryName", "Country", 120);
            SetColumn("BloodTypeName", "Blood Type", 100);
            SetColumn("CreatedDate", "Created Date", 150);
            SetColumn("EmergencyContact", "Emergency Contact", 150);
        }

        private void SetColumn(string columnName, string header, int width)
        {
            if (dgvPatient.Columns.Contains(columnName))
            {
                dgvPatient.Columns[columnName].HeaderText = header;
                dgvPatient.Columns[columnName].Width = width;
            }
        }
        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool IsNone = cbFilterBy.Text == "None";
            bool IsBloodType = cbFilterBy.Text == "Blood Type";

            txtFilterValue.Visible = !IsBloodType && !IsNone;
            cbBloodType.Visible = IsBloodType && !IsNone;

            // إعادة تعيين الفلتر عند تغيير العمود المراد البحث به
            _dtAllPatients.DefaultView.RowFilter = "";
            lblRecordsCount.Text = _dtAllPatients.DefaultView.Count.ToString();

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = string.Empty;
                txtFilterValue.Focus();
            }
            else if (cbBloodType.Visible)
            {
                cbBloodType.SelectedIndex = 0; // حقل "All" واجهة افتراضية
                cbBloodType.Focus();
            }
            else
            {
                cbFilterBy.Focus();
            }
        }

        private void cbBloodType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dtAllPatients == null) return;

            string FilterValue = cbBloodType.Text;

            if (FilterValue == "All")
                _dtAllPatients.DefaultView.RowFilter = "";
            else
                _dtAllPatients.DefaultView.RowFilter = string.Format("[BloodTypeName] = '{0}'", FilterValue);

            lblRecordsCount.Text = _dtAllPatients.DefaultView.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";

            // ربط الاختيارات بأسماء حقول الـ SQL الدقيقة بما فيها حقل الـ Country الجديد
            switch (cbFilterBy.Text)
            {
                case "Patient ID":
                    FilterColumn = "PatientID";
                    break;
                case "Person ID":
                    FilterColumn = "PersonID";
                    break;
                case "Full Name":
                    FilterColumn = "FullName";
                    break;
                case "Medical History":
                    FilterColumn = "MedicalHistory";
                    break;
                case "Country":  // الحقل الجديد الذي قمت بإضافته في الـ ComboBox
                    FilterColumn = "CountryName";
                    break;
                case "Emergency Contact":
                    FilterColumn = "EmergencyContact";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtAllPatients.DefaultView.RowFilter = "";
                lblRecordsCount.Text = _dtAllPatients.DefaultView.Count.ToString();
                return;
            }

            if (FilterColumn == "PatientID" || FilterColumn == "PersonID")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out _))
                {
                    _dtAllPatients.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
                }
                else
                {
                    _dtAllPatients.DefaultView.RowFilter = "1=0"; // إخفاء الصفوف عند إدخال نص في حقل رقمي
                }
            }
            else
            {
                if (!_dtAllPatients.Columns.Contains(FilterColumn))
                {
                    return;
                }
                // حماية وتطهير النص المدخل باستخدام Replace لمنع مشاكل الـ (') وتأمين البحث في الحقول النصية ومنها الدول
                _dtAllPatients.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim().Replace("'", "''"));
            }

            lblRecordsCount.Text = _dtAllPatients.DefaultView.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.Text == "Patient ID" || cbFilterBy.Text == "Person ID")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }




        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPatient.CurrentRow == null) return;

            int PatientID = (int)dgvPatient.CurrentRow.Cells[0].Value;
            frmAddUpdatePatient frm = new frmAddUpdatePatient(PatientID);
            frm.DataBack += _DataBackToUpdate;
            frm.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPatient.CurrentRow == null) return;

            int PatientID = (int)dgvPatient.CurrentRow.Cells["PatientID"].Value;
            frmPatientInfo frm = new frmPatientInfo(PatientID);
            frm.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPatient.CurrentRow == null) return;

            int PatientID = (int)dgvPatient.CurrentRow.Cells["PatientID"].Value;

            if (MessageBox.Show("Are you sure you want to delete Patient [" + PatientID + "]?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                clsPatient patient = clsPatient.FindPatientByID(PatientID);
                if (patient == null)
                    return;
                if (patient.DeletePatient())
                {
                    MessageBox.Show("Patient Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _DataBackToDelete(PatientID);
                }
                else
                {
                    MessageBox.Show("Patient was not deleted because it has data linked to it in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ================== الـ DataBack Mechanism لسرعة وكفاءة التحديث بالذاكرة محلياً ==================

        private void _DataBackToAdd(object sender, int PatientID)
        {
            _ResetDefault();
        }

        private void _DataBackToUpdate(object sender, int PatientID)
        {
            DataRow UpdatePatienRow = clsPatient.GetPatientByID(PatientID);
            if (UpdatePatienRow != null)
            {
                _dtAllPatients.UpsertRow(UpdatePatienRow, PatientID);
            }
            lblRecordsCount.Text = _dtAllPatients.DefaultView.Count.ToString();
        }

        private void _DataBackToDelete(int PatientID)
        {
            DataRow row = _dtAllPatients.Rows.Find(PatientID);
            if (row != null)
            {
                _dtAllPatients.Rows.Remove(row);
                _dtAllPatients.AcceptChanges();
                lblRecordsCount.Text = _dtAllPatients.DefaultView.Count.ToString();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePatient frm = new frmAddUpdatePatient();
            frm.DataBack += _DataBackToAdd;
            frm.ShowDialog();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFindPatient frm = new frmFindPatient();
            frm.ShowDialog();
        }

        private void btnAddPatient_Click(object sender, EventArgs e)
        {
            frmAddUpdatePatient frm = new frmAddUpdatePatient();
            frm.DataBack += _DataBackToAdd;
            frm.ShowDialog();
        }
    }
}