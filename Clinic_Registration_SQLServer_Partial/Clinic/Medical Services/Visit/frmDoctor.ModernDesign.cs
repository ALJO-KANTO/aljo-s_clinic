using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Clinic.Medical_Services.Visit
{
    public partial class frmDoctor
    {
        private readonly Color mdTeal = Color.FromArgb(0, 112, 118);
        private readonly Color mdDark = Color.FromArgb(4, 63, 72);
        private readonly Color mdBg = Color.FromArgb(240, 247, 250);
        private readonly Color mdText = Color.FromArgb(25, 48, 68);
        private TableLayoutPanel mdShell;
        private TableLayoutPanel mdContent;
        private Panel mdPatientCard;
        private Panel mdQueueHeader;
        private TextBox mdQueueSearch;
        private CheckBox mdChkToday;
        private DateTimePicker mdQueueDate;
        private Panel mdDateFilterPanel;
        private DataGridView mdQueueSource;
        private string mdSearchText = "";
        private Label MdLabel(string text, int size = 10, bool bold = false)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = mdText,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                Margin = new Padding(4, 7, 4, 4)
            };
        }

        private void MdButton(Button b, Color color)
        {
            b.BackColor = color;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.Image = null;
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.Dock = DockStyle.Fill;
            b.Margin = new Padding(4, 8, 4, 8);
        }

        private void MdGrid(DataGridView g)
        {
            g.Dock = DockStyle.Fill;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.RowHeadersVisible = false;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = mdTeal;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            g.ColumnHeadersHeight = 43;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.RowTemplate.Height = 43;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 245, 244);
            g.DefaultCellStyle.SelectionForeColor = mdText;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 250, 251);
            g.GridColor = Color.FromArgb(230, 238, 241);
        }

        private TableLayoutPanel MdTable(int columns, int rows)
        {
            var t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = columns,
                RowCount = rows,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            return t;
        }

        private void MdInfoCell(TableLayoutPanel t, int col, int row, string caption, Control value)
        {
            var p = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(246, 250, 252),
                Margin = new Padding(5),
                Padding = new Padding(12, 6, 8, 4)
            };
            var inner = MdTable(1, 2);
            inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            inner.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var title = MdLabel(caption, 9);
            title.ForeColor = Color.FromArgb(94, 114, 132);
            title.Dock = DockStyle.Fill;
            value.Dock = DockStyle.Fill;
            value.Margin = new Padding(4, 0, 0, 0);
            value.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            value.ForeColor = mdText;
            inner.Controls.Add(title, 0, 0);
            inner.Controls.Add(value, 0, 1);
            p.Controls.Add(inner);
            t.Controls.Add(p, col, row);
        }
        public void ApplyModernDoctorDesign()
        {
            SuspendLayout();
            // Reuse original controls and events. Do not run InitializeComponent again.
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = mdBg;
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(1100, 700);
            Padding = Padding.Empty;

            panTitle.Parent = null;
            gpPatientInfo.Parent = null;
            panel3.Parent = null;
            Controls.Clear();

            // Main page: header, workspace, footer
            mdShell = MdTable(1, 3);
            mdShell.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            mdShell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mdShell.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
            Controls.Add(mdShell);

            panTitle.Dock = DockStyle.Fill;
            panTitle.BackColor = mdDark;
            panTitle.Margin = Padding.Empty;
            lblTitle.Text = "Add New Visit";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Location = new Point(330, 18);
            lblTitle.AutoSize = true;
            label24.Text = "Modern Clinic  •  Doctor Station";
            label24.ForeColor = Color.FromArgb(204, 239, 241);
            label24.Font = new Font("Segoe UI", 11F);
            label24.AutoSize = true;
            label24.Location = new Point(18, 28);
            lbDate.ForeColor = Color.White;
            lbDate.Font = new Font("Segoe UI", 10F);
            lbDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbDate.Location = new Point(Width - 330, 30);
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.Location = new Point(Width - 55, 18);
            panTitle.Resize += (s, e) =>
            {
                lbDate.Left = Math.Max(650, panTitle.ClientSize.Width - 335);
                btnExit.Left = panTitle.ClientSize.Width - btnExit.Width - 15;
            };
            mdShell.Controls.Add(panTitle, 0, 0);

            mdContent = MdTable(2, 1);
            mdContent.Padding = new Padding(12, 14, 12, 6);
            mdContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            mdContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            mdShell.Controls.Add(mdContent, 0, 1);

            // Queue card
            pnlWaittingList.Parent = null;
            pnlWaittingList.Dock = DockStyle.Fill;
            pnlWaittingList.Padding = new Padding(10);
            pnlWaittingList.BackColor = Color.White;
            pnlWaittingList.Margin = new Padding(0, 0, 12, 0);
            mdContent.Controls.Add(pnlWaittingList, 0, 0);
            panel2.Height = 60;
            panel2.Dock = DockStyle.Top;
            panel2.BackColor = Color.White;
            label2.Text = "Patient Waiting List";
            label2.ForeColor = mdText;
            label2.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label2.AutoSize = true;
            label2.Location = new Point(12, 12);
            btnRefresh.Text = "↻";
            btnRefresh.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            btnRefresh.Image = null;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.BackColor = mdBg;
            btnRefresh.ForeColor = mdTeal;
            btnRefresh.Size = new Size(44, 40);
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel2.Resize += (s, e) => btnRefresh.Left = panel2.Width - btnRefresh.Width - 8;
            panel1.Height = 40;
            panel1.BackColor = Color.White;
            MdGrid(dgvDoctorQueue);
            // Search is intentionally a UI-only filter; don't assume unknown queue column names.
            mdQueueHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 53,
                BackColor = Color.White,
                Padding = new Padding(5, 5, 5, 8)
            };
            mdQueueSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            mdQueueHeader.Controls.Add(mdQueueSearch);
            pnlWaittingList.Controls.Add(mdQueueHeader);
            mdQueueHeader.BringToFront();
            panel2.BringToFront();
            mdQueueSearch.TextChanged += (s, e) => MdFilterQueue();

            // Date filter panel
            mdDateFilterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.White,
                Padding = new Padding(5, 5, 5, 5)
            };

            var dateLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty
            };

            dateLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 90));

            dateLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            mdChkToday = new CheckBox
            {
                Text = "Today",
                Checked = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                ForeColor = mdText,
                AutoSize = true
            };

            mdQueueDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Today,
                Enabled = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(4, 5, 4, 5)
            };

            dateLayout.Controls.Add(mdChkToday, 0, 0);
            dateLayout.Controls.Add(mdQueueDate, 1, 0);

            mdDateFilterPanel.Controls.Add(dateLayout);
            pnlWaittingList.Controls.Add(mdDateFilterPanel);

            // Display order: Search -> Date -> Waiting List
            mdDateFilterPanel.BringToFront();
            mdQueueHeader.BringToFront();
            panel2.BringToFront();

            // Events
            mdChkToday.CheckedChanged += (s, e) =>
            {
                mdQueueDate.Enabled = !mdChkToday.Checked;

                if (mdChkToday.Checked)
                    mdQueueDate.Value = DateTime.Today;

                MdFilterQueue();
            };

            mdQueueDate.ValueChanged += (s, e) =>
            {
                if (!mdChkToday.Checked)
                    MdFilterQueue();
            };

            // Right workspace: patient card and visit tabs
            var right = MdTable(1, 2);
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mdContent.Controls.Add(right, 1, 0);

            gpPatientInfo.Parent = null;
            gpPatientInfo.Dock = DockStyle.Fill;
            gpPatientInfo.Margin = new Padding(0, 0, 0, 12);
            gpPatientInfo.BackColor = Color.White;
            gpPatientInfo.ForeColor = mdText;
            gpPatientInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            gpPatientInfo.Text = "  Patient Information";
            gpPatientInfo.Controls.Clear();
            var info = MdTable(4, 2);
            info.Dock = DockStyle.Fill;
            info.Padding = new Padding(8, 10, 8, 5);
            for (int i = 0; i < 4; i++) info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            info.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            info.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            MdInfoCell(info, 0, 0, "Visit ID", lblVisitID);
            MdInfoCell(info, 1, 0, "Appointment ID", lblAppointmentID);
            MdInfoCell(info, 2, 0, "Patient ID", lblPatientID);
            MdInfoCell(info, 3, 0, "Doctor ID", lblDoctorID);
            llPatientInfo.Dock = DockStyle.Fill;
            llPatientInfo.Text = "View Patient Details  →";
            llPatientInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            llDoctorInfo.Dock = DockStyle.Fill;
            llDoctorInfo.Text = "View Doctor Details  →";
            llDoctorInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            info.Controls.Add(llPatientInfo, 2, 1);
            info.Controls.Add(llDoctorInfo, 3, 1);
            gpPatientInfo.Controls.Add(info);
            right.Controls.Add(gpPatientInfo, 0, 0);

            tcVisitInfo.Parent = null;
            tcVisitInfo.Dock = DockStyle.Fill;
            tcVisitInfo.Margin = Padding.Empty;
            tcVisitInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            tcVisitInfo.SizeMode = TabSizeMode.Fixed;
            tcVisitInfo.ItemSize = new Size(170, 37);
            tpVisitInfo.Text = "Visit Information";
            tpServices.Text = "Services";
            tpPrescriptionInfo.Text = "Prescription";
            foreach (TabPage page in tcVisitInfo.TabPages) page.BackColor = Color.White;
            right.Controls.Add(tcVisitInfo, 0, 1);

            // Visit tab: two-column responsive arrangement, no obsolete overlapping banner
            tpVisitInfo.Controls.Clear();
            var visitBody = MdTable(2, 2);
            visitBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            visitBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            visitBody.Padding = new Padding(14);
            visitBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            visitBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            pnlVisitInfo.Controls.Clear();
            pnlVisitInfo.Dock = DockStyle.Fill;
            pnlVisitInfo.Controls.Add(visitBody);
            tpVisitInfo.Controls.Add(pnlVisitInfo);
            var visitFields = MdTable(1, 5);
            visitFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            visitFields.Controls.Add(MdLabel("Visit Details", 14, true), 0, 0);
            label12.Text = "Diagnosis";
            label12.Dock = DockStyle.Fill;
            label12.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            visitFields.Controls.Add(label12, 0, 1);
            txtDiagnosis.Multiline = true;
            txtDiagnosis.Dock = DockStyle.Fill;
            txtDiagnosis.Margin = new Padding(4, 0, 14, 12);
            txtDiagnosis.Font = new Font("Segoe UI", 10F);
            visitFields.Controls.Add(txtDiagnosis, 0, 2);
            label4.Text = "Visit Notes";
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            visitFields.Controls.Add(label4, 0, 3);
            txtNotes.Multiline = true;
            txtNotes.Dock = DockStyle.Fill;
            txtNotes.Margin = new Padding(4, 0, 14, 8);
            txtNotes.Font = new Font("Segoe UI", 10F);
            visitFields.Controls.Add(txtNotes, 0, 4);
            visitBody.Controls.Add(visitFields, 0, 0);

            var vitalsArea = MdTable(1, 2);
            vitalsArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            vitalsArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            vitalsArea.Controls.Add(MdLabel("Vitals Information", 14, true), 0, 0);
            var vitals = MdTable(2, 2);
            vitals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            vitals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            vitals.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            vitals.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            MdInfoCell(vitals, 0, 0, "Pulse", lbPluse);
            MdInfoCell(vitals, 1, 0, "Temperature", lblTemp);
            MdInfoCell(vitals, 0, 1, "Weight", lbWeight);
            MdInfoCell(vitals, 1, 1, "Blood Pressure", lbBP);
            vitalsArea.Controls.Add(vitals, 0, 1);
            visitBody.Controls.Add(vitalsArea, 1, 0);
            // Keep the original Visit Information Next button and Click handler.
            var nextPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnSaveVisit.Parent = null;
            btnSaveVisit.Visible = true;
            btnSaveVisit.Dock = DockStyle.None;
            btnSaveVisit.Size = new Size(150, 42);
            btnSaveVisit.Text = "Next →";
            btnSaveVisit.Image = null;
            btnSaveVisit.BackColor = mdTeal;
            btnSaveVisit.ForeColor = Color.White;
            nextPanel.Controls.Add(btnSaveVisit);
            visitBody.Controls.Add(nextPanel, 0, 1);
            visitBody.SetColumnSpan(nextPanel, 2);

            // Services tab
            tpServices.Controls.Clear();
            var servicesBody = MdTable(1, 3);
            servicesBody.Padding = new Padding(15);
            servicesBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            servicesBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            servicesBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            pnlServicesInfo.Controls.Clear();
            pnlServicesInfo.Dock = DockStyle.Fill;
            pnlServicesInfo.Controls.Add(servicesBody);
            tpServices.Controls.Add(pnlServicesInfo);
            var serviceTop = MdTable(6, 1);
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));  // Service label
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Service selector
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135)); // New Service
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 45));  // Qty label
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));  // Qty input
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145)); // Add Service
            serviceTop.Controls.Add(MdLabel("Service", 10, true), 0, 0);
            cmbServices.Dock = DockStyle.Fill;
            cmbServices.Margin = new Padding(4, 8, 6, 8);
            serviceTop.Controls.Add(cmbServices, 1, 0);

            // Reuse the existing Designer button and its btnNewService_Click handler.
            btnNewService.Parent = null;
            MdButton(btnNewService, mdTeal);
            btnNewService.Text = "+ New";
            btnNewService.Dock = DockStyle.Fill;
            btnNewService.Margin = new Padding(4, 8, 6, 8);
            serviceTop.Controls.Add(btnNewService, 2, 0);

            serviceTop.Controls.Add(MdLabel("Qty", 10, true), 3, 0);
            nudQuality.Dock = DockStyle.Fill;
            nudQuality.Margin = new Padding(4, 8, 6, 8);
            serviceTop.Controls.Add(nudQuality, 4, 0);

            MdButton(btnAddService, mdTeal);
            btnAddService.Text = "+ Add Service";
            btnAddService.Dock = DockStyle.Fill;
            btnAddService.Margin = new Padding(4, 8, 6, 8);
            serviceTop.Controls.Add(btnAddService, 5, 0);
            servicesBody.Controls.Add(serviceTop, 0, 0);
            MdGrid(dgvServices);
            servicesBody.Controls.Add(dgvServices, 0, 1);
            var serviceBottom = MdTable(4, 1);
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            serviceBottom.Controls.Add(MdLabel("Discount", 10, true), 0, 0);
            txtDiscount.Dock = DockStyle.Fill;
            txtDiscount.Margin = new Padding(4, 12, 4, 12);
            serviceBottom.Controls.Add(txtDiscount, 1, 0);
            MdButton(btnSaveVisitServices, mdTeal);
            btnSaveVisitServices.Text = "Next →";
            serviceBottom.Controls.Add(btnSaveVisitServices, 3, 0);
            servicesBody.Controls.Add(serviceBottom, 0, 2);

            // Prescription tab
            tpPrescriptionInfo.Controls.Clear();
            var prescriptionBody = MdTable(1, 3);
            prescriptionBody.Padding = new Padding(15);
            prescriptionBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            prescriptionBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            prescriptionBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            pnlPrescriptionInfo.Controls.Clear();
            pnlPrescriptionInfo.Dock = DockStyle.Fill;
            pnlPrescriptionInfo.Controls.Add(prescriptionBody);
            tpPrescriptionInfo.Controls.Add(pnlPrescriptionInfo);
            var rxTop = MdTable(4, 1);
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15));
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            rxTop.Controls.Add(MdLabel("Prescription Date", 10, true), 0, 0);
            dtpPrescriptionDate.Dock = DockStyle.Fill;
            dtpPrescriptionDate.Margin = new Padding(4, 10, 4, 10);
            rxTop.Controls.Add(dtpPrescriptionDate, 1, 0);
            MdButton(btnAddMedicen, mdTeal);
            btnAddMedicen.Text = "+ Add Medicine";
            rxTop.Controls.Add(btnAddMedicen, 3, 0);
            prescriptionBody.Controls.Add(rxTop, 0, 0);
            MdGrid(dgvMedicines);
            prescriptionBody.Controls.Add(dgvMedicines, 0, 1);
            var rxBottom = MdTable(1, 2);
            rxBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            rxBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rxBottom.Controls.Add(MdLabel("Prescription Notes", 10, true), 0, 0);
            txtPrescriptionNotes.Multiline = true;
            txtPrescriptionNotes.Dock = DockStyle.Fill;
            txtPrescriptionNotes.Margin = new Padding(4);
            rxBottom.Controls.Add(txtPrescriptionNotes, 0, 1);
            prescriptionBody.Controls.Add(rxBottom, 0, 2);

            // Action bar; retain original Click handlers
            pnlActions.Parent = null;
            pnlActions.Controls.Clear();
            pnlActions.Dock = DockStyle.Fill;
            pnlActions.Margin = Padding.Empty;
            pnlActions.Padding = new Padding(12, 7, 12, 7);
            pnlActions.BackColor = Color.White;
            var actions = MdTable(6, 1);
            for (int i = 0; i < 6; i++) actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 6));
            MdButton(btnFullSave, mdTeal);
            MdButton(btnInProgress, Color.FromArgb(245, 155, 18));
            MdButton(btnNextPatient, Color.FromArgb(15, 116, 235));
            MdButton(btnPostpone, Color.FromArgb(121, 83, 225));
            MdButton(btnCalnceVisit, Color.FromArgb(224, 61, 78));
            MdButton(btnClose, Color.FromArgb(99, 121, 139));
            actions.Controls.Add(btnFullSave, 0, 0);
            actions.Controls.Add(btnInProgress, 1, 0);
            actions.Controls.Add(btnNextPatient, 2, 0);
            actions.Controls.Add(btnPostpone, 3, 0);
            actions.Controls.Add(btnCalnceVisit, 4, 0);
            actions.Controls.Add(btnClose, 5, 0);
            pnlActions.Controls.Add(actions);
            mdShell.Controls.Add(pnlActions, 0, 2);
            ResumeLayout(true);
        }

        public void ApplyModernDoctorDesignOld()
        {
            SuspendLayout();
            // Reuse original controls and events. Do not run InitializeComponent again.
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = mdBg;
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(1100, 700);
            Padding = Padding.Empty;

            panTitle.Parent = null;
            gpPatientInfo.Parent = null;
            panel3.Parent = null;
            Controls.Clear();

            // Main page: header, workspace, footer
            mdShell = MdTable(1, 3);
            mdShell.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            mdShell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mdShell.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
            Controls.Add(mdShell);

            panTitle.Dock = DockStyle.Fill;
            panTitle.BackColor = mdDark;
            panTitle.Margin = Padding.Empty;
            lblTitle.Text = "Add New Visit";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Location = new Point(330, 18);
            lblTitle.AutoSize = true;
            label24.Text = "Modern Clinic  •  Doctor Station";
            label24.ForeColor = Color.FromArgb(204, 239, 241);
            label24.Font = new Font("Segoe UI", 11F);
            label24.AutoSize = true;
            label24.Location = new Point(18, 28);
            lbDate.ForeColor = Color.White;
            lbDate.Font = new Font("Segoe UI", 10F);
            lbDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbDate.Location = new Point(Width - 330, 30);
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.Location = new Point(Width - 55, 18);
            panTitle.Resize += (s, e) =>
            {
                lbDate.Left = Math.Max(650, panTitle.ClientSize.Width - 335);
                btnExit.Left = panTitle.ClientSize.Width - btnExit.Width - 15;
            };
            mdShell.Controls.Add(panTitle, 0, 0);

            mdContent = MdTable(2, 1);
            mdContent.Padding = new Padding(12, 14, 12, 6);
            mdContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            mdContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            mdShell.Controls.Add(mdContent, 0, 1);

            // Queue card
            pnlWaittingList.Parent = null;
            pnlWaittingList.Dock = DockStyle.Fill;
            pnlWaittingList.Padding = new Padding(10);
            pnlWaittingList.BackColor = Color.White;
            pnlWaittingList.Margin = new Padding(0, 0, 12, 0);
            mdContent.Controls.Add(pnlWaittingList, 0, 0);
            panel2.Height = 60;
            panel2.Dock = DockStyle.Top;
            panel2.BackColor = Color.White;
            label2.Text = "Patient Waiting List";
            label2.ForeColor = mdText;
            label2.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label2.AutoSize = true;
            label2.Location = new Point(12, 12);
            btnRefresh.Text = "↻";
            btnRefresh.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            btnRefresh.Image = null;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.BackColor = mdBg;
            btnRefresh.ForeColor = mdTeal;
            btnRefresh.Size = new Size(44, 40);
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel2.Resize += (s, e) => btnRefresh.Left = panel2.Width - btnRefresh.Width - 8;
            panel1.Height = 40;
            panel1.BackColor = Color.White;
            MdGrid(dgvDoctorQueue);
            // Search is intentionally a UI-only filter; don't assume unknown queue column names.
            mdQueueHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 53,
                BackColor = Color.White,
                Padding = new Padding(5, 5, 5, 8)
            };
            mdQueueSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            mdQueueHeader.Controls.Add(mdQueueSearch);
            pnlWaittingList.Controls.Add(mdQueueHeader);
            mdQueueHeader.BringToFront();
            panel2.BringToFront();
            mdQueueSearch.TextChanged += (s, e) => MdFilterQueue();

            // Right workspace: patient card and visit tabs
            var right = MdTable(1, 2);
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mdContent.Controls.Add(right, 1, 0);

            gpPatientInfo.Parent = null;
            gpPatientInfo.Dock = DockStyle.Fill;
            gpPatientInfo.Margin = new Padding(0, 0, 0, 12);
            gpPatientInfo.BackColor = Color.White;
            gpPatientInfo.ForeColor = mdText;
            gpPatientInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            gpPatientInfo.Text = "  Patient Information";
            gpPatientInfo.Controls.Clear();
            var info = MdTable(4, 2);
            info.Dock = DockStyle.Fill;
            info.Padding = new Padding(8, 10, 8, 5);
            for (int i = 0; i < 4; i++) info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            info.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            info.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            MdInfoCell(info, 0, 0, "Visit ID", lblVisitID);
            MdInfoCell(info, 1, 0, "Appointment ID", lblAppointmentID);
            MdInfoCell(info, 2, 0, "Patient ID", lblPatientID);
            MdInfoCell(info, 3, 0, "Doctor ID", lblDoctorID);
            llPatientInfo.Dock = DockStyle.Fill;
            llPatientInfo.Text = "View Patient Details  →";
            llPatientInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            llDoctorInfo.Dock = DockStyle.Fill;
            llDoctorInfo.Text = "View Doctor Details  →";
            llDoctorInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            info.Controls.Add(llPatientInfo, 2, 1);
            info.Controls.Add(llDoctorInfo, 3, 1);
            gpPatientInfo.Controls.Add(info);
            right.Controls.Add(gpPatientInfo, 0, 0);

            tcVisitInfo.Parent = null;
            tcVisitInfo.Dock = DockStyle.Fill;
            tcVisitInfo.Margin = Padding.Empty;
            tcVisitInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            tcVisitInfo.SizeMode = TabSizeMode.Fixed;
            tcVisitInfo.ItemSize = new Size(170, 37);
            tpVisitInfo.Text = "Visit Information";
            tpServices.Text = "Services";
            tpPrescriptionInfo.Text = "Prescription";
            foreach (TabPage page in tcVisitInfo.TabPages) page.BackColor = Color.White;
            right.Controls.Add(tcVisitInfo, 0, 1);

            // Visit tab: two-column responsive arrangement, no obsolete overlapping banner
            tpVisitInfo.Controls.Clear();
            var visitBody = MdTable(2, 2);
            visitBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            visitBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            visitBody.Padding = new Padding(14);
            visitBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            visitBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            pnlVisitInfo.Controls.Clear();
            pnlVisitInfo.Dock = DockStyle.Fill;
            pnlVisitInfo.Controls.Add(visitBody);
            tpVisitInfo.Controls.Add(pnlVisitInfo);
            var visitFields = MdTable(1, 5);
            visitFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            visitFields.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            visitFields.Controls.Add(MdLabel("Visit Details", 14, true), 0, 0);
            label12.Text = "Diagnosis";
            label12.Dock = DockStyle.Fill;
            label12.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            visitFields.Controls.Add(label12, 0, 1);
            txtDiagnosis.Multiline = true;
            txtDiagnosis.Dock = DockStyle.Fill;
            txtDiagnosis.Margin = new Padding(4, 0, 14, 12);
            txtDiagnosis.Font = new Font("Segoe UI", 10F);
            visitFields.Controls.Add(txtDiagnosis, 0, 2);
            label4.Text = "Visit Notes";
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            visitFields.Controls.Add(label4, 0, 3);
            txtNotes.Multiline = true;
            txtNotes.Dock = DockStyle.Fill;
            txtNotes.Margin = new Padding(4, 0, 14, 8);
            txtNotes.Font = new Font("Segoe UI", 10F);
            visitFields.Controls.Add(txtNotes, 0, 4);
            visitBody.Controls.Add(visitFields, 0, 0);

            var vitalsArea = MdTable(1, 2);
            vitalsArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            vitalsArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            vitalsArea.Controls.Add(MdLabel("Vitals Information", 14, true), 0, 0);
            var vitals = MdTable(2, 2);
            vitals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            vitals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            vitals.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            vitals.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            MdInfoCell(vitals, 0, 0, "Pulse", lbPluse);
            MdInfoCell(vitals, 1, 0, "Temperature", lblTemp);
            MdInfoCell(vitals, 0, 1, "Weight", lbWeight);
            MdInfoCell(vitals, 1, 1, "Blood Pressure", lbBP);
            vitalsArea.Controls.Add(vitals, 0, 1);
            visitBody.Controls.Add(vitalsArea, 1, 0);
            // Keep the original Visit Information Next button and Click handler.
            var nextPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnSaveVisit.Parent = null;
            btnSaveVisit.Visible = true;
            btnSaveVisit.Dock = DockStyle.None;
            btnSaveVisit.Size = new Size(150, 42);
            btnSaveVisit.Text = "Next →";
            btnSaveVisit.Image = null;
            btnSaveVisit.BackColor = mdTeal;
            btnSaveVisit.ForeColor = Color.White;
            nextPanel.Controls.Add(btnSaveVisit);
            visitBody.Controls.Add(nextPanel, 0, 1);
            visitBody.SetColumnSpan(nextPanel, 2);

            // Services tab
            tpServices.Controls.Clear();
            var servicesBody = MdTable(1, 3);
            servicesBody.Padding = new Padding(15);
            servicesBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            servicesBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            servicesBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            pnlServicesInfo.Controls.Clear();
            pnlServicesInfo.Dock = DockStyle.Fill;
            pnlServicesInfo.Controls.Add(servicesBody);
            tpServices.Controls.Add(pnlServicesInfo);
            var serviceTop = MdTable(5, 1);
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            serviceTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            serviceTop.Controls.Add(MdLabel("Service", 10, true), 0, 0);
            cmbServices.Dock = DockStyle.Fill;
            cmbServices.Margin = new Padding(4, 8, 6, 8);
            serviceTop.Controls.Add(cmbServices, 1, 0);
            serviceTop.Controls.Add(MdLabel("Qty", 10, true), 2, 0);
            nudQuality.Dock = DockStyle.Fill;
            nudQuality.Margin = new Padding(4, 8, 6, 8);
            serviceTop.Controls.Add(nudQuality, 3, 0);
            MdButton(btnAddService, mdTeal);
            btnAddService.Text = "+ Add Service";
            serviceTop.Controls.Add(btnAddService, 4, 0);
            servicesBody.Controls.Add(serviceTop, 0, 0);
            MdGrid(dgvServices);
            servicesBody.Controls.Add(dgvServices, 0, 1);
            var serviceBottom = MdTable(4, 1);
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            serviceBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            serviceBottom.Controls.Add(MdLabel("Discount", 10, true), 0, 0);
            txtDiscount.Dock = DockStyle.Fill;
            txtDiscount.Margin = new Padding(4, 12, 4, 12);
            serviceBottom.Controls.Add(txtDiscount, 1, 0);
            MdButton(btnSaveVisitServices, mdTeal);
            btnSaveVisitServices.Text = "Next →";
            serviceBottom.Controls.Add(btnSaveVisitServices, 3, 0);
            servicesBody.Controls.Add(serviceBottom, 0, 2);

            // Prescription tab
            tpPrescriptionInfo.Controls.Clear();
            var prescriptionBody = MdTable(1, 3);
            prescriptionBody.Padding = new Padding(15);
            prescriptionBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            prescriptionBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            prescriptionBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            pnlPrescriptionInfo.Controls.Clear();
            pnlPrescriptionInfo.Dock = DockStyle.Fill;
            pnlPrescriptionInfo.Controls.Add(prescriptionBody);
            tpPrescriptionInfo.Controls.Add(pnlPrescriptionInfo);
            var rxTop = MdTable(4, 1);
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15));
            rxTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            rxTop.Controls.Add(MdLabel("Prescription Date", 10, true), 0, 0);
            dtpPrescriptionDate.Dock = DockStyle.Fill;
            dtpPrescriptionDate.Margin = new Padding(4, 10, 4, 10);
            rxTop.Controls.Add(dtpPrescriptionDate, 1, 0);
            MdButton(btnAddMedicen, mdTeal);
            btnAddMedicen.Text = "+ Add Medicine";
            rxTop.Controls.Add(btnAddMedicen, 3, 0);
            prescriptionBody.Controls.Add(rxTop, 0, 0);
            MdGrid(dgvMedicines);
            prescriptionBody.Controls.Add(dgvMedicines, 0, 1);
            var rxBottom = MdTable(1, 2);
            rxBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            rxBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rxBottom.Controls.Add(MdLabel("Prescription Notes", 10, true), 0, 0);
            txtPrescriptionNotes.Multiline = true;
            txtPrescriptionNotes.Dock = DockStyle.Fill;
            txtPrescriptionNotes.Margin = new Padding(4);
            rxBottom.Controls.Add(txtPrescriptionNotes, 0, 1);
            prescriptionBody.Controls.Add(rxBottom, 0, 2);

            // Action bar; retain original Click handlers
            pnlActions.Parent = null;
            pnlActions.Controls.Clear();
            pnlActions.Dock = DockStyle.Fill;
            pnlActions.Margin = Padding.Empty;
            pnlActions.Padding = new Padding(12, 7, 12, 7);
            pnlActions.BackColor = Color.White;
            var actions = MdTable(6, 1);
            for (int i = 0; i < 6; i++) actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 6));
            MdButton(btnFullSave, mdTeal);
            MdButton(btnInProgress, Color.FromArgb(245, 155, 18));
            MdButton(btnNextPatient, Color.FromArgb(15, 116, 235));
            MdButton(btnPostpone, Color.FromArgb(121, 83, 225));
            MdButton(btnCalnceVisit, Color.FromArgb(224, 61, 78));
            MdButton(btnClose, Color.FromArgb(99, 121, 139));
            actions.Controls.Add(btnFullSave, 0, 0);
            actions.Controls.Add(btnInProgress, 1, 0);
            actions.Controls.Add(btnNextPatient, 2, 0);
            actions.Controls.Add(btnPostpone, 3, 0);
            actions.Controls.Add(btnCalnceVisit, 4, 0);
            actions.Controls.Add(btnClose, 5, 0);
            pnlActions.Controls.Add(actions);
            mdShell.Controls.Add(pnlActions, 0, 2);
            ResumeLayout(true);
        }


        private void MdFilterQueue()
        {
            if (mdQueueSearch == null ||
                mdQueueDate == null ||
                mdChkToday == null ||
                dgvDoctorQueue == null)
                return;

            DataView dv = dgvDoctorQueue.DataSource as DataView;

            DataTable dt = dgvDoctorQueue.DataSource as DataTable;

            if (dv == null && dt != null)
                dv = dt.DefaultView;

            if (dv == null)
                return;

            List<string> filters = new List<string>();

            // 3. Search within the filtered waiting list.
            string search = mdQueueSearch.Text.Trim()
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("*", "[*]");

            if (!string.IsNullOrWhiteSpace(search))
            {
                List<string> parts = new List<string>();

                foreach (DataColumn col in dv.Table.Columns)
                {
                    if (col.DataType == typeof(string))
                    {
                        string columnName =
                            "[" + col.ColumnName.Replace("]", "\\]") + "]";

                        parts.Add(
                            "CONVERT(" + columnName +
                            ", 'System.String') LIKE '%" +
                            search + "%'");
                    }
                }

                if (parts.Count > 0)
                {
                    filters.Add(
                        "(" + string.Join(" OR ", parts) + ")");
                }
            }

            // 4. Apply all filters together.
            dv.RowFilter = string.Join(" AND ", filters);
        }


    }
}

