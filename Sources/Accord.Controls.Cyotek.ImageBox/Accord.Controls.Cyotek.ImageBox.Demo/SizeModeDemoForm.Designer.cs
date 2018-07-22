namespace Accord.Controls.Cyotek.Demo
{
  partial class SizeModeDemoForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SizeModeDemoForm));
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.imageBox1 = new Accord.Controls.Cyotek.ImageBox();
      this.cyotekPreviewHeaderLabel = new System.Windows.Forms.Label();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.imageBox2 = new Accord.Controls.Cyotek.ImageBox();
      this.label1 = new System.Windows.Forms.Label();
      this.imageBox3 = new Accord.Controls.Cyotek.ImageBox();
      this.label2 = new System.Windows.Forms.Label();
      this.menuStrip.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
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
      this.menuStrip.TabIndex = 8;
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
      this.statusStrip.TabIndex = 9;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 24);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.imageBox1);
      this.splitContainer1.Panel1.Controls.Add(this.cyotekPreviewHeaderLabel);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
      this.splitContainer1.Size = new System.Drawing.Size(771, 382);
      this.splitContainer1.SplitterDistance = 257;
      this.splitContainer1.SplitterWidth = 3;
      this.splitContainer1.TabIndex = 10;
      // 
      // imageBox1
      // 
      this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox1.Image = ((System.Drawing.Image)(resources.GetObject("imageBox1.Image")));
      this.imageBox1.Location = new System.Drawing.Point(0, 0);
      this.imageBox1.Name = "imageBox1";
      this.imageBox1.Size = new System.Drawing.Size(257, 361);
      this.imageBox1.TabIndex = 0;
      // 
      // cyotekPreviewHeaderLabel
      // 
      this.cyotekPreviewHeaderLabel.AutoEllipsis = true;
      this.cyotekPreviewHeaderLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.cyotekPreviewHeaderLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.cyotekPreviewHeaderLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
      this.cyotekPreviewHeaderLabel.Location = new System.Drawing.Point(0, 361);
      this.cyotekPreviewHeaderLabel.Name = "cyotekPreviewHeaderLabel";
      this.cyotekPreviewHeaderLabel.Size = new System.Drawing.Size(257, 21);
      this.cyotekPreviewHeaderLabel.TabIndex = 8;
      this.cyotekPreviewHeaderLabel.Text = "Normal";
      this.cyotekPreviewHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.imageBox2);
      this.splitContainer2.Panel1.Controls.Add(this.label1);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.imageBox3);
      this.splitContainer2.Panel2.Controls.Add(this.label2);
      this.splitContainer2.Size = new System.Drawing.Size(511, 382);
      this.splitContainer2.SplitterDistance = 255;
      this.splitContainer2.SplitterWidth = 3;
      this.splitContainer2.TabIndex = 11;
      // 
      // imageBox2
      // 
      this.imageBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox2.Image = ((System.Drawing.Image)(resources.GetObject("imageBox2.Image")));
      this.imageBox2.Location = new System.Drawing.Point(0, 0);
      this.imageBox2.Name = "imageBox2";
      this.imageBox2.Size = new System.Drawing.Size(255, 361);
      this.imageBox2.SizeMode = Accord.Controls.Cyotek.ImageBoxSizeMode.Stretch;
      this.imageBox2.TabIndex = 1;
      this.imageBox2.Zoom = 167;
      // 
      // label1
      // 
      this.label1.AutoEllipsis = true;
      this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.label1.ForeColor = System.Drawing.SystemColors.HighlightText;
      this.label1.Location = new System.Drawing.Point(0, 361);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(255, 21);
      this.label1.TabIndex = 8;
      this.label1.Text = "Stretch";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // imageBox3
      // 
      this.imageBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox3.Image = ((System.Drawing.Image)(resources.GetObject("imageBox3.Image")));
      this.imageBox3.Location = new System.Drawing.Point(0, 0);
      this.imageBox3.Name = "imageBox3";
      this.imageBox3.Size = new System.Drawing.Size(253, 361);
      this.imageBox3.SizeMode = Accord.Controls.Cyotek.ImageBoxSizeMode.Fit;
      this.imageBox3.TabIndex = 1;
      this.imageBox3.Zoom = 166;
      // 
      // label2
      // 
      this.label2.AutoEllipsis = true;
      this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.label2.ForeColor = System.Drawing.SystemColors.HighlightText;
      this.label2.Location = new System.Drawing.Point(0, 361);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(253, 21);
      this.label2.TabIndex = 8;
      this.label2.Text = "Fit";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // SizeModeDemoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(771, 428);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.menuStrip);
      this.Controls.Add(this.statusStrip);
      this.Name = "SizeModeDemoForm";
      this.Text = "SizeMode Demonstation";
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.ResumeLayout(false);
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
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private ImageBox imageBox1;
    private ImageBox imageBox2;
    private ImageBox imageBox3;
    private System.Windows.Forms.Label cyotekPreviewHeaderLabel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;

  }
}
