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
        }


        protected override void Init()
        {
            base.Init();
            base.Text = "Fingerprint Enrollment";
            Enroller = new DPFP.Processing.Enrollment();
            UpdateStatus();
        }

        protected override void Process(Sample Sample)
        {
            base.Process(Sample);
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            if (features != null)
            
                try
                {
                    MakeReport("The fingerprint feature set was created.");
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

                            
                                MemoryStream fingerprintData = new MemoryStream();
                                Enroller.Template.Serialize(fingerprintData);
                                fingerprintData.Position = 0;
                                BinaryReader br = new BinaryReader(fingerprintData);
                                byte[] bytes = br.ReadBytes((Int32)fingerprintData.Length);

                              
                                try
                                {
                                    string connStr = "server=localhost;uid=root;pwd=123456;database=yourdb";
                                    using (MySqlConnection conn = new MySqlConnection(connStr))
                                    {
                                        conn.Open();
                                        string query = "UPDATE employees SET fingerprint_template=@finger WHERE ID=@id";
                                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                                        {
                                            cmd.Parameters.Add("@finger", MySqlDbType.Blob).Value = bytes;
                                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = employeeId;

                                            int rowsAffected = cmd.ExecuteNonQuery(); 

                                            if (rowsAffected > 0)
                                                MessageBox.Show("Fingerprint enrolled successfully!");
                                            else
                                                MessageBox.Show("Error: Employee ID not found!");
                                        }
                                    }
                                
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error saving fingerprint: " + ex.Message);
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
        private void UpdateStatus()
        {
            SetStatus(String.Format("Fingerprint sample needed: {0}", Enroller.FeaturesNeeded));

        }

        private void enroll_Load(object sender, EventArgs e)
        {

        }
    }
}
