using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;



namespace Attendance_Monitoring_System
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            insertUsername.Text = "Insert Username";
            insertpw.Text = "Insert Password";
            insertUsername.ForeColor = Color.Black;
            insertpw.ForeColor = Color.Black;
        }

        private void insertUsername_Enter_1(object sender, EventArgs e)
        {
            {
                if (insertUsername.Text == "Insert Username")
                {
                    insertUsername.Text = "";
                    insertUsername.ForeColor = Color.Black;
                }
            }
        }

        private void insertUsername_Leave_1(object sender, EventArgs e)
        {
            {
                if (insertUsername.Text == "")
                {
                    insertUsername.Text = "Insert Username";
                    insertUsername.ForeColor = Color.LightGray;
                }
            }
        }

        private void insertpw_Enter_1(object sender, EventArgs e)
        {
            {
                if (insertpw.Text == "Insert Password")
                {
                    insertpw.Text = "";
                    insertpw.ForeColor = Color.Black;
                }
            }
        }

        private void insertpw_Leave(object sender, EventArgs e)
        {
            {
                if (insertpw.Text == "")
                {
                    insertpw.Text = "Insert Password";
                    insertpw.ForeColor = Color.LightGray;
                }
            }
        }

 

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void pictureBoxMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {

            string connectionString = "server=localhost;user id=root;password=admin123;database=lasam_attendance;";


            string username = insertUsername.Text;
            string password = insertpw.Text;


            string query = "SELECT * FROM users WHERE username = '" + username + "' AND password_hash = MD5('" + password + "')";

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {

                    Dashboard dash = new Dashboard();
                    dash.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                }

                con.Close();
            }
        }

        private void showpw_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void showpw_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void insertpw_OnValueChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}

