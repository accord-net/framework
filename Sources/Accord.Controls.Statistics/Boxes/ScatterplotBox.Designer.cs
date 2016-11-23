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
            this.scatterplotView1 = new Accord.Controls.ScatterplotView();
            this.SuspendLayout();
            // 
            // scatterplotView1
            // 
            this.scatterplotView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scatterplotView1.LinesVisible = false;
            this.scatterplotView1.Location = new System.Drawing.Point(0, 0);
            this.scatterplotView1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.scatterplotView1.Name = "scatterplotView1";
            this.scatterplotView1.ScaleTight = false;
            this.scatterplotView1.Size = new System.Drawing.Size(726, 557);
            this.scatterplotView1.SymbolSize = 7F;
            this.scatterplotView1.TabIndex = 0;
            // 
            // ScatterplotBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 557);
            this.Controls.Add(this.scatterplotView1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ScatterplotBox";
            this.Text = "Scatter Plot";
            this.ResumeLayout(false);

        }

        #endregion

        private ScatterplotView scatterplotView1;



    }
}