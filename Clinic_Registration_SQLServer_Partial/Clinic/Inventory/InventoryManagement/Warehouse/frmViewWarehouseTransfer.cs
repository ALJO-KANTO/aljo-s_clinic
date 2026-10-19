
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;


namespace YamyProject
{
    public partial class frmViewWarehouseTransfer : Form
    {
        DataTable dt = new DataTable();
        int id;
        int itemId;
        int warehouseId;

        public frmViewWarehouseTransfer(int _id=0, int _itemId = 0,int _warehouseId = 0)
        {
            InitializeComponent();
           // LocalizationManager.LocalizeForm(this);
            headerUC1.FormText = "Warehouse - Transfer History";
            this.id = _id;
            this.itemId = _itemId;
            this.warehouseId = _warehouseId;
        }
        public DataTable COMPANYINFO(int id)
        {
            return ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_company ", ClinicDatabase.CreateParameterNew("@1", id));
        }
        private void frmViewWarehouseTransfer_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvFrom);
            LoadData();
            dgvFrom.DataSource = dt;
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvFrom);
        }

        private void LoadData()
        {
            string query = @"select wt.Date, concat(w1.code,' - ',w1.name) as 'Warehouse From',concat(w2.code,' - ',w2.name) as 'Warehouse To',
                                                    concat(i.code,' - ',i.name) as 'Item Name' ,wt.qty, wt.description from tbl_item_warehouse_transaction wt inner join
                                                    tbl_warehouse w1 on wt.warehouse_from = w1.id inner join tbl_warehouse w2 on wt.warehouse_to = w2.id inner join tbl_items i
                                                    on wt.item_id = i.id";

            var parameters = new List<MySqlParameter>();

            if (id > 0)
            {
                query += " and wt.id = @id";
                parameters.Add(new MySqlParameter("id", id));
            }
            if (itemId > 0)
            {
                query += " and i.id = @itemId";
                parameters.Add(new MySqlParameter("itemId", itemId));
            }
            //if (warehouseId > 0)
            //{
            //    query += " and wt.warehouse_to = @warehouseId";
            //    parameters.Add(new MySqlParameter("warehouseId", warehouseId));
            //}

            DataTable dt = ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
            if (dt != null && dt.Rows.Count > 0)
                {
                this.dt = dt;
            }
            else
            {
                MessageBox.Show("No transfer available for the report.");
                this.dt = new DataTable();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2TileButton18_Click(object sender, EventArgs e)
        {
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
    }
}
