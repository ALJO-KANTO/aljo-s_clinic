using System.Windows.Forms;

namespace Clinic.ControlsMain
{
    partial class ucPrescriptionItems
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDetailsPopup = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailsPopup)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDetailsPopup
            // 
            this.dgvDetailsPopup.AllowUserToAddRows = false;
            this.dgvDetailsPopup.AllowUserToDeleteRows = false;
            this.dgvDetailsPopup.AllowUserToResizeRows = false;
            this.dgvDetailsPopup.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetailsPopup.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(242)))), ((int)(((byte)(242))))); // خلفية ناعمة لجدول التفاصيل الداخلي
            this.dgvDetailsPopup.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetailsPopup.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(166)))), ((int)(((byte)(154))))); // درجة أخضر مائي مميزة ومختلفة عن الجدول الرئيسي للتفريق البصري
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(166)))), ((int)(((byte)(154)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetailsPopup.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDetailsPopup.ColumnHeadersHeight = 40;
            this.dgvDetailsPopup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDetailsPopup.Dock = System.Windows.Forms.DockStyle.Fill; // يملأ الـ Control بالكامل بشكل مرن ومستجيب
            this.dgvDetailsPopup.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvDetailsPopup.EnableHeadersVisualStyles = false;
            this.dgvDetailsPopup.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(225)))), ((int)(((byte)(224)))));
            this.dgvDetailsPopup.Location = new System.Drawing.Point(15, 10);
            this.dgvDetailsPopup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvDetailsPopup.MultiSelect = false;
            this.dgvDetailsPopup.Name = "dgvDetailsPopup";
            this.dgvDetailsPopup.ReadOnly = true;
            this.dgvDetailsPopup.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(232)))), ((int)(((byte)(230))))); // تحديد بلون متناسق جداً مع تفاصيل الأدوية
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(77)))), ((int)(((byte)(64)))));
            this.dgvDetailsPopup.RowsDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(248)))), ((int)(((byte)(248))))); // أسطر متبادلة ناعمة داخل التفاصيل
            this.dgvDetailsPopup.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDetailsPopup.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvDetailsPopup.RowTemplate.Height = 38;
            this.dgvDetailsPopup.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetailsPopup.Size = new System.Drawing.Size(1346, 292);
            this.dgvDetailsPopup.TabIndex = 256;
            this.dgvDetailsPopup.TabStop = false;
            // 
            // ucPrescriptionItems
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(235))))); // رمادي دافئ خفيف يعطي إيحاء الـ Nesting (الاحتواء بالداخل)
            this.Controls.Add(this.dgvDetailsPopup);
            this.Name = "ucPrescriptionItems";
            this.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10); // هوامش جانبية لكي لا يلتصق بحواف الشاشة الأصلية
            this.Size = new System.Drawing.Size(1376, 312);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailsPopup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDetailsPopup;
    }
}