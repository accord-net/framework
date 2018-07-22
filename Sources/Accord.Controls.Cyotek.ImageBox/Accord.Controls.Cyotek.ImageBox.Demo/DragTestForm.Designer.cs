namespace Accord.Controls.Cyotek.Demo
{
  partial class DragTestForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DragTestForm));
      this.imageBox = new Accord.Controls.Cyotek.ImageBox();
      this.demoLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // imageBox
      // 
      this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox.Location = new System.Drawing.Point(0, 0);
      this.imageBox.Name = "imageBox";
      this.imageBox.SelectionMode = Accord.Controls.Cyotek.ImageBoxSelectionMode.Rectangle;
      this.imageBox.Size = new System.Drawing.Size(342, 278);
      this.imageBox.TabIndex = 0;
      this.imageBox.VirtualMode = true;
      this.imageBox.VirtualSize = new System.Drawing.Size(256, 256);
      this.imageBox.Selecting += new System.EventHandler<Accord.Controls.Cyotek.ImageBoxCancelEventArgs>(this.imageBox_Selecting);
      this.imageBox.SelectionRegionChanged += new System.EventHandler(this.imageBox_SelectionRegionChanged);
      this.imageBox.VirtualDraw += new System.Windows.Forms.PaintEventHandler(this.imageBox_VirtualDraw);
      this.imageBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseDown);
      this.imageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseMove);
      this.imageBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseUp);
      // 
      // demoLabel
      // 
      this.demoLabel.AutoEllipsis = true;
      this.demoLabel.BackColor = System.Drawing.SystemColors.Info;
      this.demoLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.demoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.demoLabel.ForeColor = System.Drawing.SystemColors.InfoText;
      this.demoLabel.Location = new System.Drawing.Point(0, 0);
      this.demoLabel.Name = "demoLabel";
      this.demoLabel.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
      this.demoLabel.Size = new System.Drawing.Size(360, 278);
      this.demoLabel.TabIndex = 1;
      this.demoLabel.Text = resources.GetString("demoLabel.Text");
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(10, 23);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.imageBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.demoLabel);
      this.splitContainer.Size = new System.Drawing.Size(705, 278);
      this.splitContainer.SplitterDistance = 342;
      this.splitContainer.SplitterWidth = 3;
      this.splitContainer.TabIndex = 2;
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
      this.menuStrip.Size = new System.Drawing.Size(726, 24);
      this.menuStrip.TabIndex = 3;
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
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
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
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
      this.aboutToolStripMenuItem.Text = "&About...";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // statusStrip
      // 
      this.statusStrip.Location = new System.Drawing.Point(0, 301);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
      this.statusStrip.Size = new System.Drawing.Size(726, 22);
      this.statusStrip.TabIndex = 4;
      // 
      // DragTestForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(726, 323);
      this.Controls.Add(this.statusStrip);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.menuStrip);
      this.MainMenuStrip = this.menuStrip;
      this.Name = "DragTestForm";
      this.Text = "Drag Test";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ImageBox imageBox;
    private System.Windows.Forms.Label demoLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
  }
}
