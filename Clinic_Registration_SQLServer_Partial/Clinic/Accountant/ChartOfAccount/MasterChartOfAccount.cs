using Clinic;
using Clinic.Login;
using Clinic_DataAccess;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterChartOfAccount : Form
    {
        EventHandler lvl1, lvl2, lvl3, lvl4;
        public MasterChartOfAccount()
        {
            InitializeComponent();

            dgvCustomer.CellFormatting -=
                dgvCustomer_CellFormatting;

            dgvCustomer.CellFormatting +=
                dgvCustomer_CellFormatting;

           // LocalizationManager.LocalizeForm(this);

            lvl1 = lvl2 = lvl3 = lvl4 =
                (sender, args) => BindDGV();

            //.lvl1Account += lvl1;
            //.lvl2Account += lvl2;
            //.lvl3Account += lvl3;
            //.lvl4Account += lvl4;

            headerUC1.FormText = "Chart of Accounts";

            ApplyModernDesign();
        }
        private void ApplyModernDesign()
        {
            // =========================================================
            // FORM
            // =========================================================
            BackColor = Color.FromArgb(248, 248, 252);

            // =========================================================
            // GRID
            // =========================================================
            dgvCustomer.BackgroundColor = Color.White;
            dgvCustomer.BorderStyle = BorderStyle.None;

            dgvCustomer.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            // Visible soft separator exactly like reference
            dgvCustomer.GridColor =
                Color.FromArgb(224, 222, 232);

            dgvCustomer.EnableHeadersVisualStyles = false;
            dgvCustomer.RowHeadersVisible = false;

            dgvCustomer.AllowUserToAddRows = false;
            dgvCustomer.AllowUserToDeleteRows = false;
            dgvCustomer.AllowUserToResizeRows = false;

            dgvCustomer.MultiSelect = false;

            dgvCustomer.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCustomer.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;

            dgvCustomer.RowTemplate.Height = 43;

            // ---------------- ROW STYLE ----------------
            dgvCustomer.DefaultCellStyle.BackColor =
                Color.White;

            dgvCustomer.DefaultCellStyle.ForeColor =
                Color.FromArgb(52, 47, 70);

            dgvCustomer.DefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Regular);

            dgvCustomer.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(235, 238, 246);

            dgvCustomer.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(43, 30, 88);

            dgvCustomer.DefaultCellStyle.Padding =
                new Padding(12, 0, 12, 0);

            dgvCustomer.AlternatingRowsDefaultCellStyle.BackColor =
                Color.White;

            // =========================================================
            // HEADER
            // =========================================================
            dgvCustomer.ColumnHeadersVisible = true;
            dgvCustomer.ColumnHeadersHeight = 42;

            dgvCustomer.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvCustomer.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(246, 244, 250);

            dgvCustomer.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(64, 52, 90);

            dgvCustomer.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvCustomer.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(12, 0, 12, 0);

            dgvCustomer.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;

            // =========================================================
            // BOTTOM ACTION BAR
            // =========================================================
            panel1.BackColor = Color.FromArgb(248, 248, 252);
            panel1.Height = 70;
            panel1.Dock = DockStyle.Bottom;
            panel1.Padding = new Padding(0);

            // =========================================================
            // ADD ACCOUNT
            // =========================================================
            btnNew.Size = new Size(175, 42);
            btnNew.Location = new Point(20, 14);

            btnNew.Anchor =
                AnchorStyles.Left | AnchorStyles.Bottom;

            btnNew.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            btnNew.FillColor =
                Color.FromArgb(111, 55, 220);

            btnNew.ForeColor = Color.White;
            btnNew.BorderRadius = 8;
            btnNew.BorderThickness = 0;

            btnNew.HoverState.FillColor =
                Color.FromArgb(124, 69, 231);

            // =========================================================
            // EDIT ACCOUNT
            // =========================================================
            btnEdit.Size = new Size(190, 42);
            btnEdit.Location = new Point(205, 14);

            btnEdit.Anchor =
                AnchorStyles.Left | AnchorStyles.Bottom;

            btnEdit.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            btnEdit.FillColor = Color.White;

            btnEdit.ForeColor =
                Color.FromArgb(78, 58, 126);

            btnEdit.BorderColor =
                Color.FromArgb(207, 199, 225);

            btnEdit.BorderThickness = 1;
            btnEdit.BorderRadius = 8;

            btnEdit.HoverState.FillColor =
                Color.FromArgb(243, 239, 252);

            // =========================================================
            // IMPORT / EXPORT
            // =========================================================
            btnImport_Export.Size = new Size(180, 42);

            btnImport_Export.Location =
                new Point(panel1.ClientSize.Width - 200, 14);

            btnImport_Export.Anchor =
                AnchorStyles.Right | AnchorStyles.Bottom;

            btnImport_Export.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            btnImport_Export.FillColor =
                Color.FromArgb(247, 242, 255);

            btnImport_Export.ForeColor =
                Color.FromArgb(104, 55, 190);

            btnImport_Export.BorderColor =
                Color.FromArgb(198, 180, 232);

            btnImport_Export.BorderThickness = 1;
            btnImport_Export.BorderRadius = 8;

            btnImport_Export.HoverState.FillColor =
                Color.FromArgb(237, 228, 252);
        }
        private void MasterChartOfAccount_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.lvl1Account -= lvl1;
            //.lvl2Account -= lvl2;
            //.lvl3Account -= lvl3;
            //.lvl4Account -= lvl4;
        }
        private void MasterChartOfAccount_Load(object sender, EventArgs e)
        {
            AddDGVColumns();
            BindDGV();

            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            ApplyLanguageLayout(isArabic);
        }

        private void AddDGVColumns()
        {
            // Avoid adding the columns more than once
            if (dgvCustomer.Columns.Count > 0)
                return;

            dgvCustomer.Columns.Add("state", "");
            dgvCustomer.Columns.Add("loadState", "");
            dgvCustomer.Columns.Add("currLvl", "");

            dgvCustomer.Columns.Add(
                "lvl1",
                "Account Hierarchy");

            dgvCustomer.Columns.Add("id", "");

            dgvCustomer.Columns["lvl1"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;

            dgvCustomer.Columns["state"].Visible = false;
            dgvCustomer.Columns["loadState"].Visible = false;
            dgvCustomer.Columns["currLvl"].Visible = false;
            dgvCustomer.Columns["id"].Visible = false;

           // LocalizationManager.LocalizeDataGridViewHeaders(
                //dgvCustomer);
        }
        public void BindDGV()
        {
            dgvCustomer.Rows.Clear();

            using (MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    "SELECT concat(code, ' - ', name) as name, id " +
                    "FROM tbl_coa_level_1"))
            {
                while (reader.Read())
                {
                    dgvCustomer.Rows.Add(
                        "e",
                        "u",
                        "1",
                        "▸  " +
                        TranslateAccountName(
                            reader["name"].ToString()),
                        reader["id"].ToString());
                }
            }
        }

        private void dgvCustomer_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int currLvl = Convert.ToInt32(
                dgvCustomer.Rows[e.RowIndex]
                    .Cells["currLvl"].Value);

            // Level 4 is an account.
            // DO NOT open edit form by double-click.
            // User must use Edit Account / تعديل الحساب button.
            if (currLvl >= 4)
            {
                new frmEditLevel4(
                    int.Parse(
                        dgvCustomer.Rows[e.RowIndex]
                            .Cells["id"].Value.ToString()
                    )
                ).ShowDialog();

                return;
            }

            string state =
                dgvCustomer.Rows[e.RowIndex]
                    .Cells["state"].Value.ToString();

            string loadState =
                dgvCustomer.Rows[e.RowIndex]
                    .Cells["loadState"].Value.ToString();

            string cellValue =
                dgvCustomer.Rows[e.RowIndex]
                    .Cells["lvl1"].Value.ToString();

            if (state == "e")
            {
                if (loadState == "u")
                {
                    int parentId = Convert.ToInt32(
                        dgvCustomer.Rows[e.RowIndex]
                            .Cells["id"].Value);

                    using (MySqlDataReader reader =
                        ClinicDatabase.ExecuteReader(
                            @"SELECT concat(code,' - ',name) as name, id
                      FROM tbl_coa_level_" + (currLvl + 1) + @"
                      WHERE main_id = @parentId
                      ORDER BY code",
                            ClinicDatabase.CreateParameterNew(
                                "parentId",
                                parentId)))
                    {
                        int insertIndex = e.RowIndex + 1;

                        while (reader.Read())
                        {
                            string nextState =
                                (currLvl + 1 == 4) ? "n" : "e";

                            string nextIcon =
                                (currLvl + 1 == 4) ? "•" : "▸";

                            dgvCustomer.Rows.Insert(
                                insertIndex,
                                nextState,
                                "u",
                                currLvl + 1,
                                nextIcon + "  " +
                                TranslateAccountName(
                                    reader["name"].ToString()),
                                reader["id"].ToString());

                            insertIndex++;
                        }
                    }

                    dgvCustomer.Rows[e.RowIndex]
                        .Cells["loadState"].Value = "l";
                }
                else
                {
                    GetAllChildren(e.RowIndex);
                }

                dgvCustomer.Rows[e.RowIndex]
                    .Cells["lvl1"].Value =
                    cellValue.Replace("►", "▼");

                dgvCustomer.Rows[e.RowIndex]
                    .Cells["state"].Value = "c";
            }
            else if (state == "c")
            {
                ToggleChildRowsVisibility(
                    e.RowIndex,
                    currLvl,
                    false);

                dgvCustomer.Rows[e.RowIndex]
                    .Cells["lvl1"].Value =
                    cellValue.Replace("▾", "▸");

                dgvCustomer.Rows[e.RowIndex]
                    .Cells["state"].Value = "e";
            }
        }
        private void dgvCustomer_CellFormatting(
       object sender,
       DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // Only style the visible account column
            if (dgvCustomer.Columns[e.ColumnIndex].Name != "lvl1")
                return;

            DataGridViewRow row = dgvCustomer.Rows[e.RowIndex];

            if (row.Cells["currLvl"].Value == null)
                return;

            int level = Convert.ToInt32(
                row.Cells["currLvl"].Value);

            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            // =========================================================
            // LEVEL DESIGN
            // =========================================================
            if (level == 1)
            {
                e.CellStyle.Font =
                    new Font(
                        "Segoe UI",
                        9.5F,
                        FontStyle.Bold);

                e.CellStyle.ForeColor =
                    Color.FromArgb(47, 36, 82);

                e.CellStyle.BackColor =
                    Color.FromArgb(250, 250, 252);
            }
            else if (level == 2)
            {
                e.CellStyle.Font =
                    new Font(
                        "Segoe UI",
                        9.25F,
                        FontStyle.Bold);

                e.CellStyle.ForeColor =
                    Color.FromArgb(65, 55, 90);

                e.CellStyle.BackColor =
                    Color.FromArgb(242, 244, 249);
            }
            else
            {
                e.CellStyle.Font =
                    new Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Regular);

                e.CellStyle.ForeColor =
                    Color.FromArgb(73, 70, 82);

                e.CellStyle.BackColor =
                    Color.White;
            }

            // =========================================================
            // ARABIC -> RIGHT
            // ENGLISH -> LEFT
            // =========================================================
            if (isArabic)
            {
                e.CellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                // Level 1 = nearest to right edge
                // Child levels move progressively toward the left.
                int rightIndent = 18 + ((level - 1) * 28);

                e.CellStyle.Padding =
                    new Padding(
                        0,
                        0,
                        rightIndent,
                        0);
            }
            else
            {
                e.CellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;

                int leftIndent = 18 + ((level - 1) * 28);

                e.CellStyle.Padding =
                    new Padding(
                        leftIndent,
                        0,
                        0,
                        0);
            }
        }
        private void ToggleChildRowsVisibility(int parentRowIndex, int parentLevel, bool visible)
        {
            for (int i = parentRowIndex + 1; i < dgvCustomer.Rows.Count; i++)
            {
                int rowLevel = int.Parse(dgvCustomer.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel > parentLevel)
                {
                    if (visible)
                    {
                        dgvCustomer.Rows[i].Visible = true;

                        if (dgvCustomer.Rows[i].Cells["state"].Value.ToString() == "c")
                            GetAllChildren(dgvCustomer.Rows[i].Index);
                    }
                    else
                        dgvCustomer.Rows[i].Visible = false;
                }

                else
                    break;
            }
        }

        private void GetAllChildren(int parentRowIndex)
        {
            int parentLevel = int.Parse(dgvCustomer.Rows[parentRowIndex].Cells["currLvl"].Value.ToString());

            for (int i = parentRowIndex + 1; i < dgvCustomer.Rows.Count; i++)
            {
                int rowLevel = int.Parse(dgvCustomer.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel > parentLevel)
                {
                    if (rowLevel == parentLevel + 1)
                    {
                        dgvCustomer.Rows[i].Visible = true;
                        if (dgvCustomer.Rows[i].Cells["state"].Value.ToString() == "c")
                            GetAllChildren(i);
                    }
                    else
                        continue;
                }
                else
                    break;
            }

        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void LbHeader_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnImport_Export_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmChartOfAccountList(""));
        }

        private DataGridViewRow GetParentRow(int currentRowIndex)
        {
            if (currentRowIndex <= 0) return null;

            int currentLevel = int.Parse(dgvCustomer.Rows[currentRowIndex].Cells["currLvl"].Value.ToString());

            for (int i = currentRowIndex - 1; i >= 0; i--)
            {
                int rowLevel = int.Parse(dgvCustomer.Rows[i].Cells["currLvl"].Value.ToString());

                if (rowLevel == currentLevel - 1)
                    return dgvCustomer.Rows[i];
            }

            return null;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            new frmAddAccount().ShowDialog();
        }
        public void ApplyLanguageLayout(bool isArabic)
        {
            SuspendLayout();
            panel1.SuspendLayout();
            dgvCustomer.SuspendLayout();

            try
            {
                // Never mirror the complete form.
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                if (isArabic)
                {
                    // =====================================================
                    // ARABIC TEXT
                    // =====================================================
                    Text = "دليل الحسابات";

                    headerUC1.FormText = "دليل الحسابات";
                    headerUC1.ArabicLayout = true;

                    btnNew.Text = "＋  إضافة حساب";
                    btnEdit.Text = "✎  تعديل الحساب";
                    btnImport_Export.Text = "⇅  استيراد / تصدير";

                    // =====================================================
                    // ARABIC GRID
                    // =====================================================
                    dgvCustomer.RightToLeft = RightToLeft.No; dgvCustomer.RowsDefaultCellStyle.Alignment =
    DataGridViewContentAlignment.MiddleRight;

                    dgvCustomer.AlternatingRowsDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                    dgvCustomer.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    dgvCustomer.DefaultCellStyle.Padding =
                        new Padding(8, 0, 14, 0);

                    if (dgvCustomer.Columns.Contains("lvl1"))
                    {
                        dgvCustomer.Columns["lvl1"].HeaderText =
                            "التسلسل الهرمي للحسابات";

                        dgvCustomer.Columns["lvl1"]
                            .DefaultCellStyle.Alignment =
                            DataGridViewContentAlignment.MiddleRight;

                        dgvCustomer.Columns["lvl1"]
                            .HeaderCell.Style.Alignment =
                            DataGridViewContentAlignment.MiddleRight;
                    }

                    // Translate rows already loaded.
                    foreach (DataGridViewRow row in dgvCustomer.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        if (row.Cells["lvl1"].Value == null)
                            continue;

                        row.Cells["lvl1"].Value =
                            TranslateAccountName(
                                row.Cells["lvl1"].Value.ToString());
                    }

                    // =====================================================
                    // ARABIC BUTTON POSITIONS
                    //
                    // LEFT                       RIGHT
                    // Import/Export       Edit       Add
                    // =====================================================

                    btnImport_Export.Anchor =
                        AnchorStyles.Left |
                        AnchorStyles.Bottom;

                    btnImport_Export.Location =
                        new Point(20, 14);


                    btnNew.Anchor =
                        AnchorStyles.Right |
                        AnchorStyles.Bottom;

                    btnNew.Location =
                        new Point(
                            panel1.ClientSize.Width -
                            btnNew.Width - 20,
                            14);


                    btnEdit.Anchor =
                        AnchorStyles.Right |
                        AnchorStyles.Bottom;

                    btnEdit.Location =
                        new Point(
                            btnNew.Left -
                            btnEdit.Width - 10,
                            14);
                }
                else
                {
                    // =====================================================
                    // ENGLISH
                    // =====================================================
                    Text = "Chart of Accounts";

                    headerUC1.FormText = "Chart of Accounts";
                    headerUC1.ArabicLayout = false;

                    btnNew.Text = "＋  Add Account";
                    btnEdit.Text = "✎  Edit Account";
                    btnImport_Export.Text =
                        "⇅  Import / Export";

                    // =====================================================
                    // ENGLISH GRID
                    // =====================================================
                    dgvCustomer.RightToLeft =
                        RightToLeft.No;

                    dgvCustomer.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    dgvCustomer.DefaultCellStyle.Padding =
                        new Padding(14, 0, 8, 0);

                    if (dgvCustomer.Columns.Contains("lvl1"))
                    {
                        dgvCustomer.Columns["lvl1"].HeaderText =
                            "Account Hierarchy";

                        dgvCustomer.Columns["lvl1"]
                            .DefaultCellStyle.Alignment =
                            DataGridViewContentAlignment.MiddleLeft;

                        dgvCustomer.Columns["lvl1"]
                            .HeaderCell.Style.Alignment =
                            DataGridViewContentAlignment.MiddleLeft;
                    }

                    // =====================================================
                    // ENGLISH BUTTON POSITIONS
                    // =====================================================
                    btnNew.Anchor =
                        AnchorStyles.Left |
                        AnchorStyles.Bottom;

                    btnNew.Location =
                        new Point(20, 14);


                    btnEdit.Anchor =
                        AnchorStyles.Left |
                        AnchorStyles.Bottom;

                    btnEdit.Location =
                        new Point(205, 14);


                    btnImport_Export.Anchor =
                        AnchorStyles.Right |
                        AnchorStyles.Bottom;

                    btnImport_Export.Location =
                        new Point(
                            panel1.ClientSize.Width -
                            btnImport_Export.Width - 20,
                            14);
                }

                btnNew.BringToFront();
                btnEdit.BringToFront();
                btnImport_Export.BringToFront();
            }
            finally
            {
                dgvCustomer.ResumeLayout(false);
                panel1.ResumeLayout(false);
                ResumeLayout(false);

                panel1.PerformLayout();
                panel1.Invalidate(true);
                dgvCustomer.Invalidate(true);
            }
        }
        private string TranslateAccountName(string value)
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            // English -> database value exactly as stored
            if (!language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            if (string.IsNullOrWhiteSpace(value))
                return value;

            // ---------------------------------------------------------
            // Keep account code / tree icon separate from account name
            // Example:
            // "▸ 1101 - Banks"
            // codePart = "▸ 1101 - "
            // namePart = "Banks"
            // ---------------------------------------------------------
            int dashIndex = value.IndexOf(" - ");

            string prefix = "";
            string accountName = value.Trim();

            if (dashIndex >= 0)
            {
                prefix = value.Substring(0, dashIndex + 3);
                accountName = value.Substring(dashIndex + 3).Trim();
            }

            string arabicName = GetArabicAccountName(accountName);

            return prefix + arabicName;
        }
        private string GetArabicAccountName(string englishName)
        {
            if (string.IsNullOrWhiteSpace(englishName))
                return "";

            string key = englishName
                .Trim()
                .ToLowerInvariant();

            switch (key)
            {
                // =====================================================
                // ASSETS
                // =====================================================
                case "assets":
                    return "الأصول";

                case "current":
                case "current assets":
                case "current asset":
                    return "الأصول المتداولة";

                case "non-current assets":
                case "non current assets":
                    return "الأصول غير المتداولة";

                case "fixed assets":
                    return "الأصول الثابتة";

                case "long-term assets":
                case "long term assets":
                    return "الأصول طويلة الأجل";

                case "banks":
                case "bank":
                    return "البنوك";

                case "petty cash":
                    return "النقدية النثرية";

                case "cash":
                    return "النقدية";

                case "cash on hand":
                    return "النقدية في الصندوق";

                case "cash & cash equivalents":
                case "cash and cash equivalents":
                    return "النقد وما في حكمه";

                case "accounts receivable":
                    return "الذمم المدينة";

                case "other's receivable":
                case "other receivable":
                case "other receivables":
                    return "ذمم مدينة أخرى";

                case "related parties receivable":
                case "related party receivable":
                    return "ذمم الأطراف ذات العلاقة";

                case "employee receivable":
                case "employee receivables":
                    return "ذمم الموظفين";

                case "inventory":
                    return "المخزون";

                case "prepayment":
                case "prepayments":
                    return "المدفوعات المقدمة";

                case "short term investment":
                case "short-term investment":
                case "short term investments":
                    return "استثمارات قصيرة الأجل";

                case "investment":
                case "investments":
                    return "الاستثمارات";

                case "property, plant and equipment":
                case "property plant and equipment":
                    return "الممتلكات والآلات والمعدات";

                case "accumulated depreciation":
                    return "مجمع الإهلاك";

                case "intangible assets":
                    return "الأصول غير الملموسة";


                // =====================================================
                // LIABILITIES
                // =====================================================
                case "liabilities":
                case "liability":
                    return "الخصوم";

                case "current liabilities":
                    return "الخصوم المتداولة";

                case "non-current liabilities":
                case "non current liabilities":
                    return "الخصوم غير المتداولة";

                case "long-term liabilities":
                case "long term liabilities":
                    return "الخصوم طويلة الأجل";

                case "accounts payable":
                    return "الذمم الدائنة";

                case "trade payable":
                case "trade payables":
                    return "الذمم التجارية الدائنة";

                case "accrued employee benefits expense":
                    return "مصروفات منافع الموظفين المستحقة";

                case "accrued expenses":
                    return "المصروفات المستحقة";

                case "other current liabilities":
                    return "خصوم متداولة أخرى";

                case "payroll liabilities":
                    return "التزامات الرواتب";

                case "other payable":
                case "other payables":
                    return "ذمم دائنة أخرى";

                case "long-term employee benefits":
                case "long term employee benefits":
                    return "منافع الموظفين طويلة الأجل";

                case "tax payable":
                    return "الضرائب المستحقة";

                case "sales tax payable":
                    return "ضريبة المبيعات المستحقة";

                case "vat payable":
                    return "ضريبة القيمة المضافة المستحقة";

                case "loan":
                case "loans":
                    return "القروض";


                // =====================================================
                // EQUITY
                // =====================================================
                case "equity":
                    return "حقوق الملكية";

                case "capital":
                    return "رأس المال";

                case "share capital":
                    return "رأس المال";

                case "members draw":
                case "member draw":
                case "drawings":
                    return "المسحوبات";

                case "opening balance equity":
                    return "حقوق ملكية الرصيد الافتتاحي";

                case "reserve":
                    return "الاحتياطي";

                case "retained earnings":
                    return "الأرباح المحتجزة";

                case "current year earnings":
                    return "أرباح السنة الحالية";


                // =====================================================
                // INCOME / REVENUE
                // =====================================================
                case "income":
                case "revenue":
                    return "الإيرادات";

                case "sales":
                case "sales income":
                case "sales revenue":
                    return "المبيعات";

                case "sales return":
                case "sales returns":
                    return "مردودات المبيعات";

                case "maintenance income":
                    return "إيرادات الصيانة";

                case "service income":
                case "service revenue":
                    return "إيرادات الخدمات";

                case "other income":
                    return "إيرادات أخرى";

                case "discount received":
                    return "خصم مكتسب";


                // =====================================================
                // COST / COGS
                // =====================================================
                case "cost":
                    return "التكلفة";

                case "cost of sales":
                    return "تكلفة المبيعات";

                case "cost of goods sold":
                case "cogs":
                    return "تكلفة البضاعة المباعة";

                case "direct cost":
                case "direct costs":
                    return "التكاليف المباشرة";

                case "freight shipping cost":
                    return "تكلفة الشحن";

                case "customs & clearance":
                case "customs and clearance":
                    return "الجمارك والتخليص";

                case "other cost":
                case "other costs":
                    return "تكاليف أخرى";


                // =====================================================
                // EXPENSES
                // =====================================================
                case "expenses":
                case "expense":
                    return "المصروفات";

                case "general & direct expenses":
                case "general and direct expenses":
                    return "المصروفات العامة والمباشرة";

                case "general expenses":
                case "general expense":
                    return "المصروفات العامة";

                case "administrative expenses":
                case "administrative expense":
                    return "المصروفات الإدارية";

                case "salary":
                case "salaries":
                case "salary expense":
                    return "الرواتب";

                case "rent":
                case "rent expense":
                    return "مصروف الإيجار";

                case "electricity":
                    return "الكهرباء";

                case "telephone":
                    return "الهاتف";

                case "internet":
                    return "الإنترنت";

                case "transportation":
                    return "النقل";

                case "depreciation":
                case "depreciation expense":
                    return "مصروف الإهلاك";

                case "bank charges":
                    return "رسوم بنكية";

                case "office expenses":
                case "office expense":
                    return "مصروفات المكتب";

                case "maintenance":
                case "maintenance expense":
                    return "مصروفات الصيانة";

                case "insurance":
                case "insurance expense":
                    return "مصروف التأمين";


                // =====================================================
                // LOSSES / STOCK
                // =====================================================
                case "loss":
                case "losses":
                    return "الخسائر";

                case "stock write-off":
                case "stock write-offs":
                case "stock writeoff":
                case "stock writeoffs":
                    return "شطب المخزون";

                case "stock adjustment":
                case "stock adjustments":
                    return "تسويات المخزون";


                // =====================================================
                // IMPORTANT:
                // In Arabic mode never show untranslated English.
                // =====================================================
                default:
                    return RemoveEnglishCharacters(englishName);
            }
        }
        private string RemoveEnglishCharacters(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            System.Text.StringBuilder result =
                new System.Text.StringBuilder();

            foreach (char c in value)
            {
                // Remove English A-Z / a-z
                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z'))
                {
                    continue;
                }

                result.Append(c);
            }

            return result
                .ToString()
                .Trim();
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.CurrentRow == null)
                return;

            DataGridViewRow row = dgvCustomer.CurrentRow;

            if (row.Cells["currLvl"].Value == null ||
                row.Cells["id"].Value == null)
                return;

            int currLvl =
                Convert.ToInt32(
                    row.Cells["currLvl"].Value);

            int accountId =
                Convert.ToInt32(
                    row.Cells["id"].Value);

            if (currLvl == 4)
            {
                using (frmEditLevel4 frm =
                    new frmEditLevel4(accountId))
                {
                    frm.ShowDialog();
                }

                return;
            }
        }
    }
}
