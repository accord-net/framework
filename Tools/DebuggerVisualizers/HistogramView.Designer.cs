namespace AForge.DebuggerVisualizers
{
    partial class HistogramView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( HistogramView ) );
            this.mainPanel = new System.Windows.Forms.Panel( );
            this.histogramControl = new AForge.Controls.Histogram( );
            this.textBox = new System.Windows.Forms.TextBox( );
            this.statsBox = new System.Windows.Forms.TextBox( );
            this.logCheck = new System.Windows.Forms.CheckBox( );
            this.mainPanel.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mainPanel.Controls.Add( this.histogramControl );
            this.mainPanel.Location = new System.Drawing.Point( 10, 10 );
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size( 474, 159 );
            this.mainPanel.TabIndex = 0;
            // 
            // histogramControl
            // 
            this.histogramControl.AllowSelection = true;
            this.histogramControl.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.histogramControl.Color = System.Drawing.Color.DodgerBlue;
            this.histogramControl.Location = new System.Drawing.Point( 154, 33 );
            this.histogramControl.Name = "histogramControl";
            this.histogramControl.Size = new System.Drawing.Size( 196, 73 );
            this.histogramControl.TabIndex = 0;
            this.histogramControl.Values = null;
            this.histogramControl.SelectionChanged += new AForge.Controls.HistogramEventHandler( this.histogramControl_SelectionChanged );
            this.histogramControl.PositionChanged += new AForge.Controls.HistogramEventHandler( this.histogramControl_PositionChanged );
            // 
            // textBox
            // 
            this.textBox.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.textBox.Location = new System.Drawing.Point( 10, 203 );
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size( 424, 20 );
            this.textBox.TabIndex = 2;
            // 
            // statsBox
            // 
            this.statsBox.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.statsBox.Location = new System.Drawing.Point( 10, 175 );
            this.statsBox.Name = "statsBox";
            this.statsBox.ReadOnly = true;
            this.statsBox.Size = new System.Drawing.Size( 474, 20 );
            this.statsBox.TabIndex = 1;
            // 
            // logCheck
            // 
            this.logCheck.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.logCheck.AutoSize = true;
            this.logCheck.Location = new System.Drawing.Point( 440, 206 );
            this.logCheck.Name = "logCheck";
            this.logCheck.Size = new System.Drawing.Size( 44, 17 );
            this.logCheck.TabIndex = 3;
            this.logCheck.Text = "&Log";
            this.logCheck.UseVisualStyleBackColor = true;
            this.logCheck.CheckedChanged += new System.EventHandler( this.logCheck_CheckedChanged );
            // 
            // HistogramView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size( 494, 231 );
            this.Controls.Add( this.logCheck );
            this.Controls.Add( this.statsBox );
            this.Controls.Add( this.textBox );
            this.Controls.Add( this.mainPanel );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "$this.Icon" ) ) );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size( 400, 250 );
            this.Name = "HistogramView";
            this.ShowInTaskbar = false;
            this.Text = "Histogram";
            this.mainPanel.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private AForge.Controls.Histogram histogramControl;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.TextBox statsBox;
        private System.Windows.Forms.CheckBox logCheck;
    }
}