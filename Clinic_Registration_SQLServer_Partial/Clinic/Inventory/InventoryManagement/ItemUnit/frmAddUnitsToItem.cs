using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmAddUnitsToItem : Form
    {
        private int id;
        private frmViewItem _item;
        private EventHandler itemUnitUpdatedHandler;

        public frmAddUnitsToItem(
            frmViewItem item,
            int id = 0)
        {
            InitializeComponent();

           // LocalizationManager.LocalizeForm(this);

            this._item = item;
            this.id = id;

            itemUnitUpdatedHandler =
                delegate (object sender, EventArgs args)
                {
                    BindCombos.PopulateItemUnit(cmbUnit);
                };

            //.ItemUnit +=
                ///itemUnitUpdatedHandler;

            headerUC1.FormText =
                id == 0
                    ? "Add Item UOM"
                    : "Edit Item UOM";
        }


        private void frmAddUnitsToItem_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            try
            {
                if (itemUnitUpdatedHandler != null)
                {
                    //.ItemUnit -=
                        //itemUnitUpdatedHandler;
                }
            }
            catch
            {
                // Do not block closing the form.
            }
        }


        private void btnClose_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }


        private void frmAddUnitsToItem_Load(
            object sender,
            EventArgs e)
        {
            try
            {
                //AppTheme.DefaultFont(
                    //dgvLvl1);

                BindCombos.PopulateItemUnit(
                    cmbUnit);


                if (_item == null)
                {
                    MessageBox.Show(
                        "The Item form could not be found.",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                if (id != 0)
                {
                    dgvLvl1.Rows.Clear();

                    int count =
                        Math.Min(
                            _item.unitIds.Count,
                            Math.Min(
                                _item.unitNames.Count,
                                _item.unitFactors.Count));


                    for (int i = 0; i < count; i++)
                    {
                        dgvLvl1.Rows.Add(
                            _item.unitIds[i],
                            _item.unitNames[i],
                            _item.unitFactors[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Item units could not be loaded.\n\n" +
                    GetDeepestMessage(ex),
                    "Item Unit",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void btnSave_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (_item == null)
                {
                    MessageBox.Show(
                        "The Item form is not available.",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                _item.unitIds.Clear();
                _item.unitFactors.Clear();
                _item.unitNames.Clear();


                UpdateItemUnitLists(
                    _item.unitIds,
                    _item.unitFactors,
                    _item.unitNames);


                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Item units could not be saved.\n\n" +
                    GetDeepestMessage(ex),
                    "Item Unit",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void UpdateItemUnitLists(
            List<int> unitIds,
            List<decimal> unitFactors,
            List<string> unitNames)
        {
            foreach (DataGridViewRow row in dgvLvl1.Rows)
            {
                if (row.IsNewRow)
                    continue;


                object unitIdValue =
                    row.Cells["unitid"].Value;

                object factorValue =
                    row.Cells["factor"].Value;

                object nameValue =
                    row.Cells["name"].Value;


                if (unitIdValue == null ||
                    factorValue == null ||
                    nameValue == null)
                {
                    continue;
                }


                int unitId;

                decimal factor;


                if (!int.TryParse(
                        unitIdValue.ToString(),
                        out unitId))
                {
                    continue;
                }


                if (!decimal.TryParse(
                        factorValue.ToString(),
                        out factor))
                {
                    continue;
                }


                unitIds.Add(
                    unitId);

                unitFactors.Add(
                    factor);

                unitNames.Add(
                    nameValue.ToString());
            }
        }


        private void lnkNewUnit_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                using (frmViewItemUnit frm =
                    new frmViewItemUnit())
                {
                    frm.ShowDialog(this);
                }


                BindCombos.PopulateItemUnit(
                    cmbUnit);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The Unit page could not be opened.\n\n" +
                    GetDeepestMessage(ex),
                    "Item Unit",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void btnAdd_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (_item == null)
                {
                    MessageBox.Show(
                        "The Item form is not available.",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                if (cmbUnit.SelectedValue == null ||
                    string.IsNullOrWhiteSpace(
                        cmbUnit.Text))
                {
                    MessageBox.Show(
                        "Choose Unit First",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    cmbUnit.Focus();

                    return;
                }


                decimal factor;


                if (!decimal.TryParse(
                        txtFactor.Text,
                        out factor))
                {
                    MessageBox.Show(
                        "Please enter a valid factor value.",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtFactor.Focus();

                    return;
                }


                if (factor <= 0)
                {
                    MessageBox.Show(
                        "Factor must be greater than zero.",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtFactor.Focus();

                    return;
                }


                string mainUnit =
                    GetMainUnitName();

                if (string.Equals(
                        cmbUnit.Text.Trim(),
                        mainUnit.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Main Unit Can't Be Added As Sub Unit",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                bool exists =
                    dgvLvl1.Rows
                        .Cast<DataGridViewRow>()
                        .Where(
                            row =>
                                !row.IsNewRow)
                        .Any(
                            row =>
                                row.Cells["name"].Value != null &&
                                string.Equals(
                                    row.Cells["name"].Value
                                        .ToString()
                                        .Trim(),
                                    cmbUnit.Text.Trim(),
                                    StringComparison.OrdinalIgnoreCase));


                if (exists)
                {
                    MessageBox.Show(
                        "Unit Already Exists",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }


                dgvLvl1.Rows.Add(
                    cmbUnit.SelectedValue,
                    cmbUnit.Text,
                    factor);


                cmbUnit.SelectedIndex =
                    -1;

                txtFactor.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The Unit could not be added.\n\n" +
                    GetDeepestMessage(ex),
                    "Item Unit",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void btnRemove_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (dgvLvl1.SelectedRows.Count == 0)
                {
                    MessageBox.Show(
                        "Please select a row to remove.",
                        "Item Unit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }


                DataGridViewRow row =
                    dgvLvl1.SelectedRows[0];


                if (!row.IsNewRow)
                {
                    dgvLvl1.Rows.Remove(
                        row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The Unit could not be removed.\n\n" +
                    GetDeepestMessage(ex),
                    "Item Unit",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void LbHeader_Click(
            object sender,
            EventArgs e)
        {
            BringToFront();
        }


        private void guna2Button4_Click(
            object sender,
            EventArgs e)
        {
            if (WindowState ==
                FormWindowState.Maximized)
            {
                WindowState =
                    FormWindowState.Normal;
            }
            else
            {
                WindowState =
                    FormWindowState.Maximized;
            }
        }


        private void guna2Button1_Click(
            object sender,
            EventArgs e)
        {
            WindowState =
                FormWindowState.Minimized;
        }


        private string GetMainUnitName()
        {
            if (_item == null)
            {
                return string.Empty;
            }

            Control mainUnitControl =
                FindControlRecursive(
                    _item,
                    "cmbMainUnit");

            if (mainUnitControl == null)
            {
                return string.Empty;
            }

            return mainUnitControl.Text ?? string.Empty;
        }


        private static Control FindControlRecursive(
            Control parent,
            string controlName)
        {
            if (parent == null ||
                string.IsNullOrWhiteSpace(controlName))
            {
                return null;
            }

            foreach (Control child in parent.Controls)
            {
                if (string.Equals(
                        child.Name,
                        controlName,
                        StringComparison.Ordinal))
                {
                    return child;
                }

                Control found =
                    FindControlRecursive(
                        child,
                        controlName);

                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }


        private static string GetDeepestMessage(
            Exception ex)
        {
            if (ex == null)
            {
                return "Unknown error.";
            }


            while (ex.InnerException != null)
            {
                ex =
                    ex.InnerException;
            }


            return ex.Message;
        }
    }
}