using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Clinic_Common;

namespace Clinic.Login
{
    // Add as a normal CLASS file, not a Windows Form. Do not create a Designer file.
    internal static class LoginRegistrationLanguage
    {
        private static readonly Dictionary<string, string> Arabic =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Welcome To", "مرحباً بك في" },
            { "Clinic Management System", "نظام إدارة العيادة" },
            { "PROFESSIONAL CARE | BETTER HEALTH | BRIGHTER TOMORROW", "رعاية احترافية | صحة أفضل | مستقبل أكثر إشراقاً" },
            { "Manage Appointments", "إدارة المواعيد" },
            { "Efficient scheduling for better service", "جدولة فعالة لخدمة أفضل" },
            { "Patient Management", "إدارة المرضى" },
            { "Comprehensive patient records", "سجلات شاملة للمرضى" },
            { "Reports & Analytics", "التقارير والتحليلات" },
            { "Insights for improved care", "رؤى لتحسين الرعاية" },
            { "Version 1.0", "الإصدار 1.0" },
            { "Login to your account", "تسجيل الدخول إلى حسابك" },
            { "Access your clinic management system", "الوصول إلى نظام إدارة العيادة" },
            { "Remember Me", "تذكرني" },
            { "Forgot Password?", "هل نسيت كلمة المرور؟" },
            { "Login", "تسجيل الدخول" },
            { "Register Account", "إنشاء حساب" },
            { "New to the system?", "مستخدم جديد؟" },
            { "Register", "تسجيل" },
            { "Create Account", "إنشاء حساب" },
            { "Create Your Account", "إنشاء حسابك" },
            { "Register Clinic Account", "إنشاء حساب للعيادة" },
            { "Fill in your details to create a new account", "أدخل بياناتك لإنشاء حساب جديد" },
            { "Join our clinic management system and be part of better healthcare tomorrow.", "انضم إلى نظام إدارة العيادة وساهم في رعاية صحية أفضل غداً." },
            { "Secure Registration", "تسجيل آمن" },
            { "Easy Access", "سهولة الوصول" },
            { "Better Healthcare", "رعاية صحية أفضل" },
            { "Full Name", "الاسم الكامل" },
            { "Phone", "الهاتف" },
            { "Phone Number", "رقم الهاتف" },
            { "Email", "البريد الإلكتروني" },
            { "Email Address", "البريد الإلكتروني" },
            { "Address", "العنوان" },
            { "Nationality", "الجنسية" },
            { "Country", "الدولة" },
            { "Username", "اسم المستخدم" },
            { "User Name", "اسم المستخدم" },
            { "Password", "كلمة المرور" },
            { "Confirm Password", "تأكيد كلمة المرور" },
            { "Date of Birth", "تاريخ الميلاد" },
            { "Gender", "الجنس" },
            { "Male", "ذكر" },
            { "Female", "أنثى" },
            { "Role", "الدور" },
            { "Select Role", "اختر الدور" },
            { "Save", "حفظ" },
            { "Close", "إغلاق" },
            { "Cancel", "إلغاء" },
            { "Already have an account?", "هل لديك حساب بالفعل؟" },
            { "Your account will be ready to use immediately after registration.", "سيكون حسابك جاهزاً للاستخدام فور التسجيل." }
        };

        private sealed class Original
        {
            public string Text;
            public Rectangle Bounds;
            public ContentAlignment? Alignment;
            public RightToLeft Direction;
            public AnchorStyles Anchor;
        }

        private static readonly Dictionary<Control, Original> Originals =
            new Dictionary<Control, Original>();

        private static string Normalize(string text)
        {
            if (text == null) return "";
            // Makes multi-line labels and decorative separators translatable.
            return Regex.Replace(text.Replace('➜', ' ').Replace('✚', ' ')
                .Replace('─', ' ').Replace('✓', ' '), @"\s+", " ").Trim();
        }

        public static void Apply(Control root)
        {
            if (root == null || root.IsDisposed) return;
            Capture(root);
            bool arabic = TranslationClass.IsArabic;
            root.SuspendLayout();
            try
            {
                // Always start with the original English geometry, so toggling
                // never causes cumulative movement or layout drift.
                RestoreGeometry(root);
                if (arabic) MirrorChildren(root);
                ApplyText(root, arabic);
            }
            finally { root.ResumeLayout(true); }
        }

        private static void Capture(Control parent)
        {
            if (!Originals.ContainsKey(parent))
            {
                ContentAlignment? alignment = null;
                if (parent is Label) alignment = ((Label)parent).TextAlign;
                else if (parent is Button) alignment = ((Button)parent).TextAlign;
                else if (parent is CheckBox) alignment = ((CheckBox)parent).TextAlign;
                Originals[parent] = new Original
                {
                    Text = parent.Text ?? "",
                    Bounds = parent.Bounds,
                    Alignment = alignment,
                    Direction = parent.RightToLeft,
                    Anchor = parent.Anchor
                };
            }
            foreach (Control child in parent.Controls) Capture(child);
        }

