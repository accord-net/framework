namespace Accord.Controls
{
    partial class HistogramBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistogramBox));
            this.histogramView1 = new Accord.Controls.HistogramView();
            this.SuspendLayout();
            // 
            // histogramView1
            // 
            this.histogramView1.BinWidth = null;
            this.histogramView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.histogramView1.Histogram = ((Accord.Statistics.Visualizations.Histogram)(resources.GetObject("histogramView1.Histogram")));
            this.histogramView1.Location = new System.Drawing.Point(0, 0);
            this.histogramView1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.histogramView1.Name = "histogramView1";
            this.histogramView1.NumberOfBins = null;
            this.histogramView1.Size = new System.Drawing.Size(726, 557);
            this.histogramView1.TabIndex = 0;
            // 
            // HistogramBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 557);
            this.Controls.Add(this.histogramView1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "HistogramBox";
            this.Text = "Histogram";
            this.ResumeLayout(false);

        }

        #endregion

        private HistogramView histogramView1;


    }
}