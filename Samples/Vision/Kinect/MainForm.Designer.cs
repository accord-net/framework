namespace KinectController
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tbHorizontal = new System.Windows.Forms.TrackBar();
            this.tbVertical = new System.Windows.Forms.TrackBar();
            this.tbScale = new System.Windows.Forms.TrackBar();
            this.button2 = new System.Windows.Forms.Button();
            this.btnHorizontalMin = new System.Windows.Forms.Button();
            this.btnHorizontalMax = new System.Windows.Forms.Button();
            this.btnScaleMin = new System.Windows.Forms.Button();
            this.btnScaleMax = new System.Windows.Forms.Button();
            this.btnVerticalMin = new System.Windows.Forms.Button();
            this.btnVerticalMax = new System.Windows.Forms.Button();
            this.btnAngleMin = new System.Windows.Forms.Button();
            this.btnAngleMax = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.depthVisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackerVisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.faceControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
            this.pbLeftArm = new AForge.Controls.PictureBox();
            this.pbRightArm = new AForge.Controls.PictureBox();
            this.pictureBox1 = new AForge.Controls.PictureBox();
            this.pbLeftHand = new AForge.Controls.PictureBox();
            this.pbRightHand = new AForge.Controls.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.angleBox1 = new Accord.Controls.AngleBox();
            this.controller = new Accord.Controls.Vision.HeadController();
            ((System.ComponentModel.ISupportInitialize)(this.tbHorizontal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVertical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbScale)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftArm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightArm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftHand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightHand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tbHorizontal
            // 
            this.tbHorizontal.Location = new System.Drawing.Point(63, 266);
            this.tbHorizontal.Maximum = 20;
            this.tbHorizontal.Minimum = -20;
            this.tbHorizontal.Name = "tbHorizontal";
            this.tbHorizontal.Size = new System.Drawing.Size(150, 45);
            this.tbHorizontal.TabIndex = 0;
            // 
            // tbVertical
            // 
            this.tbVertical.Location = new System.Drawing.Point(219, 67);
            this.tbVertical.Maximum = 20;
            this.tbVertical.Minimum = -20;
            this.tbVertical.Name = "tbVertical";
            this.tbVertical.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbVertical.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbVertical.Size = new System.Drawing.Size(45, 147);
            this.tbVertical.TabIndex = 0;
            // 
            // tbScale
            // 
            this.tbScale.Location = new System.Drawing.Point(12, 60);
            this.tbScale.Maximum = 20;
            this.tbScale.Minimum = -20;
            this.tbScale.Name = "tbScale";
            this.tbScale.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbScale.Size = new System.Drawing.Size(45, 154);
            this.tbScale.TabIndex = 0;
            this.tbScale.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(95, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnHorizontalMin
            // 
            this.btnHorizontalMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHorizontalMin.Location = new System.Drawing.Point(12, 266);
            this.btnHorizontalMin.Name = "btnHorizontalMin";
            this.btnHorizontalMin.Size = new System.Drawing.Size(45, 45);
            this.btnHorizontalMin.TabIndex = 1;
            this.btnHorizontalMin.Text = "Set\r\nLeft";
            this.btnHorizontalMin.UseVisualStyleBackColor = true;
            this.btnHorizontalMin.Click += new System.EventHandler(this.btnHorizontalMin_Click);
            // 
            // btnHorizontalMax
            // 
            this.btnHorizontalMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHorizontalMax.Location = new System.Drawing.Point(219, 259);
            this.btnHorizontalMax.Name = "btnHorizontalMax";
            this.btnHorizontalMax.Size = new System.Drawing.Size(45, 45);
            this.btnHorizontalMax.TabIndex = 1;
            this.btnHorizontalMax.Text = "Set\r\nRight";
            this.btnHorizontalMax.UseVisualStyleBackColor = true;
            this.btnHorizontalMax.Click += new System.EventHandler(this.btnHorizontalMax_Click);
            // 
            // btnScaleMin
            // 
            this.btnScaleMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScaleMin.Location = new System.Drawing.Point(12, 224);
            this.btnScaleMin.Name = "btnScaleMin";
            this.btnScaleMin.Size = new System.Drawing.Size(45, 33);
            this.btnScaleMin.TabIndex = 1;
            this.btnScaleMin.Text = "Scale\r\nMin";
            this.btnScaleMin.UseVisualStyleBackColor = true;
            this.btnScaleMin.Click += new System.EventHandler(this.btnScaleMin_Click);
            // 
            // btnScaleMax
            // 
            this.btnScaleMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScaleMax.Location = new System.Drawing.Point(12, 31);
            this.btnScaleMax.Name = "btnScaleMax";
            this.btnScaleMax.Size = new System.Drawing.Size(45, 33);
            this.btnScaleMax.TabIndex = 1;
            this.btnScaleMax.Text = "Scale\r\nMax";
            this.btnScaleMax.UseVisualStyleBackColor = true;
            this.btnScaleMax.Click += new System.EventHandler(this.btnScaleMax_Click);
            // 
            // btnVerticalMin
            // 
            this.btnVerticalMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerticalMin.Location = new System.Drawing.Point(219, 220);
            this.btnVerticalMin.Name = "btnVerticalMin";
            this.btnVerticalMin.Size = new System.Drawing.Size(45, 33);
            this.btnVerticalMin.TabIndex = 1;
            this.btnVerticalMin.Text = "Set\r\nBottom";
            this.btnVerticalMin.UseVisualStyleBackColor = true;
            this.btnVerticalMin.Click += new System.EventHandler(this.btnVerticalMin_Click);
            // 
            // btnVerticalMax
            // 
            this.btnVerticalMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerticalMax.Location = new System.Drawing.Point(219, 27);
            this.btnVerticalMax.Name = "btnVerticalMax";
            this.btnVerticalMax.Size = new System.Drawing.Size(45, 33);
            this.btnVerticalMax.TabIndex = 1;
            this.btnVerticalMax.Text = "Set\r\nTop";
            this.btnVerticalMax.UseVisualStyleBackColor = true;
            this.btnVerticalMax.Click += new System.EventHandler(this.btnVerticalMax_Click);
            // 
            // btnAngleMin
            // 
            this.btnAngleMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAngleMin.Location = new System.Drawing.Point(63, 220);
            this.btnAngleMin.Name = "btnAngleMin";
            this.btnAngleMin.Size = new System.Drawing.Size(40, 40);
            this.btnAngleMin.TabIndex = 1;
            this.btnAngleMin.Text = "Left Tilt";
            this.btnAngleMin.UseVisualStyleBackColor = true;
            this.btnAngleMin.Click += new System.EventHandler(this.btnAngleMin_Click);
            // 
            // btnAngleMax
            // 
            this.btnAngleMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAngleMax.Location = new System.Drawing.Point(173, 220);
            this.btnAngleMax.Name = "btnAngleMax";
            this.btnAngleMax.Size = new System.Drawing.Size(40, 40);
            this.btnAngleMax.TabIndex = 1;
            this.btnAngleMax.Text = "Right Tilt";
            this.btnAngleMax.UseVisualStyleBackColor = true;
            this.btnAngleMax.Click += new System.EventHandler(this.btnAngleMax_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 385);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(989, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.showToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(989, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectCameraToolStripMenuItem,
            this.saveImagesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // selectCameraToolStripMenuItem
            // 
            this.selectCameraToolStripMenuItem.Name = "selectCameraToolStripMenuItem";
            this.selectCameraToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.selectCameraToolStripMenuItem.Text = "Select camera";
            this.selectCameraToolStripMenuItem.Click += new System.EventHandler(this.btnSelectCamera_Click);
            // 
            // saveImagesToolStripMenuItem
            // 
            this.saveImagesToolStripMenuItem.Name = "saveImagesToolStripMenuItem";
            this.saveImagesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveImagesToolStripMenuItem.Text = "Save images";
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.depthVisionToolStripMenuItem,
            this.trackerVisionToolStripMenuItem,
            this.faceControlsToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.showToolStripMenuItem.Text = "Show";
            // 
            // depthVisionToolStripMenuItem
            // 
            this.depthVisionToolStripMenuItem.Name = "depthVisionToolStripMenuItem";
            this.depthVisionToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.depthVisionToolStripMenuItem.Text = "Depth vision";
            // 
            // trackerVisionToolStripMenuItem
            // 
            this.trackerVisionToolStripMenuItem.Name = "trackerVisionToolStripMenuItem";
            this.trackerVisionToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.trackerVisionToolStripMenuItem.Text = "Tracker vision";
            this.trackerVisionToolStripMenuItem.Click += new System.EventHandler(this.btnTrackerVision_Click);
            // 
            // faceControlsToolStripMenuItem
            // 
            this.faceControlsToolStripMenuItem.Name = "faceControlsToolStripMenuItem";
            this.faceControlsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.faceControlsToolStripMenuItem.Text = "Face control (experimental)";
            this.faceControlsToolStripMenuItem.Click += new System.EventHandler(this.btnFaceControls_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.Location = new System.Drawing.Point(270, 31);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(351, 280);
            this.videoSourcePlayer1.TabIndex = 5;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            this.videoSourcePlayer1.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer1_NewFrame);
            // 
            // pbLeftArm
            // 
            this.pbLeftArm.Image = null;
            this.pbLeftArm.Location = new System.Drawing.Point(270, 317);
            this.pbLeftArm.Name = "pbLeftArm";
            this.pbLeftArm.Size = new System.Drawing.Size(63, 61);
            this.pbLeftArm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLeftArm.TabIndex = 6;
            this.pbLeftArm.TabStop = false;
            // 
            // pbRightArm
            // 
            this.pbRightArm.Image = null;
            this.pbRightArm.Location = new System.Drawing.Point(339, 317);
            this.pbRightArm.Name = "pbRightArm";
            this.pbRightArm.Size = new System.Drawing.Size(63, 61);
            this.pbRightArm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbRightArm.TabIndex = 6;
            this.pbRightArm.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = null;
            this.pictureBox1.Location = new System.Drawing.Point(558, 317);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(63, 61);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pbLeftHand
            // 
            this.pbLeftHand.Image = null;
            this.pbLeftHand.Location = new System.Drawing.Point(488, 317);
            this.pbLeftHand.Name = "pbLeftHand";
            this.pbLeftHand.Size = new System.Drawing.Size(63, 61);
            this.pbLeftHand.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLeftHand.TabIndex = 6;
            this.pbLeftHand.TabStop = false;
            // 
            // pbRightHand
            // 
            this.pbRightHand.Image = null;
            this.pbRightHand.Location = new System.Drawing.Point(419, 317);
            this.pbRightHand.Name = "pbRightHand";
            this.pbRightHand.Size = new System.Drawing.Size(63, 61);
            this.pbRightHand.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbRightHand.TabIndex = 6;
            this.pbRightHand.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(627, 31);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(350, 280);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // angleBox1
            // 
            this.angleBox1.Angle = 0F;
            this.angleBox1.Location = new System.Drawing.Point(63, 64);
            this.angleBox1.Name = "angleBox1";
            this.angleBox1.Size = new System.Drawing.Size(150, 150);
            this.angleBox1.TabIndex = 2;
            // 
            // controller
            // 
            this.controller.AngleMax = global::KinectController.Properties.Settings.Default.angleMax;
            this.controller.AngleMin = global::KinectController.Properties.Settings.Default.angleMin;
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("AngleMax", global::KinectController.Properties.Settings.Default, "angleMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("AngleMin", global::KinectController.Properties.Settings.Default, "angleMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("ScaleMax", global::KinectController.Properties.Settings.Default, "scaleMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("ScaleMin", global::KinectController.Properties.Settings.Default, "scaleMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("XAxisMax", global::KinectController.Properties.Settings.Default, "xMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("XAxisMin", global::KinectController.Properties.Settings.Default, "xMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("YAxisMax", global::KinectController.Properties.Settings.Default, "yMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("YAxisMin", global::KinectController.Properties.Settings.Default, "yMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.ScaleMax = global::KinectController.Properties.Settings.Default.scaleMax;
            this.controller.ScaleMin = global::KinectController.Properties.Settings.Default.scaleMin;
            this.controller.SynchronizingObject = this;
            this.controller.XAxisMax = global::KinectController.Properties.Settings.Default.xMax;
            this.controller.XAxisMin = global::KinectController.Properties.Settings.Default.xMin;
            this.controller.YAxisMax = global::KinectController.Properties.Settings.Default.yMax;
            this.controller.YAxisMin = global::KinectController.Properties.Settings.Default.yMin;
            this.controller.HeadMove += new System.EventHandler<Accord.Controls.Vision.HeadEventArgs>(this.headController1_HeadMove);
            this.controller.HeadEnter += new System.EventHandler<Accord.Controls.Vision.HeadEventArgs>(this.controller_HeadEnter);
            this.controller.HeadLeave += new System.EventHandler<System.EventArgs>(this.controller_HeadLeave);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 407);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pbRightHand);
            this.Controls.Add(this.pbLeftHand);
            this.Controls.Add(this.pbRightArm);
            this.Controls.Add(this.pbLeftArm);
            this.Controls.Add(this.videoSourcePlayer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.angleBox1);
            this.Controls.Add(this.btnVerticalMax);
            this.Controls.Add(this.btnVerticalMin);
            this.Controls.Add(this.btnHorizontalMax);
            this.Controls.Add(this.btnScaleMax);
            this.Controls.Add(this.btnAngleMax);
            this.Controls.Add(this.btnAngleMin);
            this.Controls.Add(this.btnScaleMin);
            this.Controls.Add(this.btnHorizontalMin);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tbScale);
            this.Controls.Add(this.tbVertical);
            this.Controls.Add(this.tbHorizontal);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(415, 430);
            this.Name = "MainForm";
            this.Text = "Dynamic virtual wall for hand segmentation (Kinect)";
            ((System.ComponentModel.ISupportInitialize)(this.tbHorizontal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVertical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbScale)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftArm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightArm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftHand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightHand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbHorizontal;
        private System.Windows.Forms.TrackBar tbVertical;
        private System.Windows.Forms.TrackBar tbScale;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnHorizontalMin;
        private System.Windows.Forms.Button btnHorizontalMax;
        private System.Windows.Forms.Button btnScaleMin;
        private System.Windows.Forms.Button btnScaleMax;
        private System.Windows.Forms.Button btnVerticalMin;
        private System.Windows.Forms.Button btnVerticalMax;
        private Accord.Controls.AngleBox angleBox1;
        private System.Windows.Forms.Button btnAngleMin;
        private System.Windows.Forms.Button btnAngleMax;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trackerVisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem faceControlsToolStripMenuItem;
        public Accord.Controls.Vision.HeadController controller;
        private System.Windows.Forms.ToolStripMenuItem depthVisionToolStripMenuItem;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
        private AForge.Controls.PictureBox pbRightArm;
        private AForge.Controls.PictureBox pbLeftArm;
        private AForge.Controls.PictureBox pictureBox1;
        private AForge.Controls.PictureBox pbRightHand;
        private AForge.Controls.PictureBox pbLeftHand;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem saveImagesToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

