namespace BlobsExplorer
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
            this.mainMenu = new System.Windows.Forms.MenuStrip( );
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.loaddemoImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator( );
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.statusStrip = new System.Windows.Forms.StatusStrip( );
            this.blobsCountLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog( );
            this.splitContainer = new System.Windows.Forms.SplitContainer( );
            this.blobsBrowser = new BlobsExplorer.BlobsBrowser( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.highlightTypeCombo = new System.Windows.Forms.ComboBox( );
            this.propertyGrid = new System.Windows.Forms.PropertyGrid( );
            this.showRectangleAroundSelectionCheck = new System.Windows.Forms.CheckBox( );
            this.mainMenu.SuspendLayout( );
            this.statusStrip.SuspendLayout( );
            this.splitContainer.Panel1.SuspendLayout( );
            this.splitContainer.Panel2.SuspendLayout( );
            this.splitContainer.SuspendLayout( );
            this.groupBox1.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem} );
            this.mainMenu.Location = new System.Drawing.Point( 0, 0 );
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size( 711, 24 );
            this.mainMenu.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.loaddemoImageToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 37, 20 );
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O ) ) );
            this.openToolStripMenuItem.Size = new System.Drawing.Size( 170, 22 );
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler( this.openToolStripMenuItem_Click );
            // 
            // loaddemoImageToolStripMenuItem
            // 
            this.loaddemoImageToolStripMenuItem.Name = "loaddemoImageToolStripMenuItem";
            this.loaddemoImageToolStripMenuItem.Size = new System.Drawing.Size( 170, 22 );
            this.loaddemoImageToolStripMenuItem.Text = "Load &demo image";
            this.loaddemoImageToolStripMenuItem.Click += new System.EventHandler( this.loaddemoImageToolStripMenuItem_Click );
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size( 167, 6 );
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size( 170, 22 );
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler( this.exitToolStripMenuItem_Click );
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
            this.blobsCountLabel} );
            this.statusStrip.Location = new System.Drawing.Point( 0, 414 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 711, 22 );
            this.statusStrip.TabIndex = 1;
            // 
            // blobsCountLabel
            // 
            this.blobsCountLabel.AutoSize = false;
            this.blobsCountLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.blobsCountLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.blobsCountLabel.Name = "blobsCountLabel";
            this.blobsCountLabel.Size = new System.Drawing.Size( 696, 17 );
            this.blobsCountLabel.Spring = true;
            this.blobsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Image files (*.jpg,*.png,*.tif,*.bmp,*.gif)|*.jpg;*.png;*.tif;*.bmp;*.gif|JPG fil" +
                "es (*.jpg)|*.jpg|PNG files (*.png)|*.png|TIF files (*.tif)|*.tif|BMP files (*.bm" +
                "p)|*.bmp|GIF files (*.gif)|*.gif";
            this.openFileDialog.Title = "Open image file";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point( 0, 24 );
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add( this.blobsBrowser );
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add( this.groupBox1 );
            this.splitContainer.Panel2.Controls.Add( this.propertyGrid );
            this.splitContainer.Panel2MinSize = 100;
            this.splitContainer.Size = new System.Drawing.Size( 711, 390 );
            this.splitContainer.SplitterDistance = 498;
            this.splitContainer.TabIndex = 2;
            // 
            // blobsBrowser
            // 
            this.blobsBrowser.Highlighting = BlobsExplorer.BlobsBrowser.HightlightType.Quadrilateral;
            this.blobsBrowser.Location = new System.Drawing.Point( 88, 74 );
            this.blobsBrowser.Name = "blobsBrowser";
            this.blobsBrowser.Size = new System.Drawing.Size( 322, 242 );
            this.blobsBrowser.TabIndex = 0;
            this.blobsBrowser.BlobSelected += new BlobsExplorer.BlobSelectionHandler( this.blobsBrowser_BlobSelected );
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox1.Controls.Add( this.showRectangleAroundSelectionCheck );
            this.groupBox1.Controls.Add( this.highlightTypeCombo );
            this.groupBox1.Location = new System.Drawing.Point( 3, 3 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 200, 65 );
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Highlight Type";
            // 
            // highlightTypeCombo
            // 
            this.highlightTypeCombo.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.highlightTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.highlightTypeCombo.FormattingEnabled = true;
            this.highlightTypeCombo.Items.AddRange( new object[] {
            "Convex Hull",
            "Left/Right Edges",
            "Top/Bottom Edges",
            "Quadrilateral"} );
            this.highlightTypeCombo.Location = new System.Drawing.Point( 6, 20 );
            this.highlightTypeCombo.Name = "highlightTypeCombo";
            this.highlightTypeCombo.Size = new System.Drawing.Size( 188, 21 );
            this.highlightTypeCombo.TabIndex = 0;
            this.highlightTypeCombo.SelectedIndexChanged += new System.EventHandler( this.highlightTypeCombo_SelectedIndexChanged );
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point( 0, 74 );
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size( 209, 316 );
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // showRectangleAroundSelectionCheck
            // 
            this.showRectangleAroundSelectionCheck.AutoSize = true;
            this.showRectangleAroundSelectionCheck.Location = new System.Drawing.Point( 6, 45 );
            this.showRectangleAroundSelectionCheck.Name = "showRectangleAroundSelectionCheck";
            this.showRectangleAroundSelectionCheck.Size = new System.Drawing.Size( 181, 17 );
            this.showRectangleAroundSelectionCheck.TabIndex = 1;
            this.showRectangleAroundSelectionCheck.Text = "Show rectangle around selection";
            this.showRectangleAroundSelectionCheck.UseVisualStyleBackColor = true;
            this.showRectangleAroundSelectionCheck.CheckedChanged += new System.EventHandler( this.showRectangleAroundSelectionCheck_CheckedChanged );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 711, 436 );
            this.Controls.Add( this.splitContainer );
            this.Controls.Add( this.statusStrip );
            this.Controls.Add( this.mainMenu );
            this.MainMenuStrip = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size( 480, 360 );
            this.Name = "MainForm";
            this.Text = "Blobs Explorer";
            this.Load += new System.EventHandler( this.MainForm_Load );
            this.mainMenu.ResumeLayout( false );
            this.mainMenu.PerformLayout( );
            this.statusStrip.ResumeLayout( false );
            this.statusStrip.PerformLayout( );
            this.splitContainer.Panel1.ResumeLayout( false );
            this.splitContainer.Panel2.ResumeLayout( false );
            this.splitContainer.ResumeLayout( false );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private BlobsBrowser blobsBrowser;
        private System.Windows.Forms.ToolStripStatusLabel blobsCountLabel;
        private System.Windows.Forms.ToolStripMenuItem loaddemoImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox highlightTypeCombo;
        private System.Windows.Forms.CheckBox showRectangleAroundSelectionCheck;
    }
}

