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
    public partial class frmViewPosition : Form
    {

        private int id;
        public frmViewPosition( int id = 0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);

            this.id = id;

            headerUC1.FormText = id == 0 ? "Employee - New Position" : "Employee - Edit Position";
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmViewPosition_Load(object sender, EventArgs e)
        {
            BindCombos.PopulateDepartments(cmbDepartments);
            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_position where id =@id", ClinicDatabase.CreateParameterNew("id", id)))
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
                MessageBox.Show("Please Enter Department Name First");
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_position where name = @name",
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
                int lastId = ClinicDatabase.ExecuteNonQuery("insert into tbl_position (name,department_id) values(@name,@department_id)",
                    ClinicDatabase.CreateParameterNew("name", txtName.Text),
                    ClinicDatabase.CreateParameterNew("department_id", cmbDepartments.SelectedValue.ToString()));
                if (lastId != 0)
                {
                    ////Utilities.LogAudit(-1, "Create Position", "Position", lastId, "Created Position: " + txtName.Text);
                }
                else
                {
                    string message = Utilities.LastLogError();
                    MessageBox.Show("Error Check your data" + message);
                }
            }
            else
            {
                ClinicDatabase.ExecuteNonQuery("update tbl_position set name=@name where id = @id",
                    ClinicDatabase.CreateParameterNew("id", id),
                    ClinicDatabase.CreateParameterNew("name", txtName.Text));
                ////Utilities.LogAudit(-1, "Update Position", "Position", id, "Updated Position: " + txtName.Text);
            }
            string key = "select * from tbl_position where department_id=";
            BindCombos.ClearCache(key);
            //.RefreshEmployeePosition();
            this.Close();
        }
    }
}
