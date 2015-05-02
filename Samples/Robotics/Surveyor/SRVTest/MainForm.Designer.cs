namespace SRVTest
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
            this.ipBox = new System.Windows.Forms.TextBox( );
            this.label1 = new System.Windows.Forms.Label( );
            this.statusStrip = new System.Windows.Forms.StatusStrip( );
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.versionLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.cameraPlayer = new AForge.Controls.VideoSourcePlayer( );
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.groupBox4 = new System.Windows.Forms.GroupBox( );
            this.resolutionCombo = new System.Windows.Forms.ComboBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.qualityCombo = new System.Windows.Forms.ComboBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.groupBox5 = new System.Windows.Forms.GroupBox( );
            this.maxPowerUpDown = new System.Windows.Forms.NumericUpDown( );
            this.maxPowerLabel = new System.Windows.Forms.Label( );
            this.minPowerUpDown = new System.Windows.Forms.NumericUpDown( );
            this.minPowerLabel = new System.Windows.Forms.Label( );
            this.turnControl = new AForge.Controls.SliderControl( );
            this.directControlRadio = new System.Windows.Forms.RadioButton( );
            this.predefinedCommandsRadio = new System.Windows.Forms.RadioButton( );
            this.srvDriverControl = new SRVTest.SrvDriverControl( );
            this.manipulatorControl = new AForge.Controls.ManipulatorControl( );
            this.aboutButton = new System.Windows.Forms.Button( );
            this.label4 = new System.Windows.Forms.Label( );
            this.portUpDown = new System.Windows.Forms.NumericUpDown( );
            this.groupBox1.SuspendLayout( );
            this.statusStrip.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            this.groupBox4.SuspendLayout( );
            this.groupBox5.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.maxPowerUpDown ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.minPowerUpDown ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.portUpDown ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.portUpDown );
            this.groupBox1.Controls.Add( this.label4 );
            this.groupBox1.Controls.Add( this.disconnectButton );
            this.groupBox1.Controls.Add( this.connectButton );
            this.groupBox1.Controls.Add( this.ipBox );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 342, 80 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SRV-1 Connection";
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new System.Drawing.Point( 260, 24 );
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size( 75, 23 );
            this.disconnectButton.TabIndex = 5;
            this.disconnectButton.Text = "&Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler( this.disconnectButton_Click );
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point( 180, 24 );
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size( 75, 23 );
            this.connectButton.TabIndex = 4;
            this.connectButton.Text = "&Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler( this.connectButton_Click );
            // 
            // ipBox
            // 
            this.ipBox.Location = new System.Drawing.Point( 75, 25 );
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size( 100, 20 );
            this.ipBox.TabIndex = 1;
            this.ipBox.Text = "169.254.0.10";
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
            // statusStrip
            // 
            this.statusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.versionLabel,
            this.fpsLabel} );
            this.statusStrip.Location = new System.Drawing.Point( 0, 408 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 685, 22 );
            this.statusStrip.TabIndex = 3;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = false;
            this.statusLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.statusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size( 100, 17 );
            this.statusLabel.Text = "Disconnected";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = false;
            this.versionLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.versionLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size( 350, 17 );
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fpsLabel
            // 
            this.fpsLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.fpsLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size( 220, 17 );
            this.fpsLabel.Spring = true;
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.cameraPlayer );
            this.groupBox2.Location = new System.Drawing.Point( 10, 95 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 342, 275 );
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Left Camera View";
            // 
            // cameraPlayer
            // 
            this.cameraPlayer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cameraPlayer.ForeColor = System.Drawing.Color.White;
            this.cameraPlayer.Location = new System.Drawing.Point( 10, 20 );
            this.cameraPlayer.Name = "cameraPlayer";
            this.cameraPlayer.Size = new System.Drawing.Size( 322, 242 );
            this.cameraPlayer.TabIndex = 0;
            this.cameraPlayer.VideoSource = null;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler( this.timer_Tick );
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add( this.resolutionCombo );
            this.groupBox4.Controls.Add( this.label3 );
            this.groupBox4.Controls.Add( this.qualityCombo );
            this.groupBox4.Controls.Add( this.label2 );
            this.groupBox4.Location = new System.Drawing.Point( 360, 10 );
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size( 313, 79 );
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Video settings";
            // 
            // resolutionCombo
            // 
            this.resolutionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolutionCombo.FormattingEnabled = true;
            this.resolutionCombo.Items.AddRange( new object[] {
            "160x120",
            "320x240",
            "640x480"} );
            this.resolutionCombo.Location = new System.Drawing.Point( 195, 25 );
            this.resolutionCombo.Name = "resolutionCombo";
            this.resolutionCombo.Size = new System.Drawing.Size( 70, 21 );
            this.resolutionCombo.TabIndex = 3;
            this.resolutionCombo.SelectedIndexChanged += new System.EventHandler( this.resolutionCombo_SelectedIndexChanged );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 135, 28 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 60, 13 );
            this.label3.TabIndex = 2;
            this.label3.Text = "Resolution:";
            // 
            // qualityCombo
            // 
            this.qualityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.qualityCombo.FormattingEnabled = true;
            this.qualityCombo.Items.AddRange( new object[] {
            "1 - Best",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8 - Worst"} );
            this.qualityCombo.Location = new System.Drawing.Point( 55, 25 );
            this.qualityCombo.Name = "qualityCombo";
            this.qualityCombo.Size = new System.Drawing.Size( 70, 21 );
            this.qualityCombo.TabIndex = 1;
            this.qualityCombo.SelectedIndexChanged += new System.EventHandler( this.qualityCombo_SelectedIndexChanged );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 28 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 42, 13 );
            this.label2.TabIndex = 0;
            this.label2.Text = "Quality:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add( this.maxPowerUpDown );
            this.groupBox5.Controls.Add( this.maxPowerLabel );
            this.groupBox5.Controls.Add( this.minPowerUpDown );
            this.groupBox5.Controls.Add( this.minPowerLabel );
            this.groupBox5.Controls.Add( this.turnControl );
            this.groupBox5.Controls.Add( this.directControlRadio );
            this.groupBox5.Controls.Add( this.predefinedCommandsRadio );
            this.groupBox5.Controls.Add( this.srvDriverControl );
            this.groupBox5.Controls.Add( this.manipulatorControl );
            this.groupBox5.Location = new System.Drawing.Point( 360, 95 );
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size( 313, 275 );
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Driving control";
            // 
            // maxPowerUpDown
            // 
            this.maxPowerUpDown.Location = new System.Drawing.Point( 260, 55 );
            this.maxPowerUpDown.Maximum = new decimal( new int[] {
            127,
            0,
            0,
            0} );
            this.maxPowerUpDown.Minimum = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.maxPowerUpDown.Name = "maxPowerUpDown";
            this.maxPowerUpDown.Size = new System.Drawing.Size( 45, 20 );
            this.maxPowerUpDown.TabIndex = 15;
            this.maxPowerUpDown.Value = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.maxPowerUpDown.Visible = false;
            this.maxPowerUpDown.ValueChanged += new System.EventHandler( this.maxPowerUpDown_ValueChanged );
            // 
            // maxPowerLabel
            // 
            this.maxPowerLabel.AutoSize = true;
            this.maxPowerLabel.Location = new System.Drawing.Point( 245, 40 );
            this.maxPowerLabel.Name = "maxPowerLabel";
            this.maxPowerLabel.Size = new System.Drawing.Size( 62, 13 );
            this.maxPowerLabel.TabIndex = 14;
            this.maxPowerLabel.Text = "Max power:";
            this.maxPowerLabel.Visible = false;
            // 
            // minPowerUpDown
            // 
            this.minPowerUpDown.Location = new System.Drawing.Point( 10, 55 );
            this.minPowerUpDown.Maximum = new decimal( new int[] {
            127,
            0,
            0,
            0} );
            this.minPowerUpDown.Minimum = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.minPowerUpDown.Name = "minPowerUpDown";
            this.minPowerUpDown.Size = new System.Drawing.Size( 45, 20 );
            this.minPowerUpDown.TabIndex = 13;
            this.minPowerUpDown.Value = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.minPowerUpDown.Visible = false;
            this.minPowerUpDown.ValueChanged += new System.EventHandler( this.minPowerUpDown_ValueChanged );
            // 
            // minPowerLabel
            // 
            this.minPowerLabel.AutoSize = true;
            this.minPowerLabel.Location = new System.Drawing.Point( 10, 40 );
            this.minPowerLabel.Name = "minPowerLabel";
            this.minPowerLabel.Size = new System.Drawing.Size( 59, 13 );
            this.minPowerLabel.TabIndex = 12;
            this.minPowerLabel.Text = "Min power:";
            this.minPowerLabel.Visible = false;
            // 
            // turnControl
            // 
            this.turnControl.Location = new System.Drawing.Point( 55, 245 );
            this.turnControl.Name = "turnControl";
            this.turnControl.Size = new System.Drawing.Size( 200, 23 );
            this.turnControl.TabIndex = 3;
            this.turnControl.Text = "turnControl1";
            this.turnControl.Visible = false;
            this.turnControl.PositionChanged += new AForge.Controls.SliderControl.PositionChangedHandler( this.turnControl_PositionChanged );
            // 
            // directControlRadio
            // 
            this.directControlRadio.AutoSize = true;
            this.directControlRadio.Location = new System.Drawing.Point( 170, 20 );
            this.directControlRadio.Name = "directControlRadio";
            this.directControlRadio.Size = new System.Drawing.Size( 88, 17 );
            this.directControlRadio.TabIndex = 2;
            this.directControlRadio.Text = "Direct control";
            this.directControlRadio.UseVisualStyleBackColor = true;
            this.directControlRadio.CheckedChanged += new System.EventHandler( this.directControlRadio_CheckedChanged );
            // 
            // predefinedCommandsRadio
            // 
            this.predefinedCommandsRadio.AutoSize = true;
            this.predefinedCommandsRadio.Checked = true;
            this.predefinedCommandsRadio.Location = new System.Drawing.Point( 10, 20 );
            this.predefinedCommandsRadio.Name = "predefinedCommandsRadio";
            this.predefinedCommandsRadio.Size = new System.Drawing.Size( 130, 17 );
            this.predefinedCommandsRadio.TabIndex = 1;
            this.predefinedCommandsRadio.TabStop = true;
            this.predefinedCommandsRadio.Text = "Predefined commands";
            this.predefinedCommandsRadio.UseVisualStyleBackColor = true;
            // 
            // srvDriverControl
            // 
            this.srvDriverControl.Location = new System.Drawing.Point( 10, 50 );
            this.srvDriverControl.Name = "srvDriverControl";
            this.srvDriverControl.Size = new System.Drawing.Size( 291, 163 );
            this.srvDriverControl.TabIndex = 0;
            this.srvDriverControl.SrvDrivingCommand += new SRVTest.SrvDrivingCommandHandler( this.srvDriverControl_SrvDrivingCommand );
            // 
            // manipulatorControl
            // 
            this.manipulatorControl.Location = new System.Drawing.Point( 56, 40 );
            this.manipulatorControl.Name = "manipulatorControl";
            this.manipulatorControl.Size = new System.Drawing.Size( 200, 200 );
            this.manipulatorControl.TabIndex = 1;
            this.manipulatorControl.Text = "manipulatorControl1";
            this.manipulatorControl.Visible = false;
            this.manipulatorControl.PositionChanged += new AForge.Controls.ManipulatorControl.PositionChangedHandler( this.manipulatorControl_PositionChanged );
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point( 598, 376 );
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size( 75, 23 );
            this.aboutButton.TabIndex = 12;
            this.aboutButton.Text = "About";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler( this.aboutButton_Click );
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 10, 52 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 29, 13 );
            this.label4.TabIndex = 2;
            this.label4.Text = "Port:";
            // 
            // portUpDown
            // 
            this.portUpDown.Location = new System.Drawing.Point( 75, 50 );
            this.portUpDown.Maximum = new decimal( new int[] {
            65535,
            0,
            0,
            0} );
            this.portUpDown.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
            this.portUpDown.Name = "portUpDown";
            this.portUpDown.Size = new System.Drawing.Size( 100, 20 );
            this.portUpDown.TabIndex = 3;
            this.portUpDown.Value = new decimal( new int[] {
            10001,
            0,
            0,
            0} );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 685, 430 );
            this.Controls.Add( this.aboutButton );
            this.Controls.Add( this.groupBox5 );
            this.Controls.Add( this.groupBox4 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.statusStrip );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Surveyor SRV-1 Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.statusStrip.ResumeLayout( false );
            this.statusStrip.PerformLayout( );
            this.groupBox2.ResumeLayout( false );
            this.groupBox4.ResumeLayout( false );
            this.groupBox4.PerformLayout( );
            this.groupBox5.ResumeLayout( false );
            this.groupBox5.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.maxPowerUpDown ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.minPowerUpDown ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.portUpDown ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private AForge.Controls.VideoSourcePlayer cameraPlayer;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripStatusLabel versionLabel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox resolutionCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox qualityCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox5;
        private SrvDriverControl srvDriverControl;
        private AForge.Controls.ManipulatorControl manipulatorControl;
        private System.Windows.Forms.RadioButton directControlRadio;
        private System.Windows.Forms.RadioButton predefinedCommandsRadio;
        private AForge.Controls.SliderControl turnControl;
        private System.Windows.Forms.NumericUpDown maxPowerUpDown;
        private System.Windows.Forms.NumericUpDown minPowerUpDown;
        private System.Windows.Forms.Label maxPowerLabel;
        private System.Windows.Forms.Label minPowerLabel;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown portUpDown;
    }
}

