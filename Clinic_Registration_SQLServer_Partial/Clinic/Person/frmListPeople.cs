using Clinic.global_classes;
using Clinic.Person;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    public partial class frmListPeople : Form
    {
        private DataTable _dtAllPeople = clsPerson.GetAllPersons();




        public frmListPeople()
        {
            InitializeComponent();

        }

        private void frmListPeople_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;
            if (_dtAllPeople != null)
            {
                _dtAllPeople.PrimaryKey = new DataColumn[] { _dtAllPeople.Columns["PersonID"] };
                _dtAllPeople.Columns["GenderCaption"].ReadOnly = false;
                dgvPeople.DataSource = _dtAllPeople;
                lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
                cbFilterBy.SelectedIndex = 0;
                if (dgvPeople.Rows.Count > 0)
                {
                    dgvPeople.Columns["PersonID"].HeaderText = "PersonID";
                    dgvPeople.Columns["PersonID"].Width = 100;

                    dgvPeople.Columns["FullName"].HeaderText = "FullName";
                    dgvPeople.Columns["FullName"].Width = 200;

                    dgvPeople.Columns["DateOfBirth"].HeaderText = "DateOfBirth";
                    dgvPeople.Columns["DateOfBirth"].Width = 150;

                    dgvPeople.Columns["GenderCaption"].HeaderText = "Gender";
                    dgvPeople.Columns["GenderCaption"].Width = 100;

                    dgvPeople.Columns["PhoneNumber"].HeaderText = "Phone";
                    dgvPeople.Columns["PhoneNumber"].Width = 170;

                    dgvPeople.Columns["CountryName"].HeaderText = "Country";
                    dgvPeople.Columns["CountryName"].Width = 100;

                    dgvPeople.Columns["Address"].HeaderText = "Address";
                    dgvPeople.Columns["Address"].Width = 170;

                    dgvPeople.Columns["Email"].HeaderText = "Email";
                    dgvPeople.Columns["Email"].Width = 170;


                }
            }

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dtAllPeople.DefaultView.RowFilter = string.Empty;
            lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();

            bool None = cbFilterBy.Text == "None";
            bool Gender = cbFilterBy.Text == "Gender";

            txtFilterValue.Visible = !None && !Gender;
            cbFilterGender.Visible = Gender && !None;

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = string.Empty;
                txtFilterValue.Focus();

            }
            else if (cbFilterGender.Visible)
            {
                cbFilterGender.SelectedIndex = 0;
                cbFilterBy.Focus();

            }
            else
            {
                cbFilterBy.Focus();
                txtFilterValue.Text = "";

            }
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.Text == "Person ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (_dtAllPeople == null) return;
            string FilterColumn = "";
            switch (cbFilterBy.Text)
            {
                case "Person ID":
                    FilterColumn = "PersonID";
                    break;
                case "Full Name":
                    FilterColumn = "FullName";
                    break;
                case "Phone":
                    FilterColumn = "PhoneNumber";
                    break;
                case "Country":
                    FilterColumn = "CountryName";
                    break;
                case "Address":
                    FilterColumn = "Address";
                    break;
                case "Email":
                    FilterColumn = "Email";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }
            if (txtFilterValue.Text == "" || FilterColumn == "None")
            {
                _dtAllPeople.DefaultView.RowFilter = string.Empty;
                lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
                return;
            }

            if (FilterColumn == "PersonID")
                _dtAllPeople.DefaultView.RowFilter = $"{FilterColumn}={txtFilterValue.Text}";
            else
                _dtAllPeople.DefaultView.RowFilter = $"{FilterColumn} LIKE '%{txtFilterValue.Text}%'";
            lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
        }

        private void cbFilterGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dtAllPeople == null)
                return;
            if (cbFilterGender.Text == "All")
                _dtAllPeople.DefaultView.RowFilter = string.Empty;
            else
                _dtAllPeople.DefaultView.RowFilter = $"GenderCaption = '{cbFilterGender.Text}'";

            lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.DataBack += _DatatBackToAdd;
            frm.ShowDialog();
            frmListPeople_Load(null, null);

        }

        private void _DatatBackToAdd(object sender, int PersonID)
        {
            // 1. جلب بيانات الشخص الجديد كـ DataRow من الـ Business Layer
            DataRow NewPersonRow = clsPerson.GetPersonByID(PersonID);

            if (NewPersonRow != null)
            {
                _dtAllPeople.UpsertRow(NewPersonRow, PersonID);
                lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
            }
        }
        private void _DataBackToUpdate(object sender, int PersonID)
        {
            DataRow UpdatePersonRow = clsPerson.GetPersonByID(PersonID);
            if (UpdatePersonRow != null)
            {
               
                _dtAllPeople.UpsertRow(UpdatePersonRow, PersonID);
            }
        }
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson((int)dgvPeople.CurrentRow.Cells[0].Value);
            frm.DataBack += _DataBackToUpdate;
            frm.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = (int)dgvPeople.CurrentRow.Cells[0].Value;
            frmShowPersonInfo frm = new frmShowPersonInfo(PersonID);
            frm.ShowDialog();
            _DataBackToUpdate(null, PersonID);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to delete Person [" + dgvPeople.CurrentRow.Cells[0].Value + "]", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)

            {

                int PersonID = (int)dgvPeople.CurrentRow.Cells[0].Value;
                //Perform Delele and refresh
                if (clsPerson.SoftDelete(PersonID))
                {
                    MessageBox.Show("Person Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _DataBackToDelete(PersonID);
                }

                else
                    MessageBox.Show("Person was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void _DataBackToDelete(int PersonID)
        {
            DataRow row = _dtAllPeople.Rows.Find(PersonID);
            if (row != null)
            {
                _dtAllPeople.Rows.Remove(row);
                _dtAllPeople.AcceptChanges();
                lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
            }
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature Is Not Implemented Yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature Is Not Implemented Yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

     

        private void btnTrash_Click(object sender, EventArgs e)
        {
            frmTrashPeople frm = new frmTrashPeople();
            frm.DataBack += _DatatBackToAdd;
            frm.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
