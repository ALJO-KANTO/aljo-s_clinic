
using Clinic.Person;
using Clinic.Properties;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Clinic;

namespace Clinic.Controls
{
    public partial class ctrlPersonCard : UserControl
    {
        private int _PersonID = -1;
        private clsPerson _Person;

        // Store original positions so repeated language switches
        // do not move controls progressively.
        private readonly Dictionary<Control, Point> _originalPositions =
            new Dictionary<Control, Point>();

        private bool _originalPositionsSaved = false;

        public ctrlPersonCard()
        {
            InitializeComponent();
        }

        public int PersonID
        {
            get { return _PersonID; }
        }

        public clsPerson SelectedPersonInfo
        {
            get { return _Person; }
        }

        public void LoadPersonInfo(int PersonID)
        {
            _PersonID = PersonID;
            _Person = clsPerson.Find(_PersonID);

            if (_Person == null)
            {
                ResetPersonInfo();

                MessageBox.Show(
                    TranslationClass.Translate(
                        "No Person with National No. = ",
                        TranslationClass.IsArabic) + PersonID,
                    TranslationClass.Translate(
                        "Error",
                        TranslationClass.IsArabic),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            _FillPersonInfo();

            // Reapply language after loading the person's information.
            ApplyLanguage();
        }

        public void ResetPersonInfo()
        {
            _PersonID = -1;

            lblPersonID.Text = "[????]";
            lblFullName.Text = "[????]";

            pbGendor.Image = Resources.Man_32;

            lblGendor.Text = "[????]";
            lblEmail.Text = "[????]";
            lblPhone.Text = "[????]";
            lblDateOfBirth.Text = "[????]";
            lblCountry.Text = "[????]";
            lblAddress.Text = "[????]";

            pbPersonImage.Image = Resources.Male_512;

            llEditPersonInfo.Enabled = false;

            ApplyLanguage();
        }

        public void _FillPersonInfo()
        {
            if (_Person == null)
                return;

            llEditPersonInfo.Enabled = true;

            lblPersonID.Text = _Person.PersonID.ToString();
            lblFullName.Text = _Person.Name.ToString();

            lblGendor.Text = _Person.Gender == 0
                ? TranslationClass.Translate("Male", TranslationClass.IsArabic)
                : TranslationClass.Translate("Female", TranslationClass.IsArabic);

            lblEmail.Text = _Person.Email;
            lblPhone.Text = _Person.PhoneNumber;

            lblDateOfBirth.Text =
                clsFormat.DateToShort(_Person.DateOfBirth);

            lblCountry.Text = _Person.CountryInfo.CountryName;
            lblAddress.Text = _Person.Address;

            _LoadPersonImage();
        }

        private void _LoadPersonImage()
        {
            if (_Person == null)
                return;

            if (_Person.Gender == 0)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            string ImagePath = _Person.ImagePath;

            if (!string.IsNullOrEmpty(ImagePath))
            {
                if (File.Exists(ImagePath))
                {
                    pbPersonImage.ImageLocation = ImagePath;
                }
                else
                {
                    MessageBox.Show(
                        TranslationClass.Translate(
                            "Could not find this image: = ",
                            TranslationClass.IsArabic) + ImagePath,
                        TranslationClass.Translate(
                            "Error",
                            TranslationClass.IsArabic),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void llEditPersonInfo_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdatePerson frm =
                new frmAddUpdatePerson(_PersonID);

            frm.ShowDialog();

            LoadPersonInfo(_PersonID);
        }

        private void lblAddress_Click(object sender, EventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        public void ApplyLanguage()
        {
            bool isArabic = TranslationClass.IsArabic;

            // Save positions after the control has been initialized.
            if (!_originalPositionsSaved)
            {
                SaveOriginalPositions(this);
                _originalPositionsSaved = true;
            }

            groupBox1.Text =
                TranslationClass.Translate("Person Information", isArabic);

            label22.Text =
                TranslationClass.Translate("Person ID:", isArabic);

            label1.Text =
                TranslationClass.Translate("Name:", isArabic);

            label5.Text =
                TranslationClass.Translate("Gender:", isArabic);

            label3.Text =
                TranslationClass.Translate("Date Of Birth:", isArabic);

            label4.Text =
                TranslationClass.Translate("Phone:", isArabic);

            label6.Text =
                TranslationClass.Translate("Email:", isArabic);

            label15.Text =
                TranslationClass.Translate("Country:", isArabic);

            label12.Text =
                TranslationClass.Translate("Address:", isArabic);

            llEditPersonInfo.Text =
                TranslationClass.Translate("Edit Person Info", isArabic);

            // Apply text direction and alignment.
            ApplyLanguageDirection(this, isArabic);

            // Mirror or restore the original positions.
            MirrorControls(this, isArabic);
        }

        private void SaveOriginalPositions(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (!_originalPositions.ContainsKey(control))
                {
                    _originalPositions[control] = control.Location;
                }

                if (control.HasChildren)
                {
                    SaveOriginalPositions(control);
                }
            }
        }

        private void ApplyLanguageDirection(
            Control parent,
            bool isArabic)
        {
            parent.RightToLeft = isArabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            foreach (Control control in parent.Controls)
            {
                control.RightToLeft = isArabic
                    ? RightToLeft.Yes
                    : RightToLeft.No;

                if (control is Label label)
                {
                    label.TextAlign = isArabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }
                else if (control is LinkLabel linkLabel)
                {
                    linkLabel.TextAlign = isArabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }

                if (control.HasChildren)
                {
                    ApplyLanguageDirection(control, isArabic);
                }
            }
        }

        private void MirrorControls(Control parent, bool isArabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (!_originalPositions.TryGetValue(
                    control, out Point originalPosition))
                {
                    continue;
                }

                control.Left = isArabic
                    ? parent.ClientSize.Width
                        - originalPosition.X
                        - control.Width
                    : originalPosition.X;

                if (control.HasChildren)
                {
                    MirrorControls(control, isArabic);
                }
            }
        }
    }
}
