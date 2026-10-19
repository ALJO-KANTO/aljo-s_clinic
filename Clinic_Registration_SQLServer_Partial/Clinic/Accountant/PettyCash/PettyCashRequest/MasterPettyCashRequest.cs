using Clinic_DataAccess;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterPettyCashRequest : Form
    {
        public MasterPettyCashRequest()
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            headerUC1.FormText = this.Text;
        }
        private void MasterPettyCashRequest_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvPetty);
            BindPettyCash();
        }

        public void BindPettyCash()
        {
            try
            {
                string query = @"
                                SELECT p.id,request_date `Date`,request_ref 'Ref',e.name `Petty Cash Name`,amount `Amount`,Description,a.name Account,p.State,Pay,`change` from tbl_petty_cash_request p
                                INNER JOIN tbl_coa_level_4 a ON a.id = p.debit_account_id
                                LEFT JOIN tbl_employee e ON e.id = p.Petty_cash_name
                                ";
                DataTable dt = ClinicDatabase.ExecuteDataTable(query);
                    
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                dgvPetty.SuspendLayout(); 
                dgvPetty.DataSource = dt;

                if (dgvPetty.Columns.Contains("id"))
                    dgvPetty.Columns["id"].Visible = false;

                if (dgvPetty.Columns.Contains("Description"))
                {
                    dgvPetty.Columns["Description"].MinimumWidth = 180;
                    dgvPetty.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (dgvPetty.Columns.Contains("Petty Cash Name"))
                {
                    dgvPetty.Columns["Petty Cash Name"].MinimumWidth = 180;
                    dgvPetty.Columns["Petty Cash Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                dgvPetty.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 9, FontStyle.Bold);
                dgvPetty.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
                dgvPetty.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dgvPetty.EnableHeadersVisualStyles = false;

                dgvPetty.RowsDefaultCellStyle.BackColor = Color.White;
                dgvPetty.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#eaf1fa");

                dgvPetty.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#d5dbdb");
                dgvPetty.DefaultCellStyle.SelectionForeColor = Color.Black;

                dgvPetty.BorderStyle = BorderStyle.None;
                dgvPetty.CellBorderStyle = DataGridViewCellBorderStyle.None;
                dgvPetty.RowHeadersVisible = false;
                dgvPetty.AllowUserToAddRows = false;
                dgvPetty.AllowUserToResizeRows = false;

                dgvPetty.ResumeLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading petty cash data:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPetty_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void dgvPetty_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;

        }

        private void btnNewRequest_Click(object sender, EventArgs e)
        {
            new frmPettyCashRequest(0).ShowDialog();

            BindPettyCash();
        }

        private void dgvPetty_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                int id = Convert.ToInt32(dgvCustomer.SelectedRows[0].Cells["id"].Value);
                frmPettyCashRequest frmPettyCash = new frmPettyCashRequest(id);
                frmPettyCash.ShowDialog();
                BindPettyCash();
            }
        }
    }
}
