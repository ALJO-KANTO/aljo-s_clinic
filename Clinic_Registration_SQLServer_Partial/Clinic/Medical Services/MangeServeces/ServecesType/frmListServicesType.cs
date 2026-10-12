using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Serveces.ServecesType
{
    public partial class frmListServicesType : Form
    {
        private DataTable _dtListServices=clsServicesType.GetServiceList();
        public frmListServicesType()
        {
            InitializeComponent();
        }

        private void _RefreshData()
        {
            _dtListServices = clsServicesType.GetServiceList();
            dgvServicesType.DataSource = _dtListServices;
            lblRecordsCount.Text = _dtListServices.Rows.Count.ToString();
        }
        private void frmListServicesType_Load(object sender, EventArgs e)
        {
            dgvServicesType.DataSource = _dtListServices;
            lblRecordsCount.Text=_dtListServices.Rows.Count.ToString();
            if (dgvServicesType.Rows.Count == 0)
                return;
            dgvServicesType.Columns["ServiceID"].HeaderText= "Service ID";
            dgvServicesType.Columns["ServiceID"].Width = 150;

            dgvServicesType.Columns["ServiceName"].HeaderText = "Service Name";
            dgvServicesType.Columns["ServiceName"].Width = 200;

            dgvServicesType.Columns["Description"].HeaderText = "Description";
            dgvServicesType.Columns["Description"].Width = 400;

            dgvServicesType.Columns["ServiceFees"].HeaderText = "Service Fees";
            dgvServicesType.Columns["ServiceFees"].Width = 100;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateServices frmAddUpdateServices = 
                    new frmAddUpdateServices((int)dgvServicesType.CurrentRow.Cells["ServiceID"].Value);
            frmAddUpdateServices.ShowDialog();
            _RefreshData();
        }

        private void addServicesTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateServices frmAddUpdateServices =
                new frmAddUpdateServices();
            frmAddUpdateServices.ShowDialog();
            _RefreshData();
        }

        private void deleteServicesTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete this service type?", "Delete Service Type", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int serviceID = (int)dgvServicesType.CurrentRow.Cells["ServiceID"].Value;
               if( clsServicesType.Delete(serviceID))
                {
                    MessageBox.Show("Service type deleted successfully.", "Delete Service Type", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshData();
                }
                else
                {
                    MessageBox.Show("Failed to delete service type.", "Delete Service Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
