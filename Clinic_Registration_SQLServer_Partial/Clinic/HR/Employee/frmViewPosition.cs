using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmViewPosition : Form
    {
        private int id;
        private bool isArabic;

        public frmViewPosition(int id = 0)
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
                ? T("Employee - New Position",
                    "الموظف - منصب جديد")
                : T("Employee - Edit Position",
                    "الموظف - تعديل المنصب");

            this.Text = headerUC1.FormText;

            TranslateControls(this);

            txtName.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            // Keep ComboBox values unchanged
            cmbDepartments.RightToLeft = RightToLeft.No;
        }

        private void TranslateControls(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Do not translate ComboBox or input values
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
                        case "Departments":
                        case "Departments:":
                        case "Department":
                        case "Department:":
                            ctrl.Text = "القسم";
                            break;

                        case "Position Name":
                        case "Position Name:":
                            ctrl.Text = "اسم المنصب";
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
                T("Positions", "المناصب الوظيفية"),
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

        private void frmViewPosition_Load(
            object sender, EventArgs e)
        {
            // Keep original department names
            BindCombos.PopulateDepartments(cmbDepartments);

            if (id != 0)
            {
                using (MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(
                        @"SELECT * FROM tbl_position
                          WHERE id = @id",
                        ClinicDatabase.CreateParameterNew(
                            "id", id)))
                {
                    if (reader.Read())
                    {
                        txtName.Text =
                            reader["name"].ToString();

                        // Restore saved department
                        if (reader["department_id"] != DBNull.Value)
                        {
                            cmbDepartments.SelectedValue =
                                Convert.ToInt32(
                                    reader["department_id"]);
                        }
                    }
                }
            }
        }

        private void btnSave_Click(
            object sender, EventArgs e)
        {
            string positionName = txtName.Text.Trim();

            // 1. Validate position name
            if (string.IsNullOrWhiteSpace(positionName))
            {
                ShowLocalizedMessage(
                    "Please enter the position name first.",
                    "يرجى إدخال اسم المنصب أولاً.");

                txtName.Focus();
                return;
            }

            // 2. Validate department selection
            if (cmbDepartments.SelectedValue == null ||
                !int.TryParse(
                    Convert.ToString(cmbDepartments.SelectedValue),
                    out int departmentId) ||
                departmentId <= 0)
            {
                ShowLocalizedMessage(
                    "Please select a department first.",
                    "يرجى اختيار القسم أولاً.");

                cmbDepartments.Focus();
                return;
            }

            // 3. Check duplicate position
            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    @"SELECT * FROM tbl_position
                      WHERE name = @name",
                    ClinicDatabase.CreateParameterNew(
                        "name", positionName)))
            {
                if (reader.Read())
                {
                    if (id == 0 ||
                        id != Convert.ToInt32(reader["id"]))
                    {
                        ShowLocalizedMessage(
                            "Position already exists. Enter another name.",
                            "المنصب موجود بالفعل. يرجى إدخال اسم آخر.");

                        txtName.Focus();
                        return;
                    }
                }
            }

            // 4. Insert new position
            if (id == 0)
            {
                int lastId =
                    ClinicDatabase.ExecuteNonQuery(
                        @"INSERT INTO tbl_position
                          (name, department_id)
                          VALUES (@name, @department_id)",

                        ClinicDatabase.CreateParameterNew(
                            "name", positionName),

                        ClinicDatabase.CreateParameterNew(
                            "department_id", departmentId));

                if (lastId == 0)
                {
                    string message = Utilities.LastLogError();

                    ShowLocalizedMessage(
                        "Error. Please check your data.\n" + message,
                        "حدث خطأ. يرجى التحقق من البيانات.\n" + message,
                        MessageBoxIcon.Error);

                    return;
                }
            }

            // 5. Update existing position
            else
            {
                ClinicDatabase.ExecuteNonQuery(
                    @"UPDATE tbl_position
                      SET name = @name,
                          department_id = @department_id
                      WHERE id = @id",

                    ClinicDatabase.CreateParameterNew(
                        "id", id),

                    ClinicDatabase.CreateParameterNew(
                        "name", positionName),

                    ClinicDatabase.CreateParameterNew(
                        "department_id", departmentId));
            }

            // 6. Clear position cache
            string key =
                "select * from tbl_position where department_id=";

            BindCombos.ClearCache(key);

            this.Close();
        }
    }
}