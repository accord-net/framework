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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScatterplotBox));
            this.histogramView1 = new Accord.Controls.HistogramView();
            this.SuspendLayout();
            // 
            // scatterplotView1
            // 
            this.histogramView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.histogramView1.Location = new System.Drawing.Point(0, 0);
            this.histogramView1.Name = "histogramView1";
            this.histogramView1.Histogram = ((Accord.Statistics.Visualizations.Histogram)(resources.GetObject("histogramtView1.Scatterplot")));
            this.histogramView1.Size = new System.Drawing.Size(484, 362);
            this.histogramView1.TabIndex = 0;
            // 
            // ScatterplotBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 362);
            this.Controls.Add(this.histogramView1);
            this.Name = "HistogramView";
            this.Text = "Histogram";
            this.ResumeLayout(false);

        }

        #endregion

        private HistogramView histogramView1;

    }
}