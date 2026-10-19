using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace YamyProject
{
    public partial class frmCostCenter : Form
    {
        // ============================================================
        // FIELDS
        // ============================================================

        private int id;
        private int projectId = 0;

        private int selectedIndex = 0;

        private EventHandler editMain;

        // ============================================================
        // LANGUAGE FIELDS
        // ============================================================

        private bool _isArabic;
        private bool _lastArabicLanguage;
        private bool _languageReady;
        private bool _applyingLanguage;

        private Timer _languageTimer;

        // ============================================================
        // ENGLISH / LTR LAYOUT STORAGE
        // ============================================================

        private readonly Dictionary<Control, Rectangle> _englishBounds =
            new Dictionary<Control, Rectangle>();

        private readonly Dictionary<Control, AnchorStyles> _englishAnchors =
            new Dictionary<Control, AnchorStyles>();

        private bool _layoutCaptured;

        // ============================================================
        // ENGLISH STATIC TEXT STORAGE
        // ============================================================

        private readonly Dictionary<Control, string> _englishTexts =
            new Dictionary<Control, string>();

        private string _englishFormText = "Cost Center";

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public frmCostCenter(int id, int _projectId = 0)
        {
            InitializeComponent();

            this.id = id;

            // --------------------------------------------------------
            // INITIAL LANGUAGE
            // --------------------------------------------------------

            _isArabic = GetGlobalArabicLanguage();
            _lastArabicLanguage = _isArabic;

            // --------------------------------------------------------
            // EVENT HUB
            // --------------------------------------------------------

            editMain = (sender, args) => BindCombo();

            //.MainCostCenter += editMain;

            // --------------------------------------------------------
            // PROJECT
            // --------------------------------------------------------

            if (id == 0 && _projectId != 0)
            {
                projectId = _projectId;

                radioSub.Checked = true;
            }

            // --------------------------------------------------------
            // DESIGN
            // --------------------------------------------------------

            ApplyModernDesign();
            UpdateCostCenterUI();

            // --------------------------------------------------------
            // EVENTS
            //
            // -= / += prevents duplicate event handlers if the
            // Designer already contains the event.
            // --------------------------------------------------------

            this.Load -= frmCostCenter_Load;
            this.Load += frmCostCenter_Load;

            this.Activated -= frmCostCenter_Activated;
            this.Activated += frmCostCenter_Activated;

            this.Resize -= frmCostCenter_Resize;
            this.Resize += frmCostCenter_Resize;

            this.FormClosed -= frmCostCenter_FormClosed;
            this.FormClosed += frmCostCenter_FormClosed;

            this.radioMain.CheckedChanged -= radioMain_CheckedChanged;
            this.radioMain.CheckedChanged += radioMain_CheckedChanged;

            this.radioSub.CheckedChanged -= radioSub_CheckedChanged;
            this.radioSub.CheckedChanged += radioSub_CheckedChanged;

            // --------------------------------------------------------
            // LANGUAGE TIMER
            // --------------------------------------------------------

            _languageTimer = new Timer();
            _languageTimer.Interval = 250;
            _languageTimer.Tick += LanguageTimer_Tick;
            _languageTimer.Start();
        }

        // ============================================================
        // GLOBAL LANGUAGE DETECTION
        // ============================================================

        private bool GetGlobalArabicLanguage()
        {
            // ============================================================
            // LANGUAGE SOURCE 1
            // Check the parent chain.
            // ============================================================

            try
            {
                Control current = Parent;

                while (current != null)
                {
                    Type currentType = current.GetType();

                    System.Reflection.FieldInfo field =
                        currentType.GetField(
                            "_isArabic",
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);

                    if (field != null &&
                        field.FieldType == typeof(bool))
                    {
                        return Convert.ToBoolean(
                            field.GetValue(current));
                    }

                    System.Reflection.PropertyInfo property =
                        currentType.GetProperty(
                            "IsArabic",
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);

                    if (property != null &&
                        property.PropertyType == typeof(bool) &&
                        property.GetIndexParameters().Length == 0)
                    {
                        return Convert.ToBoolean(
                            property.GetValue(current, null));
                    }

                    current = current.Parent;
                }
            }
            catch
            {
            }

            // ============================================================
            // LANGUAGE SOURCE 2
            // Check all currently open forms.
            //
            // This is important because frmCostCenter can be opened as
            // a child form whose Parent is not the main language form.
            // ============================================================

            try
            {
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm == null ||
                        openForm.IsDisposed ||
                        openForm == this)
                    {
                        continue;
                    }

                    Type formType =
                        openForm.GetType();

                    System.Reflection.FieldInfo field =
                        formType.GetField(
                            "_isArabic",
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);

                    if (field != null &&
                        field.FieldType == typeof(bool))
                    {
                        return Convert.ToBoolean(
                            field.GetValue(openForm));
                    }

                    System.Reflection.PropertyInfo property =
                        formType.GetProperty(
                            "IsArabic",
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic);

                    if (property != null &&
                        property.PropertyType == typeof(bool) &&
                        property.GetIndexParameters().Length == 0)
                    {
                        return Convert.ToBoolean(
                            property.GetValue(
                                openForm,
                                null));
                    }
                }
            }
            catch
            {
            }

            // ============================================================
            // LANGUAGE SOURCE 3
            // Application language setting.
            // ============================================================

            try
            {
                string culture =
                    Clinic.Properties.Settings.Default.ApplicationLanguage;

                if (!string.IsNullOrWhiteSpace(culture))
                {
                    culture = culture.Trim();

                    if (culture.Equals(
                            "ar",
                            StringComparison.OrdinalIgnoreCase) ||
                        culture.Equals(
                            "Arabic",
                            StringComparison.OrdinalIgnoreCase) ||
                        culture.StartsWith(
                            "ar-",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    if (culture.Equals(
                            "en",
                            StringComparison.OrdinalIgnoreCase) ||
                        culture.Equals(
                            "English",
                            StringComparison.OrdinalIgnoreCase) ||
                        culture.StartsWith(
                            "en-",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }
            catch
            {
            }

            // ============================================================
            // LANGUAGE SOURCE 4
            // Current UI culture.
            // ============================================================

            try
            {
                string uiLanguage =
                    CultureInfo.CurrentUICulture
                        .TwoLetterISOLanguageName;

                if (string.Equals(
                    uiLanguage,
                    "ar",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (string.Equals(
                    uiLanguage,
                    "en",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            catch
            {
            }

            // ============================================================
            // LAST
            // Keep the current form state.
            // ============================================================

            return _isArabic;
        }

        // ============================================================
        // TRANSLATION HELPER
        // ============================================================

        private string T(
            string english,
            string arabic)
        {
            return _isArabic
                ? arabic
                : english;
        }

        // ============================================================
        // KNOWN TEXT TRANSLATION
        //
        // Supports BOTH English and Arabic as input so that switching
        // repeatedly never gets stuck in one language.
        // ============================================================

        private string TranslateKnownText(
            string text,
            bool arabic)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            switch (text.Trim())
            {
                case "Cost Center":
                case "مركز التكلفة":
                    return arabic
                        ? "مركز التكلفة"
                        : "Cost Center";

                case "Cost Center Setup":
                case "إعداد مركز التكلفة":
                    return arabic
                        ? "إعداد مركز التكلفة"
                        : "Cost Center Setup";

                case "Create and organize your school's accounting cost centers.":
                case "إنشاء وتنظيم مراكز التكلفة المحاسبية للمدرسة.":
                    return arabic
                        ? "إنشاء وتنظيم مراكز التكلفة المحاسبية للمدرسة."
                        : "Create and organize your school's accounting cost centers.";

                case "Cost Center Type":
                case "نوع مركز التكلفة":
                    return arabic
                        ? "نوع مركز التكلفة"
                        : "Cost Center Type";

                case "Choose the accounting level":
                case "اختر المستوى المحاسبي":
                    return arabic
                        ? "اختر المستوى المحاسبي"
                        : "Choose the accounting level";

                case "Main":
                case "رئيسي":
                    return arabic
                        ? "رئيسي"
                        : "Main";

                case "Sub":
                case "فرعي":
                    return arabic
                        ? "فرعي"
                        : "Sub";

                case "Parent Cost Center":
                case "مركز التكلفة الرئيسي":
                    return arabic
                        ? "مركز التكلفة الرئيسي"
                        : "Parent Cost Center";

                case "Select the main cost center":
                case "اختر مركز التكلفة الرئيسي":
                    return arabic
                        ? "اختر مركز التكلفة الرئيسي"
                        : "Select the main cost center";

                case "Edit":
                case "تعديل":
                    return arabic
                        ? "تعديل"
                        : "Edit";

                case "Code":
                case "الكود":
                    return arabic
                        ? "الكود"
                        : "Code";

                case "Auto generated":
                case "يتم إنشاؤه تلقائياً":
                    return arabic
                        ? "يتم إنشاؤه تلقائياً"
                        : "Auto generated";

                case "Name":
                case "الاسم":
                    return arabic
                        ? "الاسم"
                        : "Name";

                case "Enter a clear accounting name":
                case "أدخل اسماً واضحاً للحساب":
                    return arabic
                        ? "أدخل اسماً واضحاً للحساب"
                        : "Enter a clear accounting name";

                case "Make sure the cost center name is unique.":
                case "تأكد من أن اسم مركز التكلفة فريد.":
                    return arabic
                        ? "تأكد من أن اسم مركز التكلفة فريد."
                        : "Make sure the cost center name is unique.";

                case "Save Cost Center":
                case "حفظ مركز التكلفة":
                    return arabic
                        ? "حفظ مركز التكلفة"
                        : "Save Cost Center";

                case "Close":
                case "إغلاق":
                    return arabic
                        ? "إغلاق"
                        : "Close";

                case "New Main Cost Center":
                case "مركز تكلفة رئيسي جديد":
                    return arabic
                        ? "مركز تكلفة رئيسي جديد"
                        : "New Main Cost Center";

                case "New Sub Cost Center":
                case "مركز تكلفة فرعي جديد":
                    return arabic
                        ? "مركز تكلفة فرعي جديد"
                        : "New Sub Cost Center";

                case "Save Sub Cost Center":
                case "حفظ مركز التكلفة الفرعي":
                    return arabic
                        ? "حفظ مركز التكلفة الفرعي"
                        : "Save Sub Cost Center";

                case "Please select Main Cost Center or Sub Cost Center.":
                case "يرجى اختيار مركز تكلفة رئيسي أو مركز تكلفة فرعي.":
                    return arabic
                        ? "يرجى اختيار مركز تكلفة رئيسي أو مركز تكلفة فرعي."
                        : "Please select Main Cost Center or Sub Cost Center.";

                case "Please select a Parent Cost Center.":
                case "يرجى اختيار مركز التكلفة الرئيسي.":
                    return arabic
                        ? "يرجى اختيار مركز التكلفة الرئيسي."
                        : "Please select a Parent Cost Center.";

                case "Please enter the Cost Center name.":
                case "يرجى إدخال اسم مركز التكلفة.":
                    return arabic
                        ? "يرجى إدخال اسم مركز التكلفة."
                        : "Please enter the Cost Center name.";

                case "This Cost Center name already exists. Please enter another name.":
                case "اسم مركز التكلفة هذا موجود بالفعل. يرجى إدخال اسم آخر.":
                    return arabic
                        ? "اسم مركز التكلفة هذا موجود بالفعل. يرجى إدخال اسم آخر."
                        : "This Cost Center name already exists. Please enter another name.";

                case "Unable to find the selected Parent Cost Center.":
                case "تعذر العثور على مركز التكلفة الرئيسي المحدد.":
                    return arabic
                        ? "تعذر العثور على مركز التكلفة الرئيسي المحدد."
                        : "Unable to find the selected Parent Cost Center.";

                case "Select a Parent Cost Center first.":
                case "يرجى اختيار مركز التكلفة الرئيسي أولاً.":
                    return arabic
                        ? "يرجى اختيار مركز التكلفة الرئيسي أولاً."
                        : "Select a Parent Cost Center first.";

                case "Select Level 1 First.":
                case "يرجى اختيار المستوى الأول أولاً.":
                    return arabic
                        ? "يرجى اختيار المستوى الأول أولاً."
                        : "Select Level 1 First.";

                default:
                    return null;
            }
        }

        // ============================================================
        // STATIC TEXT CONTROL CHECK
        //
        // Do NOT include TextBox / ComboBox because their Text is
        // user/data content and must never be restored as a label.
        // ============================================================

        private bool IsStaticTextControl(Control control)
        {
            if (control == null)
                return false;

            if (control is Form)
                return true;

            if (control is Label)
                return true;

            if (control is LinkLabel)
                return true;

            if (control is ButtonBase)
                return true;

            if (control is TabPage)
                return true;

            string typeName = control.GetType().Name;

            if (typeName == "Guna2Button")
                return true;

            if (typeName == "Guna2CheckBox")
                return true;

            if (typeName == "Guna2RadioButton")
                return true;

            if (typeName == "Guna2HtmlLabel")
                return true;

            return false;
        }

        // ============================================================
        // CAPTURE ENGLISH STATIC TEXT
        // ============================================================

        private void CaptureEnglishTexts()
        {
            _englishTexts.Clear();

            _englishFormText = this.Text;

            CaptureEnglishTextsRecursive(this);
        }

        private void CaptureEnglishTextsRecursive(Control parent)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                // HeaderUC manages its own language/alignment.
                // Do not let frmCostCenter change its internal label alignment.
                if (control is HeaderUC)
                {
                    continue;
                }
                if (IsStaticTextControl(control) &&
                    !string.IsNullOrWhiteSpace(control.Text))
                {
                    _englishTexts[control] =
                        control.Text;
                }

                if (control.HasChildren)
                {
                    CaptureEnglishTextsRecursive(control);
                }
            }
        }

        // ============================================================
        // RESTORE ENGLISH STATIC TEXT
        // ============================================================

        private void RestoreEnglishTexts()
        {
            if (!string.IsNullOrWhiteSpace(_englishFormText))
            {
                this.Text = _englishFormText;
            }

            foreach (KeyValuePair<Control, string> item
                in _englishTexts)
            {
                try
                {
                    if (item.Key != null &&
                        !item.Key.IsDisposed)
                    {
                        item.Key.Text = item.Value;
                    }
                }
                catch
                {
                }
            }
        }

        // ============================================================
        // APPLY TRANSLATION TO STATIC CONTROLS
        // ============================================================

        private void ApplyTranslationsToControlTree(
            Control parent,
            bool arabic)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                if (IsStaticTextControl(control) &&
                    !string.IsNullOrWhiteSpace(control.Text))
                {
                    string translated =
                        TranslateKnownText(
                            control.Text,
                            arabic);

                    if (translated != null)
                    {
                        control.Text = translated;
                    }
                }

                if (control.HasChildren)
                {
                    ApplyTranslationsToControlTree(
                        control,
                        arabic);
                }
            }
        }

        // ============================================================
        // CAPTURE ENGLISH / LTR LAYOUT
        // ============================================================

        private void CaptureEnglishLayout()
        {
            _englishBounds.Clear();
            _englishAnchors.Clear();

            CaptureEnglishLayoutRecursive(this);

            _layoutCaptured = true;
        }

        private void CaptureEnglishLayoutRecursive(
            Control parent)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                try
                {
                    _englishBounds[control] =
                        control.Bounds;

                    _englishAnchors[control] =
                        control.Anchor;
                }
                catch
                {
                }

                if (control.HasChildren)
                {
                    CaptureEnglishLayoutRecursive(
                        control);
                }
            }
        }

        // ============================================================
        // RESTORE ENGLISH / LTR LAYOUT
        // ============================================================

        private void RestoreEnglishLayout()
        {
            if (!_layoutCaptured)
                return;

            // --------------------------------------------------------
            // Restore parent controls first.
            // --------------------------------------------------------

            foreach (KeyValuePair<Control, Rectangle> item
                in _englishBounds)
            {
                try
                {
                    Control control = item.Key;

                    if (control == null ||
                        control.IsDisposed)
                    {
                        continue;
                    }

                    if (_englishAnchors.ContainsKey(control))
                    {
                        control.Anchor =
                            _englishAnchors[control];
                    }

                    control.Bounds =
                        item.Value;
                }
                catch
                {
                }
            }
        }

        // ============================================================
        // MIRROR ANCHORS
        // ============================================================

        private AnchorStyles MirrorAnchors(
            AnchorStyles anchors)
        {
            bool left =
                (anchors & AnchorStyles.Left) != 0;

            bool right =
                (anchors & AnchorStyles.Right) != 0;

            AnchorStyles result =
                anchors &
                ~(AnchorStyles.Left |
                  AnchorStyles.Right);

            if (left)
                result |= AnchorStyles.Right;

            if (right)
                result |= AnchorStyles.Left;

            return result;
        }

        // ============================================================
        // MIRROR RECTANGLE
        // ============================================================

        private Rectangle MirrorRectangle(
            Control parent,
            Rectangle rectangle)
        {
            if (parent == null)
                return rectangle;

            int parentWidth =
                parent.ClientSize.Width;

            int x =
                parentWidth -
                rectangle.Right;

            if (x < 0)
                x = 0;

            int maxX =
                Math.Max(
                    0,
                    parentWidth -
                    rectangle.Width);

            if (x > maxX)
                x = maxX;

            return new Rectangle(
                x,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height);
        }

        // ============================================================
        // MIRROR PHYSICAL LAYOUT
        // ============================================================

        private void MirrorControlTree(
            Control parent)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                try
                {
                    if (_englishBounds.ContainsKey(control))
                    {
                        Rectangle englishBounds =
                            _englishBounds[control];

                        // ------------------------------------------------
                        // Docked controls keep their dock behavior.
                        // Their children are still mirrored.
                        // ------------------------------------------------

                        if (control.Dock == DockStyle.None)
                        {
                            control.Bounds =
                                MirrorRectangle(
                                    parent,
                                    englishBounds);

                            if (_englishAnchors.ContainsKey(control))
                            {
                                control.Anchor =
                                    MirrorAnchors(
                                        _englishAnchors[control]);
                            }
                        }
                    }
                }
                catch
                {
                }

                if (control.HasChildren)
                {
                    MirrorControlTree(control);
                }
            }
        }

        // ============================================================
        // APPLY RTL TEXT ALIGNMENT
        // ============================================================

        private void ApplyArabicAlignment(
            Control parent,
            bool arabic)
        {
            if (parent == null)
                return;

            foreach (Control control in parent.Controls)
            {
                // HeaderUC manages its own title alignment.
                // Keep the header title centered in BOTH languages.
                if (control is HeaderUC)
                {
                    continue;
                }

                try
                {
                    control.RightToLeft =
                        arabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    // ------------------------------------------------
                    // LABEL
                    // ------------------------------------------------

                    if (control is Label label)
                    {
                        // HeaderUC controls its own title alignment.
                        // Never override the centered header title here.
                        if (label.Parent is Guna.UI2.WinForms.Guna2GradientPanel &&
                            label.Name == "lblFormName")
                        {
                            label.TextAlign = ContentAlignment.MiddleCenter;
                            label.RightToLeft =
                                arabic ? RightToLeft.Yes : RightToLeft.No;
                        }
                        else
                        {
                            label.TextAlign =
                                arabic
                                    ? ContentAlignment.MiddleRight
                                    : ContentAlignment.MiddleLeft;
                        }
                    }

                    // ------------------------------------------------
                    // RADIO BUTTON
                    // ------------------------------------------------

                    else if (control is RadioButton radio)
                    {
                        radio.TextAlign =
                            arabic
                                ? ContentAlignment.MiddleRight
                                : ContentAlignment.MiddleLeft;
                    }

                    // ------------------------------------------------
                    // CHECK BOX
                    // ------------------------------------------------

                    else if (control is CheckBox checkBox)
                    {
                        checkBox.TextAlign =
                            arabic
                                ? ContentAlignment.MiddleRight
                                : ContentAlignment.MiddleLeft;
                    }

                    // ------------------------------------------------
                    // STANDARD TEXTBOX
                    // ------------------------------------------------

                    else if (control is TextBox textBox)
                    {
                        textBox.TextAlign =
                            arabic
                                ? HorizontalAlignment.Right
                                : HorizontalAlignment.Left;
                    }

                    // ------------------------------------------------
                    // GUNA TEXTBOX
                    // ------------------------------------------------

                    else if (control.GetType().Name ==
                             "Guna2TextBox")
                    {
                        try
                        {
                            dynamic gunaTextBox = control;

                            gunaTextBox.TextAlign =
                                arabic
                                    ? HorizontalAlignment.Right
                                    : HorizontalAlignment.Left;
                        }
                        catch
                        {
                        }
                    }

                    // ------------------------------------------------
                    // GUNA COMBOBOX
                    // ------------------------------------------------

                    else if (control.GetType().Name ==
                             "Guna2ComboBox")
                    {
                        try
                        {
                            dynamic gunaComboBox = control;

                            gunaComboBox.TextAlign =
                                arabic
                                    ? HorizontalAlignment.Right
                                    : HorizontalAlignment.Left;
                        }
                        catch
                        {
                        }
                    }

                    // ------------------------------------------------
                    // DATETIME PICKER
                    // ------------------------------------------------

                    else if (control is DateTimePicker dateTimePicker)
                    {
                        dateTimePicker.RightToLeftLayout =
                            arabic;
                    }
                }
                catch
                {
                }

                if (control.HasChildren)
                {
                    ApplyArabicAlignment(
                        control,
                        arabic);
                }
            }
        }

        // ============================================================
        // FOOTER BUTTON PHYSICAL LAYOUT
        // Arabic  -> LEFT
        // English -> RIGHT
        // ============================================================

        private void ApplyFooterButtonLayout()
        {
            if (btnClose == null ||
                btnSave == null)
            {
                return;
            }

            const int sideMargin = 18;
            const int gap = 12;

            int footerWidth =
                btnClose.Parent != null
                    ? btnClose.Parent.ClientSize.Width
                    : ClientSize.Width;

            // Arabic and English both keep the footer group
            // physically on the RIGHT. Arabic text itself is RTL.
            btnClose.Anchor =
                AnchorStyles.Right |
                AnchorStyles.Bottom;

            btnSave.Anchor =
                AnchorStyles.Right |
                AnchorStyles.Bottom;

            btnClose.Left =
                footerWidth -
                sideMargin -
                btnClose.Width;

            btnSave.Left =
                btnClose.Left -
                gap -
                btnSave.Width;

            btnClose.BringToFront();
            btnSave.BringToFront();
        }

        // ============================================================
        // REBUILD CURRENT LANGUAGE LAYOUT
        //
        // Used after changing Main/Sub or resizing.
        // ============================================================

        // ============================================================
        // COST CENTER TYPE - ARABIC PHYSICAL RTL LAYOUT
        // ============================================================

        private bool ContainsControlRecursive(
            Control parent,
            Control target)
        {
            if (parent == null ||
                target == null)
            {
                return false;
            }

            if (parent == target)
            {
                return true;
            }

            foreach (Control child in parent.Controls)
            {
                if (ContainsControlRecursive(
                    child,
                    target))
                {
                    return true;
                }
            }

            return false;
        }

        private Control FindCostCenterTypeCard()
        {
            if (lblTypeTitle == null ||
                lblTypeDescription == null ||
                radioMain == null ||
                radioSub == null)
            {
                return null;
            }

            Control candidate =
                lblTypeTitle.Parent;

            while (candidate != null &&
                   !(candidate is Form))
            {
                bool hasTitle =
                    ContainsControlRecursive(
                        candidate,
                        lblTypeTitle);

                bool hasDescription =
                    ContainsControlRecursive(
                        candidate,
                        lblTypeDescription);

                bool hasMain =
                    ContainsControlRecursive(
                        candidate,
                        radioMain);

                bool hasSub =
                    ContainsControlRecursive(
                        candidate,
                        radioSub);

                if (hasTitle &&
                    hasDescription &&
                    hasMain &&
                    hasSub)
                {
                    bool containsCode =
                        cardCode != null &&
                        ContainsControlRecursive(
                            candidate,
                            cardCode);

                    bool containsName =
                        cardName != null &&
                        ContainsControlRecursive(
                            candidate,
                            cardName);

                    bool containsParent =
                        cardParent != null &&
                        ContainsControlRecursive(
                            candidate,
                            cardParent);

                    if (!containsCode &&
                        !containsName &&
                        !containsParent)
                    {
                        return candidate;
                    }
                }

                candidate = candidate.Parent;
            }

            return null;
        }

        private void ApplyArabicCostCenterTypePhysicalLayout()
        {
            if (!_isArabic)
            {
                return;
            }

            try
            {
                Control typeCard =
                    FindCostCenterTypeCard();

                if (typeCard == null ||
                    typeCard.Parent == null)
                {
                    return;
                }

                Control parent =
                    typeCard.Parent;

                if (typeCard.Dock == DockStyle.None)
                {
                    typeCard.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right;

                    typeCard.Left =
                        Math.Max(
                            0,
                            parent.ClientSize.Width
                            - typeCard.Width
                            - 18);
                }

                lblTypeTitle.RightToLeft =
                    RightToLeft.Yes;

                lblTypeTitle.TextAlign =
                    ContentAlignment.MiddleRight;

                lblTypeDescription.RightToLeft =
                    RightToLeft.Yes;

                lblTypeDescription.TextAlign =
                    ContentAlignment.MiddleRight;

                radioMain.RightToLeft =
                    RightToLeft.Yes;

                radioMain.TextAlign =
                    ContentAlignment.MiddleRight;

                radioSub.RightToLeft =
                    RightToLeft.Yes;

                radioSub.TextAlign =
                    ContentAlignment.MiddleRight;
            }
            catch
            {
            }
        }

        // ============================================================
        // COMPLETE ARABIC PHYSICAL RTL LAYOUT
        // ============================================================
        //
        // The form uses manual physical mirroring instead of WinForms
        // RightToLeftLayout. These explicit placements handle the cards
        // whose designer/container structure prevents the generic
        // MirrorControlTree() from moving them.
        // ============================================================

        private void MoveControlToArabicRight(
            Control control,
            int rightMargin)
        {
            if (control == null ||
                control.Parent == null)
            {
                return;
            }

            try
            {
                control.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Right;

                control.Left =
                    Math.Max(
                        0,
                        control.Parent.ClientSize.Width
                        - control.Width
                        - rightMargin);
            }
            catch
            {
            }
        }

        private void ApplyArabicPageHeaderPhysicalLayout()
        {
            if (!_isArabic)
                return;

            MoveControlToArabicRight(
                lblPageTitle,
                18);

            MoveControlToArabicRight(
                lblPageDescription,
                18);

            lblPageTitle.RightToLeft =
                RightToLeft.Yes;

            lblPageTitle.TextAlign =
                ContentAlignment.MiddleRight;

            lblPageDescription.RightToLeft =
                RightToLeft.Yes;

            lblPageDescription.TextAlign =
                ContentAlignment.MiddleRight;
        }

        private void ApplyArabicCodeNamePhysicalLayout()
        {
            if (!_isArabic ||
                cardCode == null ||
                cardName == null)
            {
                return;
            }

            try
            {
                Control parent =
                    cardCode.Parent;

                if (parent == null)
                    return;

                // Keep the original vertical position/size.
                // Only change the horizontal physical order:
                //
                // Arabic:
                // [ NAME ] [ CODE ]
                //
                // CODE is the rightmost card.
                const int rightMargin = 18;
                const int gap = 12;

                cardCode.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Right;

                cardName.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Right;

                cardCode.Left =
                    Math.Max(
                        0,
                        parent.ClientSize.Width
                        - cardCode.Width
                        - rightMargin);

                cardName.Left =
                    Math.Max(
                        0,
                        cardCode.Left
                        - gap
                        - cardName.Width);
            }
            catch
            {
            }
        }

        private void ApplyArabicParentPhysicalLayout()
        {
            if (!_isArabic ||
                cardParent == null ||
                !cardParent.Visible)
            {
                return;
            }

            MoveControlToArabicRight(
                cardParent,
                18);
        }

        private void ApplyArabicCompletePhysicalLayout()
        {
            if (!_isArabic)
                return;

            ApplyArabicPageHeaderPhysicalLayout();

            ApplyArabicCostCenterTypePhysicalLayout();

            ApplyArabicParentPhysicalLayout();

            ApplyArabicCodeNamePhysicalLayout();
        }

        private void RefreshPhysicalLanguageLayout()
        {
            if (!_languageReady ||
                _applyingLanguage)
            {
                return;
            }

            try
            {
                SuspendLayout();

                // ----------------------------------------------------
                // Always rebuild from English canonical layout.
                // ----------------------------------------------------

                _isArabic = false;

                RestoreEnglishLayout();

                UpdateCostCenterUI();

                CaptureEnglishLayout();

                // ----------------------------------------------------
                // Apply requested language.
                // ----------------------------------------------------

                if (_lastArabicLanguage)
                {
                    _isArabic = true;

                    ApplyModernDesign();

                    UpdateCostCenterUI();

                    RestoreEnglishLayout();

                    MirrorControlTree(this);
                    ApplyArabicAlignment(
                        this,
                        true);

                    ApplyArabicCompletePhysicalLayout();

                }
                else
                {
                    _isArabic = false;

                    ApplyModernDesign();

                    UpdateCostCenterUI();

                    RestoreEnglishLayout();
                    ApplyArabicAlignment(
                        this,
                        false);
                }

                ApplyFooterButtonLayout();
            }
            finally
            {
                _isArabic =
                    _lastArabicLanguage;

                ResumeLayout(true);
                Refresh();
            }
        }

        // ============================================================
        // APPLY LANGUAGE
        //
        // PUBLIC so frmMain can also call:
        //
        // frmCostCenter.ApplyLanguage(true);
        // frmCostCenter.ApplyLanguage(false);
        // ============================================================

        public void ApplyLanguage(bool arabic)
        {
            if (IsDisposed)
                return;

            if (_applyingLanguage)
                return;

            _applyingLanguage = true;

            _lastArabicLanguage = arabic;

            try
            {
                SuspendLayout();

                // ====================================================
                // STEP 1
                // FORCE ENGLISH
                //
                // This creates the canonical LTR state.
                // ====================================================

                _isArabic = false;

                RestoreEnglishTexts();
                RestoreEnglishLayout();

                ApplyModernDesign();
                UpdateCostCenterUI();

                // ----------------------------------------------------
                // Save the canonical English state.
                // ----------------------------------------------------

                CaptureEnglishTexts();
                CaptureEnglishLayout();

                // ====================================================
                // STEP 2
                // APPLY REQUESTED LANGUAGE
                // ====================================================

                _isArabic = arabic;

                ApplyModernDesign();
                UpdateCostCenterUI();

                // ----------------------------------------------------
                // Translate other static designer controls.
                // ----------------------------------------------------

                ApplyTranslationsToControlTree(
                    this,
                    arabic);

                // ====================================================
                // ENGLISH
                // ====================================================
                if (!arabic)
                {
                    RestoreEnglishLayout();

                    ApplyArabicAlignment(
                        this,
                        false);

                    // ====================================================
                    // ENGLISH PAGE HEADER -> LEFT
                    // ====================================================

                    const int leftMargin = 18;

                    lblPageTitle.Left = leftMargin;
                    lblPageTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblPageDescription.Left = leftMargin;
                    lblPageDescription.TextAlign =
                        ContentAlignment.MiddleLeft;

                    // ====================================================
                    // ENGLISH COST CENTER TYPE -> LEFT
                    // ====================================================

                    lblTypeTitle.Left = leftMargin;
                    lblTypeTitle.TextAlign =
                        ContentAlignment.MiddleLeft;

                    lblTypeDescription.Left = leftMargin;
                    lblTypeDescription.TextAlign =
                        ContentAlignment.MiddleLeft;

                    radioMain.Left = leftMargin;
                    radioSub.Left =
                        radioMain.Right + 25;
                }
                // ====================================================
                // ARABIC
                // ====================================================
                else
                {
                    RestoreEnglishLayout();

                    // Mirror the rest of the form for Arabic 123
                    MirrorControlTree(this);

                    ApplyArabicAlignment(
                        this,
                        true);

                    ApplyArabicCompletePhysicalLayout();
                }

                ApplyFooterButtonLayout();

                _languageReady = true;
            }
            finally
            {
                _isArabic = arabic;
                _lastArabicLanguage = arabic;

                ResumeLayout(true);
                Refresh();

                _applyingLanguage = false;
            }
        }

        // ============================================================
        // LANGUAGE TIMER
        // ============================================================

        private void LanguageTimer_Tick(
            object sender,
            EventArgs e)
        {
            if (!_languageReady ||
                _applyingLanguage ||
                IsDisposed)
            {
                return;
            }

            bool arabic =
                GetGlobalArabicLanguage();

            if (arabic != _isArabic)
            {
                ApplyLanguage(arabic);
            }
        }

        // ============================================================
        // FORM ACTIVATED
        // ============================================================

        private void frmCostCenter_Activated(
            object sender,
            EventArgs e)
        {
            if (!_languageReady ||
                _applyingLanguage)
            {
                return;
            }

            bool arabic =
                GetGlobalArabicLanguage();

            if (arabic != _isArabic)
            {
                ApplyLanguage(arabic);
            }
        }

        // ============================================================
        // FORM RESIZE
        // ============================================================

        private void frmCostCenter_Resize(
            object sender,
            EventArgs e)
        {
            if (!_languageReady ||
                _applyingLanguage)
            {
                return;
            }

            RefreshPhysicalLanguageLayout();
        }

        // ============================================================
        // FORM CLOSED
        // ============================================================

        private void frmCostCenter_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            try
            {
                if (_languageTimer != null)
                {
                    _languageTimer.Stop();
                    _languageTimer.Tick -=
                        LanguageTimer_Tick;
                    _languageTimer.Dispose();
                    _languageTimer = null;
                }
            }
            catch
            {
            }

            try
            {
                if (editMain != null)
                {
                    //.MainCostCenter -=
                        //editMain;
                }
            }
            catch
            {
            }
        }

        // ============================================================
        // APPLY MODERN DESIGN + TRANSLATION
        // ============================================================

        private void ApplyModernDesign()
        {
            // ========================================================
            // FORM
            // ========================================================

            this.Text =
                T(
                    "Cost Center",
                    "مركز التكلفة");

            this.BackColor =
                Color.FromArgb(
                    246,
                    245,
                    250);

            this.StartPosition =
                FormStartPosition.Manual;

            // IMPORTANT:
            // We use manual physical mirroring instead of WinForms
            // automatic RTL mirroring.
            this.RightToLeft =
                RightToLeft.No;

            this.RightToLeftLayout =
                false;

            // ========================================================
            // PAGE HEADER
            // ========================================================

            headerUC1.ArabicLayout = _isArabic;

            headerUC1.FormText =
                T(
                    "Cost Center",
                    "مركز التكلفة");

            // ========================================================
            // PAGE TITLE
            // ========================================================

            lblPageTitle.Text =
                T(
                    "Cost Center Setup",
                    "إعداد مركز التكلفة");

            // ========================================================
            // PAGE DESCRIPTION
            // ========================================================

            lblPageDescription.Text =
                T(
                    "Create and organize your school's accounting cost centers.",
                    "إنشاء وتنظيم مراكز التكلفة المحاسبية للمدرسة.");

            // ========================================================
            // TYPE CARD
            // ========================================================

            lblTypeTitle.Text =
                T(
                    "Cost Center Type",
                    "نوع مركز التكلفة");

            lblTypeDescription.Text =
                T(
                    "Choose the accounting level",
                    "اختر المستوى المحاسبي");

            // ========================================================
            // MAIN
            // ========================================================

            radioMain.Text =
                T(
                    "Main",
                    "رئيسي");

            // ========================================================
            // SUB
            // ========================================================

            radioSub.Text =
                T(
                    "Sub",
                    "فرعي");

            radioMain.Font =
                new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold);

            radioSub.Font =
                new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold);

            radioMain.ForeColor =
                Color.FromArgb(
                    65,
                    55,
                    100);

            radioSub.ForeColor =
                Color.FromArgb(
                    65,
                    55,
                    100);

            // ========================================================
            // PARENT CARD
            // ========================================================

            lblParentTitle.Text =
                T(
                    "Parent Cost Center",
                    "مركز التكلفة الرئيسي");

            lblParentDescription.Text =
                T(
                    "Select the main cost center",
                    "اختر مركز التكلفة الرئيسي");

            lblMain.Text =
                T(
                    "Parent Cost Center",
                    "مركز التكلفة الرئيسي");

            // ========================================================
            // EDIT
            // ========================================================

            lnkEditCategory.Text =
                T(
                    "Edit",
                    "تعديل");

            lnkEditCategory.LinkColor =
                Color.FromArgb(
                    104,
                    55,
                    215);

            lnkEditCategory.Font =
                new Font(
                    "Segoe UI",
                    8.5F);

            // ========================================================
            // CODE
            // ========================================================

            label3.Text =
                T(
                    "Code",
                    "الكود");

            lblCodeHint.Text =
                T(
                    "Auto generated",
                    "يتم إنشاؤه تلقائياً");

            txtCode.PlaceholderText =
                T(
                    "Auto",
                    "تلقائي");

            // ========================================================
            // NAME
            // ========================================================

            label2.Text =
                T(
                    "Name",
                    "الاسم");

            lblNameHint.Text =
                T(
                    "Enter a clear accounting name",
                    "أدخل اسماً واضحاً للحساب");

            txtName.PlaceholderText =
                T(
                    "e.g. Administration, Transport, Library...",
                    "مثال: الإدارة، النقل، المكتبة...");

            txtName.Font =
                new Font(
                    "Segoe UI",
                    9.5F);

            txtName.BackColor =
                Color.White;

            txtName.ForeColor =
                Color.FromArgb(
                    44,
                    44,
                    58);

            // ========================================================
            // FOOTER
            // ========================================================

            lblFooterHint.Text =
                T(
                    "Make sure the cost center name is unique.",
                    "تأكد من أن اسم مركز التكلفة فريد.");

            // ========================================================
            // SAVE BUTTON
            // ========================================================

            btnSave.Text =
                T(
                    "Save Cost Center",
                    "حفظ مركز التكلفة");

            btnSave.Font =
                new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold);

            btnSave.FillColor =
                Color.FromArgb(
                    104,
                    55,
                    215);

            btnSave.ForeColor =
                Color.White;

            btnSave.BorderRadius =
                8;

            btnSave.HoverState.FillColor =
                Color.FromArgb(
                    118,
                    67,
                    229);

            // ========================================================
            // CLOSE BUTTON
            // ========================================================

            btnClose.Text =
                T(
                    "Close",
                    "إغلاق");

            btnClose.Font =
                new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold);

            btnClose.FillColor =
                Color.White;

            btnClose.ForeColor =
                Color.FromArgb(
                    65,
                    58,
                    82);

            btnClose.BorderColor =
                Color.FromArgb(
                    211,
                    205,
                    224);

            btnClose.BorderThickness =
                1;

            btnClose.BorderRadius =
                8;
        }

        // ============================================================
        // CLOSE BUTTON
        // ============================================================

        private void btnClose_Click(
            object sender,
            EventArgs e)
        {
            frmMain.openChildForm(
                new MasterCostCenter());
        }

        // ============================================================
        // MAIN / SUB UI
        // ============================================================

        private void UpdateCostCenterUI()
        {
            bool isMain =
                radioMain.Checked;

            bool isSub =
                radioSub.Checked;

            // ========================================================
            // MAIN COST CENTER
            // ========================================================

            if (isMain)
            {
                // ----------------------------------------------------
                // HIDE PARENT CARD
                // ----------------------------------------------------

                cardParent.Visible =
                    false;

                lblMain.Visible =
                    false;

                cmbCostCenter.Visible =
                    false;

                lnkEditCategory.Visible =
                    false;

                // ----------------------------------------------------
                // MOVE CODE + NAME UP
                // ----------------------------------------------------

                cardCode.Location =
                    new Point(
                        0,
                        160);

                cardName.Location =
                    new Point(
                        235,
                        160);

                // ----------------------------------------------------
                // HEADER
                // ----------------------------------------------------

                headerUC1.FormText =
                    T(
                        "New Main Cost Center",
                        "مركز تكلفة رئيسي جديد");

                // ----------------------------------------------------
                // SAVE
                // ----------------------------------------------------

                btnSave.Text =
                    T(
                        "Save Cost Center",
                        "حفظ مركز التكلفة");

                // ----------------------------------------------------
                // CLEAR PARENT
                // ----------------------------------------------------

                if (cmbCostCenter.Items.Count > 0)
                {
                    cmbCostCenter.SelectedIndex =
                        -1;
                }
            }

            // ========================================================
            // SUB COST CENTER
            // ========================================================

            else if (isSub)
            {
                // ----------------------------------------------------
                // SHOW PARENT CARD
                // ----------------------------------------------------

                cardParent.Visible =
                    true;

                cardParent.BringToFront();

                // ----------------------------------------------------
                // SHOW PARENT CONTROLS
                // ----------------------------------------------------

                lblMain.Visible =
                    true;

                cmbCostCenter.Visible =
                    true;

                lnkEditCategory.Visible =
                    true;

                // ----------------------------------------------------
                // MOVE CODE + NAME DOWN
                // ----------------------------------------------------

                cardCode.Location =
                    new Point(
                        0,
                        260);

                cardName.Location =
                    new Point(
                        235,
                        260);

                // ----------------------------------------------------
                // HEADER
                // ----------------------------------------------------

                headerUC1.FormText =
                    T(
                        "New Sub Cost Center",
                        "مركز تكلفة فرعي جديد");

                // ----------------------------------------------------
                // SAVE
                // ----------------------------------------------------

                btnSave.Text =
                    T(
                        "Save Sub Cost Center",
                        "حفظ مركز التكلفة الفرعي");
            }
        }

        // ============================================================
        // FORM LOAD
        // ============================================================

        private void frmCostCenter_Load(
            object sender,
            EventArgs e)
        {
            // ========================================================
            // LOAD PARENT COST CENTERS
            // ========================================================

            BindCombo();

            // ========================================================
            // DEFAULT
            // ========================================================

            if (projectId == 0 &&
                !radioSub.Checked)
            {
                radioMain.Checked =
                    true;
            }

            // ========================================================
            // APPLY INITIAL LANGUAGE
            // ========================================================

            ApplyLanguage(
                GetGlobalArabicLanguage());
        }

        // ============================================================
        // BIND COMBO
        // ============================================================

        private void BindCombo()
        {
            BindCombos.PopulateCostCenter(
                cmbCostCenter);

            //.Refreshlvl1Account();
            //.Refreshlvl2Account();

            if (selectedIndex > 0 &&
                selectedIndex <
                cmbCostCenter.Items.Count)
            {
                cmbCostCenter.SelectedIndex =
                    selectedIndex;
            }
        }

        // ============================================================
        // MAIN RADIO
        // ============================================================

        private void radioMain_CheckedChanged(
            object sender,
            EventArgs e)
        {
            if (radioMain.Checked)
            {
                UpdateCostCenterUI();

                if (_languageReady &&
                    !_applyingLanguage)
                {
                    RefreshPhysicalLanguageLayout();
                }
            }
        }

        // ============================================================
        // SUB RADIO
        // ============================================================

        private void radioSub_CheckedChanged(
            object sender,
            EventArgs e)
        {
            if (radioSub.Checked)
            {
                UpdateCostCenterUI();

                if (_languageReady &&
                    !_applyingLanguage)
                {
                    RefreshPhysicalLanguageLayout();
                }
            }
        }

        // ============================================================
        // SAVE
        // ============================================================

        private void btnSave_Click(
            object sender,
            EventArgs e)
        {
            // ========================================================
            // TYPE VALIDATION
            // ========================================================

            if (!radioMain.Checked &&
                !radioSub.Checked)
            {
                MessageBox.Show(
                    T(
                        "Please select Main Cost Center or Sub Cost Center.",
                        "يرجى اختيار مركز تكلفة رئيسي أو مركز تكلفة فرعي."),
                    T(
                        "Cost Center",
                        "مركز التكلفة"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // ========================================================
            // SUB MUST HAVE PARENT
            // ========================================================

            if (radioSub.Checked)
            {
                if (cmbCostCenter.SelectedValue == null ||
                    cmbCostCenter.SelectedIndex < 0 ||
                    string.IsNullOrWhiteSpace(
                        cmbCostCenter.Text))
                {
                    MessageBox.Show(
                        T(
                            "Please select a Parent Cost Center.",
                            "يرجى اختيار مركز التكلفة الرئيسي."),
                        T(
                            "Cost Center",
                            "مركز التكلفة"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }
            }

            // ========================================================
            // INSERT
            // ========================================================

            if (insertCost())
            {
                //.Refreshlvl1Account();
                //.Refreshlvl2Account();

                this.Close();
            }
            else
            {
                // ====================================================
                // UPDATE
                // ====================================================

                if (updateCost())
                {
                    //.Refreshlvl1Account();
                    //.Refreshlvl2Account();

                    this.Close();
                }
            }
        }

        // ============================================================
        // UPDATE COST CENTER
        // ============================================================

        private bool updateCost()
        {
            return true;
        }

        // ============================================================
        // INSERT COST CENTER
        // ============================================================

        private bool insertCost()
        {
            int mainCode = 101;

            // ========================================================
            // NAME VALIDATION
            // ========================================================

            if (string.IsNullOrWhiteSpace(
                txtName.Text))
            {
                MessageBox.Show(
                    T(
                        "Please enter the Cost Center name.",
                        "يرجى إدخال اسم مركز التكلفة."),
                    T(
                        "Cost Center",
                        "مركز التكلفة"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return false;
            }

            // ========================================================
            // DUPLICATE NAME
            // ========================================================

            using (
                MySqlDataReader reader =
                ClinicDatabase.ExecuteReader(
                    "SELECT * FROM tbl_cost_center WHERE name = @name",
                    ClinicDatabase.CreateParameterNew(
                        "name",
                        txtName.Text.Trim())))
            {
                if (reader.Read())
                {
                    MessageBox.Show(
                        T(
                            "This Cost Center name already exists. Please enter another name.",
                            "اسم مركز التكلفة هذا موجود بالفعل. يرجى إدخال اسم آخر."),
                        T(
                            "Cost Center",
                            "مركز التكلفة"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return false;
                }
            }

            // ========================================================
            // MAIN COST CENTER
            // ========================================================

            if (radioMain.Checked)
            {
                using (
                    MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(
                        @"SELECT code
                          FROM tbl_cost_center
                          ORDER BY CAST(SUBSTRING_INDEX(code, '-', -1) AS UNSIGNED) DESC
                          LIMIT 1;"))
                {
                    if (reader.Read() &&
                        reader["code"].ToString() != "")
                    {
                        mainCode =
                            int.Parse(
                                reader["code"].ToString()) + 1;
                    }

                    ClinicDatabase.ExecuteNonQuery(
                        @"INSERT INTO tbl_cost_center
                          (name, code, project_id)
                          VALUES
                          (@name, @code, @project_id);",

                        ClinicDatabase.CreateParameterNew(
                            "name",
                            txtName.Text.Trim()),

                        ClinicDatabase.CreateParameterNew(
                            "code",
                            mainCode),

                        ClinicDatabase.CreateParameterNew(
                            "project_id",
                            projectId));

                    //Utilities.LogAudit(
                       //
                }

                return true;
            }

            // ========================================================
            // SUB COST CENTER
            // ========================================================

            if (radioSub.Checked)
            {
                if (cmbCostCenter.SelectedValue == null)
                {
                    MessageBox.Show(
                        T(
                            "Please select a Parent Cost Center.",
                            "يرجى اختيار مركز التكلفة الرئيسي."),
                        T(
                            "Cost Center",
                            "مركز التكلفة"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return false;
                }

                string parentId =
                    cmbCostCenter.SelectedValue.ToString();

                // ====================================================
                // GET MAIN CODE
                // ====================================================

                using (
                    MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(
                        @"SELECT code
                          FROM tbl_cost_center
                          WHERE id = @id",

                        ClinicDatabase.CreateParameterNew(
                            "id",
                            parentId)))
                {
                    if (!reader.Read())
                    {
                        MessageBox.Show(
                            T(
                                "Unable to find the selected Parent Cost Center.",
                                "تعذر العثور على مركز التكلفة الرئيسي المحدد."),
                            T(
                                "Cost Center",
                                "مركز التكلفة"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return false;
                    }

                    string mainCostCode =
                        reader["code"].ToString();

                    // =================================================
                    // GET LAST SUB CODE
                    // =================================================

                    int newSubCode = 1;

                    using (
                        MySqlDataReader r =
                        ClinicDatabase.ExecuteReader(
                            @"SELECT MAX(code)
                              FROM tbl_sub_cost_center
                              WHERE code LIKE @mainCode",

                            ClinicDatabase.CreateParameterNew(
                                "mainCode",
                                mainCostCode + "%")))
                    {
                        if (r.Read() &&
                            r[0] != DBNull.Value)
                        {
                            string lastCode =
                                r[0].ToString();

                            if (lastCode.Length >
                                mainCostCode.Length)
                            {
                                string subPart =
                                    lastCode.Substring(
                                        mainCostCode.Length);

                                int lastSubCode;

                                if (int.TryParse(
                                    subPart,
                                    out lastSubCode))
                                {
                                    newSubCode =
                                        lastSubCode + 1;
                                }
                            }
                        }
                    }

                    // =================================================
                    // CREATE CODE
                    // =================================================

                    string formattedSubCode =
                        mainCostCode +
                        newSubCode.ToString("D3");

                    // =================================================
                    // INSERT
                    // =================================================

                    ClinicDatabase.ExecuteNonQuery(
                        @"INSERT INTO tbl_sub_cost_center
                          (code, name, main_id, project_id)
                          VALUES
                          (@code, @name, @project_id, @main_id);",

                        ClinicDatabase.CreateParameterNew(
                            "code",
                            formattedSubCode),

                        ClinicDatabase.CreateParameterNew(
                            "name",
                            txtName.Text.Trim()),

                        ClinicDatabase.CreateParameterNew(
                            "project_id",
                            projectId),

                        ClinicDatabase.CreateParameterNew(
                            "main_id",
                            parentId));

                    //Utilities.LogAudit(
                       

                    return true;
                }
            }

            return false;
        }

        // ============================================================
        // EDIT PARENT
        // ============================================================

        private void lnkEditCategory_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            if (cmbCostCenter.SelectedValue == null)
            {
                MessageBox.Show(
                    T(
                        "Select a Parent Cost Center first.",
                        "يرجى اختيار مركز التكلفة الرئيسي أولاً."),
                    T(
                        "Cost Center",
                        "مركز التكلفة"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            new frmViewEditMain(
                int.Parse(
                    cmbCostCenter.SelectedValue.ToString()))
                .ShowDialog();

            selectedIndex =
                cmbCostCenter.SelectedIndex;

            BindCombo();
        }

        // ============================================================
        // OLD LINK HANDLER
        // ============================================================

        private void lnkLvl1Edit_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            if (cmbCostCenter.SelectedValue == null ||
                cmbCostCenter.Text == "")
            {
                MessageBox.Show(
                    T(
                        "Select Level 1 First.",
                        "يرجى اختيار المستوى الأول أولاً."));

                return;
            }
        }

        // ============================================================
        // HEADER CLICK
        // ============================================================

        private void LbHeader_Click(
            object sender,
            EventArgs e)
        {
            this.BringToFront();
        }
    }
}