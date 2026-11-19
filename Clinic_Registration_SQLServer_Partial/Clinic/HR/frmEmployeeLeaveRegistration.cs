using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Clinic_DataAccess;

namespace YamyProject
{
    // New standalone form; no changes to existing Designer files.
    public class frmEmployeeLeaveRegistration : Form
    {
        private readonly ComboBox employee = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
        private readonly ComboBox leaveType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
        private readonly DateTimePicker start = new DateTimePicker { Format = DateTimePickerFormat.Short };
        private readonly DateTimePicker end = new DateTimePicker { Format = DateTimePickerFormat.Short };
        private readonly NumericUpDown days = new NumericUpDown { DecimalPlaces = 2, Minimum = 0.5M, Maximum = 366, Increment = 0.5M, Width = 110 };
        private readonly TextBox notes = new TextBox { Width = 300, MaxLength = 500 };
        private readonly DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private readonly Button save = new Button { Text = "Register Leave", Width = 150, Height = 34 };
        private readonly Button approve = new Button { Text = "Approve", Width = 100, Height = 34 };
        private readonly Button reject = new Button { Text = "Reject", Width = 100, Height = 34 };
        private readonly Button cancel = new Button { Text = "Cancel Leave", Width = 120, Height = 34 };
        private readonly Button refresh = new Button { Text = "Refresh", Width = 100, Height = 34 };

