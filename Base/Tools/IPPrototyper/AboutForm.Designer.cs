namespace IPPrototyper
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
            this.emailLabel = new System.Windows.Forms.LinkLabel( );
            this.label3 = new System.Windows.Forms.Label( );
            this.aforgeFrameworkLabel = new System.Windows.Forms.LinkLabel( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.SuspendLayout( );
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point( 71, 99 );
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size( 153, 13 );
            this.emailLabel.TabIndex = 12;
            this.emailLabel.TabStop = true;
            this.emailLabel.Text = "andrew.kirillov@aforgenet.com";
            this.emailLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LinkClicked );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 65, 84 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 165, 13 );
            this.label3.TabIndex = 11;
            this.label3.Text = "Copyright © 2010, Andrew Kirillov";
            // 
            // aforgeFrameworkLabel
            // 
            this.aforgeFrameworkLabel.AutoSize = true;
            this.aforgeFrameworkLabel.Location = new System.Drawing.Point( 51, 59 );
            this.aforgeFrameworkLabel.Name = "aforgeFrameworkLabel";
            this.aforgeFrameworkLabel.Size = new System.Drawing.Size( 192, 13 );
            this.aforgeFrameworkLabel.TabIndex = 10;
            this.aforgeFrameworkLabel.TabStop = true;
            this.aforgeFrameworkLabel.Text = "http://www.aforgenet.com/framework/";
            this.aforgeFrameworkLabel.VisitedLinkColor = System.Drawing.Color.Blue;
            this.aforgeFrameworkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LinkClicked );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 63, 44 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 169, 13 );
            this.label2.TabIndex = 9;
            this.label2.Text = "Based on AForge.NET Framework";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 204 ) ) );
            this.label1.Location = new System.Drawing.Point( 46, 14 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 202, 13 );
            this.label1.TabIndex = 8;
            this.label1.Text = "Image Processing Prototyper 1.0.1";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 294, 126 );
            this.Controls.Add( this.emailLabel );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.aforgeFrameworkLabel );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About ...";
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.LinkLabel emailLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel aforgeFrameworkLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}