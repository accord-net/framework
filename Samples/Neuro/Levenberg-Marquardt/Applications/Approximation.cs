// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2017, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// Based on code from the:
//   Neuro Approximation sample application
//   AForge.NET Framework
//   http://www.aforgenet.com/framework/
//

using Accord;
using Accord.IO;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using AForge;
using Accord.Controls;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SampleApp
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Approximation : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView dataList;
        private System.Windows.Forms.Button loadDataButton;
        private System.Windows.Forms.ColumnHeader xColumnHeader;
        private System.Windows.Forms.ColumnHeader yColumnHeader;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox2;
        private Accord.Controls.Chart chart;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox alphaBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox learningRateBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox iterationsBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox currentErrorBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox currentIterationBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox neuronsBox;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private double[,] data = null;

        private double learningRate = 100;
        private double sigmoidAlphaValue = 2.0;
        private int neuronsInFirstLayer = 10;
        private int iterations = 500;
        private bool useRegularization;
        private bool useNguyenWidrow;

        private Thread workerThread = null;
        private CheckBox cbRegularization;
        private CheckBox cbNguyenWidrow;
        private volatile bool needToStop = false;

        DoubleRange xRange;
        DoubleRange yRange;

        // Constructor
        public Approximation()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // init chart control
            chart.AddDataSeries("data", Color.Red, Chart.SeriesType.Dots, 5);
            chart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.Line, 1);

            // init controls
            UpdateSettings();

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Sample data (approximation)");
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Approximation));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataList = new System.Windows.Forms.ListView();
            this.xColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.yColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadDataButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chart = new Accord.Controls.Chart();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbNguyenWidrow = new System.Windows.Forms.CheckBox();
            this.cbRegularization = new System.Windows.Forms.CheckBox();
            this.neuronsBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.alphaBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.learningRateBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.currentErrorBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataList);
            this.groupBox1.Controls.Add(this.loadDataButton);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 467);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // dataList
            // 
            this.dataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.xColumnHeader,
            this.yColumnHeader});
            this.dataList.FullRowSelect = true;
            this.dataList.GridLines = true;
            this.dataList.Location = new System.Drawing.Point(16, 29);
            this.dataList.Name = "dataList";
            this.dataList.Size = new System.Drawing.Size(256, 373);
            this.dataList.TabIndex = 0;
            this.dataList.UseCompatibleStateImageBehavior = false;
            this.dataList.View = System.Windows.Forms.View.Details;
            // 
            // xColumnHeader
            // 
            this.xColumnHeader.Text = "X";
            // 
            // yColumnHeader
            // 
            this.yColumnHeader.Text = "Y";
            // 
            // loadDataButton
            // 
            this.loadDataButton.Location = new System.Drawing.Point(16, 417);
            this.loadDataButton.Name = "loadDataButton";
            this.loadDataButton.Size = new System.Drawing.Size(120, 33);
            this.loadDataButton.TabIndex = 1;
            this.loadDataButton.Text = "&Load";
            this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            this.openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chart);
            this.groupBox2.Location = new System.Drawing.Point(320, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 467);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Function";
            // 
            // chart
            // 
            this.chart.Location = new System.Drawing.Point(16, 29);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(448, 424);
            this.chart.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbNguyenWidrow);
            this.groupBox3.Controls.Add(this.cbRegularization);
            this.groupBox3.Controls.Add(this.neuronsBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.alphaBox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.learningRateBox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.iterationsBox);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(816, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(312, 285);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // cbNguyenWidrow
            // 
            this.cbNguyenWidrow.AutoSize = true;
            this.cbNguyenWidrow.Location = new System.Drawing.Point(21, 175);
            this.cbNguyenWidrow.Name = "cbNguyenWidrow";
            this.cbNguyenWidrow.Size = new System.Drawing.Size(241, 24);
            this.cbNguyenWidrow.TabIndex = 26;
            this.cbNguyenWidrow.Text = "Use Nguyen-Widrow Weights";
            this.cbNguyenWidrow.UseVisualStyleBackColor = true;
            // 
            // cbRegularization
            // 
            this.cbRegularization.AutoSize = true;
            this.cbRegularization.Location = new System.Drawing.Point(21, 142);
            this.cbRegularization.Name = "cbRegularization";
            this.cbRegularization.Size = new System.Drawing.Size(239, 24);
            this.cbRegularization.TabIndex = 26;
            this.cbRegularization.Text = "Use Bayesian Regularization";
            this.cbRegularization.UseVisualStyleBackColor = true;
            // 
            // neuronsBox
            // 
            this.neuronsBox.Location = new System.Drawing.Point(198, 104);
            this.neuronsBox.Name = "neuronsBox";
            this.neuronsBox.Size = new System.Drawing.Size(96, 26);
            this.neuronsBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(14, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(184, 22);
            this.label4.TabIndex = 6;
            this.label4.Text = "Neurons in first layer:";
            // 
            // alphaBox
            // 
            this.alphaBox.Location = new System.Drawing.Point(198, 67);
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size(96, 26);
            this.alphaBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(14, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 22);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sigmoid\'s alpha value:";
            // 
            // learningRateBox
            // 
            this.learningRateBox.Location = new System.Drawing.Point(200, 29);
            this.learningRateBox.Name = "learningRateBox";
            this.learningRateBox.Size = new System.Drawing.Size(96, 26);
            this.learningRateBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Learning rate:";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(16, 215);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(280, 3);
            this.label8.TabIndex = 22;
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(200, 227);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(96, 26);
            this.iterationsBox.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(202, 256);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 20);
            this.label10.TabIndex = 25;
            this.label10.Text = "( 0 - inifinity )";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 229);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(112, 24);
            this.label9.TabIndex = 8;
            this.label9.Text = "Iterations:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.currentErrorBox);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.currentIterationBox);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(816, 307);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(312, 110);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Current iteration";
            // 
            // currentErrorBox
            // 
            this.currentErrorBox.Location = new System.Drawing.Point(200, 66);
            this.currentErrorBox.Name = "currentErrorBox";
            this.currentErrorBox.ReadOnly = true;
            this.currentErrorBox.Size = new System.Drawing.Size(96, 26);
            this.currentErrorBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Error:";
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(200, 29);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(96, 26);
            this.currentIterationBox.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 24);
            this.label5.TabIndex = 0;
            this.label5.Text = "Iteration:";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(1008, 446);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(120, 33);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(864, 446);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(120, 33);
            this.startButton.TabIndex = 7;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // Approximation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1146, 500);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Approximation";
            this.Text = "Approximation using Multi-Layer Neural Network (Levenberg-Marquardt)";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion



        // Thread safe updating of control's text property
        private void SetText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                Invoke((Action)(() => SetText(control, text)));
            }
            else
            {
                control.Text = text;
            }
        }

        // On main form closing
        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // check if worker thread is running
            if ((workerThread != null) && (workerThread.IsAlive))
            {
                needToStop = true;
                while (!workerThread.Join(100))
                    Application.DoEvents();
            }
        }

        // Update settings controls
        private void UpdateSettings()
        {
            learningRateBox.Text = learningRate.ToString();
            alphaBox.Text = sigmoidAlphaValue.ToString();
            neuronsBox.Text = neuronsInFirstLayer.ToString();
            iterationsBox.Text = iterations.ToString();
        }

        // Load data
        private void loadDataButton_Click(object sender, System.EventArgs e)
        {
            // show file selection dialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                data = null;

                try
                {
                    // open selected file
                    using (TextReader stream = new StreamReader(openFileDialog.FileName))
                    using (CsvReader reader = new CsvReader(stream, false))
                    {
                        data = reader.ToTable().ToMatrix(CultureInfo.InvariantCulture);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DoubleRange[] ranges = data.GetRange(dimension: 0);

                xRange = ranges[0];
                yRange = ranges[1];

                // update list and chart
                UpdateDataListView();
                chart.RangeX = new Range((float)xRange.Min, (float)xRange.Max);
                chart.UpdateDataSeries("data", data);
                chart.UpdateDataSeries("solution", null);

                // enable "Start" button
                startButton.Enabled = true;
            }
        }

        // Update data in list view
        private void UpdateDataListView()
        {
            // remove all current records
            dataList.Items.Clear();

            // add new records
            for (int i = 0; i < data.GetLength(0); i++)
            {
                dataList.Items.Add(data[i, 0].ToString());
                dataList.Items[i].SubItems.Add(data[i, 1].ToString());
            }
        }


        // Enable/disable controls (safe for threading)
        private void EnableControls(bool enable)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => EnableControls(enable)));
            }
            else
            {
                loadDataButton.Enabled = enable;
                learningRateBox.Enabled = enable;
                alphaBox.Enabled = enable;
                neuronsBox.Enabled = enable;
                iterationsBox.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // On button "Start"
        private void startButton_Click(object sender, System.EventArgs e)
        {
            // get learning rate
            try { learningRate = double.Parse(learningRateBox.Text); }
            catch { learningRate = 0.1; }

            // get sigmoid's alpha value
            try { sigmoidAlphaValue = Math.Max(0.001, Math.Min(50, double.Parse(alphaBox.Text))); }
            catch { sigmoidAlphaValue = 2; }

            // get neurons count in first layer
            try { neuronsInFirstLayer = Math.Max(5, Math.Min(50, int.Parse(neuronsBox.Text))); }
            catch { neuronsInFirstLayer = 20; }

            // iterations
            try { iterations = Math.Max(0, int.Parse(iterationsBox.Text)); }
            catch { iterations = 100; }

            useRegularization = cbRegularization.Checked;
            useNguyenWidrow = cbNguyenWidrow.Checked;

            // update settings controls
            UpdateSettings();

            // disable all settings controls except "Stop" button
            EnableControls(false);

            // run worker thread
            needToStop = false;
            workerThread = new Thread(new ThreadStart(SearchSolution));
            workerThread.Start();
        }

        // On button "Stop"
        private void stopButton_Click(object sender, System.EventArgs e)
        {
            // stop worker thread
            needToStop = true;
            while (!workerThread.Join(100))
                Application.DoEvents();
            workerThread = null;
        }

        // Worker thread
        void SearchSolution()
        {
            // number of learning samples
            int samples = data.GetLength(0);

            // prepare learning data
            DoubleRange unit = new DoubleRange(-1, 1);
            double[][] input = data.GetColumn(0).Scale(fromRange: xRange, toRange: unit).ToJagged();
            double[][] output = data.GetColumn(1).Scale(fromRange: yRange, toRange: unit).ToJagged();


            // create multi-layer neural network
            ActivationNetwork network = new ActivationNetwork(
                new BipolarSigmoidFunction(sigmoidAlphaValue),
                1, neuronsInFirstLayer, 1);

            if (useNguyenWidrow)
            {
                new NguyenWidrow(network).Randomize();
            }

            // create teacher
            var teacher = new LevenbergMarquardtLearning(network, useRegularization);

            // set learning rate and momentum
            teacher.LearningRate = learningRate;

            // iterations
            int iteration = 1;

            // solution array
            double[,] solution = new double[samples, 2];


            // loop
            while (!needToStop)
            {
                // run epoch of learning procedure
                double error = teacher.RunEpoch(input, output) / samples;

                // calculate solution
                for (int j = 0; j < samples; j++)
                {
                    double x = input[j][0];
                    double y = network.Compute(new[] { x })[0];
                    solution[j, 0] = x.Scale(fromRange: unit, toRange: xRange);
                    solution[j, 1] = y.Scale(fromRange: unit, toRange: yRange);
                }

                chart.UpdateDataSeries("solution", solution);

                // calculate error
                double learningError = 0.0;
                for (int j = 0; j < samples; j++)
                {
                    double x = input[j][0];
                    double expected = data[j, 1];
                    double actual = network.Compute(new[] { x })[0];
                    learningError += Math.Abs(expected - actual);
                }

                // set current iteration's info
                SetText(currentIterationBox, iteration.ToString());
                SetText(currentErrorBox, learningError.ToString("F3"));

                // increase current iteration
                iteration++;

                // check if we need to stop
                if ((iterations != 0) && (iteration > iterations))
                    break;
            }


            // enable settings controls
            EnableControls(true);
        }
    }
}
