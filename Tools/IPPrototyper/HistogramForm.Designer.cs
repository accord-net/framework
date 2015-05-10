namespace IPPrototyper
{
    partial class HistogramForm
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
            this.histogram = new AForge.Controls.Histogram( );
            this.label1 = new System.Windows.Forms.Label( );
            this.channelCombo = new System.Windows.Forms.ComboBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label3 = new System.Windows.Forms.Label( );
            this.label4 = new System.Windows.Forms.Label( );
            this.label5 = new System.Windows.Forms.Label( );
            this.label6 = new System.Windows.Forms.Label( );
            this.meanLabel = new System.Windows.Forms.Label( );
            this.stdDevLabel = new System.Windows.Forms.Label( );
            this.medianLabel = new System.Windows.Forms.Label( );
            this.minLabel = new System.Windows.Forms.Label( );
            this.maxLabel = new System.Windows.Forms.Label( );
            this.label7 = new System.Windows.Forms.Label( );
            this.label8 = new System.Windows.Forms.Label( );
            this.label9 = new System.Windows.Forms.Label( );
            this.levelLabel = new System.Windows.Forms.Label( );
            this.countLabel = new System.Windows.Forms.Label( );
            this.percentileLabel = new System.Windows.Forms.Label( );
            this.SuspendLayout( );
            // 
            // histogram
            // 
            this.histogram.AllowSelection = true;
            this.histogram.BackColor = System.Drawing.SystemColors.ControlDark;
            this.histogram.Location = new System.Drawing.Point( 11, 37 );
            this.histogram.Name = "histogram";
            this.histogram.Size = new System.Drawing.Size( 258, 150 );
            this.histogram.TabIndex = 0;
            this.histogram.Values = null;
            this.histogram.SelectionChanged += new AForge.Controls.HistogramEventHandler( this.histogram_SelectionChanged );
            this.histogram.PositionChanged += new AForge.Controls.HistogramEventHandler( this.histogram_PositionChanged );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 12 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 49, 13 );
            this.label1.TabIndex = 1;
            this.label1.Text = "Channel:";
            // 
            // channelCombo
            // 
            this.channelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channelCombo.Enabled = false;
            this.channelCombo.FormattingEnabled = true;
            this.channelCombo.Items.AddRange( new object[] {
            "Red",
            "Green",
            "Blue"} );
            this.channelCombo.Location = new System.Drawing.Point( 65, 10 );
            this.channelCombo.Name = "channelCombo";
            this.channelCombo.Size = new System.Drawing.Size( 100, 21 );
            this.channelCombo.TabIndex = 2;
            this.channelCombo.SelectedIndexChanged += new System.EventHandler( this.channelCombo_SelectedIndexChanged );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 200 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 37, 13 );
            this.label2.TabIndex = 3;
            this.label2.Text = "Mean:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 10, 220 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 49, 13 );
            this.label3.TabIndex = 4;
            this.label3.Text = "Std Dev:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 10, 240 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 45, 13 );
            this.label4.TabIndex = 5;
            this.label4.Text = "Median:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point( 10, 260 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 27, 13 );
            this.label5.TabIndex = 6;
            this.label5.Text = "Min:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point( 10, 280 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 30, 13 );
            this.label6.TabIndex = 7;
            this.label6.Text = "Max:";
            // 
            // meanLabel
            // 
            this.meanLabel.Location = new System.Drawing.Point( 65, 200 );
            this.meanLabel.Name = "meanLabel";
            this.meanLabel.Size = new System.Drawing.Size( 66, 20 );
            this.meanLabel.TabIndex = 8;
            // 
            // stdDevLabel
            // 
            this.stdDevLabel.Location = new System.Drawing.Point( 65, 220 );
            this.stdDevLabel.Name = "stdDevLabel";
            this.stdDevLabel.Size = new System.Drawing.Size( 55, 13 );
            this.stdDevLabel.TabIndex = 9;
            // 
            // medianLabel
            // 
            this.medianLabel.Location = new System.Drawing.Point( 65, 240 );
            this.medianLabel.Name = "medianLabel";
            this.medianLabel.Size = new System.Drawing.Size( 55, 13 );
            this.medianLabel.TabIndex = 10;
            // 
            // minLabel
            // 
            this.minLabel.Location = new System.Drawing.Point( 65, 260 );
            this.minLabel.Name = "minLabel";
            this.minLabel.Size = new System.Drawing.Size( 55, 13 );
            this.minLabel.TabIndex = 11;
            // 
            // maxLabel
            // 
            this.maxLabel.Location = new System.Drawing.Point( 65, 280 );
            this.maxLabel.Name = "maxLabel";
            this.maxLabel.Size = new System.Drawing.Size( 55, 13 );
            this.maxLabel.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point( 160, 200 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 36, 13 );
            this.label7.TabIndex = 13;
            this.label7.Text = "Level:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point( 160, 220 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 38, 13 );
            this.label8.TabIndex = 14;
            this.label8.Text = "Count:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point( 160, 240 );
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size( 57, 13 );
            this.label9.TabIndex = 15;
            this.label9.Text = "Percentile:";
            // 
            // levelLabel
            // 
            this.levelLabel.Location = new System.Drawing.Point( 220, 200 );
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size( 49, 13 );
            this.levelLabel.TabIndex = 16;
            // 
            // countLabel
            // 
            this.countLabel.Location = new System.Drawing.Point( 220, 220 );
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size( 49, 13 );
            this.countLabel.TabIndex = 17;
            // 
            // percentileLabel
            // 
            this.percentileLabel.Location = new System.Drawing.Point( 220, 240 );
            this.percentileLabel.Name = "percentileLabel";
            this.percentileLabel.Size = new System.Drawing.Size( 49, 13 );
            this.percentileLabel.TabIndex = 18;
            // 
            // HistogramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 281, 306 );
            this.Controls.Add( this.percentileLabel );
            this.Controls.Add( this.countLabel );
            this.Controls.Add( this.levelLabel );
            this.Controls.Add( this.label9 );
            this.Controls.Add( this.label8 );
            this.Controls.Add( this.label7 );
            this.Controls.Add( this.maxLabel );
            this.Controls.Add( this.minLabel );
            this.Controls.Add( this.medianLabel );
            this.Controls.Add( this.stdDevLabel );
            this.Controls.Add( this.meanLabel );
            this.Controls.Add( this.label6 );
            this.Controls.Add( this.label5 );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.channelCombo );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.histogram );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistogramForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Histogram";
            this.TopMost = true;
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private AForge.Controls.Histogram histogram;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox channelCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label meanLabel;
        private System.Windows.Forms.Label stdDevLabel;
        private System.Windows.Forms.Label medianLabel;
        private System.Windows.Forms.Label minLabel;
        private System.Windows.Forms.Label maxLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label levelLabel;
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.Label percentileLabel;
    }
}