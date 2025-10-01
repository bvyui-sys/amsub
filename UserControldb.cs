using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;

namespace Attendance_Monitoring_System
{
    public partial class UserControldb : UserControl
    {
        public UserControldb()
        {
            InitializeComponent();
        }

        private void UserControldb_Load(object sender, EventArgs e)
        {
            LoadTotalEmployees();
            LoadAttendanceData();
        }

        private void LoadTotalEmployees()
        {
            int total = MySQL.GetCount("SELECT COUNT(*) FROM employees");
            totaltoday.Text = total.ToString();
        }

        private void LoadAttendanceData(string searchText = "")
        {
            string searchParam = "%" + searchText.Replace("'", "''") + "%";

            string query = $@"
                SELECT a.ID,
                       a.employee_id,
                       e.first_name,
                       e.middle_name,
                       e.last_name,
                       a.date,
                       a.am_in,
                       a.am_out,
                       a.pm_in,
                       a.pm_out,
                       a.ot_in,
                       a.ot_out,
                       a.status
                FROM attendance a
                LEFT JOIN employees e ON a.employee_id = e.ID
                WHERE CONCAT(e.first_name, ' ', e.middle_name, ' ', e.last_name) LIKE '{searchParam}'
                   OR a.employee_id LIKE '{searchParam}'
                   OR a.status LIKE '{searchParam}'
                ORDER BY a.date DESC;
            ";

            DataTable dt = MySQL.Pull(query);
            dgvAtt.DataSource = dt;

            SetupAttendanceGrid();
        }

        private void SetupAttendanceGrid()
        {
            dgvAtt.AutoGenerateColumns = false;
            dgvAtt.Columns.Clear();

            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID", HeaderText = "Number" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employee_id", HeaderText = "Employee ID" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "first_name", HeaderText = "First Name" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "middle_name", HeaderText = "Middle Name" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "last_name", HeaderText = "Last Name" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "date", HeaderText = "Date" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "am_in", HeaderText = "AM In" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "am_out", HeaderText = "AM Out" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "pm_in", HeaderText = "PM In" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "pm_out", HeaderText = "PM Out" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ot_in", HeaderText = "OT In" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ot_out", HeaderText = "OT Out" });
            dgvAtt.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "status", HeaderText = "Status" });

            dgvAtt.AllowUserToAddRows = false;
            dgvAtt.ReadOnly = true;
            dgvAtt.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAtt.MultiSelect = false;
            dgvAtt.BorderStyle = BorderStyle.None;
            dgvAtt.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAtt.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvAtt.EnableHeadersVisualStyles = false;
            dgvAtt.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60);
            dgvAtt.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAtt.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvAtt.ColumnHeadersHeight = 45;

            dgvAtt.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAtt.DefaultCellStyle.BackColor = Color.White;
            dgvAtt.DefaultCellStyle.ForeColor = Color.Black;
            dgvAtt.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 34, 34);
            dgvAtt.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvAtt.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvAtt.RowTemplate.Height = 50;
            dgvAtt.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void guna2TextBox16_TextChanged(object sender, EventArgs e)
        {
            LoadAttendanceData(guna2TextBox16.Text.Trim());
        }
    }
}
