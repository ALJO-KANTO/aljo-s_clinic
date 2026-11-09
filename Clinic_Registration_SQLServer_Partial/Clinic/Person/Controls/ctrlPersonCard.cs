using Clinic.Person;
using Clinic.Properties;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Clinic;

namespace Clinic.Controls
{
    public partial class ctrlPersonCard : UserControl
    {
        private int _PersonID = -1;
        private clsPerson _Person;
        private readonly Dictionary<Control, Point> _originalPositions = new Dictionary<Control, Point>();
        private bool _originalPositionsSaved;
        private Label _emptyPhotoCaption;
        private Image _placeholderImage;
        private Image _loadedPhoto;
        private static readonly Color Blue = Color.FromArgb(43, 139, 235);
        private static readonly Color Ink = Color.FromArgb(31, 55, 87);
        private static readonly Color Muted = Color.FromArgb(115, 135, 159);

        public ctrlPersonCard()
        {
            InitializeComponent();
            ApplyCardDesign();
            ResetPersonInfo();
        }

        public int PersonID { get { return _PersonID; } }
        public clsPerson SelectedPersonInfo { get { return _Person; } }

        public void LoadPersonInfo(int personID)
        {
            _PersonID = personID;
            _Person = clsPerson.Find(personID);
            if (_Person == null)
            {
                ResetPersonInfo();
                MessageBox.Show(TranslationClass.Translate("No Person with National No. = ", TranslationClass.IsArabic) + personID,
                    TranslationClass.Translate("Error", TranslationClass.IsArabic), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _FillPersonInfo();
            ApplyLanguage();
        }

        public void ResetPersonInfo()
        {
            _PersonID = -1;
            _Person = null;
            foreach (Label value in new[] { lblPersonID, lblFullName, lblGendor, lblEmail,
                lblPhone, lblDateOfBirth, lblCountry, lblAddress }) value.Text = "—";
            pbGendor.Image = Resources.Man_32;
            llEditPersonInfo.Enabled = false;
            ShowPlaceholder();
            ApplyLanguage();
        }

        public void _FillPersonInfo()
        {
            if (_Person == null) return;
            llEditPersonInfo.Enabled = true;
            lblPersonID.Text = _Person.PersonID.ToString();
            lblFullName.Text = ValueOrDash(_Person.Name);
            lblGendor.Text = TranslationClass.Translate(_Person.Gender == 0 ? "Male" : "Female", TranslationClass.IsArabic);
            pbGendor.Image = _Person.Gender == 0 ? Resources.Man_32 : Resources.Man_32;
            lblEmail.Text = ValueOrDash(_Person.Email);
            lblPhone.Text = ValueOrDash(_Person.PhoneNumber);
            lblDateOfBirth.Text = clsFormat.DateToShort(_Person.DateOfBirth);
            lblCountry.Text = _Person.CountryInfo == null ? "—" : ValueOrDash(_Person.CountryInfo.CountryName);
            lblAddress.Text = ValueOrDash(_Person.Address);
            _LoadPersonImage();
        }

        private static string ValueOrDash(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "—" : value;
        }

        private void _LoadPersonImage()
        {
            // Clear ImageLocation so WinForms cannot asynchronously overwrite the new image.
            pbPersonImage.ImageLocation = null;
            if (_loadedPhoto != null)
            {
                if (ReferenceEquals(pbPersonImage.Image, _loadedPhoto)) pbPersonImage.Image = null;
                _loadedPhoto.Dispose();
                _loadedPhoto = null;
            }
            string path = _Person == null ? null : _Person.ImagePath;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                try
                {
                    // Read a copy immediately; don't leave the source image file locked.
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (Image source = Image.FromStream(stream))
                        _loadedPhoto = new Bitmap(source);
                    pbPersonImage.Image = _loadedPhoto;
                    _emptyPhotoCaption.Visible = false;
                    return;
                }
                catch (Exception) { /* Fall back to the avatar when the image cannot be read. */ }
            }
            ShowPlaceholder();
        }

        private void ShowPlaceholder()
        {
            pbPersonImage.ImageLocation = null;
            if (_loadedPhoto != null)
            {
                if (ReferenceEquals(pbPersonImage.Image, _loadedPhoto)) pbPersonImage.Image = null;
                _loadedPhoto.Dispose();
                _loadedPhoto = null;
            }
            pbPersonImage.Image = _placeholderImage;
            _emptyPhotoCaption.Visible = true;
            _emptyPhotoCaption.Text = TranslationClass.IsArabic ? "لم يتم تحديد شخص" : "No person selected";
        }

        private void ApplyCardDesign()
        {
            BackColor = Color.White;
            groupBox1.BackColor = Color.White;
            groupBox1.ForeColor = Ink;
            groupBox1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            // Position values consistently, immediately after their icons, not after varying label widths.
            SetField(label22, null, lblPersonID, 16, 28, 135, 166);
            SetField(label1, pictureBox8, lblFullName, 16, 65, 135, 166);
            SetField(label5, pbGendor, lblGendor, 16, 101, 135, 166);
            SetField(label6, pictureBox5, lblEmail, 16, 137, 135, 166);
            SetField(label12, pictureBox7, lblAddress, 16, 173, 135, 166);
            SetField(label3, pictureBox4, lblDateOfBirth, 337, 65, 465, 498);
            SetField(label4, pictureBox2, lblPhone, 337, 101, 465, 498);
            SetField(label15, pictureBox1, lblCountry, 337, 137, 465, 498);
            lblAddress.AutoSize = false;
            lblAddress.Size = new Size(165, 56);
            lblAddress.AutoEllipsis = true;
            lblPersonID.ForeColor = Color.FromArgb(18, 166, 114);
            lblFullName.ForeColor = Ink;
            llEditPersonInfo.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            llEditPersonInfo.LinkColor = Blue;
            llEditPersonInfo.Location = new Point(704, 25);
            pbPersonImage.Location = new Point(705, 58);
            pbPersonImage.Size = new Size(145, 126);
            pbPersonImage.BorderStyle = BorderStyle.None;
            pbPersonImage.BackColor = Color.FromArgb(240, 247, 255);
            pbPersonImage.SizeMode = PictureBoxSizeMode.Zoom;
            _placeholderImage = CreateAvatar(160, 160);
            _emptyPhotoCaption = new Label
            {
                Text = "No person selected",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Muted,
                BackColor = Color.FromArgb(240, 247, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(705, 186),
                Size = new Size(145, 28)
            };
            groupBox1.Controls.Add(_emptyPhotoCaption);
            _emptyPhotoCaption.BringToFront();
            // Capture the updated English baseline once. Arabic will mirror from this baseline.
            SaveOriginalPositions(this);
            _originalPositionsSaved = true;
        }

        private void SetField(Label title, PictureBox icon, Label value, int titleX, int y, int iconX, int valueX)
        {
            title.AutoSize = false;
            title.Bounds = new Rectangle(titleX, y, titleX > 300 ? 126 : 115, 26);
            title.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            title.ForeColor = Ink;
            if (icon != null)
            {
                icon.Bounds = new Rectangle(iconX, y + 2, 21, 21);
                icon.SizeMode = PictureBoxSizeMode.Zoom;
            }
            value.AutoSize = false;
            value.Bounds = new Rectangle(valueX, y, titleX > 300 ? 160 : 155, 26);
            value.Font = new Font("Segoe UI", 9F);
            value.ForeColor = Ink;
            value.AutoEllipsis = true;
        }

        private static Bitmap CreateAvatar(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.FromArgb(240, 247, 255));
                using (Brush circle = new SolidBrush(Color.FromArgb(218, 235, 253)))
                    g.FillEllipse(circle, 17, 6, 126, 126);
                using (Brush shirt = new SolidBrush(Color.FromArgb(77, 153, 222)))
                    g.FillEllipse(shirt, 28, 109, 104, 78);
                using (Brush neck = new SolidBrush(Color.FromArgb(232, 173, 130)))
                    g.FillRectangle(neck, 70, 89, 22, 28);
                using (Brush face = new SolidBrush(Color.FromArgb(244, 191, 148)))
                    g.FillEllipse(face, 52, 38, 58, 70);
                using (Brush hair = new SolidBrush(Color.FromArgb(44, 71, 102)))
                {
                    g.FillPie(hair, 49, 27, 65, 52, 180, 180);
                    g.FillEllipse(hair, 49, 37, 17, 33);
                }
            }
            return bmp;
        }

