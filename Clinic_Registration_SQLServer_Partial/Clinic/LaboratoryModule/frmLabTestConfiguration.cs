using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Clinic_DataAccess;

namespace Clinic.Medical_Services.Laboratory
{
    public partial class frmLabTestConfiguration : Form
    {
        private readonly int _testID;
        private DataTable _parameters;
        private bool _loading;

        // Open after SaveLabTest returns a valid TestID.
        public frmLabTestConfiguration(int testID)
        {
            if (testID <= 0)
                throw new ArgumentOutOfRangeException(nameof(testID), "Save the laboratory test first.");

            _testID = testID;
            InitializeComponent();
            Load += frmLabTestConfiguration_Load;
            btnAddParameter.Click += btnAddParameter_Click;
            btnSave.Click += btnSave_Click;
            btnClose.Click += (s, e) => Close();
            btnRemoveParameter.Click += btnRemoveParameter_Click;
            dgvParameters.DataError += (s, e) => { e.ThrowException = false; };
        }

        private MySqlConnection OpenConnection()
        {
            var connection = new MySqlConnection(clsDataAccessSettings.ConnectionString);
            connection.Open();
            return connection;
        }

        private void frmLabTestConfiguration_Load(object sender, EventArgs e)
        {
            try { LoadTestAndParameters(); }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Laboratory Configuration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTestAndParameters()
        {
            _loading = true;
            try
            {
                using (var connection = OpenConnection())
                {
                    using (var cmd = new MySqlCommand(
                        "SELECT TestName, TestPrice FROM laboratorytests WHERE TestID=@ID", connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", _testID);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new InvalidOperationException("The selected laboratory test no longer exists.");
                            txtTestName.Text = Convert.ToString(reader["TestName"]);
                            txtTestPrice.Text = Convert.ToDecimal(reader["TestPrice"]).ToString("N2");
                            lblTestID.Text = "Test ID: " + _testID;
                        }
                    }

                    _parameters = new DataTable();
                    using (var cmd = new MySqlCommand(@"
                        SELECT ParameterID, ParameterName,
                               MaleReferenceRange, FemaleReferenceRange,
                               Unit, ResultType, ReferenceRange, DisplayOrder, IsActive
                        FROM laboratorytestparameters
                        WHERE TestID = @ID AND IsActive = 1
                        ORDER BY DisplayOrder, ParameterID;", connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", _testID);
                        using (var adapter = new MySqlDataAdapter(cmd))
                            adapter.Fill(_parameters);
                    }
                }

                // DataTable can contain DBNull; the UI treats blank fields as empty strings.
                dgvParameters.AutoGenerateColumns = false;
                dgvParameters.DataSource = _parameters;
                lblCount.Text = _parameters.Rows.Count + " parameters";
            }
            finally { _loading = false; }
        }

        private void btnAddParameter_Click(object sender, EventArgs e)
        {
            if (_parameters == null) return;
            dgvParameters.EndEdit();
            DataRow row = _parameters.NewRow();
            row["ParameterName"] = "";
            row["MaleReferenceRange"] = "";
            row["FemaleReferenceRange"] = "";
            row["Unit"] = "";
            row["ResultType"] = "Quantitative";
            row["ReferenceRange"] = "";
            row["DisplayOrder"] = _parameters.Rows.Count + 1;
            row["IsActive"] = true;
            _parameters.Rows.Add(row);
            lblCount.Text = _parameters.Rows.Count + " parameters";
            int index = dgvParameters.Rows.Count - 1;
            if (index >= 0)
            {
                dgvParameters.CurrentCell = dgvParameters.Rows[index].Cells["colParameterName"];
                dgvParameters.BeginEdit(true);
            }
        }

        private void btnRemoveParameter_Click(object sender, EventArgs e)
        {
            if (dgvParameters.CurrentRow == null || dgvParameters.CurrentRow.IsNewRow) return;
            dgvParameters.EndEdit();
            var view = dgvParameters.CurrentRow.DataBoundItem as DataRowView;
            if (view == null) return;
            if (MessageBox.Show(this, "Remove this parameter from the active test configuration?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            // Removed existing parameters are disabled on Save, not physically deleted.
            view.Delete();
            lblCount.Text = _parameters.Select(null, null, DataViewRowState.CurrentRows).Length + " parameters";
        }

        private static string CellText(DataRow row, string column)
        {
            return row.IsNull(column) ? "" : Convert.ToString(row[column]).Trim();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_loading || _parameters == null) return;
            try
            {
                Validate();
                dgvParameters.EndEdit();
                if (dgvParameters.DataSource != null)
                {
                    CurrencyManager cm =
                        (CurrencyManager)BindingContext[dgvParameters.DataSource];

                    cm.EndCurrentEdit();
                }

                var activeRows = _parameters.Select(null, "DisplayOrder ASC", DataViewRowState.CurrentRows);
                foreach (DataRow row in activeRows)
                {
                    if (string.IsNullOrWhiteSpace(CellText(row, "ParameterName")))
                    {
                        MessageBox.Show(this, "Every parameter must have a name.",
                            "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                using (var connection = OpenConnection())
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int order = 0;
                        foreach (DataRow row in activeRows)
                        {
                            order++;
                            object id = row.IsNull("ParameterID") ? (object)DBNull.Value : row["ParameterID"];
                            bool existing = id != DBNull.Value && Convert.ToInt32(id) > 0;
                            string sql = existing ? @"
                                UPDATE laboratorytestparameters SET
                                    ParameterName=@Name,
                                    MaleReferenceRange=@Male,
                                    FemaleReferenceRange=@Female,
                                    Unit=@Unit, ResultType=@Type,
                                    ReferenceRange=@General, DisplayOrder=@Order,
                                    IsActive=1
                                WHERE ParameterID=@ParameterID AND TestID=@TestID;" : @"
                                INSERT INTO laboratorytestparameters
                                    (TestID, ParameterName, MaleReferenceRange, FemaleReferenceRange,
                                     Unit, ResultType, ReferenceRange, DisplayOrder, IsActive)
                                VALUES
                                    (@TestID, @Name, @Male, @Female,
                                     @Unit, @Type, @General, @Order, 1);";
                            using (var cmd = new MySqlCommand(sql, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TestID", _testID);
                                cmd.Parameters.AddWithValue("@ParameterID", id);
                                cmd.Parameters.AddWithValue("@Name", CellText(row, "ParameterName"));
                                cmd.Parameters.AddWithValue("@Male", CellText(row, "MaleReferenceRange"));
                                cmd.Parameters.AddWithValue("@Female", CellText(row, "FemaleReferenceRange"));
                                cmd.Parameters.AddWithValue("@Unit", CellText(row, "Unit"));
                                cmd.Parameters.AddWithValue("@Type", CellText(row, "ResultType"));
                                cmd.Parameters.AddWithValue("@General", CellText(row, "ReferenceRange"));
                                cmd.Parameters.AddWithValue("@Order", order);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Soft-disable removed parameters, retaining historical result links.
                        foreach (DataRow row in _parameters.Rows)
                        {
                            if (row.RowState != DataRowState.Deleted) continue;
                            object oldID = row["ParameterID", DataRowVersion.Original];
                            if (oldID == DBNull.Value) continue;
                            using (var cmd = new MySqlCommand(@"
                                UPDATE laboratorytestparameters SET IsActive=0
                                WHERE TestID=@TestID AND ParameterID=@ID;", connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TestID", _testID);
                                cmd.Parameters.AddWithValue("@ID", oldID);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch { transaction.Rollback(); throw; }
                }
                LoadTestAndParameters();
                MessageBox.Show(this, "Laboratory parameters saved successfully.",
                    "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Save Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
