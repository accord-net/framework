namespace ShapeChecker
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
            this.components = new System.ComponentModel.Container( );
            this.pictureBox = new System.Windows.Forms.PictureBox( );
            this.menuStrip = new System.Windows.Forms.MenuStrip( );
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator( );
            this.loadDemoImage1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.loadDemoImage2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.loadDemoImage3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.loadDemoImage4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator( );
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.label9 = new System.Windows.Forms.Label( );
            this.label10 = new System.Windows.Forms.Label( );
            this.label7 = new System.Windows.Forms.Label( );
            this.label8 = new System.Windows.Forms.Label( );
            this.label5 = new System.Windows.Forms.Label( );
            this.label6 = new System.Windows.Forms.Label( );
            this.label3 = new System.Windows.Forms.Label( );
            this.label4 = new System.Windows.Forms.Label( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog( );
            this.splitContainer = new System.Windows.Forms.SplitContainer( );
            this.toolTip = new System.Windows.Forms.ToolTip( this.components );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).BeginInit( );
            this.menuStrip.SuspendLayout( );
            this.groupBox1.SuspendLayout( );
            this.splitContainer.Panel1.SuspendLayout( );
            this.splitContainer.Panel2.SuspendLayout( );
            this.splitContainer.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point( 102, 32 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 320, 240 );
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem} );
            this.menuStrip.Location = new System.Drawing.Point( 0, 0 );
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size( 524, 24 );
            this.menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.loadDemoImage1ToolStripMenuItem,
            this.loadDemoImage2ToolStripMenuItem,
            this.loadDemoImage3ToolStripMenuItem,
            this.loadDemoImage4ToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 37, 20 );
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O ) ) );
            this.openToolStripMenuItem.Size = new System.Drawing.Size( 179, 22 );
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler( this.openToolStripMenuItem_Click );
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size( 176, 6 );
            // 
            // loadDemoImage1ToolStripMenuItem
            // 
            this.loadDemoImage1ToolStripMenuItem.Name = "loadDemoImage1ToolStripMenuItem";
            this.loadDemoImage1ToolStripMenuItem.Size = new System.Drawing.Size( 179, 22 );
            this.loadDemoImage1ToolStripMenuItem.Text = "Load demo image &1";
            this.loadDemoImage1ToolStripMenuItem.Click += new System.EventHandler( this.loadDemoImage1ToolStripMenuItem_Click );
            // 
            // loadDemoImage2ToolStripMenuItem
            // 
            this.loadDemoImage2ToolStripMenuItem.Name = "loadDemoImage2ToolStripMenuItem";
            this.loadDemoImage2ToolStripMenuItem.Size = new System.Drawing.Size( 179, 22 );
            this.loadDemoImage2ToolStripMenuItem.Text = "Load demo image &2";
            this.loadDemoImage2ToolStripMenuItem.Click += new System.EventHandler( this.loadDemoImage2ToolStripMenuItem_Click );
            // 
            // loadDemoImage3ToolStripMenuItem
            // 
            this.loadDemoImage3ToolStripMenuItem.Name = "loadDemoImage3ToolStripMenuItem";
            this.loadDemoImage3ToolStripMenuItem.Size = new System.Drawing.Size( 179, 22 );
            this.loadDemoImage3ToolStripMenuItem.Text = "Load demo image &3";
            this.loadDemoImage3ToolStripMenuItem.Click += new System.EventHandler( this.loadDemoImage3ToolStripMenuItem_Click );
            // 
            // loadDemoImage4ToolStripMenuItem
            // 
            this.loadDemoImage4ToolStripMenuItem.Name = "loadDemoImage4ToolStripMenuItem";
            this.loadDemoImage4ToolStripMenuItem.Size = new System.Drawing.Size( 179, 22 );
            this.loadDemoImage4ToolStripMenuItem.Text = "Load demo image &4";
            this.loadDemoImage4ToolStripMenuItem.Click += new System.EventHandler( this.loadDemoImage4ToolStripMenuItem_Click );
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size( 176, 6 );
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size( 179, 22 );
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
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox1.Controls.Add( this.label9 );
            this.groupBox1.Controls.Add( this.label10 );
            this.groupBox1.Controls.Add( this.label7 );
            this.groupBox1.Controls.Add( this.label8 );
            this.groupBox1.Controls.Add( this.label5 );
            this.groupBox1.Controls.Add( this.label6 );
            this.groupBox1.Controls.Add( this.label3 );
            this.groupBox1.Controls.Add( this.label4 );
            this.groupBox1.Controls.Add( this.label2 );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Location = new System.Drawing.Point( 10, 1 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 502, 45 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Legend";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point( 410, 22 );
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size( 82, 13 );
            this.label9.TabIndex = 9;
            this.label9.Text = "Known triangles";
            this.toolTip.SetToolTip( this.label9, "Equilateral, Isosceles, Rectangled or Rectangled Isosceles Triangle" );
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Green;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point( 390, 18 );
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size( 20, 20 );
            this.label10.TabIndex = 8;
            this.toolTip.SetToolTip( this.label10, "Equilateral, Isosceles, Rectangled or Rectangled Isosceles Triangle" );
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point( 330, 22 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 50, 13 );
            this.label7.TabIndex = 7;
            this.label7.Text = "Triangles";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Blue;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point( 310, 18 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 20, 20 );
            this.label8.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point( 200, 22 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 105, 13 );
            this.label5.TabIndex = 5;
            this.label5.Text = "Known quadrilaterals";
            this.toolTip.SetToolTip( this.label5, "Trapezoid, Parallelogram, Rectangle, Rhombus or Square" );
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Brown;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point( 180, 18 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 20, 20 );
            this.label6.TabIndex = 4;
            this.toolTip.SetToolTip( this.label6, "Trapezoid, Parallelogram, Rectangle, Rhombus or Square" );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 100, 22 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 71, 13 );
            this.label3.TabIndex = 3;
            this.label3.Text = "Quadrilaterals";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Red;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point( 80, 18 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 20, 20 );
            this.label4.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 30, 22 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 38, 13 );
            this.label2.TabIndex = 1;
            this.label2.Text = "Circles";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Yellow;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point( 10, 18 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 20, 20 );
            this.label1.TabIndex = 0;
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
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add( this.groupBox1 );
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add( this.pictureBox );
            this.splitContainer.Panel2.Resize += new System.EventHandler( this.splitContainer_Panel2_Resize );
            this.splitContainer.Size = new System.Drawing.Size( 524, 358 );
            this.splitContainer.SplitterDistance = 51;
            this.splitContainer.TabIndex = 1;
            this.splitContainer.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size( 524, 382 );
            this.Controls.Add( this.splitContainer );
            this.Controls.Add( this.menuStrip );
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size( 540, 420 );
            this.Name = "MainForm";
            this.Text = "Simple Shape Checker";
            this.Load += new System.EventHandler( this.MainForm_Load );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).EndInit( );
            this.menuStrip.ResumeLayout( false );
            this.menuStrip.PerformLayout( );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.splitContainer.Panel1.ResumeLayout( false );
            this.splitContainer.Panel2.ResumeLayout( false );
            this.splitContainer.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadDemoImage1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDemoImage2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDemoImage3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDemoImage4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

