namespace SampleApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.components = new System.ComponentModel.Container();
            this.videoSourcePlayer1 = new Accord.Controls.VideoSourcePlayer();
            this.videoSourcePlayer2 = new Accord.Controls.VideoSourcePlayer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.camera1FpsLabel = new System.Windows.Forms.Label();
            this.camera1Combo = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.camera2FpsLabel = new System.Windows.Forms.Label();
            this.camera2Combo = new System.Windows.Forms.ComboBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.videoSourcePlayer1.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer1.Location = new System.Drawing.Point(15, 77);
            this.videoSourcePlayer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(483, 372);
            this.videoSourcePlayer1.TabIndex = 0;
            this.videoSourcePlayer1.VideoSource = null;
            // 
            // videoSourcePlayer2
            // 
            this.videoSourcePlayer2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.videoSourcePlayer2.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer2.Location = new System.Drawing.Point(15, 77);
            this.videoSourcePlayer2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.videoSourcePlayer2.Name = "videoSourcePlayer2";
            this.videoSourcePlayer2.Size = new System.Drawing.Size(483, 372);
            this.videoSourcePlayer2.TabIndex = 1;
            this.videoSourcePlayer2.VideoSource = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.camera1FpsLabel);
            this.groupBox1.Controls.Add(this.camera1Combo);
            this.groupBox1.Controls.Add(this.videoSourcePlayer1);
            this.groupBox1.Location = new System.Drawing.Point(15, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(513, 485);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera 1";
            // 
            // camera1FpsLabel
            // 
            this.camera1FpsLabel.Location = new System.Drawing.Point(423, 454);
            this.camera1FpsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.camera1FpsLabel.Name = "camera1FpsLabel";
            this.camera1FpsLabel.Size = new System.Drawing.Size(75, 26);
            this.camera1FpsLabel.TabIndex = 4;
            this.camera1FpsLabel.Text = "label1";
            this.camera1FpsLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // camera1Combo
            // 
            this.camera1Combo.FormattingEnabled = true;
            this.camera1Combo.Location = new System.Drawing.Point(15, 31);
            this.camera1Combo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.camera1Combo.Name = "camera1Combo";
            this.camera1Combo.Size = new System.Drawing.Size(481, 28);
            this.camera1Combo.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.camera2FpsLabel);
            this.groupBox2.Controls.Add(this.camera2Combo);
            this.groupBox2.Controls.Add(this.videoSourcePlayer2);
            this.groupBox2.Location = new System.Drawing.Point(540, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(513, 485);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera 2";
            // 
            // camera2FpsLabel
            // 
            this.camera2FpsLabel.Location = new System.Drawing.Point(423, 454);
            this.camera2FpsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.camera2FpsLabel.Name = "camera2FpsLabel";
            this.camera2FpsLabel.Size = new System.Drawing.Size(75, 26);
            this.camera2FpsLabel.TabIndex = 5;
            this.camera2FpsLabel.Text = "label2";
            this.camera2FpsLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // camera2Combo
            // 
            this.camera2Combo.FormattingEnabled = true;
            this.camera2Combo.Location = new System.Drawing.Point(15, 31);
            this.camera2Combo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.camera2Combo.Name = "camera2Combo";
            this.camera2Combo.Size = new System.Drawing.Size(481, 28);
            this.camera2Combo.TabIndex = 2;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(818, 515);
            this.startButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(112, 35);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "&Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(940, 515);
            this.stopButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(112, 35);
            this.stopButton.TabIndex = 5;
            this.stopButton.Text = "S&top";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1066, 568);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Two Cameras Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Accord.Controls.VideoSourcePlayer videoSourcePlayer1;
        private Accord.Controls.VideoSourcePlayer videoSourcePlayer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox camera1Combo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox camera2Combo;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label camera1FpsLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label camera2FpsLabel;
    }
}

