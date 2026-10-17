namespace Clinic.Medical_Services.Radiology
{
    partial class frmXRay
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        // Keep designer initialization simple. Complex dynamic layout is built in frmXRay.cs
        // so the Visual Studio WinForms designer can open without parsing custom loops.
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 760);
            this.MinimumSize = new System.Drawing.Size(760, 520);
            this.Name = "frmXRay";
            this.Text = "X-Ray / Radiology Management";
            this.ResumeLayout(false);
        }
    }
}
