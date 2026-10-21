using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmUomCategory : Form
    {
        public frmUomCategory()
        {
            InitializeComponent();
            headerUC1.FormText = this.Text;
            Localization.LocalizationManager.LocalizeForm(this);
        }

        private void frmUomCategory_Load(object sender, EventArgs e)
        {
            LoadData();
            Localization.LocalizationManager.LocalizeDataGridViewHeaders(dgvUom);
        }

        private void LoadData()
        {
            dgvUom.Rows.Clear();

            string search =
                txtSearch.Text.Trim();

            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(@"
                    SELECT
                        c.id,
                        c.name,

                        COALESCE(
                            MAX(
                                CASE
                                    WHEN u.uom_type = 'reference'
                                    THEN u.name
                                END
                            ),
                            ''
                        ) AS reference_uom,

                        COALESCE(
                            GROUP_CONCAT(
                                u.name
                                ORDER BY
                                    CASE
                                        WHEN u.uom_type = 'reference'
                                        THEN 0
                                        ELSE 1
                                    END,
                                    u.id
                                SEPARATOR ', '
                            ),
                            ''
                        ) AS uoms,

                        c.active

                    FROM tbl_uom_category c

                    LEFT JOIN tbl_uom u
                        ON u.category_id = c.id
                       AND u.state = 0

                    WHERE c.state = 0

                      AND
                      (
                          @search = ''
                          OR c.name LIKE CONCAT('%', @search, '%')
                          OR u.name LIKE CONCAT('%', @search, '%')
                      )

                    GROUP BY
                        c.id,
                        c.name,
                        c.active

                    ORDER BY c.name;",

                    ClinicDatabase.CreateParameter(
                        "@search",
                        search)))
            {
                while (reader.Read())
                {
                    dgvUom.Rows.Add(
                        reader["id"],
                        reader["name"],
                        reader["reference_uom"],
                        reader["uoms"],
                        Convert.ToInt32(
                            reader["active"]) == 1
                    );
                }
            }
        }

        private void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            LoadData();
        }

        private void btnNew_Click(
            object sender,
            EventArgs e)
        {
            using (var frm =
                new frmViewUomCategory())
            {
                frm.ShowDialog(this);
            }

            LoadData();
        }

        private void dgvUom_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int categoryId =
                Convert.ToInt32(
                    dgvUom.Rows[e.RowIndex]
                    .Cells["colId"]
                    .Value);

            using (var frm =
                new frmViewUomCategory(categoryId))
            {
                frm.ShowDialog(this);
            }

            LoadData();
        }
    }
}