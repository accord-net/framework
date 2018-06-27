namespace Accord.Controls.Cyotek.Demo
{
  partial class SwitchImageDuringZoomDemoForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.statusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.cursorToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.zoomToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.mapNameToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.imageBox = new Accord.Controls.Cyotek.ImageBox();
      this.messageLabel = new System.Windows.Forms.Label();
      this.resetMessageTimer = new System.Windows.Forms.Timer(this.components);
      this.refreshMapTimer = new System.Windows.Forms.Timer(this.components);
      this.menuStrip.SuspendLayout();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(747, 24);
      this.menuStrip.TabIndex = 10;
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
            this.statusToolStripStatusLabel,
            this.cursorToolStripStatusLabel,
            this.zoomToolStripStatusLabel,
            this.mapNameToolStripStatusLabel});
      this.statusStrip.Location = new System.Drawing.Point(0, 496);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(747, 22);
      this.statusStrip.TabIndex = 11;
      // 
      // statusToolStripStatusLabel
      // 
      this.statusToolStripStatusLabel.Name = "statusToolStripStatusLabel";
      this.statusToolStripStatusLabel.Size = new System.Drawing.Size(649, 17);
      this.statusToolStripStatusLabel.Spring = true;
      this.statusToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cursorToolStripStatusLabel
      // 
      this.cursorToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.Cursor;
      this.cursorToolStripStatusLabel.Name = "cursorToolStripStatusLabel";
      this.cursorToolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
      this.cursorToolStripStatusLabel.ToolTipText = "Current Cursor Position";
      // 
      // zoomToolStripStatusLabel
      // 
      this.zoomToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.Zoom;
      this.zoomToolStripStatusLabel.Name = "zoomToolStripStatusLabel";
      this.zoomToolStripStatusLabel.Size = new System.Drawing.Size(51, 17);
      this.zoomToolStripStatusLabel.Text = "100%";
      this.zoomToolStripStatusLabel.ToolTipText = "Zoom";
      // 
      // mapNameToolStripStatusLabel
      // 
      this.mapNameToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.SmallMap;
      this.mapNameToolStripStatusLabel.Name = "mapNameToolStripStatusLabel";
      this.mapNameToolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
      this.mapNameToolStripStatusLabel.ToolTipText = "Zoom";
      // 
      // imageBox
      // 
      this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox.Location = new System.Drawing.Point(0, 24);
      this.imageBox.Name = "imageBox";
      this.imageBox.Size = new System.Drawing.Size(747, 451);
      this.imageBox.TabIndex = 12;
      this.imageBox.ZoomChanged += new System.EventHandler(this.imageBox_ZoomChanged);
      this.imageBox.Zoomed += new System.EventHandler<Accord.Controls.Cyotek.ImageBoxZoomEventArgs>(this.imageBox_Zoomed);
      this.imageBox.MouseLeave += new System.EventHandler(this.imageBox_MouseLeave);
      this.imageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseMove);
      // 
      // messageLabel
      // 
      this.messageLabel.AutoEllipsis = true;
      this.messageLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.messageLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.messageLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
      this.messageLabel.Location = new System.Drawing.Point(0, 475);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(747, 21);
      this.messageLabel.TabIndex = 13;
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // resetMessageTimer
      // 
      this.resetMessageTimer.Interval = 5000;
      this.resetMessageTimer.Tick += new System.EventHandler(this.resetMessageTimer_Tick);
      // 
      // refreshMapTimer
      // 
      this.refreshMapTimer.Interval = 5;
      this.refreshMapTimer.Tick += new System.EventHandler(this.refreshMapTimer_Tick);
      // 
      // SwitchImageDuringZoomDemoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(747, 518);
      this.Controls.Add(this.imageBox);
      this.Controls.Add(this.menuStrip);
      this.Controls.Add(this.messageLabel);
      this.Controls.Add(this.statusStrip);
      this.Name = "SwitchImageDuringZoomDemoForm";
      this.Text = "Switch Image During Zoom Demonstration";
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
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
    private ImageBox imageBox;
    private System.Windows.Forms.ToolStripStatusLabel zoomToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel mapNameToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel cursorToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel statusToolStripStatusLabel;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.Timer resetMessageTimer;
    private System.Windows.Forms.Timer refreshMapTimer;
  }
}
