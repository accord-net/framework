﻿namespace SampleApp
{
    partial class FaceForm
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
            SampleApp.Properties.Settings settings1 = new SampleApp.Properties.Settings();
            this.btnVerticalMax = new System.Windows.Forms.Button();
            this.btnVerticalMin = new System.Windows.Forms.Button();
            this.btnHorizontalMax = new System.Windows.Forms.Button();
            this.btnHorizontalMin = new System.Windows.Forms.Button();
            this.tbVertical = new System.Windows.Forms.TrackBar();
            this.tbHorizontal = new System.Windows.Forms.TrackBar();
            this.faceController = new Accord.Controls.Vision.FaceController();
            this.pointBox1 = new Accord.Controls.PointBox();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tbVertical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHorizontal)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVerticalMax
            // 
            this.btnVerticalMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerticalMax.Location = new System.Drawing.Point(341, 14);
            this.btnVerticalMax.Name = "btnVerticalMax";
            this.btnVerticalMax.Size = new System.Drawing.Size(45, 33);
            this.btnVerticalMax.TabIndex = 13;
            this.btnVerticalMax.Text = "Set\r\nTop";
            this.btnVerticalMax.UseVisualStyleBackColor = true;
            this.btnVerticalMax.Click += new System.EventHandler(this.btnVerticalMax_Click);
            // 
            // btnVerticalMin
            // 
            this.btnVerticalMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerticalMin.Location = new System.Drawing.Point(341, 271);
            this.btnVerticalMin.Name = "btnVerticalMin";
            this.btnVerticalMin.Size = new System.Drawing.Size(45, 33);
            this.btnVerticalMin.TabIndex = 12;
            this.btnVerticalMin.Text = "Set\r\nBottom";
            this.btnVerticalMin.UseVisualStyleBackColor = true;
            this.btnVerticalMin.Click += new System.EventHandler(this.btnVerticalMin_Click);
            // 
            // btnHorizontalMax
            // 
            this.btnHorizontalMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHorizontalMax.Location = new System.Drawing.Point(341, 310);
            this.btnHorizontalMax.Name = "btnHorizontalMax";
            this.btnHorizontalMax.Size = new System.Drawing.Size(45, 45);
            this.btnHorizontalMax.TabIndex = 11;
            this.btnHorizontalMax.Text = "Set\r\nRight";
            this.btnHorizontalMax.UseVisualStyleBackColor = true;
            this.btnHorizontalMax.Click += new System.EventHandler(this.btnHorizontalMax_Click);
            // 
            // btnHorizontalMin
            // 
            this.btnHorizontalMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHorizontalMin.Location = new System.Drawing.Point(12, 310);
            this.btnHorizontalMin.Name = "btnHorizontalMin";
            this.btnHorizontalMin.Size = new System.Drawing.Size(45, 45);
            this.btnHorizontalMin.TabIndex = 7;
            this.btnHorizontalMin.Text = "Set\r\nLeft";
            this.btnHorizontalMin.UseVisualStyleBackColor = true;
            this.btnHorizontalMin.Click += new System.EventHandler(this.btnHorizontalMin_Click);
            // 
            // tbVertical
            // 
            this.tbVertical.Location = new System.Drawing.Point(341, 43);
            this.tbVertical.Maximum = 20;
            this.tbVertical.Minimum = -20;
            this.tbVertical.Name = "tbVertical";
            this.tbVertical.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbVertical.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbVertical.Size = new System.Drawing.Size(45, 232);
            this.tbVertical.TabIndex = 4;
            // 
            // tbHorizontal
            // 
            this.tbHorizontal.Location = new System.Drawing.Point(63, 310);
            this.tbHorizontal.Maximum = 20;
            this.tbHorizontal.Minimum = -20;
            this.tbHorizontal.Name = "tbHorizontal";
            this.tbHorizontal.Size = new System.Drawing.Size(272, 45);
            this.tbHorizontal.TabIndex = 5;
            // 
            // faceController
            // 
            settings1.angleMax = 1.3290820121765137D;
            settings1.angleMin = 1.83104407787323D;
            settings1.face_xmax = -0.65D;
            settings1.face_xmin = 0.65D;
            settings1.face_ymax = -0.1D;
            settings1.face_ymin = 0.35D;
            settings1.scaleMax = 140;
            settings1.scaleMin = 90;
            settings1.SettingsKey = "";
            settings1.xMax = 128;
            settings1.xMin = 180;
            settings1.yMax = 100;
            settings1.yMin = 150;
            this.faceController.DataBindings.Add(new System.Windows.Forms.Binding("XAxisMax", settings1, "face_xmax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.faceController.DataBindings.Add(new System.Windows.Forms.Binding("XAxisMin", settings1, "face_xmin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.faceController.DataBindings.Add(new System.Windows.Forms.Binding("YAxisMax", settings1, "face_ymax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.faceController.DataBindings.Add(new System.Windows.Forms.Binding("YAxisMin", settings1, "face_ymin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.faceController.SynchronizingObject = this;
            this.faceController.XAxisMax = settings1.face_xmax;
            this.faceController.XAxisMin = settings1.face_xmin;
            this.faceController.YAxisMax = settings1.face_ymax;
            this.faceController.YAxisMin = settings1.face_ymin;
            this.faceController.FaceMove += new System.EventHandler<Accord.Controls.Vision.FaceEventArgs>(this.controller_FaceMove);
            // 
            // pointBox1
            // 
            this.pointBox1.Location = new System.Drawing.Point(124, 80);
            this.pointBox1.Name = "pointBox1";
            this.pointBox1.PointX = 0F;
            this.pointBox1.PointY = 0F;
            this.pointBox1.Size = new System.Drawing.Size(150, 150);
            this.pointBox1.TabIndex = 14;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(153, 18);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(399, 369);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pointBox1);
            this.Controls.Add(this.btnVerticalMax);
            this.Controls.Add(this.btnVerticalMin);
            this.Controls.Add(this.btnHorizontalMax);
            this.Controls.Add(this.btnHorizontalMin);
            this.Controls.Add(this.tbVertical);
            this.Controls.Add(this.tbHorizontal);
            this.Name = "FaceForm";
            this.Text = "Face-based Controller";
            ((System.ComponentModel.ISupportInitialize)(this.tbVertical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHorizontal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVerticalMax;
        private System.Windows.Forms.Button btnVerticalMin;
        private System.Windows.Forms.Button btnHorizontalMax;
        private System.Windows.Forms.Button btnHorizontalMin;
        private System.Windows.Forms.TrackBar tbVertical;
        private System.Windows.Forms.TrackBar tbHorizontal;
        private Accord.Controls.PointBox pointBox1;
        private System.Windows.Forms.Button button2;
        public Accord.Controls.Vision.FaceController faceController;
    }
}