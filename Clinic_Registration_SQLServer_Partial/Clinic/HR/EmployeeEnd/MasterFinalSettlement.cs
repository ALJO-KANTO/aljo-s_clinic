using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterFinalSettlement : Form
    {

        public MasterFinalSettlement()
        {
            InitializeComponent();

           // LocalizationManager.LocalizeForm(this);

            // We already have the modern page heading.
            // Do not show the old purple HeaderUC.
            headerUC1.Visible = false;
            headerUC1.Enabled = false;
            ApplyLanguageLayout();
        }


        private void btnNew_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmFinalSettlement());
        }
        private void MasterFinalSettlement_Load(object sender, EventArgs e)
        {
            //AppTheme.DefaultFont(dgvCustomer);

            BindFinalSet();

            // IMPORTANT:
            // Apply our final modern design AFTER BindFinalSet(),
            // because BindFinalSet currently changes grid fonts/widths/colors.
            ApplyLanguageLayout();
        }
        public void ApplyLanguageLayout()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            bool isArabic =
                language.StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            SuspendLayout();

            try
            {
                // Never mirror the whole Form.
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                // Old HeaderUC remains hidden.
                headerUC1.Visible = false;
                headerUC1.Enabled = false;

                // =====================================================
                // FIND MODERN LABELS
                // =====================================================

                Label pageTitle = FindLabelByTexts(
                    this,
                    "Final Settlement",
                    "التسوية النهائية");

                Label pageSubtitle = FindLabelByTexts(
                    this,
                    "Review and manage employee final settlement records.",
                    "عرض وإدارة سجلات التسوية النهائية للموظفين.");

                Label recordsTitle = FindLabelByTexts(
                    this,
                    "Settlement Records",
                    "سجلات التسوية");

                Label recordsSubtitle = FindLabelByTexts(
                    this,
                    "Double-click a record to open settlement.",
                    "انقر نقراً مزدوجاً على السجل لفتح التسوية.");

                Label stateLabel = FindLabelByTexts(
                    this,
                    "State",
                    "دولة",
                    "الحالة");

                // =====================================================
                // TEXT
                // =====================================================

                if (isArabic)
                {
                    Text = "التسوية النهائية";

                    btnNew.Text = "إضافة جديد";
                    btnDelete.Text = "حذف";

                    if (pageTitle != null)
                        pageTitle.Text = "التسوية النهائية";

                    if (pageSubtitle != null)
                        pageSubtitle.Text =
                            "عرض وإدارة سجلات التسوية النهائية للموظفين.";

                    if (recordsTitle != null)
                        recordsTitle.Text = "سجلات التسوية";

                    if (recordsSubtitle != null)
                        recordsSubtitle.Text =
                            "انقر نقراً مزدوجاً على السجل لفتح التسوية.";

                    if (stateLabel != null)
                        stateLabel.Text = "الحالة";
                }
                else
                {
                    Text = "Final Settlement";

                    btnNew.Text = "Add New";
                    btnDelete.Text = "Delete";

                    if (pageTitle != null)
                        pageTitle.Text = "Final Settlement";

                    if (pageSubtitle != null)
                        pageSubtitle.Text =
                            "Review and manage employee final settlement records.";

                    if (recordsTitle != null)
                        recordsTitle.Text = "Settlement Records";

                    if (recordsSubtitle != null)
                        recordsSubtitle.Text =
                            "Double-click a record to open settlement.";

                    if (stateLabel != null)
                        stateLabel.Text = "State";
                }

                // =====================================================
                // MAIN PAGE TITLE
                // English -> LEFT
                // Arabic  -> RIGHT
                // =====================================================

                if (pageTitle != null && pageTitle.Parent != null)
                {
                    Control parent = pageTitle.Parent;

                    if (isArabic)
                    {
                        pageTitle.AutoSize = true;

                        pageTitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Right;

                        pageTitle.RightToLeft = RightToLeft.Yes;
                        pageTitle.TextAlign = ContentAlignment.MiddleRight;

                        pageTitle.Left =
                            parent.ClientSize.Width
                            - pageTitle.PreferredWidth
                            - 45;
                    }
                    else
                    {
                        pageTitle.AutoSize = true;

                        pageTitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Left;

                        pageTitle.RightToLeft = RightToLeft.No;
                        pageTitle.TextAlign = ContentAlignment.MiddleLeft;

                        // Keep your original English position.
                        pageTitle.Left = 145;
                    }
                }

                // =====================================================
                // MAIN SUBTITLE
                // =====================================================

                if (pageSubtitle != null && pageSubtitle.Parent != null)
                {
                    Control parent = pageSubtitle.Parent;

                    if (isArabic)
                    {
                        pageSubtitle.AutoSize = true;

                        pageSubtitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Right;

                        pageSubtitle.RightToLeft = RightToLeft.Yes;
                        pageSubtitle.TextAlign =
                            ContentAlignment.MiddleRight;

                        pageSubtitle.Left =
                            parent.ClientSize.Width
                            - pageSubtitle.PreferredWidth
                            - 45;
                    }
                    else
                    {
                        pageSubtitle.AutoSize = true;

                        pageSubtitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Left;

                        pageSubtitle.RightToLeft = RightToLeft.No;
                        pageSubtitle.TextAlign =
                            ContentAlignment.MiddleLeft;

                        pageSubtitle.Left = 145;
                    }
                }

                // =====================================================
                // FILTER
                //
                // ENGLISH:
                // State                              [ All ]
                //
                // ARABIC:
                // [ الكل ]                         الحالة
                // =====================================================

                Control filterParent = cmbState.Parent;

                if (filterParent != null)
                {
                    const int margin = 30;

                    if (isArabic)
                    {
                        // Combo -> LEFT
                        cmbState.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Left;

                        cmbState.Left = margin;
                        cmbState.RightToLeft = RightToLeft.Yes;

                        if (stateLabel != null)
                        {
                            stateLabel.AutoSize = true;

                            stateLabel.Anchor =
                                AnchorStyles.Top |
                                AnchorStyles.Right;

                            stateLabel.RightToLeft =
                                RightToLeft.Yes;

                            stateLabel.TextAlign =
                                ContentAlignment.MiddleRight;

                            stateLabel.Left =
                                filterParent.ClientSize.Width
                                - stateLabel.PreferredWidth
                                - margin;
                        }
                    }
                    else
                    {
                        // Combo -> RIGHT
                        cmbState.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Right;

                        cmbState.Left =
                            filterParent.ClientSize.Width
                            - cmbState.Width
                            - margin;

                        cmbState.RightToLeft = RightToLeft.No;

                        if (stateLabel != null)
                        {
                            stateLabel.AutoSize = true;

                            stateLabel.Anchor =
                                AnchorStyles.Top |
                                AnchorStyles.Left;

                            stateLabel.RightToLeft =
                                RightToLeft.No;

                            stateLabel.TextAlign =
                                ContentAlignment.MiddleLeft;

                            stateLabel.Left = margin;
                        }
                    }
                }

                // =====================================================
                // SETTLEMENT RECORDS HEADING
                // =====================================================

                if (recordsTitle != null &&
                    recordsTitle.Parent != null)
                {
                    Control parent = recordsTitle.Parent;

                    if (isArabic)
                    {
                        recordsTitle.AutoSize = true;

                        recordsTitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Right;

                        recordsTitle.RightToLeft =
                            RightToLeft.Yes;

                        recordsTitle.TextAlign =
                            ContentAlignment.MiddleRight;

                        recordsTitle.Left =
                            parent.ClientSize.Width
                            - recordsTitle.PreferredWidth
                            - 60;
                    }
                    else
                    {
                        recordsTitle.AutoSize = true;

                        recordsTitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Left;

                        recordsTitle.RightToLeft =
                            RightToLeft.No;

                        recordsTitle.TextAlign =
                            ContentAlignment.MiddleLeft;

                        recordsTitle.Left = 75;
                    }
                }

                if (recordsSubtitle != null &&
                    recordsSubtitle.Parent != null)
                {
                    Control parent = recordsSubtitle.Parent;

                    if (isArabic)
                    {
                        recordsSubtitle.AutoSize = true;

                        recordsSubtitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Right;

                        recordsSubtitle.RightToLeft =
                            RightToLeft.Yes;

                        recordsSubtitle.TextAlign =
                            ContentAlignment.MiddleRight;

                        recordsSubtitle.Left =
                            parent.ClientSize.Width
                            - recordsSubtitle.PreferredWidth
                            - 60;
                    }
                    else
                    {
                        recordsSubtitle.AutoSize = true;

                        recordsSubtitle.Anchor =
                            AnchorStyles.Top |
                            AnchorStyles.Left;

                        recordsSubtitle.RightToLeft =
                            RightToLeft.No;

                        recordsSubtitle.TextAlign =
                            ContentAlignment.MiddleLeft;

                        recordsSubtitle.Left = 75;
                    }
                }

                // =====================================================
                // FOOTER BUTTONS
                //
                // ENGLISH -> RIGHT
                // ARABIC  -> LEFT
                // =====================================================

                Control buttonParent = btnNew.Parent;

                if (buttonParent != null)
                {
                    const int margin = 30;
                    const int gap = 12;

                    if (isArabic)
                    {
                        btnNew.Anchor =
                            AnchorStyles.Left |
                            AnchorStyles.Top;

                        btnDelete.Anchor =
                            AnchorStyles.Left |
                            AnchorStyles.Top;

                        btnNew.Left = margin;

                        btnDelete.Left =
                            btnNew.Right + gap;
                    }
                    else
                    {
                        btnNew.Anchor =
                            AnchorStyles.Right |
                            AnchorStyles.Top;

                        btnDelete.Anchor =
                            AnchorStyles.Right |
                            AnchorStyles.Top;

                        btnNew.Left =
                            buttonParent.ClientSize.Width
                            - btnNew.Width
                            - margin;

                        btnDelete.Left =
                            btnNew.Left
                            - gap
                            - btnDelete.Width;
                    }

                    btnDelete.Top = btnNew.Top;
                }

                // =====================================================
                // GRID DIRECTION
                // =====================================================

                dgvCustomer.RightToLeft =
                    isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                dgvCustomer.EnableHeadersVisualStyles = false;
                dgvCustomer.BackgroundColor = Color.White;
                dgvCustomer.BorderStyle = BorderStyle.None;
                dgvCustomer.RowHeadersVisible = false;
                dgvCustomer.AllowUserToAddRows = false;
                dgvCustomer.AllowUserToResizeRows = false;

                dgvCustomer.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dgvCustomer.MultiSelect = false;

                dgvCustomer.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                dgvCustomer.RowTemplate.Height = 38;

                dgvCustomer.ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                dgvCustomer.ColumnHeadersHeight = 48;

                // =====================================================
                // GRID FONT / COLORS
                // =====================================================

                dgvCustomer.DefaultCellStyle.Font =
                    new Font(
                        "Segoe UI",
                        9.5F,
                        FontStyle.Regular);

                dgvCustomer.ColumnHeadersDefaultCellStyle.Font =
                    new Font(
                        "Segoe UI Semibold",
                        9.5F,
                        FontStyle.Bold);

                dgvCustomer.ColumnHeadersDefaultCellStyle.BackColor =
                    Color.FromArgb(126, 146, 165);

                dgvCustomer.ColumnHeadersDefaultCellStyle.ForeColor =
                    Color.White;

                dgvCustomer.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(126, 146, 165);

                dgvCustomer.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                    Color.White;

                dgvCustomer.DefaultCellStyle.BackColor =
                    Color.White;

                dgvCustomer.DefaultCellStyle.ForeColor =
                    Color.FromArgb(35, 45, 65);

                dgvCustomer.DefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(235, 228, 252);

                dgvCustomer.DefaultCellStyle.SelectionForeColor =
                    Color.FromArgb(35, 45, 65);

                dgvCustomer.AlternatingRowsDefaultCellStyle.BackColor =
                    Color.FromArgb(249, 250, 252);

                // =====================================================
                // GRID HEADERS
                // =====================================================

                SetColumnHeader(
                    dgvCustomer,
                    "Emp Code",
                    isArabic ? "رمز الموظف" : "Employee Code");

                SetColumnHeader(
                    dgvCustomer,
                    "Employee Name",
                    isArabic ? "اسم الموظف" : "Employee Name");

                SetColumnHeader(
                    dgvCustomer,
                    "DateCommencement",
                    isArabic ? "تاريخ بدء العمل" : "Start Date");

                SetColumnHeader(
                    dgvCustomer,
                    "DateLastWork",
                    isArabic ? "تاريخ آخر يوم عمل" : "Last Working Date");

                SetColumnHeader(
                    dgvCustomer,
                    "TotalSalary",
                    isArabic ? "إجمالي الراتب" : "Total Salary");

                SetColumnHeader(
                    dgvCustomer,
                    "TotalAdditions",
                    isArabic ? "إجمالي الإضافات" : "Total Additions");

                SetColumnHeader(
                    dgvCustomer,
                    "TotalDeductions",
                    isArabic ? "إجمالي الخصومات" : "Total Deductions");

                SetColumnHeader(
                    dgvCustomer,
                    "NetAccruals",
                    isArabic ? "صافي المستحقات" : "Net Accruals");

                // =====================================================
                // IMPORTANT: GRID COLUMN ORDER
                //
                // DisplayIndex is LTR logical order.
                // RightToLeft.Yes visually mirrors it in Arabic.
                // =====================================================

                SetDisplayIndex("Emp Code", 0);
                SetDisplayIndex("Employee Name", 1);
                SetDisplayIndex("DateCommencement", 2);
                SetDisplayIndex("DateLastWork", 3);
                SetDisplayIndex("TotalSalary", 4);
                SetDisplayIndex("TotalAdditions", 5);
                SetDisplayIndex("TotalDeductions", 6);
                SetDisplayIndex("NetAccruals", 7);

                // =====================================================
                // COLUMN SIZES
                // =====================================================

                SetGridFillColumn("Emp Code", 12, 100);
                SetGridFillColumn("Employee Name", 25, 180);
                SetGridFillColumn("DateCommencement", 16, 125);
                SetGridFillColumn("DateLastWork", 16, 125);
                SetGridFillColumn("TotalSalary", 14, 110);
                SetGridFillColumn("TotalAdditions", 15, 115);
                SetGridFillColumn("TotalDeductions", 15, 120);
                SetGridFillColumn("NetAccruals", 15, 120);

                // =====================================================
                // ALIGNMENT
                // =====================================================

                dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                    isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;

                dgvCustomer.DefaultCellStyle.Alignment =
                    isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;

                if (dgvCustomer.Columns.Contains("Emp Code"))
                {
                    dgvCustomer.Columns["Emp Code"]
                        .DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;

                    dgvCustomer.Columns["Emp Code"]
                        .HeaderCell.Style.Alignment =
                        isArabic
                            ? DataGridViewContentAlignment.MiddleRight
                            : DataGridViewContentAlignment.MiddleLeft;
                }

                if (dgvCustomer.Columns.Contains("Employee Name"))
                {
                    dgvCustomer.Columns["Employee Name"]
                        .DefaultCellStyle.Alignment =
                        isArabic
                            ? DataGridViewContentAlignment.MiddleRight
                            : DataGridViewContentAlignment.MiddleLeft;

                    dgvCustomer.Columns["Employee Name"]
                        .HeaderCell.Style.Alignment =
                        isArabic
                            ? DataGridViewContentAlignment.MiddleRight
                            : DataGridViewContentAlignment.MiddleLeft;
                }

                // =====================================================
                // MONEY
                // =====================================================

                string[] moneyColumns =
                {
            "TotalSalary",
            "TotalAdditions",
            "TotalDeductions",
            "NetAccruals"
        };

                foreach (string columnName in moneyColumns)
                {
                    if (!dgvCustomer.Columns.Contains(columnName))
                        continue;

                    dgvCustomer.Columns[columnName]
                        .DefaultCellStyle.Alignment =
                        isArabic
                            ? DataGridViewContentAlignment.MiddleLeft
                            : DataGridViewContentAlignment.MiddleRight;

                    dgvCustomer.Columns[columnName]
                        .DefaultCellStyle.Format = "N2";
                }

                // =====================================================
                // Z ORDER
                // =====================================================

                if (pageTitle != null)
                    pageTitle.BringToFront();

                if (pageSubtitle != null)
                    pageSubtitle.BringToFront();

                if (recordsTitle != null)
                    recordsTitle.BringToFront();

                if (recordsSubtitle != null)
                    recordsSubtitle.BringToFront();

                cmbState.BringToFront();
                btnNew.BringToFront();
                btnDelete.BringToFront();
            }
            finally
            {
                ResumeLayout(true);
                PerformLayout();
            }
        }
        private void SetDisplayIndex(
    string columnName,
    int displayIndex)
        {
            if (!dgvCustomer.Columns.Contains(columnName))
                return;

            dgvCustomer.Columns[columnName].DisplayIndex =
                displayIndex;
        }
        private void SetGridFillColumn(
    string columnName,
    float fillWeight,
    int minimumWidth)
        {
            if (!dgvCustomer.Columns.Contains(columnName))
                return;

            DataGridViewColumn column =
                dgvCustomer.Columns[columnName];

            column.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;

            column.FillWeight = fillWeight;
            column.MinimumWidth = minimumWidth;
        }


        private Label FindLabelByTexts(
            Control parent,
            params string[] possibleTexts)
        {
            if (parent == null)
                return null;

            foreach (Control control in parent.Controls)
            {
                if (control is Label label)
                {
                    foreach (string text in possibleTexts)
                    {
                        if (string.Equals(
                            label.Text?.Trim(),
                            text?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return label;
                        }
                    }
                }

                if (control.HasChildren)
                {
                    Label result =
                        FindLabelByTexts(
                            control,
                            possibleTexts);

                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private void SetColumnHeader(
            DataGridView dgv,
            string columnName,
            string text)
        {
            if (dgv.Columns.Contains(columnName))
                dgv.Columns[columnName].HeaderText = text;
        }
        public void BindFinalSet()
        {
            DataTable dt;
            string query = @"
                            SELECT 
                                e.id,
                                e.code AS 'Emp Code', 
                                e.name AS 'Employee Name',
                                tf.DateCommencement AS 'DateCommencement', 
                                tf.DateLastWork AS 'DateLastWork',
                                tf.TotalSalary AS 'TotalSalary', 
                                tf.TotalAdditions AS 'TotalAdditions', 
                                tf.TotalDeductions AS 'TotalDeductions',
                                tf.NetAccruals AS 'NetAccruals'
                            FROM tbl_final_settlement tf
                            INNER JOIN tbl_employee e ON tf.emp_id = e.id";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            if (cmbState.Text == "Active")
                query += " AND active = 0";
            else if (cmbState.Text != "All")
                query += " AND active != 0";

            dt = ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
            dgvCustomer.DataSource = dt;

            if (dgvCustomer.Columns.Contains("id"))
            {
                dgvCustomer.Columns["id"].Visible = false;
            }

            //// Align numeric values for better readability
            //dgvCustomer.Columns["TotalSalary"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dgvCustomer.Columns["TotalAdditions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dgvCustomer.Columns["TotalDeductions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dgvCustomer.Columns["NetAccruals"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

          

            if (dgvCustomer.Rows.Count > 0)
            {
                btnDelete.Visible = UserPermissions.canDelete("Final Settlement");
            }
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
            ApplyLanguageLayout();
        }


        //private void btnEdit_Click(object sender, EventArgs e)
        //{
        //    if (dgvCustomer.Rows.Count == 0)
        //        return;
        //    _mainForm.openChildForm(new frmViewEmployee(_mainForm, this, int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        //}
        private void dgvCustomer_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void dgvCustomer_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;

        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            ClinicDatabase.ExecuteNonQuery("UPDATE tbl_employee SET state = -1 WHERE id = @id ",
                                          ClinicDatabase.CreateParameterNew("id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));
            ////Utilities.LogAudit(-1, "Delete Final Settlement", "Final Settlement", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()), "Deleted Final Settlement for Employee: " + dgvCustomer.SelectedRows[0].Cells["Employee Name"].Value.ToString());
            BindFinalSet();
        }

        private void cmbState_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFinalSet();
        }

        private void dgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmFinalSettlement(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }

        //private void btnOther_Click(object sender, EventArgs e)
        //{
        //    if (pnlWindow.Visible == true)
        //        pnlWindow.Visible = false;
        //    else pnlWindow.Visible = true;
        //}

        //private void btnAttendenceSheet_Click(object sender, EventArgs e)
        //{
        //    pnlWindow.Visible = false;
        //    _mainForm.openChildForm(new MasterAttendanceSheet(_mainForm));
        //}

        //private void btnSalarySheet_Click(object sender, EventArgs e)
        //{
        //    pnlWindow.Visible = false;
        //    _mainForm.openChildForm(new frmSalarySheet(_mainForm));
        //}

        //private void btnLoan_Click(object sender, EventArgs e)
        //{
        //    pnlWindow.Visible = false;
        //    pnlWindow.Visible = false;
        //    _mainForm.openChildForm(new frmViewLoan(_mainForm));
        //}

        //private void btnEndOfService_Click(object sender, EventArgs e)
        //{
        //    pnlWindow.Visible = false;
        //    _mainForm.openChildForm(new frmEndOfServices(_mainForm));
        //}

        //private void btnLeaveSalary_Click(object sender, EventArgs e)
        //{
        //    pnlWindow.Visible = false;
        //    _mainForm.openChildForm(new frmLeaveSalary(_mainForm));
        //}

        //private void btnFinalSettlement_Click(object sender, EventArgs e)
        //{
        //    pnlWindow.Visible = false;
        //    _mainForm.openChildForm(new frmFinalSettlement(_mainForm));
        //}
    }
}
