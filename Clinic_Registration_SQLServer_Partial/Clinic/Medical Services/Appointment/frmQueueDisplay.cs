using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using static Clinic_Business.clsDoctor;

namespace Clinic.Medical_Services.Appointment
{
    public partial class frmQueueDisplay : Form
    {
        private List<DoctorInfo> _DoctorsList; // القائمة التي ستمررها من الشاشة السابقة
        private int _currentIndex = 0;
        private int _CurrentDoctorID = -1;


        private System.Windows.Forms.Timer _timerCarousel; // التايمر الخاص بالتبديل

        public frmQueueDisplay(List<DoctorInfo> doctors)
        {
            InitializeComponent();
            _DoctorsList = doctors;

            // إعداد التايمر الخاص بالتبديل بين الأطباء (30 ثانية)
            _timerCarousel = new System.Windows.Forms.Timer();
            _timerCarousel.Interval = 30000;
            _timerCarousel.Tick += _TimerCarousel_Tick;
        }

        private void frmQueueDisplay_Load(object sender, EventArgs e)
        {
            if (_DoctorsList != null && _DoctorsList.Count > 0)
            {
                _LoadQueueForCurrentDoctor();
                timer1.Start();
                _timerCarousel.Start(); // بدء التبديل التلقائي
            }
        }

        private void _TimerCarousel_Tick(object sender, EventArgs e)
        {
            _currentIndex++;
            if (_currentIndex >= _DoctorsList.Count) _currentIndex = 0; // العودة للبداية

            _LoadQueueForCurrentDoctor();
        }

        private void _LoadQueueForCurrentDoctor()
        {
            var currentDoc = _DoctorsList[_currentIndex];

            // تحديث العنوان
            lblHeader.Text = $"Doctor Queue: {currentDoc.DoctorName}";
            // تحديث بيانات الـ ID الحالي ليستخدمه RefreshQueueList
            _CurrentDoctorID = currentDoc.DoctorID;

            _RefreshQueueList();
        }

        private void _RefreshQueueList()
        {
            // استدعاء الدالة التي تجلب المرضى المنتظرين فقط لهذا الطبيب
            DataTable dtQueue = clsAppointment.GetWaitingPatients(_CurrentDoctorID);
            dgvQueue.DataSource = dtQueue;
            if (dgvQueue.Columns["StatusText"] != null)
                dgvQueue.Columns["StatusText"].ReadOnly = true;

            if (dtQueue.Rows.Count > 0)
            {
                _SetupDataGridViewColumns();
            }
        }

        private void _SetupDataGridViewColumns()
        {
            // تنسيق الأعمدة لتناسب استعلام المرضى (PatientName, CheckInTime)
            if (dgvQueue.Columns["PatientName"] != null)
            {
                dgvQueue.Columns["PatientName"].HeaderText = "اسم المريض";
                dgvQueue.Columns["PatientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

          

            if (dgvQueue.Columns["CheckInTime"] != null)
            {
                dgvQueue.Columns["CheckInTime"].HeaderText = "وقت الحضور";
                dgvQueue.Columns["CheckInTime"].DefaultCellStyle.Format = "hh:mm tt";
                dgvQueue.Columns["CheckInTime"].Width = 200;
            }

            if (dgvQueue.Columns["StatusText"] != null)
            {
                dgvQueue.Columns["StatusText"].HeaderText = "الحالة";
                dgvQueue.Columns["StatusText"].Width = 200;
            }
            // إخفاء أي أعمدة تقنية غير ضرورية للطبيب
            if (dgvQueue.Columns["AppointmentID"] != null)
                dgvQueue.Columns["AppointmentID"].Visible = false;
            dgvQueue.Columns["CallType"].Visible = false;
            dgvQueue.Columns["IsCalled"].Visible=false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            _timerCarousel.Stop();
            _timerCarousel.Dispose();
            timer1.Stop();
            timer1.Dispose();
        }

        private void dgvQueue_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn column in dgvQueue.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbDate.Text = DateTime.Now.ToString();
        }

        private void dgvQueue_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 1. تحقق من صحة الصف والعمود
            if (e.RowIndex < 0 || e.RowIndex >= dgvQueue.Rows.Count) return;

            // 2. استخراج القيم من الصف الحالي
            var row = dgvQueue.Rows[e.RowIndex];

            // تأكد من عدم وجود null
            object callTypeObj = row.Cells["CallType"].Value;
            object isCalledObj = row.Cells["IsCalled"].Value;

            if (callTypeObj == null || isCalledObj == null) return;

            string callType = callTypeObj.ToString();
            bool isCalled = Convert.ToBoolean(isCalledObj);

            // 3. منطق تغيير الألوان (يتم تنفيذه لكل خلية في الصف)
            if (isCalled)
            {
                if (callType == "1")
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (callType == "2")
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }
           
            // 4. منطق تغيير النص (يتم تنفيذه فقط لعمود StatusText)
            if (dgvQueue.Columns[e.ColumnIndex].Name == "StatusText")
            {
                if (isCalled)
                {
                    if (callType == "1") e.Value = "يرجى التوجه لغرفة القياسات";
                    else if (callType == "2") e.Value = "يرجى التوجه لغرفة الطبيب";

                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvQueue_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
    }
}