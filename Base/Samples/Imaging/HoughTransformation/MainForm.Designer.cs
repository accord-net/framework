namespace HoughTransform
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip( );
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator( );
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog( );
            this.tabControl = new System.Windows.Forms.TabControl( );
            this.tabPage1 = new System.Windows.Forms.TabPage( );
            this.sourcePictureBox = new System.Windows.Forms.PictureBox( );
            this.tabPage2 = new System.Windows.Forms.TabPage( );
            this.houghLinePictureBox = new System.Windows.Forms.PictureBox( );
            this.tabPage3 = new System.Windows.Forms.TabPage( );
            this.houghCirclePictureBox = new System.Windows.Forms.PictureBox( );
            this.menuStrip1.SuspendLayout( );
            this.tabControl.SuspendLayout( );
            this.tabPage1.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.sourcePictureBox ) ).BeginInit( );
            this.tabPage2.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.houghLinePictureBox ) ).BeginInit( );
            this.tabPage3.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.houghCirclePictureBox ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem} );
            this.menuStrip1.Location = new System.Drawing.Point( 0, 0 );
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size( 632, 24 );
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 35, 20 );
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O ) ) );
            this.openToolStripMenuItem.Size = new System.Drawing.Size( 151, 22 );
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler( this.openToolStripMenuItem_Click );
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size( 148, 6 );
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size( 151, 22 );
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler( this.exitToolStripMenuItem_Click );
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Image files (*.jpg,*.png,*.tif,*.bmp,*.gif)|*.jpg;*.png;*.tif;*.bmp;*.gif|JPG fil" +
                "es (*.jpg)|*.jpg|PNG files (*.png)|*.png|TIF files (*.tif)|*.tif|BMP files (*.bm" +
                "p)|*.bmp|GIF files (*.gif)|*.gif";
            this.openFileDialog.Title = "Open image";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add( this.tabPage1 );
            this.tabControl.Controls.Add( this.tabPage2 );
            this.tabControl.Controls.Add( this.tabPage3 );
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point( 0, 24 );
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size( 632, 442 );
            this.tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add( this.sourcePictureBox );
            this.tabPage1.Location = new System.Drawing.Point( 4, 22 );
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding( 3 );
            this.tabPage1.Size = new System.Drawing.Size( 624, 416 );
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Original image";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // sourcePictureBox
            // 
            this.sourcePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcePictureBox.Location = new System.Drawing.Point( 3, 3 );
            this.sourcePictureBox.Name = "sourcePictureBox";
            this.sourcePictureBox.Size = new System.Drawing.Size( 618, 410 );
            this.sourcePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.sourcePictureBox.TabIndex = 0;
            this.sourcePictureBox.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add( this.houghLinePictureBox );
            this.tabPage2.Location = new System.Drawing.Point( 4, 22 );
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding( 3 );
            this.tabPage2.Size = new System.Drawing.Size( 624, 416 );
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Hough lines";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // houghLinePictureBox
            // 
            this.houghLinePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.houghLinePictureBox.Location = new System.Drawing.Point( 3, 3 );
            this.houghLinePictureBox.Name = "houghLinePictureBox";
            this.houghLinePictureBox.Size = new System.Drawing.Size( 618, 410 );
            this.houghLinePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.houghLinePictureBox.TabIndex = 0;
            this.houghLinePictureBox.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add( this.houghCirclePictureBox );
            this.tabPage3.Location = new System.Drawing.Point( 4, 22 );
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size( 624, 416 );
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Hough circles";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // houghCirclePictureBox
            // 
            this.houghCirclePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.houghCirclePictureBox.Location = new System.Drawing.Point( 0, 0 );
            this.houghCirclePictureBox.Name = "houghCirclePictureBox";
            this.houghCirclePictureBox.Size = new System.Drawing.Size( 624, 416 );
            this.houghCirclePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.houghCirclePictureBox.TabIndex = 0;
            this.houghCirclePictureBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 632, 466 );
            this.Controls.Add( this.tabControl );
            this.Controls.Add( this.menuStrip1 );
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Hough transformation";
            this.menuStrip1.ResumeLayout( false );
            this.menuStrip1.PerformLayout( );
            this.tabControl.ResumeLayout( false );
            this.tabPage1.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) ( this.sourcePictureBox ) ).EndInit( );
            this.tabPage2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) ( this.houghLinePictureBox ) ).EndInit( );
            this.tabPage3.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) ( this.houghCirclePictureBox ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PictureBox sourcePictureBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox houghLinePictureBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PictureBox houghCirclePictureBox;
    }
}

