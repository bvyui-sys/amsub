using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;

namespace Attendance_Monitoring_System
{
    public partial class view : Form
    {
        private int empId;

        public view(int empId)
        {
            InitializeComponent();
            this.empId = empId;
            StyleUI();
            LoadEmployeeData();
        }

        private void StyleUI()
        {
            guna2HtmlLabel1.Text = "Employee Profile";
            guna2HtmlLabel1.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            guna2HtmlLabel1.ForeColor = Color.Red;
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.AutoSize = true;

            guna2GradientPanel2.FillColor = Color.White;
            guna2GradientPanel2.FillColor2 = Color.White;
            guna2GradientPanel2.BorderColor = Color.Red;
            guna2GradientPanel2.BorderThickness = 2;
            guna2GradientPanel2.BorderRadius = 12;
            guna2GradientPanel2.ShadowDecoration.Enabled = true;
            guna2GradientPanel2.ShadowDecoration.Color = Color.FromArgb(180, 0, 0);
            guna2GradientPanel2.ShadowDecoration.Depth = 8;

            StyleTextBox(tbName);
            StyleTextBox(tbRole);
            StyleTextBox(tbEmpCode);
            StyleTextBox(tbDept);
            StyleTextBox(tbEmpType);
            StyleTextBox(tbDateHired);
            StyleTextBox(tbBday);
            StyleTextBox(tbCNum);
            StyleTextBox(tbShift);
            StyleTextBox(tbAddress);
            StyleTextBox(tbGender);
            StyleTextBox(tbPosition);
        }

        private void StyleTextBox(Guna2TextBox tb)
        {
            tb.ReadOnly = true;
            tb.BorderRadius = 8;
            tb.BorderColor = Color.LightGray;
            tb.FocusedState.BorderColor = Color.Red;
            tb.HoverState.BorderColor = Color.Red;
            tb.FillColor = Color.White;
            tb.ForeColor = Color.Black;
            tb.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            tb.PlaceholderForeColor = Color.Silver;
        }

        private void view_Load(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            
            string query = @"SELECT e.employee_code,
                        CONCAT(e.first_name,
                               CASE WHEN e.middle_name IS NULL OR e.middle_name = '' 
                                    THEN '' ELSE CONCAT(' ', e.middle_name) END,
                               ' ', e.last_name) AS employee_name,
                        e.role,
                        d.description AS department,
                        et.Type AS employment_type,
                        e.date_added,
                        e.birthdate,
                        e.contact_number,
                        s.shift_name AS shift,
                        e.address,
                        e.sex,
                        e.position,
                        e.photo
                 FROM employees e
                 LEFT JOIN departments d ON e.department_id = d.ID
                 LEFT JOIN employmenttype et ON e.employmenttype_id = et.ID
                 LEFT JOIN shift s ON e.shift_id = s.ID
                 WHERE e.ID = @id";

            using (MySqlConnection conn = new MySqlConnection(MySQL.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", empId);
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tbEmpCode.Text = reader["employee_code"].ToString();
                        tbName.Text = reader["employee_name"].ToString();
                        tbRole.Text = reader["role"].ToString();
                        tbDept.Text = reader["department"].ToString();
                        tbEmpType.Text = reader["employment_type"].ToString();
                        tbDateHired.Text = Convert.ToDateTime(reader["date_added"]).ToString("MMMM dd, yyyy");
                        tbBday.Text = Convert.ToDateTime(reader["birthdate"]).ToString("MMMM dd, yyyy");
                        tbCNum.Text = reader["contact_number"].ToString();
                        tbShift.Text = reader["shift"].ToString();
                        tbAddress.Text = reader["address"].ToString();
                        tbGender.Text = reader["sex"].ToString();
                        tbPosition.Text = reader["position"].ToString();

                        if (!(reader["photo"] is DBNull))
                        {
                            byte[] imgBytes = (byte[])reader["photo"];
                            using (MemoryStream ms = new MemoryStream(imgBytes))
                            {
                                pbEmp.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            pbEmp.Image = null;
                        }
                    }
                }
            }
        }
    }
}
