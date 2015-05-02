namespace SRVTest
{
    partial class SrvDriverControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.stopButton = new System.Windows.Forms.Button( );
            this.forwardButton = new System.Windows.Forms.Button( );
            this.backwardButton = new System.Windows.Forms.Button( );
            this.leftButton = new System.Windows.Forms.Button( );
            this.leftBackwardButton = new System.Windows.Forms.Button( );
            this.rightBackwardButton = new System.Windows.Forms.Button( );
            this.leftDriftButton = new System.Windows.Forms.Button( );
            this.rightButton = new System.Windows.Forms.Button( );
            this.rightDriftButton = new System.Windows.Forms.Button( );
            this.speedUpButton = new System.Windows.Forms.Button( );
            this.slowDownButton = new System.Windows.Forms.Button( );
            this.SuspendLayout( );
            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.Color.LightCoral;
            this.stopButton.Location = new System.Drawing.Point( 100, 40 );
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size( 90, 30 );
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "&Stop";
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler( this.stopButton_Click );
            // 
            // forwardButton
            // 
            this.forwardButton.Location = new System.Drawing.Point( 100, 0 );
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size( 90, 30 );
            this.forwardButton.TabIndex = 1;
            this.forwardButton.Text = "&Forward";
            this.forwardButton.UseVisualStyleBackColor = true;
            this.forwardButton.Click += new System.EventHandler( this.forwardButton_Click );
            // 
            // backwardButton
            // 
            this.backwardButton.Location = new System.Drawing.Point( 100, 80 );
            this.backwardButton.Name = "backwardButton";
            this.backwardButton.Size = new System.Drawing.Size( 90, 30 );
            this.backwardButton.TabIndex = 7;
            this.backwardButton.Text = "&Backward";
            this.backwardButton.UseVisualStyleBackColor = true;
            this.backwardButton.Click += new System.EventHandler( this.backwardButton_Click );
            // 
            // leftButton
            // 
            this.leftButton.Location = new System.Drawing.Point( 0, 0 );
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size( 90, 30 );
            this.leftButton.TabIndex = 0;
            this.leftButton.Text = "&Left";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler( this.leftButton_Click );
            // 
            // leftBackwardButton
            // 
            this.leftBackwardButton.Location = new System.Drawing.Point( 0, 80 );
            this.leftBackwardButton.Name = "leftBackwardButton";
            this.leftBackwardButton.Size = new System.Drawing.Size( 90, 30 );
            this.leftBackwardButton.TabIndex = 6;
            this.leftBackwardButton.Text = "Left backword";
            this.leftBackwardButton.UseVisualStyleBackColor = true;
            this.leftBackwardButton.Click += new System.EventHandler( this.leftBackwardButton_Click );
            // 
            // rightBackwardButton
            // 
            this.rightBackwardButton.Location = new System.Drawing.Point( 200, 80 );
            this.rightBackwardButton.Name = "rightBackwardButton";
            this.rightBackwardButton.Size = new System.Drawing.Size( 90, 30 );
            this.rightBackwardButton.TabIndex = 8;
            this.rightBackwardButton.Text = "Right backword";
            this.rightBackwardButton.UseVisualStyleBackColor = true;
            this.rightBackwardButton.Click += new System.EventHandler( this.rightBackwardButton_Click );
            // 
            // leftDriftButton
            // 
            this.leftDriftButton.Location = new System.Drawing.Point( 0, 40 );
            this.leftDriftButton.Name = "leftDriftButton";
            this.leftDriftButton.Size = new System.Drawing.Size( 90, 30 );
            this.leftDriftButton.TabIndex = 3;
            this.leftDriftButton.Text = "Left drift";
            this.leftDriftButton.UseVisualStyleBackColor = true;
            this.leftDriftButton.Click += new System.EventHandler( this.leftDriftButton_Click );
            // 
            // rightButton
            // 
            this.rightButton.Location = new System.Drawing.Point( 200, 0 );
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size( 90, 30 );
            this.rightButton.TabIndex = 2;
            this.rightButton.Text = "&Right";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler( this.rightButton_Click );
            // 
            // rightDriftButton
            // 
            this.rightDriftButton.Location = new System.Drawing.Point( 200, 40 );
            this.rightDriftButton.Name = "rightDriftButton";
            this.rightDriftButton.Size = new System.Drawing.Size( 90, 30 );
            this.rightDriftButton.TabIndex = 5;
            this.rightDriftButton.Text = "Right drift";
            this.rightDriftButton.UseVisualStyleBackColor = true;
            this.rightDriftButton.Click += new System.EventHandler( this.rightDriftButton_Click );
            // 
            // speedUpButton
            // 
            this.speedUpButton.Location = new System.Drawing.Point( 0, 130 );
            this.speedUpButton.Name = "speedUpButton";
            this.speedUpButton.Size = new System.Drawing.Size( 90, 30 );
            this.speedUpButton.TabIndex = 9;
            this.speedUpButton.Text = "Speed up";
            this.speedUpButton.UseVisualStyleBackColor = true;
            this.speedUpButton.Click += new System.EventHandler( this.speedUpButton_Click );
            // 
            // slowDownButton
            // 
            this.slowDownButton.Location = new System.Drawing.Point( 200, 130 );
            this.slowDownButton.Name = "slowDownButton";
            this.slowDownButton.Size = new System.Drawing.Size( 90, 30 );
            this.slowDownButton.TabIndex = 10;
            this.slowDownButton.Text = "Slow down";
            this.slowDownButton.UseVisualStyleBackColor = true;
            this.slowDownButton.Click += new System.EventHandler( this.slowDownButton_Click );
            // 
            // SrvDriverControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.slowDownButton );
            this.Controls.Add( this.speedUpButton );
            this.Controls.Add( this.rightDriftButton );
            this.Controls.Add( this.leftDriftButton );
            this.Controls.Add( this.rightBackwardButton );
            this.Controls.Add( this.leftBackwardButton );
            this.Controls.Add( this.rightButton );
            this.Controls.Add( this.leftButton );
            this.Controls.Add( this.backwardButton );
            this.Controls.Add( this.forwardButton );
            this.Controls.Add( this.stopButton );
            this.Name = "SrvDriverControl";
            this.Size = new System.Drawing.Size( 290, 160 );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button forwardButton;
        private System.Windows.Forms.Button backwardButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button leftBackwardButton;
        private System.Windows.Forms.Button rightBackwardButton;
        private System.Windows.Forms.Button leftDriftButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Button rightDriftButton;
        private System.Windows.Forms.Button speedUpButton;
        private System.Windows.Forms.Button slowDownButton;
    }
}
