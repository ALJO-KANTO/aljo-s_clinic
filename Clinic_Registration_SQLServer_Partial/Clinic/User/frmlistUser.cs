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
        private DataTable _dtAllUsers;
        private bool _isApplyingTranslation;
        private string _originalFormTitle;

        public frmlistUser()
        {
            InitializeComponent();
            _originalFormTitle = this.Text;

            // Apply Arabic/RTL settings before the form is displayed to avoid flickering.
            ApplyUsersArabicLayout();
        }

        // Use the same central TranslationClass dictionary as frmListDoctors.
        private string TranslateText(string english)
        {
            if (string.IsNullOrWhiteSpace(english))
                return english;

            string key = english.Trim();

            // Message contains a dynamic user ID, so translate its fixed prefix.
            if (key.StartsWith("Are you sure you want to delete User [", StringComparison.Ordinal))
            {
                string prefix = TranslationClass.Translate(
                    "Are you sure you want to delete User [", TranslationClass.IsArabic);
                return prefix + key.Substring("Are you sure you want to delete User [".Length);
            }

            return TranslationClass.Translate(key, TranslationClass.IsArabic);
        }

        // Convert translated display text back to its English dictionary key so
        // existing filtering logic continues to use the original values.
        private string GetEnglishText(string displayedText)
        {
            if (string.IsNullOrWhiteSpace(displayedText))
                return displayedText;

            string text = displayedText.Trim();
            foreach (KeyValuePair<string, string> item in TranslationClass.sidebarTranslations)
            {
                if (string.Equals(item.Value, text, StringComparison.Ordinal))
                    return item.Key;
            }

            return text;
        }

        private void TranslateControlTree(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label || control is Button || control is CheckBox || control is GroupBox)
                {
                    string english = control.Tag as string;
                    if (string.IsNullOrWhiteSpace(english))
                    {
                        english = control.Text;
                        control.Tag = english;
                    }

                    control.Text = TranslateText(english);
                }

                if (control is ComboBox combo && combo.Items.Count > 0)
                {
                    int selectedIndex = combo.SelectedIndex;
                    List<object> translatedItems = new List<object>();

                    foreach (object item in combo.Items)
                    {
                        string englishItem = GetEnglishText(item == null ? string.Empty : item.ToString());
                        translatedItems.Add(TranslateText(englishItem));
                    }

                    _isApplyingTranslation = true;
                    combo.BeginUpdate();
                    combo.Items.Clear();
                    foreach (object item in translatedItems)
                        combo.Items.Add(item);
                    if (selectedIndex >= 0 && selectedIndex < combo.Items.Count)
                        combo.SelectedIndex = selectedIndex;
                    combo.EndUpdate();
                    _isApplyingTranslation = false;
                }

                if (control is MenuStrip menuStrip)
                    TranslateToolStripItems(menuStrip.Items);

                if (control.HasChildren)
                    TranslateControlTree(control);
            }
        }

        private void TranslateToolStripItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                string english = item.Tag as string;
                if (string.IsNullOrWhiteSpace(english))
                {
                    english = item.Text;
                    item.Tag = english;
                }

                item.Text = TranslateText(english);

                if (item is ToolStripDropDownItem dropDown)
                    TranslateToolStripItems(dropDown.DropDownItems);
            }
        }

        private void ApplyUsersGridArabic()
        {
            bool isArabic = TranslationClass.IsArabic;
            dgvUsers.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;

            foreach (DataGridViewColumn column in dgvUsers.Columns)
            {
                string english = column.Tag as string;
                if (string.IsNullOrWhiteSpace(english))
                {
                    english = GetEnglishText(column.HeaderText);
                    column.Tag = english;
                }
                column.HeaderText = TranslateText(english);
            }
        }

        private void ApplyUsersArabicLayout()
        {
            bool isArabic = TranslationClass.IsArabic;
            RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
            RightToLeftLayout = isArabic;

            Text = TranslateText(_originalFormTitle);
            TranslateControlTree(this);
            ApplyUsersGridArabic();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmlistUser_Load(object sender, EventArgs e)
        {
            _dtAllUsers = clsUser.GetAllUsers();
            dgvUsers.DataSource = _dtAllUsers;

            // تعيين المفتاح الأساسي للـ DataTable لتمكين خاصية Rows.Find من العمل بنجاح
            if (_dtAllUsers.Columns.Contains("UserID"))
            {
                _dtAllUsers.PrimaryKey = new DataColumn[] { _dtAllUsers.Columns["UserID"] };
            }

            cbFilterBy.SelectedIndex = 0; // سيقوم تلقائياً باستدعاء حدث SelectedIndexChanged لتنسيق العناصر
            lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();

            // Keep the original English column names for filtering and translation.
            if (dgvUsers.Columns.Count >= 6)
            {
                dgvUsers.Columns[0].HeaderText = "User ID";
                dgvUsers.Columns[0].Tag = "User ID";
                dgvUsers.Columns[0].Width = 110;

                dgvUsers.Columns[1].HeaderText = "Person ID";
                dgvUsers.Columns[1].Tag = "Person ID";
                dgvUsers.Columns[1].Width = 120;

                dgvUsers.Columns[2].HeaderText = "Full Name";
                dgvUsers.Columns[2].Tag = "Full Name";
                dgvUsers.Columns[2].Width = 350;

                dgvUsers.Columns[3].HeaderText = "UserName";
                dgvUsers.Columns[3].Tag = "UserName";
                dgvUsers.Columns[3].Width = 120;

                dgvUsers.Columns[4].HeaderText = "Role Name";
                dgvUsers.Columns[4].Tag = "Role Name";
                dgvUsers.Columns[4].Width = 140;

                dgvUsers.Columns[5].HeaderText = "Is Active";
                dgvUsers.Columns[5].Tag = "Is Active";
                dgvUsers.Columns[5].Width = 120;
            }

            // The form layout was translated in the constructor.
            // Apply grid headers after the data and columns are ready.
            ApplyUsersGridArabic();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isApplyingTranslation || _dtAllUsers == null)
                return;

            string selectedFilter = GetEnglishText(cbFilterBy.Text);
            bool IsNone = selectedFilter == "None";
            bool IsActive = selectedFilter == "IsActive" || selectedFilter == "Is Active";

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
            if (_isApplyingTranslation || _dtAllUsers == null)
                return;

            string FilterColumn = "IsActive";
            string FilterValue = GetEnglishText(cbIsActive.Text);

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
            if (_isApplyingTranslation || _dtAllUsers == null)
                return;

            string FilterColumn = "";

            switch (GetEnglishText(cbFilterBy.Text))
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
            if (GetEnglishText(cbFilterBy.Text) == "Person ID" || GetEnglishText(cbFilterBy.Text) == "User ID")
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
            if (MessageBox.Show(TranslateText("Are you sure you want to delete User [") + dgvUsers.CurrentRow.Cells[0].Value + "]", TranslateText("Confirm Delete"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                int UserID = (int)dgvUsers.CurrentRow.Cells[0].Value;
                clsUser User = clsUser.Find(UserID);
                if (User == null)
                    return;

                if (User.DeleteUser())
                {
                    MessageBox.Show(TranslateText("User Deleted Successfully."), TranslateText("Successful"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _DataBackToDelete(UserID); // تمرير الـ UserID وليس الـ PersonID
                }
                else
                {
                    MessageBox.Show(TranslateText("User was not deleted because it has data linked to it."), TranslateText("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _DatatBackToAdd(object sender, int UserID)
        {
            DataRow NewUserRow = clsUser.GetUserByID(UserID);

            if (NewUserRow != null)
            {
                _dtAllUsers.UpsertRow(NewUserRow, UserID);
                lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
            }


        }

        private void _DataBackToUpdate(object sender, int UserID)
        {
            DataRow UpdateUserRow = clsUser.GetUserByID(UserID);
            if (UpdateUserRow != null)
            {
                _dtAllUsers.UpsertRow(UpdateUserRow, UserID);
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
            MessageBox.Show(TranslateText("This Feature Is Not Implemented Yet!"), TranslateText("Not Ready!"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(TranslateText("This Feature Is Not Implemented Yet!"), TranslateText("Not Ready!"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}