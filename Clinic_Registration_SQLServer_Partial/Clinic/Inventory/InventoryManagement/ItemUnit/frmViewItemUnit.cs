
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewItemUnit : Form
    {
        private int id;

        public frmViewItemUnit(int id = 0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);

            this.id = id;

            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            headerUC1.FormText = isArabic
                ? (id == 0
                    ? "المخزون - إضافة وحدة صنف"
                    : "المخزون - تعديل وحدة صنف")
                : (id == 0
                    ? "Inventory - Add Item Unit"
                    : "Inventory - Edit Item Unit");

            headerUC1.ArabicLayout = isArabic;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmViewItemUnit_Load(object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                    "select * from tbl_unit where id =@id",
                    ClinicDatabase.CreateParameterNew("id", id)))
                {
                    reader.Read();
                    txtName.Text = reader["name"].ToString();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Item Unit First");
                return;
            }

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                "select * from tbl_unit where name = @name",
                ClinicDatabase.CreateParameterNew("name", txtName.Text)))
            {
                if (reader.Read())
                {
                    if (id == 0 ||
                        (id != int.Parse(reader["id"].ToString())))
                    {
                        MessageBox.Show(
                            "Item Unit Already Exists. Enter Another Name.");
                        return;
                    }
                }
            }

            if (id == 0)
            {
                int refId = ClinicDatabase.ExecuteNonQuery(
                    "insert into tbl_unit (name) values(@name)",
                    ClinicDatabase.CreateParameterNew("@name", txtName.Text));

                if (refId != 0)
                {
                    //Utilities.LogAudit(
                       
                }
                else
                {
                    string message = Utilities.LastLogError();
                    MessageBox.Show("Error Check your data" + message);
                }
            }
            else
            {
                ClinicDatabase.ExecuteNonQuery(
                    "update tbl_unit set name=@name where id = @id",
                    ClinicDatabase.CreateParameterNew("@id", id),
                    ClinicDatabase.CreateParameterNew("@name", txtName.Text));

                //Utilities.LogAudit(
                   
            }

            //.RefreshItemUnit();
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
