namespace KinectCapture
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
            this.label1 = new System.Windows.Forms.Label( );
            this.devicesCombo = new System.Windows.Forms.ComboBox( );
            this.connectButton = new System.Windows.Forms.Button( );
            this.disconnectButton = new System.Windows.Forms.Button( );
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel( );
            this.leftPanel = new System.Windows.Forms.Panel( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.videoCameraPlayer = new AForge.Controls.VideoSourcePlayer( );
            this.rightPanel = new System.Windows.Forms.Panel( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.depthCameraPlayer = new AForge.Controls.VideoSourcePlayer( );
            this.label2 = new System.Windows.Forms.Label( );
            this.ledColorCombo = new System.Windows.Forms.ComboBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.tiltUpDown = new System.Windows.Forms.NumericUpDown( );
            this.label4 = new System.Windows.Forms.Label( );
            this.videoModeCombo = new System.Windows.Forms.ComboBox( );
            this.statusStrip = new System.Windows.Forms.StatusStrip( );
            this.accelerometerLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.tableLayoutPanel.SuspendLayout( );
            this.leftPanel.SuspendLayout( );
            this.groupBox1.SuspendLayout( );
            this.rightPanel.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.tiltUpDown ) ).BeginInit( );
            this.statusStrip.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 13 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 75, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Select device:";
            // 
            // devicesCombo
            // 
            this.devicesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.devicesCombo.FormattingEnabled = true;
            this.devicesCombo.Location = new System.Drawing.Point( 90, 10 );
            this.devicesCombo.Name = "devicesCombo";
            this.devicesCombo.Size = new System.Drawing.Size( 121, 21 );
            this.devicesCombo.TabIndex = 1;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point( 220, 10 );
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size( 75, 23 );
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "&Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler( this.connectButton_Click );
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point( 300, 10 );
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size( 75, 23 );
            this.disconnectButton.TabIndex = 3;
            this.disconnectButton.Text = "&Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler( this.disconnectButton_Click );
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            this.tableLayoutPanel.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            this.tableLayoutPanel.Controls.Add( this.leftPanel, 0, 0 );
            this.tableLayoutPanel.Controls.Add( this.rightPanel, 1, 0 );
            this.tableLayoutPanel.Location = new System.Drawing.Point( 0, 40 );
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 50F ) );
            this.tableLayoutPanel.Size = new System.Drawing.Size( 826, 295 );
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add( this.groupBox1 );
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point( 3, 3 );
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size( 407, 289 );
            this.leftPanel.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox1.Controls.Add( this.videoCameraPlayer );
            this.groupBox1.Location = new System.Drawing.Point( 5, 5 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 396, 277 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Video Camera";
            // 
            // videoCameraPlayer
            // 
            this.videoCameraPlayer.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.videoCameraPlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoCameraPlayer.ForeColor = System.Drawing.Color.FromArgb( ( (int) ( ( (byte) ( 192 ) ) ) ), ( (int) ( ( (byte) ( 192 ) ) ) ), ( (int) ( ( (byte) ( 255 ) ) ) ) );
            this.videoCameraPlayer.Location = new System.Drawing.Point( 11, 15 );
            this.videoCameraPlayer.Name = "videoCameraPlayer";
            this.videoCameraPlayer.Size = new System.Drawing.Size( 373, 253 );
            this.videoCameraPlayer.TabIndex = 0;
            this.videoCameraPlayer.VideoSource = null;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add( this.groupBox2 );
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point( 416, 3 );
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size( 407, 289 );
            this.rightPanel.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox2.Controls.Add( this.depthCameraPlayer );
            this.groupBox2.Location = new System.Drawing.Point( 6, 6 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 395, 277 );
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Depth Camera";
            // 
            // depthCameraPlayer
            // 
            this.depthCameraPlayer.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.depthCameraPlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.depthCameraPlayer.ForeColor = System.Drawing.Color.FromArgb( ( (int) ( ( (byte) ( 192 ) ) ) ), ( (int) ( ( (byte) ( 192 ) ) ) ), ( (int) ( ( (byte) ( 255 ) ) ) ) );
            this.depthCameraPlayer.Location = new System.Drawing.Point( 11, 15 );
            this.depthCameraPlayer.Name = "depthCameraPlayer";
            this.depthCameraPlayer.Size = new System.Drawing.Size( 373, 253 );
            this.depthCameraPlayer.TabIndex = 0;
            this.depthCameraPlayer.Text = "videoSourcePlayer2";
            this.depthCameraPlayer.VideoSource = null;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 390, 13 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 31, 13 );
            this.label2.TabIndex = 6;
            this.label2.Text = "LED:";
            // 
            // ledColorCombo
            // 
            this.ledColorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ledColorCombo.FormattingEnabled = true;
            this.ledColorCombo.Items.AddRange( new object[] {
            "Off",
            "Green",
            "Red",
            "Yellow",
            "Blinking Green",
            "Blinking Red-Yellow"} );
            this.ledColorCombo.Location = new System.Drawing.Point( 425, 10 );
            this.ledColorCombo.Name = "ledColorCombo";
            this.ledColorCombo.Size = new System.Drawing.Size( 95, 21 );
            this.ledColorCombo.TabIndex = 7;
            this.ledColorCombo.SelectedIndexChanged += new System.EventHandler( this.ledColorCombo_SelectedIndexChanged );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 530, 13 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 24, 13 );
            this.label3.TabIndex = 8;
            this.label3.Text = "Tilt:";
            // 
            // tiltUpDown
            // 
            this.tiltUpDown.Location = new System.Drawing.Point( 560, 10 );
            this.tiltUpDown.Maximum = new decimal( new int[] {
            31,
            0,
            0,
            0} );
            this.tiltUpDown.Minimum = new decimal( new int[] {
            31,
            0,
            0,
            -2147483648} );
            this.tiltUpDown.Name = "tiltUpDown";
            this.tiltUpDown.Size = new System.Drawing.Size( 62, 20 );
            this.tiltUpDown.TabIndex = 9;
            this.tiltUpDown.ValueChanged += new System.EventHandler( this.tiltUpDown_ValueChanged );
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 635, 13 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 66, 13 );
            this.label4.TabIndex = 10;
            this.label4.Text = "Video mode:";
            // 
            // videoModeCombo
            // 
            this.videoModeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.videoModeCombo.FormattingEnabled = true;
            this.videoModeCombo.Items.AddRange( new object[] {
            "Color",
            "Bayer",
            "InfraRed"} );
            this.videoModeCombo.Location = new System.Drawing.Point( 706, 10 );
            this.videoModeCombo.Name = "videoModeCombo";
            this.videoModeCombo.Size = new System.Drawing.Size( 100, 21 );
            this.videoModeCombo.TabIndex = 11;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.accelerometerLabel} );
            this.statusStrip.Location = new System.Drawing.Point( 0, 338 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 827, 22 );
            this.statusStrip.TabIndex = 12;
            this.statusStrip.Text = "statusStrip1";
            // 
            // accelerometerLabel
            // 
            this.accelerometerLabel.AutoSize = false;
            this.accelerometerLabel.Name = "accelerometerLabel";
            this.accelerometerLabel.Size = new System.Drawing.Size( 781, 17 );
            this.accelerometerLabel.Spring = true;
            this.accelerometerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler( this.timer_Tick );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 827, 360 );
            this.Controls.Add( this.statusStrip );
            this.Controls.Add( this.videoModeCombo );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.tiltUpDown );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.ledColorCombo );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.tableLayoutPanel );
            this.Controls.Add( this.disconnectButton );
            this.Controls.Add( this.connectButton );
            this.Controls.Add( this.devicesCombo );
            this.Controls.Add( this.label1 );
            this.MinimumSize = new System.Drawing.Size( 840, 390 );
            this.Name = "MainForm";
            this.Text = "Kinect Capture";
            this.Load += new System.EventHandler( this.MainForm_Load );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.tableLayoutPanel.ResumeLayout( false );
            this.leftPanel.ResumeLayout( false );
            this.groupBox1.ResumeLayout( false );
            this.rightPanel.ResumeLayout( false );
            this.groupBox2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) ( this.tiltUpDown ) ).EndInit( );
            this.statusStrip.ResumeLayout( false );
            this.statusStrip.PerformLayout( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox devicesCombo;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private AForge.Controls.VideoSourcePlayer videoCameraPlayer;
        private AForge.Controls.VideoSourcePlayer depthCameraPlayer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ledColorCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown tiltUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox videoModeCombo;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel accelerometerLabel;
        private System.Windows.Forms.Timer timer;
    }
}

