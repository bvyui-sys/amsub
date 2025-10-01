using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;

namespace Attendance_Monitoring_System
{
    public partial class Leavereq : Form
    {
        public Leavereq()
        {
            InitializeComponent();
            LoadEmployeeIDs();
            LoadDepartments();
            LoadLeaveTypes();
            LoadApprovers();
            dtp_startdate.ValueChanged += dtp_ValueChanged;
            dtp_enddate.ValueChanged += dtp_ValueChanged;
        }
        private void LoadEmployeeIDs()
        {
            string query = "SELECT ID, first_name, middle_name, last_name, position FROM employees ORDER BY ID";
            DataTable dt = MySQL.Pull(query);

            cb_id.DataSource = dt;
            cb_id.DisplayMember = "ID";
            cb_id.ValueMember = "ID";
            cb_id.SelectedIndex = -1;
            cb_id.SelectedIndexChanged += cb_id_SelectedIndexChanged;
        }

        private void LoadDepartments()
        {
            string query = "SELECT ID, description FROM departments ORDER BY description";
            DataTable dt = MySQL.Pull(query);

            cb_dept.DataSource = dt;
            cb_dept.DisplayMember = "description";
            cb_dept.ValueMember = "ID";
            cb_dept.SelectedIndex = -1;
        }

        private void LoadLeaveTypes()
        {
            string query = "SELECT ID, Type FROM leavetype ORDER BY Type";
            DataTable dt = MySQL.Pull(query);

            cb_leave.DataSource = dt;
            cb_leave.DisplayMember = "Type";
            cb_leave.ValueMember = "ID";
            cb_leave.SelectedIndex = -1;
        }
        private void LoadApprovers()
        {
            string query = @"
        SELECT la.id AS approver_id, 
               CONCAT(e.first_name, ' ', e.middle_name, ' ', e.last_name) AS approver_name
        FROM leave_approver la
        INNER JOIN employees e ON la.emp_id = e.ID
        ORDER BY e.first_name;
    ";

            DataTable dt = MySQL.Pull(query);

            cb_approver.DataSource = dt;
            cb_approver.DisplayMember = "approver_name";
            cb_approver.ValueMember = "approver_id";
            cb_approver.SelectedIndex = -1;
        }

        private void cb_id_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_id.SelectedIndex != -1)
            {
                DataRowView drv = cb_id.SelectedItem as DataRowView;
                if (drv != null)
                {
                    tb_firstname.Text = drv["first_name"].ToString();
                    tb_middlename.Text = drv["middle_name"].ToString();
                    tb_lastname.Text = drv["last_name"].ToString();
                    tb_position.Text = drv["position"].ToString(); 
                }
            }
            else
            {
                tb_firstname.Text = "";
                tb_middlename.Text = "";
                tb_lastname.Text = "";
                tb_position.Text = ""; 
                
            }
        }

        private void cb_approver_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_approver.SelectedIndex != -1)
            {
                string selectedApproverId = cb_approver.SelectedValue.ToString();
                string selectedApproverName = cb_approver.Text;
                Console.WriteLine($"Selected Approver ID: {selectedApproverId}, Name: {selectedApproverName}");
            }
        }

        private void cb_dept_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_dept.SelectedIndex != -1)
            {
                string selectedDeptId = cb_dept.SelectedValue.ToString();
                string selectedDeptName = cb_dept.Text;
                Console.WriteLine($"Selected Department ID: {selectedDeptId}, Name: {selectedDeptName}");
            }
        }

        private void cb_leave_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_leave.SelectedIndex != -1)
            {
                string selectedLeaveId = cb_leave.SelectedValue.ToString();
                string selectedLeaveType = cb_leave.Text;
                Console.WriteLine($"Selected Leave ID: {selectedLeaveId}, Type: {selectedLeaveType}");
            }
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            UpdateDays();
        }

        private void UpdateDays()
        {
            TimeSpan diff = dtp_enddate.Value.Date - dtp_startdate.Value.Date;
            int days = diff.Days + 1;
            tb_days.Text = (days > 0) ? days.ToString() : "0";
        }
        private void btn_applyleave_Click(object sender, EventArgs e)
        {
            if (cb_id.SelectedIndex == -1 || cb_dept.SelectedIndex == -1 || cb_leave.SelectedIndex == -1 || string.IsNullOrEmpty(tb_reason.Text))
            {
                MessageBox.Show("Please fill all required fields.");
                return;
            }

            int empId = Convert.ToInt32(cb_id.SelectedValue);
            int deptId = Convert.ToInt32(cb_dept.SelectedValue);
            int leaveTypeId = Convert.ToInt32(cb_leave.SelectedValue);
            DateTime startDate = dtp_startdate.Value.Date;
            DateTime endDate = dtp_enddate.Value.Date;
            int days = int.Parse(tb_days.Text);
            int approverId = (cb_approver.SelectedIndex != -1) ? Convert.ToInt32(cb_approver.SelectedValue) : 0;
            DateTime dateFiled = dtp_datefiled.Value.Date;
            string reason = tb_reason.Text;

            string query = @"INSERT INTO leave_request
                     (emp_id, department_id, leave_type_id, start_date, end_date, days, status, approver_id, date_filed, reason)
                     VALUES (@emp, @dept, @type, @start, @end, @days, 'Pending', @approver, @filed, @reason)";

            var parameters = new Dictionary<string, object>
            {
                {"@emp", empId},
                {"@dept", deptId},
                {"@type", leaveTypeId},
                {"@start", startDate},
                {"@end", endDate},
                {"@days", days},
                {"@approver", approverId},
                {"@filed", dateFiled},
                {"@reason", reason}
            };

            MySQL.Execute(query, parameters);
            MessageBox.Show("Leave request applied successfully!");

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            cb_id.SelectedIndex = -1;
            cb_dept.SelectedIndex = -1;
            cb_leave.SelectedIndex = -1;
            cb_approver.SelectedIndex = -1;

            dtp_startdate.Value = DateTime.Now;
            dtp_enddate.Value = DateTime.Now;
            dtp_datefiled.Value = DateTime.Now;

            tb_days.Text = "";
            tb_reason.Text = "";
            tb_firstname.Text = "";
            tb_middlename.Text = "";
            tb_lastname.Text = "";
            tb_position.Text = ""; // Clear position on reset
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void cb_approver_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}