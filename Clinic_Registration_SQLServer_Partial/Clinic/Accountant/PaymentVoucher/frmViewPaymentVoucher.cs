using Clinic_DataAccess;
using Guna.UI2.WinForms;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BindCombos;
//using YamyProject.DAL;
//using YamyProject.Localization;


namespace YamyProject
{
    public partial class frmViewPaymentVoucher : Form
    {
        int id, defaultCustomerId = 0;
        decimal totalAmount = 0;
        string code;
        bool isSubContractors;
        string VendorType = "Vendor";

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private Guna2Panel pnlBottomActions;
        private Guna2Panel _loadingPanel;
        private Guna2ProgressIndicator _loadingSpinner;
        private Label _loadingTitle;
        private Label _loadingStatus;
        private Label _loadingPercent;

        private int _loadingValue = 0;
        private bool _pageReady = false;
        public frmViewPaymentVoucher(
    int id = 0,
    int _customerId = 0,
    bool _isSubContractors = false)
        {
            InitializeComponent();

            this.id = id;

            // LocalizationManager.LocalizeForm(this);

            bool arabic = IsArabicLanguage();

            headerUC1.FormText =
                arabic
                    ? "سند الدفع"
                    : "Payment Voucher";

            //headerUC1.ArabicLayout = arabic;
            this.defaultCustomerId = _customerId;
            this.isSubContractors = _isSubContractors;

            CbSubcontractors.Checked = _isSubContractors;

            VendorType =
                isSubContractors
                    ? "Subcontractor"
                    : "Vendor";

            //AppTheme.DefaultFont(dgvInv);
        }
        private void CreateLoadingOverlay()
        {
            if (_loadingPanel != null)
                return;

            bool arabic = IsArabicLanguage();

            _loadingPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(247, 248, 252),
                BackColor = Color.FromArgb(247, 248, 252),
                Name = "pnlPaymentLoading"
            };

            _loadingSpinner = new Guna2ProgressIndicator
            {
                Size = new Size(64, 64),
                AutoStart = true,
                CircleSize = 1F
            };

            _loadingTitle = new Label
            {
                AutoSize = false,
                Height = 38,
                Width = 420,
                Font = new Font(
                    "Segoe UI Semibold",
                    15F,
                    FontStyle.Bold),
                ForeColor = Color.FromArgb(23, 42, 83),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = arabic
                    ? "جاري تحميل سند الصرف"
                    : "Loading Payment Voucher"
            };

            _loadingStatus = new Label
            {
                AutoSize = false,
                Height = 28,
                Width = 500,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = arabic
                    ? "يرجى الانتظار..."
                    : "Please wait..."
            };

            _loadingPercent = new Label
            {
                AutoSize = false,
                Height = 28,
                Width = 100,
                Font = new Font(
                    "Segoe UI Semibold",
                    10F,
                    FontStyle.Bold),
                ForeColor = Color.FromArgb(91, 55, 225),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "0%"
            };

            _loadingPanel.Controls.Add(_loadingSpinner);
            _loadingPanel.Controls.Add(_loadingTitle);
            _loadingPanel.Controls.Add(_loadingStatus);
            _loadingPanel.Controls.Add(_loadingPercent);

            Controls.Add(_loadingPanel);

            _loadingPanel.Resize += (s, e) =>
                PositionLoadingOverlay();

            PositionLoadingOverlay();

            _loadingPanel.BringToFront();
        }

