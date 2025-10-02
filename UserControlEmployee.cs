using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;

namespace Attendance_Monitoring_System
{
    public partial class UserControlEmployee : UserControl
    {
        public UserControlEmployee()
        {
            InitializeComponent();
        }

        private void UserControlEmployee_Load(object sender, EventArgs e)
        {
            // Add DataError event handler to handle display errors gracefully
            dgvEmpList.DataError += DgvEmpList_DataError;
            
            string query = @"
                SELECT e.ID,
                       e.employee_code,
                       e.first_name,
                       e.middle_name,
                       e.last_name,
                       e.birthdate,
                       e.sex,
                       e.address,
                       d.description AS department,
                       s.shift_name AS shift,
                       e.date_added,
                       et.Type AS employment_type,
                       e.role,
                       e.position,
                       e.contact_number,
                       e.photo
                FROM employees e
                LEFT JOIN departments d ON e.department_id = d.ID
                LEFT JOIN shift s ON e.shift_id = s.ID
                LEFT JOIN employmenttype et ON e.employmenttype_id = et.ID
                ORDER BY e.ID DESC;
            ";

            dgvEmpList.DataSource = MySQL.Pull(query);
            SetupEmployeeGrid();
        }

        private void DgvEmpList_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Handle DataGridView errors gracefully
            if (e.Exception is ArgumentException && e.Exception.Message.Contains("Parameter is not valid"))
            {
                // This is likely the fingerprint_template column trying to display as image
                e.ThrowException = false;
                dgvEmpList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "Binary Data";
            }
        }

        private void SetupEmployeeGrid()
        {
            dgvEmpList.AutoGenerateColumns = false;
            dgvEmpList.Columns.Clear();


            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID", HeaderText = "ID" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employee_code", HeaderText = "Employee Code" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "first_name", HeaderText = "First Name" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "middle_name", HeaderText = "Middle Name" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "last_name", HeaderText = "Last Name" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "birthdate", HeaderText = "Birthdate" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "sex", HeaderText = "Sex" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "address", HeaderText = "Address" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "department", HeaderText = "Department" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "shift", HeaderText = "Shift" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "date_added", HeaderText = "Date Added" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employment_type", HeaderText = "Employment Type" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "role", HeaderText = "Role" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "position", HeaderText = "Position" });
            dgvEmpList.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "contact_number", HeaderText = "Contact Number" });

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn
            {
                DataPropertyName = "photo",
                HeaderText = "Photo",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 80
            };
            dgvEmpList.Columns.Add(imgCol);

            dgvEmpList.AllowUserToAddRows = false;
            dgvEmpList.ReadOnly = true;
            dgvEmpList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmpList.MultiSelect = false;
            dgvEmpList.BorderStyle = BorderStyle.None;
            dgvEmpList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvEmpList.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvEmpList.EnableHeadersVisualStyles = false;
            dgvEmpList.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60);
            dgvEmpList.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEmpList.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvEmpList.ColumnHeadersHeight = 45;
            dgvEmpList.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvEmpList.DefaultCellStyle.BackColor = Color.White;
            dgvEmpList.DefaultCellStyle.ForeColor = Color.Black;
            dgvEmpList.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 34, 34);
            dgvEmpList.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvEmpList.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvEmpList.RowTemplate.Height = 70;
            dgvEmpList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = guna2TextBox1.Text.Trim();
            string searchParam = "%" + searchText.Replace("'", "''") + "%"; 

            string query = $@"
        SELECT e.ID,
               e.employee_code,
               e.first_name,
               e.middle_name,
               e.last_name,
               e.birthdate,
               e.sex,
               e.address,
               d.description AS department,
               s.shift_name AS shift,
               e.date_added,
               et.Type AS employment_type,
               e.role,
               e.position,
               e.contact_number,
               e.photo
        FROM employees e
        LEFT JOIN departments d ON e.department_id = d.ID
        LEFT JOIN shift s ON e.shift_id = s.ID
        LEFT JOIN employmenttype et ON e.employmenttype_id = et.ID
        WHERE CONCAT(e.first_name, ' ', e.middle_name, ' ', e.last_name) LIKE '{searchParam}'
           OR e.first_name LIKE '{searchParam}'
           OR e.middle_name LIKE '{searchParam}'
           OR e.last_name LIKE '{searchParam}'
        ORDER BY e.ID DESC;
    ";

            DataTable dt = MySQL.Pull(query);
            dgvEmpList.DataSource = dt;

            if (dgvEmpList.Columns.Contains("photo") && dgvEmpList.Columns["photo"] is DataGridViewImageColumn imgCol)
            {
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmpList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to view.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int empId = Convert.ToInt32(dgvEmpList.SelectedRows[0].Cells[0].Value);
            view frm = new view(empId); 
            frm.ShowDialog();
        }
    }
}
