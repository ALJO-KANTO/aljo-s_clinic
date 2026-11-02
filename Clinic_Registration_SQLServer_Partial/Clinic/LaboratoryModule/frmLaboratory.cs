using Clinic.ControlsMain;
using Clinic.Medical_Services.Casher;
using Clinic_DataAccess;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace Clinic.Medical_Services.Laboratory
{
    public partial class frmLaboratory : Form
    {
        public frmLaboratory()
        {
            InitializeComponent();

            ApplyArabicProperties();

            // Keep Arabic heading centered when resized.
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
                title.Dock = DockStyle.None;
                title.AutoSize = false;
                title.Left = 0;
                title.Width = header.ClientSize.Width;
                title.Height = 44;
                title.TextAlign = ContentAlignment.MiddleCenter;

                title.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right;

                subtitle.Dock = DockStyle.None;
                subtitle.AutoSize = false;
                subtitle.Left = 0;
                subtitle.Width = header.ClientSize.Width;
                subtitle.Height = 24;
                subtitle.TextAlign = ContentAlignment.MiddleCenter;

                subtitle.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right;

                title.Top = 14;
                subtitle.Top = title.Bottom;
            }
            else
            {
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
               
                ClinicDatabase.BackfillLabRequestPrices();
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
                DataTable stats =
                    ClinicDatabase.GetLabDashboardStats();

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
                        .DefaultCellStyle.Format =
                            "dd MMM yyyy HH:mm";
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

            if (!dgvRequests.Columns.Contains("LabRequestID"))
                return -1;

            object value =
                dgvRequests.CurrentRow.Cells["LabRequestID"].Value;

            if (value == null || value == DBNull.Value)
                return -1;

            return Convert.ToInt32(value);
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

        // =====================================================
        // NEW: SEND LAB REQUEST TO BILLING
        // =====================================================
        private void btnSendToBilling_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null)
            {
                MessageBox.Show("Please select a laboratory request.");
                return;
            }

            try
            {
                int requestID = Convert.ToInt32(
                    dgvRequests.CurrentRow.Cells["LabRequestID"].Value);

                int billID =
                    ClinicDatabase.GetOrCreateLabRequestBill(requestID);

                using (var frm = new
                    Clinic.Medical_Services.Casher.frmManageBills(billID))
                {
                    frm.ShowDialog(this);
                }

                ReloadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Laboratory Billing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // =====================================================
        // UPDATED: ENTER RESULT ONLY AFTER PAYMENT
        // =====================================================
        private void btnEnterResult_Click(
            object sender,
            EventArgs e)
        {
            int id = SelectedRequestID();

            if (id < 1)
            {
                MessageBox.Show(
                    T("Select a lab request first.")
                );
                return;
            }

            string status = Convert.ToString(
                dgvRequests.CurrentRow.Cells["Status"].Value
            );

            if (string.Equals(
                    status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    status,
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    T("Completed or cancelled requests cannot be overwritten.")
                );
                return;
            }

            try
            {
                bool isPaid =
                    ClinicDatabase.IsLabRequestPaid(id);

                if (!isPaid)
                {
                    MessageBox.Show(
                        T("Please complete laboratory payment before entering results."),
                        T("Payment Required"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                using (var form = new frmLabResult(id))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                        ReloadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    T("Laboratory"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}