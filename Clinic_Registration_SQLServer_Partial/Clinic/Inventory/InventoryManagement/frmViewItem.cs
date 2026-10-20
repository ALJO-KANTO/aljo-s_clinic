using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;
namespace YamyProject
{
    public partial class frmViewItem : Form
    {

        int id; string itemCode;
        public List<int> unitIds = new List<int>();
        public List<decimal> unitFactors = new List<decimal>();
        public List<string> unitNames = new List<string>();
        private EventHandler itemCategoryUpdatedHandler;
        private EventHandler itemUnitUpdatedHandler;
        private EventHandler warehouseUpdatedHandler;

        // Item image state
        private string _selectedImageSourcePath = string.Empty;
        private string _currentImageDbPath = string.Empty;
        private bool _removeImageRequested = false;
        private byte[] _selectedImageBytes = null;
        private string _selectedImageFileName = string.Empty;
        private System.Drawing.Image _defaultUploadImage;
        private System.Windows.Forms.Timer _inventoryLanguageTimer;
        private string _lastInventoryLanguage = string.Empty;

        public frmViewItem(int id = 0)
        {
            InitializeComponent();
            InitializeReferenceIcons();
            ApplyModernInputTypography(this);
            this.Resize += frmViewItem_Resize;
            LayoutModernInventoryPage();


            if (picItemPlaceholder != null && picItemPlaceholder.Image != null)
            {
                _defaultUploadImage =
                    new System.Drawing.Bitmap(picItemPlaceholder.Image);
            }

            SetImageUploadOverlay(true);

            this.id = id;
            this.Text = id != 0 ? "Inventory - Edit Item" : "Inventory - New Item";
            ApplyInventoryLanguageLayout();
            _lastInventoryLanguage = GetInventoryLanguage();

            // Re-apply captions and RTL/LTR properties when the application language changes.
            _inventoryLanguageTimer = new System.Windows.Forms.Timer();
            _inventoryLanguageTimer.Interval = 400;
            _inventoryLanguageTimer.Tick += (sender, args) =>
            {
                string currentLanguage = GetInventoryLanguage();
                if (!string.Equals(currentLanguage, _lastInventoryLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    _lastInventoryLanguage = currentLanguage;
                    ApplyInventoryLanguageLayout();
                }
            };
            _inventoryLanguageTimer.Start();
            this.FormClosed += (sender, args) =>
            {
                if (_inventoryLanguageTimer != null)
                {
                    _inventoryLanguageTimer.Stop();
                    _inventoryLanguageTimer.Dispose();
                    _inventoryLanguageTimer = null;
                }
            };

            itemCategoryUpdatedHandler = (sender, args) => setComboBoxCategory();
            itemUnitUpdatedHandler = (sender, args) => BindCombos.PopulateItemUnit(cmbMainUnit);
            warehouseUpdatedHandler = (sender, args) => BindCombos.PopulateWarehouse(cmbWarehouse);
            //.ItemCategory += itemCategoryUpdatedHandler;
            //.ItemUnit += itemUnitUpdatedHandler;
            //.wareHouse += warehouseUpdatedHandler;

            //btnSave.Text = id != 0 ? "Update" : btnSave.Text;
            dtStart.Value = DateTime.Now;
            UpdateInventoryHeaderText();
        }
        private void frmViewItem_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.ItemCategory -= itemCategoryUpdatedHandler;
            //.ItemUnit -= itemUnitUpdatedHandler;
            //.wareHouse -= warehouseUpdatedHandler;

            if (_defaultUploadImage != null)
            {
                _defaultUploadImage.Dispose();
                _defaultUploadImage = null;
            }
        }
        private string GetInventoryLanguage()
        {
            try
            {
                return Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";
            }
            catch
            {
                return "en";
            }
        }

        private bool IsInventoryArabic()
        {
            string language = GetInventoryLanguage();
            return language.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
                || language.Equals("Arabic", StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateInventoryHeaderText()
        {
            bool arabic = IsInventoryArabic();
            string title = id != 0
                ? (arabic ? "المخزون - تعديل صنف" : "Inventory - Edit Item")
                : (arabic ? "المخزون - صنف جديد" : "Inventory - New Item");

            this.Text = title;
            if (headerUC1 != null)
                headerUC1.FormText = title;
        }

        private void ApplyInventoryArabicText()
        {
            // =========================================================
            // Translate by current English text.
            // This means we don't need to know every label's variable name.
            // =========================================================

            Dictionary<string, string> ar =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase)
                {
            // Page
            { "New Inventory Item", "صنف مخزون جديد" },
            { "Edit Inventory Item", "تعديل صنف المخزون" },
            { "Add a new item to your inventory", "إضافة صنف جديد إلى المخزون" },
            { "Back to Items ←", "العودة إلى الأصناف ←" },
            { "Back to Items", "العودة إلى الأصناف" },

            // Top
            { "Warehouse", "المستودع" },
            { "Category", "الفئة" },
            { "Item Type", "نوع الصنف" },
            { "Type", "النوع" },
            { "New +", "جديد +" },

            // Item details
            { "Item Details", "تفاصيل الصنف" },
            { "Item Code", "كود الصنف" },
            { "Item Name", "اسم الصنف" },
            { "School Fee Type", "نوع الرسوم المدرسية" },
            { "Grade", "الصف الدراسي" },
            { "Grade / Standard", "الصف / المرحلة" },
            { "Add Grade +", "إضافة صف +" },

            // Image
            { "Item Image", "صورة الصنف" },
            { "Browse", "استعراض" },
            { "Remove", "إزالة" },

            // Pricing
            { "Pricing & Quantity", "الأسعار والكميات" },
            { "Pricing  Quantity", "الأسعار والكميات" },
            { "Opening Quantity", "الكمية الافتتاحية" },
            { "Opening Qty", "الكمية الافتتاحية" },
            { "As of Date", "اعتباراً من" },
            { "Cost Price", "سعر التكلفة" },
            { "Sales Price", "سعر البيع" },
            { "Total Value", "القيمة الإجمالية" },
            { "Active and available in transactions",
              "نشط ومتاح في المعاملات" },

            // Valuation
            { "Inventory Valuation Method", "طريقة تقييم المخزون" },
            { "Average (AVG)", "المتوسط" },
            { "FIFO", "الوارد أولاً يصرف أولاً" },
            { "LIFO", "الوارد أخيراً يصرف أولاً" },

            // Additional information
            { "Additional Information", "معلومات إضافية" },
            { "Barcode", "الباركود" },
            { "Main Unit", "الوحدة الرئيسية" },
            { "Add Unit +", "إضافة وحدة +" },
            { "Min. Amount", "الحد الأدنى للكمية" },
            { "Max. Amount", "الحد الأقصى للكمية" },

            // Accounting
            { "Accounting Information", "المعلومات المحاسبية" },
            { "COGS Account", "حساب تكلفة البضاعة المباعة" },
            { "Income Account", "حساب الدخل" },
            { "Asset Account", "حساب الأصول" },
            { "Vendor Account", "حساب المورد" },

            // Assembly
            { "Assembly", "التجميع" },
            { "Item Assembly", "تجميع الصنف" },
            { "Quantity", "الكمية" },

            // Buttons
            { "Save", "حفظ" },
            { "Save New", "حفظ وجديد" },
            { "Save_New", "حفظ وجديد" },
            { "Clear", "مسح" },
            { "Close", "إغلاق" },

            // Generic
            { "None", "لا يوجد" }
                };

            TranslateInventoryControls(this, ar);

            // Explicitly enforce the captions marked in the Arabic UI screenshot.
            // These controls can contain icon glyphs or designer-specific text.
            SetInventoryCaption(this, "Add Grade", "إضافة صف");
            SetInventoryCaption(this, "Add Unit", "إضافة وحدة");
            SetInventoryCaption(this, "Browse", "استعراض");
            SetInventoryCaption(this, "Save_New", "حفظ وجديد");
            SetInventoryCaption(this, "Save New", "حفظ وجديد");
            SetInventoryCaption(this, "Clear", "مسح");
            SetInventoryCaption(this, "Close", "إغلاق");
            SetInventoryCaption(this, "Remove", "إزالة");

            // Set the complete caption explicitly: the designer text "Save_New"
            // must not become a mixed caption such as "New_حفظ".
            if (btnSaveNew != null)
                btnSaveNew.Text = "حفظ وجديد";
            if (btnSave != null)
                btnSave.Text = id != 0 ? "تحديث" : "حفظ";
            if (btnClear != null)
                btnClear.Text = "مسح";
            if (btnClose != null)
                btnClose.Text = "إغلاق";

            // Placeholder translations
            SetArabicPlaceholder(txtName, "اسم الصنف");
            SetArabicPlaceholder(txtBarcode, "أدخل الباركود");

            // Form title
            this.Text =
                id != 0
                    ? "المخزون - تعديل صنف"
                    : "المخزون - صنف جديد";

            if (headerUC1 != null)
                headerUC1.FormText = this.Text;
        }
        private void SetInventoryCaption(Control parent, string english, string arabic)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                string current = (control.Text ?? string.Empty).Trim();
                if (current.IndexOf(english, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    control.Text = current.Replace(english, arabic);
                    if (control is Label label)
                    {
                        label.RightToLeft = RightToLeft.Yes;
                        label.TextAlign = ContentAlignment.MiddleRight;
                    }
                    else if (control is LinkLabel link)
                    {
                        link.RightToLeft = RightToLeft.Yes;
                        link.TextAlign = ContentAlignment.MiddleRight;
                    }
                    else
                    {
                        control.RightToLeft = RightToLeft.Yes;
                    }
                }

                if (control.HasChildren)
                    SetInventoryCaption(control, english, arabic);
            }
        }

        private void SetArabicPlaceholder(
    Guna.UI2.WinForms.Guna2TextBox textBox,
    string arabicText)
        {
            if (textBox == null)
                return;

            textBox.PlaceholderText = arabicText;
            textBox.RightToLeft = RightToLeft.Yes;
            textBox.TextAlign = HorizontalAlignment.Right;
        }
        private void TranslateInventoryControls(
    Control parent,
    Dictionary<string, string> translations)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                string text =
                    control.Text == null
                        ? ""
                        : control.Text.Trim();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    // First try an exact match, then handle captions that include
                    // an icon, shortcut marker, or extra whitespace.
                    if (translations.TryGetValue(text, out string arabic))
                    {
                        control.Text = arabic;
                    }
                    else
                    {
                        foreach (KeyValuePair<string, string> entry in translations)
                        {
                            if (text.IndexOf(entry.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                control.Text = text.Replace(entry.Key, entry.Value);
                                break;
                            }
                        }
                    }

                    bool arabicMode = IsInventoryArabic();
                    if (arabicMode)
                    {
                        if (control is Label label)
                        {
                            label.RightToLeft = RightToLeft.Yes;
                            label.TextAlign = ContentAlignment.MiddleRight;
                        }
                        else if (control is LinkLabel link)
                        {
                            link.RightToLeft = RightToLeft.Yes;
                            link.TextAlign = ContentAlignment.MiddleRight;
                        }
                        else if (control is Button button)
                        {
                            button.RightToLeft = RightToLeft.Yes;
                        }
                        else if (control is Guna.UI2.WinForms.Guna2Button gunaButton)
                        {
                            gunaButton.RightToLeft = RightToLeft.Yes;
                        }
                    }
                }

                if (control is Guna.UI2.WinForms.Guna2TextBox txt)
                {
                    string placeholder =
                        txt.PlaceholderText == null
                            ? ""
                            : txt.PlaceholderText.Trim();

                    if (!string.IsNullOrWhiteSpace(placeholder) &&
                        translations.TryGetValue(
                            placeholder,
                            out string arabicPlaceholder))
                    {
                        txt.PlaceholderText = arabicPlaceholder;
                    }
                }

                if (control.HasChildren)
                    TranslateInventoryControls(control, translations);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmViewItem_Load(object sender, EventArgs e)
        {
            LayoutModernInventoryPage();

           

            //AppTheme.DefaultFont(this.dgvItems);

            bindCombo();
            bindItemAssembly();

            if (id != 0)
            {
                BindItem();

                bool canEditItem =
                    UserPermissions.canEdit("Inventory Items");

                btnSave.Enabled = canEditItem;
                btnSaveNew.Enabled = canEditItem;
            }

            // ==========================================
            // LANGUAGE + POSITIONING
            // ==========================================
           // LocalizationManager.LocalizeForm(this);
           // LocalizationManager.LocalizeDataGridViewHeaders(dgvItems);

            ApplyInventoryLanguageLayout();
        }
        private void ApplyInventoryLanguageLayout()
        {
            bool arabic = IsInventoryArabic();

            RightToLeft = RightToLeft.No;
            RightToLeftLayout = false;

            SuspendLayout();

            try
            {
                // First restore/rebuild the normal English layout.
                LayoutModernInventoryPage();
                ApplyItemTypeLayout();

                // Normal localization manager first.
               // LocalizationManager.LocalizeForm(this);
               // LocalizationManager.LocalizeDataGridViewHeaders(dgvItems);

                if (arabic)
                {
                    // This page contains several manually-created/custom texts,
                    // therefore apply its Arabic captions explicitly.
                    ApplyInventoryArabicText();

                    ApplyInventoryHeaderDirection(true);
                    ApplyInventoryContentDirection(true);
                    ApplyInventoryInputDirection(true);
                    ApplyInventoryGridDirection(true);

                    // MUST be after ApplyItemTypeLayout()
                    ApplyInventoryArabicPositions();

                    ApplyInventoryFooterDirection(true);
                }
                else
                {
                    ApplyInventoryHeaderDirection(false);
                    ApplyInventoryContentDirection(false);
                    ApplyInventoryInputDirection(false);
                    ApplyInventoryGridDirection(false);

                    ApplyInventoryEnglishPositions();
                    ApplyInventoryFooterDirection(false);
                }

                UpdateInventoryHeaderText();
            }
            finally
            {
                ResumeLayout(true);
                PerformLayout();
            }
        }
        private void ApplyInventoryHeaderDirection(bool arabic)
        {
            if (pnlPageHeader == null)
                return;

            pnlPageHeader.RightToLeft =
                arabic ? RightToLeft.Yes : RightToLeft.No;

            foreach (Control control in pnlPageHeader.Controls)
            {
                if (control is Label lbl)
                {
                    lbl.RightToLeft =
                        arabic ? RightToLeft.Yes : RightToLeft.No;

                    lbl.TextAlign =
                        arabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }
            }
        }


