namespace FaceTracking
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
            this.menuMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openVideoFileusingDirectShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localVideoCaptureDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openJPEGURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMJPEGURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawTrackingWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawObjectAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawObjectBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.displayBackprojectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.defineObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodetectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rGBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hSLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mixedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localVideoCaptureSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.objectsCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.menuMenu.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMenu
            // 
            this.menuMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.trackingToolStripMenuItem,
            this.colorSpaceToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuMenu.Location = new System.Drawing.Point(0, 0);
            this.menuMenu.Name = "menuMenu";
            this.menuMenu.Size = new System.Drawing.Size(432, 24);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openVideoFileusingDirectShowToolStripMenuItem
            // 
            this.openVideoFileusingDirectShowToolStripMenuItem.Name = "openVideoFileusingDirectShowToolStripMenuItem";
            this.openVideoFileusingDirectShowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openVideoFileusingDirectShowToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.openVideoFileusingDirectShowToolStripMenuItem.Text = "&Open";
            this.openVideoFileusingDirectShowToolStripMenuItem.Click += new System.EventHandler(this.openVideoFileusingDirectShowToolStripMenuItem_Click);
            // 
            // localVideoCaptureDeviceToolStripMenuItem
            // 
            this.localVideoCaptureDeviceToolStripMenuItem.Name = "localVideoCaptureDeviceToolStripMenuItem";
            this.localVideoCaptureDeviceToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.localVideoCaptureDeviceToolStripMenuItem.Text = "Local &Video Capture Device";
            this.localVideoCaptureDeviceToolStripMenuItem.Click += new System.EventHandler(this.localVideoCaptureDeviceToolStripMenuItem_Click);
            // 
            // openJPEGURLToolStripMenuItem
            // 
            this.openJPEGURLToolStripMenuItem.Name = "openJPEGURLToolStripMenuItem";
            this.openJPEGURLToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.openJPEGURLToolStripMenuItem.Text = "Open JPEG &URL";
            this.openJPEGURLToolStripMenuItem.Click += new System.EventHandler(this.openJPEGURLToolStripMenuItem_Click);
            // 
            // openMJPEGURLToolStripMenuItem
            // 
            this.openMJPEGURLToolStripMenuItem.Name = "openMJPEGURLToolStripMenuItem";
            this.openMJPEGURLToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.openMJPEGURLToolStripMenuItem.Text = "Open &MJPEG URL";
            this.openMJPEGURLToolStripMenuItem.Click += new System.EventHandler(this.openMJPEGURLToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.openToolStripMenuItem.Text = "Open video file (using VFW interface)";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(267, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // trackingToolStripMenuItem
            // 
            this.trackingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawTrackingWindowToolStripMenuItem,
            this.drawObjectAxisToolStripMenuItem,
            this.drawObjectBoxToolStripMenuItem,
            this.toolStripSeparator1,
            this.displayBackprojectionToolStripMenuItem,
            this.toolStripSeparator2,
            this.defineObjectToolStripMenuItem,
            this.autodetectToolStripMenuItem});
            this.trackingToolStripMenuItem.Name = "trackingToolStripMenuItem";
            this.trackingToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.trackingToolStripMenuItem.Text = "Tracking";
            // 
            // drawTrackingWindowToolStripMenuItem
            // 
            this.drawTrackingWindowToolStripMenuItem.Name = "drawTrackingWindowToolStripMenuItem";
            this.drawTrackingWindowToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.drawTrackingWindowToolStripMenuItem.Text = "Show tracking window";
            this.drawTrackingWindowToolStripMenuItem.Click += new System.EventHandler(this.drawTrackingWindowToolStripMenuItem_Click);
            // 
            // drawObjectAxisToolStripMenuItem
            // 
            this.drawObjectAxisToolStripMenuItem.Name = "drawObjectAxisToolStripMenuItem";
            this.drawObjectAxisToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.drawObjectAxisToolStripMenuItem.Text = "Show object axis";
            this.drawObjectAxisToolStripMenuItem.Click += new System.EventHandler(this.drawObjectAxisToolStripMenuItem_Click);
            // 
            // drawObjectBoxToolStripMenuItem
            // 
            this.drawObjectBoxToolStripMenuItem.Checked = true;
            this.drawObjectBoxToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.drawObjectBoxToolStripMenuItem.Name = "drawObjectBoxToolStripMenuItem";
            this.drawObjectBoxToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.drawObjectBoxToolStripMenuItem.Text = "Show object bounding box";
            this.drawObjectBoxToolStripMenuItem.Click += new System.EventHandler(this.drawObjectBoxToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(239, 6);
            // 
            // displayBackprojectionToolStripMenuItem
            // 
            this.displayBackprojectionToolStripMenuItem.Name = "displayBackprojectionToolStripMenuItem";
            this.displayBackprojectionToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.displayBackprojectionToolStripMenuItem.Text = "Show histogram backprojection";
            this.displayBackprojectionToolStripMenuItem.Click += new System.EventHandler(this.displayBackprojectionToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(239, 6);
            // 
            // defineObjectToolStripMenuItem
            // 
            this.defineObjectToolStripMenuItem.Name = "defineObjectToolStripMenuItem";
            this.defineObjectToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.defineObjectToolStripMenuItem.Text = "Define object";
            this.defineObjectToolStripMenuItem.Click += new System.EventHandler(this.defineObjectToolStripMenuItem_Click);
            // 
            // autodetectToolStripMenuItem
            // 
            this.autodetectToolStripMenuItem.Name = "autodetectToolStripMenuItem";
            this.autodetectToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.autodetectToolStripMenuItem.Text = "Detect faces";
            this.autodetectToolStripMenuItem.Click += new System.EventHandler(this.autodetectToolStripMenuItem_Click);
            // 
            // colorSpaceToolStripMenuItem
            // 
            this.colorSpaceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rGBToolStripMenuItem,
            this.hSLToolStripMenuItem,
            this.mixedToolStripMenuItem});
            this.colorSpaceToolStripMenuItem.Name = "colorSpaceToolStripMenuItem";
            this.colorSpaceToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.colorSpaceToolStripMenuItem.Text = "Color space";
            // 
            // rGBToolStripMenuItem
            // 
            this.rGBToolStripMenuItem.Checked = true;
            this.rGBToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rGBToolStripMenuItem.Name = "rGBToolStripMenuItem";
            this.rGBToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.rGBToolStripMenuItem.Text = "RGB";
            this.rGBToolStripMenuItem.Click += new System.EventHandler(this.rGBToolStripMenuItem_Click);
            // 
            // hSLToolStripMenuItem
            // 
            this.hSLToolStripMenuItem.Name = "hSLToolStripMenuItem";
            this.hSLToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.hSLToolStripMenuItem.Text = "HSL";
            this.hSLToolStripMenuItem.Click += new System.EventHandler(this.hSLToolStripMenuItem_Click);
            // 
            // mixedToolStripMenuItem
            // 
            this.mixedToolStripMenuItem.Name = "mixedToolStripMenuItem";
            this.mixedToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.mixedToolStripMenuItem.Text = "Mixed";
            this.mixedToolStripMenuItem.Click += new System.EventHandler(this.mixedToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localVideoCaptureSettingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.toolsToolStripMenuItem_DropDownOpening);
            // 
            // localVideoCaptureSettingsToolStripMenuItem
            // 
            this.localVideoCaptureSettingsToolStripMenuItem.Name = "localVideoCaptureSettingsToolStripMenuItem";
            this.localVideoCaptureSettingsToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.localVideoCaptureSettingsToolStripMenuItem.Text = "Local &Video Capture Settings";
            this.localVideoCaptureSettingsToolStripMenuItem.Click += new System.EventHandler(this.localVideoCaptureSettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "AVI files (*.avi)|*.avi|All files (*.*)|*.*";
            this.openFileDialog.Title = "Opem movie";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.objectsCountLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 332);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(432, 24);
            this.statusBar.TabIndex = 3;
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = false;
            this.fpsLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.fpsLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(150, 19);
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // objectsCountLabel
            // 
            this.objectsCountLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.objectsCountLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.objectsCountLabel.Name = "objectsCountLabel";
            this.objectsCountLabel.Size = new System.Drawing.Size(267, 19);
            this.objectsCountLabel.Spring = true;
            this.objectsCountLabel.Text = "Please select a capture device!";
            this.objectsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.videoSourcePlayer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(432, 308);
            this.panel1.TabIndex = 4;
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.AutoSizeControl = true;
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point(55, 33);
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size(322, 242);
            this.videoSourcePlayer.TabIndex = 0;
            this.videoSourcePlayer.VideoSource = null;
            this.videoSourcePlayer.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer_NewFrame);
            this.videoSourcePlayer.Click += new System.EventHandler(this.videoSourcePlayer_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 356);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMenu;
            this.Name = "MainForm";
            this.Text = "Face Tracking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuMenu.ResumeLayout(false);
            this.menuMenu.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem openJPEGURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMJPEGURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localVideoCaptureDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openVideoFileusingDirectShowToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel objectsCountLabel;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localVideoCaptureSettingsToolStripMenuItem;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.ToolStripMenuItem trackingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawTrackingWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawObjectAxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayBackprojectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawObjectBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defineObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodetectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rGBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hSLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mixedToolStripMenuItem;
    }
}

