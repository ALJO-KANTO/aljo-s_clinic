using Clinic_Business;
using Clinic_DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Clinic.Login
{
    public partial class frmLogin : Form
    {
        public static Dictionary<string, int> defaultAccounts;

        public frmLogin()
        {
            InitializeComponent();

            this.TopMost = false;

            ApplyVisualEffects();
        }

        private void ApplyVisualEffects()
        {
            leftPanel.Paint += (s, e) =>
            {
                Rectangle r = leftPanel.ClientRectangle;
                if (r.Width == 0 || r.Height == 0) return;
                using (var brush = new LinearGradientBrush(r, Color.FromArgb(0, 80, 103), Color.FromArgb(0, 29, 54), 90f))
                    e.Graphics.FillRectangle(brush, r);
            };
            rightPanel.Paint += (s, e) =>
            {
                Rectangle r = rightPanel.ClientRectangle;
                if (r.Width == 0 || r.Height == 0) return;
                using (var brush = new LinearGradientBrush(r, Color.FromArgb(195, 239, 251), Color.FromArgb(248, 252, 255), 25f))
                    e.Graphics.FillRectangle(brush, r);
                using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 80, Math.Max(1, r.Height)),
                    Color.FromArgb(0, 188, 194), Color.FromArgb(156, 230, 240), 0f))
                    e.Graphics.FillRectangle(brush, 0, 0, 80, r.Height);
            };
            loginCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var p = RoundedPath(new Rectangle(1, 1, loginCard.Width - 3, loginCard.Height - 3), 28))
                using (var pen = new Pen(Color.FromArgb(218, 239, 248))) e.Graphics.DrawPath(pen, p);
            };
            foreach (Panel p in new[] { userField, passwordField })
                p.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var path = RoundedPath(new Rectangle(1, 1, p.Width - 3, p.Height - 3), 12))
                    using (var pen = new Pen(Color.FromArgb(174, 217, 235), 1.5f)) e.Graphics.DrawPath(pen, path);
                };
            btnLogin.Paint += (s, e) =>
            {
                Rectangle r = btnLogin.ClientRectangle;
                if (r.Width == 0 || r.Height == 0) return;
                using (var brush = new LinearGradientBrush(r, Color.FromArgb(0, 195, 168), Color.FromArgb(0, 69, 189), 0f))
                    e.Graphics.FillRectangle(brush, r);
                TextRenderer.DrawText(e.Graphics, btnLogin.Text, btnLogin.Font, r, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            };
            btnLogin.MouseEnter += (s, e) => btnLogin.Invalidate();
            btnLogin.MouseLeave += (s, e) => btnLogin.Invalidate();
            lblForgot.Cursor = Cursors.Hand;
            // No recovery workflow exists in the supplied code; do not invent one.
        }

        private static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        //private void LoadClinicArtwork()
        //{
        //    // Prefer the image already embedded in Properties.Resources; fall back to Resources files.
        //    Image brain = FindImage("High Tech", "Brain", "Technology");
        //    Image logo = FindImage("Medical Medicine Logo", "Clinic Logo", "1780023857231");
        //    Image hermes = FindImage("Staff Of Hermes", "Caduceus", "Asclepius");
        //    if (brain != null) picBrain.Image = MakeTransparentWhite(brain, 0.85f);
        //    if (logo != null) picClinicLogo.Image = MakeTransparentWhite(logo, 1f);
        //    if (hermes != null) picHermes.Image = MakeTransparentWhite(hermes, 1f);
        //    try { picUser.Image = global::Clinic.Properties.Resources.Person_32; } catch { }
        //    try { picLock.Image = global::Clinic.Properties.Resources.Password_32; } catch { }
        //    if (picUser.Image == null) picUser.BackColor = Color.FromArgb(218, 244, 251);
        //    if (picLock.Image == null) picLock.BackColor = Color.FromArgb(218, 244, 251);
        //}

        private static Image FindImage(params string[] terms)
        {
            // Resource keys often differ from the original downloaded filenames.
            try
            {
                var asm = typeof(frmLogin).Assembly;
                foreach (string resourceName in asm.GetManifestResourceNames())
                {
                    if (!resourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase)) continue;
                    try
                    {
                        var rm = new ResourceManager(resourceName.Substring(0, resourceName.Length - 10), asm);
                        ResourceSet set = rm.GetResourceSet(System.Globalization.CultureInfo.InvariantCulture, true, false);
                        if (set == null) continue;
                        foreach (DictionaryEntry entry in set)
                        {
                            string key = Convert.ToString(entry.Key);
                            foreach (string term in terms)
                                if (Normalize(key).Contains(Normalize(term)) && entry.Value is Image)
                                    return new Bitmap((Image)entry.Value);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
                {
                    string resources = Path.Combine(dir, "Resources");
                    if (Directory.Exists(resources))
                        foreach (string path in Directory.GetFiles(resources))
                        {
                            string name = Path.GetFileNameWithoutExtension(path);
                            foreach (string term in terms)
                                if (Normalize(name).Contains(Normalize(term)))
                                {
                                    using (Image source = Image.FromFile(path)) return new Bitmap(source);
                                }
                        }
                    DirectoryInfo parent = Directory.GetParent(dir);
                    dir = parent == null ? null : parent.FullName;
                }
            }
            catch (Exception) { }
            return null;
        }

        private static string Normalize(string text)
        {
            var chars = new System.Text.StringBuilder();
            foreach (char c in text)
                if (char.IsLetterOrDigit(c)) chars.Append(char.ToLowerInvariant(c));
            return chars.ToString();
        }

        private static Bitmap MakeTransparentWhite(Image source, float opacity)
        {
            Bitmap input = new Bitmap(source);
            Bitmap output = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
            // Remove only near-white canvas; leave all saturated logo colors intact.
            for (int y = 0; y < input.Height; y++)
                for (int x = 0; x < input.Width; x++)
                {
                    Color c = input.GetPixel(x, y);
                    int a = c.A;
                    int min = Math.Min(c.R, Math.Min(c.G, c.B));
                    if (min >= 250) a = 0;
                    else if (min > 235) a = a * (250 - min) / 15;
                    a = (int)(a * opacity);
                    output.SetPixel(x, y, Color.FromArgb(a, c.R, c.G, c.B));
                }
            input.Dispose();
            return output;
        }

        private void lblEye_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            lblEye.Text = txtPassword.UseSystemPasswordChar ? "◉" : "◎";
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (var register = new frmRegister()) register.ShowDialog(this);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                ShowError("Please fill all required fields.", "Validation Error");
                return;
            }

            string username = txtUserName.Text.Trim();
            string password = txtPassword.Text;

            string hashedPassword = clsGlobal.ComputeHash(password);

            try
            {
                // Authenticate against the existing application user database.
                clsUser user = clsUser.FindByUsernameAndPassword(
                    username, hashedPassword);

                if (user == null)
                {
                    ShowError(
                        "Invalid Username or Password.\n\n" +
                        "Please verify that your account is registered " +
                        "and active in the Clinic Management System.",
                        "Wrong Credentials");

                    return;
                }

                if (!user.IsActive)
                {
                    ShowError("Your account is not active.", "Inactive Account");
                    return;
                }

                if (user.PersonInfo == null || user.RoleInfo == null)
                {
                    ShowError(
                        "Your account is missing its person or role record.",
                        "Account Setup Required");
                    return;
                }

                clsGlobal.CurrentUser = user;
                clsGlobal.PersonName = user.PersonInfo.Name;

                clsGlobal.SaveUserCredentialsToRegistry(
                    chkRememberMe.Checked ? username : "",
                    chkRememberMe.Checked ? password : "");

                frmMain mainForm = new frmMain(this);

                mainForm.FormClosed += (s, args) =>
                {
                    if (!this.IsDisposed)
                    {
                        clsGlobal.CurrentUser = null;
                        this.Show();
                        this.Activate();
                    }
                };

                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Login failed:\n\n" + ex.Message,
                    "Login Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ShowError(string message, string title)
        {
            txtUserName.Focus();
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            defaultAccounts = BindCombos.LoadDefaultAccounts();

            string username = string.Empty, password = string.Empty;
            if (clsGlobal.GetStoredCredentialFromRegistry(ref username, ref password))
            {
                txtUserName.Text = username;
                txtPassword.Text = password;
                chkRememberMe.Checked = true;
            }
            else
            {
                chkRememberMe.Checked = false;
                txtPassword.Text = "";
                txtUserName.Text = "";
                txtUserName.Focus();
            }
        }

        private void btnClose_Click(object sender, EventArgs e) { Close(); }

        private void TextBoxValiditin(object sender, CancelEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box == null) return;
            e.Cancel = string.IsNullOrWhiteSpace(box.Text);
            errorProvider1.SetError(box, e.Cancel ? "This field is required." : "");
        }
    }
}
