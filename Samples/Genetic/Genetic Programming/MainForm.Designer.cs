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
            this.btnApproximation = new System.Windows.Forms.Button();
            this.btnTimeSeries = new System.Windows.Forms.Button();
            this.btnXOR = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnApproximation
            // 
            this.btnApproximation.Location = new System.Drawing.Point(18, 18);
            this.btnApproximation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnApproximation.Name = "btnApproximation";
            this.btnApproximation.Size = new System.Drawing.Size(240, 120);
            this.btnApproximation.TabIndex = 0;
            this.btnApproximation.Text = "Approximation";
            this.btnApproximation.UseVisualStyleBackColor = true;
            this.btnApproximation.Click += new System.EventHandler(this.btnApproximation_Click);
            // 
            // btnTimeSeries
            // 
            this.btnTimeSeries.Location = new System.Drawing.Point(267, 18);
            this.btnTimeSeries.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTimeSeries.Name = "btnTimeSeries";
            this.btnTimeSeries.Size = new System.Drawing.Size(240, 120);
            this.btnTimeSeries.TabIndex = 0;
            this.btnTimeSeries.Text = "Time-Series";
            this.btnTimeSeries.UseVisualStyleBackColor = true;
            this.btnTimeSeries.Click += new System.EventHandler(this.btnTimeSeries_Click);
            // 
            // btnXOR
            // 
            this.btnXOR.Location = new System.Drawing.Point(516, 18);
            this.btnXOR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnXOR.Name = "btnXOR";
            this.btnXOR.Size = new System.Drawing.Size(240, 120);
            this.btnXOR.TabIndex = 0;
            this.btnXOR.Text = "Optimization";
            this.btnXOR.UseVisualStyleBackColor = true;
            this.btnXOR.Click += new System.EventHandler(this.btnOptimization_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(776, 154);
            this.Controls.Add(this.btnXOR);
            this.Controls.Add(this.btnTimeSeries);
            this.Controls.Add(this.btnApproximation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Genetic Programming sample applications";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApproximation;
        private System.Windows.Forms.Button btnTimeSeries;
        private System.Windows.Forms.Button btnXOR;
    }
}