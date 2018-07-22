namespace Accord.Controls.Cyotek.Demo
{
  partial class MiniMapDemoForm
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
      this.zoomImageBox = new Accord.Controls.Cyotek.ImageBox();
      this.miniMapImageBox = new Accord.Controls.Cyotek.ImageBox();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.imageViewPortToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.calculatedRectangleToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.statusStrip1.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // zoomImageBox
      // 
      this.zoomImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.zoomImageBox.Location = new System.Drawing.Point(0, 0);
      this.zoomImageBox.Name = "zoomImageBox";
      this.zoomImageBox.Size = new System.Drawing.Size(680, 483);
      this.zoomImageBox.TabIndex = 0;
      this.zoomImageBox.TabStop = false;
      this.zoomImageBox.ImageChanged += new System.EventHandler(this.zoomImageBox_ImageChanged);
      this.zoomImageBox.Zoomed += new System.EventHandler<Accord.Controls.Cyotek.ImageBoxZoomEventArgs>(this.zoomImageBox_Zoomed);
      this.zoomImageBox.Scroll += new System.Windows.Forms.ScrollEventHandler(this.zoomImageBox_Scroll);
      this.zoomImageBox.Resize += new System.EventHandler(this.zoomImageBox_Resize);
      // 
      // miniMapImageBox
      // 
      this.miniMapImageBox.AllowZoom = false;
      this.miniMapImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.miniMapImageBox.AutoPan = false;
      this.miniMapImageBox.Location = new System.Drawing.Point(3, 3);
      this.miniMapImageBox.Name = "miniMapImageBox";
      this.miniMapImageBox.Size = new System.Drawing.Size(291, 155);
      this.miniMapImageBox.SizeMode = Accord.Controls.Cyotek.ImageBoxSizeMode.Fit;
      this.miniMapImageBox.TabIndex = 0;
      this.miniMapImageBox.TabStop = false;
      this.miniMapImageBox.VirtualMode = true;
      this.miniMapImageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.miniMapImageBox_Paint);
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageViewPortToolStripStatusLabel,
            this.calculatedRectangleToolStripStatusLabel});
      this.statusStrip1.Location = new System.Drawing.Point(0, 507);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(980, 22);
      this.statusStrip1.TabIndex = 2;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // imageViewPortToolStripStatusLabel
      // 
      this.imageViewPortToolStripStatusLabel.Name = "imageViewPortToolStripStatusLabel";
      this.imageViewPortToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
      // 
      // calculatedRectangleToolStripStatusLabel
      // 
      this.calculatedRectangleToolStripStatusLabel.Name = "calculatedRectangleToolStripStatusLabel";
      this.calculatedRectangleToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
      this.menuStrip.Size = new System.Drawing.Size(980, 24);
      this.menuStrip.TabIndex = 0;
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.closeToolStripMenuItem.Text = "&Close";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
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
      this.aboutToolStripMenuItem.Text = "&About...";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(0, 24);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.zoomImageBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.miniMapImageBox);
      this.splitContainer.Size = new System.Drawing.Size(980, 483);
      this.splitContainer.SplitterDistance = 680;
      this.splitContainer.SplitterWidth = 3;
      this.splitContainer.TabIndex = 1;
      this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
      // 
      // MiniMapDemoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(980, 529);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.menuStrip);
      this.Controls.Add(this.statusStrip1);
      this.Name = "MiniMapDemoForm";
      this.Text = "Minimap Overlay Demonstration";
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Accord.Controls.Cyotek.ImageBox zoomImageBox;
    private Accord.Controls.Cyotek.ImageBox miniMapImageBox;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel imageViewPortToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel calculatedRectangleToolStripStatusLabel;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.SplitContainer splitContainer;
  }
}

