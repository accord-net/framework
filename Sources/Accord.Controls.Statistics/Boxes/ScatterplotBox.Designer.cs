namespace Accord.Controls
{
    partial class ScatterplotBox
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
            this.scatterplotView1 = new Accord.Controls.ScatterplotView();
            this.SuspendLayout();
            // 
            // scatterplotView1
            // 
            this.scatterplotView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scatterplotView1.Location = new System.Drawing.Point(0, 0);
            this.scatterplotView1.Name = "scatterplotView1";
            this.scatterplotView1.Scatterplot = ((Accord.Statistics.Visualizations.Scatterplot)(resources.GetObject("scatterplotView1.Scatterplot")));
            this.scatterplotView1.Size = new System.Drawing.Size(484, 362);
            this.scatterplotView1.TabIndex = 0;
            // 
            // ScatterplotBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 362);
            this.Controls.Add(this.scatterplotView1);
            this.Name = "ScatterplotBox";
            this.Text = "Scatter Plot";
            this.ResumeLayout(false);

        }

        #endregion

        private ScatterplotView scatterplotView1;


    }
}