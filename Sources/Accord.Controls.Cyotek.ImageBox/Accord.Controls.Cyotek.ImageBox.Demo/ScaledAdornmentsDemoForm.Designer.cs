namespace Accord.Controls.Cyotek.Demo
{
  partial class ScaledAdornmentsDemoForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaledAdornmentsDemoForm));
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.imageBox = new Accord.Controls.Cyotek.ImageBox();
      this.demoLabel = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.scaleAdornmentsCheckBox = new System.Windows.Forms.CheckBox();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
      this.positionToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.panel1.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
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
      this.splitContainer.Panel2.Controls.Add(this.panel1);
      this.splitContainer.Size = new System.Drawing.Size(789, 375);
      this.splitContainer.SplitterDistance = 574;
      this.splitContainer.TabIndex = 5;
      // 
      // imageBox
      // 
      this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.Map;
      this.imageBox.Location = new System.Drawing.Point(0, 0);
      this.imageBox.Name = "imageBox";
      this.imageBox.Size = new System.Drawing.Size(574, 375);
      this.imageBox.TabIndex = 0;
      this.imageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.imageBox_Paint);
      this.imageBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseClick);
      this.imageBox.MouseLeave += new System.EventHandler(this.imageBox_MouseLeave);
      this.imageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseMove);
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
      this.demoLabel.Padding = new System.Windows.Forms.Padding(9, 9, 9, 9);
      this.demoLabel.Size = new System.Drawing.Size(211, 352);
      this.demoLabel.TabIndex = 1;
      this.demoLabel.Text = resources.GetString("demoLabel.Text");
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.scaleAdornmentsCheckBox);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 352);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(211, 23);
      this.panel1.TabIndex = 2;
      // 
      // scaleAdornmentsCheckBox
      // 
      this.scaleAdornmentsCheckBox.AutoSize = true;
      this.scaleAdornmentsCheckBox.Checked = true;
      this.scaleAdornmentsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.scaleAdornmentsCheckBox.Location = new System.Drawing.Point(0, 3);
      this.scaleAdornmentsCheckBox.Name = "scaleAdornmentsCheckBox";
      this.scaleAdornmentsCheckBox.Size = new System.Drawing.Size(112, 17);
      this.scaleAdornmentsCheckBox.TabIndex = 0;
      this.scaleAdornmentsCheckBox.Text = "&Scale Adornments";
      this.scaleAdornmentsCheckBox.UseVisualStyleBackColor = true;
      this.scaleAdornmentsCheckBox.CheckedChanged += new System.EventHandler(this.scaleAdornmentsCheckBox_CheckedChanged);
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(809, 24);
      this.menuStrip.TabIndex = 6;
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
      this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.positionToolStripStatusLabel});
      this.statusStrip.Location = new System.Drawing.Point(0, 398);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(809, 22);
      this.statusStrip.TabIndex = 7;
      // 
      // toolStripStatusLabel1
      // 
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(794, 17);
      this.toolStripStatusLabel1.Spring = true;
      // 
      // positionToolStripStatusLabel
      // 
      this.positionToolStripStatusLabel.Name = "positionToolStripStatusLabel";
      this.positionToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
      // 
      // ScaledAdornmentsDemoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(809, 420);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.menuStrip);
      this.Controls.Add(this.statusStrip);
      this.Name = "ScaledAdornmentsDemoForm";
      this.Text = "Scaled Adornments";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
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
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.CheckBox scaleAdornmentsCheckBox;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    private System.Windows.Forms.ToolStripStatusLabel positionToolStripStatusLabel;
  }
}
