using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;

namespace Attendance_Monitoring_System
{
    public partial class UserControlLeave : UserControl
    {
        public UserControlLeave()
        {
            InitializeComponent();
        }

        private void UserControlLeave_Load(object sender, EventArgs e)
        {
            LoadDepartments();
            LoadLeaveTypes();
            LoadLeaveRequests();
        }

        private void LoadLeaveRequests()
        {
            string query = @"
        SELECT lr.ID, 
               CONCAT(e.first_name,' ',e.last_name) AS employee_name,
               d.description AS department,
               lt.Type AS leave_type,
               lr.start_date, 
               lr.end_date, 
               lr.days,
               lr.status, 
               IFNULL(CONCAT(a.first_name,' ',a.last_name), 'Pending') AS approver_name,
               lr.date_filed, 
               lr.reason
        FROM leave_request lr
        JOIN employees e ON lr.emp_id = e.ID
        LEFT JOIN departments d ON lr.department_id = d.ID
        JOIN leavetype lt ON lr.leave_type_id = lt.ID
        LEFT JOIN employees a ON lr.approver_id = a.ID
        ORDER BY lr.date_filed DESC";

            dgvLeave.DataSource = MySQL.Pull(query);
            SetupLeaveDataGridView();
        }

        private void SetupLeaveDataGridView()
        {
            dgvLeave.AutoGenerateColumns = false;
            dgvLeave.Columns.Clear();

            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID", HeaderText = "ID" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employee_name", HeaderText = "Employee" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "department", HeaderText = "Department" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "leave_type", HeaderText = "Leave Type" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "start_date", HeaderText = "Start" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "end_date", HeaderText = "End" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "days", HeaderText = "Days" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "reason", HeaderText = "Reason" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "status", HeaderText = "Status" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "approver_name", HeaderText = "Approver" });
            dgvLeave.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "date_filed", HeaderText = "Date Filed" });

            dgvLeave.EnableHeadersVisualStyles = false;
            dgvLeave.BackgroundColor = Color.WhiteSmoke;
            dgvLeave.BorderStyle = BorderStyle.None;
            dgvLeave.GridColor = Color.FromArgb(200, 50, 50);
            dgvLeave.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvLeave.RowTemplate.Height = 50;

            dgvLeave.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60);
            dgvLeave.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLeave.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvLeave.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLeave.ColumnHeadersHeight = 50;
            dgvLeave.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvLeave.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvLeave.DefaultCellStyle.BackColor = Color.White;
            dgvLeave.DefaultCellStyle.ForeColor = Color.Black;
            dgvLeave.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 34, 34);
            dgvLeave.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvLeave.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            dgvLeave.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLeave.MultiSelect = false;
            dgvLeave.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadDepartments()
        {
            string query = "SELECT ID, description FROM departments ORDER BY description";
            DataTable dt = MySQL.Pull(query);

            cb_dept.DataSource = dt;
            cb_dept.DisplayMember = "description";
            cb_dept.ValueMember = "ID";
            cb_dept.SelectedIndex = -1;

            cb_dept.SelectedIndexChanged += (s, e) => FilterLeaveRequests();
        }

        private void LoadLeaveTypes()
        {
            var dt = MySQL.Pull("SELECT ID, Type FROM lasam_attendance.leavetype");
            cbtype.DataSource = dt;
            cbtype.DisplayMember = "Type";
            cbtype.ValueMember = "ID";
            cbtype.SelectedIndex = -1;

            cbtype.SelectedIndexChanged += (s, e) => FilterLeaveRequests();
        }

        private void FilterLeaveRequests()
        {
            string query = @"
        SELECT lr.ID, 
               CONCAT(e.first_name,' ',e.last_name) AS employee_name,
               d.description AS department,
               lt.Type AS leave_type,
               lr.start_date, 
               lr.end_date, 
               lr.days,
               lr.status, 
               IFNULL(CONCAT(a.first_name,' ',a.last_name), 'Pending') AS approver_name,
               lr.date_filed, 
               lr.reason
        FROM leave_request lr
        JOIN employees e ON lr.emp_id = e.ID
        LEFT JOIN departments d ON lr.department_id = d.ID
        JOIN leavetype lt ON lr.leave_type_id = lt.ID
        LEFT JOIN employees a ON lr.approver_id = a.ID
        WHERE 1=1";

            if (cb_dept.SelectedValue != null && cb_dept.SelectedIndex >= 0)
            {
                query += " AND lr.department_id = " + cb_dept.SelectedValue;
            }

            if (cbtype.SelectedValue != null && cbtype.SelectedIndex >= 0)
            {
                query += " AND lr.leave_type_id = " + cbtype.SelectedValue;
            }

            query += " ORDER BY lr.date_filed DESC";

            dgvLeave.DataSource = MySQL.Pull(query);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            cb_dept.SelectedIndex = -1;
            cbtype.SelectedIndex = -1;
            LoadLeaveRequests();
        }

        private void btn_addleave_Click(object sender, EventArgs e)
        {
            Leavereq leaveForm = new Leavereq();
            leaveForm.ShowDialog();
        }
    }
}