        private void ApplyInventoryContentDirection(bool arabic)
        {
            if (flowContent == null)
                return;

            // Keep TopDown. Do NOT reverse the complete FlowLayoutPanel.
            flowContent.FlowDirection = FlowDirection.TopDown;
            flowContent.WrapContents = false;
            flowContent.RightToLeft = RightToLeft.No;

            ApplyPanelDirection(pnlOP, arabic);
            ApplyPanelDirection(pnlPricingCard, arabic);
            ApplyPanelDirection(pnlValuationCard, arabic);

            ApplyPanelDirection(pnlSub, arabic);
            ApplyPanelDirection(pnlAdditionalCard, arabic);
            ApplyPanelDirection(panel9, arabic);

            if (pnlAssembly != null)
                ApplyPanelDirection(pnlAssembly, arabic);
        }


        private void ApplyPanelDirection(
            Control panel,
            bool arabic)
        {
            if (panel == null)
                return;

            panel.RightToLeft =
                arabic ? RightToLeft.Yes : RightToLeft.No;

            foreach (Control control in panel.Controls)
            {
                if (control is Label lbl)
                {
                    lbl.RightToLeft =
                        arabic ? RightToLeft.Yes : RightToLeft.No;

                    lbl.TextAlign =
                        arabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }
                else if (control is LinkLabel link)
                {
                    link.RightToLeft =
                        arabic ? RightToLeft.Yes : RightToLeft.No;

                    link.TextAlign =
                        arabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }
            }
        }
        private void ApplyInventoryInputDirection(bool arabic)
        {
            ApplyInputDirection(this, arabic);
        }


        private void ApplyInputDirection(
            Control parent,
            bool arabic)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                Guna.UI2.WinForms.Guna2TextBox txt =
                    control as Guna.UI2.WinForms.Guna2TextBox;

                if (txt != null)
                {
                    txt.RightToLeft =
                        arabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    txt.TextAlign =
                        arabic
                            ? HorizontalAlignment.Right
                            : HorizontalAlignment.Left;
                }

                Guna.UI2.WinForms.Guna2ComboBox combo =
                    control as Guna.UI2.WinForms.Guna2ComboBox;

                if (combo != null)
                {
                    combo.RightToLeft =
                        arabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                }

                DateTimePicker date =
                    control as DateTimePicker;

                if (date != null)
                {
                    date.RightToLeft =
                        arabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    date.RightToLeftLayout = arabic;
                }

