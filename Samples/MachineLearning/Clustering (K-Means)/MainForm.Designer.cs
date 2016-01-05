namespace SampleApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.numClusters = new System.Windows.Forms.NumericUpDown();
            this.numBandwidth = new System.Windows.Forms.NumericUpDown();
            this.radioClusters = new System.Windows.Forms.RadioButton();
            this.radioRadius = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClusters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBandwidth)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Image = global::SampleApp.Properties.Resources.leaf;
            this.pictureBox.Location = new System.Drawing.Point(18, 18);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(750, 500);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(650, 526);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 34);
            this.button1.TabIndex = 1;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(18, 526);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 34);
            this.button2.TabIndex = 2;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // numClusters
            // 
            this.numClusters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numClusters.Location = new System.Drawing.Point(264, 531);
            this.numClusters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numClusters.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numClusters.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numClusters.Name = "numClusters";
            this.numClusters.Size = new System.Drawing.Size(75, 26);
            this.numClusters.TabIndex = 3;
            this.numClusters.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numBandwidth
            // 
            this.numBandwidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numBandwidth.DecimalPlaces = 2;
            this.numBandwidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numBandwidth.Location = new System.Drawing.Point(528, 531);
            this.numBandwidth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numBandwidth.Name = "numBandwidth";
            this.numBandwidth.Size = new System.Drawing.Size(75, 26);
            this.numBandwidth.TabIndex = 3;
            this.numBandwidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            131072});
            // 
            // radioClusters
            // 
            this.radioClusters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioClusters.AutoSize = true;
            this.radioClusters.Checked = true;
            this.radioClusters.Location = new System.Drawing.Point(92, 533);
            this.radioClusters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioClusters.Name = "radioClusters";
            this.radioClusters.Size = new System.Drawing.Size(160, 24);
            this.radioClusters.TabIndex = 5;
            this.radioClusters.TabStop = true;
            this.radioClusters.Text = "K-Means clusters:";
            this.radioClusters.UseVisualStyleBackColor = true;
            // 
            // radioRadius
            // 
            this.radioRadius.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioRadius.AutoSize = true;
            this.radioRadius.Location = new System.Drawing.Point(354, 533);
            this.radioRadius.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioRadius.Name = "radioRadius";
            this.radioRadius.Size = new System.Drawing.Size(163, 24);
            this.radioRadius.TabIndex = 5;
            this.radioRadius.TabStop = true;
            this.radioRadius.Text = "Mean-Shift radius:";
            this.radioRadius.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(780, 572);
            this.Controls.Add(this.radioRadius);
            this.Controls.Add(this.radioClusters);
            this.Controls.Add(this.numBandwidth);
            this.Controls.Add(this.numClusters);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "K-Means and Mean-Shift Color Clustering";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClusters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBandwidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown numClusters;
        private System.Windows.Forms.NumericUpDown numBandwidth;
        private System.Windows.Forms.RadioButton radioClusters;
        private System.Windows.Forms.RadioButton radioRadius;
    }
}

