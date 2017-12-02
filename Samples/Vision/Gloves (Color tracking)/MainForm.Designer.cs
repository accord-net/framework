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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openVideoFileusingDirectShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localVideoCaptureDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openJPEGURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMJPEGURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localVideoCaptureSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.videoSourcePlayer = new Accord.Controls.VideoSourcePlayer();
            this.cbContour = new System.Windows.Forms.CheckBox();
            this.cbFingertips = new System.Windows.Forms.CheckBox();
            this.cbContainer = new System.Windows.Forms.CheckBox();
            this.tbSensitivity = new System.Windows.Forms.TrackBar();
            this.cbAngle = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.menuMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSensitivity)).BeginInit();
            this.SuspendLayout();
            // 
            // menuMenu
            // 
            this.menuMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuMenu.Location = new System.Drawing.Point(0, 0);
            this.menuMenu.Name = "menuMenu";
            this.menuMenu.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuMenu.Size = new System.Drawing.Size(706, 35);
            this.menuMenu.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openVideoFileusingDirectShowToolStripMenuItem,
            this.localVideoCaptureDeviceToolStripMenuItem,
            this.openJPEGURLToolStripMenuItem,
            this.openMJPEGURLToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openVideoFileusingDirectShowToolStripMenuItem
            // 
            this.openVideoFileusingDirectShowToolStripMenuItem.Name = "openVideoFileusingDirectShowToolStripMenuItem";
            this.openVideoFileusingDirectShowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openVideoFileusingDirectShowToolStripMenuItem.Size = new System.Drawing.Size(377, 30);
            this.openVideoFileusingDirectShowToolStripMenuItem.Text = "&Open";
            this.openVideoFileusingDirectShowToolStripMenuItem.Click += new System.EventHandler(this.openVideoFileusingDirectShowToolStripMenuItem_Click);
            // 
            // localVideoCaptureDeviceToolStripMenuItem
            // 
            this.localVideoCaptureDeviceToolStripMenuItem.Name = "localVideoCaptureDeviceToolStripMenuItem";
            this.localVideoCaptureDeviceToolStripMenuItem.Size = new System.Drawing.Size(377, 30);
            this.localVideoCaptureDeviceToolStripMenuItem.Text = "Local &Video Capture Device";
            this.localVideoCaptureDeviceToolStripMenuItem.Click += new System.EventHandler(this.localVideoCaptureDeviceToolStripMenuItem_Click);
            // 
            // openJPEGURLToolStripMenuItem
            // 
            this.openJPEGURLToolStripMenuItem.Name = "openJPEGURLToolStripMenuItem";
            this.openJPEGURLToolStripMenuItem.Size = new System.Drawing.Size(377, 30);
            this.openJPEGURLToolStripMenuItem.Text = "Open JPEG &URL";
            this.openJPEGURLToolStripMenuItem.Click += new System.EventHandler(this.openJPEGURLToolStripMenuItem_Click);
            // 
            // openMJPEGURLToolStripMenuItem
            // 
            this.openMJPEGURLToolStripMenuItem.Name = "openMJPEGURLToolStripMenuItem";
            this.openMJPEGURLToolStripMenuItem.Size = new System.Drawing.Size(377, 30);
            this.openMJPEGURLToolStripMenuItem.Text = "Open &MJPEG URL";
            this.openMJPEGURLToolStripMenuItem.Click += new System.EventHandler(this.openMJPEGURLToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(377, 30);
            this.openToolStripMenuItem.Text = "Open video file (using VFW interface)";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(374, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(377, 30);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localVideoCaptureSettingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(67, 29);
            this.toolsToolStripMenuItem.Text = "&Tools";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.toolsToolStripMenuItem_DropDownOpening);
            // 
            // localVideoCaptureSettingsToolStripMenuItem
            // 
            this.localVideoCaptureSettingsToolStripMenuItem.Name = "localVideoCaptureSettingsToolStripMenuItem";
            this.localVideoCaptureSettingsToolStripMenuItem.Size = new System.Drawing.Size(311, 30);
            this.localVideoCaptureSettingsToolStripMenuItem.Text = "Local &Video Capture Settings";
            this.localVideoCaptureSettingsToolStripMenuItem.Click += new System.EventHandler(this.localVideoCaptureSettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(134, 30);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "AVI files (*.avi)|*.avi|All files (*.*)|*.*";
            this.openFileDialog.Title = "Opem movie";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.videoSourcePlayer);
            this.panel1.Location = new System.Drawing.Point(0, 36);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(572, 526);
            this.panel1.TabIndex = 4;
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoSourcePlayer.AutoSizeControl = true;
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point(125, 142);
            this.videoSourcePlayer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size(322, 242);
            this.videoSourcePlayer.TabIndex = 0;
            this.videoSourcePlayer.VideoSource = null;
            this.videoSourcePlayer.NewFrameReceived += new Accord.Video.NewFrameEventHandler(this.videoSourcePlayer_NewFrame);
            // 
            // cbContour
            // 
            this.cbContour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbContour.AutoSize = true;
            this.cbContour.Location = new System.Drawing.Point(582, 42);
            this.cbContour.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbContour.Name = "cbContour";
            this.cbContour.Size = new System.Drawing.Size(92, 24);
            this.cbContour.TabIndex = 5;
            this.cbContour.Text = "Contour";
            this.cbContour.UseVisualStyleBackColor = true;
            this.cbContour.CheckedChanged += new System.EventHandler(this.cbContour_CheckedChanged);
            // 
            // cbFingertips
            // 
            this.cbFingertips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFingertips.AutoSize = true;
            this.cbFingertips.Location = new System.Drawing.Point(581, 77);
            this.cbFingertips.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbFingertips.Name = "cbFingertips";
            this.cbFingertips.Size = new System.Drawing.Size(105, 24);
            this.cbFingertips.TabIndex = 5;
            this.cbFingertips.Text = "Fingertips";
            this.cbFingertips.UseVisualStyleBackColor = true;
            this.cbFingertips.CheckedChanged += new System.EventHandler(this.cbContour_CheckedChanged);
            // 
            // cbContainer
            // 
            this.cbContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbContainer.AutoSize = true;
            this.cbContainer.Location = new System.Drawing.Point(582, 112);
            this.cbContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbContainer.Name = "cbContainer";
            this.cbContainer.Size = new System.Drawing.Size(104, 24);
            this.cbContainer.TabIndex = 5;
            this.cbContainer.Text = "Container";
            this.cbContainer.UseVisualStyleBackColor = true;
            this.cbContainer.CheckedChanged += new System.EventHandler(this.cbContour_CheckedChanged);
            // 
            // tbSensitivity
            // 
            this.tbSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSensitivity.Location = new System.Drawing.Point(603, 184);
            this.tbSensitivity.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbSensitivity.Maximum = 255;
            this.tbSensitivity.Minimum = 1;
            this.tbSensitivity.Name = "tbSensitivity";
            this.tbSensitivity.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbSensitivity.Size = new System.Drawing.Size(69, 309);
            this.tbSensitivity.TabIndex = 6;
            this.tbSensitivity.Value = 1;
            this.tbSensitivity.Scroll += new System.EventHandler(this.tbSensitivity_Scroll);
            // 
            // cbAngle
            // 
            this.cbAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAngle.AutoSize = true;
            this.cbAngle.Location = new System.Drawing.Point(580, 144);
            this.cbAngle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbAngle.Name = "cbAngle";
            this.cbAngle.Size = new System.Drawing.Size(76, 24);
            this.cbAngle.TabIndex = 5;
            this.cbAngle.Text = "Angle";
            this.cbAngle.UseVisualStyleBackColor = true;
            this.cbAngle.CheckedChanged += new System.EventHandler(this.cbContour_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(580, 510);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 34);
            this.button1.TabIndex = 7;
            this.button1.Text = "Set color";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(706, 562);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbSensitivity);
            this.Controls.Add(this.cbAngle);
            this.Controls.Add(this.cbContainer);
            this.Controls.Add(this.cbFingertips);
            this.Controls.Add(this.cbContour);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMenu;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Glove Tracking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuMenu.ResumeLayout(false);
            this.menuMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbSensitivity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem openJPEGURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMJPEGURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localVideoCaptureDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openVideoFileusingDirectShowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localVideoCaptureSettingsToolStripMenuItem;
        private Accord.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.CheckBox cbContour;
        private System.Windows.Forms.CheckBox cbFingertips;
        private System.Windows.Forms.CheckBox cbContainer;
        private System.Windows.Forms.TrackBar tbSensitivity;
        private System.Windows.Forms.CheckBox cbAngle;
        private System.Windows.Forms.Button button1;
    }
}

