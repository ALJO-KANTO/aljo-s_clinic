using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmViewCategory : Form
    {
        private int id;
        private bool isArabic;

        public frmViewCategory(int id = 0)
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
            this.Text = id == 0
                ? T("New Category", "فئة جديدة")
                : T("Edit Category", "تعديل الفئة");

            headerUC1.FormText = this.Text;

            TranslateControls(this);

            txtName.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;
        }

        private void TranslateControls(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Preserve original input and ComboBox values
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
                        case "Name":
                        case "Name:":
                        case "Category Name":
                        case "Category Name:":
                        case "Item Category Name":
                            ctrl.Text = "اسم الفئة";
                            break;

                        case "Category":
                        case "Item Category":
                            ctrl.Text = "فئة الصنف";
                            break;

                        case "New Category":
                            ctrl.Text = "فئة جديدة";
                            break;

                        case "Edit Category":
                            ctrl.Text = "تعديل الفئة";
                            break;

                        case "Save":
                            ctrl.Text = "حفظ";
                            break;

                        case "Close":
                            ctrl.Text = "إغلاق";
                            break;

                        case "Cancel":
                            ctrl.Text = "إلغاء";
                            break;

                        case "Save & Close":
                            ctrl.Text = "حفظ وإغلاق";
                            break;
                    }
                }

                if (ctrl.HasChildren)
                {
                    TranslateControls(ctrl);
                }
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
                T("Item Category", "فئة الصنف"),
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

        private void btnSave_Click(
            object sender, EventArgs e)
        {
            string categoryName = txtName.Text.Trim();

            // 1. Validate category name
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ShowLocalizedMessage(
                    "Please enter the category name first.",
                    "يرجى إدخال اسم الفئة أولاً.");

                txtName.Focus();
                return;
            }

            // 2. Check duplicate category
            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    @"SELECT * FROM tbl_item_category
                      WHERE name = @name",

                    ClinicDatabase.CreateParameterNew(
                        "name", categoryName)))
            {
                if (reader.Read())
                {
                    if (id == 0 ||
                        id != Convert.ToInt32(reader["id"]))
                    {
                        ShowLocalizedMessage(
                            "Category name is already in use. Enter another name.",
                            "اسم الفئة مستخدم بالفعل. يرجى إدخال اسم آخر.");

                        txtName.Focus();
                        return;
                    }
                }
            }

            // 3. Add new category
            if (id == 0)
            {
                object result =
                    ClinicDatabase.ExecuteScalar(
                        @"SELECT LPAD(
                            MAX(CAST(code AS UNSIGNED)),
                            3, '0')
                          FROM tbl_item_category;");

                int nextId =
                    (result == DBNull.Value || result == null)
                        ? 1
                        : Convert.ToInt32(result) + 1;

                string formattedId =
                    nextId.ToString().PadLeft(3, '0');

                ClinicDatabase.ExecuteNonQuery(
                    @"INSERT INTO tbl_item_category
                      (code, name)
                      VALUES (@code, @name);",

                    ClinicDatabase.CreateParameterNew(
                        "@code", formattedId),

                    ClinicDatabase.CreateParameterNew(
                        "@name", categoryName));

                // Keep audit log values in English
                Utilities.LogAudit(
                    clsGlobal.CurrentUser.UserID,
                    "Add Item Category",
                    "Item Category",
                    nextId,
                    "Added Item Category: " + categoryName);
            }

            // 4. Update existing category
            else
            {
                ClinicDatabase.ExecuteNonQuery(
                    @"UPDATE tbl_item_category
                      SET name = @name
                      WHERE id = @id;",

                    ClinicDatabase.CreateParameterNew(
                        "@name", categoryName),

                    ClinicDatabase.CreateParameterNew(
                        "@id", id));

                // Keep audit log values in English
                Utilities.LogAudit(
                    clsGlobal.CurrentUser.UserID,
                    "Update Item Category",
                    "Item Category",
                    id,
                    "Updated Item Category: " + categoryName);
            }

            // Refresh category list
            EventHub.RefreshItemCategory();

            this.Close();
        }

        private void frmViewCategory_Load(
            object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(
                        @"SELECT * FROM tbl_item_category
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

        private void guna2Button1_Click(
            object sender, EventArgs e)
        {
            if (this.WindowState ==
                FormWindowState.Maximized)
            {
                this.WindowState =
                    FormWindowState.Normal;
            }
            else
            {
                this.WindowState =
                    FormWindowState.Maximized;
            }
        }

        private void guna2Button2_Click(
            object sender, EventArgs e)
        {
            this.WindowState =
                FormWindowState.Minimized;
        }

        private void panel6_Click(
            object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}