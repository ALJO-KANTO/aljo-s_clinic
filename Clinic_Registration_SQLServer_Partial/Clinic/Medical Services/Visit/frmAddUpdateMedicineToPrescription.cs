using Clinic_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Visit
{
    public partial class frmAddUpdateMedicineToPrescription : Form
    {
        public event Action<object> DataBack;
        public enum enMode { AddNew = 1, Update = 2, ShowVisit = 3 }
        private enMode _Mode = enMode.AddNew;
        private DataRow _Row;
        private DataTable _dtMedicines;
        private decimal _TaxRate = 0.0m;
        private decimal _price = 0.0m;
        public frmAddUpdateMedicineToPrescription(ref DataTable dtMedicines)
        {
            InitializeComponent();
            _dtMedicines = dtMedicines;
                _Mode = enMode.AddNew;
        }

        public frmAddUpdateMedicineToPrescription(ref DataTable dtMedicines, DataRow Row)
        {
            InitializeComponent();
            _Mode = enMode.Update;
            _dtMedicines = dtMedicines;
            _Row = Row;
        }

        private void _FillMedicensInCheckBox()
        {
            DataTable dtMedicen = clsMedicine.GetAllMedicines();
            if (dtMedicen != null)
            {
                cbMedicines.DataSource = dtMedicen;
                cbMedicines.DisplayMember = "MedicineName";
                cbMedicines.ValueMember = "MedicineID";
            }
        }

        private void _ResetDefaultValues()
        {
            _FillMedicensInCheckBox();
            cbMedicines.SelectedIndex = 1;
            NUDQuantity.Value = 1;
            txtDosage.Text = "";
            txtInstructions.Text = "";
            if (_Mode == enMode.AddNew)
            {

                lblTitle.Text = "Add New Medicine";


            }
            else if (_Mode == enMode.Update)
            {
                lblTitle.Text = "Update Medicine Info";

            }

            this.Text = lblTitle.Text;
        }

        private void _LoadData()
        {
            if (_Row == null)
            {
                this.Close();
                return;

            }
            cbMedicines.SelectedValue = _Row["MedicineID"];
            NUDQuantity.Value = (int)_Row["Quantity"];
            nudFrequency.Value = (int)_Row["Frequency"];
            txtDosage.Text = _Row["Dosage"].ToString();
            txtDiscount.Text = _Row["DiscountAmount"].ToString();
            txtInstructions.Text = _Row["Instructions"] == DBNull.Value || _Row["Instructions"] == null ? "" : _Row["Instructions"].ToString();

        }

        private void frmAddUpdateMedicineToPrescription_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();
            if (_Mode == enMode.Update)
                _LoadData();
        }

        private void txtDosage_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDosage.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDosage, "This Falid Is Required");

            }
            else

                errorProvider1.SetError(txtDosage, null);
        }

        private void cbMedicines_SelectedIndexChanged(object sender, EventArgs e)
        {
            // البحث عن الصف الذي يحمل الـ ID المحدد
            if (cbMedicines.DataSource == null || cbMedicines.SelectedValue == null)
                return;

            // 2. استخدم int.TryParse أو تحقق من النوع لمنع الأخطاء
            if (int.TryParse(cbMedicines.SelectedValue.ToString(), out int selectedID))
            {
                // 3. البحث باستخدام الفلتر
                DataRow[] foundRows = ((DataTable)cbMedicines.DataSource).Select($"MedicineID = {selectedID}");

                if (foundRows.Length > 0)
                {
                    _price = Convert.ToDecimal(foundRows[0]["MedicinePrice"]);
                     _TaxRate = Convert.ToDecimal(foundRows[0]["TaxRate"]);

                    decimal totalPrice = _price + (_price * (_TaxRate / 100m));
                    lblMedicinePrice.Text = totalPrice.ToString("F2");
                }
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. التحقق من الحقول الإجبارية بالشاشة
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please fill all required fields correctly.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. التأكد من اختيار الدواء وتجنب القيمة الفارغة مبكراً
            if (cbMedicines.SelectedValue == null)
            {
                MessageBox.Show("Please select a medicine first.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selectedMedicineID = (int)cbMedicines.SelectedValue;
            int requestedQuantity = (int)NUDQuantity.Value;

            // 3. حساب وتدقيق نسبة الخصم
            decimal discount = 0;
            decimal.TryParse(txtDiscount.Text, out discount);

            if (!clsDiscount.ValidateDiscount(clsGlobal.CurrentUser.RoleID, clsDiscount.enTargetType.Medicine, discount))
            {
                MessageBox.Show("Discount exceeds your allowed limit or is invalid for this role.", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. التحقق من توفر الكمية في المخزن (للصيدلاني)
            if (clsGlobal.CurrentUser.RoleID == (int)clsGlobal.UserRole.Pharmacist)
            {
                if (!clsMedicine.IsMedicineAvailable(selectedMedicineID, requestedQuantity))
                {
                    MessageBox.Show("This medicine is not available in the required quantity or is out of stock.",
                                    "⚠️ Insufficient Stock",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
            }

            // 5. مرحلة الحفظ أو التعديل
            if (_Mode == enMode.AddNew)
            {
                // التحقق من تكرار الدواء في القائمة
                if (_dtMedicines.Select($"MedicineID = {selectedMedicineID}").Length > 0)
                {
                    MessageBox.Show("This Medicine has already been added.", "⚠️ Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // إنشاء صف جديد
                DataRow newRow = _dtMedicines.NewRow();
                newRow["MedicineID"] = selectedMedicineID;
                newRow["SavedMedicineName"] = cbMedicines.Text;
                newRow["Quantity"] = requestedQuantity;
                newRow["Frequency"] = nudFrequency.Value;
                newRow["SavedMedicinePrice"] = _price;
                newRow["Dosage"] = txtDosage.Text;
                newRow["Instructions"] = txtInstructions.Text;
                newRow["DiscountAmount"] = discount;
                newRow["TaxRate"] = _TaxRate;


                _dtMedicines.Rows.Add(newRow);
            }
            else if (_Mode == enMode.Update)
            {
                // الثغرة: التحقق من التكرار عند التعديل (تأكد أنه لم يختر دواءً موجوداً مسبقاً في صف آخر)
                DataRow[] duplicateRows = _dtMedicines.Select($"MedicineID = {selectedMedicineID}");
                if (duplicateRows.Length > 0 && duplicateRows[0] != _Row)
                {
                    MessageBox.Show("This Medicine is already in the list. Cannot update to a duplicate medicine.", "⚠️ Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // تحديث الصف الحالي
                _Row["MedicineID"] = selectedMedicineID;
                _Row["SavedMedicineName"] = cbMedicines.Text;
                _Row["Quantity"] = requestedQuantity;
                _Row["Frequency"] = nudFrequency.Value; 
                _Row["SavedMedicinePrice"] = _price; 
                _Row["Dosage"] = txtDosage.Text;
                _Row["Instructions"] = txtInstructions.Text;
                _Row["DiscountAmount"] = discount;
                _Row["TaxRate"] = _TaxRate;

            }

            // 6. إرجاع البيانات وإغلاق الشاشة نجاح العملية
            DataBack?.Invoke(this);
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح فقط بالأرقام، أزرار التحكم (BackSpace)، والفاصلة
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // منع تكرار الفاصلة العشرية
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDiscount_Leave(object sender, EventArgs e)
        {
            decimal value;
            if (decimal.TryParse(txtDiscount.Text, out value))
            {
                txtDiscount.Text = value.ToString("F2");
            }
            else
            {
                // إذا كان الحقل فارغاً أو خطأ، اجعله صفر
                txtDiscount.Text = "0.00";
            }
        }
    }
}