        private static void RestoreGeometry(Control parent)
        {
            Original original;
            if (Originals.TryGetValue(parent, out original))
            {
                if (!(parent is Form) && parent.Dock == DockStyle.None &&
                    !(parent.Parent is FlowLayoutPanel) &&
                    !(parent.Parent is TableLayoutPanel))
                {
                    parent.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    parent.Bounds = original.Bounds;
                    parent.Anchor = original.Anchor;
                }
                parent.RightToLeft = original.Direction;
            }
            foreach (Control child in parent.Controls) RestoreGeometry(child);
        }

        private static void MirrorChildren(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                Original original;
                if (!Originals.TryGetValue(child, out original)) continue;

                // Layout-managed controls are positioned by their layout engines.
                // Avoid fighting Dock, TableLayoutPanel or FlowLayoutPanel.
                if (child.Dock == DockStyle.None &&
                    !(parent is SplitContainer) && !(parent is SplitterPanel) &&
                    !(parent is FlowLayoutPanel) && !(parent is TableLayoutPanel))
                {
                    child.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    child.Left = Math.Max(0, parent.ClientSize.Width - original.Bounds.Right);
                    child.Anchor = MirrorAnchor(original.Anchor);
                }
                MirrorChildren(child);
            }
        }

        private static AnchorStyles MirrorAnchor(AnchorStyles anchor)
        {
            bool left = (anchor & AnchorStyles.Left) != 0;
            bool right = (anchor & AnchorStyles.Right) != 0;
            anchor &= ~(AnchorStyles.Left | AnchorStyles.Right);
            if (left) anchor |= AnchorStyles.Right;
            if (right) anchor |= AnchorStyles.Left;
            return anchor;
        }

        private static ContentAlignment MirrorAlignment(ContentAlignment a)
        {
            switch (a)
            {
                case ContentAlignment.TopLeft: return ContentAlignment.TopRight;
                case ContentAlignment.MiddleLeft: return ContentAlignment.MiddleRight;
                case ContentAlignment.BottomLeft: return ContentAlignment.BottomRight;
                case ContentAlignment.TopRight: return ContentAlignment.TopLeft;
                case ContentAlignment.MiddleRight: return ContentAlignment.MiddleLeft;
                case ContentAlignment.BottomRight: return ContentAlignment.BottomLeft;
                default: return a;
            }
        }

        private static void ApplyText(Control parent, bool arabic)
        {
            Original original;
            if (Originals.TryGetValue(parent, out original))
            {
                bool input = parent is TextBoxBase || parent is ComboBox ||
                             parent is DateTimePicker || parent is NumericUpDown;
                if (!input)
                {
                    string english = original.Text;
                    string translated;
                    string key = Normalize(english);
                    if (arabic && Arabic.TryGetValue(key, out translated))
                        parent.Text = translated;
                    else if (arabic && key.Contains("New to the system?"))
                        parent.Text = "مستخدم جديد؟";
                    else if (arabic && key.StartsWith("Join our clinic management system"))
                        parent.Text = Arabic["Join our clinic management system and be part of better healthcare tomorrow."];
                    else if (arabic && key.Contains("Secure Registration"))
                        parent.Text = english.Replace("Secure Registration", "تسجيل آمن")
                            .Replace("Easy Access", "سهولة الوصول");
                    else if (arabic && key.Contains("BETTER HEALTH"))
                        parent.Text = Arabic["PROFESSIONAL CARE | BETTER HEALTH | BRIGHTER TOMORROW"];
                    else
                        parent.Text = english;
                }

                if (parent is Label || parent is Button || parent is CheckBox)
                {
                    if (original.Alignment.HasValue)
                    {
                        var a = arabic ? MirrorAlignment(original.Alignment.Value) : original.Alignment.Value;
                        if (parent is Label) ((Label)parent).TextAlign = a;
                        else if (parent is Button) ((Button)parent).TextAlign = a;
                        else ((CheckBox)parent).TextAlign = a;
                    }
                }
                // Text fields with IDs, emails, dates and passwords must not be reversed.
                if (parent is TextBoxBase || parent is ComboBox || parent is DateTimePicker)
                    parent.RightToLeft = original.Direction;
            }
            foreach (Control child in parent.Controls) ApplyText(child, arabic);
        }

        public static void Forget(Control root)
        {
            if (root == null) return;
            foreach (Control child in root.Controls) Forget(child);
            Originals.Remove(root);
        }
    }
}
