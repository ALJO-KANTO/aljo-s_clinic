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

namespace Clinic.Appointment.AppointmentsType
{
    public partial class frmListAppointmentType : Form
    {
        private DataTable _dtAppointmetType = clsAppointmentType.GetAllAppointmentType();
        public frmListAppointmentType()
        {
            InitializeComponent();
        }
        private void frmListAppointmentType_Load(object sender, EventArgs e)
        {
            dgvAppointmentType.DataSource = _dtAppointmetType;
            lblRecordsCount.Text=_dtAppointmetType.Rows.Count.ToString();
            if (_dtAppointmetType.Rows.Count == 0)
                return;

            dgvAppointmentType.Columns["AppointmentTypeID"].HeaderText="Appointment ID";
            dgvAppointmentType.Columns["AppointmentTypeID"].Width = 120;

            dgvAppointmentType.Columns["TypeName"].HeaderText = "Appointment Type Name";
            dgvAppointmentType.Columns["TypeName"].Width = 200;

            dgvAppointmentType.Columns["DefaultFees"].HeaderText = "Default Fees";
            dgvAppointmentType.Columns["DefaultFees"].Width = 120;
        }

        private void _RefreshAppintmetsType()
        {
            _dtAppointmetType=clsAppointmentType.GetAllAppointmentType();
            dgvAppointmentType.DataSource= _dtAppointmetType;
            lblRecordsCount.Text=_dtAppointmetType.Rows.Count.ToString();
        }
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdddUpdateAppointmentType frm = 
             new frmAdddUpdateAppointmentType((int)dgvAppointmentType.CurrentRow.Cells["AppointmentTypeID"].Value);
            frm.ShowDialog();
            _RefreshAppintmetsType();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addAppointmentTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdddUpdateAppointmentType frm = 
                new frmAdddUpdateAppointmentType();
            frm.ShowDialog();
            _RefreshAppintmetsType();
        }

        private void deletAppointmentTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete this appointment type?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int appointmentTypeId = (int)dgvAppointmentType.CurrentRow.Cells["AppointmentTypeID"].Value;
              if(clsAppointmentType.Delete(appointmentTypeId))
                {
                    MessageBox.Show("Appointment type deleted successfully.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshAppintmetsType();

                }
                else
                {
                    MessageBox.Show("Failed to delete the appointment type. It may be associated with existing appointments.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
