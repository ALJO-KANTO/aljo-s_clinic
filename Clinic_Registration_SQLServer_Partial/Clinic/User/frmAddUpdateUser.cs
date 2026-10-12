using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Clinic.User
{
    public partial class frmAddUpdateUser : Form
    {
        public enum enMode { AddNew = 1, Update = 2 };
        private enMode _Mode = enMode.AddNew;
        private int _UserID = -1;
        private clsUser _User;

        //public delegate void DataBackEventHandler(object sender, int UserID);

        //// Declare an event using the delegate
        //public event DataBackEventHandler DataBack;

        public event Action<object, int > DataBack;
        public frmAddUpdateUser()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdateUser(int userID)
        {
            InitializeComponent();
            _UserID = userID;
            _Mode = enMode.Update;
        }

        private void _FillRolesInComoboBox()
        {
            DataTable dt = clsRole.GetAllRole();
            if (dt != null && dt.Rows.Count > 0)
            {
                cbRoles.DataSource = dt;
                cbRoles.DisplayMember = "RoleName";
                cbRoles.ValueMember = "RoleID";
                cbRoles.SelectedIndex = 0; // تحسين: البدء من أول عنصر دائماً كوضع افتراضي تلافياً للأخطاء
            }
        }

        private void _LoadData()
        {
            _User = clsUser.Find(_UserID);
            if (_User == null)
            {
                MessageBox.Show("No User with ID = " + _UserID, "User Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            ctrlPersonCardWithFilter1.LoadPersonInfo(_User.PersonID);
            ctrlPersonCardWithFilter1.FilterEnabled = false; // قفل الفلتر في التعديل لحماية البيانات

            lblUserID.Text = _UserID.ToString();
            txtUserName.Text = _User.UserName;
            txtPassword.Text = _User.Password;
            txtConfirmPassword.Text = _User.Password;
            chkIsActive.Checked = _User.IsActive;

            // تحسين: الحماية من خطأ عدم إيجاد الـ Role في الـ ComboBox
            int roleIndex = cbRoles.FindString(_User.RoleInfo.RoleName);
            if (roleIndex != -1)
                cbRoles.SelectedIndex = roleIndex;
        }

        private void _ResetDefult()
        {
            _FillRolesInComoboBox();

            tpLoginInfo.Enabled = false;
            btnSave.Enabled = false;

            if (_Mode == enMode.AddNew)
            {
                _User = new clsUser();
                ctrlPersonCardWithFilter1.FilterEnabled = true;
                ctrlPersonCardWithFilter1.FocuseTextBox();
                lblTitle.Text = "Add New User";
            }
            else
            {
                lblTitle.Text = "Update User";
            }

            this.Text = lblTitle.Text;
        }

        private void frmAddUpdateUser_Load(object sender, EventArgs e)
        {
            _ResetDefult();
            if (_Mode == enMode.Update)
                _LoadData();
        }

        private void btnPersonInfoNext_Click(object sender, EventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                btnSave.Enabled = true;
                tpLoginInfo.Enabled = true;
                tcUserInfo.SelectedTab = tcUserInfo.TabPages["tpLoginInfo"];
                return;
            }

            if (ctrlPersonCardWithFilter1.PersonID == -1)
            {
                MessageBox.Show("Please Select a Person", "Select a Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FocuseTextBox();
                return;
            }

            if (clsUser.IsUserExistForPersonID(ctrlPersonCardWithFilter1.PersonID))
            {
                MessageBox.Show("Selected Person already has a user, choose another one.", "Select another Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FocuseTextBox();
            }
            else
            {
                btnSave.Enabled = true;
                tpLoginInfo.Enabled = true;
                tcUserInfo.SelectedTab = tcUserInfo.TabPages["tpLoginInfo"];
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtUserName_Validating(object sender, CancelEventArgs e)
        {
            string userNameInput = txtUserName.Text.Trim();

            if (string.IsNullOrEmpty(userNameInput))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtUserName, "Username cannot be blank");
                return;
            }

            // إصلاح: تم تعديل _User.Name إلى _User.UserName لتطابق خصائص كلاس البزنس
            bool isUsernameDuplicate = (_Mode == enMode.AddNew && clsUser.IsUserExist(userNameInput)) ||
                                       (_Mode == enMode.Update && userNameInput != _User.UserName && clsUser.IsUserExist(userNameInput));

            if (isUsernameDuplicate)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtUserName, "Username is used by another user");
                return;
            }

            errorProvider1.SetError(txtUserName, null);
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPassword, "Password cannot be blank");
            }
            else
            {
                errorProvider1.SetError(txtPassword, null);
            }
        }

        private void txtConfirmPassword_Validating(object sender, CancelEventArgs e)
        {
            if (txtConfirmPassword.Text.Trim() != txtPassword.Text.Trim())
            {
                e.Cancel = true;
                errorProvider1.SetError(txtConfirmPassword, "Password Confirmation does not match Password!");
            }
            else
            {
                errorProvider1.SetError(txtConfirmPassword, null);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid!, put the mouse over the red icon(s) to see the error",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // تحسين أمني: الحماية من قراءة قيمة null من الـ ComboBox
            if (cbRoles.SelectedValue == null)
            {
                MessageBox.Show("Please select a valid role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _User.PersonID = ctrlPersonCardWithFilter1.PersonID;
            _User.UserName = txtUserName.Text.Trim();
            _User.Password = clsGlobal.ComputeHash(txtPassword.Text.Trim());
            _User.IsActive = chkIsActive.Checked;
            _User.RoleID = (int)cbRoles.SelectedValue;

            if (_User.Save())
            {
                lblUserID.Text = _User.UserID.ToString();

                // تحسين: تغيير الحالة وقفل الفلتر فوراً لحماية تماسك البيانات بالذاكرة
                _Mode = enMode.Update;
                ctrlPersonCardWithFilter1.FilterEnabled = false;

                lblTitle.Text = "Update User";
                this.Text = "Update User";
                DataBack?.Invoke(this, _User.UserID);
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // تحسين أمني حرج: منع الموظف من تجاوز شروط اختيار الشخص عبر الضغط اليدوي على التبويبات
        private void tcUserInfo_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage.Name == "tpLoginInfo" && _Mode == enMode.AddNew)
            {
                if (ctrlPersonCardWithFilter1.PersonID == -1 || clsUser.IsUserExistForPersonID(ctrlPersonCardWithFilter1.PersonID))
                {
                    e.Cancel = true; // إلغاء الانتقال وإجبار المستخدم على البقاء في تبويب الشخص
                    MessageBox.Show("Please select a valid person first before moving to login info.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}