        public frmEmployeeLeaveRegistration()
        {
            Text = "Employee Leave Registration";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1050, 670);
            MinimumSize = new Size(780, 530);
            Font = new Font("Segoe UI", 9);
            var fields = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 160, Padding = new Padding(15), AutoScroll = true, WrapContents = true };
            AddField(fields, "Employee", employee);
            AddField(fields, "Leave Type", leaveType);
            AddField(fields, "Start Date", start);
            AddField(fields, "End Date", end);
            AddField(fields, "Duration (days)", days);
            AddField(fields, "Notes", notes);
            fields.Controls.Add(save);
            fields.Controls.Add(refresh);
            fields.Controls.Add(approve);
            fields.Controls.Add(reject);
            fields.Controls.Add(cancel);
            Controls.Add(grid);
            Controls.Add(fields);
            leaveType.Items.AddRange(new object[] { "Annual", "Sick", "Unpaid", "Maternity", "Parental", "Compassionate", "Study", "Other" });
            leaveType.SelectedIndex = 0;
            start.ValueChanged += (s, e) => CalculateCalendarDays();
            end.ValueChanged += (s, e) => CalculateCalendarDays();
            save.Click += (s, e) => SaveLeave();
            refresh.Click += (s, e) => LoadLeaves();
            approve.Click += (s, e) => ChangeStatus("Approved");
            reject.Click += (s, e) => ChangeStatus("Rejected");
            cancel.Click += (s, e) => ChangeStatus("Cancelled");
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.DataBindingComplete += (s, e) => { if (grid.Columns.Contains("DurationDays")) grid.Columns["DurationDays"].DefaultCellStyle.Format = "N2"; };
            Load += (s, e) => { CalculateCalendarDays(); LoadEmployees(); LoadLeaves(); };
        }

        private static void AddField(FlowLayoutPanel parent, string caption, Control control)
        {
            var panel = new FlowLayoutPanel { Width = Math.Max(180, control.Width + 10), Height = 66, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            panel.Controls.Add(new Label { Text = caption, AutoSize = true });
            panel.Controls.Add(control);
            parent.Controls.Add(panel);
        }

        private void CalculateCalendarDays()
        {
            if (end.Value.Date < start.Value.Date) return;
            decimal count = (decimal)(end.Value.Date - start.Value.Date).TotalDays + 1M;
            days.Value = Math.Min(days.Maximum, count);
        }

        private void LoadEmployees()
        {
            try
            {
                var dt = ClinicDatabase.ExecuteDataTable("SELECT id, CONCAT(code, ' - ', COALESCE(name,'')) AS DisplayName FROM tbl_employee WHERE state = 0 ORDER BY name");
                employee.DataSource = dt;
                employee.DisplayMember = "DisplayName";
                employee.ValueMember = "id";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Employee loading failed"); }
        }

        private void LoadLeaves()
        {
            try
            {
                grid.DataSource = ClinicDatabase.ExecuteDataTable(@"SELECT l.LeaveID, e.code AS EmployeeCode, e.name AS EmployeeName,
                    l.LeaveType, l.StartDate, l.EndDate, l.DurationDays, l.Status, l.Notes, l.CreatedAt
                    FROM tbl_employee_leave_requests l INNER JOIN tbl_employee e ON e.id=l.EmployeeID
                    ORDER BY l.LeaveID DESC");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Leave loading failed"); }
        }

        private void ChangeStatus(string newStatus)
        {
            if (grid.CurrentRow == null || grid.CurrentRow.IsNewRow)
            { MessageBox.Show("Select a leave record."); return; }
            int leaveId = Convert.ToInt32(grid.CurrentRow.Cells["LeaveID"].Value);
            string oldStatus = Convert.ToString(grid.CurrentRow.Cells["Status"].Value);
            if (oldStatus == "Cancelled" || oldStatus == "Rejected" ||
                (oldStatus == "Approved" && newStatus != "Cancelled"))
            { MessageBox.Show("This status transition is not allowed."); return; }
            if (newStatus == "Cancelled" && oldStatus != "Pending" && oldStatus != "Approved")
            { MessageBox.Show("Only pending or approved leave can be cancelled."); return; }
            if (MessageBox.Show("Change leave #" + leaveId + " to " + newStatus + "?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                using (var connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                using (var command = new MySqlCommand(@"UPDATE tbl_employee_leave_requests
                    SET Status=@newStatus WHERE LeaveID=@id AND Status=@oldStatus", connection))
                {
                    command.Parameters.AddWithValue("@newStatus", newStatus);
                    command.Parameters.AddWithValue("@id", leaveId);
                    command.Parameters.AddWithValue("@oldStatus", oldStatus);
                    connection.Open();
                    if (command.ExecuteNonQuery() != 1)
                    { MessageBox.Show("Leave status was changed by another user. Refresh."); LoadLeaves(); return; }
                }
                LoadLeaves();
                MessageBox.Show("Leave status updated. Review attendance and payroll separately before posting.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Status update failed"); }
        }

        private void SaveLeave()
        {
            if (employee.SelectedValue == null || employee.SelectedValue is DataRowView)
            { MessageBox.Show("Select an employee."); return; }
            if (end.Value.Date < start.Value.Date)
            { MessageBox.Show("End date cannot precede start date."); return; }
            try
            {
                int id = Convert.ToInt32(employee.SelectedValue);
                // Reject overlapping pending/approved requests for the same employee.
                var overlap = ClinicDatabase.ExecuteDataTable(@"SELECT LeaveID FROM tbl_employee_leave_requests
                    WHERE EmployeeID=@employee AND Status IN ('Pending','Approved')
                    AND StartDate<=@endDate AND EndDate>=@startDate LIMIT 1",
                    new MySqlParameter("@employee", id),
                    new MySqlParameter("@startDate", start.Value.Date),
                    new MySqlParameter("@endDate", end.Value.Date));
                if (overlap.Rows.Count > 0) { MessageBox.Show("Overlapping leave request exists."); return; }
                using (var connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                using (var command = new MySqlCommand(@"INSERT INTO tbl_employee_leave_requests
                    (EmployeeID,LeaveType,StartDate,EndDate,DurationDays,Status,Notes)
                    VALUES (@employee,@type,@startDate,@endDate,@days,'Pending',@notes)", connection))
                {
                    command.Parameters.AddWithValue("@employee", id);
                    command.Parameters.AddWithValue("@type", leaveType.Text);
                    command.Parameters.AddWithValue("@startDate", start.Value.Date);
                    command.Parameters.AddWithValue("@endDate", end.Value.Date);
                    command.Parameters.AddWithValue("@days", days.Value);
                    command.Parameters.AddWithValue("@notes", notes.Text.Trim());
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Leave request registered as Pending.");
                LoadLeaves();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Unable to register leave"); }
        }
    }
}
