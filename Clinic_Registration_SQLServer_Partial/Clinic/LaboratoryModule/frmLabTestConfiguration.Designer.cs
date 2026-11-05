using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Laboratory
{
    partial class frmLabTestConfiguration
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Panel pnlActions;
        private Label lblTitle, lblTestID, lblName, lblPrice, lblCount, lblHint;
        private TextBox txtTestName, txtTestPrice;
        private DataGridView dgvParameters;
        private Button btnAddParameter, btnRemoveParameter, btnSave, btnClose;
        private DataGridViewTextBoxColumn colParameterID, colParameterName,
            colMaleRange, colFemaleRange, colUnit, colGeneralRange, colDisplayOrder;
        private DataGridViewComboBoxColumn colResultType;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Text = "Lab Test Configuration";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(950, 550);
            Size = new Size(1160, 720);
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(247, 249, 251);

            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 145, BackColor = Color.White };
            lblTitle = new Label
            {
                Text = "Lab Test Configuration",
                Location = new Point(20, 14),
                Size = new Size(490, 32),
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 119, 105)
            };
            lblTestID = new Label
            {
                Text = "Test ID:",
                Location = new Point(940, 23),
                AutoSize = true,
                ForeColor = Color.DimGray
            };
            lblName = new Label { Text = "Test Name", Location = new Point(22, 70), AutoSize = true };
            txtTestName = new TextBox
            {
                Location = new Point(120, 65),
                Size = new Size(455, 30),
                ReadOnly = true,
                BackColor = Color.FromArgb(241, 243, 249)
            };
            lblPrice = new Label { Text = "Test Price", Location = new Point(605, 70), AutoSize = true };
            txtTestPrice = new TextBox
            {
                Location = new Point(705, 65),
                Size = new Size(160, 30),
                ReadOnly = true,
                BackColor = Color.FromArgb(241, 243, 249)
            };
            lblHint = new Label
            {
                Text = "Set each parameter's reference range for male and female patients. Leave a range blank when not applicable.",
                Location = new Point(22, 112),
                Size = new Size(1050, 23),
                ForeColor = Color.DimGray
            };
            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblTestID, lblName,
                txtTestName, lblPrice, txtTestPrice, lblHint });

            pnlActions = new Panel { Dock = DockStyle.Top, Height = 64, Padding = new Padding(20, 12, 20, 10) };
            btnAddParameter = MakeButton("+ Add Parameter", 165, Color.FromArgb(0, 125, 110));
            btnAddParameter.Location = new Point(20, 10);
            btnRemoveParameter = MakeButton("Remove Selected", 170, Color.FromArgb(158, 64, 64));
            btnRemoveParameter.Location = new Point(195, 10);
            lblCount = new Label
            {
                Text = "0 parameters",
                AutoSize = true,
                Location = new Point(390, 21),
                ForeColor = Color.DimGray
            };
            pnlActions.Controls.AddRange(new Control[] { btnAddParameter, btnRemoveParameter, lblCount });

            dgvParameters = new DataGridView
            {
                Name = "dgvParameters",
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                RowTemplate = { Height = 35 },
                ColumnHeadersHeight = 42,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvParameters.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 107);
            dgvParameters.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvParameters.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvParameters.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 249, 248);

            colParameterID = TextColumn("colParameterID", "ParameterID", "ID", 35);
            colParameterID.Visible = false;
            colParameterName = TextColumn("colParameterName", "ParameterName", "Parameter", 220);
            colMaleRange = TextColumn("colMaleRange", "MaleReferenceRange", "Normal Range (Male)", 160);
            colFemaleRange = TextColumn("colFemaleRange", "FemaleReferenceRange", "Normal Range (Female)", 160);
            colUnit = TextColumn("colUnit", "Unit", "UOM", 80);
            colResultType = new DataGridViewComboBoxColumn
            {
                Name = "colResultType",
                DataPropertyName = "ResultType",
                HeaderText = "Result Value Type",
                FillWeight = 125,
                FlatStyle = FlatStyle.Flat
            };
            colResultType.Items.AddRange("Quantitative", "Qualitative", "Text");
            colGeneralRange = TextColumn("colGeneralRange", "ReferenceRange", "General Range / Remark", 160);
            colDisplayOrder = TextColumn("colDisplayOrder", "DisplayOrder", "Order", 50);
            colDisplayOrder.Visible = false;
            dgvParameters.Columns.AddRange(colParameterID, colParameterName, colMaleRange,
                colFemaleRange, colUnit, colResultType, colGeneralRange, colDisplayOrder);

            var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 72, BackColor = Color.White };
            btnSave = MakeButton("Save Configuration", 210, Color.FromArgb(0, 120, 105));
            btnSave.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnSave.Location = new Point(700, 15);
            btnClose = MakeButton("Close", 145, Color.FromArgb(91, 105, 115));
            btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnClose.Location = new Point(920, 15);
            pnlFooter.Controls.AddRange(new Control[] { btnSave, btnClose });
            Controls.Add(dgvParameters);
            Controls.Add(pnlFooter);
            Controls.Add(pnlActions);
            Controls.Add(pnlHeader);
        }

        private static Button MakeButton(string caption, int width, Color color)
        {
            return new Button
            {
                Text = caption,
                Width = width,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private static DataGridViewTextBoxColumn TextColumn(string name, string property,
            string header, float weight)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = property,
                HeaderText = header,
                FillWeight = weight,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }
    }
}
