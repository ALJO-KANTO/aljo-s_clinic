using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
//using YamyProject.DAL;
//using YamyProject.Localization;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Threading.Tasks;
using Clinic_DataAccess;
using Clinic.Login;
using Clinic;
using Novacode;

namespace YamyProject
{
    public partial class MasterEmployee : Form
    {
        private DataView _dataView;
        private EventHandler employeeUpdateHandler;
        private Panel _modernDirectory;
        private FlowLayoutPanel _employeeCards;
        private FlowLayoutPanel _departmentList;
        private TextBox _modernSearch;
        private Label _resultCount;
        private string _departmentFilter = "All";
        private DataTable _modernEmployees;
        private Form _masterLoadingForm;
        private Thread _masterLoadingThread;

        private volatile bool _masterLoadingReady = false;
        private volatile int _masterLoadingPercent = 5;

        private bool _masterPageLoaded = false;

        public MasterEmployee()
        {
            // Show full application cover FIRST.
            StartMasterEmployeeLoading();

            InitializeComponent();

            // Employee page stays invisible during construction.
            Opacity = 0;
            ShowInTaskbar = false;

           // LocalizationManager.LocalizeForm(this);

            ApplyPageLanguage();

            employeeUpdateHandler = (sender, args) =>
            {
                BindEmployee();

                if (_modernDirectory != null)
                    LoadModernEmployees();

                ApplyPageLanguage();
            };

            //.Employee += employeeUpdateHandler;

            headerUC1.FormText = Text;

            EnableDoubleBuffering(this);
        }
        private async void MasterEmployee_Load(object sender, EventArgs e)
        {
            try
            {
                _masterPageLoaded = false;

                // =====================================================
                // 10%
                // =====================================================
                SetMasterLoadingProgress(
                    10,
                    IsArabic()
                        ? "جاري تجهيز شاشة الموظفين..."
                        : "Preparing employee center...");

                // =====================================================
                // EXISTING LOGIC
                // =====================================================
                ConfigureDataGridViews();

                SetMasterLoadingProgress(
                    20,
                    IsArabic()
                        ? "جاري تحميل الموظفين..."
                        : "Loading employees...");

                // =====================================================
                // EXISTING DATABASE LOGIC
                // =====================================================
                BindEmployee();

                SetMasterLoadingProgress(
                    40,
                    IsArabic()
                        ? "جاري تجهيز بيانات الموظفين..."
                        : "Preparing employee information...");

                pnlInfo.Height = 80;

                headerUC1.FormText =
                    IsArabic()
                        ? "الموارد البشرية"
                        : "Human Resource";

                //AppTheme.DefaultFont(dgvTransactions);

                // =====================================================
                // BUILD MODERN UI
                // =====================================================
                SetMasterLoadingProgress(
                    50,
                    IsArabic()
                        ? "جاري تجهيز دليل الموظفين..."
                        : "Building employee directory...");

                BuildModernEmployeeDirectory();

                SetMasterLoadingProgress(
                    65,
                    IsArabic()
                        ? "جاري تحميل بطاقات الموظفين..."
                        : "Loading employee cards...");

                // =====================================================
                // EXISTING LOGIC
                // =====================================================
                LoadModernEmployees();

                // =====================================================
                // LOCALIZATION
                // =====================================================
                SetMasterLoadingProgress(
                    80,
                    IsArabic()
                        ? "جاري تطبيق اللغة..."
                        : "Applying language...");

               // LocalizationManager.LocalizeForm(this);

               // LocalizationManager.LocalizeDataGridViewHeaders(
                    //

               // LocalizationManager.LocalizeDataGridViewHeaders(
                   //

                // =====================================================
                // EXISTING DIRECTION LOGIC
                // =====================================================
                SetMasterLoadingProgress(
                    88,
                    IsArabic()
                        ? "جاري ترتيب الصفحة..."
                        : "Arranging page...");

                ApplyPageLanguage();

                // =====================================================
                // FINAL LAYOUT
                // =====================================================
                SetMasterLoadingProgress(
                    94,
                    IsArabic()
                        ? "جاري إنهاء التحميل..."
                        : "Finishing...");

                SuspendLayout();

                try
                {
                    PerformLayout();

                    if (_modernDirectory != null)
                    {
                        _modernDirectory.PerformLayout();
                    }

                    if (_employeeCards != null)
                    {
                        _employeeCards.PerformLayout();
                    }

                    if (_departmentList != null)
                    {
                        _departmentList.PerformLayout();
                    }
                }
                finally
                {
                    ResumeLayout(true);
                }

                // Allow queued layout operations to finish.
                await Task.Yield();

                Application.DoEvents();

                // =====================================================
                // NOW SHOW THE COMPLETED PAGE
                // =====================================================
                // =====================================================
                // FINAL PAINT - STILL COMPLETELY COVERED BY LOADER
                // =====================================================

                _masterPageLoaded = true;

                // The real MasterEmployee can now paint,
                // but the full-screen loader is still above it.
                this.Opacity = 1;

                PerformLayout();

                if (_modernDirectory != null)
                {
                    _modernDirectory.PerformLayout();
                }

                if (_employeeCards != null)
                {
                    _employeeCards.PerformLayout();
                }

                if (_departmentList != null)
                {
                    _departmentList.PerformLayout();
                }

                Invalidate(true);
                Update();

                if (_modernDirectory != null)
                {
                    _modernDirectory.Invalidate(true);
                    _modernDirectory.Update();
                }

                Application.DoEvents();

                // Give Windows one paint cycle while the cover is still visible.
                await Task.Delay(80);

                // =====================================================
                // ONLY NOW IS THE PAGE REALLY READY
                // =====================================================
                SetMasterLoadingProgress(
                    100,
                    IsArabic()
                        ? "تم التحميل"
                        : "Ready");

                // Keep 100% visible very briefly.
                await Task.Delay(120);
            }
            catch (Exception ex)
            {
                // Never leave the actual form invisible.
                this.Opacity = 1;

                MessageBox.Show(
                    ex.Message,
                    "Employee Center",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // =====================================================
                // REMOVE LOADER ONLY AFTER PAGE HAS BEEN PAINTED
                // =====================================================
                this.Opacity = 1;

                StopMasterEmployeeLoading();
            }
        }
        private void StartMasterEmployeeLoading()
        {
            _masterLoadingReady = false;
            _masterLoadingPercent = 5;

            bool arabic =
                (Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en")
                .StartsWith(
                    "ar",
                    StringComparison.OrdinalIgnoreCase);

            // =========================================================
            // EMPLOYEE CONTENT AREA ONLY
            //
            // We do NOT use frmMain.Bounds anymore.
            // =========================================================
            Rectangle coverBounds;

            //try
            //{
            //    Screen.PrimaryScreen.WorkingArea;
            //    //if (frmMain != null &&
            //    //    !frmMain.IsDisposed &&
            //    //    frmMain.IsHandleCreated)
            //    //{
            //    //    // pnlMain is the area where child forms are displayed.
            //    //    //Control employeeHost =
            //    //    //    frmMain.Controls.Find(
            //    //    //        "pnlMain",
            //    //    //        true)
            //    //    //    .FirstOrDefault();

            //    //    //if (employeeHost != null)
            //    //    //{
            //    //    //    Point screenLocation =
            //    //    //        employeeHost.PointToScreen(
            //    //    //            Point.Empty);

            //    //    //    coverBounds =
            //    //    //        new Rectangle(
            //    //    //            screenLocation,
            //    //    //            employeeHost.ClientSize);
            //    //    //}
            //    //    //else
            //    //    //{
            //    //    //    // Safe fallback.
            //    //    //    Rectangle clientRectangle =
            //    //    //        frmMain.RectangleToScreen(
            //    //    //            frmMain.ClientRectangle);

            //    //    //    coverBounds =
            //    //    //        clientRectangle;
            //    //    //}
            //    //}
            //    //else
            //    //{
            //    //    coverBounds =
            //    //        Screen.PrimaryScreen.WorkingArea;
            //    //}
            //}
            //catch
            //{
                coverBounds =
                    Screen.PrimaryScreen.WorkingArea;
            //}

            _masterLoadingThread =
                new Thread(() =>
                {
                    try
                    {
                        // =================================================
                        // EMPLOYEE AREA LOADING COVER
                        // =================================================
                        Form loader = new Form
                        {
                            FormBorderStyle =
                                FormBorderStyle.None,

                            StartPosition =
                                FormStartPosition.Manual,

                            Location =
                                coverBounds.Location,

                            Size =
                                coverBounds.Size,

                            ShowInTaskbar = false,
                            ShowIcon = false,

                            TopMost = true,

                            Opacity = 1.0,

                            BackColor =
                                Color.FromArgb(
                                    242,
                                    245,
                                    249)
                        };

                        loader.Text = "";

                        // =================================================
                        // CENTER LOADER CONTAINER
                        // =================================================
                        Panel centerContainer =
                            new Panel
                            {
                                Width = 360,
                                Height = 300,

                                BackColor =
                                    Color.FromArgb(
                                        242,
                                        245,
                                        249)
                            };

                        Action positionCenter = () =>
                        {
                            centerContainer.Location =
                                new Point(
                                    Math.Max(
                                        0,
                                        (loader.ClientSize.Width -
                                         centerContainer.Width) / 2),

                                    Math.Max(
                                        0,
                                        (loader.ClientSize.Height -
                                         centerContainer.Height) / 2)
                                );
                        };

                        positionCenter();

                        // =================================================
                        // CIRCLE
                        // =================================================
                        Panel circlePanel =
                            new Panel
                            {
                                Width = 180,
                                Height = 180,
                                BackColor = Color.Transparent
                            };

                        circlePanel.Location =
                            new Point(
                                (centerContainer.Width -
                                 circlePanel.Width) / 2,
                                15);

                        // =================================================
                        // PERCENTAGE
                        // =================================================
                        Label percentageLabel =
                            new Label
                            {
                                Name =
                                    "lblLoadingPercentage",

                                AutoSize = false,

                                Width = 130,
                                Height = 60,

                                Location =
                                    new Point(
                                        25,
                                        60),

                                TextAlign =
                                    ContentAlignment.MiddleCenter,

                                Font =
                                    new Font(
                                        "Segoe UI",
                                        20F,
                                        FontStyle.Bold),

                                ForeColor =
                                    Color.FromArgb(
                                        45,
                                        125,
                                        210),

                                BackColor =
                                    Color.Transparent,

                                Text = "5%"
                            };

                        circlePanel.Controls.Add(
                            percentageLabel);

                        // =================================================
                        // DRAW CIRCLE
                        // =================================================
                        circlePanel.Paint +=
                            (s, e) =>
                            {
                                e.Graphics.SmoothingMode =
                                    SmoothingMode.AntiAlias;

                                Rectangle outerRectangle =
                                    new Rectangle(
                                        12,
                                        12,
                                        circlePanel.Width - 24,
                                        circlePanel.Height - 24);

                                // Background ring
                                using (Pen backgroundPen =
                                       new Pen(
                                           Color.FromArgb(
                                               215,
                                               225,
                                               238),
                                           9))
                                {
                                    e.Graphics.DrawArc(
                                        backgroundPen,
                                        outerRectangle,
                                        -90,
                                        360);
                                }

                                // Progress
                                float sweep =
                                    360F *
                                    _masterLoadingPercent /
                                    100F;

                                using (Pen progressPen =
                                       new Pen(
                                           Color.FromArgb(
                                               55,
                                               135,
                                               220),
                                           10))
                                {
                                    progressPen.StartCap =
                                        LineCap.Round;

                                    progressPen.EndCap =
                                        LineCap.Round;

                                    e.Graphics.DrawArc(
                                        progressPen,
                                        outerRectangle,
                                        -90,
                                        sweep);
                                }

                                // Inner circle
                                Rectangle innerRectangle =
                                    new Rectangle(
                                        30,
                                        30,
                                        circlePanel.Width - 60,
                                        circlePanel.Height - 60);

                                using (Pen innerPen =
                                       new Pen(
                                           Color.FromArgb(
                                               220,
                                               230,
                                               240),
                                           2))
                                {
                                    e.Graphics.DrawEllipse(
                                        innerPen,
                                        innerRectangle);
                                }
                            };

                        // =================================================
                        // TITLE
                        // =================================================
                        Label loadingTitle =
                            new Label
                            {
                                Name =
                                    "lblLoadingTitle",

                                AutoSize = false,

                                Width =
                                    centerContainer.Width,

                                Height = 32,

                                Location =
                                    new Point(
                                        0,
                                        200),

                                TextAlign =
                                    ContentAlignment.MiddleCenter,

                                Font =
                                    new Font(
                                        "Segoe UI Semibold",
                                        12F,
                                        FontStyle.Bold),

                                ForeColor =
                                    Color.FromArgb(
                                        45,
                                        48,
                                        60),

                                BackColor =
                                    Color.Transparent,

                                Text =
                                    arabic
                                        ? "مركز الموظفين"
                                        : "Employee Center"
                            };

                        // =================================================
                        // STATUS
                        // =================================================
                        Label statusLabel =
                            new Label
                            {
                                Name =
                                    "lblLoadingStatus",

                                AutoSize = false,

                                Width =
                                    centerContainer.Width,

                                Height = 30,

                                Location =
                                    new Point(
                                        0,
                                        235),

                                TextAlign =
                                    ContentAlignment.MiddleCenter,

                                Font =
                                    new Font(
                                        "Segoe UI",
                                        9.5F),

                                ForeColor =
                                    Color.FromArgb(
                                        105,
                                        115,
                                        130),

                                BackColor =
                                    Color.Transparent,

                                Text =
                                    arabic
                                        ? "جاري تحميل بيانات الموظفين..."
                                        : "Loading employee data..."
                            };

                        centerContainer.Controls.Add(
                            circlePanel);

                        centerContainer.Controls.Add(
                            loadingTitle);

                        centerContainer.Controls.Add(
                            statusLabel);

                        loader.Controls.Add(
                            centerContainer);

                        _masterLoadingForm = loader;

                        // =================================================
                        // KEEP CIRCLE CENTERED
                        // =================================================
                        loader.Resize +=
                            (s, e) =>
                            {
                                positionCenter();
                            };

                        // =================================================
                        // PROGRESS ANIMATION
                        // =================================================
                        System.Windows.Forms.Timer
                            animationTimer =
                            new System.Windows.Forms.Timer();

                        animationTimer.Interval = 80;

                        animationTimer.Tick +=
                            (s, e) =>
                            {
                                if (_masterLoadingPercent < 94)
                                {
                                    _masterLoadingPercent++;
                                }

                                percentageLabel.Text =
                                    _masterLoadingPercent +
                                    "%";

                                circlePanel.Invalidate();
                            };

                        // =================================================
                        // READY
                        // =================================================
                        loader.Shown +=
                            (s, e) =>
                            {
                                loader.Refresh();

                                circlePanel.Refresh();

                                percentageLabel.Refresh();

                                statusLabel.Refresh();

                                _masterLoadingReady = true;

                                animationTimer.Start();

                                loader.Activate();

                                loader.BringToFront();
                            };

                        loader.FormClosed +=
                            (s, e) =>
                            {
                                try
                                {
                                    animationTimer.Stop();
                                    animationTimer.Dispose();
                                }
                                catch
                                {
                                }
                            };

                        Application.Run(loader);
                    }
                    catch
                    {
                        _masterLoadingReady = true;
                    }
                });

            _masterLoadingThread.IsBackground = true;

            _masterLoadingThread.SetApartmentState(
                ApartmentState.STA);

            _masterLoadingThread.Start();

            int wait = 0;

            while (!_masterLoadingReady &&
                   wait < 200)
            {
                Thread.Sleep(10);
                wait++;
            }
        }
        private void SetMasterLoadingProgress(
     int percentage,
     string status)
        {
            if (percentage < 0)
                percentage = 0;

            if (percentage > 100)
                percentage = 100;

            _masterLoadingPercent = percentage;

            try
            {
                Form loader =
                    _masterLoadingForm;

                if (loader == null ||
                    loader.IsDisposed ||
                    !loader.IsHandleCreated)
                {
                    return;
                }

                loader.BeginInvoke(
                    new Action(() =>
                    {
                        try
                        {
                            if (loader.IsDisposed)
                                return;

                            Label percentageLabel =
                                FindControlRecursive(
                                    loader,
                                    "lblLoadingPercentage")
                                as Label;

                            Label statusLabel =
                                FindControlRecursive(
                                    loader,
                                    "lblLoadingStatus")
                                as Label;

                            if (percentageLabel != null)
                            {
                                percentageLabel.Text =
                                    _masterLoadingPercent +
                                    "%";
                            }

                            if (statusLabel != null)
                            {
                                statusLabel.Text =
                                    status;
                            }

                            // Find the panel containing percentage.
                            if (percentageLabel?.Parent != null)
                            {
                                percentageLabel.Parent.Invalidate();
                            }
                        }
                        catch
                        {
                            // Loader must never interrupt employee loading.
                        }
                    }));
            }
            catch
            {
            }
        }
        private Control FindControlRecursive(
    Control parent,
    string controlName)
        {
            if (parent == null)
                return null;

            foreach (Control control
                     in parent.Controls)
            {
                if (control.Name == controlName)
                {
                    return control;
                }

                Control result =
                    FindControlRecursive(
                        control,
                        controlName);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        private void StopMasterEmployeeLoading()
        {
            try
            {
                Form loader =
                    _masterLoadingForm;

                if (loader == null ||
                    loader.IsDisposed)
                {
                    return;
                }

                if (loader.InvokeRequired)
                {
                    loader.BeginInvoke(
                        new Action(() =>
                        {
                            try
                            {
                                if (!loader.IsDisposed)
                                {
                                    loader.Close();
                                }
                            }
                            catch
                            {
                            }
                        }));
                }
                else
                {
                    loader.Close();
                }
            }
            catch
            {
            }
            finally
            {
                _masterLoadingReady = false;
            }
        }
        private void EnableDoubleBuffering(
    Control parent)
        {
            if (parent == null)
                return;

            try
            {
                typeof(Control)
                    .GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance)
                    ?.SetValue(
                        parent,
                        true,
                        null);
            }
            catch
            {
            }

            foreach (Control child
                     in parent.Controls)
            {
                EnableDoubleBuffering(child);
            }
        }
        private bool IsArabic()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage;

            return !string.IsNullOrWhiteSpace(language) &&
                   language.StartsWith(
                       "ar",
                       StringComparison.OrdinalIgnoreCase);
        }


        public void ApplyPageLanguage()
        {
            bool arabic = IsArabic();

            SuspendLayout();

            try
            {
                // Do NOT mirror the whole form automatically
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                ApplyMenuLanguage(arabic);

                // NEW - mirror title/menu positions
                ApplyHeaderDirection(arabic);

                ApplyEmployeeListDirection(arabic);
                ApplyProfileDirection(arabic);
                ApplyGridDirection(arabic);
                ApplyModernDirectoryDirection(arabic);

                headerUC1.FormText =
                    arabic
                        ? "مركز الموظفين"
                        : "Employee Center";
            }
            finally
            {
                ResumeLayout(true);
            }

            Invalidate(true);
        }
        private void nToolStripMenuItem_Click(
    object sender,
    EventArgs e)
        {
            frmMain.openChildForm(
                new frmViewEmployee(0));
        }
        private void ApplyHeaderDirection(bool arabic)
        {
            // =========================================================
            // HEADER
            //
            // ENGLISH:
            // [ Employee Center ]                 [ Menus ]
            //
            // ARABIC:
            // [ Menus ]                 [ مركز الموظفين ]
            // =========================================================

            if (arabic)
            {
                // -------------------------------
                // TITLE -> RIGHT
                // -------------------------------
                headerUC1.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Right;

                headerUC1.Left =
                    ClientSize.Width -
                    headerUC1.Width -
                    35;

                headerUC1.RightToLeft =
                    RightToLeft.Yes;


                // -------------------------------
                // MENU -> LEFT
                // -------------------------------
                menuStrip1.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left;

                menuStrip1.Left = 35;

                menuStrip1.RightToLeft =
                    RightToLeft.Yes;
            }
            else
            {
                // -------------------------------
                // TITLE -> LEFT
                // -------------------------------
                headerUC1.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left;

                headerUC1.Left = 35;

                headerUC1.RightToLeft =
                    RightToLeft.No;


                // -------------------------------
                // MENU -> RIGHT
                // -------------------------------
                menuStrip1.Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Right;

                menuStrip1.Left =
                    ClientSize.Width -
                    menuStrip1.Width -
                    35;

                menuStrip1.RightToLeft =
                    RightToLeft.No;
            }
        }
        private void ApplyEmployeeListDirection(bool arabic)
        {
            // =========================================================
            // EMPLOYEE LIST PANEL
            //
            // English -> LEFT
            // Arabic  -> RIGHT
            // =========================================================

            panel3.Dock =
                arabic
                    ? DockStyle.Right
                    : DockStyle.Left;


            // Search
            txtSearch.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            txtSearch.TextAlign =
                arabic
                    ? HorizontalAlignment.Right
                    : HorizontalAlignment.Left;

            txtSearch.PlaceholderText =
                arabic
                    ? "ابحث عن الموظف بالاسم أو الرمز"
                    : "Search By Name, Code";


            // State combo
            cmbState.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;


            // =========================================================
            // EMPLOYEE GRID
            // =========================================================

            dgvCustomer.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            dgvCustomer.ColumnHeadersDefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvCustomer.DefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;


            // Amount should remain numeric/right aligned
            if (dgvCustomer.Columns.Contains("Amount"))
            {
                dgvCustomer.Columns["Amount"]
                    .DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
        }
        private void ApplyProfileDirection(bool arabic)
        {
            Label[] labels =
            {
        label1,
        label2,
        label3,
        label4,
        label6,
        label8,
        label9,
        label10,
        label12,

        lblCode,
        lblName,
        lblPosition,
        lblAddress,
        lblEmail,
        lblPhone
    };

            foreach (Label lbl in labels)
            {
                if (lbl == null)
                    continue;

                lbl.RightToLeft =
                    arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                lbl.TextAlign =
                    arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
            }
        }
        private void ApplyGridDirection(bool arabic)
        {
            dgvTransactions.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            dgvTransactions.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvTransactions.DefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;


            // Keep numeric values right aligned
            string[] numericColumns =
            {
        "Amount",
        "Paid Amt",
        "Balance"
    };

            foreach (string columnName in numericColumns)
            {
                if (dgvTransactions.Columns.Contains(columnName))
                {
                    dgvTransactions.Columns[columnName]
                        .DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }
            }
        }
        private void ApplyMenuLanguage(bool arabic)
        {
            menuStrip1.RightToLeft =
                arabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

            if (arabic)
            {
                newCustomerToolStripMenuItem.Text =
                    "  عمليات الموظفين  ▼";

                nToolStripMenuItem.Text =
                    "موظف جديد";

                editCustomerToolStripMenuItem1.Text =
                    "تعديل الموظف";

                deleteEmployeeToolStripMenuItem.Text =
                    "حذف الموظف";


                newTransactionToolStripMenuItem.Text =
                    "  معاملة جديدة  ▼";

                createInvoiceToolStripMenuItem.Text =
                    "إنشاء فاتورة";

                receivePaymentToolStripMenuItem.Text =
                    "دفع الفواتير";


                printToolStripMenuItem.Text =
                    "  طباعة  ▼";

                cToolStripMenuItem.Text =
                    "قائمة الموظفين";

                transactionListToolStripMenuItem.Text =
                    "قائمة المعاملات";

                creditPaymentToolStripMenuItem.Text =
                    "الائتمان والدفع";


                excelToolStripMenuItem.Text =
                    "  إكسل  ▼";

                importToolStripMenuItem.Text =
                    "استيراد وتصدير";

                toolStripMenuItem1.Text =
                    "قائمة المعاملات";

                toolStripMenuItem2.Text =
                    "الائتمان والدفع";


                wordToolStripMenuItem.Text =
                    "  وورد  ▼";

                customerListToolStripMenuItem1.Text =
                    "قائمة الموظفين";

                toolStripMenuItem4.Text =
                    "قائمة المعاملات";

                toolStripMenuItemword.Text =
                    "الائتمان والدفع";
            }
            else
            {
                newCustomerToolStripMenuItem.Text =
                    "  Employee Operations  ▼";

                nToolStripMenuItem.Text =
                    "New Employee";

                editCustomerToolStripMenuItem1.Text =
                    "Edit Employee";

                deleteEmployeeToolStripMenuItem.Text =
                    "Delete Employee";


                newTransactionToolStripMenuItem.Text =
                    "  New Transactions   ▼";

                createInvoiceToolStripMenuItem.Text =
                    "Create Invoice";

                receivePaymentToolStripMenuItem.Text =
                    "Pay Bills";


                printToolStripMenuItem.Text =
                    "  Print   ▼";

                cToolStripMenuItem.Text =
                    "Employee List";

                transactionListToolStripMenuItem.Text =
                    "Transaction List";

                creditPaymentToolStripMenuItem.Text =
                    "Credit & Payment";


                excelToolStripMenuItem.Text =
                    "  Excel   ▼";

                importToolStripMenuItem.Text =
                    "Import & Export";

                toolStripMenuItem1.Text =
                    "Transaction List";

                toolStripMenuItem2.Text =
                    "Credit & Payment";


                wordToolStripMenuItem.Text =
                    "  Word  ▼";

                customerListToolStripMenuItem1.Text =
                    "Employee List";

                toolStripMenuItem4.Text =
                    "Transaction List";

                toolStripMenuItemword.Text =
                    "Credit & Payment";
            }
        }
        private void ApplyModernDirectoryDirection(bool arabic)
        {
            // =========================================================
            // MODERN EMPLOYEE DIRECTORY
            // =========================================================
            if (_modernDirectory != null)
            {
                // Don't mirror the whole container.
                // We control individual elements instead.
                _modernDirectory.RightToLeft = RightToLeft.No;
            }

            // =========================================================
            // EMPLOYEE CARDS
            // English -> Left to Right
            // Arabic  -> Right to Left
            // =========================================================
            if (_employeeCards != null)
            {
                _employeeCards.RightToLeft =
                    arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                _employeeCards.FlowDirection =
                    arabic
                        ? FlowDirection.RightToLeft
                        : FlowDirection.LeftToRight;

                _employeeCards.WrapContents = true;
                _employeeCards.AutoScroll = true;
            }

            // =========================================================
            // DEPARTMENT LIST
            //
            // IMPORTANT:
            // ALWAYS VERTICAL.
            //
            // Department
            // All
            // Accounts
            // Sales
            // HR
            // ...
            //
            // Never LeftToRight / RightToLeft FlowDirection.
            // =========================================================
            if (_departmentList != null)
            {
                _departmentList.FlowDirection =
                    FlowDirection.TopDown;

                _departmentList.WrapContents = false;

                _departmentList.AutoScroll = true;

                // We only change TEXT direction.
                // We do NOT change the FlowDirection.
                _departmentList.RightToLeft =
                    arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                // Make every department button use the available width.
                int buttonWidth =
                    Math.Max(
                        100,
                        _departmentList.ClientSize.Width -
                        _departmentList.Padding.Left -
                        _departmentList.Padding.Right -
                        SystemInformation.VerticalScrollBarWidth -
                        4);

                foreach (Control control in _departmentList.Controls)
                {
                    if (control is Button button)
                    {
                        button.Width = buttonWidth;

                        button.TextAlign =
                            arabic
                                ? ContentAlignment.MiddleRight
                                : ContentAlignment.MiddleLeft;
                    }
                }
            }

            // =========================================================
            // SEARCH
            // =========================================================
            if (_modernSearch != null)
            {
                _modernSearch.RightToLeft =
                    arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                _modernSearch.TextAlign =
                    arabic
                        ? HorizontalAlignment.Right
                        : HorizontalAlignment.Left;
            }

            // =========================================================
            // RESULT COUNT
            // =========================================================
            if (_resultCount != null)
            {
                _resultCount.RightToLeft =
                    arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                _resultCount.TextAlign =
                    arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
            }
        }
        private void MasterEmployee_FormClosing(object sender, FormClosingEventArgs e)
        {
            //.Employee -= employeeUpdateHandler;
        }
        private void ConfigureDataGridViews()
        {
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvTransactions.EnableHeadersVisualStyles = false;
            dgvTransactions.RowsDefaultCellStyle.BackColor = Color.White;
            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#eaf1fa");
            dgvTransactions.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#D1EAD0");
            dgvTransactions.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvTransactions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvTransactions.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvTransactions.RowHeadersVisible = false;
            dgvTransactions.AllowUserToAddRows = dgvTransactions.AllowUserToResizeRows = false;
        }
        public void BindEmployee()
        {
            // Start building the base query
            string query = @"
                               SELECT 
                                c.id, 
                                c.code,
                                c.name AS Name,
                                p.name AS Position, 
                                c.Position_id,
                                c.address,
                                c.email,
                                c.phone,
                                COALESCE(SUM(
                                    CASE 
                                            WHEN t.type = 'Employee Salary Payment' THEN -IF(t.debit = 0, t.credit, t.debit)
                                            WHEN t.type = 'Petty Cash' THEN -IF(t.debit = 0, t.credit, t.debit)
                                            WHEN t.type = 'Employee Loan Payment' THEN 0 
                                            WHEN t.type = 'Check Cancel (Employee)' THEN t.credit
                                            WHEN t.type = 'Employee Salary' THEN IF(t.debit = 0, t.credit, t.debit)
                                            ELSE 0 
                                        END
                                    ), 0) AS Amount
                                FROM tbl_employee c
                                LEFT JOIN tbl_position p ON c.Position_id = p.id  
                                LEFT JOIN tbl_transaction t ON t.hum_id = c.id AND t.state = 0
                            ";

            // Add filter conditionally based on combo box
            if (cmbState.Text == "Active Employee")
            {
                query += " WHERE c.active = 0";
            }
            else if (cmbState.Text == "Inactive Employee")
            {
                query += " WHERE c.active != 0";
            }

            // Final GROUP BY (must match selected fields except aggregates)
            query += @"
                        GROUP BY 
                            c.id, 
                            c.code, 
                            c.name, 
                            p.name, 
                            c.Position_id,
                            c.address, 
                            c.email, 
                            c.phone;
                ";

            // Execute and bind to DataGridView
            DataTable dt = ClinicDatabase.ExecuteDataTable(query);
            _dataView = dt.DefaultView;
            dgvCustomer.DataSource = _dataView;

            // Format DataGridView
            dgvCustomer.Columns["id"].Visible = false;
            dgvCustomer.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomer.Columns["name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            HideUnusedColumns();
            if (dgvCustomer.Rows.Count > 0)
            {
                // Do NOT load employee transactions here.
                // They will load when the user selects/clicks an employee.

                editCustomerToolStripMenuItem1.Visible =
                    UserPermissions.canEdit("Human Resource Center");

                deleteEmployeeToolStripMenuItem.Visible =
                    UserPermissions.canDelete("Human Resource Center");
            }
            else
            {
                ClearEmployeerDetails();
            }

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvCustomer);
        }
        private void HideUnusedColumns()
        {
            string[] hiddenColumns = { "id", "code", "Position_id", "Position", "phone", "email", "address", "email" };
            foreach (var col in hiddenColumns)
                dgvCustomer.Columns[col].Visible = false;
        }
        private void BindEmployeeInvoice()
        {
            BindCustomerInfo();
            //BindInvoice(new List<string> { "%Purchase Invoice Cash%" }, new List<string>(), dgvInvoiceCash);
            //BindInvoice(new List<string> { "Vendor%", "Purchase%" , "Check Cancel (Vendor)" }, new List<string> { "%Purchase Invoice Cash%" }, dgvInvoiceCredit);
            BindInvoice();
            AdjustColumnWidths();
        }
        private void BindInvoice()
        {
            List<string> includeTypes = new List<string> { "Employee%", "Employee Loan Payment%", "Employee Salary Payment%", "Check Cancel (Employee)", "%Employee Salary%" };
            List<string> excludeTypes = new List<string> { };

            string query = @"WITH CTE AS (
                            SELECT 
                                t.id,
                                t.transaction_id AS `Invoice Id`,
                                ROW_NUMBER() OVER (ORDER BY t.date, t.id) AS SN,
                                t.voucher_no AS `V - No`,
                                t.date AS `Date`,
                                CONCAT(ta.code, ' - ', ta.name) AS `Account Name`,
                                t.type AS `Type`,
                                CASE
                                    WHEN t.type = 'Employee Salary Payment' THEN IF(t.debit = 0, t.credit, t.debit)
                                    WHEN t.type = 'Petty Cash' THEN IF(t.debit = 0, t.credit, t.debit)
                                    WHEN t.type = 'Check Cancel (Employee)' THEN t.credit
                                    WHEN t.type = 'Employee Salary' THEN IF(t.debit = 0, t.credit, t.debit)
                                    WHEN t.type = 'Employee Loan Payment' THEN IF(t.debit = 0, t.credit, t.debit)
                                    WHEN t.type = 'Employee Petty Cash Payment' THEN IF(t.debit = 0, t.credit, t.debit)
                                    ELSE 0
                                END AS `Amount`,
                                CASE
                                    WHEN t.type = 'Employee Salary Payment' THEN IF(t.debit = 0, t.credit, t.debit)
                                    WHEN t.type = 'Petty Cash' THEN IF(t.debit = 0, t.credit, t.debit)
                                    WHEN t.type = 'Employee Loan Payment' THEN IF(t.debit = 0, t.credit, t.debit)
                                     WHEN t.type = 'Employee Petty Cash Payment' THEN IF(t.debit = 0, t.credit, t.debit)
                                    WHEN t.type = 'Check Cancel (Employee)' THEN -t.credit
                                    ELSE 0
                                END AS `Paid Amt`
                
                            FROM tbl_transaction t
                            INNER JOIN tbl_coa_level_4 ta ON t.account_id = ta.id
                            WHERE t.hum_id = @id AND t.state = 0
                              AND (
                                    t.type LIKE 'Employee%' OR 
                                    t.type = 'Employee Loan Payment' OR 
                                    t.type = 'Employee Salary Payment' OR 
                                    t.type = 'Petty Cash' OR 
                                    t.type = 'Check Cancel (Employee)' OR
                                    t.type = 'Employee Petty Cash Payment' OR 
                                    t.type LIKE 'Employee Salary'
                                  )
                        ),
                        Running_Balance AS (
                            SELECT *,
                                SUM(`Amount` - `Paid Amt`) OVER (ORDER BY `Date`, id ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS `Balance`
                            FROM CTE
                        )
                        SELECT 
                            `id`,
                            `Invoice Id`,
                            `SN` as `SN#`,
                            `V - No` as No,
                            `Date`,
                            `Account Name`,
                            `Type`,
                            `Amount`,
                            `Paid Amt`,
                            `Balance`
                        FROM Running_Balance
                        ORDER BY `Date`, id;";

            var parameters = new List<MySqlParameter> { new MySqlParameter("@id", dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()) };

            DataTable dt = ClinicDatabase.ExecuteDataTable(query, parameters.ToArray());
            decimal totalAmount = 0, totalPaidAmount = 0, amount = 0, paidAmount = 0;
            string _type = "";
            foreach (DataRow row in dt.Rows)
            {
                _type = string.IsNullOrEmpty(row["Type"]?.ToString()) ? "" : row["Type"]?.ToString();

                if (decimal.TryParse(row["Amount"]?.ToString(), out amount))
                {
                    totalAmount += amount;
                }
                if (decimal.TryParse(row["Paid Amt"]?.ToString(), out paidAmount))
                {
                    if (_type == "Employee Payment" || _type == "Employee Salary Payment" || _type == "Petty Cash")
                    {
                        totalPaidAmount += paidAmount;
                        totalAmount = totalAmount - paidAmount;
                    }
                    else
                    {
                        decimal _paid = paidAmount;// (paidAmount == 0 ? amount : 0);
                        totalPaidAmount += _paid;
                        //row["Paid Amt"] = _paid.ToString("N2");
                    }
                }
                row["Balance"] = (totalAmount - totalPaidAmount).ToString("F2");
            }
            //DataRow totalRow = dt.NewRow();
            //totalRow["Account Name"] = "Total";
            //totalRow["Amount"] = totalAmount.ToString("F2");
            //dt.Rows.Add(totalRow);
            dgvTransactions.DataSource = dt;
            dgvTransactions.Columns["Amount"].DefaultCellStyle.Alignment = dgvTransactions.Columns["Paid Amt"].DefaultCellStyle.Alignment = dgvTransactions.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTransactions.Columns["Account Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvTransactions.Columns["Amount"].DefaultCellStyle.Format = dgvTransactions.Columns["Paid Amt"].DefaultCellStyle.Format = dgvTransactions.Columns["Balance"].DefaultCellStyle.Format = "N2";
            foreach (DataGridViewColumn column in dgvTransactions.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

           // LocalizationManager.LocalizeDataGridViewHeaders(dgvTransactions);
        }
        private void AdjustColumnWidths()
        {
            dgvTransactions.Columns["id"].Visible = dgvTransactions.Columns["Invoice Id"].Visible = false;
            dgvTransactions.Columns["No"].Width = 80;
            dgvTransactions.Columns["Amount"].Width = dgvTransactions.Columns["Amount"].Width = 90;
            dgvTransactions.Columns["Paid Amt"].Width = 90;
            dgvTransactions.Columns["Balance"].Width = 100;
            dgvTransactions.Columns["Balance"].DefaultCellStyle.Format = "N2";
        }

        private void BindCustomerInfo()
        {
            //var row = dgvCustomer.SelectedRows[0].Cells;
            var eId = dgvCustomer.SelectedRows[0].Cells["id"].Value;
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader("select e.code,e.name,e.address,e.Email,e.phone,e.Position_id,IFNULL((SELECT NAME FROM tbl_position WHERE id =e.Position_id),'') Position from tbl_employee e where e.id = @eId", ClinicDatabase.CreateParameterNew("@eId", eId)))
            {
                if (reader.Read())
                {
                    lblAddress.Text = string.IsNullOrWhiteSpace(reader["address"]?.ToString()) ? "-" : reader["address"].ToString();
                    lblEmail.Text = string.IsNullOrWhiteSpace(reader["Email"]?.ToString()) ? "-" : reader["Email"].ToString();
                    lblPhone.Text = string.IsNullOrWhiteSpace(reader["phone"]?.ToString()) ? "-" : reader["phone"].ToString();
                    lblPosition.Text = string.IsNullOrWhiteSpace(reader["Position"]?.ToString()) ? "-" : reader["Position"].ToString();

                    lblCode.Text = reader["code"]?.ToString();
                    lblName.Text = reader["name"]?.ToString();
                }
            }
        }
        private void ClearEmployeerDetails()
        {
            dgvTransactions.DataSource = null;
            lblCode.Text = lblAddress.Text = lblPhone.Text = lblEmail.Text = lblName.Text = lblPosition.Text = lblPosition.Text = "-";
        }
        private void dgvInvoice_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvTransactions.Rows.Count)
            {
                if (dgvTransactions.Columns.Contains("type"))
                {
                    if (CommonInsert.GetFormName(dgvTransactions.Rows[e.RowIndex].Cells["type"].Value.ToString()) == "Purchase")
                    {
                        frmMain.openChildForm(new frmPurchase(int.Parse(dgvTransactions.Rows[e.RowIndex].Cells["Invoice Id"].Value.ToString())));
                    }
                    else if (dgvTransactions.Rows[e.RowIndex].Cells["type"].Value.ToString() == ("Purchase Invoice Cash") || dgvTransactions.Rows[e.RowIndex].Cells["type"].Value.ToString() == ("Purchase Invoice"))
                    {
                        //frmMain.openChildForm(new frmPurchaseReturn(int.Parse(dgvTransactions.Rows[e.RowIndex].Cells["Invoice Id"].Value.ToString())));
                    }
                    //if (dgvTransactions.Rows[e.RowIndex].Cells["type"].Value.ToString().ToUpper().Contains("PURCHASE"))
                    //    frmMain.openChildForm(new frmPurchase(int.Parse(dgvTransactions.Rows[e.RowIndex].Cells["Invoice Id"].Value.ToString())));
                }
            }
        }
        private void dgvInvoiceCash_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0 && e.RowIndex < dgvTransactions.Rows.Count)
            //{
            //    if (dgvTransactions.Columns.Contains("type"))
            //    {
            //        if (dgvTransactions.Rows[e.RowIndex].Cells["type"].Value.ToString().ToUpper().Contains("PURCHASE"))
            //            frmMain.openChildForm(new frmPurchase(int.Parse(dgvInvoiceCash.Rows[e.RowIndex].Cells["Invoice Id"].Value.ToString())));
            //    }
            //}
        }
        private void btnExpand_Click(object sender, EventArgs e)
        {
            //if (pnlInfo.Height == 80)
            //{
            //    pnlInfo.Height = 240;
            //    btnExpand.Text = "Less";
            //}
            //else
            //{
            //    pnlInfo.Height = 80;
            //    btnExpand.Text = "More";
            //}
        }
        private void createInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmPurchase(0, "", int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (s, ev) =>
            {
                int leftMargin = 50;
                int rightMargin = 50;
                int yPosition = 50;
                int lineHeight = 30;
                int pageWidth = ev.PageBounds.Width;
                int availableWidth = pageWidth - leftMargin - rightMargin;

                Font headerFont = new Font("Times New Roman", 16, FontStyle.Bold);
                Font columnHeaderFont = new Font("Times New Roman", 10, FontStyle.Bold);
                Font cellFont = new Font("Times New Roman", 10);

                // Center title
                string[] headerLines = {
                                            "Employee Data Report",
                                            " Employee ",
                                            "All Employee"
                                        };
                foreach (string line in headerLines)
                {
                    SizeF textSize = ev.Graphics.MeasureString(line, headerFont);
                    ev.Graphics.DrawString(line, headerFont, Brushes.Black, new PointF((pageWidth - textSize.Width) / 2, yPosition));
                    yPosition += (int)textSize.Height + 5;
                }

                yPosition += 10;
                string dateText = "Printed on: " + DateTime.Now.ToString("MM/dd/yyyy");
                ev.Graphics.DrawString(dateText, new Font("Segoe UI", 10), Brushes.Black, new PointF(leftMargin, yPosition));
                yPosition += 25;

                // Column widths
                int nameWidth = (int)(availableWidth * 0.6);
                int amountWidth = availableWidth - nameWidth;
                int colTop = yPosition;

                // Draw column headers
                ev.Graphics.FillRectangle(Brushes.White, leftMargin, yPosition, nameWidth, lineHeight);
                ev.Graphics.FillRectangle(Brushes.White, leftMargin + nameWidth, yPosition, amountWidth, lineHeight);

                StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                ev.Graphics.DrawString("Name", columnHeaderFont, Brushes.Black, new RectangleF(leftMargin, yPosition, nameWidth, lineHeight), centerFormat);
                ev.Graphics.DrawString("Amount", columnHeaderFont, Brushes.Black, new RectangleF(leftMargin + nameWidth, yPosition, amountWidth, lineHeight), centerFormat);

                ev.Graphics.DrawRectangle(Pens.Black, leftMargin, yPosition, nameWidth, lineHeight);
                ev.Graphics.DrawRectangle(Pens.Black, leftMargin + nameWidth, yPosition, amountWidth, lineHeight);

                yPosition += lineHeight;

                // Row text formats
                //StringFormat nameFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };     // RIGHT
                StringFormat nameFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                StringFormat amountFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center }; // LEFT

                // Print rows
                foreach (DataGridViewRow row in dgvCustomer.Rows)
                {
                    if (row.IsNewRow) continue;

                    string name = row.Cells["Name"].Value?.ToString() ?? "";
                    string amount = row.Cells["Amount"].FormattedValue?.ToString() ?? "";

                    // Draw name (right aligned)
                    ev.Graphics.DrawString(name, cellFont, Brushes.Black,
                        new RectangleF(leftMargin, yPosition, nameWidth, lineHeight), nameFormat);

                    // Draw amount (left aligned)
                    ev.Graphics.DrawString(amount, cellFont, Brushes.Black,
                        new RectangleF(leftMargin + nameWidth, yPosition, amountWidth, lineHeight), amountFormat);

                    // Draw borders
                    ev.Graphics.DrawRectangle(Pens.Black, leftMargin, yPosition, nameWidth, lineHeight);
                    ev.Graphics.DrawRectangle(Pens.Black, leftMargin + nameWidth, yPosition, amountWidth, lineHeight);

                    yPosition += lineHeight;

                    if (yPosition > ev.MarginBounds.Bottom)
                    {
                        ev.HasMorePages = true;
                        return;
                    }
                }

                ev.HasMorePages = false;
            };

            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 1000,
                Height = 800
            };
            previewDialog.ShowDialog();
        }

        private void customerListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Save Excel File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelExporter exporter = new ExcelExporter(dgvCustomer);
                exporter.ExportToExcel(saveFileDialog.FileName);
            }
        }
        private void exportTransactionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Save Excel File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelExporter exporter = new ExcelExporter(dgvTransactions);
                exporter.ExportToExcel(saveFileDialog.FileName);
            }
        }
        private void customerListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Documents|*.docx",
                Title = "Save Word Document"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenXmlWordExporter wordExporter = new OpenXmlWordExporter();
                wordExporter.ExportToWord(dgvCustomer, saveFileDialog.FileName);
            }
        }
        private void transactionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Documents|*.docx",
                Title = "Save Word Document"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenXmlWordExporter wordExporter = new OpenXmlWordExporter();
                wordExporter.ExportToWord(dgvTransactions, saveFileDialog.FileName);
            }
        }
        private void cmbState_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindEmployee();
        }
        private void editCustomerToolStripMenuItem1_Click(
       object sender,
       EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            int employeeId =
                int.Parse(
                    dgvCustomer
                        .SelectedRows[0]
                        .Cells["id"]
                        .Value
                        .ToString());

            frmMain.openChildForm(
                new frmViewEmployee(employeeId));
        }
        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //BindInvoice(new List<string> { "%Purchase Invoice Cash%" }, new List<string>(), dgvInvoiceCash);
            //BindInvoice(new List<string> { "Vendor%", "Purchase%" ,"Check Cancel"}, new List<string> { "%Purchase Invoice Cash%" }, dgvTransactions);
            BindCustomerInfo();
            BindInvoice();
            ConfigureDataGridViews();
            AdjustColumnWidths();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _dataView.RowFilter = "name like '%" + txtSearch.Text + "%'";
        }
        string GetCompanyName()
        {
            return ClinicDatabase.ExecuteScalar("select name from tbl_company LIMIT 1")?.ToString() ?? "";
        }
        private void creditPaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer.");
                return;
            }

            string companyName = GetCompanyName(); // from DB
            string customerName = dgvCustomer.SelectedRows[0].Cells["Name"].Value?.ToString() ?? "Unknown Employee";

            PrintDocument printDocument = new PrintDocument();
            printDocument.DefaultPageSettings.Landscape = true;

            printDocument.PrintPage += (s, ev) =>
            {
                int leftMargin = 50;
                int rightMargin = 50;
                int topMargin = 50;
                int yPosition = topMargin;
                int lineHeight = 40;
                int pageWidth = ev.PageBounds.Width;
                int availableWidth = pageWidth - leftMargin - rightMargin;

                // Fonts
                Font companyFont = new Font("Times New Roman", 18, FontStyle.Bold);
                Font subTitleFont = new Font("Times New Roman", 14, FontStyle.Bold);
                Font labelFont = new Font("Times New Roman", 11, FontStyle.Regular);
                Font colFont = new Font("Times New Roman", 11, FontStyle.Bold);
                Font cellFont = new Font("Times New Roman", 11);

                // 1. Time and Date (Top-left)
                string timeNow = DateTime.Now.ToString("hh:mm tt");
                string dateNow = DateTime.Now.ToString("dd/MM/yyyy");

                ev.Graphics.DrawString(timeNow, labelFont, Brushes.Black, leftMargin, yPosition);
                yPosition += (int)labelFont.GetHeight(ev.Graphics);

                ev.Graphics.DrawString(dateNow, labelFont, Brushes.Black, leftMargin, yPosition);
                yPosition += (int)labelFont.GetHeight(ev.Graphics);

                // 2. Company Name (Center)
                SizeF companySize = ev.Graphics.MeasureString(companyName, companyFont);
                ev.Graphics.DrawString(companyName, companyFont, Brushes.Black,
                    new PointF((pageWidth - companySize.Width) / 2, topMargin)); // align with original top

                // 3. Subtitle
                yPosition += 10;
                string subtitle = "All Transactions for " + customerName;
                SizeF subtitleSize = ev.Graphics.MeasureString(subtitle, subTitleFont);
                ev.Graphics.DrawString(subtitle, subTitleFont, Brushes.Black,
                    new PointF((pageWidth - subtitleSize.Width) / 2, yPosition));
                yPosition += (int)subtitleSize.Height + 10;

                // 4. Horizontal bold line
                Pen boldPen = new Pen(Color.Black, 4);
                ev.Graphics.DrawLine(boldPen, leftMargin, yPosition, pageWidth - rightMargin, yPosition);
                yPosition += 15;

                // 5. Column definitions
                string[] columnNames = { "SN#", "No", "Date", "Account Name", "Type", "Amount", "Balance" };

                Dictionary<string, float> widthRatios = new Dictionary<string, float>
    {
        { "SN#", 0.06f },
        { "No", 0.08f },
        { "Date", 0.10f },
        { "Account Name", 0.24f },
        { "Type", 0.18f },
        { "Amount", 0.17f },
        { "Balance", 0.17f }
    };

                Dictionary<string, int> columnWidths = new Dictionary<string, int>();
                foreach (var col in columnNames)
                {
                    columnWidths[col] = (int)(availableWidth * widthRatios[col]);
                }

                // 6. Draw Table Headers
                int rowX = leftMargin;
                foreach (string col in columnNames)
                {
                    ev.Graphics.DrawString(col, colFont, Brushes.Black,
                        new RectangleF(rowX, yPosition, columnWidths[col], lineHeight),
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    ev.Graphics.DrawRectangle(Pens.Black, rowX, yPosition, columnWidths[col], lineHeight);
                    rowX += columnWidths[col];
                }
                yPosition += lineHeight;

                // 7. Draw Table Rows
                foreach (DataGridViewRow row in dgvTransactions.Rows)
                {
                    if (row.IsNewRow) continue;
                    rowX = leftMargin;

                    foreach (string col in columnNames)
                    {
                        string value = row.Cells[col].FormattedValue?.ToString() ?? "";
                        RectangleF cellBounds = new RectangleF(rowX, yPosition, columnWidths[col], lineHeight);

                        StringFormat format = new StringFormat { LineAlignment = StringAlignment.Center };
                        if (col == "Amount" || col == "Balance")
                            format.Alignment = StringAlignment.Far;
                        else
                            format.Alignment = StringAlignment.Near;

                        ev.Graphics.DrawString(value, cellFont, Brushes.Black, cellBounds, format);
                        ev.Graphics.DrawRectangle(Pens.Black, rowX, yPosition, columnWidths[col], lineHeight);
                        rowX += columnWidths[col];
                    }

                    yPosition += lineHeight;

                    if (yPosition + lineHeight > ev.MarginBounds.Bottom)
                    {
                        ev.HasMorePages = true;
                        return;
                    }
                }

                ev.HasMorePages = false;
            };

            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 1000,
                Height = 800
            };
            previewDialog.ShowDialog();
        }
        private void cashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //    // Get selected customer details
            //    string customerCode = "-", customerName = "-";
            //    if (dgvCustomer.SelectedRows.Count > 0)
            //    {
            //        string fullName = dgvCustomer.SelectedRows[0].Cells["Name"].Value?.ToString() ?? "";
            //        var parts = fullName.Split(new[] { '-' }, 2);
            //        if (parts.Length == 2)
            //        {
            //            customerCode = parts[0].Trim();
            //            customerName = parts[1].Trim();
            //        }
            //        else
            //        {
            //            customerName = fullName;
            //        }
            //    }

            //    string companyName = GetCompanyName(); // Company name from DB

            //    PrintDocument printDocument = new PrintDocument();
            //    printDocument.DefaultPageSettings.Landscape = true;

            //    printDocument.PrintPage += (s, ev) =>
            //    {
            //        int leftMargin = 50;
            //        int rightMargin = 50;
            //        int topMargin = 50;
            //        int yPosition = topMargin;
            //        int lineHeight = 30;
            //        int pageWidth = ev.PageBounds.Width;
            //        int availableWidth = pageWidth - leftMargin - rightMargin;

            //        ev.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //        // Fonts
            //        Font companyFont = new Font("Times New Roman", 18, FontStyle.Bold);
            //        Font subTitleFont = new Font("Times New Roman", 14, FontStyle.Bold);
            //        Font titleFont = new Font("Times New Roman", 13, FontStyle.Bold);
            //        Font colFont = new Font("Times New Roman", 11, FontStyle.Bold);
            //        Font cellFont = new Font("Times New Roman", 11);
            //        Font labelFont = new Font("Times New Roman", 11);

            //        // Time & Date (Top Left)
            //        string timeNow = DateTime.Now.ToString("hh:mm tt");
            //        string dateNow = DateTime.Now.ToString("dd/MM/yyyy");

            //        ev.Graphics.DrawString(timeNow, labelFont, Brushes.Black, leftMargin, yPosition);
            //        yPosition += (int)labelFont.GetHeight(ev.Graphics);
            //        ev.Graphics.DrawString(dateNow, labelFont, Brushes.Black, leftMargin, yPosition);
            //        yPosition += (int)labelFont.GetHeight(ev.Graphics) + 10;

            //        // Company Name (Centered)
            //        SizeF companySize = ev.Graphics.MeasureString(companyName, companyFont);
            //        ev.Graphics.DrawString(companyName, companyFont, Brushes.Black,
            //            new PointF((pageWidth - companySize.Width) / 2, topMargin));

            //        // SubTitle
            //        string subTitle = "All Transactions for " + customerCode + " - " + customerName;
            //        SizeF subSize = ev.Graphics.MeasureString(subTitle, subTitleFont);
            //        ev.Graphics.DrawString(subTitle, subTitleFont, Brushes.Black,
            //            new PointF((pageWidth - subSize.Width) / 2, yPosition));
            //        yPosition += (int)subSize.Height + 15;

            //        // Report Title
            //        SizeF reportTitleSize = ev.Graphics.MeasureString("Cash Invoice Report", titleFont);
            //        ev.Graphics.DrawString("Cash Invoice Report", titleFont, Brushes.Black,
            //            new PointF((pageWidth - reportTitleSize.Width) / 2, yPosition));
            //        yPosition += (int)reportTitleSize.Height + 10;

            //        // Horizontal Line
            //        Pen boldPen = new Pen(Color.Black, 3);
            //        ev.Graphics.DrawLine(boldPen, leftMargin, yPosition, pageWidth - rightMargin, yPosition);
            //        yPosition += 15;

            //        // Table Layout
            //        string[] columns = { "SN", "V - No", "Date", "Account Name", "Amount" };
            //        Dictionary<string, float> widthRatio = new Dictionary<string, float>
            //{
            //    { "SN", 0.08f },
            //    { "V - No", 0.12f },
            //    { "Date", 0.14f },
            //    { "Account Name", 0.46f },
            //    { "Amount", 0.20f }
            //};
            //        Dictionary<string, int> widths = columns.ToDictionary(c => c, c => (int)(availableWidth * widthRatio[c]));

            //        // Draw headers
            //        int x = leftMargin;
            //        foreach (var col in columns)
            //        {
            //            ev.Graphics.DrawString(col, colFont, Brushes.Black,
            //                new RectangleF(x, yPosition, widths[col], lineHeight),
            //                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            //            ev.Graphics.DrawRectangle(Pens.Black, x, yPosition, widths[col], lineHeight);
            //            x += widths[col];
            //        }
            //        yPosition += lineHeight;

            //        // Draw data rows
            //        foreach (DataGridViewRow row in dgvInvoiceCash.Rows)
            //        {
            //            if (row.IsNewRow) continue;
            //            x = leftMargin;

            //            foreach (var col in columns)
            //            {
            //                string text = row.Cells[col].FormattedValue?.ToString() ?? "";
            //                RectangleF bounds = new RectangleF(x, yPosition, widths[col], lineHeight);

            //                StringFormat format = new StringFormat { LineAlignment = StringAlignment.Center };
            //                if (col == "Amount")
            //                    format.Alignment = StringAlignment.Far;
            //                else
            //                    format.Alignment = StringAlignment.Near;

            //                ev.Graphics.DrawString(text, cellFont, Brushes.Black, bounds, format);
            //                ev.Graphics.DrawRectangle(Pens.Black, x, yPosition, widths[col], lineHeight);
            //                x += widths[col];
            //            }

            //            yPosition += lineHeight;

            //            if (yPosition + lineHeight > ev.MarginBounds.Bottom)
            //            {
            //                ev.HasMorePages = true;
            //                return;
            //            }
            //        }

            //        ev.HasMorePages = false;
            //    };

            //    PrintPreviewDialog previewDialog = new PrintPreviewDialog
            //    {
            //        Document = printDocument,
            //        Width = 1000,
            //        Height = 800
            //    };
            //    previewDialog.ShowDialog();
        }
        private void ExportToExcel(DataGridView dgvTransactions, string customerName, decimal customerBalance)
        {
            Excel.Application xlApp = new Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!");
                return;
            }

            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            int row = 1;

            // Customer Info
            xlWorkSheet.Cells[row, 1] = "Employee Name:";
            xlWorkSheet.Cells[row, 2] = customerName;
            row++;
            xlWorkSheet.Cells[row, 1] = "Balance:";
            xlWorkSheet.Cells[row, 2] = customerBalance.ToString("N2");
            row += 2;

            // Column Headers
            for (int i = 2; i < dgvTransactions.Columns.Count; i++)
            {
                xlWorkSheet.Cells[row, i - 1] = dgvTransactions.Columns[i].HeaderText;
            }
            row++;

            // Data Rows
            for (int i = 0; i < dgvTransactions.Rows.Count; i++)
            {
                for (int j = 2; j < dgvTransactions.Columns.Count; j++)
                {
                    var cellValue = dgvTransactions.Rows[i].Cells[j].Value;
                    xlWorkSheet.Cells[row + i, j - 1] = cellValue?.ToString();
                }
            }

            xlWorkSheet.Columns.AutoFit();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                xlWorkBook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Exported Successfully!");
            }

            xlWorkBook.Close(false);
            xlApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.CurrentRow == null)
            {
                MessageBox.Show("Please select a Employee first.");
                return;
            }

            try
            {
                string customerName = dgvCustomer.CurrentRow.Cells["Name"].Value.ToString();

                // Sum of all Balance values
                decimal customerBalance = 0;
                foreach (DataGridViewRow row in dgvTransactions.Rows)
                {
                    decimal value;
                    if (row.Cells["Balance"].Value != null && decimal.TryParse(row.Cells["Balance"].Value.ToString(), out value))
                    {
                        customerBalance += value;
                    }
                }

                ExportToExcel(dgvTransactions, customerName, customerBalance);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message);
            }
        }
        private void ExportCashInvoiceToExcel(DataGridView dgv, string customerName)
        {
            //Excel.Application xlApp = new Excel.Application();
            //if (xlApp == null)
            //{
            //    MessageBox.Show("Excel is not properly installed!");
            //    return;
            //}

            //Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            //Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //int row = 1;

            //// Header info
            //xlWorkSheet.Cells[row, 1] = "Employee Name:";
            //xlWorkSheet.Cells[row, 2] = customerName;
            //row += 2;

            //// Get column indexes to export (skip "id", "Invoice Id", and "Balance")
            //List<int> exportColumns = new List<int>();
            //for (int i = 0; i < dgv.Columns.Count; i++)
            //{
            //    string colName = dgv.Columns[i].HeaderText.ToLower();
            //    if (colName != "id" && colName != "invoice id" && colName != "balance")
            //    {
            //        exportColumns.Add(i);
            //    }
            //}

            //// Column Headers
            //for (int i = 0; i < exportColumns.Count; i++)
            //{
            //    xlWorkSheet.Cells[row, i + 1] = dgv.Columns[exportColumns[i]].HeaderText;
            //}
            //row++;

            //// Data Rows
            //for (int i = 0; i < dgv.Rows.Count; i++)
            //{
            //    for (int j = 0; j < exportColumns.Count; j++)
            //    {
            //        var cellValue = dgv.Rows[i].Cells[exportColumns[j]].Value;
            //        xlWorkSheet.Cells[row + i, j + 1] = cellValue?.ToString();
            //    }
            //}

            //xlWorkSheet.Columns.AutoFit();

            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            //if (saveFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    xlWorkBook.SaveAs(saveFileDialog.FileName);
            //    MessageBox.Show("Cash Invoice Exported Successfully!");
            //}

            //xlWorkBook.Close(false);
            //xlApp.Quit();

            //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //if (dgvCustomer.CurrentRow == null)
            //{
            //    MessageBox.Show("Please select a customer first.");
            //    return;
            //}

            //try
            //{
            //    string customerName = dgvCustomer.CurrentRow.Cells["Name"].Value.ToString();
            //    ExportCashInvoiceToExcel(dgvInvoiceCash, customerName);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Export failed: " + ex.Message);
            //}
        }
        private void dgvCustomer_CellDoubleClick(
      object sender,
      DataGridViewCellEventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;

            int employeeId =
                int.Parse(
                    dgvCustomer
                        .SelectedRows[0]
                        .Cells["id"]
                        .Value
                        .ToString());

            frmMain.openChildForm(
                new frmViewEmployee(employeeId));
        }

        private void receivePaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewPaymentVoucher(0));

        }
        private void DeleteData(int _id)
        {
            ClinicDatabase.ExecuteNonQuery(@"Delete from tbl_employee where id = @id;Delete from tbl_transaction where transaction_id=@id",
                ClinicDatabase.CreateParameterNew("id", _id));
            ////Utilities.LogAudit(-1, "Delete Employee", "HR", _id, "Deleted Employee with ID: " + _id);
            BindEmployee();
            MessageBox.Show("Employee deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void toolStripMenuItemword_Click(object sender, EventArgs e)
        {
            //if (dgvCustomer.CurrentRow == null)
            //{
            //    MessageBox.Show("Please select a Employee.");
            //    return;
            //}

            //string customerName = dgvCustomer.CurrentRow.Cells["Name"].Value.ToString();

            //SaveFileDialog saveDialog = new SaveFileDialog();
            //saveDialog.Filter = "Word Document (*.docx)|*.docx";
            //saveDialog.Title = "Save Cash Invoices";

            //if (saveDialog.ShowDialog() == DialogResult.OK)
            //{
            //    ExportCashToWord(dgvInvoiceCash, customerName, saveDialog.FileName);
            //    MessageBox.Show("Cash Invoice Exported Successfully!");
            //}
        }

        private void toolStripMenuItemCashWord_Click(object sender, EventArgs e)
        {

            //if (dgvCustomer.CurrentRow == null)
            //{
            //    MessageBox.Show("Please select a customer.");
            //    return;
            //}

            //string customerName = dgvCustomer.CurrentRow.Cells["Name"].Value.ToString();

            //SaveFileDialog saveDialog = new SaveFileDialog();
            //saveDialog.Filter = "Word Document (*.docx)|*.docx";
            //saveDialog.Title = "Save Cash Invoices";

            //if (saveDialog.ShowDialog() == DialogResult.OK)
            //{
            //    ExportCashToWord(dgvInvoiceCash, customerName, saveDialog.FileName);
            //    MessageBox.Show("Cash Invoice Exported Successfully!");
            //}
        }
        private void ExportCashToWord(DataGridView dgv, string customerName, string filePath)
        {
            var doc = DocX.Create(filePath);

            // Header
            var title = doc.InsertParagraph("Employee Name: " + customerName)
                           .FontSize(14)
                           .Bold()
                           .SpacingAfter(20);

            // Determine columns to export (skip "id", "invoice id")
            List<int> includedCols = new List<int>();
            for (int c = 0; c < dgv.Columns.Count; c++)
            {
                string header = dgv.Columns[c].HeaderText.ToLower();
                if (header != "id" && header != "invoice id")
                {
                    includedCols.Add(c);
                }
            }

            int rows = dgv.Rows.Count + 1;
            int cols = includedCols.Count;
            Table table = doc.AddTable(rows, cols);
            table.Design = TableDesign.TableGrid;

            // Set column width and padding
            foreach (var row in table.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    cell.Width = 100;
                    cell.MarginLeft = 5;
                    cell.MarginRight = 5;
                }
            }

            // Header row
            for (int i = 0; i < includedCols.Count; i++)
            {
                table.Rows[0].Cells[i].Paragraphs[0]
                    .Append(dgv.Columns[includedCols[i]].HeaderText)
                    .Bold();
            }

            // Data rows
            for (int r = 0; r < dgv.Rows.Count; r++)
            {
                for (int c = 0; c < includedCols.Count; c++)
                {
                    var val = dgv.Rows[r].Cells[includedCols[c]].Value?.ToString() ?? "";
                    table.Rows[r + 1].Cells[c].Paragraphs[0].Append(val);
                }
            }

            doc.InsertTable(table);
            doc.Save();
        }


        // =============================================================
        // MODERN HUMAN RESOURCE / EMPLOYEE DIRECTORY
        // Odoo-style card list with search and department/position filter.
        // =============================================================
        private void BuildModernEmployeeDirectory()
        {
            if (_modernDirectory != null)
                return;

            // Keep the old controls alive for the existing accounting/print code,
            // but show the modern employee directory instead.
            panel2.Visible = false;
            panel3.Visible = false;
            panel10.Visible = false;
            pnlInfo.Visible = false;

            _modernDirectory = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(247, 248, 252)
            };
            Controls.Add(_modernDirectory);
            _modernDirectory.BringToFront();

            // IMPORTANT: use a table layout instead of overlapping Dock=Fill panels.
            // Row 0 = toolbar, Row 1 = content. They can never overlap.
            TableLayoutPanel page = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.FromArgb(247, 248, 252)
            };
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            page.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _modernDirectory.Controls.Add(page);

            Panel top = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = Padding.Empty
            };
            page.Controls.Add(top, 0, 0);

            Button btnNew = new Button
            {
                Text = "+ New",
                Width = 82,
                Height = 40,
                Location = new Point(14, 16),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(112, 82, 155),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.Click += (s, e) =>
            {
                frmMain.openChildForm(
                    new frmViewEmployee(0));
            };
            top.Controls.Add(btnNew);

            Label title = new Label
            {
                AutoSize = true,
                Text = "Employees",
                Font = new Font("Segoe UI", 15F),
                ForeColor = Color.FromArgb(45, 48, 60),
                Location = new Point(108, 21)
            };
            top.Controls.Add(title);

            _resultCount = new Label
            {
                AutoSize = false,
                Width = 90,
                Height = 32,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(65, 67, 82),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            top.Controls.Add(_resultCount);

            _modernSearch = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                Width = 360,
                Height = 34,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _modernSearch.TextChanged += (s, e) => RenderEmployeeCards();
            top.Controls.Add(_modernSearch);

            Label searchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 18F),
                AutoSize = false,
                Width = 34,
                Height = 32,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(65, 67, 82),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            top.Controls.Add(searchIcon);

            Action positionTopControls = () =>
            {
                _resultCount.Left = Math.Max(0, top.ClientSize.Width - _resultCount.Width - 18);
                _resultCount.Top = 18;

                int searchWidth = Math.Min(360, Math.Max(220, top.ClientSize.Width / 3));
                _modernSearch.Width = searchWidth;
                _modernSearch.Left = Math.Max(330, _resultCount.Left - searchWidth - 28);
                _modernSearch.Top = 18;

                searchIcon.Left = _modernSearch.Left - searchIcon.Width;
                searchIcon.Top = 18;
            };
            top.Resize += (s, e) => positionTopControls();
            top.HandleCreated += (s, e) => positionTopControls();

            // Content uses fixed left column + remaining card area.
            // This also prevents the department panel from covering employee cards.
            TableLayoutPanel content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.FromArgb(247, 248, 252)
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            page.Controls.Add(content, 0, 1);

            Panel left = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = new Padding(14, 10, 10, 10)
            };
            content.Controls.Add(left, 0, 0);

            Label deptTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                Text = "DEPARTMENT",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 48, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };
            left.Controls.Add(deptTitle);

            _departmentList = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(0, 4, 0, 0)
            };
            left.Controls.Add(_departmentList);
            _departmentList.BringToFront();

            _employeeCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(247, 248, 252),
                Margin = Padding.Empty,
                Padding = new Padding(14, 14, 10, 14)
            };
            content.Controls.Add(_employeeCards, 1, 0);

            _employeeCards.Resize += (s, e) =>
            {
                if (_modernEmployees != null && IsHandleCreated && !IsDisposed)
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (!IsDisposed)
                            RenderEmployeeCards();
                    }));
                }
            };
        }

        private void LoadModernEmployees()
        {
            try
            {
                // The current database stores the employee grouping in tbl_position.
                // We expose that grouping as Department in this directory.
                string sql = @"
                    SELECT
                        e.id,
                        IFNULL(e.code,'') AS code,
                        IFNULL(e.name,'') AS Name,
                        IFNULL(e.email,'') AS Email,
                        IFNULL(e.phone,'') AS Phone,
                        IFNULL(p.name,'Unassigned') AS Department,
                        IFNULL(e.active,0) AS active
                    FROM tbl_employee e
                    LEFT JOIN tbl_position p ON p.id = e.Position_id
                    ORDER BY e.name;";

                _modernEmployees = ClinicDatabase.ExecuteDataTable(sql);
                BuildDepartmentFilter();
                RenderEmployeeCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load employees.\n\n" + ex.Message,
                    "Human Resource", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildDepartmentFilter()
        {
            if (_departmentList == null || _modernEmployees == null)
                return;

            _departmentList.Controls.Clear();

            AddDepartmentButton("All",
                _modernEmployees.AsEnumerable().Count());

            var groups = _modernEmployees.AsEnumerable()
                .GroupBy(r => Convert.ToString(r["Department"]).Trim())
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .OrderBy(g => g.Key);

            foreach (var g in groups)
                AddDepartmentButton(g.Key, g.Count());
        }

        private void AddDepartmentButton(string department, int count)
        {
            Button b = new Button
            {
                Width = 155,
                Height = 36,
                Margin = new Padding(0, 0, 0, 1),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = department == "All" ? "  All" : "    " + department + "     " + count,
                Font = new Font("Segoe UI", 10F,
                    department == _departmentFilter ? FontStyle.Bold : FontStyle.Regular),
                BackColor = department == _departmentFilter
                    ? Color.FromArgb(218, 235, 247)
                    : Color.White,
                ForeColor = Color.FromArgb(43, 47, 58),
                Cursor = Cursors.Hand,
                Tag = department
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += (s, e) =>
            {
                _departmentFilter = Convert.ToString(((Button)s).Tag);
                BuildDepartmentFilter();
                RenderEmployeeCards();
            };
            _departmentList.Controls.Add(b);
        }

        private void RenderEmployeeCards()
        {
            if (_employeeCards == null || _modernEmployees == null)
                return;

            _employeeCards.SuspendLayout();
            _employeeCards.Controls.Clear();

            string search = (_modernSearch?.Text ?? "").Trim().ToLowerInvariant();

            IEnumerable<DataRow> rows = _modernEmployees.AsEnumerable();

            if (_departmentFilter != "All")
                rows = rows.Where(r =>
                    string.Equals(Convert.ToString(r["Department"]),
                        _departmentFilter, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(search))
            {
                rows = rows.Where(r =>
                    Convert.ToString(r["Name"]).ToLowerInvariant().Contains(search) ||
                    Convert.ToString(r["Email"]).ToLowerInvariant().Contains(search) ||
                    Convert.ToString(r["Phone"]).ToLowerInvariant().Contains(search) ||
                    Convert.ToString(r["code"]).ToLowerInvariant().Contains(search) ||
                    Convert.ToString(r["Department"]).ToLowerInvariant().Contains(search));
            }

            List<DataRow> result = rows.ToList();

            // Compact reference-style cards.
            // Do NOT stretch cards across the whole available screen.
            int viewportWidth = Math.Max(300, _employeeCards.ClientSize.Width);
            int availableWidth = viewportWidth
                                 - _employeeCards.Padding.Left
                                 - _employeeCards.Padding.Right
                                 - SystemInformation.VerticalScrollBarWidth;

            const int preferredCardWidth = 500;
            const int cardGap = 12;

            int cardWidth;
            if (availableWidth >= (preferredCardWidth * 2) + cardGap)
                cardWidth = preferredCardWidth;
            else if (availableWidth >= 700)
                cardWidth = Math.Max(330, (availableWidth - cardGap) / 2);
            else
                cardWidth = Math.Max(280, availableWidth - 4);

            foreach (DataRow row in result)
                _employeeCards.Controls.Add(CreateEmployeeCard(row, cardWidth));

            _resultCount.Text = result.Count + " / " + _modernEmployees.Rows.Count;
            _employeeCards.ResumeLayout();
        }

        private Control CreateEmployeeCard(DataRow row, int width)
        {
            int id = Convert.ToInt32(row["id"]);
            string name = Convert.ToString(row["Name"]);
            string department = Convert.ToString(row["Department"]);
            string email = Convert.ToString(row["Email"]);
            string phone = Convert.ToString(row["Phone"]);
            string initial = string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();

            Panel card = new Panel
            {
                Width = width,
                Height = 112,
                Margin = new Padding(0, 0, 12, 10),
                BackColor = Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                Tag = id
            };

            int seed = Math.Abs(name.GetHashCode());
            Color[] avatarColors =
            {
                Color.FromArgb(75, 45, 200),
                Color.FromArgb(39, 187, 93),
                Color.FromArgb(196, 36, 113),
                Color.FromArgb(71, 163, 88),
                Color.FromArgb(151, 43, 195),
                Color.FromArgb(45, 185, 151),
                Color.FromArgb(104, 193, 30)
            };

            int avatarWidth = width < 430 ? 88 : 108;

            Panel avatar = new Panel
            {
                Dock = DockStyle.Left,
                Width = avatarWidth,
                BackColor = avatarColors[seed % avatarColors.Length]
            };
            Label initialLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = initial,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 39F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };
            avatar.Controls.Add(initialLabel);
            card.Controls.Add(avatar);

            int textLeft = avatarWidth + 14;
            int textWidth = Math.Max(120, width - textLeft - 55);

            Label nameLabel = new Label
            {
                AutoEllipsis = true,
                Text = name,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 28, 35),
                Location = new Point(textLeft, 10),
                Size = new Size(textWidth, 27)
            };
            card.Controls.Add(nameLabel);

            Label deptLabel = new Label
            {
                AutoEllipsis = true,
                Text = department,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(70, 72, 80),
                Location = new Point(textLeft, 35),
                Size = new Size(textWidth, 22)
            };
            card.Controls.Add(deptLabel);

            Label emailLabel = new Label
            {
                AutoEllipsis = true,
                Text = string.IsNullOrWhiteSpace(email) ? "" : "✉  " + email,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 74, 110),
                Location = new Point(textLeft, 59),
                Size = new Size(textWidth, 22)
            };
            card.Controls.Add(emailLabel);

            Label phoneLabel = new Label
            {
                AutoEllipsis = true,
                Text = string.IsNullOrWhiteSpace(phone) ? "" : "☎  " + phone,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 74, 110),
                Location = new Point(textLeft, 82),
                Size = new Size(textWidth, 22)
            };
            card.Controls.Add(phoneLabel);

            Label clock = new Label
            {
                Text = "◷",
                Font = new Font("Segoe UI Symbol", 16F),
                ForeColor = Color.FromArgb(90, 105, 115),
                AutoSize = false,
                Size = new Size(30, 30),
                Location = new Point(Math.Max(textLeft, width - 42), 80),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(clock);

            EventHandler openEmployee = (s, e) =>
                frmMain.openChildForm(new frmViewEmployee(id));

            card.DoubleClick += openEmployee;
            nameLabel.DoubleClick += openEmployee;
            avatar.DoubleClick += openEmployee;
            initialLabel.DoubleClick += openEmployee;

            return card;
        }

        private void deleteEmployeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            int _id = (int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString()));

            if (dgvTransactions.Rows.Count <= 1)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DeleteData(_id);
                }
                else
                {
                    MessageBox.Show("Deletion canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Employee has transactions. Cannot delete.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.openChildForm(new frmOpeningBalanceEmployee());
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
                return;
            frmMain.openChildForm(new frmViewEmployee(int.Parse(dgvCustomer.SelectedRows[0].Cells["id"].Value.ToString())));
        }
    }
}
