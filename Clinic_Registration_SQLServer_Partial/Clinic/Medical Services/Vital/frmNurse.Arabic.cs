
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Vital
{
    public partial class frmNurse : Form
    {
        private readonly Dictionary<Control, string>
            _nurseOriginalTexts = new Dictionary<Control, string>();

        private readonly Dictionary<Control, Rectangle>
            _nurseOriginalBounds = new Dictionary<Control, Rectangle>();

        private readonly Dictionary<Control, ContentAlignment>
            _nurseOriginalAlignments =
                new Dictionary<Control, ContentAlignment>();

        private readonly Dictionary<DataGridViewColumn, string>
            _nurseOriginalHeaders =
                new Dictionary<DataGridViewColumn, string>();

        private bool IsNurseArabic()
        {
            string language =
                Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

            return language.StartsWith(
                "ar", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyNurseArabicUI()
        {
            bool arabic = IsNurseArabic();

            SuspendLayout();

            try
            {
                // Do not globally mirror the form: that can
                // interfere with docking and numeric controls.
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;

                Text = arabic
                    ? "العيادة الحديثة - محطة التمريض"
                    : "Modern Clinic - Nurse Station";

                TranslateNurseControls(this, arabic);
                ApplyNursePositions(arabic);
                ApplyNurseGrid(arabic);

                // Mirror header Exit button separately.
                ApplyNurseExitButtonPosition(arabic);

                txtBloodPressure.RightToLeft = RightToLeft.No;
                nudTemperature.RightToLeft = RightToLeft.No;
                nudWeight.RightToLeft = RightToLeft.No;
                nudPulse.RightToLeft = RightToLeft.No;
                lbDate.RightToLeft = RightToLeft.No;

                // Patient name comes from the database.
                lbPatientName.RightToLeft =
                    arabic ? RightToLeft.Yes : RightToLeft.No;
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        private void TranslateNurseControls(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                bool dynamicValue =
                    control == lbPatientName ||
                    control == lblRecordsCount ||
                    control == lbDate;

                bool input =
                    control is TextBoxBase ||
                    control is NumericUpDown ||
                    control is ComboBox ||
                    control is DataGridView;

                if (!dynamicValue && !input)
                {
                    if (!_nurseOriginalTexts.ContainsKey(control))
                        _nurseOriginalTexts[control] = control.Text ?? "";

                    string original = _nurseOriginalTexts[control];

                    if (!string.IsNullOrWhiteSpace(original))
                        control.Text = TranslateNurseText(original, arabic);

                    if (control is Label label)
                    {
                        if (!_nurseOriginalAlignments.ContainsKey(control))
                            _nurseOriginalAlignments[control] = label.TextAlign;

                        label.RightToLeft =
                            arabic ? RightToLeft.Yes : RightToLeft.No;

                        label.TextAlign = arabic
                            ? ContentAlignment.MiddleRight
                            : _nurseOriginalAlignments[control];
                    }

                    if (control is ButtonBase button)
                    {
                        button.RightToLeft =
                            arabic ? RightToLeft.Yes : RightToLeft.No;
                    }
                }

                if (control.HasChildren)
                    TranslateNurseControls(control, arabic);
            }
        }

        private string TranslateNurseText(string english, bool arabic)
        {
            if (!arabic)
                return english;

            string normalized = Regex.Replace(
                english.Replace("\r", " ").Replace("\n", " "),
                @"\s+", " ").Trim();

            if (string.IsNullOrEmpty(normalized))
                return english;

            var dictionary = Clinic.TranslationClass.sidebarTranslations;

            if (dictionary.TryGetValue(normalized, out string translated))
                return translated;

            string withoutColon = normalized.TrimEnd(':', ' ', '：');

            if (dictionary.TryGetValue(withoutColon, out translated))
                return translated.TrimEnd(':') +
                    (normalized.EndsWith(":") ? ":" : "");

            if (dictionary.TryGetValue(withoutColon + ":", out translated))
                return translated;

            // Preserve a leading icon when translating captions.
            foreach (var item in dictionary.OrderByDescending(x => x.Key.Length))
            {
                if (normalized.EndsWith(
                    " " + item.Key, StringComparison.OrdinalIgnoreCase))
                {
                    string prefix = normalized.Substring(
                        0, normalized.Length - item.Key.Length).Trim();

                    if (prefix.Length <= 3)
                        return prefix + " " + item.Value;
                }
            }

            return english;
        }

        private void ApplyNursePositions(bool arabic)
        {
            // Capture original positions once, before mirroring.
            // Only move freely positioned controls.
            MirrorNurseChildren(this, arabic);
        }

        private void MirrorNurseChildren(Control parent, bool arabic)
        {
            foreach (Control control in parent.Controls)
            {
                if (!_nurseOriginalBounds.ContainsKey(control))
                    _nurseOriginalBounds[control] = control.Bounds;

                Rectangle original = _nurseOriginalBounds[control];

                // Docked/anchored containers require layout-aware
                // handling, not arbitrary coordinate changes.
                if (control.Dock == DockStyle.None &&
                    control.Anchor == (AnchorStyles.Top | AnchorStyles.Left) &&
                    !(control is TextBoxBase) &&
                    !(control is NumericUpDown))
                {
                    int x = arabic
                        ? parent.ClientSize.Width - original.Right
                        : original.Left;

                    control.Location = new Point(x, original.Top);
                }

                // Don't mirror the internals of the grid.
                if (control is DataGridView)
                    continue;

                if (control.HasChildren)
                    MirrorNurseChildren(control, arabic);
            }
        }
        private void ApplyNurseExitButtonPosition(bool arabic)
        {
            Control button = btnExit;

            if (button.Parent == null)
                return;

            // Save the original English position once.
            if (!_nurseOriginalBounds.ContainsKey(button))
            {
                _nurseOriginalBounds[button] = button.Bounds;
            }

            Rectangle original = _nurseOriginalBounds[button];

            // Prevent right anchoring from overriding our position.
            button.Anchor = arabic
                ? AnchorStyles.Top | AnchorStyles.Left
                : AnchorStyles.Top | AnchorStyles.Right;

            if (arabic)
            {
                // Move Exit button to the left side of its parent.
                button.Left = 15;
            }
            else
            {
                // Restore its original English position.
                button.Left = original.Left;
            }

            button.Top = original.Top;
            button.BringToFront();
        }
        private void ApplyNurseGrid(bool arabic)
        {
            foreach (DataGridViewColumn column in dgvNurseQueue.Columns)
            {
                if (!_nurseOriginalHeaders.ContainsKey(column))
                    _nurseOriginalHeaders[column] = column.HeaderText;

                string original = _nurseOriginalHeaders[column];

                column.HeaderText =
                    TranslateNurseText(original, arabic);
            }

            dgvNurseQueue.RightToLeft =
                arabic ? RightToLeft.Yes : RightToLeft.No;

            dgvNurseQueue.ColumnHeadersDefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleCenter
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvNurseQueue.DefaultCellStyle.Alignment =
                arabic
                    ? DataGridViewContentAlignment.MiddleRight
                    : DataGridViewContentAlignment.MiddleLeft;

            dgvNurseQueue.Refresh();
        }
    }
}
