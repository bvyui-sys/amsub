using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DPFP;
using MySql.Data.MySqlClient;

namespace Attendance_Monitoring_System
{
    public partial class enroll : capture
    {
        public delegate void OnTemplateEventHandler(DPFP.Template template);

        public event OnTemplateEventHandler OnTemplate;

        private DPFP.Processing.Enrollment Enroller;
        private int employeeId;
        public enroll(int empId) : base(empId)   
        {
            InitializeComponent();
            this.employeeId = empId;
            EnsureFingerprintColumnExists();
        }

        private void EnsureFingerprintColumnExists()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MySQL.ConnectionString))
                {
                    conn.Open();
                    string checkColumnQuery = "SHOW COLUMNS FROM employees LIKE 'fingerprint_template'";
                    using (MySqlCommand cmd = new MySqlCommand(checkColumnQuery, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // Column doesn't exist, add it
                                reader.Close();
                                string addColumnQuery = "ALTER TABLE employees ADD COLUMN fingerprint_template LONGBLOB";
                                using (MySqlCommand addCmd = new MySqlCommand(addColumnQuery, conn))
                                {
                                    addCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ Database Schema Warning!\n\n" +
                              "Failed to verify/create fingerprint_template column.\n\n" +
                              "Error: " + ex.Message + "\n\n" +
                              "The enrollment may still work if the column already exists.\n" +
                              "Please check database permissions and try again.",
                              "Schema Warning", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Warning);
            }
        }


        protected override void Init()
        {
            base.Init();
            base.Text = "Fingerprint Enrollment - Employee ID: " + employeeId;
            Enroller = new DPFP.Processing.Enrollment();
            UpdateStatus();
            MakeReport("Ready to start fingerprint enrollment. Click 'START SCAN' to begin.");
        }

        protected override void Process(Sample Sample)
        {
            base.Process(Sample);
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            if (features != null)
            {
                try
                {
                    MakeReport("✅ Fingerprint sample captured successfully!");
                    Enroller.AddFeatures(features);
                }
                finally
                {
                    UpdateStatus();

                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:
                            {
                                OnTemplate(Enroller.Template);

                                // Serialize the fingerprint template to byte array
                                MemoryStream fingerprintData = new MemoryStream();
                                Enroller.Template.Serialize(fingerprintData);
                                fingerprintData.Position = 0;
                                BinaryReader br = new BinaryReader(fingerprintData);
                                byte[] bytes = br.ReadBytes((Int32)fingerprintData.Length);

                                // Save fingerprint template to database
                                try
                                {
                                    using (MySqlConnection conn = new MySqlConnection(MySQL.ConnectionString))
                                    {
                                        conn.Open();
                                        string query = "UPDATE employees SET fingerprint_template=@finger WHERE ID=@id";
                                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                                        {
                                            cmd.Parameters.Add("@finger", MySqlDbType.Blob).Value = bytes;
                                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = employeeId;

                                            int rowsAffected = cmd.ExecuteNonQuery(); 

                                            if (rowsAffected > 0)
                                            {
                                                MessageBox.Show("✅ Fingerprint Registration Successful!\n\n" +
                                                              "The fingerprint template has been successfully saved to the database.\n" +
                                                              "Employee ID: " + employeeId + "\n" +
                                                              "Template Size: " + bytes.Length + " bytes\n\n" +
                                                              "The employee can now use fingerprint authentication for attendance.",
                                                              "Registration Complete", 
                                                              MessageBoxButtons.OK, 
                                                              MessageBoxIcon.Information);
                                            }
                                            else
                                            {
                                                MessageBox.Show("❌ Registration Failed!\n\n" +
                                                              "Error: Employee ID " + employeeId + " was not found in the database.\n\n" +
                                                              "Please ensure the employee record exists before enrolling fingerprint.",
                                                              "Registration Error", 
                                                              MessageBoxButtons.OK, 
                                                              MessageBoxIcon.Error);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("❌ Database Error!\n\n" +
                                                  "Failed to save fingerprint template to database.\n\n" +
                                                  "Error Details: " + ex.Message + "\n\n" +
                                                  "Please check:\n" +
                                                  "• Database connection\n" +
                                                  "• Employee record exists\n" +
                                                  "• Database permissions",
                                                  "Database Error", 
                                                  MessageBoxButtons.OK, 
                                                  MessageBoxIcon.Error);
                                }

                                Stop(); 
                                break;
                            }

                        case DPFP.Processing.Enrollment.Status.Failed:
                            {
                                Enroller.Clear();
                                Stop();
                                UpdateStatus();
                                OnTemplate(null); 
                                Start();
                                break;
                            }
                    }
                }
            }
        }
        private void UpdateStatus()
        {
            uint featuresNeeded = Enroller.FeaturesNeeded;
            string statusMessage = "";
            
            if (featuresNeeded > 0)
            {
                statusMessage = String.Format("📋 Progress: {0} more fingerprint samples needed", featuresNeeded);
                MakeReport("Please scan the same finger again. Make sure to place your finger flat on the scanner.");
            }
            else
            {
                statusMessage = "✅ Template creation in progress...";
                MakeReport("All fingerprint samples collected. Creating template...");
            }
            
            SetStatus(statusMessage);
        }

        private void enroll_Load(object sender, EventArgs e)
        {

        }
    }
}
