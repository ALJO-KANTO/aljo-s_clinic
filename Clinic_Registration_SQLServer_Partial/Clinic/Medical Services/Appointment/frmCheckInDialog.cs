using Clinic_Business;
using System;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Appointment
{
    public partial class frmCheckInDialog : Form
    {
        private int _AppointmentID;
        private decimal _Fees;

        public string SelectedPaymentMethod => cbPaymentMethod.SelectedItem?.ToString();

        public frmCheckInDialog(int appointmentID, decimal fees, string patientName)
        {
            InitializeComponent();
            _AppointmentID = appointmentID;
            _Fees = fees;

            lblPatientInfo.Text = $"Patient Name: {patientName}";
            lblFees.Text = $"Consultation Fees: {_Fees:C}"; // التنسيق كعملة
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // التحقق من أن الموظف اختار طريقة دفع
            if (cbPaymentMethod.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a payment method before confirming.",
                                "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None; // تمنع النافذة من الإغلاق
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmCheckInDialog_Load(object sender, EventArgs e)
        {
            // إعدادات أولية عند تحميل النافذة
            cbPaymentMethod.SelectedIndex = 0; // اختيار "Cash" كافتراضي

           
        }


    }
}