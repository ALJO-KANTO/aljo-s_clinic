using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmViewFixedAssetsCategory : Form
    {
        private int id;
        public frmViewFixedAssetsCategory(int id = 0)
        {
            InitializeComponent();

            ApplyModernDesign();

           // LocalizationManager.LocalizeForm(this);

            this.id = id;

            headerUC1.FormText =
                id == 0
                    ? "Fixed Asset Category - New"
                    : "Fixed Asset Category - Edit";
        }
        private void ApplyModernDesign()
        {
            // =====================================
            // FORM
            // =====================================
            this.BackColor =
                Color.FromArgb(248, 249, 252);

            // Style all controls recursively
            foreach (Control control in this.Controls)
            {
                StyleControlRecursive(control);
            }

            // =====================================
            // HEADER
            // =====================================
            headerUC1.FormText =
                id == 0
                    ? "Fixed Asset Category - New"
                    : "Fixed Asset Category - Edit";

            // =====================================
            // CATEGORY NAME
            // =====================================
            cmbCategoryName.Font =
                new Font(
                    "Segoe UI",
                    9.5F);

            // =====================================
            // DELETE
            // =====================================
            if (btnDelete is Guna.UI2.WinForms.Guna2Button deleteButton)
            {
                deleteButton.Text = "Delete";

                deleteButton.FillColor =
                    Color.FromArgb(255, 246, 247);

                deleteButton.ForeColor =
                    Color.FromArgb(185, 55, 72);

                deleteButton.BorderColor =
                    Color.FromArgb(235, 195, 201);

                deleteButton.BorderThickness = 1;
                deleteButton.BorderRadius = 8;

                deleteButton.Font =
                    new Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Bold);

                deleteButton.HoverState.FillColor =
                    Color.FromArgb(252, 233, 236);
            }

            // =====================================
            // SAVE
            // =====================================
            if (btnSave is Guna.UI2.WinForms.Guna2Button saveButton)
            {
                saveButton.Text = "Save";

                saveButton.FillColor =
                    Color.FromArgb(103, 58, 183);

                saveButton.ForeColor =
                    Color.White;

                saveButton.BorderThickness = 0;
                saveButton.BorderRadius = 8;

                saveButton.Font =
                    new Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Bold);

                saveButton.HoverState.FillColor =
                    Color.FromArgb(87, 48, 160);
            }

            // =====================================
            // CLOSE
            // =====================================
            if (btnClose is Guna.UI2.WinForms.Guna2Button closeButton)
            {
                closeButton.Text = "Close";

                closeButton.FillColor =
                    Color.White;

                closeButton.ForeColor =
                    Color.FromArgb(60, 53, 78);

                closeButton.BorderColor =
                    Color.FromArgb(207, 203, 217);

                closeButton.BorderThickness = 1;
                closeButton.BorderRadius = 8;

                closeButton.Font =
                    new Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Bold);

                closeButton.HoverState.FillColor =
                    Color.FromArgb(245, 243, 249);
            }
        }
        private void StyleControlRecursive(Control control)
        {
            if (control == null)
                return;

            // =====================================
            // TEXTBOX
            // =====================================
            if (control is Guna.UI2.WinForms.Guna2TextBox textBox)
            {
                textBox.BorderRadius = 7;

                textBox.BorderColor =
                    Color.FromArgb(210, 213, 222);

                textBox.FocusedState.BorderColor =
                    Color.FromArgb(103, 58, 183);

                textBox.HoverState.BorderColor =
                    Color.FromArgb(165, 153, 193);

                textBox.FillColor =
                    textBox.Enabled
                        ? Color.White
                        : Color.FromArgb(242, 243, 246);

                textBox.ForeColor =
                    Color.FromArgb(49, 45, 61);

                textBox.Font =
                    new Font("Segoe UI", 9F);
            }

            // =====================================
            // COMBOBOX
            // =====================================
            else if (control is Guna.UI2.WinForms.Guna2ComboBox comboBox)
            {
                comboBox.BorderRadius = 7;

                comboBox.BorderColor =
                    Color.FromArgb(210, 213, 222);

                comboBox.FocusedColor =
                    Color.FromArgb(103, 58, 183);

                comboBox.FillColor = Color.White;

                comboBox.ForeColor =
                    Color.FromArgb(49, 45, 61);

                comboBox.Font =
                    new Font("Segoe UI", 9F);

                // Prevent huge blank rows in dropdown
                comboBox.ItemHeight = 30;

                // Enough width for account code + Arabic name
                comboBox.DropDownWidth =
                    Math.Max(comboBox.Width, 390);

                // Account code + Arabic name displays better LTR
                comboBox.RightToLeft = RightToLeft.No;
            }

            // =====================================
            // LABELS
            // =====================================
            else if (control is Label label)
            {
                label.ForeColor =
                    Color.FromArgb(62, 56, 78);

                label.Font =
                    new Font(
                        "Segoe UI",
                        8.5F,
                        FontStyle.Regular);
            }

            foreach (Control child in control.Controls)
            {
                StyleControlRecursive(child);
            }
        }
        private void FixAccountComboDisplay(
    Guna.UI2.WinForms.Guna2ComboBox combo)
        {
            if (combo == null)
                return;

            combo.RightToLeft = RightToLeft.No;

            combo.Font =
                new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Regular);

            combo.ForeColor =
                Color.FromArgb(49, 45, 61);

            combo.FillColor = Color.White;

            combo.ItemHeight = 30;

            combo.DropDownWidth =
                Math.Max(combo.Width, 390);

            combo.Invalidate();
            combo.Refresh();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (id <= 0)
            {
                MessageBox.Show(
                    "Please select an existing category first.",
                    "Fixed Asset Category",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            DialogResult result =
                MessageBox.Show(
                    "Are you sure you want to delete this category?",
                    "Delete Category",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            ClinicDatabase.ExecuteNonQuery(
                @"DELETE FROM tbl_fixed_assets_category
          WHERE id = @id;",
                ClinicDatabase.CreateParameterNew("id", id));

            id = 0;

            //.RefreshFixedAssetsCategory();

            MessageBox.Show(
                "Category deleted successfully.",
                "Fixed Asset Category",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmViewFixedAssetsCategory_Load(
            object sender,
            EventArgs e)
        {
            // ==========================================
            // ACCOUNT COMBOS
            // ==========================================
            BindCombos.PopulateAllLevel4Account(
                cmbAssetsAccount);

            BindCombos.PopulateAllLevel4Account(
                cmbDepreciationAccount);

            BindCombos.PopulateAllLevel4Account(
                cmbExpenceAccount);

            // Fix Guna dropdown rendering
            FixAccountComboDisplay(
                cmbAssetsAccount);

            FixAccountComboDisplay(
                cmbDepreciationAccount);

            FixAccountComboDisplay(
                cmbExpenceAccount);

            // ==========================================
            // FIXED ASSET CATEGORY
            // ==========================================
            DataTable dt =
                ClinicDatabase.ExecuteDataTable(
                    "select * from tbl_fixed_assets_category");

            cmbCategoryName.ValueMember = "id";
            cmbCategoryName.DisplayMember = "category_name";
            cmbCategoryName.DataSource = dt;
            cmbCategoryName.SelectedIndex = -1;

            if (dt.Rows.Count > 0)
            {
                AutoCompleteStringCollection autoSource =
                    new AutoCompleteStringCollection();

                foreach (DataRow row in dt.Rows)
                {
                    autoSource.Add(
                        row["category_name"].ToString());
                }

                cmbCategoryName.AutoCompleteCustomSource =
                    autoSource;
            }

            // ==========================================
            // EDIT MODE
            // ==========================================
            if (id != 0)
            {
                BindData();
            }
        }
        private void BindData()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_fixed_assets_category where id =@id", ClinicDatabase.CreateParameterNew("id", id)))
            {
                reader.Read();
                cmbCategoryName.Text = reader["category_name"].ToString();

                if (reader["assets_account_id"] != DBNull.Value && int.Parse(reader["assets_account_id"].ToString()) > 0)
                    cmbAssetsAccount.SelectedValue = int.Parse(reader["assets_account_id"].ToString());

                if (reader["depreciation_account_id"] != DBNull.Value && int.Parse(reader["depreciation_account_id"].ToString()) > 0)
                    cmbDepreciationAccount.SelectedValue = int.Parse(reader["depreciation_account_id"].ToString());

                if (reader["expence_account_id"] != DBNull.Value && int.Parse(reader["expence_account_id"].ToString()) > 0)
                    cmbExpenceAccount.SelectedValue = int.Parse(reader["expence_account_id"].ToString());

            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCategoryName.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Category Name First");
                return;
            }
            using(MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_fixed_assets_category where category_name = @category_name",
                ClinicDatabase.CreateParameterNew("category_name", cmbCategoryName.Text)))
            if (reader.Read())
            {
                if (id == 0 || (id != int.Parse(reader["id"].ToString())))
                {
                    reader.Dispose();
                    MessageBox.Show("Category Already Exists. Enter Another Name.");
                    return;
                }
            }
            if (id == 0)
                ClinicDatabase.ExecuteNonQuery(@"
                        INSERT INTO tbl_fixed_assets_category (
                            category_name, assets_account_id, depreciation_account_id, expence_account_id
                        ) VALUES (
                            @category_name, @assets_account_id, @depreciation_account_id, @expence_account_id
                        );",
                        ClinicDatabase.CreateParameterNew("category_name", cmbCategoryName.Text),
                        ClinicDatabase.CreateParameterNew("assets_account_id", cmbAssetsAccount.SelectedValue),
                        ClinicDatabase.CreateParameterNew("depreciation_account_id", cmbDepreciationAccount.SelectedValue),
                        ClinicDatabase.CreateParameterNew("expence_account_id", cmbExpenceAccount.SelectedValue)
                );
            else
                ClinicDatabase.ExecuteNonQuery(@"
                    UPDATE tbl_fixed_assets_category 
                    SET 
                        category_name = @category_name, 
                        assets_account_id = @assets_account_id, 
                        depreciation_account_id = @depreciation_account_id, 
                        expence_account_id = @expence_account_id 
                    WHERE id = @id;",
                    ClinicDatabase.CreateParameterNew("id", id),
                    ClinicDatabase.CreateParameterNew("category_name", cmbCategoryName.Text),
                    ClinicDatabase.CreateParameterNew("assets_account_id", cmbAssetsAccount.SelectedValue),
                    ClinicDatabase.CreateParameterNew("depreciation_account_id", cmbDepreciationAccount.SelectedValue),
                    ClinicDatabase.CreateParameterNew("expence_account_id", cmbExpenceAccount.SelectedValue)
                );

            //.RefreshFixedAssetsCategory();
            this.Close();
        }


        private void cmbAssetsAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAssetsAccount.SelectedValue == null)
            {
                txtAccountCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbAssetsAccount.SelectedValue.ToString()))
                if (reader.Read())
                    txtAccountCode.Text = reader["code"].ToString();
                else
                    txtAccountCode.Text = "";

        }

        private void cmbDepreciationAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDepreciationAccount.SelectedValue == null)
            {
                txtCreditAccountCode.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbDepreciationAccount.SelectedValue.ToString()))
                if (reader.Read())
                    txtCreditAccountCode.Text = reader["code"].ToString();
                else
                    txtCreditAccountCode.Text = "";

        }

        private void cmbExpenceAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbExpenceAccount.SelectedValue == null)
            {
                txtExpenceAccount.Text = "";
                return;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where id = " + cmbExpenceAccount.SelectedValue.ToString()))
                if (reader.Read())
                    txtExpenceAccount.Text = reader["code"].ToString();
                else
                    txtExpenceAccount.Text = "";

        }

        private void txtAccountCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtAccountCode.Text)))
                if (reader.Read())
                    cmbAssetsAccount.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtCreditAccountCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
                if (reader.Read())
                    cmbDepreciationAccount.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void txtExpenceAccount_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtExpenceAccount.Text)))
                if (reader.Read())
                    cmbExpenceAccount.SelectedValue = int.Parse(reader["id"].ToString());
        }

        private void cmbCategoryName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategoryName.Focused)
            {
                if (cmbCategoryName.SelectedValue == null)
                {
                    txtCategoryCode.Text = "";
                    return;
                }

                if (cmbCategoryName.SelectedValue != null && int.Parse(cmbCategoryName.SelectedValue.ToString()) != 0)
                {
                    object obj = ClinicDatabase.ExecuteScalar(@"select id from tbl_fixed_assets_category where id =@id",
                                ClinicDatabase.CreateParameterNew("@id", cmbCategoryName.SelectedValue.ToString()));
                    int oldId = (obj != null && obj != DBNull.Value) ? int.Parse(obj.ToString()) : 0;
                    if (oldId > 0)
                    {
                        id = oldId;
                        BindData();
                    }
                    //using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_fixed_assets_category where id =@id", ClinicDatabase.CreateParameterNew("id", cmbCategoryName.SelectedValue.ToString())))
                    //{
                    //    if (reader.Read())
                    //    {
                    //        id = int.Parse(reader["id"].ToString());
                    //        txtCategoryCode.Text = reader["code"].ToString();
                    //        if (reader["assets_account_id"] != DBNull.Value && int.Parse(reader["assets_account_id"].ToString()) > 0)
                    //            cmbAssetsAccount.SelectedValue = int.Parse(reader["assets_account_id"].ToString());

                    //        if (reader["depreciation_account_id"] != DBNull.Value && int.Parse(reader["depreciation_account_id"].ToString()) > 0)
                    //            cmbDepreciationAccount.SelectedValue = int.Parse(reader["depreciation_account_id"].ToString());

                    //        if (reader["expence_account_id"] != DBNull.Value && int.Parse(reader["expence_account_id"].ToString()) > 0)
                    //            cmbExpenceAccount.SelectedValue = int.Parse(reader["expence_account_id"].ToString());
                    //    }
                    //    else
                    //    {
                    //        txtCategoryCode.Text = "";
                    //    }
                    //}
                }
            }
        }

        private void Lbheader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

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
    }
}
