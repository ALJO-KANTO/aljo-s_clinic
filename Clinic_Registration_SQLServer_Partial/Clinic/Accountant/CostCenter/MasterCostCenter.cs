
using Clinic;
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
//using YamyProject.Localization;

namespace YamyProject
{
    public partial class MasterCostCenter : Form
    {
        // ============================================================
        // EVENTS
        // ============================================================

        private EventHandler lvl1;
        private EventHandler lvl2;

        // ============================================================
        // LANGUAGE
        // ============================================================

        private bool _isArabic;
        private bool _applyingLanguage;
        private bool _languageInitialized;

        private System.Windows.Forms.Timer _languageTimer;

        // ============================================================
        // FORM GEOMETRY
        // ============================================================

        private Rectangle _stableBounds;
        private FormWindowState _stableWindowState =
            FormWindowState.Normal;

        private bool _geometryCaptured;
        private bool _suppressResizeTracking;

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public MasterCostCenter()
        {
            InitializeComponent();

            // --------------------------------------------------------
            // Capture initial geometry
            // --------------------------------------------------------

            CaptureInitialGeometry();

            // --------------------------------------------------------
            // Existing localization system
            // --------------------------------------------------------

            try
            {
               // LocalizationManager.LocalizeForm(this);
            }
            catch
            {
                // Never stop the form because of localization.
            }

            // --------------------------------------------------------
            // Read current global language
            // --------------------------------------------------------

            _isArabic = GetGlobalArabicLanguage();
            _languageInitialized = true;

            // --------------------------------------------------------
            // Apply current language
            // --------------------------------------------------------

            ApplyLanguage();

            // --------------------------------------------------------
            // GRID
            // --------------------------------------------------------

            AddDGVColumns();

            // --------------------------------------------------------
            // EXISTING EVENTS
            // --------------------------------------------------------

            lvl1 = (sender, args) => BindRefresh();
            lvl2 = (sender, args) => BindRefresh();

            //.lvl1Account += lvl1;
            //.lvl2Account += lvl2;

            // --------------------------------------------------------
            // HEADER
            // --------------------------------------------------------

            if (headerUC1 != null)
            {
                headerUC1.FormText = this.Text;
            }

            // --------------------------------------------------------
            // FORM EVENTS
            // --------------------------------------------------------

            Resize += MasterCostCenter_Resize;
            FormClosed += MasterCostCenter_FormClosed;

            // --------------------------------------------------------
            // LANGUAGE MONITOR
            //
            // Allows switching English <-> Arabic while the form
            // remains open.
            // --------------------------------------------------------

            _languageTimer =
                new System.Windows.Forms.Timer();

            _languageTimer.Interval = 250;

            _languageTimer.Tick +=
                LanguageTimer_Tick;

            _languageTimer.Start();
        }

        // ============================================================
        // GLOBAL LANGUAGE DETECTION
        // ============================================================

