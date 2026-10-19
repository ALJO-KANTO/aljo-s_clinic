using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using static Guna.UI2.WinForms.Suite.Descriptions;
////using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmAddAccount : Form
    {
        EventHandler lvl1, lvl2, lvl3;
        public frmAddAccount()
        {
            InitializeComponent(); 
            //LocalizationManager.LocalizeForm(this);
            btnSave.Enabled = lnkNewCategory.Enabled = lnlLvl2Add.Enabled = lnkLvl3Add.Enabled = UserPermissions.canView("Chart Of Account");
            lnkLvl1Edit.Enabled = lnkLvl2Edit.Enabled = lnkLvl3Edit.Enabled = UserPermissions.canEdit("Chart Of Account");
            lnkLvl1Delete.Enabled = lnkLvl2Delete.Enabled = lnkLvl3Delete.Enabled = UserPermissions.canDelete("Chart Of Account");
            lvl1 = (sender, args) => setComboBoxLevel1();
            lvl2 = (sender, args) => setComboBoxLevel2();
            lvl3 = (sender, args) => setComboBoxLevel3();
            //.lvl1Account += lvl1;
            //.lvl2Account += lvl2;
            //.lvl3Account += lvl3;
            headerUC1.FormText = "COA - New Account";
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmAddAccount_Load(object sender, EventArgs e)
        {
            dtOpen.Value = DateTime.Now.Date;

            BindCombos.Populatelevel1Account(cmbLevel1);

            BindCombos.Populatelevel2Account(
                cmbLevel2,
                cmbLevel1.SelectedValue.ToString() == ""
                    ? 0
                    : (int)cmbLevel1.SelectedValue);

            BindCombos.Populatelevel3Account(
                cmbLevel3,
                cmbLevel2.SelectedValue.ToString() == ""
                    ? 0
                    : (int)cmbLevel2.SelectedValue);

            //LocalizationManager.LocalizeForm(this);
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic = language.Equals(
                "ar",
                StringComparison.OrdinalIgnoreCase);


            ApplyLanguageLayout(isArabic);
        }
        public void ApplyLanguageLayout(bool isArabic)
        {
            // Do NOT mirror the complete Form.
            RightToLeft = RightToLeft.No;
            RightToLeftLayout = false;

            SuspendLayout();
            guna2GroupBox2.SuspendLayout();
            panel1.SuspendLayout();

            try
            {
                if (isArabic)
                {
                    // =====================================================
                    // ARABIC TEXT
                    // =====================================================

                    Text = "إضافة حساب جديد";
                    headerUC1.FormText = "إضافة حساب جديد";

                    label1.Text = "المستوى 1";
                    label2.Text = "المستوى 2";
                    label4.Text = "المستوى 3";
                    label7.Text = "المستوى 4";

                    label6.Text = "مدين";
                    label5.Text = "دائن";
                    label3.Text = "التاريخ";

                    txtLevel4.PlaceholderText = "أدخل اسم الحساب";
                    txtDebit.PlaceholderText = "مدين";
                    txtCredit.PlaceholderText = "دائن";

                    lnkNewCategory.Text = "إضافة جديد";
                    lnlLvl2Add.Text = "إضافة جديد";
                    lnkLvl3Add.Text = "إضافة جديد";

                    lnkLvl1Edit.Text = "تعديل";
                    lnkLvl2Edit.Text = "تعديل";
                    lnkLvl3Edit.Text = "تعديل";

                    lnkLvl1Delete.Text = "حذف";
                    lnkLvl2Delete.Text = "حذف";
                    lnkLvl3Delete.Text = "حذف";

                    btnSave.Text = "حفظ";
                    btnClose.Text = "إغلاق";


                    // =====================================================
                    // MAIN FIELDS -> RIGHT SIDE
                    //
                    // English:
                    //
                    // Level | [ Combo ] [Add] [Edit] [Delete]
                    //
                    // Arabic:
                    //
                    // [Delete] [Edit] [Add] [ Combo ] | المستوى
                    // =====================================================

                    int rightFieldX =
                        guna2GroupBox2.ClientSize.Width - 82 - 273;

                    // Combos
                    cmbLevel1.Location =
                        new System.Drawing.Point(rightFieldX, 36);

                    cmbLevel2.Location =
                        new System.Drawing.Point(rightFieldX, 90);

                    cmbLevel3.Location =
                        new System.Drawing.Point(rightFieldX, 147);

                    txtLevel4.Location =
                        new System.Drawing.Point(
                            guna2GroupBox2.ClientSize.Width - 82 - 272,
                            204);


                    // =====================================================
                    // LEVEL LABELS -> RIGHT OF FIELDS
                    // =====================================================

                    int labelX =
                        guna2GroupBox2.ClientSize.Width - 77;

                    label1.Location =
                        new System.Drawing.Point(labelX, 31);

                    label2.Location =
                        new System.Drawing.Point(labelX, 86);

                    label4.Location =
                        new System.Drawing.Point(labelX, 144);

                    label7.Location =
                        new System.Drawing.Point(labelX, 201);

                    label1.TextAlign =
                        System.Drawing.ContentAlignment.MiddleLeft;

                    label2.TextAlign =
                        System.Drawing.ContentAlignment.MiddleLeft;

                    label4.TextAlign =
                        System.Drawing.ContentAlignment.MiddleLeft;

                    label7.TextAlign =
                        System.Drawing.ContentAlignment.MiddleLeft;


                    // =====================================================
                    // ADD / EDIT / DELETE LINKS -> LEFT SIDE
                    //
                    // Original links were on RIGHT.
                    // Arabic moves them to LEFT.
                    // =====================================================

                    int addX = 166;
                    int editX = 84;
                    int deleteX = 2;

                    // LEVEL 1
                    lnkNewCategory.Location =
                        new System.Drawing.Point(addX, 33);

                    lnkLvl1Edit.Location =
                        new System.Drawing.Point(editX, 33);

                    lnkLvl1Delete.Location =
                        new System.Drawing.Point(deleteX, 33);


                    // LEVEL 2
                    lnlLvl2Add.Location =
                        new System.Drawing.Point(addX, 89);

                    lnkLvl2Edit.Location =
                        new System.Drawing.Point(editX, 89);

                    lnkLvl2Delete.Location =
                        new System.Drawing.Point(deleteX, 89);


                    // LEVEL 3
                    lnkLvl3Add.Location =
                        new System.Drawing.Point(addX, 144);

                    lnkLvl3Edit.Location =
                        new System.Drawing.Point(editX, 144);

                    lnkLvl3Delete.Location =
                        new System.Drawing.Point(deleteX, 144);


                    // =====================================================
                    // TEXT ALIGNMENT
                    // =====================================================

                    cmbLevel1.RightToLeft = RightToLeft.Yes;
                    cmbLevel2.RightToLeft = RightToLeft.Yes;
                    cmbLevel3.RightToLeft = RightToLeft.Yes;

                    txtLevel4.TextAlign =
                        HorizontalAlignment.Right;

                    txtDebit.TextAlign =
                        HorizontalAlignment.Right;

                    txtCredit.TextAlign =
                        HorizontalAlignment.Right;


                    // =====================================================
                    // BOTTOM VALUES
                    //
                    // Mirror:
                    //
                    // English:
                    // Debit       Credit                 Date
                    //
                    // Arabic:
                    // Date                    Credit      Debit
                    // =====================================================

                    // Debit -> RIGHT
                    txtDebit.Location =
                        new System.Drawing.Point(435, 241);

                    label6.Location =
                        new System.Drawing.Point(517, 237);

                    label6.TextAlign =
                        System.Drawing.ContentAlignment.MiddleLeft;


                    // Credit -> center
                    txtCredit.Location =
                        new System.Drawing.Point(247, 241);

                    label5.Location =
                        new System.Drawing.Point(329, 237);

                    label5.TextAlign =
                        System.Drawing.ContentAlignment.MiddleLeft;


                    // Date -> LEFT
                    dtOpen.Location =
                        new System.Drawing.Point(40, 245);

                    label3.Location =
                        new System.Drawing.Point(154, 242);

                    label3.TextAlign =
                        System.Drawing.ContentAlignment.MiddleLeft;


                    // =====================================================
                    // BOTTOM BUTTONS
                    //
                    // English Save/Close are RIGHT.
                    // Arabic -> LEFT
                    // =====================================================

                    btnSave.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left;

                    btnClose.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left;

                    btnSave.Location =
                        new System.Drawing.Point(10, 3);

                    btnClose.Location =
                        new System.Drawing.Point(
                            btnSave.Right + 6,
                            3);
                }
                else
                {
                    // =====================================================
                    // ENGLISH - EXACT ORIGINAL DESIGNER POSITIONS
                    // =====================================================

                    Text = "Add Account Level1";
                    headerUC1.FormText = "COA - New Account";

                    label1.Text = "LEVEL 1";
                    label2.Text = "LEVEL 2";
                    label4.Text = "LEVEL 3";
                    label7.Text = "LEVEL 4";

                    label6.Text = "Debit";
                    label5.Text = "Credit";
                    label3.Text = "Due Date";

                    txtLevel4.PlaceholderText =
                        "Enter Level 4";

                    txtDebit.PlaceholderText =
                        "Debit ";

                    txtCredit.PlaceholderText =
                        "Credit";

                    lnkNewCategory.Text = "Add New";
                    lnlLvl2Add.Text = "Add New";
                    lnkLvl3Add.Text = "Add New";

                    lnkLvl1Edit.Text = "Edit Name";
                    lnkLvl2Edit.Text = "Edit Name";
                    lnkLvl3Edit.Text = "Edit Name";

                    lnkLvl1Delete.Text = "Delete Name";
                    lnkLvl2Delete.Text = "Delete Name";
                    lnkLvl3Delete.Text = "Delete Name";

                    btnSave.Text = "Save";
                    btnClose.Text = "Close";


                    // =====================================================
                    // ORIGINAL FIELD POSITIONS
                    // =====================================================

                    cmbLevel1.Location =
                        new System.Drawing.Point(82, 36);

                    cmbLevel2.Location =
                        new System.Drawing.Point(82, 90);

                    cmbLevel3.Location =
                        new System.Drawing.Point(82, 147);

                    txtLevel4.Location =
                        new System.Drawing.Point(82, 204);


                    // Labels
                    label1.Location =
                        new System.Drawing.Point(4, 31);

                    label2.Location =
                        new System.Drawing.Point(4, 86);

                    label4.Location =
                        new System.Drawing.Point(4, 144);

                    label7.Location =
                        new System.Drawing.Point(7, 201);

                    label1.TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight;

                    label2.TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight;

                    label4.TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight;

                    label7.TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight;


                    // =====================================================
                    // ORIGINAL LINKS
                    // =====================================================

                    lnkNewCategory.Location =
                        new System.Drawing.Point(358, 33);

                    lnkLvl1Edit.Location =
                        new System.Drawing.Point(422, 33);

                    lnkLvl1Delete.Location =
                        new System.Drawing.Point(506, 33);


                    lnlLvl2Add.Location =
                        new System.Drawing.Point(358, 89);

                    lnkLvl2Edit.Location =
                        new System.Drawing.Point(422, 89);

                    lnkLvl2Delete.Location =
                        new System.Drawing.Point(506, 89);


                    lnkLvl3Add.Location =
                        new System.Drawing.Point(358, 144);

                    lnkLvl3Edit.Location =
                        new System.Drawing.Point(422, 144);

                    lnkLvl3Delete.Location =
                        new System.Drawing.Point(506, 144);


                    // =====================================================
                    // ORIGINAL BOTTOM VALUES
                    // =====================================================

                    txtDebit.Location =
                        new System.Drawing.Point(82, 241);

                    label6.Location =
                        new System.Drawing.Point(40, 237);


                    txtCredit.Location =
                        new System.Drawing.Point(276, 241);

                    label5.Location =
                        new System.Drawing.Point(228, 237);


                    dtOpen.Location =
                        new System.Drawing.Point(451, 245);

                    label3.Location =
                        new System.Drawing.Point(358, 242);

                    label3.TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight;


                    // Text direction
                    cmbLevel1.RightToLeft = RightToLeft.No;
                    cmbLevel2.RightToLeft = RightToLeft.No;
                    cmbLevel3.RightToLeft = RightToLeft.No;

                    txtLevel4.TextAlign =
                        HorizontalAlignment.Left;

                    txtDebit.TextAlign =
                        HorizontalAlignment.Left;

                    txtCredit.TextAlign =
                        HorizontalAlignment.Left;


                    // =====================================================
                    // ORIGINAL SAVE / CLOSE
                    // =====================================================

                    btnSave.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right;

                    btnClose.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right;

                    btnSave.Location =
                        new System.Drawing.Point(456, 3);

                    btnClose.Location =
                        new System.Drawing.Point(524, 3);
                }
            }
            finally
            {
                guna2GroupBox2.ResumeLayout(false);
                panel1.ResumeLayout(false);
                ResumeLayout(false);

                guna2GroupBox2.PerformLayout();
                panel1.PerformLayout();

                guna2GroupBox2.Invalidate(true);
                panel1.Invalidate(true);
            }
        }
        private void setComboBoxLevel1()
        {
            BindCombos.Populatelevel1Account(cmbLevel1);
            SetSelectedIndexToLastItem(cmbLevel1);
        }
        private void setComboBoxLevel2()
        {
            BindCombos.Populatelevel2Account(cmbLevel2, cmbLevel1.SelectedValue.ToString() == "" ? 0 : (int)cmbLevel1.SelectedValue);
            SetSelectedIndexToLastItem(cmbLevel2);
        }
        private void setComboBoxLevel3()
        {
            BindCombos.Populatelevel3Account(cmbLevel3, cmbLevel2.SelectedValue.ToString() == "" ? 0 : (int)cmbLevel2.SelectedValue);
            SetSelectedIndexToLastItem(cmbLevel3);
        }

        private void SetSelectedIndexToLastItem(Guna.UI2.WinForms.Guna2ComboBox comboBox)
        {
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = comboBox.Items.Count - 1;
            }
        }
        private void cmbLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCombos.Populatelevel2Account(cmbLevel2, cmbLevel1.SelectedValue.ToString() == "" ? 0 : (int)cmbLevel1.SelectedValue);
            if (cmbLevel2.SelectedValue == null)
            {
                cmbLevel3.DataSource = null;
                return;
            }
            BindCombos.Populatelevel3Account(cmbLevel3, cmbLevel2.SelectedValue.ToString() == "" ? 0 : (int)cmbLevel2.SelectedValue);
        }

        private void cmbLevel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCombos.Populatelevel3Account(cmbLevel3, cmbLevel2.SelectedValue.ToString() == "" ? 0 : (int)cmbLevel2.SelectedValue);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (insertAccount())
            {
                //.Refreshlvl4Account();
                this.Close();
            }
        }
        private bool insertAccount()
        {
            if (dtOpen.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Enter Correct Date");
                return false;
            }
            if (txtLevel4.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Level 4 Account Name First.");
                return false;
            }
            if (cmbLevel3.SelectedValue == null || cmbLevel3.Text == "")
            {
                MessageBox.Show("Please Select Level 3 First.");
                return false;
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where name = @name",
               ClinicDatabase.CreateParameterNew("name", txtLevel4.Text)))
                if (reader.Read())
                {
                    MessageBox.Show("Account Already Exists. Enter Another Name.");
                    reader.Dispose();
                    return false;
                }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select code from tbl_coa_level_3 where id = @id",
                ClinicDatabase.CreateParameterNew("id", cmbLevel3.SelectedValue.ToString())))
            {
                reader.Read();
                using (MySqlDataReader r = ClinicDatabase.ExecuteReader("select max(code) as code from tbl_coa_level_4 where code like '" + reader["code"].ToString() + "%'"))
                {

                    if (txtCredit.Text == "")
                    {
                        txtCredit.Text = "0";
                    }
                    if (txtDebit.Text == "")
                    {
                        txtDebit.Text = "0";
                    }
                    if (r.Read() && r["code"].ToString() != "")
                    {
                        int refId = int.Parse(ClinicDatabase.ExecuteScalarNew(@"INSERT INTO `tbl_coa_level_4`(`name`, `code`, `main_id`,debit,credit,date)
                                    VALUES(@name,@code, @id,@debit,@credit,@date); SELECT LAST_INSERT_ID();",
                                                       ClinicDatabase.CreateParameterNew("name", txtLevel4.Text),
                                                       ClinicDatabase.CreateParameterNew("id", cmbLevel3.SelectedValue.ToString()),
                                                       ClinicDatabase.CreateParameterNew("code", int.Parse(r["code"].ToString()) + 1),
                                                       ClinicDatabase.CreateParameterNew("debit", txtDebit.Text.ToString()),
                                                       ClinicDatabase.CreateParameterNew("credit", txtCredit.Text),
                                                       ClinicDatabase.CreateParameterNew("date", dtOpen.Value)).ToString());
                        if (decimal.Parse(txtCredit.Text.ToString().Trim()) > 0 || decimal.Parse(txtDebit.Text.ToString().Trim()) > 0)
                        {
                            insertTransactions(refId, (int.Parse(r["code"].ToString()) + 1).ToString());
                        }
                        ////Utilities.LogAudit(-1, "Add Account", "Chart Of Account", refId, "Added Account: " + txtLevel4.Text + " with Code: " + (int.Parse(r["code"].ToString()) + 1).ToString());
                    }
                    else
                    {
                        int refId = int.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO `tbl_coa_level_4`(`name`, `code`, `main_id`,debit,credit,date)
                                    VALUES(@name,@code, @id,@debit,@credit,@date); SELECT LAST_INSERT_ID();",
                                                  ClinicDatabase.CreateParameterNew("name", txtLevel4.Text),
                                                  ClinicDatabase.CreateParameterNew("id", cmbLevel3.SelectedValue.ToString()),
                                                  ClinicDatabase.CreateParameterNew("code", int.Parse(reader["code"].ToString()) + "001"),
                                                       ClinicDatabase.CreateParameterNew("debit", txtDebit.Text.ToString()),
                                                       ClinicDatabase.CreateParameterNew("credit", txtCredit.Text),
                                                       ClinicDatabase.CreateParameterNew("date", dtOpen.Value)).ToString());
                        if (decimal.Parse(txtCredit.Text.ToString().Trim()) > 0 || decimal.Parse(txtDebit.Text.ToString().Trim()) > 0)
                        {
                            insertTransactions(refId, (int.Parse(reader["code"].ToString()) + "001").ToString());
                        }
                        ////Utilities.LogAudit(-1, "Add Account", "Chart Of Account", refId, "Added Account: " + txtLevel4.Text + " with Code: " + (int.Parse(reader["code"].ToString()) + "001").ToString());
                    }
                }
            }
            return true;
        }
        private void insertTransactions(int refId, string code)
        {
            string accountId = refId.ToString();
            string openingBalanceEquity = BindCombos.SelectDefaultLevelAccount("Opening Balance Equity").ToString();

            decimal creditAmount = decimal.Parse(txtCredit.Text);
            decimal debitAmount = decimal.Parse(txtDebit.Text);
            if (int.Parse(openingBalanceEquity) == 0)
            {
                object result = ClinicDatabase.ExecuteScalar(@"SELECT id FROM tbl_coa_level_4 WHERE name = 'Opening Balance Equity'");
                if (result != null && result != DBNull.Value)
                {
                    openingBalanceEquity = result.ToString();
                }
            }
            if (int.Parse(openingBalanceEquity) > 0)
            {
                if (creditAmount != 0)
                {
                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, openingBalanceEquity, creditAmount.ToString(), "0",
                      refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Opening Balance Equity - Ledger", -1, DateTime.Now.Date, "");

                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, accountId, "0", creditAmount.ToString(),
                       refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Account Payable - Ledger Code ", -1, DateTime.Now.Date, "");
                }

                if (debitAmount != 0)
                {
                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, openingBalanceEquity, "0", debitAmount.ToString(),
                            refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Opening Balance Equity - Ledger Code ", -1, DateTime.Now.Date, "");

                    CommonInsert.addTransactionEntry(dtOpen.Value.Date, accountId, debitAmount.ToString(), "0",
                       refId.ToString(), "0", "General Ledger Opening Balance", "GENERAL LEDGER OPENING BALANCE", "Account Payable - Ledger Code - ", -1, DateTime.Now.Date, "");
                }
            }
            else
            {
                MessageBox.Show("Cannot make opening balance without opening balance equity account");
            }
        }

        private void lnkNewCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmViewLevel1(0).ShowDialog();
        }


        private void lnkLvl1Edit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cmbLevel1.SelectedValue == null || cmbLevel1.Text == "")
            {
                MessageBox.Show("Select Level 1 First.");
                return;
            }
            new frmViewLevel1(int.Parse(cmbLevel1.SelectedValue.ToString())).ShowDialog();
        }

        private void lnkLvl2Edit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cmbLevel2.SelectedValue == null || cmbLevel2.Text == "")
            {
                MessageBox.Show("Select Level 2 First.");
                return;
            }
            new frmViewLevel2(int.Parse(cmbLevel2.SelectedValue.ToString())).ShowDialog();
        }

        private void lnkLvl3Edit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cmbLevel3.SelectedValue == null || cmbLevel2.Text == "")
            {
                MessageBox.Show("Select Level 3 First.");
                return;
            }
            new frmViewLevel3(int.Parse(cmbLevel3.SelectedValue.ToString())).ShowDialog();
        }
        private void lnkLvl3Add_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmViewLevel3(0, int.Parse(cmbLevel1.SelectedValue.ToString()), int.Parse(cmbLevel2.SelectedValue.ToString())).ShowDialog();
        }

        private void lnkLvl1Delete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cmbLevel1.SelectedValue == null || cmbLevel1.Text == "")
            {
                MessageBox.Show("Select Level 1 First.");
                return;
            }
            else
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select count(*) from tbl_coa_level_2 where main_id = @id",
                ClinicDatabase.CreateParameterNew("id", cmbLevel1.SelectedValue.ToString())))
                    if (reader.Read())
                    {
                        if (int.Parse(reader[0].ToString()) > 0)
                        {
                            MessageBox.Show(cmbLevel1.Text.ToString() + " is used. Cannot delete.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ClinicDatabase.ExecuteNonQuery("delete from tbl_coa_level_1 where id = @id",
                            ClinicDatabase.CreateParameterNew("id", cmbLevel1.SelectedValue.ToString()));

                            MessageBox.Show(cmbLevel1.Text.ToString() + " deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //.Refreshlvl4Account();
                        }
                    }
            }
        }

        private void lnkLvl2Delete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cmbLevel2.SelectedValue == null || cmbLevel2.Text == "")
            {
                MessageBox.Show("Select Level 2 First.");
                return;
            }
            else
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select count(*) from tbl_coa_level_3 where main_id = @id",
                ClinicDatabase.CreateParameterNew("id", cmbLevel2.SelectedValue.ToString())))
                    if (reader.Read())
                    {
                        if (int.Parse(reader[0].ToString()) > 0)
                        {
                            MessageBox.Show(cmbLevel2.Text.ToString() + " is used. Cannot delete.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ClinicDatabase.ExecuteNonQuery("delete from tbl_coa_level_2 where id = @id",
                            ClinicDatabase.CreateParameterNew("id", cmbLevel2.SelectedValue.ToString()));

                            MessageBox.Show(cmbLevel2.Text.ToString() + " deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //.Refreshlvl4Account();
                        }
                    }
            }
        }

        private void lnkLvl3Delete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cmbLevel3.SelectedValue == null || cmbLevel3.Text == "")
            {
                MessageBox.Show("Select Level 3 First.");
                return;
            }
            else
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select count(*) from tbl_coa_level_4 where main_id = @id",
                ClinicDatabase.CreateParameterNew("id", cmbLevel3.SelectedValue.ToString())))
                    if (reader.Read())
                    {
                        if (int.Parse(reader[0].ToString()) > 0)
                        {
                            MessageBox.Show(cmbLevel3.Text.ToString() + " is used. Cannot delete.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ClinicDatabase.ExecuteNonQuery("delete from tbl_coa_level_3 where id = @id",
                            ClinicDatabase.CreateParameterNew("id", cmbLevel3.SelectedValue.ToString()));

                            MessageBox.Show(cmbLevel3.Text.ToString() + " deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //.Refreshlvl4Account();
                        }
                    }
            }
        }

        private void guna2GroupBox2_Click(object sender, EventArgs e)
        {

        }

        private void txtDebit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void txtCredit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void panel8_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void frmAddAccount_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.lvl1Account -= lvl1;
            //.lvl2Account -= lvl2;
            //.lvl3Account -= lvl3;
        }


        private void lnlLvl2Add_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmViewLevel2(0, int.Parse(cmbLevel1.SelectedValue.ToString())).ShowDialog();
        }
    }
}
