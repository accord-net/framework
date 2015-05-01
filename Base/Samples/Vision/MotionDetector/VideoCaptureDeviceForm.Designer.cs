namespace MotionDetectorSample
{
    partial class VideoCaptureDeviceForm
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
            this.label1 = new System.Windows.Forms.Label( );
            this.devicesCombo = new System.Windows.Forms.ComboBox( );
            this.cancelButton = new System.Windows.Forms.Button( );
            this.okButton = new System.Windows.Forms.Button( );
            this.SuspendLayout( );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 10 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 168, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Select local video capture device:";
            // 
            // devicesCombo
            // 
            this.devicesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.devicesCombo.FormattingEnabled = true;
            this.devicesCombo.Location = new System.Drawing.Point( 10, 35 );
            this.devicesCombo.Name = "devicesCombo";
            this.devicesCombo.Size = new System.Drawing.Size( 325, 21 );
            this.devicesCombo.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Location = new System.Drawing.Point( 180, 80 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Location = new System.Drawing.Point( 90, 80 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 6;
            this.okButton.Text = "Ok";
            this.okButton.Click += new System.EventHandler( this.okButton_Click );
            // 
            // VideoCaptureDeviceForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 344, 116 );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.Controls.Add( this.devicesCombo );
            this.Controls.Add( this.label1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VideoCaptureDeviceForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open local  video capture device";
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox devicesCombo;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}