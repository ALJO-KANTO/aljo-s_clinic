using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmPettyCashCategory : Form
    {
        private int id;

        public frmPettyCashCategory(int _id = 0)
        {
            InitializeComponent();
            this.id = _id;

            if (id != 0)
                this.Text = "Petty Cash Category Edit";
            else
                this.Text = "Petty Cash Category";

            headerUC1.FormText = this.Text;

            ApplyPettyCashCategoryLanguage();
        }

        // Use the application's saved language setting. Do not rely on thread culture.
        private bool IsArabicLanguage()
        {
            string language = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.Equals("ar", StringComparison.OrdinalIgnoreCase) ||
                   language.Equals("Arabic", StringComparison.OrdinalIgnoreCase) ||
                   language.StartsWith("ar-", StringComparison.OrdinalIgnoreCase);
        }

        private string TranslatePettyCashCategoryText(string value, bool arabic)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            switch (value.Trim())
            {
                case "Petty Cash Category":
                    return arabic ? "فئة العهدة النثرية" : "Petty Cash Category";

                case "Petty Cash Category Edit":
                    return arabic ? "تعديل فئة العهدة النثرية" : "Petty Cash Category Edit";

                case "Category":
                    return arabic ? "الفئة" : "Category";

                case "Category Name":
                case "Name":
                    return arabic ? "اسم الفئة" : value;

                case "Description":
                    return arabic ? "الوصف" : "Description";

                case "Save":
                    return arabic ? "حفظ" : "Save";

                case "Close":
                    return arabic ? "إغلاق" : "Close";

                case "Cancel":
                    return arabic ? "إلغاء" : "Cancel";

                case "Please Enter Category Name First":
                    return arabic ? "يرجى إدخال اسم الفئة أولاً" : value;

                case "Category Already Exists. Enter Another Name.":
                    return arabic ? "الفئة موجودة بالفعل. يرجى إدخال اسم آخر." : value;

                default:
                    return value;
            }
        }

        private void ApplyPettyCashCategoryLanguage()
        {
            bool arabic = IsArabicLanguage();

            string formTitle = id != 0
                ? "Petty Cash Category Edit"
                : "Petty Cash Category";

            this.Text = TranslatePettyCashCategoryText(formTitle, arabic);

            if (headerUC1 != null)
                headerUC1.FormText = this.Text;

            this.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;

            TranslatePettyCashCategoryControls(this, arabic);

            // Set these explicitly because the buttons may be custom controls.
            if (btnSave != null)
                btnSave.Text = TranslatePettyCashCategoryText("Save", arabic);

            if (btnClose != null)
                btnClose.Text = TranslatePettyCashCategoryText("Close", arabic);

            if (headerUC1 != null)
                headerUC1.FormText = this.Text;
        }

        private void TranslatePettyCashCategoryControls(Control parent, bool arabic)
        {
            if (parent == null)
                return;

            // Do not translate text entered by the user or values loaded from the database.
            if (!(parent is TextBoxBase) &&
                !(parent is ComboBox) &&
                !(parent is DataGridView))
            {
                string translated = TranslatePettyCashCategoryText(parent.Text, arabic);

                if (!string.IsNullOrEmpty(translated))
                {
                    parent.Text = translated;

                    if (arabic)
                        parent.RightToLeft = RightToLeft.Yes;
                }
            }

            foreach (Control child in parent.Controls)
                TranslatePettyCashCategoryControls(child, arabic);

            if (parent.ContextMenuStrip != null)
                TranslatePettyCashCategoryToolStripItems(parent.ContextMenuStrip.Items, arabic);
        }

        private void TranslatePettyCashCategoryToolStripItems(ToolStripItemCollection items, bool arabic)
        {
            foreach (ToolStripItem item in items)
            {
                item.Text = TranslatePettyCashCategoryText(item.Text, arabic);

                if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
                    TranslatePettyCashCategoryToolStripItems(menuItem.DropDownItems, arabic);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPettyCashCategory_Load(object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                    "select * from tbl_petty_cash_category where id =@id",
                    ClinicDatabase.CreateParameterNew("id", id)))
                {
                    if (reader.Read())
                        txtName.Text = reader["name"].ToString();
                }
            }

            // Reapply after loading so the form title and labels retain the selected language.
            ApplyPettyCashCategoryLanguage();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show(TranslatePettyCashCategoryText(
                    "Please Enter Category Name First", IsArabicLanguage()));
                return;
            }

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                "select * from tbl_petty_cash_category where name = @name",
                ClinicDatabase.CreateParameterNew("name", txtName.Text)))
            {
                if (reader.Read())
                {
                    if (id == 0 || (id != int.Parse(reader["id"].ToString())))
                    {
                        MessageBox.Show(TranslatePettyCashCategoryText(
                            "Category Already Exists. Enter Another Name.", IsArabicLanguage()));
                        return;
                    }
                }
            }

            if (id == 0)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "insert into tbl_petty_cash_category (name,description) values(@name,@description)",
                    ClinicDatabase.CreateParameterNew("name", txtName.Text),
                    ClinicDatabase.CreateParameterNew("description", txtDescription.Text));
                ////Utilities.LogAudit(-1, "Create Petty Cash Category", "Petty Cash Category", 0, "Created Petty Cash Category: " + txtName.Text);
            }
            else
            {
                ClinicDatabase.ExecuteNonQuery(
                    "update tbl_petty_cash_category set name=@name,description=@description where id = @id",
                    ClinicDatabase.CreateParameterNew("id", id),
                    ClinicDatabase.CreateParameterNew("name", txtName.Text));
                ////Utilities.LogAudit(-1, "Update Petty Cash Category", "Petty Cash Category", id, "Updated Petty Cash Category: " + txtName.Text);
            }

            this.Close();
        }
    }
}
