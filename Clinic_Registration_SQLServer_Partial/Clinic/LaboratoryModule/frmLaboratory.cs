using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Clinic_DataAccess;

namespace Clinic.Medical_Services.Laboratory
{
    public partial class frmLaboratory : Form
    {
        public frmLaboratory() { InitializeComponent(); }

        private void frmLaboratory_Load(object sender, EventArgs e)
        {
            try { ClinicDatabase.EnsureLaboratoryTables(); ReloadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Laboratory setup", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void FiltersChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated) ReloadData();
        }

        private void ReloadData()
        {
            try
            {
                DataTable stats = ClinicDatabase.GetLabDashboardStats();
                if (stats.Rows.Count > 0)
                {
                    DataRow r = stats.Rows[0];
                    string[] fields = { "TotalTests", "TotalRequests", "Pending", "InProgress", "Completed" };
                    for (int i = 0; i < fields.Length; i++) statValues[i].Text = r[fields[i]].ToString();
                }
                dgvRequests.DataSource = ClinicDatabase.GetLabRequests(txtSearch.Text, cmbStatus.Text);
                if (dgvRequests.Columns.Contains("ResultNotes")) dgvRequests.Columns["ResultNotes"].Visible = false;
                if (dgvRequests.Columns.Contains("ResultValue")) dgvRequests.Columns["ResultValue"].Visible = false;
                if (dgvRequests.Columns.Contains("ResultedAt")) dgvRequests.Columns["ResultedAt"].Visible = false;
                if (dgvRequests.Columns.Contains("RequestedAt"))
                    dgvRequests.Columns["RequestedAt"].DefaultCellStyle.Format = "dd MMM yyyy HH:mm";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Load laboratory", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private int SelectedRequestID()
        {
            if (dgvRequests.CurrentRow == null || dgvRequests.CurrentRow.IsNewRow) return -1;
            return Convert.ToInt32(dgvRequests.CurrentRow.Cells["LabRequestID"].Value);
        }

        private void btnNewRequest_Click(object sender, EventArgs e)
        {
            using (var form = new frmLabRequest())
                if (form.ShowDialog(this) == DialogResult.OK) ReloadData();
        }

        private void btnManageTests_Click(object sender, EventArgs e)
        {
            using (var form = new frmLabTests())
            { form.ShowDialog(this); ReloadData(); }
        }

        private void btnEnterResult_Click(object sender, EventArgs e)
        {
            int id = SelectedRequestID();
            if (id < 1) { MessageBox.Show("Select a lab request first."); return; }
            string status = Convert.ToString(dgvRequests.CurrentRow.Cells["Status"].Value);
            if (status == "Completed" || status == "Cancelled")
            { MessageBox.Show("Completed or cancelled requests cannot be overwritten."); return; }
            using (var form = new frmLabResult(id))
                if (form.ShowDialog(this) == DialogResult.OK) ReloadData();
        }
    }
}
