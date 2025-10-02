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
using DPFP.Capture;
using DPFP.Processing;
using MySql.Data.MySqlClient;

namespace Attendance_Monitoring_System
{
    public partial class verify : Form, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture Capturer;
        private DPFP.Verification.Verification Verifier;
        private int employeeId;
        private string employeeName;
        private string attendanceType; // "Time In" or "Time Out"

        public verify(string attType = "Time In")
        {
            InitializeComponent();
            this.attendanceType = attType;
            this.Text = $"Fingerprint Verification - {attendanceType}";
        }

        protected void Init()
        {
            this.Text = $"Fingerprint Verification - {attendanceType}";
            Verifier = new DPFP.Verification.Verification();
            
            // Initialize the fingerprint capturer
            try
            {
                Capturer = new DPFP.Capture.Capture();
                if (Capturer != null)
                {
                    Capturer.EventHandler = this;
                }
            }
            catch (Exception ex)
            {
                MakeReport("❌ Error initializing fingerprint scanner: " + ex.Message);
            }
            
            UpdateStatus();
            MakeReport($"Ready for {attendanceType.ToLower()}. Place your finger on the scanner.");
        }

        protected void Process(Sample Sample)
        {
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            if (features != null)
            {
                MakeReport("✅ Fingerprint sample captured successfully!");
                
                // Get all employees with fingerprint templates
                var employees = GetEmployeesWithFingerprints();
                
                foreach (var emp in employees)
                {
                    try
                    {
                        // Deserialize the stored template
                        MemoryStream templateStream = new MemoryStream(emp.FingerprintTemplate);
                        DPFP.Template storedTemplate = new DPFP.Template(templateStream);
                        
                        // Verify against stored template
                        DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                        Verifier.Verify(features, storedTemplate, ref result);
                        
                        if (result.Verified)
                        {
                            // Match found!
                            this.employeeId = emp.Id;
                            this.employeeName = emp.Name;
                            
                            MakeReport($"🎉 Match found! Welcome {emp.Name} ({emp.EmployeeCode})");
                            SetStatus($"✅ Verified: {emp.Name}");
                            
                            // Record attendance
                            RecordAttendance(emp.Id, emp.EmployeeCode, emp.Name);
                            
                            StopCapture();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MakeReport($"⚠️ Error verifying employee {emp.Name}: {ex.Message}");
                    }
                }
                
                // No match found
                MakeReport("❌ No matching fingerprint found. Please try again.");
                SetStatus("❌ Verification failed - No match found");
            }
        }

        private List<EmployeeData> GetEmployeesWithFingerprints()
        {
            var employees = new List<EmployeeData>();
            
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MySQL.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT ID, employee_code, CONCAT(first_name, ' ', last_name) as full_name, fingerprint_template FROM employees WHERE fingerprint_template IS NOT NULL";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new EmployeeData
                                {
                                    Id = reader.GetInt32("ID"),
                                    EmployeeCode = reader.GetString("employee_code"),
                                    Name = reader.GetString("full_name"),
                                    FingerprintTemplate = (byte[])reader["fingerprint_template"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MakeReport($"❌ Database error: {ex.Message}");
            }
            
            return employees;
        }

        private void RecordAttendance(int empId, string empCode, string empName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MySQL.ConnectionString))
                {
                    conn.Open();
                    
                    // Check if employee already has attendance record for today
                    string checkQuery = @"SELECT id, am_in 
                                        FROM attendance 
                                        WHERE employee_id = @empId 
                                        AND DATE(attendance_date) = CURDATE()";
                    int existingRecordId = -1;
                    bool hasTimeIn = false;
                    
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@empId", empId);
                        using (MySqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existingRecordId = reader.GetInt32("id");
                                hasTimeIn = !reader.IsDBNull(reader.GetOrdinal("am_in"));
                            }
                        }
                    }
                    
