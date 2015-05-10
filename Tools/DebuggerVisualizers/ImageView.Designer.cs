namespace AForge.DebuggerVisualizers
{
    partial class ImageView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ImageView ) );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.imagePanel = new System.Windows.Forms.Panel( );
            this.pictureBox = new AForge.Controls.PictureBox( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.saveButton = new System.Windows.Forms.Button( );
            this.clipboardButton = new System.Windows.Forms.Button( );
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog( );
            this.groupBox1.SuspendLayout( );
            this.imagePanel.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).BeginInit( );
            this.groupBox2.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox1.Controls.Add( this.imagePanel );
            this.groupBox1.Location = new System.Drawing.Point( 10, 75 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 362, 333 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image";
            // 
            // imagePanel
            // 
            this.imagePanel.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.imagePanel.AutoScroll = true;
            this.imagePanel.Controls.Add( this.pictureBox );
            this.imagePanel.Location = new System.Drawing.Point( 10, 20 );
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size( 342, 302 );
            this.imagePanel.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Image = null;
            this.pictureBox.Location = new System.Drawing.Point( 50, 51 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 243, 193 );
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox2.Controls.Add( this.clipboardButton );
            this.groupBox2.Controls.Add( this.saveButton );
            this.groupBox2.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 362, 55 );
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point( 10, 20 );
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size( 75, 23 );
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "&Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler( this.saveButton_Click );
            // 
            // clipboardButton
            // 
            this.clipboardButton.Location = new System.Drawing.Point( 100, 20 );
            this.clipboardButton.Name = "clipboardButton";
            this.clipboardButton.Size = new System.Drawing.Size( 100, 23 );
            this.clipboardButton.TabIndex = 1;
            this.clipboardButton.Text = "Put to &Clipboard";
            this.clipboardButton.UseVisualStyleBackColor = true;
            this.clipboardButton.Click += new System.EventHandler( this.clipboardButton_Click );
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp";
            this.saveFileDialog.Title = "Save image";
            // 
            // ImageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 384, 418 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "$this.Icon" ) ) );
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size( 160, 120 );
            this.Name = "ImageView";
            this.ShowInTaskbar = false;
            this.Text = "ImageView";
            this.Load += new System.EventHandler( this.ImageView_Load );
            this.Resize += new System.EventHandler( this.ImageView_Resize );
            this.groupBox1.ResumeLayout( false );
            this.imagePanel.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).EndInit( );
            this.groupBox2.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel imagePanel;
        private AForge.Controls.PictureBox pictureBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button clipboardButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}