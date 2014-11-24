namespace XimeaSample
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
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer( );
            this.statusStrip = new System.Windows.Forms.StatusStrip( );
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.spareLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.topPanel = new System.Windows.Forms.Panel( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.offsetYUpDown = new System.Windows.Forms.NumericUpDown( );
            this.label9 = new System.Windows.Forms.Label( );
            this.offsetXUpDown = new System.Windows.Forms.NumericUpDown( );
            this.label10 = new System.Windows.Forms.Label( );
            this.heightUpDown = new System.Windows.Forms.NumericUpDown( );
            this.label8 = new System.Windows.Forms.Label( );
            this.widthUpDown = new System.Windows.Forms.NumericUpDown( );
            this.label7 = new System.Windows.Forms.Label( );
            this.gainUpDown = new System.Windows.Forms.NumericUpDown( );
            this.label6 = new System.Windows.Forms.Label( );
            this.exposureUpDown = new System.Windows.Forms.NumericUpDown( );
            this.label5 = new System.Windows.Forms.Label( );
            this.typeBox = new System.Windows.Forms.TextBox( );
            this.snBox = new System.Windows.Forms.TextBox( );
            this.nameBox = new System.Windows.Forms.TextBox( );
            this.label4 = new System.Windows.Forms.Label( );
            this.label3 = new System.Windows.Forms.Label( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.disconnectButton = new System.Windows.Forms.Button( );
            this.connectButton = new System.Windows.Forms.Button( );
            this.deviceCombo = new System.Windows.Forms.ComboBox( );
            this.mainPanel = new System.Windows.Forms.Panel( );
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.statusStrip.SuspendLayout( );
            this.topPanel.SuspendLayout( );
            this.groupBox1.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.offsetYUpDown ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.offsetXUpDown ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.heightUpDown ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.widthUpDown ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.gainUpDown ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.exposureUpDown ) ).BeginInit( );
            this.mainPanel.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.AutoSizeControl = true;
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point( 194, 60 );
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size( 322, 242 );
            this.videoSourcePlayer.TabIndex = 7;
            this.videoSourcePlayer.VideoSource = null;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.spareLabel} );
            this.statusStrip.Location = new System.Drawing.Point( 0, 508 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 710, 22 );
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = false;
            this.fpsLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.fpsLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size( 100, 17 );
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spareLabel
            // 
            this.spareLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.spareLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.spareLabel.Name = "spareLabel";
            this.spareLabel.Size = new System.Drawing.Size( 595, 17 );
            this.spareLabel.Spring = true;
            this.spareLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add( this.groupBox1 );
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point( 0, 0 );
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size( 710, 145 );
            this.topPanel.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox1.Controls.Add( this.offsetYUpDown );
            this.groupBox1.Controls.Add( this.label9 );
            this.groupBox1.Controls.Add( this.offsetXUpDown );
            this.groupBox1.Controls.Add( this.label10 );
            this.groupBox1.Controls.Add( this.heightUpDown );
            this.groupBox1.Controls.Add( this.label8 );
            this.groupBox1.Controls.Add( this.widthUpDown );
            this.groupBox1.Controls.Add( this.label7 );
            this.groupBox1.Controls.Add( this.gainUpDown );
            this.groupBox1.Controls.Add( this.label6 );
            this.groupBox1.Controls.Add( this.exposureUpDown );
            this.groupBox1.Controls.Add( this.label5 );
            this.groupBox1.Controls.Add( this.typeBox );
            this.groupBox1.Controls.Add( this.snBox );
            this.groupBox1.Controls.Add( this.nameBox );
            this.groupBox1.Controls.Add( this.label4 );
            this.groupBox1.Controls.Add( this.label3 );
            this.groupBox1.Controls.Add( this.label2 );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Controls.Add( this.disconnectButton );
            this.groupBox1.Controls.Add( this.connectButton );
            this.groupBox1.Controls.Add( this.deviceCombo );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 689, 129 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera parameters:";
            // 
            // offsetYUpDown
            // 
            this.offsetYUpDown.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.offsetYUpDown.Location = new System.Drawing.Point( 520, 50 );
            this.offsetYUpDown.Name = "offsetYUpDown";
            this.offsetYUpDown.Size = new System.Drawing.Size( 62, 20 );
            this.offsetYUpDown.TabIndex = 17;
            this.offsetYUpDown.ValueChanged += new System.EventHandler( this.offsetYUpDown_ValueChanged );
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point( 468, 53 );
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size( 48, 13 );
            this.label9.TabIndex = 16;
            this.label9.Text = "Offset Y:";
            // 
            // offsetXUpDown
            // 
            this.offsetXUpDown.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.offsetXUpDown.Location = new System.Drawing.Point( 360, 50 );
            this.offsetXUpDown.Name = "offsetXUpDown";
            this.offsetXUpDown.Size = new System.Drawing.Size( 62, 20 );
            this.offsetXUpDown.TabIndex = 15;
            this.offsetXUpDown.ValueChanged += new System.EventHandler( this.offsetXUpDown_ValueChanged );
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point( 311, 53 );
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size( 48, 13 );
            this.label10.TabIndex = 14;
            this.label10.Text = "Offset X:";
            // 
            // heightUpDown
            // 
            this.heightUpDown.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.heightUpDown.Location = new System.Drawing.Point( 520, 20 );
            this.heightUpDown.Name = "heightUpDown";
            this.heightUpDown.Size = new System.Drawing.Size( 62, 20 );
            this.heightUpDown.TabIndex = 13;
            this.heightUpDown.ValueChanged += new System.EventHandler( this.heightUpDown_ValueChanged );
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point( 475, 23 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 41, 13 );
            this.label8.TabIndex = 12;
            this.label8.Text = "Height:";
            // 
            // widthUpDown
            // 
            this.widthUpDown.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
            this.widthUpDown.Location = new System.Drawing.Point( 360, 20 );
            this.widthUpDown.Name = "widthUpDown";
            this.widthUpDown.Size = new System.Drawing.Size( 62, 20 );
            this.widthUpDown.TabIndex = 11;
            this.widthUpDown.ValueChanged += new System.EventHandler( this.widthUpDown_ValueChanged );
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point( 321, 23 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 38, 13 );
            this.label7.TabIndex = 10;
            this.label7.Text = "Width:";
            // 
            // gainUpDown
            // 
            this.gainUpDown.DecimalPlaces = 2;
            this.gainUpDown.Location = new System.Drawing.Point( 360, 80 );
            this.gainUpDown.Name = "gainUpDown";
            this.gainUpDown.Size = new System.Drawing.Size( 62, 20 );
            this.gainUpDown.TabIndex = 19;
            this.gainUpDown.ValueChanged += new System.EventHandler( this.gainUpDown_ValueChanged );
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point( 305, 83 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 54, 13 );
            this.label6.TabIndex = 18;
            this.label6.Text = "Gain (dB):";
            // 
            // exposureUpDown
            // 
            this.exposureUpDown.DecimalPlaces = 2;
            this.exposureUpDown.Increment = new decimal( new int[] {
            2,
            0,
            0,
            0} );
            this.exposureUpDown.Location = new System.Drawing.Point( 520, 80 );
            this.exposureUpDown.Name = "exposureUpDown";
            this.exposureUpDown.Size = new System.Drawing.Size( 62, 20 );
            this.exposureUpDown.TabIndex = 21;
            this.exposureUpDown.ValueChanged += new System.EventHandler( this.exposureUpDown_ValueChanged );
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point( 440, 83 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 76, 13 );
            this.label5.TabIndex = 20;
            this.label5.Text = "Exposure (ms):";
            // 
            // typeBox
            // 
            this.typeBox.Location = new System.Drawing.Point( 70, 100 );
            this.typeBox.Name = "typeBox";
            this.typeBox.ReadOnly = true;
            this.typeBox.Size = new System.Drawing.Size( 210, 20 );
            this.typeBox.TabIndex = 9;
            // 
            // snBox
            // 
            this.snBox.Location = new System.Drawing.Point( 70, 75 );
            this.snBox.Name = "snBox";
            this.snBox.ReadOnly = true;
            this.snBox.Size = new System.Drawing.Size( 210, 20 );
            this.snBox.TabIndex = 7;
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point( 70, 50 );
            this.nameBox.Name = "nameBox";
            this.nameBox.ReadOnly = true;
            this.nameBox.Size = new System.Drawing.Size( 210, 20 );
            this.nameBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 10, 103 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 34, 13 );
            this.label4.TabIndex = 8;
            this.label4.Text = "Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 10, 78 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 46, 13 );
            this.label3.TabIndex = 6;
            this.label3.Text = "Serial #:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 53 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 38, 13 );
            this.label2.TabIndex = 4;
            this.label2.Text = "Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 23 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 58, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Device &ID:";
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point( 205, 20 );
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size( 75, 23 );
            this.disconnectButton.TabIndex = 3;
            this.disconnectButton.Text = "&Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler( this.disconnectButton_Click );
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point( 125, 20 );
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size( 75, 23 );
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "&Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler( this.connectButton_Click );
            // 
            // deviceCombo
            // 
            this.deviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceCombo.FormattingEnabled = true;
            this.deviceCombo.Location = new System.Drawing.Point( 70, 20 );
            this.deviceCombo.Name = "deviceCombo";
            this.deviceCombo.Size = new System.Drawing.Size( 50, 21 );
            this.deviceCombo.TabIndex = 1;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add( this.videoSourcePlayer );
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point( 0, 145 );
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size( 710, 363 );
            this.mainPanel.TabIndex = 10;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler( this.timer_Tick );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 710, 530 );
            this.Controls.Add( this.mainPanel );
            this.Controls.Add( this.topPanel );
            this.Controls.Add( this.statusStrip );
            this.Name = "MainForm";
            this.Text = "Ximea Video Acqusition Sample";
            this.Load += new System.EventHandler( this.MainForm_Load );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.statusStrip.ResumeLayout( false );
            this.statusStrip.PerformLayout( );
            this.topPanel.ResumeLayout( false );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.offsetYUpDown ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.offsetXUpDown ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.heightUpDown ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.widthUpDown ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.gainUpDown ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.exposureUpDown ) ).EndInit( );
            this.mainPanel.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ComboBox deviceCombo;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.ToolStripStatusLabel spareLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TextBox typeBox;
        private System.Windows.Forms.TextBox snBox;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown exposureUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown heightUpDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown widthUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown gainUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown offsetYUpDown;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown offsetXUpDown;
        private System.Windows.Forms.Label label10;
    }
}

