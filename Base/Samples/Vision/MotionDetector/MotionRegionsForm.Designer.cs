namespace MotionDetectorSample
{
    partial class MotionRegionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MotionRegionsForm ) );
            this.toolStrip = new System.Windows.Forms.ToolStrip( );
            this.rectangleButton = new System.Windows.Forms.ToolStripButton( );
            this.okButton = new System.Windows.Forms.Button( );
            this.cancelButton = new System.Windows.Forms.Button( );
            this.defineRegionsControl = new MotionDetectorSample.DefineRegionsControl( );
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator( );
            this.clearButton = new System.Windows.Forms.ToolStripButton( );
            this.toolStrip.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.rectangleButton,
            this.toolStripSeparator1,
            this.clearButton} );
            this.toolStrip.Location = new System.Drawing.Point( 0, 0 );
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size( 342, 25 );
            this.toolStrip.TabIndex = 1;
            // 
            // rectangleButton
            // 
            this.rectangleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rectangleButton.Image = ( (System.Drawing.Image) ( resources.GetObject( "rectangleButton.Image" ) ) );
            this.rectangleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rectangleButton.Name = "rectangleButton";
            this.rectangleButton.RightToLeftAutoMirrorImage = true;
            this.rectangleButton.Size = new System.Drawing.Size( 23, 22 );
            this.rectangleButton.ToolTipText = "Draw rectangular region";
            this.rectangleButton.Click += new System.EventHandler( this.rectangleButton_Click );
            // 
            // okButton
            // 
            this.okButton.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point( 174, 301 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 2;
            this.okButton.Text = "&Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 255, 300 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // defineRegionsControl
            // 
            this.defineRegionsControl.BackgroundImage = null;
            this.defineRegionsControl.DrawingMode = MotionDetectorSample.DrawingMode.None;
            this.defineRegionsControl.Location = new System.Drawing.Point( 10, 35 );
            this.defineRegionsControl.Name = "defineRegionsControl";
            this.defineRegionsControl.Size = new System.Drawing.Size( 322, 242 );
            this.defineRegionsControl.TabIndex = 0;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size( 6, 25 );
            // 
            // clearButton
            // 
            this.clearButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearButton.Image = ( (System.Drawing.Image) ( resources.GetObject( "clearButton.Image" ) ) );
            this.clearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size( 23, 22 );
            this.clearButton.ToolTipText = "Clear all regions";
            this.clearButton.Click += new System.EventHandler( this.clearButton_Click );
            // 
            // MotionRegionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 342, 336 );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.Controls.Add( this.toolStrip );
            this.Controls.Add( this.defineRegionsControl );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MotionRegionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Motion Regions";
            this.toolStrip.ResumeLayout( false );
            this.toolStrip.PerformLayout( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private DefineRegionsControl defineRegionsControl;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolStripButton rectangleButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton clearButton;
    }
}