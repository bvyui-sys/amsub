using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attendance_Monitoring_System
{
    public partial class attendance : Form
    {
        public attendance()
        {
            InitializeComponent();
        }

        private void attendance_Load(object sender, EventArgs e)
        {
            // Create attendance table if it doesn't exist
            try
            {
                MySQL.CreateAttendanceTable();
                lblStatus.Text = "✅ Database ready for attendance tracking";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Database error: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            try
            {
                verify verifyForm = new verify("Time In");
                verifyForm.ShowDialog();
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

        private void btnViewAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                // Open attendance view form (you can create this later)
                MessageBox.Show("📊 Attendance View\n\n" +
                              "This feature will show attendance records.\n" +
                              "To be implemented in the next phase.",
                              "Coming Soon",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error opening attendance view!\n\n" +
                              "Error: " + ex.Message,
                              "View Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = "Current Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