                if (control.HasChildren)
                    ApplyInputDirection(control, arabic);
            }
        }
        private void ApplyInventoryArabicPositions()
        {
            RightToLeft = RightToLeft.No;
            RightToLeftLayout = false;

            // =========================================================
            // TOP AREA
            // Mirror ALL labels + inputs + icons + links together.
            // =========================================================


            // If your top fields are NOT inside pnlTopFields,
            // the fallback below mirrors their actual parent containers.
            ApplyItemDetailsArabicLayout();
            //ApplyTopFieldsArabicLayout();


            // =========================================================
            // PRICING / VALUATION
            // English:
            // [ Valuation ] [ Pricing ]
            //
            // Arabic:
            // [ Pricing ] [ Valuation ]
            // when reading RTL.
            // =========================================================

            if (pnlOP != null)
            {
                if (pnlPricingCard != null &&
                    pnlValuationCard != null &&
                    pnlValuationCard.Visible)
                {
                    pnlValuationCard.Left = 0;

                    pnlPricingCard.Left =
                        pnlOP.ClientSize.Width -
                        pnlPricingCard.Width;
                }
                else if (pnlPricingCard != null)
                {
                    pnlPricingCard.Left = 0;
                }
            }

            MirrorDirectControlsOnce(pnlPricingCard);
            MirrorDirectControlsOnce(pnlValuationCard);


            // =========================================================
            // ACCOUNTING / ADDITIONAL
            // =========================================================

            if (pnlSub != null)
            {
                if (pnlAdditionalCard != null &&
                    pnlAdditionalCard.Visible)
                {
                    // Accounting left
                    if (panel9 != null)
                        panel9.Left = 0;

                    // Additional information right
                    pnlAdditionalCard.Left =
                        pnlSub.ClientSize.Width -
                        pnlAdditionalCard.Width;
                }
                else
                {
                    if (panel9 != null)
                        panel9.Left = 0;
                }
            }

            ApplyAccountingArabicLayout();
            ApplyAdditionalInfoArabicLayout();


            // =========================================================
            // TEXT ALIGNMENT
            // =========================================================

            ApplyArabicAlignment(this);

            ApplyInventoryInputDirection(true);
            ApplyInventoryGridDirection(true);
        }
        private void PlaceArabicField(
    Control parent,
    Control input,
    string englishLabel,
    string arabicLabel,
    int rightMargin)
        {
            if (parent == null || input == null)
                return;

            Control label =
                FindControlByText(
                    parent,
                    englishLabel,
                    arabicLabel);

            if (label == null)
                return;

            // Force Arabic caption.
            label.Text = arabicLabel;
            label.AutoSize = true;

            int gap = 12;

            // Label on RIGHT.
            label.Left =
                parent.ClientSize.Width -
                rightMargin -
                label.Width;

            // Input on LEFT of label.
            input.Left =
                label.Left -
                gap -
                input.Width;

            // Vertically center label against input.
            label.Top =
                input.Top +
                Math.Max(
                    0,
                    (input.Height - label.Height) / 2);

            label.RightToLeft = RightToLeft.Yes;

            if (label is Label lbl)
                lbl.TextAlign = ContentAlignment.MiddleRight;

            if (label is LinkLabel link)
                link.TextAlign = ContentAlignment.MiddleRight;

            input.RightToLeft = RightToLeft.Yes;

            if (input is Guna.UI2.WinForms.Guna2TextBox txt)
                txt.TextAlign = HorizontalAlignment.Right;
        }
        private void ApplyAccountingArabicLayout()
        {
            if (panel9 == null)
                return;

            int margin = 25;

            PlaceArabicField(
                panel9,
                cmbCOGSAccount,
                "COGS Account",
                "حساب تكلفة البضاعة المباعة",
                margin);

            PlaceArabicField(
                panel9,
                cmbIncomeAccount,
                "Income Account",
                "حساب الدخل",
                margin);

            PlaceArabicField(
                panel9,
                cmbAssetAccount,
                "Asset Account",
                "حساب الأصول",
                margin);

            PlaceArabicField(
                panel9,
                cmbVendorAccount,
                "Vendor Account",
                "حساب المورد",
                margin);
        }
        private void ApplyAdditionalInfoArabicLayout()
        {
            if (pnlAdditionalCard == null)
                return;

            int margin = 25;

            PlaceArabicField(
                pnlAdditionalCard,
                txtBarcode,
                "Barcode",
                "الباركود",
                margin);

            PlaceArabicField(
                pnlAdditionalCard,
                cmbMainUnit,
                "Main Unit",
                "الوحدة الرئيسية",
                margin);

            PlaceArabicField(
                pnlAdditionalCard,
                txtMin,
                "Min. Amount",
                "الحد الأدنى للكمية",
                margin);

            PlaceArabicField(
                pnlAdditionalCard,
                txtMax,
                "Max. Amount",
                "الحد الأقصى للكمية",
                margin);

            // Arabic placeholder
            txtBarcode.PlaceholderText = "أدخل الباركود";

            // Translate link
            Control addUnit =
                FindControlByText(
                    pnlAdditionalCard,
                    "Add Unit +",
                    "إضافة وحدة +");

            if (addUnit != null)
            {
                addUnit.Text = "إضافة وحدة +";
                addUnit.RightToLeft = RightToLeft.Yes;
            }
        }
        private void PositionArabicField(
    Control input,
    Control label,
    int rightMargin = 25,
    int gap = 12)
        {
            if (input == null ||
                label == null ||
                input.Parent == null)
                return;

            Control parent = input.Parent;

            // Input goes toward the RIGHT.
            input.Left =
                parent.ClientSize.Width -
                rightMargin -
                input.Width;

            // Label sits immediately to the LEFT of the input.
            label.AutoSize = true;

            label.Left =
                input.Left -
                gap -
                label.Width;

            label.Top =
                input.Top +
                Math.Max(
                    0,
                    (input.Height - label.Height) / 2);

            if (label is Label lbl)
            {
                lbl.RightToLeft = RightToLeft.Yes;
                lbl.TextAlign = ContentAlignment.MiddleRight;
            }

            input.RightToLeft = RightToLeft.Yes;

            if (input is Guna.UI2.WinForms.Guna2TextBox txt)
                txt.TextAlign = HorizontalAlignment.Right;
        }
        private void MirrorParentOf(Control control)
        {
            if (control == null || control.Parent == null)
                return;

            MirrorDirectControlsOnce(control.Parent);
        }
        private Control FindControlByText(
    Control parent,
    params string[] texts)
        {
            if (parent == null)
                return null;

            foreach (Control control in parent.Controls)
            {
                string value =
                    (control.Text ?? "")
                    .Replace("*", "")
                    .Trim();

                foreach (string text in texts)
                {
                    if (string.Equals(
                        value,
                        text,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return control;
                    }
                }

                if (control.HasChildren)
                {
                    Control found =
                        FindControlByText(control, texts);

                    if (found != null)
                        return found;
                }
            }

            return null;
        }
        private void ApplyItemDetailsArabicLayout()
        {
            if (txtcode == null || txtName == null)
                return;

            Control parent = txtcode.Parent;

            if (parent == null)
                return;

            Control lblCode =
                FindControlByText(
                    parent,
                    "Item Code",
                    "كود الصنف");

            Control lblName =
                FindControlByText(
                    parent,
                    "Item Name",
                    "اسم الصنف");

            if (lblCode == null || lblName == null)
                return;

            // =====================================================
            // FORCE ARABIC TEXT
            // =====================================================
            lblCode.Text = "* كود الصنف";
            lblName.Text = "* اسم الصنف";

            lblCode.AutoSize = true;
            lblName.AutoSize = true;

            int margin = 25;
            int labelGap = 12;
            int groupGap = 70;

            int rightEdge = parent.ClientSize.Width - margin;

            // =====================================================
            // ITEM CODE -> RIGHT SIDE
            //
            // [ textbox ]  * كود الصنف
            // =====================================================

            lblCode.Left =
                rightEdge - lblCode.Width;

            txtcode.Left =
                lblCode.Left -
                labelGap -
                txtcode.Width;

            lblCode.Top =
                txtcode.Top +
                Math.Max(
                    0,
                    (txtcode.Height - lblCode.Height) / 2);


            // =====================================================
            // ITEM NAME -> LEFT GROUP
            //
            // [ textbox ]  * اسم الصنف
            // =====================================================

            int nameRightEdge =
                txtcode.Left - groupGap;

            lblName.Left =
                nameRightEdge - lblName.Width;

            txtName.Left =
                lblName.Left -
                labelGap -
                txtName.Width;

            lblName.Top =
                txtName.Top +
                Math.Max(
                    0,
                    (txtName.Height - lblName.Height) / 2);


            // =====================================================
            // RTL
            // =====================================================

            lblCode.RightToLeft = RightToLeft.Yes;
            lblName.RightToLeft = RightToLeft.Yes;

            if (lblCode is Label codeLabel)
                codeLabel.TextAlign = ContentAlignment.MiddleRight;

            if (lblName is Label nameLabel)
                nameLabel.TextAlign = ContentAlignment.MiddleRight;

            txtcode.RightToLeft = RightToLeft.Yes;
            txtName.RightToLeft = RightToLeft.Yes;

            txtcode.TextAlign = HorizontalAlignment.Right;
            txtName.TextAlign = HorizontalAlignment.Right;

            txtName.PlaceholderText = "اسم الصنف";
        }
        private readonly HashSet<Control> _arabicMirroredContainers =
    new HashSet<Control>();

        private void MirrorDirectControlsOnce(Control parent)
        {
            if (parent == null)
                return;

            // Prevent repeated Resize events from flipping the controls
            // backwards again.
            if (_arabicMirroredContainers.Contains(parent))
                return;

            int width = parent.ClientSize.Width;

            if (width <= 0)
                return;

            foreach (Control control in parent.Controls)
            {
                if (!control.Visible)
                    continue;

                if (control.Dock != DockStyle.None)
                    continue;

                // Don't move complete cards/containers here.
                if (control is Panel ||
                    control is FlowLayoutPanel ||
                    control is TableLayoutPanel)
                {
                    continue;
                }

                control.Left =
                    width -
                    control.Left -
                    control.Width;
            }

            _arabicMirroredContainers.Add(parent);
        }
        private void ApplyArabicAlignment(Control parent)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (control is Label lbl)
                {
                    lbl.RightToLeft = RightToLeft.Yes;
                    lbl.TextAlign = ContentAlignment.MiddleRight;
                }
                else if (control is LinkLabel link)
                {
                    link.RightToLeft = RightToLeft.Yes;
                    link.TextAlign = ContentAlignment.MiddleRight;
                }
                else if (control is Guna.UI2.WinForms.Guna2TextBox txt)
                {
                    txt.RightToLeft = RightToLeft.Yes;
                    txt.TextAlign = HorizontalAlignment.Right;
                }
                else if (control is Guna.UI2.WinForms.Guna2ComboBox combo)
                {
                    combo.RightToLeft = RightToLeft.Yes;
                }

                if (control.HasChildren)
                    ApplyArabicAlignment(control);
            }
        }
        private void MirrorSectionOnce(Control parent)
        {
            if (parent == null)
                return;

            int width = parent.ClientSize.Width;

            if (width <= 0)
                return;

            foreach (Control control in parent.Controls)
            {
                if (!control.Visible)
                    continue;

                if (control.Dock != DockStyle.None)
                    continue;

                // Don't move whole nested layout containers here.
                if (control is Panel ||
                    control is FlowLayoutPanel ||
                    control is TableLayoutPanel)
                {
                    continue;
                }

                int oldLeft = control.Left;

                control.Left =
                    Math.Max(
                        0,
                        width - oldLeft - control.Width);

                // Arabic label alignment
                if (control is Label lbl)
                {
                    lbl.RightToLeft = RightToLeft.Yes;
                    lbl.TextAlign = ContentAlignment.MiddleRight;
                }
                else if (control is LinkLabel link)
                {
                    link.RightToLeft = RightToLeft.Yes;
                    link.TextAlign = ContentAlignment.MiddleRight;
                }
            }
        }
        private void MirrorFieldPair(Control input)
        {
            if (input == null || input.Parent == null)
                return;

            Control parent = input.Parent;

            int width = parent.ClientSize.Width;

            if (width <= 0)
                return;

            input.Left =
                Math.Max(
                    0,
                    width - input.Left - input.Width);
        }
        private void MirrorControlsInsideContainer(Control parent)
        {
            if (parent == null)
                return;

            int width = parent.ClientSize.Width;

            if (width <= 0)
                return;

            foreach (Control control in parent.Controls)
            {
                // Do not mirror Docked controls.
                if (control.Dock != DockStyle.None)
                    continue;

                // Avoid changing hidden controls unnecessarily.
                if (!control.Visible)
                    continue;

                int newLeft =
                    width -
                    control.Right;

                if (newLeft >= 0)
                    control.Left = newLeft;
            }
        }
        private void frmViewItem_Resize(object sender, EventArgs e)
        {
            LayoutModernInventoryPage();

            bool arabic = IsInventoryArabic();

            ApplyItemTypeLayout();

            // New English coordinates were just restored,
            // so allow Arabic containers to be mirrored again.
            _arabicMirroredContainers.Clear();

            if (arabic)
            {
                ApplyInventoryArabicText();

                ApplyInventoryHeaderDirection(true);
                ApplyInventoryContentDirection(true);
                ApplyInventoryInputDirection(true);

                ApplyInventoryArabicPositions();
                ApplyInventoryFooterDirection(true);
            }
            else
            {
                ApplyInventoryEnglishPositions();
                ApplyInventoryFooterDirection(false);
            }
        }
        private void ApplyInventoryEnglishPositions()
        {
            // Restore your existing English item/service layout.
            ApplyItemTypeLayout();

            ApplyInventoryInputDirection(false);
            ApplyInventoryGridDirection(false);

            if (pnlPageHeader != null)
                pnlPageHeader.RightToLeft = RightToLeft.No;

            if (flowContent != null)
                flowContent.RightToLeft = RightToLeft.No;

            if (panel1 != null)
                panel1.RightToLeft = RightToLeft.No;
        }
        private void ApplyInventoryFooterDirection(bool arabic)
        {
            if (panel1 == null)
                return;

            panel1.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            int margin = 25;
            int gap = 10;

            if (arabic)
            {
                // Save buttons -> left side
                int x = margin;

                if (btnSave != null)
                {
                    btnSave.Anchor =
                        AnchorStyles.Bottom |
                        AnchorStyles.Left;

                    btnSave.Left = x;
                    x += btnSave.Width + gap;
                }

                if (btnSaveNew != null)
                {
                    btnSaveNew.Anchor =
                        AnchorStyles.Bottom |
                        AnchorStyles.Left;

                    btnSaveNew.Left = x;
                    x += btnSaveNew.Width + gap;
                }

                if (btnClear != null)
                {
                    btnClear.Anchor =
                        AnchorStyles.Bottom |
                        AnchorStyles.Left;

                    btnClear.Left = x;
                }

                // Back/Close -> right
                if (btnBackToItems != null)
                {
                    btnBackToItems.Anchor =
                        AnchorStyles.Bottom |
                        AnchorStyles.Right;

                    btnBackToItems.Left =
                        panel1.ClientSize.Width -
                        btnBackToItems.Width -
                        margin;
                }
            }
            else
            {
                // Restore normal LTR anchors.
                if (btnSave != null)
                    btnSave.RightToLeft = RightToLeft.No;

                if (btnSaveNew != null)
                    btnSaveNew.RightToLeft = RightToLeft.No;

                if (btnClear != null)
                    btnClear.RightToLeft = RightToLeft.No;

                if (btnBackToItems != null)
                    btnBackToItems.RightToLeft = RightToLeft.No;
            }
        }
        private void ApplyInventoryGridDirection(bool arabic)
        {
            if (dgvItems == null)
                return;

            dgvItems.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            dgvItems.ColumnHeadersDefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvItems.DefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvItems);
        }
        private void bindItemAssembly()
        {
            dgvItems.Rows.Clear();
            DataTable dt = ClinicDatabase.ExecuteDataTable("select code,name from tbl_items where  state = 0 and active = 0");
            DataGridViewComboBoxColumn name = (DataGridViewComboBoxColumn)dgvItems.Columns["name"];
            name.DataSource = dt;
            name.DisplayMember = "name";
            name.ValueMember = "code";
        }
        private void setComboBoxCategory()
        {
            BindCombos.PopulateItemsCategory(cmbCategory);
            SetSelectedIndexToLastItem(cmbCategory);
        }

        private void SetSelectedIndexToLastItem(Guna.UI2.WinForms.Guna2ComboBox comboBox)
        {
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = comboBox.Items.Count - 1;
            }
        }

        public void bindCombo()
        {
            BindCombos.PopulateWarehouse(cmbWarehouse);
            BindCombos.PopulateItemsCategory(cmbCategory);
            BindCombos.PopulateItemUnit(cmbMainUnit);
            BindCombos.PopulateVendors(cmbVendorAccount);
            cmbVendorAccount.SelectedValue = -1;
            BindCombos.PopulateAllLevel4Account(cmbIncomeAccount);
            BindCombos.PopulateAllLevel4Account(cmbAssetAccount);
            BindCombos.PopulateAllLevel4Account(cmbCOGSAccount);
            cmbIncomeAccount.SelectedValue = BindCombos.SelectDefaultLevelAccount("Sales");
            cmbAssetAccount.SelectedValue = BindCombos.SelectDefaultLevelAccount("Inventory");
            cmbCOGSAccount.SelectedValue = BindCombos.SelectDefaultLevelAccount("COGS");


           

            ApplyItemTypeLayout();
        }
        private void BindItem()
        {
            MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_items where id = " + id);
            if (reader.Read())
            {
                if (reader["method"].ToString() == "lifo")
                    RadLifo.Checked = true;
                else if (reader["method"].ToString() == "fifo")
                    radFifo.Checked = true;
                else
                    radAvg.Checked = true;
                //cmbCategory.Enabled = cmbType.Enabled = cmbWarehouse.Enabled = false;
                cmbCategory.SelectedValue = int.Parse(reader["category_id"].ToString());
                txtName.Text = reader["name"].ToString();
                txtBarcode.Text = reader["barcode"].ToString();
                txtCostPrice.Text = reader["cost_price"].ToString();
                txtSalesPrice.Text = reader["sales_price"].ToString();
                txtMin.Text = reader["min_amount"].ToString();
                txtMax.Text = reader["max_amount"].ToString();
                string OnHand = reader["on_hand"].ToString();
                txtTotalValue.Text = reader["total_value"].ToString();
                cmbType.Text = reader["type"].ToString();
                cmbMainUnit.SelectedValue = reader["unit_id"].ToString();
                txtcode.Text = itemCode = reader["code"].ToString();
                cmbCOGSAccount.SelectedValue = reader["cogs_account_id"].ToString();
                cmbVendorAccount.SelectedValue = reader["vendor_id"].ToString();
                cmbIncomeAccount.SelectedValue = reader["income_account_id"].ToString();
                cmbAssetAccount.SelectedValue = reader["asset_account_id"].ToString();
                cmbPurchasetype.Text = reader["Item_type"].ToString();

                // Load saved item image.
                _currentImageDbPath =
                    HasColumn(reader, "img") && reader["img"] != DBNull.Value
                        ? reader["img"].ToString()
                        : string.Empty;

                _selectedImageSourcePath = string.Empty;
                _selectedImageBytes = null;
                _selectedImageFileName = string.Empty;
                _removeImageRequested = false;

                // New behavior: read the actual picture from MySQL first.
                // Old path-based records still work as a fallback.
                if (!LoadItemImageFromDatabaseBytes(id))
                    LoadItemImageFromDatabasePath(_currentImageDbPath);

                

                dtStart.Value = DateTime.Parse(reader["date"].ToString());
                chkActive.Checked = (int.Parse(reader["active"].ToString()) == 0) ? true : false;
                txtCostPrice.Enabled = txtSalesPrice.Enabled = txtOnHand.Enabled = dtStart.Enabled = true;
                reader = ClinicDatabase.ExecuteReader("select tbl_items_unit.* , tbl_unit.name from tbl_items_unit  inner join tbl_unit on tbl_items_unit.unit_id = tbl_unit.id where item_id =@id", ClinicDatabase.CreateParameterNew("id", id));
                while (reader.Read())
                {
                    if (cmbMainUnit.SelectedValue != null && int.Parse(reader["unit_id"].ToString()) == int.Parse(cmbMainUnit.SelectedValue.ToString()))
                        continue;
                    unitIds.Add(int.Parse(reader["unit_id"].ToString()));
                    unitFactors.Add(decimal.Parse(reader["factor"].ToString()));
                    unitNames.Add(reader["name"].ToString());
                }
                reader = ClinicDatabase.ExecuteReader("select ti.code,ti.id,ta.qty,ti.cost_price from tbl_item_assembly ta inner join tbl_items ti on ta.item_id = ti.id where ta.assembly_id =@id", ClinicDatabase.CreateParameterNew("id", id));
                while (reader.Read())
                {
                    dgvItems.Rows.Add(reader["id"].ToString(), "", reader["code"].ToString(), reader["code"].ToString(), reader["cost_price"].ToString(), reader["qty"].ToString());
                }

                object result0 = ClinicDatabase.ExecuteScalar(@"SELECT COUNT(1) from tbl_item_transaction where type != 'Opening Qty' and item_id = @id", ClinicDatabase.CreateParameterNew("id", id));
                int recordCount0 = 0;
                if (result0 != null && result0 != DBNull.Value)
                    recordCount0 = Convert.ToInt32(result0);
                if (recordCount0 > 0)
                {
                    txtOnHand.Enabled = false;
                    object result1 = ClinicDatabase.ExecuteScalar(@"SELECT IFNULL(qty_in,0) from tbl_item_transaction where type = 'Opening Qty' and item_id = @id", ClinicDatabase.CreateParameterNew("id", id));
                    decimal recordQty = 0;
                    if (result1 != null && result1 != DBNull.Value)
                        recordQty = Convert.ToInt32(result1);
                    if (recordQty > 0)
                    {
                        txtOnHand.Text = recordQty.ToString("N2");
                    }
                }
                else
                {
                    txtOnHand.Enabled = true;
                    txtOnHand.Text = Convert.ToDecimal(OnHand).ToString("N2");
                }

                ApplyItemTypeLayout();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveCurrentItem())
            {
                //.RefreshItem();
                Close();
            }
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (SaveCurrentItem())
            {
                //.RefreshItem();
                ResetForNewItem();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetForNewItem();
        }

        private void btnBackToItems_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool SaveCurrentItem()
        {
            try
            {
                if (id == 0)
                    return insertItem();

                return updateItem();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The inventory item could not be saved.\n\n" +
                    GetDeepestMessage(ex),
                    "Inventory Item",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        void insertItemUnits(int id)
        {
            ClinicDatabase.ExecuteReader(@"insert into tbl_items_unit (item_id,unit_id,factor) values (@id,@unit_id,@factor)",
                   ClinicDatabase.CreateParameterNew("id", id),
                   ClinicDatabase.CreateParameterNew("unit_id", cmbMainUnit.SelectedValue),
                   ClinicDatabase.CreateParameterNew("factor", 1));
            ////Utilities.LogAudit(-1, "Insert Item Unit", "Item Unit", id, "Inserted Main Unit: " + cmbMainUnit.Text + " for Item: " + txtName.Text);
            if (unitFactors != null)
                for (int i = 0; i < unitFactors.Count; i++)
                {
                    ClinicDatabase.ExecuteReader(@"insert into tbl_items_unit (item_id,unit_id,factor) values (@id,@unit_id,@factor)",
                        ClinicDatabase.CreateParameterNew("id", id),
                        ClinicDatabase.CreateParameterNew("unit_id", unitIds[i]),
                        ClinicDatabase.CreateParameterNew("factor", unitFactors[i]));
                    ////Utilities.LogAudit(-1, "Insert Item Unit", "Item Unit", id, "Inserted Sub Unit: " + unitNames[i] + " for Item: " + txtName.Text);
                }
        }
        private bool updateItem()
        {
            if (!chkRequiredDate())
                return false;
            MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_items where name = @name",
               ClinicDatabase.CreateParameterNew("name", txtName.Text));
            if (reader.Read() && id != int.Parse(reader["id"].ToString()))
            {
                MessageBox.Show("Item Already Exists. Enter Another Name.");
                return false;
            }
            string imageDbPath = PrepareImageForSave(itemCode);
            byte[] imageBytes = GetImageBytesForDatabase();
            string imageFileName = GetImageFileNameForDatabase();

            ClinicDatabase.ExecuteNonQuery(@"UPDATE tbl_items SET type = @type,category_id=@category,name = @name,unit_id = @unit_id,barcode = @barcode,
                              cost_price = @cost_price,cogs_account_id = @cogs_account_id,vendor_id = @vendor_id,sales_price = @sales_price,
                              income_account_id = @income_account_id,asset_account_id = @asset_account_id,
                              min_amount = @min_amount,max_amount = @max_amount,on_hand = @on_hand,method=@method,total_value = @total_value,
                              date = @date,img = @img,ItemImg = @img_data,active = @active,state = @state,created_By = @created_By,created_date = @created_date,
                              Item_type=@Item_type
                              WHERE id = @id",
                            ClinicDatabase.CreateParameterNew("type", cmbType.Text),
                            ClinicDatabase.CreateParameterNew("name", txtName.Text),
                            ClinicDatabase.CreateParameterNew("category", cmbCategory.SelectedValue),
                             ClinicDatabase.CreateParameterNew("unit_id", cmbMainUnit.SelectedValue ?? 0),
                            ClinicDatabase.CreateParameterNew("barcode", txtBarcode.Text),
                            ClinicDatabase.CreateParameterNew("cost_price", txtCostPrice.Text == "" ? null : txtCostPrice.Text),
                             ClinicDatabase.CreateParameterNew("cogs_account_id", cmbCOGSAccount.SelectedValue),
                             ClinicDatabase.CreateParameterNew("vendor_id", cmbVendorAccount.SelectedValue ?? 0),
                             ClinicDatabase.CreateParameterNew("sales_price", txtSalesPrice.Text == "" ? null : txtSalesPrice.Text),
                            ClinicDatabase.CreateParameterNew("income_account_id", cmbIncomeAccount.SelectedValue),
                            ClinicDatabase.CreateParameterNew("asset_account_id", cmbAssetAccount.SelectedValue),
                            ClinicDatabase.CreateParameterNew("min_amount", txtMin.Text),
                            ClinicDatabase.CreateParameterNew("max_amount", txtMax.Text),
                            ClinicDatabase.CreateParameterNew("on_hand", txtOnHand.Text),
                            ClinicDatabase.CreateParameterNew("method", radAvg.Checked ? "avg" : radFifo.Checked ? "fifo" : "lifo"),
                            ClinicDatabase.CreateParameterNew("total_value", txtTotalValue.Text),
                            ClinicDatabase.CreateParameterNew("date", dtStart.Value.Date),
                            ClinicDatabase.CreateParameterNew("img", imageDbPath),
                            ClinicDatabase.CreateParameterNew("img_data", (object)imageBytes ?? DBNull.Value),
                            ClinicDatabase.CreateParameterNew("active", chkActive.Checked ? 0 : 1),
                            ClinicDatabase.CreateParameterNew("state", 0),
                            ClinicDatabase.CreateParameterNew("created_By", -1),
                            ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date),
                            ClinicDatabase.CreateParameterNew("id", id),
                            ClinicDatabase.CreateParameterNew("@Item_type", cmbPurchasetype.Text));
            ClinicDatabase.ExecuteNonQuery("delete from tbl_items_unit where item_id=@id", ClinicDatabase.CreateParameterNew("id", id));
            ClinicDatabase.ExecuteNonQuery("delete from tbl_item_assembly where assembly_id=@id", ClinicDatabase.CreateParameterNew("id", id));
            if (cmbType.Text != "12 - Service" && cmbMainUnit.SelectedValue != null)
                insertItemUnits(id);
            if (cmbType.Text == "13 - Inventory Assembly")
                insertItemsAssembly(id);

            ClinicDatabase.ExecuteNonQuery(@"DELETE FROM tbl_item_transaction where type = 'Opening Qty' and item_id = @id;DELETE FROM tbl_transaction where type = 'Opening Qty' AND transaction_id= @id", ClinicDatabase.CreateParameterNew("id", id));
            ClinicDatabase.ExecuteNonQuery("DELETE FROM tbl_item_card_details WHERE `trans_type` = 'Opening Qty' and itemId = @id", ClinicDatabase.CreateParameterNew("id", id));
            if (txtOnHand.Text.Trim() != "0" && cmbType.Text != "12 - Service")
            {
                if (isUpdateOpening())
                {
                    insertItemTransaction();
                    insertItemJournal();
                }
            }
            ////Utilities.LogAudit(-1, "Update Item", "Item", id, "Updated Item: " + txtName.Text + " with Code: " + itemCode);

            _selectedImageBytes = null;
            _selectedImageSourcePath = string.Empty;
            _removeImageRequested = false;

            return true;
        }
        private bool isUpdateOpening()
        {
            if (decimal.Parse(txtOnHand.Text.ToString()) <= 0)
            {
                return false;
            }

            return true;
        }
        string getNextCode()
        {
            string typeCategory = cmbType.Text.Split('-')[0].Trim() + cmbCategory.Text.Split('-')[0].Trim();
            string query = "SELECT MAX(RIGHT(code, 4)) FROM tbl_items WHERE LEFT(code, 5) = @typeCategory";
            object result = ClinicDatabase.ExecuteScalar(query, ClinicDatabase.CreateParameterNew("@typeCategory", typeCategory));
            int nextSerial = (result == DBNull.Value || result == null) ? 1 : Convert.ToInt32(result) + 1;
            string formattedSerial = nextSerial.ToString().PadLeft(4, '0');
            string newItemCode = typeCategory + formattedSerial;
            return newItemCode;
        }
        private bool insertItem()
        {
            if (!chkRequiredDate())
                return false;
            MySqlDataReader reader = ClinicDatabase.ExecuteReader("select * from tbl_items where name = @name",
               ClinicDatabase.CreateParameterNew("name", txtName.Text));
            if (reader.Read())
            {
                MessageBox.Show("Item Already Exists. Enter Another Name.");
                return false;
            }
            itemCode = getNextCode();
            string imageDbPath = PrepareImageForSave(itemCode);
            byte[] imageBytes = GetImageBytesForDatabase();
            string imageFileName = GetImageFileNameForDatabase();
            id = (int)decimal.Parse(ClinicDatabase.ExecuteScalar(@"INSERT INTO `tbl_items`(`code`,`warehouse_id`,  `type`,category_id, `name`, `unit_id`, `barcode`, `cost_price`, 
                                    `cogs_account_id`, `vendor_id`, `sales_price`, `income_account_id`, `asset_account_id`, 
                                    `min_amount`, `max_amount`, `on_hand`,method, `total_value`, `date`, `img`, `ItemImg`,  `active`, `state`, 
                                    `created_By`, `created_date`,Item_type, GlobalId, DeviceId) VALUES (
                                    @code, @warehouseId, @type,@category, @name, @unit_id, @barcode, @cost_price, 
                                    @cogs_account_id, @vendor_id, @sales_price, @income_account_id, @asset_account_id, 
                                    @min_amount, @max_amount, @on_hand,@method, @total_value, @date, @img, @img_data, @active, @state, 
                                    @created_By, @created_date,@Item_type, @GlobalId, @DeviceId); SELECT LAST_INSERT_ID();",
                             ClinicDatabase.CreateParameterNew("code", itemCode),
                             ClinicDatabase.CreateParameterNew("warehouseId", cmbWarehouse.SelectedValue.ToString()),
                             ClinicDatabase.CreateParameterNew("type", cmbType.Text),
                             ClinicDatabase.CreateParameterNew("category", cmbCategory.SelectedValue),
                             ClinicDatabase.CreateParameterNew("name", txtName.Text),
                             ClinicDatabase.CreateParameterNew("unit_id", cmbMainUnit.SelectedValue ?? 0),
                             ClinicDatabase.CreateParameterNew("barcode", txtBarcode.Text),
                             ClinicDatabase.CreateParameterNew("cost_price", txtCostPrice.Text == "" ? "0" : txtCostPrice.Text),
                             ClinicDatabase.CreateParameterNew("cogs_account_id", cmbCOGSAccount.SelectedValue),
                             ClinicDatabase.CreateParameterNew("vendor_id", cmbVendorAccount.SelectedValue ?? 0),
                             ClinicDatabase.CreateParameterNew("sales_price", txtSalesPrice.Text == "" ? "0" : txtSalesPrice.Text),
                             ClinicDatabase.CreateParameterNew("income_account_id", cmbIncomeAccount.SelectedValue),
                             ClinicDatabase.CreateParameterNew("asset_account_id", cmbAssetAccount.SelectedValue),
                             ClinicDatabase.CreateParameterNew("min_amount", txtMin.Text),
                             ClinicDatabase.CreateParameterNew("max_amount", txtMax.Text),
                             ClinicDatabase.CreateParameterNew("on_hand", txtOnHand.Text),
                             ClinicDatabase.CreateParameterNew("method", radAvg.Checked ? "avg" : radFifo.Checked ? "fifo" : "lifo"),
                             ClinicDatabase.CreateParameterNew("total_value", txtTotalValue.Text),
                             ClinicDatabase.CreateParameterNew("date", dtStart.Value.Date),
                             ClinicDatabase.CreateParameterNew("img", imageDbPath),
                             ClinicDatabase.CreateParameterNew("img_data", (object)imageBytes ?? DBNull.Value),
                             ClinicDatabase.CreateParameterNew("active", chkActive.Checked ? 0 : 1),
                ClinicDatabase.CreateParameterNew("GlobalId", Guid.NewGuid().ToString()),
                ClinicDatabase.CreateParameterNew("DeviceId", Utilities.MachineName),
                ClinicDatabase.CreateParameterNew("state", 0),
                             ClinicDatabase.CreateParameterNew("created_By", -1),
                             ClinicDatabase.CreateParameterNew("created_date", DateTime.Now.Date),
                             ClinicDatabase.CreateParameterNew("@Item_type", cmbPurchasetype.Text)).ToString());

            if (cmbType.Text != "12 - Service" && cmbMainUnit.SelectedValue != null)
                insertItemUnits(id);
            if (cmbType.Text == "13 - Inventory Assembly")
                insertItemsAssembly(id);
            if (txtOnHand.Text.Trim() != "0" && cmbType.Text != "12 - Service")
            {
                if (isUpdateOpening())
                {
                    insertItemTransaction();
                    insertItemJournal();
                }
            }
            ////Utilities.LogAudit(-1, "Insert Item", "Item", id, "Inserted Item: " + txtName.Text + " with Code: " + itemCode);

            _selectedImageBytes = null;
            _selectedImageSourcePath = string.Empty;
            _removeImageRequested = false;

            return true;
        }

        private void insertItemsAssembly(int id)
        {
            for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
            {
                ClinicDatabase.ExecuteNonQuery("insert into tbl_item_assembly (assembly_id,item_id,qty) values (@assembly_id,@item_id,@qty)",
                    ClinicDatabase.CreateParameterNew("assembly_id", id),
                    ClinicDatabase.CreateParameterNew("item_id", dgvItems.Rows[i].Cells["itemId"].Value.ToString()),
                    ClinicDatabase.CreateParameterNew("qty", dgvItems.Rows[i].Cells["qty"].Value.ToString()));
            }
        }

        private void insertItemTransaction()
        {
            CommonInsert.InsertItemTransaction(dtStart.Value.Date, "Opening Qty", "0", id.ToString(), txtCostPrice.Text,
                txtOnHand.Text, "0", "0", txtOnHand.Text, "Opening Balance", cmbWarehouse.SelectedValue.ToString());
        }
        private void insertItemJournal()
        {
            CommonInsert.InsertTransactionEntry(dtStart.Value.Date, BindCombos.SelectDefaultLevelAccount("Inventory").ToString(), txtTotalValue.Text, "0",
                id.ToString(), id.ToString(), "Opening Qty", "Opening Balance - Item Code - " + itemCode, -1, DateTime.Now.Date);
            CommonInsert.InsertTransactionEntry(dtStart.Value.Date, BindCombos.SelectDefaultLevelAccount("Opening Balance Equity").ToString(), "0", txtTotalValue.Text,
                    id.ToString(), "0", "Opening Qty", "Opening Balance Equity - Item Code - " + itemCode, -1, DateTime.Now.Date);
        }

        private bool chkRequiredDate()
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Enter Item Name First.");
                txtName.Focus();
                return false;
            }
            if (cmbWarehouse.Text.Trim() == "")
            {
                MessageBox.Show("Enter Warehouse First");
                cmbWarehouse.Focus();
                return false;
            }
            if (cmbCategory.Text.Trim() == "")
            {
                MessageBox.Show("Enter Category First");
                cmbCategory.Focus();
                return false;
            }

            if (txtOnHand.Text != "" && txtCostPrice.Text == "")
            {
                MessageBox.Show("Enter Cost Price");
                return false;
            }

            if (cmbType.Text != "12 - Service")
            {

                if (dtStart.Value.Date > DateTime.Now.Date && txtOnHand.Text != "")
                {
                    MessageBox.Show("Date Must Be Less Or Equal Today");
                    return false;
                }

                //if (decimal.Parse(txtCostPrice.Text) >= decimal.Parse(txtSalesPrice.Text))
                //{
                //    MessageBox.Show("Cost Price Must Be Less Than Sale Price");
                //    return false;
                //}
                if (!Utilities.AreDefaultAccountsSet(new List<string> { "COGS", "Sales", "Inventory" }))
                {
                    MessageBox.Show("Default accounts for Item are not properly configured. Please check your settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                foreach (int unit in unitIds)
                {
                    if (unit == (int)cmbMainUnit.SelectedValue)
                    {
                        MessageBox.Show("Main Unit Can't Be Included As Sub Unit");
                        return false;
                    }
                }
            }
            if (txtMin.Text.Trim() == "")
                txtMin.Text = "0";
            if (txtMax.Text.Trim() == "")
                txtMax.Text = "0";
            if (txtOnHand.Text.Trim() == "")
                txtOnHand.Text = "0";

            if (txtTotalValue.Text.Trim() == "")
                txtTotalValue.Text = "0";
            if (cmbType.Text == "13 - Inventory Assembly")
            {
                for (int i = 0; i < dgvItems.Rows.Count - 1; i++)
                {
                    if (dgvItems.Rows[i].Cells["qty"].Value == null || dgvItems.Rows[i].Cells["name"].Value == null || dgvItems.Rows[i].Cells["qty"].Value.ToString() == "" || dgvItems.Rows[i].Cells["qty"].Value.ToString() == "0" || dgvItems.Rows[i].Cells["code"].Value == null || dgvItems.Rows[i].Cells["code"].Value.ToString() == "")
                    {
                        MessageBox.Show("Item Assembly Can't be 0 or empty");
                        return false;
                    }
                }
            }


            return true;
        }


        private static bool HasColumn(
            MySqlDataReader reader,
            string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch
            {
                return false;
            }
        }

        private void resetTextBox()
        {
            txtcode.Text = "";
            txtName.Text = "";
            txtBarcode.Text = "";
            txtCostPrice.Text = "";
            txtSalesPrice.Text = "";
            txtMin.Text = "";
            txtMax.Text = "";
            txtOnHand.Text = "";
            dtStart.Value = DateTime.Now;

            dgvItems.Rows.Clear();
        }

        private void txtSalesPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }
        private void txtCostPrice_TextChanged(object sender, EventArgs e)
        {

            decimal costPrice = string.IsNullOrWhiteSpace(txtCostPrice.Text) ? 0 : Convert.ToDecimal(txtCostPrice.Text);
            decimal OnHand = string.IsNullOrWhiteSpace(txtOnHand.Text) ? 0 : Convert.ToDecimal(txtOnHand.Text);

            decimal totalSalary = costPrice * OnHand;
            txtTotalValue.Text = totalSalary.ToString();
        }

        private void lnkAddItemUnit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmMain.LoadFormIntoPanel(new frmViewItemUnit(0));
            BindCombos.PopulateItemUnit(cmbMainUnit, false, true);
        }



        private void lnkNewWarehouse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmMain.LoadFormIntoPanel(new frmViewWarehouse());
            BindCombos.PopulateWarehouse(cmbWarehouse, false, true);
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyItemTypeLayout();
        }

        private void ApplyItemTypeLayout()
        {
            bool isService =
                string.Equals(
                    cmbType.Text,
                    "12 - Service",
                    StringComparison.OrdinalIgnoreCase);

            bool isAssembly =
                string.Equals(
                    cmbType.Text,
                    "13 - Inventory Assembly",
                    StringComparison.OrdinalIgnoreCase);

            // -------------------------------------------------
            // IMPORTANT:
            // Service items still need Sales Price and Accounting.
            // Only inventory-specific quantity / valuation fields
            // are hidden.
            // -------------------------------------------------
            pnlOP.Visible = true;
            pnlSub.Visible = true;

            // Sales Price is required for Service items.
            label16.Visible = true;
            txtSalesPrice.Visible = true;
            txtSalesPrice.Enabled = true;

            // Active status is also required for Service items.
            chkActive.Visible = true;
            lblActiveHint.Visible = true;

            // Accounting is always required.
            panel9.Visible = true;

            if (isService)
            {
                // Inventory-only pricing controls.
                lblOpeningQty.Visible = false;
                txtOnHand.Visible = false;

                label11.Visible = false;
                dtStart.Visible = false;

                lblCostPrice.Visible = false;
                txtCostPrice.Visible = false;

                lblTotalValue.Visible = false;
                txtTotalValue.Visible = false;

                // Inventory valuation has no meaning for a Service.
                pnlValuationCard.Visible = false;

                // Additional inventory/unit/stock information is not
                // required for a Service item.
                pnlAdditionalCard.Visible = false;

                // Give the remaining Service cards a clean full-width layout.
                pnlPricingCard.Location = new System.Drawing.Point(0, 0);
                pnlPricingCard.Size = new System.Drawing.Size(1475, 150);
                pnlOP.Size = new System.Drawing.Size(1475, 150);

                // Place Sales Price near the left and keep it easy to see.
                label16.Location = new System.Drawing.Point(25, 82);
                txtSalesPrice.Location = new System.Drawing.Point(155, 72);
                txtSalesPrice.Size = new System.Drawing.Size(220, 39);

                chkActive.Location = new System.Drawing.Point(430, 73);
                lblActiveHint.Location = new System.Drawing.Point(430, 112);

                // Accounting becomes full width for Service items.
                panel9.Location = new System.Drawing.Point(0, 0);
                panel9.Size = new System.Drawing.Size(1475, 210);
                pnlSub.Size = new System.Drawing.Size(1475, 210);

                // Make account selectors wider.
                cmbCOGSAccount.Size = new System.Drawing.Size(520, 32);
                cmbIncomeAccount.Size = new System.Drawing.Size(520, 32);
            }
            else
            {
                // Restore inventory layout.
                lblOpeningQty.Visible = true;
                txtOnHand.Visible = true;

                label11.Visible = true;
                dtStart.Visible = true;

                lblCostPrice.Visible = true;
                txtCostPrice.Visible = true;

                lblTotalValue.Visible = true;
                txtTotalValue.Visible = true;

                pnlValuationCard.Visible = true;
                pnlAdditionalCard.Visible = true;

                pnlPricingCard.Location = new System.Drawing.Point(0, 0);
                pnlPricingCard.Size = new System.Drawing.Size(987, 212);

                pnlValuationCard.Location = new System.Drawing.Point(1002, 0);
                pnlValuationCard.Size = new System.Drawing.Size(472, 212);

                pnlOP.Size = new System.Drawing.Size(1475, 212);

                // Restore normal pricing positions.
                label16.Location = new System.Drawing.Point(365, 149);
                txtSalesPrice.Location = new System.Drawing.Point(456, 139);
                txtSalesPrice.Size = new System.Drawing.Size(175, 39);

                chkActive.Location = new System.Drawing.Point(725, 66);

                pnlAdditionalCard.Location = new System.Drawing.Point(0, 0);
                pnlAdditionalCard.Size = new System.Drawing.Size(737, 237);

                panel9.Location = new System.Drawing.Point(752, 0);
                panel9.Size = new System.Drawing.Size(722, 237);

                pnlSub.Size = new System.Drawing.Size(1475, 238);

                txtCostPrice.Enabled = !isAssembly;

                pnlAssembly.Visible = isAssembly;
            }

            if (!isAssembly)
                pnlAssembly.Visible = false;

            // Recalculate the scrollable page height after a type change.
            if (flowContent != null)
            {
                flowContent.PerformLayout();
                flowContent.AutoScrollPosition =
                    new System.Drawing.Point(
                        Math.Abs(flowContent.AutoScrollPosition.X),
                        Math.Abs(flowContent.AutoScrollPosition.Y));
            }
        }

        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvItems.Rows.Count > 1 && dgvItems.CurrentRow.Cells["delete"].ColumnIndex == e.ColumnIndex && !dgvItems.CurrentRow.IsNewRow)
                    dgvItems.Rows.Remove(dgvItems.CurrentRow);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItems.Rows.Count > 1)
            {
                var row = dgvItems.Rows[e.RowIndex];

                if (e.ColumnIndex == dgvItems.Columns["Code"].Index)
                {
                    string codeValue = row.Cells["Code"].Value?.ToString();
                    DataGridViewComboBoxCell comboCell = row.Cells["name"] as DataGridViewComboBoxCell;
                    if (comboCell != null)
                        insertItemThroughCodeOrCombo("code", comboCell, null);
                }
                calculateCost();
            }
        }

        private void insertItemThroughCodeOrCombo(string type, DataGridViewComboBoxCell comboCell, ComboBox comboBox)
        {
            MySqlDataReader reader = null;
            if (type == "code")
            {
                reader = ClinicDatabase.ExecuteReader(@"SELECT *
                  FROM tbl_items 
                  WHERE code = @code ",
                    ClinicDatabase.CreateParameterNew("code", dgvItems.CurrentRow.Cells["code"].Value.ToString()));
            }
            else if (type == "combo" && comboBox.SelectedValue != null && !comboBox.SelectedValue.ToString().ToLower().Contains("rowview"))
            {
                string selectedItemCode = comboBox.SelectedValue.ToString();
                reader = ClinicDatabase.ExecuteReader(@"SELECT tbl_items.id,method,type, code,  sales_price,  cost_price 
                  FROM tbl_items 
                  WHERE  code = @code",
                    ClinicDatabase.CreateParameterNew("code", selectedItemCode));
            }
            if (reader != null && reader.Read())
            {
                dgvItems.CurrentRow.Cells["qty"].Value = "1";
                dgvItems.CurrentRow.Cells["code"].Value = reader["code"].ToString();
                dgvItems.CurrentRow.Cells["price"].Value = reader["cost_price"].ToString();
                dgvItems.CurrentRow.Cells["itemid"].Value = reader["id"].ToString();
                if (type == "code" && comboCell != null)
                    comboCell.Value = dgvItems.CurrentRow.Cells["code"].Value.ToString();
            }
            else
            {
                if (!dgvItems.CurrentRow.IsNewRow)
                    dgvItems.Rows.Remove(dgvItems.CurrentRow);
            }
        }
        decimal totalCost = 0;
        private void calculateCost()
        {
            for (int i = 0; i < dgvItems.Rows.Count; i++)
            {
                if (dgvItems.Rows[i].Cells["price"].Value == null || dgvItems.Rows[i].Cells["qty"].Value == null || dgvItems.Rows[i].Cells["price"].Value.ToString() == "" || dgvItems.Rows[i].Cells["qty"].Value.ToString() == "")
                    continue;
                totalCost += decimal.Parse(dgvItems.Rows[i].Cells["price"].Value.ToString()) * decimal.Parse(dgvItems.Rows[i].Cells["qty"].Value.ToString());
            }
            txtCostPrice.Text = totalCost.ToString();
            totalCost = 0;
        }

        private void ComboBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedValue != null)
            {
                insertItemThroughCodeOrCombo("combo", null, comboBox);
            }
        }

        private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvItems.CurrentCell.ColumnIndex == dgvItems.Columns["name"].Index)
            {
                ComboBox comboBox = e.Control as ComboBox;
                if (comboBox != null)
                {
                    comboBox.SelectedIndexChanged -= new EventHandler(ComboBoxName_SelectedIndexChanged);
                    comboBox.SelectedIndexChanged += new EventHandler(ComboBoxName_SelectedIndexChanged);
                }
            }
            else if (dgvItems.CurrentCell.ColumnIndex == dgvItems.Columns["qty"].Index || dgvItems.CurrentCell.ColumnIndex == dgvItems.Columns["price"].Index)
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

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lnkCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //frmMain.LoadFormIntoPanel(new frmViewCategory(0));
            //BindCombos.PopulateItemsCategory(cmbCategory, false, true);

            using (var frm = new frmViewCategory(0))
            {
                frm.ShowDialog(this);
            }

            BindCombos.PopulateItemsCategory(cmbCategory, false, true);

        }

        private void txtTotalValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSalesPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel6_Click(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Title = "Choose Item Image";
                    dialog.Filter =
                        "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                    dialog.Multiselect = false;
                    dialog.CheckFileExists = true;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;

                    System.IO.FileInfo file =
                        new System.IO.FileInfo(dialog.FileName);

                    const long maxBytes = 2L * 1024L * 1024L;

                    if (file.Length > maxBytes)
                    {
                        MessageBox.Show(
                            "Please select an image smaller than 2 MB.",
                            "Item Image",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    System.Drawing.Image image =
                        LoadImageWithoutLock(dialog.FileName);

                    if (image == null)
                    {
                        MessageBox.Show(
                            "The selected file could not be loaded as an image.",
                            "Item Image",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    SetPreviewImage(image);

                    _selectedImageSourcePath =
                        dialog.FileName;

                    // Keep an in-memory copy so Save writes the actual
                    // picture into MySQL, not only a local file path.
                    _selectedImageBytes =
                        System.IO.File.ReadAllBytes(dialog.FileName);

                    _selectedImageFileName =
                        System.IO.Path.GetFileName(dialog.FileName);

                    _removeImageRequested = false;

                    SetImageUploadOverlay(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The item image could not be selected.\n\n" +
                    GetDeepestMessage(ex),
                    "Item Image",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            _selectedImageSourcePath = string.Empty;
            _selectedImageBytes = null;
            _selectedImageFileName = string.Empty;
            _removeImageRequested = true;

            RestoreDefaultUploadImage();
            SetImageUploadOverlay(true);
        }

        private void SetPreviewImage(System.Drawing.Image image)
        {
            if (picItemPlaceholder == null)
                return;

            System.Drawing.Image old =
                picItemPlaceholder.Image;

            picItemPlaceholder.Image =
                image;

            picItemPlaceholder.SizeMode =
                System.Windows.Forms.PictureBoxSizeMode.Zoom;

            if (old != null &&
                !object.ReferenceEquals(old, _defaultUploadImage))
            {
                old.Dispose();
            }
        }

        private void RestoreDefaultUploadImage()
        {
            if (picItemPlaceholder == null)
                return;

            System.Drawing.Image old =
                picItemPlaceholder.Image;

            picItemPlaceholder.Image =
                _defaultUploadImage == null
                    ? null
                    : new System.Drawing.Bitmap(_defaultUploadImage);

            picItemPlaceholder.SizeMode =
                System.Windows.Forms.PictureBoxSizeMode.CenterImage;

            if (old != null &&
                !object.ReferenceEquals(old, _defaultUploadImage))
            {
                old.Dispose();
            }
        }

        private void SetImageUploadOverlay(bool visible)
        {
            if (lblImageHint != null)
                lblImageHint.Visible = visible;

            if (lblImageFormat != null)
                lblImageFormat.Visible = visible;
        }

        private void LoadItemImageFromDatabasePath(string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                RestoreDefaultUploadImage();
                SetImageUploadOverlay(true);
                return;
            }

            try
            {
                string physicalPath =
                    ResolveItemImagePath(databasePath);

                if (!System.IO.File.Exists(physicalPath))
                {
                    RestoreDefaultUploadImage();
                    SetImageUploadOverlay(true);
                    return;
                }

                System.Drawing.Image image =
                    LoadImageWithoutLock(physicalPath);

                if (image == null)
                {
                    RestoreDefaultUploadImage();
                    SetImageUploadOverlay(true);
                    return;
                }

                SetPreviewImage(image);
                SetImageUploadOverlay(false);
            }
            catch
            {
                RestoreDefaultUploadImage();
                SetImageUploadOverlay(true);
            }
        }

        private byte[] GetImageBytesForDatabase()
        {
            // User clicked Remove.
            if (_removeImageRequested)
                return null;

            // User selected a new image.
            if (_selectedImageBytes != null &&
                _selectedImageBytes.Length > 0)
            {
                return _selectedImageBytes;
            }

            // Editing an existing item without changing the picture:
            // keep the current DB bytes.
            if (id > 0)
            {
                object value =
                    ClinicDatabase.ExecuteScalar(
                        @"SELECT ItemImg
                          FROM tbl_items
                          WHERE id=@id
                          LIMIT 1;",
                        ClinicDatabase.CreateParameterNew("@id", id));

                if (value != null &&
                    value != DBNull.Value &&
                    value is byte[])
                {
                    return (byte[])value;
                }
            }

            return null;
        }

        private string GetImageFileNameForDatabase()
        {
            if (_removeImageRequested)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(_selectedImageFileName))
                return _selectedImageFileName;

            //if (id > 0)
            //{
            //    object value =
            //        ClinicDatabase.ExecuteScalar(
            //            @"SELECT IFNULL(img_file_name,'')
            //              FROM tbl_items
            //              WHERE id=@id
            //              LIMIT 1;",
            //            ClinicDatabase.CreateParameterNew("@id", id));

            //    return value == null || value == DBNull.Value
            //        ? string.Empty
            //        : Convert.ToString(value);
            //}

            return string.Empty;
        }

        private bool LoadItemImageFromDatabaseBytes(int itemId)
        {
            if (itemId <= 0)
                return false;

            try
            {
                object value =
                    ClinicDatabase.ExecuteScalar(
                        @"SELECT ItemImg
                          FROM tbl_items
                          WHERE id=@id
                          LIMIT 1;",
                        ClinicDatabase.CreateParameterNew("@id", itemId));

                if (value == null ||
                    value == DBNull.Value ||
                    !(value is byte[]))
                {
                    return false;
                }

                byte[] bytes = (byte[])value;

                if (bytes.Length == 0)
                    return false;

                using (System.IO.MemoryStream stream =
                    new System.IO.MemoryStream(bytes))
                using (System.Drawing.Image source =
                    System.Drawing.Image.FromStream(stream))
                {
                    SetPreviewImage(
                        new System.Drawing.Bitmap(source));
                }

                _selectedImageBytes = null;
                _selectedImageSourcePath = string.Empty;
                _removeImageRequested = false;

                //object fileNameValue =
                //    ClinicDatabase.ExecuteScalar(
                //        @"SELECT IFNULL(img_file_name,'')
                //          FROM tbl_items
                //          WHERE id=@id
                //          LIMIT 1;",
                //        ClinicDatabase.CreateParameterNew("@id", itemId));

                //_selectedImageFileName =
                //    fileNameValue == null ||
                //    fileNameValue == DBNull.Value
                //        ? string.Empty
                //        : Convert.ToString(fileNameValue);

                SetImageUploadOverlay(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string PrepareImageForSave(string code)
        {
            if (_removeImageRequested)
            {
                DeleteOwnedItemImage(_currentImageDbPath);

                _currentImageDbPath =
                    string.Empty;

                _selectedImageSourcePath =
                    string.Empty;

                _removeImageRequested =
                    false;

                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(
                    _selectedImageSourcePath))
            {
                return _currentImageDbPath ?? string.Empty;
            }

            string imageFolder =
                System.IO.Path.Combine(
                    Application.StartupPath,
                    "ItemImages");

            System.IO.Directory.CreateDirectory(
                imageFolder);

            string extension =
                System.IO.Path.GetExtension(
                    _selectedImageSourcePath);

            extension =
                string.IsNullOrWhiteSpace(extension)
                    ? ".png"
                    : extension.ToLowerInvariant();

            if (extension != ".jpg" &&
                extension != ".jpeg" &&
                extension != ".png")
            {
                extension = ".png";
            }

            string safeCode =
                MakeSafeFileName(
                    string.IsNullOrWhiteSpace(code)
                        ? "ITEM"
                        : code);

            string fileName =
                safeCode +
                "_" +
                Guid.NewGuid().ToString("N") +
                extension;

            string destination =
                System.IO.Path.Combine(
                    imageFolder,
                    fileName);

            System.IO.File.Copy(
                _selectedImageSourcePath,
                destination,
                true);

            string oldDbPath =
                _currentImageDbPath;

            _currentImageDbPath =
                System.IO.Path.Combine(
                    "ItemImages",
                    fileName);

            _selectedImageSourcePath =
                string.Empty;

            _removeImageRequested =
                false;

            if (!string.IsNullOrWhiteSpace(oldDbPath) &&
                !string.Equals(
                    oldDbPath,
                    _currentImageDbPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                DeleteOwnedItemImage(oldDbPath);
            }

            return _currentImageDbPath;
        }

        private static string ResolveItemImagePath(
            string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
                return string.Empty;

            if (System.IO.Path.IsPathRooted(databasePath))
                return databasePath;

            return System.IO.Path.Combine(
                Application.StartupPath,
                databasePath);
        }

        private static System.Drawing.Image LoadImageWithoutLock(
            string filePath)
        {
            using (System.IO.FileStream stream =
                new System.IO.FileStream(
                    filePath,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read,
                    System.IO.FileShare.ReadWrite))
            using (System.Drawing.Image source =
                System.Drawing.Image.FromStream(stream))
            {
                return new System.Drawing.Bitmap(source);
            }
        }

        private static string MakeSafeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "ITEM";

            foreach (char invalid in
                System.IO.Path.GetInvalidFileNameChars())
            {
                value =
                    value.Replace(
                        invalid,
                        '_');
            }

            return value.Trim();
        }

        private static void DeleteOwnedItemImage(
            string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
                return;

            try
            {
                string physicalPath =
                    ResolveItemImagePath(databasePath);

                string imageRoot =
                    System.IO.Path.GetFullPath(
                        System.IO.Path.Combine(
                            Application.StartupPath,
                            "ItemImages"));

                string fullPath =
                    System.IO.Path.GetFullPath(
                        physicalPath);

                if (!fullPath.StartsWith(
                        imageRoot,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Image deletion must not block item save.
            }
        }

        private void ResetForNewItem()
        {
            id = 0;
            itemCode = string.Empty;

            unitIds.Clear();
            unitFactors.Clear();
            unitNames.Clear();

            txtcode.Clear();
            txtName.Clear();
            txtBarcode.Clear();
            txtCostPrice.Text = "0.00";
            txtSalesPrice.Text = "0.00";
            txtOnHand.Text = "0";
            txtTotalValue.Text = "0.00";
            txtMin.Text = "0.00";
            txtMax.Text = "0.00";

            dtStart.Value = DateTime.Now;
            chkActive.Checked = true;
            radAvg.Checked = true;

            dgvItems.Rows.Clear();

            _selectedImageSourcePath = string.Empty;
            _currentImageDbPath = string.Empty;
            _removeImageRequested = false;

            RestoreDefaultUploadImage();
            SetImageUploadOverlay(true);

            bindCombo();

            id = 0;
            UpdateInventoryHeaderText();
            ApplyInventoryLanguageLayout();

            txtName.Focus();
        }

        private static string GetDeepestMessage(Exception ex)
        {
            if (ex == null)
                return "Unknown error.";

            while (ex.InnerException != null)
                ex = ex.InnerException;

            return ex.Message;
        }
        private void InitializeReferenceIcons()
        {
            if (picHeaderIcon != null)
                picHeaderIcon.Image = ImageFromBase64("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");
            if (picWarehouseIcon != null)
                picWarehouseIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABkAAAAYCAYAAAAPtVbGAAAE6UlEQVR4nH2WzY8dxRHAf1U9M+9j2dWujb3mw+tg43iJjZEiFCETA0JBCCdBIpFyyoljzjnmxom/IMofkANIUQIHxAEBEigHFCFhPmQFDDYQrLW9n/a+92amu4pDz3trOwktjaa71V1VXV1Vv5amSY4L4LgDAgKIMmvu3v1BEBBhNgEgQp4R/lcrQBABd5ntzX//r8VBFURw45aF/n+Fz5S4G37Louk2vJudWQnmDuaICCKCA96Nf6jtOeVWwz1/U2VI5yIHUUV1zzkhaD7QD+hRUATN1uGd2xSkId+VIq7UN4UP3r/Cpx9uMrnhqIC74Z5A0u1Git9mdaEInRfQYOABEce8ppQ+7sLalYY3//EV7779MWVZcubnJzj3m+PcvawkrxFJuA8RUVwMkU6Bd8aPx2MvihI3AWmJ0SlCSRGEZqR8dn6XV//6Ppe/3OYnp49Q1zWXL25y6J79PP/bRzj96DyDeccsB0QoINKiAopmT9R1cnFHguHmECsuXRyztZ746strvPfOeXa2nDOP/5TnX1ihibD2n13+9uoFvv76Io+dXeXM2aMce2jI/KIQLeIoQafhVCCxcXc3NLRgBZe/aHn5pVeIkyVSAjwQCmVpf0V/DsqeMz+3wO7WAp9f+I5QgId1nvn1YX73+58RLWJeIZIQHPGCAiAEwdwQYNAPLB/cx+72ELMebZxQVkpshBupRoDNtZuIjLjvyBz9fo+6Cdx11yKColrlyPQcRDhIiua44GaoGpaUG+vKpS/GjMfG4lIPDUZRKNUgEJtIoULVV65fq1n7zjn8QMF9R0uqIbgJ5mAkcMVNKJBpQijJIpCYNMpf/vw643qTJ558io/OX2Aw7PPcrx7l4/OfsHkVfvHcSV77+1tsrQvHji/z4h/OUA0D7oqoIiazIFazRHLP2pPiVrK14ax922fl8CqrJ46ycTWgLHHs2D7mhwe58k2Np4pfnjtHr1hk81pBPRZwxTziBiI5SQXQnDY+TejuRE5MNR52aG1CtMj2zggzJyUBjEm9y4nVBYyapnZwISVFKLFErm/iSE73ACKY+ExZVQWqnmBxnqtrESeQmDCeGEiiaYTtjZbJOHFzdx2lQkRRNVzGoAkJEZEE6hRu2pWBmCusQxAo+5F9+5c49fA+bo5OUccdBkOlLCMHlivm5itQWFgYICpYUixFQtlgVuWrFkHFKQIgKiRLBK3YuQ5vvPERWo04/KMBiwecs08fYjI+wLW1HR4+/WNOnoJqkBiNlNXVk3z4wSUu/nubQ/cvkNwJVuACgmWXtW100RZVx+OAf/1zm5dfeoWF4RH6gwINRl23xCbgOqaejAhakrxBvE89qqgngZXjNX/807OsPKi49TsmZfAUAqgUuCVEoT8Hy/eW9CsjtruYJYqeEcoS8wlFlbrKawgtw7mC8bjFdYeNzQ1WWM53kfma77ltzcFRdUQCk5FzfW1EkB6WqYR3CLBoJDPMDFHpqmwgRSh7kYP3DKgGzozjHQKlrd1FwdwIQWY8n1IojxNmkdQMOlpmNCCZK5lpOWiKAmKCEOiSA6RtplK9izJjxm0P3cMhL7coe0YCIgYkRB08MAOtxNz3PC4cz4KQPZjNUDp9leS+qHWCpROg3WPCOgXTF8IdZMT9jndJ50eZHlfY82C6Q6B0iA637/fqNiXfA8M4kSlyR6fMAAAAAElFTkSuQmCC");
            if (picCategoryIcon != null)
                picCategoryIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAFEklEQVR4nG2WTY9cRxWGn/dU3e4ez9CxiYmA2I7t2IoTYVsE5FiBCAQLJMQyG6RIiB/FBhb8CBagKEgsEIkESkIGNCAnSiZG8YzHY89Hz3T3vbfOYXFvT48DZ1V16t73fNZ7SnXtQYhOgoVIgE7tUb8PIpba5W/denkWJ7osLQwsPozlfwGSOm044YFMCOERSODumIyIJegSzjsj/I8so5IMiB7QMBOllC7KgIjoHYgTp7os6CQL6ozE/zEAJuEeT6WwAzUEOGBanirU5/hLaFJv5FRJFh5FgJlYpNO9w2gbJ2WRzHAPgujqtQDWKRP9+ul0KU6KKAn35ZF7dDUREKJtQAalFHLOT4F+WdS2JQjrukIOISI6A0mQDHZ2Wjb+eZ+HW48ICrdu3eD6S2eBQEl4X/PTHRmx8HCRrgjChSUICQXsPT7kk38/odRD7m8e8vbvPqaZOWjG+vXC1esX2NjY4KvnR7z51l2uvDjCEni0FBxLItyIEGpLHUKUAqhGcYb9HefXv3qXT+9tkSwDiUEa8/ylZ2jbKW09YHvriMPDGfPmET/+6SV+/su7rKxC4JgBMjwMQmT6Sxa0XVAO9+7ts/7+DtdevMK3X73G+vonvP69m3zrZqYUmBzAg62ad/98n39tiIcPCvNZYbSawSs8HMtgciIS2ZQo7ohMRJfXtbUBUqIaHXH2XOL4eJ/19X/w+f0VBpXxZH+Xl289x6WrA977yy5nRjcxG6AIwoEkTAUohBI5QsgTRCK865hz50Y89/UxZ8/D/mSPrQe7PNo5YrTiSKJpjM83v+DSC1f54U+u8tobY9bGoORghmwKchQVEuRSWkwZL90FiRBbWwfsPjrg4uUxd+48T+ZnuFdcviK2t2EyOeaV20NE5k9/3GB65HgDUsJyEJEhBJEgIEdAiQ4cc8ITe3tzJoc1e4+nHE7gw4/eYz4zPvh74mhyzGCQeeXV7/KfzcLbv/8rdyeX+c6di1ThDJUIKty7K6qTy+g9e0YhPFHPCmLIcDDmzBm49tJFSi1ycg72Kw4PG5JVeGt4O6aZjWjaloE1NPWQqkqUnjBFH4nUKbwYCvAios3M50Y9h2xDiq9RKAxHhZXVFY4nUNeFq9cu8Oyz52nbICXhXlPKgBLeM3YiE4YToKYrPFC3UywlitcoweODj7n/2YR5PWdtdYVnzq6ys3uBL7b3kU344P1dbtw2XvvaRURLRAE65giCbGYU7+hbJsKduj6m+IxvfuMFLlyEt37xfY4ngTuMxyJlIYO7d+BvV5zf/uYdPvrwM17/wSWaxqgGTt0akJCcDC3CiKgQTtuKyWRKNZyz8zB45w9H5OExgwFYnhPREiXjLnIesL3dEG7sP6nxFqyaUnyIqeNCSeSOBhKl7agdYPyVVfLA2fx0k+2tTaqVOSaDSLiLiKCUGUHLfCpm8yG5GiMD2RAvucPqSTNLmXD1DNoyWEm88aOXaZvMfCpW1yqwOVW1QpQRIlG8ofiUtswgKprpkBu318hDCBmSTs16UD3rthI4c0JzchpRHw+ICCwXZC1ERXjCDDwaUNt1ZDvAwjoDNsPMEaOegTsjZhnM6HkrkdMQEKmCXAlhlNYQHe14BKggFaTAUotyUJh3I5XUj21Ohpiapg4wFKlTqmu/bpAF4ERA9BSBHFlD4BAVIvUj2vthtRzZC8lBOfGAoOObhRNaPCS6B0T0ug4ocfIOEURYDxmdc6em8X8Bubi32jDRCDAAAAAASUVORK5CYII=");
            if (picItemTypeIcon != null)
                picItemTypeIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABsAAAAaCAYAAABGiCfwAAAFbklEQVR4nHWW328d1RHHPzNn714T45CEECfIREmbkBAgCSU4ahEKSKCWPlQVD0ARCAgKIBAP/B1VJR7aKqQPgOANBKiRWtUCAooE4UeApnVIxA/FplBcQmIHY+zdPTN9OGev7RiOdO/duzs73/memTPzlaoyx0lLQBBEwPM9x8FBJBvku70XfnR5z0e7CkGSwwx0vpGKgIC7425LHIrooqBgPjDJ7/XA0sPzIpRkkACkdy2ZYs++58dzmEtXsk0Eih/ZgcRUhBSwIy3zhYH1rmXx3x9C5QfBBPe0oRhYXOxXQ/KlIeW1vW8YjpHSYoCCaw4kMS8W7qmI4ECMoA5vHR7n87Gz4I54gXtNeYHT6XS4dvcGhtYPpO0NjmgEIgJEInhAvKStBBFfyMx7yQxBOfHPWf74hxG8WonLLOLLwAqMBhHn6Nun2ffIHoY2NngssUaR0KHxBpFiAbO0LU5D0ZZ5+jhOBFemzhixLtmwYQ1X7hhkerLDVxP/Y/vOTRw7Nsr4qdMc+NMh7r5/F5dvK1EFc0O9wGJOe5tidxBFF6Sq9yM44opKYPXqAX516+WsWt1HuWyKG3+5kr2PDLPlyrWMj03y1P4jjH54hmrOibGmaSyBWVsoEcQAyWCSQTJLBUIBhXapZvr5218/4Y3XjqLi9C+Hn27t464HdrD5iuWc+vQ7nvj9Qf71/jd408GpEbU26nSM8lFSxBHx3oOgCgT6+pzQqXA9Q9+F0+wYHuC2O4bpH1AkwPqNy7jvoRu46poBps4GnjnwNsfenSZQIqEmhJp03gMqAXejwB1VxQXMBHdHVVAVyj6h01fz69/sZvkKKLognUhsnOgweGmXex/aw5NPHObE6Nc8c+AIRfELtl/XD0VN0OQTETQ4Kgs6QioQBzUuvAiizfLfryY48uYnmIN5xGIFBIgBi5FVg/Do43v42a4tnJ6IPLX/CCf/PY3SIfr3uDsxOu6KOhAt9bygShECqsrFa0uuHd7Ct5POwec/5ZW/j9PMBerZLtYIMRqOEqRkYFXBLbfu4KIV/Xwx/i0fn5xAEGJsek3BzFGfb++ICKJgEhlYKdx5z252XncZ56ZmePn5wxz6xymsUuq5dFYsRmKEyTORw298wMzMWbZtv4Srdw4Ro4F1URGa2jCrkbqO8y3EwTEMQ1wRK5j6Bp5+8h2OvjNKWcL9+37L8A0rcKkpQsl/xmr+8udX+OzjCTZtWs+Dj93EmqFIY7OE4gKwgAQDqZC6WjC58pRwhFin8seN6UnlxedO8urIUTplw917d3PjzVv48vOKp/cf58TxcbZetZrf3beL9ZvBdS61Xe8i6gi6tDfmKsmdpiKEEjdYcbFx+96fUC6b4vWRMV549guYG+K9945zYvRL1m2c5Z5917Dusg5BBaeTZqDm1LTnuJqz89AEAzplQ1M3BC1REdyNmXPKC899xOsjYzRVZGZ2kq3bhnj48Z8zeGkXQsQtgIJIxNURigzmyCJZkMHSCGkQDJECi4J5RCmYPQcHX/qMQyPvM7huJXfdez0bNvfR6RrRnKAB1wgSczMuEjggdWWLd1JAVMDaRpoGp0VJcsGhmnNOfjTO2rWXsGZdFzykeBWcGtWYe1/ACbQSQao588WqwHBxxBJ9lwhSI5S4ac8mRRNxGsT6s60hWic2Hlo6uRZCGjGLVVLuxuJ54hq49sRO2nOjVwFtMdAO0OR4XoA5uOE4xVI5pllrxARm2SGANClSD2CdrA/mm3gCbwdmloMWQCw34iUrJ8YDQkjqwVsNJImxGD3tI/N6w/Ml0mSkkPJmSYcULV/vfQEuiYGkcdMOPJdWN+oCkSWZFbko2rQYTkPSVInt/wFbg7XZo0r4GQAAAABJRU5ErkJggg==");
            if (picPurchaseTypeIcon != null)
                picPurchaseTypeIcon.Image = ImageFromBase64("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");
            if (picItemDetailsIcon != null)
                picItemDetailsIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAEoElEQVR4nJWWzY8dVxHFf6fq9nszNrHHgRgUhA1EQQlEYUwkEgFCSBE78B5FQsqKvwRWLLNgg1ghlmwiKyKLrAAhCDbEikQQIgFDvpTx13j8Xve9VSxuv+cJYCRq1equ7jp1TtW5rVrHBAGOEACpFcSSiAYaaW1BtKQUgYIMZ7FwpikwN9AIVEAod9lGAhYUiLnAHMqejJAKGHgWXGAGobu4G5GBvJK2gnYSckBWSU0Ih7SOPQ1N05jSsU4Sbhwkv/3lu7zz9vtIjXG1RCbkFbPGeiX2nzrPl54+TWYi0z30AmkDFkhRMh0IpCDTocHLl65w6ed/YVoHY71BsQdoEQSVaJXVyvnNr/7M99qzPHFhj2E3kUGGkCAJRGcERBE2k5ed7zT+/tZ1Dm+I/f0nOffILshpDdbr4Mrl17l5fWTQHj964RWee/6rPPP1h1jsNjBDFkADDNLJhEL2EpBkVqQBs2CcbvLUM6f45sWHIZMa4u4dePe9N5E1vn3xy1y69BI/+8mrLHa+yFe+8QmwNe6iteyaIDITSxIyiTCyGaRo1WmtJ9eaBCtSa8YRxnVweKvy9rUDzp97jDu3nZde/D3v/bMidoi2g3LRuwiBJkrSeh/NIQMErUKGE60P9TQFNoAPyd5HnQ8+EL+7/GuGsoO04OjQuHM7IUUmgG/oAbVO14a7jVaRlYjoU4JYDjuoJHY6+c539zl4/4hhUajjDi/88FUy7yILMhuSEwE5b58EfRGyI05ZB2CJe0EuDg7u8osX/0Tg7JzcYWd5gvXdyuNPfIzz505Syoo2TWSIyIrSie1gJWKgSDnvTSIC0hkWBS8wLIzDo+tcff01dk+cJK0hkkzx4Nkv8Onzn6fVpCW0KiIqpBPpIDCvCOtF0AiK3lE4Q3GGJcgqj37uYb7/g+cIqSObly1T3LwOrRnZnDaJbF2TRiJLsjUkm+nSGpmhWEKC2cBiGSyXzjv/mPjpj1/j8LDR8ohTZ5JvXXyazz5ygpiA5si801SFUeaNn2Zb2WqyQAkykQIvgZeGefLAnvO1Z89y57ZhbgyLNaf2gqlOtBhwD0ITWAWrZA7z1mv2L6NsDWfjmglmBWFEM4o5+xc+RTZoDVpARDCtICqUMlBpmAdm3bJaJmaGerVeRNrML/MoFyKcV15+gz9eeZM6ObVNtFa7uIyYDOI0t26u+cgZcAPJIRLzDlqzgAUFYoCEVKIinrzwca5efourf7hGnRZkc+TrbnzpSIFJSLfIEJ95bI8HH9qlLEamcYV8ibDtnmgaM1HrQhFkDGQd+Osbh1z7221cu0AQKcBQCqkd47bx6ONnOPtJB5swH2iVXkRdA43rSGwEptkOChnCLMkwhJEZRPo9ZNvjozt3ZMNLIVNdj/nQ2zwvUvepZLE9tNwBjsAGaA44ZkFa3Z5MytKtQw1TIWN2i0ywexpDbqbLZsU1e1ciFv2+sRVRKsdnESSSgmTH7s9/CluDPD7CmwnTJu3Yo81FiuPvbwEeiw0b20hRNmvZfxzE/SK7h//PnPtF+b9e1IevRWd5w/S/tfjhIvdiS+R/fv84EP2XjPsUAPgXcsZc7/c0nOwAAAAASUVORK5CYII=");
            if (picPricingIcon != null)
                picPricingIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABoAAAAZCAYAAAAv3j5gAAAFeUlEQVR4nGWW229cVxXGf2vtPZeMx/UldmI7TpzExA0UZBkRqUJQRKUqQgUpD1RI0Jc+8B/wzr/CS0FqH6hEeaiEKESUPqU4CVYSu4mvSerajm/xZeacsxYP+8zYwH6ZObNn7fXttb71fUfa7czxAAjguDuqSloO5LhkWBEIWgUR3A087QqCmaRoA3PQACKGCDiGIEjWdgdwL88FjJwQChxDNWK5oKqICG6CW4lDHTcQgXZWAI65o6KoCl284kQ63+XkU0TRkGMWUK/QPoYnCy9ZXtxj8+sDjo/ArKDeEEbH60xeG+T8WC8aHJETxO6dc4UIBpIDCt28TlE4QSJ728YnHz/hb3/5F8dHBWZtHCgyIcuUGCKjF/p56yfTvP7DEaqNFogiEpASveBEpKyX5IgogmIGRRFQVR7ObfDB+5/y6tSrzPx4ktGLTc40KnhR48Vmi/kHG9z7YpkP3v+ciSs/ZWLK0VAgaNl3AREiFkACSAskAxR3Qa2KAMdHLSzr4fLVXn508xzNs4ZoFUXwvMHka32sLFVYWsxot4yiiGhs4QLi9UQEINLtjaT+IIjmFOQYVfr6azR7ern913kWn6xz6coFzo+eI2iN7c0D5u4uMTe7xcCQEisZldhMvcFBDNEcCU50skRSFxwBFPFIJQTchP6+Qb7/g2kEQ+Mx2/sbLHz5gBdbR2THyplaD5cu9zI02kP/YAMH3AKiiQwuCgVEp0CIuIfyksLxgTI/t8f8wxXWVr9ibzvQ0zjD6IV+ZqauMjwyzbOn22x8tcvk5XEezG3w4NEXvNw/YOBcDxoEPM2lFRFBiBT1bs9QsBz+/McF/vThLDFWqdTaqAQO9jf47PZDQjRmXr/Ie79+g+rMBB/+bpnZO8/Z2ttjd/eQS6GJqIM7ToG74R6JifdSMt9wF5aWlvnG9bP8/BdvMjic9vMW7O4esrL6jLn7q9z++11uvnWDldXHjF4UpobGGR2vEyvlQMuJALhDdMkRKoBhZuSZsrNzRPA6MUKjCfVaCuwfajB4bpKDvTM8XpxFxbk2dYGR0SHOX4SBwd7yfCnJACKOYURQnDbuipvQbjkrixu8WHfWlj/i2vUh+gaEStU5PGjzfO0lz5/tMz5RQ4BGT5V/3n7E1t48v/nt21y9PowVJAkKClKA5EQRS+woImBU68Ktd27w2aeP2dky7t1ZQ0JGtabUa3Vq9SoTV/v57vcmUFVGRho8lIBKIMRSdtTL+QndwY14TNQNnspXBNafZnzz+ne4ceMyO/svODo+REXpaTRpNl9hfX2Po+NN/n1viz/8/iP6XjnLzbdnGBkbRIKhWCntASfpXwRBNZQMEbxwnnz5NWursywu3+e16TGGh0awXFl7us7Ttbvcv7fAyGg/7/7qFj+7dZNvT48xca2K1gE1wNJcegFiII5kWZbm2DRd1+Hzf2zwycd3WF16yf5OwPMGlgtuLWL9kIFh4403v8U7v5xGq6AxlQtJRFBSdU4vabfdBcNxQAkqWA7bmxlrq0csPHrOxvoulVClVlPODvdwZXKYsfEmjV7FpEg90SSegibT63C7m6jlDoZTIOJJ2l1RDVgBeZacU5xkZJ3hFqewHFEQlRTXtYWOO5+sU8bXmTDDxTE3UCVUk5wE9WThCGZg5sRqPHFbt24sXc0+lUi0k1lLi6CkZKJqCGAFFIUQSm8W0u0sB+na9QlQCODSNb6yR3nH+bqimh47luzdXfdOoPyXxHSWdzXn5JgOsJj2UgIXOVXfVArE0hEeSvjePTb9uWyca6nYnW0/yYKg3sUhpxCWgSh4clw8nPzWBdMBdOoV6n9WJ3U8/fB/f7GAE8qidMolZQ87N+ogL5OdAtt5AwL4D5NBg99Q42JwAAAAAElFTkSuQmCC");
            if (picValuationIcon != null)
                picValuationIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAYAAACpSkzOAAAEZElEQVR4nG2WP48lVxHFf6fu7Z63s7LxwsqbGZkVAlInfAYc8RH4HiQIiZCMnIiMyIiEyLElApAQQgSsBLbE4vXa7Kxn3uu+dRzc26/fYDp409N9u/6dU6dKx2MzGMTXLhH9xr58iGSw8fjINtIw4P73/P84U0PC+/t7l8fP+Z1BBikwxtkD1BalweQw7ntG6zhJd/f/nW2GjEkDq0CXxr1Hxf1MLhyNAujrKWVeOuyBRAiv4Lw8OTLQVrb92Wa29voOjKx7kUTs9x6RB+L581ue/ePfvH59x5Mnj/jOd98mSn8vdXwko9Dwb2qPdkFeEdfYArI72aqpBAXZIBp89OHH/ObXfyR4yKMnCz/9xft88+0ZRw/YCpCJgSmRg1Y543aNWwzjiVkG8j3TEKAVCW5e3XHz34XlOPHyxS3LKUfW3oFFIGMlbBnhuAB++0lgAU+0Bi0TsyJPPdoS2D3zS3jtgbk9mLkSFGov6gqs4IopkAEOrAYUci0QO9Msd6BLYc0coOuid3oVeoIVZ1BxL6QNmQlZEcLMRIEvPoW//Ok/ZIO3vjXxg+8fmOpMLROlBLUWAEoZDR7GOaojIxrrslIlka641V4qNUxgG6f4+18/5Ve//B0RM4eHX/Kzn//kzOhS45xoKeeakxp3DrAoJTu9yUBASwONzAZ0HI63Ik9vQVxxs97Slm5Q6vQvVShEDLVKQwxO6Sw5pTtKJ60lUqEttRsT6HBLayumYa+gdZBiSM+mezJEh5E06a4lISMndumsUzTk5LPnRz747d/44gXMhxM/+vG7tBa0thJ1Yjkuvf4juE2+tksyEUbZqxaIIHDSMbJNicLnL1/zh9//mZuXB6wXvPfDx7h9A6cgoa2Q2WnbWpJunUAXjkyvTPyPftZdfUUtE2++8ZiyPmRFlDKRDUrMZEKUgi3sJG2yGTtHB16OksuxkRCmpntzlQCFqVNDZYVMhCjTAL4EXiFlUgstg5ZBcSFToBwCXoHeLvY+0wIgNEEWNBQi6jjgK6ZaOTy4ohSI6HQlD2TrJc/cHBTIwg6ah8PeuFUhyE7P6Sp485FZ1xvm65U6JzQzHV4RPrEeb4maTIeV+foVmoP5+paITevGMNY+lDNbV43jkg4twMpyLPzz2R3LccY+8c7Twnr3kH89e4W5RZF8++ljXt99xifPQMxMhyNPv/cGV4fSs3LdJQi6PkroeGqOSKQkmDgdxekI8wSqJ8TMeoQ6m2yizmZpd3h9QCnQsqFyRykTdkFEZ5+7alhjJbk7NvchZfrOIUoU0r2/cIxJ8SXKB+TYA9x6xIoxz1RxjqVEK7jQWydB6n0EwtlnR0QbvaDuRIAWpAV0hdowGBuXe9/g++MCNex9ixoNOzTJQap11XX/2KKP5KjQSmdYrMNB9ogdoNox6l4G7/ZNqIvqvY1FFz2wdbcHyNr0f9zHeWhqyNE5aLXxbIhB3256HftH2qfseawncux7pGNrwXF2Vwah/dxmyPAVkkeRZn6bfB0AAAAASUVORK5CYII=");
            if (picAdditionalIcon != null)
                picAdditionalIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAYAAACpSkzOAAAFdUlEQVR4nG2W229cdxHHPzO/39mLLzVOnI2TQClqGsnkBgKkCkMpFGjLpUiQJ5QXxBN/Do8IqU/AC4gHQKp4AQWSlHJp2lBCEqU0jmMndl3b9XV3z/nN8PA7a6+b7Gqlc347Z+Y735n5zpHKkgOAgyRwRzzgLohAsoSnCALigiVBQkUoKswMlTaIgxggOI6bIgJQAgWYEt1BULw2U3HcNT8oStACN2F3J9HfNbxyYstpjUWKFqSqREQAAUDUcMg+BmdAdBN8kJCCS/5LPGIVbKzBnVvrXLl0jTu3lllf7dI51uTZL59h9rnTHDkakeiogqpQlQaagUNmBXek13fPRyUiCVHAG1hf+c9b6/z6V29y939rpLLEywK3Ucx7FI3AyLjxwssneemVGUaeEFTBvE5BHBHL4cSRXtc8Z1gBFe6C0uKfV9f5xauXWF0yGq3I9PQIY6NtdrchNozVFWf5QZeinZh9/gQXLp5lbLKPSAQUNJcCTagYUURwB7zARQgSWJjr85tfvs7S4iZHp8f41vfO8LnPP8X2lnD50m2+/tIpPlwree13t7j2j0Uu//k2T0wGvnvhGRrNhETJdRPJDAEqUmcqoAQE4Y0rd1m416PZbPDNb5/lxVc+xfSTwsraA67+7QqLy3OcPF3wo5+cYebcIVKKXL10g6XFbQRFNQdQBREhaETREgkVqKGxots1blyfp+oJTz9zgtmvPA1k7peWtultT7FwzzCc0Ul47hvTjE8kNja6vHtrlUYIeMpZIIaoIyKoIKgqbgpWsLnuvP+wJETns184zmQnt2sy6Bwf4asvzjBzrkNlFclKTp46zvT0EcqesDi/jjuIOmBAwj1h5qi5AooZmEG/B2WpiAZGx5qUaZMq5YwOHf4YK2vzVFWXoBE3pdWGZqtJSrCz20cjuFPPUh5eM0fzRZ4jc0cjIBX9vrO1lYA2iKEBdnb6/Pvtm9yfX0ZFaBSBnd0tNjdybdrtJskzYDcwM1KlVBUoJNwcEXAqRseFyalIVTl3bq7Q3WwQtSQGh+Q0wwRlF4Jk5AtzO3zw/g6iwuHOaJ4jHDOnqoSUEpYCihtmCcRwT4yMKmfPf5xWy7h9Y4F33lxFvIklGJ+AzjGh04kEhWrXufynOXa2SqaOtJg53cGswujjbogXQMh90e9X7ua4C8lKCm3xcN742U//ynvvPmSqM8KFH85y7vwhQnQ+XCmZmIysrTqv/eEar//lPuYVL7x8ih9cPAuxm6XMI3iRmXKyMog4ZiD1NwA33t7l1Z//kQf3dwla8MknJzl+YoJWs8H6eo/5uVU+WNkiWZ8vfe0TXPzxF2mNJ9BAsizI4oNpNaTf8/01MWh/dzDl5jsb/P631/nv9RW62w4GKZWoREJRMTqRmH3+JN/5/nkOTzf2dM6xrAJei6v0hwIJgCEYqo5Z5nh1Gf71xhJv/f0u9+8tU5V9imbk1MxRnp19ik9/5hjt0bwBsvp7/nkuBy4gFVKW7rKnuBVOwh1CgLwTC0iBzTXYWEsgSozO5JTSaCdcDKVANXNiWu0xM1g/uCBl5UOcVYAjFAiQUhcJJW6GmYC3EG9Qb0lidIwSEcXxDFZDHhdqscbBFeknc0jgWVAhD9tgEZpXIHkuVLIaaL0CzAxVw4mg5KAK4op4yDvbc0niAJ0c7IfclkbeLw5Bc5tqAPOEIGgIuRa5A1CNNUrZ465esEjZr2XhI4EGB3vp1/tl3/Bx1/ugxWG/KE6s+3HftT8SbSjloftB1i4HTQfD4gft4yMu89vEI04zRT4UV/gopmGQud4D+8cGysGGpR53XGTIyeM/UttITbf7PtvxQL41yoPIyHIyhG440AHbgdeaUpE6GPB/tvv5vl6VL6EAAAAASUVORK5CYII=");
            if (picAccountingIcon != null)
                picAccountingIcon.Image = ImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABsAAAAaCAYAAABGiCfwAAAFRElEQVR4nH2W3atdVxXFf2POtc85yc1NjbEGsdA2aVr6kQRKxQcLIvjal+qLiiAIgi/9E3z1yf4LIuIH9E2RWn2IKIVWkEa0FFFs1VjbpsYk7c25Z++15uzD2ie5tw0uOLDO2muvOdaYY465NbUpMxyFSFWkRrZCRiJBZOJWqFVgDSsbhrIgQpCOGbTcByoyETFAFgBQg7R5npRoTiYoASYyRN2I9V4QDdxANqJ0EvBhYHJY7TihhExMhQQyAhAoyAQSoPUloEQAGWTfxrgu/OkPV3jux79hvW6M4wYRSAOZRovKyU/s8u1nvsR9D4rMIEO4r8iZiaQijIwCaogGSnRzPSUEAjKDm9dWfPc7z3P59QnzhtlEsR3QhnEU2XZpeZ17Ti/4+jc/z4UnjiOHCCAA6/RlCJqRglQDktL5TVAFNF+/sDo68tTTj3P2kWNkm4iAv762z69+8Rcefuws165d50ff/x3T9DkufOY4PlRsMJSFTAf1dHXmhGQUWQINUwP1YG4L3Nfce2aXRy6coE4QLanjDaY6cubsCR579BzPfu95fvKD37I88lnOP/5JMhtJA7yLwCYIAU6dgiILpP7rDyATIhISxnFk3HfkwVgn3I2LF3/P63+/Cum8/e/3+fOlN3j03Cl8cBoTSkMWYGsUC6IJKJibYeaYCrUFUtJyQ4vaEbJAXkjbsHOXcd/9d+P5cf5z+QbL4SjOEiKxBPUEdfUJREEmzAoSFNkE6RALlsPAeyHMGlLFLNncFPs3QbbL2dPwrWdOYJasluLiL6/y0x++hGJFBrTWkBekiuRkHIUIWusVMFefQUILkIEP29oovPjiK/z6hT8ylBUghgHuPuV89StfJm2iNZiqeg3KCXoNdaH1fJngvfehKB1SpLpQsJ4fLwPIePChM8AJYnKmDRDJsKoUh9qC2ir7YyWSOd+JYi7mBNJwh3+88SalR99aS2CUzrUqQcVLYVqvyDQWi2D3uHjgoVMc2YHa1rTWaBVqBWkffIWazQqvwAICXnv1LXrmslOZ0emMOhCxAYK337nMSy+/ymIYGMc9huWG3RNPcvLkaUwG4Yz7ok6wWDgtwHAinUZQHN69MvHKpUvbnHWOc6YCSzIb0eD8+Qe4/94zZJvLgWD3rkIxKFp2W0rDBLU1VCBz6KDTaIJN3ePhc5+m5OyYmRVJBMbUrlPrhn/98wY7uzDFut8+HFfh+v8axhGu/neNeZCMXSCepE0dlBfIJKic+tSKr33ji5TMNqvRu6EaLBfH2LuR/Py5v/HCzwRUUGJynALaYBI39wJ5slz1ViMGUiOyCplIpVsgE7KkBAHRbRKD1Q489fQTTNPLXHnrXTbVGYrjJXEbcAawCWni2DK558xxnvzCaYZVEiaIVfdZzZ7IMM8Tbab9ZDbPzEqLituSd97csL8WbgtMSSngbvhcRzIYp5GdYwO7H4NQpdbs6rPsrSUdIaTeMDXWTRIDIFoDeTCNG9zLrUYIiRkUHzAMrBExYRporbeQCDBbELV3ZTT1gOrohNBYx+yt28hZjomI2tG3mDAXLsPw7nk+khnAgHAietMMGm4T5KJbIFt5d9yF1PxiIiBSvX6sl0LxYX7JuiEkKGzuktZLBkDCrSCNZDsQ4WC8cayHw28d5cAIJZqtfG5CbD8strs7KR0weXDt9iiSbtG3PfDDwz6yNv/P29ODN8kPR9kGu+3OzIm907btffI2Lerg7nSuPgKubyqHabv1/XWHYHnoUcadgXUAh8/YKrIcXvh/Q7NnH6B8i+MApO4cM6DcXqCD/QArQ8vUqGk4SQAAAABJRU5ErkJggg==");
            if (picItemPlaceholder != null)
                picItemPlaceholder.Image = ImageFromBase64("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");
        }

        private static System.Drawing.Image ImageFromBase64(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            byte[] bytes = System.Convert.FromBase64String(value);

            using (System.IO.MemoryStream stream =
                new System.IO.MemoryStream(bytes))
            using (System.Drawing.Image source =
                System.Drawing.Image.FromStream(stream))
            {
                return new System.Drawing.Bitmap(source);
            }
        }

        private static void ApplyModernInputTypography(System.Windows.Forms.Control parent)
        {
            if (parent == null)
                return;

            foreach (System.Windows.Forms.Control control in parent.Controls)
            {
                Guna.UI2.WinForms.Guna2TextBox textBox =
                    control as Guna.UI2.WinForms.Guna2TextBox;

                if (textBox != null)
                {
                    textBox.Font =
                        new System.Drawing.Font(
                            "Segoe UI Semibold",
                            9F,
                            System.Drawing.FontStyle.Bold);

                    textBox.ForeColor =
                        System.Drawing.Color.FromArgb(49, 56, 83);

                    textBox.PlaceholderForeColor =
                        System.Drawing.Color.FromArgb(165, 171, 190);
                }

                Guna.UI2.WinForms.Guna2ComboBox comboBox =
                    control as Guna.UI2.WinForms.Guna2ComboBox;

                if (comboBox != null)
                {
                    comboBox.Font =
                        new System.Drawing.Font(
                            "Segoe UI Semibold",
                            9F,
                            System.Drawing.FontStyle.Bold);

                    comboBox.ForeColor =
                        System.Drawing.Color.FromArgb(49, 56, 83);
                }

                System.Windows.Forms.DateTimePicker datePicker =
                    control as System.Windows.Forms.DateTimePicker;

                if (datePicker != null)
                {
                    datePicker.Font =
                        new System.Drawing.Font(
                            "Segoe UI Semibold",
                            9F,
                            System.Drawing.FontStyle.Bold);
                }

                if (control.HasChildren)
                    ApplyModernInputTypography(control);
            }
        }

        private void LayoutModernInventoryPage()
        {
            if (pnlPageHeader == null ||
                panel1 == null ||
                flowContent == null)
                return;

            int headerHeight = pnlPageHeader.Height;
            int footerHeight = panel1.Height;

            int contentHeight =
                this.ClientSize.Height -
                headerHeight -
                footerHeight;

            if (contentHeight < 100)
                contentHeight = 100;

            flowContent.Location =
                new System.Drawing.Point(
                    0,
                    headerHeight);

            flowContent.Size =
                new System.Drawing.Size(
                    this.ClientSize.Width,
                    contentHeight);

            flowContent.AutoScroll = true;
            flowContent.WrapContents = false;
            flowContent.FlowDirection =
                System.Windows.Forms.FlowDirection.TopDown;

            // Makes the lower Accounting fields reachable even on
            // smaller embedded screens.
            flowContent.AutoScrollMinSize =
                new System.Drawing.Size(
                    0,
                    780);

            pnlPageHeader.BringToFront();
            panel1.BringToFront();
        }



    }
}
