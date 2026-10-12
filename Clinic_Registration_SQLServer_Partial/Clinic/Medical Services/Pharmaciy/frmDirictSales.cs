using Clinic.Medical_Services.Visit;
using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Pharmaciy
{
    public partial class frmDirictSales : Form
    {
        private DataTable _dtAllMedicines = new DataTable();
        private clsPrescription _Prescription = new clsPrescription();
        public frmDirictSales()
        {
            InitializeComponent();
        }

        private void InitializePrescriptionsTable()
        {
            _dtAllMedicines.Columns.Clear(); // تنظيف الجدول أولاً

            _dtAllMedicines.Columns.Add("MedicineID", typeof(int));
            _dtAllMedicines.Columns.Add("Quantity", typeof(int)).DefaultValue = 1;
            _dtAllMedicines.Columns.Add("Dosage", typeof(string));
            _dtAllMedicines.Columns.Add("Frequency", typeof(int));
            _dtAllMedicines.Columns.Add("Instructions", typeof(string));
            _dtAllMedicines.Columns.Add("DiscountAmount", typeof(decimal));
            _dtAllMedicines.Columns.Add("SavedMedicineName", typeof(string));
            _dtAllMedicines.Columns.Add("SavedMedicinePrice", typeof(decimal)).DefaultValue = 0m;
            _dtAllMedicines.Columns.Add("TaxRate", typeof(decimal)).DefaultValue = 0m;


            string expression =
                       "((SavedMedicinePrice * Quantity) - (DiscountAmount * Quantity)) + " +
                       "((SavedMedicinePrice * Quantity) * (TaxRate / 100))";

            _dtAllMedicines.Columns.Add("Total", typeof(decimal), expression);
        }

        private void _SetupMedicinesGrid()
        {
            dgPrescriptionDetails.AutoGenerateColumns = false;

            if (dgPrescriptionDetails.Columns.Count > 0)
                return;

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "MedicineID", DataPropertyName = "MedicineID", Visible = false });
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "Instructions", HeaderText = "Instructions", DataPropertyName = "Instructions", Visible = false });
            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "TaxRate", HeaderText = "TaxRate", DataPropertyName = "TaxRate", Visible = false });

            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "SavedMedicineName",
                HeaderText = "Medicine Name",
                DataPropertyName = "SavedMedicineName",
                ReadOnly = true,
                Width = 220 
            };
            dgPrescriptionDetails.Columns.Add(nameColumn);

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Dosage",
                HeaderText = "Dosage",
                DataPropertyName = "Dosage",
                ReadOnly = true,
                Width = 220
            });

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SavedMedicinePrice",
                HeaderText = "Unit Price",
                DataPropertyName = "SavedMedicinePrice",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Qty",
                DataPropertyName = "Quantity",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Frequency",
                HeaderText = "FRQ",
                DataPropertyName = "Frequency",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DiscountAmount",
                HeaderText = "Discount",
                DataPropertyName = "DiscountAmount",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dgPrescriptionDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Total Price",
                DataPropertyName = "Total",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            DataGridViewLinkColumn actionColumn = new DataGridViewLinkColumn
            {
                Name = "Actions",
                HeaderText = "Actions",
                Text = "Edit  |  Delete",
                UseColumnTextForLinkValue = true, // لعرض النص الثابت على كل الأسطر
                ActiveLinkColor = Color.Blue,
                LinkColor = Color.Brown, // لون مميز ومناسب للهوية البصرية للعيادة
                TrackVisitedState = false,
                Width = 110 // مساحة ثابتة وممتازة للكلمتين معاً
            };
            // جعل النص يتوسط الخلية تماماً بشكل أنيق
            actionColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgPrescriptionDetails.Columns.Add(actionColumn);

            InitializePrescriptionsTable();

            dgPrescriptionDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; 
            dgPrescriptionDetails.RowTemplate.Height = 50; 
            dgPrescriptionDetails.DataSource = _dtAllMedicines;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddToPrescription_Click(object sender, EventArgs e)
        {
            using (frmAddUpdateMedicineToPrescription frm = new frmAddUpdateMedicineToPrescription(ref _dtAllMedicines))
            {
                frm.DataBack += _DataBack;
                frm.ShowDialog();
               
            } 

        }

        private void frmDirictSales_Load(object sender, EventArgs e)
        {
            _SetupMedicinesGrid();
        }

        private void CalculateTotalFastestWay()
        {

            decimal totalSum = _dtAllMedicines.Rows.Count > 0 ? Convert.ToDecimal(_dtAllMedicines.Compute("Sum(Total)", "")) : 0;

            lblTotalAmount.Text = "$ " + totalSum.ToString();
        }
        private void _DataBack(object sender)
        {
            dgPrescriptionDetails.DataSource = _dtAllMedicines;
            CalculateTotalFastestWay();

        }

        private void dgPrescriptionDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgPrescriptionDetails.Columns[e.ColumnIndex].Name == "Actions")
            {
                int medicineID = Convert.ToInt32(dgPrescriptionDetails.Rows[e.RowIndex].Cells["MedicineID"].Value);

                DataRow selectedRow = _dtAllMedicines.Rows[e.RowIndex];

                var cellMousePos = dgPrescriptionDetails.PointToClient(Cursor.Position);
                var cellRect = dgPrescriptionDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                int clickXInsideCell = cellMousePos.X - cellRect.X; // المسافة الأفقية للنقرة من بداية الخلية

                // نقسم الخلية افتراضياً من المنتصف: 
                // إذا كانت النقرة في النصف الأول (اليمين أو اليسار حسب لغة الواجهة) تكون Edit، وإلا Delete
                // في التصميم العادي: النصف الأول (الأيسر) هو Edit والنصف الثاني (الأيمن) هو Delete
                if (clickXInsideCell < (cellRect.Width / 2))
                {
                    // ----------------------------------------------------
                    // 📝 حدث التعديل (EDIT)
                    // ----------------------------------------------------
                    // هنا تفتح شاشة التعديل وتمرر لها السطر الحالي
                    using (frmAddUpdateMedicineToPrescription frm = new frmAddUpdateMedicineToPrescription(ref _dtAllMedicines, selectedRow))
                    {
                        frm.DataBack += _DataBack;
                        frm.ShowDialog();
                    }
                }
                else
                {
                    // ----------------------------------------------------
                    // ❌ حدث الحذف (DELETE)
                    // ----------------------------------------------------
                    DialogResult result = MessageBox.Show("Are you sure you want to remove this medicine from prescription?",
                                                            "Confirm Delete",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // حذف السطر من مصدر البيانات مباشرة
                        _dtAllMedicines.Rows[e.RowIndex].Delete();
                        _dtAllMedicines.AcceptChanges();

                        // إعادة حساب الإجمالي فوراً بعد الحذف
                        CalculateTotalFastestWay();
                    }
                }
            }
        }

        private void btnSavePrescription_Click(object sender, EventArgs e)
        {
            if (_dtAllMedicines.Rows.Count == 0)
            {
                MessageBox.Show("Cannot proceed: You must select at least one medicine to dispense.",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _Prescription.PrescriptionDate = DateTime.Now;
            _Prescription.VisitID = null;
            _Prescription.PrescriptionNotes = "";
            _Prescription.dtMedicines = _dtAllMedicines;
            _Prescription.Prescriptiontype = (byte)clsPrescription.enPrescriptionType.PharmacyDirect;

            if (_Prescription.Save())
            {
                lbPrescriptionID.Text = _Prescription.PrescriptionID.ToString();
                MessageBox.Show("Prescription saved successfully.", "✅ Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
                MessageBox.Show("Failed to save Prescription.", "❌ Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
