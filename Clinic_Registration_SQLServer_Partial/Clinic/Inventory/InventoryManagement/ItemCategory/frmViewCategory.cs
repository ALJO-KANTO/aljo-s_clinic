using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewCategory : Form
    {
      
        private int id;

        public frmViewCategory( int id=0)
        {
            InitializeComponent();

            this.id = id;
            if (id == 0)
                this.Text = id == 0 ? "New Category" : "Edit Category";
            headerUC1.FormText = this.Text;
            LocalizationManager.LocalizeForm(this);
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Name First");
                return;
            }

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_item_category where name = @name",
            ClinicDatabase.CreateParameterNew("name", txtName.Text)))
                if (reader.Read())
                {
                    if (id == 0 || (id != int.Parse(reader["id"].ToString())))
                    {
                        MessageBox.Show(" Name Already In Use. Enter Another Name.");
                        return;
                    }
                }


            if (id == 0)
            {
                object result = ClinicDatabase.ExecuteScalar("SELECT LPAD(MAX(CAST(code AS UNSIGNED)), 3, '0') FROM tbl_item_category;");
                int nextId = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
                string formattedId = nextId.ToString().PadLeft(3, '0');
                ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_item_category (code, name) VALUES (@code, @name);",
                            ClinicDatabase.CreateParameterNew("@code", formattedId),
                            ClinicDatabase.CreateParameterNew("@name", txtName.Text));
                Utilities.LogAudit(clsGlobal.CurrentUser.UserID, "Add Item Category", "Item Category", nextId, "Added Item Category: " + txtName.Text);
            }
            else
            {
                ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_item_category SET  name = @name WHERE id = @id;",
                        ClinicDatabase.CreateParameterNew("@name", txtName.Text),
                        ClinicDatabase.CreateParameterNew("@id", id));
                Utilities.LogAudit(clsGlobal.CurrentUser.UserID, "Update Item Category", "Item Category", id, "Updated Item Category: " + txtName.Text);
            }

            EventHub.RefreshItemCategory();

            this.Close();
        }

        private void frmViewCategory_Load(object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_item_category where id = @id",
                      ClinicDatabase.CreateParameterNew("id", id)))
                    if(reader.Read())
                        txtName.Text = reader["name"].ToString();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal; // Restore down
            }
            else
            {
                this.WindowState = FormWindowState.Maximized; // Maximize
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void panel6_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
