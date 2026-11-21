using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmViewDepartments : Form
    {
        private int id;
        private bool isArabic;

        public frmViewDepartments(int id = 0)
        {
            InitializeComponent();

            this.id = id;
            isArabic = TranslationClass.IsArabic;

            ApplyLanguage();
        }

        private string T(string english, string arabic)
        {
            return isArabic ? arabic : english;
        }

        private void ApplyLanguage()
        {
            headerUC1.FormText = id == 0
                ? T("Employee - New Department",
                    "الموظف - قسم جديد")
                : T("Employee - Edit Department",
                    "الموظف - تعديل القسم");

            this.Text = headerUC1.FormText;

            TranslateControls(this);

            txtName.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;
        }

        private void TranslateControls(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Do not translate input or ComboBox values
                if (ctrl is ComboBox ||
                    ctrl is TextBoxBase ||
                    ctrl is Guna.UI2.WinForms.Guna2TextBox)
                {
                    continue;
                }

                if (isArabic)
                {
                    switch (ctrl.Text.Trim())
                    {
                        case "Department Name":
                        case "Department Name:":
                            ctrl.Text = "اسم القسم";
                            break;

                        case "Save":
                            ctrl.Text = "حفظ";
                            break;

                        case "Close":
                            ctrl.Text = "إغلاق";
                            break;
                    }
                }

                if (ctrl.HasChildren)
                    TranslateControls(ctrl);
            }
        }

        private void ShowLocalizedMessage(
            string english,
            string arabic,
            MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            MessageBox.Show(
                this,
                T(english, arabic),
                T("Departments", "الأقسام"),
                MessageBoxButtons.OK,
                icon,
                MessageBoxDefaultButton.Button1,
                isArabic
                    ? MessageBoxOptions.RightAlign |
                      MessageBoxOptions.RtlReading
                    : (MessageBoxOptions)0);
        }

        private void btnClose_Click(
            object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmViewDepartments_Load(
            object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(
                        @"SELECT * FROM tbl_departments
                          WHERE id = @id",
                        ClinicDatabase.CreateParameterNew(
                            "id", id)))
                {
                    if (reader.Read())
                    {
                        txtName.Text =
                            reader["name"].ToString();
                    }
                }
            }
        }

        private void btnSave_Click(
            object sender, EventArgs e)
        {
            string departmentName = txtName.Text.Trim();

            // Validate department name
            if (string.IsNullOrWhiteSpace(departmentName))
            {
                ShowLocalizedMessage(
                    "Please enter the department name first.",
                    "يرجى إدخال اسم القسم أولاً.");

                txtName.Focus();
                return;
            }

            // Check duplicate department
            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    @"SELECT * FROM tbl_departments
                      WHERE name = @name",
                    ClinicDatabase.CreateParameterNew(
                        "name", departmentName)))
            {
                if (reader.Read())
                {
                    if (id == 0 ||
                        id != Convert.ToInt32(reader["id"]))
                    {
                        ShowLocalizedMessage(
                            "Department already exists. Enter another name.",
                            "القسم موجود بالفعل. يرجى إدخال اسم آخر.");

                        txtName.Focus();
                        return;
                    }
                }
            }

            if (id == 0)
            {
                // Insert new department
                int LastInsertId =
                    ClinicDatabase.ExecuteNonQuery(
                        @"INSERT INTO tbl_departments (name)
                          VALUES (@name)",
                        ClinicDatabase.CreateParameterNew(
                            "name", departmentName));

                if (LastInsertId == 0)
                {
                    string message = Utilities.LastLogError();

                    ShowLocalizedMessage(
                        "Error. Please check your data.\n" + message,
                        "حدث خطأ. يرجى التحقق من البيانات.\n" + message,
                        MessageBoxIcon.Error);

                    return;
                }
            }
            else
            {
                // Update existing department
                ClinicDatabase.ExecuteNonQuery(
                    @"UPDATE tbl_departments
                      SET name = @name
                      WHERE id = @id",
                    ClinicDatabase.CreateParameterNew(
                        "id", id),
                    ClinicDatabase.CreateParameterNew(
                        "name", departmentName));
            }

            this.Close();
        }
    }
}