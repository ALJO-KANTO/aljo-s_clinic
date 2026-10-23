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

        int IncomeAccount = 0;
        int AssetAccount = 0;
        int COGSAccount = 0;

        private void ShowSuccess(string message) => MessageBox.Show(T(message), T("✅ Success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowError(string message) => MessageBox.Show(T(message), T("❌ Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowWarning(string message) => MessageBox.Show(T(message), T("⚠️ Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private DialogResult ConfirmAction(string message) => MessageBox.Show(T(message), T("❓ Confirm Action"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        public frmManageDrugs()
        {
            InitializeComponent();
            nudCurrentStock.DecimalPlaces = 3;
            nudCurrentStock.Maximum = 99999999999.999M;

            nudReorderLevel.DecimalPlaces = 3;
            nudReorderLevel.Maximum = 99999999999.999M;
            SetupMedicineGridDesign();
            lbCurrentUser.Text = clsGlobal.CurrentUser.PersonInfo.Name;
            dgMedicines.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            ApplyArabicProperties();

            BindCombos.PopulateWarehouse(cmbWarehouse);
            BindCombos.PopulateItemTax(nudTaxRate);

            IncomeAccount = BindCombos.SelectDefaultLevelAccount("Sales");
            AssetAccount = BindCombos.SelectDefaultLevelAccount("Inventory");
            COGSAccount = BindCombos.SelectDefaultLevelAccount("COGS");
        }

        private string T(string text)
        {
            return TranslationClass.Translate(text, TranslationClass.IsArabic);
        }

        // Call this again when the language is changed while this form is open.
        public void ApplyArabicProperties()
        {
            bool arabic = TranslationClass.IsArabic;
            this.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            this.RightToLeftLayout = arabic;
            ApplyTranslationToControls(this);
            ApplyLanguageSpecificLayout(arabic);
            TranslateMedicineGridHeaders();
        }

        // Changes only the requested positions when Arabic is selected.
        private void ApplyLanguageSpecificLayout(bool arabic)
        {
            if (arabic)
            {
                // Center the form heading across the title panel.
                lblTitle.AutoSize = false;
                lblTitle.Location = new Point(0, 10);
                lblTitle.Size = new Size(panTitle.ClientSize.Width, 55);
                lblTitle.TextAlign = ContentAlignment.MiddleCenter;
                lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                // Put the username to the left of the current-user caption.
                lbCurrentUser.AutoSize = false;
                lbCurrentUser.Location = new Point(875, 24);
                lbCurrentUser.Size = new Size(180, 28);
                lbCurrentUser.TextAlign = ContentAlignment.MiddleRight;
                lbCurrentUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                label5.AutoSize = false;
                label5.Location = new Point(1060, 24);
                label5.Size = new Size(155, 28);
                label5.TextAlign = ContentAlignment.MiddleRight;
                label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                // Place medicine field labels to the right of their input controls.
                SetArabicFieldLabel(label1, 920, 28, 185, 28);
                SetArabicFieldLabel(label2, 920, 78, 185, 28);
                SetArabicFieldLabel(label3, 920, 128, 185, 28);
                SetArabicFieldLabel(label4, 920, 178, 185, 28);
                SetArabicFieldLabel(label7, 920, 230, 185, 28);

                // Put the search label on the right side of the search textbox.
                txtSearch.Location = new Point(600, 14);
                txtSearch.RightToLeft = RightToLeft.Yes;
                label6.AutoSize = false;
                label6.Location = new Point(1150, 16);
                label6.Size = new Size(130, 28);
                label6.TextAlign = ContentAlignment.MiddleRight;
                lbBlaceholder.Location = new Point(615, 20);
                lbBlaceholder.RightToLeft = RightToLeft.Yes;

                foreach (Control control in new Control[] { txtMedicneName, txtMedicinePrice, nudCurrentStock, nudReorderLevel, nudTaxRate })
                    control.RightToLeft = RightToLeft.Yes;
            }
            else
            {
                // Restore the original designer positions for English.
                lblTitle.AutoSize = true;
                lblTitle.Location = new Point(75, 14);
                lblTitle.Size = new Size(399, 45);
                lblTitle.TextAlign = ContentAlignment.MiddleLeft;
                lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left;

                lbCurrentUser.AutoSize = true;
                lbCurrentUser.Location = new Point(1143, 24);
                lbCurrentUser.Size = new Size(53, 28);
                lbCurrentUser.TextAlign = ContentAlignment.MiddleLeft;
                lbCurrentUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                label5.AutoSize = true;
                label5.Location = new Point(1000, 24);
                label5.Size = new Size(137, 28);
                label5.TextAlign = ContentAlignment.MiddleLeft;
                label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                SetEnglishFieldLabel(label1, 186, 28);
                SetEnglishFieldLabel(label2, 186, 78);
                SetEnglishFieldLabel(label3, 186, 128);
                SetEnglishFieldLabel(label4, 186, 178);
                SetEnglishFieldLabel(label7, 186, 230);

                txtSearch.Location = new Point(118, 14);
                txtSearch.RightToLeft = RightToLeft.No;
                label6.AutoSize = true;
                label6.Location = new Point(19, 16);
                label6.Size = new Size(83, 28);
                label6.TextAlign = ContentAlignment.MiddleLeft;
                lbBlaceholder.Location = new Point(135, 20);
                lbBlaceholder.RightToLeft = RightToLeft.No;

                foreach (Control control in new Control[] { txtMedicneName, txtMedicinePrice, nudCurrentStock, nudReorderLevel, nudTaxRate })
                    control.RightToLeft = RightToLeft.No;
            }
        }

        private void SetArabicFieldLabel(Label label, int x, int y, int width, int height)
        {
            label.AutoSize = false;
            label.Location = new Point(x, y);
            label.Size = new Size(width, height);
            label.TextAlign = ContentAlignment.MiddleRight;
            label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        private void SetEnglishFieldLabel(Label label, int x, int y)
        {
            label.AutoSize = true;
            label.Location = new Point(x, y);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Anchor = AnchorStyles.None;
        }

        private void ApplyTranslationToControls(Control parent)
        {
            bool arabic = TranslationClass.IsArabic;
            foreach (Control control in parent.Controls)
            {
                if (!string.IsNullOrWhiteSpace(control.Text))
                    control.Text = T(control.Text);

                control.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
                if (control is Label label && arabic)
                    label.TextAlign = ContentAlignment.MiddleRight;

                if (control.HasChildren)
                    ApplyTranslationToControls(control);
            }

            dgMedicines.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            dgMedicines.ColumnHeadersDefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft;
            dgMedicines.DefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft;
            dgMedicines.RowHeadersDefaultCellStyle.Alignment = arabic
                ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft;
        }

        private void TranslateMedicineGridHeaders()
        {
            foreach (DataGridViewColumn column in dgMedicines.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                    column.HeaderText = T(column.HeaderText);
            }
            if (dgMedicines.Columns.Contains("Actions"))
            {
                DataGridViewLinkColumn actionsColumn = dgMedicines.Columns["Actions"] as DataGridViewLinkColumn;
                if (actionsColumn != null)
                    actionsColumn.Text = T("Edit  |  Delete");
            }
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
            TranslateMedicineGridHeaders();
        }

        private void _RefreshMedicinesList()
        {
            _MedicineTable = clsMedicine.GetAllMedicines();
            dgMedicines.DataSource = _MedicineTable;
            TranslateMedicineGridHeaders();
        }
        private void _ResetFaild()
        {
            txtMedicinePrice.Text = string.Empty;
            txtMedicineCost.Text = string.Empty;
            txtMedicneName.Text = string.Empty;
            nudCurrentStock.Value = 1;
            nudReorderLevel.Value = 5;
            nudTaxRate.SelectedValue = 0;
            txtBatchNo.Text = string.Empty;
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
            {
                _Medicine = new clsMedicine();
            }
            if(cmbWarehouse.SelectedValue==null)
            {
                MessageBox.Show("Please add Warehouse");
                return;
            }    

            _Medicine.MedicineName = txtMedicneName.Text.Trim();
            _Medicine.MedicinePrice = Convert.ToDecimal(txtMedicinePrice.Text);
            _Medicine.MedicineCost = Convert.ToDecimal(txtMedicineCost.Text);

            _Medicine.CurrentStock = nudCurrentStock.Value;
            _Medicine.ReorderLevel = nudReorderLevel.Value;
            _Medicine.TaxRate = Convert.ToInt32(nudTaxRate.SelectedValue);
            _Medicine.IncomeAccount = IncomeAccount;
            _Medicine.AssetAccount = AssetAccount;
            _Medicine.COGSAccount = COGSAccount;
            _Medicine.WarehouseId = Convert.ToInt32(cmbWarehouse.SelectedValue);
            _Medicine.BatchNo = txtBatchNo.Text.Trim();
            _Medicine.ExpiryDate = dtpExpiryDate.Value;

            if (_Medicine.Save())
            {
                ShowSuccess("Medicine saved successfully.");

                Mode = enMode.AddNew;
                _Medicine = null;
                btnSave.Text = T("Add New Drug");

                _ResetFaild();
            }
            else
            {
                ShowError("Failed to save medicine.");
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

            // Allow decimal quantities and larger stock values.
            nudCurrentStock.DecimalPlaces = 3;
            nudReorderLevel.DecimalPlaces = 3;

            nudCurrentStock.Maximum = 99999999999.999M;
            nudReorderLevel.Maximum = 99999999999.999M;

            // Ensure the controls can display existing values.
            nudCurrentStock.Minimum = Math.Min(0M, _Medicine.CurrentStock);
            nudReorderLevel.Minimum = Math.Min(0M, _Medicine.ReorderLevel);

            btnSave.Text = T("Update Drug");
            Mode = enMode.Update;

            txtMedicneName.Text = _Medicine.MedicineName;
            txtMedicinePrice.Text = _Medicine.MedicinePrice.ToString();
            txtMedicineCost.Text = _Medicine.MedicineCost.ToString();

            nudCurrentStock.Value = _Medicine.CurrentStock;
            nudReorderLevel.Value = _Medicine.ReorderLevel;
            nudTaxRate.SelectedValue = _Medicine.TaxRate;

            if (_Medicine.WarehouseId != 0)
            {
                cmbWarehouse.SelectedValue = _Medicine.WarehouseId;
            }
            if (_Medicine.IncomeAccount != 0)
            {
                IncomeAccount = _Medicine.IncomeAccount;
            }
            if (_Medicine.AssetAccount != 0)
            {
                AssetAccount = _Medicine.AssetAccount;
            }
            if (_Medicine.COGSAccount != 0)
            {
                COGSAccount = _Medicine.COGSAccount;
            }
            if(_Medicine.TaxRate !=0)
            {
                nudTaxRate.SelectedValue = Convert.ToInt32(_Medicine.TaxRate);
            }
            if (_Medicine.BatchNo != null)
            {
                txtBatchNo.Text = _Medicine.BatchNo;
            }
            if (_Medicine.ExpiryDate != DateTime.MinValue)
            {
                dtpExpiryDate.Value = _Medicine.ExpiryDate;
            }
            if (clsMedicine.IsUsedInTransactions(MedicineId) && _Medicine.CurrentStock > 0)
            {
                nudCurrentStock.Enabled =  txtBatchNo.Enabled =  dtpExpiryDate.Enabled = false;
            }
            else
            {
                nudCurrentStock.Enabled = txtBatchNo.Enabled = dtpExpiryDate.Enabled = true;
            }

        }
        private void dgMedicines_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgMedicines.Columns[e.ColumnIndex].Name == "Actions" && e.RowIndex >= 0)
            {
                int medicineID = (int)dgMedicines.Rows[e.RowIndex].Cells["MedicineID"].Value;

                string action = dgMedicines.Rows[e.RowIndex].Cells["Actions"].Value.ToString();

                DialogResult result = MessageBox.Show(T("Choose action: Yes for Edit, No for Delete"),
                                                     T("Medicine Action"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _UpdateMedicine(medicineID);
                }
                else if (result == DialogResult.No)
                {
                    // Block deletion if the medicine has inventory transactions
                    if (clsMedicine.IsUsedInTransactions(medicineID))
                    {
                        ShowWarning(
                            "This medicine cannot be deleted because it has inventory transactions."
                        );
                        return;
                    }

                    if (ConfirmAction("Are you sure you want to delete this medicine?")
                        == DialogResult.Yes)
                    {
                        if (clsMedicine.DeleteMedicine(medicineID))
                        {
                            ShowSuccess("Deleted Successfully");
                            _RefreshMedicinesList();
                        }
                        else
                        {
                            ShowError("Failed to delete medicine.");
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
                errorProvider1.SetError(txtMedicneName, T("Medicine name cannot be blank!"));
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
                errorProvider1.SetError(txtMedicinePrice, T("Please enter a valid price greater than 0!"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtMedicinePrice, "");
            }
        }

        private void txtMedicineCost_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtMedicineCost_Validating(object sender, CancelEventArgs e)
        {
            decimal price;
            if (string.IsNullOrWhiteSpace(txtMedicineCost.Text) || !decimal.TryParse(txtMedicineCost.Text, out price) || price <= 0)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtMedicineCost, T("Please enter a valid cost greater than 0!"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtMedicineCost, "");
            }
        }
    }
}
