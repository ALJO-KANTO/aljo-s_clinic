using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class frmEndOfService : Form
    {

        public frmEndOfService(int id = 0)
        {
            InitializeComponent();

           // LocalizationManager.LocalizeForm(this);

            ApplyLanguageLayout();
        }

        private void frmEndOfService_Load(object sender, EventArgs e)
        {
            bindEndOfService();
            //AppTheme.DefaultFont(dgvLeaveSalary);

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
            pnlPageHeader.SuspendLayout();

            try
            {
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                if (isArabic)
                {
                    // =====================================================
                    // ARABIC
                    // =====================================================

                    Text = "نهاية الخدمة";

                    headerUC1.FormText = "نهاية الخدمة";
                    headerUC1.RightToLeft = RightToLeft.Yes;

                    // =====================================================
                    // MODERN PAGE HEADER -> RIGHT
                    // =====================================================

                    lblTitle.Text = "نهاية الخدمة";

                    lblSubtitle.Text =
                        "عرض وإدارة تفاصيل نهاية خدمة الموظفين";

                    pnlIcon.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right;

                    lblTitle.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right;

                    lblSubtitle.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right;

                    lblTitle.RightToLeft =
                        RightToLeft.Yes;

                    lblSubtitle.RightToLeft =
                        RightToLeft.Yes;

                    lblTitle.TextAlign =
                        ContentAlignment.MiddleRight;

                    lblSubtitle.TextAlign =
                        ContentAlignment.MiddleRight;

                    // Icon -> far RIGHT
                    pnlIcon.Location =
                        new Point(
                            pnlPageHeader.ClientSize.Width
                                - 43
                                - pnlIcon.Width,
                            24);

                    // Arabic title -> left of icon
                    lblTitle.Location =
                        new Point(
                            pnlIcon.Left
                                - 20
                                - lblTitle.PreferredWidth,
                            25);

                    // Arabic subtitle -> left of icon
                    lblSubtitle.Location =
                        new Point(
                            pnlIcon.Left
                                - 20
                                - lblSubtitle.PreferredWidth,
                            72);

                    // =====================================================
                    // GRID
                    // =====================================================

                    dgvLeaveSalary.RightToLeft =
                        RightToLeft.Yes;

                    dgvLeaveSalary.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    dgvLeaveSalary.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Code",
                        "رمز الموظف");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Name",
                        "اسم الموظف");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "End Of Service Days",
                        "أيام نهاية الخدمة");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "End Of Service Amount",
                        "مبلغ نهاية الخدمة");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "EOS Remaining Days",
                        "الأيام المتبقية");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "EOS Received Amount",
                        "المبلغ المستلم");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "EOS Remaining Amount",
                        "المبلغ المتبقي");
                }
                else
                {
                    // =====================================================
                    // ENGLISH
                    // =====================================================

                    Text = "End Of Service";

                    headerUC1.FormText =
                        "End Of Service";

                    headerUC1.RightToLeft =
                        RightToLeft.No;

                    // =====================================================
                    // RESTORE ORIGINAL ENGLISH HEADER
                    // =====================================================

                    lblTitle.Text =
                        "End Of Service";

                    lblSubtitle.Text =
                        "View and manage employee end of service details.";

                    pnlIcon.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left;

                    lblTitle.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left;

                    lblSubtitle.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left;

                    lblTitle.RightToLeft =
                        RightToLeft.No;

                    lblSubtitle.RightToLeft =
                        RightToLeft.No;

                    lblTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblSubtitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    // Exact Designer positions
                    pnlIcon.Location =
                        new Point(43, 24);

                    lblTitle.Location =
                        new Point(146, 25);

                    lblSubtitle.Location =
                        new Point(149, 72);

                    // =====================================================
                    // GRID
                    // =====================================================

                    dgvLeaveSalary.RightToLeft =
                        RightToLeft.No;

                    dgvLeaveSalary.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    dgvLeaveSalary.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleLeft;

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Code",
                        "Employee Code");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "Employee Name",
                        "Employee Name");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "End Of Service Days",
                        "End Of Service Days");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "End Of Service Amount",
                        "End Of Service Amount");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "EOS Remaining Days",
                        "EOS Remaining Days");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "EOS Received Amount",
                        "EOS Received Amount");

                    SetColumnHeader(
                        dgvLeaveSalary,
                        "EOS Remaining Amount",
                        "EOS Remaining Amount");
                }

                headerUC1.BringToFront();
            }
            finally
            {
                pnlPageHeader.ResumeLayout(true);

                ResumeLayout(true);
                PerformLayout();
            }
        }

        private void SetColumnHeader(
            DataGridView dgv,
            string columnName,
            string text)
        {
            if (dgv.Columns.Contains(columnName))
                dgv.Columns[columnName].HeaderText = text;
        }
        private void bindEndOfService()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"SELECT 
                    CODE AS 'Employee Code',
                    NAME AS 'Employee Name',
                    SUM(leave_days) AS 'End Of Service Days',
                    SUM(credit) AS 'End Of Service Amount',
                    SUM(leave_days) - (SUM(debit) / (SUM(credit) / SUM(leave_days))) AS 'EOS Remaining Days',
                    SUM(debit) AS 'EOS Received Amount',
                    SUM(credit) - SUM(debit) AS 'EOS Remaining Amount'
                FROM tbl_end_of_service
                GROUP BY CODE,name;
                ");
            dgvLeaveSalary.DataSource = dt;

            dgvLeaveSalary.Columns["Employee Code"].Width = 100;
            dgvLeaveSalary.Columns["Employee Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvLeaveSalary.Columns["End Of Service Days"].Width = 120;
            dgvLeaveSalary.Columns["End Of Service Days"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["End Of Service Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvLeaveSalary.Columns["End Of Service Amount"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["EOS Remaining Days"].Width = 100;
            dgvLeaveSalary.Columns["EOS Remaining Days"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvLeaveSalary.Columns["EOS Received Amount"].Width = 150;
            dgvLeaveSalary.Columns["EOS Received Amount"].DefaultCellStyle.Format = "N2";
            dgvLeaveSalary.Columns["EOS Remaining Amount"].Width = 160;
            dgvLeaveSalary.Columns["EOS Remaining Amount"].DefaultCellStyle.Format = "N2";
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvLeaveSalary);
        }


        private void dgvLeaveSalary_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvLeaveSalary.Rows.Count != 0)
            {
              frmMain.openChildForm(new frmEndOfServiceStatement(int.Parse(dgvLeaveSalary.CurrentRow.Cells["Employee Code"].Value.ToString())));
            }
        }
    }
}
