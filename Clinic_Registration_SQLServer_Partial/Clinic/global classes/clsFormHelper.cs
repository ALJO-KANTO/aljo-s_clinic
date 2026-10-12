using System;
using System.Windows.Forms;

public static class clsFormHelper
{
    /// <summary>
    /// 1️⃣ الحالة الأولى: فتح الشاشات التي لا تستقبل قيم (Constructor فارغ)
    /// </summary>
    public static void ShowForm<T>() where T : Form, new()
    {
        // البحث عن الشاشة المفتوحة بالاسم داخل قائمة الـ OpenForms
        T frm = (T)Application.OpenForms[typeof(T).Name];

        if (frm != null)
        {
            // إذا كانت مفتوحة مسبقاً، قم بتركيز العين عليها وإظهارها
            frm.BringToFront();
            frm.WindowState = FormWindowState.Normal;
            frm.Focus();
        }
        else
        {
            // إذا لم تكن مفتوحة، قم بإنشائها وفتحها لأول مرة
            frm = new T();
            frm.Show();
        }
    }

    /// <summary>
    /// 2️⃣ الحالة الثانية: فتح الشاشات التي تستقبل قيم (معاملات بداخل الـ Constructor)
    /// </summary>
    /// 
        // البحث عن الشاشة المفتوحة بالاسم
        public static void ShowForm<T>(Func<T> formFactory, Action onFormClosed = null) where T : Form
    {
        T frm = (T)Application.OpenForms[typeof(T).Name];

        if (frm != null)
        {
            frm.BringToFront();
            frm.WindowState = FormWindowState.Normal;
            frm.Focus();
        }
        else
        {
            frm = formFactory();

            // 🎯 إذا قمنا بتمرير دالة ليتم استدعاؤها عند الإغلاق
            if (onFormClosed != null)
            {
                frm.FormClosed += (sender, e) => onFormClosed();
            }

            frm.Show();
        }
    }
}