        private void PositionLoadingOverlay()
        {
            if (_loadingPanel == null)
                return;

            int centerX = _loadingPanel.ClientSize.Width / 2;
            int centerY = _loadingPanel.ClientSize.Height / 2;

            _loadingSpinner.Location =
                new Point(
                    centerX - (_loadingSpinner.Width / 2),
                    centerY - 100);

            _loadingTitle.Location =
                new Point(
                    centerX - (_loadingTitle.Width / 2),
                    _loadingSpinner.Bottom + 15);

            _loadingStatus.Location =
                new Point(
                    centerX - (_loadingStatus.Width / 2),
                    _loadingTitle.Bottom + 3);

            _loadingPercent.Location =
                new Point(
                    centerX - (_loadingPercent.Width / 2),
                    _loadingStatus.Bottom + 3);
        }
        private async Task UpdateLoadingAsync(
    int value,
    string englishText,
    string arabicText)
        {
            if (_loadingPanel == null)
                return;

            bool arabic = IsArabicLanguage();

            value = Math.Max(0, Math.Min(100, value));

            // Smoothly move to requested percentage.
            while (_loadingValue < value)
            {
                _loadingValue++;

                _loadingPercent.Text =
                    $"{_loadingValue}%";

                if (_loadingValue % 2 == 0)
                {
                    await Task.Delay(4);
                    Application.DoEvents();
                }
            }

            _loadingStatus.Text =
                arabic
                    ? arabicText
                    : englishText;

            _loadingPanel.BringToFront();

            await Task.Yield();
        }
        private async void frmViewPaymentVoucher_Load(
object sender,
EventArgs e)
        {
            if (_pageReady)
                return;

            CreateLoadingOverlay();

            _loadingPanel.BringToFront();

            try
            {
                // =====================================================
                // INITIAL
                // =====================================================

                await UpdateLoadingAsync(
                    5,
                    "Preparing payment voucher...",
                    "جاري تجهيز سند الصرف...");

                dtOpen.Value =
                    dtpTransDate.Value =
                    dt_check_date.Value =
                    DateTime.Now.Date;

                //AppTheme.DefaultFont(dgvInv);

                txtPVCode.Text =
                    GenerateNextPaymentCode();

                txtId.Text =
                    GenerateNextPaymentId();

                // =====================================================
                // EMPLOYEES
                // =====================================================

                await UpdateLoadingAsync(
                    15,
                    "Loading employees...",
                    "جاري تحميل الموظفين...");

                BindCombos.PopulateListEmployeesNew(
                    lstOfEmp,
                    false);

                await Task.Yield();

                // =====================================================
                // COST CENTERS
                // =====================================================

                await UpdateLoadingAsync(
                    30,
                    "Loading cost centers...",
                    "جاري تحميل مراكز التكلفة...");

                BindCombos.PopulateCostCenter(
                    cmbDebitCostCenter,
                    true);

                BindCombos.PopulateCostCenter(
                    cmbCreditCostCenter,
                    true);

                await Task.Yield();

                // =====================================================
                // ACCOUNTS
                // =====================================================

                await UpdateLoadingAsync(
                    48,
                    "Loading accounts...",
                    "جاري تحميل الحسابات...");

                BindCombos.PopulateAllLevel4Account(
                    cmbDebitAccountName,
                    true);

                BindCombos.PopulateAllLevel4Account(
                    cmbCreditAccountName,
                    true);

                await Task.Yield();

                // =====================================================
                // BANKS
                // =====================================================

                await UpdateLoadingAsync(
                    62,
                    "Loading bank information...",
                    "جاري تحميل معلومات البنوك...");

                BindCombos.PopulateRegisterBanks(
                    cmbBankName,
                    true);

                // =====================================================
                // DEFAULT VALUES
                // =====================================================

                await UpdateLoadingAsync(
                    70,
                    "Preparing voucher information...",
                    "جاري تجهيز بيانات السند...");

                cmbMethod.SelectedIndex = 0;
                cmbPaymentType.SelectedIndex = 0;

                // =====================================================
                // EXISTING / NEW VOUCHER
                // =====================================================

                if (id > 0)
                {
                    await UpdateLoadingAsync(
                        78,
                        "Loading voucher...",
                        "جاري تحميل بيانات السند...");

                    btnSave.Enabled =
                        BtnSaveNew.Enabled =
                        UserPermissions.canEdit("Vouchers");

                    BtnDelete.Enabled =
                        UserPermissions.canDelete("Vouchers");

                    BindVoucher();
                }
                else
                {
                    if (defaultCustomerId > 0)
                    {
                        await UpdateLoadingAsync(
                            78,
                            "Loading customer information...",
                            "جاري تحميل بيانات العميل...");

                        cmbVendor.SelectedValue =
                            defaultCustomerId;

                        LoadData();
                    }
                }

                // =====================================================
                // FINAL DESIGN
                // =====================================================

                await UpdateLoadingAsync(
                    88,
                    "Preparing interface...",
                    "جاري تجهيز واجهة سند الصرف...");

                SuspendLayout();

                try
                {
                    // LocalizationManager
                    //.LocalizeDataGridViewHeaders(dgvInv);

                    ApplyModernDesign();

                    StyleInvoiceGrid();

                    StylePaymentLayout();

                    // Localization AFTER English base design.
                    // LocalizationManager.LocalizeForm(this);

                    // LocalizationManager
                    //.LocalizeDataGridViewHeaders(dgvInv);

                    ApplyLanguageLayout(
                        IsArabicLanguage());
                }
                finally
                {
                    ResumeLayout(true);
                }

                // =====================================================
                // FINAL RENDER
                // =====================================================

                await UpdateLoadingAsync(
                    95,
                    "Finalizing page...",
                    "جاري إنهاء تحميل الصفحة...");

                PerformLayout();

                Refresh();

                await Task.Delay(40);

                Application.DoEvents();

                _pageReady = true;

                await UpdateLoadingAsync(
                    100,
                    "Ready",
                    "تم التحميل");

                await Task.Delay(80);

                _loadingPanel.Visible = false;

                // Ensure the real UI gets one clean final paint.
                Invalidate(true);
                Update();
            }
            catch (Exception ex)
            {
                if (_loadingPanel != null)
                    _loadingPanel.Visible = false;

                MessageBox.Show(
                    IsArabicLanguage()
                        ? "حدث خطأ أثناء تحميل سند الصرف:\n" +
                          ex.Message
                        : "Error loading Payment Voucher:\n" +
                          ex.Message,
                    IsArabicLanguage()
                        ? "خطأ"
                        : "Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private bool IsArabicLanguage()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.StartsWith(
                "ar",
                StringComparison.OrdinalIgnoreCase);
        }

        public void ApplyLanguageLayout(bool isArabic)
        {
            SuspendLayout();

            try
            {
                // Keep the form itself LTR. We reposition the controls ourselves.
                // This avoids WinForms/Guna double-mirroring.
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                headerUC1.FormText =
                    isArabic ? "سند صرف" : "Payment Voucher";

                Text =
                    isArabic ? "سند صرف" : "Payment Voucher";

                tabPage3.Text =
                    isArabic ? "الرئيسية" : "Main";

                tabPage4.Text =
                    isArabic ? "التقارير" : "Reports";

                // Keep the tab captions on the right in Arabic and on the left in English.
                // This changes only the presentation; it does not change any business logic.
                tabControl2.RightToLeft = isArabic ? RightToLeft.Yes : RightToLeft.No;
                tabControl2.RightToLeftLayout = isArabic;
                tabControl2.Alignment = TabAlignment.Top;

                txtcostcenter.Text =
                    isArabic
                        ? "معلومات سند الصرف"
                        : "Payment Voucher Information";

                guna2GroupBox2.Text =
                    isArabic
                        ? "الدفع إلى - مدين"
                        : "Pay To - Debit";

                guna2GroupBox3.Text =
                    isArabic
                        ? "الدفع من - دائن"
                        : "Pay From - Credit";

                // Toolbar
                BtnClear.Text = isArabic ? "جديد" : "New";
                BtnSaveNew.Text = isArabic ? "حفظ" : "Save";
                BtnDelete.Text = isArabic ? "حذف" : "Delete";
                BtnCopy.Text = isArabic ? "نسخ" : "Copy";
                BtnPrint.Text = isArabic ? "طباعة" : "Print";
                BtnEmail.Text = isArabic ? "البريد الإلكتروني" : "Email";
                BtnAttach.Text = isArabic ? "إرفاق" : "Attach";

                // Bottom action buttons
                guna2Button3.Text =
                    isArabic ? "💾  حفظ وإغلاق" : "💾  Save & Close";

                btnSave.Text =
                    isArabic ? "💾  حفظ" : "💾  Save";

                btnClose.Text =
                    isArabic ? "✕  إغلاق" : "✕  Close";

                // Known captions that are visible on this screen.
                label2.Text = isArabic ? "التاريخ" : "Date";
                label1.Text = isArabic ? "نوع الدفع" : "Payment Type";
                lblBalance.Text = isArabic ? "طريقة الدفع" : "Payment Method";

                label3.Text = isArabic ? "كود الحساب" : "Account Code";
                label9.Text = isArabic ? "اسم الحساب" : "Account Name";
                lblCostcenter.Text = isArabic ? "مركز التكلفة" : "Cost Center";
                label4.Text = isArabic ? "ملاحظة" : "Description";

                label7.Text = isArabic ? "كود الحساب" : "Account Code";
                label6.Text = isArabic ? "اسم الحساب" : "Account Name";
                label10.Text = isArabic ? "مركز التكلفة" : "Cost Center";

                CbSubcontractors.Text =
                    isArabic ? "المقاولون من الباطن" : "Subcontractors";

                // Direction of inputs and grid.
                SetPaymentInputDirection(this, isArabic);

                dgvInv.RightToLeft =
                    isArabic ? RightToLeft.Yes : RightToLeft.No;

                dgvInv.ColumnHeadersDefaultCellStyle.Alignment =
                    isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;

                dgvInv.DefaultCellStyle.Alignment =
                    isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;

                // Reposition the major fields to the opposite side.
                PositionPaymentLanguageControls(isArabic);

                // Footer is intentionally on LEFT for Arabic.
                PositionBottomButtons();
            }
            finally
            {
                ResumeLayout(true);
                Invalidate(true);
            }
        }

        private void SetPaymentInputDirection(
            Control parent,
            bool isArabic)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (control is Guna2TextBox ||
                    control is TextBox ||
                    control is Guna2ComboBox ||
                    control is ComboBox ||
                    control is Label ||
                    control is CheckBox)
                {
                    control.RightToLeft =
                        isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                }

                SetPaymentInputDirection(
                    control,
                    isArabic);
            }
        }

        private void PositionPaymentLanguageControls(bool isArabic)
        {
            // Always restore the known English layout first.
            // This prevents controls from being mirrored again when localization is reapplied.
            ApplyModernLayout();

            if (!isArabic)
                return;

            int infoWidth = txtcostcenter.ClientSize.Width;

            if (isArabic)
            {
                // Payment information card.
                int oldDateX = 18;
                int dateWidth = 190;

                label2.Left =
                    infoWidth - oldDateX - label2.Width;
                dtOpen.Left =
                    infoWidth - oldDateX - dateWidth;

                txtAmount.Left =
                    infoWidth - 18 - txtAmount.Width;

                txtAmountInWord.Left =
                    txtAmount.Left - 12 - txtAmountInWord.Width;

                int paymentTypeX = 18;
                label1.Left = paymentTypeX;
                cmbPaymentType.Left = paymentTypeX;

                lblBalance.Left = paymentTypeX;
                cmbMethod.Left = paymentTypeX;

                CbSubcontractors.Left =
                    cmbPaymentType.Right + 18;

                // Debit section: mirror the normal left-side account fields.
                MirrorControlInParent(label3, guna2ShadowPanel1);
                MirrorControlInParent(txtDebitCode, guna2ShadowPanel1);
                MirrorControlInParent(label9, guna2ShadowPanel1);
                MirrorControlInParent(cmbDebitAccountName, guna2ShadowPanel1);
                MirrorControlInParent(lblCostcenter, guna2ShadowPanel1);
                MirrorControlInParent(cmbDebitCostCenter, guna2ShadowPanel1);
                MirrorControlInParent(label4, guna2ShadowPanel1);
                MirrorControlInParent(txtDescription1, guna2ShadowPanel1);

                // Customer/vendor panel moves to the left.
                pnlCustomer.Left = 10;

                // Credit section.
                MirrorControlInParent(label7, guna2ShadowPanel2);
                MirrorControlInParent(txtCreditAccountCode, guna2ShadowPanel2);
                MirrorControlInParent(label6, guna2ShadowPanel2);
                MirrorControlInParent(cmbCreditAccountName, guna2ShadowPanel2);
                MirrorControlInParent(label10, guna2ShadowPanel2);
                MirrorControlInParent(cmbCreditCostCenter, guna2ShadowPanel2);
            }

        }

        private void MirrorControlInParent(
            Control control,
            Control parent)
        {
            if (control == null || parent == null)
                return;

            control.Left =
                parent.ClientSize.Width
                - control.Left
                - control.Width;
        }

        private void ApplyModernDesign()
        {
            Color purple = Color.FromArgb(91, 55, 225);
            Color purpleDark = Color.FromArgb(72, 43, 180);
            Color pageColor = Color.FromArgb(247, 248, 252);
            Color borderColor = Color.FromArgb(226, 228, 238);
            Color titleColor = Color.FromArgb(23, 42, 83);
            Color sectionColor = Color.FromArgb(246, 243, 255);

            SuspendLayout();

            BackColor = pageColor;
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1100, 720);
            AutoScroll = true;

            headerUC1.FormText = IsArabicLanguage() ? "سند صرف" : "Payment Voucher";
            headerUC1.BringToFront();

            // Toolbar
            panel9.BackColor = Color.White;
            panel9.Height = 105;

            tabControl2.Font = new Font("Segoe UI Semibold", 9F);
            tabPage3.BackColor = Color.White;
            tabPage4.BackColor = Color.White;

            txtPVCode.BackColor = Color.White;
            txtPVCode.ForeColor = titleColor;
            txtPVCode.BorderStyle = BorderStyle.None;
            txtPVCode.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            Guna2TileButton[] toolbarButtons =
            {
                BtnClear, BtnSaveNew, BtnDelete, BtnCopy,
                guna2TileButton19, BtnPrint, BtnEmail, BtnAttach,
                guna2TileButton15
            };

            foreach (Guna2TileButton button in toolbarButtons)
            {
                if (button == null)
                    continue;

                button.FillColor = Color.White;
                button.ForeColor = titleColor;
                button.Font = new Font("Segoe UI Semibold", 8.5F);
                button.BorderRadius = 7;
                button.Cursor = Cursors.Hand;
                button.HoverState.FillColor = sectionColor;
                button.HoverState.ForeColor = purpleDark;
            }

            BtnDelete.ForeColor = Color.FromArgb(220, 38, 38);

            btnPrevious.FillColor = Color.White;
            btnPrevious.ForeColor = titleColor;
            btnPrevious.BorderRadius = 7;
            btnPrevious.HoverState.FillColor = sectionColor;

            btnNext.FillColor = Color.White;
            btnNext.ForeColor = titleColor;
            btnNext.BorderRadius = 7;
            btnNext.HoverState.FillColor = sectionColor;

            // Voucher information card
            txtcostcenter.Dock = DockStyle.None;
            txtcostcenter.FillColor = Color.White;
            txtcostcenter.BackColor = pageColor;
            txtcostcenter.BorderColor = borderColor;
            txtcostcenter.BorderThickness = 1;
            txtcostcenter.BorderRadius = 10;
            txtcostcenter.CustomBorderColor = sectionColor;
            txtcostcenter.CustomBorderThickness = new Padding(0, 36, 0, 0);
            txtcostcenter.Text = "Payment Voucher Information";
            txtcostcenter.ForeColor = titleColor;
            txtcostcenter.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            txtcostcenter.TextOffset = new Point(10, 0);
            txtcostcenter.Padding = new Padding(0);

            // Debit card
            guna2GroupBox2.Dock = DockStyle.None;
            guna2GroupBox2.FillColor = Color.White;
            guna2GroupBox2.BackColor = pageColor;
            guna2GroupBox2.BorderColor = borderColor;
            guna2GroupBox2.BorderThickness = 1;
            guna2GroupBox2.BorderRadius = 10;
            guna2GroupBox2.CustomBorderColor = sectionColor;
            guna2GroupBox2.CustomBorderThickness = new Padding(0, 36, 0, 0);
            guna2GroupBox2.Text = "Pay To - Debit";
            guna2GroupBox2.ForeColor = titleColor;
            guna2GroupBox2.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            guna2GroupBox2.TextOffset = new Point(10, 0);

            // Credit card
            guna2GroupBox3.Dock = DockStyle.None;
            guna2GroupBox3.FillColor = Color.White;
            guna2GroupBox3.BackColor = pageColor;
            guna2GroupBox3.BorderColor = borderColor;
            guna2GroupBox3.BorderThickness = 1;
            guna2GroupBox3.BorderRadius = 10;
            guna2GroupBox3.CustomBorderColor = sectionColor;
            guna2GroupBox3.CustomBorderThickness = new Padding(0, 36, 0, 0);
            guna2GroupBox3.Text = "Pay From - Credit";
            guna2GroupBox3.ForeColor = titleColor;
            guna2GroupBox3.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            guna2GroupBox3.TextOffset = new Point(10, 0);

            guna2ShadowPanel1.FillColor = Color.White;
            guna2ShadowPanel1.ShadowDepth = 0;
            guna2ShadowPanel1.ShadowColor = Color.Transparent;

            guna2ShadowPanel2.FillColor = Color.White;
            guna2ShadowPanel2.ShadowDepth = 0;
            guna2ShadowPanel2.ShadowColor = Color.Transparent;

            pnlCustomer.BackColor = Color.White;
            pnlBank.BackColor = Color.White;
            pnlTrans.BackColor = Color.White;

            foreach (Control control in Controls)
                StylePaymentControl(control);

            txtAmount.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            txtAmount.ForeColor = purple;
            txtAmount.FillColor = Color.White;
            txtAmount.BorderColor = borderColor;
            txtAmount.BorderRadius = 6;
            txtAmount.TextAlign = HorizontalAlignment.Right;

            txtAmountInWord.FillColor = Color.FromArgb(248, 249, 252);
            txtAmountInWord.BorderColor = borderColor;
            txtAmountInWord.BorderRadius = 6;

            CbSubcontractors.ForeColor = titleColor;
            CbSubcontractors.Font = new Font("Segoe UI Semibold", 8.5F);
            CbSubcontractors.CheckedState.FillColor = purple;
            CbSubcontractors.CheckedState.BorderColor = purple;

            chkPrint.ForeColor = titleColor;
            chkPrint.Font = new Font("Segoe UI", 8.5F);

            StyleInvoiceGrid();
            StyleBottomButtons();
            ApplyModernLayout();

            ResumeLayout(true);
        }

        private void ApplyModernLayout()
        {
            int margin = 18;
            int gap = 10;
            int availableWidth = ClientSize.Width - (margin * 2);

            if (availableWidth < 800)
                availableWidth = 800;

            // Toolbar
            panel9.Dock = DockStyle.None;
            panel9.Left = margin;
            panel9.Top = headerUC1.Bottom + 4;
            panel9.Width = availableWidth;
            panel9.Height = 100;
            panel9.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Payment information
            txtcostcenter.Location = new Point(margin, panel9.Bottom + gap);
            txtcostcenter.Size = new Size(availableWidth, 145);
            txtcostcenter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            label2.Location = new Point(18, 45);
            label2.AutoSize = true;
            dtOpen.Location = new Point(18, 67);
            dtOpen.Size = new Size(190, 28);

            // Keep the amount fields fully visible and avoid overlap on different window sizes.
            int amountWidth = Math.Min(220, Math.Max(170, (availableWidth - 60) / 4));
            int amountWordsLeft = 18 + amountWidth + 12;
            int amountWordsWidth = Math.Min(390, Math.Max(220, availableWidth - amountWordsLeft - 30));

            txtAmount.Location = new Point(18, 105);
            txtAmount.Size = new Size(amountWidth, 30);

            txtAmountInWord.Location = new Point(amountWordsLeft, 105);
            txtAmountInWord.Size = new Size(amountWordsWidth, 30);
            txtAmountInWord.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            int rightColumnX = Math.Max(660, availableWidth - 420);

            label1.Location = new Point(rightColumnX, 45);
            label1.AutoSize = true;
            cmbPaymentType.Location = new Point(rightColumnX, 67);
            cmbPaymentType.Size = new Size(260, 30);

            lblBalance.Location = new Point(rightColumnX, 99);
            lblBalance.AutoSize = true;
            cmbMethod.Location = new Point(rightColumnX, 105);
            cmbMethod.Size = new Size(260, 30);

            CbSubcontractors.Location = new Point(rightColumnX + 275, 72);

            // Debit
            guna2GroupBox2.Location = new Point(margin, txtcostcenter.Bottom + gap);
            guna2GroupBox2.Size = new Size(availableWidth, 190);
            guna2GroupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            guna2ShadowPanel1.Location = new Point(10, 42);
            guna2ShadowPanel1.Size = new Size(guna2GroupBox2.ClientSize.Width - 20, 138);
            guna2ShadowPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            label3.Location = new Point(15, 8);
            label3.AutoSize = true;
            txtDebitCode.Location = new Point(15, 31);
            txtDebitCode.Size = new Size(180, 30);

            label9.Location = new Point(210, 8);
            label9.AutoSize = true;
            cmbDebitAccountName.Location = new Point(210, 31);
            cmbDebitAccountName.Size = new Size(315, 30);

            lblCostcenter.Location = new Point(540, 8);
            lblCostcenter.AutoSize = true;
            cmbDebitCostCenter.Location = new Point(540, 31);
            cmbDebitCostCenter.Size = new Size(200, 30);

            label4.Location = new Point(15, 66);
            label4.AutoSize = true;
            txtDescription1.Location = new Point(15, 87);
            txtDescription1.Size = new Size(725, 42);

            pnlCustomer.Location = new Point(760, 8);
            pnlCustomer.Size = new Size(
                Math.Max(250, guna2ShadowPanel1.ClientSize.Width - 775),
                121);
            pnlCustomer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Credit
            guna2GroupBox3.Location = new Point(margin, guna2GroupBox2.Bottom + gap);
            guna2GroupBox3.Size = new Size(availableWidth, 140);
            guna2GroupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            guna2ShadowPanel2.Location = new Point(10, 42);
            guna2ShadowPanel2.Size = new Size(guna2GroupBox3.ClientSize.Width - 20, 88);
            guna2ShadowPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            label7.Location = new Point(15, 8);
            label7.AutoSize = true;
            txtCreditAccountCode.Location = new Point(15, 31);
            txtCreditAccountCode.Size = new Size(180, 30);

            label6.Location = new Point(210, 8);
            label6.AutoSize = true;
            cmbCreditAccountName.Location = new Point(210, 31);
            cmbCreditAccountName.Size = new Size(315, 30);

            label10.Location = new Point(540, 8);
            label10.AutoSize = true;
            cmbCreditCostCenter.Location = new Point(540, 31);
            cmbCreditCostCenter.Size = new Size(200, 30);

            CreateBottomActionBar();

            // Invoice grid
            dgvInv.Dock = DockStyle.None;
            dgvInv.Location = new Point(margin, guna2GroupBox3.Bottom + gap);
            dgvInv.Width = availableWidth;

            int footerTop = pnlBottomActions != null
                ? pnlBottomActions.Top
                : ClientSize.Height - 90;

            dgvInv.Height = Math.Max(100, footerTop - dgvInv.Top - 12);
            dgvInv.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            PositionBottomButtons();

            panel9.BringToFront();
            txtcostcenter.BringToFront();
            guna2GroupBox2.BringToFront();
            guna2GroupBox3.BringToFront();
            dgvInv.BringToFront();

            if (pnlBottomActions != null)
                pnlBottomActions.BringToFront();
        }
        private void StyleBottomButtons()
        {
            Color purple = Color.FromArgb(79, 70, 229);
            Color purpleHover = Color.FromArgb(67, 56, 202);
            Color textDark = Color.FromArgb(30, 41, 59);
            Color border = Color.FromArgb(226, 232, 240);

            // =========================================
            // SAVE & CLOSE
            // =========================================
            guna2Button3.Text = "💾  Save & Close";
            guna2Button3.Size = new Size(160, 46);

            guna2Button3.FillColor = purple;
            guna2Button3.ForeColor = Color.White;

            guna2Button3.BorderRadius = 8;
            guna2Button3.BorderThickness = 0;

            guna2Button3.Font =
                new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            guna2Button3.Cursor = Cursors.Hand;

            guna2Button3.ShadowDecoration.Enabled = true;
            guna2Button3.ShadowDecoration.Depth = 5;
            guna2Button3.ShadowDecoration.Shadow =
    new Padding(2, 2, 3, 3);

            guna2Button3.HoverState.FillColor = purpleHover;
            guna2Button3.HoverState.ForeColor = Color.White;


            // =========================================
            // SAVE
            // =========================================
            btnSave.Text = "💾  Save";
            btnSave.Size = new Size(125, 46);

            btnSave.FillColor = purple;
            btnSave.ForeColor = Color.White;

            btnSave.BorderRadius = 8;
            btnSave.BorderThickness = 0;

            btnSave.Font =
                new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            btnSave.Cursor = Cursors.Hand;

            btnSave.ShadowDecoration.Enabled = true;
            btnSave.ShadowDecoration.Depth = 5;
            btnSave.ShadowDecoration.Shadow =
    new Padding(2, 2, 3, 3);

            btnSave.HoverState.FillColor = purpleHover;
            btnSave.HoverState.ForeColor = Color.White;


            // =========================================
            // CLOSE
            // =========================================
            btnClose.Text = "✕  Close";
            btnClose.Size = new Size(125, 46);

            btnClose.FillColor = Color.White;
            btnClose.ForeColor = textDark;

            btnClose.BorderRadius = 8;
            btnClose.BorderThickness = 1;
            btnClose.BorderColor = border;

            btnClose.Font =
                new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            btnClose.Cursor = Cursors.Hand;

            btnClose.ShadowDecoration.Enabled = true;
            btnClose.ShadowDecoration.Depth = 5;
            btnClose.ShadowDecoration.Shadow =
    new Padding(2, 2, 3, 3);
            btnClose.HoverState.FillColor =
                Color.FromArgb(248, 250, 252);

            btnClose.HoverState.BorderColor =
                Color.FromArgb(203, 213, 225);
        }
        private void StylePaymentControl(Control control)
        {
            if (control == null)
                return;

            // ============================================
            // TEXTBOX
            // ============================================
            if (control is Guna2TextBox textBox)
            {
                textBox.BorderRadius = 6;

                textBox.BorderColor =
                    Color.FromArgb(211, 213, 222);

                textBox.FocusedState.BorderColor =
                    Color.FromArgb(103, 58, 183);

                textBox.HoverState.BorderColor =
                    Color.FromArgb(170, 158, 195);

                textBox.FillColor =
                    textBox.Enabled
                        ? Color.White
                        : Color.FromArgb(243, 244, 247);

                textBox.ForeColor =
                    Color.FromArgb(50, 46, 62);

                textBox.Font =
                    new Font(
                        "Segoe UI",
                        9F);
            }


            // ============================================
            // COMBOBOX
            // ============================================
            else if (control is Guna2ComboBox comboBox)
            {
                comboBox.BorderRadius = 6;

                comboBox.BorderColor =
                    Color.FromArgb(211, 213, 222);

                comboBox.FocusedColor =
                    Color.FromArgb(103, 58, 183);

                comboBox.FillColor =
                    Color.White;

                comboBox.ForeColor =
                    Color.FromArgb(50, 46, 62);

                comboBox.Font =
                    new Font(
                        "Segoe UI",
                        9F);
            }


            // ============================================
            // DATE PICKER
            // ============================================
            else if (
                control is Guna2DateTimePicker picker)
            {
                picker.BorderRadius = 6;

                picker.BorderColor =
                    Color.FromArgb(211, 213, 222);

                picker.BorderThickness = 1;

                picker.FillColor =
                    Color.White;

                picker.ForeColor =
                    Color.FromArgb(50, 46, 62);

                picker.Font =
                    new Font(
                        "Segoe UI",
                        9F);
            }


            // ============================================
            // LABEL
            // ============================================
            else if (control is Label label)
            {
                label.ForeColor =
                    Color.FromArgb(62, 57, 78);

                label.Font =
                    new Font(
                        "Segoe UI",
                        8.5F,
                        FontStyle.Regular);
            }


            // ============================================
            // PANEL
            // ============================================
            else if (control is Panel panel)
            {
                if (panel.BackColor ==
                    SystemColors.Control)
                {
                    panel.BackColor =
                        Color.FromArgb(248, 249, 252);
                }
            }


            foreach (Control child in control.Controls)
            {
                StylePaymentControl(child);
            }
        }
        private void CreateBottomActionBar()
        {
            if (pnlBottomActions != null)
                return;

            pnlBottomActions = new Guna2Panel
            {
                Name = "pnlBottomActions",
                Dock = DockStyle.Bottom,
                Height = 82,
                FillColor = Color.White,
                BackColor = Color.White,
                BorderThickness = 0
            };

            if (guna2Button3.Parent != null)
                guna2Button3.Parent.Controls.Remove(guna2Button3);

            if (btnSave.Parent != null)
                btnSave.Parent.Controls.Remove(btnSave);

            if (btnClose.Parent != null)
                btnClose.Parent.Controls.Remove(btnClose);

            panel1.Visible = false;

            Controls.Add(pnlBottomActions);

            pnlBottomActions.Controls.Add(guna2Button3);
            pnlBottomActions.Controls.Add(btnSave);
            pnlBottomActions.Controls.Add(btnClose);

            guna2Button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            pnlBottomActions.Resize -= pnlBottomActions_Resize;
            pnlBottomActions.Resize += pnlBottomActions_Resize;

            PositionBottomButtons();
            pnlBottomActions.BringToFront();
        }

        private void pnlBottomActions_Resize(object sender, EventArgs e)
        {
            PositionBottomButtons();
        }
        private void PositionBottomButtons()
        {
            if (pnlBottomActions == null)
                return;

            int margin = 22;
            int gap = 14;
            int y = 16;

            guna2Button3.Size = new Size(160, 46);
            btnSave.Size = new Size(125, 46);
            btnClose.Size = new Size(125, 46);

            bool isArabic = IsArabicLanguage();

            if (isArabic)
            {
                // Arabic: Save & Close, Save and Close are on the LEFT.
                guna2Button3.Anchor =
                    AnchorStyles.Top | AnchorStyles.Left;
                btnSave.Anchor =
                    AnchorStyles.Top | AnchorStyles.Left;
                btnClose.Anchor =
                    AnchorStyles.Top | AnchorStyles.Left;

                guna2Button3.Location =
                    new Point(margin, y);

                btnSave.Location =
                    new Point(
                        guna2Button3.Right + gap,
                        y);

                btnClose.Location =
                    new Point(
                        btnSave.Right + gap,
                        y);
            }
            else
            {
                // English: keep the original right-side footer.
                guna2Button3.Anchor =
                    AnchorStyles.Top | AnchorStyles.Right;
                btnSave.Anchor =
                    AnchorStyles.Top | AnchorStyles.Right;
                btnClose.Anchor =
                    AnchorStyles.Top | AnchorStyles.Right;

                btnClose.Location =
                    new Point(
                        pnlBottomActions.ClientSize.Width
                        - margin
                        - btnClose.Width,
                        y);

                btnSave.Location =
                    new Point(
                        btnClose.Left
                        - gap
                        - btnSave.Width,
                        y);

                guna2Button3.Location =
                    new Point(
                        btnSave.Left
                        - gap
                        - guna2Button3.Width,
                        y);
            }

            guna2Button3.BringToFront();
            btnSave.BringToFront();
            btnClose.BringToFront();
            pnlBottomActions.BringToFront();
        }
        private void StylePaymentLayout()
        {
            dgvInv.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            // Toolbar button
            BtnSaveNew.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left;

            CreateBottomActionBar();
        }
        private void StyleInvoiceGrid()
        {
            dgvInv.BackgroundColor = Color.White;
            dgvInv.BorderStyle = BorderStyle.None;

            dgvInv.RowHeadersVisible = false;
            dgvInv.EnableHeadersVisualStyles = false;

            dgvInv.ColumnHeadersHeight = 42;

            dgvInv.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;

            dgvInv.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);

            dgvInv.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(30, 41, 59);

            dgvInv.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI Semibold",
                    9F,
                    FontStyle.Bold);

            dgvInv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dgvInv.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(8, 0, 0, 0);

            dgvInv.RowTemplate.Height = 40;

            dgvInv.DefaultCellStyle.BackColor =
                Color.White;

            dgvInv.DefaultCellStyle.ForeColor =
                Color.FromArgb(51, 65, 85);

            dgvInv.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            dgvInv.DefaultCellStyle.Padding =
                new Padding(7, 3, 7, 3);

            dgvInv.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(238, 242, 255);

            dgvInv.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(67, 56, 202);

            dgvInv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(250, 251, 253);

            dgvInv.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            dgvInv.GridColor =
                Color.FromArgb(241, 245, 249);

            dgvInv.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvInv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void BindVoucher()
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT * FROM tbl_payment_voucher WHERE id = " + id))
                if (reader.Read())
                {
                    txtId.Text = reader["id"].ToString();
                    txtPVCode.Text = reader["code"].ToString();
                    dtOpen.Value = DateTime.Parse(reader["date"].ToString());
                    code = reader["code"].ToString();
                    string typeValue = reader["type"].ToString();
                    cmbPaymentType.SelectedIndex = cmbPaymentType.FindStringExact(typeValue);
                    string methodValue = reader["method"].ToString();
                    cmbMethod.SelectedIndex = cmbMethod.FindStringExact(methodValue);
                    txtAmount.Text = reader["amount"].ToString();
                    cmbCreditAccountName.SelectedValue = reader["credit_account_id"].ToString();
                    cmbCreditCostCenter.SelectedValue = reader["credit_cost_center_id"].ToString();
                    txtDescription1.Text = reader["description"].ToString();
                    cmbDebitAccountName.SelectedValue = reader["debit_account_id"].ToString();
                    cmbDebitCostCenter.SelectedValue = reader["debit_cost_center_id"].ToString();

                    if (reader["bank_id"] != null)
                    {
                        if (cmbMethod.Text == "Cheque")
                        {
                            cmbBankName.SelectedValue = reader["bank_id"].ToString();
                        }
                    }
                    //txtBankCode.Text = reader["bank_code"].ToString();
                    if (cmbMethod.Text == "Cheque")
                    {
                        txtCheckName.Text = reader["check_name"].ToString();
                        cmbCheckNo.SelectedValue = reader["check_no"].ToString();
                    }
                    if (reader["trans_date"].ToString() != "")
                    {
                        dtpTransDate.Value = DateTime.Parse(reader["trans_date"].ToString());
                    }
                    txtTransName.Text = reader["trans_name"].ToString();
                    txtTransRef.Text = reader["trans_ref"].ToString();

                    if (cmbPaymentType.Text == "Vendor")
                    {
                        txtAmount.Enabled = false;
                        BindCombos.PopulateVendors(cmbVendor, false, false, VendorType);
                        dgvInv.Columns["InvNo"].HeaderText = "Inv No";
                        cmbCreditCostCenter.Visible =
                        lblCostcenter.Visible =
                        dgvInv.Visible = true;
                    }
                    else if (cmbPaymentType.Text == "Petty Cash")
                    {
                        //txtAmount.Enabled = true;
                        //cmbCreditCostCenter.Visible = false;
                        //lblCostcenter.Visible = dgvInv.Visible = true;
                    }

                    loadPaymentDataDetails();
                }
        }
        private void loadPaymentDataDetails()
        {
            dgvInv.Rows.Clear();

            int serialNumber = 1;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
                     SELECT dt.id,dt.date,dt.payment_id,dt.hum_id,dt.inv_code,dt.inv_id,dt.payment,dt.total,dt.description,dt.cost_center_id,voucher_type
                     FROM tbl_payment_voucher_details dt
                     WHERE dt.payment_id = @id;",
                    ClinicDatabase.CreateParameterNew("id", id)))
                while (reader.Read())
                {
                    string query = "";
                    if (cmbPaymentType.Text == "Vendor")
                    {
                        query = @"
                            SELECT tbl_purchase.date,tbl_purchase.id,tbl_purchase.invoice_id,tbl_purchase.net,tbl_purchase.pay,tbl_purchase.change 
                            FROM tbl_purchase 
                            INNER JOIN tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id
                            WHERE tbl_purchase.id = @id
                            ";
                    }
                    else if (cmbPaymentType.Text == "Employee")
                    {
                        query = @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_attendance_salary.date) AS SN, tbl_attendance_salary.date,tbl_attendance_salary.id,tbl_attendance_salary.ss_no invoice_id,
                                tbl_attendance_salary.net_salary net,tbl_attendance_salary.pay,tbl_attendance_salary.change FROM tbl_attendance_salary 
                                WHERE emp_code = (SELECT CODE FROM tbl_employee where id=@id ) AND `change` <> 0";
                    }
                    else if (cmbPaymentType.Text == "Petty Cash")
                    {
                        query = @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_petty_cash_request.request_date) AS SN, 
                                tbl_petty_cash_request.request_date date,tbl_petty_cash_request.id,tbl_petty_cash_request.request_ref invoice_id,tbl_petty_cash_request.amount net,
                                tbl_petty_cash_request.pay,tbl_petty_cash_request.change FROM tbl_petty_cash_request 
                                WHERE tbl_petty_cash_request.Petty_cash_name = @id AND `change` <> 0";
                    }

                    string inv_id = "0", inv_code = reader["inv_code"].ToString(), bill_amount = "0", bill_date = "", payment = "0", description = "", cost_center_id = "0";
                    cost_center_id = (reader["cost_center_id"].ToString() == "" ? null : reader["cost_center_id"].ToString());
                    description = reader["description"].ToString();
                    payment = Utilities.FormatDecimal(reader["payment"].ToString());
                    string humId = reader["hum_id"].ToString();
                    cmbVendor.SelectedValue = int.Parse(humId);
                    string vType = reader["voucher_type"].ToString();
                    //if (vType.StartsWith("Petty Cash"))
                    //{
                    //    cbpettycash.Enabled = true;
                    //    cbpettycash.Checked = true;
                    //    cbpettycash.Visible = true;
                    //    lstOfEmp.Enabled = true;
                    //}
                    string _refId = (cmbPaymentType.Text == "Employee" ? reader["hum_id"].ToString() : cmbPaymentType.Text == "Petty Cash" ? reader["hum_id"].ToString() : reader["inv_id"].ToString());
                    using (MySqlDataReader drSale = ClinicDatabase.ExecuteReader(query,
                        ClinicDatabase.CreateParameterNew("id", _refId)))
                        if (drSale.Read())
                        {
                            inv_id = drSale["id"].ToString();
                            bill_amount = Utilities.FormatDecimal(drSale["net"].ToString());
                            bill_date = DateTime.Parse(drSale["date"].ToString()).ToShortDateString();
                        }
                        else
                        {
                            bill_amount = Utilities.FormatDecimal(reader["total"].ToString());
                            bill_date = DateTime.Parse(reader["date"].ToString()).ToShortDateString();
                        }
                    dgvInv.Rows.Add(serialNumber.ToString(), humId, inv_id, bill_date, inv_code, bill_amount, false, payment, description, cost_center_id, vType);

                    serialNumber++;
                }

            dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
            StyleInvoiceGrid();
            StylePaymentGridColumns();
            calculateTotal();

            if (cmbPaymentType.Text == "Employee")
            {
                if (id > 0)
                    TickFillCheckNameEmployee();
            }
            else if (cmbPaymentType.Text == "Petty Cash")
            {
                if (id > 0)
                    TickFillCheckNameEmployee();
            }
        }
        private string GenerateNextPaymentCode()
        {
            string newCode = "PV-0001";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(CAST(SUBSTRING(code, 4) AS UNSIGNED)) AS lastCode FROM tbl_payment_voucher"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = "PV-" + code.ToString("D4");
                }
            }

            return newCode;
        }
        private string GenerateNextPaymentId()
        {
            string newCode = "1";

            using (var reader = ClinicDatabase.ExecuteReader("SELECT MAX(id) AS lastCode FROM tbl_payment_voucher"))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                {
                    int code = int.Parse(reader["lastCode"].ToString()) + 1;
                    newCode = code.ToString();
                }
            }

            return newCode;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                if (insertPV())
                {
                    //.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Paid  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                }
            }
            else
            {
                if (updatePV())
                {
                    //.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Updated !  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                }
            }
        }

        private void loadPrint(string text)
        {
            throw new NotImplementedException();
        }

        private bool updatePV()
        {
            if (!chkRequireData())
                return false;

            string GlobalId = ClinicDatabase.ExecuteScalar("select GlobalId from tbl_payment_voucher where id=@id", ClinicDatabase.CreateParameterNew("id", id)).ToString();

            ClinicDatabase.ExecuteScalar(@"UPDATE tbl_payment_voucher SET date=@date, code=@code, type=@type, method=@method, amount=@amount,  
                                                            debit_account_id=@debit_account_id, debit_cost_center_id=@debit_cost_center_id, description=@description,credit_account_id=@credit_account_id,
                                                            credit_cost_center_id=@credit_cost_center_id,bank_id=@bank_id, bank_account_id=@bank_account_id, book_no=@book_no, check_name=@check_name, check_no=@check_no,
                                                            check_date=@check_date, trans_date=@trans_date, trans_name=@trans_name, trans_ref=@trans_ref, modified_by=@modified_by, modified_date=@modified_date,LastModified=@LastModified,SyncStatus = 0 where id=@id",
                                                            ClinicDatabase.CreateParameterNew("id", id),
                                                            ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                                                            ClinicDatabase.CreateParameterNew("code", code),
                                                            ClinicDatabase.CreateParameterNew("type", cmbPaymentType.Text),
                                                            ClinicDatabase.CreateParameterNew("method", cmbMethod.Text),
                                                            ClinicDatabase.CreateParameterNew("amount", txtAmount.Text),
                                                            ClinicDatabase.CreateParameterNew("debit_account_id", cmbDebitAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("debit_cost_center_id", cmbDebitCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("description", txtDescription1.Text),
                                                            ClinicDatabase.CreateParameterNew("credit_account_id", cmbCreditAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("credit_cost_center_id", cmbCreditCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("bank_id", cmbMethod.Text == "Cheque" ? cmbBankName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("bank_account_id", cmbMethod.Text == "Cheque" ? cmbBankAccountName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("book_no", cmbBookNo.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                                                            ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                                                            ClinicDatabase.CreateParameterNew("check_date", cmbMethod.Text == "Cheque" ? (object)dt_check_date.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_date", cmbMethod.Text == "Transfer" ? (object)dtpTransDate.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_name", txtTransName.Text),
                                                            ClinicDatabase.CreateParameterNew("trans_ref", txtTransRef.Text),
                                                            ClinicDatabase.CreateParameterNew("modified_by", -1),
                                                            ClinicDatabase.CreateParameterNew("modified_date", DateTime.Now.Date),
                                                            ClinicDatabase.CreateParameterNew("LastModified", DateTime.Now.Date));

            ClinicDatabase.ExecuteNonQuery("Delete from tbl_payment_voucher_details where payment_id=@id", ClinicDatabase.CreateParameterNew("id", id));
            CommonInsert.DeleteTransactionEntry(id, "PAYMENT");

            if (cmbMethod.Text == "Cheque")
            {
                object result = ClinicDatabase.ExecuteScalar(
                 @"SELECT COUNT(1) FROM tbl_check_details 
                  WHERE pvc_no = @pvc_no AND check_type = @check_type AND check_id = @check_id",
                 ClinicDatabase.CreateParameterNew("pvc_no", id),
                 ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                 ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue)
                 );

                int recordCount = 0;
                if (result != null && result != DBNull.Value)
                    recordCount = Convert.ToInt32(result); // safer than (int)

                if (recordCount > 0)
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                    UPDATE tbl_check_details
                    SET check_no = @check_no, 
                        check_date = @check_date, 
                        check_name = @check_name, 
                        amount = @amount
                    WHERE pvc_no = @pvc_no AND check_type = @check_type AND check_id = @check_id",
                        ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                        ClinicDatabase.CreateParameterNew("check_date", dt_check_date.Value.Date),
                        ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                        ClinicDatabase.CreateParameterNew("amount", decimal.Parse(txtAmount.Text)),
                        ClinicDatabase.CreateParameterNew("pvc_no", id),
                        ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                        ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue)
                    );
                }
                else
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                    INSERT INTO tbl_check_details
                    (date, check_id, check_no, check_date, check_type, pvc_no, check_name, amount, state)
                    VALUES
                    (@date, @check_id, @check_no, @check_date, @check_type, @pvc_no, @check_name, @amount, @state)",
                        ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                        ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue),
                        ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                        ClinicDatabase.CreateParameterNew("check_date", dt_check_date.Value.Date),
                        ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                        ClinicDatabase.CreateParameterNew("pvc_no", id),
                        ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                        ClinicDatabase.CreateParameterNew("amount", decimal.Parse(txtAmount.Text)),
                        ClinicDatabase.CreateParameterNew("state", "New")
                    );
                }
            }
            else if (cmbMethod.Text == "Transfer")
            {
            }
            //insertJournals();
            insertINV(id, GlobalId);
            CommonInsert.DeleteCostCenterTransactionEntry(id.ToString(), "Payment");
            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, txtAmount.Text.ToString(), "0", id.ToString(), "Payment", "Payment Debit Entry", cmbDebitCostCenter.SelectedValue.ToString());
            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, "0", txtAmount.Text.ToString(), id.ToString(), "Payment", "Payment Credit Entry", cmbCreditCostCenter.SelectedValue.ToString());

            ////Utilities.LogAudit(-1, "Update Payment Voucher", "Payment Voucher", id, "Updated Payment Voucher: " + code);
            if (!string.IsNullOrEmpty(filePath))
            {
                AddFileData(id);
            }
            return true;
        }
        private bool insertPV()
        {
            if (!chkRequireData())
                return false;
            code = GenerateNextPaymentCode();
            string GlobalId = Guid.NewGuid().ToString();
            id = (int)decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO tbl_payment_voucher(date, code, type, method, amount,  
                                                            debit_account_id, debit_cost_center_id, description,credit_account_id,
                                                            credit_cost_center_id,bank_id, bank_account_id, book_no, check_name, check_no,
                                                            check_date, trans_date, trans_name, trans_ref, created_by, created_date, state, GlobalId, DeviceId) 
                                                            VALUES (@date, @code, @type, @method, @amount, @debit_account_id, @debit_cost_center_id,
                                                            @description,@credit_account_id, @credit_cost_center_id, @bank_id, @bank_account_id, 
                                                            @book_no, @check_name, @check_no,@check_date, @trans_date, @trans_name, @trans_ref, 
                                                            @created_by, @created_date, 0, @GlobalId, @DeviceId); SELECT LAST_INSERT_ID();",
                                                            ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                                                            ClinicDatabase.CreateParameterNew("code", code),
                                                            ClinicDatabase.CreateParameterNew("type", cmbPaymentType.Text),
                                                            ClinicDatabase.CreateParameterNew("method", cmbMethod.Text),
                                                            ClinicDatabase.CreateParameterNew("amount", txtAmount.Text),
                                                            ClinicDatabase.CreateParameterNew("debit_account_id", cmbDebitAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("debit_cost_center_id", cmbDebitCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("description", txtDescription1.Text),
                                                            ClinicDatabase.CreateParameterNew("credit_account_id", cmbCreditAccountName.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("credit_cost_center_id", cmbCreditCostCenter.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("bank_id", cmbMethod.Text == "Cheque" ? cmbBankName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("bank_account_id", cmbMethod.Text == "Cheque" ? cmbBankAccountName.SelectedValue ?? 0 : 0),
                                                            ClinicDatabase.CreateParameterNew("book_no", cmbBookNo.SelectedValue ?? 0),
                                                            ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                                                            ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                                                            ClinicDatabase.CreateParameterNew("check_date", cmbMethod.Text == "Cheque" ? (object)dt_check_date.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_date", cmbMethod.Text == "Transfer" ? (object)dtpTransDate.Value.Date : DBNull.Value),
                                                            ClinicDatabase.CreateParameterNew("trans_name", txtTransName.Text),
                                                            ClinicDatabase.CreateParameterNew("trans_ref", txtTransRef.Text),
                                                            ClinicDatabase.CreateParameterNew("created_by", -1),
                ClinicDatabase.CreateParameterNew("GlobalId", GlobalId),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName),
                                                            ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date)).ToString());
            if (cmbMethod.Text == "Cheque")
            {
                ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_check_details
                    (date, check_id, check_no, check_date, check_type, pvc_no, check_name, amount, state)
                    VALUES
                    (@date, @check_id, @check_no, @check_date, @check_type, @pvc_no, @check_name, @amount, @state)",
                    ClinicDatabase.CreateParameterNew("date", dtOpen.Value.Date),
                    ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue),
                    ClinicDatabase.CreateParameterNew("check_no", cmbCheckNo.SelectedItem?.ToString()),
                    ClinicDatabase.CreateParameterNew("check_date", dt_check_date.Value.Date),
                    ClinicDatabase.CreateParameterNew("check_type", "Payment"),
                    ClinicDatabase.CreateParameterNew("pvc_no", id),
                    ClinicDatabase.CreateParameterNew("check_name", txtCheckName.Text),
                    ClinicDatabase.CreateParameterNew("amount", decimal.Parse(txtAmount.Text)),
                    ClinicDatabase.CreateParameterNew("state", "New")
                );
            }
            else if (cmbMethod.Text == "Transfer")
            {
            }
            //insertJournals();
            insertINV(id, GlobalId);
            ////Utilities.LogAudit(-1, "Insert Payment Voucher", "Payment Voucher", id, "Inserted Payment Voucher: " + code);

            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, txtAmount.Text.ToString(), "0", id.ToString(), "Payment", "Payment Debit Entry", cmbDebitCostCenter.SelectedValue.ToString());
            CommonInsert.InsertCostCenterTransaction(dtOpen.Value, "0", txtAmount.Text.ToString(), id.ToString(), "Payment", "Payment Credit Entry", cmbCreditCostCenter.SelectedValue.ToString());

            if (!string.IsNullOrEmpty(filePath))
            {
                AddFileData(id);
            }

            return true;
        }
        // Displays validation alerts in the currently selected language.
        // This changes only the alert text and direction; it does not change validation or database logic.
        private void ShowPaymentVoucherAlert(string englishMessage, string arabicMessage)
        {
            bool arabic = IsArabicLanguage();
            string message = arabic ? arabicMessage : englishMessage;
            string caption = arabic ? "تنبيه" : "Validation";

            MessageBox.Show(
                this,
                message,
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1,
                arabic ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : 0);
        }

        private bool chkRequireData()
        {
            if (cmbPaymentType.Text.Trim() == "")
            {
                ShowPaymentVoucherAlert("Please Choose Payment Type.", "يرجى اختيار نوع الدفع.");
                return false;
            }
            if (txtAmount.Text.Trim() == "")
            {
                ShowPaymentVoucherAlert("Please Enter Payment Amount.", "يرجى إدخال مبلغ الدفع.");
                txtAmount.Focus();
                return false;
            }
            if (cmbDebitAccountName.Text.Trim() == "")
            {
                ShowPaymentVoucherAlert("Please Choose Debit Account Name", "يرجى اختيار اسم الحساب المدين.");
                return false;
            }
            if (cmbCreditAccountName.Text.Trim() == "")
            {
                ShowPaymentVoucherAlert("Please Choose Credit Account Name", "يرجى اختيار اسم الحساب الدائن.");
                return false;
            }
            if ((txtAmount.Text == "" || decimal.Parse(txtAmount.Text) == 0) && dgvInv.Rows.Count == 0)
            {
                ShowPaymentVoucherAlert("Enter Amount", "يرجى إدخال المبلغ.");
                txtAmount.Focus();
                return false;
            }
            if (cmbMethod.Text == "Cheque")
            {
                if (cmbBankName.SelectedValue == null || cmbBankAccountName.SelectedValue == null || cmbBookNo.SelectedValue == null)
                {
                    ShowPaymentVoucherAlert("Please Enter Bank Information First", "يرجى إدخال معلومات البنك أولاً.");
                    return false;
                }
            }
            else if (cmbMethod.Text == "Transfer")
            {
                if (txtTransName.Text == "" || txtTransRef.Text == "")
                {
                    ShowPaymentVoucherAlert("Please Enter Transfer Information First", "يرجى إدخال معلومات التحويل أولاً.");
                    return false;
                }
            }
            return true;
        }
        private void insertINV(int pvId, string globalId)
        {
            for (int i = 0; i < dgvInv.Rows.Count; i++)
            {
                if (dgvInv.Rows[i].Cells["Pay"].Value == null ||
                    dgvInv.Rows[i].Cells["Pay"].Value.ToString() == "" ||
                    decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString()) == 0)
                    continue;
                var invId = dgvInv.Rows[i].Cells["invId"].Value?.ToString() != "" ? dgvInv.Rows[i].Cells["invId"].Value?.ToString() : "0";
                var hum = dgvInv.Rows[i].Cells["humId"].Value?.ToString() ?? string.Empty;
                var total = dgvInv.Rows[i].Cells["Total"].Value?.ToString() ?? string.Empty;
                string payment = decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString()).ToString();
                string change = (decimal.Parse(total) - decimal.Parse(payment)).ToString();
                var description = dgvInv.Rows[i].Cells["Description"].Value?.ToString() ?? "";
                var voucherType = dgvInv.Rows[i].Cells["voucherType"].Value?.ToString() ?? "";

                DateTime date = DateTime.Parse(dgvInv.Rows[i].Cells["invDate"].Value.ToString());

                ClinicDatabase.ExecuteNonQuery(@"INSERT INTO tbl_payment_voucher_details(date,payment_id,hum_id, inv_id,inv_code,total, payment, description,voucher_type, GlobalId, DeviceId) 
                                        VALUES (@date,@payment_id,@hum_id ,@inv_id,@inv_code,@total, @payment, @description,@voucher_type, @GlobalId, @DeviceId);",
                                        ClinicDatabase.CreateParameterNew("@payment_id", pvId),
                                        ClinicDatabase.CreateParameterNew("@date", date),
                                        ClinicDatabase.CreateParameterNew("@hum_id", hum),
                                        ClinicDatabase.CreateParameterNew("@total", total),
                                        ClinicDatabase.CreateParameterNew("@inv_code", dgvInv.Rows[i].Cells["InvNo"].Value.ToString()),
                                        ClinicDatabase.CreateParameterNew("@inv_id", int.Parse(invId)),
                                        ClinicDatabase.CreateParameterNew("@payment", payment),
                                        ClinicDatabase.CreateParameterNew("@description", description),
                                        ClinicDatabase.CreateParameterNew("@voucher_type", voucherType),
                ClinicDatabase.CreateParameterNew("GlobalId", globalId),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName)
                                        );

                if (cmbPaymentType.Text == "Employee")
                {
                    if (dgvInv.Rows[i].Cells["voucherType"].Value != null &&
                            dgvInv.Rows[i].Cells["voucherType"].Value.ToString() == "Salary")
                    {
                        ClinicDatabase.ExecuteNonQuery(" UPDATE tbl_attendance_salary SET tbl_attendance_salary.pay = pay+@pay , tbl_attendance_salary.change = @change where id = @id",
                        ClinicDatabase.CreateParameterNew("pay", payment),
                        ClinicDatabase.CreateParameterNew("change", change),
                        ClinicDatabase.CreateParameterNew("id", int.Parse(dgvInv.Rows[i].Cells["invId"].Value.ToString())));
                    }
                    //Employee Loan Payment
                    //else if (dgvInv.Rows[i].Cells["voucherType"].Value?.ToString() == "Employee Loan Payment")
                    //{
                    //    int employeeCode = Convert.ToInt32(dgvInv.Rows[i].Cells["invno"].Value.ToString().Split('-')[0]);
                    //    ClinicDatabase.ExecuteNonQuery(" UPDATE tbl_loan SET pay = pay+@pay , `change` = @change where EmployeeID = @code and loanDate=@date",
                    //    ClinicDatabase.CreateParameterNew("pay", decimal.Parse(payment)),
                    //    ClinicDatabase.CreateParameterNew("change", change),
                    //    ClinicDatabase.CreateParameterNew("code", employeeCode.ToString()),
                    //    ClinicDatabase.CreateParameterNew("date", DateTime.Parse(dgvInv.Rows[i].Cells["invDate"].Value.ToString())));
                    //}
                }
                else if (cmbPaymentType.Text == "Vendor")
                {
                    int currentInvId = dgvInv.Rows[i].Cells["invId"].Value?.ToString() != "" ? int.Parse(dgvInv.Rows[i].Cells["invId"].Value?.ToString()) : 0;
                    decimal newPayment = decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString());

                    // 🟡 Fetch net amount directly from tbl_purchase
                    object netResult = ClinicDatabase.ExecuteScalar(
                        "SELECT net FROM tbl_purchase WHERE id = @id",
                        ClinicDatabase.CreateParameterNew("id", currentInvId)
                    );

                    decimal net = netResult != DBNull.Value ? Convert.ToDecimal(netResult) : 0;

                    // 🟢 Fetch cumulative payment (total paid so far)
                    object result = ClinicDatabase.ExecuteScalar(
                        "SELECT SUM(payment) FROM tbl_payment_voucher_details WHERE inv_id = @id",
                        ClinicDatabase.CreateParameterNew("id", currentInvId)
                    );

                    decimal totalPaid = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                    // 🔵 Calculate remaining amount
                    decimal remaining = net - totalPaid;

                    // 🔴 Update tbl_purchase with correct pay and change
                    ClinicDatabase.ExecuteNonQuery(
                        "UPDATE tbl_purchase SET pay = @pay, `change` = @change WHERE id = @id",
                        ClinicDatabase.CreateParameterNew("pay", totalPaid),
                        ClinicDatabase.CreateParameterNew("change", remaining),
                        ClinicDatabase.CreateParameterNew("id", currentInvId)
                    );

                }
                else if (cmbPaymentType.Text == "Petty Cash")
                {
                    // Petty Cash Pay
                    //if (dgvInv.Rows[i].Cells["voucherType"].Value != null &&
                    //    dgvInv.Rows[i].Cells["voucherType"].Value.ToString() == "Petty Cash Request")
                    if (cbpettycash.Checked)
                    {
                        int requestId = int.Parse(dgvInv.Rows[i].Cells["invId"].Value.ToString());
                        if (cbpettycash.Checked)
                        {
                            ClinicDatabase.ExecuteNonQuery(
                            "UPDATE tbl_petty_cash_request SET pay = pay + @pay, `change` = @change WHERE id = @id",
                            ClinicDatabase.CreateParameterNew("pay", payment),
                            ClinicDatabase.CreateParameterNew("change", change),
                            ClinicDatabase.CreateParameterNew("id", requestId));
                        }
                    }
                }
                insertJournals(cmbPaymentType.Text.ToString(), voucherType, payment, description);
            }
        }
        void insertJournals(string type, string voucherType, string amount, string description)
        {
            string tType = "", humId = "0";
            if (cmbPaymentType.Text == "Vendor")
            {
                tType = CbSubcontractors.Checked ? "Subcontractor Payment" : "Vendor Payment";
                humId = cmbPaymentType.Text == "Vendor" ? cmbVendor.SelectedValue.ToString() : "0";
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                humId = (lstOfEmp.SelectedItem as ListItemModel).Id.ToString();
                if (voucherType == "Salary")
                {
                    tType = "Employee Salary Payment";
                }
                //else if (voucherType == "Employee Loan Payment")
                //{
                //    tType = "Employee Loan Payment";
                //}
            }
            else if (cmbPaymentType.Text == "Petty Cash")
            {
                //if (voucherType == "Petty Cash Request")
                //{
                tType = "Employee Petty Cash Payment";
                //}
            }

            CommonInsert.addTransactionEntry(dtOpen.Value.Date,
                cmbDebitAccountName.SelectedValue.ToString(),
                amount, "0", id.ToString(), humId, tType, "PAYMENT", "Payment Voucher NO. " + code,
                 -1, DateTime.Now.Date, txtPVCode.Text);

            CommonInsert.addTransactionEntry(dtOpen.Value.Date,
                                  cmbCreditAccountName.SelectedValue.ToString(),
                               "0", amount, id.ToString(), "0", tType, "PAYMENT", "Payment Voucher NO. " + code,
                                   -1, DateTime.Now.Date, txtPVCode.Text);
        }

        private void cmbPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvInv.Rows.Clear();
            if (cmbPaymentType.Text == "Vendor")
            {
                dgvInv.Visible = pnlCustomer.Visible = true;
                lstOfEmp.Visible = txtAmount.Enabled = false;
                BindCombos.PopulateVendors(cmbVendor, false, false, VendorType);
                dgvInv.Columns["InvNo"].HeaderText = "Inv No";
                dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "Vendor")))
                    if (reader.Read())
                    {
                        txtDebitCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbDebitAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }

                cbpettycash.Visible = false;
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                pnlCustomer.Visible = txtAmount.Enabled = false;
                lstOfEmp.Visible = true;
                dgvInv.Columns["InvNo"].HeaderText = "Employee Name";
                dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "Accrued Salaries")))
                    if (reader.Read())
                    {
                        txtDebitCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbDebitAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }
                cbpettycash.Visible = false;

            }
            if (cmbPaymentType.Text == "Petty Cash")
            {
                pnlCustomer.Visible = false;
                txtAmount.Enabled = false;
                lstOfEmp.Visible = true;
                cbpettycash.Visible = true;
                cbpettycash.Checked = true;
                cbpettycash.Enabled = true;
                dgvInv.Columns["InvNo"].HeaderText = "Petty Cash Ref";
                // Debit = Cash / Bank if employee returns money
                // OR employee advance account if expense settlement

                using (MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(@"
                        SELECT id,code 
                        FROM tbl_coa_level_4
                        WHERE id = (
                            SELECT account_id
                            FROM tbl_coa_config
                            WHERE category='Employee Advance'
                        )"))
                {
                    if (reader.Read())
                    {
                        txtDebitCode.Text = reader["code"].ToString();
                        cmbDebitAccountName.SelectedValue =
                            Convert.ToInt32(reader["id"]);
                    }
                }
            }
        }
        private void LoadData()
        {
            if (cmbPaymentType.Text == "Vendor")
            {
                decimal customerTotalBalance = 0, customerTotalOB = 0; int counter = 0;
                dgvInv.Rows.Clear();
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT balance as amount FROM tbl_vendor where id = @id
                    ", ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                    if (reader.Read() && reader["amount"].ToString() != "")
                        customerTotalOB = decimal.Parse(reader["amount"].ToString());

                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT c.id, c.date,
                        (SELECT sum(t.payment) FROM tbl_payment_voucher_details t WHERE t.inv_code ='Vendor Opening Balance' AND t.hum_id = c.id) as amount 
                        FROM tbl_vendor c where c.id = @id;",
                          ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                {
                    if (reader.Read() && reader["amount"].ToString() != "")
                        customerTotalBalance = decimal.Parse(reader["amount"].ToString());

                    if (customerTotalOB != customerTotalBalance)
                        dgvInv.Rows.Add(++counter, cmbVendor.SelectedValue, "", DateTime.Parse(reader["date"].ToString()).ToShortDateString(), "Vendor Opening Balance", customerTotalOB - customerTotalBalance, 0, 0, "Vendor Opening Balance");
                }

                using (var reader = ClinicDatabase.ExecuteReader(@"SELECT ROW_NUMBER() OVER (ORDER BY tbl_purchase.date) AS SN, 
                                                                      tbl_purchase.date AS DATE, tbl_purchase.id, tbl_purchase.invoice_id AS 'INV NO', 
                                                                      tbl_purchase.change FROM tbl_purchase INNER JOIN tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id
                                                                      WHERE tbl_purchase.state = 0 AND tbl_purchase.change <> 0 AND tbl_vendor.id = @id
                                                                      GROUP BY tbl_purchase.id, tbl_purchase.date;",
                                                                      ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                {
                    while (reader.Read())
                    {
                        string date = DateTime.Parse(reader["DATE"].ToString()).ToShortDateString();
                        string invoiceId = reader["INV NO"].ToString();
                        string change = reader["change"].ToString();
                        dgvInv.Rows.Add((int.Parse(reader["SN"].ToString()) + counter).ToString(), cmbVendor.SelectedValue, reader["id"].ToString(), date, invoiceId, change, 0, 0, "");
                    }
                }
                dgvInv.Columns["humId"].Visible = dgvInv.Columns["invId"].Visible = false;
                StyleInvoiceGrid();
                StylePaymentGridColumns();
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                //
            }
            else if (cmbPaymentType.Text == "Petty Cash")
            {
                dgvInv.Rows.Clear();
                int empId = 0; // GetSelectedEmployeeId();

                using (var reader = ClinicDatabase.ExecuteReader(@"
                    SELECT id,
                           request_date,
                           request_ref,
                           `change`
                    FROM tbl_petty_cash_request
                    WHERE Petty_cash_name=@empId
                    AND state='Approved'
                    AND `change` > 0",
                    ClinicDatabase.CreateParameterNew("@empId", empId)))
                {
                    int i = 1;

                    while (reader.Read())
                    {
                        dgvInv.Rows.Add(
                            i++,
                            empId,
                            reader["id"],
                            Convert.ToDateTime(reader["request_date"]).ToShortDateString(),
                            reader["request_ref"].ToString(),
                            reader["change"].ToString(),
                            0,
                            0,
                            "Outstanding Petty Cash"
                        );
                    }
                }
            }
        }
        private void StylePaymentGridColumns()
        {
            if (dgvInv == null)
                return;

            if (dgvInv.Columns.Contains("no"))
                dgvInv.Columns["no"].Width = 55;

            if (dgvInv.Columns.Contains("invDate"))
                dgvInv.Columns["invDate"].Width = 105;

            if (dgvInv.Columns.Contains("InvNo"))
                dgvInv.Columns["InvNo"].Width = 130;

            if (dgvInv.Columns.Contains("Total"))
            {
                dgvInv.Columns["Total"].Width = 110;

                dgvInv.Columns["Total"]
                    .DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvInv.Columns.Contains("Pay"))
            {
                dgvInv.Columns["Pay"].Width = 110;

                dgvInv.Columns["Pay"]
                    .DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvInv.Columns.Contains("Description"))
            {
                dgvInv.Columns["Description"]
                    .AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;

                dgvInv.Columns["Description"].MinimumWidth = 250;
            }
        }
        private void cmbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void txtTypeCode_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtTypeCode_Leave(object sender, EventArgs e)
        {

        }

        bool isDebitAccountNameRefreshing = false;
        private void cmbDebitAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDebitAccountName.SelectedValue == null)
            {
                txtDebitCode.Text = "";
                return;
            }

            if (cmbDebitAccountName.SelectedItem != null)
            {
                if (cmbDebitAccountName.SelectedItem is DataRowView row)
                {
                    string code = row["code"].ToString();
                    txtDebitCode.Text = code == "<< Add >>" ? "" : code;
                }
            }
            else
            {
                txtDebitCode.Text = "";
            }


            if (cmbDebitAccountName.Focused)
            {
                if (isDebitAccountNameRefreshing) return;

                if (cmbDebitAccountName.SelectedValue != null &&
                (cmbDebitAccountName.SelectedValue.ToString() == "0" || cmbDebitAccountName.Text == "<< Add >>"))
                {
                    new frmAddAccount().ShowDialog();
                    isDebitAccountNameRefreshing = true;
                    BindCombos.PopulateAllLevel4Account(cmbDebitAccountName, true, true);
                    BindCombos.PopulateAllLevel4Account(cmbCreditAccountName, true);
                    cmbDebitAccountName.SelectedIndex = cmbDebitAccountName.Items.Count - 1;
                    isDebitAccountNameRefreshing = false;
                }
            }
        }
        bool isCreditAccountNameRefreshing = false;
        private void cmbCreditAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCreditAccountName.SelectedValue == null)
            {
                txtCreditAccountCode.Text = "";
                return;
            }

            if (cmbCreditAccountName.SelectedItem != null)
            {
                if (cmbCreditAccountName.SelectedItem is DataRowView row)
                {
                    string code = row["code"].ToString();
                    txtCreditAccountCode.Text = code == "<< Add >>" ? "" : code;
                }
            }
            else
            {
                txtCreditAccountCode.Text = "";
            }

            if (cmbCreditAccountName.Focused)
            {
                if (isCreditAccountNameRefreshing) return;

                if (cmbCreditAccountName.SelectedValue != null &&
                (cmbCreditAccountName.SelectedValue.ToString() == "0" || cmbCreditAccountName.Text == "<< Add >>"))
                {
                    new frmAddAccount().ShowDialog(); // This will wait until form is closed

                    isCreditAccountNameRefreshing = true;
                    BindCombos.PopulateAllLevel4Account(cmbCreditAccountName, true);
                    cmbCreditAccountName.SelectedIndex = cmbCreditAccountName.Items.Count - 1;
                    isCreditAccountNameRefreshing = false;
                }
            }
        }

        private void txtDebitCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
              ClinicDatabase.CreateParameterNew("code", txtDebitCode.Text)))
            {
                if (reader.Read())
                    cmbDebitAccountName.SelectedValue = int.Parse(reader["id"].ToString());
            }

            if (txtDebitCode.Focused)
            {
                string input = txtDebitCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_coa_level_4 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Code = row["code"].ToString(),
                        Name = row["name"].ToString()
                    });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtDebitCode.Parent.PointToScreen(txtDebitCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtDebitCode.Height,
                        txtDebitCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtDebitCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void txtCreditAccountCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                "SELECT * FROM tbl_coa_level_4 WHERE code = @code",
                ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
            {
                if (reader.Read())
                    cmbCreditAccountName.SelectedValue = int.Parse(reader["id"].ToString());
            }

            if (txtCreditAccountCode.Focused)
            {
                string input = txtCreditAccountCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_coa_level_4 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion { Id = Convert.ToInt32(row["id"]), Code = row["code"].ToString(), Name = row["name"].ToString() });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtCreditAccountCode.Parent.PointToScreen(txtCreditAccountCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtCreditAccountCode.Height,
                        txtCreditAccountCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtCreditAccountCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void txtDebitCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code", ClinicDatabase.CreateParameterNew("code", txtDebitCode.Text)))
                if (!reader.Read())
                    cmbDebitAccountName.SelectedIndex = -1;

            BeginInvoke((Action)(() =>
            {
                if (!lstAccountSuggestions.Focused)
                    lstAccountSuggestions.Visible = false;
            }));
        }

        private void txtCreditAccountCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code", ClinicDatabase.CreateParameterNew("code", txtCreditAccountCode.Text)))
                if (!reader.Read())
                    cmbCreditAccountName.SelectedIndex = -1;

            BeginInvoke((Action)(() =>
            {
                if (!lstAccountSuggestions.Focused)
                    lstAccountSuggestions.Visible = false;
            }));
        }
        private void cmbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMethod.Text == "Cash")
            {
                pnlBank.Visible = pnlTrans.Visible = false;
                cmbBankName.SelectedValue = cmbBankAccountName.SelectedValue = -1;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "Default Account For Cash")))
                    if (reader.Read())
                    {
                        txtCreditAccountCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }
            }
            else if (cmbMethod.Text == "Cheque")
            {
                pnlBank.Visible = true;
                pnlTrans.Visible = false;

                CheckDateCheck();
            }
            else if (cmbMethod.Text == "Transfer")
            {
                pnlBank.Visible = false;
                pnlTrans.Visible = true;
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_level_4 WHERE main_id = (SELECT id FROM tbl_coa_level_3 WHERE NAME = 'Banks') LIMIT 1;"))
                    if (reader.Read())
                    {
                        txtCreditAccountCode.Text = reader["code"].ToString();
                        string accountId = reader["id"].ToString();
                        if (!string.IsNullOrEmpty(accountId))
                        {
                            cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                        }
                    }
            }
            else
            {
                pnlTrans.Visible = true;
                pnlBank.Visible = false;
            }
            if (cmbPaymentType.Text == "Vendor")
            {
                AutoFillCheckNameVendor();
            }
            else if (cmbPaymentType.Text == "Employee")
            {
                AutoFillCheckNameEmployee();
            }
        }

        private void dt_check_date_ValueChanged(object sender, EventArgs e)
        {
            if (dt_check_date.Focused)
                CheckDateCheck();
        }

        private void CheckDateCheck()
        {
            if (!pnlBank.Visible)
                return;

            if (cmbMethod.Text == "Cheque")
            {
                if (dt_check_date.Value.Date == DateTime.Now.Date)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_level_4 WHERE main_id = (SELECT id FROM tbl_coa_level_3 WHERE NAME = 'Banks') LIMIT 1;"))
                        if (reader.Read())
                        {
                            txtCreditAccountCode.Text = reader["code"].ToString();
                            string accountId = reader["id"].ToString();
                            if (!string.IsNullOrEmpty(accountId))
                            {
                                cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                            }
                        }
                }
                else
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT id,code FROM tbl_coa_level_4 WHERE id = (select account_id from tbl_coa_config where category=@cat)", ClinicDatabase.CreateParameterNew("@cat", "PDC Payable")))
                        if (reader.Read())
                        {
                            txtCreditAccountCode.Text = reader["code"].ToString();
                            string accountId = reader["id"].ToString();
                            if (!string.IsNullOrEmpty(accountId))
                            {
                                cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                            }
                        }
                }
            }
            else
            {
                dt_check_date.Enabled = false;
                cmbBookNo.Enabled = false;
                cmbBankName.Enabled = false;
                cmbBankAccountName.Enabled = false;
                txtCheckName.Enabled = false;
                cmbCheckNo.Enabled = false;
            }
        }
        bool isBankNameRefreshing = false;
        private void cmbBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBankName.SelectedValue == null)
            {
                txtBankCode.Text = "";
                return;
            }

            if (cmbBankName.Focused)
            {
                if (isBankNameRefreshing) return;
                if (cmbBankName.SelectedValue != null &&
            (cmbBankName.SelectedValue.ToString() == "0" || cmbBankName.Text == "<< Add >>"))
                {
                    //var frm = new frmBankRegister(0);
                    //frm.ShowDialog();
                    //isBankNameRefreshing = true;
                    //BindCombos.PopulateRegisterBanks(cmbBankName, true);
                    //cmbBankName.SelectedIndex = cmbBankName.Items.Count - 1;
                    //isBankNameRefreshing = false;
                }
            }


            if (cmbBankName.SelectedItem != null)
            {
                string displayText = cmbBankName.Text;
                string code = displayText.Split('-')[0].Trim();
                txtBankCode.Text = code == "<< Add >>" ? "" : code;
            }
            else
            {
                txtBankCode.Text = "";
            }

            DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT id,account_name  from tbl_bank_card WHERE bank_id= " + cmbBankName.SelectedValue.ToString());
            cmbBankAccountName.DisplayMember = "account_name";
            cmbBankAccountName.ValueMember = "id";
            cmbBankAccountName.DataSource = dt;
        }

        private void txtBankCode_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_bank where code =@code",
           ClinicDatabase.CreateParameterNew("code", txtBankCode.Text)))
                if (reader.Read())
                    cmbBankName.SelectedValue = int.Parse(reader["id"].ToString());

            if (txtBankCode.Focused)
            {
                string input = txtBankCode.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_bank 
                         WHERE state = 0 and code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion { Id = Convert.ToInt32(row["id"]), Code = row["code"].ToString(), Name = row["name"].ToString() });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtBankCode.Parent.PointToScreen(txtBankCode.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtBankCode.Height,
                        txtBankCode.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtBankCode;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void txtBankCode_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_bank where code =@code",
                    ClinicDatabase.CreateParameterNew("code", txtBankCode.Text)))
                if (!reader.Read())
                    cmbBankName.SelectedIndex = -1;
        }

        private void cmbBankAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBankAccountName.SelectedValue == null)
                return;
            DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT id,chq_book_no  from tbl_cheque WHERE bank_card_id = " + cmbBankAccountName.SelectedValue.ToString());
            cmbBookNo.DisplayMember = "chq_book_no";
            cmbBookNo.ValueMember = "id";
            cmbBookNo.DataSource = dt;
        }

        private void dgvInv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInv.Rows.Count <= 0)
                return;
            if (dgvInv.CurrentRow == null)
                return;

            if (e.ColumnIndex == dgvInv.Columns["chkPay"].Index)
            {
                if (dgvInv.CurrentRow.Cells["chkPay"].Value.ToString() == "True")
                    dgvInv.CurrentRow.Cells["Pay"].Value = dgvInv.CurrentRow.Cells["Total"].Value.ToString();
                else
                    dgvInv.CurrentRow.Cells["Pay"].Value = "0";
            }
            if (e.ColumnIndex == dgvInv.Columns["Pay"].Index)
            {
                if (dgvInv.CurrentRow.Cells["Pay"].Value == null || dgvInv.CurrentRow.Cells["Pay"].Value.ToString() == "" || dgvInv.CurrentRow.Cells["Total"].Value == null)
                    return;

                decimal pay = decimal.Parse(dgvInv.CurrentRow.Cells["Pay"].Value.ToString());
                decimal total = decimal.Parse(dgvInv.CurrentRow.Cells["Total"].Value.ToString());

                if (pay > 0)
                {
                    if (decimal.Parse(dgvInv.CurrentRow.Cells["Pay"].Value.ToString()) > decimal.Parse(dgvInv.CurrentRow.Cells["Total"].Value.ToString()))
                    {
                        var dialog = MessageBox.Show("The amount you entered is grater than the total amount. Do you want to set the payment to the total amount?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialog == DialogResult.No)
                            if (decimal.Parse(dgvInv.CurrentRow.Cells["Pay"].Value.ToString()) > decimal.Parse(dgvInv.CurrentRow.Cells["Total"].Value.ToString()))
                                dgvInv.CurrentRow.Cells["Pay"].Value = dgvInv.CurrentRow.Cells["Total"].Value.ToString();
                    }
                }
                else if (pay < 0)
                {
                    if (pay < total)
                        dgvInv.CurrentRow.Cells["Pay"].Value = dgvInv.CurrentRow.Cells["Total"].Value.ToString();
                }

            }
            calculateTotal();
        }
        private void calculateTotal()
        {
            for (int i = 0; i < dgvInv.Rows.Count; i++)
            {
                if (dgvInv.Rows[i].Cells["Pay"].Value.ToString() == "" || dgvInv.CurrentRow.Cells["Total"].Value.ToString() == "")
                    continue;
                totalAmount += decimal.Parse(dgvInv.Rows[i].Cells["Pay"].Value.ToString());
            }
            txtAmount.Text = totalAmount.ToString();
            totalAmount = 0;
        }
        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if (txtAmount.Text == "")
            {
                txtAmountInWord.Text = "";
                return;
            }
            txtAmountInWord.Text = AmountToWordsHelper.ConvertToSelectedLanguage(decimal.Parse(txtAmount.Text));
        }


        private string GenerateNextChequeCode()
        {
            int code;
            using (var reader = ClinicDatabase.ExecuteReader("select max(check_no) as lastCode from tbl_check_details   where check_type = 'Payment' and check_id =@id ",
                ClinicDatabase.CreateParameterNew("id", cmbBookNo.SelectedValue.ToString())))
            {
                if (reader.Read() && reader["lastCode"] != DBNull.Value)
                    code = int.Parse(reader["lastCode"].ToString()) + 1;
                else
                    code = 0001;
            }
            return code.ToString("D4");
        }

        private void cmbBookNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBookNo.SelectedValue == null)
                return;

            if (cmbBookNo.Focused)
            {
                cmbCheckNo.Items.Clear(); // Clear existing items

                // Get the selected cheque book's start and end leaves
                using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT leaves_start_from, leaves_end_in FROM tbl_cheque WHERE id = @id",
                    ClinicDatabase.CreateParameterNew("id", cmbBookNo.SelectedValue.ToString())))

                    if (reader.Read())
                    {
                        int start = int.Parse(reader["leaves_start_from"].ToString());
                        int end = int.Parse(reader["leaves_end_in"].ToString());
                        reader.Dispose();

                        // Create full list of all check numbers
                        List<int> availableChecks = new List<int>();
                        for (int i = start; i <= end; i++)
                        {
                            availableChecks.Add(i);
                        }

                        // Get used check numbers and remove them
                        using (MySqlDataReader usedReader = ClinicDatabase.ExecuteReader(@"
                            SELECT CAST(check_no AS UNSIGNED) AS check_no 
                            FROM tbl_check_details 
                            WHERE check_type = 'Payment' AND check_id = @check_id",
                            ClinicDatabase.CreateParameterNew("check_id", cmbBookNo.SelectedValue.ToString())))
                            while (usedReader.Read())
                            {
                                if (usedReader["check_no"] != DBNull.Value)
                                {
                                    int used = Convert.ToInt32(usedReader["check_no"]);
                                    availableChecks.Remove(used); // Remove if exists
                                }
                            }

                        // Add remaining (available) checks to combo box
                        foreach (int checkNo in availableChecks)
                        {
                            cmbCheckNo.Items.Add(checkNo.ToString());
                        }
                    }
            }
        }

        bool isVendorRefreshing = false;

        private void cmbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (id > 0)
                return;

            if (cmbVendor.SelectedValue == null)
            {
                txtVendor.Text = "";
                txtDebitCode.Text = "";
                cmbDebitAccountName.SelectedIndex = -1;
                return;
            }

            if (cmbVendor.Focused)
            {
                if (isVendorRefreshing) return;

                if (cmbVendor.SelectedValue != null &&
                (cmbVendor.SelectedValue.ToString() == "0" || cmbVendor.Text == "<< Add >>"))
                {
                    var addVendorForm = new frmViewVendor();
                    addVendorForm.ShowDialog(); // This will wait until form is closed

                    isVendorRefreshing = true;
                    BindCombos.PopulateVendors(cmbVendor, true, true, VendorType);
                    cmbVendor.SelectedIndex = cmbVendor.Items.Count - 1;
                    isVendorRefreshing = false;
                }
            }

            if (cmbVendor.SelectedItem != null)
            {
                if (cmbVendor.SelectedItem is DataRowView row)
                {
                    string code = row["code"].ToString();
                    txtVendor.Text = code == "<< Add >>" ? "" : code;
                }
            }
            else
            {
                txtVendor.Text = "";
            }

            dgvInv.Rows.Clear();
            if (cmbVendor.SelectedValue == null)
                return;
            int counter = 0; decimal vendorTotalBalance = 0, vendorTotalOB = 0;
            string vendorOBDate = DateTime.Now.ToShortDateString(); // Default date
            using (MySqlDataReader obReader = ClinicDatabase.ExecuteReader(@"SELECT balance, date FROM tbl_vendor WHERE id = @id",
                ClinicDatabase.CreateParameterNew("@id", cmbVendor.SelectedValue)))

                if (obReader.Read())
                {
                    if (!string.IsNullOrEmpty(obReader["balance"].ToString()))
                        vendorTotalOB = decimal.Parse(obReader["balance"].ToString());

                    if (!string.IsNullOrEmpty(obReader["date"].ToString()))
                        vendorOBDate = DateTime.Parse(obReader["date"].ToString()).ToShortDateString();
                }
            // 2️⃣ Get how much has been paid from opening balance
            using (MySqlDataReader paidReader = ClinicDatabase.ExecuteReader(@"SELECT 
                            (SELECT SUM(payment) 
                             FROM tbl_payment_voucher_details 
                             WHERE inv_code = 'Vendor Opening Balance' AND hum_id = @id) AS amount",
                ClinicDatabase.CreateParameterNew("@id", cmbVendor.SelectedValue)))

                if (paidReader.Read() && paidReader["amount"].ToString() != "")
                    vendorTotalBalance = decimal.Parse(paidReader["amount"].ToString());

            // 3️⃣ Add opening balance row if there's an unpaid amount
            if (vendorTotalOB != vendorTotalBalance)
            {
                decimal pendingOB = vendorTotalOB - vendorTotalBalance;
                if (pendingOB != 0)
                {
                    dgvInv.Rows.Add(++counter, cmbVendor.SelectedValue, "", vendorOBDate, "Vendor Opening Balance", pendingOB, 0, 0);
                }
            }
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT ROW_NUMBER() OVER (
                ORDER BY tbl_purchase.date) AS SN, 
                 tbl_purchase.date AS DATE, tbl_purchase.id, tbl_purchase.invoice_id AS 'INV NO', 
                 tbl_purchase.change
                FROM tbl_purchase
                INNER JOIN tbl_vendor ON tbl_purchase.vendor_id = tbl_vendor.id
                WHERE tbl_purchase.state = 0 AND tbl_purchase.change <> 0 AND tbl_vendor.id = @id
                GROUP BY tbl_purchase.id, tbl_purchase.date;",
            ClinicDatabase.CreateParameterNew("id", cmbVendor.SelectedValue)))
                while (reader.Read())
                    dgvInv.Rows.Add((int.Parse(reader["SN"].ToString()) + counter).ToString(), cmbVendor.SelectedValue, reader["id"].ToString(),
                    DateTime.Parse(reader["DATE"].ToString()).ToShortDateString(), reader["INV NO"].ToString(), reader["change"].ToString(), 0, 0, "");

            AutoFillCheckNameVendor();
        }

        private void txtVendor_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id,CODE from tbl_vendor where code =@code",
                  ClinicDatabase.CreateParameterNew("code", txtVendor.Text)))
                if (reader.Read())
                    cmbVendor.SelectedValue = int.Parse(reader["id"].ToString());

            if (txtVendor.Focused)
            {
                string input = txtVendor.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    lstAccountSuggestions.Visible = false;
                    return;
                }
                string query = @"SELECT id, code, name FROM tbl_vendor 
                         WHERE code LIKE @search OR name LIKE @search LIMIT 20";

                DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

                lstAccountSuggestions.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    lstAccountSuggestions.Items.Add(new ItemSuggestion { Id = Convert.ToInt32(row["id"]), Code = row["code"].ToString(), Name = row["name"].ToString() });
                }
                if (lstAccountSuggestions.Items.Count > 0)
                {
                    Point locationOnForm = txtVendor.Parent.PointToScreen(txtVendor.Location);
                    Point locationRelativeToForm = this.PointToClient(locationOnForm);

                    lstAccountSuggestions.SetBounds(
                        locationRelativeToForm.X,
                        locationRelativeToForm.Y + txtVendor.Height,
                        txtVendor.Width + 100,
                        120
                    );

                    lstAccountSuggestions.Tag = txtVendor;
                    lstAccountSuggestions.Visible = true;
                    lstAccountSuggestions.BringToFront();
                }
                else
                {
                    lstAccountSuggestions.Visible = false;
                }
            }
        }

        private void dgvInv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dgvInv.Columns[e.ColumnIndex].Name == "chkPay" && e.RowIndex >= 0)
            {
                bool isChecked = Convert.ToBoolean(dgvInv.Rows[e.RowIndex].Cells["chkPay"].Value);

                if (isChecked)
                {
                    dgvInv.Rows[e.RowIndex].Cells["chkPay"].Value = false;
                }
                else
                {
                    dgvInv.Rows[e.RowIndex].Cells["chkPay"].Value = true;
                }
            }
        }

        private void txtVendor_Leave(object sender, EventArgs e)
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id,CODE from tbl_vendor where code =@code",
                ClinicDatabase.CreateParameterNew("code", txtVendor.Text)))
                if (!reader.Read())
                    cmbVendor.SelectedIndex = -1;

            BeginInvoke((Action)(() =>
            {
                if (!lstAccountSuggestions.Focused)
                    lstAccountSuggestions.Visible = false;
            }));
        }
        // Called from ItemCheck (employee selection)
        private void AutoFillCheckNameEmployee()
        {
            foreach (var item in lstOfEmp.CheckedItems)
            {
                if (item is ListItemModel empItem && !string.IsNullOrEmpty(empItem.Name) && empItem.Name.Contains("-"))
                {
                    // Split into code and name
                    string[] parts = empItem.Name.Split('-');
                    if (parts.Length >= 2)
                    {
                        txtCheckName.Text = parts[1].Trim(); // Only the first checked name
                        return;
                    }
                }
            }
            //foreach (var item in lstOfEmp.CheckedItems)
            //{
            //    var rowView = item as DataRowView;
            //    if (rowView != null && rowView["name"].ToString().Contains("-"))
            //    {
            //        txtCheckName.Text = rowView["name"].ToString().Split('-')[1].Trim();
            //        return; // only the first checked name
            //    }
            //}

            txtCheckName.Text = "";
        }
        private void TickFillCheckNameEmployee()
        {
            // Clear previous checks if needed
            for (int i = 0; i < lstOfEmp.Items.Count; i++)
            {
                if (lstOfEmp.Items[i] is ListItemModel empItem)
                {
                    int empId = empItem.Id; // Access the Id directly

                    // Loop through DataGridView rows
                    foreach (DataGridViewRow row in dgvInv.Rows)
                    {
                        if (row.Cells["humId"].Value != null &&
                            int.TryParse(row.Cells["humId"].Value.ToString(), out int humId))
                        {
                            if (humId == empId)
                            {
                                // Check this employee in the list
                                lstOfEmp.SetItemChecked(i, true);
                                break; // No need to continue checking this employee
                            }
                        }
                    }
                }
            }
        }
        private void AutoFillCheckNameVendor()
        {
            if (cmbMethod.Text != "Cheque")
                return;

            if (cmbPaymentType.Text == "Vendor" && cmbVendor.SelectedValue != null)
            {
                using (var reader = ClinicDatabase.ExecuteReader(
                    "SELECT name FROM tbl_vendor WHERE id = @id",
                    ClinicDatabase.CreateParameterNew("@id", cmbVendor.SelectedValue)))
                {
                    if (reader.Read())
                    {
                        txtCheckName.Text = reader["name"].ToString();
                    }
                }
            }
            else
            {
                txtCheckName.Text = "";
            }
        }

        public DataTable COMPANYINFO(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT * FROM tbl_company ;", ClinicDatabase.CreateParameterNew("@id", a1));
        }
        public DataTable PaymentVoucherData(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT 1 sn,id,date,CODE AS RvNo,type,method,amount,
                                                  CASE 
                                                    WHEN tbl_payment_voucher.type = 'Customer' THEN (SELECT code FROM tbl_customer WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Vendor' THEN (SELECT code FROM tbl_vendor WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Employee' THEN (SELECT code FROM tbl_employee WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'General' THEN (SELECT code FROM tbl_coa_level_4 WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    ELSE ''
                                                  END AS reciverCode,
                                                    CASE 
                                                    WHEN tbl_payment_voucher.type = 'Customer' THEN (SELECT name FROM tbl_customer WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Vendor' THEN (SELECT name FROM tbl_vendor WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'Employee' THEN (SELECT name FROM tbl_employee WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    WHEN tbl_payment_voucher.type = 'General' THEN (SELECT name FROM tbl_coa_level_4 WHERE id=(SELECT hum_id FROM tbl_payment_voucher_details WHERE payment_id = tbl_payment_voucher.id LIMIT 1))
                                                    ELSE ''
                                                   END AS reciverName,
                                             (SELECT code FROM tbl_coa_level_4 WHERE id=debit_account_id) DebitCode,
                                            (SELECT NAME FROM tbl_coa_level_4 WHERE id=debit_account_id) debitAccount,
                                            (SELECT NAME FROM tbl_sub_cost_center WHERE id=debit_cost_center_id) debitCostCenter,
                                            (SELECT code FROM tbl_coa_level_4 WHERE id=credit_account_id) CreditCode,
                                            (SELECT NAME FROM tbl_coa_level_4 WHERE id=credit_account_id) creditAccount,
                                            (select name FROM tbl_sub_cost_center WHERE id=credit_cost_center_id) creditCostCenter,
                                            description ,(select name FROM tbl_bank WHERE id = bank_id) bankName,
                                            (select code FROM tbl_bank_card,tbl_bank WHERE tbl_bank_card.id = bank_account_id AND tbl_bank.id = tbl_bank_card.bank_id) BankCode,
                                            (select account_name FROM tbl_bank_card WHERE id = bank_account_id) bankAccount,
                                            check_name,check_no,check_date,trans_date,trans_name,trans_ref 
                                            FROM tbl_payment_voucher where id=@paymentId;", ClinicDatabase.CreateParameterNew("@paymentId", a1));
        }

        public DataTable PaymentDetail(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT ROW_NUMBER() OVER (ORDER BY d.id) AS sn,d.date,d.inv_id,
                                CASE
                                    WHEN i.type = 'Customer' THEN(SELECT NAME FROM tbl_customer WHERE id = d.hum_id)
                                    WHEN i.type = 'Vendor' THEN (SELECT NAME FROM tbl_vendor WHERE id = d.hum_id)
                                    WHEN i.type = 'Employee' THEN (SELECT NAME FROM tbl_employee WHERE id = d.hum_id)
                                    WHEN i.type = 'General' THEN (SELECT NAME FROM tbl_coa_level_4 WHERE id = d.hum_id)
                                    ELSE 'Unregistered'
                                END AS NAME,d.hum_id,d.inv_code,d.total,d.payment,d.description,(SELECT NAME FROM tbl_sub_cost_center WHERE id = d.cost_center_id) AS costCenterName
                            FROM tbl_payment_voucher_details d,tbl_payment_voucher i WHERE d.payment_id = i.id AND i.id= @paymentId;", ClinicDatabase.CreateParameterNew("@paymentId", a1));
        }

        public DataTable VoucherTransaction(string a1)
        {
            return ClinicDatabase.ExecuteDataTable(@"SELECT ROW_NUMBER() OVER (ORDER BY tr.id) AS sn,
                            (SELECT Code FROM tbl_coa_level_4 WHERE id=tr.account_id)as acCode,
                            (SELECT NAME FROM tbl_coa_level_4 WHERE id=tr.account_id)as acName,
                            tr.debit,tr.credit,
                            (SELECT NAME FROM tbl_sub_cost_center WHERE id = (select cost_center_id FROM tbl_payment_voucher_details WHERE payment_id=r.id LIMIT 1)) AS centerName 
                            FROM tbl_transaction tr,tbl_payment_voucher r WHERE tr.transaction_id=r.id and r.id=@paymentId AND tr.TYPE = 'Vendor payment';", ClinicDatabase.CreateParameterNew("@paymentId", a1));
        }

        private void guna2TileButton18_Click(object sender, EventArgs e)
        {
            double amount = 0;

            bool hasValidAmount = double.TryParse(txtAmount.Text, out amount) && amount > 0;

            if (id > 0 || hasValidAmount)
            {
                loadPrint(cmbMethod.Text);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

            if (id == 0)
            {
                if (insertPV())
                {
                    //.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Paid  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                    dgvInv.Rows.Clear();
                    loadPaymentDataDetails();
                    txtPVCode.Text = GenerateNextPaymentCode();
                    this.Close();
                }
            }
            else
            {
                if (updatePV())
                {
                    //.RefreshPaymentVoucher();
                    MessageBox.Show("The Payment Voucher Updated !  ");
                    if (chkPrint.Checked == true)
                    {
                        loadPrint(cmbMethod.Text);
                    }
                    dgvInv.Rows.Clear();
                    loadPaymentDataDetails();
                    txtPVCode.Text = GenerateNextPaymentCode();
                    this.Close();
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
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

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtId.Text); // Utilities.GetVoucherIdFromCode(txtPVCode.Text);
            if (currentId == null || currentId <= 1)
                return;

            currentId = currentId - 1;
            txtId.Text = currentId.ToString();
            if (currentId <= 0)
            {
                clear();
                MessageBox.Show("No previous records.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string query = "select id from tbl_payment_voucher where state = 0 and id =@id";
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("@id", currentId)))
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindVoucher();
                }
                else
                {
                    clear();
                    MessageBox.Show("No previous record found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int? currentId = Convert.ToInt32(txtId.Text); // Utilities.GetVoucherIdFromCode(txtPVCode.Text);
            if (currentId is null) return;

            currentId = currentId + 1;
            txtId.Text = currentId.ToString();
            string query = "SELECT id FROM tbl_payment_voucher WHERE state = 0 AND id =@id";
            using (var reader = ClinicDatabase.ExecuteReader(query, ClinicDatabase.CreateParameterNew("@id", currentId)))
            {
                if (reader.Read())
                {
                    id = int.Parse(reader["id"].ToString());
                    BindVoucher();
                }
                else
                {
                    clear();
                    MessageBox.Show("No next record found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtPVCode.Text = GenerateNextPaymentCode();
            clear();
        }
        private void clear()
        {
            dgvInv.Rows.Clear();
            defaultCustomerId = 0;
            totalAmount = 0;
            id = 0;
        }

        private void BtnSaveNew_Click(object sender, EventArgs e)
        {
            btnSave.PerformClick();
            dgvInv.Rows.Clear();
            txtPVCode.Text = GenerateNextPaymentCode();
            loadPaymentDataDetails();
            clear();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            //ClinicDatabase.ExecuteNonQuery("UPDATE tbl_payment_voucher SET state = -1 WHERE id = @id ",
            //                              ClinicDatabase.CreateParameterNew("id", id.ToString()));

            // 1. Read all records before deletion
            DataTable dtVoucher = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_payment_voucher WHERE id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            DataTable dtDetails = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_payment_voucher_details WHERE payment_id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            DataTable dtTransaction = ClinicDatabase.ExecuteDataTable("SELECT * FROM tbl_transaction WHERE transaction_id = @id AND t_type='PAYMENT'",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            // 2. Insert backups into tbl_deleted_records
            foreach (DataRow row in dtVoucher.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                    ClinicDatabase.CreateParameterNew("table", "tbl_payment_voucher"),
                    ClinicDatabase.CreateParameterNew("data", "Newtonsoft.Json.JsonConvert.SerializeObject(row)"),
                    ClinicDatabase.CreateParameterNew("user", "-1")
                );
            }

            foreach (DataRow row in dtDetails.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                    ClinicDatabase.CreateParameterNew("table", "tbl_payment_voucher_details"),
                    ClinicDatabase.CreateParameterNew("data", "Newtonsoft.Json.JsonConvert.SerializeObject(row)"),
                    ClinicDatabase.CreateParameterNew("user", "-1")
                );
            }

            foreach (DataRow row in dtTransaction.Rows)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "INSERT INTO tbl_deleted_records (table_name, record_data, deleted_by) VALUES (@table, @data, @user)",
                    ClinicDatabase.CreateParameterNew("table", "tbl_transaction"),
                    ClinicDatabase.CreateParameterNew("data", "Newtonsoft.Json.JsonConvert.SerializeObject(row)"),
                    ClinicDatabase.CreateParameterNew("user", "-1")
                );
            }

            // 3. Now delete records permanently
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_transaction WHERE transaction_id = @id AND t_type='PAYMENT'",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_payment_voucher_details WHERE payment_id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_payment_voucher WHERE id = @id",
                ClinicDatabase.CreateParameterNew("id", id.ToString()));

            ////Utilities.LogAudit(-1, "Payment Voucher Permanently Deleted", "Payment Voucher", id, "Deleted Payment Voucher with ID: " + id);
            clear();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            id = 0;
        }

        private void guna2TileButton25_Click(object sender, EventArgs e)
        {

        }

        private void guna2TileButton26_Click(object sender, EventArgs e)
        {

        }

        bool isDebitCostCenterRefreshing = false;

        private void cmbDebitCostCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cmbDebitCostCenter.Focused)
                return;

            if (isDebitCostCenterRefreshing) return;
            if (cmbDebitCostCenter.SelectedValue != null &&
        (cmbDebitCostCenter.SelectedValue.ToString() == "0" || cmbDebitCostCenter.Text == "<< Add >>"))
            {
                var frm = new frmCostCenter(0);
                frm.ShowDialog();

                isDebitCostCenterRefreshing = true;
                BindCombos.PopulateCostCenter(cmbDebitCostCenter, true);
                cmbDebitCostCenter.SelectedIndex = cmbDebitCostCenter.Items.Count - 1;
                isDebitCostCenterRefreshing = false;
            }
        }

        bool isCreditCostCenterRefreshing = false;

        private void cmbCreditCostCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cmbCreditCostCenter.Focused)
                return;
            if (isCreditCostCenterRefreshing) return;

            if (cmbCreditCostCenter.SelectedValue != null &&
        (cmbCreditCostCenter.SelectedValue.ToString() == "0" || cmbCreditCostCenter.Text == "<< Add >>"))
            {
                var frm = new frmCostCenter(0);
                frm.ShowDialog();
                isCreditCostCenterRefreshing = true;
                BindCombos.PopulateCostCenter(cmbCreditCostCenter, true);
                cmbCreditCostCenter.SelectedIndex = cmbCreditCostCenter.Items.Count - 1;
                isCreditCostCenterRefreshing = false;
            }
        }


        //private bool _suppressTextChanged = false;
        //private bool _isSelectingFromList = false;

        //private void ShowAccountSuggestions0(TextBox tb)
        //{
        //    string input = tb.Text.Trim();

        //    if (string.IsNullOrEmpty(input))
        //    {
        //        lstAccountSuggestions.Visible = false;
        //        return;
        //    }

        //    string query = @"SELECT code, name FROM tbl_coa_level_4 
        //             WHERE code LIKE @search OR name LIKE @search LIMIT 20";

        //    DataTable dt = ClinicDatabase.ExecuteDataTable(query, ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

        //    lstAccountSuggestions.Items.Clear();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        lstAccountSuggestions.Items.Add(new ItemSuggestion
        //{
        //    Code = row["code"].ToString(),
        //    Name = row["name"].ToString()
        //});
        //    }

        //    if (lstAccountSuggestions.Items.Count > 0)
        //    {
        //        lstAccountSuggestions.Visible = true;

        //        // Position the listbox just below the textbox
        //        Point locationOnForm = tb.Parent.PointToScreen(tb.Location);
        //        Point locationRelativeToForm = this.PointToClient(locationOnForm);

        //        lstAccountSuggestions.SetBounds(
        //            locationRelativeToForm.X,
        //            locationRelativeToForm.Y + tb.Height,
        //            tb.Width + 100,
        //            120
        //        );

        //        lstAccountSuggestions.BringToFront();

        //        // Optional: store current TextBox for selection use
        //        lstAccountSuggestions.Tag = tb;
        //    }
        //    else
        //    {
        //        lstAccountSuggestions.Visible = false;
        //    }
        //}

        //private void ShowAccountSuggestions1(Guna2TextBox tb)
        //{
        //    string input = tb.Text.Trim();

        //    if (string.IsNullOrEmpty(input))
        //    {
        //        lstAccountSuggestions.Visible = false;
        //        return;
        //    }

        //    string query = @"SELECT code, name FROM tbl_coa_level_4 
        //             WHERE code LIKE @search OR name LIKE @search LIMIT 20";

        //    DataTable dt = ClinicDatabase.ExecuteDataTable(query,
        //        ClinicDatabase.CreateParameterNew("@search", "%" + input + "%"));

        //    lstAccountSuggestions.Items.Clear();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        lstAccountSuggestions.Items.Add(new ItemSuggestion
        //{
        //    Code = row["code"].ToString(),
        //    Name = row["name"].ToString()
        //});
        //    }

        //    if (lstAccountSuggestions.Items.Count > 0)
        //    {
        //        Point locationOnForm = tb.Parent.PointToScreen(tb.Location);
        //        Point locationRelativeToForm = this.PointToClient(locationOnForm);

        //        lstAccountSuggestions.SetBounds(
        //            locationRelativeToForm.X,
        //            locationRelativeToForm.Y + tb.Height,
        //            tb.Width + 100,
        //            120
        //        );

        //        lstAccountSuggestions.Tag = tb;
        //        lstAccountSuggestions.Visible = true;
        //        lstAccountSuggestions.BringToFront();
        //        // ❌ Do NOT call Focus here
        //    }
        //    else
        //    {
        //        lstAccountSuggestions.Visible = false;
        //    }
        //}

        //private void lstAccountSuggestions_Click(object sender, EventArgs e)
        //{
        //    if (lstAccountSuggestions.SelectedItem != null && lstAccountSuggestions.Tag is TextBox tb)
        //    {
        //        tb.Text = lstAccountSuggestions.SelectedItem.ToString().Split('-')[0].Trim(); // Set only the code part
        //        lstAccountSuggestions.Visible = false;
        //        tb.Focus(); // bring focus back to textbox
        //    }
        //    else if(lstAccountSuggestions.SelectedItem != null && lstAccountSuggestions.Tag is Guna2TextBox tb1)
        //    {
        //        tb1.Text = lstAccountSuggestions.SelectedItem.ToString().Split('-')[0].Trim(); // Set only the code part
        //        lstAccountSuggestions.Visible = false;
        //        tb1.Focus(); // bring focus back to textbox
        //    }
        //}

        private void lstAccountSuggestions_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    lstAccountSuggestions_Click(sender, e);
            //    e.Handled = true;
            //}

            //int index = lstAccountSuggestions.IndexFromPoint(e.Location);
            //if (index != ListBox.NoMatches)
            //{
            //    lstAccountSuggestions.SelectedIndex = index;
            //    string selectedCode = lstAccountSuggestions.SelectedItem.ToString().Split('-')[0].Trim();

            //    if (lstAccountSuggestions.Tag is Guna2TextBox tb)
            //    {
            //        _isSelectingFromList = true;
            //        _suppressTextChanged = true;
            //        tb.Text = selectedCode;
            //        _suppressTextChanged = false;
            //        _isSelectingFromList = false;
            //    }

            //    lstAccountSuggestions.Visible = false;
            //}
        }

        private void lstAccountSuggestions_Click(object sender, EventArgs e)
        {
            if (lstAccountSuggestions.SelectedItem != null && lstAccountSuggestions.Tag is Guna2TextBox targetTextBox)
            {
                string selected = lstAccountSuggestions.SelectedItem.ToString();
                string selectedCode = "";// selected.Split('-')[0].Trim();
                if (lstAccountSuggestions.SelectedItem is ItemSuggestion item)
                {
                    selectedCode = item.Code;
                }

                targetTextBox.Text = selectedCode;
                lstAccountSuggestions.Visible = false;

                // Trigger TextChanged manually if needed
                targetTextBox.Focus();
                targetTextBox.SelectionStart = targetTextBox.Text.Length;

                // Example of post-selection logic:
                if (targetTextBox == txtDebitCode)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                        ClinicDatabase.CreateParameterNew("code", selectedCode)))
                        if (reader.Read())
                            cmbDebitAccountName.SelectedValue = int.Parse(reader["id"].ToString());
                }
                else if (targetTextBox == txtCreditAccountCode)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_coa_level_4 where code =@code",
                        ClinicDatabase.CreateParameterNew("code", selectedCode)))
                        if (reader.Read())
                            cmbCreditAccountName.SelectedValue = int.Parse(reader["id"].ToString());
                }
                else if (targetTextBox == txtVendor)
                {
                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select id from tbl_vendor where code =@code",
                        ClinicDatabase.CreateParameterNew("code", selectedCode)))
                        if (reader.Read())
                            cmbVendor.SelectedValue = int.Parse(reader["id"].ToString());
                }
            }

            //if (lstAccountSuggestions.SelectedItem != null)
            //{
            //    txtDebitCode.Text = lstAccountSuggestions.SelectedItem.ToString();
            //    lstAccountSuggestions.Visible = false;

            //    MessageBox.Show("You selected: " + txtDebitCode.Text);
            //}
        }

        private void cbpettycash_CheckedChanged(object sender, EventArgs e)
        {
            if (cbpettycash.Checked == true)
            {
                //lspettycashemployee.Visible = true;
                //lstOfEmp.Visible = true;
                //else if (cmbMethod.Text == "Petty Cash")
                //{
                //pnlBank.Visible = false;
                //pnlTrans.Visible = true;
                //using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("SELECT * FROM tbl_coa_level_4 WHERE main_id = (SELECT id FROM tbl_coa_level_3 WHERE NAME = 'Petty') LIMIT 1;"))
                //    if (reader.Read())
                //    {
                //        txtCreditAccountCode.Text = reader["code"].ToString();
                //        string accountId = reader["id"].ToString();
                //        if (!string.IsNullOrEmpty(accountId))
                //        {
                //            cmbCreditAccountName.SelectedValue = int.Parse(accountId);
                //        }
                //    }
                //}
            }
            else
            {
                //lspettycashemployee.Visible = false;
                lstOfEmp.Visible = false;
            }

        }

        private void CbSubcontractors_CheckedChanged(object sender, EventArgs e)
        {
            isSubContractors = CbSubcontractors.Checked;
            VendorType = isSubContractors ? "Subcontractor" : "Vendor";
            if (cmbPaymentType.Text == "Vendor" && cmbVendor.SelectedValue != null)
            {
                BindCombos.PopulateVendors(cmbVendor, false, false, VendorType);
            }
        }

        private void dgvInv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvInv.CurrentCell.ColumnIndex == dgvInv.Columns["Pay"].Index)
            {
                TextBox txt = e.Control as TextBox;
                if (txt != null)
                {
                    txt.KeyPress -= Txt_KeyPress;
                    txt.KeyPress += Txt_KeyPress;
                }
            }
        }

        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (backspace, delete, etc.)
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.')
            {
                e.Handled = true; // Block the input
            }

            // Allow only one decimal point
            TextBox txt = sender as TextBox;
            if (e.KeyChar == '.' && txt.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void lstOfEmp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lstOfEmp.Focused)
                return; // Ignore changes not made by user interaction

            if (lstOfEmp.SelectedIndex == -1)
            {
                txtCheckName.Text = "";
                return;
            }

            txtCheckName.Text = lstOfEmp.Text;

        }

        private void lstOfEmp_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!lstOfEmp.Focused)
                return;

            ListItemModel empItem = lstOfEmp.Items[e.Index] as ListItemModel;
            if (empItem == null)
                return;

            string fullName = empItem.Name ?? "";
            string[] nameParts = fullName.Split('-');
            if (nameParts.Length < 2) return;

            string empCode = nameParts[0].Trim();
            string empName = nameParts[1].Trim();
            int empId = empItem.Id;

            if (e.NewValue == CheckState.Checked)
            {

                var _idd = "0";


                /****Petty Cash****/
                if (cbpettycash.Visible && cbpettycash.Enabled)
                {
                    using (MySqlDataReader pReader1 = ClinicDatabase.ExecuteReader(@"SELECT pcr.id, pcr.`change`,request_date,pcr.amount AS total FROM tbl_petty_cash_request pcr 
                                                 INNER JOIN tbl_employee te ON pcr.Petty_cash_name = te.id
                                                 WHERE te.id = " + empId + " AND pcr.state = 'Approved'"
                                             ))

                        while (pReader1.Read())
                        {
                            decimal total = 0;
                            if (pReader1["total"] != DBNull.Value &&
                                decimal.TryParse(pReader1["total"].ToString(), out total) && total > 0)
                            {
                                dgvInv.Rows.Add(dgvInv.Rows.Count + 1, empId, pReader1["id"].ToString(), pReader1["request_date"].ToString(),
                                                fullName.ToString(), Utilities.FormatDecimal(total),
                                                0, 0, "", "Petty Cash Request");
                            }
                        }
                }
                else
                {

                    using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(
                        @"SELECT ROW_NUMBER() OVER (ORDER BY tbl_attendance_salary.id) AS SN,  
                          tbl_attendance_salary.* 
                          FROM tbl_attendance_salary 
                          WHERE emp_code = @code AND `change` <> 0",
                        ClinicDatabase.CreateParameterNew("code", empCode)))
                    {
                        while (reader.Read())
                        {
                            decimal netSalary = Convert.ToDecimal(reader["net_salary"]);
                            decimal totalLoan = Convert.ToDecimal(reader["total_loan"]);
                            decimal paid = Convert.ToDecimal(reader["Pay"]);
                            decimal finalAmount = netSalary - totalLoan - paid;

                            dgvInv.Rows.Add(
                                reader["SN"].ToString(),     // Serial number
                                empId,                        // Employee ID
                                reader["id"].ToString(),      // Salary record ID
                                reader["date"].ToString(),    // Date
                                empName,                      // Employee Name
                                Utilities.FormatDecimal(finalAmount), // Remaining Salary
                                0,
                                0,
                                "",
                                "Salary"
                            );

                            _idd = reader["SN"].ToString();
                        }
                    }

                    // Leave Salary
                    //MySqlDataReader reade = ClinicDatabase.ExecuteReader(
                    //     "SELECT SUM(tl.credit) - SUM(tl.debit) AS total " +
                    //     "FROM tbl_leave_salary tl " +
                    //     "INNER JOIN tbl_employee te ON tl.code = te.code " +
                    //     "WHERE te.id = " + empId);


                    //if (reade.Read())
                    //{
                    //    decimal total = 0;
                    //    if (reade["total"] != DBNull.Value &&
                    //        decimal.TryParse(reade["total"].ToString(), out total) && total > 0)
                    //    {
                    //        dgvInv.Rows.Add(no, empId, _idd, DateTime.Now.Date, rowView["name"].ToString(),
                    //                        total.ToString("F2"), false, "", "Leave Salary");
                    //    }
                    //}
                    //            MySqlDataReader Ereade = ClinicDatabase.ExecuteReader(
                    //"SELECT SUM(tl.credit) - SUM(tl.debit) AS total " +
                    //"FROM tbl_end_of_service tl " +
                    //"INNER JOIN tbl_employee te ON tl.code = te.code " +
                    //"WHERE te.id = " + empId);

                    //            if (Ereade.Read())
                    //            {
                    //                decimal total = 0;
                    //                if (Ereade["total"] != DBNull.Value &&
                    //                    decimal.TryParse(Ereade["total"].ToString(), out total) && total > 0)
                    //                {
                    //                    dgvInv.Rows.Add(no, empId, _idd, DateTime.Now.Date, rowView["name"].ToString(),
                    //                                    total.ToString("F2"), false, "", "End Of Service");
                    //                }
                    //            }


                    // Employee Loan
                    //        MySqlDataReader pReader = ClinicDatabase.ExecuteReader(@"
                    //SELECT 
                    //ROW_NUMBER() OVER (ORDER BY loanDate) AS SN, 
                    //id, LoanDate, RequestAmount AS Amount, `Change` 
                    //FROM tbl_loan 
                    //WHERE EmployeeID = @code AND `change` > 0 
                    //LIMIT 1;",
                    //        ClinicDatabase.CreateParameterNew("code", empCode));

                    //        while (pReader.Read())
                    //        {
                    //            dgvInv.Rows.Add(
                    //                pReader["SN"].ToString(),
                    //                empId,
                    //                pReader["id"].ToString(),                       // invId (Loan ID)
                    //                pReader["LoanDate"].ToString(),                 // invDate
                    //                fullName,                     // InvNo (e.g. "30001 - John")
                    //                Utilities.FormatDecimal(pReader["Change"]),     // Total (Amount to pay)
                    //                false,                                          // humId
                    //                "",                                             // Pay (you can set = Change if needed)
                    //                "",
                    //                "Employee Loan Payment"// Description
                    //            );

                    //            _idd = pReader["SN"].ToString();
                    //        }
                    //        pReader.Dispose();
                }
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                for (int i = dgvInv.Rows.Count - 1; i >= 0; i--)
                    if (dgvInv.Rows[i].Cells["humId"].Value != null && int.Parse(dgvInv.Rows[i].Cells["humId"].Value.ToString()) == empId)
                        dgvInv.Rows.RemoveAt(i);

                calculateTotal();
            }
            dgvInv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.BeginInvoke((MethodInvoker)delegate
            {
                AutoFillCheckNameEmployee();
            });
        }
        static string filePath = "";
        private void BtnAttach_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
            }
        }
        private void AddFileData(int fId)
        {
            AttachmentManager.SaveAttachment(
                    "PAYMENT VOUCHER",
                    fId,
                    openFileDialog1.FileName
                );
        }
    }
}