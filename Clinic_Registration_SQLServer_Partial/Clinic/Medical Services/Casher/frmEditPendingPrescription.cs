using Clinic_Business;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Casher
{
    public partial class frmEditPendingPrescription : Form
    {
        private readonly int _billID;
        private clsEDitPendingPrescriptionDetails _prescription;

        private readonly NumericUpDown _qtyEditor = new NumericUpDown();
        private bool _loadingEditorValue = false;

        public frmEditPendingPrescription(int billID)
        {
            InitializeComponent();

            _billID = billID;

            _InitializeQuantityEditor();
            _RegisterEvents();

            LoadPrescription();
        }

        private void _InitializeQuantityEditor()
        {
            _qtyEditor.Minimum = 0;
            _qtyEditor.Visible = false;
            _qtyEditor.BorderStyle = BorderStyle.None;
            _qtyEditor.TextAlign = HorizontalAlignment.Center;
            _qtyEditor.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            _qtyEditor.BackColor = Color.FromArgb(10, 50, 51);
            _qtyEditor.ForeColor = Color.White;

            _qtyEditor.ValueChanged += QtyEditor_ValueChanged;

            dgvPrescriptionItems.Controls.Add(_qtyEditor);
        }

        private void _RegisterEvents()
        {
            dgvPrescriptionItems.SelectionChanged += dgvPrescriptionItems_SelectionChanged;
            dgvPrescriptionItems.Paint += dgvPrescriptionItems_Paint;
            dgvPrescriptionItems.Scroll += dgvPrescriptionItems_Scroll;
            dgvPrescriptionItems.DataError += dgvPrescriptionItems_DataError;
        }

        private void LoadPrescription()
        {
            try
            {
                _prescription = new clsEDitPendingPrescriptionDetails(_billID);

                if (_prescription == null || _prescription.dtDispensedItems.Rows.Count==0 )
                {
                    MessageBox.Show(
                        "The prescription could not be loaded.",
                        "Load Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    Close();
                    return;
                }

                lblPrescriptionIDVal.Text = _prescription.PrescriptionID.ToString();
                lblDateVal.Text = _prescription.PrescriptionDate.ToString("dd/MM/yyyy");

                EnsureQtyReturnedColumnExists();

                dgvPrescriptionItems.DataSource = _prescription.dtDispensedItems;

                ConfigureGridColumns();

                ShowQuantityEditor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An unexpected error occurred while loading the prescription.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Close();
            }
        }

        private void EnsureQtyReturnedColumnExists()
        {
            if (_prescription.dtDispensedItems.Columns.Contains("QtyReturned"))
                return;

            DataColumn column = new DataColumn("QtyReturned", typeof(int))
            {
                DefaultValue = 0
            };

            _prescription.dtDispensedItems.Columns.Add(column);
        }

        private void ConfigureGridColumns()
        {
            if (dgvPrescriptionItems.Columns.Contains("PrescriptionDetailsID"))
                dgvPrescriptionItems.Columns["PrescriptionDetailsID"].Visible = false;

            dgvPrescriptionItems.Columns["SavedMedicineName"].HeaderText = "Medicine Name";
            dgvPrescriptionItems.Columns["SavedMedicineName"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            dgvPrescriptionItems.Columns["SavedMedicineName"].MinimumWidth = 350;
            dgvPrescriptionItems.Columns["SavedMedicineName"].ReadOnly = true;

            dgvPrescriptionItems.Columns["DispensedQuantity"].HeaderText = "Qty Bought";
            dgvPrescriptionItems.Columns["DispensedQuantity"].Width = 140;
            dgvPrescriptionItems.Columns["DispensedQuantity"].ReadOnly = true;

            dgvPrescriptionItems.Columns["SavedMedicinePrice"].HeaderText = "Unit Price";
            dgvPrescriptionItems.Columns["SavedMedicinePrice"].Width = 130;
            dgvPrescriptionItems.Columns["SavedMedicinePrice"].ReadOnly = true;

            dgvPrescriptionItems.Columns["QtyReturned"].HeaderText = "Qty Returned";
            dgvPrescriptionItems.Columns["QtyReturned"].Width = 140;
            dgvPrescriptionItems.Columns["QtyReturned"].ReadOnly = true;

            dgvPrescriptionItems.Columns["QtyReturned"].DefaultCellStyle.BackColor =
                Color.FromArgb(224, 255, 255);

            dgvPrescriptionItems.Columns["QtyReturned"].DefaultCellStyle.ForeColor =
                Color.Black;

            dgvPrescriptionItems.SelectionMode =
                DataGridViewSelectionMode.CellSelect;

            dgvPrescriptionItems.AllowUserToAddRows = false;
            dgvPrescriptionItems.RowHeadersVisible = false;
        }

        private void ShowQuantityEditor()
        {
            if (dgvPrescriptionItems.CurrentCell == null)
            {
                _qtyEditor.Visible = false;
                return;
            }

            if (dgvPrescriptionItems.CurrentCell.OwningColumn.Name != "QtyReturned")
            {
                _qtyEditor.Visible = false;
                return;
            }

            int rowIndex = dgvPrescriptionItems.CurrentCell.RowIndex;
            int columnIndex = dgvPrescriptionItems.CurrentCell.ColumnIndex;

            DataGridViewRow row = dgvPrescriptionItems.Rows[rowIndex];

            int dispensedQty =
                Convert.ToInt32(row.Cells["DispensedQuantity"].Value);

            if (dispensedQty <= 0)
            {
                _qtyEditor.Visible = false;
                return;
            }

            Rectangle rect =
                dgvPrescriptionItems.GetCellDisplayRectangle(
                    columnIndex,
                    rowIndex,
                    true);

            if (rect.Width == 0 || rect.Height == 0)
            {
                _qtyEditor.Visible = false;
                return;
            }

            _qtyEditor.Maximum = dispensedQty;
            _qtyEditor.Location = rect.Location;
            _qtyEditor.Size = rect.Size;

            _loadingEditorValue = true;

            object value = row.Cells["QtyReturned"].Value;

            _qtyEditor.Value =
                value == null || value == DBNull.Value
                ? 0
                : Convert.ToDecimal(value);

            _loadingEditorValue = false;

            _qtyEditor.Visible = true;
            _qtyEditor.Focus();
            _qtyEditor.Select(0, _qtyEditor.Text.Length);
        }
        private void QtyEditor_ValueChanged(object sender, EventArgs e)
        {
            if (_loadingEditorValue)
                return;

            if (dgvPrescriptionItems.CurrentCell == null)
                return;

            if (dgvPrescriptionItems.CurrentCell.OwningColumn.Name != "QtyReturned")
                return;

            dgvPrescriptionItems.CurrentCell.Value = (int)_qtyEditor.Value;
        }

        private void dgvPrescriptionItems_SelectionChanged(object sender, EventArgs e)
        {
            ShowQuantityEditor();
        }

        private void dgvPrescriptionItems_Paint(object sender, PaintEventArgs e)
        {
            if (_qtyEditor.Visible)
                ShowQuantityEditor();
        }

        private void dgvPrescriptionItems_Scroll(object sender, ScrollEventArgs e)
        {
            ShowQuantityEditor();
        }

        private void dgvPrescriptionItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private bool HasReturnedItems()
        {
            return _prescription.dtDispensedItems.AsEnumerable()
                .Any(r => r.Field<int>("QtyReturned") > 0);
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            dgvPrescriptionItems.EndEdit();

            CurrencyManager cm =
                (CurrencyManager)BindingContext[dgvPrescriptionItems.DataSource];

            cm.EndCurrentEdit();

            if (!HasReturnedItems())
            {
                MessageBox.Show(
                    "Please specify the returned quantity for at least one medication before saving.",
                    "Nothing to Save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            btnSaveChanges.Enabled = false;

            try
            {
                bool result = _prescription.UpdateAfterEditPendingPrescription(
                    clsGlobal.CurrentUser.UserID);

                if (result)
                {
                    MessageBox.Show(
                        "The prescription has been updated successfully.",
                        "Update Completed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "The prescription could not be updated. Please try again.",
                        "Update Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An unexpected error occurred.\n\n{ex.Message}",
                    "Unexpected Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnSaveChanges.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}