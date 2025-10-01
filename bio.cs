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
    public partial class bio : Form, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture Capturer;
        public bio()
        {
            InitializeComponent();
        }

        protected void MakeReport(string message)
        {
            this.Invoke(new Action(delegate ()
           {
               StatusText.Text = message;
           }));

        }

        private void bio_Load(object sender, EventArgs e)
        {
            try
            {
                Capturer = new DPFP.Capture.Capture();
                if (Capturer != null)
                {
                    Capturer.EventHandler = this;
                    MakeReport("Press start capture to start scanning.");
                }
                else
                {
                    MakeReport("Cannot initiate capture operation.");
                }
            }
            catch
            {
                MessageBox.Show("Can't initiate capture operation!", "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void  OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("Fingerprint reader was connected.");
        }

        public void OnReaderDisconnect(object Capture, string RReaderSerialNumber)
        {
            MakeReport("Fingerprint reader was disconnected.");
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The finger was remove from the fingerprint reader.");
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The finger reader was touch.");
        }
        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            MakeReport("Fingerprint sample captured.");
            Process(Sample);
        }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
                MakeReport("The quality of the fingerprint sample is good.");
            else
                MakeReport("The quality of the fingerprint sample is poor.");
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

        private void DrawPicture(Bitmap bitmap)
        {
            FImage.Image = new Bitmap(bitmap, FImage.Size);
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if(Capturer != null)
            {
                try
                {
                    Capturer.StartCapture();
                    MakeReport("Using the fingerprint scanner, scan your fingerprint.");
                }
                catch
                {
                    MakeReport("Can't initiate capture!");
                }
            }
        }
    }
}
