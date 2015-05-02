namespace AForge.Video.DirectShow
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
            this.cancelButton = new System.Windows.Forms.Button( );
            this.okButton = new System.Windows.Forms.Button( );
            this.devicesCombo = new System.Windows.Forms.ComboBox( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.videoInputsCombo = new System.Windows.Forms.ComboBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.snapshotsLabel = new System.Windows.Forms.Label( );
            this.snapshotResolutionsCombo = new System.Windows.Forms.ComboBox( );
            this.videoResolutionsCombo = new System.Windows.Forms.ComboBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.pictureBox = new System.Windows.Forms.PictureBox( );
            this.groupBox1.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point( 239, 190 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point( 149, 190 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler( this.okButton_Click );
            // 
            // devicesCombo
            // 
            this.devicesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.devicesCombo.FormattingEnabled = true;
            this.devicesCombo.Location = new System.Drawing.Point( 100, 40 );
            this.devicesCombo.Name = "devicesCombo";
            this.devicesCombo.Size = new System.Drawing.Size( 325, 21 );
            this.devicesCombo.TabIndex = 9;
            this.devicesCombo.SelectedIndexChanged += new System.EventHandler( this.devicesCombo_SelectedIndexChanged );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.videoInputsCombo );
            this.groupBox1.Controls.Add( this.label3 );
            this.groupBox1.Controls.Add( this.snapshotsLabel );
            this.groupBox1.Controls.Add( this.snapshotResolutionsCombo );
            this.groupBox1.Controls.Add( this.videoResolutionsCombo );
            this.groupBox1.Controls.Add( this.label2 );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Controls.Add( this.pictureBox );
            this.groupBox1.Controls.Add( this.devicesCombo );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 440, 165 );
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Video capture device settings";
            // 
            // videoInputsCombo
            // 
            this.videoInputsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.videoInputsCombo.FormattingEnabled = true;
            this.videoInputsCombo.Location = new System.Drawing.Point( 100, 130 );
            this.videoInputsCombo.Name = "videoInputsCombo";
            this.videoInputsCombo.Size = new System.Drawing.Size( 150, 21 );
            this.videoInputsCombo.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 100, 115 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 63, 13 );
            this.label3.TabIndex = 16;
            this.label3.Text = "Video input:";
            // 
            // snapshotsLabel
            // 
            this.snapshotsLabel.AutoSize = true;
            this.snapshotsLabel.Location = new System.Drawing.Point( 275, 70 );
            this.snapshotsLabel.Name = "snapshotsLabel";
            this.snapshotsLabel.Size = new System.Drawing.Size( 101, 13 );
            this.snapshotsLabel.TabIndex = 15;
            this.snapshotsLabel.Text = "Snapshot resoluton:";
            // 
            // snapshotResolutionsCombo
            // 
            this.snapshotResolutionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.snapshotResolutionsCombo.FormattingEnabled = true;
            this.snapshotResolutionsCombo.Location = new System.Drawing.Point( 275, 85 );
            this.snapshotResolutionsCombo.Name = "snapshotResolutionsCombo";
            this.snapshotResolutionsCombo.Size = new System.Drawing.Size( 150, 21 );
            this.snapshotResolutionsCombo.TabIndex = 14;
            // 
            // videoResolutionsCombo
            // 
            this.videoResolutionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.videoResolutionsCombo.FormattingEnabled = true;
            this.videoResolutionsCombo.Location = new System.Drawing.Point( 100, 85 );
            this.videoResolutionsCombo.Name = "videoResolutionsCombo";
            this.videoResolutionsCombo.Size = new System.Drawing.Size( 150, 21 );
            this.videoResolutionsCombo.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 100, 70 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 83, 13 );
            this.label2.TabIndex = 12;
            this.label2.Text = "Video resoluton:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 100, 25 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 72, 13 );
            this.label1.TabIndex = 11;
            this.label1.Text = "Video device:";
            // 
            // pictureBox
            // 
            this.pictureBox.Image = global::AForge.Video.DirectShow.Properties.Resources.camera;
            this.pictureBox.Location = new System.Drawing.Point( 20, 28 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 64, 64 );
            this.pictureBox.TabIndex = 10;
            this.pictureBox.TabStop = false;
            // 
            // VideoCaptureDeviceForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 462, 221 );
            this.Controls.Add( this.groupBox1 );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "VideoCaptureDeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open local  video capture device";
            this.Load += new System.EventHandler( this.VideoCaptureDeviceForm_Load );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).EndInit( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox devicesCombo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label snapshotsLabel;
        private System.Windows.Forms.ComboBox snapshotResolutionsCombo;
        private System.Windows.Forms.ComboBox videoResolutionsCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox videoInputsCombo;
        private System.Windows.Forms.Label label3;
    }
}