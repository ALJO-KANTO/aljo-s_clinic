using Clinic.Appointment.AppointmentsType;
using Clinic.Doctor;
using Clinic.Patient;
using Clinic.Serveces.ServecesType;
using Clinic.User;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.ControlsMain
{
    public partial class ucAdminStation : UserControl
    {
        public ucAdminStation()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            EnableDoubleBuffered(treeSystemManagement);
            EnableDoubleBuffered(treeAccountSettings);
        }

        public void LoadAdminStations()
        {
            // أوقف الرسم تماماً على مستوى الكنترول
            this.panel1.SuspendLayout();
            this.panel1.Visible = false;

            // تحميل مباشر وسريع (الـ 214ms التي رأيتها هي سرعة جيدة جداً)
            var doc = new ucDoctorStation { Location = new Point(300, 6) };
            var pha = new ucCasherStation { Location = new Point(920, 6) };
            var cas = new ucPharmacistStation { Location = new Point(310, 321) };
            var rec = new ucReceptionistStation { Location = new Point(980, 321) };
            var nur = new ucNurseStation { Location = new Point(16, 512) };

            this.panel1.Controls.AddRange(new Control[] { doc, pha, cas, rec, nur });

            this.panel1.ResumeLayout(true);
            this.panel1.Visible = true;
        }
        private void EnableDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, control, new object[] { true });
        }

        public void ExpandAll()
        {
            treeSystemManagement.ExpandAll();

        }

        private void treeSystemManagement_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Node.Text)
            {
                case "Users":
                    OpenForm(new frmlistUser());
                    break;

                case "Doctors":
                    OpenForm(new frmListDoctors());
                    break;

                case "Patients":
                    OpenForm(new frmListPatient());
                    break;
                case "Peaple":
                    OpenForm(new frmListPeople());
                    break;

                default:
                    break;
            }
        }

        private void OpenForm(Form form)
        {
            form.ShowDialog();
        }

        private void treeAccountSettings_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Node.Text)
            {
                case "Mange Appoinment":
                    OpenForm(new frmListAppointmentType());
                    break;

                case "Mange Services":
                    OpenForm(new frmListServicesType());
                    break;


                default:
                    break;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
