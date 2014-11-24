namespace SVSTest
{
    partial class AboutForm
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
            this.panel1 = new System.Windows.Forms.Panel( );
            this.emailLabel = new System.Windows.Forms.LinkLabel( );
            this.label3 = new System.Windows.Forms.Label( );
            this.aforgeLabel = new System.Windows.Forms.LinkLabel( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.pictureBox1 = new System.Windows.Forms.PictureBox( );
            this.panel1.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.emailLabel );
            this.panel1.Controls.Add( this.label3 );
            this.panel1.Controls.Add( this.aforgeLabel );
            this.panel1.Controls.Add( this.label2 );
            this.panel1.Controls.Add( this.label1 );
            this.panel1.Location = new System.Drawing.Point( 115, 12 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 242, 139 );
            this.panel1.TabIndex = 5;
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point( 45, 108 );
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size( 153, 13 );
            this.emailLabel.TabIndex = 5;
            this.emailLabel.TabStop = true;
            this.emailLabel.Text = "andrew.kirillov@aforgenet.com";
            this.emailLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LinkClicked );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 25, 93 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 192, 13 );
            this.label3.TabIndex = 4;
            this.label3.Text = "Copyright © 2009-2010, Andrew Kirillov";
            // 
            // aforgeLabel
            // 
            this.aforgeLabel.AutoSize = true;
            this.aforgeLabel.Location = new System.Drawing.Point( 25, 58 );
            this.aforgeLabel.Name = "aforgeLabel";
            this.aforgeLabel.Size = new System.Drawing.Size( 192, 13 );
            this.aforgeLabel.TabIndex = 3;
            this.aforgeLabel.TabStop = true;
            this.aforgeLabel.Text = "http://www.aforgenet.com/framework/";
            this.aforgeLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LinkClicked );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 9, 43 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 225, 13 );
            this.label2.TabIndex = 2;
            this.label2.Text = "Sample application of AForge.NET framework:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 59, 18 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 124, 13 );
            this.label1.TabIndex = 1;
            this.label1.Text = "Surveyor SVS Test 1.0.2";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::SVSTest.Properties.Resources.svs;
            this.pictureBox1.Location = new System.Drawing.Point( 10, 52 );
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size( 98, 59 );
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 369, 162 );
            this.Controls.Add( this.pictureBox1 );
            this.Controls.Add( this.panel1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.panel1.ResumeLayout( false );
            this.panel1.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).EndInit( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel emailLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel aforgeLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}