using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewVendorCategory : Form
    {
        private int id;
        public frmViewVendorCategory( int id)
        {
            InitializeComponent();
            this.id = id;
            this.Text = id != 0 ? "Vendors - Edit Vendors Category" : "Vendors - New Vendors Category";
            headerUC1.FormText = this.Text;
            LocalizationManager.LocalizeForm(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmNewCustomer_Load(object sender, EventArgs e)
        {
            if (id != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_vendor_category where id =@id", ClinicDatabase.CreateParameterNew("id", id)))
                {
                    reader.Read();
                    txtName.Text = reader["name"].ToString();
                }
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            string categoryName = txtName.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Please enter a category name first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id from tbl_vendor_category where name = @name",
                ClinicDatabase.CreateParameterNew("name", txtName.Text)))
                if (reader.Read())
                {
                    int existingId = Convert.ToInt32(reader["id"]);
                    if (id == 0 || id != existingId)
                    {
                        MessageBox.Show("Category already exists. Please enter another name.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            if (id == 0)
            {
                int refId = ClinicDatabase.ExecuteNonQuery("insert into tbl_vendor_category (name) values(@name)",
                    ClinicDatabase.CreateParameterNew("name", txtName.Text));
                if (refId != 0)
                {
                }
                else
                {
                    string message = Utilities.LastLogError();
                    MessageBox.Show("Error Check your data" + message);
                }
            }
            else
                ClinicDatabase.ExecuteNonQuery("update tbl_vendor_category set name=@name where id = @id",
                    ClinicDatabase.CreateParameterNew("id", id),
              ClinicDatabase.CreateParameterNew("name", txtName.Text));
            MessageBox.Show("Category saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            EventHub.RefreshVendorCategory();
            this.Close();
        }
    }
}
