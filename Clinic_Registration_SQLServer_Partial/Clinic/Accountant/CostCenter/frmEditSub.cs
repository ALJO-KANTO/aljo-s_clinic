using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmEditSub : Form
    {

        private int id ;
        public frmEditSub(int id)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            this.id = id;
            headerUC1.FormText = this.Text;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtAccount.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Sub Name First");
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_sub_cost_center where name = @name",
                ClinicDatabase.CreateParameterNew("name", txtAccount.Text)))
                if (reader.Read())
                {
                    if ((id != int.Parse(reader["id"].ToString())))
                    {
                        MessageBox.Show("Name Already Exists. Enter Another Name.");
                        return;
                    }
                }
            ClinicDatabase.ExecuteNonQuery("update tbl_sub_cost_center set name=@name where id = @id",
                ClinicDatabase.CreateParameterNew("id", id),
            ClinicDatabase.CreateParameterNew("name", txtAccount.Text));
            ////Utilities.LogAudit(-1, "Update Sub Cost Center", "Sub Cost Center", id, "Updated Sub Cost Center: " + txtAccount.Text);
            //.Refreshlvl1Account();
            //.Refreshlvl2Account();
            this.Close();
        }

        private void frmEditSub_Load(object sender, EventArgs e)
        {
            if (id > 0)
            {
                btnDelete.Enabled = true;
                btnSave.Enabled = UserPermissions.canEdit("Cost Center");
                btnDelete.Enabled = UserPermissions.canDelete("Cost Center");
            }
            else
            {
                btnDelete.Enabled = false;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_sub_cost_center where id = " + id))
            {
                reader.Read();
                txtAccount.Text = reader["name"].ToString();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtAccount.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Sub Name First");
                return;
            }
            object result = ClinicDatabase.ExecuteScalar(@"SELECT COUNT(1) FROM tbl_cost_center_transaction 
                  WHERE cost_center_id = @id", ClinicDatabase.CreateParameterNew("id", id));
            int recordCount = 0;
            if (result != null && result != DBNull.Value)
                recordCount = Convert.ToInt32(result);
            if (recordCount > 0)
            {
                MessageBox.Show("Already used");
                return;
            }
            ClinicDatabase.ExecuteNonQuery("delete from tbl_sub_cost_center where id = @id",
            ClinicDatabase.CreateParameterNew("id", id));
            //.Refreshlvl1Account();
            //.Refreshlvl2Account();
            this.Close();
        }

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
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

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }
    }
}
