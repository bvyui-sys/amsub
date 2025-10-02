using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Attendance_Monitoring_System
{
    public partial class UserControlAttendance : UserControl
    {
        public UserControlAttendance()
        {
            InitializeComponent();
        }

        private void UserControlAttendance_Load(object sender, EventArgs e)
        {
            LoadAttendanceData();
            LoadCurrentTime();
            
            // Start timer for real-time updates
            timer1.Start();
        }

        private void LoadAttendanceData()
        {
            try
            {
                string query = @"
                    SELECT 
                        a.id,
                        e.employee_code,
                        CONCAT(e.first_name, ' ', e.last_name) as employee_name,
                        a.attendance_date,
                        a.am_in,
                        a.am_out,
                        a.pm_in,
                        a.pm_out,
                        a.status,
                        CASE 
                            WHEN a.am_in IS NOT NULL AND a.am_out IS NOT NULL AND a.pm_in IS NOT NULL AND a.pm_out IS NOT NULL THEN 'Complete'
                            WHEN a.am_in IS NOT NULL AND a.am_out IS NOT NULL THEN 'AM Complete'
                            WHEN a.am_in IS NOT NULL THEN 'AM In Only'
                            ELSE 'No Record'
                        END as record_status
                    FROM attendance a
                    INNER JOIN employees e ON a.employee_id = e.ID
                    ORDER BY a.attendance_date DESC, a.am_in DESC
                ";

                DataTable dt = MySQL.Pull(query);
                dgvAttendance.DataSource = dt;
                SetupAttendanceGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error loading attendance data!\n\n" +
                              "Error: " + ex.Message,
                              "Database Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void SetupAttendanceGrid()
        {
            dgvAttendance.AutoGenerateColumns = false;
            dgvAttendance.Columns.Clear();

            // Add columns (ID removed, Employee Code & Name expanded)
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employee_code", HeaderText = "Employee Code", Width = 150 });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employee_name", HeaderText = "Employee Name", Width = 250 });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "attendance_date", HeaderText = "Date", Width = 120 });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "am_in", HeaderText = "AM In", Width = 100 });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "am_out", HeaderText = "AM Out", Width = 100 });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "pm_in", HeaderText = "PM In", Width = 100 });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "pm_out", HeaderText = "PM Out", Width = 100 });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "record_status", HeaderText = "Status", Width = 150 });

            // Style the grid
            dgvAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvAttendance.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvAttendance.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void LoadCurrentTime()
        {
            lblCurrentTime.Text = "Current Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            try
            {
                verify verifyForm = new verify("Time In");
                verifyForm.ShowDialog();
                LoadAttendanceData(); // Refresh data after verification
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error opening Time In verification!\n\n" +
                              "Error: " + ex.Message,
                              "Verification Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btnTimeOut_Click(object sender, EventArgs e)
        {
            try
            {
                verify verifyForm = new verify("Time Out");
                verifyForm.ShowDialog();
                LoadAttendanceData(); // Refresh data after verification
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error opening Time Out verification!\n\n" +
                              "Error: " + ex.Message,
                              "Verification Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAttendanceData();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Simple export to CSV
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog.Title = "Export Attendance Data";
                saveFileDialog.FileName = "Attendance_Report_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    MessageBox.Show("✅ Attendance data exported successfully!",
                                  "Export Complete",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error exporting data!\n\n" +
                              "Error: " + ex.Message,
                              "Export Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
            {
                // Write headers
                writer.WriteLine("ID,Employee Code,Employee Name,Date,AM In,AM Out,PM In,PM Out,Status");

                // Write data
                foreach (DataGridViewRow row in dgvAttendance.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        writer.WriteLine($"{row.Cells[0].Value},{row.Cells[1].Value},{row.Cells[2].Value},{row.Cells[3].Value},{row.Cells[4].Value},{row.Cells[5].Value},{row.Cells[6].Value},{row.Cells[7].Value},{row.Cells[8].Value}");
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(searchText))
            {
                LoadAttendanceData();
                return;
            }

            try
            {
                string query = $@"
                    SELECT 
                        a.id,
                        e.employee_code,
                        CONCAT(e.first_name, ' ', e.last_name) as employee_name,
                        a.attendance_date,
                        a.am_in,
                        a.am_out,
                        a.pm_in,
                        a.pm_out,
                        a.status,
                        CASE 
                            WHEN a.am_in IS NOT NULL AND a.am_out IS NOT NULL AND a.pm_in IS NOT NULL AND a.pm_out IS NOT NULL THEN 'Complete'
                            WHEN a.am_in IS NOT NULL AND a.am_out IS NOT NULL THEN 'AM Complete'
                            WHEN a.am_in IS NOT NULL THEN 'AM In Only'
                            ELSE 'No Record'
                        END as record_status
                    FROM attendance a
                    INNER JOIN employees e ON a.employee_id = e.ID
                    WHERE LOWER(e.employee_code) LIKE '%{searchText}%'
                       OR LOWER(CONCAT(e.first_name, ' ', e.last_name)) LIKE '%{searchText}%'
                    ORDER BY a.attendance_date DESC, a.am_in DESC
                ";

                DataTable dt = MySQL.Pull(query);
                dgvAttendance.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error searching attendance data!\n\n" +
                              "Error: " + ex.Message,
                              "Search Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LoadCurrentTime();
        }
    }
}
