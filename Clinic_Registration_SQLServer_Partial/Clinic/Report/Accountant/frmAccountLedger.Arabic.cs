using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YamyProject.UI.Reports.Accountant
{
    public partial class frmAccountLedger
    {
        private readonly Dictionary<Control, string> _ledgerOriginalTexts =
            new Dictionary<Control, string>();

        private static readonly Dictionary<string, string> _ledgerArabic =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Account Ledger", "دفتر الأستاذ" },
            { "Select Account", "اختر الحساب" },
            { "Select an account", "اختر حسابًا" },
            { "-- Select Account --", "-- اختر الحساب --" },
            { "Account", "الحساب" },
            { "Account Name", "اسم الحساب" },
            { "From Date", "من تاريخ" },
            { "To Date", "إلى تاريخ" },
            { "From", "من" },
            { "To", "إلى" },
            { "View", "عرض" },
            { "View Report", "عرض التقرير" },
            { "View Ledger", "عرض دفتر الأستاذ" },
            { "Print", "طباعة" },
            { "Export", "تصدير" },
           { "Excel", "إكسل" },
            { "PDF", "بي دي إف" },
            { "Close", "إغلاق" },
            { "Date", "التاريخ" },
            { "Reference", "المرجع" },
            { "Transaction", "المعاملة" },
            { "Description", "الوصف" },
            { "Debit", "مدين" },
            { "Credit", "دائن" },
            { "Balance", "الرصيد" },
            { "Opening Balance", "الرصيد الافتتاحي" },
            { "Closing Balance", "الرصيد الختامي" },
            { "Period Total", "إجمالي الفترة" },
            { "Period", "الفترة" },
            { "Total Debit", "إجمالي المدين" },
            { "Total Credit", "إجمالي الدائن" },
            { "Transactions", "المعاملات" },
            { "Transaction Count", "عدد المعاملات" }
        };

        private string LedgerTranslate(string english)
        {
            if (!Clinic.TranslationClass.IsArabic ||
                string.IsNullOrWhiteSpace(english))
                return english;

            string arabic;

            if (_ledgerArabic.TryGetValue(english.Trim(), out arabic))
                return arabic;

            return Clinic.TranslationClass.Translate(english, true);
        }


        private readonly Dictionary<string, string> _ledgerOriginalHeaders =
            new Dictionary<string, string>();


        public void ApplyArabicProperties()
        {
            bool arabic = Clinic.TranslationClass.IsArabic;

            this.SuspendLayout();

            try
            {
                // IMPORTANT:
                // Do not enable automatic form mirroring.
                // We are mirroring positions manually.
                this.RightToLeftLayout = false;
                this.RightToLeft = RightToLeft.No;

                // Capture the original English coordinates
                // only once for each control.
                CaptureLedgerEnglishLayout(this);

                // Physically reverse the control positions.
                MirrorLedgerControlPositions(this, arabic);

                // Translate all static labels and buttons.
                TranslateLedgerControls(this);

                // Ledger grid.
                if (dgvLedger != null)
                {
                    dgvLedger.RightToLeft = arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;

                    dgvLedger.ColumnHeadersDefaultCellStyle.Alignment =
                        arabic
                            ? DataGridViewContentAlignment.MiddleRight
                            : DataGridViewContentAlignment.MiddleLeft;

                    foreach (DataGridViewColumn column
                             in dgvLedger.Columns)
                    {
                        if (column.Name == "TransactionId")
                            continue;

                        if (!_ledgerOriginalHeaders.ContainsKey(
                            column.Name))
                        {
                            _ledgerOriginalHeaders[column.Name] =
                                column.HeaderText;
                        }

                        column.HeaderText = LedgerTranslate(
                            _ledgerOriginalHeaders[column.Name]);
                    }

                    foreach (string name in new[]
                    {
                "Debit", "Credit", "Balance"
            })
                    {
                        if (dgvLedger.Columns.Contains(name))
                        {
                            dgvLedger.Columns[name]
                                .DefaultCellStyle.Alignment =
                                DataGridViewContentAlignment.MiddleRight;
                        }
                    }
                }

                // Account heading.
                if (lblSelectedAccount != null)
                {
                    if (lblSelectedAccount.Text ==
                            "Select an account" ||
                        lblSelectedAccount.Text == "اختر حسابًا")
                    {
                        lblSelectedAccount.Text =
                            LedgerTranslate("Select an account");
                    }

                    lblSelectedAccount.TextAlign = arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }

                // Period heading.
                if (lblPeriod != null)
                {
                    if (lblPeriod.Text == "Period" ||
                        lblPeriod.Text == "الفترة")
                    {
                        lblPeriod.Text = LedgerTranslate("Period");
                    }

                    lblPeriod.TextAlign = arabic
                        ? ContentAlignment.MiddleRight
                        : ContentAlignment.MiddleLeft;
                }

                // Keep currency values readable.
                foreach (Label label in new[]
                {
            lblOpeningValue,
            lblDebitValue,
            lblCreditValue,
            lblClosingValue
        })
                {
                    if (label != null)
                    {
                        label.RightToLeft = RightToLeft.No;
                        label.TextAlign = arabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                }

                // Account dropdown text.
                if (cmbAccount != null)
                {
                    cmbAccount.RightToLeft = arabic
                        ? RightToLeft.Yes
                        : RightToLeft.No;
                }

                // Keep dates readable.
                dtpFrom.RightToLeft = RightToLeft.No;
                dtpTo.RightToLeft = RightToLeft.No;

                TranslateLedgerResultRows();

                this.Text = LedgerTranslate("Account Ledger");
            }
            finally
            {
                this.ResumeLayout(true);
            }
        }

        private void TranslateLedgerControls(Control parent)
        {
            bool arabic = Clinic.TranslationClass.IsArabic;

            foreach (Control control in parent.Controls)
            {
                control.RightToLeft = arabic
    ? RightToLeft.Yes
    : RightToLeft.No;
                // Do not translate account names, numeric results or dates.
                bool dynamicControl =
                    control == lblSelectedAccount ||
                    control == lblPeriod ||
                    control == lblOpeningValue ||
                    control == lblDebitValue ||
                    control == lblCreditValue ||
                    control == lblClosingValue ||
                    control == lblTransactionCount ||
                    control == cmbAccount ||
                    control == dgvLedger;

                if (!dynamicControl)
                {
                    if (!_ledgerOriginalTexts.ContainsKey(control))
                        _ledgerOriginalTexts[control] = control.Text;

                    control.Text = LedgerTranslate(
                        _ledgerOriginalTexts[control]);
                }

                if (control is Label)
                {
                    Label label = (Label)control;

                    if (!dynamicControl)
                    {
                        label.TextAlign = arabic
                            ? ContentAlignment.MiddleRight
                            : ContentAlignment.MiddleLeft;
                    }
                }

                if (control is Button)
                {
                    ((Button)control).TextAlign =
                        ContentAlignment.MiddleCenter;
                }

                if (control.HasChildren)
                    TranslateLedgerControls(control);
            }
        }

        private sealed class LedgerControlLayout
        {
            public Rectangle Bounds { get; set; }
            public AnchorStyles Anchor { get; set; }
        }

        private readonly Dictionary<Control, LedgerControlLayout>
            _ledgerEnglishLayout =
                new Dictionary<Control, LedgerControlLayout>();

        private void CaptureLedgerEnglishLayout(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (!_ledgerEnglishLayout.ContainsKey(control))
                {
                    _ledgerEnglishLayout[control] =
                        new LedgerControlLayout
                        {
                            Bounds = control.Bounds,
                            Anchor = control.Anchor
                        };
                }

                if (control.HasChildren)
                    CaptureLedgerEnglishLayout(control);
            }
        }

        private static AnchorStyles MirrorLedgerAnchor(
            AnchorStyles original)
        {
            bool left = (original & AnchorStyles.Left) != 0;
            bool right = (original & AnchorStyles.Right) != 0;

            AnchorStyles result =
                original & ~(AnchorStyles.Left | AnchorStyles.Right);

            if (left)
                result |= AnchorStyles.Right;

            if (right)
                result |= AnchorStyles.Left;

            return result;
        }

        private void MirrorLedgerControlPositions(
            Control parent,
            bool arabic)
        {
            parent.SuspendLayout();

            try
            {
                foreach (Control control in parent.Controls)
                {
                    LedgerControlLayout original;

                    if (!_ledgerEnglishLayout.TryGetValue(
                        control, out original))
                        continue;

                    // Preserve docking. A docked control occupies
                    // an edge or fills its parent.
                    if (control.Dock == DockStyle.None)
                    {
                        Rectangle bounds = original.Bounds;

                        int newX = arabic
                            ? parent.ClientSize.Width
                                - bounds.Left
                                - bounds.Width
                            : bounds.Left;

                        // Set bounds before swapping anchors.
                        control.Anchor = AnchorStyles.Top |
                                         AnchorStyles.Left;

                        control.Bounds = new Rectangle(
                            newX,
                            bounds.Top,
                            bounds.Width,
                            bounds.Height);

                        control.Anchor = arabic
                            ? MirrorLedgerAnchor(original.Anchor)
                            : original.Anchor;
                    }

                    // Each panel must mirror its own children.
                    if (control.HasChildren)
                    {
                        MirrorLedgerControlPositions(
                            control, arabic);
                    }
                }
            }
            finally
            {
                parent.ResumeLayout(true);
            }
        }


        private void TranslateLedgerResultRows()
        {
            if (dgvLedger == null)
                return;

            foreach (DataGridViewRow row in dgvLedger.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var cell = row.Cells["Description"];

                string value = Convert.ToString(cell.Value);

                if (value == "Opening Balance" ||
                    value == "Period Total" ||
                    value == "الرصيد الافتتاحي" ||
                    value == "إجمالي الفترة")
                {
                    string english =
                        value == "الرصيد الافتتاحي"
                            ? "Opening Balance"
                            : value == "إجمالي الفترة"
                                ? "Period Total"
                                : value;

                    cell.Value = LedgerTranslate(english);
                }
            }
        }
    }
}
