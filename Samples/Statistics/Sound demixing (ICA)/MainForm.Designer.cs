namespace Demixing.ICA
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRunAnalysis = new System.Windows.Forms.Button();
            this.btnSource1 = new System.Windows.Forms.Button();
            this.btnMic2 = new System.Windows.Forms.Button();
            this.btnMic1 = new System.Windows.Forms.Button();
            this.btnSource2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(303, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 88);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please click to play the mixed sources captured by two distinct microphones.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(377, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 39);
            this.label2.TabIndex = 1;
            this.label2.Text = "Then click the button below to separate the sources.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 428);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(391, 28);
            this.label3.TabIndex = 1;
            this.label3.Text = "After the processing is done, click the buttons above to play the resulting signa" +
    "ls.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRunAnalysis
            // 
            this.btnRunAnalysis.Image = global::Demixing.ICA.Properties.Resources.gears;
            this.btnRunAnalysis.Location = new System.Drawing.Point(380, 153);
            this.btnRunAnalysis.Name = "btnRunAnalysis";
            this.btnRunAnalysis.Size = new System.Drawing.Size(161, 170);
            this.btnRunAnalysis.TabIndex = 0;
            this.btnRunAnalysis.Text = "Run Analysis";
            this.btnRunAnalysis.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRunAnalysis.UseVisualStyleBackColor = true;
            this.btnRunAnalysis.Click += new System.EventHandler(this.btnRunAnalysis_Click);
            // 
            // btnSource1
            // 
            this.btnSource1.Enabled = false;
            this.btnSource1.Image = global::Demixing.ICA.Properties.Resources.sound;
            this.btnSource1.Location = new System.Drawing.Point(54, 261);
            this.btnSource1.Name = "btnSource1";
            this.btnSource1.Size = new System.Drawing.Size(139, 166);
            this.btnSource1.TabIndex = 0;
            this.btnSource1.Text = "Play source 1";
            this.btnSource1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSource1.UseVisualStyleBackColor = true;
            this.btnSource1.Click += new System.EventHandler(this.btnPlaySource1_Click);
            // 
            // btnMic2
            // 
            this.btnMic2.Image = global::Demixing.ICA.Properties.Resources.mic;
            this.btnMic2.Location = new System.Drawing.Point(158, 12);
            this.btnMic2.Name = "btnMic2";
            this.btnMic2.Size = new System.Drawing.Size(139, 163);
            this.btnMic2.TabIndex = 0;
            this.btnMic2.Text = "Play microphone 2";
            this.btnMic2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnMic2.UseVisualStyleBackColor = true;
            this.btnMic2.Click += new System.EventHandler(this.btnPlayMic2_Click);
            // 
            // btnMic1
            // 
            this.btnMic1.Image = global::Demixing.ICA.Properties.Resources.mic;
            this.btnMic1.Location = new System.Drawing.Point(13, 12);
            this.btnMic1.Name = "btnMic1";
            this.btnMic1.Size = new System.Drawing.Size(139, 163);
            this.btnMic1.TabIndex = 0;
            this.btnMic1.Text = "Play microphone 1";
            this.btnMic1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnMic1.UseVisualStyleBackColor = true;
            this.btnMic1.Click += new System.EventHandler(this.btnPlayMic1_Click);
            // 
            // btnSource2
            // 
            this.btnSource2.Enabled = false;
            this.btnSource2.Image = global::Demixing.ICA.Properties.Resources.sound;
            this.btnSource2.Location = new System.Drawing.Point(199, 261);
            this.btnSource2.Name = "btnSource2";
            this.btnSource2.Size = new System.Drawing.Size(139, 166);
            this.btnSource2.TabIndex = 0;
            this.btnSource2.Text = "Play source 2";
            this.btnSource2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSource2.UseVisualStyleBackColor = true;
            this.btnSource2.Click += new System.EventHandler(this.btnPlaySource2_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 463);
            this.Controls.Add(this.btnRunAnalysis);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSource2);
            this.Controls.Add(this.btnSource1);
            this.Controls.Add(this.btnMic2);
            this.Controls.Add(this.btnMic1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Independent Component Analysis";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMic1;
        private System.Windows.Forms.Button btnRunAnalysis;
        private System.Windows.Forms.Button btnSource1;
        private System.Windows.Forms.Button btnMic2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSource2;
    }
}

