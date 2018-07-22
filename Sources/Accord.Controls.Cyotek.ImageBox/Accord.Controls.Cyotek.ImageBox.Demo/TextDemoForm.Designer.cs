namespace Accord.Controls.Cyotek.Demo
{
  partial class TextDemoForm
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
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.propertyGrid = new Accord.Controls.Cyotek.Demo.PropertyGrid();
      this.imageBox = new Accord.Controls.Cyotek.ImageBox();
      this.menuStrip.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(771, 24);
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
      this.statusStrip.Location = new System.Drawing.Point(0, 406);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(771, 22);
      this.statusStrip.TabIndex = 2;
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
      this.splitContainer.Panel1.Controls.Add(this.propertyGrid);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.imageBox);
      this.splitContainer.Size = new System.Drawing.Size(771, 382);
      this.splitContainer.SplitterDistance = 300;
      this.splitContainer.SplitterWidth = 3;
      this.splitContainer.TabIndex = 1;
      // 
      // propertyGrid
      // 
      this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.propertyGrid.Location = new System.Drawing.Point(0, 0);
      this.propertyGrid.Name = "propertyGrid";
      this.propertyGrid.SelectedObject = this.imageBox;
      this.propertyGrid.Size = new System.Drawing.Size(300, 382);
      this.propertyGrid.TabIndex = 0;
      // 
      // imageBox
      // 
      this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
      this.imageBox.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.samplecrop;
      this.imageBox.Location = new System.Drawing.Point(0, 0);
      this.imageBox.Name = "imageBox";
      this.imageBox.ScaleText = true;
      this.imageBox.ShowPixelGrid = true;
      this.imageBox.Size = new System.Drawing.Size(468, 382);
      this.imageBox.TabIndex = 0;
      this.imageBox.Text = "This is a demonstration of the ImageBox text functionality";
      this.imageBox.TextBackColor = System.Drawing.Color.Teal;
      this.imageBox.TextDisplayMode = Accord.Controls.Cyotek.ImageBoxGridDisplayMode.Image;
      this.imageBox.TextPadding = new System.Windows.Forms.Padding(3);
      // 
      // TextDemoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(771, 428);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.menuStrip);
      this.Controls.Add(this.statusStrip);
      this.Name = "TextDemoForm";
      this.Text = "Text Demonstation";
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.SplitContainer splitContainer;
    private PropertyGrid propertyGrid;
    private ImageBox imageBox;

  }
}
