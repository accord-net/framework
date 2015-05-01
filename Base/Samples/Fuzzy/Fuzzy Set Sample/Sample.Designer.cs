namespace FuzzySetSample
{
    partial class Sample
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
            this.runFuzzySetTestButton = new System.Windows.Forms.Button();
            this.chart = new AForge.Controls.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.runLingVarTestButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // runFuzzySetTestButton
            // 
            this.runFuzzySetTestButton.Location = new System.Drawing.Point(10, 12);
            this.runFuzzySetTestButton.Name = "runFuzzySetTestButton";
            this.runFuzzySetTestButton.Size = new System.Drawing.Size(125, 23);
            this.runFuzzySetTestButton.TabIndex = 0;
            this.runFuzzySetTestButton.Text = "Run Fuzzy Set Test";
            this.runFuzzySetTestButton.UseVisualStyleBackColor = true;
            this.runFuzzySetTestButton.Click += new System.EventHandler(this.runFuzzySetTestButton_Click);
            // 
            // chart
            // 
            this.chart.Location = new System.Drawing.Point(10, 41);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(418, 250);
            this.chart.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(116, 299);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Cool";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.LightCoral;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(222, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Warm";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.CornflowerBlue;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "Cold";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Firebrick;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(328, 299);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "Hot";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // runLingVarTestButton
            // 
            this.runLingVarTestButton.Location = new System.Drawing.Point(141, 12);
            this.runLingVarTestButton.Name = "runLingVarTestButton";
            this.runLingVarTestButton.Size = new System.Drawing.Size(158, 23);
            this.runLingVarTestButton.TabIndex = 6;
            this.runLingVarTestButton.Text = "Run Linguistic Variable Test";
            this.runLingVarTestButton.UseVisualStyleBackColor = true;
            this.runLingVarTestButton.Click += new System.EventHandler(this.runLingVarTestButton_Click);
            // 
            // Sample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 332);
            this.Controls.Add(this.runLingVarTestButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.runFuzzySetTestButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Sample";
            this.Text = "Fuzzy Sets Sample";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button runFuzzySetTestButton;
        private AForge.Controls.Chart chart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button runLingVarTestButton;
    }
}

