namespace QwerkStart
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
            this.components = new System.ComponentModel.Container( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.disconnectButton = new System.Windows.Forms.Button( );
            this.connectButton = new System.Windows.Forms.Button( );
            this.qwerkIPBox = new System.Windows.Forms.TextBox( );
            this.label1 = new System.Windows.Forms.Label( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer( );
            this.statusStrip = new System.Windows.Forms.StatusStrip( );
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.voltageLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.groupBox3 = new System.Windows.Forms.GroupBox( );
            this.led9Button = new System.Windows.Forms.Button( );
            this.led8Button = new System.Windows.Forms.Button( );
            this.led7Button = new System.Windows.Forms.Button( );
            this.led6Button = new System.Windows.Forms.Button( );
            this.led5Button = new System.Windows.Forms.Button( );
            this.led4Button = new System.Windows.Forms.Button( );
            this.led3Button = new System.Windows.Forms.Button( );
            this.led2Button = new System.Windows.Forms.Button( );
            this.led1Button = new System.Windows.Forms.Button( );
            this.led0Button = new System.Windows.Forms.Button( );
            this.groupBox1.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            this.statusStrip.SuspendLayout( );
            this.groupBox3.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.disconnectButton );
            this.groupBox1.Controls.Add( this.connectButton );
            this.groupBox1.Controls.Add( this.qwerkIPBox );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 342, 60 );
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Qwerk connection";
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new System.Drawing.Point( 260, 24 );
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size( 75, 23 );
            this.disconnectButton.TabIndex = 3;
            this.disconnectButton.Text = "&Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler( this.disconnectButton_Click );
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point( 180, 24 );
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size( 75, 23 );
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "&Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler( this.connectButton_Click );
            // 
            // qwerkIPBox
            // 
            this.qwerkIPBox.Location = new System.Drawing.Point( 75, 25 );
            this.qwerkIPBox.Name = "qwerkIPBox";
            this.qwerkIPBox.Size = new System.Drawing.Size( 100, 20 );
            this.qwerkIPBox.TabIndex = 1;
            this.qwerkIPBox.Text = "192.168.0.5";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 27 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 61, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "IP Address:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.videoSourcePlayer );
            this.groupBox2.Location = new System.Drawing.Point( 10, 75 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 342, 272 );
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera View";
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point( 10, 20 );
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size( 322, 242 );
            this.videoSourcePlayer.TabIndex = 0;
            this.videoSourcePlayer.VideoSource = null;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.voltageLabel,
            this.fpsLabel} );
            this.statusStrip.Location = new System.Drawing.Point( 0, 410 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 362, 24 );
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = false;
            this.statusLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.statusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size( 100, 19 );
            this.statusLabel.Text = "Disconnected";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // voltageLabel
            // 
            this.voltageLabel.AutoSize = false;
            this.voltageLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.voltageLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.voltageLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.voltageLabel.Name = "voltageLabel";
            this.voltageLabel.Size = new System.Drawing.Size( 120, 19 );
            this.voltageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fpsLabel
            // 
            this.fpsLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.fpsLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.fpsLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size( 127, 19 );
            this.fpsLabel.Spring = true;
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler( this.timer_Tick );
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add( this.led9Button );
            this.groupBox3.Controls.Add( this.led8Button );
            this.groupBox3.Controls.Add( this.led7Button );
            this.groupBox3.Controls.Add( this.led6Button );
            this.groupBox3.Controls.Add( this.led5Button );
            this.groupBox3.Controls.Add( this.led4Button );
            this.groupBox3.Controls.Add( this.led3Button );
            this.groupBox3.Controls.Add( this.led2Button );
            this.groupBox3.Controls.Add( this.led1Button );
            this.groupBox3.Controls.Add( this.led0Button );
            this.groupBox3.Location = new System.Drawing.Point( 10, 350 );
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size( 342, 53 );
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "LEDs";
            // 
            // led9Button
            // 
            this.led9Button.BackColor = System.Drawing.Color.Black;
            this.led9Button.Enabled = false;
            this.led9Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led9Button.ForeColor = System.Drawing.Color.White;
            this.led9Button.Location = new System.Drawing.Point( 298, 20 );
            this.led9Button.Name = "led9Button";
            this.led9Button.Size = new System.Drawing.Size( 30, 23 );
            this.led9Button.TabIndex = 9;
            this.led9Button.Text = "9";
            this.led9Button.UseVisualStyleBackColor = false;
            this.led9Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led8Button
            // 
            this.led8Button.BackColor = System.Drawing.Color.Black;
            this.led8Button.Enabled = false;
            this.led8Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led8Button.ForeColor = System.Drawing.Color.White;
            this.led8Button.Location = new System.Drawing.Point( 266, 20 );
            this.led8Button.Name = "led8Button";
            this.led8Button.Size = new System.Drawing.Size( 30, 23 );
            this.led8Button.TabIndex = 8;
            this.led8Button.Text = "8";
            this.led8Button.UseVisualStyleBackColor = false;
            this.led8Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led7Button
            // 
            this.led7Button.BackColor = System.Drawing.Color.Black;
            this.led7Button.Enabled = false;
            this.led7Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led7Button.ForeColor = System.Drawing.Color.White;
            this.led7Button.Location = new System.Drawing.Point( 234, 20 );
            this.led7Button.Name = "led7Button";
            this.led7Button.Size = new System.Drawing.Size( 30, 23 );
            this.led7Button.TabIndex = 7;
            this.led7Button.Text = "7";
            this.led7Button.UseVisualStyleBackColor = false;
            this.led7Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led6Button
            // 
            this.led6Button.BackColor = System.Drawing.Color.Black;
            this.led6Button.Enabled = false;
            this.led6Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led6Button.ForeColor = System.Drawing.Color.White;
            this.led6Button.Location = new System.Drawing.Point( 202, 20 );
            this.led6Button.Name = "led6Button";
            this.led6Button.Size = new System.Drawing.Size( 30, 23 );
            this.led6Button.TabIndex = 6;
            this.led6Button.Text = "6";
            this.led6Button.UseVisualStyleBackColor = false;
            this.led6Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led5Button
            // 
            this.led5Button.BackColor = System.Drawing.Color.Black;
            this.led5Button.Enabled = false;
            this.led5Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led5Button.ForeColor = System.Drawing.Color.White;
            this.led5Button.Location = new System.Drawing.Point( 170, 20 );
            this.led5Button.Name = "led5Button";
            this.led5Button.Size = new System.Drawing.Size( 30, 23 );
            this.led5Button.TabIndex = 5;
            this.led5Button.Text = "5";
            this.led5Button.UseVisualStyleBackColor = false;
            this.led5Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led4Button
            // 
            this.led4Button.BackColor = System.Drawing.Color.Black;
            this.led4Button.Enabled = false;
            this.led4Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led4Button.ForeColor = System.Drawing.Color.White;
            this.led4Button.Location = new System.Drawing.Point( 138, 20 );
            this.led4Button.Name = "led4Button";
            this.led4Button.Size = new System.Drawing.Size( 30, 23 );
            this.led4Button.TabIndex = 4;
            this.led4Button.Text = "4";
            this.led4Button.UseVisualStyleBackColor = false;
            this.led4Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led3Button
            // 
            this.led3Button.BackColor = System.Drawing.Color.Black;
            this.led3Button.Enabled = false;
            this.led3Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led3Button.ForeColor = System.Drawing.Color.White;
            this.led3Button.Location = new System.Drawing.Point( 106, 20 );
            this.led3Button.Name = "led3Button";
            this.led3Button.Size = new System.Drawing.Size( 30, 23 );
            this.led3Button.TabIndex = 3;
            this.led3Button.Text = "3";
            this.led3Button.UseVisualStyleBackColor = false;
            this.led3Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led2Button
            // 
            this.led2Button.BackColor = System.Drawing.Color.Black;
            this.led2Button.Enabled = false;
            this.led2Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led2Button.ForeColor = System.Drawing.Color.White;
            this.led2Button.Location = new System.Drawing.Point( 74, 20 );
            this.led2Button.Name = "led2Button";
            this.led2Button.Size = new System.Drawing.Size( 30, 23 );
            this.led2Button.TabIndex = 2;
            this.led2Button.Text = "2";
            this.led2Button.UseVisualStyleBackColor = false;
            this.led2Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led1Button
            // 
            this.led1Button.BackColor = System.Drawing.Color.Black;
            this.led1Button.Enabled = false;
            this.led1Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led1Button.ForeColor = System.Drawing.Color.White;
            this.led1Button.Location = new System.Drawing.Point( 42, 20 );
            this.led1Button.Name = "led1Button";
            this.led1Button.Size = new System.Drawing.Size( 30, 23 );
            this.led1Button.TabIndex = 1;
            this.led1Button.Text = "1";
            this.led1Button.UseVisualStyleBackColor = false;
            this.led1Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // led0Button
            // 
            this.led0Button.BackColor = System.Drawing.Color.Black;
            this.led0Button.Enabled = false;
            this.led0Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.led0Button.ForeColor = System.Drawing.Color.White;
            this.led0Button.Location = new System.Drawing.Point( 10, 20 );
            this.led0Button.Name = "led0Button";
            this.led0Button.Size = new System.Drawing.Size( 30, 23 );
            this.led0Button.TabIndex = 0;
            this.led0Button.Text = "0";
            this.led0Button.UseVisualStyleBackColor = false;
            this.led0Button.Click += new System.EventHandler( this.ledButton_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 362, 434 );
            this.Controls.Add( this.groupBox3 );
            this.Controls.Add( this.statusStrip );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Qwerk Start";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.groupBox2.ResumeLayout( false );
            this.statusStrip.ResumeLayout( false );
            this.statusStrip.PerformLayout( );
            this.groupBox3.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox qwerkIPBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel voltageLabel;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button led9Button;
        private System.Windows.Forms.Button led8Button;
        private System.Windows.Forms.Button led7Button;
        private System.Windows.Forms.Button led6Button;
        private System.Windows.Forms.Button led5Button;
        private System.Windows.Forms.Button led4Button;
        private System.Windows.Forms.Button led3Button;
        private System.Windows.Forms.Button led2Button;
        private System.Windows.Forms.Button led1Button;
        private System.Windows.Forms.Button led0Button;
    }
}

