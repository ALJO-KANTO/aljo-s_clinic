
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewWarehouse : Form
    {
        private int id;

        public frmViewWarehouse(int id = 0)
        {
            InitializeComponent();

            this.id = id;

           // LocalizationManager.LocalizeForm(this);

            ApplyWarehouseLanguage();
        }

        private void ApplyWarehouseLanguage()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ||
                language.IndexOf("arabic", StringComparison.OrdinalIgnoreCase) >= 0;

            headerUC1.FormText = isArabic
                ? (id == 0 ? "مستودع جديد" : "تعديل المستودع")
                : (id == 0 ? "New Warehouse" : "Edit Warehouse");

            btnSave.Text = isArabic
                ? (id == 0 ? "حفظ" : "تحديث")
                : (id == 0 ? "Save" : "Update");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GenerateNextSalesCode()
        {
            string newCode = "WH-0001";

            using (var reader = ClinicDatabase.ExecuteReader(
                "SELECT MAX(CAST(SUBSTRING(code, 4) AS UNSIGNED)) AS lastCode FROM tbl_warehouse"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = "WH-" + code.ToString("D4");
                }
            }

            return newCode;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Name First");
                return;
            }

            //if (cmbEmp.SelectedValue==null)
            //{
            //    MessageBox.Show("Please Enter Manager Name First");
            //    return;
            //}

            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                "select * from tbl_warehouse where name = @name",
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
                int refId = ClinicDatabase.ExecuteNonQuery(
                    @"INSERT INTO `tbl_warehouse` (code, name, emp_id, city, building_name,
                                        account_id, state, created_by, created_date) VALUES (@code, @name, @emp_id,
                                        @city, @building_name, @account_id, 0, @created_by, @created_date);",
                    ClinicDatabase.CreateParameterNew("@code", GenerateNextSalesCode()),
                    ClinicDatabase.CreateParameterNew("@name", txtName.Text),
                    ClinicDatabase.CreateParameterNew("@emp_id", cmbEmp.SelectedValue ?? 0),
                    ClinicDatabase.CreateParameterNew("@city", cmbCity.Text),
                    ClinicDatabase.CreateParameterNew("@building_name", txtBuildingName.Text),
                    ClinicDatabase.CreateParameterNew("@account_id", cmbMainAccount.SelectedValue),
                    ClinicDatabase.CreateParameterNew("@created_by", -1),
                    ClinicDatabase.CreateParameterNew("@created_date", DateTime.Now.Date));

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
                    @"UPDATE `tbl_warehouse` SET name = @name, emp_id = @emp_id, city = @city, 
                                         building_name = @building_name, account_id = @account_id WHERE id = @id;",
                    ClinicDatabase.CreateParameterNew("@name", txtName.Text),
                    ClinicDatabase.CreateParameterNew("@emp_id", cmbEmp.SelectedValue ?? 0),
                    ClinicDatabase.CreateParameterNew("@city", cmbCity.Text),
                    ClinicDatabase.CreateParameterNew("@building_name", txtBuildingName.Text),
                    ClinicDatabase.CreateParameterNew("@account_id", cmbMainAccount.SelectedValue),
                    ClinicDatabase.CreateParameterNew("@id", id));

                //Utilities.LogAudit(
                   
            }

            //.RefreshWarehouse();
            this.Close();
        }

        private void frmViewWarehouse_Load(object sender, EventArgs e)
        {
            BindCombos.PopulateEmployees(cmbEmp);
            BindCombos.PopulateAllLevel4Account(cmbMainAccount);
            BindCombos.PopulateCities(cmbCity, 49);

            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                    "select * from tbl_warehouse where id = @id",
                    ClinicDatabase.CreateParameterNew("id", id)))
                {
                    if (reader.Read() && reader.HasRows)
                    {
                        txtCode.Text = reader["code"].ToString();
                        txtName.Text = reader["name"].ToString();
                        txtBuildingName.Text = reader["building_name"].ToString();
                        cmbCity.Text = reader["city"].ToString();
                        cmbEmp.SelectedValue = int.Parse(reader["emp_id"].ToString());
                        cmbMainAccount.SelectedValue = int.Parse(reader["account_id"].ToString());
                    }
                }
            }
        }

        private void addEmployee_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //frmMain.openChildForm(new frmViewEmployee());
            frmViewEmployee emp = new frmViewEmployee();
            emp.ShowDialog();
        }

        private void addCity_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //(new frmViewCity());
            frmViewCity emp = new frmViewCity();
            emp.ShowDialog();
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

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
