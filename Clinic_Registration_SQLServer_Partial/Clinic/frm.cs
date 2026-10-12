using Clinic.Appointment.AppointmentsType;
using Clinic.ControlsMain;
using Clinic.Medical_Services.Appointment;
using Clinic.Properties;
using Clinic.Serveces.ServecesType;
using Clinic.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{

    public partial class frm : Form
    {
        private bool _isInitialized = false;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_SETREDRAW = 0x000B;

        public frm()
        {
            InitializeComponent();

           

        }
        private void OpenForm(Form form)
        {
            form.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // تعريف الأدوار (يفضل وضعه في ملف منفصل)

        private void LoadUserStation(int roleId)
        {
            // 1. إيقاف الرسم فوراً
            SendMessage(pnlMainContainer.Handle, WM_SETREDRAW, 0, 0);
            pnlMainContainer.Controls.Clear();

            UserControl station = CreateStation(roleId);
            if (station == null) return;

            station.Visible = false;

            if (roleId == (int)clsGlobal.UserRole.Admin)
            {
                station.Dock = DockStyle.Fill;
                // استدعاء التحميل (الآن هو متزامن وسينتهي قبل إكمال الكود)
                ((ucAdminStation)station).LoadAdminStations();
                ((ucAdminStation)station).ExpandAll();
            }
            else
            {
                station.Dock = DockStyle.None;
                CenterControl(station);
            }

            pnlMainContainer.Controls.Add(station);

            // 2. تفعيل الرسم فقط بعد إضافة كل شيء
            station.Visible = true;
            SendMessage(pnlMainContainer.Handle, WM_SETREDRAW, 1, 0);

            // 3. إجبار الـ UI على الرسم
            pnlMainContainer.Refresh();
        }
        // دالة منفصلة لإنشاء الكائنات (Factory Pattern)
        private UserControl CreateStation(int roleId)
        {
            switch ((clsGlobal.UserRole)roleId)
            {
                case clsGlobal.UserRole.Admin:
                    return new ucAdminStation();
                case clsGlobal.UserRole.Doctor:
                    return new ucDoctorStation();
                case clsGlobal.UserRole.Receptionist:
                    return new ucReceptionistStation();
                case clsGlobal.UserRole.Nurse:
                    return new ucNurseStation();
                case clsGlobal.UserRole.Cashier:
                    return new ucCasherStation();
                case clsGlobal.UserRole.Pharmacist:
                    return new ucPharmacistStation();
                default:
                    return null;
            }
        }

        private void CenterControl(Control ctrl)
        {
            ctrl.Location = new Point(
                (pnlMainContainer.Width - ctrl.Width) / 2,
                (pnlMainContainer.Height - ctrl.Height) / 2
            );
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // تأكد أن التحميل يتم مرة واحدة فقط
            if (!_isInitialized)
            {
                // تجميد كامل للفورم
                SendMessage(this.Handle, WM_SETREDRAW, 0, 0);

                LoadUserStation(clsGlobal.CurrentUser.RoleID);

                // استئناف الرسم وإجبار الفورم على التحديث
                SendMessage(this.Handle, WM_SETREDRAW, 1, 0);
                this.Refresh();

                _isInitialized = true;
            }
        }

        private void treeAccountSettings_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Node.Text)
            {
                case "Change Password":
                   OpenForm(new frmChangePassword(clsGlobal.CurrentUser.UserID));
                    break;

                case "Current User Info":
                    OpenForm(new frmUserInfo(clsGlobal.CurrentUser.UserID));
                    break;


                default:
                    break;
            }
        }

        private void treeAccountSettings_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
