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

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Accord.Neuro.Learning;
using AForge;
using Accord.Controls;
using Accord.Neuro;
using Accord;

namespace SampleApp
{
    public class XorProblem : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox learningRateBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox alphaBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox errorLimitBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox sigmoidTypeCombo;
        private System.Windows.Forms.TextBox currentErrorBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox currentIterationBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox saveFilesCheck;
        private Chart errorChart;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private double initialStep = 0.0125;
        private double sigmoidAlphaValue = 2.0;
        private double learningErrorLimit = 0.1;
        private int sigmoidType = 0;
        private bool saveStatisticsToFiles = false;

        private Thread workerThread = null;
        private volatile bool needToStop = false;


        public XorProblem()
        {
            InitializeComponent();

            // update controls
            UpdateSettings();

            // initialize charts
            errorChart.AddDataSeries("error", Color.Red, Chart.SeriesType.Line, 1);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XorProblem));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.currentErrorBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.sigmoidTypeCombo = new System.Windows.Forms.ComboBox();
            this.errorLimitBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.alphaBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.learningRateBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.errorChart = new Accord.Controls.Chart();
            this.saveFilesCheck = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stopButton);
            this.groupBox1.Controls.Add(this.startButton);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.currentErrorBox);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.currentIterationBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.sigmoidTypeCombo);
            this.groupBox1.Controls.Add(this.errorLimitBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.alphaBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.learningRateBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 380);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Neural Network";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(176, 329);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(120, 33);
            this.stopButton.TabIndex = 28;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(40, 329);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(120, 33);
            this.startButton.TabIndex = 27;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(16, 308);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(280, 3);
            this.label5.TabIndex = 26;
            // 
            // currentErrorBox
            // 
            this.currentErrorBox.Location = new System.Drawing.Point(200, 270);
            this.currentErrorBox.Name = "currentErrorBox";
            this.currentErrorBox.ReadOnly = true;
            this.currentErrorBox.Size = new System.Drawing.Size(96, 26);
            this.currentErrorBox.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(16, 273);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(194, 21);
            this.label11.TabIndex = 24;
            this.label11.Text = "Current summary error:";
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(200, 234);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(96, 26);
            this.currentIterationBox.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(16, 237);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(157, 23);
            this.label8.TabIndex = 22;
            this.label8.Text = "Current iteration:";
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(16, 219);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(280, 3);
            this.label7.TabIndex = 21;
            // 
            // sigmoidTypeCombo
            // 
            this.sigmoidTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sigmoidTypeCombo.Items.AddRange(new object[] {
            "Unipolar",
            "Bipolar"});
            this.sigmoidTypeCombo.Location = new System.Drawing.Point(200, 140);
            this.sigmoidTypeCombo.Name = "sigmoidTypeCombo";
            this.sigmoidTypeCombo.Size = new System.Drawing.Size(96, 28);
            this.sigmoidTypeCombo.TabIndex = 9;
            // 
            // errorLimitBox
            // 
            this.errorLimitBox.Location = new System.Drawing.Point(200, 104);
            this.errorLimitBox.Name = "errorLimitBox";
            this.errorLimitBox.Size = new System.Drawing.Size(96, 26);
            this.errorLimitBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 22);
            this.label3.TabIndex = 6;
            this.label3.Text = "Learning error limit:";
            // 
            // alphaBox
            // 
            this.alphaBox.Location = new System.Drawing.Point(200, 67);
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size(96, 26);
            this.alphaBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 70);
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
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 22);
            this.label4.TabIndex = 8;
            this.label4.Text = "Sigmoid\'s type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.errorChart);
            this.groupBox2.Controls.Add(this.saveFilesCheck);
            this.groupBox2.Location = new System.Drawing.Point(344, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 380);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Error\'s dynamics";
            // 
            // errorChart
            // 
            this.errorChart.Location = new System.Drawing.Point(16, 29);
            this.errorChart.Name = "errorChart";
            this.errorChart.Size = new System.Drawing.Size(320, 300);
            this.errorChart.TabIndex = 2;
            this.errorChart.Text = "chart1";
            // 
            // saveFilesCheck
            // 
            this.saveFilesCheck.Location = new System.Drawing.Point(16, 341);
            this.saveFilesCheck.Name = "saveFilesCheck";
            this.saveFilesCheck.Size = new System.Drawing.Size(320, 26);
            this.saveFilesCheck.TabIndex = 1;
            this.saveFilesCheck.Text = "Save errors to files";
            // 
            // XorProblem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(708, 409);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "XorProblem";
            this.Text = "XOR Problem (Resilient Backpropagation)";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

       

        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback(System.Windows.Forms.Control control, string text);

        // Thread safe updating of control's text property
        private void SetText(System.Windows.Forms.Control control, string text)
        {
            if (control.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { control, text });
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
            learningRateBox.Text = initialStep.ToString();
            alphaBox.Text = sigmoidAlphaValue.ToString();
            errorLimitBox.Text = learningErrorLimit.ToString();
            sigmoidTypeCombo.SelectedIndex = sigmoidType;

            saveFilesCheck.Checked = saveStatisticsToFiles;
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback(bool enable);

        // Enable/disale controls (safe for threading)
        private void EnableControls(bool enable)
        {
            if (InvokeRequired)
            {
                EnableCallback d = new EnableCallback(EnableControls);
                Invoke(d, new object[] { enable });
            }
            else
            {
                learningRateBox.Enabled = enable;
                alphaBox.Enabled = enable;
                errorLimitBox.Enabled = enable;
                sigmoidTypeCombo.Enabled = enable;
                saveFilesCheck.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // On "Start" button click
        private void startButton_Click(object sender, System.EventArgs e)
        {
            // get learning rate
            try
            {
                initialStep = Math.Max(0.00001, Math.Min(1, double.Parse(learningRateBox.Text)));
            }
            catch
            {
                initialStep = 0.1;
            }
            // get sigmoid's alpha value
            try
            {
                sigmoidAlphaValue = Math.Max(0.01, Math.Min(100, double.Parse(alphaBox.Text)));
            }
            catch
            {
                sigmoidAlphaValue = 2;
            }
            // get learning error limit
            try
            {
                learningErrorLimit = Math.Max(0, double.Parse(errorLimitBox.Text));
            }
            catch
            {
                learningErrorLimit = 0.1;
            }
            // get sigmoid's type
            sigmoidType = sigmoidTypeCombo.SelectedIndex;

            saveStatisticsToFiles = saveFilesCheck.Checked;

            // update settings controls
            UpdateSettings();

            // disable all settings controls
            EnableControls(false);

            // run worker thread
            needToStop = false;
            workerThread = new Thread(new ThreadStart(SearchSolution));
            workerThread.Start();
        }

        // On "Stop" button click
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
            // initialize input and output values
            double[][] input = null;
            double[][] output = null;

            if (sigmoidType == 0)
            {
                // unipolar data
                input = new double[4][] {
                                            new double[] {0, 0},
                                            new double[] {0, 1},
                                            new double[] {1, 0},
                                            new double[] {1, 1}
                                        };
                output = new double[4][] {
                                             new double[] {0},
                                             new double[] {1},
                                             new double[] {1},
                                             new double[] {0}
                                         };
            }
            else
            {
                // bipolar data
                input = new double[4][] {
                                            new double[] {-1, -1},
                                            new double[] {-1,  1},
                                            new double[] { 1, -1},
                                            new double[] { 1,  1}
                                        };
                output = new double[4][] {
                                             new double[] {-1},
                                             new double[] { 1},
                                             new double[] { 1},
                                             new double[] {-1}
                                         };
            }

            // create neural network
            ActivationNetwork network = new ActivationNetwork(
                (sigmoidType == 0) ?
                    (IActivationFunction)new SigmoidFunction(sigmoidAlphaValue) :
                    (IActivationFunction)new BipolarSigmoidFunction(sigmoidAlphaValue),
                2, 2, 1);

            // create teacher
            var teacher = new ParallelResilientBackpropagationLearning(network);


            // set learning rate and momentum
            teacher.Reset(initialStep);


            // iterations
            int iteration = 0;

            // statistic files
            StreamWriter errorsFile = null;

            try
            {
                // check if we need to save statistics to files
                if (saveStatisticsToFiles)
                {
                    // open files
                    errorsFile = File.CreateText("errors.csv");
                }

                // erros list
                ArrayList errorsList = new ArrayList();

                // loop
                while (!needToStop)
                {
                    // run epoch of learning procedure
                    double error = teacher.RunEpoch(input, output);
                    errorsList.Add(error);

                    // save current error
                    if (errorsFile != null)
                    {
                        errorsFile.WriteLine(error);
                    }

                    // show current iteration & error
                    SetText(currentIterationBox, iteration.ToString());
                    SetText(currentErrorBox, error.ToString());
                    iteration++;

                    // check if we need to stop
                    if (error <= learningErrorLimit)
                        break;
                }

                // show error's dynamics
                double[,] errors = new double[errorsList.Count, 2];

                for (int i = 0, n = errorsList.Count; i < n; i++)
                {
                    errors[i, 0] = i;
                    errors[i, 1] = (double)errorsList[i];
                }

                errorChart.RangeX = new Range(0, errorsList.Count - 1);
                errorChart.UpdateDataSeries("error", errors);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed writing file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // close files
                if (errorsFile != null)
                    errorsFile.Close();
            }

            // enable settings controls
            EnableControls(true);
        }
    }
}
