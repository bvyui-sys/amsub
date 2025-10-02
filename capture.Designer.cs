namespace Attendance_Monitoring_System
{
    partial class capture
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
            this.components = new System.ComponentModel.Container();
            this.FImage = new Guna.UI2.WinForms.Guna2PictureBox();
            this.Prompt = new Guna.UI2.WinForms.Guna2TextBox();
            this.StatusText = new Guna.UI2.WinForms.Guna2TextBox();
            this.StatusLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.fname = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.FImage)).BeginInit();
            this.SuspendLayout();
            // 
            // FImage
            // 
            this.FImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FImage.ImageRotate = 0F;
            this.FImage.Location = new System.Drawing.Point(34, 23);
            this.FImage.Name = "FImage";
            this.FImage.Size = new System.Drawing.Size(346, 339);
            this.FImage.TabIndex = 0;
            this.FImage.TabStop = false;
            // 
            // Prompt
            // 
            this.Prompt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Prompt.DefaultText = "";
            this.Prompt.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.Prompt.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.Prompt.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.Prompt.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.Prompt.FillColor = System.Drawing.Color.Silver;
            this.Prompt.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Prompt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Prompt.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Prompt.Location = new System.Drawing.Point(398, 22);
            this.Prompt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Prompt.Name = "Prompt";
            this.Prompt.PlaceholderForeColor = System.Drawing.Color.DimGray;
            this.Prompt.PlaceholderText = "";
            this.Prompt.SelectedText = "";
            this.Prompt.Size = new System.Drawing.Size(387, 28);
            this.Prompt.TabIndex = 1;
            // 
            // StatusText
            // 
            this.StatusText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.StatusText.DefaultText = "";
            this.StatusText.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.StatusText.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.StatusText.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.StatusText.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.StatusText.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.StatusText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StatusText.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.StatusText.Location = new System.Drawing.Point(398, 60);
            this.StatusText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StatusText.Multiline = true;
            this.StatusText.Name = "StatusText";
            this.StatusText.PlaceholderText = "";
            this.StatusText.SelectedText = "";
            this.StatusText.Size = new System.Drawing.Size(387, 302);
            this.StatusText.TabIndex = 2;
            // 
            // StatusLabel
            // 
            this.StatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.StatusLabel.Location = new System.Drawing.Point(40, 407);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(66, 18);
            this.StatusLabel.TabIndex = 3;
            this.StatusLabel.Text = "[STATUS]";
            // 
            // fname
            // 
            this.fname.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fname.DefaultText = "";
            this.fname.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.fname.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.fname.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.fname.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.fname.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.fname.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.fname.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.fname.Location = new System.Drawing.Point(398, 370);
            this.fname.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fname.Name = "fname";
            this.fname.PlaceholderText = "";
            this.fname.SelectedText = "";
            this.fname.Size = new System.Drawing.Size(387, 25);
            this.fname.TabIndex = 4;
            this.fname.TextChanged += new System.EventHandler(this.fname_TextChanged);
            // 
            // guna2Button1
            // 
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Location = new System.Drawing.Point(644, 419);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(141, 42);
            this.guna2Button1.TabIndex = 5;
            this.guna2Button1.Text = "START SCAN";
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // capture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.guna2Button1);
            this.Controls.Add(this.fname);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.StatusText);
            this.Controls.Add(this.Prompt);
            this.Controls.Add(this.FImage);
            this.Name = "capture";
            this.Text = "capture";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.capture_FormClosing);
            this.Load += new System.EventHandler(this.capture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2PictureBox FImage;
        private Guna.UI2.WinForms.Guna2TextBox Prompt;
        private Guna.UI2.WinForms.Guna2TextBox StatusText;
        private Guna.UI2.WinForms.Guna2HtmlLabel StatusLabel;
        private Guna.UI2.WinForms.Guna2TextBox fname;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private System.Windows.Forms.Timer timer1;
    }
}