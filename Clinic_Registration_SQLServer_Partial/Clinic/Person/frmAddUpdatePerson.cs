using Clinic.Properties;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Person
{
    public partial class frmAddUpdatePerson : Form
    {
        private int _PersonID = -1;
        private clsPerson _Person;
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;

        public delegate void DataBackEventHandler(object sender, int PersonID);

        // Declare an event using the delegate
        public event DataBackEventHandler DataBack;
        public frmAddUpdatePerson()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;

        }

        public frmAddUpdatePerson(int PersonID)
        {
            InitializeComponent();
            _PersonID = PersonID;
            _Mode = enMode.Update;
        }

        // Translate visible text only. Do not change control positions or form layout.
        private string Tr(string englishText)
        {
            return TranslationClass.Translate(englishText, TranslationClass.IsArabic);
        }

        private void ApplyPersonFormTranslation()
        {
            lblTitle.Text = Tr(_Mode == enMode.AddNew
                ? "Add New Person"
                : "Update Person");

            this.Text = Tr("Add / Edit Person Info.");

            label22.Text = Tr("Person ID:");
            label1.Text = Tr("Full Name:");
            label3.Text = Tr("Date Of Birth:");
            label5.Text = Tr("Gender:");
            label6.Text = Tr("Email:");
            label4.Text = Tr("Phone:");
            label15.Text = Tr("Country:");
            label12.Text = Tr("Address:");

            rbMale.Text = Tr("Male");
            rbFemale.Text = Tr("Female");
            llSetImage.Text = Tr("Set Image");
            llRemoveImage.Text = Tr("Remove");
            btnSave.Text = "   " + Tr("Save");
            btnClose.Text = "   " + Tr("Close");
        }

        private void txtFirstName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = char.IsDigit(e.KeyChar);
        }

        private void _FillCountriesInComoboBox()
        {
            List<clsCountry> countries = clsCountry.GetAllCountriesList();

            cbCountry.DataSource = countries;

            cbCountry.DisplayMember = "CountryName";
            cbCountry.ValueMember = "CountryID";


        }

        private void _Loadata()
        {
            _Person = clsPerson.Find(_PersonID);
            if (_Person == null)
            {

                MessageBox.Show(Tr("No Person with ID = ") + _PersonID, Tr("Person Not Found"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            lblPersonID.Text = _PersonID.ToString();
            txtFullName.Text = _Person.Name;

            dtpDateOfBirth.Value = _Person.DateOfBirth;
            txtEmail.Text = _Person.Email;
            txtPhone.Text = _Person.PhoneNumber;
            txtAddress.Text = _Person.Address;

            cbCountry.SelectedIndex = cbCountry.FindString(_Person.CountryInfo.CountryName);

            if (_Person.Gender == 0)
                rbMale.Checked = true;
            else
                rbFemale.Checked = true;

            //load person image incase it was set.
            if (_Person.ImagePath != "")
            {
                pbPersonImage.ImageLocation = _Person.ImagePath;

            }

            //hide/show the remove linke incase there is no image for the person.
            llRemoveImage.Visible = (_Person.ImagePath != "");

        }

        private void _ResetDefualtValues()
        {
            //this will initialize the reset the defaule values
            _FillCountriesInComoboBox();

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = Tr("Add New Person");

                _Person = new clsPerson();
            }
            else
            {
                lblTitle.Text = Tr("Update Person");
            }
            this.Text = Tr("Add / Edit Person Info.");

            //set default image for the person.
            if (rbMale.Checked)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            //hide/show the remove linke incase there is no image for the person.
            llRemoveImage.Visible = (pbPersonImage.ImageLocation != null);


            lblPersonID.Text = Tr("N/A");
            txtFullName.Text = string.Empty;

            dtpDateOfBirth.Value = DateTime.Now;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtAddress.Text = string.Empty;

            cbCountry.SelectedIndex = cbCountry.FindString("Yemen");


        }
        private bool _layoutMirrored = false;

        private void MirrorFormLayout()
        {
            if (_layoutMirrored || !TranslationClass.IsArabic)
                return;

            _layoutMirrored = true;
            MirrorControls(this);
        }

        private void MirrorControls(Control parent)
        {
            parent.SuspendLayout();

            foreach (Control control in parent.Controls)
            {
                // Mirror the control's horizontal position within its parent.
                if (control.Dock == DockStyle.None)
                    control.Left = parent.ClientSize.Width - control.Left - control.Width;

                // Swap left/right docking so docked controls mirror as well.
                if (control.Dock == DockStyle.Left)
                    control.Dock = DockStyle.Right;
                else if (control.Dock == DockStyle.Right)
                    control.Dock = DockStyle.Left;

                // Swap left/right anchoring while preserving top/bottom anchoring.
                AnchorStyles anchor = control.Anchor;
                bool left = (anchor & AnchorStyles.Left) == AnchorStyles.Left;
                bool right = (anchor & AnchorStyles.Right) == AnchorStyles.Right;
                anchor &= ~(AnchorStyles.Left | AnchorStyles.Right);
                if (left)
                    anchor |= AnchorStyles.Right;
                if (right)
                    anchor |= AnchorStyles.Left;
                control.Anchor = anchor;

                if (control.HasChildren)
                    MirrorControls(control);
            }

            parent.ResumeLayout(true);
        }

        private void frmAddUpdatePerson_Load(object sender, EventArgs e)
        {
            _ResetDefualtValues();
            if (_Mode == enMode.Update)
                _Loadata();

            ApplyPersonFormTranslation();
            MirrorFormLayout();
        }

        private void ValidateTextBox(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    e.Cancel = true;
                    errorProvider1.SetError(textBox, Tr("This field is required."));
                }
                else
                {
                    errorProvider1.SetError(textBox, string.Empty);
                }
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            //no need to validate the email incase it's empty.
            if (txtEmail.Text.Trim() == "")
                return;

            //validate email format
            if (!clsValidatoin.ValidateEmail(txtEmail.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, Tr("Invalid Email Address Format!"));
            }
            else
            {
                errorProvider1.SetError(txtEmail, null);
            }
            ;

        }

        private void CheckedImageGender(object sender, EventArgs e)
        {
            if (pbPersonImage.ImageLocation == null)
            {
                if (rbFemale.Checked)
                    pbPersonImage.Image = Resources.Female_512;
                else
                    pbPersonImage.Image = Resources.Male_512;
                llRemoveImage.Visible = false;
            }
        }

        private void llSetImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.Filter = Tr("Image Files") + "|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string SelectedFilePath = openFileDialog1.FileName;
                pbPersonImage.Load(SelectedFilePath);
                llRemoveImage.Visible = true;
            }

        }

        private void llRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pbPersonImage.ImageLocation = null;
            CheckedImageGender(null, null);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool _HandlePersonImage()
        {

            //this procedure will handle the person image,
            //it will take care of deleting the old image from the folder
            //in case the image changed. and it will rename the new image with guid and 
            // place it in the images folder.


            //_Person.ImagePath contains the old Image, we check if it changed then we copy the new image
            if (_Person.ImagePath != pbPersonImage.ImageLocation)
            {
                if (_Person.ImagePath != "")
                {
                    //first we delete the old image from the folder in case there is any.

                    try
                    {
                        File.Delete(_Person.ImagePath);
                    }
                    catch (IOException)
                    {
                        // We could not delete the file.
                        //log it later   
                    }
                }

                if (pbPersonImage.ImageLocation != null)
                {
                    //then we copy the new image to the image folder after we rename it
                    string SourceImageFile = pbPersonImage.ImageLocation.ToString();

                    if (clsUtil.CopyImageToProjectImagesFolder(ref SourceImageFile))
                    {
                        pbPersonImage.ImageLocation = SourceImageFile;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(Tr("Error Copying Image File"), Tr("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

            }
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show(Tr("Some fields are not valid. Move the mouse over the red icon(s) to see the error."), Tr("Validation Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (!this.ValidateChildren())
            {
                MessageBox.Show(Tr("Some fields are not valid. Move the mouse over the red icon(s) to see the error."),
                    Tr("Validation Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_HandlePersonImage())
                return;

            _Person.Name = txtFullName.Text;
            _Person.DateOfBirth = dtpDateOfBirth.Value;
            _Person.PhoneNumber = txtPhone.Text;
            if (rbMale.Checked)
                _Person.Gender = 0;
            else
                _Person.Gender = 1;
            _Person.Email = txtEmail.Text;
            _Person.Address = txtAddress.Text;
            _Person.NationalityCountryID = (int)cbCountry.SelectedValue;

            if (pbPersonImage.ImageLocation != null)
                _Person.ImagePath = pbPersonImage.ImageLocation;
            else
                _Person.ImagePath = "";

            if (_Person.Save())
            {
                lblPersonID.Text = _Person.PersonID.ToString();
                //change form mode to update.
                _Mode = enMode.Update;
                lblTitle.Text = Tr("Update Person");

                MessageBox.Show(Tr("Data Saved Successfully."), Tr("Saved"), MessageBoxButtons.OK, MessageBoxIcon.Information);


                // Trigger the event to send data back to the caller form.
                DataBack?.Invoke(this, _Person.PersonID);
            }
            else
                MessageBox.Show(Tr("Error: Data Is not Saved Successfully."), Tr("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
