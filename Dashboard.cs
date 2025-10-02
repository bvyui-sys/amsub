using System;
using System.Windows.Forms;

namespace Attendance_Monitoring_System
{
    delegate void Function();
    public partial class Dashboard : Form
    {

        public Dashboard()
        {
            InitializeComponent();
        }
 
        private void btnReport_Click(object sender, EventArgs e)
        {
    
            userControldb1.Visible = false;
            userControlRegister1.Visible = false;
            userControlEmployee1.Visible = false;
            leave1.Visible = true;
            userControlLeave1.Visible = false;
        }

        private void Dashboard_Load_1(object sender, EventArgs e)
        {
            datentime.Start();
            lbDatenTime.Text = DateTime.Now.ToLongDateString();
            this.ControlBox = false;

            userControldb1.Dock = DockStyle.Fill;
            userControlRegister1.Dock = DockStyle.Fill;
            userControlEmployee1.Dock = DockStyle.Fill;
            leave1.Dock = DockStyle.Fill;
            userControlLeave1.Dock = DockStyle.Fill;
            userControlAttendance1.Dock = DockStyle.Fill;
            
            // Set Dashboard as default view
            userControldb1.Visible = true;
            userControlRegister1.Visible = false;
            userControlEmployee1.Visible = false;
            leave1.Visible = false;
            userControlLeave1.Visible = false;
            userControlAttendance1.Visible = false;

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            userControldb1.Visible = true;
            userControlRegister1.Visible = false;
            userControlEmployee1.Visible = false;
            leave1.Visible = false;
            userControlLeave1.Visible = false;
            userControlAttendance1.Visible = false;



        }

        private void datentime_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            lbDatenTime.Text = now.ToString("F");
            datentime.Start();
        }

        private void btnEmp_Click(object sender, EventArgs e)
        {
            this.btnEmp.Click += new System.EventHandler(this.btnEmp_Click);
            userControldb1.Visible = false;
            userControlRegister1.Visible = false;
            userControlEmployee1.Visible = true;
            leave1.Visible = false;
            userControlLeave1.Visible = false;
            userControlAttendance1.Visible = false;

        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            this.btnReg.Click += new System.EventHandler(this.btnReg_Click);
            userControldb1.Visible = false;
            userControlRegister1.Visible = true;
            userControlEmployee1.Visible = false;
            leave1.Visible = false;
            userControlLeave1.Visible = false;
            userControlAttendance1.Visible = false;
        }


        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            DialogResult dialogResult = MessageBox.Show("Do you want to log out?", "Log Out",
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
            }

        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnReports1_Click(object sender, EventArgs e)
        {
            this.btnReports1.Click += new System.EventHandler(this.btnReports1_Click);
            userControldb1.Visible = false;
            userControlRegister1.Visible = false;
            userControlEmployee1.Visible = false;
            leave1.Visible = true;
            userControlLeave1.Visible = false;
            userControlAttendance1.Visible = false;
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                // Hide all other user controls
                userControldb1.Visible = false;
                userControlRegister1.Visible = false;
                userControlEmployee1.Visible = false;
                leave1.Visible = false;
                userControlLeave1.Visible = false;
                
                // Show attendance user control
                userControlAttendance1.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error opening Attendance section!\n\n" +
                              "Error: " + ex.Message,
                              "Attendance Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void btnLeave1_Click(object sender, EventArgs e)
        {
            this.btnLeave1.Click += new System.EventHandler(this.btnLeave1_Click);
            userControldb1.Visible = false;
            userControlRegister1.Visible = false;
            userControlEmployee1.Visible = false;
            leave1.Visible = false;
            userControlLeave1.Visible = true;
            userControlAttendance1.Visible = false;
        }

        private void userControlLeave1_Load(object sender, EventArgs e)
        {

        }
    }
}
    
    


    
