namespace MotionDetectorSample
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MainForm ) );
            this.menuMenu = new System.Windows.Forms.MenuStrip( );
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openVideoFileusingDirectShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.localVideoCaptureDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openJPEGURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openMJPEGURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator( );
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.motionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.motionDetectionAlgorithmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.noneToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem( );
            this.twoFramesDifferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.simpleBackgroundModelingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.motionProcessingAlgorithmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.noneToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem( );
            this.motionAreaHighlightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.motionBorderHighlightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.blobCountingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.gridMotionAreaProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator( );
            this.defineMotionregionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator( );
            this.showMotionHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.localVideoCaptureSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.crossbarVideoSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog( );
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.statusBar = new System.Windows.Forms.StatusStrip( );
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.objectsCountLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.panel1 = new System.Windows.Forms.Panel( );
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer( );
            this.alarmTimer = new System.Windows.Forms.Timer( this.components );
            this.menuMenu.SuspendLayout( );
            this.statusBar.SuspendLayout( );
            this.panel1.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // menuMenu
            // 
            this.menuMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.motionToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem} );
            this.menuMenu.Location = new System.Drawing.Point( 0, 0 );
            this.menuMenu.Name = "menuMenu";
            this.menuMenu.Size = new System.Drawing.Size( 432, 24 );
            this.menuMenu.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.openVideoFileusingDirectShowToolStripMenuItem,
            this.localVideoCaptureDeviceToolStripMenuItem,
            this.openJPEGURLToolStripMenuItem,
            this.openMJPEGURLToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 37, 20 );
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openVideoFileusingDirectShowToolStripMenuItem
            // 
            this.openVideoFileusingDirectShowToolStripMenuItem.Name = "openVideoFileusingDirectShowToolStripMenuItem";
            this.openVideoFileusingDirectShowToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O ) ) );
            this.openVideoFileusingDirectShowToolStripMenuItem.Size = new System.Drawing.Size( 270, 22 );
            this.openVideoFileusingDirectShowToolStripMenuItem.Text = "&Open";
            this.openVideoFileusingDirectShowToolStripMenuItem.Click += new System.EventHandler( this.openVideoFileusingDirectShowToolStripMenuItem_Click );
            // 
            // localVideoCaptureDeviceToolStripMenuItem
            // 
            this.localVideoCaptureDeviceToolStripMenuItem.Name = "localVideoCaptureDeviceToolStripMenuItem";
            this.localVideoCaptureDeviceToolStripMenuItem.Size = new System.Drawing.Size( 270, 22 );
            this.localVideoCaptureDeviceToolStripMenuItem.Text = "Local &Video Capture Device";
            this.localVideoCaptureDeviceToolStripMenuItem.Click += new System.EventHandler( this.localVideoCaptureDeviceToolStripMenuItem_Click );
            // 
            // openJPEGURLToolStripMenuItem
            // 
            this.openJPEGURLToolStripMenuItem.Name = "openJPEGURLToolStripMenuItem";
            this.openJPEGURLToolStripMenuItem.Size = new System.Drawing.Size( 270, 22 );
            this.openJPEGURLToolStripMenuItem.Text = "Open JPEG &URL";
            this.openJPEGURLToolStripMenuItem.Click += new System.EventHandler( this.openJPEGURLToolStripMenuItem_Click );
            // 
            // openMJPEGURLToolStripMenuItem
            // 
            this.openMJPEGURLToolStripMenuItem.Name = "openMJPEGURLToolStripMenuItem";
            this.openMJPEGURLToolStripMenuItem.Size = new System.Drawing.Size( 270, 22 );
            this.openMJPEGURLToolStripMenuItem.Text = "Open &MJPEG URL";
            this.openMJPEGURLToolStripMenuItem.Click += new System.EventHandler( this.openMJPEGURLToolStripMenuItem_Click );
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size( 270, 22 );
            this.openToolStripMenuItem.Text = "Open video file (using VFW interface)";
            this.openToolStripMenuItem.Click += new System.EventHandler( this.openToolStripMenuItem_Click );
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size( 267, 6 );
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size( 270, 22 );
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler( this.exitToolStripMenuItem_Click );
            // 
            // motionToolStripMenuItem
            // 
            this.motionToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.motionDetectionAlgorithmToolStripMenuItem,
            this.motionProcessingAlgorithmToolStripMenuItem,
            this.toolStripMenuItem2,
            this.defineMotionregionsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.showMotionHistoryToolStripMenuItem} );
            this.motionToolStripMenuItem.Name = "motionToolStripMenuItem";
            this.motionToolStripMenuItem.Size = new System.Drawing.Size( 58, 20 );
            this.motionToolStripMenuItem.Text = "&Motion";
            this.motionToolStripMenuItem.DropDownOpening += new System.EventHandler( this.motionToolStripMenuItem_DropDownOpening );
            // 
            // motionDetectionAlgorithmToolStripMenuItem
            // 
            this.motionDetectionAlgorithmToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem1,
            this.twoFramesDifferenceToolStripMenuItem,
            this.simpleBackgroundModelingToolStripMenuItem} );
            this.motionDetectionAlgorithmToolStripMenuItem.Name = "motionDetectionAlgorithmToolStripMenuItem";
            this.motionDetectionAlgorithmToolStripMenuItem.Size = new System.Drawing.Size( 230, 22 );
            this.motionDetectionAlgorithmToolStripMenuItem.Text = "Motion Detection Algorithm";
            // 
            // noneToolStripMenuItem1
            // 
            this.noneToolStripMenuItem1.Name = "noneToolStripMenuItem1";
            this.noneToolStripMenuItem1.Size = new System.Drawing.Size( 231, 22 );
            this.noneToolStripMenuItem1.Text = "None";
            this.noneToolStripMenuItem1.Click += new System.EventHandler( this.noneToolStripMenuItem1_Click );
            // 
            // twoFramesDifferenceToolStripMenuItem
            // 
            this.twoFramesDifferenceToolStripMenuItem.Name = "twoFramesDifferenceToolStripMenuItem";
            this.twoFramesDifferenceToolStripMenuItem.Size = new System.Drawing.Size( 231, 22 );
            this.twoFramesDifferenceToolStripMenuItem.Text = "Two Frames Difference";
            this.twoFramesDifferenceToolStripMenuItem.Click += new System.EventHandler( this.twoFramesDifferenceToolStripMenuItem_Click );
            // 
            // simpleBackgroundModelingToolStripMenuItem
            // 
            this.simpleBackgroundModelingToolStripMenuItem.Name = "simpleBackgroundModelingToolStripMenuItem";
            this.simpleBackgroundModelingToolStripMenuItem.Size = new System.Drawing.Size( 231, 22 );
            this.simpleBackgroundModelingToolStripMenuItem.Text = "Simple Background Modeling";
            this.simpleBackgroundModelingToolStripMenuItem.Click += new System.EventHandler( this.simpleBackgroundModelingToolStripMenuItem_Click );
            // 
            // motionProcessingAlgorithmToolStripMenuItem
            // 
            this.motionProcessingAlgorithmToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem2,
            this.motionAreaHighlightingToolStripMenuItem,
            this.motionBorderHighlightingToolStripMenuItem,
            this.blobCountingToolStripMenuItem,
            this.gridMotionAreaProcessingToolStripMenuItem} );
            this.motionProcessingAlgorithmToolStripMenuItem.Name = "motionProcessingAlgorithmToolStripMenuItem";
            this.motionProcessingAlgorithmToolStripMenuItem.Size = new System.Drawing.Size( 230, 22 );
            this.motionProcessingAlgorithmToolStripMenuItem.Text = "Motion Processing Algorithm";
            // 
            // noneToolStripMenuItem2
            // 
            this.noneToolStripMenuItem2.Name = "noneToolStripMenuItem2";
            this.noneToolStripMenuItem2.Size = new System.Drawing.Size( 225, 22 );
            this.noneToolStripMenuItem2.Text = "None";
            this.noneToolStripMenuItem2.Click += new System.EventHandler( this.noneToolStripMenuItem2_Click );
            // 
            // motionAreaHighlightingToolStripMenuItem
            // 
            this.motionAreaHighlightingToolStripMenuItem.Name = "motionAreaHighlightingToolStripMenuItem";
            this.motionAreaHighlightingToolStripMenuItem.Size = new System.Drawing.Size( 225, 22 );
            this.motionAreaHighlightingToolStripMenuItem.Text = "Motion Area Highlighting";
            this.motionAreaHighlightingToolStripMenuItem.Click += new System.EventHandler( this.motionAreaHighlightingToolStripMenuItem_Click );
            // 
            // motionBorderHighlightingToolStripMenuItem
            // 
            this.motionBorderHighlightingToolStripMenuItem.Name = "motionBorderHighlightingToolStripMenuItem";
            this.motionBorderHighlightingToolStripMenuItem.Size = new System.Drawing.Size( 225, 22 );
            this.motionBorderHighlightingToolStripMenuItem.Text = "Motion Border Highlighting";
            this.motionBorderHighlightingToolStripMenuItem.Click += new System.EventHandler( this.motionBorderHighlightingToolStripMenuItem_Click );
            // 
            // blobCountingToolStripMenuItem
            // 
            this.blobCountingToolStripMenuItem.Name = "blobCountingToolStripMenuItem";
            this.blobCountingToolStripMenuItem.Size = new System.Drawing.Size( 225, 22 );
            this.blobCountingToolStripMenuItem.Text = "Blob Counting Processing";
            this.blobCountingToolStripMenuItem.Click += new System.EventHandler( this.blobCountingToolStripMenuItem_Click );
            // 
            // gridMotionAreaProcessingToolStripMenuItem
            // 
            this.gridMotionAreaProcessingToolStripMenuItem.Name = "gridMotionAreaProcessingToolStripMenuItem";
            this.gridMotionAreaProcessingToolStripMenuItem.Size = new System.Drawing.Size( 225, 22 );
            this.gridMotionAreaProcessingToolStripMenuItem.Text = "Grid Motion Area Processing";
            this.gridMotionAreaProcessingToolStripMenuItem.Click += new System.EventHandler( this.gridMotionAreaProcessingToolStripMenuItem_Click );
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size( 227, 6 );
            // 
            // defineMotionregionsToolStripMenuItem
            // 
            this.defineMotionregionsToolStripMenuItem.Name = "defineMotionregionsToolStripMenuItem";
            this.defineMotionregionsToolStripMenuItem.Size = new System.Drawing.Size( 230, 22 );
            this.defineMotionregionsToolStripMenuItem.Text = "Define motion &regions";
            this.defineMotionregionsToolStripMenuItem.Click += new System.EventHandler( this.defineMotionregionsToolStripMenuItem_Click );
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size( 227, 6 );
            // 
            // showMotionHistoryToolStripMenuItem
            // 
            this.showMotionHistoryToolStripMenuItem.Name = "showMotionHistoryToolStripMenuItem";
            this.showMotionHistoryToolStripMenuItem.Size = new System.Drawing.Size( 230, 22 );
            this.showMotionHistoryToolStripMenuItem.Text = "Show motion history";
            this.showMotionHistoryToolStripMenuItem.Click += new System.EventHandler( this.showMotionHistoryToolStripMenuItem_Click );
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.localVideoCaptureSettingsToolStripMenuItem,
            this.crossbarVideoSettingsToolStripMenuItem} );
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size( 48, 20 );
            this.toolsToolStripMenuItem.Text = "&Tools";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler( this.toolsToolStripMenuItem_DropDownOpening );
            // 
            // localVideoCaptureSettingsToolStripMenuItem
            // 
            this.localVideoCaptureSettingsToolStripMenuItem.Name = "localVideoCaptureSettingsToolStripMenuItem";
            this.localVideoCaptureSettingsToolStripMenuItem.Size = new System.Drawing.Size( 225, 22 );
            this.localVideoCaptureSettingsToolStripMenuItem.Text = "Local &Video Capture Settings";
            this.localVideoCaptureSettingsToolStripMenuItem.Click += new System.EventHandler( this.localVideoCaptureSettingsToolStripMenuItem_Click );
            // 
            // crossbarVideoSettingsToolStripMenuItem
            // 
            this.crossbarVideoSettingsToolStripMenuItem.Name = "crossbarVideoSettingsToolStripMenuItem";
            this.crossbarVideoSettingsToolStripMenuItem.Size = new System.Drawing.Size( 225, 22 );
            this.crossbarVideoSettingsToolStripMenuItem.Text = "Crossbar Video Settings";
            this.crossbarVideoSettingsToolStripMenuItem.Click += new System.EventHandler( this.crossbarVideoSettingsToolStripMenuItem_Click );
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem} );
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size( 44, 20 );
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size( 107, 22 );
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler( this.aboutToolStripMenuItem_Click );
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "AVI files (*.avi)|*.avi|All files (*.*)|*.*";
            this.openFileDialog.Title = "Opem movie";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler( this.timer_Tick );
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.objectsCountLabel} );
            this.statusBar.Location = new System.Drawing.Point( 0, 334 );
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size( 432, 22 );
            this.statusBar.TabIndex = 3;
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = false;
            this.fpsLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.fpsLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size( 150, 17 );
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // objectsCountLabel
            // 
            this.objectsCountLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.objectsCountLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.objectsCountLabel.Name = "objectsCountLabel";
            this.objectsCountLabel.Size = new System.Drawing.Size( 267, 17 );
            this.objectsCountLabel.Spring = true;
            this.objectsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.videoSourcePlayer );
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point( 0, 24 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 432, 310 );
            this.panel1.TabIndex = 4;
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.AutoSizeControl = true;
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point( 55, 34 );
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size( 322, 242 );
            this.videoSourcePlayer.TabIndex = 0;
            this.videoSourcePlayer.VideoSource = null;
            this.videoSourcePlayer.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler( this.videoSourcePlayer_NewFrame );
            // 
            // alarmTimer
            // 
            this.alarmTimer.Interval = 200;
            this.alarmTimer.Tick += new System.EventHandler( this.alarmTimer_Tick );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 432, 356 );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.statusBar );
            this.Controls.Add( this.menuMenu );
            this.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "$this.Icon" ) ) );
            this.MainMenuStrip = this.menuMenu;
            this.Name = "MainForm";
            this.Text = "Motion Detector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.menuMenu.ResumeLayout( false );
            this.menuMenu.PerformLayout( );
            this.statusBar.ResumeLayout( false );
            this.statusBar.PerformLayout( );
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem openJPEGURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMJPEGURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem localVideoCaptureDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openVideoFileusingDirectShowToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel objectsCountLabel;
        private System.Windows.Forms.ToolStripMenuItem defineMotionregionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localVideoCaptureSettingsToolStripMenuItem;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.ToolStripMenuItem motionDetectionAlgorithmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem twoFramesDifferenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simpleBackgroundModelingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motionProcessingAlgorithmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem motionBorderHighlightingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blobCountingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridMotionAreaProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motionAreaHighlightingToolStripMenuItem;
        private System.Windows.Forms.Timer alarmTimer;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem showMotionHistoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crossbarVideoSettingsToolStripMenuItem;
    }
}

