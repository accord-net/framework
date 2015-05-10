namespace IPPrototyper
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
            this.menuStrip = new System.Windows.Forms.MenuStrip( );
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator( );
            this.recentFoldersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator( );
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.copyImageClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.showhistogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.modulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.imageviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.centerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.stretchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openLastFolderOnStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.statusStrip = new System.Windows.Forms.StatusStrip( );
            this.imagesCountLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.processingTimeLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.imageSizeLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel( );
            this.mainPanel = new System.Windows.Forms.Panel( );
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer( );
            this.splitContainer1 = new System.Windows.Forms.SplitContainer( );
            this.filesListView = new System.Windows.Forms.ListView( );
            this.fileNameColumn = new System.Windows.Forms.ColumnHeader( );
            this.logListView = new System.Windows.Forms.ListView( );
            this.processingStepsColumn = new System.Windows.Forms.ColumnHeader( );
            this.splitContainer2 = new System.Windows.Forms.SplitContainer( );
            this.pictureBox = new System.Windows.Forms.PictureBox( );
            this.logBox = new System.Windows.Forms.TextBox( );
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog( );
            this.autoSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.menuStrip.SuspendLayout( );
            this.statusStrip.SuspendLayout( );
            this.mainPanel.SuspendLayout( );
            this.mainSplitContainer.Panel1.SuspendLayout( );
            this.mainSplitContainer.Panel2.SuspendLayout( );
            this.mainSplitContainer.SuspendLayout( );
            this.splitContainer1.Panel1.SuspendLayout( );
            this.splitContainer1.Panel2.SuspendLayout( );
            this.splitContainer1.SuspendLayout( );
            this.splitContainer2.Panel1.SuspendLayout( );
            this.splitContainer2.Panel2.SuspendLayout( );
            this.splitContainer2.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.modulesToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem} );
            this.menuStrip.Location = new System.Drawing.Point( 0, 0 );
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size( 694, 24 );
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem,
            this.toolStripMenuItem1,
            this.recentFoldersToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 37, 20 );
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O ) ) );
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size( 180, 22 );
            this.openFolderToolStripMenuItem.Text = "&Open folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler( this.openFolderToolStripMenuItem_Click );
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size( 177, 6 );
            // 
            // recentFoldersToolStripMenuItem
            // 
            this.recentFoldersToolStripMenuItem.Name = "recentFoldersToolStripMenuItem";
            this.recentFoldersToolStripMenuItem.Size = new System.Drawing.Size( 180, 22 );
            this.recentFoldersToolStripMenuItem.Text = "&Recent Folders";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size( 177, 6 );
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size( 180, 22 );
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler( this.exitToolStripMenuItem_Click );
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.copyImageClipboardToolStripMenuItem,
            this.showhistogramToolStripMenuItem} );
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size( 48, 20 );
            this.toolsToolStripMenuItem.Text = "&Tools";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler( this.toolsToolStripMenuItem_DropDownOpening );
            // 
            // copyImageClipboardToolStripMenuItem
            // 
            this.copyImageClipboardToolStripMenuItem.Name = "copyImageClipboardToolStripMenuItem";
            this.copyImageClipboardToolStripMenuItem.Size = new System.Drawing.Size( 205, 22 );
            this.copyImageClipboardToolStripMenuItem.Text = "&Copy image to clipboard";
            this.copyImageClipboardToolStripMenuItem.Click += new System.EventHandler( this.copyImageClipboardToolStripMenuItem_Click );
            // 
            // showhistogramToolStripMenuItem
            // 
            this.showhistogramToolStripMenuItem.Name = "showhistogramToolStripMenuItem";
            this.showhistogramToolStripMenuItem.Size = new System.Drawing.Size( 205, 22 );
            this.showhistogramToolStripMenuItem.Text = "Show &histogram";
            this.showhistogramToolStripMenuItem.Click += new System.EventHandler( this.showhistogramToolStripMenuItem_Click );
            // 
            // modulesToolStripMenuItem
            // 
            this.modulesToolStripMenuItem.Name = "modulesToolStripMenuItem";
            this.modulesToolStripMenuItem.Size = new System.Drawing.Size( 65, 20 );
            this.modulesToolStripMenuItem.Text = "&Modules";
            this.modulesToolStripMenuItem.DropDownOpening += new System.EventHandler( this.modulesToolStripMenuItem_DropDownOpening );
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.imageviewToolStripMenuItem,
            this.openLastFolderOnStartToolStripMenuItem} );
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size( 61, 20 );
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // imageviewToolStripMenuItem
            // 
            this.imageviewToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.normalToolStripMenuItem,
            this.centerToolStripMenuItem,
            this.stretchToolStripMenuItem,
            this.autoSizeToolStripMenuItem} );
            this.imageviewToolStripMenuItem.Name = "imageviewToolStripMenuItem";
            this.imageviewToolStripMenuItem.Size = new System.Drawing.Size( 201, 22 );
            this.imageviewToolStripMenuItem.Text = "Image &view";
            this.imageviewToolStripMenuItem.DropDownOpening += new System.EventHandler( this.imageviewToolStripMenuItem_DropDownOpening );
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.normalToolStripMenuItem.Text = "&Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler( this.normalToolStripMenuItem_Click );
            // 
            // centerToolStripMenuItem
            // 
            this.centerToolStripMenuItem.Name = "centerToolStripMenuItem";
            this.centerToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.centerToolStripMenuItem.Text = "&Center";
            this.centerToolStripMenuItem.Click += new System.EventHandler( this.centerToolStripMenuItem_Click );
            // 
            // stretchToolStripMenuItem
            // 
            this.stretchToolStripMenuItem.Name = "stretchToolStripMenuItem";
            this.stretchToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.stretchToolStripMenuItem.Text = "&Stretch";
            this.stretchToolStripMenuItem.Click += new System.EventHandler( this.stretchToolStripMenuItem_Click );
            // 
            // openLastFolderOnStartToolStripMenuItem
            // 
            this.openLastFolderOnStartToolStripMenuItem.Name = "openLastFolderOnStartToolStripMenuItem";
            this.openLastFolderOnStartToolStripMenuItem.Size = new System.Drawing.Size( 201, 22 );
            this.openLastFolderOnStartToolStripMenuItem.Text = "&Open last folder on start";
            this.openLastFolderOnStartToolStripMenuItem.Click += new System.EventHandler( this.openLastFolderOnStartToolStripMenuItem_Click );
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem} );
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size( 44, 20 );
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size( 107, 22 );
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler( this.aboutToolStripMenuItem_Click );
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.imagesCountLabel,
            this.processingTimeLabel,
            this.imageSizeLabel,
            this.toolStripStatusLabel1} );
            this.statusStrip.Location = new System.Drawing.Point( 0, 436 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 694, 22 );
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // imagesCountLabel
            // 
            this.imagesCountLabel.AutoSize = false;
            this.imagesCountLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.imagesCountLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.imagesCountLabel.Name = "imagesCountLabel";
            this.imagesCountLabel.Size = new System.Drawing.Size( 150, 17 );
            this.imagesCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // processingTimeLabel
            // 
            this.processingTimeLabel.AutoSize = false;
            this.processingTimeLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.processingTimeLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.processingTimeLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.processingTimeLabel.Name = "processingTimeLabel";
            this.processingTimeLabel.Size = new System.Drawing.Size( 150, 17 );
            this.processingTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.processingTimeLabel.ToolTipText = "Time taken processing last image";
            // 
            // imageSizeLabel
            // 
            this.imageSizeLabel.AutoSize = false;
            this.imageSizeLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.imageSizeLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.imageSizeLabel.Name = "imageSizeLabel";
            this.imageSizeLabel.Size = new System.Drawing.Size( 150, 17 );
            this.imageSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.imageSizeLabel.ToolTipText = "Size of currently shown image";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size( 229, 17 );
            this.toolStripStatusLabel1.Spring = true;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add( this.mainSplitContainer );
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point( 0, 24 );
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size( 694, 412 );
            this.mainPanel.TabIndex = 2;
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point( 0, 0 );
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add( this.splitContainer1 );
            this.mainSplitContainer.Panel1.Resize += new System.EventHandler( this.mainSplitContainer_Panel1_Resize );
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add( this.splitContainer2 );
            this.mainSplitContainer.Size = new System.Drawing.Size( 694, 412 );
            this.mainSplitContainer.SplitterDistance = 185;
            this.mainSplitContainer.TabIndex = 4;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point( 0, 0 );
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add( this.filesListView );
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add( this.logListView );
            this.splitContainer1.Size = new System.Drawing.Size( 185, 412 );
            this.splitContainer1.SplitterDistance = 206;
            this.splitContainer1.TabIndex = 3;
            // 
            // filesListView
            // 
            this.filesListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.fileNameColumn} );
            this.filesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesListView.FullRowSelect = true;
            this.filesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.filesListView.HideSelection = false;
            this.filesListView.Location = new System.Drawing.Point( 0, 0 );
            this.filesListView.MultiSelect = false;
            this.filesListView.Name = "filesListView";
            this.filesListView.Size = new System.Drawing.Size( 185, 206 );
            this.filesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.filesListView.TabIndex = 0;
            this.filesListView.UseCompatibleStateImageBehavior = false;
            this.filesListView.View = System.Windows.Forms.View.Details;
            this.filesListView.SelectedIndexChanged += new System.EventHandler( this.filesListView_SelectedIndexChanged );
            // 
            // fileNameColumn
            // 
            this.fileNameColumn.Text = "File names";
            this.fileNameColumn.Width = 130;
            // 
            // logListView
            // 
            this.logListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.processingStepsColumn} );
            this.logListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logListView.FullRowSelect = true;
            this.logListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.logListView.HideSelection = false;
            this.logListView.Location = new System.Drawing.Point( 0, 0 );
            this.logListView.MultiSelect = false;
            this.logListView.Name = "logListView";
            this.logListView.Size = new System.Drawing.Size( 185, 202 );
            this.logListView.TabIndex = 2;
            this.logListView.UseCompatibleStateImageBehavior = false;
            this.logListView.View = System.Windows.Forms.View.Details;
            this.logListView.SelectedIndexChanged += new System.EventHandler( this.logListView_SelectedIndexChanged );
            // 
            // processingStepsColumn
            // 
            this.processingStepsColumn.Text = "Processing steps";
            this.processingStepsColumn.Width = 130;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point( 0, 0 );
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add( this.pictureBox );
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add( this.logBox );
            this.splitContainer2.Size = new System.Drawing.Size( 505, 412 );
            this.splitContainer2.SplitterDistance = 301;
            this.splitContainer2.TabIndex = 6;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point( 0, 0 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 505, 301 );
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Location = new System.Drawing.Point( 0, 0 );
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size( 505, 107 );
            this.logBox.TabIndex = 7;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select folder containing images to process:";
            // 
            // autoSizeToolStripMenuItem
            // 
            this.autoSizeToolStripMenuItem.Name = "autoSizeToolStripMenuItem";
            this.autoSizeToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
            this.autoSizeToolStripMenuItem.Text = "&Auto Size";
            this.autoSizeToolStripMenuItem.Click += new System.EventHandler( this.autoSizeToolStripMenuItem_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 694, 458 );
            this.Controls.Add( this.mainPanel );
            this.Controls.Add( this.statusStrip );
            this.Controls.Add( this.menuStrip );
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Image Processing Prototyper";
            this.Load += new System.EventHandler( this.MainForm_Load );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.menuStrip.ResumeLayout( false );
            this.menuStrip.PerformLayout( );
            this.statusStrip.ResumeLayout( false );
            this.statusStrip.PerformLayout( );
            this.mainPanel.ResumeLayout( false );
            this.mainSplitContainer.Panel1.ResumeLayout( false );
            this.mainSplitContainer.Panel2.ResumeLayout( false );
            this.mainSplitContainer.ResumeLayout( false );
            this.splitContainer1.Panel1.ResumeLayout( false );
            this.splitContainer1.Panel2.ResumeLayout( false );
            this.splitContainer1.ResumeLayout( false );
            this.splitContainer2.Panel1.ResumeLayout( false );
            this.splitContainer2.Panel2.ResumeLayout( false );
            this.splitContainer2.Panel2.PerformLayout( );
            this.splitContainer2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ListView filesListView;
        private System.Windows.Forms.ColumnHeader fileNameColumn;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ListView logListView;
        private System.Windows.Forms.ColumnHeader processingStepsColumn;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem centerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stretchToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem modulesToolStripMenuItem;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyImageClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showhistogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentFoldersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openLastFolderOnStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel imagesCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel processingTimeLabel;
        private System.Windows.Forms.ToolStripStatusLabel imageSizeLabel;
        private System.Windows.Forms.ToolStripMenuItem autoSizeToolStripMenuItem;
    }
}

