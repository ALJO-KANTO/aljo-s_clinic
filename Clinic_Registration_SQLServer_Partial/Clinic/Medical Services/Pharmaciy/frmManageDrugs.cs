using Clinic.ControlsMain;
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

namespace Clinic.Medical_Services.Pharmaciy
{
    public partial class frmManageDrugs : Form
    {
        private clsMedicine _Medicine;
        private DataTable _MedicineTable = clsMedicine.GetAllMedicines();
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        private void ShowSuccess(string message) => MessageBox.Show(message, "✅ Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowError(string message) => MessageBox.Show(message, "❌ Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowWarning(string message) => MessageBox.Show(message, "⚠️ Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private DialogResult ConfirmAction(string message) => MessageBox.Show(message, "❓ Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        public frmManageDrugs()
        {
            InitializeComponent();
            SetupMedicineGridDesign();
            lbCurrentUser.Text = clsGlobal.CurrentUser.PersonInfo.Name;
            dgMedicines.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        }

        private void SetupMedicineGridDesign()
        {
            dgMedicines.AutoGenerateColumns = false;
            dgMedicines.Columns.Clear();

            #region
            // 1. عمود تعريفي (اختياري، مثلاً لعرض أيقونة)
            //dgMedicines.Columns.Add(new DataGridViewTextBoxColumn
            //{
            //    Name = "Icon",
            //    HeaderText = "",
            //    Width = 45,
            //    ReadOnly = true,
            //    DefaultCellStyle = new DataGridViewCellStyle
            //    {
            //        Alignment = DataGridViewContentAlignment.MiddleCenter,
            //        Font = new Font("Segoe UI", 12, FontStyle.Bold),
            //        ForeColor = Color.FromArgb(0, 210, 190)
            //    }
            //});
            #endregion
            dgMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "MedicineID", HeaderText = "ID", DataPropertyName = "MedicineID", ReadOnly = true, Width = 80 });
            dgMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "MedicineName", HeaderText = "Medicine Name", DataPropertyName = "MedicineName", ReadOnly = true, Width = 250 });
            dgMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "MedicinePrice", HeaderText = "Price", DataPropertyName = "MedicinePrice", ReadOnly = true, Width = 120 });
            dgMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "CurrentStock", HeaderText = "Stock", DataPropertyName = "CurrentStock", ReadOnly = true, Width = 100 });
            dgMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "ReorderLevel", HeaderText = "Reorder", DataPropertyName = "ReorderLevel", ReadOnly = true, Width = 100 });
            dgMedicines.Columns.Add(new DataGridViewTextBoxColumn { Name = "TaxRate", HeaderText = "Tax %", DataPropertyName = "TaxRate", ReadOnly = true, Width = 100 });

            // 3. عمود الإجراءات (Actions)
            DataGridViewLinkColumn actionColumn = new DataGridViewLinkColumn
            {
                Name = "Actions",
                HeaderText = "Actions",
                Text = "Edit  |  Delete",
                UseColumnTextForLinkValue = true,
                ActiveLinkColor = Color.Blue,
                LinkColor = Color.Brown,
                TrackVisitedState = false,
                Width = 148
            };

            dgMedicines.Columns.Add(actionColumn);

            dgMedicines.DataSource = _MedicineTable;
           
        }

        private void _RefreshMedicinesList()
        {
            _MedicineTable = clsMedicine.GetAllMedicines();
            dgMedicines.DataSource = _MedicineTable;
        }
        private void _ResetFaild()
        {
            txtMedicinePrice.Text = string.Empty;
            txtMedicneName.Text = string.Empty;
            nudCurrentStock.Value = 1;
            nudReorderLevel.Value = 5;
            nudTaxRate.Value = 0;
            _RefreshMedicinesList();


        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                ShowWarning("Please correct the input errors before saving.");
                return;
            }
            if (Mode == enMode.AddNew)
                _Medicine = new clsMedicine();
            _Medicine.MedicineName = txtMedicneName.Text;
            _Medicine.MedicinePrice = Convert.ToDecimal(txtMedicinePrice.Text);
            _Medicine.CurrentStock = (int)nudCurrentStock.Value;
            _Medicine.ReorderLevel = (int)nudReorderLevel.Value;
            _Medicine.TaxRate = (decimal)nudTaxRate.Value;

            if (_Medicine.Save())
            {
                ShowSuccess("Medicine  saved successfully.");
                _ResetFaild();
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void _UpdateMedicine(int MedicineId)
        {
            _Medicine = clsMedicine.Find(MedicineId);
            if (_Medicine == null)
            {
                ShowError("This Medicine Is Not Found ...");
                return;
            }
            btnSave.Text = "Update Drug";
            Mode = enMode.Update;
            txtMedicneName.Text = _Medicine.MedicineName;
            txtMedicinePrice.Text = _Medicine.MedicinePrice.ToString();
            nudCurrentStock.Value = _Medicine.CurrentStock;
            nudReorderLevel.Value = _Medicine.ReorderLevel;
            nudTaxRate.Value = _Medicine.TaxRate;

        }
        private void dgMedicines_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgMedicines.Columns[e.ColumnIndex].Name == "Actions" && e.RowIndex >= 0)
            {
                int medicineID = (int)dgMedicines.Rows[e.RowIndex].Cells["MedicineID"].Value;

                string action = dgMedicines.Rows[e.RowIndex].Cells["Actions"].Value.ToString();

                DialogResult result = MessageBox.Show("Choose action: Yes for Edit, No for Delete",
                                                     "Medicine Action", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _UpdateMedicine(medicineID);
                }
                else if (result == DialogResult.No)
                {
                    if (ConfirmAction("Are you sure you want to delete this medicine?")
                                     == DialogResult.Yes)
                    {
                        if (clsMedicine.DeleteMedicine(medicineID)) 
                        {
                          ShowSuccess("Deleted Successfully");
                            _RefreshMedicinesList();
                        }
                    }
                }
            }
        }
        private void lbBlaceholder_Click(object sender, EventArgs e)
            => txtSearch.Focus();

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lbBlaceholder.Visible = string.IsNullOrEmpty(txtSearch.Text);
            if (lbBlaceholder.Visible)
            {
                _MedicineTable.DefaultView.RowFilter = "";
            }
            else
            {
                _MedicineTable.DefaultView.RowFilter = string.Format("MedicineName LIKE '{0}%'", txtSearch.Text.Replace("'", "''"));
            }
        }

        private void txtMedicinePrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح بالأرقام، ومفتاح الرجوع (Backspace)، والفاصلة العشرية
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // السماح بفاصلة عشرية واحدة فقط
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtMedicneName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMedicneName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtMedicneName, "Medicine name cannot be blank!");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtMedicneName, "");
            }
        }
        private void txtMedicinePrice_Validating(object sender, CancelEventArgs e)
        {
            decimal price;
            if (string.IsNullOrWhiteSpace(txtMedicinePrice.Text) || !decimal.TryParse(txtMedicinePrice.Text, out price) || price <= 0)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtMedicinePrice, "Please enter a valid price greater than 0!");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtMedicinePrice, "");
            }
        }

    }
}
