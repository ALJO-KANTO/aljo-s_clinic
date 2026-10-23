
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Serveces.ServecesType
{
    public partial class frmListServicesType : Form
    {
        private DataTable _dtListServices;

        // Store the original English layout.
        private readonly Dictionary<Control, Rectangle> _originalBounds =
            new Dictionary<Control, Rectangle>();

        private readonly Dictionary<Control, string> _originalTexts =
            new Dictionary<Control, string>();

        private bool _layoutCaptured = false;

        public frmListServicesType()
        {
            InitializeComponent();

            // Capture the original Designer layout.
            CaptureOriginalLayout(this);

            _layoutCaptured = true;

            // Apply current application language.
            ApplyLanguage();
        }

        // ============================================
        // LANGUAGE
        // ============================================

        private bool IsArabic()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.StartsWith(
                "ar",
                StringComparison.OrdinalIgnoreCase);
        }

        private string Translate(string english)
        {
            return TranslationClass.Translate(
                english,
                IsArabic());
        }

        // ============================================
        // SAVE ORIGINAL DESIGNER LAYOUT
        // ============================================

        private void CaptureOriginalLayout(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                _originalBounds[control] = control.Bounds;
                _originalTexts[control] = control.Text;

                if (control.HasChildren)
                    CaptureOriginalLayout(control);
            }
        }

        // ============================================
        // APPLY TRANSLATION + RTL
        // ============================================

        private void ApplyLanguage()
        {
            if (!_layoutCaptured)
                return;

            bool arabic = IsArabic();

            this.SuspendLayout();

            try
            {
                this.RightToLeft = arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                this.RightToLeftLayout = arabic;

                TranslateControls(this, arabic);
                ApplyControlLayout(this, arabic);
                ApplyGridLanguage(arabic);
                TranslateContextMenu(arabic);
                FixServicesHeader(this, arabic);
            }
            finally
            {
                this.ResumeLayout(true);
            }
        }

        // ============================================
        // TRANSLATE LABELS, BUTTONS, GROUPBOXES
        // ============================================

        private void TranslateControls(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (_originalTexts.TryGetValue(control, out string english))
                {
                    string translated = GetTranslatedText(
                        english, arabic);

                    if (!(control is DataGridView))
                        control.Text = translated;
                }

                if (control is Label label)
                {
                    label.RightToLeft = arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                    // Record count should remain readable.
                    if (control == lblRecordsCount)
                    {
                        label.TextAlign =
                            ContentAlignment.MiddleCenter;
                    }
                    else
                    {
                        label.TextAlign = arabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                }
                else if (control is Button button)
                {
                    button.RightToLeft = arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;
                }

                if (control.HasChildren)
                    TranslateControls(control, arabic);
            }
        }

        private string GetTranslatedText(string english, bool arabic)
        {
            if (!arabic || string.IsNullOrWhiteSpace(english))
                return english;

            switch (english.Trim())
            {
                case "Service Types":
                case "Services Types":
                case "Service Type List":
                case "Services List":
                    return "قائمة الخدمات";

                case "Service Type":
                    return "نوع الخدمة";

                case "Services":
                    return "الخدمات";

                case "Add":
                case "Add Service":
                    return "إضافة خدمة";

                case "Edit":
                    return "تعديل";

                case "Delete":
                    return "حذف";

                case "Close":
                case "Exit":
                    return "إغلاق";

                case "Records":
                case "Records Count":
                    return "عدد السجلات";

                case "Search":
                    return "بحث";

                default:
                    return TranslationClass.Translate(english, true);
            }
        }

        // ============================================
        // POSITION CONTROLS
        // ============================================

        private void ApplyControlLayout(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (!_originalBounds.TryGetValue(
                    control, out Rectangle original))
                    continue;
                if (_originalTexts.TryGetValue(control, out string originalText) &&
    (originalText.Trim() == "Mange Services Type" ||
     originalText.Trim() == "Manage Services Type" ||
     originalText.Trim() == "Manage Service Types"))
                {
                    control.Bounds = original;
                    continue;
                }
                // Do not manually mirror docked controls.
                if (control.Dock == DockStyle.None)
                {
                    int left = arabic
                        ? parent.ClientSize.Width
                          - original.Right
                        : original.Left;

                    control.Left = Math.Max(0, left);
                    control.Top = original.Top;
                }

                if (control is FlowLayoutPanel flow)
                {
                    flow.FlowDirection = arabic
                        ? FlowDirection.RightToLeft
                        : FlowDirection.LeftToRight;
                }

                if (control.HasChildren)
                    ApplyControlLayout(control, arabic);
            }
        }

        // ============================================
        // DATAGRIDVIEW TRANSLATION
        // ============================================

        private void ApplyGridLanguage(bool arabic)
        {
            dgvServicesType.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            dgvServicesType.ColumnHeadersDefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvServicesType.DefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvServicesType.RowHeadersVisible = false;

            dgvServicesType.AllowUserToAddRows = false;

            dgvServicesType.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvServicesType.MultiSelect = false;

            dgvServicesType.EnableHeadersVisualStyles = false;

            SetColumn(
                "ServiceID",
                arabic ? "رقم الخدمة" : "Service ID",
                120);

            SetColumn(
                "ServiceName",
                arabic ? "اسم الخدمة" : "Service Name",
                220);

            SetColumn(
                "Description",
                arabic ? "الوصف" : "Description",
                350);

            SetColumn(
                "ServiceFees",
                arabic ? "رسوم الخدمة" : "Service Fees",
                120);
        }

        private void SetColumn(
            string columnName,
            string header,
            int width)
        {
            if (!dgvServicesType.Columns.Contains(columnName))
                return;

            DataGridViewColumn column =
                dgvServicesType.Columns[columnName];

            column.HeaderText = header;
            column.Width = width;

            if (columnName == "ServiceFees")
            {
                column.DefaultCellStyle.Format = "N2";

                column.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
        }

        // ============================================
        // CONTEXT MENU TRANSLATION
        // ============================================

        private void TranslateContextMenu(bool arabic)
        {
            editToolStripMenuItem.Text =
                arabic ? "تعديل" : "Edit";

            addServicesTypeToolStripMenuItem.Text =
                arabic ? "إضافة خدمة" : "Add Service";

            deleteServicesTypeToolStripMenuItem.Text =
                arabic ? "حذف الخدمة" : "Delete Service";
        }

        // ============================================
        // LOAD AND REFRESH DATA
        // ============================================

        private void _RefreshData()
        {
            _dtListServices = clsServicesType.GetServiceList();

            dgvServicesType.DataSource = _dtListServices;

            lblRecordsCount.Text =
                _dtListServices.Rows.Count.ToString();

            ApplyGridLanguage(IsArabic());
        }

        private void frmListServicesType_Load(
            object sender, EventArgs e)
        {
            _RefreshData();
            ApplyLanguage();
        }

        // ============================================
        // CLOSE
        // ============================================

        private void btnClose_Click(
            object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit_Click(
            object sender, EventArgs e)
        {
            this.Close();
        }

        // ============================================
        // EDIT SERVICE
        // ============================================

        private void editToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            if (!TryGetSelectedServiceID(out int serviceID))
                return;

            using (frmAddUpdateServices frm =
                new frmAddUpdateServices(serviceID))
            {
                frm.ShowDialog(this);
            }

            _RefreshData();
        }

        // ============================================
        // ADD SERVICE
        // ============================================

        private void addServicesTypeToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            using (frmAddUpdateServices frm =
                new frmAddUpdateServices())
            {
                frm.ShowDialog(this);
            }

            _RefreshData();
        }

        // ============================================
        // DELETE SERVICE
        // ============================================

        private void deleteServicesTypeToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            if (!TryGetSelectedServiceID(out int serviceID))
                return;

            string message = IsArabic()
                ? "هل أنت متأكد أنك تريد حذف هذه الخدمة؟"
                : "Are you sure you want to delete this service type?";

            string title = IsArabic()
                ? "حذف الخدمة"
                : "Delete Service Type";

            if (MessageBox.Show(
                message,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            if (clsServicesType.Delete(serviceID))
            {
                MessageBox.Show(
                    IsArabic()
                        ? "تم حذف الخدمة بنجاح."
                        : "Service type deleted successfully.",
                    title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                _RefreshData();
            }
            else
            {
                MessageBox.Show(
                    IsArabic()
                        ? "فشل حذف الخدمة."
                        : "Failed to delete service type.",
                    title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // SELECTED SERVICE ID
        // ============================================
        private void FixServicesHeader(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label label)
                {
                    string text = label.Text.Trim();

                    if (text == "Mange Services Type" ||
                        text == "Manage Services Type" ||
                        text == "Manage Service Types" ||
                        text == "إدارة أنواع الخدمات")
                    {
                        label.Text = arabic
                            ? "إدارة أنواع الخدمات"
                            : "Manage Services Type";

                        label.AutoSize = false;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.RightToLeft = arabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                        label.Font = new Font(
                            "Segoe UI",
                            18F,
                            FontStyle.Bold);

                        label.BringToFront();
                    }
                }

                if (control.HasChildren)
                    FixServicesHeader(control, arabic);
            }
        }
        private bool TryGetSelectedServiceID(out int serviceID)
        {
            serviceID = -1;

            if (dgvServicesType.CurrentRow == null ||
                dgvServicesType.CurrentRow.IsNewRow ||
                !dgvServicesType.Columns.Contains("ServiceID"))
            {
                MessageBox.Show(
                    IsArabic()
                        ? "يرجى تحديد خدمة أولاً."
                        : "Please select a service first.");

                return false;
            }

            object value =
                dgvServicesType.CurrentRow.Cells["ServiceID"].Value;

            if (value == null ||
                value == DBNull.Value ||
                !int.TryParse(value.ToString(), out serviceID))
            {
                return false;
            }

            return true;
        }
    }
}
