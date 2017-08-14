namespace Accord.DebuggerVisualizers
{
    partial class ImageView
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
            this.tscMain = new System.Windows.Forms.ToolStripContainer();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.tsslStatusWidth = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslStatusHeight = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslStatusPixelFormat = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslStatusType = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslDPI = new System.Windows.Forms.ToolStripStatusLabel();
            this.pMain = new System.Windows.Forms.Panel();
            this.pbPreview = new Accord.Controls.PictureBox();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tsddbZoom = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsbCopyImage = new System.Windows.Forms.ToolStripButton();
            this.tsbSaveImage = new System.Windows.Forms.ToolStripButton();
            this.tsbOpenGitHub = new System.Windows.Forms.ToolStripButton();
            this.tsslX = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslY = new System.Windows.Forms.ToolStripStatusLabel();
            this.tscMain.BottomToolStripPanel.SuspendLayout();
            this.tscMain.ContentPanel.SuspendLayout();
            this.tscMain.TopToolStripPanel.SuspendLayout();
            this.tscMain.SuspendLayout();
            this.ssMain.SuspendLayout();
            this.pMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscMain
            // 
            // 
            // tscMain.BottomToolStripPanel
            // 
            this.tscMain.BottomToolStripPanel.Controls.Add(this.ssMain);
            // 
            // tscMain.ContentPanel
            // 
            this.tscMain.ContentPanel.Controls.Add(this.pMain);
            this.tscMain.ContentPanel.Size = new System.Drawing.Size(684, 512);
            this.tscMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tscMain.Location = new System.Drawing.Point(0, 0);
            this.tscMain.Name = "tscMain";
            this.tscMain.Size = new System.Drawing.Size(684, 561);
            this.tscMain.TabIndex = 1;
            this.tscMain.Text = "toolStripContainer1";
            // 
            // tscMain.TopToolStripPanel
            // 
            this.tscMain.TopToolStripPanel.Controls.Add(this.tsMain);
            // 
            // ssMain
            // 
            this.ssMain.Dock = System.Windows.Forms.DockStyle.None;
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatusWidth,
            this.tsslStatusHeight,
            this.tsslStatusPixelFormat,
            this.tsslStatusType,
            this.tsslDPI,
            this.tsslX,
            this.tsslY});
            this.ssMain.Location = new System.Drawing.Point(0, 0);
            this.ssMain.Name = "ssMain";
            this.ssMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.ssMain.Size = new System.Drawing.Size(684, 24);
            this.ssMain.TabIndex = 0;
            // 
            // tsslStatusWidth
            // 
            this.tsslStatusWidth.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslStatusWidth.Name = "tsslStatusWidth";
            this.tsslStatusWidth.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.tsslStatusWidth.Size = new System.Drawing.Size(51, 19);
            this.tsslStatusWidth.Text = "Width:";
            // 
            // tsslStatusHeight
            // 
            this.tsslStatusHeight.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslStatusHeight.Name = "tsslStatusHeight";
            this.tsslStatusHeight.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.tsslStatusHeight.Size = new System.Drawing.Size(55, 19);
            this.tsslStatusHeight.Text = "Height:";
            // 
            // tsslStatusPixelFormat
            // 
            this.tsslStatusPixelFormat.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslStatusPixelFormat.Name = "tsslStatusPixelFormat";
            this.tsslStatusPixelFormat.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.tsslStatusPixelFormat.Size = new System.Drawing.Size(82, 19);
            this.tsslStatusPixelFormat.Text = "Pixel format:";
            // 
            // tsslStatusType
            // 
            this.tsslStatusType.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslStatusType.Name = "tsslStatusType";
            this.tsslStatusType.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.tsslStatusType.Size = new System.Drawing.Size(44, 19);
            this.tsslStatusType.Text = "Type:";
            // 
            // tsslDPI
            // 
            this.tsslDPI.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslDPI.Name = "tsslDPI";
            this.tsslDPI.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.tsslDPI.Size = new System.Drawing.Size(33, 19);
            this.tsslDPI.Text = "DPI:";
            // 
            // tsslX
            // 
            this.tsslX.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslX.Name = "tsslX";
            this.tsslX.Size = new System.Drawing.Size(17, 19);
            this.tsslX.Text = "X:";
            // 
            // tsslY
            // 
            //this.tsslY.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslY.Name = "tsslY";
            this.tsslY.Size = new System.Drawing.Size(17, 19);
            this.tsslY.Text = "Y:";
            // 
            // pMain
            // 
            this.pMain.AutoScroll = true;
            this.pMain.BackColor = System.Drawing.Color.White;
            this.pMain.Controls.Add(this.pbPreview);
            this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pMain.Location = new System.Drawing.Point(0, 0);
            this.pMain.Name = "pMain";
            this.pMain.Size = new System.Drawing.Size(684, 512);
            this.pMain.TabIndex = 1;
            // 
            // pbPreview
            // 
            this.pbPreview.Location = new System.Drawing.Point(0, 0);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(400, 400);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbPreview.TabIndex = 0;
            this.pbPreview.TabStop = false;
            this.pbPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPreview_MouseDown);
            this.pbPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPreview_MouseMove);
            this.pbPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPreview_MouseUp);
            // 
            // tsMain
            // 
            this.tsMain.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddbZoom,
            this.tsbCopyImage,
            this.tsbSaveImage,
            this.tsbOpenGitHub});
            this.tsMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsMain.Location = new System.Drawing.Point(0, 0);
            this.tsMain.Name = "tsMain";
            this.tsMain.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.tsMain.Size = new System.Drawing.Size(684, 25);
            this.tsMain.Stretch = true;
            this.tsMain.TabIndex = 0;
            // 
            // tsddbZoom
            // 
            this.tsddbZoom.AutoToolTip = false;
            this.tsddbZoom.Image = global::Accord.DebuggerVisualizers.Properties.Resources.magnifier;
            this.tsddbZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbZoom.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
            this.tsddbZoom.Name = "tsddbZoom";
            this.tsddbZoom.Size = new System.Drawing.Size(102, 22);
            this.tsddbZoom.Text = "Zoom: 100%";
            // 
            // tsbCopyImage
            // 
            this.tsbCopyImage.AutoToolTip = false;
            this.tsbCopyImage.Image = global::Accord.DebuggerVisualizers.Properties.Resources.document_copy;
            this.tsbCopyImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopyImage.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
            this.tsbCopyImage.Name = "tsbCopyImage";
            this.tsbCopyImage.Size = new System.Drawing.Size(91, 22);
            this.tsbCopyImage.Text = "Copy image";
            this.tsbCopyImage.Click += new System.EventHandler(this.tsbCopyImage_Click);
            // 
            // tsbSaveImage
            // 
            this.tsbSaveImage.AutoToolTip = false;
            this.tsbSaveImage.Image = global::Accord.DebuggerVisualizers.Properties.Resources.disk_black;
            this.tsbSaveImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveImage.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
            this.tsbSaveImage.Name = "tsbSaveImage";
            this.tsbSaveImage.Size = new System.Drawing.Size(96, 22);
            this.tsbSaveImage.Text = "Save image...";
            this.tsbSaveImage.Click += new System.EventHandler(this.tsbSaveImage_Click);
            // 
            // tsbOpenGitHub
            // 
            this.tsbOpenGitHub.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbOpenGitHub.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenGitHub.Image = global::Accord.DebuggerVisualizers.Properties.Resources.GitHub;
            this.tsbOpenGitHub.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenGitHub.Name = "tsbOpenGitHub";
            this.tsbOpenGitHub.Size = new System.Drawing.Size(23, 22);
            this.tsbOpenGitHub.Text = "Open GitHub page";
            this.tsbOpenGitHub.Click += new System.EventHandler(this.tsbOpenGitHub_Click);
            // 
            // ImageVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(684, 561);
            this.Controls.Add(this.tscMain);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "ImageVisualizerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Visualizer 1.2.0";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.ImageView_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageView_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ImageView_KeyUp);
            this.tscMain.BottomToolStripPanel.ResumeLayout(false);
            this.tscMain.BottomToolStripPanel.PerformLayout();
            this.tscMain.ContentPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.PerformLayout();
            this.tscMain.ResumeLayout(false);
            this.tscMain.PerformLayout();
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.pMain.ResumeLayout(false);
            this.pMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private Accord.Controls.PictureBox pbPreview;
        private System.Windows.Forms.ToolStripContainer tscMain;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripButton tsbCopyImage;
        private System.Windows.Forms.ToolStripButton tsbSaveImage;
        private System.Windows.Forms.ToolStripDropDownButton tsddbZoom;
        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatusWidth;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatusHeight;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatusPixelFormat;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatusType;
        private System.Windows.Forms.Panel pMain;
        private System.Windows.Forms.ToolStripButton tsbOpenGitHub;
        private System.Windows.Forms.ToolStripStatusLabel tsslDPI;
        private System.Windows.Forms.ToolStripStatusLabel tsslX;
        private System.Windows.Forms.ToolStripStatusLabel tsslY;
    }
}