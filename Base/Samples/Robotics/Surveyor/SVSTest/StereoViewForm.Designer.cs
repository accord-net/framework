namespace SVSTest
{
    partial class StereoViewForm
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
            this.pictureBox = new System.Windows.Forms.PictureBox( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point( 11, 11 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 322, 242 );
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // StereoViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 344, 264 );
            this.Controls.Add( this.pictureBox );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StereoViewForm";
            this.ShowInTaskbar = false;
            this.Text = "Stereo View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.StereoViewForm_FormClosing );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).EndInit( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
    }
}