namespace Attendance_Monitoring_System
{
    partial class bio
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StatusText = new System.Windows.Forms.Label();
            this.FImage = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.FImage)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusText
            // 
            this.StatusText.AutoSize = true;
            this.StatusText.Location = new System.Drawing.Point(17, 436);
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(46, 17);
            this.StatusText.TabIndex = 0;
            this.StatusText.Text = "label1";
            // 
            // FImage
            // 
            this.FImage.ImageRotate = 0F;
            this.FImage.Location = new System.Drawing.Point(20, 12);
            this.FImage.Name = "FImage";
            this.FImage.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.FImage.Size = new System.Drawing.Size(414, 357);
            this.FImage.TabIndex = 1;
            this.FImage.TabStop = false;
            // 
            // guna2Button1
            // 
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Location = new System.Drawing.Point(312, 375);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(122, 35);
            this.guna2Button1.TabIndex = 2;
            this.guna2Button1.Text = "Start Capture";
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // bio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 472);
            this.Controls.Add(this.guna2Button1);
            this.Controls.Add(this.FImage);
            this.Controls.Add(this.StatusText);
            this.Name = "bio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "bio";
            this.Load += new System.EventHandler(this.bio_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StatusText;
        private Guna.UI2.WinForms.Guna2CirclePictureBox FImage;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
    }
}