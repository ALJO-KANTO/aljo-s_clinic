
using Clinic.global_classes;
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

        private DataTable _dtRoles;
        private string _originalFormTitle;

        public event Action<object, int> DataBack;

        public frmAddUpdateUser()
        {
            InitializeComponent();

            _Mode = enMode.AddNew;
            _originalFormTitle = this.Text;

            // Apply Arabic/RTL settings before displaying the form.
            ApplyUserArabicLayout();
        }

        public frmAddUpdateUser(int userID)
        {
            InitializeComponent();

            _UserID = userID;
            _Mode = enMode.Update;
            _originalFormTitle = this.Text;

            // Apply Arabic/RTL settings before displaying the form.
            ApplyUserArabicLayout();
        }

        // Use the central TranslationClass dictionary.
        private string TranslateText(string english)
        {
            if (string.IsNullOrWhiteSpace(english))
                return english;

            string key = english.Trim();

            // Handle the dynamic user ID message.
            if (key.StartsWith("No User with ID = ", StringComparison.Ordinal))
            {
                string prefix = TranslationClass.Translate(
                    "No User with ID = ",
                    TranslationClass.IsArabic
                );

                return prefix + key.Substring("No User with ID = ".Length);
            }

            return TranslationClass.Translate(
                key,
                TranslationClass.IsArabic
            );
        }

        // Translate controls while preserving original English text.
        private void TranslateControlTree(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label ||
                    control is Button ||
                    control is CheckBox ||
                    control is GroupBox)
                {
                    string english = control.Tag as string;

                    if (string.IsNullOrWhiteSpace(english))
                    {
                        english = control.Text;
                        control.Tag = english;
                    }

                    control.Text = TranslateText(english);
                }

                if (control is MenuStrip menuStrip)
                    TranslateToolStripItems(menuStrip.Items);

                if (control.HasChildren)
                    TranslateControlTree(control);
            }
        }

        // Translate menu items if the form contains any.
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

        // Apply RTL in the same way as frmlistUser.
        private void ApplyUserArabicLayout()
        {
            bool isArabic = TranslationClass.IsArabic;

            RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            RightToLeftLayout = isArabic;

            // Translate form controls.
            TranslateControlTree(this);

            // Translate tab captions and apply RTL.
            foreach (TabPage page in tcUserInfo.TabPages)
            {
                string english = page.Tag as string;

                if (string.IsNullOrWhiteSpace(english))
                {
                    english = page.Text;
                    page.Tag = english;
                }

                page.Text = TranslateText(english);

                page.RightToLeft = isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;
            }

            // Translate the form title.
            string title = _Mode == enMode.AddNew
                ? "Add New User"
                : "Update User";

            lblTitle.Text = TranslateText(title);
            this.Text = TranslateText(title);
        }

        // Fill roles without translating dropdown values.
        // Original RoleID and RoleName values remain unchanged.
        private void _FillRolesInComoboBox()
        {
            _dtRoles = clsRole.GetAllRole();

            if (_dtRoles != null && _dtRoles.Rows.Count > 0)
            {
                cbRoles.DataSource = _dtRoles;
                cbRoles.DisplayMember = "RoleName";
                cbRoles.ValueMember = "RoleID";
                cbRoles.SelectedIndex = 0;
            }
        }

        private void _LoadData()
        {
            _User = clsUser.Find(_UserID);

            if (_User == null)
            {
                MessageBox.Show(
                    TranslateText("No User with ID = " + _UserID),
                    TranslateText("User Not Found"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );

                this.Close();
                return;
            }

            ctrlPersonCardWithFilter1.LoadPersonInfo(_User.PersonID);

            ctrlPersonCardWithFilter1.FilterEnabled = false;

            lblUserID.Text = _UserID.ToString();

            txtUserName.Text = _User.UserName;
            txtPassword.Text = _User.Password;
            txtConfirmPassword.Text = _User.Password;

            chkIsActive.Checked = _User.IsActive;

            // Find the role using its original English name.
            // Dropdown values remain in English.
            if (_User.RoleInfo != null)
            {
                int roleIndex = cbRoles.FindString(
                    _User.RoleInfo.RoleName
                );

                if (roleIndex != -1)
                    cbRoles.SelectedIndex = roleIndex;
            }
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

            ApplyUserArabicLayout();
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

                tcUserInfo.SelectedTab =
                    tcUserInfo.TabPages["tpLoginInfo"];

                return;
            }

            if (ctrlPersonCardWithFilter1.PersonID == -1)
            {
                MessageBox.Show(
                    TranslateText("Please Select a Person"),
                    TranslateText("Select a Person"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                ctrlPersonCardWithFilter1.FocuseTextBox();
                return;
            }

            if (clsUser.IsUserExistForPersonID(
                ctrlPersonCardWithFilter1.PersonID))
            {
                MessageBox.Show(
                    TranslateText(
                        "Selected Person already has a user, choose another one."
                    ),
                    TranslateText("Select another Person"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                ctrlPersonCardWithFilter1.FocuseTextBox();
            }
            else
            {
                btnSave.Enabled = true;
                tpLoginInfo.Enabled = true;

                tcUserInfo.SelectedTab =
                    tcUserInfo.TabPages["tpLoginInfo"];
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtUserName_Validating(
            object sender,
            CancelEventArgs e)
        {
            string userNameInput = txtUserName.Text.Trim();

            if (string.IsNullOrEmpty(userNameInput))
            {
                e.Cancel = true;

                errorProvider1.SetError(
                    txtUserName,
                    TranslateText("Username cannot be blank")
                );

                return;
            }

            bool isUsernameDuplicate =
                (_Mode == enMode.AddNew &&
                 clsUser.IsUserExist(userNameInput)) ||

                (_Mode == enMode.Update &&
                 userNameInput != _User.UserName &&
                 clsUser.IsUserExist(userNameInput));

            if (isUsernameDuplicate)
            {
                e.Cancel = true;

                errorProvider1.SetError(
                    txtUserName,
                    TranslateText("Username is used by another user")
                );

                return;
            }

            errorProvider1.SetError(txtUserName, null);
        }

        private void txtPassword_Validating(
            object sender,
            CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                e.Cancel = true;

                errorProvider1.SetError(
                    txtPassword,
                    TranslateText("Password cannot be blank")
                );
            }
            else
            {
                errorProvider1.SetError(txtPassword, null);
            }
        }

        private void txtConfirmPassword_Validating(
            object sender,
            CancelEventArgs e)
        {
            if (txtConfirmPassword.Text.Trim() !=
                txtPassword.Text.Trim())
            {
                e.Cancel = true;

                errorProvider1.SetError(
                    txtConfirmPassword,
                    TranslateText(
                        "Password Confirmation does not match Password!"
                    )
                );
            }
            else
            {
                errorProvider1.SetError(
                    txtConfirmPassword,
                    null
                );
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show(
                    TranslateText(
                        "Some fields are not valid!, put the mouse over the red icon(s) to see the error"
                    ),
                    TranslateText("Validation Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            if (cbRoles.SelectedValue == null)
            {
                MessageBox.Show(
                    TranslateText("Please select a valid role."),
                    TranslateText("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            _User.PersonID =
                ctrlPersonCardWithFilter1.PersonID;

            _User.UserName =
                txtUserName.Text.Trim();

            _User.Password =
                clsGlobal.ComputeHash(txtPassword.Text.Trim());

            _User.IsActive =
                chkIsActive.Checked;

            _User.RoleID =
                Convert.ToInt32(cbRoles.SelectedValue);

            if (_User.Save())
            {
                lblUserID.Text =
                    _User.UserID.ToString();

                _Mode = enMode.Update;

                ctrlPersonCardWithFilter1.FilterEnabled = false;

                lblTitle.Text = "Update User";
                this.Text = TranslateText("Update User");

                DataBack?.Invoke(this, _User.UserID);

                MessageBox.Show(
                    TranslateText("Data Saved Successfully."),
                    TranslateText("Saved"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    TranslateText(
                        "Error: Data Is not Saved Successfully."
                    ),
                    TranslateText("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void tcUserInfo_Selecting(
            object sender,
            TabControlCancelEventArgs e)
        {
            if (e.TabPage.Name == "tpLoginInfo" &&
                _Mode == enMode.AddNew)
            {
                if (ctrlPersonCardWithFilter1.PersonID == -1 ||
                    clsUser.IsUserExistForPersonID(
                        ctrlPersonCardWithFilter1.PersonID))
                {
                    e.Cancel = true;

                    MessageBox.Show(
                        TranslateText(
                            "Please select a valid person first before moving to login info."
                        ),
                        TranslateText("Selection Required"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