                    if (attendanceType == "Time In")
                    {
                        if (existingRecordId >= 0)
                        {
                            // Update existing record with AM in
                            string updateQuery = "UPDATE attendance SET am_in = @timeIn WHERE id = @id";
                            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@timeIn", DateTime.Now.TimeOfDay);
                                updateCmd.Parameters.AddWithValue("@id", existingRecordId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Create new record with AM in
                            string insertQuery = "INSERT INTO attendance (employee_id, attendance_date, am_in, status) VALUES (@empId, @date, @timeIn, 'Present')";
                            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@empId", empId);
                                insertCmd.Parameters.AddWithValue("@date", DateTime.Now.Date);
                                insertCmd.Parameters.AddWithValue("@timeIn", DateTime.Now.TimeOfDay);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                        
                        MessageBox.Show($"✅ Time In Recorded!\n\n" +
                                      $"Employee Code: {empCode}\n" +
                                      $"Employee: {empName}\n" +
                                      $"Time: {DateTime.Now:HH:mm:ss}\n" +
                                      $"Date: {DateTime.Now:yyyy-MM-dd}",
                                      "Time In Verification",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                    else // Time Out
                    {
                        if (existingRecordId >= 0 && hasTimeIn)
                        {
                            // Update existing record with AM out
                            string updateQuery = "UPDATE attendance SET am_out = @timeOut WHERE id = @id";
                            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@timeOut", DateTime.Now.TimeOfDay);
                                updateCmd.Parameters.AddWithValue("@id", existingRecordId);
                                updateCmd.ExecuteNonQuery();
                            }
                            
                            MessageBox.Show($"✅ Time Out Recorded!\n\n" +
                                          $"Employee Code: {empCode}\n" +
                                          $"Employee: {empName}\n" +
                                          $"Time: {DateTime.Now:HH:mm:ss}\n" +
                                          $"Date: {DateTime.Now:yyyy-MM-dd}",
                                          "Time Out Verification",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"⚠️ No Time In Record Found!\n\n" +
                                          $"Employee: {empCode} - {empName}\n\n" +
                                          $"Please record Time In first before Time Out.",
                                          "No Time In Record",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Database Error!\n\n" +
                              $"Failed to record attendance.\n\n" +
                              $"Error: {ex.Message}",
                              "Database Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }

        protected void Setfname(string value)
        {
            this.Invoke(new Action(delegate ()
            {
                fname.Text = value;
            }));
        }

        private void verify_Load(object sender, EventArgs e)
        {
            Init();
        }

        void DPFP.Capture.EventHandler.OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            MakeReport("Fingerprint sample captured.");
            SetPrompt("Processing...");
            Process(Sample);
        }

        void DPFP.Capture.EventHandler.OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            MakeReport("Finger touched the scanner.");
        }

        void DPFP.Capture.EventHandler.OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            MakeReport("Finger removed from scanner.");
        }

        void DPFP.Capture.EventHandler.OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("Fingerprint reader connected.");
        }

        void DPFP.Capture.EventHandler.OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("Fingerprint reader disconnected.");
        }

        void DPFP.Capture.EventHandler.OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
                MakeReport("The quality of the fingerprint sample is good.");
            else
                MakeReport("The quality of the fingerprint sample is poor.");
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StartCapture();
                    MakeReport($"Using the fingerprint scanner for {attendanceType.ToLower()}.");
                }
                catch
                {
                    MakeReport("Can't initiate capture!");
                }
            }
        }

        private void verify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    // Ignore errors on exit
                }
            }
        }

        private void UpdateStatus()
        {
            SetStatus($"Ready for {attendanceType.ToLower()} verification");
        }

        protected void MakeReport(string message)
        {
            this.Invoke(new Action(delegate ()
            {
                StatusText.AppendText(message + Environment.NewLine);
                StatusText.SelectionStart = StatusText.Text.Length;
                StatusText.ScrollToCaret();
            }));
        }

        protected void SetPrompt(string prompt)
        {
            this.Invoke(new Action(delegate ()
            {
                Prompt.Text = prompt;
            }));
        }

        protected void SetStatus(string status)
        {
            this.Invoke(new Action(delegate ()
            {
                StatusLabel.Text = status;
            }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Timer tick event - can be used for additional functionality
        }

        private void StopCapture()
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch (Exception ex)
                {
                    MakeReport("⚠️ Error stopping capture: " + ex.Message);
                }
            }
        }
    }

    public class EmployeeData
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public byte[] FingerprintTemplate { get; set; }
    }
}
