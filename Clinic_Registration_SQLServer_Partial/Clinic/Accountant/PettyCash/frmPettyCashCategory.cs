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
    public partial class frmPettyCashCategory : Form
    {
        private int id;
        public frmPettyCashCategory(int _id =0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            this.id = _id;
            if (id != 0)
                this.Text = "Petty Cash Category Edit";
            else
                this.Text = "Petty Cash Category";

            headerUC1.FormText = this.Text;
        }
       
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPettyCashCategory_Load(object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_petty_cash_category where id =@id", ClinicDatabase.CreateParameterNew("id", id)))
                    if (reader.Read())
                        txtName.Text = reader["name"].ToString();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Category Name First");
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_petty_cash_category where name = @name",
                ClinicDatabase.CreateParameterNew("name", txtName.Text)))
                if (reader.Read())
                {
                    if (id == 0 || (id != int.Parse(reader["id"].ToString())))
                    {
                        MessageBox.Show("Category Already Exists. Enter Another Name.");
                        return;
                    }
                }
            if (id == 0)
            {
                ClinicDatabase.ExecuteNonQuery("insert into tbl_petty_cash_category (name,description) values(@name,@description)",
                    ClinicDatabase.CreateParameterNew("name", txtName.Text),
                    ClinicDatabase.CreateParameterNew("description", txtDescription.Text));
                ////Utilities.LogAudit(-1, "Create Petty Cash Category", "Petty Cash Category", 0, "Created Petty Cash Category: " + txtName.Text);
            }
            else
            {
                ClinicDatabase.ExecuteNonQuery("update tbl_petty_cash_category set name=@name,description=@description where id = @id",
                    ClinicDatabase.CreateParameterNew("id", id),
                ClinicDatabase.CreateParameterNew("name", txtName.Text));
                ////Utilities.LogAudit(-1, "Update Petty Cash Category", "Petty Cash Category", id, "Updated Petty Cash Category: " + txtName.Text);
            }
            this.Close();
        }
    }
}
