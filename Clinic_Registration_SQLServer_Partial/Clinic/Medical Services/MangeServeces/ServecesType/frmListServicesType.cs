
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
        private Panel _serviceActions;
        private TextBox _serviceSearch;
        private Label _serviceHelp;
        private bool _actionsInstalled;


        public frmListServicesType()
        {
            InitializeComponent();

            // Capture the original Designer layout.
            CaptureOriginalLayout(this);

            _layoutCaptured = true;
            InstallServiceActions();

            // Apply current application language.
            ApplyLanguage();
            Resize += (s, e) => FixServicesGridLayout();
        }

        // Visible controls are created at runtime; the existing Designer is unchanged.
        private void InstallServiceActions()
        {
            if (_actionsInstalled || dgvServicesType.Parent == null) return;
            _actionsInstalled = true;

            Control host = dgvServicesType.Parent;
            _serviceActions = new Panel
            {
                Name = "pnlServiceActionsRuntime",
                BackColor = Color.FromArgb(245, 248, 252),
                Height = 84,
                Left = dgvServicesType.Left,
                Top = dgvServicesType.Top,
                Width = dgvServicesType.Width,
                Anchor = dgvServicesType.Anchor
            };

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 43,
                Padding = new Padding(5, 5, 0, 0),
                WrapContents = false,
                AutoScroll = true
            };
            Button add = ActionButton("+ Add Service", (s, e) => OpenAddService());
            Button edit = ActionButton("Edit", (s, e) => OpenEditService());
            Button delete = ActionButton("Delete", (s, e) => DeleteSelectedService());
            Button refresh = ActionButton("Refresh", (s, e) => _RefreshData());
            buttons.Controls.Add(add);
            buttons.Controls.Add(edit);
            buttons.Controls.Add(delete);
            buttons.Controls.Add(refresh);
            _serviceSearch = new TextBox { Width = 205, Margin = new Padding(12, 3, 3, 3) };
            _serviceSearch.TextChanged += (s, e) => FilterServices();
            buttons.Controls.Add(_serviceSearch);

            _serviceHelp = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 37,
                AutoEllipsis = true,
                Padding = new Padding(10, 5, 8, 0),
                ForeColor = Color.FromArgb(62, 80, 101),
                Font = new Font("Segoe UI", 9F)
            };
            _serviceActions.Controls.Add(buttons);
            _serviceActions.Controls.Add(_serviceHelp);
            host.Controls.Add(_serviceActions);
            _serviceActions.BringToFront();

            // Reserve space for the new toolbar while preserving the grid's bottom edge.
            FixServicesGridLayout();
            dgvServicesType.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) OpenEditService();
            };
            UpdateServiceActionsLanguage();
        }

        private static Button ActionButton(string caption, EventHandler action)
        {
            var button = new Button
            {
                Text = caption,
                AutoSize = false,
                Width = caption.Contains("Service") ? 125 : 85,
                Height = 29,
                Margin = new Padding(3, 2, 3, 2),
                BackColor = Color.FromArgb(22, 91, 112),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += action;
            return button;
        }

        private void UpdateServiceActionsLanguage()
        {
            if (_serviceActions == null) return;
            bool ar = IsArabic();
            var buttons = (FlowLayoutPanel)_serviceActions.Controls[0];
            buttons.Controls[0].Text = ar ? "+ إضافة خدمة" : "+ Add Service";
            buttons.Controls[1].Text = ar ? "تعديل" : "Edit";
            buttons.Controls[2].Text = ar ? "حذف" : "Delete";
            buttons.Controls[3].Text = ar ? "تحديث" : "Refresh";
            _serviceHelp.Text = ar
                ? "أضف أو عدّل الخدمات ورسومها هنا. لإضافتها إلى فاتورة، اختر الخدمة في زيارة المريض أو شاشة الفوترة."
                : "Manage service names and fees here. To bill a service, select it during a patient visit or in billing.";
            if (_serviceSearch != null && !_serviceSearch.Focused)
                _serviceSearch.AccessibleName = ar ? "بحث عن خدمة" : "Search services";
        }

        private void FilterServices()
        {
            if (_dtListServices == null || _serviceSearch == null) return;
            string query = _serviceSearch.Text.Trim();
            var view = new DataView(_dtListServices);
            if (query.Length > 0)
            {
                string escaped = query.Replace("'", "''").Replace("[", "[[]")
                    .Replace("%", "[%]").Replace("*", "[*]");
                var filters = new List<string>();
                foreach (DataColumn column in _dtListServices.Columns)
                {
                    if (column.ColumnName.Equals("ServiceName", StringComparison.OrdinalIgnoreCase) ||
                        column.ColumnName.Equals("Description", StringComparison.OrdinalIgnoreCase))
                        filters.Add("CONVERT([" + column.ColumnName + "], 'System.String') LIKE '%" + escaped + "%'");
                }
                if (filters.Count > 0) view.RowFilter = string.Join(" OR ", filters);
            }
            dgvServicesType.DataSource = view;
            lblRecordsCount.Text = view.Count.ToString();
            ApplyGridLanguage(IsArabic());
        }

        private void OpenAddService()
        {
            using (var frm = new frmAddUpdateServices()) frm.ShowDialog(this);
            _RefreshData();
        }

        private void OpenEditService()
        {
            if (!TryGetSelectedServiceID(out int id)) return;
            using (var frm = new frmAddUpdateServices(id)) frm.ShowDialog(this);
            _RefreshData();
        }

        private void DeleteSelectedService()
        {
            if (!TryGetSelectedServiceID(out int id)) return;
            var confirm = MessageBox.Show(
                IsArabic() ? "حذف هذه الخدمة؟ لا تحذف الخدمات المستخدمة في الفواتير."
                           : "Delete this service? Services referenced by existing bills should not be deleted.",
                IsArabic() ? "تأكيد الحذف" : "Confirm deletion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            try
            {
                if (!clsServicesType.Delete(id))
                    MessageBox.Show(IsArabic() ? "تعذر حذف الخدمة. قد تكون قيد الاستخدام."
                                               : "Unable to delete this service. It may be in use.");
                _RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, IsArabic() ? "خطأ في الحذف" : "Delete failed");
            }
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
                FixServicesGridLayout();
                ApplyGridLanguage(arabic);
                TranslateContextMenu(arabic);
                FixServicesHeader(this, arabic);
                UpdateServiceActionsLanguage();
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

                // The service grid has a runtime toolbar above it.
                // Restoring its Designer Top would hide the headers and first row.
                if (control == dgvServicesType)
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

        // Keep the toolbar above the grid, without covering its headers or rows.
        // Uses the original Designer rectangle as the available grid area.
        private void FixServicesGridLayout()
        {
            if (_serviceActions == null ||
                !_originalBounds.TryGetValue(dgvServicesType, out Rectangle original))
                return;

            int left = IsArabic()
                ? Math.Max(0, dgvServicesType.Parent.ClientSize.Width - original.Right)
                : original.Left;

            _serviceActions.SetBounds(left, original.Top, original.Width, _serviceActions.Height);
            dgvServicesType.SetBounds(
                left,
                original.Top + _serviceActions.Height + 4,
                original.Width,
                Math.Max(100, original.Height - _serviceActions.Height - 4));

            dgvServicesType.ColumnHeadersVisible = true;
            dgvServicesType.ColumnHeadersHeight = 40;
            dgvServicesType.ScrollBars = ScrollBars.Both;
            dgvServicesType.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvServicesType.BringToFront();
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
            FilterServices();

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