        private void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_PersonID <= 0) return;
            using (frmAddUpdatePerson frm = new frmAddUpdatePerson(_PersonID)) frm.ShowDialog();
            LoadPersonInfo(_PersonID);
        }

        private void lblAddress_Click(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }

        public void ApplyLanguage()
        {
            bool arabic = TranslationClass.IsArabic;
            if (!_originalPositionsSaved)
            {
                SaveOriginalPositions(this);
                _originalPositionsSaved = true;
            }
            groupBox1.Text = TranslationClass.Translate("Person Information", arabic);
            label22.Text = TranslationClass.Translate("Person ID:", arabic);
            label1.Text = TranslationClass.Translate("Name:", arabic);
            label5.Text = TranslationClass.Translate("Gender:", arabic);
            label3.Text = TranslationClass.Translate("Date Of Birth:", arabic);
            label4.Text = TranslationClass.Translate("Phone:", arabic);
            label6.Text = TranslationClass.Translate("Email:", arabic);
            label15.Text = TranslationClass.Translate("Country:", arabic);
            label12.Text = TranslationClass.Translate("Address:", arabic);
            llEditPersonInfo.Text = TranslationClass.Translate("Edit Person Info", arabic);
            if (_Person != null)
                lblGendor.Text = TranslationClass.Translate(_Person.Gender == 0 ? "Male" : "Female", arabic);
            if (_emptyPhotoCaption != null)
                _emptyPhotoCaption.Text = arabic ? "لم يتم تحديد شخص" : "No person selected";
            ApplyLanguageDirection(this, arabic);
            MirrorControls(this, arabic);
            // Keep the empty-state caption centered, including after RTL changes.
            if (_emptyPhotoCaption != null) _emptyPhotoCaption.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void SaveOriginalPositions(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (!_originalPositions.ContainsKey(control)) _originalPositions[control] = control.Location;
                if (control.HasChildren) SaveOriginalPositions(control);
            }
        }

        private void ApplyLanguageDirection(Control parent, bool arabic)
        {
            parent.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
            foreach (Control control in parent.Controls)
            {
                control.RightToLeft = arabic ? RightToLeft.Yes : RightToLeft.No;
                Label label = control as Label;
                if (label != null) label.TextAlign = arabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                if (control.HasChildren) ApplyLanguageDirection(control, arabic);
            }
        }

        private void MirrorControls(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                Point original;
                if (!_originalPositions.TryGetValue(control, out original)) continue;
                control.Left = arabic ? parent.ClientSize.Width - original.X - control.Width : original.X;
                if (control.HasChildren) MirrorControls(control, arabic);
            }
        }
    }
}
