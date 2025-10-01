using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DPFP;
using DPFP.Capture;

namespace Attendance_Monitoring_System
{
    public partial class capture : Form, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture Capturer;
        public string Firstname = "";
        private int employeeId;  // store the employee ID for later use

        public capture(int empId)
        {
            InitializeComponent();
            this.employeeId = empId;
        }


        protected void SetPrompt (string prompt)
        {
            this.Invoke(new Function (delegate()
                {
                Prompt.Text = prompt;
            }));
        }

        protected void SetStatus(string status)
        {
            this.Invoke(new Function(delegate ()
            {
                StatusLabel.Text = status;
            }));
        }
        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Function(delegate ()
            {
                FImage.Image = new Bitmap(bitmap, FImage.Size);
            }));
        }
        protected void SetFname(string value)
        {
            this.Invoke(new Function(delegate ()
            {
                fname.Text = value;
            }));
        }



        protected virtual void Init()
        {
            try
            {
                Capturer = new DPFP.Capture.Capture();
                if (null != Capturer)

                    Capturer.EventHandler = this; //subscribe capturer

                else
                    SetPrompt("Can't initiate capture operation.");
            }
            catch
            {
                MessageBox.Show("Can't initiate capture operation.");
            }
        }

  

        protected virtual void Process(DPFP.Sample Sample)
        {
            DrawPicture(ConvertSampleToBitmap(Sample));
        }

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();
            Bitmap bitmap = null;
            Convertor.ConvertToPicture(Sample, ref bitmap);
            return bitmap;
        }

        protected void Start()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    SetPrompt("Using the fingerprinter reader, scan your fingerprint");
                }
                catch
                {
                    SetPrompt("Can't initiate capture");
                }
            }
        }

        protected void Stop()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                    timer1.Dispose(); 
                }
                catch
                {
                    SetPrompt("Can't terminate capture!");
                }
            }
        }

        protected void MakeReport(string message)
        {
            this.Invoke(new Function(delegate ()
            {
                StatusText.AppendText(message + "\r\n"); //function to display logs 
            }));
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
            this.Invoke(new Function(delegate ()
            {
                fname.Text = value;
            }));
        }


        private void capture_Load(object sender, EventArgs e)
        {
            Init();
        }


        void DPFP.Capture.EventHandler.OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            MakeReport("Fingerprint sample captured.");
            SetPrompt("Scan the same fingerprint again.");
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

        void DPFP.Capture.EventHandler.OnSampleQuality(object Capture, string ReaderSerialNumber,DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == CaptureFeedback.Good)
                MakeReport("Good fingerprint quality.");
            else
                MakeReport("Poor fingerprint quality.");
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Start();
        }

        private void capture_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void fname_TextChanged(object sender, EventArgs e)
        {
            Firstname = fname.Text;
        }
    }
}
