using Clinic_DataAccess;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Mail;
using System.Windows.Forms;

namespace Clinic.Login
{
    public partial class frmRegister : Form
    {
        private readonly bool _isRoleVisible;
        public frmRegister(bool isRoleVisible = false)
        {
            InitializeComponent();
            DoubleBuffered = true;
            UpdateRegistrationHint(this);
            gender.Items.AddRange(new object[] { "Male", "Female" });
            gender.SelectedIndex = 0;
            dob.MaxDate = DateTime.Today;
            dob.Value = DateTime.Today.AddYears(-18);
            Load += FrmRegister_Load;
            btnRegister.Click += Register_Click;
            btnClose.Click += (s, e) => Close();
            btnShowPassword.Click += (s, e) => TogglePassword(password, btnShowPassword);
            btnShowConfirm.Click += (s, e) => TogglePassword(confirm, btnShowConfirm);
            panelBrand.Paint += PanelBrand_Paint;
            btnRegister.Paint += BtnRegister_Paint;
            _isRoleVisible = isRoleVisible;
        }

        private static void UpdateRegistrationHint(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.Text != null && c.Text.Contains("Registration requires administrator approval"))
                    c.Text = "Your account will be ready to use immediately after registration.";
                UpdateRegistrationHint(c);
            }
        }

        private void FrmRegister_Load(object sender, EventArgs e)
        {
            try
            {
                ClinicDatabase.InitializeRegistrationTables();

                DataTable dt = ClinicDatabase.GetRegistrationCountries();

                country.DataSource = null;
                country.Items.Clear();

                country.DisplayMember = "CountryName";
                country.ValueMember = "CountryID";
                country.DataSource = dt;

                country.SelectedIndex = -1;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "No countries found in the database.",
                        "Nationality",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                cmbRole.Visible = lblRole.Visible = _isRoleVisible;

                DataTable dt0 = ClinicDatabase.GetRoles();

                cmbRole.DataSource = null;
                cmbRole.Items.Clear();

                cmbRole.DisplayMember = "RoleName";
                cmbRole.ValueMember = "RoleID";
                cmbRole.DataSource = dt0;

                cmbRole.SelectedIndex = -1;
                if (cmbRole.Visible)
                {
                    cmbRole.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load countries:\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void TogglePassword(TextBox field, Button button)
        {
            field.UseSystemPasswordChar = !field.UseSystemPasswordChar;
            //button.Text = field.UseSystemPasswordChar ? "Show" : "Hide";
        }

        private void Register_Click(object sender, EventArgs e)
        {
            string name = fullName.Text.Trim();
            string mobile = phone.Text.Trim();
            string mail = email.Text.Trim();
            string addr = address.Text.Trim();
            string user = username.Text.Trim();
            int roleId = cmbRole.Visible && cmbRole.SelectedValue != null && cmbRole.SelectedValue is int ? (int)cmbRole.SelectedValue : 5;
            if (name.Length == 0)
            {
                MessageBox.Show("Please enter Full Name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fullName.Focus();
                return;
            }

            if (mobile.Length == 0)
            {
                MessageBox.Show("Please enter Phone Number.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                phone.Focus();
                return;
            }

            if (addr.Length == 0)
            {
                MessageBox.Show("Please enter Address.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                address.Focus();
                return;
            }

            if (country.SelectedValue == null ||
                country.SelectedValue is DataRowView)
            {
                MessageBox.Show("Please select Nationality.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                country.Focus();
                return;
            }

            if (user.Length == 0)
            {
                MessageBox.Show("Please enter Username.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                username.Focus();
                return;
            }

            if (password.Text.Length < 4)
            {
                MessageBox.Show("Password must contain at least 4 characters.",
                    "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                password.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(mail))
            {
                MessageBox.Show("Please enter your email address to receive account details.", "Validation");
                email.Focus();
                return;
            }
            try { var parsed = new MailAddress(mail); if (!string.Equals(parsed.Address, mail, StringComparison.OrdinalIgnoreCase)) throw new FormatException(); }
            catch { MessageBox.Show("Please enter a valid email address.", "Validation"); email.Focus(); return; }
            if (password.Text != confirm.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation");
                confirm.Focus();
                return;
            }
            btnRegister.Enabled = false;
            try
            {
                // Preserve the hashing approach used by the existing login code.
                string passwordHash = clsGlobal.ComputeHash(password.Text);
                ClinicDatabase.RegisterClinicAccount(
                  name,
                  dob.Value.Date,
                  (byte)gender.SelectedIndex,
                  mobile,
                  mail,
                  addr,
                  Convert.ToInt32(country.SelectedValue),
                  user,
                  passwordHash,roleId);

                string emailMessage = "Account created. You can log in now with your username and password.";
                try
                {
                    clsRegistrationData.SendWelcomeEmail(mail, user);
                    emailMessage += "\nA confirmation email was sent to " + mail + ".";
                }
                catch (Exception mailError)
                {
                    emailMessage += "\nThe account is active, but the confirmation email could not be sent: "
                        + mailError.Message;
                }
                MessageBox.Show(emailMessage, "Registration Successful",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed: " + ex.Message,
                    "Registration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { btnRegister.Enabled = true; }
        }

        private void PanelBrand_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = panelBrand.ClientRectangle;
            if (rect.Width < 1 || rect.Height < 1) return;
            using (var brush = new LinearGradientBrush(rect,
                Color.FromArgb(0, 86, 115), Color.FromArgb(0, 30, 66), 80f))
                e.Graphics.FillRectangle(brush, rect);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Color.FromArgb(65, 26, 224, 237), 3))
            {
                e.Graphics.DrawArc(pen, -180, 45, 720, 840, 250, 160);
                e.Graphics.DrawArc(pen, -210, 70, 780, 900, 245, 160);
            }
        }

        private void BtnRegister_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = btnRegister.ClientRectangle;
            if (rect.Width < 1 || rect.Height < 1) return;
            using (var brush = new LinearGradientBrush(rect,
                Color.FromArgb(0, 197, 177), Color.FromArgb(0, 75, 198), 0f))
                e.Graphics.FillRectangle(brush, rect);
            TextRenderer.DrawText(e.Graphics, btnRegister.Text, btnRegister.Font, rect,
                Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
