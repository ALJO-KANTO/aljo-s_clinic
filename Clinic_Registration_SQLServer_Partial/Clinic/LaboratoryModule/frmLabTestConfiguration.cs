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
            ApplyConfigurationLanguage();
            Load += frmLabTestConfiguration_Load;
            btnAddParameter.Click += btnAddParameter_Click;
            btnSave.Click += btnSave_Click;
            btnClose.Click += (s, e) => Close();
            btnRemoveParameter.Click += btnRemoveParameter_Click;
            dgvParameters.DataError += (s, e) => { e.ThrowException = false; };
        }

        private void UpdateParameterCount()
        {
            int count = _parameters == null ? 0 :
                _parameters.Select(null, null, DataViewRowState.CurrentRows).Length;
            lblCount.Text = TranslationClass.IsArabic
                ? count + " معلمات" : count + " parameters";
        }

        private void ApplyConfigurationLanguage()
        {
            bool ar = TranslationClass.IsArabic;
            // Keep form-level automatic mirroring OFF: manually mirror only the
            // three fixed-layout panels, avoiding double mirroring and clipping.
            RightToLeft = RightToLeft.No;
            RightToLeftLayout = false;
            Text = ar ? "إعداد فحوصات المختبر" : "Lab Test Configuration";
            lblTitle.Text = Text;
            lblName.Text = ar ? "اسم الفحص" : "Test Name";
            lblPrice.Text = ar ? "سعر الفحص" : "Test Price";
            lblTestID.Text = ar ? "معرّف الفحص:" : "Test ID:";
            lblHint.Text = ar
                ? "حدّد المجال المرجعي لكل معلمة للذكور والإناث. اترك المجال فارغاً إذا لم ينطبق."
                : "Set each parameter's reference range for male and female patients. Leave a range blank when not applicable.";
            btnAddParameter.Text = ar ? "+ إضافة معلمة" : "+ Add Parameter";
            btnRemoveParameter.Text = ar ? "إزالة المحدد" : "Remove Selected";
            btnSave.Text = ar ? "حفظ الإعدادات" : "Save Configuration";
            btnClose.Text = ar ? "إغلاق" : "Close";
            colParameterName.HeaderText = ar ? "المعلمة" : "Parameter";
            colMaleRange.HeaderText = ar ? "المجال الطبيعي (ذكور)" : "Normal Range (Male)";
            colFemaleRange.HeaderText = ar ? "المجال الطبيعي (إناث)" : "Normal Range (Female)";
            colUnit.HeaderText = ar ? "وحدة القياس" : "UOM";
            colResultType.HeaderText = ar ? "نوع قيمة النتيجة" : "Result Value Type";
            colGeneralRange.HeaderText = ar ? "المجال العام / ملاحظة" : "General Range / Remark";
            colDisplayOrder.HeaderText = ar ? "الترتيب" : "Order";
            dgvParameters.RightToLeft = ar ? RightToLeft.Yes : RightToLeft.No;
            dgvParameters.ColumnHeadersDefaultCellStyle.Alignment = ar
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;
            dgvParameters.DefaultCellStyle.Alignment = ar
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;
            dgvParameters.ColumnHeadersHeight = ar ? 48 : 42;
            dgvParameters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Localized display labels with unchanged English database values.
            var types = new DataTable();
            types.Columns.Add("Value", typeof(string));
            types.Columns.Add("Caption", typeof(string));
            types.Rows.Add("Quantitative", ar ? "كمي" : "Quantitative");
            types.Rows.Add("Qualitative", ar ? "نوعي" : "Qualitative");
            types.Rows.Add("Text", ar ? "نصي" : "Text");
            colResultType.Items.Clear();
            colResultType.DataSource = types;
            colResultType.ValueMember = "Value";
            colResultType.DisplayMember = "Caption";

            lblTitle.TextAlign = ar ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            lblHint.TextAlign = ar ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            lblTestID.TextAlign = ar ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;
            lblCount.TextAlign = ar ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            txtTestName.RightToLeft = ar ? RightToLeft.Yes : RightToLeft.No;
            txtTestPrice.RightToLeft = RightToLeft.No;
            UpdateParameterCount();

            Action layout = () =>
            {
                int w = pnlHeader.ClientSize.Width;
                // Responsive positioning; Arabic order is a true mirror of English.
                int nameWidth = Math.Min(455, Math.Max(240, (w - 250) / 2));
                int priceWidth = 160;
                if (!ar)
                {
                    lblTitle.SetBounds(20, 14, Math.Max(300, w - 350), 36);
                    lblTestID.SetBounds(Math.Max(0, w - 220), 23, 200, 28);
                    lblName.SetBounds(22, 70, 100, 28);
                    txtTestName.SetBounds(120, 65, nameWidth, 30);
                    lblPrice.SetBounds(Math.Min(w - 300, 605), 70, 100, 28);
                    txtTestPrice.SetBounds(Math.Min(w - 180, 705), 65, priceWidth, 30);
                    lblHint.SetBounds(22, 110, Math.Max(250, w - 44), 32);
                }
                else
                {
                    lblTitle.SetBounds(220, 14, Math.Max(300, w - 240), 36);
                    lblTestID.SetBounds(20, 23, 200, 28);
                    lblName.SetBounds(Math.Max(0, w - 125), 70, 103, 28);
                    txtTestName.SetBounds(Math.Max(0, w - 125 - nameWidth - 8), 65, nameWidth, 30);
                    lblPrice.SetBounds(190, 70, 105, 28);
                    txtTestPrice.SetBounds(20, 65, priceWidth, 30);
                    lblHint.SetBounds(22, 110, Math.Max(250, w - 44), 32);
                }
                int actionW = pnlActions.ClientSize.Width;
                if (ar)
                {
                    btnAddParameter.Left = actionW - btnAddParameter.Width - 20;
                    btnRemoveParameter.Left = btnAddParameter.Left - btnRemoveParameter.Width - 12;
                    lblCount.Left = Math.Max(10, btnRemoveParameter.Left - 125);
                }
                else
                {
                    btnAddParameter.Left = 20;
                    btnRemoveParameter.Left = btnAddParameter.Right + 12;
                    lblCount.Left = btnRemoveParameter.Right + 20;
                }
                int footerW = btnSave.Parent.ClientSize.Width;
                if (ar)
                {
                    btnClose.Left = 22;
                    btnSave.Left = btnClose.Right + 12;
                }
                else
                {
                    btnClose.Left = footerW - btnClose.Width - 22;
                    btnSave.Left = btnClose.Left - btnSave.Width - 12;
                }
            };
            pnlHeader.Resize += (s, e) => layout();
            pnlActions.Resize += (s, e) => layout();
            btnSave.Parent.Resize += (s, e) => layout();
            Shown += (s, e) => layout();
            layout();
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
                            lblTestID.Text = (TranslationClass.IsArabic ? "معرّف الفحص: " : "Test ID: ") + _testID;
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
                UpdateParameterCount();
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
            UpdateParameterCount();
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
            if (MessageBox.Show(this, (TranslationClass.IsArabic ? "هل تريد إزالة هذه المعلمة من إعدادات الفحص النشطة؟" : "Remove this parameter from the active test configuration?"),
                (TranslationClass.IsArabic ? "تأكيد" : "Confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            // Removed existing parameters are disabled on Save, not physically deleted.
            view.Delete();
            UpdateParameterCount();
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
                        MessageBox.Show(this, (TranslationClass.IsArabic ? "يجب إدخال اسم لكل معلمة." : "Every parameter must have a name."),
                            (TranslationClass.IsArabic ? "التحقق من البيانات" : "Validation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(this, (TranslationClass.IsArabic ? "تم حفظ معلمات المختبر بنجاح." : "Laboratory parameters saved successfully."),
                    (TranslationClass.IsArabic ? "تم الحفظ" : "Saved"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Save Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
