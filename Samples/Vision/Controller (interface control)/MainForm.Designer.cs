namespace Controller
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
            this.angleBox1 = new Accord.Controls.AngleBox();
            this.controller = new Accord.Controls.Vision.HeadController();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackerVisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.faceControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.tbHorizontal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVertical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbScale)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbHorizontal
            // 
            this.tbHorizontal.Location = new System.Drawing.Point(63, 327);
            this.tbHorizontal.Maximum = 20;
            this.tbHorizontal.Minimum = -20;
            this.tbHorizontal.Name = "tbHorizontal";
            this.tbHorizontal.Size = new System.Drawing.Size(272, 45);
            this.tbHorizontal.TabIndex = 0;
            // 
            // tbVertical
            // 
            this.tbVertical.Location = new System.Drawing.Point(341, 60);
            this.tbVertical.Maximum = 20;
            this.tbVertical.Minimum = -20;
            this.tbVertical.Name = "tbVertical";
            this.tbVertical.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbVertical.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbVertical.Size = new System.Drawing.Size(45, 232);
            this.tbVertical.TabIndex = 0;
            // 
            // tbScale
            // 
            this.tbScale.Location = new System.Drawing.Point(12, 60);
            this.tbScale.Maximum = 20;
            this.tbScale.Minimum = -20;
            this.tbScale.Name = "tbScale";
            this.tbScale.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbScale.Size = new System.Drawing.Size(45, 232);
            this.tbScale.TabIndex = 0;
            this.tbScale.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(153, 35);
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
            this.btnHorizontalMin.Location = new System.Drawing.Point(12, 327);
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
            this.btnHorizontalMax.Location = new System.Drawing.Point(341, 327);
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
            this.btnScaleMin.Location = new System.Drawing.Point(12, 288);
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
            this.btnVerticalMin.Location = new System.Drawing.Point(341, 288);
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
            this.btnVerticalMax.Location = new System.Drawing.Point(341, 31);
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
            this.btnAngleMin.Location = new System.Drawing.Point(124, 258);
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
            this.btnAngleMax.Location = new System.Drawing.Point(234, 258);
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
            this.statusStrip1.Size = new System.Drawing.Size(399, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // angleBox1
            // 
            this.angleBox1.Angle = 0F;
            this.angleBox1.Location = new System.Drawing.Point(124, 102);
            this.angleBox1.Name = "angleBox1";
            this.angleBox1.Size = new System.Drawing.Size(150, 150);
            this.angleBox1.TabIndex = 2;
            // 
            // controller
            // 
            this.controller.AngleMax = global::Controller.Properties.Settings.Default.angleMax;
            this.controller.AngleMin = global::Controller.Properties.Settings.Default.angleMin;
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("AngleMax", global::Controller.Properties.Settings.Default, "angleMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("AngleMin", global::Controller.Properties.Settings.Default, "angleMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("ScaleMax", global::Controller.Properties.Settings.Default, "scaleMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("ScaleMin", global::Controller.Properties.Settings.Default, "scaleMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("XAxisMax", global::Controller.Properties.Settings.Default, "xMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("XAxisMin", global::Controller.Properties.Settings.Default, "xMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("YAxisMax", global::Controller.Properties.Settings.Default, "yMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.DataBindings.Add(new System.Windows.Forms.Binding("YAxisMin", global::Controller.Properties.Settings.Default, "yMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.controller.ScaleMax = global::Controller.Properties.Settings.Default.scaleMax;
            this.controller.ScaleMin = global::Controller.Properties.Settings.Default.scaleMin;
            this.controller.SynchronizingObject = this;
            this.controller.XAxisMax = global::Controller.Properties.Settings.Default.xMax;
            this.controller.XAxisMin = global::Controller.Properties.Settings.Default.xMin;
            this.controller.YAxisMax = global::Controller.Properties.Settings.Default.yMax;
            this.controller.YAxisMin = global::Controller.Properties.Settings.Default.yMin;
            this.controller.HeadMove += new System.EventHandler<Accord.Controls.Vision.HeadEventArgs>(this.headController1_HeadMove);
            this.controller.HeadEnter += new System.EventHandler<Accord.Controls.Vision.HeadEventArgs>(this.controller_HeadEnter);
            this.controller.HeadLeave += new System.EventHandler<System.EventArgs>(this.controller_HeadLeave);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.showToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(399, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectCameraToolStripMenuItem});
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
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trackerVisionToolStripMenuItem,
            this.faceControlsToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.showToolStripMenuItem.Text = "Show";
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 407);
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
            this.Text = "Head-based Controller";
            ((System.ComponentModel.ISupportInitialize)(this.tbHorizontal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVertical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbScale)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
    }
}

