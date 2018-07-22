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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.videoSourcePlayer = new Accord.Controls.VideoSourcePlayer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.spareLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.offsetYUpDown = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.offsetXUpDown = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.heightUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.widthUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.gainUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.exposureUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.typeBox = new System.Windows.Forms.TextBox();
            this.snBox = new System.Windows.Forms.TextBox();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.deviceCombo = new System.Windows.Forms.ComboBox();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.offsetYUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetXUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gainUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exposureUpDown)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.AutoSizeControl = true;
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoSourcePlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point(0, 0);
            this.videoSourcePlayer.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size(1216, 768);
            this.videoSourcePlayer.TabIndex = 7;
            this.videoSourcePlayer.VideoSource = null;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.spareLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 1065);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(3, 0, 28, 0);
            this.statusStrip.Size = new System.Drawing.Size(1216, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = false;
            this.fpsLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.fpsLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(100, 17);
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spareLabel
            // 
            this.spareLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.spareLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.spareLabel.Name = "spareLabel";
            this.spareLabel.Size = new System.Drawing.Size(1085, 17);
            this.spareLabel.Spring = true;
            this.spareLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.groupBox1);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1216, 297);
            this.topPanel.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.offsetYUpDown);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.offsetXUpDown);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.heightUpDown);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.widthUpDown);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.gainUpDown);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.exposureUpDown);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.typeBox);
            this.groupBox1.Controls.Add(this.snBox);
            this.groupBox1.Controls.Add(this.nameBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.disconnectButton);
            this.groupBox1.Controls.Add(this.connectButton);
            this.groupBox1.Controls.Add(this.deviceCombo);
            this.groupBox1.Location = new System.Drawing.Point(20, 20);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.groupBox1.Size = new System.Drawing.Size(1175, 264);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera parameters:";
            // 
            // offsetYUpDown
            // 
            this.offsetYUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.offsetYUpDown.Location = new System.Drawing.Point(1040, 103);
            this.offsetYUpDown.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.offsetYUpDown.Name = "offsetYUpDown";
            this.offsetYUpDown.Size = new System.Drawing.Size(124, 31);
            this.offsetYUpDown.TabIndex = 17;
            this.offsetYUpDown.ValueChanged += new System.EventHandler(this.offsetYUpDown_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(936, 109);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 25);
            this.label9.TabIndex = 16;
            this.label9.Text = "Offset Y:";
            // 
            // offsetXUpDown
            // 
            this.offsetXUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.offsetXUpDown.Location = new System.Drawing.Point(720, 103);
            this.offsetXUpDown.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.offsetXUpDown.Name = "offsetXUpDown";
            this.offsetXUpDown.Size = new System.Drawing.Size(124, 31);
            this.offsetXUpDown.TabIndex = 15;
            this.offsetXUpDown.ValueChanged += new System.EventHandler(this.offsetXUpDown_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(621, 109);
            this.label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 25);
            this.label10.TabIndex = 14;
            this.label10.Text = "Offset X:";
            // 
            // heightUpDown
            // 
            this.heightUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.heightUpDown.Location = new System.Drawing.Point(1040, 41);
            this.heightUpDown.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.heightUpDown.Name = "heightUpDown";
            this.heightUpDown.Size = new System.Drawing.Size(124, 31);
            this.heightUpDown.TabIndex = 13;
            this.heightUpDown.ValueChanged += new System.EventHandler(this.heightUpDown_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(949, 47);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 25);
            this.label8.TabIndex = 12;
            this.label8.Text = "Height:";
            // 
            // widthUpDown
            // 
            this.widthUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.widthUpDown.Location = new System.Drawing.Point(720, 41);
            this.widthUpDown.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.widthUpDown.Name = "widthUpDown";
            this.widthUpDown.Size = new System.Drawing.Size(124, 31);
            this.widthUpDown.TabIndex = 11;
            this.widthUpDown.ValueChanged += new System.EventHandler(this.widthUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(643, 47);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 25);
            this.label7.TabIndex = 10;
            this.label7.Text = "Width:";
            // 
            // gainUpDown
            // 
            this.gainUpDown.DecimalPlaces = 2;
            this.gainUpDown.Location = new System.Drawing.Point(720, 164);
            this.gainUpDown.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.gainUpDown.Name = "gainUpDown";
            this.gainUpDown.Size = new System.Drawing.Size(124, 31);
            this.gainUpDown.TabIndex = 19;
            this.gainUpDown.ValueChanged += new System.EventHandler(this.gainUpDown_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(611, 171);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 25);
            this.label6.TabIndex = 18;
            this.label6.Text = "Gain (dB):";
            // 
            // exposureUpDown
            // 
            this.exposureUpDown.DecimalPlaces = 2;
            this.exposureUpDown.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.exposureUpDown.Location = new System.Drawing.Point(1040, 164);
            this.exposureUpDown.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.exposureUpDown.Name = "exposureUpDown";
            this.exposureUpDown.Size = new System.Drawing.Size(124, 31);
            this.exposureUpDown.TabIndex = 21;
            this.exposureUpDown.ValueChanged += new System.EventHandler(this.exposureUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(880, 171);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(157, 25);
            this.label5.TabIndex = 20;
            this.label5.Text = "Exposure (ms):";
            // 
            // typeBox
            // 
            this.typeBox.Location = new System.Drawing.Point(140, 205);
            this.typeBox.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.typeBox.Name = "typeBox";
            this.typeBox.ReadOnly = true;
            this.typeBox.Size = new System.Drawing.Size(416, 31);
            this.typeBox.TabIndex = 9;
            // 
            // snBox
            // 
            this.snBox.Location = new System.Drawing.Point(140, 153);
            this.snBox.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.snBox.Name = "snBox";
            this.snBox.ReadOnly = true;
            this.snBox.Size = new System.Drawing.Size(416, 31);
            this.snBox.TabIndex = 7;
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(140, 103);
            this.nameBox.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.nameBox.Name = "nameBox";
            this.nameBox.ReadOnly = true;
            this.nameBox.Size = new System.Drawing.Size(416, 31);
            this.nameBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 211);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 160);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "Serial #:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 109);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 47);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Device &ID:";
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(411, 41);
            this.disconnectButton.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(149, 47);
            this.disconnectButton.TabIndex = 3;
            this.disconnectButton.Text = "&Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(251, 41);
            this.connectButton.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(149, 47);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "&Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // deviceCombo
            // 
            this.deviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceCombo.FormattingEnabled = true;
            this.deviceCombo.Location = new System.Drawing.Point(140, 41);
            this.deviceCombo.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.deviceCombo.Name = "deviceCombo";
            this.deviceCombo.Size = new System.Drawing.Size(96, 33);
            this.deviceCombo.TabIndex = 1;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.videoSourcePlayer);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 297);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1216, 768);
            this.mainPanel.TabIndex = 10;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1216, 1087);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.Name = "MainForm";
            this.Text = "Ximea Video Acquisition Sample";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.offsetYUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetXUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gainUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exposureUpDown)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Accord.Controls.VideoSourcePlayer videoSourcePlayer;
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

