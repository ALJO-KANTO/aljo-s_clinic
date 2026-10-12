using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.ControlsMain
{
    public partial class ucPrescriptionItems : UserControl
    {
        public ucPrescriptionItems()
        {
            InitializeComponent();
        }
        private void SetupChildGridDesign()
        {
            dgvDetailsPopup.AutoGenerateColumns = false;
            dgvDetailsPopup.Columns.Clear();
            dgvDetailsPopup.Columns.Add(new DataGridViewTextBoxColumn { Name = "MedicineName", HeaderText = "Medicine Name", DataPropertyName = "MedicineName" });
            dgvDetailsPopup.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dosage", HeaderText = "Dosage", DataPropertyName = "Dosage" });
            dgvDetailsPopup.Columns.Add(new DataGridViewTextBoxColumn { Name = "Frequency", HeaderText = "Frequency", DataPropertyName = "Frequency" });
            dgvDetailsPopup.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Quantity", DataPropertyName = "Quantity" });
            dgvDetailsPopup.Columns.Add(new DataGridViewTextBoxColumn { Name = "Instructions", HeaderText = "Instructions", DataPropertyName = "Instructions" });
            dgvDetailsPopup.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Price", DataPropertyName = "Price" });
            dgvDetailsPopup.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiscountAmount", HeaderText = "Discount", DataPropertyName = "DiscountAmount" });

            //dgvDetailsPopup.DataSource = dtMaster;




        }

        public void LoadDetails(int prescriptionID)
        {
            // استخدام الطبقة المسؤولة عن البيانات
            dgvDetailsPopup.DataSource = clsPrescription.GetPrescriptionItemsRaw(prescriptionID);
        }
       
        private void dgvDetailsPopup_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