        private bool GetGlobalArabicLanguage()
        {
            // --------------------------------------------------------
            // IMPORTANT:
            // Do NOT inspect this form itself first.
            //
            // This prevents this form's own _isArabic value from
            // locking the language.
            // --------------------------------------------------------

            // --------------------------------------------------------
            // 1. MDI parent
            // --------------------------------------------------------

            try
            {
                if (MdiParent != null)
                {
                    bool? value =
                        TryReadArabicFlag(MdiParent);

                    if (value.HasValue)
                    {
                        return value.Value;
                    }

                    Control current =
                        MdiParent.Parent;

                    while (current != null)
                    {
                        value =
                            TryReadArabicFlag(current);

                        if (value.HasValue)
                        {
                            return value.Value;
                        }

                        current = current.Parent;
                    }
                }
            }
            catch
            {
            }

            // --------------------------------------------------------
            // 2. Parent chain
            // --------------------------------------------------------

            try
            {
                Control current = Parent;

                while (current != null)
                {
                    bool? value =
                        TryReadArabicFlag(current);

                    if (value.HasValue)
                    {
                        return value.Value;
                    }

                    current = current.Parent;
                }
            }
            catch
            {
            }

            // --------------------------------------------------------
            // 3. Main application form
            // --------------------------------------------------------

            try
            {
                //if (frmMain != null)
                //{
                //    bool? value =
                //        TryReadArabicFlag(frmMain);

                //    if (value.HasValue)
                //    {
                //        return value.Value;
                //    }
                //}
            }
            catch
            {
            }

            // --------------------------------------------------------
            // 4. Culture fallback
            // --------------------------------------------------------

            try
            {
                string uiLanguage =
                    Thread.CurrentThread
                        .CurrentUICulture
                        .TwoLetterISOLanguageName;

                if (string.Equals(
                    uiLanguage,
                    "ar",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                string cultureLanguage =
                    CultureInfo.CurrentCulture
                        .TwoLetterISOLanguageName;

                if (string.Equals(
                    cultureLanguage,
                    "ar",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
            }

            return _isArabic;
        }

        // ============================================================
        // READ GLOBAL LANGUAGE FLAG
        // ============================================================

        private bool? TryReadArabicFlag(object source)
        {
            if (source == null)
            {
                return null;
            }

            Type type =
                source.GetType();

            // --------------------------------------------------------
            // _isArabic field
            // --------------------------------------------------------

            try
            {
                FieldInfo field =
                    type.GetField(
                        "_isArabic",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic |
                        BindingFlags.Public);

                if (field != null &&
                    field.FieldType == typeof(bool))
                {
                    return (bool)field.GetValue(source);
                }
            }
            catch
            {
            }

            // --------------------------------------------------------
            // IsArabic property
            // --------------------------------------------------------

            try
            {
                PropertyInfo property =
                    type.GetProperty(
                        "IsArabic",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic |
                        BindingFlags.Public);

                if (property != null &&
                    property.PropertyType == typeof(bool) &&
                    property.CanRead)
                {
                    return (bool)property.GetValue(source);
                }
            }
            catch
            {
            }

            return null;
        }

        // ============================================================
        // TRANSLATION
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
        // LANGUAGE TIMER
        // ============================================================

        private void LanguageTimer_Tick(
            object sender,
            EventArgs e)
        {
            if (_applyingLanguage)
            {
                return;
            }

            if (IsDisposed || Disposing)
            {
                return;
            }

            bool currentArabic =
                GetGlobalArabicLanguage();

            // --------------------------------------------------------
            // First initialization
            // --------------------------------------------------------

            if (!_languageInitialized)
            {
                _isArabic = currentArabic;
                _languageInitialized = true;

                ApplyLanguage();

                return;
            }

            // --------------------------------------------------------
            // Language changed
            // --------------------------------------------------------

            if (currentArabic != _isArabic)
            {
                _isArabic = currentArabic;

                ApplyLanguage();
            }
        }

        // ============================================================
        // MAIN LANGUAGE METHOD
        //
        // IMPORTANT:
        // THERE IS ONLY ONE ApplyLanguage() METHOD IN THIS FILE.
        // ============================================================

        private void ApplyLanguage()
        {
            if (_applyingLanguage)
            {
                return;
            }

            _applyingLanguage = true;
            _suppressResizeTracking = true;

            try
            {
                // ----------------------------------------------------
                // Save geometry
                // ----------------------------------------------------

                FormWindowState savedState =
                    WindowState;

                Rectangle savedBounds =
                    _stableBounds;

                if (savedState ==
                    FormWindowState.Normal)
                {
                    if (!_geometryCaptured ||
                        !IsValidBounds(savedBounds))
                    {
                        if (IsValidBounds(Bounds))
                        {
                            savedBounds = Bounds;
                        }
                    }
                }

                // ----------------------------------------------------
                // Re-read language
                // ----------------------------------------------------

                _isArabic =
                    GetGlobalArabicLanguage();

                _languageInitialized = true;

                // ----------------------------------------------------
                // FORM
                // ----------------------------------------------------

                Text =
                    _isArabic
                        ? "إدارة مراكز التكلفة"
                        : "Master Cost Center";

                // ----------------------------------------------------
                // DO NOT MIRROR WHOLE FORM
                // ----------------------------------------------------

                RightToLeft =
                    RightToLeft.No;

                RightToLeftLayout =
                    false;

                // ----------------------------------------------------
                // HEADER
                // ----------------------------------------------------

                if (headerUC1 != null)
                {
                    headerUC1.FormText = Text;

                    headerUC1.RightToLeft =
                        _isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                }

                // ----------------------------------------------------
                // PAGE TITLE
                // ----------------------------------------------------

                if (lblPageTitle != null)
                {
                    lblPageTitle.Text =
                        _isArabic
                            ? "إدارة مراكز التكلفة"
                            : "Cost Center Management";

                    lblPageTitle.RightToLeft =
                        _isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    lblPageTitle.TextAlign =
                        _isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }

                // ----------------------------------------------------
                // PAGE DESCRIPTION
                // ----------------------------------------------------

                if (lblPageDescription != null)
                {
                    lblPageDescription.Text =
                        _isArabic
                            ? "إنشاء وتنظيم وإدارة مراكز التكلفة المحاسبية."
                            : "Create, organize and manage your accounting cost centers.";

                    lblPageDescription.RightToLeft =
                        _isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    lblPageDescription.TextAlign =
                        _isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }

                // ----------------------------------------------------
                // NEW BUTTON
                // ----------------------------------------------------

                if (btnSave != null)
                {
                    btnSave.Text =
                        _isArabic
                            ? "+  مركز تكلفة جديد"
                            : "+  New Cost Center";

                    btnSave.RightToLeft =
                        _isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;
                }

                // ----------------------------------------------------
                // GRID TITLE
                // ----------------------------------------------------

                if (lblGridTitle != null)
                {
                    lblGridTitle.Text =
                        _isArabic
                            ? "مراكز التكلفة"
                            : "Cost Centers";

                    lblGridTitle.RightToLeft =
                        _isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    lblGridTitle.TextAlign =
                        _isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }

                // ----------------------------------------------------
                // GRID DESCRIPTION
                // ----------------------------------------------------

                if (lblGridDescription != null)
                {
                    lblGridDescription.Text =
                        _isArabic
                            ? "انقر نقراً مزدوجاً على مركز التكلفة لعرض أو تعديل التفاصيل."
                            : "Double-click a cost center to view or edit details.";

                    lblGridDescription.RightToLeft =
                        _isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    lblGridDescription.TextAlign =
                        _isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }

                // ----------------------------------------------------
                // FOOTER
                // ----------------------------------------------------

                if (lblFooterText != null)
                {
                    lblFooterText.Text =
                        _isArabic
                            ? "نصيحة: انقر نقراً مزدوجاً على مركز التكلفة لعرضه أو تعديله."
                            : "Tip: Double-click a cost center to view or edit it.";

                    lblFooterText.RightToLeft =
                        _isArabic
                            ? RightToLeft.Yes
                            : RightToLeft.No;

                    lblFooterText.TextAlign =
                        _isArabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                }

                // ----------------------------------------------------
                // GRID LANGUAGE
                // ----------------------------------------------------

                ApplyGridLanguage();

                // ----------------------------------------------------
                // Restore geometry
                // ----------------------------------------------------

                RestoreFormGeometry(
                    savedState,
                    savedBounds);
            }
            finally
            {
                _suppressResizeTracking = false;
                _applyingLanguage = false;
            }
        }

        // ============================================================
        // GRID LANGUAGE
        // ============================================================

        private void ApplyGridLanguage()
        {
            if (dgvCustomer == null)
            {
                return;
            }

            try
            {
                // ----------------------------------------------------
                // Direction
                // ----------------------------------------------------

                dgvCustomer.RightToLeft =
                    _isArabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                // ----------------------------------------------------
                // Cell alignment
                // ----------------------------------------------------

                dgvCustomer.DefaultCellStyle.Alignment =
                    _isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;

                dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                // ----------------------------------------------------
                // Font
                // ----------------------------------------------------

                dgvCustomer.DefaultCellStyle.Font =
                    _isArabic
                        ? new Font(
                            "Tahoma",
                            9.5F,
                            FontStyle.Regular)
                        : new Font(
                            "Segoe UI",
                            9.5F,
                            FontStyle.Regular);

                dgvCustomer.ColumnHeadersDefaultCellStyle.Font =
                    _isArabic
                        ? new Font(
                            "Tahoma",
                            9.5F,
                            FontStyle.Bold)
                        : new Font(
                            "Segoe UI",
                            9.5F,
                            FontStyle.Bold);

                // ----------------------------------------------------
                // Existing localization manager
                // ----------------------------------------------------

                try
                {
                   // LocalizationManager
                        //.LocalizeDataGridViewHeaders(
                            //dgvCustomer);
                }
                catch
                {
                }

                // ----------------------------------------------------
                // Required translations
                // ----------------------------------------------------

                if (dgvCustomer.Columns.Contains("lvl1"))
                {
                    dgvCustomer.Columns["lvl1"].HeaderText =
                        _isArabic
                            ? "الاسم"
                            : "Name";
                }

                if (dgvCustomer.Columns.Contains("Add"))
                {
                    dgvCustomer.Columns["Add"].HeaderText =
                        _isArabic
                            ? "إضافة"
                            : "Add";
                }

                if (dgvCustomer.Columns.Contains("Edit"))
                {
                    dgvCustomer.Columns["Edit"].HeaderText =
                        _isArabic
                            ? "تعديل"
                            : "Edit";
                }
            }
            catch
            {
            }
        }

        // ============================================================
        // FORM LOAD
        // ============================================================

        private void MasterCostCenter_Load(
            object sender,
            EventArgs e)
        {
            try
            {
                // Read the final language after the main form has
                // finished loading.
                _isArabic =
                    GetGlobalArabicLanguage();

                _languageInitialized = true;

                ApplyLanguage();

                BindDGV();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    _isArabic
                        ? "تعذر تحميل مراكز التكلفة.\n\n" +
                          ex.Message
                        : "Cost centers could not be loaded.\n\n" +
                          ex.Message,
                    _isArabic
                        ? "مراكز التكلفة"
                        : "Cost Centers",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // INITIAL GEOMETRY
        // ============================================================

        private void CaptureInitialGeometry()
        {
            try
            {
                if (WindowState ==
                    FormWindowState.Maximized)
                {
                    _stableWindowState =
                        FormWindowState.Maximized;

                    _geometryCaptured = true;

                    return;
                }

                if (WindowState ==
                    FormWindowState.Normal &&
                    IsValidBounds(Bounds))
                {
                    _stableBounds = Bounds;

                    _stableWindowState =
                        FormWindowState.Normal;

                    _geometryCaptured = true;
                }
            }
            catch
            {
            }
        }

        // ============================================================
        // VALID BOUNDS
        // ============================================================

        private bool IsValidBounds(
            Rectangle bounds)
        {
            int minimumWidth = 900;
            int minimumHeight = 600;

            try
            {
                if (MinimumSize.Width > 0)
                {
                    minimumWidth =
                        Math.Max(
                            900,
                            MinimumSize.Width);
                }

                if (MinimumSize.Height > 0)
                {
                    minimumHeight =
                        Math.Max(
                            600,
                            MinimumSize.Height);
                }
            }
            catch
            {
            }

            return bounds.Width >= minimumWidth &&
                   bounds.Height >= minimumHeight;
        }

        // ============================================================
        // RESTORE GEOMETRY
        // ============================================================

        private void RestoreFormGeometry(
            FormWindowState savedState,
            Rectangle savedBounds)
        {
            try
            {
                if (savedState ==
                    FormWindowState.Maximized)
                {
                    WindowState =
                        FormWindowState.Maximized;

                    return;
                }

                WindowState =
                    FormWindowState.Normal;

                if (IsValidBounds(savedBounds))
                {
                    Bounds = savedBounds;

                    _stableBounds =
                        savedBounds;

                    _stableWindowState =
                        FormWindowState.Normal;

                    _geometryCaptured = true;
                }
            }
            catch
            {
            }
        }

        // ============================================================
        // RESIZE
        // ============================================================

        private void MasterCostCenter_Resize(
            object sender,
            EventArgs e)
        {
            if (_suppressResizeTracking ||
                _applyingLanguage)
            {
                return;
            }

            if (WindowState ==
                FormWindowState.Minimized)
            {
                return;
            }

            try
            {
                if (WindowState ==
                    FormWindowState.Maximized)
                {
                    _stableWindowState =
                        FormWindowState.Maximized;

                    _geometryCaptured = true;
                }
                else if (
                    WindowState ==
                    FormWindowState.Normal &&
                    IsValidBounds(Bounds))
                {
                    _stableBounds = Bounds;

                    _stableWindowState =
                        FormWindowState.Normal;

                    _geometryCaptured = true;
                }
            }
            catch
            {
            }
        }

        // ============================================================
        // FORM CLOSED
        // ============================================================

        private void MasterCostCenter_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            try
            {
                // ----------------------------------------------------
                // Stop language timer
                // ----------------------------------------------------

                if (_languageTimer != null)
                {
                    _languageTimer.Stop();

                    _languageTimer.Tick -=
                        LanguageTimer_Tick;

                    _languageTimer.Dispose();

                    _languageTimer = null;
                }

                // ----------------------------------------------------
                // Remove // handlers
                // ----------------------------------------------------

                if (lvl1 != null)
                {
                    //.lvl1Account -= lvl1;
                }

                if (lvl2 != null)
                {
                    //.lvl2Account -= lvl2;
                }
            }
            catch
            {
            }
        }

        // ============================================================
        // REFRESH
        // ============================================================

        private void BindRefresh()
        {
            try
            {
                dgvCustomer.Rows.Clear();
                dgvCustomer.Columns.Clear();

                AddDGVColumns();

                BindDGV();
            }
            catch
            {
            }
        }

        // ============================================================
        // GRID COLUMNS
        // ============================================================

        private void AddDGVColumns()
        {
            dgvCustomer.Columns.Add(
                "state",
                "");

            dgvCustomer.Columns.Add(
                "loadState",
                "");

            dgvCustomer.Columns.Add(
                "currLvl",
                "");

            dgvCustomer.Columns.Add(
                "lvl1",
                _isArabic
                    ? "الاسم"
                    : "Name");

            dgvCustomer.Columns.Add(
                "id",
                "");

            dgvCustomer.Columns.Add(
                "Add",
                _isArabic
                    ? "إضافة"
                    : "Add");

            dgvCustomer.Columns.Add(
                "Edit",
                _isArabic
                    ? "تعديل"
                    : "Edit");

            // --------------------------------------------------------
            // Column sizing
            // --------------------------------------------------------

            dgvCustomer.Columns["lvl1"]
                .AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;

            dgvCustomer.Columns["state"]
                .Visible = false;

            dgvCustomer.Columns["id"]
                .Visible = false;

            dgvCustomer.Columns["loadState"]
                .Visible = false;

            dgvCustomer.Columns["currLvl"]
                .Visible = false;

            dgvCustomer.Columns["Add"]
                .Width = 70;

            dgvCustomer.Columns["Edit"]
                .Width = 70;

            // --------------------------------------------------------
            // Existing localization system
            // --------------------------------------------------------

            try
            {
               // LocalizationManager
                    //.LocalizeDataGridViewHeaders(
                        //dgvCustomer);
            }
            catch
            {
            }

            // --------------------------------------------------------
            // Required translations
            // --------------------------------------------------------

            dgvCustomer.Columns["lvl1"].HeaderText =
                _isArabic
                    ? "الاسم"
                    : "Name";

            dgvCustomer.Columns["Add"].HeaderText =
                _isArabic
                    ? "إضافة"
                    : "Add";

            dgvCustomer.Columns["Edit"].HeaderText =
                _isArabic
                    ? "تعديل"
                    : "Edit";

            ApplyGridLanguage();
        }

        // ============================================================
        // LOAD GRID DATA
        // ============================================================

        public void BindDGV()
        {
            dgvCustomer.Rows.Clear();

            using (
                MySqlDataReader reader =
                    ClinicDatabase.ExecuteReader(
                        "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_cost_center"))
            {
                while (reader.Read())
                {
                    dgvCustomer.Rows.Add(
                        "e",
                        "u",
                        "1",
                        "► " +
                        reader["name"].ToString(),
                        reader["id"].ToString());
                }
            }

            ApplyGridLanguage();
        }

        // ============================================================
        // GRID CELL FORMATTING
        // ============================================================

        private void dgvCustomer_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            string columnName =
                dgvCustomer.Columns[
                    e.ColumnIndex].Name;

            if (columnName == "Add" ||
                columnName == "Edit")
            {
                e.CellStyle.ForeColor =
                    Color.Blue;

                e.CellStyle.Font =
                    new Font(
                        dgvCustomer
                            .DefaultCellStyle
                            .Font,
                        FontStyle.Underline);

                e.CellStyle.Alignment =
                    _isArabic
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft;
            }
        }

        // ============================================================
        // GRID DOUBLE CLICK
        // ============================================================

        private void dgvCustomer_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            int lvl;

            bool success =
                int.TryParse(
                    dgvCustomer
                        .Rows[e.RowIndex]
                        .Cells["currLvl"]
                        .Value?
                        .ToString(),
                    out lvl);

            int currLvl =
                success
                    ? lvl
                    : 0;

            // ========================================================
            // SUB LEVEL
            // ========================================================

            if (currLvl >= 2)
            {
                int id;

                if (int.TryParse(
                    dgvCustomer
                        .Rows[e.RowIndex]
                        .Cells["id"]
                        .Value?
                        .ToString(),
                    out id))
                {
                    new frmEditSub(id)
                        .ShowDialog();
                }
                else
                {
                    MessageBox.Show(
                        T(
                            "Invalid ID format.",
                            "تنسيق المعرّف غير صالح."),
                        T(
                            "Cost Center",
                            "مركز التكلفة"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                return;
            }

            string columnName =
                dgvCustomer
                    .Columns[e.ColumnIndex]
                    .Name;

            string state =
                dgvCustomer
                    .Rows[e.RowIndex]
                    .Cells["state"]
                    .Value?
                    .ToString();

            string loadState =
                dgvCustomer
                    .Rows[e.RowIndex]
                    .Cells["loadState"]
                    .Value?
                    .ToString();

            string cellValue =
                dgvCustomer
                    .Rows[e.RowIndex]
                    .Cells["lvl1"]
                    .Value?
                    .ToString();

            string space =
                new string(
                    ' ',
                    currLvl * 6);

            // ========================================================
            // LEVEL 1
            // ========================================================

            if (dgvCustomer.Columns["lvl1"].Index ==
                e.ColumnIndex)
            {
                if (state == "e")
                {
                    if (loadState == "u")
                    {
                        int parentId =
                            int.Parse(
                                dgvCustomer
                                    .Rows[e.RowIndex]
                                    .Cells["id"]
                                    .Value
                                    .ToString());

                        using (
                            MySqlDataReader reader =
                                ClinicDatabase.ExecuteReader(
                                    @"
                                    select CONCAT(CODE , ' - ' , NAME) AS name, id
                                    from tbl_sub_cost_center
                                    WHERE main_id = @parentId",
                                    ClinicDatabase.CreateParameterNew(
                                        "parentId",
                                        parentId)))
                        {
                            int insertIndex =
                                e.RowIndex + 1;

                            while (reader.Read())
                            {
                                string nextState =
                                    (currLvl + 1 == 4)
                                        ? "n"
                                        : "e";

                                dgvCustomer.Rows.Insert(
                                    insertIndex,
                                    nextState,
                                    "u",
                                    currLvl + 1,
                                    space +
                                    "  " +
                                    reader["name"]
                                        .ToString(),
                                    reader["id"]
                                        .ToString());

                                insertIndex++;
                            }
                        }

                        dgvCustomer
                            .Rows[e.RowIndex]
                            .Cells["loadState"]
                            .Value = "l";
                    }
                    else
                    {
                        GetAllChildren(
                            e.RowIndex);
                    }

                    dgvCustomer
                        .Rows[e.RowIndex]
                        .Cells["lvl1"]
                        .Value =
                        (cellValue ?? "")
                            .Replace(
                                "►",
                                "▼");

                    dgvCustomer
                        .Rows[e.RowIndex]
                        .Cells["state"]
                        .Value = "c";
                }
                else if (state == "c")
                {
                    ToggleChildRowsVisibility(
                        e.RowIndex,
                        currLvl,
                        false);

                    dgvCustomer
                        .Rows[e.RowIndex]
                        .Cells["lvl1"]
                        .Value =
                        (cellValue ?? "")
                            .Replace(
                                "▼",
                                "►");

                    dgvCustomer
                        .Rows[e.RowIndex]
                        .Cells["state"]
                        .Value = "e";

                    // ------------------------------------------------
                    // Original code had:
                    //
                    // if (SelectedRows.Count == 0)
                    //
                    // followed by SelectedRows[0].
                    //
                    // That was incorrect and could crash.
                    // ------------------------------------------------

                    if (columnName == "Add")
                    {
                        if (dgvCustomer.SelectedRows.Count > 0)
                        {
                            string idString =
                                dgvCustomer
                                    .SelectedRows[0]
                                    .Cells["id"]
                                    .Value?
                                    .ToString();

                            int id;

                            if (!int.TryParse(
                                idString,
                                out id))
                            {
                                MessageBox.Show(
                                    T(
                                        "Invalid ID format.",
                                        "تنسيق المعرّف غير صالح."),
                                    T(
                                        "Cost Center",
                                        "مركز التكلفة"),
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                                return;
                            }

                            new frmCostCenter(id)
                                .ShowDialog();
                        }
                    }
                }
            }
        }

        // ============================================================
        // GRID MOUSE MOVE
        // ============================================================

        private void dgvCustomer_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            var hitTestInfo =
                dgvCustomer.HitTest(
                    e.X,
                    e.Y);

            if (hitTestInfo.RowIndex >= 0 &&
                hitTestInfo.ColumnIndex >= 0 &&
                (
                    dgvCustomer.Columns[
                        hitTestInfo.ColumnIndex]
                        .Name == "Add" ||

                    dgvCustomer.Columns[
                        hitTestInfo.ColumnIndex]
                        .Name == "Edit"))
            {
                Cursor =
                    Cursors.Hand;
            }
            else
            {
                Cursor =
                    Cursors.Default;
            }
        }

        // ============================================================
        // TOGGLE CHILD ROWS
        // ============================================================

        private void ToggleChildRowsVisibility(
            int parentRowIndex,
            int parentLevel,
            bool visible)
        {
            for (
                int i = parentRowIndex + 1;
                i < dgvCustomer.Rows.Count;
                i++)
            {
                int rowLevel =
                    int.Parse(
                        dgvCustomer
                            .Rows[i]
                            .Cells["currLvl"]
                            .Value
                            .ToString());

                if (rowLevel > parentLevel)
                {
                    if (visible)
                    {
                        dgvCustomer
                            .Rows[i]
                            .Visible = true;

                        if (
                            dgvCustomer
                                .Rows[i]
                                .Cells["state"]
                                .Value
                                .ToString() == "c")
                        {
                            GetAllChildren(
                                dgvCustomer
                                    .Rows[i]
                                    .Index);
                        }
                    }
                    else
                    {
                        dgvCustomer
                            .Rows[i]
                            .Visible = false;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        // ============================================================
        // GET ALL CHILDREN
        // ============================================================

        private void GetAllChildren(
            int parentRowIndex)
        {
            int parentLevel =
                int.Parse(
                    dgvCustomer
                        .Rows[parentRowIndex]
                        .Cells["currLvl"]
                        .Value
                        .ToString());

            for (
                int i = parentRowIndex + 1;
                i < dgvCustomer.Rows.Count;
                i++)
            {
                int rowLevel =
                    int.Parse(
                        dgvCustomer
                            .Rows[i]
                            .Cells["currLvl"]
                            .Value
                            .ToString());

                if (rowLevel > parentLevel)
                {
                    if (rowLevel ==
                        parentLevel + 1)
                    {
                        dgvCustomer
                            .Rows[i]
                            .Visible = true;

                        if (
                            dgvCustomer
                                .Rows[i]
                                .Cells["state"]
                                .Value
                                .ToString() == "c")
                        {
                            GetAllChildren(i);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        // ============================================================
        // GET PARENT ROW
        // ============================================================

        private DataGridViewRow GetParentRow(
            int currentRowIndex)
        {
            if (currentRowIndex <= 0)
            {
                return null;
            }

            int currentLevel =
                int.Parse(
                    dgvCustomer
                        .Rows[currentRowIndex]
                        .Cells["currLvl"]
                        .Value
                        .ToString());

            for (
                int i = currentRowIndex - 1;
                i >= 0;
                i--)
            {
                int rowLevel =
                    int.Parse(
                        dgvCustomer
                            .Rows[i]
                            .Cells["currLvl"]
                            .Value
                            .ToString());

                if (rowLevel ==
                    currentLevel - 1)
                {
                    return dgvCustomer.Rows[i];
                }
            }

            return null;
        }

        // ============================================================
        // NEW COST CENTER
        // ============================================================

        private void btnSave_Click(
            object sender,
            EventArgs e)
        {
            new frmCostCenter(0)
                .ShowDialog();
        }

        // ============================================================
        // HEADER CLICK
        // ============================================================

        private void LbHeader_Click(
            object sender,
            EventArgs e)
        {
            BringToFront();
        }

        // ============================================================
        // MINIMIZE
        // ============================================================

        private void guna2Button1_Click(
            object sender,
            EventArgs e)
        {
            WindowState =
                FormWindowState.Minimized;
        }

        // ============================================================
        // DOUBLE CLICK
        // ============================================================

        private void MasterCostCenter_DoubleClick(
            object sender,
            EventArgs e)
        {
        }
    }
}