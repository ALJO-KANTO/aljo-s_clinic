using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace YamyProject
{
    public partial class HeaderUC : UserControl
    {
        private string _originalHeaderText = "";

        private readonly Dictionary<Control, Rectangle> _originalBounds =
            new Dictionary<Control, Rectangle>();

        private int _originalHeaderWidth = 0;
        private Random random = new Random();
        private Point previousLocation;
        public HeaderUC()
        {
            InitializeComponent();

            _originalHeaderText = lblFormName.Text;

            this.Load += (s, e) =>
            {
                CaptureHeaderLayout();
                ApplyLanguage();
            };

            this.Resize += (s, e) =>
            {
                if (IsHandleCreated)
                    ApplyHeaderDirection();
            };
        }
        private void HeaderUC_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.ParentForm != null)
            {
                if (this.ParentForm.Dock == DockStyle.Fill)
                {
                    this.ParentForm.Dock = DockStyle.None; 
                        previousLocation = this.ParentForm.Location;
                }
                else
                    this.ParentForm.Dock = DockStyle.Fill;
            }
        }
        private void ApplyLanguage()
        {
            bool arabic = _arabicLayout;

            string english = _originalHeaderText;

            if (string.IsNullOrWhiteSpace(english))
                english = lblFormName.Text;

            string translated = Clinic.TranslationClass.Translate(
                english.Trim(),
                arabic);

            lblFormName.Text = translated;

            lblFormName.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            lblFormName.TextAlign = HorizontalAlignment.Center;

            ApplyHeaderDirection();
        }
        public string FormText
        {
            get { return lblFormName.Text; }
            set
            {
                _originalHeaderText = value;
                ApplyLanguage();
            }
        }

        public string HeaderText
        {
            get { return lblFormName.Text; }
            set
            {
                _originalHeaderText = value;
                ApplyLanguage();
            }
        }

        private bool _arabicLayout = false;
     
       

        public bool ArabicLayout
        {
            get { return _arabicLayout; }
            set
            {
                _arabicLayout = value;

                if (IsHandleCreated)
                    ApplyLanguage();
            }
        }
        private void CaptureHeaderLayout()
        {
            if (_originalBounds.Count > 0)
                return;

            _originalHeaderWidth = ClientSize.Width;

            Control[] buttons =
            {
        btnClose,
        btnMaximize,
        btnMinimize
    };

            foreach (Control button in buttons)
            {
                _originalBounds[button] = button.Bounds;
            }
        }
        private int GetHeaderX(Control control)
        {
            int x = control.Left;
            Control parent = control.Parent;

            while (parent != null && parent != this)
            {
                x += parent.Left;
                parent = parent.Parent;
            }

            return x;
        }

        private void SetHeaderX(Control control, int targetX)
        {
            Control parent = control.Parent;

            if (parent == null)
                return;

            int parentX = 0;
            Control current = parent;

            while (current != null && current != this)
            {
                parentX += current.Left;
                current = current.Parent;
            }

            control.Left = targetX - parentX;
        }

        private void ApplyHeaderDirection()
        {
            if (ClientSize.Width <= 0)
                return;

            CaptureHeaderLayout();

            SuspendLayout();

            try
            {
                Control[] buttons =
                {
            btnClose,
            btnMaximize,
            btnMinimize
        };

                foreach (Control button in buttons)
                {
                    Rectangle original = _originalBounds[button];

                    if (button.Dock != DockStyle.None)
                        continue;

                    int originalRightMargin =
                        _originalHeaderWidth - original.Right;

                    int targetX;

                    if (_arabicLayout)
                    {
                        // Mirror buttons to the opposite side.
                        targetX = originalRightMargin;
                        button.Anchor =
                            AnchorStyles.Top | AnchorStyles.Left;
                    }
                    else
                    {
                        // Preserve the original right-side spacing.
                        targetX = ClientSize.Width
                            - originalRightMargin
                            - original.Width;

                        button.Anchor =
                            AnchorStyles.Top | AnchorStyles.Right;
                    }

                    button.Left = Math.Max(0, targetX);
                    button.Top = original.Top;
                }

                // Keep title centered in both languages.
                lblFormName.RightToLeft = _arabicLayout
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                lblFormName.TextAlign = HorizontalAlignment.Center;
            }
            finally
            {
                ResumeLayout(true);
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            BeginInvoke(new Action(() =>
            {
                CaptureHeaderLayout();
                ApplyLanguage();
            }));
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            var parentForm = this.ParentForm;
            if (parentForm != null)
                parentForm.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            var parentForm = this.ParentForm;
            if (parentForm != null)
                parentForm.Close();
        }
        private void btnMaximize_Click(object sender, EventArgs e)
        {
            try
            {
                var parentForm = this.ParentForm;
                if (parentForm != null)
                {
                    if (parentForm.Dock == DockStyle.Fill)
                    {
                        parentForm.Dock = DockStyle.None;
                        Rectangle screenBounds = Screen.PrimaryScreen.WorkingArea;
                        int randomX = random.Next(screenBounds.Left, screenBounds.Right - parentForm.Width);
                        int randomY = random.Next(screenBounds.Top, screenBounds.Bottom - parentForm.Height);
                        parentForm.Location = new Point(randomX, randomY);
                    }
                    else
                        parentForm.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        private void lblFormName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.ParentForm != null)
            {
                if (this.ParentForm.Dock == DockStyle.Fill)
                {
                    this.ParentForm.Dock = DockStyle.None;

                    int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                    int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

                    int x = (screenWidth - this.ParentForm.Width) / 2;
                    int y = (screenHeight - this.ParentForm.Height) / 2;

                    this.ParentForm.Location = new Point(x, y);
                }
                else
                    this.ParentForm.Dock = DockStyle.Fill;
            }
        }

        private void lblFormName_Click(object sender, EventArgs e)
        {

        }
    }
}
