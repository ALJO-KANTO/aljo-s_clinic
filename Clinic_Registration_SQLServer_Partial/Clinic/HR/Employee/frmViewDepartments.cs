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
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewDepartments : Form
    {

        private int id;
        public frmViewDepartments(int id = 0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);

            this.id = id;
                    headerUC1.FormText = id == 0 ? " Employee - New Department" : "Employee - Edit Department";
        }

       
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmViewDepartments_Load(object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_departments where id = @id", ClinicDatabase.CreateParameterNew("id", id)))
                    if (reader.Read())
                        txtName.Text = reader["name"].ToString();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Department Name First");
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_departments where name = @name",
                ClinicDatabase.CreateParameterNew("name", txtName.Text)))
                if (reader.Read())
                {
                    if (id == 0 || (id != int.Parse(reader["id"].ToString())))
                    {
                        MessageBox.Show("Department Already Exists. Enter Another Name.");
                        return;
                    }
                }
            if (id == 0)
            {
                int LastInsertId = ClinicDatabase.ExecuteNonQuery("insert into tbl_departments (name) values(@name)",
                    ClinicDatabase.CreateParameterNew("name", txtName.Text));
                if (LastInsertId != 0)
                {
                    ////Utilities.LogAudit(-1, "Add Department", "Department", LastInsertId, "Added Department: " + txtName.Text);
                }
                else
                {
                    string message = Utilities.LastLogError();
                    MessageBox.Show("Error Check your data" + message);
                }
            }
            else
            {
                ClinicDatabase.ExecuteNonQuery("update tbl_departments set name=@name where id = @id",
                    ClinicDatabase.CreateParameterNew("id", id),
              ClinicDatabase.CreateParameterNew("name", txtName.Text));
                ////Utilities.LogAudit(-1, "Update Department", "Department", id, "Updated Department: " + txtName.Text);
            }
            //.RefreshEmployeeDept();
            this.Close();
        }
    }
}
