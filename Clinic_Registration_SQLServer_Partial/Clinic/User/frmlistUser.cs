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

namespace Clinic.User
{
    public partial class frmlistUser : Form
    {
        // سحب البيانات من طبقة الـ Business
        private DataTable _dtAllUsers = clsUser.GetAllUsers();

        public frmlistUser()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _ResetDefault()
        {
            _dtAllUsers = clsUser.GetAllUsers();
            dgvUsers.DataSource = _dtAllUsers;
            lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
        }

        private void frmlistUser_Load(object sender, EventArgs e)
        {
            dgvUsers.DataSource = _dtAllUsers;

            // تعيين المفتاح الأساسي للـ DataTable لتمكين خاصية Rows.Find من العمل بنجاح
            if (_dtAllUsers.Columns.Contains("UserID"))
            {
                _dtAllUsers.PrimaryKey = new DataColumn[] { _dtAllUsers.Columns["UserID"] };
            }

            cbFilterBy.SelectedIndex = 0; // سيقوم تلقائياً باستدعاء حدث SelectedIndexChanged لتنسيق العناصر
            lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();

            // تنسيق أعمدة الـ DataGridView حتى لو لم يكن هناك صفوف
            if (dgvUsers.Rows.Count > 0)
            {
                dgvUsers.Columns[0].HeaderText = "User ID";
                dgvUsers.Columns[0].Width = 110;

                dgvUsers.Columns[1].HeaderText = "Person ID";
                dgvUsers.Columns[1].Width = 120;

                dgvUsers.Columns[2].HeaderText = "Full Name";
                dgvUsers.Columns[2].Width = 350;

                dgvUsers.Columns[3].HeaderText = "UserName";
                dgvUsers.Columns[3].Width = 120;

                dgvUsers.Columns[4].HeaderText = "Role Name";
                dgvUsers.Columns[4].Width = 140;

                dgvUsers.Columns[5].HeaderText = "Is Active";
                dgvUsers.Columns[5].Width = 120;
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool IsNone = cbFilterBy.Text == "None";
            bool IsActive = cbFilterBy.Text == "IsActive" || cbFilterBy.Text == "Is Active";

            txtFilterValue.Visible = !IsActive && !IsNone;
            cbIsActive.Visible = IsActive && !IsNone;

            // إعادة تعيين الفلتر عند تغيير العمود المراد البحث به
            _dtAllUsers.DefaultView.RowFilter = "";
            lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = string.Empty;
                txtFilterValue.Focus();
            }
            else if (cbIsActive.Visible)
            {
                cbIsActive.SelectedIndex = 0;
                cbIsActive.Focus();
            }
            else
            {
                cbFilterBy.Focus();
            }
        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            string FilterColumn = "IsActive";
            string FilterValue = cbIsActive.Text;

            switch (FilterValue)
            {
                case "Yes":
                    FilterValue = "1";
                    break;
                case "No":
                    FilterValue = "0";
                    break;
                default:
                    FilterValue = "All";
                    break;
            }

            if (FilterValue == "All")
                _dtAllUsers.DefaultView.RowFilter = "";
            else
                _dtAllUsers.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, FilterValue);

            lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "User ID":
                    FilterColumn = "UserID";
                    break;
                case "User Name":
                    FilterColumn = "UserName";
                    break;
                case "Person ID":
                    FilterColumn = "PersonID";
                    break;
                case "Full Name":
                    FilterColumn = "FullName";
                    break;
                case "Role Name":
                    FilterColumn = "RoleName";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtAllUsers.DefaultView.RowFilter = "";
                lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
                return;
            }

            if (FilterColumn == "UserID" || FilterColumn == "PersonID")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out _))
                {
                    _dtAllUsers.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
                }
                else
                {
                    _dtAllUsers.DefaultView.RowFilter = "1=0";
                }
            }
            else
            {
                _dtAllUsers.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim().Replace("'", "''"));
            }

            lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.Text == "Person ID" || cbFilterBy.Text == "User ID")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser();
            frm.DataBack += _DatatBackToAdd;
            frm.ShowDialog();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser();
            frm.DataBack += _DatatBackToAdd;
            frm.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int UserID = (int)dgvUsers.CurrentRow.Cells[0].Value;
            frmAddUpdateUser frm = new frmAddUpdateUser(UserID);
            frm.DataBack += _DataBackToUpdate;
            frm.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int UserID = (int)dgvUsers.CurrentRow.Cells[0].Value;
            frmUserInfo frm = new frmUserInfo(UserID);
            frm.ShowDialog();
        }

        private void ChangePasswordtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            int UserID = (int)dgvUsers.CurrentRow.Cells[0].Value;
            frmChangePassword frm = new frmChangePassword(UserID);
            frm.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete User [" + dgvUsers.CurrentRow.Cells[0].Value + "]", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                int UserID = (int)dgvUsers.CurrentRow.Cells[0].Value;
                clsUser User = clsUser.Find(UserID);
                if (User == null)
                    return;

                if (User.DeleteUser())
                {
                    MessageBox.Show("User Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _DataBackToDelete(UserID); // تمرير الـ UserID وليس الـ PersonID
                }
                else
                {
                    MessageBox.Show("User was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _DatatBackToAdd(object sender, int UserID)
        {
            DataRow NewUserRow = clsUser.GetUserByID(UserID);

            if (NewUserRow != null)
            {
               _dtAllUsers.UpsertRow(NewUserRow,UserID);
               lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
            }

            
        }

        private void _DataBackToUpdate(object sender, int UserID)
        {
            DataRow UpdateUserRow = clsUser.GetUserByID(UserID);
            if (UpdateUserRow != null)
            {
               _dtAllUsers.UpsertRow(UpdateUserRow,UserID);
                lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();


            }
           
        }

        // 💡 تم تصحيح اسم المتغير لـ UserID ليتطابق مع الـ Find والـ PrimaryKey الخاص بجدول المستخدمين المفتوح بالشاشة
        private void _DataBackToDelete(int UserID)
        {
            DataRow row = _dtAllUsers.Rows.Find(UserID);
            if (row != null)
            {
                _dtAllUsers.Rows.Remove(row);
                _dtAllUsers.AcceptChanges();
                lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
            }
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature Is Not Implemented Yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature Is Not Implemented Yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}