using Clinic;
using Clinic_DataAccess;
using DocumentFormat.OpenXml.Office2010.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YamyProject.DAL;
using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmAddUnitsToItem : Form
    {
        private EventHandler itemUnitUpdatedHandler;

        public List<int> unitIds = new List<int>();
        public List<decimal> unitFactors = new List<decimal>();
        public List<string> unitNames = new List<string>();
        private int itemId;
        private string itemCode;
        private string itemName;
        private int defaultUnitId;
        private string defaultUnitName;

        public frmAddUnitsToItem(int _itemId = 0, string _itemCode = "", string _itemName = "", int _defaultUnitId = 0, string _defaultUnitName = "")
        {
            InitializeComponent();
            LocalizationManager.LocalizeForm(this);
            itemUnitUpdatedHandler = (sender, args) => BindCombos.PopulateItemUnit(cmbUnit);
            EventHub.ItemUnit += itemUnitUpdatedHandler;
            this.itemId = _itemId;
            this.itemCode = _itemCode;
            this.itemName = _itemName;
            this.defaultUnitId = _defaultUnitId;
            this.defaultUnitName = _defaultUnitName;
            //headerUC1.FormText = id == 0 ? "Add Item UOM" : "Edit Item UOM";
        }

        private void frmAddUnitsToItem_FormClosing(object sender, FormClosingEventArgs e)
        {
            EventHub.ItemUnit -= itemUnitUpdatedHandler;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAddUnitsToItem_Load(object sender, EventArgs e)
        {
            BindCombos.PopulateItemUnit(cmbUnit);
            if (itemId != 0)
            {
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select tbl_items_unit.* , tbl_unit.name from tbl_items_unit  inner join tbl_unit on tbl_items_unit.unit_id = tbl_unit.id where item_id =@id", ClinicDatabase.CreateParameterNew("id", itemId)))
                {
                    while (reader.Read())
                    {
                        if (int.Parse(reader["unit_id"].ToString()) == defaultUnitId)
                            continue;
                        unitIds.Add(int.Parse(reader["unit_id"].ToString()));
                        unitFactors.Add(decimal.Parse(reader["factor"].ToString()));
                        unitNames.Add(reader["name"].ToString());
                    }
                    for (int i = 0; i < unitIds.Count; i++)
                    {
                        dgvLvl1.Rows.Add(unitIds[i].ToString(), unitNames[i].ToString(), unitFactors[i].ToString());
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            UpdateItemUnitLists(unitIds, unitFactors, unitNames);
            ClinicDatabase.ExecuteNonQuery("delete from tbl_items_unit where item_id=@id", ClinicDatabase.CreateParameterNew("id", itemId));
            insertItemUnits(itemId);

            this.Close();
        }
        void insertItemUnits(int id)
        {
            ClinicDatabase.ExecuteReader(@"insert into tbl_items_unit (item_id,unit_id,factor) values (@id,@unit_id,@factor)",
                   ClinicDatabase.CreateParameterNew("id", id),
                   ClinicDatabase.CreateParameterNew("unit_id", defaultUnitId),
                   ClinicDatabase.CreateParameterNew("factor", 1));
            Utilities.LogAudit(clsGlobal.CurrentUser.UserID, "Insert Item Unit", "Item Unit", id, "Inserted Main Unit: " + defaultUnitName + " for Item: " + itemName);

            if (unitFactors != null)
                for (int i = 0; i < unitFactors.Count; i++)
                {
                    ClinicDatabase.ExecuteReader(@"insert into tbl_items_unit (item_id,unit_id,factor) values (@id,@unit_id,@factor)",
                        ClinicDatabase.CreateParameterNew("id", id),
                        ClinicDatabase.CreateParameterNew("unit_id", unitIds[i]),
                        ClinicDatabase.CreateParameterNew("factor", unitFactors[i]));
                    Utilities.LogAudit(clsGlobal.CurrentUser.UserID, "Insert Item Unit", "Item Unit", id, "Inserted Sub Unit: " + unitNames[i] + " for Item: " + itemName);
                }
        }

        private void UpdateItemUnitLists(
            List<int> unitIds,
            List<decimal> unitFactors,
            List<string> unitNames)
        {
            unitIds.Clear();
            unitFactors.Clear();
            unitNames.Clear();

            foreach (DataGridViewRow row in dgvLvl1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells["unitid"].Value == null ||
                    row.Cells["factor"].Value == null ||
                    row.Cells["name"].Value == null)
                    continue;

                unitIds.Add(Convert.ToInt32(row.Cells["unitid"].Value));
                unitFactors.Add(Convert.ToDecimal(row.Cells["factor"].Value));
                unitNames.Add(row.Cells["name"].Value.ToString());
            }
        }

        private void lnkNewUnit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmViewItemUnit().ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbUnit.SelectedValue == null || string.IsNullOrWhiteSpace(cmbUnit.Text))
            {
                MessageBox.Show("Choose Unit First");
                return;
            }

            if (!decimal.TryParse(txtFactor.Text, out decimal factor) || factor <= 0)
            {
                MessageBox.Show("Factor must be greater than zero.");
                txtFactor.Focus();
                return;
            }

            int unitId = Convert.ToInt32(cmbUnit.SelectedValue);

            // Don't compare unit by name if you can compare IDs
            if (unitId == Convert.ToInt32(defaultUnitId))
            {
                MessageBox.Show("Main Unit Can't Be Added As Sub Unit");
                return;
            }

            bool exists = dgvLvl1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Any(r =>
                    r.Cells["unitid"].Value != null &&
                    Convert.ToInt32(r.Cells["unitid"].Value) == unitId);

            if (exists)
            {
                MessageBox.Show("Unit Already Exists");
                return;
            }

            dgvLvl1.Rows.Add(
                unitId,
                cmbUnit.Text,
                factor
            );

            cmbUnit.SelectedIndex = -1;
            txtFactor.Clear();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvLvl1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to remove.");
                return;
            }
            dgvLvl1.Rows.Remove(dgvLvl1.SelectedRows[0]);
        }

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal; // Restore down
            }
            else
            {
                this.WindowState = FormWindowState.Maximized; // Maximize
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}

public class ItemUnitOption
{
    public int UnitId { get; set; }
    public string UnitName { get; set; }
    public decimal Factor { get; set; }
    public bool IsDefault { get; set; }
}