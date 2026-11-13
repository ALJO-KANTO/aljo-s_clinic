using Clinic_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Clinic.Patient
{
    public partial class frmAddUpdatePatient : Form
    {
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;
        private int _PatientID = -1;
        private clsPatient _Patient;

        public delegate void DataBackEventHandler(object sender, int PatientID);
        public event DataBackEventHandler DataBack;

        // UI-only elements. All existing patient/business/database methods remain unchanged.
        private Panel _progressPanel;
        private Panel _guideBanner;
        private Label _stepOne;
        private Label _stepTwo;
        private Label _stepHint;
        private Label _brand;
        private Label _sectionHint;
        private bool _patientSaved;

        private static readonly Color Navy = Color.FromArgb(9, 65, 105);
        private static readonly Color Blue = Color.FromArgb(13, 111, 207);
        private static readonly Color Teal = Color.FromArgb(0, 145, 166);
        private static readonly Color Pale = Color.FromArgb(236, 247, 253);
        private static readonly Color Ink = Color.FromArgb(24, 54, 83);

        public frmAddUpdatePatient()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
            InitializeRegistrationGuide();
            ApplyVisualPolish();
        }

        public frmAddUpdatePatient(int patientID)
        {
            InitializeComponent();
            _PatientID = patientID;
            _Mode = enMode.Update;
            InitializeRegistrationGuide();
            ApplyVisualPolish();
        }

        private string Tr(string text)
        {
            return TranslationClass.Translate(text, TranslationClass.IsArabic);
        }

        private void InitializeRegistrationGuide()
        {
            SuspendLayout();
            // Enlarging the window gives the original person card room to breathe.
            ClientSize = new Size(1080, 790);
            MinimumSize = new Size(980, 740);
            panelHeaderContainer.Height = 208;
            panelHeaderContainer.Dock = DockStyle.Top;
            tcPatient.Bounds = new Rectangle(18, 225, ClientSize.Width - 36, ClientSize.Height - 315);
            tcPatient.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            btnClose.Bounds = new Rectangle(ClientSize.Width - 338, ClientSize.Height - 70, 150, 48);
            btnSave.Bounds = new Rectangle(ClientSize.Width - 176, ClientSize.Height - 70, 154, 48);
            btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnSave.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            _brand = new Label
            {
                Text = "✚  CLINIC MANAGEMENT",
                ForeColor = Color.FromArgb(186, 233, 248),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Bounds = new Rectangle(24, 10, 300, 24),
                BackColor = Color.Transparent
            };
            panelHeaderContainer.Controls.Add(_brand);
            lblTitle.AutoSize = false;
            lblTitle.Bounds = new Rectangle(42, 37, ClientSize.Width - 110, 52);
            lblTitle.Font = new Font("Segoe UI Semibold", 23F, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            btnExit.Bounds = new Rectangle(ClientSize.Width - 64, 20, 42, 40);
            btnExit.Image = null;
            btnExit.Text = "×";
            btnExit.Font = new Font("Segoe UI", 23F);
            btnExit.ForeColor = Color.White;

            _progressPanel = new Panel
            {
                BackColor = Color.White,
                Bounds = new Rectangle(18, 105, ClientSize.Width - 36, 94),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            _progressPanel.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(215, 230, 242)))
                    e.Graphics.DrawRectangle(pen, 0, 0, _progressPanel.Width - 1, _progressPanel.Height - 1);
            };
            _stepOne = new Label
            {
                Bounds = new Rectangle(20, 11, 480, 36),
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            _stepTwo = new Label
            {
                Bounds = new Rectangle(510, 11, 480, 36),
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            _guideBanner = new Panel
            {
                BackColor = Pale,
                Bounds = new Rectangle(14, 52, ClientSize.Width - 64, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _stepHint = new Label
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 0, 12, 0),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Ink,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _guideBanner.Controls.Add(_stepHint);
            _progressPanel.Controls.Add(_stepOne);
            _progressPanel.Controls.Add(_stepTwo);
            _progressPanel.Controls.Add(_guideBanner);
            panelHeaderContainer.Controls.Add(_progressPanel);
            _progressPanel.BringToFront();
            btnExit.BringToFront();

            btnPersonInfoNext.Size = new Size(215, 42);
            btnPersonInfoNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPersonInfoNext.Location = new Point(tpPersonalInfo.ClientSize.Width - 235, tpPersonalInfo.ClientSize.Height - 58);
            btnPersonInfoNext.Image = null;
            btnPersonInfoNext.TextAlign = ContentAlignment.MiddleCenter;

            tcPatient.SelectedIndexChanged += (sender, e) => UpdateRegistrationGuide();
            Resize += (sender, e) => PositionGuideControls();
            ResumeLayout(true);
        }

        private void PositionGuideControls()
        {
            if (_progressPanel == null) return;
            bool arabic = TranslationClass.IsArabic;
            int half = (_progressPanel.Width - 52) / 2;
            _stepOne.Bounds = new Rectangle(arabic ? half + 30 : 18, 10, half, 36);
            _stepTwo.Bounds = new Rectangle(arabic ? 18 : half + 30, 10, half, 36);
            _guideBanner.Width = _progressPanel.Width - 28;
            _stepHint.TextAlign = arabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            if (tcPatient != null && tpPersonalInfo != null && btnPersonInfoNext != null)
                btnPersonInfoNext.Location = new Point(Math.Max(8, tpPersonalInfo.ClientSize.Width - btnPersonInfoNext.Width - 18), Math.Max(8, tpPersonalInfo.ClientSize.Height - btnPersonInfoNext.Height - 15));
        }

        private void UpdateRegistrationGuide()
        {
            if (_progressPanel == null) return;
            bool arabic = TranslationClass.IsArabic;
            bool second = tcPatient.SelectedTab == tpPatientInfo;
            bool saved = _Patient != null && _Patient.PatientID > 0;
            _stepOne.Text = arabic ? "●  ١  المعلومات الشخصية" : "●  1   Personal Information";
            _stepTwo.Text = arabic ? "●  ٢  معلومات المريض" : "●  2   Patient Information";
            _stepOne.BackColor = second ? Color.FromArgb(239, 245, 251) : Blue;
            _stepOne.ForeColor = second ? Ink : Color.White;
            _stepTwo.BackColor = second ? Blue : Color.FromArgb(239, 245, 251);
            _stepTwo.ForeColor = second ? Color.White : Ink;
            if (_Mode == enMode.Update && !_patientSaved)
                _stepHint.Text = arabic ? "راجع بيانات الشخص، ثم اضغط التالي لتعديل بيانات المريض." : "Review the person record, then click Next to edit patient details.";
            else if (saved)
                _stepHint.Text = arabic ? "تم حفظ المريض. رقم المريض: " + _Patient.PatientID : "Patient saved successfully. Patient ID: " + _Patient.PatientID;
            else if (second)
                _stepHint.Text = arabic ? "أدخل بيانات المريض ثم اضغط حفظ لإنشاء رقم المريض." : "Enter the patient details and click Save to generate the Patient ID.";
            else
                _stepHint.Text = arabic ? "احفظ الشخص من بطاقة الشخص أولاً، ثم اضغط التالي. يُنشأ رقم المريض عند حفظ الخطوة الثانية." : "Save the person record in the person card, then click Next. Patient ID is generated after saving Step 2.";
            btnPersonInfoNext.Text = arabic ? "التالي: معلومات المريض" : "Next: Patient Info";
            btnSave.Text = arabic ? Tr("Save") : "Save";
            PositionGuideControls();
            RefreshEmptyState();
        }

        private void ApplyArabicTranslation()
        {
            bool arabic = TranslationClass.IsArabic;
            string title = _Mode == enMode.AddNew ? "Add New Patient" : "Update Patient Info";
            Text = Tr(title);
            lblTitle.Text = Tr(title);
            lblTitle.TextAlign = arabic ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            tpPersonalInfo.Text = Tr("Personal Info");
            tpPatientInfo.Text = Tr("Patient Info");
            label6.Text = Tr("Blood Type:");
            label15.Text = Tr("Emergency Contact:");
            label7.Text = Tr("Medical History:");
            label8.Text = Tr("Patient ID:");
            btnClose.Text = arabic ? Tr("Close") : "Close";
            _brand.Text = arabic ? "✚  نظام إدارة العيادة" : "✚  CLINIC MANAGEMENT";
            UpdateRegistrationGuide();
        }

        private void ApplyVisualPolish()
        {
            BackColor = Color.FromArgb(246, 250, 253);
            panelHeaderContainer.BackColor = Navy;
            lblTitle.ForeColor = Color.White;
            tcPatient.Font = new Font("Segoe UI Semibold", 10F);
            tcPatient.Appearance = TabAppearance.Normal;
            tpPersonalInfo.BackColor = Color.White;
            tpPatientInfo.BackColor = Color.White;
            ctrlPersonCardWithFilter1.BackColor = Color.White;
            ctrlPersonCardWithFilter1.Location = new Point(8, 10);
            ctrlPersonCardWithFilter1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            foreach (Button button in new[] { btnSave, btnPersonInfoNext })
            {
                button.BackColor = Blue;
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = Teal;
                button.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
                button.Cursor = Cursors.Hand;
                // Keep the existing resource images (Save_32 / Next_32).
                button.ImageAlign = ContentAlignment.MiddleLeft;
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.UseVisualStyleBackColor = false;
            }
            btnClose.Image = null;
            btnClose.ImageAlign = ContentAlignment.MiddleLeft;
            btnClose.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnClose.Text = "Close";
            btnClose.BackColor = Color.White;
            btnClose.ForeColor = Ink;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 1;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(190, 214, 230);
            btnClose.Font = new Font("Segoe UI Semibold", 10F);
            btnClose.TextAlign = ContentAlignment.MiddleCenter;
            btnClose.UseVisualStyleBackColor = false;

            cbBloodType.BackColor = Color.White;
            cbBloodType.ForeColor = Ink;
            cbBloodType.FlatStyle = FlatStyle.Standard;
            cbBloodType.Width = 325;
            txtEmergencyContact.Width = 325;
            txtMedicalHistory.Size = new Size(450, 112);
            foreach (TextBox field in new[] { txtEmergencyContact, txtMedicalHistory })
            {
                field.BackColor = Color.White;
                field.ForeColor = Ink;
                field.BorderStyle = BorderStyle.FixedSingle;
            }
            lblPatientID.ForeColor = Blue;
            lblPatientID.Text = "—";
            foreach (Label field in new[] { label6, label7, label8, label15 })
            {
                field.ForeColor = Ink;
                field.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            }
            // Align the existing fields into a neat two-column label/input grid.
            label8.Location = new Point(55, 66);
            pictureBox5.Location = new Point(260, 65);
            lblPatientID.Location = new Point(310, 67);
            label15.Location = new Point(55, 139);
            pictureBox6.Location = new Point(260, 138);
            txtEmergencyContact.Location = new Point(310, 135);
            label6.Location = new Point(55, 212);
            pictureBox4.Location = new Point(260, 211);
            cbBloodType.Location = new Point(310, 207);
            label7.Location = new Point(55, 285);
            pictureBox7.Location = new Point(260, 284);
            txtMedicalHistory.Location = new Point(310, 279);
            foreach (PictureBox picture in new[] { pictureBox4, pictureBox5, pictureBox6, pictureBox7 })
            {
                picture.Size = new Size(32, 32);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                // Keep exact original project icon images, do not replace with emoji.
            }

            // The nested person card owns its photo and its data display.
            // Never cover, hide or overwrite its controls.
            // Subtle gradients and rounded button shapes to match the reference.
            panelHeaderContainer.Paint += (sender, e) =>
            {
                using (var brush = new LinearGradientBrush(
                    panelHeaderContainer.ClientRectangle,
                    Color.FromArgb(8, 54, 102), Color.FromArgb(4, 77, 135), 0f))
                    e.Graphics.FillRectangle(brush, panelHeaderContainer.ClientRectangle);
            };
            // Draw modern buttons ourselves. The old bitmap icons are intentionally
            // removed: their backgrounds and pixelated edges caused the ugly result.
            ConfigureActionButton(btnSave, ActionGlyph.Save, true);
            ConfigureActionButton(btnClose, ActionGlyph.Close, false);
            ConfigureActionButton(btnPersonInfoNext, ActionGlyph.Next, true);
            btnSave.Text = "Save";
            btnClose.Text = "Close";
            btnPersonInfoNext.Text = "Next: Patient Info";
            btnSave.Size = new Size(154, 50);
            btnClose.Size = new Size(154, 50);
            btnPersonInfoNext.Size = new Size(215, 46);
            PositionGuideControls();
        }

        private enum ActionGlyph { Save, Close, Next }

        private void ConfigureActionButton(Button button, ActionGlyph glyph, bool primary)
        {
            button.Image = null;
            button.BackgroundImage = null;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            button.BackColor = primary ? Blue : Color.White;
            button.ForeColor = primary ? Color.White : Navy;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Padding = Padding.Empty;
            button.Paint += (sender, e) => PaintActionButton(button, e.Graphics, glyph, primary);
            button.Resize += (sender, e) => button.Invalidate();
            button.MouseEnter += (sender, e) => button.Invalidate();
            button.MouseLeave += (sender, e) => button.Invalidate();
            button.EnabledChanged += (sender, e) => button.Invalidate();
        }

        private static GraphicsPath RoundedRect(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float d = radius * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void PaintActionButton(Button button, Graphics g, ActionGlyph glyph, bool primary)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            RectangleF rect = new RectangleF(1, 1, button.Width - 3, button.Height - 3);
            if (rect.Width < 20 || rect.Height < 20) return;

            bool enabled = button.Enabled;
            bool hovered = button.ClientRectangle.Contains(button.PointToClient(Cursor.Position));
            Color bg = !enabled ? Color.FromArgb(183, 204, 225)
                : primary ? (hovered ? Color.FromArgb(6, 91, 182) : Color.FromArgb(13, 111, 207))
                : (hovered ? Color.FromArgb(235, 244, 252) : Color.White);
            Color fg = primary ? Color.White : Color.FromArgb(10, 62, 110);
            if (!enabled) fg = Color.FromArgb(240, 245, 250);

            using (var shape = RoundedRect(rect, 11))
            using (var fill = new SolidBrush(bg))
            using (var border = new Pen(primary ? bg : Color.FromArgb(185, 208, 229), 1.3F))
            {
                g.FillPath(fill, shape);
                g.DrawPath(border, shape);
            }

            // One unified icon/text group, centered together, not stretched.
            string caption = button.Text ?? "";
            Size textSize = TextRenderer.MeasureText(g, caption, button.Font,
                new Size(1000, button.Height), TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            int iconSize = 20;
            int gap = 11;
            int groupWidth = iconSize + gap + textSize.Width;
            int iconX = Math.Max(10, (button.Width - groupWidth) / 2);
            int iconY = (button.Height - iconSize) / 2;
            DrawActionGlyph(g, glyph, new Rectangle(iconX, iconY, iconSize, iconSize), fg);
            var textRect = new Rectangle(iconX + iconSize + gap, 0,
                Math.Max(1, button.Width - iconX - iconSize - gap - 6), button.Height);
            TextRenderer.DrawText(g, caption, button.Font, textRect, fg,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
        }

        private static void DrawActionGlyph(Graphics g, ActionGlyph glyph, Rectangle box, Color color)
        {
            using (var pen = new Pen(color, 1.9F))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                float x = box.X, y = box.Y;
                if (glyph == ActionGlyph.Close)
                {
                    g.DrawEllipse(pen, x + 1.5F, y + 1.5F, 17, 17);
                    g.DrawLine(pen, x + 6, y + 6, x + 14, y + 14);
                    g.DrawLine(pen, x + 14, y + 6, x + 6, y + 14);
                }
                else if (glyph == ActionGlyph.Next)
                {
                    g.DrawEllipse(pen, x + 1.5F, y + 1.5F, 17, 17);
                    g.DrawLine(pen, x + 5, y + 10, x + 15, y + 10);
                    g.DrawLine(pen, x + 11, y + 6, x + 15, y + 10);
                    g.DrawLine(pen, x + 15, y + 10, x + 11, y + 14);
                }
                else
                {
                    // Minimal vector floppy-disk icon; no embedded bitmap required.
                    g.DrawRectangle(pen, x + 2, y + 2, 16, 16);
                    g.DrawRectangle(pen, x + 6, y + 3, 9, 6);
                    g.DrawRectangle(pen, x + 6, y + 12, 9, 6);
                }
            }
        }

        private void RefreshEmptyState()
        {
            // Update only this form's patient ID label.
            // Do not manipulate the photo or labels in ctrlPersonCardWithFilter1:
            // that control updates them when a person is found or selected.
            if (_Patient == null || _Patient.PatientID <= 0)
                lblPatientID.Text = TranslationClass.IsArabic
                    ? "لم يصدر بعد" : "Not generated yet";
        }

        private void _FillBloodTypesInComboBox()
        {
            DataTable dt = clsBloodType.GetAllBloodTypes();
            if (dt != null && dt.Rows.Count > 0)
            {
                cbBloodType.DataSource = dt;
                cbBloodType.DisplayMember = "BloodTypeName";
                cbBloodType.ValueMember = "BloodTypeID";
                cbBloodType.SelectedIndex = cbBloodType.FindString("Unknown");
            }
        }

        private void _LoadData()
        {
            _Patient = clsPatient.FindPatientByID(_PatientID);
            if (_Patient == null)
            {
                MessageBox.Show(Tr("No Patient with ID = ") + _PatientID, Tr("Patient Not Found"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
                return;
            }
            ctrlPersonCardWithFilter1.LoadPersonInfo(_Patient.PersonID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            lblPatientID.Text = _PatientID.ToString();
            RefreshEmptyState();
            txtEmergencyContact.Text = _Patient.EmergencyContact;
            txtMedicalHistory.Text = _Patient.MedicalHistory;
            if (_Patient.BloodTypeInfo != null)
            {
                int bloodTypeIndex = cbBloodType.FindString(_Patient.BloodTypeInfo.BloodTypeName);
                if (bloodTypeIndex != -1)
                    cbBloodType.SelectedIndex = bloodTypeIndex;
            }
        }

        private void _ResetDefaultValues()
        {
            _FillBloodTypesInComboBox();
            tpPatientInfo.Enabled = false;
            btnSave.Enabled = false;
            _patientSaved = false;
            if (_Mode == enMode.AddNew)
            {
                _Patient = new clsPatient();
                ctrlPersonCardWithFilter1.FilterEnabled = true;
                ctrlPersonCardWithFilter1.FocuseTextBox();
                lblTitle.Text = "Add New Patient";
            }
            else
            {
                lblTitle.Text = "Update Patient Info";
            }
            Text = lblTitle.Text;
        }

        private void frmAddUpdatePatient_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();
            if (_Mode == enMode.Update)
                _LoadData();
            ApplyArabicTranslation();
            RefreshEmptyState();
        }

        private void btnPersonInfoNext_Click(object sender, EventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                btnSave.Enabled = true;
                tpPatientInfo.Enabled = true;
                tcPatient.SelectedTab = tpPatientInfo;
                UpdateRegistrationGuide();
                return;
            }

            int personID = ctrlPersonCardWithFilter1.PersonID;

            if (personID <= 0)
            {
                MessageBox.Show(
                    TranslationClass.IsArabic
                        ? "يرجى اختيار شخص أو حفظ بيانات شخص جديد أولاً."
                        : "Please select a person or save a new person first.",
                    Tr("Select a Person"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                ctrlPersonCardWithFilter1.FocuseTextBox();
                return;
            }

            if (clsPatient.IsPatientExistForPersonID(personID))
            {
                DialogResult result = MessageBox.Show(
                    TranslationClass.IsArabic
                        ? "هذا الشخص مسجل بالفعل كمريض.\n\nهل تريد فتح سجل المريض الحالي للتعديل؟"
                        : "This person is already registered as a patient.\n\nWould you like to open the existing patient record for editing?",
                    TranslationClass.IsArabic
                        ? "المريض مسجل بالفعل"
                        : "Patient Already Registered",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int existingPatientID = GetExistingPatientID(personID);

                    if (existingPatientID <= 0)
                    {
                        MessageBox.Show(
                            Tr("Unable to find the existing patient record."),
                            Tr("Error"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    using (frmAddUpdatePatient editForm =
                        new frmAddUpdatePatient(existingPatientID))
                    {
                        editForm.ShowDialog(this);
                    }

                    return;
                }

                ctrlPersonCardWithFilter1.FocuseTextBox();
                return;
            }

            btnSave.Enabled = true;
            tpPatientInfo.Enabled = true;
            tcPatient.SelectedTab = tpPatientInfo;

            UpdateRegistrationGuide();
        }
        private void btnClose_Click(object sender, EventArgs e) { Close(); }

        //   private void txtEmergencyContact_Validating(
        //object sender, CancelEventArgs e)
        //   {
        //       e.Cancel = false;
        //       errorProvider1.SetError(txtEmergencyContact, "");
        //   }

        private void txtEmergencyContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                MessageBox.Show(Tr("Some fields are not valid!, put the mouse over the red icon(s) to see the error"),
                    Tr("Validation Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Existing database assignment and save logic intentionally unchanged.
            _Patient.PersonID = ctrlPersonCardWithFilter1.PersonID;
            _Patient.EmergencyContact = txtEmergencyContact.Text.Trim();
            _Patient.MedicalHistory = string.IsNullOrEmpty(txtMedicalHistory.Text.Trim()) ? null : txtMedicalHistory.Text.Trim();
            _Patient.BloodTypeID = (int)cbBloodType.SelectedValue;
            _Patient.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            if (_Patient.SavePatient())
            {
                lblPatientID.Text = _Patient.PatientID.ToString();
                _Mode = enMode.Update;
                _patientSaved = true;
                ctrlPersonCardWithFilter1.FilterEnabled = false;
                lblTitle.Text = Tr("Update Patient Info");
                Text = Tr("Update Patient Info");
                UpdateRegistrationGuide();
                DataBack?.Invoke(this, _Patient.PatientID);
                MessageBox.Show(
                    TranslationClass.IsArabic
                        ? "تم حفظ بيانات المريض بنجاح. رقم المريض: " + _Patient.PatientID
                        : "Patient saved successfully. Patient ID: " + _Patient.PatientID,
                    Tr("Saved"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Tr("Error: Data Is not Saved Successfully."), Tr("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetExistingPatientID(int personID)
        {
            clsPatient existingPatient =
                clsPatient.FindPatientByPersonID(personID);

            return existingPatient != null
                ? existingPatient.PatientID
                : -1;
        }
        private void tcPatient_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage.Name == "tpPatientInfo" && _Mode == enMode.AddNew)
            {
                if (ctrlPersonCardWithFilter1.PersonID == -1 || clsPatient.IsPatientExistForPersonID(ctrlPersonCardWithFilter1.PersonID))
                {
                    e.Cancel = true;
                    MessageBox.Show(Tr("Please select a valid person first before moving to patient medical info."), Tr("Selection Required"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void frmAddUpdatePatient_Activated(object sender, EventArgs e)
        {
            // Do not force focus or modify nested person-card controls on activation.
            // Searching and opening the person editor may reactivate this form.
            // The person card must be allowed to finish loading its own photo.
        }

        private void btnExit_Click(object sender, EventArgs e) { Close(); }
    }
}
