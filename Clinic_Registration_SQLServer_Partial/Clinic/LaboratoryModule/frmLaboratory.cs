
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Clinic_DataAccess;
using Clinic.ControlsMain;

namespace Clinic.Medical_Services.Laboratory
{
    public partial class frmLaboratory : Form
    {
        public frmLaboratory()
        {
            InitializeComponent();
            ApplyArabicProperties();

            // Keep the Arabic heading centered when the header is resized.
            header.Resize += Header_Resize;
        }

        private string T(string text)
        {
            return TranslationClass.Translate(
                text,
                TranslationClass.IsArabic
            );
        }

        public void ApplyArabicProperties()
        {
            bool arabic = TranslationClass.IsArabic;

            this.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            this.RightToLeftLayout = arabic;

            ApplyTranslationToControls(this);

            if (arabic)
            {
                // Center the main heading across the entire header.
                title.Dock = DockStyle.None;
                title.AutoSize = false;
                title.Left = 0;
                title.Width = header.ClientSize.Width;
                title.Height = 44;
                title.TextAlign = ContentAlignment.MiddleCenter;
                title.Anchor = AnchorStyles.Top
                             | AnchorStyles.Left
                             | AnchorStyles.Right;

                // Center the subtitle in Arabic mode as well.
                subtitle.Dock = DockStyle.None;
                subtitle.AutoSize = false;
                subtitle.Left = 0;
                subtitle.Width = header.ClientSize.Width;
                subtitle.Height = 24;
                subtitle.TextAlign = ContentAlignment.MiddleCenter;
                subtitle.Anchor = AnchorStyles.Top
                                | AnchorStyles.Left
                                | AnchorStyles.Right;

                // Position the heading and subtitle within the header.
                title.Top = 14;
                subtitle.Top = title.Bottom;
            }
            else
            {
                // Preserve the original English layout.
                title.Dock = DockStyle.Top;
                title.AutoSize = false;
                title.Height = 44;
                title.TextAlign = ContentAlignment.MiddleLeft;

                subtitle.Dock = DockStyle.Top;
                subtitle.AutoSize = false;
                subtitle.Height = 24;
                subtitle.TextAlign = ContentAlignment.MiddleLeft;
            }

            if (dgvRequests.Columns.Count > 0)
                TranslateGridHeaders();
        }

        private void Header_Resize(object sender, EventArgs e)
        {
            if (!TranslationClass.IsArabic)
                return;

            title.Left = 0;
            title.Width = header.ClientSize.Width;
            title.TextAlign = ContentAlignment.MiddleCenter;

            subtitle.Left = 0;
            subtitle.Width = header.ClientSize.Width;
            subtitle.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void ApplyTranslationToControls(Control parent)
        {
            bool arabic = TranslationClass.IsArabic;

            foreach (Control control in parent.Controls)
            {
                if (!string.IsNullOrWhiteSpace(control.Text))
                    control.Text = T(control.Text);

                control.RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control is Label label)
                {
                    label.TextAlign = arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;

                    string name = label.Name.ToLowerInvariant();

                    if (arabic &&
                        (name.Contains("title") ||
                         name.Contains("heading")))
                    {
                        label.TextAlign = ContentAlignment.MiddleCenter;
                    }
                }

                if (control is Button button)
                {
                    button.TextAlign = ContentAlignment.MiddleCenter;
                }

                if (control.HasChildren)
                    ApplyTranslationToControls(control);
            }

            dgvRequests.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            dgvRequests.ColumnHeadersDefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleCenter
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvRequests.DefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;
        }

        private void TranslateGridHeaders()
        {
            foreach (DataGridViewColumn column in dgvRequests.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                    column.HeaderText = T(column.HeaderText);
            }
        }

        private void frmLaboratory_Load(object sender, EventArgs e)
        {
            try
            {
                ClinicDatabase.EnsureLaboratoryTables();
                ReloadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(ex.Message),
                    T("Laboratory setup"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void FiltersChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
                ReloadData();
        }

        private void ReloadData()
        {
            try
            {
                DataTable stats = ClinicDatabase.GetLabDashboardStats();

                if (stats.Rows.Count > 0)
                {
                    DataRow r = stats.Rows[0];

                    string[] fields =
                    {
                        "TotalTests",
                        "TotalRequests",
                        "Pending",
                        "InProgress",
                        "Completed"
                    };

                    for (int i = 0; i < fields.Length; i++)
                    {
                        statValues[i].Text =
                            r[fields[i]].ToString();
                    }
                }

                dgvRequests.DataSource =
                    ClinicDatabase.GetLabRequests(
                        txtSearch.Text,
                        cmbStatus.Text
                    );

                if (dgvRequests.Columns.Contains("ResultNotes"))
                    dgvRequests.Columns["ResultNotes"].Visible = false;

                if (dgvRequests.Columns.Contains("ResultValue"))
                    dgvRequests.Columns["ResultValue"].Visible = false;

                if (dgvRequests.Columns.Contains("ResultedAt"))
                    dgvRequests.Columns["ResultedAt"].Visible = false;

                if (dgvRequests.Columns.Contains("RequestedAt"))
                {
                    dgvRequests.Columns["RequestedAt"]
                        .DefaultCellStyle.Format = "dd MMM yyyy HH:mm";
                }

                TranslateGridHeaders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(ex.Message),
                    T("Load laboratory"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private int SelectedRequestID()
        {
            if (dgvRequests.CurrentRow == null ||
                dgvRequests.CurrentRow.IsNewRow)
            {
                return -1;
            }

            return Convert.ToInt32(
                dgvRequests.CurrentRow.Cells["LabRequestID"].Value
            );
        }

        private void btnNewRequest_Click(object sender, EventArgs e)
        {
            using (var form = new frmLabRequest())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    ReloadData();
            }
        }

        private void btnManageTests_Click(object sender, EventArgs e)
        {
            using (var form = new frmLabTests())
            {
                form.ShowDialog(this);
                ReloadData();
            }
        }

        private void btnEnterResult_Click(object sender, EventArgs e)
        {
            int id = SelectedRequestID();

            if (id < 1)
            {
                MessageBox.Show(T("Select a lab request first."));
                return;
            }

            string status = Convert.ToString(
                dgvRequests.CurrentRow.Cells["Status"].Value
            );

            if (status == "Completed" || status == "Cancelled")
            {
                MessageBox.Show(
                    T("Completed or cancelled requests cannot be overwritten.")
                );
                return;
            }

            using (var form = new frmLabResult(id))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    ReloadData();
            }
        }
    }
}
