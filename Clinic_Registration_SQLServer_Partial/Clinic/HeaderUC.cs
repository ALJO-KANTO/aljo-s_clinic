using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace YamyProject
{
    public partial class HeaderUC : UserControl
    {
        private Random random = new Random();
        private Point previousLocation;
        public HeaderUC()
        {
            InitializeComponent();

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
        public string FormText
        {
            get { return lblFormName.Text; }
            set { lblFormName.Text = value; }
        }

        public string HeaderText {
            get { return lblFormName.Text; }
            set { lblFormName.Text = value; }
        }

        private bool _arabicLayout = false;
        private readonly Dictionary<Control, int> _originalHeaderPositions =
    new Dictionary<Control, int>();
        private readonly Dictionary<Control, Rectangle> _headerOriginalBounds =
            new Dictionary<Control, Rectangle>();

        public bool ArabicLayout
        {
            get { return _arabicLayout; }
            set
            {
                _arabicLayout = value;

                if (IsHandleCreated)
                    ApplyHeaderDirection();
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

            Control[] buttons =
            {
        btnClose,
        btnMaximize,
        btnMinimize
    };

            foreach (Control button in buttons)
            {
                if (!_headerOriginalBounds.ContainsKey(button))
                    _headerOriginalBounds[button] = button.Bounds;

                if (button.Dock != DockStyle.None)
                    continue;

                Rectangle original = _headerOriginalBounds[button];

                // Determine original position relative to HeaderUC.
                // Store it once, using the original English layout.
                if (!_originalHeaderPositions.ContainsKey(button))
                    _originalHeaderPositions[button] = GetHeaderX(button);

                int originalX = _originalHeaderPositions[button];

                int targetX = _arabicLayout
                    ? ClientSize.Width - originalX - original.Width
                    : originalX;

                button.Anchor = AnchorStyles.Top |
                    (_arabicLayout ? AnchorStyles.Left : AnchorStyles.Right);

                SetHeaderX(button, targetX);
            }

            lblFormName.RightToLeft = _arabicLayout
                ? RightToLeft.Yes
                : RightToLeft.No;

            lblFormName.TextAlign = HorizontalAlignment.Center;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            BeginInvoke(new Action(ApplyHeaderDirection));
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
