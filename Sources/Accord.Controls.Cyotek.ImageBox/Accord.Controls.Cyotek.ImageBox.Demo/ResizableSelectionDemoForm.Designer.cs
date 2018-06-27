namespace Accord.Controls.Cyotek.Demo
{
  partial class ResizableSelectionDemoForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResizableSelectionDemoForm));
      this.demoLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.imageBox = new Accord.Controls.Cyotek.Demo.ImageBoxEx();
      this.eventsSplitContainer = new System.Windows.Forms.SplitContainer();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.eventsTabPage = new System.Windows.Forms.TabPage();
      this.eventsListBox = new Accord.Controls.Cyotek.Demo.EventsListBox();
      this.dragHandlesTabPage = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.enabledCheckedListBox = new System.Windows.Forms.CheckedListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.visibleCheckedListBox = new System.Windows.Forms.CheckedListBox();
      this.label2 = new System.Windows.Forms.Label();
      this.propertiesTabPage = new System.Windows.Forms.TabPage();
      this.propertyGrid = new System.Windows.Forms.PropertyGrid();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.cursorToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.statusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.autoScrollPositionToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.imageSizeToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.zoomToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.selectionToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.eventsSplitContainer.Panel1.SuspendLayout();
      this.eventsSplitContainer.Panel2.SuspendLayout();
      this.eventsSplitContainer.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.eventsTabPage.SuspendLayout();
      this.dragHandlesTabPage.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.propertiesTabPage.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
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
      this.demoLabel.Size = new System.Drawing.Size(360, 180);
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
      this.splitContainer.Panel2.Controls.Add(this.eventsSplitContainer);
      this.splitContainer.Size = new System.Drawing.Size(705, 362);
      this.splitContainer.SplitterDistance = 342;
      this.splitContainer.SplitterWidth = 3;
      this.splitContainer.TabIndex = 5;
      // 
      // imageBox
      // 
      this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.imageBox.Location = new System.Drawing.Point(0, 0);
      this.imageBox.Name = "imageBox";
      this.imageBox.SelectionMode = Accord.Controls.Cyotek.ImageBoxSelectionMode.Rectangle;
      this.imageBox.Size = new System.Drawing.Size(342, 362);
      this.imageBox.TabIndex = 0;
      this.imageBox.VirtualMode = true;
      this.imageBox.VirtualSize = new System.Drawing.Size(256, 256);
      this.imageBox.SelectionMoved += new System.EventHandler(this.imageBox_SelectionMoved);
      this.imageBox.SelectionMoving += new System.ComponentModel.CancelEventHandler(this.imageBox_SelectionMoving);
      this.imageBox.SelectionResized += new System.EventHandler(this.imageBox_SelectionResized);
      this.imageBox.SelectionResizing += new System.ComponentModel.CancelEventHandler(this.imageBox_SelectionResizing);
      this.imageBox.Selected += new System.EventHandler<System.EventArgs>(this.imageBox_Selected);
      this.imageBox.Selecting += new System.EventHandler<Accord.Controls.Cyotek.ImageBoxCancelEventArgs>(this.imageBox_Selecting);
      this.imageBox.SelectionRegionChanged += new System.EventHandler(this.imageBox_SelectionRegionChanged);
      this.imageBox.VirtualDraw += new System.Windows.Forms.PaintEventHandler(this.imageBox_VirtualDraw);
      this.imageBox.Zoomed += new System.EventHandler<Accord.Controls.Cyotek.ImageBoxZoomEventArgs>(this.imageBox_Zoomed);
      this.imageBox.Scroll += new System.Windows.Forms.ScrollEventHandler(this.imageBox_Scroll);
      this.imageBox.MouseLeave += new System.EventHandler(this.imageBox_MouseLeave);
      this.imageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseMove);
      this.imageBox.Resize += new System.EventHandler(this.imageBox_Resize);
      // 
      // eventsSplitContainer
      // 
      this.eventsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.eventsSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.eventsSplitContainer.IsSplitterFixed = true;
      this.eventsSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.eventsSplitContainer.Name = "eventsSplitContainer";
      this.eventsSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // eventsSplitContainer.Panel1
      // 
      this.eventsSplitContainer.Panel1.Controls.Add(this.demoLabel);
      // 
      // eventsSplitContainer.Panel2
      // 
      this.eventsSplitContainer.Panel2.Controls.Add(this.tabControl1);
      this.eventsSplitContainer.Size = new System.Drawing.Size(360, 362);
      this.eventsSplitContainer.SplitterDistance = 180;
      this.eventsSplitContainer.SplitterWidth = 3;
      this.eventsSplitContainer.TabIndex = 2;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.eventsTabPage);
      this.tabControl1.Controls.Add(this.dragHandlesTabPage);
      this.tabControl1.Controls.Add(this.propertiesTabPage);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(360, 179);
      this.tabControl1.TabIndex = 0;
      // 
      // eventsTabPage
      // 
      this.eventsTabPage.Controls.Add(this.eventsListBox);
      this.eventsTabPage.Location = new System.Drawing.Point(4, 22);
      this.eventsTabPage.Name = "eventsTabPage";
      this.eventsTabPage.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
      this.eventsTabPage.Size = new System.Drawing.Size(352, 153);
      this.eventsTabPage.TabIndex = 0;
      this.eventsTabPage.Text = "Events";
      this.eventsTabPage.UseVisualStyleBackColor = true;
      // 
      // eventsListBox
      // 
      this.eventsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.eventsListBox.FormattingEnabled = true;
      this.eventsListBox.Location = new System.Drawing.Point(5, 5);
      this.eventsListBox.Name = "eventsListBox";
      this.eventsListBox.Size = new System.Drawing.Size(342, 143);
      this.eventsListBox.TabIndex = 0;
      // 
      // dragHandlesTabPage
      // 
      this.dragHandlesTabPage.Controls.Add(this.splitContainer1);
      this.dragHandlesTabPage.Location = new System.Drawing.Point(4, 24);
      this.dragHandlesTabPage.Name = "dragHandlesTabPage";
      this.dragHandlesTabPage.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
      this.dragHandlesTabPage.Size = new System.Drawing.Size(351, 175);
      this.dragHandlesTabPage.TabIndex = 1;
      this.dragHandlesTabPage.Text = "Drag Handles";
      this.dragHandlesTabPage.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(5, 5);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.enabledCheckedListBox);
      this.splitContainer1.Panel1.Controls.Add(this.label1);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.visibleCheckedListBox);
      this.splitContainer1.Panel2.Controls.Add(this.label2);
      this.splitContainer1.Size = new System.Drawing.Size(341, 165);
      this.splitContainer1.SplitterDistance = 167;
      this.splitContainer1.SplitterWidth = 3;
      this.splitContainer1.TabIndex = 0;
      // 
      // enabledCheckedListBox
      // 
      this.enabledCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.enabledCheckedListBox.FormattingEnabled = true;
      this.enabledCheckedListBox.IntegralHeight = false;
      this.enabledCheckedListBox.Location = new System.Drawing.Point(3, 16);
      this.enabledCheckedListBox.Name = "enabledCheckedListBox";
      this.enabledCheckedListBox.Size = new System.Drawing.Size(162, 147);
      this.enabledCheckedListBox.TabIndex = 1;
      this.enabledCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.enabledCheckedListBox_ItemCheck);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(-3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(91, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Enabled Handles:";
      // 
      // visibleCheckedListBox
      // 
      this.visibleCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.visibleCheckedListBox.FormattingEnabled = true;
      this.visibleCheckedListBox.IntegralHeight = false;
      this.visibleCheckedListBox.Location = new System.Drawing.Point(3, 16);
      this.visibleCheckedListBox.Name = "visibleCheckedListBox";
      this.visibleCheckedListBox.Size = new System.Drawing.Size(166, 147);
      this.visibleCheckedListBox.TabIndex = 1;
      this.visibleCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.visibleCheckedListBox_ItemCheck);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(0, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(82, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Visible Handles:";
      // 
      // propertiesTabPage
      // 
      this.propertiesTabPage.Controls.Add(this.propertyGrid);
      this.propertiesTabPage.Location = new System.Drawing.Point(4, 24);
      this.propertiesTabPage.Name = "propertiesTabPage";
      this.propertiesTabPage.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
      this.propertiesTabPage.Size = new System.Drawing.Size(351, 175);
      this.propertiesTabPage.TabIndex = 2;
      this.propertiesTabPage.Text = "Properties";
      this.propertiesTabPage.UseVisualStyleBackColor = true;
      // 
      // propertyGrid
      // 
      this.propertyGrid.CommandsVisibleIfAvailable = false;
      this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.propertyGrid.HelpVisible = false;
      this.propertyGrid.Location = new System.Drawing.Point(5, 5);
      this.propertyGrid.Name = "propertyGrid";
      this.propertyGrid.SelectedObject = this.imageBox;
      this.propertyGrid.Size = new System.Drawing.Size(341, 165);
      this.propertyGrid.TabIndex = 0;
      this.propertyGrid.ToolbarVisible = false;
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
      this.menuStrip.Size = new System.Drawing.Size(726, 24);
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
      // editToolStripMenuItem
      // 
      this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem});
      this.editToolStripMenuItem.Name = "editToolStripMenuItem";
      this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
      this.editToolStripMenuItem.Text = "&Edit";
      // 
      // selectAllToolStripMenuItem
      // 
      this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
      this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
      this.selectAllToolStripMenuItem.Text = "Select &All";
      this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
      // 
      // selectNoneToolStripMenuItem
      // 
      this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
      this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
      this.selectNoneToolStripMenuItem.Text = "Select &None";
      this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
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
            this.cursorToolStripStatusLabel,
            this.statusToolStripStatusLabel,
            this.autoScrollPositionToolStripStatusLabel,
            this.imageSizeToolStripStatusLabel,
            this.zoomToolStripStatusLabel,
            this.selectionToolStripStatusLabel});
      this.statusStrip.Location = new System.Drawing.Point(0, 385);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
      this.statusStrip.Size = new System.Drawing.Size(726, 22);
      this.statusStrip.TabIndex = 7;
      // 
      // cursorToolStripStatusLabel
      // 
      this.cursorToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.Cursor;
      this.cursorToolStripStatusLabel.Name = "cursorToolStripStatusLabel";
      this.cursorToolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
      this.cursorToolStripStatusLabel.ToolTipText = "Current Cursor Position";
      // 
      // statusToolStripStatusLabel
      // 
      this.statusToolStripStatusLabel.Name = "statusToolStripStatusLabel";
      this.statusToolStripStatusLabel.Size = new System.Drawing.Size(665, 17);
      this.statusToolStripStatusLabel.Spring = true;
      this.statusToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // autoScrollPositionToolStripStatusLabel
      // 
      this.autoScrollPositionToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.Position;
      this.autoScrollPositionToolStripStatusLabel.Name = "autoScrollPositionToolStripStatusLabel";
      this.autoScrollPositionToolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
      this.autoScrollPositionToolStripStatusLabel.ToolTipText = "Auto Scroll Position";
      this.autoScrollPositionToolStripStatusLabel.Visible = false;
      // 
      // imageSizeToolStripStatusLabel
      // 
      this.imageSizeToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.Size;
      this.imageSizeToolStripStatusLabel.Name = "imageSizeToolStripStatusLabel";
      this.imageSizeToolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
      this.imageSizeToolStripStatusLabel.ToolTipText = "Image Size";
      this.imageSizeToolStripStatusLabel.Visible = false;
      // 
      // zoomToolStripStatusLabel
      // 
      this.zoomToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.Zoom;
      this.zoomToolStripStatusLabel.Name = "zoomToolStripStatusLabel";
      this.zoomToolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
      this.zoomToolStripStatusLabel.ToolTipText = "Zoom";
      // 
      // selectionToolStripStatusLabel
      // 
      this.selectionToolStripStatusLabel.Image = global::Accord.Controls.Cyotek.Demo.Properties.Resources.SelectAll;
      this.selectionToolStripStatusLabel.Name = "selectionToolStripStatusLabel";
      this.selectionToolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
      // 
      // ResizableSelectionDemoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(726, 407);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.menuStrip);
      this.Controls.Add(this.statusStrip);
      this.Name = "ResizableSelectionDemoForm";
      this.Text = "Resizable Selection Demonstration";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.eventsSplitContainer.Panel1.ResumeLayout(false);
      this.eventsSplitContainer.Panel2.ResumeLayout(false);
      this.eventsSplitContainer.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.eventsTabPage.ResumeLayout(false);
      this.dragHandlesTabPage.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      this.splitContainer1.ResumeLayout(false);
      this.propertiesTabPage.ResumeLayout(false);
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ImageBoxEx imageBox;
    private System.Windows.Forms.Label demoLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel selectionToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel cursorToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel statusToolStripStatusLabel;
    private System.Windows.Forms.SplitContainer eventsSplitContainer;
    private EventsListBox eventsListBox;
    private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
    private System.Windows.Forms.ToolStripStatusLabel zoomToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel autoScrollPositionToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel imageSizeToolStripStatusLabel;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage eventsTabPage;
    private System.Windows.Forms.TabPage dragHandlesTabPage;
    private System.Windows.Forms.TabPage propertiesTabPage;
    private System.Windows.Forms.PropertyGrid propertyGrid;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.CheckedListBox enabledCheckedListBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckedListBox visibleCheckedListBox;
    private System.Windows.Forms.Label label2;
  }
}